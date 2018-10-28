using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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

namespace WdBroker
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : Application
    {

        public static SocketServer Socket; // Single instance of SocketServer using this app
        public static List<Publisher> Pubs; // List of Publisher object to be managed in this app

        // Delegeat handlers
        public delegate void ConnectPublisherEventHandler(object sender, int index); // for drawing
        public delegate void MessageEventHandler(object sender, string message);

        // Properties
        public event ConnectPublisherEventHandler SocketServerConnectPublisher; // for drawing
        public static event MessageEventHandler AppMessage;

        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        ///最初の行であるため、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Socket = new SocketServer();
        }

        #region Event handler
        private async void MessageEvent(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppMessage?.Invoke(this, message);
            });
        }
        #endregion

        private const int CMD_REQUEST_PUBLISHER_CONNECTION = 1;
        private const int CMD_SET_ATTRIBUTES = 2;
        private const int CMD_START_PUBLISHER = 3;
        private const int CMD_STOP_PUBLISHER = 4;
        private const int CMD_DISPOSE_PUBLISHER = 5;
        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";
        static List<string> CommandList = new List<string> { "1", "2", "3", "4", "5" };  // Command word sent by Publisher

        // for drawing
        private async void ConnectPublisherEvent(int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerConnectPublisher?.Invoke(this, index);
            });
        }

        public void PublisherCommandHandler(string request)
        {
            try
            {
                char sp = ','; // separater
                string[] arr = request.Split(sp);
                var list = new List<string>();
                list.AddRange(arr);

                // decode
                if (list.Count < 2)
                {
                    // error, resend?
                }
                string publisher_id = list[0];
                string command = list[1];
                string data = string.Empty;
                if (list.Count >= 2)
                {
                    data = list[2];
                }

                int command_index = CommandList.IndexOf(request) + 1;
                if (command_index > 0)
                {
                    switch (command_index)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            this.MessageEvent("Request Publisher Connect command is received.");

                            //// Do the publisher 1st contact process
                            //// 1. Create a new instance
                            App.pubs.Add(new Publisher());

                            //// 2. Generate Publisher Id, smallest number of pubs
                            float id = 1; // set the base id number
                            float id_new = id;
                            for (int j = 0; j < App.pubs.Count; j++)
                            {
                                if (App.pubs[j].Id != id)
                                {
                                    // ToDo: find if id is already stored into another pubs[].Id
                                    id_new = id;
                                    break;
                                }
                                id++;
                            }
                            App.pubs[App.pubs.Count - 1].Id = id_new;
                            ConnectPublisherEvent(App.pubs.Count - 1);  // Notify to caller 

                            //// 3. Respond to the publisher
                            //// Echo the request back as the response.
                            App.TheSocketServer.StreamSocketListener_CommandResponse(id_new.ToString());
                            MessageEvent(string.Format("Assigned and sent Publisher ID: {0}", id_new.ToString()));

                            break;

                        case CMD_SET_ATTRIBUTES:
                            var list_data = new List<string>();
                            list_data.AddRange(data.Split(sp));

                            Publisher pub = App.pubs[int.Parse(publisher_id)];
                            pub.DeviceSize.Width = double.Parse(list_data[0]);
                            pub.DeviceSize.Height = double.Parse(list_data[1]);
                            pub.PointSize = float.Parse(list_data[2]);
                            pub.DeviceName = list_data[3];
                            pub.SerialNumber = list_data[4];
                            pub.Battery = float.Parse(list_data[5]);
                            pub.DeviceType = list_data[6];
                            pub.TransferMode = list_data[7];
                            break;

                        case CMD_START_PUBLISHER:
                            App.pubs[int.Parse(publisher_id)].Start();
                            break;

                        case CMD_STOP_PUBLISHER:
                            App.pubs[int.Parse(publisher_id)].Stop();
                            break;

                        case CMD_DISPOSE_PUBLISHER:
                            App.pubs[int.Parse(publisher_id)].Dispose();
                            break;

                        //default:
                        //    commandString = string.Empty;

                        default:
                            break;
                    }
                    //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //this.ListBox_Message.Items.Add(string.Format("server received the request: \"{0}\"", request)));
                    //            this.SocketServerMessage?.Invoke(this, string.Format("StreamSocketListener_ConnectionReceived(): server received the request: \"{0}\"", request));

                    // Echo the request back as the response.
                    //using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                    //{s
                    //    using (var streamWriter = new StreamWriter(outputStream))
                    //    {
                    //        await streamWriter.WriteLineAsync(request);
                    //        await streamWriter.FlushAsync();
                    //    }
                    //}

                    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //{
                    //    this.SocketServerMessage?.Invoke(this, string.Format("StreamSocketListener_ConnectionReceived(): server sent back the response: \"{0}\"", request));
                    //});
                }
                else
                {
                    // invalid command word
                }
            }
            catch (Exception ex)
            {

            }

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
