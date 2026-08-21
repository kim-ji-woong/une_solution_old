using System.Collections.Generic;

namespace Common.IDAL
{
    using Model;
    using Model.Option;
    using Model.History;
    using System.Collections;
    using SOPManager.Model.Sop.Category;

    /// <summary>
    /// 쿼리가 성공하면 strErrorMessage가 null이 된다.
    /// strErrorMessage가 null이 아니면 뭔가 문제가 생긴 것이다.
    /// </summary>
    public interface ISelect
    {
        // Option
        Options SelectOption(Options.OptionTarget eTargetName, int id, out string strErrorMessage);
        List<Options> SelectOption(Options.OptionTarget eTargetName, string strPropertyName, out string strErrorMessage);
        List<Options> SelectOptions(Options.OptionTarget eTargetName, out string strErrorMessage);
        // topNCount가 null이 아닐 경우 전체 데이터를 받아오지 않고 topNCount 개수만큼만 리턴하도록 한다.
        List<Options> SelectOptions(Options.OptionTarget eTargetName, string strAdditionalCondition, int? topNCount, out string strErrorMessage);

        // History
        ActionStepHistory SelectActionStepHistory(int id, out string strErrorMessage);
        List<ActionStepHistory> SelectActionStepHistories(Dictionary<ActionStepHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<ActionStepHistory> SelectActionStepHistories(Dictionary<ActionStepHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        List<ActionStepHistory> SelectActionStepHistories(string strCondition, out string strErrorMessage);
        List<ActionStepHistory> SelectActionStepHistories(string strCondition, int? topNCount, out string strErrorMessage);
        ComponentHistory SelectComponentHistory(int id, out string strErrorMessage);
        List<ComponentHistory> SelectComponentHistories(string strCondition, out string strErrorMessage);
        List<ComponentHistory> SelectComponentHistories(string strCondition, int? topNCount, out string strErrorMessage);
        List<ComponentHistory> SelectComponentHistories(Dictionary<ComponentHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<ComponentHistory> SelectComponentHistories(Dictionary<ComponentHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ComponentHistoryDetail SelectComponentHistoryDetail(int id, out string strErrorMessage);
        List<ComponentHistoryDetail> SelectComponentHistoryDetails(string strCondition, out string strErrorMessage);
        List<ComponentHistoryDetail> SelectComponentHistoryDetails(string strCondition, int? topNCount, out string strErrorMessage);
        ActionStepAutoClose SelectActionStepAutoClose(int id, out string strErrorMessage);
        List<ActionStepAutoClose> SelectActionStepAutoCloses(string strCondition, out string strErrorMessage);
        List<ActionStepAutoClose> SelectActionStepAutoCloses(string strCondition, int? topNCount, out string strErrorMessage);
        Shelter SelectShelter(int id, out string strErrorMessage);
        List<Shelter> SelectShelters(string strCondition, out string strErrorMessage);
        List<Shelter> SelectShelters(string strCondition, int? topNCount, out string strErrorMessage);
        Site SelectSite(int id, out string strErrorMessage);
        List<Site> SelectSites(string strCondition, out string strErrorMessage);
        List<Site> SelectSites(string strCondition, int? topNCount, out string strErrorMessage);

        KakaoInfo SelectKakaoInfo(out string strErrorMessage);

        UserHistory SelectUserHistory(int id, out string strErrorMessage);
        List<UserHistory> SelectUserHistories(Dictionary<UserHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<UserHistory> SelectUserHistories(Dictionary<UserHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        string GetCurrentTime();

        /// <summary>
        /// SopHistoryActionStep, SopCategoryActionStep
        /// </summary>
        /// <param name="dicConditions1"></param>
        /// <param name="dicConditions2"></param>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinActionStepHistoryActionStep(Dictionary<ActionStepHistory.Fields, object> dicConditions1, Dictionary<ActionStep.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinActionStepHistoryActionStep(Dictionary<ActionStepHistory.Fields, object> dicConditions1, Dictionary<ActionStep.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
    }
}
