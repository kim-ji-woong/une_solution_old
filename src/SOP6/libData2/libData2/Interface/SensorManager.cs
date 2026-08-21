using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;

namespace UnE.Sensor
{
    public interface ISensorManager
    {
        // equipZoneID를 이용하여 EquipmentZone을 찾은 다음, 그 Zone에서 sensorType에 해당하는 Sensor들을 얻어온다.
        List<ISensor> FindZoneInSensor(int equipZoneID, IFacility.FacilityType sensorType);
        //equipZoneID를 이용하여 EquipmentZone을 찾은 다음, 그 Zone에서 sensorTypes중 하나에 해당하는 Sensor들을 얻어온다.
        List<ISensor> FindZoneInSensor(int equipZoneID, List<IFacility.FacilityType> sensorTypes);
    }
}
