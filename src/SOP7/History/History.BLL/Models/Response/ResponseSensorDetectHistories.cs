using History.BLL.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseSensorDetectHistories
    {
        private List<SensorDetectHistoryData> m_sensorDetectHistoryDatas = new List<SensorDetectHistoryData>();
        public List<SensorDetectHistoryData> SensorDetectHistoryDatas
        {
            get { return m_sensorDetectHistoryDatas; }
            set { m_sensorDetectHistoryDatas = value; }
        }

        private int m_nLastSensorReactionHistoryID = -1;
        public int LastSensorReactionHistoryID
        {
            get { return m_nLastSensorReactionHistoryID; }
            set { m_nLastSensorReactionHistoryID = value; }
        }
    }
}
