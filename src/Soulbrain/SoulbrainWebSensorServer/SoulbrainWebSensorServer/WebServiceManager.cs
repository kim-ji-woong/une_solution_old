using dnsCommunicateSopServer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;

namespace SoulbrainWebSensorServer
{
    public class WebServiceManager
    {
        private string BaseAddress = "";
        private string m_strSoulbrainID = null;
        private string m_strSoulbrainPW = null;

        private string m_strToken = null;
        private string m_strRefreshtoken = null;

        private Dictionary<string, bool> m_dicAlarmLogChk = new Dictionary<string, bool>();

        Dictionary<string, DataDevice> m_dicDevices = new Dictionary<string, DataDevice>();
        public Dictionary<string, DataDevice> DicDevices
        {
            get { return m_dicDevices; }
        }

        private WSopDataManager m_wsopDataMgr = null;
        public WSopDataManager WSopDataMgr
        {
            set { m_wsopDataMgr = value; }
            get { return m_wsopDataMgr; }
        }

        private SopQueryManager m_SopQueryMgr = null;

        private List<AlarmData> m_listAlarm = new List<AlarmData>();

        Dictionary<string, MemberData> m_dicMembers = new Dictionary<string, MemberData>();
        public Dictionary<string, MemberData> DicMembers
        {
            get { return m_dicMembers; }
        }

        public WebServiceManager()
        {
            this.BaseAddress = ConfigurationManager.AppSettings.Get("WebServiceBaseURL");
            if (this.BaseAddress == null || this.BaseAddress.Length == 0)
                this.BaseAddress = "http://si.soulbrain.co.kr:8989";


            string strSoulbrainID = ConfigurationManager.AppSettings.Get("SOULBARIN_ID");
            if (strSoulbrainID == null || strSoulbrainID.Length == 0)
                strSoulbrainID = "T10692";

            string strSoulbrainPW = ConfigurationManager.AppSettings.Get("SOULBARIN_PW");
            if (strSoulbrainPW == null || strSoulbrainPW.Length == 0)
                strSoulbrainPW = "T10692";

            m_strSoulbrainID = strSoulbrainID;
            m_strSoulbrainPW = strSoulbrainPW;

            //RequestDeviceList();

            //RequestAllSensorData();

            m_SopQueryMgr = new SopQueryManager();

        }

        public bool RequestLogin()
        {
            // Login 요청 정보 작성
            string strURL = "/api/login";
            string strErrorMessage = null;

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            string strJson = "{\"userid\": \"" + m_strSoulbrainID + "\", \"password\":\"" + m_strSoulbrainPW + "\"}";

            // Login REST API 요청
            string strResult = SendQuery(dicHeaders, strJson, strURL, out strErrorMessage, "POST");

            if (strErrorMessage == "success")
            {
                // 로그인 성공 >> 토큰 저장
                JObject jResult = JObject.Parse(strResult);
                string strToken = jResult["token"].ToString();
                string strRefreshtoken = jResult["refreshtoken"].ToString();

                m_strToken = strToken;
                m_strRefreshtoken = strRefreshtoken;
            }
            else
            {
                m_strToken = null;
                m_strRefreshtoken = null;

                return false;
            }

            return true;
        }


        public bool RequestDeviceList()
        {
            // 로그인 실패로 인해서 토큰 값이 없음.
            if (m_strToken == null)
                return false;

            // Device List 요청 정보 작성
            string strURL = "/api/deviceext/list?size=1000";        // size 값은 한번 요청 시 확인할 device 갯수
            string strErrorMessage = null;

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("Authorization", "Bearer " + m_strToken);

            string strJson = null;

            // Device List REST API 요청
            string strResult = SendQuery(dicHeaders, strJson, strURL, out strErrorMessage);

            if (strErrorMessage == "success")
            {
                // 디바이스 조회 성공
                JObject jResult = JObject.Parse(strResult);
                JArray jArrDevices = (JArray)jResult["content"];

                // 조회된 디바이스가 없음
                if (jArrDevices == null || jArrDevices.Count == 0)
                    return false;

                List<DataDevice> ListDevices = new List<DataDevice>();

                // 디바이스 리스트 생성
                for (int i = 0; i < jArrDevices.Count; i++)
                {
                    JObject jDevice = (JObject)jArrDevices[i];
                    
                    // 제외된 디바이스 항목
                    // 파주 공장은 제외 또는 인디게이터,게이트웨이,레벨감지기_수신반 제외 또는 TEST Device 제외
                    if (jDevice["organizationName"].ToString().Trim() == CommonString.FACT_PAJU ||
                        jDevice["versionId"].ToString().Trim() == CommonString.VERSION_30063 ||
                        jDevice["versionId"].ToString().Trim() == CommonString.VERSION_32001 ||
                        jDevice["versionId"].ToString().Trim() == CommonString.VERSION_32003 ||
                        jDevice["deviceId"].ToString().Trim() == CommonString.TEST_DEVICE1 ||
                        jDevice["deviceId"].ToString().Trim() == CommonString.TEST_DEVICE2)
                        continue;

                    // DB에 생성된 센서 이외는 제외(디바이스 이름으로 구별)
                    List<string> ETCSensors = m_wsopDataMgr.ETCSensors;
                    List<string> PSMSensors = m_wsopDataMgr.PSMSensors;

                    // DB에 등록된 센서만
                    //if (!ETCSensors.Contains(jDevice["deviceId"].ToString().Trim()) &&
                    //    !PSMSensors.Contains(jDevice["deviceId"].ToString().Trim()))
                    //    continue;
                    // .TODO: 등록되지 않는 센서만 기록
                    //if (ETCSensors.Contains(jDevice["deviceId"].ToString().Trim()) ||
                    //    PSMSensors.Contains(jDevice["deviceId"].ToString().Trim()))
                    //    continue;


                    DataDevice device = null;

                    // 기존에 deviceId가 존재한다면 업데이트, 없다면 새로 생성
                    if (m_dicDevices.ContainsKey(jDevice["deviceId"].ToString().Trim()))
                    {
                        device = (DataDevice)m_dicDevices[jDevice["deviceId"].ToString().Trim()];
                        //device.DeviceId = jDevice["deviceId"].ToString().Trim();
                        device.DeviceName = jDevice["deviceName"].ToString().Trim();
                        device.OrganizationName = jDevice["organizationName"].ToString().Trim();
                        device.Status = jDevice["status"].ToString().Trim();
                        device.VersionId = jDevice["versionId"].ToString().Trim();
                        
                        if (jDevice["placeExt1"] != null && jDevice["placeExt2"] != null && jDevice["placeExt3"] != null)
                        {
                            device.PlaceExt1 = jDevice["placeExt1"].ToString().Trim();
                            device.PlaceExt2 = jDevice["placeExt2"].ToString().Trim();
                            device.PlaceExt3 = jDevice["placeExt3"].ToString().Trim();
                        }
                        if (jDevice["placeAreaName"] != null)
                        {
                            device.PlaceAreaName = jDevice["placeAreaName"].ToString().Trim();
                        }
                    }
                    else
                    {
                        device = new DataDevice();
                        device.DeviceId = jDevice["deviceId"].ToString().Trim();
                        device.DeviceName = jDevice["deviceName"].ToString().Trim();
                        device.OrganizationName = jDevice["organizationName"].ToString().Trim();
                        device.Status = jDevice["status"].ToString().Trim();
                        device.VersionId = jDevice["versionId"].ToString().Trim();

                        if (jDevice["placeExt1"] != null && jDevice["placeExt2"] != null && jDevice["placeExt3"] != null)
                        {
                            device.PlaceExt1 = jDevice["placeExt1"].ToString().Trim();
                            device.PlaceExt2 = jDevice["placeExt2"].ToString().Trim();
                            device.PlaceExt3 = jDevice["placeExt3"].ToString().Trim();
                        }
                        if (jDevice["placeAreaName"] != null)
                        {
                            device.PlaceAreaName = jDevice["placeAreaName"].ToString().Trim();
                        }

                        m_dicDevices[device.DeviceId] = device;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        // 단일 디바이스의 센서 데이터를 조회
        public bool RequestSensorData(DataDevice device, bool bChkAlarm = true)
        {
            // Device Sensor Data 요청 정보 작성
            string strURL = "/api/datarecordext/" + device.DeviceId + "/latest";
            string strErrorMessage = null;

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("Authorization", "Bearer " + m_strToken);

            string strJson = null;

            // Device Sensor Data REST API 요청
            string strResult = SendQuery(dicHeaders, strJson, strURL, out strErrorMessage);

            if (strErrorMessage == "success")
            {
                // Device Sensor Data 조회 성공
                JArray jArrSensor = JArray.Parse(strResult);
                List<DataSensor> listSensorData = new List<DataSensor>();

                // 조회된 Sensor Data가 없음
                if (jArrSensor == null || jArrSensor.Count == 0)
                    return false;

                // 버전에 따른 분류
                if (device.VersionId == CommonString.VERSION_30064 ||
                    device.VersionId == CommonString.VERSION_32004)
                {   // B 타입
                    DataSensor sensorData = new DataSensor();

                    for (int i = 0; i < jArrSensor.Count; i++)
                    {
                        JObject jSensor = (JObject)jArrSensor[i];

                        //DataSensor sensor = new DataSensor
                        //{
                        //    SensorId = jSensor["sensorId"].ToString().Trim(),
                        //    SensorName = jSensor["sensorName"].ToString().Trim(),
                        //    ModelName = jSensor["modelName"].ToString().Trim(),
                        //    SensorStatus = jSensor["sensorStatus"].ToString().Trim(),
                        //    Value = jSensor["value"].ToString().Trim()
                        //};
                        DataSensor sensor = new DataSensor();
                        sensor.SensorId = jSensor["sensorId"].ToString().Trim();
                        sensor.SensorName = jSensor["sensorName"].ToString().Trim();
                        sensor.ModelName = jSensor["modelName"].ToString().Trim();
                        sensor.SensorStatus = jSensor["sensorStatus"].ToString().Trim();
                        sensor.Value = jSensor["value"].ToString().Trim();

                        // 해당 센서 외에는 제외
                        //listSensorData.Add(sensor);

                        if (sensor.SensorName == CommonString.SENSOR_GAS_TYPE ||
                            sensor.SensorName == CommonString.SENSOR_KIND)
                        {
                            sensorData.SensorId = sensor.SensorId;
                            sensorData.SensorName = sensor.Value;

                            // H2 오타 경우 예외처리
                            if (sensorData.SensorName == "H")
                                sensorData.SensorName = CommonString.PSM_H2;
                        }
                        else if (sensor.SensorName == CommonString.ETC_Value)
                        {
                            sensorData.Value = sensor.Value;
                            sensorData.SensorStatus = sensor.SensorStatus;
                        }
                        //else if (sensor.SensorName == CommonString.ETC_CONNECT)
                        //{
                        //    // 알람 신호 체크
                        //    if (bChkAlarm)
                        //        CheckAlarmData(device, sensor);
                        //    // 해당 센서만 추가
                        //    listSensorData.Add(sensor);
                        //}
                    }

                    // 알람 신호 체크
                    if (bChkAlarm)
                        CheckAlarmData(device, sensorData);
                    // 해당 센서만 추가
                    listSensorData.Add(sensorData);
                }
                else if (device.VersionId == CommonString.VERSION_32005)
                {   // B-2 타입
                    DataSensor sensorData = new DataSensor();

                    for (int i = 0; i < jArrSensor.Count; i++)
                    {
                        JObject jSensor = (JObject)jArrSensor[i];

                        //DataSensor sensor = new DataSensor
                        //{
                        //    SensorId = jSensor["sensorId"].ToString().Trim(),
                        //    SensorName = jSensor["sensorName"].ToString().Trim(),
                        //    ModelName = jSensor["modelName"].ToString().Trim(),
                        //    SensorStatus = jSensor["sensorStatus"].ToString().Trim(),
                        //    Value = jSensor["value"].ToString().Trim()
                        //};
                        DataSensor sensor = new DataSensor();
                        sensor.SensorId = jSensor["sensorId"].ToString().Trim();
                        sensor.SensorName = jSensor["sensorName"].ToString().Trim();
                        sensor.ModelName = jSensor["modelName"].ToString().Trim();
                        sensor.SensorStatus = jSensor["sensorStatus"].ToString().Trim();
                        sensor.Value = jSensor["value"].ToString().Trim();

                        // 해당 센서 외에는 제외
                        //listSensorData.Add(sensor);

                        if (sensor.SensorName == CommonString.SENSOR_MEASURE)
                        {
                            sensorData.SensorId = sensor.SensorId;
                            sensorData.SensorName = sensor.Value;
                        }
                        else if (sensor.SensorName == CommonString.ETC_Value)
                        {
                            sensorData.Value = sensor.Value;
                            sensorData.SensorStatus = sensor.SensorStatus;
                        }
                        //else if (sensor.SensorName == CommonString.ETC_CONNECT ||
                        //    sensor.SensorName == CommonString.ETC_WATER_TEMP)
                        else if (sensor.SensorName == CommonString.ETC_WATER_TEMP)
                        {
                            // 알람 신호 체크
                            if (bChkAlarm)
                                CheckAlarmData(device, sensor);
                            // 해당 센서만 추가
                            listSensorData.Add(sensor);
                        }
                    }

                    // 알람 신호 체크
                    if (bChkAlarm)
                        CheckAlarmData(device, sensorData);
                    // 해당 센서만 추가
                    listSensorData.Add(sensorData);
                }
                else if (device.VersionId == CommonString.VERSION_30065)
                {   // B-3 타입
                    DataSensor sensorGAS1 = new DataSensor();
                    DataSensor sensorGAS2 = new DataSensor();
                    DataSensor sensorGAS3 = new DataSensor();
                    DataSensor sensorGAS4 = new DataSensor();
                    DataSensor sensorGAS5 = new DataSensor();

                    for (int i = 0; i < jArrSensor.Count; i++)
                    {
                        JObject jSensor = (JObject)jArrSensor[i];

                        //DataSensor sensor = new DataSensor
                        //{
                        //    SensorId = jSensor["sensorId"].ToString().Trim(),
                        //    SensorName = jSensor["sensorName"].ToString().Trim(),
                        //    ModelName = jSensor["modelName"].ToString().Trim(),
                        //    SensorStatus = jSensor["sensorStatus"].ToString().Trim(),
                        //    Value = jSensor["value"].ToString().Trim()
                        //};
                        DataSensor sensor = new DataSensor();
                        sensor.SensorId = jSensor["sensorId"].ToString().Trim();
                        sensor.SensorName = jSensor["sensorName"].ToString().Trim();
                        sensor.ModelName = jSensor["modelName"].ToString().Trim();
                        sensor.SensorStatus = jSensor["sensorStatus"].ToString().Trim();
                        sensor.Value = jSensor["value"].ToString().Trim();

                        if (sensor.SensorName == CommonString.SENSOR_GAS_NAME1)
                        {
                            sensorGAS1.SensorId = sensor.SensorId;
                            sensorGAS1.SensorName = sensor.Value;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_VAL1)
                        {
                            sensorGAS1.Value = sensor.Value;
                            sensorGAS1.SensorStatus = sensor.SensorStatus;
                        } 
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_NAME2)
                        {
                            sensorGAS2.SensorId = sensor.SensorId;
                            sensorGAS2.SensorName = sensor.Value;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_VAL2)
                        {
                            sensorGAS2.Value = sensor.Value;
                            sensorGAS2.SensorStatus = sensor.SensorStatus;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_NAME3)
                        {
                            sensorGAS3.SensorId = sensor.SensorId;
                            sensorGAS3.SensorName = sensor.Value;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_VAL3)
                        {
                            sensorGAS3.Value = sensor.Value;
                            sensorGAS3.SensorStatus = sensor.SensorStatus;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_NAME4)
                        {
                            sensorGAS4.SensorId = sensor.SensorId;
                            sensorGAS4.SensorName = sensor.Value;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_VAL4)
                        {
                            sensorGAS4.Value = sensor.Value;
                            sensorGAS4.SensorStatus = sensor.SensorStatus;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_NAME5)
                        {
                            sensorGAS5.SensorId = sensor.SensorId;
                            sensorGAS5.SensorName = sensor.Value;
                        }
                        else if (sensor.SensorName == CommonString.SENSOR_GAS_VAL5)
                        {
                            sensorGAS5.Value = sensor.Value;
                            sensorGAS5.SensorStatus = sensor.SensorStatus;
                        }
                    }

                    // 알람 신호 체크
                    if (bChkAlarm)
                    {
                        CheckAlarmData(device, sensorGAS1);
                        CheckAlarmData(device, sensorGAS2);
                        CheckAlarmData(device, sensorGAS3);
                        CheckAlarmData(device, sensorGAS4);
                        CheckAlarmData(device, sensorGAS5);
                    }

                    // 해당 센서만 추가
                    listSensorData.Add(sensorGAS1);
                    listSensorData.Add(sensorGAS2);
                    listSensorData.Add(sensorGAS3);
                    listSensorData.Add(sensorGAS4);
                    listSensorData.Add(sensorGAS5);
                }
                else if (device.VersionId == CommonString.VERSION_30061)
                {   // C 타입 - 스크러버
                    List<DataSensor> temps = new List<DataSensor>();

                    for (int i = 0; i < jArrSensor.Count; i++)
                    {
                        JObject jSensor = (JObject)jArrSensor[i];

                        //DataSensor sensor = new DataSensor
                        //{
                        //    SensorId = jSensor["sensorId"].ToString().Trim(),
                        //    SensorName = jSensor["sensorName"].ToString().Trim(),
                        //    ModelName = jSensor["modelName"].ToString().Trim(),
                        //    SensorStatus = jSensor["sensorStatus"].ToString().Trim(),
                        //    Value = jSensor["value"].ToString().Trim()
                        //};
                        DataSensor sensor = new DataSensor();
                        sensor.SensorId = jSensor["sensorId"].ToString().Trim();
                        sensor.SensorName = jSensor["sensorName"].ToString().Trim();
                        sensor.ModelName = jSensor["modelName"].ToString().Trim();
                        sensor.SensorStatus = jSensor["sensorStatus"].ToString().Trim();
                        sensor.Value = jSensor["value"].ToString().Trim();

                        //temps.Add(sensor);

                        if (sensor.SensorName == CommonString.ETC_Value)
                        {
                            DataSensor sensorData = new DataSensor();
                            sensorData.SensorId = sensor.SensorId;
                            sensorData.SensorName = CommonString.ETC_SCRUBBER;
                            sensorData.Value = sensor.Value;
                            sensorData.SensorStatus = sensor.SensorStatus;

                            // 알람 신호 체크
                            if (bChkAlarm)
                                CheckAlarmData(device, sensorData);
                            // 해당 센서만 추가
                            listSensorData.Add(sensorData);
                        }
                        else if (sensor.SensorName == CommonString.ETC_TEMP)
                        {
                            // 알람 신호 체크
                            if (bChkAlarm)
                                CheckAlarmData(device, sensor);
                            // 해당 센서만 추가
                            listSensorData.Add(sensor);
                        }
                    }

                    //foreach (DataSensor sensor in temps)
                    //{
                    //    if (sensor.SensorName == CommonString.ETC_Value)
                    //    {
                    //        DataSensor sensorData = new DataSensor();
                    //        sensorData.SensorId = sensor.SensorId;
                    //        sensorData.SensorName = CommonString.ETC_SCRUBBER;
                    //        sensorData.Value = sensor.Value;
                    //        sensorData.SensorStatus = sensor.SensorStatus;

                    //        // 알람 신호 체크
                    //        if (bChkAlarm)
                    //            CheckAlarmData(device, sensorData);
                    //        // 해당 센서만 추가
                    //        listSensorData.Add(sensorData);
                    //    }
                    //    else if (sensor.SensorName == CommonString.ETC_TEMP)
                    //    {
                    //        // 알람 신호 체크
                    //        if (bChkAlarm)
                    //            CheckAlarmData(device, sensor);
                    //        // 해당 센서만 추가
                    //        listSensorData.Add(sensor);
                    //    }

                    //}
                }
                else if (device.VersionId == CommonString.VERSION_32002)
                {   // C 타입 - HF
                    List<DataSensor> temps = new List<DataSensor>();

                    for (int i = 0; i < jArrSensor.Count; i++)
                    {
                        JObject jSensor = (JObject)jArrSensor[i];

                        //DataSensor sensor = new DataSensor
                        //{
                        //    SensorId = jSensor["sensorId"].ToString().Trim(),
                        //    SensorName = jSensor["sensorName"].ToString().Trim(),
                        //    ModelName = jSensor["modelName"].ToString().Trim(),
                        //    SensorStatus = jSensor["sensorStatus"].ToString().Trim(),
                        //    Value = jSensor["value"].ToString().Trim()
                        //};
                        DataSensor sensor = new DataSensor();
                        sensor.SensorId = jSensor["sensorId"].ToString().Trim();
                        sensor.SensorName = jSensor["sensorName"].ToString().Trim();
                        sensor.ModelName = jSensor["modelName"].ToString().Trim();
                        sensor.SensorStatus = jSensor["sensorStatus"].ToString().Trim();
                        sensor.Value = jSensor["value"].ToString().Trim();

                        // 해당 센서 외에는 제외
                        //temps.Add(sensor);

                        if (sensor.SensorName == CommonString.ETC_Value)
                        {
                            DataSensor sensorData = new DataSensor();
                            sensorData.SensorId = sensor.SensorId;
                            sensorData.SensorName = CommonString.PSM_HF;
                            sensorData.Value = sensor.Value;
                            sensorData.SensorStatus = sensor.SensorStatus;

                            // 알람 신호 체크
                            if (bChkAlarm)
                                CheckAlarmData(device, sensorData);
                            // 해당 센서만 추가
                            listSensorData.Add(sensorData);
                        }
                        else if (sensor.SensorName == CommonString.ETC_BATTERY ||
                            sensor.SensorName == CommonString.ETC_OPERATION)
                        {
                            // 알람 신호 체크
                            if (bChkAlarm)
                                CheckAlarmData(device, sensor);
                            // 해당 센서만 추가
                            listSensorData.Add(sensor);
                        }
                    }

                    //foreach (DataSensor sensor in temps)
                    //{
                    //    if (sensor.SensorName == CommonString.ETC_Value)
                    //    {
                    //        DataSensor sensorData = new DataSensor();
                    //        sensorData.SensorId = sensor.SensorId;
                    //        sensorData.SensorName = CommonString.PSM_HF;
                    //        sensorData.Value = sensor.Value;
                    //        sensorData.SensorStatus = sensor.SensorStatus;

                    //        // 알람 신호 체크
                    //        if (bChkAlarm)
                    //            CheckAlarmData(device, sensorData);
                    //        // 해당 센서만 추가
                    //        listSensorData.Add(sensorData);
                    //    }
                    //    else if (sensor.SensorName == CommonString.ETC_BATTERY ||
                    //        sensor.SensorName == CommonString.ETC_OPERATION)
                    //    {
                    //        // 알람 신호 체크
                    //        if (bChkAlarm)
                    //            CheckAlarmData(device, sensor);
                    //        // 해당 센서만 추가
                    //        listSensorData.Add(sensor);
                    //    }
                    //}
                }
                else
                {   // A 타입
                    for (int i = 0; i < jArrSensor.Count; i++)
                    {
                        JObject jSensor = (JObject)jArrSensor[i];

                        //DataSensor sensor = new DataSensor
                        //{
                        //    SensorId = jSensor["sensorId"].ToString().Trim(),
                        //    SensorName = jSensor["sensorName"].ToString().Trim(),
                        //    ModelName = jSensor["modelName"].ToString().Trim(),
                        //    SensorStatus = jSensor["sensorStatus"].ToString().Trim(),
                        //    Value = jSensor["value"].ToString().Trim()
                        //};
                        DataSensor sensor = new DataSensor();
                        sensor.SensorId = jSensor["sensorId"].ToString().Trim();
                        sensor.SensorName = jSensor["sensorName"].ToString().Trim();
                        sensor.ModelName = jSensor["modelName"].ToString().Trim();
                        sensor.SensorStatus = jSensor["sensorStatus"].ToString().Trim();
                        sensor.Value = jSensor["value"].ToString().Trim();

                        // VERSION_31007 디바이스의 센서 타입 명칭이 기존 명칭과 다르다. 변환 작업
                        if (device.VersionId == CommonString.VERSION_31007)
                            sensor.SensorName = CommonString.ChangeSensorType(sensor.SensorName);

                        // 알람 신호 체크
                        if (bChkAlarm)
                            CheckAlarmData(device, sensor);
                        // 해당 센서만 추가
                        listSensorData.Add(sensor);
                    }
                }                    

                device.SensorDataList = listSensorData;
            }
            else
                return false;

            return true;
        }

        private bool CheckAlarmData(DataDevice device, DataSensor sensor)
        {
            // TODO: 센서 알람 테스트
            //if (device.DeviceId == "BERRY40MG-00001" && sensor.SensorName == "TVOC")
            //{
            //    Console.WriteLine(device.DeviceName);
            //    sensor.SensorStatus = CommonString.STATUS_WARNING;
            //}

            // 현재 알람 조회
            List<AlarmData> alarms = m_wsopDataMgr.GetAlarmList();
            AlarmData alarm = null;

            //alarm = alarms.Find(x => x.DeviceName == device.DeviceName && x.SensorName == sensor.SensorName);
            if (alarms != null && alarms.Count > 0)
            {
                foreach (AlarmData temp in alarms)
                {
                    //if (temp.DeviceName == device.DeviceName && temp.SensorName == sensor.SensorName)
                    if (temp.DeviceID == (device.DeviceId + "_" + sensor.SensorName))
                    {
                        alarm = temp;
                        break;
                    }
                }
            }
            

            // 알람 리스트 중 복귀된 신호 확인
            if (alarm != null && (sensor.SensorStatus == CommonString.STATUS_NORMAL || sensor.SensorStatus == CommonString.STATUS_OFFLINE))
            {
                AlarmData alarmData = m_wsopDataMgr.GetAlarmData(device, sensor);
                if (alarmData != null)
                {
                    alarm.IsAlarm = false;

                    ArrayList arrData = new ArrayList();
                    arrData.Add(alarmData.SensorType);
                    arrData.Add(alarmData.SensorTagID);
                    arrData.Add(alarmData.SensorZoneID);
                    arrData.Add(alarmData.IsAlarm);

                    // 알람 신호 전송
                    // TODO: 현재 알람 단계 관련 데이터가 빠짐
                    m_SopQueryMgr.SendAlarmQuery(arrData, CommonString.ALARM_METHOD, alarmData.URL);
                    // 알람 로그 작성
                    WriteAlarmLog(device, sensor, false);
                }
            }

            // 알람 발생 시 (CAUTION, WARNING) >> 디버깅용,센서값,mA,접점,릴레이,가스종류,MAC,TYPE,GW_ID,종류,측정종류,기기상태,에러상태,통신상태 센서 제외 
            if ((sensor.SensorStatus == CommonString.STATUS_CAUTION || sensor.SensorStatus == CommonString.STATUS_WARNING)
            && sensor.ModelName != CommonString.MODEL_DEBUGGING
            && sensor.SensorName != CommonString.SENSOR_RESULT
            && sensor.SensorName != CommonString.ETC_mA
            && sensor.SensorName != CommonString.ETC_Contact
            && sensor.SensorName != CommonString.ETC_Relay
            && sensor.SensorName != CommonString.SENSOR_GAS_TYPE
            && sensor.SensorName != CommonString.ETC_CONNECT
            && sensor.SensorName != CommonString.SENSOR_MAC
            && sensor.SensorName != CommonString.SENSOR_TYPE
            && sensor.SensorName != CommonString.SENSOR_GW_ID
            && sensor.SensorName != CommonString.SENSOR_KIND
            && sensor.SensorName != CommonString.SENSOR_MEASURE
            && sensor.SensorName != CommonString.DEVICE_STATUS
            && sensor.SensorName != CommonString.SENSOR_ERROR
            && sensor.SensorName != CommonString.SENSOR_CH_NUM
            && sensor.SensorName != CommonString.ETC_BLE_Count)
            {
                // 알람 발생일 경우 여기서 판단하지 말고 일단 서버로 알람 전송
                AlarmData alarmData = m_wsopDataMgr.GetAlarmData(device, sensor);
                if (alarmData != null)
                {
                    alarmData.IsAlarm = true;

                    ArrayList arrData = new ArrayList();
                    arrData.Add(alarmData.SensorType);
                    arrData.Add(alarmData.SensorTagID);
                    arrData.Add(alarmData.SensorZoneID);
                    arrData.Add(alarmData.IsAlarm);

                    if (sensor.SensorStatus == CommonString.STATUS_CAUTION)
                    {
                        int nAlarmLevel = CommonString.LEVEL_CAUTION;
                        arrData.Add(nAlarmLevel);
                    }
                    else if (sensor.SensorStatus == CommonString.STATUS_WARNING)
                    {
                        int nAlarmLevel = CommonString.LEVEL_WARNING;
                        arrData.Add(nAlarmLevel);
                    }
                    else
                    {
                        int nAlarmLevel = CommonString.LEVEL_CAUTION;
                        arrData.Add(nAlarmLevel);
                    }

                    // 알람 신호 전송
                    m_SopQueryMgr.SendAlarmQuery(arrData, CommonString.ALARM_METHOD, alarmData.URL);
                    // 알람 로그 작성
                    WriteAlarmLog(device, sensor, true);
                }
            }
        
            return true;
        }

        private void WriteAlarmLog(DataDevice device, DataSensor sensor, bool bIsRun)
        {
            // 로그 체크
            string strUniqueKey = device.DeviceId + "_" + sensor.SensorName;

            string strSensorStatus = sensor.SensorStatus;

            string strAlarmLog = "";


            if (m_dicAlarmLogChk.ContainsKey(strUniqueKey) == false)
            {   // 처음 작성
                m_dicAlarmLogChk[strUniqueKey] = bIsRun;

                if (bIsRun == true)
                {   // 알람 발생
                    strAlarmLog = string.Format("{0} {1} 알람이 발생하였습니다.", strUniqueKey, strSensorStatus);
                }
                else
                {   // 알람 중지
                    strAlarmLog = string.Format("{0} 알람이 중지되었습니다.", strUniqueKey);
                }

                // 로그 작성
                Logger.Instance.Write(strAlarmLog);
            }
            else
            {
                bool bAlarmLogChk = m_dicAlarmLogChk[strUniqueKey];

                if (bIsRun != bAlarmLogChk)
                {   // 상태 변화
                    m_dicAlarmLogChk[strUniqueKey] = bIsRun;

                    if (bIsRun == true)
                    {   // 알람 발생
                        strAlarmLog = string.Format("{0} {1} 알람이 발생하였습니다.", strUniqueKey, strSensorStatus);
                    }
                    else
                    {   // 알람 중지
                        strAlarmLog = string.Format("{0} 알람이 중지되었습니다.", strUniqueKey);
                    }

                    // 로그 작성
                    Logger.Instance.Write(strAlarmLog);
                }
            }
        }

        // 조회된 모든 디바이스의 센서 데이터를 조회
        public bool RequestAllSensorData()
        {
            // 조회된 디바이스가 없음
            if (m_dicDevices == null || m_dicDevices.Count == 0)
                return false;

            foreach (KeyValuePair<string, DataDevice> pair in m_dicDevices)
            {
                DataDevice device = pair.Value;

                if (!RequestSensorData(device, false))
                    return false;
            }

            return true;
        }

        // 계정 리스트 조회
        public bool RequestAccountList()
        {
            // 로그인 실패로 인해서 토큰 값이 없음.
            if (m_strToken == null)
                return false;

            // Device List 요청 정보 작성
            string strURL = "/api/accountext/list";        
            string strErrorMessage = null;

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("Authorization", "Bearer " + m_strToken);

            string strJson = null;

            // Device List REST API 요청
            string strResult = SendQuery(dicHeaders, strJson, strURL, out strErrorMessage);

            if (strErrorMessage == "success")
            {
                // 계정 조회 성공
                JObject jResult = JObject.Parse(strResult);
                JArray jArrMembers = (JArray)jResult["content"];

                // 조회된 계정이 없음
                if (jArrMembers == null || jArrMembers.Count == 0)
                    return false;

                List<MemberData> ListMembers = new List<MemberData>();

                // 계정 리스트 생성
                for (int i = 0; i < jArrMembers.Count; i++)
                {
                    JObject jMember = (JObject)jArrMembers[i];

                    // 파주 공장은 제외
                    if (jMember["belongOrgName"].ToString().Trim() == CommonString.FACT_PAJU)
                        continue;

                    MemberData member = null;

                    member = new MemberData();
                    member.ID = jMember["userId"].ToString().Trim();
                    member.Name = jMember["userName"].ToString().Trim();
                    member.BelongorgName = jMember["belongOrgName"].ToString().Trim();
                    member.TeamName = jMember["teamName"].ToString().Trim();
                    member.Mobile = jMember["mobile"].ToString().Trim();
                    member.Email = jMember["email"].ToString().Trim();

                    m_dicMembers[member.ID] = member;
                }
            }
            else
            {
                return false;
            }

            return true;
        }


        private string SendQuery(Dictionary<string, string> dicHeaders, string strBodyJson, string strURL, out string strErrorMessage, string strMethodType = "GET")
        {
            strErrorMessage = "";
            string url = BaseAddress;

            if (strURL.StartsWith("/"))
                url += strURL;
            else
                url += "/" + strURL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = strMethodType;

            if (dicHeaders != null)
            {
                request.ContentType = "application/json; charset=utf-8";

                // 요청 헤더 추가
                foreach (KeyValuePair<string, string> pair in dicHeaders)
                {
                    string key = pair.Key;
                    string value = pair.Value;
                    request.Headers.Add(key, value);
                }
            }

            string strResponse = "";

            try
            {
                if (strBodyJson != null && strBodyJson != "")
                {
                    StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
                    streamWriter.Write(strBodyJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResponse = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

            }
            catch (WebException ex)
            {
                strErrorMessage = ex.Status.ToString();
                return "";
            }

            if (strResponse == null)
            {
                strErrorMessage = "Request 실패";
                return "";
            }

            strErrorMessage = "success";
            return strResponse;
        }
    }
}
