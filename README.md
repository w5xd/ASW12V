ASW12V

<p>
Remote controllable multi-channel DC switch<br/>
ASW12V: antenna switch for 12V
</p>
<p>
My shack has a number of 12VDC controlled circuits for antennas. When
I am sitting at the operating position, they are
 controlled by rotary switches there. 
When connected to the station remotely using WriteLog, I need a way to 
control the switches from the shack PC. This device is a USB controlled
solid-state multi-channel DC switch that controls up to 60VDC
at up to 3A. In the absence of control commands over its USB input,
its output channels follow its input channels. 5VDC on the micro
USB plug is required to power the embedded microprocessor for it
follow its inputs.
</p>

<p>The power and ground are optically isolated on both input and output side
in groups of four channels that all share a common power and ground.
All the input and output power and ground circuits are optically isolate
from the USB power and ground.</p>
<p>
The enclosures presented here are in two different configurations. This one
has 24 channel control:</p>
<p align='center'><img height="50%" src='Picture24Channel.jpg' alt='Picture24Channel.jpg'/></p>

The other enclosure is half as wide and controls 12 channels.

Power and Ground
The PCB layout has three sections each with four input/output channels and with grounds and
VCC isolated to that section. The power and ground circuit is this:
<a href='ASW12V-circuit3.pdf'>ASW12V-circuit3.pdf</a>. The PCB has jumpers that can optionally connect:
<ol>
<li>the VCC pin to the VCC on the adjacent section.
<li>the OUTPUT GND pin to the OUTPUT Gnd pin of the adjacent section.
<li>the INPUT Gnd pin to the OUTPUT Gnd pin of the section.
<li>the INPUT GND pin to the INPUT Gnd pin of the adjacent section.
 </ol>
 The positions of the jumpers are hand-outlined on this <a href='ASW12V-bottom.pdf'>bottom view of the PCB silkscreen</a>. 
 The jumpers are most easily installed (and removed) on the bottom side of the PCB. The jumpers corresponding to (1) and (2) are at the top of the picture, the ones for (4), the INPUT to INPUT, are at the bottom and the ones for (3) are between.
 
Construction
A parts list and a PCB layout are published <a href='construction.md'>here</a>.

Controlling the ASW12V
An example program is published <a href='W5XD-antennas'>here</a>. It is specific to the antennas to be controlled at W5XD. It doesn't put much on the screen. as not much is required at W5XD.

<img src='w5xd-antennas-1.png' alt='w5xd-antennas-1.png'/>
 
