﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;

namespace SensorServer
{
    public class ClientDataFireView : ClientData
    {
         private object m_LockObj = new object();

         static bool bClient = true;

         private static log4net.ILog logger = null;

         private NetworkClient m_mgr = null;

         private int m_nPingCount = 0;
         public int PingCount
         {
             get { return m_nPingCount; }
             set { m_nPingCount = value; }
         }

         private int m_hDevice = -1;
         private int m_nReciverNum = -1;

         private string m_szIPAddress = "";
         public string ReciverAddress
         {
             get { return m_szIPAddress; }
             set { m_szIPAddress = value; }
         }

         private string m_szLastErrorMsg = "";
         public string LastErrorMsg
         {
             get { return m_szLastErrorMsg; }
             set { m_szLastErrorMsg = value; }
         }

         private bool m_bIsConnected = false;
         public bool IsConnected
         {
             get { return m_bIsConnected; }
             set { m_bIsConnected = value; }
         }
        
         private byte[] bufPreRecive = new byte[2048];
         private byte[] bufTemp = new byte[2048];

         int m_nSiteID = 1;

        public ClientDataFireView(SimensServiceProvider provider, ConnectionState state)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_provider = provider;
            Type = ClientType.GIMENS;                
            m_bIsConnected = true;

            this.m_szIPAddress = ((System.Net.IPEndPoint)state.RemoteEndPoint).Address.ToString();

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);;

            SendConnect();
        }
       
        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (state == null || bytes == null)
                return false;

            if (nHeader == 0)
                return false;

            if (arrDatas == null || arrDatas.Count == 0)
                return false;

            AddLog(bytes, bytes.Length);

            try
            {
                SaveTagHistory(arrDatas, nHeader);
            }
            catch(Exception)
            {

            }
            

            switch(nHeader)
            {
                case 0x91: // 전체복구
                    SendReset(arrDatas);
                    break;
                case 0x92: // 화재발생
                    ProcessSensorData(bytes, arrDatas, true);
                    break;
                case 0x93: // 화재복구
                    ProcessSensorData(bytes, arrDatas, false);
                    break;
                case 0x94: // 장애발생
                    break;
                case 0x95: // 장애복구
                    break;
                case 0x96: // 감시발생
                    break;
                case 0x97: // 감시복구
                    break;
                case 0x98: // 예비경보발생
                    break;
                case 0x99: // 예비경보복구
                    break;
            }

			return true;
		}

        private void SaveTagHistory(ArrayList arDatas, int nHeader)
        {
            int nData = 0;
            int nTagType = 0;
            switch (nHeader)
            {
                case 0x91: // 전체복구
                    nData = 'R';
                    nTagType = 0;
                    break;
                case 0x92: // 신호발생
                    nData = 'N';
                    nTagType = 1;
                    break;
                case 0x93: // 신호복구
                    nData = 'F';
                    nTagType = 1;
                    break;
                case 0x94: // 장애발생
                    nData = 'E';
                    nTagType = 2;
                    break;
                case 0x95: // 장애복구
                    nData = 'C';
                    nTagType = 2;
                    break;
                case 0x96: // 감시발생
                    nData = 'N';
                    nTagType = 3;
                    break;
                case 0x97: // 감시복구
                    nData = 'F';
                    nTagType = 3;
                    break;
                case 0x98: // 예비경보발생
                    break;
                case 0x99: // 예비경보복구
                    break;
            }

            int nStx = (short)arDatas[0];
            int nDataLength = (int)arDatas[1];

            short tx = (short)arDatas[2];
            short ty = (short)arDatas[3];

            short nOpCode = (short)arDatas[4];
            short nSeq = (short)arDatas[5];

            string szDate = (string)arDatas[6];
            string szAddress = (string)arDatas[7];

            string szAreaName = (string)arDatas[8];
            string szTargetName = (string)arDatas[9];

            string szMessage = "";
            if (arDatas.Count == 11)
            {
                szMessage = (string)arDatas[10];
            }

            string szTestLine = nOpCode.ToString() + " " + szDate + " " + szAddress + " " + szAreaName + " " + szTargetName;
            //System.Windows.Forms.MessageBox.Show(szTestLine);

            string[] szList = szAddress.Split(new char[] { '-' });

            string szReciver = szList[0];
            string szUnit = szList[1];
            string szLine = szList[2];
            string szLine2 = szList[3];
            string szCircuitNum = "1" + szUnit + szLine + szLine2;

            int nReciver = -1;
            int.TryParse(szReciver, out nReciver);

            int nCircuit = -1;
            int.TryParse(szCircuitNum, out nCircuit);

            if (m_nSiteID == 1)
            {
                int.TryParse(szLine2, out nCircuit);
            }


            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                return;
            }

            int nType = (nHeader < 90 ? 11 : 0);

            // get max id
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            string szSQL1 = "SELECT max(ID) FROM SensorTagHistory";
            ArrayList arResult = dbMgr.GetResultData(szSQL1, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nMaxID = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), 0);
                int nID = nMaxID + 1;
                int nSensorTagInfoID = GetSensorTagInfoID(dbMgr, nReciver, nCircuit);

                if (nSensorTagInfoID >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                               " ( " + nID + "," + nSensorTagInfoID + "," + nTagType + "," + szDate + "," + nData + "," + nType + "," + m_nSiteID + ")";
                    //string szSQL = "INSERT INTO SensorTagHistory (ID, SensorServerID, TagNo, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                    //           " ( " + nID + "," + nReciver + "," + nCircuit + "," + nTagType + "," + szDate + "," + nData + "," + nType + "," + m_nSiteID + ")";
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    dbMgr.GetResultData(strSQL, 0);
                }
            }
        }

        private int GetSensorTagInfoID(DBUtility.WebDBManager dbMgr, int nSensorServerID, int nTagNo)
        {
            string strSQL = string.Format("Select ID from SensorTagInfo where SensorServerID = {0} and TagNo = {1}", nSensorServerID, nTagNo);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return -1;

            return id.Data;
        }

        public void ExitClose()
        {
        }

        public override void Close()
        {
            SendDisconnect();
            try
            {
                ExitClose();
            }
            catch(Exception)
            {
            }            
            m_bIsConnected = false;           

        }

        private void AddLog(Byte[] bufRecive, int ret)
        {
            string tmp = "";
            for (int j = 0; j < ret; j++)
            {
                byte b = bufRecive[j];
                if (tmp.Length == 0)
                    tmp = string.Format("{0:X2}", (int)b);
                else
                    tmp += string.Format(" {0:X2}", (int)b);
            }
            string tmp2 = System.Text.Encoding.ASCII.GetString(bufRecive);

            logger.Debug("[" + m_szIPAddress + "][RECIVED TXT] : " + tmp2);
            logger.Debug("[" + m_szIPAddress + "][RECIVED BIN] : " + tmp);            
        }

        private void ProcessSensorData(byte[] bytes, ArrayList arDatas, bool bActivate)
        {

            int nStx = (short)arDatas[0];
            ConnectionLogClient.Instance.WriteLine("stx : " + nStx);
            int nDataLength = (int)arDatas[1];
            ConnectionLogClient.Instance.WriteLine("Length : " + nDataLength);

            short tx = (short)arDatas[2];
            ConnectionLogClient.Instance.WriteLine("tx : " + tx);
            short ty = (short)arDatas[3];
            ConnectionLogClient.Instance.WriteLine("ty : " + ty);
            short nOpCode = (short)arDatas[4];
            ConnectionLogClient.Instance.WriteLine("Opcode : " + nOpCode);
            short nSeq = (short)arDatas[5];
            ConnectionLogClient.Instance.WriteLine("Seq : " + nSeq);

            string szDate = (string)arDatas[6];
            ConnectionLogClient.Instance.WriteLine("Date : " + szDate);
            string szAddress = (string)arDatas[7];
            ConnectionLogClient.Instance.WriteLine("Address : " + szAddress);

            string szAreaName = (string)arDatas[8];
            ConnectionLogClient.Instance.WriteLine("Area : " + szAreaName);
            string szTargetName = (string)arDatas[9];
            ConnectionLogClient.Instance.WriteLine("Name : " + szTargetName);
            string szMessage = "";
            if (arDatas.Count == 11)
            {
                szMessage = (string)arDatas[10];
            }
            
            string szTestLine = nOpCode.ToString() + " " + szDate + " " + szAddress + " " + szAreaName + " " + szTargetName;
            //System.Windows.Forms.MessageBox.Show(szTestLine);

            string[] szList = szAddress.Split(new char[] { '-' });

            string szReciver = szList[0];
            string szUnit = szList[1];
            string szLine = szList[2];
            string szLine2 = szList[3];
            string szCircuitNum = "1" + szUnit + szLine + szLine2;

            int nReciver = -1;
            int.TryParse(szReciver, out nReciver);



            int nCircuit = -1;
            int.TryParse(szCircuitNum, out nCircuit);

            if( m_nSiteID == 1)
            {
                int.TryParse(szLine2, out nCircuit);
            }


            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                return;
            }


            Reciver reciver = NetworkServer.Instance.IOManager.FindReciver(nReciver);
            //Reciver reciver = NetworkServer.Instance.IOManager.FindReciver(szReciver, m_szIPAddress, szAreaName);
            if (reciver != null)
            {
                Circuit curcuit = null;
                if (reciver.Curcuits.ContainsKey(nCircuit))
                {
                    curcuit = reciver.Curcuits[nCircuit];
                }

                if (curcuit != null)
                {
                    byte nData = 0;
                    if (bActivate == true)
                        nData = 1;
                    else
                        nData = 0;
                    SendSensorData(curcuit, nData);
                }
            }    						
        }

        private void SendSensorData(Circuit curcuit, int nData)
        {
            int nCurcuit = curcuit.TagNum;

            logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");
            
			 //int nEquipzoneID = curcuit.TargetZoneID;
            int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;
            
            int nTagNum = curcuit.TagNum;
            int nSensorType = curcuit.SensorType;
            //if(nSensorType == 6 && nSensorType == 9)
            {
                m_mgr = NetworkClient.Instance;
                m_mgr.SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString());
            }
            logger.Debug("[SensorType]" + nSensorType);
            

        }

        private void SendReset(ArrayList arDatas)
        {
            if (arDatas.Count < 8)
                return;

            string szAddress = (string)arDatas[7];
            ConnectionLogClient.Instance.WriteLine("Address : " + szAddress);
            
            string[] szList = szAddress.Split(new char[] { '-' });

            string szReciver = szList[0];
            string szUnit = szList[1];
            string szLine = szList[2];
            string szLine2 = szList[3];
            string szCircuitNum = "1" + szUnit + szLine + szLine;

            int nReciver = -1;
            int.TryParse(szReciver, out nReciver);

            int nCircuit = -1;
            int.TryParse(szCircuitNum, out nCircuit);


            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                //return;
            }

            string szSensorName = "";
            if (arDatas.Count >= 9)
            {
                szSensorName = (string)arDatas[8];
            }
            logger.Debug("[SOP서버 수신반 " + szReciver + " 에 대해 복구 값 전송]");
           
            Reciver reciver = NetworkServer.Instance.IOManager.FindReciver(nReciver);
            //Reciver reciver = NetworkServer.Instance.IOManager.FindReciver(szReciver, m_szIPAddress);
            //Reciver reciver = NetworkServer.Instance.IOManager.FindReciver(szReciver, m_szIPAddress, szSensorName);
            int nData = 0;
            if (reciver != null)
            {
                foreach (KeyValuePair<int, Circuit> pair in reciver.Curcuits)
                {
                    Circuit curcuit = pair.Value;
                    SendSensorData(curcuit, nData);
                    Thread.Sleep(50);
                }
            }
            
        }

        private void SendDisconnect()
        {
            if (NetworkServer.Instance.IOManager.IsValidReciver(m_szIPAddress))
            {
                logger.Debug("[SOP서버 수신반 " + m_szIPAddress + " 접속끊어짐 전달]");
                m_mgr = NetworkClient.Instance;

                ArrayList arResult = NetworkServer.Instance.IOManager.FindRecivers(m_szIPAddress);
                foreach (Reciver reciver in arResult)
                {
                    reciver.IsConnected = false;
                    logger.Debug("[SOP서버 수신반 " + m_szIPAddress + " 접속끊어짐 전달]");
                    m_mgr.SendReciverState(reciver.ID, false);
                }
           }
        }

        private void SendConnect()
        {
            if( NetworkServer.Instance.IOManager.IsValidReciver(m_szIPAddress))
            {

               // m_szIPAddress = "192.168.10.50";
                m_mgr = NetworkClient.Instance;
                ArrayList arResult = NetworkServer.Instance.IOManager.FindRecivers(m_szIPAddress);
                foreach (Reciver reciver in arResult)
                {
                    logger.Debug("[SOP서버 수신반 " + m_szIPAddress + " 접속됨 전달]");
                    reciver.IsConnected = true;
                    m_mgr.SendReciverState(reciver.ID, true);
                }

                //m_szIPAddress = "192.168.10.51";
                //m_mgr = NetworkClient.Instance;
                //arResult = NetworkServer.Instance.IOManager.FindRecivers(m_szIPAddress);
                //foreach (Reciver reciver in arResult)
                //{
                //    logger.Debug("[SOP서버 수신반 " + m_szIPAddress + " 접속됨 전달]");
                //    m_mgr.SendReciverState(reciver.ID, true);
                //}
            }
           
        }        
	}
}

