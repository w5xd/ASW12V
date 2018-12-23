using System;
/* The split between control-site and remote-site is 
 * done at the serial port message level. That is,
 * control sends the remote the serial commands that it,
 * in turn, writes to the serial port. 
 *
 * because at the remote site, the serial port is
 * managed in an on-screen Windows Form, there are
 * also methods that look like Form methods so
 * that the control site just does nothing on them.  */
namespace W5XD_antennas
{
    // When split 
    public interface RemotableSerial
    {
        void Command(String s);
        void Show();
        void Dispose();
        bool IsDisposed { get; }
    }
}
