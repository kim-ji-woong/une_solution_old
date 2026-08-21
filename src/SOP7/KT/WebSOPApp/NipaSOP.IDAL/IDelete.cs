using System.Collections.Generic;
using NipaSOP.Model.Sop;

namespace NipaSOP.IDAL
{
    public interface IDelete
    {
        bool DeleteStartInfo(int id, out string strErrorMessage);
        bool DeleteStartInfo(Dictionary<StartInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteLocationLinkedSOP(int nFacilityID, out string strErrorMessage);
        bool DeleteLocationLinkedSOP(Dictionary<LocationLinkedSOP.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFacility(int id, out string strErrorMessage);
        bool DeleteFacility(Dictionary<Facility.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
