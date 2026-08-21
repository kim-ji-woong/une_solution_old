using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafetyServer.BLL.Data.Spatial
{
    public class ZoneData
    {
        private int m_nZoneID = -1;
        private int? m_nFloorIndex = null;
        private string m_strZoneName = "";

        public int ID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int? FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public string Name
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }
    }
}
