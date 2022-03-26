#include <SPI.h>
#define DIM(x) (sizeof(x) / sizeof((x)[0]))
#define DBG_PRINT 0

/* Arduino sketch for the ASW12V remote control switch box
** The hardware is an array of input screw terminals and 
** an array of output screw terminals of the same size.
** The "12V" in the name is only because 12VDC is typically
** used in ham antenna control boxes. This circuit can handle
** up to 60V and either AC or DC.
**
** The architecture is to be able to insert this device
** in a low voltage switching circuit such that 
** manual switching at our inputs controls the 
** devices at our outputs by asserting supply voltage on 
** those outputs in various combinations. The circuit 
** isolates the controlled supply and ground from our Arduino
** power, which is on USB power. 
** The TLP355A solid state relay (SSR) can switch up 60V AC or DC
** The circuit board traces on the OUTPUT side of the PCB
** are capable of handling 3A. The INPUT side has 
** narrower traces.
**
** All the input sensing channels and all the output control channels
** are on a single MAX7301 Port expander. It has a 28 channel capacity,
** of which we need 12 inputs and 12 outputs. The MAX7301 numbers its
** channels starting at P4 up through P31. This design leaves the four
** lowest unused, and uses P8 through P31.
**
** At code startup, we assume we're operating stand alone, which
** means copying our inputs 
** to our outputs. "We" means this sketch on the Arduino.
** It uses only 3 SPI pins and a M7301_SELECT pin. VCC and ground are the remaining
** connections. The Adafruit QT Py device has all these pins,
** is tiny, and has a USB-C connector with 500mV of 3.3VDC available
** to power the control circuits. 
**
** The board is designed for daisy chaining. That means 
** we might have another MAX7301 on a second circuit
** board cabled to our J2 and their J1. The daisy chaining
** can continue to more boards.
** This firmware needs to know how many boards
** are out there on the daisy chain. It calculates that by insisting:
** a) that the final board on the daisy chain (even if
** we're the only board) has its J2 I/O pins jumpered 
** together. That allows us to shift data out and detect
** when it comes back.
** b) in order to put a limit on how much work we must do
** to detect the length of the daisy chain, that
** number is limited to 8 boards (which totals 96 channels.)
**
** Once we detect the length of the daisy chain, 
** our loop() copies the input registers to
** the output registers. 
**
** The LED in each SSR requires about
** 10mA to turn on. That means a limit of at most 50 channels ON at one time.
**
** loop() also monitors the serial input for commands.
** One set of commands is designed for self test.
**
*/

namespace {
    const int M7301_SELECT = A2; // the PCB wires the QT PY pin A2 to the MAX7301 select
    const int MAX_BOARDS_DAISY_CHAINED = 8;
    const uint8_t WRITE_MAX7301_P08 = 0x48u;

    const uint8_t WRITE_MAX7301_P16 = 0x50u;
    const uint8_t WRITE_MAX7301_P24 = 0x58u;
    const uint8_t READ_MAX7301_MASK = 0x80u;
    const uint8_t READ_MAX7301_P08 = WRITE_MAX7301_P08 | READ_MAX7301_MASK;
    const uint8_t READ_MAX7301_P16 = WRITE_MAX7301_P16 | READ_MAX7301_MASK;
    const uint8_t PORT_CONFIG_MAX7301_P04 = 0x9u;
    const uint8_t PORT_CONFIG_MAX7301_P08 = 0xAu;
    const uint8_t PORT_CONFIG_MAX7301_P12 = 0xBu;
    const uint8_t PORT_CONFIG_MAX7301_P16 = 0xCu;
    const uint8_t PORT_CONFIG_MAX7301_P20 = 0xDu;
    const uint8_t PORT_CONFIG_MAX7301_P24 = 0xEu;
    const uint8_t PORT_CONFIG_MAX7301_P28 = 0xFu;
    const uint8_t CONFIG_MAX7301 = 0x4u;
    const uint8_t MAX7301_NOOP = 0;
    const uint8_t MAX7301_DUMMY = 0;
    const uint8_t CONFIG_NORMAL = 1u;

    const uint8_t PORT_P18_P19_OUTPUT = 0b01010000u;
    const uint8_t PORT_P22_P24_OUTPUT = PORT_P18_P19_OUTPUT;

    const uint8_t PortSetup[][2] =
    {
        {PORT_CONFIG_MAX7301_P08, 0xffu},
        {PORT_CONFIG_MAX7301_P12, 0xffu},
        {PORT_CONFIG_MAX7301_P16, 0xfu | PORT_P18_P19_OUTPUT},
        {PORT_CONFIG_MAX7301_P20, 0xfu | PORT_P22_P24_OUTPUT},
        {PORT_CONFIG_MAX7301_P24, 0x55u},
        {PORT_CONFIG_MAX7301_P28, 0x55u},
        {WRITE_MAX7301_P16, 0xff},
        {WRITE_MAX7301_P24, 0xff},
        {CONFIG_MAX7301, CONFIG_NORMAL},
    };
}

struct ShiftRegister_t {
    uint8_t LeftRegister; // high 4 bits are left register.
    uint8_t RightAndMiddleRegister; // low 4 are Middle, high 4 are Right
    ShiftRegister_t() : LeftRegister(0), RightAndMiddleRegister(0){}
    void init() { LeftRegister = RightAndMiddleRegister = 0;}
    // the channels are numbered 0 through 3 which are,
    // respectively, numbered 1 through 4 on the PCB silkscreen
    void setLeftBits(uint8_t channel, uint8_t mask) // both range of 0 to f 
    {   
        mask &= 0xf; // only 4 bits allowed, the least ones.
        LeftRegister &= ~mask;
        LeftRegister |= channel & mask;
    }
    void setMiddleBits(uint8_t channel, uint8_t mask)
    {
        mask &= 0xf;
        RightAndMiddleRegister &= ~mask;
        RightAndMiddleRegister |= channel & mask;
    }
    void setRightBits(uint8_t v, uint8_t mask)
    {
        mask <<= 4;
        mask &= 0xf0;
        RightAndMiddleRegister &= ~mask;
        RightAndMiddleRegister |= (v<<4) & mask;
    }

    uint8_t rawMRout()
    {   // translate RightAndMiddleRegister to WRITE_MAX7301_P24
#if 0 // if we bit diddle
        static const int P24_BIT = 0;
        static const int P25_BIT = 1;
        static const int P26_BIT = 2;
        static const int P27_BIT = 3;
        static const int P28_BIT = 4;
        static const int P29_BIT = 5;
        static const int P30_BIT = 6;
        static const int P31_BIT = 7;
        uint8_t ret = 0xff;
        // middle bits
        if (RightAndMiddleRegister & 1)
            ret &= ~(1 << P24_BIT);
        if (RightAndMiddleRegister & 2)
            ret &= ~(1 << P25_BIT);
        if (RightAndMiddleRegister & 4)
            ret &= ~(1 << P26_BIT);
        if (RightAndMiddleRegister & 8)
            ret &= ~(1 << P27_BIT);
        // right bits
        if (RightAndMiddleRegister & 0x10u)
            ret &= ~(1 << P28_BIT);
        if (RightAndMiddleRegister & 0x20u)
            ret &= ~(1 << P29_BIT);
        if (RightAndMiddleRegister & 0x40u)
            ret &= ~(1 << P30_BIT);
        if (RightAndMiddleRegister & 0x80u)
            ret &= ~(1 << P31_BIT);
        return ret;
#else // but the PCB layout arranges for these outputs to align 
        // with the MAX7301 pin layout
        return ~RightAndMiddleRegister;
#endif
    }

    uint8_t rawLout()
    {   // translate LeftRegister to WRITE_MAX7301_P16
        static const int P18_BIT = 2;
        static const int P19_BIT = 3;
        static const int P22_BIT = 6;
        static const int P23_BIT = 7;
        uint8_t ret = 0xff;
        if (LeftRegister & 1)
            ret &= ~(1 << P23_BIT);
        if (LeftRegister & 2)
            ret &= ~(1 << P19_BIT);
        if (LeftRegister & 4)
            ret &= ~(1 << P18_BIT);
        if (LeftRegister & 8)
            ret &= ~(1 << P22_BIT);
        return ret;
    }
};

struct ShiftRegisterWithTimer_t : public ShiftRegister_t
{
    static const int DELAY_DETECTING_ZERO_MSEC; // to support AC input
    void rawMRin(uint8_t v, bool allowZeroing)
    {   // translate the results of READ_MAX7301_P08 to RightAndMiddleRegister 
        static const int P08_BIT = 0;
        static const int P09_BIT = 1;
        static const int P10_BIT = 2;
        static const int P11_BIT = 3;
        static const int P12_BIT = 4;
        static const int P13_BIT = 5;
        static const int P14_BIT = 6;
        static const int P15_BIT = 7;

        uint8_t tempM = 0;
        uint8_t tempR = 0;

        // translate v to tempM and tempR
        // invert cuz input v is active low, but tempM/R are active high

        if (0 == (v & (1 << P15_BIT)))
            tempM |= 1;
        if (0 == (v & (1 << P11_BIT)))
            tempM |= 2;
        if (0 == (v & (1 << P14_BIT)))
            tempM |= 4;
        if (0 == (v & (1 << P10_BIT)))
            tempM |= 8;

        if (0 == (v & (1 << P13_BIT)))
            tempR |= 0x10u;
        if (0 == (v & (1 << P09_BIT)))
            tempR |= 0x20u;
        if (0 == (v & (1 << P12_BIT)))
            tempR |= 0x40u;
        if (0 == (v & (1 << P08_BIT)))
            tempR |= 0x80u;

        uint8_t tempRM = 0;
        auto now = millis();
        if (!allowZeroing)
        {   // extend the last-seen timers on long delay between polls
            FirstReadZeroMsecM = now;
            FirstReadZeroMsecR = now;
        }
        // Only after going DELAY_DETECTING_ZERO_MSEC with all zeros do we
        // allow any of the input bits to be recorded as zero.
        if (tempM == 0 && PrevM != 0)
            FirstReadZeroMsecM = now;
        if (tempR == 0 && PrevR != 0)
            FirstReadZeroMsecR = now;

        if (static_cast<int>(now - FirstReadZeroMsecM) > DELAY_DETECTING_ZERO_MSEC)
            tempRM |= tempM;
        else // time not elapsed. Any new nonzero bits or'd with any existing
            tempRM |= (0xfu & RightAndMiddleRegister) | tempM;

        if (static_cast<int>(now - FirstReadZeroMsecR) > DELAY_DETECTING_ZERO_MSEC)
            tempRM |= tempR;
        else
            tempRM |= (0xf0u & RightAndMiddleRegister) | tempR;

        PrevM = tempM;
        PrevR = tempR;

        RightAndMiddleRegister = tempRM;
    }

    void rawLin(uint8_t v, bool allowZeros)
    {   // translate READ_MAX7301_P16 to L
        uint8_t tempL = 0;

        static const int P16_BIT = 0;
        static const int P17_BIT = 1;
        static const int P20_BIT = 4;
        static const int P21_BIT = 5;

        // translate v to tempM and tempR
        // invert cuz input v is active low, but tempL is active high

        if (0 == (v & (1 << P20_BIT)))
            tempL |= 1;
        if (0 == (v & (1 << P21_BIT)))
            tempL |= 2;
        if (0 == (v & (1 << P17_BIT)))
            tempL |= 4;
        if (0 == (v & (1 << P16_BIT)))
            tempL |= 8;

        uint8_t temp = 0;
        auto now = millis();
        if (!allowZeros  // extend the last-seen timers on long delay between polls
            || ((tempL == 0) && (PrevL != 0)))
        {  
            // Only after going DELAY_DETECTING_ZERO_MSEC with all zeros do we
            // allow any of the input bits to be recorded as zero.
            FirstReadZeroMsecL = now;
        }
        if 
            FirstReadZeroMsecL = now;
        if (static_cast<int>(now - FirstReadZeroMsecL) > DELAY_DETECTING_ZERO_MSEC)
            temp |= tempL;
        else // time not elapsed. Any new nonzero bits or'd with any existing
            temp |= (0xfu & LeftRegister) | tempL;

        PrevL = tempL;
        LeftRegister = temp;
        LeftRegister |= 0xc0; // for compatibility with older PCB that always reads those two highest bits zero
    }
protected:
    unsigned long FirstReadZeroMsecL;
    uint8_t PrevL;
    unsigned long FirstReadZeroMsecM;
    uint8_t PrevM;
    unsigned long FirstReadZeroMsecR;
    uint8_t PrevR;
};
const int ShiftRegisterWithTimer_t::DELAY_DETECTING_ZERO_MSEC = 15; // input stays zero this long to believe it is zero
namespace {

    // here are the SPI parameters for the MAX7301
    const SPISettings SPISetup(10000000, MSBFIRST, SPI_MODE0);

    // each of the following arrays is addressed such that the zeroth element
    // is the one with the QT PY controller installed. In the user CLI, the
    // origin element is number one.
    ShiftRegisterWithTimer_t ShiftRegisters[MAX_BOARDS_DAISY_CHAINED];
    ShiftRegister_t AsOutput[MAX_BOARDS_DAISY_CHAINED];
    ShiftRegister_t MaskForceOn[MAX_BOARDS_DAISY_CHAINED];
    ShiftRegister_t MaskForceOff[MAX_BOARDS_DAISY_CHAINED];

    int DaisyChainLength = -1; // zero or negative: don't know

    enum OperatingMode_t {
        MODE_DEFAULT, // Copy inputs to outputs
        MODE_OPERATE, // Copy inputs to outputs, modified by masks from Serial
        MODE_MANUAL, // Outputs manipulated directly from Serial
    } OperatingMode = MODE_DEFAULT;

    int FindDaisyChainLength()
    {   // prime the shift register with all zeros
        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
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
            uint8_t b1 = SPI.transfer(0);
            uint8_t b2 = SPI.transfer(0);
            if ((b1 == MARKER1) && (b2 == MARKER2))
                break;
        }
        digitalWrite(M7301_SELECT, HIGH); // 7301 executes on this. only no-ops are in them now.
        SPI.endTransaction();
        if (ret >= MAX_BOARDS_DAISY_CHAINED)
            return -1;
        return ret;
    }

    bool printOutOnce;
    int numNotOkToZero;

    void readInputs(ShiftRegisterWithTimer_t sr[])
    {
        auto now = millis();
        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
        /* The 12 inputs appear on P08 through P21 (excepting P18 and P19)
        ** All 8 of the R and M channels are read using
        **                        command 0x48 on the MAX7301 (READ_MAX7301_P08)
        ** The 4 L channels are read with 0x50 (READ_MAX7301_P16)
        ** For each 12-channel group in the daisy chain we must:
        **      Read two bytes from the PE, each of which requires:
        **          A two byte SPI shift of the READ command followed by
        **          A two byte SPI shift to read the result.
        */
        int i = 0;
        for (i = 0; i < DaisyChainLength; i += 1)
        {
            SPI.transfer(READ_MAX7301_P08); // read 8 lower inputs
            SPI.transfer(MAX7301_DUMMY);
        }
        digitalWrite(M7301_SELECT, HIGH);
        SPI.endTransaction(); // Tell the PE's to execute the first READ command

        static unsigned long lastReadMsec;
        bool OkToZero = now < lastReadMsec + ShiftRegisterWithTimer_t::DELAY_DETECTING_ZERO_MSEC / 2;
        if (!OkToZero)
            numNotOkToZero += 1;
        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
        for (i -= 1; i >= 0; i -= 1) // daisy chain reads PCBs in reverse order
        {
            auto res1 = SPI.transfer(MAX7301_NOOP);
            auto res2 = SPI.transfer(MAX7301_DUMMY);
#if DBG_PRINT
            Serial.print("readInputs P08. i=");
            Serial.print(i);
            Serial.print(" res1 = 0x"); Serial.print((int)res1, HEX);
            Serial.print(" res2=0x"); Serial.println((int)res2, HEX);
            OkToZero = true;
#endif
            sr[i].rawMRin(res2, OkToZero);
        }
        digitalWrite(M7301_SELECT, HIGH);
        SPI.endTransaction(); // 

        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
        // now the L channels on the entire daisy chain
        for (i = 0; i < DaisyChainLength; i += 1)
        {
            SPI.transfer(READ_MAX7301_P16);
            SPI.transfer(MAX7301_DUMMY);
        }
        digitalWrite(M7301_SELECT, HIGH);
        SPI.endTransaction(); // Tell the PE's to execute the first READ command

        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
        for (i -= 1; i >= 0; i -= 1)
        {
            auto res1 = SPI.transfer(MAX7301_NOOP);
            auto res2 = SPI.transfer(MAX7301_DUMMY);
#if DBG_PRINT
            Serial.print("readInputs P16. i=");
            Serial.print(i); Serial.print(" res1 = 0x"); 
            Serial.print((int)res1, HEX);
            Serial.print(" res2=0x"); Serial.println((int)res2, HEX);
            OkToZero = true;
#endif
            sr[i].rawLin(res2, OkToZero);
        }
        digitalWrite(M7301_SELECT, HIGH);
        SPI.endTransaction();
        lastReadMsec = now;
    }

    void writeOutputs(ShiftRegister_t sr[])
    {
        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
        for (int i = DaisyChainLength - 1; // most distant PCB first
            i >= 0; i -= 1)
        {
            SPI.transfer(WRITE_MAX7301_P24);
            SPI.transfer(sr[i].rawMRout());
        }
        digitalWrite(M7301_SELECT, HIGH);
        SPI.endTransaction();
        // output L
        SPI.beginTransaction(SPISetup);
        digitalWrite(M7301_SELECT, LOW);
        for (int i = DaisyChainLength - 1; // most distant PCB first
            i >= 0; i -= 1)
        {
            SPI.transfer(WRITE_MAX7301_P16);
            SPI.transfer(sr[i].rawLout());
        }
        digitalWrite(M7301_SELECT, HIGH);
        SPI.endTransaction();
    }

    void DoCopyMode(bool HonorMasks)
    {
        if (DaisyChainLength <= 0)
            return;

        readInputs(ShiftRegisters);

        // apply HonorMasks
        for (int i = 0; i < DaisyChainLength; i += 1)
        {   // outputs do not invert
            uint8_t LeftRegister = ShiftRegisters[i].LeftRegister;
            uint8_t RightAndMiddleRegister = ShiftRegisters[i].RightAndMiddleRegister;
            if (HonorMasks)
            {
                LeftRegister |= MaskForceOn[i].LeftRegister;
                RightAndMiddleRegister |= MaskForceOn[i].RightAndMiddleRegister;
                LeftRegister &= ~MaskForceOff[i].LeftRegister;
                RightAndMiddleRegister &= ~MaskForceOff[i].RightAndMiddleRegister;
            }
            AsOutput[i].LeftRegister = LeftRegister;
            AsOutput[i].RightAndMiddleRegister = RightAndMiddleRegister;
        }

        // output MR
        writeOutputs(AsOutput);

        if (printOutOnce)
        {
            Serial.print("Out:  ");
            for (int i = 0; i < DaisyChainLength; i += 1)
            {
                if (i != 0)
                    Serial.print(" ");
                Serial.print((int)AsOutput[i].LeftRegister, HEX);
                Serial.print(" ");
                if (AsOutput[i].RightAndMiddleRegister < 16)
                    Serial.print("0");
                Serial.print((int)AsOutput[i].RightAndMiddleRegister, HEX);
            }
            Serial.println();
        }
        printOutOnce = false;
    }

    void Configure7301()
    {
        for (int i = 0; i < DIM(PortSetup); i++)
        {
#if DBG_PRINT
            SPI.beginTransaction(SPISetup);
            digitalWrite(M7301_SELECT, LOW);
            int j = 0;
            for (j = 0; j < DaisyChainLength; j++)
            {
                SPI.transfer(READ_MAX7301_MASK | PortSetup[i][0]);
                SPI.transfer(0);
            }
            digitalWrite(M7301_SELECT, HIGH); // executes the command
            SPI.endTransaction();

            SPI.beginTransaction(SPISetup);
            digitalWrite(M7301_SELECT, LOW);
            for (j -= 1; j >= 0 ; j -= 1)
            {
                int p1 = SPI.transfer(MAX7301_NOOP);
                int p2 = SPI.transfer(MAX7301_DUMMY);
                Serial.print("Configure i=");
                Serial.print(i);
                Serial.print(" cmd=0x"); Serial.print((int)p1, HEX);
                Serial.print(" res=0x"); Serial.println((int)p2, HEX);
            }
            digitalWrite(M7301_SELECT, HIGH); // executes the command
            SPI.endTransaction();
#endif
            SPI.beginTransaction(SPISetup);
            digitalWrite(M7301_SELECT, LOW);
            for (int j = 0; j < DaisyChainLength; j++)
            { // 7301's in the chain get the identical setup
                int p1 = SPI.transfer(PortSetup[i][0]);
                int p2 = SPI.transfer(PortSetup[i][1]);
            }
            digitalWrite(M7301_SELECT, HIGH); // executes the command
            SPI.endTransaction();
        }  
    }
}

void setup()
{
    digitalWrite(M7301_SELECT, HIGH);    // SS pin we are using for SPI
    pinMode(M7301_SELECT, OUTPUT);

    Serial.begin(9600); // control at 9600 baud, ascii

    SPI.begin();
    DaisyChainLength = FindDaisyChainLength();

#if DBG_PRINT==0
    Configure7301();
#endif
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
static unsigned char channelToWrite;

static void processCopyMode()
{
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
                Serial.println("ASW12V controller"); // tell user we're here...
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
                channelToWrite = 0;
                OperatingMode = MODE_DEFAULT;
            }
            else if ((char)incoming == (char)'i')
            {   // print current inputs
                Serial.print("Read:");
                for (int i = 0; i < DaisyChainLength; i++)
                {
                    unsigned char receivedRM = ShiftRegisters[i].RightAndMiddleRegister;
                    unsigned char receivedL = ShiftRegisters[i].LeftRegister;
                    Serial.print(" ");
                    Serial.print((int)receivedL, HEX);
                    Serial.print(" ");
                    if (receivedRM < 16)
                        Serial.print("0");
                    Serial.print((int)receivedRM, HEX);
                }
                Serial.println();
            }
            else if ((char)incoming == (char)'o')
            {   // print current outputs
                printOutOnce = true;
            }
            else if ((char)incoming == (char)'t')
            {   // shift a single ONE across all the outputs for each "t" press
                OperatingMode = MODE_MANUAL;
                if (DaisyChainLength > 0)
                {
                    //channelToWrite < 12 is right-most board
                    // up to 24 is second, etc.
                    if ((unsigned)channelToWrite >= 12 * (unsigned)DaisyChainLength)
                        channelToWrite = 0;
                    Serial.print("Right-to-Left channel ");
                    Serial.print(1 + channelToWrite);
                    Serial.println(" is ON. all others OFF.");
                    // channel numbers for test documentation start at right and work to left.
                    // but we have to write shift registers at left and work to right
                    // so transpose the channel number to that higher is right
                    unsigned char channelToWriteInThisBoard = (12 * DaisyChainLength) - 1 - (unsigned)channelToWrite;
                    // and count down during shifting by boards, 
                    for (int i = 0; i < DaisyChainLength; i++)
                    {
                        unsigned char outputRM = 0; // 8 bits 
                        unsigned char outputL = 0;   // 4 bits are output
                        if (channelToWriteInThisBoard < 12)
                        {
                            uint16_t mask = 1u << (unsigned)channelToWriteInThisBoard;
                            static const unsigned char boardMask[8] =
                            {
                                0x80, 0x40, 0x20, 0x10, 8, 4, 2, 1,
                            };
                            if (channelToWriteInThisBoard < 4)
                                outputL = boardMask[7 - channelToWriteInThisBoard];
                            else
                                outputRM = boardMask[11 - channelToWriteInThisBoard];
                            AsOutput[i].LeftRegister = outputL;
                            AsOutput[i].RightAndMiddleRegister = outputRM;
                        }
                        channelToWriteInThisBoard -= 12;
                    }
                    writeOutputs(AsOutput);
                    channelToWrite += 1;
                }
            }
            else if ((char)incoming == (char)'m')
            {
                ParseState = PARSE_MASK;
                OperatingMode = MODE_OPERATE;
            }
            else if ((char)incoming == (char)'n') 
            {   
                Serial.print("numNotOKtoZero="); Serial.println(numNotOkToZero);
            }
#if DBG_PRINT
            else if ((char)incoming == (char)'C')
            {
                Configure7301();
            }
            else if ((char)incoming == (char)'I')
            {
                readInputs(ShiftRegisters);
            }
            else if ((char)incoming == (char)'M')
                processCopyMode();
#endif
        } else if (ParseState == PARSE_MASK)
        {
            // Mask command syntax:
            // m <board-number> <L|M|R><value><mask> [repeat]
            //   The <L|M|R><value><mask> can be repeated multiple times on the same line                
            // where:
            //   m is the character m
            //   <board-number> is a single digit signifying which daisy-chained board in the range 1-9
            //   <L|M|R> is one of the characters L,M or R
            //   value is a single hex digit 0-9 a-f or A-F
            //   mask is a single hex digit 0-9 a-f or A-F
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
                // ... sets up runtime masks that, at loop() time, force a given
		// output either on or off regardless of input. By default, 
		// each output is driven by its corresponding input.
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

#if DBG_PRINT == 0
    processCopyMode();
#endif
}
