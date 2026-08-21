using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Threading;
using SDMS;

namespace IntegratedManagement2
{
	public class ClientDataSDMS : ClientData
	{
        private bool m_waitSOPSimulatorPermission = false;
        private bool m_runSOPSimulatorPermission = false;
        private DateTime m_dtLastRequest = new DateTime();

        private int m_nSDMSIndex = 0;
        public int SDMSIndex
        {
            get { return m_nSDMSIndex; }
        }

		public ClientDataSDMS(ServiceProvider provider)
		{
			m_provider = provider;
			Type = ClientType.SDMS_CLIENT;
		}

		// bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
		{
            if (nHeader == TCP_ID.INTERNAL_MESSAGE)
            {
                ProcessInternalMessage(arrDatas);
            }

			return true;
		}

        private bool ProcessInternalMessage(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0)
                return false;

            byte msg;

            try
            {
                msg = (byte)arrDatas[0];
            }
            catch (Exception)
            {
                return false;
            }

            switch (msg)
            {
                case InternalMessage.REPLY_CHECK_SOP_SIMULATOR:
                    ProcessReplyCheckSOPSimulator(arrDatas);
                    break;
                case InternalMessage.REQUEST_PERMISSION_SOP_SIMULATOR:
                    ProcessRequestPermissionSOPSimulator(arrDatas);
                    break;
            }

            return true;
        }

        private void ProcessRequestPermissionSOPSimulator(ArrayList arrDatas)
        {
            if (m_waitSOPSimulatorPermission)
                return;

            DateTime dtNow = DateTime.Now;
            TimeSpan span = dtNow - m_dtLastRequest;
            
            // 마지막으로 REQUEST_PERMISSION_SOP_SIMULATOR 처리를 한지 1초가 지나지 않았으면 요청을 무시한다.
            if (span.TotalSeconds < 1.0)
            {
                return;
            }

            m_dtLastRequest = dtNow;

            ArrayList arrDatas2 = new ArrayList();
            arrDatas2.Add(InternalMessage.CHECK_SOP_SIMULATOR);
            arrDatas2.Add(this.GetHashCode());

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.INTERNAL_MESSAGE, arrDatas2);
            Thread t = new Thread(new ParameterizedThreadStart(WaitSOPSimulatorPermissionThread));
            t.Start(bytes);
        }

        private void WaitSOPSimulatorPermissionThread(object param)
        {
            byte[] bytes = (byte[])param;
            m_waitSOPSimulatorPermission = true;
            this.m_provider.SendDataToOther(bytes, this, false, ClientType.SDMS_CLIENT);

            // 1초 동안만 대기한다.
            for (int i=0;i<10;i++)
            {
                Thread.Sleep(100);

                if (!m_waitSOPSimulatorPermission)
                {
                    if (m_runSOPSimulatorPermission)
                        RunSOPSimulator();

                    return;
                }
            }

            m_waitSOPSimulatorPermission = false;

            // 1초 동안 응답이 없으므로 그냥 실행시킨다.
            RunSOPSimulator();
        }

        private void RunSOPSimulator()
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(InternalMessage.RUN_SOP_SIMULATOR);

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.INTERNAL_MESSAGE, arrDatas);
            this.m_provider.Send(bytes, 0, bytes.Length, m_state);
        }

        private bool ProcessReplyCheckSOPSimulator(ArrayList arrDatas)
        {
            if (arrDatas.Count < 2)
                return false;

            bool visibleSOPSimulator;

            try
            {
                visibleSOPSimulator = (bool)arrDatas[1];

                if (arrDatas.Count >= 3 && arrDatas[2] is int)
                {
                    int nHashCodeClient = (int)arrDatas[2];
                    ClientData client = m_provider.GetClientData(nHashCodeClient);

                    if (client != null && client is ClientDataSDMS)
                    {
                        ClientDataSDMS otherClient = (ClientDataSDMS)client;
                        otherClient.ProcessReplyCheckSOPSimulator(visibleSOPSimulator);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            if (visibleSOPSimulator)
            {
                m_provider.Server.LastCommand = NetworkServer.Command.NONE;
            }
            else
            {
                if (m_provider.Server.LastCommand == NetworkServer.Command.CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0)
                {
                    ConnectionState sdms1, sdms2;
                    m_provider.GetSDMSClients(out sdms1, out sdms2);

                    if (sdms1 != null)
                        m_provider.Server.ProcessCommand(sdms1, NetworkServer.Command.RUN_SOP_SIMULATOR0);
                    else
                        m_provider.Server.ProcessCommand(null, NetworkServer.Command.RESERVE_RUN_SOP_SIMULATOR0);
                }
            }

            return true;
        }

        private void ProcessReplyCheckSOPSimulator(bool visibleSOPSimulator)
        {
            m_waitSOPSimulatorPermission = false;
            m_runSOPSimulatorPermission = !visibleSOPSimulator;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            byte[] bytes = ReceivedData;

            int nIndex = 0;

            if (!GetChunkDatai(bytes, ref nIndex, out m_nSDMSIndex))
                return false;

            switch (m_provider.Server.LastCommand)
            {
                case NetworkServer.Command.RESERVE_RUN_SOP_SIMULATOR0:
                    if (m_nSDMSIndex == 0)
                    {
                        m_provider.RunSOPSimulator(this.ConnectionState);
                        m_provider.Server.LastCommand = NetworkServer.Command.NONE;
                    }
                    break;

                case NetworkServer.Command.RESERVE_RUN_SOP_SIMULATOR1:
                    if (m_nSDMSIndex == 1)
                    {
                        m_provider.RunSOPSimulator(this.ConnectionState);
                        m_provider.Server.LastCommand = NetworkServer.Command.NONE;
                    }
                    break;

                case NetworkServer.Command.RESERVE_CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0:
                    if (m_nSDMSIndex == 1)
                    {
                        m_provider.CheckSOPSimulator(this.ConnectionState);
                        m_provider.Server.LastCommand = NetworkServer.Command.CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0;
                    }
                    break;
            }

            return true;
        }
	}
}
