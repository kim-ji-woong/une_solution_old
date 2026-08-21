using FireSensorServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireSensorServer.Network
{
    public class SensorDetector
    {
        /// <summary>
        /// Detector들을 순회하며 값을 가져오는 Thread
        /// </summary>
        private Thread m_CheckThread = null;
        private bool m_bExitThread = false;

        private int m_nFunction = 4;
        public int Function
        {
            get { return m_nFunction; }
            set { m_nFunction = value; }
        }

        private int m_nSlaveID = 1;
        public int SlaveID
        {
            get { return m_nSlaveID; }
            set { m_nSlaveID = value; }
        }

        //private int m_nLastUnit = 0;
        private int m_nStartUnit = 0;

        private bool m_bOffline = true;
        public bool IsOnline()
        {
            return !m_bOffline;
        }

        private int m_nFaultCount = 0;
        private List<DetectRegister> m_Detectors = new List<DetectRegister>();
        public List<DetectRegister> Detectors
        {
            get { return m_Detectors; }
            set { m_Detectors = value; }
        }

        public SensorDetector(int nStartAddress/*, int EndAddress*/)
        {
            //m_nLastUnit = EndAddress;
            m_nStartUnit = nStartAddress;
        }

        public void CheckValue(NMuxNetworkManager manager, int nTimeOut)
        {
            if (manager.IsConnected)
            {
                manager.ClearBuffer();

                byte nHeader = 0;

                byte[] start = BitConverter.GetBytes(m_nStartUnit);
                byte[] length = BitConverter.GetBytes((short)(2)); // 2개의 레지스터를 읽는다.
                byte[] data = MsgHelper.MakeLongCmdReadRegisterData(
                    (byte)start[1], (byte)start[0], length[1], length[0], (byte)m_nSlaveID, out nHeader);

                //System.Diagnostics.Trace.WriteLine(m_szName);

                manager.SendBytes(data); // value check

                int nCount = 0;
                while (manager.ReciverData == false)
                {
                    nCount++;
                    if (nCount == 5) // 줄일 수 있으면 줄이기
                        break;
                    Thread.Sleep(200);
                }

                if (manager.ReciverData == true)
                {
                    byte[] smReadBuffer = manager.ReciveBuffer;
                    manager.ReciverData = false;
                    if (smReadBuffer != null)
                    {
                        SetLevelValue(nHeader, smReadBuffer);
                    }
                    manager.ClearBuffer();
                    m_bOffline = false;
                    m_nFaultCount = 0;
                }
                else
                {
                    m_nFaultCount++;

                    if (m_nFaultCount == 2)
                    {
                        m_nFaultCount = 0;
                        if (m_Detectors != null)
                        {
                            for (int i = 0; i < m_Detectors.Count; i++)
                            {
                                m_Detectors[i].Value = -999;
                            }
                        }

                        m_bOffline = true;
                    }
                }
            }
        }

        private void SetLevelValue(byte nHeader, byte[] bStatus)
        {
            ////Header            Ad FC Cnt
            ////01 00 00 00 00 06 01 04 02 00 240
            ////DD 00 00 00 00 11 01 04 0E 01 F0 00 00 02 12 02 E6 02 D2 03 9A 03 9A
            int nLength = bStatus[2];
            int length = this.DataLength;

            int nUnit = 0;
            for (int i = 0; i < nLength; i+=4)
            {
                DetectRegister aUnit = m_Detectors[nUnit];
                if (aUnit != null)
                {
                    byte[] temp = new byte[4];
                    temp[3] = bStatus[i + 3];
                    temp[2] = bStatus[i + 4];
                    temp[1] = bStatus[i + 5];
                    temp[0] = bStatus[i + 6];

                    if (nUnit >= m_Detectors.Count)
                        break;

                    System.Diagnostics.Trace.WriteLine(aUnit.SensorTagInfo.SensorName + " Status : " + aUnit.Value);
                }
            }

            
            //DetectRegister aUnit = m_Detectors[nUnit];
            //if (aUnit != null)
            //{
            //    byte[] temp = new byte[2];
            //    temp[1] = bStatus[11];
            //    temp[0] = bStatus[12];

            //    //11110000
            //    //00000001
            //    //--------
            //    //00000000

            //    //11110001
            //    //00000001
            //    //--------
            //    //00000001

            //    if ((temp[0] & 1) == 1)
            //    {
            //        // 화재 발생
            //        aUnit.Value = 1;
            //    }
            //    else if ((temp[0] & 1) == 0)
            //    {
            //        // 화재 아님
            //        aUnit.Value = 0;
            //    }

                
            //}
        }

        public float GetValue(int nUnit)
        {
            if (m_Detectors == null)
                return -999;
            if (nUnit < 0 || m_Detectors.Count <= nUnit)
                return -999;

            return m_Detectors[nUnit].Value;
        }

        public int GetStatus(int nUnit)
        {
            if (m_Detectors == null)
                return -1;
            if (nUnit < 0 || m_Detectors.Count <= nUnit)
                return -1;

            DetectRegister aUnit = m_Detectors[nUnit];
            return Convert.ToInt32(aUnit.Value);
            //return aUnit.GetStatus();
        }

        public int DataLength { get; set; }

        public void SetOff()
        {
            if (m_Detectors != null)
            {
                for (int i = 0; i < m_Detectors.Count; i++)
                {
                    m_Detectors[i].Value = -999;
                }
            }

            //if (m_Detectors != null)
            //{
            //    for (int i = 0; i < m_Detectors.Count; i++)
            //    {
            //        DetectRegister unit = m_Detectors[i];
            //        unit.SetOff();
            //    }
            //}
            m_bOffline = true;
        }
    }

    public class DetectRegister
    {
        private int m_nValue = 0;
        public int Value
        {
            get { return m_nValue; }
            set { m_nValue = value; }
        }

        private SensorInfo m_sensorInfo = null;
        public SensorInfo SensorTagInfo
        {
            get { return m_sensorInfo; }
            set { m_sensorInfo = value; }
        }

        private int m_nAddress = -1;
        public int Address
        {
            get { return m_nAddress; }
        }
                
        public DetectRegister(int nAddress)
        {
            m_nAddress = nAddress;
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
    }
}
