using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamEditor.BLL;
using TeamEditor.Model.Sop.Team;

public class CommonString
{
    // 조직 정보
    public const string REGULAR_ROOT = "솔브레인";
    public const string REGULAR_ROOT_TEAM = "teamName";

    // 디바이스 버전
    //public const string VERSION_30055 = "30055";
    public const string VERSION_30056 = "30056";
    public const string VERSION_30057 = "30057";
    public const string VERSION_30058 = "30058";
    public const string VERSION_30061 = "30061";
    public const string VERSION_30062 = "30062";
    public const string VERSION_30063 = "30063";    // 인디게이터
    public const string VERSION_30064 = "30064";
    public const string VERSION_30065 = "30065";
    public const string VERSION_31007 = "31007";
    public const string VERSION_31008 = "31008";
    public const string VERSION_32001 = "32001";    // 게이트웨이
    public const string VERSION_32002 = "32002";
    public const string VERSION_32003 = "32003";    // 레벨감지기 수신반
    public const string VERSION_32004 = "32004";
    public const string VERSION_32005 = "32005";
    public const string VERSION_32006 = "32006";

    // 공장 이름
    public const string FACT_PAJU = "파주 공장";
    public const string FACT_GONGJU = "공주 공장";

    // 센서 상태
    public const string STATUS_OFFLINE = "OFFLINE";
    public const string STATUS_NORMAL = "NORMAL";
    public const string STATUS_CAUTION = "CAUTION";
    public const string STATUS_WARNING = "WARNING";

    public const int LEVEL_CAUTION = 2;
    public const int LEVEL_WARNING = 3;

    // 알람 관련 정보
    public const string ALARM_METHOD = "POST";

    // 디버깅용 센서
    public const string MODEL_DEBUGGING = "디버깅용";
    //public const string SENSOR_mA = "mA";
    //public const string SENSOR_CONTACT = "접점";
    //public const string SENSOR_RELAY = "릴레이";
    public const string SENSOR_GAS_TYPE = "가스종류";
    public const string SENSOR_LEVEL = "LEVEL";
    //public const string SENSOR_VALUE = "수치";
    public const string SENSOR_RESULT = "센서값";
    public const string SENSOR_STATUS = "STATUS";
    public const string TEST_DEVICE1 = "BERRY51-I003";      // 설치 장소가 표시 안된 디바이스
    public const string TEST_DEVICE2 = "TestDevice01";      // 한컴 테스트 디바이스
    public const string SENSOR_DMGD = "DMGD";
    
    public const string SENSOR_MAC = "MAC";
    public const string SENSOR_TYPE = "TYPE";
    public const string SENSOR_GW_ID = "GW_ID";
    public const string SENSOR_KIND = "종류";
    public const string SENSOR_MEASURE = "측정종류";

    public const string DEVICE_STATUS = "기기상태";
    public const string SENSOR_ERROR = "에러상태";
    public const string SENSOR_CH_NUM = "CH_NUM";

    public const string SENSOR_GAS_VAL1 = "Gas Value 1";
    public const string SENSOR_GAS_VAL2 = "Gas Value 2";
    public const string SENSOR_GAS_VAL3 = "Gas Value 3";
    public const string SENSOR_GAS_VAL4 = "Gas Value 4";
    public const string SENSOR_GAS_VAL5 = "Gas Value 5";

    public const string SENSOR_GAS_NAME1 = "Gas Name 1";
    public const string SENSOR_GAS_NAME2 = "Gas Name 2";
    public const string SENSOR_GAS_NAME3 = "Gas Name 3";
    public const string SENSOR_GAS_NAME4 = "Gas Name 4";
    public const string SENSOR_GAS_NAME5 = "Gas Name 5";


    // ETC 종류
    public const string ETC_TEMP = "온도";
    public const string ETC_TEMP_31007 = "Temperature";
    public const string ETC_HUMI = "습도";
    public const string ETC_HUMI_31007 = "Humidity";
    public const string ETC_CO2 = "CO2";
    public const string ETC_TVOC = "TVOC";
    public const string ETC_PM1 = "미세먼지(PM 1.0)";
    public const string ETC_PM1_31007 = "PM1.0";
    public const string ETC_PM2 = "미세먼지(PM 2.5)";
    public const string ETC_PM2_31007 = "PM2.5";
    public const string ETC_PM10 = "미세먼지(PM 10)";
    public const string ETC_PM10_31007 = "PM10";
    public const string ETC_AirPress = "기압";
    public const string ETC_Inclin_X = "기울기(X)";
    public const string ETC_Inclin_Y = "기울기(Y)";
    public const string ETC_Vib_X = "진동(X)";
    public const string ETC_Vib_Y = "진동(Y)";
    public const string ETC_Vib_Z = "진동(Z)";
    public const string ETC_Noise = "소음";
    public const string ETC_BLE_Count = "BLE_Count";
    public const string ETC_BLE_Count2 = "BLE Count";
    public const string ETC_O2 = "O2";
    public const string ETC_Value = "수치";
    public const string ETC_mA = "mA";
    public const string ETC_Contact = "접점";
    public const string ETC_Relay = "릴레이";

    public const string ETC_pH = "pH";
    public const string ETC_AUTO = "자동모드";
    public const string ETC_GATE1_OPEN = "수문1 열림";
    public const string ETC_GATE1_CLOSE = "수문1 닫힘";
    public const string ETC_GATE1_RATE = "수문1 개도율";
    public const string ETC_GATE1_FAULT = "수문1 FAULT";
    public const string ETC_GATE2_OPEN = "수문2 열림";
    public const string ETC_GATE2_CLOSE = "수문2 닫힘";
    public const string ETC_GATE2_RATE = "수문2 개도율";
    public const string ETC_GATE2_FAULT = "수문2 FAULT";
    public const string ETC_BATTERY = "배터리";
    public const string ETC_OPERATION = "동작상태";
    public const string ETC_WATER_TEMP = "수온";
    public const string ETC_SCRUBBER = "스크러버";
    public const string ETC_Flame = "Flame";
    public const string ETC_Leak = "Leak";
    public const string ETC_LEL = "LEL";
    public const string ETC_CONNECT = "통신상태";


    // PSM 종류
    public const string PSM_HF = "HF";
    public const string PSM_CO = "CO";
    public const string PSM_HCL = "HCL";
    public const string PSM_CH3C = "CH3COOH";
    public const string PSM_N2H4 = "N2H4";
    public const string PSM_CA = "CA";
    public const string PSM_EA = "EA";
    public const string PSM_VOC = "VOC";
    public const string PSM_H2O2 = "H2O2"; 
    public const string PSM_THC = "THC";
    public const string PSM_HNO3 = "HNO3";
    public const string PSM_CL = "CL";
    public const string PSM_TOLUENE = "TOLUENE";
    public const string PSM_F2 = "F2";
    public const string PSM_NH3 = "NH3";
    public const string PSM_LNG = "LNG";
    public const string PSM_PGME = "PGME";
    public const string PSM_H2S = "H2S";

    public const string PSM_F = "F";
    public const string PSM_H2 = "H2";
    public const string PSM_CL2 = "CL2";
    public const string PSM_C2H6O = "C2H6O";
    public const string PSM_TEPO = "TEPO";

    /*
    public const int PSM_HF_TYPE = 1;
    public const int PSM_CO_TYPE = 2;
    public const int PSM_HCL_TYPE = 3;

    public const int TYPE_HF = 215;
    public const int TYPE_CO = 216;
    public const int TYPE_HCL = 222;
    */

    public static bool IsPSMSensorType(string strType)
    {
        // 센서 타입 예외처리
        if (strType == "HCI")
            strType = "HCL";
        else if (strType == "CH3C")
            strType = "CH3COOH";
        else if (strType == "THC\u0005")
            strType = "THC";
        else if (strType == "NH3\u0005")
            strType = "NH3";
        else if (strType == "VOC\u0005")
            strType = "VOC";
        else if (strType == "HCL\u0005")
            strType = "HCL";
        else if (strType == "H2S\u0005")
            strType = "H2S";

        if (strType == CommonString.PSM_HF ||
            strType == CommonString.PSM_CO ||
            strType == CommonString.PSM_HCL ||
            strType == CommonString.PSM_CH3C ||
            strType == CommonString.PSM_N2H4 ||
            strType == CommonString.PSM_CA ||
            strType == CommonString.PSM_EA ||
            strType == CommonString.PSM_VOC ||
            strType == CommonString.PSM_H2O2 ||
            strType == CommonString.PSM_THC ||
            strType == CommonString.PSM_HNO3 ||
            strType == CommonString.PSM_CL ||
            strType == CommonString.PSM_TOLUENE ||
            strType == CommonString.PSM_F2 ||
            strType == CommonString.PSM_NH3 ||
            strType == CommonString.PSM_LNG ||
            strType == CommonString.PSM_PGME ||
            strType == CommonString.PSM_H2S ||
            strType == CommonString.PSM_F ||
            strType == CommonString.PSM_H2 ||
            strType == CommonString.PSM_CL2 ||
            strType == CommonString.PSM_C2H6O ||
            strType == CommonString.PSM_TEPO)
            return true;
        else
            return false;
    }

    public static bool IsETCSensorType(string strType)
    {
        if (strType == CommonString.ETC_TEMP ||
            strType == CommonString.ETC_TEMP_31007 ||
            strType == CommonString.ETC_HUMI ||
            strType == CommonString.ETC_HUMI_31007 ||
            strType == CommonString.ETC_CO2 ||
            strType == CommonString.ETC_TVOC ||
            strType == CommonString.ETC_PM1 ||
            strType == CommonString.ETC_PM1_31007 ||
            strType == CommonString.ETC_PM2 ||
            strType == CommonString.ETC_PM2_31007 ||
            strType == CommonString.ETC_PM10 ||
            strType == CommonString.ETC_PM10_31007 ||
            strType == CommonString.ETC_AirPress ||
            strType == CommonString.ETC_Inclin_X ||
            strType == CommonString.ETC_Inclin_Y ||
            strType == CommonString.ETC_Vib_X ||
            strType == CommonString.ETC_Vib_Y ||
            strType == CommonString.ETC_Vib_Z ||
            strType == CommonString.ETC_Noise ||
            strType == CommonString.ETC_BLE_Count ||
            strType == CommonString.ETC_O2 ||
            strType == CommonString.ETC_Value ||
            strType == CommonString.ETC_mA ||
            strType == CommonString.ETC_Contact ||
            strType == CommonString.ETC_pH ||
            strType == CommonString.ETC_AUTO ||
            strType == CommonString.ETC_GATE1_OPEN ||
            strType == CommonString.ETC_GATE1_CLOSE ||
            strType == CommonString.ETC_GATE1_RATE ||
            strType == CommonString.ETC_GATE1_FAULT ||
            strType == CommonString.ETC_GATE2_OPEN ||
            strType == CommonString.ETC_GATE2_CLOSE ||
            strType == CommonString.ETC_GATE2_RATE ||
            strType == CommonString.ETC_GATE2_FAULT ||
            strType == CommonString.ETC_BATTERY ||
            strType == CommonString.ETC_OPERATION ||
            strType == CommonString.ETC_WATER_TEMP ||
            strType == CommonString.ETC_SCRUBBER ||
            strType == CommonString.ETC_Relay ||
            strType == CommonString.ETC_Flame ||
            strType == CommonString.ETC_Leak ||
            strType == CommonString.ETC_LEL ||
            strType == CommonString.ETC_CONNECT) 
            return true;
        else
            return false;
    }

    public static string ChangeSensorType(string strSensorType)
    {
        string strRet = strSensorType;

        /*
        if (strSensorType == CommonString.ETC_TEMP_31007)
            strRet = CommonString.ETC_TEMP;
        else if (strSensorType == CommonString.ETC_HUMI_31007)
            strRet = CommonString.ETC_HUMI;
        else */
        if (strSensorType == CommonString.ETC_PM1_31007)
            strRet = CommonString.ETC_PM1;
        else if (strSensorType == CommonString.ETC_PM2_31007)
            strRet = CommonString.ETC_PM2;
        else if (strSensorType == CommonString.ETC_PM10_31007)
            strRet = CommonString.ETC_PM10;

        return strRet;
    }

    public static string GetSensorType(string strDeviceName)
    {
        if (0 == strDeviceName.IndexOf(CommonString.PSM_HF))
            return CommonString.PSM_HF;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_CO))
            return CommonString.PSM_CO;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_HCL))
            return CommonString.PSM_HCL;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_CH3C))
            return CommonString.PSM_CH3C;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_N2H4))
            return CommonString.PSM_N2H4;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_CA))
            return CommonString.PSM_CA;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_EA))
            return CommonString.PSM_EA;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_VOC))
            return CommonString.PSM_VOC;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_H2O2))
            return CommonString.PSM_H2O2;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_THC))
            return CommonString.PSM_THC;
        else if (0 == strDeviceName.IndexOf(CommonString.PSM_HNO3))
            return CommonString.PSM_HNO3;

        return null;
    }
}

public class DataDevice
{
    string m_strDeviceId = "";
    string m_strDeviceName = "";
    string m_strOrganizationName = "";
    string m_strStatus = "NORMAL";
    string m_strPlaceExt1 = "";
    string m_strPlaceExt2 = "";
    string m_strPlaceExt3 = "";
    string m_strPlaceAreaName = "";
    string m_strVersionId = "";
    List<DataSensor> m_listSensorData = null;

    public string DeviceId
    {
        get { return m_strDeviceId; }
        set { m_strDeviceId = value; }
    }

    public string DeviceName
    {
        get { return m_strDeviceName; }
        set { m_strDeviceName = value; }
    }

    public string OrganizationName
    {
        get { return m_strOrganizationName; }
        set { m_strOrganizationName = value; }
    }

    public string Status
    {
        get { return m_strStatus; }
        set { m_strStatus = value; }
    }

    public string PlaceExt1
    {
        get { return m_strPlaceExt1; }
        set { m_strPlaceExt1 = value; }
    }

    public string PlaceExt2
    {
        get { return m_strPlaceExt2; }
        set { m_strPlaceExt2 = value; }
    }

    public string PlaceExt3
    {
        get { return m_strPlaceExt3; }
        set { m_strPlaceExt3 = value; }
    }

    public string PlaceAreaName
    {
        get { return m_strPlaceAreaName; }
        set { m_strPlaceAreaName = value; }
    }

    public string VersionId
    {
        get { return m_strVersionId; }
        set { m_strVersionId = value; }
    }

    public List<DataSensor> SensorDataList
    {
        get { return m_listSensorData; }
        set { m_listSensorData = value; }
    }
}

public class DataSensor
{
    string m_strSensorId = "";
    string m_strSensorName = "";
    string m_strModelName = "";
    string m_strSensorStatus = "NORMAL";
    string m_strValue = "";

    public string SensorId
    {
        get { return m_strSensorId; }
        set { m_strSensorId = value; }
    }

    public string SensorName
    {
        get { return m_strSensorName; }
        set { m_strSensorName = value; }
    }

    public string ModelName
    {
        get { return m_strModelName; }
        set { m_strModelName = value; }
    }

    public string SensorStatus
    {
        get { return m_strSensorStatus; }
        set { m_strSensorStatus = value; }
    }

    public string Value
    {
        get { return m_strValue; }
        set { m_strValue = value; }
    }
}

public class AlarmData
{
    private string m_strDeviceId = "";
    private string m_strSensorId = "";
    private int m_nSensorType = -1;
    private int m_nSensorTagID = -1;
    private int m_nSensorZoneID = -1;
    private bool m_bIsAlarm = false;
    private string m_strUrl = "";

    private string m_strDeviceName = "";
    private int m_nOrgSensorID = -1;
    private string m_strSensorName = "";

    public string DeviceID
    {
        get { return m_strDeviceId; }
        set { m_strDeviceId = value; }
    }

    public string DeviceName
    {
        get { return m_strDeviceName; }
        set { m_strDeviceName = value; }
    }

    public string SensorID
    {
        get { return m_strSensorId; }
        set { m_strSensorId = value; }
    }

    public string SensorName
    {
        get { return m_strSensorName; }
        set { m_strSensorName = value; }
    }

    public int SensorType
    {
        get { return m_nSensorType; }
        set { m_nSensorType = value; }
    }

    public int SensorTagID
    {
        get { return m_nSensorTagID; }
        set { m_nSensorTagID = value; }
    }

    public int SensorZoneID
    {
        get { return m_nSensorZoneID; }
        set { m_nSensorZoneID = value; }
    }

    public bool IsAlarm
    {
        get { return m_bIsAlarm; }
        set { m_bIsAlarm = value; }
    }

    public string URL
    {
        get { return m_strUrl; }
        set { m_strUrl = value; }
    }

    public int OrgSensorID
    {
        get { return m_nOrgSensorID; }
        set { m_nOrgSensorID = value; }
    }
}

public class MemberData
{
    private string m_strID = "";
    private string m_strName = "";
    private string m_strBelongorgName = "";     // 소속 조직(공장)
    private string m_strTeamName = "";          // 소속 부서명
    private string m_strMobile = "";
    private string m_strEmail = "";

    public string ID
    {
        get { return m_strID; }
        set { m_strID = value; }
    }

    public string Name
    {
        get { return m_strName; }
        set { m_strName = value; }
    }

    public string BelongorgName
    {
        get { return m_strBelongorgName; }
        set { m_strBelongorgName = value; }
    }

    public string TeamName
    {
        get { return m_strTeamName; }
        set { m_strTeamName = value; }
    }

    public string Mobile
    {
        get { return m_strMobile; }
        set { m_strMobile = value; }
    }

    public string Email
    {
        get { return m_strEmail; }
        set { m_strEmail = value; }
    }
}


public class HrMemberData
{
    //private string m_strENTER_CD = "";      // 회사구분
    //private string m_strENTER_NM = "";      // 회사명
    private string m_strSABUN = "";         // 사번
    private string m_strNAME = "";          // 성명
    //private string m_strSEX_TYPE = "";      // 성별
    private string m_strORG_CD = "";        // 부서코드
    private string m_strORG_NM = "";        // 부서명
    private string m_strSTATUS_CD = "";     // 재직상태코드
    private string m_strSTATUS_NM = "";     // 재직상태명
    //private string m_strMANAGE_CD = "";     // 사원구분코드
    //private string m_strMANAGE_NM = "";     // 사원구분명
    private string m_strJIKWEE_CD = "";     // 직위코드
    private string m_strJIKWEE_NM = "";     // 직위명
    private string m_strJIKCHAK_CD = "";    // 직책코드
    private string m_strJIKCHAK_NM = "";    // 직책명
    private string m_strADDRESS_OT = "";    // 사내전화번호
    private string m_strADDRESS_HP = "";    // 핸드폰번호
    private string m_strADDRESS_IM = "";    // 메일주소

    //public string ENTER_CD
    //{
    //    get { return m_strENTER_CD; }
    //    set { m_strENTER_CD = value; }
    //}

    //public string ENTER_NM
    //{
    //    get { return m_strENTER_NM; }
    //    set { m_strENTER_NM = value; }
    //}

    public string SABUN
    {
        get { return m_strSABUN; }
        set { m_strSABUN = value; }
    }

    public string NAME
    {
        get { return m_strNAME; }
        set { m_strNAME = value; }
    }

    //public string SEX_TYPE
    //{
    //    get { return m_strSEX_TYPE; }
    //    set { m_strSEX_TYPE = value; }
    //}

    public string ORG_CD
    {
        get { return m_strORG_CD; }
        set { m_strORG_CD = value; }
    }

    public string ORG_NM
    {
        get { return m_strORG_NM; }
        set { m_strORG_NM = value; }
    }

    public string STATUS_CD
    {
        get { return m_strSTATUS_CD; }
        set { m_strSTATUS_CD = value; }
    }

    public string STATUS_NM
    {
        get { return m_strSTATUS_NM; }
        set { m_strSTATUS_NM = value; }
    }

    //public string MANAGE_CD
    //{
    //    get { return m_strMANAGE_CD; }
    //    set { m_strMANAGE_CD = value; }
    //}

    //public string MANAGE_NM
    //{
    //    get { return m_strMANAGE_NM; }
    //    set { m_strMANAGE_NM = value; }
    //}

    public string JIKWEE_CD
    {
        get { return m_strJIKWEE_CD; }
        set { m_strJIKWEE_CD = value; }
    }

    public string JIKWEE_NM
    {
        get { return m_strJIKWEE_NM; }
        set { m_strJIKWEE_NM = value; }
    }

    public string JIKCHAK_CD
    {
        get { return m_strJIKCHAK_CD; }
        set { m_strJIKCHAK_CD = value; }
    }

    public string JIKCHAK_NM
    {
        get { return m_strJIKCHAK_NM; }
        set { m_strJIKCHAK_NM = value; }
    }

    public string ADDRESS_OT
    {
        get { return m_strADDRESS_OT; }
        set { m_strADDRESS_OT = value; }
    }

    public string ADDRESS_HP
    {
        get { return m_strADDRESS_HP; }
        set { m_strADDRESS_HP = value; }
    }

    public string ADDRESS_IM
    {
        get { return m_strADDRESS_IM; }
        set { m_strADDRESS_IM = value; }
    }
}

public class HrTeamData
{
    private string m_strENTER_CD = "";
    private string m_strENTER_NM = "";
    private string m_strSDATE = "";
    private string m_strORG_CD = "";
    private string m_strORG_NM = "";
    private string m_strPRIOR_ORG_CD = null;
    private string m_strORDER_SEQ = null;
    private string m_strORG_LEVEL = null;
    private DateTime m_dtCHKDATE = new DateTime();
    private string m_strCHKID = null;

    public string ENTER_CD
    {
        get { return m_strENTER_CD; }
        set { m_strENTER_CD = value; }
    }

    public string ENTER_NM
    {
        get { return m_strENTER_NM; }
        set { m_strENTER_NM = value; }
    }

    public string SDATE
    {
        get { return m_strSDATE; }
        set { m_strSDATE = value; }
    }


    public string ORG_CD
    {
        get { return m_strORG_CD; }
        set { m_strORG_CD = value; }
    }

    public string ORG_NM
    {
        get { return m_strORG_NM; }
        set { m_strORG_NM = value; }
    }

    public string PRIOR_ORG_CD
    {
        get { return m_strPRIOR_ORG_CD; }
        set { m_strPRIOR_ORG_CD = value; }
    }

    public string ORDER_SEQ
    {
        get { return m_strORDER_SEQ; }
        set { m_strORDER_SEQ = value; }
    }

    public string ORG_LEVEL
    {
        get { return m_strORG_LEVEL; }
        set { m_strORG_LEVEL = value; }
    }

    public DateTime CHKDATE
    {
        get { return m_dtCHKDATE; }
        set { m_dtCHKDATE = value; }
    }

    public string CHKID
    {
        get { return m_strCHKID; }
        set { m_strCHKID = value; }
    }
}

public class HrRegular : RegularTeam
{
    //private List<HrRegular> m_childs = new List<HrRegular>();
    private string m_strORG_CD = null;
    //private string m_strPath = "";

    //public List<HrRegular> Childs
    //{
    //    get { return m_childs; }
    //    set { m_childs = value; }
    //}

    public string ORG_CD
    {
        get { return m_strORG_CD; }
        set { m_strORG_CD = value; }
    }

    //public string Path
    //{
    //    get { return m_strPath; }
    //    set { m_strPath = value; }
    //}
}

public class HrRegularMember : RegularMember
{
    private string m_strORG_CD = null;

    public string ORG_CD
    {
        get { return m_strORG_CD; }
        set { m_strORG_CD = value; }
    }
}

public class LogManager
{
    public bool Log_Info(string strMsg)
    {
        try
        {
            string strCheckFolder = "";
            string strFileName = "";
            //현재 EXE 파일가 위치 하고 있는 폴더를 가져옴. 
            string strLocal = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));

            //로그 폴더가 없으면 생성 
            strCheckFolder = strLocal + "\\Log";
            if (!System.IO.Directory.Exists(strCheckFolder))
            {
                System.IO.Directory.CreateDirectory(strCheckFolder);
            }

            strFileName = strCheckFolder + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            System.IO.StreamWriter FileWriter = new System.IO.StreamWriter(strFileName, true);
            FileWriter.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " => " + strMsg + "\r\n");
            FileWriter.Flush();
            FileWriter.Close();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool Log_SQL(string strMsg)
    {
        try
        {
            string strCheckFolder = "";
            string strFileName = "";
            //현재 EXE 파일가 위치 하고 있는 폴더를 가져옴. 
            string strLocal = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\"));

            //로그 폴더가 없으면 생성 
            strCheckFolder = strLocal + "\\Sql";
            if (!System.IO.Directory.Exists(strCheckFolder))
            {
                System.IO.Directory.CreateDirectory(strCheckFolder);
            }

            strFileName = strCheckFolder + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_SQL.sql";

            System.IO.StreamWriter FileWriter = new System.IO.StreamWriter(strFileName, true, Encoding.Default);
            FileWriter.Write(strMsg + "\r\n");
            FileWriter.Flush();
            FileWriter.Close();
        }
        catch
        {
            return false;
        }

        return true;
    }

}