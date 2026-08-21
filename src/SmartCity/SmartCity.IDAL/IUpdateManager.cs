using SmartCity.Model;
using System;
using System.Collections.Generic;

namespace SmartCity.IDAL
{
    public interface IUpdateManager
    {
        bool UpdateAccountSession(AccountSession accountSession, out string strErrorMessage);
        bool UpdateAccountSession(Dictionary<AccountSession.Fields, object> dicSets, Dictionary<AccountSession.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool UpdateAccountUser(AccountUser accountUser, out string strErrorMessage);
        bool UpdateAccountUser(Dictionary<AccountUser.Fields, object> dicSets, Dictionary<AccountUser.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);

    }
}
