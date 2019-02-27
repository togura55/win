using System;
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
        public int PublisherCurrentState;
        public readonly int PUBLISHER_STATE_NEUTRAL = 0;
        public readonly int PUBLISHER_STATE_ACTIVE = 1;
        public readonly int PUBLISHER_STATE_IDLE = 2;

        string CommandState = string.Empty;
        public bool DeviceStarted;
        string Width = String.Empty;
        string Height = String.Empty;
        string PointSize = String.Empty;
        string DeviceName = String.Empty;
        string ESN = String.Empty;
        string Battery = String.Empty;
        string DeviceType = String.Empty;
        string TransferMode = String.Empty;
        public string ServerIpAddress = String.Empty;
        public string ServerPortNumberBase = String.Empty;
        public string DeviceState = String.Empty;
        public string ClientIpAddress = String.Empty;
        public string DeviceVersionNumber = String.Empty;

        public DeviceWatcher deviceWatcher = null;

        public RfCommunications rfComm = null;

        public WdControllers()
        {
            PublisherCurrentState = PUBLISHER_STATE_NEUTRAL;

            CommandState = CMD_NEUTRAL;
            DeviceState = "false";
            DeviceStarted = false;

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

        private const string CMD_NEUTRAL = "neutral";

        private const string CMD_GETCONFIG = "getconfig";
        private const string CMD_SETCONFIG = "setconfig";  // setconfig,aaa,bbb,ccc
        private const string CMD_GETVERSION = "getversion";
        private const string CMD_START = "start";
        private const string CMD_STOP = "stop";
        private const string CMD_DISCARD = "discard";
        private const string CMD_RESTART = "restart";
        private const string CMD_POWEROFF = "poweroff";

        private const string RES_ACK = "ack";
        private const string RES_NAK = "nak";

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

        public async Task GetConfig()
        {
            CommandState = CMD_GETCONFIG;
            await rfComm.SendCommand(CMD_GETCONFIG);
        }

        public async Task DeviceStart()
        {
            await rfComm.SendCommand(CMD_START);
        }

        public async Task DeviceStop()
        {
            await rfComm.SendCommand(CMD_STOP);
        }

        public async Task GetVersion()
        {
            CommandState = CMD_GETVERSION;
            await rfComm.SendCommand(CMD_GETVERSION);
        }

        public async Task DeviceDiscard()
        {
            CommandState = CMD_DISCARD;
            await rfComm.SendCommand(CMD_DISCARD);
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

        #endregion

        private void ResponseDispatcher(object sender, string message)
        {
            try
            {
                string res = string.Empty;

                switch (CommandState)
                {
                    case CMD_START:
                        DeviceState = message.Equals((string)RES_ACK) ? "true" : "false";
                        ActionEvent("UpdateUI");
                        break;

                    case CMD_STOP:
                        DeviceState = message.Equals((string)RES_ACK) ? "false" : "true";
                        ActionEvent("UpdateUI");
                        break;

                    case CMD_GETCONFIG:
                        char sp = ','; // separater
                        string[] arr = message.Split(sp);
                        var list = new List<string>();
                        list.AddRange(arr);

                        // decode
                        if (list.Count < 11)
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
                            DeviceType = list[++i];
                            TransferMode = list[++i];
                            ServerIpAddress = list[++i];
                            ServerPortNumberBase = list[++i];
                            DeviceState = list[++i];
                            ClientIpAddress = list[++i];    // added 1.0.2

                            ActionEvent("UpdateUI");
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
                        }
                        break;

                    case CMD_GETVERSION:
                        DeviceVersionNumber = message;
                        ActionEvent("UpdateUI");
                        break;

                    case CMD_DISCARD:
                        break;

                    case CMD_RESTART:
                        break;

                    case CMD_POWEROFF:
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageEvent(string.Format("CommandsDispatcher: Exception: {0}", ex.Message));
            }
        }
    }
}
