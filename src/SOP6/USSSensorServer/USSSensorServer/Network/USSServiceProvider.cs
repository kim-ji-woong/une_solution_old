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
using SOPWebClient;
using System.Collections;

namespace USSFireSensorServer.Network
{
    public class USSServiceProvider : TcpServiceProvider
    {
        private ConcurrentDictionary<ConnectionState, ConnectionState> m_dicClients = new ConcurrentDictionary<ConnectionState, ConnectionState>();
        private bool m_isAliveThread = false;
        private IUSSServiceOwner m_owner = null;
        private Logger m_logger = null;

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        public USSServiceProvider(IUSSServiceOwner owner, Logger logger)
        {
            m_owner = owner;
            InitLog(logger);

            Thread t = new Thread(new ThreadStart(PingThread));
            t.Start();
        }

        public void WriteLineLog(string str)
        {
            if (m_logger != null)
                m_logger.Write(str);
        }

        public void WriteLineLog(string str, Exception e)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
            string strLog = "프로그램 오류 : " + str + ", " + e.Message;
            strLog += "\r\nLine : " + trace.GetFrame(0).GetFileLineNumber().ToString();

            WriteLineLog(strLog);
        }

        private void InitLog(Logger logger)
        {
            m_logger = logger;
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
                WriteLineLog("Send", ex);
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

        public void SendEarthquakeSignal(int nIntensity, DateTime timeStamp)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(EarthquakeDataType.Intensity);
            arrDatas.Add(nIntensity);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.EARTH_QUAKE_DATA, arrDatas);

            if (bytes != null)
            {
                List<ConnectionState> states = m_dicClients.Values.ToList();
                int nLength = bytes.Length;

                foreach (ConnectionState state in states)
                {
                    ClientData client = (ClientData)state.Tag;

                    if (client == null || client.HasEvent(EventType.Earthquake) == false)
                        continue;

                    // 이전값과 같으면 다시 보내지 않는다.
                    //if (client.PrevIntensity == nIntensity)
                    //    continue;

                    if (Send(bytes, 0, nLength, state))
                        client.PrevIntensity = nIntensity;
                }
            }
        }

        public void SendWindSignal(int nSensorID, float fWindSpeed, DateTime timeStamp)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensorID);
            arrDatas.Add(fWindSpeed);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.WIND_SENSOR_DATA, arrDatas);

            if (bytes != null)
            {
                List<ConnectionState> states = m_dicClients.Values.ToList();
                int nLength = bytes.Length;

                foreach (ConnectionState state in states)
                {
                    ClientData client = (ClientData)state.Tag;

                    if (client == null || client.HasEvent(EventType.Wind) == false)
                        continue;

                    // 이전값과 같으면 다시 보내지 않는다.
                    //if (client.GetPrevWindSpeed(nSensorID) == fWindSpeed)
                    //    continue;

                    if (Send(bytes, 0, nLength, state))
                        client.SetPrevWindSpeed(nSensorID, fWindSpeed);
                }
            }
        }

        public void SetClientInfo(ConnectionState state, List<byte> eventTypes)
        {
            if (m_owner != null)
                m_owner.SetClientInfo(state, eventTypes);
        }
    }

    public interface IUSSServiceOwner
    {
        void OnAccept(ConnectionState state);
        void OnDropConnection(ConnectionState state);
        void OnEarthquakeSignal(int nIntensity, int nSensorZoneID, DateTime timeStamp);
        void OnStrongWindSignal(float fWindSpeed, int nSensorZoneID, DateTime timeStamp);
        void SetClientInfo(ConnectionState state, List<byte> eventTypes);
    }
}
