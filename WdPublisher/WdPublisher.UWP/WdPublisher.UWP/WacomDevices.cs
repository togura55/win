using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Wacom;
using Wacom.Devices;
using Wacom.SmartPadCommunication;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using Windows.Storage.Streams;

namespace WillDevicesSampleApp
{
    public class WacomDevices
    {
        private const uint MASK_ID = 0x00FF;
        private const uint MASK_STROKE = 0x0F00;
        private const uint MASK_COMMAND = 0xF000;

        private const float micrometerToDip = 96.0f / 25400.0f;
        private CancellationTokenSource m_cts;
        private static readonly float maxP = 1.402218f;
        private static readonly float pFactor = 1.0f / (maxP - 1.0f);
        int PointCount;
        int StrokeCount;
        ////        public InkTransfer inkTransfer;
        int m_StrokeOrder; // 1: start, 0: intermediate, 2: end

        public delegate void MessageEventHandler(object sender, string message);
        public delegate void ScanAndConnectCompletedNotificationHandler(object sender, bool result);
        public delegate void StartRealTimeInkCompletedNotificationHandler(object sender, bool result);
        public delegate void DeviceEventNotificationNotificationHandler(object sender, string message);
        // Properties
        public event MessageEventHandler WacomDevicesMessage;
        public event ScanAndConnectCompletedNotificationHandler ScanAndConnectCompletedNotification;
        public event StartRealTimeInkCompletedNotificationHandler StartRealTimeInkCompletedNotification;
        public event DeviceEventNotificationNotificationHandler DeviceEventNotification;

        public float PublisherAttribute;

        InkDeviceWatcherUSB m_watcherUSB;
        InkDeviceInfo m_connectingDeviceInfo;
        ObservableCollection<InkDeviceInfo> m_deviceInfos;

        #region Realtime Ink Raw Data Collection
        public ObservableCollection<StrokeRawData> StrokeRawDataInfos
        {
            get
            {
                return m_StrokeRawData;
            }
        }
        public ObservableCollection<StrokeRawData> m_StrokeRawData;
        public class StrokeRawData
        {
            public string PointCount { get; set; }
            public string StrokeCount { get; set; }
            public string X { get; set; }
            public string Y { get; set; }
            public string W { get; set; }
            //            public string FormattedStrings { get; }
            public StrokeRawData(string PointCount, string StrokeCount, string x, string y, string w)
            {
                this.X = x;
                this.Y = y;
                this.W = w;
                //this.FormattedStrings = string.Format("[{0}][{1}] {2}",
                //    PointCount, StrokeCount, x + '\t' + y + '\t' + w);
            }
        }
        #endregion

        #region Device attribute definition and object sharing between Publisher and Broker
        public class DeviceAttributes
        {
            public string Width;
            public string Height;
            public string PointSize;
            public string Name;
            public string ESN;
            public string Battery;
            public string FirmwareVersion;
            public string DeviceType;
            public string TransferMode;
            public string Barcode;

            public DeviceAttributes()
            {
                Width = string.Empty;
                Height = string.Empty;
                PointSize = string.Empty;
                Name = string.Empty;
                ESN = string.Empty;
                Battery = string.Empty;
                FirmwareVersion = string.Empty;
                DeviceType = string.Empty;
                TransferMode = string.Empty;
                Barcode = string.Empty;
            }

            public string GenerateStrings()
            {
                string s = string.Empty;

                s = Width + "," + Height + "," + PointSize + "," + Name + "," +
                    ESN + "," + Battery + "," + FirmwareVersion + "," + 
                    DeviceType + "," + TransferMode + "," + Barcode;

                return s;
            }
        }
        public DeviceAttributes Attribute;
        #endregion

        public WacomDevices()
        {
            PublisherAttribute = 0;
            PointCount = 0;
            StrokeCount = 0;
            m_StrokeOrder = 1; // 1: start, 0: intermediate, 2: end
            m_cts = new CancellationTokenSource();
            WacomDevicesMessage = null;
            ScanAndConnectCompletedNotification = null;
            StartRealTimeInkCompletedNotification = null;

            m_watcherUSB = new InkDeviceWatcherUSB();
            m_watcherUSB.DeviceAdded += OnDeviceAdded;
            m_watcherUSB.DeviceRemoved += OnDeviceRemoved;
            m_watcherUSB.WatcherStopped += OnUsbWatcherStopped;
            //       m_watcherUSB.EnumerationCompleted += OnUsbEnumerationCompleted;

            m_connectingDeviceInfo = null;
            m_deviceInfos = new ObservableCollection<InkDeviceInfo>();
            m_StrokeRawData = new ObservableCollection<StrokeRawData>();

            Attribute = new DeviceAttributes();
        }

        private async Task MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.WacomDevicesMessage?.Invoke(this, message);
            });
        }

        #region RealTimeInk
        public async void StartRealTimeInk()
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            if (device == null)
            {
                await MessageEvent("StartRealtimeInk: Device not connected.");
                return;
            }

            device.Disconnected += OnDeviceDisconnected;
            device.DeviceStatusChanged += OnDeviceStatusChanged;
            //            device.PairingModeEnabledCallback = OnPairingModeEnabledAsync;
            device.BarCodeScanned += OnBarCodeScanned;

            IRealTimeInkService service = device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;
            if(service == null)
            {
                await MessageEvent("StartRealTimeInk: The Real-time Ink service is not supported on this device");
                return;
            }

            try
            {
                //uint width = (uint)await device.GetPropertyAsync("Width", m_cts.Token);
                //uint height = (uint)await device.GetPropertyAsync("Height", m_cts.Token);
                //uint ptSize = (uint)await device.GetPropertyAsync("PointSize", m_cts.Token);

                //// Register private events for getting stroke data /////
                service.StrokeStarted += Service_BeginStroke;
                //service.StrokeUpdated += Service_MiddleStroke;
                //service.StrokeEnded += Service_EndStroke;

                //            service.HoverPointReceived += OnHoverPointReceived;
                /////////////////////////////////////////////////////////////

                await MessageEvent(string.Format("StartRealTimeInk: {0}", AppObjects.GetStringForDeviceStatus(device.DeviceStatus)));

                bool start_flag = false;
                if (!service.IsStarted)
                {
//                    await service.StartAsync(false, m_cts.Token);
                    await service.StartAsync(true, m_cts.Token);
                    start_flag = true;
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.StartRealTimeInkCompletedNotification?.Invoke(this, start_flag);
                });
            }
            catch (Exception ex)
            {
                await MessageEvent(string.Format("StartRealTimeInk: Exception: {0}", ex.Message));
            }
        }

        public async Task StopRealTimeInk()
        {
            try
            {
                if (AppObjects.Instance.Device != null)
                {
                    if ((AppObjects.Instance.Device.GetService(InkDeviceService.RealTimeInk) is IRealTimeInkService service) && service.IsStarted)
                    {
                        await service.StopAsync(m_cts.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageEvent(string.Format("StopRealtimeInk: Exception: {0}", ex.Message));
            }
        }

        #region Stroke event handlers
//        private async void Service_BeginStroke(object sender, StrokeStartedEventArgs e)
        private void Service_BeginStroke(object sender, StrokeStartedEventArgs e)
        {
            try
            {
                m_StrokeOrder = 1;

                //m_addNewStrokeToModel = true;
                StrokeCount++;

                // for debug
                //                MessageEvent("BeginStroke");

                //await Task.Run(() => 
                //{
                //    this.WacomDevicesMessage?.Invoke(this, "BeginStroke");
                //});


                var ignore = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.WacomDevicesMessage?.Invoke(this, "BeginStroke");
                });

                //// For debug
                if (!AppObjects.Instance.Publisher.Debug)
                    // End For debug
                    if (AppObjects.Instance.DataSocketService != null)
                        AppObjects.Instance.DataSocketService.StreamSocket_SendData(CreateBuffer(null, m_StrokeOrder));
            }
            catch (Exception ex)
            {
//                await MessageEvent(string.Format("Service_BeginStroke: Exception: {0}", ex.Message));
            }
        }

//        private async void Service_MiddleStroke(object sender, StrokeUpdatedEventArgs e)
        private async Task Service_MiddleStroke(object sender, StrokeUpdatedEventArgs e)
        {
            try
            {
                m_StrokeOrder = 0;

                var pathPart = e.PathPart;

                // For debug
                if (!AppObjects.Instance.Publisher.Debug)
                // End For debug
                if (AppObjects.Instance.DataSocketService != null)
                    AppObjects.Instance.DataSocketService.StreamSocket_SendData(CreateBuffer(pathPart, m_StrokeOrder));

                //var point = new StylusPoint(x * m_scale, y * m_scale, w);
                //if (m_addNewStrokeToModel)
                //{
                //    m_addNewStrokeToModel = false;
                //}
                PointCount++;
            }
            catch (Exception ex)
            {
                await MessageEvent(string.Format("Service_MiddleStroke: Exception: {0}", ex.Message));
            }
        }

//        private async void Service_EndStroke(object sender, StrokeEndedEventArgs e)
        private async Task Service_EndStroke(object sender, StrokeEndedEventArgs e)
        {
            try
            {
                m_StrokeOrder = 2;
 //               m_addNewStrokeToModel = true;

                var pathPart = e.PathPart;
                // For debug
                if (!AppObjects.Instance.Publisher.Debug)
                // End For debug
                    if (AppObjects.Instance.DataSocketService != null)
                    AppObjects.Instance.DataSocketService.StreamSocket_SendData(CreateBuffer(pathPart, m_StrokeOrder));
            }
            catch (Exception ex)
            {
                await MessageEvent(string.Format("Service_EndStroke: Exception: {0}", ex.Message));
            }
        }

        private IBuffer CreateBuffer(Wacom.Ink.Path pathPart, int strokeOrder)
        {
            IBuffer buffer = null;
            int count = 0;
            int index = 0;
            float x = 0, y = 0, p = 0, f = 0;
            int stride = 0;
            int num_bytes = sizeof(float);  // 4 bytes

            try
            {
                f = ((uint)this.PublisherAttribute & ~MASK_STROKE) | ((uint)strokeOrder << 8);
                this.PublisherAttribute = f;

                if (pathPart == null) // BeginStroke
                {
                    count = 1;
                }
                else  // MiddleStroke ot EndStroke
                {
                    if (pathPart.DataStride == 3)
                    {
                        stride = 3;
                        count = pathPart.Data.Count / stride;
                    }
                    else
                    {

                    }
                }

                //                    InkPoint[] points = new InkPoint[count];

                byte[] ByteArray = new byte[num_bytes * 4 * count];
                int offset = 0;
                for (int i = 0; i < count; i++)
                {
                    if (pathPart != null)  // only for MiddleStroke or EndStroke
                    {
                        x = pathPart.Data[index];
                        y = pathPart.Data[index + 1];
                        p = pathPart.Data[index + 2] / maxP;
                    }

                    //                        points[i] = new InkPoint(new Windows.Foundation.Point(x * mScale, y * mScale), p);

                    Array.Copy(BitConverter.GetBytes(f), 0, ByteArray, offset, num_bytes);
                    Array.Copy(BitConverter.GetBytes(x), 0, ByteArray, offset += num_bytes, num_bytes);
                    Array.Copy(BitConverter.GetBytes(y), 0, ByteArray, offset += num_bytes, num_bytes);
                    Array.Copy(BitConverter.GetBytes(p), 0, ByteArray, offset += num_bytes, num_bytes);

                    index += stride;
                    offset += num_bytes;
                }

                using (DataWriter writer = new DataWriter())
                {
                    writer.WriteBytes(ByteArray);
                    buffer = writer.DetachBuffer();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CreateBuffer: Exception: {0}", ex.Message));
            }

            return buffer;
        }

        private async void OnBarCodeScanned(object sender, BarcodeScannedEventArgs e)
        {
            try
            {
                this.Attribute.Barcode = Encoding.ASCII.GetString(e.BarcodeData);
                await MessageEvent(string.Format("OnBarCodeScanned: {0}", this.Attribute.Barcode));

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.DeviceEventNotification?.Invoke(this, "BarCodeScanned");
                });
            }
            catch (Exception ex)
            {
                await MessageEvent(string.Format("OnBarCodeScanned: Exception: {0}", ex.Message));
            }
        }

        private void OnHoverPointReceived(object sender, HoverPointReceivedEventArgs e)
        {
            string hoverPointCoords = string.Empty;

            switch (e.Phase)
            {
                case Wacom.Ink.InputPhase.Begin:
                case Wacom.Ink.InputPhase.Move:
                    hoverPointCoords = string.Format("X:{0:0.0}, Y:{1:0.0}", e.X, e.Y);
                    break;
            }

            //var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //        //textBlockHoverCoordinates.Text = hoverPointCoords;
            //    });
        }
        #endregion

        //private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        //{
        //    //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //    //{
        //        switch (e.Status)
        //    {
        //        case DeviceStatus.NotAuthorizedConnectionNotConfirmed:
        //            //await new MessageDialog(AppObjects.GetStringForDeviceStatus(e.Status)).ShowAsync();
        //            //						Frame.Navigate(typeof(ScanAndConnectPage));
        //            break;

        //        default:
        //            //textBlockPrompt.Text = AppObjects.GetStringForDeviceStatus(e.Status);
        //            break;
        //    }
        //    //});
        //}

        private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            IDigitalInkDevice device = sender as IDigitalInkDevice;

            if (device == null)
                return;

//            TextBlock textBlock = null;

            switch (device.TransportProtocol)
            {
                case TransportProtocol.BLE:
//                   textBlock = tbBle;
                    break;

                case TransportProtocol.USB:
//                    textBlock = tbUsb;
                    break;

                case TransportProtocol.BTC:
//                    textBlock = tbBtc;
                    break;
            }

            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    textBlock.Text = AppObjects.GetStringForDeviceStatus(e.Status);
            //});
        }

        private async Task<bool> OnPairingModeEnabledAsync(bool authorizedInThisSession)
        {
            if (!authorizedInThisSession)
                return true;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            //var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            //{
            bool keepUsingDevice = await AppObjects.Instance.ShowPairingModeEnabledDialogAsync();

            tcs.SetResult(keepUsingDevice);

            if (!keepUsingDevice)
            {
                //					Frame.Navigate(typeof(ScanAndConnectPage));
            }
            //});

            return await tcs.Task;
        }

        private void OnDeviceDisconnected(object sender, EventArgs e)
        {
            AppObjects.Instance.Device = null;

            //var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            //{
            //await new MessageDialog($"The device {AppObjects.Instance.DeviceInfo.DeviceName} was disconnected.").ShowAsync();

            //				Frame.Navigate(typeof(ScanAndConnectPage));
            //});
        }

        #endregion
        #region ScanAndConnect

        public ObservableCollection<InkDeviceInfo> DeviceInfos
        {
            get
            {
                return m_deviceInfos;
            }
        }

        public async void StartScanAndConnect()
        {
            await MessageEvent("StartScanAndConnect");

            AppObjects.Instance.DeviceInfo = null;

            if (AppObjects.Instance.Device != null)
            {
                AppObjects.Instance.Device.Close();
                AppObjects.Instance.Device = null;
            }

            StartScanning();
        }

        //       public void StopScanAndConnect(object sender, RoutedEventArgs e)
        public async void StopScanAndConnect()
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            await MessageEvent("StopScanAndConnect");

            if (device != null)
            {
                device.Disconnected -= OnDeviceDisconnected;
                //                device.DeviceStatusChanged -= OnDeviceStatusChanged_ScanAndConnect;
                device.DeviceStatusChanged -= OnDeviceStatusChanged;
                device.BarCodeScanned -= OnBarCodeScanned;
            }

            StopWatchers();

            Application.Current.Suspending -= OnAppSuspending;
            Application.Current.Resuming -= OnAppResuming;

            m_watcherUSB.DeviceAdded -= OnDeviceAdded;
            m_watcherUSB.DeviceRemoved -= OnDeviceRemoved;
            m_watcherUSB.WatcherStopped -= OnUsbWatcherStopped;
        }

        private void StartScanning()
        {
            StartWatchers();
        }

        private void StopScanning()
        {
            StopWatchers();
        }

        private void StartWatchers()
        {
            // Do just a USB
            m_watcherUSB.Start();
        }

        private void StopWatchers()
        {
            // Do just a USB
            m_watcherUSB.Stop();
        }

        private async Task ConnectInkDevice(InkDeviceInfo info)
        {
            //           int index = listView.SelectedIndex;
            //int index = 0;  // assumed to be connected a deviceat default

            //if ((index < 0) || (index >= m_deviceInfos.Count))
            //    return;

            IDigitalInkDevice device = null;
//            m_connectingDeviceInfo = m_deviceInfos[index];
            m_connectingDeviceInfo = info;

            //btnConnect.IsEnabled = false;

            StopScanning();

            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (m_connectingDeviceInfo != null)
            {
                await MessageEvent(string.Format("ConnectInkDevice: Initializing connection with device: {0}", m_connectingDeviceInfo.DeviceName));

                switch (m_connectingDeviceInfo.TransportProtocol)
                {
                    case TransportProtocol.BLE:
                        //tbBle.Text = msg;
                        break;

                    case TransportProtocol.USB:
                        //tbUsb.Text = msg;
                        break;

                    case TransportProtocol.BTC:
                        //tbBtc.Text = msg;
                        break;
                }
            }

            try
            {
                device = await InkDeviceFactory.Instance.CreateDeviceAsync(
                    m_connectingDeviceInfo,
                    AppObjects.Instance.AppId,
                    true,
                    false,
//                    OnDeviceStatusChanged_ScanAndConnect).Result;
                OnDeviceStatusChanged);

            }
            catch (Exception ex)
            {
                await MessageEvent(string.Format($"ConnectInkDevice: Device creation failed: {0}", ex.Message));
//                return;
            }

            if (device == null)
            {
                await MessageEvent($"ConnectInkDevice: InkDeviceFactory: CreateDeviceAsync returns null.");

                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                m_connectingDeviceInfo = null;
                //btnConnect.IsEnabled = true;
                StartScanning();
                return;
            }

            await MessageEvent($"ConnectInkDevice: InkDeviceFactory: CreateDeviceAsync successfully gets the device object");
            AppObjects.Instance.DeviceInfo = m_connectingDeviceInfo;
            AppObjects.Instance.Device = device;
            m_connectingDeviceInfo = null;

            try
            {
                GetDeviceAttributesAsync(device);  // Get device attributes from connected device

                await AppObjects.SerializeDeviceInfoAsync(AppObjects.Instance.DeviceInfo);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.ScanAndConnectCompletedNotification?.Invoke(this, true);
                });
            }
            catch (Exception ex)
            {
                string message = string.Format($"ConnectInkDevice: The final process got an error: {0}", ex.Message);
                await MessageEvent(message);
                throw new Exception(message);
            }

        }

        async void GetDeviceAttributesAsync(IDigitalInkDevice device)
        {
            Attribute.Width = ((uint)await device.GetPropertyAsync("Width", m_cts.Token)).ToString();
            Attribute.Height = ((uint)await device.GetPropertyAsync("Height", m_cts.Token)).ToString();
            Attribute.PointSize = ((uint)await device.GetPropertyAsync("PointSize", m_cts.Token)).ToString();
            Attribute.Name = (string)await device.GetPropertyAsync(SmartPadProperties.DeviceName, m_cts.Token);
            Attribute.ESN = (string)await device.GetPropertyAsync(SmartPadProperties.SerialNumber, m_cts.Token);
            Attribute.Battery = ((int)await device.GetPropertyAsync(SmartPadProperties.BatteryLevel, m_cts.Token)).ToString();
//            Attribute.FirmwareVersion = (string)await device.GetPropertyAsync(SmartPadProperties.FirmwareVersion, m_cts.Token);
            Attribute.TransferMode = "LiveMode";
            Attribute.DeviceType = "PHU-111";
        }

        //private void OnDeviceStatusChanged_ScanAndConnect(object sender, DeviceStatusChangedEventArgs e)
        //{
        //    if (!(sender is IDigitalInkDevice device))
        //        return;

        //    //TextBlock textBlock = null;

        //    switch (device.TransportProtocol)
        //    {
        //        case TransportProtocol.BLE:
        //            //textBlock = tbBle;
        //            break;

        //        case TransportProtocol.USB:
        //            //textBlock = tbUsb;
        //            break;

        //        case TransportProtocol.BTC:
        //            //textBlock = tbBtc;
        //            break;
        //    }

        //    //         var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    //{
        //    //	textBlock.Text = AppObjects.GetStringForDeviceStatus(e.Status);
        //    //});
        //}

        private async void OnUsbEnumerationCompleted(object sender, object e)
        {
            if (m_deviceInfos.Count == 0)
            {
                await MessageEvent("OnUsbEnumerationCompleted: No devices were enumarated.");
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    this.ScanAndConnectCompletedNotification?.Invoke(this, false);
                //});
            }
        }

        private async void OnDeviceAdded(object sender, InkDeviceInfo info)
        {
            await MessageEvent("OnDeviceAdded: Device is added");

            //var ignore = DependencyObject.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    m_deviceInfos.Add(info);
            //});
            await ConnectInkDevice(info);
        }

        private async void OnDeviceRemoved(object sender, InkDeviceInfo info)
        {
            await MessageEvent("OnDeviceRemoved: Device is removed");
            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            RemoveDevice(info);
            //});
        }

        private async void OnUsbWatcherStopped(object sender, object e)
        {
            await MessageEvent("OnUsbWatcherStopped: ");
            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            if ((m_connectingDeviceInfo != null) && (m_connectingDeviceInfo.TransportProtocol == TransportProtocol.USB))
                return;

            //tbUsb.Text = "Not scanning";
            //});
        }

        private void OnAppSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            StopWatchers();
        }

        private void OnAppResuming(object sender, object e)
        {
            if (AppObjects.Instance.DeviceInfo == null)
            {
                StartScanning();
            }
        }

        private void RemoveDevice(InkDeviceInfo info)
        {
            int index = -1;

            for (int i = 0; i < m_deviceInfos.Count; i++)
            {
                if (ReferenceEquals(m_deviceInfos[i], info))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                m_deviceInfos.RemoveAt(index);
            }
        }
        #endregion
    }
}