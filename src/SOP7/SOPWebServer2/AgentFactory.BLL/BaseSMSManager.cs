using System.Collections.Generic;
using dnsData.Alarm;
using SDMS.Model.Config;

namespace AgentFactory.BLL
{
    public abstract class BaseSMSManager
    {
        protected Factory m_factory = null;

        public BaseSMSManager(Factory factory)
        {
            m_factory = factory;
        }

        public abstract int GetPhoneNumbers(AlarmData alarm, SMS.SMSMessageTypes type);
        // 훈련모드일 경우 훈련모드에 맞는 태그문구를 리턴한다.
        // 그렇지 않을 경우 빈 문자열을 리턴한다.
        public abstract string GetTrainingModeString();
        public abstract int SendSMS(string strCaller, ICollection<string> phoneNumbers, string strMessage, int nSensorReactionHistoryID);
        public abstract int SendSMS(AlarmData alarm, string strCaller, ICollection<string> phoneNumbers, ICollection<int> regularMemberIDs, string strMessage, int nSensorReactionHistoryID);
    }
}
