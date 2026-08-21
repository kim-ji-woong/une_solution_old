using Microsoft.AspNetCore.Mvc;
using SafetyServer.BLL;
using SafetyServer.BLL.Data.Request;
using SafetyServer.BLL.Data.Response;
using SDMS.IDAL;
using dnsSopID;
using System.Collections;
using dnsData.Sensor;

namespace SafetyManager.Controllers
{
    using Model.Request;

    [Route("api/[controller]")]
    [ApiController]
    public class FireSensorController : ControllerBase
    {
        private MainManager m_mainManager = null;

        public FireSensorController(IDataManager dataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = new MainManager(dataManager, commonDataManager, teamDataManager);
        }

        [HttpPost]
        public IActionResult Post(SensorParameter param)
        {
            string strLog = string.Format("fieldID: {0}, userIDs : {1}",
                param.ClientInfo == null ? "null" : param.ClientInfo,
                param.Header,
                SafetyController.ListToString(param.Values));

            Logger.Instance.Write("Request from Client : SensorParameter, " + strLog);

            Parser parser = new Parser();
            ArrayList arrDatas = parser.ToArrayList(param.Values);

            if (arrDatas != null && arrDatas.Count > 0 && arrDatas[0] is int)
            {
                int nSensorType = (int)arrDatas[0];
                Result result = m_mainManager.SensorManager.OnReceive(Facility.ToFacilityType(nSensorType), param.Header, param.ClientInfo, arrDatas);
                return Ok(result);
            }
            else if (param.Header == Header.CLEAR_DETECT_ALL)
            {
                Result result = m_mainManager.SensorManager.OnReceive(Facility.FacilityType.FIRE_SENSOR, param.Header, param.ClientInfo, arrDatas);
                return Ok(result);
            }

            return Ok(false);
        }
    }
}
