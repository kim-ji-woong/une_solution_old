using dnsData.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.BLL.Models.Alarm
{
    public class AlarmData : ICloneable
    {
        private DateTime m_dtTime = new DateTime();
        public DateTime dtTime
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        private string m_strDateTime = "";
        public string StrDateTime
        {
            get { return m_strDateTime; }
            set { m_strDateTime = value; }
        }

        private int? m_nOrgSensorID = -1;
        public int? OrgSensorID
        {
            get { return m_nOrgSensorID; }
            set { m_nOrgSensorID = value; }
        }

        private int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        private string m_strSensorName = "";
        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        private string m_strPositionName = "";
        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }

        private string m_strBuildingName = "";
        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        private string m_strZoneName = "";
        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        private int m_nZoneID = -1;
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        private int m_nEquipZoneID = -1;
        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        private Facility.FacilityType m_facilityType = Facility.FacilityType.NONE;
        public Facility.FacilityType FacilityType
        {
            get { return m_facilityType; }
            set { m_facilityType = value; }
        }

        private string m_strFacilityTypeString = "";
        public string FacilityTypeString
        {
            get { return m_strFacilityTypeString; }
            set { m_strFacilityTypeString = value; }
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        private int m_nSopStatus = -1;
        public int SopStatus
        {
            get { return m_nSopStatus; }
            set { m_nSopStatus = value; }
        }

        private int m_AlarmDepth = -1;
        public int AlarmDepth
        {
            get { return m_AlarmDepth; }
            set { m_AlarmDepth = value; }
        }

        private List<int> m_AlarmSensorZoneIDs = new List<int>();
        public List<int> AlarmSensorZoneIDs
        {
            get { return m_AlarmSensorZoneIDs; }
            set { m_AlarmSensorZoneIDs = value; }
        }

        private string m_strReleaseInfo = "";
        public string ReleaseInfo
        {
            get { return m_strReleaseInfo; }
            set { m_strReleaseInfo = value; }
        }

        private bool m_bIsAlarm = true;
        public bool IsAlarm
        {
            get { return m_bIsAlarm; }
            set { m_bIsAlarm = value; }
        }

        private string m_strReportPerson = "";
        public string ReportPerson
        {
            get { return m_strReportPerson; }
            set { m_strReportPerson = value; }
        }

        private string m_strMemo = "";
        public string Memo
        {
            get { return m_strMemo; }
            set { m_strMemo = value; }
        }

        private int? m_nMaterialType = null;
        public int? MaterialType 
        {
            get { return m_nMaterialType; }
            set { m_nMaterialType = value; }
        }

        private string m_strMaterialTypeString = "";
        public string MaterialTypeString
        {
            get { return m_strMaterialTypeString; }
            set { m_strMaterialTypeString = value; }
        }

        public object Clone()
        {
            AlarmData data = new AlarmData();
            Copy(this, data);
            /*data.m_dtTime = m_dtTime;
            data.m_strDateTime = m_strDateTime;
            data.m_nOrgSensorID = m_nOrgSensorID;
            data.m_nSensorZoneID = m_nSensorZoneID;
            data.m_nSensorZoneHistoryID = m_nSensorZoneHistoryID;
            data.m_strSensorName = m_strSensorName;
            data.m_strPositionName = m_strPositionName;
            data.m_strBuildingName = m_strBuildingName;
            data.m_strZoneName = m_strZoneName;
            data.m_nZoneID = m_nZoneID;
            data.m_nEquipZoneID = m_nEquipZoneID;
            data.m_facilityType = m_facilityType;
            data.m_strFacilityTypeString = m_strFacilityTypeString;
            data.m_strMessage = m_strMessage;
            data.m_nSopStatus = m_nSopStatus;
            data.m_AlarmDepth = m_AlarmDepth;
            data.m_AlarmSensorZoneIDs = m_AlarmSensorZoneIDs;
            data.m_strReleaseInfo = m_strReleaseInfo;
            data.m_bIsAlarm = m_bIsAlarm;
            data.m_strReportPerson = m_strReportPerson;
            data.m_strMemo = m_strMemo;*/

            return data;
        }

        public static void Copy(AlarmData src, AlarmData trg)
        {
            trg.m_dtTime = src.m_dtTime;
            trg.m_strDateTime = src.m_strDateTime;
            trg.m_nOrgSensorID = src.m_nOrgSensorID;
            trg.m_nSensorZoneID = src.m_nSensorZoneID;
            trg.m_nSensorZoneHistoryID = src.m_nSensorZoneHistoryID;
            trg.m_strSensorName = src.m_strSensorName;
            trg.m_strPositionName = src.m_strPositionName;
            trg.m_strBuildingName = src.m_strBuildingName;
            trg.m_strZoneName = src.m_strZoneName;
            trg.m_nZoneID = src.m_nZoneID;
            trg.m_nEquipZoneID = src.m_nEquipZoneID;
            trg.m_facilityType = src.m_facilityType;
            trg.m_strFacilityTypeString = src.m_strFacilityTypeString;
            trg.m_strMessage = src.m_strMessage;
            trg.m_nSopStatus = src.m_nSopStatus;
            trg.m_AlarmDepth = src.m_AlarmDepth;
            trg.m_AlarmSensorZoneIDs = src.m_AlarmSensorZoneIDs;
            trg.m_strReleaseInfo = src.m_strReleaseInfo;
            trg.m_bIsAlarm = src.m_bIsAlarm;
            trg.m_strReportPerson = src.m_strReportPerson;
            trg.m_strMemo = src.m_strMemo;
            trg.m_nMaterialType = src.m_nMaterialType;
            trg.m_strMaterialTypeString = src.m_strMaterialTypeString;
        }
    }
}
