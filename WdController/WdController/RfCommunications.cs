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
        private DataWriter dataWriter = null;
        private RfcommDeviceService rfcommDeviceService = null;
        public string BtServiceName = string.Empty;
        public string BtDeviceName = string.Empty;
        private StreamSocket streamSocket = null;

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
                rfcommDeviceService = rfcommServices.Services[0];
            }
            else
            {
                MessageEvent(
                   "Could not discover the WdX service on the remote device");
                //               ResetMainUI();
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await rfcommDeviceService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Constants.SdpServiceNameAttributeId))
            {
                MessageEvent(
                    "The WdX service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcomm server.");
                //                ResetMainUI();
                return;
            }
            var attributeReader = DataReader.FromBuffer(attributes[Constants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Constants.SdpServiceNameAttributeType)
            {
                MessageEvent(
                    "The WdX service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcomm server.");
                //                ResetMainUI();
                return;
            }
            var serviceNameLength = attributeReader.ReadByte();

            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            ActionEvent("StopWatcher"); //  StopWatcher();

            lock (this)
            {
                streamSocket = new StreamSocket();
            }
            try
            {
                await streamSocket.ConnectAsync(rfcommDeviceService.ConnectionHostName, rfcommDeviceService.ConnectionServiceName);

                BtDeviceName = bluetoothDevice.Name;
                BtServiceName = attributeReader.ReadString(serviceNameLength);
                ActionEvent("EnableControlUI"); //  SetChatUI(BtServiceName, BtDeviceName);
                dataWriter = new DataWriter(streamSocket.OutputStream);

                DataReader dataReader = new DataReader(streamSocket.InputStream);
                MessageEvent("RfConnect: Connection established. Set the receive message loop.");
                ReceiveStringLoop(dataReader);
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
            if (dataWriter != null)
            {
                dataWriter.DetachStream();
                dataWriter = null;
            }

            if (rfcommDeviceService != null)
            {
                rfcommDeviceService.Dispose();
                rfcommDeviceService = null;
            }
            lock (this)
            {
                if (streamSocket != null)
                {
                    streamSocket.Dispose();
                    streamSocket = null;
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
                    dataWriter.WriteUInt32((uint)command.Length);
                    dataWriter.WriteString(command);

                    //                    ConversationList.Items.Add("Sent: " + MessageTextBox.Text);
                    MessageEvent("Sent: " + command);
                    //                    TextBox_Message.Text = "";
                    await dataWriter.StoreAsync();

                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                MessageEvent("Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message);
            }
        }
        #endregion

        private async void ReceiveStringLoop(DataReader dataReader)
        {
            try
            {
                uint size = await dataReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    RfDisconnect("Remote device terminated connection - make sure only one instance of server is running on remote device");
                    return;
                }

                uint stringLength = dataReader.ReadUInt32();
                uint actualStringLength = await dataReader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                string readString = dataReader.ReadString(stringLength);
                //                ConversationList.Items.Add("Received: " + dataReader.ReadString(stringLength));
                MessageEvent("Received: " + readString);

                // handle responses sent by BLE server
                ResponseDispatcher(readString);

                ReceiveStringLoop(dataReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (streamSocket == null)
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
