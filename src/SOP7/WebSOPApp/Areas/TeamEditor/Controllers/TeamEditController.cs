using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using TeamEditor.BLL.Models.Request;
using TeamEditor.BLL.Models.Response;
using TeamEditor.Model.Sop.Team;


namespace WebSOPApp.Areas.TeamEditor.Controllers
{
    [Area("TeamEditor")]
    public class TeamEditController : Controller
    {
        private global::TeamEditor.BLL.ProcessManager m_processManager = null;
        public TeamEditController(global::TeamEditor.IDAL.IDataManager dataManager, global::Common.IDAL.IDataManager commonDataManager, global::SOPManager.IDAL.IDataManager sopDataManager, global::SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_processManager = new global::TeamEditor.BLL.ProcessManager(commonDataManager, dataManager, sopDataManager, sdmsDataManager);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data.RequestTemporaryMembers != null)
                return RequestTemporaryMembers();

            return BadRequest();
        }

        [HttpGet]
        public List<Regular> DisplayRegular()
        {
            string strErrorMessage;
            List<Regular> regulars = m_processManager.TeamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);
            if (regulars == null)
                return null;

            return regulars;
        }

        [HttpGet]
        public string DisplayRegularMember()
        {
            List<RegularMember> regularMembers = m_processManager.GetLoadManager().LoadRegularMember();
            
            return JsonConvert.SerializeObject(regularMembers);
        }

        [HttpPost]
        public List<Temporary> DisplayTemporary([FromBody] Temporary param)
        {
            bool bIsNormal = param.IsNormal;

            string strErrorMessage;
            Dictionary<Temporary.Fields, object> dicConditions = new Dictionary<Temporary.Fields, object>();
            dicConditions[Temporary.Fields.IsNormal] = bIsNormal;
            List<Temporary> temporaries = m_processManager.TeamDataManager.GetSelectManager().SelectTemporaries(dicConditions, out strErrorMessage);
            //List<Temporary> temporaries = m_processManager.TeamDataManager.GetSelectManager().GetTemporaries(bIsNormal, out strErrorMessage);
            if (temporaries == null)
                return null;

            return temporaries;
        }

        [HttpPost]
        public string DisplayTemporaryMember([FromBody] Temporary param)
        {
            int nID = param.ID;
            bool bIsNoraml = param.IsNormal;
            string strErrorMessage;

            List<RegularmemberTemporarymember> temporaryMembers =
                m_processManager.TeamDataManager.GetSelectManager().JoinRegularMemberTemporaryMember(nID, bIsNoraml, out strErrorMessage);

            return JsonConvert.SerializeObject(temporaryMembers);
        }

        public IActionResult RequestTemporaryMembers()
        {
            ResponseTemporaryMembers result = m_processManager.GetLoadManager().LoadTemporaryMembers();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveAddRegularTeam([FromBody] RequestCommandAddRegularTeam data)
        {
            ResponseCommand result = m_processManager.GetSaveManager().Save(data);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveRemoveRegularTeam([FromBody] RequestCommandRemoveRegularTeam data)
        {
            ResponseCommand result = m_processManager.GetSaveManager().Save(data);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveChangeRegularTeamInfo([FromBody] RequestCommandChangeRegularTeamInfo data)
        {
            ResponseCommand result = m_processManager.GetSaveManager().Save(data);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveChangeRegularMemeberInfo([FromBody] RequestCommandChangeRegularMemberInfo data)
        {
            ResponseCommand result = m_processManager.GetSaveManager().Save(data);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveRemoveRegularMemeber([FromBody] RequestCommandRemoveRegularMember data)
        {
            ResponseCommand result = m_processManager.GetSaveManager().Save(data);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult SaveUpdateData([FromBody] RequestSaveUpdateData data)
        {
            MessageResult result = m_processManager.GetSaveManager().SaveUpdateData(data);

            return Ok(result);
        }

        [HttpGet]
        public string GetJobLevels()
        {
            List<Options> options = m_processManager.GetLoadManager().LoadJobLevel();
            return JsonConvert.SerializeObject(options);
        }

        [HttpGet]
        public string GetJobPositions()
        {
            List<Options> options = m_processManager.GetLoadManager().LoadJobPosition();
            return JsonConvert.SerializeObject(options);
        }

        [HttpPost]
        public IActionResult UpdateRegularMember([FromBody] RequestUpdateRegularMember data)
        {
            ResponseUpdateRegularMember result = m_processManager.GetSaveManager().UpdateRegularMember(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult RemoveRegularMembers([FromBody] RequestRemoveRegularMember data)
        {
            MessageResult result = m_processManager.GetSaveManager().RemoveRegularMembers(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult UpdateTemporaryMember([FromBody] RequestUpdateTemporaryMember data)
        {
            ResponseUpdateTemporaryMember result = m_processManager.GetSaveManager().UpdateTemporaryMember(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult RemoveTemporaryMembers([FromBody] RequestRemoveTemporaryMember data)
        {
            MessageResult result = m_processManager.GetSaveManager().RemoveTemporaryMembers(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult UpdateRegularTeam([FromBody] RequestUpdateRegularTeam data)
        {
            ResponseUpdateRegularTeam result = m_processManager.GetSaveManager().UpdateRegularTeam(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult RemoveRegularTeams([FromBody] RequestRemoveRegularTeam data)
        {
            MessageResult result = m_processManager.GetSaveManager().RemoveRegularTeams(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult UpdateTemporaryTeam([FromBody] RequestUpdateTemporaryTeam data)
        {
            ResponseUpdateTemporaryTeam result = m_processManager.GetSaveManager().UpdateTemporaryTeam(data);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult RemoveTemporaryTeams([FromBody] RequestRemoveTemporaryTeam data)
        {
            MessageResult result = m_processManager.GetSaveManager().RemoveTemporaryTeams(data);
            return Ok(result);
        }
    }
}
