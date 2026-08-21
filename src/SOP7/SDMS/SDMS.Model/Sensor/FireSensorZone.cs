using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorFire + SdmsSensorZone
    /// </summary>
    public class FireSensorZone : IIDObject
    {
        public enum Fields { ID, Name, PositionName, X, Y, Z, ZoneID, Department, DepartmentPhoneNumber, SensorZoneID, SensorType, OrgSensorID, EquipZoneID, IsAlarmStatus };

        private int m_nID = -1;
        // 센서 이름
        private string m_strName = "";
        // 센서 설치 위치
        private string m_strPositionName = null;
        private float? m_x = null;
        private float? m_y = null;
        private float? m_z = null;
        private int m_nZoneID = -1;
        // 센서 탐지시 연락해야할(혹은 조치해야할) 부서
        private string m_strDepartment = null;
        // 센서 탐지시 연락해야할(혹은 조치해야할) 부서의 전화번호
        private string m_strDepartmentPhoneNumber = null;

        private int m_nSensorZoneID = -1;
        // FacilityType(SensorType)
        private int m_nSensorType = -1;
        // Original Sensor ID
        private int m_nOrgSensorID = -1;
        // EquipmentZone ID
        private int m_nEquipZoneID = -1;
        // 현재 알람 상태인가?
        private bool m_isAlarmStatus = false;

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

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        /// <summary>
        /// FacilityType(SensorType)
        /// </summary>
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        /// <summary>
        /// Original Sensor ID
        /// </summary>
        public int OrgSensorID
        {
            get { return m_nOrgSensorID; }
            set { m_nOrgSensorID = value; }
        }

        /// <summary>
        /// EquipmentZone ID
        /// </summary>
        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        // 현재 알람 상태인가?
        public bool IsAlarmStatus
        {
            get { return m_isAlarmStatus; }
            set { m_isAlarmStatus = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.Name ||
                field == Fields.ZoneID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsSensorFire"; }
        }
    }
}
