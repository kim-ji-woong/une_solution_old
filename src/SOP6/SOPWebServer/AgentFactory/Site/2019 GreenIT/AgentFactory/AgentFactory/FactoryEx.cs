using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AgentFactory
{
    public class FactoryEx : Factory
    {
        public override BaseProcessAgent MakeProcessAgent()
        {
            return new Agent.ProcessAgent();
        }
    }
}