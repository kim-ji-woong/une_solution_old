using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackoutServer
{
    public class MsgHelper
    {
        private static byte Header = 0x01;
        public static byte[] MakeMsgReadCoils(byte nAddress, out byte nHeader)
        {
            byte[] data = new byte[12];
            data[0] = Header;
            data[1] = 0x00;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x06;
            data[6] = 0x01;

            data[7] = 0x01; //function code
                        
            data[8] = 0x00;
            data[9] = 0x00; // 읽어올 주소 8로 (8은 출력포트 기본 값)

            data[10] = 0x00;
            data[11] = 0x06; // 시작 주소부터 8개의 출력포트를 읽음

            Header++;
            nHeader = Header;
            return data;
        }
    }
}
