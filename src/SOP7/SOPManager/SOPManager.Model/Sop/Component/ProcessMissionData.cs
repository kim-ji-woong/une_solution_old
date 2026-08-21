using System.Collections.Generic;

namespace SOPManager.Model.Sop.Component
{
    public class ProcessMissionData
    {
        public enum MissionDataType { Normal = 0, External, None };

        private MissionDataType m_type = MissionDataType.None;
        private int? m_nID = null;
        private string m_strMissionText = null;
        private int m_nProcessID = -1;
        private int? m_nOrderIndex = null;
        private int? m_nProgramID = null;
        private string m_strProgramName = null;
        private int? m_nParameterIndex = null;
        private string m_strValue = null;
        private List<string> m_parameters = null;
        private bool m_bChecked = false; // sop-simulator에서 사용

        public MissionDataType MissionType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int? ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MissionText
        {
            get { return m_strMissionText; }
            set { m_strMissionText = value; }
        }

        public int ProcessID
        {
            get { return m_nProcessID; }
            set { m_nProcessID = value; }
        }

        public int? OrderIndex
        {
            get { return m_nOrderIndex; }
            set { m_nOrderIndex = value; }
        }

        public int? ProgramID
        {
            get { return m_nProgramID; }
            set { m_nProgramID = value; }
        }

        public string ProgramName
        {
            get { return m_strProgramName; }
            set { m_strProgramName = value; }
        }

        public List<string> Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }

        public int? ParameterIndex
        {
            get { return m_nParameterIndex; }
            set { m_nParameterIndex = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public bool Checked
        {
            get { return m_bChecked; }
            set { m_bChecked = value; }
        }
    }
}
