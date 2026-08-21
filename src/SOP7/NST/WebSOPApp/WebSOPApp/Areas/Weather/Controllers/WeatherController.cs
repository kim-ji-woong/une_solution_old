using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weather.BLL;
using Weather.BLL.Models.Request;
using Weather.BLL.Models.Response;
using Weather.IDAL;

namespace WebSOPApp.Areas.Weather.Controllers
{
    [Area("Weather")]
    public class WeatherController : Controller
    {
        private ProcessManager m_processManager = null;
        public WeatherController(IDataManager dataManager)
        {
            m_processManager = new ProcessManager(dataManager);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestWeatherInfo != null)
                return RequestWeatherInfo();
            else if (data.RequestWeatherWeeklyInfo != null)
                return RequestWeatherWeeklyInfo();

            return null;
        }

        private IActionResult RequestWeatherInfo()
        {
            ResponseWeatherInfo result = m_processManager.GetLoadManager().GetWeatherInfo();
            return Ok(result);
        }

        private IActionResult RequestWeatherWeeklyInfo()
        {
            ResponseWeatherWeeklyInfo result = m_processManager.GetLoadManager().GetWeatherWeeklyInfo();
            return Ok(result);
        }
    }
}
