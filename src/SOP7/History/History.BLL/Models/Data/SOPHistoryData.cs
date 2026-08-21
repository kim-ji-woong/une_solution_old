using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Data
{
    public class SOPHistoryData
    {
        private int m_nSensorZoneHistoryID = -1;
        private int m_nActionStepHistoryID = -1;
        private int m_nLastAccessedUserID = -1;
        private string m_strDisasterName = "";
        private string m_strSopName = "";
        private string m_strActionStepName = "";
        private string m_strSensorName = "";
        private string m_strRealMode = "";
        private string m_strPosition = "";        
        private string m_strBeginTime = "";
        private string m_strEndTime = "";
        private string m_strUserName = "";
        private List<int> m_allSensorZoneIDs = null;

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }
        public int LastAccessedUserID
        {
            get { return m_nLastAccessedUserID; }
            set { m_nLastAccessedUserID = value; }
        }
        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }
        public string SopName
        {
            get { return m_strSopName; }
            set { m_strSopName = value; }
        }
        public string ActionStepName
        {
            get { return m_strActionStepName; }
            set { m_strActionStepName = value; }
        }
        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }
        public string RealMode
        {
            get { return m_strRealMode; }
            set { m_strRealMode = value; }
        }
        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }
        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }
        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }
        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }
        public List<int> AllSensorZoneIDs
        {
            get { return m_allSensorZoneIDs; }
            set { m_allSensorZoneIDs = value; }
        }
    }
}
