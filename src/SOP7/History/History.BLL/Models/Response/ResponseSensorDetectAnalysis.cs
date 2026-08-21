using History.BLL.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseSensorDetectAnalysis
    {
        private List<SensorDetectAnalysisData> m_sensorDetectAnalysisDatas = new List<SensorDetectAnalysisData>();
        private int m_nAllDetectCount = 0;
        private double m_fAllMalfunctionRate = 0.0f;
        private string m_strMaxCountSensorName = "";
        private string m_strSearchZoneName = "전체";

        public List<SensorDetectAnalysisData> SensorDetectAnalysisDatas
        {
            get { return m_sensorDetectAnalysisDatas; }
            set { m_sensorDetectAnalysisDatas = value; }
        }
        public int AllDetectCount
        {
            get { return m_nAllDetectCount; }
            set { m_nAllDetectCount = value; }
        }
        public double AllMalfunctionRate
        {
            get { return m_fAllMalfunctionRate; }
            set { m_fAllMalfunctionRate = value; }
        }
        public string MaxCountSensorName
        {
            get { return m_strMaxCountSensorName; }
            set { m_strMaxCountSensorName = value; }
        }
        public string SearchZoneName
        {
            get { return m_strSearchZoneName; }
            set { m_strSearchZoneName = value; }
        }
    }
}
