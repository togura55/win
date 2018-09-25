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

namespace WdBroker
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

        List<Publisher> pubs = new List<Publisher>();

        public HostName ServerHostName;
        private StreamSocketListener streamSocketListener = null;
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

                streamSocketListener = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionDataReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListener.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.SocketServerMessage?.Invoke(this,
                     String.Format("Start(): now server {0}:{1} is listening...", ServerHostName.ToString(), PortNumber));
                });
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Start(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        public void Stop()
        {
            if (streamSocketListener != null)
            {
                streamSocketListener.ConnectionReceived -= StreamSocketListener_ConnectionDataReceived;
                streamSocketListener.Dispose();
            }
        }

        private async void StreamSocketListener_ConnectionDataReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            const int num_bytes = sizeof(float);    // assuming float type of data

            try
            {
                using (var dataReader = new DataReader(args.Socket.InputStream))
                {
                    int index = 0;
                    int count = 0;
                    float f = 0, x = 0, y = 0, z = 0;
                    string label = string.Empty;
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
                            float data = BitConverter.ToSingle(databyte, i * num_bytes);

                            if ((count % 4) == 0)
                            {
                                count = 0;
                                f = x = y = z = 0;
                            }

                            switch (count)
                            {
                                case 0:
                                    label = "f"; f = data; break;
                                case 1:
                                    label = "x"; x = data; break;
                                case 2:
                                    label = "y"; y = data; break;
                                case 3:
                                    label = "z"; z = data; break;
                            }
                            string output = "StreamSocketListener_ConnectionDataReceived(): server received the request[{0}]: {1}=";
                            if (label == "z")
                                output += "\"{2:0.######}\"";
                            else
                                output += "\"{2}\"";
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                this.SocketServerMessage?.Invoke(this,
                                    string.Format(output, index, label, data));
                            });

                            // ------------------------
                            if (label == "z")  // all together
                            {
                                // f   [4][  4      ]|[4][4]
                                //     [ ][Begin/End]|[Id  ] 
                                // c.f. ESN = 7BQS0C1000131
                                //const uint Mask_Reserve = 0xFF000000;
                                const uint Mask_PathOrder = 0x00FF0000;
                                const uint Mask_Id = 0x0000FFFF;

                                uint pub_id = ((uint)f | Mask_Id);
                                uint path_order = ((uint)f | Mask_PathOrder);

                                if (pubs.Count == 0 || !pubs.Exists(pubs => pubs.Id == pub_id))
                                {
                                    pubs.Add(new Publisher());
                                    pubs[pubs.Count].Id = pub_id;
                                }
                                else  // Publisher already existed
                                {
                                    // Search by Id, add data list and store raw data
                                    int pi = pubs.FindIndex(n => n.Id == pub_id);

                                    if (path_order == 1)  // begin storoke?
                                    {
                                        pubs[pi].Strokes.Add(new Stroke());
                                    }
                                    else if (path_order == 2)  // end stroke?
                                    {

                                    }
                                    else  // intermediate
                                    {
                                        int s = pubs[pi].Strokes.Count;
                                        pubs[pi].Strokes[s].DeviceRawDataList.Add(new DeviceRawData(x, y, z));
                                    }
                                }
                            }

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