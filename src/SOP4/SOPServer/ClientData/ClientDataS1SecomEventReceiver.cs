using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Collections;
using SDMS;

namespace SDMSServer
{
    public class ClientDataS1SecomEventReceiver : ClientData
    {
        private object m_LockObj = new object();
        private ClientDataCommon mCommonProcessor = null;

        public static string ServerTypeName
        {
            get { return "[Secom Manager]"; }
        }

        public ClientDataS1SecomEventReceiver(ServiceProvider provider)
        {
            m_szServerType = ServerTypeName;
            m_provider = provider;
            ClientType = SDMS.TCP_CLIENT.INTEGRATE_MANAGE;
            mCommonProcessor = new ClientDataCommon(provider, this, ClientType);
            //mCommonProcessor.CreateBeginReactionLog = this.CreateAccessEventLog;
            //mCommonProcessor.CreateIgnoreReactionLog = this.CreateIgnoreAccessEvent;
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            ReciverManager.Instance.UpdateState(ReciverType.SECOM_RECEIVER, true);
            return base.ProcessFirstConnection(state);
        }

        public override void CloseClient()
        {
            ReciverManager.Instance.UpdateState(ReciverType.SECOM_RECEIVER, false);
            base.CloseClient();
        }

        // 다른 ClientData에서 수신된 Data를 이용하여 Secom 이벤트를 처리하는 경우에 사용함
        // 주의점 : 내부에서 Lock을 사용하므로 동일 Lock루틴중에 사용하면 Deadlock이 발생함
        public bool ProcessSensorData(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            return OnReceive(state, bytes, nHeader, arrDatas);
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;
            return mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
        }
    }
}
