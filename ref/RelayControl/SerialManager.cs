using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;

namespace RelayControl
{
    class SerialManager
    {
        private SerialPort sPort = new SerialPort();
        private Utility m_ini = new Utility();
        private string m_szPort = "COM1";
        private int nBaudrate = 9600;
        private int nDataBits = 8;
        private int nStopBit = 1;
        private int nParity = 0;
        private int nHandShake = 0;
        private int nReadBufferSize = 1024;
        private int nWriteBufferSize = 8;
       
        private bool bSendForamtAscii = true;
        public bool SendForamt
        {
            get { return bSendForamtAscii; }
            set { bSendForamtAscii = value; }
        }

        private byte[] mCmdBuff = new byte[8];
        public SerialManager()
        {
            Load_SerialInfo();
                      
            
            sPort.PortName = m_szPort;
            sPort.BaudRate = nBaudrate;
            sPort.DataBits = nDataBits;
            sPort.StopBits = (StopBits)nStopBit;
            sPort.Parity = (Parity)nParity;
            sPort.Handshake = (Handshake)nHandShake;
            sPort.ReadBufferSize = nReadBufferSize;
            sPort.WriteBufferSize = nWriteBufferSize;

            mCmdBuff[0] = 0x55;
            mCmdBuff[1] = 0x01;


            mCmdBuff[2] = 0x00;

            mCmdBuff[3] = 0x01;
            mCmdBuff[4] = 0x01;
            mCmdBuff[5] = 0x01;
            mCmdBuff[6] = 0x01;
            mCmdBuff[7] = checkSum();
        }

        private byte checkSum()
        {
            return (byte)(mCmdBuff[0] + mCmdBuff[1] + mCmdBuff[2] + mCmdBuff[3] + mCmdBuff[4] + mCmdBuff[5] + mCmdBuff[6]);            
        }

        private void Load_SerialInfo()
        {
            string strSection = "Serial Info";
            m_szPort = m_ini.getinivalue(strSection, "port");
            try
            {
                string szBaud = m_ini.getinivalue(strSection, "BaudRate");
                nBaudrate = int.Parse(szBaud);
            }
            catch (System.Exception)
            {            	
            }
            
            try
            {
                string szDataBits = m_ini.getinivalue(strSection, "DataBits");
                nDataBits = int.Parse(szDataBits);
            }
            catch (System.Exception)
            {            
            }
            
            try
            {
                string szStopBit = m_ini.getinivalue(strSection, "StopBits");
                nStopBit = int.Parse(szStopBit);
            }
            catch (System.Exception)
            {
            }

            try
            {
                string szHandShake = m_ini.getinivalue(strSection, "Handshake");
                nHandShake = int.Parse(szHandShake);
            }
            catch (System.Exception)
            {
            }

            try
            {
                string szReadBufferSize = m_ini.getinivalue(strSection, "ReadBufferSize");
                nReadBufferSize = int.Parse(szReadBufferSize);
            }
            catch (System.Exception)
            {
            }

            try
            {
                string szWriteBufferSize = m_ini.getinivalue(strSection, "WriteBufferSize");
                nWriteBufferSize = int.Parse(szWriteBufferSize);
            }
            catch (System.Exception)
            {
            }

            try
            {
                string szSendForamtAscii = m_ini.getinivalue(strSection, "SendForamtAscii");
                bSendForamtAscii = bool.Parse(szSendForamtAscii);
            }
            catch (System.Exception)
            {
            }
        }

        private void Connect()
        {
            try
            {
                sPort.Open();
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
                sPort.Close();
            }
            catch (System.Exception)
            {
            }
        }


        public void RunRelay(int nChannel)
        {
            Connect();
            
            int nBit1 = nChannel & 1;
            int nBit2 = nChannel & 2;
            int nBit3 = nChannel & 4;
            int nBit4 = nChannel & 8;

            if (sPort.IsOpen)
            {
                mCmdBuff[2] = 0x01;        // Action

                mCmdBuff[3] = (nBit1 >= 1 ? (byte)0x02 : (byte)0x01);// on
                mCmdBuff[4] = (nBit2 >= 1 ? (byte)0x02 : (byte)0x01);// on
                mCmdBuff[5] = (nBit3>= 1 ? (byte)0x02 : (byte)0x01);// on
                mCmdBuff[6] = (nBit4 >= 1 ? (byte)0x02 : (byte)0x01);// on
                
                mCmdBuff[7] = checkSum();
                string szMessage = "";
                foreach (byte b in mCmdBuff)
                {
                    szMessage += string.Format("{0:x2} ", b);
                }
                //sPort.Write(szMessage);
                sPort.Write(mCmdBuff, 0, 8);
            }
            Disconnect();
        }

        public void StopRelay()
        {
            Connect();
            if (sPort.IsOpen)
            {

                mCmdBuff[2] = 0x01;        // Action
                mCmdBuff[3] = 0x01; // off
                mCmdBuff[4] = 0x01;  // off
                mCmdBuff[5] = 0x01; // off
                mCmdBuff[6] = 0x01;  // off
                mCmdBuff[7] = checkSum();
                string szMessage = "";
                foreach (byte b in mCmdBuff)
                {
                    szMessage += string.Format("{0:x2} ", b);
                }
               // sPort.Write(szMessage);
                sPort.Write(mCmdBuff, 0, 8);

            }
            Disconnect();
        }

        private bool bRealData = false;
        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] array = new byte[1024];
            int temp;
            string str = string.Empty;
           
            temp = sPort.Read(array, 0, 1024);
            if (temp > 0)
                bRealData = true;

        }

        public bool CheckRelay()
        {
            sPort.DataReceived += serialPort1_DataReceived;
            Connect();
            if (sPort.IsOpen)
            {
                string szMessage = "";
                mCmdBuff[2] = 0x00;        // Action
                mCmdBuff[3] = 0x00; // off
                mCmdBuff[4] = 0x00;  // off
                mCmdBuff[5] = 0x00; // off
                mCmdBuff[6] = 0x00;  // off
                mCmdBuff[7] = checkSum();
                szMessage = "";
                foreach (byte b in mCmdBuff)
                {
                    szMessage += string.Format("{0:x2} ", b);
                }
                sPort.Write(szMessage);

                mCmdBuff[2] = 0x01; // Action
                mCmdBuff[3] = 0x01; // off
                mCmdBuff[4] = 0x01;  // off
                mCmdBuff[5] = 0x01; // off
                mCmdBuff[6] = 0x01;  // off
                mCmdBuff[7] = checkSum();
                
                sPort.Write(mCmdBuff, 0, 8);
                //sPort.Write(szMessage);
            }
            Disconnect();
            sPort.DataReceived -= serialPort1_DataReceived;
            return true;
        }
    }
}
