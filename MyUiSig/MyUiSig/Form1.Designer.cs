namespace MyUiSig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnSign = new System.Windows.Forms.Button();
            this.wizCtl = new Florentis.AxWizCtl();
            this.pbSignatureBox = new System.Windows.Forms.PictureBox();
            this.txtName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.wizCtl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSignatureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSign
            // 
            this.btnSign.Location = new System.Drawing.Point(329, 12);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(75, 23);
            this.btnSign.TabIndex = 0;
            this.btnSign.Text = "button1";
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
            // 
            // wizCtl
            // 
            this.wizCtl.Enabled = true;
            this.wizCtl.Location = new System.Drawing.Point(12, 124);
            this.wizCtl.Name = "wizCtl";
            this.wizCtl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("wizCtl.OcxState")));
            this.wizCtl.Size = new System.Drawing.Size(397, 240);
            this.wizCtl.TabIndex = 1;
            this.wizCtl.UseWaitCursor = true;
            // 
            // pbSignatureBox
            // 
            this.pbSignatureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSignatureBox.Location = new System.Drawing.Point(482, 144);
            this.pbSignatureBox.Name = "pbSignatureBox";
            this.pbSignatureBox.Size = new System.Drawing.Size(214, 98);
            this.pbSignatureBox.TabIndex = 2;
            this.pbSignatureBox.TabStop = false;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(13, 85);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(310, 22);
            this.txtName.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 376);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.pbSignatureBox);
            this.Controls.Add(this.wizCtl);
            this.Controls.Add(this.btnSign);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.wizCtl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSignatureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSign;
        private Florentis.AxWizCtl wizCtl;
        private System.Windows.Forms.PictureBox pbSignatureBox;
        private System.Windows.Forms.TextBox txtName;
    }
}

