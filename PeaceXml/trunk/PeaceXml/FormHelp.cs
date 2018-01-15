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
    public partial class FormHelp : Form
    {
        public FormHelp()
        {
            InitializeComponent();
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {
            // Show help contents
            this.Text = Program.appName + " Help";
            textBox_help.Text = Program.co.CreateHelpContents();
            textBox_help.SelectionStart = 0;
        }

    }
}
