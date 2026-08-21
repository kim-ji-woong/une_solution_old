using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.Model;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.IDAL
{
    public interface ICreate
    {
        bool AddRegular(Regular regular);
        bool AddRegular(Regular regular, out string strErrorMessage);
        bool AddRegular(int? nID, int? nParentTeamID, string strTeamName, out string strErrorMessage);
        bool AddRegularMember(RegularMember member);
        RegularMember AddRegularMember(RegularMember member, out string strErrorMessage);
        bool AddRegularMember(int? nID, string strEmail, int? nJobLevelID, int? nJobPositionID, string strMemberID, string strMemberName, string strOfficePhoneNumber, string strPhoneNumber, int nRegularID, int nStatusID, out string strErrorMessage);
        bool AddTemporary(Temporary temporary);
        bool AddTemporary(Temporary temporary, out string strErrorMessage);
        bool AddTemporary(int? nID, int? nParentTeamID, string strTeamName, bool bIsNormal, int nSiteID, out string strErrorMessage);
        bool AddTemporaryMember(TemporaryMember temporaryMember);
        bool AddTemporaryMember(TemporaryMember temporaryMember, out string strErrorMessage);
        bool AddTemporaryMember(int? nID, string strDisplaySOPName, int nTeamID, int? nRegularID, int? nRegularMemberID, int nIsNormal, int? nRole, out string strErrorMessage);
        bool AddOptions(Options options);
        bool AddOptions(Options options, out string strErrorMessage);
        bool AddOptions(int? nID, int nPropertyID, string strPropertyName, string strPropertyValue, out string strErrorMessage);




        bool AddRegularMemberList();
        bool AddTemporaryMemberList();
        bool AddFacilityManager();
        bool AddEquipZoneFacilityManager();
        bool AddBuildingFacilityManager();

    }
}
