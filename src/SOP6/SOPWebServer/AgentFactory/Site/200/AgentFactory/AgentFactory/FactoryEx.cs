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
    }
}
