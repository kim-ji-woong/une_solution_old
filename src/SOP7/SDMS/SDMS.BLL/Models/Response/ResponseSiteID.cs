using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    using SDMS.BLL.Models.Data;
    using SDMS.Model.CCTV;
    using SDMS.Model.Sensor;

    public class ResponseSiteID : MessageResult
    {
        private int? m_nSiteID = null;

        public int? SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
    }
}
