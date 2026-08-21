using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace SensorMonitor
{
    public class ReciverClientProvider
    {
        private static log4net.ILog logger = null;

        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;
        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        private int m_hDevice = -1;
        private int m_nReciverNum = -1;

        private Dictionary<int, Curcuit> m_nCircuits = null;

        private string m_szIPAddress = "";
        public string ReciverAddress
        {
            get { return m_szIPAddress; }
            set { m_szIPAddress = value; }
        }

        private int m_nPort = 0;
        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }


        private string m_szLastErrorMsg = "";
        public string LastErrorMsg
        {
            get { return m_szLastErrorMsg; }
            set { m_szLastErrorMsg = value; }
        }

        private bool m_bIsConnected = false;
        public bool IsConnected
        {
            get { return m_bIsConnected; }
            set { m_bIsConnected = value; }
        }
        public void Init()
        {
            IPSerial.nsio_init();
        }

        public void End()
        {
            IPSerial.nsio_end();
        }

        public bool Connect()
        {
			m_bIsConnected = false;
         
            int nRet = IPSerial.nsio_checkalive(m_szIPAddress, 2000);
            if (nRet < IPSerial.NSIO_OK)
            {
                try
                {
                    ExitClose();
                }
                catch (Exception)
                {

                }

                Debug.WriteLine("[" + m_szIPAddress + "][Server Alive Check Fail]");
                logger.Debug("[" + m_szIPAddress + "][Server Alive Check Fail]");

                //Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                return false;
            }

            //Thread.CurrentThread.Priority = ThreadPriority.Normal;

            int nPort = m_Reciver.Port;
            m_hDevice = IPSerial.nsio_open(m_szIPAddress, 2, 6000);            
            if (m_hDevice < IPSerial.NSIO_OK)
            {
                try
                {
                    Close();
                }
                catch (Exception)
                {

                }

                m_szLastErrorMsg = "Open port Two fail! - Code(" + m_hDevice.ToString() + ")";
                logger.Debug("[" + m_szIPAddress + "][ERROR] : " + m_szLastErrorMsg + ", " + m_hDevice.ToString());
                Debug.WriteLine("[" + m_szIPAddress + "][ERROR] : " + m_szLastErrorMsg + ", " + m_hDevice.ToString());
                return false;
            }

            // baudrate 9600, N81
            int nBaud = IPSerial.GetBuadrate(m_Reciver.BuadRate);
            int nMode = m_Reciver.Mode;


            int ret = IPSerial.nsio_ioctl(m_hDevice,IPSerial.B9600, 0x03);
            if (ret < IPSerial.NSIO_OK)
            {
                try
                {
                    Close();
                }
                catch (Exception)
                {

                }
                m_szLastErrorMsg = "Open port Two IO control settings fail! - Code(" + ret.ToString() + ")"; 
                logger.Debug("[" + m_szIPAddress + "][ERROR] : " + m_szLastErrorMsg);
                Debug.WriteLine("[" + m_szIPAddress + "][ERROR] : " + m_szLastErrorMsg);
                return false;
            }
        
            // HW flow control
            int nFlow = m_Reciver.FlowCtrl;
            ret = IPSerial.nsio_flowctrl(m_hDevice, nFlow);
            if (ret < IPSerial.NSIO_OK)
            {
                try
                {
                    Close();
                }
                catch (Exception)
                {

                }
                m_szLastErrorMsg = "Open port Two flow control settings fail!";
                logger.Debug("[" + m_szIPAddress + "][ERROR] : " + m_szLastErrorMsg);
                Debug.WriteLine("[" + m_szIPAddress + "][ERROR] : " + m_szLastErrorMsg);
                return false;
            }

            int nDI = IPSerial.nsio_data_status(m_hDevice);


            IPSerial.nsio_iqueue(16);
            IPSerial.nsio_oqueue(16);

            m_bIsConnected = true;
#if WIN
            FormMain.Instance.OnConnectReciver(m_Reciver.ID);
#endif
            SendConnect();
           
            m_Reciver.IsConnected = true;
            
            Debug.WriteLine("[" + m_szIPAddress + "][Connect Suceesss]");
            logger.Debug("[" + m_szIPAddress + "][Connect Suceesss]");
            return true;
        }

        public void ExitClose()
        {
            if (m_hDevice >= 0)
            {
                IPSerial.nsio_close(m_hDevice);
                
            }
            m_hDevice = -1;
        }

        public void Close()
        {
            try
            {
                ExitClose();
            }
            catch(Exception)
            {
            }            
            m_bIsConnected = false;           

#if WIN
            FormMain.Instance.OnDisconnectReciver(m_Reciver.ID);
#endif
            logger.Debug("[" + m_szIPAddress + "][Connect Close]");
            Debug.WriteLine("[" + m_szIPAddress + "][Connect Close]");

            SendDisconnect();
            m_Reciver.IsConnected = false;
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
            string tmp2 = System.Text.Encoding.ASCII.GetString(bufRecive);

            logger.Debug("[" + m_szIPAddress + "][RECIVED TXT] : " + tmp2);
            logger.Debug("[" + m_szIPAddress + "][RECIVED BIN] : " + tmp);

            Debug.WriteLine("[" + m_szIPAddress + "][RECIVED BIN] : " + tmp);
        }

        private byte[] bufPreRecive = new byte[200];
        private byte[] bufTemp = new byte[200];

        private int FindETX(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)               
            {
                if (bytes[i] == 0x03)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindSTX(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x02)
                {
                    return i;
                }
            }
            return -1;
        }
        public bool OnReceiveData()
        {
            byte[] bufRecive = new byte[16];
            int ret;

            int nResult = IPSerial.nsio_checkalive(m_szIPAddress, 10);
            if( nResult < 0)
            {
                m_nPingCount = 201;
                return false;
            }

            ret = IPSerial.nsio_read(m_hDevice, bufRecive, 16);



            if (ret > 0)
            {

                //AddLog(bufRecive, ret);
                //if (bufRecive[ret - 1] == 0x03)
                //{
                    if (!IsPoll(bufRecive))
                    {
                        byte[] data = new byte[ret];
                        int startIndex = 0;
                        if (startIndex >= 0)
                        {
                            Array.Copy(bufRecive, startIndex, data, 0, ret);
                        
                            
                            //02 50 4F 4C 03 00 00 00 00 00 00 00 00 00 00 00 - poll
                            if (CheckValidation(data))
                            {
                                // ACK를 보낸다.
                                SendACK();
                                AddLog(data, ret);
                                // 회로번호를 가져온다.
                                int nCurcuit = GetCurcuit(data);

                                // 해당 데이터를 처리한다.
                                if (nCurcuit != -1)
                                    ProcessSensorData(data, nCurcuit);
                            }
                            else
                            {
                                SendNACK();
                            }
                        }
                        else
                        {
                            // POL인경우 ACK를 보낸다.
                            SendACK();
                        }
                    //}                  
                    
                }
                else
                {                    
                    SendACK();
                }                
                m_nPingCount = 0;
            }
            else if( ret < 0)
            {
                return false;
            }

            IPSerial.nsio_flush(m_hDevice, 0);

            return true;
        }

        private bool IsPoll(byte[] data)
        {
            int nSTX = FindSTX(data);
            int nETX = FindETX(data);

            char b2 = (char)data[nSTX+1];
            char c1 = (char)data[nSTX+2];
            char c2 = (char)data[nSTX+3];

            StringBuilder sb2 = new StringBuilder();
            sb2.Append(b2);
            sb2.Append(c1);
            sb2.Append(c2);
            string szTag = sb2.ToString();
            if (szTag == "POL")
            {
                Debug.WriteLine("[" + m_szIPAddress + "][RECIVED TXT] : POL");
                return true;
            }

            return false;
        }

        private int GetCurcuit(byte[] data)
        {
            char b2 = (char)data[5];
            char c1 = (char)data[7];
            char c2 = (char)data[8];

            StringBuilder sb2 = new StringBuilder();
            sb2.Append(b2);
            sb2.Append(c1);
            sb2.Append(c2);
            string szTag = sb2.ToString();

            int nCurcuitID = -1;
            if (int.TryParse(szTag, out nCurcuitID))
            {
                return nCurcuitID;
            }
            return -1;
        }

        private Reciver m_Reciver = null;
        public ReciverClientProvider(NetworkManager mgr, Reciver reciver)
        {
            m_Reciver = reciver;
            m_mgr = mgr;

            m_nPort = reciver.Port;
            m_szIPAddress = reciver.Address;
            m_nCircuits = reciver.Curcuits;
            m_nReciverNum = reciver.ID;

            m_bIsConnected = false;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private bool CheckSum(byte[] buffer)
        {
            if (buffer.Length < 11)
                return false;
            byte sum = (byte)(((buffer[0] + buffer[1] + buffer[2] + buffer[3] + buffer[4] + buffer[5] + buffer[6] + buffer[7] + buffer[8] + buffer[9] + buffer[11]) % (byte)16) + (byte)0x30);

            return (sum == buffer[10] ? true : false);
        }

        private void ProcessSensorData(byte[] bytes, int nCurcuit)
        {
            if (bytes[9] == 'E' || bytes[9] == 'e')
                return;

            if (bytes[9] == 'R')
            {
                SendReset();
                return;
            }
          
            Curcuit curcuit = null;
            if (m_nCircuits.ContainsKey(nCurcuit))
            {
                curcuit = m_nCircuits[nCurcuit];
            }

            // 회로번호가 없는 경우
            if (curcuit == null)
            {
                logger.Debug("없는 회로 번호 : " + nCurcuit);
                return;
            }

            int isFire = bytes[4] - '0';
            byte nData = 0;
            if (bytes[9] == 'N' && isFire == 1)
            {
                nData = 1;
            }
            else if (bytes[9] == 'N' && isFire == 2)
            {
                if (curcuit.SensorType == 3)
                {
                    nData = 1;
                }                
            }
            else if (bytes[9] == 'F' && isFire == 2)
            {
                if (curcuit.SensorType == 3)
                {
                    nData = 0;
                }
            }
            else if (bytes[9] == 'F' && isFire == 1)
            {
                nData = 0;
            }
            else if (bytes[9] == 'R')
            {
                SendReset();
                return;
            }
            else
            {
                logger.Debug("처리할 수없는 데이터 유형 : " + nCurcuit);
                return;
            }

            SendSensorData(curcuit, nData);            						
        }

        private void SendSensorData(Curcuit curcuit, int nData)
        {
            int nCurcuit = curcuit.TagNum;

            //logger.Debug("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");
            Debug.WriteLine("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

			 int nSensorZoneID = curcuit.TargetZoneID;
            
            int nTagNum = curcuit.TagNum;
            int nSensorType = curcuit.SensorType;
			
            m_mgr.SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString());


        }

        private void SendReset()
        {
			int nData = 0;
			foreach (KeyValuePair<int, Curcuit> pair in m_nCircuits)
			{
				Curcuit curcuit = pair.Value;
				SendSensorData(curcuit, nData);
				Thread.Sleep(50);
			}			
        }

        private void SendDisconnect()
        {
#if WIN
            // nothing
#else
            m_Reciver.IsConnected = false;
            m_mgr.SendReciverState(m_Reciver.ID, false);
#endif

        }

        private void SendConnect()
        {
#if WIN
            // nothing
#else
            m_Reciver.IsConnected = true;
            m_mgr.SendReciverState(m_Reciver.ID, true);
#endif

        }

        private bool CheckValidation(byte[] bytes)
        {
            bool bCheck = CheckSum(bytes);
            if (bCheck == true)
                return true;

            return false;
        }

        /// <summary>
        /// Send ACK 
        /// </summary>
        public void SendACK()
        {
            SendData(SERIAL_ID.ACK);
        }

        public void SendNACK()
        {
            SendData(SERIAL_ID.NACK);
            Debug.WriteLine("Send NACK");
        }

        // header 1 Byte로만 이루어진 데이터
        private void SendData(byte header)
        {
            //02 50 4F 4C 03 00 00 00 00 00 00 00 00 00 00 00 - poll
            //sb = new StringBuilder();
            //sb.Append((char)header);
            string sz = ((char)header).ToString();
            
            //logger.Debug("["+m_szIPAddress+"][SEDN BIN : " + sz + "]")534
            //byte[] data = new byte[1];
            //data[0] = header;
            if (m_hDevice >= 0)
            {
                 //IPSerial.nsio_SetWriteTimeouts(m_hDevice, 0);
                //IPSerial.nsio_lctrl(m_hDevice, 1);

                //int nLine = IPSerial.nsio_lstatus(m_hDevice);
                //Debug.WriteLine("LineStatus : " + nLine);
                //IPSerial.nsio_DTR(m_hDevice, 1);
                int nRet = IPSerial.nsio_write(m_hDevice, sz, sz.Length);
                //IPSerial.nsio_flush(m_hDevice, 1);
                //Debug.WriteLine("Write RET : " + nRet);

               // IPSerial.nsio_DTR(m_hDevice, 0);
                //IPSerial.nsio_lctrl(m_hDevice, 0);

               // IPSerial.nsio_SetWriteTimeouts(m_hDevice, 100);
               
            }
        }
    }
	
}

