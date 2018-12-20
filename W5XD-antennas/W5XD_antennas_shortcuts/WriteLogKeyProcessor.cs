using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace W5XD_antennas_shortcuts
{
    // class to present the W5XD_antennas form under WriteLog's keyboard shortcut
    // processor. This enables us to make antenna switches from WriteLog without
    // moving the Windows keyboard focus out of WriteLog.
    public class WriteLogKeyProcessor : WriteLogShortcutHelper.ShortcutHelper, IDisposable
    {
        /* These are the names of the commands as they will appear
                   in WriteLog's Keyboard Shortcuts "Command to run" list: */
        private string[] CommandNames = new string[] {
                // These will appear in WriteLog with "External:" prepended.
                // Look in the E's in that menu.
                "PrimaryClockwise",
                "PrimaryCounterClockwise",
                "SecondaryClockwise",
                "SecondaryCounterClockwise",
         };

        public override string this[int which]
        {
            get
            {
                if ((which < 0) || (which >= CommandNames.Length))
                    throw new IndexOutOfRangeException();
                return CommandNames[which];
            }
        }

        public override int ShortcutCount { get { return CommandNames.Length; } }

        public override void InvokeShortcut(int which, global::WriteLogClrTypes.IWriteL wl)
        {
            if ((which < 0) || (which >= CommandNames.Length))
                throw new IndexOutOfRangeException();

            if (!m_running)
            {   // put the .NET gui on its own thread. It "works" on WriteLog's main thread,
                // but the .NET clr has some GUI features that only work properly if 
                // the clr has that gui's message loop. so...here we go...
                System.Threading.ManualResetEvent ev = new System.Threading.ManualResetEvent(false);
                guiThread = new System.Threading.Thread(() => GuiThreadEntry(ev));
                guiThread.Start();
                ev.WaitOne(); // inside the form, on its FormLoad handler, this event is set.
                ev.Dispose();
            }

            m_doShortcut?.Invoke(which);
        }

        public void Dispose()
        {
            if (m_running)
            {
                m_closeThread?.Invoke();
                if (guiThread != null)
                    guiThread.Join();
            }
        }

        private bool m_running = false;

        System.Threading.Thread guiThread;

        private delegate void CloseThread();
        CloseThread m_closeThread;
        private delegate void DoShortcut(int i);
        DoShortcut m_doShortcut;

        private void GuiThreadEntry(System.Threading.ManualResetEvent e)
        {
            m_running = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            W5XD_antennas.W5XD_beverages form = new W5XD_antennas.W5XD_beverages(e);
            // calls from WriteLog are on the wrong thread. Use BeginInvoke to 
            // cross-thread them.
            m_closeThread = () => {
                form.BeginInvoke(new Action(form.Close));
            };
            m_doShortcut = (int i) =>  {
                form.BeginInvoke(new Action<int>(form.DoShortcut), i);
            };
            Application.Run(form); // stay in here as long as form is up.
            m_running = false;
            m_doShortcut = null;
        }

    }
}
