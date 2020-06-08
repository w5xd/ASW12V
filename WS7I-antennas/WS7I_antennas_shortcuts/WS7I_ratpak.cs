using System;
using System.Windows.Forms;
using System.Collections.Generic;
/*
MIT License

Copyright (c) 2020 Wayne Wright, W5XD

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

namespace WS7I_antennas
{
     public delegate WS7I_antennas.RemotableSerial GetSerial(Form f);

    /* This application drives an ASW12V on a serial port.
     * It is very specific to the WS7I antenna farm, so do not expect to
     * be able to use it for anything more than an example of how to use .NET to control 
     * an ASW12V. 
     */
    public partial class WS7I_ratpak : Form
    {
        private System.Threading.ManualResetEvent m_formLoaded;
        private GetSerial m_gs;
        public WS7I_ratpak(GetSerial gs, System.Threading.ManualResetEvent formLoaded = null)
        {
            m_formLoaded = formLoaded;
            m_gs = gs;
            InitializeComponent();
        }

        public void Command(String s)
        {
            m_Setup.Command(s);
        }

        private List<RadioButton> radioButtons = new List<RadioButton>();

        private int whichIsChecked = 0;

        private void WS7I_antennas_Load(object sender, EventArgs e)
        {
            if ((null == m_Setup) || (m_Setup.IsDisposed))
                m_Setup = m_gs(this);
            radioButtonDisconnected.Checked = true;
            radioButtons.Add(radioButtonDisconnected); // 0
            radioButtons.Add(radioButtonTH7);   // 1
            radioButtons.Add(radioButton7MHz); // 2
            radioButtons.Add(radioButton80m);   // 3
            radioButtons.Add(radioButton2MHz);  // 4
            radioButtons.Add(radioButtonVertical);  // 5
            radioButtons.Add(radioButtonLoop);  // 6
            if (null != m_formLoaded)
                m_formLoaded.Set();
        }

        public WS7I_antennas.RemotableSerial serialPortHandler { get { return m_Setup; } set { m_Setup = value; } }
        private WS7I_antennas.RemotableSerial m_Setup;

        private void buttonSetup_Click(object sender, EventArgs e)
        {
            if (m_Setup.IsDisposed)
                m_Setup = m_gs(this);
            m_Setup.Show();
        }

        const int AntennaDisconnect = 0;
        const int AntennaTH7 = 1;
        const int Antenna7Hz = 2;
        const int Antenna3dot5MHz = 3;
        const int Antenna2MHz = 4;
        const int AntennaVertical = 5;
        const int AntennaLoop = 6;

        public void DoShortcut(int which) 
        {
            if ((which >= 0) && (which < radioButtons.Count))
                radioButtons[which].Checked = true;
        }

        // array mapping the on-screen buttons to the strings to send to the ASW12V when that button is clicked.
        // The beverages account for 8 of the 12 channels on the device, and they are the Right and Middle (R and M in this array)
        private string[] commandTable = new string[] 
            
        {   "m 1 L0F M03",    // disconnect
            "m 1 L1F M03" ,     // TH7
            "m 1 L2F M03",      // 7MHz
            "m 1 L4F M03",      // 3.5 MHz
            "m 1 L8F M03",       // 2.0 MHz
            "m 1 L0F M13",       // Vertical
            "m 1 L0F M23",       // Loop
        };
        // Where did those magic strings come from? R1F M1F etc.
        // The beginning "m" means its a "mask" command to the ASW12V. It tells the firmware
        // to ignore certain inputs when computing the outputs and instead force the output 
        // on or off.
        //
        // The "m 1" means the mask command applies to the first board in the daisy chain. 
        // A "daisy chain" is constructed of multiple PCB's each with 3 groups of 4, L, M and R.
        //
        // The R1F means:
        // The R section is to be forced to value 1, and for all four of its bits (the "F")
        // 
      
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton whButton = sender as RadioButton;
            if (whButton != null && whButton.Checked)
            {
                whichIsChecked = whButton.TabIndex;
                if (!checkBoxManual.Checked)
                    m_Setup.Command(commandTable[whichIsChecked] + "\r");
            }
        }      

        public bool ManualControl { get { return checkBoxManual.Checked; } }

        private void checkBoxManual_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxManual.Checked)
                m_Setup.Command("c");
            else
                m_Setup.Command(commandTable[whichIsChecked] + "\r");
            EnableDisableRadios(!checkBoxManual.Checked);
        }

        private void EnableDisableRadios(bool enable)
        {
            foreach (var b in radioButtons)
                b.Enabled = enable;
        }

        private void WS7I_antennas_FormClosed(object sender, FormClosedEventArgs e)
        {
            // go back to full manual
            m_Setup.Command("c");
            m_Setup.Dispose();
            Properties.ASW12V.Default.Save();
        }
       
    }
}
