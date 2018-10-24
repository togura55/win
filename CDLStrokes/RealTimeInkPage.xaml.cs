using System;
using System.Diagnostics;
using System.Threading;
using Wacom.Devices;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Wacom.UX.ViewModels;
using Wacom.UX.Gestures;
using Windows.Foundation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;

namespace WillDevicesSampleApp
{
    public sealed partial class RealTimeInkPage : Page
    {
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader;

        private const float micrometerToDip = 96.0f / 25400.0f;
        private static float maxP = 1.402218f;
        private static float pFactor = 1.0f / (maxP - 1.0f);

        private CancellationTokenSource m_cts = new CancellationTokenSource();

        private double m_scale = 1.0;
        private Size m_deviceSize;
        private bool m_addNewStrokeToModel = true;

        public RealTimeInkPage()
        {
            this.InitializeComponent();

            resourceLoader = ResourceLoader.GetForCurrentView();

            Loaded += RealTimeInkPage_Loaded;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += RealTimeInkPage_BackRequested;
        }

        private async Task EventMessage(object sender, string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ListBox_Messages.Items.Add(message);
            });
        }

        private async void RealTimeInkPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppObjects.Instance.Device != null)
            {
                IRealTimeInkService service = AppObjects.Instance.Device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;

                if ((service != null) && service.IsStarted)
                {
                    await service.StopAsync(m_cts.Token);
                }
            }

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested -= RealTimeInkPage_BackRequested;
        }

//        inkCanvas.DataContext = this;
        private async void RealTimeInkPage_Loaded(object sender, RoutedEventArgs e)
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            //           this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");

            if (device == null)
            {
                textBlockPrompt.Text = "Device not connected";
                return;
            }

            device.Disconnected += OnDeviceDisconnected;
            device.DeviceStatusChanged += OnDeviceStatusChanged;
            device.PairingModeEnabledCallback = OnPairingModeEnabledAsync;

            IRealTimeInkService service = device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;
            service.NewPage += OnNewPage;
            service.HoverPointReceived += OnHoverPointReceived;

            if (service == null)
            {
                textBlockPrompt.Text = "The Real-time Ink service is not supported on this device";
                return;
            }

            textBlockPrompt.Text = AppObjects.GetStringForDeviceStatus(device.DeviceStatus);

            try
            {
                uint width = (uint)await device.GetPropertyAsync("Width", m_cts.Token);
                uint height = (uint)await device.GetPropertyAsync("Height", m_cts.Token);
                uint ptSize = (uint)await device.GetPropertyAsync("PointSize", m_cts.Token);

                service.Transform = AppObjects.CalculateTransform(width, height, ptSize);

                float scaleFactor = ptSize * AppObjects.micrometerToDip;

                InkCanvasDocument document = new InkCanvasDocument();
//                document.Size = new Windows.Foundation.Size(height * scaleFactor, width * scaleFactor);
                document.Size = new Windows.Foundation.Size(width * scaleFactor, height * scaleFactor);
                document.InkCanvasLayers.Add(new InkCanvasLayer());

                inkCanvas.InkCanvasDocument = document;
                inkCanvas.GesturesManager = new GesturesManager();
                inkCanvas.StrokeDataProvider = service;

                // get raw data
                m_deviceSize.Width = width;
                m_deviceSize.Height = height;
                SetCanvasScaling();
                service.StrokeStarted += Service_StrokeStarted;
                service.StrokeUpdated += Service_StrokeUpdated;
                service.StrokeEnded += Service_StrokeEnded;
                // -----

                if (!service.IsStarted)
                {
                    await service.StartAsync(false, m_cts.Token);
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetCanvasScaling()
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            if (device != null)
            {
                double sx = inkCanvas.ActualWidth / m_deviceSize.Width;
                double sy = inkCanvas.ActualHeight / m_deviceSize.Height;
                m_scale = Math.Min(sx, sy);
            }
        }

        private async void Service_StrokeEnded(object sender, StrokeEndedEventArgs e)
        {
            //Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //{
            var pathPart = e.PathPart;
            var data = pathPart.Data.GetEnumerator();

            //Data is stored XYW
            float x = -1;
            float y = -1;
            float w = -1;
            float z = -1;

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
                z = data.Current;
                //Clamp to 0.0 -> 1.0
                w = Math.Max(0.0f, Math.Min(1.0f, (data.Current - 1.0f) * pFactor));
            }

            //var point = new System.Windows.Input.StylusPoint(x * m_scale, y * m_scale, w);
            //Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    _strokes[_strokes.Count - 1].StylusPoints.Add(point);
            //    NotifyPropertyChanged("Strokes");
            //}));

            m_addNewStrokeToModel = true;

            await EventMessage(sender, string.Format("Stroke End: {0} : {1} : {2}", x, y, z));

            //}));
        }

        private async void Service_StrokeUpdated(object sender, StrokeUpdatedEventArgs e)
        {
            var pathPart = e.PathPart;
            var data = pathPart.Data.GetEnumerator();

            //Data is stored XYW
            float x = -1;
            float y = -1;
            float w = -1;
            float z = -1;

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
                z = data.Current;
                //Clamp to 0.0 -> 1.0
                w = Math.Max(0.0f, Math.Min(1.0f, (data.Current - 1.0f) * pFactor));
            }

            //   var point = new System.Windows.Input.StylusPoint(x * m_scale, y * m_scale, w);
            //   if (m_addNewStrokeToModel)
            //   {
            //       m_addNewStrokeToModel = false;
            //       var points = new System.Windows.Input.StylusPointCollection();
            //       points.Add(point);

            //       var stroke = new Stroke(points);
            //       stroke.DrawingAttributes = m_DrawingAttributes;

            //Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //       {
            //           _strokes.Add(stroke);
            //       }));
            //   }
            //   else
            //   {
            //       Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //       {
            //           _strokes[_strokes.Count - 1].StylusPoints.Add(point);
            //       }));
            //   }

            await EventMessage(sender, string.Format("Stroke Middle: {0} : {1} : {2}", x,y,z));
        }

        private async void  Service_StrokeStarted(object sender, StrokeStartedEventArgs e)
        {
            m_addNewStrokeToModel = true;
            await EventMessage(sender, "Stroke Started");
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

            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                textBlockHoverCoordinates.Text = hoverPointCoords;
            });
        }

        private void OnNewPage(object sender, EventArgs e)
        {
            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //inkCanvas.Clear();
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            IDigitalInkDevice device = AppObjects.Instance.Device;

            if (device != null)
            {
                device.PairingModeEnabledCallback = null;
                device.DeviceStatusChanged -= OnDeviceStatusChanged;
                device.Disconnected -= OnDeviceDisconnected;

                IRealTimeInkService service = device.GetService(InkDeviceService.RealTimeInk) as IRealTimeInkService;

                if (service != null)
                {
                    service.NewPage -= OnNewPage;
                    service.HoverPointReceived -= OnHoverPointReceived;
                }
            }

            m_cts.Cancel();
        }

        private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                switch (e.Status)
                {
                    case DeviceStatus.NotAuthorizedConnectionNotConfirmed:
                        await new MessageDialog(AppObjects.GetStringForDeviceStatus(e.Status)).ShowAsync();
                        Frame.Navigate(typeof(ScanAndConnectPage));
                        break;

                    default:
                        textBlockPrompt.Text = AppObjects.GetStringForDeviceStatus(e.Status);
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
                    Frame.Navigate(typeof(ScanAndConnectPage));
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

                Frame.Navigate(typeof(ScanAndConnectPage));
            });
        }

        private void Pbtn_Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
