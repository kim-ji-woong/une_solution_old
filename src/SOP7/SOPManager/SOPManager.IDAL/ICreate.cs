using System;
using System.Collections.Generic;

namespace SOPManager.IDAL
{
    using Model.Sop.Account;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Config;

    public interface ICreate
    {
        // Sop.Account
        Level CreateLevel(int? levelID, string strLevelName);
        User CreateUser(int? nMemberID, int nUserLevel, string strUserID, string strPassword, string strNickName, int nSiteID, string strPasswordCode = null);
        Option CreateOption(int nUserID, string strCategory, string strSubCategory, string strPropertyValue1, string strPropertyValue2, string strPropertyValue3, string strPropertyValue4);
        Session CreateSession(int nAccountUserID, string strSessionKey, DateTime dtCreateDate, DateTime dtUpdateeDate, bool autoLogin);

        // Sop.Category
        DisasterCategory CreateDisasterCategory(string strCategoryName, int nSiteID);
        SubDisasterCategory CreateSubDisasterCategory(int nDisasterCategoryID, string strSubCategoryName);
        Disaster CreateDisaster(string strDisasterName, int nSubDisasterCategoryID, int nVersionID, string strUserLevelIDs = null, string strDescription = null);
        DisasterType CreateDisasterType(string strTypeName, int nSubDisasterCategoryID);
        Version CreateVersion(bool isNormal, DateTime dtCreate, DateTime dtLastAcess, string strVersionName, int nOwnerID, int nSiteID, string strDescription = null);
        ActionStep CreateActionStep(string strStepName, int nDisasterID, int? nUserDefinedConfigID = null);

        // Sop.Component
        Annotation CreateAnnotation(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null);
        Arrow CreateArrow(int nBeginComponentID, int nBeginComponentPosition, int nEndComponentID, int nEndComponentPosition, int nStepMemberID, string strText = null);
        Decision CreateDecision(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, int nStepMemberID, int? nTeamID = null, int? nTeamType = null, int? nSectionNumber = null, string strDescription = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null, string strAutoRunScript = null, string strAutoRunScriptVariableTypes = null);
        EndPoint CreateEndPoint(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool isBegin, int nStepMemberID, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null);
        ExternalProgram CreateExternalProgram(string strExeName, string strDescription, string strInstallPath = null);
        ExternalProgramParameter CreateExternalProgramParameter(int nProgramID, int nParameterIndex, string strParameterName, int nValueType, bool isNullable);
        InternalTransmission CreateInternalTransmission(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, bool useSMS, bool useBroadcast, bool useEmail, int nStepMemberID, bool autoRun, string strMessage = null, List<Receiver> teamList = null, bool? useSiren = null, bool? onlyTeamLeader = null, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null);
        Link CreateLink(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, string strComponentID, string strLinkedComponentIDs, int nStepMemberID, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null);
        Process CreateProcess(int nGridID, int nGridRowIndex, int nGridColumnIndex, float fWidth, float fHeight, string strText, List<Receiver> teamList, string strComponentID, int nStepMemberID, bool autoRun, bool? onlyTeamLeader, int? nSectionNumber = null, int? nVAlign = null, int? nHAlign = null, string strFontName = null, int? nFontStyle = null, float? fFontSize = null, float? fLineSpace = null, int? nFontColor = null);
        ProcessMission CreateProcessMission(string strMissionText, int nProcessID);
        ProcessExternalMission CreateProcessExternalMission(int nProcessID, int nOrderIndex, int nProgramID, int nParameterIndex, string strValue = null);
        StepMember CreateStepMember(int nTeamID, int nTeamType, int nActionStepID);
        SectionGridColumn CreateGridColumn(int nGridID, int nColumnIndex, int nWidth);
        SectionGridRow CreateGridRow(int nGridID, int nRowIndex, int nHeight);
        SectionGrid CreateGrid(int nStepMemberID);
        SpecialMessage CreateSpecialMessage(string strCategory, string strMessage, string strDescription = null);

        LinkedSop CreateLinkedSop(int nFacilityTypeID, int nDisasterCategoryID, int nSubDisasterCategoryID, string strDisasterName, int? nLinkedBuildingID, int? nLinkedZoneID, string strDescription);



        string GetErrorMessage();
    }
}
