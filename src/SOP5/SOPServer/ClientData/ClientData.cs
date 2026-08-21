using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Collections;
using System.Diagnostics;

namespace SDMSServer
{
    public abstract class ClientData
    {
        /*public enum ClientType
        {
            ALL = 0,
            SDMS_CLIENT = 1,
            SOP_SIMULATOR = 2,
            SENSOR_SIMULATOR = 3,
            SOP_MONITOR = 4,
            SOP_RESOTRE = 5,
            INTEGRATE_MANAGER = 6, 
            SDMS_CLIENT_SECOND = 7, 
            SERVER_MONITOR = 8, 
            SOP_MONITOR2 = 9, 
            SERVER_COMMANDER = 10,
            TRAINING_SIMULATOR = 11,
            SOP_WEATHER = 12,
            PSM_SENSOR = 13,
            EARTHQUAKE_SENSOR_SERVER = 15,

            
            SVMS_EVENT_RECIVER = 16,
            ACCESS_EVENT_RECIVER = 17,     
            SAINTOP_EVENT_RECIVER = 18,
            ASIN_EVENT_RECIVER = 19,
            S1_SENSOR_SERVER = 20,
            UNKNOWN
        };*/

        private int m_nPingCount = 0;
        private byte m_clientType = TCP_CLIENT.UNKNOWN;
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

        public byte ClientType
        {
            get { return m_clientType; }
            set { m_clientType = value; }
        }

        protected string m_szServerType = "[Access]";
        public string ServerType
        {
            get { return m_szServerType; }
            set { m_szServerType = value; }
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

        /*private bool m_isProcessingReceive = false;
        // OnReceive가 진행중인가?
        public bool IsProcessingReceive
        {
            get { return m_isProcessingReceive; }
            set { m_isProcessingReceive = value; }
        }*/

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
                return ProcessIAmHere(arrDatas);
            }          
            
            bool bResult = OnReceive(state, curReceivedData, nHeader, arrDatas);           
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

        protected virtual bool ProcessIAmHere(ArrayList arrDatas)
        {
            return true;
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

                //Debug.WriteLine("Origin Bytes Length : " + bytes.Length.ToString() + ", bytes1 length " + bytes1.Length.ToString() + ", bytes2 length : " + bytes2.Length.ToString());
                //Debug.Write("bytes: ");
                //WriteByteArray(bytes);
                //Debug.Write("bytes1: ");
                //WriteByteArray(bytes1);
                //Debug.Write("bytes2: ");
                //WriteByteArray(bytes2);

                OnReceiveData(state, bytes1, false);

                if (!OnReceiveData(state, bytes2))
                    return false;

                this.ReceivedData = null;
                return false;
            }
            else if (length < nIndex)
                return false;

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
                arrDatas = TcpHelper.ReadBytes(curReceivedData, out nHeader);
                //int nHeader = BitConverter.ToInt16(curReceivedData, 0);

                if (m_provider != null)
                    m_provider.RecvLog(curReceivedData, state);

                return nHeader;
            }

            return 0;
        }

        public virtual void CloseClient()
        {

        }
    }

    
}
