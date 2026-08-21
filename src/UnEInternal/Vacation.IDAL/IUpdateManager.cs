using System;
using System.Collections.Generic;

namespace Vacation.IDAL
{
    using Model;

    public interface IUpdateManager
    {
        bool UpdateRegularTeam(RegularTeam team, out string strErrorMessage);
        bool UpdateRegularTeam(Dictionary<RegularTeam.Fields, object> dicSets, Dictionary<RegularTeam.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateCompanyMember(CompanyMember member, out string strErrorMessage);
        bool UpdateCompanyMember(Dictionary<CompanyMember.Fields, object> dicSets, Dictionary<CompanyMember.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateJobLevel(JobLevel jobLevel, out string strErrorMessage);
        bool UpdateJobLevel(Dictionary<JobLevel.Fields, object> dicSets, Dictionary<JobLevel.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateHistory(History history, out string strErrorMessage);
        bool UpdateHistory(Dictionary<History.Fields, object> dicSets, Dictionary<History.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateRequest(Request request, out string strErrorMessage);
        bool UpdateRequest(Dictionary<Request.Fields, object> dicSets, Dictionary<Request.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateResponse(Response response, out string strErrorMessage);
        bool UpdateResponse(Dictionary<Response.Fields, object> dicSets, Dictionary<Response.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateSpecialVacationRequest(SpecialVacationRequest request, out string strErrorMessage);
        bool UpdateSpecialVacationRequest(Dictionary<SpecialVacationRequest.Fields, object> dicSets, Dictionary<SpecialVacationRequest.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateSpecialVacationResponse(SpecialVacationResponse response, out string strErrorMessage);
        bool UpdateSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicSets, Dictionary<SpecialVacationResponse.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateReservation(Reservation reservation, out string strErrorMessage);
        bool UpdateReservation(Dictionary<Reservation.Fields, object> dicSets, Dictionary<Reservation.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateSpecialVacation(SpecialVacation vacation, out string strErrorMessage);
        bool UpdateSpecialVacation(Dictionary<SpecialVacation.Fields, object> dicSets, Dictionary<SpecialVacation.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateOption(VacationOption option, out string strErrorMessage);
        bool UpdateOption(Dictionary<VacationOption.Fields, object> dicSets, Dictionary<VacationOption.Fields, object> dicConditions, out string strErrorMessage);
        bool UpdateExternalLogin(ExternalLogin login, out string strErrorMessage);
        bool UpdateExternalLogin(Dictionary<ExternalLogin.Fields, object> dicSets, Dictionary<ExternalLogin.Fields, object> dicConditions, out string strErrorMessage);
    }
}
