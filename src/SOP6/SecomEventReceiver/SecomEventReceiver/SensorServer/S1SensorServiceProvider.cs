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

using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using DBUtility2;

namespace SecomEventReceiver
{
    public class S1NetworkServiceProvider : TcpServiceProvider
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        private ConcurrentDictionary<ConnectionState, ClientData> m_arrClients = new ConcurrentDictionary<ConnectionState, ClientData>();
        public object LockObject
        {
            get { return m_arrClients; }
        }

        private bool m_isAliveThread = true;

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private Thread m_PingThread = null;
        public S1NetworkServiceProvider()
        {

            InitLog();
            ReadOption();
            
            
            m_PingThread = new Thread(new ThreadStart(PingThread));
            m_PingThread.Start();
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

            if (bytes[0] != SOPWebServer.Header.I_AM_HERE || !m_exceptPingLog)
            {
                string strClient = "Unknown";

                ClientData data = (ClientData)state.Tag;

                if (data != null)
                {
                    if (data.Type == ClientData.ClientType.SIEMENS)
                        strClient = "SIEMENS";

                    else if (data.Type == ClientData.ClientType.PSMTester)
                        strClient = " PSMTester";
                }

                strClient += "(" + state.RemoteEndPoint.ToString() + ")";

                string strLog = string.Format("RecvMessage : Header({0}), Length({1}) from {2}", (int)bytes[0], (int)bytes.Length, strClient);
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
        }

        // arrDropList가 null이 아닐 경우, 예외가 발생하면 바로 OnDropConnection()을 호출하지 않고 해당 state를 일단 arrDropList에 담아둔다.
        // m_arrClient Loop 실행 도중 OnDropConnection() 호출로 인하여 m_arrClient가 변경되는 것을 막기 위함이다.
        private bool _Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, ArrayList arrDropList)
        {
            try
            {
                if (state.WriteAsync(bytes, nOffset, nLength))
                {
                    try
                    {
                        if (!IsLogOpened)
                            return true;

                        if (bytes[nOffset+1] != 0x06 || !m_exceptPingLog)
                        {
                            StringBuilder sb = new StringBuilder();
                            string strClient = "Unknown";

                            ClientData data = (ClientData)state.Tag;
                            if (data != null)
                            {
                                if (data.Type == ClientData.ClientType.SIEMENS)
                                    strClient = "SIEMENS";
                                else if (data.Type == ClientData.ClientType.PSMTester)
                                    strClient = " PSMTester";
                            }

                            string szRemote = state.RemoteEndPoint.ToString();

                            sb.AppendFormat("SendMessage : Header({0}), Length({1}) to {2}({3})", (int)bytes[nOffset], nLength, strClient, szRemote);

                            bool bFirst = true;

                            foreach (byte b in bytes)
                            {
                                if (bFirst == true)
                                {
                                    bFirst = false;
                                    sb.AppendFormat("\r\n\t\t{0:X2}", (int)b);
                                }
                                else
                                    sb.AppendFormat(" {0:X2}", (int)b);
                            }

                            WriteLineLog(sb.ToString());
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
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);
                }
            }
            catch (Exception ex)
            {
                ConnectionLogClient.Instance.WriteLine("_Send", ex);

                if (arrDropList == null)
                    OnDropConnection(state);
                else
                    arrDropList.Add(state);

                return false;
            }
            return false;
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, bool noLock = false, ArrayList arrDropList = null)
        {
            if (!noLock)
            {
                return _Send(bytes, nOffset, nLength, state, arrDropList);
            }
            return _Send(bytes, nOffset, nLength, state, arrDropList);
        }

        

        public string getinivalue(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int nLen = GetPrivateProfileString(section, key, "", temp, 255, filepath);

            return temp.ToString();

        }

        private void ReadOption()
        {
            WebDBManager dbMgr = S1NetworkServer.Instance.DBManager;
            string strSOPTimeout = "", strDetectTimeout = "", strNotifyTimeout = "";

            //if (GetDBOption(dbMgr, "OptionSOPSimulator", "SopTimeout", ref strSOPTimeout))
            //    double.TryParse(strSOPTimeout, out m_dSOPTimeout);

            //if (GetDBOption(dbMgr, "OptionSDMS", "DetectFireTimeout", ref strDetectTimeout))
            //    double.TryParse(strDetectTimeout, out m_dDetectFireTimeout);

            //if (GetDBOption(dbMgr, "OptionSDMS", "NotifyFireTimeout", ref strNotifyTimeout))
            //    double.TryParse(strNotifyTimeout, out m_dNotifyFireTimeout);

        }

        private bool GetDBOption(WebDBManager dbMgr, string strTableName, string strPropertyName, ref string strValue)
        {
            string strSQL = "Select PropertyValue from " + strTableName + " where PropertyName = '" + strPropertyName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            strValue = arrResult[0].ToString();
            return true;
        }

        public override object Clone()
        {
            return this;
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            if (m_isAliveThread == false)
                return;

            //lock (m_arrClients)
            {
                //state.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);

                state.LengthAdd = false;

                ClientData data = new ClientDataS1SensorTester(this, state);
                state.Tag = data;
                if (m_arrClients.TryAdd(state, data))
                {
                    //SendMessage(TCP_ID.WHO_ARE_YOU, state);
                    S1NetworkServer.Instance.AddClient(state);
                }
            }
        }

        // Header만 있는 메시지 보내기
        private void SendMessage(byte header, ConnectionState state)
        {
            byte[] bytes = new byte[6] { header, 0, 0, 0, 0, 0 };
            try
            {
                Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogClient.Instance.WriteLine("SendMessage : " + header, ex);
            }
        }

        private object m_bLockObj = new object();

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

        public void SendClientData(byte[] bytes, ClientData.ClientType type, bool nolock)
        {
            ICollection<ConnectionState> arClient = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClient = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);

            foreach (ConnectionState state in arClient)
            {

                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    try
                    {
                        Send(bytes, 0, bytes.Length, state, nolock, null);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogClient.Instance.WriteLine("SendClientData", ex);
                    }
                }
            }
        }

        public override void OnDropConnection(ConnectionState state)
        {
            _OnDropConnection(state, false);
        }

        private void _OnDropConnection(ConnectionState state, bool noLock)
        {
            // 서버가 종료상태면 다른 처리를 하지 않는다.
            if (m_isAliveThread == false)
                return;

            if (noLock)
            {
                ClientData data = null;
                if (m_arrClients.TryRemove(state, out data))
                {
                    data.Close();
                    S1NetworkServer.Instance.RemoveClient(state);
                }
            }
            else
            {
                DdMonitor.Enter(m_arrClients, true);
                ClientData data = null;
                if (m_arrClients.TryRemove(state, out data))
                {
                    data.Close();
                    S1NetworkServer.Instance.RemoveClient(state);
                }
                DdMonitor.Exit(m_arrClients, true);
            }

            ClientData client = (ClientData)state.Tag;
            client.TempData = null;

            try
            {
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                ConnectionLogClient.Instance.WriteLine("CG.Collect", ex);
            }
        }

        // nClientCount가 0보다 크면 nCount만큼의 Client에게만 데이터를 보낸다.
        public void SendData(byte[] bytes, bool noLock = false, ClientData.ClientType type = ClientData.ClientType.ALL, int nClientCount = -1)
        {

            if (!noLock)
            {
                SendClientData(bytes, type, noLock);
            }
            else
            {
                SendClientData(bytes, type, noLock);
            }
        }

        //private static int nCountThread = 0;
        //// 연결이 지속되고 있는지 여부를 확인하는 Thread
        private void PingThread()
        {
            int nCountThread = 0;
            while (m_isAliveThread)
            {
                ICollection<ConnectionState> arClientList = null;
                DdMonitor.Enter(m_arrClients, false);
                {
                    arClientList = m_arrClients.Keys;
                }
                DdMonitor.Exit(m_arrClients, false);

                int nClientCount = arClientList.Count;

                foreach (ConnectionState state in arClientList)
                {
                    ClientData client = (ClientData)state.Tag;
                    if (!state.Connected || client.PingCount > 10000)
                    {
                        try
                        {
                            state.EndConnection();
                            S1NetworkServer.Instance.RemoveClient(state);
                            client.TempData = null;
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogClient.Instance.WriteLine("PingThread", ex);
                        }
                    }
                    else
                    {
                      //  NetworkServer.Instance.ServiceProvider.SendACK(state);
                        client.PingCount++;
                    }
                }

                Thread.Sleep(1000);

                nCountThread++;

                if (nCountThread == 3600)
                {
                    nCountThread = 0;
                    try
                    {
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        ConnectionLogClient.Instance.WriteLine("PingThread GCCollect", ex);
                    }

                }
            }
        }

        public void ReleaseThread()
        {
            m_isAliveThread = false;

            // 쓰레드 종료를 2초간 기다린다.
            Thread.Sleep(2000);

            try
            {
                if (m_PingThread.IsAlive)
                {
                    m_PingThread.Abort();
                    m_PingThread.Join();
                }
            }
            catch (System.Exception ex)
            {
                ConnectionLogClient.Instance.WriteLine("ReleaseThread", ex);
            }
        }

        public void SendACK(ConnectionState state)
        {
            byte[] datas = new byte[3];
            datas[0] = 0x02;
            datas[1] = 0x06;
            datas[2] = 0x03;

            Send(datas, 0, 3, state);
        }
        
        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;
            ArrayList arrResult = new ArrayList();


            ConnectionLogClient.Instance.WriteLine("BufferLength : " + bytes.Length);

            short stx = bytes[0];
            
            arrResult.Add(stx);

            int nLength1 = (bytes[1] - (byte)0x80);
            int nLength2 = (bytes[2] - (byte)0x80);
            int nLength = nLength1 * 128 + nLength2 + 2;
            ConnectionLogClient.Instance.WriteLine("Length Field : " + nLength);

            arrResult.Add(nLength);

            int nDataLength = nLength - 8;

            short tx = bytes[3];
            arrResult.Add(tx);

            short ty = bytes[4];
            arrResult.Add(ty);

            short nOpCode = bytes[5];
            arrResult.Add(nOpCode);
            nHeader = nOpCode;

            short nSeq = bytes[6];
            arrResult.Add(nSeq);


            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);

            string szData = encEUC_KR.GetString(bytes, 7, nDataLength);
            //string szData = Encoding.UTF8.GetString(bytes, 7, nDataLength);
            ConnectionLogClient.Instance.WriteLine("DataLEngth : " + szData);
            char [] sep = {','};
            string[] splits = szData.Split(sep);
            if( splits.Length > 0)
            {
                string szDate = splits[0];
                arrResult.Add(szDate);
                if (splits.Length > 1)
                {
                    string szAddress = splits[1];
                    arrResult.Add(szAddress);
                }
                if (splits.Length >2)
                {
                    string szAreaName = splits[2];
                    arrResult.Add(szAreaName);
                }
                if (splits.Length > 2)
                {
                    string szTargetName = splits[3];
                    arrResult.Add(szTargetName);
                }               
                if (splits.Length == 5)
                {
                    string szMessage = splits[4];
                    arrResult.Add(szMessage);
                }
            }

            return arrResult;
        }
    }

    public class ArrayListEx : ArrayList
    {
        public ArrayListEx()
        {
        }

        public override int Add(object value)
        {
            return base.Add(value);
        }
    }

    public class ConnectionLogClient : ConnectionLog
    {
        private log4net.ILog logger = null;
        private static ConnectionLogClient m_instance2 = new ConnectionLogClient();

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

            SetLevel("Log4net.Client", "DEBUG");
            AddAppender("Log4net.Client", CreateFileAppender("Client", "SensorData.log"));                       
            m_instance2.logger = log4net.LogManager.GetLogger("Log4net.Client");
           
         
            m_instance2.m_isOpened = true;
            return m_instance2.m_isOpened;
        }

        public override bool Write(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.DebugFormat("{0}", str);

            return true;
        }

        public override bool WriteLine(object str, Exception e)
        {
            if (logger != null)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                logger.Debug("프로그램 오류 : " + str, e);
                logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
            }
            return true;
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.Debug(str);

            return true;
        }

        public static void SetLevel(string loggerName, string levelName)
        {
            ILog log = log4net.LogManager.GetLogger(loggerName);
            Logger l = (Logger)log.Logger;

            l.Level = l.Hierarchy.LevelMap[levelName];
            }


        public static void AddAppender(string loggerName, IAppender appender)
        {
            ILog log = log4net.LogManager.GetLogger(loggerName);
            Logger l = (Logger)log.Logger;

            l.AddAppender(appender);
        }

        public static IAppender CreateFileAppender(string name, string fileName)
        {
            FileAppender appender = new log4net.Appender.RollingFileAppender();
            appender.Name = name;
            appender.File = fileName;
            appender.AppendToFile = true;

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "[%-5p][%d{yyyy-MM-dd HH:mm:ss}] : %m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }
    }
}
