using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace DrawPointsMultiCanvas
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ResourceLoader resource;
        List<DrawPoint> DrawPointList;
        int Count;
        int MaxCount;
        List<TextBlock> TextBlockList;
        List<InkCanvas> CanvasStrokesList;
        List<Border> BorderList;

        InkStrokeBuilder inkStrokeBuilder;
        List<InkStrokeBuilder> InkStrokeBuilderList;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;

            DrawPointList = new List<DrawPoint>();
            Count = 0;
            MaxCount = 6;
            InkStrokeBuilderList = new List<InkStrokeBuilder>();

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
            TextBlockList = new List<TextBlock> {
                TextBlock_1,
                TextBlock_2,
                TextBlock_3
            };
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            resource = ResourceLoader.GetForCurrentView();
            Pbtn_Run.Content = resource.GetString("IDC_Run");
            Pbtn_Stop.Content = resource.GetString("IDC_Stop");
            Pbtn_Stop.IsEnabled = false;

            foreach (Border b in BorderList)
            {
                b.Visibility = Visibility.Collapsed;
            }
            foreach (TextBlock t in TextBlockList)
            {
                t.Visibility = Visibility.Collapsed;
            }
        }

        //private void ReceivedAction(object sender, string message)
        //{
        //    DrawPoint dp = (DrawPoint)sender;
        //    TextBlock tb = TextBlockList[dp.index];

        //    tb.Text = message;
        //}
        private void ReceivedAction(object sender, int x, int y)
        {
            DrawPoint dp = (DrawPoint)sender;
            //           TextBlock tb = TextBlockList[dp.index];
            //tb.Text = message;

            DrawStroke(x, y, dp.index);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Pbtn_Run_Click(object sender, RoutedEventArgs e)
        {
            if (Count >= MaxCount)
            {
                return;
            }
            BorderList[Count].Visibility = Visibility.Visible;

            DrawPoint drawPoints = new DrawPoint();
            drawPoints.DrawPointAction += ReceivedAction; // set the action message delegate
            drawPoints.index = Count;
            DrawPointList.Add(drawPoints);

            // 描画属性を作成する
            InkDrawingAttributes attributes = new InkDrawingAttributes();
            attributes.Size = new Size(2, 2);          // ペンのサイズ
            attributes.IgnorePressure = false;          // ペンの圧力を使用するかどうか
            attributes.FitToCurve = false;
            attributes.Color = UIColors[Count];

            inkStrokeBuilder = new InkStrokeBuilder();
            inkStrokeBuilder.SetDefaultDrawingAttributes(attributes);
            InkStrokeBuilderList.Add(inkStrokeBuilder);

            CanvasStrokesList[Count].InkPresenter.UpdateDefaultDrawingAttributes(attributes);  // set UI attributes
                                                                                               //            CanvasStrokesList[Count].InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen;

            double interval = 100;
            drawPoints.start(interval);

            Pbtn_Stop.IsEnabled = true;
            Count++;
        }

        private void Pbtn_Stop_Click(object sender, RoutedEventArgs e)
        {
            DrawPoint drawPoints = DrawPointList.Last();
            drawPoints.stop();
            CanvasStrokesList[DrawPointList.Count - 1].InkPresenter.StrokeContainer.Clear();
            BorderList[DrawPointList.Count - 1].Visibility = Visibility.Collapsed;
            DrawPointList.RemoveAt(DrawPointList.Count - 1);

            if (DrawPointList.Count == 0)
            {
                Pbtn_Stop.IsEnabled = false;
            }
            Count--;
        }

        #region Drawing
        //        private InkStrokeBuilder m_inkStrokeBuilder = new InkStrokeBuilder();
//        private double mScale = 1;

        // Raw data event handler sent by SocketServer object
        //        private void ReceiveDrawing(object sender, List<DeviceRawData> data_list, int index)
        private void ReceiveDrawing(object sender, int x, int y, int index)
        {
            DrawStroke(x, y, index);
        }

        //       private async void DrawStroke(float f, float x, float y, float p, int index)
        //        private async void DrawStroke(List<DeviceRawData> deviceRawDataList, int index)
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
                    InkStroke s = InkStrokeBuilderList[index].CreateStrokeFromInkPoints(
                    points, System.Numerics.Matrix3x2.Identity);
                    CanvasStrokesList[index].InkPresenter.StrokeContainer.AddStroke(s);

                //});
            }
            catch (Exception ex)
            {
                //               ListBox_Message.Items.Add(string.Format("DrawStroke: {0}", ex.Message));
            }
        }


        // A list of color table
        List<Windows.UI.Color> UIColors = new List<Windows.UI.Color>() {
            Windows.UI.Colors.SteelBlue,
            //           colors.Add(Windows.UI.Colors.AliceBlue);
            //           colors.Add(Windows.UI.Colors.AntiqueWhite);
            Windows.UI.Colors.Aqua,
            Windows.UI.Colors.Aquamarine,
//            Windows.UI.Colors.Azure,
            //Windows.UI.Colors.Beige,
            //Windows.UI.Colors.Bisque,
            //Windows.UI.Colors.Black,
            //Windows.UI.Colors.BlanchedAlmond,
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
