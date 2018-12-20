using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace W5XD_antennas
{
    /* Choose a serial port.
     * printout what commands are being sent.
     * provide a couple of diagnostic functions.
     */
    public partial class SerialPortHandler : Form
    {
        private Control m_Invoker;
        public SerialPortHandler(Control invoker)
        {
            m_Invoker = invoker;
            InitializeComponent();
        }

        private System.IO.Ports.SerialPort m_SerialPort;

        private void SetupForm_Load(object sender, EventArgs e)
        { }

        private void comboBoxSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_SerialPort != null)
                m_SerialPort.Dispose();
            String comName = comboBoxSerialPorts.SelectedItem.ToString();
            m_SerialPort = new System.IO.Ports.SerialPort(comName,
                9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            m_SerialPort.Handshake = System.IO.Ports.Handshake.None;
            m_SerialPort.Encoding = new System.Text.ASCIIEncoding();

            m_SerialPort.RtsEnable = true;
            m_SerialPort.DtrEnable = false;
            m_SerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
            m_SerialPort.Open();
            Command("\r");
        }

        public void Command(String c)
        {
            if ((m_SerialPort != null) && m_SerialPort.IsOpen)
                m_SerialPort.Write(c);
        }

        const int MAX_LENGTH = 64000;

        private void ReadNow()
        {
            String s = m_SerialPort.ReadExisting();
            textBoxSerial.Text += s;
            if (textBoxSerial.Text.Length > MAX_LENGTH)
                textBoxSerial.Text = textBoxSerial.Text.Substring(textBoxSerial.Text.Length - MAX_LENGTH);
            textBoxSerial.Select(textBoxSerial.Text.Length, 0);
            textBoxSerial.ScrollToCaret();
        }

        private void port_DataReceived(object sender,
              System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            m_Invoker.BeginInvoke(new Action(ReadNow));
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            Command("i");
        }

        private void buttonOutputs_Click(object sender, EventArgs e)
        {
            Command("o");
        }

        private void SerialPortHandler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void SerialPortHandler_Shown(object sender, EventArgs e)
        {
            comboBoxSerialPorts.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
                comboBoxSerialPorts.Items.Add(s);
        }
    }
}
