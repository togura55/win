namespace QARepParsForm
{
    partial class MainWindow
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
            this.labelSMTPPassword = new System.Windows.Forms.Label();
            this.buttonParse = new System.Windows.Forms.Button();
            this.textBoxLogFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSMTPServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxCc = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxBcc = new System.Windows.Forms.TextBox();
            this.textBoxSign = new System.Windows.Forms.TextBox();
            this.textBoxContents = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxFromDisplayName = new System.Windows.Forms.TextBox();
            this.checkBoxSMTPSSL = new System.Windows.Forms.CheckBox();
            this.checkBoxSMTPAuth = new System.Windows.Forms.CheckBox();
            this.labelSMTPPort = new System.Windows.Forms.Label();
            this.textBoxSMTPPort = new System.Windows.Forms.TextBox();
            this.textBoxSMTPPassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSendMail = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonOpenLogFile = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSMTPPassword
            // 
            this.labelSMTPPassword.AutoSize = true;
            this.labelSMTPPassword.Location = new System.Drawing.Point(214, 87);
            this.labelSMTPPassword.Name = "labelSMTPPassword";
            this.labelSMTPPassword.Size = new System.Drawing.Size(88, 12);
            this.labelSMTPPassword.TabIndex = 24;
            this.labelSMTPPassword.Text = "SMTP Password";
            // 
            // buttonParse
            // 
            this.buttonParse.Location = new System.Drawing.Point(391, 27);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(75, 23);
            this.buttonParse.TabIndex = 0;
            this.buttonParse.Text = "&Parse";
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.Click += new System.EventHandler(this.buttonParse_Click);
            // 
            // textBoxLogFile
            // 
            this.textBoxLogFile.Location = new System.Drawing.Point(77, 29);
            this.textBoxLogFile.Name = "textBoxLogFile";
            this.textBoxLogFile.Size = new System.Drawing.Size(102, 19);
            this.textBoxLogFile.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Log File";
            // 
            // textBoxSMTPServer
            // 
            this.textBoxSMTPServer.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::QARepParsForm.Properties.Settings.Default, "SMTPServer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxSMTPServer.Location = new System.Drawing.Point(84, 18);
            this.textBoxSMTPServer.Name = "textBoxSMTPServer";
            this.textBoxSMTPServer.Size = new System.Drawing.Size(100, 19);
            this.textBoxSMTPServer.TabIndex = 3;
            this.textBoxSMTPServer.Text = global::QARepParsForm.Properties.Settings.Default.SMTPServer;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "To";
            // 
            // textBoxTo
            // 
            this.textBoxTo.Location = new System.Drawing.Point(86, 146);
            this.textBoxTo.Name = "textBoxTo";
            this.textBoxTo.Size = new System.Drawing.Size(100, 19);
            this.textBoxTo.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Subject";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(86, 173);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(247, 19);
            this.textBoxTitle.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(194, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Cc";
            // 
            // textBoxCc
            // 
            this.textBoxCc.Location = new System.Drawing.Point(219, 148);
            this.textBoxCc.Name = "textBoxCc";
            this.textBoxCc.Size = new System.Drawing.Size(100, 19);
            this.textBoxCc.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(341, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Bcc";
            // 
            // textBoxBcc
            // 
            this.textBoxBcc.AcceptsReturn = true;
            this.textBoxBcc.Location = new System.Drawing.Point(370, 148);
            this.textBoxBcc.Name = "textBoxBcc";
            this.textBoxBcc.Size = new System.Drawing.Size(96, 19);
            this.textBoxBcc.TabIndex = 11;
            // 
            // textBoxSign
            // 
            this.textBoxSign.Location = new System.Drawing.Point(86, 254);
            this.textBoxSign.Name = "textBoxSign";
            this.textBoxSign.Size = new System.Drawing.Size(247, 19);
            this.textBoxSign.TabIndex = 13;
            // 
            // textBoxContents
            // 
            this.textBoxContents.Location = new System.Drawing.Point(86, 198);
            this.textBoxContents.Multiline = true;
            this.textBoxContents.Name = "textBoxContents";
            this.textBoxContents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxContents.Size = new System.Drawing.Size(247, 50);
            this.textBoxContents.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "Body";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBoxFromDisplayName);
            this.groupBox1.Controls.Add(this.checkBoxSMTPSSL);
            this.groupBox1.Controls.Add(this.checkBoxSMTPAuth);
            this.groupBox1.Controls.Add(this.labelSMTPPort);
            this.groupBox1.Controls.Add(this.textBoxSMTPPort);
            this.groupBox1.Controls.Add(this.labelSMTPPassword);
            this.groupBox1.Controls.Add(this.textBoxSMTPPassword);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBoxAccount);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.buttonSendMail);
            this.groupBox1.Controls.Add(this.textBoxContents);
            this.groupBox1.Controls.Add(this.textBoxSign);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxBcc);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxCc);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxTitle);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxTo);
            this.groupBox1.Controls.Add(this.textBoxSMTPServer);
            this.groupBox1.Location = new System.Drawing.Point(11, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(493, 283);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Notifier";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(214, 123);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(106, 12);
            this.label12.TabIndex = 30;
            this.label12.Text = "From Display Name";
            // 
            // textBoxFromDisplayName
            // 
            this.textBoxFromDisplayName.Location = new System.Drawing.Point(326, 120);
            this.textBoxFromDisplayName.Name = "textBoxFromDisplayName";
            this.textBoxFromDisplayName.Size = new System.Drawing.Size(140, 19);
            this.textBoxFromDisplayName.TabIndex = 29;
            // 
            // checkBoxSMTPSSL
            // 
            this.checkBoxSMTPSSL.AutoSize = true;
            this.checkBoxSMTPSSL.Location = new System.Drawing.Point(34, 88);
            this.checkBoxSMTPSSL.Name = "checkBoxSMTPSSL";
            this.checkBoxSMTPSSL.Size = new System.Drawing.Size(104, 16);
            this.checkBoxSMTPSSL.TabIndex = 28;
            this.checkBoxSMTPSSL.Text = "SMTP over SSL";
            this.checkBoxSMTPSSL.UseVisualStyleBackColor = true;
            this.checkBoxSMTPSSL.CheckedChanged += new System.EventHandler(this.checkBoxSMTPSSL_CheckedChanged);
            // 
            // checkBoxSMTPAuth
            // 
            this.checkBoxSMTPAuth.AutoSize = true;
            this.checkBoxSMTPAuth.Location = new System.Drawing.Point(34, 55);
            this.checkBoxSMTPAuth.Name = "checkBoxSMTPAuth";
            this.checkBoxSMTPAuth.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSMTPAuth.TabIndex = 27;
            this.checkBoxSMTPAuth.Text = "SMTP Auth.";
            this.checkBoxSMTPAuth.UseVisualStyleBackColor = true;
            this.checkBoxSMTPAuth.CheckedChanged += new System.EventHandler(this.checkBoxSMTPAuth_CheckedChanged);
            // 
            // labelSMTPPort
            // 
            this.labelSMTPPort.AutoSize = true;
            this.labelSMTPPort.Location = new System.Drawing.Point(214, 58);
            this.labelSMTPPort.Name = "labelSMTPPort";
            this.labelSMTPPort.Size = new System.Drawing.Size(60, 12);
            this.labelSMTPPort.TabIndex = 26;
            this.labelSMTPPort.Text = "SMTP Port";
            // 
            // textBoxSMTPPort
            // 
            this.textBoxSMTPPort.Location = new System.Drawing.Point(308, 55);
            this.textBoxSMTPPort.Name = "textBoxSMTPPort";
            this.textBoxSMTPPort.Size = new System.Drawing.Size(100, 19);
            this.textBoxSMTPPort.TabIndex = 25;
            // 
            // textBoxSMTPPassword
            // 
            this.textBoxSMTPPassword.Location = new System.Drawing.Point(308, 84);
            this.textBoxSMTPPassword.Name = "textBoxSMTPPassword";
            this.textBoxSMTPPassword.Size = new System.Drawing.Size(100, 19);
            this.textBoxSMTPPassword.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 127);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "From";
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Location = new System.Drawing.Point(86, 121);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(100, 19);
            this.textBoxAccount.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 256);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "Signature";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "SMTP Server";
            // 
            // buttonSendMail
            // 
            this.buttonSendMail.Location = new System.Drawing.Point(391, 254);
            this.buttonSendMail.Name = "buttonSendMail";
            this.buttonSendMail.Size = new System.Drawing.Size(75, 23);
            this.buttonSendMail.TabIndex = 18;
            this.buttonSendMail.Text = "&Send Mail";
            this.buttonSendMail.UseVisualStyleBackColor = true;
            this.buttonSendMail.Click += new System.EventHandler(this.buttonSendMail_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonOpenLogFile);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxLogFile);
            this.groupBox2.Controls.Add(this.buttonParse);
            this.groupBox2.Location = new System.Drawing.Point(11, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(492, 60);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log Parser";
            // 
            // buttonOpenLogFile
            // 
            this.buttonOpenLogFile.Location = new System.Drawing.Point(188, 26);
            this.buttonOpenLogFile.Name = "buttonOpenLogFile";
            this.buttonOpenLogFile.Size = new System.Drawing.Size(28, 23);
            this.buttonOpenLogFile.TabIndex = 3;
            this.buttonOpenLogFile.Text = "...";
            this.buttonOpenLogFile.UseVisualStyleBackColor = true;
            this.buttonOpenLogFile.Click += new System.EventHandler(this.buttonOpenLogFile_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 379);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainWindow";
            this.Text = "QARepParsForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.TextBox textBoxLogFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSMTPServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxCc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxBcc;
        private System.Windows.Forms.TextBox textBoxSign;
        private System.Windows.Forms.TextBox textBoxContents;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonSendMail;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelSMTPPort;
        private System.Windows.Forms.TextBox textBoxSMTPPort;
        private System.Windows.Forms.TextBox textBoxSMTPPassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxAccount;
        private System.Windows.Forms.Button buttonOpenLogFile;
        private System.Windows.Forms.CheckBox checkBoxSMTPSSL;
        private System.Windows.Forms.CheckBox checkBoxSMTPAuth;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxFromDisplayName;
        private System.Windows.Forms.Label labelSMTPPassword;
    }
}

