using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace AgentFactory
{
    public abstract class BaseSMSManager
    {
        public enum SMSMessageType
        {
            UNKNOWN = -1,
            RESET_FIRE = 0,     // 화재복구(0)
            DETECT_FIRE,        // 화재탐지(1)
            REPORT_FIRE,        // 화재신고(2)
            DETECT_PSM,         // 누출탐지(3)
            REPORT_PSM,         // 누출신고(4)
            RESET_PSM,          // 누출복구(5)
            DETECT_SECURITY,    // 방범탐지(6)
            REPORT_SECURITY,    // 방범신고(7)
            RESET_SECURITY,     // 방범복구(8)
            DETECT_EARTHQUAKE,  // 지진탐지(9)
            DETECT_TH,          // 온도/습도 탐지(10)
            RESET_TH,           // 온도/습도 복구(11)
            RESET_ETC,          // ETC 복구
            DETECT_ETC,         // ETC 탐지
            REPORT_ETC          // ETC 신고
        }

        protected Factory m_factory = null;

        public BaseSMSManager(Factory factory)
        {
            m_factory = factory;
        }

        public abstract int GetPhoneNumbers(DirectDBManager dbMgr, AlarmData alarm, SMSMessageType type);
        // 훈련모드일 경우 훈련모드에 맞는 태그문구를 리턴한다.
        // 그렇지 않을 경우 빈 문자열을 리턴한다.
        public abstract string GetTrainingModeString(DirectDBManager dbMgr);
        public abstract int SendSMS(DirectDBManager dbMgr, string strCaller, List<string> phoneNumbers, string strMessage, int nSensorReactionHistoryID);
        public abstract int SendSMS(DirectDBManager dbMgr, AlarmData alarm, string strCaller, List<string> phoneNumbers, List<int> regularMemberIDs, List<int> externalMemberIDs, string strMessage, int nSensorReactionHistoryID);
    }
}
