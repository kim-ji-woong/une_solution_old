using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory.Agent;

namespace AgentFactory
{
    public class FactoryEx : Factory
    {
        public override BaseProcessAgent MakeProcessAgent()
        {
            return new ProcessAgent();
        }

        public override BaseAgent MakeAgent(AgentType type)
        {
            if (type == AgentType.SDMS)
                return new SDMSAgent();
            else if (type == AgentType.SOPSimulator)
                return new SOPSimulatorAgent();

            return new DummyAgent();
        }
    }
}
