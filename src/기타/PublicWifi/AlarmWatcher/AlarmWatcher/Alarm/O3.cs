using System;

namespace AlarmWatcher.Alarm
{
    public class O3
    {
        public enum Status { None = 0, Level2, Level4 };

        private float m_level2 = 120;
        private float m_level4 = 300;

        private Status m_prevStatus = Status.None;
        private DateTime? m_prevAlarmTime = null;
        private AvgQuaterDay m_alarmData = null;

        private AvgQuaterDay m_currentData = null;

        public float? AlarmData
        {
            get
            {
                if (m_alarmData == null)
                    return null;

                return m_alarmData.DataAverage;
            }
        }

        public O3(string strOptionLevel2, string strOptionLevel4)
        {
            float level2, level4;

            if (float.TryParse(strOptionLevel2, out level2) && float.TryParse(strOptionLevel4, out level4))
            {
                m_level2 = level2;
                m_level4 = level4;
            }
        }

        public void SetStatus(Status status, DateTime? time)
        {
            m_prevStatus = status;
            m_prevAlarmTime = time;
    }

        public Status SetData(float data, DateTime time, out bool isChanged)
        {
            isChanged = false;

            if (m_currentData == null)
            {
                m_currentData = new AvgQuaterDay(time);
                m_currentData.SetData(data);
            }
            else
            {
                AvgQuaterDay quater = new AvgQuaterDay(time);

                if (m_currentData.DayIndex == quater.DayIndex && m_currentData.Quater == quater.Quater)
                {
                    m_currentData.SetData(data);
                }
                else
                {
                    // 이미 기록된 시간보다 이전 시간의 데이터는 무시한다.
                    if ((quater.Quater == 0 && m_currentData.Quater == 3) || quater.Quater > m_currentData.Quater)
                    {
                        quater.SetData(data);
                        AvgQuaterDay prevData = m_currentData;
                        TraceLastData(prevData);

                        m_currentData = quater;

                        CheckAlarm(prevData, time, out isChanged);
                    }
                }
            }

            /*if (IsSameTime(time))
            {
                if (m_prevStatus == Status.Level2)
                {
                    if (data >= m_level4)
                    {
                        m_prevStatus = Status.Level4;
                        isChanged = true;
                    }
                }
                else if (m_prevStatus == Status.None)
                {
                    if (data >= m_level4)
                    {
                        m_prevStatus = Status.Level4;
                        isChanged = true;
                    }
                    else if (data >= m_level2)
                    {
                        m_prevStatus = Status.Level2;
                        isChanged = true;
                    }
                }
            }
            else
            {
                if (m_prevStatus == Status.None)
                {
                    if (data >= m_level4)
                    {
                        m_prevStatus = Status.Level4;
                        isChanged = true;
                    }
                    else if (data >= m_level2)
                    {
                        m_prevStatus = Status.Level2;
                        isChanged = true;
                    }
                }
                else if (m_prevStatus == Status.Level2)
                {
                    if (data >= m_level4)
                    {
                        m_prevStatus = Status.Level4;
                        isChanged = true;
                    }
                    else if (data < m_level2)
                    {
                        m_prevStatus = Status.None;
                        isChanged = true;
                    }
                }
                else
                {
                    if (data < m_level4)
                    {
                        if (data >= m_level2)
                        {
                            m_prevStatus = Status.Level2;
                            isChanged = true;
                        }
                        else
                        {
                            m_prevStatus = Status.None;
                            isChanged = true;
                        }
                    }
                }
            }*/

            /*if (isChanged)
                m_prevAlarmTime = time;*/

            return m_prevStatus;
        }

        private void TraceLastData(AvgQuaterDay data)
        {
            System.Diagnostics.Trace.WriteLine("O3, " + data.DayIndex + "_" + data.Quater + " : " + data.DataAverage);
        }

        private void CheckAlarm(AvgQuaterDay data, DateTime time, out bool isChanged)
        {
            isChanged = false;
            float avg = data.DataAverage;

            if (m_prevAlarmTime == null || (DateTime)m_prevAlarmTime < time)
            {
                if (m_prevStatus == Status.None)
                {
                    if (avg >= m_level4)
                    {
                        m_prevStatus = Status.Level4;
                        m_prevAlarmTime = time;
                        isChanged = true;
                    }
                    else if (avg >= m_level2)
                    {
                        m_prevStatus = Status.Level2;
                        m_prevAlarmTime = time;
                        isChanged = true;
                    }
                }
                else if (m_prevStatus == Status.Level2)
                {
                    if (avg >= m_level4)
                    {
                        m_prevStatus = Status.Level4;
                        m_prevAlarmTime = time;
                        isChanged = true;
                    }
                    else if (avg < m_level2)
                    {
                        m_prevStatus = Status.None;
                        m_prevAlarmTime = time;
                        isChanged = true;
                    }
                }
                else
                {
                    if (avg < m_level4)
                    {
                        if (avg >= m_level2)
                        {
                            m_prevStatus = Status.Level2;
                            m_prevAlarmTime = time;
                            isChanged = true;
                        }
                        else
                        {
                            m_prevStatus = Status.None;
                            m_prevAlarmTime = time;
                            isChanged = true;
                        }
                    }
                }

                if (isChanged)
                {
                    if (m_prevStatus == Status.None)
                        m_alarmData = null;
                    else
                        m_alarmData = data;
                }
            }
        }

        private void SetData(float data, DateTime time)
        {
            if (m_currentData == null)
            {
                m_currentData = new AvgQuaterDay(time);
                m_currentData.SetData(data);
            }
            else
            {
                AvgQuaterDay quater = new AvgQuaterDay(time);

                if (m_currentData.DayIndex == quater.DayIndex && m_currentData.Quater == quater.Quater)
                {
                    m_currentData.SetData(data);
                }
                else
                {
                    quater.SetData(data);
                    AvgQuaterDay prevData = m_currentData;
                    m_currentData = quater;

                    //CheckAlarm(prevData, isChanged)
                }
            }
        }

        // 6시간 단위로 시간을 쪼개었을때, 같은 시간대인가?
        /*private bool IsSameTime(DateTime time)
        {
            if (m_prevAlarmTime.Year == time.Year &&
                m_prevAlarmTime.Month == time.Month &&
                m_prevAlarmTime.Day == time.Day)
            {
                int nPrevHour = m_prevAlarmTime.Hour / 6;
                int nCurrentHour = time.Hour / 6;

                if (nPrevHour == nCurrentHour)
                    return true;
            }

            return false;
        }*/
    }
}
