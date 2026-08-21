using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasDetector
{
    internal class MsgHelper
    {
        internal static byte[] MakeData(byte nBaseHi, byte nBaseLow, byte nHmiHi, byte nHmiLow, byte nFunc, byte nAddress)
        {
            byte[] data = new byte[8];

            data[0] = nAddress;
            data[1] = nFunc;
            data[2] = nBaseHi;
            data[3] = nBaseLow;
            data[4] = nHmiHi;
            data[5] = nHmiLow;

            byte[] crc = new byte[2];
            GetCRC(data, ref crc);

            data[6] = crc[0];
            data[7] = crc[1];
            return data;
        }

        internal static byte[] MakeLongCmdData(byte nStartAddressHi, byte nStartAddressLow, byte nRegCntHi, byte nRegCntLow, byte nCnt, byte nValueHi, byte nValueLow, byte nFunc, byte nAddress)
        {
            byte[] data = new byte[11];

            data[0] = nAddress;
            data[1] = nFunc;

            data[2] = nStartAddressHi;
            data[3] = nStartAddressLow;

            data[4] = nRegCntHi;
            data[5] = nRegCntLow;

            data[6] = nCnt;

            data[7] = nValueHi;
            data[8] = nValueLow;

            byte[] crc = new byte[2];
            GetCRC(data, ref crc);

            data[9] = crc[0];
            data[10] = crc[1];
            return data;
        }

        private static void GetCRC(byte[] message, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
    }
}
