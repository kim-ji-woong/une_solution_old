using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using DBUtility;
using System.Collections;

namespace AccessSecurityServer
{
    public class AccessWatcher_SQLServer : AccessWatcher
    {
        //private SqlConnection m_connection = null;
        // Key : Location ID
        private Dictionary<int, Location> m_dicLocation = new Dictionary<int, Location>();
        // Key : Device ID
        private Dictionary<int, Device> m_dicDevice = new Dictionary<int, Device>();
        // Key : Alarm ID
        private Dictionary<int, Alarm> m_dicAlarm = new Dictionary<int, Alarm>();
        private string m_strConnection = "";

        public AccessWatcher_SQLServer(AccessWatcherOwner owner, WebDBManager dbMgr, int nSiteID)
            : base(owner, dbMgr, nSiteID)
        {
        }

        public override bool Run()
        {
            m_strConnection = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};", ServerURL, DatabaseName, UserName, Password);
            NetworkManager.Instance.AccessDBConnectionString = m_strConnection;
            /*m_connection = new SqlConnection();
            m_connection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};", ServerURL, DatabaseName, UserName, Password);
            m_connection.Open();

            if (m_connection.State != System.Data.ConnectionState.Open)
                return false;

            NetworkManager.Instance.AccessDBConnection = m_connection;*/

            Thread t = new Thread(new ThreadStart(MonitoringThread));
            t.Start();
            return true;
        }

        private void MonitoringThread()
        {
            if (ReadLocation())
            {
                if (ReadDevice())
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

            //m_connection.Close();
        }

        private bool ReadDevice()
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = m_strConnection;
                connection.Open();

                if (connection.State != System.Data.ConnectionState.Open)
                    return false;

                string strSQL = "Select DeviceID, DeviceName, LocationID, EqTypeID, EqTypeName from View_External_Device";
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader == null)
                {
                    connection.Close();
                    return false;
                }

                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                        continue;

                    Device device = new Device();
                    device.ID = (int)reader[0];
                    device.Name = reader.IsDBNull(1) ? "" : reader[1].ToString();
                    device.Location = reader.IsDBNull(2) ? null : GetLocation((int)reader[2]);
                    device.DeviceType = reader.IsDBNull(3) || reader.IsDBNull(4) ? null : GetDeviceType((int)reader[3], reader[4].ToString());

                    m_dicDevice[device.ID] = device;
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private Location GetLocation(int nLocationID)
        {
            Location location = null;

            if (m_dicLocation.TryGetValue(nLocationID, out location))
                return location;

            return null;
        }

        private DeviceType GetDeviceType(int nDeviceTypeID, string strDeviceTypeName)
        {
            DeviceType deviceType = null;

            if (DeviceType.DeviceTypes.TryGetValue(nDeviceTypeID, out deviceType))
                return deviceType;

            deviceType = new DeviceType();
            deviceType.TypeID = nDeviceTypeID;
            deviceType.TypeName = strDeviceTypeName;
            DeviceType.DeviceTypes[deviceType.TypeID] = deviceType;

            return deviceType;
        }

        private bool ReadLocation()
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = m_strConnection;
                connection.Open();

                if (connection.State != System.Data.ConnectionState.Open)
                    return false;

                string strSQL = "Select LocationID, LocationName from View_External_Location";
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader == null)
                {
                    connection.Close();
                    return false;
                }

                while (reader.Read())
                {
                    if (reader.IsDBNull(0) || reader.IsDBNull(1))
                        continue;

                    Location location = new Location();
                    location.ID = (int)reader[0];
                    location.Name = reader[1].ToString();

                    m_dicLocation[location.ID] = location;
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.Message);
                return false;
            }

            return ReadEquipmentZone();
        }

        private bool ReadEquipmentZone()
        {
            if (m_dbMgr == null)
                return false;

            string strSQL = "Select LocationID, EquipZoneID from AccessLink_View_External_Location where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            Location location = null;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> locationID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (locationID == null || equipZoneID == null)
                    continue;

                if (m_dicLocation.TryGetValue(locationID.Data, out location))
                {
                    location.EquipZoneID = equipZoneID.Data;
                }
            }

            return true;
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

                // 하루가 지난 알람은 처리하지 않는다.
                DateTime today = DateTime.Now;
                DateTime yesterday = today.AddDays(-1.0);
                string strYesterDay = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'",
                    yesterday.Year, yesterday.Month, yesterday.Day, yesterday.Hour, yesterday.Minute, yesterday.Second);

                string strSQL = "Select AlarmID, AlarmState, EventDateTime, RecvDateTime, DeviceID, State, PreState, CardNo, Content1, Content2, Content3, Content4 from View_External_Alarm where EventDateTime > " + strYesterDay;
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader == null)
                {
                    connection.Close();
                    return;
                }

                Alarm alarm = null;
                // 기기별 마지막 알람상태
                Dictionary<Device, Alarm> dicDeviceAlarm = new Dictionary<Device, Alarm>();
                List<int> alarmIds = new List<int>();

                List<Alarm> deletedAlarms = m_dicAlarm.Values.ToList();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                        continue;

                    int nAlarmID = (int)reader[0];
                    alarmIds.Add(nAlarmID);

                    if (m_dicAlarm.TryGetValue(nAlarmID, out alarm) == false)
                    {
                        alarm = new Alarm();
                        alarm.ID = nAlarmID;
                    }

                    deletedAlarms.Remove(alarm);

                    //Alarm.StateType prevAlarmState = alarm.AlarmState;

                    alarm.AlarmState = reader.IsDBNull(1) ? Alarm.StateType.UNKNOWN : Alarm.ToAlarmState(reader[1].ToString());
                    alarm.EventTime = reader.IsDBNull(2) ? null : WebDBManager.GetDateTimeField(reader[2]);
                    alarm.ReceivedTime = reader.IsDBNull(3) ? null : WebDBManager.GetDateTimeField(reader[3]);
                    alarm.Device = reader.IsDBNull(4) ? null : GetDevice((int)reader[4]);
                    alarm.State = reader.IsDBNull(5) ? null : reader[5].ToString();
                    alarm.PrevState = reader.IsDBNull(6) ? null : reader[6].ToString();
                    alarm.CardNo = reader.IsDBNull(7) ? null : reader[7].ToString();
                    alarm.Content1 = reader.IsDBNull(8) ? null : reader[8].ToString();
                    alarm.Content2 = reader.IsDBNull(9) ? null : reader[9].ToString();
                    alarm.Content3 = reader.IsDBNull(10) ? null : reader[10].ToString();
                    alarm.Content4 = reader.IsDBNull(11) ? null : reader[11].ToString();

                    if (alarm.EventTime == null)
                        continue;

                    if (alarm.Device != null)
                    {
                        Alarm lastAlarm = null;

                        if (dicDeviceAlarm.TryGetValue(alarm.Device, out lastAlarm) == false)
                        {
                            lastAlarm = alarm;
                            dicDeviceAlarm[alarm.Device] = lastAlarm;
                            continue;
                        }

                        if (lastAlarm.EventTime == null)
                            dicDeviceAlarm[alarm.Device] = alarm;
                        else if (alarm.EventTime != null && lastAlarm.EventTime.Data < alarm.EventTime.Data)
                            dicDeviceAlarm[alarm.Device] = alarm;
                    }
                }

                reader.Close();
                connection.Close();


                // 삭제된 알람을 없앤다.
                foreach (Alarm delAlarm in deletedAlarms)
                {
                    m_dicAlarm.Remove(delAlarm.ID);
                }

                foreach (KeyValuePair<Device, Alarm> pair in dicDeviceAlarm)
                {
                    if (pair.Value.AlarmState == Alarm.StateType.NONE || pair.Value.AlarmState == Alarm.StateType.UNKNOWN)
                        RemoveAlarm(pair.Key, pair.Value);
                    else
                        AddAlarm(pair.Value);
                }
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.Message);
                return;
            }
        }
        /*private void ReadAlarm()
        {
            string strSQL = "Select AlarmID, AlarmState, EventDateTime, RecvDateTime, DeviceID, State, PreState, CardNo, Content1, Content2, Content3, Content4 from View_External_Alarm";
            SqlCommand cmd = new SqlCommand(strSQL, m_connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader == null)
                return;

            Alarm alarm = null;

            while (reader.Read())
            {
                if (reader.IsDBNull(0))
                    continue;

                int nAlarmID = (int)reader[0];

                if (m_dicAlarm.TryGetValue(nAlarmID, out alarm) == false)
                {
                    alarm = new Alarm();
                    alarm.ID = nAlarmID;
                }

                Alarm.StateType prevAlarmState = alarm.AlarmState;

                alarm.AlarmState = reader.IsDBNull(1) ? Alarm.StateType.UNKNOWN : Alarm.ToAlarmState(reader[1].ToString());
                alarm.EventTime = reader.IsDBNull(2) ? null : WebDBManager.GetDateTimeField(reader[2]);
                alarm.ReceivedTime = reader.IsDBNull(3) ? null : WebDBManager.GetDateTimeField(reader[3]);
                alarm.Device = reader.IsDBNull(4) ? null : GetDevice((int)reader[4]);
                alarm.State = reader.IsDBNull(5) ? null : reader[5].ToString();
                alarm.PrevState = reader.IsDBNull(6) ? null : reader[6].ToString();
                alarm.CardNo = reader.IsDBNull(7) ? null : reader[7].ToString();
                alarm.Content1 = reader.IsDBNull(8) ? null : reader[8].ToString();
                alarm.Content2 = reader.IsDBNull(9) ? null : reader[9].ToString();
                alarm.Content3 = reader.IsDBNull(10) ? null : reader[10].ToString();
                alarm.Content4 = reader.IsDBNull(11) ? null : reader[11].ToString();

                if (alarm.AlarmState == Alarm.StateType.NONE || alarm.AlarmState == Alarm.StateType.UNKNOWN)
                    RemoveAlarm(alarm, prevAlarmState);
                else
                    AddAlarm(alarm);
            }

            reader.Close();
        }*/

        private void AddAlarm(Alarm alarm)
        {
            if (m_dicAlarm.ContainsKey(alarm.ID) == false)
            {
                m_dicAlarm[alarm.ID] = alarm;

                if (m_owner != null)
                    m_owner.AddAlarm(alarm);
            }
        }

        private void RemoveAlarm(Device device, Alarm alarm)
        {
            m_dicAlarm.Remove(alarm.ID);

            if (m_owner != null)
                m_owner.RemoveAlarm(device);
        }
        /*private void RemoveAlarm(Alarm alarm, Alarm.StateType prevAlarmState)
        {
            m_dicAlarm.Remove(alarm.ID);

            if (m_owner != null)
                m_owner.RemoveAlarm(alarm, prevAlarmState);
        }*/

        private Device GetDevice(int nDeviceID)
        {
            Device device = null;

            if (m_dicDevice.TryGetValue(nDeviceID, out device))
                return device;

            return device;
        }

        public override void MakeTestAlarm()
        {
            Device device = null;

            foreach (KeyValuePair<int, Device> pair in m_dicDevice)
            {
                if (pair.Value.Location == null)
                    continue;

                device = pair.Value;
                break;
            }

            Alarm alarm = new Alarm();

            alarm.AlarmState = Alarm.StateType.GENERAL_INTRUSION1;
            alarm.Device = device;
            alarm.EventTime = new VariousData<DateTime>(DateTime.Now);
            alarm.ID = 1;

            AddAlarm(alarm);
        }

        public override void ClearTestAlarm()
        {
            Device device = null;

            foreach (KeyValuePair<int, Device> pair in m_dicDevice)
            {
                if (pair.Value.Location == null)
                    continue;

                device = pair.Value;
                break;
            }

            Alarm alarm = new Alarm();

            alarm.AlarmState = Alarm.StateType.NONE;
            alarm.Device = device;
            alarm.EventTime = new VariousData<DateTime>(DateTime.Now);
            alarm.ID = 1;

            RemoveAlarm(device, alarm);
        }
    }
}
