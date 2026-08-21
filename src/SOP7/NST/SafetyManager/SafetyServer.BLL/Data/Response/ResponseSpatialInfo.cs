using System.Collections.Generic;

namespace SafetyServer.BLL.Data.Response
{
    using Spatial;

    public class ResponseSpatialInfo
    {
        private List<BuildingGroupData> m_buildingGroups = new List<BuildingGroupData>();
        private List<ZoneData> m_outdoorZones = new List<ZoneData>();

        public List<BuildingGroupData> BuildingGroups
        {
            get { return m_buildingGroups; }
            set { m_buildingGroups = value; }
        }

        public List<ZoneData> OutdoorFields
        {
            get { return m_outdoorZones; }
            set { m_outdoorZones = value; }
        }
    }
}
