using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Networking;
using Windows.Networking.Connectivity;
using System.Threading.Tasks;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Publisher
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Every protocol typically has a standard port number. For example, HTTP is typically 80, FTP is 20 and 21, etc.
        // For this example, we'll choose an arbitrary port number.
        static string PortNumber = "1337";
        static string HostNameString = "192.168.0.7";

        public MainPage()
        {
            this.InitializeComponent();

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");

            this.TextBox_HostName.Text = HostNameString;
            this.TextBox_PortNumber.Text = PortNumber;
        }

        private void GetUiState()
        {
            HostNameString = this.TextBox_HostName.Text;
            PortNumber = this.TextBox_PortNumber.Text;
        }

        private void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            try
            {
                SocketClient socketClient = new SocketClient();
                //           socketClient.StartClient(HostNameString, PortNumber);

                socketClient.Connect(HostNameString, PortNumber);

                //socketClient.Send("Hello World");

                //socketClient.Receive();

                //socketClient.Disonnect();

                clientListBox.Items.Add(string.Format("{0}", "Completed."));
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("{0}", ex.Message));
            }
        }



        private async void Pbtn_Test_Click(object sender, RoutedEventArgs e)
        {
            clientListBox.Items.Add(string.Format("{0}", "Start Test"));
            try
            {
                await Test();
                //if (ret.Result)
                //    msg = "true";
                //else
                //    msg = "false";
                clientListBox.Items.Add(string.Format("{0}", "call Test() completed."));
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("{0}", ex.Message));
            }
        }

        private async Task Test()
        {
            try
            {
                await Task.Run(() => { throw (new Exception("Exception in Test().")); }).ConfigureAwait(false);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //private void Pbtn_Test_Click(object sender, RoutedEventArgs e)
        //{
        //    clientListBox.Items.Add(string.Format("{0}", "Start Test"));
        //    string msg;
        //    try
        //    {
        //        Task<bool> ret = Test();
        //        if (ret.Result)
        //            msg = "true";
        //        else
        //            msg = "false";
        //        clientListBox.Items.Add(string.Format("{0}", msg));
        //    }
        //    catch (Exception ex)
        //    {
        //        clientListBox.Items.Add(string.Format("{0}", ex.Message));
        //    }
        //}

        //private async Task<bool> Test()
        //{
        //    bool ret = false;
        //    try
        //    {
        //        await Task.Run(() => { throw (new Exception("Exception in Test().")); }).ConfigureAwait(false);
        //        ret = true;
        //    }

        //    catch (Exception ex)
        //    {
        //        ret = false;
        //        throw;
        //    }

        //    return ret;
        //}
    }
}
