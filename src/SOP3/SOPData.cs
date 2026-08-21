using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOP
{
    public class SDMSConfig
    {
        public enum ConfigType
        {
            EQUIPZONE_FACILITY_MANAGER = 1,
            BUILDING_FACILITY_MANAGER = 2,
            ENTIRE_FACILITY_MANAGER = 4,
            COMPANY_MEMBER = 8,
            REGULAR_TEAM = 16,
            TEMPARARY_NORMAL_TEAM = 32,     // 평일 비상조직
            TEMPARAY_EMERGENCY_TEAM = 64,   // 야간 및 휴일 비상조직
            EQUIPZONE_CCTV = 128,
            TEMPORARY_MEMBER = 256,
            EXTERNAL_MEMBER = 512,          // 외부 협력업체 직원
            EXTERNAL_TEAM = 1024,           // 외부 협력업체 팀
            USER_DEFINED_TEAM = 2048        // 사용자 정의조직
        };

        public static string GetPropertyName(ConfigType type)
        {
            switch (type)
            {
                case ConfigType.EQUIPZONE_FACILITY_MANAGER:
                case ConfigType.BUILDING_FACILITY_MANAGER:
                case ConfigType.ENTIRE_FACILITY_MANAGER:
                case ConfigType.COMPANY_MEMBER:
                case ConfigType.REGULAR_TEAM:
                case ConfigType.TEMPARARY_NORMAL_TEAM:
                case ConfigType.TEMPARAY_EMERGENCY_TEAM:
                    return PropertyName;

                case ConfigType.EQUIPZONE_CCTV:
                    return "EquipZoneCCTV";
            }

            return "";
        }

        public static string PropertyName
        {
            get { return "SDMSConfig"; }
        }
    }

    public class SOPSimulatorConfig
    {
        public enum ConfigType
        {
            WORKING_BEGIN_HOUR = 1,
            WORKING_END_HOUR = 2,
            USE_SMS = 4,
            USE_BROADCAST = 8,
            SMS_TO_EXTERNAL_MEMBER = 16,
            RUN_SOP_ON_LOADED = 32,
            SOP_AUTO_CLOSE = 64,
            USE_CONFIRMSENDSMS = 128,
            USE_VIRTUALMODE_IN_SENSOR = 256

            /*SOP_CLOSE_WAIT_INPUT_TIME,
            SOP_CLOSE_WAIT,
            SOP_CLOSE_SENSOR_CLOSE,
            SOP_CLOSE_SENSOR_CLOSE_WAIT_TIME,
            SOP_CLOSE_S*/
        };

        public static string GetPropertyName(ConfigType type)
        {
            switch (type)
            {
                case ConfigType.WORKING_BEGIN_HOUR:
                    return "WorkingBeginHour";

                case ConfigType.WORKING_END_HOUR:
                    return "WorkingEndHour";

                case ConfigType.USE_SMS:
                    return "UseSMS";

                case ConfigType.USE_BROADCAST:
                    return "UseBroadcast";

                case ConfigType.SMS_TO_EXTERNAL_MEMBER:
                    return "SMSToExternalCompany";

                case ConfigType.USE_CONFIRMSENDSMS:
                    return "UseConfirmSendSMS";

                case ConfigType.RUN_SOP_ON_LOADED:
                    return "SOPPlayOnLoaded";

                case ConfigType.SOP_AUTO_CLOSE:
                    return "SOPAutoClose";

                // 센서신호로 인한 SOP 동작시 훈련모드로 작동시킬 것인가?
                case ConfigType.USE_VIRTUALMODE_IN_SENSOR:
                    return "UseVirtualModeInSensor";
            }

            return "";
        }
    }

}
