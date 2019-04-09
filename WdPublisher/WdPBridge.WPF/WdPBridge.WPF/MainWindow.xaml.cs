using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WdPBridge.WPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppServiceConnection _appServiceConnection;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Start in minimized window
            this.WindowState = WindowState.Minimized;

            await ConnectAsync();
        }

        private async Task<bool> ConnectAsync()
        {
            if (_appServiceConnection != null)
            {
                return true;
            }

            var appServiceConnection = new AppServiceConnection();
            appServiceConnection.AppServiceName = "InProcessAppService";
            appServiceConnection.PackageFamilyName = Package.Current.Id.FamilyName;
            appServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
            var r = await appServiceConnection.OpenAsync() == AppServiceConnectionStatus.Success;
            if (r)
            {
                _appServiceConnection = appServiceConnection;
            }

            return r;
        }

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!(await ConnectAsync()))
        //    {
        //        MessageBox.Show($"Failed");
        //        return;
        //    }

        //    var res = await _appServiceConnection.SendMessageAsync(new ValueSet
        //    {
        //        ["Input"] = inputTextBox.Text,
        //    });

        //    logTextBlock.Text = res.Message["Result"] as string;
        //}

        private void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            void setText()
            {
                //                logTextBlock.Text = (string)args.Request.Message["Now"];
                string s = (string)args.Request.Message["Now"];

                bool quit_flag = false;
                int mode = MODE_SHUTDOWN;
                switch (s)
                {
                    case "shutdown":
                        mode = MODE_SHUTDOWN;
                        break;

                    case "restart":
                        mode = MODE_REBOOT;
                        break;

                    case "quit":
                        quit_flag = true;
                        break;

                    default:
                        break;
                }

                if (!quit_flag)
                    Run(mode, 0);
                else
                {
                    Application.Current.Shutdown();
                }
            }

            if (Dispatcher.CheckAccess())
            {
                setText();
            }
            else
            {
                Dispatcher.Invoke(() => setText());
            }
        }

        public const int MODE_REBOOT = 0;
        public const int MODE_SHUTDOWN = 1;
        public void Run(int mo, int timeout, bool nowarning = true)
        {
            string arguments = string.Empty;
            string[] modes = { "-r", "-s" };
            string f = "-f";

            if (!nowarning)
                f = string.Empty;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "shutdown.exe";

                switch (mo)
                {
                    case MODE_REBOOT:
                    case MODE_SHUTDOWN:
                        //                        arguments = modes[mo] + " " + "-t" + " " + timeout.ToString();
                        arguments = modes[mo] + " " + f;  // immediately execute, delay is ensured by the caller process
                        break;

                    default:
                        break;
                }
                psi.Arguments = arguments;

                // psi.Arguments = "-s -t 0";   // shutdown
                //psi.Arguments = "-r -t 0";   // reboot
                psi.CreateNoWindow = true;
                Process p = Process.Start(psi);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
