using System.Collections.Generic;
using dnsData.Alarm;
using SDMS.Model.Config;

namespace AgentFactory.BLL
{
    public abstract class BaseEmailManager
    {
        protected Factory m_factory = null;

        public BaseEmailManager(Factory factory)
        {
            m_factory = factory;
        }

        //public abstract int SendSMS(string strCaller, ICollection<string> phoneNumbers, string strMessage, int nSensorReactionHistoryID);
        public abstract int SendEmail(AlarmData alarm, string strCaller, ICollection<string> listEmail, ICollection<int> regularMemberIDs, string strMessage, int nSensorReactionHistoryID);
    }
}
