using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using AgentFactory;

namespace ServerProcess.Data
{
    public class ProcessManager : AgentFactory.BaseProcessManager
    {
        public ProcessManager(Factory factory)
            : base(factory)
        {
        }

        public override void NewAlarm(DirectDBManager dbMgr, AlarmData alarm)
        {
            // 알람 발생전 할일
            List<ClientMessage> messages = m_processAgent.PrevNewAlarm(dbMgr, alarm, AlarmManager.Instance);
            ProcessClientMessages(messages);

            ProcessAlarm(dbMgr, alarm);

            Client.SDMSServer.Instance.SendSensorZoneData(1, alarm);
            Client.SDMSServer.Instance.SendSensorReactionLog(alarm);

            // 알람 발생후 할일
            messages = m_processAgent.PostNewAlarm(dbMgr, alarm, AlarmManager.Instance);
            ProcessClientMessages(messages);
        }

        // 알람상태가 prevAlarm에서 alarm으로 바뀌었다.
        public override void ChangeAlarm(DirectDBManager dbMgr, AlarmData alarm, AlarmData prevAlarm)
        {
            // 알람 변경전 할일
            List<ClientMessage> messages = m_processAgent.PrevChangeAlarm(dbMgr, alarm, prevAlarm, AlarmManager.Instance);
            ProcessClientMessages(messages);

            ProcessAlarm(dbMgr, alarm);

            // 알람 변경후 할일
            messages = m_processAgent.PostChangeAlarm(dbMgr, alarm, prevAlarm, AlarmManager.Instance);
            ProcessClientMessages(messages);
        }

        public override void ReportAlarm(DirectDBManager dbMgr, AlarmData alarm)
        {
            // 재난 신고전 할일
            List<ClientMessage> messages = m_processAgent.PrevReportAlarm(dbMgr, alarm, AlarmManager.Instance);
            ProcessClientMessages(messages);

            BaseSMSManager.SMSMessageType messageType = ProcessAlarm(dbMgr, alarm);

            if (alarm.SensorZoneID >= SOPWebServer.Header.ManualReportDefaultID)
            {
                // 수동 신고
                int nZoneID = -1;

                if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
                {
                    Client.SDMSServer.Instance.SendSensorReactionLog(alarm);
                    SOPSimulatorManager.ServerInstance.SendSensorSignal(alarm, nZoneID, -1);
                    //Client.SOPSimulatorServer.Instance.SendSensorSignal(alarm, nZoneID, -1);
                }
            }
            else
            {
                string strOriginSensorTableName = FacilityManager.GetFacilityTypeTable(alarm.SensorType);
                float x = 0.0f, y = 0.0f, z = 0.0f;
                int nOrgSensorID = -1;

                if (strOriginSensorTableName.Length > 0)
                {
                    string strSQL = string.Format("select sz.OrgSensorID, os.X, os.Y, os.Z from SensorZoneHistory as szh, SensorZone as sz, {0} as os where szh.ID = {1} and szh.SensorID = sz.ID and sz.OrgSensorID = os.ID",
                                        strOriginSensorTableName, alarm.SensorZoneHistoryID);
                    ArrayList arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult != null && arrResult.Count >= 4)
                    {
                        nOrgSensorID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                        x = WebDBManager.GetFloatField(arrResult[1].ToString(), 0.0f);
                        y = WebDBManager.GetFloatField(arrResult[2].ToString(), 0.0f);
                        z = WebDBManager.GetFloatField(arrResult[3].ToString(), 0.0f);
                    }
                }

                Client.SDMSServer.Instance.SendSensorReactionLog(alarm);
                SOPSimulatorManager.ServerInstance.SendSensorSignal(alarm, -1, nOrgSensorID, x, y, z);
                //Client.SOPSimulatorServer.Instance.SendSensorSignal(alarm, -1, nOrgSensorID, x, y, z);
            }

            // 재난 신고후 할일
            messages = m_processAgent.PostReportAlarm(dbMgr, alarm, AlarmManager.Instance);
            ProcessClientMessages(messages);
        }

        public override void ClearAlarm(DirectDBManager dbMgr, AlarmData alarm)
        {
            // 알람 복구전 할일
            List<ClientMessage> messages = m_processAgent.PrevClearAlarm(dbMgr, alarm, AlarmManager.Instance);
            ProcessClientMessages(messages);

            ProcessAlarm(dbMgr, alarm, false);

            Client.SDMSServer.Instance.SendSensorZoneData(0, alarm);
            Client.SDMSServer.Instance.SendClearAlarm(alarm);
            SOPSimulatorManager.ServerInstance.SendClearAlarm(alarm);
            //Client.SOPSimulatorServer.Instance.SendClearAlarm(alarm);

            // 알람 복구후 할일
            messages = m_processAgent.PostClearAlarm(dbMgr, alarm, AlarmManager.Instance);
            ProcessClientMessages(messages);
        }

        private void ProcessClientMessages(List<ClientMessage> messages)
        {
            if (messages == null)
                return;

            foreach (ClientMessage message in messages)
            {
                if (message.ClientType == SOPWebServer.ClientType.FIRE_SENSOR_SERVER)
                    Client.FireSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.PSM_SENSOR_SERVER)
                    Client.PSMSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SECURITY_SENSOR_SERVER)
                    Client.SecuritySensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.EARTHQUAKE_SENSOR_SERVER)
                    Client.EarthquakeSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.LOGIN_SERVER)
                    Client.LoginServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.TEMPERATURE_HUMIDITY_SERVER)
                    Client.TempHumidityServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SDMS)
                    Client.SDMSServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SOP_SIMULATOR)
                {
                    SOPSimulatorManager.ServerInstance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                    //Client.SOPSimulatorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                }
                else if (message.ClientType == SOPWebServer.ClientType.SOP_MANAGER)
                    Client.SOPManagerServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SOP_COMMANDER)
                    Client.ServerCommander.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.ETC)
                    Client.EtcSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
            }
    }

        private BaseSMSManager.SMSMessageType ProcessAlarm(DirectDBManager dbMgr, AlarmData alarm, bool phoneNumberClear = true)
        {
            BaseSMSManager.SMSMessageType messageType;
            string strCaller = NeedSMS(dbMgr, alarm, out messageType);

            if (strCaller != null)
            {
                if (phoneNumberClear)
                {
                    alarm.PhoneNumbers.Clear();
                    alarm.RegularMemberIDs.Clear();
                    alarm.ExternalMemberIDs.Clear();
                }

                int nReceiverCount = m_factory.SMSManager.GetPhoneNumbers(dbMgr, alarm, messageType);

                if (nReceiverCount > 0)
                {
                    int nResult = m_factory.SMSManager.SendSMS(dbMgr, alarm, strCaller, alarm.PhoneNumbers.Values.ToList(), alarm.RegularMemberIDs.Values.ToList(), alarm.ExternalMemberIDs.Values.ToList(), alarm.Message, alarm.SensorReactionHistoryID);

                    if (nResult == SOPWebServer.ErrorMessageType.SUCCESS)
                    {
                        ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                        VariousData<int> status = new VariousData<int>((int)detectionStatus);
                        AlarmManager.Instance.AddReactionHistory(alarm, (int)ReactionType.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, status, dbMgr, false);
                    }
                }
            }

            BaseBroadcastManager.SituationType situationType;

            if (NeedBroadcast(dbMgr, alarm, out situationType))
            {
                int nRepeatCount;
                bool useSiren;
                string strMessage = m_factory.BroadcastManager.GetBroadcastMessage(dbMgr, alarm, situationType, out nRepeatCount, out useSiren);

                if (m_factory.BroadcastManager.RunBroadcast(dbMgr, strMessage, nRepeatCount, useSiren))
                {
                    ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    AlarmManager.Instance.AddReactionHistory(alarm, (int)ReactionType.RUN_BROADCAST, DateTime.Now, strMessage, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, null, null, status, dbMgr, false);
                }
            }

            return messageType;
        }

        // Return 값 : 문자발송이 필요한 상황이면 발신자 번호를 리턴한다.
        //             문자발송이 필요하지 않은 상황이면 null을 리턴한다.
        public override string NeedSMS(DirectDBManager dbMgr, AlarmData alarm, out BaseSMSManager.SMSMessageType messageType)
        {
            messageType = ServerProcess.Data.SMSManager.ReactionTypeToMessageType(alarm.Status, alarm.SensorType);

            if (messageType == BaseSMSManager.SMSMessageType.UNKNOWN)
                return null;

            string strSQL = string.Format("Select UseSMS from SDMSSMSConfig where MessageType = {0} and SiteID = {1}", (int)messageType, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> useSMS = WebDBManager.GetIntField(arrResult[0].ToString());

            if (useSMS == null || useSMS.Data != 1)
                return null;

            strSQL = string.Format("Select PropertyValue from OptionSDMS where PropertyName = 'SMSCaller' and SiteID = {0}", dbMgr.SiteID);
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            return WebDBManager.GetStringField(arrResult[0]);
        }

        public override bool NeedBroadcast(DirectDBManager dbMgr, AlarmData alarm, out BaseBroadcastManager.SituationType situationType)
        {
            situationType = BaseBroadcastManager.ReactionTypeToSituationType(alarm.Status, alarm.SensorType);

            if (situationType == BaseBroadcastManager.SituationType.Unknown)
                return false;
            else if (situationType == BaseBroadcastManager.SituationType.DETECT_EARTHQUAKE)
            {
                if (alarm.Tag != null && alarm.Tag is UnE.Earthquake.EarthquakeOption)
                {
                    UnE.Earthquake.EarthquakeOption option = (UnE.Earthquake.EarthquakeOption)alarm.Tag;
                    return option.UseBroadcast;
                }
                else
                    return false;
            }

            string szText = "SELECT UseBroadcast FROM SDMSBroadcastConfig WHERE SituationType = {0} and SiteID = {1}";
            string szSQL = string.Format(szText, (int)situationType, dbMgr.SiteID);

            ArrayList arResult = dbMgr.GetResultData(szSQL);
            if (arResult == null || arResult.Count == 0)
            {
                return false;
            }
            else
            {
                int nResult = WebDBManager.GetIntField(arResult[0].ToString(), -1);

                if (nResult == 1)
                    return true;
            }

            return false;
        }
    }
}
