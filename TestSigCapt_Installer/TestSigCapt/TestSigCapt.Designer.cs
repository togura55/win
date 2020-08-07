namespace TestSigCapt
{
    partial class TestSigCapt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
 
        private void InitializeComponent()
        {
            this.sigImage = new System.Windows.Forms.PictureBox();
            this.btnSign = new System.Windows.Forms.Button();
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnConvert = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sigImage)).BeginInit();
            this.SuspendLayout();
            // 
            // sigImage
            // 
            this.sigImage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.sigImage.Location = new System.Drawing.Point(26, 22);
            this.sigImage.Name = "sigImage";
            this.sigImage.Size = new System.Drawing.Size(200, 138);
            this.sigImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sigImage.TabIndex = 0;
            this.sigImage.TabStop = false;
            // 
            // btnSign
            // 
            this.btnSign.Location = new System.Drawing.Point(324, 22);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(94, 41);
            this.btnSign.TabIndex = 1;
            this.btnSign.Text = "Sign";
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
            // 
            // txtDisplay
            // 
            this.txtDisplay.Location = new System.Drawing.Point(26, 179);
            this.txtDisplay.Multiline = true;
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDisplay.Size = new System.Drawing.Size(392, 94);
            this.txtDisplay.TabIndex = 2;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(324, 69);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(94, 41);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Enabled = false;
            this.btnConvert.Location = new System.Drawing.Point(324, 116);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(94, 41);
            this.btnConvert.TabIndex = 4;
            this.btnConvert.Text = "FSS convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // TestSigCapt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 293);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.btnSign);
            this.Controls.Add(this.sigImage);
            this.Name = "TestSigCapt";
            this.Text = "TestSigCapt";
            ((System.ComponentModel.ISupportInitialize)(this.sigImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox sigImage;
        private System.Windows.Forms.Button btnSign;
        private System.Windows.Forms.TextBox txtDisplay;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnConvert;
    }
}

