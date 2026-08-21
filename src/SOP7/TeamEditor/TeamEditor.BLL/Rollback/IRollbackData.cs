using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL.Rollback
{
    public interface IRollbackData
    {
        bool Rollback(SDMS.IDAL.IDataManager sdmsDataManager, TeamEditor.IDAL.IDataManager teamDataManager, SOPManager.IDAL.IDataManager sopDataManager);
    }
}
