using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Threading;
using System.Collections;
using System.Data.SqlClient;

namespace ExternalFireSensorDBWatcher
{
    public class FireSensorDBWatcher_SQLServer : FireSensorDBWatcher
    {
        private Dictionary<int, ExternalFireSensor> m_dicExternalFireSensor = new Dictionary<int, ExternalFireSensor>();
        // Key : ExternalSensorID
        // Value : SensorZoneID
        //private Dictionary<int, int> m_dicExternalSensorZone = new Dictionary<int, int>();

        private string m_strConnection = "";

        public FireSensorDBWatcher_SQLServer(SensorWatcherOwner owner, WebDBManager dbMgr, int nSiteID)
            : base(owner, dbMgr, nSiteID)
        {
        }

        public override bool Run()
        {
            m_strConnection = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};", ServerURL, DatabaseName, UserName, Password);
            
            Thread t = new Thread(new ThreadStart(MonitoringThread));
            t.Start();
            return true;
        }

        // SensorZone별 SensorTagInfo 정보를 얻어온다.
        // Return 값 : Key(SensorZoneID), Value(SensorTagInfoID)
        private Dictionary<int, int> ReadSensorTagInfo()
        {
            // 서울대 아신서버의 화재센서는 모두 FIRE_SENSOR 타입을 사용한다.
            int nSensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;
            string strSQL = "Select ID, SensorZoneID from SensorTagInfo where SensorType = " + nSensorType.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            Dictionary<int, int> dicSensorTagInfo = new Dictionary<int, int>();

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (sensorTagInfoID == null || sensorZoneID == null)
                    continue;

                dicSensorTagInfo[sensorZoneID.Data] = sensorTagInfoID.Data;
            }

            return dicSensorTagInfo;
        }

        private bool ReadExternalFireSensors()
        {
            Dictionary<int, int> dicSensorTagInfo = ReadSensorTagInfo();

            if (dicSensorTagInfo == null)
                return false;

            string strSQL = "Select ExternalSensorID, SensorZoneID from ExternalFireSensorLink where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            int nSensorTagInfoID;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> externalID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (externalID == null || sensorZoneID == null)
                    continue;

                if (dicSensorTagInfo.TryGetValue(sensorZoneID.Data, out nSensorTagInfoID) == false)
                    continue;

                ExternalFireSensor sensor = new ExternalFireSensor();
                sensor.ID = externalID.Data;
                sensor.SensorZoneID = sensorZoneID.Data;
                sensor.SensorTagInfoID = nSensorTagInfoID;

                m_dicExternalFireSensor[externalID.Data] = sensor;
            }

            return true;
        }

        private void MonitoringThread()
        {
            if (ReadExternalFireSensors())
            {
                m_runThread = true;

                while (m_runThread)
                {
                    // 1초에 한번씩 알람을 검사한다.
                    Thread.Sleep(1000);

                    ReadAlarm();
                }
            }
        }

        private void ReadAlarm()
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = m_strConnection;
                connection.Open();

                if (connection.State != System.Data.ConnectionState.Open)
                    return;

                string strSQL = "Select Idx, Tag, Val from tb_ar_ue";
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader == null)
                {
                    connection.Close();
                    return;
                }

                ExternalFireSensor sensor = null;

                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                        continue;

                    int nExternalSensorID = (int)reader[0];
                    string strTagName = reader[1].ToString();
                    string strValue = reader[2].ToString();

                    int nValue;

                    if (int.TryParse(strValue, out nValue) == false)
                        continue;

                    ExternalFireSensor.SensorState state = ExternalFireSensor.ToSensorState(nValue);

                    if (m_dicExternalFireSensor.TryGetValue(nExternalSensorID, out sensor) == false)
                        continue;

                    sensor.TagName = strTagName;

                    if (state == ExternalFireSensor.SensorState.ALARM)
                    {
                        if (sensor.State == ExternalFireSensor.SensorState.NORMAL || sensor.State == ExternalFireSensor.SensorState.UNKNOWN)
                            AddAlarm(sensor);

                        sensor.State = state;
                    }
                    else if (state == ExternalFireSensor.SensorState.NORMAL)
                    {
                        if (sensor.State == ExternalFireSensor.SensorState.ALARM)
                            RemoveAlarm(sensor);

                        sensor.State = state;
                    }
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        private void AddAlarm(ExternalFireSensor sensor)
        {
            if (m_owner != null)
                m_owner.AddAlarm(sensor);
        }

        private void RemoveAlarm(ExternalFireSensor sensor)
        {
            if (m_owner != null)
                m_owner.RemoveAlarm(sensor);
        }
    }
}
