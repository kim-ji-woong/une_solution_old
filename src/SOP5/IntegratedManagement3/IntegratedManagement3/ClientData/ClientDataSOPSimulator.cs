using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TcpLib2;
using SDMS;
using System.Diagnostics;

namespace IntegratedManagement3
{
    internal class ClientDataSOPSimulator : ClientData
    {
        private const string m_strLogFileName = "SOPSimulatorInternal.log";
        public static string LogFileName
        {
            get { return m_strLogFileName; }
        }

        public ClientDataSOPSimulator(ServiceProvider provider)
		{
			m_provider = provider;
			Type = ClientType.SOP_SIMULATOR;

            m_log = new ConnectionLogEx2(LogFileName);
		}

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.INTERNAL_MESSAGE)
            {
                ProcessInternalMessage(arrDatas, bytes);
            }
            else
                SendLog(bytes, "", 0);

            return true;
        }

        private bool ProcessInternalMessage(ArrayList arrDatas, byte[] bytes)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0)
            {
                SendLog(bytes, "", 0);
                return false;
            }

            byte msg;

            try
            {
                msg = (byte)arrDatas[0];
            }
            catch (Exception)
            {
                SendLog(bytes, "", 0);
                return false;
            }

            switch (msg)
            {
                case InternalMessage.SOP_SIMULATOR_2_SDMS:
                    SendLog(bytes, "SDMS", arrDatas);
                    Process process = ProcessManager.Instance.RunCheckProcess("SDMS");
                    if (process != null)
                    {
                        this.m_provider.SendDataToOther(bytes, arrDatas, this, false, ClientType.SDMS_CLIENT);
                    }
                    else if (arrDatas.Count >= 2 && arrDatas[1] is short)
                    {
                        short cmd = (short)arrDatas[1];

                        if (cmd == InternalMessage.SopSimulatorToSdms.TOGGLE_MINIMUM_WINDOW)
                            FormMain.Instance.ExecuteManager.Run("SDMS");
                    }
                    /*else
                    {
                        FormMain.Instance.ExecuteManager.Run("SDMS");              
                    }*/
                    break;

                default:
                    SendLog(bytes, "", 0);
                    break;
            }

            return true;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }
    }
}
