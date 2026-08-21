using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using UnE.Sensor;

namespace SOPMonitoringSystem.Data
{
    public class EtcSensorManager
    {
        private class OptionEtcData
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
                get { return m_minDatai; }
                set { m_minDatai = value; }
            }

            public VariousData<float> MinDataf
            {
                get { return m_minDataf; }
                set { m_minDataf = value; }
            }

            public VariousData<float> MaxDataf
            {
                get { return m_minDataf; }
                set { m_minDataf = value; }
            }

            public string MinDatas
            {
                get { return m_minDatas; }
                set { m_minDatas = value; }
            }

            public string MaxDatas
            {
                get { return m_minDatas; }
                set { m_minDatas = value; }
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
        }

        private class OptionEtcSensor
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

            public OptionEtcData GetData(int data)
            {
                if (m_dataType != DataType.IntType)
                    return null;

                foreach (KeyValuePair<int, OptionEtcData> pair in m_dicDatas)
                {
                    if (pair.Value.MinDatai == null || pair.Value.MaxDatai == null)
                        continue;

                    if (data >= pair.Value.MinDatai.Data && data < pair.Value.MaxDatai.Data)
                        return pair.Value;
                }

                return null;
            }

            public OptionEtcData GetData(float data)
            {
                if (m_dataType != DataType.FloatType)
                    return null;

                foreach (KeyValuePair<int, OptionEtcData> pair in m_dicDatas)
                {
                    if (pair.Value.MinDataf == null || pair.Value.MaxDataf == null)
                        continue;

                    if (data >= pair.Value.MinDataf.Data && data < pair.Value.MaxDataf.Data)
                        return pair.Value;
                }

                return null;
            }

            public OptionEtcData GetData(string data)
            {
                if (m_dataType != DataType.StringType)
                    return null;

                foreach (KeyValuePair<int, OptionEtcData> pair in m_dicDatas)
                {
                    if (pair.Value.MinDatas == null || pair.Value.MaxDatas == null)
                        continue;

                    int minResult = string.Compare(data, pair.Value.MinDatas);
                    int maxResult = string.Compare(data, pair.Value.MaxDatas);

                    if (minResult >= 0 && maxResult < 0)
                        return pair.Value;
                }

                return null;
            }
        }

        private Dictionary<IFacility.FacilityType, OptionEtcSensor> m_optionSensorData = new Dictionary<IFacility.FacilityType, OptionEtcSensor>();
        private WebDBManager m_dbMgr = null;

        public EtcSensorManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            ReadOptionDatas(dbMgr);
        }

        private void ReadOptionDatas(WebDBManager dbMgr)
        {
            string strSQL = "Select sensor.SensorType, sensor.DataType, sensor.CloseAlarmSeconds, sensor.DelaySeconds, data.DataMini, data.DataMinf, data.DataMins, data.DataMaxi, data.DataMaxf, data.DataMaxs, data.AlarmDepth, data.SendSDMS ";//, data.RunSOP, data.LinkedSOP ";
            strSQL += "from OptionEtcSensor as sensor, OptionEtcSensorData as data where sensor.SensorType = data.SensorTypeID and sensor.SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> dataType = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> closeAlarmSeconds = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> delaySeconds = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> mini = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<float> minf = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                string mins = WebDBManager.GetStringField(arrResult[i + 6]);
                VariousData<int> maxi = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<float> maxf = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                string maxs = WebDBManager.GetStringField(arrResult[i + 9]);
                VariousData<int> alarmDepth = WebDBManager.GetIntField(arrResult[i + 10].ToString());
                VariousData<int> sendSDMS = WebDBManager.GetIntField(arrResult[i + 11].ToString());
                //VariousData<int> runSOP = WebDBManager.GetIntField(arrResult[i + 12].ToString());
                //string linkedSOP = WebDBManager.GetStringField(arrResult[i + 13]);

                if (sensorType == null || dataType == null || alarmDepth == null || sendSDMS == null/* || runSOP == null || linkedSOP == null*/)
                    continue;

                IFacility.FacilityType facilityType = IFacility.ToFacilityType(sensorType.Data);

                if (facilityType == IFacility.FacilityType.NONE)
                    continue;

                OptionEtcSensor.DataType type;

                if (OptionEtcSensor.ToDataType(dataType.Data, out type) == false)
                    continue;

                OptionEtcData data = new OptionEtcData();

                if (type == OptionEtcSensor.DataType.IntType)
                {
                    if (mini == null || maxi == null)
                        continue;

                    data.MinDatai = mini;
                    data.MaxDatai = maxi;
                }
                else if (type == OptionEtcSensor.DataType.FloatType)
                {
                    if (minf == null || maxf == null)
                        continue;

                    data.MinDataf = minf;
                    data.MaxDataf = maxf;
                }
                else if (type == OptionEtcSensor.DataType.StringType)
                {
                    if (mins == null || maxs == null)
                        continue;

                    data.MinDatas = mins;
                    data.MaxDatas = maxs;
                }

                data.AlarmDepth = alarmDepth.Data;
                //data.LinkedSOP = linkedSOP;
                //data.RunSOP = runSOP.Data == 1;
                data.SendSDMS = sendSDMS.Data == 1;

                OptionEtcSensor etcSensor = null;

                if (m_optionSensorData.TryGetValue(facilityType, out etcSensor) == false)
                {
                    etcSensor = new OptionEtcSensor();
                    m_optionSensorData[facilityType] = etcSensor;

                    etcSensor.CloseAlarmSeconds = closeAlarmSeconds;
                    etcSensor.SensorType = facilityType;
                    etcSensor.SensorDataType = type;
                }

                etcSensor.AddOptionData(alarmDepth.Data, data);
            }
        }

        /*public void ProcessEtcSensorDetect(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 8)
                return;

            if (arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is long && arrDatas[5] is int && arrDatas[6] is bool && arrDatas[7] is string)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSensorZoneHistoryID = (int)arrDatas[2];
                DateTime timeStamp = DateTime.FromBinary((long)arrDatas[3]);
                int nAlarmLevel = (int)arrDatas[5];
                bool runSOP = (bool)arrDatas[6];
                string strSOPPath = (string)arrDatas[7];
                string strSensorData = "";

                OptionEtcSensor optionData;
                IFacility.FacilityType facilityType = IFacility.ToFacilityType(nSensorType);

                if (m_optionSensorData.TryGetValue(facilityType, out optionData) == false)
                    return;

                if (optionData.SensorDataType == OptionEtcSensor.DataType.IntType && arrDatas[4] is int ||
                    optionData.SensorDataType == OptionEtcSensor.DataType.FloatType && arrDatas[4] is float ||
                    optionData.SensorDataType == OptionEtcSensor.DataType.StringType && arrDatas[4] is string)
                {
                    strSensorData = arrDatas[4].ToString().Trim();
                }
                else
                    return;

                if (facilityType == IFacility.FacilityType.STRONG_WIND)
                {
                    float fWindSpeed;

                    if (float.TryParse(strSensorData, out fWindSpeed))
                        StubWorker.Instance.OpenSOP_StrongWind(strSOPPath, timeStamp, nSensorZoneHistoryID, nSensorZoneID, nAlarmLevel, fWindSpeed, GetBuildingNameFromSensorZoneID(nSensorZoneID));
                }
            }
        }*/

        private string GetBuildingNameFromSensorZoneID(int nSensorZoneID)
        {
            string strSQL = "Select b.BuildingName from SensorZone as sz, Zone as z, Building as b where sz.Zone = z.ID and z.BuildingID = b.ID and sz.ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strBuildingName = WebDBManager.GetStringField(arrResult[0]);

            if (strBuildingName == null)
                strBuildingName = "";

            return strBuildingName;
        }
    }
}
