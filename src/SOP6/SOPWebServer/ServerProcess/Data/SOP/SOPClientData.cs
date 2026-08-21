using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using DBUtility2;
using libSOPPolicy;

namespace ServerProcess.Data.SOP
{
    public class SOPClientData : ServerProcess.Client.ClientData
    {
        private int m_nSOPGenUserID = -1;
        private string m_strSOPGenUserID = "";
        private string m_strNickName = "";
        private int m_nUserLevel = 0;
        private BaseSOPUser m_sopUser = null;
        private DirectDBManager m_dbMgr = null;

        public int ID
        {
            get { return m_nSOPGenUserID; }
            set { SetSOPUser(value); }
        }

        public BaseSOPUser User
        {
            get { return m_sopUser; }
        }

        public string UserID
        {
            get { return m_strSOPGenUserID; }
            set { m_strSOPGenUserID = value; }
        }

        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        public SOPClientData(DirectDBManager dbMgr)
            : base()
        {
            m_dbMgr = dbMgr;
        }

        public SOPClientData(string strSessionID, IPostMan postMan, DirectDBManager dbMgr)
            : base(strSessionID, postMan)
        {
            m_dbMgr = dbMgr;
        }

        public SOPClientData(string strSessionID, IPostMan postMan, int nClientType, int nClientSubType, DirectDBManager dbMgr)
            : base(strSessionID, postMan, nClientType, nClientSubType)
        {
            m_dbMgr = dbMgr;
        }

        private void SetSOPUser(int nUserID)
        {
            m_nSOPGenUserID = nUserID;

            if (m_sopUser == null)
            {
                m_sopUser = SOPUserFactory.CreateSOPUser(nUserID, m_dbMgr);
            }
        }

        // alarm에 대한 SOP 실행권한이 있는지 확인한다.
        public bool AbleToAccess(AlarmData alarm)
        {
            if (m_sopUser != null)
                return m_sopUser.AbletoAccess(alarm.SensorZoneID, m_dbMgr.SiteID, m_dbMgr);

            return false;
        }
    }
}
