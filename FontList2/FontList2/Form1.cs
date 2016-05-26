using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FontList2
{
    public partial class FormFontList2 : Form
    {
        public string fontName = "Arial";
        public string fontdispName = "";
        public int diffCnt = 0;
        public int totalCnt = 0;
        private string diffCntText = "Diff Cnt. : ";
        private string uniRangeText = "Unicode Range: ";
        private string dulation = "Dulation: ";
        private string encodeName = "shift_jis";
        private string resultName = "list.txt";
        private string sCRLF = System.Environment.NewLine;   // \n
        private string sAppPath = "";
        private string sUniListFile = "";
        private bool groupListMode = true;

        public class UniRange
        {
            public string blockName;
            public string start;
            public string end;
            public int match;
        }
        public List<UniRange> uniRangeList = new List<UniRange>();

        public class UniList
        {
            public int iStart;
            public int iEnd;
            public TimeSpan ts;
            public string sBody;

            // Constructor
            public UniList()
            {
                iStart = 0;
                iEnd = 0x241FE;
                sBody = "";
            }
        }
        UniList uL = new UniList();

        public List<string> uniSoloList = new List<string>();

        // Unicaode information 
        public struct UniFont 
        {
            public string codePoint;    // Unicode code point
            public string fontName;     // Font Name
            public string dispName;     // Displayed Font Name
        }
        public List<UniFont> uniFont = new List<UniFont>();

        public FormFontList2()
        {
            InitializeComponent();

            sAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Initialize GUI state
            //textBoxStart.Text = Convert.ToString(uL.iStart, 16).ToUpper();  // Conv. to Hex Strings
            //textBoxEnd.Text = Convert.ToString(uL.iEnd, 16).ToUpper();      
            checkBoxWrite.Checked = true;
            buttonStart.Enabled = false;

            //InstalledFontCollectionオブジェクトの取得
            System.Drawing.Text.InstalledFontCollection ifc =
                new System.Drawing.Text.InstalledFontCollection();
            //インストールされているすべてのフォントファミリを取得
            FontFamily[] ffs = ifc.Families;

            foreach (FontFamily ff in ffs)
            {
                //ここではスタイルにRegularが使用できるフォントのみを表示
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    //Fontオブジェクトを作成
                    Font fnt = new Font(ff, 8);
                    comboBoxFont.Items.Add(fnt.Name);
                    fnt.Dispose();
                }
            }
            int index = this.comboBoxFont.Items.IndexOf(fontName);
            comboBoxFont.SelectedIndex = index;

            // Initialize ListView
            listViewUniList.View = View.Details;
            listViewUniList.Items.Clear();
            listViewUniList.Columns.Clear();
            //Add column 1
            ColumnHeader ColumnH1 = new ColumnHeader();
            ColumnH1.Text = "Block Name";      //Unicode ブロック名
            ColumnH1.Width = 140;             //横幅
            listViewUniList.Columns.Add(ColumnH1);  //項目の追加
            //Add column 2
            ColumnHeader ColumnH2 = new ColumnHeader();
            ColumnH2.Text = "Start";
            ColumnH2.Width = 45;
            listViewUniList.Columns.Add(ColumnH2);
            //Add column 3
            ColumnHeader ColumnH3 = new ColumnHeader();
            ColumnH3.Text = "End";
            ColumnH3.Width = 45;
            listViewUniList.Columns.Add(ColumnH3);

            // Initialize Radio Buttons
            radioButtonGroup.Checked = groupListMode;
            radioButtonSolo.Checked = !groupListMode;

            updateUI();
        }

        private void procSolo()
        {
            // Set progressbar range
            progressBarFont.Minimum = 0;
            progressBarFont.Maximum = uniSoloList.Count;
            progressBarFont.Value = progressBarFont.Minimum;

            for (int j = 0; j < uniSoloList.Count; j++)
            {
                // Set Range value on GUI
                textBoxStart.Text = uniSoloList[j];
                textBoxEnd.Text = uniSoloList[j];

                // Get start/end values from GUI
                uL.iStart = Convert.ToInt32(textBoxStart.Text, 16);
                uL.iEnd = Convert.ToInt32(textBoxEnd.Text, 16) + 1;

                try
                {
                    int startUni = uL.iStart;
                    int endUni = uL.iEnd;

                    string codePoint = "", charDisp = "";
                    for (int i = startUni; i < endUni; i++)
                    {
                        if (i > 0xffff)
                        {
                            charDisp = Char.ConvertFromUtf32(i);
                        }
                        else
                        {
                            char c = (char)i;
                            charDisp = Convert.ToString(c);
                            if (c == 0x22) charDisp = "\\";          // cannot write chars
                            if (c == 0xa) charDisp = "\\n";
                            if (c == 0xd) charDisp = "\\r";
                            if (c == 0x2c) charDisp = " ";         // Distinguich canma
                        }
                        richTextBox.Text = charDisp;                 // For display
                        codePoint = Convert.ToString(i, 16);     // For write file

                        this.richTextBox.SelectAll();           // Get display font name
                        fontdispName = richTextBox.SelectionFont.Name;
                        if (fontName.CompareTo(fontdispName) != 0)
                            diffCnt++;

                        // Add result
                        uL.sBody += string.Format("{0},{1},{2},{3},{4}{5}",
                            codePoint, charDisp, fontName, fontdispName, " ", sCRLF);

                        // Force proceed messages in the queue 
                        Application.DoEvents();

                        // Counter increment
                        totalCnt++;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }

                // Update ProgressBar
                progressBarFont.Value = j;
            }
            // Finalize ProgressBar
            progressBarFont.Value = progressBarFont.Maximum;
        }

        private void procBlock()
        {
            // Set progressbar range
            progressBarFont.Minimum = 0;
            progressBarFont.Maximum = uniRangeList.Count;
            progressBarFont.Value = progressBarFont.Minimum;

            for (int j = 0; j < uniRangeList.Count; j++)
            {
                // Set Range value on GUI
                textBoxStart.Text = uniRangeList[j].start;
                textBoxEnd.Text = uniRangeList[j].end;
                labelUnicodeRange.Text = string.Format("{0}: {1}",
                    uniRangeText, uniRangeList[j].blockName);

                // Get start/end values from GUI
                uL.iStart = Convert.ToInt32(textBoxStart.Text, 16);
                uL.iEnd = Convert.ToInt32(textBoxEnd.Text, 16);

                // Highlight ListView item
                listViewUniList.Focus();
                if (j != 0)
                    listViewUniList.Items[j - 1].Selected = false;
                listViewUniList.Items[j].Selected = true;
                listViewUniList.EnsureVisible(j);

                try
                {
                    int startUni = uL.iStart;
                    int endUni = uL.iEnd;

                    string codePoint = "", charDisp = "";
                    for (int i = startUni; i < endUni; i++)
                    {
                        if (i > 0xffff)
                        {
                            charDisp = Char.ConvertFromUtf32(i);
                        }
                        else
                        {
                            char c = (char)i;
                            charDisp = Convert.ToString(c);
                            if (c == 0x22) charDisp = " ";          // cannot write chars
                            if (c == 0xa) charDisp = " ";
                            if (c == 0xd) charDisp = " ";
                            if (c == 0x2c) charDisp = " ";         // Distinguich canma
                        }
                        richTextBox.Text = charDisp;                 // For display
                        codePoint = Convert.ToString(i, 16);     // For write file

                        this.richTextBox.SelectAll();           // Get display font name
                        fontdispName = richTextBox.SelectionFont.Name;
                        if (fontName.CompareTo(fontdispName) != 0)
                            diffCnt++;

                        // Add result
                        uL.sBody += string.Format("{0},{1},{2},{3},{4}{5}",
                            codePoint, charDisp, fontName, fontdispName, uniRangeList[j].blockName, sCRLF);

                        // Force proceed messages in the queue 
                        Application.DoEvents();

                        // Counter increment
                        totalCnt++;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                // Update ProgressBar
                progressBarFont.Value = j;
            }
            // Finalize ProgressBar
            progressBarFont.Value = progressBarFont.Maximum;
       }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;        // button disable by the end of process
            labelDiffCnt.Text = diffCntText;    // UI reset
            labelDulation.Text = dulation;      // UI

            // Set GUI Font
            fontName = comboBoxFont.Text;
            this.richTextBox.Font = new Font(fontName, 24);
            this.richTextBox.SelectionAlignment = HorizontalAlignment.Center;

            // Get the current time stamp
            DateTime dtStart = DateTime.Now;

            if (groupListMode)
                procBlock();
            else
                procSolo();

            DateTime dtEnd = DateTime.Now;
            uL.ts = dtEnd - dtStart;

            string t = uL.ts.ToString();
            if (t.LastIndexOf('.') != -1) t = t.Remove(t.LastIndexOf('.'));
            labelDulation.Text = string.Format("{0}{1}", dulation, t);
            labelDiffCnt.Text += string.Format("{0}/{1}", diffCnt.ToString(), totalCnt.ToString());

            buttonStart.Enabled = true;

            if (checkBoxWrite.Checked)
            {
                WriteResult(resultName, encodeName, uL.sBody);
            }
        }

        private void WriteResult(string fileName, string encode, string body)
        {
            // Write file
            string sAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string filePath = string.Format("{0}\\{1}", sAppPath, fileName);
            //文字コード(ここでは、Shift JIS)
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding(encode);

            //TextBox1の内容を書き込む
            //ファイルが存在しているときは、上書きする
            System.IO.File.WriteAllText(filePath, body, enc);

            ////ファイルの末尾にTextBox1の内容を書き加える
            //System.IO.File.AppendAllText(filePath, body, enc);
        }

        private void ListParsSolo(string text)
        {
            bool bStat = true;
            int startIndex = 0, endIndex = 0;
            //int s = 0, e = 0;
            string line = "";

            while (bStat)
            {
                if (startIndex > text.Length)
                    break;

                endIndex = text.IndexOf(sCRLF, startIndex);
                if (endIndex == -1) endIndex = text.Length;

                line = text.Substring(startIndex, endIndex - startIndex);
                if (line.Length != 0)   // Ignore blank line
                    uniSoloList.Add(line);

                startIndex = endIndex+sCRLF.Length;
            }

            labelSum.Text = uniSoloList.Count.ToString();
        }

        private void ListParsBlock(string text)
        {
            bool bStat = true;
            int startIndex = 0, endIndex = 0;
            int s = 0, e = 0;
            string line = "";
           
            while (bStat)
            {
                if (startIndex > text.Length)
                    break;

                endIndex = text.IndexOf(sCRLF, startIndex);
                if (endIndex == -1) endIndex = text.Length;

                line = text.Substring(startIndex, endIndex - startIndex);

                UniRange uR = new UniRange();
                s = 0;
                e = line.IndexOf(",", s);
                uR.blockName = line.Substring(s, e - s);

                s = e + 1;
                e = line.IndexOf(",", s);
                if (e == -1) break;
                uR.start = line.Substring(s, e - s);
                s = e + 1;
                uR.end = line.Substring(s, line.Length - s);
                uniRangeList.Add(uR);

                //リストビューの項目に追加
                ListViewItem Listdata = new ListViewItem();
                Listdata.Text = uR.blockName;
                Listdata.SubItems.Add(uR.start);
                Listdata.SubItems.Add(uR.end);
                listViewUniList.Items.Add(Listdata);

                startIndex = endIndex + sCRLF.Length;
            }
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            sUniListFile = textBoxUniList.Text;
            string text = "";

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(
                    sUniListFile, Encoding.GetEncoding("Shift_JIS")))
                {
                    text = sr.ReadToEnd();
                }

                if (groupListMode)
                    ListParsBlock(text);
                else
                    ListParsSolo(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }

            // Enable buttons
            buttonStart.Enabled = true;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.InitialDirectory = sAppPath;
            ofd.Filter =
                "すべてのファイル(*.*)|*.*|CSVファイル(*.csv)|*.csv";
            ofd.FilterIndex = 2;
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(ofd.FileName);
                // Show on UI
                textBoxUniList.Text = ofd.FileName;
            }
        }

        private void radioButtonGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGroup.Checked == true)
                groupListMode = true;
            else
                groupListMode = false;

            updateUI();
        }
        
        private void updateUI()
        {
            listViewUniList.Enabled = groupListMode;
            //textBoxStart.Enabled = groupListMode;
            //textBoxEnd.Enabled = groupListMode;
        }

    }
}
