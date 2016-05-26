using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Foldersize
{
    public partial class MainWindow : Form
    {
        static public string folderPath = "";
        
        public MainWindow()
        {
            InitializeComponent();

            // Check .NET Framework version, required 2.0 and later
            // ToDo

            // GUI Updates
            radioButtonSum.Checked = true;

            // Load and Set GUI Strings
            this.Text = Properties.Resources.IDS_CAP_MAINWINDOW;
            buttonFolderPath.Text = Properties.Resources.IDS_PBTN_FOLDERPATH;
            radioButtonSum.Text = Properties.Resources.IDS_RBTN_SUM;
            radioButtonLower.Text = Properties.Resources.IDS_RBTN_LOWER;
            checkBoxNumFiles.Text = Properties.Resources.IDS_CB_NUMFILES;
            checkBoxSize.Text = Properties.Resources.IDS_CB_SIZE;
            radioButtonSize.Text = Properties.Resources.IDS_RBTN_SIZE;
            radioButtonFileName.Text = Properties.Resources.IDS_RBTN_FILENAME;
            radioButtonNumFiles.Text = Properties.Resources.IDS_RBTN_NUMFILES;
            radioButtonDescend.Text = Properties.Resources.IDS_RBTN_DESCEND;
            radioButtonAscend.Text = Properties.Resources.IDS_RBTN_ASCEND;
            groupBoxLayer.Text = Properties.Resources.IDS_GB_LAYER;
            groupBoxDisplay.Text = Properties.Resources.IDS_GB_DISPLAY;
            labelFolderPath.Text = Properties.Resources.IDS_TXT_FOLDERPATH;
            groupBoxSortType.Text = Properties.Resources.IDS_GB_SORTTYPE;
            groupBoxSortOrder.Text = Properties.Resources.IDS_GB_SORTORDER;
            buttonExecute.Text = Properties.Resources.IDS_PBTN_EXECUTE;
            buttonCopy.Text = Properties.Resources.IDS_PBTN_COPY;
            buttonQuit.Text = Properties.Resources.IDS_PBTN_QUIT;
        }

        private void buttonFolderPath_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialogクラスのインスタンスを作成
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fbd.Description = "フォルダを指定してください。";
            //ルートフォルダを指定する
            //デフォルトでDesktop
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fbd.SelectedPath = @"C:\Windows";
            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fbd.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                Console.WriteLine(fbd.SelectedPath);

                // Show on UI
                textBoxFolderPath.Text = fbd.SelectedPath;
            }
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            // フォルダパスの取得
            if (textBoxFolderPath.Text.Length == 0)
                MessageBox.Show("No Target Folder Path in the Edit Box",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            else
            {
                long size = 0;
                string result = "";

                folderPath = textBoxFolderPath.Text;
                DirectoryInfo di = new DirectoryInfo(folderPath);

                if (radioButtonSum.Checked)
                {
                    // This folder
                    size = GetDirectorySize(di);

                    result = folderPath + " " + size.ToString();
                }
                else
                {
                    // Retrieve lower layer folders

                    //指定フォルダ以下のサブフォルダをすべて取得する
                    //ワイルドカード"*"は、すべてのフォルダを意味する
                    string[] subFolders = System.IO.Directory.GetDirectories(
                       folderPath, "*", System.IO.SearchOption.AllDirectories);

                    // Retrieve lower layer files

                    //指定フォルダ以下のファイルをすべて取得
                    //ワイルドカード"*"は、すべてのファイルを意味する
                    string[] files = System.IO.Directory.GetFiles(
                       folderPath, "*", System.IO.SearchOption.AllDirectories);

                }
                richTextBoxResult.Text = result;
            }

            //

        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {

        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void radioButtonSum_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonSize_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonDescend_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxNumFiles_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxSize_CheckedChanged(object sender, EventArgs e)
        {

        }

        public static long GetDirectorySize(DirectoryInfo dirInfo)
        {
            long size = 0;

            //フォルダ内の全ファイルの合計サイズを計算する
            foreach (FileInfo fi in dirInfo.GetFiles())
                size += fi.Length;

            //サブフォルダのサイズを合計していく
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                size += GetDirectorySize(di);

            //結果を返す
            return size;
        }

    }
}
