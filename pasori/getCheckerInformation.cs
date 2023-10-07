using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCSC;
using System.IO;

namespace pasori
{
    public partial class getCheckerInformation : Form
    {
        Status data;


        public getCheckerInformation(ref Status t)
        {
            InitializeComponent();
            data = t;
            
        }

        private  void getCheckerInformation_Load(object sender, EventArgs e)
        {

            


        }

        private void getCheckerInformation_Shown(object sender, EventArgs e)
        {
            
            this.Refresh();
            #region "リソースマネージャに接続してハンドルを取得する"
            //メモリブロックの先頭
            IntPtr hContext = IntPtr.Zero;

            uint ret = Api.SCardEstablishContext(Constant.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out hContext);
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

            #region "PCに接続されているNFCリーダを取得する。"

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
            var readerName = readerNameMultiString.Substring(0, nullindex);
            #endregion

            #region "接続されているNFCリーダを指定して、カードを接続する。30秒間、カードと接続できなかった場合、接続されるまでループする"

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            IntPtr hCard = IntPtr.Zero;
            IntPtr activeProtocol = IntPtr.Zero;

            while (stopWatch.ElapsedTicks < 300000000)
            {
                hCard = IntPtr.Zero;
                activeProtocol = IntPtr.Zero;
                ret = Api.SCardConnect(hContext, readerName, Constant.SCARD_SHARE_SHARED, Constant.SCARD_PROTOCOL_T1, ref hCard, ref activeProtocol);
                if (ret != Constant.SCARD_S_SUCCESS)
                {
                    continue;
                }
                else
                {
                    stopWatch.Stop();
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

                    ret = Api.SCardTransmit(hCard, pci, sendBUffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);

                    if (ret != Constant.SCARD_S_SUCCESS)
                    {
                        throw new ApplicationException("NFCカードへの送信に失敗しました " + ret);
                    }

                    //受信データからIDmを抽出する
                    string cardId = BitConverter.ToString(recvBuffer, 0, pcbRecvLength - 2);
                    //MessageBox.Show(cardId);


                    int j = 2;



                    #endregion
                    #endregion

                    //基本の部分のクエリを取得する
                    string query = new StreamReader(@"../../sql/getNameFromCardId.txt").ReadToEnd();

                    //読み取った情報に置き換える
                    //query = query.Replace("0116060016109B11", cardId);

                    check = interaction_Access.interaction(ref dt,query, const_Util.db1);

                    #endregion

                    if(dt.Rows.Count == 0)
                    {
                        MessageBox.Show("登録されていないユーザーです");
                        stopWatch.Start();
                    } else
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
                        stopWatch.Start();
                        break;
                    }
                }
            }

            stopWatch.Stop();
            if (stopWatch.ElapsedTicks >= 300000000)
            {
                MessageBox.Show("規定時間を超過したので初期画面に戻ります");
            }



            int i = 2;
            #endregion

           


            #region "カードから切断する"

            ret = Api.SCardDisconnect(hCard, Constant.SCARD_LEAVE_CARD);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードとの切断に失敗しました。 code = " + ret);
            }

            #endregion




            return;
        }


    }
}
