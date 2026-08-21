using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Threading;
using System.IO;
using System.Collections;

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

                    // 1. Log와는 별개로 SDMSConfig를 2초에 한번씩 검사한다.
                    // 2. 새로운 SDMSMessage가 있는지 검사하여, 있으면 SDMS Client들에게 알린다.
                    if (i % 2 == 0)
                    {
                        Data.SDMSConfigWatcher.Instance.Watch();
                        SDMSMessageWatcher.ReadNewMessage(m_dbMgr);
                    }
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

            if (!CheckSDMSMessage(strCompareTime))
                return false;

            if (CheckSensorTagHistory(strCompareTime))
                SDMSServer.NetworkServer.Instance.ServiceProvider.SendRemoveSensorTagHistory(dtNow.AddYears(-1));
            else
                return false;

            return true;
        }

        private bool CheckSensorTagHistory(string strCompareTime)
        {
            string strSQL = string.Format("Delete from SensorTagHistory where TimeStamp < {0} and SiteID = {1}", strCompareTime, m_nSiteID);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        // strCompareTime 이전에 작성된 SDMSMessage는 삭제한다.
        private bool CheckSDMSMessage(string strCompareTime)
        {
            string strSQL = string.Format("Delete from SDMSMessage where SiteID = {0} and SendTime < {1}",
                m_nSiteID, strCompareTime);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
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

                string szText = "";

                if (m_dbMgr.DatabaseType == WebDBManager.DBType.mysql)
                {
                    szText = "Delete from HistoryDisasterPos where SiteID = {0} and id < " +
                                " (select min(id) from HistoryDisasterPos where SiteID = {0} and id in " +
                                " (select id from HistoryDisasterPos where SiteID = {0} order by id desc LIMIT 0, {1}))";
                }
                else if (m_dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
                {
                    szText = "Delete from HistoryDisasterPos where SiteID = {0} and id < " +
                                " (select min(id) from HistoryDisasterPos where SiteID = {0} and id in " +
                                " (select top {1} id from HistoryDisasterPos where SiteID = {0} order by id desc))";
                }
                else
                    return false;
                
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

            // ComponentHistory ID, ActionStepHistory 가져오기
            string strActionStepHistoryIDs;
            string strIDs = GetComponentHistoryIDs4Delete(strCompareTime, out strActionStepHistoryIDs);

            if (strIDs == null)
                return false;
            else if (strIDs.Length == 0)
                return true;

            if (strActionStepHistoryIDs == "")
                return true;

            string strSQL = "Delete from ComponentHistoryDetail where ComponentHistoryID in (" + strIDs + ")";

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            strSQL = "Delete from ComponentHistory where ID in (" + strIDs + ")";

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            strSQL = "Delete from ActionStepUsingTeam where ActionStepHistoryID in (" + strActionStepHistoryIDs + ")";

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            strSQL = "Delete from ActionStepHistory where ID in (" + strActionStepHistoryIDs + ")";

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            /*string szText = "Delete from ComponentHistory where ActionStepHistoryID in " +
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
                return false;*/

            return true;
        }

        private string GetComponentHistoryIDs4Delete(string strCompareTime, out string strActionStepHistoryIDs)
        {
            strActionStepHistoryIDs = null;

            string strSQL = string.Format("SELECT ash.ID FROM ActionStepHistory as ash, ActionStep as step, Disaster as dis, SubDisasterCategory as sdc, DisasterCategory as dc " +
                            "   WHERE ash.ActionStepID = step.ID AND step.DisasterID = dis.ID AND dis.SubDisasterID = sdc.ID " +
                            "      AND sdc.DisasterID = dc.ID AND dc.SiteID = {0} AND ash.BeginTime < {1}",
                            m_nSiteID, strCompareTime);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());

                if (nID == null)
                    continue;

                if (strActionStepHistoryIDs == null)
                    strActionStepHistoryIDs = nID.Data.ToString();
                else
                    strActionStepHistoryIDs += ", " + nID.Data.ToString();
            }

            if (strActionStepHistoryIDs == null)
                return "";

            strSQL = "Select ID from ComponentHistory where ActionStepHistoryID in (" + strActionStepHistoryIDs + ")";

            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            string strIDs = "";

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());

                if (nID == null)
                    continue;

                if (strIDs.Length == 0)
                    strIDs = nID.Data.ToString();
                else
                    strIDs += ", " + nID.Data.ToString();
            }

            return strIDs;
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

        // 데이터 삭제를 위한 ID 검사시 ID의 최소, 최대값을 사용하는 방식
        private bool CheckSensorZoneHistory(string strCompareTime)
        {
            string strSQL = string.Format("SELECT id FROM SensorZoneHistory where SiteID = {0} and Time < {1}", m_nSiteID, strCompareTime);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            int nMinSensorZoneHistoryID = -1, nMaxSensorZoneHistoryID = -1;
            Dictionary<int, int> dicSensorZoneHistoryIDs = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount; i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (nMinSensorZoneHistoryID < 0)
                    nMinSensorZoneHistoryID = id.Data;
                else if (nMinSensorZoneHistoryID > id.Data)
                    nMinSensorZoneHistoryID = id.Data;

                if (nMaxSensorZoneHistoryID < id.Data)
                    nMaxSensorZoneHistoryID = id.Data;

                dicSensorZoneHistoryIDs[id.Data] = id.Data;
            }

            if (dicSensorZoneHistoryIDs.Count == 0)
                return true;

            strSQL = "Select ID, SensorHistoryID from SensorReactionHistory where SensorHistoryID >= " + nMinSensorZoneHistoryID.ToString() + " and SensorHistoryID <= " + nMaxSensorZoneHistoryID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;
            int nMinSensorReactionHistoryID = -1, nMaxSensorReactionHistoryID = -1;
            Dictionary<int, int> dicSensorReactionHistoryIDs = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || sensorHistoryID == null)
                    continue;

                if (dicSensorZoneHistoryIDs.ContainsKey(sensorHistoryID.Data) == false)
                    continue;

                if (nMinSensorReactionHistoryID < 0)
                    nMinSensorReactionHistoryID = id.Data;
                else if (nMinSensorReactionHistoryID > id.Data)
                    nMinSensorReactionHistoryID = id.Data;

                if (nMaxSensorReactionHistoryID < id.Data)
                    nMaxSensorReactionHistoryID = id.Data;

                dicSensorReactionHistoryIDs[id.Data] = id.Data;
            }

            if (dicSensorReactionHistoryIDs.Count > 0)
            {
                string strNotIncludeIDs = GetNotIncludeIDs("SensorReactionHistory", nMinSensorReactionHistoryID, nMaxSensorReactionHistoryID, dicSensorReactionHistoryIDs);
                string strCondition = MakeConditionWithNotIncludeIDs("ReactionHistoryID", nMinSensorReactionHistoryID, nMaxSensorReactionHistoryID, strNotIncludeIDs);

                strSQL = "Delete from SDMSSMSHistory where " + strCondition;

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                if (RemoveSensorReactionHistoryDescriptions(nMinSensorReactionHistoryID, nMaxSensorReactionHistoryID, strNotIncludeIDs) == false)
                    return false;

                strCondition = MakeConditionWithNotIncludeIDs("ID", nMinSensorReactionHistoryID, nMaxSensorReactionHistoryID, strNotIncludeIDs);
                strSQL = "Delete from SensorReactionHistory where " + strCondition;

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            string strNotIncludeSensorZoneHistoryIDs = GetNotIncludeIDs("SensorZoneHistory", nMinSensorZoneHistoryID, nMaxSensorZoneHistoryID, dicSensorZoneHistoryIDs);
            string strCondition2 = MakeConditionWithNotIncludeIDs("ID", nMinSensorZoneHistoryID, nMaxSensorZoneHistoryID, strNotIncludeSensorZoneHistoryIDs);

            strSQL = "Delete from SensorZoneHistory where " + strCondition2;

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool RemoveSensorReactionHistoryDescriptions(int nMinSensorReactionHistoryID, int nMaxSensorReactionHistoryID, string strNotIncludeIDs)
        {
            string strSQL = "Select ID, RefCount from SensorReactionHistoryDescriptionText";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            Dictionary<int, int> dicDescRefCount = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> refCount = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || refCount == null)
                    continue;

                dicDescRefCount[id.Data] = refCount.Data;
            }

            string strCondition = MakeConditionWithNotIncludeIDs("SensorReactionHistoryID", nMinSensorReactionHistoryID, nMaxSensorReactionHistoryID, strNotIncludeIDs);

            strSQL = "Select ID, DescriptionID from SensorReactionHistoryDescription where " + strCondition;
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            int nMinDescriptionID = -1, nMaxDescriptionID = -1;
            Dictionary<int, int> dicDescriptionIDs = new Dictionary<int, int>();
            string strRemoveTextIDs = "";

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> textID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || textID == null)
                    continue;

                dicDescRefCount[textID.Data] = dicDescRefCount[textID.Data] - 1;

                if (nMinDescriptionID < 0)
                    nMinDescriptionID = id.Data;
                else if (nMinDescriptionID > id.Data)
                    nMinDescriptionID = id.Data;

                if (nMaxDescriptionID < id.Data)
                    nMaxDescriptionID = id.Data;

                dicDescriptionIDs[id.Data] = id.Data;
            }

            foreach (KeyValuePair<int, int> pair in dicDescRefCount)
            {
                if (pair.Value <= 0)
                {
                    if (strRemoveTextIDs.Length == 0)
                        strRemoveTextIDs = pair.Value.ToString();
                    else
                        strRemoveTextIDs += ", " + pair.Value.ToString();
                }
            }

            if (dicDescriptionIDs.Count > 0)
            {
                strNotIncludeIDs = GetNotIncludeIDs("SensorReactionHistoryDescription", nMinDescriptionID, nMaxDescriptionID, dicDescriptionIDs);
                strCondition = MakeConditionWithNotIncludeIDs("ID", nMinDescriptionID, nMaxDescriptionID, strNotIncludeIDs);

                strSQL = "Delete from SensorReactionHistoryDescription where " + strCondition;

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                if (strRemoveTextIDs.Length > 0)
                {
                    strSQL = "Delete from SensorReactionHistoryDescriptionText where ID in (" + strRemoveTextIDs + ")";

                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return false;
                }
            }

            return true;
        }

        private string MakeConditionWithNotIncludeIDs(string strFieldName, int nMinID, int nMaxID, string strNotIncludeIDs)
        {
            string strCondition = "";

            if (strNotIncludeIDs.Length > 0)
                strCondition = string.Format("{0} >= {1} and {0} <= {2} and {0} not in ({3})", strFieldName, nMinID, nMaxID, strNotIncludeIDs);
            else
                strCondition = string.Format("{0} >= {1} and {0} <= {2}", strFieldName, nMinID, nMaxID);

            return strCondition;
        }

        // nMinID와 nMaxID 사이에 있는 값중에 dicIDs에 포함되지 않는 리스트를 얻어온다.
        private string GetNotIncludeIDs(string strTableName, int nMinID, int nMaxID, Dictionary<int, int> dicIDs)
        {
            string strSQL = "Select ID from " + strTableName + " where ID >= " + nMinID.ToString() + " and ID <= " + nMaxID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return "";

            string strNotIncludeIDs = "";
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (dicIDs.ContainsKey(id.Data) == false)
                {
                    if (strNotIncludeIDs.Length == 0)
                        strNotIncludeIDs = id.Data.ToString();
                    else
                        strNotIncludeIDs += ", " + id.Data.ToString();
                }
            }

            return strNotIncludeIDs;
        }

        // 데이터 삭제를 위한 ID 검사시 전체 ID List를 사용하는 방식
        /*private bool CheckSensorZoneHistory(string strCompareTime)
        {
            string strSQL = string.Format("SELECT id FROM SensorZoneHistory where SiteID = {0} and Time < {1}", m_nSiteID, strCompareTime);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            string strSensorZoneHistoryIDs = "";

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (strSensorZoneHistoryIDs.Length == 0)
                    strSensorZoneHistoryIDs = id.Data.ToString();
                else
                    strSensorZoneHistoryIDs += ", " + id.Data.ToString();
            }

            if (strSensorZoneHistoryIDs.Length == 0)
                return true;

            strSQL = "Select ID from SensorReactionHistory where SensorHistoryID in (" + strSensorZoneHistoryIDs + ")";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;
            string strSensorReactionHistoryIDs = "";

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (strSensorReactionHistoryIDs.Length == 0)
                    strSensorReactionHistoryIDs = id.Data.ToString();
                else
                    strSensorReactionHistoryIDs += ", " + id.Data.ToString();
            }

            if (strSensorReactionHistoryIDs.Length > 0)
            {
                strSQL = "Delete from SDMSSMSHistory where ReactionHistoryID in (" + strSensorReactionHistoryIDs + ")";

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                if (RemoveSensorReactionHistoryDescriptions(strSensorReactionHistoryIDs) == false)
                    return false;

                strSQL = "Delete from SensorReactionHistory where ID in (" + strSensorReactionHistoryIDs + ")";

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            strSQL = "Delete from SensorZoneHistory where ID in (" + strSensorZoneHistoryIDs + ")";

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool RemoveSensorReactionHistoryDescriptions(string strSensorReactionHistoryIDs)
        {
            string strSQL = "Select ID, RefCount from SensorReactionHistoryDescriptionText";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            Dictionary<int, int> dicDescRefCount = new Dictionary<int, int>();

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> refCount = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || refCount == null)
                    continue;

                dicDescRefCount[id.Data] = refCount.Data;
            }

            strSQL = "Select ID, DescriptionID from SensorReactionHistoryDescription where SensorReactionHistoryID in (" + strSensorReactionHistoryIDs + ")";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            string strRemoveDescriptionIDs = "", strRemoveTextIDs = "";

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> textID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || textID == null)
                    continue;

                dicDescRefCount[textID.Data] = dicDescRefCount[textID.Data] - 1;

                if (strRemoveDescriptionIDs.Length == 0)
                    strRemoveDescriptionIDs = id.Data.ToString();
                else
                    strRemoveDescriptionIDs += ", " + id.Data.ToString();
            }

            foreach (KeyValuePair<int, int> pair in dicDescRefCount)
            {
                if (pair.Value <= 0)
                {
                    if (strRemoveTextIDs.Length == 0)
                        strRemoveTextIDs = pair.Value.ToString();
                    else
                        strRemoveTextIDs += ", " + pair.Value.ToString();
                }
            }

            if (strRemoveDescriptionIDs.Length > 0)
            {
                strSQL = "Delete from SensorReactionHistoryDescription where ID in (" + strRemoveDescriptionIDs + ")";

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                if (strRemoveTextIDs.Length > 0)
                {
                    strSQL = "Delete from SensorReactionHistoryDescriptionText where ID in (" + strRemoveTextIDs + ")";

                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return false;
                }
            }

            return true;
        }*/

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

    public class SDMSMessageWatcher
    {
        public class Message
        {
            private int m_nID = -1;
            // 제목
            private string m_strTitle = "";
            // 본문
            private string m_strMessage = null;
            private string m_strRtf = null;
            private int m_nSOPGenUserID = -1;
            private string m_strSenderName = null;
            private DateTime m_dtReceiveTime = new DateTime();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            // 제목
            public string Title
            {
                get { return m_strTitle; }
                set { m_strTitle = value; }
            }

            // 본문
            public string Text
            {
                get { return m_strMessage; }
                set { m_strMessage = value; }
            }

            public string RTF
            {
                get { return m_strRtf; }
                set { m_strRtf = value; }
            }

            public int SOPGenUserID
            {
                get { return m_nSOPGenUserID; }
                set { m_nSOPGenUserID = value; }
            }

            public string SenderName
            {
                get { return m_strSenderName; }
                set { m_strSenderName = value; }
            }

            public DateTime Time
            {
                get { return m_dtReceiveTime; }
                set { m_dtReceiveTime = value; }
            }
        }

        private static string m_strIniFileName = "";//"LastReadMessage.ini";
        private static int m_nLastReadID = -1;
        private const int SDMS_PUBLIC_MESSAGE_TYPE = 0;

        public static int LastReadID
        {
            get { return m_nLastReadID; }
        }

        private static void CheckIniFile()
        {
            if (m_strIniFileName.Length == 0)
            {
                string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                m_strIniFileName = Directory.GetParent(szPath).FullName + "\\LastReadMessage.ini";
            }
        }

        private static void ReadLastID()
        {
            CheckIniFile();

            if (File.Exists(m_strIniFileName))
            {
                StreamReader reader = new StreamReader(m_strIniFileName, Encoding.UTF8);
                string strLine = reader.ReadLine().Trim();
                reader.Close();

                int.TryParse(strLine, out m_nLastReadID);
            }
        }

        private static void WriteLastID(int nID)
        {
            CheckIniFile();

            StreamWriter writer = new StreamWriter(m_strIniFileName, false, Encoding.UTF8);
            writer.Write(nID);
            writer.Close();

            m_nLastReadID = nID;
        }

        public static void ReadNewMessage(WebDBManager dbMgr)
        {
            if (m_nLastReadID < 0)
                ReadLastID();

            string strSQL = "Select ID, SendTime, Title, Text, RichTextFormat, SOPGenUserID, SenderName from SDMSMessage ";
            strSQL += string.Format("where SiteID = {0} and MessageType = {1} and ID > {2}",
                SDMSServer.NetworkServer.Instance.SiteID, SDMS_PUBLIC_MESSAGE_TYPE, m_nLastReadID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nLastReadID = m_nLastReadID;
            List<Message> messages = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 1].ToString());
                string strTitle = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strText = WebDBManager.GetStringField(arrResult[i + 3]);
                string strRtf = WebDBManager.GetStringField(arrResult[i + 4], "");
                VariousData<int> userID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strSenderName = WebDBManager.GetStringField(arrResult[i + 6], "");

                if (id == null || time == null || strText == null || userID == null)
                    continue;

                if (nLastReadID < id.Data)
                    nLastReadID = id.Data;

                Message message = new Message();
                message.ID = id.Data;
                message.RTF = strRtf;
                message.SenderName = strSenderName;
                message.SOPGenUserID = userID.Data;
                message.Title = strTitle;
                message.Text = strText;
                message.Time = time.Data;

                if (messages == null)
                    messages = new List<Message>();

                messages.Add(message);
            }

            if (messages != null)
            {
                if (SendMessages(messages))
                    WriteLastID(nLastReadID);

                messages.Clear();
            }
        }

        private static bool SendMessages(List<Message> messages)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(SDMS.SDMSCommandType.SDMS_PUBLIC_MESSAGE);
            arrDatas.Add(messages.Count);

            foreach (Message message in messages)
            {
                arrDatas.Add(message.ID);
                arrDatas.Add(message.Time.ToBinary());
                arrDatas.Add(message.Title);
                arrDatas.Add(message.Text);
                arrDatas.Add(message.RTF);
                arrDatas.Add(message.SOPGenUserID);
                arrDatas.Add(message.SenderName);
            }

            byte[] bytes = TcpLib2.TcpHelper.MakeBytes(SDMS.TCP_ID.SDMS_COMMAND, arrDatas);

            if (bytes == null)
                return false;

            SDMSServer.NetworkServer.Instance.ServiceProvider.SendClientData(bytes, SDMS.TCP_CLIENT.SDMS_CLIENT, false);
            return true;
        }
    }
}
