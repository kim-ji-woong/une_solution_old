using System;

namespace SOPManager.Model.Sop.Component
{
    public class ProcessExternalMission : IComparable
    {
        public enum Fields { ProcessID, OrderIndex, ProgramID, ParameterIndex, Value };

        private int m_nProcessID = -1;
        private int m_nOrderIndex = -1;
        private int m_nProgramID = -1;
        private int m_nParameterIndex = -1;
        private string m_strValue = null;

        public int ProcessID
        {
            get { return m_nProcessID; }
            set { m_nProcessID = value; }
        }

        public int OrderIndex
        {
            get { return m_nOrderIndex; }
            set { m_nOrderIndex = value; }
        }

        public int ProgramID
        {
            get { return m_nProgramID; }
            set { m_nProgramID = value; }
        }

        public int ParameterIndex
        {
            get { return m_nParameterIndex; }
            set { m_nParameterIndex = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public static string TableName
        {
            get { return "SopComponentProcessExternalMission"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Value)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public int CompareTo(object obj)
        {
            ProcessExternalMission param1 = (ProcessExternalMission)this;
            ProcessExternalMission param2 = (ProcessExternalMission)obj;

            if (param2 == null)
                return -1;

            if (param1.OrderIndex < param2.OrderIndex)
                return -1;
            else if (param1.ParameterIndex > param2.ParameterIndex)
                return 1;
            else
            {
                if (param1.ParameterIndex < param2.ParameterIndex)
                    return -1;
                else if (param1.ParameterIndex > param2.ParameterIndex)
                    return 1;
            }
            
            return 0;
        }
    }
}
