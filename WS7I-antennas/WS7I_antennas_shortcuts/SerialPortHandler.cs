using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS7I_antennas
{
    /* Choose a serial port.
     * printout what commands are being sent.
     * provide a couple of diagnostic functions.
     */
    public partial class SerialPortHandler : Form, RemotableSerial
    {
        private Control m_Invoker;
        public SerialPortHandler(Control invoker)
        {
            m_Invoker = invoker;
            InitializeComponent();
            uint settingsVersion = Properties.ASW12V.Default.SavedVersion;
            if (settingsVersion < UpgradedVersion)
            {
                Properties.ASW12V.Default.Upgrade();
                Properties.ASW12V.Default.SavedVersion = UpgradedVersion;
            }

            try
            {
                InitSerialPort(Properties.ASW12V.Default.CommPort);
            }
            catch (System.Exception)
            {
                if (null != m_SerialPort)
                {
                    m_SerialPort.Dispose();
                    m_SerialPort = null;
                }
            }
        }

        private System.IO.Ports.SerialPort m_SerialPort;
        const uint UpgradedVersion = 1;  // increment every release

        private void SetupForm_Load(object sender, EventArgs e)
        {
        }

        private void comboBoxSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            String comName = comboBoxSerialPorts.SelectedItem.ToString();
            InitSerialPort(comName);
            Properties.ASW12V.Default.CommPort = comName;
        }

        private string prevCommPort;

        private void InitSerialPort(String comName)
        {
            if (null != m_SerialPort && String.Equals(comName,prevCommPort))
                return;
            if (m_SerialPort != null)
                m_SerialPort.Dispose();
            m_SerialPort = new System.IO.Ports.SerialPort(comName,
               9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            m_SerialPort.Handshake = System.IO.Ports.Handshake.None;
            m_SerialPort.Encoding = new System.Text.ASCIIEncoding();

            m_SerialPort.RtsEnable = true;
            m_SerialPort.DtrEnable = false;
            m_SerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
            m_SerialPort.Open();
            textBoxSerial.Clear();
            Command("\r");
            prevCommPort = comName;
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
                this.Shown += new System.EventHandler(this.SerialPortHandler_Shown);
            }
        }

        private void SerialPortHandler_Shown(object sender, EventArgs e)
        {
            var prevCommPort = Properties.ASW12V.Default.CommPort;
            object selItem = null;
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                var sel = comboBoxSerialPorts.Items.Add(s);
                if (s == prevCommPort)
                    selItem = s;
            }
            if (selItem != null)
                comboBoxSerialPorts.SelectedItem = selItem; ;
        }
    }
}
