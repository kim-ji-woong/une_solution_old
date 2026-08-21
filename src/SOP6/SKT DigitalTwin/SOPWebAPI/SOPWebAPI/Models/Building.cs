using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOPWebAPI.Models
{
    public class Building
    {
        private int m_nID = -1;
        private string m_strCode = "";
        private string m_strName = "";
        private int m_nMinFloorIndex = 0;
        private int m_nMaxFloorIndex = 0;
        private BuildingGroup m_buildingGroup = null;
        private string m_strSiteID = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SiteID
        {
            get { return m_strSiteID; }
            set { m_strSiteID = value; }
        }

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public int MinFloorIndex
        {
            get { return m_nMinFloorIndex; }
            set { m_nMinFloorIndex = value; }
        }

        public int MaxFloorIndex
        {
            get { return m_nMaxFloorIndex; }
            set { m_nMaxFloorIndex = value; }
        }

        public BuildingGroup BuildingGroup
        {
            get { return m_buildingGroup; }
            set { m_buildingGroup = value; }
        }
    }

    public class BuildingGroup
    {
        private int m_nID = -1;
        private BuildingGroup m_parentGroup = null;
        private string m_strName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public BuildingGroup ParentGroup
        {
            get { return m_parentGroup; }
            set { m_parentGroup = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
    }
}