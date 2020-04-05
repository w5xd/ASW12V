namespace WS7I_antennas
{
    partial class WS7I_ratpak
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
            this.radioButtonTH7 = new System.Windows.Forms.RadioButton();
            this.radioButton7MHz = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisconnected = new System.Windows.Forms.RadioButton();
            this.radioButton80m = new System.Windows.Forms.RadioButton();
            this.radioButton2MHz = new System.Windows.Forms.RadioButton();
            this.buttonSetup = new System.Windows.Forms.Button();
            this.checkBoxManual = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonTH7
            // 
            this.radioButtonTH7.AutoSize = true;
            this.radioButtonTH7.Enabled = false;
            this.radioButtonTH7.Location = new System.Drawing.Point(20, 25);
            this.radioButtonTH7.Name = "radioButtonTH7";
            this.radioButtonTH7.Size = new System.Drawing.Size(49, 17);
            this.radioButtonTH7.TabIndex = 1;
            this.radioButtonTH7.TabStop = true;
            this.radioButtonTH7.Text = "TH-7";
            this.radioButtonTH7.UseVisualStyleBackColor = true;
            this.radioButtonTH7.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton7MHz
            // 
            this.radioButton7MHz.AutoSize = true;
            this.radioButton7MHz.Enabled = false;
            this.radioButton7MHz.Location = new System.Drawing.Point(79, 25);
            this.radioButton7MHz.Name = "radioButton7MHz";
            this.radioButton7MHz.Size = new System.Drawing.Size(56, 17);
            this.radioButton7MHz.TabIndex = 2;
            this.radioButton7MHz.TabStop = true;
            this.radioButton7MHz.Text = "7 MHz";
            this.radioButton7MHz.UseVisualStyleBackColor = true;
            this.radioButton7MHz.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonDisconnected);
            this.groupBox1.Controls.Add(this.radioButton80m);
            this.groupBox1.Controls.Add(this.radioButton2MHz);
            this.groupBox1.Controls.Add(this.radioButtonTH7);
            this.groupBox1.Controls.Add(this.radioButton7MHz);
            this.groupBox1.Location = new System.Drawing.Point(1, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(313, 86);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // radioButtonDisconnected
            // 
            this.radioButtonDisconnected.AutoSize = true;
            this.radioButtonDisconnected.Enabled = false;
            this.radioButtonDisconnected.Location = new System.Drawing.Point(20, 63);
            this.radioButtonDisconnected.Name = "radioButtonDisconnected";
            this.radioButtonDisconnected.Size = new System.Drawing.Size(91, 17);
            this.radioButtonDisconnected.TabIndex = 0;
            this.radioButtonDisconnected.TabStop = true;
            this.radioButtonDisconnected.Text = "Disconnected";
            this.radioButtonDisconnected.UseVisualStyleBackColor = true;
            this.radioButtonDisconnected.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton80m
            // 
            this.radioButton80m.AutoSize = true;
            this.radioButton80m.Enabled = false;
            this.radioButton80m.Location = new System.Drawing.Point(144, 25);
            this.radioButton80m.Name = "radioButton80m";
            this.radioButton80m.Size = new System.Drawing.Size(65, 17);
            this.radioButton80m.TabIndex = 3;
            this.radioButton80m.TabStop = true;
            this.radioButton80m.Text = "3.5 MHz";
            this.radioButton80m.UseVisualStyleBackColor = true;
            this.radioButton80m.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton2MHz
            // 
            this.radioButton2MHz.AutoSize = true;
            this.radioButton2MHz.Enabled = false;
            this.radioButton2MHz.Location = new System.Drawing.Point(214, 25);
            this.radioButton2MHz.Name = "radioButton2MHz";
            this.radioButton2MHz.Size = new System.Drawing.Size(56, 17);
            this.radioButton2MHz.TabIndex = 4;
            this.radioButton2MHz.TabStop = true;
            this.radioButton2MHz.Text = "2 MHz";
            this.radioButton2MHz.UseVisualStyleBackColor = true;
            this.radioButton2MHz.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // buttonSetup
            // 
            this.buttonSetup.Location = new System.Drawing.Point(21, 123);
            this.buttonSetup.Name = "buttonSetup";
            this.buttonSetup.Size = new System.Drawing.Size(59, 23);
            this.buttonSetup.TabIndex = 1;
            this.buttonSetup.Text = "Setup...";
            this.buttonSetup.UseVisualStyleBackColor = true;
            this.buttonSetup.Click += new System.EventHandler(this.buttonSetup_Click);
            // 
            // checkBoxManual
            // 
            this.checkBoxManual.AutoSize = true;
            this.checkBoxManual.Checked = true;
            this.checkBoxManual.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxManual.Location = new System.Drawing.Point(143, 126);
            this.checkBoxManual.Name = "checkBoxManual";
            this.checkBoxManual.Size = new System.Drawing.Size(61, 17);
            this.checkBoxManual.TabIndex = 2;
            this.checkBoxManual.Text = "Manual";
            this.checkBoxManual.UseVisualStyleBackColor = true;
            this.checkBoxManual.CheckedChanged += new System.EventHandler(this.checkBoxManual_CheckedChanged);
            // 
            // WS7I_ratpak
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 167);
            this.Controls.Add(this.checkBoxManual);
            this.Controls.Add(this.buttonSetup);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WS7I_ratpak";
            this.Text = "WS7I Antennas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WS7I_antennas_FormClosed);
            this.Load += new System.EventHandler(this.WS7I_antennas_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton radioButtonTH7;
        private System.Windows.Forms.RadioButton radioButton7MHz;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton80m;
        private System.Windows.Forms.RadioButton radioButton2MHz;
        private System.Windows.Forms.Button buttonSetup;
        private System.Windows.Forms.CheckBox checkBoxManual;
        private System.Windows.Forms.RadioButton radioButtonDisconnected;
    }
}