using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.PSM
{
    public interface IPSMManager
    {
        // nSensorID : SensorZone ID가 아닌 Origin Sensor ID
        //             PSMSensor Table의 ID를 의미한다.
        PSMSensor GetSensor(int nSensorID);
    }
}
