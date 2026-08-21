using Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.IDAL
{
    public interface IDelete
    {
        bool DeleteCurrentWorkPermit(string strPlantPrcsID, out string strErrorMessage);
        bool DeleteCurrentWorkPermit(Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);

    }
}
