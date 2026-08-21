using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SOPWebClient
{
    public class Logger
    {
        private static Logger m_instance = null;

        private string m_strLogFolder = "";
        private double m_dLogLifeDays = 30;
        private string m_strLogTag = "";

        private static int m_nPrevYear = 0, m_nPrevMonth = 0, m_nPrevDay = 0;

        private StreamWriter m_writer = null;

        public Logger(string strLogFolder, string strFileName, int nLogLifeDays)
        {
            m_strLogFolder = strLogFolder;
            m_strLogTag = strFileName;
            m_dLogLifeDays = nLogLifeDays;
        }

        public void Write(string strLog)
        {
            RemoveOldLogs();

            if (m_strLogFolder.Length == 0)
                return;

            if (!Directory.Exists(m_strLogFolder))
                Directory.CreateDirectory(m_strLogFolder);

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

        public void RemoveOldLogs()
        {
            if (!Directory.Exists(m_strLogFolder))
                return;

            DateTime dtNow = DateTime.Now;

            if (dtNow.Year != m_nPrevYear && dtNow.Month != m_nPrevMonth && dtNow.Day != m_nPrevDay)
            {
                m_nPrevYear = dtNow.Year;
                m_nPrevMonth = dtNow.Month;
                m_nPrevDay = dtNow.Day;
            }
            else
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

        public static string GetByteString(byte[] bytes, int nIndex, int len)
        {
            string strBytes = "";

            if (len <= 0)
                return strBytes;

            strBytes = string.Format("{0:X2}", (int)bytes[nIndex]);

            for (int i=nIndex + 1;i<nIndex + len;i++)
            {
                strBytes += string.Format(" {0:X2}", (int)bytes[i]);
            }

            return strBytes;
        }
    }
}
