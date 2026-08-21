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
    public class FireSensorController : ControllerBase
    {
        private SOPWebServer.BLL.MainManager m_mainManager = null;

        public FireSensorController(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = SOPWebServer.BLL.MainManager.GetMainManager(sdmsDataManager, commonDataManager, teamDataManager);
        }

        [HttpPost]
        public IActionResult Post(SensorParameter param)
        {
            Parser parser = new Parser();
            ArrayList arrDatas = parser.ToArrayList(param.Values);
            
            if (arrDatas != null && arrDatas.Count > 0 && arrDatas[0] is int)
            {
                int nSensorType = (int)arrDatas[0];
                Result result = m_mainManager.SensorManager.OnReceive(Facility.ToFacilityType(nSensorType), param.Header, param.ClientInfo, arrDatas);
                return Ok(result);
            }
            else if (param.Header == Header.CLEAR_DETECT_ALL) // 화재 신호 모두 복구
            {
                Result result = m_mainManager.SensorManager.OnReceive(Facility.FacilityType.FIRE_SENSOR, param.Header, param.ClientInfo, arrDatas);
                return Ok(result);
            }

            return Ok(false);
        }
    }
}
