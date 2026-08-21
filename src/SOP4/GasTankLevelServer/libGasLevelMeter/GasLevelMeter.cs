using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GasDetector
{
    internal class GasLevelMeter
    {
        LevelMeter[] m_Units = null;

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

        public GasLevelMeter(int nID, int nStartAddress, int EndAddress, LevelMeter[] aUnits)
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
                byte[] data = MsgHelper.MakeData((byte)0, 0, 0, (byte)m_nLastUnit, (byte)m_nFunction, (byte)m_nSlaveID, out nHeader);
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
                byte[] data = MsgHelper.MakeData((byte)0, 0, 0, (byte)m_nLastUnit, (byte)m_nFunction, (byte)m_nSlaveID, out nHeader);

                System.Diagnostics.Trace.WriteLine(m_szName);

                sm.SendBytes(data); // value check

                int nCount = 0;
                while (sm.ReciverData == false)
                {
                    nCount++;
                    if (nCount == 5)
                        break;
                    Thread.Sleep(100);
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

                    if (m_nFaultCount == 3)
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
                    LevelMeter unit = m_Units[i];
                    unit.SetOff();
                }
            }
            m_bOffline = true;
        }

        private void SetLevelValue(byte nHeader, byte[] bStatus)
        {
            //04 00 00 00 00 09 01 04 06 00 00 00 53 00 A7
            // 01 00 00 00 00 07 02 04 04 00 01 00 20
            int nLength = bStatus[8];

            int nUnit = 0;
            for (int i = 0; i < nLength; i += 2)
            {
                byte[] temp = new byte[2];
                temp[1] = bStatus[i+9];
                temp[0] = bStatus[i+10];

                LevelMeter aUnit = m_Units[nUnit];
                if( aUnit != null)
                {
                    int ratio = aUnit.Ratio;
                    if (ratio == 0)
                        ratio = 1;

                    int value = BitConverter.ToInt16(temp, 0) / ratio;
                    if (value < 0)
                        value = 0;

                    aUnit.Value = value;
                    nUnit++;

                    System.Diagnostics.Trace.WriteLine("Value : " + value);
                }              
            }
        }

        public int GetValue(int nUnit)
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

            LevelMeter aUnit = m_Units[nUnit];
            return aUnit.GetStatus();
        }
    }


    internal class LevelMeter
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

        public LevelMeter(int nID, int nAddress, int nFunction)
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


        private string m_szName = "LevelMeter";
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
    }  
}
