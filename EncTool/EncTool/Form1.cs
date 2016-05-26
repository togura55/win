using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EncTool
{
    public partial class EncToolForm : Form
    {
        static string password = "sdllab.1";

        public EncToolForm()
        {
            InitializeComponent();
        }

        private void buttonEncript_Click(object sender, EventArgs e)
        {
            string source = textBoxEncript.ToString();
            string target = "";
            if (source.Length != 0)
            {
                // call encript
                target = Encript.EncryptString(source, password);
                if (target.Length != 0)
                    textBoxDecript.Text = target;
            }
            else
                MessageBox.Show("No strings are in the editbox");
        }

        private void buttonDecript_Click(object sender, EventArgs e)
        {
            string source = textBoxDecript.ToString();
            string target = "";
            if (source.Length != 0)
            {
                // call decript
                target = Encript.DecryptString(source, password);
                if (target.Length != 0)
                    textBoxEncript.Text = target;
            }
            else
                MessageBox.Show("No strings are in the editbox");

        }
    }
}
