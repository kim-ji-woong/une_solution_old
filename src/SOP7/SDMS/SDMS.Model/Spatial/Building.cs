using UnE.Geometry;

namespace SDMS.Model.Spatial
{
    public class Building : IIDObject
    {
        public enum Fields { ID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor, TextCenter, BroadcastText, DisplayText };

        private int m_nID = -1;
        private string m_strBuildingCode = "";
        private string m_strBuildingName = "";
        private int m_nBuildingGroupID = -1;
        // 건물 가장 꼭대기 층(1층이면 0, 2층이면 1, 지하일 경우 음수)
        private int m_nMaxFloor = 0;
        // 건물 가장 아래층(1층이면 0, 2층이면 1, 지하일 경우 음수)
        private int m_nMinFloor = 0;
        // 3D 또는 2D 상에 표시할 때 Text 위치
        private Vertex3D m_vTextCenter = null;
        // 방송용 이름(한글)
        private string m_strBroadcastText = null;
        // 화면에 표시할 이름(Null이면 GroupName이 사용된다.)
        private string m_strDisplayText = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingCode
        {
            get { return m_strBuildingCode; }
            set { m_strBuildingCode = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }
        /// <summary>
        /// 건물 가장 꼭대기 층(1층이면 0, 2층이면 1, 지하일 경우 음수)
        /// </summary>
        public int MaxFloor
        {
            get { return m_nMaxFloor; }
            set { m_nMaxFloor = value; }
        }

        /// <summary>
        /// 건물 가장 아래층(1층이면 0, 2층이면 1, 지하일 경우 음수)
        /// </summary>
        public int MinFloor
        {
            get { return m_nMinFloor; }
            set { m_nMinFloor = value; }
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

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.TextCenter ||
                field == Fields.BroadcastText ||
                field == Fields.DisplayText)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsSpatialBuilding"; }
        }
    }
}
