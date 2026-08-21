using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace SerialServer
{
    class SerialManager
    {
        private SerialPort sPort = new SerialPort();
        
        private string m_szPort = "COM13";
        public string Port
        {
            get { return m_szPort; }
            set { m_szPort = value; }
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

        private byte[] mCmdBuff = new byte[12];
        private byte[] mDataBuff = new byte[12];
        private Thread pollThread = null;
        private bool bExitThread = false;
        private bool bRealData = false;
        private bool m_bData = false;
        
        public SerialManager()
        {            
            sPort.ReadBufferSize = 16;            
            sPort.PortName = m_szPort;
            sPort.BaudRate = 9800;
            sPort.DataBits = 8;
            sPort.StopBits = StopBits.One;
            sPort.Parity = Parity.None;
            sPort.Handshake = Handshake.None;
            sPort.WriteBufferSize = 16;
            
            sPort.DiscardNull = true;
            //sPort.RtsEnable = false;
            //sPort.DtrEnable = true;
            
            sPort.Encoding = Encoding.ASCII;
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
        
        public void BeginServer()
        {
            bExitThread = false;
            pollThread = new Thread(Polling);
            pollThread.Name = "PollingThread";
            pollThread.Start();

            //Connect();
        }

        public void StopServer()
        {
            bExitThread = true;
            try
            {
                pollThread.Join();
            }catch(Exception)
            {}
           
            pollThread = null;

           // Disconnect();
        }

        public void Connect()
        {
            try
            {
                sPort.PortName = m_szPort;
                sPort.Open();
                sPort.DataReceived += serialPort1_DataReceived;
                if( sPort.CDHolding == true)
                {
                    Debug.WriteLine("CA DETECT");
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        private void Disconnect()
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
        
        public void Polling()
        {
            Connect();

            while (!bExitThread)
            {
                if (m_bData == true)
                {
                    //sPort.Write(mDataBuff, 0, 16);
                    m_bData = false;
                    //sPort.BaseStream.Flush();
                    Thread.Sleep(1000);
                }
                else
                {
                    SendPol();
                    Thread.Sleep(1000);
                }

            }
        }

        public void SendPol()
        {
            try
            {
                mCmdBuff[0] = 0x02;        // Action
                mCmdBuff[1] = (byte)'P';
                mCmdBuff[2] = (byte)'O';
                mCmdBuff[3] = (byte)'L';
                mCmdBuff[4] = 0x03;  // off
                //sPort.DtrEnable = true;
                sPort.Write(mCmdBuff, 0, 5);
                //sPort.DtrEnable = false;
                Debug.WriteLine("SEND POL");
            }
            catch(Exception)
            {

            }
            
        }
       
        public void SendBytes(byte[] CmdBuff)
        {
            if (sPort.IsOpen == true)
            {
                m_bData = true;
                Array.Copy(CmdBuff, mDataBuff, 12);
                sPort.DiscardOutBuffer();

                //sPort.DtrEnable = true;


                sPort.Write(mDataBuff, 0, 12);
                Thread.Sleep(50);

                //sPort.DtrEnable = false;

                SendLog(CmdBuff, CmdBuff.Length);
            }           
        }

        public void ReciveData()
        {
           // sPort.RtsEnable = true;
            //if (sPort.IsOpen == true)
            //{
            //    if (sPort.BytesToRead > 0)
            //    {
            //        // 얼마냐 ?
            //        byte[] buffer = new byte[sPort.ReadBufferSize + 1];
            //        //혹시나 숫자 여부로 나누는 에러필요할때 쓰자. 읽은 내용 순간 버퍼값 다 내놔 1대 8bit
            //        int count = sPort.Read(buffer, 0, sPort.ReadBufferSize);
            //        //바이트 그룹을 문자로 
            //        if (count > 0)
            //        {
            //            bRealData = true;
            //            AddLog(buffer, count);
            //            sPort.DiscardInBuffer();
            //        }
            //    }
            //}
            //sPort.RtsEnable = false;
        }

        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {            
            if (sPort.BytesToRead >= 0)
            {
                // 얼마냐 ?
                byte[] buffer = new byte[sPort.BytesToRead + 1];
                //혹시나 숫자 여부로 나누는 에러필요할때 쓰자. 읽은 내용 순간 버퍼값 다 내놔 1대 8bit
                int count = sPort.Read(buffer, 0, sPort.BytesToRead);
                //바이트 그룹을 문자로 
                if (count > 0)
                {
                    bRealData = true;
                    AddLog(buffer, count);
                   // if (buffer[0] == (byte)21)
                   // {
                   //     Thread.Sleep(50);
                        //SendBytes(mDataBuff);
                   //     Thread.Sleep(50);
                        
                   // }
                   // else
                   // {
                        //Array.Clear(mDataBuff, 0, mDataBuff.Length);
                        //mDataBuff[0] = 0x02;        // Action
                        //mDataBuff[1] = (byte)'P';
                        //mDataBuff[2] = (byte)'O';
                        //mDataBuff[3] = (byte)'L';
                        //mDataBuff[4] = 0x03;  // off
                        
                   // }
                }

                sPort.DiscardInBuffer();
            }
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
