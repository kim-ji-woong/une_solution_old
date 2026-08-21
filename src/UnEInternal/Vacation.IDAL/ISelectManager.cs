using System;
using System.Collections.Generic;
using System.Collections;

namespace Vacation.IDAL
{
    using Model;

    public interface ISelectManager
    {
        RegularTeam SelectReqularTeam(int id, out string strErrorMessage);
        List<RegularTeam> SelectRegularTeams(Dictionary<RegularTeam.Fields, object> dicConditions, out string strErrorMessage);
        CompanyMember SelectCompanyMember(int id, out string strErrorMessage);
        List<CompanyMember> SelectCompanyMembers(Dictionary<CompanyMember.Fields, object>dicConditions, out string strErrorMessage);
        List<CompanyMember> SelectCompanyMembers(Dictionary<CompanyMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        JobLevel SelectJobLevel(int id, out string strErrorMessage);
        List<JobLevel> SelectJobLevels(Dictionary<JobLevel.Fields, object> dicConditions, out string strErrorMessage);
        ArrayList SelectCompanyMemberHistories(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<History.Fields, object> dicConditions2, out string strErrorMessage);
        ArrayList SelectCompanyMemberHistories(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<History.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        History SelectHistory(int memberID, int year, out string strErrorMessage);
        List<History> SelectHistories(Dictionary<History.Fields, object> dicConditions, out string strErrorMessage);
        List<History> SelectHistories(Dictionary<History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        int GetMinimumHistoryYear(List<int> memberIDs, out string strErrorMessage);
        Request SelectRequest(int id, out string strErrorMessage);
        List<Request> SelectRequests(Dictionary<Request.Fields, object> dicConditions, out string strErrorMessage);
        List<Request> SelectRequests(Dictionary<Request.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        Response SelectResponse(int id, out string strErrorMessage);
        List<Response> SelectResponse(Dictionary<Response.Fields, object> dicConditions, out string strErrorMessage);
        List<Response> SelectResponse(Dictionary<Response.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Response> SelectResponse(List<int> requestIDs, out string strErrorMessage);
        SpecialVacationRequest SelectSpecialVacationRequest(int id, out string strErrorMessage);
        List<SpecialVacationRequest> SelectSpecialVacationRequests(Dictionary<SpecialVacationRequest.Fields, object> dicConditions, out string strErrorMessage);
        List<SpecialVacationRequest> SelectSpecialVacationRequests(Dictionary<SpecialVacationRequest.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        SpecialVacationResponse SelectSpecialVacationResponse(int id, out string strErrorMessage);
        List<SpecialVacationResponse> SelectSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicConditions, out string strErrorMessage);
        List<SpecialVacationResponse> SelectSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SpecialVacationResponse> SelectSpecialVacationResponse(List<int> requestIDs, out string strErrorMessage);
        Reservation SelectReservation(int id, out string strErrorMessage);
        List<Reservation> SelectReservations(Dictionary<Reservation.Fields, object> dicConditions, out string strErrorMessage);
        SpecialVacation SelectSpecialVacation(int id, out string strErrorMessage);
        List<SpecialVacation> SelectSpecialVacations(Dictionary<SpecialVacation.Fields, object> dicConditions, out string strErrorMessage);
        List<SpecialVacation> SelectSpecialVacations(Dictionary<SpecialVacation.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SpecialVacation> SelectSpecialVacations(int year, out string strErrorMessage);
        List<SpecialVacation> SelectSpecialVacations(int memberID, int year, out string strErrorMessage);
        VacationOption SelectOption(int id, out string strErrorMessage);
        List<VacationOption> SelectOptions(List<string> propertyNames, out string strErrorMessage);
        OptionKakaoInfo SelectOptionKakaoInfo(out string strErrorMessage);
        int SelectAdminLength(int teamID, out string strErrorMessage);
        ExternalLogin SelectExternalLogin(string userID, out string strErrorMessage);
        List<ExternalLogin> SelectExternalLogins(Dictionary<ExternalLogin.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList SelectCompanyMemberJobLevelRegularTeam(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<JobLevel.Fields, object> dicConditions2, Dictionary<RegularTeam.Fields, object> dicConditions3, out string strErrorMessage);
        ArrayList SelectCompanyMemberJobLevelRegularTeam(Dictionary<CompanyMember.Fields, object> dicConditions1, Dictionary<JobLevel.Fields, object> dicConditions2, Dictionary<RegularTeam.Fields, object> dicConditions3, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinCompanyMemberRequest(string strAdditionalConditions, out string strErrorMessage);
    }
}
