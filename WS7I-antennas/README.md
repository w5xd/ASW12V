Antenna switch control for station WS7I

This is a .NET application for controlling the antenna switching at WS7I.
It serves as an example of both how an ASW12V might be used,
and how to split a WriteLog keyboard shortcut processor between control-site
and remote-site.


The station has one antenna switch that attaches one of these to the rig
<ul>
<li>TH7
<li>7 MHz
<li>3.5 MHz
<li>2.0 MHz
</ul>




To use within WriteLog, place a copy of the WS7I_antennas_shortcuts.dll in WriteLog's Program directory,
and make a new entry in WriteLog.ini. WriteLog can then access the beverage switching from
keyboard shortcuts. The INI entry looks like this:
<pre><code>
[ExternalShortcuts]
WS7I_antennas_shortcuts.WriteLogKeyProcessor=WS7I_antennas_shortcuts</code>
</pre>
WriteLog doesn't need the .exe file. Only the .dll need be installed in WriteLog's Program folder. To get the antenna controller on the screen assign any (or all) of its four shortcuts to keyboard keys, then invoke one.


<h3>WriteLog Remote Control</h3>
WriteLog as of its version 12.36 supports keyboard shortcut processing split across
the control-site to the remote-site. This project shows how to accomplish that
in its .NET source code. The Writelog.ini setting at the remote site is as above, but at the control site,
you need this entry instead:
<br/><code>WS7I_antennas_shortcuts.WriteLogKeyProcessorControlSite=WS7I_antennas_shortcuts</code>
 
