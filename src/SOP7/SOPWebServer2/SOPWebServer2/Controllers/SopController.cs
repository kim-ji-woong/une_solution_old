using dnsSopID;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SOPWebServer.BLL.Response;
using SOPWebServer2.Model.Request;
using System.Collections;

namespace SOPWebServer2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SopController : ControllerBase
    {
        private SOPWebServer.BLL.MainManager m_mainManager = null;
        public SopController(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = SOPWebServer.BLL.MainManager.GetMainManager(sdmsDataManager, commonDataManager, teamDataManager);
        }

        [HttpPost]
        public IActionResult Post(SensorParameter param)
        {
            Parser parser = new Parser();
            ArrayList arrDatas = parser.ToArrayList(param.Values);

            Result result = m_mainManager.SopManager.OnReceive(param.Header, param.ClientInfo, arrDatas);
            return Ok(false);
        }
    }
}
