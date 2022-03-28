ASW12V

<p>
Remote controllable multi-channel DC switch<br/>
ASW12V: antenna switch for 12V
</p>

My shack has a number of 12VDC controlled circuits for antennas. When
I am sitting at the operating position, they are
 controlled by rotary switches there. 
When connected to the station remotely using WriteLog, I need a way to 
control the switches from the shack PC. This device is a USB controlled
solid state multi-channel DC switch. While its named "12V" it can
safely switch up to about 40V AC or DC, and at up to about 3A. The remainder of
this document uses "12V" to describe the power supply, even though the PCB
is more capable.

In the absence of 
control commands over its USB input,
its output channels follow its input channels. 5VDC on the micro
USB plug is required to power the embedded microprocessor for it to
follow its inputs.

<p>This device is controlled by ASCII command strings sent over
its USB virtual serial port. WriteLog is not required to operate it,
although a WriteLog plugin is posted <a href='W5XD-antennas'>here</a> showing 
how to embed
antenna switching commands destined for this hardware
within WriteLog. The example has on-screen controls corresponding
to the antennas at W5XD. Full source code is posted.</p>

<p>The power and ground are optically isolated on both input and output side
in groups of four channels. Each group of four has its own power and ground.
All the input and output power and ground circuits are optically isolated
from the USB power and ground, which is isolated to the controller.</p>
<p>
Here is the 24 channel version built with screw terminals:</p>
<p align='center'><img height="50%" width='50%' src='Picture24Channel.jpg' alt='Picture24Channel.jpg'/></p>
The longest dimension is about 8in (21cm). The short dimension is about 4in (10cm). The other 
12 channel enclosure is half as wide (4" x 4" or 10cm x 10cm). 
Here is an assembled PCB using connectors instead of screw terminals.
<p align='center'><img src='PCBassembledConnectors.jpg' alt='PCBassembledConnectors.jpg'/></p>
<br/>
<p>Power and Ground</p>

<p>The PCB layout has three sections each with four input/output channels and with grounds and
VCC isolated to that section. In the photo above, all the G and 12V connections are, by default,
 isolated from each other. The PCB has solder jumpers to enable interconnects among them. See below.
 The power and ground circuit is this:
<a href='ASW12V-circuit3.pdf'>ASW12V-circuit3.pdf</a>. 
Many different power supply options are supported by jumper options on the PCB for exactly what Ground and 
VCC connections are interconnected on
the PCB. One thing that is not optional: fuse at about 3A or otherwise current limit the connection from the
 power supply to the Vcc terminal on the ASW12V. The ASW12V PCB has components too dense to service. Plan on
 replacing it if it is damaged by over current.
 
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
 The jumpers are most easily installed (and removed) on the bottom side of the PCB. The jumpers corresponding to (1) and (2) are at the top of the picture, 
 the ones for (4), the INPUT to INPUT, are at the bottom and the ones for (3) are between.
 
 The circuit board traces are wide enough
 to suppoart a total of 3A to the VCC and GND connectors on each terminal block.
 
Construction

<p>A parts list, construction details, and a PCB layout are published <a href='construction.md'>here</a>.</p>

Controlling the ASW12V

A USB connection to the USB-C connector is required for operation of the microcontroller such that it
operates in a pass through mode in the absence of any incoming USB commands. That is, the firmware transfers the INPUT
side as a pass through to the OUTPUT&mdash;a signal on an INPUT pin results in Vcc applied to corresponding OUTPUT pin on
the opposite side of the box.
<p>When plugged in to a computer, the drivers for its USB port automatically are installed (on most
 operating systems) and create a virtual serial port. An example program to write useful serial commands is published <a href='W5XD-antennas'>here on github</a>. The example
is specific to the antennas to be controlled at W5XD. It doesn't put much on the screen. as not much is required at W5XD.</p>
<p>Control the beverages, one to the RX antenna on each of two radios:</p>
<img src='w5xd-antennas-1.png' alt='w5xd-antennas-1.png'/>
<p>Control the main transmit antenna, one of 6 to each of two radios:</p>
<img src='w5xd-antennas-RatPak.png' alt='w5xd-antennas-RatPak.png'/>
<p>All the buttons on the two windows above can be programmed as keyboard shortcuts in WriteLog.</p>
Enclosures

<p>This repository publishes designs for enclosures for each of these variations: a 12 channel device (with 1 PCB) or a 24 channel 
device (with 2 PCBs) and for either screw binding posts or connectors on the PCB. The 3D designs were done with <a href='solidworks.com'>SolidWorks</a>. 
The PCB hole pattern accommodates either the screw terminals or the connector. The
Soldworks models are in the CAD folder. The 3D printable parts are published in the <a href="STL/">STL</a> directory. Unfamiliar with 3D design 
and printing? Don't be bashful about clicking the <a href="STL/">STL</a> link and the enclosure models. </p>

<p>The orientation of the STL models should not be taken as a recommendation for
the printed orientation on any particular 3D printer. I use a <a href='http://prusa3d.com'>prusa3d</a> printer and rotate the
lettering down for the enclosure tops, and, for the bottoms, the flat side down.</p>
 
