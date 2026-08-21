using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using SDMS;


namespace SensorMonitor
{
    internal class FireReciverProvider : ClientServiceProvider
    {
        private static log4net.ILog logger = null;

        private int m_nPingCount = 0;
        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        private string m_szIP = "";
        public string IPAddress
        {
            get { return m_szIP; }
            set { m_szIP = value; }
        }

        private int m_nPort = 4002;
        public int PortNumber
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        private Thread pollThread = null;

        private bool bExitThread = false;


        private Reciver m_Reciver = null;

        private NetworkManager m_mgr = null;        
 
        private bool m_bReciverData = false;
        public bool ReciverData
        {
            get { return m_bReciverData; }
            set { m_bReciverData = value; }
        }

        private int m_nReciverNum = -1;

        private Dictionary<int, Curcuit> m_nCircuits = null;
                
        // 이전에 처리되고 남은 수신데이터
        // 다음번 처리시 앞에 붙는다.
        private byte[] m_extraBuffer = null;

        public FireReciverProvider(NetworkManager mgr, Reciver reciver)
        {

            LengthAdd = false;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_Reciver = reciver;
            m_mgr = mgr;

            m_nPort = reciver.Port;
            m_szIP = reciver.Address;
            m_nCircuits = reciver.Curcuits;
            m_nReciverNum = reciver.ID;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private FireReciverProvider()
        {
            LengthAdd = false;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            m_nPort = 4002;
        }

        public override void OnDropConnection()
        {
            m_Reciver.IsConnected = false;
        }

        public new bool IsConnected
        {
            get
            {
                try
                {
                    if (IsClientDisposed == true)
                        return false;
                    if (base.IsConnected == true)
                        return true;
                }
                catch (Exception)
                {
                }
                return false;
            }
        }

        public bool BeginServer()
        {
            bExitThread = false;

            //bool bResult = ConnectServer();

            pollThread = new Thread(ConnectThread);
            pollThread.Start();

            return true;
        }

        public void StopServer()
        {
            bExitThread = true;
            if (pollThread != null)
            {
                try
                {
                    pollThread.Join(3000);
                }
                catch (Exception)
                {

                }
                pollThread = null;
            }
            

            Disconnect();
        }        

        private bool ConnectServer()
        {
            try
            {
                if (IsConnected == true)
                    return true;

                if (IsConnected == false)
                {
                    if (IsClientDisposed == true)
                    {
                      
                        LengthAdd = false;
                        
                    }                    
                }
                

                if (IsConnected == false)
                {
                    try
                    {
                        bool bResult = Connect(m_szIP, m_nPort);
                        logger.Debug("Connnect [ " + m_szIP + " ] : " + bResult);
                        System.Diagnostics.Trace.WriteLine("Connnect [ " + m_szIP + " ] : " + bResult);

                        if (bResult== true)
                        {
                            m_nPingCount = 0;
                            //SendNACK();
                        }

                        return bResult;
                    }
                    catch (System.Exception ex)
                    {
                        logger.Debug("Connnect Error [ " + m_szIP + " ] : " + ex);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Connnect Error [ " + m_szIP + " ] : " + e);
                System.Diagnostics.Trace.WriteLine("Connnect Error [ " + m_szIP + " ] : " + e);
            }
            return false;
        }
        
        private void ConnectThread()
        {
            while (!bExitThread)
            {

                bool bConnect = false;
                try
                {
                    bConnect = ConnectServer();
                    if (!bConnect)
                    {
                        if (IsClientDisposed == true)
                        {
                            LengthAdd = false;
                        }
                    }
                    else
                    {
                        m_Reciver.IsConnected = true;
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug("ConThread Error[ " + m_szIP + "] : " + ex);
                    System.Diagnostics.Trace.WriteLine("ConThread Error[ " + m_szIP + "] : " + ex);
                    m_Reciver.IsConnected = false;
                }


                m_nPingCount++;
                if (m_nPingCount == 20)
                {
                    Disconnect();
                    m_Reciver.IsConnected = false;
                    m_nPingCount = 0;
                }
                        
                int nTime = 3;
                if (bConnect == false)
                    nTime = 3;
                for (int i = 0; i < nTime; i++)
                {
                    if (bExitThread == true)
                    {
                        break;
                    }
                    Thread.Sleep(300);
                }
            }
        }

        private void Disconnect()
        {
            if (IsConnected == true)
            {
                try
                {
                    m_bReciveFirstPoll = false;

                    m_Reciver.IsConnected = false;
                    m_Reciver.RecivedPoll = false;
                    
                    ClearBuffer();

                    Close();
                }
                catch (System.Exception)
                {
                }
            }
        }
  
        private int FindETX(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x03)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindSTX(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x02)
                {
                    return i;
                }
            }
            return -1;
        }

        private bool IsZero(byte[] buffer)
        {
            if (buffer == null)
                return true;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != 0x00)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnReceiveData()
        {
            if (ReceivedData == null)
                return;

            int ret = ReceivedData.Length;
            if (ret > 0)
            {
                byte[] data = new byte[ret];
                int startIndex = 0;
                if (startIndex >= 0)
                {

                    //AddLog(ReceivedData, ret);

                    //02 30 30 2D 30 30 2D 30 30 65 34 03
                    Array.Copy(ReceivedData, startIndex, data, 0, ret);
                    ProcessRecivedData(data);
                    m_nPingCount = 0;

                    // ACK를 보낸다.
                    //SendACK();
                }
                else
                {
                    // POL인경우 ACK를 보낸다.
                    //SendACK();
                }

                m_nPingCount = 0;
            }
            else if (ret < 0)
            {
                return;
            }
            
            return;
        }

        private bool m_bReciveFirstPoll = false;

        private void ProcessRecivedData(byte[] data)
        {            
            if (m_extraBuffer != null && m_extraBuffer.Length > 0)
            {
                byte[] nTotalData = new byte[m_extraBuffer.Length + data.Length];
                Array.Copy(m_extraBuffer, 0, nTotalData, 0, m_extraBuffer.Length);
                Array.Copy(data, 0, nTotalData, m_extraBuffer.Length, data.Length);

                data = nTotalData;
            }

            ArrayList arDatas = new ArrayList();
            int nBeginIdx = -1;
            int nEndIdx = -1;
            int nLastIdx = -1;
            // 회로번호를 가져온다.
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x02)
                {
                    nBeginIdx = i;
                    nLastIdx = i;
                }
                if (data[i] == 0x03)
                {
                    nEndIdx = i;
                    nLastIdx = i;
                    if (nBeginIdx != -1)
                    {
                        int nLenght = nEndIdx - nBeginIdx + 1;
                        byte[] cmd = new byte[nLenght];
                        Array.Copy(data, nBeginIdx, cmd, 0, nLenght);
                        arDatas.Add(cmd);

                        nEndIdx = -1;
                        nBeginIdx = -1;
                    }
                    nEndIdx = -1;
                    nBeginIdx = -1;
                }
            }

            // nLastIdx가 data범위 안에 있는경우
            if (nLastIdx < data.Length && nLastIdx >= 0)
            {
                if (nLastIdx > 0)
                {
                    m_extraBuffer = new byte[data.Length - nLastIdx - 1];
                    Array.Copy(data, nLastIdx - 1, m_extraBuffer, 0, m_extraBuffer.Length);
                }
                else if (nLastIdx == 0)
                {
                    m_extraBuffer = new byte[data.Length - nLastIdx];
                    Array.Copy(data, nLastIdx, m_extraBuffer, 0, m_extraBuffer.Length);
                }
            }
            else
            {
                m_extraBuffer = null;
            }

            if (IsZero(m_extraBuffer))
                m_extraBuffer = null;

            foreach (byte[] cmd in arDatas)
            {
                if (!IsPoll(cmd))
                {
                    
                   
                    AddLog(cmd, cmd.Length);
                    int nCurcuit = GetCurcuit(cmd);
                    // 해당 데이터를 처리한다.
                    ProcessSensorData(cmd, nCurcuit);
                    SendACK();
                }
                else
                {
                    if (m_bReciveFirstPoll == false)
                    {
                        m_bReciveFirstPoll = true;
                        m_Reciver.RecivedPoll = true;

                        logger.Debug("[" + this.m_szIP+"] Recived Poll");
                        Debug.WriteLine("[" + this.m_szIP + "] Recived Poll");
                    }
                    
                    // ACK를 보낸다.
                    SendACK();

                    //SendPoll();
                }
            }
        }

        private int GetCurcuit(byte[] data)
        {
            //02 30 30 2D 30 30 2D 30 30 65 34 03
            if (data.Length < 9)
                return -1;
            char b2 = (char)data[5];
            char c1 = (char)data[7];
            char c2 = (char)data[8];

            StringBuilder sb2 = new StringBuilder();
            sb2.Append(b2);
            sb2.Append(c1);
            sb2.Append(c2);
            string szTag = sb2.ToString();
            System.Diagnostics.Trace.WriteLine("회로번호 : " + szTag);
            int nCurcuitID = -1;
            if (int.TryParse(szTag, out nCurcuitID))
            {
                return nCurcuitID;
            }
            return -1;
        }

        private bool IsPoll(byte[] data)
        {

            int nSTX = FindSTX(data);
            int nETX = FindETX(data);

            if (nSTX == -1 || nETX == -1)
                return false;


            if (data.Length <= nSTX + 3)
                return false;

            char b2 = (char)data[nSTX + 1];
            char c1 = (char)data[nSTX + 2];
            char c2 = (char)data[nSTX + 3];

            StringBuilder sb2 = new StringBuilder();
            sb2.Append(b2);
            sb2.Append(c1);
            sb2.Append(c2);
            string szTag = sb2.ToString();
            if (szTag == "POL")
            {
                Debug.WriteLine("[" + m_szIP + "][RECIVED TXT] : POL");
                return true;
            }

            return false;
        }

        private bool CheckSum(byte[] buffer)
        {
            if (buffer.Length < 11)
                return false;
            byte sum = (byte)(((buffer[0] + buffer[1] + buffer[2] + buffer[3] + buffer[4] + buffer[5] + buffer[6] + buffer[7] + buffer[8] + buffer[9] + buffer[11]) % (byte)16) + (byte)0x30);

            return (sum == buffer[10] ? true : false);
        }

        private void ProcessSensorData(byte[] bytes, int nCurcuit)
        {

            byte nData = 0;
            if (bytes[9] == 'E' || bytes[9] == 'e')
                return;

            if (bytes[9] == 'R')
            {
                SendReset();
                return;
            }

            if (nCurcuit < 0)
                return;

            Curcuit curcuit = null;
            if (m_nCircuits.ContainsKey(nCurcuit))
            {
                curcuit = m_nCircuits[nCurcuit];
            }

            // 회로번호가 없는 경우
            if (curcuit == null)
            {
                logger.Info("없는 회로 번호 : " + nCurcuit);
                return;
            }

            int isFire = bytes[4] - '0';

            if (bytes[9] == 'N' && isFire == 1)
            {
                nData = 1;
            }
            else if (bytes[9] == 'N' && isFire == 2)
            {
                if (curcuit.SensorType == 3)
                {
                    nData = 1;
                }
            }
            else if (bytes[9] == 'F' && isFire == 2)
            {
                if (curcuit.SensorType == 3)
                {
                    nData = 0;
                }
            }
            else if (bytes[9] == 'F' && isFire == 1)
            {
                nData = 0;
            }
            else if (bytes[9] == 'R')
            {
                SendReset();
                return;
            }
            else
            {
                logger.Info("처리할 수없는 데이터 유형 : " + nCurcuit);
                return;
            }

            SendSensorData(curcuit, nData);
        }

        private void SendSensorData(Curcuit curcuit, int nData)
        {
            int nCurcuit = curcuit.TagNum;

            logger.Info("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");
            Debug.WriteLine("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

            int nSensorZoneID = curcuit.TargetZoneID;

            int nTagNum = curcuit.TagNum;
            int nSensorType = curcuit.SensorType;

            m_mgr.SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString());

            //m_mgr.SendSensorData(curcuit.ReciverID, nCurcuit, nData);
        }

        public void SendReset()
        {
            int nData = 0;
            foreach (KeyValuePair<int, Curcuit> pair in m_nCircuits)
            {
                Curcuit curcuit = pair.Value;
                SendSensorData(curcuit, nData);
                Thread.Sleep(50);
            }
            logger.Info("[SOP서버로 수신반 리셋 : " + this.m_Reciver.Address + "]");

        }

        private void ClearBuffer()
        {
            m_extraBuffer = null;
        }

        /// <summary>
        /// Send ACK 
        /// </summary>
        public void SendACK()
        {
            SendData(SERIAL_ID.ACK);
        }

        public void SendPoll()
        {
            SendData(SERIAL_ID.POLL);
        }

        public void SendNACK()
        {
            SendData(SERIAL_ID.NACK);
            Debug.WriteLine("Send NACK");
        }

        // header 1 Byte로만 이루어진 데이터
        private void SendData(byte header)
        {
            byte[] send = new byte[1];
            send[0] = header;
            base.Send(send, 0, 1);
        }

        private void SendLog(Byte[] bufRecive, int ret)
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

            Debug.WriteLine("[SEND TXT] : " + tmp);

            logger.Debug("[SEND TXT] : " + tmp);
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
            Debug.WriteLine("[RECIVED TXT] : " + tmp);
            logger.Debug("[RECIVED TXT] : " + tmp);
        }
    }

}
