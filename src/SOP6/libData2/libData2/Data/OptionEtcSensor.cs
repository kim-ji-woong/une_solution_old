using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace UnE.Sensor
{
    public class OptionEtcData
    {
        // 최소값 : 최소 ~ 이상
        private VariousData<int> m_minDatai = null;
        private VariousData<float> m_minDataf = null;
        private string m_minDatas = null;
        // 최대값 : 최대 ~ 미만
        private VariousData<int> m_maxDatai = null;
        private VariousData<float> m_maxDataf = null;
        private string m_maxDatas = null;
        private int m_nAlarmDepth = 0;
        // SDMS에게 알람신호를 전달할 것인가?
        private bool m_sendSDMS = false;
        // SOP를 실행시킬 것인가?
        private bool m_runSOP = false;
        // 연결된 SOP 전체경로
        private string m_strLinkedSOP = "";
        private List<int> m_linkedBuildingIDs = null;
        private List<int> m_linkedZoneIDs = null;

        public int AlarmDepth
        {
            get { return m_nAlarmDepth; }
            set { m_nAlarmDepth = value; }
        }

        public VariousData<int> MinDatai
        {
            get { return m_minDatai; }
            set { m_minDatai = value; }
        }

        public VariousData<int> MaxDatai
        {
            get { return m_maxDatai; }
            set { m_maxDatai = value; }
        }

        public VariousData<float> MinDataf
        {
            get { return m_minDataf; }
            set { m_minDataf = value; }
        }

        public VariousData<float> MaxDataf
        {
            get { return m_maxDataf; }
            set { m_maxDataf = value; }
        }

        public string MinDatas
        {
            get { return m_minDatas; }
            set { m_minDatas = value; }
        }

        public string MaxDatas
        {
            get { return m_maxDatas; }
            set { m_maxDatas = value; }
        }

        public bool SendSDMS
        {
            get { return m_sendSDMS; }
            set { m_sendSDMS = value; }
        }

        public bool RunSOP
        {
            get { return m_runSOP; }
            set { m_runSOP = value; }
        }

        public string LinkedSOP
        {
            get { return m_strLinkedSOP; }
            set { m_strLinkedSOP = value; }
        }

        // 이 값이 null이면 사용하지 않는다.
        // LinkedZoneIDs까지 둘다 null이면 모든 건물이나 Zone에 적용된다.
        public List<int> LinkedBuildingIDs
        {
            get { return m_linkedBuildingIDs; }
            set { m_linkedBuildingIDs = value; }
        }

        // 이 값이 null이면 사용하지 않는다.
        // LinkedBuildingIDs까지 둘다 null이면 모든 건물이나 Zone에 적용된다.
        public List<int> LinkedZoneIDs
        {
            get { return m_linkedZoneIDs; }
            set { m_linkedZoneIDs = value; }
        }

        public static List<int> ToIDList(string strIDs)
        {
            if (strIDs == null)
                return null;

            string[] tokens = strIDs.Trim().Split(',');
            List<int> ids = new List<int>();
            int nID;

            foreach (string strToken in tokens)
            {
                string strData = strToken.Trim();

                if (strData.Length == 0)
                    continue;

                if (int.TryParse(strData, out nID) == false)
                    return null;
                else
                    ids.Add(nID);
            }

            return ids;
        }
    }

    public class OptionEtcSensor
    {
        public enum DataType { None = -1, IntType = 0, FloatType, StringType };

        // Key : 알람단계
        private Dictionary<int, OptionEtcData> m_dicDatas = new Dictionary<int, OptionEtcData>();
        private IFacility.FacilityType m_sensorType = IFacility.FacilityType.NONE;
        private DataType m_dataType = DataType.None;
        // 알람을 발생시키는 가장 낮은 수치
        private VariousData<int> m_minAlarmDatai = null;
        private VariousData<float> m_minAlarmDataf = null;
        private string m_minAlarmDatas = null;
        // 센서값이 최소값 미만으로 m_closeAlarmSeconds 초 이상 지속되면 알람을 종료시킨다.
        private VariousData<int> m_closeAlarmSeconds = null;
        // 같은 위험단계에 대한 알람은 연속해서 보내지 않도록 한다.
        // 적어도 m_delaySeconds 만큼은 지난 다음에 같은 단계 데이터를 보낸다.(null이 아닐 경우)
        private VariousData<int> m_delaySeconds = null;

        public IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }

        public DataType SensorDataType
        {
            get { return m_dataType; }
            set { m_dataType = value; }
        }

        public VariousData<int> MinAlarmDatai
        {
            get { return m_minAlarmDatai; }
        }

        public VariousData<float> MinAlarmDataf
        {
            get { return m_minAlarmDataf; }
        }

        public string MinAlarmDatas
        {
            get { return m_minAlarmDatas; }
        }

        // 센서값이 최소값 미만으로 m_nCloseAlarmSeconds 초 이상 지속되면 알람을 종료시킨다.
        public VariousData<int> CloseAlarmSeconds
        {
            get { return m_closeAlarmSeconds; }
            set { m_closeAlarmSeconds = value; }
        }

        // 같은 위험단계에 대한 알람은 연속해서 보내지 않도록 한다.
        // 적어도 m_delaySeconds 만큼은 지난 다음에 같은 단계 데이터를 보낸다.(null이 아닐 경우)
        public VariousData<int> DelaySeconds
        {
            get { return m_delaySeconds; }
            set { m_delaySeconds = value; }
        }

        public static bool ToDataType(int nDataType, out DataType type)
        {
            type = DataType.None;

            if (nDataType < (int)DataType.IntType ||
                nDataType > (int)DataType.StringType)
                return false;

            type = (DataType)nDataType;
            return true;
        }

        public void AddOptionData(int nAlarmDepth, OptionEtcData data)
        {
            m_dicDatas[nAlarmDepth] = data;

            if (data.MinDatai != null)
            {
                if (m_minAlarmDatai != null)
                {
                    if (m_minAlarmDatai.Data > data.MinDatai.Data)
                        m_minAlarmDatai = data.MinDatai;
                }
                else
                    m_minAlarmDatai = data.MinDatai;
            }

            if (data.MinDataf != null)
            {
                if (m_minAlarmDataf != null)
                {
                    if (m_minAlarmDataf.Data > data.MinDataf.Data)
                        m_minAlarmDataf = data.MinDataf;
                }
                else
                    m_minAlarmDataf = data.MinDataf;
            }

            if (data.MinDatas != null)
            {
                if (m_minAlarmDatas != null)
                {
                    int nResult = string.Compare(m_minAlarmDatas, data.MinDatas);

                    if (nResult > 0)
                        m_minAlarmDatas = data.MinDatas;
                }
                else
                    m_minAlarmDatas = data.MinDatas;
            }
        }

        public OptionEtcData GetOptionData(int nAlarmDepth)
        {
            OptionEtcData data;

            if (m_dicDatas.TryGetValue(nAlarmDepth, out data) == false)
                return data;

            return null;
        }

        private bool CheckPermit(OptionEtcData option, int nBuildingID, int nZoneID)
        {
            bool permit = option.LinkedBuildingIDs == null && option.LinkedZoneIDs == null;

            if (option.LinkedBuildingIDs != null)
            {
                permit = option.LinkedBuildingIDs.Contains(nBuildingID);
            }

            if (permit == false && option.LinkedZoneIDs != null)
            {
                permit = option.LinkedZoneIDs.Contains(nZoneID);
            }

            return permit;
        }

        public OptionEtcData GetData(int data, int nBuildingID = -1, int nZoneID = -1)
        {
            if (m_dataType != DataType.IntType)
                return null;

            foreach (KeyValuePair<int, OptionEtcData> pair in m_dicDatas)
            {
                OptionEtcData option = pair.Value;

                if (option.MinDatai == null || option.MaxDatai == null)
                    continue;

                if (CheckPermit(option, nBuildingID, nZoneID) == false)
                    continue;

                if (data >= option.MinDatai.Data && data < option.MaxDatai.Data)
                    return option;
            }

            return null;
        }

        public OptionEtcData GetData(float data, int nBuildingID = -1, int nZoneID = -1)
        {
            if (m_dataType != DataType.FloatType)
                return null;

            foreach (KeyValuePair<int, OptionEtcData> pair in m_dicDatas)
            {
                OptionEtcData option = pair.Value;

                if (option.MinDataf == null || option.MaxDataf == null)
                    continue;

                if (CheckPermit(option, nBuildingID, nZoneID) == false)
                    continue;

                if (data >= option.MinDataf.Data && data < option.MaxDataf.Data)
                    return option;
            }

            return null;
        }

        public OptionEtcData GetData(string data, int nBuildingID = -1, int nZoneID = -1)
        {
            if (m_dataType != DataType.StringType)
                return null;

            foreach (KeyValuePair<int, OptionEtcData> pair in m_dicDatas)
            {
                OptionEtcData option = pair.Value;

                if (option.MinDatas == null || option.MaxDatas == null)
                    continue;

                if (CheckPermit(option, nBuildingID, nZoneID) == false)
                    continue;

                int minResult = string.Compare(data, option.MinDatas);
                int maxResult = string.Compare(data, option.MaxDatas);

                if (minResult >= 0 && maxResult < 0)
                    return option;
            }

            return null;
        }

        public string GetMinimumData()
        {
            string strData = "";

            if (m_minAlarmDatai != null)
                strData = m_minAlarmDatai.Data.ToString();
            else if (m_minAlarmDataf != null)
                strData = string.Format("{0:F1}", m_minAlarmDataf.Data);
            else if (m_minAlarmDatas != null)
                strData = m_minAlarmDatas;
            else
                return "";

            string strUnit = GetUnitString(this.SensorType);
            return strData + strUnit;
        }

        public static string GetUnitString(IFacility.FacilityType type)
        {
            if (type == IFacility.FacilityType.STRONG_WIND)
                return "m/s";

            return "";
        }
    }
}
