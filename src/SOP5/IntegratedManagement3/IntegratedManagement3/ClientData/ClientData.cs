using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Diagnostics;
using SDMS;

namespace IntegratedManagement3
{
    public abstract class ClientData
    {
        public enum ClientType
        {
            ALL = 0,
            SDMS_CLIENT = 1,
            SOP_SIMULATOR = 2,
            UNKNOWN = 11
        };

        private int m_nPingCount = 0;
        private ClientType m_type = ClientType.UNKNOWN;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        protected ServiceProvider m_provider = null;
        protected ConnectionState m_state = null;

        protected ConnectionLogEx2 m_log = null;

        public byte[] ReceivedData
        {
            get { return m_arrReceived; }
            set { m_arrReceived = value; }
        }

        public byte[] TempData
        {
            get { return m_arrTemp; }
            set { m_arrTemp = value; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public IntegratedManagement3.ServiceProvider ServiceProvider
        {
            get { return m_provider; }
            set { m_provider = value; }
        }

        public TcpLib2.ConnectionState ConnectionState
        {
            get { return m_state; }
            set { m_state = value; }
        }

        // bytes는 length byte가 제거되었음
        protected abstract bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas);

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected virtual bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }

        protected bool ProcessFirstConnection(ClientData data, ConnectionState state)
        {
            return data.ProcessFirstConnection(state);
        }

        public virtual bool OnReceiveData(ConnectionState state, byte[] bytes, bool checkValidation = true)
        {
            ArrayList arrDatas;
            byte[] curReceivedData = null;
            int nHeader = GetHeader(state, bytes, out curReceivedData, checkValidation, out arrDatas);

            if (nHeader < 0)
                return false;
            else if (nHeader == 0)
                return true;

            // I_AM_HERE는 ClientData에서 처리한다.
            if (nHeader == TCP_ID.I_AM_HERE)
            {
                return ProcessIAmHere(arrDatas);
            }          
            
            bool bResult = OnReceive(state, curReceivedData, nHeader, arrDatas);           
            return bResult;
        }

        protected virtual bool ProcessIAmHere(ArrayList arrDatas)
        {
            return true;
        }

        protected bool CheckValidation(ConnectionState state)
        {
            byte[] bytes = this.ReceivedData;

            int length = bytes.Length;
            if (length < 6)
                return false;

            int nDataCount = BitConverter.ToInt32(bytes, 0);

            int nTotalData = (nDataCount + 4);
            int nIndex = nTotalData;

            if (length > nIndex)
            {
                byte[] bytes1 = new byte[nIndex];
                byte[] bytes2 = new byte[length - nIndex];

                Array.Copy(bytes, bytes1, nIndex);
                Array.Copy(bytes, nIndex, bytes2, 0, length - nIndex);

                OnReceiveData(state, bytes1, false);

                if (!OnReceiveData(state, bytes2))
                    return false;

                this.ReceivedData = null;
                return false;
            }
            else if (length < nIndex)
                return false;

            return true;
        }

        // Return 값 : 0보다 작으면 validation 실패
        //             0이면 읽을 데이터가 없음
        protected int GetHeader(TcpLib2.ConnectionState state, byte[] bytes, out byte[] curReceivedData, bool checkValidation, out ArrayList arrDatas)
        {
            arrDatas = null;
            this.ReceivedData = bytes;
            curReceivedData = null;

			if (bytes == null)
				return 0;

            if (this.TempData != null)
            {
                int nReceivedCount = this.ReceivedData.Length;
                int nTempCount = this.TempData.Length;

                byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                Array.Copy(this.TempData, arrBuffer, nTempCount);
                Array.Copy(this.ReceivedData, 0, arrBuffer, nTempCount, nReceivedCount);

                this.ReceivedData = arrBuffer;
                this.TempData = null;
            }

            int nBytesCount = this.ReceivedData.Count();

            if (nBytesCount > 0)
            {
                this.PingCount = 0;

                if (checkValidation)
                {
                    if (!CheckValidation(state))
                    {
                        this.TempData = this.ReceivedData;
                        return -1;
                    }
                }

                int nDataLength = this.ReceivedData.Length - 4;
                curReceivedData = new byte[nDataLength];
                System.Buffer.BlockCopy(this.ReceivedData, 4, curReceivedData, 0, nDataLength);

                short nHeader;
                arrDatas = ServiceProvider.ReadBytes(curReceivedData, out nHeader);
                return nHeader;
            }

            return 0;
        }

        protected bool GetChunkDatai(byte[] bytes, ref int nIndex, out int nData)
        {
            nData = 0;

            if (bytes.Length < nIndex + 9)
                return false;

            if (bytes[nIndex] != TCP_TYPE.INTEGER)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength != 4)
                return false;

            nData = BitConverter.ToInt32(bytes, nIndex + 5);
            nIndex += 9;

            return true;
        }

        protected void RecvLog(byte[] bytes, string strSource, short cmd)
        {
            if (m_log == null)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE)
            {
                string strLog = "";
                
                if (strSource.Length > 0)
                    strLog = string.Format("RecvMessage from {0} : Header({1}), Command({2}), Length({3})", strSource, (int)bytes[0], cmd, (int)bytes.Length);
                else
                    strLog = string.Format("RecvMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);

                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                m_log.WriteLine(strLog + strBytes);
            }
        }

        public void RecvLog(ClientType from, byte[] bytes, ArrayList arrDatas)
        {
            if (m_log == null)
                return;

            string strSource = "";

            if (from == ClientType.SDMS_CLIENT)
                strSource = "SDMS";
            else if (from == ClientType.SOP_SIMULATOR)
                strSource = "SOP Simulator";

            if (arrDatas.Count > 1 && arrDatas[1] is short)
            {
                short cmd = (short)arrDatas[1];
                RecvLog(bytes, strSource, cmd);
            }
            else
                RecvLog(bytes, "", 0);
        }

        protected void SendLog(byte[] bytes, string strTarget, short cmd)
        {
            if (m_log == null)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE)
            {
                string strLog = "";
                
                if (strTarget.Length > 0)
                    strLog = string.Format("SendMessage to {0} : Header({1}), Command({2}), Length({3})", strTarget, (int)bytes[0], cmd, (int)bytes.Length);
                else
                    strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);

                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                m_log.WriteLine(strLog + strBytes);
            }
        }

        protected void SendLog(byte[] bytes, string strTarget, ArrayList arrDatas)
        {
            if (m_log == null)
                return;

            if (arrDatas.Count > 1 && arrDatas[1] is short)
            {
                short cmd = (short)arrDatas[1];
                SendLog(bytes, strTarget, cmd);
            }
            else
                SendLog(bytes, "", 0);
        }

        protected class ConnectionLogEx2
        {
            private System.IO.StreamWriter logger = null;
            private System.IO.FileStream m_stream = null;
            private string m_prevDate = "";
            private string m_strFilePath = "";

            public ConnectionLogEx2(string strFileName)
            {
                DateTime dtNow = DateTime.Now;

                string strDate = "";
                string strToday = string.Format("{0}-{1:00}-{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);
                
                m_strFilePath = GetFolderPath() + "\\" + strFileName;
                
                // 기존에 존재하는 파일이 있으면 날짜가 경과되었는지 확인하여
                // 날짜가 지났으면 해당 날짜의 파일로 이름을 바꾸어주고
                // 날짜가 지나지 않았으면 이어서 쓰도록 한다.
                if (ReadPrevLog(m_strFilePath, out strDate))
                {
                    if (strToday == strDate)
                    {
                        m_stream = System.IO.File.Open(m_strFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                        m_prevDate = strDate;
                    }
                    else
                    {
                        System.IO.File.Move(m_strFilePath, m_strFilePath + "-" + strDate);
                        m_stream = System.IO.File.Open(m_strFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                    }
                }
                else
                    m_stream = System.IO.File.Open(m_strFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);

                logger = new System.IO.StreamWriter(m_stream, Encoding.UTF8);
            }

            private string GetFolderPath()
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                int nIndex = strPath.LastIndexOf('\\');

                if (nIndex < 0)
                    strPath = ".\\logs";
                else
                    strPath = strPath.Substring(0, nIndex) + "\\logs";

                if (System.IO.Directory.Exists(strPath) == false)
                    System.IO.Directory.CreateDirectory(strPath);

                return strPath;
            }

            private bool ReadPrevLog(string strFileName, out string strDate)
            {
                strDate = "";

                if (System.IO.File.Exists(strFileName) == false)
                    return false;

                System.IO.FileStream stream = System.IO.File.Open(strFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                string strLine = reader.ReadLine();
                stream.Close();
                reader.Close();
                if (strLine == null) return false;

                int nIndex2 = strLine.IndexOf(' ');

                if (nIndex2 < 0)
                    return false;

                int nIndex1 = strLine.LastIndexOf('[', nIndex2);

                if (nIndex1 < 0)
                    return false;

                strDate = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                return true;
            }

            public bool WriteLine(string str)
            {
                if (logger != null)
                {
                    DateTime dtNow = DateTime.Now;
                    string strToday = string.Format("{0}-{1:00}-{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);
                    string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

                    if (m_prevDate.Length > 0 && m_prevDate != strToday)
                    {
                        m_stream.Close();
                        logger.Close();
                        System.IO.File.Move(m_strFilePath, m_strFilePath + "-" + m_prevDate);

                        m_stream = System.IO.File.Open(m_strFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                        logger = new System.IO.StreamWriter(m_stream, Encoding.UTF8);
                        //logger = new System.IO.StreamWriter(m_strFilePath, false, Encoding.UTF8);
                    }

                    logger.WriteLine("[" + strToday + " " + strTime + "] : " + str);
                    logger.Flush();

                    m_prevDate = strToday;
                }

                return true;
            }
        }
    }
}
