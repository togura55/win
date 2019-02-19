using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;

namespace DrawPointsMultiCanvas
{
    class DrawPoint
    {
        private ThreadPoolTimer _timer;
        private int _count;
        public int index;

        // Delegate handlers
        //        public delegate void MessageEventHandler(object sender, string message);
        public delegate void MessageEventHandler(object sender, int x, int y);

        // Properties
        public event MessageEventHandler DrawPointAction;

        public DrawPoint()
        {
            _count = 0;
        }

        //private async Task ActionEvent(string message)
        //{
        //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        this.DrawPointAction?.Invoke(this, message);
        //    });
        //}
        private async Task ActionEvent(int x, int y)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.DrawPointAction?.Invoke(this, x, y);
            });
        }

        public void start(double interval)
        {
            this._timer = ThreadPoolTimer.CreatePeriodicTimer(_timerEvent, TimeSpan.FromMilliseconds(1));
            //              this.Pbtn_Run.Content = "タイマー停止";
        }

        public void stop()
        {
            this._timer.Cancel();
            this._timer = null;
            //          this.Pbtn_Run.Content = "タイマー再開";
        }


        private async void _timerEvent(ThreadPoolTimer timer)
        {
            this._count++;

            // 乱数からx, yを生成
            Random rnd = new Random();                  //乱数を発生させます
            int x = rnd.Next(257);        //ランダムなX座標の取得
            int y = rnd.Next(300);        //ランダムなY座標の取得

            // UIスレッド以外のスレッドから画面を更新する場合はDispatcher.RunAsyncを利用する
            // 非同期処理なのでawait/asyncキーワードが必要になる
            //            await ActionEvent(this._count.ToString());
            await ActionEvent(x, y);

            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    MainPage.TextBlock_Test.Text = this._count.ToString();
            //});
        }
    }
}
