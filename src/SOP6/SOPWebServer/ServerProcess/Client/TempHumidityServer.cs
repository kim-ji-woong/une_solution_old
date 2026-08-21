using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using AgentFactory;
using System.ServiceModel;
using System.Collections;
using DBUtility2;
using UnE.Sensor;
using UnE.Spatial;

namespace ServerProcess.Client
{
    using ServerProcess.Data;

    // 온/습도 서버
    public class TempHumidityServer : BaseClient
    {
        private static TempHumidityServer m_instance = null;
        private Dictionary<int, string> m_dicAlarmTypes = new Dictionary<int, string>();
        // 수신반과 직접 연결중인 Client들의 정보를 기억해둔다.
        private Dictionary<ClientData, Receiver> m_dicClientReceivers = new Dictionary<ClientData, Receiver>();

        public static TempHumidityServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.TEMPERATURE_HUMIDITY_SERVER; }
        }

        public TempHumidityServer()
            : base()
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.TemperatureHumidity);
        }

        public TempHumidityServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.TemperatureHumidity);
        }

        protected override void OnLoadEvent()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "Select ID, AlarmName from THAlarmType";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            dbMgr.Close();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strAlarmName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strAlarmName == null)
                    continue;

                m_dicAlarmTypes[id.Data] = strAlarmName;
            }
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] bytes, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.SENSOR_DATA)
                return ProcessSensorData(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATA_TEST)
                return ProcessSensorData(arrDatas, false);
            else if (header == SOPWebServer.Header.SENSOR_DATAS)
                return ProcessSensorDatas(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATAS_TEST)
                return ProcessSensorDatas(arrDatas, false);
            else if (header == SOPWebServer.Header.SENSOR_USER_RESET)
                return _ProcessUserReset(arrDatas);
            else if (header == SOPWebServer.Header.RECEIVER_CONNECT)
                return ProcessReceiverState(data, ctx, arrDatas, true);
            else if (header == SOPWebServer.Header.RECEIVER_DISCONNECT)
                return ProcessReceiverState(data, ctx, arrDatas, false);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int _ProcessUserReset(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                return ProcessUserReset(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
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
            }

            return nResult;
        }

        private int ProcessSensorDatas(ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nAlarmCount = (int)arrDatas[1];

                int nIndex = 2;
                int nResult = SOPWebServer.ErrorMessageType.SUCCESS;

                for (int i=0;i<nAlarmCount;i++)
                {
                    if (arrDatas.Count >= nIndex + 3 && arrDatas[nIndex] is int && arrDatas[nIndex + 1] is int && arrDatas[nIndex + 2] is int)
                    {
                        int nSensorTagID = (int)arrDatas[nIndex];
                        int nSensorZoneID = (int)arrDatas[nIndex + 1];
                        int nSensorData = (int)arrDatas[nIndex + 2];

                        int result = ProcessSensorData(nSensorType, nSensorTagID, nSensorZoneID, nSensorData, isReal);

                        if (result != SOPWebServer.ErrorMessageType.SUCCESS)
                            nResult = result;

                        nIndex += 3;
                    }
                    else
                        return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                }

                return nResult;
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

                return ProcessSensorData(nSensorType, nSensorTagID, nSensorZoneID, nSensorData, isReal);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessSensorData(int nSensorType, int nSensorTagID, int nSensorZoneID, int nSensorData, bool isReal)
        {
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
                int nResult = AddAlarm(group, nSensorTagID, sensorZone, nSensorData, isReal, out alarm);

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
                int nResult = SOPWebServer.ErrorMessageType.SUCCESS;

                if (alarm != null)
                {
                    if (nSensorData == 0)
                    {
                        // 알람 모두 해제
                        nResult = RemoveAlarm(group, sensorZone, isReal);

                        if (alarm != null && group.CurrentAlarm == null)
                        {
                            alarm.Status = BaseProcessManager.ReactionType.END_STATUS;

                            DirectDBManager dbMgr = m_dbMgr.Clone();

                            if (dbMgr.Connect() == false)
                                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                            m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                            dbMgr.Close();
                        }
                    }
                    else
                    {
                        // nSensorData에 대한 알람만 해제
                        nResult = RemoveAlarm(group, sensorZone, -nSensorData, isReal);

                        if (nResult != SOPWebServer.ErrorMessageType.SUCCESS)
                            return nResult;

                        if (group.GetSensorDatas().Count == 0)
                        {
                            alarm.Status = BaseProcessManager.ReactionType.END_STATUS;

                            DirectDBManager dbMgr = m_dbMgr.Clone();

                            if (dbMgr.Connect() == false)
                                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                            m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                            dbMgr.Close();
                        }
                    }
                }

                return nResult;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        // 오작동 처리
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

            string strMessage = GetUserResetMessage(sensorZone.EquipZone, alarm.IsReal);
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

        // 신호 복구(nAlarmType에 대해서만)
        public int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, int nAlarmType, bool isReal)
        {
            List<KeyValuePair<SensorZone, int>> sensorDatas = group.GetSensorDatas();
            int nSensorData = (1 << (nAlarmType - 1));

            if (sensorDatas.Count == 0 || sensorDatas[0].Value == nSensorData)
                return RemoveAlarm(group, sensorZone, isReal);

            int nCurrentSensorData = sensorDatas[0].Value;
            int nPostSensorData = (nCurrentSensorData ^ nSensorData);

            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            group.SetSensorData(sensorZone, nPostSensorData, dbMgr, true);
            
            // nAlarmType의 알람은 복구되었지만 다른 알람이 아직 남아있는 상황
            if (dbMgr.BatchCommit())
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        // 신호 복구(전체 알람)
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

            string strMessage = GetClearTHMessage(sensorZone.EquipZone, isReal);
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

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, int nAlarmType, bool isReal, out AlarmData alarm)
        {
            alarm = null;

            // 하나의 센서가 여러개의 알람상태를 가질수 있다.
            int nSensorData = (1 << (nAlarmType - 1));

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (SensorZoneManager.Instance.IsActiveSensor(nSensorTagID) == false)
            {
                WriteLog("AddAlarm 무시(비활성화된 센서) : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            AlarmData currentAlarm = group.CurrentAlarm;
            List<KeyValuePair<SensorZone, int>> sensorDatas = group.GetSensorDatas();
            int nSensorDataCount = sensorDatas.Count;

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
                // 항온/항습 Sensor는 하나의 센서에 하나의 SensorZoneGroup만 존재한다.
                int nCurrentSensorData = sensorDatas[0].Value;

                if ((nCurrentSensorData & nSensorData) == nSensorData)
                {
                    // 이미 같은 종류의 알람이 존재한다.
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }

                nSensorData = (nCurrentSensorData | nSensorData);

                alarm.TimeStamp = DateTime.Now;
                // 항온항습관련 알람의 AlarmDepth는 이 센서에 발생한 알람의 개수를 의미한다.
                alarm.AlarmDepth = alarm.AlarmDepth + 1;
                alarm.SensorZoneID = sensorZone.ID;
                alarm.Message = GetAlarmMessage(sensorZone, nAlarmType, isReal);

                group.SetSensorData(sensorZone, nSensorData, dbMgr, false);
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

                group.SetSensorData(sensorZone, nSensorData, dbMgr, true);

                DateTime timeStamp = DateTime.Now;
                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, nSensorData, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = 1;
                    group.CurrentAlarm = alarm;

                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = GetAlarmMessage(sensorZone, nAlarmType, isReal);
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

        /*private string GetDetectFireMessage(EquipmentZone equipZone, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + "화재가 탐지되었습니다";
                else
                    return string.Format("{0}[{1}]에서 화재가 탐지되었습니다", strTag, equipZone.DisplayText);
            }

            if (equipZone == null)
                return "[테스트]화재가 탐지되었습니다";

            return string.Format("[테스트][{0}]에서 화재가 탐지되었습니다", equipZone.DisplayText);
        }*/

        private string GetAlarmMessage(SensorZone sensorZone, int nAlarmType, bool isReal)
        {
            string strTag = isReal ? "" : "[테스트]";
            string strAlarmMessage = "";

            if (m_dicAlarmTypes.TryGetValue(nAlarmType, out strAlarmMessage) == false)
            {
                return string.Format("{1}[{0}]에서 알람이 발생하였습니다.", sensorZone.EquipZone.ZoneName, strTag);
            }

            return string.Format("{2}[{0}]에서 {1} 알람이 발생하였습니다.", sensorZone.EquipZone.ZoneName, strAlarmMessage, strTag);
        }

        private string GetClearTHMessage(EquipmentZone equipZone, bool isReal)
        {
            string strMessage = "상황해제";

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    strMessage = strTag + "알람신호가 복구되었습니다";
                else
                    strMessage = string.Format("{0}[{1}]에서 탐지된 알람신호가 복구되었습니다", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    strMessage = "[테스트]알람신호가 복구되었습니다";
                else
                    strMessage = string.Format("[테스트][{0}]에서 탐지된 알람신호가 복구되었습니다", equipZone.DisplayText);
            }

            return strMessage;
        }

        private string GetUserResetMessage(EquipmentZone equipZone, bool isReal)
        {
            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                    return strTag + "탐지된 알람신호가 시스템 복구되었습니다.";
                else
                    return string.Format("{0}[{1}]에서 탐지된 알람신호가 시스템 복구되었습니다", strTag, equipZone.DisplayText);
            }
            else
            {
                if (equipZone == null)
                    return "[테스트]알람신호가 시스템 복구되었습니다";
                else
                    return string.Format("[테스트][{0}]에서 탐지된 알람신호가 시스템 복구되었습니다", equipZone.DisplayText);
            }
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

        private int ProcessReceiverState(ClientData data, OperationContext ctx, ArrayList arrDatas, bool isConnected)
        {
            ClientData client = (ClientData)GetClientData(ctx);

            if (client == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_CLIENT;

            if (arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                int nReceiverID = (int)arrDatas[0];
                Receiver receiver= Data.ReceiverManager.Instance.GetReceiver(nReceiverID);

                if (receiver != null)
                {
                    Data.ReceiverManager.Instance.UpdateState(receiver, isConnected, isConnected, dbMgr);
                    m_dicClientReceivers[data] = receiver;
                }

                dbMgr.Close();
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        // 클라이언트와의 접속이 끊어지면 그와 연결된 수신반과의 연결상태를 갱신해준다.
        protected override void OnRemoveClient(ClientData data)
        {
            Receiver receiver;

            if (m_dicClientReceivers.TryGetValue(data, out receiver))
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect())
                {
                    Data.ReceiverManager.Instance.UpdateState(receiver, false, false, dbMgr);
                    dbMgr.Close();
                }

                m_dicClientReceivers.Remove(data);
            }
        }
    }
}
