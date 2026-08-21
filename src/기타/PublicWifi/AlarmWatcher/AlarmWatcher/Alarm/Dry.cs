using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmWatcher.Alarm
{
    public class Dry
    {
        public enum Status { None = 0, Level2, Level4 };

        private float m_humidityAvgLevel2 = 25;
        private int m_nHumidityDayLevel2 = 2;
        private float m_humidityAvgLevel4 = 15;
        private int m_nHumidityDayLevel4 = 2;

        private Status m_prevStatus = Status.None;
        private DateTime? m_prevAlarmTime = null;
        private AvgDay m_alarmData = null;
        private List<AvgDay> m_datas = new List<AvgDay>();
        private int m_nMaxDay = 2;

        public float? AlarmData
        {
            get
            {
                if (m_alarmData == null)
                    return null;

                return m_alarmData.DataAverage;
            }
        }

        public Dry(string strOptionLevel2, string strOptionLevel4)
        {
            float humidityLevel2, humidityLevel4;
            int dayLevel2, dayLevel4;

            if (ParseOption(strOptionLevel2, out humidityLevel2, out dayLevel2) &&
                ParseOption(strOptionLevel4, out humidityLevel4, out dayLevel4))
            {
                m_humidityAvgLevel2 = humidityLevel2;
                m_nHumidityDayLevel2 = dayLevel2;
                m_humidityAvgLevel4 = humidityLevel4;
                m_nHumidityDayLevel4 = dayLevel4;

                if (dayLevel2 > dayLevel4)
                    m_nMaxDay = dayLevel2;
                else
                    m_nMaxDay = dayLevel4;
            }
        }

        public void SetStatus(Status status, DateTime? time)
        {
            m_prevStatus = status;
            m_prevAlarmTime = time;
        }

        private bool ParseOption(string strOption, out float avg, out int day)
        {
            avg = 0;
            day = 0;

            string[] tokens = strOption.Split('_');

            if (tokens.Count() != 2)
                return false;

            if (float.TryParse(tokens[0].Trim(), out avg) && int.TryParse(tokens[1].Trim(), out day))
                return true;

            return false;
        }

        public Status SetData(float data, DateTime time, out bool isChanged)
        {
            isChanged = false;
            SetData(data, time);

            if (m_prevAlarmTime == null || (DateTime)m_prevAlarmTime < time)
            {
                bool? level2 = null;
                bool? level4 = null;
                int level2Count = 0, level4Count = 0;

                int nDataCount = m_datas.Count;
                int nBeginIndex = nDataCount - 1;

                for (int i = nBeginIndex; i >= 0; i--)
                {
                    AvgDay avgDay = m_datas[i];

                    if (level4Count < m_nHumidityDayLevel4 && (level4 == null || (bool)level4 == true))
                    {
                        if (avgDay.DataAverage <= m_humidityAvgLevel4)
                        {
                            level4Count++;
                            level4 = true;
                        }
                        else
                        {
                            level4Count = 0;
                            level4 = false;
                        }
                    }

                    if (level2Count < m_nHumidityDayLevel2 && (level2 == null || (bool)level2 == true))
                    {
                        if (avgDay.DataAverage <= m_humidityAvgLevel2)
                        {
                            level2Count++;
                            level2 = true;
                        }
                        else
                        {
                            level2Count = 0;
                            level2 = false;
                        }
                    }
                }

                if (level4Count >= m_nHumidityDayLevel4 && level4 != null && (bool)level4 == true)
                {
                    isChanged = m_prevStatus != Status.Level4;
                    m_prevStatus = Status.Level4;
                    m_prevAlarmTime = time;
                }
                else if (level2Count >= m_nHumidityDayLevel2 && level2 != null && (bool)level2 == true)
                {
                    isChanged = m_prevStatus != Status.Level2;
                    m_prevStatus = Status.Level2;
                    m_prevAlarmTime = time;
                }
                else
                {
                    isChanged = m_prevStatus != Status.None;
                    m_prevStatus = Status.None;
                    m_prevAlarmTime = time;
                }

                if (isChanged)
                {
                    if (m_prevStatus == Status.None)
                        m_alarmData = null;
                    else
                        m_alarmData = m_datas[nBeginIndex];
                }
            }

            return m_prevStatus;
        }

        private AvgDay GetDayData(DateTime time)
        {
            foreach (AvgDay day in m_datas)
            {
                if (day.Year == time.Year && day.Month == time.Month && day.Day == time.Day)
                    return day;
            }

            return null;
        }

        private void SetData(float data, DateTime time)
        {
            AvgDay avgDay = null;
            int nDataCount = m_datas.Count;

            if (nDataCount == 0)
            {
                avgDay = new AvgDay(time);
                m_datas.Add(avgDay);
                nDataCount++;
            }
            else
            {
                avgDay = GetDayData(time);
                //avgDay = m_datas[nDataCount - 1];

                if (avgDay == null)
                //if (avgDay.Year != time.Year && avgDay.Month != time.Month && avgDay.Day != time.Day)
                {
                    avgDay = new AvgDay(time);
                    m_datas.Add(avgDay);
                    TraceLastData();

                    if (nDataCount > m_nMaxDay)
                        m_datas.RemoveAt(0);
                    else
                        nDataCount++;
                }
            }

            avgDay.SetData(data);
        }

        private void TraceLastData()
        {
            if (m_datas.Count >= 2)
            {
                AvgDay last = m_datas[m_datas.Count - 2];
                System.Diagnostics.Trace.WriteLine("Dry, " + last.Day + " : " + last.DataAverage);
            }
        }
    }
}
