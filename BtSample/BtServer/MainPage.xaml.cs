using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using System.Diagnostics;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace BtServer
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

        RfcommServiceProvider _provider;
        StreamSocket _socket;

        async void Initialize()
        {
            try
            {
                // Initialize the provider for the hosted RFCOMM service
                _provider = await RfcommServiceProvider.CreateAsync(RfcommServiceId.ObexObjectPush);

                // Create a listener for this service and start listening
                StreamSocketListener listener = new StreamSocketListener();
                listener.ConnectionReceived += OnConnectionReceivedAsync;
                await listener.BindServiceNameAsync(
                    _provider.ServiceId.AsString(),
                    SocketProtectionLevel
                        .BluetoothEncryptionAllowNullAuthentication);

                // Set the SDP attributes and start advertising
                InitializeServiceSdpAttributes(_provider);
                _provider.StartAdvertising(listener);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}", ex.Message));
            }

        }

        const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;   // UINT32
        const uint SERVICE_VERSION = 200;
        void InitializeServiceSdpAttributes(RfcommServiceProvider provider)
        {
            try
            {
                var writer = new Windows.Storage.Streams.DataWriter();

                // First write the attribute type
                writer.WriteByte(SERVICE_VERSION_ATTRIBUTE_TYPE);
                // Then write the data
                writer.WriteUInt32(SERVICE_VERSION);

                var data = writer.DetachBuffer();
                provider.SdpRawAttributes.Add(SERVICE_VERSION_ATTRIBUTE_ID, data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}", ex.Message));
            }
        }

        void OnConnectionReceivedAsync(
            StreamSocketListener listener,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                // Stop advertising/listening so that we're only serving one client
                _provider.StopAdvertising();
                listener.Dispose();  // await listener.Close();
                _socket = args.Socket;

                // The client socket is connected. At this point the App can wait for
                // the user to take some action, e.g. click a button to receive a file
                // from the device, which could invoke the Picker and then save the
                // received file to the picked location. The transfer itself would use
                // the Sockets API and not the Rfcomm API, and so is omitted here for
                // brevity.
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}", ex.Message));
            }
        }
    }
}
