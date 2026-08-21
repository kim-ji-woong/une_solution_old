using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace CCTVAlarmWatcher
{
    public class AlarmWatcher
    {
        private IAlarmOwner m_owner = null;
        private bool m_runThread = false;
        private UTF8Encoding m_encoding = new UTF8Encoding(true);

        private string m_strTargetFile = "CCTVAlarm.txt";
        private string m_strCurrentFile = "";
        private DateTime m_dtCurrent;
        private long m_nPrevFileSize = 0;

        // AlarmWatcher가 꺼져있는 동안에 발생했던 알람은 무시한다.
        private DateTime m_dtLastEvent = DateTime.Now;
        private Dictionary<string, string> m_dicReadEvent = new Dictionary<string, string>();

        public AlarmWatcher(IAlarmOwner owner)
        {
            m_owner = owner;
        }

        public void Run()
        {
            Thread t = new Thread(new ThreadStart(Watch));
            t.Start();
        }

        public void Stop()
        {
            m_runThread = false;
        }

        private void Watch()
        {
            m_runThread = true;

            while (m_runThread)
            {
                CheckAlarm();
                Thread.Sleep(1000);
            }
        }

        private void CheckAlarm()
        {
            if (m_owner == null)
                return;

            DateTime dtNow = DateTime.Now;

            if (m_strCurrentFile.Length == 0)
            {
                SetCurrentFile(dtNow);
                m_nPrevFileSize = 0;
            }
            else if (IsSameDay(dtNow, m_dtCurrent) == false)
            {
                SetCurrentFile(dtNow);
                m_nPrevFileSize = 0;
            }

            if (System.IO.File.Exists(m_strCurrentFile))
            {
                using (FileStream fs = File.Open(m_strCurrentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long nFileSize = fs.Seek(0, SeekOrigin.End);
                    fs.Seek(0, SeekOrigin.Begin);

                    if (nFileSize > 0 && m_nPrevFileSize < nFileSize)
                    {
                        byte[] bytes = new byte[nFileSize];

                        if (fs.Read(bytes, 0, bytes.Length) > 0)
                        {
                            string strLines = m_encoding.GetString(bytes);

                            if (strLines.Length > 0)
                            {
                                string[] lines = strLines.Split('\n');

                                foreach (string strLine in lines)
                                {
                                    CheckAlarm(strLine.Trim());
                                }
                            }
                        }
                    }

                    m_nPrevFileSize = nFileSize;
                }

                /*System.IO.StreamReader reader = new System.IO.StreamReader(strFileName, Encoding.UTF8);

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0 || strLine.StartsWith("#"))
                        continue;

                    string[] tokens = strLine.Split(' ');

                    if (tokens.Count() != 3)
                        continue;

                    int nAlarmType, fromCCTV, nID;

                    if (int.TryParse(tokens[0].Trim(), out nAlarmType) == false)
                        continue;
                    if (int.TryParse(tokens[1].Trim(), out fromCCTV) == false)
                        continue;
                    if (int.TryParse(tokens[2].Trim(), out nID) == false)
                        continue;

                    if (nAlarmType < 0)
                    {
                        if (fromCCTV == 1)
                            m_owner.OnAlarmOff(nID);
                        else
                            m_owner.OnAlarmOff2(nID);
                    }
                    else
                    {
                        if (fromCCTV == 1)
                            m_owner.OnAlarmOn((AlarmType)nAlarmType, nID);
                        else
                            m_owner.OnAlarmOn2((AlarmType)nAlarmType, nID);
                    }

                    break;
                }

                reader.Close();*/
            }
        }

        private void SetCurrentFile(DateTime dtNow)
        {
            int nIndex1 = m_strTargetFile.LastIndexOf('\\');
            int nIndex2 = m_strTargetFile.LastIndexOf('.');

            if (nIndex2 < 0 || nIndex2 < nIndex1)
                m_strCurrentFile = m_strTargetFile + GetDateTimeFileString(dtNow);
            else
            {
                string str1 = m_strTargetFile.Substring(0, nIndex2);
                string str2 = m_strTargetFile.Substring(nIndex2);

                m_strCurrentFile = str1 + GetDateTimeFileString(dtNow) + str2;
            }

            m_dtCurrent = dtNow;
        }

        private string GetDateTimeFileString(DateTime timeStamp)
        {
            return string.Format("_{0}{1:00}{2:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day);
        }

        private bool IsSameDay(DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
        }

        private void CheckAlarm(string strLine)
        {
            if (strLine.Length == 0 || strLine.StartsWith("#"))
                return;

            string[] tokens = strLine.Split(' ');

            if (tokens.Count() != 4)
                return;

            DateTime timeStamp;
            int nAlarmType, fromCCTV, nID;

            if (ReadDateTime(tokens[0].Trim(), out timeStamp) == false)
                return;
            if (int.TryParse(tokens[1].Trim(), out nAlarmType) == false)
                return;
            if (int.TryParse(tokens[2].Trim(), out fromCCTV) == false)
                return;
            if (int.TryParse(tokens[3].Trim(), out nID) == false)
                return;

            // 이미 읽었던 알람을 다시 발생시키지 않도록 한다.
            if (timeStamp < m_dtLastEvent)
                return;
            else if (timeStamp == m_dtLastEvent)
            {
                if (m_dicReadEvent.ContainsKey(strLine))
                    return;
            }
            else
            {
                m_dtLastEvent = timeStamp;
                m_dicReadEvent.Clear();
            }

            if (nAlarmType < 0)
            {
                if (fromCCTV == 1)
                    m_owner.OnAlarmOff(nID, timeStamp);
                else
                    m_owner.OnAlarmOff2(nID, timeStamp);
            }
            else
            {
                if (fromCCTV == 1)
                    m_owner.OnAlarmOn((AlarmType)nAlarmType, nID, timeStamp);
                else
                    m_owner.OnAlarmOn2((AlarmType)nAlarmType, nID, timeStamp);
            }

            m_dicReadEvent[strLine] = strLine;
        }

        private bool ReadDateTime(string str, out DateTime timeStamp)
        {
            timeStamp = new DateTime();

            if (str.Length < 19)
                return false;

            string strYear = str.Substring(0, 4);
            string strMonth = str.Substring(5, 2);
            string strDay = str.Substring(8, 2);
            string strHour = str.Substring(11, 2);
            string strMin = str.Substring(14, 2);
            string strSec = str.Substring(17, 2);

            int year, month, day, hour, min, sec;

            if (int.TryParse(strYear, out year) == false || int.TryParse(strMonth, out month) == false || int.TryParse(strDay, out day) == false)
                return false;
            if (int.TryParse(strHour, out hour) == false || int.TryParse(strMin, out min) == false || int.TryParse(strSec, out sec) == false)
                return false;

            timeStamp = new DateTime(year, month, day, hour, min, sec);
            return true;
        }
    }
}
