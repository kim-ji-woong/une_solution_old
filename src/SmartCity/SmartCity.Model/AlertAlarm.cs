using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class AlertAlarm
    {
        public enum Fields { ID, FacilityType, SensorID, RiskLevel, Address, IsCheck, CreateTime };

        private int m_nID = -1;
        private int m_nFacilityType = -1;
        private int m_nSensorID = -1;
        private string m_strRiskLevel = "";
        private string m_strAddress = "";
        private int m_nIsCheck = 0;
        private DateTime m_dtCreateTime = new DateTime();


        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }

        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        public string RiskLevel
        {
            get { return m_strRiskLevel; }
            set { m_strRiskLevel = value; }
        }

        public string Address
        {
            get { return m_strAddress; }
            set { m_strAddress = value; }
        }

        public int IsCheck
        {
            get { return m_nIsCheck; }
            set { m_nIsCheck = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreateTime; }
            set { m_dtCreateTime = value; }
        }


        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "AlertAlarm"; }
        }
    }
}
