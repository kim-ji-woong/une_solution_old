using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    using SDMS.BLL.Models.Data;
    using SDMS.Model.CCTV;
    using SDMS.Model.Sensor;

    public class ResponseFacilityType : MessageResult
    {
        private FacilityType m_facilityType = null;

        public FacilityType FacilityType
        {
            get { return m_facilityType; }
            set { m_facilityType = value; }
        }
    }
}
