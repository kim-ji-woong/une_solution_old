using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWatcher.Alarm
{
    public class AvgDay
    {
        private int m_nYear = 0;
        private int m_nMonth = 0;
        private int m_nDay = 0;
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

        public AvgDay()
        {
        }

        public AvgDay(DateTime time)
        {
            m_nYear = time.Year;
            m_nMonth = time.Month;
            m_nDay = time.Day;
        }

        public void SetData(float fData)
        {
            m_fData += fData;
            m_nDataCount++;
        }
    }
}
