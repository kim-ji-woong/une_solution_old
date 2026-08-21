using System;
using System.Collections.Generic;
using System.Text;

namespace SOPSimulator.BLL.Models.Response
{
    public class ResponseSensorName
    {
        private string m_strSensorName = "";
        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }
    }
}
