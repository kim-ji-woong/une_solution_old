using UnE.Geometry;

namespace SDMS.Model.Spatial
{
    /// <summary>
    /// 건물내 하나의 층을 나타내거나 건물외부의 외부영역을 나타낸다.
    /// </summary>
    public class Zone : IIDObject
    {
        public enum Fields { ID, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, TextCenter, BroadcastText, DisplayText, SiteID };

        private int m_nID = -1;
        private string m_strZoneName = "";
        // 건물외부를 표현할 경우 m_nBuildingID는 null이다.
        private int? m_nBuildingID = null;
        // 건물외부를 표현할 경우 m_nFloorIndex는 null이다.
        // 1층이면 0, 2층이면 1, 지하일 경우 음수
        private int? m_nFloorIndex = null;
        // 1.4층, 2.5층과 같은 층을 나타내기 위한 소수점
        private float? m_fAddFloor = null;
        // Zone의 외곽영역
        private Polygon m_boundary = null;
        // 3D 또는 2D 상에 표시할 때 Text 위치
        private Vertex3D m_vTextCenter = null;
        // 방송용 이름(한글)
        private string m_strBroadcastText = null;
        // 화면에 표시할 이름(Null이면 GroupName이 사용된다.)
        private string m_strDisplayText = null;
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        /// <summary>
        /// 건물외부를 표현할 경우 m_nBuildingID는 null이다.
        /// </summary>
        public int? BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        /// <summary>
        /// 건물외부를 표현할 경우 m_nFloorIndex는 null이다.
        /// 1층이면 0, 2층이면 1, 지하일 경우 음수
        /// </summary>
        public int? FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        /// <summary>
        /// 1.4층, 2.5층과 같은 층을 나타내기 위한 소수점
        /// </summary>
        public float? AddFloor
        {
            get { return m_fAddFloor; }
            set { m_fAddFloor = value; }
        }

        /// <summary>
        /// Zone의 외곽영역
        /// </summary>
        public Polygon Boundary
        {
            get { return m_boundary; }
            set { m_boundary = value; }
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
        /// 방송용 이름(한글)
        /// </summary>
        public string BroadcastText
        {
            get { return m_strBroadcastText; }
            set { m_strBroadcastText = value; }
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
            get { return "SdmsSpatialZone"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.SiteID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
