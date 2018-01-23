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
            this.listViewDevices = new System.Windows.Forms.ListView();
            this.PbtnScanDevices = new System.Windows.Forms.Button();
            this.PbtnConnect = new System.Windows.Forms.Button();
            this.tbBle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PbtnStart
            // 
            this.PbtnStart.Location = new System.Drawing.Point(31, 213);
            this.PbtnStart.Name = "PbtnStart";
            this.PbtnStart.Size = new System.Drawing.Size(99, 36);
            this.PbtnStart.TabIndex = 0;
            this.PbtnStart.Text = "button1";
            this.PbtnStart.UseVisualStyleBackColor = true;
            this.PbtnStart.Click += new System.EventHandler(this.PbtnStart_Click);
            // 
            // PbtnFileOpen
            // 
            this.PbtnFileOpen.Location = new System.Drawing.Point(152, 213);
            this.PbtnFileOpen.Name = "PbtnFileOpen";
            this.PbtnFileOpen.Size = new System.Drawing.Size(104, 36);
            this.PbtnFileOpen.TabIndex = 1;
            this.PbtnFileOpen.Text = "button1";
            this.PbtnFileOpen.UseVisualStyleBackColor = true;
            this.PbtnFileOpen.Click += new System.EventHandler(this.PbtnFileOpen_Click);
            // 
            // listViewDevices
            // 
            this.listViewDevices.Location = new System.Drawing.Point(31, 12);
            this.listViewDevices.Name = "listViewDevices";
            this.listViewDevices.Size = new System.Drawing.Size(225, 126);
            this.listViewDevices.TabIndex = 3;
            this.listViewDevices.UseCompatibleStateImageBehavior = false;
            this.listViewDevices.SelectedIndexChanged += new System.EventHandler(this.ListViewDevices_SelectedIndexChanged);
            // 
            // PbtnScanDevices
            // 
            this.PbtnScanDevices.Location = new System.Drawing.Point(31, 160);
            this.PbtnScanDevices.Name = "PbtnScanDevices";
            this.PbtnScanDevices.Size = new System.Drawing.Size(99, 35);
            this.PbtnScanDevices.TabIndex = 4;
            this.PbtnScanDevices.Text = "button1";
            this.PbtnScanDevices.UseVisualStyleBackColor = true;
            this.PbtnScanDevices.Click += new System.EventHandler(this.PbtnScanDevices_Click);
            // 
            // PbtnConnect
            // 
            this.PbtnConnect.Location = new System.Drawing.Point(152, 160);
            this.PbtnConnect.Name = "PbtnConnect";
            this.PbtnConnect.Size = new System.Drawing.Size(104, 35);
            this.PbtnConnect.TabIndex = 5;
            this.PbtnConnect.Text = "button1";
            this.PbtnConnect.UseVisualStyleBackColor = true;
            this.PbtnConnect.Click += new System.EventHandler(this.PbtnConnect_Click);
            // 
            // tbBle
            // 
            this.tbBle.AutoSize = true;
            this.tbBle.Location = new System.Drawing.Point(150, 143);
            this.tbBle.Name = "tbBle";
            this.tbBle.Size = new System.Drawing.Size(35, 12);
            this.tbBle.TabIndex = 6;
            this.tbBle.Text = "label1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tbBle);
            this.Controls.Add(this.PbtnConnect);
            this.Controls.Add(this.PbtnScanDevices);
            this.Controls.Add(this.listViewDevices);
            this.Controls.Add(this.PbtnFileOpen);
            this.Controls.Add(this.PbtnStart);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PbtnStart;
        private System.Windows.Forms.Button PbtnFileOpen;
        private System.Windows.Forms.ListView listViewDevices;
        private System.Windows.Forms.Button PbtnScanDevices;
        private System.Windows.Forms.Button PbtnConnect;
        private System.Windows.Forms.Label tbBle;
    }
}

