using dnsData.Sensor;
using dnsData.Alarm;
using SDMS.Model.History;
using SDMS.Model.Config;

namespace AgentFactory.BLL
{
    public abstract class BaseBroadcastManager
    {
        protected Factory m_factory = null;

        public BaseBroadcastManager(Factory factory)
        {
            m_factory = factory;
        }

        public static bool IsFireSensor(Facility.FacilityType sensorType)
        {
            if (sensorType == Facility.FacilityType.FIRE_SENSOR)
                return true;
            else if (sensorType >= Facility.FacilityType.FireSensor_TypeA && sensorType <= Facility.FacilityType.FireSensor_MonitoringType)
                return true;
            else if (sensorType == Facility.FacilityType.Fire_S1 || sensorType == Facility.FacilityType.FireF1_S1 || sensorType == Facility.FacilityType.SecomFire)
                return true;

            return false;
        }

        public static bool IsPSMSensor(Facility.FacilityType sensorType)
        {
            if (sensorType == Facility.FacilityType.PSM_SENSOR ||
                sensorType == Facility.FacilityType.HF ||
                sensorType == Facility.FacilityType.CO ||
                sensorType == Facility.FacilityType.HCL ||
                sensorType == Facility.FacilityType.CH3C ||
                sensorType == Facility.FacilityType.N2H4 ||
                sensorType == Facility.FacilityType.CA ||
                sensorType == Facility.FacilityType.EA ||
                sensorType == Facility.FacilityType.VOC ||
                sensorType == Facility.FacilityType.H2O2 ||
                sensorType == Facility.FacilityType.THC ||
                sensorType == Facility.FacilityType.HNO3 ||
                sensorType == Facility.FacilityType.CL ||
                sensorType == Facility.FacilityType.TOLUENE ||
                sensorType == Facility.FacilityType.F2 ||
                sensorType == Facility.FacilityType.NH3 ||
                sensorType == Facility.FacilityType.LNG ||
                sensorType == Facility.FacilityType.PGMEA ||
                sensorType == Facility.FacilityType.H2S)
                return true;

            return false;
        }

        public static bool IsSecuritySensor(Facility.FacilityType sensorType)
        {
            if (sensorType >= Facility.FacilityType.Security_Sensor && sensorType <= Facility.FacilityType.VirtualFence_S1)
                return true;
            else if (sensorType >= Facility.FacilityType.EmergencyBell_S1 && sensorType <= Facility.FacilityType.VaultIntrusionT4_S1)
                return true;
            else if (sensorType >= Facility.FacilityType.CustomerEmergencyC1_S1 && sensorType <= Facility.FacilityType.ExternalAlarmBell)
                return true;
            else if (sensorType >= Facility.FacilityType.SecomExternalAlarmBell && sensorType <= Facility.FacilityType.SecomWomenAlarmBell)
                return true;

            return false;
        }

        public static bool IsEarthquakeSensor(Facility.FacilityType sensorType)
        {
            if (sensorType == Facility.FacilityType.Earthquake)
                return true;

            return false;
        }

        public static bool IsETCSensor(Facility.FacilityType sensorType)
        {
            if ((sensorType >= Facility.FacilityType.FIREWALL && sensorType <= Facility.FacilityType.ETC) ||
                sensorType == Facility.FacilityType.Temp ||
                sensorType == Facility.FacilityType.Humi ||
                sensorType == Facility.FacilityType.CO2 ||
                sensorType == Facility.FacilityType.TVOC ||
                sensorType == Facility.FacilityType.Dust_PM1 ||
                sensorType == Facility.FacilityType.Dust_PM2 ||
                sensorType == Facility.FacilityType.Dust_PM10 ||
                sensorType == Facility.FacilityType.AirPress ||
                sensorType == Facility.FacilityType.Inclin_X ||
                sensorType == Facility.FacilityType.Inclin_Y ||
                sensorType == Facility.FacilityType.Vib_X ||
                sensorType == Facility.FacilityType.Vib_Y ||
                sensorType == Facility.FacilityType.Vib_Z ||
                sensorType == Facility.FacilityType.Noise ||
                sensorType == Facility.FacilityType.BLE_Count ||
                sensorType == Facility.FacilityType.O2 ||
                sensorType == Facility.FacilityType.Value ||
                sensorType == Facility.FacilityType.mA ||
                sensorType == Facility.FacilityType.Contact ||
                sensorType == Facility.FacilityType.Relay)
                    return true;

            return false;
        }

        public static bool IsTemperatureHumiditySensor(Facility.FacilityType sensorType)
        {
            if (sensorType == Facility.FacilityType.TEMPERATURE_HUMIDITY)
                return true;

            return false;
        }

        public static Broadcast.SituationTypes ReactionTypeToSituationType(SensorReactionHistory.ReactionTypes reactionType, Facility.FacilityType sensorType)
        {
            if (reactionType == SensorReactionHistory.ReactionTypes.BEGIN_STATUS)
            {
                if (IsFireSensor(sensorType))
                    return Broadcast.SituationTypes.DETECT_FIRE;
                else if (IsPSMSensor(sensorType))
                    return Broadcast.SituationTypes.DETECT_PSM;
                else if (IsEarthquakeSensor(sensorType))
                    return Broadcast.SituationTypes.DETECT_EARTHQUAKE;
            }
            else if (reactionType == SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL)
            {
                if (IsFireSensor(sensorType))
                    return Broadcast.SituationTypes.REPORT_FIRE;
                else if (IsPSMSensor(sensorType))
                    return Broadcast.SituationTypes.REPORT_PSM;
            }

            return Broadcast.SituationTypes.Unknown;
        }

        // 상황에 맞는 방송문구를 만든다.
        // nRepeatCount : 0이면 한번만 방송한다. 0보다 크면 한번 이상 반복 방송한다.
        // Return 값 : null이거나 빈 문자열이면 방송하지 않는다.
        public abstract string GetBroadcastMessage(AlarmData alarm, Broadcast.SituationTypes type, out int nRepeatCount, out bool useSiren);
        public abstract bool RunBroadcast(string strMessage, int nRepeatCount, bool useSiren);
    }
}
