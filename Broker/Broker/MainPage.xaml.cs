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

using Windows.ApplicationModel.Resources;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Broker
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static string PortNumber = "1337";
        static string HostNameString = "192.168.0.7";
        static bool fStart = true;
        SocketServer socketServer;
        ResourceLoader resourceLoader;

        public MainPage()
        {
            this.InitializeComponent();

            resourceLoader = ResourceLoader.GetForCurrentView();
            this.TextBlock_HostName.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Start.Content = resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop");
            this.TextBox_PortNumberValue.Text = PortNumber;

            socketServer = new SocketServer();
            this.TextBlock_HostNameValue.Text = socketServer.ServerHostName.ToString();

        }

        // Message handler sent by SocketServer object
        private void ReceiveSocketServerMessage(object sender, string message)
        {
            ListBox_Message.Items.Add(message);
        }

        // UI Control handlers 
        private async void Pbtn_Start_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Message.Items.Add(
                string.Format("{0} button was clicked.", resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop")));

            try
            {
                if (fStart)
                {
                    socketServer.SocketServerMessage += ReceiveSocketServerMessage;

                    await socketServer.Start(PortNumber);
                }
                else
                {
                    socketServer.Stop();
                    socketServer.SocketServerMessage -= ReceiveSocketServerMessage;
                }

                fStart = fStart ? false : true;   // toggle if success
                Pbtn_Start.Content = resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop");
            }
            catch (Exception ex)
            {
                ListBox_Message.Items.Add(ex.Message);
            }
        }
    }
}
