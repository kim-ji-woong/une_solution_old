using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaData.Models
{
    public class Sensor
    {
        public enum DataType { INTEGER = 0, FLOAT, STRING, UNKNOWN };

        private string m_strSensorName = "";
        private string m_strSensorType = "";
        // 하나의 센서가 값을 표현하는 영역의 반지름(meter)
        // 0보다 작으면 Coverage가 무시됨
        private int m_nCoverage = -1;
        private int m_nID = -1;
        // 위도 : -90 ~ 90
        //        범위를 벗어나면 값이 무시됨
        private float m_fLatitude = -100.0f;
        // 경도 : -180 ~ 180
        //        범위를 벗어나면 값이 무시됨
        private float m_fLongitude = -200.0f;
        private string m_strDescription = "";
        private DataType m_dataType = DataType.UNKNOWN;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public DataType SensorDataType
        {
            get { return m_dataType; }
            set { m_dataType = value; }
        }

        public int Coverage
        {
            get { return m_nCoverage; }
            set { m_nCoverage = value; }
        }

        public float Latitude
        {
            get { return m_fLatitude; }
            set { m_fLatitude = value; }
        }

        public float Longitude
        {
            get { return m_fLongitude; }
            set { m_fLongitude = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public bool IsValidCoverage()
        {
            return m_nCoverage > 0;
        }

        public bool IsValidLatitude()
        {
            return m_fLatitude >= -90.0f && m_fLatitude <= 90.0f;
        }

        public bool IsValidLongitude()
        {
            return m_fLongitude >= -180.0f && m_fLongitude <= 180.0f;
        }
    }
}
