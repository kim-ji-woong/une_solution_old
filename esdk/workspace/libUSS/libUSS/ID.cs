using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libUSS
{
    public class Header
    {
        public const short ARE_YOU_THERE = 1;
        public const short I_AM_HERE = 2;
        public const short REQUEST_SELECT_EVENT_TYPE = 3;
        public const short RESPONSE_SELECT_EVENT_TYPE = 4;
        public const short FIRE_SENSOR_DATA = 5;
        public const short POWER_OFF_DATA = 6;
        public const short EARTH_QUAKE_DATA = 7;
        public const short WIND_SENSOR_DATA = 8;
    }

    public class DataType
    {
        public const byte BYTE = 1;
        public const byte SHORT = 2;
        public const byte INTEGER = 3;
        public const byte LONG = 4;
        public const byte FLOAT = 5;
        public const byte DOUBLE = 6;
        // utf-8 encoding
        public const byte STRING = 7;
        public const byte DATETIME = 8;
        public const byte UNKNOWN = 9;
    }

    public class EventType
    {
        public const byte Fire = 1;
        public const byte PowerOff = 2;
        public const byte Earthquake = 3;
        public const byte Wind = 4;
    }

    public class EarthquakeDataType
    {
        // 진도
        public const byte Intensity = 0;
        // 규모
        public const byte Magnitue = 1;
        public const byte Gal = 2;
    }
}
