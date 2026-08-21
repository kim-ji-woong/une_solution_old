using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Data
{
    public class SensorDetectAnalysisData
    {
        private int m_nSensorZoneHistoryID = -1;
        private int m_nSensorZoneID = -1;
        private string m_strType = "";
        private int m_nZoneID = -1;
        private string m_strZoneName = "";
        private string m_strSensorName = "";
        private int m_nDetectCount = 0; // 탐지 횟수
        private double m_nDetectRate = 0; // 탐지률(%)
        private int m_nEndCount = 0;    // 현장 복구 횟수
        private int m_nUserResetCount = 0;   // 사용자 복구 횟수
        private int m_nMalfunctionCount = 0; // 오작동, 사용자복구(누출) 횟수
        private double m_fMalfunctionRate = 0.0f; // 오작동률(%)

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }
        public string Type
        {
            get { return m_strType; }
            set { m_strType = value; }
        }
        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }
        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }
        public int DetectCount
        {
            get { return m_nDetectCount; }
            set { m_nDetectCount = value; }
        }
        public double DetectRate
        {
            get { return m_nDetectRate; }
            set { m_nDetectRate = value; }
        }
        public int UserResetCount
        {
            get { return m_nUserResetCount; }
            set { m_nUserResetCount = value; }
        }
        public int EndCount
        {
            get { return m_nEndCount; }
            set { m_nEndCount = value; }
        }
        public int MalfunctionCount
        {
            get { return m_nMalfunctionCount; }
            set { m_nMalfunctionCount = value; }
        }
        public double MalfunctionRate
        {
            get { return m_fMalfunctionRate; }
            set { m_fMalfunctionRate = value; }
        }
    }
}
