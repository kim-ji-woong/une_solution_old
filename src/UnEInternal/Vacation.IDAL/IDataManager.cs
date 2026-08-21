using System;

namespace Vacation.IDAL
{
    public interface IDataManager
    {
        ICreateManager GetCreateManager();
        ISelectManager GetSelectManager();
        IDeleteManager GetDeleteManager();
        IUpdateManager GetUpdateManager();
        string Encrypt(string input);
        string Decrypt(string input);
    }
}
