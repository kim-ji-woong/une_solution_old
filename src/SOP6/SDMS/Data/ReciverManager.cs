using System;
using System.Collections;
using System.Collections.Generic;
using DBUtility2;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
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

		public ReciverManager()
		{
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

		public void UpdateState(int nReciverID, bool bConnected, bool bRecivePoll)
		{
			DateTime dtNow = DateTime.Now;

			if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
			{
				Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
				reciver.State = (bConnected == true ? 1 : 0);

                if (bRecivePoll == true)
                {
                    reciver.State += 10;
                }
				reciver.UpdateTime = dtNow;
			}
		}

		public void LoadReciverList()
		{
			m_arReciverList.Clear();
			m_dicReciverList.Clear();

			WebDBManager m_dbMgr = FormMain.Instance.DBManager;
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			string strSQL = "select ID,Place,IP,MacAddr,Baudrate,Mode,FlowCtrl,Multiport,Timeout,Description,ConnectionState,ConnectionTime, ReciverType from SensorServerInfo WHERE SiteID = "+nSiteID.ToString();

			ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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

				int nState = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
				DateTime dtTime = WebDBManager.GetDateTimeField(arrResult[i + 11].ToString(), DateTime.Now);
                int nType = WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);
				if (m_dicReciverList.ContainsKey(nID))
				{
					Reciver reciver = m_dicReciverList[nID];

					DateTime dtNow = DateTime.Now;
					TimeSpan span = dtNow - dtTime;
					double nTime = span.TotalMinutes;
					if (nTime < 0.0 && nTime >= 60.0)
						reciver.State = -1;
				}
				else
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
					reciver.State = nState;
                    reciver.Type = (Reciver.ReciverType)nType;
					m_arReciverList.Add(reciver);
					m_dicReciverList.Add(nID, reciver);
				}
			}
		}
	}
}