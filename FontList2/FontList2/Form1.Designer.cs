namespace FontList2
{
    partial class FormFontList2
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxFont = new System.Windows.Forms.ComboBox();
            this.textBoxStart = new System.Windows.Forms.TextBox();
            this.textBoxEnd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelUnicodeRange = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.labelDiffCnt = new System.Windows.Forms.Label();
            this.labelDulation = new System.Windows.Forms.Label();
            this.checkBoxWrite = new System.Windows.Forms.CheckBox();
            this.progressBarFont = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.buttonRead = new System.Windows.Forms.Button();
            this.textBoxUniList = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelSum = new System.Windows.Forms.Label();
            this.radioButtonSolo = new System.Windows.Forms.RadioButton();
            this.radioButtonGroup = new System.Windows.Forms.RadioButton();
            this.listViewUniList = new System.Windows.Forms.ListView();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxFont
            // 
            this.comboBoxFont.FormattingEnabled = true;
            this.comboBoxFont.Location = new System.Drawing.Point(72, 246);
            this.comboBoxFont.Name = "comboBoxFont";
            this.comboBoxFont.Size = new System.Drawing.Size(184, 20);
            this.comboBoxFont.TabIndex = 0;
            // 
            // textBoxStart
            // 
            this.textBoxStart.Location = new System.Drawing.Point(105, 301);
            this.textBoxStart.Name = "textBoxStart";
            this.textBoxStart.Size = new System.Drawing.Size(65, 19);
            this.textBoxStart.TabIndex = 1;
            // 
            // textBoxEnd
            // 
            this.textBoxEnd.Location = new System.Drawing.Point(236, 301);
            this.textBoxEnd.Name = "textBoxEnd";
            this.textBoxEnd.Size = new System.Drawing.Size(65, 19);
            this.textBoxEnd.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "From (Hex.):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 304);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "To (Hex.):";
            // 
            // labelUnicodeRange
            // 
            this.labelUnicodeRange.AutoSize = true;
            this.labelUnicodeRange.Location = new System.Drawing.Point(29, 280);
            this.labelUnicodeRange.Name = "labelUnicodeRange";
            this.labelUnicodeRange.Size = new System.Drawing.Size(82, 12);
            this.labelUnicodeRange.TabIndex = 5;
            this.labelUnicodeRange.Text = "Unicode Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 249);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Font";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(31, 352);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(94, 23);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "&Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(189, 330);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(96, 60);
            this.richTextBox.TabIndex = 8;
            this.richTextBox.Text = "";
            // 
            // labelDiffCnt
            // 
            this.labelDiffCnt.AutoSize = true;
            this.labelDiffCnt.Location = new System.Drawing.Point(29, 378);
            this.labelDiffCnt.Name = "labelDiffCnt";
            this.labelDiffCnt.Size = new System.Drawing.Size(58, 12);
            this.labelDiffCnt.TabIndex = 9;
            this.labelDiffCnt.Text = "Diff Cnt. : ";
            // 
            // labelDulation
            // 
            this.labelDulation.AutoSize = true;
            this.labelDulation.Location = new System.Drawing.Point(29, 395);
            this.labelDulation.Name = "labelDulation";
            this.labelDulation.Size = new System.Drawing.Size(53, 12);
            this.labelDulation.TabIndex = 10;
            this.labelDulation.Text = "Dulation: ";
            // 
            // checkBoxWrite
            // 
            this.checkBoxWrite.AutoSize = true;
            this.checkBoxWrite.Location = new System.Drawing.Point(31, 330);
            this.checkBoxWrite.Name = "checkBoxWrite";
            this.checkBoxWrite.Size = new System.Drawing.Size(110, 16);
            this.checkBoxWrite.TabIndex = 11;
            this.checkBoxWrite.Text = "Write a file (csv)";
            this.checkBoxWrite.UseVisualStyleBackColor = true;
            // 
            // progressBarFont
            // 
            this.progressBarFont.Location = new System.Drawing.Point(189, 396);
            this.progressBarFont.Name = "progressBarFont";
            this.progressBarFont.Size = new System.Drawing.Size(98, 16);
            this.progressBarFont.TabIndex = 12;
            // 
            // buttonRead
            // 
            this.buttonRead.Location = new System.Drawing.Point(210, 65);
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.Size = new System.Drawing.Size(51, 23);
            this.buttonRead.TabIndex = 13;
            this.buttonRead.Text = "&Read";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // textBoxUniList
            // 
            this.textBoxUniList.Location = new System.Drawing.Point(10, 67);
            this.textBoxUniList.Name = "textBoxUniList";
            this.textBoxUniList.Size = new System.Drawing.Size(136, 19);
            this.textBoxUniList.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelSum);
            this.groupBox1.Controls.Add(this.radioButtonSolo);
            this.groupBox1.Controls.Add(this.radioButtonGroup);
            this.groupBox1.Controls.Add(this.listViewUniList);
            this.groupBox1.Controls.Add(this.buttonOpen);
            this.groupBox1.Controls.Add(this.buttonRead);
            this.groupBox1.Controls.Add(this.textBoxUniList);
            this.groupBox1.Location = new System.Drawing.Point(23, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 218);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Unicode List";
            // 
            // labelSum
            // 
            this.labelSum.AutoSize = true;
            this.labelSum.Location = new System.Drawing.Point(9, 197);
            this.labelSum.Name = "labelSum";
            this.labelSum.Size = new System.Drawing.Size(0, 12);
            this.labelSum.TabIndex = 19;
            // 
            // radioButtonSolo
            // 
            this.radioButtonSolo.AutoSize = true;
            this.radioButtonSolo.Location = new System.Drawing.Point(14, 37);
            this.radioButtonSolo.Name = "radioButtonSolo";
            this.radioButtonSolo.Size = new System.Drawing.Size(205, 16);
            this.radioButtonSolo.TabIndex = 18;
            this.radioButtonSolo.TabStop = true;
            this.radioButtonSolo.Text = "Read Unicode Codepoint List (solo)";
            this.radioButtonSolo.UseVisualStyleBackColor = true;
            // 
            // radioButtonGroup
            // 
            this.radioButtonGroup.AutoSize = true;
            this.radioButtonGroup.Location = new System.Drawing.Point(14, 18);
            this.radioButtonGroup.Name = "radioButtonGroup";
            this.radioButtonGroup.Size = new System.Drawing.Size(150, 16);
            this.radioButtonGroup.TabIndex = 17;
            this.radioButtonGroup.TabStop = true;
            this.radioButtonGroup.Text = "Read Unicode Block List";
            this.radioButtonGroup.UseVisualStyleBackColor = true;
            this.radioButtonGroup.CheckedChanged += new System.EventHandler(this.radioButtonGroup_CheckedChanged);
            // 
            // listViewUniList
            // 
            this.listViewUniList.Location = new System.Drawing.Point(8, 95);
            this.listViewUniList.Name = "listViewUniList";
            this.listViewUniList.Size = new System.Drawing.Size(253, 93);
            this.listViewUniList.TabIndex = 16;
            this.listViewUniList.UseCompatibleStateImageBehavior = false;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(152, 65);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(26, 23);
            this.buttonOpen.TabIndex = 15;
            this.buttonOpen.Text = "...";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // FormFontList2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 439);
            this.Controls.Add(this.progressBarFont);
            this.Controls.Add(this.checkBoxWrite);
            this.Controls.Add(this.labelDulation);
            this.Controls.Add(this.labelDiffCnt);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelUnicodeRange);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxEnd);
            this.Controls.Add(this.textBoxStart);
            this.Controls.Add(this.comboBoxFont);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormFontList2";
            this.Text = "FontList2";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxFont;
        private System.Windows.Forms.TextBox textBoxStart;
        private System.Windows.Forms.TextBox textBoxEnd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelUnicodeRange;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Label labelDiffCnt;
        private System.Windows.Forms.Label labelDulation;
        private System.Windows.Forms.CheckBox checkBoxWrite;
        private System.Windows.Forms.ProgressBar progressBarFont;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.TextBox textBoxUniList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listViewUniList;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.RadioButton radioButtonSolo;
        private System.Windows.Forms.RadioButton radioButtonGroup;
        private System.Windows.Forms.Label labelSum;
    }
}

