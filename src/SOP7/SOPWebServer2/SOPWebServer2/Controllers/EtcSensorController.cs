using Microsoft.AspNetCore.Mvc;
using dnsSopID;
using System.Collections;
using SOPWebServer.BLL.Response;
using dnsData.Sensor;

namespace SOPWebServer2.Controllers
{
    using Model.Request;

    [Route("api/[controller]")]
    [ApiController]
    public class EtcSensorController : ControllerBase
    {
        private SOPWebServer.BLL.MainManager m_mainManager = null;

        public EtcSensorController(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = SOPWebServer.BLL.MainManager.GetMainManager(sdmsDataManager, commonDataManager, teamDataManager);
        }

        [HttpPost]
        public IActionResult Post(SensorParameter param)
        {
            Parser parser = new Parser();
            ArrayList arrDatas = parser.ToArrayList(param.Values);

            Result result = m_mainManager.SensorManager.OnReceive(Facility.FacilityType.ETC, param.Header, param.ClientInfo, arrDatas);
            return Ok(result);
        }
    }
}
