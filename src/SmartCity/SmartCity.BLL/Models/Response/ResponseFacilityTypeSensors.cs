using SmartCity.Model;
using System.Collections.Generic;

namespace SmartCity.BLL.Models.Response
{

    public class ResponseFacilityTypeSensors : MessageResult
    {
        private List<Sensor> m_listSensor = null;

        public List<Sensor> FacilityTypeSensors
        {
            get { return m_listSensor; }
            set { m_listSensor = value; }
        }
    }
}
