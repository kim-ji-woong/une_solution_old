using System.Threading;
using System.Collections;
using System.Configuration;
using DBUtility2;
using System.ServiceProcess;
using System.Diagnostics;

namespace RestartSOPWebServer
{
    public class ProcessData
    {
        private WebDBManager m_dbMgr = null;
        private string m_strServiceName = null;
        private string m_strExePath = null;
        private bool m_processing = false;

        public bool ReadConfig(int nIndex)
        {
            string strSiteID = ConfigurationManager.AppSettings.Get("db" + nIndex.ToString());

            int nSiteID;

            if (strSiteID != null && int.TryParse(strSiteID.Trim(), out nSiteID))
            {
                string strServiceName = ConfigurationManager.AppSettings.Get("serivce" + nIndex.ToString());
                string strExePath = ConfigurationManager.AppSettings.Get("exe" + nIndex.ToString());

                if (strServiceName != null && strServiceName.Trim().Length > 0)
                    m_strServiceName = strServiceName.Trim();
                else if (strExePath != null && strExePath.Trim().Length > 0)
                    m_strExePath = strExePath.Trim();
                else
                    return false;

                m_dbMgr = new WebDBManager(nSiteID);
                return true;
            }

            return false;
        }

        public void CheckDB()
        {
            if (m_processing)
                return;

            if (m_dbMgr != null)
            {
                m_processing = true;
                
                if (ReadData() == false)
                    m_processing = false;
            }
        }

        private bool ReadData()
        {
            string strSQL = "Select Restart from RestartServer where ID > 0";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return false;

            VariousData<int> restart = WebDBManager.GetIntField(arrResult[0].ToString());

            if (restart == null)
                return false;

            if (restart.Data == 1)
            {
                RunThread();
                return true;
            }

            return false;
        }

        private void RunThread()
        {
            if (m_strServiceName != null)
            {
                Thread t = new Thread(new ThreadStart(ServiceThread));
                t.Start();
            }
            else if (m_strExePath != null)
            {
                Thread t = new Thread(new ThreadStart(ExeThread));
                t.Start();
            }
        }

        private void ServiceThread()
        {
            ServiceController sc = new ServiceController(m_strServiceName);

            sc.Stop();

            // 초
            int nTimeOut = 10;

            for (int i=0;i<nTimeOut * 10;i++)
            {
                if (sc.Status == ServiceControllerStatus.Stopped)
                    break;

                Thread.Sleep(100);
            }

            if (sc.Status != ServiceControllerStatus.Stopped)
            {
                FinishDB();
                m_processing = false;
                return;
            }

            sc.Start();

            for (int i = 0; i < nTimeOut * 10; i++)
            {
                if (sc.Status == ServiceControllerStatus.Running)
                    break;

                Thread.Sleep(100);
            }

            FinishDB();
            m_processing = false;
        }

        private void ExeThread()
        {
            int nSlashIndex = m_strExePath.LastIndexOf('\\');
            int nDotIndex = m_strExePath.LastIndexOf('.');

            if (nSlashIndex < 0 || nDotIndex < 0)
            {
                FinishDB();
                m_processing = false;
                return;
            }

            string strProcessName = m_strExePath.Substring(nSlashIndex + 1, nDotIndex - nSlashIndex - 1);
            Process[] processList = Process.GetProcessesByName(strProcessName);

            foreach (Process process in processList)
            {
                process.Kill();
            }

            Process.Start(m_strExePath);

            FinishDB();
            m_processing = false;
        }

        private void FinishDB()
        {
            string strSQL = "Update RestartServer set Restart = 0 where ID = 1";
            m_dbMgr.GetResultData(strSQL);
        }
    }
}
