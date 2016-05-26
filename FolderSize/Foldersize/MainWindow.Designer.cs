namespace Foldersize
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
            this.textBoxFolderPath = new System.Windows.Forms.TextBox();
            this.buttonFolderPath = new System.Windows.Forms.Button();
            this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
            this.radioButtonSum = new System.Windows.Forms.RadioButton();
            this.radioButtonLower = new System.Windows.Forms.RadioButton();
            this.checkBoxNumFiles = new System.Windows.Forms.CheckBox();
            this.checkBoxSize = new System.Windows.Forms.CheckBox();
            this.radioButtonSize = new System.Windows.Forms.RadioButton();
            this.radioButtonFileName = new System.Windows.Forms.RadioButton();
            this.radioButtonNumFiles = new System.Windows.Forms.RadioButton();
            this.radioButtonDescend = new System.Windows.Forms.RadioButton();
            this.radioButtonAscend = new System.Windows.Forms.RadioButton();
            this.groupBoxLayer = new System.Windows.Forms.GroupBox();
            this.groupBoxDisplay = new System.Windows.Forms.GroupBox();
            this.labelFolderPath = new System.Windows.Forms.Label();
            this.groupBoxSortType = new System.Windows.Forms.GroupBox();
            this.groupBoxSortOrder = new System.Windows.Forms.GroupBox();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.groupBoxLayer.SuspendLayout();
            this.groupBoxDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxFolderPath
            // 
            this.textBoxFolderPath.Location = new System.Drawing.Point(13, 29);
            this.textBoxFolderPath.Name = "textBoxFolderPath";
            this.textBoxFolderPath.Size = new System.Drawing.Size(206, 19);
            this.textBoxFolderPath.TabIndex = 0;
            // 
            // buttonFolderPath
            // 
            this.buttonFolderPath.Location = new System.Drawing.Point(239, 22);
            this.buttonFolderPath.Name = "buttonFolderPath";
            this.buttonFolderPath.Size = new System.Drawing.Size(56, 30);
            this.buttonFolderPath.TabIndex = 1;
            this.buttonFolderPath.Text = "button1";
            this.buttonFolderPath.UseVisualStyleBackColor = true;
            this.buttonFolderPath.Click += new System.EventHandler(this.buttonFolderPath_Click);
            // 
            // richTextBoxResult
            // 
            this.richTextBoxResult.Location = new System.Drawing.Point(323, 29);
            this.richTextBoxResult.Name = "richTextBoxResult";
            this.richTextBoxResult.Size = new System.Drawing.Size(248, 234);
            this.richTextBoxResult.TabIndex = 2;
            this.richTextBoxResult.Text = "";
            // 
            // radioButtonSum
            // 
            this.radioButtonSum.AutoSize = true;
            this.radioButtonSum.Location = new System.Drawing.Point(14, 22);
            this.radioButtonSum.Name = "radioButtonSum";
            this.radioButtonSum.Size = new System.Drawing.Size(88, 16);
            this.radioButtonSum.TabIndex = 3;
            this.radioButtonSum.TabStop = true;
            this.radioButtonSum.Text = "radioButton1";
            this.radioButtonSum.UseVisualStyleBackColor = true;
            this.radioButtonSum.CheckedChanged += new System.EventHandler(this.radioButtonSum_CheckedChanged);
            // 
            // radioButtonLower
            // 
            this.radioButtonLower.AutoSize = true;
            this.radioButtonLower.Location = new System.Drawing.Point(14, 44);
            this.radioButtonLower.Name = "radioButtonLower";
            this.radioButtonLower.Size = new System.Drawing.Size(88, 16);
            this.radioButtonLower.TabIndex = 4;
            this.radioButtonLower.TabStop = true;
            this.radioButtonLower.Text = "radioButton2";
            this.radioButtonLower.UseVisualStyleBackColor = true;
            this.radioButtonLower.CheckedChanged += new System.EventHandler(this.radioButtonSum_CheckedChanged);
            // 
            // checkBoxNumFiles
            // 
            this.checkBoxNumFiles.AutoSize = true;
            this.checkBoxNumFiles.Location = new System.Drawing.Point(8, 23);
            this.checkBoxNumFiles.Name = "checkBoxNumFiles";
            this.checkBoxNumFiles.Size = new System.Drawing.Size(80, 16);
            this.checkBoxNumFiles.TabIndex = 5;
            this.checkBoxNumFiles.Text = "checkBox1";
            this.checkBoxNumFiles.UseVisualStyleBackColor = true;
            this.checkBoxNumFiles.CheckedChanged += new System.EventHandler(this.checkBoxNumFiles_CheckedChanged);
            // 
            // checkBoxSize
            // 
            this.checkBoxSize.AutoSize = true;
            this.checkBoxSize.Location = new System.Drawing.Point(8, 45);
            this.checkBoxSize.Name = "checkBoxSize";
            this.checkBoxSize.Size = new System.Drawing.Size(80, 16);
            this.checkBoxSize.TabIndex = 6;
            this.checkBoxSize.Text = "checkBox2";
            this.checkBoxSize.UseVisualStyleBackColor = true;
            this.checkBoxSize.CheckedChanged += new System.EventHandler(this.checkBoxSize_CheckedChanged);
            // 
            // radioButtonSize
            // 
            this.radioButtonSize.AutoSize = true;
            this.radioButtonSize.Location = new System.Drawing.Point(27, 161);
            this.radioButtonSize.Name = "radioButtonSize";
            this.radioButtonSize.Size = new System.Drawing.Size(88, 16);
            this.radioButtonSize.TabIndex = 7;
            this.radioButtonSize.TabStop = true;
            this.radioButtonSize.Text = "radioButton3";
            this.radioButtonSize.UseVisualStyleBackColor = true;
            this.radioButtonSize.CheckedChanged += new System.EventHandler(this.radioButtonSize_CheckedChanged);
            // 
            // radioButtonFileName
            // 
            this.radioButtonFileName.AutoSize = true;
            this.radioButtonFileName.Location = new System.Drawing.Point(27, 183);
            this.radioButtonFileName.Name = "radioButtonFileName";
            this.radioButtonFileName.Size = new System.Drawing.Size(88, 16);
            this.radioButtonFileName.TabIndex = 8;
            this.radioButtonFileName.TabStop = true;
            this.radioButtonFileName.Text = "radioButton4";
            this.radioButtonFileName.UseVisualStyleBackColor = true;
            this.radioButtonFileName.CheckedChanged += new System.EventHandler(this.radioButtonSize_CheckedChanged);
            // 
            // radioButtonNumFiles
            // 
            this.radioButtonNumFiles.AutoSize = true;
            this.radioButtonNumFiles.Location = new System.Drawing.Point(27, 204);
            this.radioButtonNumFiles.Name = "radioButtonNumFiles";
            this.radioButtonNumFiles.Size = new System.Drawing.Size(88, 16);
            this.radioButtonNumFiles.TabIndex = 9;
            this.radioButtonNumFiles.TabStop = true;
            this.radioButtonNumFiles.Text = "radioButton5";
            this.radioButtonNumFiles.UseVisualStyleBackColor = true;
            this.radioButtonNumFiles.CheckedChanged += new System.EventHandler(this.radioButtonSize_CheckedChanged);
            // 
            // radioButtonDescend
            // 
            this.radioButtonDescend.AutoSize = true;
            this.radioButtonDescend.Location = new System.Drawing.Point(174, 160);
            this.radioButtonDescend.Name = "radioButtonDescend";
            this.radioButtonDescend.Size = new System.Drawing.Size(88, 16);
            this.radioButtonDescend.TabIndex = 10;
            this.radioButtonDescend.TabStop = true;
            this.radioButtonDescend.Text = "radioButton6";
            this.radioButtonDescend.UseVisualStyleBackColor = true;
            this.radioButtonDescend.CheckedChanged += new System.EventHandler(this.radioButtonDescend_CheckedChanged);
            // 
            // radioButtonAscend
            // 
            this.radioButtonAscend.AutoSize = true;
            this.radioButtonAscend.Location = new System.Drawing.Point(174, 182);
            this.radioButtonAscend.Name = "radioButtonAscend";
            this.radioButtonAscend.Size = new System.Drawing.Size(88, 16);
            this.radioButtonAscend.TabIndex = 11;
            this.radioButtonAscend.TabStop = true;
            this.radioButtonAscend.Text = "radioButton7";
            this.radioButtonAscend.UseVisualStyleBackColor = true;
            this.radioButtonAscend.CheckedChanged += new System.EventHandler(this.radioButtonDescend_CheckedChanged);
            // 
            // groupBoxLayer
            // 
            this.groupBoxLayer.Controls.Add(this.radioButtonLower);
            this.groupBoxLayer.Controls.Add(this.radioButtonSum);
            this.groupBoxLayer.Location = new System.Drawing.Point(13, 64);
            this.groupBoxLayer.Name = "groupBoxLayer";
            this.groupBoxLayer.Size = new System.Drawing.Size(129, 69);
            this.groupBoxLayer.TabIndex = 12;
            this.groupBoxLayer.TabStop = false;
            this.groupBoxLayer.Text = "groupBox1";
            // 
            // groupBoxDisplay
            // 
            this.groupBoxDisplay.Controls.Add(this.checkBoxSize);
            this.groupBoxDisplay.Controls.Add(this.checkBoxNumFiles);
            this.groupBoxDisplay.Location = new System.Drawing.Point(166, 65);
            this.groupBoxDisplay.Name = "groupBoxDisplay";
            this.groupBoxDisplay.Size = new System.Drawing.Size(129, 69);
            this.groupBoxDisplay.TabIndex = 13;
            this.groupBoxDisplay.TabStop = false;
            this.groupBoxDisplay.Text = "groupBox2";
            // 
            // labelFolderPath
            // 
            this.labelFolderPath.AutoSize = true;
            this.labelFolderPath.Location = new System.Drawing.Point(11, 9);
            this.labelFolderPath.Name = "labelFolderPath";
            this.labelFolderPath.Size = new System.Drawing.Size(35, 12);
            this.labelFolderPath.TabIndex = 14;
            this.labelFolderPath.Text = "label1";
            // 
            // groupBoxSortType
            // 
            this.groupBoxSortType.Location = new System.Drawing.Point(13, 139);
            this.groupBoxSortType.Name = "groupBoxSortType";
            this.groupBoxSortType.Size = new System.Drawing.Size(129, 94);
            this.groupBoxSortType.TabIndex = 15;
            this.groupBoxSortType.TabStop = false;
            this.groupBoxSortType.Text = "groupBox3";
            // 
            // groupBoxSortOrder
            // 
            this.groupBoxSortOrder.Location = new System.Drawing.Point(166, 140);
            this.groupBoxSortOrder.Name = "groupBoxSortOrder";
            this.groupBoxSortOrder.Size = new System.Drawing.Size(129, 72);
            this.groupBoxSortOrder.TabIndex = 16;
            this.groupBoxSortOrder.TabStop = false;
            this.groupBoxSortOrder.Text = "groupBox4";
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(199, 233);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(96, 30);
            this.buttonExecute.TabIndex = 17;
            this.buttonExecute.Text = "button2";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(371, 283);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(91, 27);
            this.buttonCopy.TabIndex = 18;
            this.buttonCopy.Text = "button3";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonQuit
            // 
            this.buttonQuit.Location = new System.Drawing.Point(480, 283);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(91, 27);
            this.buttonQuit.TabIndex = 19;
            this.buttonQuit.Text = "button4";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 322);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonExecute);
            this.Controls.Add(this.labelFolderPath);
            this.Controls.Add(this.radioButtonAscend);
            this.Controls.Add(this.radioButtonDescend);
            this.Controls.Add(this.radioButtonNumFiles);
            this.Controls.Add(this.radioButtonFileName);
            this.Controls.Add(this.radioButtonSize);
            this.Controls.Add(this.richTextBoxResult);
            this.Controls.Add(this.buttonFolderPath);
            this.Controls.Add(this.textBoxFolderPath);
            this.Controls.Add(this.groupBoxLayer);
            this.Controls.Add(this.groupBoxDisplay);
            this.Controls.Add(this.groupBoxSortType);
            this.Controls.Add(this.groupBoxSortOrder);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.groupBoxLayer.ResumeLayout(false);
            this.groupBoxLayer.PerformLayout();
            this.groupBoxDisplay.ResumeLayout(false);
            this.groupBoxDisplay.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFolderPath;
        private System.Windows.Forms.Button buttonFolderPath;
        private System.Windows.Forms.RichTextBox richTextBoxResult;
        private System.Windows.Forms.RadioButton radioButtonSum;
        private System.Windows.Forms.RadioButton radioButtonLower;
        private System.Windows.Forms.CheckBox checkBoxNumFiles;
        private System.Windows.Forms.CheckBox checkBoxSize;
        private System.Windows.Forms.RadioButton radioButtonSize;
        private System.Windows.Forms.RadioButton radioButtonFileName;
        private System.Windows.Forms.RadioButton radioButtonNumFiles;
        private System.Windows.Forms.RadioButton radioButtonDescend;
        private System.Windows.Forms.RadioButton radioButtonAscend;
        private System.Windows.Forms.GroupBox groupBoxLayer;
        private System.Windows.Forms.GroupBox groupBoxDisplay;
        private System.Windows.Forms.Label labelFolderPath;
        private System.Windows.Forms.GroupBox groupBoxSortType;
        private System.Windows.Forms.GroupBox groupBoxSortOrder;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonQuit;
    }
}

