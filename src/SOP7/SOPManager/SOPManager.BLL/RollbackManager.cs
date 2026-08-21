using System.Collections.Generic;
using System.Collections;

namespace SOPManager.BLL
{
    using IDAL;

    public class RollbackManager
    {
        private List<IRollbackData> m_datas = new List<IRollbackData>();

        public void AddData(IRollbackData data)
        {
            m_datas.Add(data);
        }

        public bool Rollback(IDataManager dataManager)
        {
            foreach (IRollbackData data in m_datas)
            {
                if (data.Rollback(dataManager) == false)
                    return false;
            }

            m_datas.Clear();
            return true;
        }
    }
}
