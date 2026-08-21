using System.Collections.Generic;

namespace Vacation.BLL.Rollback
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
            int nDataCount = m_datas.Count;

            for (int i = nDataCount - 1; i >= 0; i--)
            {
                IRollbackData data = m_datas[i];

                if (data.Rollback(dataManager) == false)
                    return false;
            }

            m_datas.Clear();
            return true;
        }
    }
}
