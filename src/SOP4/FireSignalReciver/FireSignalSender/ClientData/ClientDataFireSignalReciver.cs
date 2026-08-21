using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using SDMS;
using System.Collections;
using System.Diagnostics;

namespace FireSignalSender
{    

    public class ClientDataFireSignalReciver : ClientData
    {
        public ClientDataFireSignalReciver(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.SOP_MONITOR2;
        }

		protected override bool ProcessFirstConnection(ConnectionState state)
		{
            m_provider.BeginSendSignal(this);
			return true;
		}

        protected virtual bool ProcessDropConnection(ConnectionState state)
        {
            m_provider.StopSendSignal(this);
            return true;
        }
        
        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;

            if (nHeader == TCP_ID.SENSOR_DATA)
            {
                // Send Sensor Data
            }
            else if(nHeader == TCP_ID.ALL_RECIVER_STATE)
            {
                // Send Reciver State
                ProcessRequestReiverState(state, arrDatas);
            }
            return true;
        }


        private void ProcessRequestReiverState(ConnectionState state, ArrayList arrDatas)
        {
            DataManager.Instance.ReadReciverState();
            ArrayList arList = (ArrayList)DataManager.Instance.ReciverState.Clone();
            byte[] data = ServiceProvider.MakeBytes(TCP_ID.ALL_RECIVER_STATE, arList);
            m_provider.SendData(data, false, ClientType.SOP_MONITOR2);
        }
    }
}
