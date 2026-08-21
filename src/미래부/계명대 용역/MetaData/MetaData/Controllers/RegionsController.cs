using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaData.Models;

namespace MetaData.Controllers
{
    public class RegionsController : ApiController
    {
        // AddRegion
        public HttpResponseMessage Post(Region region)
        {
            region.ID = SensorRepository.LastRegionID;
            SensorRepository.AddRegion(region);

            var response = Request.CreateResponse<Region>(System.Net.HttpStatusCode.Created, region);
            return response;
        }

        /*public IEnumerable<Region> GetAllRegions()
        {
            return new List<Region>(SensorRepository.Regions.Values);
        }*/

        // GetRegion
        public IHttpActionResult Get(int id)
        {
            Region region = SensorRepository.GetRegion(id);
            if (region == null)
                return NotFound();

            return Ok(region);
        }

        // RemoveRegion
        public void Delete(int id)
        {
            SensorRepository.RemoveRegion(id);
        }
    }
}
