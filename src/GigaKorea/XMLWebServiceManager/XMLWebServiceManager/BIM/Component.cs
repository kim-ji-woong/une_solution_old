using System;
using System.Collections.Generic;
using System.Text;

namespace XMLWebServiceManager.BIM
{
    public class Component
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strTypeName = "";
        private string m_strComponentName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string XMLID
        {
            get { return m_strXMLID; }
            set { m_strXMLID = value; }
        }

        public string TypeName
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        public string ComponentName
        {
            get { return m_strComponentName; }
            set { m_strComponentName = value; }
        }

        public string WebServiceCode
        {
            get
            {
                if (m_strTypeName == "Structure")
                    return "WTP_01";
                else if (m_strTypeName == "Fake")
                    return "WTP_02";
                else if (m_strTypeName == "Partition")
                    return "WTP_03";
                else if (m_strTypeName == "Handrail")
                    return "WTP_04";
                else if (m_strTypeName == "CurtainWall")
                    return "WTP_05";

                return "WTP_03";
            }
        }

        public static string GetComponentNameFromCode(string strCode)
        {
            if (strCode == "WTP_01")
                return "콘크리트";
            else if (strCode == "WTP_02")
                return "가벽";
            else if (strCode == "WTP_03")
                return "파티션";
            else if (strCode == "WTP_04")
                return "철재";
            else if (strCode == "WTP_05")
                return "유리벽";

            return "";
        }
    }
}

