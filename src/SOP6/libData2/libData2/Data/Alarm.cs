using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.Alarm
{
    public enum AlarmType
    {
        // 모든 타입의 알람 해제를 의미한다.
        NO_ALARM = 0,
        // 모든 타입의 알람 발생을 의미한다.
        ALARM = 1,
        // 통신 연결 끊김
        NOT_CONNECTED = 2,
        // 통신 재개
        CONNECTED = 3,
        // PSM의 알람 단계
        PSM_ALARM_1 = 21,
        PSM_ALARM_2 = 22,
        PSM_ALARM_3 = 23
    }
}
