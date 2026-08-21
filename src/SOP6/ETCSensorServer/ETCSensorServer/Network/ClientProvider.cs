using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections;
using libUSS;
using ETCSensorServer.Data;

namespace ETCSensorServer.Network
{
    public class ClientProvider : ClientServiceProvider
    {
        private ClientManager m_mgr = null;
        private int m_nPingCount = 0;
        private SensorManager m_sensorMgr = null;
        private NetworkWebManager m_webMgr = null;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;
        
        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProvider(ClientManager mgr, SensorManager sensorManager, NetworkWebManager webManager)
        {
            m_sensorMgr = sensorManager;
            m_mgr = mgr;
            m_webMgr = webManager;

            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            this.LengthAdd = false;
        }

        public override void OnReceiveData()
        {
            if (ReceivedData != null)
            {
                m_isReadingProcess = true;

                int nBytesCount = ReceivedData.Count();

                if (nBytesCount > 0)
                {
                    m_nPingCount = 0;

                    if (!CheckValidation(ReceivedData))
                        goto RETURN;

                    short header;
                    ArrayList arrDatas = BinaryHelper.ReadBytes(ReceivedData, out header);
                    OnReceive(header, arrDatas);
                }
            }

            RETURN:
            m_isReadingProcess = false;
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 4)
                return false;

            int nChunkCount = (int)bytes[1];
            int nIndex = 4;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (length < nIndex + 3)
                    return false;

                int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                if (length < nIndex + 3 + nDataLength)
                    return false;

                nIndex += 3 + nDataLength;
            }

            return true;
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        private void OnReceive(short header, ArrayList arrDatas)
        {
            if (header == Header.ARE_YOU_THERE)
                SendData(Header.I_AM_HERE);
            else if (header == Header.POWER_OFF_DATA)
                ProcessPowerOffData(arrDatas);
            else if (header == Header.WIND_SENSOR_DATA)
                ProcessWindData(arrDatas);
        }

        private void ProcessPowerOffData(ArrayList arrDatas)
        {
            if (m_sensorMgr == null || m_webMgr == null)
                return;

            if (arrDatas.Count >= 4 && arrDatas[0] is byte && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is DateTime)
            {
                byte off = (byte)arrDatas[0];
                int nBuildingID = (int)arrDatas[1];
                int nSpaceID = (int)arrDatas[2];
                DateTime timeStamp = (DateTime)arrDatas[3];

                SensorTagInfo sensor = m_sensorMgr.GetPowerOffSensor(nBuildingID);

                if (sensor == null)
                    return;

                m_webMgr.SendSensorData(sensor, (int)off);
            }
        }

        private void ProcessWindData(ArrayList arrDatas)
        {
            /*if (m_sensorMgr == null || m_webMgr == null)
                return;

            if (arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is DateTime)
            {
                int nSensorID = (int)arrDatas[0];
                float fSpeed = (float)arrDatas[1];
                DateTime timeStamp = (DateTime)arrDatas[2];
                int nBuildingID = nSensorID;

                SensorTagInfo sensor = m_sensorMgr.GetWindSensor(nBuildingID);

                if (sensor == null)
                    return;

                m_webMgr.SendSensorData(sensor, (int)off);
            }*/
        }

        public int Send(byte[] buffer, int offset, int size)
        {
            return base.Send(buffer, offset, size);
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
            byte[] bytes = new byte[4];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes((short)0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];

            if (this.Client.Client.Connected == true)
                m_mgr.Send(bytes);
        }
    }

    public interface IServiceOwner
    {
        void OnConnect();
        void OnDropConnection();
    }
}
