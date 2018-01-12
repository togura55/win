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


namespace EditBarcode
{
    static class Program
    {
        //  Global constant values
        public const string cdefValueGui = "GUI";
        public const string cdefValueCli = "CLI";
        public const string cdefValueOn = "ON";
        public const string cdefValueOff = "OFF";

        // Global valuables setting at the initial state
        public static bool fGUI;    // ui mode
        static bool fHelp;  // show help
        static bool fVer;   // show this version
        public static CommandOptions co = null;
        public static string appName;   // This app name
        public static Version version;  // This version strings
        public static string lastMessage;

        static bool fSet;
        public static string barcodeUpper;
        public static string incValue;  // +-
        public static string templateDir;
        public static string clbVersion;
        public static string clbRootPath;

        /// <summary>
        /// Main entry point of this application
        /// </summary>
        [STAThread]
        static void Main()
        {
            Initialize();
            if (!ReadParams())  // command-line params
                Environment.Exit(0);  // exit

            if (fGUI == true)
            {
                // Go Form UI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
            // Go Command-Line Interface
            else
                CmdMain(co);
        }

        /// <summary>
        /// Set default values and states for Main
        /// </summary>
        private static void Initialize()
        {
            // set default values
            fGUI = true;
            fHelp = false;
            fVer = false;

            fSet = false;
            barcodeUpper = string.Empty;
            incValue = "0";
            templateDir = string.Empty;
            clbVersion = string.Empty;
            clbRootPath = string.Empty;

            AssemblyTitleAttribute asmttl = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            appName = asmttl.Title;
            Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            version = asm.GetName().Version;

            lastMessage = string.Empty;
        }

        /// <summary>
        /// Execute the procedure of CLI mode
        /// </summary>
        /// <param name="co">An object of CommandOptions</param>
        private static void CmdMain(CommandOptions co)
        {
            if (Program.fHelp)
                Console.Write(co.CreateHelpContents("Get/Set CLB barcode value"));
            else if (Program.fVer)
                Console.WriteLine(string.Format("{0} Version: {1}", appName, version));
            else
            {
                ClbBarcode cb = new ClbBarcode();
                string f = string.Empty;
                bool bRet = false;

                if (clbVersion != string.Empty)
                    cb.clbVersion = clbVersion;

                if (clbRootPath != string.Empty)
                    cb.clbConfigRootPath = clbRootPath;

                // set&show OR show
                if (fSet)
                {
                    f = barcodeUpper;
                    if (cb.SetUpperBarcode(ref f, incValue))
                        bRet = cb.SetUserConfigXml(ref f);  // write info to config file
                }
                else
                    bRet = cb.GetUserConfigXml(ref f);  // just reading the config

                if (bRet)
                {
                    string upper = string.Empty;
                    foreach (string v in cb.BarcodeValues)
                        upper += v;
                    Console.WriteLine(string.Format("Barcode Upper Digit: {0}", upper));
                }
                else
                    Console.WriteLine(string.Format("Error: {0}", f));
            }
            Console.WriteLine(Environment.NewLine);
        }

        /// <summary>
        /// Get the valid value in Param 
        /// </summary>
        /// <param name="eh"></param>
        /// <param name="val"></param>
        /// <returns>true/false</returns>
        private static bool SetValueByParam(CommandOptions.Param eh, ref string val)
        {
            bool bRet = true;

            if (eh.value == string.Empty)
            {
                Console.WriteLine(InvalidOptionMessage(eh.element, "Invalid Option and Value: {0}")); // For debug
                bRet = false;// Go Exit
            }
            else
            {
                val = eh.value;
            }
            return bRet;
        }

        /// <summary>
        /// Format the invalidate message for displaying
        /// </summary>
        /// <param name="msg">Message strings</param>
        /// <param name="form">Format pattern strings</param>
        /// <returns></returns>
        private static string InvalidOptionMessage(string msg, string form = "Invalid Option: {0}")
        {
            return string.Format(form + Environment.NewLine +
                "see help contents by executing {1} /h:on", msg, appName.ToLower());
        }

        /// <summary>
        ///  Read command-line options and set our parameters
        /// </summary>
        /// <returns>true/false</returns>
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
                    string str = string.Empty;
                    foreach (CommandOptions.Param eh in co.args)
                    {
                        switch (eh.entry)
                        {
                            case CommandOptions.help:
                                if (String.Compare(eh.value, cdefValueOn, true) == 0) // ignore case
                                    fHelp = true;
                                else if (String.Compare(eh.value, cdefValueOff, true) == 0)
                                    fHelp = false;
                                else
                                {
                                    Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                    bRet = false;
                                }
                                break;
                            case CommandOptions.ver:
                                if (String.Compare(eh.value, cdefValueOn, true) == 0) // ignore case
                                    fVer = true;
                                else if (String.Compare(eh.value, cdefValueOff, true) == 0)
                                    fVer = false;
                                else
                                {
                                    Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                    bRet = false;// Go Exit
                                }
                                break;
                            case CommandOptions.ui:
                                if (String.Compare(eh.value, cdefValueGui, true) == 0) // ignore case
                                    fGUI = true;
                                else if (String.Compare(eh.value, cdefValueCli, true) == 0)
                                    fGUI = false;
                                else
                                {
                                    Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                                    bRet = false;// Go Exit
                                }
                                break;

                            case CommandOptions.bc:
                                if (bRet = SetValueByParam(eh, ref barcodeUpper)) fSet = true;
                                break;
                            case CommandOptions.bi:
                                if (bRet = SetValueByParam(eh, ref incValue)) fSet = true;
                                break;
                            case CommandOptions.td:
                                if (bRet = SetValueByParam(eh, ref templateDir)) fSet = true;
                                break;

                            case CommandOptions.cv:
                                if (bRet = SetValueByParam(eh, ref clbVersion)) fSet = true;
                                break;
                            case CommandOptions.sp:
                                if (bRet = SetValueByParam(eh, ref clbRootPath)) fSet = true;
                                break;

                            default:
                                Console.WriteLine(InvalidOptionMessage(eh.element, "Undefined Option: ")); // For debug
                                bRet = false;
                                break;
                        }
                    }
                }

                // investigate mondatory options, only when CLI
                if (!fGUI)
                {
                    //                   string s = string.Empty;
                    //                   if (sourcePath == string.Empty)
                    //                       s = CommandOptions.sp;
                    //                   if (destPath == string.Empty)
                    //                       s = CommandOptions.dp;
                    //                   if (s != string.Empty)
                    //                   {
                    ////                       Console.WriteLine("No options were input: {0}", MergeSplit.sl + s + MergeSplit.col);
                    //                       bRet = false;
                    //                   }
                }
            }
            //            Exit_ReadParams:
            return bRet;
        }
    }
}
