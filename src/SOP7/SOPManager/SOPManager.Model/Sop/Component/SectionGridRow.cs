using System;

namespace SOPManager.Model.Sop.Component
{
    public class SectionGridRow : IComparable
    {
        public enum Fields { GridID, RowIndex, Height };

        private int m_nGridID = -1;
        private int m_nRowIndex = -1;
        private int m_nHeight = 0;

        // SectionGrid의 ID
        public int GridID
        {
            get { return m_nGridID; }
            set { m_nGridID = value; }
        }

        public int RowIndex
        {
            get { return m_nRowIndex; }
            set { m_nRowIndex = value; }
        }

        public int Height
        {
            get { return m_nHeight; }
            set { m_nHeight = value; }
        }

        public static string TableName
        {
            get { return "SopComponentSectionGridRow"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public int CompareTo(object obj)
        {
            SectionGridRow row = (SectionGridRow)obj;

            if (this.m_nRowIndex > row.m_nRowIndex)
                return 1;
            else if (this.m_nRowIndex < row.m_nRowIndex)
                return -1;

            return 0;
        }
    }
}
