namespace USBTempMon
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.DetectSensor = new System.Windows.Forms.Button();
            this.DetectSensorText = new System.Windows.Forms.Label();
            this.SetLED1 = new System.Windows.Forms.Button();
            this.SetLED2 = new System.Windows.Forms.Button();
            this.SetHeater = new System.Windows.Forms.Button();
            this.HeaterDescText = new System.Windows.Forms.Label();
            this.TempText = new System.Windows.Forms.Label();
            this.HumidText = new System.Windows.Forms.Label();
            this.HeaterStatText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DetectSensor
            // 
            this.DetectSensor.AccessibleDescription = null;
            this.DetectSensor.AccessibleName = null;
            resources.ApplyResources(this.DetectSensor, "DetectSensor");
            this.DetectSensor.BackgroundImage = null;
            this.DetectSensor.Font = null;
            this.DetectSensor.Name = "DetectSensor";
            this.DetectSensor.UseVisualStyleBackColor = true;
            this.DetectSensor.Click += new System.EventHandler(this.DetectSensor_Click);
            // 
            // DetectSensorText
            // 
            this.DetectSensorText.AccessibleDescription = null;
            this.DetectSensorText.AccessibleName = null;
            resources.ApplyResources(this.DetectSensorText, "DetectSensorText");
            this.DetectSensorText.Font = null;
            this.DetectSensorText.Name = "DetectSensorText";
            // 
            // SetLED1
            // 
            this.SetLED1.AccessibleDescription = null;
            this.SetLED1.AccessibleName = null;
            resources.ApplyResources(this.SetLED1, "SetLED1");
            this.SetLED1.BackgroundImage = null;
            this.SetLED1.Font = null;
            this.SetLED1.Name = "SetLED1";
            this.SetLED1.UseVisualStyleBackColor = true;
            this.SetLED1.Click += new System.EventHandler(this.SetLED1_Click);
            // 
            // SetLED2
            // 
            this.SetLED2.AccessibleDescription = null;
            this.SetLED2.AccessibleName = null;
            resources.ApplyResources(this.SetLED2, "SetLED2");
            this.SetLED2.BackgroundImage = null;
            this.SetLED2.Font = null;
            this.SetLED2.Name = "SetLED2";
            this.SetLED2.UseVisualStyleBackColor = true;
            this.SetLED2.Click += new System.EventHandler(this.SetLED2_Click);
            // 
            // SetHeater
            // 
            this.SetHeater.AccessibleDescription = null;
            this.SetHeater.AccessibleName = null;
            resources.ApplyResources(this.SetHeater, "SetHeater");
            this.SetHeater.BackgroundImage = null;
            this.SetHeater.Font = null;
            this.SetHeater.Name = "SetHeater";
            this.SetHeater.UseVisualStyleBackColor = true;
            this.SetHeater.Click += new System.EventHandler(this.SetHeater_Click);
            // 
            // HeaterDescText
            // 
            this.HeaterDescText.AccessibleDescription = null;
            this.HeaterDescText.AccessibleName = null;
            resources.ApplyResources(this.HeaterDescText, "HeaterDescText");
            this.HeaterDescText.Font = null;
            this.HeaterDescText.Name = "HeaterDescText";
            // 
            // TempText
            // 
            this.TempText.AccessibleDescription = null;
            this.TempText.AccessibleName = null;
            resources.ApplyResources(this.TempText, "TempText");
            this.TempText.Font = null;
            this.TempText.Name = "TempText";
            // 
            // HumidText
            // 
            this.HumidText.AccessibleDescription = null;
            this.HumidText.AccessibleName = null;
            resources.ApplyResources(this.HumidText, "HumidText");
            this.HumidText.Font = null;
            this.HumidText.Name = "HumidText";
            // 
            // HeaterStatText
            // 
            this.HeaterStatText.AccessibleDescription = null;
            this.HeaterStatText.AccessibleName = null;
            resources.ApplyResources(this.HeaterStatText, "HeaterStatText");
            this.HeaterStatText.Font = null;
            this.HeaterStatText.Name = "HeaterStatText";
            // 
            // MainWindow
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.HeaterStatText);
            this.Controls.Add(this.HumidText);
            this.Controls.Add(this.TempText);
            this.Controls.Add(this.HeaterDescText);
            this.Controls.Add(this.SetHeater);
            this.Controls.Add(this.SetLED2);
            this.Controls.Add(this.SetLED1);
            this.Controls.Add(this.DetectSensorText);
            this.Controls.Add(this.DetectSensor);
            this.Font = null;
            this.Icon = null;
            this.Name = "MainWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button DetectSensor;
        private System.Windows.Forms.Label DetectSensorText;
        private System.Windows.Forms.Button SetLED1;
        private System.Windows.Forms.Button SetLED2;
        private System.Windows.Forms.Button SetHeater;
        private System.Windows.Forms.Label HeaterDescText;
        private System.Windows.Forms.Label TempText;
        private System.Windows.Forms.Label HumidText;
        private System.Windows.Forms.Label HeaterStatText;
    }
}

