using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;


using System.Xml.Resolvers;



namespace PeaceXml
{
    class XmlFileList
    {
        public string filePath;
        public string fileName;
        public string relativePath;
        public long fileSize;   // byte
        public long contentsSize;
        public long headerSize;
        public int headerStart;
        public int headerEnd;
        public int contentsStart;
        public int contentsEnd;
        public string header;
        public string contents;
        public string decl;
        public string dtd;

        // Constructor
        public XmlFileList()
        {
            filePath = string.Empty;
            fileName = string.Empty;
            relativePath = string.Empty;
            fileSize = 0;
            headerSize = 0;
            contentsSize = 0;
            headerStart = 0;
            headerEnd = 0;
            contentsStart = 0;
            contentsEnd = -1;
            header = String.Empty;
            contents = String.Empty;
            decl = String.Empty;
            dtd = String.Empty;
        }

        public void DividePath(string s)
            // Param: s: full path
            // dividing path and fiilename and set them to member valuables
        {
            int i = s.LastIndexOf(MergeSplit.bs); // separater
            if (i >= 0)
            {
                filePath = s.Substring(0, i);

                //(Bug #14513) Merge時にSource Pathの最後にバックスラッシュ文字があるとファイル名が不正、の対応
                char[] chTrims = {'\\'};
                string filePath2 = filePath.TrimEnd(chTrims);
                string sourcePath2 = Program.sourcePath.TrimEnd(chTrims);

                //(Bug #14516)【DCR】Split 時、出力先フォルダの直下にSplitファイル名(_前部分)を作成して展開
                // -> Merge時のSourceDirectoryの最配下のフォルダ名をrelativePathに含めて、Mergeファイルを作成するよう変更
                int j = sourcePath2.LastIndexOf(MergeSplit.bs); // separater
                if (j >= 0)
                {
                    string filePath3 = sourcePath2.Substring(0, j);
                    relativePath = filePath2.Replace(filePath3, ""); // relative dir path for embedding attribute
                }

                fileName = s.Substring(i + MergeSplit.bs.Length,
                    s.Length - (i + MergeSplit.bs.Length));
            }
        }

        public void size(string s)
        {
            Read(s);

            System.IO.FileInfo fi = new System.IO.FileInfo(s);
            fileSize = fi.Length;

            contentsSize = fileSize - headerSize;
        }

        public void Read(string XmlFile)
        {
          
            // 「XmlReader」インスタンスの宣言(nullで初期化)
            XmlReader reader = null;
            // 特別なルールを設定する場合、「XmlReaderSettings」クラスを使用する。
            XmlReaderSettings settings = new XmlReaderSettings();
            // 検証は「整形式かどうか」の検査のみ
            settings.ConformanceLevel = ConformanceLevel.Document;

            settings.DtdProcessing = DtdProcessing.Ignore;  // Ignore processing DOCTYPE element

            //settings.DtdProcessing = DtdProcessing.Parse;
            //settings.ValidationType = ValidationType.DTD;

            bool bHeader = true;  // read from top

            try
            {
                // 「XmlReader」の作成
                reader = XmlReader.Create(XmlFile, settings);
                while (reader.Read())
                {
                    // Find a XmlDeclaration
                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        decl = "<?xml " + reader.Value + "?>";  // ToDo: no hardcode
                    }

                    // Find a 1st element 
                    // assumed as checking DTD ignore
                    if (bHeader && reader.Depth == 0 && reader.NodeType == XmlNodeType.Element)
                    {
                        StreamReader sr = new StreamReader(XmlFile, Encoding.GetEncoding("utf-8")); // ToDo: no hardcode
                        string text = sr.ReadToEnd();
                        sr.Close();

                        string element_begin = MergeSplit.lt + reader.Name;
                        int i = text.IndexOf(element_begin);
                        if (i >= 0)
                        {
                            header = text.Substring(0, i);
                            contents = text.Substring(i + 1, text.Length - (i + 1));
                            headerSize = i;     // ToDo: num of characters is technically NOT header byte size
                            this.headerStart = 0;
                            this.headerEnd = i;
                            this.contentsStart = i + 1 - 1;
                            this.contentsEnd = -1;
                        }

                        string decl_end = MergeSplit.qu + MergeSplit.gt;
                        int j = text.IndexOf(decl_end);
                        if (j >= 0)
                        {
                            decl = text.Substring(0, j + decl_end.Length);
                            dtd = text.Substring(j + decl_end.Length + 1,
                                (int)headerSize - decl.Length - 1);
                        }
                        bHeader = false;
                        //(Bug #14514)「分割実行時に"Error : Reference to undeclared entity 'XXX', Line X, position."エラーがコマンドラインで表示される」 の修正対応
                        //1st elementは取得済みなため、ループを抜ける。
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.lastMessage = String.Format("Error : {0}", ex.Message);
                Console.WriteLine(Program.lastMessage);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

    }
}
