using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public class PSMAlarm
    {
        private string m_strTankName = "";
        private string m_strAlarmData = "";
        private string m_strInjuryLocation = "";
        private string m_strInjuryData = "";
        private bool m_isRealMode = false;

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

        private string m_strDeadCount = "";
        private string m_strInjuryCount = "";
        private string m_strLostCount = "";
        private string m_strTankTemp= "";

        public string DeadCount
        {
            get { return m_strDeadCount; }
            set { m_strDeadCount = value; }
        }

        public string InjuryCount
        {
            get { return m_strInjuryCount; }
            set { m_strInjuryCount = value; }
        }
        public string LostCount
        {
            get { return m_strLostCount; }
            set { m_strLostCount = value; }
        }
        public string TankTemp
        {
            get { return m_strTankTemp; }
            set { m_strTankTemp = value; }
        }
    }
}