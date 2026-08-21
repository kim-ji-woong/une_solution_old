using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWatcher.Alarm
{
    public class AvgHour
    {
        private int m_nHour = 0;
        // 시간당 누적 데이터
        private float m_fData = 0;
        // 데이터 개수
        private int m_nDataCount = 0;

        public int Hour
        {
            get { return m_nHour; }
            set { m_nHour = value; }
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

        public AvgHour()
        {
        }

        public AvgHour(int hour)
        {
            m_nHour = hour;
        }

        public void SetData(float fData)
        {
            m_fData += fData;
            m_nDataCount++;
        }
    }
}
