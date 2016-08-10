using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyUiSig
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            ShowCustomSignWindow();
        }

        private void ShowCustomSignWindow()
        {
            bool success = wizCtl.PadConnect();
            if (success)
            {
                if (wizCtl.PadWidth != 800)
                {
                    success = false;
                }
            }

            if (success != true)
            {
                MessageBox.Show("This demo is only for STU-530");
                return;
            }

            wizCtl.Reset();

            wizCtl.AddObject(ObjectType.ObjectImage, "", "left", "top", "signature_color.png", null);
            wizCtl.AddObject(ObjectType.ObjectImage, "OK", "550", "384", "accept_button.png", null);
            wizCtl.AddObject(ObjectType.ObjectImage, "Cancel", "550", "300", "cancel_button.png", null);
            wizCtl.AddObject(ObjectType.ObjectText, "who", "30", "460", txtName.Text, null);
            wizCtl.AddObject(ObjectType.ObjectText, "why", "334", "460", "Acknowledged and confirmed", null);
            wizCtl.AddObject(ObjectType.ObjectSignature, "signature", 0, 0, sigObj, null);


            callback.EventHandler = new WizardCallback.Handler(button_handler);
            wizCtl.SetEventHandler(callback);


            wizCtl.Display();
        }
    }
}
