using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace FindSignpad
{
    public partial class Form1 : Form
    {
        static bool _shouldStop;
        private Thread workerThread = null;
        delegate void SetTextCallback(string text);

        public class usbdeviceid
        {
            public string id;
            public string description;
        }
        public List<usbdeviceid> venderlist;    // USB VenderID
        public List<usbdeviceid> productlist;   // USB Product ID
        public List<usbdeviceid> configlist;    // config list  

        // parameters read from config.lst
        int duration = 1000;
        string logfile = "log.txt";
        bool showbuildversion = true;
        bool autostart = false;
        bool minimized = false;
        bool appendlog = true;
        bool logwhenchanged = true;
        bool showid = false;

        public Form1()
        {
            InitializeComponent();

            _shouldStop = false;

            configlist = new List<usbdeviceid>();
            ReadIdList("config.lst", ref configlist);
            string d = FindDescription("duration", configlist);
            if (d != string.Empty) duration = int.Parse(d);
            d = FindDescription("logfile", configlist);
            if (d != string.Empty) logfile = d;
            d = FindDescription("autostart", configlist);
            if (d == "yes") autostart = true;
            d = FindDescription("minimized", configlist);
            if (d == "yes") minimized = true;
            d = FindDescription("appendlog", configlist);
            if (d == "no") appendlog = false;
            d = FindDescription("logwhenchanged", configlist);
            if (d == "no") logwhenchanged = false;
            d = FindDescription("showid", configlist);
            if (d == "yes") showid = true;

            venderlist = new List<usbdeviceid>();
            ReadIdList("venderid.lst", ref venderlist, "|");
            productlist = new List<usbdeviceid>();
            ReadIdList("productid.lst", ref productlist, "|");
        }

        private void SetText(string text)
        {
            if (this.labelOutput.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.labelOutput.Text = text;
            }
        }

        private void ThreadProcSafe()
        {
            int defCount = -1;

            int prevCount = defCount;

            while (!_shouldStop)
            {
                wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();
                if (usbDevices.Count != 0)
                {
                    try
                    {
                        if (!logwhenchanged)
                            prevCount = defCount;

                        if (prevCount != usbDevices.Count)
                        {
                            wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device
                            string product = FindDescription(usbDevice.idProduct.ToString(), productlist, "Unknown", true);
                            string vender = FindDescription(usbDevice.idVendor.ToString(), venderlist, "Unknown", true);
                            if (showid)
                            {
                                product += " " + usbDevice.idProduct.ToString();
                                vender += " " + usbDevice.idVendor.ToString();
                            }
                            ProcessMessage(String.Format("Product: {0}, Vender: {1}", product, vender));

                        }
                        prevCount = usbDevices.Count;

                        Thread.Sleep(duration);
                    }
                    catch (Exception ex)
                    {
                        //                        RequestStop();
                        //                        MessageBox.Show(ex.Message);
                        ProcessMessage(ex.Message, false);
                    }
                }
                else
                {
                    if (prevCount != usbDevices.Count)
                    {
                        ProcessMessage(String.Format("No STU devices attached"));
                    }
                    prevCount = usbDevices.Count;
                }
            }
            Console.WriteLine("worker thread: terminating gracefully.");
        }

        private string ProcessMessage(string msg, bool show_msg = true)
        {
            string str;

            DateTime dt = DateTime.Now;
            str = String.Format("{0}/{1} {2}:{3}:{4}:{5} {6}",
                dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, msg);
            Debug.WriteLine(str);

            if (show_msg) SetText(str);

            WriteLog(str);

            return str;
        }

        private void WriteLog(string str, bool append = true)
        {
            string path = Directory.GetCurrentDirectory();
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            StreamWriter writer = new StreamWriter(path + "\\" + logfile, append, sjisEnc);

            if(append) writer.WriteLine(str);
            writer.Close();
        }

        public string FindDescription(string id, List<usbdeviceid> devicelist, string defid = "", bool append = false)
        {
            string str = defid;
           
            foreach (usbdeviceid obj in devicelist)
            {
                if (obj.id == id)
                {
                    str = obj.description;
                    break;
                }
            }

            if (str == defid && append)
                str +=  String.Format(" ({0})", id);

            return str;
        }

        public void ReadIdList(string file, ref List<usbdeviceid> mylist, string separater = "=")
        {
            string path = Directory.GetCurrentDirectory();

            try
            {
                // read and set vender id
                StreamReader cReader = (new StreamReader(path + "\\" + file, Encoding.Default));

                while (cReader.Peek() >= 0)
                {
                    string stBuffer = cReader.ReadLine();

                    char sep = separater[0];
                    string[] stArrayData = stBuffer.Split(sep);

                    mylist.Add(new usbdeviceid());
                    mylist[mylist.Count - 1].id = stArrayData[0].Trim();  //id;
                    mylist[mylist.Count - 1].description = stArrayData[1].Trim();//description;
                }

                cReader.Close();
            }
            catch (Exception ex)
            {
                ProcessMessage(ex.Message, false);
            }
        }

        // ---------------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            pbtnStart.Text = "Start";
            pbtnStop.Text = "Stop";
            pbtnStop.Enabled = false;
            labelOutput.Text = string.Empty;

            string version = string.Empty;
            if (showbuildversion)
            {
                var assm = Assembly.GetExecutingAssembly();
                var name = assm.GetName();
                version = name.Version.ToString();
            }

            Text = Path.GetFileNameWithoutExtension(Application.ExecutablePath)
                + " " + version;

            if (autostart)
            {
                pbtnStart.Enabled = !pbtnStart.Enabled;
                pbtnStop.Enabled = !pbtnStop.Enabled;
                StartThread();
            }

            if (minimized)
                this.WindowState = FormWindowState.Minimized;

            if (!appendlog) WriteLog("", false);
        }

        private void StartThread()
        {
            ProcessMessage("Start", false);

            workerThread = new Thread(ThreadProcSafe);
            workerThread.Start();

            //// Loop until worker thread activates.
            while (!workerThread.IsAlive) ;

            //// Put the main thread to sleep for N millisecond to
            //// allow the worker thread to do some work:
 //           Thread.Sleep(duration);
        }

        private void pbtnStart_Click(object sender, EventArgs e)
        {
            pbtnStart.Enabled = !pbtnStart.Enabled;
            pbtnStop.Enabled = !pbtnStop.Enabled;
            StartThread();
        }

        private void pbtnStop_Click(object sender, EventArgs e)
        {
            //// Request that the worker thread stop itself:
            //workerObject.RequestStop();

            //// Use the Join method to block the current thread 
            //// until the object's thread terminates.
            //workerThread.Join();
            workerThread.Abort();
            Console.WriteLine("main thread: Worker thread has terminated.");

            ProcessMessage("Stop", false);
            pbtnStart.Enabled = !pbtnStart.Enabled;
            pbtnStop.Enabled = !pbtnStop.Enabled;
        }
    }
}
