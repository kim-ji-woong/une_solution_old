using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSimulator
{
    public class TimePattern
    {
        private bool m_useYear = true;
        private bool m_useMonth = true;
        private bool m_useDay = true;
        private bool m_useHour = true;
        private bool m_useMinute = true;
        private bool m_useSecond = true;
        private DateTime m_dtBegin = new DateTime();

        private int m_nYearLength = 4;
        private string m_strTimeFormat = "{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}";

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
        }

        public string ToTimeString(DateTime time)
        {
            int[] data = new int[6] { 0, 0, 0, 0, 0, 0 };
            int nCount = 0;

            if (m_useYear)
            {
                if (m_nYearLength == 4)
                    data[nCount++] = time.Year;
                else
                    data[nCount++] = time.Year - 2000;
            }

            if (m_useMonth)
                data[nCount++] = time.Month;

            if (m_useDay)
                data[nCount++] = time.Day;

            if (m_useHour)
                data[nCount++] = time.Hour;

            if (m_useMinute)
                data[nCount++] = time.Minute;

            if (m_useSecond)
                data[nCount++] = time.Second;

            if (nCount == 6)
                return string.Format(m_strTimeFormat, data[0], data[1], data[2], data[3], data[4], data[5]);
            else if (nCount == 5)
                return string.Format(m_strTimeFormat, data[0], data[1], data[2], data[3], data[4]);
            else if (nCount == 4)
                return string.Format(m_strTimeFormat, data[0], data[1], data[2], data[3]);
            else if (nCount == 3)
                return string.Format(m_strTimeFormat, data[0], data[1], data[2]);
            else if (nCount == 2)
                return string.Format(m_strTimeFormat, data[0], data[1]);
            else if (nCount == 1)
                return string.Format(m_strTimeFormat, data[0]);

            return "";
        }

        public void ReadPattern(string strTime)
        {
            bool begin = false;
            int numCount = 0, nDelemeterIndex = -1, nNumIndex = -1;

            List<string> delemeters = new List<string>();
            List<int> lengthList = new List<int>();

            for (int i = 0; i < strTime.Length; i++)
            {
                char ch = strTime.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    if (nNumIndex < 0)
                        nNumIndex = i;

                    numCount++;

                    if (begin && nDelemeterIndex > 0)
                    {
                        string strDelemeter = strTime.Substring(nDelemeterIndex, i - nDelemeterIndex);
                        delemeters.Add(strDelemeter);
                    }

                    nDelemeterIndex = -1;
                    begin = true;
                }
                else
                {
                    if (nNumIndex >= 0)
                    {
                        lengthList.Add(i - nNumIndex);
                    }

                    nNumIndex = -1;

                    if (nDelemeterIndex < 0)
                        nDelemeterIndex = i;
                }
            }

            if (nNumIndex >= 0)
                lengthList.Add(strTime.Length - nNumIndex);

            int nLengthCount = lengthList.Count();

            if (delemeters.Count() + 1 != nLengthCount)
            {
                System.Diagnostics.Trace.WriteLine("TimePattern Error : " + strTime);
                return;
            }

            m_strTimeFormat = "";
            int nParamIndex = 0;

            if (numCount == 14)
            {
                for (int i = 0; i < nLengthCount; i++)
                {
                    int len = lengthList[i];

                    for (int j=0;j<len;)
                    {
                        if (m_strTimeFormat.Length == 0)
                            m_strTimeFormat = "{" + nParamIndex.ToString() + ":0000}";
                        else
                            m_strTimeFormat += "{" + nParamIndex.ToString() + ":00}";

                        nParamIndex++;

                        if (i == 0 && j == 0)
                            j += 4;
                        else
                            j += 2;
                    }

                    if (i < nLengthCount - 1)
                    {
                        m_strTimeFormat += delemeters[i];
                    }
                }
            }
            else if (numCount == 12)
            {
                if (nLengthCount == 5)
                    ReadOtherPattern(delemeters, lengthList);
                else
                {
                    m_nYearLength = 2;

                    for (int i = 0; i < nLengthCount; i++)
                    {
                        int len = lengthList[i];

                        for (int j = 0; j < len;)
                        {
                            if (m_strTimeFormat.Length == 0)
                                m_strTimeFormat = "{" + nParamIndex.ToString() + ":00}";
                            else
                                m_strTimeFormat += "{" + nParamIndex.ToString() + ":00}";

                            nParamIndex++;
                            j += 2;
                        }

                        if (i < nLengthCount - 1)
                        {
                            m_strTimeFormat += delemeters[i];
                        }
                    }
                }
            }
            else
            {
                ReadOtherPattern(delemeters, lengthList);
            }

            int nBeginIndex = 0;
            int nYear = DateTime.Now.Year, nMonth = 1, nDay = 1, nHour = 0, nMinute = 0, nSecond = 0;

            if (m_useYear)
                nYear = GetDate(strTime, m_nYearLength, ref nBeginIndex);

            if (m_useMonth)
                nMonth = GetDate(strTime, 2, ref nBeginIndex);

            if (m_useDay)
                nDay = GetDate(strTime, 2, ref nBeginIndex);

            if (m_useHour)
                nHour = GetDate(strTime, 2, ref nBeginIndex);

            if (m_useMinute)
                nMinute = GetDate(strTime, 2, ref nBeginIndex);

            if (m_useSecond)
                nSecond = GetDate(strTime, 2, ref nBeginIndex);

            m_dtBegin = new DateTime(nYear, nMonth, nDay, nHour, nMinute, nSecond);
        }

        private int GetDate(string strTime, int nDateLength, ref int nBeginIndex)
        {
            bool begin = false;

            for (int i=nBeginIndex;i<strTime.Length;i++)
            {
                char ch = strTime.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    if (begin == false)
                        nBeginIndex = i;

                    if (i - nBeginIndex + 1 == nDateLength)
                    {
                        string strDate = strTime.Substring(nBeginIndex, i - nBeginIndex + 1);

                        int date;

                        if (int.TryParse(strDate, out date))
                        {
                            nBeginIndex = i + 1;
                            return date;
                        }
                    }

                    begin = true;
                }
                else
                {
                    if (i - nBeginIndex == nDateLength)
                    {
                        string strDate = strTime.Substring(nBeginIndex, i - nBeginIndex);

                        int date;

                        if (int.TryParse(strDate, out date))
                        {
                            nBeginIndex = i;
                            return date;
                        }
                    }
                    else if (begin)
                        break;
                }
            }

            return 0;
        }

        private void ReadOtherPattern(List<string> delemeters, List<int> lengthList)
        {
            int nParamIndex = 0;
            int nLengthCount = lengthList.Count();

            if (lengthList[0] == 4)
            {
                // Year 부터
                for (int i = 0; i < nLengthCount; i++)
                {
                    int len = lengthList[i];

                    for (int j = 0; j < len;)
                    {
                        if (m_strTimeFormat.Length == 0)
                            m_strTimeFormat = "{" + nParamIndex.ToString() + ":0000}";
                        else
                            m_strTimeFormat += "{" + nParamIndex.ToString() + ":00}";

                        nParamIndex++;

                        if (i == 0 && j == 0)
                            j += 4;
                        else
                            j += 2;
                    }

                    if (i < nLengthCount - 1)
                    {
                        m_strTimeFormat += delemeters[i];
                    }
                }

                if (nParamIndex == 1)
                {
                    m_useMonth = m_useDay = m_useHour = m_useMinute = m_useSecond = false;
                }
                else if (nParamIndex == 2)
                {
                    m_useDay = m_useHour = m_useMinute = m_useSecond = false;
                }
                else if (nParamIndex == 3)
                {
                    m_useHour = m_useMinute = m_useSecond = false;
                }
                else if (nParamIndex == 4)
                {
                    m_useMinute = m_useSecond = false;
                }
                else if (nParamIndex == 5)
                {
                    m_useSecond = false;
                }
            }
            else
            {
                // Second 부터
                for (int i = 0; i < nLengthCount; i++)
                {
                    int len = lengthList[i];

                    for (int j = 0; j < len;)
                    {
                        if (m_strTimeFormat.Length == 0)
                            m_strTimeFormat = "{" + nParamIndex.ToString() + ":00}";
                        else
                            m_strTimeFormat += "{" + nParamIndex.ToString() + ":00}";

                        nParamIndex++;
                        j += 2;
                    }

                    if (i < nLengthCount - 1)
                    {
                        m_strTimeFormat += delemeters[i];
                    }
                }

                if (nParamIndex == 1)
                {
                    m_useYear = m_useMonth = m_useDay = m_useHour = m_useMinute = false;
                }
                else if (nParamIndex == 2)
                {
                    m_useYear = m_useMonth = m_useDay = m_useHour = false;
                }
                else if (nParamIndex == 3)
                {
                    m_useYear = m_useMonth = m_useDay = false;
                }
                else if (nParamIndex == 4)
                {
                    m_useYear = m_useMonth = false;
                }
                else if (nParamIndex == 5)
                {
                    m_useYear = false;
                }
            }
        }

        public void ReadLogBytes(string strLine, string strStartTime, Dictionary<string, List<byte[]>> dicTimeLogs)
        {
            if (strStartTime.Length == 0)
                return;

            int nBeginIndex = 0, nEndIndex = -1, nPosition = 0;
            int nLineLength = strLine.Length;

            for (int i=0;i<nLineLength;i++)
            {
                char ch = strLine.ElementAt(i);
                int nCheck = CheckIndex(ch, strStartTime, ref nPosition);

                if (nCheck == 1)
                {
                    if (nPosition == 1)
                        nBeginIndex = i;
                }
                else if (nCheck == 2)
                {
                    nEndIndex = i;
                    break;
                }
            }

            if (nEndIndex < 0)
                return;

            string strTime = strLine.Substring(nBeginIndex, nEndIndex - nBeginIndex + 1);

            if (string.Compare(strTime, strStartTime) >= 0)
            {
                string str = strLine.Substring(nEndIndex + 1);
                byte[] bytes = GetBytes(str);

                if (bytes != null)
                {
                    List<byte[]> byteList = null;

                    if (dicTimeLogs.TryGetValue(strTime, out byteList) == false)
                    {
                        byteList = new List<byte[]>();
                        dicTimeLogs[strTime] = byteList;
                    }

                    byteList.Add(bytes);
                }
            }
        }

        private byte[] GetBytes(string strLine)
        {
            List<byte> byteList = new List<byte>();

            string[] tokens = strLine.Split(' ');
            int nTokenCount = tokens.Count();

            byte b;
            bool begin = false;

            for (int i=nTokenCount-1;i>=0;i--)
            {
                string str = tokens[i].Trim();

                if (str.Length == 2 && byte.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out b))
                {
                    byteList.Add(b);
                    begin = true;
                }
                else if (begin)
                    break;
            }

            if (byteList.Count == 0)
                return null;

            byte[] bytes = new byte[byteList.Count];

            for (int i=byteList.Count-1,j=0;i>=0;i--,j++)
            {
                bytes[j] = byteList[i];
            }

            return bytes;
        }

        // Return 값 : 2(완료), 1(부분 완료), 0(실패)
        private int CheckIndex(char ch, string strTimeSample, ref int nPosition)
        {
            char chSample = strTimeSample.ElementAt(nPosition);

            if (chSample >= '0' && chSample <= '9')
            {
                if (ch >= '0' && ch <= '9')
                {
                    nPosition++;

                    if (nPosition == strTimeSample.Length)
                        return 2;
                    else
                        return 1;
                }
                else
                {
                    nPosition = 0;
                    return 0;
                }
            }
            else if (chSample == ch)
            {
                nPosition++;
                return 1;
            }

            nPosition = 0;
            return 0;
        }
    }
}
