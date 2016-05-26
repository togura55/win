namespace EncTool
{
    partial class EncToolForm
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
            this.textBoxEncript = new System.Windows.Forms.TextBox();
            this.buttonEncript = new System.Windows.Forms.Button();
            this.buttonDecript = new System.Windows.Forms.Button();
            this.textBoxDecript = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxEncript
            // 
            this.textBoxEncript.Location = new System.Drawing.Point(24, 25);
            this.textBoxEncript.Name = "textBoxEncript";
            this.textBoxEncript.Size = new System.Drawing.Size(255, 19);
            this.textBoxEncript.TabIndex = 0;
            // 
            // buttonEncript
            // 
            this.buttonEncript.Location = new System.Drawing.Point(303, 25);
            this.buttonEncript.Name = "buttonEncript";
            this.buttonEncript.Size = new System.Drawing.Size(66, 19);
            this.buttonEncript.TabIndex = 1;
            this.buttonEncript.Text = "Encript";
            this.buttonEncript.UseVisualStyleBackColor = true;
            this.buttonEncript.Click += new System.EventHandler(this.buttonEncript_Click);
            // 
            // buttonDecript
            // 
            this.buttonDecript.Location = new System.Drawing.Point(303, 60);
            this.buttonDecript.Name = "buttonDecript";
            this.buttonDecript.Size = new System.Drawing.Size(66, 19);
            this.buttonDecript.TabIndex = 3;
            this.buttonDecript.Text = "Decript";
            this.buttonDecript.UseVisualStyleBackColor = true;
            this.buttonDecript.Click += new System.EventHandler(this.buttonDecript_Click);
            // 
            // textBoxDecript
            // 
            this.textBoxDecript.Location = new System.Drawing.Point(24, 60);
            this.textBoxDecript.Name = "textBoxDecript";
            this.textBoxDecript.Size = new System.Drawing.Size(255, 19);
            this.textBoxDecript.TabIndex = 2;
            // 
            // EncToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 117);
            this.Controls.Add(this.buttonDecript);
            this.Controls.Add(this.textBoxDecript);
            this.Controls.Add(this.buttonEncript);
            this.Controls.Add(this.textBoxEncript);
            this.Name = "EncToolForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxEncript;
        private System.Windows.Forms.Button buttonEncript;
        private System.Windows.Forms.Button buttonDecript;
        private System.Windows.Forms.TextBox textBoxDecript;
    }
}

