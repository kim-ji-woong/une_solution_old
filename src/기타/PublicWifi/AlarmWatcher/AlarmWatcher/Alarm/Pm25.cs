using System;
using System.Collections.Generic;
using System.Linq;

namespace AlarmWatcher.Alarm
{
    public class Pm25
    {
        public enum Status { None = 0, Level2, Level4 };

        private float m_hourAvgLevel2 = 75;
        private float m_hourAvgLevel4 = 150;
        private int m_nHourLevel2 = 4;
        private int m_nHourLevel4 = 4;
        private float m_hourAvgLevel2Clear = 50;
        private float m_hourAvgLevel4Clear = 90;
        private int m_nMaxHour = 4;

        private Status m_prevStatus = Status.None;
        private DateTime? m_prevAlarmTime = null;
        private List<AvgHour> m_datas = new List<AvgHour>();
        private AvgHour m_alarmData = null;

        public float? AlarmData
        {
            get
            {
                if (m_alarmData == null)
                    return null;

                return m_alarmData.DataAverage;
            }
        }

        public Pm25(string strOptionLevel2, string strOptionLevel4, string strOptionLevel2Clear, string strOptionLevel4Clear)
        {
            float hourAvgLevel2, hourAvgLevel4, hourAvgLevel2Clear, hourAvgLevel4Clear;
            int hourLevel2, hourLevel4;

            if (ParseOption(strOptionLevel2, out hourAvgLevel2, out hourLevel2) &&
                ParseOption(strOptionLevel4, out hourAvgLevel4, out hourLevel4) &&
                float.TryParse(strOptionLevel2Clear, out hourAvgLevel2Clear) &&
                float.TryParse(strOptionLevel4Clear, out hourAvgLevel4Clear))
            {
                m_hourAvgLevel2 = hourAvgLevel2;
                m_hourAvgLevel4 = hourAvgLevel4;
                m_nHourLevel2 = hourLevel2;
                m_nHourLevel4 = hourLevel4;
                m_hourAvgLevel2Clear = hourAvgLevel2Clear;
                m_hourAvgLevel4Clear = hourAvgLevel4Clear;

                if (m_nHourLevel2 > m_nHourLevel4)
                    m_nMaxHour = m_nHourLevel2;
                else
                    m_nMaxHour = m_nHourLevel4;
            }
        }

        public void SetStatus(Status status, DateTime? time)
        {
            m_prevStatus = status;
            m_prevAlarmTime = time;
        }

        private bool ParseOption(string strOption, out float hourAvgLevel, out int hourLevel)
        {
            hourAvgLevel = 0;
            hourLevel = 0;

            string[] tokens = strOption.Split('_');

            if (tokens.Count() != 2)
                return false;

            if (float.TryParse(tokens[0].Trim(), out hourAvgLevel) && int.TryParse(tokens[1].Trim(), out hourLevel))
                return true;

            return false;
        }

        private AvgHour GetHourData(int hour)
        {
            foreach (AvgHour hourData in m_datas)
            {
                if (hourData.Hour == hour)
                    return hourData;
            }

            return null;
        }

        public Status SetData(float data, DateTime time, out bool isChanged)
        {
            isChanged = false;

            AvgHour avgHour = null;
            int nDataCount = m_datas.Count;

            if (nDataCount == 0)
            {
                avgHour = new AvgHour(time.Hour);
                m_datas.Add(avgHour);
                nDataCount++;
            }
            else
            {
                avgHour = GetHourData(time.Hour);
                //avgHour = m_datas[nDataCount - 1];

                if (avgHour == null)
                //if (avgHour.Hour != time.Hour)
                {
                    avgHour = new AvgHour(time.Hour);
                    m_datas.Add(avgHour);

                    TraceLastData();

                    if (nDataCount > m_nMaxHour)
                        m_datas.RemoveAt(0);
                    else
                        nDataCount++;
                }
            }

            avgHour.SetData(data);

            if (nDataCount > m_nMaxHour)
            {
                if (m_prevAlarmTime == null || (DateTime)m_prevAlarmTime < time)
                {
                    AvgHour alarmData;
                    float lastAvgData;
                    Status status = CheckAlarmStatus(nDataCount, out lastAvgData, out alarmData);

                    if (m_prevStatus != Status.None)
                    {
                        if (status == Status.Level4)
                        {
                            isChanged = m_prevStatus != status;
                            m_prevStatus = status;
                            m_prevAlarmTime = time;

                            if (isChanged)
                                m_alarmData = alarmData;
                        }
                        else if (m_prevStatus == Status.Level4)
                        {
                            if (lastAvgData < m_hourAvgLevel2Clear)
                            {
                                m_prevStatus = Status.None;
                                m_prevAlarmTime = time;
                                isChanged = true;
                                m_alarmData = null;
                            }
                            else if (lastAvgData < m_hourAvgLevel4Clear)
                            {
                                m_prevStatus = Status.Level2;
                                m_prevAlarmTime = time;
                                isChanged = true;
                                m_alarmData = alarmData;
                            }
                        }
                        else if (m_prevStatus == Status.Level2)
                        {
                            if (lastAvgData < m_hourAvgLevel2Clear)
                            {
                                m_prevStatus = Status.None;
                                m_prevAlarmTime = time;
                                isChanged = true;
                                m_alarmData = null;
                            }
                        }
                        else
                        {
                            isChanged = m_prevStatus != status;
                            m_prevStatus = status;
                            m_prevAlarmTime = time;

                            if (isChanged)
                                m_alarmData = alarmData;
                        }
                    }
                    else
                    {
                        isChanged = m_prevStatus != status;
                        m_prevStatus = status;
                        m_prevAlarmTime = time;

                        if (isChanged)
                            m_alarmData = alarmData;
                    }
                }
            }

            return m_prevStatus;
        }

        private void TraceLastData()
        {
            if (m_datas.Count >= 2)
            {
                AvgHour last = m_datas[m_datas.Count - 2];
                System.Diagnostics.Trace.WriteLine("Pm2.5, " + last.Hour + " : " + last.DataAverage);
            }
        }

        private Status CheckAlarmStatus(int nDataCount, out float lastAvgData, out AvgHour alarmData)
        {
            alarmData = null;
            lastAvgData = 0;

            bool? level2 = null;
            bool? level4 = null;
            int level2Count = 0, level4Count = 0;

            int nBeginIndex = nDataCount - 2;

            for (int i = nBeginIndex; i >= 0; i--)
            {
                AvgHour hourData = m_datas[i];
                float fAvgData = hourData.DataAverage;

                if (i == nDataCount - 2)
                    lastAvgData = fAvgData;

                if (level2Count < m_nHourLevel2 && (level2 == null || (bool)level2 == true))
                {
                    if (fAvgData >= m_hourAvgLevel2)
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

                if (level4Count < m_nHourLevel4 && (level4 == null || (bool)level4 == true))
                {
                    if (fAvgData >= m_hourAvgLevel4)
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
            }

            if (level4 != null && (bool)level4 == true)
            {
                alarmData = m_datas[nBeginIndex];
                return Status.Level4;
            }

            if (level2 != null && (bool)level2 == true)
            {
                alarmData = m_datas[nBeginIndex];
                return Status.Level2;
            }

            return Status.None;
        }
    }
}
