using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasDetector
{
    internal class DetectorFactory
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

        private DetectorManager m_Main = null;
        
        public DetectorFactory(DetectorManager dm, ConfigFile file)
        {
            m_Main = dm;
            m_Conf = file;
        }

        private bool m_bReadContent = false;
        
        private List<GasDetector> m_List = new List<GasDetector>();
        
        private List<GasDetector> CreateDetector()
        {
            string szUnitNum = m_Conf.GetValue("DETECTOR Setting", "UNITNUM");
            if (!int.TryParse(szUnitNum, out m_nUnitNum))
            {
                m_nUnitNum = 0;
            }

            string szBeginAddress = m_Conf.GetValue("DETECTOR Setting", "ADDRBEGIN");
            if (!int.TryParse(szBeginAddress, out m_nBeginAddress))
            {
                m_nBeginAddress = 0;
            }

            string szEndAddress = m_Conf.GetValue("DETECTOR Setting", "ADDREND");
            if (!int.TryParse(szEndAddress, out m_nEndAddress))
            {
                m_nEndAddress = 0;
            }

            m_List.Clear();
            if( m_nUnitNum > 0)
            {

                for (int i = 0; i < m_nUnitNum; i++)
                {
                    string szComUnitName = "CU" + (i + 1).ToString();
                    string szUnitValues = m_Conf.GetValue("DETECTOR Setting", szComUnitName);

                    if (szUnitValues != null && szUnitValues.Length > 0)
                    {
                        string[] values = szUnitValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length >= 6)
                        {
                            // COMM ID, UNIT 개수, UNIT 시작 Address, UNIT 종료 Address, FUNCTION, Alarm Unit 설정
                            int nID = Convert.ToInt32(values[0]);

                            int nUnit = Convert.ToInt32(values[1]);
                            int nStartUnit = Convert.ToInt32(values[2]);
                            int nEndUnit = Convert.ToInt32(values[3]);
                            int nFunction = Convert.ToInt32(values[4]);
                            string szUnits = values[5];

                            AlarmUnit[] aUnits = ReadAlarmUnit(nUnit, nStartUnit, szUnits);

                            GasDetector detector = new GasDetector(nID, nStartUnit, nEndUnit, aUnits);
                            detector.Function = nFunction;

                            detector.OnNotifyAlarm += m_Main.OnAlarmNotify;
                            m_List.Add(detector);
                        }
                    }
                }
            }

            m_bReadContent = true;
            return m_List;
        }

        private AlarmUnit[] ReadAlarmUnit(int nUnit, int nStartAddress, string unitNames)
        {
            //AU1;AU2;AU3
            AlarmUnit[] result = new AlarmUnit[nUnit];

            string[] szUnitNames = unitNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < nUnit; i++)
            {
                string szUnitValues = m_Conf.GetValue("ALARMUNIT Setting", szUnitNames[i]);
                if (szUnitValues != null && szUnitValues.Length > 0)
                {
                    string[] values = szUnitValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values != null && values.Length >= 5)
                    {
                        int nID = nStartAddress + i;

                        //# 전체Chennel,시작Chennel, 종료Chennel, FUNCTION
                        int nCount = Convert.ToInt32(values[0]);
                        int nStartUnit = Convert.ToInt32(values[1]);
                        int nEndUnit = Convert.ToInt32(values[2]);
                        int nFunction = Convert.ToInt32(values[3]);
                        string szChNames = values[4];

                        ChannelInfo[] chs = CreateChannelInfo(nCount, nStartUnit, szChNames);

                        result[i] = new AlarmUnit(nID, nStartUnit, nEndUnit, chs);
                        result[i].Function = nFunction;
                        result[i].UnitName = szUnitNames[i];
                    }
                }                     
            }
            return result;
        }

        private ChannelInfo[] CreateChannelInfo(int nCount, int nStartUnit, string strChNames)
        {
            string[] szUnitNames = strChNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            ChannelInfo[] result = new ChannelInfo[nCount];
            for(int i = 0; i < nCount ; i++)
            {
                string szUnitValues = m_Conf.GetValue("CHANNEL Setting", szUnitNames[i]);
                if (szUnitValues != null && szUnitValues.Length > 0)
                {
                    string[] values = szUnitValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values != null && values.Length > 2)
                    {
                        //CH1=Alarm-1,1,1
                        result[i] = new ChannelInfo();
                        result[i].Channel = nStartUnit + i;
                        result[i].ChannelName = values[0];
                        result[i].EnableNotify = (Convert.ToInt32(values[1]) == 1 ? true : false);
                        result[i].TargetValue = Convert.ToInt32(values[2]);
                    }                    
                }                
            }
            return result;
        }       
        
        public List<GasDetector> CreateDetectorList()
        {
            return CreateDetector();
        }
     }
}
