using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace AgentFactory
{
    public class BaseProcessAgent
    {
        // Return 값 : Client에게 전달할 내용이 있을때 List에 담아 전달한다.
        // 
        public virtual List<ClientMessage> PrevNewAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostNewAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PrevChangeAlarm(DirectDBManager dbMgr, AlarmData alarm, AlarmData prevAlarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostChangeAlarm(DirectDBManager dbMgr, AlarmData alarm, AlarmData prevAlarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PrevReportAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostReportAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PrevClearAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }

        public virtual List<ClientMessage> PostClearAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            return null;
        }
    }
}
