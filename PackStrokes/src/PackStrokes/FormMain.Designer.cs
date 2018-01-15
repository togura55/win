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
            this.Pbtn_start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Pbtn_start
            // 
            this.Pbtn_start.Location = new System.Drawing.Point(73, 192);
            this.Pbtn_start.Name = "Pbtn_start";
            this.Pbtn_start.Size = new System.Drawing.Size(148, 36);
            this.Pbtn_start.TabIndex = 0;
            this.Pbtn_start.Text = "button1";
            this.Pbtn_start.UseVisualStyleBackColor = true;
            this.Pbtn_start.Click += new System.EventHandler(this.Pbtn_start_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.Pbtn_start);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Pbtn_start;
    }
}

