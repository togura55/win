using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Globalization;
using System.Diagnostics;

namespace QARepPars
{
    public class Program
    {
        public struct ProcBlock
        {
            public int iStart;          // Start char number
            public string sStart;       // Start line contents - Start XXX Process:date-time
            public string sStartKeyword;    // Line head word Start
            public int iEnd;            // End char number
            public string sEnd;         // End line contents - End XXX Process:date-time
            public string sEndKeyword;  // Line head word End
            public List<string> sError; // List of error descriptions
            public List<string> sWarning;   // List of warning descriptions
            public List<string> slcd;   // List of lcd line
        }
        static public List<ProcBlock> listStartEnd = new List<ProcBlock>();
        static public List<ProcBlock> listDownload = new List<ProcBlock>();
        static public List<ProcBlock> listUpload = new List<ProcBlock>();

        static private string[] slistErrorDesc = {"no such file or directory",
                                               "unable to change",
                                               "unable to open",
                                               "unknown command"};
        static private string[] slistErrorException = { "\"rem\"" };
        static private string[] slistWarningDesc = { "nothing matched" };
        static private string[] slistWarningException = { "\"rem\"" };

        public struct ErrorDesc
        {
            public string[] slDesc ;
            public string[] slExcept;

            public ErrorDesc(string[] _Error, string[] _Except)
            {
                slDesc = _Error;
                slExcept = _Except;
            }
        };

        static string sCRLF = System.Environment.NewLine;   // \n
        static string sSeparater = "--------------------------------------------";
        static string sLogFile = "";        // referred from static method
        static string sLogFilePath = "";    // Full path
        static private string sPCName = "";
        static string sAppName = "";
        static string sAppPath = "";
        static Settings conf = null;        // Dynamic properties

        public class Settings
        {
            private string _SMTPServer;
            private int _SMTPServerPort;
            private bool _SMTPAuth;
            private bool _SMTPSSL;
            private string _From;
            private string _To;
            private string _Cc;
            private string _Bcc;
            private string _Pwd;
            private string _Subject;
            private string _Body;
            private string _Sign;
            private string _FromDisplayName;
            private bool _SimpleDesc;

            public string SMTPServer { get { return _SMTPServer; } set { _SMTPServer = value; } }
            public int SMTPServerPort { get { return _SMTPServerPort; } set { _SMTPServerPort = value; } }
            public bool SMTPAuth { get { return _SMTPAuth; } set { _SMTPAuth = value; } }
            public bool SMTPSSL { get { return _SMTPSSL; } set { _SMTPSSL = value; } }
            public string From { get { return _From; } set { _From = value; } }           
            public string To { get { return _To; } set { _To = value; } }
            public string Cc { get { return _Cc; } set { _Cc = value; } }           
            public string Bcc { get { return _Bcc; } set { _Bcc = value; } }
            public string Pwd { get { return _Pwd; } set { _Pwd = value; } }           
            public string Subject { get { return _Subject; } set { _Subject = value; } }
            public string Body { get { return _Body; } set { _Body = value; } }           
            public string Sign { get { return _Sign; } set { _Sign = value; } }
            public string FromDisplayName { get { return _FromDisplayName; } set { _FromDisplayName = value; } }
            public bool SimpleDesc { get { return _SimpleDesc; } set { _SimpleDesc = value; } }

            public Settings()
            {
                _SMTPServer = "smtp.gmail.com";
                _SMTPServerPort = 587;
                _SMTPAuth = true;
                _SMTPSSL = true;
                _From = "test.sdljapan@gmail.com";
                _To = "togura@sdl.com";       // colon separate for multiple sending
                _Cc = "";
                _Bcc = "";
                _Pwd = "sdllab.2";
                _Subject = "";
                _Body = "This is a test mail";
                _Sign = "";
                _FromDisplayName = "QARepPars Notifier";
                _SimpleDesc = true;   // Show by simple description
            }
        }


        static int Main(string[] args)
        {
            // Get PC name
            IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
            sPCName = hostInfo.HostName;
            // Get App Name
            sAppName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            sAppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Create conf 
            conf = new Settings();

            // Check command line parameters
            if (ReadArgs(args) == -1) return -1;

            // Read/Set configuration
            if (ReadSettings(string.Format("{0}\\{1}.xml",sAppPath,sAppName), ref conf) == -1) return -1;

            // Read and parse log file
            if (LogParse(sLogFilePath) == -1) return -1;

            Debug.Write(conf.Body);
            

            // Succeeded? Go Send Mail
            if (doSendMail() == -1) return -1;

            return 0;
        }

        static private int ReadArgs(string [] args)
        {
            // 引数チェック
            if (args.Length == 0)
            {
                Console.WriteLine("Error: Num. of arguments: {0}\n", args.Length);
                return -1;
            }

            sLogFilePath = args[0];
            //sConfFilePath = args[1];

            Console.Write(string.Format("ReadArgs Completed.{0}",sCRLF));
            return 0;
        }

        static int ReadSettings(string fileName, ref Settings appSettings)
        {
            //System.IO.FileStream fs1 = null; 
            System.IO.FileStream fs2 = null;

            try
            {
                ////＜XMLファイルに書き込む＞
                ////XmlSerializerオブジェクトを作成
                ////書き込むオブジェクトの型を指定する
                //System.Xml.Serialization.XmlSerializer serializer1 =
                //    new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                ////ファイルを開く
                //System.IO.FileStream fs1 =
                //    new System.IO.FileStream(fileName, System.IO.FileMode.Create);
                ////シリアル化し、XMLファイルに保存する
                //serializer1.Serialize(fs1, appSettings);
                ////閉じる
                //fs1.Close();

                //＜XMLファイルから読み込む＞
                //XmlSerializerオブジェクトの作成
                System.Xml.Serialization.XmlSerializer serializer2 =
                    new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                //ファイルを開く
                fs2 = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
                //XMLファイルから読み込み、逆シリアル化する
                appSettings =
                    (Settings)serializer2.Deserialize(fs2);
            }
            catch (Exception e)
            {
                Console.Write(e.Message + sCRLF);
                return -1;
            }
            finally
            {
                if (fs2 != null)
                    //閉じる
                    fs2.Close();
            }

            Console.Write("ReadSettings Completed." + sCRLF);
            return 0;
        }

        static private int LogParse(string path)
        {
            try
            {
                // Logファイルを開いて中身をparse
                using (StreamReader sr = new StreamReader(
                    path, Encoding.GetEncoding("Shift_JIS")))   // Closed in IDisposable
                {
                    sLogFile = sr.ReadToEnd();

                    // parse Start/End bloack
                    FindBlock(sLogFile, "Start:", "End:", listStartEnd);

                    // parse Start Download Process/End bloack
                    FindBlock(sLogFile, "Start Download Process:", "End Download Process:", listDownload);

                    // parse Start Upload Process/End bloack
                    FindBlock(sLogFile, "Start Upload Process:", "End Upload Process:", listUpload);

                    // Country Date/Time format
                    CultureInfo cFormat = new CultureInfo("ja-JP", false);

                    // -- Format Process
                    conf.Body = conf.Body.Remove(0);            // Clear
                    if (listStartEnd.Count != 0) FormatBlock("[Summary]", listStartEnd[0], cFormat);
                    if (listDownload.Count != 0) FormatBlock("[Download Process]", listDownload[0], cFormat);
                    if (listUpload.Count != 0) FormatBlock("[Upload Process]", listUpload[0], cFormat);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message + sCRLF);
                return -1;
            }

            Console.Write("LogParse Completed." + sCRLF);
            return 0;
        }

        static private int doSendMail()
        {
            // Create Subject
            conf.Subject += string.Format(" [{0}] {1} from {2}", 
                DateTime.Today.ToString("yyyy/MM/dd"), sAppName, sPCName);
            // Add Signature
            FileVersionInfo ver
                = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            conf.Body += string.Format ("{0}{1}{2}{3}{4}{5} {6}",
                sCRLF,sSeparater,sCRLF,conf.Sign,sCRLF,sAppName,ver.FileVersion.ToString());

            // SMTP process
            SmtpClient sc;
            MailMessage msg = new MailMessage();
            try
            {
                if (!conf.SMTPAuth && !conf.SMTPSSL)
                {
                    // Traditional SMTP
                    sc = new SmtpClient();
                    sc.Host = conf.SMTPServer;
                }
                else
                {
                    sc = new SmtpClient(conf.SMTPServer, conf.SMTPServerPort);
                    sc.EnableSsl = conf.SMTPSSL;
                    if (conf.SMTPAuth)
                        sc.Credentials = new NetworkCredential(conf.From, conf.Pwd);
                }
                // 差出人
                MailAddress from = new MailAddress(conf.From, conf.FromDisplayName, 
                    Encoding.GetEncoding("iso-2022-jp"));

                // MailMessage を作成
                msg.From = from;
                msg.To.Add(conf.To);
                msg.Subject = conf.Subject;
                msg.SubjectEncoding = Encoding.GetEncoding("iso-2022-jp");
                msg.Body = conf.Body;
                if (conf.Cc.Length != 0) msg.CC.Add(conf.Cc);
                if (conf.Bcc.Length != 0) msg.Bcc.Add(conf.Bcc);

                sc.Send(msg);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + sCRLF);
                return -1;
            }

            Console.Write("Send Mail Successfully Completed." + sCRLF);
            return 0;
        }

        static void DetectErrors(ref ProcBlock pb, ErrorDesc ed, ref List<string> err, 
            string source)
        {
            for (int i = 0; i<ed.slDesc.Length; i++)
            {
                int iStart = pb.iStart;

                int iIndex = 0;
                while (iIndex != -1)
                {
                    iIndex = source.IndexOf(ed.slDesc[i], iStart, pb.iEnd - iStart);
                    if (iIndex != -1)
                    {
                        // 行番号を計算する
                        // 行頭まで戻って、1行を抜き出す。
                        string s = source.Substring(0, iIndex);
                        int iHead = source.LastIndexOf(sCRLF, iIndex);
                        string sLine = (source.Substring(iHead + sCRLF.Length,
                            (source.IndexOf(sCRLF, iIndex, pb.iEnd - iIndex) - (iHead + sCRLF.Length)))).TrimStart();

                        // 除外キーワードが含まれている場合は、スキップ
                        for (int j = 0; j < ed.slExcept.Length; j++)
                        {
                            if (ed.slExcept[j].Length == 0 || 
                                sLine.IndexOf(ed.slExcept[j]) == -1)
                            {
                                string sApx = "";
                                if (ed.slExcept[j].Length == 0)
                                {
                                    // sLineから余分な文字列を削除
                                    int n = sLine.IndexOf(ed.slDesc[j]) + ed.slDesc[j].Length + 1;
                                    sLine = sLine.Substring(n);
                                }
                                else
                                {
                                    // Errorにlocalかremoteの文字列で開始していたら、セクション情報を付加する。
                                    string[] slAppendix = { "local", "remote", "nothing matched" };
                                    for (int k = 0; k < slAppendix.Length; k++)
                                    {
                                        if (sLine.IndexOf(slAppendix[k] + ":") == 0)
                                        {
                                            // セクション情報の前方検索
                                            int iSec = source.LastIndexOf(slAppendix[k] + " directory is", iHead);
                                            if (iSec != -1)
                                            {
                                                string str = source.Substring(iSec, 
                                                    source.IndexOf(sCRLF, iSec) - iSec);
                                                str = str.Replace(slAppendix[k] + " directory is", "").TrimStart();
                                                if (conf.SimpleDesc)    // 簡易表示
                                                {
                                                    string strTmp = str;
                                                    if (DescSimplify(ref strTmp) != -1)
                                                        str = strTmp;
                                                }
                                                sApx = string.Format(" in \"{0}\"", str);
                                            }
                                        }
                                    }
                                }

                                err.Add(string.Format("[{0}]{1}{2}",
                                    (s.Length-s.Replace(sCRLF, "").Length).ToString(), sLine,sApx));
                            }
                        }

                        iStart = iIndex+1;
                    }
                }
            }

        }

        static int FindKeyword(string source, string value, ref int startIndex, ref int endIndex)
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

        static bool FindBlock(string source, string begin, string end, List<ProcBlock> listPb)
        {
            bool bRet = true;

            int startIndex = 0;
            int endIndex = 0;
            while (bRet)
            {
                if (FindKeyword(source, begin, ref startIndex, ref endIndex) == 0)
                {
                    ProcBlock item = new ProcBlock();
                    item.iStart = startIndex;
                    item.sStart = source.Substring(startIndex, endIndex - startIndex);
                    item.sStartKeyword = begin;
                    item.sError = new List<string>();
                    item.sWarning = new List<string>();
                    item.slcd = new List<string>();
                    listPb.Add(item);

                    startIndex = endIndex;
                    endIndex = 0;
                    if (FindKeyword(source, end, ref startIndex, ref endIndex) == 0)
                    {
                        int i = listPb.Count - 1;
                        ProcBlock pb = listPb[i];   // copy to temporary struct
                        pb.iEnd = startIndex;       // cannot edit member directly for generic collection
                        pb.sEnd = source.Substring(startIndex, endIndex - startIndex);
                        pb.sEndKeyword = end;

                        // Detect Errors/Warning
                        DetectErrors(ref pb, new ErrorDesc(slistErrorDesc, slistErrorException), 
                            ref pb.sError, source);
                        DetectErrors(ref pb, new ErrorDesc(slistWarningDesc, slistWarningException), 
                            ref pb.sWarning, source);

                        // Search all "New local directory is" 
                        string[] sllcd = { "New local directory is" };
                        string[] sldummy = { "" };
                        DetectErrors(ref pb, new ErrorDesc(sllcd, sldummy), 
                            ref pb.slcd, source);
 
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

        static int DescSimplify(ref string str)
        {
            int start = 0;
            int numFw = 2, numBk = 3;
            for (int i = 0; i < numFw; i++)
            {
                start = str.IndexOf("\\", start);
                if (start != -1)
                {
                    // \が2個続いているかの判定, ネットワークパス参照の場合など
                    if (str.IndexOf("\\", start+1, 1) != -1) start++; // Yes, increment
                    start++;
                }
                else
                    return -1;
            }

            int end = str.Length;
            for (int i = 0; i < numBk; i++)
            {
                end = str.LastIndexOf("\\", end);
                if (end != -1)
                    end--;
                else
                    return -1;
            }

            str = str.Substring(0, start) + "..." + str.Substring(end+1);

            return 0;
        }

        static void FormatBlock(string sProcess, ProcBlock Pb, CultureInfo cFormat)
        {
            // Add Process name
            conf.Body += sProcess;
            if (sProcess == "[Summary]")
            {
                conf.Body += string.Format("{0}Date: {1}{2}Reported from: {3}",
                    sCRLF, DateTime.Today.ToString("yyyy/MM/dd"),sCRLF,sPCName);
            }
            // Total Time
            DateTime dtStart 
                = DateTime.Parse((Pb.sStart.Replace(Pb.sStartKeyword, "").Replace("-", " ")), cFormat);
            DateTime dtEnd 
                = DateTime.Parse((Pb.sEnd.Replace(Pb.sEndKeyword, "").Replace("-", " ")), cFormat);
            TimeSpan ts = dtEnd - dtStart;
            string t = ts.ToString();
            if (t.LastIndexOf('.') != -1) t = t.Remove(t.LastIndexOf('.'));
            conf.Body += string.Format("{0}Total Time: {1}, {2}, {3}", 
                sCRLF,t,Pb.sStart,Pb.sEnd);

            // lcdで表示される全Sectionを表示
            if (sProcess != "[Summary]")
            {
                conf.Body += string.Format("{0}Sections: {1}", sCRLF, Pb.slcd.Count.ToString());

                // 含まれるlcd行の表示
                conf.Body += string.Format("{0}Section Details:", sCRLF);
                for (int i = 0; i < Pb.slcd.Count; i++)
                {
                    string str = Pb.slcd[i];
                    if (conf.SimpleDesc)    // 簡易表示
                    {
                        if (DescSimplify(ref str) != -1)
                            Pb.slcd[i] = str;
                    }
                    conf.Body += string.Format("{0} {1}", sCRLF, str);
                }
            }

            // # of Error/Warning
            conf.Body += string.Format("{0}Errors: {1}", sCRLF, Pb.sError.Count.ToString());
            conf.Body += string.Format("{0}Warnings: {1}", sCRLF, Pb.sWarning.Count.ToString());

            if (sProcess != "[Summary]")    // No details are needed in the Summary section
            {
                // Error/Warning 行番号でソートして表示する
                Pb.sError.Sort();
                if (Pb.sError.Count > 0) conf.Body += string.Format("{0}Error Details:", sCRLF);
                for (int i = 0; i < Pb.sError.Count; i++)
                    conf.Body += string.Format("{0} {1}", sCRLF, Pb.sError[i]);
                Pb.sWarning.Sort();
                if (Pb.sWarning.Count > 0) conf.Body += string.Format("{0}Warning Details:", sCRLF);
                for (int i = 0; i < Pb.sWarning.Count; i++)
                    conf.Body += string.Format("{0} {1}", sCRLF, Pb.sWarning[i]);
            }
            conf.Body += (sCRLF + sCRLF);
        }
    }
}
