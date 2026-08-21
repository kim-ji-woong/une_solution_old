using System;
using System.Text;
using System.IO;

namespace FireSensorServer.Network
{
    public class Logger
    {
        private string m_strLogFolder = ".";
        private double m_dLogLifeDays = 30;
        private string m_strLogTag = "";

        private static int m_nPrevYear = 0, m_nPrevMonth = 0, m_nPrevDay = 0;

        private StreamWriter m_writer = null;

        public Logger(string strLogFolder, string strLogTag)
        {
            m_strLogFolder = strLogFolder;
            m_strLogTag = strLogTag;

            CheckLogFolder();
        }

        private void CheckLogFolder()
        {
            string[] tokens = m_strLogFolder.Split(new char[]{ '\\', '/' });
            int nTokenCount = tokens.Length;

            string strFolder = "";

            try
            {
                for (int i = 0; i < nTokenCount; i++)
                {
                    if (strFolder.Length == 0)
                        strFolder = tokens[i].Trim();
                    else
                        strFolder += "\\" + tokens[i].Trim();

                    if (strFolder.EndsWith(":"))
                        continue;

                    if (!Directory.Exists(strFolder))
                    {
                        Directory.CreateDirectory(strFolder);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("CheckLogFolder Fail : " + e.Message);
                m_strLogFolder = "";
            }
        }

        public void Write(string strLog)
        {
            if (m_strLogFolder.Length == 0)
                return;

            DateTime dtNow = DateTime.Now;

            string strFilePath = m_strLogFolder + string.Format("\\{3}_{0}{1:00}{2:00}.log", dtNow.Year, dtNow.Month, dtNow.Day, m_strLogTag);
            StreamWriter writer = m_writer;

            try
            {
                if (!File.Exists(strFilePath))
                {
                    if (writer != null)
                        writer.Close();

                    writer = new StreamWriter(strFilePath, false, Encoding.UTF8);
                }
                else if (writer == null)
                {
                    writer = new StreamWriter(strFilePath, true, Encoding.UTF8);
                }

                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                writer.WriteLine("[" + strTime + "] " + strLog);
                writer.Flush();
            }
            catch (Exception)
            {
                if (writer != null)
                {
                    writer.Close();
                }

                m_writer = null;
                return;
            }

            m_writer = writer;
            CheckOldLogFiles();
        }

        public void Close()
        {
            StreamWriter writer = m_writer;

            try
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }

            m_writer = null;
        }

        private void CheckOldLogFiles()
        {
            DateTime dtNow = DateTime.Now;

            if (dtNow.Year != m_nPrevYear || dtNow.Month != m_nPrevMonth || dtNow.Day != m_nPrevDay)
            {
                m_nPrevYear = dtNow.Year;
                m_nPrevMonth = dtNow.Month;
                m_nPrevDay = dtNow.Day;

                RemoveOldLogs(dtNow);
            }
        }

        private void RemoveOldLogs(DateTime dtNow)
        {
            if (!Directory.Exists(m_strLogFolder))
                return;

            DateTime dtLimit = dtNow.AddDays(-m_dLogLifeDays);
            string strDate = string.Format("{0}{1:00}{2:00}", dtLimit.Year, dtLimit.Month, dtLimit.Day);

            foreach (string strFile in Directory.GetFiles(m_strLogFolder, "*.log"))
            {
                int nIndex = strFile.LastIndexOf('_');

                if (nIndex < 0)
                    continue;

                string strFileDate = strFile.Substring(nIndex + 1, 8);

                if (strFileDate.CompareTo(strDate) < 0)
                    File.Delete(strFile);
            }
        }
    }
}
