using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace WdBroker
{
    public class Broker
    {
        SocketServices mServerSock = null;

        // Delegate event handlers
        public delegate void BrokerEventHandler(object sender, string message);
        public delegate void ConnectPublisherEventHandler(object sender, int index);

        // Properties
        public event BrokerEventHandler BrokerMessage;
        public event ConnectPublisherEventHandler AppConnectPublisher;

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
            mServerSock = new SocketServices();
        }

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


        public async Task Start(string hostName, string portNumber)
        {
            // ------- For Commands -------------
            // サーバーのイベント設定
            mServerSock.AcceptComplete += Server_AcceptComplete;
            mServerSock.ReceivePacketComplete += Server_ReceivePacketComplete;
            mServerSock.SendPacketComplete += Server_SendPacketComplete;

            // サーバーのリッスン開始
            mServerSock.Listen(IPAddress.Parse(hostName), int.Parse(portNumber));

            //this.SocketServerMessage?.Invoke(this,
            //    String.Format("Start(): try to listen the port for command {0}:{1}...", ServerHostName.ToString(), PortNumber));

            //streamSocketListenerCommand = new StreamSocketListener();

            //// The ConnectionReceived event is raised when connections are received.
            //streamSocketListenerCommand.ConnectionReceived += StreamSocketListener_ReceiveString;

            //// Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
            //await streamSocketListenerCommand.BindEndpointAsync(ServerHostName, PortNumber).AsTask().ConfigureAwait(false);

            //this.SocketServerMessage?.Invoke(this,
            // String.Format("Start(): The server for command {0}:{1} is now listening...", ServerHostName.ToString(), PortNumber));

            // -------- For Data -----------------
            await App.Socket.Start(portNumber);
        }

        public void Stop()
        {

        }

        #region Delegate handlers
        private async void Server_AcceptComplete(object sender, SocketErrorEventArgs e)
        {
            MessageEvent(string.Format("client socket accepted: {0}", e.Error));
        }

        /// <summary>
        /// パケットの受信完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Server_ReceivePacketComplete(object sender, ReceivePacketEventArgs e)
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

            // パケットの返信
            //msg += " From Server";
            //mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(msg));
            CommandPublisher(msg);
            MessageEvent(string.Format("return message: {0}", msg));
        }

        /// <summary>
        /// パケットの送信完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Server_SendPacketComplete(object sender, SocketErrorEventArgs e)
        {
            MessageEvent(string.Format("packet send completed: {0}", e.Error));
        }
        #endregion


        private void CommandPublisher(string request)
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
                string data = string.Empty;
                if (list.Count >= 2)
                {
                    data = list[2];
                }

                int command_index = CommandList.IndexOf(command) + 1;
                if (command_index > 0)
                {
                    switch (command_index)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            this.MessageEvent("Request Publisher Connect command is received.");

                            //// Do the publisher 1st contact process
                            //// 1. Create a new instance
                            //                          Publisher publisher = new Publisher();
                            App.Pubs.Add(new Publisher());

                            //// 2. Generate Publisher Id, smallest number of pubs
                            float id = 1; // set the base id number
                            float id_new = id;
                            for (int j = 0; j < App.Pubs.Count; j++)
                            {
                                if (App.Pubs[j].Id != id)
                                {
                                    // ToDo: find if id is already stored into another pubs[].Id
                                    id_new = id;
                                    break;
                                }
                                id++;
                            }
                            App.Pubs[App.Pubs.Count - 1].Id = id_new;
                            //                           ConnectPublisherEvent(App.Pubs.Count - 1);  // Notify to caller 

                            //// 3. Respond to the publisher
                            //// response back.
                            //                            App.Socket.SendCommandResponseAsync(args, id_new.ToString());
                            res = id_new.ToString();
                            mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Assigned and sent Publisher ID: {0}", res));

                            break;

                        // Suppose getting this request only one time at the Publisher connection...
                        case CMD_SET_ATTRIBUTES:
                            //var list_data = new List<string>();
                            //list_data.AddRange(data.Split(sp));

                            Publisher pub = App.Pubs[int.Parse(publisher_id)];
                            pub.DeviceSize.Width = double.Parse(list[0]);
                            pub.DeviceSize.Height = double.Parse(list[1]);
                            pub.PointSize = float.Parse(list[2]);
                            pub.DeviceName = list[3];
                            pub.SerialNumber = list[4];
                            pub.Battery = float.Parse(list[5]);
                            pub.DeviceType = list[6];
                            pub.TransferMode = list[7];

                            //                           App.Socket.SendCommandResponseAsync(args, RES_ACK);
                            res = RES_ACK;
                            mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher: {0}", res));

                            // ToDo: What shoud we do when the Publisher request to change the attribute?
                            ConnectPublisherEvent(App.Pubs.Count - 1);  // Notify to caller 
                            MessageEvent("Notify Publisher is connect");

                            break;

                        case CMD_START_PUBLISHER:
                            App.Pubs[int.Parse(publisher_id)].Start();

                            //                            App.Socket.SendCommandResponseAsync(args, RES_ACK);
                            res = RES_ACK;
                            mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher: {0}", res));
                            break;

                        case CMD_STOP_PUBLISHER:
                            App.Pubs[int.Parse(publisher_id)].Stop();

                            //                            App.Socket.SendCommandResponseAsync(args, RES_ACK);
                            res = RES_ACK;
                            mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher: {0}", res));
                            break;

                        case CMD_DISPOSE_PUBLISHER:
                            App.Pubs[int.Parse(publisher_id)].Dispose();

                            //                            App.Socket.SendCommandResponseAsync(args, RES_ACK);
                            res = RES_ACK;
                            mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                            MessageEvent(string.Format("Response to Publisher: {0}", res));
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
                    mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(res));
                    MessageEvent(string.Format("Response to Publisher: {0}", res));
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("OnCommandPublisherEvent: Exception: {0}", ex.Message));
            }
        }
    }
}
