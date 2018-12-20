using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.ApplicationModel.Background;
using Windows.Storage;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください


// https://docs.microsoft.com/ja-jp/windows/uwp/devices-sensors/send-or-receive-files-with-rfcomm

namespace BtServer
{
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


    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        // The background task registration for the background advertisement watcher 
        private IBackgroundTaskRegistration taskRegistration;
        // The watcher trigger used to configure the background task registration 
        private RfcommConnectionTrigger trigger;
        // A name is given to the task in order for it to be identifiable across context. 
        private string taskName = "BtServer_BackgroundTask";
        // Entry point for the background task. 
        private string taskEntryPoint = "BtServer.RfcommServerTask";


        // Define the raw bytes that are converted into SDP record
        private byte[] sdpRecordBlob = new byte[]
        {
            0x35, 0x4a,  // DES len = 74 bytes

            // Vol 3 Part B 5.1.15 ServiceName
            // 34 bytes
            0x09, 0x01, 0x00, // UINT16 (0x09) value = 0x0100 [ServiceName]
            0x25, 0x1d,       // TextString (0x25) len = 29 bytes
                0x42, 0x6c, 0x75, 0x65, 0x74, 0x6f, 0x6f, 0x74, 0x68, 0x20,     // Bluetooth <sp>
                0x52, 0x66, 0x63, 0x6f, 0x6d, 0x6d, 0x20,                       // Rfcomm <sp>
                0x43, 0x68, 0x61, 0x74, 0x20,                                   // Chat <sp>
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x64,                       // Service <sp>
            // Vol 3 Part B 5.1.15 ServiceDescription
            // 40 bytes
            0x09, 0x01, 0x01, // UINT16 (0x09) value = 0x0101 [ServiceDescription]
            0x25, 0x23,       // TextString (0x25) = 33 bytes,
                0x42, 0x6c, 0x75, 0x65, 0x74, 0x6f, 0x6f, 0x74, 0x68, 0x20,     // Bluetooth <sp>
                0x52, 0x66, 0x63, 0x6f, 0x6d, 0x6d, 0x20,                       // Rfcomm <sp>
                0x43, 0x68, 0x61, 0x74, 0x20,                                   // Chat <sp>
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x64, 0x20,                  // Service <sp>
                0x69, 0x6e, 0x20, 0x43, 0x23                                    // in C#

        };



        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        //MainPage rootPage = MainPage.Current;
        //public Scenario3_BgChatServer()
        //{
        //    this.InitializeComponent();
        //    trigger = new RfcommConnectionTrigger();

        //    // Local service Id is the only mandatory field that should be used to filter a known service UUID.  
        //    trigger.InboundConnection.LocalServiceId = RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid);

        //    // The SDP record is nice in order to populate optional name and description fields
        //    trigger.InboundConnection.SdpRecord = sdpRecordBlob.AsBuffer();
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    AttachProgressAndCompletedHandlers(task.Value);
                }
            }
        }

//        private async void ListenButton_Click(object sender, RoutedEventArgs e)
        private async void StartListen()
        {
            //ListenButton.IsEnabled = false;
            //DisconnectButton.IsEnabled = true;

            // Registering a background trigger if it is not already registered. Rfcomm Chat Service will now be advertised in the SDP record
            // First get the existing tasks to see if we already registered for it

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    taskRegistration = task.Value;
                    break;
                }
            }

            if (taskRegistration != null)
            {
                ListBox_Messages.Items.Add("Background watcher already registered.");
                //                rootPage.NotifyUser("Background watcher already registered.", NotifyType.StatusMessage);
                return;
            }
            else
            {
                // Applications registering for background trigger must request for permission.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                var builder = new BackgroundTaskBuilder();
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(trigger);
                builder.Name = taskName;

                try
                {
                    taskRegistration = builder.Register();
                    AttachProgressAndCompletedHandlers(taskRegistration);

                    // Even though the trigger is registered successfully, it might be blocked. Notify the user if that is the case.
                    if ((backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed) || (backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy))
                    {
                        ListBox_Messages.Items.Add("Background watcher registered.");
                        //                        rootPage.NotifyUser("Background watcher registered.", NotifyType.StatusMessage);
                    }
                    else
                    {
                        ListBox_Messages.Items.Add("Background tasks may be disabled for this app");
                        //                        rootPage.NotifyUser("Background tasks may be disabled for this app", NotifyType.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ListBox_Messages.Items.Add(string.Format("Exception: Background task not registered: {0}", ex.Message));
                    //                    rootPage.NotifyUser("Background task not registered",
                    //                            NotifyType.ErrorMessage);
                }
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
        /// Sends the current message in MessageTextBox.  Also makes sure the text is not empty and updates the conversation list.  
        /// </summary>
        private void SendMessage()
        {
//            var message = MessageTextBox.Text;
            var message = "Hello";

            var previousMessage = (string)ApplicationData.Current.LocalSettings.Values["SendMessage"];

            // Make sure previous message has been sent
            if (previousMessage == null || previousMessage == "")
            {
                // Save the current message to local settings so the background task can pick it up. 
                ApplicationData.Current.LocalSettings.Values["SendMessage"] = message;

                // Clear the messageTextBox for a new message
//                MessageTextBox.Text = "";
//                ConversationListBox.Items.Add("Sent: " + message);
            }
            else
            {
                // Do nothing until previous message has been sent.  
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        /// <summary>
        /// Called when background task defferal is completed.  This can happen for a number of reasons (both expected and unexpected).  
        /// IF this is expected, we'll notify the user.  If it's not, we'll show that this is an error.  Finally, clean up the connection by calling Disconnect().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("TaskCancelationReason"))
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ListBox_Messages.Items.Add("Task cancelled unexpectedly - reason: ");
                    //                    rootPage.NotifyUser("Task cancelled unexpectedly - reason: " + settings.Values["TaskCancelationReason"].ToString(), NotifyType.ErrorMessage);
                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ListBox_Messages.Items.Add("Background task completed");
                    //                    rootPage.NotifyUser("Background task completed", NotifyType.StatusMessage);
                });
            }
            try
            {
                args.CheckResult();
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(ex.Message);
                //                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
            Disconnect();
        }

        /// <summary>
        /// Handles UX changes and task registration changes when socket is disconnected
        /// </summary>
        private async void Disconnect()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //ListenButton.IsEnabled = true;
                //DisconnectButton.IsEnabled = false;
                //ConversationListBox.Items.Clear();

                // Unregistering the background task will remove the Rfcomm Chat Service from the SDP record and stop listening for incoming connections
                // First get the existing tasks to see if we already registered for it
                if (taskRegistration != null)
                {
                    taskRegistration.Unregister(true);
                    taskRegistration = null;
//                    rootPage.NotifyUser("Background watcher unregistered.", NotifyType.StatusMessage);
                }
                else
                {
                    // At this point we assume we haven't found any existing tasks matching the one we want to unregister
//                    rootPage.NotifyUser("No registered background watcher found.", NotifyType.StatusMessage);
                }
            });

        }

        /// <summary>
        /// The background task updates the progress counter.  When that happens, this event handler gets invoked
        /// When the handler is invoked, we will display the value stored in local settings to the user.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="args"></param>
        private async void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {

            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("ReceivedMessage"))
            {
                string backgroundMessage = (string)ApplicationData.Current.LocalSettings.Values["ReceivedMessage"];
                string remoteDeviceName = (string)ApplicationData.Current.LocalSettings.Values["RemoteDeviceName"];

                if (!backgroundMessage.Equals(""))
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
//                        rootPage.NotifyUser("Client Connected: " + remoteDeviceName, NotifyType.StatusMessage);
//                        ConversationListBox.Items.Add("Received: " + backgroundMessage);
                    });
                }
            }
        }

        private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        {
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        //        RfcommServiceProvider _provider;
        ////        RfcommDeviceService _service;
        //        StreamSocket _socket;

        //        async void Initialize()
        //        {
        //            try
        //            {
        //                // Initialize the provider for the hosted RFCOMM service
        //                _provider = await RfcommServiceProvider.CreateAsync(
        //                    RfcommServiceId.ObexObjectPush);

        //                // Create a listener for this service and start listening
        //                StreamSocketListener listener = new StreamSocketListener();
        //                listener.ConnectionReceived += OnConnectionReceivedAsync;
        //                await listener.BindServiceNameAsync(
        //                    _provider.ServiceId.AsString(),
        //                    SocketProtectionLevel
        //                        .BluetoothEncryptionAllowNullAuthentication);

        //                // Set the SDP attributes and start advertising
        //                InitializeServiceSdpAttributes(_provider);
        //                _provider.StartAdvertising(listener);
        //            }
        //            catch (Exception ex)
        //            {
        //                ListBox_Messages.Items.Add(ex.Message);
        //            }
        //        }

        //        const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        //        const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;   // UINT32
        //        const uint SERVICE_VERSION = 200;
        //        void InitializeServiceSdpAttributes(RfcommServiceProvider provider)
        //        {
        //            var writer = new Windows.Storage.Streams.DataWriter();

        //            // First write the attribute type
        //            writer.WriteByte(SERVICE_VERSION_ATTRIBUTE_TYPE);
        //            // Then write the data
        //            writer.WriteUInt32(SERVICE_VERSION);

        //            var data = writer.DetachBuffer();
        //            provider.SdpRawAttributes.Add(SERVICE_VERSION_ATTRIBUTE_ID, data);
        //        }

        //        async void OnConnectionReceivedAsync(
        //            StreamSocketListener listener,
        //            StreamSocketListenerConnectionReceivedEventArgs args)
        //        {
        //            // Stop advertising/listening so that we're only serving one client
        //            _provider.StopAdvertising();
        //  //          await listener.Close();
        //            listener.Dispose();

        //            _socket = args.Socket;

        //            // The client socket is connected. At this point the App can wait for
        //            // the user to take some action, e.g. click a button to receive a file
        //            // from the device, which could invoke the Picker and then save the
        //            // received file to the picked location. The transfer itself would use
        //            // the Sockets API and not the Rfcomm API, and so is omitted here for
        //            // brevity.
        //        }

        private void Pbtn_Start_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Messages.Items.Add("Start Initialization");
            InitializeBtServer();
        }

        private void InitializeBtServer()
        {
            trigger = new RfcommConnectionTrigger();

            // Local service Id is the only mandatory field that should be used to filter a known service UUID.  
            trigger.InboundConnection.LocalServiceId = RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid);

            // The SDP record is nice in order to populate optional name and description fields
            trigger.InboundConnection.SdpRecord = sdpRecordBlob.AsBuffer();


            StartListen();
        }
    }
}
