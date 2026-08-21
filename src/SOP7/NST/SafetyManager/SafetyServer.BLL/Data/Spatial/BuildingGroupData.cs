using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafetyServer.BLL.Data.Spatial
{
    public class BuildingGroupData
    {
        private int m_nBuildingGroupID = -1;
        private string m_strBuildingGroupName = "";
        private List<BuildingData> m_buildings = new List<BuildingData>();

        public int ID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public string Name
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }

        public List<BuildingData> Buildings
        {
            get { return m_buildings; }
            set { m_buildings = value; }
        }
    }
}
