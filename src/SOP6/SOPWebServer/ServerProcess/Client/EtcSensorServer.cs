using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using System.ServiceModel;
using System.Collections;
using DBUtility2;
using ServerProcess.Data;
using UnE.Sensor;
using UnE.Spatial;
using System.Collections.Concurrent;

namespace ServerProcess.Client
{
    public class EtcSensorServer : BaseClient
    {
        private class SOPRunningData
        {
            // 위험단계별 SOP가 현재 실행중인가?
            // m_sopBegins[0] : 1단계(주의 또는 예방) SOP가 실행중인가?
            private bool[] m_sopBegins = new bool[4] { false, false, false, false };
            // 위험단계별 SOP 실행을 SOP Simulator에게 요청한 시간
            // 같은 위험단계의 SOP를 연속해서 계속 SOP Simulator에게 요청하지 않도록 하기 위해서 요청 시간을 기록한다.
            private VariousData<DateTime>[] m_sopSend = new VariousData<DateTime>[4] { null, null, null, null };

            public int StepCount
            {
                get { return m_sopBegins.Count(); }
            }

            public bool IsRunning(int nIndex)
            {
                if (nIndex < 0 || nIndex >= StepCount)
                    return false;

                return m_sopBegins[nIndex];
            }

            public void SetRunning(int nIndex, bool isRunning)
            {
                if (nIndex < 0 || nIndex >= StepCount)
                    return;

                m_sopBegins[nIndex] = isRunning;
            }

            public VariousData<DateTime> GetSendTime(int nIndex)
            {
                if (nIndex < 0 || nIndex >= StepCount)
                    return null;

                return m_sopSend[nIndex];
            }

            public void SetSendTime(int nIndex, VariousData<DateTime> timeStamp)
            {
                if (nIndex < 0 || nIndex >= StepCount)
                    return;

                m_sopSend[nIndex] = timeStamp;
            }
        }

        private Dictionary<IFacility.FacilityType, OptionEtcSensor> m_optionSensorData = new Dictionary<IFacility.FacilityType, OptionEtcSensor>();
        // 센서별로 마지막으로 알람을 발생시킨 시간
        // Key : SensorZoneID
        private Dictionary<int, DateTime> m_dicSensorLastEvent = new Dictionary<int, DateTime>();
        // 센서별 SOP 실행현황
        private Dictionary<int, SOPRunningData> m_dicSenorSOPBegins = new Dictionary<int, SOPRunningData>();

        private static EtcSensorServer m_instance = null;
        private bool m_initialized = false;

        public static EtcSensorServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.ETC; }
        }

        public EtcSensorServer()
            : base()
        {
            m_instance = this;
        }

        public EtcSensorServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.Etc);
        }

        protected override void OnLoadEvent()
        {
            ReadOptionDatas();
            ReadPrevAlarmSOP();
            m_initialized = true;
        }

        // 이전에 발생했던 알람에 대한 SOP 실행여부를 확인한다.
        private void ReadPrevAlarmSOP()
        {
            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;
            string strSensorZoneHistoryIDs = "";

            foreach (AlarmData alarm in alarms)
            {
                if (IFacility.IsETCSensorType(alarm.SensorType))
                {
                    if (strSensorZoneHistoryIDs.Length == 0)
                        strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                    else
                        strSensorZoneHistoryIDs += ", " + alarm.SensorZoneHistoryID.ToString();
                }
            }

            if (strSensorZoneHistoryIDs.Length > 0)
            {
                string strSQL = "Select ID, SensorZoneHistoryID ";
                strSQL += "from ActionStepHistory ";
                strSQL += "where SensorZoneHistoryID in (" + strSensorZoneHistoryIDs + ") ";

                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return;

                ArrayList arrResult = dbMgr.GetResultData(strSQL);
                dbMgr.Close();

                if (arrResult != null)
                {
                    int nResultCount = arrResult.Count;

                    for (int i = 0; i < nResultCount - 1; i += 2)
                    {
                        VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                        VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                        
                        if (actionStepHistoryID == null || sensorZoneHistoryID == null)
                            continue;

                        AlarmData alarm = AlarmManager.Instance.GetAlarm(sensorZoneHistoryID.Data);

                        if (alarm != null)
                        {
                            alarm.SOPProcess = AlarmData.SOPProcessType.Run;
                        }
                    }
                }
            }
        }

        // EtcSensor들의 옵션 정보를 읽어온다.
        private void ReadOptionDatas()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "Select sensor.SensorType, sensor.DataType, sensor.CloseAlarmSeconds, sensor.DelaySeconds, data.DataMini, data.DataMinf, data.DataMins, data.DataMaxi, data.DataMaxf, data.DataMaxs, data.LinkedBuildingID, data.LinkedZoneID, data.AlarmDepth, data.SendSDMS ";//, data.RunSOP, data.LinkedSOP ";
            strSQL += "from OptionEtcSensor as sensor, OptionEtcSensorData as data where sensor.SensorType = data.SensorTypeID and sensor.SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            dbMgr.Close();

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-13;i+=14)
            {
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> dataType = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> closeAlarmSeconds = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> delaySeconds = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> mini = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<float> minf = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                string mins = WebDBManager.GetStringField(arrResult[i + 6]);
                VariousData<int> maxi = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<float> maxf = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                string maxs = WebDBManager.GetStringField(arrResult[i + 9]);
                string strLinkedBuildingIDs = WebDBManager.GetStringField(arrResult[i + 10]);
                string strLinkedZoneIDs = WebDBManager.GetStringField(arrResult[i + 11]);
                VariousData<int> alarmDepth = WebDBManager.GetIntField(arrResult[i + 12].ToString());
                VariousData<int> sendSDMS = WebDBManager.GetIntField(arrResult[i + 13].ToString());
                //VariousData<int> runSOP = WebDBManager.GetIntField(arrResult[i + 14].ToString());
                //string linkedSOP = WebDBManager.GetStringField(arrResult[i + 15]);

                if (sensorType == null || dataType == null || alarmDepth == null || sendSDMS == null/* || runSOP == null || linkedSOP == null*/)
                    continue;

                IFacility.FacilityType facilityType = IFacility.ToFacilityType(sensorType.Data);

                if (facilityType == IFacility.FacilityType.NONE)
                    continue;

                OptionEtcSensor.DataType type;

                if (OptionEtcSensor.ToDataType(dataType.Data, out type) == false)
                    continue;

                OptionEtcData data = new OptionEtcData();

                if (type == OptionEtcSensor.DataType.IntType)
                {
                    if (mini == null || maxi == null)
                        continue;

                    data.MinDatai = mini;
                    data.MaxDatai = maxi;
                }
                else if (type == OptionEtcSensor.DataType.FloatType)
                {
                    if (minf == null || maxf == null)
                        continue;

                    data.MinDataf = minf;
                    data.MaxDataf = maxf;
                }
                else if (type == OptionEtcSensor.DataType.StringType)
                {
                    if (mins == null || maxs == null)
                        continue;

                    data.MinDatas = mins;
                    data.MaxDatas = maxs;
                }

                data.AlarmDepth = alarmDepth.Data;
                //data.LinkedSOP = linkedSOP;
                //data.RunSOP = runSOP.Data == 1;
                data.SendSDMS = sendSDMS.Data == 1;
                data.LinkedBuildingIDs = OptionEtcData.ToIDList(strLinkedBuildingIDs);
                data.LinkedZoneIDs = OptionEtcData.ToIDList(strLinkedZoneIDs);

                OptionEtcSensor etcSensor = null;

                if (m_optionSensorData.TryGetValue(facilityType, out etcSensor) == false)
                {
                    etcSensor = new OptionEtcSensor();
                    m_optionSensorData[facilityType] = etcSensor;

                    etcSensor.CloseAlarmSeconds = closeAlarmSeconds;
                    etcSensor.SensorType = facilityType;
                    etcSensor.SensorDataType = type;
                    etcSensor.DelaySeconds = delaySeconds;
                }

                etcSensor.AddOptionData(alarmDepth.Data, data);
            }
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] bytes, ArrayList arrDatas)
        {
            // 초기화되기 전에는 통신 데이터를 처리하지 않는다.
            if (m_initialized == false || SOPSimulatorManager.ServerInstance.Initialized == false)
                return SOPWebServer.ErrorMessageType.SUCCESS;

            if (header == SOPWebServer.Header.ETC_SENSOR_DETECT)
                return ProcessSensorDataByStrongwind(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATA)
                return ProcessSensorData(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATA_TEST)
                return ProcessSensorData(arrDatas, false);
            else if (header == SOPWebServer.Header.ETC_SENSOR_DATA_STRING)
                return ProcessSensorDatas(arrDatas, true);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ProcessSensorDatas(ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 5 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int && arrDatas[4] is string)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int alarmOn = (int)arrDatas[3];
                string strSensorData = (string)arrDatas[4];

                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                if (alarmOn == 1)
                {
                    // 알람 발생
                    AlarmData alarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, isReal, out alarm);

                    if (alarm != null)
                    {
                        alarm.Message = strSensorData;
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;
                    int nResult = RemoveAlarm(group, sensorZone, isReal);

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        alarm.Status = BaseProcessManager.ReactionType.END_STATUS;

                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessSensorData(ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int nSensorData = (int)arrDatas[3];

                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                if (nSensorData > 0)
                {
                    // 알람 발생
                    AlarmData alarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, isReal, out alarm);

                    if (alarm != null)
                    {
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;
                    int nResult = RemoveAlarm(group, sensorZone, isReal);

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        alarm.Status = BaseProcessManager.ReactionType.END_STATUS;

                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessSensorDataByStrongwind(ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 5 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[4] is long)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                //int nSensorData = (int)arrDatas[3];
                DateTime timeStamp = DateTime.FromBinary((long)arrDatas[4]);

                OptionEtcSensor optionData;

                if (GetEtcSensorOption(nSensorType, out optionData) == false)
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                if (optionData.SensorDataType == OptionEtcSensor.DataType.IntType)
                {
                    if (arrDatas[3] is int)
                    {
                        int nSensorData = (int)arrDatas[3];
                        return ProcessSensorData(nSensorType, nSensorTagID, nSensorZoneID, nSensorData, isReal, timeStamp, optionData);
                    }
                }
                else if (optionData.SensorDataType == OptionEtcSensor.DataType.FloatType)
                {
                    if (arrDatas[3] is float)
                    {
                        float fSensorData = (float)arrDatas[3];
                        return ProcessSensorData(nSensorType, nSensorTagID, nSensorZoneID, fSensorData, isReal, timeStamp, optionData);
                    }
                }
                else if (optionData.SensorDataType == OptionEtcSensor.DataType.StringType)
                {
                    if (arrDatas[3] is string)
                    {
                        string strSensorData = (string)arrDatas[3];
                        return ProcessSensorData(nSensorType, nSensorTagID, nSensorZoneID, strSensorData, isReal, timeStamp, optionData);
                    }
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessSensorData(int nSensorType, int nSensorTagID, int nSensorZoneID, int nSensorData, bool isReal, DateTime timeStamp, OptionEtcSensor optionData)
        {
            SensorZoneGroup group;
            SensorZone sensorZone;
            int nResult;

            if (ProcessSensorDataPrev(nSensorZoneID, out group, out sensorZone, out nResult) == false)
                return nResult;

            // 알람을 종료해도 되는지 여부를 판단한다.
            // nSensorData의 값이 optionData.MinAlarmData 미만의 값으로 optionData.CloseAlarmSeconds 이상 유지하면 알람을 종료시킨다.
            CheckCloseAlarm(nSensorData, nSensorZoneID, optionData);

            int nBuildingID = -1, nZoneID = -1;
            GetBuildingNZoneIDFromSensorZone(sensorZone, ref nBuildingID, ref nZoneID);

            OptionEtcData data = optionData.GetData(nSensorData, nBuildingID, nZoneID);
            return ProcessSensorDataPost(group, sensorZone, nSensorType, nSensorTagID, nSensorZoneID, isReal, timeStamp, nSensorData, optionData, data);
        }

        private int ProcessSensorData(int nSensorType, int nSensorTagID, int nSensorZoneID, float fSensorData, bool isReal, DateTime timeStamp, OptionEtcSensor optionData)
        {
            SensorZoneGroup group;
            SensorZone sensorZone;
            int nResult;

            if (ProcessSensorDataPrev(nSensorZoneID, out group, out sensorZone, out nResult) == false)
                return nResult;

            // 알람을 종료해도 되는지 여부를 판단한다.
            // nSensorData의 값이 optionData.MinAlarmData 미만의 값으로 optionData.CloseAlarmSeconds 이상 유지하면 알람을 종료시킨다.
            CheckCloseAlarm(fSensorData, nSensorZoneID, optionData);

            int nBuildingID = -1, nZoneID = -1;
            GetBuildingNZoneIDFromSensorZone(sensorZone, ref nBuildingID, ref nZoneID);

            OptionEtcData data = optionData.GetData(fSensorData, nBuildingID, nZoneID);
            return ProcessSensorDataPost(group, sensorZone, nSensorType, nSensorTagID, nSensorZoneID, isReal, timeStamp, fSensorData, optionData, data);
        }

        private int ProcessSensorData(int nSensorType, int nSensorTagID, int nSensorZoneID, string strSensorData, bool isReal, DateTime timeStamp, OptionEtcSensor optionData)
        {
            SensorZoneGroup group;
            SensorZone sensorZone;
            int nResult;

            if (ProcessSensorDataPrev(nSensorZoneID, out group, out sensorZone, out nResult) == false)
                return nResult;

            // 알람을 종료해도 되는지 여부를 판단한다.
            // nSensorData의 값이 optionData.MinAlarmData 미만의 값으로 optionData.CloseAlarmSeconds 이상 유지하면 알람을 종료시킨다.
            CheckCloseAlarm(strSensorData, nSensorZoneID, optionData);

            int nBuildingID = -1, nZoneID = -1;
            GetBuildingNZoneIDFromSensorZone(sensorZone, ref nBuildingID, ref nZoneID);

            OptionEtcData data = optionData.GetData(strSensorData, nBuildingID, nZoneID);
            return ProcessSensorDataPost(group, sensorZone, nSensorType, nSensorTagID, nSensorZoneID, isReal, timeStamp, strSensorData, optionData, data);
        }

        private void GetBuildingNZoneIDFromSensorZone(SensorZone sensorZone, ref int nBuildingID, ref int nZoneID)
        {
            if (sensorZone != null)
            {
                nZoneID = sensorZone.ZoneID;

                if (sensorZone.EquipZone != null && sensorZone.EquipZone.Building != null)
                    nBuildingID = sensorZone.EquipZone.Building.ID;
            }
        }

        private int ProcessSensorDataPost(SensorZoneGroup group, SensorZone sensorZone, int nSensorType, int nSensorTagID, int nSensorZoneID, bool isReal, DateTime timeStamp, object sensorData, OptionEtcSensor optionData, OptionEtcData data)
        {
            if (data == null)
                return SOPWebServer.ErrorMessageType.SUCCESS;

            // nAlarmDepth에 해당하는 SOP가 이미 실행중인가?(더 높은 SOP 포함)
            if (CheckBeginSOP(nSensorZoneID, data.AlarmDepth) == false)
            {
                SOPRunningData sopRunningData;

                if (m_dicSenorSOPBegins.TryGetValue(nSensorZoneID, out sopRunningData) == false)
                {
                    sopRunningData = new SOPRunningData();
                    m_dicSenorSOPBegins[nSensorZoneID] = sopRunningData;
                }

                // 같은 위험단계 또는 상위 위험단계에 대한 지진 알람을 연속으로 발생시키지 않도록 한다.
                // 적어도 DelaySeconds 이상은 지났는지 확인한다.
                if (CheckSOPRequestTime(nSensorZoneID, data.AlarmDepth, sopRunningData, optionData))
                {
                    sopRunningData.SetSendTime(data.AlarmDepth - 1, new VariousData<DateTime>(DateTime.Now));

                    // 알람 발생
                    AlarmData alarm, prevAlarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, sensorData, data.AlarmDepth, isReal, timeStamp, data, out alarm, out prevAlarm);

                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                    {
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }

                    if (alarm == null)
                        return nResult;

                    if (alarm != null && prevAlarm != null)
                    {
                        m_agentFactory.ProcessManager.ChangeAlarm(dbMgr, alarm, prevAlarm);
                    }
                    else if (alarm != null)
                    {
                        m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                    }

                    dbMgr.Close();

                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add(nSensorType);
                    arrDatas.Add(nSensorZoneID);
                    arrDatas.Add(alarm.SensorZoneHistoryID);
                    arrDatas.Add(alarm.TimeStamp.ToBinary());
                    arrDatas.Add(sensorData);

                    PostAlarm(arrDatas, data, alarm);
                    //System.Diagnostics.Trace.WriteLine("기타알람 : " + data.AlarmDepth);
                    return nResult;
                }
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private bool ProcessSensorDataPrev(int nSensorZoneID, out SensorZoneGroup group, out SensorZone sensorZone, out int nResult)
        {
            group = null;
            sensorZone = null;
            nResult = SOPWebServer.ErrorMessageType.SUCCESS;

            group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (group == null)
            {
                nResult = SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;
                return false;
            }

            sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
            {
                nResult = SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;
                return false;
            }

            return true;
        }

        private void PostAlarm(ArrayList arrDatas, OptionEtcData option, AlarmData alarm)
        {
            if (option.LinkedSOP.Length > 0 && option.RunSOP && alarm.SOPProcess == AlarmData.SOPProcessType.None)
            {
                /*ArrayList datas = new ArrayList();

                datas.AddRange(arrDatas);
                datas.Add(option.AlarmDepth);
                datas.Add(option.RunSOP);
                datas.Add(option.LinkedSOP);

                byte[] bytes2 = SOPWebServer.BinaryHelper.MakeBytes(datas);
                SOPSimulatorManager.ServerInstance.SendClientData(SOPWebServer.Header.ETC_SENSOR_DETECT, bytes2, SOPWebServer.ClientType.SOP_SIMULATOR, -1);*/
            }

            if (option.SendSDMS)
            {
                ArrayList datas = new ArrayList();

                datas.AddRange(arrDatas);
                datas.Add(option.AlarmDepth);

                byte[] bytes2 = SOPWebServer.BinaryHelper.MakeBytes(datas);
                SDMSServer.Instance.SendClientData(SOPWebServer.Header.ETC_SENSOR_DETECT, bytes2, SOPWebServer.ClientType.SDMS, -1);
            }
        }

        // nAlarmDepth 또는 그 보다 높은 알람단계에 해당하는 SOP 실행을 SOP Simulator에게 마지막으로 요청한 시간이
        // 적어도 DelaySeconds 만큼은 지났는지 확인한다.
        private bool CheckSOPRequestTime(int nSensorZoneID, int nAlarmDepth, SOPRunningData sopRunningData, OptionEtcSensor optionData)
        {
            if (optionData.DelaySeconds == null)
                return true;

            int nIndex = nAlarmDepth - 1;
            int nStepCount = sopRunningData.StepCount;
            DateTime dtNow = DateTime.Now;

            for (int i = nIndex; i < nStepCount; i++)
            {
                VariousData<DateTime> time = sopRunningData.GetSendTime(i);

                if (time == null)
                    continue;

                TimeSpan span = dtNow - time.Data;

                if (span.TotalSeconds < optionData.DelaySeconds.Data)
                    return false;
            }

            return true;
        }

        // nAlarmDepth에 해당하는 SOP가 실행중인가?
        // 아니면 nAlarmDepth보다 더 강한 알람단계에 대한 SOP가 실행중인가?
        private bool CheckBeginSOP(int nSensorZoneID, int nAlarmDepth)
        {
            SOPRunningData data;

            if (m_dicSenorSOPBegins.TryGetValue(nSensorZoneID, out data) == false)
            {
                data = new SOPRunningData();
                m_dicSenorSOPBegins[nSensorZoneID] = data;
                return false;
            }

            int nIndex = nAlarmDepth - 1;
            int nStepCount = data.StepCount;

            for (int i = nStepCount - 1; i >= 0 && i >= nIndex; i--)
            {
                if (data.IsRunning(i))
                    return true;
            }

            return false;
        }

        // 알람을 종료해도 되는지 여부를 판단한다.
        // nSensorData의 값이 optionData.MinAlarmData 미만의 값으로 optionData.CloseAlarmSeconds 이상 유지하면 알람을 종료시킨다.
        private void CheckCloseAlarm(int nSensorData, int nSensorZoneID, OptionEtcSensor optionData)
        {
            if (optionData.MinAlarmDatai == null || optionData.CloseAlarmSeconds == null)
                return;

            if (optionData.MinAlarmDatai.Data <= nSensorData)
            {
                m_dicSensorLastEvent[nSensorZoneID] = DateTime.Now;
                return;
            }

            CheckCloseAlarm(nSensorZoneID, optionData);
        }

        // 알람을 종료해도 되는지 여부를 판단한다.
        // fSensorData의 값이 optionData.MinAlarmData 미만의 값으로 optionData.CloseAlarmSeconds 이상 유지하면 알람을 종료시킨다.
        private void CheckCloseAlarm(float fSensorData, int nSensorZoneID, OptionEtcSensor optionData)
        {
            if (optionData.MinAlarmDataf == null || optionData.CloseAlarmSeconds == null)
                return;

            if (optionData.MinAlarmDataf.Data <= fSensorData)
            {
                m_dicSensorLastEvent[nSensorZoneID] = DateTime.Now;
                return;
            }

            CheckCloseAlarm(nSensorZoneID, optionData);
        }

        // 알람을 종료해도 되는지 여부를 판단한다.
        // strSensorData의 값이 optionData.MinAlarmData 미만의 값으로 optionData.CloseAlarmSeconds 이상 유지하면 알람을 종료시킨다.
        private void CheckCloseAlarm(string strSensorData, int nSensorZoneID, OptionEtcSensor optionData)
        {
            if (optionData.MinAlarmDatas == null || optionData.CloseAlarmSeconds == null)
                return;

            int nResult = string.Compare(strSensorData, optionData.MinAlarmDatas);

            if (nResult >= 0)
            {
                m_dicSensorLastEvent[nSensorZoneID] = DateTime.Now;
                return;
            }

            CheckCloseAlarm(nSensorZoneID, optionData);
        }

        private void CheckCloseAlarm(int nSensorZoneID, OptionEtcSensor optionData)
        {
            AlarmData sensorAlarm = null;
            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SensorZoneID == nSensorZoneID)
                {
                    sensorAlarm = alarm;
                    break;
                }
            }

            if (sensorAlarm == null)
                return;

            DateTime dtLastEvent;

            if (m_dicSensorLastEvent.TryGetValue(nSensorZoneID, out dtLastEvent) == false)
                dtLastEvent = new DateTime();

            TimeSpan span = DateTime.Now - dtLastEvent;

            if (span.TotalSeconds >= optionData.CloseAlarmSeconds.Data)
            {
                string strMessage = GetSensorResetString(optionData);
                ProcessReset(sensorAlarm.SensorZoneHistoryID, sensorAlarm.SensorZoneID, strMessage);
                //ProcessMalfunction(sensorAlarm.SensorZoneHistoryID, sensorAlarm.SensorZoneID, -1, "");
            }
        }

        private string GetSensorResetString(OptionEtcSensor optionData)
        {
            if (optionData.CloseAlarmSeconds.Data % 3600 == 0)
                return string.Format("{0}시간 이상 센서값이 알람범위({1}) 아래에서 유지되었기 때문에 알람이 종료됩니다.", optionData.CloseAlarmSeconds.Data / 3600, optionData.GetMinimumData());
            else if (optionData.CloseAlarmSeconds.Data % 60 == 0)
                return string.Format("{0}분 이상 센서값이 알람범위({1}) 아래에서 유지되었기 때문에 알람이 종료됩니다.", optionData.CloseAlarmSeconds.Data / 60, optionData.GetMinimumData());

            return string.Format("{0}초 이상 센서값이 알람범위({1}) 아래에서 유지되었기 때문에 알람이 종료됩니다.", optionData.CloseAlarmSeconds.Data, optionData.GetMinimumData());
        }

        private bool GetEtcSensorOption(int nSensorType, out OptionEtcSensor option)
        {
            option = null;
            IFacility.FacilityType facilityType = IFacility.ToFacilityType(nSensorType);

            if (facilityType == IFacility.FacilityType.NONE)
                return false;

            return m_optionSensorData.TryGetValue(facilityType, out option);
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, object sensorData, int nAlarmDepth, bool isReal, DateTime timeStamp, OptionEtcData data, out AlarmData alarm, out AlarmData prevAlarm)
        {
            prevAlarm = alarm = null;

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (SensorZoneManager.Instance.IsActiveSensor(nSensorTagID) == false)
            {
                WriteLog("AddAlarm 무시(비활성화된 센서) : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

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
                int nSensorData = ToIntSensorData(sensorData);

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

                        string strDisasterData;
                        alarm.Message = GetChangeAlarmDepthString(alarm.AlarmDepth, prevAlarm.AlarmDepth, sensorZone, sensorData, isReal, out strDisasterData);

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
                        string strParam4 = strDisasterData;

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
                        if (data.SendSDMS)
                        {
                            // 센서 데이터가 바뀌었다.
                            DirectDBManager dbMgr = m_dbMgr.Clone();

                            if (dbMgr.Connect() == false)
                                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                            // 알람단계가 바뀌지 않았으므로 바뀐 Sensor 데이터만 SDMS에게 알려준다.
                            group.SetSensorData(sensorZone, nSensorData, dbMgr, false);
                            AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);
                            dbMgr.Close();
                        }

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

                group.SetSensorData(sensorZone, 1, dbMgr, true);

                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, 1, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = nAlarmDepth;
                    group.CurrentAlarm = alarm;
                    
                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = GetDetectETCMessage(group.EquipmentZone, isReal, sensorZone.Type);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    ProcessManager.ReactionType reactionType = ProcessManager.ReactionType.BEGIN_STATUS;

                    string strParam3 = ((int)sensorZone.Type).ToString();
                    string strParam4 = GetSensorDataString(sensorData, sensorZone);
                    string strParam5 = nAlarmDepth.ToString();

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), strParam3, strParam4, strParam5, status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            alarm.Message = strMessage;
                            alarm.IsReal = isReal;
                            alarm.Status = reactionType;
                            dbMgr.Close();
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
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        alarm = null;
                        dbMgr.BatchRollback();
                    }
                }
                else
                {
                    group.RemoveSensorData(sensorZone, dbMgr);
                    WriteLog("AddAlarm 실패 : " + sensorZone.ID.ToString());
                    dbMgr.BatchRollback();
                }

                dbMgr.Close();
            }

            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private string GetChangeAlarmDepthString(int nAlarmDepth, int nPrevAlarmDepth, SensorZone sensorZone, object sensorData, bool isReal, out string strDisasterData)
        {
            string strTag = isReal ? "" : "[테스트]";
            string strDisasterType = "";
            string strData = GetSensorTypeString(sensorData, sensorZone, out strDisasterType);
            string strUpDown = nAlarmDepth > nPrevAlarmDepth ? "상향" : "하향";

            strDisasterData = strData;

            if (strData.Length > 0)
                return strTag + string.Format("{0}의 {1} 감지되었습니다.\r\n알람단계가 {2}단계에서 {3}단계로 {4}되었습니다.", strData, strDisasterType, nPrevAlarmDepth, nAlarmDepth, strUpDown);

            return strTag + string.Format("{0} 감지되었습니다.\r\n알람단계가 {1}단계에서 {2}단계로 {3}되었습니다.", strDisasterType, nPrevAlarmDepth, nAlarmDepth, strUpDown);
        }

        private string GetSensorTypeString(object sensorData, SensorZone sensorZone, out string strDisasterType)
        {
            strDisasterType = "알람이";

            if (sensorZone == null || sensorData == null)
                return "";

            if (sensorZone.Type == IFacility.FacilityType.STRONG_WIND)
            {
                return GetStrongWindString(sensorData, out strDisasterType);
            }

            return "";
        }

        private string GetStrongWindString(object sensorData, out string strDisasterType)
        {
            strDisasterType = "강풍이";

            if (sensorData is float)
            {
                return string.Format("풍속 : {0:F1}{1}", (float)sensorData, OptionEtcSensor.GetUnitString(IFacility.FacilityType.STRONG_WIND));
            }

            return "";
        }

        private string GetSensorDataString(object sensorData, SensorZone sensorZone)
        {
            if (sensorZone.Type == IFacility.FacilityType.STRONG_WIND)
            {
                string strDisasterType;
                return GetStrongWindString(sensorData, out strDisasterType);
            }

            if (sensorData == null)
                return "";

            return sensorData.ToString();
        }

        private int ToIntSensorData(object sensorData)
        {
            if (sensorData == null)
                return 0;

            if (sensorData is int)
                return (int)sensorData;
            else if (sensorData is float)
            {
                // float일 경우 100을 곱한 값을 사용한다.
                return (int)((float)sensorData * 100);
            }
            else if (sensorData is string)
            {
                int data;

                if (int.TryParse((string)sensorData, out data))
                    return data;
            }

            return sensorData.GetHashCode();
        }

        // alarmDepthChange : 0(변화없음), 1(이전보다 알람단계가 더 높아졌음 => 더 위험해졌음), 2(이전보다 알람단계가 더 낮아졌음 => 덜 위험해졌음)
        // sensorDataChange : 0(변화없음), 1(이전보다 센서 데이터가 더 높아졌음 => 더 위험해졌음), 2(이전보다 센서 데이터가 더 낮아졌음 => 덜 위험해졌음)
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

        public int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, string strMessage, bool isReal)
        {
            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                bool rollback = dbMgr.BatchRollback();
                dbMgr.Close();
                System.Diagnostics.Trace.WriteLine("Rollback : " + rollback.ToString());
                WriteLog("RemoveSensorData 실패 : " + sensorZone.ID.ToString());
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

            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    dbMgr.Close();
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
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        // 센서값이 일정시간 동안 알람범위 아래에 있었기 때문에 알람을 종료시킨다.
        public int ProcessReset(int nSensorZoneHistoryID, int nSensorZoneID, string strMessage)
        {
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);
            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (sensorZone == null || group == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

            // 알람 해제
            AlarmData alarm = group.CurrentAlarm;
            int nResult = RemoveAlarm(group, sensorZone, strMessage, true);

            if (alarm != null && group.CurrentAlarm == null)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                alarm.Status = BaseProcessManager.ReactionType.MALFUNCTION;
                m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                dbMgr.Clone();
            }

            return nResult;
        }

        public int ProcessMalfunction(int nSensorZoneHistoryID, int nSensorZoneID, int nSOPGenUserID, string strDescription)
        {
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);
            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (sensorZone == null || group == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

            // 알람 해제
            AlarmData alarm = group.CurrentAlarm;
            int nResult = RemoveAlarm_Malfunction(group, sensorZone, nSOPGenUserID);

            if (alarm != null && group.CurrentAlarm == null)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                alarm.Status = BaseProcessManager.ReactionType.MALFUNCTION;
                m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                dbMgr.Clone();
            }

            return nResult;
        }

        private int RemoveAlarm_Malfunction(SensorZoneGroup group, SensorZone sensorZone, int nSOPGenUserID)
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

            // 오작동 처리는 SDMS에서 사용자에 의하여 보내기 때문에 특정 센서 뿐만 아니라
            // SensorZoneGroup내에 있는 모든 센서 데이터를 초기화 시킨다.
            if (group.RemoveAllSensorData(dbMgr) == false)
            //if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                dbMgr.Close();
                WriteLog("RemoveAllSensorData 실패 : " + sensorZone.ID.ToString());
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

            string strMessage = GetMalfunctionMessage(sensorZone.EquipZone, alarm.IsReal, sensorZone.Type);
            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.MALFUNCTION, strMessage, strEquipZoneID, sensorZone.ID.ToString(), nSOPGenUserID.ToString(), null, null, status, dbMgr))
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
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        public int ProcessManualReportEtc(int nSensorZoneID, int nZoneID, int nSOPGenUserID, string strMemo, int nAlarmStep)
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
            // SendSOPSimulatorControl(nSOPGenUserID) => 구현할 것
            AlarmData alarm = AlarmManager.Instance.GetManualAlarm(nZoneID, IFacility.FacilityType.PSM_SENSOR, dbMgr);
            
            if (alarm != null)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.ALREADY_PROCESSED;
            }

            // Transaction 처리
            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime dtNow = DateTime.Now;

            int nType = nSensorZoneID - SOPWebServer.Header.ManualReportDefaultID;
            IFacility.FacilityType type = IFacility.ToFacilityType(nType);
            string strParam2 = ((int)type).ToString();

            alarm = AlarmManager.Instance.AddAlarm(nSensorZoneID, 1, nZoneID.ToString(), strParam2, null, dtNow, dbMgr);

            if (alarm == null)
            {
                dbMgr.BatchRollback();
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }
            else
            {
                alarm.IsManual = true;
                alarm.AlarmDepth = nAlarmStep;
            }

            string strMessage = GetEtcManualReportString(nZoneID, type);
            string strParam1 = nZoneID.ToString();
            strParam2 = nSensorZoneID.ToString();
            string strParam3 = nSOPGenUserID.ToString();
            string strParam5 = nAlarmStep.ToString();

            ProcessManager.DetectionStatus detectionStatus = ProcessManager.DetectionStatus.REAL;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);
            BaseProcessManager.ReactionType reactionType = BaseProcessManager.ReactionType.NOTIFY_SIGNAL;

            if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, dtNow, strMessage, strParam1, strParam2, strParam3, null, strParam5, status, dbMgr, true))
            {
                if (strMemo.Length == 0 || AlarmManager.Instance.AddReactionHistoryDescription(alarm.SensorReactionHistoryID, alarm.SensorZoneHistoryID, strMemo, dbMgr, true))
                {
                    if (dbMgr.BatchCommit())
                    {
                        alarm.Message = strMessage;
                        alarm.IsReal = true;
                        alarm.Status = reactionType;

                        dbMgr.Close();
                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.ReportAlarm(dbMgr, alarm);

                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.SUCCESS;
                    }
                    else
                    {
                        dbMgr.Close();
                        WriteLog("ProcessManualReportEtc 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                }
                else
                {
                    WriteLog("AddReactionHistoryDescription 실패 : " + alarm.SensorZoneHistoryID.ToString() + " / ErrorMessage : " + dbMgr.ErrorMessage);
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            // SensorZoneHistory
            // Param1 : Zone ID
            // Param2 : Sensor Type(FacilityType)

            // SensorReactionHistory
            // Param1 : Zone ID
            // Param2 : SennsorZone ID(없으니까 당연히 0)
            // Param3 : SOPGenUserID

            dbMgr.Close();
            //DateTime timeStamp = DateTime.Now;

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private string GetTrainingModeString()
        {
            string strTag = "";

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect())
            {
                strTag = m_agentFactory.SMSManager.GetTrainingModeString(dbMgr);
                dbMgr.Close();
            }

            return strTag;
        }

        private string GetDetectETCMessage(EquipmentZone equipZone, bool isReal, IFacility.FacilityType facilityType)
        {
            string strType = IFacility.GetFacilityTypeString(facilityType);

            if (isReal)
            {
                string strTag = GetTrainingModeString();
                
                if (equipZone == null)
                    return string.Format("{0}[{1}] 신호가 탐지되었습니다.", strTag, strType);
                else
                    return string.Format("{0}[{1}]에서 {2} 신호가 탐지되었습니다", strTag, equipZone.DisplayText, strType);
            }

            if (equipZone == null)
                return string.Format("[테스트]{0} 신호가 탐지되었습니다", strType);

            return string.Format("[테스트][{0}]에서 {1} 신호가 탐지되었습니다", equipZone.DisplayText, strType);
        }

        private string GetClearETCMessage(EquipmentZone equipZone, bool isReal, IFacility.FacilityType facilityType)
        {
            string strMessage = "상황해제";

            string strType = IFacility.GetFacilityTypeString(facilityType);

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    strMessage = string.Format("{0}[{1}] 신호가 복구되었습니다.", strTag, strType);
                else
                    strMessage = string.Format("{0}[{1}]에서 탐지된 [{2}] 신호가 복구되었습니다", strTag, equipZone.DisplayText, strType);
            }
            else
            {
                if (equipZone == null)
                    strMessage = string.Format("[테스트][{0}] 신호가 복구되었습니다", strType);
                else
                    strMessage = string.Format("[테스트][{0}]에서 탐지된 [{1}] 신호가 탐지되었습니다", equipZone.DisplayText, strType);
            }

            return strMessage;
        }

        private string GetMalfunctionMessage(EquipmentZone equipZone, bool isReal, IFacility.FacilityType facilityType)
        {
            string strType = IFacility.GetFacilityTypeString(facilityType);

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return string.Format("{0}탐지된 [{1}] 신호가 오작동으로 신고되었습니다.", strTag, strType);
                else
                    return string.Format("{0}[{1}]에서 탐지된 [{2}] 신호가 오작동으로 신고되었습니다", strTag, equipZone.DisplayText, strType);
            }
            else
            {
                if (equipZone == null)
                    return string.Format("[테스트][{0}] 신호가 오작동으로 신고되었습니다", strType);
                else
                    return string.Format("[테스트][{0}]에서 탐지된 [{1}] 신호가 오작동으로 신고되었습니다", equipZone.DisplayText, strType);
            }
        }

        private string GetEtcManualReportString(int nZoneID, IFacility.FacilityType type)
        {
            string strMessage = "";
            string strType = IFacility.GetFacilityTypeString(type);

            if (nZoneID < 0)
                strMessage = string.Format("[{0}] 상황이 신고되었습니다", strType);
            else
            {
                Zone zone = SensorZoneManager.Instance.GetZone(nZoneID);
                if (zone != null)
                    strMessage = string.Format("[{0}]에서 [{1}] 상황이 신고되었습니다", zone.DisplayText, strType);
            }

            return strMessage;
        }

        private string GetClearManualPSMMessage(AlarmData alarm, IFacility.FacilityType type)
        {
            string strType = IFacility.GetFacilityTypeString(type);

            string strMessage = string.Format("신고된 [{0}] 상황이 종료되었습니다", strType);
            int nZoneID;
            
            if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
            {
                Zone zone = SensorZoneManager.Instance.GetZone(nZoneID);

                if (zone != null)
                {
                    strMessage = string.Format("[{0}]에 신고된 [{1}] 상황이 종료되었습니다", zone.DisplayText, strType); 
                }
                else
                {
                    strMessage = string.Format("신고된 [{0}] 상황이 종료되었습니다", strType);
                }
            }

            return strMessage;
        }

        // 수동 신고된 신호 복구
        public int RemoveManualAlarm(AlarmData alarm)
        {
            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;
            string strMessage = GetClearManualPSMMessage(alarm, alarm.SensorType);
            string strEquipZoneID = null;
            ProcessManager.DetectionStatus detectionStatus = ProcessManager.DetectionStatus.REAL;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, alarm.SensorZoneID.ToString(), null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    dbMgr.Close();
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
            WriteLog("Remove.ManualAlarm 실패 : " + alarm.SensorZoneHistoryID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        public int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, bool isReal)
        {
            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                bool rollback = dbMgr.BatchRollback();
                dbMgr.Close();
                System.Diagnostics.Trace.WriteLine("Rollback : " + rollback.ToString());
                WriteLog("RemoveSensorData 실패 : " + sensorZone.ID.ToString());
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

            string strMessage = GetClearFireMessage(sensorZone.EquipZone, isReal);
            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, strEquipZoneID, sensorZone.ID.ToString(), null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    dbMgr.Close();
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
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, bool isReal, out AlarmData alarm)
        {
            alarm = null;

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (SensorZoneManager.Instance.IsActiveSensor(nSensorTagID) == false)
            {
                WriteLog("AddAlarm 무시(비활성화된 센서) : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

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
                DirectDBManager dbMgr = m_dbMgr.Clone();
                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                // 이미 알람이 발생중이다.
                // Sensor 데이터만 기록하고 종료한다.
                group.SetSensorData(sensorZone, 1, dbMgr, false);
                AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.SUCCESS;
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

                group.SetSensorData(sensorZone, 1, dbMgr, true);

                DateTime timeStamp = DateTime.Now;
                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, 1, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = 1;
                    group.CurrentAlarm = alarm;

                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = GetDetectFireMessage(group.EquipmentZone, isReal);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    ProcessManager.ReactionType reactionType = ProcessManager.ReactionType.BEGIN_STATUS;

                    string strParam3 = ((int)sensorZone.Type).ToString();

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), strParam3, null, null, status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            alarm.Message = strMessage;
                            alarm.IsReal = isReal;
                            alarm.Status = reactionType;
                            dbMgr.Close();
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
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        alarm = null;
                        dbMgr.BatchRollback();
                    }
                }
                else
                {
                    group.RemoveSensorData(sensorZone, dbMgr);
                    WriteLog("AddAlarm 실패 : " + sensorZone.ID.ToString());
                    dbMgr.BatchRollback();
                }

                dbMgr.Close();
            }

            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private string GetDetectFireMessage(EquipmentZone equipZone, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + "정전 상황이 탐지되었습니다";
                else
                    return string.Format("{0}[{1}]에서 정전 상황이 탐지되었습니다", strTag, equipZone.DisplayText);
            }

            if (equipZone == null)
                return "[테스트]정전 상황이 탐지되었습니다";

            return string.Format("[테스트][{0}]에서 정전 상황이 탐지되었습니다", equipZone.DisplayText);
        }

        private string GetClearFireMessage(EquipmentZone equipZone, bool isReal)
        {
            string strMessage = "상황해제";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    strMessage = strTag + "정전신호가 복구되었습니다";
                else
                    strMessage = string.Format("{0}[{1}]에서 탐지된 화재신호가 복구되었습니다", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    strMessage = "[테스트]정전신호가 복구되었습니다";
                else
                    strMessage = string.Format("[테스트][{0}]에서 탐지된 화재신호가 복구되었습니다", equipZone.DisplayText);
            }

            return strMessage;
        }
    }
}
