using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    using SDMS.BLL.Models.Data;
    using SDMS.Model.CCTV;

    public class ResponseEquipZoneCCTV : MessageResult
    {
        private EquipZoneCCTV m_equipZoneCCTV = null;

        public EquipZoneCCTV EquipZoneCCTV
        {
            get { return m_equipZoneCCTV; }
            set { m_equipZoneCCTV = value; }
        }
    }

    public class ResponseEquipZoneCCTVFromSensor : MessageResult
    {
        private int m_nEquipZoneID = -1;
        private string m_strEquipZoneDisplayName = "";
        private EquipZoneCCTV m_equipZoneCCTV = null;

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string EquipZoneDisplayName
        {
            get { return m_strEquipZoneDisplayName; }
            set { m_strEquipZoneDisplayName = value; }
        }

        public EquipZoneCCTV EquipZoneCCTV
        {
            get { return m_equipZoneCCTV; }
            set { m_equipZoneCCTV = value; }
        }
    }
}
