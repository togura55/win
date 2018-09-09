using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.IO;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Threading;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Publisher
{
    public class SocketClient
    {
        public string HostNameString;
        public string PortNumberString;

        private const string DEFAULT_PORTNUMBER = "1337";
        private const string DEFAULT_HOSTNAME = "192.168.0.7";

        HostName hostName;
        StreamSocket streamSocket;

        public delegate void MessageEventHandler(object sender, string message);

        // Properties
        public event MessageEventHandler SocketClientMessage;


        public SocketClient()
        {
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            HostNameString = DEFAULT_HOSTNAME;
            PortNumberString = DEFAULT_PORTNUMBER;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HostNameString"></param>
        /// <param name="PortNumberString"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task Connect(int timeout = 10000)
        {
            try
            {
                this.SocketClientMessage?.Invoke(this,
                    string.Format("Connect(): call ConnectAsync with timeout {0}", timeout.ToString()));

                // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                hostName = new HostName(HostNameString);

                // Create the StreamSocket and establish a connection to the echo server.
                streamSocket = new StreamSocket();

                CancellationTokenSource cts = new CancellationTokenSource();

                cts.CancelAfter(timeout);
                await streamSocket.ConnectAsync(hostName, PortNumberString).AsTask().ConfigureAwait(false);
            }
            catch (TaskCanceledException ex)
            {
                //clientSocket.Close();
                //               clientSocket.Dispose();
                // Debug.WriteLine("Operation was cancelled.");
                throw new TaskCanceledException(string.Format("Connect(): TaskCanceledException: {0}",
                     ex.Message));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Connect(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disonnect()
        {
            try
            {
                //    this.streamSocket?.Disconnect(false);
                this.streamSocket?.Dispose();
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(string.Format("Disconnect(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public async Task Send(string request)
        {
            try
            {
                // Send a request to the echo server.

                using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(request);
                        await streamWriter.FlushAsync();
                    }
                }
                Debug.WriteLine(string.Format("Send: client sent the request: \"{0}\"", request));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Send(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async Task SendByte(float f)

        {
            try
            {
                using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                {
                    //                   using (var streamWriter = new StreamWriter(outputStream))
                    using (var binaryWriter = new BinaryWriter(outputStream))
                    {
                        byte[] byteArray = BitConverter.GetBytes(f);

                        await Task.Run(new Action(() => binaryWriter.Write(byteArray, 0, byteArray.Length)));
                        await Task.Run(new Action(() => binaryWriter.Flush()));
                    }
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("SendByte(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        // This implementation incurs kernel transition overhead for each packet written.
        //public async void SendMultipleBuffersInefficiently(string message)
        //{
        //    var packetsToSend = new List<IBuffer>();
        //    for (int count = 0; count < 5; ++count) { packetsToSend.Add(Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(message, Windows.Security.Cryptography.BinaryStringEncoding.Utf8)); }

        //    foreach (IBuffer packet in packetsToSend)
        //    {
        //        await streamSocket.OutputStream.WriteAsync(packet);
        //    }
        //}

        // A C#-only technique for batched sends.
        public void BatchedSends(IBuffer buffer)
        {
            try
            {
                var packetsToSend = new List<IBuffer>();
                packetsToSend.Add(buffer);

                var pendingTasks = new System.Threading.Tasks.Task[packetsToSend.Count];

                for (int index = 0; index < packetsToSend.Count; ++index)
                {
                    // track all pending writes as tasks, but don't wait on one before beginning the next.
                    pendingTasks[index] = streamSocket.OutputStream.WriteAsync(packetsToSend[index]).AsTask();
                    // Don't modify any buffer's contents until the pending writes are complete.
                }

                // Wait for all of the pending writes to complete.
                System.Threading.Tasks.Task.WaitAll(pendingTasks);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("SendMultipleBuffersInefficiently(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async Task SendMultipleBuffersInefficiently(float f)
        {
            try
            {
                int index = 0;
                var packetsToSend = new List<IBuffer>();
                byte[] byteArray = BitConverter.GetBytes(f);

                for (int count = 0; count < 5; ++count) { packetsToSend.Add(byteArray.AsBuffer()); }
                foreach (IBuffer packet in packetsToSend)
                {
                    await streamSocket.OutputStream.WriteAsync(packet);

                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.SocketClientMessage?.Invoke(this, string.Format("{0}", index));
                    });
                    index++;
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("SendMultipleBuffersInefficiently(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async void Receive()
        {
            try
            {
                // Read data from the echo server.
                string response;
                using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                {
                    using (StreamReader streamReader = new StreamReader(inputStream))
                    {
                        response = await streamReader.ReadLineAsync();
                    }
                }
                //                   this.clientListBox.Items.Add(string.Format("client received the response: \"{0}\" ", response));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(string.Format("Receive(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async void StartClient(string HostNameString, string PortNumberString)
        {
            try
            {
                // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                //                    var hostName = new Windows.Networking.HostName("localhost");
                //           hostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();
                hostName = new HostName(HostNameString);

                // Create the StreamSocket and establish a connection to the echo server.
                using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
                {

                    //                   this.clientListBox.Items.Add("client is trying to connect...");

                    await streamSocket.ConnectAsync(hostName, PortNumberString);

                    //                   this.clientListBox.Items.Add("client connected");

                    // Send a request to the echo server.
                    string request = "Hello, World!";
                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }

                    //                    this.clientListBox.Items.Add(string.Format("client sent the request: \"{0}\"", request));

                    // Read data from the echo server.
                    string response;
                    using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {
                            response = await streamReader.ReadLineAsync();
                        }
                    }

                    //                   this.clientListBox.Items.Add(string.Format("client received the response: \"{0}\" ", response));
                }

                //                this.clientListBox.Items.Add("client closed its socket");
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                //                this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        //public IPEndPoint ServerIPEndPoint { get; set; }
        //private Socket Socket { get; set; }
        //public const int BufferSize = 1024;
        //public byte[] Buffer { get; } = new byte[BufferSize];
        //In
        ////public Client()
        ////{
        ////    this.ServerIPEndPoint = new IPEndPoint(IPAddress.Loopback, 12345);
        ////}

        //// ソケット通信の接続
        //public void Connect()
        //{
        //    this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    this.Socket.Connect(this.ServerIPEndPoint);

        //    // 非同期で受信を待機
        //    this.Socket.BeginReceive(this.Buffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), this.Socket);
        //}

        //// ソケット通信接続の切断
        //public void DisConnect()
        //{
        //    this.Socket?.Disconnect(false);
        //    this.Socket?.Dispose();
        //}

        //// メッセージの送信(同期処理)
        //public void Send(string message)
        //{
        //    var sendBytes = new UTF8Encoding().GetBytes(message);
        //    this.Socket.Send(sendBytes);
        //}

        //// 非同期受信のコールバックメソッド(別スレッドで実行される)
        //private void ReceiveCallback(IAsyncResult asyncResult)
        //{
        //    var socket = asyncResult.AsyncState as Socket;

        //    var byteSize = -1;
        //    try
        //    {
        //        // 受信を待機
        //        byteSize = socket.EndReceive(asyncResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return;
        //    }

        //    // 受信したデータがある場合、その内容を表示する
        //    // 再度非同期での受信を開始する
        //    if (byteSize > 0)
        //    {
        //        Console.WriteLine($"{Encoding.UTF8.GetString(this.Buffer, 0, byteSize)}");
        //        socket.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, ReceiveCallback, socket);
        //    }
        //}

    }
}
