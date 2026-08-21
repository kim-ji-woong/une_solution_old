using System.Collections.Generic;

namespace Common.IDAL
{
    using Model;
    using Model.Option;
    using Model.History;

    public interface IUpdate
    {
        // Option
        // strCondition : where를 제외한 조건문
        bool UpdateOption(Options.OptionTarget eTargetName, Options option, string strCondition = null);
        bool UpdateOption(Options.OptionTarget target, Dictionary<Options.Fields, object> dicSets, Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);

        // History
        // strCondition : where를 제외한 조건문
        bool UpdateActionStepHistory(ActionStepHistory actionStepHistory, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        bool UpdateComponentHistory(ComponentHistory componentStepHistory, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        bool UpdateComponentHistoryDetail(ComponentHistoryDetail componentHistoryDetail, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        bool UpdateActionStepAutoClose(ActionStepAutoClose actionStepAutoClose, string strCondition = null);
        bool UpdateShelter(Shelter shelter, string strCondition = null);
        bool UpdateSite(Site site, string strCondition = null);

        string GetErrorMessage();
    }
}
