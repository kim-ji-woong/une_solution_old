using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentFactory
{
    public class BaseFactory
    {
        private static Factory m_factory = null;

        public static Factory GetFactory()
        {
            if (m_factory == null)
                m_factory = new Factory();

            return m_factory;
        }
    }
}
