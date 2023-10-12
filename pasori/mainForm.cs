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
        Dictionary<string, string> car;
        Dictionary<string, string> inner = new Dictionary<string, string>();
        Dictionary<string, string> visit = new Dictionary<string, string>();
        Dictionary<string, string> daycare = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> loc = new Dictionary<string, Dictionary<string, string>>();
        public mainForm(ref Status d)
        {
            InitializeComponent();
            data = d;
            dt = new DataTable();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            //基本の部分のクエリを取得する
            string query = new StreamReader(@"../../sql/getAllCarsByPlace.txt").ReadToEnd();

            //読み取った情報に置き換える
            query = query.Replace("where 場所='院内'", "");

            bool check = interaction_Access.interaction(ref dt, query, const_Util.alc);

            int i = 0;
            while (i < dt.Rows.Count)
            {
                string tmp = dt.Rows[i].ItemArray[2].ToString();
                switch(tmp)
                {
                    case "訪問看護":
                        visit.Add(dt.Rows[i].ItemArray[1].ToString(),dt.Rows[i].ItemArray[0].ToString());
                        break;
                    case "院内":
                        inner.Add(dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[0].ToString());
                        break;
                    case "デイケア":
                        daycare.Add(dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[0].ToString());
                        break;
                }
                i++;
            }

            loc.Add("訪問看護", visit);
            loc.Add("院内", inner);
            loc.Add("デイケア", daycare);

            dt.Clear();
        }

        //1 場所を選択すると、車種が出る
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dt = new DataTable();
            comboBox2.Text = "";

            //car = new Dictionary<string, string>();
            comboBox2.Items.Clear();
            //MessageBox.Show(comboBox1.SelectedValue.ToString());
            if (comboBox1.Items[comboBox1.SelectedIndex] != "その他(自家用車)")
            {
                foreach (var n in loc[comboBox1.SelectedItem.ToString()])
                {
                    comboBox2.Items.Add(n.Key);
                }
            }
            

            
        }

        

        //1 異常なしボタンクリック
        private void okButton_Click(object sender, EventArgs e)
        {
            
            //車種を選択していなかったら
            if(comboBox2.SelectedIndex == -1)
            {
                if( comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("利用する車を選択してください");
                    return; 
                } else if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
                {
                    MessageBox.Show("利用する車を選択してください");
                    return;
                }
                
            }
            if (before.Checked)
            {
                data.propertyDriveStatus = false;
            } else
            {
                data.propertyDriveStatus = true;
            }
            data.propertyCarName = comboBox2.SelectedItem.ToString();
            if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
            {
                data.propertyCarNumber = loc[comboBox1.SelectedItem.ToString()][comboBox2.SelectedItem.ToString()];
            }
            
            data.propertyAlcoholFlag = false;

            //カードを読み取るTabに移動する
            mainTab.SelectedTab = getCheckerInformation;

            dt.Clear();
        }

        //1 異常ありボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {
            //車種を選択していなかったら
            if (comboBox2.SelectedIndex == -1)
            {
                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("利用する車を選択してください");
                    return;
                } else if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
                {
                    MessageBox.Show("利用する車を選択してください");
                    return;
                }

            }
            if (before.Checked)
            {
                data.propertyDriveStatus = false;
            }
            else
            {
                data.propertyDriveStatus = true;
            }
            data.propertyCarName = comboBox2.SelectedItem.ToString();
            if (comboBox1.SelectedItem.ToString() != "その他(自家用車)")
            {
                data.propertyCarNumber = loc[comboBox1.SelectedItem.ToString()][comboBox2.SelectedItem.ToString()];
            }


            //カードを読み取るTabに移動する
            data.propertyAlcoholFlag = true;
            mainTab.SelectedTab = getCheckerInformation;

            dt.Clear();
        }

        //タブが切り替わる際に起こるイベント
        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTab.SelectedTab == getCheckerInformation)
            {
                timer1.Enabled = true;
            }

            if (mainTab.SelectedTab == confirmPage)
            {
                textBox6.Text = "";
                if(data.propertyAlcoholFlag)
                {
                    textBox6.Text = textBox6.Text + "検出者名:" + data.propertyName + "\r\n";
                } else
                {
                    if(data.propertyDriveStatus)
                    {
                        textBox6.Text = textBox6.Text + "状態: 運転後" + "\r\n";
                    } else
                    {
                        textBox6.Text = textBox6.Text + "状態: 運転前" + "\r\n";
                    }
                    
                    textBox6.Text = textBox6.Text + "運転者名: " + data.propertyName + "\r\n";
                    textBox6.Text = textBox6.Text + "車ナンバー: " + data.propertyCarNumber + "\r\n";
                    textBox6.Text = textBox6.Text + "車種: " + data.propertyCarName + "\r\n";
                    textBox6.Text = textBox6.Text + "立会人: " + data.propertyWitnessName + "\r\n";
                    if(data.propertyPhysicalCondition)
                    {
                        textBox6.Text = textBox6.Text + "体調: 悪い" + "\r\n";
                        textBox6.Text = textBox6.Text + "コメント:\r\n" + data.propertyComment + "\r\n";
                    } else
                    {
                        textBox6.Text = textBox6.Text + "体調: 良い" + "\r\n";
                    }
                    
                }
                
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //MessageBox.Show("hello");
            
        }

        //2 次のページに進む
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if(textBox1.Text !="" && textBox2.Text != "")
            {
                mainTab.SelectedTab = healthCheck;
                data.propertyName = textBox2.Text;
            } else
            {
                MessageBox.Show("カードを読み取るか、職員ｺｰﾄﾞを入力してください");
            }
            
        }

        //2 職員ｺｰﾄﾞから職員名を取得する
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            dt = new DataTable();
            if(e.KeyChar == (char)Keys.Enter)
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(@"../../sql/getNameByCode.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("1086", textBox1.Text);



                bool check = interaction_Access.interaction(ref dt, query, const_Util.db1);



                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("ユーザーが存在しません");
                    textBox1.Text = "";
                    return;
                }

                textBox2.Text = dt.Rows[0].ItemArray[0].ToString();
            }

            dt.Clear();
        }

        //2 クリアがクリックされた場合
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        //3 クリアがクリックされた場合
        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox4.Text = "";
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
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Enterが押された場合
            if (e.KeyChar == (char)Keys.Enter)
            {
                //基本の部分のクエリを取得する
                string query = new StreamReader(@"../../sql/getNameByCode.txt").ReadToEnd();

                //読み取った情報に置き換える
                query = query.Replace("1086", textBox3.Text);



                bool check = interaction_Access.interaction(ref dt, query, const_Util.db1);



                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("ユーザーが存在しません");
                    textBox3.Text = "";
                    return;
                }

                textBox4.Text = dt.Rows[0].ItemArray[0].ToString();
            }

            dt.Clear();
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

        //3 次へをクリック
        private void button4_Click(object sender, EventArgs e)
        {
            //異常なしの場合
            if (radioButton1.Checked == true)
            {

                //立会人が入力されていない場合
                if (textBox3.Text == "" || textBox4.Text == "")
                {
                    MessageBox.Show("立会人を入力してください");
                    return;
                }

                data.propertyPhysicalCondition = false;
                data.propertyWitnessName = textBox4.Text;

                //立会人のカードを読み取るTabに移動する
                mainTab.SelectedTab = checkWitnessInformation;
            }
            else
            {
                //異常があるのにコメントが入力されていない場合
                if (textBox5.Text == "")
                {
                    MessageBox.Show("症状を入力してください");
                    return;
                }
                //立会人が入力されていない場合
                if(textBox3.Text == "" || textBox4.Text == "")
                {
                    MessageBox.Show("立会人を入力してください");
                    return;
                }

                data.propertyPhysicalCondition = true;
                data.propertyWitnessName = textBox4.Text;
                data.propertyComment = textBox5.Text;
                //立会人のカードを読み取るTabに移動する
                mainTab.SelectedTab = checkWitnessInformation;
            }
        }

        //確認ページで「戻る」ボタンを押した場合
        private void confirm_backButton_Click(object sender, EventArgs e)
        {

        }

        //確認ページで「登録する」ボタンを押した場合
        private void confirm_registerButton_Click(object sender, EventArgs e)
        {
            mainTab.SelectedTab = lastPage;
        }
    }
}
