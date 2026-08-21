using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public class ProcessMission : IComparable
    {
        private int m_nTextID = -1;
        private int m_nMissionTextID = -1;
        private string m_strText = "";
        private string m_strMissionText = "";
        private bool m_bIsChecked = false;
        private int m_nSectionNumber = -1;

        public int TextID
        {
            get { return m_nTextID; }
            set { m_nTextID = value; }
        }

        public int MissionTextID
        {
            get { return m_nMissionTextID; }
            set { m_nMissionTextID = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public string MissionText
        {
            get { return m_strMissionText; }
            set { m_strMissionText = value; }
        }

        public bool IsChecked
        {
            get { return m_bIsChecked; }
            set { m_bIsChecked = value; }
        }

        public int SectionNumber
        {
            get { return m_nSectionNumber; }
            set { m_nSectionNumber = value; }
        }

        public int CompareTo(object obj)
        {
            ProcessMission mission = (ProcessMission)obj;
            return this.m_nSectionNumber.CompareTo(mission.m_nSectionNumber);
        }
    }
}