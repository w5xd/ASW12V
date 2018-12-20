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
    public partial class TwoByRatPak : Form
    {
        public SerialPortHandler serialPortHandler { get { return m_Setup; } set { m_Setup = value; } }
        private SerialPortHandler m_Setup;

        public TwoByRatPak()
        {
            InitializeComponent();
        }


        public bool ManualControl
        {
            set
            {
                buttonL10m.Enabled = !value;
                buttonL15m.Enabled = !value;
                buttonL20m.Enabled = !value;
                buttonL40m.Enabled = !value;
                buttonL80_160m.Enabled = !value;
                buttonL80_15m.Enabled = !value;

                buttonR10m.Enabled = !value;
                buttonR15m.Enabled = !value;
                buttonR20m.Enabled = !value;
                buttonR40m.Enabled = !value;
                buttonR80_160m.Enabled = !value;
                buttonR80_15m.Enabled = !value;
            }
        }

        /* The RatPak switches are wired only to, and to all of, the inputs/outputs on board #2
         * Inputs observed as the switch positions are moved on the RatPaks. 
         * For both the L and R columns, the "other" RatPak was in an off position that reads all zeros.
         * L radio(all 4 L, and 2 of the M)   R radio (4 R and 2 M)
         * 10       C1 00                     C0 04
         * 15       C2 00                     C0 08
         * 20       C4 00                     C0 10
         * 40       C8 00                     C0 20
         * 80-160   C0 01                     C0 40
         * 80-15    C0 02                     C0 80
         * 
         */

        //                                                  10             15            20             40             80&160          80-15
        private string[] commandTableLRig = new string[] { "m 2 L1F M03", "m 2 L2F M03", "m 2 L4F M03", "m 2 L8F M03", "m 2 L0F M13", "m 2 L0F M23" };
        private string[] commandTableRRig = new string[] { "m 2 R0F M4C", "m 2 R0F M8C", "m 2 R1F M0C", "m 2 R2F M0C", "m 2 R4F M0C", "m 2 R8F M0C" };

        private void buttonL_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton whButton = sender as RadioButton;
            if (whButton.Checked)
            {
                int idx = whButton.TabIndex;
                if ((idx >= 0) && (idx < commandTableLRig.Length))
                    m_Setup.Command(commandTableLRig[idx] + "\r");
            }
        }

        private void buttonR_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton whButton = sender as RadioButton;
            if (whButton.Checked)
            {
                int idx = whButton.TabIndex;
                if ((idx >= 0) && (idx < commandTableRRig.Length))
                    m_Setup.Command(commandTableRRig[idx] + "\r");
            }
        }

        private void TwoByRatPak_Load(object sender, EventArgs e)
        {
            buttonL10m.CheckedChanged += new System.EventHandler(buttonL_CheckedChanged);
            buttonL15m.CheckedChanged += new System.EventHandler(buttonL_CheckedChanged);
            buttonL20m.CheckedChanged += new System.EventHandler(buttonL_CheckedChanged);
            buttonL40m.CheckedChanged += new System.EventHandler(buttonL_CheckedChanged);
            buttonL80_160m.CheckedChanged += new System.EventHandler(buttonL_CheckedChanged);
            buttonL80_15m.CheckedChanged += new System.EventHandler(buttonL_CheckedChanged);

            buttonR10m.CheckedChanged += new System.EventHandler(buttonR_CheckedChanged);
            buttonR15m.CheckedChanged += new System.EventHandler(buttonR_CheckedChanged);
            buttonR20m.CheckedChanged += new System.EventHandler(buttonR_CheckedChanged);
            buttonR40m.CheckedChanged += new System.EventHandler(buttonR_CheckedChanged);
            buttonR80_160m.CheckedChanged += new System.EventHandler(buttonR_CheckedChanged);
            buttonR80_15m.CheckedChanged += new System.EventHandler(buttonR_CheckedChanged);
        }
    }
}
