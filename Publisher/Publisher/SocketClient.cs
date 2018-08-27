﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.IO;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Publisher
{
    public class SocketClient
    {
        HostName hostName;
        StreamSocket streamSocket;

        public SocketClient()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HostNameString"></param>
        /// <param name="PortNumberString"></param>
        public async void Connect(string HostNameString, string PortNumberString)
        {
            try
            {
                // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                hostName = new HostName(HostNameString);

                // Create the StreamSocket and establish a connection to the echo server.
                using (streamSocket = new StreamSocket())
                {
                    Debug.WriteLine(string.Format("client is trying to connect..."));
                    //                    await streamSocket.ConnectAsync(hostName, PortNumberString);
 //                   await Task.Run(() => { streamSocket.ConnectAsync(hostName, PortNumberString); }).ConfigureAwait(false);

                    await streamSocket.ConnectAsync(hostName, PortNumberString).AsTask().ConfigureAwait(false);

                    //if (!streamSocket.ConnectAsync(hostName, PortNumberString).AsTask().Wait(2000))
                    //{
                    //    throw (new Exception("Connection Timeout."));
                    //}

                    Debug.WriteLine(string.Format("client connected"));
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(string.Format("Connect(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));

                throw;

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
        public async void Send(string request)
        {
            try
            {
                // Send a request to the echo server.
                //            string request = "Hello, World!";
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
                Debug.WriteLine(string.Format("Send(): Exception: {0}",
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
