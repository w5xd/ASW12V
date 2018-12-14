# ASW12V
Remote controllable multi-channel DC switch
ASW12V: antenna switch for 12V

My shack has a number of 12VDC controlled circuits for antennas. When
I am sitting at the operating position, they are
 controlled by rotary switches there. 
When connected to the station remotely using WriteLog, I need a way to 
control the switches from the shack PC. This device is a USB controlled
solid-state multi-channel DC switch that controls up to 30VDC
at up to 3A. In the absence of control commands over its USB input,
its output channels follow its input channels. 5VDC on the micro
USB plug is required to power the embedded microprocessor for it
follow its inputs.

The power and ground are optically isolated on both input and output side
in groups of four channels that all share a common power and ground.
All the input and output power and ground circuits are optically isolate
from the USB power and ground.

The enclosures presented here are in two different configurations. This one
has 24 channel control:
<p align='center'><img src='Picture24Channel.jpg' alt='Picture24Channel.jpg'/></p>

The other enclosure is half as wide and controls 12 channels.

