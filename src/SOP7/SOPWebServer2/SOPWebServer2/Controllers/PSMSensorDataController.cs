using Microsoft.AspNetCore.Mvc;
using SOPWebServer.BLL.Response;

namespace SOPWebServer2.Controllers
{
    using Model.Request;

    [Route("api/[controller]")]
    [ApiController]
    public class PSMSensorDataController : ControllerBase
    {
        public PSMSensorDataController()
        {
        }

        [HttpPost]
        public IActionResult Post(PSMSensorDatas data)
        {
            MessageResult result = new MessageResult(true, "");

            string strLog = "[PSMSensorData] : " + data.Datas.Count.ToString();

            for (int i=0;i<data.Datas.Count;i++)
            {
                PSMSensorData sensorData = data.Datas[i];
                strLog += string.Format("\r\nSensorID({0}), Value({1:F2})", sensorData.SensorID, sensorData.SensorData);
            }

            System.Diagnostics.Trace.WriteLine(strLog + "\r\n\r\n");
            return Ok(result);
        }
    }
}
