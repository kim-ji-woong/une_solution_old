using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Config
{
    public class SpreadMessage : IIDObject
    {
        public enum Fields { ID, FacilityType, BuilidingGroupID, BuilidingID, RegularID, RegularMemberID, MessageType, Message };

        public enum MessageTypes
        {
            UNKNOWN = -1,
            SMS = 0,     // 문자
            EMAIL,        // 이메일
        }

        private int m_nID = -1;
        private int m_nFacilityType = -1;
        private int? m_nBuildingGroupID = null;
        private int? m_nBuildingID = null;
        private string m_strRegularID = null;
        private string m_strRegularMemberID = null;
        private int m_nMessageType = (int)MessageTypes.UNKNOWN;
        private string m_strMessage = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }

        public int? BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public int? BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public string RegularID
        {
            get { return m_strRegularID; }
            set { m_strRegularID = value; }
        }

        public string RegularMemberID
        {
            get { return m_strRegularMemberID; }
            set { m_strRegularMemberID = value; }
        }

        public int MessageType
        {
            get { return m_nMessageType; }
            set { m_nMessageType = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public static string TableName
        {
            get { return "SdmsConfigSpreadMessage"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.BuilidingGroupID ||
                field == Fields.BuilidingID ||
                field == Fields.RegularID ||
                field == Fields.RegularMemberID ||
                field == Fields.Message )
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
