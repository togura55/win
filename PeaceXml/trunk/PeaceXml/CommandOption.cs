using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PeaceXml
{
    class CommandOptions
    {
        const string tab = "\t";

        public List<param> args;
        public string ui, mode, cindicator;
        public string sp, dp, df, ex, ca, re, se, fa, pa, ha;
        public string help, ver, cliindicator, sd;
        public string guideHelp = "see help contnets by /h:on";

        public class param
        {
            public string element;
            public string entry;
            public string value;
        }

        private string[] cmdArgs;

        // Properties
        public string beginSep;  // start chars of option

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

        // Constructor
        public CommandOptions()
        {
            args = new List<param>();
            beginSep = MergeSplit.sl;  // /
            endSep = MergeSplit.col;   // :
            lastMsg = String.Empty;

            // definition of Command-Line Switch option: mondatory
            ui = "ui";
            mode = "mo";
            cindicator = "ci";
            sp = "sp";
            dp = "dp";
            // : optional
            df = "df";
            ex = "ex";
            ca = "ca";
            re = "re";
            se = "se";
            fa = "fa";
            pa = "pa";
            ha = "ha";
            help = "h";
            ver = "v";
            cliindicator = "ci";
            sd = "sd";
        }

        private string lines(long n)
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

        private string formatArg(string source)
        {
            string s = beginSep + source + endSep;
            return s;
        }

        public string CreateHelpContents()
        {
            string contents = String.Empty;

            // Description
            contents = "Merge/Split XML file.";

            // Synopsis
            AssemblyTitleAttribute asmttl = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof(AssemblyTitleAttribute));
            contents = contents + lines(2) + asmttl.Title + tab +
                //(Bug #14517) Unable to work correctly when setting attribute names in the split mode の修正対応, fa, pa, haコメントアウト
                //String.Format("[{0}] [{1}] [{2}] [{3}] [{4}] [{5}] [{6}] [{7}] [{8}] [{9}] [{10}] [{11}] [{12}] [{13}] [{14}] [{15}]",
                String.Format("[{0}] [{1}] [{2}] [{3}] [{4}] [{5}] [{6}] [{7}] [{8}] [{9}] [{10}] [{11}] [{12}]]",
                formatArg(ui), formatArg(mode),
                formatArg(sp), formatArg(dp), formatArg(df), formatArg(ex), formatArg(ca), formatArg(re), formatArg(se), // formatArg(fa), formatArg(pa), formatArg(ha),
                formatArg(help), formatArg(ver), formatArg(cliindicator), formatArg(sd));
            contents = contents + lines(1) + " All arguments are allowed to provide case-insensitive.";

            // Parameters
            contents = contents + lines(1);

            contents = contents + lines(1) + formatArg(ui) + Program.cdefValueCli + tab +
                "Run application by Command-line mode";
            contents = contents + lines(1) + new string(' ', formatArg(ui).Length) + Program.cdefValueGui + tab +
                "Run application by GUI mode (default)";
            contents = contents + lines(1) + formatArg(mode) + Program.cdefValueMerge + tab +
                "Execute merging XML files (default)";
            contents = contents + lines(1) + new string(' ', formatArg(mode).Length) + Program.cdefValueSplit + tab +
                "Execute splitting XML files";

            contents = contents + lines(1) + formatArg(this.sp) + "XXX" +  tab +
                "Source directory path. Mondatory when CLI mode";
            contents = contents + lines(1) + formatArg(this.dp) + "XXX" + tab +
                "Destination directory path. Mondatory when CLI mode";
            contents = contents + lines(1) + formatArg(this.df) + "XXX" + tab +
                "Destination file name";
            contents = contents + lines(1) + formatArg(this.ex) + "XXX" + tab +
                "Extension string of source file(s)";
            contents = contents + lines(1) + formatArg(this.ca) + "XXX" + tab +
                "Size of upper limit for merged file (KB)";
            contents = contents + lines(1) + formatArg(this.re) + "XXX" + tab +
                "Root element name";
            contents = contents + lines(1) + formatArg(this.se) + "XXX" + tab +
                "Sub element name";
            //(Bug #14517) Unable to work correctly when setting attribute names in the split mode の修正対応
            //contents = contents + lines(1) + formatArg(this.fa) + "XXX" + tab +
            //    "Filename attribute name";
            //contents = contents + lines(1) + formatArg(this.pa) + "XXX" + tab +
            //    "Path attribute name";
            //contents = contents + lines(1) + formatArg(this.ha) + "XXX" + tab +
            //"Header attribute name";

            contents = contents + lines(1) + formatArg(this.help) +  Program.cdefValueOn + "/" + Program.cdefValueOff + tab +
                "Show Help contents";
            contents = contents + lines(1) + formatArg(this.ver) + Program.cdefValueOn + "/" + Program.cdefValueOff + tab +
                "Show Version";
            contents = contents + lines(1) + formatArg(this.cliindicator) + Program.cdefValueOn + "/" + Program.cdefValueOff + tab +
                "Show CLI progress indicator";
            //(Bug #14511)Source Directory内のSubFolderを処理対象とするか否か選択できるよう機能追加
            contents = contents + lines(1) + formatArg(this.sd) + Program.cdefValueOn + "/" + Program.cdefValueOff + tab +
                "Sub folder";
            return contents;
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
 //                   Console.WriteLine(cmdArgs[i]);  //For debug
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

