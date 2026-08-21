namespace SOPManager.IDAL
{
    public interface IDelete
    {
        // Sop.Account
        bool DeleteLevel(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteLevel(string strCondition);
        bool DeleteUser(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteUser(string strCondition);
        bool DeleteOption(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteOption(string strCondition);
        bool DeleteSession(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteSession(string strCondition);

        // Sop.Category
        bool DeleteDisasterCategory(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteDisasterCategory(string strCondition);
        bool DeleteSubDisasterCategory(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteSubDisasterCategory(string strCondition);
        bool DeleteDisaster(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteDisaster(string strCondition);
        bool DeleteDisasterType(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteDisasterType(string strCondition);
        bool DeleteVersion(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteVersion(string strCondition);
        bool DeleteActionStep(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteActionStep(string strCondition);

        // Sop.Component
        bool DeleteAnnotation(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteAnnotation(string strCondition);
        bool DeleteArrow(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteArrow(string strCondition);
        bool DeleteDecision(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteDecision(string strCondition);
        bool DeleteEndPoint(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteEndPoint(string strCondition);
        bool DeleteExternalProgram(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteExternalProgram(string strCondition);
        bool DeleteExternalProgramParameter(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteExternalProgramParameter(string strCondition);
        bool DeleteInternalTransmission(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteInternalTransmission(string strCondition);
        bool DeleteLink(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteLink(string strCondition);
        bool DeleteProcess(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteProcess(string strCondition);
        bool DeleteProcessMission(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteProcessMission(string strCondition);
        bool DeleteProcessExternalMission(int nProcessID, int nOrderIndex, int nProgramID, int nParameterIndex);
        // strCondition : where를 제외한 조건문
        bool DeleteProcessExternalMission(string strCondition);
        bool DeleteStepMember(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteStepMember(string strCondition);
        bool DeleteGridColumn(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteGridColumn(string strCondition);
        bool DeleteGridRow(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteGridRow(string strCondition);
        bool DeleteGrid(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteGrid(string strCondition);
        bool DeleteSpecialMessage(int id);
        // strCondition : where를 제외한 조건문
        bool DeleteSpecialMessage(string strCondition);


        bool DeleteLinkedSop(int id);
        bool DeleteLinkedSop(string strCondition);


        // strCondition : where를 제외한 조건문
        bool DeleteTable(string strTableName, string strCondition);

        string GetErrorMessage();
    }
}
