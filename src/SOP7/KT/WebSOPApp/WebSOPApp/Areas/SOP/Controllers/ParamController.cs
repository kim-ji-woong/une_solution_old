using Microsoft.AspNetCore.Mvc;
using NipaSOP.BLL;
using NipaSOP.BLL.Models.Request;
using NipaSOP.BLL.Models.Response;
using NipaSOP.IDAL;
using System.Web.Http.Cors;

namespace WebSOPApp.Areas.SOP.Controllers
{
    [Area("SOP")]
    public class ParamController : Controller
    {
        private ProcessManager m_processManager = null;

        public ParamController(IDataManager dataManager, global::SOPManager.IDAL.IDataManager sopDataManager, Common.IDAL.IDataManager commonDataManager, global::TeamEditor.IDAL.IDataManager teamDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_processManager = new ProcessManager(dataManager, sopDataManager, commonDataManager, teamDataManager, sdmsDataManager);
        }

        [EnableCors(origins: "UnEPolicy", headers: "*", methods: "*")]
        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData request)
        {
            if (request == null)
                return BadRequest();

            if (request.StartInfo != null)
                return SetSopParameter(request.StartInfo);
            else if (request.RunSOP != null)
                return RunSOP(request.RunSOP);

            return BadRequest();
        }

        private IActionResult RunSOP(RunSOP data)
        {
            ResponseRunSOP response = m_processManager.RunSOP(data.BeginCode);
            return Ok(response);
        }

        private IActionResult SetSopParameter(StartInfo data)
        {
            ResponseStartInfo response = m_processManager.SetStartInfo(data);
            return Ok(response);
        }
    }
}
