using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Timers;

using log4net;
using log4net.Appender;
using log4net.Config;

namespace UnE.Log
{
    public class LogFileCleanupTask
    {
        private Action m_DailyTask = null;
        public LogFileCleanupTask()
        {

        }

        private void CleanUpDailyTask(object sender, ElapsedEventArgs e)
        {
            if (m_DailyTask != null)
            {
                m_DailyTask.Invoke();
            }
        }

        private System.Timers.Timer m_dailyTimer = null;

        public void BeginDailyTask(Action mCall)
        {
            m_DailyTask = mCall;

            const double nTaskTime = 24 * 60 * 60 * 1000; // milliseconds to one hour

            if (m_dailyTimer != null)
            {
                if (m_dailyTimer.Enabled == true)
                {
                    m_dailyTimer.Stop();
                    m_dailyTimer.Enabled = false;
                }
            }

            m_dailyTimer = new System.Timers.Timer(nTaskTime);
            m_dailyTimer.Elapsed += new ElapsedEventHandler(CleanUpDailyTask);
            m_dailyTimer.Enabled = true;
            m_dailyTimer.Start();
        }

        public void CleanUp()
        {
            string directory = string.Empty;
            string filePrefix = string.Empty;

            var repo = LogManager.GetAllRepositories().FirstOrDefault(); ;
            if (repo == null)
                throw new NotSupportedException("Log4Net has not been configured yet.");

            var app = repo.GetAppenders().Where(x => x.GetType() == typeof(RollingFileAppender)).FirstOrDefault();
            if (app != null)
            {
                var appender = app as RollingFileAppender;

                int nMaxCount = appender.MaxSizeRollBackups;
                if (nMaxCount < 1)
                    nMaxCount = 30;

                DateTime date = DateTime.Now.AddDays(-nMaxCount);

                directory = Path.GetDirectoryName(appender.File);
                filePrefix = Path.GetFileName(appender.File);

                CleanUp(directory, filePrefix, date);
            }
        }

        public void CleanUp(string logDirectory, string logPrefix, DateTime date)
        {
            if (string.IsNullOrEmpty(logDirectory))
                throw new ArgumentException("logDirectory is missing");

            if (string.IsNullOrEmpty(logPrefix))
                throw new ArgumentException("logPrefix is missing");

            var dirInfo = new DirectoryInfo(logDirectory);
            if (!dirInfo.Exists)
                return;

            string szFormat = string.Format("{0}*", logPrefix);
            FileInfo[] fileInfos = dirInfo.GetFiles(szFormat);
            if (fileInfos.Length == 0)
                return;

            foreach (FileInfo info in fileInfos)
            {
                string szFileName = info.FullName;
                string strFile = Path.GetFileName(szFileName).ToLower();

                // 생성일자가 지정일 이전인 경우 삭제한다.
                if (info.LastWriteTime < date)
                {
                    info.Delete();
                }

                int len = logPrefix.Length + 1;

                int nIndex = strFile.IndexOf(logPrefix.ToLower());
                if (nIndex >= 0)
                {
                    int nSubIndex = nIndex + len;
                    if (nSubIndex > 0 && strFile.Length > nSubIndex)
                    {
                        string strDate = strFile.Substring(nIndex + len);
                        DateTime dt = Convert.ToDateTime(strDate);
                        if (dt < date)
                            info.Delete();
                    }
                }

            }
        }
    }
}

