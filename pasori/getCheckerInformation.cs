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
            //try
            //{
            //    var readerNames = ContextFactory.Instance.Establish(SCardScope.System).GetReaders();
            //    //認識できた
            //    if (readerNames == null || readerNames.Length == 0 ) 
            //    {
            //        MessageBox.Show("カードリーダが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            //        Application.Exit();
            //    }

            //    using(var monitor = MonitorFactory.Instance.Create(SCardScope.System))
            //    {
            //        AttachToAllEvents(monitor); 
            //    }

            //}
            //catch (NoServiceException)
            //{
            //    MessageBox.Show("", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            //    Application.Exit();
            //}

            //メモリブロックの先頭
            IntPtr hContext = IntPtr.Zero;

            uint ret = Api.SCa


        }

        

        
    }
}
