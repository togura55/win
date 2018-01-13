using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Reflection;

using System.Xml.Resolvers;


namespace PeaceXml
{
    class MergeSplit
    {
        public const string dot = ".", bs = "\\", dq = @"""", sp = " ", ast = "*";
        public const string lt = "<", gt = ">", sl = "/", col = ":", eq = "=", qu = "?";

        static List<XmlFileList> xmlFiles = null;
        static long amount;
        public long numFiles;
        public long numDest;
        static string header;
        static string contents;
        static bool newFile;

        // XML attribute struct
        class attr
        {
            public string name;
            public string value;
        }

        // Base64 Strings Encode/Decode 
        public class Base64str
        {
            private Encoding enc;

            public Base64str(string encStr)
            {
                enc = Encoding.GetEncoding(encStr);
            }

            public string Encode(string str)
            {
                return Convert.ToBase64String(enc.GetBytes(str));
            }

            public string Decode(string str)
            {
                return enc.GetString(Convert.FromBase64String(str));
            }
        }

        // Constructor
        public MergeSplit()
        {
            xmlFiles = new List<XmlFileList>();
            amount = 0;
            numFiles = 0;
            numDest = 0;
            header = string.Empty;
            contents = string.Empty;
            newFile = true;
        }

        private bool ReadXmlFiles(string path, string ext, bool subf)
        {
            bool bRet = true;
            CliIndicator ci = null;

            //(Bug #14511)Source Directory内のSubFolderを処理対象とするか否か選択できるよう機能追加
            SearchOption strSubFolOpt;
            if (subf == true)
                strSubFolOpt = SearchOption.AllDirectories;
            else
                strSubFolOpt = SearchOption.TopDirectoryOnly;

            try
            {
                //  Enumarate all files under the folder
                IEnumerable<string> files = Directory.EnumerateFiles(
                        path, ast + dot + Program.extension, strSubFolOpt);

                //retrieving each files
                ci = new CliIndicator();
                foreach (string f in files)
                {
                    if (Program.fCliIndicator && !Program.fGUI) ci.update((int)numFiles, files.Count()); // show CLI progress indicator

                    xmlFiles.Add(new XmlFileList());
                    xmlFiles[xmlFiles.Count - 1].size(f); // get/set file size
                    xmlFiles[xmlFiles.Count - 1].DividePath(f); // set path and filename
                    amount += xmlFiles[xmlFiles.Count - 1].fileSize;
                    numFiles++;

                    //if (!Program.fGUI) Console.WriteLine(string.Format("{0}, {1}, {2:N0}B, {3:N0}B, [{4},{5}], [{6},{7}]",
                    //    xmlFiles[xmlFiles.Count - 1].filePath, xmlFiles[xmlFiles.Count - 1].fileName, xmlFiles[xmlFiles.Count - 1].fileSize,
                    //    xmlFiles[xmlFiles.Count - 1].contentsSize, xmlFiles[xmlFiles.Count - 1].headerStart, xmlFiles[xmlFiles.Count - 1].headerEnd,
                    //    xmlFiles[xmlFiles.Count - 1].contentsStart, xmlFiles[xmlFiles.Count - 1].contentsEnd));  // for debug
                }
                if (!Program.fGUI) Console.WriteLine("Enum. completed. Num. files: {0}, Total size: {1:N0}B",
                    numFiles, Math.Round((decimal)amount / 1000));  // ToDo:
                bRet = true;
            }
            catch (Exception ex)
            {
                Program.lastMessage = String.Format("Error : {0}", ex.Message);
                bRet = false;
            }
            finally
            {
                if (ci != null) ci.destory();
            }

            return bRet;
        }

        private void CloseMerge(StreamWriter w)
        {
            if (w != null)
            {
                w.Write(lt + sl + Program.rootElement + gt);
                w.Close();
            }
        }

        private void InitMerge()
        {
            amount = 0; // reset
        }

        private string CreateMergedPath(int count)
        {
            GenDestFilename(ref Program.destFilename);

            string fullpath_base = Program.destPath + bs + Program.destFilename + "_";
            string fullpath = string.Empty;

            // create files in flat at the destination folder
            // del if existed
            fullpath = fullpath_base + count.ToString() + dot + Program.extension;   // xxx_1.xml
            if (File.Exists(fullpath))
                File.Delete(fullpath);

            return fullpath;
        }

        // Use root source folder name unless no options were addressed
        //  params: name - global param of destFilename
        private void GenDestFilename(ref string name)
        {
            if (name == string.Empty)
            {

                char[] chTrims = { '\\' };
                string sourcePath2 = Program.sourcePath.TrimEnd(chTrims); 

                string s = bs;
                int pos = sourcePath2.LastIndexOf(s);
                name = sourcePath2.Substring(pos + s.Length, sourcePath2.Length - (pos + s.Length));
            }
            else
                name = Program.destFilename;
        }

        public bool MergeFiles()
        {
            bool bRet = false;

            List<string> headersList = new List<string>(); // Unique list of header part
            StreamReader sr = null;
            StreamWriter sw = null;

            if (!ReadXmlFiles(Program.sourcePath, Program.extension, Program.fSubFolder))
                return false;
            
            CliIndicator ci = null;
            try
            {
                int count = 1;  // merged file counter

                // Init process
                InitMerge();

                ci = new CliIndicator();

                foreach (XmlFileList x in xmlFiles)
                {
                    if(Program.fCliIndicator && !Program.fGUI) ci.update(count, xmlFiles.Count()); // show CLI progress indicator

                    string fullpath = CreateMergedPath(count);

                    // Check if capacity is over Or 1st time
                    if (amount > Program.capacity || newFile)
                    {
                        // post process
                        CloseMerge(sw);

                        // Init process
                        InitMerge();
                        fullpath = CreateMergedPath(count);
                        sw = new StreamWriter(fullpath, true); // encode = def
                        sw.Write(x.decl);
                        sw.Write(lt + Program.rootElement + gt);

                        // reset amount, create a new file for destination
                        amount = 0;
                        count++;   // increment file number counter
                        newFile = false;
                    }
                    else
                        newFile = false;

                    sr = new StreamReader(x.filePath + bs + x.fileName, Encoding.GetEncoding("utf-8"));
                    string source = sr.ReadToEnd();
                    sr.Close();

//                    GenDestFilename(ref Program.destFilename);    // update dest filename string
                    Base64str base64 = new Base64str("utf-8");  // ToDo: no hardcode
                    string mergeDtd = lt + Program.subElement + sp +
                        Program.pathAttr + eq + dq + x.relativePath + dq + sp + // path=
                        Program.filesAttr + eq + dq + x.fileName + dq + sp +    // filename=
                        Program.headerAttr + eq + dq + base64.Encode(x.header) + dq +   // header=
                        gt;
                    sw.Write(mergeDtd); // DTD

                    string mergeContents = String.Empty;
                    if (x.contentsEnd < 0)
                        mergeContents = source.Substring(x.contentsStart, source.Length - x.contentsStart);
                    else
                        mergeContents = source.Substring(x.contentsStart, x.contentsEnd - x.contentsStart);
                    sw.Write(mergeContents);    // Contents

                    sw.Write(lt + sl + Program.subElement + gt);  // close element

                    amount += mergeDtd.Length + mergeContents.Length;
                }

                CloseMerge(sw);  //  post process
                numDest = count;  // num of merged files, finally

                Program.lastMessage = String.Format("Merge completed. {0} source(s) into {1} file(s).",
                            numFiles, numDest - 1);  // 1 base
                bRet = true;
            }
            catch (Exception ex)
            {
                Program.lastMessage = String.Format("Error: MergeFiles: {0}", ex.Message);
                bRet = false;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (sw != null) sw.Close();
                if (ci != null) ci.destory();
            }

            return bRet;
        }

        private void SplitWrite(List<attr> att)
        {
            int indexPath = 0, indexFile = 0, indexHeader = 0;
            for (int i = 0; i < att.Count; i++)
            {
                if (att[i].name == Program.pathAttr)
                    indexPath = i;
                else if (att[i].name == Program.filesAttr)
                    indexFile = i;
                else if (att[i].name == Program.headerAttr)
                    indexHeader = i;
            }
            string subpath = att[indexPath].value;
            string filename = att[indexFile].value;
            string fullpath = string.Empty;
            string decodedHeader = att[indexHeader].value;

            // create folders/files under the destination folder
            // del if existed
            if (subpath != string.Empty)
                subpath += bs;
            fullpath = Program.destPath + bs + subpath;
            if (!Directory.Exists(fullpath))
                Directory.CreateDirectory(fullpath);
            fullpath += filename;   //
            if (File.Exists(fullpath))
                File.Delete(fullpath);

            StreamWriter w = new StreamWriter(fullpath, true); // string encde = def
            Base64str base64 = new Base64str("utf-8");  // ToDo: no hardcode
            w.Write(base64.Decode(decodedHeader));   // decode header and write
            w.Write(contents);
            w.Close();
        }

        //(Bug #14514) 分割実行時に"Error : Reference to undeclared entity 'XXX', Line X, position."エラーがコマンドラインで表示される 不具合対応
        //XML宣言、ルートタグ、SubEelementを一時的に作成する
        static string CreateXml(string subElmts)
            {
            string ret;
            string xMLdec = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>";// ToDo: no hardcode
            string rootTag = Program.rootElement;

            ret = xMLdec + lt + Program.rootElement + gt + subElmts + lt + sl + Program.subElement + gt + lt + sl + Program.rootElement + gt;
             ret = ret + "";
             return ret;
            }

        //(Bug #14514) 分割実行時に"Error : Reference to undeclared entity 'XXX', Line X, position."エラーがコマンドラインで表示される 不具合対応
        bool SplitBody(string text,  string subElmt)
        {
            int current1 = 0;

            bool bRet = true;
            bool bEof = true;


            //(Bug #14518)Insufficient restore contents under (D) Outline の修正
            while(bEof)
            {
                string name1 = lt + subElmt; // find the subElement ex. <sdlj_files
                int index1 = text.IndexOf(name1, current1);

                if (index1 >= 0)
                {
                    string name2 = dq + gt; // find "> to find the < of the first element . ex. "><sie_bunmyaku_list
                    current1 = index1 + 1;
                    int index2 = text.IndexOf(name2, current1);

                    if (index2 >= 0)
                    {
                        //index2の位置を、「">」
                        index2 = index2 + name2.Length;
                        string name3 = lt + sl + subElmt + gt; // find the last subElement ex. <\sdlj_files>
                        current1 = index2 + 1;
                        int index3 = text.IndexOf(name3, current1);

                        if (index3 >= 0)
                        {
                            string subElmtsContent = text.Substring(index1, index2 - index1);
                            current1 = index3 + 1;

                            string xmlContent = CreateXml(subElmtsContent);

                            XmlTextReader tr = null;
                            //XmlReaderSettings settings = new XmlReaderSettings();
                            //settings.ConformanceLevel = ConformanceLevel.Document;
                            //settings.DtdProcessing = DtdProcessing.Ignore;  // Ignore processing DOCTYPE element

                            try
                            {
                                tr = new XmlTextReader(new StringReader(xmlContent));// ToDo: no hardcode

                                string elementName2 = string.Empty;

                                List<attr> att2 = new List<attr>();
                                //reader = XmlReader.Create(xmlContent, settings);
                                while (tr.Read())  // read nodes one by one
                                {
                                    // Find a 1st element and extract its attributes
                                    if (tr.Depth == 1 && tr.NodeType == XmlNodeType.Element)
                                    {
                                        string name = lt + tr.Name;
                                        int index4 = xmlContent.IndexOf(name, 0);
                                        if (index4 >= 0)
                                        {
                                            int cnt = tr.AttributeCount;
                                            for (int i = 0; i < cnt; i++)
                                            {
                                                // 属性ノードへ移動
                                                tr.MoveToAttribute(i);
                                                // 属性名、及び属性の値
                                                att2.Add(new attr());
                                                att2[att2.Count - 1].name = tr.Name;
                                                att2[att2.Count - 1].value = tr.Value;
                                            }
                                        }
                                        contents = text.Substring(index2, index3 - index2);
                                        SplitWrite(att2);
                                        numDest++;
                                    }

                                }
                                bRet = true;
                            }
                            catch (Exception ex)
                            {
                                Program.lastMessage = String.Format("Error : {0}", ex.Message);
                                bRet = false;
                            }
                            finally
                            {
                                if (tr != null) tr.Close();

                            }
                        }

                    }
                }
                else
                {
                    // subElement ex. <sdlj_files が無い場合、処理を抜ける
                    bEof = false;
                }
            }
            return bRet;
        }


        public bool SplitFile()
        {
            bool bRet = false, bBody = true; 
            int fileCount = 0;

            // Read all files under the directory
            IEnumerable<string> files = Directory.EnumerateFiles(
                Program.sourcePath, ast + dot + Program.extension, SearchOption.TopDirectoryOnly);
            numFiles = files.Count();  // num source files

            CliIndicator ci = new CliIndicator();
            foreach (string f in files) // _1.xml, _2xml,.....
            {
                int current = 0;
                contents = string.Empty;
                header = string.Empty;

                if (Program.fCliIndicator && !Program.fGUI) ci.update(fileCount, files.Count()); // show CLI progress indicator
                fileCount++;

                XmlReader reader = null;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;
                settings.DtdProcessing = DtdProcessing.Ignore;  // Ignore processing DOCTYPE element

                bool bHeader = true;  // read from top
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(f, Encoding.GetEncoding("utf-8"));    // ToDo: no hardcode
                    string text = sr.ReadToEnd();
                    //Console.WriteLine("{0}", text);
                    sr.Close();

                    List<attr> att = new List<attr>();
                    string elementName = string.Empty;

                    reader = XmlReader.Create(f, settings);
                    while (reader.Read())  // read nodes one by one
                    {
                        // Find a 1st element and set them to header
                        // assumed as DTD ignore
                        if (bHeader && reader.Depth == 0 && reader.NodeType == XmlNodeType.Element)
                        {
                            string name = lt + reader.Name;
                            int index = text.IndexOf(name, current);
                            if (index >= 0)
                            {
                                header = text.Substring(current, index);
                                current = index + 1;
                            }
                            bHeader = false;
                        }
                        // Find Depth=1 element : <peacexml_files...
                        else if (!bHeader && reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                        {
                            string name = reader.Name;
                            int index = text.IndexOf(name, current);
                            if (index >= 0)
                            {
                                //(Bug #14518)Insufficient restore contents under (D) Outline の修正
                                //(Bug #14526)Destination Directoryフィールドが空の状態で、Splitを実行しようとすると、エラー表示されない。の対応。 SplitBodyの戻値をbBodyに受け取るよう追加。
                                bBody  = SplitBody(text, name);
                                break;
                            }
                            //(Bug #14514) 分割実行時に"Error : Reference to undeclared entity 'XXX', Line X, position."エラーがコマンドラインで表示される 不具合対応によりコメントアウト
                            // 属性からパスとファイル名を読みだす
                            //int cnt = reader.AttributeCount;
                            //for (int i = 0; i < cnt; i++)
                            //{
                            //    // 属性ノードへ移動
                            //    reader.MoveToAttribute(i);
                            //    // 属性名、及び属性の値
                            //    att.Add(new attr());
                            //    att[att.Count - 1].name = reader.Name;
                            //    att[att.Count - 1].value = reader.Value;
                                //                               if (!Program.fGUI) Console.WriteLine("[{0}] {1}={2}", i, reader.Name, reader.Value);
                            //}
                        }
                        // Find Depth=2 element
                        //else if (!bHeader && reader.Depth == 2 && reader.NodeType == XmlNodeType.Element)
                        //{
                        //    // element全体を読み出す: ToDo
                        //    string name = reader.Name;
                        //    int index = text.IndexOf(name, current);
                        //    if (index >= 0)
                        //    {
                        //(Bug #14514) 分割実行時に"Error : Reference to undeclared entity 'XXX', Line X, position."エラーがコマンドラインで表示される 不具合対応によりコメントアウト
                        //        name = lt + sl + reader.Name;  // find end
                        //        current = index + 1;
                        //        index = text.IndexOf(name, current);
                        //        if (index >= 0)
                        //        {
                        //            contents = text.Substring(current - 1, index - (current - 1) + (name + gt).Length);
                        //        }
                        //        else
                        //        {
                        //            // ToDo:
                        //        }
                        //    }
                        //    // Write to file
                        //    SplitWrite(att);
                        //    numDest++;      // num dest files
                        //}
                    }
                    //(Bug #14526)Destination Directoryフィールドが空の状態で、Splitを実行しようとすると、エラー表示されない。の対応 bBody判定追加
                    if (bBody)
                    {
                        Program.lastMessage = String.Format("Split completed. {0} source(s) into {1} file(s).",
                            numFiles, numDest);
                        bRet = true;
                    }
                    else
                        bRet = false;
                }
                catch (Exception ex)
                {
                    Program.lastMessage = String.Format("Error : {0}", ex.Message);
                    bRet = false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                    if (sr != null) sr.Close();
                    if (ci != null) ci.destory();
                }
                //ここに書く

            }
            return bRet;
        }
    }


    // CLI progress indicator
    class CliIndicator
    {
        int per, cnt;
        char[] bars = { '／', '―', '＼', '｜' };

        // constructor
        public CliIndicator()
        {
            Console.CursorVisible = false;  // hide cursor
            per = 0;
            cnt = 0;
        }

        public void update(int c, int amount)
        {
            if (cnt > amount / 10)
            {
                Console.Write(bars[per % 4]);
                Console.Write("{0, 4:d0}% [{1}/{2}]", (per + 1) * 10, c, amount);
                Console.SetCursorPosition(0, Console.CursorTop);
                cnt = 0; // reset
                per++;
            }
            else
                cnt++;
        }

        public void destory()
        {
            Console.CursorVisible = true;  // show cursor
        }
    }

}
