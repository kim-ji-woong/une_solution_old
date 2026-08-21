using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using libUSS;

namespace SampleServer
{
    public class ServiceProvider : TcpServiceProvider
    {
        private ConcurrentDictionary<ConnectionState, ConnectionState> m_dicClients = new ConcurrentDictionary<ConnectionState, ConnectionState>();
        private bool m_isAliveThread = false;
        private IServiceOwner m_owner = null;

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        public ServiceProvider(IServiceOwner owner)
        {
            m_owner = owner;
            InitLog();

            Thread t = new Thread(new ThreadStart(PingThread));
            t.Start();
        }

        public void WriteLog(object str)
        {
            if (ConnectionLogClient.Instance.IsOpened)
                ConnectionLogClient.Instance.Write(str);
        }

        public void WriteLineLog(object str)
        {
            if (ConnectionLogClient.Instance.IsOpened)
                ConnectionLogClient.Instance.WriteLine(str);
        }

        public void WriteLineLog(object str, Exception e)
        {
            if (ConnectionLogClient.Instance.IsOpened)
                ConnectionLogClient.Instance.WriteLine(str, e);
        }

        private void InitLog()
        {
            if (ConnectionLogClient.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

        private void PingThread()
        {
            m_isAliveThread = true;
            List<ConnectionState> removeClients = new List<ConnectionState>();

            while (m_isAliveThread)
            {
                List<ConnectionState> states = m_dicClients.Values.ToList();
                DateTime dtNow = DateTime.Now;

                foreach (ConnectionState state in states)
                {
                    if (state.Connected == false)
                        removeClients.Add(state);
                    else
                    {
                        ClientData data = (ClientData)state.Tag;

                        if (data != null)
                        {
                            TimeSpan span = dtNow - data.LastMessageTime;

                            if (span.TotalSeconds > 5.0)
                            {
                                state.EndConnection();
                            }
                            else if (span.TotalSeconds > 1.0)
                            {
                                // 마지막으로 데이터를 받은뒤 1초가 지났으면 AreYouThere를 보낸다.
                                SendData(Header.ARE_YOU_THERE, state);
                            }
                        }
                    }
                }

                foreach (ConnectionState state in removeClients)
                {
                    OnDropConnection(state);
                }

                removeClients.Clear();
                Thread.Sleep(1000);
            }
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header, ConnectionState state)
        {
            byte[] bytes = new byte[4];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes((short)0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];

            Send(bytes, 0, bytes.Length, state);
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state)
        {
            try
            {
                if (state.WriteAsync(bytes, nOffset, nLength))
                {
                    try
                    {
                        if (!IsLogOpened)
                            return true;

                        string szRemote = state.RemoteEndPoint.ToString();
                        string strLog = string.Format("[{0}] SendMessage : Length({1})", szRemote, nLength);

                        bool bFirst = true;

                        foreach (byte b in bytes)
                        {
                            if (bFirst == true)
                            {
                                bFirst = false;
                                strLog += string.Format("\r\n\t\t{0:X2}", (int)b);
                            }
                            else
                                strLog += string.Format(" {0:X2}", (int)b);
                        }

                        WriteLineLog(strLog);
                    }
                    catch (System.Exception exx)
                    {
                        WriteLineLog("Write Send log", exx);
                    }
                    return true;
                }
                else
                {
                    OnDropConnection(state);
                }
            }
            catch (Exception ex)
            {
                ConnectionLogClient.Instance.WriteLine("Send", ex);
                OnDropConnection(state);
            }

            return false;
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, byte eventType)
        {
            List<ConnectionState> states = m_dicClients.Values.ToList();
            bool result = true;

            foreach (ConnectionState state in states)
            {
                ClientData client = (ClientData)state.Tag;

                if (client == null || client.HasEvent(eventType) == false)
                    continue;

                if (Send(bytes, nOffset, nLength, state) == false)
                    result = false;
            }

            return result;
        }

        /*public bool Send(byte[] bytes, int nOffset, int nLength)
        {
            List<ConnectionState> states = m_dicClients.Values.ToList();

            foreach (ConnectionState state in states)
            {
                try
                {
                    if (state.WriteAsync(bytes, nOffset, nLength))
                    {
                        try
                        {
                            if (!IsLogOpened)
                                return true;

                            string szRemote = state.RemoteEndPoint.ToString();
                            string strLog = string.Format("[{0}] SendMessage : Length({1})", szRemote, nLength);

                            bool bFirst = true;

                            foreach (byte b in bytes)
                            {
                                if (bFirst == true)
                                {
                                    bFirst = false;
                                    strLog += string.Format("\r\n\t\t{0:X2}", (int)b);
                                }
                                else
                                    strLog += string.Format(" {0:X2}", (int)b);
                            }

                            WriteLineLog(strLog);
                        }
                        catch (System.Exception exx)
                        {
                            WriteLineLog("Write Send log", exx);
                        }
                        return true;
                    }
                    else
                    {
                        OnDropConnection(state);
                    }
                }
                catch (Exception ex)
                {
                    ConnectionLogClient.Instance.WriteLine("Send", ex);
                    OnDropConnection(state);
                    return false;
                }
            }

            return false;
        }*/

        public override object Clone()
        {
            return this;
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            if (m_isAliveThread == false)
                return;

            state.LengthAdd = false;

            if (m_dicClients.TryAdd(state, state))
            {
                ClientData client = new ClientData(this);
                client.EndPoint = state.RemoteEndPoint.ToString();
                state.Tag = client;

                if (m_owner != null)
                    m_owner.OnAccept(state);

                string strIP = state.RemoteEndPoint.ToString();
                string strLog = string.Format("[{0}] Client Connect", strIP);
                WriteLineLog(strLog);
            }
        }

        public override bool OnReceiveData(ConnectionState state)
        {
            if (base.OnReceiveData(state) == false)
                return false;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            bool bResult = client.OnReceive(state, state.RecivedBuffer);
            state.RecivedBuffer = null;
            return bResult;
        }

        public override void OnDropConnection(ConnectionState state)
        {
            // 서버가 종료상태면 다른 처리를 하지 않는다.
            if (m_isAliveThread == false)
                return;

            ConnectionState data = null;

            if (m_dicClients.TryRemove(state, out data))
            {
                if (m_owner != null)
                    m_owner.OnDropConnection(state);

                ClientData client = (ClientData)state.Tag;

                if (client != null)
                {
                    string strIP = client.EndPoint;
                    // RemoveClient
                    string strLog = string.Format("[{0}] Client Disconnect", strIP);
                    WriteLineLog(strLog);
                }
            }
        }

        public void ReleaseThread()
        {
            m_isAliveThread = false;
        }
    }

    public class ConnectionLogClient : ConnectionLog
    {
        private static ConnectionLogClient m_instance2 = new ConnectionLogClient();
        private string m_strLogTag = "USS";

        private static int m_nPrevYear = 0, m_nPrevMonth = 0, m_nPrevDay = 0;

        private StreamWriter m_writer = null;

        public static ConnectionLogClient Instance
        {
            get
            {
                return m_instance2;
            }
        }

        public static bool MakeInstance()
        {
            if (m_instance2.m_isOpened == true)
                return true;

            m_instance2.m_isOpened = true;
            return m_instance2.m_isOpened;
        }

        public override bool Write(object str, bool writeTime = true)
        {
            return _Write(str.ToString(), false, writeTime);
        }

        public override bool WriteLine(object str, Exception e)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
            string strLog = "프로그램 오류 : " + str + ", " + e.Message;
            strLog += "\r\nLine : " + trace.GetFrame(0).GetFileLineNumber().ToString();

            return _Write(strLog, true, true);
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            return _Write(str.ToString(), true, writeTime);
        }

        private bool _Write(string strLog, bool lineFeed, bool writeTime)
        {
            DateTime dtNow = DateTime.Now;

            string strFilePath = string.Format("{3}_{0}{1:00}{2:00}.log", dtNow.Year, dtNow.Month, dtNow.Day, m_strLogTag);
            StreamWriter writer = m_writer;

            try
            {
                if (!File.Exists(strFilePath))
                {
                    if (writer != null)
                        writer.Close();

                    writer = new StreamWriter(strFilePath, false, Encoding.UTF8);
                }
                else if (writer == null)
                {
                    writer = new StreamWriter(strFilePath, true, Encoding.UTF8);
                }

                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                string str = "[" + strTime + "] " + strLog;

                if (lineFeed)
                    writer.WriteLine(str);
                else
                    writer.Write(str);

                writer.Flush();
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                }

                m_writer = null;
                return false;
            }

            m_writer = writer;
            return true;
        }
    }

    public interface IServiceOwner
    {
        void OnAccept(ConnectionState state);
        void OnDropConnection(ConnectionState state);
    }
}
