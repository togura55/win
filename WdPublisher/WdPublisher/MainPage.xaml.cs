using System;
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
//        CancellationTokenSource m_cts = new CancellationTokenSource();

        ResourceLoader resourceLoader;

        public MainPage()
        {
            this.InitializeComponent();

            AppObjects.Instance.Publisher = new Publisher();
            AppObjects.Instance.Publisher.PublisherMessage += ReceivedMessage; // set the message delegate         publisher = AppObjects.Instance.Publisher;
            AppObjects.Instance.Publisher.UpdateUi += ReceivedUpdateUi;
            AppObjects.Instance.Publisher.PublisherControl += ReceivedPublisherControl;

            // For debug
            if (!AppObjects.Instance.Publisher.Debug)
            // End for debug
            { 
                AppObjects.Instance.RemoteController = new RemoteControllers();
            AppObjects.Instance.RemoteController.RCMessage += ReceivedMessage; // 
            AppObjects.Instance.RemoteController.UpdateUi += ReceivedUpdateUi;
            AppObjects.Instance.RemoteController.PublisherControl += ReceivedPublisherControl;
            }

            RestoreSettings();  // read stored setting values

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

            // For debug
            if (!AppObjects.Instance.Publisher.Debug)
                // End for debug
                StartRemoteControllerTask();
        }

        private void StartRemoteControllerTask()
        {
            // Background Task Registration
            AppObjects.Instance.RemoteController.RegisterBackgroundTask();

            // Start Remote Controller services
            AppObjects.Instance.RemoteController.StartListen();
        }

        private void GetUiState()
        {
            AppObjects.Instance.Publisher.HostNameString = this.TextBox_HostName.Text;
            AppObjects.Instance.Publisher.PortNumberString = this.TextBox_PortNumber.Text;
            AppObjects.Instance.Publisher.Debug = (bool)this.CB_Debug.IsChecked;
        }

        private void SetUiState()
        {
            Publisher pub = AppObjects.Instance.Publisher;

            this.TextBox_HostName.Text = pub.HostNameString;
            this.TextBox_PortNumber.Text = pub.PortNumberString;

            this.CB_Debug.Content = resourceLoader.GetString("IDC_Debug");
            this.CB_Debug.IsChecked = pub.Debug;

            // swich UI correspond to the current state of Publisher
            if (pub.CurrentState == pub.STATE_NEUTRAL)
            {
                this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");
                this.Pbtn_Resume.Visibility = Visibility.Collapsed;    // hide
            }
            else if (pub.CurrentState == pub.STATE_ACTIVE)
            {
                this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Stop");
                this.Pbtn_Resume.Content = resourceLoader.GetString("IDC_Suspend");
                this.Pbtn_Resume.Visibility = Visibility.Visible;    // show
            }
            else if (pub.CurrentState == pub.STATE_IDLE)
            {
                this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Stop");
                this.Pbtn_Resume.Content = resourceLoader.GetString("IDC_Resume");
                this.Pbtn_Resume.Visibility = Visibility.Visible;    // show
            }
        }

        #region Event Handlers of MainPage
        private void ReceivedUpdateUi(object sender, string message)
        {
            SetUiState();
        }

        private void ReceivedPublisherControl(object sender, string message)
        {
            switch (message)
            {
                //case "Discard":
                //    StopPublisher();
                //    break;

                case "DeviceStart":
                    RunPublisher();
                    break;

                case "DeviceStop":  // call from RemoteController
                    StopPublisher();
                    break;

                case "DeviceSuspend":
                    SuspendPublisher();
                    break;

                case "DeviceResume":
                    ResumePublisher();
                    break;

                case "RegisterBackgroundWatcher":
                    StartRemoteControllerTask();
                    break;

                    // ToDo: move to Publisher ?
                case "SendStopToBrokerComplete":
                    AppObjects.Instance.Publisher.InitializationCompletedNotification -= PublisherInitialization_Completed;

                    AppObjects.Instance.DataSocketService.SocketMessage -= ReceivedMessage; // 
                    AppObjects.Instance.DataSocketService.Dispose();
                    AppObjects.Instance.DataSocketService = null;

                    AppObjects.Instance.CommandSocketService.SocketMessage -= ReceivedMessage; // 
                    AppObjects.Instance.CommandSocketService.Dispose();
                    AppObjects.Instance.CommandSocketService = null;

                    AppObjects.Instance.WacomDevice.WacomDevicesMessage -= ReceivedMessage; // set the message delegate
                                                                                            //               AppObjects.Instance.WacomDevice.Dispose(); 
                    AppObjects.Instance.WacomDevice = null;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Message event handler sent by instance object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void ReceivedMessage(object sender, string message)
        {
            clientListBox.Items.Add(message);
            clientListBox.ScrollIntoView(clientListBox.Items[clientListBox.Items.Count - 1]);    // scroll to the bottom
        }

        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs s)
        {
            GetUiState();
            StoreSettings();

            if (AppObjects.Instance.WacomDevice != null)
                AppObjects.Instance.WacomDevice.WacomDevicesMessage -= ReceivedMessage;
            if (AppObjects.Instance.CommandSocketService != null)
                AppObjects.Instance.CommandSocketService.SocketMessage -= ReceivedMessage;
            if (AppObjects.Instance.DataSocketService != null)
                AppObjects.Instance.DataSocketService.SocketMessage -= ReceivedMessage;
            if (AppObjects.Instance.Publisher != null)
            {
                AppObjects.Instance.Publisher.Stop();
                AppObjects.Instance.Publisher.PublisherControl -= ReceivedPublisherControl;
                AppObjects.Instance.Publisher.UpdateUi -= ReceivedUpdateUi;
                AppObjects.Instance.Publisher.PublisherMessage -= ReceivedMessage;
            }
            if (AppObjects.Instance.RemoteController != null)
            {
                AppObjects.Instance.RemoteController.PublisherControl -= ReceivedPublisherControl;
                AppObjects.Instance.RemoteController.UpdateUi -= ReceivedUpdateUi;
            }
        }

        private void RunPublisher()
        {
            Publisher pub = AppObjects.Instance.Publisher;

            if (pub.CurrentState == pub.STATE_NEUTRAL ||
                pub.CurrentState == pub.STATE_IDLE)
            {
                AppObjects.Instance.WacomDevice = new WacomDevices();     // stored for using this app 
                AppObjects.Instance.WacomDevice.WacomDevicesMessage += ReceivedMessage; // set the message delegate

                AppObjects.Instance.CommandSocketService = new SocketServices();
                AppObjects.Instance.CommandSocketService.SocketMessage += ReceivedMessage; // 

                AppObjects.Instance.DataSocketService = new SocketServices();
                AppObjects.Instance.DataSocketService.SocketMessage += ReceivedMessage; // 

                pub.InitializationCompletedNotification += PublisherInitialization_Completed;
                pub.Start();
            }
            else
            {
                pub.Stop();
            }
        }

        private void StopPublisher()
        {
            // Stop and dispose all w/o RemoteController
            AppObjects.Instance.Publisher.Stop();
        }

        private void SuspendPublisher()
        {

        }

        private void ResumePublisher()
        {

        }

        private void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            RunPublisher();  // Start/Stop
        }

        private void Pbtn_Suspend_Click(object sender, RoutedEventArgs e)
        {
            SuspendPublisher();
        }

        private void Pbtn_Resume_Click(object sender, RoutedEventArgs e)
        {
            ResumePublisher();
        }
        #endregion

        #region Delegate Completion Handlers
        private async void PublisherInitialization_Completed(object sender, bool result)
        {
            try
            {
                if (result)
                {
                    int msDelay = 1000;
                    ReceivedMessage(this, 
                        string.Format("PublisherInitialization_Completed: Go to StartRealTimeInk after {0}ms delay.", msDelay));
                    await Task.Delay(msDelay);
                    AppObjects.Instance.WacomDevice.StartRealTimeInk();
                }
            }
            catch (Exception ex)
            {
                 ReceivedMessage(this, string.Format("PublisherInitialization_Completed: Exception: {0}", ex.Message));
            }
        }
        #endregion

        #region Store/Restore local data
        private void StoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["HostNameString"] = AppObjects.Instance.Publisher.HostNameString;
            container.Values["PortNumberString"] = AppObjects.Instance.Publisher.PortNumberString;
            container.Values["Debug"] = AppObjects.Instance.Publisher.Debug;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("HostNameString"))
                AppObjects.Instance.Publisher.HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("PortNumberString"))
                AppObjects.Instance.Publisher.PortNumberString = container.Values["PortNumberString"].ToString();
            if (container.Values.ContainsKey("Debug"))   
                AppObjects.Instance.Publisher.Debug = Convert.ToBoolean(container.Values["Debug"].ToString());
        }

        #endregion


    }
}
