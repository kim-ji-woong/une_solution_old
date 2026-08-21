using UnE.Geometry;
using System.Collections.Generic;

namespace SDMS.Model.Spatial
{
    /// <summary>
    /// 설비영역은 센서에 대한 영역이다.
    /// 특정 센서가 작동했을때 이 센서가 대표하는 구역을 의미한다.
    /// 하나의 Zone내에 있을수도 있고, 여러 Zone에 걸쳐 있을수도 있다.
    /// </summary>
    public class EquipmentZone : IIDObject
    {
        public enum Fields { ID, ZoneName, Boundary, LinkedZoneIDList, Type, TextCenter, BroadcastText, DisplayText, SiteID };

        private int m_nID = -1;
        private string m_strZoneName = "";
        // 외곽영역 정보
        private Polygon m_boundary = null;
        // 연결된 Zone ID List
        private List<int> m_linkedZoneIDs = new List<int>();
        // 설비영역 타입
        private int? m_nType = null;
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
        /// 연결된 Zone ID List
        /// </summary>
        public List<int> LinkedZoneIDs
        {
            get { return m_linkedZoneIDs; }
            set { m_linkedZoneIDs = value; }
        }

        /// <summary>
        /// 설비영역 타입
        /// </summary>
        public int? Type
        {
            get { return m_nType; }
            set { m_nType = value; }
        }

        /// <summary>
        /// 외곽영역 정보
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
            get { return "SdmsSpatialEquipmentZone"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.ZoneName ||
                field == Fields.LinkedZoneIDList ||
                field == Fields.SiteID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
