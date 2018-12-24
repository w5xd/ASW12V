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
solid state multi-channel DC switch.The output transistors are rated to 60VDC
at up to 3A. In the absence of control commands over its USB input,
its output channels follow its input channels. 5VDC on the micro
USB plug is required to power the embedded microprocessor for it to
follow its inputs.
</p>

<p>The power and ground are optically isolated on both input and output side
in groups of four channels. Each group of four has its power and ground.
All the input and output power and ground circuits are optically isolated
from the USB power and ground, which is only used by the controller.</p>
<p>
Here is the 24 channel version:</p>
<p align='center'><img height="50%" src='Picture24Channel.jpg' alt='Picture24Channel.jpg'/></p>
The longest dimension is about 8in (21cm). The short dimension is about 4in (10cm). The other 12 channel enclosure is half as wide (4" x 4" or 10cm x 10cm).

<p>Power and Ground</p>

<p>The PCB layout has three sections each with four input/output channels and with grounds and
VCC isolated to that section. The power and ground circuit is this:
<a href='ASW12V-circuit3.pdf'>ASW12V-circuit3.pdf</a>. 
There are a lot of options on the PCB for exactly what Ground and VCC connections get connected internal
to the device. 
 
You can successfully use this device with NONE of these jumpers in place. But consider how you want to use it.
In my own case, the same 12VDC supply powers all my antenna relays, so I can safely tie all the Gnd and VCC
terminals together. Your shack may differ.

The PCB has jumpers that can internally connect:</p>
<ol>
<li>the VCC pin to the VCC on the adjacent section.
<li>the OUTPUT GND pin to the OUTPUT Gnd pin of the adjacent section.
<li>the INPUT Gnd pin to the OUTPUT Gnd pin of the section.
<li>the INPUT GND pin to the INPUT Gnd pin of the adjacent section.
 </ol>
 The positions of the jumpers are hand-outlined on this <a href='ASW12V-bottom.pdf'>bottom view of the PCB silkscreen</a>. 
 The jumpers are most easily installed (and removed) on the bottom side of the PCB. The jumpers corresponding to (1) and (2) are at the top of the picture, the ones for (4), the INPUT to INPUT, are at the bottom and the ones for (3) are between.
 
 The 3A maximum current is for the OUTPUT terminal block. The circuit board traces are wide enough
 for a total of 3A to the VCC and GND connectors on that terminal block.
 
Construction

<p>A parts list and a PCB layout are published <a href='construction.md'>here</a>.</p>

Controlling the ASW12V

A powered USB connection to the microUSB connector is required. At boot up, the firmware operates the OUTPUT
side as a pass through&mdash;a signal on an input pin results in V+ applied to corresponding OUTPUT pin on
the opposite side of the box.
<p>When plugged in to a computer, the fdichip chip drivers for its USB port automatically are installed (on most
 operating systems) and create a virtual serial port. An example program to write useful serial commands is published <a href='W5XD-antennas'>here on github</a>. The example
is specific to the antennas to be controlled at W5XD. It doesn't put much on the screen. as not much is required at W5XD.</p>

<img src='w5xd-antennas-1.png' alt='w5xd-antennas-1.png'/>

Enclosures

<p>This repository publishes designs for enclosures for each of two configurations: a 12 channel device (with 1 PCB) or a 24 channel device (with 2 PCBs.) The 3D designs were done with <a href='solidworks.com'>SolidWorks</a>. The
Soldworks models are in the CAD folder. The 3D printable parts are published in the <a href="STL/">STL</a> directory. Unfamiliar with 3D design and printing? Don't be bashful about clicking the <a href="STL/">STL</a> link and the enclosure models. </p>

<p>The orientation of the STL models should not be taken as a recommendation for
the printed orientation on any particular 3D printer. I use a <a href='http://prusa3d.com'>prusa3d</a> printer and rotate the
lettering down for the enclosure tops, and, for the bottoms, the flat side down.</p>
 
