using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Collections;

namespace SDMSServer
{
    public abstract class ClientData
    {
        public enum ClientType { ALL = 0, SDMS_CLIENT, SOP_SIMULATOR, SENSOR_SIMULATOR, SOP_MONITOR, SOP_RESOTRE, INTEGRATE_MANAGER, SDMS_CLIENT_SECOND, UNKNOWN };

        private int m_nPingCount = 0;
        private ClientType m_type = ClientType.UNKNOWN;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        protected ServiceProvider m_provider = null;
        protected ConnectionState m_state = null;

        public byte[] ReceivedData
        {
            get { return m_arrReceived; }
            set { m_arrReceived = value; }
        }

        public byte[] TempData
        {
            get { return m_arrTemp; }
            set { m_arrTemp = value; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public SDMSServer.ServiceProvider ServiceProvider
        {
            get { return m_provider; }
            set { m_provider = value; }
        }

        public TcpLib2.ConnectionState ConnectionState
        {
            get { return m_state; }
            set { m_state = value; }
        }

        // bytes는 length byte가 제거되었음
        protected abstract bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas);

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected virtual bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }

        protected bool ProcessFirstConnection(ClientData data, ConnectionState state)
        {
            return data.ProcessFirstConnection(state);
        }

        public virtual bool OnReceiveData(ConnectionState state, byte[] bytes, bool checkValidation = true)
        {
            ArrayList arrDatas;
            byte[] curReceivedData = null;
            int nHeader = GetHeader(state, bytes, out curReceivedData, checkValidation, out arrDatas);

            if (nHeader < 0)
                return false;
            else if (nHeader == 0)
                return true;

            // I_AM_HERE는 ClientData에서 처리한다.
            if (nHeader == TCP_ID.I_AM_HERE)
            {
                //this.PingCount = 0;
                return true;
            }

            return OnReceive(state, curReceivedData, nHeader, arrDatas);
        }

        protected bool CheckValidation(ConnectionState state)
        {
            byte[] bytes = this.ReceivedData;

            int length = bytes.Length;
            if (length < 6)
                return false;

            //Debug.WriteLine("Data Length : " + length);
            int nDataCount = BitConverter.ToInt32(bytes, 0);

            int nTotalData = (nDataCount + 4);
            //Debug.WriteLine("TotalData : " + nDataCount);
            int nIndex = nTotalData;
            
            if (length > nIndex)
            {
                byte[] bytes1 = new byte[nIndex];
                byte[] bytes2 = new byte[length - nIndex];

                Array.Copy(bytes, bytes1, nIndex);
                Array.Copy(bytes, nIndex, bytes2, 0, length - nIndex);

                OnReceiveData(state, bytes1, false);

                if (!OnReceiveData(state, bytes2))
                    return false;

                this.ReceivedData = null;
                return false;
            }

            return true;
        }

        // Return 값 : 0보다 작으면 validation 실패
        //             0이면 읽을 데이터가 없음
        protected int GetHeader(TcpLib2.ConnectionState state, byte[] bytes, out byte[] curReceivedData, bool checkValidation, out ArrayList arrDatas)
        {
            arrDatas = null;
            this.ReceivedData = bytes;
            curReceivedData = null;

			if (bytes == null)
				return 0;

            if (this.TempData != null)
            {
                int nReceivedCount = this.ReceivedData.Length;
                int nTempCount = this.TempData.Length;

                byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                Array.Copy(this.TempData, arrBuffer, nTempCount);
                Array.Copy(this.ReceivedData, 0, arrBuffer, nTempCount, nReceivedCount);

                this.ReceivedData = arrBuffer;
                this.TempData = null;
            }

            int nBytesCount = this.ReceivedData.Count();

            if (nBytesCount > 0)
            {
                this.PingCount = 0;

                if (checkValidation)
                {
                    if (!CheckValidation(state))
                    {
                        this.TempData = this.ReceivedData;
                        return -1;
                    }
                }

                int nDataLength = this.ReceivedData.Length - 4;
                curReceivedData = new byte[nDataLength];
                System.Buffer.BlockCopy(this.ReceivedData, 4, curReceivedData, 0, nDataLength);

                //this.ReceivedData = curRecivedData;

                short nHeader;
                arrDatas = ServiceProvider.ReadBytes(curReceivedData, out nHeader);
                //int nHeader = BitConverter.ToInt16(curReceivedData, 0);

                if (m_provider != null)
                    m_provider.RecvLog(curReceivedData, state);

                return nHeader;
            }

            return 0;
        }
    }
}
