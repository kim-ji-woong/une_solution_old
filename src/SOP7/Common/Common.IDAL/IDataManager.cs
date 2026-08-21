namespace Common.IDAL
{
    public interface IDataManager
    {
        int SiteID
        {
            get;
        }

        ISelect GetSelectManager();
        ICreate GetCreateManager();
        IDelete GetDeleteManager();
        IUpdate GetUpdateManager();
    }
}
