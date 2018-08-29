using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.UI.Core;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace Broker
{
    public class SocketServer
    {
        public HostName ServerHostName;
        private Windows.Networking.Sockets.StreamSocketListener streamSocketListener = null;
        public delegate void MessageEventHandler(object sender, string message);

        // Properties
        public event MessageEventHandler SocketServerMessage;

        public SocketServer()
        {
            ServerHostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();

        }

        public async Task Start(string PortNumber)
        {
            try
            {
                this.SocketServerMessage?.Invoke(this, "try to listen the port...");

                streamSocketListener = new Windows.Networking.Sockets.StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                ////                await streamSocketListener.BindServiceNameAsync(MainPage.PortNumber);
                await streamSocketListener.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

                this.SocketServerMessage?.Invoke(this, "now server is listening...");
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
                streamSocketListener.Dispose();
            }
        }

        public async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }

            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //this.ListBox_Message.Items.Add(string.Format("server received the request: \"{0}\"", request)));

            // Echo the request back as the response.
            using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(request);
                    await streamWriter.FlushAsync();
                }
            }

            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ListBox_Message.Items.Add(string.Format("server sent back the response: \"{0}\"", request)));

            sender.Dispose();

            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ListBox_Message.Items.Add("server closed its socket"));
        }
    }



}
