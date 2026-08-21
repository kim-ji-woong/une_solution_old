using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireSensorServer.Network
{
    /// <summary>
    /// NMux modbus 서버랑 통신하는 Manager
    /// </summary>
    public class NMuxNetworkManager
    {
        private SOPWebClient.Logger logger = null;
        ClientProvider m_Client = new ClientProvider();

        private bool bExitThread = true;
        private Thread pollThread = null;

        // 중계반 번호
        private int m_nReceiverID = 1;
        public int ReceiverID
        {
            get { return m_nReceiverID; }
            set { m_nReceiverID = value; }
        }


        private byte[] m_ReciveBuffer = null;
        public byte[] ReciveBuffer
        {
            get { return m_ReciveBuffer; }
        }

        private bool m_bReciverData = false;
        public bool ReciverData
        {
            get { return m_bReciverData; }
            set { m_bReciverData = value; }
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (m_Client.IsClientDisposed == true)
                        return false;
                    if (m_Client.IsConnected == true)
                        return true;
                }
                catch (Exception)
                {

                }

                return false;
            }
        }

        private string m_szIP = "";
        public string IPAddress
        {
            get { return m_szIP; }
            set { m_szIP = value; }
        }

        private int m_nPort = 502;
        public int PortNumber
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public NMuxNetworkManager()
        {
            m_Client.NMuxNetworkManager = this;
            m_Client.LengthAdd = false;

            string strIP = System.Configuration.ConfigurationManager.AppSettings["ip"].ToString();
            string strPort = System.Configuration.ConfigurationManager.AppSettings["Port"].ToString();
            string strLogFile = System.Configuration.ConfigurationManager.AppSettings.Get("logFile");
            string strLogFolder = System.Configuration.ConfigurationManager.AppSettings.Get("logFolder");

            m_szIP = strIP;
            m_nPort = int.Parse(strPort);

            if (logger == null)
                logger = new SOPWebClient.Logger(strLogFolder, strLogFile, 30);
        }

        private static List<NMuxNetworkManager> mManagerList = new List<NMuxNetworkManager>();
        public static NMuxNetworkManager CreateNetworkManager()
        {
            NMuxNetworkManager manager = new NMuxNetworkManager();
            mManagerList.Add(manager);

            return manager;
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
                        m_Client.NMuxNetworkManager = this;
                        m_Client.LengthAdd = false;
                    }
                }

                if (IsConnected == false)
                {
                    try
                    {
                        bool bResult = m_Client.Connect(m_szIP, m_nPort);
                        WriteLog("Connect [ " + m_szIP + " ] : " + bResult);
                        System.Diagnostics.Trace.WriteLine("Connect [ " + m_szIP + " ] : " + bResult);
                        return bResult;
                    }
                    catch (System.Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                WriteLog("Connect Error [ " + m_szIP + " ] : " + e);
                System.Diagnostics.Trace.WriteLine("Connect Error [ " + m_szIP + " ] : " + e);
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
                    bConnect = Connect();

                }
                catch (Exception ex)
                {
                    WriteLog("ConThread Error[ " + m_szIP + "] : " + ex);
                    System.Diagnostics.Trace.WriteLine("ConThread Error[ " + m_szIP + "] : " + ex);
                }

                int nTime = 10;
                if (bConnect == false)
                    nTime = 3;
                for (int i = 0; i < nTime; i++)
                {
                    if (bExitThread == true)
                        break;
                    Thread.Sleep(300);
                }
            }
        }

        public void StopServer()
        {
            bExitThread = true;
            pollThread = null;

            Disconnect();
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

        private byte[] tempBuffer = null;
        internal void DataReceived(byte[] data)
        {
            if (data == null)
                return;

            int nRead = data.Length;
            if (nRead >= 0)
            {
                // 얼마냐 ?
                if (tempBuffer == null && IsCheckData(data))
                {
                    if (tempBuffer == null)
                    {
                        m_ReciveBuffer = data;
                    }
                    else
                    {
                        m_ReciveBuffer = new byte[tempBuffer.Length + data.Length];
                        Array.Copy(tempBuffer, 0, m_ReciveBuffer, 0, tempBuffer.Length);
                        Array.Copy(data, 0, m_ReciveBuffer, tempBuffer.Length, data.Length);
                    }

                    AddLog(m_ReciveBuffer, m_ReciveBuffer.Length);
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

                        if (IsCheckData(tempBuffer))
                        {
                            m_ReciveBuffer = tempBuffer;
                            AddLog(m_ReciveBuffer, m_ReciveBuffer.Length);
                            tempBuffer = null;
                        }
                    }
                }
            }
            else
            {
                //m_bReciverData = false;
            }
        }

        public void ClearBuffer()
        {
            m_ReciveBuffer = null;
            tempBuffer = null;
        }

        private bool IsCheckData(byte[] data)
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

        private byte[] m_DataBuff = new byte[2048];
        public void SendBytes(byte[] CmdBuff)
        {
            SendLog(CmdBuff, CmdBuff.Length);
            try
            {
                if (m_Client.IsClientDisposed != true && m_Client.IsConnected == true)
                {
                    Array.Copy(CmdBuff, m_DataBuff, CmdBuff.Length);

                    m_Client.LengthAdd = false;
                    m_Client.Send(m_DataBuff, 0, CmdBuff.Length);

                    Thread.Sleep(50);
                }
            }
            catch (Exception)
            {
            }
        }

        private void AddLog(Byte[] bufRecive, int ret)
        {
            string szName = "[" + this.ReceiverID + "]";
            string tmp = "";
            for (int j = 0; j < ret; j++)
            {
                byte b = bufRecive[j];
                if (tmp.Length == 0)
                    tmp = string.Format("{0:X2}", (int)b);
                else
                    tmp += string.Format(" {0:X2}", (int)b);
            }
            //System.Diagnostics.Trace.WriteLine(szName + "[RECV TXT] : " + tmp);
            WriteLog(szName + "[RECV TXT] : " + tmp);
        }

        private void SendLog(Byte[] bufRecive, int ret)
        {
            string szName = "[" + this.ReceiverID + "]";
            string tmp = "";
            for (int j = 0; j < ret; j++)
            {
                byte b = bufRecive[j];
                if (tmp.Length == 0)
                    tmp = string.Format("{0:X2}", (int)b);
                else
                    tmp += string.Format(" {0:X2}", (int)b);
            }

            //System.Diagnostics.Trace.WriteLine(szName + "[SEND TXT] : " + tmp);

            WriteLog(szName + "[SEND TXT] : " + tmp);
        }

        public void WriteLog(string strLog)
        {
            logger.Write(strLog);
        }
    }

    internal class ClientProvider : TcpLib2.ClientServiceProvider
    {
        private NMuxNetworkManager m_nmuxManager = null;
        internal NMuxNetworkManager NMuxNetworkManager
        {
            get { return m_nmuxManager; }
            set { m_nmuxManager = value; }
        }

        public override void OnReceiveData()
        {
            if (m_nmuxManager != null)
            {
                m_nmuxManager.DataReceived(this.ReceivedData);
            }
            //Form1.Instance.OnReceive();
        }

        public override void OnDropConnection()
        {
            //Form1.Instance.OnDropConnection();
        }
    }
}
