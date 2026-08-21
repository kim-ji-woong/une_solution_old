using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Vacation.Model;
using Vacation.BLL;
using Vacation.IDAL;
using Vacation.BLL.Models.Account;
using Vacation.BLL.Models.Vacation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UnEInternal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationController : ControllerBase
    {
        private IConfiguration m_config = null;
        private ProcessManager m_processManager = null;

        public VacationController(IDataManager dataManager, IConfiguration config)
        {
            m_config = config;
            m_processManager = new ProcessManager(dataManager);
        }

        // POST api/<VacationController>
        [HttpPost]
        public IActionResult Post([FromBody] VacationData data)
        {
            if (data.RequestManager != null)
                return RequestManager(data.RequestManager);
            else if (data.RequestSpecialVacationManager != null)
                return RequestSpecialVacationManager(data.RequestSpecialVacationManager);
            else if (data.RequestHistory != null)
                return RequestHistory(data.RequestHistory);
            else if (data.RequestVacation != null)
                return RequestVacation(data.RequestVacation);
            else if (data.RequestSpecialVacation != null)
                return RequestSpecialVacation(data.RequestSpecialVacation);
            else if (data.RequestManagerData != null)
                return RequestManagerData(data.RequestManagerData);
            else if (data.ProcessRequest != null)
                return ProcessRequest(data.ProcessRequest);
            else if (data.RequestMemberHistory != null)
                return RequestMemberHistory(data.RequestMemberHistory);
            else if (data.RequestVacationList != null)
                return RequestVacationList(data.RequestVacationList);
            else if (data.RequestCancelVacations != null)
                return CancelVacations(data.RequestCancelVacations);
            else if (data.RequestHolidays != null)
                return RequestHolidays(data.RequestHolidays);

            return null;
        }

        private IActionResult RequestHolidays(RequestHolidays data)
        {
            ResponseHolidays response = data.Month == null ? HolidayManager.GetHolidays(data.Year, Startup.HolidayApiUrl, Startup.HolidayApiLicenseKey, Startup.CustomHolidays) : HolidayManager.GetHolidays(data.Year, (int)data.Month, Startup.HolidayApiUrl, Startup.HolidayApiLicenseKey, Startup.CustomHolidays);
            return Ok(response);
        }

        private IActionResult CancelVacations(RequestCancelVacations data)
        {
            ResponseCancelVacations result = Vacation.BLL.RequestManager.CancelVacations(m_processManager.GetDataManager(), data.RequestIDs, m_processManager.GetVacationManager());
            return Ok(result);
        }

        private IActionResult RequestVacationList(RequestVacationList data)
        {
            ResponseVacationList response = m_processManager.GetVacationManager().GetVacationList(data.UserID, data.Year);
            return Ok(response);
        }

        private IActionResult RequestHistory(_RequestHistory data)
        {
            Vacation.BLL.Models.Vacation.History history = m_processManager.GetVacationManager().GetVacationHistory(data.UserID, data.Year, data.Month, data.Day);
            return Ok(history);
        }

        private IActionResult RequestManager(_RequestManager data)
        {
            RequestManagerResult result = m_processManager.GetVacationManager().GetManager(data.UserID, data.GetDateList());
            return Ok(result);
        }

        private IActionResult RequestSpecialVacationManager(_RequestManager data)
        {
            RequestManagerResult result = m_processManager.GetVacationManager().GetSpecialVacationManager(data.UserID);
            return Ok(result);
        }

        private IActionResult RequestVacation(_RequestVacation data)
        {
            RequestVacationResult result = m_processManager.GetVacationManager().RequestVacation(data.UserID, data.GetDateList(), data.Description);
            return Ok(result);
        }

        private IActionResult RequestSpecialVacation(_RequestSpecialVacation data)
        {
            RequestSpecialVacationResult result = m_processManager.GetVacationManager().RequestSpecialVacation(data.RequestManagerID, data.UserIDs, data.Days, data.Reason);
            return Ok(result);
        }

        private IActionResult RequestManagerData(_RequestManagerData data)
        {
            ManagerRequestData result = m_processManager.GetVacationManager().RequestManagerData(data.ManagerUserID, data.Year);
            return Ok(result);
        }

        private IActionResult ProcessRequest(_ProcessRequest data)
        {
            ProcessRequestResult result = m_processManager.GetVacationManager().ProcessRequest(data.RequestID, data.IsPermit, data.ManagerUserID, data.ManagerDescription, data.IsNormal);
            return Ok(result);
        }

        private IActionResult RequestMemberHistory(_RequestMemberHistory data)
        {
            MemberVacationHistory result = m_processManager.GetVacationManager().GetMemberVacationHistory(data.ManagerUserID);
            return Ok(result);
        }
    }
}
