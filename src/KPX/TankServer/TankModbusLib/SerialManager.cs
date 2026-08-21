using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace TankModbusLib
{    
    internal class SerialManager
    {
        private SerialPort sPort = new SerialPort();

        private string m_szPort = "COM15";
        public string Port
        {
            get { return m_szPort; }
            set
            {
                m_szPort = value;
                sPort.PortName = m_szPort;
            }
        }

        private int m_nSwitchNum = 1;
        public int SwitchNum
        {
            get { return m_nSwitchNum; }
            set { m_nSwitchNum = value; }
        }

        private bool bSendForamtAscii = true;
        public bool SendForamt
        {
            get { return bSendForamtAscii; }
            set { bSendForamtAscii = value; }
        }

        private byte[] mCmdBuff = new byte[512];
        private byte[] mDataBuff = new byte[512];

        private byte[] mReciveBuffer = null;
        public byte[] ReciveBuffer
        {
            get { return mReciveBuffer; }
        }

        private Thread pollThread = null;
        private bool bExitThread = false;
        private bool bRealData = false;
        private bool m_bData = false;

        public SerialManager(LeakDetectorManager dm, ConfigFile file)
        {
            sPort.ReadBufferSize = 16;
            sPort.PortName = m_szPort;
            sPort.BaudRate = 38400;
            sPort.DataBits = 8;
            sPort.StopBits = StopBits.One;
            sPort.Parity = Parity.None;
            sPort.Handshake = Handshake.None;
            sPort.WriteBufferSize = 16;
        }

        void sPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            byte[] array = new byte[1024];
            int temp;
            string str = string.Empty;

            temp = sPort.Read(array, 0, 1);
            if (temp > 0)
            {
                bRealData = true;
                AddLog(array, temp);
            }
        }

        public bool IsConnected
        {
            get { return sPort.IsOpen; }
        }

        public bool BeginServer()
        {
            bExitThread = false;

            return Connect();
        }

        public void StopServer()
        {
            bExitThread = true;
            pollThread = null;

            Disconnect();
        }

        private bool Connect()
        {
            if (IsConnected == true)
                return true;

            if (IsConnected == false)
            {
                try
                {

                    sPort.Open();
                    sPort.DataReceived += serialPort1_DataReceived;
                    if (sPort.CDHolding == true)
                    {
                        Debug.WriteLine("CA DETECT");
                    }
                    return true;
                }
                catch (System.Exception e)
                {

                    Debug.WriteLine(e.StackTrace);
                }
            }
            return false;
        }

        private void Disconnect()
        {
            if (IsConnected == true)
            {
                try
                {
                    sPort.DataReceived -= serialPort1_DataReceived;
                    sPort.Close();
                }
                catch (System.Exception)
                {
                }
            }            
        }

        public void SendBytes(byte[] CmdBuff)
        {
            try
            {
                if (sPort.IsOpen == true)
                {
                    m_bData = true;
                    Array.Copy(CmdBuff, mDataBuff, CmdBuff.Length);
                    //sPort.DiscardOutBuffer();

                    // sPort.DtrEnable = true;

                    sPort.RtsEnable = true;
                    sPort.Write(mDataBuff, 0, CmdBuff.Length);
                    Thread.Sleep(50);
                    sPort.RtsEnable = false;

                    //   sPort.DtrEnable = false;

                    SendLog(CmdBuff, CmdBuff.Length);
                }
            }
            catch(Exception)
            {
            }            
        }

        private bool m_bReciverData = false;

        public bool ReciverData
        {
            get { return m_bReciverData; }
            set { m_bReciverData = value; }
        }

        private byte[] tempBuffer = null;
        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int nRead = sPort.BytesToRead;
            if (nRead >= 0)
            {
                // 얼마냐 ?
                byte[] buffer = new byte[nRead];
                //혹시나 숫자 여부로 나누는 에러필요할때 쓰자. 읽은 내용 순간 버퍼값 다 내놔 1대 8bit
                int count = sPort.Read(buffer, 0, nRead);
                //바이트 그룹을 문자로 
                if (count > 0)
                {
                    //AddTempLog(buffer, count);
                }

                if (tempBuffer == null && IsCompleteData(buffer))
                {
                    if (tempBuffer == null)
                    {
                        mReciveBuffer = buffer;
                    }
                    else
                    {
                        mReciveBuffer = new byte[tempBuffer.Length + buffer.Length];
                        Array.Copy(tempBuffer, 0, mReciveBuffer, 0, tempBuffer.Length);
                        Array.Copy(buffer, 0, mReciveBuffer, tempBuffer.Length, buffer.Length);

                    }

                    AddLog(mReciveBuffer, mReciveBuffer.Length);
                    tempBuffer = null;

                    m_bReciverData = true;
                }
                else
                {
                    if (tempBuffer == null)
                    {
                        tempBuffer = buffer;
                    }
                    else
                    {
                        int nLength = tempBuffer.Length;
                        byte[] temp = tempBuffer;
                        tempBuffer = new byte[nLength + buffer.Length];
                        Array.Copy(temp, tempBuffer, nLength);
                        Array.Copy(buffer, 0, tempBuffer, nLength, buffer.Length);

                        if (IsCompleteData(tempBuffer))
                        {
                            mReciveBuffer = tempBuffer;
                            AddLog(mReciveBuffer, mReciveBuffer.Length);
                            tempBuffer = null;
                        }
                    }
                }
                //sPort.DiscardInBuffer();
            }
            else
            {
                //m_bReciverData = false;
            }
           
        }

        public void ClearBuffer()
        {
            mReciveBuffer = null;
            tempBuffer = null;
        }

        private bool IsCompleteData(byte[] data)
        {
            if (data == null)
                return false;

            if (data.Length < 3)
                return false;

            int nLength = data[2] + 5;

            if (data.Length < nLength)
                return false;

            return true;
        }      

        private void SendLog(Byte[] bufRecive, int ret)
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

            Debug.WriteLine("[SEND TXT] : " + tmp);
        }

        private void AddLog(Byte[] bufRecive, int ret)
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
            Debug.WriteLine("[RECIVED TXT] : " + tmp);
        }
    }    
}
