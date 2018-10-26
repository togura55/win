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
        // Command Packet Byte Definition and Command values
        // f   [ 8 ][4][2][2]|[    16    ]
        //     [cmd][stroke ]|[    Id    ] 
        //      c.f. ESN = 7BQS0C1000131
        private const uint MASK_ID = 0x00FF;
        private const uint MASK_STROKE = 0x0F00;
        private const uint MASK_COMMAND = 0xF000;

        private const float CMD_REQUESTPUBLISHERCONNECT = 1;  // for binarry transmittion


        public HostName ServerHostName;
        private StreamSocketListener streamSocketListenerData = null;
        private StreamSocketListener streamSocketListenerCommand = null;
        public delegate void MessageEventHandler(object sender, string message);
        public delegate void ConnectPublisherEventHandler(object sender, int index); // for drawing
        public delegate void DrawingEventHandler(object sender, DeviceRawData data, int index); // for drawing
        public List<HostName> HostNames = new List<HostName>();

        // Properties
        public event MessageEventHandler SocketServerMessage;
        public event ConnectPublisherEventHandler SocketServerConnectPublisher; // for drawing
        public event DrawingEventHandler SocketServerDrawing; // for drawing

        public SocketServer()
        {
            ServerHostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();

            RetrieveHostNames();

            App.pubs = new List<Publisher>();
        }

        private async void MessageEvent(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerMessage?.Invoke(this, message);
            });
        }

        // for drawing
        private async void ConnectPublisherEvent(int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerConnectPublisher?.Invoke(this, index);
            });
        }

        // for drawing
        private async void DrawingEvent(DeviceRawData data, int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerDrawing?.Invoke(this, data, index);
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

        public async Task Start(string PortNumber)
        {
            try
            {
                this.SocketServerMessage?.Invoke(this,
                    String.Format("Start(): try to listen the port for command {0}:{1}...", ServerHostName.ToString(), PortNumber));

                streamSocketListenerCommand = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListenerCommand.ConnectionReceived += StreamSocketListener_CommandReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListenerCommand.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

                this.SocketServerMessage?.Invoke(this,
                 String.Format("Start(): The server for command {0}:{1} is now listening...", ServerHostName.ToString(), PortNumber));

                // ----------------------
                string port = (int.Parse(PortNumber) + 1).ToString();
                this.SocketServerMessage?.Invoke(this,
                    String.Format("Start(): try to listen the port for data {0}:{1}...", ServerHostName.ToString(), port));

                streamSocketListenerData = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListenerData.ConnectionReceived += StreamSocketListener_DataReceived;

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
                    streamSocketListenerCommand.ConnectionReceived -= StreamSocketListener_CommandReceived;
                    streamSocketListenerCommand.Dispose();
                }
                if (streamSocketListenerData != null)
                {
                    streamSocketListenerData.ConnectionReceived -= StreamSocketListener_DataReceived;
                    streamSocketListenerData.Dispose();
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("Stop(): Exception: {0}", webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

 
        public async void StreamSocketListener_CommandReceived2(Windows.Networking.Sockets.StreamSocketListener sender,
    Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }
            MessageEvent(string.Format("StreamSocketListener_ConnectionReceived2(): Command: \"{0}\"", request));

            App.PublisherCommandHandler(request);

            sender.Dispose();

            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ListBox_Message.Items.Add("server closed its socket"));
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.SocketServerMessage?.Invoke(this, "StreamSocketListener_ConnectionReceived(): server closed its socket");
            });
        }


        private async void StreamSocketListener_CommandReceived(StreamSocketListener sender,
     StreamSocketListenerConnectionReceivedEventArgs args)
        {
            const int num_bytes = sizeof(float);    // assuming float type of data

            try
            {
                using (var dataReader = new DataReader(args.Socket.InputStream))
                {
                    int index = 0;

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

                            MessageEvent(string.Format("StreamSocketListener_CommandReceived(): server received the request[{0}]: {1}",
                                index, data));

                            // command packet?
                            float command = ((uint)data & MASK_COMMAND) >> 12;
                            if (command != 0)
                            {
                                switch (command)
                                {
                                    case CMD_REQUESTPUBLISHERCONNECT:
                                        MessageEvent("Request Publisher Connect command is received.");

                                        // Do the publisher 1st contact process
                                        // 1. Create a new instance
                                        App.pubs.Add(new Publisher());

                                        // 2. Generate Publisher Id, smallest number of pubs
                                        float id = 1; // set the base id number
                                        float id_new = id;
                                        for (int j = 0; j < App.pubs.Count; j++)
                                        {
                                            if (App.pubs[j].Id != id)
                                            {
                                                // ToDo: find if id is already stored into another pubs[].Id
                                                id_new = id;
                                                break;
                                            }
                                            id++;
                                        }
                                        App.pubs[App.pubs.Count - 1].Id = id_new;
                                        ConnectPublisherEvent(App.pubs.Count - 1);  // Notify to caller 

                                        // 3. Respond to the publisher
                                        // Echo the request back as the response.
                                        using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                                        {
                                            using (var binaryWriter = new BinaryWriter(outputStream))
                                            {
                                                int num = sizeof(float);
                                                byte[] ByteArray = new byte[num_bytes * 1];
                                                int offset = 0;
                                                Array.Copy(BitConverter.GetBytes(id_new), 0, ByteArray, offset, num);
                                                binaryWriter.Write(ByteArray);
                                                binaryWriter.Flush();
                                                MessageEvent(string.Format("Assigned and sent Publisher ID: {0}", id_new.ToString()));
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                throw new Exception(string.Format("StreamSocketListener_CommandReceived(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

        private async void StreamSocketListener_DataReceived(StreamSocketListener sender,
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

                            string output = "DataReceived(): Received data [{0}]:[{1}]:[{2}] {3}=";
                            if (label == "f")
                                //                                output += "\"0x{3:X4}\"";
                                output += "\"{4}\"";
                            else if (label == "z")
                                output += "\"{4:0.######}\"";
                            else
                                output += "\"{4}\"";
                            MessageEvent(string.Format(output, index, ((uint)f & MASK_ID), ((uint)f & MASK_STROKE) >> 8, label, data));

                            // --------------------------
                            if (label == "z")  // all together
                            {
                                uint pub_id = ((uint)f & MASK_ID);
                                uint path_order = ((uint)f & MASK_STROKE) >> 8;

                                if (!App.pubs.Exists(pubs => pubs.Id == pub_id))
                                {
                                    // Error
                                    throw new Exception(string.Format("StreamSocketListener_DataReceived(): Exception: A publisher includes unknown Publisher ID: {0}",
                                        pub_id.ToString()));
                                }
                                else  // Publisher existed
                                {
                                    // Search by Id, add data list and store raw data
                                    int pi = App.pubs.FindIndex(n => n.Id == pub_id);

                                    DeviceRawData drd = new DeviceRawData(f, x, y, z);
                                    if (path_order == 1)  // begin storoke?
                                    {
                                        Stroke stroke = new Stroke();
                                        stroke.DeviceRawDataList = new List<DeviceRawData>();
                                        App.pubs[pi].Strokes.Add(stroke);
                                    }
                                    else if (path_order == 2)  // end stroke?
                                    {
                                        int s = App.pubs[pi].Strokes.Count - 1;
                                        App.pubs[pi].Strokes[s].DeviceRawDataList.Add(drd);
                                    }
                                    else  // intermediate
                                    {
                                        int s = App.pubs[pi].Strokes.Count - 1;
                                        App.pubs[pi].Strokes[s].DeviceRawDataList.Add(drd);
                                    }
                                    DrawingEvent(drd, pi);  // for drawing
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
                throw new Exception(string.Format("StreamSocketListener_DataReceived(): Exception: {0}",
                    webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
            }
        }

    }
}