using System;
using System.Collections.Generic;
using System.Linq;
using SOPWebClient;
using System.Collections;
using DBUtility2;
using System.Threading;
using System.Collections.Concurrent;

namespace PersonalSOP.Network
{
    public class NetworkWebManager
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkWebManager m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private DateTime m_dtLastSendMessage = new DateTime();

            public PostBox PostBox
            {
                get { return m_postBox; }
                set
                {
                    m_postBox = value;
                }
            }

            public int ClientType
            {
                get { return m_nClientType; }
            }

            public int ClientSubType
            {
                get { return m_nClientSubType; }
            }

            public bool IsConnected
            {
                get { return m_isConnected; }
                set
                {
                    if (m_isConnected != value)
                    {
                        m_isConnected = value;
                    }
                }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public PostMan(NetworkWebManager owner, int nClientType, int nClientSubType)
            {
                m_owner = owner;
                m_nClientType = nClientType;
                m_nClientSubType = nClientSubType;
            }

            public void OnMessage(int header, byte[] messages)
            {
                if (m_owner != null)
                    m_owner.OnMessage(header, messages, this);
            }

            public bool SendMessage(int header, byte[] messages)
            {
                if (m_postBox == null || m_isConnected == false)
                {
                    m_isConnected = false;
                }
                else
                {
                    bool closeConnection;
                    bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                    if (closeConnection)
                    {
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        m_isConnected = false;
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postManFire = null;
        private PostMan m_postManEtc = null;
        private PostMan m_postManSOPSimulator = null;
        private List<PostMan> m_postManList = new List<PostMan>();
        private bool m_shutdownThread = false;
        private WebDBManager m_dbMgr = null;
        public WebDBManager DBMgr
        {
            get { return m_dbMgr; }
        }

        private ConcurrentQueue<Message> m_queueMessages = new ConcurrentQueue<Message>();

        private static NetworkWebManager m_instance = null;

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        public static void InitInstance()
        {
            m_instance = new NetworkWebManager();
        }

        private NetworkWebManager()
        {
            SetDBManager();
            int nPort = ReadServerPort();

            m_postManFire = new PostMan(this, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);
            m_postManEtc = new PostMan(this, SOPWebServer.ClientType.ETC, SOPWebServer.ClientSubType.SIMULATOR);
            m_postManSOPSimulator = new PostMan(this, SOPWebServer.ClientType.SOP_SIMULATOR, SOPWebServer.ClientSubType.SIMULATOR);

            SetPostBox(m_postManFire, nPort);
            SetPostBox(m_postManEtc, nPort);
            SetPostBox(m_postManSOPSimulator, nPort);

            m_postManList.Add(m_postManFire);
            m_postManList.Add(m_postManEtc);
            m_postManList.Add(m_postManSOPSimulator);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        private void SetDBManager()
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString();
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString();
            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString();
            string strDBType = System.Configuration.ConfigurationManager.AppSettings["dbtype"].ToString();

            int nDBType;

            if (int.TryParse(strDBType, out nDBType) == false)
                return;

            int nSiteID = 0;

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return;

            WebDBManager dbMgr = new WebDBManager(nSiteID);
            dbMgr.WebServerURL = strWebServerURL;
            dbMgr.DatabaseName = strDBName;
            dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

            m_dbMgr = dbMgr;
        }

        public void OnMessage(int header, byte[] messages, object postMan)
        {
            if (postMan != null && postMan is PostMan)
            {
                ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

                RecvLog(header, messages);

                if (header == SOPWebServer.Header.CLOSE_CONNECTION)
                {
                    ((PostMan)postMan).IsConnected = false;
                }
                else if (header == SOPWebServer.Header.ARE_YOU_THERE)
                {
                    ((PostMan)postMan).SendMessage(SOPWebServer.Header.I_AM_HERE, null);
                }
            }
        }

        private void ConnectionThread()
        {
            List<Message> unprocessedMessages = new List<Message>();

            while (m_shutdownThread == false)
            {
                foreach (PostMan postMan in m_postManList)
                {
                    if (postMan.IsConnected == false)
                    {
                        int nPort = ReadServerPort();

                        if (postMan.Port != nPort)
                            SetPostBox(postMan, nPort);

                        if (postMan.PostBox != null)
                        {
                            if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                            {
                                postMan.IsConnected = true;
                            }
                        }
                    }
                    else
                    {
                        TimeSpan span = DateTime.Now - postMan.LastSendMessageTime;

                        // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                        if (span.TotalSeconds > 3.0)
                        {
                            // 접속이 유지되고 있는지 확인한다.
                            postMan.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                        }
                    }
                }

                while (m_queueMessages.Count > 0)
                {
                    Message message;
                    bool processed = false;

                    if (m_queueMessages.TryDequeue(out message) == false)
                        break;

                    if (message.SendToSOPSimulator() && m_postManSOPSimulator.IsConnected)
                    {
                        if (SendMessage(message, m_postManSOPSimulator))
                            processed = true;
                    }

                    if (message.SendToEtc() && m_postManEtc.IsConnected)
                    {
                        if (SendMessage(message, m_postManEtc))
                            processed = true;
                    }

                    if (message.SendToFire() && m_postManFire.IsConnected)
                    {
                        if (SendMessage(message, m_postManFire))
                            processed = true;
                    }
                    if (processed == false)
                        unprocessedMessages.Add(message);
                }

                // 처리되지 않은 메시지는 다시 큐에 넣는다.
                foreach (Message message in unprocessedMessages)
                {
                    m_queueMessages.Enqueue(message);
                }

                unprocessedMessages.Clear();

                Thread.Sleep(1000);
            }
        }

        private bool SendMessage(Message message, PostMan postMan)
        {
            byte[] bytes = message.GetBytes();
            return postMan.SendMessage(message.GetHeader(), bytes);
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private void SetPostBox(PostMan postMan, int nPort)
        {
            if (nPort > 0)
            {
                PostBox postBox = new PostBox();
                postBox.WebServerURL = m_dbMgr.WebServerURL;
                postBox.PostMan = postMan;
                postMan.PostBox = postBox;

                postMan.Port = nPort;
                postBox.Port = nPort;
            }
        }

        private bool SendMessage(int header, byte[] messages, PostMan postMan)
        {
            if (postMan.IsConnected)
            {
                SendLog(header, messages);
                return postMan.SendMessage(header, messages);
            }

            return false;
        }

        public void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (header != SOPWebServer.Header.ARE_YOU_THERE &&
                header != SOPWebServer.Header.I_AM_HERE)
            {
                string strLog = "";

                if (bytes == null)
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
                }
                else
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    strLog += strBytes;
                }

                WriteLog(strLog);
            }
        }

        public void WriteLog(string strLog)
        {
            //if (m_logger != null)
            //    m_logger.Write(strLog);
        }

        public void Close()
        {
            foreach (PostMan postMan in m_postManList)
            {
                if (postMan.IsConnected)
                {
                    // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                    // 실패하더라도 상관없다.
                    bool closeConnection;
                    postMan.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                    postMan.IsConnected = false;
                }
            }

            m_shutdownThread = true;
        }

        public void AddMessage(Message message)
        {
            m_queueMessages.Enqueue(message);
        }

        public void AddInjuryMessage(string strLocation, string strDisasterInfo, bool isFireSOP)
        {
            int nActionStepHistoryID = GetActionStepHistoryID(isFireSOP);

            if (nActionStepHistoryID < 0)
                return;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            int nID = GetMaxTableID("ActionStepHistoryMessage") + 1;

            string strSQL = "Insert into ActionStepHistoryMessage (ID, ActionStepHistoryID, UserID, TimeStamp, Image, Title, Message) ";
            strSQL += string.Format("Values ({4}, {0}, NULL, '{1}', NULL, '{2}', '{3}')",
                nActionStepHistoryID, strTime, "인명피해 발생 : " + strLocation, strDisasterInfo, nID);

            m_dbMgr.GetResultData(strSQL);

            int nProcessID = GetProcessID(nActionStepHistoryID, "인명피해");

            if (nProcessID < 0)
                return;

            AddMessage(new InjuryMessage(nActionStepHistoryID, nProcessID));
            Controllers.SOPBulletinController.nCurrentIndex[nActionStepHistoryID] = nID;
        }

        public void InitLostArticle()
        {
            if (m_dbMgr.GetResultData("Delete from ActionStepHistoryMessage") == null)
                return;

            int nBeginHistoryMessageID = 0;

            /*int nBeginHistoryMessageID = GetMaxTableID("ActionStepHistoryMessage");

            if (nBeginHistoryMessageID < 0)
                return;
            else
                nBeginHistoryMessageID++;*/

            int nBeginArticleID = GetMaxTableID("LostArticle");

            if (nBeginArticleID < 0)
                return;
            else
                nBeginArticleID++;

            string strSQL = "Select ID, DeadCount, InjuryCount, LostCount, TankTemperature from LostStatus where ID = (Select max(ID) from LostStatus)";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            strSQL = string.Format("Update LostStatus set BeginArticleID = {0}, BeginHistoryMessageID = {1}, DeadCount = 0, InjuryCount = 0, LostCount = 0, TankTemperature = NULL where ID = {2}",
                nBeginArticleID, nBeginHistoryMessageID, id.Data);
            m_dbMgr.GetResultData(strSQL);
        }

        public void AddLostArticle(string deadCount, string injuryCount, string lostCount, string tankTemp)
        {
            int dead = 0;
            int.TryParse(deadCount, out dead);
            int injury = 0;
            int.TryParse(injuryCount, out injury);
            int lost = 0;
            int.TryParse(lostCount, out lost);
            double temp;
            double.TryParse(tankTemp, out temp);

            string strSQL = "Select ID, DeadCount, InjuryCount, LostCount, TankTemperature from LostStatus where ID = (Select max(ID) from LostStatus)";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
            {
                int nID = GetMaxTableID("LostStatus") + 1;

                int nBeginHistoryMessageID = GetMaxTableID("ActionStepHistoryMessage");

                if (nBeginHistoryMessageID < 0)
                    return;
                else
                    nBeginHistoryMessageID++;

                strSQL = "Insert into LostStatus (ID, BeginArticleID, BeginHistoryMessageID, DeadCount, InjuryCount, LostCount, TankTemperature) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    nID, 1, nBeginHistoryMessageID, dead, injury, lost, temp);

                m_dbMgr.GetResultData(strSQL);
            }
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (deadCount == null)
                    dead = WebDBManager.GetIntField(arrResult[1].ToString(), 0);
                if (injuryCount == null)
                    injury = WebDBManager.GetIntField(arrResult[2].ToString(), 0);
                if (lostCount == null)
                    lost = WebDBManager.GetIntField(arrResult[3].ToString(), 0);
                if (tankTemp == null)
                    temp = WebDBManager.GetIntField(arrResult[4].ToString(), 0);

                strSQL = string.Format("Update LostStatus set DeadCount = {0}, InjuryCount = {1}, LostCount = {2}, TankTemperature = {3} where ID = {4}",
                  dead, injury, lost, temp, id.Data);

                m_dbMgr.GetResultData(strSQL);
            }    
        }

        private int GetProcessID(int nActionStepHistoryID, string strTag)
        {
            string strSQL = "Select p.ID ";
            strSQL += "from ActionStepHistory as ash, ActionStep as step, StepMember as member, Process as p ";
            strSQL += string.Format("where ash.ActionStepID = step.ID and member.ActionStepID = step.ID and p.StepMemberID = member.ID and ash.ID = {0} and p.ComponentID like '%{1}%'", nActionStepHistoryID, strTag);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;
                else
                    return id.Data;
            }

            return -1;
        }

        private int GetActionStepHistoryID(bool isFireSOP)
        {
            string strSQL = "Select ash.ID, ash.ActionStepID, dc.CategoryName ";
            strSQL += "from ActionStepHistory as ash, ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID ";
            strSQL += "and ash.EndTime is NULL and ash.CancelTime is null order by ash.ID desc";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strCategoryName = WebDBManager.GetStringField(arrResult[i + 2]);

                if (actionStepHistoryID == null || actionStepID == null || strCategoryName == null)
                    continue;

                if (isFireSOP)
                {
                    if (strCategoryName == "화재")
                        return actionStepHistoryID.Data;
                }
                else
                {
                    if (strCategoryName != "화재")
                        return actionStepHistoryID.Data;
                }
            }

            return -1;
        }

        public int GetMaxTableID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            return id.Data;
        }
    }
}