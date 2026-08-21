using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;

namespace SDMSServer
{
	public class ReciverManager
	{

		private static ReciverManager m_Instance = null;
		public static ReciverManager Instance
		{
			get 
			{
				if (m_Instance == null)
					m_Instance = new ReciverManager();
				return m_Instance;
			}

			set { m_Instance = value; }
		}

        private int m_nSiteID = 1;
		protected ReciverManager()
		{
            m_nSiteID = NetworkServer.Instance.SiteID;

		}

		private ArrayList m_arReciverList = new ArrayList();
		
		private Dictionary<int, Reciver> m_dicReciverList = new Dictionary<int, Reciver>();
		public Dictionary<int, Reciver> DicReciverList
		{
			get { return m_dicReciverList; }
			set { m_dicReciverList = value; }
		}

		public ArrayList GetReciverList()
		{
			return m_arReciverList;
		}

		public void UpdateState(int nReciverID, bool bConnected)
		{
			DateTime dtNow = DateTime.Now;

			if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
			{
				Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
				if( reciver != null)
				{
					if (reciver.State != (bConnected == true ? 1 : 0))
					{
						reciver.State = (bConnected == true ? 1 : 0);

						reciver.UpdateTime = dtNow;

						string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);
                        string szUpdate = string.Format("UPDATE SensorServerInfo SET ConnectionState ={0} , ConnectionTime = '{1}' WHERE ID = {2} AND SiteID = {3}", 
                                    reciver.State, strDateTimeField, reciver.ID, m_nSiteID);
						WebDBManager m_dbMgr = NetworkServer.Instance.DBManager;
						m_dbMgr.GetResultData(szUpdate, 0);			           
					}				
				}
				
			}
		}

		public void LoadReciverList()
		{
			WebDBManager m_dbMgr = NetworkServer.Instance.DBManager;
			
            //string strSQL = "select ID,Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, Description, ConnectionState, ConnectionTime from SensorServerInfo";

            string szText = "SELECT ID,Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, Description, ConnectionState, ConnectionTime " +
                            " FROM SensorServerInfo WHERE SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 11; i += 12)
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

				if (!m_dicReciverList.ContainsKey(nID))
				{
					Reciver reciver = new Reciver();
					reciver.ID = nID;
					reciver.Place = strPlace;
					reciver.Address = strIP;
					reciver.MacAddress = strMac;
					reciver.Port = nPort;
					reciver.Mode = nMode;
					reciver.FlowCtrl = nFlow;
					reciver.Timeout = nTimeout;
					reciver.BuadRate = nBuadrate;
					reciver.State = 0;

					m_arReciverList.Add(reciver);
					m_dicReciverList.Add(nID, reciver);
				}
				else
				{
					Reciver reciver = m_dicReciverList[nID];
					reciver.State = 0;
				}				
			}
		}
	}

	public class Reciver
	{
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

		private int m_nState = -1;

		public int State
		{
			get { return m_nState; }
			set { m_nState = value; }
		}

		private DateTime m_dtUpdateTime;
		public System.DateTime UpdateTime
		{
			get { return m_dtUpdateTime; }
			set { m_dtUpdateTime = value; }
		}
	}
}
