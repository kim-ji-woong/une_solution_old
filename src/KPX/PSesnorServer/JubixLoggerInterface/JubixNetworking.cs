using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace JubixNetwork
{
    public class JubixMessage
    {
        private int nStartFlag = 0xfafb;

        private byte nMajorNo = 0x00;
        private byte nMinorNo = 0x01;

        private byte nYear = 0x11;
        private byte nMonth = 0x05;
        private byte nDay = 0x10;
        private byte nHour = 0x00;
        private byte nMin = 0x00;
        private byte nSec = 0x00;

        public bool IsValid()
        {
            if (nDay == 0xCC)
            {
                if (nHour == 0xCC)
                {
                    if (nMin == 0xCC)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int nSiteID;
        private int command;
        public int Command
        {
            get { return command; }
        }
        private short nSeq;

        private int nDataLength = 20;
        private ArrayList m_arDataList;

        public ArrayList DataList
        {
            get { return m_arDataList; }
        }

        private int nCheckSumHi;
        private int nCheckSumLow;

        private int nEndFlag = 0xfffe;
        private bool bOnOff = false;

        private JubixMessage()
        {
            SetTime();
        }

        public JubixMessage(short nCommand)
        {
            command = nCommand;

            SetTime();
        }

        // AA 커맨트에 대한 생성자
        public JubixMessage(short nCommand, bool bOnOff)
        {
            command = nCommand;
            this.bOnOff = bOnOff;
            SetTime();
        }

        public JubixMessage(short nCommand, ArrayList arrList)
        {
            command = nCommand;
            m_arDataList = arrList;
        }

        public void SetTime()
        {
            SetTime(DateTime.Now);
            /*DateTime dtNow = DateTime.Now;
            if( dtNow.Second == 0)
            {
                dtNow.AddSeconds(-1.0);
            }
            nYear = (byte)(dtNow.Year % 1000);
            nMonth = (byte)(dtNow.Month);
            nDay = (byte)(dtNow.Day);
            nHour = (byte)(dtNow.Hour);
            nMin = (byte)(dtNow.Minute);
            nSec = (byte)(dtNow.Second);*/
        }

        public void SetTime(DateTime time)
        {
            if (time.Second == 0)
            {
                time.AddSeconds(-1.0);
            }
            nYear = (byte)(time.Year % 1000);
            nMonth = (byte)(time.Month);
            nDay = (byte)(time.Day);
            nHour = (byte)(time.Hour);
            nMin = (byte)(time.Minute);
            nSec = (byte)(time.Second);
        }

        public string GetTimeString()
        {
            return string.Format("{0}-{1}-{2} {3:00}:{4:00}:{5:00}", nYear + 2000, nMonth, nDay, nHour, nMin, nSec);
        }

        public void CalcCheckSum(byte[] data, int idx)
        {
            nCheckSumHi = data[3];
            nCheckSumLow = data[3];
            for(int i = 4 ; i < idx; i++)
            {
                nCheckSumHi ^= data[i];
                nCheckSumLow += data[i];
            }

            nCheckSumHi = (byte)nCheckSumHi;
            nCheckSumLow = (byte)nCheckSumLow;
        }

        public static short SwapShort(byte[] datas, int nIdx)
        {
            byte[] temp = new byte[2];
            temp[1] = datas[nIdx++];
            temp[0] = datas[nIdx];
            return BitConverter.ToInt16(temp, 0);
        }
            
        public static byte[] SwapShort(short data)
        {
            byte[] temp = BitConverter.GetBytes(data);

            byte[] temp2 = new byte[2];
            temp2[1] = temp[0];
            temp2[0] = temp[1];
            return temp2;
        }     

        private static object lockObj = new object();
        public static JubixMessage ReadDataValue(byte[] nRecivedDatas, out int nCmd)
        {
            lock (lockObj)
            {
                JubixMessage msg = new JubixMessage();

                msg.nStartFlag = BitConverter.ToInt16(nRecivedDatas, 0);
                msg.nMajorNo = nRecivedDatas[2];
                msg.nMinorNo = nRecivedDatas[3];

                msg.nYear = nRecivedDatas[4];
                msg.nMonth = nRecivedDatas[5];
                msg.nDay = nRecivedDatas[6];
                msg.nHour = nRecivedDatas[7];
                msg.nMin = nRecivedDatas[8];
                msg.nSec = nRecivedDatas[9];

                msg.nSiteID = SwapShort(nRecivedDatas, 10);
                msg.command = SwapShort(nRecivedDatas, 12);
                nCmd = msg.command;
                msg.nSeq = nRecivedDatas[14];
                int nLength = nRecivedDatas[15];

                msg.nDataLength = nLength;
                ArrayList arDatas = new ArrayList();
                for (int i = 0; i < nLength ; i += 2)
                {
                    short nData = SwapShort(nRecivedDatas, 16 + i);
                    arDatas.Add(nData);
                }
                msg.m_arDataList = arDatas;

                int nIdx = nLength + 16;

                msg.nCheckSumHi = nRecivedDatas[nIdx++];
                msg.nCheckSumLow = nRecivedDatas[nIdx++];
                msg.nEndFlag = BitConverter.ToInt16(nRecivedDatas, nIdx);

                return msg;
            }
        }

        public byte[] MakeByte(bool bTest = false)
        {
            int nByteLength = nDataLength + 20;
            byte[] byteData = new byte[nByteLength];
            
            byteData[0] = 0xfa;
            byteData[1] = 0xfb;

            byteData[2] = nMajorNo;
            byteData[3] = nMinorNo;

            //msg.SetTime();

            byteData[4] = nYear;
            byteData[5] = nMonth;
            byteData[6] = nDay;
            byteData[7] = nHour;
            byteData[8] = nMin;
            byteData[9] = nSec;

            byte[] bSite = SwapShort((short)nSiteID);
            byte[] bCmd = SwapShort((short)command);

            Array.Copy(bSite,0, byteData, 10, bSite.Length);
            Array.Copy(bCmd, 0, byteData, 12, bCmd.Length);

            byteData[14] = (byte)nSeq;
            byteData[15] = (byte)nDataLength;

            byte[] nData = MakeDataByte((short)command, nDataLength, bTest);
            Array.Copy(nData, 0, byteData, 16, nData.Length);

            int nIdx = nData.Length + 16;

            CalcCheckSum(byteData, nIdx);

            byteData[nIdx++] = (byte)nCheckSumHi;
            byteData[nIdx++] = (byte)nCheckSumLow;

            byteData[nIdx++] = 0xff;
            byteData[nIdx++] = 0xfe;

            return byteData;
        }


        Random r = new Random();
        int prevValue = 0;
        int nTestCnt = 0;
        private byte[] MakeDataByte(short nCmd, int nLength, bool bTest)
        {
            byte[] byteData = new byte[nLength];

            if (bTest == true)
            {
                int nRand = 500 + ( r.Next() % 200 );
                if (nTestCnt == 100)
                {
                    nRand = 0;
                    nTestCnt = 0;
                }
                for( int i = 0 ; i < nLength ; i +=2 )
                {
                    byte[] temp = SwapShort((short)nRand);
                    byteData[i] = temp[0];
                    byteData[i + 1] = temp[1];
                }
                nTestCnt++;

            }
            else
                Array.Clear(byteData, 0, nLength);

            

            if (nCmd == JUBIX_TCP_COMMAND.AT)
            {
                DateTime dtNow = DateTime.Now;
                dtNow.AddSeconds(5.0f);
                nYear = (byte)(dtNow.Year % 1000);
                nMonth = (byte)(dtNow.Month);
                nDay = (byte)(dtNow.Day);
                nHour = (byte)(dtNow.Hour);
                nMin = (byte)(dtNow.Minute);
                nSec = (byte)(dtNow.Second);

                byteData[0] = nYear;
                byteData[1] = nMonth;
                byteData[2] = nDay;
                byteData[3] = nHour;
                byteData[4] = nMin;
                byteData[5] = nSec;
            }
            if (nCmd == JUBIX_TCP_COMMAND.AA)
            {
                byteData[0] = (bOnOff ? (byte)0x01 : (byte)0x00);
                
                DateTime dtNow = DateTime.Now;
                dtNow.AddSeconds(6.0f);
                nYear = (byte)(dtNow.Year % 1000);
                nMonth = (byte)(dtNow.Month);
                nDay = (byte)(dtNow.Day);
                nHour = (byte)(dtNow.Hour);
                nMin = (byte)(dtNow.Minute);
                nSec = (byte)(dtNow.Second);
            }
            return byteData;
        }

        private byte[] SimMakeDataByte(short nCmd, int nLength, int nData, float value)
        {
            byte[] byteData = new byte[nLength];
            int nCount = 0;
            for (int i = 0; i < nLength; i += 2)
            {
                short vv = (short)0;

                if( nCount == nData)
                {
                    vv = (short)value;
                }

                byte[] temp = SwapShort((short)vv);
                byteData[i] = temp[0];
                byteData[i + 1] = temp[1];

                nCount++;
            }
            return byteData;
        }

        public byte[] SimulationMakeByte(bool p, int n, float value)
        {
            int nByteLength = nDataLength + 20;
            byte[] byteData = new byte[nByteLength];

            byteData[0] = 0xfa;
            byteData[1] = 0xfb;

            byteData[2] = nMajorNo;
            byteData[3] = nMinorNo;

            //msg.SetTime();

            byteData[4] = nYear;
            byteData[5] = nMonth;
            byteData[6] = nDay;
            byteData[7] = nHour;
            byteData[8] = nMin;
            byteData[9] = nSec;

            byte[] bSite = SwapShort((short)nSiteID);
            byte[] bCmd = SwapShort((short)command);

            Array.Copy(bSite, 0, byteData, 10, bSite.Length);
            Array.Copy(bCmd, 0, byteData, 12, bCmd.Length);

            byteData[14] = (byte)nSeq;
            byteData[15] = (byte)nDataLength;

            byte[] nData = SimMakeDataByte((short)command, nDataLength, n, value);
            Array.Copy(nData, 0, byteData, 16, nData.Length);

            int nIdx = nData.Length + 16;

            CalcCheckSum(byteData, nIdx);

            byteData[nIdx++] = (byte)nCheckSumHi;
            byteData[nIdx++] = (byte)nCheckSumLow;

            byteData[nIdx++] = 0xff;
            byteData[nIdx++] = 0xfe;

            return byteData;
        }
    }

    public class JubixLogger
    {
        private string m_szIPAddress;
        private int m_nPort;

    }


    public class JUBIX_TCP_COMMAND
    {        
        // 데이터 조회

        // 현재값 조회
        public const int AI = 0x4149;
        // 최근 1분 데이터 조회
        public const int AB = 0x4142;
        // 과거 1분 데이터 조회
        public const int AQ = 0x4151;
        // 데이터 로거 버전 조회
        public const int AV = 0x4156;
        
        // 로거 제어

        // 데이터 로거 리셋
        public const int AR = 0x4152;
        // 데이터 로거 시간설정
        public const int AT = 0x4154;
        // IO 제어 - 경광등 on/off
        public const int AA = 0x4141;

        // 로거 및 탱크 시뮬레이션
        public const int SS = 0x4242;
    }

    public class JUBIX_TCP_FLAG
    {
        public const int START = 0xfafb;
        public const int END = 0xfffe;
    }

}
