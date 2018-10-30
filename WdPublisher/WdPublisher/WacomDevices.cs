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
//using Wacom.UX.ViewModels;
//using Wacom.UX.Gestures;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Storage.Streams;

namespace WillDevicesSampleApp
{
    public class WacomDevices
    {
        private const uint MASK_ID = 0x00FF;
        private const uint MASK_STROKE = 0x0F00;
        private const uint MASK_COMMAND = 0xF000;

        private const float micrometerToDip = 96.0f / 25400.0f;
        private CancellationTokenSource m_cts = new CancellationTokenSource();
        //        private StrokeCollection _strokes = new StrokeCollection();
        //        private double m_scale = 1.0;
        //        private Size m_deviceSize;
        private bool m_addNewStrokeToModel = true;
        private static readonly float maxP = 1.402218f;
        private static readonly float pFactor = 1.0f / (maxP - 1.0f);
        int PointCount;
        int StrokeCount;
        ////        public InkTransfer inkTransfer;
        int m_StrokeOrder = 1; // 1: start, 0: intermediate, 2: end

        public delegate void MessageEventHandler(object sender, string message);
        public delegate void ScanAndConnectCompletedNotificationHandler(object sender, bool result);
        public delegate void StartRealTimeInkCompletedNotificationHandler(object sender, bool result);
        // Properties
        public event MessageEventHandler WacomDevicesMessage;
        public event ScanAndConnectCompletedNotificationHandler ScanAndConnectCompletedNotification;
        public event StartRealTimeInkCompletedNotificationHandler StartRealTimeInkCompletedNotification;

        public float PublisherAttribute;

        #region Realtime Ink Raw Data Collection
        public ObservableCollection<StrokeRawData> StrokeRawDataInfos
        {
            get
            {
                return m_StrokeRawData;
            }
        }
        public ObservableCollection<StrokeRawData> m_StrokeRawData = new ObservableCollection<StrokeRawData>();
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
            public string DeviceType;
            public string TransferMode;

            public DeviceAttributes()
            {

            }

            public string GenerateStrings()
            {
                string s = string.Empty;

                s = Width + "," + Height + "," + PointSize + "," + Name + "," + 
                    ESN + "," + Battery + "," + DeviceType + "," + TransferMode;

                return s;
            }
        }
        public DeviceAttributes Attribute = new DeviceAttributes();
        #endregion

        public WacomDevices()
        {
            //this.InitializeComponent();

            // -> Start()
            //                Loaded += RealTimeInkPage_Loaded;

            //                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += RealTimeInkPage_BackRequested;

            m_watcherUSB = new InkDeviceWatcherUSB();
            m_watcherUSB.DeviceAdded += OnDeviceAdded;
            m_watcherUSB.DeviceRemoved += OnDeviceRemoved;
            m_watcherUSB.WatcherStopped += OnUsbWatcherStopped;
            m_watcherUSB.EnumerationCompleted += OnUsbEnumerationCompleted;

            //Application.Current.Suspending += OnAppSuspending;
            //Application.Current.Resuming += OnAppResuming;

            //SystemNavigationManager.GetForCurrentView().BackRequested += ScanAndConnectPage_BackRequested;
        }

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.WacomDevicesMessage?.Invoke(this, message);
            });
        }

        #region RealTimeInk
        public async Task StartRealTimeInk()
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            if (device == null)
            {
                MessageEvent("StartRealtimeInk: Device not connected.");
                return;
            }

            device.Disconnected += OnDeviceDisconnected;
            device.DeviceStatusChanged += OnDeviceStatusChanged;
            device.PairingModeEnabledCallback = OnPairingModeEnabledAsync;

            IRealTimeInkService service = device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;
            service.HoverPointReceived += OnHoverPointReceived;

            if (service == null)
            {
                MessageEvent("StartRealTimeInk: The Real-time Ink service is not supported on this device");
                return;
            }

            //// Register private events for getting stroke data /////
            service.StrokeStarted += Service_StrokeStarted;
            service.StrokeUpdated += Service_StrokeUpdated;
            service.StrokeEnded += Service_StrokeEnded;
            /////////////////////////////////////////////////////////////

            //textBlockPrompt.Text = AppObjects.GetStringForDeviceStatus(device.DeviceStatus);

            try
            {
//                uint width = (uint)await device.GetPropertyAsync("Width", m_cts.Token);
//                uint height = (uint)await device.GetPropertyAsync("Height", m_cts.Token);
//                uint ptSize = (uint)await device.GetPropertyAsync("PointSize", m_cts.Token);
                //Attribute.Width = ((uint)await device.GetPropertyAsync("Width", m_cts.Token)).ToString();
                //Attribute.Height = ((uint)await device.GetPropertyAsync("Height", m_cts.Token)).ToString();
                //Attribute.PointSize = ((uint)await device.GetPropertyAsync("PointSize", m_cts.Token)).ToString();
                //Attribute.Name = ((uint)await device.GetPropertyAsync(SmartPadProperties.DeviceName, m_cts.Token)).ToString();
                //Attribute.ESN = ((uint)await device.GetPropertyAsync(SmartPadProperties.SerialNumber, m_cts.Token)).ToString();
                //Attribute.Battery = ((uint)await device.GetPropertyAsync(SmartPadProperties.BatteryLevel, m_cts.Token)).ToString();
                //Attribute.TransferMode = "LiveMode";
                //Attribute.DeviceType = "PHU-111";

                //service.Transform = AppObjects.CalculateTransform(width, height, ptSize);

                //float scaleFactor = ptSize * AppObjects.micrometerToDip;

                //InkCanvasDocument document = new InkCanvasDocument();
                //document.Size = new Windows.Foundation.Size(height * scaleFactor, width * scaleFactor);
                //document.InkCanvasLayers.Add(new InkCanvasLayer());

                //inkCanvas.InkCanvasDocument = document;
                //inkCanvas.GesturesManager = new GesturesManager();
                //inkCanvas.StrokeDataProvider = service;

                bool start_flag = false;
                if (!service.IsStarted)
                {
                    await service.StartAsync(false, m_cts.Token);
                    start_flag = true;
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.StartRealTimeInkCompletedNotification?.Invoke(this, start_flag);
                });

            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("StartRealTimeInk: Exception: {0}", ex.Message));
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
                MessageEvent(string.Format("StopRealtimeInk: Exception: {0}", ex.Message));
            }
        }

        #region Stroke event handlers
        private void Service_StrokeEnded(object sender, StrokeEndedEventArgs e)
        {
            m_StrokeOrder = 2;
            AppObjects.Instance.WacomDevice.PublisherAttribute =
                ((uint)AppObjects.Instance.WacomDevice.PublisherAttribute & ~MASK_STROKE) | ((uint)m_StrokeOrder << 8);

            m_addNewStrokeToModel = true;

            var pathPart = e.PathPart;

            if (AppObjects.Instance.SocketClient != null)
 //               AppObjects.Instance.SocketClient.BatchedSends(CreateBuffer(pathPart));
            AppObjects.Instance.SocketClient.SendData(CreateBuffer(pathPart));

        }

        private void Service_StrokeUpdated(object sender, StrokeUpdatedEventArgs e)
        {
            m_StrokeOrder = 0;
            AppObjects.Instance.WacomDevice.PublisherAttribute =
                ((uint)AppObjects.Instance.WacomDevice.PublisherAttribute & ~MASK_STROKE) | ((uint)m_StrokeOrder << 8);

            var pathPart = e.PathPart;

            if (AppObjects.Instance.SocketClient != null)
                //               AppObjects.Instance.SocketClient.BatchedSends(CreateBuffer(pathPart));
                AppObjects.Instance.SocketClient.SendData(CreateBuffer(pathPart));


            //var point = new StylusPoint(x * m_scale, y * m_scale, w);
            if (m_addNewStrokeToModel)
            {
                m_addNewStrokeToModel = false;
                //    var points = new StylusPointCollection { point };
                //    points.Add(point);

                //    var stroke = new WinStroke(points) { DrawingAttributes = m_DrawingAttributes };

                //    Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                //    {
                //        _strokes.Add(stroke);
                //    }));
            }
            //else
            //{
            //    Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //    {
            //        _strokes[_strokes.Count - 1].StylusPoints.Add(point);
            //    }));
            //}

            PointCount++;

            // Add to ListBox
            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            ////Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    m_StrokeRawData.Add(new StrokeRawData(PointCount.ToString(), StrokeCount.ToString(), x.ToString(), y.ToString(), w.ToString()));
            //});

            // data transfer
            //            inkTransfer.Send(pathPart);


        }

        private void Service_StrokeStarted(object sender, StrokeStartedEventArgs e)
        {
            m_StrokeOrder = 1;
            AppObjects.Instance.WacomDevice.PublisherAttribute =
                ((uint)AppObjects.Instance.WacomDevice.PublisherAttribute & ~MASK_STROKE) | ((uint)m_StrokeOrder << 8);

            m_addNewStrokeToModel = true;
            StrokeCount++;

            if (AppObjects.Instance.SocketClient != null)
                //               AppObjects.Instance.SocketClient.BatchedSends(CreateBuffer(pathPart));
                AppObjects.Instance.SocketClient.SendData(CreateBuffer(null));
        }

        private IBuffer CreateBuffer(Wacom.Ink.Path pathPart)
        {
            IBuffer buffer;

            //Data is stored XYW
            float f = AppObjects.Instance.WacomDevice.PublisherAttribute;
            float x = -1;
            float y = -1;
            float w = -1;

            if (pathPart == null)  // StartStroke
            {
                x = y = w = 0;
            }
            else   // others
            {
                var data = pathPart.Data.GetEnumerator();

                if (data.MoveNext())
                {
                    x = data.Current;
                }

                if (data.MoveNext())
                {
                    y = data.Current;
                }

                if (data.MoveNext())
                {
                    //Clamp to 0.0 -> 1.0
                    w = Math.Max(0.0f, Math.Min(1.0f, (data.Current - 1.0f) * pFactor));
                }
            }

            int num_bytes = sizeof(float);
            byte[] ByteArray = new byte[num_bytes * 4];
            int offset = 0;
            Array.Copy(BitConverter.GetBytes(f), 0, ByteArray, offset, num_bytes);
            Array.Copy(BitConverter.GetBytes(x), 0, ByteArray, offset += num_bytes, num_bytes);
            Array.Copy(BitConverter.GetBytes(y), 0, ByteArray, offset += num_bytes, num_bytes);
            Array.Copy(BitConverter.GetBytes(w), 0, ByteArray, offset += num_bytes, num_bytes);
            using (DataWriter writer = new DataWriter())
            {
                writer.WriteBytes(ByteArray);
                buffer = writer.DetachBuffer();
            }

            return buffer;
        }
        #endregion

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

        private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            //var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            //{
            switch (e.Status)
            {
                case DeviceStatus.NotAuthorizedConnectionNotConfirmed:
                    //await new MessageDialog(AppObjects.GetStringForDeviceStatus(e.Status)).ShowAsync();
                    //						Frame.Navigate(typeof(ScanAndConnectPage));
                    break;

                default:
                    //textBlockPrompt.Text = AppObjects.GetStringForDeviceStatus(e.Status);
                    break;
            }
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
        InkDeviceWatcherUSB m_watcherUSB;
        InkDeviceInfo m_connectingDeviceInfo;
        ObservableCollection<InkDeviceInfo> m_deviceInfos = new ObservableCollection<InkDeviceInfo>();

        public ObservableCollection<InkDeviceInfo> DeviceInfos
        {
            get
            {
                return m_deviceInfos;
            }
        }

        //		private void ScanAndConnectPage_Loaded(object sender, RoutedEventArgs e)
        //        public void StartScanAndConnect(object sender, RoutedEventArgs e)
        public void StartScanAndConnect()
        {
            AppObjects.Instance.DeviceInfo = null;

            if (AppObjects.Instance.Device != null)
            {
                AppObjects.Instance.Device.Close();
                AppObjects.Instance.Device = null;
            }

            StartScanning();
        }

        //		private void ScanAndConnectPage_Unloaded(object sender, RoutedEventArgs e)
        public void StopScanAndConnect(object sender, RoutedEventArgs e)
        {
            MessageEvent("StopScanAndConnect");

            if (AppObjects.Instance.Device != null)
            {
                AppObjects.Instance.Device.DeviceStatusChanged -= OnDeviceStatusChanged_ScanAndConnect;
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

        //		private async void OnButtonConnectClick(object sender, RoutedEventArgs e)
        private async Task ConnectInkDevice()
        {
            //           int index = listView.SelectedIndex;
            int index = 0;  // Auto Pilot

            if ((index < 0) || (index >= m_deviceInfos.Count))
                return;

            IDigitalInkDevice device = null;
            m_connectingDeviceInfo = m_deviceInfos[index];

            //btnConnect.IsEnabled = false;

            //           StopScanning();

            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (m_connectingDeviceInfo != null)
            {
                MessageEvent(string.Format("ConnectInkDevice(): Initializing connection with device: {0}", m_connectingDeviceInfo.DeviceName));

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
                device = InkDeviceFactory.Instance.CreateDeviceAsync(
                    m_connectingDeviceInfo,
                    AppObjects.Instance.AppId,
                    true,
                    false,
                    OnDeviceStatusChanged_ScanAndConnect).Result;

            }
            catch (Exception ex)
            {
                MessageEvent(string.Format($"ConnectInkDevice: Device creation failed: {0}", ex.Message));
                //                await new MessageDialog(message).ShowAsync();
                return;
            }

            if (device == null)
            {
                MessageEvent($"ConnectInkDevice: InkDeviceFactory: CreateDeviceAsync returns null.");

                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                m_connectingDeviceInfo = null;
                //btnConnect.IsEnabled = true;
                StartScanning();
                return;
            }

            MessageEvent($"ConnectInkDevice: InkDeviceFactory: CreateDeviceAsync successfully gets the device object");
            AppObjects.Instance.DeviceInfo = m_connectingDeviceInfo;
            AppObjects.Instance.Device = device;
            m_connectingDeviceInfo = null;
            GetDeviceAttributesAsync(device);  // Get device attributes from connected device

            await AppObjects.SerializeDeviceInfoAsync(AppObjects.Instance.DeviceInfo);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.ScanAndConnectCompletedNotification?.Invoke(this, true);
            });

        }

        async void GetDeviceAttributesAsync(IDigitalInkDevice device)
        {
            Attribute.Width = ((uint)await device.GetPropertyAsync("Width", m_cts.Token)).ToString();
            Attribute.Height = ((uint)await device.GetPropertyAsync("Height", m_cts.Token)).ToString();
            Attribute.PointSize = ((uint)await device.GetPropertyAsync("PointSize", m_cts.Token)).ToString();
            Attribute.Name = ((uint)await device.GetPropertyAsync(SmartPadProperties.DeviceName, m_cts.Token)).ToString();
            Attribute.ESN = ((uint)await device.GetPropertyAsync(SmartPadProperties.SerialNumber, m_cts.Token)).ToString();
            Attribute.Battery = ((uint)await device.GetPropertyAsync(SmartPadProperties.BatteryLevel, m_cts.Token)).ToString();
            Attribute.TransferMode = "LiveMode";
            Attribute.DeviceType = "PHU-111";
        }

        private void OnDeviceStatusChanged_ScanAndConnect(object sender, DeviceStatusChangedEventArgs e)
        {
            if (!(sender is IDigitalInkDevice device))
                return;

            //TextBlock textBlock = null;

            switch (device.TransportProtocol)
            {
                case TransportProtocol.BLE:
                    //textBlock = tbBle;
                    break;

                case TransportProtocol.USB:
                    //textBlock = tbUsb;
                    break;

                case TransportProtocol.BTC:
                    //textBlock = tbBtc;
                    break;
            }

            //         var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //	textBlock.Text = AppObjects.GetStringForDeviceStatus(e.Status);
            //});
        }

        private async void OnUsbEnumerationCompleted(object sender, object e)
        {
            if(m_deviceInfos.Count == 0)
            {
                MessageEvent("OnUsbEnumerationCompleted: No devices were enumarated.");
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.ScanAndConnectCompletedNotification?.Invoke(this, false);
                });
            }
        }

        private async void OnDeviceAdded(object sender, InkDeviceInfo info)
        {
            MessageEvent("OnDeviceAdded: Device is added");

            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            m_deviceInfos.Add(info);

            await ConnectInkDevice();
        }

        private void OnDeviceRemoved(object sender, InkDeviceInfo info)
        {
            MessageEvent("OnDeviceRemoved: Device is removed");
            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            RemoveDevice(info);
            //});
        }

        private void OnUsbWatcherStopped(object sender, object e)
        {
            MessageEvent("OnUsbWatcherStopped: ");
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