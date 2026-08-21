using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace HSMSServer
{
    public class ServiceProvider : TcpServiceProvider
    {
        private ArrayList m_arrClients = new ArrayList();

		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			set { m_bIsLogOpened = value; }
		}

		private void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

		private void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        private void InitLog()
        {
			if (ConnectionLogEx.MakeInstance())
				m_bIsLogOpened = true;
			else
				m_bIsLogOpened = false;
        }

        public void RecvLog(byte[] bytes, string strReceived, ConnectionState state)
        {
			if (!IsLogOpened)
                return;

            string strClient = "HSMS";
            strClient += "(" + state.RemoteEndPoint.ToString() + ")";

            string strLog = string.Format("RecvMessage({0} {1}), from {2} : {3}",
                bytes.Length, bytes.Length > 1 ? "bytes" : "byte", strClient, strReceived);

            string strBytes = "";

            foreach (byte b in bytes)
            {
                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLineLog(strLog + strBytes);

            strLog = string.Format("RecvMessage from {0} : {1}",
                strClient, strReceived);
            FormMain.Instance.AddLog(strLog);
        }

        // arrDropList가 null이 아닐 경우, 예외가 발생하면 바로 OnDropConnection()을 호출하지 않고 해당 state를 일단 arrDropList에 담아둔다.
        // m_arrClient Loop 실행 도중 OnDropConnection() 호출로 인하여 m_arrClient가 변경되는 것을 막기 위함이다.
        private bool _Send(byte[] bytes, string strSend, ConnectionState state, ArrayList arrDropList)
        {
            try
            {
                if (state.Write(bytes, 0, bytes.Length))
                {
                    if (!IsLogOpened)
                        return true;

                    string strClient = "HSMS";
                    strClient += "(" + state.RemoteEndPoint.ToString() + ")";

                    string strLog = string.Format("SendMessage({0} {1}) to {2} : {3}",
                    bytes.Length, bytes.Length > 1 ? "bytes" : "byte", strClient, strSend);

                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLineLog(strLog + strBytes);

                    strLog = string.Format("SendMessage to {0} : {1}",
                        strClient, strSend);
                    FormMain.Instance.AddLog(strLog);

                    return true;
                }
                else
                {
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);
                }
            }
            catch (Exception)
            {
                if (arrDropList == null)
                    OnDropConnection(state);
                else
                    arrDropList.Add(state);

                return false;
            }
        
           
            return false;
        }

        public bool Send(string strSend, ConnectionState state, bool noLock = false, ArrayList arrDropList = null)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            byte[] bytes = encEUC_KR.GetBytes((strSend + "\r").ToArray());

            if (state == null)
            {
                // state가 null이면 모두에게 보낸다.
                bool result = true;

                foreach (ConnectionState _state in m_arrClients)
                {
                    if (!noLock)
                    {
                        lock (this)
                        {
                            if (!_Send(bytes, strSend, _state, arrDropList))
                                result = false;
                        }
                    }
                    else
                    {
                        if (!_Send(bytes, strSend, _state, arrDropList))
                            result = false;
                    }
                }

                return result;
            }
            else
            {
                if (!noLock)
                {
                    lock (this)
                    {
                        return _Send(bytes, strSend, state, arrDropList);
                    }
                }
            }

            return _Send(bytes, strSend, state, arrDropList);
        }

        public ServiceProvider()
        {
            InitLog();
        }

		public override object Clone()
		{
            return this;           
		}

        public override void OnAcceptConnection(ConnectionState state)
        {
            lock (m_arrClients)
            {
                state.LengthAdd = false;
                state.Tag = new ClientData(this, state);
                state.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                m_arrClients.Add(state);

                string strLog = state.RemoteEndPoint.ToString() + " is connected";
                FormMain.Instance.AddLog(strLog);
            }
        }
		
		public override bool OnReceiveData(ConnectionState state)
		{			
            lock (this)
            {				
                if (!base.OnReceiveData(state))
                    return false;

                ClientData client = (ClientData)state.Tag;

                client.OnReceivedData(state.RecivedBuffer);
                state.RecivedBuffer = null;
                return true;
            }
		}

		public override void OnDropConnection(ConnectionState state)
		{
            _OnDropConnection(state, false);
		}

        private void _OnDropConnection(ConnectionState state, bool noLock)
        {
            if (noLock)
            {
                m_arrClients.Remove(state);
            }
            else
            {
                lock (m_arrClients)
                {
                    m_arrClients.Remove(state);
                }
            }

            string strLog = state.IPAddress + ":" + state.PortNo.ToString() + " is connected";
            FormMain.Instance.AddLog(strLog);

            try
            {
                GC.Collect();
            }
            catch (System.Exception)
            {
            }
        }
    }

    public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;

        public static ConnectionLogEx Instance
        {
            get { return (ConnectionLogEx)m_instance; }
        }

        public static bool MakeInstance()
        {
            if (m_instance == null)
                m_instance = new ConnectionLogEx();

            ConnectionLogEx instance = (ConnectionLogEx)m_instance;
            instance.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            instance.m_isOpened = true;
            return instance.m_isOpened;
        }

        public override bool Write(object obj, bool writeTime = true)
        {
            if (obj.GetType() == typeof(Exception))
            {
                Exception e = (Exception)obj;
                if (logger != null)
                    logger.Debug(e.Message, e);
            }
            else
            {
                if (logger != null)
                    logger.DebugFormat("{0}", obj.ToString());
            }
            return true;
        }

        public override bool WriteLine(object obj, bool writeTime = true)
        {
            if (obj.GetType() == typeof(Exception))
            {
                Exception e = (Exception)obj;
                if (logger != null)
                    logger.Debug(e.Message, e);
            }
            else
            {
                if (logger != null)
                    logger.Debug(obj.ToString());
            }
            return true;
        }
    }
}
