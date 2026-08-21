using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVAlarmWatcher;

namespace CCTVSingleViewer
{
    public class Alarm
    {
        private bool m_fromCCTV = true;
        private CCTV m_cctv = null;
        private int m_nEquipZoneID = 0;
        private string m_strEquipZoneName = "";
        private AlarmType m_alarmType = AlarmType.Fire;
        private DateTime m_dtTimeStamp = new DateTime();

        public bool FromCCTV
        {
            get { return m_fromCCTV; }
            set { m_fromCCTV = value; }
        }

        public CCTV CCTV
        {
            get { return m_cctv; }
            set { m_cctv = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string EquipZoneName
        {
            get { return m_strEquipZoneName; }
            set { m_strEquipZoneName = value; }
        }

        public AlarmType AlarmType
        {
            get { return m_alarmType; }
            set { m_alarmType = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_dtTimeStamp; }
            set { m_dtTimeStamp = value; }
        }

        public string AlarmText
        {
            get
            {
                string strAlarmType = "";

                if (m_alarmType == CCTVAlarmWatcher.AlarmType.Fire)
                    strAlarmType = "화재";
                else if (m_alarmType == CCTVAlarmWatcher.AlarmType.PSM)
                    strAlarmType = "누출";
                else if (m_alarmType == CCTVAlarmWatcher.AlarmType.Security)
                    strAlarmType = "방범";

                string strAlarmFrom = "";

                if (m_fromCCTV)
                {
                    strAlarmFrom = string.Format("({0})[{1}] CCTV로부터", m_cctv.ID, m_cctv.CameraName);
                }
                else
                {
                    strAlarmFrom = string.Format("{0}에서", m_strEquipZoneName);
                }

                string strAlarm = string.Format("[{0}-{1:00}-{2:00} {3:00}:{4:00}] {5} {6} 알람이 발생하였습니다.", m_dtTimeStamp.Year, m_dtTimeStamp.Month, m_dtTimeStamp.Day, m_dtTimeStamp.Hour, m_dtTimeStamp.Minute, strAlarmFrom, strAlarmType);
                return strAlarm;
            }
        }

        public string AlarmListText
        {
            get
            {
                string strAlarmType = "";

                if (m_alarmType == CCTVAlarmWatcher.AlarmType.Fire)
                    strAlarmType = "화재";
                else if (m_alarmType == CCTVAlarmWatcher.AlarmType.PSM)
                    strAlarmType = "누출";
                else if (m_alarmType == CCTVAlarmWatcher.AlarmType.Security)
                    strAlarmType = "방범";

                string strAlarmFrom = "";

                if (m_fromCCTV)
                {
                    strAlarmFrom = string.Format("({0})[{1}]", m_cctv.ID, m_cctv.CameraName);
                }
                else
                {
                    strAlarmFrom = string.Format("{0}", m_strEquipZoneName);
                }

                string strAlarm = string.Format("[{0}-{1:00}-{2:00} {3:00}:{4:00}] {5}", m_dtTimeStamp.Year, m_dtTimeStamp.Month, m_dtTimeStamp.Day, m_dtTimeStamp.Hour, m_dtTimeStamp.Minute, strAlarmFrom, strAlarmType);
                return strAlarm;
            }
        }

        public bool IsSame(Alarm alarm)
        {
            if (m_alarmType == alarm.AlarmType)
            {
                if (m_fromCCTV == alarm.m_fromCCTV)
                {
                    if (m_fromCCTV)
                    {
                        return m_cctv == alarm.m_cctv;
                    }
                    else
                    {
                        return m_nEquipZoneID == alarm.m_nEquipZoneID && m_strEquipZoneName == alarm.m_strEquipZoneName;
                    }
                }
            }

            return false;
        }
    }
}
