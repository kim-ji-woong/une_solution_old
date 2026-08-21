using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.History
{
    public class SensorReactionHistoryDescriptionText : IIDObject
    {
        public enum Fields { ID, RefCount, Description };

        private int m_nID = -1;
        // 이 데이터의 참조 개수. 이 값이 0이되면 이 데이터는 삭제되어야 한다.
        private int m_nRefCount = 0;
        private string m_strDescription = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 이 데이터의 참조 개수. 이 값이 0이되면 이 데이터는 삭제되어야 한다.
        public int RefCount
        {
            get { return m_nRefCount; }
            set { m_nRefCount = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SdmsHistorySensorReactionDescriptionText"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
