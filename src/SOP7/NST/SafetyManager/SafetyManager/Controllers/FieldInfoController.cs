using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SafetyServer.BLL;
using SDMS.IDAL;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SafetyManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldInfoController : ControllerBase
    {
        private MainManager m_mainManager = null;

        public FieldInfoController(IDataManager dataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = new MainManager(dataManager, commonDataManager, teamDataManager);
        }

        [HttpGet("{id}")]
        public IEnumerable<string> Get(int id)
        {
            string[] imageCoords = m_mainManager.GetSpatialManager().GetZoneImageCoord(id);
            return imageCoords;
        }
        // GET: api/<ValuesController>
        /*[HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        // GET api/<ValuesController>/5
        /*[HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }*/
    }
}
