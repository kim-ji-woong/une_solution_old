using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaData.Models;

namespace MetaData.Controllers
{
    public class RectTimeController : ApiController
    {
        // GetSensorDataList_rect_time
        public IEnumerable<SensorData2> Post(RectTime rectTime)
        {
            DateTime dtBegin, dtEnd;

            try
            {
                dtBegin = Convert.ToDateTime(rectTime.BeginTime);
                dtEnd = Convert.ToDateTime(rectTime.EndTime);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return SensorRepository.GetSensorDataList_rect_time(rectTime.TLx, rectTime.TLy, rectTime.BLx, rectTime.BLy, rectTime.BRx, rectTime.BRy, dtBegin, dtEnd);
        }
    }
}
