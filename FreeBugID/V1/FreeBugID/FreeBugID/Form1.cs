using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Deployment.Application;

// 2012/4/10 1st release

namespace FreeBugID
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.ActiveControl = this.textBox1;
        }

        //追加しなくてはいけないこと
        //Project IDをアプリ終了時に覚えておいて、起動時に復活させる
        //接続先、データベース名、ID、Passwordをパラメータ化して外に出す
        //重複BugIDの存在をチェックして通知する機能

        private MySqlConnection conn;
        private void button1_Click(object sender, EventArgs e)
        {
            ArrayList al = new ArrayList();

            
            if (conn != null)
            {
                conn.Close();
            }
            string connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false", "tolinux01.global.sdl.corp", "redmine01-ext", "9KULcY6L8cKNwdFK", "redmine01");
            //string connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false", "tolinux02.global.sdl.corp", "togura", "P@ssw0rd", "redmine_default");

            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                conn.ChangeDatabase("redmine01");

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + ex.Message);
                return;
            }

            //Added ProjctNum check code 2012/04/20 tbojo
            string ProjctNum = this.textBox1.Text;
            if (ProjctNum == "")
            {
                return;
            }

            MySqlDataReader reader = null;
            MySqlCommand cmd = new MySqlCommand("SELECT value FROM custom_values WHERE value LIKE '" + ProjctNum+ "S___'", conn);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    al.Add(reader.GetString(0));
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to populate database list: " + ex.Message);
                return;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            al.Sort();
            al.Reverse();

            if (al.Count == 0)
            {
                this.textBox2.Text = "Not found";
                return;
            }

            string lastID = al[0].ToString();
            int iMaxID = int.Parse(lastID.Substring(lastID.Length - 3)) + 1;
            this.textBox2.Text = this.textBox1.Text.ToUpper() + "S" + String.Format("{0:D3}", iMaxID);

            Clipboard.SetText(this.textBox2.Text);

            //this.listBox1.Items.Clear();
            //foreach (string s in al)
            //{
            //    this.listBox1.Items.Add(s);
            //}
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            　



            //System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            //System.Version ver = asm.GetName().Version;
            //MessageBox.Show("Version: " + ver.ToString(),"Version Info");
            MessageBox.Show("Version: " + GetVersion());
        }

        private string GetVersion()
        {
            if (!ApplicationDeployment.IsNetworkDeployed) return String.Empty;
            Version version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            return (
              version.Major.ToString() + "." +
              version.Minor.ToString() + "." +
              version.Build.ToString() + "." +
              version.Revision.ToString()
            );
    }


}


}
