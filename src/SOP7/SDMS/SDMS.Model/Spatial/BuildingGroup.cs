using UnE.Geometry;

namespace SDMS.Model.Spatial
{
    /// <summary>
    /// 건물그룹
    /// 여러개의 건물들이 모여 건물그룹을 이룬다.
    /// </summary>
    public class BuildingGroup : IIDObject
    {
        public enum Fields { ID, GroupName, ParentID, TextCenter, DisplayText, SiteID };

        private int m_nID = -1;
        private string m_strGroupName = "";
        // 건물그룹은 계층구조를 가질수 있다.
        private int? m_nParentID = null;
        // 3D 또는 2D 상에 표시할 때 Text 위치
        private Vertex3D m_vTextCenter = null;
        // 화면에 표시할 이름(Null이면 GroupName이 사용된다.)
        private string m_strDisplayText = null;
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        /// <summary>
        /// 건물그룹은 계층구조를 가질수 있다.
        /// </summary>
        public int? ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }

        /// <summary>
        /// 3D 또는 2D 상에 표시할 때 Text 위치
        /// </summary>
        public Vertex3D TextCenter
        {
            get { return m_vTextCenter; }
            set { m_vTextCenter = value; }
        }

        /// <summary>
        /// 화면에 표시할 이름(Null이면 GroupName이 사용된다.)
        /// </summary>
        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SdmsSpatialBuildingGroup"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.GroupName ||
                field == Fields.SiteID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
