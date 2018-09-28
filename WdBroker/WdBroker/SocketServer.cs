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
        // Command Packet Byte Definition and Command values
        private const uint MASK_ID = 0x00FF;
        private const uint MASK_STROKE = 0x0F00;
        private const uint MASK_COMMAND = 0xF000;

        private const float CMD_REQUESTPUBLISHERCONNECT = 1;


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

                            // --------------------------
                            if (label == "f")
                            {
                                // command packet?
                                float command = ((uint)f | MASK_COMMAND) >> 16;
                                if (command != 0)
                                {
                                    switch (command)
                                    {
                                        case CMD_REQUESTPUBLISHERCONNECT:
                                            // Do the publisher 1st contact process
                                            // 1. Generate Publisher Id, smallest number of pubs
                                            float id = 0;
                                            for (int j=0; j < pubs.Count; j++)
                                            {
                                                if (pubs[j].Id > id)
                                                    id = pubs[j].Id + 1;
                                            }
                                            // 2. Create Instance
                                            pubs.Add(new Publisher());
                                            pubs[pubs.Count].Id = id;

                                            // 3. Respond to the publisher
                                            // Echo the request back as the response.
                                            using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                                            {
                                                using (var binaryWriter = new BinaryWriter(outputStream))
                                                {
                                                    int num = sizeof(float);
                                                    byte[] ByteArray = new byte[num_bytes * 1];
                                                    int offset = 0;
                                                    Array.Copy(BitConverter.GetBytes(id), 0, ByteArray, offset, num);
                                                    binaryWriter.Write(ByteArray);
                                                }
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }

                            // --------------------------
                            if (label == "z")  // all together
                            {
                                // f   [ 8 ][4][2][2]|[    16    ]
                                //     [cmd][stroke ]|[    Id    ] 
                                // c.f. ESN = 7BQS0C1000131
                                //const uint Mask_Reserve = 0xFF000000;
                                const uint Mask_PathOrder = 0x00FF0000;
                                const uint Mask_Id = 0x0000FFFF;

                                uint pub_id = ((uint)f | Mask_Id);
                                uint path_order = ((uint)f | Mask_PathOrder);

                                if (!pubs.Exists(pubs => pubs.Id == pub_id))
                                {
                                    // Error
                                    throw new Exception(string.Format("StreamSocketListener_ConnectionDataReceived(): Exception: A publisher includes unknown Publisher ID: {0}",
                                        pub_id.ToString()));
                                }
                                else  // Publisher existed
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

        private IBuffer CreateTransferBuffer(float cmd)
        {
            IBuffer buffer = null;

            int num_bytes = sizeof(float);
            byte[] ByteArray = new byte[num_bytes * 1];
            int offset = 0;
            Array.Copy(BitConverter.GetBytes(cmd), 0, ByteArray, offset, num_bytes);
            using (DataWriter writer = new DataWriter())
            {
                writer.WriteBytes(ByteArray);
                buffer = writer.DetachBuffer();
            }

            return buffer;
        }

        //public void BatchedSends(IBuffer buffer)
        //{
        //    try
        //    {
        //        var packetsToSend = new List<IBuffer>
        //        {
        //            buffer
        //        };

        //        var pendingTasks = new System.Threading.Tasks.Task[packetsToSend.Count];

        //        for (int index = 0; index < packetsToSend.Count; ++index)
        //        {
        //            // track all pending writes as tasks, but don't wait on one before beginning the next.
        //            pendingTasks[index] = streamSocketListener.OutputStream.WriteAsync(packetsToSend[index]).AsTask();
        //            // Don't modify any buffer's contents until the pending writes are complete.
        //        }

        //        // Wait for all of the pending writes to complete.
        //        System.Threading.Tasks.Task.WaitAll(pendingTasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
        //        throw new Exception(string.Format("BatchedSends(): Exception: {0}",
        //            webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message));
        //    }
        //}

    }
}