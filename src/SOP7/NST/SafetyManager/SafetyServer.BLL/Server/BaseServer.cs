using System.Collections;
using System.Collections.Generic;
using AgentFactory.BLL;
using dnsSopID;

namespace SafetyServer.BLL.Server
{
    using Data.Response;

    public abstract class BaseServer
    {
        protected BaseAgent m_agent = null;
        protected Factory m_agentFactory = null;

        public BaseServer()
        {
        }

        public BaseServer(Factory factory)
        {
            m_agentFactory = factory;
        }

        public void SetAgentFactory(Factory agentFactory)
        {
            m_agentFactory = agentFactory;
        }

        public void OnLoad(SDMS.IDAL.IDataManager dataManager)
        {
            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.OnLoad, null);

            if (processType == BaseAgent.MethodProcessType.Default)
                OnLoadEvent();
            else if (processType == BaseAgent.MethodProcessType.FactoryOnly)
                m_agent.RunMethod(BaseAgent.MethodType.OnLoad, dataManager);
            else if (processType == BaseAgent.MethodProcessType.PostProcess)
            {
                OnLoadEvent();
                m_agent.RunMethod(BaseAgent.MethodType.OnLoad, dataManager);
            }
            else if (processType == BaseAgent.MethodProcessType.PreProcess)
            {
                m_agent.RunMethod(BaseAgent.MethodType.OnLoad, dataManager);
                OnLoadEvent();
            }
        }

        public Result OnReceive(int header, string strClientInfo, ArrayList arrDatas)
        {
            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.OnReceive, header);

            if (processType == BaseAgent.MethodProcessType.Default)
                return OnReceiveEvent(header, strClientInfo, arrDatas);
            else if (processType == BaseAgent.MethodProcessType.FactoryOnly)
            {
                object result = m_agent.RunMethod(BaseAgent.MethodType.OnReceive, header, strClientInfo, arrDatas);

                if (result != null && result is int)
                {
                    int nResult = (int)result;

                    if (nResult == ErrorMessageType.SUCCESS)
                        return new Result(true);
                    else
                        return GetErrorMessageResult(nResult);
                }
                else
                    return GetErrorMessageResult(ErrorMessageType.UNKNOWN_COMMAND);
            }
            else if (processType == BaseAgent.MethodProcessType.PostProcess)
            {
                Result _result = OnReceiveEvent(header, strClientInfo, arrDatas);
                object result = m_agent.RunMethod(BaseAgent.MethodType.OnReceive, header, strClientInfo, arrDatas);

                if (result != null && result is int)
                {
                    int nResult = (int)result;

                    if (nResult == ErrorMessageType.SUCCESS)
                        return new Result(true);
                    else
                        return GetErrorMessageResult(nResult);
                }
                else
                    return _result;
            }
            else if (processType == BaseAgent.MethodProcessType.PreProcess)
            {
                m_agent.RunMethod(BaseAgent.MethodType.OnReceive, header, strClientInfo, arrDatas);
                return OnReceiveEvent(header, strClientInfo, arrDatas);
            }

            return GetErrorMessageResult(ErrorMessageType.UNKNOWN_HEADER);
        }

        protected void WriteLog(string strLog)
        {
            Logger.Instance.Write(strLog);
        }

        protected MessageResult GetErrorMessageResult(int error)
        {
            return new MessageResult(false, ErrorMessageType.ToMessage(error));
        }

        protected abstract void OnLoadEvent();
        protected abstract Result OnReceiveEvent(int header, string strClientInfo, ArrayList arrDatas);
    }
}
