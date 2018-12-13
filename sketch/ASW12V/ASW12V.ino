#include <SPI.h>

#define DEBUG_DEFAULT_MODE 0

/* Arduino sketch for the ASW12V remote control switch box
** The hardware is an array of input screw terminals and 
** an array of output screw terminals of the same size.
**
** The architecture is to be able to insert this device
** in a 12V DC switching circuit such that 
** the manual switch at our inputs controls the 
** devices at our outputs by asserting 12V DC on 
** those outputs in various combinations. The circuit 
** isolates its DC supply and ground from our Arduino
** power, which is on USB power. The SMA4021 PNP output
** devices can switch up 60V DC,
** have built-in flyback diodes for inductive loads. The
** Circuit board traces are the current handling limit,
** which is about 1.5A.
**
** Starting up, we assume we're operating stand alone, which
** means copying our inputs (circuit diagram U3 and U4)
** to our outputs (U1 and U2). We are U5 on the circuit diagram.
** We only use the 3 SPI pins, two separate strobe
** pins to load the input registers (U3, U4) or assert the
** the outputs (U1 and U2), and VCC and ground. The board
** performs essentially the same with either the 3.3V
** Pro Micro or the 5.0V version.
**
** The board is arranged for daisy chaining. That means 
** we might have another U1/U2 and U3/U4 on a second circuit
** board cabled to our J2 and their J1. The daisy chaining
** can continue. This firmware needs to know how many boards
** are out there on the daisy chain. We do that by insisting:
** a) that the final board on the daisy chain (even if
** we're the only board) has its I/O pins jumpered 
** together. That allows us to shift data out and detect
** when it comes back.
** b) in order to put a limit on how much work we must do
** to detect the length of the daisy chain, that
** number is limited to 8 boards.
**
** Once we detect the length of the daisy chain, 
** our loop() copies the input registers to
** the output registers. 
**
** loop() also monitors the serial input for commands.
** One set of commands is designed for self test.
** The self test commands are CRITICAL. Do NOT ignore
** them. They MUST be used during board construction.
** The PCB is NOT designed for easy maintenance. It has
** very tight clearances and, once all the big parts
** are installed, it is essentially impossible to 
** service the small once. If lightning damages this
** board, do not expect to service it. Throw it away
** and build another. 
**
** Construction self tests.
** Install ONLY 
**   (a) the four surface mount shift registers
**          on the board along with  
**   (b) the two .33uF power supply bypass capacitors and 
**   (c) a male header at J1. 
**
** Also install male headers on your Pro Micro and then
** use 
** (a) 7 jumpers to temporarily wire it to the PCB
** (b) jumper the I to O pins at J2. I use a solder wire
** for the I to O, which can later be cut in the middle
** to jumper to the next board, if there is one.
**
** SHIFT REGISTER DIGITAL TEST
** 1. Test that the "d" command correctly tells you there is
** is one board in the daisy chain.
** 2. Test the "n" command. Ignore the results from Sent 1, 2 and 3
** but confirm that Sent 4 Got 0, Sent 5 Got 1 and counting up 
** from there.
** 
** Do NOT install any more parts until the above two tests work.
**
** DIGITAL INPUT TEST
** Now install ONLY the 3 banks of 10K resistors on the input
** shift registers. They are labeled 1, 2, 3, and 4 on the each
** of the 3 banks. These are the ones just above the input
** optoisolators. Install the ODD numbered ones first, solder
** and trim the leads. Then install the EVEN numbered ones.
** The circuit board clearances are minimal and installing
** in this order makes it easier to avoid solder bridges.
**
** Now type the "i" command while using a test lead to
** short these pins of the input optoisolators:
**  16 to 15 (gives a "1" in the corresponding "i" command)
**  14 to 13 (gives a "2"...)
**  12 to 11 (gives a "4"...)
**  10 to 9 (gives "8").
** There are 12 input signals to verify.
** Do NOT install any more parts until this test works.
** 
** DIGITAL OUTPUT TEST
** Install the 3 banks of 3.3K resistors on the output
** shift registers. They are labeled 1,2,3, and 4, and are
** the ones just below the output optoisolators.
**
** Use the "n" command to make the outputs count up.
** The fastest bank is the right. The slowest bank
** is left. The fasted pin is 1. The slowest pin is 4
** The HIGH output is Vcc for the Pro Micro you are using:
** either 3.3V or 5.0 V.
*/

struct ShiftRegister_t {
    uint8_t LeftRegister; // high 4 bits are left register.
    uint8_t RightAndMiddleRegister; // low 4 are Middle, high 4 are Right
    ShiftRegister_t() : LeftRegister(0), RightAndMiddleRegister(0){}
    void init() { LeftRegister= RightAndMiddleRegister = 0;}
    void setLeftBits(uint8_t v, uint8_t mask) // both range of 0 to f 
    {   
        mask &= 0xf; // only 4 bits allowed
        LeftRegister &= ~mask;
        LeftRegister |= v & mask;
    }
    void setMiddleBits(uint8_t v, uint8_t mask)
    {
        mask &= 0xf;
        RightAndMiddleRegister &= ~mask;
        RightAndMiddleRegister |= v & mask;
    }

    void setRightBits(uint8_t v, uint8_t mask)
    {
        mask <<= 4;
        mask &= 0xf0;
        RightAndMiddleRegister &= ~mask;
        RightAndMiddleRegister |= (v<<4) & mask;
    }
};

namespace {
    const int INPUT_PORT_STROBE_PIN = 10;
    const int OUTPUT_PORT_STROBE_PIN = 9;
    const int MAX_BOARDS_DAISY_CHAINED = 8;

    // here are the SPI parameters for the 74hc597 AND 74HC594
    const SPISettings SPISetup(1000000, MSBFIRST, SPI_MODE0);

    ShiftRegister_t InputRegisters[MAX_BOARDS_DAISY_CHAINED];
    ShiftRegister_t MaskForceOn[MAX_BOARDS_DAISY_CHAINED];
    ShiftRegister_t MaskForceOff[MAX_BOARDS_DAISY_CHAINED];

    int DaisyChainLength = -1; // zero or negative: don't know

    enum OperatingMode_t {
            MODE_DEFAULT, // Copy inputs to outputs
            MODE_OPERATE, // Copy inputs to outputs, modified by masks from Serial
            MODE_MANUAL, // Outputs manipulated directly from Serial
    } OperatingMode = MODE_DEFAULT;

    void LatchInputs()
    {   // latch the opto-isolated pullup data into the input
        // registers.
        digitalWrite(INPUT_PORT_STROBE_PIN, LOW);
        digitalWrite(INPUT_PORT_STROBE_PIN, HIGH);
        // TWO cycles are needed because both the PL and STCP
        // leads must be clocked
        digitalWrite(INPUT_PORT_STROBE_PIN, LOW);
        digitalWrite(INPUT_PORT_STROBE_PIN, HIGH);
    }

    void LatchOutputs()
    {
        digitalWrite(OUTPUT_PORT_STROBE_PIN, LOW);
        digitalWrite(OUTPUT_PORT_STROBE_PIN, HIGH);
    }

    int FindDaisyChainLength()
    {
        // prime the shift register with all zeros
        SPI.beginTransaction(SPISetup);
        for (int i = 0; i < MAX_BOARDS_DAISY_CHAINED; i++)
        {   // each board has two bytes in each direction
            SPI.transfer(0);
            SPI.transfer(0);
        }

        static const uint8_t MARKER1 = 1;
        static const uint8_t MARKER2 = 2;

        SPI.transfer(MARKER1); //put a marker into the pipeline
        SPI.transfer(MARKER2); // it's two bytes long

        int ret = 1;
        for (; ret < MAX_BOARDS_DAISY_CHAINED; ret++)
        {
            SPI.transfer(0);
            SPI.transfer(0);
            uint8_t b1 = SPI.transfer(0);
            uint8_t b2 = SPI.transfer(0);
            if ((b1 == MARKER1) && (b2 == MARKER2))
                break;
        }

        SPI.endTransaction();
        if (ret >= MAX_BOARDS_DAISY_CHAINED)
            return -1;
        return ret;
    }

    bool printOutOnce;

    void DoCopyMode(bool HonorMasks)
    {
        if (DaisyChainLength <= 0)
            return;
#if DEBUG_DEFAULT_MODE
        static long lastTime;
        long now = millis();
        if (now - lastTime < 1000)
            return;
        lastTime = now;
#endif
        LatchInputs(); 
        SPI.beginTransaction(SPISetup);
        int i = 0;
        // read the inputs
        for (; i < DaisyChainLength; i += 1)
        {   // ~ input opto's invert
            int j = i;
            InputRegisters[i].RightAndMiddleRegister = ~SPI.transfer(0);
            InputRegisters[i++].LeftRegister = ~SPI.transfer(0);
#if DEBUG_DEFAULT_MODE
            Serial.print("read R&M: ");
            Serial.print((int)InputRegisters[j].RightAndMiddleRegister, HEX);
            Serial.print(" Left: ");
            Serial.println((int)InputRegisters[j].LeftRegister, HEX);
#endif
        }
        i -= 1;
        // write them out. Order reversed per their wiring.
        if (printOutOnce)
            Serial.print("Out: ");
        bool firstTime = true;
        for (; i > 0; i -= 1)
        {   // outputs do not invert
            uint8_t LeftRegister = InputRegisters[--i].LeftRegister;
            uint8_t RightAndMiddleRegister = InputRegisters[i].RightAndMiddleRegister;
            if (HonorMasks)
            {
                LeftRegister |= MaskForceOn[i].LeftRegister;
                RightAndMiddleRegister |= MaskForceOn[i].RightAndMiddleRegister;
                LeftRegister &= ~MaskForceOff[i].LeftRegister;
                RightAndMiddleRegister &= ~MaskForceOff[i].RightAndMiddleRegister;
            }
#if DEBUG_DEFAULT_MODE
            Serial.print("writing Left: ");
            Serial.print((int)LeftRegister, HEX);
            Serial.print(" r&m: ");
            Serial.println((int)RightAndMiddleRegister);
#endif
            SPI.transfer(LeftRegister);
            SPI.transfer(RightAndMiddleRegister);
            if (printOutOnce)
            {
                if (firstTime)
                    Serial.print(" ");
                Serial.print((int)RightAndMiddleRegister, HEX);
                Serial.print(" ");
                Serial.print((int) LeftRegister, HEX);
            }
            firstTime = false;
        }
        if (printOutOnce)
            Serial.println();
        printOutOnce = false;
        SPI.endTransaction();
        LatchOutputs();
    }
}

void setup()
{
    Serial.begin(9600); // control at 9600 baud, ascii

    // I/O pin setup--strobe pins for INPUT registers and OUTPUT registers
    pinMode(INPUT_PORT_STROBE_PIN, OUTPUT);
    pinMode(OUTPUT_PORT_STROBE_PIN, OUTPUT);
    digitalWrite(INPUT_PORT_STROBE_PIN, HIGH);
    digitalWrite(OUTPUT_PORT_STROBE_PIN, HIGH);
    SPI.begin();

    DaisyChainLength = FindDaisyChainLength();
}

int fromHex(int incoming)
{
    uint8_t mask = 0;
    bool ok = false;
    if (isdigit(incoming))
    {
        mask = (char)(incoming) - '0';
        ok = true;
    }
    else if ((char)incoming >= 'a' && (char)incoming <= 'f')
    {
        mask = 10 + (char)incoming - 'a';
        ok = true;
    }
    else if ((char)incoming >= 'A' && (char)incoming <= 'F')
    {
        mask = 10 + (char)incoming - 'A';
        ok = true;
    }
    if (ok)
        return mask;
    return -1;
}

void loop()
{
    static enum {PARSE_DEFAULT, PARSE_MASK} ParseState = PARSE_DEFAULT;
    if (Serial.available() > 0)
    {
        int incoming = Serial.read();
        if (ParseState == PARSE_DEFAULT)
        {
            // commands are a single character
            if ((char)incoming == (char)'\r')
                Serial.println("AWS 12V controller"); // tell user we're here...
            else if ((char)incoming == (char)'d')
            {   // print daisy chain length
                Serial.print("Daisy chain length = ");
                Serial.println(DaisyChainLength);
                OperatingMode = MODE_DEFAULT;
            }
            else if ((char)incoming == (char)'c')
            {   // clear all control masks
                for (int i = 0; i < MAX_BOARDS_DAISY_CHAINED; i++)
                {
                    MaskForceOn[i].init();
                    MaskForceOff[i].init();
                }
                Serial.println("All control masks cleared");
                OperatingMode = MODE_DEFAULT;
            }
            else if ((char)incoming == (char)'n')
            {   // shift a known sequence into the outputs
                static unsigned char toWrite;
                OperatingMode = MODE_MANUAL;

                SPI.beginTransaction(SPISetup);
                unsigned char received = SPI.transfer(++toWrite);
                SPI.endTransaction();

                Serial.print("Sent ");
                Serial.print((int)toWrite, HEX);
                Serial.print(" Got ");
                Serial.println((int)received, HEX);
                LatchOutputs();
            }
            else if ((char)incoming == (char)'i')
            {   // print current inputs
                LatchInputs();
                Serial.print("Read: ");
                SPI.beginTransaction(SPISetup);
                for (int i = 0; i < DaisyChainLength; i++)
                { // ~ opto pullups invert
                    unsigned char received1 = ~SPI.transfer(0);
                    unsigned char received2 = ~SPI.transfer(0);
                    if (i != 0)
                        Serial.print(" ");
                    Serial.print((int)received1, HEX);
                    Serial.print(" ");
                    Serial.print((int) received2, HEX);
                }
                SPI.endTransaction();
                Serial.println();
            }
            else if ((char)incoming == (char)'o')
            {   // print current outputs
                printOutOnce = true;
            }
            else if ((char) incoming == (char)'m')
            {
                ParseState = PARSE_MASK;
                OperatingMode = MODE_OPERATE;
            }
        } else if (ParseState == PARSE_MASK)
        {
            // Mask command syntax:
            // m <board-number> <L|M|R><value><mask> [repeat]
            //   The <L|M|R><value><mask> can be repeated multiple times on the same line                
            // where:
            // m is the character m
            // <board-number> is a single digit signifying which daisy-chained board in the range 1-9
            // <L|M|R> is one of the characters L,M or R
            // value is a single hex digit 0-9 a-f or A-F
            // mask is a single hex digit 0-9 a-f or A-F
            // The spaces after m and n are required. There are no other spaces
            static bool MaskInitialized;
            static int WhichBoard=-1;
            static ShiftRegister_t MaskOn;
            static ShiftRegister_t MaskOff;
            static const char *ErrorMessage;

            static uint8_t cmdValue;
            static enum {MASK_IS_L, MASK_IS_M, MASK_IS_R} MaskOnWhat;

            static enum {MASK_PARSE_INIT, MASK_PARSE_N, 
                MASK_PARSE_LMR_W, 
                MASK_PARSE_LMR, MASK_PARSE_V, MASK_PARSE_MASK,
                MASK_PARSE_BAD} MaskParseState;

            if (!MaskInitialized)
            {
                MaskInitialized = true;
                WhichBoard = -1;
                MaskOn.init();
                MaskOff.init();
                MaskParseState = MASK_PARSE_INIT;
                ErrorMessage = 0;
            }

            if ((char)incoming == (char)'\r')
            {
                // The m command...
                // ... sets up runtime masks to force a given output either on or off
                // regardless of input. By default, each output is driven by its 
                // corresponding input.
                static const char usage[] = "usage: m <board-number> <L|M|R><value><mask> [repeat]";
                if (MaskParseState == MASK_PARSE_BAD)
                {
                    if (!ErrorMessage)
                        ErrorMessage = "Invalid mask input";
                    Serial.println(ErrorMessage);
                    Serial.println(usage);
                }
                else if (WhichBoard >= 0)
                {
                    MaskForceOn[WhichBoard] = MaskOn;
                    MaskForceOff[WhichBoard] = MaskOff;
                    Serial.print("Setting board ");
                    Serial.print(WhichBoard+1);
                    Serial.print(" ON ");
                    Serial.print((int)MaskOn.LeftRegister, HEX);
                    Serial.print(" ");
                    Serial.print((int)MaskOn.RightAndMiddleRegister, HEX);
                    Serial.print(" OFF ");
                    Serial.print((int)MaskOff.LeftRegister, HEX);
                    Serial.print(" ");
                    Serial.println((int)MaskOff.RightAndMiddleRegister, HEX);
                }
                else
                    Serial.println(usage);
                MaskInitialized = false;
                ParseState = PARSE_DEFAULT;
            }
            else switch (MaskParseState)
            {
                case MASK_PARSE_INIT:
                    if (isspace(incoming))
                        MaskParseState = MASK_PARSE_N;
                    else
                    {
                        MaskParseState = MASK_PARSE_BAD;
                        ErrorMessage = "Unknown mask command";
                    }
                    break;

                case MASK_PARSE_N:
                    if (isspace(incoming))
                        break;
                    if (isdigit(incoming))
                    {
                        WhichBoard = incoming - '1';
                        if ((WhichBoard >= 0) && (WhichBoard < DaisyChainLength))
                        {
                            MaskOn = MaskForceOn[WhichBoard];
                            MaskOff = MaskForceOff[WhichBoard];
                            MaskParseState = MASK_PARSE_LMR_W;
                            break;
                        }
                        else
                        {
                            MaskParseState = MASK_PARSE_BAD;
                            ErrorMessage = "Invalid board number";
                        }
                    }
                    else
                    {
                        MaskParseState = MASK_PARSE_BAD;
                        ErrorMessage = "board number missing";
                    }
                    break;

                case MASK_PARSE_LMR_W:
                    if (isspace(incoming))
                        MaskParseState = MASK_PARSE_LMR;
                    else
                    {
                       MaskParseState = MASK_PARSE_BAD;
                       ErrorMessage = "Need space after <board-number>";
                    }
                    break;

                case MASK_PARSE_LMR:
                    if (isspace(incoming))
                        break;
                    if ((char)incoming == 'L')
                    {
                        MaskOnWhat = MASK_IS_L;
                        MaskParseState = MASK_PARSE_V;
                    }
                    else if ((char) incoming == 'M')
                    {
                        MaskOnWhat = MASK_IS_M;
                        MaskParseState = MASK_PARSE_V;
                    }
                    else if ((char) incoming == 'R')
                    {
                        MaskOnWhat = MASK_IS_R;
                        MaskParseState = MASK_PARSE_V;
                    }
                    else {
                        MaskParseState = MASK_PARSE_BAD;
                        ErrorMessage = "Expecting L or M or R";
                    }
                    break;

                case MASK_PARSE_V:
                    {
                        int value = fromHex(incoming);
                        if (value >= 0)
                        {
                            cmdValue = (uint8_t)value;
                            MaskParseState = MASK_PARSE_MASK;
                        }
                        else
                        {
                            MaskParseState = MASK_PARSE_BAD;
                            ErrorMessage = "Expecting hex digit";
                        }
                    }
                    break;

                case MASK_PARSE_MASK:
                    {
                        int mask = fromHex(incoming);
                        if (mask >= 0)
                        {
                            switch (MaskOnWhat)
                            {
                            case MASK_IS_L:
                                    MaskOn.setLeftBits(cmdValue, mask);
                                    MaskOff.setLeftBits(~cmdValue, mask);
                                break;
                            case MASK_IS_M:
                                    MaskOn.setMiddleBits(cmdValue, mask);
                                    MaskOff.setMiddleBits(~cmdValue, mask);
                                break;
                            case MASK_IS_R:
                                    MaskOn.setRightBits(cmdValue, mask);
                                    MaskOff.setRightBits(~cmdValue, mask);
                                break;
                            }    
                            MaskParseState = MASK_PARSE_LMR_W;
                        }
                        else
                        {
                            ErrorMessage = "Invalid mask";
                            MaskParseState = MASK_PARSE_BAD;
                        }
                    }
                default:
                    break;
            }
        }
    }

    switch (OperatingMode)
    {
    case MODE_DEFAULT:
        DoCopyMode(false); // copy inputs to outputs
        break;

    case MODE_OPERATE:
        DoCopyMode(true); // copy, but override with masks
        break;

    case MODE_MANUAL:
        break;
    }
}
