using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSimulator
{
    public class Building
    {
        private int m_nBuildingGroupID = 1;
        private int m_nID = -1;
        private string m_strBuildingName = "";
        private int m_nMinFloor = 0;
        private int m_nMaxFloor = 0;

        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int MinFloor
        {
            get { return m_nMinFloor; }
            set { m_nMinFloor = value; }
        }

        public int MaxFloor
        {
            get { return m_nMaxFloor; }
            set { m_nMaxFloor = value; }
        }
    }

    public class Zone
    {
        private int m_nID = -1;
        private int m_nBuildingID = -1;
        private string m_strZoneName = "";
        private int m_nFloorIndex = 0;
        private List<EquipmentZone> m_equipZones = new List<EquipmentZone>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public List<EquipmentZone> EquipZones
        {
            get { return m_equipZones; }
        }
    }

    public class EquipmentZone
    {
        private int m_nID = -1;
        private int m_nZoneID = -1;
        private string m_strZoneName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }
    }
}
