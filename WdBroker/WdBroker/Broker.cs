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

        // Properties
        public event BrokerEventHandler BrokerMessage;

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
            msg += " From Server";
            mServerSock.SendToClient(System.Text.Encoding.UTF8.GetBytes(msg));
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
    }
}
