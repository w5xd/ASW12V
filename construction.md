ASW12V Construction
<h3>Parts lists</h3>
The parts of the device can all&mdash;except the PCB and enclosure&mdash; be obtained at digikey: <a href="partslist.pdf">partslist.pdf</a>.
Or here is a link to the equivalent digikey shared shopping cart:
<a href="https://www.digikey.com/short/p9f2hq">https://www.digikey.com/short/p9f2hq</a>.

<p>The digikey references above do <b>not</b> include the necessary screw terminals or connectors. The PCB design accommodates 
either of these:</p>
<ul>
<li><a href='https://www.digikey.com/product-detail/en/1935200/277-1581-ND/568618/?itemSeq=277063756'>Phoenix Contact 1935200 </a>
<li><a href='https://www.digikey.com/product-detail/en/te-connectivity-amp-connectors/1744048-6/A116470-ND/4730099'>TE Connectivity AMP Connectors part 1744048-6</a>
which mates with these parts:
<ul>
<li><a href='https://www.digikey.com/product-detail/en/te-connectivity-amp-connectors/2-1744036-6/A144553-ND/5438998'>mating housing</a>
<li><a href='https://www.digikey.com/product-detail/en/te-connectivity-amp-connectors/1123721-2/A100446CT-ND/2233212'>crimp pins</a>
</ul>
</ul>
Separate enclosure STL files are published depending on your choice of part.

The printed circuit board is a four layer design. It is not commercially available.
It was designed with <a href="http://expresspcb.com">expresspcb</a> from the
file ASW12V.rrb. Or you may use the gerber
files in the <a href="gerber">gerber</a> folder. Revision 3 of the PCB is limited to 3A among its outputs.
Earlier revisions have lower limits. The same Revision 3 PCB accommodates either of the above interconnects.
 
 Program an Arduino Pro Micro with the sketch published in <a href="sketch//ASW12V">sketch/ASW12V/</a>. Both the 5V and 3.3V versions of the Pro Micro work equally well. 
    <h3>Construction Recommendations</h3>
The PCB layout is tight which does not make for easy maintenance. The
        very tight clearances mean that once all the big parts
        are installed, it is essentially impossible to
        service the small ones. If, for example, lightning damages this
        board, do not expect to service it. Expect to throw it away
        and build another.
    
Install <i>only</i> these parts and stop:
<ol type="a">
    <li>
        the four surface mount shift registers at U1, U2, U3, and U4. To solder an SMT 16 pin IC, start by
        soldering two opposite corner pins to their pads. Check that pin 1 is where it belongs. Check alignment.
        Check alignment again. Then solder all 16 pads.
    </li>
    <li>the two .33uF power supply bypass capacitors. The PCB has holes but the parts list
    specifies an <code>0805</code> sized SMT part. It can be soldered across the holed pads. Or substitute a through-hole part.</li>
    <li> a male header at J1. Mechanical clearances require a vertical header at this position for the first (or only) 12 channels,
         and a right angle header for all slave channels.</li>
</ol>
Also install male headers on your Pro Micro and then
use
<ol type="a">
    <li> 7 jumpers to temporarily wire it to the PCB. I used these from <a href='http://www.sparkfun.com/products/10898'>sparkfun</a>.</li>
    <li>
        jumper the I to O pins at J2. A wire may be used for the last board in the chain, but a right angle female header
	should be installed for all others, and a jumper installed.
    </li>
</ol>
Your board might look like this, minus the resistors I have already placed:
<img src='TestConfiguration.jpg' alt='TestConfiguration.jpg' />
The board in this photo is destined to be board number 1 (it has a vertical header installed at J1 on the right)
in an enclosure of two boards (it has a right angle connector installed at J2 on the left.)
<h3>Shift Register Digital Test</h3>
You need a serial port terminal program connected to
the Arduino&#8217;s serial port for the following tests. I recommend
<a href="https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html">putty.exe</a>.
Click its "Serial" button in its upper right to put it in serial port terminal mode.
<ol>
    <li>
        Test that the
        <code>d</code> command correctly tells you there  is one board in the daisy chain.
    </li>
    <li>
        Test the
        <code>n</code> command. Ignore the results from Sent 1, 2 and 3
        but confirm that <code>Sent 4 Got 0</code>, <code>Sent 5 Got 1</code> and counting up
        from there.
    </li>
</ol>
<p>Do <b><i>not</i></b> install any more parts until the above two tests succeed.</p>

<h3>Digital Input Test</h3>
Install only the 3 banks of 10K resistors on the input
shift registers. Now your board will match the photo above.
They are labeled 1, 2, 3, and 4 on each
of the 3 banks. These are the ones just above the input
optoisolators. Install the ODD numbered ones first, solder
and trim the leads. Then install the EVEN numbered ones.
The circuit board clearances are minimal and installing
in this order makes it easier to avoid solder bridges.
    
Type the <code>i</code> command while using a test lead to
short these adjacent pins of the input optoisolators:
<ul>
    <li>16 to 15 (gives a "1" in the corresponding <code>i</code> command)</li>
    <li>14 to 13 (gives a "2"...)</li>
    <li>12 to 11 (gives a "4"...)</li>
    <li>10 to 9 (gives "8").</li>
</ul>
There are 12 input signals to verify.
Do <b><i>not</i></b> install any more parts until this test works.
    
<h3>Digital Output Test</h3>
   
Install the 3 banks of 3.3K resistors on the output
shift registers. They are labeled 1,2,3, and 4, and are
the ones just below the output optoisolators.

Use the <code>n</code> command to make the outputs count up.
The fastest bank is the right. The slowest bank
is left. The fastest pin is 1. The slowest pin is 4
The HIGH output is Vcc for the Pro Micro you are using:
either 3.3V or 5.0 V.

Now you can populate the parts on the rest of the board.

<h3>12V output section test</h3>
This author strongly recommends testing the 12V output section using a current limiting circuit as shown 
by the 220 ohm series resistor
in the <a href='ASW12V-circuit3.pdf'>circuit diagram page 3</a>.
I have twice had to remove an already soldered-in PS2502 in its DIP16 to deal with
a permanent failure caused by applying full 12VDC to an output section that had
a solder bridge involving one of the 3.3K ohm resistors between the PS2502 and the PNP darlington.
In one of those cases, the solder bridge formed on the lower side of the board when I 
applied too much solder to the pin from the top!
<p>With the test circuit wired in as shown on the circuit diagram, 
use a terminal emulator connected to the Pro Micro's serial port
to alternately send these two commands, which, on a properly wired
output section, give, respectively, 0V and 12V on the output pin.</p>
<ul>
<li><code>m 1 L0F</code>
<li><code>m 1 LFF</code>
</ul>
<p>For the <code>L</code>, substitute <code>M</code> when testing the middle section and <code>R</code> when testing the right section.</p>

<p>The GND and VCC jumpers on the PCB, if needed, are most easily installed from the <a href='ASW12V-bottom.pdf'>bottom</a>.</p>

<h3>Enclosure</h3>

The enclosure is documented in the <a href="CAD/">CAD</a> and <a href='STL/'>STL</a> folders.
The mounting posts on the enclosure top have channels that fit #4 machine nuts. Square nuts are easier to
place, but hex nuts work as well. In either case, clean out the 3D printing debris before
pushing a nut in the channel.  And put a small piece of tape over the opening after the nut
is placed because gravity is going to try to remove it.  

Quantity 4 by 5/8 inch machine screws are needed per section to fasten the top to the bottom.

The mounting screw holes in the enclosure bottom can be cleared of print debris by pushing a
#4 machine screw through from the inside toward the outside.
