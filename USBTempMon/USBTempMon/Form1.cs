using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace USBTempMon
{
    public partial class MainWindow : Form
    {
        public class UnManagedDll : IDisposable
        {
            [DllImport("kernel32")]
            static extern int LoadLibrary(string lpFileName);
            [DllImport("kernel32")]
            static extern IntPtr GetProcAddress(int hModule, string lpProcName);
            [DllImport("kernel32")]
            static extern bool FreeLibrary(int hModule);

            int moduleHandle;

            public UnManagedDll(string lpFileName)
            {
                moduleHandle = LoadLibrary(lpFileName);
            }

            public int ModuleHandle
            {
                get
                {
                    return moduleHandle;
                }
            }

            public T GetProcDelegate<T>(string method) where T : class
            {
                IntPtr methodHandle = GetProcAddress(moduleHandle, method);
                T r = Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(T)) as T;
                return r;
            }

            public void Dispose()
            {
                FreeLibrary(moduleHandle);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Initialize UI
//          Labelコントロールで与えられた範囲にテキストを自動改行させて表示させたい場合は、
//          AutoSizeプロパティをFalseにし、Labelコントロールの幅と高さを設定すれば、
//          Labelコントロール内にテキストを自動改行して表示させることができます。
            this.Text = Properties.Resources.IDS_CAP_MAINWINDOW;
            DetectSensor.Text = Properties.Resources.IDS_PBTN_DETECTSENSOR;
            DetectSensorText.Text = Properties.Resources.IDS_TXT_DETECTSENSORSTAT_NO;
            SetLED1.Text = Properties.Resources.IDS_PBTN_LED1;
            SetLED2.Text = Properties.Resources.IDS_PBTN_LED2;
            HeaterStatText.Text = Properties.Resources.IDS_TXT_HEATERSTAT;
            SetHeater.Text = Properties.Resources.IDS_PBTN_HEATER;
            HeaterDescText.Text = Properties.Resources.IDS_TXT_HEATERDESC;
            TempText.Text = Properties.Resources.IDS_TXT_TEMP;
            HumidText.Text = Properties.Resources.IDS_TXT_HUMID;
        }

        // GetVer (ByVal str)
        // FindUSB (ByRef Long)
        // GetTmpHumid (ByVal str, ByRef double, ByRef double)
        // ControlIO (ByVal str, ByVal Long, ByVal Long)
        // SetHeater (ByVal str, ByVal Long)
        // GetTempHumidTrue (ByVal str, ByRef double, ByRef double)

        delegate string GetVersDelegate(string str);
        delegate IntPtr FindUSBDelegate(ref int index);

        private void DetectSensor_Click(object sender, EventArgs e)
        {
            using (UnManagedDll usbMeterDll = new UnManagedDll(Properties.Resources.IDS_GLB_USBDLLNAME))
            {
                if (usbMeterDll.ModuleHandle != 0)
                {
                   //  string str = "";
                   IntPtr p;
                   int index = 0;
                   string dev = "";

                   //GetVersDelegate GetVers =
                   //     usbMeterDll.GetProcDelegate<GetVersDelegate>("GetVers");

                   //dev = GetVers(dev);

                   FindUSBDelegate FindUSB =
                       usbMeterDll.GetProcDelegate<FindUSBDelegate>("_FindUSB@4");
                   try
                   {
                       p = FindUSB(ref index);
                       dev = Marshal.PtrToStringAnsi(p);
                       if (dev.Length != 0)
                       {

                       }
                   }
                   catch
                   {
                       Console.WriteLine("Exception for FindUSB\n");
                   }
                   

                }
                else
                {
                    MessageBox.Show(string.Format(
                        Properties.Resources.IDS_TXT_USBDLLLOADERROR, 
                        Properties.Resources.IDS_GLB_USBDLLNAME));
                }
            }
        }

        private void SetLED1_Click(object sender, EventArgs e)
        {

        }

        private void SetLED2_Click(object sender, EventArgs e)
        {

        }

        private void SetHeater_Click(object sender, EventArgs e)
        {

        }
    }
}
