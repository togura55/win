using System;
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
using Windows.UI.Xaml.Shapes;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace MultiView
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CanvasPage : Page
    {
        public CanvasPage()
        {
            this.InitializeComponent();

            Loaded += CanvasPage_Loaded;

        }

        private void CanvasPage_Loaded(object sender, RoutedEventArgs e)
        {
            float x = 500;
            float y = 200;

            var ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.Margin = new Thickness(x, y, 0, 0);

            canvas.Children.Add(ellipse);
        }
    }
}
