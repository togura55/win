using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WillDevicesSampleApp
{
    public class WdPublishComm
    {
        public SocketClient commandSocketClient = null;
        public SocketClient dataSocketClient = null;

        //public StreamSocket commandStreamSocket = null;
        //public StreamSocket dataStreamSocket = null;

        public WdPublishComm()
        {

        }

        public void Initialize()
        {
            try
            {
                // make a command path
                commandSocketClient = new SocketClient();
                commandSocketClient.SocketClientConnectCompletedNotification += CommandSocketClientConnect_Completed;
                commandSocketClient.SocketClientReceivedResponse += CommandSocketClient_Response;
                commandSocketClient.CommandHostNameString = "";
                commandSocketClient.CommandPortNumberString = "";
                commandSocketClient.Connect();

                // make a data path
                dataSocketClient = new SocketClient();
                dataSocketClient.SocketClientConnectCompletedNotification += DataSocketClientConnect_Completed;
                dataSocketClient.HostNameString = "";
                dataSocketClient.PortNumberString = "";
                dataSocketClient.Connect();
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
                    throw new Exception("SocketClient object is null." );
                }
                else
                {

                }
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Start() Exception: {0}", ex.Message));
            }
        }

        public void Stop()
        {
            // let WacomDevices terminate data transmission
        }

        public void Send(int command)
        {
            try
            {
                // send command packets
                if (commandSocketClient != null)
                {
                    switch (command)
                    {
                        case 1:
                            commandSocketClient.BatchedSends(RequestPublisherConnection());
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

        private IBuffer RequestPublisherConnection()
        {
            IBuffer buffer = null;

            return buffer;
        }

        #region Delegate Completion Handlers
        private async void CommandSocketClientConnect_Completed(object sender, bool result)
        {
            if (result)
            {
                IBuffer buff_command = null;
//              buff_command = CreateCommandPacket(COMMAND_REQUESTPUBLISHERCONNECTION);
                commandSocketClient.BatchedSends(buff_command);
            }
            else
            {

            }
        }

        private async void CommandSocketClient_Response(object sender, bool result)
        {

        }

        private async void DataSocketClientConnect_Completed(object sender, bool result)
        {
            if (result)
            {

            }
            else
            {

            }
        }

        #endregion
    }
}
