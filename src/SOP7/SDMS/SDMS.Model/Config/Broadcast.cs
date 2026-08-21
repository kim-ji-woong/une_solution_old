using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Config
{
    public class Broadcast : IIDObject
    {
        public enum Fields { ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, Description, SiteID };

        public enum SituationTypes
        {
            Unknown = -1,
            DETECT_FIRE = 0,        // 화재 탐지
            REPORT_FIRE = 1,        // 화재 신고
            DETECT_PSM = 2,         // 누출 탐지
            REPORT_PSM = 3,         // 누출 신고
            DETECT_EARTHQUAKE = 4,  // 지진 탐지
            DETECT_SECURITY = 5,
            REPORT_SECURITY = 6,
            DETECT_TH = 7,
            REPORT_TH = 8,
            DETECT_ETC = 9,
            REPORT_ETC = 10
        }

        private int m_nID = -1;
        // SituationTypes
        private int m_nSituationType = (int)SituationTypes.Unknown;
        private bool m_useBroadcast = false;
        private string m_strMessage = "";
        private bool m_useSiren = false;
        // 반복횟수가 1이면 한번만 방송한다.
        private int m_nRepeatCount = 1;
        private string m_strDescription = null;
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SituationType
        {
            get { return m_nSituationType; }
            set { m_nSituationType = value; }
        }

        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        // PlayType
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public bool UseSiren
        {
            get { return m_useSiren; }
            set { m_useSiren = value; }
        }

        // 반복횟수가 1이면 한번만 방송한다.
        public int RepeatCount
        {
            get { return m_nRepeatCount; }
            set { m_nRepeatCount = value; }
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
            get { return "SdmsConfigBroadcast"; }
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
