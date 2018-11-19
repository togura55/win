using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net.Sockets;
using System.Net;

namespace WillDevicesSampleApp
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

        public ReceivePacketEventArgs(SocketError err, byte[] recvData) : base(err)
        {
            this.Data = recvData;
        }
    }
    /// <summary>
    /// ソケット関連のイベント用
    /// </summary>
    public class SocketErrorEventArgs : EventArgs
    {
        public SocketError Error
        {
            get;
        }

        public SocketErrorEventArgs(SocketError err) : base()
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
            // ソケットの作成
            this.Close();
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 接続の開始
            IPEndPoint ep = new IPEndPoint(ip, port);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = ep;
            args.Completed += Connect_Completed;
            mSocket.ConnectAsync(args);
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
            if (e.SocketError == SocketError.Success)
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
            if (e.SocketError != SocketError.Success)
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

                if (e.SocketError != SocketError.Success)
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
                        this.ReceivePacketComplete?.Invoke(this, new ReceivePacketEventArgs(SocketError.Success, data));
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
                            new ReceivePacketEventArgs(SocketError.Success, token.PacketBuffer.ToArray()));

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
    }
}
