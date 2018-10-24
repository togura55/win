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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace WillDevicesSampleApp
{
    public class SocketClient
    {
        private const string DEFAULT_PORTNUMBER = "1337";
        private const string DEFAULT_HOSTNAME = "192.168.0.7";
        public string HostNameString { get; private set; }
        public string PortNumberString { get; private set; }

        HostName hostName;
        public StreamSocket streamSocket;
        public StreamSocketListener streamSocketListener;

        public delegate void MessageEventHandler(object sender, string message);
        public delegate void SocketClientConnectCompletedNotificationHandler(object sender, bool result);
        public delegate void SocketClientReceivedResponseNotificationHandler(object sender, float responce);

        // Properties
        public event SocketClientConnectCompletedNotificationHandler SocketClientConnectCompletedNotification;
        public event SocketClientReceivedResponseNotificationHandler SocketClientReceivedResponse;
        public event MessageEventHandler SocketClientMessage;

        public SocketClient()
        {
            Reset();
        }

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketClientMessage?.Invoke(this, message);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            HostNameString = DEFAULT_HOSTNAME;
            PortNumberString = DEFAULT_PORTNUMBER;
        }

        public void CreateListener(string hostNameString = DEFAULT_HOSTNAME,
            string portNumberString = DEFAULT_PORTNUMBER)
        {
            try
            {
                hostName = new HostName(hostNameString);

                streamSocketListener = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ResponseReceived;

                // Start listening for incoming TCP connections on the specified port. 
                // You can specify any port that's not currently in use.
                streamSocketListener.BindEndpointAsync(hostName, PortNumberString).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("SocketClient.CreateListener: Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task ConnectHost(string hostNameString = DEFAULT_HOSTNAME,
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
                throw new TaskCanceledException(string.Format("SocketClient.Connect(): TaskCanceledException: {0}",
                     ex.Message));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
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

        public void CommandSends(string message)
        {
            try
            {

            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("CommandSends(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));

            }
        }

        /// <summary>
        /// A C#-only technique for batched sends.
        /// </summary>
        /// <param name="buffer"></param>
        public void BatchedSends(IBuffer buffer)
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
                    pendingTasks[index] = streamSocket.OutputStream.WriteAsync(packetsToSend[index]).AsTask();
                    // Don't modify any buffer's contents until the pending writes are complete.
                }

                // Wait for all of the pending writes to complete.
                System.Threading.Tasks.Task.WaitAll(pendingTasks);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("BatchedSends(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async Task Receive()
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

                // ToDo: update response in here

                // Notify to caller
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    this.SocketClientReceivedResponse?.Invoke(this, (float)response);
                //});
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(string.Format("Receive(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async Task ResponseReceive()
        {
            const int num_bytes = sizeof(float);    // assuming float type of data

            try
            {
                using (var dataReader = new DataReader(streamSocket.InputStream))
                //                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                {
                    int count = 0;
                    float responce = 0;

                    dataReader.InputStreamOptions = InputStreamOptions.Partial;
                    while (true)
                    {
                        await dataReader.LoadAsync(256);
                        if (dataReader.UnconsumedBufferLength == 0) break;
                        IBuffer requestBuffer = dataReader.ReadBuffer(dataReader.UnconsumedBufferLength);
                        Byte[] databyte = requestBuffer.ToArray();  //ReadBytes

                        // It's depend on each packets how many bytes are included.. 
                        for (int i = 0; i < databyte.Length / num_bytes; i++)
                        {
                            responce = BitConverter.ToSingle(databyte, i * num_bytes);

                            count++;
                        }

                        this.SocketClientReceivedResponse?.Invoke(this, responce);
                    }
                }
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("ResponseReceive(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));

            }
        }

        private async void StreamSocketListener_ResponseReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            const int num_bytes = sizeof(float);    // assuming float type of data

            try
            {
                using (var dataReader = new DataReader(args.Socket.InputStream))
                //                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                {
                    int count = 0;
                    float response = 0;

                    dataReader.InputStreamOptions = InputStreamOptions.Partial;
                    while (true)
                    {
                        await dataReader.LoadAsync(256);
                        if (dataReader.UnconsumedBufferLength == 0) break;
                        IBuffer requestBuffer = dataReader.ReadBuffer(dataReader.UnconsumedBufferLength);
                        Byte[] databyte = requestBuffer.ToArray();  //ReadBytes

                        // It's depend on each packets how many bytes are included.. 
                        for (int i = 0; i < databyte.Length / num_bytes; i++)
                        {
                            response = BitConverter.ToSingle(databyte, i * num_bytes);

                            count++;
                        }

                        this.SocketClientReceivedResponse?.Invoke(this, response);
                    }
                }
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_ResponseReceived: Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));

            }
        }

    }
}
