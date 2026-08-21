using Common.Model.History;
using SOPManager.Model.Sop.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOPSimulator.BLL.Models.Response
{
    public class ResponseMonitor
    {
        private List<HistoryData> m_historyData = null;
        public List<HistoryData> HistoryData
        {
            get { return m_historyData; }
            set { m_historyData = value; }
        }
    }

    public class HistoryData
    {
        private List<ComponentHistory> m_componentHistories = null;
        public List<ComponentHistory> ComponentHistories
        {
            get { return m_componentHistories; }
            set { m_componentHistories = value; }
        }

        private List<ComponentHistoryDetail> m_componentHistoryDetails = null;
        public List<ComponentHistoryDetail> ComponentHistoryDetails
        {
            get { return m_componentHistoryDetails; }
            set { m_componentHistoryDetails = value; }
        }

        private ActionStepHistory m_actionStepHistory = null;
        public ActionStepHistory ActionStepHistory
        {
            get { return m_actionStepHistory; }
            set { m_actionStepHistory = value; }
        }

        private ActionStep m_actionStep = null;
        public ActionStep ActionStep
        {
            get { return m_actionStep; }
            set { m_actionStep = value; }
        }

        private Disaster m_disaster = null;
        public Disaster Disaster
        {
            get { return m_disaster; }
            set { m_disaster = value; }
        }

        private int m_nVersionID = -1;
        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }
    }
}
