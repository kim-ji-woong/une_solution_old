using System;
using System.Collections.Generic;
using dnsData.Alarm;
using SDMS.Model.History;
using SDMS.Model.Config;

namespace AgentFactory.BLL
{
    public abstract class BaseProcessManager
    {
        protected Factory m_factory = null;
        protected BaseProcessAgent m_processAgent = null;

        private static Dictionary<int, SensorReactionHistory.ReactionTypes> m_dicReactionType = null;
        private static Dictionary<int, SensorZoneHistory.DetectionType> m_dicDetectionStatus = null;

        public BaseProcessManager(Factory factory)
        {
            m_factory = factory;

            if (m_factory != null)
                m_processAgent = m_factory.MakeProcessAgent();
            else
                m_processAgent = new BaseProcessAgent();

            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, SensorReactionHistory.ReactionTypes>();

                foreach (SensorReactionHistory.ReactionTypes type in Enum.GetValues(typeof(SensorReactionHistory.ReactionTypes)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            if (m_dicDetectionStatus == null)
            {
                m_dicDetectionStatus = new Dictionary<int, SensorZoneHistory.DetectionType>();

                foreach (SensorZoneHistory.DetectionType status in Enum.GetValues(typeof(SensorZoneHistory.DetectionType)))
                {
                    m_dicDetectionStatus[(int)status] = status;
                }
            }
        }

        public static SensorReactionHistory.ReactionTypes ToReactionType(int nType)
        {
            SensorReactionHistory.ReactionTypes rType;
            if (m_dicReactionType.TryGetValue(nType, out rType))
                return rType;

            return SensorReactionHistory.ReactionTypes.ETC;
        }

        public static SensorZoneHistory.DetectionType ToDetectionStatus(int nStatus)
        {
            SensorZoneHistory.DetectionType status;
            if (m_dicDetectionStatus.TryGetValue(nStatus, out status))
                return status;

            return SensorZoneHistory.DetectionType.None;
        }

        // 새로운 알람이 탐지되었다.
        public abstract void NewAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs);
        // 알람에 관련된 센서 정보가 변경되었다.
        public abstract void UpdateAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs);
        // 탐지된 알람이 복구되었다.
        public abstract void ClearAlarm(AlarmData alarm);
        // 탐지된 알람이 실제상황으로 보고되었다.
        public abstract void ReportAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs);
        // 알람상태가 prevAlarm에서 alarm으로 바뀌었다.
        public abstract void ChangeAlarm(AlarmData alarm, AlarmData prevAlarm);

        // Return 값 : 문자발송이 필요한 상황이면 발신자 번호를 리턴한다.
        //             문자발송이 필요하지 않은 상황이면 null을 리턴한다.
        public abstract string NeedSMS(AlarmData alarm, out SMS.SMSMessageTypes messageType);
        public abstract string NeedSMSCaller();
        public abstract string NeedEmailCaller();
        public abstract bool NeedBroadcast(AlarmData alarm, out Broadcast.SituationTypes situationType);
    }
}
