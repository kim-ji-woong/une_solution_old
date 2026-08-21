using System;

namespace SDMS.Model.Spatial
{
    public class BuildingData : IComparable
    {
        public enum Fields { BuildingID, OrderIndex, Value, WithDot, IndentDepth };

        private int m_nBuildingID = -1;
        private int m_nOrderIndex = -1;
        private string m_strValue = "";
        private bool m_withDot = true;
        private int? m_indentDepth = null;

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
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
            get { return "SdmsSpatialBuildingData"; }
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
            BuildingData data1 = this;
            BuildingData data2 = (BuildingData)obj;

            if (data1.BuildingID < data1.BuildingID)
                return -1;
            else if (data1.BuildingID > data1.BuildingID)
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
