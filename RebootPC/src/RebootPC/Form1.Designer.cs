namespace RebootPC
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
            this.Rbtn_Reboot = new System.Windows.Forms.RadioButton();
            this.Rbtn_Shutdown = new System.Windows.Forms.RadioButton();
            this.TextBox_Timeout = new System.Windows.Forms.TextBox();
            this.Label_Timeout = new System.Windows.Forms.Label();
            this.Label_Messages = new System.Windows.Forms.Label();
            this.Label_MaxCount = new System.Windows.Forms.Label();
            this.TextBox_MaxCount = new System.Windows.Forms.TextBox();
            this.Pbtn_Start = new System.Windows.Forms.Button();
            this.Pbtn_Close = new System.Windows.Forms.Button();
            this.GroupBox_Mode = new System.Windows.Forms.GroupBox();
            this.Label_Counter = new System.Windows.Forms.Label();
            this.Label_CounterValue = new System.Windows.Forms.Label();
            this.Pbtn_Reset = new System.Windows.Forms.Button();
            this.Label_ExtApp = new System.Windows.Forms.Label();
            this.TextBox_FilePath = new System.Windows.Forms.TextBox();
            this.Pbtn_FilePath = new System.Windows.Forms.Button();
            this.TextBox_Delay = new System.Windows.Forms.TextBox();
            this.Label_Delay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Rbtn_Reboot
            // 
            resources.ApplyResources(this.Rbtn_Reboot, "Rbtn_Reboot");
            this.Rbtn_Reboot.Name = "Rbtn_Reboot";
            this.Rbtn_Reboot.TabStop = true;
            this.Rbtn_Reboot.UseVisualStyleBackColor = true;
            // 
            // Rbtn_Shutdown
            // 
            resources.ApplyResources(this.Rbtn_Shutdown, "Rbtn_Shutdown");
            this.Rbtn_Shutdown.Name = "Rbtn_Shutdown";
            this.Rbtn_Shutdown.TabStop = true;
            this.Rbtn_Shutdown.UseVisualStyleBackColor = true;
            // 
            // TextBox_Timeout
            // 
            resources.ApplyResources(this.TextBox_Timeout, "TextBox_Timeout");
            this.TextBox_Timeout.Name = "TextBox_Timeout";
            // 
            // Label_Timeout
            // 
            resources.ApplyResources(this.Label_Timeout, "Label_Timeout");
            this.Label_Timeout.Name = "Label_Timeout";
            // 
            // Label_Messages
            // 
            resources.ApplyResources(this.Label_Messages, "Label_Messages");
            this.Label_Messages.Name = "Label_Messages";
            // 
            // Label_MaxCount
            // 
            resources.ApplyResources(this.Label_MaxCount, "Label_MaxCount");
            this.Label_MaxCount.Name = "Label_MaxCount";
            // 
            // TextBox_MaxCount
            // 
            resources.ApplyResources(this.TextBox_MaxCount, "TextBox_MaxCount");
            this.TextBox_MaxCount.Name = "TextBox_MaxCount";
            // 
            // Pbtn_Start
            // 
            resources.ApplyResources(this.Pbtn_Start, "Pbtn_Start");
            this.Pbtn_Start.Name = "Pbtn_Start";
            this.Pbtn_Start.UseVisualStyleBackColor = true;
            this.Pbtn_Start.Click += new System.EventHandler(this.Pbtn_Start_Click);
            // 
            // Pbtn_Close
            // 
            resources.ApplyResources(this.Pbtn_Close, "Pbtn_Close");
            this.Pbtn_Close.Name = "Pbtn_Close";
            this.Pbtn_Close.UseVisualStyleBackColor = true;
            this.Pbtn_Close.Click += new System.EventHandler(this.Pbtn_Close_Click);
            // 
            // GroupBox_Mode
            // 
            resources.ApplyResources(this.GroupBox_Mode, "GroupBox_Mode");
            this.GroupBox_Mode.Name = "GroupBox_Mode";
            this.GroupBox_Mode.TabStop = false;
            // 
            // Label_Counter
            // 
            resources.ApplyResources(this.Label_Counter, "Label_Counter");
            this.Label_Counter.Name = "Label_Counter";
            // 
            // Label_CounterValue
            // 
            resources.ApplyResources(this.Label_CounterValue, "Label_CounterValue");
            this.Label_CounterValue.Name = "Label_CounterValue";
            // 
            // Pbtn_Reset
            // 
            resources.ApplyResources(this.Pbtn_Reset, "Pbtn_Reset");
            this.Pbtn_Reset.Name = "Pbtn_Reset";
            this.Pbtn_Reset.UseVisualStyleBackColor = true;
            this.Pbtn_Reset.Click += new System.EventHandler(this.Pbtn_Reset_Click);
            // 
            // Label_ExtApp
            // 
            resources.ApplyResources(this.Label_ExtApp, "Label_ExtApp");
            this.Label_ExtApp.Name = "Label_ExtApp";
            // 
            // TextBox_FilePath
            // 
            resources.ApplyResources(this.TextBox_FilePath, "TextBox_FilePath");
            this.TextBox_FilePath.Name = "TextBox_FilePath";
            // 
            // Pbtn_FilePath
            // 
            resources.ApplyResources(this.Pbtn_FilePath, "Pbtn_FilePath");
            this.Pbtn_FilePath.Name = "Pbtn_FilePath";
            this.Pbtn_FilePath.UseVisualStyleBackColor = true;
            this.Pbtn_FilePath.Click += new System.EventHandler(this.Pbtn_FilePath_Click);
            // 
            // TextBox_Delay
            // 
            resources.ApplyResources(this.TextBox_Delay, "TextBox_Delay");
            this.TextBox_Delay.Name = "TextBox_Delay";
            // 
            // Label_Delay
            // 
            resources.ApplyResources(this.Label_Delay, "Label_Delay");
            this.Label_Delay.Name = "Label_Delay";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Label_Delay);
            this.Controls.Add(this.TextBox_Delay);
            this.Controls.Add(this.Pbtn_FilePath);
            this.Controls.Add(this.TextBox_FilePath);
            this.Controls.Add(this.Label_ExtApp);
            this.Controls.Add(this.Pbtn_Reset);
            this.Controls.Add(this.Label_CounterValue);
            this.Controls.Add(this.Label_Counter);
            this.Controls.Add(this.Pbtn_Close);
            this.Controls.Add(this.Pbtn_Start);
            this.Controls.Add(this.Label_MaxCount);
            this.Controls.Add(this.TextBox_MaxCount);
            this.Controls.Add(this.Label_Messages);
            this.Controls.Add(this.Label_Timeout);
            this.Controls.Add(this.TextBox_Timeout);
            this.Controls.Add(this.Rbtn_Shutdown);
            this.Controls.Add(this.Rbtn_Reboot);
            this.Controls.Add(this.GroupBox_Mode);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton Rbtn_Reboot;
        private System.Windows.Forms.RadioButton Rbtn_Shutdown;
        private System.Windows.Forms.TextBox TextBox_Timeout;
        private System.Windows.Forms.Label Label_Timeout;
        private System.Windows.Forms.Label Label_MaxCount;
        private System.Windows.Forms.TextBox TextBox_MaxCount;
        private System.Windows.Forms.Button Pbtn_Start;
        private System.Windows.Forms.Button Pbtn_Close;
        private System.Windows.Forms.GroupBox GroupBox_Mode;
        private System.Windows.Forms.Label Label_Counter;
        private System.Windows.Forms.Label Label_CounterValue;
        private System.Windows.Forms.Button Pbtn_Reset;
        private System.Windows.Forms.Label Label_ExtApp;
        private System.Windows.Forms.TextBox TextBox_FilePath;
        private System.Windows.Forms.Button Pbtn_FilePath;
        private System.Windows.Forms.TextBox TextBox_Delay;
        private System.Windows.Forms.Label Label_Delay;
        private System.Windows.Forms.Label Label_Messages;
    }
}

