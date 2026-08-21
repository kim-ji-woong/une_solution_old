import ProjectResource from "../../Root/resource/id";

import imgCloudy from '../../Common/img/weather/cloudy.png';
import imgCloudDay from '../../Common/img/weather/cloud_day.png';
import imgCloudNight from '../../Common/img/weather/cloud_night.png';
import imgHeavySnow from '../../Common/img/weather/heavySnow.png';
import imgSnow from '../../Common/img/weather/snow.png';
import imgSnowRain from '../../Common/img/weather/snowRain.png';
import imgHeavyRain from '../../Common/img/weather/heavyRain.png';
import imgRain from '../../Common/img/weather/rain.png';
import imgSunnyDay from '../../Common/img/weather/sunny_day.png';
import imgSunnyNight from '../../Common/img/weather/sunny_night.png';
import imgThunder from '../../Common/img/weather/thunder.png';
import imgDustStorm from '../../Common/img/weather/dustStorm.png';

export default class SdmsResource {
    static get ID() {
        return SdmsResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            projectName: "SDMS",

            menu:
            {
                statusInfo: "현황정보",
                allCCTV: "전체 CCTV",
                cctv: "CCTV 영상정보",
                alarmCCTV: "알람 CCTV",
                alarmCCTV1: "알람 CCTV_1",
                alarmCCTV2: "알람 CCTV_2",
                alarmCCTV3: "알람 CCTV_3",
                dashboard: "대시보드",
                eventInfo: "이벤트 정보",
                miniMap: "미니맵",
                editMode: "편집모드",
                manualReport: "수동신고",
                weatherInfo: "기상정보",
                editModeStatusInfo: "편집모드",
                buildingInfo: "정보",
            },

            buildingInfo:
            {
                buildingGroupType: "건물그룹",
                buildingType: "건물",
                equipmentType: "설비",
                sensorInfo: "센서정보",
            },
            common:
            {
                confirm: "확인",
                cancel: "취소"
            },
            broadcast:
            {
                on: "방송장비의 알람상태를 동작시키며, 방송을 진행하게 됩니다.",
                onInfo: ["방송장비의 알람상태를 동작합니다.", "방송을 진행하게 됩니다.", "계속 할까요?"],
                close: "방송장비의 알람상태를 해제하며, 진행중인 방송이 있으면 종료시킵니다.",
                closeInfo: ["방송장비의 알람상태를 해제합니다.", "진행중인 방송이 있으면 즉시 종료됩니다.", "계속 할까요?"],
                onBroadcast: "방송시작",
                closeBroadcast: "방송종료"
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

        Intrusion_S1: 900,                    // SVMS 침입
        Loiter_S1: 901,                       // SVMS 배회
        Collapse_S1: 902,                     // SVMS 쓰러짐
        Theft_S1: 903,                        // SVMS 도난
        Neglect_S1: 904,                      // SVMS 방치
        VirtualFence_S1: 905,                 // SVMS 가상펜스
        Fire_S1: 906,                         // SVMS 화재
    }

    static materialType = {
        PSM: 11,        // 유해화학물질 누출감지 센서

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
        pH: 238,
        AUTO: 239,
        GATE1_OPEN: 240,
        GATE1_CLOSE: 241,
        GATE1_RATE: 242,
        GATE1_FAULT: 243,
        GATE2_OPEN: 244,
        GATE2_CLOSE: 245,
        GATE2_RATE: 246,
        GATE2_FAULT: 247,
        BATTERY: 248,
        OPERATION: 249,
        WATER_TEMP: 250,
        SCRUBBER: 251,
        F: 252,
        H2: 253,
        CL2: 254,
        C2H6O: 255,
        Flame: 256,
        Leak: 257,
        LEL: 258,
        TEPO: 259,
        CONNECT: 260,
    }

    static getFacilityTypeString(nType) {
        if (nType === SdmsResource.facilityType.FIRE)
            return "화재센서";
        else if (nType === SdmsResource.facilityType.PSM_SENSOR)
            return "누출";
        else if (nType === SdmsResource.facilityType.ETC)
            return "기타";
        else if (nType === SdmsResource.materialType.Temp)
            return "온도";
        else if (nType === SdmsResource.materialType.Humi)
            return "습도";
        else if (nType === SdmsResource.materialType.CO2)
            return "이산화탄소";
        else if (nType === SdmsResource.materialType.TVOC)
            return "TVOC";
        else if (nType === SdmsResource.materialType.Dust_PM1)
            return "미세먼지(PM 1.0)";
        else if (nType === SdmsResource.materialType.Dust_PM2)
            return "미세먼지(PM 2.5)";
        else if (nType === SdmsResource.materialType.Dust_PM10)
            return "미세먼지(PM 10)";
        else if (nType === SdmsResource.materialType.AirPress)
            return "기압";
        else if (nType === SdmsResource.materialType.Inclin_X)
            return "기울기(X)";
        else if (nType === SdmsResource.materialType.Inclin_Y)
            return "기울기(Y)";
        else if (nType === SdmsResource.materialType.Vib_X)
            return "진동(X)";
        else if (nType === SdmsResource.materialType.Vib_Y)
            return "진동(Y)";
        else if (nType === SdmsResource.materialType.Vib_Z)
            return "진동(Z)";
        else if (nType === SdmsResource.materialType.Noise)
            return "소음";
        else if (nType === SdmsResource.materialType.BLE_Count)
            return "BLE Count";
        else if (nType === SdmsResource.materialType.HF)
            return "불화수소";
        else if (nType === SdmsResource.materialType.CO)
            return "일산화탄소";
        else if (nType === SdmsResource.materialType.O2)
            return "산소";
        else if (nType === SdmsResource.materialType.Value)
            return "ESH_v5.1 측정값";
        else if (nType === SdmsResource.materialType.mA)
            return "mA";
        else if (nType === SdmsResource.materialType.Contact)
            return "접점";
        else if (nType === SdmsResource.materialType.Relay)
            return "릴레이";
        else if (nType === SdmsResource.materialType.HCL)
            return "염화수소";
        else if (nType === SdmsResource.materialType.CH3C)
            return "초산";
        else if (nType === SdmsResource.materialType.N2H4)
            return "하이드라진";
        else if (nType === SdmsResource.materialType.CA)
            return "CA Gas";
        else if (nType === SdmsResource.materialType.EA)
            return "에틸알콜";
        else if (nType === SdmsResource.materialType.VOC)
            return "VOC";
        else if (nType === SdmsResource.materialType.H2O2)
            return "과수";
        else if (nType === SdmsResource.materialType.THC)
            return "에탄올";
        else if (nType === SdmsResource.materialType.HNO3)
            return "질산";
        else if (nType === SdmsResource.materialType.CL)
            return "염소가스";
        else if (nType === SdmsResource.materialType.TOLUENE)
            return "톨루엔";
        else if (nType === SdmsResource.materialType.F2)
            return "불소";
        else if (nType === SdmsResource.materialType.NH3)
            return "암모니아";
        else if (nType === SdmsResource.materialType.LNG)
            return "액화천연가스";
        else if (nType === SdmsResource.materialType.PGMEA)
            return "유기가스";
        else if (nType === SdmsResource.materialType.H2S)
            return "황화수소";
        else if (nType === SdmsResource.materialType.pH)
            return "pH";
        else if (nType === SdmsResource.materialType.AUTO)
            return "자동모드";
        else if (nType === SdmsResource.materialType.GATE1_OPEN)
            return "수문1 열림";
        else if (nType === SdmsResource.materialType.GATE1_CLOSE)
            return "수문1 닫힘";
        else if (nType === SdmsResource.materialType.GATE1_RATE)
            return "수문1 개도율";
        else if (nType === SdmsResource.materialType.GATE1_FAULT)
            return "수문1 FAULT";
        else if (nType === SdmsResource.materialType.GATE2_OPEN)
            return "수문2 열림";
        else if (nType === SdmsResource.materialType.GATE2_CLOSE)
            return "수문2 닫힘";
        else if (nType === SdmsResource.materialType.GATE2_RATE)
            return "수문2 개도율";
        else if (nType === SdmsResource.materialType.GATE2_FAULT)
            return "수문2 FAULT";
        else if (nType === SdmsResource.materialType.BATTERY)
            return "배터리";
        else if (nType === SdmsResource.materialType.OPERATION)
            return "동작상태";
        else if (nType === SdmsResource.materialType.WATER_TEMP)
            return "수온";
        else if (nType === SdmsResource.materialType.SCRUBBER)
            return "스크러버";
        else if (nType === SdmsResource.materialType.F)
            return "F";
        else if (nType === SdmsResource.materialType.H2)
            return "수소";
        else if (nType === SdmsResource.materialType.CL2)
            return "CL2";
        else if (nType === SdmsResource.materialType.C2H6O)
            return "C2H6O";
        else if (nType === SdmsResource.materialType.Flame)
            return "Flame";
        else if (nType === SdmsResource.materialType.Leak)
            return "Leak";
        else if (nType === SdmsResource.materialType.LEL)
            return "LEL";
        else if (nType === SdmsResource.materialType.TEPO)
            return "TEPO";
        else if (nType === SdmsResource.materialType.CONNECT)
            return "통신상태";
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
            type === SdmsResource.materialType.Temp ||
            type === SdmsResource.materialType.Humi ||
            type === SdmsResource.materialType.CO2 ||
            type === SdmsResource.materialType.TVOC ||
            type === SdmsResource.materialType.Dust_PM1 ||
            type === SdmsResource.materialType.Dust_PM2 ||
            type === SdmsResource.materialType.Dust_PM10 ||
            type === SdmsResource.materialType.AirPress ||
            type === SdmsResource.materialType.Inclin_X ||
            type === SdmsResource.materialType.Inclin_Y ||
            type === SdmsResource.materialType.Vib_X ||
            type === SdmsResource.materialType.Vib_Y ||
            type === SdmsResource.materialType.Vib_Z ||
            type === SdmsResource.materialType.Noise ||
            type === SdmsResource.materialType.BLE_Count ||
            type === SdmsResource.materialType.O2 ||
            type === SdmsResource.materialType.Value ||
            type === SdmsResource.materialType.mA ||
            type === SdmsResource.materialType.Contact ||
            type === SdmsResource.materialType.Relay ||
            type === SdmsResource.materialType.pH ||
            type === SdmsResource.materialType.AUTO ||
            type === SdmsResource.materialType.GATE1_OPEN ||
            type === SdmsResource.materialType.GATE1_CLOSE ||
            type === SdmsResource.materialType.GATE1_RATE ||
            type === SdmsResource.materialType.GATE1_FAULT ||
            type === SdmsResource.materialType.GATE2_OPEN ||
            type === SdmsResource.materialType.GATE2_CLOSE ||
            type === SdmsResource.materialType.GATE2_RATE ||
            type === SdmsResource.materialType.GATE2_FAULT ||
            type === SdmsResource.materialType.BATTERY ||
            type === SdmsResource.materialType.OPERATION ||
            type === SdmsResource.materialType.WATER_TEMP ||
            type === SdmsResource.materialType.SCRUBBER ||
            type === SdmsResource.materialType.Flame ||
            type === SdmsResource.materialType.Leak ||
            type === SdmsResource.materialType.LEL ||
            type === SdmsResource.materialType.CONNECT)
            return true;

        return false;
    }

    static isPSMSensorType(type) {
        if (type === SdmsResource.facilityType.PSM_SENSOR ||
            type === SdmsResource.materialType.HF ||
            type === SdmsResource.materialType.CO ||
            type === SdmsResource.materialType.HCL ||
            type === SdmsResource.materialType.CH3C ||
            type === SdmsResource.materialType.N2H4 ||
            type === SdmsResource.materialType.CA ||
            type === SdmsResource.materialType.EA ||
            type === SdmsResource.materialType.VOC ||
            type === SdmsResource.materialType.H2O2 ||
            type === SdmsResource.materialType.THC ||
            type === SdmsResource.materialType.HNO3 ||
            type === SdmsResource.materialType.CL ||
            type === SdmsResource.materialType.TOLUENE ||
            type === SdmsResource.materialType.F2 ||
            type === SdmsResource.materialType.NH3 ||
            type === SdmsResource.materialType.LNG ||
            type === SdmsResource.materialType.PGMEA ||
            type === SdmsResource.materialType.H2S ||
            type === SdmsResource.materialType.F ||
            type === SdmsResource.materialType.H2 ||
            type === SdmsResource.materialType.CL2 ||
            type === SdmsResource.materialType.C2H6O ||
            type === SdmsResource.materialType.TEPO)
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

    static getWeatherStateString(state) {
        if (state === SdmsResource.WeatherInfo.Sunshine) {
            return "맑음";
        }
        else if (state === SdmsResource.WeatherInfo.Thunder) {
            return "천둥번개";
        }
        else if (state === SdmsResource.WeatherInfo.SnowRain) {
            return "진눈깨비";
        }
        else if (state === SdmsResource.WeatherInfo.HeavySnow) {
            return "폭설";
        }
        else if (state === SdmsResource.WeatherInfo.Snow) {
            return "눈";
        }
        else if (state === SdmsResource.WeatherInfo.HeavyRain) {
            return "폭우";
        }
        else if (state === SdmsResource.WeatherInfo.Rain) {
            return "비";
        }
        else if (state === SdmsResource.WeatherInfo.Cloudy) {
            return "구름";
        }
        else if (state === SdmsResource.WeatherInfo.DustStorm) {
            return "황사";
        }

        if (SdmsResource.isDayLight()) {
            return "구름조금";
        }

        return "밤";
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
        manualReport: 54,
        weatherInfo: 55,
        editMode: 56,

        cctvAlarm1: 49,
        cctvAlarm2: 50,
        cctvAlarm3: 51,
    }

    // SDMS 팝업 시스템 초기화 셋팅 값
    static popupResetLocation = {
        weatherInfo: {
            x: '0.5%', y: '6%', height: '180px', width: '320px'
        },
        statusInfo: {
            x: '0.5%', y: '26%', height: '500px', width: '320px'
        },
        buildingInfo: {
            x: '0.5%', y: '78%', height: '210px', width: '320px'
        },
        dashboard: {
            x: '26%', y: '9%', height: '79px', width: '970px'
        },
        miniMap: {
            x: '19%', y: '70%', height: '260px', width: '350px'
        },
        event: {
            x: '80%', y: '13%', height: '425px', width: '360px'
        },
        cctvInfo: {
            x: '80%', y: '57%', height: '380px', width: '360px'
        },
        cctvInfo_1: {
            x: '60%', y: '57%', height: '380px', width: '360px'
        },
        cctvInfo_2: {
            x: '40%', y: '57%', height: '380px', width: '360px'
        },
        cctvInfo_3: {
            x: '20%', y: '57%', height: '380px', width: '360px'
        },
    }

    static BroadcastState = {
        None: 0,
        Run: 1,
        Stop: 2,
    }

    static popupLayer = {
        statusInfo: "statusInfo",
        cctvInfo: "cctvInfo",
        cctvInfo_1: "cctvInfo_1",
        cctvInfo_2: "cctvInfo_2",
        cctvInfo_3: "cctvInfo_3",
        buildingInfo: "buildingInfo",
        dashboard: "dashboard",
        event: "event",
        miniMap: "miniMap",
        weatherInfo: "weatherInfo",
        editModeStatusInfo: "editModeStatusInfo",
        manualReport: "manualReport"
    }
}