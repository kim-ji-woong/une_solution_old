namespace SDMS.BLL.Excel.Rollback
{
    public interface IRollbackData
    {
        bool Rollback(SDMS.IDAL.IDataManager sdmsDataManager, TeamEditor.IDAL.IDataManager teamDataManager);
    }
}
