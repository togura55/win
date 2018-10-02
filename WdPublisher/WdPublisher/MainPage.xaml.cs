using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Wacom.Devices;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Radios;
using System.Collections.Generic;

using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;  // UIAutomation
using Windows.Storage.Streams;
using System.Collections;
using Windows.Storage;

namespace WillDevicesSampleApp
{    

    public sealed partial class MainPage : Page
    {
        CancellationTokenSource m_cts = new CancellationTokenSource();

        static WacomDevices wacomDevices;
        static WdPublishComm wdPubComm;
        string HostNameString;
        string PortNumberString;

        public MainPage()
        {
            this.InitializeComponent();

            wdPubComm = new WdPublishComm();  // single instance

            wacomDevices = new WacomDevices();
            wacomDevices.WacomDevicesMessage += ReceivedMessage; // set the message delegate

            AppObjects.Instance.SocketClient = new SocketClient();
            AppObjects.Instance.SocketClient.SocketClientMessage += ReceivedMessage; // 

            RestoreSettings();

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");

            this.TextBox_HostName.Text = HostNameString;
            this.TextBox_PortNumber.Text = PortNumberString;

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
        }

        private void GetUiState()
        {
            HostNameString = this.TextBox_HostName.Text;
            PortNumberString = this.TextBox_PortNumber.Text;
        }

        /// <summary>
        /// Message event handler sent by instance object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void ReceivedMessage(object sender, string message)
        {
            clientListBox.Items.Add(message);
        }

        #region Event Handlers of MainPage
        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs s)
        {
            GetUiState();
            StoreSettings();

            wacomDevices.WacomDevicesMessage -= ReceivedMessage;
            AppObjects.Instance.SocketClient.SocketClientMessage -= ReceivedMessage;

            wdPubComm.Stop();
        }

        private void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            try
            {
                // Set task completion delegation 
                wacomDevices.ScanAndConnectCompletedNotification += ScanAndConnect_Completed;
                wdPubComm.InitializationCompletedNotification += WdPubCommInitialization_Completed;
                wacomDevices.StartRealTimeInkCompletedNotification += StartRealTimeInk_Completed;

                wacomDevices.StartScanAndConnect();
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("Pbtn_Exec_Click: {0}", ex.Message));
            }
        }
        #endregion

        #region Delegate Completion Handlers
        private async void ScanAndConnect_Completed(object sender, bool result)
        {
            if (AppObjects.Instance.Device != null)
            {
                clientListBox.Items.Add("ScanAndConnect_Completed: Go Socket initialization");
                wdPubComm.Initialize(HostNameString, PortNumberString);
            }
            else
            {
                clientListBox.Items.Add("ScanAndConnect_Completed: Device is null");
            }
        }

        private async void WdPubCommInitialization_Completed(object sender, bool result)
        {
            if (result)
            {
                clientListBox.Items.Add("WdPubCommInitialization_Completed: Go to StartRealTimeInk.");
                await wacomDevices.StartRealTimeInk();
            }
        }

        private async void StartRealTimeInk_Completed(object sender, bool result)
        {
            if (result)  // socket was established
            {
                clientListBox.Items.Add("StartRealTimeInk_Completed: All pre-process were done.");
//                await wacomDevices.StartRealTimeInk();
            }
            else
            {
                clientListBox.Items.Add("StartRealTime_Completed: got false.");
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
