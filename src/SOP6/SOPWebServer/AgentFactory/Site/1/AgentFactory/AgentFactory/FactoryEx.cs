using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AgentFactory
{
    public class FactoryEx : Factory
    {
        public override BaseAgent MakeAgent(AgentType type)
        {
            if (type == AgentType.PSM)
                return new Agent.PSMAgent();

            return base.MakeAgent(type);
        }
    }
}