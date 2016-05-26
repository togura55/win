using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FontList
{
    public partial class FormFontList : Form
    {
        public string fontName = "Arial";
        public string fontNameDisp = "";
        public int diffCnt = 0;
        private string diffCntText = "Diff Cnt. : ";
        private string dulation = "Dulation: ";
        private string encodeName = "shift_jis";
        private string resultName = "list.csv";
        private string sCRLF = System.Environment.NewLine;   // \n

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

        // Unicaode information 
        public struct UniFont 
        {
            public string unicode;      // Unicode
            public string nameFont;     // Font Name
            public string nameDisp;     // Displayed Font Name
        }
        public List<UniFont> uniFont = new List<UniFont>();

        public FormFontList()
        {
            InitializeComponent();

            // Initialize GUI state
            textBoxStart.Text = Convert.ToString(uL.iStart, 16).ToUpper();  // Conv. to Hex Strings
            textBoxEnd.Text = Convert.ToString(uL.iEnd, 16).ToUpper();      
            checkBoxWrite.Checked = true;

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

            // Get start/end values from GUI
            uL.iStart = Convert.ToInt32(textBoxStart.Text, 16);
            uL.iEnd = Convert.ToInt32(textBoxEnd.Text, 16);
            // Set progressbar range
            progressBarFont.Minimum = uL.iStart;
            progressBarFont.Maximum = uL.iEnd;
            progressBarFont.Value = progressBarFont.Minimum;


            ////BackgroundWorkerのProgressChangedイベントが発生するようにする
            //backgroundWorkerMain.WorkerReportsProgress = true;
            ////パラメータを指定して、処理を開始するパラメータが必要なければ省略できる
            //backgroundWorkerMain.RunWorkerAsync(uL);

            DateTime dtStart = DateTime.Now;

            try
            {
                int startUni = uL.iStart;
                int endUni = uL.iEnd;

                string str;
                for (int i = startUni; i < endUni; i++)
                {
                    char c = (char)i;
                    str = Convert.ToString(c);
                    if (c == 0x22) str = "\\";
                    richTextBox.Text = str;                 // For display
                    string s = Convert.ToString(c, 16);     // For write file

                    this.richTextBox.SelectAll();           // Get display font name
                    fontNameDisp = richTextBox.SelectionFont.Name;
                    if (fontName.CompareTo(fontNameDisp) != 0)
                        diffCnt++;

                    // Add result
                    uL.sBody += string.Format("{0},{1},{2},{3},{4}", s, str, fontName, fontNameDisp, sCRLF);

                    // Update ProgressBar
                    progressBarFont.Value = i;

                    // Force proceed messages in the queue 
                    Application.DoEvents();

                    //ProgressChangedイベントハンドラを呼び出し、
                    //コントロールの表示を変更する
                    //bgWorker.ReportProgress(i);
                }
                // Finalize ProgressBar
                progressBarFont.Value = progressBarFont.Maximum;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            DateTime dtEnd = DateTime.Now;
            uL.ts = dtEnd - dtStart;

            string t = uL.ts.ToString();
            if (t.LastIndexOf('.') != -1) t = t.Remove(t.LastIndexOf('.'));
            labelDulation.Text = string.Format("{0}{1}", dulation, t);
            labelDiffCnt.Text += string.Format("{0}/{1}", diffCnt.ToString(),(uL.iEnd-uL.iStart).ToString());

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

            //ファイルの末尾にTextBox1の内容を書き加える
            System.IO.File.AppendAllText(filePath, body, enc);
        }


//        private void backgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
//        {
//            BackgroundWorker bgWorker = (BackgroundWorker)sender;

//            //パラメータを取得する
//            UniList uL = (UniList)e.Argument;
//            int startUni = uL.iStart;
//            int endUni = uL.iEnd;

//            ////時間のかかる処理を開始する
//            //for (int i = 1; i <= maxLoops; i++)
//            //{
//            //    //1秒間待機する（時間のかかる処理があるものとする）
//            //    System.Threading.Thread.Sleep(1000);

//            //    //ProgressChangedイベントハンドラを呼び出し、
//            //    //コントロールの表示を変更する
//            //    bgWorker.ReportProgress(i);
//            //}

//            DateTime dtStart = DateTime.Now;
//            string str;
//            for (int i = startUni; i < endUni; i++)
//            {
//                char c = (char)i;
//                str = Convert.ToString(c);
//                richTextBox.Text = str;                  // For display
//                string s = Convert.ToString(c, 16);     // For write file

//                //UniFont item = new UniFont();
//                //item.unicode = s;
//                //item.nameFont = comboBoxFont.Text;      // Font which attempt te be loaded
//                this.richTextBox.SelectAll();           // Get display font name
//                //item.nameDisp = this.richTextBox.SelectionFont.Name;    // Actual display font
//                //uniFont.Add(item);

//                fontNameDisp = richTextBox.SelectionFont.Name;
//                if (fontName.CompareTo(fontNameDisp) != 0)
//                    diffCnt++;
//                if (str == "\\") str = "\\";
//                body += s + "," + str + "," + fontName + "," + fontNameDisp + sCRLF;

//                //ProgressChangedイベントハンドラを呼び出し、
//                //コントロールの表示を変更する
//                bgWorker.ReportProgress(i);
//            }

//            DateTime dtEnd = DateTime.Now;
//            uL.ts = dtEnd - dtStart;

//            //ProgressChangedで取得できる結果を設定する
//            //結果が必要なければ省略できる
//            e.Result = uL;

//        }

//        private void backgroundWorkerMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
//        {
//            //progressBarFontの値を変更する
//            progressBarFont.Value = e.ProgressPercentage;
//            //Label1のテキストを変更する
////            Label1.Text = e.ProgressPercentage.ToString();

//        }

//        private void backgroundWorkerMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            if (e.Error != null)
//            {
//                //エラーが発生したとき
// //               Label1.Text = "エラー:" + e.Error.Message;
//            }
//            else
//            {
//                //正常に終了したとき
//                //結果を取得する
//                UniList uL = (UniList)e.Result;
////                Label1.Text = result.ToString() + "回で完了しました。";

//                string t = uL.ts.ToString();
//                if (t.LastIndexOf('.') != -1) t = t.Remove(t.LastIndexOf('.'));
//                labelDulation.Text = dulation + t;
//                labelDiffCnt.Text += diffCnt.ToString();

//                WriteResult();
//            }

//            //Button1を有効に戻す
//            //Button1.Enabled = true;

//        }


    }
}
