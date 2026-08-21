namespace Common.IDAL
{
    using Model.Option;

    public interface IDelete
    {
        // Option
        bool DeleteOption(Options.OptionTarget eTargetName, int id);
        bool DeleteOption(Options.OptionTarget eTargetName, string strPropertyName);

        // History
        bool DeleteActionStepHistory(int id);
        bool DeleteActionStepHistory(string strCondition);
        bool DeleteComponentHistory(int id);
        bool DeleteComponentHistory(string strCondition);
        bool DeleteComponentHistoryDetail(int id);
        bool DeleteComponentHistoryDetail(string strCondition);
        bool DeleteActionStepAutoClose(int id);
        bool DeleteActionStepAutoClose(string strCondition);
        bool DeleteShelter(int id);
        bool DeleteShelter(string strCondition);
        bool DeleteSite(int id);
        bool DeleteSite(string strCondition);

        string GetErrorMessage();
    }
}
