using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentFactory
{
    public interface ILogger
    {
        void Write(string strLog);
        ILogger Clone(string strTag);
        void Close();
    }
}
