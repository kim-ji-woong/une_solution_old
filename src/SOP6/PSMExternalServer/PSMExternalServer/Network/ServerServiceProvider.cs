using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TcpLib2;

namespace PSMExternalServer.Network
{
    public class ServerServiceProvider : TcpServiceProvider
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        public enum CommandType { Detect = 0, Restore, RestoreAll };

        private ConcurrentDictionary<ConnectionState, ClientData> m_dicClients = new ConcurrentDictionary<ConnectionState, ClientData>();
        
        private bool m_isAliveThread = false;

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        public ServerServiceProvider()
        {
            InitLog();

            Thread pingThread = new Thread(new ThreadStart(PingThread));
            pingThread.Start();
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

        public void RecvLog(byte[] bytes, ConnectionState state)
        {
            if (!IsLogOpened)
                return;

            bool isAck = IsAck(bytes);

            if (isAck && m_exceptPingLog)
                return;

            string strClient = "[" + state.RemoteEndPoint.ToString() + "]";

            string strLog = string.Format("RecvMessage : Length({0})", bytes.Length);
            string strBytes = "";

            foreach (byte b in bytes)
            {
                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLineLog(strLog + strBytes);
        }

        private bool IsAck(byte[] bytes)
        {
            if (bytes.Count() >= 3)
            {
                if (bytes[0] == SERIAL_ID.STX && bytes[1] == SERIAL_ID.ACK && bytes[2] == SERIAL_ID.ETX)
                    return true;
            }

            return false;
        }

        private bool IsPoll(byte[] bytes)
        {
            if (bytes.Count() == 5)
            {
                if (bytes[0] == SERIAL_ID.STX && (char)bytes[1] == 'P' && (char)bytes[2] == 'O' && (char)bytes[3] == 'L' && bytes[4] == SERIAL_ID.ETX)
                    return true;
            }

            return false;
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

                        if (IsPoll(bytes) == false || !m_exceptPingLog)
                        {
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

            ClientData data = new ClientData(this, state);
            state.Tag = data;

            if (m_dicClients.TryAdd(state, data))
            {
                string strIP = state.RemoteEndPoint.ToString();
                string strLog = string.Format("[{0}] Client Connect", strIP);
                WriteLineLog(strLog);
            }
        }

        public override bool OnReceiveData(ConnectionState state)
        {
            if (!base.OnReceiveData(state))
                return false;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            bool bResult = client.OnReceiveData(state, state.RecivedBuffer);
            state.RecivedBuffer = null;
            return bResult;
        }

        private void WriteByteArray(byte[] bytes)
        {
            Debug.Write("{");
            for (int i = 0; i < bytes.Length; i++)
            {
                Debug.Write(string.Format("{0:X}", bytes[i]));
                Debug.Write(" ");
            }
            Debug.WriteLine("}");
        }

        public override void OnDropConnection(ConnectionState state)
        {
            // 서버가 종료상태면 다른 처리를 하지 않는다.
            if (m_isAliveThread == false)
                return;

            ClientData data = null;

            if (m_dicClients.TryRemove(state, out data))
            {
                // RemoveClient
            }

            try
            {
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                ConnectionLogClient.Instance.WriteLine("CG.Collect", ex);
            }
        }
        
        private void PingThread()
        {
            m_isAliveThread = true;

            byte[] poll = new byte[5];

            poll[0] = 0x02;
            poll[1] = (byte)'P';
            poll[2] = (byte)'O';
            poll[3] = (byte)'L';
            poll[4] = 0x03;

            int nBytesCount = poll.Count();

            List<ConnectionState> removes = new List<ConnectionState>();

            while (m_isAliveThread)
            {
                List<ConnectionState> clients = m_dicClients.Keys.ToList();

                foreach (ConnectionState state in clients)
                {
                    try
                    {
                        if (Send(poll, 0, nBytesCount, state) == false)
                            removes.Add(state);
                        else
                        {
                            if (state.Tag != null && state.Tag is ClientData)
                            {
                                ClientData data = (ClientData)state.Tag;
                                data.PingCount = 0;
                            }
                            else
                                removes.Add(state);
                        }
                    }
                    catch (Exception)
                    {
                        removes.Add(state);
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (m_isAliveThread == false)
                        return;

                    System.Threading.Thread.Sleep(1000);
                }

                foreach (ConnectionState state in clients)
                {
                    if (removes.Contains(state))
                        continue;

                    // Poll을 보낸것에 대한 응답이 없으면 접속이 끊어진 것으로 간주한다.
                    ClientData data = (ClientData)state.Tag;

                    if (data.PingCount == 0)
                        removes.Add(state);
                }

                ClientData _data;

                foreach (ConnectionState state in removes)
                {
                    try
                    {
                        state.EndConnection();
                    }
                    catch (Exception)
                    {
                    }

                    m_dicClients.TryRemove(state, out _data);
                }

                removes.Clear();
            }
        }

        public void ReleaseThread()
        {
            m_isAliveThread = false;
            ConnectionLogClient.Instance.WriteLine("ReleaseThread");
        }

        public bool SendCommand(string strValue, CommandType cmd)
        {
            if (strValue.Length != 6)
                return false;

            int nBytesLength = 12;
            byte[] bytes = new byte[nBytesLength];

            bytes[0] = SERIAL_ID.STX;
            bytes[1] = (byte)strValue.ElementAt(0);
            bytes[2] = (byte)strValue.ElementAt(1);
            bytes[3] = (byte)'-';
            bytes[4] = (byte)strValue.ElementAt(2);
            bytes[5] = (byte)strValue.ElementAt(3);
            bytes[6] = (byte)'-';
            bytes[7] = (byte)strValue.ElementAt(4);
            bytes[8] = (byte)strValue.ElementAt(5);
            bytes[9] = GetCommandByte(cmd);
            bytes[11] = SERIAL_ID.ETX;

            int checkSum = 0;
            
            for (int i=0;i<nBytesLength;i++)
            {
                if (i == 10)
                    continue;

                checkSum += (int)bytes[i];
            }

            checkSum = checkSum % 16 + 0x30;
            bytes[10] = (byte)checkSum;

            List<ConnectionState> clients = m_dicClients.Keys.ToList();
            bool result = true;

            foreach (ConnectionState state in clients)
            {
                if (state.Tag != null && state.Tag is ClientData)
                {
                    ClientData data = (ClientData)state.Tag;

                    if (Send(bytes, 0, nBytesLength, state) == false)
                    {
                        result = false;
                    }
                    else
                        data.LastSendBytes = bytes;
                }
            }

            return result;
        }

        private byte GetCommandByte(CommandType cmd)
        {
            if (cmd == CommandType.Detect)
                return (byte)'N';
            else if (cmd == CommandType.Restore)
                return (byte)'F';
            //else if (cmd == CommandType.RestoreAll)
                return (byte)'R';
        }
    }

    public class ConnectionLogClient : ConnectionLog
    {
        private static ConnectionLogClient m_instance2 = new ConnectionLogClient();

        private string m_strLogFolder = "";
        private double m_dLogLifeDays = 30;
        private string m_strLogTag = "";

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

            string strLogFolder = System.Configuration.ConfigurationManager.AppSettings["logFolder"].ToString().Replace('/', '\\');
            m_instance2.m_strLogTag = System.Configuration.ConfigurationManager.AppSettings["logFileTag"].ToString();

            string strLifeTime = System.Configuration.ConfigurationManager.AppSettings["logLifeTime"].ToString();
            double.TryParse(strLifeTime, out m_instance2.m_dLogLifeDays);

            if (strLogFolder.StartsWith("."))
            {
                m_instance2.m_strLogFolder = System.Windows.Forms.Application.StartupPath + "\\" + strLogFolder;
            }
            else if (strLogFolder.StartsWith("\\"))
            {
                m_instance2.m_strLogFolder = System.Windows.Forms.Application.StartupPath + strLogFolder;
            }
            else
                m_instance2.m_strLogFolder = strLogFolder;

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
            if (m_strLogFolder.Length == 0)
                return false;

            if (!Directory.Exists(m_strLogFolder))
                Directory.CreateDirectory(m_strLogFolder);

            DateTime dtNow = DateTime.Now;

            string strFilePath = m_strLogFolder + string.Format("\\{0}{1:00}{2:00}_{3}.log", dtNow.Year, dtNow.Month, dtNow.Day, m_strLogTag);
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
