namespace WS7I_antennas
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
            if (disposing && m_SerialPort != null)
                    m_SerialPort.Dispose();

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
            this.SuspendLayout();
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(3, 12);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSerialPorts.TabIndex = 2;
            this.comboBoxSerialPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxSerialPorts_SelectedIndexChanged);
            // 
            // textBoxSerial
            // 
            this.textBoxSerial.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSerial.Location = new System.Drawing.Point(3, 111);
            this.textBoxSerial.Multiline = true;
            this.textBoxSerial.Name = "textBoxSerial";
            this.textBoxSerial.ReadOnly = true;
            this.textBoxSerial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSerial.Size = new System.Drawing.Size(307, 125);
            this.textBoxSerial.TabIndex = 3;
            // 
            // buttonInput
            // 
            this.buttonInput.Location = new System.Drawing.Point(197, 9);
            this.buttonInput.Name = "buttonInput";
            this.buttonInput.Size = new System.Drawing.Size(87, 23);
            this.buttonInput.TabIndex = 4;
            this.buttonInput.Text = "Read inputs";
            this.buttonInput.UseVisualStyleBackColor = true;
            this.buttonInput.Click += new System.EventHandler(this.buttonInput_Click);
            // 
            // buttonOutputs
            // 
            this.buttonOutputs.Location = new System.Drawing.Point(197, 63);
            this.buttonOutputs.Name = "buttonOutputs";
            this.buttonOutputs.Size = new System.Drawing.Size(87, 23);
            this.buttonOutputs.TabIndex = 5;
            this.buttonOutputs.Text = "Show outputs";
            this.buttonOutputs.UseVisualStyleBackColor = true;
            this.buttonOutputs.Click += new System.EventHandler(this.buttonOutputs_Click);
            // 
            // SerialPortHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 235);
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
    }
}