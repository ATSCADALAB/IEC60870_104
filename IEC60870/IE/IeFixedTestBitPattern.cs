﻿using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeFixedTestBitPattern : InformationElement
    {
        public IeFixedTestBitPattern()
        {
        }

        public IeFixedTestBitPattern(BinaryReader reader)
        {
            if (reader.ReadByte() != 0x55 || reader.ReadByte() != 0xaa)
            {
                throw new IOException("Incorrect bit pattern in Fixed Test Bit Pattern.");
            }
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = 0x55;
            buffer[i] = 0xaa;
            return 2;
        }

        public override string ToString()
        {
            return "Fixed test bit pattern";
        }
    }
}