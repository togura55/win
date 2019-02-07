﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            resourceLoader = ResourceLoader.GetForCurrentView();
            this.TextBlock_HostName.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Start.Content = resourceLoader.GetString(fStart ? "IDC_Start" : "IDC_Stop");
            this.TextBox_PortNumberValue.Text = PortNumberString;
            this.Pbtn_Clearlog.Content = resourceLoader.GetString("IDC_Clearlog");
            this.CB_ShowStrokeRawData.Content = resourceLoader.GetString("IDC_ShowStrokeRawData");

            this.TextBox_FixedHostNameValue.Visibility = Visibility.Collapsed;
            this.TextBlock_HostNameValue.Visibility = Visibility.Collapsed;

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

            InkDrawingAttributes attributes = new InkDrawingAttributes();
            attributes.Color = Windows.UI.Colors.Red; //UIColors[index]; //
            attributes.Size = new Size(2, 2);          // ペンのサイズ
            attributes.IgnorePressure = false;          // ペンの圧力を使用するかどうか
            attributes.FitToCurve = false;
            m_inkStrokeBuilder.SetDefaultDrawingAttributes(attributes);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
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
            //inkContainer.Width = cw;
            //inkContainer.Height = ch;
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

        private void ReceiveAppConnectPublisher(object sender, int index)
        {
            SetCanvasScaling(index);
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
            DrawStroke(data_list, index);
        }

        //       private async void DrawStroke(float f, float x, float y, float p, int index)
        private async void DrawStroke(List<DeviceRawData> deviceRawDataList, int index)
        {
            try
            {
//                Publisher pub = App.Pubs[index];

                int count = deviceRawDataList.Count;

                InkPoint[] points = new InkPoint[count];
                for (int i = 0; i < count; i++)
                {
                    points[i] = new InkPoint(new Windows.Foundation.Point(
                        deviceRawDataList[i].x * mScale,
                        deviceRawDataList[i].y * mScale),
                        deviceRawDataList[i].z);
                }

                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                //{
                // 描画属性を作成する
                InkDrawingAttributes attributes = new InkDrawingAttributes();
                attributes.Color = UIColors[index]; //Windows.UI.Colors.Red; //
                                                    //attributes.Size = new Size(10, 2);          // ペンのサイズ
                                                    //attributes.IgnorePressure = false;          // ペンの圧力を使用するかどうか
                                                    //attributes.FitToCurve = false;
                Canvas_Strokes.InkPresenter.UpdateDefaultDrawingAttributes(attributes);  // set UI attributes

                // Make a stroke by array of point
                InkStroke s = m_inkStrokeBuilder.CreateStrokeFromInkPoints(
                    points, System.Numerics.Matrix3x2.Identity);
                Canvas_Strokes.InkPresenter.StrokeContainer.AddStroke(s);

                //});
 //               pub.StartState = false;
            }
            catch (Exception ex)
            {
                ListBox_Message.Items.Add(string.Format("DrawStroke: {0}", ex.Message));
            }
        }

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

        // A list of color table
        List<Windows.UI.Color> UIColors = new List<Windows.UI.Color>() {
            Windows.UI.Colors.SteelBlue,
            //           colors.Add(Windows.UI.Colors.AliceBlue);
            //           colors.Add(Windows.UI.Colors.AntiqueWhite);
            Windows.UI.Colors.Aqua,
            Windows.UI.Colors.Aquamarine,
            Windows.UI.Colors.Azure,
            Windows.UI.Colors.Beige,
            Windows.UI.Colors.Bisque,
            Windows.UI.Colors.Black,
            Windows.UI.Colors.BlanchedAlmond,
            Windows.UI.Colors.Blue,
            Windows.UI.Colors.BlueViolet,
            Windows.UI.Colors.Brown,
            Windows.UI.Colors.BurlyWood,
            Windows.UI.Colors.CadetBlue,
            Windows.UI.Colors.Chartreuse,
            Windows.UI.Colors.Chocolate,
            Windows.UI.Colors.Coral,
            Windows.UI.Colors.CornflowerBlue,
            Windows.UI.Colors.Cornsilk,
            Windows.UI.Colors.Crimson,
            Windows.UI.Colors.Cyan,
            Windows.UI.Colors.DarkBlue,
            Windows.UI.Colors.DarkCyan,
            Windows.UI.Colors.DarkGoldenrod,
            Windows.UI.Colors.DarkGray,
            Windows.UI.Colors.DarkGreen,
            Windows.UI.Colors.DarkKhaki,
            Windows.UI.Colors.DarkMagenta,
            Windows.UI.Colors.DarkOliveGreen,
            Windows.UI.Colors.DarkOrange,
            Windows.UI.Colors.DarkOrchid,
            Windows.UI.Colors.DarkRed,
            Windows.UI.Colors.DarkSalmon,
            Windows.UI.Colors.DarkSeaGreen,
            Windows.UI.Colors.DarkSlateBlue,
            Windows.UI.Colors.DarkSlateGray,
            Windows.UI.Colors.DarkTurquoise,
            Windows.UI.Colors.DarkViolet,
            Windows.UI.Colors.DeepPink,
            Windows.UI.Colors.DeepSkyBlue,
            Windows.UI.Colors.DimGray,
            Windows.UI.Colors.DodgerBlue,
            Windows.UI.Colors.Firebrick,
            Windows.UI.Colors.FloralWhite,
            Windows.UI.Colors.ForestGreen,
            Windows.UI.Colors.Fuchsia,
            Windows.UI.Colors.Gainsboro,
            Windows.UI.Colors.GhostWhite,
            Windows.UI.Colors.Gold,
            Windows.UI.Colors.Goldenrod,
            Windows.UI.Colors.Gray,
            Windows.UI.Colors.Green,
            Windows.UI.Colors.GreenYellow,
            Windows.UI.Colors.Honeydew,
            Windows.UI.Colors.HotPink,
            Windows.UI.Colors.IndianRed,
            Windows.UI.Colors.Indigo,
            Windows.UI.Colors.Ivory,
            Windows.UI.Colors.Khaki,
            Windows.UI.Colors.Lavender,
            Windows.UI.Colors.LavenderBlush,
            Windows.UI.Colors.LawnGreen,
            Windows.UI.Colors.LemonChiffon,
            Windows.UI.Colors.LightBlue,
            Windows.UI.Colors.LightCoral,
            Windows.UI.Colors.LightCyan,
            Windows.UI.Colors.LightGoldenrodYellow,
            Windows.UI.Colors.LightGray,
            Windows.UI.Colors.LightGreen,
            Windows.UI.Colors.LightPink,
            Windows.UI.Colors.LightSalmon,
            Windows.UI.Colors.LightSeaGreen,
            Windows.UI.Colors.LightSkyBlue,
            Windows.UI.Colors.LightSlateGray,
            Windows.UI.Colors.LightSteelBlue,
            Windows.UI.Colors.LightYellow,
            Windows.UI.Colors.Lime,
            Windows.UI.Colors.LimeGreen,
            Windows.UI.Colors.Linen,
            Windows.UI.Colors.Magenta,
            Windows.UI.Colors.Maroon,
            Windows.UI.Colors.MediumAquamarine,
            Windows.UI.Colors.MediumBlue,
            Windows.UI.Colors.MediumOrchid,
            Windows.UI.Colors.MediumPurple,
            Windows.UI.Colors.MediumSeaGreen,
            Windows.UI.Colors.MediumSlateBlue,
            Windows.UI.Colors.MediumSpringGreen,
            Windows.UI.Colors.MediumTurquoise,
            Windows.UI.Colors.MediumVioletRed,
            Windows.UI.Colors.MidnightBlue,
            Windows.UI.Colors.MintCream,
            Windows.UI.Colors.MistyRose,
            Windows.UI.Colors.Moccasin,
            Windows.UI.Colors.NavajoWhite,
            Windows.UI.Colors.Navy,
            Windows.UI.Colors.OldLace,
            Windows.UI.Colors.Olive,
            Windows.UI.Colors.OliveDrab,
            Windows.UI.Colors.Orange,
            Windows.UI.Colors.OrangeRed,
            Windows.UI.Colors.Orchid,
            Windows.UI.Colors.PaleGoldenrod,
            Windows.UI.Colors.PaleGreen,
            Windows.UI.Colors.PaleTurquoise,
            Windows.UI.Colors.PaleVioletRed,
            Windows.UI.Colors.PapayaWhip,
            Windows.UI.Colors.PeachPuff,
            Windows.UI.Colors.Peru,
            Windows.UI.Colors.Pink,
            Windows.UI.Colors.Plum,
            Windows.UI.Colors.PowderBlue,
            Windows.UI.Colors.Purple,
            Windows.UI.Colors.Red,
            Windows.UI.Colors.RosyBrown,
            Windows.UI.Colors.RoyalBlue,
            Windows.UI.Colors.SaddleBrown,
            Windows.UI.Colors.Salmon,
            Windows.UI.Colors.SandyBrown,
            Windows.UI.Colors.SeaGreen,
            Windows.UI.Colors.SeaShell,
            Windows.UI.Colors.Sienna,
            Windows.UI.Colors.Silver,
            Windows.UI.Colors.SkyBlue,
            Windows.UI.Colors.SlateBlue,
            Windows.UI.Colors.SlateGray,
            Windows.UI.Colors.Snow,
            Windows.UI.Colors.SpringGreen,
            Windows.UI.Colors.SteelBlue,
            Windows.UI.Colors.Tan,
            Windows.UI.Colors.Teal,
            Windows.UI.Colors.Thistle,
            Windows.UI.Colors.Tomato,
            Windows.UI.Colors.Transparent,
            Windows.UI.Colors.Turquoise,
            Windows.UI.Colors.Violet,
            Windows.UI.Colors.Wheat,
            Windows.UI.Colors.White,
            Windows.UI.Colors.WhiteSmoke,
            Windows.UI.Colors.Yellow,
            Windows.UI.Colors.YellowGreen
            };
        #endregion
    }
}
