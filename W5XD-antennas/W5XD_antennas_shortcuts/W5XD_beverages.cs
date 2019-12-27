using System;
using System.Windows.Forms;
using System.Collections.Generic;
/*
MIT License

Copyright (c) 2018 Wayne Wright

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

namespace W5XD_antennas
{

     public delegate W5XD_antennas.RemotableSerial GetSerial(Form f);

    /* This application drives an ASW12V on a serial port.
     * It is very specific to the W5XD antenna farm, so do not expect to
     * be able to use it for anything more than an example of how to use .NET to control 
     * an ASW12V. 
     */
    public partial class W5XD_beverages : Form
    {
        private System.Threading.ManualResetEvent m_formLoaded;
        private GetSerial m_gs;
        public W5XD_beverages(GetSerial gs, System.Threading.ManualResetEvent formLoaded = null)
        {
            m_formLoaded = formLoaded;
            m_gs = gs;
            InitializeComponent();
        }

        public void Command(String s)
        {
            m_Setup.Command(s);
        }

        private void W5XD_antennas_Load(object sender, EventArgs e)
        {
            if ((null == m_Setup) || (m_Setup.IsDisposed))
                m_Setup = m_gs(this);
            radioButtonL_SW.Checked = true;
            radioButtonR_SW.Checked = true;
            m_primaryRadioButtons.Add(radioButtonL_SW);
            m_primaryRadioButtons.Add(radioButtonL_NW);
            m_primaryRadioButtons.Add(radioButtonL_NE);
            m_primaryRadioButtons.Add(radioButtonL_SE);

            m_secondaryRadioButtons.Add(radioButtonR_SW);
            m_secondaryRadioButtons.Add(radioButtonR_NW);
            m_secondaryRadioButtons.Add(radioButtonR_NE);
            m_secondaryRadioButtons.Add(radioButtonR_SE);
            if (null != m_formLoaded)
                m_formLoaded.Set();
        }

        public W5XD_antennas.RemotableSerial serialPortHandler { get { return m_Setup; } set { m_Setup = value; } }
        private W5XD_antennas.RemotableSerial m_Setup;

        private void buttonSetup_Click(object sender, EventArgs e)
        {
            if (m_Setup.IsDisposed)
                m_Setup = m_gs(this);
            m_Setup.Show();
        }

        int PrimaryIndex = 0;
        int SecondaryIndex = 0;

        private List<RadioButton> m_primaryRadioButtons = new List<RadioButton>();
        private List<RadioButton> m_secondaryRadioButtons = new List<RadioButton>();

        const int PrimaryClockwise = 0;
        const int PrimaryCounterClockwise = 1;
        const int SecondaryClockwise = 2;
        const int SecondaryCounterClockwise = 3;

        public void DoShortcut(int which)
        {
            RadioButton toCheck = null;
            int idx;
            switch (which)
            {
                case PrimaryClockwise:
                    idx = PrimaryIndex + 1;
                    if (idx >= m_primaryRadioButtons.Count)
                        idx = 0;
                    toCheck = m_primaryRadioButtons[idx];
                    break;
                case PrimaryCounterClockwise:
                    idx = PrimaryIndex - 1;
                    if (idx < 0)
                        idx = m_primaryRadioButtons.Count - 1;
                    toCheck = m_primaryRadioButtons[idx];
                    break;
                case SecondaryClockwise:
                    idx = SecondaryIndex + 1;
                    if (idx >= m_secondaryRadioButtons.Count)
                        idx = 0;
                    toCheck = m_secondaryRadioButtons[idx];
                    break;
                case SecondaryCounterClockwise:
                    idx = SecondaryIndex - 1;
                    if (idx < 0)
                        idx = m_secondaryRadioButtons.Count - 1;
                    toCheck = m_secondaryRadioButtons[idx];
                    break;
            }
            if ((toCheck != null) && toCheck.Enabled)
                toCheck.Checked = true;
        }

        // 2D array mapping the on-screen buttons to the strings to send to the ASW12V when that button is clicked.
        // The beverages account for 8 of the 12 channels on the device, and they are the Right and Middle (R and M in this array)
        //                                                  PRI:  SW             NW              NE             SE                 //SEC
        private string[][] commandTable = new string[][] {  new string[]{ "m 1 R1F M1F", "m 1 R2F M3F" , "m 1 R4F M5F", "m 1 R8F M9F"},    //SW
                                                    new string[]{ "m 1 R1F M3F", "m 1 R2F M2F" , "m 1 R4F M6F", "m 1 R8F MAF"},    //NW
                                                    new string[]{ "m 1 R1F M5F", "m 1 R2F M6F" , "m 1 R4F M4F", "m 1 R8F MCF"},    //NE
                                                    new string[]{ "m 1 R1F M9F", "m 1 R2F MAF" , "m 1 R4F MCF", "m 1 R8F M8F"},    //SE
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
        // With the ASW12V in "manual" mode (i.e. passing its inputs straight to its outputs)
        // Move the operator position switches to their various positions and click the "Read inputs"
        // on the AWS12V Setup page of this program. Here are the results:
        //
        //  PRIMARY RIG:  SW             NW              NE             SE     //SECONDAY RIG
        //             11 C0          23 C0           45 C0          89 C0     //SW
        //             13 C0          22 C0           46 C0          8A C0     //NW
        //             15 C0          26 C0           44 C0          8C C0     //NE
        //             19 C0          2A C0           4C C0          88 C0     //SE
        // 
        // The "C0" just shows the fact that the L inputs don't change when I move the beverage
        // switches. The beverage switch inputs are all on R and M, so the output commands in 
        // the table are all R and M. Nothing for L.
        // 
        // Inspecting the results of the inputs and the corresponding commands above, you
        // can see (eventually, if you look long enough) that this program uses its on-screen
        // buttons to force the ASW12V outputs to be what they would be if only the
        // operator were present to turn the rotary switches.
        // 

        /* The particular beverage relay switching arrangment at W5XD
         * has the limitation that the Secondary radio, if the operator
         * switches it to the same compass position as the primary radio,
         * get no antenna at all. This program does the courtesy of
         * arbitrarily switching SOME direction onto the secondary radio
         * should the primary be commanded to the direction currently
         * heard by the secondary.
         */
        private void IfSecondaryMatches(bool SecondaryPriority)
        {
            if (PrimaryIndex == SecondaryIndex)
            {
                bool save = m_holdChanges;
                // while we're setting the button states here, stop our notification routine actions
                m_holdChanges = true;
                if (SecondaryPriority)
                {
                    PrimaryIndex += 1;
                    if (PrimaryIndex >= 4)
                        PrimaryIndex = 0;
                    switch (PrimaryIndex)
                    {
                        case 0:
                            radioButtonL_SW.Checked = true;
                            break;
                        case 1:
                            radioButtonL_NW.Checked = true;
                            break;
                        case 2:
                            radioButtonL_NE.Checked = true;
                            break;
                        case 3:
                            radioButtonL_SE.Checked = true;
                            break;
                    }
                }
                else
                {
                    SecondaryIndex += 1;
                    if (SecondaryIndex >= 4)
                        SecondaryIndex = 0;
                    switch (SecondaryIndex)
                    {
                        case 0:
                            radioButtonR_SW.Checked = true;
                            break;
                        case 1:
                            radioButtonR_NW.Checked = true;
                            break;
                        case 2:
                            radioButtonR_NE.Checked = true;
                            break;
                        case 3:
                            radioButtonR_SE.Checked = true;
                            break;
                    }
                }
                m_holdChanges = save;
            }
        }

        private bool m_holdChanges = false;

        private void radioButtonL_CheckedChanged(object sender, EventArgs e)
        {
            if (m_holdChanges)
                return;
            RadioButton whButton = sender as RadioButton;
            if (whButton.Checked)
            {
                PrimaryIndex = whButton.TabIndex;
                IfSecondaryMatches(false);
                if (!checkBoxManual.Checked)
                    m_Setup.Command(commandTable[SecondaryIndex][PrimaryIndex] + "\r");
            }
        }

        private void radioButtonR_CheckedChanged(object sender, EventArgs e)
        {
            if (m_holdChanges)
                return;
            RadioButton whButton = sender as RadioButton;
            if (whButton.Checked)
            {
                SecondaryIndex = whButton.TabIndex;
                IfSecondaryMatches(true);
                if (!checkBoxManual.Checked)
                    m_Setup.Command(commandTable[SecondaryIndex][PrimaryIndex] + "\r");
            }
        }

        public bool ManualControl { get { return checkBoxManual.Checked; } }

        private void checkBoxManual_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxManual.Checked)
                m_Setup.Command("c");
            else
                m_Setup.Command(commandTable[SecondaryIndex][PrimaryIndex] + "\r");
            EnableDisableRadios(!checkBoxManual.Checked);
            if (m_ratPakForm != null && !m_ratPakForm.IsDisposed)
                m_ratPakForm.ManualControl = ManualControl;
        }

        private void EnableDisableRadios(bool enable)
        {
            foreach (var b in m_primaryRadioButtons)
                b.Enabled = enable;
            foreach (var b in m_secondaryRadioButtons)
                b.Enabled = enable;
        }

        private void W5XD_antennas_FormClosed(object sender, FormClosedEventArgs e)
        {
            // go back to full manual
            m_Setup.Command("c");
            m_Setup.Dispose();
            Properties.ASW12V.Default.Save();
        }

        private void checkBoxLoopTunerPower_CheckedChanged(object sender, EventArgs e)
        {
            // The W5XD station has an MFJ-998RT tuner. The "R" means remote,
            // which means the operator cannot reach its front panel even when
            // sitting at the operating position. In my case, its at the base
            // of my tower. That tuner can be forced to retune itself by removing
            // and reapplying its 12V DC. It happens to be wired to
            // the Left section, position 1. There is no input to be monitored
            // because the W5XD shack has no switch within reach of the operator.
            String s = checkBoxLoopTunerPower.Checked ?
                "m 1 L11\r" : "m 1 L01\r";
            m_Setup.Command(s);
        }

        private void buttonRatpak_Click(object sender, EventArgs e)
        {
            if ((m_ratPakForm == null) || m_ratPakForm.IsDisposed)
            {
                m_ratPakForm = new TwoByRatPak();
                m_ratPakForm.Show();
            }
            else if (m_ratPakForm.WindowState == FormWindowState.Minimized)
                m_ratPakForm.WindowState = FormWindowState.Normal;
            m_ratPakForm.serialPortHandler = m_Setup;
            m_ratPakForm.ManualControl = ManualControl;
        }
        private TwoByRatPak m_ratPakForm;
        public TwoByRatPak ratPakForm { set { m_ratPakForm = value; } }
       
    }
}
