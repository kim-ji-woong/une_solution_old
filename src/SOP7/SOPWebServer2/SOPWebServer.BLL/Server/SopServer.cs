using AgentFactory.BLL;
using dnsSopID;
using SOPWebServer.BLL.Models;
using SOPWebServer.BLL.Response;
using System.Collections;

namespace SOPWebServer.BLL.Server
{
    public class SopServer : BaseServer
    {
        private MainManager m_mainManager = null;

        public SopServer(MainManager mainManager, Factory factory)
        {
            m_mainManager = mainManager;
            m_agent = factory.MakeAgent(Factory.AgentType.SOPSimulator);
        }

        protected override void OnLoadEvent()
        {
            
        }

        protected override Result OnReceiveEvent(int header, string strClientInfo, ArrayList arrDatas)
        {
            if (header == Header.SITUATION_NOTICE)
                return ProcessSituationNotice(arrDatas);

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_COMMAND));
        }

        private Result ProcessSituationNotice(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];

                SensorZoneGroup group = m_mainManager.SensorManager.GetSensorZoneGroup(nSensorZoneID);
                if (group == null)
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_SENSOR_ID);

                if (group.CurrentAlarm == null)
                    return GetErrorMessageResult(ErrorMessageType.NO_SUCH_ALARM);

                if (group.BeginSituationNotice(m_mainManager.SDMSDataManager))
                    return new Result(true);
            }

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.INVALID_MESSAGE));
        }
    }
}
