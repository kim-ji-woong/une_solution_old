using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Sensor.Option
{
    public class EtcData
    {
        public enum Fields { SensorType, AlarmDepth, DataMini, DataMinf, DataMins, DataMaxi, DataMaxf, DataMaxs, LinkedBuildingIDs, LinkedZoneIDs, SendSDMS };

        // FacilityType(SensorType)
        private int m_nSensorType = -1;
        private int m_nAlarmDepth = -1;
        // 알람을 발생시키는 최소값(최소 ~ 이상이면 알람)
        private int? m_nDataMin = null;
        private float? m_fDataMin = null;
        private string m_strDataMin = null;
        // 알람을 발생시키는 최대값(최대 ~미만이면 알람)
        private int? m_nDataMax = null;
        private float? m_fDataMax = null;
        private string m_strDataMax = null;
        // 이 값이 null이 아니면 이 BuildingID에 해당하는 곳에만 적용되는 옵션이다. 만일 LinkedBuildingID와 LinkedZoneID가 모두 null이면 모든 곳에 적용되는 옵션이 된다.
        private List<int> m_linkedBuildingIDs = null;
        // 이 값이 null이 아니면 이 ZoneID에 해당하는 곳에만 적용되는 옵션이다. 만일 LinkedBuildingID와 LinkedZoneID가 모두 null이면 모든 곳에 적용되는 옵션이 된다.
        private List<int> m_linkedZoneIDs = null;
        // SDMS에게 알람 신호를 보낼 것인가?
        private bool m_sendSDMS = false;

        /// <summary>
        /// FacilityType(SensorType)
        /// </summary>
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int AlarmDepth
        {
            get { return m_nAlarmDepth; }
            set { m_nAlarmDepth = value; }
        }

        // 알람을 발생시키는 최소값(최소 ~ 이상이면 알람)
        public int? DataMini
        {
            get { return m_nDataMin; }
            set { m_nDataMin = value; }
        }

        // 알람을 발생시키는 최소값(최소 ~ 이상이면 알람)
        public float? DataMinf
        {
            get { return m_fDataMin; }
            set { m_fDataMin = value; }
        }

        // 알람을 발생시키는 최소값(최소 ~ 이상이면 알람)
        public string DataMins
        {
            get { return m_strDataMin; }
            set { m_strDataMin = value; }
        }

        // 알람을 발생시키는 최대값(최대 ~미만이면 알람)
        public int? DataMaxi
        {
            get { return m_nDataMax; }
            set { m_nDataMax = value; }
        }

        // 알람을 발생시키는 최대값(최대 ~미만이면 알람)
        public float? DataMaxf
        {
            get { return m_fDataMax; }
            set { m_fDataMax = value; }
        }

        // 알람을 발생시키는 최대값(최대 ~미만이면 알람)
        public string DataMaxs
        {
            get { return m_strDataMax; }
            set { m_strDataMax = value; }
        }

        // 이 값이 null이 아니면 이 BuildingID에 해당하는 곳에만 적용되는 옵션이다. 만일 LinkedBuildingID와 LinkedZoneID가 모두 null이면 모든 곳에 적용되는 옵션이 된다.
        public List<int> LinkedBuildingIDs
        {
            get { return m_linkedBuildingIDs; }
            set { m_linkedBuildingIDs = value; }
        }

        // 이 값이 null이 아니면 이 ZoneID에 해당하는 곳에만 적용되는 옵션이다. 만일 LinkedBuildingID와 LinkedZoneID가 모두 null이면 모든 곳에 적용되는 옵션이 된다.
        public List<int> LinkedZoneIDs
        {
            get { return m_linkedZoneIDs; }
            set { m_linkedZoneIDs = value; }
        }

        // SDMS에게 알람 신호를 보낼 것인가?
        public bool SendSDMS
        {
            get { return m_sendSDMS; }
            set { m_sendSDMS = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.SensorType ||
                field == Fields.AlarmDepth ||
                field == Fields.SendSDMS)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "OptionEtcSensorData"; }
        }
    }
}
