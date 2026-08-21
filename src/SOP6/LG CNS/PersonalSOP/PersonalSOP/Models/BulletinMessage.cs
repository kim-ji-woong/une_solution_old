using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public class BulletinMessage
    {
        private int m_nID = -1;
        private DateTime m_dtTime = new DateTime();
        private string m_strTitle = "";
        private string m_strMessage = "";
        private string m_strImagePath = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public DateTime Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string ImagePath
        {
            get { return m_strImagePath; }
            set { m_strImagePath = value; }
        }
    }
}