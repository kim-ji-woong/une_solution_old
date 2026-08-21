using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Collections;
using System.Diagnostics;

namespace PSMSensorServer
{
    public abstract class ClientData
    {
        public enum ClientType
        {
            ALL = 0,
            GIMENS = 1,
            PSMTester = 2,
            PSMSensor = 3,
            UNKNOWN
        };

        private int m_nPingCount = 0;
        private ClientType m_type = ClientType.UNKNOWN;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        protected PSMServiceProvider m_provider = null;
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

        public PSMServiceProvider ServiceProvider
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

        private bool CheckPool(ConnectionState state, byte[] data)
        {
            if(data.Length == 3)
            {
                if( data[0] == 0x02)
                {
                    if(data[1] == 0x06)
                    {
                        if (data[2] == 0x03)
                            return true;
                    }
                }
            }
            return false;
        }

        public virtual void Close()
        {

        }

        public virtual bool OnReceiveData(ConnectionState state, byte[] bytes, bool checkValidation = true)
        {
            ArrayList arrDatas;
            byte[] curReceivedData = null;
            
            
            WriteByteArray(bytes);
            
            int nHeader = GetHeader(state, bytes, out curReceivedData, checkValidation, out arrDatas);
            if( nHeader == 100)
            {
                m_provider.SendACK(state);
                return true;
            }

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



        //02 80 BE 80 80 95 80 32 30 31 35 2D 30 39 2D 32 32 20 31 30 3A 31 36 3A 32 38 2C 30 31 2D 30 30 2D 32 2D 30 39 32 2C C5 CD BA F3 32 2D 2C C1 DF 32 46 2D 30 35 2D 30 31 2D B9 DF BD C5 B1 E2 03
        private int mMinLength = 38;
        protected bool CheckValidation(ConnectionState state)
        {

            // 읽어가지 않은 누적데이터가 RecivedData에 존재함
            byte[] bytes = this.ReceivedData;
            WriteByteArray(bytes);
            // 전체길이는 항상 40byte이상
            int length = bytes.Length;
            if (length < mMinLength)
            {
                if (bytes.Length == 3)
                    return true;
                return false;
            }
             

            // 첫 바이트는 stx
            int stx = bytes[0];

            int nLength1 = (bytes[1] - (byte)0x80);
            int nLength2 = (bytes[2] - (byte)0x80);
            int nDataCount = nLength1 * 128 + nLength2 + 2;

            int nTotalData = nDataCount;
            if (state.LengthAdd == true)
                nTotalData += 4;

            if (length < nDataCount)
                return false;

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
            //this.m_provider.WriteLineLog("ReadByte : " + nBytesCount);
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

                if(state.LengthAdd == true)
                {
                    int nDataLength = this.ReceivedData.Length - 4;
                    curReceivedData = new byte[nDataLength];
                    System.Buffer.BlockCopy(this.ReceivedData, 4, curReceivedData, 0, nDataLength);
                }
                else
                {
                    int nDataLength = this.ReceivedData.Length;

                    //m_provider.WriteLineLog("DataLEngth : " + nDataLength);
                    curReceivedData = new byte[nDataLength];
                    System.Buffer.BlockCopy(this.ReceivedData, 0, curReceivedData, 0, nDataLength);
                }

                //m_provider.WriteLineLog("DataLEngth : " + curReceivedData.Length);

                short nHeader = 100;
                if(curReceivedData.Length > 3)
                {
                    arrDatas = PSMServiceProvider.ReadBytes(curReceivedData, out nHeader);
                }
               
                //if (m_provider != null)
                //    m_provider.RecvLog(curReceivedData, state);

                return nHeader;
            }
            return 0;
        }
    }
}
