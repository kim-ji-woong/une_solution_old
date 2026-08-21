using System;

namespace SDMS.Model.Facility
{
    public class InfoData : IComparable
    {
        public enum Fields { FacilityInfoID, OrderIndex, Value, WithDot, IndentDepth };

        private int m_nFacilityInfoID = -1;
        private int m_nOrderIndex = -1;
        private string m_strValue = "";
        private bool m_withDot = true;
        private int? m_indentDepth = null;

        public int FacilityInfoID
        {
            get { return m_nFacilityInfoID; }
            set { m_nFacilityInfoID = value; }
        }

        public int OrderIndex
        {
            get { return m_nOrderIndex; }
            set { m_nOrderIndex = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public bool WithDot
        {
            get { return m_withDot; }
            set { m_withDot = value; }
        }

        public int? IndentDepth
        {
            get { return m_indentDepth; }
            set { m_indentDepth = value; }
        }

        public static string TableName
        {
            get { return "SdmsFacilityInfoData"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.IndentDepth)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public int CompareTo(object obj)
        {
            InfoData data1 = this;
            InfoData data2 = (InfoData)obj;

            if (data1.FacilityInfoID < data1.FacilityInfoID)
                return -1;
            else if (data1.FacilityInfoID > data1.FacilityInfoID)
                return 1;
            else
            {
                if (data1.OrderIndex < data2.OrderIndex)
                    return -1;
                else if (data1.OrderIndex > data2.OrderIndex)
                    return 1;
            }

            return 0;
        }
    }
}
