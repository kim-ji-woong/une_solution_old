using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Collections;
using System.IO;
using System.Threading;

namespace ServerSimulator
{
    class ServiceProvider : TcpLib2.TcpServiceProvider
    {
        public void RecvLog(byte[] bytes, ConnectionState state)
        {
            string strClient = state.RemoteEndPoint.ToString();

            string strLog = string.Format("RecvMessage : Length({0}) from {1}", (int)bytes.Length, strClient);
            string strBytes = "";

            foreach (byte b in bytes)
            {
                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLineLog(strLog + strBytes);
        }

        // arrDropList가 null이 아닐 경우, 예외가 발생하면 바로 OnDropConnection()을 호출하지 않고 해당 state를 일단 arrDropList에 담아둔다.
        // m_arrClient Loop 실행 도중 OnDropConnection() 호출로 인하여 m_arrClient가 변경되는 것을 막기 위함이다.
        private bool _Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, ArrayList arrDropList)
        {
            try
            {
                if (state.Connected == false)
                {
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);

                    return false;
                }


                if (state.WriteAsync(bytes, nOffset, nLength))
                {
                    try
                    {
                        string szRemote = state.RemoteEndPoint.ToString();

                        string strLog = string.Format("SendMessage : Length({0}) to {1}", nLength, szRemote);

                        bool bFirst = true;

                        foreach (byte b in bytes)
                        {
                            if (bFirst == true)
                            {
                                bFirst = false;
                                strLog += string.Format("\r\n\t\t{0:X2}", (int)b);
                            }
                            else
                                strLog += string.Format(" {0:X2}", (int)b);
                        }

                        WriteLineLog(strLog);
                    }
                    catch (System.Exception exx)
                    {
                        WriteLineLog("Write Send log : " + exx.Message);
                    }
                    return true;
                }
                else
                {
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);
                }
            }
            catch (Exception ex)
            {
                WriteLineLog("_Send : "+ ex.Message);

                if (arrDropList == null)
                    OnDropConnection(state);
                else
                    arrDropList.Add(state);

                return false;
            }
            return false;
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, bool noLock = false, ArrayList arrDropList = null)
        {
            if (!noLock)
            {

                return _Send(bytes, nOffset, nLength, state, arrDropList);

            }

            return _Send(bytes, nOffset, nLength, state, arrDropList);
            /*lock(this)
            {	
                if (state.Write(bytes, nOffset, nLength))
                {
                    if (!IsLogOpened)
                        return true;

                    if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
                    {
                        string strClient = "Unknown";

                        ClientData data = (ClientData)state.Tag;

                        if (data != null)
                        {
                            if (data.Type == ClientData.ClientType.SDMS_CLIENT)
                                strClient = "SDMS Client";
                            else if (data.Type == ClientData.ClientType.SENSOR_SIMULATOR)
                                strClient = "Sensor Simulator";
                            else if (data.Type == ClientData.ClientType.SOP_SIMULATOR)
                                strClient = "SOP Simulator";
                            else if (data.Type == ClientData.ClientType.SOP_MONITOR)
                                strClient = "Sensor Monitor";
                            else if (data.Type == ClientData.ClientType.SOP_RESOTRE)
                                strClient = "Restore Manager";
                            else if (data.Type == ClientData.ClientType.INTEGRATE_MANAGER)
                                strClient = "Integrate Manager";
                            else if (data.Type == ClientData.ClientType.SDMS_CLIENT_SECOND)
                                strClient = "SDMS Client Sub Line";
                        }

                        strClient += "(" + state.RemoteEndPoint.ToString() + ")";

                        string strLog = string.Format("SendMessage : Header({0}), Length({1}) to {2}", (int)bytes[nOffset], nLength, strClient);
                        string strBytes = "";

                        foreach (byte b in bytes)
                        {
                            if (strBytes.Length == 0)
                                strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                            else
                                strBytes += string.Format(" {0:X2}", (int)b);
                        }

                        WriteLineLog(strLog + strBytes);
                    }

                    return true;
                }
            }
            return false;*/
        }

        private static ServiceProvider m_Instance = null;
        public static ServiceProvider Instance
        {
            get { return ServiceProvider.m_Instance; }
        }

        public ServiceProvider()
        {
            m_Instance = this;
        }

        public override object Clone()
        {
            return this;
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            NetworkServer.Instance.AddClient(state);
        }

        public override bool OnReceiveData(ConnectionState state)
        {

            if (!base.OnReceiveData(state))
                return false;

            RecvLog(state.RecivedBuffer, state);
            ProcessData(state.RecivedBuffer, state);
            state.RecivedBuffer = null;
            return true;
        }

        private void ProcessData(byte[] bytes, ConnectionState state)
        {
        }

        private byte[] ToByteArray(string str)
        {
            string[] tokens = str.Split(' ');
            byte[] bytes = new byte[tokens.Count()];

            for (int i=0;i<tokens.Count();i++)
            {
                bytes[i] = byte.Parse(tokens[i].Trim(), System.Globalization.NumberStyles.HexNumber);
            }

            return bytes;
        }

        public override void OnDropConnection(ConnectionState state)
        {
            NetworkServer.Instance.RemoveClient(state);
        }

        private void WriteLineLog(string str)
        {
            ConnectionLogEx.Instance.WriteLine(str, true);
            System.Diagnostics.Trace.WriteLine(str);
        }
    }

    public class ConnectionLogEx : ConnectionLog
    {
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get
            {
                return m_instance2;
            }
        }

        private StreamWriter m_writer = null;

        public void InitLog(string strLogFilePath)
        {
            if (m_writer != null)
                m_writer.Close();

            if (strLogFilePath == null)
                m_writer = null;
            else
                m_writer = new StreamWriter(strLogFilePath, true, Encoding.UTF8);
        }

        public override bool Write(object str, bool writeTime = true)
        {
            if (m_writer == null)
                return true;

            if (writeTime)
                str = string.Format("[{0:00}:{1:00}:{2:00}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) + str;

            m_writer.Write(str);
            m_writer.Flush();

            return true;
        }

        public override bool WriteLine(object str, Exception e)
        {
            if (m_writer == null)
                return true;

            str = string.Format("[{0:00}:{1:00}:{2:00}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) + str;
            m_writer.WriteLine(e.Message);
            m_writer.Flush();

            return true;
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            if (m_writer == null)
                return true;

            if (writeTime)
                str = string.Format("[{0:00}:{1:00}:{2:00}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) + str;

            m_writer.WriteLine(str);
            m_writer.Flush();

            return true;
        }
    }
}
