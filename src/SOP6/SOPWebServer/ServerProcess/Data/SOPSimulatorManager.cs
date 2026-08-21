using AgentFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProcess.Data
{
    public static class SOPSimulatorManager
    {
        private static ISOPSimulatorServer m_server = null;

        public static ISOPSimulatorServer ServerInstance
        {
            get { return m_server; }
            set { m_server = value; }
        }
    }

    public interface ISOPSimulatorServer
    {
        void SendChangedConfig(int nConfigData);
        void SendClearAlarm(AlarmData alarm);
        void SendClientData(int header, byte[] bytes, IClientData client);
        void SendClientData(int header, byte[] bytes, int nClientType, int nClientSubType, IClientData exceptClient = null);
        void SendSensorSignal(AlarmData alarm, int nZoneID, int nOriginSensorID, float x = 0.0f, float y = 0.0f, float z = 0.0f);
        // 초기화가 완료되었는가?
        bool Initialized
        {
            get;
        }
        void DeleteActionStepHistory(List<int> actionStepHistoryIDs, List<int> actionStepIDs);
    }
}
