﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public SocketClient commandSocketClient = null;
        public SocketClient dataSocketClient = null;

        float CommandResponse;
        float PublisherId;
        string HostNameString;
        string PortNumberString;

        public delegate void InitializationCompletedNotificationHandler(object sender, bool result);

        // Properties
        public event InitializationCompletedNotificationHandler InitializationCompletedNotification;


        public WdPublishComm()
        {
            CommandResponse = 0;
            PublisherId = 0;
        }

        public async void Initialize(string HostName, string PortNumber)
        {
            HostNameString = HostName;
            PortNumberString = PortNumber;

            try
            {
                // make the command path
                commandSocketClient = new SocketClient();
                commandSocketClient.SocketClientConnectCompletedNotification += CommandSocketClientConnect_Completed;
                commandSocketClient.SocketClientReceivedResponse += CommandSocketClient_Response;
                await commandSocketClient.Connect(HostName, PortNumber);

            }
            catch (Exception ex)
            {

            }
        }

        public void Close()
        {
            try
            {
                if (commandSocketClient != null)
                {
                    commandSocketClient.Disonnect();
                    commandSocketClient.SocketClientConnectCompletedNotification -= CommandSocketClientConnect_Completed;
                    commandSocketClient = null;
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

            }
        }

        public void Start()
        {
            // let WacomDevices post data packets
            try
            {
                if (commandSocketClient == null || dataSocketClient == null)
                {
                    throw new Exception("SocketClient object is null.");
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Start() Exception: {0}", ex.Message));
            }
        }

        public void Stop()
        {
            // let WacomDevices terminate data transmission
        }

        const float CMD_NEUTRAL = 0x0000;
        const float CMD_REQUEST_PUBLISHER_CONNECTION = 0x1000;

        public void Send(float command)
        {
            try
            {
                // send command packets
                if (commandSocketClient != null)
                {
                    switch (command)
                    {
                        case CMD_REQUEST_PUBLISHER_CONNECTION:
                            commandSocketClient.BatchedSends(CreateCommandBuffer(command));
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Receive()
        {
            // receive a command packets

        }

        private IBuffer CreateCommandBuffer(float cmd)
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
        private void CommandSocketClientConnect_Completed(object sender, bool result)
        {
            if (result)
            {
                CommandResponse = CMD_REQUEST_PUBLISHER_CONNECTION;
                this.Send(CMD_REQUEST_PUBLISHER_CONNECTION);
            }
            else
            {

            }
        }

        private async void CommandSocketClient_Response(object sender, float res)
        {
            if (CommandResponse == CMD_REQUEST_PUBLISHER_CONNECTION)
            {
                this.PublisherId = res;
                this.CommandResponse = CMD_NEUTRAL;

                // make the data path
                dataSocketClient = new SocketClient();
                dataSocketClient.SocketClientConnectCompletedNotification += DataSocketClientConnect_Completed;
                await dataSocketClient.Connect(HostNameString, (int.Parse(PortNumberString) + 1).ToString());
            }
        }

        private async void DataSocketClientConnect_Completed(object sender, bool result)
        {
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
    }
}
