using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            wizCtl.AddObject(ObjectType.ObjectImage, "", "left", "top", "sign_area.bmp", null);
            //            wizCtl.AddObject(ObjectType.ObjectImage, "OK", "550", "384", "accept_button.png", null);
            //            wizCtl.AddObject(ObjectType.ObjectImage, "Cancel", "550", "300", "cancel_button.png", null);
            wizCtl.AddObject(ObjectType.ObjectText, "who", "30", "460", txtName.Text, null);
            wizCtl.AddObject(ObjectType.ObjectText, "why", "334", "460", "Acknowledged and confirmed", null);
            //            wizCtl.AddObject(ObjectType.ObjectSignature, "signature", 0, 0, sigObj, null);

            callback.EventHandler = new WizardCallback.Handler(button_handler);
            wizCtl.SetEventHandler(callback);


            wizCtl.Display();
        }

        private void button_handler(object clt, object id, object type)
        {
            switch (id.ToString())
            {
                case "OK":
                    {
                        //ShowSignature();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSign.Text = "Sign";

            ShowCustomSignWindow();
        }
    }
}
