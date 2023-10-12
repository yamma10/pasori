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
        Status data;
        public healthCheck(ref Status d)
        {
            InitializeComponent();
            data = d;
        }

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }

        private void healthCheck_Shown(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            data.propertyPhysicalCondition = true;
            this.Close();
        }
    }
}
