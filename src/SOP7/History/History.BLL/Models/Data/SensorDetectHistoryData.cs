using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Data
{
    public class SensorDetectHistoryData
    {
        private int m_nSensorZoneHistoryID = -1;
        private int m_nReactionType = -1;
        private string m_strTime = "";
        private string m_strEndTime = "";
        private string m_strType = "";
        private string m_strSensorName = "";
        private string m_strZoneName = "";
        private string m_strRealMode = "";
        private string m_strDetectType = ""; //감지 유형 ?
        private string m_strDetectInfo = ""; //감지 정보 ?
        private string m_strAlarmLevel = "";
        private string m_strSopBeginTime = "";
        private string m_strSopEndTime = "";
        private string m_strSopName = "";
        private int m_nActionStepHistoryID = -1;
        private string m_strMemo = "";
        private List<int> m_allSensorZoneIDs = null;
        private int m_nSensorZoneID = -1;

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }
        public int ReactionType
        {
            get { return m_nReactionType; }
            set { m_nReactionType = value; }
        }
        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }
        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }
        public string Type
        {
            get { return m_strType; }
            set { m_strType = value; }
        }
        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }
        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }
        public string RealMode
        {
            get { return m_strRealMode; }
            set { m_strRealMode = value; }
        }
        public string DetectType
        {
            get { return m_strDetectType; }
            set { m_strDetectType = value; }
        }
        public string DetectInfo
        {
            get { return m_strDetectInfo; }
            set { m_strDetectInfo = value; }
        }
        public string AlarmLevel
        {
            get { return m_strAlarmLevel; }
            set { m_strAlarmLevel = value; }
        }
        public string SopBeginTime
        {
            get { return m_strSopBeginTime; }
            set { m_strSopBeginTime = value; }
        }
        public string SopEndTime
        {
            get { return m_strSopEndTime; }
            set { m_strSopEndTime = value; }
        }
        public string SopName
        {
            get { return m_strSopName; }
            set { m_strSopName = value; }
        }
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }
        public string Memo
        {
            get { return m_strMemo; }
            set { m_strMemo = value; }
        }
        public List<int> AllSensorZoneIDs
        {
            get { return m_allSensorZoneIDs; }
            set { m_allSensorZoneIDs = value; }
        }
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }
    }
}
