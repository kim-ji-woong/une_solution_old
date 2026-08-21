using System.Collections.Generic;
using SDMS.Model.Sensor;

namespace AgentFactory.BLL.Agent
{
    public class FireAgent : BaseAgent
    {
        public override MethodProcessType CheckMethod(MethodType type, params object[] args)
        {
            return MethodProcessType.Default;
        }

        public override object RunMethod(MethodType type, params object[] args)
        {
            return null;
        }

        public override int GetAlarmDepth(IAlarmManager alarmManager, KeyValuePair<SensorZone, int>[] sensorDatas, SensorZone sensorZone)
        {
            int nDetectCount = 0;
            bool contains = false;

            foreach (KeyValuePair<SensorZone, int> pair in sensorDatas)
            {
                if (pair.Value > 0)
                    nDetectCount++;

                if (sensorZone != null && pair.Key == sensorZone)
                {
                    if (pair.Value > 0)
                        contains = true;
                }
            }

            if (sensorZone != null && contains == false)
                nDetectCount++;

            // 한 구역에 1개의 센서가 탐지되면 주의
            // 한 구역에 2개의 센서가 탐지되면 경계
            // 한 구역에 3개 이상의 센서가 탐지되면 심각
            int nAlarmDepth = nDetectCount + 1;

            if (nAlarmDepth > 4)
                nAlarmDepth = 4;

            return nAlarmDepth;
        }
    }
}
