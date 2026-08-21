using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrainingLink.Data
{
    public class MessageManager
    {
        private FormMain m_form = null;
        WebDBManager m_dbMgr = null;

        private Thread m_MessageThread = null;
        private bool m_shutdownThread = false;
        public void Shutdown()
        {
            m_shutdownThread = true;
            m_MessageThread.Abort();
        }

        private int m_nLastMessageID = 0;

        private string m_strSearch = "";
        public string Search
        {
            get { return m_strSearch; }
            set
            {
                m_strSearch = value;
                m_nLastMessageID = 0;
            }
        }

        private int m_nActionStepHistoryID = -1;

        public MessageManager(FormMain form)
        {
            m_form = form;
            m_dbMgr = m_form.DBManager;

            m_MessageThread = new Thread(new ThreadStart(MessageThread));
            m_MessageThread.Name = "Message.Reader";
        }

        private void MessageThread()
        {
            // 상황이 없을 때 화면 초기화 및 메시지 초기화
            int nActionStepID = -1;
            int nActionStepHistoryID = -1;

            bool bChk = LoadActionStepIDs(out nActionStepHistoryID, out nActionStepID);
            if (nActionStepHistoryID == -1 && nActionStepID == -1)
            {   // 상황이 없을 경우
                m_form.ClearThreadMessage();

                // 메시지 타입 확인처리
                UpdateMessageCheck();
            }

            while (!m_shutdownThread)
            {
                // 새로운 메시지 읽기
                Dictionary<int, MessageData> dicMessage = LoadThreadMessage();

                if (dicMessage == null)
                    continue;

                // 화면에 표시
                foreach (KeyValuePair<int, MessageData> pair in dicMessage)
                {
                    MessageData message = pair.Value;
                    m_form.ShowThreadMessage(message);
                }

                // 상황 종료시 화면 초기화
                nActionStepID = -1;
                nActionStepHistoryID = -1;

                bChk = LoadActionStepIDs(out nActionStepHistoryID, out nActionStepID);

                if (nActionStepHistoryID == -1 && m_nActionStepHistoryID != -1)
                {   // 상황이 발생했다가 상황이 종료된 경우
                    m_nActionStepHistoryID = -1;
                    m_form.ClearThreadMessage();

                    // 메시지 타입 확인처리
                    UpdateMessageCheck();
                }
                else if (nActionStepHistoryID != -1)
                {
                    m_nActionStepHistoryID = nActionStepHistoryID;
                }

   

                Thread.Sleep(5 * 1000);
            }
        }

        public bool UpdateMessageCheck()
        {
            string strSQL = string.Format("UPDATE LinkMessage SET IsCheck = 1  Where IsCheck = 0");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        private bool LoadActionStepIDs(out int nActionStepHistoryID, out int nActionStepID)
        {
            nActionStepHistoryID = -1;
            nActionStepID = -1;

            string strSQL = string.Format("SELECT ID, ActionStepID FROM actionstephistory where EndTime IS NULL AND CancelTime IS NULL");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                nActionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
            }

            return true;
        }

        public void StartThread()
        {
            m_MessageThread.Start();
        }

        public Dictionary<int, MessageData> LoadMessage()
        {
            Dictionary<int, MessageData> dicMessage = new Dictionary<int, MessageData>();

            string strSQL = string.Format("SELECT ID, Sender, Receiver, Message, CreateDate FROM LinkMessage Where IsCheck = 0");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return dicMessage;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSender = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strReceiver = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strMessage = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtCreateDate = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);

                if (m_nLastMessageID < nID)
                    m_nLastMessageID = nID;

                MessageData message = new MessageData();
                message.ID = nID;
                message.Sender = strSender;
                message.Receiver = strReceiver;
                message.Message = strMessage;
                message.CreateTime = dtCreateDate;

                dicMessage[nID] = message;
            }

            return dicMessage;
        }

        public Dictionary<int, MessageData> LoadSearchMessage()
        {
            Dictionary<int, MessageData> dicMessage = new Dictionary<int, MessageData>();

            string strSQL = string.Format("SELECT ID, Sender, Receiver, Message, CreateDate FROM LinkMessage Where IsCheck = 0 " +
                "AND (Sender LIKE '%" + m_strSearch + "%' OR Receiver LIKE '%" + m_strSearch + "%' OR Message LIKE '%" + m_strSearch + "%')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return dicMessage;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSender = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strReceiver = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strMessage = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtCreateDate = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);

                if (m_nLastMessageID < nID)
                    m_nLastMessageID = nID;

                MessageData message = new MessageData();
                message.ID = nID;
                message.Sender = strSender;
                message.Receiver = strReceiver;
                message.Message = strMessage;
                message.CreateTime = dtCreateDate;

                dicMessage[nID] = message;
            }

            return dicMessage;
        }

        private Dictionary<int, MessageData> LoadThreadMessage()
        {
            Dictionary<int, MessageData> dicMessage = new Dictionary<int, MessageData>();

            string strSQL = string.Format("SELECT ID, Sender, Receiver, Message, CreateDate FROM LinkMessage Where IsCheck = 0 AND ID > " + m_nLastMessageID +
                " AND (Sender LIKE '%" + m_strSearch + "%' OR Receiver LIKE '%" + m_strSearch + "%' OR Message LIKE '%" + m_strSearch + "%')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return dicMessage;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSender = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strReceiver = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strMessage = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtCreateDate = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);

                if (m_nLastMessageID < nID)
                    m_nLastMessageID = nID;

                MessageData message = new MessageData();
                message.ID = nID;
                message.Sender = strSender;
                message.Receiver = strReceiver;
                message.Message = strMessage;
                message.CreateTime = dtCreateDate;

                dicMessage[nID] = message;
            }

            return dicMessage;
        }
    }

    
}
