using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using System.Xml.Resolvers;

namespace PeaceXml
{
    static class Program
    {
        //  Global constant values
        public const string cdefValueGui = "GUI";
        public const string cdefValueCli = "CLI";
        public const string cdefValueMerge = "M";
        public const string cdefValueSplit = "S";
        public const string cdefValueOn = "ON";
        public const string cdefValueOff = "OFF";
        // public const long ldefCapMag = 1000;

        // Global valuables setting at the initial state
        public static bool fGUI;    // ui mode
        public static bool fMerge;  // merge/split mode
        static bool fHelp;  // show help
        static bool fVer;   // show this version
        public static bool fCliIndicator;   // show indicator in CLI
        public static CommandOptions co = null;
        public static Stopwatch sw;     // Stop watch
        public static string appName;   // This app name
        public static Version version;  // This version strings
        public static string lastMessage;

        public static string sourcePath;    // source path
        public static string destPath;      // destination path
        public static string destFilename;  // destination file prefix
        public static string extension;     // source/dest file extension
        public static long capacity;        // uppe limit of file size
        public static string rootElement;   // root element string
        public static string subElement;    // sub element string    
        public static string filesAttr;     // attribute string - file
        public static string pathAttr;      // attribute string - path
        public static string headerAttr;    // attribute string - encripted header
        public static bool fSubFolder;      // sub folder


        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Initialize();
            if (!ReadParams())  // command-line params
                Environment.Exit(0);  // exit

            capacity *= 1000;  // ToDo: magnifier should be the parameter

            if (fGUI == true)
            {
                // Go Form UI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            else
            {   // Go Command-Line Interface
                CmdMain(co);
            }

        }

        // default values and state
        private static void Initialize()
        {
            // set default values
            sourcePath = string.Empty;
            destPath = string.Empty;
//            destFilename = @"peacexml";
            destFilename = string.Empty;  // spec supported
            extension = "xml";
            capacity = 1000;  // KB
            rootElement = "sdlj_root";
            subElement = "sdlj_files";
            filesAttr = "filename";
            pathAttr = "path";
            headerAttr = "header";
            fSubFolder = true;

            fGUI = true;
            fMerge = true;
            fHelp = false;
            fVer = false;
            fCliIndicator = false;

            sw = new Stopwatch();
            AssemblyTitleAttribute asmttl = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            appName = asmttl.Title;
            Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            version = asm.GetName().Version;

            lastMessage = string.Empty;
        }

        // Execute CLI mode
        private static void CmdMain(CommandOptions co)
        {
            if (Program.fHelp)
                Console.Write(co.CreateHelpContents());
            else if (Program.fVer)
            {
                Console.WriteLine(string.Format("{0} Version: {1}", appName, version));
            }
            else
            {
                MergeSplit ms = new MergeSplit();

                if (Program.fMerge)
                {
                    sw.Start();  // Stopwatch
                    Console.WriteLine("Merge start");
                    if (ms.MergeFiles())
                    {
                        Program.sw.Stop();
                        Console.WriteLine(lastMessage + " Elaped Time: {0}", Program.sw.Elapsed);
                    }
                    else  // error occured
                    {
                        Console.WriteLine(lastMessage);
                    }
                }
                else  // Split
                {
                    Program.sw.Start();
                    Console.WriteLine("Split start");
                    if (ms.SplitFile())
                    {
                        Program.sw.Stop();
                        Console.WriteLine(lastMessage + " Elaped Time: {0}", Program.sw.Elapsed);
                    }
                    else  // error occured
                    {
                        Console.WriteLine(lastMessage);
                    }
                }
            }
            Console.WriteLine(Environment.NewLine);

            //(Bug #14512) No ReadKey waiting are required on CLI 対応によりコメントアウト
            //Console.WriteLine("Hit any keys!");
            //Console.ReadKey();
        }

        private static bool SetValueByParam(CommandOptions.param eh, ref string val)
        {
            bool bRet = true;

            if (eh.value == string.Empty)
            {
                Console.WriteLine(InvalidOptionMessage(eh.element,"Invalid Option and Value: {0}")); // For debug
                bRet = false;// Go Exit
            }
            else
            {
                val = eh.value;
            }
            return bRet;
        }

        private static string InvalidOptionMessage(string msg, string form = "Invalid Option: {0}")
        {
            return string.Format(form + Environment.NewLine + 
                "see help contents by executing {1} /h:on", msg, appName.ToLower());
        }
        
        // Read command-line options and set our parameters
        private static bool ReadParams()
        {
            bool bRet = true;
            co = new CommandOptions(); // Create an object

            if (co.ReadParams() == false)    // read command-line options
            {
                Console.WriteLine("main: " + co.LastMessage);
                // Run w/ default valuesif
            }
            else
            {
                if (co.SplitParams() == false)    // decode options
                {
                    Console.WriteLine("main: " + co.LastMessage);
                    bRet = false;// Go Exit
                }
                else
                {
                    foreach (CommandOptions.param eh in co.args)
                    {
                        // special options, should be treated first
                        if (eh.entry == co.help)  // option includes help
                        {
                            if (String.Compare(eh.value, cdefValueOn, true) == 0) // ignore case
                            {
                                fHelp = true;
                                goto Exit_ReadParams;
                            }
                            else if (String.Compare(eh.value, cdefValueOff, true) == 0)
                                fHelp = false;
                            else
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                bRet = false;// Go Exit
                                goto Exit_ReadParams;
                            }
                        }
                        else if (eh.entry == co.ver) // option includes version
                        {
                            if (String.Compare(eh.value, cdefValueOn, true) == 0) // ignore case
                            {
                                fVer = true;
                                goto Exit_ReadParams;
                            }
                            else if (String.Compare(eh.value, cdefValueOff, true) == 0)
                                fVer = false;
                            else
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                bRet = false;// Go Exit
                                goto Exit_ReadParams;
                            }
                        }


                        else if (eh.entry == co.ui)
                        {// option includes ui 
                            if (String.Compare(eh.value, cdefValueGui, true) == 0) // ignore case
                                fGUI = true;
                            else if (String.Compare(eh.value, cdefValueCli, true) == 0)
                                fGUI = false;
                            else
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                bRet = false;// Go Exit
                                goto Exit_ReadParams;
                            }
                        }
                        else if (eh.entry == co.mode)
                        {
                            if (String.Compare(eh.value, cdefValueMerge, true) == 0) // ignore case
                                fMerge = true;
                            else if (String.Compare(eh.value, cdefValueSplit, true) == 0)
                                fMerge = false;
                            else
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                bRet = false;// Go Exit
                                goto Exit_ReadParams;
                            }
                        }
                        else if (eh.entry == co.cindicator)
                        {
                            if (String.Compare(eh.value, cdefValueOn, true) == 0) // ignore case
                                fCliIndicator = true;
                            else if (String.Compare(eh.value, cdefValueOff, true) == 0)
                                fCliIndicator = false;
                            else
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                bRet = false;// Go Exit
                                goto Exit_ReadParams;
                            }
                        }
                        // Mondatory
                        else if (eh.entry == co.sp)
                        {
                            bRet = SetValueByParam(eh, ref sourcePath);
                            if (!bRet && !fGUI)
                                goto Exit_ReadParams;
                        }
                        else if (eh.entry == co.dp)
                        {
                            bRet = SetValueByParam(eh, ref destPath);
                            if (!bRet && !fGUI)
                                goto Exit_ReadParams;
                        }

                        // Optional
                        else if (eh.entry == co.df)
                            bRet = SetValueByParam(eh, ref destFilename);

                        else if (eh.entry == co.ex)
                            bRet = SetValueByParam(eh, ref extension);

                        else if (eh.entry == co.ca)
                        {
                            string str = string.Empty;
                            bRet = SetValueByParam(eh, ref str);
                            capacity = long.Parse(str);     // ToDo: check if convart-able strings?
                        }
                        else if (eh.entry == co.re)
                            bRet = SetValueByParam(eh, ref rootElement);

                        else if (eh.entry == co.se)
                            bRet = SetValueByParam(eh, ref subElement);

                        //(Bug #14517) Unable to work correctly when setting attribute names in the split mode 
                        //の対応により、GUI, Helpは記載を表示、および削除。しかし、co.fa、co.pa、co.haは隠しコマンドとしてコメントアウトせず、生かしておく。
                        else if (eh.entry == co.fa)
                            bRet = SetValueByParam(eh, ref filesAttr);

                        else if (eh.entry == co.pa)
                            bRet = SetValueByParam(eh, ref pathAttr);

                        else if (eh.entry == co.ha)
                            bRet = SetValueByParam(eh, ref headerAttr);

                        //(Bug #14511)Source Directory内のSubFolderを処理対象とするか否か選択できるよう機能追加
                        else if (eh.entry == co.sd)
                        {
                            if (String.Compare(eh.value, cdefValueOn, true) == 0) // ignore case
                                fSubFolder = true;
                            else if (String.Compare(eh.value, cdefValueOff, true) == 0)
                                fSubFolder = false;
                            else
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                bRet = false;// Go Exit
                                goto Exit_ReadParams;
                            }
                        }

                        else
                        {// unknown options....
                            {
                                Console.WriteLine(InvalidOptionMessage(eh.element, "Undefined Option: ")); // For debug
                                bRet = false;// Go Exit
                            }
                        }
                    }
                }

                // investigate mondatory options, only when CLI
                if (!fGUI)
                {
                    string s = string.Empty;
                    if (sourcePath == string.Empty)
                        s = co.sp;
                    if (destPath == string.Empty)
                        s = co.dp;
                    if (s != string.Empty)
                    {
                        Console.WriteLine("No options were input: {0}", MergeSplit.sl + s + MergeSplit.col);
                        bRet = false;
                    }
                }
            }
        Exit_ReadParams:
            return bRet;
        }
    }
}
