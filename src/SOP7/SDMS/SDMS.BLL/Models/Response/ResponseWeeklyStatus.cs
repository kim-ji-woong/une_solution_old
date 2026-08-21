using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    using SDMS.BLL.Models.Data;
    using SDMS.Model.History;
    using System;

    public class ResponseWeeklyStatus : MessageResult
    {
        private List<AlarmInfo> m_alarmInfos = null;

        public List<AlarmInfo> AlarmInfos
        {
            get { return m_alarmInfos; }
            set { m_alarmInfos = value; }
        }
    }

    public class AlarmInfo
    {
        DateTime m_dtTime = new DateTime();
        int? m_nOrgSensorID = null;
        int m_nFacilityType = -1;
        int m_nSensorZoneID = -1;
        int m_nZoneID = -1;
        int m_nBuildingID = -1;
        int m_nBuildingGroupID = -1;
        int? m_nMaterialType = null;

        public DateTime Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        public int? OrgSensorID
        {
            get { return m_nOrgSensorID; }
            set { m_nOrgSensorID = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public int? MaterialType
        {
            get { return m_nMaterialType; }
            set { m_nMaterialType = value; }
        }
    }


}
