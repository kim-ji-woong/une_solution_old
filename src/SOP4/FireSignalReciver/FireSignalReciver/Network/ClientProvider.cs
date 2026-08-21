using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Net.Sockets;
using System.Diagnostics;

namespace FireSignalReciver
{
    public class ClientProvider : ClientServiceProvider
    {
        protected NetworkManager m_mgr = null;
        protected int m_nPingCount = 0;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        protected bool m_isReadingProcess = false;

        
        protected bool m_exceptPingLog = true;

        protected bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        protected int m_nType = TCP_CLIENT.SENSOR_MONITOR2;
        public int ClientType
        {
            get { return m_nType; }
            set { m_nType = value; }
        }

        protected byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        protected byte[] m_arrTemp = null;

        public byte[] ReceivedBuffer
        {
            get { return m_arrReceived; }
            set { m_arrReceived = value; }
        }

        public byte[] TempBuffer
        {
            get { return m_arrTemp; }
            set { m_arrTemp = value; }
        }

        public ClientProvider(NetworkManager mgr)
        {
            m_mgr = mgr;
			this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
			//this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            InitLog();
        }

        protected virtual bool OnReceive(byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            return true;
        }

        public override void OnReceiveData()
        {
            m_isReadingProcess = true;
            
            byte[] tempData = new byte[ReceivedData.Length];
            System.Buffer.BlockCopy(ReceivedData, 0, tempData, 0, tempData.Length);
            bool bResult = OnReceiveData(tempData);
            
            PingCount = 0;

            m_isReadingProcess = false;
        }

        //public override void OnReceiveData()
        //{
        //    if (ReceivedData != null)
        //    {
        //        m_isReadingProcess = true;

        //        int nBytesCount = ReceivedData.Count();

        //        if (nBytesCount > 0)
        //        {
        //            m_nPingCount = 0;

        //            if (!CheckValidation(ReceivedData))
        //                goto RETURN;

        //            if (ReceivedData[0] == TCP_ID.ARE_YOU_THERE)
        //            {
        //                SendData(TCP_ID.I_AM_HERE);
        //            }
        //            else if (ReceivedData[0] == TCP_ID.WHO_ARE_YOU)
        //            {
        //                SendWhoIam();
        //            }
        //        }
        //    }

        //RETURN:
        //    m_isReadingProcess = false;
        //}

        protected void SendWhoIam()
        {
            byte[] bytes = new byte[15];
            byte[] dataBytes = MakeBytes(m_nType);
            
            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.WHO_I_AM);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

            m_mgr.Send(bytes, this);            
        }       

        // header 1 Byte로만 이루어진 데이터
		public void SendData(short header)
		{
			byte[] bytes = new byte[6];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(0);

			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			if (this.Client.Client.Connected == true)
				m_mgr.Send(bytes, this);
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
            //byte[] dataBytes = new byte[data.Length * sizeof(char)];


            //System.Buffer.BlockCopy(data.ToCharArray(), 0, dataBytes, 0, dataBytes.Length);
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

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        private bool m_bUseSendLog = true;

        public new int Send(byte[] buffer, int offset,  int size)
        {
           // if (Client.Client.Connected == false)
           //     return -1;

            int nResult = -1;
            try
            {
                nResult = base.Send(buffer, offset, size);
                if (nResult > 0 && m_bUseSendLog == true)
                {
                    if (buffer[offset] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                    {
                        StringBuilder sb = new StringBuilder();
                      
                        string szRemote = this.Client.Client.RemoteEndPoint.ToString();

                        sb.AppendFormat("SendMessage : Header({0}), Length({1}) to {2}", (int)buffer[offset], size, szRemote);

                        bool bFirst = true;

                        foreach (byte b in buffer)
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
                m_nPingCount = 0;
            }
            catch(Exception exx)
            {
                if (m_bUseSendLog == true)
                    WriteLineLog("Write Send log", exx);
            }
            return nResult;
        }   

        public int Send_NoLengthByte(byte[] buffer, int offset,  int size)
        {
            //if (Client.Client.Connected == false)
            //    return -1;

            if (Client != null)
            {
                SocketError nErrCode = SocketError.Success;
                int nSendSize = 0;

                nSendSize = Client.Client.Send(buffer, 0, size, SocketFlags.None, out nErrCode);

                if (nErrCode == SocketError.Success)
                    return nSendSize;
            }

            return -1;
        }


        protected void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

        protected void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        protected void WriteLineLog(object str, Exception e)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str, e);
        }

        protected void InitLog()
        {
            if (ConnectionLogEx.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

        protected void RecvLog(byte[] bytes)
        {
            if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                string strClient = "(" + this.Client.Client.RemoteEndPoint.ToString() + ")";

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


        protected void WriteByteArray(byte[] bytes)
        {
            Debug.Write("{");
            for (int i = 0; i < bytes.Length; i++)
            {
                Debug.Write(string.Format("{0:X}", bytes[i]));
                Debug.Write(" ");
            }
            Debug.WriteLine("}");
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

        protected virtual bool ProcessIAmHere(ArrayList arrDatas)
        {
            PingCount = 0;
            SendData(TCP_ID.I_AM_HERE);
            return true;
        }

        public virtual bool OnReceiveData(byte[] bytes, bool checkValidation = true)
        {
            ArrayList arrDatas;
            byte[] curReceivedData = null;
            int nHeader = GetHeader(bytes, out curReceivedData, checkValidation, out arrDatas);

            if (nHeader < 0)
                return false;
            else if (nHeader == 0)
                return true;

            // I_AM_HERE는 ClientData에서 처리한다.
            if (nHeader == TCP_ID.ARE_YOU_THERE)
            {
                return ProcessIAmHere(arrDatas);
            }
            else if (nHeader == TCP_ID.WHO_ARE_YOU)
            {
                SendWhoIam();
                return true;
            }

            bool bResult = OnReceive(curReceivedData, nHeader, arrDatas);
            return bResult;
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 6)
                return false;

            int nChunkCount = (int)bytes[1];
            int nIndex = 6;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (length < nIndex + 5)
                    return false;

                int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                if (length < nIndex + 5 + nDataLength)
                    return false;

                nIndex += 5 + nDataLength;
            }

            return true;
        }

        // Return 값 : 0보다 작으면 validation 실패
        //             0이면 읽을 데이터가 없음
        protected int GetHeader(byte[] bytes, out byte[] curReceivedData, bool checkValidation, out ArrayList arrDatas)
        {
            arrDatas = null;
            this.ReceivedBuffer = bytes;
            curReceivedData = null;

            if (bytes == null)
                return 0;

            if (this.TempBuffer != null)
            {
                int nReceivedCount = this.ReceivedBuffer.Length;
                int nTempCount = this.TempBuffer.Length;

                byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                Array.Copy(this.TempBuffer, arrBuffer, nTempCount);
                Array.Copy(this.ReceivedBuffer, 0, arrBuffer, nTempCount, nReceivedCount);

                this.ReceivedBuffer = arrBuffer;
                this.TempBuffer = null;
            }

            int nBytesCount = bytes.Length;

            if (nBytesCount > 0)
            {
                this.PingCount = 0;

                if (checkValidation)
                {
                    if (!CheckValidation(ReceivedBuffer))
                    {
                        this.TempBuffer = this.ReceivedBuffer;
                        return -1;
                    }
                }
                short nHeader = 0;

                curReceivedData = new byte[nBytesCount];
                System.Buffer.BlockCopy(this.ReceivedBuffer, 0, curReceivedData, 0, nBytesCount);
                arrDatas = ReadBytes(curReceivedData, out nHeader);
                
                RecvLog(curReceivedData);

                return nHeader;
            }

            return 0;
        }

        //public void RecvLog(byte[] bytes)
        //{
        //    if (!ConnectionLogEx.Instance.IsOpened)
        //        return;

        //    if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
        //    {
        //        string strLog = string.Format("RecvMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
        //        string strBytes = "";

        //        foreach (byte b in bytes)
        //        {
        //            if (strBytes.Length == 0)
        //                strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
        //            else
        //                strBytes += string.Format(" {0:X2}", (int)b);
        //        }
        //        WriteLineLog(strLog + strBytes);
        //    }
        //}

        //public void SensorRecvLog(byte[] bytes)
        //{
        //    if (!ConnectionLogEx.Instance.IsOpened)
        //        return;

        //    if (bytes[0] != SERIAL_ID.POLL || !m_exceptPingLog)
        //    {
        //        string strLog = string.Format("RecvSensorMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
        //        string strBytes = "";

        //        foreach (byte b in bytes)
        //        {
        //            if (strBytes.Length == 0)
        //                strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
        //            else
        //                strBytes += string.Format(" {0:X2}", (int)b);
        //        }

        //        WriteLineLog(strLog + strBytes);
        //    }
        //}

        //private int WriteSendLog(int nResult, byte[] bytes, int nOffset)
        //{
        //    if (nResult > 0)
        //    {
        //        if (!ConnectionLogEx.Instance.IsOpened)
        //            return nResult;

        //        if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
        //        {
        //            string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[nOffset], (int)bytes.Length);
        //            string strBytes = "";

        //            for (int i=nOffset;i<bytes.Length;i++)
        //            {
        //                byte b = bytes[i];

        //                if (strBytes.Length == 0)
        //                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
        //                else
        //                    strBytes += string.Format(" {0:X2}", (int)b);
        //            }
        //            WriteLineLog(strLog + strBytes);
        //        }
        //        PingCount = 0;
        //    }
        //    return nResult;
        //}
    }
}
