using System;
using System.Collections.Generic;

namespace Vacation.IDAL
{
    using Model;

    public interface ICreateManager
    {
        RegularTeam CreateRegularTeam(string name, int? parentID);
        CompanyMember CreateCompanyMember(string name, int jobLevelID, DateTime startDate, int teamID, bool isTeamLeader, bool isAdmin, string strUserID, string strPassword, string strPasswordCode, string strPhoneNumber);
        JobLevel CreateJobLevel(string strLevelName);
        History CreateHistory(int memberID, int year, float totalDays, float usedDays, float waitingDays, List<int> requestIDs, DateTime nextVacationDay);
        Request CreateRequest(DateTime requestTime, int memberID, List<Date> days, List<int> managerIDs, Response.ResponseType response, string strRequestDescription, int year, int? year2, DateTime? mailSendTime);
        Request CreateRequest(int id, DateTime requestTime, int memberID, List<Date> days, List<int> managerIDs, Response.ResponseType response, string strRequestDescription, int year, int? year2, DateTime? mailSendTime);
        Response CreateResponse(int requestID, int managerID, Response.ResponseType response, DateTime? responseTime, string strResponseDescription, int? prevResponseID);
        Response CreateResponse(int id, int requestID, int managerID, Response.ResponseType response, DateTime? responseTime, string strResponseDescription, int? prevResponseID);
        SpecialVacationRequest CreateSpecialVacationRequest(float days, DateTime requestTime, int requestManagerID, List<int> memberIDs, List<int> responseManagerIDs, Response.ResponseType response, string strRequestDescription);
        SpecialVacationResponse CreateSpecialVacationResponse(int requestID, int managerID, Response.ResponseType response, DateTime? responseTime, string strResponseDescription, int? prevResponseID);
        Reservation CreateReservation(int memberID, int requestID, List<Date> days, int year, int? year2);
        SpecialVacation CreateSpecialVacation(int memberID, float days, DateTime createTime, List<int> managerIDs, int requestID, string strDescription);
        VacationOption CreateOption(int id, string strPropertyName, string strPropertyValue, string strDescription);
        ExternalLogin CreateExternalLogin(string userID, long loginKey, DateTime loginTime, bool enabled, out string strErrorMessage);
    }
}
