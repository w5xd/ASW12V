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
    public partial class SerialPortHandler : Form, RemotableSerial
    {
        const uint UpgradedVersion = 2;  // increment every release
        private Control m_Invoker;
        private int m_whichPort;
        public SerialPortHandler(Control invoker)
        {
            m_Invoker = invoker;
            m_SerialPorts = new List<System.IO.Ports.SerialPort>() { null, null };
            prevCommPorts = new List<string>() { null, null };
            InitializeComponent();
            uint settingsVersion = Properties.ASW12V.Default.SavedVersion;
            if (settingsVersion < UpgradedVersion)
            {
                Properties.ASW12V.Default.Upgrade();
                Properties.ASW12V.Default.SavedVersion = UpgradedVersion;
            }

            String[] portNames = { Properties.ASW12V.Default.CommPort, Properties.ASW12V.Default.CommPort2 };

            for (int i = 0; i < m_SerialPorts.Count; i++)
            {
                try
                {
                    InitSerialPort(i, portNames[i]);
                }
                catch (System.Exception)
                {
                    if (m_SerialPorts[i] != null)
                        m_SerialPorts[i].Dispose();
                    m_SerialPorts[i] = null;
                }
            }
        }

        private List<System.IO.Ports.SerialPort> m_SerialPorts;

        private void SetupForm_Load(object sender, EventArgs e)
        {
        }

        private void comboBoxSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            String comName = comboBoxSerialPorts.SelectedItem.ToString();
            InitSerialPort(0, comName);
            Properties.ASW12V.Default.CommPort = comName;
        }
        private void comboBoxSerialPorts2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String comName = comboBoxSerialPorts2.SelectedItem.ToString();
            InitSerialPort(1, comName);
            Properties.ASW12V.Default.CommPort2 = comName;
        }
        private List<string> prevCommPorts;

        private void InitSerialPort(int i, String comName)
        {
            if (null != m_SerialPorts[i] && String.Equals(comName,prevCommPorts[i]))
                return;
            if (m_SerialPorts[i] != null)
                m_SerialPorts[i].Dispose();
            m_SerialPorts[i] = new System.IO.Ports.SerialPort(comName,
               9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            m_SerialPorts[i].Handshake = System.IO.Ports.Handshake.None;
            m_SerialPorts[i].Encoding = new System.Text.ASCIIEncoding();

            m_SerialPorts[i].RtsEnable = true;
            m_SerialPorts[i].DtrEnable = false;
            m_SerialPorts[i].DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
            m_SerialPorts[i].Open();
            if (i == 0)
                textBoxSerial.Clear();
            else if (i == 1)
                textBoxSerial2.Clear();
            m_whichPort = i;
            Command("\r");
            prevCommPorts[i] = comName;
        }

        public void Command(String c)
        {
            if (c.StartsWith("port="))
            {
                c = c.Substring(5);
                int i = 0;
                while (c.Any())
                {
                    if (Char.IsDigit(c[0]))
                    {
                        i *= 10;
                        i += c[0] - '0';
                        c = c.Substring(1);
                    }
                    else 
                    {
                        while (c.Any() && Char.IsWhiteSpace(c[0]))
                            c = c.Substring(1);
                        break;
                    }
                }
                if (i < m_SerialPorts.Count)
                    m_whichPort = i;
                if (String.IsNullOrEmpty(c))
                    return;
            }
            if ((m_SerialPorts[m_whichPort] != null) && m_SerialPorts[m_whichPort].IsOpen)
                m_SerialPorts[m_whichPort].Write(c);
        }

        const int MAX_LENGTH = 64000;

        private void ReadNow()
        {
            TextBox[] tbArray = { textBoxSerial, textBoxSerial2 };
            for (int i = 0; i < m_SerialPorts.Count; i++)
            {
                if (null != m_SerialPorts[i])
                {
                    String s = m_SerialPorts[i].ReadExisting();
                    if (String.IsNullOrEmpty(s))
                        continue;
                    tbArray[i].Text += s;
                    if (tbArray[i].Text.Length > MAX_LENGTH)
                        tbArray[i].Text = tbArray[i].Text.Substring(tbArray[i].Text.Length - MAX_LENGTH);
                    tbArray[i].Select(tbArray[i].Text.Length, 0);
                    tbArray[i].ScrollToCaret();
                }
            }
        }

        private void port_DataReceived(object sender,
              System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            m_Invoker.BeginInvoke(new Action(ReadNow));
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            m_whichPort = 0;
            Command("i");
        }
        private void buttonInput2_Click(object sender, EventArgs e)
        {
            m_whichPort = 1;
            Command("i");
        }

        private void buttonOutputs_Click(object sender, EventArgs e)
        {
            m_whichPort = 0;
            Command("o");
        }
        private void buttonOutputs2_Click(object sender, EventArgs e)
        {
            m_whichPort = 1;
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
            var prevCommPort1 = Properties.ASW12V.Default.CommPort;
            var prevCommPort2 = Properties.ASW12V.Default.CommPort2;
            object selItem1 = null;
            object selItem2 = null;
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                var sel = comboBoxSerialPorts.Items.Add(s);
                if (s == prevCommPort1)
                    selItem1 = s;
                sel = comboBoxSerialPorts2.Items.Add(s);
                if (s == prevCommPort2)
                    selItem2 = s;
            }
            if (selItem1 != null)
                comboBoxSerialPorts.SelectedItem = selItem1; ;
            if (selItem2 != null)
                comboBoxSerialPorts2.SelectedItem = selItem2; ;
        }

    }
}
