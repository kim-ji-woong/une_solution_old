using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.IDAL
{
    public interface IDataManager
    {
        int SiteID
        {
            get;
        }

        ICreate GetCreateManager();
        IDelete GetDeleteManager();
        IUpdate GetUpdateManager();
        ISelect GetSelectManager();
    }
}
