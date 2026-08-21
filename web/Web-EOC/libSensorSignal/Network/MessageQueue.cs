using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SensorTester
{
    public class MessageQueue
    {
        private ArrayList m_arrDatas = new ArrayList();
        private bool m_ableToSend = false;

        public bool AbleToSend
        {
            get { return m_ableToSend; }
            set { m_ableToSend = value; }
        }

        public void Add(QueueData data)
        {
            lock (this)
            {
                int nDataCount = m_arrDatas.Count;

                // 같은 Type의 데이터가 이미 존재하면 기존의 데이터를 없애고 새로운 데이터를 추가한다.
                for (int i = 0; i < nDataCount; i++)
                {
                    QueueData _data = (QueueData)m_arrDatas[i];

                    if (_data.IsSameType(data))
                    {
                        m_arrDatas.RemoveAt(i);
                        break;
                    }
                }

                /*if (data.GetType() == typeof(QueueData_AllReceiverState))
                    m_arrDatas.Insert(0, data);
                else*/
                    m_arrDatas.Add(data);
            }
        }

        public void Send(NetworkClient mgr)
        {
            lock (this)
            {
                if (!AbleToSend)
                    return;

                int nBytesLength = GetTotalBytesLength();

                if (nBytesLength <= 0)
                    return;

                byte[] bytes = new byte[nBytesLength];

                int nIndex = 0;
                int nDataCount = m_arrDatas.Count;

                //for (int i = nDataCount - 1; i >= 0; i--)
                for (int i = 0; i < nDataCount;i++ )
                {
                    QueueData data = (QueueData)m_arrDatas[i];

                    //if (i < nDataCount - 1)
                    {
                        byte[] bytesLength = BitConverter.GetBytes(data.Bytes.Length);
                        System.Buffer.BlockCopy(bytesLength, 0, bytes, nIndex, 4);
                        nIndex += 4;
                    }

                    System.Buffer.BlockCopy(data.Bytes, 0, bytes, nIndex, data.Bytes.Length);
                    nIndex += data.Bytes.Length;
                }

                mgr.Send_NoLengthByte(bytes, mgr.ClientProvider);
                m_arrDatas.Clear();
            }
        }

        private int GetTotalBytesLength()
        {
            int nBytesLength = 0;

            foreach (QueueData data in m_arrDatas)
            {
                if (data.Bytes == null)
                    continue;

                nBytesLength += data.Bytes.Length + 4;
            }

            /*// 첫번째 데이터는 TcpLib2에서 길이 바이트를 추가하므로 따로 4바이트를 붙이지 않는다.
            nBytesLength -= 4;*/
            return nBytesLength;
        }
    }

    public abstract class QueueData
    {
        private byte[] m_bytes = null;

        public byte[] Bytes
        {
            get { return m_bytes; }
            set { m_bytes = value; }
        }

        public abstract bool IsSameType(QueueData data);
    }

    public class QueueData_SensorData : QueueData
    {
        private int m_nEquipZoneID = -1;

        //1(화재탐지 센서), 2(소화 센서), 3(압력 센서), 4(발신기)
        private int m_nSensorType = -1;

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        //1(화재탐지 센서), 2(소화 센서), 3(압력 센서), 4(발신기)
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public QueueData_SensorData()
        {
        }

        public QueueData_SensorData(byte[] bytes, int nEquipZoneID, int nSensorType)
        {
            Bytes = bytes;
            m_nEquipZoneID = nEquipZoneID;
            m_nSensorType = nSensorType;
        }

        public override bool IsSameType(QueueData data)
        {
            if (data.GetType() != this.GetType())
                return false;

            QueueData_SensorData _data = (QueueData_SensorData)data;

            return _data.m_nEquipZoneID == this.m_nEquipZoneID &&
                _data.m_nSensorType == this.m_nSensorType;
        }
    }

    public class QueueData_AllReceiverState : QueueData
    {
        public QueueData_AllReceiverState()
        {
        }

        public QueueData_AllReceiverState(byte[] bytes)
        {
            Bytes = bytes;
        }

        public override bool IsSameType(QueueData data)
        {
            return data.GetType() == this.GetType();
        }
    }

    public class QueueData_ReceiverConnection : QueueData
    {
        private int m_nReceiver = -1;
        
        public int Receiver
        {
            get { return m_nReceiver; }
            set { m_nReceiver = value; }
        }

        public QueueData_ReceiverConnection()
        {
        }

        public QueueData_ReceiverConnection(byte[] bytes, int nReceiver)
        {
            Bytes = bytes;
            m_nReceiver = nReceiver;
        }

        public override bool IsSameType(QueueData data)
        {
            if (data.GetType() != this.GetType())
                return false;

            QueueData_ReceiverConnection _data = (QueueData_ReceiverConnection)data;
            return _data.m_nReceiver == this.m_nReceiver;
        }
    }
}
