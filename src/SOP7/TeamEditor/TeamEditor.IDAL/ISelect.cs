using System;
using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.IDAL
{
    public interface ISelect
    {
        string ReadSiteName();
        Regular SelectRegular(int id, out string strErrorMessage);
        List<Regular> SelectRegulars(out string strErrorMessage);
        List<Regular> SelectRegulars(Dictionary<Regular.Fields, object> dicConditions, out string strErrorMessage);
        List<Regular> SelectRegulars(Dictionary<Regular.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Regular> SelectRegulars(Dictionary<Regular.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        List<RegularMember> SelectRegularMembers(out string strErrorMessage);
        List<RegularMember> SelectRegularMembers(Dictionary<RegularMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<RegularMember> SelectRegularMembers(Dictionary<RegularMember.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        RegularMember SelectRegularMember(int nID, out string strErrorMessage);
        List<RegularMember> SelectRegularMembers(string strCondition, out string strErrorMessage);
        int GetMaxID(string strTableName, out string strErrorMessage, string strCondition = "");
        Options SelectOptions(int nID, out string strErrorMessage);
        List<Options> SelectOptions(Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Options> SelectOptions(Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        List<Options> SelectOptions(out string strErrorMessage);
        List<Options> SelectOptions(string strCondition, out string strErrorMessage);

        Temporary SelectTemporary(int id, out string strErrorMessage);
        List<Temporary> SelectTemporaries(Dictionary<Temporary.Fields, object> dicConditions, out string strErrorMessage);
        List<Temporary> SelectTemporaries(Dictionary<Temporary.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        TemporaryMember SelectTemporaryMember(int id, out string strErrorMessage);
        List<TemporaryMember> SelectTemporaryMembers(Dictionary<TemporaryMember.Fields, object> dicConditions, out string strErrorMessage);
        List<TemporaryMember> SelectTemporaryMembers(Dictionary<TemporaryMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<TemporaryMember> SelectTemporaryMembers(Dictionary<TemporaryMember.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        List<RegularmemberTemporarymember> JoinRegularMemberTemporaryMember(int temporaryID, bool isNormal, out string strErrorMessage);
    }
}
