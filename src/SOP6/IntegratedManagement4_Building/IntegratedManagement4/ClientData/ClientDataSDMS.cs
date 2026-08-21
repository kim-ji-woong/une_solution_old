using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Threading;
using SDMS;
using System.Diagnostics;

namespace IntegratedManagement4
{
	internal class ClientDataSDMS : ClientData
	{
        private const string m_strLogFileName = "SDMSInternal.log";
        public static string LogFileName
        {
            get { return m_strLogFileName; }
        }

		public ClientDataSDMS(ServiceProvider provider)
		{
			m_provider = provider;
			Type = ClientType.SDMS_CLIENT;

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
                case InternalMessage.SDMS_2_SOP_SIMULATOR:
                    SendLog(bytes, "SOP Simulator", arrDatas);
                    Process process = ProcessManager.Instance.RunCheckProcess("SOPSimulator2");
                    if (process != null)
                    {
                        this.m_provider.SendDataToOther(bytes, arrDatas, this, false, ClientType.SOP_SIMULATOR);
                    }
                    else if (arrDatas.Count >= 2 && arrDatas[1] is short)
                    {
                        short cmd = (short)arrDatas[1];

                        if (cmd == InternalMessage.SdmsToSopSimulator.SHOW_HIDE_SOP_SIMULATOR ||
                            cmd == InternalMessage.SdmsToSopSimulator.SHOW_SOP_SIMULATOR ||
                            cmd == InternalMessage.SdmsToSopSimulator.SHOW_SOP_SIMULATOR_IF_INVISIBLE)
                            FormMain.Instance.ExecuteManager.Run("SOPMonitoringSystem");
                    }
                    /*else
                    {
                        FormMain.Instance.ExecuteManager.Run("SOPMonitoringSystem");                        
                    }*/

                    if (arrDatas.Count >= 2 && arrDatas[1] is short)
                    {
                        short cmd = (short)arrDatas[1];

                        // SDMS가 없이 SOP Simulator만 단독으로 실행되고 있는 경우에도 SDMS_to_SOPSimulator 메시지를 받을수 있도록
                        // SOP Server에 메시지를 전달한다.
                        // SDMS -> 통합관리자(Local) -> SOP Server -> 모든 통합관리자(Except Local) -> 모든 SOP Simulator(Except Local)
                        if (cmd == InternalMessage.SdmsToSopSimulator.OPEN_SOP_FIRE ||
                            cmd == InternalMessage.SdmsToSopSimulator.OPEN_SOP_PSM ||
                            cmd == InternalMessage.SdmsToSopSimulator.OPEN_SOP_SECURITY)
                        {
                            FormMain.Instance.NetManager.SendMessage(SOPWebServer.Header.INTERNAL_MESSAGE, SOPWebServer.BinaryHelper.MakeBytes(arrDatas));
                        }
                    }
                   
                    break;

                case InternalMessage.SDMS_2_MANAGER:
                    ProcessMessageFromSDMS(arrDatas);
                    break;

                default:
                    SendLog(bytes, "", 0);
                    break;
            }

            return true;
        }

        private void ProcessMessageFromSDMS(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount >= 2 && arrDatas[1] is short)
            {
                short cmd = (short)arrDatas[1];

                if (cmd == InternalMessage.SdmsToManager.CLEAR_N_ADD_PROCESS_IDS)
                {
                    if (nDataCount >= 3 && arrDatas[2] is int)
                    {
                        int nIDCount = (int)arrDatas[2];

                        if (nIDCount == nDataCount - 3)
                        {
                            List<int> ids = new List<int>();

                            for (int i=3;i<nDataCount;i++)
                            {
                                if (arrDatas[i] is int)
                                {
                                    ids.Add((int)arrDatas[i]);
                                }
                                else
                                    return;
                            }

                            ProcessManager.Instance.ClearNAddProcessIDs(ids);
                        }
                    }
                }
                else if (cmd == InternalMessage.SdmsToManager.ADD_PROCESS_IDS)
                {
                    if (nDataCount >= 3 && arrDatas[2] is int)
                    {
                        int nIDCount = (int)arrDatas[2];

                        if (nIDCount == nDataCount - 3)
                        {
                            List<int> ids = new List<int>();

                            for (int i = 3; i < nDataCount; i++)
                            {
                                if (arrDatas[i] is int)
                                {
                                    ids.Add((int)arrDatas[i]);
                                }
                                else
                                    return;
                            }

                            ProcessManager.Instance.AddProcessIDs(ids);
                        }
                    }
                }
                else if (cmd == InternalMessage.SdmsToManager.REMOVE_PROCESS_IDS)
                {
                    if (nDataCount >= 3 && arrDatas[2] is int)
                    {
                        int nIDCount = (int)arrDatas[2];

                        if (nIDCount == nDataCount - 3)
                        {
                            List<int> ids = new List<int>();

                            for (int i = 3; i < nDataCount; i++)
                            {
                                if (arrDatas[i] is int)
                                {
                                    ids.Add((int)arrDatas[i]);
                                }
                                else
                                    return;
                            }

                            ProcessManager.Instance.RemoveProcessIDs(ids);
                        }
                    }
                }
                else if (cmd == InternalMessage.SdmsToManager.CLEAR_PROCESS_IDS)
                {
                    ProcessManager.Instance.ClearProcessIDs();
                }
            }
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            return true;
        }
	}
}
