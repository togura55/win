using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ChangeFieldValue
{
    public partial class Form1 : Form
    {
        static private MySqlConnection conn;
        static private string connStr = "";
        static bool bConnection;
        static ArrayList al = new ArrayList();

        ////For Staging Server
        //string ServerAddress = "10.3.3.156";
        //string MySQLUserName = "redmine01-ext";
        //string MySQLPin = "ASvdGqq7WUDuJ8Dj";
        //string DBName = "redmine01";

        //Main server
        string ServerAddress = "tolinux01.global.sdl.corp";
        string MySQLUserName = "redmine01-ext";
        string MySQLPin = "9KULcY6L8cKNwdFK";
        string DBName = "redmine01";


        public Form1()
        {
            InitializeComponent();
            CheckServerStatus();
            InitCombobox();
        }

        private void InitCombobox()
        {
            if (bConnection)
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `id`, `name` FROM`custom_fields` ", conn);
                MySqlDataReader reader = null;
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CustomField cf = new CustomField();
                        cf.ID = int.Parse(reader.GetString(0));
                        cf.FieldName = reader.GetString(1);

                        al.Add(cf);
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                    if (al.Count != 0)
                    {
                        foreach (CustomField c in al)
                        {
                            comboFiled.Items.Add(c.FieldName);
                        }
                    }
                }
            }
        }

        private int GetFieldIDFromName(string FieldName)
        {
            int i = -1;
            foreach (CustomField cf in al)
            {
                if (cf.FieldName == FieldName)
                {
                    i = cf.ID;
                }
            }
            return i;
        }

        private void CheckServerStatus()
        {
            connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false; charset=utf8", ServerAddress, MySQLUserName, MySQLPin, DBName);
            bConnection = true;
            // QARepDBからBugIDとCDETS#のリストを作って返す
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                conn.ChangeDatabase("redmine01");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                bConnection = false;
            }
            if (bConnection == true)
            {
                labelServerStatus.Text = "Connection Status = OK";
            }
            else
            {
                labelServerStatus.Text = "Connection Error";
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (bConnection != true)
            {
                MessageBox.Show("Confirm the server setting, please.");
                return;
            }

            string FilePath = tbListPath.Text;
            if (File.Exists(FilePath) == false)
            {
                MessageBox.Show("File not found.");
                return;
            }

            if (comboFiled.SelectedIndex < 0)
            {
                MessageBox.Show("Select Target Filed.");
                return;
            }

           

            //Textからリストを読み込んでArrayListに突っ込む
            string line = "";
            ArrayList alIssueID = new ArrayList();

            using (StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding("Shift_JIS")))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    alIssueID.Add(line);
                }
            }


            //customized_id と custom_field_id で絞ってid を取得。そのIDのValueを書き換える


            int customFieldID = GetFieldIDFromName(comboFiled.Text);
            MySqlDataReader reader = null;
            
            ///WHERE`customized_id`=12288 AND`custom_field_id`=100

            ArrayList alID = new ArrayList();

            foreach (string myID in alIssueID)
            {

                MySqlCommand cmd = new MySqlCommand("SELECT`id` ,`value` FROM custom_values WHERE `customized_id`=" + myID + " AND custom_field_id =" + customFieldID, conn);
                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        FieldData fd = new FieldData();
                        fd.ID = int.Parse(reader.GetString(0));
                        fd.Value = reader.GetString(1);
                        alID.Add(fd);
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Failed to get Project ID: " + ex.Message);
                }
                finally
                {
                    if (reader != null) reader.Close();
                }

            }


            //AddだったらFieldの値に追加。RemoveだったらReplace ""
            foreach (FieldData fd in alID)
            {
                if (radioBtnAdd.Checked == true)
                {
                    fd.Value = fd.Value + tbValue.Text;
                }
                else
                {
                    fd.Value = fd.Value.Replace(tbValue.Text, "");
                }
            }

            foreach (FieldData fd in alID)
            {
                string Update_SQL2 = "UPDATE`redmine01`.`custom_values`SET`value`='" + fd.Value + "' WHERE`custom_values`.`id`=" + fd.ID + ";";
                MySqlCommand cmd = new MySqlCommand(Update_SQL2, conn);
                cmd.ExecuteNonQuery();
            }


            MessageBox.Show("END");


            //DataTable dt = new DataTable();
            //string myString = "SELECT* FROM`custom_values` WHERE`custom_field_id` =" + customFieldID.ToString() + "";
            //MySqlDataAdapter da = new MySqlDataAdapter(myString, conn);
            //da.Fill(dt);
            //dataGridView1.DataSource = dt;
            //MessageBox.Show("END");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult resutl = MessageBox.Show("Quit?", "Quit", MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
            if (resutl == DialogResult.Yes)
            {
                CloseForm();
            }
        }

        private void CloseForm()
        {
            this.Close();
        }


        private class CustomField
        {
            public int ID = -1;
            public string FieldName = "";
        }

        private class FieldData
        {
            public int ID = -1;
            public string Value = "";
        }
    }
}
