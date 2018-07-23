using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace RebootPC
{

    public partial class Form1 : Form
    {
        string configfile = "config.xml";

        //       private Timer timer = null;
        RebootPc rp = null;

        private void lang()
        {
            try
            {
                string culture = Thread.CurrentThread.CurrentCulture.Name;
                //               Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                // C#
                // Declare a Resource Manager instance.
                System.Resources.ResourceManager LocRM =
                    new System.Resources.ResourceManager("RebootPC.Properties.Resources",
                    Assembly.GetExecutingAssembly());
 //                   typeof(Form1).Assembly);
                // Assign the string for the "strMessage" key to a message box.
                MessageBox.Show(LocRM.GetString("GroupBox_Mode"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public Form1()
        {
            InitializeComponent();

            //               rp = new RebootPc();  
//            lang();

            try
            {
//                rp.ReadSettings();
                rp = (RebootPc)XmlDeserialize(configfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //            if (stream != null)
            {
                // Initialize UI strings
                this.Text = string.Format("{0} {1}", GetAppName(), GetAppVersion());
                Rbtn_Reboot.Text = Properties.Resources.Rbtn_Reboot;
                Rbtn_Shutdown.Text = Properties.Resources.Rbtn_Shutdown;
                Label_Timeout.Text = Properties.Resources.Label_Timeout;
                Label_MaxCount.Text = Properties.Resources.Label_MaxCount;
                Label_Messages.Text = Properties.Resources.Label_Messages;
                Pbtn_Close.Text = Properties.Resources.Pbtn_Close;
                GroupBox_Mode.Text = Properties.Resources.GroupBox_Mode;
                Label_Counter.Text = Properties.Resources.Label_Counter;
                Pbtn_Reset.Text = Properties.Resources.Pbtn_Reset;
                Label_ExtApp.Text = Properties.Resources.Label_ExtApp;
                Pbtn_FilePath.Text = Properties.Resources.Pbtn_FilePath;

                // Set UI state and values

                UpdateUi();

                if (rp.start)
                {
                    ExecLoopProcess();
                }


                //           if (!start)
                //               StartTimer(MODE_REBOOT, timeout);
            }
            //           else
            //               this.Close();

        }

        private string GetAppName()
        {
            AssemblyTitleAttribute asmttl =
                (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            return asmttl.Title;
        }
        private string GetAppVersion()
        {
            Assembly asm =Assembly.GetExecutingAssembly();
            return asm.GetName().Version.ToString();
        }

        private void UpdateUi()
        {
            TextBox_Timeout.Text = rp.timeout.ToString();
            TextBox_MaxCount.Text =rp. maxCount.ToString();
            Label_CounterValue.Text = rp.counter.ToString();
            Pbtn_Start.Text = (rp.start) ? Properties.Resources.Pbtn_Stop : Properties.Resources.Pbtn_Start;
            TextBox_FilePath.Text = rp.filepath.ToString();
            SetModeState();
        }

        private void UpdateParam()
        {
            rp.timeout = Int32.Parse(TextBox_Timeout.Text);
            rp.maxCount = Int32.Parse(TextBox_MaxCount.Text);
            GetModeState();
            // start = 
            // counter = 
            rp.filepath = TextBox_FilePath.Text;
        }

        private bool ExecLoopProcess()
        {
            bool res = true;

            rp.counter++;
            try
            {
                if (rp.counter <= rp.maxCount)
                {
                    bool result = false;
                    // 何もなければ何もしない=未設定状態
                    if (rp.filepath == string.Empty)
                    {
                        result = false;
                    }
                    // あるけど存在しない場合は例外発生
                    else if (!File.Exists(rp.filepath))
                    {
                        // 例外発生させる
                        result = false;
                        throw new Exception(String.Format("No files are existed: {0}",rp.filepath));
                    }
                    // 以外は正常実行
                     else
                    {
                        // launch
                        //Processオブジェクトを作成する
                        Process p = new Process();
                        p.StartInfo.FileName = rp.filepath;
                        //起動する。プロセスが起動した時はTrueを返す。
                        result = p.Start();
                    }

                    if (result)
                    {
 //                       rp.WriteSettings();
                        XmlSerialize(configfile, rp);
                        rp.Run(rp.mode, rp.timeout);
                    }
                    else
                    {
                        throw new Exception(String.Format("Startup failed: {0}", rp.filepath));
                    }
                }
                else
                {
                    Label_Messages.Text = String.Format(Properties.Resources.Msg_LoopEnd, rp.maxCount);
                    Pbtn_Start.GetType().InvokeMember("OnClick",
                        BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                        null,Pbtn_Start, new object[] { EventArgs.Empty });

                    res = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text);
            }

            return res;
        }

        private void SetModeState()
        {
            if (rp.mode == RebootPc.MODE_REBOOT)
                Rbtn_Reboot.Checked = true;
            else if (rp.mode == RebootPc.MODE_SHUTDOWN)
                Rbtn_Shutdown.Checked = true;
            else { }
        }

        private void GetModeState()
        {
            if (Rbtn_Reboot.Checked == true)
                rp.mode = RebootPc.MODE_REBOOT;
            else if (Rbtn_Shutdown.Checked == true)
                rp.mode = RebootPc.MODE_SHUTDOWN;
            else { }
        }

        //private void StartTimer(int mode, int timeout = 30)
        //{
        //    // set the form components
        //    this.timer = new Timer(this.components);
        //    this.timer.Interval = timeout * 1000;
        //    this.timer.Start();

        //    // Tick timer event 
        //    this.timer.Tick += (semder, e) =>
        //    {
        //        reboot.Run(mode, timeout);

        //        StopTimer();
        //    };
        //}

        //private void StopTimer()
        //{
        //    // Stop timer
        //    this.timer.Stop();

        //    // releasing
        //    using (this.timer) { }
        //}

        private void XmlSerialize(string fileName, object obj)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(RebootPc));
            StreamWriter sw = new StreamWriter(
                fileName, false, new UTF8Encoding(false));
            serializer.Serialize(sw, obj);
            sw.Close();
        }

        private object XmlDeserialize(string fileName)
        {
            RebootPc obj;

            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(RebootPc));
            StreamReader sr = new StreamReader(
                fileName, new UTF8Encoding(false));
            obj = (RebootPc)serializer.Deserialize(sr);
            sr.Close();

            return obj;
        }

        private void Pbtn_Start_Click(object sender, EventArgs e)
        {
//            MessageBox.Show("Click");
            rp.start = !rp.start;
            Pbtn_Start.Text = (rp.start) ? Properties.Resources.Pbtn_Stop : Properties.Resources.Pbtn_Start;

            if (!rp.start)
//                rp.WriteSettings();
                  XmlSerialize(configfile, rp);
            else
            {
                UpdateParam();
                ExecLoopProcess();
            }
            //if (!start)
            //    StartTimer(timeout);
            //else
            //    StopTimer();
        }

        // Save the current settings and close app
        private void Pbtn_Close_Click(object sender, EventArgs e)
        {
            // Write state
            UpdateParam();  // UI to valuables
 //           rp.WriteSettings();
            XmlSerialize("config.xml", rp);

            Application.Exit();
        }

        private void Pbtn_Reset_Click(object sender, EventArgs e)
        {
            rp.ResetSettings();
            UpdateUi();
//            rp.WriteSettings();
            XmlSerialize(configfile, rp);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Write state
//            rp.WriteSettings();
            XmlSerialize(configfile, rp);

            //ApplicationExitイベントハンドラを削除
            Application.ApplicationExit -= new EventHandler(Application_ApplicationExit);
        }

        private void Pbtn_FilePath_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    FileName = string.Empty,
                    //           ofd.InitialDirectory = @"C:\";
                    InitialDirectory = string.Empty,  // current directory
                    Filter = "All Files(*.*)|*.*",
                    FilterIndex = 2,
                    //set the title
                    Title = "Please set the File",
                    RestoreDirectory = true,
                    CheckFileExists = true,
                    CheckPathExists = true
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
 //                   string path = string.Empty;
                    rp.filepath = ofd.FileName;    // full path + filename + extension
                    TextBox_FilePath.Text = rp.filepath;                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
