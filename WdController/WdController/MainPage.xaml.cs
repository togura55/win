using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WdController
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ResourceLoader resource = null;
        string CommandState = CMD_NEUTRAL;
        bool DeviceStarted = false;
        string Width = String.Empty;
        string Height = String.Empty;
        string PointSize = String.Empty;
        string DeviceName = String.Empty;
        string ESN = String.Empty;
        string Battery = String.Empty;
        string DeviceType = String.Empty;
        string TransferMode = String.Empty;
        string IpAddress = String.Empty;
        string PortNumberBase = String.Empty;
        string DeviceState = String.Empty;
        string DeviceVersionNumber = String.Empty;

        string BleServiceName = string.Empty;
        string BleDeviceName = string.Empty;

        public MainPage()
        {
            this.InitializeComponent();

            DeviceState = "false";

            resource = ResourceLoader.GetForCurrentView();
            Pbtn_Start.Content = DeviceStarted ? resource.GetString("IDC_Stop") : resource.GetString("IDC_Start");
            Pbtn_Connect.Content = resource.GetString("IDC_Connect");
            Pbtn_RequestAccess.Content = resource.GetString("IDC_RequestAccess");
            Pbtn_SetConfig.Content = resource.GetString("IDC_SetConfig");
            Pbtn_GetConfig.Content = resource.GetString("IDC_GetConfig");
            Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStart");
            Pbtn_GetVersion.Content = resource.GetString("IDC_GetVersion");
            TextBlock_PublisherDeviceName.Text = resource.GetString("IDC_PublisherDeviceName");
            TextBlock_IP.Text = resource.GetString("IDC_IP");
            TextBlock_Port.Text = resource.GetString("IDC_Port");

            UpdateUI();
            //            Pbtn_DeviceRestart.Content = resource.GetString("IDC_DeviceRestart");
        }

        private void UpdateUI()
        {
            TextBox_Name.Text = Name;
            TextBox_IP.Text = IpAddress;
            TextBox_Port.Text = PortNumberBase;
            TextBlock_DeviceVersion.Text = DeviceVersionNumber;
            TextBlock_ServiceName.Text = BleServiceName;
            TextBlock_DeviceName.Text = BleDeviceName;

            if (DeviceState == "false")
                Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStart");
            else
                Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceSop");
        }

        public ObservableCollection<RfcommChatDeviceDisplay> ResultCollection
        {
            get;
            private set;
        }

        private DeviceWatcher deviceWatcher = null;
        private StreamSocket chatSocket = null;
        private DataWriter chatWriter = null;
        private RfcommDeviceService chatService = null;
        private BluetoothDevice bluetoothDevice;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //            rootPage = MainPage.Current;
            ResultCollection = new ObservableCollection<RfcommChatDeviceDisplay>();
            DataContext = this;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopWatcher();
        }

        private void StopWatcher()
        {
            if (null != deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status))
                {
                    deviceWatcher.Stop();
                }
                deviceWatcher = null;
            }
        }

        void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // Make sure we clean up resources on suspend.
            Disconnect("App Suspension disconnects");
        }

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

            if (deviceWatcher == null)
            {
                SetDeviceWatcherUI();
                StartUnpairedDeviceWatcher();
            }
            else
            {
                ResetMainUI();
            }
        }

        private void SetDeviceWatcherUI()
        {
            // Disable the button while we do async operations so the user can't Run twice.
            Pbtn_Start.Content = resource.GetString("IDC_Stop");
            ListBox_Messages.Items.Add("Device watcher started");
            resultsListView.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = true;
        }

        private void ResetMainUI()
        {
            Pbtn_Start.Content = resource.GetString("IDC_Start");
            Pbtn_Start.IsEnabled = true;
            Pbtn_Connect.Visibility = Visibility.Visible;
            resultsListView.Visibility = Visibility.Visible;
            resultsListView.IsEnabled = true;

            // Re-set device specific UX
            //            ChatBox.Visibility = Visibility.Collapsed;
            Pbtn_RequestAccess.Visibility = Visibility.Collapsed;
            //            if (ConversationList.Items != null) ConversationList.Items.Clear();
            if (ListBox_Messages.Items != null) ListBox_Messages.Items.Clear();
            StopWatcher();
        }

        private void StartUnpairedDeviceWatcher()
        {
            // Request additional properties
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);

            // Hook up handlers for the watcher events before starting the watcher
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, deviceInfo) =>
            {

                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

            deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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

            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    ListBox_Messages.Items.Add(
                        String.Format("{0} devices found. Enumeration completed. Watching for updates...", ResultCollection.Count));
                });
            });

            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, deviceInfoUpdate) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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

            deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    ResultCollection.Clear();
                });
            });

            deviceWatcher.Start();
        }

        /// <summary>
        /// Invoked once the user has selected the device to connect to.
        /// Once the user has selected the device,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Pbtn_Connect_Click(object sender, RoutedEventArgs e)
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

            // Perform device access checks before trying to get the device.
            // First, we check if consent has been explicitly denied by the user.
            DeviceAccessStatus accessStatus = DeviceAccessInformation.CreateFromId(deviceInfoDisp.Id).CurrentStatus;
            if (accessStatus == DeviceAccessStatus.DeniedByUser)
            {
                ListBox_Messages.Items.Add("This app does not have access to connect to the remote device (please grant access in Settings > Privacy > Other Devices");
                return;
            }
            // If not, try to get the Bluetooth device
            try
            {
                bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceInfoDisp.Id);
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(ex.Message);
 //               ResetMainUI();
                return;
            }
            // If we were unable to get a valid Bluetooth device object,
            // it's most likely because the user has specified that all unpaired devices
            // should not be interacted with.
            if (bluetoothDevice == null)
            {
                ListBox_Messages.Items.Add("Bluetooth Device returned null. Access Status = " + accessStatus.ToString());
            }

            // This should return a list of uncached Bluetooth services (so if the server was not active when paired, it will still be detected by this call
            var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(
                RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid), BluetoothCacheMode.Uncached);

            if (rfcommServices.Services.Count > 0)
            {
                chatService = rfcommServices.Services[0];
            }
            else
            {
                ListBox_Messages.Items.Add(
                   "Could not discover the chat service on the remote device");
                //               ResetMainUI();
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Constants.SdpServiceNameAttributeId))
            {
                ListBox_Messages.Items.Add(
                    "The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
//                ResetMainUI();
                return;
            }
            var attributeReader = DataReader.FromBuffer(attributes[Constants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Constants.SdpServiceNameAttributeType)
            {
                ListBox_Messages.Items.Add(
                    "The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
//                ResetMainUI();
                return;
            }
            var serviceNameLength = attributeReader.ReadByte();

            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = UnicodeEncoding.Utf8;

            StopWatcher();

            lock (this)
            {
                chatSocket = new StreamSocket();
            }
            try
            {
                await chatSocket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName);

                BleDeviceName = bluetoothDevice.Name;
                BleServiceName = attributeReader.ReadString(serviceNameLength);
                SetChatUI(BleServiceName, BleDeviceName);
                chatWriter = new DataWriter(chatSocket.OutputStream);

                DataReader chatReader = new DataReader(chatSocket.InputStream);
                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80070490) // ERROR_ELEMENT_NOT_FOUND
            {
                ListBox_Messages.Items.Add("Please verify that you are running the BluetoothRfcommChat server.");
 //               ResetMainUI();
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072740) // WSAEADDRINUSE
            {
                ListBox_Messages.Items.Add("Please verify that there is no other RFCOMM connection to the same device.");
//                ResetMainUI();
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
            // Make sure user has given consent to access device
            DeviceAccessStatus accessStatus = await bluetoothDevice.RequestAccessAsync();

            if (accessStatus != DeviceAccessStatus.Allowed)
            {
                ListBox_Messages.Items.Add(
                    "Access to the device is denied because the application was not granted access");
            }
            else
            {
                ListBox_Messages.Items.Add("Access granted, you are free to pair devices");
            }
        }
        //private void SendButton_Click(object sender, RoutedEventArgs e)
        //{
        //    SendMessage();
        //}

        //public void KeyboardKey_Pressed(object sender, KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        SendMessage();
        //    }
        //}

        /// <summary>
        /// Takes the contents of the MessageTextBox and writes it to the outgoing chatWriter
        /// </summary>
        private async void SendMessage()
        {
            try
            {
                //if (TextBox_Message.Text.Length != 0)
                //{
                //    chatWriter.WriteUInt32((uint)TextBox_Message.Text.Length);
                //    chatWriter.WriteString(TextBox_Message.Text);

                //    //                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                //    ListBox_Messages.Items.Add("Sent: " + TextBox_Message.Text);
                //    TextBox_Message.Text = "";
                //    await chatWriter.StoreAsync();

                //}
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                ListBox_Messages.Items.Add("Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message);
            }
        }

        private async void ReceiveStringLoop(DataReader chatReader)
        {
            try
            {
                uint size = await chatReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    Disconnect("Remote device terminated connection - make sure only one instance of server is running on remote device");
                    return;
                }

                uint stringLength = chatReader.ReadUInt32();
                uint actualStringLength = await chatReader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                string readString = chatReader.ReadString(stringLength);
                //                ConversationList.Items.Add("Received: " + chatReader.ReadString(stringLength));
                ListBox_Messages.Items.Add("Received: " + readString);

                // handle responses sent by BLE server
                ResponseDispatcher(readString);

                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (chatSocket == null)
                    {
                        // Do not print anything here -  the user closed the socket.
                        if ((uint)ex.HResult == 0x80072745)
                            ListBox_Messages.Items.Add("Disconnect triggered by remote device");
                        else if ((uint)ex.HResult == 0x800703E3)
                            ListBox_Messages.Items.Add("The I/O operation has been aborted because of either a thread exit or an application request.");
                    }
                    else
                    {
                        Disconnect("ReceiveStringLoop: Exception: Read stream failed with error: " + ex.Message);
                    }
                }
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect("Disconnected");
        }


        /// <summary>
        /// Cleans up the socket and DataWriter and reset the UI
        /// </summary>
        /// <param name="disconnectReason"></param>
        private void Disconnect(string disconnectReason)
        {
            if (chatWriter != null)
            {
                chatWriter.DetachStream();
                chatWriter = null;
            }

            if (chatService != null)
            {
                chatService.Dispose();
                chatService = null;
            }
            lock (this)
            {
                if (chatSocket != null)
                {
                    chatSocket.Dispose();
                    chatSocket = null;
                }
            }

            ListBox_Messages.Items.Add(disconnectReason);
//            ResetMainUI();
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
            //            ChatBox.Visibility = Visibility.Visible;
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
        //        }

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

        private void Pbtn_SetConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string command = CMD_SETCONFIG;
                string separater = ",";

                if (!String.IsNullOrEmpty(TextBox_Name.Text))
                {
                    command += separater + TextBox_Name.Text;
                }
                else
                {
                    throw new Exception(string.Format("SetConfig: Exception: DeviceName text box is empty."));
                }

                if (!String.IsNullOrEmpty(TextBox_IP.Text))
                {
                    command += separater + TextBox_IP.Text;
                }
                else
                {
                    throw new Exception(string.Format("SetConfig: Exception: IP Address text box is empty."));
                }

                if (!String.IsNullOrEmpty(TextBox_Port.Text))
                {
                    command += separater + TextBox_Port.Text;
                }
                else
                {
                    throw new Exception(string.Format("SetConfig: Exception: Port Number text box is empty."));
                }

                CommandState = CMD_SETCONFIG;
                SendCommand(command);
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(String.Format("Pbtn_SetConfig_Click: Exception: {0}", ex.Message));
            }
        }

        private void Pbtn_GetConfig_Click(object sender, RoutedEventArgs e)
        {
            CommandState = CMD_GETCONFIG;
            SendCommand(CMD_GETCONFIG);
        }

        private void Pbtn_DeviceStart_Click(object sender, RoutedEventArgs e)
        {
            CommandState = CMD_START;
            SendCommand(CMD_START);

            DeviceStarted = !DeviceStarted;
            Pbtn_DeviceStart.Content = DeviceStarted ? resource.GetString("IDC_Stop") : resource.GetString("IDC_Start");
        }

        private void Pbtn_GetVersion_Click(object sender, RoutedEventArgs e)
        {
            CommandState = CMD_GETVERSION;
            SendCommand(CMD_GETVERSION);
        }

        private const string CMD_START = "start";
        private const string CMD_STOP = "stop";
        private const string CMD_RESTART = "restart";
        private const string CMD_GETCONFIG = "getconfig";
        private const string CMD_SETCONFIG = "setconfig";  // setconfig,aaa,bbb,ccc
        private const string CMD_GETVERSION = "getversion";
        private const string CMD_NEUTRAL = "neutral";
        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";

        /// <summary>
        /// 
        /// </summary>
        private async void SendCommand(string command)
        {
            try
            {
                if (command.Length != 0)
                {
                    chatWriter.WriteUInt32((uint)command.Length);
                    chatWriter.WriteString(command);

                    //                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                    ListBox_Messages.Items.Add("Sent: " + command);
                    //                    TextBox_Message.Text = "";
                    await chatWriter.StoreAsync();

                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                ListBox_Messages.Items.Add("Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message);
            }
        }

        private void ResponseDispatcher(string message)
        {
            try
            {
                string res = string.Empty;

                switch (CommandState)
                {
                    case CMD_START:

                        if (message.Equals((string)RES_ACK))
                        {
                            // ok
                            Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStop");
                        }
                        else
                        {
                            // error
                        }
                        break;

                    case CMD_STOP:

                        if (message.Equals((string)RES_ACK))
                        {
                            // ok
                            Pbtn_DeviceStart.Content = resource.GetString("IDC_DeviceStart");
                        }
                        else
                        {
                            // error
                        }
                        break;

                    case CMD_GETCONFIG:
                        char sp = ','; // separater
                        string[] arr = message.Split(sp);
                        var list = new List<string>();
                        list.AddRange(arr);

                        // decode
                        if (list.Count < 11)
                        {
                            // error, resend?
                            throw new Exception("GetConfig returns the smaller number of parameters.");
                        }
                        else
                        {
                            int i = -1;

                            Width = list[++i];
                            Height = list[++i];
                            PointSize = list[++i];
                            Name = list[++i];
                            ESN = list[++i];
                            Battery = list[++i];
                            DeviceType = list[++i];
                            TransferMode = list[++i];
                            IpAddress = list[++i];
                            PortNumberBase = list[++i];
                            DeviceState = list[++i];

                            UpdateUI();
                        }

                        break;

                    case CMD_SETCONFIG:

                        if (message.Equals((string)RES_ACK))
                        {
                            // ok
                        }
                        else
                        {
                            // error
                        }
                        break;

                    case CMD_GETVERSION:
                        DeviceVersionNumber = message;
                        UpdateUI();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(string.Format("CommandsDispatcher: Exception: {0}", ex.Message));
            }
        }
    }
}
