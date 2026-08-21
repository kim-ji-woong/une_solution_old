using CrisisAlertServer.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisisAlertServer.Network
{
    public class NetworkManager
    {
        private ClientProvider m_provider = new ClientProvider();
        private FormMain m_form = null;
        private DataManager m_dataMgr = null;

        private string m_NexmoreURL = null;
        private int m_NexmorePort = -1;

        private DateTime m_dtLastCheck = new DateTime();
        private double m_dCheckTime = 60;                     // 연결 체크 시간
        private int m_nHeader = 8;                              // 헤더 길이

        private bool m_isConnected = false;
        private Thread m_ConnThread = null;
        private Thread m_FloodThread = null;

        private static NetworkManager m_instance = null;
        public static NetworkManager Instance
        {
            get { return m_instance; }
        }

        // 강수량 관리 정보
        private FloodInfo m_floodInfo = new FloodInfo();

        private bool m_shutdownThread = false;
        public void Shutdown()
        {
            m_shutdownThread = true;
            m_ConnThread.Abort();
            m_FloodThread.Abort();
        }

        private bool m_startThread = false;
        public void StartThread()
        {
            m_startThread = true;
            m_form.ShowTextMessage("센서 데이터 서비스 시작");

        }
        public void StopThread()
        {
            m_startThread = false;
            m_isConnected = false;

            if (m_provider.IsConnected == true)
            {
                m_provider.Close();
            }

            m_form.ShowTextMessage("센서 데이터 서비스 종료");
        }

        //public NetworkManager(DataManager dataManager)
        public NetworkManager(FormMain form)
        {
            m_instance = this;
            m_provider.LengthAdd = false;

            m_form = form;
            m_dataMgr = form.DataManager;

            m_NexmoreURL = ConfigurationManager.AppSettings.Get("NexmoreURL");
            if (m_NexmoreURL == null || m_NexmoreURL.Length == 0)
                m_NexmoreURL = "121.161.186.85";

            string strNexmorePort = ConfigurationManager.AppSettings.Get("NexmorePort");
            if (strNexmorePort == null || strNexmorePort.Length == 0)
                m_NexmorePort = 49301;
            else
                m_NexmorePort = Int32.Parse(strNexmorePort);

            m_ConnThread = new Thread(new ThreadStart(ConnectionThread));
            m_ConnThread.Name = "Server.Connection";
            m_ConnThread.Start();

            m_FloodThread = new Thread(new ThreadStart(FloodCheckThread));
            m_FloodThread.Name = "FloodSensor.Check";
            m_FloodThread.Start();
        }

        private void ConnectionThread()
        {
            while (!m_shutdownThread)
            {
                if (m_startThread)
                {
                    if (m_provider.IsConnected == true)
                    {
                        if (m_isConnected)
                            CheckConnection();
                        else
                        {
                            m_provider.Close();
                            m_form.ShowTextMessage("Nexmore서버와 통신 이상으로 재연결 시도...");

                            ConnectSensorServer();
                        }

                        // 마지막 체크 시간을 비교하여 
                        TimeSpan span = DateTime.Now - m_dtLastCheck;

                        if (span.TotalSeconds > m_dCheckTime * 6)
                        {   // TODO: 6분 이상이라면 해제 >> 정상은 1분 >> 현재는 5분에 한번씩 신호가 들어옴.
                            m_isConnected = false;
                        }
                    }
                    else if (m_provider.IsConnected == false)
                    {
                        ConnectSensorServer();
                    }

                    Thread.Sleep(Convert.ToInt32(m_dCheckTime) * 1000);
                }
            }
        }

        private void FloodCheckThread()
        {   // TODO: 강수량 누적 쓰레드 작성
            while (!m_shutdownThread)
            {
                if (m_startThread)
                {
                    // 강수량 정보 체크
                    if (m_floodInfo.IsRaining == false)
                    {
                        foreach (RainInfo info in m_floodInfo.ListRainInfos)
                        {
                            // 현재 시간 차이 구하기
                            DateTime dtNow = DateTime.Now;
                            TimeSpan time = dtNow - info.CreateTime;

                            // 현재 비가 안오고 3시간 이상 경과된 데이터이라면 삭제
                            if (time.TotalMinutes >= 180)
                                m_floodInfo.ListRainInfos.Remove(info);
                        }
                    }

                    // 홍수 센서당 강우량 계산
                    CheckSensorFall();

                    Thread.Sleep(5 * 60000); // 5분
                }
            }
        }

        private void CheckSensorFall()
        {
            // 홍수 센서 불러오기
            Dictionary<int, FloodSensor> dicFloodSensors = m_dataMgr.DicFloodSensors;
            // 시간당 강우량 데이터 불러오기
            Dictionary<string, FloodSensorFallData> dicFloodSensorFalls = m_dataMgr.DicFloodSensorFalls;
            // 단계별 강우량 데이터 불러오기
            Dictionary<string, FloodSensorLevelData> dicFloodSensorLevels = m_dataMgr.DicFloodSensorLevels;


            // 시간당 강우량 데이터 조회 및 입력
            foreach (KeyValuePair<int, FloodSensor> pair in dicFloodSensors)
            {
                FloodSensor sensor = pair.Value;
                string strSensorID = sensor.SensorID;
                float fDepth = 0;   // 센서 강우량 변수

                // 해당 센서에 대한 시간당 강우량 데이터 없다면 
                if (!dicFloodSensorFalls.ContainsKey(strSensorID))
                    continue;

                // 현재 강우량 정보
                foreach(RainInfo info in m_floodInfo.ListRainInfos)
                {
                    // 강우량
                    int nFall = info.Fall;

                    // 현재 시간 차이 구하기
                    DateTime dtNow = DateTime.Now;
                    TimeSpan time = dtNow - info.CreateTime;
                    int nTime = Convert.ToInt32(FloodInfo.InitFallTime(time.TotalMinutes));

                    // 10분 경과 후 부터 강우량 체크
                    if (nTime == 0)
                        continue;

                    // 시간에 대한 강우량 구하기
                    FloodSensorFallData fallData = dicFloodSensorFalls[strSensorID];
                    TimeFallData timeFall = fallData.DicTimeFalls[nFall];
                    float fFall = timeFall.DicFalls[nTime];

                    // 해당 시간에 대한 강우량 더하기
                    fDepth += fFall;
                }

                string strState = RiskLevel.Normal.ToString();

                // 단계별 강우량 조회 및 상태 입력
                if (dicFloodSensorLevels.ContainsKey(strSensorID))
                    strState = dicFloodSensorLevels[strSensorID].CheckSensorLevel(fDepth);

                // 위기단계 변동 이력
                if (sensor.State != strState)
                    m_dataMgr.InsertAlertReport(FacilityType.FLOOD_SENSOR, sensor.ID, ChangeLevelTypeToKor(sensor.State), ChangeLevelTypeToKor(strState));

                if (sensor.Depth != fDepth || sensor.State != strState)
                {   // 센서 업데이트
                    sensor.Depth = fDepth;
                    sensor.State = strState;

                    m_dataMgr.UpdateFloodSensorData(sensor);
                }
                
            }
        }

        private void ConnectSensorServer()
        {
            if (m_provider.Connect(m_NexmoreURL, m_NexmorePort))
            {
                m_dtLastCheck = DateTime.Now;
                m_isConnected = true;

                m_form.ShowTextMessage("Nexmore서버 연결 성공");
            }
            else
            {
                m_form.ShowTextMessage("Nexmore서버 연결 실패, 관리자에게 문의해주세요.");
            }
        }

        public void OnReceive()
        {
            if (!m_startThread)
                return;

            byte[] bytes = m_provider.ReceivedData;
            if (bytes == null)
                return;

            int nBytesCount = bytes.Count();
            int nDataSize = 0;
            byte[] arrHeader = new byte[m_nHeader];
            byte[] data;
            MessageType type = MessageType.NONE;

            Array.Copy(bytes, 0, arrHeader, 0, m_nHeader);

            // 무슨 메시지인지 확인
            CheckHeader(arrHeader, out type, out nDataSize);
            // 응답
            ResponseData(type);

            if (type == MessageType.HEALTH)
            {
                // 수신 받아 체크시간 기록
                m_dtLastCheck = DateTime.Now;
                m_isConnected = true;
            }
            else if (type == MessageType.COLLAPSE)
            {
                // 경사지 붕괴 데이터 만들기
                data = new byte[nDataSize];
                Array.Copy(bytes, m_nHeader, data, 0, nDataSize);

                CollapseData collapse = SplitCollapseData(data);
                if (collapse == null)
                    return;

                // 경사지 붕괴 데이터 저장
                if (!m_dataMgr.InsertCollapseData(collapse))
                    return;

                // 센서 정보 가져오기 
                CollapseSensor sensor = m_dataMgr.GetCollapseSensor(collapse.SensorID);
                if (sensor == null)
                    return;

                // 센서 레벨값 비교
                string strOldLevel = sensor.State;
                string strNewLevel = SensorData.ChangeLevelNumToType(collapse.Level);

                if (CheckAlarmLevel(strOldLevel, strNewLevel))
                {
                    // 알람신호 발생
                    strNewLevel = ChangeLevelTypeToKor(strNewLevel);
                    m_dataMgr.InsertAlertAarm(FacilityType.COLLAPSE_SENSOR, sensor.ID, sensor.Addr, strNewLevel);
                }

                // 경계레벨 리포트
                if (strOldLevel != strNewLevel)
                    m_dataMgr.InsertAlertReport(FacilityType.COLLAPSE_SENSOR, sensor.ID, strOldLevel, strNewLevel);

                // 경사지 붕괴 센서 업데이트
                m_dataMgr.UpdateCollapseSensorData(collapse);
            }
            else if (type == MessageType.HEAT)
            {
                // 폭염 데이터 만들기
                data = new byte[nDataSize];
                Array.Copy(bytes, m_nHeader, data, 0, nDataSize);

                HeatData heatData = SplitHeatData(data);
                if (heatData == null)
                    return;

                // 폭염 데이터 저장
                if (!m_dataMgr.InsertHeatData(heatData))
                    return;

                // 센서 정보 가져오기 
                HeatSensor sensor = m_dataMgr.GetHeatSensor(heatData.GroupID, heatData.UniqueID);
                if (sensor == null)
                    return;

                // 센서 레벨값 비교
                string strOldLevel = sensor.State;
                string strNewLevel = SensorData.ChangeLevelNumToType(heatData.Grade.ToString());

                if (CheckAlarmLevel(strOldLevel, strNewLevel))
                {
                    // 알람신호 발생
                    strNewLevel = ChangeLevelTypeToKor(strNewLevel);
                    m_dataMgr.InsertAlertAarm(FacilityType.HEAT_SENSOR, sensor.ID, sensor.Addr, strNewLevel);
                }

                // 경계레벨 리포트
                if (strOldLevel != strNewLevel)
                    m_dataMgr.InsertAlertReport(FacilityType.HEAT_SENSOR, sensor.ID, strOldLevel, strNewLevel);

                // 폭염 센서 업데이트
                m_dataMgr.UpdateHeatSensorData(heatData);
            }
            else if (type == MessageType.FIRE)
            {
                // 화재 데이터 만들기
                data = new byte[nDataSize];
                Array.Copy(bytes, m_nHeader, data, 0, nDataSize);

                FireData fire = SplitFireData(data);
                if (fire == null)
                    return;

                // 화재 데이터 저장
                if (!m_dataMgr.InsertFireData(fire))
                    return;

                // 센서 정보 가져오기 
                FireSensor sensor = m_dataMgr.GetFireSensor(fire.BuildingId.ToString());
                if (sensor == null)
                    return;

                // 센서 레벨값 비교
                string strOldLevel = sensor.State;
                //string strNewLevel = ChangeLevelKorToType(fire.DangerStep);
                string strNewLevel = SensorData.ChangeLevelNumToType(fire.DangerStep);

                if (CheckAlarmLevel(strOldLevel, strNewLevel))
                {
                    // 알람신호 발생
                    strNewLevel = ChangeLevelTypeToKor(strNewLevel);
                    m_dataMgr.InsertAlertAarm(FacilityType.FIRE_SENSOR, sensor.ID, sensor.Addr, strNewLevel);
                }

                // 경계레벨 리포트
                if (strOldLevel != strNewLevel)
                    m_dataMgr.InsertAlertReport(FacilityType.COLLAPSE_SENSOR, sensor.ID, strOldLevel, strNewLevel);

                // 화재 센서 업데이트
                m_dataMgr.UpdateFireSensorData(fire);

            }
            else if (type == MessageType.FLOOD)
            {
                // 수위 데이터 만들기
                data = new byte[nDataSize];
                Array.Copy(bytes, m_nHeader, data, 0, nDataSize);

                // TODO: 수정 필요
                FloodNewData flood = SplitFloodData(data);
                if (flood == null)
                    return;

                // 수위 데이터 저장
                bool bChk = m_dataMgr.InsertFloodData(flood);
                if (!bChk)
                    return;

                // 서구 지역 강수량만 입력
                if (flood.DistrictCode != "271700")
                    return;

                // 강수량 입력
                bChk = m_floodInfo.AddRainInfo(flood.Fall);
                if (!bChk)
                    return;

                //bChk = m_dataMgr.CheckFloodSensorID(flood.SensorID);
                //// 홍수 센서 데이터 업데이트
                //if (bChk)
                //{
                //    // 해당 센서가 있으면 업데이트
                //    m_dataMgr.UpdateFloodSensorData(flood);
                //}
                //else
                //{
                //    // 해당 센서가 없으면 추가
                //    FloodSensor sensor = new FloodSensor();
                //    sensor.SensorID = flood.SensorID;
                //    sensor.MeasureTime = flood.MeasureTime;

                //    m_dataMgr.InsertFloodSensor(sensor);
                //}
            }
        }
        
        

        

        private bool CheckAlarmLevel(string strOldLevel, string strNewLevel)
        {
            bool bRet = false;

            if (strOldLevel == CommonString.RiskLevel_Normal && (strNewLevel == CommonString.RiskLevel_Attention || strNewLevel == CommonString.RiskLevel_Caution || strNewLevel == CommonString.RiskLevel_Alert || strNewLevel == CommonString.RiskLevel_Serious))
            {
                bRet = true;
            }
            else if (strOldLevel == CommonString.RiskLevel_Attention && (strNewLevel == CommonString.RiskLevel_Caution || strNewLevel == CommonString.RiskLevel_Alert || strNewLevel == CommonString.RiskLevel_Serious))
            {
                bRet = true;
            }
            else if (strOldLevel == CommonString.RiskLevel_Caution && (strNewLevel == CommonString.RiskLevel_Alert || strNewLevel == CommonString.RiskLevel_Serious))
            {
                bRet = true;
            }
            else if (strOldLevel == CommonString.RiskLevel_Alert && (strNewLevel == CommonString.RiskLevel_Serious))
            {
                bRet = true;
            }

            return bRet;
        }

        private string ChangeLevelTypeToKor(string strRiskLevel)
        {
            string strRiskKor = "";

            if (strRiskLevel == CommonString.RiskLevel_Normal)
                strRiskKor = CommonString.RiskLevel_Normal_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Attention)
                strRiskKor = CommonString.RiskLevel_Attention_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Caution)
                strRiskKor = CommonString.RiskLevel_Caution_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Alert)
                strRiskKor = CommonString.RiskLevel_Alert_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Serious)
                strRiskKor = CommonString.RiskLevel_Serious_Kor;

            return strRiskKor;
        }

        private FloodNewData SplitFloodData(byte[] data)
        {
            FloodNewData flood = null;

            if (data == null || data.Count() != 44)
                return flood;

            byte[] arrObserveTime = new byte[14];
            byte[] arrDistrictCode = new byte[10];
            byte[] arrFall = new byte[20];

            DateTime dtObserveTime;
            string strObserveTime = "";
            string strDistrictCode = "";
            string strFall = "";

            int nDataLength = 0;

            Array.Copy(data, 0, arrObserveTime, 0, arrObserveTime.Length);
            nDataLength += arrObserveTime.Length;
            Array.Copy(data, nDataLength, arrDistrictCode, 0, arrDistrictCode.Length);
            nDataLength += arrDistrictCode.Length;
            Array.Copy(data, nDataLength, arrFall, 0, arrFall.Length);

            char chNumber = '0';
            strObserveTime = Encoding.Default.GetString(arrObserveTime).Trim();
            strObserveTime.PadRight(14, chNumber);

            strDistrictCode = Encoding.Default.GetString(arrDistrictCode).Trim();
            strFall = Encoding.Default.GetString(arrFall).Trim();

            dtObserveTime = DateTime.ParseExact(strObserveTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            flood = new FloodNewData();
            flood.ObserveTime = dtObserveTime;
            flood.DistrictCode = strDistrictCode;
            flood.Fall = strFall;

            return flood;
        }

        private CollapseData SplitCollapseData(byte[] data)
        {
            CollapseData collapse = null;

            //if (data == null || data.Count() != 36)
            if (data == null || data.Count() != 31)
                return collapse;
            return collapse;

            byte[] arrSensorID = new byte[10];
            byte[] arrSlopeID = new byte[2];
            byte[] arrEvelDate = new byte[14];
            byte[] arrLevel = new byte[1];

            byte[] arrRainfall = new byte[4];

            string strSensorID = "";
            short nSlopeID = 0;
            string strEvelDate = "";
            string strLevel = "";

            string strRainfall = "";

            DateTime dtEvelDate;

            Array.Copy(data, 0, arrSensorID, 0, arrSensorID.Length);
            Array.Copy(data, arrSensorID.Length, arrSlopeID, 0, arrSlopeID.Length);
            Array.Copy(data, arrSensorID.Length + arrSlopeID.Length, arrEvelDate, 0, arrEvelDate.Length);
            Array.Copy(data, arrSensorID.Length + arrSlopeID.Length + arrEvelDate.Length, arrLevel, 0, arrLevel.Length);

            Array.Copy(data, arrSensorID.Length + arrSlopeID.Length + arrEvelDate.Length + arrLevel.Length, arrRainfall, 0, arrRainfall.Length);

            /*
            Array.Reverse(arrGroupID);
            Array.Reverse(arrSensorID);
            Array.Reverse(arrTime);
            Array.Reverse(arrMinute);
            Array.Reverse(arrLevel);
            */

            strSensorID = Encoding.Default.GetString(arrSensorID).Trim();
            nSlopeID = BitConverter.ToInt16(arrSlopeID, 0);
            strLevel = Encoding.Default.GetString(arrLevel).Trim();

            strRainfall = Encoding.Default.GetString(arrRainfall).Trim();

            char chNumber = '0';
            strEvelDate = Encoding.Default.GetString(arrEvelDate).Trim();
            strEvelDate.PadRight(14, chNumber);

            dtEvelDate = DateTime.ParseExact(strEvelDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            collapse = new CollapseData();
            collapse.SensorID = strSensorID;
            collapse.SlopeID = nSlopeID;
            collapse.EvelDate = dtEvelDate;
            collapse.Level = strLevel;

            collapse.Rainfall = strRainfall;

            return collapse;
        }

        private FireData SplitFireData(byte[] data)
        {
            FireData fire = null;

            if (data == null || data.Count() != 46)
                return fire;

            byte[] arrEventID = new byte[4];
            byte[] arrOccurType = new byte[5];
            byte[] arrOccurTime = new byte[14];
            byte[] arrLatitude = new byte[4];
            byte[] arrLongitude = new byte[4];
            byte[] arrDangerRange = new byte[4];
            byte[] arrDangerStep = new byte[5];
            byte[] arrBuildingId = new byte[4];
            byte[] arrEventFinishYn = new byte[2];

            int nEventID = 0;
            string strOccurType = "";
            string strOccurTime = "";
            string strLatitude = "";
            string strLongitude = "";
            float fLatitude = 0;
            float fLongitude = 0;
            int nDangerRange = 0;
            string strDangerStep = "";
            int nBuildingId = 0;
            short nEventFinishYn = 0;

            DateTime dtOccurTime;

            Array.Copy(data, 0, arrEventID, 0, arrEventID.Length);
            Array.Copy(data, arrEventID.Length, arrOccurType, 0, arrOccurType.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length, arrOccurTime, 0, arrOccurTime.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length + arrOccurTime.Length, arrLatitude, 0, arrLatitude.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length + arrOccurTime.Length + arrLatitude.Length, arrLongitude, 0, arrLongitude.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length + arrOccurTime.Length + arrLatitude.Length + arrLongitude.Length, arrDangerRange, 0, arrDangerRange.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length + arrOccurTime.Length + arrLatitude.Length + arrLongitude.Length + arrDangerRange.Length, arrDangerStep, 0, arrDangerStep.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length + arrOccurTime.Length + arrLatitude.Length + arrLongitude.Length + arrDangerRange.Length + arrDangerStep.Length, arrBuildingId, 0, arrBuildingId.Length);
            Array.Copy(data, arrEventID.Length + arrOccurType.Length + arrOccurTime.Length + arrLatitude.Length + arrLongitude.Length + arrDangerRange.Length + arrDangerStep.Length + arrBuildingId.Length, arrEventFinishYn, 0, arrEventFinishYn.Length);


            nEventID = BitConverter.ToInt32(arrEventID, 0);
            strOccurType = Encoding.Default.GetString(arrOccurType).Trim();

            char chNumber = '0';
            strOccurTime = Encoding.Default.GetString(arrOccurTime).Trim();
            strOccurTime.PadRight(14, chNumber);

            dtOccurTime = DateTime.ParseExact(strOccurTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            strLatitude = Encoding.Default.GetString(arrLatitude).Trim();
            strLongitude = Encoding.Default.GetString(arrLongitude).Trim();
            fLatitude = float.Parse(strLatitude);
            fLongitude = float.Parse(strLongitude);

            nDangerRange = BitConverter.ToInt32(arrDangerRange, 0);
            strDangerStep = Encoding.Default.GetString(arrDangerStep).Trim();
            nBuildingId = BitConverter.ToInt32(arrBuildingId, 0);
            nEventFinishYn = BitConverter.ToInt16(arrEventFinishYn, 0);

            fire = new FireData();
            fire.EventID = nEventID;
            fire.OccurType = strOccurType;
            fire.OccurTime = dtOccurTime;
            fire.Latitude = fLatitude;
            fire.Longitude = fLongitude;
            fire.DangerRange = nDangerRange;
            fire.DangerStep = strDangerStep;
            fire.BuildingId = nBuildingId;
            fire.EventFinishYn = nEventFinishYn;

            return fire;
        }

        private HeatData SplitHeatData(byte[] data)
        {
            HeatData heatData = null;

            //if (data == null || data.Count() != 58)
            if (data == null || data.Count() != 86)
                return heatData;

            byte[] arrEventID = new byte[4];
            byte[] arrGroupID = new byte[4];
            byte[] arrUniqueID = new byte[2];
            byte[] arrLatitude = new byte[8];
            byte[] arrLongitude = new byte[8];
            byte[] arrMeasureTime = new byte[14];
            byte[] arrTemperature = new byte[8];
            byte[] arrHumidity = new byte[8];
            //byte[] arrDust = new byte[10];
            //byte[] arrDirection = new byte[4];
            //byte[] arrVelocity = new byte[4];
            byte[] arrGrade = new byte[4];
            byte[] arrWorkStatus = new byte[4];
            byte[] arrPrevTemperature = new byte[8];
            byte[] arrRegDate = new byte[14];


            int nEventID = -1;
            int nGroupID = -1;
            short nUniqueID = -1;
            string strLatitude = "";
            string strLongitude = "";
            string strMeasureTime = "";
            string strTemperature = "";
            string strHumidity = "";
            //string strDust = "";
            //int nDirection = 0;
            //int nVelocity = 0;
            int nGrade = 0;
            int nWorkStatus = -1;
            string strPrevTemperature = "";
            string strRegDate = "";

            double dLatitude = 0;
            double dLongitude = 0;
            double dPrevTemperature = 0;

            DateTime dtMeasureTime;
            DateTime dtRegDate;

            int nDataLength = 0;

            Array.Copy(data, 0, arrEventID, 0, arrEventID.Length);
            nDataLength = arrEventID.Length;
            Array.Copy(data, nDataLength, arrGroupID, 0, arrGroupID.Length);
            nDataLength += arrGroupID.Length;
            Array.Copy(data, nDataLength, arrUniqueID, 0, arrUniqueID.Length);
            nDataLength += arrUniqueID.Length;
            Array.Copy(data, nDataLength, arrLatitude, 0, arrLatitude.Length);
            nDataLength += arrLatitude.Length;
            Array.Copy(data, nDataLength, arrLongitude, 0, arrLongitude.Length);
            nDataLength += arrLongitude.Length;
            Array.Copy(data, nDataLength, arrMeasureTime, 0, arrMeasureTime.Length);
            nDataLength += arrMeasureTime.Length;
            Array.Copy(data, nDataLength, arrTemperature, 0, arrTemperature.Length);
            nDataLength += arrTemperature.Length;
            Array.Copy(data, nDataLength, arrHumidity, 0, arrHumidity.Length);
            nDataLength += arrHumidity.Length;
            //Array.Copy(data, nDataLength, arrDust, 0, arrDust.Length);
            //nDataLength += arrDust.Length;
            //Array.Copy(data, nDataLength, arrDirection, 0, arrDirection.Length);
            //nDataLength += arrDirection.Length;
            //Array.Copy(data, nDataLength, arrVelocity, 0, arrVelocity.Length);
            Array.Copy(data, nDataLength, arrGrade, 0, arrGrade.Length);
            nDataLength += arrGrade.Length;
            Array.Copy(data, nDataLength, arrWorkStatus, 0, arrWorkStatus.Length);
            nDataLength += arrWorkStatus.Length;
            Array.Copy(data, nDataLength, arrPrevTemperature, 0, arrPrevTemperature.Length);
            nDataLength += arrPrevTemperature.Length;
            Array.Copy(data, nDataLength, arrRegDate, 0, arrRegDate.Length);
            nDataLength += arrRegDate.Length;


            nEventID = BitConverter.ToInt32(arrEventID, 0);
            nGroupID = BitConverter.ToInt32(arrGroupID, 0);
            nUniqueID = BitConverter.ToInt16(arrUniqueID, 0);
            strLatitude = Encoding.Default.GetString(arrLatitude).Trim();
            strLongitude = Encoding.Default.GetString(arrLongitude).Trim();
            strMeasureTime = Encoding.Default.GetString(arrMeasureTime).Trim();
            strTemperature = Encoding.Default.GetString(arrTemperature).Trim();
            strHumidity = Encoding.Default.GetString(arrHumidity).Trim();
            //strDust = Encoding.Default.GetString(arrDust).Trim();
            //nDirection = BitConverter.ToInt32(arrDirection, 0);
            //nVelocity = BitConverter.ToInt32(arrVelocity, 0);
            nGrade = BitConverter.ToInt32(arrGrade, 0);
            nWorkStatus = BitConverter.ToInt32(arrWorkStatus, 0);
            strPrevTemperature = Encoding.Default.GetString(arrPrevTemperature).Trim();
            strRegDate = Encoding.Default.GetString(arrRegDate).Trim();

            dLatitude = double.Parse(strLatitude);
            dLongitude = double.Parse(strLongitude);
            dPrevTemperature = double.Parse(strPrevTemperature);

            char chNumber = '0';
            strMeasureTime.PadRight(14, chNumber);
            strRegDate.PadRight(14, chNumber);

            dtMeasureTime = DateTime.ParseExact(strMeasureTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            dtRegDate = DateTime.ParseExact(strRegDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

            heatData = new HeatData();
            heatData.EventID = nEventID;
            heatData.GroupID = nGroupID;
            heatData.UniqueID = nUniqueID;
            heatData.Latitude = dLatitude;
            heatData.Longitude = dLongitude;
            heatData.MeasureTime = dtMeasureTime;
            heatData.Temperature = strTemperature;
            heatData.Humidity = strHumidity;
            //heatData.Dust = strDust;
            //heatData.Direction = nDirection;
            //heatData.Velocity = nVelocity;
            heatData.Grade = nGrade;
            heatData.WorkStatus = nWorkStatus;
            heatData.PrevTemperature = dPrevTemperature;
            heatData.RegDate = dtRegDate;

            return heatData;
        }

        private void CheckHeader(byte[] arrHeader, out MessageType type, out int nDataSize)
        {
            byte[] arrMsgCode = new byte[2];
            byte[] arrMsgSubCode = new byte[2];
            byte[] arrDataSize = new byte[4];

            short nMsgCode = 0;
            short nMsgSubCode = 0;

            Array.Copy(arrHeader, 0, arrMsgCode, 0, arrMsgCode.Length);
            Array.Copy(arrHeader, arrMsgCode.Length, arrMsgSubCode, 0, arrMsgSubCode.Length);
            Array.Copy(arrHeader, arrMsgCode.Length + arrMsgSubCode.Length, arrDataSize, 0, arrDataSize.Length);

            Array.Reverse(arrMsgCode);
            Array.Reverse(arrMsgSubCode);
            Array.Reverse(arrDataSize);

            nMsgCode = BitConverter.ToInt16(arrMsgCode, 0);
            nMsgSubCode = BitConverter.ToInt16(arrMsgSubCode, 0);
            nDataSize = BitConverter.ToInt32(arrDataSize, 0);

            if (nMsgCode == 12 && nMsgSubCode == 1)
            {
                type = MessageType.HEALTH;
            }
            else if (nMsgCode == 12 && nMsgSubCode == 2)
            {
                type = MessageType.COLLAPSE;
            }
            else if (nMsgCode == 12 && nMsgSubCode == 3)
            {
                type = MessageType.HEAT;
            }
            else if (nMsgCode == 12 && nMsgSubCode == 4)
            {
                type = MessageType.FIRE;
            }
            else if (nMsgCode == 12 && nMsgSubCode == 5)
            {
                type = MessageType.FLOOD;
            }
            else 
                type = MessageType.NONE;
        }



        public void OnDropConnection()
        {
            m_form.ShowTextMessage("센서서버 연결 끊김");
        }

        private void CheckConnection()
        {
            try
            {
                short nMsgCode = 11;
                short nMsgSubCode = 1;
                int nDataSize = 0;

                byte[] arrMsgCode = BitConverter.GetBytes(nMsgCode);
                byte[] arrMsgSubCode = BitConverter.GetBytes(nMsgSubCode);
                byte[] arrDataSize = BitConverter.GetBytes(nDataSize);

                Array.Reverse(arrMsgCode);
                Array.Reverse(arrMsgSubCode);
                Array.Reverse(arrDataSize);

                byte[] datas = new byte[arrMsgCode.Length + arrMsgSubCode.Length + arrDataSize.Length];

                Array.Copy(arrMsgCode, 0, datas, 0, arrMsgCode.Length);
                Array.Copy(arrMsgSubCode, 0, datas, arrMsgCode.Length, arrMsgSubCode.Length);
                Array.Copy(arrDataSize, 0, datas, arrMsgCode.Length + arrMsgSubCode.Length, arrDataSize.Length);

                m_provider.Client.Client.Send(datas, 0, datas.Length, SocketFlags.None);

            }
            catch (Exception)
            {
                return;
            }
        }

        private void ResponseData(MessageType type)
        {
            short nMsgCode = 0;
            short nMsgSubCode = 0;
            int nDataSize = 0;
            int nResult = 0;

            if (type == MessageType.FLOOD)
            {
                nMsgCode = 11;
                nMsgSubCode = 4;
                nDataSize = 4;
                nResult = 0;
            }
            else if (type == MessageType.COLLAPSE)
            {
                nMsgCode = 11;
                nMsgSubCode = 5;
                nDataSize = 4;
                nResult = 0;
            }
            else if (type == MessageType.COLLAPSE_LEVEL)
            {
                nMsgCode = 11;
                nMsgSubCode = 6;
                nDataSize = 4;
                nResult = 0;
            }
            else if (type == MessageType.HEAT)
            {
                nMsgCode = 11;
                nMsgSubCode = 7;
                nDataSize = 4;
                nResult = 0;
            }

            byte[] arrMsgCode = BitConverter.GetBytes(nMsgCode);
            byte[] arrMsgSubCode = BitConverter.GetBytes(nMsgSubCode);
            byte[] arrDataSize = BitConverter.GetBytes(nDataSize);
            byte[] arrResult = BitConverter.GetBytes(nResult);

            Array.Reverse(arrMsgCode);
            Array.Reverse(arrMsgSubCode);
            Array.Reverse(arrDataSize);
            Array.Reverse(arrResult);

            byte[] datas = new byte[arrMsgCode.Length + arrMsgSubCode.Length + arrDataSize.Length + arrResult.Length];

            Array.Copy(arrMsgCode, 0, datas, 0, arrMsgCode.Length);
            Array.Copy(arrMsgSubCode, 0, datas, arrMsgCode.Length, arrMsgSubCode.Length);
            Array.Copy(arrDataSize, 0, datas, arrMsgCode.Length + arrMsgSubCode.Length, arrDataSize.Length);
            Array.Copy(arrResult, 0, datas, arrMsgCode.Length + arrMsgSubCode.Length + arrDataSize.Length, arrResult.Length);

            try
            {
                m_provider.Client.Client.Send(datas, 0, datas.Length, SocketFlags.None);

            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
