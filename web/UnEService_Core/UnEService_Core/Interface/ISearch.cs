using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core.Interface
{
    public interface ISearch
    {
        bool Search(string strURL, out List<string> files, out List<string> folders);
    }
}
