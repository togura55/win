namespace ScoreScraping
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
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.labelUrl = new System.Windows.Forms.Label();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.pbtnStart = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.labelHtml = new System.Windows.Forms.Label();
            this.textBoxHtml = new System.Windows.Forms.TextBox();
            this.labelView = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.pbtnShowResult = new System.Windows.Forms.Button();
            this.pbtnEditList = new System.Windows.Forms.Button();
            this.comboBoxWebsites = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(34, 15);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(35, 12);
            this.labelUrl.TabIndex = 0;
            this.labelUrl.Text = "label1";
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Location = new System.Drawing.Point(32, 30);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(584, 19);
            this.textBoxUrl.TabIndex = 1;
            // 
            // pbtnStart
            // 
            this.pbtnStart.Location = new System.Drawing.Point(505, 119);
            this.pbtnStart.Name = "pbtnStart";
            this.pbtnStart.Size = new System.Drawing.Size(107, 26);
            this.pbtnStart.TabIndex = 2;
            this.pbtnStart.Text = "button1";
            this.pbtnStart.UseVisualStyleBackColor = true;
            this.pbtnStart.Click += new System.EventHandler(this.pbtnStart_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(33, 211);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(35, 12);
            this.labelTitle.TabIndex = 3;
            this.labelTitle.Text = "label2";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(36, 235);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(580, 19);
            this.textBoxTitle.TabIndex = 4;
            // 
            // labelHtml
            // 
            this.labelHtml.AutoSize = true;
            this.labelHtml.Location = new System.Drawing.Point(35, 268);
            this.labelHtml.Name = "labelHtml";
            this.labelHtml.Size = new System.Drawing.Size(35, 12);
            this.labelHtml.TabIndex = 6;
            this.labelHtml.Text = "label3";
            // 
            // textBoxHtml
            // 
            this.textBoxHtml.Location = new System.Drawing.Point(36, 293);
            this.textBoxHtml.Multiline = true;
            this.textBoxHtml.Name = "textBoxHtml";
            this.textBoxHtml.Size = new System.Drawing.Size(578, 123);
            this.textBoxHtml.TabIndex = 7;
            // 
            // labelView
            // 
            this.labelView.AutoSize = true;
            this.labelView.Location = new System.Drawing.Point(33, 168);
            this.labelView.Name = "labelView";
            this.labelView.Size = new System.Drawing.Size(35, 12);
            this.labelView.TabIndex = 8;
            this.labelView.Text = "label4";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Location = new System.Drawing.Point(31, 102);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(35, 12);
            this.labelID.TabIndex = 9;
            this.labelID.Text = "label1";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(34, 127);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(184, 19);
            this.textBoxID.TabIndex = 10;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(253, 127);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '●';
            this.textBoxPassword.Size = new System.Drawing.Size(193, 19);
            this.textBoxPassword.TabIndex = 12;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(250, 102);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(35, 12);
            this.labelPassword.TabIndex = 11;
            this.labelPassword.Text = "label2";
            // 
            // pbtnShowResult
            // 
            this.pbtnShowResult.Location = new System.Drawing.Point(504, 168);
            this.pbtnShowResult.Name = "pbtnShowResult";
            this.pbtnShowResult.Size = new System.Drawing.Size(109, 28);
            this.pbtnShowResult.TabIndex = 13;
            this.pbtnShowResult.Text = "button1";
            this.pbtnShowResult.UseVisualStyleBackColor = true;
            this.pbtnShowResult.Click += new System.EventHandler(this.pbtnShowResult_Click);
            // 
            // pbtnEditList
            // 
            this.pbtnEditList.Location = new System.Drawing.Point(508, 56);
            this.pbtnEditList.Name = "pbtnEditList";
            this.pbtnEditList.Size = new System.Drawing.Size(105, 28);
            this.pbtnEditList.TabIndex = 14;
            this.pbtnEditList.Text = "button2";
            this.pbtnEditList.UseVisualStyleBackColor = true;
            this.pbtnEditList.Click += new System.EventHandler(this.pbtnEditList_Click);
            // 
            // comboBoxWebsites
            // 
            this.comboBoxWebsites.FormattingEnabled = true;
            this.comboBoxWebsites.Location = new System.Drawing.Point(32, 64);
            this.comboBoxWebsites.Name = "comboBoxWebsites";
            this.comboBoxWebsites.Size = new System.Drawing.Size(134, 20);
            this.comboBoxWebsites.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBoxWebsites);
            this.Controls.Add(this.pbtnEditList);
            this.Controls.Add(this.pbtnShowResult);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.labelID);
            this.Controls.Add(this.labelView);
            this.Controls.Add(this.textBoxHtml);
            this.Controls.Add(this.labelHtml);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pbtnStart);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.labelUrl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.Button pbtnStart;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label labelHtml;
        private System.Windows.Forms.TextBox textBoxHtml;
        private System.Windows.Forms.Label labelView;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Button pbtnShowResult;
        private System.Windows.Forms.Button pbtnEditList;
        private System.Windows.Forms.ComboBox comboBoxWebsites;
    }
}

