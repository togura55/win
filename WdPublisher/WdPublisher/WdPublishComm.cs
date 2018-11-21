using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace WillDevicesSampleApp
{
    public class WdPublishComm
    {
        public Socket mSocket = null;

        public SocketServices dataSocketClient = null;

        float CommandResponseState;
        float PublisherId;
        string HostNameString = string.Empty;
        string PortNumberString = string.Empty;

        // Delegate handlers
        public delegate void MessageEventHandler(object sender, string message);
        public delegate void InitializationCompletedNotificationHandler(object sender, bool result);

        // Properties
        public event MessageEventHandler WdPublishCommMessage;
        public event InitializationCompletedNotificationHandler InitializationCompletedNotification;

        public WdPublishComm()
        {
            CommandResponseState = CMD_NEUTRAL;
            PublisherId = 0;
        }

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.WdPublishCommMessage?.Invoke(this, message);
            });
        }

        public async Task Initialize(string host, string port)
        {
            HostNameString = host;
            PortNumberString = port;
            AppObjects.Instance.SocketService = new SocketServices();
            SocketServices socketService = AppObjects.Instance.SocketService;

            try
            {
                // Socket client delegate settings
                socketService.ConnectComplete += CommandSocketClientConnect_Completed; // Client_ConnectComplete;
                socketService.ReceivePacketComplete += CommandSocketClient_Response; //  Client_ReceivePacketComplete;
                socketService.SendPacketComplete += Client_SendPacketComplete;

                socketService.Connect(System.Net.IPAddress.Parse(host), int.Parse(port));
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("Initialize() Exception: {0}", ex.Message));
            }
        }

        public async Task Close()
        {
            try
            {
                CommandResponseState = CMD_DISPOSE_PUBLISHER;
                await this.SendCommand(CMD_DISPOSE_PUBLISHER);

                if (AppObjects.Instance.SocketService != null)
                {
                    SocketServices socketService = AppObjects.Instance.SocketService;
                    socketService.ConnectComplete -= CommandSocketClientConnect_Completed; // Client_ConnectComplete;
                    socketService.ReceivePacketComplete -= CommandSocketClient_Response; //  Client_ReceivePacketComplete;
                    socketService.SendPacketComplete -= Client_SendPacketComplete;
                }

                if (dataSocketClient != null)
                {
                    dataSocketClient.Disonnect();
                    dataSocketClient.SocketClientConnectCompletedNotification -= DataSocketClientConnect_Completed;
                    dataSocketClient = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Close() Exception: {0}", ex.Message));
            }
        }

        //public void Start()
        //{
        //    // let WacomDevices post data packets
        //    try
        //    {
        //        if (commandSocketClient == null || dataSocketClient == null)
        //        {
        //            throw new Exception("SocketClient object is null.");
        //        }
        //        else
        //        {

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format("Start() Exception: {0}", ex.Message));
        //    }
        //}

        public async void Stop()
        {
            // let WacomDevices terminate data transmission
            // ToDo

            // request Broker stop the communication
            CommandResponseState = CMD_STOP_PUBLISHER;
            await this.SendCommand(CMD_STOP_PUBLISHER);
        }

        const float CMD_NEUTRAL = 0x0000;
        const float CMD_REQUEST_PUBLISHER_CONNECTION = 0x1000;
        const float CMD_SET_ATTRIBUTES = 0x2000;
        const float CMD_START_PUBLISHER = 0x3000;
        const float CMD_STOP_PUBLISHER = 0x4000;
        const float CMD_DISPOSE_PUBLISHER = 0x5000;
        const string RES_ACK = "ack";
        const string RES_NAK = "nak";

        public async Task SendCommand(float command)
        {
            try
            {
                SocketServices socketService = AppObjects.Instance.SocketService;

                // send command packets
                if (socketService != null)
                {
                    string commandString;

                    switch (command)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            commandString = string.Format("{0},{1},{2}", 0, 1,
                                AppObjects.Instance.Token);
                            break;

                        case CMD_SET_ATTRIBUTES:
                            commandString = string.Format("{0},{1},{2}", PublisherId, 2,
                                AppObjects.Instance.WacomDevice.Attribute.GenerateStrings());
                            break;

                        case CMD_START_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 3);
                            break;

                        case CMD_STOP_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 4);
                            break;

                        case CMD_DISPOSE_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 5);
                            break;

                        default:
                            commandString = string.Empty;
                            break;
                    }

                    CommandResponseState = command;
                    socketService.SendToServer(System.Text.Encoding.UTF8.GetBytes(commandString));

                    // Then, waiting for response string at CommandSocketClient_Response
                    //                    await commandSocketClient.ResponseReceiveAsync();
                    //                   commandSocketClient.ResponseReceive();
                }
                else
                {
                    throw new Exception("SocketClient object is null.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SendCommand Exception: {0}", ex.Message));
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

        #region Delegate Completion Handlers
        private async void CommandSocketClientConnect_Completed(object sender, SocketErrorEventArgs e)
        {
            MessageEvent("CommandSocketClientConnect_Completed.");

            if (e.Error != System.Net.Sockets.SocketError.Success)
            {
                MessageEvent(string.Format("Command connection error: {0}", e.Error));
                return;
            }
            else
            {
                CommandResponseState = CMD_REQUEST_PUBLISHER_CONNECTION;
                await this.SendCommand(CMD_REQUEST_PUBLISHER_CONNECTION);
            }
        }

        private async void DataSocketClientConnect_Completed(object sender, bool result)
        {
            MessageEvent("DataSocketClientConnect_Completed.");

            if (result && this.PublisherId != 0)
            {
                // Get ready to do all
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.InitializationCompletedNotification?.Invoke(this, true);
                });
            }
            else
            {

            }
        }
        #endregion

        #region Delegate Event Handlers
        private async void CommandSocketClient_Response(object sender, ReceivePacketEventArgs e)
        {
            try
            {
                if (e.Error != System.Net.Sockets.SocketError.Success)
                {
                    // 受信に失敗
                    MessageEvent(string.Format("packet receive error: {0}", e.Error));
                    return;
                }

                // 受信したデータをテキストに変換
                string response = System.Text.Encoding.UTF8.GetString(e.Data);
                MessageEvent(string.Format("packet received : {0}", response));

                switch (CommandResponseState)
                {
                    case CMD_REQUEST_PUBLISHER_CONNECTION:
                        {
                            this.PublisherId = float.Parse(response);
                            AppObjects.Instance.WacomDevice.PublisherAttribute
                                = (uint)AppObjects.Instance.WacomDevice.PublisherAttribute | (uint)this.PublisherId;
                            MessageEvent(string.Format("CommandSocketClient_Response: response = {0}, PublisherAttribute = {1}",
                                response, AppObjects.Instance.WacomDevice.PublisherAttribute));

                            this.CommandResponseState = CMD_REQUEST_PUBLISHER_CONNECTION;
                            //await commandSocketClient.SendCommand(RES_ACK);
                            //MessageEvent("Send ACK");

                            await this.SendCommand(CMD_SET_ATTRIBUTES);
                        }
                        break;

                    case CMD_SET_ATTRIBUTES:
                        // ACK/NAK
                        switch (response)
                        {
                            case RES_ACK:
                                // Establish the data path
                                string port = (int.Parse(PortNumberString) + 1).ToString();
                                dataSocketClient = AppObjects.Instance.SocketService;    // share with WacomDevices
                                dataSocketClient.SocketClientConnectCompletedNotification += DataSocketClientConnect_Completed;
                                await dataSocketClient.StreamSocket_Connect(HostNameString, port);
                                this.CommandResponseState = CMD_NEUTRAL;

                                break;

                            case RES_NAK:
                                throw new Exception("CMD_SET_ATTRIBUTES returns NAK");
                            //                                break;

                            default:
                                break;
                        }
                        break;

                    case CMD_START_PUBLISHER:
                        // ACK/NAK
                        switch (response)
                        {
                            case RES_ACK:
                                this.CommandResponseState = CMD_NEUTRAL;
                                break;

                            case RES_NAK:
                                throw new Exception("CMD_START_PUBLISHER returns NAK");
                            //                                break;

                            default:
                                break;
                        }
                        break;

                    case CMD_STOP_PUBLISHER:
                        // ACK/NAK
                        switch (response)
                        {
                            case RES_ACK:
                                this.CommandResponseState = CMD_NEUTRAL;
                                break;

                            case RES_NAK:
                                throw new Exception("CMD_STOP_PUBLISHER returns NAK");
                            //                               break;

                            default:
                                break;
                        }
                        break;

                    case CMD_DISPOSE_PUBLISHER:
                        // ACK/NAK
                        switch (response)
                        {
                            case RES_ACK:
                                this.CommandResponseState = CMD_NEUTRAL;
                                break;

                            case RES_NAK:
                                throw new Exception("CMD_DISPOSE_PUBLISHER returns NAK");
                            //                               break;

                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CommandSocketClient_Response: Exception: {0}", ex.Message));
            }
        }

        private async void Client_SendPacketComplete(object sender, SocketErrorEventArgs e)
        {

        }

        #endregion
    }
}
