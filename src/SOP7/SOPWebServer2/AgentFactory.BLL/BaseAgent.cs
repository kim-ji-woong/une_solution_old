using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using SDMS.Model.Sensor;

namespace AgentFactory.BLL
{
    public abstract class BaseAgent
    {
        private ConcurrentQueue<ArrayList> m_queueTimerDatas = new ConcurrentQueue<ArrayList>();

        // BaseClient.OnTimer Event에서 사용할 데이터
        public ConcurrentQueue<ArrayList> TimerDatas
        {
            get { return m_queueTimerDatas; }
        }

        // Default : Factory에서 아무것도 수행하지 않고, 호출한 쪽에서 알아서 처리하도록 한다.
        // FactoryOnly : Factory에서 모든것을 처리하게 되며, 호출한 쪽은 아무 신경도 쓰지 않는다.
        // PostProcess : 호출한 쪽에서 먼저 처리를 하고 그 후에 Factory에서 처리한다.
        // PreProcess : Factory에서 먼저 처리를 하고, 호출한 쪽에서 나중에 처리한다.
        public enum MethodProcessType { Default, FactoryOnly, PostProcess, PreProcess };
        public enum MethodType { OnLoad, OnTimer, OnReceive, OnClose, Etc };

        public abstract MethodProcessType CheckMethod(MethodType type, params object[] args);
        public abstract object RunMethod(MethodType type, params object[] args);

        public virtual int GetAlarmDepth(IAlarmManager alarmManager, KeyValuePair<SensorZone, int>[] sensorDatas, SensorZone sensorZone)
        {
            return 1;
        }
    }
}
