using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TankModbusLib
{
    internal class KPXLeakDetector
    {
        LeakDetectRegister[] m_Units = null;

        private int m_nFunction = 4;
        public int Function
        {
            get { return m_nFunction; }
            set { m_nFunction = value; }
        }
        
        private string m_szName = "";
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        private int m_nStatusFunction = 1;


        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
        }

        private int m_nSlaveID = -1;
        public int SlaveID
        {
            get { return m_nSlaveID; }
            set { m_nSlaveID = value; }
        }
         
        private int m_nLastUnit = 0;
        private int m_nStartUnit = 0;

        private SerialManager mSerialMan = null;
        internal SerialManager Serial
        {
            get { return mSerialMan; }
            set 
            {
                m_bTcpMode = false;
                mSerialMan = value; 
            }
        }

        private NetworkManager mNetworkMan = null;
        internal NetworkManager NetworkMan
        {
            get { return mNetworkMan; }
            set
            {
                m_bTcpMode = true;
                mNetworkMan = value;
            }
        }

        private bool m_bTcpMode = false;
        public bool TcpMode
        {
            get { return m_bTcpMode; }

        }

        private bool m_bOffline = true;
        public bool IsOnline()
        {
            return !m_bOffline;
        }

        private int m_nFaultCount = 0;

        public KPXLeakDetector(int nID, int nStartAddress, int EndAddress, LeakDetectRegister[] aUnits)
        {
            m_nID = nID;
            m_nLastUnit = EndAddress;
            m_nStartUnit = nStartAddress;
            int nLength = aUnits.Length;

            for(int i = 0 ; i < aUnits.Length ; i++)
            {
                aUnits[i].Value = -999;
                m_nStatusFunction = aUnits[i].Function;
            }
            m_Units = aUnits;
        }

        public void CheckValue(SerialManager sm, int nTimeOut)
        {
            if (sm.IsConnected == true)
            {
                sm.ClearBuffer();


                byte nHeader = 0;
                byte[] data = MsgHelper.MakeData((byte)0, 0, (byte)m_nStartUnit, (byte)m_nLastUnit, (byte)m_nFunction, (byte)m_nSlaveID, out nHeader);
                sm.SendBytes(data); // value check

                int nCount = 0;
                while (sm.ReciverData == false)
                {
                    nCount++;
                    if (nCount == 7)
                        break;
                    Thread.Sleep(3);
                }

                if (sm.ReciverData == true)
                {
                    byte[] smReadBuffer = sm.ReciveBuffer;
                    sm.ReciverData = false;
                    if (smReadBuffer != null)
                    {
                        SetLevelValue(nHeader, smReadBuffer);                        
                    }
                    sm.ClearBuffer();
                    m_bOffline = false;
                    m_nFaultCount = 0;
                }
                else
                {
                    m_nFaultCount++;

                    if( m_nFaultCount == 3)
                    {
                        m_nFaultCount = 0;

                        if( m_Units != null)
                        {
                            for (int i = 0; i < m_Units.Length; i++)
                            {
                                m_Units[i].Value = -999;
                            }
                        }
                        
                        m_bOffline = true;
                    }                   
                }     
            }           
        }

        public void CheckValue(NetworkManager sm, int nTimeOut)
        {
            if (sm.IsConnected == true)
            {
                sm.ClearBuffer();

                byte nHeader = 0;
                 
                byte[] start = BitConverter.GetBytes(m_nStartUnit);
                byte[] length = BitConverter.GetBytes((short)(m_nLastUnit - m_nStartUnit));                 
                byte[] data = MsgHelper.MakeLongCmdReadRegisterData((byte)start[1], (byte)start[0], length[1], length[0], (byte)m_nSlaveID, out nHeader); 

                System.Diagnostics.Trace.WriteLine(m_szName);

                sm.SendBytes(data); // value check

                int nCount = 0;
                while (sm.ReciverData == false)
                {
                    nCount++;
                    if (nCount == 10)
                        break;
                    Thread.Sleep(200);
                }
                 
                /*                
                byte[] sign1 = { 0x04, 0x00, 0x00, 0x00, 0x00, 0x97, 0x01, 0x04, 0x94, 0x45, 0xBA, 0x47, 0xFF, 0x41, 0xB9, 0x99, 0x99, 0x3F, 0xAF, 0x82, 0xFF, 0x3F, 0xAE, 0x2F, 0x22, 0x3F, 0x8C, 0x02, 0x55, 0x00, 0x00, 0x00, 0x00, 0x45, 0x98, 0xE7, 0xFF, 0x42, 0x0A, 0x66, 0x66, 0x3F, 0x90, 0x01, 0xBB, 0x3F, 0x8E, 0x94, 0x44, 0x3F, 0xAE, 0x72, 0x11, 0xBB, 0x2C, 0x34, 0xCC, 0x45, 0xE8, 0xB7, 0xFF, 0x41, 0xB8, 0xCC, 0xCC, 0x3F, 0x93, 0xA1, 0xCC, 0x3F, 0x92, 0x95, 0x99, 0x3F, 0x6E, 0x60, 0x11, 0x00, 0x00, 0x00, 0x00, 0x46, 0x51, 0x40, 0x00, 0x41, 0xB9, 0x99, 0x99, 0x40, 0x04, 0x65, 0xFF, 0x40, 0x03, 0x70, 0xCC, 0x3F, 0xD8, 0x09, 0x11, 0x00, 0x00, 0x00, 0x00, 0x41, 0xA0, 0x00, 0x00, 0x41, 0xB4, 0x00, 0x00, 0x3C, 0x54, 0x1B, 0x77, 0x3C, 0x52, 0xBC, 0xDD, 0x3C, 0x2C, 0xBD, 0xDD, 0x00, 0x00, 0x00, 0x00, 0x45, 0xF8, 0x00, 0x00, 0xC3, 0x96, 0x00, 0x00, 0x40, 0x2D, 0xD7, 0xCC, 0x40, 0x52, 0xF1, 0xBB, 0x40, 0x40, 0x15, 0xFF, 0x3B, 0x83, 0x3D, 0xAA, 0x45, 0x9D, 0x70, 0x00 };
                byte[] sign4 = { 0x03, 0x00, 0x00, 0x00, 0x00, 0x6B, 0x01, 0x04, 0x68, 0x3E, 0xAF, 0x9D, 0xBB, 0x41, 0xC7, 0xFF, 0xFF, 0x42, 0x45, 0x22, 0xDD, 0x42, 0x45, 0x22, 0xDD, 0x42, 0xB4, 0x7A, 0x55, 0x00, 0x00, 0x00, 0x00, 0x3E, 0xA8, 0x72, 0xBB, 0x41, 0x4F, 0xFF, 0xFF, 0x42, 0x36, 0xF0, 0xAA, 0x42, 0x36, 0xF0, 0xAA, 0x42, 0xA8, 0x94, 0x44, 0x00, 0x00, 0x00, 0x00, 0x40, 0xE6, 0x1C, 0xAA, 0x41, 0xD4, 0x00, 0x00, 0x45, 0x1A, 0xE1, 0xCC, 0x45, 0x1A, 0xE1, 0xCC, 0x45, 0x8D, 0xAD, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x45, 0xBA, 0x47, 0xFF, 0x41, 0xB9, 0x99, 0x99, 0x3F, 0xAF, 0x82, 0xFF, 0x3F, 0xAE, 0x2F, 0x22, 0x3F, 0x8C, 0x02, 0x55, 0x00, 0x00, 0x00, 0x00 };
                byte[] sign3 = { 0x02, 0x00, 0x00, 0x00, 0x00, 0x97, 0x01, 0x04, 0x94, 0x46, 0x0E, 0x8B, 0xFF, 0x41, 0xBC, 0xCC, 0xCC, 0x3F, 0x51, 0xA0, 0x66, 0x3F, 0x4F, 0xF9, 0xCC, 0x3F, 0x27, 0xC6, 0x22, 0x00, 0x00, 0x00, 0x00, 0x46, 0x2A, 0x23, 0xFF, 0x41, 0xBC, 0xCC, 0xCC, 0x3F, 0x8D, 0xA0, 0x66, 0x3F, 0x8C, 0x87, 0x55, 0x3F, 0x64, 0x87, 0x22, 0xBA, 0x9D, 0xCA, 0xEE, 0x43, 0xFB, 0x00, 0x00, 0x41, 0xC0, 0xCC, 0xCC, 0x3C, 0xDB, 0x27, 0x00, 0x3C, 0xD9, 0x53, 0x66, 0x3C, 0xAF, 0x51, 0x11, 0x00, 0x00, 0x00, 0x00, 0x45, 0x93, 0xC0, 0x00, 0x41, 0xDB, 0x33, 0x33, 0x3F, 0x45, 0x90, 0xDD, 0x3F, 0x45, 0x90, 0xDD, 0x3F, 0xB4, 0xA2, 0x66, 0x00, 0x00, 0x00, 0x00, 0x44, 0x14, 0x80, 0x00, 0x41, 0xC7, 0xFF, 0xFF, 0x3D, 0xB1, 0xD3, 0xEE, 0x3D, 0xB1, 0xD3, 0xEE, 0x3E, 0x22, 0xCD, 0x22, 0x00, 0x00, 0x00, 0x00, 0x43, 0xAB, 0x7F, 0xFF, 0x41, 0xC7, 0xFF, 0xFF, 0x3D, 0x49, 0xDE, 0x00, 0x3D, 0x49, 0xDE, 0x00, 0x3D, 0xB8, 0xCF, 0x33, 0x00, 0x00, 0x00, 0x00, 0x43, 0xA4, 0x7F, 0xFF };
                byte[] sign2 = { 0x01, 0x00, 0x00, 0x00, 0x00, 0x97, 0x01, 0x04, 0x94, 0x45, 0x9D, 0x70, 0x00, 0x41, 0xCB, 0x33, 0x33, 0x3F, 0x85, 0x88, 0xFF, 0x3F, 0x84, 0x88, 0x22, 0x3F, 0x7B, 0x55, 0x66, 0x00, 0x00, 0x00, 0x00, 0x46, 0x16, 0x68, 0x00, 0x41, 0xC0, 0x00, 0x00, 0x3F, 0xCA, 0xDB, 0x99, 0x3F, 0xC9, 0x2F, 0x88, 0x3F, 0xA2, 0x4B, 0xEE, 0x00, 0x00, 0x00, 0x00, 0x45, 0x99, 0x07, 0xFF, 0x41, 0xBF, 0x33, 0x33, 0x3F, 0x4E, 0x31, 0xDD, 0x3F, 0x4B, 0xB7, 0x99, 0x3F, 0x87, 0xE3, 0xBB, 0x00, 0x00, 0x00, 0x00, 0x45, 0x7F, 0x6F, 0xFF, 0x41, 0xBC, 0xCC, 0xCC, 0x3E, 0xA9, 0xD1, 0xDD, 0x3E, 0xA8, 0x6F, 0xEE, 0x3E, 0x85, 0xA7, 0xAA, 0xBA, 0x78, 0xD3, 0x99, 0x45, 0xBF, 0xC7, 0xFF, 0x41, 0x94, 0xCC, 0xCC, 0x3F, 0x07, 0x27, 0x88, 0x3F, 0x06, 0xD2, 0xDD, 0x3F, 0x05, 0xD3, 0x66, 0x00, 0x00, 0x00, 0x00, 0x45, 0x85, 0xD0, 0x00, 0x41, 0xBC, 0xCC, 0xCC, 0x3E, 0xC3, 0xFE, 0x33, 0x3E, 0xC1, 0xB7, 0xBB, 0x3F, 0x01, 0x38, 0x22, 0x00, 0x00, 0x00, 0x00, 0x46, 0x0E, 0x8B, 0xFF };

                byte[] sign = null;
                if (m_nSlaveID == 1)
                    sign = sign1;
                if (m_nSlaveID == 2)
                    sign = sign2;
                if (m_nSlaveID == 3)
                    sign = sign3;
                if (m_nSlaveID == 4)
                    sign = sign4;

                SetLevelValue(nHeader, sign);
                return;
                */
                 
                if (sm.ReciverData == true)
                {
                    byte[] smReadBuffer = sm.ReciveBuffer;
                    sm.ReciverData = false;
                    if (smReadBuffer != null)
                    {
                        SetLevelValue(nHeader, smReadBuffer);
                    }
                    sm.ClearBuffer();
                    m_bOffline = false;
                    m_nFaultCount = 0;
                }
                else
                {
                    m_nFaultCount++;

                    if (m_nFaultCount == 2)
                    {
                        m_nFaultCount = 0;
                        if (m_Units != null)
                        {
                            for (int i = 0; i < m_Units.Length; i++)
                            {
                                m_Units[i].Value = -999;
                            }
                        }

                        m_bOffline = true;
                    }
                }
            }
        }

        // Write Single Register 0x06
        public void SendValue(NetworkManager sm, int nTimeOut, int address, int writeType, int value)
        {
            if (sm.IsConnected == true)
            {
                sm.ClearBuffer();

                byte nHeader = 0;
                byte[] start = BitConverter.GetBytes(writeType);
                byte[] regValue = BitConverter.GetBytes(value);
                byte[] data = MsgHelper.MakeCmdWriteSingle((byte)address, (byte)6, (byte)start[1], (byte)start[0], (byte)regValue[1], (byte)regValue[0]);
                 
                //System.Diagnostics.Trace.WriteLine(m_szName);

                sm.SendBytes(data); // value check 
            }
        }
        
        public void SetOff()
        {
            if (m_Units != null)
            {
                for (int i = 0; i < m_Units.Length; i++)
                {
                    m_Units[i].Value = -999;
                }
            }

            if (m_Units != null)
            {
                for (int i = 0; i < m_Units.Length; i++)
                {
                    LeakDetectRegister unit = m_Units[i];
                    unit.SetOff();
                }
            }
            m_bOffline = true;
        }

        private void SetLevelValue(byte nHeader, byte[] bStatus)
        { 
            ////Header            Ad FC Cnt
            ////01 00 00 00 00 06 01 04 02 00 240
            ////DD 00 00 00 00 11 01 04 0E 01 F0 00 00 02 12 02 E6 02 D2 03 9A 03 9A
            int nLength = bStatus[8];
            int length = this.DataLength;

            // Leak 상태 정보만 저장한다 (다른 정보는 log만 찍으면 됨)
            //30001: ID-- 기기 아이디
            //30002: Leak--누출 상태
            //30003: Broken-- 단선기능 사용 안함
            //30004: Alarm - set - Percent--  설정된 전류 감도
            //30005: Current - Reading - Percent-- 현재 검출된 전류량(ADC값에 대한 백분율로 표시됨)
            int nUnit = 0;            
            LeakDetectRegister aUnit = m_Units[nUnit];
            if (aUnit != null && aUnit.Name.Contains("LBSM")) 
            {
                if (aUnit.ValueType.ToLower() == "int")
                {
                    byte[] temp = new byte[2];
                    temp[1] = bStatus[11];
                    temp[0] = bStatus[12];

                    //11110000
                    //00000001
                    //--------
                    //00000000

                    //11110001
                    //00000001
                    //--------
                    //00000001

                    if ((temp[0] & 1) == 1)
                    {
                        // Leak 발생
                        aUnit.Value = 1;
                    }
                    else if ((temp[0] & 1) == 0)
                    {
                        // Leak 아님
                        aUnit.Value = 0;
                    }

                    System.Diagnostics.Trace.WriteLine(aUnit.Name + " Leak Status : " + aUnit.Value);
                }
            }
            
            //int nUnit = 0;
            //for (int i = 0; i < nLength; i += length)
            //{
            //    if (nUnit >= m_Units.Length)
            //        break;
              
            //    LeakDetectRegister aUnit = m_Units[nUnit];
            //    if (aUnit != null && aUnit.Name.Contains("LBSM")) // Leak 여부 정보만 저장한다 (다른 정보는 log만 찍으면 됨)
            //    {                    
            //        if (aUnit.ValueType.ToLower() == "int")
            //        {
            //            byte[] temp = new byte[2];
            //            temp[1] = bStatus[i + 9];
            //            temp[0] = bStatus[i + 10];

            //            //11110000
            //            //00000001
            //            //--------
            //            //00000000

            //            //11110001
            //            //00000001
            //            //--------
            //            //00000001

            //            if ((temp[0] & 1) == 1) 
            //            {
            //                // Leak 발생
            //                aUnit.Value = 1;
            //            }
            //            else if ((temp[0] & 1) == 0)
            //            {
            //                // Leak 아님
            //                aUnit.Value = 0;
            //            }

            //            System.Diagnostics.Trace.WriteLine(aUnit.Name + " Leak Status : " + aUnit.Value);
            //        }
            //        //else if(aUnit.ValueType.ToLower() == "float")
            //        //{
            //        //    byte[] temp = new byte[4];
            //        //    temp[3] = bStatus[i + 9];
            //        //    temp[2] = bStatus[i + 10];
            //        //    temp[1] = bStatus[i + 11];
            //        //    temp[0] = bStatus[i + 12];
                        
            //        //    float ratio = aUnit.RatioF;
            //        //    if (ratio == 0.0f)
            //        //        ratio = 1.0f;

            //        //    float value = BitConverter.ToSingle(temp, 0) / ratio;
            //        //    aUnit.Value = value;

            //        //    string szTemp3 = string.Format("{0}", value);
            //        //    System.Diagnostics.Trace.WriteLine(aUnit.Name + " Value : " + szTemp3);
            //        //} 
            //        //else if(aUnit.ValueType.ToLower() == "decimal")
            //        //{
            //        //    byte[] temp = new byte[2];
            //        //    temp[1] = bStatus[i + 9];
            //        //    temp[0] = bStatus[i + 10];
 
            //        //    int ratio = aUnit.Ratio;
            //        //    if (ratio == 0)
            //        //        ratio = 1;

            //        //    int value = BitConverter.ToInt32(temp, 0) / ratio;
            //        //    aUnit.Value = value;

            //        //    string szTemp3 = string.Format("{0}", value);
            //        //    System.Diagnostics.Trace.WriteLine(aUnit.Name + " Value : " + szTemp3);
            //        //}
            //    }
            //    nUnit++;
            //}
        }

        public float GetValue(int nUnit)
        {
            if (m_Units == null)
                return -999;
            if (nUnit < 0 || m_Units.Length <= nUnit)
                return -999;

            return m_Units[nUnit].Value;
        }

        public int GetStatus(int nUnit)
        {
            if (m_Units == null)
                return -1;
            if (nUnit < 0 || m_Units.Length <= nUnit)
                return -1;

            LeakDetectRegister aUnit = m_Units[nUnit];
            return Convert.ToInt32(aUnit.Value);
            //return aUnit.GetStatus();
        }

        public int DataLength { get; set; }
    }


    internal class LeakDetectRegister
    {
        private int m_nValue = 0;
        public int Value
        {
            get { return m_nValue; }
            set { m_nValue = value; }
        }

        public int GetStatus()
        {
            return 0;
        }

        public void SetOff()
        {

        }

        public LeakDetectRegister(int nID, int nAddress, int nFunction)
        {
            m_nID = nID;
            m_nFunction = nFunction;
            m_nAddress = nAddress;
        }
        
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
        }

        private int m_nFunction = -1;
        public int Function
        {
            get { return m_nFunction; }
        }

        private int m_nAddress = -1;
        public int Address
        {
            get { return m_nAddress; }
        }


        private string m_szName = "LeakDetect";
        public string Name
        {
          get { return m_szName; }
          set { m_szName = value; }
        }

        private int m_nLowCut = -1;
        public int LowCut
        {
          get { return m_nLowCut; }
          set { m_nLowCut = value; }
        }

        private int m_nHiCut = -1;
        public int HiCut
        {
          get { return m_nHiCut; }
          set { m_nHiCut = value; }
        }

        private int m_nDelayTime = -1;
        public int DelayTime
        {
          get { return m_nDelayTime; }
          set { m_nDelayTime = value; }
        }

        private int m_nValueTime = 500;
        public int ValueTime
        {
            get { return m_nValueTime; }
            set { m_nValueTime = value; }
        }

        private int m_nRatio = -1;
        public int Ratio
        {
            get { return m_nRatio; }
            set { m_nRatio = value; }
        }

        private float m_fRatio = -1;
       
        public float RatioF
        {
            get { return m_fRatio; }
            set { m_fRatio = value; }
        }

        private string m_valueType = "Decimal";
        public string ValueType 
        {
            get { return m_valueType; }
            set { m_valueType = value; }
        }
    }

    public enum WriteSingleType
    {
        None,
        // 0 : Reset
        Reset = 0x0000,
        // Relay & 알람 설정 모드 [ 0 = leak 발생시 '1' = cable 이상 or leak시 ]
        RealyNAlarmConfigMode = 0x0007,
        // Buzzer 강제 알람 상태(정지) [ '1' = OFF '0' = 복귀 ]
        BuzzerAlarmStatus = 0x0008,
        // Leak 발생 후 정상 상태가 되면 자동 복귀할지를 결정하는 MODE, Auto Reset이 아니면 외부 Reset '0'=AUTO '1'=Manual
        ResetMode = 0x0009
    }
}
