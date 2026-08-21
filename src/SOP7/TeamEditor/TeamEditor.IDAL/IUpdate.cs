using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.Model;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.IDAL
{
    public interface IUpdate
    {
        bool UpdateRegular(Regular regular, out string strErrorMessage);
        bool UpdateRegular(Dictionary<Regular.Fields, object> dicSets, Dictionary<Regular.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateRegularMember(RegularMember member, out string strErrorMessage);
        bool UpdateRegularMember(Dictionary<RegularMember.Fields, object> dicSets, Dictionary<RegularMember.Fields, object> dicConditions, out string strErrorMessage);        
        bool UpdateTemporary(Temporary temporary, out string strErrorMessage);
        bool UpdateTemporary(Dictionary<Temporary.Fields, object> dicSets, Dictionary<Temporary.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateTemporaryMember(TemporaryMember temporaryMember, out string strErrorMessage);
        bool UpdateTemporaryMember(Dictionary<TemporaryMember.Fields, object> dicSets, Dictionary<TemporaryMember.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateSQL(string strSQL, out string strErrorMessage);
        bool UpdateOptions(Options options, out string strErrorMessage);
    }
}
