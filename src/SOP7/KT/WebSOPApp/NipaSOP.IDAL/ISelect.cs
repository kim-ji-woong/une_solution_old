using System.Collections.Generic;
using NipaSOP.Model.Sop;

namespace NipaSOP.IDAL
{
    public interface ISelect
    {
        StartInfo SelectStartInfo(int id, out string strErrorMessage);
        List<StartInfo> SelectStartInfos(Dictionary<StartInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        LocationLinkedSOP SelectLocationLinkedSOP(int nFacilityID, out string strErrorMessage);
        List<LocationLinkedSOP> SelectLocationLinkedSOPs(Dictionary<LocationLinkedSOP.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        Facility SelectFacility(int id, out string strErrorMessage);
        List<Facility> SelectFacilities(Dictionary<Facility.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
