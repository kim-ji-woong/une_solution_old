using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using SDMS;
using System.Collections;

namespace PSMSensorServer
{
    public class SOPClientProvider : ClientServiceProvider
    {
        private NetworkClient m_mgr = null;
        private int m_nPingCount = 0;

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

        private static log4net.ILog logger = null;

        public SOPClientProvider(NetworkClient mgr)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_mgr = mgr;
			this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
			//this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
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

                    short nHeader;
                    ArrayList arrDatas = ReadBytes(ReceivedData, out nHeader);

                    if (nHeader == TCP_ID.ARE_YOU_THERE)
                    {
                        SendData(TCP_ID.I_AM_HERE);
                    }
                    else if (nHeader == TCP_ID.WHO_ARE_YOU)
                    {
                        SendWhoIam();
                    }
                    else if (nHeader == TCP_ID.EDIT_SENSOR_ZONE)
                    {
                        ProcessEditSensorZone(arrDatas);
                    }
                    else if( nHeader == TCP_ID.SERVER_COMMAND)
                    {
                        ProcessServerCommand(arrDatas);
                    }
                    
                }
            }

        RETURN:
            m_isReadingProcess = false;
        }
        public void ProcessServerCommand(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            byte nHeader = (byte)arrDatas[0];
            if (nHeader == ServerCommandType.REQUEST_PSM_SENSOR_ALARM)
            {
                int nSensorID = (int)arrDatas[1];
                ProcessRequestSensorAlarm(nSensorID);
            }
            else if(nHeader == ServerCommandType.REQUEST_PSM_SENSOR_RESET)
            {
                int nSensorID = (int)arrDatas[1];
                ProcessRequestSensorReset(nSensorID);

            }
            else if(nHeader == ServerCommandType.REQUEST_PSM_BUZZER)
            {
                int nSensorID = (int)arrDatas[1];
                int nOnOff = (int)arrDatas[2];
                ProcessRequestBuzzer(nSensorID, nOnOff);
            }
            else if(nHeader == ServerCommandType.REQUEST_PSM_TEST_ALARM)
            {
                int nSensorID = (int)arrDatas[1];
                ProcessRequestTestAlamr(nSensorID);
            }
        }

        private void ProcessRequestTestAlamr(int nSensorID)
        {
            PSMSensorManager.Instance.RequestTestAlarm(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 테스트값 입력", nSensorID);
            logger.Info(szMsg);
        }
        

        private void ProcessRequestBuzzer(int nSensorID, int nOnOff)
        {
            PSMSensorManager.Instance.BuzzerSet(nSensorID, nOnOff);

            if( nOnOff == 1)
            {
                string szMsg = string.Format("Request SOPServer : {0}번 센서 부저 켜기", nSensorID);
                logger.Info(szMsg);
            }
            else
            {
                string szMsg = string.Format("Request SOPServer : {0}번 센서 부저 정지", nSensorID);
                logger.Info(szMsg);
            }
            
        }


        private void ProcessRequestSensorReset(int nSensorID)
        {
            PSMSensorManager.Instance.RequestReset(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 신호 리셋", nSensorID);
            logger.Info(szMsg);
        }


        private void ProcessRequestSensorAlarm(int nSensorID)
        {
            PSMSensorManager.Instance.RequestAlarm(nSensorID);

            string szMsg = string.Format("Request SOPServer : {0}번 센서 신호 요청", nSensorID);
            logger.Info(szMsg);           
        }

        private void ProcessEditSensorZone(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount % 4 != 0)
                return;

            for (int i = 0; i < nDataCount; i += 4)
            {
                int nSensorZoneID = (int)arrDatas[i];
                int nOriginEquipZoneID = (int)arrDatas[i + 1];
                int nChangedEquipZoneID = (int)arrDatas[i + 2];
                int nZoneID = (int)arrDatas[i + 3];

                SensorZone sensorZone = PSMNetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    continue;

                EquipmentZone equipZoneOrigin = PSMNetworkServer.Instance.IOManager.GetEquipmentZone(nOriginEquipZoneID);
                EquipmentZone equipZoneChanged = PSMNetworkServer.Instance.IOManager.GetEquipmentZone(nChangedEquipZoneID);

                if (equipZoneOrigin != null)
                {
                    ArrayList arrSensorZones;

                    if (PSMNetworkServer.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneOrigin, out arrSensorZones))
                    {
                        arrSensorZones.Remove(sensorZone);
                    }
                }

                if (equipZoneChanged != null)
                {
                    ArrayList arrSensorZones;

                    if (!PSMNetworkServer.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneChanged, out arrSensorZones))
                    {
                        arrSensorZones = new ArrayList();
                        PSMNetworkServer.Instance.IOManager.D_EquipZoneSensor[equipZoneChanged] = arrSensorZones;
                    }

                    if (!arrSensorZones.Contains(sensorZone))
                        arrSensorZones.Add(sensorZone);
                }

                sensorZone.EquipZone = equipZoneChanged;
            }
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

        private void SendWhoIam()
        {
            byte[] bytes = new byte[15];

            byte[] dataBytes = MakeBytes((int)TCP_CLIENT.PSM_SENSOR_SERVER);

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
            m_mgr.MessageQueueReady();
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
        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        public new int Send(byte[] buffer, int offset,  int size)
        {
            return base.Send(buffer, offset, size);
        }

        public int Send_NoLengthByte(byte[] buffer, int offset,  int size)
        {
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
    }
}
