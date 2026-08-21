namespace SOPSimulator.IDAL
{
    public interface IDataManager
    {
        int SiteID
        {
            get;
        }

        ISelect GetSelectManager();
    }
}
