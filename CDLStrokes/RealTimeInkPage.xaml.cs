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
using Windows.UI.Input.Inking;

namespace WillDevicesSampleApp
{
    public sealed partial class RealTimeInkPage : Page
    {
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader;

        private const float micrometerToDip = 96.0f / 25400.0f;
        private static float maxP = 1.402218f;
        private static float pFactor = 1.0f / (maxP - 1.0f);

        private InkStrokeBuilder m_inkStrokeBuilder = new InkStrokeBuilder();
        private double mScale = 1;

        private CancellationTokenSource m_cts = new CancellationTokenSource();

        private Size m_deviceSize;
        private bool m_addNewStrokeToModel = true;

        public RealTimeInkPage()
        {
            this.InitializeComponent();

            resourceLoader = ResourceLoader.GetForCurrentView();

            Loaded += RealTimeInkPage_Loaded;

            // Default settings of Stroke
            InkDrawingAttributes attributes = new InkDrawingAttributes();
            attributes.Color = Windows.UI.Colors.Black;
            attributes.Size = new Windows.Foundation.Size(1, 1);
            m_inkStrokeBuilder.SetDefaultDrawingAttributes(attributes);

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
            Pbtn_Save.Content = resourceLoader.GetString("IDS_Save");

            //
            inkCanvas.Width = 432;
            inkCanvas.Height = 594;

            IDigitalInkDevice device = AppObjects.Instance.Device;

            if (device == null)
            {
                textBlockPrompt.Text =
                    resourceLoader.GetString("IDS_DeviceNotConnected2");
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
                textBlockPrompt.Text =
                    resourceLoader.GetString("IDS_RealtimeinkNotSupported");
                return;
            }

            textBlockPrompt.Text = AppObjects.GetStringForDeviceStatus(device.DeviceStatus);

            try
            {
                uint deviceWidth = (uint)await device.GetPropertyAsync("Width", m_cts.Token);
                uint deviceHeight = (uint)await device.GetPropertyAsync("Height", m_cts.Token);
                uint ptSize = (uint)await device.GetPropertyAsync("PointSize", m_cts.Token);

                // preparing for getting stroke raw data
                m_deviceSize.Width = deviceWidth;
                m_deviceSize.Height = deviceHeight;
                service.StrokeStarted += Service_StrokeStarted;
                service.StrokeUpdated += Service_StrokeUpdated;
                service.StrokeEnded += Service_StrokeEnded;

                // Calc coordination scale
                double sx = inkCanvas.Width / deviceWidth;
                double sy = inkCanvas.Height / deviceHeight;

                mScale = sx < sy ? sx : sy;

                int cw = (int)(deviceWidth * mScale);
                int ch = (int)(deviceHeight * mScale);

                // InkCanvas size settings for displaying
                inkCanvas.Width = cw;
                inkCanvas.Height = ch;
                inkContainer.Width = cw;
                inkContainer.Height = ch;

                if (!service.IsStarted)
                {
                    await service.StartAsync(false, m_cts.Token);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("RealTimeInkPage_Loaded: Exception: {0}", ex));
            }
        }

        private async void DrawStrokes(StrokeUpdatedEventArgs e)
        {
            if (e.PathPart.DataStride == 3)
            {
                int stride = 3;
                int count = e.PathPart.Data.Count / stride;
                int index = 0;

                InkPoint[] points = new InkPoint[count];

                for (int i = 0; i < count; i++)
                {
                    float x = e.PathPart.Data[index];
                    float y = e.PathPart.Data[index + 1];
                    float p = e.PathPart.Data[index + 2] / maxP;

                    points[i] = new InkPoint(new Windows.Foundation.Point(x * mScale, y * mScale), p);

                    index += stride;
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Make a stroke by array of point
                    InkStroke s = m_inkStrokeBuilder.CreateStrokeFromInkPoints(
                        points, System.Numerics.Matrix3x2.Identity
                        );
                    inkCanvas.InkPresenter.StrokeContainer.AddStroke(s);
                });
            }
        }

        private async void Service_StrokeEnded(object sender, StrokeEndedEventArgs e)
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

            m_addNewStrokeToModel = true;

            await EventMessage(sender, string.Format("Stroke End: {0} : {1} : {2}", x, y, z));

        }

        private async void Service_StrokeUpdated(object sender, StrokeUpdatedEventArgs e)
        {
            DrawStrokes(e);

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

            await EventMessage(sender, string.Format("Stroke Middle: {0} : {1} : {2}", x, y, z));
        }

        private async void Service_StrokeStarted(object sender, StrokeStartedEventArgs e)
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
                await new MessageDialog(
                    string.Format(resourceLoader.GetString("IDS_DeviceDisconnected"), AppObjects.Instance.DeviceInfo.DeviceName)).ShowAsync();

                Frame.Navigate(typeof(ScanAndConnectPage));
            });
        }

        private async void Pbtn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderPicker = new Windows.Storage.Pickers.FolderPicker();

                folderPicker.FileTypeFilter.Add("*");
                Windows.Storage.StorageFolder folder =
                    await folderPicker.PickSingleFolderAsync();

                if (folder == null)
                {
                    return;
                }

                string path = folder.Path.ToString();
                string filename = "data.txt";

                // Create a data stored file; replace if exists.
                Windows.Storage.StorageFile dataFile =
                    await folder.CreateFileAsync(filename,
                        Windows.Storage.CreationCollisionOption.ReplaceExisting);

                string dataString = string.Empty;
                foreach (String item in ListBox_Messages.Items)
                {
                    dataString += item + System.Environment.NewLine;
                }

                if (dataFile != null)
                {
                    await Windows.Storage.FileIO.WriteTextAsync(dataFile, dataString);
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
