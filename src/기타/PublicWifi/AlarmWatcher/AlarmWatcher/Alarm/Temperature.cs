using System;
using System.Collections.Generic;
using System.Linq;

namespace AlarmWatcher.Alarm
{
    public class Temperature
    {
        public enum Status { None = 0, ColdLevel2, HeatLevel2, ColdLevel4, HeatLevel4 };

        private const int ColdBeginMonth = 10;
        private const int ColdEndMonth = 4;

        private float m_yesterdayLevel2 = 10;
        private float m_minimumTempLevel2 = 3;
        private float m_continuosMinimumTempLevel2 = -12;
        private int m_continuousDayLevel2 = 2;
        private float m_maximumTempLevel2 = 35;
        private int m_continuousHeatDayLevel2 = 2;
        private float m_yesterdayLevel4 = 15;
        private float m_minimumTempLevel4 = 3;
        private float m_continuosMinimumTempLevel4 = -15;
        private int m_continuousDayLevel4 = 2;
        private float m_maximumTempLevel4 = 37;
        private int m_continuousHeatDayLevel4 = 2;

        private Status m_prevStatus = Status.None;
        private float? m_fAlarmData = null;
        private DateTime? m_prevAlarmTime = null;

        // 날짜별 최저온도
        private Dictionary<int, float> m_dicMinimumTemp = new Dictionary<int, float>();
        // 날짜별 최고온도
        private Dictionary<int, float> m_dicMaximumTemp = new Dictionary<int, float>();
        private int m_nMaxDay = 2;

        public float? AlarmData
        {
            get { return m_fAlarmData; }
        }

        public Temperature(string strOptionCold1Level2, string strOptionCold2Level2, string strOptionHeatLevel2, string strOptionCold1Level4, string strOptionCold2Level4, string strOptionHeatLevel4)
        {
            float yesterdayLevel2, minLevel2, continueMinLevel2, maxLevel2;
            float yesterdayLevel4, minLevel4, continueMinLevel4, maxLevel4;
            int continueDayLevel2, continueHeatDayLevel2, continueDayLevel4, continueHeatDayLevel4;

            if (ParseOption1(strOptionCold1Level2, out yesterdayLevel2, out minLevel2) &&
                ParseOption2(strOptionCold2Level2, out continueMinLevel2, out continueDayLevel2) &&
                ParseOption2(strOptionHeatLevel2, out maxLevel2, out continueHeatDayLevel2) &&
                ParseOption1(strOptionCold1Level4, out yesterdayLevel4, out minLevel4) &&
                ParseOption2(strOptionCold2Level4, out continueMinLevel4, out continueDayLevel4) &&
                ParseOption2(strOptionHeatLevel4, out maxLevel4, out continueHeatDayLevel4))
            {
                m_yesterdayLevel2 = yesterdayLevel2;
                m_minimumTempLevel2 = minLevel2;
                m_continuosMinimumTempLevel2 = continueMinLevel2;
                m_continuousDayLevel2 = continueDayLevel2;
                m_maximumTempLevel2 = maxLevel2;
                m_continuousHeatDayLevel2 = continueHeatDayLevel2;
                m_yesterdayLevel4 = yesterdayLevel4;
                m_minimumTempLevel4 = minLevel4;
                m_continuosMinimumTempLevel4 = continueMinLevel4;
                m_continuousDayLevel4 = continueDayLevel4;
                m_maximumTempLevel4 = maxLevel4;
                m_continuousHeatDayLevel4 = continueHeatDayLevel4;

                if (m_nMaxDay < continueDayLevel2)
                    m_nMaxDay = continueDayLevel2;

                if (m_nMaxDay < continueDayLevel4)
                    m_nMaxDay = continueDayLevel4;

                if (m_nMaxDay < continueHeatDayLevel2)
                    m_nMaxDay = continueHeatDayLevel2;

                if (m_nMaxDay < continueHeatDayLevel4)
                    m_nMaxDay = continueHeatDayLevel4;
            }
        }

        public void SetStatus(Status status, DateTime? time)
        {
            m_prevStatus = status;
            m_prevAlarmTime = time;
        }

        private bool ParseOption1(string strOption, out float yesterday, out float min)
        {
            yesterday = min = 0;

            string[] tokens = strOption.Split('_');

            if (tokens.Count() != 2)
                return false;

            if (float.TryParse(tokens[0].Trim(), out yesterday) && float.TryParse(tokens[1].Trim(), out min))
                return true;

            return false;
        }

        private bool ParseOption2(string strOption, out float min, out int day)
        {
            min = 0;
            day = 0;

            string[] tokens = strOption.Split('_');

            if (tokens.Count() != 2)
                return false;

            if (float.TryParse(tokens[0].Trim(), out min) && int.TryParse(tokens[1].Trim(), out day))
                return true;

            return false;
        }

        public Status SetData(float data, DateTime time, out bool isChanged)
        {
            isChanged = false;
            SetData(data, time);

            if (m_prevAlarmTime == null || (DateTime)m_prevAlarmTime < time)
            {
                if (CheckCold(data, time, out isChanged) == false)
                {
                    bool changed;
                    CheckHeat(data, time, out changed);

                    if (isChanged || changed)
                        isChanged = true;
                }
            }

            return m_prevStatus;
        }

        private bool CheckHeat(float data, DateTime time, out bool isChanged)
        {
            isChanged = false;

            if (CheckHeatLevel4(time))
            {
                isChanged = m_prevStatus != Status.HeatLevel4;
                m_prevStatus = Status.HeatLevel4;
                m_prevAlarmTime = time;
                return true;
            }
            else if (CheckHeatLevel2(time))
            {
                isChanged = m_prevStatus != Status.HeatLevel2;
                m_prevStatus = Status.HeatLevel2;
                m_prevAlarmTime = time;
                return true;
            }

            if (m_prevStatus == Status.HeatLevel2 || m_prevStatus == Status.HeatLevel4)
            {
                m_prevStatus = Status.None;
                m_fAlarmData = null;
                m_prevAlarmTime = time;
                isChanged = true;
            }

            return false;
        }

        private bool CheckHeatLevel2(DateTime time)
        {
            float max;
            int checkCount = 0;
            float? alarmData = null;

            for (int i = 0; i < m_continuousHeatDayLevel2; i++)
            {
                int day = DateTimeToInt(time.AddDays(-i));

                if (m_dicMaximumTemp.TryGetValue(day, out max))
                {
                    if (max >= m_maximumTempLevel2)
                    {
                        if (alarmData == null)
                            alarmData = max;

                        checkCount++;
                    }
                    else
                    {
                        alarmData = null;
                        break;
                    }
                }
                else
                {
                    alarmData = null;
                    break;
                }
            }

            bool result = checkCount >= m_continuousHeatDayLevel2;

            if (result && alarmData != null)
                m_fAlarmData = alarmData;

            return result;
        }

        private bool CheckHeatLevel4(DateTime time)
        {
            float max;
            int checkCount = 0;
            float? alarmData = null;

            for (int i = 0; i < m_continuousHeatDayLevel4; i++)
            {
                int day = DateTimeToInt(time.AddDays(-i));

                if (m_dicMaximumTemp.TryGetValue(day, out max))
                {
                    if (max >= m_maximumTempLevel4)
                    {
                        if (alarmData == null)
                            alarmData = max;

                        checkCount++;
                    }
                    else
                    {
                        alarmData = null;
                        break;
                    }
                }
                else
                {
                    alarmData = null;
                    break;
                }
            }

            bool result = checkCount >= m_continuousHeatDayLevel4;

            if (result && alarmData != null)
                m_fAlarmData = alarmData;

            return result;
        }

        // 한파 측정은 아침최저기온 기준으로 하는데
        // 0시 ~ 9시 사이의 최저기온을 의미한다.
        private bool IsColdTime(DateTime time)
        {
            // 최소 오전 9시는 지나야만 최소기온에 대한 검사가 가능하다.
            if (time.Hour >= 9)
                return true;

            return false;
        }

        private bool CheckCold(float data, DateTime time, out bool isChanged)
        {
            isChanged = false;

            if ((time.Month > ColdEndMonth && time.Month < ColdBeginMonth) || IsColdTime(time) == false)
                return false;

            if (CheckColdLevel4(time))
            {
                isChanged = m_prevStatus != Status.ColdLevel4;
                m_prevStatus = Status.ColdLevel4;
                m_prevAlarmTime = time;
                return true;
            }
            else if (CheckColdLevel2(time))
            {
                isChanged = m_prevStatus != Status.ColdLevel2;
                m_prevStatus = Status.ColdLevel2;
                m_prevAlarmTime = time;
                return true;
            }

            if (m_prevStatus == Status.ColdLevel2 || m_prevStatus == Status.ColdLevel4)
            {
                m_prevStatus = Status.None;
                m_fAlarmData = null;
                m_prevAlarmTime = time;
                isChanged = true;
            }
 
            return false;
        }

        private bool CheckColdLevel2(DateTime time)
        {
            int today = DateTimeToInt(time);
            int yesterday = DateTimeToInt(time.AddDays(-1));

            float todayMin, yesterdayMin;

            if (m_dicMinimumTemp.TryGetValue(today, out todayMin) &&
                m_dicMinimumTemp.TryGetValue(yesterday, out yesterdayMin))
            {
                if (yesterdayMin - todayMin >= m_yesterdayLevel2 && todayMin <= m_minimumTempLevel2)
                {
                    m_fAlarmData = todayMin;
                    return true;
                }
            }

            float min;
            int checkCount = 0;
            float? alarmData = null;

            for (int i=0;i<m_continuousDayLevel2;i++)
            {
                int day = DateTimeToInt(time.AddDays(-i));

                if (m_dicMinimumTemp.TryGetValue(day, out min))
                {
                    if (min <= m_continuosMinimumTempLevel2)
                    {
                        if (alarmData == null)
                            alarmData = min;

                        checkCount++;
                    }
                    else
                    {
                        alarmData = null;
                        break;
                    }
                }
                else
                {
                    alarmData = null;
                    break;
                }
            }

            bool result = checkCount >= m_continuousDayLevel2;

            if (result && alarmData != null)
                m_fAlarmData = alarmData;

            return result;
        }

        private bool CheckColdLevel4(DateTime time)
        {
            int today = DateTimeToInt(time);
            int yesterday = DateTimeToInt(time.AddDays(-1));

            float todayMin, yesterdayMin;

            if (m_dicMinimumTemp.TryGetValue(today, out todayMin) &&
                m_dicMinimumTemp.TryGetValue(yesterday, out yesterdayMin))
            {
                if (yesterdayMin - todayMin >= m_yesterdayLevel4 && todayMin <= m_minimumTempLevel4)
                {
                    m_fAlarmData = todayMin;
                    return true;
                }
            }

            float min;
            int checkCount = 0;
            float? alarmData = null;

            for (int i = 0; i < m_continuousDayLevel4; i++)
            {
                int day = DateTimeToInt(time.AddDays(-i));

                if (m_dicMinimumTemp.TryGetValue(day, out min))
                {
                    if (min <= m_continuosMinimumTempLevel4)
                    {
                        if (alarmData == null)
                            alarmData = min;

                        checkCount++;
                    }
                    else
                    {
                        alarmData = null;
                        break;
                    }
                }
                else
                {
                    alarmData = null;
                    break;
                }
            }

            bool result = checkCount >= m_continuousDayLevel4;

            if (result && alarmData != null)
                m_fAlarmData = alarmData;

            return result;
        }

        private void SetData(float data, DateTime time)
        {
            int day = DateTimeToInt(time);

            if ((time.Month >= ColdBeginMonth || time.Month <= ColdEndMonth) &&
                time.Hour >= 0 && time.Hour < 9)
            {
                float min;

                if (m_dicMinimumTemp.TryGetValue(day, out min))
                {
                    if (min > data)
                    {
                        m_dicMinimumTemp[day] = data;
                        System.Diagnostics.Trace.WriteLine("한파, " + day.ToString() + " : " + data);
                    }
                }
                else
                {
                    m_dicMinimumTemp[day] = data;
                    RemoveOldDatas(m_dicMinimumTemp, time);
                    System.Diagnostics.Trace.WriteLine("한파, " + day.ToString() + " : " + data);
                }
            }

            float max;

            if (m_dicMaximumTemp.TryGetValue(day, out max))
            {
                if (max < data)
                    m_dicMaximumTemp[day] = data;
            }
            else
            {
                m_dicMaximumTemp[day] = data;
                RemoveOldDatas(m_dicMaximumTemp, time);
            }
        }

        private void RemoveOldDatas(Dictionary<int, float> dicDatas, DateTime time)
        {
            DateTime dtToday = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);

            List<int> days = new List<int>();
            days.AddRange(dicDatas.Keys);

            foreach (int day in days)
            {
                DateTime dtDay = new DateTime(day / 10000, (day % 10000) / 100, (day % 100), 0, 0, 0);
                TimeSpan span = dtToday - dtDay;
                int diff = (int)(span.TotalDays + 0.1);

                if (diff > m_nMaxDay)
                    dicDatas.Remove(day);
            }
        }

        private int DateTimeToInt(DateTime time)
        {
            return time.Year * 10000 + time.Month * 100 + time.Day;
        }
    }
}
