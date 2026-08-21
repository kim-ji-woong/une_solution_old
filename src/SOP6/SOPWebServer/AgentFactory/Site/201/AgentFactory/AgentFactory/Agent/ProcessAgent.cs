using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections.Concurrent;

namespace AgentFactory.Agent
{
    class ProcessAgent : BaseProcessAgent
    {
        public override List<ClientMessage> PrevNewAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            List<ClientMessage> messages = null;

            if (alarm.SensorType == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
            {
                List<AlarmData> alarms = alarmManager.CurrentAlarms;
                messages = Alarm.FireAlarmManager.CheckNewAlarm(dbMgr, alarm, alarms);
            }

            return messages;
        }

        public override List<ClientMessage> PrevChangeAlarm(DirectDBManager dbMgr, AlarmData alarm, AlarmData prevAlarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public override List<ClientMessage> PrevReportAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public override List<ClientMessage> PrevClearAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            List<ClientMessage> messages = null;

            if (alarm.SensorType == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
            {
                List<AlarmData> alarms = alarmManager.CurrentAlarms;
                messages = Alarm.FireAlarmManager.CheckClearAlarm(dbMgr, alarm, alarms);
            }

            return messages;
        }
    }
}
