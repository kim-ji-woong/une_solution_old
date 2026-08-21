using System;
using System.Collections.Generic;
using System.Collections;

namespace DBUtility2
{
    /// <summary>
    /// Proxy 설정등과 같이 DirectDB를 사용할 수 없는 환경을 위한 클래스
    /// </summary>
    public class NoDirectDBManager : DirectDBManager
    {
        private WebDBManager m_dbMgr = null;
        private DBType m_dbType = DBType.TypeCount;

        public DBType DatabaseType
        {
            get { return m_dbType; }
            set { m_dbType = value; }
        }

        public override bool Connect()
        {
            if (m_dbMgr == null)
            {
                string strWebServerURL = this.Host.ToLower();

                m_dbMgr = new WebDBManager(DBName, m_nSiteID);
                m_dbMgr.DatabaseType = (WebDBManager.DBType)m_dbType;
                m_dbMgr.WebServerURL = Host.ToLower().StartsWith("http://") ? Host : "http://" + Host;
            }

            return true;
        }

        public override ArrayList GetResultData(string strSQL)
        {
            if (m_dbMgr == null)
                return null;

            return m_dbMgr.GetResultData(strSQL);
        }

        public override bool BeginBatch()
        {
            if (m_dbMgr == null)
                return false;

            return m_dbMgr.BeginBatch();
        }

        public override ArrayList GetBatchData(string strSQL)
        {
            if (m_dbMgr == null)
                return null;

            return m_dbMgr.GetBatchData(strSQL);
        }

        public override bool BatchCommit()
        {
            if (m_dbMgr == null)
                return false;

            return m_dbMgr.BatchCommit();
        }

        public override bool BatchRollback()
        {
            if (m_dbMgr == null)
                return false;

            return m_dbMgr.BatchRollback();
        }

        public override void Close()
        {
        }

        public override DirectDBManager Clone()
        {
            NoDirectDBManager mgr = new NoDirectDBManager();

            mgr.Host = Host;
            mgr.ID = ID;
            mgr.PW = PW;
            mgr.DBName = DBName;
            mgr.CharSet = CharSet;
            mgr.SiteID = SiteID;
            mgr.m_dbType = m_dbType;
            mgr.m_dbMgr = m_dbMgr == null ? null : m_dbMgr.Clone();

            return mgr;
        }
    }
}
