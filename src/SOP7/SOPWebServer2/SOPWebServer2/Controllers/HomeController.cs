using Microsoft.AspNetCore.Mvc;

namespace SOPWebServer2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public HomeController(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            // MainManager 객체를 생성시켜 놓기 위하여 호출한다.
            SOPWebServer.BLL.MainManager.GetMainManager(sdmsDataManager, commonDataManager, teamDataManager);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("스마트 재난관리 시스템 서버가 작동중입니다.");
        }
    }
}
