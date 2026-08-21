using System;
using System.Collections.Generic;
using System.Collections;

namespace WifiSensorService.Data.Response
{
    public class ResponseSensorAvgData : MessageResult
    {
        private ArrayList m_sensorAvgDatas = new ArrayList();

        public ArrayList SensorAvgDatas
        {
            get { return m_sensorAvgDatas; }
            set { m_sensorAvgDatas = value; }
        }

        public ResponseSensorAvgData()
        {
        }

        public ResponseSensorAvgData(bool success, string message)
            : base(success, message)
        {
        }
    }

    public class SensorAvgData : IComparable
    {
        private string m_strSerno = null;
        private float? m_fPm2_5 = null;
        private float? m_fNo2 = null;
        private float? m_fO3 = null;
        private float? m_fTemp = null;
        private float? m_fHumidity = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? Pm2_5
        {
            get { return m_fPm2_5; }
            set { m_fPm2_5 = value; }
        }

        public float? No2
        {
            get { return m_fNo2; }
            set { m_fNo2 = value; }
        }

        public float? O3
        {
            get { return m_fO3; }
            set { m_fO3 = value; }
        }

        public float? Temp
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public float? Humi
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgData(string strSerno = null)
        {
            m_strSerno = strSerno;
        }

        public SensorAvgData(int year, string strSerno = null)
        {
            m_strDate = year.ToString();
            m_strSerno = strSerno;
        }

        public SensorAvgData(int year, int month, string strSerno = null)
        {
            m_strDate = string.Format("{0}-{1:00}", year, month);
            m_strSerno = strSerno;
        }

        public SensorAvgData(int year, int month, int week, bool isWeek, string strSerno = null)
        {
            if (isWeek)
                m_strDate = string.Format("{0}-{1:00}-{2}", year, month, week);
            else
            {
                int day = week;
                m_strDate = string.Format("{0}-{1:00}-{2:00}", year, month, day);
            }

            m_strSerno = strSerno;
        }

        public int CompareTo(object obj)
        {
            SensorAvgData data = (SensorAvgData)obj;
            return string.Compare(this.m_strDate, data.m_strDate);
        }

        public object ToSensorAvgObject()
        {
            if (m_strSerno == null)
            {
                if (m_fPm2_5 != null && m_fNo2 != null && m_fO3 != null && m_fTemp != null && m_fHumidity != null)
                    return new SensorAvgDataAllNoSerno(m_fPm2_5, m_fNo2, m_fO3, m_fTemp, m_fHumidity, m_strDate);
                else if (m_fPm2_5 != null)
                    return new SensorAvgDataPm2_5NoSerno(m_fPm2_5, m_strDate);
                else if (m_fNo2 != null)
                    return new SensorAvgDataNo2NoSerno(m_fNo2, m_strDate);
                else if (m_fO3 != null)
                    return new SensorAvgDataO3NoSerno(m_fO3, m_strDate);
                else if (m_fTemp != null)
                    return new SensorAvgDataTempNoSerno(m_fTemp, m_strDate);
                else if (m_fHumidity != null)
                    return new SensorAvgDataHumidityNoSerno(m_fHumidity, m_strDate);
            }
            else
            {
                if (m_fPm2_5 != null && m_fNo2 != null && m_fO3 != null && m_fTemp != null && m_fHumidity != null)
                    return new SensorAvgDataAllWithSerno(m_strSerno, m_fPm2_5, m_fNo2, m_fO3, m_fTemp, m_fHumidity, m_strDate);
                else if (m_fPm2_5 != null)
                    return new SensorAvgDataPm2_5WithSerno(m_strSerno, m_fPm2_5, m_strDate);
                else if (m_fNo2 != null)
                    return new SensorAvgDataNo2WithSerno(m_strSerno, m_fNo2, m_strDate);
                else if (m_fO3 != null)
                    return new SensorAvgDataO3WithSerno(m_strSerno, m_fO3, m_strDate);
                else if (m_fTemp != null)
                    return new SensorAvgDataTempWithSerno(m_strSerno, m_fTemp, m_strDate);
                else if (m_fHumidity != null)
                    return new SensorAvgDataHumidityWithSerno(m_strSerno, m_fHumidity, m_strDate);
            }

            return null;
        }
    }

    public class SensorAvgDataAllWithSerno
    {
        private string m_strSerno = null;
        private float? m_fPm2_5 = null;
        private float? m_fNo2 = null;
        private float? m_fO3 = null;
        private float? m_fTemp = null;
        private float? m_fHumidity = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? Pm2_5
        {
            get { return m_fPm2_5; }
            set { m_fPm2_5 = value; }
        }

        public float? No2
        {
            get { return m_fNo2; }
            set { m_fNo2 = value; }
        }

        public float? O3
        {
            get { return m_fO3; }
            set { m_fO3 = value; }
        }

        public float? Temp
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public float? Humi
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataAllWithSerno(string strSerno, float? fPm2_5, float? fNo2, float? fO3, float? fTemp, float? fHumidity, string strDate)
        {
            m_strSerno = strSerno;
            m_fPm2_5 = fPm2_5;
            m_fNo2 = fNo2;
            m_fO3 = fO3;
            m_fTemp = fTemp;
            m_fHumidity = fHumidity;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataAllNoSerno
    {
        private float? m_fPm2_5 = null;
        private float? m_fNo2 = null;
        private float? m_fO3 = null;
        private float? m_fTemp = null;
        private float? m_fHumidity = null;
        private string m_strDate = "";

        public float? Pm2_5
        {
            get { return m_fPm2_5; }
            set { m_fPm2_5 = value; }
        }

        public float? No2
        {
            get { return m_fNo2; }
            set { m_fNo2 = value; }
        }

        public float? O3
        {
            get { return m_fO3; }
            set { m_fO3 = value; }
        }

        public float? Temp
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public float? Humi
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataAllNoSerno(float? fPm2_5, float? fNo2, float? fO3, float? fTemp, float? fHumidity, string strDate)
        {
            m_fPm2_5 = fPm2_5;
            m_fNo2 = fNo2;
            m_fO3 = fO3;
            m_fTemp = fTemp;
            m_fHumidity = fHumidity;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataPm2_5WithSerno
    {
        private string m_strSerno = null;
        private float? m_fPm2_5 = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? Pm2_5
        {
            get { return m_fPm2_5; }
            set { m_fPm2_5 = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataPm2_5WithSerno(string strSerno, float? fPm2_5, string strDate)
        {
            m_strSerno = strSerno;
            m_fPm2_5 = fPm2_5;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataPm2_5NoSerno
    {
        private float? m_fPm2_5 = null;
        private string m_strDate = "";

        public float? Pm2_5
        {
            get { return m_fPm2_5; }
            set { m_fPm2_5 = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataPm2_5NoSerno(float? fPm2_5, string strDate)
        {
            m_fPm2_5 = fPm2_5;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataNo2WithSerno
    {
        private string m_strSerno = null;
        private float? m_fNo2 = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? No2
        {
            get { return m_fNo2; }
            set { m_fNo2 = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataNo2WithSerno(string strSerno, float? fNo2, string strDate)
        {
            m_strSerno = strSerno;
            m_fNo2 = fNo2;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataNo2NoSerno
    {
        private float? m_fNo2 = null;
        private string m_strDate = "";

        public float? No2
        {
            get { return m_fNo2; }
            set { m_fNo2 = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataNo2NoSerno(float? fNo2, string strDate)
        {
            m_fNo2 = fNo2;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataO3WithSerno
    {
        private string m_strSerno = null;
        private float? m_fO3 = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? O3
        {
            get { return m_fO3; }
            set { m_fO3 = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataO3WithSerno(string strSerno, float? fO3, string strDate)
        {
            m_strSerno = strSerno;
            m_fO3 = fO3;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataO3NoSerno
    {
        private float? m_fO3 = null;
        private string m_strDate = "";

        public float? O3
        {
            get { return m_fO3; }
            set { m_fO3 = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataO3NoSerno(float? fO3, string strDate)
        {
            m_fO3 = fO3;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataTempWithSerno
    {
        private string m_strSerno = null;
        private float? m_fTemp = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? Temp
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataTempWithSerno(string strSerno, float? fTemp, string strDate)
        {
            m_strSerno = strSerno;
            m_fTemp = fTemp;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataTempNoSerno
    {
        private float? m_fTemp = null;
        private string m_strDate = "";

        public float? Temp
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataTempNoSerno(float? fTemp, string strDate)
        {
            m_fTemp = fTemp;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataHumidityWithSerno
    {
        private string m_strSerno = null;
        private float? m_fHumidity = null;
        private string m_strDate = "";

        public string Serno
        {
            get { return m_strSerno; }
            set { m_strSerno = value; }
        }

        public float? Humi
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataHumidityWithSerno(string strSerno, float? fHumidity, string strDate)
        {
            m_strSerno = strSerno;
            m_fHumidity = fHumidity;
            m_strDate = strDate;
        }
    }

    public class SensorAvgDataHumidityNoSerno
    {
        private float? m_fHumidity = null;
        private string m_strDate = "";

        public float? Humi
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public SensorAvgDataHumidityNoSerno(float? fHumidity, string strDate)
        {
            m_fHumidity = fHumidity;
            m_strDate = strDate;
        }
    }
}
