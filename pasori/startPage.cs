using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace pasori
{
    
    public partial class startPage : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
