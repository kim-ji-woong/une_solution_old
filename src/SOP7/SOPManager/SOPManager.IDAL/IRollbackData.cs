namespace SOPManager.IDAL
{
    public interface IRollbackData
    {
        void SetData(string strSQL);
        bool Rollback(IDataManager dataManager);
        // args : Insert 문의 따옴표 여부
        //        1이면 따옴표 필요
        bool AddInsertRollback(IDataManager dataManager, string strSelectSQL, params object[] args);
        bool AddDeleteRollback(string strDeleteSQL);
        bool AddUpdateRollback(string strUpdateSQL);
        bool AddUpdateRollback(IDataManager dataManager, string strSelectSQL, params object[] args);
    }
}
