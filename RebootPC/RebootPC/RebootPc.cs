using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.IO;

namespace RebootPC
{
    public class RebootPc
    {
        public const int MODE_REBOOT = 0;
        public const int MODE_SHUTDOWN = 1;
        private const int DEFAULT_TIMEOUT = 30;
        private const int DEFAULT_MAXCOUNT = 100;
        private const int DEFAULT_MODE = MODE_REBOOT;
        private const int DEFAULT_COUNTER = 0;
        private const bool DEFAULT_START = false;  // stop
        private const string DEFAULT_FILEPATH = "";

        public int timeout;
        public int maxCount;
        public int mode;
        public bool start;
        public int counter;
        public string filepath;

        public RebootPc()
        {
            ResetSettings();

        }

        public void ResetSettings()
        {
            timeout = DEFAULT_TIMEOUT;
            maxCount = DEFAULT_MAXCOUNT;
            mode = DEFAULT_MODE;
            start = DEFAULT_START;
            counter = DEFAULT_COUNTER;
            filepath = DEFAULT_FILEPATH;
        }


        //// Read stored settings
        //public void ReadSettings()
        //{
        //    try
        //    {
        //        // Deserialize
        //        using (Stream stream = File.OpenRead("config.bin"))
        //        {
        //            BinaryReader reader = new BinaryReader(stream);

        //            timeout = reader.ReadInt32();
        //            maxCount = reader.ReadInt32();
        //            mode = reader.ReadInt32();
        //            start = reader.ReadBoolean();
        //            counter = reader.ReadInt32();
        //            filepath = reader.ReadString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public void WriteSettings()
        //{
        //    try
        //    {
        //        // Serialize
        //        using (Stream stream = File.OpenWrite("config.bin"))
        //        {
        //            BinaryWriter writer = new BinaryWriter(stream);

        //            writer.Write(timeout);
        //            writer.Write(maxCount);
        //            writer.Write(mode);
        //            writer.Write(start);
        //            writer.Write(counter);
        //            writer.Write(filepath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void Run(int mo, int timeout, bool nowarning = true)
        {
            string arguments = string.Empty;
            string[] modes = { "-r", "-s" };
            string f = "-f";

            if (!nowarning)
                f = string.Empty;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "shutdown.exe";

                switch (mo)
                {
                    case MODE_REBOOT:
                    case MODE_SHUTDOWN:
                        arguments = modes[mo] + " " + "-t" + " " + timeout.ToString();
                        break;

                    default:
                        break;
                }
                psi.Arguments = arguments;

                // psi.Arguments = "-s -t 0";   // shutdown
                //psi.Arguments = "-r -t 0";   // reboot
                psi.CreateNoWindow = true;
                Process p = Process.Start(psi);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
