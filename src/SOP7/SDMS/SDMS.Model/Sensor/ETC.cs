namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorETC ETC 센서
    /// </summary>
    public class ETC : IIDObject
    {
        public enum Fields { ID, Name, PositionName, X, Y, Z, CurrentData, ZoneID, Department, DepartmentPhoneNumber, Enabled, Status, UniqueKey, MaterialType };

        private int m_nID = -1;
        // 센서 이름
        private string m_strName = "";
        // 센서 설치 위치
        private string m_strPositionName = null;
        private float? m_x = null;
        private float? m_y = null;
        private float? m_z = null;
        private string m_strCurrentData = null;
        private int m_nZoneID = -1;
        // 센서 탐지시 연락해야할(혹은 조치해야할) 부서
        private string m_strDepartment = null;
        // 센서 탐지시 연락해야할(혹은 조치해야할) 부서의 전화번호
        private string m_strDepartmentPhoneNumber = null;
        private bool? m_enabled = null;
        private string m_strStatus = null;
        private string m_strUniqueKey = null;
        private int? m_nMaterialType = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        /// <summary>
        /// 센서 이름
        /// </summary>
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// 센서 설치 위치
        /// </summary>
        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }

        public float? X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public float? Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public float? Z
        {
            get { return m_z; }
            set { m_z = value; }
        }

        /// <summary>
        /// 센서 수치값
        /// </summary>
        public string CurrentData
        {
            get { return m_strCurrentData; }
            set { m_strCurrentData = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        /// <summary>
        /// 센서 탐지시 연락해야할(혹은 조치해야할) 부서
        /// </summary>
        public string Department
        {
            get { return m_strDepartment; }
            set { m_strDepartment = value; }
        }

        /// <summary>
        /// 센서 탐지시 연락해야할(혹은 조치해야할) 부서의 전화번호
        /// </summary>
        public string DepartmentPhoneNumber
        {
            get { return m_strDepartmentPhoneNumber; }
            set { m_strDepartmentPhoneNumber = value; }
        }

        public bool? Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        public string Status
        {
            get { return m_strStatus; }
            set { m_strStatus = value; }
        }

        public string UniqueKey
        {
            get { return m_strUniqueKey; }
            set { m_strUniqueKey = value; }
        }

        /// <summary>
        ///  종류
        /// </summary>
        public int? MaterialType
        {
            get { return m_nMaterialType; }
            set { m_nMaterialType = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.Name ||
                field == Fields.ZoneID || 
                field == Fields.UniqueKey)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsSensorETC"; }
        }
    }
}
