namespace PackStrokes
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
            this.PbtnStart = new System.Windows.Forms.Button();
            this.PbtnFileOpen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PbtnStart
            // 
            this.PbtnStart.Location = new System.Drawing.Point(73, 192);
            this.PbtnStart.Name = "PbtnStart";
            this.PbtnStart.Size = new System.Drawing.Size(148, 36);
            this.PbtnStart.TabIndex = 0;
            this.PbtnStart.Text = "button1";
            this.PbtnStart.UseVisualStyleBackColor = true;
            this.PbtnStart.Click += new System.EventHandler(this.PbtnStart_Click);
            // 
            // PbtnFileOpen
            // 
            this.PbtnFileOpen.Location = new System.Drawing.Point(76, 144);
            this.PbtnFileOpen.Name = "PbtnFileOpen";
            this.PbtnFileOpen.Size = new System.Drawing.Size(144, 36);
            this.PbtnFileOpen.TabIndex = 1;
            this.PbtnFileOpen.Text = "button1";
            this.PbtnFileOpen.UseVisualStyleBackColor = true;
            this.PbtnFileOpen.Click += new System.EventHandler(this.PbtnFileOpen_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.PbtnFileOpen);
            this.Controls.Add(this.PbtnStart);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PbtnStart;
        private System.Windows.Forms.Button PbtnFileOpen;
    }
}

