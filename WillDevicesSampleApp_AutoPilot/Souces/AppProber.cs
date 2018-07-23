using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WillDevicesSampleApp
{
//    public static class AppProber
    public class AppProber
    {
        static string filename = "log.txt";

        public static string Logfile
        {
            get { return filename; }
            set { filename = value; }
        }

        public static void WriteLog(string str, bool append = true, bool time = true)
        {
//            string path = Directory.GetCurrentDirectory();
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            StreamWriter writer = new StreamWriter(path + "\\" + Logfile, append, sjisEnc);

            str = time ? TimeMessage(str) : str;
            if (append) writer.WriteLine(str);
            writer.Close();
        }

        private static string TimeMessage(string msg)
        {
            DateTime dt = DateTime.Now;
            return String.Format("{0}/{1} {2}:{3}:{4}:{5}: {6}",
                    dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, msg);
        }
    }
}
