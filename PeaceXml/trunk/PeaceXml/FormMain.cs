using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PeaceXml
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Set UI and Captions
            //   ToDo: caption strings should be loaded from the resx 
            this.Text = Program.appName + " " + Program.version;    // Title
            pbtn_exec.Text = "Execute";
            pbtn_close.Text = "Close";
            pbtn_help.Text = "Help";
            pbtn_sourcedir.Text = "Select";
            pbtn_destdir.Text = "Select";
            label_sourcedir.Text = "Source Directory";
            label_destdir.Text = "Destination Directory";
            groupBox_mode.Text = "Execution Mode";
            radioButton_merge.Text = "Merge";
            radioButton_split.Text = "Split";
            tabPage_main.Text = "General";
            tabPage_option.Text = "Option";

            label_df.Text = "Destination filename suffix.\n(Use folder string if blank)";
            label_ex.Text = "File extension string";
            label_ca.Text = "Merge file capacity (KB)";
            label_re.Text = "Root element name";
            label_se.Text = "Sub element name";
            label_fa.Text = "File name attribute";
            label_pa.Text = "Path name attribute";
            label_ha.Text = "Header name attribute";
            label_sf.Text = "Merge sub folder(s)";

            // Show initial values 
            textBox_sourcedir.Text = Program.sourcePath;
            textBox_destdir.Text = Program.destPath;
            if (Program.fMerge)
                radioButton_merge.Checked = true;
            else
                radioButton_split.Checked = true;

            textBox_df.Text = Program.destFilename;
            textBox_ex.Text = Program.extension;
            textBox_ca.Text = (Program.capacity/1000).ToString (); // ToDo: offset value?
            textBox_re.Text = Program.rootElement;
            textBox_se.Text = Program.subElement;
            textBox_fa.Text = Program.filesAttr;
            textBox_pa.Text = Program.pathAttr;
            textBox_ha.Text = Program.headerAttr;
            //(Bug #14511)Source Directory内のSubFolderを処理対象とするか否か選択できるよう機能追加
            if (Program.fSubFolder)
                checkBox_sf.Checked = true;
            else
                checkBox_sf.Checked = false;
            //checkBox_sf.Checked = Program.fSubFolder;
        }

        // Show the help window
        private void pbtn_help_Click(object sender, EventArgs e)
        {
            FormHelp fh = new FormHelp();
            fh.Show(this);
        }

        // Close button proc
        private void pbtn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Exec button proc
        private void pbtn_exec_Click(object sender, EventArgs e)
        {
            // get UI state in main
            Program.sourcePath = textBox_sourcedir.Text;
            Program.destPath = textBox_destdir.Text;
            // get radio button state and set the flag
            Program.fMerge = radioButton_merge.Checked ? true : false;
            //     in optional
            Program.destFilename = textBox_df.Text;
            Program.extension = textBox_ex.Text;
            Program.capacity = long.Parse(textBox_ca.Text) * 1000; // ToDo: offset value?
            Program.rootElement = textBox_re.Text;
            Program.subElement = textBox_se.Text;
            Program.filesAttr = textBox_fa.Text;
            Program.pathAttr = textBox_pa.Text;
            Program.headerAttr = textBox_ha.Text;
            Program.fSubFolder = checkBox_sf.Checked ? true : false;

            // run 
            MergeSplit ms = new MergeSplit();

            if (Program.fMerge)
            {
                Program.sw.Start();
                if (ms.MergeFiles())
                {
                    Program.sw.Stop(); // Stopwatch
                    MessageBox.Show(string.Format( Program.lastMessage + " Elaped Time: {0}", Program.sw.Elapsed),
                        "Merge Completed",MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    // show error dialog
                    MessageBox.Show(Program.lastMessage, "Merge Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else  // Split
            {
                Program.sw.Start();
                if (ms.SplitFile())
                {
                    Program.sw.Stop();
                    MessageBox.Show(string.Format(Program.lastMessage + " Elaped Time: {0}", Program.sw.Elapsed),
                        "Split Completed", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    // show error dialog
                    MessageBox.Show(Program.lastMessage, "Split Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Select button proc
        private void pbtn_sourcedir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select Source Directory";

            // Desktop as default state: ToDo
            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbd.SelectedPath = @"C:\Windows";  // ToDo: 
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = true;

            //Show dialog
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                // Show on UI
                textBox_sourcedir.Text = fbd.SelectedPath;
            }
        }

        // Select button proc
        private void pbtn_destdir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select Destination Directory";

            // Desktop as default state: ToDo
            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbd.SelectedPath = @"C:\Windows";   // ToDo:
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = true;

            //show dialog
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                // Show on UI
                textBox_destdir.Text = fbd.SelectedPath;
            }
        }

        private void tabPage_option_Click(object sender, EventArgs e)
        {

        }

    }
}
