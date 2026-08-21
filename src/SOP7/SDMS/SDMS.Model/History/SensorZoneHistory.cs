using System;
using System.Collections.Generic;

namespace SDMS.Model.History
{
    /// <summary>
    /// 센서의 탐지이력을 나타낸다.
    /// 센서의 모든 탐지신호를 의미하는 것은 아니고, 알람을 발생시킨 센서값들만 나타낸다.
    /// 즉, 하나의 SensorZoneHistory는 하나의 알람을 가르킨다.
    /// </summary>
    public class SensorZoneHistory : IIDObject
    {
        public enum Fields { ID, SensorZoneID, Data, Time, ZoneID, SensorType, DetectionStatus, SiteID, AllSensorZoneIDs, Memo };

        // 실제, 오동작, 테스트
        public enum DetectionType { None = 0, Real, Malfunction, Test}

        private int m_nID = -1;
        private int m_nSensorZoneID = -1;
        // 알람발생시 센서값
        private string m_strData = null;
        // 알람발생시간
        private DateTime m_time = new DateTime();
        // 센서가 위치한 Zone의 ID
        private int m_nZoneID = -1;
        // FacilityType
        private int m_nSensorType = -1;
        // 탐지된 센서신호가 실제 재난이었는가를 나타낸다.
        private DetectionType m_detectionStatus = DetectionType.None;
        private int m_nSiteID = -1;
        // 하나의 알람에 여러 센서들이 연관되어 있을수 있다. 알람이 발생한 이후에 추가적으로 다른 센서도 동작할수 있기 때문에 전체 센서들의 ID를 담는다.
        private List<int> m_allSensorZoneIDs = null;
        private string m_strMemo = null;

        private static Dictionary<int, DetectionType> m_dicDetectionType = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        /// <summary>
        /// 알람발생시 센서값
        /// </summary>
        public string Data
        {
            get { return m_strData; }
            set { m_strData = value; }
        }

        /// <summary>
        /// 알람발생시간
        /// </summary>
        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        /// <summary>
        /// 센서가 위치한 Zone의 ID
        /// </summary>
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        /// <summary>
        /// FacilityType
        /// </summary>
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        /// <summary>
        /// 탐지된 센서신호가 실제 재난이었는가를 나타낸다.
        /// </summary>
        public DetectionType DetectionStatus
        {
            get { return m_detectionStatus; }
            set { m_detectionStatus = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        // 하나의 알람에 여러 센서들이 연관되어 있을수 있다. 알람이 발생한 이후에 추가적으로 다른 센서도 동작할수 있기 때문에 전체 센서들의 ID를 담는다.
        public List<int> AllSensorZoneIDs
        {
            get { return m_allSensorZoneIDs; }
            set { m_allSensorZoneIDs = value; }
        }

        public string Memo
        {
            get { return m_strMemo; }
            set { m_strMemo = value; }
        }

        public static string TableName
        {
            get { return "SdmsHistorySensorZone"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Data ||
                field == Fields.DetectionStatus ||
                field == Fields.AllSensorZoneIDs ||
                field == Fields.Memo)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static DetectionType ToDetectionType(int nType)
        {
            if (m_dicDetectionType == null)
            {
                m_dicDetectionType = new Dictionary<int, DetectionType>();

                foreach (DetectionType type in Enum.GetValues(typeof(DetectionType)))
                {
                    m_dicDetectionType[(int)type] = type;
                }
            }

            DetectionType rType;
            if (m_dicDetectionType.TryGetValue(nType, out rType))
                return rType;

            return DetectionType.None;
        }
    }
}
