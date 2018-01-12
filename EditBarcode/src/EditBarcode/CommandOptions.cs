using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace EditBarcode
{
    /// <summary>
    /// Operation of command-line options
    /// </summary>
    class CommandOptions
    {
        const string tab = "\t";

        public List<Param> args;
        public const string guideHelp = "see help contnets by /h:on";
        
        // definition of Command-Line Switch option: mondatory
        public const string ui = "ui";
        public const string bc = "bc";
        public const string bi = "bi";
        public const string td = "td";
        public const string cv = "cv";
        public const string sp = "sp";

        public const string help = "h";
        public const string ver = "v";

        /// <summary>
        /// Command-line option format consist of three parts
        /// </summary>
        public class Param
        {
            public string element;
            public string entry;
            public string value;
        }

        private string[] cmdArgs;

        // Properties
        public string beginSep;  // start chars of option

        /// <summary>
        /// 
        /// </summary>
        public string BeginSeparater
        {
            get { return beginSep; }
            set { beginSep = value; }
        }

        private string endSep;    // end chars of option

        public string EndSeparater
        {
            get { return endSep; }
            set { endSep = value; }
        }

        private string lastMsg; // stored message buffer for caller
        public string LastMessage
        {
            get { return lastMsg; }
        }

        public CommandOptions()
        {
            args = new List<Param>();
            beginSep = "/";
            endSep = ":";
            lastMsg = String.Empty;
        }

        /// <summary>
        /// Insert new lines
        /// </summary>
        /// <param name="n">Number of new lines to be inserted.</param>
        /// <returns>string of lines</returns>
        private string Lines(long n)
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

        /// <summary>
        /// Create a formatted parameter string
        /// </summary>
        /// <param name="source">parameter word</param>
        /// <returns>Formatted string</returns>
        private string FormatArg(string source)
        {
            string s = beginSep + source + endSep;
            return s;
        }

        /// <summary>
        /// Create the formatted string of help contents
        /// </summary>
        /// <param name="desc">Application description strings that is shown at the first line of the help contents</param>
        /// <returns>Help conents strings</returns>
        public string CreateHelpContents(string desc)
        {
            // Description
            string contents = desc;

            // Synopsis
            AssemblyTitleAttribute asmttl = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof(AssemblyTitleAttribute));
            contents += Lines(2) + asmttl.Title + tab +
                String.Format("[{0}] [{1}] [{2}] [{3}] [{4}] [{5}]]",
                FormatArg(ui), FormatArg(bc),
                FormatArg(bi), FormatArg(td), 
                FormatArg(help), FormatArg(ver));
            contents += Lines(1) + " All arguments are allowed to provide case-insensitive.";

            // Parameters
            contents += Lines(1);

            contents += Lines(1) + FormatArg(ui) + Program.cdefValueCli + tab +
                "Run application by Command-line mode";
            contents = contents + Lines(1) + new string(' ', FormatArg(ui).Length) + Program.cdefValueGui + tab +
                "Run application by GUI mode (default)";

            contents += Lines(1) + FormatArg(bc) + "XXX" + tab +
                "Set the upper 6-digit barcode number";
            contents += Lines(1) + FormatArg(bi) + "X/0/-X" + tab +
                "Increment/Decriment upper 6-digit barcode number by provided value";
            contents += Lines(1) + FormatArg(td) + "XXX" + tab +
                "Specify the template directory";
            contents += Lines(1) + FormatArg(help) + Program.cdefValueOn + "/" + Program.cdefValueOff + tab +
                "Show Help contents";
            contents += Lines(1) + FormatArg(ver) + Program.cdefValueOn + "/" + Program.cdefValueOff + tab +
                "Show Version";
            return contents;
        }

        /// <summary>
        /// Read command-line arguments and set them into the array
        /// </summary>
        /// <returns>true/false</returns>
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
                                                   //                   Console.WriteLine(cmdArgs[i]);  //For debug
                }
            }

            return ret;
        }

        /// <summary>
        /// Split parameter string into separators and values
        /// </summary>
        /// <returns>true/false</returns>
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
                    if ((ca == String.Empty) || (Decode(ca) == false))
                    {
                        lastMsg = String.Format("Parameter mismatch: {0}", ca);
                        ret = false;
                        break;  // exit foreach
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Decode parameter word and add them into the list of Param struct
        /// </summary>
        /// <param name="source">String of parameter word</param>
        /// <returns>true/false</returns>
        private bool Decode(string source)
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
                args.Add(new Param());
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
