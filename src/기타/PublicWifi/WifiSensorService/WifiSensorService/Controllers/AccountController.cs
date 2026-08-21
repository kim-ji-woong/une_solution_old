using Microsoft.AspNetCore.Mvc;
using dnsDBUtil;
using System.Web.Http.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WifiSensorService.Controllers
{
    using Data;
    using Data.Request;
    using Data.Response;

    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private WebDBManager m_dbMgr = null;

        public AccountController(Option option)
        {
            m_dbMgr = option.DBManager;
        }

        [EnableCors(origins: "UnEPolicy", headers: "*", methods: "*")]
        [HttpPost]
        public IActionResult RequestData([FromBody] RequestAccount data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestManagerList != null)
                return RequestManagerList();
            else if (data.CreateManager != null)
                return CreateManager(data.CreateManager);
            else if (data.UpdatePassword != null)
                return UpdatePassword(data.UpdatePassword);
            else if (data.RemoveManager != null)
                return RemoveManager(data.RemoveManager);

            return NotFound();
        }

        private IActionResult RequestManagerList()
        {
            ResponseManagerList response = AccountManager.GetManagerList(m_dbMgr);
            return Ok(response);
        }

        private IActionResult CreateManager(RequestCreateManager data)
        {
            MessageResult result = AccountManager.CreateManager(m_dbMgr, data);
            return Ok(result);
        }

        private IActionResult UpdatePassword(RequestUpdatePassword data)
        {
            MessageResult result = AccountManager.UpdatePassword(m_dbMgr, data);
            return Ok(result);
        }

        private IActionResult RemoveManager(RequestRemoveManager data)
        {
            MessageResult result = AccountManager.RemoveManager(m_dbMgr, data);
            return Ok(result);
        }
    }
}
