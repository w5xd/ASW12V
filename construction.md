ASW12V Construction
<h3>Parts lists</h3>
The parts of a 12-channel, single-PCB device can all&mdash;except the PCB and enclosure&mdash; be obtained at digikey: <a href="partslist.pdf">partslist.pdf</a>.
Or here is a link to the equivalent digikey shared shopping cart:
<a target="_blank" href="https://www.digikey.com/short/z8p5mv">https://www.digikey.com/short/z8p5mv</a>.

<p>The digikey references above do <b>not</b> include the necessary input/output screw terminals or connectors. The PCB design accommodates 
either of these connector configurations:</p>
<ul>
<li><a target="_blank" href='https://www.digikey.com/short/z8zprc'>Phoenix Contact 1935200 </a>. (screw terminals)
<li><a target="_blank" href='https://www.digikey.com/short/z8zprj'>TE Connectivity AMP Connectors part 1744048-6</a>. (male pins on plug.)
This plug mates with these parts:
<ul>
<li><a target="_blank" href='https://www.digikey.com/short/z8zprb'>mating housing</a>
<li><a target="_blank" href='https://www.digikey.com/short/z8zprd'>crimp pins</a>
</ul>
</ul>

<p>The parts lists above also do <b>not</b> include the board-to-board connectors you'll need to interconnect the two PCBs if you build the 24 channel version:</p>
<ul>
<li><a target="_blank" href='https://www.digikey.com/short/z8prdt'>Right angle connection header, male</a>. </li>
<li><a target="_blank" href='https://www.digikey.com/short/z8pr0h'>Right angle connection header, female</a>. </li>
<li>Double all the quantities in the parts list above.</li>
</ul>

Separate enclosure STL files are published for each choice of connectors.

The printed circuit board is a four layer design. It is not commercially available.
It was designed with <a target="_blank" href="http://expresspcb.com">expresspcb</a> and the
file is ASW12V.rrb. Or you may use the gerber
files in the <a href="gerber">gerber</a> folder. 
<ol>
<li>This repository maintains only the most recent PCB revision's documentation in the master branch. Look for tags to find documentation of older revisions.</li>
<li>Revision 3 and later of the PCB is limited to 3A among its outputs.
Earlier revisions have lower current limits. Revision 3 and later of the PCB accommodates either of the above interconnects. </li>
<li>Revision 4 adds pads for SMD <code>1206</code> size resistors while the through-hole positions remain an option. 
	Install one or the other. </li>
	<li>Revision 4 and earlier accommodate the SMA4021 12-pin SI Darlington PNP with built-in flywheel diodes. </li>
<li>Revision 6 changes the layout to accommodate the pinout of the more readily available STA402 10-pin SIP PNP darlington, and 
adds 2 Amp SMD size <code>1206</code> flywheel diodes to accommodate inductive loads (which includes, for example, a relay coil!) The 
SMA4021 part for earlier PCBs has built-in flywheel diodes.
<li>Revision 5 is identical to Revision 6, but has a stray trace that must be cut! On the Right section output PS2502 optoisolator, its pin 11 on the top side of the board, has a stray trace connecting to the "far" side of the 3.3K resistor. There must be no connections on the top side on pin 11 of that optoisolator. Before installing any components, use an Xacto knife to remove that stray trace.
</ol>
 
Program an Arduino Pro Micro with the sketch published in <a href="sketch//ASW12V">sketch/ASW12V/</a>. Both the 5V and 3.3V 
versions of the Pro Micro work equally well. 
    <h3>Construction Recommendations</h3>
The PCB layout is very tight which does not make for easy maintenance. The
        tight clearances mean that once all the big parts
        are installed, it is essentially impossible to
        service the small ones. If, for example, lightning damages this
        board, do not expect to service it. Expect to throw it away
        and build another.
    
Install <i>only</i> these parts and stop:
<ol type="a">
    <li>If you have a reflow solder oven, use it! Its only effect on these
	    instructions is that you pick and place <i>all</i> surface mount parts in the first step. The SMD parts are U1-4, the 24 by 10K resistors, 24 by 3.3K resistors, 2 by .33uF capacitors, and the 12 diodes.</li>
   <li>
        Install the four surface mount shift registers at U1, U2, U3, and U4. To hand solder an SMT 16 pin IC, start by
        soldering two opposite corner pins to their pads. Check that pin 1 is where it belongs. Check alignment.
        Check alignment again. Then solder all 16 pads.    </li>
    <li>Install the two .33uF power supply bypass capacitors. The PCB has holes but the parts list
    specifies an <code>0805</code> sized SMT part. It can be hand soldered across the holed pads. Or substitute a through-hole part.</li>
     <li>Install the 12 SMT <code>1206</code> sized flywheel diodes. Observe the cathode marks! One of
	    each set of four is backwards from the others!</li>
    <li>Install a male header at J1. No header is needed for the first board (the one with the Arduino plugged in) 
    and a right angle header for all slave channels.</li>
</ol>
Also install male headers on your Pro Micro and then use
<ol type="a">
    <li> 7 jumpers to temporarily wire it to the PCB. I used these jumpers from <a href='http://www.sparkfun.com/products/10898'>sparkfun</a>.</li>
    <li>If this test is for a board that will have the Arduino, and you have no header as in (e) above, then it is highly recommended
    that you install a sacrificial header now and defer installing the female headers for the Arduino (otherwise, if there is a problem with the SMD parts, you'll have to work around those headers.) A vertical header (straight pins) will work, but if you don't know where you're going to
    install this board, then install a right angle header. If it ever is to put in an enclosure as the first board,
    you'll have to cut all the pins off the header for mechanical clearance. But by then they'll not be needed 
    because you'll have the female headers installed for the Arduino.</li>
    <li>
        jumper the I pin to the O pin at J2. A wire is used for the last board in the chain. A right angle female header
	is used for all others. In either case, for initial test, install this jumper. If there is no header, you may twist a bare
    wire between the two pins to avoid soldering.
    </li>
    <li>The jumper list from J1 to the Arduino Pro Micro is:
    <ol type='1'> 
        <li>LO to pin D9</li>
        <li>G to either pin GND</li>
        <li>vcc to VCC</li>
        <li>O to D11</li>
        <li>I to D16</li>
        <li>Li to D10</li>
        <li>C to D15</li>
    </ol>
    </li>
</ol>
Your board might look like this, minus the resistors I have already placed:
<img src='TestConfiguration.jpg' alt='TestConfiguration.jpg' />
The board in this photo is destined to be board number 1 (it has a vertical header installed at J1 on the right.)
And it is planned to go in a more-than-12 channel enclosure, so it has a right angle connector installed at J2 on the left.
<h3>Shift Register Digital Test</h3>
You need a serial port terminal program connected to
the Arduino&#8217;s serial port for the following tests. I recommend
<a href="https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html">putty.exe</a>.
Click its "Serial" button in its upper right to put it in serial port terminal mode. You will also need a DVM to accomplish these tests.
<ol>
    <li>
        Test that the
        <code>d</code> command correctly tells you there is one board in the daisy chain.
    </li>
    <li>
        Test the
        <code>n</code> command. Ignore the results from Sent 1, 2 and 3
        but confirm that <code>Sent 4 Got 0</code>, <code>Sent 5 Got 1</code> and counting up
        from there.
    </li>
</ol>
<p>Do <b><i>not</i></b> install any more parts until the above two tests succeed. Only the four ICs are involved in these tests.</p>

<h3>Digital Input Test</h3>
Install only the 3 banks of 10K resistors on the input
shift registers. Now your board will match the photo above.
They are labeled 1, 2, 3, and 4 on each
of the 3 banks. These are the ones just above the input
optoisolators. Install the ODD numbered ones first, solder
and trim the leads. Then install the EVEN numbered ones.
The circuit board clearances are minimal and installing
in this order makes it easier to avoid solder bridges.  
Type the <code>i</code> command while using a test lead to, one at a time,
short these adjacent pins on each of the <i>input</i> optoisolators (also one at a time):
<ul>
    <li>16 to 15 (prints out a "1" in the corresponding <code>i</code> command)</li>
    <li>14 to 13 (prints out a "2"...)</li>
    <li>12 to 11 (prints out a "4"...)</li>
    <li>10 to 9 (prints out "8").</li>
</ul>
There are 12 input signals to verify. Do <b><i>not</i></b> install any more parts until this test succeeds.
    
<h3>Digital Output Test</h3>
   
Install the 3 banks of 3.3K resistors on the output
shift registers. They are labeled 1,2,3, and 4, and are
the ones just below the output optoisolators. Verify that both ends of each 
has NO contininuity to its GND output pin (labeled
on the bottom silk screen.) Note that each of the 3 sections has a separate GND pin, and 
you must test each resistor against it own section's GND pin. 

Use the <code>C</code> command to clear the internal test state, and then repeatedly
use the <code>T</code> command to assert each channel in the daisy change, right most to left
with the board top-up and the silk screen lettering right-side-up.
The HIGH output is Vcc for the Pro Micro you are using:
either 3.3V or 5.0 V.

Populate the parts on the rest of the board <b><i>only</i></b> after you verify each of the outputs.
After installing the three output PS2502 parts (the three closest to the output edge) go back and
<i>again</i> verify NO continuity from every PS2502 output in (pins 9 through 16) to the GND output pin. Solder bridges can and will form
on the opposite side of the board that you solder the PS2502 pins from!

<h3>12V output section test</h3>
This author strongly recommends testing the 12V output section using a current limiting circuit as shown 
by the 220 ohm series resistor
in the <a href='ASW12V-circuit3.pdf'>circuit diagram page 3</a>.
I have twice had to remove an already soldered-in PS2502 to deal with
a permanent failure caused by applying full 12VDC to an output section that had
a solder bridge involving one of the 3.3K ohm resistors between the PS2502 and the PNP darlington.
In one of those cases, the solder bridge formed on the top side of the board when I 
applied too much solder to the pin from the bottom!
<p>With the test circuit wired in as shown on the circuit diagram, 
use a terminal emulator connected to the Pro Micro's serial port
to alternately send these two commands, which, on a properly wired
output section, give, respectively, 0V and VCC on the corresponding output pin.</p>
<ul>
<li><code>m 1 L0F</code>
<li><code>m 1 LFF</code>
</ul>
<p>For the <code>L</code>, substitute <code>M</code> when testing the middle section and <code>R</code> when testing the right section.</p>

<p>The GND and VCC jumpers on the PCB, if needed, are most easily installed on the <a href='ASW12V-bottom.pdf'>bottom</a>. There is a discussion about why you would or would not install the jumpers in the main <a href='README.md'>README</a>.</p>

<h3>Enclosure</h3>

The enclosure is documented in the <a href="CAD/">CAD</a> and <a href='STL/'>STL</a> folders.
The mounting posts on the enclosure top have channels that fit #4 machine nuts. Square nuts are easier to
place, but hex nuts work as well. In either case, clean out the 3D printing debris before
pushing a nut in the channel.  And put a small piece of tape over the opening after the nut
is placed because gravity is going to try to remove it while you do the remainder of the assembly.  

Quantity 4 by 5/8 inch machine screws are needed per section to fasten the top to the bottom.

The mounting screw holes in the enclosure bottom can be cleared of print debris by pushing a
#4 machine screw through from the inside toward the outside.
