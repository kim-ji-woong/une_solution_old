using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaData.Models;

namespace MetaData.Controllers
{
    public class RegionTimeController : ApiController
    {
        // GetSensorDataList_region_time
        public IEnumerable<SensorData2> Post(RegionTime regionTime)
        {
            DateTime dtBegin, dtEnd;

            try
            {
                dtBegin = Convert.ToDateTime(regionTime.BeginTime);
                dtEnd = Convert.ToDateTime(regionTime.EndTime);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return SensorRepository.GetSensorDataList_region_time(regionTime.RegionID, dtBegin, dtEnd);
        }
    }
}
