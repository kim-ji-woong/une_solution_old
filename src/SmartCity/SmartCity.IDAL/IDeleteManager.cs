using SmartCity.Model;
using System;
using System.Collections.Generic;

namespace SmartCity.IDAL
{
    public interface IDeleteManager
    {
        bool DeleteAccountSession(int nID, out string strErrorMessage);
        bool DeleteAccountSession(Dictionary<AccountSession.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
