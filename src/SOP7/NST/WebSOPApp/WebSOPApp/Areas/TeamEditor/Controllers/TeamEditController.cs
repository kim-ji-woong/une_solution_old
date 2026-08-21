using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamEditor.BLL.Models.Request;
using TeamEditor.BLL.Models.Response;
using TeamEditor.Model.Sop.Team;

namespace WebSOPApp.Areas.TeamEditor.Controllers
{
    [Area("TeamEditor")]
    public class TeamEditController : Controller
    {
        private global::TeamEditor.BLL.ProcessManager m_processManager = null;
        public TeamEditController(global::TeamEditor.IDAL.IDataManager dataManager, global::Common.IDAL.IDataManager commonDataManager, global::SOPManager.IDAL.IDataManager sopDataManager)
        {
            m_processManager = new global::TeamEditor.BLL.ProcessManager(commonDataManager, dataManager, sopDataManager);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data.RequestTemporaryMembers != null)
                return RequestTemporaryMembers();
            else if (data.RequestRegularMembers != null)
                return RequestRegularMembers();
            else if (data.RequestRegulars != null)
                return RequestRegulars();

            return BadRequest();
        }

        public IActionResult RequestTemporaryMembers()
        {
            ResponseTemporaryMembers result = m_processManager.GetLoadManager().LoadTemporaryMembers();

            return Ok(result);
        }

        public IActionResult RequestRegularMembers()
        {
            ResponseRegularMembers result = m_processManager.GetLoadManager().LoadRegularMembers();

            return Ok(result);
        }

        public IActionResult RequestRegulars()
        {
            ResponseRegulars result = m_processManager.GetLoadManager().LoadRegulars();

            return Ok(result);
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

            Dictionary<Temporary.Fields, object> dicConditions = new Dictionary<Temporary.Fields, object>();
            dicConditions[Temporary.Fields.IsNormal] = bIsNormal;

            string strErrorMessage;
            List<Temporary> temporaries = m_processManager.TeamDataManager.GetSelectManager().SelectTemporaries(dicConditions, out strErrorMessage);
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
    }
}
