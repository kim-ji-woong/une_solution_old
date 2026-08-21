
export default class SensorMakerResource {    
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

        Security_Sensor: 899,
        Intrusion_S1: 900,                    // SVMS 침입
        Loiter_S1: 901,                       // SVMS 배회
        Collapse_S1: 902,                     // SVMS 쓰러짐
        Theft_S1: 903,                        // SVMS 도난
        Neglect_S1: 904,                      // SVMS 방치
        VirtualFence_S1: 905,                 // SVMS 가상펜스
        Fire_S1: 906,                         // SVMS 화재
    }

    static getFacilityTypeString(nType) {
        if (nType === SensorMakerResource.facilityType.FIRE)
            return "화재센서";
        else if (nType === SensorMakerResource.facilityType.PSM_SENSOR)
            return "누출";
        else if (nType === SensorMakerResource.facilityType.ETC)
            return "기타";
        else if (nType === SensorMakerResource.facilityType.Temp)
            return "온도";
        else if (nType === SensorMakerResource.facilityType.Humi)
            return "습도";
        else if (nType === SensorMakerResource.facilityType.CO2)
            return "이산화탄소";
        else if (nType === SensorMakerResource.facilityType.TVOC)
            return "TVOC";
        else if (nType === SensorMakerResource.facilityType.Dust_PM1)
            return "미세먼지(PM 1.0)";
        else if (nType === SensorMakerResource.facilityType.Dust_PM2)
            return "미세먼지(PM 2.5)";
        else if (nType === SensorMakerResource.facilityType.Dust_PM10)
            return "미세먼지(PM 10)";
        else if (nType === SensorMakerResource.facilityType.AirPress)
            return "기압";
        else if (nType === SensorMakerResource.facilityType.Inclin_X)
            return "기울기(X)";
        else if (nType === SensorMakerResource.facilityType.Inclin_Y)
            return "기울기(Y)";
        else if (nType === SensorMakerResource.facilityType.Vib_X)
            return "진동(X)";
        else if (nType === SensorMakerResource.facilityType.Vib_Y)
            return "진동(Y)";
        else if (nType === SensorMakerResource.facilityType.Vib_Z)
            return "진동(Z)";
        else if (nType === SensorMakerResource.facilityType.Noise)
            return "소음";
        else if (nType === SensorMakerResource.facilityType.BLE_Count)
            return "BLE Count";
        else if (nType === SensorMakerResource.facilityType.HF)
            return "불화수소";
        else if (nType === SensorMakerResource.facilityType.CO)
            return "일산화탄소";
        else if (nType === SensorMakerResource.facilityType.O2)
            return "산소";
        else if (nType === SensorMakerResource.facilityType.Value)
            return "ESH_v5.1 측정값";
        else if (nType === SensorMakerResource.facilityType.mA)
            return "mA";
        else if (nType === SensorMakerResource.facilityType.Contact)
            return "접점";
        else if (nType === SensorMakerResource.facilityType.Relay)
            return "릴레이";
        else if (nType === SensorMakerResource.facilityType.HCL)
            return "염화수소";
        else if (nType === SensorMakerResource.facilityType.CH3C)
            return "초산";
        else if (nType === SensorMakerResource.facilityType.N2H4)
            return "하이드라진";
        else if (nType === SensorMakerResource.facilityType.CA)
            return "CA Gas";
        else if (nType === SensorMakerResource.facilityType.EA)
            return "에틸알콜";
        else if (nType === SensorMakerResource.facilityType.VOC)
            return "VOC";
        else if (nType === SensorMakerResource.facilityType.H2O2)
            return "과수";
        else if (nType === SensorMakerResource.facilityType.THC)
            return "에탄올";
        else if (nType === SensorMakerResource.facilityType.HNO3)
            return "질산";
        else if (nType === SensorMakerResource.facilityType.CL)
            return "염소가스";
        else if (nType === SensorMakerResource.facilityType.TOLUENE)
            return "톨루엔";
        else if (nType === SensorMakerResource.facilityType.F2)
            return "불소";
        else if (nType === SensorMakerResource.facilityType.NH3)
            return "암모니아";
        else if (nType === SensorMakerResource.facilityType.LNG)
            return "액화천연가스";
        else if (nType === SensorMakerResource.facilityType.PGMEA)
            return "유기가스";
        else if (nType === SensorMakerResource.facilityType.H2S)
            return "황화수소";
        else if (nType === SensorMakerResource.facilityType.Intrusion_S1)
            return "지능형영상(침입)";
        else if (nType === SensorMakerResource.facilityType.Loiter_S1)
            return "지능형영상(배회)";
        else if (nType === SensorMakerResource.facilityType.Collapse_S1)
            return "지능형영상(쓰러짐)";
        else if (nType === SensorMakerResource.facilityType.Theft_S1)
            return "지능형영상(도난)";
        else if (nType === SensorMakerResource.facilityType.Neglect_S1)
            return "지능형영상(방치)";
        else if (nType === SensorMakerResource.facilityType.VirtualFence_S1)
            return "지능형영상(가상펜스)";
        else if (nType === SensorMakerResource.facilityType.Fire_S1)
            return "지능형영상(화재)";

        return "";
    }

    static isSVMSSensorType(type) {
        if (type === SensorMakerResource.facilityType.Security_Sensor ||
            type === SensorMakerResource.facilityType.Intrusion_S1 ||
            type === SensorMakerResource.facilityType.Loiter_S1 ||
            type === SensorMakerResource.facilityType.Collapse_S1 ||
            type === SensorMakerResource.facilityType.Theft_S1 ||
            type === SensorMakerResource.facilityType.Neglect_S1 ||
            type === SensorMakerResource.facilityType.VirtualFence_S1 ||
            type === SensorMakerResource.facilityType.Fire_S1 ||
            type === SensorMakerResource.facilityType.EmergencyBell_S1)
            return true;

        return false;
    }

    static isETCSensorType(type) {
        if ((type >= SensorMakerResource.facilityType.FIREWALL && type <= SensorMakerResource.facilityType.ETC) ||
            type === SensorMakerResource.facilityType.Temp ||
            type === SensorMakerResource.facilityType.Humi ||
            type === SensorMakerResource.facilityType.CO2 ||
            type === SensorMakerResource.facilityType.TVOC ||
            type === SensorMakerResource.facilityType.Dust_PM1 ||
            type === SensorMakerResource.facilityType.Dust_PM2 ||
            type === SensorMakerResource.facilityType.Dust_PM10 ||
            type === SensorMakerResource.facilityType.AirPress ||
            type === SensorMakerResource.facilityType.Inclin_X ||
            type === SensorMakerResource.facilityType.Inclin_Y ||
            type === SensorMakerResource.facilityType.Vib_X ||
            type === SensorMakerResource.facilityType.Vib_Y ||
            type === SensorMakerResource.facilityType.Vib_Z ||
            type === SensorMakerResource.facilityType.Noise ||
            type === SensorMakerResource.facilityType.BLE_Count ||
            type === SensorMakerResource.facilityType.O2 ||
            type === SensorMakerResource.facilityType.Value ||
            type === SensorMakerResource.facilityType.mA ||
            type === SensorMakerResource.facilityType.Contact ||
            type === SensorMakerResource.facilityType.Relay)
            return true;

        return false;
    }

    static isPSMSensorType(type) {
        if (type === SensorMakerResource.facilityType.PSM_SENSOR ||
            type === SensorMakerResource.facilityType.HF ||
            type === SensorMakerResource.facilityType.CO ||
            type === SensorMakerResource.facilityType.HCL ||
            type === SensorMakerResource.facilityType.CH3C ||
            type === SensorMakerResource.facilityType.N2H4 ||
            type === SensorMakerResource.facilityType.CA ||
            type === SensorMakerResource.facilityType.EA ||
            type === SensorMakerResource.facilityType.VOC ||
            type === SensorMakerResource.facilityType.H2O2 ||
            type === SensorMakerResource.facilityType.THC ||
            type === SensorMakerResource.facilityType.HNO3 ||
            type === SensorMakerResource.facilityType.CL ||
            type === SensorMakerResource.facilityType.TOLUENE ||
            type === SensorMakerResource.facilityType.F2 ||
            type === SensorMakerResource.facilityType.NH3 ||
            type === SensorMakerResource.facilityType.LNG ||
            type === SensorMakerResource.facilityType.PGMEA ||
            type === SensorMakerResource.facilityType.H2S)
            return true;

        return false;
    }
}