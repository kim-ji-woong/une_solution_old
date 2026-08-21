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

namespace LeakComTest
{
    internal class LeakSensorClientProvider : ClientServiceProvider
    {
        private static log4net.ILog logger = null;

        private int m_nPingCount = 0;
        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        private string m_szIP = "192.168.0.195";
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

        private Thread pollThread = null;

        private bool bExitThread = false;
        
        private bool m_bReciverData = false;
        public bool ReciverData
        {
            get { return m_bReciverData; }
            set { m_bReciverData = value; }
        }
                 
        // 이전에 처리되고 남은 수신데이터
        // 다음번 처리시 앞에 붙는다.
        private byte[] m_extraBuffer = null;

        public LeakSensorClientProvider()
        {
            LengthAdd = false;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            m_nPort = 502;
        }

        public override void OnDropConnection()
        {
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

                        if (bResult == true)
                        {
                            m_nPingCount = 0;
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
                }
                catch (Exception ex)
                {
                    logger.Debug("ConThread Error[ " + m_szIP + "] : " + ex);
                    System.Diagnostics.Trace.WriteLine("ConThread Error[ " + m_szIP + "] : " + ex);
                }


                m_nPingCount++;
                if (m_nPingCount == 20)
                {
                    Disconnect();
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

                    ClearBuffer();

                    Close();
                }
                catch (System.Exception)
                {
                }
            }
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

                    AddLog(ReceivedData, ret);

                    Array.Copy(ReceivedData, startIndex, data, 0, ret);
                    
                    ProcessRecivedData(data);
                    
                    m_nPingCount = 0;
                }               
                m_nPingCount = 0;
            }
            else if (ret < 0)
            {
                return;
            }
            return;
        }

        private void ProcessRecivedData(byte[] data)
        {
         
        }

     
        private void ClearBuffer()
        {
            m_extraBuffer = null;
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
