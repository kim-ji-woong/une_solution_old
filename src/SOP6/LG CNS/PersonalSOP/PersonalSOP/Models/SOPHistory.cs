using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace PersonalSOP.Models
{
    using History;

    public class SOPHistory
    {
        private ActionStepHistory m_actionStepHistory = null;
        private string m_strSOPName = "";
        private string m_strSOPInfo = "";
        // SOP 시작시간
        private DateTime m_dtBegin;
        // 경과시간(초)
        private int m_nElapsedSeconds = 0;
        private ConcurrentDictionary<SOPHistoryData, SOPHistoryData> m_dicHistoryDatas = new ConcurrentDictionary<SOPHistoryData, SOPHistoryData>();
        private List<SOPHistoryData> m_sortedDatas = new List<SOPHistoryData>();

        public ActionStepHistory ActionStepHistory
        {
            get { return m_actionStepHistory; }
            set { m_actionStepHistory = value; }
        }

        public string SOPName
        {
            get { return m_strSOPName; }
            set { m_strSOPName = value; }
        }

        public string SOPInfo
        {
            get { return m_strSOPInfo; }
            set { m_strSOPInfo = value; }
        }

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public string BeginTimeString
        {
            get { return string.Format("{0}.{1}.{2} {3}:{4}", m_dtBegin.Year, m_dtBegin.Month, m_dtBegin.Day, m_dtBegin.Hour, m_dtBegin.Minute); }
        }

        public int ElapsedSeconds
        {
            get { return m_nElapsedSeconds; }
            set { m_nElapsedSeconds = value; }
        }

        private const int Minute = 60;
        private const int Hour = 3600;
        private const int Day = 3600 * 24;

        public string ElapsedTime
        {
            get
            {
                if (m_nElapsedSeconds >= Day)
                {
                    int nDay = m_nElapsedSeconds / Day;
                    int nSeconds = m_nElapsedSeconds % Day;

                    int nHour = nSeconds / Hour;
                    nSeconds = nSeconds % Hour;

                    int nMinute = nSeconds / Minute;
                    nSeconds = nSeconds % Minute;
                    return string.Format("경과 {0}일 {1}시간 {2}분 {3}초", nDay, nHour, nMinute, nSeconds);
                }
                else if (m_nElapsedSeconds >= Hour)
                {
                    int nHour = m_nElapsedSeconds / Hour;
                    int nSeconds = m_nElapsedSeconds % Hour;

                    int nMinute = nSeconds / Minute;
                    nSeconds = nSeconds % Minute;
                    return string.Format("경과 {0}시간 {1}분 {2}초", nHour, nMinute, nSeconds);
                }
                else if (m_nElapsedSeconds >= Minute)
                {
                    int nMinute = m_nElapsedSeconds / Minute;
                    int nSeconds = m_nElapsedSeconds % Minute;
                    return string.Format("경과 {0}분 {1}초", nMinute, nSeconds);
                }

                return string.Format("경과 {0}초", m_nElapsedSeconds);
            }
        }

        public int HistoryDataCount
        {
            get { return m_dicHistoryDatas.Count; }
        }

        public List<SOPHistoryData> HistoryDatas
        {
            get { return m_dicHistoryDatas.Values.ToList(); }
        }

        public List<SOPHistoryData> SortedHistoryDatas
        {
            get { return m_sortedDatas; }
        }

        public void AddHistoryData(SOPHistoryData data)
        {
            m_dicHistoryDatas[data] = data;
        }

        public void ClearHistoryDatas()
        {
            m_dicHistoryDatas.Clear();
        }

        public SOPHistory Clone()
        {
            SOPHistory history = new SOPHistory();

            history.m_actionStepHistory = m_actionStepHistory;
            history.m_strSOPInfo = m_strSOPInfo;
            history.m_strSOPName = m_strSOPName;
            history.m_dtBegin = m_dtBegin;
            history.m_nElapsedSeconds = m_nElapsedSeconds;

            List<SOPHistoryData> datas = HistoryDatas;

            foreach (SOPHistoryData data in datas)
            {
                history.m_dicHistoryDatas[data] = data;
            }

            return history;
        }

        public string GetDisasterName()
        {
            string[] tokens = m_strSOPName.Split('/');

            if (tokens.Count() >= 3)
                return tokens[2].Trim();

            return m_strSOPName;
        }

        public string GetActionStepName()
        {
            string strActionStepName = "심각";
            string[] tokens = m_strSOPName.Split('/');

            if (tokens.Count() >= 4)
                strActionStepName = tokens[3].Trim();

            int nIndex = strActionStepName.IndexOf('(');

            if (nIndex > 0)
                strActionStepName = strActionStepName.Substring(0, nIndex);

            return strActionStepName;
        }

        public string GetActionStepImage()
        {
            return "/Images/" + GetActionStepName() + ".png";
        }
    }

    public class SOPHistoryData : IComparable
    {
        private ComponentHistory m_componentHistory = null;
        private int m_no = 0;
        private DateTime? m_timeStamp = null;
        private string m_strTime = "";
        private string m_strTask = "";
        private string m_strState = "";
        private bool m_showingDetails = false;
        private List<SOPHistoryData> m_detailDatas = new List<SOPHistoryData>();

        public int No
        {
            get { return m_no; }
            set { m_no = value; }
        }

        public string Time
        {
            get
            {
                if (m_strTime.Length > 0)
                    return m_strTime;

                if (m_timeStamp != null)
                    m_strTime = SOPHistoryManager.MakeLogTimeString((DateTime)m_timeStamp);

                return m_strTime;
            }
            /*set
            {
                // 완료된 시각이 아닌 시작시각이 표시되도록 한다.
                if (m_strTime.Length == 0)
                    m_strTime = value;
                else if (string.Compare(m_strTime, value) > 0)
                    m_strTime = value;
            }*/
        }

        public string Task
        {
            get { return m_strTask; }
            set { m_strTask = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public ComponentHistory ComponentHistory
        {
            get { return m_componentHistory; }
            set { m_componentHistory = value; }
        }

        public bool ShowingDetails
        {
            get { return m_showingDetails; }
            set { m_showingDetails = value; }
        }

        public List<SOPHistoryData> DetailDatas
        {
            get { return m_detailDatas; }
        }

        public bool IsComplete
        {
            get { return m_strState == "완료"; }
        }

        public int GetIndex(SOPHistory history)
        {
            return history.HistoryDatas.IndexOf(this);
        }

        public void SetTime(DateTime time)
        {
            // 완료된 시각이 아닌 시작시각이 표시되도록 한다.
            if (m_timeStamp == null)
                m_timeStamp = time;
            else if ((DateTime)m_timeStamp > time)
                m_timeStamp = time;

            SOPHistoryManager.MakeLogTimeString((DateTime)m_timeStamp);
        }

        public DateTime GetTime()
        {
            return (DateTime)m_timeStamp;
        }

        public int CompareTo(object obj)
        {
            SOPHistoryData data = (SOPHistoryData)obj;

            if (this.ComponentHistory != null && data.ComponentHistory != null)
                return this.ComponentHistory.CompareTo(data.ComponentHistory);

            return m_strTime.CompareTo(data.m_strTime);
        }
    }
}
