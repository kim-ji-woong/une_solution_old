using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using UnE.Sensor;

namespace AgentFactory
{
    public abstract class BaseBroadcastManager
    {
        public enum SituationType
        {
            Unknown = -1,
            DETECT_FIRE = 0,        // 화재 탐지
            REPORT_FIRE = 1,        // 화재 신고
            DETECT_PSM = 2,         // 누출 탐지
            REPORT_PSM = 3,         // 누출 신고
            DETECT_EARTHQUAKE = 4,  // 지진 탐지
            DETECT_SECURITY = 5,
            REPORT_SECURITY = 6,
            DETECT_TH = 7,
            REPORT_TH = 8,
            DETECT_ETC = 9,
            REPORT_ETC = 10
        }

        protected Factory m_factory = null;

        public BaseBroadcastManager(Factory factory)
        {
            m_factory = factory;
        }

        public static bool IsFireSensor(IFacility.FacilityType sensorType)
        {
            if (sensorType == IFacility.FacilityType.FIRE_SENSOR)
                return true;
            else if (sensorType >= IFacility.FacilityType.FireSensor_TypeA && sensorType <= IFacility.FacilityType.FireSensor_MonitoringType)
                return true;
            else if (sensorType == IFacility.FacilityType.Fire_S1 || sensorType == IFacility.FacilityType.FireF1_S1 || sensorType == IFacility.FacilityType.SecomFire)
                return true;

            return false;
        }

        public static bool IsPSMSensor(IFacility.FacilityType sensorType)
        {
            if (sensorType == IFacility.FacilityType.PSM_SENSOR)
                return true;
            
            return false;
        }

        public static bool IsSecuritySensor(IFacility.FacilityType sensorType)
        {
            if (sensorType >= IFacility.FacilityType.Security_Sensor && sensorType <= IFacility.FacilityType.VirtualFence_S1)
                return true;
            else if (sensorType >= IFacility.FacilityType.EmergencyBell_S1 && sensorType <= IFacility.FacilityType.VaultIntrusionT4_S1)
                return true;
            else if (sensorType >= IFacility.FacilityType.CustomerEmergencyC1_S1 && sensorType <= IFacility.FacilityType.ExternalAlarmBell)
                return true;
            else if (sensorType >= IFacility.FacilityType.SecomExternalAlarmBell && sensorType <= IFacility.FacilityType.SecomWomenAlarmBell)
                return true;

            return false;
        }

        public static bool IsEarthquakeSensor(IFacility.FacilityType sensorType)
        {
            if (sensorType == IFacility.FacilityType.Earthquake)
                return true;
            
            return false;
        }

        public static bool IsETCSensor(IFacility.FacilityType sensorType)
        {
            if (sensorType >= IFacility.FacilityType.FIREWALL && sensorType <= IFacility.FacilityType.TERROR)
                return true;

            return false;
        }

        public static bool IsTemperatureHumiditySensor(IFacility.FacilityType sensorType)
        {
            if (sensorType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
                return true;

            return false;
        }

        public static SituationType ReactionTypeToSituationType(BaseProcessManager.ReactionType reactionType, IFacility.FacilityType sensorType)
        {
            if (reactionType == BaseProcessManager.ReactionType.BEGIN_STATUS)
            {
                if (IsFireSensor(sensorType))
                    return SituationType.DETECT_FIRE;
                else if (IsPSMSensor(sensorType))
                    return SituationType.DETECT_PSM;
                else if (IsEarthquakeSensor(sensorType))
                    return SituationType.DETECT_EARTHQUAKE;
            }
            else if (reactionType == BaseProcessManager.ReactionType.NOTIFY_SIGNAL)
            {
                if (IsFireSensor(sensorType))
                    return SituationType.REPORT_FIRE;
                else if (IsPSMSensor(sensorType))
                    return SituationType.REPORT_PSM;
            }
            
            return SituationType.Unknown;
        }

        // 상황에 맞는 방송문구를 만든다.
        // nRepeatCount : 0이면 한번만 방송한다. 0보다 크면 한번 이상 반복 방송한다.
        // Return 값 : null이거나 빈 문자열이면 방송하지 않는다.
        public abstract string GetBroadcastMessage(DirectDBManager dbMgr, AlarmData alarm, SituationType type, out int nRepeatCount, out bool useSiren);
        public abstract bool RunBroadcast(DirectDBManager dbMgr, string strMessage, int nRepeatCount, bool useSiren);
    }
}
