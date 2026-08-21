using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnE.Spatial;
using UnE.Sensor;

namespace S1SensorServer
{
    public class SensorZone : Object
    {
        private EquipmentZone m_Zone = null;

        public EquipmentZone EquipZone
        {
            get { return m_Zone; }
            set { m_Zone = value; }
        }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        private IFacility.FacilityType type = IFacility.FacilityType.FIRE_SENSOR;

        public IFacility.FacilityType Type
        {
            get { return type; }
            set { type = value; }
        }

        public override string ToString()
        {
            string strResult = "";

            if (type >= IFacility.FacilityType.FIRE_SENSOR && type <= IFacility.FacilityType.FireSensor_MonitoringType)
                strResult = "화재 탐지";
            else if (type == IFacility.FacilityType.COOLER_SENSOR)
                strResult = "소화 센서";
            else if (type == IFacility.FacilityType.PRESSURE_SENSOR)
                strResult = "압력 센서";
            else if (type == IFacility.FacilityType.PSM_SENSOR)
                strResult = "유해화학물질 누출감지 센서";

            return strResult;
        }
    }

    public class Circuit2 : Circuit
    {
        private SensorZone m_sensorZone = null;
        public SensorZone SensorZone
        {
            get { return m_sensorZone; }
            set { m_sensorZone = value; }
        }
    }

}
