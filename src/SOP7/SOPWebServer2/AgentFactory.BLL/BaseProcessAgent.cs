using System.Collections.Generic;
using dnsData.Alarm;

namespace AgentFactory.BLL
{
    public class BaseProcessAgent
    {
        // Return 값 : Client에게 전달할 내용이 있을때 List에 담아 전달한다.
        // 
        public virtual List<ClientMessage> PrevNewAlarm(AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostNewAlarm(AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PrevChangeAlarm(AlarmData alarm, AlarmData prevAlarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostChangeAlarm(AlarmData alarm, AlarmData prevAlarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PrevReportAlarm(AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostReportAlarm(AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PrevClearAlarm(AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostClearAlarm(AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }
    }
}
