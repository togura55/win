using AngleSharp.Html.Parser;
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
using System.Security.Cryptography;

namespace ScoreScraping
{
    public partial class FormMain : Form
    {

        ScoreScraping scr;

        public FormMain()
        {
            InitializeComponent();

            Application.ApplicationExit += new EventHandler(AppExit);

            scr = new ScoreScraping();
            DeSerialize();  // restore parameters
            if (scr.pwd != string.Empty)
                scr.password = Decrypt(scr.pwd, AES_IV, AES_Key);   // decript strings


            labelUrl.Text = Properties.Resources.IDC_LABEL_URL;
            pbtnStart.Text = Properties.Resources.IDC_PBTN_START;
            labelView.Text = Properties.Resources.IDC_LABEL_VIEW;
            labelTitle.Text = Properties.Resources.IDC_LABEL_TITLE;
            labelHtml.Text = Properties.Resources.IDC_LABEL_HTML;
            labelID.Text = Properties.Resources.IDC_LABEL_ID;
            labelPassword.Text = Properties.Resources.IDC_LABEL_PASSWORD;
            pbtnEditList.Text = Properties.Resources.IDC_PBTN_EDITLIST;
            pbtnShowResult.Text = Properties.Resources.IDC_PBTN_SHOWRESULT;
            

            pbtnShowResult.Enabled = false;

            textBoxUrl.Text = scr.loginUrl;
            textBoxID.Text = scr.id;
            textBoxPassword.Text = scr.password;

            InitializeComboBoxWebsites();
        }

        // Initialize ComboBox.
        private void InitializeComboBoxWebsites()
        {
            this.comboBoxWebsites.Text = scr.website;
            string[] installs = new string[] { "GDO", "ShotNavi"};
            comboBoxWebsites.Items.AddRange(installs);
            this.Controls.Add(this.comboBoxWebsites);

            // Hook up the event handler.
            //this.comboBoxWebsites.DropDown +=
            //    new System.EventHandler(ComboBoxWebsites_DropDown);
        }

        // Handles the ComboBox1 DropDown event. If the user expands the  
        // drop-down box, a message box will appear, recommending the
        // typical installation.
        private void ComboBoxWebsites_DropDown(object sender, System.EventArgs e)
        {
            //MessageBox.Show("Typical installation is strongly recommended.",
            //"Install information", MessageBoxButtons.OK,
            //    MessageBoxIcon.Information);
        }

        private void AppExit(object sender, EventArgs e)
        {
            scr.pwd = Encrypt(scr.password, AES_IV, AES_Key); // encript for particular strings
            Serialize();    // store parameters
        }

        //
        // https://qiita.com/kz-rv04/items/62a56bd4cd149e36ca70
        //
        private const string AES_IV = @"pf69DL6GrWFyZcMK";
        private const string AES_Key = @"9Fix4L4HB4PKeKWY";

        private void DeSerialize()
        {
            try
            {
                //保存先のファイル名
                string fileName = @"Serialize.xml";

                //オブジェクトの型（今回はMemberinfo）を指定して、XmlSerializerを作成する。
                XmlSerializer se = new XmlSerializer(typeof(ScoreScraping));

                //ファイルを開く
                StreamReader sr = new StreamReader(fileName, new UTF8Encoding(false));

                //デシリアライズして復元
                scr = (ScoreScraping)se.Deserialize(sr);

                //ファイルを閉じる
                sr.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void Serialize()
        {
            //保存先のファイル名
            string fileName = @"Serialize.xml";

            //オブジェクトの型（今回はMemberinfo）を指定して、XmlSerializerを作成する。
            XmlSerializer se = new XmlSerializer(typeof(ScoreScraping));

            //ファイルを開く
            StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false));

            //シリアライズして保存
            se.Serialize(sw, scr);

            //ファイルを閉じる
            sw.Close();
        }

        private void pbtnStart_Click(object sender, EventArgs e)
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
                scr.loginUrl = textBoxUrl.Text;
                scr.id =textBoxID.Text;
                scr.password =textBoxPassword.Text;

                switch(scr.website)
                {
                    case "ShotNavi":
                        ShotNavi(scr.loginUrl, scr.id, scr.password);
                        break;

                    case "GDO":
                        Gdo(scr.loginUrl, scr.id, scr.password);
                        break;

                    default:
                        break;
                }

                // 「取得中」の文言を不可視にします。
                labelView.Visible = false;

                pbtnShowResult.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

        private void pbtnEditList_Click(object sender, EventArgs e)
        {
            try
            {
                //メモ帳を起動する
                Process p = Process.Start("notepad.exe", "crawllist.txt"); //  crawllist.txt
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void pbtnShowResult_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = Process.Start("excel.exe", "shotnavi.csv"); //  shotnavi.csv
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        // --- for ShotNavi ------------------------------
        private void ShotNavi(string url, string id, string pwd)
        {
            string htmlText = string.Empty;

            try
            {
                htmlText = scr.Login(url, id, pwd, "ShotNavi");

                // Read URL list
                string line = string.Empty;
                using (StreamReader sr = new StreamReader(
                    "crawllist.txt", Encoding.GetEncoding("Shift_JIS")))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        Hole hh = new Hole();
                        scr.holeList.Add(hh);
                        ((Hole)scr.holeList[scr.holeList.Count - 1]).url = line;
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
                    h.row = tableElements[0].TextContent;
                }

                // 抽出
                string[] sep_dame = { "打目" };
                string sep_Y = "Y";
                foreach (Hole h in scr.holeList)
                {
                    //"打目"が何回出現するかでカウント
                    //"打目"のすぐ後が"－"の場合はyardが入っていないと見なす
                    // そうでなければ、
                    // "打目"と"Y"の間がヤード数
                    string[] arr = h.row.Split(sep_dame, StringSplitOptions.None);
                    h.yardList = new ArrayList();
                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        string y = string.Empty;
                        int pos = arr[i].IndexOf(sep_Y);
                        if (pos > 0)
                        {
                            y = arr[i].Substring(0, pos);
                        }
                        h.yardList.Add(y);
                    }
                }

                // 書き出し
                using (StreamWriter sw = new StreamWriter(
                    @"shotnavi.csv", false, Encoding.GetEncoding("Shift_JIS")))
                {
                    string output = string.Empty;

                    foreach (Hole h in scr.holeList)
                    {
                        foreach (string s in h.yardList)
                        {
                            output += s + ',';
                        }
                        output.Substring(0, output.Length - 1); // delete a last ','
                        output += Environment.NewLine;
                    }
                    // テキストを書き込む
                    sw.WriteLine(output);

                    // ファイルを閉じる
                    sw.Close();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("ShotNavi:{0}", ex.Message));
            }
        }

        // --- for GDO ------------------------------
        private void Gdo(string url, string id, string pwd)
        {
            // クラスをインスタンス化します。
            var scr = new ScoreScraping();

            string htmlText = string.Empty;
            try
            {
                // login
                url = @"https://usr.golfdigest.co.jp/pg/frlogin.php?mm_rurl=https%3A%2F%2Fwww.golfdigest.co.jp%2F"; // login page
                htmlText = scr.Login(url, id, pwd, "GDO");

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

        /// <summary>
        /// 対称鍵暗号を使って文字列を暗号化する
        /// </summary>
        /// <param name="text">暗号化する文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string text, string iv, string key)
        {

            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;

                rijndael.IV = Encoding.UTF8.GetBytes(iv);
                rijndael.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                byte[] encrypted;
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(ctStream))
                        {
                            sw.Write(text);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
                return (System.Convert.ToBase64String(encrypted));
            }
        }

        /// <summary>
        /// 対称鍵暗号を使って暗号文を復号する
        /// </summary>
        /// <param name="cipher">暗号化された文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>復号された文字列</returns>
        public static string Decrypt(string cipher, string iv, string key)
        {
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;

                rijndael.IV = Encoding.UTF8.GetBytes(iv);
                rijndael.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                string plain = string.Empty;
                using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(cipher)))
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(ctStream))
                        {
                            plain = sr.ReadLine();
                        }
                    }
                }
                return plain;
            }
        }
    }
}
