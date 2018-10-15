using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace MultiView
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CanvasPage : Page
    {
        private CoreDispatcher currentDispatcher;
        RawData PrevRawDara;

        public CanvasPage()
        {
            this.InitializeComponent();

            Loaded += CanvasPage_Loaded;

            App.CurrentApp.PublisherEventMessage += PublisherMessage; // インスタンス作成時にメッセージ受け先を登録しておく。

            PrevRawDara = new RawData();
        }

        private void CanvasPage_Loaded(object sender, RoutedEventArgs e)
        {
            currentDispatcher = Window.Current.Dispatcher;

            int iColor = 0;
            DrawStroke(1, 0, 0, iColor);
            DrawStroke(0, 400, 300, iColor);
            DrawStroke(0, 510, 310, iColor);
            DrawStroke(0, 620, 340, iColor);
            DrawStroke(2, 710, 300, iColor);

            iColor++;
            DrawStroke(1, 0, 0, iColor);
            DrawStroke(0, 300, 200, iColor);
            DrawStroke(0, 410, 210, iColor);
            DrawStroke(0, 520, 240, iColor);
            DrawStroke(2, 610, 200, iColor);
        }

        class RawData
        {
            public float f;
            public float x;
            public float y;
            public float index;

            public RawData(float f = 0, float x = 0, float y = 0, float index = 0)
            {
                this.f = f;
                this.x = x;
                this.y = y;
                this.index = index;
            }
        }

        bool startFlag = false;

        private void DrawStroke(float f, float x, float y, int index)
        {
            if (f == 1)
            {
                // start point, nothing to do
                PrevRawDara.f = f;
                PrevRawDara.x = x;
                PrevRawDara.y = y;
                PrevRawDara.index = index;

                startFlag = true;
            }
            else
            {
                // intermediates and end
                var ellipse = new Ellipse();
                ellipse.Fill = new SolidColorBrush(UIColors[index]);
                ellipse.Width = 4;
                ellipse.Height = 4;
                ellipse.Margin = new Thickness(x, y, 0, 0);

                canvas.Children.Add(ellipse);

                if (!startFlag)
                {
                    //Draw line
                    var line1 = new Line();
                    SolidColorBrush brush = new SolidColorBrush(UIColors[index]);
                    line1.Stroke = brush;
                    line1.X1 = PrevRawDara.x + ellipse.Width / 2;
                    line1.X2 = x + ellipse.Width / 2;
                    line1.Y1 = PrevRawDara.y + ellipse.Height / 2;
                    line1.Y2 = y + ellipse.Height / 2;
                    line1.StrokeThickness = 1;
                    canvas.Children.Add(line1);  //yyはXAML側のGRIDにつけた名前。
                }

                PrevRawDara.f = f;
                PrevRawDara.x = x;
                PrevRawDara.y = y;
                PrevRawDara.index = index;

                startFlag = false;
            }
        }

        //       private void PublisherMessage(object sender, string message)
        private async void PublisherMessage(object sender, Publisher pub)
        {
            //            string msg = message;
            // do something
            Publisher p = pub;
            float x = pub.Strokes[0].DeviceRawDataList[0].x;
            float y = pub.Strokes[0].DeviceRawDataList[0].y;

            var ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.Margin = new Thickness(x, y, 0, 0);

            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    canvas.Children.Add(ellipse);

            //});

            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.canvas.Children.Add(ellipse);
                });
            }
            catch (Exception ex)
            {
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
    }
}
