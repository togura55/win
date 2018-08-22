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
using System.Timers;

namespace RebootPC
{

    public partial class Form1 : Form
    {
        string configfile = "config.xml";

        //       private Timer timer = null;
        RebootPc rp = null;

        public Form1()
        {
            InitializeComponent();

            try
            {
                rp = (RebootPc)XmlDeserialize(configfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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
                Label_Delay.Text = Properties.Resources.Label_Delay;


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
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetName().Version.ToString();
        }

        private void UpdateUi()
        {
            TextBox_Timeout.Text = rp.timeout.ToString();
            TextBox_MaxCount.Text = rp.maxCount.ToString();
            Label_CounterValue.Text = rp.counter.ToString();
            Pbtn_Start.Text = (rp.start) ? Properties.Resources.Pbtn_Stop : Properties.Resources.Pbtn_Start;
            TextBox_FilePath.Text = rp.filepath.ToString();
            TextBox_Delay.Text = rp.extapp_delay.ToString();
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
            rp.extapp_delay = Int32.Parse(TextBox_Delay.Text);
        }

        static System.Timers.Timer execloop_timer;

        private bool ExecLoopProcessTimer(int sec)
        {
            try
            {
                execloop_timer = new System.Timers.Timer();
                {
                    execloop_timer.Enabled = true;
                    execloop_timer.AutoReset = true;
                    execloop_timer.Interval = sec * 1000;    // msec
                    execloop_timer.Elapsed += new ElapsedEventHandler(OnElapsed_ExecLoopProcessTimer);

                    execloop_timer.Start();

                    return true;
                }
            }
            catch (ArgumentException aex)
            {
                return false;
            }
        }

        static void OnElapsed_ExecLoopProcessTimer(object sender, ElapsedEventArgs e)
        {
            execloop_timer.Stop();

            ExecLoopProcess();
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
                        throw new Exception(String.Format("No files are existed: {0}", rp.filepath));
                    }
                    // 以外は正常実行
                    else
                    {
                        result = DelayedStart(rp.extapp_delay);
                        
                    }

                    if (result)
                    {
                        XmlSerialize(configfile, rp);
 //                       rp.Run(rp.mode, rp.timeout);   // Go reboot/shutdown
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
                        null, Pbtn_Start, new object[] { EventArgs.Empty });

                    res = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text);
            }

            return res;
        }


        static System.Timers.Timer timer; 

        private bool DelayedStart(int sec)
        {
            try
            {
                timer = new System.Timers.Timer();
                {
                    filepath = rp.filepath;

                    //           var timer = new Timer();
                    timer.Enabled = true;
                    timer.AutoReset = true;
                    timer.Interval = sec * 1000;    // msec
                    timer.Elapsed += new ElapsedEventHandler(OnElapsed_TimersTimer);

                    timer.Start();

                    return true;
                }
            }
            catch (ArgumentException aex)
            {
                return false;
            }
        }

        static string filepath;

        static void OnElapsed_TimersTimer(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            // launch
            Process p = new Process();
            p.StartInfo.FileName = filepath;
            bool result = p.Start();  // return true when success
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
            rp.start = !rp.start;
            Pbtn_Start.Text = (rp.start) ? Properties.Resources.Pbtn_Stop : Properties.Resources.Pbtn_Start;

            if (!rp.start)
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
            XmlSerialize("config.xml", rp);

            Application.Exit();
        }

        private void Pbtn_Reset_Click(object sender, EventArgs e)
        {
            rp.ResetSettings();
            UpdateUi();
            XmlSerialize(configfile, rp);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Write state
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
