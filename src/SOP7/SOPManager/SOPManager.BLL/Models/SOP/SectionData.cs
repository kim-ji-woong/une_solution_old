using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.BLL.Models.SOP
{
    using Model.Sop.Component;

    public class SectionData
    {
        private int m_nID = -1;
        private int m_nComponentType = -1;
        private int m_nGridID = -1;
        private int m_nGridColumnIndex = -1;
        private int m_nGridRowIndex = -1;
        private float m_fWidth = 0;
        private float m_fHeight = 0;
        private string m_strComponentID = "";
        private string m_strText = "";
        private bool? m_isBegin = null;
        private bool? m_autoRun = null;
        // string : TeamType(int) + "_" + TeamID(int) 조합
        private List<Receiver> m_receivers = null;
        private bool? m_onlyTeamLeader = null;
        private List<ProcessMissionData> m_processMissions = null;
        private bool? m_isSMS = null;
        private bool? m_isBroadcast = null;
        private bool? m_isEmail = null;
        private bool? m_useSiren = null;
        private string m_strMessage = "";
        private int? m_nTeamID = null;
        // m_nTeamID의 Team Type
        // StepMember.MemberTeamType
        private int? m_nTeamType = null;
        private string m_strTeamName = null;
        private string m_strAutoRunScript = null;
        //autoRunScript에서 사용되는 변수들의 Type에 관한 정보.(Unknown, boolean, double, integer, string)
        private string m_strAutoRunScriptVariableTypes = null;
        private int? m_nSectionNumber = null;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ComponentType
        {
            get { return m_nComponentType; }
            set { m_nComponentType = value; }
        }

        public int GridID
        {
            get { return m_nGridID; }
            set { m_nGridID = value; }
        }

        public int GridColumnIndex
        {
            get { return m_nGridColumnIndex; }
            set { m_nGridColumnIndex = value; }
        }

        public int GridRowIndex
        {
            get { return m_nGridRowIndex; }
            set { m_nGridRowIndex = value; }
        }

        public float Width
        {
            get { return m_fWidth; }
            set { m_fWidth = value; }
        }

        public float Height
        {
            get { return m_fHeight; }
            set { m_fHeight = value; }
        }

        public string ComponentID
        {
            get { return m_strComponentID; }
            set { m_strComponentID = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public bool? IsBegin
        {
            get { return m_isBegin; }
            set { m_isBegin = value; }
        }

        public bool? AutoRun
        {
            get { return m_autoRun; }
            set { m_autoRun = value; }
        }

        public List<Receiver> Receivers
        {
            get { return m_receivers; }
            set { m_receivers = value; }
        }

        public bool? OnlyTeamLeader
        {
            get { return m_onlyTeamLeader; }
            set { m_onlyTeamLeader = value; }
        }

        public List<ProcessMissionData> Missions
        {
            get { return m_processMissions; }
            set { m_processMissions = value; }
        }

        public bool? IsSMS
        {
            get { return m_isSMS; }
            set { m_isSMS = value; }
        }

        public bool? IsBroadcast
        {
            get { return m_isBroadcast; }
            set { m_isBroadcast = value; }
        }

        public bool? IsEmail
        {
            get { return m_isEmail; }
            set { m_isEmail = value; }
        }

        public bool? UseSiren
        {
            get { return m_useSiren; }
            set { m_useSiren = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public int? TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        // m_nTeamID의 Team Type
        // StepMember.MemberTeamType
        public int? TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string AutoRunScript
        {
            get { return m_strAutoRunScript; }
            set { m_strAutoRunScript = value; }
        }

        public string AutoRunScriptVariableTypes
        {
            get { return m_strAutoRunScriptVariableTypes; }
            set { m_strAutoRunScriptVariableTypes = value; }
        }

        public int? SectionNumber
        {
            get { return m_nSectionNumber; }
            set { m_nSectionNumber = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        private bool m_bChecked = false; // sop-simulator에서 사용
        public bool Checked
        {
            get { return m_bChecked; }
            set { m_bChecked = value; }
        }
    }
}
