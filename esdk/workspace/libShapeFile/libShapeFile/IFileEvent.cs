using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libShapeFile
{
    public interface IFileEventListener
    {
        void BeginReadFile(string szPath, string szType, int nCount);
        void ReadEntity(string szName, int nCount);
        void EndReadFile(string szPath, string szType);
    }
}
