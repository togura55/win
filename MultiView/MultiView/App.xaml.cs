using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

namespace MultiView
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : Application
    {
        // TIPS #71
        // 各ウィンドウ（アプリビュー）にメッセージを伝えるためのイベント
        public event Action<string> MessageEvent;

        // App クラスのインスタンス
        public static App CurrentApp { get { return Application.Current as App; } }

//        public delegate void MessageEventHandler(object sender, string message);
        public delegate void MessageEventHandler(object sender, Publisher pub);
        public event MessageEventHandler PublisherEventMessage;

//        public void PublisherEvent(object sender, string message)
        public void PublisherEvent(object sender, Publisher pub)
        {
//            App.CurrentApp.PublisherEventMessage?.Invoke(sender, message);
            App.CurrentApp.PublisherEventMessage?.Invoke(sender, pub);
        }

        // TIPS #72
        // 唯一のインスタンスとして Clock のオブジェクトを保持
        //       private static Clock _clock;
        //       public static Clock TheClock { get { return _clock; } }

        static App()
        {
            //            _clock = new Clock();
        }


        // MainPage のウィンドウ（アプリビュー）
        private Windows.UI.ViewManagement.ApplicationView _mainView;
        // ウィンドウ（アプリビュー）を開くためだけなら、ApplicationView オブジェクトを丸ごと保持する必要はない。Idだけで十分。

        // MainPage のウィンドウ（アプリビュー）を隣に表示する
        public async System.Threading.Tasks.Task ShowMainViewAsync()
        {
            bool success = await Windows.UI.ViewManagement.ApplicationViewSwitcher
                                  .TryShowAsStandaloneAsync(_mainView.Id);
            if (success)
            {
                // イベントで情報を伝達
                if (MessageEvent != null)
                    MessageEvent("MainPageをアクティブにしました");
            }
        }



        // SecondaryPage のウィンドウ（アプリビュー）

        // SecondaryPage のアプリビューを保持しておくコレクション
        // ここではオブジェクトごと保持しているが、切り替えるだけならSecondaryPageのIdだけを保持しておけばよい
        public Dictionary<string, Windows.UI.ViewManagement.ApplicationView> _viewDictionary
          = new Dictionary<string, Windows.UI.ViewManagement.ApplicationView>();

        // Publisher object that is unique in this app
 //       public Dictionary<string, Publisher> pubs = new Dictionary<string, Publisher>();
        public Dictionary<string, Publisher> Pubs { get; set; }

        public async Task ShowSecondaryViewAsync(Type page, string title, string param)
        {
            var viewKey = CreateKeyString(page, param);
            if (!_viewDictionary.ContainsKey(viewKey))
            {
                // Create a Window (i.e. ApplicationView) because there are not exsisted yet.
                // First of all, create a CoreApplicationView, and then creating a new Frame in that thread
                //  and obtain a ApplicationView

                CoreApplicationView newView = CoreApplication.CreateNewView(); // generated Window and ApplicationView together
                ApplicationView newAppView = null;
                // Run into the CoreapplicationView we created
                await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Frame frame = new Frame();
                    frame.Navigate(page, param);
                    Window.Current.Content = frame;

                    // You have to activate the window in order to show it later.
                    Window.Current.Activate();
                    newAppView = ApplicationView.GetForCurrentView();  // get the ApplicationView
                    newAppView.Title = title;

                    // Set the event handler for closing window by user
                    newAppView.Consolidated += AppView_Consolidated;
                });

                // Store the ApplicationView in the list (for the purpose of using swit window, etc.) 
                _viewDictionary[viewKey] = newAppView;
            }

            // Display window which is specified by viewKey
            bool success = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_viewDictionary[viewKey].Id);
            if (success)
            {
                // イベントで情報を伝達
                //if (MessageEvent != null)
                //    MessageEvent(string.Format("「{0}」をアクティブにしました", viewKey));
            }
        }

        // Dictionary に格納するときのキー文字列を生成する
        public string CreateKeyString(Type page, string param)
        {
            return page.Name + "-" + param;
        }

        // 新しいFrameを作ってWindowにセットし、目的の画面にナビゲートする
        private void SetContent(Windows.UI.ViewManagement.ApplicationView newAppView, Type page, string title, string param)
        {
            // Frame を作り、引数で指定された画面を表示させ、その Frame を現在の Window に結び付ける
            var newFrame = new Frame();
            newFrame.Navigate(page, param);
            Window.Current.Content = newFrame;
            // ↑この Window.Current は、現スレッドを実行している CoreApplicationView のもの

            // ApplicationView に、引数で指定されたタイトル文字列を追加する
            newAppView.Title = title;

            // エンドユーザーがウィンドウを閉じたときのイベントハンドラを結び付けておく
            newAppView.Consolidated += AppView_Consolidated;
        }

        // エンドユーザーがウィンドウを閉じたときのイベントハンドラ
        private void AppView_Consolidated(Windows.UI.ViewManagement.ApplicationView sender, Windows.UI.ViewManagement.ApplicationViewConsolidatedEventArgs args)
        {
            // 管理用のコレクションからも削除しておく(次に開くときは新規に開始させたいので)
            var title = RemoveViewFromDictionary(sender.Id);

            if (MessageEvent != null)
                MessageEvent(string.Format("ユーザーが「{0}」を閉じました", title));
        }



        // 指定されたウィンドウ（アプリビュー）を閉じる
        public void CloseView(Window secondaryWindow, int viewId)
        {
            // Window の Close メソッドで、ApplicationView も失われてしまう。
            // そこで管理用のコレクションからも削除しておく(次に開くときは、新しく生成される)。
            var title = RemoveViewFromDictionary(viewId);

            if (MessageEvent != null && !string.IsNullOrEmpty(title))
                MessageEvent(string.Format("ボタンクリックで「{0}」を閉じました", title));

            secondaryWindow.Close();
        }

        private string RemoveViewFromDictionary(int viewId)
        {
            foreach (var k in _viewDictionary.Keys)
            {
                if (_viewDictionary[k].Id == viewId)
                {
                    var title = _viewDictionary[k].Title;

                    _viewDictionary.Remove(k);
                    return title;
                }
            }
            return null;
        }



        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        ///最初の行であるため、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
        /// アプリケーションが特定のファイルを開くために起動されたときなどに使用されます。
        /// </summary>
        /// <param name="e">起動の要求とプロセスの詳細を表示します。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください
            if (rootFrame == null)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 以前中断したアプリケーションから状態を読み込みます
                }

                // フレームを現在のウィンドウに配置します
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // ナビゲーション スタックが復元されない場合は、最初のページに移動します。
                    // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
                    //構成します
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 現在のウィンドウがアクティブであることを確認します
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 特定のページへの移動が失敗したときに呼び出されます
        /// </summary>
        /// <param name="sender">移動に失敗したフレーム</param>
        /// <param name="e">ナビゲーション エラーの詳細</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// アプリケーションの実行が中断されたときに呼び出されます。
        /// アプリケーションが終了されるか、メモリの内容がそのままで再開されるかに
        /// かかわらず、アプリケーションの状態が保存されます。
        /// </summary>
        /// <param name="sender">中断要求の送信元。</param>
        /// <param name="e">中断要求の詳細。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: アプリケーションの状態を保存してバックグラウンドの動作があれば停止します
            deferral.Complete();
        }
    }
}
