using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaData.Models
{
    public abstract class SensorValue
    {
        private Sensor m_sensor = null;
        private DateTime m_time = new DateTime();
        // 위도 : -90 ~ 90
        //        범위를 벗어나면 m_sensor의 값을 사용한다.
        private float m_fLatitude = -100.0f;
        // 경도 : -180 ~ 180
        //        범위를 벗어나면 m_sensor의 값을 사용한다.
        private float m_fLongitude = -200.0f;
        private string m_strDescription = "";
        private int m_nID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public Sensor Sensor
        {
            get { return m_sensor; }
            set { m_sensor = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
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

        public bool IsValidLatitude()
        {
            return m_fLatitude >= -90.0f && m_fLatitude <= 90.0f;
        }

        public bool IsValidLongitude()
        {
            return m_fLongitude >= -180.0f && m_fLongitude <= 180.0f;
        }

        public abstract string GetValueString();
    }

    public class SensorValuei : SensorValue
    {
        private int m_nData = 0;

        public int Data
        {
            get { return m_nData; }
            set { m_nData = value; }
        }

        public SensorValuei()
        {
        }

        public SensorValuei(int nData)
        {
            m_nData = nData;
        }

        public override string GetValueString()
        {
            return m_nData.ToString();
        }
    }

    public class SensorValuef : SensorValue
    {
        private float m_fData = 0.0f;

        public float Data
        {
            get { return m_fData; }
            set { m_fData = value; }
        }

        public SensorValuef()
        {
        }

        public SensorValuef(float fData)
        {
            m_fData = fData;
        }

        public override string GetValueString()
        {
            return m_fData.ToString();
        }
    }

    public class SensorValues : SensorValue
    {
        private string m_strData = "";

        public string Data
        {
            get { return m_strData; }
            set { m_strData = value; }
        }

        public SensorValues()
        {
        }

        public SensorValues(string strData)
        {
            m_strData = strData;
        }

        public override string GetValueString()
        {
            return m_strData;
        }
    }

    public class SensorData
    {
        public int ID
        {
            get;
            set;
        }

        public int SensorID
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public float Latitude
        {
            get;
            set;
        }

        public float Longitude
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }

    public class SensorData2
    {
        public int ID
        {
            get;
            set;
        }

        public int SensorID
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }

        public SensorData2()
        {
        }

        public SensorData2(int nID, int nSensorID, DateTime time, string data)
        {
            ID = nID;
            SensorID = nSensorID;
            Time = time;
            Data = data;
        }
    }

    public class RegionTime
    {
        public int RegionID
        {
            get;
            set;
        }

        public string BeginTime
        {
            get;
            set;
        }

        public string EndTime
        {
            get;
            set;
        }
    }

    public class RectTime
    {
        public float TLx
        {
            get;
            set;
        }

        public float TLy
        {
            get;
            set;
        }

        public float BLx
        {
            get;
            set;
        }

        public float BLy
        {
            get;
            set;
        }

        public float BRx
        {
            get;
            set;
        }

        public float BRy
        {
            get;
            set;
        }

        public string BeginTime
        {
            get;
            set;
        }

        public string EndTime
        {
            get;
            set;
        }
    }
}
