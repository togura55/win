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

using Windows.Networking.Sockets;   //
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace WdBroker
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : Application
    {
        public static Broker Broker = null;
        public static SocketServices Socket = null; // Single instance of SocketServices using this app
        public static List<Publisher> Pubs = new List<Publisher>(); // List of Publisher object to be managed in this app

        public static HostName ServerHostName;
        public static List<HostName> HostNames = new List<HostName>();

        // Delegeat handlers
        public delegate void MessageEventHandler(object sender, string message);
        public delegate void ConnectPublisherEventHandler(object sender, int index);
        public delegate void DrawingEventHandler(object sender, DeviceRawData data, int index); // for drawing

        // Properties
        public static event MessageEventHandler AppMessage;
        public static event ConnectPublisherEventHandler AppConnectPublisher;
        public static event DrawingEventHandler AppDrawing; // for drawing

        // Definition of constants
        private const uint MASK_ID = 0x00FF;
        private const uint MASK_STROKE = 0x0F00;
        private const uint MASK_COMMAND = 0xF000;

        private const int CMD_REQUEST_PUBLISHER_CONNECTION = 1;
        private const int CMD_SET_ATTRIBUTES = 2;
        private const int CMD_START_PUBLISHER = 3;
        private const int CMD_STOP_PUBLISHER = 4;
        private const int CMD_DISPOSE_PUBLISHER = 5;
        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";
        static List<string> CommandList = new List<string> { "1", "2", "3", "4", "5" };  // Command word sent by Publisher


        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        ///最初の行であるため、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            AppInitialize();
        }
        private void AppInitialize()
        {
            ServerHostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();
            RetrieveHostNames();

            Broker = new Broker();
            Socket = new SocketServices();

            //            Socket.CommandEvent += OnCommandPublisherEvent;
            Socket.ReceivePacketComplete += OnCommandPublisherEvent;
            Socket.StreamSocketReceiveEvent += OnDataPublisherEvent;
        }

        private void AppDispose()
        {
            Socket.ReceivePacketComplete -= OnCommandPublisherEvent;
            Socket.StreamSocketReceiveEvent -= OnDataPublisherEvent;

            Socket.StreamSocket_Stop();
            Socket = null;
        }

        #region Event handlers
        private async void MessageEvent(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppMessage?.Invoke(this, message);
            });
        }

        //// for drawing
        private async void DrawingEvent(DeviceRawData data, int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppDrawing?.Invoke(this, data, index);
            });
        }

        private async void ConnectPublisherEvent(int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppConnectPublisher?.Invoke(this, index);
            });
        }

        // for Networking
        public void RetrieveHostNames()
        {
            foreach (HostName hostName in NetworkInformation.GetHostNames())
            {
                if (hostName.IPInformation != null)
                {
                    if (hostName.Type == HostNameType.Ipv4)
                    {
                        HostNames.Add(new HostName(hostName.ToString()));
                    }
                }
            }
        }

        public async void SendCommandResponseAsync(string response)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                App.Socket.SendToClient(System.Text.Encoding.UTF8.GetBytes(response));
            });
        }

        //        private void OnCommandPublisherEvent(StreamSocketListenerConnectionReceivedEventArgs args, string request)
        private void OnCommandPublisherEvent(object sender, ReceivePacketEventArgs e)
        {
            try
            {
                if (e.Error != System.Net.Sockets.SocketError.Success)
                {
                    // 受信に失敗
                    MessageEvent(string.Format("Command packet receive error: {0}", e.Error));
                    return;
                }

                // 受信したデータをテキストに変換
                string request = System.Text.Encoding.UTF8.GetString(e.Data);
                MessageEvent(string.Format("packet received : {0}", request));

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

                int command_index = CommandList.IndexOf(command) + 1;
                if (command_index > 0)
                {
                    switch (command_index)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            this.MessageEvent("Request Publisher Connect command is received.");

                            //// Do the publisher 1st contact process
                            //// 1. Create a new instance
                            //                          Publisher publisher = new Publisher();
                            App.Pubs.Add(new Publisher());

                            //// 2. Generate Publisher Id, smallest number of pubs
                            float id = 1; // set the base id number
                            float id_new = id;
                            for (int j = 0; j < App.Pubs.Count; j++)
                            {
                                if (App.Pubs[j].Id != id)
                                {
                                    // ToDo: find if id is already stored into another pubs[].Id
                                    id_new = id;
                                    break;
                                }
                                id++;
                            }
                            App.Pubs[App.Pubs.Count - 1].Id = id_new;
                            //                           ConnectPublisherEvent(App.Pubs.Count - 1);  // Notify to caller 

                            //// 3. Respond to the publisher
                            //// response back.
                            SendCommandResponseAsync(id_new.ToString());
                            MessageEvent(string.Format("Assigned and sent Publisher ID: {0}", id_new.ToString()));

                            break;

                        // Suppose getting this request only one time at the Publisher connection...
                        case CMD_SET_ATTRIBUTES:
                            var list_data = new List<string>();
                            list_data.AddRange(data.Split(sp));

                            Publisher pub = App.Pubs[int.Parse(publisher_id)];
                            pub.DeviceSize.Width = double.Parse(list_data[0]);
                            pub.DeviceSize.Height = double.Parse(list_data[1]);
                            pub.PointSize = float.Parse(list_data[2]);
                            pub.DeviceName = list_data[3];
                            pub.SerialNumber = list_data[4];
                            pub.Battery = float.Parse(list_data[5]);
                            pub.DeviceType = list_data[6];
                            pub.TransferMode = list_data[7];

                            SendCommandResponseAsync(RES_ACK);
                            MessageEvent(string.Format("Response to Publisher: {0}", RES_ACK));

                            // ToDo: What shoud we do when the Publisher request to change the attribute?
                            ConnectPublisherEvent(App.Pubs.Count - 1);  // Notify to caller 
                            MessageEvent("Notify Publisher is connect");

                            break;

                        case CMD_START_PUBLISHER:
                            App.Pubs[int.Parse(publisher_id)].Start();

                            SendCommandResponseAsync(RES_ACK);
                            MessageEvent(string.Format("Response to Publisher: {0}", RES_ACK));
                            break;

                        case CMD_STOP_PUBLISHER:
                            App.Pubs[int.Parse(publisher_id)].Stop();

                            SendCommandResponseAsync(RES_ACK);
                            MessageEvent(string.Format("Response to Publisher: {0}", RES_ACK));
                            break;

                        case CMD_DISPOSE_PUBLISHER:
                            App.Pubs[int.Parse(publisher_id)].Dispose();

                            SendCommandResponseAsync(RES_ACK);
                            MessageEvent(string.Format("Response to Publisher: {0}", RES_ACK));
                            break;

                        //default:
                        //    commandString = string.Empty;

                        default:
                            break;
                    }
                }
                else
                {
                    // invalid command word
                    SendCommandResponseAsync(RES_NAK);
                    MessageEvent(string.Format("Response to Publisher: {0}", RES_NAK));
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("OnCommandPublisherEvent: Exception: {0}", ex.Message));
            }
        }

        private void OnDataPublisherEvent(Byte[] databyte)
        {
            const int num_bytes = sizeof(float);
            int index = 0;
            int count = 0;
            float f = 0, x = 0, y = 0, z = 0;
            string label = string.Empty;

            try
            {
                // It's depend on each packets how many bytes are included.. 
                for (int i = 0; i < databyte.Length / num_bytes; i++)
                {
                    float data = BitConverter.ToSingle(databyte, i * num_bytes);

                    if ((count % 4) == 0)
                    {
                        count = 0;
                        f = x = y = z = 0;
                    }

                    switch (count)
                    {
                        case 0:
                            label = "f"; f = data; break;
                        case 1:
                            label = "x"; x = data; break;
                        case 2:
                            label = "y"; y = data; break;
                        case 3:
                            label = "z"; z = data; break;
                    }

                    string output = "OnDataPublisherEvent(): Received data [{0}]:[{1}]:[{2}] {3}=";
                    if (label == "f")
                        //                                output += "\"0x{3:X4}\"";
                        output += "\"{4}\"";
                    else if (label == "z")
                        output += "\"{4:0.######}\"";
                    else
                        output += "\"{4}\"";
                    MessageEvent(string.Format(output, index, ((uint)f & MASK_ID), ((uint)f & MASK_STROKE) >> 8, label, data));

                    // --------------------------
                    if (label == "z")  // all together
                    {
                        uint pub_id = ((uint)f & MASK_ID);
                        uint path_order = ((uint)f & MASK_STROKE) >> 8;

                        if (!App.Pubs.Exists(pubs => pubs.Id == pub_id))
                        {
                            // Error
                            throw new Exception(string.Format("OnDataPublisherEvent(): Exception: A publisher includes unknown Publisher ID: {0}",
                                pub_id.ToString()));
                        }
                        else  // Publisher existed
                        {
                            // Search by Id, add data list and store raw data
                            int pi = App.Pubs.FindIndex(n => n.Id == pub_id);

                            DeviceRawData drd = new DeviceRawData(f, x, y, z);
                            if (path_order == 1)  // begin storoke?
                            {
                                Stroke stroke = new Stroke
                                {
                                    DeviceRawDataList = new List<DeviceRawData>()
                                };
                                App.Pubs[pi].Strokes.Add(stroke);
                            }
                            else if (path_order == 2)  // end stroke?
                            {
                                int s = App.Pubs[pi].Strokes.Count - 1;
                                App.Pubs[pi].Strokes[s].DeviceRawDataList.Add(drd);
                            }
                            else  // intermediate
                            {
                                int s = App.Pubs[pi].Strokes.Count - 1;
                                App.Pubs[pi].Strokes[s].DeviceRawDataList.Add(drd);
                            }
                            DrawingEvent(drd, pi);  // for drawing
                        }
                    }
                    index++;
                    count++;
                }

                //                        if (index == 5) break;  // for debug
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("OnDataPublisherEvent: Exception: {0}", ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// アプリケーションがエンド ユーザーによって正常に起動されたときに呼び出されます。他のエントリ ポイントは、
        /// アプリケーションが特定のファイルを開くために起動されたときなどに使用されます。
        /// </summary>
        /// <param name="e">起動の要求とプロセスの詳細を表示します。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください
            if (!(Window.Current.Content is Frame rootFrame))
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
            AppDispose();

            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: アプリケーションの状態を保存してバックグラウンドの動作があれば停止します
            deferral.Complete();
        }
    }
}
