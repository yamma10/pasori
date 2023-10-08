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
            while(true)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Status data = new Status();

                Application.Run(new mainForm(ref data));

                //音楽テスト
                //Application.Run(new Form1());

                Application.Run(new startPage(ref data));
                Application.Run(new getCheckerInformation(ref data));

                //最初のページでアルコール反応が出ていたら終わり
                if (data.propertyAlcoholFlag)
                {
                    Application.Run(new endPage(ref data));
                    continue;
                }

                Application.Run(new healthCheck(ref data));

                Application.Run(new getWitnessInformation(ref data));

            }
            
        }
    }
}
