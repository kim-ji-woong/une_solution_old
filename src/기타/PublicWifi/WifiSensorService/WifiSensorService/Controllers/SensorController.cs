using Microsoft.AspNetCore.Mvc;
using dnsDBUtil;
using System.Web.Http.Cors;

namespace WifiSensorService.Controllers
{
    using Data;
    using Data.Request;
    using Data.Response;

    [ApiController]
    [Route("[controller]")]
    public class SensorController : ControllerBase
    {
        private WebDBManager m_dbMgr = null;
        // 데이터 보존기한
        private Option m_option = null;

        public SensorController(Option option)
        {
            m_dbMgr = option.DBManager;
            m_option = option;
        }

        [HttpGet]
        public string Get()
        {
            return "공공 Wifi 센서데이터 수집서버입니다.";
        }

        [EnableCors(origins: "UnEPolicy", headers: "*", methods: "*")]
        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.SensorData != null)
                return SetSensorData(data.SensorData);
            else if (data.RequestSensorData != null)
                return RequestSensorData(data.RequestSensorData);
            else if (data.RequestSensorAvgData != null)
                return RequestSensorAvgData(data.RequestSensorAvgData);
            else if (data.RequestSensorAlarm != null)
                return RequestSensorAlarm(data.RequestSensorAlarm);

            return NotFound();
        }

        private IActionResult RequestSensorAvgData(RequestSensorAvgData request)
        {
            return Ok(SensorManager.GetSensorAvgData(request, m_dbMgr));
        }

        private IActionResult RequestSensorData(RequestSensorData request)
        {
            return Ok(SensorManager.GetSensorData(request, m_dbMgr));
        }

        private IActionResult SetSensorData(SensorData data)
        {
            return Ok(SensorManager.InsertSensorData(data, m_dbMgr, m_option.LifeTime, m_option.RebootMinutes, m_option.WarmingupMinutes));
        }

        private IActionResult RequestSensorAlarm(RequestSensorAlarm data)
        {
            ResponseSensorAlarmList response = SensorManager.GetAlarmList(m_dbMgr, data.ActiveOnly);
            return Ok(response);
        }
    }
}
