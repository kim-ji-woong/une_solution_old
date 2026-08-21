using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public class Dashboard
    {
        private SOPHistory m_sopHistory = new SOPHistory();
        public SOPHistory SOPHistory
        {
            get { return m_sopHistory; }
            set { m_sopHistory = value; }
        }

        private SOPHistoryData m_sopHistoryData = new SOPHistoryData();
        public SOPHistoryData SOPHistoryData
        {
            get { return m_sopHistoryData; }
            set { m_sopHistoryData = value; }
        }
        
        private List<Models.BulletinMessage> m_bulletinMessages = new List<BulletinMessage>();
        public List<Models.BulletinMessage> BulletinMessages
        {
            get { return m_bulletinMessages; }
            set { m_bulletinMessages = value; }
        }

        private LostStatus m_lostStatus = new LostStatus();
        public LostStatus LostStatus
        {
            get { return m_lostStatus; }
            set { m_lostStatus = value; }
        }
    }
}