using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Florentis;    // Signature SDK

namespace MyUiSig
{
    public partial class Form1 : Form
    {
        WizardCallback callback;
        SigObj sigObj;

        public Form1()
        {
            InitializeComponent();
        
            callback = new WizardCallback();
            callback.EventHandler = null;
            wizCtl.SetEventHandler(callback);
            sigObj = new SigObj();
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
 //           ShowCustomSignWindow();
        }


        // Loadで実行しないとダメ
        private void ShowCustomSignWindow()
        {
            try
            {

                bool success = wizCtl.PadConnect();
                if (success)
                {
                    if (wizCtl.PadWidth != 800)
                    {
                        //                   success = false;
                    }
                }

                if (success != true)
                {
                    MessageBox.Show("This demo is only for STU-530");
                    return;
                }

                wizCtl.Reset();

                wizCtl.AddObject(ObjectType.ObjectImage, "", "left", "top", "sign_area.png", null);
                wizCtl.AddObject(ObjectType.ObjectImage, "OK", "200", "140", "button_ok.png", null);
                //            wizCtl.AddObject(ObjectType.ObjectImage, "Cancel", "550", "300", "cancel_button.png", null);
                wizCtl.AddObject(ObjectType.ObjectText, "who", "30", "220", "山田", null);
                wizCtl.AddObject(ObjectType.ObjectText, "why", "200", "180", "Acknowledged and confirmed", null);
                wizCtl.AddObject(ObjectType.ObjectSignature, "signature", 0, 0, sigObj, null);

                callback.EventHandler = new WizardCallback.Handler(button_handler);
                wizCtl.SetEventHandler(callback);

                wizCtl.Display();
            }
            catch (Exception ex)
            {
                Debug.Print("ShowCustomSignWindow: {0}", ex.Message);
            }
        }

        private void button_handler(object clt, object id, object type)
        {
            switch (id.ToString())
            {
                case "OK":
                    {
                        ShowSignature();
                        //loadColors();
                        break;
                    }
                case "Cancel":
                    {
                        //closeWizard();
                        //SignatureBox.Image = null;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        private void ShowSignature()
        {
            try
            {
                Debug.Print("scriptCompleted");
                //           SigObj sigObj = (SigObj)SigCtl.Signature;

                if (sigObj.IsCaptured)
                {
                    sigObj.set_ExtraData("AdditionalData", "C# Wizard test: Additional data");
                    String filename = "sig1.png";
                    sigObj.RenderBitmap(filename, 200, 150, "image/png", 0.5f, 0xff0000, 0xffffff, -1.0f, -1.0f, RBFlags.RenderOutputFilename | RBFlags.RenderColor32BPP | RBFlags.RenderEncodeData);
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        sigImage.Image = System.Drawing.Image.FromStream(fs);     // display image on window
                        fs.Close();
                    }

                    sigImage.Load(filename);
                }
                closeWizard();
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
            }

        }

        private void closeWizard()
        {
            Debug.Print("closeWizard()");
//            ScriptIsRunning = false;
            wizCtl.Reset();
            wizCtl.Display();
            wizCtl.PadDisconnect();
            callback.EventHandler = null;       // remove handler
            wizCtl.SetEventHandler(callback);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSign.Text = "Sign";

            ShowCustomSignWindow();
        }
    }
}
