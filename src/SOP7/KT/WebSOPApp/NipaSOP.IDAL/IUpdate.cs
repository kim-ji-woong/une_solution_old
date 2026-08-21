using System.Collections.Generic;
using NipaSOP.Model.Sop;

namespace NipaSOP.IDAL
{
    public interface IUpdate
    {
        bool UpdateStartInfo(StartInfo startInfo, out string strErrorMessage);
        bool UpdateStartInfo(Dictionary<StartInfo.Fields, object> dicSets, Dictionary<StartInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool UpdateLocationLinkedSOP(LocationLinkedSOP sop, out string strErrorMessage);
        bool UpdateLocationLinkedSOP(Dictionary<LocationLinkedSOP.Fields, object> dicSets, Dictionary<LocationLinkedSOP.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool UpdateFacility(Facility facility, out string strErrorMessage);
        bool UpdateFacility(Dictionary<Facility.Fields, object> dicSets, Dictionary<Facility.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
