﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WdController
{
    public sealed partial class MainPage : Page
    {
        ResourceLoader resource = null;
        WdControllers wdController = null;


        public class RfcommChatDeviceDisplay : INotifyPropertyChanged
        {
            private DeviceInformation deviceInfo;

            public RfcommChatDeviceDisplay(DeviceInformation deviceInfoIn)
            {
                deviceInfo = deviceInfoIn;
                UpdateGlyphBitmapImage();
            }

            public DeviceInformation DeviceInformation
            {
                get
                {
                    return deviceInfo;
                }

                private set
                {
                    deviceInfo = value;
                }
            }

            public string Id
            {
                get
                {
                    return deviceInfo.Id;
                }
            }

            public string Name
            {
                get
                {
                    return deviceInfo.Name;
                }
            }

            public BitmapImage GlyphBitmapImage
            {
                get;
                private set;
            }

            public void Update(DeviceInformationUpdate deviceInfoUpdate)
            {
                deviceInfo.Update(deviceInfoUpdate);
                UpdateGlyphBitmapImage();
            }

            private async void UpdateGlyphBitmapImage()
            {
                DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
                BitmapImage glyphBitmapImage = new BitmapImage();
                await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
                GlyphBitmapImage = glyphBitmapImage;
                OnPropertyChanged("GlyphBitmapImage");
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public ObservableCollection<RfcommChatDeviceDisplay> ResultCollection
        {
            get;
            private set;
        }

        public MainPage()
        {
            this.InitializeComponent();

            wdController = new WdControllers();
            wdController.WdControllerMessage += ReceivedMessage; // set the message delegate
            wdController.WdControllerAction += ReceivedAction; // set the action message delegate

            wdController.rfComm.RfCommMessage += ReceivedMessage;
            wdController.rfComm.RfCommAction += ReceivedAction; // set the action message delegate

            // UI initialization
            resource = ResourceLoader.GetForCurrentView();
            Pbtn_Start.Content = wdController.DeviceStarted ? resource.GetString("IDC_Stop") : resource.GetString("IDC_Start");
            Pbtn_Connect.Content = resource.GetString("IDC_Connect");
            Pbtn_RequestAccess.Content = resource.GetString("IDC_RequestAccess");
            Pbtn_SetConfig.Content = resource.GetString("IDC_SetConfig");
            Pbtn_GetConfig.Content = resource.GetString("IDC_GetConfig");
            Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStart");
            Pbtn_GetVersion.Content = resource.GetString("IDC_GetVersion");
            //            Pbtn_DeviceRestart.Content = resource.GetString("IDC_DeviceRestart");
            TextBlock_PublisherDeviceName.Text = resource.GetString("IDC_PublisherDeviceName");
            TextBlock_IP.Text = resource.GetString("IDC_IP");
            TextBlock_Port.Text = resource.GetString("IDC_Port");

            UpdateUI();

            var versionInfo = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = string.Format(
                               "{0}.{1}.{2}.{3}",
                               versionInfo.Major, versionInfo.Minor,
                               versionInfo.Build, versionInfo.Revision);
            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = version;
        }

        private void ReceivedMessage(object sender, string message)
        {
            ListBox_Messages.Items.Add(message);
        }

        private void ReceivedAction(object sender, string message)
        {
            if (message == "UpdateUI")
                UpdateUI();
            else if (message == "StopWatcher")
                wdController.StopWatcher();
            else if (message == "SetChatUI")
                UpdateUI();
            else
                ListBox_Messages.Items.Add(string.Format("ReceivedAction: No match {0}", message));
        }

        #region System UI operations
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //            rootPage = MainPage.Current;
            ResultCollection = new ObservableCollection<RfcommChatDeviceDisplay>();
            DataContext = this;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            wdController.StopWatcher();
        }

        void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // Make sure we clean up resources on suspend.
            ListBox_Messages.Items.Add("App Suspension disconnects.");
            wdController.Disconnect();

            wdController.WdControllerMessage -= ReceivedMessage;
            wdController.rfComm.RfCommMessage -= ReceivedMessage;
        }

        private void UpdateUI()
        {
            TextBox_Name.Text = Name;
            TextBox_IP.Text = wdController.IpAddress;
            TextBox_Port.Text = wdController.PortNumberBase;
            TextBlock_DeviceVersion.Text = wdController.DeviceVersionNumber;
            TextBlock_ServiceName.Text = wdController.rfComm.BleServiceName;
            TextBlock_DeviceName.Text = wdController.rfComm.BleDeviceName;

            if (wdController.DeviceState == "false" || wdController.DeviceState == "False")
                Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStart");
            else
                Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStop");
        }

        private void SetDeviceWatcherUI()
        {
            // Disable the button while we do async operations so the user can't Run twice.
            //            Pbtn_Start.Content = resource.GetString("IDC_Stop");
            if (wdController != null)
            {
                Pbtn_Start.Content = wdController.DeviceStarted ? resource.GetString("IDC_Stop") : resource.GetString("IDC_Start");

                string message = wdController.DeviceStarted ? "Device watcher started" : "Device watcher stopped";
                ListBox_Messages.Items.Add(message);

                resultsListView.Visibility = wdController.DeviceStarted ? Visibility.Visible : Visibility.Collapsed;
                resultsListView.IsEnabled = wdController.DeviceStarted ? true : false;
            }
        }

        private void ResetMainUI()
        {
            //            Pbtn_Start.Content = resource.GetString("IDC_Start");
            //            Pbtn_Start.IsEnabled = true;

            //            Pbtn_Connect.Visibility = Visibility.Visible;
            //            resultsListView.Visibility = Visibility.Visible;
            //            resultsListView.IsEnabled = true;

            // Re-set device specific UX
            //            ChatBox.Visibility = Visibility.Collapsed;
            //           Pbtn_RequestAccess.Visibility = Visibility.Collapsed;
            //            if (ConversationList.Items != null) ConversationList.Items.Clear();
            //           if (ListBox_Messages.Items != null) ListBox_Messages.Items.Clear();

            wdController.StopWatcher();
        }

        private void SetChatUI(string serviceName, string deviceName)
        {
            ListBox_Messages.Items.Add("Connected");
            TextBlock_ServiceName.Text = "Service Name: " + serviceName;
            TextBlock_DeviceName.Text = "Connected to: " + deviceName;
            Pbtn_Start.IsEnabled = false;
            Pbtn_Connect.Visibility = Visibility.Collapsed;
            Pbtn_RequestAccess.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = false;
            resultsListView.Visibility = Visibility.Collapsed;
        }

        private void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePairingButtons();
        }

        private void UpdatePairingButtons()
        {
            RfcommChatDeviceDisplay deviceDisp = (RfcommChatDeviceDisplay)resultsListView.SelectedItem;

            if (null != deviceDisp)
            {
                Pbtn_Connect.IsEnabled = true;
            }
            else
            {
                Pbtn_Connect.IsEnabled = false;
            }
        }

        public void StartUnpairedDeviceWatcher()
        {
            // Request additional properties
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            wdController.deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);

            // Hook up handlers for the watcher events before starting the watcher
            wdController.deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {

                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Make sure device name isn't blank
                    if (deviceInfo.Name != "")
                    {
                        ResultCollection.Add(new RfcommChatDeviceDisplay(deviceInfo));
                        ListBox_Messages.Items.Add(
                            String.Format("{0} devices found.", ResultCollection.Count));
                    }

                });
            });

            wdController.deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    foreach (RfcommChatDeviceDisplay rfcommInfoDisp in ResultCollection)
                    {
                        if (rfcommInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            rfcommInfoDisp.Update(deviceInfoUpdate);
                            break;
                        }
                    }
                });
            });

            wdController.deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {

                    ListBox_Messages.Items.Add(
                    String.Format("{0} devices found. Enumeration completed. Watching for updates...",
                    ResultCollection.Count));
                });
            });

            wdController.deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    // Find the corresponding DeviceInformation in the collection and remove it
                    foreach (RfcommChatDeviceDisplay rfcommInfoDisp in ResultCollection)
                    {
                        if (rfcommInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            ResultCollection.Remove(rfcommInfoDisp);
                            break;
                        }
                    }

                    ListBox_Messages.Items.Add(
                        String.Format("{0} devices found.", ResultCollection.Count));
                });
            });

            wdController.deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    ResultCollection.Clear();
                });
            });

            wdController.deviceWatcher.Start();
        }

        #endregion

        #region UI handlers
        /// <summary>
        /// When the user presses the run button, query for all nearby unpaired devices
        /// Note that in this case, the other device must be running the Rfcomm Chat Server before being paired.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        //        private void RunButton_Click(object sender, RoutedEventArgs e)
        private void Pbtn_Start_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Messages.Items.Add("Start Initialization");
            //            Initialize();

            if (wdController.deviceWatcher == null)
            {
                StartUnpairedDeviceWatcher();
//                SetDeviceWatcherUI();
            }
            else
            {
                ResetMainUI();
            }
        }

        //private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        //{
        //    rdController.Disconnect();
        //}

        /// <summary>
        /// Invoked once the user has selected the device to connect to.
        /// Once the user has selected the device,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Pbtn_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Make sure user has selected a device first
                if (resultsListView.SelectedItem != null)
                {
                    ListBox_Messages.Items.Add("Connecting to remote device. Please wait...");
                }
                else
                {
                    ListBox_Messages.Items.Add("Please select an item to connect to");
                    return;
                }

                RfcommChatDeviceDisplay deviceInfoDisp = resultsListView.SelectedItem as RfcommChatDeviceDisplay;

                await wdController.Connect(deviceInfoDisp.Id);
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(string.Format("Pbtn_Connect_Click: Exception: {0}", ex.Message));
            }
        }

        /// <summary>
        ///  If you believe the Bluetooth device will eventually be paired with Windows,
        ///  you might want to pre-emptively get consent to access the device.
        ///  An explicit call to RequestAccessAsync() prompts the user for consent.
        ///  If this is not done, a device that's working before being paired,
        ///  will no longer work after being paired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RequestAccessButton_Click(object sender, RoutedEventArgs e)
        {
            await wdController.RequestAccess();
        }

        private async void Pbtn_SetConfig_Click(object sender, RoutedEventArgs e)
        {
            string parameters = string.Empty;
            string separater = ",";

            parameters += separater + TextBox_Name.Text;
            //if (!String.IsNullOrEmpty(TextBox_Name.Text))
            //{
            //    command += separater + TextBox_Name.Text;
            //}
            //else
            //{
            //    throw new Exception(string.Format("SetConfig: Exception: DeviceName text box is empty."));
            //}

            if (!String.IsNullOrEmpty(TextBox_IP.Text))
            {
                parameters += separater + TextBox_IP.Text;
            }
            else
            {
                throw new Exception(string.Format("SetConfig: Exception: IP Address text box is empty."));
            }

            if (!String.IsNullOrEmpty(TextBox_Port.Text))
            {
                parameters += separater + TextBox_Port.Text;
            }
            else
            {
                throw new Exception(string.Format("SetConfig: Exception: Port Number text box is empty."));
            }

            await wdController.SetConfig(parameters);
        }

        private async void Pbtn_GetConfig_Click(object sender, RoutedEventArgs e)
        {
            await wdController.GetConfig();
        }

        private async void Pbtn_DeviceStart_Click(object sender, RoutedEventArgs e)
        {
            //CommandState = DeviceStarted? CMD_STOP: CMD_START;  // toggle command

            //SendCommand(CommandState);
            if (wdController.DeviceStarted)
                await wdController.DeviceStop();
            else
                await wdController.DeviceStart();


            wdController.DeviceStarted = !wdController.DeviceStarted;  // toggle state, ToDo: should be moved into the completion delegate.
            Pbtn_DeviceStart.Content = 
                wdController.DeviceStarted ? resource.GetString("IDC_Stop") : resource.GetString("IDC_Start");
        }

        private async void Pbtn_GetVersion_Click(object sender, RoutedEventArgs e)
        {
            await wdController.GetVersion();
        }
        #endregion
    }
}
