using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertManager.Data
{
    public class FacilityManual
    {
        private int m_nID = -1;
        private FacilityType m_facilityType = FacilityType.NONE;
        private string m_strRiskLevel = CommonString.RiskLevel_Normal;
        string m_strTitle = "";
        int m_nNumber = -1;
        string m_strManualMembers = "";
        string m_strManual = "";


        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public FacilityType FacilityType
        {
            get { return m_facilityType; }
            set { m_facilityType = value; }
        }

        public string RiskLevel
        {
            get { return m_strRiskLevel; }
            set { m_strRiskLevel = value; }
        }

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        public int Number
        {
            get { return m_nNumber; }
            set { m_nNumber = value; }
        }

        public string Members
        {
            get { return m_strManualMembers; }
            set { m_strManualMembers = value; }
        }

        public string Manual
        {
            get { return m_strManual; }
            set { m_strManual = value; }
        }
    }
}
