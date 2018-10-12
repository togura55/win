using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace MultiView
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int window_count;

        public MainPage()
        {
            this.InitializeComponent();
            window_count = 0;
        }

        private async void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            // Suppose a publisher connection was made....


            await App.CurrentApp.ShowSecondaryViewAsync(typeof(CanvasPage), "2nd Window", window_count.ToString());
            window_count++;
        }

        private void Pbtn_Data_Click(object sender, RoutedEventArgs e)
        {
            int count = App.CurrentApp._viewDictionary.Count;
//            Windows.UI.ViewManagement.ApplicationView appView = App.CurrentApp._viewDictionary["2nd Window" + "-" + "2"];

            for (int i=0; i<count; i++)
            {

                Publisher pub = new Publisher();
                pub.Id = 0;
                pub.Strokes = new List<Publisher.Stroke>();
                pub.Strokes.Add(new Publisher.Stroke());
                pub.Strokes[pub.Strokes.Count - 1].DeviceRawDataList = new List<Publisher.DeviceRawData>();
                pub.Strokes[pub.Strokes.Count - 1].DeviceRawDataList.Add(new Publisher.DeviceRawData(100*(i+1),100*(i+1),0));

                MessageEvent("Data comes from Publisher", pub);
            }
        }

        private async void MessageEvent(string message, Publisher pub)
        {
 //           string key = App.CurrentApp.CreateKeyString(typeof(CanvasPage), id.ToString());

            // ↓誰に対して?
//            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//            {
                App.CurrentApp.PublisherEvent(this, pub);

//            });
        }


        // 複数のウィンドウに同じデータを表示するには？
        // http://www.atmarkit.co.jp/ait/articles/1404/24/news112.html

        // 複数のウィンドウに情報を伝達するには？
        // http://www.atmarkit.co.jp/ait/articles/1404/17/news072.html

    }
}
