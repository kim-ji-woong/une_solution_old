using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafetyServer.BLL;
using SafetyServer.BLL.Data.Request;
using SafetyServer.BLL.Data.Response;

namespace WebSOPApp.Areas.Safety.Controllers
{
    [Area("Safety")]
    public class SafetyController : Controller
    {
        private MainManager m_mainManager = null;

        public SafetyController(global::SDMS.IDAL.IDataManager dataManager, Common.IDAL.IDataManager commonDataManager, global::TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = new MainManager(dataManager, commonDataManager, teamDataManager);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestMobileUserList != null)
                return RequestMobileUserList();

            return NotFound();
        }

        private IActionResult RequestMobileUserList()
        {
            ResponseMobieUserList result = m_mainManager.GetSpatialManager().GetMobileUserList();
            return Ok(result);
        }
    }
}
