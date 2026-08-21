using System.Collections.Generic;
using SDMS.Model.Sensor;

namespace AgentFactory.BLL.Agent
{
    public class SecurityAgent : BaseAgent
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
            return 2;
        }
    }
}
