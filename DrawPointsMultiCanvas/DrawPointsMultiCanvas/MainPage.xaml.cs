using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public MainPage()
        {
            this.InitializeComponent();

            resource = ResourceLoader.GetForCurrentView();
            Pbtn_Run.Content = resource.GetString("IDC_RUN");

            DrawPointList = new List<DrawPoint>();
            Count = 0;
        }

        private void Pbtn_Run_Click(object sender, RoutedEventArgs e)
        {
            DrawPoint drawPoints = new DrawPoint();
            DrawPointList.Add(drawPoints);

            drawPoints.Start();

            Count++;
        }
    }
}
