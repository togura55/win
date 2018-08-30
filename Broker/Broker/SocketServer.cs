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
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                ////                await streamSocketListener.BindServiceNameAsync(MainPage.PortNumber);
                await streamSocketListener.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

                this.SocketServerMessage?.Invoke(this,
                     String.Format("Start(): now server {0}:{1} is listening...", ServerHostName.ToString(), PortNumber));
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

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                // Your UI update code goes here!
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
    }



}
