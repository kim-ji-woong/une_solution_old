using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public class Building
    {
        private int m_nID = 0;
        private string m_strBuildingName = "";
        private int m_nMinFloor = 0;
        private int m_nMaxFloor = 0;

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

        public override string ToString()
        {
            return m_strBuildingName;
        }
    }
}
