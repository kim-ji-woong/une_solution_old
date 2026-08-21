using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSOPPolicy.Common
{
    /// <summary>
    /// 센서신호를 받았을때 SOP를 실행할 것인지에 대한 옵션
    /// </summary>
    public class SOPonSensorDetect
    {
        /// <summary>
        /// SOP를 실행한다.
        /// </summary>
        public const int OpenNRun = 0;
        /// <summary>
        /// SOP를 실행하진 않고 열기만 한다.
        /// </summary>
        public const int JustOpen = 1;
        /// <summary>
        /// 아무것도 하지 않는다.
        /// </summary>
        public const int NotConcern = 2;

        public static int GetOption(int nOption)
        {
            if (nOption < OpenNRun || nOption > NotConcern)
                return NotConcern;

            return nOption;
        }
    }

    public interface ISensorZoneManager
    {
        UnE.Spatial.BuildingGroup GetBuildingGroup(int nBuildingGroupID);
        UnE.Spatial.Building GetBuilding(int nBuildingID);
        UnE.Spatial.Zone GetZone(int nZoneID);
        UnE.Spatial.EquipmentZone GetEquipmentZone(int nEquipmentZone);
    }
}
