using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using UnE.Sensor;

namespace libSDMSReport
{
    public interface IReportOwner
    {
        //담당자 찾아옴
        string FindManagerName(Zone zone, IFacility.FacilityType facilityType);
    }
}
