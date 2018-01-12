namespace EditBarcode
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxHigh = new System.Windows.Forms.TextBox();
            this.textBoxMiddle = new System.Windows.Forms.TextBox();
            this.textBoxLow = new System.Windows.Forms.TextBox();
            this.labelHigh = new System.Windows.Forms.Label();
            this.labelMiddle = new System.Windows.Forms.Label();
            this.labelLow = new System.Windows.Forms.Label();
            this.PbtnRead = new System.Windows.Forms.Button();
            this.PbtnWrite = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxHigh
            // 
            this.textBoxHigh.Location = new System.Drawing.Point(94, 52);
            this.textBoxHigh.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxHigh.Name = "textBoxHigh";
            this.textBoxHigh.Size = new System.Drawing.Size(40, 19);
            this.textBoxHigh.TabIndex = 0;
            // 
            // textBoxMiddle
            // 
            this.textBoxMiddle.Location = new System.Drawing.Point(94, 85);
            this.textBoxMiddle.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxMiddle.Name = "textBoxMiddle";
            this.textBoxMiddle.Size = new System.Drawing.Size(40, 19);
            this.textBoxMiddle.TabIndex = 1;
            // 
            // textBoxLow
            // 
            this.textBoxLow.Location = new System.Drawing.Point(94, 119);
            this.textBoxLow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxLow.Name = "textBoxLow";
            this.textBoxLow.Size = new System.Drawing.Size(40, 19);
            this.textBoxLow.TabIndex = 2;
            // 
            // labelHigh
            // 
            this.labelHigh.AutoSize = true;
            this.labelHigh.Location = new System.Drawing.Point(37, 56);
            this.labelHigh.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelHigh.Name = "labelHigh";
            this.labelHigh.Size = new System.Drawing.Size(35, 12);
            this.labelHigh.TabIndex = 3;
            this.labelHigh.Text = "label1";
            // 
            // labelMiddle
            // 
            this.labelMiddle.AutoSize = true;
            this.labelMiddle.Location = new System.Drawing.Point(37, 88);
            this.labelMiddle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMiddle.Name = "labelMiddle";
            this.labelMiddle.Size = new System.Drawing.Size(35, 12);
            this.labelMiddle.TabIndex = 4;
            this.labelMiddle.Text = "label2";
            // 
            // labelLow
            // 
            this.labelLow.AutoSize = true;
            this.labelLow.Location = new System.Drawing.Point(37, 123);
            this.labelLow.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelLow.Name = "labelLow";
            this.labelLow.Size = new System.Drawing.Size(35, 12);
            this.labelLow.TabIndex = 5;
            this.labelLow.Text = "label3";
            // 
            // PbtnRead
            // 
            this.PbtnRead.Location = new System.Drawing.Point(39, 184);
            this.PbtnRead.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PbtnRead.Name = "PbtnRead";
            this.PbtnRead.Size = new System.Drawing.Size(56, 24);
            this.PbtnRead.TabIndex = 6;
            this.PbtnRead.Text = "button1";
            this.PbtnRead.UseVisualStyleBackColor = true;
            this.PbtnRead.Click += new System.EventHandler(this.PbtnRead_Click);
            // 
            // PbtnWrite
            // 
            this.PbtnWrite.Location = new System.Drawing.Point(143, 184);
            this.PbtnWrite.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PbtnWrite.Name = "PbtnWrite";
            this.PbtnWrite.Size = new System.Drawing.Size(56, 24);
            this.PbtnWrite.TabIndex = 7;
            this.PbtnWrite.Text = "button2";
            this.PbtnWrite.UseVisualStyleBackColor = true;
            this.PbtnWrite.Click += new System.EventHandler(this.PbtnWrite_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 238);
            this.Controls.Add(this.PbtnWrite);
            this.Controls.Add(this.PbtnRead);
            this.Controls.Add(this.labelLow);
            this.Controls.Add(this.labelMiddle);
            this.Controls.Add(this.labelHigh);
            this.Controls.Add(this.textBoxLow);
            this.Controls.Add(this.textBoxMiddle);
            this.Controls.Add(this.textBoxHigh);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHigh;
        private System.Windows.Forms.TextBox textBoxMiddle;
        private System.Windows.Forms.TextBox textBoxLow;
        private System.Windows.Forms.Label labelHigh;
        private System.Windows.Forms.Label labelMiddle;
        private System.Windows.Forms.Label labelLow;
        private System.Windows.Forms.Button PbtnRead;
        private System.Windows.Forms.Button PbtnWrite;
    }
}

