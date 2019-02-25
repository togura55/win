using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.UI.Core;

namespace WdBroker
{
    public class Broker
    {
        SocketServices mServerSocket = null;
        public List<Subscriber> subs = null;

        // Delegate event handlers
        public delegate void BrokerEventHandler(object sender, string message);
        public delegate void ConnectPublisherEventHandler(object sender, int index);
        public delegate void DisconnectPublisherEventHandler(object sender, int index);
        public delegate void DrawingEventHandler(object sender, List<DeviceRawData> data, int index); // for drawing

        // Properties
        public event BrokerEventHandler BrokerMessage;
        public event ConnectPublisherEventHandler AppConnectPublisher;
        public event DisconnectPublisherEventHandler AppDisconnectPublisher;
        public event DrawingEventHandler AppDrawing; // for drawing

        // Definition of constants
        private const uint MASK_ID = 0x00FF;
        private const uint MASK_STROKE = 0x0F00;
        private const uint MASK_COMMAND = 0xF000;

        private const int CMD_REQUEST_PUBLISHER_CONNECTION = 1;
        private const int CMD_SET_ATTRIBUTES = 2;
        private const int CMD_START_PUBLISHER = 3;
        private const int CMD_STOP_PUBLISHER = 4;
        private const int CMD_DISPOSE_PUBLISHER = 5;
        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";
        static List<string> CommandList = new List<string> { "1", "2", "3", "4", "5" };  // Command word sent by Publisher


        public Broker()
        {
            mServerSocket = new SocketServices();
            subs = new List<Subscriber>();
        }

        #region Wrapper of invoke property messages
        private async void MessageEvent(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.BrokerMessage?.Invoke(this, message);
            });
        }

        private async void ConnectPublisherEvent(int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppConnectPublisher?.Invoke(this, index);
            });
        }

        private async void DisconnectPublisherEvent(int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppDisconnectPublisher?.Invoke(this, index);
            });
        }

        private async void DrawingEvent(List<DeviceRawData> data, int index)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AppDrawing?.Invoke(this, data, index);
            });
        }
        #endregion

        #region Delegate handlers
        private void Server_AcceptComplete(object sender, SocketErrorEventArgs e)
        {
            MessageEvent(string.Format("client socket accepted: {0}", e.Error));
        }

        /// <summary>
        /// パケットの受信完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_ReceivePacketComplete(object sender, ReceivePacketEventArgs e)
        {
            if (e.Error != SocketError.Success)
            {
                // Receive failed
                MessageEvent(string.Format("packet receive error: {0}", e.Error));
                return;
            }

            // 受信したデータをテキストに変換
            string msg = System.Text.Encoding.UTF8.GetString(e.Data);
            MessageEvent(string.Format("packet received: {0}", msg));

            // 受信コマンドの振り分け

            string remoteEndPoint = e.Socket.RemoteEndPoint.ToString();
            PublisherResponseDispatcher(msg, remoteEndPoint);
        }

        /// <summary>
        /// パケットの送信完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_SendPacketComplete(object sender, SocketErrorEventArgs e)
        {
            MessageEvent(string.Format("packet send completed: {0}", e.Error));
        }

        private void DataPublisherReceiveEvent(Byte[] databyte)
        {
            const int num_bytes = sizeof(float);  // 4 bytes
            int index = 0;
            int count = 0;
            float f = 0, x = 0, y = 0, z = 0;
            string label = string.Empty;

            try
            {
                uint path_order = 0;
                int pi = 0;

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

                    if (App.ShowStrokeRawData)
                    {
                        string output = "DataPublisherEvent(): Received data [{0}]:[{1}]:[{2}] {3}=";
                        if (label == "f")
                            //                                output += "\"0x{3:X4}\"";
                            output += "\"{4}\"";
                        else if (label == "z")
                            output += "\"{4:0.######}\"";
                        else
                            output += "\"{4}\"";
                        MessageEvent(string.Format(output, index, ((uint)f & MASK_ID), ((uint)f & MASK_STROKE) >> 8, label, data));
                    }

                    // --------------------------
                    if (label == "z")  // all together
                    {
                        uint pub_id = ((uint)f & MASK_ID);
                        path_order = ((uint)f & MASK_STROKE) >> 8;

                        if (!App.Pubs.Exists(pubs => pubs.Id == pub_id.ToString()))
                        {
                            // Error
                            throw new Exception(string.Format("DataPublisherEvent(): Exception: A publisher includes unknown Publisher ID: {0}",
                                pub_id.ToString()));
                        }
                        else  // Publisher existed
                        {
                            // Search by Id, add data list and store raw data
                            pi = App.Pubs.FindIndex(n => n.Id == pub_id.ToString());

                            DeviceRawData drd = new DeviceRawData(f, x, y, z);
                            if (path_order == 1)  // begin storoke?
                            {
                                Stroke stroke = new Stroke
                                {
                                    DeviceRawDataList = new List<DeviceRawData>()
                                };
                                App.Pubs[pi].Strokes.Add(stroke);
                            }
                            else if (path_order == 2)  // end stroke?
                            {
                                int s = App.Pubs[pi].Strokes.Count - 1;
                                App.Pubs[pi].Strokes[s].DeviceRawDataList.Add(drd);
                            }
                            else  // middle?
                            {
                                int s = App.Pubs[pi].Strokes.Count - 1;
                                App.Pubs[pi].Strokes[s].DeviceRawDataList.Add(drd);
                            }
                            //                           DrawingEvent(drd, pi);  // for drawing
                        }
                    }
                    index++;
                    count++;
                }

                if (path_order != 1)
                {
                    int s = App.Pubs[pi].Strokes.Count - 1;
                    DrawingEvent(App.Pubs[pi].Strokes[s].DeviceRawDataList, pi);  // for drawing
                }

                //                        if (index == 5) break;  // for debug
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("DataPublisherEvent: Exception: {0}", ex.Message));
            }
        }

        private void DataPublisherErrorEvent(HostName host, string message)
        {
            try
            {
                switch (message)
                {
                    case "ConnectionResetByPeer":
                        string hostNameString = host.ToString();
                        int index = -1;
                        for (int i = 0; i < App.Pubs.Count; i++)
                        {
                            if (App.Pubs[i].IpAddress == hostNameString)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index < 0)
                        {
                            throw new Exception(string.Format("No match HostName: {0} in Pubs.", hostNameString));
                        }
                        else
                        {
                            DisconnectPublisherEvent(index);
                            App.Pubs[index].Dispose();
                        }
                        break;

                    default:
                        throw new Exception(string.Format("Unresolved error codes: {0}", message));
                        //          break;
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("DataPublisherErrorEvent: Exception: {0}", ex.Message));
            }
        }
        #endregion

        #region Services
        public async Task Start(HostName hostName, string portNumber)
        {
            try
            {
                // ------- For Commands -------------
                // Delegation settings
                mServerSocket.AcceptComplete += Server_AcceptComplete;
                mServerSocket.ReceivePacketComplete += Server_ReceivePacketComplete;
                mServerSocket.SendPacketComplete += Server_SendPacketComplete;

                // Start server listen for command
                mServerSocket.Listen(IPAddress.Parse(hostName.ToString()), int.Parse(portNumber));

                // -------- For Data -----------------
                // Delegation Settings
                App.Socket.StreamSocketReceiveEvent += DataPublisherReceiveEvent;
                App.Socket.StreamSocketErrorEvent += DataPublisherErrorEvent;

                // Start
                await App.Socket.StreamSocket_Start(hostName, portNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Broker.Start: Exception: {0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                // ---- For Data ----
                App.Socket.StreamSocket_Stop();

                App.Socket.StreamSocketReceiveEvent -= DataPublisherReceiveEvent;
                App.Socket.StreamSocketErrorEvent -= DataPublisherErrorEvent;

                // ---- For Command -------
                mServerSocket.Disonnect();

                mServerSocket.AcceptComplete -= Server_AcceptComplete;
                mServerSocket.ReceivePacketComplete -= Server_ReceivePacketComplete;
                mServerSocket.SendPacketComplete -= Server_SendPacketComplete;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Broker.Stop: Exception: {0}", ex.Message));
            }
        }
        #endregion

        private int FindPublisherId(string id)
        {
            int index = -1;
            int i = 0;
            foreach (Publisher p in App.Pubs)
            {
                if (p.Id == id)
                {
                    index = i;
                    break;
                }
                i++;
            }
            return index;
        }

        /// <summary>
        /// Dispatch command responses sent by Publisher
        /// </summary>
        /// <param name="request"></param>
        /// <param name="remoteEndPoint"></param>
        private void PublisherResponseDispatcher(string request, string remoteEndPoint)
        {
            try
            {
                string res = string.Empty;

                char sp = ','; // separater
                string[] arr = request.Split(sp);
                var list = new List<string>();
                list.AddRange(arr);

                // decode
                if (list.Count < 2)
                {
                    // error, resend?
                }
                string publisher_id = list[0];
                string command = list[1];

                int index = -1;

                int command_index = CommandList.IndexOf(command) + 1;
                if (command_index > 0)
                {
                    switch (command_index)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            this.MessageEvent("Request Publisher Connect command is received.");

                            //if (Count >= MaxCount)
                            //{
                            //    return;
                            //}

                            //// Do the publisher 1st contact process
                            //// 1. Create a new instance
                            //                          Publisher publisher = new Publisher();
                            App.Pubs.Add(new Publisher());

                            // ToDo: rewrite GeneratePublisherId for identical ID strings
                            //// 2. Generate unique Publisher Id, smallest number of pubs
                            float id = 1; // set the base id number
                            float id_new = id;
                            for (int j = 0; j < App.Pubs.Count; j++)
                            {
                                if (App.Pubs[j].Id != id.ToString())
                                {
                                    // ToDo: find if id is already stored into another pubs[].Id
                                    id_new = id;
                                    break;
                                }
                                id++;
                            }
                            App.Pubs[App.Pubs.Count - 1].Id = id_new.ToString();

                            // 3. Get the Publisher's IP address and store it 
                            {
                                char sp2 = ':'; // separater
                                string[] arr2 = remoteEndPoint.Split(sp2);
                                var list2 = new List<string>();
                                list2.AddRange(arr2);

                                App.Pubs[App.Pubs.Count - 1].IpAddress = list2[0];
                            }

                            //// 4. Respond to the Publisher
                            //// response back.
                            res = id_new.ToString();
                            mServerSocket.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Assigned and sent Publisher ID: {0}", res));

                            break;

                        // Suppose getting this request only one time at the Publisher connection...
                        case CMD_SET_ATTRIBUTES:
                            MessageEvent(string.Format("CMD_SET_ATTRIBUTES received from ID: {0}", publisher_id));
                            if ((index = FindPublisherId(publisher_id)) < 0)
                                res = RES_NAK;
                            else
                            {
                                int i = 1;
                                Publisher pub = App.Pubs[index];
                                pub.DeviceSize.Width = double.Parse(list[++i]);
                                pub.DeviceSize.Height = double.Parse(list[++i]);
                                pub.PointSize = float.Parse(list[++i]);
                                pub.DeviceName = list[++i];
                                pub.SerialNumber = list[++i];
                                pub.Battery = float.Parse(list[++i]);
                                pub.DeviceType = list[++i];
                                pub.TransferMode = list[++i];

                                // ToDo: What shoud we do when the Publisher request to change the attribute?
                                ConnectPublisherEvent(App.Pubs.Count - 1);  // Notify to caller 
                                MessageEvent(string.Format("Notify: Publisher is connected ID={0}", pub.Id));

                                res = RES_ACK;
                            }
                            mServerSocket.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher ID {0}: {1}", publisher_id, res));

                            break;

                        case CMD_START_PUBLISHER:
                            MessageEvent(string.Format("CMD_START_PUBLISHER received from ID: {0}", publisher_id));
                            if ((index = FindPublisherId(publisher_id)) < 0)
                                res = RES_NAK;
                            else
                            {
                                App.Pubs[index].Start();
                                res = RES_ACK;
                            }
                            mServerSocket.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher ID {0}: {1}", publisher_id, res));
                            break;

                        case CMD_STOP_PUBLISHER:
                            MessageEvent(string.Format("CMD_STOP_PUBLISHER received from ID: {0}", publisher_id));
                            if ((index = FindPublisherId(publisher_id)) < 0)
                                res = RES_NAK;
                            else
                            {
                                App.Pubs[index].Stop();
                                res = RES_ACK;
                            }
                            mServerSocket.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher ID {0}: {1}", publisher_id, res));
                            break;

                        case CMD_DISPOSE_PUBLISHER:
                            MessageEvent(string.Format("CMD_DISPOSE_PUBLISHER received from ID: {0}", publisher_id));
                            if ((index = FindPublisherId(publisher_id)) < 0)
                                res = RES_NAK;
                            else
                            {
                                // ToDo: do something
                                res = RES_ACK;
                            }
                            mServerSocket.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher ID {0}: {1}", publisher_id, res));
                            break;

                        //default:
                        //    commandString = string.Empty;

                        default:
                            break;
                    }
                }
                else
                {
                    // invalid command word
                    //                    App.Socket.SendCommandResponseAsync(args, RES_NAK);
                    res = RES_ACK;
                    mServerSocket.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                    MessageEvent(string.Format("Response to Publisher: {0}", res));
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("CommandPublisher: Exception: {0}", ex.Message));
            }
        }
    }
}
