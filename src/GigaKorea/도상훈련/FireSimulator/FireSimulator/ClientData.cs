using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Collections;

namespace FireSimulator
{
    class ClientData
    {
        private ServiceProvider m_provider = null;

        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        private IListener m_listener = null;

        public ClientData(ServiceProvider provider, IListener listener)
        {
            m_provider = provider;
            m_listener = listener;
        }

        public virtual bool OnReceiveData(ConnectionState state, byte[] bytes, bool checkValidation = true)
        {
            ArrayList arrDatas;
            byte[] curReceivedData = null;
            //short nHeader = GetHeader(state, bytes, out curReceivedData, checkValidation, out arrDatas);
            short nHeader = CheckHeader(bytes);

            if (nHeader == 0)
                return false;

            //if (nHeader == TCP_ID.ARE_YOU_THERE)
            //{
            //    return SendIAmHere(state);
            //}
            //else if (nHeader == TCP_ID.REPORT_FIRE ||
            //    //nHeader == TCP_ID.REPORT_EARTHQUAKE4 ||
            //    nHeader == TCP_ID.REPORT_EARTHQUAKE5 ||
            //    nHeader == TCP_ID.REPORT_SEQURITY ||
            //    nHeader == TCP_ID.REPORT_FINEDUST1 ||
            //    nHeader == TCP_ID.REPORT_FINEDUST2)
            //{
            //    ProcessAlarm(nHeader, arrDatas);
            //}
            //else if (nHeader == TCP_ID.CLEAR_FIRE ||
            //    //nHeader == TCP_ID.CLEAR_EARTHQUAKE4 ||
            //    nHeader == TCP_ID.CLEAR_EARTHQUAKE5 ||
            //    nHeader == TCP_ID.CLEAR_SEQURITY ||
            //    nHeader == TCP_ID.CLEAR_FINEDUST1 ||
            //    nHeader == TCP_ID.CLEAR_FINEDUST2)
            //{
            //    ClearAlarm(nHeader, arrDatas);
            //}

            //if (nHeader == TCP_ID.ARE_YOU_THERE)
            //{
            //    return SendIAmHere(state);
            //}

            if (nHeader == TCP_ID.SIMULATOR_OPEN)
            {
                ProcessOpen();
            }

            return true;
        }

        private void ProcessOpen()
        {
            FormMain.Instance.Visiable(true);
        }

        private short CheckHeader(byte[] arrHeader)
        {
            byte[] arrMsgCode = new byte[2];

            short nMsgCode = 0;

            Array.Copy(arrHeader, 0, arrMsgCode, 0, arrMsgCode.Length);

            nMsgCode = BitConverter.ToInt16(arrMsgCode, 0);

            return nMsgCode;
        }

        // Return 값 : 0보다 작으면 validation 실패
        //             0이면 읽을 데이터가 없음
        protected short GetHeader(TcpLib2.ConnectionState state, byte[] bytes, out byte[] curReceivedData, bool checkValidation, out ArrayList arrDatas)
        {
            arrDatas = null;
            m_arrReceived = bytes;
            curReceivedData = null;

            if (bytes == null)
                return 0;

            if (m_arrTemp != null)
            {
                int nReceivedCount = m_arrReceived.Length;
                int nTempCount = m_arrTemp.Length;

                byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                Array.Copy(m_arrTemp, arrBuffer, nTempCount);
                Array.Copy(m_arrReceived, 0, arrBuffer, nTempCount, nReceivedCount);

                m_arrReceived = arrBuffer;
                m_arrTemp = null;
            }

            int nBytesCount = m_arrReceived.Count();

            if (nBytesCount > 0)
            {
                if (checkValidation)
                {
                    if (!CheckValidation(state))
                    {
                        m_arrTemp = m_arrReceived;
                        return -1;
                    }
                }

                int nDataLength = m_arrReceived.Length - 4;
                curReceivedData = new byte[nDataLength];
                System.Buffer.BlockCopy(m_arrReceived, 4, curReceivedData, 0, nDataLength);

                //this.ReceivedData = curRecivedData;

                short nHeader;
                arrDatas = TcpHelper.ReadBytes(curReceivedData, out nHeader);
                //int nHeader = BitConverter.ToInt16(curReceivedData, 0);

                return nHeader;
            }

            return 0;
        }

        protected bool CheckValidation(ConnectionState state)
        {
            byte[] bytes = m_arrReceived;

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

                m_arrReceived = null;
                return false;
            }
            else if (length < nIndex)
                return false;

            return true;
        }

        private bool SendIAmHere(ConnectionState state)
        {
            byte[] bytes = new byte[6] { (byte)TCP_ID.I_AM_HERE, 0, 0, 0, 0, 0 };
            return m_provider.Send(bytes, state);
        }

        private void ProcessAlarm(int nHeader, ArrayList arrDatas)
        {
            if (m_listener != null)
                m_listener.ProcessAlarm(nHeader, arrDatas);
        }

        private void ClearAlarm(int nHeader, ArrayList arrDatas)
        {
            if (m_listener != null)
                m_listener.ClearAlarm(nHeader, arrDatas);
        }
    }
}
