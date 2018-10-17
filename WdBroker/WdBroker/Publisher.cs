using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public float Id;
        public string HostName;
        public string PortNumber;
        public string DeviceType;
        public string DeviceName;
        public float Barcode;
        public List<Stroke> Strokes;

        public Publisher()
        {
            Strokes = new List<Stroke>();
        }
    }


}
