using System;

namespace SmartCity.Data
{
    public enum MessageType { NONE = 0, HEALTH, FLOOD, COLLAPSE, COLLAPSE_LEVEL, HEAT };

    public enum FacilityType { NONE = 0, FIRE_SENSOR = 1, FLOOD_SENSOR, HEAT_SENSOR, COLLAPSE_SENSOR };

    public class SensorData
    {
    }
}
