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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Status data = new Status();

            //音楽テスト
            //Application.Run(new Form1());

            Application.Run(new startPage(ref data));
            Application.Run(new getCheckerInformation(ref data));
            
            //最初のページでアルコール反応が出ていたら終わり
            if(data.propertyAlcoholFlag)
            {
                Application.Run(new endPage(ref data));
            } else
            {
                
            }
        }
    }
}
