using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WriteLogRemoteControlShortcuts;

namespace W5XD_antennas_shortcuts
{
    // class to present the W5XD_antennas form under WriteLog's keyboard shortcut
    // processor. This enables us to make antenna switches from WriteLog without
    // moving the Windows keyboard focus out of WriteLog.

    // the class is abstract and has separate specializations for WriteLog's
    // control site and remote site. The latter has a real serial port to
    // write, the former tells the latter what to write.

    public abstract class WriteLogKeyProcessorBase
        : WriteLogShortcutHelper.ShortcutHelper
        ,IDisposable
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
                // but the .NET clr has some GUI keyboard features that only work properly if 
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
        protected delegate void Command(String s);
        protected Command m_command;

        protected abstract W5XD_antennas.GetSerial getSerial();

        private void GuiThreadEntry(System.Threading.ManualResetEvent e)
        {
            m_running = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            W5XD_antennas.W5XD_beverages form = new W5XD_antennas.W5XD_beverages(getSerial(), e);
            // calls from WriteLog are on the wrong thread. Use BeginInvoke to 
            // cross-thread them.
            m_closeThread = () => {
                form.BeginInvoke(new Action(form.Close));
            };
            m_doShortcut = (int i) =>  {
                form.BeginInvoke(new Action<int>(form.DoShortcut), i);
            };
            m_command = (String s) =>
            {
                form.BeginInvoke(new Action<String>(form.Command), s);
            };
            Application.Run(form); // stay in here as long as form is up.
            m_running = false;
            form.Dispose();
            m_command = null;
            m_doShortcut = null;
            m_closeThread = null;
            m_doShortcut = null;
        }
    
    }
    /* key processor has two derivatives
     * WriteLogKeyProcessor is the "normal" one. In a remote control 
     * scenario, it is on the remote.
     * 
     * WriteLogKeyProcessorControlSite doesn't work locally. It 
     * must be connected to a remote site site WriteLogKeyProcessor
     */

    // instance this class to control a local COM port. We are "remote-site"
    public class WriteLogKeyProcessor 
        : WriteLogKeyProcessorBase
        , WriteLogRemoteControlShortcuts.IRemoteSiteShortcutRpc
    {

        protected override W5XD_antennas.GetSerial getSerial()
        {
            return (Form f) => { return new W5XD_antennas.SerialPortHandler(f); };
        }

        #region remote site

        // our design for remote/control split is for the control send us
        // strings to be written to the serial port
        public byte[] FromControl(byte[] buf)
        {
            String cmd = System.Text.Encoding.Unicode.GetString(buf);
            m_command?.Invoke(cmd);
#if DEBUG
            // We are an example of a remote-site, control-site split
            // and happen not to use the fact that the remote site (which 
            // is us in this method) can send a reponse back to control.
            return new byte[] { 1, 2, 3};
#else
            return null; // no answers
#endif
        }

        public void OnDisconnectRemote()
        {
        }

#endregion
    }

    // instance this class to send the commands to the remote
    public class WriteLogKeyProcessorControlSite
        :WriteLogKeyProcessorBase
        ,WriteLogRemoteControlShortcuts.IControlSiteShortcutRpc
        , W5XD_antennas.RemotableSerial
    {

        protected override W5XD_antennas.GetSerial getSerial()
        {
            return (Form f) => { return this; };
        }

#region IControlSiteShortcutRpc

        // We're at the control site. Tell WL at the remote how to dispatch the RPCs 
        public String KeyAtRemote { get
            { return "W5XD_antennas_shortcuts.WriteLogKeyProcessor=W5XD_antennas_shortcuts"; } }

        // this class won't work if there is no remote to talk to
        public bool ControlSiteOnly { get { return true; } }

        private ToRemote m_toRemote;
        public ToRemote toRemote { set { m_toRemote = value; } }

        public void FromRemote(byte[] buf)
        {   /* not used. will be [1,2,3] in DEBUG */        }

        #endregion

        #region  RemotableSerial
        public new void Command(string s)
        {
            if (m_toRemote != null)
            {
                byte[] buf = System.Text.Encoding.Unicode.GetBytes(s);
                m_toRemote(buf);
            }
        }

        public void Show() { }

        // this object has two Dispose's to worry about.
        // Our "real" one is for the case WriteLog is getting rid of us.
        // This one, though, is shutting down the serial port...which we don't do.
        public new void Dispose() { }

        public bool IsDisposed { get { return false; } }
        #endregion

     }
}
