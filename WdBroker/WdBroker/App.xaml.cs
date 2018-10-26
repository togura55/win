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
        // List of Publisher object to be managed in this app
        public static List<Publisher> pubs;

        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        ///最初の行であるため、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        // Delegate Handlers
        public delegate void MessageEventHandler(object sender, string message);
        // Properties
        public static event MessageEventHandler AppMessage;

        private async void MessageEvent(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.AppMessage?.Invoke(this, message);
            });
        }

        private const float CMD_REQUEST_PUBLISHER_CONNECTION = 1;
        private const float CMD_SET_ATTRIBUTES = 2;
        private const float CMD_START_PUBLISHER = 3;
        private const float CMD_STOP_PUBLISHER = 4;
        private const float CMD_DISPOSE_PUBLISHER = 5;
        static List<string> CommandList = new List<string> { "1", "2", "3", "4", "5" };  // Command word sent by Publisher

        public void PublisherCommandHandler(string request)
        {
            try
            {
                char sp = ','; // separater
                string[] arr = request.Split(',');
                var list = new List<string>();
                list.AddRange(arr);

                // decode
                if (list.Count < 2)
                {
                    // error, resend?
                }
                string id = list[0];
                string command = list[1];
                if (list.Count >= 2)
                {
                    string data = list[2];
;                }

                float command = CommandList.IndexOf(request) + 1;
                if (command > 0)
                {
                    switch (command)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            this.MessageEvent("Request Publisher Connect command is received.");

                            // Do the publisher 1st contact process
                            // 1. Create a new instance
                            App.pubs.Add(new Publisher());

                            // 2. Generate Publisher Id, smallest number of pubs
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

                            // 3. Respond to the publisher
                            // Echo the request back as the response.
                            using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                            {
                                using (var binaryWriter = new BinaryWriter(outputStream))
                                {
                                    int num = sizeof(float);
                                    byte[] ByteArray = new byte[num_bytes * 1];
                                    int offset = 0;
                                    Array.Copy(BitConverter.GetBytes(id_new), 0, ByteArray, offset, num);
                                    binaryWriter.Write(ByteArray);
                                    binaryWriter.Flush();
                                    MessageEvent(string.Format("Assigned and sent Publisher ID: {0}", id_new.ToString()));
                                }
                            }
                            break;

                        case CMD_SET_ATTRIBUTES:
                            commandString = string.Format("{0},{1},{2}", PublisherId, 2,
                                AppObjects.Instance.WacomDevice.Attribute.GenerateStrings()); ;
                            break;

                        case CMD_START_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 3);
                            break;

                        case CMD_STOP_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 4);
                            break;

                        case CMD_DISPOSE_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 5);
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
