using System.Collections.Generic;

namespace SOPManager.IDAL
{
    using Model.Sop.Account;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Config;

    /// <summary>
    /// strCondition : where를 제외한 조건문
    /// </summary>
    public interface IUpdate
    {
        // Sop.Account
        bool UpdateLevel(Level level, string strCondition = null);
        bool UpdateUser(User user, string strCondition = null);
        bool UpdateOption(Option user, string strCondition = null);
        bool UpdateSession(Session session, string strCondition = null);

        // Sop.Category
        bool UpdateDisasterCategory(DisasterCategory disasterCategory, string strCondition = null);
        bool UpdateSubDisasterCategory(SubDisasterCategory subDisasterCategory, string strCondition = null);
        bool UpdateDisaster(Disaster disaster, string strCondition = null);
        bool UpdateDisasterType(DisasterType disasterType, string strCondition = null);
        bool UpdateVersion(Version version, string strCondition = null);
        bool UpdateActionStep(ActionStep actionStep, string strCondition = null);

        // Sop.Component
        bool UpdateAnnotation(Annotation annotation, string strCondition = null);
        bool UpdateArrow(Arrow arrow, string strCondition = null);
        bool UpdateDecision(Decision decision, string strCondition = null);
        bool UpdateEndPoint(EndPoint endPoint, string strCondition = null);
        bool UpdateExternalProgram(ExternalProgram program, string strCondition = null);
        bool UpdateExternalProgramParameter(ExternalProgramParameter parameter, string strCondition = null);
        bool UpdateInternalTransmission(InternalTransmission internalTransmission, string strCondition = null);
        bool UpdateLink(Link link, string strCondition = null);
        bool UpdateProcess(Process process, string strCondition = null);
        bool UpdateProcessMission(ProcessMission processMission, string strCondition = null);
        bool UpdateProcessExternalMission(ProcessExternalMission processExternalMission, string strCondition = null);
        bool UpdateStepMember(StepMember stepMember, string strCondition = null);
        bool UpdateGridColumn(SectionGridColumn column, string strCondition = null);
        bool UpdateGridRow(SectionGridRow row, string strCondition = null);
        bool UpdateGrid(SectionGrid grid, string strCondition = null);
        bool UpdateSpecialMessage(SpecialMessage message, string strCondition = null);

        bool UpdateLinkedSop(LinkedSop linkedSop, string strCondition = null);

        string GetErrorMessage();
    }
}
