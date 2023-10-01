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
    
    public partial class startPage : Form
    {
        Status data;
        public startPage(ref Status t)
        {
            InitializeComponent();
            data = t;
        }

        

        //問題なしの場合
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.before.Checked == true)
            {
                data.propertyDriveStatus = false;
            } else
            {
                data.propertyDriveStatus = true;
            }

            data.propertyAlcoholFlag = false;

            this.Close();
        }

        //問題ありの場合
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.before.Checked == true)
            {
                data.propertyDriveStatus = false;
            }
            else
            {
                data.propertyDriveStatus = true;
            }

            data.propertyAlcoholFlag = true;

            this.Close();
        }
        
    }
}
