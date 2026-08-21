using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.IO;

namespace ServerProcess.Data
{
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

        //private static string m_strIniFileName = "";//"LastReadMessage.ini";
        private int m_nLastReadID = -1;
        private const int SDMS_PUBLIC_MESSAGE_TYPE = 0;
        private const string PROPERTY_NAME = "LastReadSDMSMessageID";

        private static SDMSMessageWatcher m_instance = new SDMSMessageWatcher();

        public static SDMSMessageWatcher Instance
        {
            get { return m_instance; }
        }

        public int LastReadID
        {
            get { return m_nLastReadID; }
        }

        /*private static void CheckIniFile()
        {
            if (m_strIniFileName.Length == 0)
            {
                string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                m_strIniFileName = Directory.GetParent(szPath).FullName + "\\LastReadMessage.ini";
            }
        }*/

        private bool ReadLastID(DirectDBManager dbMgr)
        {
            string strSQL = string.Format("Select ID, PropertyValue from OptionSDMS where PropertyName = '{0}' and SiteID = {1}", PROPERTY_NAME, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                arrResult = dbMgr.GetResultData("Select Max(ID) From OptionSDMS");
                if (arrResult == null)
                    return false;

                int nMaxID = 0;
                if (arrResult.Count > 0)
                    nMaxID = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString(), 0);

                nMaxID++;

                strSQL = string.Format("Insert into OptionSDMS (ID, PropertyName, PropertyValue, SiteID, Description) values ({2}, '{0}', '-1', {1}, '마지막에 읽은 SDMSMessage ID')",
                    PROPERTY_NAME, dbMgr.SiteID, nMaxID);

                if (dbMgr.GetResultData(strSQL) == null)
                    return false;

                m_nLastReadID = -1;
                return true;
            }
            else
            {
                if (arrResult.Count >= 2)
                {
                    VariousData<int> readID = WebDBManager.GetIntField(arrResult[1].ToString());

                    if (readID != null)
                    {
                        m_nLastReadID = readID.Data;
                        return true;
                    }
                }
            }

            return false;
        }

        private void WriteLastID(DirectDBManager dbMgr, int nID)
        {
            string strSQL = string.Format("Update OptionSDMS set PropertyValue = '{0}' where PropertyName = '{1}' and SiteID = {2}",
                nID, PROPERTY_NAME, dbMgr.SiteID);

            if (dbMgr.GetResultData(strSQL) != null)
                m_nLastReadID = nID;
        }

        public void ReadNewMessage(DirectDBManager dbMgr)
        {
            if (m_nLastReadID < 0)
            {
                if (ReadLastID(dbMgr) == false)
                    return;
            }

            string strSQL = "Select ID, SendTime, Title, Text, RichTextFormat, SOPGenUserID, SenderName from SDMSMessage ";
            strSQL += string.Format("where SiteID = {0} and MessageType = {1} and ID > {2}",
                dbMgr.SiteID, SDMS_PUBLIC_MESSAGE_TYPE, m_nLastReadID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
                    WriteLastID(dbMgr, nLastReadID);

                messages.Clear();
            }
        }

        private bool SendMessages(List<Message> messages)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(SOPWebServer.SDMSCommandType.SDMS_PUBLIC_MESSAGE);
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

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            Client.SDMSServer.Instance.SendClientData(SOPWebServer.Header.SDMS_COMMAND, bytes, SOPWebServer.ClientType.SDMS, -1);
            return true;
        }
    }
}
