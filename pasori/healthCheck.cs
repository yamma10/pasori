using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pasori
{
    public partial class healthCheck : Form
    {
        public healthCheck()
        {
            InitializeComponent();
        }

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }

        private void healthCheck_Shown(object sender, EventArgs e)
        {

        }
    }
}
