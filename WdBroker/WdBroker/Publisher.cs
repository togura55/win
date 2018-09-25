using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WdBroker
{
    public class DeviceRawData
    {
        float x;
        float y;
        float z;

        public DeviceRawData(float x = 0, float y = 0, float z = 0)
        {
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
