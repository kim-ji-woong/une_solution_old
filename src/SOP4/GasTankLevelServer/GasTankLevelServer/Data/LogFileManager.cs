using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Threading;
using System.IO;

namespace GasLevelServer
{
    public class LogFileManager
    {
        private bool m_isWorkingThread = false;
        private Thread m_thread = null;

        private int m_nSiteID = 1;
        public LogFileManager()
        {
            m_nSiteID = LevelMeterNetworkServer.Instance.SiteID;           
            Run();
        }

        public void Stop()
        {
            try
            {
                if (m_thread.IsAlive)
                {
                    m_isWorkingThread = false;
                    m_thread.Join(1000);
                    m_thread.Abort();

                    m_thread = null;
                }
            }
            catch (Exception)
            {
                m_isWorkingThread = false;
            }
        }

        private void Run()
        {
            m_thread = new Thread(new ThreadStart(WorkerThreadMethod));
            m_thread.IsBackground = false;
            m_thread.Start();
        }

        private void WorkerThreadMethod()
        {
            m_isWorkingThread = true;
            int nSleepTime = 3500;

            while (m_isWorkingThread)
            {
                DateTime dtNow = DateTime.Now;

                // 새벽 1시에 한번만 업데이트 한다.
                if (dtNow.Hour == 1)
                {
                    CheckTcpLogs(dtNow);

                    // 1시가 다시 나올수 없도록 SleepTime을 4000초로 준다.
                    nSleepTime = 4000;
                }
                else
                    nSleepTime = 3500;

                for (int i = 0; i < nSleepTime; i++)
                {
                    if (!m_isWorkingThread)
                        break;
                    Thread.Sleep(1000);
                }
            }
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtFile = new DateTime(nYear, nMonth, nDay);
            TimeSpan spant = dtNow - dtFile;
            if (spant.TotalDays > 30.0)
                return true;
            return false;    
        }
        
        private void CheckTcpLogs(DateTime dtNow)
        {
            string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string szFullPath = System.IO.Directory.GetParent(szPath).FullName;

            string[] arrFiles = Directory.GetFiles(szFullPath);

            // Server는 센서 모니터와 Server 로그를 한꺼번에 지운다.
            string strKey = "SensorData.log-";
            string strKey2 = "LevelServer.log-";

            int len = strKey.Length;
            int nYear, nMonth, nDay;

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.IndexOf(strKey);

                if (nIndex < 0)
                {
                    nIndex = strFile.IndexOf(strKey2);

                    if (nIndex < 0)
                        continue;
                }

                string strDate = strFile.Substring(nIndex + len);

                int nIndex1 = strDate.IndexOf('-');
                int nIndex2 = strDate.LastIndexOf('-');

                if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                    continue;

                string strYear = strDate.Substring(0, nIndex1);
                string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDay = strDate.Substring(nIndex2 + 1);

                if (!int.TryParse(strYear, out nYear))
                    continue;
                if (!int.TryParse(strMonth, out nMonth))
                    continue;
                if (!int.TryParse(strDay, out nDay))
                    continue;

                if (IsPassedTime(dtNow, nYear, nMonth, nDay))
                    File.Delete(strFile);
            }
        }
    }
}
