﻿using BindSample.Common;
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

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace BindSample
{
  /// <summary>
  /// 多くのアプリケーションに共通の特性を指定する基本ページ。
  /// </summary>
  public sealed partial class MainPage : Page
  {

    private NavigationHelper navigationHelper;
    private ObservableDictionary defaultViewModel = new ObservableDictionary();

    /// <summary>
    /// これは厳密に型指定されたビュー モデルに変更できます。
    /// </summary>
    public ObservableDictionary DefaultViewModel
    {
      get { return this.defaultViewModel; }
    }

    /// <summary>
    /// NavigationHelper は、ナビゲーションおよびプロセス継続時間管理を
    /// 支援するために、各ページで使用します。
    /// </summary>
    public NavigationHelper NavigationHelper
    {
      get { return this.navigationHelper; }
    }


    public MainPage()
    {
      this.InitializeComponent();
      this.navigationHelper = new NavigationHelper(this);
      this.navigationHelper.LoadState += navigationHelper_LoadState;
      this.navigationHelper.SaveState += navigationHelper_SaveState;


      // TIPS #71
      // 現在のディスパッチャを保持しておく
      _currentDispatcher = Window.Current.Dispatcher;
      // App クラスからメッセージを受け取るためのイベントハンドラー
      App.CurrentApp.MessageEvent += App_MessageEvent;


      // TIPS #72
      // App クラスがスタティックに保持している Clock オブジェクトをバインドしてみる
      //this.ClockText.DataContext = App.TheClock;
      // MainPage だけでやるなら OK だが、これを SecondaryPage でも行うと例外が発生する!
      // すなわち、1つのバインディングソース・オブジェクトを複数のウィンドウ（アプリビュー）にバインドできないのだ。

      // そこで、それぞれの画面の UI スレッドで PropertyChanged イベントを発火するようなプロキシクラスを介すようにする
      this.ClockText.DataContext = new ClockProxy(App.TheClock);

#if DEBUG
      (this.ClockText.DataContext as ClockProxy).PropertyChanged += MainPage_PropertyChanged;
#endif
    }

    /// <summary>
    /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
    /// 再作成する場合は、保存状態も指定されます。
    /// </summary>
    /// <param name="sender">
    /// イベントのソース (通常、<see cref="NavigationHelper"/>)>
    /// </param>
    /// <param name="e">このページが最初に要求されたときに
    /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたナビゲーション パラメーターと、
    /// 前のセッションでこのページによって保存された状態の辞書を提供する
    /// セッション。ページに初めてアクセスするとき、状態は null になります。</param>
    private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
    {
    }

#if DEBUG
    void MainPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // 【補足】複数ウィンドウ表示のとき、ときどき生じる文字化けについて

      // 次のように、毎秒ごとに ClockText に表示されているはずの文字列を取得して検査してみる。
      // しかし、データとしての表示文字列に異常はないようだ。

      var time = ClockText.Text;
      if (!System.Text.RegularExpressions.Regex.IsMatch(time, "[0-9][0-9]:[0-9][0-9]:[0-9][0-9] \\[[0-9]\\]"))
        throw new InvalidOperationException();
    }
#endif

    private Windows.UI.Core.CoreDispatcher _currentDispatcher;

    private async void App_MessageEvent(string msg)
    {
      await _currentDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
        () =>
        {
          this.MessageTextBlock.Text = msg;
        });
    }

    /// <summary>
    /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
    /// このページに関連付けられた状態を保存します。値は、
    /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
    /// </summary>
    /// <param name="sender">イベントのソース (通常、<see cref="NavigationHelper"/>)</param>
    /// <param name="e">シリアル化可能な状態で作成される空のディクショナリを提供するイベント データ
    ///。</param>
    private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
    {
    }

    #region NavigationHelper の登録

    /// このセクションに示したメソッドは、NavigationHelper がページの
    /// ナビゲーション メソッドに応答できるようにするためにのみ使用します。
    /// 
    /// ページ固有のロジックは、
    /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
    /// および <see cref="GridCS.Common.NavigationHelper.SaveState"/> のイベント ハンドラーに配置する必要があります。
    /// LoadState メソッドでは、前のセッションで保存されたページの状態に加え、
    /// ナビゲーション パラメーターを使用できます。

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      navigationHelper.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      navigationHelper.OnNavigatedFrom(e);
    }

    #endregion



    private async void Show2nd_Click(object sender, RoutedEventArgs e)
    {
      await App.CurrentApp.ShowSecondaryViewAsync(typeof(SecondaryPage), "2nd Window", "2");
    }

    private async void Show3rd_Click(object sender, RoutedEventArgs e)
    {
      await App.CurrentApp.ShowSecondaryViewAsync(typeof(SecondaryPage), "3rd Window", "3");
    }
  }
}
