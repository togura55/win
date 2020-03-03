using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace SignPro_Api_Demo_Desktop
{
    public partial class ExeForm : Form
    {
        public ExeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Functions por set the position of a window
        /// </summary>
        [DllImport("user32")]
        public static extern long SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int y, int cx, int cy, int wFlagslong);
        const short SWP_NOSIZE = 0x0001;
        const short SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(
            IntPtr hwnd,
            int nIndex
        );

        const int WS_THICKFRAME = 0x00040000;
        const int GWL_STYLE = -16;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(
            IntPtr hwnd,
            int nIndex,
            int dwNewLong
        );

        public void CallExeOnMonitor(string Exe, string Param, int nMonitor)
        {
            Process process = new Process();
            int numberMonitor;
            Rectangle monitor;
            process.StartInfo.FileName = Exe;
            process.StartInfo.Arguments = Param;
            numberMonitor = nMonitor;
            process.Start();
            //get the handle for window this is neccesary for get the handle
            process.WaitForInputIdle();
            Thread.Sleep(3000);
            if (numberMonitor >= 1)
            {
                if (Screen.AllScreens.Length < numberMonitor)
                {
                    Close();
                }
                else
                {
                    numberMonitor--;
                    //Get the data of the monitor
                    monitor = Screen.AllScreens[numberMonitor].WorkingArea;
                    //change the window to the second monitor
                    SetWindowPos(process.MainWindowHandle, 0,
                    monitor.Left, monitor.Top, monitor.Width,
                    monitor.Height, 0);
                    Close();
                }
            }
        }
    }
}
