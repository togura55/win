namespace FindSignpad
{
    partial class Form1
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
            this.pbtnStart = new System.Windows.Forms.Button();
            this.pbtnStop = new System.Windows.Forms.Button();
            this.labelOutput = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbtnStart
            // 
            this.pbtnStart.Location = new System.Drawing.Point(56, 43);
            this.pbtnStart.Name = "pbtnStart";
            this.pbtnStart.Size = new System.Drawing.Size(75, 23);
            this.pbtnStart.TabIndex = 0;
            this.pbtnStart.Text = "button1";
            this.pbtnStart.UseVisualStyleBackColor = true;
            this.pbtnStart.Click += new System.EventHandler(this.pbtnStart_Click);
            // 
            // pbtnStop
            // 
            this.pbtnStop.Location = new System.Drawing.Point(178, 43);
            this.pbtnStop.Name = "pbtnStop";
            this.pbtnStop.Size = new System.Drawing.Size(75, 23);
            this.pbtnStop.TabIndex = 1;
            this.pbtnStop.Text = "button2";
            this.pbtnStop.UseVisualStyleBackColor = true;
            this.pbtnStop.Click += new System.EventHandler(this.pbtnStop_Click);
            // 
            // labelOutput
            // 
            this.labelOutput.AutoSize = true;
            this.labelOutput.Location = new System.Drawing.Point(56, 99);
            this.labelOutput.Name = "labelOutput";
            this.labelOutput.Size = new System.Drawing.Size(43, 15);
            this.labelOutput.TabIndex = 2;
            this.labelOutput.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 148);
            this.Controls.Add(this.labelOutput);
            this.Controls.Add(this.pbtnStop);
            this.Controls.Add(this.pbtnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pbtnStart;
        private System.Windows.Forms.Button pbtnStop;
        private System.Windows.Forms.Label labelOutput;
    }
}

