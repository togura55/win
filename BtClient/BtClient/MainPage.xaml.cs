using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.ApplicationModel.Resources;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace BtClient
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ResourceLoader resource = null;

        /// <summary>
        /// Class containing Attributes and UUIDs that will populate the SDP record.
        /// </summary>
        class Constants
        {
            // The Chat Server's custom service Uuid: 34B1CF4D-1069-4AD6-89B6-E161D79BE4D8
            public static readonly Guid RfcommChatServiceUuid = Guid.Parse("34B1CF4D-1069-4AD6-89B6-E161D79BE4D9");

            // The Id of the Service Name SDP attribute
            public const UInt16 SdpServiceNameAttributeId = 0x100;

            // The SDP Type of the Service Name SDP attribute.
            // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
            //    -  the Attribute Type size in the least significant 3 bits,
            //    -  the SDP Attribute Type value in the most significant 5 bits.
            public const byte SdpServiceNameAttributeType = (4 << 3) | 5;

            // The value of the Service Name SDP attribute
            public const string SdpServiceName = "TestTest";
        }

        public MainPage()
        {
            this.InitializeComponent();

            App.Current.Suspending += App_Suspending;

            resource = ResourceLoader.GetForCurrentView();
            Pbtn_Start.Content = resource.GetString("IDC_Start");
            Pbtn_Connect.Content = resource.GetString("IDC_Connect");
            Pbtn_RequestAccess.Content = resource.GetString("IDC_RequestAccess");
        }

        //       public sealed partial class Scenario1_ChatClient : Page
        //       {
        // A pointer back to the main page is required to display status messages.
        //       private MainPage rootPage = MainPage.Current;

        // Used to display list of available devices to chat with
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
                ResetMainUI();
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
                ResetMainUI();
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Constants.SdpServiceNameAttributeId))
            {
                ListBox_Messages.Items.Add(
                    "The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUI();
                return;
            }
            var attributeReader = DataReader.FromBuffer(attributes[Constants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Constants.SdpServiceNameAttributeType)
            {
                ListBox_Messages.Items.Add(
                    "The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUI();
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

                SetChatUI(attributeReader.ReadString(serviceNameLength), bluetoothDevice.Name);
                chatWriter = new DataWriter(chatSocket.OutputStream);

                DataReader chatReader = new DataReader(chatSocket.InputStream);
                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80070490) // ERROR_ELEMENT_NOT_FOUND
            {
                ListBox_Messages.Items.Add("Please verify that you are running the BluetoothRfcommChat server.");
                ResetMainUI();
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072740) // WSAEADDRINUSE
            {
                ListBox_Messages.Items.Add("Please verify that there is no other RFCOMM connection to the same device.");
                ResetMainUI();
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
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        public void KeyboardKey_Pressed(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
            }
        }

        /// <summary>
        /// Takes the contents of the MessageTextBox and writes it to the outgoing chatWriter
        /// </summary>
        private async void SendMessage()
        {
            try
            {
                if (TextBox_Message.Text.Length != 0)
                {
                    chatWriter.WriteUInt32((uint)TextBox_Message.Text.Length);
                    chatWriter.WriteString(TextBox_Message.Text);

//                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                    ListBox_Messages.Items.Add("Sent: " + TextBox_Message.Text);
                    TextBox_Message.Text = "";
                    await chatWriter.StoreAsync();

                }
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

//                ConversationList.Items.Add("Received: " + chatReader.ReadString(stringLength));
                ListBox_Messages.Items.Add("Received: " + chatReader.ReadString(stringLength));

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
                        Disconnect("Read stream failed with error: " + ex.Message);
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
            ResetMainUI();
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
        //Windows.Devices.Bluetooth.Rfcomm.RfcommDeviceService _service;
        //Windows.Networking.Sockets.StreamSocket _socket;

        //async void Initialize()
        //{
        //    try
        //    {
        //        // Enumerate devices with the object push service
        //        var services =
        //            await DeviceInformation.FindAllAsync(
        //                RfcommDeviceService.GetDeviceSelector(
        //                    RfcommServiceId.ObexObjectPush));

        //        if (services.Count > 0)
        //        {
        //            // ターゲットBluetooth BR deviceの初期化
        //            var service = await RfcommDeviceService.FromIdAsync(services[0].Id);

        //            // サービスがアプリの最小要件を満たしているかチェック
        //            if (SupportsProtection(service) && await IsCompatibleVersionAsync(service))
        //            {
        //                _service = service;

        //                // ソケットのチェックとターゲットへの接続
        //                _socket = new StreamSocket();
        //                await _socket.ConnectAsync(
        //                    _service.ConnectionHostName,
        //                    _service.ConnectionServiceName,
        //                    SocketProtectionLevel
        //                        .BluetoothEncryptionAllowNullAuthentication);

        //                // ソケットが接続されています。 この時点で、Appはユーザーが
        //                // デバイスにファイルを取得するのを待つことができます。これにより、
        //                //  Pickerが起動され、選択されたファイルが送信されます。 
        //                // 転送自体はRfcomm APIではなくSockets APIを使用するため、
        //                // ここでは簡潔にするために省略しています。

        //            }
        //        }
        //        else
        //        {
        //            ListBox_Messages.Items.Add(string.Format("RfcommDeviceService Count={0}", services.Count));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ListBox_Messages.Items.Add(ex.Message);
        //    }
        //}

        //// This App requires a connection that is encrypted but does not care about
        //// whether it's authenticated.
        //bool SupportsProtection(RfcommDeviceService service)
        //{
        //    switch (service.ProtectionLevel)
        //    {
        //        case SocketProtectionLevel.PlainSocket:
        //            if ((service.MaxProtectionLevel == SocketProtectionLevel
        //                    .BluetoothEncryptionWithAuthentication)
        //                || (service.MaxProtectionLevel == SocketProtectionLevel
        //                    .BluetoothEncryptionAllowNullAuthentication))
        //            {
        //                // The connection can be upgraded when opening the socket so the
        //                // App may offer UI here to notify the user that Windows may
        //                // prompt for a PIN exchange.
        //                return true;
        //            }
        //            else
        //            {
        //                // The connection cannot be upgraded so an App may offer UI here
        //                // to explain why a connection won't be made.
        //                return false;
        //            }
        //        case SocketProtectionLevel.BluetoothEncryptionWithAuthentication:
        //            return true;
        //        case SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication:
        //            return true;
        //    }
        //    return false;
        //}

        //// このアプリはサービスのバージョン2.0で利用可能なCRC32チェックに依存
        //const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        //const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;   // UINT32
        //const uint MINIMUM_SERVICE_VERSION = 200;
        //async Task<bool> IsCompatibleVersionAsync(RfcommDeviceService service)
        //{
        //    try
        //    {
        //        var attributes = await service.GetSdpRawAttributesAsync(
        //            BluetoothCacheMode.Uncached);
        //        var attribute = attributes[SERVICE_VERSION_ATTRIBUTE_ID];
        //        var reader = DataReader.FromBuffer(attribute);

        //        // 第１バイトは属性のタイプを含む
        //        byte attributeType = reader.ReadByte();
        //        if (attributeType == SERVICE_VERSION_ATTRIBUTE_TYPE)
        //        {
        //            // 残りはデータ
        //            uint version = reader.ReadUInt32();
        //            return version >= MINIMUM_SERVICE_VERSION;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ListBox_Messages.Items.Add(string.Format("IsCompatibleVersionAsync: Exception: {0}",ex.Message));
        //        //                return true;
        //        return false;
        //    }

        //    return false;
        //}


    }
}
