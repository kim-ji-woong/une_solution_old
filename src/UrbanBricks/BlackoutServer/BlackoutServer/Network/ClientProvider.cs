using BlackoutServer.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpLib2;

namespace BlackoutServer.Network
{
    public class ClientProvider : ClientServiceProvider
    {
        private NetworkModbusManager m_networkModbusManager = null;
        private NetworkWebManager m_networkManager = null;

        private Thread m_CheckThread = null;

        private bool m_runThread = false;
        public bool RunThread
        {
            get { return m_runThread; }
            set { m_runThread = value; }
        }

        private int m_nPingCount = 0;
        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        private byte[] mDataBuff = new byte[2048];

        private bool m_bReciverData = false;
        private byte[] m_reciveBuffer = null;
        private byte[] m_tempBuffer = null;

        public ClientProvider(NetworkModbusManager networkModbusManager, NetworkWebManager webworkManager)
        {
            m_networkModbusManager = networkModbusManager;
            m_networkManager = webworkManager;

            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);

            m_CheckThread = new Thread(CheckThread);
            m_CheckThread.Start();
        }

        private void CheckThread()
        {
            m_runThread = true;
            int nCount = 0;

            while (m_runThread)
            {
                if (this.IsConnected)
                {
                    CheckValue();

                    if (!m_runThread)
                        break;
                }

                Thread.Sleep(1000);

                nCount++;

                if (nCount == 1000)
                {
                    try
                    {
                        nCount = 0;
                        if (!m_runThread)
                            GC.Collect();
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        private void CheckValue()
        {
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            byte nHeader = 0;
            byte[] data = MsgHelper.MakeMsgReadCoils(0X01, out nHeader);

            SendBytes(data);

            int nCount = 0;
            while (!m_bReciverData)
            {
                nCount++;
                if (nCount == 10)
                    break;

                Thread.Sleep(200);
            }

            if (m_bReciverData)
            {
                byte[] smReadBuffer = m_reciveBuffer;
                m_bReciverData = false;
                if (smReadBuffer != null)
                {
                    SetLevelValue(nHeader, smReadBuffer);
                }
                ClearBuffer();
                //m_nFaultCount = 0;
            }
            else
            {
                //m_nFaultCount++;

                //if (m_nFaultCount == 2)
                //{
                //    m_nFaultCount = 0;
                //    if (m_Units != null)
                //    {
                //        for (int i = 0; i < m_Units.Length; i++)
                //        {
                //            m_Units[i].Value = -999;
                //        }
                //    }

                //    m_bOffline = true;
                //}
            }
        }

        public override void OnReceiveData()
        {
            byte[] data = this.ReceivedData;
            if (data == null)
            {
                m_bReciverData = false;
                return;
            }

            int nReadCount = data.Length;
            if (nReadCount == 0)
            {
                m_bReciverData = false;
                return;
            }

            if (m_tempBuffer == null && IsCompleteData(data))
            {
                if (m_tempBuffer == null)
                {
                    m_reciveBuffer = data;
                }
                else
                {
                    m_reciveBuffer = new byte[m_tempBuffer.Length + data.Length];
                    Array.Copy(m_tempBuffer, 0, m_reciveBuffer, 0, m_tempBuffer.Length);
                    Array.Copy(data, 0, m_reciveBuffer, m_tempBuffer.Length, data.Length);
                }

                RecivedLog(m_reciveBuffer, m_reciveBuffer.Length);
                m_tempBuffer = null;

                m_bReciverData = true;
            }
            else
            {
                if (m_tempBuffer == null)
                {
                    m_tempBuffer = data;
                }
                else
                {
                    int nLength = m_tempBuffer.Length;
                    byte[] temp = m_tempBuffer;
                    m_tempBuffer = new byte[nLength + data.Length];
                    Array.Copy(temp, m_tempBuffer, nLength);
                    Array.Copy(data, 0, m_tempBuffer, nLength, data.Length);

                    if (IsCompleteData(m_tempBuffer))
                    {
                        m_reciveBuffer = m_tempBuffer;
                        RecivedLog(m_reciveBuffer, m_reciveBuffer.Length);
                        m_tempBuffer = null;
                    }
                }
            }
        }

        public void ClearBuffer()
        {
            m_reciveBuffer = null;
            m_tempBuffer = null;
        }

        private bool IsCompleteData(byte[] data)
        {
            if (data == null)
                return false;

            //if (data.Length < 3)
            //    return false;

            //int nLength = data[2] + 5;

            //if (data.Length < nLength)
            //    return false;

            return true;
        }

        private void SetLevelValue(byte nHeader, byte[] bStatus)
        {
            try
            {
                if (bStatus == null)
                    return;

                if (bStatus.Length < 10)
                    return;

                byte status = bStatus[9];

                // 1. 1000 1101 8bit 일 때
                BitArray b = new BitArray(new byte[] { bStatus[9] });
                int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();
                if (bits.Length < 8)
                    return;
                //BitArray bitArr = new BitArray(BitConverter.GetBytes(bStatus[8])); //bool[]

                int data1 = bits[0]; //00001 : 호텔 정전 신호
                int data2 = bits[1]; //00002 : 오피스A 정전 신호
                int data3 = bits[2]; //00003 : 오피스B 정전 신호
                int data4 = bits[3]; //00004 : 판매시설B 정전 신호
                int data5 = bits[4]; //00005 : 판매시설D 정전 신호
                int data6 = bits[5]; //00006 : SI 테스트 정전 신호

                foreach (Sensor sensor in DataManager.Sensors)
                {
                    if (sensor.SensorName.Contains("호텔"))
                    {
                        if (sensor.Data != data1)
                            m_networkManager.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, 17, data1);
                    }
                    else if (sensor.SensorName.Contains("UTower"))
                    {
                        if (sensor.Data != data2)
                            m_networkManager.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, 17, data2);
                    }
                    else if (sensor.SensorName.Contains("TTower"))
                    {
                        if (sensor.Data != data3)
                            m_networkManager.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, 17, data3);
                    }
                    else if (sensor.SensorName.Contains("백화점"))
                    {
                        int retailData = 0;
                        if (data4 == 1 || data5 == 1)
                            retailData = 1;

                        if (sensor.Data != retailData)
                            m_networkManager.SendSensorData(sensor.SensorZoneID, sensor.SensorTagInfoID, 17, retailData);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("[ERROR] " + ex.Message);
                if (bStatus == null)
                {
                    Logger.Instance.Write("[ERROR] bStatus is null");                    
                }
                else
                {
                    string tmp = GetTEXT(bStatus, bStatus.Length);
                    Logger.Instance.Write("[ERROR] " + tmp);
                }
            }
        }

        public override void OnDropConnection()
        {
            
        }

        public void SendBytes(byte[] CmdBuff)
        {
            SendLog(CmdBuff, CmdBuff.Length);
            try
            {                
                if (!this.IsClientDisposed && this.IsConnected)
                {
                    Array.Copy(CmdBuff, mDataBuff, CmdBuff.Length);

                    this.LengthAdd = false;
                    this.Send(mDataBuff, 0, CmdBuff.Length);

                    Thread.Sleep(50);
                }
            }
            catch (Exception)
            {
            }
        }

        private void WriteLog(string strLog)
        {
            Logger.Instance.Write(strLog);
        }

        private void RecivedLog(Byte[] bufRecive, int ret)
        {
            string tmp = GetTEXT(bufRecive, ret);
            WriteLog("[RECIVED TXT] : " + tmp);
        }

        private void SendLog(Byte[] bufRecive, int ret)
        {
            string tmp = GetTEXT(bufRecive, ret);
            WriteLog("[SEND TXT] : " + tmp);
        }

        private string GetTEXT(Byte[] bufRecive, int ret)
        {
            string tmp = "";
            for (int j = 0; j < ret; j++)
            {
                byte b = bufRecive[j];
                if (tmp.Length == 0)
                    tmp = string.Format("{0:X2}", (int)b);
                else
                    tmp += string.Format(" {0:X2}", (int)b);
            }

            return tmp;
        }
    }
}
