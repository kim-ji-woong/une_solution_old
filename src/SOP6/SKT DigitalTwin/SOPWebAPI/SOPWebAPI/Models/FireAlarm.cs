using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SOPWebAPI.Models
{
    public class FireAlarm
    {
        private string m_strEquipCode = "";
        private string m_strEquipStatus = "";
        private string m_eventID = "";
        private DateTime m_eventTime = new DateTime();
        private string m_eventType = "";
        private Zone m_zone = null;
        private bool m_isAlarmOn = true;
        private int m_nSensorZoneHistoryID = -1;
        private int m_nSensorTagID = -1;
        private int m_nSensorZoneID = -1;
        private int m_nWebHistoryID = -1;
        private string m_strSiteID = "";

        public string EquipCode
        {
            get { return m_strEquipCode; }
            set { SetEquipCode(value); }
        }

        public string EquipStatus
        {
            get { return m_strEquipStatus; }
            set { m_strEquipStatus = value; }
        }

        public string EventID
        {
            get { return m_eventID; }
            set { m_eventID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_eventTime; }
            set { m_eventTime = value; }
        }

        public string EventType
        {
            get { return m_eventType; }
            set { m_eventType = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public bool IsAlarmOn
        {
            get { return m_isAlarmOn; }
            set { m_isAlarmOn = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public int SensorTagID
        {
            get { return m_nSensorTagID; }
            set { m_nSensorTagID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int WebHistoryID
        {
            get { return m_nWebHistoryID; }
            set { m_nWebHistoryID = value; }
        }

        public string SiteID
        {
            get { return m_strSiteID; }
        }

        public string SensorZoneHistoryIDString
        {
            get { return MakeSensorZoneHistoryIDString(m_strSiteID, m_nSensorZoneHistoryID); }
        }

        private void SetEquipCode(string strEquipCode)
        {
            m_strEquipCode = strEquipCode;

            if (m_strEquipCode != null)
            {
                string strSiteID = DataManager.Instance.GetSiteID(m_strEquipCode);

                if (strSiteID == null)
                    m_strSiteID = "";
                else
                    m_strSiteID = strSiteID;
            }
            else
                m_strSiteID = "";
        }

        public static string MakeSensorZoneHistoryIDString(string strSiteID, int nSensorZoneHistoryID)
        {
            if (nSensorZoneHistoryID > 0 && strSiteID.Length > 0)
                return strSiteID + "_" + nSensorZoneHistoryID.ToString();

            return "";
        }

        public void SetSensorZoneHistoryID(string strSensorZoneHistoryID)
        {
            int nIndex = strSensorZoneHistoryID.IndexOf('_');

            if (nIndex < 0)
                return;

            string strSiteID = strSensorZoneHistoryID.Substring(0, nIndex).Trim();
            string strHistoryID = strSensorZoneHistoryID.Substring(nIndex + 1).Trim();

            if (m_strSiteID == strSiteID)
            {
                int nHistoryID;

                if (int.TryParse(strHistoryID, out nHistoryID))
                    m_nSensorZoneHistoryID = nHistoryID;
            }
        }
    }

    public class FireParams
    {
        /// <summary>
        /// 센서장비의 고유 코드
        /// </summary>
        public string dvcCd = "";

        /// <summary>
        /// 3 : 화재신호 탐지
        /// 0 : 화재신호 꺼짐
        /// </summary>
        public string dvcStatus = "";

        /// <summary>
        /// 이벤트 고유 ID
        /// </summary>
        public string evtId = "";

        /// <summary>
        /// 이벤트 발생 시각
        /// </summary>
        public string evtTime = "";

        /// <summary>
        /// 이벤트 발생 타입
        /// </summary>
        public string evtType = "";

        /// <summary>
        /// 도면 코드
        /// </summary>
        public string mapCd = "";

        /// <summary>
        /// 층정보. 지하면 B로 시작, 지상이면 F로 시작.(B1, B2, F2, F3...)
        /// </summary>
        public string floorId = "";
    }

    public class CheckParams
    {
        /// <summary>
        /// 이벤트 고유 ID
        /// </summary>
        [Required]
        public string evtId = "";

        /// <summary>
        /// 실제 알람인지 여부
        /// 1이면 실제재난 발생
        /// 0이면 센서 오작동
        /// </summary>
        public int isReal = 0;

        /// <summary>
        /// 확인된 알람에 대한 설명
        /// </summary>
        public string description = "";
    }
}
