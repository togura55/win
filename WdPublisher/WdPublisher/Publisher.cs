﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace WillDevicesSampleApp
{
    public class Publisher
    {
        public Socket mSocket = null;

        public SocketServices dataSocketClient = null;

        float CommandResponseState;
        float PublisherId;

        public string HostNameString = string.Empty;
        public string PortNumberString = string.Empty;
        public string ClientIpAddress = string.Empty;
        public int CurrentState;  // State of Publisher 

        public readonly int STATE_NEUTRAL = 0;
        public readonly int STATE_ACTIVE = 1;
        public readonly int STATE_IDLE = 2;

        // Delegate handlers
        public delegate void MessageEventHandler(object sender, string message);
        public delegate void InitializationCompletedNotificationHandler(object sender, bool result);

        // Properties
        public event MessageEventHandler PublisherMessage, UpdateUi;
        public event InitializationCompletedNotificationHandler InitializationCompletedNotification;

        public Publisher()
        {
            CommandResponseState = CMD_NEUTRAL;
            PublisherId = 0;
            HostNameString = "192.168.0.7";
            PortNumberString = "1337";
            //            State = PUBLISHER_STATE_STOP;
            CurrentState = STATE_NEUTRAL;
            HostName hostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();
            ClientIpAddress = hostName.ToString();
        }

        private string GeneratePublisherAttributeStrings()
        {
            string s = ClientIpAddress + "," + CurrentState.ToString();

            return s;
        }

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.PublisherMessage?.Invoke(this, message);
            });
        }

        private async void UpdateUiEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.UpdateUi?.Invoke(this, message);
            });
        }
        //        private async Task InitCommandCommunication(string host, string port)
        private void InitCommandCommunication(string host, string port)
        {
            if (AppObjects.Instance.SocketService == null)
            {
                throw new Exception("SocketServices is not created yet.");
            }

            SocketServices socketService = AppObjects.Instance.SocketService;

            try
            {
                // Socket for commands delegate settings
                socketService.ConnectComplete += CommandSocketClient_Connect_Completed; // Client_ConnectComplete;
                socketService.ReceivePacketComplete += CommandSocketClient_Response; //  Client_ReceivePacketComplete;
                socketService.SendPacketComplete += CommandSocketClient_SendPacket_Completed;

                socketService.Connect(System.Net.IPAddress.Parse(host), int.Parse(port));
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("InitCommandCommunication: Exception: {0}", ex.Message));
            }
        }

        private async Task InitDataCommunication(string host, string base_port)
        {
            try
            {
                string port = (int.Parse(base_port) + 1).ToString();
                dataSocketClient = AppObjects.Instance.SocketService;    // share with WacomDevices
                dataSocketClient.SocketClientConnectCompletedNotification += DataSocketClient_Connect_Completed;
                await dataSocketClient.StreamSocket_Connect(host, port);
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("InitDataCommunication: Exception: {0}", ex.Message));
            }
        }

        #region Services
        public void Close()
        {
            try
            {
                MessageEvent("Close");
                CurrentState = STATE_NEUTRAL;

                CommandResponseState = CMD_DISCARD_PUBLISHER;
                this.SendCommandStrings(CMD_DISCARD_PUBLISHER);

                if (AppObjects.Instance.SocketService != null)
                {
                    SocketServices socketService = AppObjects.Instance.SocketService;
                    socketService.ConnectComplete -= CommandSocketClient_Connect_Completed; // Client_ConnectComplete;
                    socketService.ReceivePacketComplete -= CommandSocketClient_Response; //  Client_ReceivePacketComplete;
                    socketService.SendPacketComplete -= CommandSocketClient_SendPacket_Completed;
                }

                if (dataSocketClient != null)
                {
                    dataSocketClient.Disonnect();
                    dataSocketClient.SocketClientConnectCompletedNotification -= DataSocketClient_Connect_Completed;
                    dataSocketClient = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Close() Exception: {0}", ex.Message));
            }
        }

        public void Start()
        {
            try
            {
                MessageEvent("Start");

                // Set task completion delegation 
                WacomDevices wacomDevice = AppObjects.Instance.WacomDevice;
                wacomDevice.ScanAndConnectCompletedNotification += ScanAndConnect_Completed;
                wacomDevice.StartRealTimeInkCompletedNotification += StartRealTimeInk_Completed;

                // At first, try to detect a device
                wacomDevice.StartScanAndConnect();

                CurrentState = STATE_ACTIVE;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Start: Exception: {0}", ex.Message));
            }
        }

        public async void Stop()
        {
            try
            {
                MessageEvent("Stop");

                // let WacomDevices terminate data transmission
                // ToDo: check if Realtime Ink is ongoing 
                WacomDevices wacomDevice = AppObjects.Instance.WacomDevice;
                if (wacomDevice.DeviceInfos.Count != 0)
                {
                    await wacomDevice.StopRealTimeInk();
                    wacomDevice.StopScanAndConnect();
                }
                wacomDevice.ScanAndConnectCompletedNotification -= ScanAndConnect_Completed;
                wacomDevice.StartRealTimeInkCompletedNotification -= StartRealTimeInk_Completed;


                if (AppObjects.Instance.Device != null)
                    AppObjects.Instance.Device.Close();

                // request Broker stop the communication
                CommandResponseState = CMD_STOP_PUBLISHER;
                this.SendCommandStrings(CMD_STOP_PUBLISHER);

                CurrentState = STATE_NEUTRAL;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Stop: Exception: {0}", ex.Message));
            }
        }
        #endregion

        const float CMD_NEUTRAL = 0x0000;
        const float CMD_REQUEST_PUBLISHER_CONNECTION = 0x1000;
        const float CMD_SET_ATTRIBUTES = 0x2000;
        const float CMD_START_PUBLISHER = 0x3000;
        const float CMD_STOP_PUBLISHER = 0x4000;
        const float CMD_DISCARD_PUBLISHER = 0x5000;
        const float CMD_RESTART_PUBLISHER = 0x6000;
        const float CMD_POWEROFF_PUBLISHER = 0x7000;

        const string RES_ACK = "ack";
        const string RES_NAK = "nak";

        private void SendCommandStrings(float command)
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
                                AppObjects.Instance.WacomDevice.Attribute.GenerateStrings() +
                                "," +
                                GeneratePublisherAttributeStrings());
                            break;

                        case CMD_START_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 3);
                            break;

                        case CMD_STOP_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 4);
                            break;

                        case CMD_DISCARD_PUBLISHER:
                            commandString = string.Format("{0},{1}", PublisherId, 5);
                            break;

                        default:
                            commandString = string.Empty;
                            break;
                    }

                    CommandResponseState = command;
                    //var task = Task.Run(() =>
                    //{
                        socketService.SendToServer(System.Text.Encoding.UTF8.GetBytes(commandString));
                    //});


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
//        private async void ScanAndConnect_Completed(object sender, bool result)
        private void ScanAndConnect_Completed(object sender, bool result)
        {
            try
            {
                if (result || AppObjects.Instance.Device != null)
                {
                    MessageEvent("ScanAndConnect_Completed: Go Socket initialization");

                    // Second, initialize the command path to Broker
 //                   await InitCommandCommunication(HostNameString, PortNumberString);
                    InitCommandCommunication(HostNameString, PortNumberString);
                }
                else
                {
                    CurrentState = STATE_NEUTRAL;
                    UpdateUiEvent(null);
                    MessageEvent("ScanAndConnect_Completed: Could not be detected devices.");
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("ScanAndConnect_Completed: Exception: {0}", ex.Message));
            }
        }

        private void StartRealTimeInk_Completed(object sender, bool result)
        {
            try
            {
                if (result)  // socket was established
                {
                    CurrentState = STATE_ACTIVE;
                    UpdateUiEvent(null);
                    MessageEvent("StartRealTimeInk_Completed: All pre-process were done.");
                }
                else
                {
                    CurrentState = STATE_NEUTRAL;
                    UpdateUiEvent(null);
                    MessageEvent("StartRealTime_Completed: got false.");
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("StartRealTimeInk_Completed: Exception: {0}", ex.Message));
            }
        }

        private void CommandSocketClient_Connect_Completed(object sender, SocketErrorEventArgs e)
        {
            MessageEvent("CommandSocketClientConnect_Completed.");

            if (e.Error != System.Net.Sockets.SocketError.Success)
            {
                CurrentState = STATE_NEUTRAL;
                UpdateUiEvent(null);
                MessageEvent(string.Format("Command connection error: {0}", e.Error));
                return;
            }
            else
            {
                CommandResponseState = CMD_REQUEST_PUBLISHER_CONNECTION;
                this.SendCommandStrings(CMD_REQUEST_PUBLISHER_CONNECTION);
            }
        }

        private void CommandSocketClient_SendPacket_Completed(object sender, SocketErrorEventArgs e)
        {

        }

        private async void DataSocketClient_Connect_Completed(object sender, bool result)
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
                    MessageEvent(string.Format("packet receive error: {0}", e.Error));
                    return;
                }

                // convert to text
                string response = System.Text.Encoding.UTF8.GetString(e.Data);
                MessageEvent(string.Format("packet received : {0}", response));

                string msg = string.Empty;
                switch (CommandResponseState)
                {
                    case CMD_REQUEST_PUBLISHER_CONNECTION:
                        msg = string.Format("CMD_REQUEST_PUBLISHER_CONNECTION returns {0}", response);
                        {
                            this.PublisherId = float.Parse(response);
                            AppObjects.Instance.WacomDevice.PublisherAttribute
                                = (uint)AppObjects.Instance.WacomDevice.PublisherAttribute | (uint)this.PublisherId;
                            MessageEvent(string.Format("CommandSocketClient_Response: response = {0}, PublisherAttribute = {1}",
                                response, AppObjects.Instance.WacomDevice.PublisherAttribute));

                            this.CommandResponseState = CMD_REQUEST_PUBLISHER_CONNECTION;
                            //await commandSocketClient.SendCommand(RES_ACK);
                            //MessageEvent("Send ACK");

                            this.SendCommandStrings(CMD_SET_ATTRIBUTES);
                        }
                        break;

                    case CMD_SET_ATTRIBUTES:
                        // ACK/NAK
                        msg = string.Format("CMD_SET_ATTRIBUTES returns {0}", response);
                        switch (response)
                        {
                            case RES_ACK:
                                MessageEvent(msg);
                                // Establish the data path
                                await InitDataCommunication(HostNameString, PortNumberString);
                                this.CommandResponseState = CMD_NEUTRAL;

                                break;

                            case RES_NAK:
                                throw new Exception(msg);

                            default:
                                throw new Exception(string.Format("Unknown response: {0}", response));
                        }
                        break;

                    case CMD_START_PUBLISHER:
                        // ACK/NAK
                        msg = string.Format("CMD_START_PUBLISHER returns {0}", response);
                        switch (response)
                        {
                            case RES_ACK:
                                MessageEvent(msg);
                                this.CommandResponseState = CMD_NEUTRAL;
                                break;

                            case RES_NAK:
                                throw new Exception(msg);

                            default:
                                throw new Exception(string.Format("Unknown response: {0}", response));
                        }
                        break;

                    case CMD_STOP_PUBLISHER:
                        // ACK/NAK
                        msg = string.Format("CMD_STOP_PUBLISHER returns {0}", response);
                        switch (response)
                        {
                            case RES_ACK:
                                MessageEvent(msg);
                                this.CommandResponseState = CMD_NEUTRAL;
                                break;

                            case RES_NAK:
                                throw new Exception(msg);

                            default:
                                throw new Exception(string.Format("Unknown response: {0}", response));
                        }
                        break;

                    case CMD_DISCARD_PUBLISHER:
                        // ACK/NAK
                        msg = string.Format("CMD_DISPOSE_PUBLISHER returns {0}", response);
                        switch (response)
                        {
                            case RES_ACK:
                                MessageEvent(msg);
                                this.CommandResponseState = CMD_NEUTRAL;
                                break;

                            case RES_NAK:
                                throw new Exception(msg);

                            default:
                                throw new Exception(string.Format("Unknown response: {0}", response));
                        }
                        break;

                    default:
                        throw new Exception(string.Format("Unknown CommandResponseState: {0}", CommandResponseState));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CommandSocketClient_Response: Exception: {0}", ex.Message));
            }
        }
        #endregion
    }
}
