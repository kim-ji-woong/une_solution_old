using Dashboard.Model;
using System;
using System.Collections.Generic;

namespace Dashboard.IDAL
{
    public interface ISelect
    {
        CurrentWorkPermit SelectCurrentWorkPermit(string strPlantPrcsID, out string strErrorMessage);
        List<CurrentWorkPermit> SelectCurrentWorkPermits(Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<CurrentWorkPermit> SelectCurrentWorkPermits(Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);

    }
}
