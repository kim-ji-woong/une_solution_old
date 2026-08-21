using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using DBUtility2;

namespace libTTS
{
    class SerialManager
    {
        private SerialPort sPort = new SerialPort();
        private Utility m_ini = new Utility();
        private string m_szPort = "COM1";
        private int m_nSwitchNum = 1;
        private int m_nMicOnNum = 2;
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
            sPort.ReadBufferSize = 1024;

            sPort.PortName = m_szPort;
            sPort.BaudRate = 9600;
            sPort.DataBits = 8;
            sPort.StopBits = StopBits.One;
            sPort.Parity = Parity.None;
            sPort.Handshake = Handshake.None;
            sPort.WriteBufferSize = 8;

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
                string szSwitchNum = m_ini.getinivalue(strSection, "switch_on");
                m_nSwitchNum = int.Parse(szSwitchNum);
            }
            catch (System.Exception)
            {
            }

            try
            {
                string szMicOnNum = m_ini.getinivalue(strSection, "mic_on");
                m_nMicOnNum = int.Parse(szMicOnNum);
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

        public void Start()
        {
            Connect();
            if (sPort.IsOpen)
            {
                mCmdBuff[2] = 0x01;        // Action
                mCmdBuff[2 + m_nSwitchNum] = 0x02; // on
                mCmdBuff[2 + m_nMicOnNum] = 0x02;  // on
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

        public void Stop()
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

        public bool CheckSwitch()
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
