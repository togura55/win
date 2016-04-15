using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Reflection;

namespace CleanVsproj
{
    class Program
    {
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

        static string ast = "*", dot = ".";
        static string searchTag = "Compile";
        static string refName = "_common_";
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
        static bool deleteDirs(string path, string subFolder)
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

        static void Main(string[] args)
        {
           bool bRet = true;

            //オプションパラメータの取得
            CommandOptions co = new CommandOptions();
            co.beginSep = "-";
            co.endSep = ":";
            if (!co.ReadParams())
            {
                Console.WriteLine("Error: Read Parameter: {0}", co.lastMsg);
                Environment.Exit(0);
            }
            co.SplitParams();

            //指定ディレクトリ下での列挙
            //string[] extensions = { "vbproj", "csproj" };
            string[] dirs = { "bin", "obj", "backup", "bk", "tags" , "branches"}; 
            string path = co.args[0].value;

            foreach (string d in dirs)
            {
                bRet = deleteDirs(path, d);
                if (!bRet)
                    break;
            }

            if (!bRet)
            {
                goto exit;
            }
            else
            {
                //データの書き出し
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

        exit:
            //終了処理
            Console.WriteLine("Finished. Hit any keys!");
            Console.ReadKey();
        }
    }
}
