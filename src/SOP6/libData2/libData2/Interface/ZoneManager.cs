using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace UnE.Spatial
{
    public interface IZoneManager
    {
        EquipmentZone GetEquipZone(int nEquipZoneID);
        Zone GetZone(int nZoneID);
        int GetZoneCount();
        List<EquipmentZone> GetEquipmentZoneList(Zone zone);
    }
}
