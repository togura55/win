using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.UI.Input.Inking;

namespace WdBroker
{
    public class DeviceRawData
    {
        public float f;
        public float x;
        public float y;
        public float z;

        public DeviceRawData(float f = 0, float x = 0, float y = 0, float z = 0)
        {
            this.f = f;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Stroke
    {
        public List<DeviceRawData> DeviceRawDataList;
    }

    public class Publisher
    {
        public string Id;
        public string HostName;
        public string PortNumber;
        public string DeviceType;
        public string DeviceName;
        public Size DeviceSize; // Width,Height
        public float PointSize;
        public string SerialNumber;
        public string TransferMode;
        public float Battery;
        public float Barcode;
        public List<Stroke> Strokes;
        public DeviceRawData PrevRawData;
        public bool StartState;
        public double ViewScale;

        public int Index;

        public Publisher()
        {
            this.DeviceSize.Height = 29700;    // ToDo: get from Publishers
            this.DeviceSize.Width = 21600;

            this.Strokes = new List<Stroke>();
            this.PrevRawData = new DeviceRawData();
            this.StartState = true;
            this.ViewScale = 1.0;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void Dispose()
        {

        }

    }

}
