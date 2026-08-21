using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WifiSensorService.Data.Response
{
    using Request;

    public class ResponseSensorData : MessageResult
    {
        private List<SensorData> m_sensorDatas = new List<SensorData>();

        public List<SensorData> SensorDatas
        {
            get { return m_sensorDatas; }
            set { m_sensorDatas = value; }
        }

        public ResponseSensorData()
            : base()
        {
        }

        public ResponseSensorData(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
