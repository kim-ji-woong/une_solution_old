using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspUrlEditor
{
    public class BuildingGroup
    {
        private int m_nID = -1;
        private string m_strName = "";
        private bool m_isOutdoor = false;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public bool IsOutdoor
        {
            get { return m_isOutdoor; }
            set { m_isOutdoor = value; }
        }

        public override string ToString()
        {
            return m_strName;
        }
    }

    public class Building
    {
        private int m_nBuildingGroupID = -1;
        private string m_strBuildingGroupName = "";
        private int m_nID = -1;
        private string m_strBuildingName = "";
        private List<Zone> m_zones = new List<Zone>();

        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public string BuildingGroupName
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
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

        public List<Zone> Zones
        {
            get { return m_zones; }
        }

        public override string ToString()
        {
            return m_strBuildingName;
        }
    }
    public class Zone : IComparable
    {
        private int m_nBuildingID = -1;
        private string m_strBuildingName = "";
        private int m_nID = -1;
        private string m_strZoneName = "";
        private float m_fFloorIndex = 0.0f;
        private List<EquipmentZone> m_equipZones = new List<EquipmentZone>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public float FloorIndex
        {
            get { return m_fFloorIndex; }
            set { m_fFloorIndex = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public List<EquipmentZone> EquipZones
        {
            get { return m_equipZones; }
        }

        public override string ToString()
        {
            return m_strZoneName;
        }

        public int CompareTo(object obj)
        {
            Zone zone = (Zone)obj;

            if (this.BuildingID < 0)
            {
                return string.Compare(this.m_strZoneName, zone.m_strZoneName);
            }

            if (this.FloorIndex > zone.FloorIndex)
                return 1;
            else if (this.FloorIndex < zone.FloorIndex)
                return -1;
            //else
            return 0;
        }
    }

    public class EquipmentZone
    {
        private int m_nID = -1;
        private string m_strName = "";
        
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
    }

    public class CCTV
    {
        private int m_nID = -1;
        private string m_strCCTVName = "";
        private Zone m_zone = null;
        private string m_strURL = "";
        private bool m_isNew = false;

        public bool NewCCTV
        {
            get { return m_isNew; }
            set { m_isNew = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CCTVName
        {
            get { return m_strCCTVName; }
            set { m_strCCTVName = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }
    }
}
