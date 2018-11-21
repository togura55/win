using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net.Sockets;
using System.Net;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace WdBroker
{
    /// <summary>
    /// パケット受信イベント用
    /// </summary>
    public class ReceivePacketEventArgs : SocketErrorEventArgs
    {
        public byte[] Data
        {
            get;
        }

        public ReceivePacketEventArgs(System.Net.Sockets.SocketError err, byte[] recvData) : base(err)
        {
            this.Data = recvData;
        }
    }
    /// <summary>
    /// ソケット関連のイベント用
    /// </summary>
    public class SocketErrorEventArgs : EventArgs
    {
        public System.Net.Sockets.SocketError Error
        {
            get;
        }

        public SocketErrorEventArgs(System.Net.Sockets.SocketError err) : base()
        {
            this.Error = err;
        }
    }

    /// <summary>
    /// 一対一用のサーバー、クライアント用ソケット
    /// </summary>
    public class SocketServices : IDisposable
    {
        /// <summary>
        /// 自分のソケット
        /// </summary>
        private Socket mSocket = null;

        /// <summary>
        /// 接続されたクライアントのソケット
        /// </summary>
        private Socket mClientSocket = null;

        /// <summary>
        /// 受信処理用のデータ
        /// </summary>
        private class SocketUserToken
        {
            public Socket ReceiveSocket = null;
            public int PacketSize = 0;
            public MemoryStream PacketBuffer = new MemoryStream();
        }

        #region イベントハンドラ

        /// <summary>
        /// クライアントの受け入れ完了イベント
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> AcceptComplete;
        /// <summary>
        /// クライアントの接続完了イベント
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> ConnectComplete;
        /// <summary>
        /// パケットの送信完了イベント
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> SendPacketComplete;
        /// <summary>
        /// パケットの受信完了イベント
        /// </summary>
        public event EventHandler<ReceivePacketEventArgs> ReceivePacketComplete;

        #endregion

        #region プロパティ

        /// <summary>
        /// 受信用バッファのサイズ
        /// </summary>
        public int ReceiveBufferSize
        {
            get; set;
        } = 2048;

        #endregion

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketMessage?.Invoke(this, message);
            });
        }

        /// <summary>
        /// ソケットのクローズ
        /// </summary>
        public void Close()
        {
            if (mSocket != null)
            {
                mSocket.Dispose();
                mSocket = null;
            }
            mClientSocket = null;
        }

        /// <summary>
        /// サーバーのリッスン開始
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Listen(IPAddress ip, int port)
        {
            // ソケットの作成
            this.Close();
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // リッスンの開始
            IPEndPoint ep = new IPEndPoint(ip, port);
            mSocket.Bind(ep);
            mSocket.Listen(1);

            // 接続受け入れ処理の開始
            this.StartAccept();
        }

        /// <summary>
        /// サーバーへの接続開始
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(IPAddress ip, int port)
        {
            // Create a socket
            this.Close();
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 接続の開始
            IPEndPoint ep = new IPEndPoint(ip, port);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = ep;
            args.Completed += Connect_Completed;
            mSocket.ConnectAsync(args);
        }

        public void Disonnect()
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        /// <summary>
        /// クライアントのソケットからサーバーへパケットを送信する
        /// </summary>
        /// <param name="data"></param>
        public void SendToServer(byte[] data)
        {
            if (mSocket != null && mSocket.Connected)
            {
                this.SendPacket(mSocket, data);
            }
        }
        /// <summary>
        /// サーバーのソケットからクライアントへパケットを送信する
        /// </summary>
        /// <param name="data"></param>
        public void SendToClient(byte[] data)
        {
            if (mClientSocket != null)
            {
                this.SendPacket(mClientSocket, data);
            }
        }

        /// <summary>
        /// 対象のソケットへパケットを送信する
        /// </summary>
        /// <param name="sendSocket"></param>
        /// <param name="data"></param>
        private void SendPacket(Socket sendSocket, byte[] data)
        {
            // パケットデータの長さ
            int len = data.Length;
            byte[] lenData = BitConverter.GetBytes(len);

            // パケットの作成
            List<byte> buf = new List<byte>();
            buf.AddRange(lenData);
            buf.AddRange(data);
            byte[] packetData = buf.ToArray();

            // 送信処理
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(packetData, 0, packetData.Length);
            args.Completed += SendPacket_Completed;

            sendSocket.SendAsync(args);
        }

        /// <summary>
        /// クライアント受け入れ処理の開始
        /// </summary>
        private void StartAccept()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += AcceptAsync_Completed;
            mSocket.AcceptAsync(args);
        }

        /// <summary>
        /// パケットの受信処理の開始
        /// </summary>
        /// <param name="sock"></param>
        private void StartReceive(Socket sock)
        {
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

            // UserTokenに設定するデータ
            SocketUserToken token = new SocketUserToken();
            token.ReceiveSocket = sock;

            // 受信用データの作成
            byte[] szData = new byte[this.ReceiveBufferSize];
            recvArgs.SetBuffer(szData, 0, szData.Length);
            recvArgs.UserToken = token;

            // 受信完了イベント
            recvArgs.Completed += Receive_Completed;

            // 受信開始
            sock.ReceiveAsync(recvArgs);
        }

        #region イベント処理

        /// <summary>
        /// 接続受け入れの完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == System.Net.Sockets.SocketError.Success)
            {
                // クライアントのソケット設定
                mClientSocket = e.AcceptSocket;
                this.StartReceive(mClientSocket);
            }

            // 接続の受け入れ完了イベント
            this.AcceptComplete?.Invoke(this, new SocketErrorEventArgs(e.SocketError));

            e.AcceptSocket = null;
            mSocket.AcceptAsync(e);
        }

        /// <summary>
        /// サーバーへの接続完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != System.Net.Sockets.SocketError.Success)
            {
                // 接続失敗
                this.Close();
            }

            // 受信の開始
            this.StartReceive(mSocket);
            // 接続完了イベント
            this.ConnectComplete?.Invoke(this, new SocketErrorEventArgs(e.SocketError));
        }

        /// <summary>
        /// パケットの送信完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendPacket_Completed(object sender, SocketAsyncEventArgs e)
        {
            this.SendPacketComplete?.Invoke(this, new SocketErrorEventArgs(e.SocketError));
        }

        /// <summary>
        /// パケットの受信完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                SocketUserToken token = (SocketUserToken)e.UserToken;

                if (e.SocketError != System.Net.Sockets.SocketError.Success)
                {
                    // 受信に失敗
                    this.ReceivePacketComplete?.Invoke(this,
                        new ReceivePacketEventArgs(e.SocketError, null));

                    // 受信用バッファのクリア
                    token.ReceiveSocket = null;
                    token.PacketSize = 0;
                    token.PacketBuffer.Dispose();
                    return;
                }

                if (token.PacketSize == 0)
                {
                    // パケットサイズの取得
                    int packetSize = BitConverter.ToInt32(e.Buffer, 0);

                    if (packetSize + 4 > e.Buffer.Length)
                    {
                        // 分割されたパケットの読み込み開始
                        token.PacketSize = packetSize;
                        token.PacketBuffer.Write(e.Buffer, 4, e.Buffer.Length - 4);
                    }
                    else
                    {
                        // 受信完了イベント
                        byte[] data = new byte[packetSize];
                        Array.Copy(e.Buffer, 4, data, 0, packetSize);
                        this.ReceivePacketComplete?.Invoke(this, new ReceivePacketEventArgs(System.Net.Sockets.SocketError.Success, data));
                    }
                }
                else
                {
                    int remainSize = token.PacketSize - ((int)token.PacketBuffer.Length + e.Buffer.Length);

                    if (remainSize <= 0)
                    {
                        // 残りのバッファを読み込み
                        token.PacketBuffer.Write(e.Buffer, 0, e.Buffer.Length + remainSize);

                        // 受信完了イベント
                        this.ReceivePacketComplete?.Invoke(this,
                            new ReceivePacketEventArgs(System.Net.Sockets.SocketError.Success, token.PacketBuffer.ToArray()));

                        // 受信用バッファのクリア
                        token.PacketSize = 0;
                        token.PacketBuffer.SetLength(0);
                    }
                    else
                    {
                        // バッファの追加
                        token.PacketBuffer.Write(e.Buffer, 0, e.Buffer.Length);
                    }
                }

                // 再度パケットデータの受信
                Array.Clear(e.Buffer, 0, e.Buffer.Length);
                token.ReceiveSocket.ReceiveAsync(e);
            }
            catch
            {
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                this.Close();

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~SimpleSocket() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region StreamSocket
        private const string DEFAULT_PORTNUMBER = "1337";
        private const string DEFAULT_HOSTNAME = "192.168.0.7";
        public string HostNameString { get; private set; }
        public string PortNumberString { get; private set; }

        HostName hostName;
        public StreamSocket streamSocket;
        private StreamSocketListener streamSocketListenerData = null;
        //        public StreamSocketListener streamSocketListener;

        // Delegate handlers
        public delegate void MessageEventHandler(object sender, string message);
        public delegate void SocketClientConnectCompletedNotificationHandler(object sender, bool result);
        public delegate void StreamSocketEventHandler(Byte[] byte_array);

        // Properties
        public event MessageEventHandler SocketMessage;
        public event SocketClientConnectCompletedNotificationHandler SocketClientConnectCompletedNotification;
        public event StreamSocketEventHandler StreamSocketReceiveEvent;

        #region StreamSocket services
        public async Task StreamSocket_Start(HostName hostName, string portNumberString)
        {
            try
            {
                // --------- For Data stream-------------
                string port = (int.Parse(portNumberString) + 1).ToString();
                this.SocketMessage?.Invoke(this,
                    String.Format("Start(): try to listen the port for data {0}:{1}...", hostName.ToString(), port));

                streamSocketListenerData = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListenerData.ConnectionReceived += StreamSocketListener_ReceiveBinary;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListenerData.BindEndpointAsync(hostName, port).AsTask().ConfigureAwait(false);

                this.SocketMessage?.Invoke(this,
                 String.Format("Start(): The server for data {0}:{1} is now listening...", hostName.ToString(), port));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Start(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async Task StreamSocket_Connect(string hostNameString = DEFAULT_HOSTNAME,
string portNumberString = DEFAULT_PORTNUMBER,
int timeout = 10000)
        {
            try
            {
                // The server hostname that we will be establishing a connection to. In this example, 
                //   the server and client are in the same process.
                if (hostNameString != DEFAULT_HOSTNAME)
                    HostNameString = hostNameString;
                hostName = new HostName(HostNameString);

                if (portNumberString != DEFAULT_PORTNUMBER)
                    PortNumberString = portNumberString;

                MessageEvent(string.Format("SocketClient.Connect({0},{1}): call ConnectAsync with timeout {2}",
                            HostNameString, PortNumberString, timeout.ToString()));

                // Create the StreamSocket and establish a connection to the echo server.
                this.streamSocket = new StreamSocket();

                CancellationTokenSource cts = new CancellationTokenSource();

                cts.CancelAfter(timeout);
                await this.streamSocket.ConnectAsync(hostName, PortNumberString).AsTask().ConfigureAwait(false);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException(string.Format("SocketClient.Connect(): TaskCanceledException: {0}",
                     ex.Message));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("SocketClient.Connect(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }

            // Notify to caller
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketClientConnectCompletedNotification?.Invoke(this, true);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void StreamSocket_Disonnect()
        {
            try
            {
                this.streamSocket?.Dispose();
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void StreamSocket_Stop()
        {
            try
            {
                if (streamSocketListenerData != null)
                {
                    //                    streamSocketListenerData.ConnectionReceived -= StreamSocketListener_ReceiveBinary;
                    streamSocketListenerData.Dispose();
                }
                MessageEvent("Stop(): Socket services were stop and disposed.");
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Stop(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public void StreamSocket_SendData(IBuffer buffer)
        {
            StreamSocket_SendBinary(this.streamSocket, buffer);
        }

        //public async Task SendCommandResponseAsync(StreamSocketListenerConnectionReceivedEventArgs args, string response)
        //{
        //    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //    () =>
        //    {
        //        StreamSocket_SendString(args, response);
        //    });
        //}
        #endregion

        #region StreamSocket I/O
        private void StreamSocket_SendBinary(StreamSocket socket, IBuffer buffer)
        {
            try
            {
                var packetsToSend = new List<IBuffer>
                {
                    buffer
                };

                var pendingTasks = new System.Threading.Tasks.Task[packetsToSend.Count];

                for (int index = 0; index < packetsToSend.Count; ++index)
                {
                    // track all pending writes as tasks, but don't wait on one before beginning the next.
                    pendingTasks[index] = socket.OutputStream.WriteAsync(packetsToSend[index]).AsTask();
                    // Don't modify any buffer's contents until the pending writes are complete.
                }

                // Wait for all of the pending writes to complete.
                System.Threading.Tasks.Task.WaitAll(pendingTasks);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocket_SendBinary: Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        private async void StreamSocketListener_ReceiveBinary(StreamSocketListener sender,
     StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                using (var dataReader = new DataReader(args.Socket.InputStream))
                {
                    dataReader.InputStreamOptions = InputStreamOptions.Partial;
                    while (true)
                    {
                        await dataReader.LoadAsync(256);
                        if (dataReader.UnconsumedBufferLength == 0) break;
                        IBuffer requestBuffer = dataReader.ReadBuffer(dataReader.UnconsumedBufferLength);
                        Byte[] databyte = requestBuffer.ToArray();  //ReadBytes

                        // inform data to caller
                        this.StreamSocketReceiveEvent?.Invoke(databyte);
                    }
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_ReceiveBinary: Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        #endregion
        #endregion
    }
}
