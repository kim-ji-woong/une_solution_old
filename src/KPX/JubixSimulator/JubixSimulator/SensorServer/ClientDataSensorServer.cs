﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Threading;

namespace S1SensorServer
{
    public class ClientDataSensorServer : ClientData
    {
         private object m_LockObj = new object();

         static bool bClient = true;

         private static log4net.ILog logger = null;
        
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

         public ClientDataSensorServer(NetworkServiceProvider provider, ConnectionState state)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_provider = provider;
            Type = ClientType.KPXSensorServer;                
            m_bIsConnected = true;

            this.m_szIPAddress = ((System.Net.IPEndPoint)state.RemoteEndPoint).Address.ToString();

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);;

        }
       
        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, JubixNetwork.JubixMessage msg)
        {
            if (state == null || bytes == null)
                return false;

            if (nHeader == 0)
                return false;

            if (msg == null)
                return false;

            AddLog(bytes, bytes.Length);

            // TODO : Process Command     

            //byte[] data = msg.MakeByte(true);

            if(m_bSendSetValue == true)
            {
                byte[] data = msg.SimulationMakeByte(true, m_nCh, m_fSendValue);

                m_provider.SendData(data);
            }
            else
            {
                if(m_bSendWorkStart == true)
                {
                    byte[] data = msg.SimulationMakeByte(true, m_nCh, m_fSendValue);
                    m_provider.SendData(data);

                    if (m_nStartCount < 6)
                    {
                        m_fSendValue += 100.0f;
                        m_nStartCount++;
                    }                  

                }
                else
                {
                    byte[] data = msg.MakeByte(true);
                    m_provider.SendData(data);
                }               
            }
            return true;
		}
        
        private bool m_bSendWorkStart = false;
        private bool m_bSendSetValue = false;
        private float m_fSendValue = 0.0f;
        private int m_nCh = 1;

        private int m_nStartCount = 0;

        public void SendoWorkStart(int nCh)
        {
            m_nCh = nCh;
            m_bSendSetValue = false;
            m_bSendWorkStart = true;
            m_fSendValue = 0;
            m_nStartCount = 0;
        }

        public void SetSimValue(int nTime, float value)
        {
            m_bSendWorkStart = false;
            m_bSendSetValue = true;
            m_fSendValue = value;
            m_nCh = nTime;
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


        
    }
}

