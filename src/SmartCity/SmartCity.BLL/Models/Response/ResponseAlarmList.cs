using SmartCity.Model;
using System.Collections.Generic;

namespace SmartCity.BLL.Models.Response
{

    public class ResponseAlarmList : MessageResult
    {
        private List<AlertAlarm> m_listAlarm = null;

        public List<AlertAlarm> Alarms
        {
            get { return m_listAlarm; }
            set { m_listAlarm = value; }
        }
    }

    
}
