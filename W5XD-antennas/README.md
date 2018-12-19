Antenna switch control for station W5XD

This is a .NET application for controlling the antenna switching at W5XD.
My station has these items to control:
<ul>
<li>Four beverage antennas (receive only) that each can be switched into the RX antenna port of either of two rigs. The rigs are called "primary" and "secondary" because the relay scheme
only allows one rig on a beverage at a time, and the "primary" radio always wins.</li>
<li>A 12VDC supply for remote tuner (MFJ-998RT). That tuner can be commanded to find a better match by cycling its power off then back on.
</ul>

Also at W5XD, the operator tends to run WriteLog for his logging software. (Funny thing
about that.) Beverages really need to be switching from within the contest logging program
with just a keystroke. This is important when trying to pull a weak signal out of the noise.
For this particular function, its really not an effective computer/user interface to
change the keyboard focus outside the logging program and then back.

That beverage-switching problem is why this .NET application is structured the way it is.
When you open this Visual Studio solution, W5XD-antennas.sln, you build both a .exe and
a .NET assembly in a .dll. If you run the exe standalone you get control over the ASW12V,
but not from within WriteLog.

If you "install" the W5XD_antennas_shortcuts.dll in WriteLog's Program directory,
and make a new entry in WriteLog.ini, then WL can access the beverage switching from
keyboard shortcuts. The INI entry looks like this:
[ExternalShortcuts]
W5XD_antenna_shortcuts=W5XD_antennas_shortcuts.WriteLogKeyProcessor.
WriteLog doesn't need the .exe file. Only the .dll need be installed in WriteLog's Program folder.
