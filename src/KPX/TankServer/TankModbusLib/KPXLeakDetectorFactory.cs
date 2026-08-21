using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankModbusLib
{
    internal class KPXLeakDetectorFactory
    {
        private ConfigFile m_Conf = null;

        private int m_nUnitNum = 0;
        public int UnitNum
        {
            get { return m_nUnitNum; }
        }

        private int m_nBeginAddress = 0;
        public int BeginAddress
        {
            get { return m_nBeginAddress; }
        }
        private int m_nEndAddress = 0;
        public int EndAddress
        {
            get { return m_nEndAddress; }
        }

        private LeakDetectorManager m_Main = null;
        
        public KPXLeakDetectorFactory(LeakDetectorManager dm, ConfigFile file)
        {
            m_Main = dm;
            m_Conf = file;
        }

        private bool m_bReadContent = false;
        
        private List<KPXLeakDetector> m_List = new List<KPXLeakDetector>();
        
        private List<KPXLeakDetector> CreateDetector()
        {

            string szMode = m_Conf.GetValue("MODE Setting", "MODE");
            if (szMode == "")
                szMode = "TCP";
            string szSection = "SERIAL Setting";
            if( szMode == "SERIAL")
            {
                szSection = "SERIAL Setting";
            }
            else if(szMode == "TCP")
            {
                szSection = "TCP Setting";
            }

            string szUnitNum = m_Conf.GetValue(szSection, "UNITNUM");
            if (!int.TryParse(szUnitNum, out m_nUnitNum))
            {
                m_nUnitNum = 0;
            }

            m_List.Clear();
            if( m_nUnitNum > 0)
            {

                for (int i = 0; i < m_nUnitNum; i++)
                {
                    string szComUnitName = "CU" + (i + 1).ToString();
                    string szUnitValues = m_Conf.GetValue(szSection, szComUnitName);

                    if (szUnitValues != null && szUnitValues.Length > 0)
                    {
                        string[] values = szUnitValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length >= 6)
                        {
                            //COM4,38400,8,1,0,0,256,256

                            // COMM ID, UNIT 개수, UNIT 시작 Address, UNIT 종료 Address, FUNCTION, Alarm Unit 설정

                            int nID = (i + 1);
                            int nSlaveID = Convert.ToInt32(values[8]);

                            int nUnit = Convert.ToInt32(values[9]);
                            int nStartUnit = Convert.ToInt32(values[10]);
                            int nEndUnit = Convert.ToInt32(values[11]);
                            int nFunction = Convert.ToInt32(values[12]);
                            int nDataLength = Convert.ToInt32(values[13]);
                            string szUnits = values[14];

                            LeakDetectRegister[] aUnits = ReadLevelMeter(nUnit, nStartUnit, szUnits);

                            KPXLeakDetector detector = new KPXLeakDetector(nID, nStartUnit, nEndUnit, aUnits);                            
                            detector.Function = nFunction;
                            detector.Name = szComUnitName;
                            detector.SlaveID = nSlaveID;
                            detector.DataLength = nDataLength;
                            if( szMode == "SERIAL")
                            {
                                SerialManager sm = new SerialManager(m_Main, m_Conf);
                                sm.Port = values[0];
                                detector.Serial = sm;
                            }
                            else if(szMode == "TCP")
                            {
                                string szIP = values[0];
                                int nPort = Convert.ToInt32(values[1]);
                                                               
                                // NetworkManager nm = new NetworkManager(m_Main, m_Conf);
                                NetworkManager nm = NetworkManager.CreateNetworkManager(m_Main, m_Conf, szIP);
                                nm.IPAddress = szIP;
                                nm.PortNumber = nPort;
                                nm.id = detector.ID;
                                nm.SwitchNum = nSlaveID;
                                detector.NetworkMan = nm;
                            }                            
                            m_List.Add(detector);
                        }
                    }
                }
            }

            m_bReadContent = true;
            return m_List;
        }

        private LeakDetectRegister[] ReadLevelMeter(int nUnit, int nStartAddress, string unitNames)
        {
            LeakDetectRegister[] result = new LeakDetectRegister[nUnit];

            string[] szUnitNames = unitNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < nUnit; i++)
            {
                string szUnitValues = m_Conf.GetValue("LEAKDETECT Setting", szUnitNames[i]);
                if (szUnitValues != null && szUnitValues.Length > 0)
                {
                    string[] values = szUnitValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values != null && values.Length >= 5)
                    {
                        int nID = nStartAddress + i;

                        string szName = values[0];
                        int nLowCut = Convert.ToInt32(values[1]);
                        int nHiCut = Convert.ToInt32(values[2]);
                        int nDelayTime = Convert.ToInt32(values[3]);

                        string szType = values[4];
                        int nFunction = Convert.ToInt32(values[5]);
                        int nAddress = Convert.ToInt32(values[6]);

                        float fRatio = 1.0f;
                        int nRatio = 1;
                        if(szType.ToLower() == "float")
                        {
                            fRatio = Convert.ToSingle(values[7]);
                        }
                        else
                        {
                            nRatio = Convert.ToInt32(values[7]);
                        }                      

                        result[i] = new LeakDetectRegister(nID, nAddress, nFunction);
                        result[i].Name = szName;
                        result[i].LowCut = nLowCut;
                        result[i].HiCut = nHiCut;
                        result[i].ValueType = szType;
                        result[i].DelayTime = nDelayTime;
                        result[i].Ratio = nRatio;
                        result[i].RatioF = fRatio;
                    }
                }                     
            }
            return result;
        }          
        
        public List<KPXLeakDetector> CreateDetectorList()
        {
            return CreateDetector();
        }
     }
}
