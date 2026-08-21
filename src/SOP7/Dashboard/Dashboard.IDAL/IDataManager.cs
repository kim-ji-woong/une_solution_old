using System;

namespace Dashboard.IDAL
{
    public interface IDataManager
    {
        int SiteID
        {
            get;
        }

        ISelect GetSelectManager();
    }
}
