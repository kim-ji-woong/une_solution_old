using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    using SDMS.BLL.Models.Data;

    public class ResponseBuildingGroupList : MessageResult
    {
        private List<BuildingGroupData> m_buildingGroups = null;
        private List<ZoneData> m_outdoorZones = new List<ZoneData>();

        public List<BuildingGroupData> BuildingGroups
        {
            get { return m_buildingGroups; }
            set { m_buildingGroups = value; }
        }

        public List<ZoneData> OutdoorZones
        {
            get { return m_outdoorZones; }
        }
    }
}
