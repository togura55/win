using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wacom.Devices;
using Wacom.Ink;

namespace PackStrokes
{
    public class StrokeAggregation
    {
        public struct Point
        {
            public float x;
            public float y;
            public float a;
        }

        public class Region
        {
            public Point min;
            public Point max;
            public uint index;
            public List<Stroke> strokes;
            public string fieldData;
            public string fieldTag;
            public string fieldId;

            public Region()
            {
                index = 0;
                strokes = new List<Stroke>();
                fieldData = string.Empty;
                fieldTag = string.Empty;
                fieldId = string.Empty;
            }
        }
        public List<Region> regions;

        //public class Hwstring
        //{
        //    List<Stroke> strokes;
        //    uint regionIndex;

        //    Hwstring()
        //    {
        //        strokes = new List<Stroke>();
        //        regionIndex = 0;
        //    }
        //}
        //public List<Hwstring> hwstrings;

        public class Stroke
        {
            public List<PathEx> pathexs;
            public Point min;
            public Point max;
            public uint regionIndex;

            public Stroke()
            {
                pathexs = new List<PathEx>();
                min.x = min.y = float.MaxValue;
                max.x = max.y = 0;
                regionIndex = 0;
            }
        }
        public List<Stroke> strokes;

        public class PathEx
        {
            public Path path;
            public List<Point> points;

            public PathEx()
            {
                points = new List<Point>();
            }
        }
        public List<PathEx> pathexs;
        public List<Point> points;


        /// <summary>
        /// Constructor of the class
        /// </summary>
        public StrokeAggregation()
        {
            regions = new List<Region>();
            //           hwstrings = new List<Hwstring>();
            strokes = new List<Stroke>();
        }

        public bool CreateRegion(float topX, float topY, float bottomX, float bottomY,
                        string fieldTag = "", string fieldData = "", string fieldId = "")
        {
            if (topX >= bottomX || topY >= bottomY)
                return false;

            Region item = new Region();
            item.min.x = topX;
            item.min.y = topY;
            item.max.x = bottomX;
            item.max.y = bottomY;
            item.index = (uint)regions.Count;
            item.fieldData = fieldData;
            item.fieldTag = fieldTag;
            item.fieldId = fieldId;
            regions.Add(item);

            return true;
        }

        public bool CreateStroke(Path path)
        {
            PathEx pe = new PathEx()
            { path = path };

            var data = path.Data.GetEnumerator();
            float f = -1;
            int count = 0;
            float x = 0, y = 0, a = 0;

            Stroke st = new Stroke();

            Point max, min;
            max.x = st.max.x;
            max.y = st.max.y;
            min.x = st.min.x;
            min.y = st.min.y;

            while (data.MoveNext())
            {
                f = data.Current;
                float mod = count % 3;
                if (mod == 0)
                {
                    x = f;
                    if (max.x < x)
                        max.x = x;
                    if (min.x > x)
                        min.x = x;
                }
                else if (mod == 1)
                {
                    y = f;
                    if (max.y < y)
                        max.y = y;
                    if (min.y > y)
                        min.y = y;
                }
                else
                {
                    a = f;

                    Point p = new Point()
                    { x = x, y = y, a = a };
                    pe.points.Add(p);
                }

                count++;
            }

            st.max.x = max.x;
            st.max.y = max.y;
            st.min.x = min.x;
            st.min.y = min.y;
            st.pathexs.Add(pe);

            st.regionIndex = 0;

            strokes.Add(st);

            return true;
        }

        public bool StrokesToRegion()
        {
            try
            {
                foreach (var s in strokes)
                {
                    foreach (var r in regions)
                    {
                        // ストロークが完全にリージョン内に収まるケース
                        if (s.min.x >= r.min.x && s.min.y >= r.min.y &&
                              s.max.x <= r.max.x && s.max.y <= r.max.y)
                        {
                            // このストロークは入っている
                            r.strokes = new List<StrokeAggregation.Stroke>() { s };

                            break;
                        }

                        // ToDo: ストロークの一部がリージョン内にかかるケース

                        // ToDo: ストロークがまったくリージョンにかからないケース
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex));
                return false;
            }
            return true;
        }

        public bool IsRegion()
        {
            bool res = false;
            if (regions.Count != 0)
                res = true;
            return res;
        }

        public bool IsStroke()
        {
            bool res = false;
            if (strokes.Count != 0)
                res = true;
            return res;
        }
    }
}
