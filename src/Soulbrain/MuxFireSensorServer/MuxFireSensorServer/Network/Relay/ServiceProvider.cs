using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace MuxFireSensorServer.Network.Relay
{
    public class ServiceProvider : TcpServiceProvider
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        public enum CommandType { Detect = 0, Restore, RestoreAll };

        private ConcurrentDictionary<ConnectionState, ConnectionState> m_dicClients = new ConcurrentDictionary<ConnectionState, ConnectionState>();

        private bool m_isAliveThread = false;

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        public ServiceProvider()
        {
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

                foreach (ConnectionState state in states)
                {
                    if (state.Connected == false)
                        removeClients.Add(state);
                }

                foreach (ConnectionState state in removeClients)
                {
                    OnDropConnection(state);
                }

                removeClients.Clear();
                Thread.Sleep(1000);
            }
        }

        public bool Send(byte[] bytes, int nOffset, int nLength)
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
        }

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
                string strIP = state.RemoteEndPoint.ToString();
                string strLog = string.Format("[{0}] Client Connect", strIP);
                WriteLineLog(strLog);
            }
        }

        public override bool OnReceiveData(ConnectionState state)
        {
            return base.OnReceiveData(state);
        }

        public override void OnDropConnection(ConnectionState state)
        {
            // 서버가 종료상태면 다른 처리를 하지 않는다.
            if (m_isAliveThread == false)
                return;

            ConnectionState data = null;

            if (m_dicClients.TryRemove(state, out data))
            {
                string strIP = state.RemoteEndPoint.ToString();
                // RemoveClient
                string strLog = string.Format("[{0}] Client Disconnect", strIP);
                WriteLineLog(strLog);
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
        private string m_strLogTag = "Relay";

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
}
