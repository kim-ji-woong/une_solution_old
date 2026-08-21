using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.History
{
    public class SMSHistory : IIDObject
    {
        public enum Fields { ID, SensorZoneHistoryID, SensorReactionHistoryID, RegularMemberIDList, SMSMessage, SendType };

        private int m_nID = -1;
        private int m_nSensorZoneHistoryID = -1;
        private int m_nSensorReactionHistoryID = -1;
        private List<int> m_regularMembers = null;
        private string m_strMessage = null;
        // true이면 자동발송, false이면 수동발송
        private bool m_sendType = true;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public int SensorReactionHistoryID
        {
            get { return m_nSensorReactionHistoryID; }
            set { m_nSensorReactionHistoryID = value; }
        }

        public List<int> RegularMemberIDList
        {
            get { return m_regularMembers; }
            set { m_regularMembers = value; }
        }

        public string SMSMessage
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        // true이면 자동발송, false이면 수동발송
        public bool SendType
        {
            get { return m_sendType; }
            set { m_sendType = value; }
        }
        public static string TableName
        {
            get { return "SdmsHistorySMS"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.RegularMemberIDList)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
