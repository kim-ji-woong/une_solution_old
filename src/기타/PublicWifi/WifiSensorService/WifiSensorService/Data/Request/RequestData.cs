using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WifiSensorService.Data.Request
{
    public class RequestData
    {
        private SensorData m_sensorData = null;
        private RequestSensorData m_requestSensorData = null;
        private RequestSensorAvgData m_requestSensorAvgData = null;
        private RequestSensorAlarm m_requestSensorAlarm = null;

        public SensorData SensorData
        {
            get { return m_sensorData; }
            set { m_sensorData = value; }
        }

        public RequestSensorData RequestSensorData
        {
            get { return m_requestSensorData; }
            set { m_requestSensorData = value; }
        }

        public RequestSensorAvgData RequestSensorAvgData
        {
            get { return m_requestSensorAvgData; }
            set { m_requestSensorAvgData = value; }
        }

        public RequestSensorAlarm RequestSensorAlarm
        {
            get { return m_requestSensorAlarm; }
            set { m_requestSensorAlarm = value; }
        }
    }

    public class SensorData
    {
        private string m_strSerNo = "";
        private float m_fPm2_5 = -999;
        private float m_fNo2 = -999;
        private float m_fO3 = -999;
        private float m_fTemp = -999;
        private float m_fHumidity = -999;
        private string m_regdate = "";
        private float m_fLatitude = -999;
        private float m_fLongitude = -999;

        public string Serno
        {
            get { return m_strSerNo; }
            set { m_strSerNo = value; }
        }

        public float Pm2_5
        {
            get { return m_fPm2_5; }
            set { m_fPm2_5 = value; }
        }

        public float No2
        {
            get { return m_fNo2; }
            set { m_fNo2 = value; }
        }

        public float O3
        {
            get { return m_fO3; }
            set { m_fO3 = value; }
        }

        public float Temp
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public float Humi
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public string Regdate
        {
            get { return m_regdate; }
            set { m_regdate = value; }
        }

        public float Lat
        {
            get { return m_fLatitude; }
            set { m_fLatitude = value; }
        }

        public float Lon
        {
            get { return m_fLongitude; }
            set { m_fLongitude = value; }
        }

        // 유효한 값이 아닌지 확인한다.
        public bool IsValid(out string strErrorMessage)
        {
            strErrorMessage = null;

            if (m_fPm2_5 < 0 || m_fPm2_5 > 1000)
            {
                strErrorMessage = string.Format("pm2_5 값이 잘못되었습니다. : {0}", m_fPm2_5);
                return false;
            }

            if (m_fNo2 < 0 || m_fNo2 > 1000)
            {
                strErrorMessage = string.Format("no2 값이 잘못되었습니다. : {0}", m_fNo2);
                return false;
            }

            if (m_fO3 < 0 || m_fO3 > 1000)
            {
                strErrorMessage = string.Format("o3 값이 잘못되었습니다. : {0}", m_fO3);
                return false;
            }

            if (m_fTemp < -100 || m_fTemp > 100)
            {
                strErrorMessage = string.Format("temp 값이 잘못되었습니다. : {0}", m_fTemp);
                return false;
            }

            if (m_fHumidity < 0 || m_fHumidity > 100)
            {
                strErrorMessage = string.Format("humi 값이 잘못되었습니다. : {0}", m_fHumidity);
                return false;
            }

            return true;
        }
    }

    public class RequestSensorData
    {
        private string m_strSerno = null;
        private int m_nBeginYear = 0;
        private int m_nEndYear = 0;
        private int? m_nBeginMonth = null;
        private int? m_nEndMonth = null;
        private int? m_nBeginDay = null;
        private int? m_nEndDay = null;
        private int? m_nBeginHour = null;
        private int? m_nEndHour = null;
        private int? m_nBeginMinute = null;
        private int? m_nEndMinute = null;
        private int? m_nBeginSecond = null;
        private int? m_nEndSecond = null;

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public int BeginYear
        {
            get { return m_nBeginYear; }
            set { m_nBeginYear = value; }
        }

        public int EndYear
        {
            get { return m_nEndYear; }
            set { m_nEndYear = value; }
        }

        public int? BeginMonth
        {
            get { return m_nBeginMonth; }
            set { m_nBeginMonth = value; }
        }

        public int? EndMonth
        {
            get { return m_nEndMonth; }
            set { m_nEndMonth = value; }
        }

        public int? BeginDay
        {
            get { return m_nBeginDay; }
            set { m_nBeginDay = value; }
        }

        public int? EndDay
        {
            get { return m_nEndDay; }
            set { m_nEndDay = value; }
        }

        public int? BeginHour
        {
            get { return m_nBeginHour; }
            set { m_nBeginHour = value; }
        }

        public int? EndHour
        {
            get { return m_nEndHour; }
            set { m_nEndHour = value; }
        }

        public int? BeginMinute
        {
            get { return m_nBeginMinute; }
            set { m_nBeginMinute = value; }
        }

        public int? EndMinute
        {
            get { return m_nEndMinute; }
            set { m_nEndMinute = value; }
        }

        public int? BeginSecond
        {
            get { return m_nBeginSecond; }
            set { m_nBeginSecond = value; }
        }

        public int? EndSecond
        {
            get { return m_nEndSecond; }
            set { m_nEndSecond = value; }
        }
    }

    public class RequestSensorAvgData
    {
        private string m_strSerno = null;
        private string m_strSensorType = "";
        private string m_strAvgDate = "";
        private int m_nBeginYear = 0;
        private int m_nEndYear = 0;
        private int? m_nBeginMonth = null;
        private int? m_nEndMonth = null;
        private int? m_nBeginDay = null;
        private int? m_nEndDay = null;

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public string AvgDate
        {
            get { return m_strAvgDate; }
            set { m_strAvgDate = value; }
        }

        public int BeginYear
        {
            get { return m_nBeginYear; }
            set { m_nBeginYear = value; }
        }

        public int EndYear
        {
            get { return m_nEndYear; }
            set { m_nEndYear = value; }
        }

        public int? BeginMonth
        {
            get { return m_nBeginMonth; }
            set { m_nBeginMonth = value; }
        }

        public int? EndMonth
        {
            get { return m_nEndMonth; }
            set { m_nEndMonth = value; }
        }

        public int? BeginDay
        {
            get { return m_nBeginDay; }
            set { m_nBeginDay = value; }
        }

        public int? EndDay
        {
            get { return m_nEndDay; }
            set { m_nEndDay = value; }
        }
    }

    public class RequestSensorAlarm
    {
        private bool m_activeOnly = false;

        public bool ActiveOnly
        {
            get { return m_activeOnly; }
            set { m_activeOnly = value; }
        }
    }
}
