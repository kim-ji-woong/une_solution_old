using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public class FirePSMAlarm
    {
        private string m_strLocation = "";
        private string m_strDisasterInfo = "";
        private string m_strTankName = "";
        private string m_strAlarmData = "";
        private string m_strInjuryLocation = "";
        private string m_strInjuryData = "";
        private bool m_isRealMode = true;

        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }

        public string DisasterInfo
        {
            get { return m_strDisasterInfo; }
            set { m_strDisasterInfo = value; }
        }

        public string TankName
        {
            get { return m_strTankName; }
            set { m_strTankName = value; }
        }

        public string TankAlarmData
        {
            get { return m_strAlarmData; }
            set { m_strAlarmData = value; }
        }

        public string InjuryLocation
        {
            get { return m_strInjuryLocation; }
            set { m_strInjuryLocation = value; }
        }

        public string InjuryData
        {
            get { return m_strInjuryData; }
            set { m_strInjuryData = value; }
        }

        public bool RealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }
    }
}