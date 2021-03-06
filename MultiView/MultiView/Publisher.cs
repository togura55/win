﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiView
{
    public class Publisher
    {
        public class DeviceRawData
        {
            public float x;
            public float y;
            public float z;

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

        public float Id;
        public string HostName;
        public string PortNumber;
        public string DeviceType;
        public string DeviceName;
        public float Barcode;
        public List<Stroke> Strokes;

        public Windows.UI.ViewManagement.ApplicationView AppView; // Subscriber

        public Publisher()
        {
            Strokes = new List<Stroke>();
        }


    }

}
