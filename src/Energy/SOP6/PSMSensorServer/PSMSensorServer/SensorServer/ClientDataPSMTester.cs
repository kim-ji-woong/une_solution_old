﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Threading;
using DBUtility2;

namespace PSMSensorServer
{
    public class ClientDataPSMTester : ClientData
    {
         private object m_LockObj = new object();

         //static bool bClient = true;

         private static log4net.ILog logger = null;

         private NetworkWebClient m_mgr = null;

         private int m_nPingCount = 0;
         public int PingCount
         {
             get { return m_nPingCount; }
             set { m_nPingCount = value; }
         }

         //private int m_hDevice = -1;
         //private int m_nReciverNum = -1;

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

         public ClientDataPSMTester(PSMServiceProvider provider, ConnectionState state)
        {
            m_nSiteID = PSMNetworkServer.Instance.SiteID;

            m_provider = provider;
            Type = ClientType.PSMTester;                
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

            SaveTagHistory(arrDatas, nHeader);

            switch(nHeader)
            {
                case 0x91: // 전체복구
                    SendReset(arrDatas);
                    break;
                case 0x92: // 신호발생
                    ProcessSensorData(bytes, arrDatas, 1);
                    break;
                case 0x93: // 신호복구
                    ProcessSensorData(bytes, arrDatas, 0);
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

                case 0x87: // PSM 1단계
                    ProcessSensorData(bytes, arrDatas, 1, true);
                    break;
                case 0x88: // PSM 2단계
                    ProcessSensorData(bytes, arrDatas, 2, true);
                    break;
                case 0x89: // PSM 3단계
                    ProcessSensorData(bytes, arrDatas, 3, true);
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
                case 0x87:
                case 0x88:
                case 0x89:
                    nData = 'N';
                    nTagType = 1;
                    break;

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

            DateTime time = Convert.ToDateTime((string)arDatas[6]);
            string szDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
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

            int nType = (nHeader < 0x90 ? 11 : 0);
            
            // get max id
            WebDBManager dbMgr = PSMNetworkServer.Instance.DBManager;
            string szSQL1 = "SELECT max(ID) FROM SensorTagHistory";
            ArrayList arResult = dbMgr.GetResultData(szSQL1);
            if (arResult != null && arResult.Count > 0)
            {
                int nMaxID = WebDBManager.GetIntField(arResult[0].ToString(), 0);
                int nID = nMaxID + 1;
                int nSensorTagInfoID = GetSensorTagInfoID(dbMgr, nReciver, nCircuit, ref nType);

                if (nSensorTagInfoID >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                                    " ( " + nID + "," + nSensorTagInfoID + "," + nTagType + ",'" + szDate + "'," + nData + "," + nType + "," + m_nSiteID + ")";
                    //string szSQL = "INSERT INTO SensorTagHistory (ID, SensorServerID, TagNo, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                    //           " ( " + nID + "," + nReciver + "," + nCircuit + "," + nTagType + ",'" + szDate + "'," + nData + "," + nType + "," + m_nSiteID + ")";
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    dbMgr.GetResultData(strSQL);
                }
            }
        }

        private int GetSensorTagInfoID(WebDBManager dbMgr, int nSensorServerID, int nTagNo, ref int nSensorType)
        {
            string strSQL = string.Format("Select ID, SensorType from SensorTagInfo where SensorServerID = {0} and TagNo = {1}", nSensorServerID, nTagNo);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return -1;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());

            if (id == null)
                return -1;

            if (sensorType != null)
            {
                if (sensorType.Data == (int)global::PSMSensorServer.Facility.FacilityType.PSM_SENSOR)
                    nSensorType = sensorType.Data;
                else if (sensorType.Data == (int)global::PSMSensorServer.Facility.FacilityType.FIRE_SENSOR ||
                    (sensorType.Data >= (int)global::PSMSensorServer.Facility.FacilityType.FireSensor_TypeA && sensorType.Data <= (int)global::PSMSensorServer.Facility.FacilityType.FireSensor_MonitoringType))
                    nSensorType = (int)global::PSMSensorServer.Facility.FacilityType.FIRE_SENSOR;
            }

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

        private void ProcessSensorData(byte[] bytes, ArrayList arDatas, int bActivate, bool bPSM = false)
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
            if (m_nSiteID == 3)
            {
                int.TryParse(szLine2, out nCircuit);
            }
             

            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                return;
            }


            int nType = (bPSM == true ? 2 : 1);

          
            
            Reciver reciver = PSMNetworkServer.Instance.IOManager.FindReciver(nReciver);
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
                    if (bActivate == 0)
                        nData = 0;
                    else
                        nData = (byte)bActivate;
                    SendSensorData(curcuit, nData, bPSM);

                    if (nData == 0 && bPSM == true)
                    {
                        // 주빅스에 복구 신호를 추가한다.
                        PSMSensorManager.Instance.Detector.SetResetJubixDB(curcuit.Name);
                    }                   
                }
            }    						
        }

        private void SendSensorData(Circuit curcuit, int nData, bool bPSM = false)
        {
            int nCurcuit = curcuit.TagNum;

            logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");
            
			 //int nEquipzoneID = curcuit.TargetZoneID;
            int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;
            
            int nTagNum = curcuit.TagNum;
            int nSensorType = curcuit.SensorType;
            //if(nSensorType == 6 && nSensorType == 9)
            {
                m_mgr = NetworkWebClient.Instance;
                m_mgr.SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString(), bPSM, true);
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
           
            Reciver reciver = PSMNetworkServer.Instance.IOManager.FindReciver(nReciver);
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
            if (PSMNetworkServer.Instance.IOManager.IsValidReciver(m_szIPAddress))
            {
                logger.Debug("[SOP서버 수신반 " + m_szIPAddress + " 접속끊어짐 전달]");
                m_mgr = NetworkWebClient.Instance;

                ArrayList arResult = PSMNetworkServer.Instance.IOManager.FindRecivers(m_szIPAddress);
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
            if( PSMNetworkServer.Instance.IOManager.IsValidReciver(m_szIPAddress))
            {

               // m_szIPAddress = "192.168.10.50";
                m_mgr = NetworkWebClient.Instance;
                ArrayList arResult = PSMNetworkServer.Instance.IOManager.FindRecivers(m_szIPAddress);
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

