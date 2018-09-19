using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using System.Diagnostics;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace BtClient
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Initialize();
        }

        Windows.Devices.Bluetooth.Rfcomm.RfcommDeviceService _service;
        Windows.Networking.Sockets.StreamSocket _socket;

        async void Initialize()
        {
            try
            {
                // Enumerate devices with the object push service
                var services =
                    await DeviceInformation.FindAllAsync(
                        RfcommDeviceService.GetDeviceSelector(
                            RfcommServiceId.ObexObjectPush));

                if (services.Count > 0)
                {
                    // Initialize the target Bluetooth BR device
                    var service = await RfcommDeviceService.FromIdAsync(services[0].Id);

                    // Check that the service meets this App's minimum requirement
                    if (SupportsProtection(service) && await IsCompatibleVersion(service))
                    {
                        _service = service;

                        // Create a socket and connect to the target
                        _socket = new StreamSocket();
                        await _socket.ConnectAsync(
                            _service.ConnectionHostName,
                            _service.ConnectionServiceName,
                            SocketProtectionLevel
                                .BluetoothEncryptionAllowNullAuthentication);

                        // The socket is connected. At this point the App can wait for
                        // the user to take some action, e.g. click a button to send a
                        // file to the device, which could invoke the Picker and then
                        // send the picked file. The transfer itself would use the
                        // Sockets API and not the Rfcomm API, and so is omitted here for
                        // brevity.
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}", ex.Message));
            }
        }

        // This App requires a connection that is encrypted but does not care about
        // whether it's authenticated.
        bool SupportsProtection(RfcommDeviceService service)
        {
            try
            {
                switch (service.ProtectionLevel)
                {
                    case SocketProtectionLevel.PlainSocket:
                        if ((service.MaxProtectionLevel == SocketProtectionLevel
                                .BluetoothEncryptionWithAuthentication)
                            || (service.MaxProtectionLevel == SocketProtectionLevel
                                .BluetoothEncryptionAllowNullAuthentication))
                        {
                            // The connection can be upgraded when opening the socket so the
                            // App may offer UI here to notify the user that Windows may
                            // prompt for a PIN exchange.
                            return true;
                        }
                        else
                        {
                            // The connection cannot be upgraded so an App may offer UI here
                            // to explain why a connection won't be made.
                            return false;
                        }
                    case SocketProtectionLevel.BluetoothEncryptionWithAuthentication:
                        return true;
                    case SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication:
                        return true;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}", ex.Message));
            }

            return false;
        }

        // This App relies on CRC32 checking available in version 2.0 of the service.
        const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;   // UINT32
        const uint MINIMUM_SERVICE_VERSION = 200;

        async Task<bool> IsCompatibleVersion(RfcommDeviceService service)
        {
            try
            {
                var attributes = await service.GetSdpRawAttributesAsync(
                    BluetoothCacheMode.Uncached);
                var attribute = attributes[SERVICE_VERSION_ATTRIBUTE_ID];
                var reader = DataReader.FromBuffer(attribute);

                // The first byte contains the attribute's type
                byte attributeType = reader.ReadByte();
                if (attributeType == SERVICE_VERSION_ATTRIBUTE_TYPE)
                {
                    // The remainder is the data
                    uint version = reader.ReadUInt32();
                    return version >= MINIMUM_SERVICE_VERSION;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}", ex.Message));
            }

            return false;
        }
    }
}
