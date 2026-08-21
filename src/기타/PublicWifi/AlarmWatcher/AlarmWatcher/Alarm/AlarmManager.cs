using System;
using System.Configuration;
using System.Collections;

namespace AlarmWatcher.Alarm
{
    public class AlarmManager
    {
        private const string Sensor_PM2_5 = "pm2_5";
        private const string Sensor_O3 = "o3";
        private const string Sensor_Temperature = "temp";
        private const string Sensor_Humidity = "humi";

        private const string AlarmLevel2 = "watch";
        private const string AlarmLevel4 = "alert";

        private Pm25 m_pm25 = null;
        private O3 m_o3 = null;
        private Temperature m_temperature = null;
        private Dry m_dry = null;
        private DBManager m_dbMgr = null;

        public AlarmManager(DBManager dbMgr)
        {
            string pm25Level2 = ConfigurationManager.AppSettings.Get("alarmLevel2_pm25");
            string pm25Level2Clear = ConfigurationManager.AppSettings.Get("alarmLevel2_clear_pm25");
            string pm25Level4 = ConfigurationManager.AppSettings.Get("alarmLevel4_pm25");
            string pm25Level4Clear = ConfigurationManager.AppSettings.Get("alarmLevel4_clear_pm25");

            if (pm25Level2 != null && pm25Level2Clear != null && pm25Level4 != null && pm25Level4Clear != null)
                m_pm25 = new Pm25(pm25Level2, pm25Level4, pm25Level2Clear, pm25Level4Clear);

            string o3Level2 = ConfigurationManager.AppSettings.Get("alarmLevel2_o3");
            string o3Level4 = ConfigurationManager.AppSettings.Get("alarmLevel4_o3");

            if (o3Level2 != null && o3Level4 != null)
                m_o3 = new O3(o3Level2, o3Level4);

            string cold1Level2 = ConfigurationManager.AppSettings.Get("alarmLevel2_cold1");
            string cold2Level2 = ConfigurationManager.AppSettings.Get("alarmLevel2_cold2");
            string heatLevel2 = ConfigurationManager.AppSettings.Get("alarmLevel2_heat");
            string cold1Level4 = ConfigurationManager.AppSettings.Get("alarmLevel4_cold1");
            string cold2Level4 = ConfigurationManager.AppSettings.Get("alarmLevel4_cold2");
            string heatLevel4 = ConfigurationManager.AppSettings.Get("alarmLevel4_heat");

            if (cold1Level2 != null && cold2Level2 != null && heatLevel2 != null &&
                cold1Level4 != null && cold2Level4 != null && heatLevel4 != null)
                m_temperature = new Temperature(cold1Level2, cold2Level2, heatLevel2, cold1Level4, cold2Level4, heatLevel4);

            string dryLevel2 = ConfigurationManager.AppSettings.Get("alarmLevel2_dry");
            string dryLevel4 = ConfigurationManager.AppSettings.Get("alarmLevel4_dry");

            if (dryLevel2 != null && dryLevel4 != null)
                m_dry = new Dry(dryLevel2, dryLevel4);

            m_dbMgr = dbMgr;

            InitDatas();
        }

        private void InitDatas()
        {
            float? fValue;
            DateTime? alarmTime;

            int alarm = InitData(Sensor_PM2_5, out fValue, out alarmTime);

            if (/*alarm > 0 && */m_pm25 != null && alarmTime != null)
            {
                if (alarm == 2)
                    m_pm25.SetStatus(Pm25.Status.Level2, alarmTime);
                else if (alarm == 4)
                    m_pm25.SetStatus(Pm25.Status.Level4, alarmTime);
                else
                    m_pm25.SetStatus(Pm25.Status.None, alarmTime);
            }

            alarm = InitData(Sensor_O3, out fValue, out alarmTime);

            if (/*alarm > 0 && */m_o3 != null && alarmTime != null)
            {
                if (alarm == 2)
                    m_o3.SetStatus(O3.Status.Level2, alarmTime);
                else if (alarm == 4)
                    m_o3.SetStatus(O3.Status.Level4, alarmTime);
                else
                    m_o3.SetStatus(O3.Status.None, alarmTime);
            }

            alarm = InitData(Sensor_Temperature, out fValue, out alarmTime);

            if (/*alarm > 0 && */m_temperature != null && alarmTime != null)
            {
                if (alarm == 2)
                {
                    if (fValue != null && (float)fValue < 10)
                        m_temperature.SetStatus(Temperature.Status.ColdLevel2, alarmTime);
                    else if (fValue != null && (float)fValue > 20)
                        m_temperature.SetStatus(Temperature.Status.HeatLevel2, alarmTime);
                }
                else if (alarm == 4)
                {
                    if (fValue != null && (float)fValue < 10)
                        m_temperature.SetStatus(Temperature.Status.ColdLevel4, alarmTime);
                    else if (fValue != null && (float)fValue > 20)
                        m_temperature.SetStatus(Temperature.Status.HeatLevel4, alarmTime);
                }
                else
                    m_temperature.SetStatus(Temperature.Status.None, alarmTime);
            }

            alarm = InitData(Sensor_Humidity, out fValue, out alarmTime);

            if (/*alarm > 0 && */m_dry != null && alarmTime != null)
            {
                if (alarm == 2)
                    m_dry.SetStatus(Dry.Status.Level2, alarmTime);
                else if (alarm == 4)
                    m_dry.SetStatus(Dry.Status.Level4, alarmTime);
                else
                    m_dry.SetStatus(Dry.Status.None, alarmTime);
            }
        }

        private int InitData(string strSensorType, out float? fValue, out DateTime? alarmTime)
        {
            int alarm = 0;
            fValue = null;
            alarmTime = null;

            string strSQL = "Select id, regdate, active, atype, value from SensorAlarm where id = (Select max(id) from SensorAlarm where stype = '" + strSensorType + "')";
            ArrayList arrResult = m_dbMgr.RunQuery(strSQL);

            if (arrResult == null)
                return alarm;

            if (arrResult.Count == 5)
            {
                VariousData<DateTime> time = DBManager.GetDateTimeField(arrResult[1].ToString());
                VariousData<int> active = DBManager.GetIntField(arrResult[2].ToString());
                string strAType = DBManager.GetStringField(arrResult[3]);
                VariousData<float> value = DBManager.GetFloatField(arrResult[4].ToString());

                if (time != null && active != null && strAType != null)// && value != null)
                {
                    if (active.Data == 1)
                    {
                        if (strAType == AlarmLevel2)
                            alarm = 2;
                        else if (strAType == AlarmLevel4)
                            alarm = 4;

                        if (value != null)
                            fValue = value.Data;
                    }

                    alarmTime = time.Data;
                }
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                strSQL = string.Format("Insert into SensorAlarm (id, regdate, stype, active, atype, value) values (IsNull((SELECT MAX(ID) FROM SensorAlarm C), 0) + 1, '{0}', '{1}', 0, '', NULL)", strTime, strSensorType);
                m_dbMgr.RunQuery(strSQL);
            }

            return alarm;
        }

        public void ProcessSensorData(int year, int month, int day, float pm2_5, float no2, float o3, float temp, float humidity, DateTime regdate)
        {
            bool isChanged = false;

            if (m_pm25 != null)
            {
                Pm25.Status status = m_pm25.SetData(pm2_5, regdate, out isChanged);

                if (isChanged)
                {
                    if (status == Pm25.Status.None)
                        AddAlarmData(regdate, Sensor_PM2_5, 0, "", null);
                    else if (status == Pm25.Status.Level2)
                        AddAlarmData(regdate, Sensor_PM2_5, 1, AlarmLevel2, m_pm25.AlarmData);
                    else if (status == Pm25.Status.Level4)
                        AddAlarmData(regdate, Sensor_PM2_5, 1, AlarmLevel4, m_pm25.AlarmData);
                }
            }

            if (m_o3 != null)
            {
                O3.Status status = m_o3.SetData(o3, regdate, out isChanged);

                if (isChanged)
                {
                    if (status == O3.Status.None)
                        AddAlarmData(regdate, Sensor_O3, 0, "", null);
                    else if (status == O3.Status.Level2)
                        AddAlarmData(regdate, Sensor_O3, 1, AlarmLevel2, m_o3.AlarmData);
                    else if (status == O3.Status.Level4)
                        AddAlarmData(regdate, Sensor_O3, 1, AlarmLevel4, m_o3.AlarmData);
                }
            }

            if (m_temperature != null)
            {
                Temperature.Status status = m_temperature.SetData(temp, regdate, out isChanged);

                if (isChanged)
                {
                    if (status == Temperature.Status.None)
                        AddAlarmData(regdate, Sensor_Temperature, 0, "", null);
                    else if (status == Temperature.Status.ColdLevel2)
                        AddAlarmData(regdate, Sensor_Temperature, 1, AlarmLevel2, m_temperature.AlarmData);
                    else if (status == Temperature.Status.ColdLevel4)
                        AddAlarmData(regdate, Sensor_Temperature, 1, AlarmLevel4, m_temperature.AlarmData);
                    else if (status == Temperature.Status.HeatLevel2)
                        AddAlarmData(regdate, Sensor_Temperature, 1, AlarmLevel2, m_temperature.AlarmData);
                    else if (status == Temperature.Status.HeatLevel4)
                        AddAlarmData(regdate, Sensor_Temperature, 1, AlarmLevel4, m_temperature.AlarmData);
                }
            }

            if (m_dry != null)
            {
                Dry.Status status = m_dry.SetData(humidity, regdate, out isChanged);

                if (isChanged)
                {
                    if (status == Dry.Status.None)
                        AddAlarmData(regdate, Sensor_Humidity, 0, "", null);
                    else if (status == Dry.Status.Level2)
                        AddAlarmData(regdate, Sensor_Humidity, 1, AlarmLevel2, m_dry.AlarmData);
                    else if (status == Dry.Status.Level4)
                        AddAlarmData(regdate, Sensor_Humidity, 1, AlarmLevel4, m_dry.AlarmData);
                }
            }
        }

        private bool AddAlarmData(DateTime time, string strSensorType, int active, string strAlarmType, float? value)
        {
            string strValue = value == null ? "NULL" : ((float)value).ToString();
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            string strSQL = string.Format("Insert into SensorAlarm (id, regdate, stype, active, atype, value) values (IsNull((SELECT MAX(ID) FROM SensorAlarm C), 0) + 1, '{0}', '{1}', {2}, '{3}', {4})",
                strTime, strSensorType, active, strAlarmType, strValue);

            return m_dbMgr.RunQuery(strSQL) != null;
        }
    }
}
