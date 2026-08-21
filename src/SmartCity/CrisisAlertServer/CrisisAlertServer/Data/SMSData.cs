using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertServer.Data
{
    public class SMSData
    {
        int m_nID = -1;
        List<string> m_listNumber;
        string m_strMessage = "";
        FacilityType m_nFacilityType = FacilityType.NONE;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public List<string> NumberList
        {
            get { return m_listNumber; }
            set { m_listNumber = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public FacilityType FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }
    }
}
