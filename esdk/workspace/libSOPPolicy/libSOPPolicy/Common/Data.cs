using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSOPPolicy
{
    public class BuildingGroup
    {
        public enum GroupType { Normal = 0, SmallGroup, City };

        private int m_nID = -1;
        private BuildingGroup m_parentGroup = null;
        private string m_strGroupName = "";
        private GroupType m_type = GroupType.Normal;

        private static Dictionary<int, GroupType> m_dicGroupType = null;

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

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public GroupType GetGroupType()
        {
            return m_type;
        }

        public void SetGroupType(GroupType type)
        {
            m_type = type;
        }

        public static GroupType ToGroupType(int nType)
        {
            if (m_dicGroupType == null)
            {
                m_dicGroupType = new Dictionary<int, GroupType>();

                foreach (GroupType type in Enum.GetValues(typeof(GroupType)))
                {
                    m_dicGroupType[(int)type] = type;
                }
            }

            GroupType gType;

            if (m_dicGroupType.TryGetValue(nType, out gType))
                return gType;

            return GroupType.Normal;
        }
    }

    public class Building
    {
        private int m_nID = -1;
        private string m_strBuildingID = "";
        private string m_strBuildingCode = "";
        private BuildingGroup m_buildingGroup = null;
        private string m_strBuildingName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingID
        {
            get { return m_strBuildingID; }
            set { m_strBuildingID = value; }
        }

        public string BuildingCode
        {
            get { return m_strBuildingCode; }
            set { m_strBuildingCode = value; }
        }

        public BuildingGroup BuildingGroup
        {
            get { return m_buildingGroup; }
            set { m_buildingGroup = value; }
        }
        
        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }
    }
}
