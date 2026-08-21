using System;

namespace SOPManager.Model.Sop.Config
{
    public class ExternalProgramParameter : IComparable
    {
        public enum Fields { ProgramID, ParameterIndex, ParameterName, ValueType, IsNullable };
        public enum ParameterValueType { NONE = 0, INTEGER = 20, FLOAT = 21, DOUBLE = 22, STRING = 23, LONG = 24, BOOLEAN = 25, SHORT = 26, BYTE = 27 };

        // ExternalProgram의 ID
        private int m_nProgramID = -1;
        // 0부터 시작한다.
        private int m_nParameterIndex = -1;
        private string m_strParameterName = "";
        private int m_nValueType = (int)ParameterValueType.NONE;
        // 생략가능한가?
        private bool m_isNullable = false;

        // ExternalProgram의 ID
        public int ProgramID
        {
            get { return m_nProgramID; }
            set { m_nProgramID = value; }
        }

        // 0부터 시작한다.
        public int ParameterIndex
        {
            get { return m_nParameterIndex; }
            set { m_nParameterIndex = value; }
        }

        public string ParameterName
        {
            get { return m_strParameterName; }
            set { m_strParameterName = value; }
        }

        // ParameterValueType
        public int ValueType
        {
            get { return m_nValueType; }
            set { m_nValueType = value; }
        }

        // 생략가능한가?
        public bool IsNullable
        {
            get { return m_isNullable; }
            set { m_isNullable = value; }
        }

        public static string TableName
        {
            get { return "SopConfigExternalProgramParameter"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public int CompareTo(object obj)
        {
            ExternalProgramParameter param1 = (ExternalProgramParameter)this;
            ExternalProgramParameter param2 = (ExternalProgramParameter)obj;

            if (param2 == null)
                return -1;

            if (param1.ParameterIndex < param2.ParameterIndex)
                return -1;
            else if (param1.ParameterIndex == param2.ParameterIndex)
                return 0;

            return 1;
        }
    }
}
