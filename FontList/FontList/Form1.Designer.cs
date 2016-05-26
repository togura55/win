namespace FontList
{
    partial class FormFontList
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.labelDiffCnt = new System.Windows.Forms.Label();
            this.labelDulation = new System.Windows.Forms.Label();
            this.checkBoxWrite = new System.Windows.Forms.CheckBox();
            this.progressBarFont = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // comboBoxFont
            // 
            this.comboBoxFont.FormattingEnabled = true;
            this.comboBoxFont.Location = new System.Drawing.Point(62, 27);
            this.comboBoxFont.Name = "comboBoxFont";
            this.comboBoxFont.Size = new System.Drawing.Size(184, 20);
            this.comboBoxFont.TabIndex = 0;
            // 
            // textBoxStart
            // 
            this.textBoxStart.Location = new System.Drawing.Point(73, 83);
            this.textBoxStart.Name = "textBoxStart";
            this.textBoxStart.Size = new System.Drawing.Size(65, 19);
            this.textBoxStart.TabIndex = 1;
            // 
            // textBoxEnd
            // 
            this.textBoxEnd.Location = new System.Drawing.Point(181, 83);
            this.textBoxEnd.Name = "textBoxEnd";
            this.textBoxEnd.Size = new System.Drawing.Size(65, 19);
            this.textBoxEnd.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "From:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "To:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Unicode Range (Hex.)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Font";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(23, 134);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(94, 23);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "&Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(181, 119);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(96, 53);
            this.richTextBox.TabIndex = 8;
            this.richTextBox.Text = "";
            // 
            // labelDiffCnt
            // 
            this.labelDiffCnt.AutoSize = true;
            this.labelDiffCnt.Location = new System.Drawing.Point(21, 160);
            this.labelDiffCnt.Name = "labelDiffCnt";
            this.labelDiffCnt.Size = new System.Drawing.Size(58, 12);
            this.labelDiffCnt.TabIndex = 9;
            this.labelDiffCnt.Text = "Diff Cnt. : ";
            // 
            // labelDulation
            // 
            this.labelDulation.AutoSize = true;
            this.labelDulation.Location = new System.Drawing.Point(21, 177);
            this.labelDulation.Name = "labelDulation";
            this.labelDulation.Size = new System.Drawing.Size(53, 12);
            this.labelDulation.TabIndex = 10;
            this.labelDulation.Text = "Dulation: ";
            // 
            // checkBoxWrite
            // 
            this.checkBoxWrite.AutoSize = true;
            this.checkBoxWrite.Location = new System.Drawing.Point(23, 112);
            this.checkBoxWrite.Name = "checkBoxWrite";
            this.checkBoxWrite.Size = new System.Drawing.Size(80, 16);
            this.checkBoxWrite.TabIndex = 11;
            this.checkBoxWrite.Text = "Write a file";
            this.checkBoxWrite.UseVisualStyleBackColor = true;
            // 
            // progressBarFont
            // 
            this.progressBarFont.Location = new System.Drawing.Point(181, 178);
            this.progressBarFont.Name = "progressBarFont";
            this.progressBarFont.Size = new System.Drawing.Size(98, 16);
            this.progressBarFont.TabIndex = 12;
            // 
            // FormFontList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 207);
            this.Controls.Add(this.progressBarFont);
            this.Controls.Add(this.checkBoxWrite);
            this.Controls.Add(this.labelDulation);
            this.Controls.Add(this.labelDiffCnt);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxEnd);
            this.Controls.Add(this.textBoxStart);
            this.Controls.Add(this.comboBoxFont);
            this.Name = "FormFontList";
            this.Text = "FontList";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxFont;
        private System.Windows.Forms.TextBox textBoxStart;
        private System.Windows.Forms.TextBox textBoxEnd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Label labelDiffCnt;
        private System.Windows.Forms.Label labelDulation;
        private System.Windows.Forms.CheckBox checkBoxWrite;
        private System.Windows.Forms.ProgressBar progressBarFont;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
    }
}

