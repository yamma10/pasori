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
        public mainForm(ref Status d)
        {
            InitializeComponent();
            data = d;
            dt = new DataTable();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {

        }

        //1 場所を選択すると、車種が出る
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.SelectedValue.ToString());
            if(comboBox1.Items[comboBox1.SelectedIndex] != "その他(自家用車)")
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(@"../../sql/getAllCarsByPlace.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("院内", comboBox1.Items[comboBox1.SelectedIndex].ToString());

                

                bool check = interaction_Access.interaction(ref dt, query, const_Util.alc);

                int i = 0;
                while(i < dt.Rows.Count)
                {
                    comboBox2.Items.Add(dt.Rows[i].ItemArray[1].ToString());
                    i++;
                }
            }
        }

        //1 異常なしボタンクリック
        private void okButton_Click(object sender, EventArgs e)
        {
            //車種を選択していなかったら
            if(comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("利用する車を選択してください");
            }
            if (before.Checked)
            {
                data.propertyDriveStatus = false;
            } else
            {
                data.propertyDriveStatus = true;
            }
            
            data.propertyCarNumber = dt.Rows[comboBox2.SelectedIndex].ItemArray[0].ToString();
            data.propertyAlcoholFlag = false;

            //カードを読み取るTabに移動する
            mainTab.SelectedTab = getCheckerInformation;
        }

        //1 異常ありボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {
            data.propertyAlcoholFlag = true;
            mainTab.SelectedTab = getCheckerInformation;
        }

        //タブが切り替わる際に起こるイベント
        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTab.SelectedTab == getCheckerInformation)
            {
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("hello");
            timer1.Enabled = false;
        }

        //2 次のページに進む
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
           
        }

        //2 職員ｺｰﾄﾞから職員名を取得する
        private void textBox1_Leave(object sender, EventArgs e)
        {

        }
        //2 クリアがクリックされた場合
        private void button3_Click(object sender, EventArgs e)
        {

        }

        //3 クリアがクリックされた場合
        private void button7_Click(object sender, EventArgs e)
        {

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
        private void textBox3_Enter(object sender, EventArgs e)
        {

        }

        //3 次へをクリック
        private void button4_Click(object sender, EventArgs e)
        {

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

        

        
    }
}
