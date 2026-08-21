using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Diagnostics; 

namespace JubixNetwork
{
    public class JubixClientProvider : ClientServiceProvider
    { 
        private IJubixNetwork m_mgr = null;
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
        private System.Timers.Timer rTimer = null;
        private double checkElasedTime = 60000;     //1분 경과 시간 체크
        private Stopwatch sw = new Stopwatch();
        public JubixClientProvider(IJubixNetwork mgr)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_mgr = mgr;

            this.LengthAdd = false;


			this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);



            rTimer = new System.Timers.Timer();
            rTimer.Interval = 1000;
            rTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeCheck);
            rTimer.Start();
			//this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }
       
        public void TimeCheck(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (sw.ElapsedMilliseconds > checkElasedTime)
            {
                this.Close();       //Connection 종료
                sw.Reset();
                logger.Debug("Jubix Client Provider - Elapsed 1 minute after last Received Time : Reconnect Jubix" );
            }
        }
        
        public override void OnReceiveData()
        {
            if (ReceivedData != null)
            {
                m_isReadingProcess = true;

                int nBytesCount = ReceivedData.Count();
                logger.Debug("Jubix Client Provider - ReceivedData.Count : " + nBytesCount);
                if (nBytesCount > 0)
                {
                    m_nPingCount = 0;

                    if (!CheckValidation(ReceivedData))
                        goto RETURN;
                                        
                    AddLog(ReceivedData, ReceivedData.Length);
                    int nHeader;
                    JubixNetwork.JubixMessage msg = JubixNetwork.JubixMessage.ReadDataValue(ReceivedData, out nHeader);
                  
                    // 현재값 조회 응답
                    if(msg.Command == JubixNetwork.JUBIX_TCP_COMMAND.AI)
                    {
                        sw.Reset();
                        sw.Start();
                        if (nBytesCount > 61)
                        {
                            this.Close();       //Connection 종료
                            logger.Debug("Jubix Client Provider - Socket Close - reconnect after few seconds");
                        }
#if !NO_DB              
                        if (IsValidMessage(msg))
                        {
                            ProcessMessage(msg);                           
                        }  
#endif
                    }
                }
            }

        RETURN:
            m_isReadingProcess = false;
        }

        private bool IsValidMessage(JubixNetwork.JubixMessage msg)
        {
            if (msg.IsValid())
                return true;
            return false;
        }

        private void ProcessMessage(JubixNetwork.JubixMessage msg)
        {
            JubixNetwork.JubixSensorManager.Instance.ProcessMessage(msg);            
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
             
            logger.Debug("[SEND TXT - Jubix] : " + tmp);
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
            //string tmp2 = System.Text.Encoding.ASCII.GetString(bufRecive);
             
            logger.Debug("[RECIVED BIN - Jubix] : " + tmp);

            WriteByteArray(bufRecive);
        } 

        private void WriteByteArray(byte[] bytes)
        {
            System.Diagnostics.Debug.Write("{");
            for (int i = 0; i < bytes.Length; i++)
            {
                System.Diagnostics.Debug.Write(string.Format("{0:X}", bytes[i]));
                System.Diagnostics.Debug.Write(" ");
            }
            System.Diagnostics.Debug.WriteLine("}");
        }
     
        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 6)
                return false;

            //int nStart = BitConverter.ToInt16(bytes, 0);

            //if (bytes[0] != JubixNetwork.JUBIX_TCP_FLAG.START)
            //    return false;

            //if (bytes[length - 1] != (JubixNetwork.JUBIX_TCP_FLAG.END)
            //    return false;
            

            //int nChunkCount = (int)bytes[1];
            //int nIndex = 6;

            //for (int i = 0; i < nChunkCount; i++)
            //{
            //    if (length < nIndex + 5)
            //        return false;

            //    int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            //    if (length < nIndex + 5 + nDataLength)
            //        return false;

            //    nIndex += 5 + nDataLength;
            //}

            return true;
        }

       
		public void SendData(short header)
		{
			 JubixNetwork.JubixMessage msg = new JubixNetwork.JubixMessage(header);
             msg.SetTime();
             byte[] datas = msg.MakeByte(false);

             m_mgr.Send(datas, this);

             SendLog(datas, datas.Length);
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
