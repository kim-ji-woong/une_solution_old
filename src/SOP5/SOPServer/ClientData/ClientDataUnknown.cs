using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Collections;

namespace SDMSServer
{
    public class ClientDataUnknown : ClientData
    {
        public ClientDataUnknown(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.UNKNOWN;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.WHO_I_AM)
            {
                if (SetClientType(bytes, state))
                {

                    ClientData client = (ClientData)state.Tag;                      
                    if (client != null)
                    {
                        ConnectionLogEx.Instance.WriteLine("Set Client : " + SDMS.TCP_CLIENT.GetClientTypeString(client.ClientType)); //client.ClientType.ToString());
                        if (!ProcessFirstConnection(client, state))
                            return false;
                    }
                }

                return true;
            }

            return true;
        }

        private bool SetClientType(byte[] bytes, ConnectionState state)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            int nClientType = BitConverter.ToInt32(bytes, 11);

            if (nClientType <= (int)TCP_CLIENT.ALL || nClientType >= (int)TCP_CLIENT.UNKNOWN)
                return false;

            client = NewClientData((byte)nClientType);
            client.PingCount = 0;
            client.ConnectionState = state;
            

            // WhoIAm과 함께 전달되는 나머지 Parameter들을 client.ReceivedData에 저장하여
            // ProcessFirstConnection()에서 처리할 수 있도록 한다.
            if (bytes.Length > 15)
            {
                int len = bytes.Length - 15;
                client.ReceivedData = new byte[len];
                System.Buffer.BlockCopy(bytes, 15, client.ReceivedData, 0, len);
            }

            NetworkServer.Instance.UpdateClientType(state,client);
            state.Tag = client;

            return true;
        }

        private ClientData NewClientData(byte clientType)
        {
            // 다른 Connection정보가 할당될 수 있으므로 반드시 lock이 필요함
            // skkim 2014-03-27
            DdMonitor.Enter(m_provider.LockObject, true);
            ClientData result = null;
            {
                
                switch (clientType)
                {
                    case TCP_CLIENT.SDMS_CLIENT:
                        result = new ClientDataSDMS(this.m_provider);
                        break;
                    case TCP_CLIENT.SOP_SIMULATOR:
                        result = new ClientDataSOPSimulator(this.m_provider);
                        break;
                    case TCP_CLIENT.SENSOR_SIMULATOR:
                        result = new ClientDataSensorSimulator(this.m_provider);
                        break;
                    case TCP_CLIENT.SENSOR_MONITOR2:
                        result = new ClientDataSOPMonitor(this.m_provider);
                        break;
                    case TCP_CLIENT.SOP_RESTORE:
                        result = new ClientDataSOPRestore(this.m_provider);
                        break;
                    case TCP_CLIENT.INTEGRATE_MANAGE:
                        result = new ClientDataIntegrateManager(this.m_provider);
                        break;
                    case TCP_CLIENT.SDMS_CLIENT_SECOND:
                        result = new ClientDataSDMSSub(this.m_provider);
                        break;
                    case TCP_CLIENT.SERVER_COMMANDER:
                        result = new ClientdataServerCommander(this.m_provider);
                        break;
                    case TCP_CLIENT.TRAINING_SIMULATOR:
                        result = new ClientDataTrainingSimulator(this.m_provider);
                        break;
                    case TCP_CLIENT.SOP_WEATHER:
                        result = new ClientDataWeather(this.m_provider);
                        break;
                    case TCP_CLIENT.PSM_SENSOR_SERVER:
                        result = new ClientDataPSMMonitor(this.m_provider);
                        break;
                    case TCP_CLIENT.EARTHQUAKE_SENSOR_SERVER:
                        result = new ClientDataEarthquakeSensorServer(this.m_provider);
                        break;
                    case TCP_CLIENT.SVMS_EVENT_RECIVER:
                        result = new ClientDataSVMSEventReciver(this.m_provider);
                        break;
                    case TCP_CLIENT.ACCESS_EVENT_RECIVER:
                        result = new ClientDataS1AccessEventReciver(this.m_provider);
                        break;

                    case TCP_CLIENT.ASIN_EVENT_RECIVER:
                        result = new ClientDataAsinFireMonitor(this.m_provider);
                        break;
                    case TCP_CLIENT.SAINTOP_EVENT_RECIVER:
                        result = new ClientDataEMPollEventReciver(this.m_provider);
                        break;
                    case TCP_CLIENT.S1_TEST_SENSOR_SERVER:
                        result = new ClientDataS1SensorServer(this.m_provider);
                        break;
                    case TCP_CLIENT.S1_SECOM_EVENT_RECEIVER:
                        result = new ClientDataS1SecomServer(this.m_provider);
                        break;
                    case TCP_CLIENT.SOP_MANAGER:
                        result = new ClientDataSOPManager(this.m_provider);
                        break;
                }
               
            }
            DdMonitor.Exit(m_provider.LockObject, true);
            return result;
        }
    }
}
