using System.Collections.Generic;

namespace SDMS.Model.Spatial
{
    public class FakeWall : IIDObject
    {
        public enum Fields { ID, ZoneID, X, Y, Z, Rotate, Scale };

        private int m_nID = -1;
        private int m_nZoneID = -1;
        private float x = 0;
        private float y = 0;
        private float z = 0;
        // Radian
        private float m_fRotate = 0;
        private float m_fScale = 0;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        // Radian
        public float Rotate
        {
            get { return m_fRotate; }
            set { m_fRotate = value; }
        }

        public float Scale
        {
            get { return m_fScale; }
            set { m_fScale = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsSpatialFakeWall"; }
        }
    }
}
