using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentFactory
{
    public class Factory
    {
        public enum AgentType { Fire, PSM, Security, Earthquake, SDMS, LogIn, TemperatureHumidity, SOPSimulator, SOPManager, SOPCommander, Etc };

        protected BaseProcessManager m_processMgr = null;
        protected BaseSMSManager m_smsMgr = null;
        protected BaseBroadcastManager m_broadcastMgr = null;
        protected ILogger m_logger = null;

        public BaseProcessManager ProcessManager
        {
            get { return GetProcessManager(); }
            set { SetProcessManager(value); }
        }

        public BaseSMSManager SMSManager
        {
            get { return GetSMSManager(); }
            set { SetSMSManager(value); }
        }

        public BaseBroadcastManager BroadcastManager
        {
            get { return GetBroadcastManager(); }
            set { SetBroadcastManager(value); }
        }

        public ILogger Logger
        {
            get { return GetLogger(); }
            set { SetLogger(value); }
        }

        public virtual BaseAgent MakeAgent(AgentType type)
        {
            return new DummyAgent();
        }

        public virtual BaseProcessAgent MakeProcessAgent()
        {
            return new BaseProcessAgent();
        }

        protected virtual BaseProcessManager GetProcessManager()
        {
            return m_processMgr;
        }

        protected virtual void SetProcessManager(BaseProcessManager mgr)
        {
            m_processMgr = mgr;
        }

        protected virtual BaseSMSManager GetSMSManager()
        {
            return m_smsMgr;
        }

        protected virtual void SetSMSManager(BaseSMSManager mgr)
        {
            m_smsMgr = mgr;
        }

        protected virtual BaseBroadcastManager GetBroadcastManager()
        {
            return m_broadcastMgr;
        }

        protected virtual void SetBroadcastManager(BaseBroadcastManager mgr)
        {
            m_broadcastMgr = mgr;
        }

        protected virtual ILogger GetLogger()
        {
            return m_logger;
        }

        protected virtual void SetLogger(ILogger logger)
        {
            m_logger = logger;
        }
    }

    internal class DummyAgent : BaseAgent
    {
        public override MethodProcessType CheckMethod(MethodType type, params object[] args)
        {
            return MethodProcessType.Default;
        }

        public override object RunMethod(MethodType type, params object[] args)
        {
            return null;
        }
    }
}
