using System.Collections.Generic;

namespace ExcelWorker.Rollback
{
    public class RollbackManager
    {
        private List<IRollbackData> m_datas = new List<IRollbackData>();

        public void AddData(IRollbackData data)
        {
            m_datas.Add(data);
        }

        public bool Rollback(SDMS.IDAL.IDataManager sdmsDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            int nDataCount = m_datas.Count;

            for (int i=nDataCount-1;i>=0;i--)
            {
                IRollbackData data = m_datas[i];

                if (data.Rollback(sdmsDataManager, teamDataManager) == false)
                    return false;
            }

            m_datas.Clear();
            return true;
        }
    }
}
