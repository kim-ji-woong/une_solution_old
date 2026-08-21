namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorPSM 누출센서(또는 유해화학물질 센서)
    /// </summary>
    public class PSM : IIDObject
    {
        public enum Fields { ID, Name, PositionName, X, Y, Z, ZoneID, CurrentData, LimitLevel1, LimitLevel2, LimitLevel3, EquipZoneID, UseLimitLevel1, UseLimitLevel2, UseLimitLevel3, Department, DepartmentPhoneNumber, Enabled, Status, UniqueKey, MaterialType };

        private int m_nID = -1;
        // 센서 이름
        private string m_strName = "";
        // 센서 설치 위치
        private string m_strPositionName = null;
        private float? m_x = null;
        private float? m_y = null;
        private float? m_z = null;
        private int m_nZoneID = -1;
        // 센서의 현재값
        private float? m_fCurrentData = null;
        // 1단계 알람의 임계치
        private float? m_fLimitLevel1 = null;
        // 2단계 알람의 임계치
        private float? m_fLimitLevel2 = null;
        // 3단계 알람의 임계치
        private float? m_fLimitLevel3 = null;
        // 1단계 알람 임계치를 사용할 것인가?
        private bool m_useLImitLevel1 = false;
        // 2단계 알람 임계치를 사용할 것인가?
        private bool m_useLImitLevel2 = false;
        // 3단계 알람 임계치를 사용할 것인가?
        private bool m_useLImitLevel3 = false;
        private int m_nEquipZoneID = -1;
        private string m_strDepartment = null;
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

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        /// <summary>
        /// 센서의 현재값
        /// </summary>
        public float? CurrentData
        {
            get { return m_fCurrentData; }
            set { m_fCurrentData = value; }
        }

        /// <summary>
        /// 1단계 알람의 임계치
        /// </summary>
        public float? LimitLevel1
        {
            get { return m_fLimitLevel1; }
            set { m_fLimitLevel1 = value; }
        }

        /// <summary>
        /// 2단계 알람의 임계치
        /// </summary>
        public float? LimitLevel2
        {
            get { return m_fLimitLevel2; }
            set { m_fLimitLevel2 = value; }
        }

        /// <summary>
        /// 3단계 알람의 임계치
        /// </summary>
        public float? LimitLevel3
        {
            get { return m_fLimitLevel3; }
            set { m_fLimitLevel3 = value; }
        }

        /// <summary>
        /// 1단계 알람 임계치를 사용할 것인가?
        /// </summary>
        public bool UseLimitLevel1
        {
            get { return m_useLImitLevel1; }
            set { m_useLImitLevel1 = value; }
        }

        /// <summary>
        /// 2단계 알람 임계치를 사용할 것인가?
        /// </summary>
        public bool UseLimitLevel2
        {
            get { return m_useLImitLevel2; }
            set { m_useLImitLevel2 = value; }
        }

        /// <summary>
        /// 3단계 알람 임계치를 사용할 것인가?
        /// </summary>
        public bool UseLimitLevel3
        {
            get { return m_useLImitLevel3; }
            set { m_useLImitLevel3 = value; }
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
        /// 유해 화학물질 종류
        /// </summary>
        public int? MaterialType
        {
            get { return m_nMaterialType; }
            set { m_nMaterialType = value; }
        }

        public static string TableName
        {
            get { return "SdmsSensorPSM"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.Name ||
                field == Fields.ZoneID ||
                field == Fields.EquipZoneID ||
                field == Fields.UseLimitLevel1 ||
                field == Fields.UseLimitLevel2 ||
                field == Fields.UseLimitLevel3 ||
                field == Fields.UniqueKey)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
