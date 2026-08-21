using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Threading;
using SDMS;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;


namespace SOPChecker
{

    public class ServiceProvider : TcpServiceProvider
    {
		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);
				
		//private log4net.ILog logger = null;

        private ArrayList m_arrClients = new ArrayList();
        //private bool m_isLock = false;
        private bool m_isAliveThread = true;
                 
		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			set { m_bIsLogOpened = value; }
		}

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

        private void InitLog()
        {
			if (ConnectionLogExEx.MakeInstance())
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
                    if (data.Type == ClientData.ClientType.CONTROLOR)
						strClient = "CONTROLER";                   
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

        private bool _Send(byte[] bytes, int nOffset, int nLength, ConnectionState state)
        {
            if (state.Write(bytes, nOffset, nLength))
            {
                if (!IsLogOpened)
                    return true;

                if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
                {
                    string strClient = "Unknown";

                    ClientData data = (ClientData)state.Tag;

                    if (data != null)
                    {
                        if (data.Type == ClientData.ClientType.CONTROLOR)
                            strClient = "CONTROLOR";                     
                    }

                    strClient += "(" + state.RemoteEndPoint.ToString() + ")";

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

            return false;
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, bool noLock = false)
        {
            if (!noLock)
            {
                lock (this)
                {
                    return _Send(bytes, nOffset, nLength, state);
                }
            }

            return _Send(bytes, nOffset, nLength, state);
			
        }

        public ServiceProvider()
        {
            InitLog();
            ReadOption();
            Thread t = new Thread(new ThreadStart(PingThread));
            t.Start();
        }
        

        public string getinivalue(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int nLen = GetPrivateProfileString(section, key, "", temp, 255, filepath);

            return temp.ToString();

        }

        private void ReadOption()
        {
			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

        }

		public override object Clone()
		{
            return this;           
		}       	

		private ArrayList m_arTimeHistory = new ArrayList();
		public override void OnAcceptConnection(ConnectionState state)
		{
            lock (m_arrClients)
            {
                state.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                //state.Tag = new ClientData();
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
        
		private object m_bLockObj = new object();		
		
		public override bool OnReceiveData(ConnectionState state)
		{			
            lock (this)
            {				
                if (!base.OnReceiveData(state))
                    return false;

                ClientData client = (ClientData)state.Tag;
                if (client == null)
                    return false;

                bool bResult =  client.OnReceiveData(state, state.RecivedBuffer);
                state.RecivedBuffer = null;
                return bResult;

            }
		}

		public override void OnDropConnection(ConnectionState state)
		{
            lock (m_arrClients)
            {
                m_arrClients.Remove(state);


                NetworkServer.Instance.RemoveClient(state);
            }

            ClientData client = (ClientData)state.Tag;

            client.TempData = null;
		}
        
        // nClientCount가 0보다 크면 nCount만큼의 Client에게만 데이터를 보낸다.
        public void SendData(byte[] bytes, bool noLock = false, ClientData.ClientType type = ClientData.ClientType.ALL, int nClientCount = -1)
        {
            int nCount = 0;

            if (!noLock)
            {
				lock (m_arrClients)
                {
                    foreach (ConnectionState state in m_arrClients)
                    {
                        ClientData client = (ClientData)state.Tag;
                        if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                            continue;

                        if (client.Type == type || type == ClientData.ClientType.ALL)
                        {
                            Send(bytes, 0, bytes.Length, state, true);
                            nCount++;
                        }

                        if (nClientCount > 0 && nCount >= nClientCount)
                            return;
                    }
                }
            }
            else
            {
                foreach (ConnectionState state in m_arrClients)
                {
                    ClientData client = (ClientData)state.Tag;
                    if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                        continue;

                    if (client.Type == type || type == ClientData.ClientType.ALL)
                    {
                        Send(bytes, 0, bytes.Length, state);
                        nCount++;
                    }

                    if (nClientCount > 0 && nCount >= nClientCount)
                        return;
                }
            }
        }

        // 연결이 지속되고 있는지 여부를 확인하는 Thread
        private void PingThread()
        {
            byte[] data = new byte[6] { TCP_ID.ARE_YOU_THERE, 0, 0, 0, 0, 0 };
            byte[] data2 = new byte[6] { TCP_ID.WHO_ARE_YOU, 0, 0, 0, 0, 0 };

            while (m_isAliveThread)
            {
				lock (m_arrClients)
                {
                    int nClientCount = m_arrClients.Count;

					for (int i = m_arrClients.Count - 1; i >= 0; i--)
                    {
						if( i >= m_arrClients.Count)
							break;
                        ConnectionState state = (ConnectionState)m_arrClients[i];
                        ClientData client = (ClientData)state.Tag;

                        if (!state.Connected || client.PingCount >= 3)
                        {
                            state.EndConnection();
                            m_arrClients.RemoveAt(i);

							NetworkServer.Instance.RemoveClient(state);
                            client.TempData = null;                           
                        }
                        else
                        {
                            if (client.Type == ClientData.ClientType.UNKNOWN)
                            {
                                if (Send(data2, 0, data2.Length, state, true))
                                    client.PingCount++;
                            }
                            else if (Send(data, 0, data.Length, state, true))
                                client.PingCount++;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public void ReleaseThread()
        {
            m_isAliveThread = false;
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
			Debug.WriteLine(value.ToString());
			return base.Add(value);
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

		public override bool Write(object str, bool writeTime = true)
		{
			if (logger != null)
				logger.DebugFormat("{0}", str);

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
