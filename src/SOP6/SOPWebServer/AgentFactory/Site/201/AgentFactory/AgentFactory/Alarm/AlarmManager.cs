using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;

namespace AgentFactory.Alarm
{
    public static class AlarmManager
    {
        public enum AlarmStep { None = 0, Step1, Step2, Step3, Step4 };

        private static ConcurrentDictionary<AlarmData, AlarmStep> m_dicAlarmSteps = new ConcurrentDictionary<AlarmData, AlarmStep>();

        public static ClientMessage MakeClientMessage(AlarmData alarm, AlarmStep step, int nClientType, int nClientSubType)
        {
            List<AlarmData> alarms = new List<AlarmData>();
            alarms.Add(alarm);
            return MakeClientMessage(alarms, step, nClientType, nClientSubType);
        }

        public static ClientMessage MakeClientMessage(List<AlarmData> alarms, AlarmStep step, int nClientType, int nClientSubType, bool updateStep = true)
        {
            ClientMessage message = new ClientMessage();

            message.ClientType = nClientType;
            message.ClientSubType = nClientSubType;
            message.Header = SOPWebServer.Header.ALARM_STEP;

            int nAlarmCount = alarms.Count;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)step);
            arrDatas.Add(nAlarmCount);

            foreach (AlarmData alarm in alarms)
            {
                arrDatas.Add(alarm.SensorZoneID);

                if (updateStep)
                    SetAlarmStep(alarm, step);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            message.Bytes = bytes;

            return message;
        }

        public static void SetAlarmStep(AlarmData alarm, AlarmStep step)
        {
            m_dicAlarmSteps[alarm] = step;
        }

        public static void RemoveAlarmStep(AlarmData alarm)
        {
            AlarmStep step;
            m_dicAlarmSteps.TryRemove(alarm, out step);
        }

        public static AlarmStep GetAlarmStep(AlarmData alarm)
        {
            AlarmStep step;

            if (m_dicAlarmSteps.TryGetValue(alarm, out step))
                return step;

            return AlarmStep.None;
        }
    }
}
