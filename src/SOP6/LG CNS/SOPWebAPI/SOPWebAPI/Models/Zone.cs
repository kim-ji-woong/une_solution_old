using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOPWebAPI.Models
{
    public class Zone
    {
        private int m_nID = -1;
        private Building m_building = null;
        private string m_strName = "";
        private int m_nFloorIndex = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }
    }
}