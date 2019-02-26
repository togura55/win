﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.Storage;
using Windows.ApplicationModel.Resources;
using Windows.UI.ViewManagement;

namespace WillDevicesSampleApp
{
    public sealed partial class MainPage : Page
    {
        CancellationTokenSource m_cts = new CancellationTokenSource();

        //        static Publishers publisher;
        //string HostNameString = "192.168.0.7";
        //string PortNumberString = "1337";
        //        static bool fStart = true;
        ResourceLoader resourceLoader = null;

        public MainPage()
        {
            this.InitializeComponent();

            //            publisher = new Publisher();  // single instance
            //            publisher.PublisherMessage += ReceivedMessage; // set the message delegate

            AppObjects.Instance.Publisher = new Publishers();
            AppObjects.Instance.Publisher.PublisherMessage += ReceivedMessage; // set the message delegate         publisher = AppObjects.Instance.Publisher;

            AppObjects.Instance.WacomDevice = new WacomDevices();     // stored for using this app 
            AppObjects.Instance.WacomDevice.WacomDevicesMessage += ReceivedMessage; // set the message delegate

            AppObjects.Instance.SocketService = new SocketServices();
            AppObjects.Instance.SocketService.SocketMessage += ReceivedMessage; // 

            AppObjects.Instance.RemoteController = new RemoteControllers();
            AppObjects.Instance.RemoteController.RCMessage += ReceivedMessage; // 
            AppObjects.Instance.RemoteController.UpdateUi += ReceivedUpdateUi;
            AppObjects.Instance.RemoteController.PublisherControl += ReceivedPublisherControl;

            RestoreSettings();

            resourceLoader = ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");

            var versionInfo = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = string.Format(
                               "{0}.{1}.{2}.{3}",
                               versionInfo.Major, versionInfo.Minor,
                               versionInfo.Build, versionInfo.Revision);
            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = version;

            SetUiState();

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);


            // Background Task Registration
            AppObjects.Instance.RemoteController.RegisterBackgroundTask();

            // Start Remote Controller services
            AppObjects.Instance.RemoteController.StartListen();
        }

        private void GetUiState()
        {
            AppObjects.Instance.Publisher.HostNameString = this.TextBox_HostName.Text;
            AppObjects.Instance.Publisher.PortNumberString = this.TextBox_PortNumber.Text;
        }

        private void SetUiState()
        {
            Publishers pub = AppObjects.Instance.Publisher;

            this.TextBox_HostName.Text = pub.HostNameString;
            this.TextBox_PortNumber.Text = pub.PortNumberString;

            // swich UI correspond to the current state of Publisher
            if (pub.CurrentState == pub.STATE_NEUTRAL)
            {
                this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");
                this.Pbtn_Discard.Visibility = Visibility.Collapsed;    // hide
            }
            else if (pub.CurrentState == pub.STATE_ACTIVE)
            {
                this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Stop");
                this.Pbtn_Discard.Content = resourceLoader.GetString("IDC_Disconnect");
                this.Pbtn_Discard.Visibility = Visibility.Visible;    // show
            }
            else if (pub.CurrentState == pub.STATE_IDLE)
            {
                this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");
                this.Pbtn_Discard.Content = resourceLoader.GetString("IDC_Disconnect");
                this.Pbtn_Discard.Visibility = Visibility.Visible;    // show
            }
        }

        #region Event Handlers of MainPage
        private void ReceivedUpdateUi(object sender, string message)
        {
            SetUiState();
        }

        private void ReceivedPublisherControl(object sender, string message)
        {
            RunPublisher();  // Do toggle

            SetUiState();
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

        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs s)
        {
            GetUiState();
            StoreSettings();

            AppObjects.Instance.WacomDevice.WacomDevicesMessage -= ReceivedMessage;
            AppObjects.Instance.SocketService.SocketMessage -= ReceivedMessage;

            AppObjects.Instance.Publisher.Stop();

            AppObjects.Instance.Publisher.PublisherMessage -= ReceivedMessage;
            AppObjects.Instance.RemoteController.PublisherControl -= ReceivedPublisherControl;
            AppObjects.Instance.RemoteController.UpdateUi -= ReceivedUpdateUi;
        }

        private void RunPublisher()
        {
            Publishers pub = AppObjects.Instance.Publisher;

            if (pub.CurrentState == pub.STATE_NEUTRAL ||
                pub.CurrentState == pub.STATE_IDLE)
            {
                pub.InitializationCompletedNotification += PublisherInitialization_Completed;
                pub.Start();
            }
            else
            {
                pub.Stop();
                pub.InitializationCompletedNotification -= PublisherInitialization_Completed;
            }
        }

        private void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            try
            {
                RunPublisher();

                SetUiState();
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("Pbtn_Exec_Click: {0}", ex.Message));
            }
        }
        #endregion

        #region Delegate Completion Handlers
        private void PublisherInitialization_Completed(object sender, bool result)
        {
            try
            {
                if (result)
                {
                    clientListBox.Items.Add("PublisherInitialization_Completed: Go to StartRealTimeInk.");
                    AppObjects.Instance.WacomDevice.StartRealTimeInk();
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
            container.Values["HostNameString"] = AppObjects.Instance.Publisher.HostNameString;
            container.Values["PortNumberString"] = AppObjects.Instance.Publisher.PortNumberString;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("HostNameString"))
                AppObjects.Instance.Publisher.HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("PortNumberString"))
                AppObjects.Instance.Publisher.PortNumberString = container.Values["PortNumberString"].ToString();
        }
        #endregion
    }
}
