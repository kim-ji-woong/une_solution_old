using UnE.Geometry;

namespace SDMS.Model.Spatial
{
    /// <summary>
    /// Zone과 연결된 데이터들에 대한 기본값들을 정의한다.
    /// </summary>
    public class ZoneData
    {
        public enum Fields { ZoneID, FakeWallElevation, PoiElevation };

        private int m_nZoneID = -1;
        // 가벽의 바닥높이
        private float? m_fFakeWallElevation = null;
        // POI 높이
        private float? m_fPoiElevation = null;
        
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        /// <summary>
        /// 가벽의 바닥높이
        /// </summary>
        public float? FakeWallElevation
        {
            get { return m_fFakeWallElevation; }
            set { m_fFakeWallElevation = value; }
        }

        /// <summary>
        /// POI 높이
        /// </summary>
        public float? PoiElevation
        {
            get { return m_fPoiElevation; }
            set { m_fPoiElevation = value; }
        }

        public static string TableName
        {
            get { return "SdmsSpatialZoneData"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ZoneID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
