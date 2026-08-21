using System;
using SDMS.DAL;
using SDMS.Model.Alarm;
using dnsCommunicateSopServer;
using SDMS.Model.Sensor;
using SDMS.Model.History;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;

namespace SVMSServer
{
    public class AlarmManager
    {
        // SVMS 알람발생시 몇초후에 자동종료되는가?
        private int? m_nSvmsEventAutoCloseSeconds = null;
        private DataManager m_dataManager = null;
        private Common.DAL.DataManager m_commonDataManager = null;
        private string m_strAlarmURL = "";

        public AlarmManager(DataManager dataManager, Common.DAL.DataManager commonDataManager)
        {
            m_dataManager = dataManager;
            m_commonDataManager = commonDataManager;
            m_strAlarmURL = ConfigurationManager.AppSettings.Get("Alarm_Security_URL");

            if (commonDataManager != null)
            {
                string strErrorMessage;
                List<Common.Model.Option.Options> options = commonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "SVMSEventAutoCloseSeconds", out strErrorMessage);

                if (options == null || options.Count == 0)
                    return;

                int seconds;

                if (int.TryParse(options[0].PropertyValue.Trim(), out seconds))
                {
                    if (seconds > 0)
                        m_nSvmsEventAutoCloseSeconds = seconds;
                }
            }
        }

        public void CheckAutoClose()
        {
            if (m_dataManager != null && m_nSvmsEventAutoCloseSeconds != null)
            {
                bool isNullable;
                string strConditions = string.Format("{0} >= {1} and {0} <= {2}",
                    CurrentAlarm.GetFieldName(CurrentAlarm.Fields.SensorType, out isNullable),
                    (int)dnsData.Sensor.Facility.FacilityType.Intrusion_S1,
                    (int)dnsData.Sensor.Facility.FacilityType.Fire_S1);

                string strErrorMessage;
                List<CurrentAlarm> alarms = m_dataManager.GetSelectManager().SelectCurrentAlarms(null, strConditions, out strErrorMessage);

                if (alarms == null)
                    return;

                
                DateTime? dtNow = GetDBTime();

                if (dtNow != null)
                {
                    foreach (CurrentAlarm alarm in alarms)
                    {
                        TimeSpan span = ((DateTime)dtNow) - alarm.TimeStamp;

                        if (span.TotalSeconds >= m_nSvmsEventAutoCloseSeconds)
                        {
                            SendCloseEvent(alarm);
                        }
                    }
                }
            }
        }

        private DateTime? GetDBTime()
        {
            string strCurrentTime = m_commonDataManager.GetSelectManager().GetCurrentTime();

            if (strCurrentTime == null || strCurrentTime.Length != 14)
                return null;

            int year, month, day, hour, minute, second;

            if (int.TryParse(strCurrentTime.Substring(0, 4), out year) == false)
                return null;
            if (int.TryParse(strCurrentTime.Substring(4, 2), out month) == false)
                return null;
            if (int.TryParse(strCurrentTime.Substring(6, 2), out day) == false)
                return null;
            if (int.TryParse(strCurrentTime.Substring(8, 2), out hour) == false)
                return null;
            if (int.TryParse(strCurrentTime.Substring(10, 2), out minute) == false)
                return null;
            if (int.TryParse(strCurrentTime.Substring(12, 2), out second) == false)
                return null;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private void SendCloseEvent(CurrentAlarm alarm)
        {
            string strErrorMessage;
            SensorZoneHistory sensorZoneHistory = m_dataManager.GetSelectManager().SelectSensorZoneHistory(alarm.SensorZoneHistoryID, out strErrorMessage);

            if (sensorZoneHistory != null && sensorZoneHistory.AllSensorZoneIDs.Count > 0)
            {
                string strConditions = "";

                foreach (int nSensorZoneID in sensorZoneHistory.AllSensorZoneIDs)
                {
                    if (strConditions.Length == 0)
                        strConditions = nSensorZoneID.ToString();
                    else
                        strConditions += ", " + nSensorZoneID.ToString();
                }

                bool isNullable;
                strConditions = string.Format("{0}.{1} in ({2})", SensorZone.TableName, SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable), strConditions);

                ArrayList arrResult = m_dataManager.GetSelectManager().JoinSensorZoneTagInfo(null, null, strConditions, out strErrorMessage);

                if (arrResult == null)
                    return;

                int nDataCount = arrResult.Count;

                for (int i=0;i<nDataCount-1;i+=2)
                {
                    if (arrResult[i] is SensorZone && arrResult[i + 1] is TagInfo)
                    {
                        SensorZone sensorZone = (SensorZone)arrResult[i];
                        TagInfo tagInfo = (TagInfo)arrResult[i + 1];

                        ArrayList arrDatas = new ArrayList();

                        arrDatas.Add(sensorZone.SensorType);
                        arrDatas.Add(tagInfo.ID);
                        arrDatas.Add(tagInfo.SensorZoneID);
                        arrDatas.Add(false);

                        SopQueryManager mgr = new SopQueryManager();
                        mgr.SendAlarmQuery(arrDatas, "POST", m_strAlarmURL);
                    }
                }
            }
        }
    }
}
