using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVAlarmWatcher
{
    public enum AlarmType
    {
        Fire = 0,
        PSM,
        Security
    }

    public interface IAlarmOwner
    {
        void OnAlarmOn(AlarmType alarmType, int nCCTVID, DateTime timeStamp);
        void OnAlarmOff(int nCCTVID, DateTime timeStamp);
        void OnAlarmOn2(AlarmType alarmType, int nEquipZoneID, DateTime timeStamp);
        void OnAlarmOff2(int nEquipZoneID, DateTime timeStamp);
    }
}
