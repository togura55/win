﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;

namespace WdController
{
    public class WdControllers
    {
        public string BleDeviceId;

        public int State;
        public readonly int STATE_NEUTRAL = 0; // initial state
        public readonly int STATE_READY = 1;   // Scan BLE completed
        public readonly int STATE_ACTIVE = 2;  // under handling BLE device


        //        public int PublisherCurrentState;
        public readonly int PUBLISHER_STATE_NEUTRAL = 0;
        public readonly int PUBLISHER_STATE_ACTIVE = 1;
        public readonly int PUBLISHER_STATE_IDLE = 2;

        string CommandState;
        public bool DeviceStarted;
        public bool DeviceSuspended;
        string Width;
        string Height;
        string PointSize;
        string DeviceName;
        string ESN;
        string Battery;
        string FirmwareVersion;
        string DeviceType;
        string TransferMode;
        string Barcode;
        public string ServerIpAddress;
        public string ServerPortNumberBase;
        public int DeviceState;
        public string ClientIpAddress;
        public string DeviceVersionNumber;

        public string Logs;

        public DeviceWatcher deviceWatcher = null;

        public RfCommunications rfComm = null;

        public WdControllers()
        {
            Reset(); // reset all parameters as default

            rfComm = new RfCommunications();

            rfComm.RfCommResponseDispatcher += ResponseDispatcher;
        }

        // Delegate handlers
        public delegate void MessageEventHandler(object sender, string message);

        // Properties
        public event MessageEventHandler WdControllerMessage;
        public event MessageEventHandler WdControllerAction;

        private async void MessageEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.WdControllerMessage?.Invoke(this, message);
            });
        }
        private async void ActionEvent(string message)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.WdControllerAction?.Invoke(this, message);
            });
        }

        #region Services
        public void Reset()
        {
            BleDeviceId = string.Empty;

            CommandState = CMD_NEUTRAL;
            DeviceState = PUBLISHER_STATE_NEUTRAL;
            DeviceStarted = false;
            DeviceSuspended = false;
            Width = String.Empty;
            Height = String.Empty;
            PointSize = String.Empty;
            DeviceName = String.Empty;
            ESN = String.Empty;
            Battery = String.Empty;
            DeviceType = String.Empty;
            TransferMode = String.Empty;
            ServerIpAddress = String.Empty;
            ServerPortNumberBase = String.Empty;
            ClientIpAddress = String.Empty;
            DeviceVersionNumber = String.Empty;

            if (rfComm != null)
            {
                rfComm.BtDeviceName = string.Empty;
                rfComm.BtServiceName = string.Empty;
            }
        }

        public async Task Connect(string deviceId)
        {
            await rfComm.RfConnect(deviceId);
        }

        public void Disconnect()
        {
            rfComm.RfDisconnect("");
        }

        public async Task RequestAccess()
        {
            await rfComm.RfRequestAccess();
        }

        public void StopWatcher()
        {
            if (null != deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status))
                {
                    deviceWatcher.Stop();
                }
                deviceWatcher = null;
            }
        }

        private const string CMD_NEUTRAL = "neutral";

        private const string CMD_GETCONFIG = "getconfig";
        private const string CMD_SETCONFIG = "setconfig";  // setconfig,aaa,bbb,ccc
        private const string CMD_GETVERSION = "getversion";
        private const string CMD_START = "start";       // Publisher state
        private const string CMD_STOP = "stop";         // Publisher state
        private const string CMD_SUSPEND = "suspend";   // Publisher state
        private const string CMD_RESUME = "resume";     // Publisher state
        private const string CMD_RESTART = "restart";
        private const string CMD_POWEROFF = "poweroff";
        private const string CMD_GETLOGS = "getlogs";
        private const string CMD_GETBARCODE = "getbarcode";

        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";

        public async Task GetConfig()
        {
            CommandState = CMD_GETCONFIG;
            await rfComm.SendCommand(CMD_GETCONFIG);
        }

        public async Task SetConfig(string parameters)
        {
            string command = CMD_SETCONFIG;
            try
            {
                CommandState = CMD_SETCONFIG;
                await rfComm.SendCommand(command + parameters);
            }
            catch (Exception ex)
            {
                MessageEvent(String.Format("Pbtn_SetConfig_Click: Exception: {0}", ex.Message));
            }
        }

        public async Task GetVersion()
        {
            CommandState = CMD_GETVERSION;
            await rfComm.SendCommand(CMD_GETVERSION);
        }

        public async Task DeviceStart()
        {
            CommandState = CMD_START;
            await rfComm.SendCommand(CMD_START);
        }

        public async Task DeviceStop()
        {
            CommandState = CMD_STOP;
            await rfComm.SendCommand(CMD_STOP);
        }

        public async Task DeviceSuspend()
        {
            CommandState = CMD_SUSPEND;
            await rfComm.SendCommand(CMD_SUSPEND);
        }

        public async Task DeviceResume()
        {
            CommandState = CMD_RESUME;
            await rfComm.SendCommand(CMD_RESUME);
        }

        public async Task DeviceRestart()
        {
            CommandState = CMD_RESTART;
            await rfComm.SendCommand(CMD_RESTART);
        }

        public async Task DevicePoweroff()
        {
            CommandState = CMD_POWEROFF;
            await rfComm.SendCommand(CMD_POWEROFF);
        }

        public async Task GetLogs()
        {
            CommandState = CMD_GETLOGS;
            await rfComm.SendCommand(CMD_GETLOGS);
        }

        public async Task GetBarcode()
        {
            CommandState = CMD_GETBARCODE;
            await rfComm.SendCommand(CMD_GETBARCODE);
        }

        #endregion

        private List<string> SplitArgument(char sp, string message)
        {
            var list = new List<string>();

            string[] arr = message.Split(sp);
            list.AddRange(arr);

            return list;
        }

        private void ResponseDispatcher(object sender, string message)
        {
            try
            {
                string res = string.Empty;

                switch (CommandState)
                {
                    case CMD_GETCONFIG:
                        {
                            var list = SplitArgument(',', message);

                            // decode
                            if (list.Count < 13)   // ToDo: should be set by enum
                            {
                                // error, resend?
                                throw new Exception("GetConfig returns the smaller number of parameters.");
                            }
                            else
                            {
                                int i = -1;

                                Width = list[++i];
                                Height = list[++i];
                                PointSize = list[++i];
                                DeviceName = list[++i];
                                ESN = list[++i];
                                Battery = list[++i];
                                FirmwareVersion = list[++i];       // added 1.1
                                DeviceType = list[++i];
                                TransferMode = list[++i];
                                Barcode = list[++i];               // added 1.1
                                ServerIpAddress = list[++i];
                                ServerPortNumberBase = list[++i];
                                DeviceState = int.Parse(list[++i]);
                                ClientIpAddress = list[++i];    // added 1.0.2
                            }
                        }
                        break;

                    case CMD_SETCONFIG:
                        if (message.Equals((string)RES_ACK))
                        {
                            // ok
                        }
                        else
                        {
                            // error
                            throw new Exception("SetConfig returns NAK.");
                        }
                        break;

                    case CMD_GETVERSION:
                        DeviceVersionNumber = message;
                        break;

                    case CMD_START:
                        DeviceState = message.Equals((string)RES_ACK) ?
                            PUBLISHER_STATE_ACTIVE : PUBLISHER_STATE_NEUTRAL;
                        break;

                    case CMD_STOP:
                        DeviceState = message.Equals((string)RES_ACK) ?
                            PUBLISHER_STATE_NEUTRAL : PUBLISHER_STATE_ACTIVE; ;
                        break;

                    case CMD_SUSPEND:
                        DeviceState = message.Equals((string)RES_ACK) ?
                            PUBLISHER_STATE_IDLE : PUBLISHER_STATE_ACTIVE; ;
                        break;

                    case CMD_RESUME:
                        DeviceState = message.Equals((string)RES_ACK) ?
                            PUBLISHER_STATE_ACTIVE : PUBLISHER_STATE_IDLE; ;
                        break;

                    case CMD_RESTART:
                        DeviceState = message.Equals((string)RES_ACK) ?
                            PUBLISHER_STATE_NEUTRAL : DeviceState;
                        break;

                    case CMD_POWEROFF:
                        DeviceState = message.Equals((string)RES_ACK) ?
                            PUBLISHER_STATE_NEUTRAL : DeviceState;
                        break;

                    case CMD_GETLOGS:
                        MessageEvent(message);
                        break;

                    case CMD_GETBARCODE:
                        Barcode = message;
                        break;
                    default:
                        break;
                }
                ActionEvent("UpdateUI");
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("ResponseDispatcher: Exception: {0}", ex.Message));
            }
        }
    }
}
