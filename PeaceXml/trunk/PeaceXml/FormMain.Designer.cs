namespace PeaceXml
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
            this.pbtn_exec = new System.Windows.Forms.Button();
            this.pbtn_help = new System.Windows.Forms.Button();
            this.pbtn_close = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage_main = new System.Windows.Forms.TabPage();
            this.textBox_destdir = new System.Windows.Forms.TextBox();
            this.pbtn_destdir = new System.Windows.Forms.Button();
            this.label_destdir = new System.Windows.Forms.Label();
            this.textBox_sourcedir = new System.Windows.Forms.TextBox();
            this.label_sourcedir = new System.Windows.Forms.Label();
            this.pbtn_sourcedir = new System.Windows.Forms.Button();
            this.groupBox_mode = new System.Windows.Forms.GroupBox();
            this.radioButton_split = new System.Windows.Forms.RadioButton();
            this.radioButton_merge = new System.Windows.Forms.RadioButton();
            this.tabPage_option = new System.Windows.Forms.TabPage();
            this.checkBox_sf = new System.Windows.Forms.CheckBox();
            this.label_sf = new System.Windows.Forms.Label();
            this.label_ha = new System.Windows.Forms.Label();
            this.textBox_ha = new System.Windows.Forms.TextBox();
            this.label_pa = new System.Windows.Forms.Label();
            this.textBox_pa = new System.Windows.Forms.TextBox();
            this.label_fa = new System.Windows.Forms.Label();
            this.textBox_fa = new System.Windows.Forms.TextBox();
            this.label_se = new System.Windows.Forms.Label();
            this.textBox_se = new System.Windows.Forms.TextBox();
            this.label_re = new System.Windows.Forms.Label();
            this.textBox_re = new System.Windows.Forms.TextBox();
            this.label_ca = new System.Windows.Forms.Label();
            this.textBox_ca = new System.Windows.Forms.TextBox();
            this.label_ex = new System.Windows.Forms.Label();
            this.textBox_ex = new System.Windows.Forms.TextBox();
            this.label_df = new System.Windows.Forms.Label();
            this.textBox_df = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPage_main.SuspendLayout();
            this.groupBox_mode.SuspendLayout();
            this.tabPage_option.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbtn_exec
            // 
            this.pbtn_exec.Location = new System.Drawing.Point(197, 261);
            this.pbtn_exec.Name = "pbtn_exec";
            this.pbtn_exec.Size = new System.Drawing.Size(75, 23);
            this.pbtn_exec.TabIndex = 1;
            this.pbtn_exec.Text = "button1";
            this.pbtn_exec.UseVisualStyleBackColor = true;
            this.pbtn_exec.Click += new System.EventHandler(this.pbtn_exec_Click);
            // 
            // pbtn_help
            // 
            this.pbtn_help.Location = new System.Drawing.Point(377, 261);
            this.pbtn_help.Name = "pbtn_help";
            this.pbtn_help.Size = new System.Drawing.Size(75, 23);
            this.pbtn_help.TabIndex = 3;
            this.pbtn_help.Text = "button4";
            this.pbtn_help.UseVisualStyleBackColor = true;
            this.pbtn_help.Click += new System.EventHandler(this.pbtn_help_Click);
            // 
            // pbtn_close
            // 
            this.pbtn_close.Location = new System.Drawing.Point(287, 261);
            this.pbtn_close.Name = "pbtn_close";
            this.pbtn_close.Size = new System.Drawing.Size(75, 23);
            this.pbtn_close.TabIndex = 2;
            this.pbtn_close.Text = "button1";
            this.pbtn_close.UseVisualStyleBackColor = true;
            this.pbtn_close.Click += new System.EventHandler(this.pbtn_close_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage_main);
            this.tabControl.Controls.Add(this.tabPage_option);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(464, 245);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage_main
            // 
            this.tabPage_main.BackColor = System.Drawing.Color.Transparent;
            this.tabPage_main.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPage_main.Controls.Add(this.textBox_destdir);
            this.tabPage_main.Controls.Add(this.pbtn_destdir);
            this.tabPage_main.Controls.Add(this.label_destdir);
            this.tabPage_main.Controls.Add(this.textBox_sourcedir);
            this.tabPage_main.Controls.Add(this.label_sourcedir);
            this.tabPage_main.Controls.Add(this.pbtn_sourcedir);
            this.tabPage_main.Controls.Add(this.groupBox_mode);
            this.tabPage_main.Location = new System.Drawing.Point(4, 22);
            this.tabPage_main.Name = "tabPage_main";
            this.tabPage_main.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_main.Size = new System.Drawing.Size(456, 219);
            this.tabPage_main.TabIndex = 0;
            this.tabPage_main.Text = "tabPage1";
            // 
            // textBox_destdir
            // 
            this.textBox_destdir.Location = new System.Drawing.Point(27, 163);
            this.textBox_destdir.Name = "textBox_destdir";
            this.textBox_destdir.Size = new System.Drawing.Size(270, 19);
            this.textBox_destdir.TabIndex = 3;
            // 
            // pbtn_destdir
            // 
            this.pbtn_destdir.Location = new System.Drawing.Point(319, 159);
            this.pbtn_destdir.Name = "pbtn_destdir";
            this.pbtn_destdir.Size = new System.Drawing.Size(75, 23);
            this.pbtn_destdir.TabIndex = 4;
            this.pbtn_destdir.Text = "button3";
            this.pbtn_destdir.UseVisualStyleBackColor = true;
            this.pbtn_destdir.Click += new System.EventHandler(this.pbtn_destdir_Click);
            // 
            // label_destdir
            // 
            this.label_destdir.AutoSize = true;
            this.label_destdir.Location = new System.Drawing.Point(25, 148);
            this.label_destdir.Name = "label_destdir";
            this.label_destdir.Size = new System.Drawing.Size(35, 12);
            this.label_destdir.TabIndex = 16;
            this.label_destdir.Text = "label2";
            // 
            // textBox_sourcedir
            // 
            this.textBox_sourcedir.Location = new System.Drawing.Point(27, 116);
            this.textBox_sourcedir.Name = "textBox_sourcedir";
            this.textBox_sourcedir.Size = new System.Drawing.Size(270, 19);
            this.textBox_sourcedir.TabIndex = 1;
            // 
            // label_sourcedir
            // 
            this.label_sourcedir.AutoSize = true;
            this.label_sourcedir.Location = new System.Drawing.Point(25, 101);
            this.label_sourcedir.Name = "label_sourcedir";
            this.label_sourcedir.Size = new System.Drawing.Size(35, 12);
            this.label_sourcedir.TabIndex = 13;
            this.label_sourcedir.Text = "label1";
            // 
            // pbtn_sourcedir
            // 
            this.pbtn_sourcedir.Location = new System.Drawing.Point(319, 114);
            this.pbtn_sourcedir.Name = "pbtn_sourcedir";
            this.pbtn_sourcedir.Size = new System.Drawing.Size(75, 23);
            this.pbtn_sourcedir.TabIndex = 2;
            this.pbtn_sourcedir.Text = "button2";
            this.pbtn_sourcedir.UseVisualStyleBackColor = true;
            this.pbtn_sourcedir.Click += new System.EventHandler(this.pbtn_sourcedir_Click);
            // 
            // groupBox_mode
            // 
            this.groupBox_mode.Controls.Add(this.radioButton_split);
            this.groupBox_mode.Controls.Add(this.radioButton_merge);
            this.groupBox_mode.Location = new System.Drawing.Point(21, 12);
            this.groupBox_mode.Name = "groupBox_mode";
            this.groupBox_mode.Size = new System.Drawing.Size(139, 68);
            this.groupBox_mode.TabIndex = 0;
            this.groupBox_mode.TabStop = false;
            this.groupBox_mode.Text = "groupBox1";
            // 
            // radioButton_split
            // 
            this.radioButton_split.AutoSize = true;
            this.radioButton_split.Location = new System.Drawing.Point(6, 49);
            this.radioButton_split.Name = "radioButton_split";
            this.radioButton_split.Size = new System.Drawing.Size(88, 16);
            this.radioButton_split.TabIndex = 1;
            this.radioButton_split.TabStop = true;
            this.radioButton_split.Text = "radioButton2";
            this.radioButton_split.UseVisualStyleBackColor = true;
            // 
            // radioButton_merge
            // 
            this.radioButton_merge.AutoSize = true;
            this.radioButton_merge.Location = new System.Drawing.Point(6, 27);
            this.radioButton_merge.Name = "radioButton_merge";
            this.radioButton_merge.Size = new System.Drawing.Size(88, 16);
            this.radioButton_merge.TabIndex = 0;
            this.radioButton_merge.TabStop = true;
            this.radioButton_merge.Text = "radioButton1";
            this.radioButton_merge.UseVisualStyleBackColor = true;
            // 
            // tabPage_option
            // 
            this.tabPage_option.BackColor = System.Drawing.Color.Transparent;
            this.tabPage_option.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabPage_option.Controls.Add(this.checkBox_sf);
            this.tabPage_option.Controls.Add(this.label_sf);
            this.tabPage_option.Controls.Add(this.label_ha);
            this.tabPage_option.Controls.Add(this.textBox_ha);
            this.tabPage_option.Controls.Add(this.label_pa);
            this.tabPage_option.Controls.Add(this.textBox_pa);
            this.tabPage_option.Controls.Add(this.label_fa);
            this.tabPage_option.Controls.Add(this.textBox_fa);
            this.tabPage_option.Controls.Add(this.label_se);
            this.tabPage_option.Controls.Add(this.textBox_se);
            this.tabPage_option.Controls.Add(this.label_re);
            this.tabPage_option.Controls.Add(this.textBox_re);
            this.tabPage_option.Controls.Add(this.label_ca);
            this.tabPage_option.Controls.Add(this.textBox_ca);
            this.tabPage_option.Controls.Add(this.label_ex);
            this.tabPage_option.Controls.Add(this.textBox_ex);
            this.tabPage_option.Controls.Add(this.label_df);
            this.tabPage_option.Controls.Add(this.textBox_df);
            this.tabPage_option.Location = new System.Drawing.Point(4, 22);
            this.tabPage_option.Name = "tabPage_option";
            this.tabPage_option.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_option.Size = new System.Drawing.Size(456, 219);
            this.tabPage_option.TabIndex = 1;
            this.tabPage_option.Text = "tabPage2";
            this.tabPage_option.Click += new System.EventHandler(this.tabPage_option_Click);
            // 
            // checkBox_sf
            // 
            this.checkBox_sf.AutoSize = true;
            this.checkBox_sf.Location = new System.Drawing.Point(38, 64);
            this.checkBox_sf.Name = "checkBox_sf";
            this.checkBox_sf.Size = new System.Drawing.Size(15, 14);
            this.checkBox_sf.TabIndex = 17;
            this.checkBox_sf.UseVisualStyleBackColor = true;
            // 
            // label_sf
            // 
            this.label_sf.AutoSize = true;
            this.label_sf.Location = new System.Drawing.Point(59, 66);
            this.label_sf.Name = "label_sf";
            this.label_sf.Size = new System.Drawing.Size(35, 12);
            this.label_sf.TabIndex = 16;
            this.label_sf.Text = "label9";
            // 
            // label_ha
            // 
            this.label_ha.AutoSize = true;
            this.label_ha.Location = new System.Drawing.Point(249, 157);
            this.label_ha.Name = "label_ha";
            this.label_ha.Size = new System.Drawing.Size(35, 12);
            this.label_ha.TabIndex = 15;
            this.label_ha.Text = "label8";
            this.label_ha.Visible = false;
            // 
            // textBox_ha
            // 
            this.textBox_ha.Location = new System.Drawing.Point(251, 172);
            this.textBox_ha.Name = "textBox_ha";
            this.textBox_ha.Size = new System.Drawing.Size(176, 19);
            this.textBox_ha.TabIndex = 14;
            this.textBox_ha.Visible = false;
            // 
            // label_pa
            // 
            this.label_pa.AutoSize = true;
            this.label_pa.Location = new System.Drawing.Point(249, 127);
            this.label_pa.Name = "label_pa";
            this.label_pa.Size = new System.Drawing.Size(35, 12);
            this.label_pa.TabIndex = 13;
            this.label_pa.Text = "label7";
            this.label_pa.Visible = false;
            // 
            // textBox_pa
            // 
            this.textBox_pa.Location = new System.Drawing.Point(251, 142);
            this.textBox_pa.Name = "textBox_pa";
            this.textBox_pa.Size = new System.Drawing.Size(176, 19);
            this.textBox_pa.TabIndex = 12;
            this.textBox_pa.Visible = false;
            // 
            // label_fa
            // 
            this.label_fa.AutoSize = true;
            this.label_fa.Location = new System.Drawing.Point(249, 94);
            this.label_fa.Name = "label_fa";
            this.label_fa.Size = new System.Drawing.Size(35, 12);
            this.label_fa.TabIndex = 11;
            this.label_fa.Text = "label6";
            this.label_fa.Visible = false;
            // 
            // textBox_fa
            // 
            this.textBox_fa.Location = new System.Drawing.Point(251, 109);
            this.textBox_fa.Name = "textBox_fa";
            this.textBox_fa.Size = new System.Drawing.Size(176, 19);
            this.textBox_fa.TabIndex = 10;
            this.textBox_fa.Visible = false;
            // 
            // label_se
            // 
            this.label_se.AutoSize = true;
            this.label_se.Location = new System.Drawing.Point(249, 64);
            this.label_se.Name = "label_se";
            this.label_se.Size = new System.Drawing.Size(35, 12);
            this.label_se.TabIndex = 9;
            this.label_se.Text = "label5";
            // 
            // textBox_se
            // 
            this.textBox_se.Location = new System.Drawing.Point(251, 83);
            this.textBox_se.Name = "textBox_se";
            this.textBox_se.Size = new System.Drawing.Size(176, 19);
            this.textBox_se.TabIndex = 8;
            // 
            // label_re
            // 
            this.label_re.AutoSize = true;
            this.label_re.Location = new System.Drawing.Point(249, 17);
            this.label_re.Name = "label_re";
            this.label_re.Size = new System.Drawing.Size(35, 12);
            this.label_re.TabIndex = 7;
            this.label_re.Text = "label4";
            // 
            // textBox_re
            // 
            this.textBox_re.Location = new System.Drawing.Point(251, 32);
            this.textBox_re.Name = "textBox_re";
            this.textBox_re.Size = new System.Drawing.Size(176, 19);
            this.textBox_re.TabIndex = 6;
            // 
            // label_ca
            // 
            this.label_ca.AutoSize = true;
            this.label_ca.Location = new System.Drawing.Point(36, 157);
            this.label_ca.Name = "label_ca";
            this.label_ca.Size = new System.Drawing.Size(35, 12);
            this.label_ca.TabIndex = 5;
            this.label_ca.Text = "label3";
            // 
            // textBox_ca
            // 
            this.textBox_ca.Location = new System.Drawing.Point(38, 172);
            this.textBox_ca.Name = "textBox_ca";
            this.textBox_ca.Size = new System.Drawing.Size(176, 19);
            this.textBox_ca.TabIndex = 4;
            // 
            // label_ex
            // 
            this.label_ex.AutoSize = true;
            this.label_ex.Location = new System.Drawing.Point(36, 110);
            this.label_ex.Name = "label_ex";
            this.label_ex.Size = new System.Drawing.Size(35, 12);
            this.label_ex.TabIndex = 3;
            this.label_ex.Text = "label2";
            // 
            // textBox_ex
            // 
            this.textBox_ex.Location = new System.Drawing.Point(38, 125);
            this.textBox_ex.Name = "textBox_ex";
            this.textBox_ex.Size = new System.Drawing.Size(176, 19);
            this.textBox_ex.TabIndex = 2;
            // 
            // label_df
            // 
            this.label_df.AutoSize = true;
            this.label_df.Location = new System.Drawing.Point(36, 3);
            this.label_df.Name = "label_df";
            this.label_df.Size = new System.Drawing.Size(35, 12);
            this.label_df.TabIndex = 1;
            this.label_df.Text = "label1";
            // 
            // textBox_df
            // 
            this.textBox_df.Location = new System.Drawing.Point(38, 32);
            this.textBox_df.Name = "textBox_df";
            this.textBox_df.Size = new System.Drawing.Size(176, 19);
            this.textBox_df.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 300);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pbtn_close);
            this.Controls.Add(this.pbtn_help);
            this.Controls.Add(this.pbtn_exec);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage_main.ResumeLayout(false);
            this.tabPage_main.PerformLayout();
            this.groupBox_mode.ResumeLayout(false);
            this.groupBox_mode.PerformLayout();
            this.tabPage_option.ResumeLayout(false);
            this.tabPage_option.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button pbtn_exec;
        private System.Windows.Forms.Button pbtn_help;
        private System.Windows.Forms.Button pbtn_close;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage_main;
        private System.Windows.Forms.TextBox textBox_destdir;
        private System.Windows.Forms.Button pbtn_destdir;
        private System.Windows.Forms.Label label_destdir;
        private System.Windows.Forms.TextBox textBox_sourcedir;
        private System.Windows.Forms.Label label_sourcedir;
        private System.Windows.Forms.Button pbtn_sourcedir;
        private System.Windows.Forms.GroupBox groupBox_mode;
        private System.Windows.Forms.RadioButton radioButton_split;
        private System.Windows.Forms.RadioButton radioButton_merge;
        private System.Windows.Forms.TabPage tabPage_option;
        private System.Windows.Forms.Label label_ha;
        private System.Windows.Forms.TextBox textBox_ha;
        private System.Windows.Forms.Label label_pa;
        private System.Windows.Forms.TextBox textBox_pa;
        private System.Windows.Forms.Label label_fa;
        private System.Windows.Forms.TextBox textBox_fa;
        private System.Windows.Forms.Label label_se;
        private System.Windows.Forms.TextBox textBox_se;
        private System.Windows.Forms.Label label_re;
        private System.Windows.Forms.TextBox textBox_re;
        private System.Windows.Forms.Label label_ca;
        private System.Windows.Forms.TextBox textBox_ca;
        private System.Windows.Forms.Label label_ex;
        private System.Windows.Forms.TextBox textBox_ex;
        private System.Windows.Forms.Label label_df;
        private System.Windows.Forms.TextBox textBox_df;
        private System.Windows.Forms.Label label_sf;
        private System.Windows.Forms.CheckBox checkBox_sf;
    }
}

