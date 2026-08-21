using System.Collections.Generic;
using dnsData.Alarm;

namespace AgentFactory.BLL
{
    public interface IAlarmManager
    {
        ICollection<AlarmData> CurrentAlarms
        {
            get;
        }

        // CurrentAlarm을 삭제한다.
        // DB에서 CurrentAlarm Table만 지운다.
        void RemoveCurrentAlarm(int nSensorZoneHistoryID);
    }
}
