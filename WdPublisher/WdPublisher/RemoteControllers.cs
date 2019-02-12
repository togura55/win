using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Storage;
using Windows.UI.Core;

namespace WillDevicesSampleApp
{
    public class RemoteControllers
    {
        //public sealed partial class Scenario3_BgChatServer : Page
        //{
        // The background task registration for the background advertisement watcher 
        private IBackgroundTaskRegistration taskRegistration;
        // The watcher trigger used to configure the background task registration 
        private RfcommConnectionTrigger trigger;
        // A name is given to the task in order for it to be identifiable across context. 
        //         private string taskName = "Scenario3_BackgroundTask";
        private string taskName = "WdPublisher_BackgroundTask";  // Equivalent with the assembly name of Tasks
        // Entry point for the background task. 
        private string taskEntryPoint = "BackgroundTasks.RfcommServerTask";


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
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65,                       // Service <sp>
            // Vol 3 Part B 5.1.15 ServiceDescription
            // 40 bytes
            0x09, 0x01, 0x01, // UINT16 (0x09) value = 0x0101 [ServiceDescription]
            0x25, 0x23,       // TextString (0x25) = 33 bytes,
                0x42, 0x6c, 0x75, 0x65, 0x74, 0x6f, 0x6f, 0x74, 0x68, 0x20,     // Bluetooth <sp>
                0x52, 0x66, 0x63, 0x6f, 0x6d, 0x6d, 0x20,                       // Rfcomm <sp>
                0x43, 0x68, 0x61, 0x74, 0x20,                                   // Chat <sp>
                0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65, 0x20,                  // Service <sp>
                0x69, 0x6e, 0x20, 0x43, 0x23                                    // in C#

        };

        // Delegate handlers
        public delegate void MessageEventHandler(object sender, string message);

        // Properties
        public event MessageEventHandler RCMessage;
        public event MessageEventHandler UpdateUi, PublisherControl;

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.RCMessage?.Invoke(this, message);
            });
        }
        private async void MessageUpdateUi()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.UpdateUi?.Invoke(this, null);
                    });
        }
        private async Task<bool> MessagePublisherControl(string message)
        {
            bool responce = true;

            Publishers pub = AppObjects.Instance.Publisher;

            if ((message == CMD_START && pub.State == pub.PUBLISHER_STATE_STOP) ||
                (message == CMD_STOP && pub.State == pub.PUBLISHER_STATE_START))
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PublisherControl?.Invoke(this, message);
                });
            }
            else
            {
                responce = false;
            }

            return responce;
        }

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        //            MainPage rootPage = MainPage.Current;
        public RemoteControllers()
        {
            //                this.InitializeComponent();
            trigger = new RfcommConnectionTrigger();

            // Local service Id is the only mandatory field that should be used to filter a known service UUID.  
            trigger.InboundConnection.LocalServiceId = RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid);

            // The SDP record is nice in order to populate optional name and description fields
            trigger.InboundConnection.SdpRecord = sdpRecordBlob.AsBuffer();
        }

        //        protected override void OnNavigatedTo(NavigationEventArgs e)
        public void RegisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    AttachProgressAndCompletedHandlers(task.Value);
                }
            }
        }

        public async void StartListen()
        //        private async void ListenButton_Click(object sender, RoutedEventArgs e)
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
                MessageEvent("Background watcher already registered.");
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
                        MessageEvent("Background watcher registered.");
                    }
                    else
                    {
                        MessageEvent("Background tasks may be disabled for this app");
                    }
                }
                catch (Exception ex)
                {
                    MessageEvent(string.Format("StartListen: Exception: Background task not registered.:{0}", ex.Message));
                }
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
        /// Sends the current message in MessageTextBox.  Also makes sure the text is not empty and updates the conversation list.  
        /// </summary>
        private void SendMessage(string message)
        //         private void SendMessage()
        {
            //            var message = MessageTextBox.Text;
            var previousMessage = (string)ApplicationData.Current.LocalSettings.Values["SendMessage"];

            // Make sure previous message has been sent
            if (previousMessage == null || previousMessage == "")
            {
                // Save the current message to local settings so the background task can pick it up. 
                ApplicationData.Current.LocalSettings.Values["SendMessage"] = message;

                // Clear the messageTextBox for a new message
                //                MessageTextBox.Text = "";
                MessageEvent("Sent: " + message);
                //                ConversationListBox.Items.Add("Sent: " + message);
            }
            else
            {
                // Do nothing until previous message has been sent.  
            }
        }

        /// <summary>
        /// Called when background task defferal is completed.  This can happen for a number of reasons (both expected and unexpected).  
        /// IF this is expected, we'll notify the user.  If it's not, we'll show that this is an error.  Finally, clean up the connection by calling Disconnect().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("TaskCancelationReason"))
            {
                MessageEvent(string.Format("Task cancelled unexpectedly - reason: {0}", settings.Values["TaskCancelationReason"].ToString()));
            }
            else
            {
                MessageEvent("Background task completed");
            }

            try
            {
                args.CheckResult();
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("OnComplete: Exception: {0}", ex.Message));
            }
            Disconnect();
        }

        /// <summary>
        /// Handles UX changes and task registration changes when socket is disconnected
        /// </summary>
        private async void Disconnect()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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
                    //                   rootPage.NotifyUser("Background watcher unregistered.", NotifyType.StatusMessage);
                    MessageEvent("Disconnect: Background watcher unregistered.");
                }
                else
                {
                    // At this point we assume we haven't found any existing tasks matching the one we want to unregister
                    //                       rootPage.NotifyUser("No registered background watcher found.", NotifyType.StatusMessage);
                    MessageEvent("Disconnect: No registered background watcher found.");
                }
            });

        }

        /// <summary>
        /// The background task updates the progress counter.  When that happens, this event handler gets invoked
        /// When the handler is invoked, we will display the value stored in local settings to the user.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="args"></param>
        private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {

            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("ReceivedMessage"))
            {
                string backgroundMessage = (string)ApplicationData.Current.LocalSettings.Values["ReceivedMessage"];
                string remoteDeviceName = (string)ApplicationData.Current.LocalSettings.Values["RemoteDeviceName"];

                if (!backgroundMessage.Equals(""))
                {
                    MessageEvent("Client Connected: " + remoteDeviceName);
                    MessageEvent("Received: " + backgroundMessage);
                    //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    //{
                    //    rootPage.NotifyUser("Client Connected: " + remoteDeviceName, NotifyType.StatusMessage);
                    //    ConversationListBox.Items.Add("Received: " + backgroundMessage);
                    //});

                    // send backgroundMessage to dispatcher
                    ConfigCommandsDispatcher(backgroundMessage);
                }
            }
        }

        private void AttachProgressAndCompletedHandlers(IBackgroundTaskRegistration task)
        {
            task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        private void SendResponce(string response)
        {
            //            var message = MessageTextBox.Text;
            var previousMessage = (string)ApplicationData.Current.LocalSettings.Values["SendMessage"];

            // Make sure previous message has been sent
            if (previousMessage == null || previousMessage == "")
            {
                // Save the current message to local settings so the background task can pick it up. 
                ApplicationData.Current.LocalSettings.Values["SendMessage"] = response;

                // Clear the messageTextBox for a new message
                //                MessageTextBox.Text = "";
                //                ConversationListBox.Items.Add("Sent: " + message);
                MessageEvent("Sent: " + response);
            }
            else
            {
                // Do nothing until previous message has been sent.  
            }
        }

        private const string CMD_START = "start";
        private const string CMD_STOP = "stop";
        private const string CMD_RESTART = "restart";
        private const string CMD_GETCONFIG = "getconfig";
        private const string CMD_SETCONFIG = "setconfig";  // setconfig,aaa,bbb,ccc
        private const string CMD_GETVERSION = "getversion";
        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";
        //        static List<string> CommandList = new List<string> { "1", "2", "3", "4", "5" };  // Command word sent by Publisher

        private string ExecuteGetConfig()
        {
            string responce = string.Empty;
            string sep = ",";
            try
            {
                WacomDevices.DeviceAttributes DevAttr = AppObjects.Instance.WacomDevice.Attribute;
                Publishers Pub = AppObjects.Instance.Publisher;

                responce +=
                    DevAttr.Width + sep +
                    DevAttr.Height + sep +
                    DevAttr.PointSize + sep +
                    DevAttr.Name + sep +
                    DevAttr.ESN + sep +
                    DevAttr.Battery + sep +
                    DevAttr.DeviceType + sep +
                    DevAttr.TransferMode + sep +
                    Pub.HostNameString + sep +
                    Pub.PortNumberString + sep +
                    Pub.State;
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("ExecuteGetConfig: Exception: {0}", ex.Message));
            }

            return responce;
        }

        private string ExecuteSetConfig(string message)
        {
            string responce = string.Empty;
            try
            {
                //WacomDevices.DeviceAttributes DevAttr = AppObjects.Instance.WacomDevice.Attribute;
                Publishers Pub = AppObjects.Instance.Publisher;

                char sp = ','; // separater
                string[] arr = message.Split(sp);
                var list = new List<string>();
                list.AddRange(arr);

                // decode
                if (list.Count != 4)
                {
                    responce = RES_NAK;
                    throw new Exception("ExecuteSetConfig: Number of parameters are wrong.");
                }
                else
                {
                    int i = 0;
 
                    if (list[++i] != string.Empty)
                    {
                        // Set Device Name
                        WacomDevices wacomDevice = AppObjects.Instance.WacomDevice;

                        if (wacomDevice != null)
                        {
                            wacomDevice.Attribute.Name = list[i];
                            
                            // ToDo: issue settings to the device
                        }
                    }

                    if (list[++i] != string.Empty)
                    {
                        // Set Broker's IP address
                        Pub.HostNameString = list[i];
                    }
                    if (list[++i] != string.Empty)
                    {
                        // Set Broker's Port number
                        Pub.PortNumberString = list[i];
                    }

                    MessageUpdateUi();
                    responce = RES_ACK;
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("ExecuteSetConfig: Exception: {0}", ex.Message));
            }

            return responce;
        }

        private string ExecuteGetVersion()
        {
            string responce = string.Empty;
            try
            {
                var versionInfo = Windows.ApplicationModel.Package.Current.Id.Version;
                responce = string.Format(
                                   "{0}.{1}.{2}.{3}",
                                   versionInfo.Major, versionInfo.Minor,
                                   versionInfo.Build, versionInfo.Revision);
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("ExecuteGetVersion: Exception: {0}", ex.Message));
            }

            return responce;
        }

        private async void ConfigCommandsDispatcher(string message)
        {
            try
            {
                string res = string.Empty;

                char sp = ','; // separater
                string[] arr = message.Split(sp);
                var list = new List<string>();
                list.AddRange(arr);

                // decode
                if (list.Count < 1)
                {
                    // error, resend?
                    return;
                }
                string command = list[0];

                switch (command)
                {
                    case CMD_START:
                    case CMD_STOP:
                        // Check and Start Publisher
                        SendResponce(
                            await MessagePublisherControl(command) ? RES_ACK : RES_NAK);
                        break;

                    case CMD_GETCONFIG:
                        SendResponce(ExecuteGetConfig());
                        break;

                    case CMD_SETCONFIG:
                        SendResponce(ExecuteSetConfig(message));
                        break;

                    case CMD_GETVERSION:
                        string ver = ExecuteGetVersion();
                        SendResponce(
                            string.IsNullOrEmpty(ver) ? RES_NAK : ver);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("ConfigCommandsDispatcher: Exception: {0}", ex.Message));
            }
        }

    }
}
