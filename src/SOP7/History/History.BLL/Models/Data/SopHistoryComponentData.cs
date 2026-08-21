using Common.Model.History;
using SOPManager.Model.Sop.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Data
{
    public class SopHistoryComponentData
    {
        private int m_nActionStepHistoryID = -1;
        private int m_nComponentHistoryID = -1;
        private int m_nComponentID = -1;
        private int m_nComponentType = -1;
        private string m_strSectionName = "";
        private List<string> m_teamList = new List<string>();
        private string m_strTime = "";
        private int m_nStatus = -1;
        private string m_strStatus = "";
        private int m_nUserID = -1;
        private string m_strUserName = "";
        private string m_strCompletion = "확인";
        private List<ComponentHistoryDetailData> m_missionDatas = new List<ComponentHistoryDetailData>();

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }
        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }
        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }
        public int ComponentType
        {
            get { return m_nComponentType; }
            set { m_nComponentType = value; }
        }
        public string SectionName
        {
            get { return m_strSectionName; }
            set { m_strSectionName = value; }
        }
        public List<string> TeamList
        {
            get { return m_teamList; }
            set { m_teamList = value; }
        }
        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }
        public string strStatus
        {
            get { return m_strStatus; }
            set { m_strStatus = value; }
        }
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }
        public string Completion
        {
            get { return m_strCompletion; }
            set { m_strCompletion = value; }
        }
        public List<ComponentHistoryDetailData> MissionDatas
        {
            get { return m_missionDatas; }
            set { m_missionDatas = value; }
        }
    }

    public class ComponentHistoryDetailData
    {
        private int m_nDataIndex = -1;
        private string m_strSectionName = "";
        private string m_strMissionText = "";
        private string m_strTime = "";
        private string m_strCompletion = "미완료";

        public int DataIndex
        {
            get { return m_nDataIndex; }
            set { m_nDataIndex = value; }
        }
        public string SectionName
        {
            get { return m_strSectionName; }
            set { m_strSectionName = value; }
        }
        public string MissionText
        {
            get { return m_strMissionText; }
            set { m_strMissionText = value; }
        }
        public string Completion
        {
            get { return m_strCompletion; }
            set { m_strCompletion = value; }
        }
        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }
    }
}
