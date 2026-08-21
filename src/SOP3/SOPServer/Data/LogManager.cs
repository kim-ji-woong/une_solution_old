using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Threading;
using System.IO;

namespace SOPServer
{
    public class LogManager
    {
        private WebDBManager m_dbMgr = null;
        private bool m_isWorkingThread = false;
        private Thread m_thread = null;

        private int m_nSiteID = 1;
        public LogManager(WebDBManager dbMgr)
        {
            m_nSiteID = SDMSServer.NetworkServer.Instance.SiteID;
            m_dbMgr = dbMgr;
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
                    CheckDBLogs(dtNow);
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

        private bool CheckDBLogs(DateTime dtNow)
        {
            string strCompareTime = string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dtNow.Year - 1, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            if (!CheckSensorZoneHistory(strCompareTime))
                return false;

            if (!CheckBroadcastHistory(strCompareTime))
                return false;

            if (!CheckActionStepHistory(strCompareTime))
                return false;

            if (!CheckHistoryDisasterPos(100))
                return false;

            return true;
        }

        private bool CheckHistoryDisasterPos(int nLimitCount)
        {
            string strSQL = null;

            if (SDMSServer.NetworkServer.Instance.SimulationMode)
            {
                // SQLite 문법
                //strSQL = "Delete from HistoryDisasterPos where id < (select min(id) from HistoryDisasterPos where id in (select";
                //strSQL += " id from HistoryDisasterPos where SiteID = " + m_nSiteID;
                //strSQL += " order by id desc LIMIT 0, " + nLimitCount.ToString() + "))";

                string szText = "Delete from HistoryDisasterPos where SiteID = {0} and id < " +
                               " (select min(id) from HistoryDisasterPos where SiteID = {0} and id in " +
                               " (select id from HistoryDisasterPos where SiteID = {0} order by id desc LIMIT 0, {1}))";
                strSQL = string.Format(szText, m_nSiteID, nLimitCount);
            }
            else
            {
                // MS-SQL 문법
                //strSQL = "Delete from HistoryDisasterPos where id < (select min(id) from HistoryDisasterPos where  id in (select top ";
                //strSQL += nLimitCount.ToString() + " id from HistoryDisasterPos order by id desc))";

                string szText = "Delete from HistoryDisasterPos where SiteID = {0} and id < " +
                                " (select min(id) from HistoryDisasterPos where SiteID = {0} and id in " +
                                " (select top {1} id from HistoryDisasterPos where SiteID = {0} order by id desc))";
                strSQL = string.Format(szText, m_nSiteID, nLimitCount);
            }

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool CheckActionStepHistory(string strCompareTime)
        {
            // Site별 ActionStepHistory가져오는 Query
            // SELECT ash.ID, ash.ActionStepID,ash.RealMode,ash.BeginTime,ash.EndTime,ash.CancelTime,ash.PausedTime,ash.DetectTime,ash.Position,ash.LastAccessedUserID,ash.Description
            // FROM ActionStepHistory as ash , ActionStep as step, Disaster as dis , SubDisasterCategory as sdc , DisasterCategory as dc
            // WHERE ash.ActionStepID = step.ID and step.DisasterID = dis.ID and dis.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and dc.SiteID = 1

            // Site별 ComponentHistory 가져 오는 법
            // SELECT ch.ID,ch.ActionStepHistoryID,ch.ComponentID,ch.ComponentType,ch.Time,ch.Status,ch.Task,
            //        ch.CompleteCount,ch.ShowBoard,ch.AccessedUserID,ch.CheckedNotify1,ch.CheckedNotify2,ch.Description
            // FROM ComponentHistory as ch, ActionStepHistory as ash , ActionStep as step, Disaster as dis , SubDisasterCategory as sdc , DisasterCategory as dc
            // WHERE ch.ActionStepHistoryID = ash.ID and ash.ActionStepID = step.ID and step.DisasterID = dis.ID and dis.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and dc.SiteID = 1
            
            //string strSQL = "Delete from ComponentHistory where ActionStepHistoryID in (Select id from ActionStepHistory where BeginTime < " + strCompareTime + ")";
            string szText = "Delete from ComponentHistory where ActionStepHistoryID in " +
                            " (SELECT ash.ID FROM ActionStepHistory as ash, ActionStep as step, Disaster as dis, SubDisasterCategory as sdc, DisasterCategory as dc " +
                            "   WHERE ash.ActionStepID = step.ID AND step.DisasterID = dis.ID AND dis.SubDisasterID = sdc.ID "+
                            "      AND sdc.DisasterID = dc.ID AND dc.SiteID = {0} AND BeginTime < {1})";
            string strSQL = string.Format(szText, m_nSiteID, strCompareTime);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            //strSQL = "Delete from ActionStepHistory where BeginTime < " + strCompareTime;           
            szText = "Delete from ActionStepHistory where ID in " +
                     " (SELECT ash.ID FROM ActionStepHistory as ash, ActionStep as step,Disaster as dis,SubDisasterCategory as sdc,DisasterCategory as dc " +
                     "   WHERE ash.ActionStepID = step.ID AND step.DisasterID = dis.ID AND dis.SubDisasterID = sdc.ID " +
                     "      AND sdc.DisasterID = dc.ID AND dc.SiteID = {0} AND BeginTime < {1})";

            strSQL = string.Format(szText, m_nSiteID, strCompareTime);
            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool CheckBroadcastHistory(string strCompareTime)
        {
            // string strSQL = "Delete from BroadcastHistory where AddTime < " + strCompareTime;
            string szText = "Delete from BroadcastHistory where SiteID = {0} AND AddTime < {1}";
            
            string strSQL = string.Format(szText, m_nSiteID, strCompareTime);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool CheckSensorZoneHistory(string strCompareTime)
        {
            //string strSQL = "Delete from SensorReactionHistory where SensorHistoryID in (Select id from SensorZoneHistory where Time < " + strCompareTime + ")";
            string szText = "Delete from SensorReactionHistory where SensorHistoryID in "+
                            " (SELECT szh.id FROM SensorZoneHistory as szh, SensorZone as sz, EquipmentZone as ez " +
                            " WHERE szh.SensorID = sz.ID and sz.EquipZoneID = ez.ID and ez.SiteID = {0} and szh.Time < {1})";
            string strSQL = string.Format(szText, m_nSiteID, strCompareTime);
            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            //strSQL = "Delete from SensorZoneHistory where Time < " + strCompareTime;
            szText = "Delete from FROM SensorZoneHistory as szh, SensorZone as sz, EquipmentZone as ez " +
                     " WHERE szh.SensorID = sz.ID and sz.EquipZoneID = ez.ID and ez.SiteID = {0} and szh.Time < {1})";
            
            strSQL = string.Format(szText, m_nSiteID, strCompareTime);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private void CheckTcpLogs(DateTime dtNow)
        {
            string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string szFullPath = System.IO.Directory.GetParent(szPath).FullName;

            string[] arrFiles = Directory.GetFiles(szFullPath);

            // Server는 센서 모니터와 Server 로그를 한꺼번에 지운다.
            string strKey = "sdmsserver.log-";
            string strKey2 = "SensorMonitor.log-";

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
