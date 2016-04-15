using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CleanVsproj
{
    class CommandOptions
    {
        public List<param> args;

        public class param
        {
            public string element;
            public string entry;
            public string value;
        }

        private string[] cmdArgs;

        // Properties
        public string beginSep;  // start chars of option
        private string BeginSeparater
        {
            get { return beginSep; }
            set { beginSep = value; }
        }

        public string endSep;    // end chars of option
        private string EndSeparater
        {
            get { return endSep; }
            set { endSep = value; }
        }

        public string lastMsg; // stored message buffer for caller
        private string LastMessage
        {
            get { return lastMsg; }
        }

        // Constructor
        public CommandOptions()
        {
            args = new List<param>();
            beginSep = "/";  // /
            endSep = ":";   // :
            lastMsg = String.Empty;
        }

       public string lines(long n)
        {
            string s = String.Empty;
            long x = 0;
            while (x < n)
            {
                s = s + Environment.NewLine;
                x = x + 1;
            }
            return s;
        }

        public string formatArg(string source)
        {
            return beginSep + source + endSep;
        }


        public bool ReadParams()
        {
            bool ret = true;
            lastMsg = String.Empty;

            //Get the command line parameters and set to array
            string[] strArray = System.Environment.GetCommandLineArgs();
            if (strArray.Count() == 1)
            {
                lastMsg = "No command-line options were provided.";
                ret = false;
            }
            else
            {
                cmdArgs = new string[strArray.Length - 1];  // path was set in [0]
                for (int i = 0; i < strArray.Length - 1; i++)
                {
                    cmdArgs[i] = strArray[i + 1];  // ignore (0) entry cause including application path 
                }
            }

            return ret;
        }

        public bool SplitParams()
        {
            bool ret = true;
            lastMsg = String.Empty;

            if (cmdArgs == null)
            {
                lastMsg = "Could not read arguments";
                ret = false;
            }
            else
            {
                foreach (string ca in cmdArgs)
                {
                    if ((ca == String.Empty) || (decode(ca) == false))
                    {
                        lastMsg = String.Format("Parameter mismatch: {0}", ca);
                        ret = false;
                        break;  // exit foreach
                    }
                }
            }

            return ret;
        }

        private bool decode(string source)
        {
            bool bRet = true;
            int _base = 0; // offset value
            string src = source.Trim();

            if (src.IndexOf(beginSep) != 0 || src.IndexOf(endSep) <= 0)
            {
                bRet = false;
            }
            else
            {
                // decode and store to the list
                args.Add(new param());
                args[args.Count - 1].element = src;
                args[args.Count - 1].entry =
                    (src.Substring(_base + src.IndexOf(beginSep) + beginSep.Length,
                    src.IndexOf(endSep) - beginSep.Length)).Trim();
                args[args.Count - 1].value =
                    (src.Substring(_base + src.IndexOf(endSep) + endSep.Length,
                    src.Length - (src.IndexOf(endSep) + 1))).Trim();
            }

            return bRet;
        }
    }
}

