using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using DBUtility2;

namespace ServerProcess.Data
{
    // 수신반 정보
    public enum ReceiverType
    {
        UNKNOWN = 0,
        FIRE_RECEIVER = 1,
        PSM_RECEIVER = 2,
        SVMS_RECEIVER = 3,
        ASIN_RECEIVER = 4,
        EMPOLL_RECEIVER = 5,
        ACCESS_RECEIVER = 6,
        SECOM_RECEIVER = 7
    }

    public class ReceiverManager
    {
        private static ReceiverManager m_Instance = new ReceiverManager();

        public static ReceiverManager Instance
        {
            get { return m_Instance; }
        }

        protected ReceiverManager()
        {
        }

        private List<Receiver> m_receivers = new List<Receiver>();
        private Dictionary<int, Receiver> m_dicReciverList = new Dictionary<int, Receiver>();
        
        public List<Receiver> GetReciverList()
        {
            return m_receivers;
        }

        public Receiver GetReceiver(int nReceiverID)
        {
            Receiver receiver;

            if (m_dicReciverList.TryGetValue(nReceiverID, out receiver) == false)
                return null;

            return receiver;
        }

        public void UpdateState(int nReceiverID, bool isConnected, DirectDBManager dbMgr)
        {
            Receiver receiver;

            if (m_dicReciverList.TryGetValue(nReceiverID, out receiver) == false)
                return;

            if (receiver.State == Receiver.ConnectionState.Connected && isConnected == true)
                return;
            else if (receiver.State == Receiver.ConnectionState.NotConnected && isConnected == false)
                return;

            bool bReceivedPoll = receiver.RecivedPoll == 1 ? true : false;

            if (isConnected == false)
                bReceivedPoll = false;

            UpdateState(receiver, isConnected, bReceivedPoll, dbMgr);
        }

        public void UpdateState(Receiver receiver, bool bConnected, bool bRecivedPoll, DirectDBManager dbMgr)
        {
            DateTime dtNow = DateTime.Now;

            if (receiver != null)
            {
                // PSM 수신반은 상태변화가 없으므로 체크하면 안됨 
                // 2016-04-29 skkim
                //if (reciver.State != (bConnected == true ? 1 : 0))
                //{

                int nState = (bConnected == true ? 1 : 0);
                receiver.State = Receiver.ToConnectionState(nState);
                receiver.RecivedPoll = bRecivedPoll == true ? 10 : 0;
                nState += receiver.RecivedPoll;
                receiver.UpdateTime = dtNow;

                string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);
                string szUpdate = string.Format("UPDATE SensorServerInfo SET ConnectionState ={0} , ConnectionTime = '{1}' WHERE ID = {2} AND SiteID = {3}",
                            nState, strDateTimeField, receiver.ID, dbMgr.SiteID);

                dbMgr.GetResultData(szUpdate);
                //}				
            }
        }

        public void Initialize(DirectDBManager dbMgr)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            LoadReceiverList(dbMgr);
            InitReciverState(dbMgr);

            dbMgr.Close();
        }

        public void LoadReceiverList(DirectDBManager dbMgr)
        {
            m_receivers.Clear();

            string szText = "SELECT ID,Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, Description, ConnectionState, ConnectionTime, ReciverType " +
                            " FROM SensorServerInfo WHERE SiteID = {0}";

            string strSQL = string.Format(szText, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 12; i += 13)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strPlace = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strIP = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strMac = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nBuadrate = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                int nMode = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nFlow = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nPort = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nTimeout = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 9], "");

                int nState = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0);
                DateTime dtTime = WebDBManager.GetDateTimeField(arrResult[i + 11].ToString(), DateTime.Now);
                int nReceiverType = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0);

                if (!m_dicReciverList.ContainsKey(nID))
                {
                    Receiver reciver = new Receiver();
                    reciver.ID = nID;
                    reciver.Place = strPlace;
                    reciver.Address = strIP;
                    reciver.MacAddress = strMac;
                    reciver.Port = nPort;
                    reciver.Mode = nMode;
                    reciver.FlowCtrl = nFlow;
                    reciver.Timeout = nTimeout;
                    reciver.BuadRate = nBuadrate;
                    reciver.State = Receiver.ConnectionState.NotConnected;
                    reciver.ReceiverType = Receiver.ToReceiverType(nReceiverType);

                    m_receivers.Add(reciver);
                    m_dicReciverList.Add(nID, reciver);
                }
                else
                {
                    Receiver reciver = m_dicReciverList[nID];
                    reciver.State = Receiver.ConnectionState.NotConnected;
                }
            }
        }

        public void InitReciverState(DirectDBManager dbMgr)
        {
            DateTime dtNow = DateTime.Now;

            foreach (Receiver receiver in m_receivers)
            {
                int nState = 0;
                receiver.State = Receiver.ToConnectionState(nState);
                receiver.RecivedPoll = 0;
                nState += receiver.RecivedPoll;
                receiver.UpdateTime = dtNow;

                string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);
                string szUpdate = string.Format("UPDATE SensorServerInfo SET ConnectionState ={0}, ConnectionTime = '{1}' WHERE ID = {2} AND SiteID = {3}",
                            nState, strDateTimeField, receiver.ID, dbMgr.SiteID);

                dbMgr.GetResultData(szUpdate);
            }
        }

        public void InitReciverState(DirectDBManager dbMgr, List<int> receiverIDs)
        {
            DateTime dtNow = DateTime.Now;

            foreach (int nID in receiverIDs)
            {
                Receiver receiver = GetReceiver(nID);

                if (receiver != null)
                {
                    int nState = 0;
                    receiver.State = Receiver.ToConnectionState(nState);
                    receiver.RecivedPoll = 0;
                    nState += receiver.RecivedPoll;
                    receiver.UpdateTime = dtNow;

                    string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);
                    string szUpdate = string.Format("UPDATE SensorServerInfo SET ConnectionState ={0}, ConnectionTime = '{1}' WHERE ID = {2} AND SiteID = {3}",
                                nState, strDateTimeField, receiver.ID, dbMgr.SiteID);

                    dbMgr.GetResultData(szUpdate);
                }
            }
        }
    }

    public class Receiver
    {
        public enum ConnectionState { Unknown = -1, NotConnected = 0, Connected };

        private int m_nID;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_strAddress;
        public string Address
        {
            get { return m_strAddress; }
            set { m_strAddress = value; }
        }

        private int m_nPort;
        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        private int m_nBuadRate = 9600;
        public int BuadRate
        {
            get { return m_nBuadRate; }
            set { m_nBuadRate = value; }
        }

        private string m_nMacAddress = "";
        public string MacAddress
        {
            get { return m_nMacAddress; }
            set { m_nMacAddress = value; }
        }

        private int m_nMode = 3;
        public int Mode
        {
            get { return m_nMode; }
            set { m_nMode = value; }
        }

        private int m_nFlowCtrl = 3;
        public int FlowCtrl
        {
            get { return m_nFlowCtrl; }
            set { m_nFlowCtrl = value; }
        }

        private string m_szName = "";
        public string Place
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        public override string ToString()
        {
            return (m_nID.ToString() + ". " + m_szName);
        }

        private int m_nTimeout = 3000;
        public int Timeout
        {
            get { return m_nTimeout; }
            set { m_nTimeout = value; }
        }

        private ConnectionState m_nState = ConnectionState.Unknown;

        public ConnectionState State
        {
            get { return m_nState; }
            set { m_nState = value; }
        }

        private int m_nRecivedPoll = 0;

        public int RecivedPoll
        {
            get { return m_nRecivedPoll; }
            set { m_nRecivedPoll = value; }
        }

        private DateTime m_dtUpdateTime;
        public System.DateTime UpdateTime
        {
            get { return m_dtUpdateTime; }
            set { m_dtUpdateTime = value; }
        }

        private ReceiverType m_reciverType = ReceiverType.FIRE_RECEIVER;
        public ReceiverType ReceiverType
        {
            get { return m_reciverType; }
            set { m_reciverType = value; }
        }

        private static Dictionary<int, ReceiverType> m_dicReceiverType = null;
        public static ReceiverType ToReceiverType(int nReceiverType)
        {
            if (m_dicReceiverType == null)
            {
                m_dicReceiverType = new Dictionary<int, ReceiverType>();

                foreach (ReceiverType type in Enum.GetValues(typeof(ReceiverType)))
                {
                    m_dicReceiverType[(int)type] = type;
                }
            }

            ReceiverType rType;

            if (m_dicReceiverType.TryGetValue(nReceiverType, out rType))
                return rType;

            return ReceiverType.UNKNOWN;
        }

        private static Dictionary<int, ConnectionState> m_dicConnectionState = null;
        public static ConnectionState ToConnectionState(int nState)
        {
            if (m_dicConnectionState == null)
            {
                m_dicConnectionState = new Dictionary<int, ConnectionState>();

                foreach (ConnectionState state in Enum.GetValues(typeof(ConnectionState)))
                {
                    m_dicConnectionState[(int)state] = state;
                }
            }

            ConnectionState cSype;

            if (m_dicConnectionState.TryGetValue(nState, out cSype))
                return cSype;

            return ConnectionState.Unknown;
        }
    }
}