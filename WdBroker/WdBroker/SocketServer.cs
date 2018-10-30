using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.UI.Core;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace WdBroker
{
    public class SocketServer
    {
        public HostName ServerHostName;
        public List<HostName> HostNames = new List<HostName>();
        private StreamSocketListener streamSocketListenerData = null;
        private StreamSocketListener streamSocketListenerCommand = null;

        // Delegate event handlers
        public delegate void MessageEventHandler(object sender, string message);
        public delegate void CommandEventHandler(StreamSocketListenerConnectionReceivedEventArgs args, string message);
        public delegate void DataEventHandler(Byte[] byte_array);

        // Properties
        public event MessageEventHandler SocketServerMessage;
        public event CommandEventHandler CommandEvent;
        public event DataEventHandler DataEvent;

        public SocketServer()
        {
            ServerHostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();

            RetrieveHostNames();
        }

        private async void MessageEvent(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerMessage?.Invoke(this, message);
            });
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
                    }
                }
            }
        }

        #region SocketServer services
        public async Task Start(string PortNumber)
        {
            try
            {
                // ------- For Commands -------------
                this.SocketServerMessage?.Invoke(this,
                    String.Format("Start(): try to listen the port for command {0}:{1}...", ServerHostName.ToString(), PortNumber));

                streamSocketListenerCommand = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListenerCommand.ConnectionReceived += StreamSocketListener_ReceiveString;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListenerCommand.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

                this.SocketServerMessage?.Invoke(this,
                 String.Format("Start(): The server for command {0}:{1} is now listening...", ServerHostName.ToString(), PortNumber));

                // --------- For Data -------------
                string port = (int.Parse(PortNumber) + 1).ToString();
                this.SocketServerMessage?.Invoke(this,
                    String.Format("Start(): try to listen the port for data {0}:{1}...", ServerHostName.ToString(), port));

                streamSocketListenerData = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListenerData.ConnectionReceived += StreamSocketListener_ReceiveBinary;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListenerData.BindEndpointAsync(ServerHostName, port).AsTask().ConfigureAwait(false);

                this.SocketServerMessage?.Invoke(this,
                 String.Format("Start(): The server for data {0}:{1} is now listening...", ServerHostName.ToString(), port));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Start(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                if (streamSocketListenerCommand != null)
                {
                    streamSocketListenerCommand.ConnectionReceived -= StreamSocketListener_ReceiveString;
                    streamSocketListenerCommand.Dispose();
                }
                if (streamSocketListenerData != null)
                {
                    streamSocketListenerData.ConnectionReceived -= StreamSocketListener_ReceiveBinary;
                    streamSocketListenerData.Dispose();
                }
                this.SocketServerMessage?.Invoke(this, "Stop(): Socket services were stop and disposed.");
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Stop(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public async Task SendCommandResponseAsync(StreamSocketListenerConnectionReceivedEventArgs args, string response)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>{
                StreamSocket_SendString(args, response);
            });
        }
        #endregion

        #region Socket I/O
        private async void StreamSocket_SendString(
            StreamSocketListenerConnectionReceivedEventArgs args, string message)
        {
            try
            {
                //  Send the response back to Publisher as string
                using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(message);
                        await streamWriter.FlushAsync();
                    }
                    this.SocketServerMessage?.Invoke(this, String.Format("StreamSocket_SendString: Sent: {0}", message));
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocket_SendString: Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        private async void StreamSocketListener_ReceiveString(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                string request;
                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                {
                    request = await streamReader.ReadLineAsync();
                }
                MessageEvent(string.Format("StreamSocketListener_ReceiveString: Command: \"{0}\"", request));

                //                App.PublisherCommandHandler(args, request);
                this.CommandEvent?.Invoke(args, request);

                //                sender.Dispose();
                //                MessageEvent(string.Format("StreamSocketListener_ReceiveString: server closed its socket"));
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_ReceiveString: Exception: {0}",
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
                        this.DataEvent?.Invoke(databyte);
                    }
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_ReceiveBinary: Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }
        #endregion
    }
}