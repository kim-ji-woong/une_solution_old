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
using HSMS;

namespace HSMSServer2
{
    public class ServiceProvider : TcpServiceProvider
    {
		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);
		
        private ArrayList m_arrClients = new ArrayList();
        private bool m_isAliveThread = true;

		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			set { m_bIsLogOpened = value; }
		}

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private object m_sendLock = new object();
        private object m_newClientLock = new object();
        private object m_receiveLock = new object();

        public object NewClientLock
        {
            get { return m_newClientLock; }
        }

		public  void WriteLog(object str)
        {

            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

        public void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        public void InitLog()
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
					if (data.Type == ClientData.ClientType.HSMS_CLIENT)
						strClient = "HSMS Client";
                }

                strClient += "(" + state.IPAddress + ":" + state.PortNo.ToString() + ")";
                //strClient += "(" + state.RemoteEndPoint.ToString() + ")";

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
        private bool _Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, ArrayList arrDropList, bool bAddedLength)
        {
            try
            {
                if (state.Write(bytes, nOffset, nLength, bAddedLength))
                {
                    if (!IsLogOpened)
                        return true;

                    if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
                    {
                        string strClient = "Unknown";

                        ClientData data = (ClientData)state.Tag;

                        if (data != null)
                        {
                            if (data.Type == ClientData.ClientType.HSMS_CLIENT)
                                strClient = "HSMS Client";
                        }

                        strClient += "(" + state.IPAddress + ":" + state.PortNo.ToString() + ")";
                        //strClient += "(" + state.RemoteEndPoint.ToString() + ")";

                        string strLog = string.Format("SendMessage : Header({0}), Length({1}) to {2}", (int)bytes[nOffset], nLength, strClient);
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

        // arrDropList가 null이 아닐 경우, 예외가 발생하면 바로 OnDropConnection()을 호출하지 않고 해당 state를 일단 arrDropList에 담아둔다.
        // m_arrClient Loop 실행 도중 OnDropConnection() 호출로 인하여 m_arrClient가 변경되는 것을 막기 위함이다.
        private bool _Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, ArrayList arrDropList)
        {
            return _Send(bytes, nOffset, nLength, state, arrDropList, state.LengthAdd);
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, bool noLock = false, ArrayList arrDropList = null)
        {
            if (!noLock)
            {
                lock (m_sendLock)
                {
                    return _Send(bytes, nOffset, nLength, state, arrDropList);
                }
            }

            return _Send(bytes, nOffset, nLength, state, arrDropList);
        }

        public bool SendNoLengthBytes(byte[] bytes, int nOffset, int nLength, ConnectionState state, bool noLock = false, ArrayList arrDropList = null)
        {
            if (!noLock)
            {
                lock (m_sendLock)
                {
                    return _Send(bytes, nOffset, nLength, state, arrDropList, false);
                }
            }

            return _Send(bytes, nOffset, nLength, state, arrDropList, false);
        }

        private Thread m_PingThread = null;
        public ServiceProvider()
        {
            InitLog();
            m_PingThread = new Thread(new ThreadStart(PingThread));
            m_PingThread.Start();
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

		private ArrayList m_arTimeHistory = new ArrayList();
		public override void OnAcceptConnection(ConnectionState state)
		{
            if (m_isAliveThread == false)
                return;

            lock (m_arrClients)
            {
                state.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                state.Tag = new ClientDataUnknown(this);
                m_arrClients.Add(state);
                SendMessage(TCP_ID.WHO_ARE_YOU, state);
				NetworkServer.Instance.AddClient(state);			
            }
		}

        // Header만 있는 메시지 보내기
        private void SendMessage(byte header, ConnectionState state)
        {
            byte[] bytes = new byte[6] { header, 0, 0, 0, 0, 0 };
            Send(bytes, 0, bytes.Length, state);
        }

		public override bool OnReceiveData(ConnectionState state)
		{			
            lock (m_receiveLock)
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
		}

        public void SendClientDataOnLoginUser(byte[] bytes, ClientData.ClientType type, bool nolock)
        {
            ArrayList arClient = null;
            lock (m_arrClients)
            {
                arClient = (ArrayList)m_arrClients.Clone();
            }

            foreach (ConnectionState state in arClient)
            {
                ClientData client = (ClientData)state.Tag;

                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (client.LoginUser == true)
                {
                    if (type == ClientData.ClientType.ALL || type == client.Type)
                    {

                        try
                        {
                            Send(bytes, 0, bytes.Length, state, nolock, null);
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                }
            }
        }

        public void SendClientData(byte[] bytes, ClientData.ClientType type, bool nolock)
        {
            ArrayList arClient = null;
            lock (m_arrClients)
            {
                arClient = (ArrayList)m_arrClients.Clone();
            }            

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
                    catch (System.Exception)
                    {                    	
                    }                   
                }
            }  
        }

        public void SendClientData_NoLengthBytes(byte[] bytes, ClientData.ClientType type, bool nolock)
        {
            ArrayList arClient = null;
            lock (m_arrClients)
            {
                arClient = (ArrayList)m_arrClients.Clone();
            }

            foreach (ConnectionState state in arClient)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    try
                    {
                        SendNoLengthBytes(bytes, 0, bytes.Length, state, nolock, null);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
        }
       
        private void ProcessDropList(ArrayList arrDropStates)
        {
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

            LoginManager.Instance.RemoveClient(state);

            if (noLock)
            {                
                m_arrClients.Remove(state);
                NetworkServer.Instance.RemoveClient(state);
            }
            else
            {
                lock (m_arrClients)
                {                   
                    m_arrClients.Remove(state);
                    NetworkServer.Instance.RemoveClient(state);
                }
            }

            try
            {
                GC.Collect();
            }
            catch (System.Exception)
            {
            }
        }

        // 자기 자신을 제외한 다른 클라이언트에 전송
        public void SendDataToOther(byte[] bytes, ClientData sender, bool nolock = false, ClientData.ClientType type = ClientData.ClientType.ALL)
        {
            ArrayList arClient = null;
            lock (m_arrClients)
            {
                arClient = (ArrayList)m_arrClients.Clone();
            }
            ArrayList arrDropStates = null;// new ArrayList();

            foreach (ConnectionState state in arClient)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    if (sender != client)
                        Send(bytes, 0, bytes.Length, state, nolock, arrDropStates);
                }
            }
            ProcessDropList(arrDropStates);
        }
       
        // nClientCount가 0보다 크면 nCount만큼의 Client에게만 데이터를 보낸다.
        public void SendData(byte[] bytes, bool noLock = false, ClientData.ClientType type = ClientData.ClientType.ALL, int nClientCount = -1)
        {
            ArrayList arrDropStates = new ArrayList();

            if (!noLock)
            {
                SendClientData(bytes, type, noLock);
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

                ArrayList arClientList = null;
				lock (m_arrClients)
                {
                    arClientList = (ArrayList)m_arrClients.Clone();
                }

                int nClientCount = arClientList.Count;
                for (int i = arClientList.Count - 1; i >= 0; i--)
                {
                    if (i >= arClientList.Count)
						break;
                    ConnectionState state = (ConnectionState)arClientList[i];
                    ClientData client = (ClientData)state.Tag;

                    if (!state.Connected || client.PingCount >= 3)
                    {        
                        try
                        {
                            state.EndConnection();
                            
                            NetworkServer.Instance.RemoveClient(state);
                            client.TempData = null;  
                        }
                        catch (System.Exception)
                        {                        	
                        }                                                 
                    }
                    else
                    {
                        try
                        {
                            if (Send(data, 0, data.Length, state, true))
                                client.PingCount++;
                        }
                        catch (System.Exception)
                        {
                            //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("이미 삭제된 개체");
                        }                       
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
                    catch (Exception)
                    { }
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
            catch (System.Exception)
            {
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

        public static byte[] MakeBytes(DateTime data)
        {
            int nDataLength = 9;
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DATETIME;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] bytesYear = BitConverter.GetBytes((short)data.Year);
            byte[] bytesMilliSecond = BitConverter.GetBytes((short)data.Millisecond);

            bytes[nCount + 1] = bytesYear[0];
            bytes[nCount + 2] = bytesYear[1];
            bytes[nCount + 3] = (byte)data.Month;
            bytes[nCount + 4] = (byte)data.Day;
            bytes[nCount + 5] = (byte)data.Hour;
            bytes[nCount + 6] = (byte)data.Minute;
            bytes[nCount + 7] = (byte)data.Second;
            bytes[nCount + 8] = bytesMilliSecond[0];
            bytes[nCount + 9] = bytesMilliSecond[1];

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
                else if (type == typeof(DateTime))
                    bytes = MakeBytes((DateTime)data);
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

        private static bool ReadBytes(byte[] bytes, ref int nIndex, ArrayList arrResult, int nByteLength)
        {
            bool isNullData;
            byte type = bytes[nIndex];

            if (type == TCP_TYPE.INTEGER)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 9, out isNullData))
                    return false;

                if (!isNullData)
                {
                    int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                    arrResult.Add(nData);
                }
            }
            else if (type == TCP_TYPE.FLOAT)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 9, out isNullData))
                    return false;

                if (!isNullData)
                {
                    float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                    arrResult.Add(fData);
                }
            }
            else if (type == TCP_TYPE.DOUBLE)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 13, out isNullData))
                    return false;

                if (!isNullData)
                {
                    double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                    arrResult.Add(dData);
                }
            }
            else if (type == TCP_TYPE.LONG)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 13, out isNullData))
                    return false;

                if (!isNullData)
                {
                    long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                    arrResult.Add(lData);
                }
            }
            else if (type == TCP_TYPE.BOOLEAN)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 6, out isNullData))
                    return false;

                if (!isNullData)
                {
                    bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                    arrResult.Add(bData);
                }
            }
            else if (type == TCP_TYPE.SHORT)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 7, out isNullData))
                    return false;

                if (!isNullData)
                {
                    short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                    arrResult.Add(sData);
                }
            }
            else if (type == TCP_TYPE.BYTE)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 6, out isNullData))
                    return false;

                if (!isNullData)
                {
                    byte data = bytes[nIndex - 1];
                    arrResult.Add(data);
                }
            }
            else if (type == TCP_TYPE.STRING)
            {
                if (nByteLength < nIndex + 5)
                    return false;

                int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                if (nDataLength < 0)
                    return false;
                else if (nDataLength > 0)
                {
                    if (nByteLength < nIndex + 5 + nDataLength)
                        return false;

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
            else if (type == TCP_TYPE.DATETIME)
            {
                if (!ReadType(bytes, nByteLength, ref nIndex, 14, out isNullData))
                    return false;

                if (!isNullData)
                {
                    int year = BitConverter.ToInt16(bytes, nIndex - 9);
                    int month = bytes[nIndex - 7];
                    int day = bytes[nIndex - 6];
                    int hour = bytes[nIndex - 5];
                    int min = bytes[nIndex - 4];
                    int sec = bytes[nIndex - 3];
                    int millisec = bytes[nIndex - 2];

                    DateTime dtTime = new DateTime(year, month, day, hour, min, sec, millisec);
                    arrResult.Add(dtTime);
                }
            }
            else
                return false;

            return true;
        }

        public static ArrayList ReadBytes(byte[] bytes, int nIndex = 0)
        {
            if (bytes == null)
                return null;

            int nLength = bytes.Length;
            ArrayList arrResult = new ArrayList();

            while (nIndex < nLength)
            {
                if (!ReadBytes(bytes, ref nIndex, arrResult, nLength))
                    return null;
            }

            return arrResult;
        }

        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            if (bytes == null)
                return null;

            int nLength = bytes.Length;

            if (nLength < 6)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 6;
            //bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                if (!ReadBytes(bytes, ref nIndex, arrResult, nLength))
                    return null;
                /*byte type = bytes[nIndex];

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
                else if (type == TCP_TYPE.DATETIME)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 14, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int year = BitConverter.ToInt16(bytes, nIndex - 9);
                        int month = bytes[nIndex - 7];
                        int day = bytes[nIndex - 6];
                        int hour = bytes[nIndex - 5];
                        int min = bytes[nIndex - 4];
                        int sec = bytes[nIndex - 3];
                        int millisec = bytes[nIndex - 2];

                        DateTime dtTime = new DateTime(year, month, day, hour, min, sec, millisec);
                        arrResult.Add(dtTime);
                    }
                }
                else
                    return null;*/
            }

            return arrResult;
        }

        // arrMembers에 담겨있는 계정들로 접속된 Client들은 Logout 시킨다.
        public void SendLogout(ArrayList arrMembers)
        {
            byte[] bytes = null;

            foreach (LoginInfoEx info in arrMembers)
            {
                ConnectionState state = LoginManager.Instance.GetLoginUser(info.UserID);

                if (state != null)
                {
                    if (bytes == null)
                        bytes = ProcessLogin.MakeLogoutBytes();

                    Send(bytes, 0, bytes.Length, state);
                }
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
