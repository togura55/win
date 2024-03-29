﻿using AngleSharp.Html.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ScoreScraping
{
    public partial class FormMain : Form
    {
        ScoreScraping scr;
        TargetWebsite tw;
        string configFileName = @"Serialize.xml";   //保存先のファイル名
        string crawllistFileName = @"crawllist.txt";
        string outputShotNaviFileName = @"shotnavi.csv";
        string outputShotNaviFileName1st = @"shotnavi1st.csv";
        string appDataPath;

        public FormMain()
        {
            InitializeComponent();

            Application.ApplicationExit += new EventHandler(AppExit);

            scr = new ScoreScraping();  // create an object instance

            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + this.ProductName;

            try
            {
                DeSerialize(appDataPath + "\\" + configFileName);  // restore parameters
                int cc = 0;
                foreach (TargetWebsite tw in scr.TargetWebsites)
                {
                    if (tw.name.Equals(scr.website))
                        scr.siteIndex = cc;

                    cc++;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string productName = this.ProductName;
            string version = System.Windows.Forms.Application.ProductVersion;
            this.Text = string.Format("{0} {1}", productName, version);

            labelUrl.Text = Properties.Resources.IDC_LABEL_URL;
            pbtnStart.Text = Properties.Resources.IDC_PBTN_START;
            labelView.Text = Properties.Resources.IDC_LABEL_VIEW;
            labelHtml.Text = Properties.Resources.IDC_LABEL_HTML;
            labelID.Text = Properties.Resources.IDC_LABEL_ID;
            labelPassword.Text = Properties.Resources.IDC_LABEL_PASSWORD;
            pbtnEditList.Text = Properties.Resources.IDC_PBTN_EDITLIST;
            pbtnShowResult.Text = Properties.Resources.IDC_PBTN_SHOWRESULT;

            pbtnShowResult.Enabled = false;

            InitializeComboBoxWebsites();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            scr.TargetWebsites[scr.siteIndex].loginUrl = textBoxUrl.Text;
            scr.TargetWebsites[scr.siteIndex].id = textBoxID.Text;
            scr.TargetWebsites[scr.siteIndex].Password = textBoxPassword.Text;
            Serialize(appDataPath + "\\" + configFileName);    // store parameters
        }

        private void UpdateTargetWebsiteSettings()
        {
            tw = scr.TargetWebsites[scr.siteIndex];
            scr.website = tw.name;
            textBoxUrl.Text = tw.loginUrl;
            textBoxID.Text = tw.id;
            textBoxPassword.Text = tw.Password;
        }

        // Initialize ComboBox.
        private void InitializeComboBoxWebsites()
        {
            List<string> installs = new List<string>();
            foreach (TargetWebsite t in scr.TargetWebsites)
            {
                installs.Add(t.name);
            }
            scr.siteIndex = installs.IndexOf(scr.website);

            comboBoxWebsites.Items.AddRange(installs.ToArray());
            this.Controls.Add(this.comboBoxWebsites);

            this.comboBoxWebsites.SelectedIndex = scr.siteIndex;
        }

        private void ComboBoxWebsites_SelectedIndexChanged(object sender, EventArgs e)
        {
            scr.siteIndex = comboBoxWebsites.SelectedIndex;
            UpdateTargetWebsiteSettings();
        }

        private void AppExit(object sender, EventArgs e)
        {

        }

        private void DeSerialize(string file)
        {
            try
            {
                //オブジェクトの型（今回はMemberinfo）を指定して、XmlSerializerを作成する。
                XmlSerializer se = new XmlSerializer(typeof(ScoreScraping));

                //ファイルを開く
                StreamReader sr = new StreamReader(file, new UTF8Encoding(false));

                //デシリアライズして復元
                scr = (ScoreScraping)se.Deserialize(sr);

                //ファイルを閉じる
                sr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DeSerialize:{0}", ex.Message));
            }
        }

        private void Serialize(string file)
        {
            try
            {
                //オブジェクトの型（今回はMemberinfo）を指定して、XmlSerializerを作成する。
                XmlSerializer se = new XmlSerializer(typeof(ScoreScraping));

                //ファイルを開く
                StreamWriter sw = new StreamWriter(file, false, new UTF8Encoding(false));

                //シリアライズして保存
                se.Serialize(sw, scr);

                //ファイルを閉じる
                sw.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Serialize:{0}", ex.Message));
            }
        }

        private void PbtnStart_Click(object sender, EventArgs e)
        {
            // 処理中に「取得中」とラベル表示します。
            // 予めURLのテキストボックス下に無記名のラベルを作成しておきます。
            // (Name)をlblViewにしておいてください。
            labelView.Visible = true;         // 可視化
            labelView.Text = Properties.Resources.IDC_TEXT_RETRIEVE;// 「取得中」の文字列を表示することで処理中であることを明記します。
            labelView.BringToFront();         // Objectを最善面に移動します。
            labelView.Update();               // 表示を更新します。
            scr.website = comboBoxWebsites.SelectedItem.ToString();

            try
            {
                // 画面上からHTMLを取得するサイトの情報を取得します。
                tw.loginUrl = textBoxUrl.Text;
                tw.id = textBoxID.Text;
                tw.Password = textBoxPassword.Text;

                switch (scr.website)
                {
                    case "ShotNavi":
                        ShotNavi(tw);
                        break;

                    case "GDO":
                        Gdo(tw);
                        break;

                    default:
                        break;
                }

                // 「取得中」の文言を不可視にします。
                labelView.Visible = false;
                pbtnShowResult.Enabled = true;
                WriteLog(Properties.Resources.IDC_TEXT_COMPLETED);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        private void PbtnEditList_Click(object sender, EventArgs e)
        {
            try
            {
                //メモ帳を起動する
                Process p = Process.Start("notepad.exe", appDataPath + "\\" + crawllistFileName); //  crawllist.txt
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        private void PbtnShowResult_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = Process.Start("excel.exe", appDataPath + "\\" + outputShotNaviFileName); //  shotnavi.csv
                p = Process.Start("excel.exe", appDataPath + "\\" + outputShotNaviFileName1st); //  shotnavi1st.csv
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        void WriteLog(String logText)
        {
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.SelectionLength = 0;
            textBoxLog.SelectedText = "[" + System.DateTime.Now.ToString() + "]" + logText + "\r\n";
        }

        // ======= Website crawling procedures ============
        // --- for ShotNavi ------------------------------
        private void ShotNavi(TargetWebsite tw)
        {
            string htmlText = string.Empty;

            try
            {
                htmlText = scr.Login(tw.loginUrl, tw.id, tw.Password, tw.name);

                // Read URL list
                string line = string.Empty;
                using (StreamReader sr = new StreamReader(
                    appDataPath + "\\" + crawllistFileName, Encoding.GetEncoding("Shift_JIS")))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length != 0)
                        {
                            Hole hh = new Hole();
                            scr.holeList.Add(hh);
                            ((Hole)scr.holeList[scr.holeList.Count - 1]).url = line;
                        }
                    }
                    sr.Close();
                }

                //// 巡回
                var parser = new HtmlParser();
                foreach (Hole h in scr.holeList)
                {
                    var htmlDocument = parser.ParseDocument(scr.ReadLog(h.url));
                    var tableElements = htmlDocument.QuerySelectorAll("table");

                    // "\n飛距離番手風心拍歩数\n(ティー)－－ -001打目191 Y－－ -002打目156 Y－－ -003打目14 Y－－ -00"
                    if (tableElements.Length != 0)
                        h.row = tableElements[0].TextContent;
                }

                // 抽出
                string[] sep_dame = { "打目" };
                string sep_Y = "Y";
                foreach (Hole h in scr.holeList)
                {
                    if (h.row != null)
                    {

                        //"打目"が何回出現するかでカウント
                        //"打目"のすぐ後が"－"の場合はyardが入っていないと見なす
                        // そうでなければ、"打目"と"Y"の間がヤード数
                        string[] arr = h.row.Split(sep_dame, StringSplitOptions.None);
                        h.yardList = new List<string>();
                        for (int i = 0; i < arr.Length; i++)
                        {
                            string y = string.Empty;
                            int pos = arr[i].IndexOf(sep_Y);
                            if (pos > 0)
                            {
                                y = arr[i].Substring(0, pos);
                                h.yardList.Add(y);
                            }
                        }

                    }
                }

                // Export to file
                using (StreamWriter sw = new StreamWriter(
                    appDataPath + "\\" + outputShotNaviFileName, false, Encoding.GetEncoding("Shift_JIS")))
                {
                    string output = string.Empty;

                    foreach (Hole h in scr.holeList)
                    {
                        if (h.row != null)
                        {
                            foreach (string s in h.yardList)
                            {
                                output += s + ',';
                            }
                            output.Substring(0, output.Length - 1); // delete a last ','
                        }
                        else
                            output += "-";

                        output += Environment.NewLine;

                        output += Environment.NewLine;
                        output += Environment.NewLine;
                    }
                    // テキストを書き込む
                    sw.WriteLine(output);

                    // ファイルを閉じる
                    sw.Close();
                }

                using (StreamWriter sw = new StreamWriter(
                    appDataPath + "\\" + outputShotNaviFileName1st, false, Encoding.GetEncoding("Shift_JIS")))
                {
                    string output = string.Empty;
                    const int ignoreStrokes = 20;

                    foreach (Hole h in scr.holeList)
                    {
                        if (h.row != null)
                        {
                            foreach (string s in h.yardList)
                            {
                                if (int.Parse(s) > ignoreStrokes)
                                {
                                    output += s + Environment.NewLine;
                                    break;
                                }
                            }
                        }
                        else
                            output += "-" + Environment.NewLine;
                    }
                    // テキストを書き込む
                    sw.WriteLine(output);

                    // ファイルを閉じる
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ShotNavi: {0}", ex.Message));
            }
        }

        // --- for GDO ------------------------------
        private void Gdo(TargetWebsite tw)
        {
            // クラスをインスタンス化します。
            var scr = new ScoreScraping();

            string htmlText = string.Empty;
            try
            {
                // login
                htmlText = scr.Login(tw.loginUrl, tw.id, tw.Password, tw.name);

                htmlText = scr.ReadLog(@"https://score.golfdigest.co.jp/score?car=top2_navi&mm_rcd=1");

                htmlText = scr.ReadLog(@"https://www.golfdigest.co.jp/?mm_rcd=1");

                // Go MyGDO
                // aspxページのリダイレクトページが表示される
                htmlText = scr.ReadLog(@"https://myp.golfdigest.co.jp/?car=top2_navi");

                // htmlText = scr.ReadLog(@"https://myp.golfdigest.co.jp/myp/mygdotop.aspx");// 直叩きだと404
                // htmlText = scr.ReadLog(@"https://score.golfdigest.co.jp/");  // スコア
                // htmlText = scr.ReadLog(@"https://score.golfdigest.co.jp/score?car=top2_navi"); // スコア


            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GDO:{0}", ex.Message));
            }

        }
    }
}
