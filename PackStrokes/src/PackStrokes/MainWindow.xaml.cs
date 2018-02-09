using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Wacom.Ink;

using Wacom.Devices;
using Wacom.Devices.Enumeration;

using Microsoft.Win32;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows.Ink;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace PackStrokes
{
    using InkPath = Wacom.Ink.Path; // resolve ref to System.IO.Path
    using WinStroke = System.Windows.Ink.Stroke;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        string csv = string.Empty;
        public StrokeAggregation sa;

        InkDeviceWatcherUSB m_watcherUSB;
        InkDeviceInfo m_connectingDeviceInfo;
        public ObservableCollection<InkDeviceInfo> m_deviceInfos = new ObservableCollection<InkDeviceInfo>();
        public ObservableCollection<InkDeviceInfo> DeviceInfos
        {
            get
            {
                return m_deviceInfos;
            }
        }

        IDigitalInkDevice device;

        //---- Stroke collection used for real-time rendering 
        private CancellationTokenSource m_cts = new CancellationTokenSource();

        private StrokeCollection _strokes = new StrokeCollection();
        private double m_scale = 1.0;
        private Size m_deviceSize;
        private bool m_addNewStrokeToModel = true;
        private static float maxP = 1.402218f;
        private static float pFactor = 1.0f / (maxP - 1.0f);
        public event PropertyChangedEventHandler PropertyChanged;
        public StrokeCollection Strokes //Used as databinding into XAML
        {
            get
            {
                return _strokes;
            }
        }
        // -----
        int PointCount;
        int StrokeCount;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            sa = new StrokeAggregation();
            device = null;
//            service = null;

            m_watcherUSB = new InkDeviceWatcherUSB();  // Only for USB connection
            m_watcherUSB.DeviceAdded += OnDeviceAdded;
            m_watcherUSB.DeviceRemoved += OnDeviceRemoved;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = @"PackStroke";
            PbtnStart.Content = @"Start";
            PbtnFileOpen.Content = @"File...";
            PbtnScanDevices.Content = @"Find Devices";
            PbtnConnect.Content = @"Connect";
            PbtnClear.Content = @"Clear";
            PbtnRealTimeInk.Content = @"RealTimeInk";
            PbtnStop.Content = @"Stop";
            PbtnFileTransfer.Content = @"FileTransfer";

            tbBle.Content = @"";
            tbUsb.Content = @"";

            PbtnConnect.IsEnabled = false;
            PbtnRealTimeInk.IsEnabled = false;
            PbtnFileTransfer.IsEnabled = false;
            PbtnStop.IsEnabled = false;
            PbtnClear.IsEnabled = false;
        }

        #region UI Control Handlers
        private void PbtnStart_Click(object sender, RoutedEventArgs e)
        {
            // Define Regions
            sa.CreateRegion(10, 10, 110, 60);
            sa.CreateRegion(200, 10, 300, 60);

            // Simurate Input Strokes
            List<float> data;
            uint stride;
            InkPath p;

            // 1st stroke
            data = new List<float> { 20, 21, 1, 20, 22, 1 };
            stride = 3;
            p = new InkPath(data, stride, PathFormat.XYA);
            sa.CreateStroke(p);

            // 2nd stroke
            data = new List<float> { 21, 30, 1, 25, 30, 1, 24, 40, 1 };
            stride = 3;
            p = new InkPath(data, stride, PathFormat.XYA);
            sa.CreateStroke(p);

            // 3rd stroke
            data = new List<float> { 210, 13, 1, 212, 30, 1, 220, 23, 1 };
            stride = 3;
            p = new InkPath(data, stride, PathFormat.XYA);
            sa.CreateStroke(p);


            // ストロークをリージョンに当てはめる
            // 後から当てはめるか、
            // リアルタイムに当てはめるか sc.Regionがあるかどうか
            sa.StrokesToRegion();
        }

        private void PbtnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    FileName = string.Empty,
                    //           ofd.InitialDirectory = @"C:\";
                    InitialDirectory = string.Empty,  // current directory
                    Filter = "PDF File (*.pdf)|*.pdf|All Files(*.*)|*.*",
                    FilterIndex = 2,
                    //set the title
                    Title = "Please set the File",
                    RestoreDirectory = true,
                    CheckFileExists = true,
                    CheckPathExists = true
                };

                if (ofd.ShowDialog() == true)
                {
                    string path = string.Empty;
                    path = ofd.FileName;    // full path + filename + extension
                                            //               textBoxReadFile.Text = path;
                    InkDocument.ReadBaxter(path, sa);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void PbtnScanDevices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // init
                AppObjects.Instance.DeviceInfo = null;

                if (AppObjects.Instance.Device != null)
                {
                    AppObjects.Instance.Device.Close();
                    AppObjects.Instance.Device = null;
                }

                StartScanning();
  //                            KeepAlive = true;


                // CDL-Classic only supports the USB connection
                if (m_watcherUSB.Status != DeviceWatcherStatus.Started &&
                    m_watcherUSB.Status != DeviceWatcherStatus.Stopping &&
                    m_watcherUSB.Status != DeviceWatcherStatus.EnumerationCompleted)
                {
                    m_watcherUSB.Start();
                    BtnUsbScanSetScanningAndDisabled();
                    TextBoxUsbSetText();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex);
            }
        }

        private async void PbtnConnect_Click(object sender, RoutedEventArgs e)
        {
            int index = ListViewDevices.SelectedIndex;

            if ((index < 0) || (index >= m_deviceInfos.Count))
                return;

            IDigitalInkDevice device = null;
            m_connectingDeviceInfo = m_deviceInfos[index];

            PbtnConnect.IsEnabled = false;

            StopScanning();

            try
            {
                device = await InkDeviceFactory.Instance.CreateDeviceAsync(m_connectingDeviceInfo,
                    AppObjects.Instance.AppId, true, false, OnDeviceStatusChanged);


            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder($"Device creation failed:\n{ex.Message}");
                string indent = "  ";
                for (Exception inner = ex.InnerException; inner != null; inner = inner.InnerException)
                {
                    sb.Append($"\n{indent}{inner.Message}");
                    indent = indent + "  ";
                }

                MessageBox.Show(sb.ToString());
            }

            if (device == null)
            {
                m_connectingDeviceInfo = null;
                PbtnConnect.IsEnabled = true;
                StartScanning();
                return;
            }

            AppObjects.Instance.DeviceInfo = m_connectingDeviceInfo;
            AppObjects.Instance.Device = device;
            m_connectingDeviceInfo = null;

            await AppObjects.SerializeDeviceInfoAsync(AppObjects.Instance.DeviceInfo);

            //if (NavigationService.CanGoBack)
            //{
            //    NavigationService.GoBack();
            //}

            PbtnRealTimeInk.IsEnabled = !PbtnRealTimeInk.IsEnabled;
            PbtnFileTransfer.IsEnabled = !PbtnFileTransfer.IsEnabled;

            DeviceStatus ds = device.DeviceStatus;
        }

        private async void PbtnRealTimeInk_Click(object sender, RoutedEventArgs e)
        {
            device = AppObjects.Instance.Device;

            device.Disconnected += OnDeviceDisconnected;

            //            NavigationService.Navigating += NavigationService_Navigating;
            //            NavigationService.Navigated += NavigationService_Navigated;

            IRealTimeInkService service = device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;
            service.NewPage += OnNewPage; //Clear page on new page or layer
            service.NewLayer += OnNewPage;
            CanvasMain.DataContext = this;

            PointCount = 0;
            StrokeCount = 0;

            try
            {
                uint width = (uint)await device.GetPropertyAsync("Width", m_cts.Token);
                uint height = (uint)await device.GetPropertyAsync("Height", m_cts.Token);
                uint ptSize = (uint)await device.GetPropertyAsync("PointSize", m_cts.Token);

                m_deviceSize.Width = width;
                m_deviceSize.Height = height;

                SetCanvasScaling();

                service.StrokeStarted += Service_StrokeStarted;
                service.StrokeUpdated += Service_StrokeUpdated;
                service.StrokeEnded += Service_StrokeEnded;

                if (!service.IsStarted)
                {
                    await service.StartAsync(true, m_cts.Token);
                }

                PbtnStop.IsEnabled = !PbtnStop.IsEnabled;
                PbtnClear.IsEnabled = !PbtnClear.IsEnabled;
                PbtnRealTimeInk.IsEnabled = !PbtnRealTimeInk.IsEnabled;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void PbtnFileTransfer_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PbtnStop_Click(object sender, RoutedEventArgs e)
        {
            IRealTimeInkService service = AppObjects.Instance.Device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;
            if ((service != null) && service.IsStarted)
            {
                await service.StopAsync(m_cts.Token);
            }


            device.Close();

            PbtnRealTimeInk.IsEnabled = !PbtnRealTimeInk.IsEnabled;
            PbtnFileTransfer.IsEnabled = !PbtnFileTransfer.IsEnabled;

            PointCount = 0;
            StrokeCount = 0;
        }

        private void PbtnClear_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Device event callbacks
        private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            var ignore = Task.Run(() =>
            {
                tbBle.Content = AppObjects.GetStringForDeviceStatus(e.Status); // FIX: make a switch on the transport protocol to switch the message for each text boxF
            });

        }

        private void OnDeviceAdded(object sender, InkDeviceInfo info)
        {
            try
            {
                //var ignore = Task.Run(() =>
                //{
                    m_deviceInfos.Add(info);

                //});

                PbtnConnect.IsEnabled = !PbtnConnect.IsEnabled;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void OnDeviceRemoved(object sender, InkDeviceInfo info)
        {
            //var ignore = Task.Run( () =>
            //{
            RemoveDevice(info);
            //});
        }

        private void RemoveDevice(InkDeviceInfo info)
        {
            int index = -1;

            for (int i = 0; i < m_deviceInfos.Count; i++)
            {
                if (m_deviceInfos[i].DeviceId == info.DeviceId)
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

        #region Realtime Ink manupilation
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
            public string FormattedStrings { get; }
            public StrokeRawData(string PointCount, string StrokeCount, string x, string y, string w)
            {
                this.X = x;
                this.Y = y;
                this.W = w;
                this.FormattedStrings = string.Format("[{0}][{1}] {2}",
                    PointCount, StrokeCount, x + '\t' + y + '\t' + w);
            }
        }

        private void ListBoxPath_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            (ListBoxPath.ItemsSource as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ListBoxPath_CollectionChanged);
        }
        void ListBoxPath_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ListBoxPath.ScrollIntoView(this.ListBoxPath.Items[this.ListBoxPath.Items.Count - 1]);
        }

        private void Service_StrokeEnded(object sender, StrokeEndedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                var pathPart = e.PathPart;
                var data = pathPart.Data.GetEnumerator();

                //Data is stored XYW
                float x = -1;
                float y = -1;
                float w = -1;

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

                var point = new StylusPoint(x * m_scale, y * m_scale, w);
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    _strokes[_strokes.Count - 1].StylusPoints.Add(point);
 //                   NotifyPropertyChanged("Strokes");
                }));

                m_addNewStrokeToModel = true;


                // Add to ListBox
                m_StrokeRawData.Add(new StrokeRawData(PointCount.ToString(), StrokeCount.ToString(),x.ToString(), y.ToString(), w.ToString()));

            }));

        }

        private void Service_StrokeUpdated(object sender, StrokeUpdatedEventArgs e)
        {
            var pathPart = e.PathPart;
            var data = pathPart.Data.GetEnumerator();

            //Data is stored XYW
            float x = -1;
            float y = -1;
            float w = -1;

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

            var point = new StylusPoint(x * m_scale, y * m_scale, w);
            if (m_addNewStrokeToModel)
            {
                m_addNewStrokeToModel = false;
                var points = new StylusPointCollection{ point };
                points.Add(point);

                var stroke = new WinStroke(points){ DrawingAttributes = m_DrawingAttributes};

                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    _strokes.Add(stroke);
                }));
            }
            else
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    _strokes[_strokes.Count - 1].StylusPoints.Add(point);
                }));
            }

            PointCount++;

            // Add to ListBox
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                m_StrokeRawData.Add(new StrokeRawData(PointCount.ToString(), StrokeCount.ToString(), x.ToString(), y.ToString(), w.ToString()));
            }));

        }

        private void Service_StrokeStarted(object sender, StrokeStartedEventArgs e)
        {
            m_addNewStrokeToModel = true;
            StrokeCount++;
        }

        private void OnNewPage(object sender, EventArgs e)
        {
            var ignore = Task.Run(() =>
            {
                _strokes.Clear();

            });
        }

        private void OnDeviceDisconnected(object sender, EventArgs e)
        {
            AppObjects.Instance.Device = null;

            MessageBox.Show($"The device {AppObjects.Instance.DeviceInfo.DeviceName} was disconnected.");

 //           NavigationService.Navigate(new ScanAndConnectPage());
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetCanvasScaling();
        }

        private void SetCanvasScaling()
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            if (device != null)
            {
                double sx = CanvasMain.ActualWidth / m_deviceSize.Width;
                double sy = CanvasMain.ActualHeight / m_deviceSize.Height;
                m_scale = Math.Min(sx, sy);
            }
        }
#endregion

        private void StartScanning()
        {
            StartWatchers();

            BtnUsbScanSetScanningAndDisabled();
            //TextBoxBleSetText();
            //TextBoxUsbSetText();
        }

        private void StopScanning()
        {
            StopWatchers();

            //BtnUsbScanSetScanAndDisabled();
            //TextBoxBleSetEmpty();
            //TextBoxUsbSetEmpty();
        }

        private void StartWatchers()
        {
            m_watcherUSB.Start();
        }

        private void StopWatchers()
        {
            m_watcherUSB.Stop();
        }

        private void BtnUsbScanSetScanningAndDisabled()
        {
            //btnUsbScan.Content = "Scanning";
            //btnUsbScan.IsEnabled = false;
        }

        private void BtnUsbScanSetScanAndDisabled()
        {
            PbtnScanDevices.Content = "Scan";
            PbtnScanDevices.IsEnabled = false;
        }

        private void TextBoxBleSetText()
        {
            tbUsb.Content = "Connect the device to a USB port and turn it on.";
        }

        private void TextBoxBleSetEmpty()
        {
            tbBle.Content = string.Empty;
        }

        private void TextBoxUsbSetText()
        {
            tbUsb.Content = "Connect the device to a USB port and turn it on.";
        }

        private void TextBoxUsbSetEmpty()
        {
            tbUsb.Content = string.Empty;
        }

    }
}
