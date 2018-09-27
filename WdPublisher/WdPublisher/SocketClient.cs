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

        public delegate void MessageEventHandler(object sender, string message);
        public delegate void SocketClientConnectCompletedNotificationHandler(object sender, bool result);
        public delegate void SocketClientReceivedResponseNotificationHandler(object sender, float responce);

        // Properties
        public event MessageEventHandler WacomDevicesMessage;
        public event SocketClientConnectCompletedNotificationHandler SocketClientConnectCompletedNotification;
        public event SocketClientReceivedResponseNotificationHandler SocketClientReceivedResponse;

        // Properties
        public event MessageEventHandler SocketClientMessage;

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketClientMessage?.Invoke(this, message);
            });
        }

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
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task Connect(string hostNameString = DEFAULT_HOSTNAME, 
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
                
                // ToDo: update response in here

                // Notify to caller
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.SocketClientReceivedResponse?.Invoke(this, (float)response);
                });
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(string.Format("Receive(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

    }
}
