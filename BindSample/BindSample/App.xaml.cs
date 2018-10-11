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

// 空のアプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234227 を参照してください

namespace BindSample
{


  /// <summary>
  /// 既定の Application クラスに対してアプリケーション独自の動作を実装します。
  /// </summary>
  sealed partial class App : Application
  {
    // TIPS #71
    // 各ウィンドウ（アプリビュー）にメッセージを伝えるためのイベント
    public event Action<string> MessageEvent;

    // App クラスのインスタンス
    public static App CurrentApp { get { return Application.Current as App; } }



    // TIPS #72
    // 唯一のインスタンスとして Clock のオブジェクトを保持
    private static Clock _clock;
    public static Clock TheClock { get { return _clock; } }

    static App()
    {
      _clock = new Clock();
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
    private Dictionary<string, Windows.UI.ViewManagement.ApplicationView> _viewDictionary
      = new Dictionary<string, Windows.UI.ViewManagement.ApplicationView>();

    // SecondaryPage のウィンドウ（アプリビュー）を必要なら作成してから隣に表示する
    public async System.Threading.Tasks.Task ShowSecondaryViewAsync(Type page, string title, string param)
    {
      var viewKey = CreateKeyString(page, param);
      if (!_viewDictionary.ContainsKey(viewKey))
      {
        // まだ存在しないウィンドウ（アプリビュー）なので、作成する。
        // それにはまず、新しい CoreApplicationView を作り、そのスレッドで新しい Frame を作って、ApplicationView を取得する

        // 1. 新しい CoreApplicationView を作る (WindowとApplicationViewが一緒に生成される)
        var coreApplicationView 
              = Windows.ApplicationModel.Core.CoreApplication.CreateNewView();

        Windows.UI.ViewManagement.ApplicationView newAppView = null;
        await coreApplicationView.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                  // 注意：2a.／2b.は、生成されたCoreApplicationViewのスレッドで行う必要がある

                  // 2a. 生成されたApplicationViewを取得する
                   newAppView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();

                  // 2b. 生成されたWindowに画面をセットする
                   SetContent(newAppView, page, title, param);
                }
              );

        // 2c. 生成されたApplicationViewをメモリに保持しておく（後でウィンドウを切り替えたりするのに必要）
        _viewDictionary[viewKey] = newAppView; 
      }

      // 3. viewKey で特定されるウィンドウ（アプリビュー）を隣に表示する
      bool success = await Windows.UI.ViewManagement.ApplicationViewSwitcher
                            .TryShowAsStandaloneAsync(_viewDictionary[viewKey].Id);

      if (success)
      {
        // イベントで情報を伝達
        if (MessageEvent != null)
          MessageEvent(string.Format("「{0}」をアクティブにしました", viewKey));
      }
    }

    // Dictionary に格納するときのキー文字列を生成する
    private string CreateKeyString(Type page, string param)
    {
      return page.Name + "-" + param;
    }

    // 新しいFrameを作ってWindowにセットし、目的の画面にナビゲートする
    private void SetContent(Windows.UI.ViewManagement.ApplicationView newAppView ,Type page, string title, string param)
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
    /// 最初の行であり、main() または WinMain() と論理的に等価です。
    /// </summary>
    public App()
    {
      this.InitializeComponent();
      this.Suspending += OnSuspending;
    }

    /// <summary>
    /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
    /// アプリケーションが特定のファイルを開くために呼び出されたときなどに使用されます。
    /// </summary>
    /// <param name="e">起動要求とプロセスの詳細を表示します。</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {

#if DEBUG
      if (System.Diagnostics.Debugger.IsAttached)
      {
        this.DebugSettings.EnableFrameRateCounter = true;
      }
#endif

      Frame rootFrame = Window.Current.Content as Frame;

      // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
      // ウィンドウがアクティブであることだけを確認してください
      if (rootFrame == null)
      {

        // TIPS#71
        // 現在の ApplicationView をメモリに保持
        _mainView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
        _mainView.Title = "Main Window"; //前回を参照



        // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
        rootFrame = new Frame();
        // 既定の言語を設定します
        rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

        rootFrame.NavigationFailed += OnNavigationFailed;

        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
          //TODO: 以前中断したアプリケーションから状態を読み込みます。
        }

        // フレームを現在のウィンドウに配置します
        Window.Current.Content = rootFrame;
      }

      if (rootFrame.Content == null)
      {
        // ナビゲーションの履歴スタックが復元されていない場合、最初のページに移動します。
        // このとき、必要な情報をナビゲーション パラメーターとして渡して、新しいページを
        // 作成します
        rootFrame.Navigate(typeof(MainPage), e.Arguments);
      }
      // 現在のウィンドウがアクティブであることを確認します
      Window.Current.Activate();
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
    /// アプリケーションの実行が中断されたときに呼び出されます。アプリケーションの状態は、
    /// アプリケーションが終了されるのか、メモリの内容がそのままで再開されるのか
    /// わからない状態で保存されます。
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
