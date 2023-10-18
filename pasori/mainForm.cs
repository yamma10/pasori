using PCSC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pasori
{
    public partial class mainForm : Form
    {
        Status data;
        DataTable dt;
        
        List<Car> carList;
        Car selectedCar = null;
        string category = "";

        #region "pasoriAPIで使用する変数の定義"
        uint ret;
        IntPtr hContext;
        string readerName;
        #endregion

        public mainForm(ref Status d)
        {
            InitializeComponent();
            data = d;
            dt = new DataTable();
        }

        #region"1"
        private void mainForm_Load(object sender, EventArgs e)
        {
            #region "車を取ってくる"
            //基本の部分のクエリを取得する
            string query = new StreamReader(System.IO.Directory.GetCurrentDirectory() + @"/sql/getAllCarsByPlace.txt").ReadToEnd();
            //string query = new StreamReader(@"System.IO.Directory.GetCurrentDiegetAllCarsByPlace.txt").ReadToEnd();

            //読み取った情報に置き換える
            query = query.Replace("where 場所='院内'", "");

            bool check =  interaction_Access.Interaction(ref dt, query, const_Util.alc);

            carList = new List<Car>();

            int i = 0;
            //車を記載するボタンが16から27までなので
            int num = 16;
            while (i < dt.Rows.Count && num <= 27)
            {
                Car tmpCar = new Car();

                tmpCar.category = dt.Rows[i].ItemArray[2].ToString();
                tmpCar.name = dt.Rows[i].ItemArray[1].ToString();
                tmpCar.number = dt.Rows[i].ItemArray[0].ToString();

                Control[] cs = this.Controls.Find("button" + num.ToString(), true);

                if (cs.Length > 0)
                {
                    ((Button)cs[0]).Text += dt.Rows[i].ItemArray[1].ToString() + "\r\n";
                    ((Button)cs[0]).Text += dt.Rows[i].ItemArray[0].ToString();
                }

                carList.Add(tmpCar);

                num++;
                i++;
            }

            //loc.Add("訪問看護", visit);
            //loc.Add("院内", inner);
            //loc.Add("デイケア", daycare);

            dt.Clear();
            #endregion

            #region "pasori リソースマネージャに接続してハンドルを取得する"
            //メモリブロックの先頭
            hContext = IntPtr.Zero;

            ret = Api.SCardEstablishContext(Constant.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                string message;
                switch (ret)
                {
                    case Constant.SCARD_E_NO_SERVICE:
                        message = "サービスが起動されていません。";
                        break;
                    default:
                        message = "サービスに接続できません。 code = " + ret;
                        break;
                }
                throw new ApplicationException(message);
            }

            if (hContext == IntPtr.Zero)
            {
                throw new ApplicationException("コンテキストの取得に失敗しました");
            }
            #endregion

            #region "pasori PCに接続されているNFCリーダを取得する。"

            uint pcchReaders = 0;

            //NFCリーダの文字列バッファのサイズを取得
            ret = Api.SCardListReaders(hContext, null, null, ref pcchReaders);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                //検出失敗
                throw new ApplicationException("NFCリーダを確認できません");
            }

            //NFCリーダの文字列を取得
            byte[] mszReaders = new byte[pcchReaders * 2]; //1文字2byte
            ret = Api.SCardListReaders(hContext, null, mszReaders, ref pcchReaders);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                //検出失敗
                throw new ApplicationException("NFCリーダの取得に失敗しました");
            }

            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            //byte列をエンコードしてStringを返す
            string readerNameMultiString = unicodeEncoding.GetString(mszReaders);

            //認識したNDCリーダの最初の一台を使用する
            int nullindex = readerNameMultiString.IndexOf((char)0);
            readerName = readerNameMultiString.Substring(0, nullindex);
            #endregion
        }
        #region "車種選択"

        //1 車種選択_病院クリック
        private void button12_Click(object sender, EventArgs e)
        {
            category = button12.Text;
            int num = 16;
            while(num <= 27)
            {
                Control[] cs = this.Controls.Find("button" + num.ToString(), true);

                if (cs.Length > 0)
                {
                    if(((Button)cs[0]).Text == "")
                    {
                        return;
                    }
                    carList.ForEach(item =>
                    {
                        if(((Button)cs[0]).Text.Contains(item.name) && ((Button)cs[0]).Text.Contains(item.number))
                        {
                            if (item.category == "院内")
                            {
                                ((Button)cs[0]).Enabled = true;
                            }
                            else
                            {
                                ((Button)cs[0]).Enabled = false;
                            }
                        }
                        
                    });
                }
                num++;
            }
        }

        //1 車種選択_デイケアクリック
        private void button13_Click(object sender, EventArgs e)
        {
            category = button13.Text;
            int num = 16;
            while (num <= 27)
            {
                Control[] cs = this.Controls.Find("button" + num.ToString(), true);

                if (cs.Length > 0)
                {
                    if (((Button)cs[0]).Text == "")
                    {
                        return;
                    }
                    carList.ForEach(item =>
                    {
                        if (((Button)cs[0]).Text.Contains(item.name) && ((Button)cs[0]).Text.Contains(item.number))
                        {
                            if (item.category == "デイケア")
                            {
                                ((Button)cs[0]).Enabled = true;
                            }
                            else
                            {
                                ((Button)cs[0]).Enabled = false;
                            }
                        }

                    });
                }
                num++;
            }
        }
        //1 車種選択_訪問看護クリック
        private void button14_Click(object sender, EventArgs e)
        {
            category = button14.Text;
            int num = 16;
            while (num <= 27)
            {
                Control[] cs = this.Controls.Find("button" + num.ToString(), true);

                if (cs.Length > 0)
                {
                    if (((Button)cs[0]).Text == "")
                    {
                        return;
                    }
                    carList.ForEach(item =>
                    {
                        if (((Button)cs[0]).Text.Contains(item.name) && ((Button)cs[0]).Text.Contains(item.number))
                        {
                            if (item.category == "訪問看護")
                            {
                                ((Button)cs[0]).Enabled = true;
                            }
                            else
                            {
                                ((Button)cs[0]).Enabled = false;
                            }
                        }

                    });
                }
                num++;
            }
        }

        //1 車種選択_その他クリック
        private void button15_Click(object sender, EventArgs e)
        {
            category = button15.Text;
            int num = 16;
            while (num <= 27)
            {
                Control[] cs = this.Controls.Find("button" + num.ToString(), true);

                if (cs.Length > 0)
                {
                    ((Button)cs[0]).Enabled = false;
                }
                num++;
            }
        }

        #endregion

        //1 車種を選択時にPublicにそれを保存する
        private void carSelectedEvent(object sender, EventArgs e)
        {
            carList.ForEach(item =>
            {
                if(((Button)sender).Text.Contains(item.name) && ((Button)sender).Text.Contains(item.number))
                {
                    selectedCar = item;
                }

            });
        }

       
        //1 異常なしボタンクリック
        private void okButton_Click(object sender, EventArgs e)
        {

            if (selectedCar == null)
            {
                //カテゴリーがその他の場合、車種の選択はいらない
                if (category != "その他")
                {
                    MessageBox.Show("車種が選択されていません");
                    return;
                }
                data.propertyCarNumber = "その他";
            } else
            {
                data.propertyCarName = selectedCar.name;
                data.propertyCarNumber = selectedCar.number;
            }

            if (before.Checked)
            {
                data.propertyDriveStatus = false;
            }
            else
            {
                data.propertyDriveStatus = true;
            }
            data.propertyAlcoholFlag = false;

            //カードを読み取るTabに移動する
            mainTab.SelectedTab = getCheckerInformation;

        }

        //1 異常ありボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {



            //カードを読み取るTabに移動する
            data.propertyAlcoholFlag = true;
            mainTab.SelectedTab = getCheckerInformation;

            dt.Clear();
        }

        #endregion
        //タブが切り替わる際に起こるイベント
        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTab.SelectedTab == getCheckerInformation || mainTab.SelectedTab == getWitnessInformation)
            {
                readInformation.Enabled = true;
            } else if(mainTab.SelectedTab == confirmTab)
            {
                textBox6.Text = "";
                textBox6.Text = textBox6.Text + "対象者No: " + data.propertyCode + "\r\n";
                textBox6.Text = textBox6.Text + "対象者名: " + data.propertyName + "\r\n";
                if (data.propertyAlcoholFlag)
                {
                    textBox6.Text = textBox6.Text + "アルコールチェック: 異常あり\r\n";
                }
                else
                {
                    textBox6.Text = textBox6.Text + "確認者No: " + data.propertyWitnessCode + "\r\n";
                    textBox6.Text = textBox6.Text + "確認者名: " + data.propertyWitnessName + "\r\n";
                    textBox6.Text = textBox6.Text + "アルコールチェック: 異常なし\r\n";
                    //確認方法
                    if (data.propertyCheckMethod)
                    {
                        //電話
                        textBox6.Text = textBox6.Text + "確認方法: 電話\r\n";
                    }
                    else
                    {
                        //対面
                        textBox6.Text = textBox6.Text + "確認方法: 対面\r\n";
                    }
                    //体調
                    if (data.propertyPhysicalCondition)
                    {
                        textBox6.Text = textBox6.Text + "体調: 悪い" + "\r\n";
                        textBox6.Text = textBox6.Text + "コメント\r\n" + data.propertyComment + "\r\n";
                    }
                    else
                    {
                        textBox6.Text = textBox6.Text + "体調: 良い" + "\r\n";
                    }

                    if (data.propertyDriveStatus)
                    {
                        textBox6.Text = textBox6.Text + "状態: 運転後" + "\r\n";
                    }
                    else
                    {
                        textBox6.Text = textBox6.Text + "状態: 運転前" + "\r\n";
                    }
                    textBox6.Text = textBox6.Text + "車ナンバー: " + data.propertyCarNumber + "\r\n";


                }
            } else if (mainTab.SelectedTab == lastTab)
            {
                toFirstTab.Enabled = true;
            }
            


        }

        #region "タイマーイベント"
        private void readInformation_Tick(object sender, EventArgs e)
        {
            //readInformation.Enabled = false;
            //MessageBox.Show("hello");
            #region "接続されているNFCリーダを指定して、カードを接続する。30秒間、カードと接続できなかった場合、接続されるまでループする"

            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            IntPtr hCard = IntPtr.Zero;
            IntPtr activeProtocol = IntPtr.Zero;
            hCard = IntPtr.Zero;
            activeProtocol = IntPtr.Zero;
            var tmpret = Api.SCardConnect(hContext, readerName, Constant.SCARD_SHARE_SHARED, Constant.SCARD_PROTOCOL_T1, ref hCard, ref activeProtocol);

            if (tmpret != Constant.SCARD_S_SUCCESS)
            {
                return;
            }
            else
            {
                readInformation.Enabled = false ;
                //stopWatch.Stop();
                #region "DBとやりとりする"

                DataTable dt = new DataTable();
                Boolean check = false;
                #region "接続"
                #region "接続したカードにコマンドを送信し、結果を受信する。ここでIDmを取得したり、カードに保存されている情報を読み取る"
                uint maxRecvDataLen = 256;
                var recvBuffer = new byte[maxRecvDataLen + 2];
                var sendBUffer = new byte[] { 0xff, 0xca, 0x00, 0x00, 0x00 }; //IDmを取得

                Api.SCARD_IO_REQUEST ioRecv = new Api.SCARD_IO_REQUEST();
                ioRecv.cbPciLength = recvBuffer.Length;

                int pcbRecvLength = recvBuffer.Length;
                int cbSendLength = sendBUffer.Length;

                IntPtr handle = Api.LoadLibrary("winscard.dll");
                IntPtr pci = Api.GetProcAddress(handle, "g_rgCardT1Pci");
                Api.FreeLibrary(handle);

                tmpret = Api.SCardTransmit(hCard, pci, sendBUffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);

                if (tmpret != Constant.SCARD_S_SUCCESS)
                {
                    throw new ApplicationException("NFCカードへの送信に失敗しました " + ret);
                }

                //受信データからIDmを抽出する
                string cardId = BitConverter.ToString(recvBuffer, 0, pcbRecvLength - 2);
                //MessageBox.Show(cardId);



                #endregion
                #endregion

                //基本の部分のクエリを取得する
                string query = new StreamReader(System.IO.Directory.GetCurrentDirectory() + @"/sql/getNameFromCardId.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("0116060016109B11", cardId);
                query = query.Replace("-", "");

                check = interaction_Access.Interaction(ref dt, query, const_Util.db1);

                #endregion

                if (dt.Rows.Count == 0)
                {
                    //警備員のカードをよみとることがあるから確認
                    if(mainTab.SelectedTab == getWitnessInformation)
                    {
                        
                        query = new StreamReader(System.IO.Directory.GetCurrentDirectory() + @"/sql/guardmanOrNot.txt").ReadToEnd();
                        //読み取った情報に置き換える
                        query = query.Replace("0116060016109B11", cardId);
                        query = query.Replace("-", "");
                        check = interaction_Access.Interaction(ref dt, query, const_Util.alc);
                        if(dt.Rows.Count == 0)
                        {
                            MessageBox.Show("登録されていないユーザーです");
                            readInformation.Enabled = true;
                            return;
                        }
                    } else
                    {
                        MessageBox.Show("登録されていないユーザーです");
                        readInformation.Enabled = true;
                        return;
                    }
                }
                else
                {
                    if(mainTab.SelectedTab == getWitnessInformation)
                    {
                        //確認者と対象者の名前が一緒ではダメ
                        if(data.propertyName == dt.Rows[0].ItemArray[1].ToString())
                        {
                            MessageBox.Show("確認者と対象者が同一人物です");
                            readInformation.Enabled = true;
                            return;
                        }
                        
                        
                    }

                    data.propertyCode = dt.Rows[0].ItemArray[0].ToString();
                    data.propertyName = dt.Rows[0].ItemArray[1].ToString();
                    if (cardId != "")
                    {
                        System.IO.Stream stream = Properties.Resources.coin;

                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);

                        player.PlaySync();

                        player.Dispose();
                    }

                    if (data.propertyAlcoholFlag)
                    {
                        mainTab.SelectedTab = confirmTab;
                    }
                    else
                    {
                        mainTab.SelectedTab = healthCheck;
                        //readInformation.Enabled = true;
                    }

                }
            }
           
            #endregion
        }
        private void toFirstTab_Tick(object sender, EventArgs e)
        {


            //データ,テキストボックス、selectedCar、categoryを初期化する
            data = new Status();

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            selectedCar = null;
            category = "";
            radioButton1.Checked = true;
            before.Checked = true;

            //最初のページの車種選択のボタンをenabledにする
            int i = 0;
            //車を記載するボタンが16から27までなので
            int num = 16;
            while (i < carList.Count && num <= 27)
            {
                Control[] cs = this.Controls.Find("button" + num.ToString(), true);

                if (cs.Length > 0)
                {
                    ((Button)cs[0]).Enabled = false;
                }

                num++;
                i++;
            }

            toFirstTab.Enabled = false;
            mainTab.SelectedTab = firstTab;
        }

        #endregion

        #region "2"
        //2 次のページに進む
        private void button1_Click(object sender, EventArgs e)
        {
            readInformation.Enabled = false;
            if(textBox1.Text !="" && textBox2.Text != "")
            {
                if(data.propertyAlcoholFlag)
                {
                    data.propertyCode = textBox1.Text;
                    data.propertyName = textBox2.Text;
                    mainTab.SelectedTab = confirmTab;
                } else
                {
                    data.propertyCode = textBox1.Text;
                    data.propertyName = textBox2.Text;
                    mainTab.SelectedTab = healthCheck;
                }
                
            } else
            {
                MessageBox.Show("カードを読み取るか、職員ｺｰﾄﾞを入力してください");
            }
            
        }

        //2　前のページに戻る
        private void checkerInformation_back_Click(object sender, EventArgs e)
        {
            //タイマーイベントを止める
            readInformation.Enabled = true;
            mainTab.SelectedTab = firstTab;
        }

        //2 職員ｺｰﾄﾞから職員名を取得する
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            dt = new DataTable();
            if(e.KeyChar == (char)Keys.Enter)
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(System.IO.Directory.GetCurrentDirectory() + @"/sql/getNameByCode.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("1086", textBox1.Text);



                bool check = interaction_Access.Interaction(ref dt, query, const_Util.db1);



                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("ユーザーが存在しません");
                    textBox1.Text = "";
                    return;
                }

                textBox2.Text = dt.Rows[0].ItemArray[0].ToString();
            }

            dt.Clear();
        }

        //2 クリアがクリックされた場合
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        #endregion

        #region "3"
        //3 クリアがクリックされた場合
        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox4.Text = "";
        }


        //3 異常なしがクリックされた場合
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel18.Visible = false;
        }

        //3 異常ありがクリックされた場合
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel18.Visible = true;

        }
        //3 総務１クリック
        private void button5_Click(object sender, EventArgs e)
        {
            textBox3.Text = const_Util.soumuCode1;
            textBox4.Text = const_Util.soumu1;
        }

        //3 総務２ クリック
        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = const_Util.soumuCode2;
            textBox4.Text = const_Util.soumu2;
        }

        //3 職員ｺｰﾄﾞから名前取得
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Enterが押された場合
            if (e.KeyChar == (char)Keys.Enter)
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(System.IO.Directory.GetCurrentDirectory() + @"/sql/getNameByCode.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("1086", textBox3.Text);



                bool check = interaction_Access.Interaction(ref dt, query, const_Util.db1);



                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("ユーザーが存在しません");
                    textBox3.Text = "";
                    return;
                }

                textBox4.Text = dt.Rows[0].ItemArray[3].ToString();
            }

            dt.Clear();
        }

        

        //3 顔色が悪いをクリック
        private void button8_Click(object sender, EventArgs e)
        {
            textBox5.Text = textBox5.Text + "顔色が悪い\r\n";
            data.propertyComplexion = true;
        }

        //3 熱があるをクリック
        private void button9_Click(object sender, EventArgs e)
        {
            textBox5.Text = textBox5.Text + "熱がある\r\n";
        }

        //3 外傷があるをクリック
        private void button10_Click(object sender, EventArgs e)
        {
            textBox5.Text = textBox5.Text +  "外傷がある\r\n";
        }

        //3 コメント欄をクリアする
        private void button11_Click(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }

        //3 次へをクリック
        private void button4_Click(object sender, EventArgs e)
        {
            //異常なしの場合
            if (radioButton1.Checked == true)
            {

                

                data.propertyPhysicalCondition = false;
                

                //立会人が入力されていない場合
                if (textBox3.Text == "" || textBox4.Text == "")
                {
                    
                    //立会人のカードを読み取るTabに移動する
                    mainTab.SelectedTab = getWitnessInformation;
                    return;
                } else if(textBox4.Text == data.propertyName)
                {
                    MessageBox.Show("確認者と対象者が同一人物です");
                    return;
                }

                //コードで入力された場合、対面ではないのでフラグを立てる
                data.propertyCheckMethod = true;
                //名前と職員ｺｰﾄﾞを保存する
                data.propertyWitnessCode = textBox3.Text;
                data.propertyWitnessName = textBox4.Text;
                //確認Tabに移動する
                mainTab.SelectedTab = confirmTab;
            }
            else
            {
                //立会人が入力されていない場合
                if (textBox3.Text == "" || textBox4.Text == "")
                {
                    //立会人のカードを読み取るTabに移動する
                    mainTab.SelectedTab = getWitnessInformation;
                    return;
                }else if (textBox4.Text == data.propertyName)
                {
                    MessageBox.Show("確認者と対象者が同一人物です");
                    return;
                }

                //異常があるのにコメントが入力されていない場合
                if (textBox5.Text == "")
                {
                    MessageBox.Show("症状を入力してください");
                    return;
                }
                
                data.propertyPhysicalCondition = true;
                //名前と職員ｺｰﾄﾞを保存する
                data.propertyWitnessCode = textBox3.Text;
                data.propertyWitnessName = textBox4.Text;
                //コメントの保存
                data.propertyComment = textBox5.Text;

                

                //コードで入力された場合、対面ではないのでフラグを立てる
                data.propertyCheckMethod = true;
                //確認Tabに移動する
                mainTab.SelectedTab = confirmTab;
            }
        }

        //3 戻る
        private void healthCheck_backPage_Click(object sender, EventArgs e)
        {
            readInformation.Enabled = true;
            mainTab.SelectedTab = getCheckerInformation;
        }

        #endregion

        //確認者のカード読み取りのところで「戻る」を押した場合
        private void getWitness_backPage_Click(object sender, EventArgs e)
        {
            readInformation.Enabled = false;
            mainTab.SelectedTab = healthCheck;
        }


        //確認ページで「戻る」ボタンを押した場合

        private void confirm_backPage_Click(object sender, EventArgs e)
        {
            readInformation.Enabled = true;
            //アルコールチェックで異常がある場合、getCheckerInformationに戻る
            if (data.propertyAlcoholFlag)
            {
                mainTab.SelectedTab = getCheckerInformation;
            } else
            {
                mainTab.SelectedTab = healthCheck;
            }
            
        }

        //確認ページで「登録する」ボタンを押した場合

        private void confirm_register_Click(object sender, EventArgs e)
        {

            #region "登録処理

            string query;
            //アルコールフラグが立っていれば対象者No,確認日時,対象者名,確認フラグのみ
            if (data.propertyAlcoholFlag)
            {
                query = replaceSQL.detect(data);
            } else
            {
                query = replaceSQL.didntDetect(data);
            }
            //
            interaction_Access.Interaction(ref dt, query, const_Util.alc);


            #endregion
            mainTab.SelectedTab = lastTab;
        }

        
    }
}
