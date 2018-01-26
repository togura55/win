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
            this.tbUsb = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PbtnStart
            // 
            this.PbtnStart.Location = new System.Drawing.Point(41, 284);
            this.PbtnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PbtnStart.Name = "PbtnStart";
            this.PbtnStart.Size = new System.Drawing.Size(132, 48);
            this.PbtnStart.TabIndex = 0;
            this.PbtnStart.Text = "button1";
            this.PbtnStart.UseVisualStyleBackColor = true;
            this.PbtnStart.Click += new System.EventHandler(this.PbtnStart_Click);
            // 
            // PbtnFileOpen
            // 
            this.PbtnFileOpen.Location = new System.Drawing.Point(203, 284);
            this.PbtnFileOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PbtnFileOpen.Name = "PbtnFileOpen";
            this.PbtnFileOpen.Size = new System.Drawing.Size(139, 48);
            this.PbtnFileOpen.TabIndex = 1;
            this.PbtnFileOpen.Text = "button1";
            this.PbtnFileOpen.UseVisualStyleBackColor = true;
            this.PbtnFileOpen.Click += new System.EventHandler(this.PbtnFileOpen_Click);
            // 
            // listViewDevices
            // 
            this.listViewDevices.Location = new System.Drawing.Point(41, 16);
            this.listViewDevices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewDevices.Name = "listViewDevices";
            this.listViewDevices.Size = new System.Drawing.Size(299, 167);
            this.listViewDevices.TabIndex = 3;
            this.listViewDevices.Tag = "";
            this.listViewDevices.UseCompatibleStateImageBehavior = false;
            this.listViewDevices.SelectedIndexChanged += new System.EventHandler(this.ListViewDevices_SelectedIndexChanged);
            // 
            // PbtnScanDevices
            // 
            this.PbtnScanDevices.Location = new System.Drawing.Point(41, 213);
            this.PbtnScanDevices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PbtnScanDevices.Name = "PbtnScanDevices";
            this.PbtnScanDevices.Size = new System.Drawing.Size(132, 47);
            this.PbtnScanDevices.TabIndex = 4;
            this.PbtnScanDevices.Text = "button1";
            this.PbtnScanDevices.UseVisualStyleBackColor = true;
            this.PbtnScanDevices.Click += new System.EventHandler(this.PbtnScanDevices_Click);
            // 
            // PbtnConnect
            // 
            this.PbtnConnect.Location = new System.Drawing.Point(203, 213);
            this.PbtnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PbtnConnect.Name = "PbtnConnect";
            this.PbtnConnect.Size = new System.Drawing.Size(139, 47);
            this.PbtnConnect.TabIndex = 5;
            this.PbtnConnect.Text = "button1";
            this.PbtnConnect.UseVisualStyleBackColor = true;
            this.PbtnConnect.Click += new System.EventHandler(this.PbtnConnect_Click);
            // 
            // tbBle
            // 
            this.tbBle.AutoSize = true;
            this.tbBle.Location = new System.Drawing.Point(200, 191);
            this.tbBle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tbBle.Name = "tbBle";
            this.tbBle.Size = new System.Drawing.Size(46, 17);
            this.tbBle.TabIndex = 6;
            this.tbBle.Text = "label1";
            // 
            // tbUsb
            // 
            this.tbUsb.AutoSize = true;
            this.tbUsb.Location = new System.Drawing.Point(38, 191);
            this.tbUsb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tbUsb.Name = "tbUsb";
            this.tbUsb.Size = new System.Drawing.Size(46, 17);
            this.tbUsb.TabIndex = 7;
            this.tbUsb.Text = "label1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 348);
            this.Controls.Add(this.tbUsb);
            this.Controls.Add(this.tbBle);
            this.Controls.Add(this.PbtnConnect);
            this.Controls.Add(this.PbtnScanDevices);
            this.Controls.Add(this.listViewDevices);
            this.Controls.Add(this.PbtnFileOpen);
            this.Controls.Add(this.PbtnStart);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
        private System.Windows.Forms.Label tbUsb;
    }
}

