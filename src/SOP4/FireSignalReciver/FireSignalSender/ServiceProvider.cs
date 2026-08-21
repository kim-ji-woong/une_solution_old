using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using SDMS;

namespace FireSignalSender
{

    public class ServiceProvider : TcpServiceProvider
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        //private log4net.ILog logger = null;


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

        private Thread m_PingThread = null;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

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

        private void WriteLineLog(object str, Exception e)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str, e);
        }

        private void InitLog()
        {
            if (ConnectionLogEx.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

        public void RecvLog(byte[] bytes, ConnectionState state)
        {
            if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
            {
                string strClient = "Unknown";

                ClientData data = (ClientData)state.Tag;

                if (data != null)
                {
                    if (data.Type == ClientData.ClientType.SOP_MONITOR2)
                        strClient = "Sensor Monitor";
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
                if (state.Connected == false)
                {
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);

                    return false;
                }


                if (state.WriteAsync(bytes, nOffset, nLength))
                {
                    try
                    {
                        if (!IsLogOpened)
                            return true;

                        if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
                        {
                            StringBuilder sb = new StringBuilder();
                            string strClient = "Unknown";

                            ClientData data = (ClientData)state.Tag;
                            if (data != null)
                            {
                                if (data.Type == ClientData.ClientType.SOP_MONITOR2)
                                    strClient = "Sensor Monitor";
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
                ConnectionLogEx.Instance.WriteLine("_Send", ex);

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

        public ServiceProvider()
        {
            InitLog();

            m_PingThread = new Thread(new ThreadStart(PingThread));
            m_PingThread.Start();
        }

        private ArrayList arSignalReciver = new ArrayList();
        private bool m_bSendSensorData = false;
        public void BeginSendSignal(ClientData data)
        {
            arSignalReciver.Add(data);
            m_bSendSensorData = true;
        }

        public void StopSendSignal(ClientData data)
        {
            if(arSignalReciver.Contains(data))
                arSignalReciver.Remove(data);

            if (arSignalReciver.Count == 0)
                m_bSendSensorData = false;
        }

        public bool SendSensorInfo(ArrayList arrDatas)
        {
            if( m_bSendSensorData == true)
            {
                byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.SENSOR_DATA, arrDatas);
                SendData(bytes, false, ClientData.ClientType.SOP_MONITOR2);
            }
            return m_bSendSensorData;
        }

        public string getinivalue(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int nLen = GetPrivateProfileString(section, key, "", temp, 255, filepath);

            return temp.ToString();
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
                ClientData data = new ClientDataUnknown(this);
                state.Tag = data;
                if (m_arrClients.TryAdd(state, data))
                {
                    SendMessage(TCP_ID.WHO_ARE_YOU, state);
                    NetworkServer.Instance.AddClient(state);
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
                ConnectionLogEx.Instance.WriteLine("SendMessage : " + header, ex);
            }
        }

        public override bool OnReceiveData(ConnectionState state)
        {

            if (!base.OnReceiveData(state))
                return false;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            //WriteByteArray(state.RecivedBuffer);

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
                        ConnectionLogEx.Instance.WriteLine("SendClientData", ex);
                    }
                }
            }
        }

        private void SendReceiverState(byte[] bytes, ClientData.ClientType type, bool noLock, bool noOnDropConnection)
        {
            ArrayList arrDropStates = noOnDropConnection ? null : new ArrayList();

            ICollection<ConnectionState> arClinets = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClinets = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);

            foreach (ConnectionState state in arClinets)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    try
                    {
                        Send(bytes, 0, bytes.Length, state, noLock, arrDropStates);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("SendReciverState", ex);
                    }
                }
            }

            if (!noOnDropConnection)
                ProcessDropList(arrDropStates);

        }

        private void ProcessDropList(ArrayList arrDropStates)
        {
            if (arrDropStates == null)
                return;

            foreach (ConnectionState state in arrDropStates)
            {
                //OnDropConnection(state);
                _OnDropConnection(state, true);
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
                    NetworkServer.Instance.RemoveClient(state);
                }
            }
            else
            {
                DdMonitor.Enter(m_arrClients, true);
                ClientData data = null;
                if (m_arrClients.TryRemove(state, out data))
                {
                    NetworkServer.Instance.RemoveClient(state);
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
                ConnectionLogEx.Instance.WriteLine("CG.Collect", ex);
            }
        }
              

        // 자기 자신을 제외한 다른 클라이언트에 전송
        public void SendDataToOther(byte[] bytes, ClientData sender, bool nolock = false, ClientData.ClientType type = ClientData.ClientType.ALL)
        {
            ICollection<ConnectionState> arClient = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClient = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);

            ArrayList arrDropStates = null;
            foreach (ConnectionState state in arClient)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    if (sender != client)
                    {
                        try
                        {
                            Send(bytes, 0, bytes.Length, state, nolock, arrDropStates);
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("SendDataToOther", ex);
                        }
                    }
                }
            }
            ProcessDropList(arrDropStates);
        }


        private object m_lockObj = new object();
        // nClientCount가 0보다 크면 nCount만큼의 Client에게만 데이터를 보낸다.
        public void SendData(byte[] bytes, bool noLock = false, ClientData.ClientType type = ClientData.ClientType.ALL, int nClientCount = -1)
        {

            if (!noLock)
            {
                lock (m_lockObj)
                {
                    SendClientData(bytes, type, noLock);
                }   
            }
            else
            {
                SendClientData(bytes, type, noLock);
            }
        }
        private static int nCountThread = 0;
        // 연결이 지속되고 있는지 여부를 확인하는 Thread
        private void PingThread()
        {
            byte[] data = new byte[6] { TCP_ID.ARE_YOU_THERE, 0, 0, 0, 0, 0 };
            byte[] data2 = new byte[6] { TCP_ID.WHO_ARE_YOU, 0, 0, 0, 0, 0 };

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
                    if (!state.Connected || client.PingCount > 5)
                    {
                        try
                        {
                            state.EndConnection();
                            NetworkServer.Instance.RemoveClient(state);
                            client.TempData = null;
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("PingThread", ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (Send(data, 0, data.Length, state, true))
                                client.PingCount++;
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("PingThread Send", ex);
                        }

                    }
                }
                //}
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
                        ConnectionLogEx.Instance.WriteLine("PingThread GCCollect", ex);
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
                ConnectionLogEx.Instance.WriteLine("ReleaseThread", ex);
            }
        }

        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] datas = enc.GetBytes(data);

            int nDataLength = datas.Length;

            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(bool data)
        {
            int nDataLength = sizeof(bool);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BOOLEAN;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short data)
        {
            int nDataLength = sizeof(short);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.SHORT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(byte data)
        {
            int nDataLength = sizeof(byte);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BYTE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short nHeader, ArrayList arrDatas)
        {
            int nChunkCount = arrDatas == null ? 0 : arrDatas.Count;

            ArrayList arrBytes = new ArrayList();
            int nBytesCount = 0;

            for (int i = 0; i < nChunkCount; i++)
            {
                object data = arrDatas[i];
                Type type = data.GetType();
                byte[] bytes = null;

                if (type == typeof(int))
                    bytes = MakeBytes((int)data);
                else if (type == typeof(long))
                    bytes = MakeBytes((long)data);
                else if (type == typeof(float))
                    bytes = MakeBytes((float)data);
                else if (type == typeof(bool))
                    bytes = MakeBytes((bool)data);
                else if (type == typeof(double))
                    bytes = MakeBytes((double)data);
                else if (type == typeof(short))
                    bytes = MakeBytes((short)data);
                else if (type == typeof(byte))
                    bytes = MakeBytes((byte)data);
                else if (type == typeof(string))
                    bytes = MakeBytes((string)data);
                else
                    return null;

                nBytesCount += bytes.Length;
                arrBytes.Add(bytes);
            }

            byte[] _bytes = new byte[6 + nBytesCount];
            byte[] headerBytes = BitConverter.GetBytes(nHeader);
            byte[] lengthBytes = BitConverter.GetBytes(nChunkCount);

            _bytes[0] = headerBytes[0];
            _bytes[1] = headerBytes[1];
            _bytes[2] = lengthBytes[0];
            _bytes[3] = lengthBytes[1];
            _bytes[4] = lengthBytes[2];
            _bytes[5] = lengthBytes[3];

            int nIndex = 6;

            foreach (byte[] bytes in arrBytes)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _bytes[nIndex + i] = bytes[i];
                }

                nIndex += bytes.Length;
            }

            return _bytes;
        }

        private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nTotalLength, out bool isNullData)
        {
            isNullData = false;

            if (nBytesLength < nIndex + 5)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;
            else if (nDataLength > 0)
            {
                if (nBytesLength < nIndex + nTotalLength)
                    return false;

                nIndex += nTotalLength;
            }
            else
            {
                isNullData = true;
                nIndex += 5;
            }

            return true;
        }

        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            int nLength = bytes.Length;

            if (nLength < 6)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 6;
            bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                byte type = bytes[nIndex];

                if (type == TCP_TYPE.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                        arrResult.Add(nData);
                    }
                }
                else if (type == TCP_TYPE.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                        arrResult.Add(fData);
                    }
                }
                else if (type == TCP_TYPE.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                        arrResult.Add(dData);
                    }
                }
                else if (type == TCP_TYPE.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                        arrResult.Add(lData);
                    }
                }
                else if (type == TCP_TYPE.BOOLEAN)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                        arrResult.Add(bData);
                    }
                }
                else if (type == TCP_TYPE.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 7, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                        arrResult.Add(sData);
                    }
                }
                else if (type == TCP_TYPE.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        byte data = bytes[nIndex - 1];
                        arrResult.Add(data);
                    }
                }
                else if (type == TCP_TYPE.STRING)
                {
                    if (nLength < nIndex + 5)
                        return null;

                    int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                    if (nDataLength < 0)
                        return null;
                    else if (nDataLength > 0)
                    {
                        if (nLength < nIndex + 5 + nDataLength)
                            return null;

                        string strData = Encoding.UTF8.GetString(bytes, nIndex + 5, nDataLength);
                        arrResult.Add(strData);

                        nIndex += 5 + nDataLength;
                    }
                    else
                    {
                        arrResult.Add("");
                        nIndex += 5;
                    }
                }
                else
                    return null;
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

    public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get
            {
                return m_instance2;
                //return (ConnectionLogEx)m_instance;    
            }
        }

        public static bool MakeInstance()
        {
            /*if (m_instance == null)
                m_instance = new ConnectionLogEx();

            if ((m_instance is ConnectionLogEx) == false)
                return false;

            ConnectionLogEx instance = (ConnectionLogEx)m_instance;*/
            m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
    }
}
