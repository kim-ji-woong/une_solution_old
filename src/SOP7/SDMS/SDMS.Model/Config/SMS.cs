using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Config
{
    public class SMS : IIDObject
    {
        public enum Fields { ID, MessageType, UseSMS, Description, SiteID };

        public enum SMSMessageTypes
        {
            UNKNOWN = -1,
            RESET_FIRE = 0,     // 화재복구(0)
            DETECT_FIRE,        // 화재탐지(1)
            REPORT_FIRE,        // 화재신고(2)
            DETECT_PSM,         // 누출탐지(3)
            REPORT_PSM,         // 누출신고(4)
            RESET_PSM,          // 누출복구(5)
            DETECT_SECURITY,    // 방범탐지(6)
            REPORT_SECURITY,    // 방범신고(7)
            RESET_SECURITY,     // 방범복구(8)
            DETECT_EARTHQUAKE,  // 지진탐지(9)
            DETECT_TH,          // 온도/습도 탐지(10)
            RESET_TH,           // 온도/습도 복구(11)
            RESET_ETC,          // ETC 복구
            DETECT_ETC,         // ETC 탐지
            REPORT_ETC          // ETC 신고
        }

        private int m_nID = -1;
        // SMSMessageTypes
        private int m_nMessageType = (int)SMSMessageTypes.UNKNOWN;
        private bool m_useSMS = false;
        private string m_strDescription = null;
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MessageType
        {
            get { return m_nMessageType; }
            set { m_nMessageType = value; }
        }

        public bool UseSMS
        {
            get { return m_useSMS; }
            set { m_useSMS = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SdmsConfigSMS"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
