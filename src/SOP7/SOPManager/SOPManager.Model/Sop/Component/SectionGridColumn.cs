using System;

namespace SOPManager.Model.Sop.Component
{
    public class SectionGridColumn : IComparable
    {
        public enum Fields { GridID, ColumnIndex, Width };

        private int m_nGridID = -1;
        private int m_nColumnIndex = -1;
        private int m_nWidth = 0;

        // SectionGrid의 ID
        public int GridID
        {
            get { return m_nGridID; }
            set { m_nGridID = value; }
        }

        public int ColumnIndex
        {
            get { return m_nColumnIndex; }
            set { m_nColumnIndex = value; }
        }

        public int Width
        {
            get { return m_nWidth; }
            set { m_nWidth = value; }
        }

        public static string TableName
        {
            get { return "SopComponentSectionGridColumn"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public int CompareTo(object obj)
        {
            SectionGridColumn column = (SectionGridColumn)obj;

            if (this.m_nColumnIndex > column.m_nColumnIndex)
                return 1;
            else if (this.m_nColumnIndex < column.m_nColumnIndex)
                return -1;

            return 0;
        }
    }
}
