using SmartCity.Model;
using System;

namespace SmartCity.IDAL
{
    public interface ICreateManager
    {
        AccountSession CreateAccountSession(int nAccountUserID, string strSessionKey, DateTime dtCreateDate, DateTime? dtUpdateDate);
    }
}
