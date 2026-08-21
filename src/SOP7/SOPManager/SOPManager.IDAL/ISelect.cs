using System.Collections.Generic;
using System.Collections;

namespace SOPManager.IDAL
{
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Account;
    using Model.Sop.Config;
    using Common.Model.History;

    /// <summary>
    /// 쿼리가 성공하면 strErrorMessage가 null이 된다.
    /// strErrorMessage가 null이 아니면 뭔가 문제가 생긴 것이다.
    /// </summary>
    public interface ISelect
    {
        // Sop.Account
        Level SelectLevel(int id, out string strErrorMessage);
        List<Level> SelectLevels(Dictionary<Level.Fields, object> dicConditions, out string strErrorMessage);
        List<Level> SelectLevels(Dictionary<Level.Fields, object> dicConditions, string strAddtionalConditions, int? topNCount, out string strErrorMessage);
        User SelectUser(int id, out string strErrorMessage);
        List<User> SelectUsers(Dictionary<User.Fields, object> dicConditions, out string strErrorMessage);
        List<User> SelectUsers(Dictionary<User.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<User> SelectUsers(string strCondition, out string strErrorMessage);
        List<User> SelectUsers(string strCondition, int? topNCount, out string strErrorMessage);
        Option SelectOption(int id, out string strErrorMessage);
        List<Option> SelectOptions(Dictionary<Option.Fields, object> dicConditions, out string strErrorMessage);
        List<Option> SelectOptions(Dictionary<Option.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Session SelectSession(int id, out string strErrorMessage);
        List<Session> SelectSessions(Dictionary<Session.Fields, object> dicConditions, out string strErrorMessage);
        List<Session> SelectSessions(Dictionary<Session.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        // Sop.Category
        DisasterCategory SelectDisasterCategory(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<DisasterCategory> SelectDisasterCategories(string strCondition, out string strErrorMessage);
        List<DisasterCategory> SelectDisasterCategories(string strCondition, int? topNCount, out string strErrorMessage);
        List<DisasterCategory> SelectDisasterCategories(Dictionary<DisasterCategory.Fields, object> dicConditions, out string strErrorMessage);
        List<DisasterCategory> SelectDisasterCategories(Dictionary<DisasterCategory.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<DisasterCategory> SelectDisasterCategories(out string strErrorMessage);
        SubDisasterCategory SelectSubDisasterCategory(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<SubDisasterCategory> SelectSubDisasterCategories(string strCondition, out string strErrorMessage);
        List<SubDisasterCategory> SelectSubDisasterCategories(string strCondition, int? topNCount, out string strErrorMessage);
        List<SubDisasterCategory> SelectSubDisasterCategories(Dictionary<SubDisasterCategory.Fields, object> dicConditions, out string strErrorMessage);
        List<SubDisasterCategory> SelectSubDisasterCategories(Dictionary<SubDisasterCategory.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<SubDisasterCategory> SelectSubDisasterCategories(DisasterCategory disasterCategory, out string strErrorMessage);
        Disaster SelectDisaster(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Disaster> SelectDisasters(string strCondition, out string strErrorMessage);
        List<Disaster> SelectDisasters(string strCondition, int? topNCount, out string strErrorMessage);
        List<Disaster> SelectDisasters(Dictionary<Disaster.Fields, object> dicConditions, out string strErrorMessage);
        List<Disaster> SelectDisasters(Dictionary<Disaster.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        // Key : Disaster Name
        // Value : Disaster의 버전이 최신것부터 정렬
        Dictionary<string, List<Disaster>> SelectDisasters(SubDisasterCategory subDisasterCategory, bool isNormal, out string strErrorMessage);
        DisasterType SelectDisasterType(int id, out string strErrorMessage);
        // Value : Disaster의 버전이 최신것부터 정렬
        List<DisasterType> SelectDisasterTypes(string strCondition, out string strErrorMessage);
        List<DisasterType> SelectDisasterTypes(string strCondition, int? topNCount, out string strErrorMessage);
        List<DisasterType> SelectDisasterTypes(Dictionary<DisasterType.Fields, object> dicConditions, out string strErrorMessage);
        List<DisasterType> SelectDisasterTypes(Dictionary<DisasterType.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        Version SelectVersion(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Version> SelectVersions(string strCondition, out string strErrorMessage);
        List<Version> SelectVersions(string strCondition, int? topNCount, out string strErrorMessage);
        List<Version> SelectVersions(Dictionary<Version.Fields, object> dicConditions, out string strErrorMessage);
        List<Version> SelectVersions(Dictionary<Version.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        ActionStep SelectActionStep(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<ActionStep> SelectActionSteps(string strCondition, out string strErrorMessage);
        List<ActionStep> SelectActionSteps(string strCondition, int? topNCount, out string strErrorMessage);
        List<ActionStep> SelectActionSteps(Dictionary<ActionStep.Fields, object> dicConditions, out string strErrorMessage);
        List<ActionStep> SelectActionSteps(Dictionary<ActionStep.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<ActionStep> SelectActionSteps(Disaster disaster, out string strErrorMessage);

        // Sop.Component
        Annotation SelectAnnotation(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Annotation> SelectAnnotations(string strCondition, out string strErrorMessage);
        List<Annotation> SelectAnnotations(string strCondition, int? topNCount, out string strErrorMessage);
        List<Annotation> SelectAnnotations(Dictionary<Annotation.Fields, object> dicConditions, out string strErrorMessage);
        List<Annotation> SelectAnnotations(Dictionary<Annotation.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<Annotation> SelectAnnotations(int stepMemberID, out string strErrorMessage);
        Arrow SelectArrow(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Arrow> SelectArrows(string strCondition, out string strErrorMessage);
        List<Arrow> SelectArrows(string strCondition, int? topNCount, out string strErrorMessage);
        List<Arrow> SelectArrows(Dictionary<Arrow.Fields, object> dicConditions, out string strErrorMessage);
        List<Arrow> SelectArrows(Dictionary<Arrow.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<Arrow> SelectArrows(int stepMemberID, out string strErrorMessage);
        Decision SelectDecision(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Decision> SelectDecisions(string strCondition, out string strErrorMessage);
        List<Decision> SelectDecisions(string strCondition, int? topNCount, out string strErrorMessage);
        List<Decision> SelectDecisions(Dictionary<Decision.Fields, object> dicConditions, out string strErrorMessage);
        List<Decision> SelectDecisions(Dictionary<Decision.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<Decision> SelectDecisions(int stepMemberID, out string strErrorMessage);
        EndPoint SelectEndPoint(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<EndPoint> SelectEndPoints(string strCondition, out string strErrorMessage);
        List<EndPoint> SelectEndPoints(string strCondition, int? topNCount, out string strErrorMessage);
        List<EndPoint> SelectEndPoints(Dictionary<EndPoint.Fields, object> dicConditions, out string strErrorMessage);
        List<EndPoint> SelectEndPoints(Dictionary<EndPoint.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<EndPoint> SelectEndPoints(int stepMemberID, out string strErrorMessage);
        ExternalProgram SelectExternalProgram(int id, out string strErrorMessage);
        List<ExternalProgram> SelectExternalPrograms(string strCondition, out string strErrorMessage);
        List<ExternalProgram> SelectExternalPrograms(string strCondition, int? topNCount, out string strErrorMessage);
        List<ExternalProgram> SelectExternalPrograms(Dictionary<ExternalProgram.Fields, object> dicConditions, out string strErrorMessage);
        List<ExternalProgram> SelectExternalPrograms(Dictionary<ExternalProgram.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        ExternalProgramParameter SelectExternalProgramParameter(int nProgramID, int nParameterIndex, out string strErrorMessage);
        List<ExternalProgramParameter> SelectExternalProgramParameters(int nProgramID, out string strErrorMessage);
        List<ExternalProgramParameter> SelectExternalProgramParameters(string strCondition, out string strErrorMessage);
        List<ExternalProgramParameter> SelectExternalProgramParameters(string strCondition, int? topNCount, out string strErrorMessage);
        List<ExternalProgramParameter> SelectExternalProgramParameters(Dictionary<ExternalProgramParameter.Fields, object> dicConditions, out string strErrorMessage);
        List<ExternalProgramParameter> SelectExternalProgramParameters(Dictionary<ExternalProgramParameter.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        InternalTransmission SelectInternalTransmission(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<InternalTransmission> SelectInternalTransmissions(string strCondition, out string strErrorMessage);
        List<InternalTransmission> SelectInternalTransmissions(string strCondition, int? topNCount, out string strErrorMessage);
        List<InternalTransmission> SelectInternalTransmissions(Dictionary<InternalTransmission.Fields, object> dicConditions, out string strErrorMessage);
        List<InternalTransmission> SelectInternalTransmissions(Dictionary<InternalTransmission.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<InternalTransmission> SelectInternalTransmissions(int stepMemberID, out string strErrorMessage);
        Link SelectLink(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Link> SelectLinks(string strCondition, out string strErrorMessage);
        List<Link> SelectLinks(string strCondition, int? topNCount, out string strErrorMessage);
        List<Link> SelectLinks(Dictionary<Link.Fields, object> dicConditions, out string strErrorMessage);
        List<Link> SelectLinks(Dictionary<Link.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<Link> SelectLinks(int stepMemberID, out string strErrorMessage);
        Process SelectProcess(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<Process> SelectProcesses(string strCondition, out string strErrorMessage);
        List<Process> SelectProcesses(string strCondition, int? topNCount, out string strErrorMessage);
        List<Process> SelectProcesses(Dictionary<Process.Fields, object> dicConditions, out string strErrorMessage);
        List<Process> SelectProcesses(Dictionary<Process.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<Process> SelectProcesses(int stepMemberID, out string strErrorMessage);
        ProcessMission SelectProcessMission(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<ProcessMission> SelectProcessMissions(string strCondition, out string strErrorMessage);
        List<ProcessMission> SelectProcessMissions(string strCondition, int? topNCount, out string strErrorMessage);
        List<ProcessMission> SelectProcessMissions(Dictionary<ProcessMission.Fields, object> dicConditions, out string strErrorMessage);
        List<ProcessMission> SelectProcessMissions(Dictionary<ProcessMission.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<ProcessMission> SelectProcessMissions(List<int> processIDs, out string strErrorMessage);
        ProcessExternalMission SelectProcessExternalMission(int nProcessID, int nOrderIndex, int nProgramID, int nParameterIndex, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<ProcessExternalMission> SelectProcessExternalMissions(string strCondition, out string strErrorMessage);
        List<ProcessExternalMission> SelectProcessExternalMissions(string strCondition, int? topNCount, out string strErrorMessage);
        List<ProcessExternalMission> SelectProcessExternalMissions(Dictionary<ProcessExternalMission.Fields, object> dicConditions, out string strErrorMessage);
        List<ProcessExternalMission> SelectProcessExternalMissions(Dictionary<ProcessExternalMission.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<ProcessExternalMission> SelectProcessExternalMissions(List<int> processIDs, out string strErrorMessage);
        StepMember SelectStepMember(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<StepMember> SelectStepMembers(string strCondition, out string strErrorMessage);
        List<StepMember> SelectStepMembers(string strCondition, int? topNCount, out string strErrorMessage);
        List<StepMember> SelectStepMembers(Dictionary<StepMember.Fields, object> dicConditions, out string strErrorMessage);
        List<StepMember> SelectStepMembers(Dictionary<StepMember.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        List<StepMember> SelectStepMembers(ActionStep actionStep, out string strErrorMessage);
        //bool SelectStepMemberComponents(StepMember stepMember, List<Section> sections, List<Arrow> arrows, out string strErrorMessage);
        SectionGridColumn SelectGridColumn(int gridID, int columnIndex, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<SectionGridColumn> SelectGridColumns(string strCondition, out string strErrorMessage);
        List<SectionGridColumn> SelectGridColumns(Dictionary<SectionGridColumn.Fields, object> dicConditions, out string strErrorMessage);
        List<SectionGridColumn> SelectGridColumns(Dictionary<SectionGridColumn.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        SectionGridRow SelectGridRow(int gridID, int rowIndex, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<SectionGridRow> SelectGridRows(string strCondition, out string strErrorMessage);
        List<SectionGridRow> SelectGridRows(string strCondition, int? topNCount, out string strErrorMessage);
        List<SectionGridRow> SelectGridRows(Dictionary<SectionGridRow.Fields, object> dicConditions, out string strErrorMessage);
        List<SectionGridRow> SelectGridRows(Dictionary<SectionGridRow.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        SectionGrid SelectGrid(int id, out string strErrorMessage);
        // strCondition : where를 제외한 조건문
        List<SectionGrid> SelectGrids(string strCondition, out string strErrorMessage);
        List<SectionGrid> SelectGrids(string strCondition, int? topNCount, out string strErrorMessage);
        List<SectionGrid> SelectGrids(Dictionary<SectionGrid.Fields, object> dicConditions, out string strErrorMessage);
        List<SectionGrid> SelectGrids(Dictionary<SectionGrid.Fields, object> dicConditions, int? topNCount, out string strErrorMessage);
        LinkedSop SelectLinkedSop(int id, out string strErrorMessaage);
        List<LinkedSop> SelectLinkedSops(Dictionary<LinkedSop.Fields, object> dicConditions, out string strErrorMessage);
        List<LinkedSop> SelectLinkedSops(Dictionary<LinkedSop.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SpecialMessage SelectSpecialMessage(int id, out string strErrorMessage);
        List<SpecialMessage> SelectSpecialMessages(Dictionary<SpecialMessage.Fields, object> dicConditions, out string strErrorMessage);
        List<SpecialMessage> SelectSpecialMessages(Dictionary<SpecialMessage.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);


        // Join
        /// <summary>
        /// </summary>
        /// <returns>
        /// Disaster, User, Version의 각 객체가 순서대로 ArrayList에 담겨진다.
        /// 에러가 발생하면 null을 리턴한다.
        /// </returns>
        ArrayList JoinDisasterUserVersion(string strCondition, out string strErrorMessage);
        ArrayList JoinDisasterUserVersion(string strCondition, int? topNCount, out string strErrorMessage);

        /// <summary>
        /// 특정 Disaster에 연관된 모든 버전정보를 얻어온다.
        /// </summary>
        /// <param name="disasterID">disasterID를 가진 Disaster가 Key값</param>
        /// <param name="strErrorMessage"></param>
        /// <returns>
        /// Disaster, User, Version의 각 객체가 순서대로 ArrayList에 담겨진다.
        /// 에러가 발생하면 null을 리턴한다.
        /// </returns>
        ArrayList JoinDisasterUserVersion(int disasterID, out string strErrorMessage);
        /// <summary>
        /// versionID를 가진 Disaster와 같은 이름을 가진 데이터들을 얻어온다.
        /// </summary>
        /// <param name="versionID">versionID를 가진 Disaster와 같은 이름을 가진 데이터들을 얻어온다.</param>
        /// <param name="isNormal">쿼리조건</param>
        /// <returns>
        /// Disaster, User, Version의 각 객체가 순서대로 ArrayList에 담겨진다.
        /// 에러가 발생하면 null을 리턴한다.
        /// </returns>
        ArrayList JoinDisasterUserVersionFromVersion(int versionID, bool isNormal, out string strErrorMessage);
        /// <summary>
        /// DisasterCategory부터 SubDisasterCategory, Disaster, User, Version 정보를 얻어온다.
        /// </summary>
        /// <param name="versionID"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinDisasterCategorySubDisasterCategoryDisasterUserVersion(int versionID, out string strErrorMessage);
        /// <summary>
        /// DisasterCategory부터 SubDisasterCategory, Disaster, ActionStep 정보를 얻어온다.
        /// </summary>
        /// <param name="actionStepID"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinDisasterCategorySubDisasterCategoryDisasterActionStep(int actionStepID, out string strErrorMessage);

        // 기타
        // 현재 실행중인 SOP 버전인가?
        bool IsRunningVersion(int versionID, out string strErrorMessage);

        ArrayList SelectSOPHistory(
              Dictionary<DisasterCategory.Fields, object> dicConditions1
            , Dictionary<SubDisasterCategory.Fields, object> dicConditions2
            , Dictionary<Disaster.Fields, object> dicConditions3
            , Dictionary<ActionStep.Fields, object> dicConditions4
            , Dictionary<ActionStepHistory.Fields, object> dicConditions5
            , string strAdditionalConditions
            , out string strErrorMessage);
        ArrayList SelectSOPHistory(
              Dictionary<DisasterCategory.Fields, object> dicConditions1
            , Dictionary<SubDisasterCategory.Fields, object> dicConditions2
            , Dictionary<Disaster.Fields, object> dicConditions3
            , Dictionary<ActionStep.Fields, object> dicConditions4
            , Dictionary<ActionStepHistory.Fields, object> dicConditions5
            , string strAdditionalConditions
            , int? topNCount
            , out string strErrorMessage);
    }
}
