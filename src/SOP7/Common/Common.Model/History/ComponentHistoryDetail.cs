using System;

namespace Common.Model.History
{
    public class ComponentHistoryDetail
    {
        public enum Fields { ID, ComponentHistoryID, DataIndex, Datai, Dataf, Datas, Time };

        private int m_nID = -1;
        private int m_nComponentHistoryID = -1;
        private int m_nDataIndex = -1;
        private int? m_nData = null;
        private float? m_fData = null;
        private string m_strData = null;
        private DateTime? m_dtTime = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public int DataIndex
        {
            get { return m_nDataIndex; }
            set { m_nDataIndex = value; }
        }

        public int? Datai
        {
            get { return m_nData; }
            set { m_nData = value; }
        }

        public float? Dataf
        {
            get { return m_fData; }
            set { m_fData = value; }
        }

        public string Datas
        {
            get { return m_strData; }
            set { m_strData = value; }
        }

        public DateTime? Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        public static string TableName
        {
            get { return "SopHistoryComponentDetail"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.ComponentHistoryID ||
                field == Fields.DataIndex)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
