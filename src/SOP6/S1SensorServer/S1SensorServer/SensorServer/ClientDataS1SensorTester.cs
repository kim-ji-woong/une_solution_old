﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Threading;
using DBUtility2;
using UnE.Sensor;

namespace S1SensorServer
{
    using Data;

    public class ClientDataS1SensorTester : ClientData
    {
         private object m_LockObj = new object();

         static bool bClient = true;

         private static log4net.ILog logger = null;

         private NetworkWebClient m_mgr = null;

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

         public ClientDataS1SensorTester(S1NetworkServiceProvider provider, ConnectionState state)
        {
            m_nSiteID = S1NetworkServer.Instance.SiteID;

            m_provider = provider;
            Type = ClientType.PSMTester;                
            m_bIsConnected = true;

            this.m_szIPAddress = ((System.Net.IPEndPoint)state.RemoteEndPoint).Address.ToString();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

            // 영흥은 라인체계가 다름, 센서주소는 센서서버-센서유닛-채널-라인 으로 구성되어 있음            
            string szAddress = (string)arDatas[7];
        
            string szAreaName = (string)arDatas[8];
            string szTargetName = (string)arDatas[9];

            string szMessage = "";
            if (arDatas.Count == 11)
            {
                szMessage = (string)arDatas[10];
            }
                        
            string[] szList = szAddress.Split(new char[] { '-' });

            string szReciver = szList[0];
            string szUnit = szList[1];
            string szChn = szList[2];
            string szLine = szList[3];

            string szCircuitNum = szUnit + szChn + szLine;

            int nReciver = -1;
            int.TryParse(szReciver, out nReciver);

            int nCircuit = -1;
            int.TryParse(szCircuitNum, out nCircuit);

            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                return;
            }            
            // 0x90보다 큰신호는 PSM일수 있다?
            int nType = (nHeader < 0x90 ? 11 : 0);
            
            // get max id
            DirectDBManagerEx dbMgr = S1NetworkServer.Instance.DBManager;
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
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    dbMgr.GetResultData(strSQL);
                }
            }
        }

        private int GetSensorTagInfoID(DirectDBManagerEx dbMgr, int nSensorServerID, int nTagNo, ref int nSensorType)
        {
            string strSQL = string.Format("Select ID, SensorType from SensorTagInfo where SensorServerID = {0} and TagNo = {1}", nSensorServerID, nTagNo);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return -1;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());

            nSensorType = sensorType.Data;

            if (id == null)
                return -1;

            if (sensorType != null)
            {
                if (sensorType.Data == (int)IFacility.FacilityType.PSM_SENSOR)
                    nSensorType = sensorType.Data;
                else if (sensorType.Data == (int)IFacility.FacilityType.FIRE_SENSOR ||
                    (sensorType.Data >= (int)IFacility.FacilityType.FireSensor_TypeA && sensorType.Data <= (int)IFacility.FacilityType.FireSensor_MonitoringType))
                    nSensorType = (int)IFacility.FacilityType.FIRE_SENSOR;
                 else if (sensorType.Data == (int)IFacility.FacilityType.Fire_S1 ||sensorType.Data == (int)IFacility.FacilityType.FireF1_S1)
                    nSensorType = (int)IFacility.FacilityType.FIRE_SENSOR;
            }

            return id.Data;
        }

        public void ExitClose()
        {
        }

        public override void Close()
        {            
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
            
            string[] szList = szAddress.Split(new char[] { '-' });

            string szReciver = szList[0];
            string szUnit = szList[1];
            string szCh = szList[2];
            string szLine = szList[3];

            string szCircuitNum = szUnit + szCh + szLine;
            if( m_nSiteID == 2)
            {
                szCircuitNum = "1" + szUnit + szCh + szLine;
            }


            int nReciver = -1;
            int.TryParse(szReciver, out nReciver);

            int nCircuit = -1;
            int.TryParse(szCircuitNum, out nCircuit);
                         
            // 회로번호가 없는 경우
            if (nCircuit < 0)
            {
                logger.Debug("없는 회로 번호 : " + nCircuit);
                return;
            }
            
            int nType = (bPSM == true ? 2 : 1);

            Reciver reciver = S1NetworkServer.Instance.IOManager.FindReciver(nReciver);
            if (reciver != null)
            {
                Circuit2 curcuit = null;
                if (reciver.Circuits.ContainsKey(nCircuit))
                {
                    curcuit = (Circuit2)reciver.Circuits[nCircuit];
                }

                if (curcuit != null)
                {
                    byte nData = 0;
                    if (bActivate == 0)
                        nData = 0;
                    else
                        nData = (byte)bActivate;

                    if (bPSM == true)
                        nData += 20;
                    SendSensorData(curcuit, nData, bPSM);
                }
            }
        }

        private void SendSensorData(Circuit2 curcuit, int nData, bool bPSM = false)
        {
            int nCurcuit = curcuit.TagNum;

            logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");
            
			 //int nEquipzoneID = curcuit.TargetZoneID;
            int nSensorZoneID = curcuit.SensorZone == null ? -1 : curcuit.SensorZone.ID;
            
            int nTagNum = curcuit.TagNum;
            int nSensorType = (int)curcuit.SensorType;
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
         
            int nReciver = -1;
            int.TryParse(szReciver, out nReciver);

            string szSensorName = "";
            if (arDatas.Count >= 9)
            {
                szSensorName = (string)arDatas[8];
            }
            logger.Debug("[SOP서버 수신반 " + szReciver + " 에 대해 복구 값 전송]");

            Reciver reciver = S1NetworkServer.Instance.IOManager.FindReciver(nReciver);      
            int nData = 0;
            if (reciver != null)
            {
                foreach (KeyValuePair<int, Circuit> pair in reciver.Circuits)
                {
                    Circuit2 curcuit = (Circuit2)pair.Value;
                    SendSensorData(curcuit, nData);
                    Thread.Sleep(50);
                }
            }            
        }      
	}
}

