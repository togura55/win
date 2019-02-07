using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace WdController
{
    public class RfCommunications
    {
        private BluetoothDevice bluetoothDevice = null;
        private DataWriter chatWriter = null;
        private RfcommDeviceService chatService = null;
        public string BleServiceName = string.Empty;
        public string BleDeviceName = string.Empty;
        private StreamSocket chatSocket = null;

        public RfCommunications()
        {

        }

        // Delegate handlers
        public delegate void MessageEventHandler(object sender, string message);

        // Properties
        public event MessageEventHandler RfCommMessage;
        public event MessageEventHandler RfCommAction;
        public event MessageEventHandler RfCommResponseDispatcher;

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.RfCommMessage?.Invoke(this, message);
            });
        }
        private async void ActionEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.RfCommAction?.Invoke(this, message);
            });
        }
        private void ResponseDispatcher(string message)
        {
//            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//            {
                this.RfCommResponseDispatcher?.Invoke(this, message);
//            });
        }

        #region Services
        public async Task RfConnect(string deviceId)
        {
            // Perform device access checks before trying to get the device.
            // First, we check if consent has been explicitly denied by the user.
            DeviceAccessStatus accessStatus = DeviceAccessInformation.CreateFromId(deviceId).CurrentStatus;
            if (accessStatus == DeviceAccessStatus.DeniedByUser)
            {
                MessageEvent("This app does not have access to connect to the remote device (please grant access in Settings > Privacy > Other Devices");
                return;
            }
            // If not, try to get the Bluetooth device
            try
            {
                bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceId);
            }
            catch (Exception ex)
            {
                MessageEvent(ex.Message);
                //               ResetMainUI();
                return;
            }
            // If we were unable to get a valid Bluetooth device object,
            // it's most likely because the user has specified that all unpaired devices
            // should not be interacted with.
            if (bluetoothDevice == null)
            {
                MessageEvent("Bluetooth Device returned null. Access Status = " + accessStatus.ToString());
            }

            // This should return a list of uncached Bluetooth services (so if the server was not active when paired, it will still be detected by this call
            var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(
                RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid), BluetoothCacheMode.Uncached);

            if (rfcommServices.Services.Count > 0)
            {
                chatService = rfcommServices.Services[0];
            }
            else
            {
                MessageEvent(
                   "Could not discover the chat service on the remote device");
                //               ResetMainUI();
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Constants.SdpServiceNameAttributeId))
            {
                MessageEvent(
                    "The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                //                ResetMainUI();
                return;
            }
            var attributeReader = DataReader.FromBuffer(attributes[Constants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Constants.SdpServiceNameAttributeType)
            {
                MessageEvent(
                    "The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.");
                //                ResetMainUI();
                return;
            }
            var serviceNameLength = attributeReader.ReadByte();

            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            ActionEvent("StopWatcher"); //  StopWatcher();

            lock (this)
            {
                chatSocket = new StreamSocket();
            }
            try
            {
                await chatSocket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName);

                BleDeviceName = bluetoothDevice.Name;
                BleServiceName = attributeReader.ReadString(serviceNameLength);
                ActionEvent("SetChatUI"); //  SetChatUI(BleServiceName, BleDeviceName);
                chatWriter = new DataWriter(chatSocket.OutputStream);

                DataReader chatReader = new DataReader(chatSocket.InputStream);
                MessageEvent("RfConnect: Connection established. Set the receive message loop.");
                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80070490) // ERROR_ELEMENT_NOT_FOUND
            {
                MessageEvent("Please verify that you are running the BluetoothRfcommChat server.");
                //               ResetMainUI();
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072740) // WSAEADDRINUSE
            {
                MessageEvent("Please verify that there is no other RFCOMM connection to the same device.");
                //                ResetMainUI();
            }
        }

        /// <summary>
        /// Cleans up the socket and DataWriter and reset the UI
        /// </summary>
        /// <param name="disconnectReason"></param>
        public void RfDisconnect(string disconnectReason)
        {
            if (chatWriter != null)
            {
                chatWriter.DetachStream();
                chatWriter = null;
            }

            if (chatService != null)
            {
                chatService.Dispose();
                chatService = null;
            }
            lock (this)
            {
                if (chatSocket != null)
                {
                    chatSocket.Dispose();
                    chatSocket = null;
                }
            }

            MessageEvent(disconnectReason);
            //            ResetMainUI();
        }

        public async Task RfRequestAccess()
        {
            // Make sure user has given consent to access device
            DeviceAccessStatus accessStatus = await bluetoothDevice.RequestAccessAsync();

            if (accessStatus != DeviceAccessStatus.Allowed)
            {
                MessageEvent(
                    "Access to the device is denied because the application was not granted access");
            }
            else
            {
                MessageEvent("Access granted, you are free to pair devices");
            }
        }

        public async Task SendCommand(string command)
        {
            try
            {
                if (command.Length != 0)
                {
                    chatWriter.WriteUInt32((uint)command.Length);
                    chatWriter.WriteString(command);

                    //                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                    MessageEvent("Sent: " + command);
                    //                    TextBox_Message.Text = "";
                    await chatWriter.StoreAsync();

                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                MessageEvent("Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message);
            }
        }
        #endregion

        private async void ReceiveStringLoop(DataReader chatReader)
        {
            try
            {
                uint size = await chatReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    RfDisconnect("Remote device terminated connection - make sure only one instance of server is running on remote device");
                    return;
                }

                uint stringLength = chatReader.ReadUInt32();
                uint actualStringLength = await chatReader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                string readString = chatReader.ReadString(stringLength);
                //                ConversationList.Items.Add("Received: " + chatReader.ReadString(stringLength));
                MessageEvent("Received: " + readString);

                // handle responses sent by BLE server
                ResponseDispatcher(readString);

                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (chatSocket == null)
                    {
                        // Do not print anything here -  the user closed the socket.
                        if ((uint)ex.HResult == 0x80072745)
                            MessageEvent("Disconnect triggered by remote device");
                        else if ((uint)ex.HResult == 0x800703E3)
                            MessageEvent("The I/O operation has been aborted because of either a thread exit or an application request.");
                    }
                    else
                    {
                        RfDisconnect("ReceiveStringLoop: Exception: Read stream failed with error: " + ex.Message);
                    }
                }
            }
        }
    }
}
