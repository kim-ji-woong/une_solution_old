using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace AirQualityServer
{
    public class SensorManager
    {
        private DirectDBManager m_dbJubixMgr = null;
        private WebDBManager m_dbMgr = null;
        private bool m_closeApp = false;

        private bool m_isConnected = false, m_isFirstConnection = true;
        private string m_strIP = "";
        private Dictionary<int, Sensor> m_dicSensors = new Dictionary<int, Sensor>();
        // 센서별로 서버에게 보낸 마지막 데이터를 기억해 놓는다..
        private Dictionary<int, int> m_dicLastSensorData = new Dictionary<int, int>();

        private const string AQ_TableName = "AirQuaility";

        private Network.NetworkWebManager m_netMgr = null;
        private IManagerOwner m_owner = null;

        // true이면 접속, false면 접속되지 않은 상태
        private VariousData<bool> m_prevReceiverState = null;
        private int m_nReceiverID = -1;

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public SensorManager(IManagerOwner owner = null, TextBox textBoxO2 = null, TextBox textBoxCO2 = null, TextBox textBoxCO = null, TextBox textBoxCH4 = null, TextBox textBoxTemp = null, TextBox textBoxHumi = null, Label labelConnectionStatus = null)
        {
            m_owner = owner;

            m_strIP = System.Configuration.ConfigurationManager.AppSettings.Get("ip");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("db_name");
            string strID = System.Configuration.ConfigurationManager.AppSettings.Get("id");
            string strPW = System.Configuration.ConfigurationManager.AppSettings.Get("pw");
            List<string> inverseSensors = ReadInverseList();

            m_dbJubixMgr = DirectDBManager.MakeInstance(DirectDBManager.DBType.mysql, m_strIP, strID, strPW, strDBName);

            if (ReadSiteID() == false)
            {
                MessageBox.Show("DB 초기화에 실패하였습니다.");
            }
            else
                m_netMgr = new Network.NetworkWebManager(m_dbMgr, this);

            InitSensor("O2", "산소", textBoxO2, inverseSensors);
            InitSensor("CO2", "이산화탄소", textBoxCO2, inverseSensors);
            InitSensor("CO", "일산화탄소", textBoxCO, inverseSensors);
            InitSensor("CH4", "메탄", textBoxCH4, inverseSensors);
            InitSensor("Temp", "온도", textBoxTemp, inverseSensors);
            InitSensor("Humi", "습도", textBoxHumi, inverseSensors);

            ReadReceiver();

            if (labelConnectionStatus != null)
                labelConnectionStatus.Text = "";
        }

        private void ReadReceiver()
        {
            string strSQL = "Select ID, Place from SensorServerInfo";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPlace = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strPlace == null)
                    continue;

                strPlace = strPlace.ToLower();

                if (strPlace.Contains("air"))
                {
                    m_nReceiverID = id.Data;
                    break;
                }
            }
        }

        private List<string> ReadInverseList()
        {
            List<string> sensors = new List<string>();
            string strInverseList = System.Configuration.ConfigurationManager.AppSettings.Get("Inverse");
            string[] tokens = strInverseList.Split(',');

            foreach (string strSensor in tokens)
            {
                sensors.Add(strSensor.Trim());
            }

            return sensors;
        }

        private bool ReadSiteID()
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("SiteID");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("SiteDBName");
            string strDBType = System.Configuration.ConfigurationManager.AppSettings.Get("SiteDBType");
            string strHost = System.Configuration.ConfigurationManager.AppSettings.Get("SiteDBHost");

            int nID, nType;

            if (int.TryParse(strSiteID, out nID) == false || int.TryParse(strDBType, out nType) == false)
                return false;

            m_dbMgr = new WebDBManager(strDBName, nID);
            m_dbMgr.DatabaseType = (WebDBManager.DBType)nType;
            m_dbMgr.WebServerURL = strHost;

            return true;
        }

        private void InitSensor(string strSensorName, string strSensorTagInfoName, TextBox textBox, List<string> inverseSensors)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings.Get(strSensorName);

            if (strValue != null && strValue.Length > 0)
            {
                int nIndex = strValue.IndexOf('_');

                if (nIndex < 0)
                    return;

                string strID = strValue.Substring(0, nIndex);
                string strTagName = strValue.Substring(nIndex + 1);

                int nID;

                if (int.TryParse(strID, out nID) == false)
                    return;

                Sensor sensor = new Sensor();
                sensor.Name = strSensorName;
                sensor.ID = nID;
                sensor.TagName = strTagName;
                sensor.Tag = textBox;

                m_dicSensors[nID] = sensor;

                if (inverseSensors.Contains(strSensorName))
                    sensor.IsInverse = true;

                string strSQL = "Select ID, SensorZoneID from SensorTagInfo where SensorName = '" + strSensorTagInfoName + "'";
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count < 2)
                    return;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[1].ToString());

                if (id != null && sensorZoneID != null)
                {
                    sensor.SensorTagInfoID = id.Data;
                    sensor.SensorZoneID = sensorZoneID.Data;
                }
            }
        }

        private void ReadSensorDataThread()
        {
            while (m_closeApp == false)
            {
                try
                {
                    ReadSensorData();
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    break;
                }
            }
        }

        private void ReadSensorData()
        {
            bool isConnected = m_dbJubixMgr.Connect();

            if (m_isConnected != isConnected || m_isFirstConnection)
            {
                SetDBConnectionState(isConnected);
            }

            m_isFirstConnection = false;

            if (isConnected == false)
                return;

            ReadData();
            m_dbJubixMgr.Close();
        }

        private void ReadData()
        {
            Dictionary<int, AlarmLevel> dicAlarmLevels = ReadAlarmLimit();

            bool isConnected = false;

            foreach (KeyValuePair<int, Sensor> pair in m_dicSensors)
            {
                string strSQL = string.Format("Select ss_ID, ss_date, ss_Value, now() from r_ss_dat where ss_ID = '{0}' and ss_date = (Select max(ss_date) from r_ss_dat where ss_ID = '{0}')", pair.Value.TagName);
                ArrayList arrResult = m_dbJubixMgr.GetResultData(strSQL);

                if (arrResult == null)
                {
                    SetDBConnectionState(false);
                    return;
                }

                UpdateSensorData(pair.Value, arrResult, dicAlarmLevels);

                if (pair.Value.IsConnected)
                    isConnected = true;
            }

            if (m_prevReceiverState == null || m_prevReceiverState.Data != isConnected)
            {
                Network.NetworkWebManager.Instance.SendAllReceiverState(isConnected, m_nReceiverID);
                m_prevReceiverState = new VariousData<bool>(isConnected);
            }
        }

        public void InitReceiverState()
        {
            m_prevReceiverState = null;
        }

        private Dictionary<int, AlarmLevel> ReadAlarmLimit()
        {
            string strSQL = "Select ID, AlarmLimit_1st, AlarmLimit_2nd, AlarmLimit_3rd from " + AQ_TableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            Dictionary<int, AlarmLevel> dicLevels = new Dictionary<int, AlarmLevel>();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<float> level1 = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<float> level2 = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> level3 = WebDBManager.GetFloatField(arrResult[i + 3].ToString());

                if (id == null)
                    continue;

                if (level1 == null && level2 == null && level3 == null)
                    continue;

                AlarmLevel level = new AlarmLevel();
                level.Level1 = level1;
                level.Level2 = level2;
                level.Level3 = level3;
                dicLevels[id.Data] = level;
            }

            return dicLevels;
        }

        private void SetDBConnectionState(bool isConnected)
        {
            m_isConnected = isConnected;

            if (m_owner != null)
                m_owner.SetDBConnectionState(m_isConnected, m_strIP);

            if (m_isConnected == false)
            {
                foreach (KeyValuePair<int, Sensor> pair in m_dicSensors)
                {
                    SetUnconnectedSensor(pair.Value);
                }
            }
        }

        private bool UpdateSensorData(Sensor sensor, ArrayList arrResult, Dictionary<int, AlarmLevel> dicAlarmLevels)
        {
            if (arrResult.Count < 4)
            {
                SetUnconnectedSensor(sensor);
                return false;
            }

            string strTagName = WebDBManager.GetStringField(arrResult[0]);
            string strSensorTime = WebDBManager.GetStringField(arrResult[1]);
            string strValue = WebDBManager.GetStringField(arrResult[2]);
            VariousData<DateTime> dtCurrent = WebDBManager.GetDateTimeField(arrResult[3]);

            /////////////////////////////////
            //dtCurrent = new VariousData<DateTime>(new DateTime(2019, 12, 12, 17, 09, 32));

            if (strTagName == null || strSensorTime == null || strValue == null || dtCurrent == null)
            {
                SetUnconnectedSensor(sensor);
                return false;
            }

            VariousData<DateTime> dtSensor = StringToDateTime(strSensorTime);
            TimeSpan span = dtCurrent.Data - dtSensor.Data;

            if (span.TotalMinutes >= 1.0)
            {
                SetUnconnectedSensor(sensor);
                return false;
            }

            float fValue = 0.0f;

            if (float.TryParse(strValue, out fValue) == false)
            {
                SetUnconnectedSensor(sensor);
                return false;
            }

            AlarmLevel alarmLevel = null;
            dicAlarmLevels.TryGetValue(sensor.ID, out alarmLevel);

            sensor.Value = fValue;
            int nAlarmLevel = CheckSensorAlarm(sensor, alarmLevel);

            string strSQL = string.Format("Update {2} set Connected = 1, Value = {0} where ID = {1}", fValue, sensor.ID, AQ_TableName);

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                sensor.IsConnected = true;

                if (m_owner != null)
                    m_owner.UpdateSensorData(sensor);

                if (sensor.SensorZoneID > 0 && sensor.SensorTagInfoID > 0)
                {
                    int nLastData;

                    if (m_dicLastSensorData.TryGetValue(sensor.ID, out nLastData))
                    {
                        if (nLastData == nAlarmLevel)
                            return true;
                    }

                    if (m_netMgr.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, nAlarmLevel, true))
                    {
                        m_dicLastSensorData[sensor.ID] = nAlarmLevel;
                    }
                }

                return true;
            }

            return false;
        }

        private int CheckSensorAlarm(Sensor sensor, AlarmLevel alarmLevel)
        {
            if (alarmLevel == null)
                return 0;

            if (sensor.IsInverse)
            {
                if (alarmLevel.Level3 != null && sensor.Value < alarmLevel.Level3.Data)
                    return 3;
                else if (alarmLevel.Level2 != null && sensor.Value < alarmLevel.Level2.Data)
                    return 2;
                else if (alarmLevel.Level1 != null && sensor.Value < alarmLevel.Level1.Data)
                    return 1;
            }
            else
            {
                if (alarmLevel.Level3 != null && sensor.Value > alarmLevel.Level3.Data)
                    return 3;
                else if (alarmLevel.Level2 != null && sensor.Value > alarmLevel.Level2.Data)
                    return 2;
                else if (alarmLevel.Level1 != null && sensor.Value > alarmLevel.Level1.Data)
                    return 1;
            }

            return 0;
        }

        private void SetUnconnectedSensor(Sensor sensor)
        {
            string strSQL = string.Format("Update {1} set Connected = 0 where ID = {0}", sensor.ID, AQ_TableName);

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                sensor.IsConnected = false;

                if (m_owner != null)
                    m_owner.SetUnconnectedSensor(sensor);
            }
        }

        private VariousData<DateTime> StringToDateTime(string strTime)
        {
            if (strTime.Length != 14)
                return null;

            string strYear = strTime.Substring(0, 4);
            string strMonth = strTime.Substring(4, 2);
            string strDay = strTime.Substring(6, 2);
            string strHour = strTime.Substring(8, 2);
            string strMin = strTime.Substring(10, 2);
            string strSec = strTime.Substring(12, 2);

            int year, month, day, hour, min, sec;

            if (int.TryParse(strYear, out year) == false || int.TryParse(strMonth, out month) == false || int.TryParse(strDay, out day) == false)
                return null;
            if (int.TryParse(strHour, out hour) == false || int.TryParse(strMin, out min) == false || int.TryParse(strSec, out sec) == false)
                return null;

            DateTime time = new DateTime(year, month, day, hour, min, sec);
            return new VariousData<DateTime>(time);
        }

        public void RunThread()
        {
            if (m_dbMgr != null)
            {
                Thread t = new Thread(new ThreadStart(ReadSensorDataThread));
                t.Start();
            }
        }

        public void CloseThread()
        {
            m_netMgr.ReleaseThread();
            m_closeApp = true;
        }
    }

    public interface IManagerOwner
    {
        void SetDBConnectionState(bool isConnected, string strIP);
        void UpdateSensorData(Sensor sensor);
        void SetUnconnectedSensor(Sensor sensor);
    }

    public class AlarmLevel
    {
        private VariousData<float> m_level1 = null;
        private VariousData<float> m_level2 = null;
        private VariousData<float> m_level3 = null;

        public VariousData<float> Level1
        {
            get { return m_level1; }
            set { m_level1 = value; }
        }

        public VariousData<float> Level2
        {
            get { return m_level2; }
            set { m_level2 = value; }
        }

        public VariousData<float> Level3
        {
            get { return m_level3; }
            set { m_level3 = value; }
        }
    }
}
