using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace EditBarcode
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Text = "EditBarcode"; // ToDo: dynamically read from the program name
            labelHigh.Text = "High";
            labelMiddle.Text = "Middle";
            labelLow.Text = "Low";
            PbtnRead.Text = "Read";
            PbtnWrite.Text = "Write";

            // 2-digit
            textBoxHigh.MaxLength = 2;
            textBoxMiddle.MaxLength = 2;
            textBoxLow.MaxLength = 2;
        }

        private void PbtnRead_Click(object sender, EventArgs e)
        {
            ClbBarcode cb = new ClbBarcode();
            string f = string.Empty;
            if (cb.GetUserConfigXml(ref f)) // Get barcode values from UserConfig
            {
                textBoxHigh.Text = cb.BarcodeValues[0];
                textBoxMiddle.Text = cb.BarcodeValues[1];
                textBoxLow.Text = cb.BarcodeValues[2];
            }
            else
            {
                MessageBox.Show(f, this.Text, MessageBoxButtons.OK);
            }
        }

        private void PbtnWrite_Click(object sender, EventArgs e)
        {
            ClbBarcode cb = new ClbBarcode();
            cb.BarcodeValues[0] = textBoxHigh.Text;
            cb.BarcodeValues[1] = textBoxMiddle.Text;
            cb.BarcodeValues[2] = textBoxLow.Text;

            // write back to a config file
            string f = string.Empty;
            if (cb.SetUserConfigXml(ref f))
            {
                MessageBox.Show("Updated.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(f, this.Text, MessageBoxButtons.OK);
            }
        }

    }
}
