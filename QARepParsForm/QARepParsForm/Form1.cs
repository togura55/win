using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Globalization;

namespace QARepParsForm
{

    public partial class MainWindow : Form
    {
        static private string sSMTPServer = "smtp.gmail.com";
        static private int iSMTPServerPort = 587;
        static private bool bSMTPAuth = true;
        static private bool bSMTPSSL = true;
        static private string sFrom = "test.sdljapan@gmail.com";
        static private string sTo = "togura@sdl.com";       // colon separate for multiple sending
        static private string sCc = "";
        static private string sBcc = "";
        static private string sPwd = "sdllab.2";
        static private string sSubject = "QARepParsForm";
        static private string sBody = "This is a test mail";
        static private string sSign = "";
        static private string sFromDisplayName = "QARepPars Notifier";

        static string sCRLF = System.Environment.NewLine;   // \n
        public string sLogFile = "";

        public List<string> sListSummary = new List<string>();
        public List<string> sListDownload = new List<string>();
        public List<string> sListUpload = new List<string>();

        public struct ProcessBlock
        {
            public int iStart;          // Start char number
            public string sStart;       // Start line contents
            public string sStartKeyword;    // Line head word Start
            public int iEnd;            // End char number
            public string sEnd;         // End line contents
            public string sEndKeyword;  // Line heas word End
            public int iError;          // ToDo: No needs?
            public List<string> sError; // List of error descriptions
            public int iWarning;        // ToDo: No needs?
            public List<string> sWarning;   // List of warning descriptions
        }
        public List<ProcessBlock> listStartEnd = new List<ProcessBlock>();
        public List<ProcessBlock> listDownload = new List<ProcessBlock>();
        public List<ProcessBlock> listUpload = new List<ProcessBlock>();

        private string[] slistErrorDesc = {"no such file or directory",
                                               "unable to",
                                               "unknown command"};

        private string[] slistWarningDesc = { "nothing matched" };

        public MainWindow()
        {
            InitializeComponent();

            // Show default parameters on UI controls 
            textBoxAccount.Text = sFrom;
            textBoxTo.Text = sTo;
            textBoxSMTPPassword.Text = sPwd;
            textBoxSMTPPort.Text = iSMTPServerPort.ToString();
            textBoxSMTPServer.Text = sSMTPServer;
            textBoxTitle.Text = sSubject;
            textBoxContents.Text = sBody;
            textBoxFromDisplayName.Text = sFromDisplayName;

            checkBoxSMTPAuth.Checked = bSMTPAuth;
            checkBoxSMTPSSL.Checked = bSMTPSSL;
            if (bSMTPAuth || bSMTPSSL)
            {
                textBoxSMTPPort.Enabled = true;
                labelSMTPPort.Enabled = true;
                textBoxSMTPPassword.Enabled = true;
                labelSMTPPassword.Enabled = true;
            }
            else
            {
                textBoxSMTPPort.Enabled = false;
                labelSMTPPort.Enabled = false;
                textBoxSMTPPassword.Enabled = false;
                labelSMTPPassword.Enabled = false;
            }
        }

        private void buttonParse_Click(object sender, EventArgs e)
        {
            sLogFile = textBoxLogFile.Text;

            string text = "";

            try
            {
                using (StreamReader sr = new StreamReader(
                    sLogFile, Encoding.GetEncoding("Shift_JIS")))
                {
                    text = sr.ReadToEnd();
                }

                LogParse(text);
                // Succeeded? Go Send Mail


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);  
            }
        }

        static private int doSendMail()
        {
            //// Create Subject
            //sSubject += " [" + DateTime.Today.ToString("yyyy/MM/dd") + "] "
            //    + Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])
            //    + " Report from "
            //    + sPCName;
            //// Add Signature
            //FileVersionInfo ver
            //    = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //sBody += sCRLF + sSeparater
            //    + sCRLF + sSign
            //    + sCRLF + Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])
            //    + " " + ver.FileVersion.ToString();

            // SMTP process
            SmtpClient sc;
            MailMessage msg = new MailMessage();
            try
            {
                if (!bSMTPAuth && !bSMTPSSL)
                {
                    // Traditional SMTP
                    sc = new SmtpClient();
                    sc.Host = sSMTPServer;
                }
                else
                {
                    sc = new SmtpClient(sSMTPServer, iSMTPServerPort);
                    sc.EnableSsl = bSMTPSSL;
                    if (bSMTPAuth)
                        sc.Credentials = new NetworkCredential(sFrom, sPwd);
                }
                // 差出人
                MailAddress from = new MailAddress(sFrom, sFromDisplayName, Encoding.GetEncoding("iso-2022-jp"));

                // MailMessage を作成
                msg.From = from;
                msg.To.Add(sTo);
                msg.Subject = sSubject;
                msg.SubjectEncoding = Encoding.GetEncoding("iso-2022-jp");
                msg.Body = sBody;
                if (sCc.Length != 0) msg.CC.Add(sCc);
                if (sBcc.Length != 0) msg.Bcc.Add(sBcc);

                sc.Send(msg);
            }
            catch (Exception ex)
            {
                //Console.Write(ex.Message + sCRLF);
                MessageBox.Show(ex.Message);
                return -1;
            }
            //bSentMail = true;

            return 0;
        }

        private void buttonSendMail_Click(object sender, EventArgs e)
        {
            // Get parameters from UI controls
            sTo = textBoxTo.Text;
            sFrom = textBoxAccount.Text;
            sPwd = textBoxSMTPPassword.Text;
            iSMTPServerPort = int.Parse(textBoxSMTPPort.Text);
            sSMTPServer = textBoxSMTPServer.Text;
            sSubject = textBoxTitle.Text;
            sBody = textBoxContents.Text;

            bSMTPAuth = (checkBoxSMTPAuth.Checked ? true : false);
            bSMTPSSL = (checkBoxSMTPSSL.Checked ? true : false);
            sCc = textBoxCc.Text;
            sBcc = textBoxBcc.Text;
            sSign = textBoxSign.Text;
            sFromDisplayName = textBoxFromDisplayName.Text;

            if (doSendMail() != -1)
                MessageBox.Show("SendMail was successfully completed.");
        }

        private void buttonOpenLogFile_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
//            ofd.FileName = "default.html";
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter =
                "すべてのファイル(*.*)|*.*|LOGファイル(*.log)|*.log";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき
                //選択されたファイル名を表示する
                Console.WriteLine(ofd.FileName);

                // Show on UI
                textBoxLogFile.Text = ofd.FileName;
            }

        }

        public void DetectErrors(ref ProcessBlock pb, string source)
        {
            for (int i = 0; i<slistErrorDesc.Length; i++)
            {
                int iStart = pb.iStart;

                int iIndex = 0;
                while (iIndex != -1)
                {
                    iIndex = source.IndexOf(slistErrorDesc[i].ToString(), iStart, pb.iEnd - iStart);
                    if (iIndex != -1)
                    {
                        // 行番号を計算する
                        // 行頭まで戻って、1行を抜き出す。
                        string s = source.Substring(0, iIndex);
                        int iHead = source.LastIndexOf(sCRLF, iIndex);
                        pb.sError.Add("[" + (s.Length - s.Replace(sCRLF, "").Length).ToString() + "] "
                            + source.Substring(iHead + sCRLF.Length, (source.IndexOf(sCRLF, iIndex, pb.iEnd - iIndex) - (iHead + sCRLF.Length))));
                        pb.iError++;
                        iStart = iIndex+1;
                    }
                }
            }

        }

        public int FindKeyword(string source, string value, ref int startIndex, ref int endIndex)
        {
            int err = 0;

            int iFind = source.IndexOf(value, startIndex);
            if (iFind == -1)
            {
                err = 1;
            }
            else
            {
                // value was found
                startIndex = iFind;
                endIndex = source.IndexOf(sCRLF, iFind);

                // ToDo: 最下行であった場合の処理(最下行にCRLFがない場合）
                if (endIndex == -1)
                {
                    endIndex = source.Length;
                }
             }
            return err;
        }

        public bool FindBlock(string source, string begin, string end, List<ProcessBlock> listPb)
        {
            bool bRet = true;

            int startIndex = 0;
            int endIndex = 0;
            while (bRet)
            {
                if (FindKeyword(source, begin, ref startIndex, ref endIndex) == 0)
                {
                    ProcessBlock item = new ProcessBlock();
                    item.iStart = startIndex;
                    item.sStart = source.Substring(startIndex, endIndex - startIndex);
                    item.sStartKeyword = begin;
                    item.sError = new List<string>();
                    item.sWarning = new List<string>();
                    listPb.Add(item);

                    startIndex = endIndex;
                    endIndex = 0;
                    if (FindKeyword(source, end, ref startIndex, ref endIndex) == 0)
                    {
                        int i = listPb.Count - 1;
                        ProcessBlock pb = listPb[i];  // copy to temporary struct
                        pb.iEnd = startIndex;               // cannot edit member directly for generic collection
                        pb.sEnd = source.Substring(startIndex, endIndex - startIndex);
                        pb.sEndKeyword = end;

                        // Detect Errors
                        DetectErrors(ref pb, source);

                        listPb[i] = pb;               // write back
                    }
                    else
                    {
                        bRet = false;
                    }
                }
                else
                {
                    bRet = false;
                }
            }
            return bRet;
        }

        public void FormatBlock(string sSection, int iCnt, List<ProcessBlock> listPb, CultureInfo cFormat)
        {
            if (listPb.Count != 0)
            {
                // Total Time
                sBody += sSection;
                DateTime dtStart = DateTime.Parse((listPb[iCnt].sStart.Replace(listPb[iCnt].sStartKeyword, "").Replace("-", " ")), cFormat);
                DateTime dtEnd = DateTime.Parse((listPb[iCnt].sEnd.Replace(listPb[iCnt].sEndKeyword, "").Replace("-", " ")), cFormat);
                TimeSpan ts = dtEnd - dtStart;
                sBody += sCRLF
                    + "Total Time: " + ts.ToString().Remove(ts.ToString().LastIndexOf('.')) + " "
                    + listPb[iCnt].sStart.ToString() + " "
                    + listPb[iCnt].sEnd.ToString();

                // # of Error/Warning
                sBody += sCRLF
                    + "Errors: " + listPb[iCnt].iError.ToString();
                sBody += sCRLF
                    + "Warnings: " + listPb[iCnt].iWarning.ToString();

                if (sSection != "[Summary]")    // No details are needed in the Summary section
                {
                    // ToDo: 行番号でソートして表示する
                    if (listPb[iCnt].sError.Count > 0) sBody += sCRLF + "Error Details:";
                    for (int i = 0; i < listPb[iCnt].sError.Count; i++)
                        sBody += sCRLF + " " + listPb[iCnt].sError[i].ToString();
                    if (listPb[iCnt].sWarning.Count > 0) sBody += sCRLF + "Warning Details:";
                    for (int i = 0; i < listPb[iCnt].sWarning.Count; i++)
                        sBody += sCRLF + " " + listPb[iCnt].sWarning[i].ToString();
                }
                sBody += (sCRLF + sCRLF);
            }
        }

        public void LogParse(string logText)
        {
            // parse Start/End bloack
            FindBlock(logText, "Start:", "End:", listStartEnd);

            // parse Start Download Process/End bloack
            FindBlock(logText, "Start Download Process:", "End Download Process:", listDownload);

            // parse Start Upload Process/End bloack
            FindBlock(logText, "Start Upload Process:", "End Upload Process:", listUpload);


            // Country Date/Time format
            CultureInfo cFormat = new CultureInfo("ja-JP", false);

            // -- Formatting [Summary]
            sBody = sBody.Remove(0);            // Clear
            FormatBlock("[Summary]", 0, listStartEnd, cFormat);
            FormatBlock("[Download Process]", 0, listDownload, cFormat);
            FormatBlock("[Upload Process]", 0, listUpload, cFormat);


            sListSummary.Add(sBody);



            textBoxContents.Text = sBody;
        }

        private void checkBoxSMTPAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSMTPAuth.Checked)
            {
                bSMTPAuth = true;
                textBoxSMTPPort.Enabled = true;
                labelSMTPPort.Enabled = true;
                textBoxSMTPPassword.Enabled = true;
                labelSMTPPassword.Enabled = true;
            }
            else
            {
                bSMTPAuth = false;
                textBoxSMTPPort.Enabled = false;
                labelSMTPPort.Enabled = false;
                textBoxSMTPPassword.Enabled = false;
                labelSMTPPassword.Enabled = false;
            }
        }

        private void checkBoxSMTPSSL_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSMTPSSL.Checked)
            {
                bSMTPSSL = true;
                textBoxSMTPPort.Enabled = true;
                labelSMTPPort.Enabled = true;
                textBoxSMTPPassword.Enabled = true;
                labelSMTPPassword.Enabled = true;
            }
            else
            {
                bSMTPSSL = false;
                textBoxSMTPPort.Enabled = false;
                labelSMTPPort.Enabled = false;
                textBoxSMTPPassword.Enabled = false;
                labelSMTPPassword.Enabled = false;
            }
        }

    }
}
