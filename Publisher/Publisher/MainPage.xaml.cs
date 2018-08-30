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
using Windows.Storage;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Publisher
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static SocketClient socketClient = new SocketClient();  // suppose single instance
 //       static string configfile = "config.xml";

        // Every protocol typically has a standard port number. For example, HTTP is typically 80, FTP is 20 and 21, etc.
        // For this example, we'll choose an arbitrary port number.
//        static string PortNumber = "1337";
//        static string HostNameString = "192.168.0.7";

        public MainPage()
        {
            this.InitializeComponent();

            socketClient = new SocketClient();
            RestoreSettings();

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");

            this.TextBox_HostName.Text = socketClient.HostNameString;
            this.TextBox_PortNumber.Text = socketClient.PortNumberString;
        }

        private void GetUiState()
        {
            socketClient.HostNameString = this.TextBox_HostName.Text;
            socketClient.PortNumberString = this.TextBox_PortNumber.Text;
        }

        // Message event handler sent by SocketClient object
        private void ReceivedMessage(object sender, string message)
        {
            clientListBox.Items.Add(message);
        }

        private async void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            try
            {
                clientListBox.Items.Add(string.Format("{0}", "Start."));

                socketClient.SocketClientMessage += ReceivedMessage;    // set a message delegate

                await socketClient.Connect();

                await socketClient.Send("Hello World");

                //socketClient.Receive();

                //socketClient.Disonnect();

                clientListBox.Items.Add(string.Format("{0}", "Completed."));
            }
            catch (Exception ex)
            {
                socketClient.Disonnect();
                clientListBox.Items.Add(string.Format("{0}", ex.Message));
            }

        }

        void App_Suspending(Object sender,Windows.ApplicationModel.SuspendingEventArgs e)
        {
            GetUiState();

            // Write state
            //           XmlSerialize(configfile, socketClient);
            StoreSettings();
        }


        //private async void Pbtn_Test_Click(object sender, RoutedEventArgs e)
        //{
        //    clientListBox.Items.Add(string.Format("{0}", "Start Test"));
        //    try
        //    {
        //        await Test();
        //        //if (ret.Result)
        //        //    msg = "true";
        //        //else
        //        //    msg = "false";
        //        clientListBox.Items.Add(string.Format("{0}", "call Test() completed."));
        //    }
        //    catch (Exception ex)
        //    {
        //        clientListBox.Items.Add(string.Format("{0}", ex.Message));
        //    }
        //}

        //private async Task Test()
        //{
        //    try
        //    {
        //        await Task.Run(() => { throw (new Exception("Exception in Test().")); }).ConfigureAwait(false);
        //    }

        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        #region Store/Restore local data
        private void StoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["HostNameString"] = socketClient.HostNameString;
            container.Values["PortNumberString"] = socketClient.PortNumberString;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("HostNameString"))
                socketClient.HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("PortNumberString"))
                socketClient.PortNumberString = container.Values["PortNumberString"].ToString();
        }
        #endregion

        //#region Serializers
        //private void XmlSerialize(string fileName, object obj)
        //{
        //    try
        //    {
        //        System.Xml.Serialization.XmlSerializer serializer =
        //             new System.Xml.Serialization.XmlSerializer(typeof(SocketClient));
        //        StreamWriter sw = new StreamWriter(
        //            (System.IO.Stream)File.OpenWrite(fileName), System.Text.Encoding.UTF8, 512);

        //        serializer.Serialize(sw, obj);
        //        sw.Dispose();
        //    }
        //    catch(Exception ex)
        //    {
        //        throw;
        //    }
        //}
        //private object XmlDeserialize(string fileName)
        //{
        //    SocketClient obj;

        //    System.Xml.Serialization.XmlSerializer serializer =
        //        new System.Xml.Serialization.XmlSerializer(typeof(SocketClient));
        //    StreamReader sr = new StreamReader(
        //        (System.IO.Stream)File.OpenRead(fileName), System.Text.Encoding.UTF8);
        //    obj = (SocketClient)serializer.Deserialize(sr);
        //    sr.Dispose();

        //    return obj;
        //}
        //#endregion
    }
}
