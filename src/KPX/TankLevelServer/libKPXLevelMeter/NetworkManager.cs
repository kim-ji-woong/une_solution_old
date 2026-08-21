using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;


namespace GasDetector
{    
    internal class NetworkManager
    {
        private static log4net.ILog logger = null;

        ClientProvider m_Client = new ClientProvider();

        private int m_nSwitchNum = 1;
        public int SwitchNum
        {
            get { return m_nSwitchNum; }
            set { m_nSwitchNum = value; }
        }

        private bool bSendForamtAscii = true;
        public bool SendForamt
        {
            get { return bSendForamtAscii; }
            set { bSendForamtAscii = value; }
        }

        private byte[] mCmdBuff = new byte[512];
        private byte[] mDataBuff = new byte[2048];

        private byte[] mReciveBuffer = null;
        public byte[] ReciveBuffer
        {
            get { return mReciveBuffer; }
        }

        private Thread pollThread = null;
        private bool bExitThread = true;
        private bool bRealData = false;
        private bool m_bData = false;

        public NetworkManager(LevelMeterManager dm, ConfigFile file)
        {
            m_Client.NetworkManager = this;
            m_Client.LengthAdd = false;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static Dictionary<string, NetworkManager> mManagerList = new Dictionary<string, NetworkManager>();
        public static NetworkManager CreateNetworkManager(LevelMeterManager dm, ConfigFile file, string szIP)
        {
            if(mManagerList.ContainsKey(szIP))
            {
                return mManagerList[szIP];
            }
            
            NetworkManager manager = new NetworkManager(dm, file);
            mManagerList.Add(szIP, manager);

            return manager;
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    //return true;

                    if (m_Client.IsClientDisposed == true)
                        return false;
                    if (m_Client.IsConnected == true)
                        return true;
                }
                catch(Exception)
                {

                }
              
                return false;
            }
        }



        public bool BeginServer()
        {
            if (bExitThread == false)
                return false;

            bExitThread = false;

            bool bResult = Connect();

            pollThread = new Thread(ConnectThread);
            pollThread.Name = "NetworkPollThread_" + m_szIP;
            pollThread.Start();

            return bResult;
        }

        public void StopServer()
        {
            bExitThread = true;
            pollThread = null;

            Disconnect();
        }

        private string m_szIP = "";

        public string IPAddress
        {
            get { return m_szIP; }
            set { m_szIP = value; }
        }

        private int m_nPort = 1742;
        public int PortNumber
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        private bool Connect()
        {
            try
            { 
                if (IsConnected == true)
                    return true;

                if (!IsConnected == false)
                {
                    if (m_Client.IsClientDisposed == true)
                    {
                        m_Client = new ClientProvider();
                        m_Client.NetworkManager = this;
                        m_Client.LengthAdd = false;                        
                    }
                }

                if (IsConnected == false)
                {
                    try
                    { 
                        bool bResult = m_Client.Connect(m_szIP, m_nPort);
                        logger.Debug("Connect [ " + m_szIP + " ] : " + bResult);
                        System.Diagnostics.Trace.WriteLine("Connect [ " + m_szIP + " ] : " + bResult);
                        return bResult;
                    }
                    catch (System.Exception e)
                    {

                    }
                }
            }
            catch(Exception e)
            {
                logger.Debug("Connect Error [ " + m_szIP + " ] : " + e);
                System.Diagnostics.Trace.WriteLine("Connect Error [ " + m_szIP + " ] : " + e);
            }
            
            return false;
        }


        private void ConnectThread()
        {
            while(!bExitThread)
            {

                bool bConnect = false;
                try
                {
                    bConnect = Connect();
                    
                }
                catch(Exception ex)
                {
                    logger.Debug("ConThread Error[ " + m_szIP + "] : " + ex);
                    System.Diagnostics.Trace.WriteLine("ConThread Error[ " + m_szIP + "] : " + ex);
                }

                int nTime = 10;
                if (bConnect == false)
                    nTime = 3;
                for (int i = 0; i < nTime; i++)
                {
                    if(bExitThread == true)
                    {
                        break;
                    }
                    Thread.Sleep(300);
                }                
            }
        }


        private int m_nConnectionCheckCount = 0;
        

        public void ConnectionCheckCount()
        {
            m_nConnectionCheckCount++;
            if( m_nConnectionCheckCount == 20)
            {
                m_nConnectionCheckCount = 0;
                Disconnect();
            }
        }
        private void Disconnect()
        {
            if (IsConnected == true)
            {
                try
                {
                    m_Client.Close();
                }
                catch (System.Exception)
                {
                }
            }            
        }

        public void SendBytes(byte[] CmdBuff)
        {
            SendLog(CmdBuff, CmdBuff.Length);
            try
            {
                if (m_Client.IsClientDisposed != true && m_Client.IsConnected == true)
                {
                    m_bData = true;
                    Array.Copy(CmdBuff, mDataBuff, CmdBuff.Length);

                    m_Client.LengthAdd = false;
                    m_Client.Send(mDataBuff, 0, CmdBuff.Length);
                  
                    Thread.Sleep(50);
                    // SendLog(CmdBuff, CmdBuff.Length);
                }
            }
            catch(Exception)
            {
            }            
        }

        private bool m_bReciverData = false;

        public bool ReciverData
        {
            get { return m_bReciverData; }
            set { m_bReciverData = value; }
        }

        private byte[] tempBuffer = null;
        internal void serialPort1_DataReceived(byte [] data)
        {
            if (data == null)
                return;

            int nRead = data.Length;
            if (nRead >= 0)
            {
                // 얼마냐 ?
                if (tempBuffer == null && IsCompleteData(data))
                {
                    if (tempBuffer == null)
                    {
                        mReciveBuffer = data;
                    }
                    else
                    {
                        mReciveBuffer = new byte[tempBuffer.Length + data.Length];
                        Array.Copy(tempBuffer, 0, mReciveBuffer, 0, tempBuffer.Length);
                        Array.Copy(data, 0, mReciveBuffer, tempBuffer.Length, data.Length);

                    }

                    AddLog(mReciveBuffer, mReciveBuffer.Length);
                    tempBuffer = null;

                    m_bReciverData = true;
                }
                else
                {
                    if (tempBuffer == null)
                    {
                        tempBuffer = data;
                    }
                    else
                    {
                        int nLength = tempBuffer.Length;
                        byte[] temp = tempBuffer;
                        tempBuffer = new byte[nLength + data.Length];
                        Array.Copy(temp, tempBuffer, nLength);
                        Array.Copy(data, 0, tempBuffer, nLength, data.Length);

                        if (IsCompleteData(tempBuffer))
                        {
                            mReciveBuffer = tempBuffer;
                            AddLog(mReciveBuffer, mReciveBuffer.Length);
                            tempBuffer = null;
                        }
                    }
                }
                //sPort.DiscardInBuffer();
            }
            else
            {
                //m_bReciverData = false;
            }
           
        }

        public void ClearBuffer()
        {
            mReciveBuffer = null;
            tempBuffer = null;
        }

        private bool IsCompleteData(byte[] data)
        {
            if (data == null)
                return false;

            if (data.Length < 3)
                return false;

            int nLength = data[2] + 5;

            if (data.Length < nLength)
                return false;

            return true;
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
        public void testLog(string msg)
        {
            logger.Debug("[sssss] : " + msg);
        }
    }    


    internal class ClientProvider: TcpLib2.ClientServiceProvider
    {

        private NetworkManager m_Manager = null;
        internal NetworkManager NetworkManager
        {
            get { return m_Manager; }
            set { m_Manager = value; }
        }

        public override void OnReceiveData()
        {
            if(m_Manager != null)
            {
                m_Manager.serialPort1_DataReceived(this.ReceivedData);
            }
            //Form1.Instance.OnReceive();
        } 

        public override void OnDropConnection()
        {
            //Form1.Instance.OnDropConnection();
        }
    }
}
