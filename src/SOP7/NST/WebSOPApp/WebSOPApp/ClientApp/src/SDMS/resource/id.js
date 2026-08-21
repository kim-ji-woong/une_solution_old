import ProjectResource from "../../Root/resource/id";

import imgCloudy from '../../Root/image/weather_Icon/cloudy.png';
import imgCloudDay from '../../Root/image/weather_Icon/cloud_day.png';
import imgCloudNight from '../../Root/image/weather_Icon/cloud_night.png';
import imgHeavySnow from '../../Root/image/weather_Icon/heavy_snow.png';
import imgSnow from '../../Root/image/weather_Icon/snow.png';
import imgSnowRain from '../../Root/image/weather_Icon/rain_snow.png';
import imgHeavyRain from '../../Root/image/weather_Icon/heavy_rain.png';
import imgRain from '../../Root/image/weather_Icon/rain.png';
import imgSunnyDay from '../../Root/image/weather_Icon/sunny_day.png';
import imgSunnyNight from '../../Root/image/weather_Icon/sunny_night.png';
import imgThunder from '../../Root/image/weather_Icon/thunder.png';
import imgDustStorm from '../../Root/image/weather_Icon/dust_storm.png';

export default class SdmsResource {
    static get ID() {
        return SdmsResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            projectName: "SDMS",

            menu:
            {
/*              statusInfo: "현황정보",
                cctv: "CCTV 영상정보",
                dashboard: "대시보드",
                eventInfo: "이벤트 정보",
                miniMap: "미니맵",
                editMode: "편집모드",
                manualReport: "수동신고",
                weatherInfo: "기상정보",
                editModeStatusInfo: "편집모드"*/

                statusInfo: "현황정보",
                cctvInfo: "CCTV 영상정보",
                workerInfo: "작업자 모니터링",
                /*"POI뷰어 설정"*/
                spreadInfo: "상황전파",

                /*"이력관리"*/
                sensorDetectHistory: "센서 감지 이력",
                sensorDetectAnalysis: "센서 감지 분석",
                sopHistory: "SOP 이력",
                spreadHistory: "상황전파 이력"

            },

            buildingInfo:
            {
                buildingGroupType: "건물그룹",
                buildingType: "건물",
                equipmentType: "설비",
            },

            errorMessage:
            {
                loadFailFacilityInfo: "설비 정보를 불러올 수 없습니다.",
                loadFailBuildingData: "건물 정보를 불러올 수 없습니다.",
                loadFailBuildingGroupData: "건물그룹 정보를 불러올 수 없습니다."
            }
        }
    }

    static facilityType = {
        FIRE: 0,

        PSM_SENSOR: 11,        // 유해화학물질 누출감지 센서

        FIREWALL: 15,                      // 방화벽

        ETC: 21,                           // 기타

        // Soulbrain 공장설비
        Temp: 200,
        Humi: 201,
        CO2: 202,
        TVOC: 203,
        Dust_PM1: 204,
        Dust_PM2: 205,
        Dust_PM10: 206,
        AirPress: 207,
        Inclin_X: 208,
        Inclin_Y: 209,
        Vib_X: 210,
        Vib_Y: 211,
        Vib_Z: 212,
        Noise: 213,
        BLE_Count: 214,
        HF: 215,
        CO: 216,
        O2: 217,
        Value: 218,
        mA: 219,
        Contact: 220,
        Relay: 221,
        HCL: 222,
        CH3C: 223,
        N2H4: 224,
        CA: 225,
        EA: 226,
        VOC: 227,
        H2O2: 228,
        THC: 229,
        HNO3: 230,
        CL: 231,
        TOLUENE: 232,
        F2: 233,
        NH3: 234,
        LNG: 235,
        PGMEA: 236,
        H2S: 237,

        Intrusion_S1: 900,                    // SVMS 침입
        Loiter_S1: 901,                       // SVMS 배회
        Collapse_S1: 902,                     // SVMS 쓰러짐
        Theft_S1: 903,                        // SVMS 도난
        Neglect_S1: 904,                      // SVMS 방치
        VirtualFence_S1: 905,                 // SVMS 가상펜스
        Fire_S1: 906,                         // SVMS 화재
    }

    static getFacilityTypeString(nType) {
        if (nType === SdmsResource.facilityType.FIRE)
            return "화재센서";
        else if (nType === SdmsResource.facilityType.ETC)
            return "기타";
        else if (nType === SdmsResource.facilityType.Temp)
            return "온도";
        else if (nType === SdmsResource.facilityType.Humi)
            return "습도";
        else if (nType === SdmsResource.facilityType.CO2)
            return "이산화탄소";
        else if (nType === SdmsResource.facilityType.TVOC)
            return "TVOC";
        else if (nType === SdmsResource.facilityType.Dust_PM1)
            return "미세먼지(PM 1.0)";
        else if (nType === SdmsResource.facilityType.Dust_PM2)
            return "미세먼지(PM 2.5)";
        else if (nType === SdmsResource.facilityType.Dust_PM10)
            return "미세먼지(PM 10)";
        else if (nType === SdmsResource.facilityType.AirPress)
            return "기압";
        else if (nType === SdmsResource.facilityType.Inclin_X)
            return "기울기(X)";
        else if (nType === SdmsResource.facilityType.Inclin_Y)
            return "기울기(Y)";
        else if (nType === SdmsResource.facilityType.Vib_X)
            return "진동(X)";
        else if (nType === SdmsResource.facilityType.Vib_Y)
            return "진동(Y)";
        else if (nType === SdmsResource.facilityType.Vib_Z)
            return "진동(Z)";
        else if (nType === SdmsResource.facilityType.Noise)
            return "소음";
        else if (nType === SdmsResource.facilityType.BLE_Count)
            return "BLE Count";
        else if (nType === SdmsResource.facilityType.HF)
            return "불화수소";
        else if (nType === SdmsResource.facilityType.CO)
            return "일산화탄소";
        else if (nType === SdmsResource.facilityType.O2)
            return "산소";
        else if (nType === SdmsResource.facilityType.Value)
            return "ESH_v5.1 측정값";
        else if (nType === SdmsResource.facilityType.mA)
            return "mA";
        else if (nType === SdmsResource.facilityType.Contact)
            return "접점";
        else if (nType === SdmsResource.facilityType.Relay)
            return "릴레이";
        else if (nType === SdmsResource.facilityType.HCL)
            return "염화수소";
        else if (nType === SdmsResource.facilityType.CH3C)
            return "초산";
        else if (nType === SdmsResource.facilityType.N2H4)
            return "하이드라진";
        else if (nType === SdmsResource.facilityType.CA)
            return "CA Gas";
        else if (nType === SdmsResource.facilityType.EA)
            return "에틸알콜";
        else if (nType === SdmsResource.facilityType.VOC)
            return "VOC";
        else if (nType === SdmsResource.facilityType.H2O2)
            return "과수";
        else if (nType === SdmsResource.facilityType.THC)
            return "에탄올";
        else if (nType === SdmsResource.facilityType.HNO3)
            return "질산";
        else if (nType === SdmsResource.facilityType.CL)
            return "염소가스";
        else if (nType === SdmsResource.facilityType.TOLUENE)
            return "톨루엔";
        else if (nType === SdmsResource.facilityType.F2)
            return "불소";
        else if (nType === SdmsResource.facilityType.NH3)
            return "암모니아";
        else if (nType === SdmsResource.facilityType.LNG)
            return "액화천연가스";
        else if (nType === SdmsResource.facilityType.PGMEA)
            return "유기가스";
        else if (nType === SdmsResource.facilityType.H2S)
            return "황화수소";
        else if (nType === SdmsResource.facilityType.Intrusion_S1)
            return "지능형영상(침입)";
        else if (nType === SdmsResource.facilityType.Loiter_S1)
            return "지능형영상(배회)";
        else if (nType === SdmsResource.facilityType.Collapse_S1)
            return "지능형영상(쓰러짐)";
        else if (nType === SdmsResource.facilityType.Theft_S1)
            return "지능형영상(도난)";
        else if (nType === SdmsResource.facilityType.Neglect_S1)
            return "지능형영상(방치)";
        else if (nType === SdmsResource.facilityType.VirtualFence_S1)
            return "지능형영상(가상펜스)";
        else if (nType === SdmsResource.facilityType.Fire_S1)
            return "지능형영상(화재)";

        return "";
    }

    static isSVMSSensorType(type) {
        if (type === SdmsResource.facilityType.Intrusion_S1 ||
            type === SdmsResource.facilityType.Loiter_S1 ||
            type === SdmsResource.facilityType.Collapse_S1 ||
            type === SdmsResource.facilityType.Theft_S1 ||
            type === SdmsResource.facilityType.Neglect_S1 ||
            type === SdmsResource.facilityType.VirtualFence_S1 ||
            type === SdmsResource.facilityType.Fire_S1 ||
            type === SdmsResource.facilityType.EmergencyBell_S1)
            return true;

        return false;
    }

    static isETCSensorType(type) {
        if ((type >= SdmsResource.facilityType.FIREWALL && type <= SdmsResource.facilityType.ETC) ||
            type === SdmsResource.facilityType.Temp ||
            type === SdmsResource.facilityType.Humi ||
            type === SdmsResource.facilityType.CO2 ||
            type === SdmsResource.facilityType.TVOC ||
            type === SdmsResource.facilityType.Dust_PM1 ||
            type === SdmsResource.facilityType.Dust_PM2 ||
            type === SdmsResource.facilityType.Dust_PM10 ||
            type === SdmsResource.facilityType.AirPress ||
            type === SdmsResource.facilityType.Inclin_X ||
            type === SdmsResource.facilityType.Inclin_Y ||
            type === SdmsResource.facilityType.Vib_X ||
            type === SdmsResource.facilityType.Vib_Y ||
            type === SdmsResource.facilityType.Vib_Z ||
            type === SdmsResource.facilityType.Noise ||
            type === SdmsResource.facilityType.BLE_Count ||
            type === SdmsResource.facilityType.O2 ||
            type === SdmsResource.facilityType.Value ||
            type === SdmsResource.facilityType.mA ||
            type === SdmsResource.facilityType.Contact ||
            type === SdmsResource.facilityType.Relay)
            return true;

        return false;
    }

    static isPSMSensorType(type) {
        if (type === SdmsResource.facilityType.PSM_SENSOR ||
            type === SdmsResource.facilityType.HF ||
            type === SdmsResource.facilityType.CO ||
            type === SdmsResource.facilityType.HCL ||
            type === SdmsResource.facilityType.CH3C ||
            type === SdmsResource.facilityType.N2H4 ||
            type === SdmsResource.facilityType.CA ||
            type === SdmsResource.facilityType.EA ||
            type === SdmsResource.facilityType.VOC ||
            type === SdmsResource.facilityType.H2O2 ||
            type === SdmsResource.facilityType.THC ||
            type === SdmsResource.facilityType.HNO3 ||
            type === SdmsResource.facilityType.CL ||
            type === SdmsResource.facilityType.TOLUENE ||
            type === SdmsResource.facilityType.F2 ||
            type === SdmsResource.facilityType.NH3 ||
            type === SdmsResource.facilityType.LNG ||
            type === SdmsResource.facilityType.PGMEA ||
            type === SdmsResource.facilityType.H2S)
            return true;

        return false;
    }

    static WeatherInfo = {
        Unknown: 0,
        Sunshine: 1,
        Thunder: 2,
        SnowRain: 3,
        HeavySnow: 4,
        Snow: 5,
        HeavyRain: 6,
        Rain: 7,
        Cloudy: 8,
        Cloud: 9,
        DustStorm: 10,
        FineDust: 11,
    }

    static getStateImage(state) {
        if (state === SdmsResource.WeatherInfo.Sunshine) {
            if (SdmsResource.isDayLight()) {
                return imgSunnyDay;
            }
            else {
                return imgSunnyNight;
            }
            //return imgSunshine;
        }
        else if (state === SdmsResource.WeatherInfo.Thunder) {
            return imgThunder;
        }
        else if (state === SdmsResource.WeatherInfo.SnowRain) {
            return imgSnowRain;
        }
        else if (state === SdmsResource.WeatherInfo.HeavySnow) {
            return imgHeavySnow;
        }
        else if (state === SdmsResource.WeatherInfo.Snow) {
            return imgSnow;
        }
        else if (state === SdmsResource.WeatherInfo.HeavyRain) {
            return imgHeavyRain;
        }
        else if (state === SdmsResource.WeatherInfo.Rain) {
            return imgRain;
        }
        else if (state === SdmsResource.WeatherInfo.Cloudy) {
            return imgCloudy;
        }
        else if (state === SdmsResource.WeatherInfo.DustStorm) {
            return imgDustStorm;
        }

        if (SdmsResource.isDayLight()) {
            return imgCloudDay;
        }

        return imgCloudNight;
    }

    static getStateString(state) {
        if (state === SdmsResource.WeatherInfo.Sunshine) {
            return "맑음";
        }
        else if (state === SdmsResource.WeatherInfo.Thunder) {
            return "천둥";
        }
        else if (state === SdmsResource.WeatherInfo.SnowRain) {
            return "진눈깨비";
        }
        else if (state === SdmsResource.WeatherInfo.HeavySnow) {
            return "강한 눈";
        }
        else if (state === SdmsResource.WeatherInfo.Snow) {
            return "눈";
        }
        else if (state === SdmsResource.WeatherInfo.HeavyRain) {
            return "강한 비";
        }
        else if (state === SdmsResource.WeatherInfo.Rain) {
            return "비";
        }
        else if (state === SdmsResource.WeatherInfo.Cloudy) {
            return "구름";
        }
        else if (state === SdmsResource.WeatherInfo.DustStorm) {
            return "미세먼지";
        }

        if (SdmsResource.isDayLight()) {
            return "구름";
        }

        return "구름";
    }

    static isDayLight() {
        const now = new Date();
        const hour = now.getHours();

        if (hour < 6 || hour >= 19) {
            return false;
        }

        return true;
    }

    static quickBtn = {
        statusInfo: 49,
        cctv: 50,
        dashboard: 51,
        eventInfo: 52,
        miniMap: 53,
        editMode: 54,
        manualReport: 55,
        weatherInfo: 56,
    }
}