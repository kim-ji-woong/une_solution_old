using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using DBUtility2;
using ServerProcess.Data;
using UnE.Earthquake;
using UnE.Sensor;

namespace ServerProcess.Client
{
    public class EarthquakeSensorServer : BaseClient
    {
        private static EarthquakeSensorServer m_instance = null;
        // 지진의 위험단계별 SOP가 현재 실행중인가?
        // m_sopBegins[0] : 1단계(주의 또는 예방) SOP가 실행중인가?
        private bool[] m_sopBegins = new bool[4] { false, false, false, false };
        // 지진의 위험단계별 SOP 실행을 SOP Simulator에게 요청한 시간
        // 같은 위험단계의 SOP를 연속해서 계속 SOP Simulator에게 요청하지 않도록 하기 위해서 요청 시간을 기록한다.
        private VariousData<DateTime>[] m_sopSend = new VariousData<DateTime>[4] { null, null, null, null };

        // 같은 위험단계에 대한 알람은 연속해서 보내지 않도록 한다.
        // 적어도 m_nDelaySeconds 만큼은 지난 다음에 같은 단계 데이터를 보낸다.
        private int m_nDelaySeconds = 15;
        // 마지막으로 진도 0보다 큰 값을 받은 시간
        private DateTime m_dtLastEvent = new DateTime();
        // 지진 알람을 종료시키기 위하여 진도 또는 규모 0의 값이 지속되어야 하는 최소 시간
        //private int m_nCloseAlarmSeconds = 18;
        private int m_nCloseAlarmSeconds = 1800;
        private AlarmData m_currentEarthquakeAlarm = null;

        public static EarthquakeSensorServer Instance
        {
            get { return m_instance; }
        }

        public EarthquakeSensorServer()
            : base()
        {
            m_instance = this;
        }

        public EarthquakeSensorServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.Earthquake);
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.EARTHQUAKE_SENSOR_SERVER; }
        }
        
        protected override void OnLoadEvent()
        {
            ReadOptionDatas();

            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SensorType == UnE.Sensor.IFacility.FacilityType.Earthquake)
                {
                    m_currentEarthquakeAlarm = alarm;
                    break;
                }
            }
        }

        private void ReadOptionDatas()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "Select DataType, CloseAlarmSeconds, DelaySeconds ";
            strSQL += string.Format("from OptionEtcSensor where SensorType = {0} and SiteID = {1}", (int)IFacility.FacilityType.Earthquake, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            dbMgr.Close();

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> dataType = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> closeAlarmSeconds = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> delaySeconds = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                // 지진 데이터가 진도인지 규모인지 판단
                if (dataType == null)
                    continue;

                if (closeAlarmSeconds != null)
                    m_nCloseAlarmSeconds = closeAlarmSeconds.Data;

                if (delaySeconds != null)
                    m_nDelaySeconds = delaySeconds.Data;

                break;
            }
        }

        protected override int OnReceiveEvent(ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT)
                return ProcessEarthquakeSensorDetect(arrDatas);
            else if (header == SOPWebServer.Header.SDMS_COMMAND)
                return SendDataToSDMSClient(header, arrDatas);
            else if (header == SOPWebServer.Header.COLLAPSE_BUILDING_DETECT)
                return SendDataToSDMSClient(header, arrDatas);
            else if (header == SOPWebServer.Header.SENSOR_USER_RESET)
                return ProcessUserReset(arrDatas);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ProcessUserReset(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                    return EarthquakeSensorServer.Instance.ProcessUserReset(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        public int ProcessUserReset(int nSensorZoneHistoryID, int nSensorZoneID, int nSOPGenUserID, string strDescription)
        {
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);
            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (sensorZone == null || group == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

            // 알람 해제
            AlarmData alarm = group.CurrentAlarm;
            int nResult = RemoveAlarm_UserReset(group, sensorZone, nSOPGenUserID);

            if (alarm != null && group.CurrentAlarm == null)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                alarm.Status = BaseProcessManager.ReactionType.USER_RESET;
                m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                dbMgr.Clone();

                m_currentEarthquakeAlarm = null;
            }

            return nResult;
        }

        // 탐지신호 사용자 복구
        private int RemoveAlarm_UserReset(SensorZoneGroup group, SensorZone sensorZone, int nSOPGenUserID)
        {
            // Transaction 처리를 위하여 객체를 새로 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            // 신호복구 처리는 SDMS에서 사용자에 의하여 보내기 때문에 특정 센서 뿐만 아니라
            // SensorZoneGroup내에 있는 모든 센서 데이터를 초기화 시킨다.
            if (group.RemoveAllSensorData(dbMgr) == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            // sensorZone의 신호는 복구되었지만 같은 영역에 다른 신호가 아직 남아있는 상황
            if (group.GetSensorDatas().Count > 0 && group.CurrentAlarm != null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            string strMessage = GetUserResetMessage(alarm.IsReal);
            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            string strParam1 = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
            string strParam2 = sensorZone.ID.ToString();
            string strParam3 = nSOPGenUserID.ToString();

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.USER_RESET, strMessage, strParam1, strParam2, strParam3, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private string GetUserResetMessage(bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();
                return strTag + "탐지된 지진신호가 복구되었습니다.";
            }

            return "[테스트]탐지된 지진신호가 복구되었습니다.";
        }

        private string GetTrainingModeString()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return "";

            string strTag = m_agentFactory.SMSManager.GetTrainingModeString(dbMgr);
            dbMgr.Close();
            return strTag;
        }

        private int SendDataToSDMSClient(int header, ArrayList arrDatas)
        {
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(header, bytes, SOPWebServer.ClientType.SDMS, -1);
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessEarthquakeSensorDetect(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 6 && arrDatas[0] is int && arrDatas[1] is float && arrDatas[2] is int && arrDatas[3] is string && arrDatas[4] is long && arrDatas[5] is bool)
            {
                int nSensorZoneID = (int)arrDatas[0];
                float fMagnitude = (float)arrDatas[1];
                int nIntensity = (int)arrDatas[2];
                //int nAlarmLevel = (int)arrDatas[3];
                string strPosition = (string)arrDatas[3];
                VariousData<DateTime> time = (long)arrDatas[4] == 0 ? null : new VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[4]));
                bool isReal = (bool)arrDatas[5];

                VariousData<float> magnitude = fMagnitude < 0.0f ? null : new VariousData<float>(fMagnitude);
                VariousData<int> intensity = nIntensity < 0 ? null : new VariousData<int>(nIntensity);

                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                {
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;
                }

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                {
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;
                }

                // 지진 알람을 종료해도 되는지 여부를 판단한다.
                // 진도 또는 규모 0의 값이 m_nCloseAlarmSeconds 이상 지속되면 지진 알람을 종료시킨다.
                CheckCloseEarthquakeAlarm(magnitude, intensity);
                
                List<EarthquakeOption> options = LoadOptions();
                EarthquakeOption option = EarthquakeOption.GetOption(nIntensity, fMagnitude, options);

                if (option == null)
                {
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    int nAlarmDepth = options.IndexOf(option) + 1;

                    // nAlarmDepth에 해당하는 SOP가 이미 실행중인가?(더 높은 SOP 포함)
                    if (CheckBeginSOP(nAlarmDepth) == false)
                    {
                        // 같은 위험단계 또는 상위 위험단계에 대한 지진 알람을 연속으로 발생시키지 않도록 한다.
                        // 적어도 m_nDelaySeconds 이상은 지났는지 확인한다.
                        if (CheckSOPRequestTime(nAlarmDepth))
                        {
                            if (option.RunSOP)
                                m_sopSend[nAlarmDepth - 1] = new VariousData<DateTime>(DateTime.Now);

                            // 알람 발생
                            AlarmData alarm, prevAlarm;
                            int nResult = AddAlarm(group, sensorZone, intensity, magnitude, nAlarmDepth, isReal, out alarm, out prevAlarm);

                            DirectDBManager dbMgr = m_dbMgr.Clone();

                            if (dbMgr.Connect() == false)
                            {
                                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                            }

                            if (alarm == null)
                                return nResult;

                            alarm.Tag = option;
                            m_dtLastEvent = DateTime.Now;

                            if (alarm != null && prevAlarm != null)
                            {
                                m_agentFactory.ProcessManager.ChangeAlarm(dbMgr, alarm, prevAlarm);
                            }
                            else if (alarm != null)
                            {
                                m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                            }

                            dbMgr.Close();

                            arrDatas.Add(alarm.SensorZoneHistoryID);

                            PostAlarm(arrDatas, option);
                            System.Diagnostics.Trace.WriteLine("지진알람 : " + nAlarmDepth);
                            return nResult;
                        }
                    }

                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        // 지진 알람을 종료해도 되는지 여부를 판단한다.
        // 진도 또는 규모 0의 값이 m_nCloseAlarmSeconds 이상 지속되면 지진 알람을 종료시킨다.
        private void CheckCloseEarthquakeAlarm(VariousData<float> magnitude, VariousData<int> intensity)
        {
            AlarmData currentAlarm = m_currentEarthquakeAlarm;

            if (currentAlarm == null)
                return;

            if (magnitude != null)
            {
                if (magnitude.Data > 0.0f)
                {
                    m_dtLastEvent = DateTime.Now;
                    return;
                }
            }
            else if (intensity != null)
            {
                if (intensity.Data > 0)
                {
                    m_dtLastEvent = DateTime.Now;
                    return;
                }
            }
            else
                return;

            TimeSpan span = DateTime.Now - m_dtLastEvent;

            if (span.TotalSeconds >= m_nCloseAlarmSeconds)
            {
                ProcessUserReset(currentAlarm.SensorZoneHistoryID, currentAlarm.SensorZoneID, -1, "");
            }
        }

        // nAlarmDepth 또는 그 보다 높은 알람단계에 해당하는 SOP 실행을 SOP Simulator에게 마지막으로 요청한 시간이
        // 적어도 m_nDelaySeconds 만큼은 지났는지 확인한다.
        private bool CheckSOPRequestTime(int nAlarmDepth)
        {
            int nIndex = nAlarmDepth - 1;
            int nStepCount = m_sopSend.Count();
            DateTime dtNow = DateTime.Now;

            for (int i=nIndex;i<nStepCount;i++)
            {
                VariousData<DateTime> time = m_sopSend[i];

                if (time == null)
                    continue;

                TimeSpan span = dtNow - time.Data;

                if (span.TotalSeconds < m_nDelaySeconds)
                    return false;
            }

            return true;
        }

        // nAlarmDepth에 해당하는 SOP가 실행중인가?
        // 아니면 nAlarmDepth보다 더 강한 지진단계에 대한 SOP가 실행중인가?
        private bool CheckBeginSOP(int nAlarmDepth)
        {
            int nIndex = nAlarmDepth - 1;
            int nStepCount = m_sopBegins.Count();

            for (int i=nStepCount-1;i>=0 && i>=nIndex;i--)
            {
                if (m_sopBegins[i])
                    return true;
            }

            return false;
        }

        private void PostAlarm(ArrayList arrDatas, UnE.Earthquake.EarthquakeOption option)
        {
            if (option.LinkedSOP.Length > 0 && option.RunSOP)
            {
                ArrayList datas = new ArrayList();

                datas.AddRange(arrDatas);
                datas.Add(option.RunSOP);
                datas.Add(option.LinkedSOP);

                byte[] bytes2 = SOPWebServer.BinaryHelper.MakeBytes(datas);
                SOPSimulatorManager.ServerInstance.SendClientData(SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT, bytes2, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
                //SOPSimulatorServer.Instance.SendClientData(SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT, bytes2, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
                //SendClientData(SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT, bytes2, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
            }
        }

        private int AddAlarm(SensorZoneGroup group, SensorZone sensorZone, VariousData<int> intensity, VariousData<float> magnitude, int nAlarmDepth, bool isReal, out AlarmData alarm, out AlarmData prevAlarm)
        {
            prevAlarm = alarm = null;
            bool isIntensity = intensity != null;

            int nSensorData = 0;

            if (intensity != null)
                nSensorData = intensity.Data;
            else if (magnitude != null)
            {
                // 규모일 경우 1000을 곱해서 넣는다.
                nSensorData = (int)(magnitude.Data * 1000);
            }
            else
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            AlarmData currentAlarm = group.CurrentAlarm;
            int nSensorDataCount = group.GetSensorDatas().Count;

            if (currentAlarm == null && nSensorDataCount > 0)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                //  논리적인 오류
                group.ClearSensorDatas(dbMgr);
                dbMgr.Close();
            }
            else if (currentAlarm != null && nSensorDataCount > 0)
            {
                // 이미 알람이 발생중이다.
                // 알람단계가 바뀌었는지 확인한다.
                List<KeyValuePair<SensorZone, int>> sensorZoneDatas = group.GetSensorDatas();
                int alarmDepthChanged, sensorDataChanged;

                if (IsChangedAlarmDepth(sensorZoneDatas, currentAlarm.AlarmDepth, sensorZone, nSensorData, nAlarmDepth, out alarmDepthChanged, out sensorDataChanged))
                {
                    if (alarmDepthChanged > 0)
                    {
                        // 알람단계가 바뀌었다.
                        prevAlarm = currentAlarm;
                        alarm = prevAlarm.Clone();

                        alarm.TimeStamp = DateTime.Now;
                        alarm.AlarmDepth = nAlarmDepth;
                        alarm.Status = BaseProcessManager.ReactionType.CHANGE_ALARM_DEPTH;
                        alarm.SensorZoneID = sensorZone.ID;
                        alarm.Message = GetChangeAlarmDepthString(alarm.AlarmDepth, prevAlarm.AlarmDepth, nSensorData, isReal, isIntensity);

                        // Transaction 처리를 위하여 객체를 새로 만든다.
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        if (dbMgr.BeginBatch() == false)
                        {
                            dbMgr.Close();
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                        }

                        group.SetSensorData(sensorZone, nAlarmDepth, dbMgr, true);
                        AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);

                        string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                        string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                        ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                        VariousData<int> status = new VariousData<int>((int)detectionStatus);

                        string strParam3 = ((int)sensorZone.Type).ToString();
                        string strParam4 = GetEarthquakeDataString(nSensorData, isIntensity);

                        if (AlarmManager.Instance.AddReactionHistory(alarm, (int)alarm.Status, alarm.TimeStamp, alarm.Message, strEquipZoneID, sensorZone.ID.ToString(), strParam3, strParam4, alarm.AlarmDepth.ToString(), status, dbMgr, true))
                        {
                            if (dbMgr.BatchCommit())
                            {
                                dbMgr.Close();
                                group.CurrentAlarm = alarm;
                                AlarmManager.Instance.SetAlarm(alarm.SensorZoneHistoryID, alarm);
                                return SOPWebServer.ErrorMessageType.SUCCESS;
                            }
                            else
                            {
                                dbMgr.BatchRollback();
                                dbMgr.Close();
                                group.SetSensorData(sensorZone, prevAlarm.AlarmDepth, null, false);
                                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                            }
                        }
                        else
                        {
                            dbMgr.BatchRollback();
                            dbMgr.Close();
                            group.SetSensorData(sensorZone, prevAlarm.AlarmDepth, null, false);
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                        }
                    }
                    else if (sensorDataChanged > 0)
                    {
                        // 지진값이 바뀌었다.
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        // 알람단계가 바뀌지 않았으므로 바뀐 Sensor 데이터만 SDMS에게 알려준다.
                        group.SetSensorData(sensorZone, nSensorData, dbMgr, false);
                        AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.SUCCESS;
                    }
                }
                else
                {
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
            }
            else
            {
                // group 영역에 대하여 발생한 알람이 없다.
                // Transaction 처리를 위하여 객체를 새로 만든다.
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                if (dbMgr.BeginBatch() == false)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                group.SetSensorData(sensorZone, nSensorData, dbMgr, true);

                DateTime timeStamp = DateTime.Now;
                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, nSensorData, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = nAlarmDepth;
                    group.CurrentAlarm = alarm;

                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = GetDetectEarthquakeMessage(nAlarmDepth, nSensorData, isReal, isIntensity);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    string strEquipZoneName = group.EquipmentZone == null ? "" : group.EquipmentZone.ZoneName;
                    ProcessManager.ReactionType reactionType = ProcessManager.ReactionType.BEGIN_STATUS;

                    string strParam3 = ((int)sensorZone.Type).ToString();
                    string strParam4 = GetEarthquakeDataString(nSensorData, isIntensity);

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), strParam3, strParam4, alarm.AlarmDepth.ToString(), status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            dbMgr.Close();
                            alarm.Message = strMessage;
                            alarm.IsReal = isReal;
                            alarm.Status = reactionType;

                            m_currentEarthquakeAlarm = alarm;
                            return SOPWebServer.ErrorMessageType.SUCCESS;
                        }
                        else
                        {
                            group.RemoveSensorData(sensorZone, null);
                            group.CurrentAlarm = null;
                        }
                    }
                    else
                    {
                        group.RemoveSensorData(sensorZone, dbMgr);
                        AlarmManager.Instance.RemoveAlarm(alarm);
                        alarm = null;
                        dbMgr.BatchRollback();
                    }
                }
                else
                {
                    group.RemoveSensorData(sensorZone, dbMgr);
                    dbMgr.BatchRollback();
                }

                dbMgr.Close();
            }

            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private string GetDetectEarthquakeMessage(int nAlarmDepth, int nSensorData, bool isReal, bool isIntensity)
        {
            string strTag = isReal ? "" : "[테스트]";
            string strData = GetEarthquakeDataString(nSensorData, isIntensity);
            return strTag + string.Format("{0}의 지진이 감지되었습니다.\r\n알람 {1}단계가 발동되었습니다.", strData, nAlarmDepth);
        }

        private string GetChangeAlarmDepthString(int nAlarmDepth, int nPrevAlarmDepth, int nSensorData, bool isReal, bool isIntensity)
        {
            string strTag = isReal ? "" : "[테스트]";
            string strData = GetEarthquakeDataString(nSensorData, isIntensity);
            string strUpDown = nAlarmDepth > nPrevAlarmDepth ? "상향" : "하향";
            return strTag + string.Format("{0}의 지진이 감지되었습니다.\r\n알람단계가 {1}단계에서 {2}단계로 {3}되었습니다.", strData, nPrevAlarmDepth, nAlarmDepth, strUpDown);
        }

        private string GetEarthquakeDataString(int nData, bool isIntensity)
        {
            if (isIntensity)
                return string.Format("진도 {0}", nData);

            return string.Format("규모 {0:F1}", nData / 1000.0);
        }

        // alarmDepthChange : 0(변화없음), 1(이전보다 알람단계가 더 높아졌음 => 더 위험해졌음), 2(이전보다 알람단계가 더 낮아졌음 => 덜 위험해졌음)
        // sensorDataChange : 0(변화없음), 1(이전보다 지진 데이터가 더 높아졌음 => 더 위험해졌음), 2(이전보다 지진 데이터가 더 낮아졌음 => 덜 위험해졌음)
        private bool IsChangedAlarmDepth(List<KeyValuePair<SensorZone, int>> sensorZoneDatas, int nPrevAlarmDepth, SensorZone sensorZone, int nSensorData, int nAlarmDepth, out int alarmDepthChange, out int sensorDataChange)
        {
            int nPrevData = -1;
            
            foreach (KeyValuePair<SensorZone, int> pair in sensorZoneDatas)
            {
                if (nPrevData < 0)
                    nPrevData = pair.Value;
                else
                {
                    if (nPrevData < pair.Value)
                        nPrevData = pair.Value;
                }
            }

            alarmDepthChange = sensorDataChange = 0;

            if (nAlarmDepth == nPrevAlarmDepth && nPrevData == nSensorData)
                return false;

            if (nAlarmDepth != nPrevAlarmDepth)
            {
                if (nAlarmDepth > nPrevAlarmDepth)
                    alarmDepthChange = 1;
                else
                    alarmDepthChange = 2;
            }

            if (nSensorData != nPrevData)
            {
                if (nSensorData > nPrevData)
                    sensorDataChange = 1;
                else
                    sensorDataChange = 2;
            }

            return true;
        }

        // strPosition : 진앙지
        private void RunBroadcast(string strMessage, int nIntensity, float fMagnitude, string strPosition, string strShelterName)
        {
            if (strMessage == null)
                return;

            strMessage = strMessage.Trim();

            if (strMessage.Length == 0)
                return;

            if (strShelterName.Length == 0)
            {
                UnE.Spatial.ZoneManager.Instance.LoadShelters();
                Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(UnE.Spatial.Shelter.ShelterTypes.Earthquake);

                strShelterName = "대피소";

                if (dicShelters != null)
                {
                    foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
                    {
                        strShelterName = pair.Value.ShelterName;
                        break;
                    }
                }
            }

            ReplaceString("{INTENS}", nIntensity.ToString(), ref strMessage);
            ReplaceString("{MAGNIT}", string.Format("{0:F1}", fMagnitude), ref strMessage);
            ReplaceString("{SHELTER}", strShelterName, ref strMessage);
            
            m_agentFactory.BroadcastManager.RunBroadcast(m_dbMgr, strMessage, 1, true);
        }

        // strPosition : 진앙지
        private void SendSMS(string strMessage, int nIntensity, float fMagnitude, string strPosition, ref string strShelterName)
        {
            if (strMessage == null)
                return;

            strMessage = strMessage.Trim();

            if (strMessage.Length == 0)
                return;

            UnE.Spatial.ZoneManager.Instance.LoadShelters();
            Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(UnE.Spatial.Shelter.ShelterTypes.Earthquake);

            strShelterName = "대피소";

            if (dicShelters != null)
            {
                foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
                {
                    strShelterName = pair.Value.ShelterName;
                    break;
                }
            }

            ReplaceString("{INTENS}", nIntensity.ToString(), ref strMessage);
            ReplaceString("{MAGNIT}", string.Format("{0:F1}", fMagnitude), ref strMessage);
            ReplaceString("{SHELTER}", strShelterName, ref strMessage);

            List<ServerProcess.Data.DataCompanyMember> members = MemberManager.Instance.GetAllRegularMember();
            List<string> phoneNumbers = new List<string>();
            foreach (ServerProcess.Data.DataCompanyMember item in members)
            {
                phoneNumbers.Add(item.PhoneNumber);
            }

            m_agentFactory.SMSManager.SendSMS(m_dbMgr, "", phoneNumbers, strMessage, -1);
        }

        // string.Replace()는 대소문자를 엄격히 구별하여 사용하여야 한다.
        // 대소문자 구별없이 같은 기능을 수행한다.
        private void ReplaceString(string strSrc, string strTrg, ref string strMessage)
        {
            int nSrcLen = strSrc.Length;
            strSrc = strSrc.ToLower();

            string strLow = strMessage.ToLower();

            int nIndex = 0;

            do
            {
                nIndex = strLow.IndexOf(strSrc, nIndex);

                if (nIndex >= 0)
                {
                    strLow = strLow.Substring(0, nIndex) + strTrg + strLow.Substring(nIndex + nSrcLen);
                    strMessage = strMessage.Substring(0, nIndex) + strTrg + strMessage.Substring(nIndex + nSrcLen);
                }
            }
            while (nIndex >= 0);
        }

        private List<EarthquakeOption> LoadOptions()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return null;

            string strSQL = "Select MinIntens, MaxIntens, IntensOption, UseSMS, SMSMessage, UseBroadcast, BroadcastMessage from OptionEarthquake";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                dbMgr.Clone();
                return null;
            }

            List<EarthquakeOption> options = new List<EarthquakeOption>();
            int nResultData = arrResult.Count;

            for (int i = 0; i < nResultData - 6; i += 7)
            {
                VariousData<float> min = WebDBManager.GetFloatField(arrResult[i].ToString());
                VariousData<float> max = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<int> option = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> useSMS = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSMS = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> useBroadcast = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strBroadcast = WebDBManager.GetStringField(arrResult[i + 6]);
                //VariousData<int> runSOP = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                //string strLinkedSOP = WebDBManager.GetStringField(arrResult[i + 8]);

                if (min == null || max == null || option == null || useSMS == null || useBroadcast == null/* || runSOP == null*/)
                    continue;

                EarthquakeOption opt = new EarthquakeOption();
                opt.Minimum = min.Data;
                opt.Maximum = max.Data;
                opt.SetMinMaxOption(option.Data);
                opt.UseSMS = useSMS.Data == 1 ? true : false;
                opt.SMSMessage = strSMS == null ? "" : strSMS;
                opt.UseBroadcast = useBroadcast.Data == 1 ? true : false;
                opt.BroadcastMessage = strBroadcast == null ? "" : strBroadcast;
                //opt.RunSOP = runSOP.Data == 1 ? true : false;
                //opt.LinkedSOP = strLinkedSOP == null ? "" : strLinkedSOP;

                options.Add(opt);
            }

            options.Sort();
            dbMgr.Clone();
            return options;
        }

        // 새로운 지진 SOP가 시작되었음을 통보받는다.
        public void OnBeginSOP(string strDisasterCategoryName, string strSubDisasterCategoryName, string strDisasterName)
        {
            List<EarthquakeOption> options = LoadOptions();

            if (options == null)
                return;

            string strSOP = strDisasterCategoryName + "/" + strSubDisasterCategoryName + "/" + strDisasterName;

            int nStepCount = m_sopBegins.Count();
            int nOptionCount = options.Count;

            for (int i=0;i<nOptionCount && i < nStepCount;i++)
            {
                EarthquakeOption option = options[i];

                if (option.LinkedSOP == strSOP)
                {
                    m_sopBegins[i] = true;
                    break;
                }
            }
        }

        // 지진 SOP가 종료되었음을 통보받는다.
        public void OnFinishSOP(string strDisasterCategoryName, string strSubDisasterCategoryName, string strDisasterName)
        {
            List<EarthquakeOption> options = LoadOptions();

            if (options == null)
                return;

            string strSOP = strDisasterCategoryName + "/" + strSubDisasterCategoryName + "/" + strDisasterName;

            int nStepCount = m_sopBegins.Count();
            int nOptionCount = options.Count;

            for (int i = 0; i < nOptionCount && i < nStepCount; i++)
            {
                EarthquakeOption option = options[i];

                if (option.LinkedSOP == strSOP)
                {
                    m_sopBegins[i] = false;
                    m_sopSend[i] = null;
                    break;
                }
            }
        }
    }
}
