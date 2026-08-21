using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HSMS
{
    public interface IChangedDataManager
    {
        void SomethingChanged(ChangedData data);
        void RemoveData(ChangedData data);
        ArrayList GetDataList();
    }
}
