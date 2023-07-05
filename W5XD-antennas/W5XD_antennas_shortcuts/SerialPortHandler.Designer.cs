namespace W5XD_antennas
{
    partial class SerialPortHandler
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
            if (disposing)
                foreach (var p in m_SerialPorts)
                    if (null != p)
                        p.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.textBoxSerial = new System.Windows.Forms.TextBox();
            this.buttonInput = new System.Windows.Forms.Button();
            this.buttonOutputs = new System.Windows.Forms.Button();
            this.buttonOutputs2 = new System.Windows.Forms.Button();
            this.buttonInput2 = new System.Windows.Forms.Button();
            this.comboBoxSerialPorts2 = new System.Windows.Forms.ComboBox();
            this.textBoxSerial2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(3, 12);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(111, 21);
            this.comboBoxSerialPorts.TabIndex = 2;
            this.comboBoxSerialPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxSerialPorts_SelectedIndexChanged);
            // 
            // textBoxSerial
            // 
            this.textBoxSerial.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSerial.Location = new System.Drawing.Point(3, 39);
            this.textBoxSerial.Multiline = true;
            this.textBoxSerial.Name = "textBoxSerial";
            this.textBoxSerial.ReadOnly = true;
            this.textBoxSerial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSerial.Size = new System.Drawing.Size(307, 129);
            this.textBoxSerial.TabIndex = 3;
            // 
            // buttonInput
            // 
            this.buttonInput.Location = new System.Drawing.Point(120, 10);
            this.buttonInput.Name = "buttonInput";
            this.buttonInput.Size = new System.Drawing.Size(74, 23);
            this.buttonInput.TabIndex = 4;
            this.buttonInput.Text = "Read inputs";
            this.buttonInput.UseVisualStyleBackColor = true;
            this.buttonInput.Click += new System.EventHandler(this.buttonInput_Click);
            // 
            // buttonOutputs
            // 
            this.buttonOutputs.Location = new System.Drawing.Point(200, 10);
            this.buttonOutputs.Name = "buttonOutputs";
            this.buttonOutputs.Size = new System.Drawing.Size(100, 23);
            this.buttonOutputs.TabIndex = 5;
            this.buttonOutputs.Text = "Show outputs";
            this.buttonOutputs.UseVisualStyleBackColor = true;
            this.buttonOutputs.Click += new System.EventHandler(this.buttonOutputs_Click);
            // 
            // buttonOutputs2
            // 
            this.buttonOutputs2.Location = new System.Drawing.Point(200, 172);
            this.buttonOutputs2.Name = "buttonOutputs2";
            this.buttonOutputs2.Size = new System.Drawing.Size(100, 23);
            this.buttonOutputs2.TabIndex = 8;
            this.buttonOutputs2.Text = "Show outputs";
            this.buttonOutputs2.UseVisualStyleBackColor = true;
            this.buttonOutputs2.Click += new System.EventHandler(this.buttonOutputs2_Click);
            // 
            // buttonInput2
            // 
            this.buttonInput2.Location = new System.Drawing.Point(120, 172);
            this.buttonInput2.Name = "buttonInput2";
            this.buttonInput2.Size = new System.Drawing.Size(74, 23);
            this.buttonInput2.TabIndex = 7;
            this.buttonInput2.Text = "Read inputs";
            this.buttonInput2.UseVisualStyleBackColor = true;
            this.buttonInput2.Click += new System.EventHandler(this.buttonInput2_Click);
            // 
            // comboBoxSerialPorts2
            // 
            this.comboBoxSerialPorts2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialPorts2.FormattingEnabled = true;
            this.comboBoxSerialPorts2.Location = new System.Drawing.Point(3, 174);
            this.comboBoxSerialPorts2.Name = "comboBoxSerialPorts2";
            this.comboBoxSerialPorts2.Size = new System.Drawing.Size(111, 21);
            this.comboBoxSerialPorts2.TabIndex = 6;
            this.comboBoxSerialPorts2.SelectedIndexChanged += new System.EventHandler(this.comboBoxSerialPorts2_SelectedIndexChanged);
            // 
            // textBoxSerial2
            // 
            this.textBoxSerial2.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSerial2.Location = new System.Drawing.Point(3, 201);
            this.textBoxSerial2.Multiline = true;
            this.textBoxSerial2.Name = "textBoxSerial2";
            this.textBoxSerial2.ReadOnly = true;
            this.textBoxSerial2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSerial2.Size = new System.Drawing.Size(307, 129);
            this.textBoxSerial2.TabIndex = 9;
            // 
            // SerialPortHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 327);
            this.Controls.Add(this.textBoxSerial2);
            this.Controls.Add(this.buttonOutputs2);
            this.Controls.Add(this.buttonInput2);
            this.Controls.Add(this.comboBoxSerialPorts2);
            this.Controls.Add(this.buttonOutputs);
            this.Controls.Add(this.buttonInput);
            this.Controls.Add(this.textBoxSerial);
            this.Controls.Add(this.comboBoxSerialPorts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SerialPortHandler";
            this.Text = "AWS12V Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialPortHandler_FormClosing);
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.Shown += new System.EventHandler(this.SerialPortHandler_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxSerialPorts;
        private System.Windows.Forms.TextBox textBoxSerial;
        private System.Windows.Forms.Button buttonInput;
        private System.Windows.Forms.Button buttonOutputs;
        private System.Windows.Forms.Button buttonOutputs2;
        private System.Windows.Forms.Button buttonInput2;
        private System.Windows.Forms.ComboBox comboBoxSerialPorts2;
        private System.Windows.Forms.TextBox textBoxSerial2;
    }
}