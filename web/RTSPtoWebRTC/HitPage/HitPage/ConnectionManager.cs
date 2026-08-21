using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace HitPage
{
    public class ConnectionManager
    {
        private class TimeLog
        {
            private DateTime? m_time = null;
            private bool m_prevConnected = false;
            private int m_nSequenceID = 0;

            public DateTime? Time
            {
                get { return m_time; }
                set { m_time = value; }
            }

            public bool PrevConnected
            {
                get { return m_prevConnected; }
                set { m_prevConnected = value; }
            }

            public int Sequence
            {
                get { return m_nSequenceID; }
                set { m_nSequenceID = value; }
            }
        }

        private string m_strBaseUrl = "";
        private string m_strRunFile = "";
        private string m_strAliveFolder = "";
        private string m_strCCTVListFile = "";

        private DateTime m_dtPrev = new DateTime();
        // Key : CCTV 이름
        // Value : 마지막 접속 시간
        private Dictionary<string, TimeLog> m_dicCCTVList = new Dictionary<string, TimeLog>();
        // Key : URL
        // Value : CCTV 이름
        private Dictionary<string, string> m_dicCCTVFile = new Dictionary<string, string>();
        private Dictionary<string, string> m_dicUnconnected = new Dictionary<string, string>();

        public ConnectionManager()
        {
            m_strBaseUrl = System.Configuration.ConfigurationManager.AppSettings.Get("url");
            m_strCCTVListFile = System.Configuration.ConfigurationManager.AppSettings.Get("cctvList");
            m_strRunFile = System.Configuration.ConfigurationManager.AppSettings.Get("runLog");
            m_strAliveFolder = System.Configuration.ConfigurationManager.AppSettings.Get("aliveLogFolder");
        }

        public void CheckConnection()
        {
            if (ReadRunFile())
            {
                ReadAlive();
            }

            foreach (KeyValuePair<string, string> pair in m_dicUnconnected)
            {
                try
                {
                    WebClient client = new WebClient();
                    client.DownloadData(m_strBaseUrl + pair.Key);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private string GetRTSPurl(string strFilePath)
        {
            string strFile = strFilePath.Replace('-', ':');
            strFile = strFile.Replace('_', '/');

            int nIndex = strFile.LastIndexOf('\\');
            int nDotIndex = strFile.LastIndexOf('.');

            if (nIndex < 0)
                return null;

            string strFileName = "";

            if (nDotIndex > nIndex)
                strFileName = strFile.Substring(nIndex + 1, nDotIndex - nIndex - 1);
            else
                strFileName = strFile.Substring(nIndex + 1);

            nIndex = strFileName.IndexOf('@');

            // ip부터 기억한다.
            if (nIndex > 0)
            {
                strFileName = strFileName.Substring(nIndex + 1);
            }
            else
            {
                int len = "rtsp://".Length;
                strFileName = strFileName.Substring(len);
            }

            return strFileName;
        }

        private void ReadAlive()
        {
            string strCCTVName = "", strFileName = "";
            string[] files = Directory.GetFiles(m_strAliveFolder);

            TimeLog timeLog;
            bool changed = false;

            foreach (string strFilePath in files)
            {
                strFileName = GetRTSPurl(strFilePath);

                if (m_dicCCTVFile.TryGetValue(strFileName, out strCCTVName) == false)
                    continue;

                if (m_dicCCTVList.TryGetValue(strCCTVName, out timeLog) == false)
                    continue;

                try
                {
                    StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);

                    while (reader.EndOfStream == false)
                    {
                        string strLine = reader.ReadLine().Trim();

                        if (strLine.Length == 0)
                            continue;

                        reader.Close();

                        DateTime time = Convert.ToDateTime(strLine);

                        if (timeLog.Time == null || timeLog.Time != time)
                        {
                            timeLog.Time = time;
                            timeLog.Sequence = timeLog.Sequence + 1;
                            changed = true;
                        }

                        break;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }

            if (changed == false)
            {
                foreach (KeyValuePair<string, TimeLog> pair in m_dicCCTVList)
                {
                    pair.Value.Sequence = 0;
                    m_dicUnconnected[pair.Key] = pair.Key;
                }

                return;
            }

            int minSequence = -1, maxSequence = -1;

            foreach (KeyValuePair<string, TimeLog> pair in m_dicCCTVList)
            {
                if (minSequence < 0)
                {
                    minSequence = pair.Value.Sequence;
                    maxSequence = pair.Value.Sequence;
                }
                else
                {
                    if (pair.Value.Sequence < minSequence)
                        minSequence = pair.Value.Sequence;
                    if (pair.Value.Sequence > maxSequence)
                        maxSequence = pair.Value.Sequence;
                }
            }

            List<string> unConnectedList = new List<string>();

            if (maxSequence - minSequence >= 2)
            {
                foreach (KeyValuePair<string, TimeLog> pair in m_dicCCTVList)
                {
                    if (pair.Value.Sequence <= maxSequence - 2)
                    {
                        unConnectedList.Add(pair.Key);
                    }
                }

                foreach (KeyValuePair<string, TimeLog> pair in m_dicCCTVList)
                {
                    if (pair.Value.Sequence == maxSequence - 1)
                    {
                        pair.Value.Sequence = pair.Value.Sequence - 1;
                    }
                    else if (pair.Value.Sequence == maxSequence)
                    {
                        m_dicUnconnected.Remove(pair.Key);
                    }
                }
            }

            foreach (string strUnconnected in unConnectedList)
            {
                m_dicUnconnected[strUnconnected] = strUnconnected;
            }
        }

        private bool ReadRunFile()
        {
            try
            {
                StreamReader reader = new StreamReader(m_strRunFile, Encoding.UTF8);

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        continue;

                    DateTime time = Convert.ToDateTime(strLine);

                    if (m_dtPrev != time)
                    {
                        m_dtPrev = time;
                        List<string> cctvList = GetCCTVList();

                        m_dicCCTVList.Clear();

                        foreach (string strCCTV in cctvList)
                        {
                            m_dicCCTVList[strCCTV] = new TimeLog();
                        }
                    }

                    break;
                }

                reader.Close();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return false;
        }

        private List<string> GetCCTVList()
        {
            List<string> cctvList = new List<string>();
            m_dicCCTVFile.Clear();
            m_dicUnconnected.Clear();

            using (StreamReader file = File.OpenText(m_strCCTVListFile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject obj = (JObject)JToken.ReadFrom(reader);

                    if (obj == null)
                        return cctvList;

                    JToken token = obj.GetValue("streams");

                    if (token == null)
                        return cctvList;

                    JToken item = token.First;

                    while (item != null)
                    {
                        JProperty prop = item.ToObject<JProperty>();
                        cctvList.Add(prop.Name);

                        JObject url = prop.Value.ToObject<JObject>();
                        string strURL = url.GetValue("url").ToString();

                        int nIndex = strURL.IndexOf('@');

                        // ip부터 기억한다.
                        if (nIndex > 0)
                        {
                            strURL = strURL.Substring(nIndex + 1);
                        }
                        else
                        {
                            int len = "rtsp://".Length;
                            strURL = strURL.Substring(len);
                        }

                        m_dicCCTVFile[strURL] = prop.Name;
                        item = item.Next;
                    }
                }
            }

            return cctvList;
        }
    }
}
