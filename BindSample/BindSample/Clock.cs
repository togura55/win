using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindSample
{
  public class Clock : System.ComponentModel.INotifyPropertyChanged
  {
    // 現在時刻を表す文字列のプロパティ "HH:mm:ss [{連番}]"
    // 注意：今回、このプロパティは別スレッドから任意のタイミングで読みだされる可能性がある。
    // すなわち、必要に応じてスレッド間で排他制御しなければならない。
    // 以下は、ReaderWriterLockSlimクラスを使ってスレッド間排他制御を行う例
    private string _nowTime;
    private System.Threading.ReaderWriterLockSlim _rwLock
      = new System.Threading.ReaderWriterLockSlim();
    public string NowTime 
    {
      get 
      {
        _rwLock.EnterReadLock();
        try
        {
          return _nowTime;
        }
        finally
        {
          _rwLock.ExitReadLock();
        }
      }
      private set
      {
        _rwLock.EnterWriteLock();
        try
        {
          _nowTime = value;
        }
        finally
        {
          _rwLock.ExitWriteLock();
        }
      } 
    }


    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    private static int _instanceIndex = 0;  // インスタンスごとに連番を振るために使う
    private string _instanceSuffix; // 時刻の後ろに付ける文字列「[{連番}]」

    private Windows.UI.Xaml.DispatcherTimer _timer; // 生成時のUIスレッドで割り込みを発生させるタイマー

    public Clock()
    {
      _instanceSuffix = string.Format("[{0}]", _instanceIndex++);
      
      Run();
    }



    private void Run()
    {
      _timer = new Windows.UI.Xaml.DispatcherTimer();
      _timer.Interval = TimeSpan.FromMilliseconds(50.0);
      _timer.Tick += _timer_Tick;

      _timer.Start();
    }

    private DateTimeOffset _lastTime;

    void _timer_Tick(object sender, object e)
    {
      var nowTime = DateTimeOffset.Now;
      if (_lastTime.Second != nowTime.Second)
      {
        _lastTime = nowTime;

        // 秒が変わったら、プロパティに時刻をセットし、イベントを発火させる
        this.NowTime = string.Format("{0} {1}", nowTime.ToString("HH:mm:ss"), _instanceSuffix);
        var eventHandler = this.PropertyChanged;
        if (eventHandler != null)
          eventHandler(this, new System.ComponentModel.PropertyChangedEventArgs("NowTime"));
      }
    }
  }
}
