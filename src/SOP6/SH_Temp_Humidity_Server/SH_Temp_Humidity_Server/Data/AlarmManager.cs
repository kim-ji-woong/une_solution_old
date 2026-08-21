using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DBUtility2;
using System.Collections;
using System.Configuration;
using System.Threading;
using UnE.Sensor;
using System.IO;

namespace SH_Temp_Humidity_Server.Data
{
    using Network;
    using Data;

    public class AlarmManager
    {
        public enum FloorPattern { None = 0, B_F, _F, 지하층, 층 };

        private DirectDBManagerEx m_dbMgr = null;
        private DirectDBManager m_dbTH = null;
        private Dictionary<int, List<Sensor>> m_dicFloorSensors = new Dictionary<int, List<Sensor>>();
        private Dictionary<int, AlarmType> m_dicAlarmTypes = new Dictionary<int, AlarmType>();
        private Dictionary<int, Sensor> m_dicSensors = new Dictionary<int, Sensor>();
        private Dictionary<string, AlarmData> m_dicAlarmDatas = new Dictionary<string, AlarmData>();

        private bool m_closeThread = true;
        private int m_nLastReadSequenceHistoryID = 0;
        private string m_strLastReadSequenceHistoryTime = "";
        private const string SequenceFileName = "sequence.txt";

        private NetworkWebClient m_webClient = null;
        private int m_nReceiverID = -1;

        private static AlarmManager m_instance = null;

        public static AlarmManager Instance
        {
            get { return m_instance; }
        }

        public int ReceiverID
        {
            get { return m_nReceiverID; }
        }

        public AlarmManager()
        {
            m_instance = this;

            string strIP = System.Configuration.ConfigurationManager.AppSettings.Get("ip");
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("site");
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("webServerURL");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = System.Configuration.ConfigurationManager.AppSettings.Get("dbType");
            string strTHDBName = System.Configuration.ConfigurationManager.AppSettings.Get("thDBName");
            string strTHDBInfo = System.Configuration.ConfigurationManager.AppSettings.Get("thInfo");
            string strOwnDBInfo = System.Configuration.ConfigurationManager.AppSettings.Get("ownInfo");

            int index = strWebServerURL.IndexOf("//");

            if (index > 0)
                strWebServerURL = strWebServerURL.Substring(index + 2).Trim();

            int nSiteID, nDBType;

            if (strSiteID != null && strSiteID.Length > 0 && int.TryParse(strSiteID, out nSiteID))
            {
                if (strWebServerURL != null && strWebServerURL.Length > 0 && strDBName != null && strDBName.Length > 0 && strDBType != null && strDBType.Length > 0 && int.TryParse(strDBType, out nDBType))
                {
                    string strID, strPW;
                    int ownType;

                    if (GetTHInfo(strOwnDBInfo, out strID, out strPW, out ownType))
                    {
                        DirectDBManager dbMgr = DirectDBManager.MakeInstance((DirectDBManager.DBType)(int)ownType, strWebServerURL, strID, strPW, strDBName);
                        dbMgr.SiteID = nSiteID;
                        m_dbMgr = new DirectDBManagerEx(dbMgr);
                    }

                    ReadReceiverInfo();
                }

                if (strIP != null && strIP.Length > 0 && strTHDBInfo != null && strTHDBInfo.Length > 0 && strTHDBName != null && strTHDBName.Length > 0)
                {
                    string strID, strPW;
                    int nTHDBType;

                    if (GetTHInfo(strTHDBInfo, out strID, out strPW, out nTHDBType))
                    {
                        m_dbTH = DirectDBManager.MakeInstance((DirectDBManager.DBType)nTHDBType, strIP, strID, strPW, strTHDBName);
                    }
                }
            }

            if (m_dbMgr != null)
                m_webClient = new NetworkWebClient(m_dbMgr);
        }

        private void ReadReceiverInfo()
        {
            string strSQL = "Select SensorServerID, SensorType from SensorTagInfo group by SensorServerID, SensorType having SensorType = " + (int)IFacility.FacilityType.TEMPERATURE_HUMIDITY;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id != null)
                m_nReceiverID = id.Data;
        }

        private bool GetTHInfo(string str, out string strID, out string strPW, out int nDBType)
        {
            strID = strPW = "";
            nDBType = 0;

            int nIndex1 = str.IndexOf('_');
            int nIndex2 = str.LastIndexOf('_');

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                return false;

            string strDBType = str.Substring(0, nIndex1);
            string strPW2 = str.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            string strID2 = str.Substring(nIndex2 + 1);

            int nIDLen = strID2.Length;
            int nPWLen = strPW2.Length;

            for (int i = nIDLen - 1;i >= 0;i--)
            {
                strID += strID2.ElementAt(i);
            }

            for (int i = nPWLen - 1; i >= 0; i--)
            {
                strPW += strPW2.ElementAt(i);
            }

            if (int.TryParse(strDBType, out nDBType))
                return true;

            return false;
        }

        public void Start()
        {
            if (m_dbMgr == null)
                return;

            if (ReadAlarmTypes() && ReadSensors() && ReadAlarmCode())
            {
                ReadSequence();

                Thread t = new Thread(new ThreadStart(CheckAlarmThread));
                t.Start();
            }
        }

        public void Stop()
        {
            WriterSequence();
            m_closeThread = true;

            if (m_webClient != null)
                m_webClient.Close();
        }

        private void ReadSequence()
        {
            if (File.Exists(SequenceFileName))
            {
                StreamReader reader = new StreamReader(SequenceFileName, Encoding.UTF8);
                string strLine = reader.ReadLine().Trim();
                reader.Close();

                int nIndex = strLine.IndexOf(' ');

                if (nIndex > 0)
                {
                    string strID = strLine.Substring(0, nIndex).Trim();
                    string strTime = strLine.Substring(nIndex + 1).Trim();

                    int.TryParse(strID, out m_nLastReadSequenceHistoryID);
                    m_strLastReadSequenceHistoryTime = strTime;
                }
            }
        }

        private void WriterSequence()
        {
            StreamWriter writer = new StreamWriter(SequenceFileName, false, Encoding.UTF8);
            writer.WriteLine(m_nLastReadSequenceHistoryID + " " + m_strLastReadSequenceHistoryTime);
            writer.Close();
        }

        private void CheckAlarmThread()
        {
            if (m_dbTH == null)
                return;

            m_closeThread = false;

            while (m_closeThread == false)
            {
                CheckAlarm();
                Thread.Sleep(1000);
            }
        }

        private void CheckAlarm()
        {
            if (m_dbTH.Connect() == false)
            {
                if (m_nReceiverID > 0)
                    m_webClient.SendReceiverInfo(m_nReceiverID, false);

                return;
            }
            else
            {
                if (m_nReceiverID > 0)
                    m_webClient.SendReceiverInfo(m_nReceiverID, true);
            }

            // seq_his 값이 주기적으로 초기화되기 때문에 seq_his와 time_evt를 동시에 비교한다.
            string strSQL = "Select seq_his, time_evt, alarm_no, alarm_state, alarm_alias ";
            strSQL += "from eam_his_table as his, eam_alarm_table as alarm ";
            strSQL += "where his.alarm_no = alarm.addr and (seq_his > " + m_nLastReadSequenceHistoryID.ToString() + " or time_evt > '" + m_strLastReadSequenceHistoryTime + "')";

            ArrayList arrResult = m_dbTH.GetResultData(strSQL);

            if (arrResult == null)
            {
                m_dbTH.Close();
                return;
            }

            bool isAlarm = false;
            int maxID = -1;
            string strLastTime = "";

            List<KeyValuePair<AlarmData, bool>> alarmDatas = new List<KeyValuePair<AlarmData, bool>>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> sequenceHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                string strAlarmCode = WebDBManager.GetStringField(arrResult[i + 2]);
                string strAlarmState = WebDBManager.GetStringField(arrResult[i + 3]);
                string strAlarmMessage = WebDBManager.GetStringField(arrResult[i + 4]);

                if (sequenceHistoryID == null || timeStamp == null || strAlarmCode == null || strAlarmState == null || strAlarmMessage == null)
                    continue;

                if (maxID < sequenceHistoryID.Data)
                    maxID = sequenceHistoryID.Data;

                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Data.Year, timeStamp.Data.Month, timeStamp.Data.Day, timeStamp.Data.Hour, timeStamp.Data.Minute, timeStamp.Data.Second);

                if (string.Compare(strLastTime, strTime) < 0)
                    strLastTime = strTime;

                if (strAlarmState == "F")
                {
                    isAlarm = true;
                }
                else if (strAlarmState == "N")
                {
                    isAlarm = false;
                }
                else
                    continue;

                AlarmData alarmData = null;

                if (m_dicAlarmDatas.TryGetValue(strAlarmCode, out alarmData) == false)
                {
                    Sensor sensor = null;
                    AlarmType alarmType = null;

                    if (GetAlarmInfo(strAlarmMessage, out sensor, out alarmType))
                    {
                        int nID = GetNewID("THSensorAlarm");

                        if (nID < 0)
                            continue;

                        alarmData = new AlarmData();
                        alarmData.AlarmCode = strAlarmCode;
                        alarmData.ID = nID;
                        alarmData.AlarmType = alarmType;
                        alarmData.Sensor = sensor;
                        alarmData.Message = strAlarmMessage;

                        if (AddNewAlarmCode(alarmData) == false)
                            continue;
                        else
                            m_dicAlarmDatas[strAlarmCode] = alarmData;
                    }
                }

                if (alarmData == null)
                    continue;

                // 연속으로 여러개의 신호를 보내면 서버에서 처리하지 못할수도 있다.
                // 여러개의 신호를 묶어 한번에 보내도록 한다.
                alarmDatas.Add(new KeyValuePair<AlarmData, bool>(alarmData, isAlarm));
                //SendAlarm(alarmData, isAlarm);
            }

            if (alarmDatas.Count > 0)
                SendAlarm(alarmDatas);

            if (maxID > 0 && strLastTime.Length > 0)
            {
                m_nLastReadSequenceHistoryID = maxID;
                m_strLastReadSequenceHistoryTime = strLastTime;
            }

            m_dbTH.Close();
        }

        private void SendAlarm(List<KeyValuePair<AlarmData, bool>> alarmDatas)
        {
            m_webClient.SendSensorDatas(alarmDatas);
        }

        private void SendAlarm(AlarmData alarm, bool isAlarm)
        {
            m_webClient.SendSensorData(alarm, isAlarm);
        }

        private bool AddNewAlarmCode(AlarmData alarm)
        {
            string strSQL = "Insert into THSensorAlarm (ID, AlarmCode, SensorMessage, AlarmTypeID, SensorID) values (";
            strSQL += string.Format("{0}, '{1}', '{2}', {3}, {4})", alarm.ID, alarm.AlarmCode, alarm.Message, alarm.AlarmType.ID, alarm.Sensor.ID);
            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private int GetNewID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 1;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 1;

            return id.Data + 1;
        }

        private bool GetAlarmInfo(string strMessage, out Sensor sensor, out AlarmType alarmType)
        {
            sensor = null;
            alarmType = null;

            int nFloorIndex;
            FloorPattern pattern;
            List<Sensor> sensors;

            if (GetFloorIndex(strMessage, out nFloorIndex, out pattern) == false)
                return false;
            
            if (m_dicFloorSensors.TryGetValue(nFloorIndex, out sensors) == false)
                return false;

            sensor = FindSensor(strMessage, nFloorIndex, pattern, sensors);

            if (sensor == null)
                return false;

            alarmType = GetAlarmType(RemoveEmpty(strMessage));

            if (alarmType == null)
                return false;

            return true;
        }

        private AlarmType GetAlarmType(string strAlarmMessage)
        {
            foreach (KeyValuePair<int, AlarmType> pair in m_dicAlarmTypes)
            {
                if (strAlarmMessage.Contains(pair.Value.TypeName))
                    return pair.Value;
            }

            return null;
        }

        private bool GetFloorIndex(string str, out int nFloorIndex, out AlarmManager.FloorPattern pattern)
        {
            pattern = AlarmManager.FloorPattern.None;
            nFloorIndex = 0;

            Regex rx = new Regex("B[0-9]{1,2}F", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(str);

            if (matches.Count > 0)
            {
                string strValue = matches[0].Value;
                strValue = strValue.Substring(1, strValue.Length - 2).Trim();

                if (int.TryParse(strValue, out nFloorIndex))
                {
                    nFloorIndex = -nFloorIndex;
                    pattern = AlarmManager.FloorPattern.B_F;
                    return true;
                }
            }

            rx = new Regex("[0-9]{1,2}F", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            matches = rx.Matches(str);

            if (matches.Count > 0)
            {
                string strValue = matches[0].Value;
                strValue = strValue.Substring(0, strValue.Length - 1).Trim();

                if (int.TryParse(strValue, out nFloorIndex))
                {
                    nFloorIndex--;
                    pattern = AlarmManager.FloorPattern._F;
                    return true;
                }
            }

            rx = new Regex("지하[ ]*[0-9]{1,2}[ ]*층", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            matches = rx.Matches(str);

            if (matches.Count > 0)
            {
                string strValue = matches[0].Value;
                strValue = strValue.Substring(2, strValue.Length - 3).Trim();

                if (int.TryParse(strValue, out nFloorIndex))
                {
                    nFloorIndex = -nFloorIndex;
                    pattern = AlarmManager.FloorPattern.지하층;
                    return true;
                }
            }

            rx = new Regex("[0-9]{1,2}[ ]*층", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            matches = rx.Matches(str);

            if (matches.Count > 0)
            {
                string strValue = matches[0].Value;
                strValue = strValue.Substring(0, strValue.Length - 1).Trim();

                if (int.TryParse(strValue, out nFloorIndex))
                {
                    nFloorIndex--;
                    pattern = AlarmManager.FloorPattern.층;
                    return true;
                }
            }

            return false;
        }

        private bool ReadAlarmCode()
        {
            string strSQL = "Select ID, AlarmCode, SensorMessage, AlarmTypeID, SensorID from THSensorAlarm";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            AlarmType alarmType;
            Sensor sensor;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strAlarmCode = WebDBManager.GetStringField(arrResult[i + 1]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> alarmTypeID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> sensorID = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (id == null || strAlarmCode == null || strMessage == null || alarmTypeID == null || sensorID == null)
                    continue;

                if (m_dicAlarmTypes.TryGetValue(alarmTypeID.Data, out alarmType) && m_dicSensors.TryGetValue(sensorID.Data, out sensor))
                {
                    AlarmData alarm = new AlarmData();

                    alarm.ID = id.Data;
                    alarm.AlarmCode = strAlarmCode;
                    alarm.Message = strMessage;
                    alarm.AlarmType = alarmType;
                    alarm.Sensor = sensor;

                    m_dicAlarmDatas[strAlarmCode] = alarm;
                }
            }

            return true;
        }

        private bool ReadAlarmTypes()
        {
            string strSQL = "Select ID, AlarmName from THAlarmType";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strAlarmName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strAlarmName == null)
                    continue;

                AlarmType alarmType = new AlarmType();
                alarmType.ID = id.Data;
                alarmType.TypeName = RemoveEmpty(strAlarmName);

                m_dicAlarmTypes[id.Data] = alarmType;
            }

            //m_alarmTypes.Sort();
            return true;
        }

        private bool ReadSensors()
        {
            string strSQL = "Select sensor.ID, sensor.SensorName, sensor.NickName, sensor.FloorIndex, sensor.SensorMeshName, sz.ID, sz.EquipZoneID, sti.ID ";
            strSQL += "from THSensor as sensor, SensorZone as sz, SensorTagInfo as sti ";
            strSQL += "where sz.OrgSensorID = sensor.ID and sti.SensorZoneID = sz.ID and sz.Type = " + (int)IFacility.FacilityType.TEMPERATURE_HUMIDITY;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strNickName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strMeshName = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                if (id == null || strSensorName == null || floorIndex == null || strMeshName == null || sensorZoneID == null || sensorTagInfoID == null)
                    continue;

                Sensor sensor = new Sensor();

                sensor.ID = id.Data;
                sensor.SensorName = strSensorName;
                sensor.NickName = strNickName;
                sensor.FloorIndex = floorIndex.Data;
                sensor.MeshName = strMeshName;
                sensor.SensorZoneID = sensorZoneID.Data;
                sensor.SensorTagInfoID = sensorTagInfoID.Data;

                List<Sensor> sensors = null;

                if (m_dicFloorSensors.TryGetValue(sensor.FloorIndex, out sensors) == false)
                {
                    sensors = new List<Sensor>();
                    m_dicFloorSensors[sensor.FloorIndex] = sensors;
                }

                sensors.Add(sensor);
                m_dicSensors[id.Data] = sensor;
            }

            return true;
        }

        public static Sensor FindSensor(string strAlarmMessage, int nFloorIndex, FloorPattern pattern, List<Sensor> sensors)
        {
            string strOriginMessage = RemoveEmpty(strAlarmMessage);

            if (pattern == FloorPattern.B_F)
            {
                strAlarmMessage = ChangeBFMessage(strOriginMessage, nFloorIndex);
            }
            else if (pattern == FloorPattern._F)
            {
                strAlarmMessage = ChangeFMessage(strOriginMessage, nFloorIndex);
            }
            else if (pattern == FloorPattern.지하층 || pattern == FloorPattern.층)
            {
            }
            else
                return null;

            if (strAlarmMessage.Length > 0)
            {
                foreach (Sensor sensor in sensors)
                {
                    if (strAlarmMessage.Contains(sensor.ShortSensorName))
                        return sensor;
                    else if (sensor.NickName != null && strAlarmMessage.Contains(sensor.ShortNickName))
                        return sensor;
                }
            }

            if (strOriginMessage != strAlarmMessage && strOriginMessage.Length > 0)
            {
                foreach (Sensor sensor in sensors)
                {
                    if (strOriginMessage.Contains(sensor.ShortSensorName))
                        return sensor;
                    else if (sensor.NickName != null && strOriginMessage.Contains(sensor.ShortNickName))
                        return sensor;
                }
            }

            return null;
        }

        private static string ChangeBFMessage(string strMessage, int nFloorIndex)
        {
            string strFloorIndex = string.Format("지하{0}층", -nFloorIndex);

            Regex rx = new Regex("B[0-9]{1,2}F", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(strMessage);

            if (matches.Count > 0)
            {
                string strValue = matches[0].Value;
                return strMessage.Replace(strValue, strFloorIndex);
            }

            return "";
        }

        private static string ChangeFMessage(string strMessage, int nFloorIndex)
        {
            string strFloorIndex = string.Format("{0}층", nFloorIndex);

            Regex rx = new Regex("[0-9]{1,2}F", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(strMessage);

            if (matches.Count > 0)
            {
                string strValue = matches[0].Value;
                return strMessage.Replace(strValue, strFloorIndex);
            }

            return "";
        }

        private static string RemoveEmpty(string str)
        {
            string[] tokens = str.Split(new char[] { ' ', '\t' });
            string strResult = "";

            foreach (string strToken in tokens)
            {
                strResult += strToken;
            }

            return strResult;
        }
    }
}
