using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWatcher.Alarm
{
    public class AvgQuaterDay
    {
        private int m_nYear = 0;
        private int m_nMonth = 0;
        private int m_nDay = 0;
        private int m_nQuater = 0;
        // 일별 누적 데이터
        private float m_fData = 0;
        // 데이터 개수
        private int m_nDataCount = 0;

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        public int Month
        {
            get { return m_nMonth; }
            set { m_nMonth = value; }
        }

        public int Day
        {
            get { return m_nDay; }
            set { m_nDay = value; }
        }

        public int Quater
        {
            get { return m_nQuater; }
            set { m_nQuater = value; }
        }

        public int DayIndex
        {
            get { return m_nYear * 10000 + m_nMonth * 100 + m_nDay; }
        }

        public float Data
        {
            get { return m_fData; }
        }

        public float DataAverage
        {
            get { return m_fData / m_nDataCount; }
        }

        public int DataCount
        {
            get { return m_nDataCount; }
        }

        public AvgQuaterDay()
        {
        }

        public AvgQuaterDay(DateTime time)
        {
            m_nYear = time.Year;
            m_nMonth = time.Month;
            m_nDay = time.Day;
            m_nQuater = time.Hour / 6;
        }

        public void SetData(float fData)
        {
            m_fData += fData;
            m_nDataCount++;
        }
    }
}
