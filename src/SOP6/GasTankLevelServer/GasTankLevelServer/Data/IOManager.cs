using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility2;

namespace GasLevelServer
{
    public class IOManager
    {     
        private int m_nSiteID = 1;

        public IOManager(int nSiteID)
        {
            m_nSiteID = nSiteID;

			LoadReciverList();
        }        

		private ArrayList m_arReciverList = new ArrayList();
		public ArrayList GetReciverList()
		{
			return m_arReciverList;
		}

        public Reciver FindReciver(int nReciverID)
        {
            foreach (Reciver reciver in m_arReciverList)
            {

               
                if (reciver.ID == nReciverID)
                {
                    return reciver;
                }
                
            }
            return null;
        }

        public Reciver FindReciver(int nReciverID, int nReciverType)
        {
            foreach (Reciver reciver in m_arReciverList)
            {

                if (reciver.ReciverType == nReciverType)
                {
                    if (reciver.ID == nReciverID)
                    {
                        return reciver;
                    }
                }
            }
            return null;
        }

        // 485 Unit ID로 리시버 검색
        public Reciver FindReciverForUnitID(int nUnitID, int nReciverType)
        {
            foreach (Reciver reciver in m_arReciverList)
            {
                if(reciver.ReciverType == nReciverType)
                {
                    if (reciver.ReciverID == nUnitID)
                    {
                        return reciver;
                    }
                }                
            }
            return null;
        }       
    
		public void LoadReciverList()
		{
            WebDBManager m_dbMgr = LevelMeterNetworkServer.Instance.DBManager;
            string strSQL = "select ID,Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, Description, ReciverID, SlaveID from LevelMeterServerInfo where SiteID =" + m_nSiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
				string strDesc = WebDBManager.GetStringField(arrResult[i +9], "");
                int nReciverID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
				int nSlaveID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);
                
                Reciver reciver = new Reciver();
				reciver.ID = nID;
				reciver.Place = strPlace;
				reciver.Address = strIP;
                //reciver.ReciverType = nSlaveID;
                reciver.ReciverID = nReciverID;
                reciver.SlaveID = nSlaveID;
                //reciver.MacAddress = strMac;
                //reciver.Port = nPort;
                //reciver.Mode = nMode;
                //reciver.FlowCtrl = nFlow;
				reciver.Timeout = nTimeout;
				//reciver.BuadRate = nBuadrate;

				LoadCurcuit(reciver);

				m_arReciverList.Add(reciver);
			}
		}

		private bool LoadCurcuit(Reciver reciver)
		{
            WebDBManager m_dbMgr = LevelMeterNetworkServer.Instance.DBManager;
            string strSQL = string.Format("SELECT ID, SlaveID, TagNo, LevelMeterName, LinkTankCount FROM LevelMeterTagInfo where LevelMeterServerID = {0}", reciver.ID);
			
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return false;
            
            int nResultCount = arrResult.Count;
			for (int i = 0; i < nResultCount - 4; i += 5)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nSlaveID = WebDBManager.GetIntField(arrResult[i +1 ].ToString(), -1);
				int nTagNo = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

				string strName = WebDBManager.GetStringField(arrResult[i + 3], "");
				int nTankCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
				
				string strDesc = WebDBManager.GetStringField(arrResult[i + 5], "");

				Circuit curcuit = new Circuit();

				curcuit.ID = nID;
                curcuit.SlaveID = nSlaveID;
				curcuit.TagNum = nTagNo;
				curcuit.ReciverID = reciver.ID;
                curcuit.TankCount = nTankCount;         
				curcuit.Name = strName;

				if (!reciver.Curcuits.ContainsKey(nID))
				{
                    reciver.Curcuits.Add(nID, curcuit);
				}				
			}
			return true;
		}    
    }
}
