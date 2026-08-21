using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Vacation.BLL;
using Vacation.BLL.Models.Teams;
using Vacation.IDAL;
using Vacation.Model;

namespace UnEInternal.Controllers
{
    public class TeamsController : Controller
    {
        private ProcessManager m_processManager = null;
        private IDataManager m_dataManager = null;

        public TeamsController(IDataManager dataManager)
        {
            m_dataManager = dataManager;
            m_processManager = new ProcessManager(dataManager);
        }

        [HttpGet]
        public List<RegularTeam> DisplayRegular()
        {
            string strErrorMessage;
            List<RegularTeam> regulars = m_dataManager.GetSelectManager().SelectRegularTeams(null, out strErrorMessage);
            if (regulars == null)
                return null;

            TeamManager.RegularTeam = regulars;

            return regulars;
        }

        [HttpGet]
        public List<JobLevel> DisplayJobLevel()
        {
            string strErrorMessage;
            List<JobLevel> levels = m_dataManager.GetSelectManager().SelectJobLevels(null, out strErrorMessage);
            if (levels == null)
                return null;

            TeamManager.JobLevel = levels;

            return levels;
        }

        [HttpPost]
        public string DisplayRegularMember([FromBody] RegularTeam data)
        {
            List<CompanyMemberData> datas = m_processManager.GetTeamManager().LoadCompanyMember(data.ID);            
            return JsonConvert.SerializeObject(datas);
        }

        [HttpPost]
        public void Save([FromBody] CompanyMemberDataCollect data)
        {
            bool suc = m_processManager.GetTeamManager().SaveMember(data.data);
        }

        [HttpPost]
        public void DeleteMember([FromBody] CompanyMemberDataCollect data)
        {
            bool suc = m_processManager.GetTeamManager().DeleteMember(data.data);
        }

        [HttpPost]
        public void SaveTeam([FromBody] RegularTeam data)
        {
            if (data.ID > 0)
                m_processManager.GetTeamManager().UpdateRegularTeam(data);
            else
                m_dataManager.GetCreateManager().CreateRegularTeam(data.Name, data.ParentTeamID);
        }

        [HttpPost]
        public void DeleteTeam([FromBody] RegularTeamDataCollect data)
        {
            bool suc = m_processManager.GetTeamManager().DeleteTeam(data.data);
        }


        [HttpPost]
        public int CheckAdminLength([FromBody] RegularTeam data)
        {
            string strErrorMessage;
            int length = m_dataManager.GetSelectManager().SelectAdminLength(data.ID, out strErrorMessage);

            return length;
        }
    }    
}
