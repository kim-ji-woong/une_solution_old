using System.Collections.Generic;

namespace Vacation.IDAL
{
    using Model;

    public interface IDeleteManager
    {
        bool DeleteRegularTeam(int id, out string strErrorMessage);
        bool DeleteRegularTeam(Dictionary<RegularTeam.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteCompanyMember(int id, out string strErrorMessage);
        bool DeleteCompanyMember(Dictionary<CompanyMember.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteJobLevel(int id, out string strErrorMessage);
        bool DeleteJobLevel(Dictionary<JobLevel.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteHistory(int memberID, int year, out string strErrorMessage);
        bool DeleteHistory(Dictionary<History.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteRequest(int id, out string strErrorMessage);
        bool DeleteRequest(Dictionary<Request.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteResponse(int id, out string strErrorMessage);
        bool DeleteResponse(Dictionary<Response.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteSpecialVacationRequest(int id, out string strErrorMessage);
        bool DeleteSpecialVacationRequest(Dictionary<SpecialVacationRequest.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteSpecialVacationResponse(int id, out string strErrorMessage);
        bool DeleteSpecialVacationResponse(Dictionary<SpecialVacationResponse.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteReservation(int id, out string strErrorMessage);
        bool DeleteReservation(Dictionary<Reservation.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteSpecialVacation(int id, out string strErrorMessage);
        bool DeleteSpecialVacation(Dictionary<SpecialVacation.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteOption(int id, out string strErrorMessage);
        bool DeleteOption(Dictionary<VacationOption.Fields, object> dicConditions, out string strErrorMessage);
        bool DeleteExternalLogin(string userID, out string strErrorMessage);
        bool DeleteExternalLogin(Dictionary<ExternalLogin.Fields, object> dicConditions, out string strErrorMessage);
    }
}
