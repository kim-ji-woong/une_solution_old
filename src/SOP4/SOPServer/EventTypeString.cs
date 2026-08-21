using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;

namespace SOPServer
{
    public class EventTypeString
    {
        public static String GetEventTypeDetectString(int nEventType)
        {
            string resultMsg = "";

            UnE.Sensor.IFacility.FacilityType eventType = UnE.Sensor.IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                case IFacility.FacilityType.FireSensor_TypeA:
                case IFacility.FacilityType.FireSensor_TypeB:
                case IFacility.FacilityType.FireSensor_GasEmission:
                case IFacility.FacilityType.FireSensor_ManualControl:
                case IFacility.FacilityType.FireSensor_LightType:
                case IFacility.FacilityType.FireSensor_SiemensType:
                case IFacility.FacilityType.FireSensor_Monitoring:
                case IFacility.FacilityType.FireSensor_SensingLine:
                case IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case IFacility.FacilityType.FireSensor_MonitoringType: 
                    resultMsg = "화재";
                    break;
                case IFacility.FacilityType.COOLER_SENSOR:
                    resultMsg = "소화 센서";
                    break;
                case IFacility.FacilityType.PRESSURE_SENSOR:
                    resultMsg = "압력 센서";
                    break;
                case IFacility.FacilityType.PSM_SENSOR:
                    resultMsg = "유해화학물질 누출감지 센서";
                    break;
                case IFacility.FacilityType.Intrusion_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.Loiter_S1:
                    resultMsg = "배회";
                    break;
                case IFacility.FacilityType.Collapse_S1:
                    resultMsg = "넘어짐";
                    break;
                case IFacility.FacilityType.Theft_S1:
                    resultMsg = "도난";
                    break;
                case IFacility.FacilityType.Neglect_S1:
                    resultMsg = "방치";
                    break;
                case IFacility.FacilityType.VirtualFence_S1:
                    resultMsg = "(가상펜스)침입";
                    break;
                case IFacility.FacilityType.Fire_S1:
                    resultMsg = "SVMS화재";
                    break;
                case IFacility.FacilityType.EmergencyBell_S1:
                    resultMsg = "비상벨";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT1_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT2_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.InternalIntrusionT3_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.VaultIntrusionT4_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.FireF1_S1:
                    resultMsg = "ACCESS화재";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC1_S1:
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC2_S1:
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.RescueQQ_S1:
                    resultMsg = "구급";
                    break;
                case IFacility.FacilityType.GasG1_S1:
                    resultMsg = "가스누출";
                    break;
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                    resultMsg = "정전";
                    break;
                case IFacility.FacilityType.LeakAbnormalityU4_S1:
                    resultMsg = "누수";
                    break;
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    resultMsg = "종합경보반 이상";
                    break;
                case IFacility.FacilityType.ExternalAlarmBell:
                    resultMsg = "외부비상벨 호출";
                    break;
                default:
                    break;
            }
            return resultMsg;
        }


        public static String GetEventTypeIgnoreString(int nEventType)
        {
            string resultMsg = "";

            UnE.Sensor.IFacility.FacilityType eventType = UnE.Sensor.IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                case IFacility.FacilityType.FireSensor_TypeA:
                case IFacility.FacilityType.FireSensor_TypeB:
                case IFacility.FacilityType.FireSensor_GasEmission:
                case IFacility.FacilityType.FireSensor_ManualControl:
                case IFacility.FacilityType.FireSensor_LightType:
                case IFacility.FacilityType.FireSensor_SiemensType:
                case IFacility.FacilityType.FireSensor_Monitoring:
                case IFacility.FacilityType.FireSensor_SensingLine:
                case IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case IFacility.FacilityType.FireSensor_MonitoringType:
                    resultMsg = "화재";
                    break;
                case IFacility.FacilityType.COOLER_SENSOR:
                    resultMsg = "소화 센서";
                    break;
                case IFacility.FacilityType.PRESSURE_SENSOR:
                    resultMsg = "압력 센서";
                    break;
                case IFacility.FacilityType.PSM_SENSOR:
                    resultMsg = "유해화학물질 누출감지 센서";
                    break;
                case IFacility.FacilityType.Intrusion_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.Loiter_S1:
                    resultMsg = "배회";
                    break;
                case IFacility.FacilityType.Collapse_S1:
                    resultMsg = "넘어짐";
                    break;
                case IFacility.FacilityType.Theft_S1:
                    resultMsg = "도난";
                    break;
                case IFacility.FacilityType.Neglect_S1:
                    resultMsg = "방치";
                    break;
                case IFacility.FacilityType.VirtualFence_S1:
                    resultMsg = "(가상펜스)침입";
                    break;
                case IFacility.FacilityType.Fire_S1:
                    resultMsg = "SVMS화재";
                    break;
                case IFacility.FacilityType.EmergencyBell_S1:
                    resultMsg = "비상벨";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT1_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT2_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.InternalIntrusionT3_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.VaultIntrusionT4_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.FireF1_S1:
                    resultMsg = "ACCESS화재";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC1_S1:
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC2_S1:
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.RescueQQ_S1:
                    resultMsg = "구급";
                    break;
                case IFacility.FacilityType.GasG1_S1:
                    resultMsg = "가스누출";
                    break;
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                    resultMsg = "정전";
                    break;
                case IFacility.FacilityType.LeakAbnormalityU4_S1:
                    resultMsg = "누수";
                    break;
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    resultMsg = "종합경보반 이상";
                    break;
                case IFacility.FacilityType.ExternalAlarmBell:
                    resultMsg = "외부비상벨 호출";
                    break;
                default:
                    break;
            }
            return resultMsg;
        }
    }
}
