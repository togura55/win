using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DispFont
{
    public partial class FormDispFont : Form
    {
        public string fontName = "Arial";
        public int fontSize = 120;

        public FormDispFont()
        {
            InitializeComponent();

            // 
            textBoxUnicode.Text = "962A";
            textBoxSize.Text = fontSize.ToString();

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

                    //フォント名をそのフォントで描画する
                    //g.DrawString(fnt.Name, fnt, Brushes.Black, 0, y);
                    //次の表示位置を計算
                    //y += (int)fnt.GetHeight(g);
                    //リソースを開放する
                    fnt.Dispose();
                }
            }
            int index = this.comboBoxFont.Items.IndexOf(fontName);

            comboBoxFont.SelectedIndex = index;


        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            show_UniChar();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            show_NextChar(-1);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            show_NextChar(1);
        }

        private int show_NextChar(int val)
        {
            int uc = Convert.ToInt32(textBoxUnicode.Text, 16);
            uc += val;
            textBoxUnicode.Text = Convert.ToString(uc, 16);
            show_UniChar();

            return 0;
        }

        private int show_UniChar()
        {
            fontName = comboBoxFont.Text;
            fontSize = int.Parse(textBoxSize.Text);

            this.richTextBox.Font = new Font(fontName, fontSize);
            this.richTextBox.SelectionAlignment = HorizontalAlignment.Center;

            string s = textBoxUnicode.Text;
            if (s.Length != 0)
            {
                
                int codePoint = Convert.ToInt32(s, 16);
                string s1 = Char.ConvertFromUtf32(codePoint);

                this.richTextBox.Text = s1;

                this.richTextBox.SelectAll();
                labelFontName.Text = this.richTextBox.SelectionFont.Name;
            }

            return 0;
        }
    }
}
