using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingMessage.Data
{
    public class DataManager
    {
        WebDBManager m_dbMgr = null;

        Dictionary<int, MemberData> m_dicMembers = new Dictionary<int, MemberData>();
        public Dictionary<int, MemberData> Members
        { 
            get { return m_dicMembers; } 
            set { m_dicMembers = value; }
        }

        public DataManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            LoadDataMember(dbMgr, m_dicMembers);
        }

        public bool LoadDataMember(WebDBManager dbMgr, Dictionary<int, MemberData> dicMembers)
        {
            dicMembers.Clear();

            string strSQL = string.Format("SELECT ID, UserID, NickName FROM LinkMember");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            MemberData data;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strUserID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strNickName = WebDBManager.GetStringField(arrResult[i + 2], "");

                data = new MemberData();
                data.ID = nID;
                data.UserID = strUserID;
                data.NickName = strNickName;

                dicMembers[nID] = data;
            }

            return true;
        }

        public bool InsertLinkMessage(string strSender, string strReceiver, string strMessage)
        {
            string strSQL = string.Format("Insert into LinkMessage (Sender, Receiver, Message) " +
                "Values ('" + strSender + "', '" + strReceiver + "', '" + strMessage + "')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }
    }

    public class MemberData
    {
        private int m_nID = -1;
        private string m_strUserID = "";
        private string m_strNickName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

    }
}
