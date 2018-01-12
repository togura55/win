using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Reflection;

namespace CleanVsproj
{
    class Program
    {
        //  Global constant values
        //public const string cdefValueGui = "GUI";
        //public const string cdefValueCli = "CLI";
        public const string cdefValueOn = "ON";
        public const string cdefValueOff = "OFF";

        // Global valuables setting at the initial state
        //public static bool fGUI;    // ui mode
        static bool fHelp;  // show help
        static bool fVer;   // show this version
        public static CommandOptions co = null;
        public static string appName;   // This app name
        public static Version version;  // This version strings
        public static string lastMessage;

        public static string targetPath;

        class ProjectInfo
        {
            public string path;
            public string name;
            public string reference;
            public int index;

            public ProjectInfo()
            {
                path = string.Empty;
                name = string.Empty;
                reference = string.Empty;
                index = 0;
            }
        }

        //static string ast = "*", dot = ".";
        //static string searchTag = "Compile";
        //static string refName = "_common_";
        static string writeFileName = @".\list.txt";
        static string encodeType = "Shift_JIS";
        static List<ProjectInfo> pi = new List<ProjectInfo>();
        static int fileIndex = 0;

        public static void DeleteDirectory(string stDirPath)
        {
            DeleteDirectory(new DirectoryInfo(stDirPath));
        }

        public static void DeleteDirectory(DirectoryInfo hDirectoryInfo)
        {
            // すべてのファイルの読み取り専用属性を解除する
            foreach (System.IO.FileInfo cFileInfo in hDirectoryInfo.GetFiles())
            {
                if ((cFileInfo.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                {
                    cFileInfo.Attributes = System.IO.FileAttributes.Normal;
                }
            }

            // サブディレクトリ内の読み取り専用属性を解除する (再帰)
            foreach (System.IO.DirectoryInfo hDirInfo in hDirectoryInfo.GetDirectories())
            {
                DeleteDirectory(hDirInfo);
            }

            // このディレクトリの読み取り専用属性を解除する
            if ((hDirectoryInfo.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
            {
                hDirectoryInfo.Attributes = System.IO.FileAttributes.Directory;
            }

            // このディレクトリを削除する
            hDirectoryInfo.Delete(true);
        }

        /// <summary>
        /// Retrieve dir 
        /// </summary>
        /// <param name="path">root directry path</param>
        /// <returns>true/false</returns>
        static bool DeleteDirs(string path, string subFolder)
        {
            bool bRet = true;
            string dirName = string.Empty;

            try
            {
                IEnumerable<string> dirs = Directory.EnumerateDirectories(
                       path, "*", System.IO.SearchOption.AllDirectories); ;

                foreach (string d in dirs)
                {
                    DirectoryInfo hDirInfo = new System.IO.DirectoryInfo(d);
                    if (hDirInfo.Name == subFolder)
                    {
                        dirName = d;
                        DeleteDirectory(d);

                        pi.Add(new ProjectInfo());
                        pi[pi.Count - 1].path = d;
                        pi[pi.Count - 1].index = fileIndex;
                        fileIndex++;
                    }
                }   //End of loop
                bRet = true;
            }

            catch (Exception ex)
            {
                bRet = false;
                Console.WriteLine(string.Format("Error: {0} in {1}", ex, dirName));
            }

            return bRet;
        }

        /// <summary>
        /// Set default values and states for Main
        /// </summary>
        private static void Initialize()
        {
            // set default values
            fHelp = false;
            fVer = false;

            targetPath = string.Empty;

            AssemblyTitleAttribute asmttl = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute));
            appName = asmttl.Title;
            Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            version = asm.GetName().Version;

            lastMessage = string.Empty;
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
        ///  Read command-line options and set our parameters
        /// </summary>
        /// <returns>true/false</returns>
        private static bool ReadOptions()
        {
            bool bRet = true;
            co = new CommandOptions()
            {
                BeginSeparater = "-",
                EndSeparater = ":"
            };

            if (co.ReadParams() == false)    // read command-line options
            {
                Console.WriteLine("main: " + co.LastMessage);
                Console.WriteLine(Environment.NewLine + co.CreateHelpContents(appName));
                // Run w/ default valuesif
            }
            else
            {
                if (co.SplitParams() == false)    // decode options
                {
                    Console.WriteLine("main: " + co.LastMessage);
                    Console.WriteLine(Environment.NewLine + co.CreateHelpContents(appName));
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
                            //case CommandOptions.ui:
                            //    if (String.Compare(eh.value, cdefValueGui, true) == 0) // ignore case
                            //        fGUI = true;
                            //    else if (String.Compare(eh.value, cdefValueCli, true) == 0)
                            //        fGUI = false;
                            //    else
                            //    {
                            //        Console.WriteLine(InvalidOptionMessage(eh.element)); // For debug
                            //        bRet = false;// Go Exit
                            //    }
                            //    break;

                            case CommandOptions.d:
                                bRet = SetValueByParam(eh, ref targetPath);
                                break;

                            default:
                                Console.WriteLine(InvalidOptionMessage(eh.element, "Undefined Option: ")); // For debug
                                bRet = false;
                                break;
                        }
                    }
                }
            }
            //            Exit_ReadOptions:
            return bRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Initialize();
            if (!ReadOptions())  // command-line params
                Environment.Exit(0);  // exit

            if (Program.fHelp)
                Console.Write(co.CreateHelpContents("Get/Set CLB barcode value"));
            else if (Program.fVer)
                Console.WriteLine(string.Format("{0} Version: {1}", appName, version));
            else
            {
                bool bRet = true;

                //string[] extensions = { "vbproj", "csproj" };
                string[] subdirs = { "bin", "obj", "backup", "bk", "tags", "branches" };
                //           string path = co.args[0].value;

                // check if directory strings are not in the option switch
                if (targetPath == string.Empty) targetPath = Directory.GetCurrentDirectory();

                // check if a root directory exists
                if (!Directory.Exists(targetPath))
                {
                    Console.WriteLine(string.Format("Directory does not exist: {0}", targetPath));
                    goto exit;
                }

                foreach (string d in subdirs)
                {
                    bRet = DeleteDirs(targetPath, d);
                    if (!bRet)
                        break;
                }

                if (!bRet)
                {
                    goto exit;
                }
                else
                {
                    // Display the process
                    Encoding enc = Encoding.GetEncoding(encodeType);
                    StreamWriter sr = new StreamWriter(writeFileName, false, enc);
                    foreach (ProjectInfo p in pi)
                    {
                        string str = String.Format("{0}\t{1}",
                            p.index, p.path);
                        sr.WriteLine(str);

                        Console.WriteLine(str);
                    }
                    sr.Close();
                }
            }


            exit:
            //End process
            Console.WriteLine("Finished. Hit any keys!");
            Console.ReadKey();
        }
    }
}
