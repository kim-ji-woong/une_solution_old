using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace IntegratedManagement2
{
    public class BroadcastWatcher
    {
        private bool m_runThread = false;
        private static BroadcastWatcher m_instance = null;

        public static BroadcastWatcher Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new BroadcastWatcher();

                return m_instance;
            }
        }


        private int m_nSiteID = 1;
        protected BroadcastWatcher()
        {
            m_nSiteID = FormMain.Instance.SiteID;

        }

        public void Start()
        {
            if (!m_runThread)
            {
                Thread t = new Thread(WatchBroadcast);
                t.Start();
            }
        }

        public void Stop()
        {
            m_runThread = false;
        }

        private void WatchBroadcast()
        {
            DBUtility.WebDBManager dbMgr = SOPHiddenServer.HiddenServer.Instance.DBManager;
            
            if (dbMgr == null)
                return;

            m_runThread = true;

            string strServerName = dbMgr.LoadIni("Server_Name", "Simulation");
            string strPort = dbMgr.LoadIni("Port", "Simulation");
            string strResultFilePath = System.Windows.Forms.Application.StartupPath + "\\TTSSimulationResult.txt";

            bool isFirst = true;

            while (m_runThread)
            {
                string strSQL = "select Text, UseSiren, RepeatCount, AddTime from Broadcast WHERE SiteID = " + m_nSiteID;
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    break;

                int nResultCount = arrResult.Count;
                DateTime dtDefault = new DateTime();

                for (int i=0;i<nResultCount-3;i+=4)
                {
                    string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i], "");
                    string useSiren = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0).ToString();
                    int nRepeatCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                    DateTime dt = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);

                    if (strMessage.Length == 0 || useSiren.Length == 0)
                        continue;

                    string strAddTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                        dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

                    if (isFirst)
                    {
                        KillProcess("TTSSimulator");
                        isFirst = false;
                    }

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.Arguments = useSiren + " 1 " + strServerName + " " + strPort + " \"" + strMessage + "\" \"" + strResultFilePath + "\"";
                    info.CreateNoWindow = true;
                    info.FileName = System.Windows.Forms.Application.StartupPath + "\\TTSSimulator.exe";

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo = info;

                    bool isStarted = false;

                    // Process가 실행되지 않으면 3회 반복한다.
                    for (int j = 0; j < 3; j++)
                    {
                        isStarted = process.Start();

                        if (isStarted)
                            break;

                        Thread.Sleep(1000);
                    }

                    dbMgr.GetResultData("Delete from Broadcast where AddTime = '" + strAddTime + "'", 0);
                    
                    if (isStarted)
                        WaitFinishBroadcast(strResultFilePath, process);
                }

                Thread.Sleep(1000);
            }
        }

        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    process.Kill();
                    break;
                }
            }
        }

        private void WaitFinishBroadcast(string strResultFilePath, System.Diagnostics.Process process)
        {
            int nTimeCount = 0;
            // 연습모드용 방송이 끝나기를 기다리는 Timer의 최대 대기시간(10분)
            int nBroadcastWaitTime = 600;

            while (nTimeCount < nBroadcastWaitTime)
            {
                if (System.IO.File.Exists(strResultFilePath))
                {
                    System.IO.File.Delete(strResultFilePath);
                    return;
                }
                else
                {
                    nTimeCount++;
                    Thread.Sleep(1000);
                }

                if (!m_runThread)
                    break;
            }

            try
            {
                process.Kill();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
    }
}
