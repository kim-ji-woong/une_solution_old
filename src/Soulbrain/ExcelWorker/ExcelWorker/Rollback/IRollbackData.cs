namespace ExcelWorker.Rollback
{
    public interface IRollbackData
    {
        bool Rollback(SDMS.IDAL.IDataManager sdmsDataManager, TeamEditor.IDAL.IDataManager teamDataManager);
    }
}
