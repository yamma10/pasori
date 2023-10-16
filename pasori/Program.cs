using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace pasori
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Status data = new Status();

                Application.Run(new mainForm(ref data));
            }
            catch
            {

            }
                    

            //音楽テスト
            //Application.Run(new Form1());


            //MessageBox.Show("hhhh");

            
        }
    }
}
