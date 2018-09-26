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

        public void initialize()
        {
            try
            {
                // make a command path
                commandSocketClient = new SocketClient();
                commandSocketClient.SocketClientConnectCompletedNotification += CommandSocketClientConnect_Completed;

                // make a data path
                dataSocketClient = new SocketClient();
                dataSocketClient.SocketClientConnectCompletedNotification += DataSocketClientConnect_Completed;
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
    }
}
