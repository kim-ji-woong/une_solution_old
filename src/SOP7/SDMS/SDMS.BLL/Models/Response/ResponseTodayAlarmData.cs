using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    using SDMS.BLL.Models.Alarm;
    using SDMS.BLL.Models.Data;
    using SDMS.Model.CCTV;
    using SDMS.Model.History;
    using SDMS.Model.Sensor;

    public class ResponseTodayAlarmData : MessageResult
    {
        private List<AlarmData> m_alarmDatas = null;

        public List<AlarmData> AlarmDatas
        {
            get { return m_alarmDatas; }
            set { m_alarmDatas = value; }
        }
    }
}
