namespace ChangeFieldValue
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
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
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
            this.btnSetting = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbListPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboFiled = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioBtnAdd = new System.Windows.Forms.RadioButton();
            this.radioBtnRemove = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.labelServerStatus = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSetting
            // 
            this.btnSetting.Enabled = false;
            this.btnSetting.Location = new System.Drawing.Point(231, 12);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(107, 31);
            this.btnSetting.TabIndex = 9;
            this.btnSetting.Text = "Setting...";
            this.btnSetting.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path to List.txt:";
            // 
            // tbListPath
            // 
            this.tbListPath.Location = new System.Drawing.Point(129, 61);
            this.tbListPath.Name = "tbListPath";
            this.tbListPath.Size = new System.Drawing.Size(380, 27);
            this.tbListPath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Target Field:";
            // 
            // comboFiled
            // 
            this.comboFiled.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFiled.FormattingEnabled = true;
            this.comboFiled.Location = new System.Drawing.Point(129, 94);
            this.comboFiled.Name = "comboFiled";
            this.comboFiled.Size = new System.Drawing.Size(193, 28);
            this.comboFiled.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(73, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Value:";
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(129, 128);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(193, 27);
            this.tbValue.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioBtnRemove);
            this.groupBox1.Controls.Add(this.radioBtnAdd);
            this.groupBox1.Location = new System.Drawing.Point(19, 161);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 73);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operation";
            // 
            // radioBtnAdd
            // 
            this.radioBtnAdd.AutoSize = true;
            this.radioBtnAdd.Checked = true;
            this.radioBtnAdd.Location = new System.Drawing.Point(20, 26);
            this.radioBtnAdd.Name = "radioBtnAdd";
            this.radioBtnAdd.Size = new System.Drawing.Size(52, 24);
            this.radioBtnAdd.TabIndex = 0;
            this.radioBtnAdd.TabStop = true;
            this.radioBtnAdd.Text = "Add";
            this.radioBtnAdd.UseVisualStyleBackColor = true;
            // 
            // radioBtnRemove
            // 
            this.radioBtnRemove.AutoSize = true;
            this.radioBtnRemove.Location = new System.Drawing.Point(78, 26);
            this.radioBtnRemove.Name = "radioBtnRemove";
            this.radioBtnRemove.Size = new System.Drawing.Size(79, 24);
            this.radioBtnRemove.TabIndex = 1;
            this.radioBtnRemove.TabStop = true;
            this.radioBtnRemove.Text = "Remove";
            this.radioBtnRemove.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(305, 203);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(107, 31);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(419, 203);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(107, 31);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // labelServerStatus
            // 
            this.labelServerStatus.AutoSize = true;
            this.labelServerStatus.Location = new System.Drawing.Point(12, 17);
            this.labelServerStatus.Name = "labelServerStatus";
            this.labelServerStatus.Size = new System.Drawing.Size(130, 20);
            this.labelServerStatus.TabIndex = 10;
            this.labelServerStatus.Text = "Connection Status.";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(38, 257);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(471, 150);
            this.dataGridView1.TabIndex = 11;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnExit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 243);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.labelServerStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboFiled);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbListPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSetting);
            this.Font = new System.Drawing.Font("メイリオ", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Redmine Change Field Value";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbListPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboFiled;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioBtnRemove;
        private System.Windows.Forms.RadioButton radioBtnAdd;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label labelServerStatus;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}

