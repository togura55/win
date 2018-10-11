using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BindSample
{
  class ClockProxy : System.ComponentModel.INotifyPropertyChanged
  {
    // 現在時刻を表す文字列のプロパティ "HH:mm:ss [｛連番｝]"
    public string NowTime { get { return _baseClock.NowTime; } }

    // このイベントは、ウィンドウごとのUIスレッドで発火させたい!
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    private Clock _baseClock;
    private Windows.UI.Core.CoreDispatcher _currentDispatcher;

    // コンストラクト時に、Clockオブジェクトを受け取る
    public ClockProxy(Clock baseClock)
    {
      _baseClock = baseClock;
      _baseClock.PropertyChanged += _baseClock_PropertyChanged;

      // コンストラクト時に、そのUIスレッドのディスパッチャを取得して保持しておく
      _currentDispatcher = Windows.UI.Xaml.Window.Current.Dispatcher;
    }

    // ClockオブジェクトのPropertyChangedイベントで呼び出されるメソッド
    async void _baseClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // このオブジェクトのPropertyChangedイベントをあらためて発火させる
      var eventHandler = this.PropertyChanged;
      if (eventHandler != null)
      {
        var eventArgs = new System.ComponentModel.PropertyChangedEventArgs(e.PropertyName);
        try
        {
          // このメソッドは、Clock オブジェクトのスレッドで呼び出されている。
          // しかし、このオブジェクトが属するUIスレッドでイベントを発火させねばならない
          await _currentDispatcher.RunAsync(
                  Windows.UI.Core.CoreDispatcherPriority.Normal,
                  () => eventHandler(this, eventArgs)
                );
        }
        catch { }
      }
    }

  }
}
