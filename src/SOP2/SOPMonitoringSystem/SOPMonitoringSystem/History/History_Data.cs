using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SOPMonitoringSystem.History
{
    public class ActionStepHistory
    {
        // ActionStepID(0보다 크면 실제 모드, 0보다 작으면 모의훈련모드), ActionStepUnitHistory List
        private Dictionary<int, ArrayList> m_dicHistory = new Dictionary<int, ArrayList>();

        public void AddHistory(int nActionStepID, bool isRealMode, int nHistoryID, DateTime dtBegin, DateTime dtEnd)
        {
            ArrayList arrHistory = null;

            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (!m_dicHistory.ContainsKey(nActionStepID))
            {
                arrHistory = new ArrayList();
                m_dicHistory[nActionStepID] = arrHistory;
            }
            else
                arrHistory = m_dicHistory[nActionStepID];

            ActionStepUnitHistory history = FindHistory(nHistoryID, arrHistory);
            if (history == null)
                arrHistory.Add(new ActionStepUnitHistory(nHistoryID, dtBegin, dtEnd));
        }

        public int GetCompletedCount(int nActionStepID, bool isRealMode)
        {
            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (m_dicHistory.ContainsKey(nActionStepID))
            {
                ArrayList arrHistory = m_dicHistory[nActionStepID];
                return arrHistory.Count;
            }

            return 0;
        }

        private ActionStepUnitHistory FindHistory(int nHistoryID, ArrayList arrHistory)
        {
            foreach (ActionStepUnitHistory history in arrHistory)
            {
                if (history.ID == nHistoryID)
                    return history;
            }

            return null;
        }

        public ActionStepUnitHistory GetHistory(int nActionStepID, bool isRealMode, int nIndex)
        {
            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (m_dicHistory.ContainsKey(nActionStepID))
            {
                ArrayList arrHistory = m_dicHistory[nActionStepID];
                if (nIndex >= arrHistory.Count)
                    return null;

                return (ActionStepUnitHistory)arrHistory[nIndex];
            }

            return null;
        }
    }

    // ActionStepHistory Table의 한 행에 해당하는 데이터
    // ActionStepHistory Class는 하나의 ActionStepID에 대한 전체 Event들을 관리하며
    // ActionStepHistoryUnit은 특정 Event에 대한 내용이다.
    public class ActionStepUnitHistory
    {
        private int m_nHistoryID = -1;
        private DateTime m_timeBegin;
        private DateTime m_timeEnd;

        public ActionStepUnitHistory(int nHistoryID, DateTime dtBegin, DateTime dtEnd)
        {
            m_nHistoryID = nHistoryID;
            m_timeBegin = dtBegin;
            m_timeEnd = dtEnd;
        }

        public int ID
        {
            get { return m_nHistoryID; }
            set { m_nHistoryID = value; }
        }

        public DateTime BeginTime
        {
            get { return m_timeBegin; }
            set { m_timeBegin = value; }
        }

        public DateTime EndTime
        {
            get { return m_timeEnd; }
            set { m_timeEnd = value; }
        }
    }
}
