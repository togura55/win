using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.UI.Core;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Broker
{
    public class SocketServer
    {
        class RawData
        {
            public float f;
            public float x;
            public float y;
            public float z;
            public RawData(float f = 0, float x = 0, float y = 0, float z = 0)
            {
                this.f = f;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public HostName ServerHostName;
        private Windows.Networking.Sockets.StreamSocketListener streamSocketListener = null;
        public delegate void MessageEventHandler(object sender, string message);
        public List<HostName> HostNames = new List<HostName>();

        // Properties
        public event MessageEventHandler SocketServerMessage;

        public SocketServer()
        {
            ServerHostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();

            RetrieveHostNames();
        }

        public void RetrieveHostNames()
        {
            foreach (HostName hostName in NetworkInformation.GetHostNames())
            {
                if (hostName.IPInformation != null)
                {
                    if (hostName.Type == HostNameType.Ipv4)
                    {
                        HostNames.Add(new HostName(hostName.ToString()));

                        //                       textblock.Text = hostName.ToString();
                        //                       break;
                    }
                }
            }
        }

        public async Task Start(string PortNumber)
        {
            try
            {
                this.SocketServerMessage?.Invoke(this,
                    String.Format("Start(): try to listen the port {0}:{1}...", ServerHostName.ToString(), PortNumber));

                streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                //               streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
                //streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionBinaryReceived;
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionDataReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                ////                await streamSocketListener.BindServiceNameAsync(MainPage.PortNumber);
                await streamSocketListener.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.SocketServerMessage?.Invoke(this,
                     String.Format("Start(): now server {0}:{1} is listening...", ServerHostName.ToString(), PortNumber));
                });
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Start(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public void Stop()
        {
            if (streamSocketListener != null)
            {
                //                streamSocketListener.ConnectionReceived -= StreamSocketListener_ConnectionReceived;
                streamSocketListener.ConnectionReceived -= StreamSocketListener_ConnectionDataReceived;
                streamSocketListener.Dispose();
            }
        }

        public async void StreamSocketListener_ConnectionBinaryReceived(Windows.Networking.Sockets.StreamSocketListener sender,
            Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                //            string request;
                //using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                //{
                //    request = await streamReader.ReadLineAsync();
                //}

                while (true)
                {
                    byte[] databyte;
                    float data;
                    //            using (BinaryReader reader = new BinaryReader(args.Socket.InputStream.AsStreamForRead()))
                    BinaryReader reader = new BinaryReader(args.Socket.InputStream.AsStreamForRead());
                    //           {
                    // intの数値を読み込む

                    // 4バイト読み込む
                    databyte = reader.ReadBytes(4);
                    data = BitConverter.ToSingle(databyte, 0);
                    //            }

                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.SocketServerMessage?.Invoke(this, string.Format("StreamSocketListener_ConnectionBinaryReceived(): server received data: \"{0}\"", data));
                    });
                }
                sender.Dispose();

                //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ListBox_Message.Items.Add("server closed its socket"));
                //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    this.SocketServerMessage?.Invoke(this, "StreamSocketListener_ConnectionBinaryReceived(): server closed its socket");
                //});
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_ConnectionBinaryReceived(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, 
            Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 this.SocketServerMessage?.Invoke(this, string.Format("StreamSocketListener_ConnectionReceived(): server received the request: \"{0}\"", request));
             });

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

            sender.Dispose();

            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ListBox_Message.Items.Add("server closed its socket"));
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerMessage?.Invoke(this, "StreamSocketListener_ConnectionReceived(): server closed its socket");
            });
        }

        private async void StreamSocketListener_ConnectionDataReceived(Windows.Networking.Sockets.StreamSocketListener sender, 
            Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            const int num_bytes = sizeof(float);    // assuming float type of data

            try
            {
                using (var dataReader = new DataReader(args.Socket.InputStream))
                {
                    int index = 0;
                    int count = 0;
                    string label = string.Empty;
                    dataReader.InputStreamOptions = InputStreamOptions.Partial;
                    while (true)
                    {
                        await dataReader.LoadAsync(256);
                        if (dataReader.UnconsumedBufferLength == 0) break;
                        IBuffer requestBuffer = dataReader.ReadBuffer(dataReader.UnconsumedBufferLength);
                        Byte[] databyte = requestBuffer.ToArray();  //ReadBytes
                        
                        // It's depend on each packets how many bytes are included.. 
                        for (int i=0; i<databyte.Length/num_bytes; i++)
                        {
                            float f = BitConverter.ToSingle(databyte, i*num_bytes);

                            if ((count % 4) == 0) count = 0;

                            switch (count)
                            {
                                case 0:
                                    label = "f"; break;
                                case 1:
                                    label = "x"; break;
                                case 2:
                                    label = "y"; break;
                                case 3:
                                    label = "z"; break;
                            }
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                this.SocketServerMessage?.Invoke(this, 
                                    string.Format("StreamSocketListener_ConnectionDataReceived(): server received the request[{0}]: {1}=\"{2}\"", index, label, f));
                            });

                            index++;
                            count++;
                        }

//                        if (index == 5) break;  // for debug

                    }
                }
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_ConnectionDataReceived(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));

            }
        }

    }
}