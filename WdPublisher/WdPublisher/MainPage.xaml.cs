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
    // Auto pilot
    public static class ButtonExtension
    {
        public static void RaiseClick(this Button button)
        {
            var peer = new ButtonAutomationPeer(button);
            var invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }
    }

    public sealed partial class MainPage : Page
    {
        static bool autopilot = true; // Auto pilot

        CancellationTokenSource m_cts = new CancellationTokenSource();

        static WacomDevices wacomDevices;

        // ---- for sockets
 //       static SocketClient socketClient;  // suppose single instance
        IBuffer buffer = null; // data buffer sent to the server

        class RawData
        {
            public float f;
            public float x;
            public float y;
            public float z;
            public RawData(float f = 0, float x = 0, float y = 0, float z = 0)
            {
                this.f = f;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        private void RawDataToBuffer(ArrayList rawdatalist)
        {
            // convert to byte array and IBuffer
            int num_bytes = sizeof(float);
            byte[] ByteArray = new byte[rawdatalist.Count * num_bytes * 4];
            int count = 0;
            foreach (RawData rd in rawdatalist)
            {
                int offset = count * num_bytes * 4;

                Array.Copy(BitConverter.GetBytes(rd.f), 0, ByteArray, offset, num_bytes);
                Array.Copy(BitConverter.GetBytes(rd.x), 0, ByteArray, offset += num_bytes, num_bytes);
                Array.Copy(BitConverter.GetBytes(rd.y), 0, ByteArray, offset += num_bytes, num_bytes);
                Array.Copy(BitConverter.GetBytes(rd.z), 0, ByteArray, offset += num_bytes, num_bytes);
                count++;
            }
            using (DataWriter writer = new DataWriter())
            {
                writer.WriteBytes(ByteArray);
                buffer = writer.DetachBuffer();
            }
        }
        // ------ end of ----------

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;

            wacomDevices = new WacomDevices();
            wacomDevices.WacomDevicesMessage += ReceivedMessage; // set the message delegate

            AppObjects.Instance.SocketClient = new SocketClient();
            AppObjects.Instance.SocketClient.SocketClientMessage += ReceivedMessage; // 

            RestoreSettings();

            //            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");

            this.TextBox_HostName.Text = AppObjects.Instance.SocketClient.HostNameString;
            this.TextBox_PortNumber.Text = AppObjects.Instance.SocketClient.PortNumberString;
            // ----- end of ------

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

        }

        private void GetUiState()
        {
            AppObjects.Instance.SocketClient.HostNameString = this.TextBox_HostName.Text;
            AppObjects.Instance.SocketClient.PortNumberString = this.TextBox_PortNumber.Text;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            //if (AppObjects.Instance.DeviceInfo == null)
            //{
            //	AppObjects.Instance.DeviceInfo = await AppObjects.DeserializeDeviceInfoAsync();
            //}

            if (AppObjects.Instance.DeviceInfo == null)
            {

                //                Pbtn_Exec.RaiseClick();  // Auto pilot
                return;
            }

            InkDeviceInfo inkDeviceInfo = AppObjects.Instance.DeviceInfo;

            try
            {
                if (AppObjects.Instance.Device == null)
                {
                    AppObjects.Instance.Device = await InkDeviceFactory.Instance.CreateDeviceAsync(inkDeviceInfo, AppObjects.Instance.AppId, false, false, OnDeviceStatusChanged);
                }

                AppObjects.Instance.Device.Disconnected += OnDeviceDisconnected;
                AppObjects.Instance.Device.DeviceStatusChanged += OnDeviceStatusChanged;
                AppObjects.Instance.Device.PairingModeEnabledCallback = OnPairingModeEnabledAsync;
            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("MainPage_Loaded: Exception: {0}", ex.Message));
                return;
            }

            // Auto pilot
            if (autopilot)
            {
                autopilot = !autopilot;
                //               Pbtn_Exec.RaiseClick();  // ButtonScan_Click
            }
        }

        // Message event handler sent by instance object
        private void ReceivedMessage(object sender, string message)
        {
            clientListBox.Items.Add(message);
        }

        private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                switch (e.Status)
                {
                    case DeviceStatus.Idle:
                        break;

                    case DeviceStatus.ExpectingConnectionConfirmation:
                        break;

                    case DeviceStatus.NotAuthorizedConnectionNotConfirmed:
                        await new MessageDialog(AppObjects.GetStringForDeviceStatus(e.Status)).ShowAsync();
                        //Frame.Navigate(typeof(ScanAndConnectPage));
                        break;

                    default:
                        //						textBlockStatus.Text = AppObjects.GetStringForDeviceStatus(e.Status);
                        break;
                }
            });
        }

        private async Task<bool> OnPairingModeEnabledAsync(bool authorizedInThisSession)
        {
            if (!authorizedInThisSession)
                return true;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                bool keepUsingDevice = await AppObjects.Instance.ShowPairingModeEnabledDialogAsync();

                tcs.SetResult(keepUsingDevice);

                if (!keepUsingDevice)
                {
                    //Frame.Navigate(typeof(ScanAndConnectPage));
                }
            });

            return await tcs.Task;
        }

        private void OnDeviceDisconnected(object sender, EventArgs e)
        {
            AppObjects.Instance.Device = null;

            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await new MessageDialog($"The device {AppObjects.Instance.DeviceInfo.DeviceName} was disconnected.").ShowAsync();

                //Frame.Navigate(typeof(ScanAndConnectPage));
            });
        }

        private void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            try
            {
                // Set completion delegation 
                wacomDevices.ScanAndConnectCompletedNotification += ScanAndConnect_Completed;
                wacomDevices.StartRealTimeInkCompletedNotification += SocketProc_Completed;
                AppObjects.Instance.SocketClient.SocketClientConnectCompletedNotification += SocketClientConnect_Completed;

                wacomDevices.StartScanAndConnect();


            }
            catch (Exception ex)
            {
                clientListBox.Items.Add(string.Format("Pbtn_Exec_Click: {0}", ex.Message));
            }
        }

        private async void ScanAndConnect_Completed(object sender, bool result)
        {
            if (AppObjects.Instance.Device != null)
            {
                clientListBox.Items.Add("ScanAndConnect_Completed: Go Socket proc.");
                await SocketProc();
            }
            else
            {
                clientListBox.Items.Add("ScanAndConnect_Completed: Device is null");
            }
        }

        private async void SocketClientConnect_Completed(object sender, bool result)
        {
            clientListBox.Items.Add("SocketClientConnect_Completed: OK, start the RealTimeInk Transmission!");
            //
            await wacomDevices.StartRealtimeInk();
        }

        private async void SocketProc_Completed(object sender, bool result)
        {
            if (result)  // socket was established
            {
                clientListBox.Items.Add("SocketProc_Completed: Go StartRealTimeInk.");
                await wacomDevices.StartRealtimeInk();
            }
            else
            {
                clientListBox.Items.Add("SocketProc_Completed: got false.");
            }
        }

        private async Task SocketProc()
        {
            // for debug: Create raw data and stack on a buffer
            ArrayList RawDataList = new ArrayList
            {
                new RawData(1, 100, 200, 3756),
                new RawData(0, 101, 223, 4675),
                new RawData(0, 102, 234, 323),
                new RawData(0, 105, 278, 32134)
            };
            RawDataToBuffer(RawDataList); // convert

            try
            {
                clientListBox.Items.Add(string.Format("{0}", "Start. SocketProc()"));

                await AppObjects.Instance.SocketClient.Connect();

 //               socketClient.BatchedSends(buffer);

                //socketClient.Receive();

                //socketClient.Disonnect();

                clientListBox.Items.Add(string.Format("{0}", "Completed. SocketProc()"));

 //               SocketProc_Completed(this, true);
            }
            catch (Exception ex)
            {
                AppObjects.Instance.SocketClient.Disonnect();
                clientListBox.Items.Add(string.Format("{0}", ex.Message));
            }
        }

        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs s)
        {
            GetUiState();
            StoreSettings();

            wacomDevices.WacomDevicesMessage -= ReceivedMessage;
            AppObjects.Instance.SocketClient.SocketClientMessage -= ReceivedMessage;
        }

        #region Store/Restore local data
        private void StoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["HostNameString"] = AppObjects.Instance.SocketClient.HostNameString;
            container.Values["PortNumberString"] = AppObjects.Instance.SocketClient.PortNumberString;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("HostNameString"))
                AppObjects.Instance.SocketClient.HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("PortNumberString"))
                AppObjects.Instance.SocketClient.PortNumberString = container.Values["PortNumberString"].ToString();
        }
        #endregion

    }
}
