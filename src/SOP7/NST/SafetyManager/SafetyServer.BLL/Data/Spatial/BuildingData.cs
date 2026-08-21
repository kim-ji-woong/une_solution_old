using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafetyServer.BLL.Data.Spatial
{
    public class BuildingData
    {
        private int m_nBuildingID = -1;
        private string m_strBuildingName = "";
        private List<ZoneData> m_zones = new List<ZoneData>();

        public int ID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public string Name
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public List<ZoneData> Fields
        {
            get { return m_zones; }
            set { m_zones = value; }
        }
    }
}
