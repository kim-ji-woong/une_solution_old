using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.History
{
    public class SensorReactionHistoryDescription : IIDObject
    {
        public enum Fields { ID, SensorReactionHistoryID, DescriptionID, SensorZoneHistoryID };

        private int m_nID = -1;
        private int m_nSensorReactionHistoryID = 0;
        // SensorReactionHistoryDescriptionText의 ID
        private int m_nDescriptionID = -1;
        private int? m_nSensorZoneHistoryID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 이 데이터의 참조 개수. 이 값이 0이되면 이 데이터는 삭제되어야 한다.
        public int SensorReactionHistoryID
        {
            get { return m_nSensorReactionHistoryID; }
            set { m_nSensorReactionHistoryID = value; }
        }

        public int DescriptionID
        {
            get { return m_nDescriptionID; }
            set { m_nDescriptionID = value; }
        }

        public int? SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public static string TableName
        {
            get { return "SdmsHistorySensorReactionDescription"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.SensorZoneHistoryID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
