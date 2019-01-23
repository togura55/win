using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.Storage;
using Windows.ApplicationModel.Resources;

namespace WillDevicesSampleApp
{    
    public sealed partial class MainPage : Page
    {
        CancellationTokenSource m_cts = new CancellationTokenSource();

        static Publisher publisher;
        string HostNameString = "192.168.0.7";
        string PortNumberString = "1337";
        static bool fStart = true;
        ResourceLoader resourceLoader = null;

        public MainPage()
        {
            this.InitializeComponent();

            publisher = new Publisher();  // single instance
            publisher.PublisherMessage += ReceivedMessage; // set the message delegate

            AppObjects.Instance.WacomDevice = new WacomDevices();     // stored for using this app 
            AppObjects.Instance.WacomDevice.WacomDevicesMessage += ReceivedMessage; // set the message delegate

            AppObjects.Instance.SocketService = new SocketServices();
            AppObjects.Instance.SocketService.SocketMessage += ReceivedMessage; // 

            AppObjects.Instance.RemoteController = new RemoteControllers();
            AppObjects.Instance.RemoteController.RCMessage += ReceivedMessage; // 

            RestoreSettings();

            resourceLoader = ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Exec.Content = resourceLoader.GetString(fStart? "IDC_Exec" : "IDC_Stop");

            this.TextBox_HostName.Text = HostNameString;
            this.TextBox_PortNumber.Text = PortNumberString;

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            // Start Remote Controller services
            AppObjects.Instance.RemoteController.StartListen();
        }

        private void GetUiState()
        {
            HostNameString = this.TextBox_HostName.Text;
            PortNumberString = this.TextBox_PortNumber.Text;
        }

        #region Event Handlers of MainPage
        /// <summary>
        /// Message event handler sent by instance object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void ReceivedMessage(object sender, string message)
        {
            clientListBox.Items.Add(message);
        }

        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs s)
        {
            GetUiState();
            StoreSettings();

            AppObjects.Instance.WacomDevice.WacomDevicesMessage -= ReceivedMessage;
            AppObjects.Instance.SocketService.SocketMessage -= ReceivedMessage;

            publisher.Stop();
        }

        private void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();
            
            try
            {
                if (fStart)
                {
                    publisher.InitializationCompletedNotification += PublisherInitialization_Completed;
                    publisher.Start(HostNameString, PortNumberString);
                }
                else
                {
                    publisher.Stop();
                    publisher.InitializationCompletedNotification -= PublisherInitialization_Completed;
                }
                fStart = fStart ? false : true; // toggle if success
                Pbtn_Exec.Content = resourceLoader.GetString(fStart ? "IDC_Exec" : "IDC_Stop");
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("Pbtn_Exec_Click: {0}", ex.Message));
            }
        }
        #endregion

        #region Delegate Completion Handlers
        private async void PublisherInitialization_Completed(object sender, bool result)
        {
            try
            {
                if (result)
                {
                    clientListBox.Items.Add("PublisherInitialization_Completed: Go to StartRealTimeInk.");
                    await AppObjects.Instance.WacomDevice.StartRealTimeInk();
                }
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("PublisherInitialization_Completed: Exception: {0}", ex.Message));
            }
        }
        #endregion

        #region Store/Restore local data
        private void StoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["HostNameString"] = HostNameString;
            container.Values["PortNumberString"] = PortNumberString;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("HostNameString"))
                HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("PortNumberString"))
                PortNumberString = container.Values["PortNumberString"].ToString();
        }
        #endregion
    }
}
