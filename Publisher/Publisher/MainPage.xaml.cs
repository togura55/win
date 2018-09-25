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

using Windows.Networking;
using Windows.Networking.Connectivity;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections;
using Windows.Storage.Streams;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Publisher
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static SocketClient socketClient = new SocketClient();  // suppose single instance
        IBuffer buffer = null; // data buffer sent to the server

        class RawData
        {
            public float f;
            public float x;
            public float y;
            public float z;
            public RawData(float f = 0, float x = 0, float y = 0, float z = 0)
            {
                this.f = f;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        private void RawDataToBuffer(ArrayList rawdatalist)
        {
            // convert to byte array and IBuffer
            int num_bytes = sizeof(float);
            byte[] ByteArray = new byte[rawdatalist.Count * num_bytes * 4];
            int count = 0;
            foreach (RawData rd in rawdatalist)
            {
                int offset = count * num_bytes * 4;

                Array.Copy(BitConverter.GetBytes(rd.f), 0, ByteArray, offset, num_bytes);
                Array.Copy(BitConverter.GetBytes(rd.x), 0, ByteArray, offset += num_bytes, num_bytes);
                Array.Copy(BitConverter.GetBytes(rd.y), 0, ByteArray, offset += num_bytes, num_bytes);
                Array.Copy(BitConverter.GetBytes(rd.z), 0, ByteArray, offset += num_bytes, num_bytes);
                count++;
            }
            using (DataWriter writer = new DataWriter())
            {
                writer.WriteBytes(ByteArray);
                buffer = writer.DetachBuffer();
            }
        }
        //private void exercise()
        //{
        //    // decode
        //    //IBuffer buffer =//なにかしらのIBufferデータ
        //    //byte[] readBytes = new byte[buffer.Length];
        //    //using (DataReader reader = DataReader.FromBuffer(buffer))
        //    //{
        //    //    reader.ReadBytes(readBytes);
        //    //}
        //    int num_bytes = sizeof(float);
        //    int num_packets = ByteArray.Length / num_bytes;
        //    byte[] tmpByte = new byte[num_bytes];
        //    for (int i = 0; i < num_packets; i++)
        //    {
        //        float data = BitConverter.ToSingle(ByteArray, i * num_bytes);
        //    }
        //}

        public MainPage()
        {
            this.InitializeComponent();

            socketClient = new SocketClient();
            RestoreSettings();

            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.TextBlock_IPAddr.Text = resourceLoader.GetString("IDC_HostName");
            this.TextBlock_PortNumber.Text = resourceLoader.GetString("IDC_PortNumber");
            this.Pbtn_Exec.Content = resourceLoader.GetString("IDC_Exec");

            this.TextBox_HostName.Text = socketClient.HostNameString;
            this.TextBox_PortNumber.Text = socketClient.PortNumberString;

        }

        private void GetUiState()
        {
            socketClient.HostNameString = this.TextBox_HostName.Text;
            socketClient.PortNumberString = this.TextBox_PortNumber.Text;
        }

        // Message event handler sent by SocketClient object
        private void ReceivedMessage(object sender, string message)
        {
            clientListBox.Items.Add(message);
        }

        // f   [4][  4      ]|[4][4]
        //     [ ][Begin/End]|[Id  ] 
        // c.f. ESN = 7BQS0C1000131
        const uint Mask_Reserve = 0xFF000000;
        const uint Mask_PathOrder = 0x00FF0000;
        const uint Mask_Id = 0x0000FFFF;
        float FlagData(int id, int order)
        {
            float f = 0;
            f = ((order << 16)) & Mask_PathOrder | (id & Mask_Id);
            return f;
        }

        private async void Pbtn_Exec_Click(object sender, RoutedEventArgs e)
        {
            GetUiState();

            // for debug: Create raw data and stack on a buffer
            ArrayList RawDataList = new ArrayList();
            RawDataList.Add(new RawData(FlagData(1,1), 100, 200, 3756));
            RawDataList.Add(new RawData(FlagData(1,1), 101, 223, 4675));
            RawDataList.Add(new RawData(FlagData(1,1), 102, 234, 323));
            RawDataList.Add(new RawData(FlagData(1,1), 105, 278, 32134));
            RawDataToBuffer(RawDataList); // convert
            socket_proc(buffer);

            RawDataList = new ArrayList();
            RawDataList.Add(new RawData(FlagData(0, 7), 123, 700, 2756));
            RawDataList.Add(new RawData(FlagData(0, 7), 191, 823, 1675));
            RawDataList.Add(new RawData(FlagData(0, 7), 122, 635, 6324));
            RawDataToBuffer(RawDataList); // convert
            socket_proc(buffer);

        }

        private async void socket_proc(IBuffer buff)
        {
            try
            {
                clientListBox.Items.Add(string.Format("{0}", "Start."));

                socketClient.SocketClientMessage += ReceivedMessage;    // set a message delegate

                await socketClient.Connect();

                //               socketClient.SendMultipleBuffersInefficiently("Hello world!");
                //                await socketClient.SendMultipleBuffersInefficiently(10);
                socketClient.BatchedSends(buff);

                //socketClient.Receive();

                //socketClient.Disonnect();

                clientListBox.Items.Add(string.Format("{0}", "Completed."));
            }
            catch (Exception ex)
            {
                socketClient.Disonnect();
                clientListBox.Items.Add(string.Format("{0}", ex.Message));
            }
        }

        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            GetUiState();

            // Write state
            //           XmlSerialize(configfile, socketClient);
            StoreSettings();
        }

        #region Store/Restore local data
        private void StoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["HostNameString"] = socketClient.HostNameString;
            container.Values["PortNumberString"] = socketClient.PortNumberString;
        }

        private void RestoreSettings()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("HostNameString"))
                socketClient.HostNameString = container.Values["HostNameString"].ToString();
            if (container.Values.ContainsKey("PortNumberString"))
                socketClient.PortNumberString = container.Values["PortNumberString"].ToString();
        }
        #endregion

    }
}
