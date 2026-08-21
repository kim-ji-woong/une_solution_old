namespace Vacation.BLL.Rollback
{
    using IDAL;

    public interface IRollbackData
    {
        bool Rollback(IDataManager dataManager);
    }
}
