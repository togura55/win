/******************************************************* 

  TestSigCapt.cs
  
  Displays a form with a button to start signature capture
  The captured signature is encoded in an image file which is displayed on the form
  
  Copyright (c) 2015 Wacom GmbH. All rights reserved.
  
********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Florentis;
namespace TestSigCapt
{
    public partial class TestSigCapt : Form
    {
        const string FILENAME = "sig1.png";
        string appDataPath;

        SigObj mSigObj = null;

        public TestSigCapt()
        {
            InitializeComponent();

            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\" + "Wacom"
                + "\\" + this.ProductName;
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            print("btnSign was pressed");
            SigCtl sigCtl = new SigCtl();
            sigCtl.Licence = "eyJhbGciOiJSUzUxMiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI3YmM5Y2IxYWIxMGE0NmUxODI2N2E5MTJkYTA2ZTI3NiIsImV4cCI6MjE0NzQ4MzY0NywiaWF0IjoxNTYwOTUwMjcyLCJyaWdodHMiOlsiU0lHX1NES19DT1JFIiwiU0lHQ0FQVFhfQUNDRVNTIl0sImRldmljZXMiOlsiV0FDT01fQU5ZIl0sInR5cGUiOiJwcm9kIiwibGljX25hbWUiOiJTaWduYXR1cmUgU0RLIiwid2Fjb21faWQiOiI3YmM5Y2IxYWIxMGE0NmUxODI2N2E5MTJkYTA2ZTI3NiIsImxpY191aWQiOiJiODUyM2ViYi0xOGI3LTQ3OGEtYTlkZS04NDlmZTIyNmIwMDIiLCJhcHBzX3dpbmRvd3MiOltdLCJhcHBzX2lvcyI6W10sImFwcHNfYW5kcm9pZCI6W10sIm1hY2hpbmVfaWRzIjpbXX0.ONy3iYQ7lC6rQhou7rz4iJT_OJ20087gWz7GtCgYX3uNtKjmnEaNuP3QkjgxOK_vgOrTdwzD-nm-ysiTDs2GcPlOdUPErSp_bcX8kFBZVmGLyJtmeInAW6HuSp2-57ngoGFivTH_l1kkQ1KMvzDKHJbRglsPpd4nVHhx9WkvqczXyogldygvl0LRidyPOsS5H2GYmaPiyIp9In6meqeNQ1n9zkxSHo7B11mp_WXJXl0k1pek7py8XYCedCNW5qnLi4UCNlfTd6Mk9qz31arsiWsesPeR9PN121LBJtiPi023yQU8mgb9piw_a-ccciviJuNsEuRDN3sGnqONG3dMSA";
            DynamicCapture dc = new DynamicCaptureClass();
            DynamicCaptureResult res = dc.Capture(sigCtl, "Who", "Why", null, null);
            if (res == DynamicCaptureResult.DynCaptOK)
            {
                print("signature captured successfully");
                //SigObj sigObj = (SigObj)sigCtl.Signature;
                btnConvert.Enabled = true;
                mSigObj = (SigObj)sigCtl.Signature;
                mSigObj.set_ExtraData("AdditionalData", "C# test: Additional data");
                try
                {
                    mSigObj.RenderBitmap(appDataPath + "\\" + FILENAME, 200, 150, "image/png", 0.5f, 0xff0000, 0xffffff, 10.0f, 10.0f, RBFlags.RenderOutputFilename | RBFlags.RenderColor32BPP | RBFlags.RenderEncodeData);

                    // 画像ファイルの読み込み、表示
                    using (FileStream fs = new FileStream(appDataPath + "\\" + FILENAME, FileMode.Open, FileAccess.Read))
                    {
                        sigImage.Image = System.Drawing.Image.FromStream(fs);
                        fs.Close();
                    }

                    // sigImage.Load(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               
            }
            else
            {
                print("Signature capture error res=" + (int)res + "  ( " + res + " )");
                switch (res)
                {
                    case DynamicCaptureResult.DynCaptCancel: print("signature cancelled"); break;
                    case DynamicCaptureResult.DynCaptError: print("no capture service available"); break;
                    case DynamicCaptureResult.DynCaptPadError: print("signing device error"); break;
                    default: print("Unexpected error code "); break;
                }
            }
        }
        private void print(string txt)
        {
            txtDisplay.Text += txt + "\r\n";
            txtDisplay.SelectionStart = txtDisplay.TextLength;
            txtDisplay.ScrollToCaret();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "ファイルを開く";
            dialog.Filter = "PNG(*.png)|*.png";

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                SigObj sigObj = new SigObj();

                using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    // SigObjとして読み込み
                    int len = (int)fs.Length;
                    var data = new byte[len];
                    fs.Read(data, 0, len);

                    sigObj.ReadEncodedBitmap(data);
                    DateTime dt = sigObj.When;

                    // 画像として読み込み
                    sigImage.Image = System.Drawing.Image.FromStream(fs);
                }

                mSigObj = sigObj;
                btnConvert.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("load error: " + ex.Message);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (mSigObj == null)
            {
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "sig.fss";
            dialog.Filter = "FSS(*.fss)|*.fss";

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                // FSSデータを保存
                using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = (byte[])mSigObj.SigData;
                    fs.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("convert error: " + ex.Message);
            }
        }
    }
}

