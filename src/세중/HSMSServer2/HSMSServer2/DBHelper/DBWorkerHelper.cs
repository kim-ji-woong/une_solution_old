using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using HSMS;

namespace HSMSServer2
{
    class DBWorkerHelper
    {
        public static bool RemoveWorker(DBConn conn, DataWorker worker)
        {
            if (worker == null)
                return false;

            bool bResult = false;
            int nSiteID = NetworkServer.Instance.SiteID;

            ArrayList arQuerys = new ArrayList();
            string strDeleteSQL = string.Format("Delete from IgnoreSensorsToWorker where WorkerID = {0} and SiteID = {1}", worker.ID, nSiteID);
            arQuerys.Add(strDeleteSQL);

            string strDeleteSQL3 = string.Format("Delete from Manager where MemberID = {0} and SiteID = {1}", worker.MemberID, nSiteID);
            arQuerys.Add(strDeleteSQL3);

            string strDeleteSQL2 = string.Format("Delete from Worker where ID = {0} and SiteID = {1}", worker.ID, nSiteID);
            arQuerys.Add(strDeleteSQL2);

            bResult = DBHelper.ExecuteSQL(conn, arQuerys);
            if (bResult == true)
            {
                worker.ID = -1;
                worker.EnterLevel = -1;
            }

            return bResult;
        }

        public static bool DeleteWorker(DBConn conn, DataWorker worker)
        {
            if (worker == null)
                return false;
            
            bool bResult = false;
            int nSiteID = NetworkServer.Instance.SiteID;

            ArrayList arQuerys = new ArrayList();

            string strDeleteSQL3 = string.Format("Delete from IgnoreAlarm where WorkerID = {0} and SiteID = {1}", worker.ID, nSiteID);
            arQuerys.Add(strDeleteSQL3);

            string strDeleteSQL = string.Format("Delete from IgnoreSensorsToWorker where WorkerID = {0} and SiteID = {1}", worker.ID, nSiteID);
            arQuerys.Add(strDeleteSQL);

            string strDeleteSQL2 = string.Format("Delete from Worker where ID = {0} and SiteID = {1}", worker.ID, nSiteID);
            arQuerys.Add(strDeleteSQL2);

            bResult = DBHelper.ExecuteSQL(conn, arQuerys);
            if (bResult == true)
            {
                worker.ID = -1;
                worker.EnterLevel = -1;
            }           

            return bResult;
        }

        public static bool UpdateWorker(DBConn conn, DataWorker worker, int nChangeLevel)
        {
            if (worker == null)
                return false;
            
            bool bResult = false;
            int nSiteID = NetworkServer.Instance.SiteID;
            string strSQL = string.Format("update Worker set WorkerLevel = '{0}' where ID = {1} and SiteID = {2}", nChangeLevel, worker.ID, nSiteID);

            bResult = DBHelper.ExecuteSQL(conn, strSQL);
           
            return bResult;
        }

        public static bool AddWorker(DBConn conn, DataWorker m_worker)
        {
            if (m_worker == null)
                return false;           

            bool bResult = false;                     
            
            int nMaxID = -1;
            string strSQL = "insert into Worker (ID,MemberID,WorkerLevel,SiteID,SensorDetect ) Values(" + DBHelper.MaxID + ",'" + m_worker.MemberID + "'," + m_worker.EnterLevel + ","+ m_worker.SiteID +","+ (m_worker.SensorDetect==true?1:0) +")";
            
            bResult = DBHelper.ExecuteSQL(conn, strSQL, "Worker", ref nMaxID);
            if (bResult == true)
            {
                m_worker.ID = nMaxID;
            } 
     
            return bResult;
        }        
    }
}
