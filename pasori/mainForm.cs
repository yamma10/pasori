﻿using PCSC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pasori
{
    public partial class mainForm : Form
    {
        Status data;
        DataTable dt;
        Dictionary<string, string> car;
        Dictionary<string, string> inner = new Dictionary<string, string>();
        Dictionary<string, string> visit = new Dictionary<string, string>();
        Dictionary<string, string> daycare = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> loc = new Dictionary<string, Dictionary<string, string>>();

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

        private void mainForm_Load(object sender, EventArgs e)
        {
            #region "車を取ってくる"
            //基本の部分のクエリを取得する
            string query = new StreamReader(@"../../sql/getAllCarsByPlace.txt").ReadToEnd();

            //読み取った情報に置き換える
            query = query.Replace("where 場所='院内'", "");

            bool check =  interaction_Access.Interaction(ref dt, query, const_Util.alc);

            int i = 0;
            while (i < dt.Rows.Count)
            {
                string tmp = dt.Rows[i].ItemArray[2].ToString();
                switch(tmp)
                {
                    case "訪問看護":
                        visit.Add(dt.Rows[i].ItemArray[1].ToString(),dt.Rows[i].ItemArray[0].ToString());
                        break;
                    case "院内":
                        inner.Add(dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[0].ToString());
                        break;
                    case "デイケア":
                        daycare.Add(dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[0].ToString());
                        break;
                }
                i++;
            }

            loc.Add("訪問看護", visit);
            loc.Add("院内", inner);
            loc.Add("デイケア", daycare);

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

        //1 場所を選択すると、車種が出る
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dt = new DataTable();
            comboBox2.Text = "";

            //car = new Dictionary<string, string>();
            comboBox2.Items.Clear();
            //MessageBox.Show(comboBox1.SelectedValue.ToString());
            if (comboBox1.Items[comboBox1.SelectedIndex] != "その他(自家用車)")
            {
                foreach (var n in loc[comboBox1.SelectedItem.ToString()])
                {
                    comboBox2.Items.Add(n.Key);
                }
            }
            

            
        }

        

        //1 異常なしボタンクリック
        private void okButton_Click(object sender, EventArgs e)
        {
            
            //車種を選択していなかったら
            if(comboBox2.SelectedIndex == -1)
            {
                if( comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("利用する車を選択してください");
                    return; 
                } else if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
                {
                    MessageBox.Show("利用する車を選択してください");
                    return;
                }
                
            }
            if (before.Checked)
            {
                data.propertyDriveStatus = false;
            } else
            {
                data.propertyDriveStatus = true;
            }
            data.propertyCarName = comboBox2.SelectedItem.ToString();
            if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
            {
                data.propertyCarNumber = loc[comboBox1.SelectedItem.ToString()][comboBox2.SelectedItem.ToString()];
            }
            
            data.propertyAlcoholFlag = false;

            //カードを読み取るTabに移動する
            mainTab.SelectedTab = getCheckerInformation;

            dt.Clear();
        }

        //1 異常ありボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {
            //車種を選択していなかったら
            if (comboBox2.SelectedIndex == -1)
            {
                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("利用する車を選択してください");
                    return;
                } else if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
                {
                    MessageBox.Show("利用する車を選択してください");
                    return;
                }

            }
            if (before.Checked)
            {
                data.propertyDriveStatus = false;
            }
            else
            {
                data.propertyDriveStatus = true;
            }
            data.propertyCarName = comboBox2.SelectedItem.ToString();
            if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
            {
                data.propertyCarNumber = loc[comboBox1.SelectedItem.ToString()][comboBox2.SelectedItem.ToString()];
            }


            //カードを読み取るTabに移動する
            data.propertyAlcoholFlag = true;
            mainTab.SelectedTab = getCheckerInformation;

            dt.Clear();
        }

        //タブが切り替わる際に起こるイベント
        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTab.SelectedTab == getCheckerInformation || mainTab.SelectedTab == getWitnessInformation)
            {
                readInformation.Enabled = true;
            }

            if (mainTab.SelectedTab == confirmTab)
            {
                textBox6.Text = "";
                if(data.propertyAlcoholFlag)
                {
                    textBox6.Text = textBox6.Text + "検出者名: " + data.propertyName + "\r\n";
                    textBox6.Text = textBox6.Text + "アルコールチェック: 異常あり\r\n";
                } else
                {
                    if(data.propertyDriveStatus)
                    {
                        textBox6.Text = textBox6.Text + "状態: 運転後" + "\r\n";
                    } else
                    {
                        textBox6.Text = textBox6.Text + "状態: 運転前" + "\r\n";
                    }
                    
                    textBox6.Text = textBox6.Text + "運転者名: " + data.propertyName + "\r\n";
                    textBox6.Text = textBox6.Text + "車ナンバー: " + data.propertyCarNumber + "\r\n";
                    textBox6.Text = textBox6.Text + "車種: " + data.propertyCarName + "\r\n";
                    textBox6.Text = textBox6.Text + "立会人: " + data.propertyWitnessName + "\r\n";
                    if(data.propertyPhysicalCondition)
                    {
                        textBox6.Text = textBox6.Text + "体調: 悪い" + "\r\n";
                        textBox6.Text = textBox6.Text + "コメント:\r\n" + data.propertyComment + "\r\n";
                    } else
                    {
                        textBox6.Text = textBox6.Text + "体調: 良い" + "\r\n";
                    }
                    
                }
                
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
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
                string query = new StreamReader(@"../../sql/getNameFromCardId.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("0116060016109B11", cardId);
                query = query.Replace("-", "");

                check = interaction_Access.Interaction(ref dt, query, const_Util.db1);

                #endregion

                if (dt.Rows.Count == 0)
                {

                    MessageBox.Show("登録されていないユーザーです");
                    readInformation.Enabled = true;
                    return;
                }
                else
                {
                    data.propertyCode = dt.Rows[0].ItemArray[0].ToString();
                    data.propertyName = dt.Rows[0].ItemArray[1].ToString();
                    if (cardId != "")
                    {
                        System.IO.Stream stream = Properties.Resources.coin;

                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);

                        player.PlaySync();

                        player.Dispose();
                    }

                    mainTab.SelectedTab = healthCheck;
                    
                }
            }
           
            #endregion
        }

        //2 次のページに進む
        private void button1_Click(object sender, EventArgs e)
        {
            readInformation.Enabled = false;
            if(textBox1.Text !="" && textBox2.Text != "")
            {
                if(data.propertyAlcoholFlag)
                {
                    data.propertyName = textBox2.Text;
                    mainTab.SelectedTab = confirmTab;
                } else
                {
                    mainTab.SelectedTab = healthCheck;
                    data.propertyName = textBox2.Text;
                    //readInformation.Enabled = true;
                }
                
            } else
            {
                MessageBox.Show("カードを読み取るか、職員ｺｰﾄﾞを入力してください");
            }
            
        }

        //2　前のページに戻る
        private void checkerInformation_back_Click(object sender, EventArgs e)
        {
            mainTab.SelectedTab = firstTab;
        }

        //2 職員ｺｰﾄﾞから職員名を取得する
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            dt = new DataTable();
            if(e.KeyChar == (char)Keys.Enter)
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(@"../../sql/getNameByCode.txt").ReadToEnd();

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
        //3 白津さんクリック
        private void button5_Click(object sender, EventArgs e)
        {
            textBox3.Text = "1055";
            textBox4.Text = "白津 美夏";
        }

        //3 加川さんクリック
        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = "798";
            textBox4.Text = "加川 智子";
        }

        //3 職員ｺｰﾄﾞから名前取得
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Enterが押された場合
            if (e.KeyChar == (char)Keys.Enter)
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(@"../../sql/getNameByCode.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("1086", textBox3.Text);



                bool check = interaction_Access.Interaction(ref dt, query, const_Util.db1);



                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("ユーザーが存在しません");
                    textBox3.Text = "";
                    return;
                }

                textBox4.Text = dt.Rows[0].ItemArray[0].ToString();
            }

            dt.Clear();
        }

        

        //3 顔色が悪いをクリック
        private void button8_Click(object sender, EventArgs e)
        {
            textBox5.Text = textBox5.Text + "顔色が悪い\r\n";
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
                data.propertyWitnessName = textBox4.Text;

                //立会人が入力されていない場合
                if (textBox3.Text == "" || textBox4.Text == "")
                {
                    //立会人のカードを読み取るTabに移動する
                    mainTab.SelectedTab = getWitnessInformation;
                }

                //立会人のカードを読み取るTabに移動する
                mainTab.SelectedTab = confirmTab;
            }
            else
            {
                //異常があるのにコメントが入力されていない場合
                if (textBox5.Text == "")
                {
                    MessageBox.Show("症状を入力してください");
                    return;
                }
                
                data.propertyPhysicalCondition = true;
                data.propertyWitnessName = textBox4.Text;
                data.propertyComment = textBox5.Text;

                //立会人が入力されていない場合
                if (textBox3.Text == "" || textBox4.Text == "")
                {
                    //立会人のカードを読み取るTabに移動する
                    mainTab.SelectedTab = getWitnessInformation;

                }

                //立会人のカードを読み取るTabに移動する
                mainTab.SelectedTab = confirmTab;
            }
        }

        

        //確認ページで「戻る」ボタンを押した場合

        private void confirm_backPage_Click(object sender, EventArgs e)
        {

        }

        //確認ページで「登録する」ボタンを押した場合

        private void confirm_register_Click(object sender, EventArgs e)
        {
            mainTab.SelectedTab = lastTab;
        }

        
    }
}
