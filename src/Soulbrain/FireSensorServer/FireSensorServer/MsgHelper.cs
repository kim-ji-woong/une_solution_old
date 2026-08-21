using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSensorServer
{
    internal class MsgHelper
    {

        private static byte Header = 0x01;
        internal static byte[] MakeData(byte nBaseHi, byte nBaseLow, byte nHmiHi, byte nHmiLow, byte nFunc, byte nAddress, out byte nHeader)
        {
            byte[] data = new byte[12];
            data[0] = Header;
            data[1] = 0;
            data[2] = 0;
            data[3] = 0;
            data[4] = 0;
            data[5] = 6;

            data[6] = nAddress;
            data[7] = nFunc;
            data[8] = nBaseHi;
            data[9] = nBaseLow;
            data[10] = nHmiHi;
            data[11] = nHmiLow;

            //byte[] crc = new byte[2];
            //GetCRC(data, ref crc);

            //data[6] = crc[0];
            //data[7] = crc[1];

            if (Header == 0xFF)
                Header = 0x00;

            Header++;
            nHeader = Header;

            return data;
        }

        internal static byte[] MakeLongCmdReadRegisterData(byte nStartAddressHi, byte nStartAddressLow, byte nRegCntHi, byte nRegCntLow, byte nAddress, out byte nHeader)
        {
            byte[] data = new byte[7];
            data[0] = Header;

            data[1] = nAddress;          // Slave Address
            data[2] = 0x03;              // Function

            data[3] = nStartAddressHi;
            data[4] = nStartAddressLow;

            data[5] = nRegCntHi;
            data[6] = nRegCntLow;

            /*
            byte[] data = new byte[12];
            data[0] = Header;
            data[1] = 0;
            data[2] = 0;
            data[3] = 0;
            data[4] = 0;
            data[5] = 6;

            data[6] = nAddress;          // Slave Address
            data[7] = 0x03;              // Function

            data[8] = nStartAddressHi;
            data[9] = nStartAddressLow;

            data[10] = nRegCntHi;
            data[11] = nRegCntLow;
            */

            Header++;
            nHeader = Header;
            return data;
        }

        internal static byte[] MakeCmdReadRegisterData(byte nStartAddressHi, byte nStartAddressLow, byte nRegCntHi, byte nRegCntLow, byte nAddress, out byte nHeader)
        {
            byte[] data = new byte[6];
            data[0] = Header; 
            data[6] = nAddress;
            data[7] = 0x04;

            data[8] = nStartAddressHi;
            data[9] = nStartAddressLow;

            data[10] = nRegCntHi;
            data[11] = nRegCntLow;

            //byte[] crc = new byte[2];
            //GetCRC(data, ref crc);

            //data[6] = crc[0];
            //data[7] = crc[1];
            Header++;
            nHeader = Header;
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
        internal static byte[] MakeCmdWriteSingle(byte nAddress, byte nFunc, byte nStartAddressHi, byte nStartAddressLow, byte nRegValueHi, byte nRegValueLo)
        {
            byte[] data = new byte[8];

            data[0] = nAddress;
            data[1] = nFunc;

            data[2] = nStartAddressHi;
            data[3] = nStartAddressLow;

            data[4] = nRegValueHi;
            data[5] = nRegValueLo;

            byte[] crc = new byte[2];
            GetCRC(data, ref crc);

            data[6] = crc[0];
            data[7] = crc[1];
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
