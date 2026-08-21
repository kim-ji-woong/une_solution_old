using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace AgentFactory
{
    public abstract class BaseProcessManager
    {
        public enum ReactionType
        {
            BEGIN_STATUS = 0,              // 상황 시작
            RUN_BROADCAST = 10,            // 사내 방송 실시         
            SEND_SMS = 11,                 // 문자메시지 발송
            MALFUNCTION = 21,              // 오작동 처리
            NOTIFY_SIGNAL = 22,            // 재난 신고
            IGNORE_SIGNAL = 23,            // 재난 탐지신호 무시
            //TRAINNING_FIRE = 24,         // 
            RUN_SOP = 30,                  // SOP 발동 
            RUN_N_CANCEL_SOP = 31,         // SOP 실행중 취소
            FINISH_SOP = 32,               // SOP 종료
            IGNORE_SOP = 33,               // SOP 실행 안함
            END_STATUS = 50,               // 상황 종료
            //BEGIN_PSM_STATUS = 60,
            //IGNORE_PSM_DETECT = 61,
            CHANGE_ALARM_DEPTH = 62,
            USER_RESET = 64,
            /*CHANGE_PSM_ALARM_DEPTH = 62,
            NOTIFY_PSM = 63,
            PSM_USER_RESET = 64,
            END_PSM_STATUS = 70,*/
            ETC = 100,                     // 기타
            RUN_DETECT_BROADCAST = 101,
            RUN_REPORT_BROADCAST = 102,
            SEND_DETECT_SMS = 111,
            SEND_REPORT_SMS = 112,
            SEND_MALFUNCTION_SMS = 113,
            SEND_REPAIR_SMS = 114,

            /*NOTIFY_SECURITY = 898,
            BEGIN_S1SVMS_STATUS = 899,
            IGNORE_S1SVMS_STATUS = 919,
            END_S1SVMS_STATUS = 920,


            BEGIN_S1ACCESS_STATUS = 921,
            IGNORE_S1ACCESS_STATUS = 939,
            END_S1ACCESS_STATUS = 940,


            BEGIN_SECOM_STATUS = 961,
            IGNORE_SECOM_STATUS = 969,
            END_SECOM_STATUS = 970,*/

            TIME_OUT = 1000
        }

        public enum DetectionStatus
        {
            REAL = 1,
            MALFUNCTION = 2,    // 오동작
            TEST = 3,
            Unknown = 4
        }

        protected Factory m_factory = null;
        protected BaseProcessAgent m_processAgent = null;

        private static Dictionary<int, ReactionType> m_dicReactionType = null;
        private static Dictionary<int, DetectionStatus> m_dicDetectionStatus = null;

        public BaseProcessManager(Factory factory)
        {
            m_factory = factory;

            if (m_factory != null)
                m_processAgent = m_factory.MakeProcessAgent();
            else
                m_processAgent = new BaseProcessAgent();

            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, ReactionType>();

                foreach (ReactionType type in Enum.GetValues(typeof(ReactionType)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            if (m_dicDetectionStatus == null)
            {
                m_dicDetectionStatus = new Dictionary<int, DetectionStatus>();

                foreach (DetectionStatus status in Enum.GetValues(typeof(DetectionStatus)))
                {
                    m_dicDetectionStatus[(int)status] = status;
                }
            }
        }

        public static ReactionType ToReactionType(int nType)
        {
            ReactionType rType;
            if (m_dicReactionType.TryGetValue(nType, out rType))
                return rType;

            return ReactionType.ETC;
        }

        public static DetectionStatus ToDetectionStatus(int nStatus)
        {
            DetectionStatus status;
            if (m_dicDetectionStatus.TryGetValue(nStatus, out status))
                return status;

            return DetectionStatus.Unknown;
        }

        // 새로운 알람이 탐지되었다.
        public abstract void NewAlarm(DirectDBManager dbMgr, AlarmData alarm);
        // 탐지된 알람이 복구되었다.
        public abstract void ClearAlarm(DirectDBManager dbMgr, AlarmData alarm);
        // 탐지된 알람이 실제상황으로 보고되었다.
        public abstract void ReportAlarm(DirectDBManager dbMgr, AlarmData alarm);
        // 알람상태가 prevAlarm에서 alarm으로 바뀌었다.
        public abstract void ChangeAlarm(DirectDBManager dbMgr, AlarmData alarm, AlarmData prevAlarm);

        // Return 값 : 문자발송이 필요한 상황이면 발신자 번호를 리턴한다.
        //             문자발송이 필요하지 않은 상황이면 null을 리턴한다.
        public abstract string NeedSMS(DirectDBManager dbMgr, AlarmData alarm, out BaseSMSManager.SMSMessageType messageType);
        public abstract bool NeedBroadcast(DirectDBManager dbMgr, AlarmData alarm, out BaseBroadcastManager.SituationType situationType);
    }
}
