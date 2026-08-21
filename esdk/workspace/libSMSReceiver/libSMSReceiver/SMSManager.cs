using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Threading;
using System.Collections;

namespace libSMSReceiver
{
    public class SMSManager
    {
        private class MessageParam
        {
            private IEventReceiver m_receiver = null;
            public IEventReceiver EventReceiver
            {
                get { return m_receiver; }
                set { m_receiver = value; }
            }

            private WebDBManager m_dbMgr = null;
            public WebDBManager DBManager
            {
                get { return m_dbMgr; }
                set { m_dbMgr = value; }
            }
        }

        // Key : Message ID + "_" + TimeString(YYYYMMDDhhmmss)
        //private Dictionary<string, Message> m_dicReadMessages = new Dictionary<string, Message>();
        // 메시지 확인 간격(milli seconds)
        private int m_nInterval = 1000;
        private int m_nLastMonth = -1;
        private bool m_runThread = false;

        /// <summary>
        /// 메시지 확인 간격(milli seconds)
        /// </summary>
        public int Interval
        {
            get { return m_nInterval; }
            set { m_nInterval = value; }
        }

        /// <summary>
        /// 메시지 수신 시작
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="strIP"></param>
        /// <param name="mySQL"></param>
        public void Start(IEventReceiver receiver, string strIP, bool mySQL)
        {
            if (m_runThread || receiver == null)
                return;

            m_runThread = true;

            string strDBType = mySQL ? "mysql" : "sqlserver";

            WebDBManager dbMgr = new WebDBManager("UNE_SMS_RECEIVE", strDBType, 10000);
            dbMgr.WebServerURL = "http://" + strIP + ":8080/SOP";
            dbMgr.DatabaseHost = strIP;

            MessageParam param = new MessageParam();
            param.DBManager = dbMgr;
            param.EventReceiver = receiver;

            Thread t = new Thread(new ParameterizedThreadStart(ReadThread));
            t.Start(param);
        }

        /// <summary>
        /// 메시지 수신 종료
        /// </summary>
        public void Stop()
        {
            m_runThread = false;
        }

        private void ReadThread(object arg)
        {
            if (arg != null && arg is MessageParam)
            {
                MessageParam param = (MessageParam)arg;
                WebDBManager dbMgr = param.DBManager;
                IEventReceiver receiver = param.EventReceiver;

                List<Message> messages = new List<Message>();

                while (m_runThread)
                {
                    int month = DateTime.Now.Month;

                    ReadSMS(messages, dbMgr, month);
                    ReadMMS(messages, dbMgr, month);

                    if (messages.Count > 0)
                    {
                        if (receiver.OnReceive(messages))
                        {
                            // DB에 읽은 표시를 한다.
                            SetReadFlags(dbMgr, messages);
                        }

                        messages.Clear();
                    }

                    m_nLastMonth = month;

                    if (m_nInterval > 0)
                        Thread.Sleep(m_nInterval);
                }
            }
        }

        private int ReadMMS(List<Message> messages, WebDBManager dbMgr, int month)
        {
            string strTable = string.Format("T_MMS_HIST_RV_{0:00}", month);

            string strSQL = "Select MSG_KEY, IN_TIME, CALLER_NO, SUBJECT, MMS_MSG from " + strTable + " where READ_FLAG = 0";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return 0;

            int nAddCount = 0;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTime = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2]);
                string strSubject = WebDBManager.GetStringField(arrResult[i + 3]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4]);

                // T_MMS_HIST_RV_XX Table의 MMS_MSG는 NULL 허용이지만 이미지를 사용하지 않기 때문에 Text가 없는 데이터는 불필요함
                if (id == null || strTime == null || strPhoneNumber == null || strMessage == null)
                    continue;

                Message message = new Message(id.Data, strTime, strSubject, strMessage, strPhoneNumber, true);
                messages.Add(message);

                /*string strKey = MakeKey(message);
                m_dicReadMessages[strKey] = message;*/
                nAddCount++;
            }

            // 달이 바뀌는 순간에 지난달의 놓친 메시지가 있는지 검사
            if (m_nLastMonth > 0)
            {
                if (m_nLastMonth != month)
                    nAddCount += ReadMMS(messages, dbMgr, m_nLastMonth);
            }

            return nAddCount;
        }

        private int ReadSMS(List<Message> messages, WebDBManager dbMgr, int month)
        {
            string strTable = string.Format("T_SMS_HIST_RV_{0:00}", month);

            string strSQL = "Select MSG_KEY, IN_TIME, CALLER_NO, SMS_MSG from " + strTable + " where READ_FLAG = 0";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return 0;

            int nAddCount = 0;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTime = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strTime == null || strPhoneNumber == null || strMessage == null)
                    continue;

                Message message = new Message(id.Data, strTime,"", strMessage, strPhoneNumber, false);
                messages.Add(message);

                /*string strKey = MakeKey(message);
                m_dicReadMessages[strKey] = message;*/
                nAddCount++;
            }

            // 달이 바뀌는 순간에 지난달의 놓친 메시지가 있는지 검사
            if (m_nLastMonth > 0)
            {
                if (m_nLastMonth != month)
                    nAddCount += ReadSMS(messages, dbMgr, m_nLastMonth);
            }

            return nAddCount;
        }

        private string MakeKey(Message message)
        {
            return message.ID.ToString() + "_" + message.TimeStamp;
        }

        private string GetTableName(Message message)
        {
            if (message.TimeStamp.Length < 6)
                return "";

            string strMonth = message.TimeStamp.Substring(4, 2);

            if (message.IsMMS)
            {
                return "T_MMS_HIST_RV_" + strMonth;
            }

            return "T_SMS_HIST_RV_" + strMonth;
        }

        // messages에 있는 데이터들을 DB에 읽은 값으로 수정시킨다.
        private void SetReadFlags(WebDBManager dbMgr, List<Message> messages)
        {
            // Key : Table 이름, Value : where 절
            Dictionary<string, string> dicTableConditions = new Dictionary<string, string>();
            // Key : Table 이름, Value : 조건문 개수
            Dictionary<string, int> dicTableConditionCount = new Dictionary<string, int>();

            string strCondition = "";
            int nCount = 0;

            foreach (Message message in messages)
            {
                string strTableName = GetTableName(message);

                if (dicTableConditionCount.TryGetValue(strTableName, out nCount) == false)
                    nCount = 0;

                if (dicTableConditions.TryGetValue(strTableName, out strCondition) == false)
                    strCondition = "";

                string str = "(MSG_KEY = " + message.ID.ToString() + " and IN_TIME = '" + message.TimeStamp + "')";

                if (strCondition.Length == 0)
                    strCondition = str;
                else
                    strCondition += " or " + str;

                // Query가 너무 길어지면 성능에 영향을 줄수 있으므로 적절히 잘라서 처리한다.
                if (++nCount >= 100)
                {
                    SetReadFlags(dbMgr, strTableName, strCondition);

                    dicTableConditions.Remove(strTableName);
                    dicTableConditionCount.Remove(strTableName);
                }
                else
                {
                    dicTableConditions[strTableName] = strCondition;
                    dicTableConditionCount[strTableName] = nCount;
                }
            }

            foreach (KeyValuePair<string, string> pair in dicTableConditions)
            {
                SetReadFlags(dbMgr, pair.Key, pair.Value);
            }
        }

        private void SetReadFlags(WebDBManager dbMgr, string strTableName, string strCondition)
        {
            string strSQL = "Update " + strTableName + " set READ_FLAG = 1 where " + strCondition;
            dbMgr.GetResultData(strSQL, 0);
        }
    }

    public class Message
    {
        private int m_nID = -1;
        private string m_timeStamp;
        private string m_strSubject = "";
        private string m_strMessage = "";
        private string m_phoneNumber = "";
        private bool m_isMMS = false;

        public int ID
        {
            get { return m_nID; }
        }

        /// <summary>
        /// 메시지 수신 시각
        /// </summary>
        public string TimeStamp
        {
            get { return m_timeStamp; }
        }

        /// <summary>
        /// 제목
        /// </summary>
        public string Subject
        {
            get { return m_strSubject; }
        }

        public string MessageText
        {
            get { return m_strMessage; }
        }

        /// <summary>
        /// 발신인 전화번호
        /// </summary>
        public string PhoneNumber
        {
            get { return m_phoneNumber; }
        }

        public bool IsMMS
        {
            get { return m_isMMS; }
        }

        public Message(int id, string timeStamp, string subject, string message, string phoneNumber, bool isMMS)
        {
            m_nID = id;
            m_timeStamp = timeStamp;
            m_strSubject = subject;
            m_strMessage = message;
            m_phoneNumber = phoneNumber;
            m_isMMS = isMMS;
        }
    }

    public interface IEventReceiver
    {
        /// <summary>
        /// 새로운 메시지가 수신되면 호출된다.
        /// </summary>
        /// <param name="messages">새로 수신된 메시지 리스트</param>
        /// <returns>
        /// true이면 수신된 메시지가 제대로 처리되었다. 이때, 처리된 메시지들은 다시 읽혀지지 않는다.
        /// 이 값이 false이면 수신된 메시지가 EventReceiver에서 제대로 처리되지 못한 것이며, 다음번 Thread 호출시 메시지가 다시 읽혀지게 된다.
        /// </returns>
        bool OnReceive(List<Message> messages);
    }
}
