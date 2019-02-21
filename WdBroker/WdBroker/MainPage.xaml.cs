using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.ApplicationModel.Resources;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;

namespace WdBroker
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //            List<DrawPoint> DrawPointList;
        int Count;
        int MaxCount;
        List<InkCanvas> CanvasStrokesList;
        List<Border> BorderList;
        //InkStrokeBuilder inkStrokeBuilder;
        //List<InkStrokeBuilder> InkStrokeBuilderList;

        static string PortNumberString = "1337";
        static string HostNameString = "192.168.0.7";
        static bool fStart = true;
        ResourceLoader resourceLoader;

        public class MainViewModel
        {
            public ObservableCollection<string> HostNameCollection { get; set; }

            public MainViewModel()
            {
                HostNameCollection = new ObservableCollection<string>();
            }
        }

        private InkStrokeBuilder m_inkStrokeBuilder = new InkStrokeBuilder();
        private double mScale = 1;
        private static float maxP = 1.402218f;

        private const uint MASK_COMMAND = 0xF000;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            App.AppMessage += ReceiveMessage;
            App.Socket.SocketMessage += ReceiveMessage;
            App.Broker.AppConnectPublisher += ReceiveAppConnectPublisher;
            App.Broker.BrokerMessage += ReceiveMessage;
            App.Broker.AppDrawing += ReceiveDrawing;  // for drawing

            RestoreSettings();

            MainViewModel viewModel = new MainViewModel();
            this.DataContext = viewModel;

            {
                int count = 0;
                int index = 0;
                foreach (Windows.Networking.HostName host in App.HostNames)
                {
                    viewModel.HostNameCollection.Add(host.ToString());
                    if (HostNameString == host.ToString())
                        index = count;
                    count++;
                }
                // update UI
                this.Combo_HostNames.SelectedIndex = index;
            }

            CB_ShowStrokeRawData.IsChecked = App.ShowStrokeRawData;

            var versionInfo = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = string.Format(
                               "{0}.{1}.{2}.{3}",
                               versionInfo.Major, versionInfo.Minor,
                               versionInfo.Build, versionInfo.Revision);
            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = version;

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            //InkDrawingAttributes attributes = new InkDrawingAttributes();
            //attributes.Color = Windows.UI.Colors.Red; //UIColors[index]; //
            //attributes.Size = new Size(2, 2);          // ペンのサイズ
            //attributes.IgnorePressure = false;          // ペンの圧力を使用するかどうか
            //attributes.FitToCurve = false;
            //m_inkStrokeBuilder.SetDefaultDrawingAttributes(attributes);

            //            DrawPointList = new List<DrawPoint>();
            Count = 0;
            MaxCount = 6;
            //            InkStrokeBuilderList = new List<InkStrokeBuilder>();

            CanvasStrokesList = new List<InkCanvas> {
                Canvas_Strokes_1,
                Canvas_Strokes_2,
                Canvas_Strokes_3,
                Canvas_Strokes_4,
                Canvas_Strokes_5,
                Canvas_Strokes_6
            };
            BorderList = new List<Border>
            {
                Border_1,
                Border_2,
                Border_3,
                Border_4,
                Border_5,
                Border_6
            };
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            resourceLoader = ResourceLoader.GetForCurrentView();
            this.TextBlock_HostName.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Start.Content = resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop");
            this.TextBox_PortNumberValue.Text = PortNumberString;
            this.Pbtn_Clearlog.Content = resourceLoader.GetString("IDC_Clearlog");
            this.CB_ShowStrokeRawData.Content = resourceLoader.GetString("IDC_ShowStrokeRawData");

            this.TextBox_FixedHostNameValue.Visibility = Visibility.Collapsed;
            this.TextBlock_HostNameValue.Visibility = Visibility.Collapsed;

            // ToDo: get dynamic from Publisher
            double deviceHeight = 29700;    // ToDo: get from Publishers
            double deviceWidth = 21600;

            // preparing for getting stroke raw data
            //m_deviceSize.Width = deviceWidth;
            //m_deviceSize.Height = deviceHeight;

            // Calc coordination scale
            double sx = Canvas_Strokes.Width / deviceWidth;
            double sy = Canvas_Strokes.Height / deviceHeight;

            mScale = sx < sy ? sx : sy;

            int cw = (int)(deviceWidth * mScale);
            int ch = (int)(deviceHeight * mScale);

            // InkCanvas size settings for displaying
            Canvas_Strokes.Width = cw;
            Canvas_Strokes.Height = ch;

            foreach (Border b in BorderList)
            {
                b.Visibility = Visibility.Collapsed;
            }
            // using for debug
            Border_debug.Visibility = Visibility.Collapsed;
        }

        private void GetUiState()
        {
            PortNumberString = this.TextBox_PortNumberValue.Text;
            HostNameString = (string)this.Combo_HostNames.SelectedValue;
            App.ShowStrokeRawData = (bool)this.CB_ShowStrokeRawData.IsChecked;
        }

        // Message event handler sent by SocketServer object
        private void ReceiveMessage(object sender, string message)
        {
            ListBox_Message.Items.Add(message);
            ListBox_Message.ScrollIntoView(message);    // scroll to bottom
        }

        #region UI Control handlers 
        private async void Pbtn_Start_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Message.Items.Add(
                string.Format("{0} button was clicked.", resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop")));

            GetUiState();
            App.ServerHostName = new Windows.Networking.HostName(HostNameString); // for debug

            try
            {
                if (fStart)
                {
                    await App.Broker.Start(App.ServerHostName, PortNumberString);
                }
                else
                {
                    App.Broker.Stop();
                }

                fStart = fStart ? false : true;   // toggle if success
                Pbtn_Start.Content = resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop");
            }
            catch (Exception ex)
            {
                ListBox_Message.Items.Add(ex.Message);
            }
        }

        private void Pbtn_Clearlog_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Message.Items.Clear();

            // ToDo Clear by Windows InkCanvas
            //            CanvasClear(this.Canvas_Strokes);
        }

        private void ShowStrokeRawData_Click(object sender, RoutedEventArgs e)
        {
            App.ShowStrokeRawData = (bool)this.CB_ShowStrokeRawData.IsChecked ? true : false;
        }

        // App exit procedure
        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            GetUiState();

            StoreSettings();

            if (App.Socket != null) App.Socket.SocketMessage -= ReceiveMessage;
            if (App.Broker != null)
            {
                App.Broker.AppConnectPublisher -= ReceiveAppConnectPublisher;
                App.Broker.BrokerMessage -= ReceiveMessage;
                App.Broker.AppDrawing -= ReceiveDrawing;  // for drawing
            }
            App.AppMessage -= ReceiveMessage;
        }

        /// <summary>
        /// Received a notification of that Publisher is conencted to the Broker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="index"></param>
        private void ReceiveAppConnectPublisher(object sender, int index)
        {
            //            SetCanvasScaling(index);

            BorderList[index].Visibility = Visibility.Visible;  // Visible the drawing area

            // Publisherが接続されたら、購読を希望しているSubscriberを紐づける
            // 本来ならSubscriberからのリクエストに応じて、Subscriber向けのコネクタ等を
            // 準備する。
            // ここでは便宜的に、ひとつのSubscriberをこのアプリ内に自発的に持つことにする
            Subscriber sub = new Subscriber();
            sub.CanvasStrokes = CanvasStrokesList[index];
            sub.Create(index);
            App.Broker.subs.Add(sub);
        }

        private void ReceiveAppDisconnectPublisher(object sender, int index)
        {
            Subscriber sub = App.Broker.subs[index];
            sub.Dispose(index);
            App.Broker.subs.RemoveAt(index);

            BorderList[index].Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Store/Restore the local data
        private void StoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["PortNumberString"] = PortNumberString;
            container.Values["HostNameString"] = HostNameString;
            string flag = (bool)this.CB_ShowStrokeRawData.IsChecked ? "1" : "0";
            container.Values["ShowStrokeRawData"] = flag;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("PortNumberString"))
                PortNumberString = container.Values["PortNumberString"].ToString();
            if (container.Values.ContainsKey("HostNameString"))
                HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("ShowStrokeRawData"))
                App.ShowStrokeRawData = (container.Values["ShowStrokeRawData"].ToString() == "1") ? true : false;
        }
        #endregion

        #region Drawing
        private void CanvasClear(Canvas canvas)
        {
            try
            {
                foreach (UIElement ui in canvas.Children)
                {
                    canvas.Children.Remove(ui);
                }
            }
            catch (Exception ex)
            {
                ListBox_Message.Items.Add(ex.Message);
            }
        }

        // Raw data event handler sent by SocketServer object
        private void ReceiveDrawing(object sender, List<DeviceRawData> data_list, int index)
        {
            //            DrawStroke(data_list, index);
            DrawStroke(data_list, index);
        }

        //       private async void DrawStroke(float f, float x, float y, float p, int index)
        //        private async void DrawStroke(List<DeviceRawData> deviceRawDataList, int index)
        //        {
        //            try
        //            {
        ////                Publisher pub = App.Pubs[index];

        //                int count = deviceRawDataList.Count;

        //                InkPoint[] points = new InkPoint[count];
        //                for (int i = 0; i < count; i++)
        //                {
        //                    points[i] = new InkPoint(new Windows.Foundation.Point(
        //                        deviceRawDataList[i].x * mScale,
        //                        deviceRawDataList[i].y * mScale),
        //                        deviceRawDataList[i].z);
        //                }

        //                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //                //{
        //                // 描画属性を作成する
        //                InkDrawingAttributes attributes = new InkDrawingAttributes();
        //                attributes.Color = UIColors[index]; //Windows.UI.Colors.Red; //
        //                                                    //attributes.Size = new Size(10, 2);          // ペンのサイズ
        //                                                    //attributes.IgnorePressure = false;          // ペンの圧力を使用するかどうか
        //                                                    //attributes.FitToCurve = false;
        //                Canvas_Strokes.InkPresenter.UpdateDefaultDrawingAttributes(attributes);  // set UI attributes

        //                // Make a stroke by array of point
        //                InkStroke s = m_inkStrokeBuilder.CreateStrokeFromInkPoints(
        //                    points, System.Numerics.Matrix3x2.Identity);
        //                Canvas_Strokes.InkPresenter.StrokeContainer.AddStroke(s);

        //                //});
        // //               pub.StartState = false;
        //            }
        //            catch (Exception ex)
        //            {
        //                ListBox_Message.Items.Add(string.Format("DrawStroke: {0}", ex.Message));
        //            }
        //        }

        private void SetCanvasScaling(int index)
        {
            try
            {
                Publisher pub = App.Pubs[index];

                if (pub != null)
                {
                    double sx = Canvas_Strokes.ActualWidth / pub.DeviceSize.Width;
                    double sy = Canvas_Strokes.ActualHeight / pub.DeviceSize.Height;
                    pub.ViewScale = Math.Min(sx, sy);
                }
            }
            catch (Exception ex)
            {
                ListBox_Message.Items.Add(ex.Message);
            }
        }

        private void DrawStroke(List<DeviceRawData> deviceRawDataList, int index)
        {
            try
            {
                int count = deviceRawDataList.Count;

                InkPoint[] points = new InkPoint[count];
                for (int i = 0; i < count; i++)
                {
                    points[i] = new InkPoint(new Windows.Foundation.Point(
                        deviceRawDataList[i].x * mScale,
                        deviceRawDataList[i].y * mScale),
                        deviceRawDataList[i].z);
                }

                // 本来ならBroker内のsubsではなく、Subscriber側で持つべき
                InkStroke s = App.Broker.subs[index].StrokeBuilder.CreateStrokeFromInkPoints(
                    points, System.Numerics.Matrix3x2.Identity);
                CanvasStrokesList[index].InkPresenter.StrokeContainer.AddStroke(s);
            }
            catch (Exception ex)
            {
                ListBox_Message.Items.Add(string.Format("DrawStroke: {0}", ex.Message));
            }
        }

        // for debug
        private void DrawStroke(int x, int y, int index)
        {
            try
            {
                //                Publisher pub = App.Pubs[index];

                //               int count = deviceRawDataList.Count;

                //                int count = 5;
                int r = 5;
                int _x, _y;

                InkPoint[] points = new InkPoint[5];
                for (int i = 0; i < 5; i++)
                {
                    _x = x;
                    _y = y;
                    switch (i)
                    {
                        case 0:
                        case 4:
                            _y = y - r;
                            break;
                        case 1:
                            _x = x - r;
                            break;
                        case 2:
                            _y = y + r;
                            break;
                        case 3:
                            _x = x + r;
                            break;
                    }
                    points[i] = new InkPoint(new Windows.Foundation.Point(_x, _y), 1);
                }

                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                //{
                // Make a stroke by array of point

                InkStroke s = App.Subs[index].StrokeBuilder.CreateStrokeFromInkPoints(
                points, System.Numerics.Matrix3x2.Identity);
                CanvasStrokesList[index].InkPresenter.StrokeContainer.AddStroke(s);

                //});
            }
            catch (Exception ex)
            {
                //               ListBox_Message.Items.Add(string.Format("DrawStroke: {0}", ex.Message));
            }
        }

        #endregion
    }
}
