using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDMS;
using TcpLib2;
using System.Collections;
using System.Threading;
using SOP;
using DBUtility;
using System.Diagnostics;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMSServer
{
    public class ClientDataSDMS : ClientData
    {
        private int m_nSiteID = 1;
        public ClientDataSDMS(ServiceProvider provider)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_provider = provider;
            ClientType = TCP_CLIENT.SDMS_CLIENT;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            // 현재 수신반 상태를 전송한다.
            //SendAllReciverState(state);

            // 현재 진행중인 화재들에 대한 마지막 Log List를 전송한다.
            if (!SendSensorReactionLogList(state))
                return false;

            return SendLastReadSDMSMessageID(state);
        }

        private bool SendLastReadSDMSMessageID(ConnectionState state)
        {
            int nLastReadID = SOPServer.SDMSMessageWatcher.LastReadID;

            if (nLastReadID < 0)
                return true;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SDMSCommandType.SDMS_PUBLIC_MESSAGE_ID);
            arrDatas.Add(nLastReadID);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SDMS_COMMAND, arrDatas);
            return m_provider.Send(bytes, 0, bytes.Length, state);
        }

        public bool SendAllReciverState(ConnectionState state)
        {
            ArrayList arReciverList = ReciverManager.Instance.GetReciverList();
            if (arReciverList == null)
                return false;

            int nDataCount = arReciverList.Count * 2;
            int nSize = 6 + (nDataCount * 9);
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.ALL_RECIVER_STATE);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(nDataCount);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            int nIndex = 6;

            if (arReciverList != null)
            {
                foreach (Reciver reciver in arReciverList)
                {
                    byte[] nReciverIDBytes = TcpHelper.MakeBytes(reciver.ID);
                    byte[] nConnectedBytes = TcpHelper.MakeBytes(reciver.State);

                    TcpHelper.CopyBytes(bytes, ref nIndex, nReciverIDBytes);
                    TcpHelper.CopyBytes(bytes, ref nIndex, nConnectedBytes);
                }
            }

            try
            {
                return m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendAllReciverState", ex);
            }
            return false;
        }

        // SensorReactionLog가 하나도 없으면 2바이트만 전송된다.
        // 이를 받은 Client는 모든 화재 상황이 해제된다.
        public bool SendSensorReactionLogList(ConnectionState state)
        {
            ArrayList arrLogBytes = new ArrayList();
            int nByteCount = 0;

            int nHistoryCount = m_provider.GetTimeHistoryCount();

            for (int i = 0; i < nHistoryCount; i++)
            {
                TimeHistory history = m_provider.GetTimeHistory(i);
                if(history != null)
                {
                    if (history.LastReactionLog == null)
                        continue;
                }  
                else
                {
                    continue;
                }

                byte[] dataBytes = history.LastReactionLog.MakeBytes();
                arrLogBytes.Add(dataBytes);

                nByteCount += dataBytes.Length - 6;
            }

            byte[] bytes = new byte[nByteCount + 6];

            bytes[0] = TCP_ID.SENSOR_REACTION_HISTORY_DATA_LIST;
            bytes[1] = 0;

            int nLogCount = (int)arrLogBytes.Count;
            byte[] chunkBytes = BitConverter.GetBytes(nLogCount * 10);
            int nIndex = 6;

            System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

            for (int i = 0; i < nLogCount; i++)
            {
                byte[] dataBytes = (byte[])arrLogBytes[i];
                int nDataLength = dataBytes.Length - 6;

                // dataBytes가 헤더 정보를 포함하여 있어 이를 제외 하기 위해 시작을 6번째 부터 한다.
                System.Buffer.BlockCopy(dataBytes, 6, bytes, nIndex, nDataLength);
                nIndex += nDataLength;
            }

            try
            {
                return m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendSensorRecationLogList", ex);
            }
            return false;
        }

        // nSituationType : 0(화재탐지), 1(화재신고)
        private static bool GetBroadcastMessage(SensorReactionLog log, BroadcastManager.SituationType type, out string szBroadcastMessage, out int nRepeat, out bool bSiren)
        {
            szBroadcastMessage = "";
            bSiren = false;
            nRepeat = 1;
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // 화재 신고시 방송
            //string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = " + ((int)type).ToString();
            string szText = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            string strSQL = string.Format(szText, ((int)type), SDMSServer.NetworkServer.Instance.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                bool useBroadcast = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 1 ? true : false;
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                bool useSiren = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 1 ? true : false;
                int nRepeatCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (useBroadcast == false)
                {
                    return false;
                }

                bSiren = useSiren;
                //nRepeat = 1;

                int nEquipZoneID = -1;
                int.TryParse(log.Param1, out nEquipZoneID);

                string strLocationName = "";

                if (nEquipZoneID != -1)
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

                    if (equipZone != null)
                        strLocationName = equipZone.BroadcastName;
                }

                if (type == BroadcastManager.SituationType.DETECT_FIRE || type == BroadcastManager.SituationType.REPORT_FIRE)
                    szBroadcastMessage = GetBroadcastMessage(strMessage, strLocationName, log.LogTime, nRepeatCount);
                else
                {
                    // 대피거리(미터)
                    int nPSMDistance;
                    string strPSMMaterialName = GetPSMInfo(log, out nPSMDistance);

                    if (strPSMMaterialName.Length > 0)
                        szBroadcastMessage = GetBroadcastMessage(strMessage, strLocationName, log.LogTime, strPSMMaterialName, nPSMDistance, nRepeatCount);
                    else
                        szBroadcastMessage = strMessage;
                }

            }
            return true;
        }

        // Return 값 : 유해화학물질 이름
        // nPSMDistance : 대피거리(미터)
        private static string GetPSMInfo(SensorReactionLog log, out int nPSMDistance)
        {
            nPSMDistance = 0;
            int nSensorZoneID;

            if (!int.TryParse(log.Param2, out nSensorZoneID))
                return "";

            SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return "";

            PSMSensor sensor = PSMManager.Instance.GetSensor(sensorZone.LinkedSensorID);

            if (sensor == null)
                return "";

            PSMMaterial material = sensor.GetLinkedMaterial();

            if (material == null)
                return "";

            if (sensor.LinkedTankList == null || sensor.LinkedTankList.Count == 0)
                return "";

            PSMTank tank = sensor.LinkedTankList[0];

            int nAlarmDepth;

            if (int.TryParse(log.Param5, out nAlarmDepth))
            {
                if (nAlarmDepth == 1)
                    nPSMDistance = tank.EvacInitDistance;
                else if (nAlarmDepth == 2 || nAlarmDepth == 3)
                {
                    if (IsDayLight(log.LogTime))
                        nPSMDistance = tank.EvacDayDistance;
                    else
                        nPSMDistance = tank.EvacNightDistance;
                }
            }

            return material.Name;
        }

        public static bool IsDayLight(DateTime time)
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where (PropertyName = 'WorkingBeginHour' or PropertyName = 'WorkingEndHour') and SiteID = " + NetworkServer.Instance.SiteID.ToString();
            ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            string strBeginHour = null, strEndHour = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                string strTime = WebDBManager.GetStringField(arrResult[i]);

                if (strTime != null && string.Compare(strTime, "WorkingBeginHour", true) == 0)
                    strBeginHour = strTime;
                else if (strTime != null && string.Compare(strTime, "WorkingEndHour", true) == 0)
                    strEndHour = strTime;
            }

            if (strBeginHour != null && strEndHour != null)
            {
                int nBeginHour = 0, nBeginMinute = 0, nEndHour = 0, nEndMinute = 0;

                if (GetWorkingHours(strBeginHour, ref nBeginHour, ref nBeginMinute) && GetWorkingHours(strEndHour, ref nEndHour, ref nEndMinute))
                {
                    if (time.Hour > nBeginHour)
                    {
                        if (time.Hour < nEndHour)
                            return true;
                        else if (time.Hour == nEndHour)
                            return time.Minute <= nEndMinute;
                    }
                    else if (time.Hour == nBeginHour)
                    {
                        if (time.Minute >= nBeginMinute)
                        {
                            if (time.Hour < nEndHour)
                                return true;
                            else if (time.Hour == nEndHour)
                                return time.Minute <= nEndMinute;
                        }
                    }
                }
            }

            return false;
        }

        private static bool GetWorkingHours(string strWorkingHours, ref int nHour, ref int nMinute)
        {
            int nIndex = strWorkingHours.IndexOf(':');

            if (nIndex < 0)
                return false;

            string strHour = strWorkingHours.Substring(0, nIndex);
            string strMinute = strWorkingHours.Substring(nIndex + 1);

            if (!int.TryParse(strHour, out nHour))
                return false;

            if (!int.TryParse(strMinute, out nMinute))
                return false;

            if (nHour < 0 || nHour > 23)
                return false;

            if (nMinute < 0 || nMinute > 59)
                return false;

            return true;
        }

        // strBeginTag와 strEndTag로 둘러쌓인 부분을 제거한 문자열을 리턴한다.
        // strFullMessage : strBeginTag와 strEndTag를 포함한 문자열
        private static string GetMessage(string strOriginMessage, string strBeginTag, string strEndTag, out string strFullMessage)
        {
            int nLen = strOriginMessage.Length;
            int nIndex = 0;

            string strMessage = "";
            strFullMessage = "";
            int nBeginTagLength = strBeginTag.Length;
            int nEndTagLength = strEndTag.Length;

            while (nIndex < nLen)
            {
                int nIndex1 = strOriginMessage.IndexOf(strBeginTag, nIndex);

                if (nIndex1 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex);
                    break;
                }

                int len = nIndex1 - nIndex;

                if (len > 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex, len);
                    strMessage += strOriginMessage.Substring(nIndex, len);
                }

                int nIndex2 = strOriginMessage.IndexOf(strEndTag, nIndex1 + nBeginTagLength);

                if (nIndex2 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex1);
                    break;
                }

                len = nIndex2 - (nIndex1 + nBeginTagLength);

                if (len > 0)
                    strFullMessage += strOriginMessage.Substring(nIndex1 + nBeginTagLength, len);

                nIndex = nIndex2 + nEndTagLength;
            }

            return strMessage;
        }

        private static string GetBroadcastMessage(string strOriginMessage, string strLocation, DateTime time, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            szBroadcastMessage = ParseSpecialMessage(strOriginMessage, time, strLocation);
            //szBroadcastMessage = szBroadcastMessage.Replace("●", strLocation);
            return szBroadcastMessage;
        }

        // nPSMDistance : 대피거리(미터)
        private static string GetBroadcastMessage(string strOriginMessage, string strLocation, DateTime time, string strPSMMaterialName, int nPSMDistance, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            szBroadcastMessage = ParseSpecialMessage(strOriginMessage, time, strLocation, strPSMMaterialName, nPSMDistance);
            return szBroadcastMessage;
        }

        private static string ParseSpecialMessage(string strOriginMessage, DateTime time, string strLocation)
        {
            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strOriginMessage, time, strLocation);
            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        private static string ParseSpecialMessage(string strOriginMessage, DateTime time, string strLocation, string strPSMMaterialName, int nPSMDistance)
        {
            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strOriginMessage, time, strLocation);
            param.PSMMaterialType = strPSMMaterialName;
            param.PSMDistance = nPSMDistance;

            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        public static void RunBroadcast(SensorReactionLog log, ServiceProvider provider, BroadcastManager.SituationType type)
        {
            // 화재 발생 방송
            string szBroadcastMsg = "";
            int nRepeat = 1;
            bool bUseSiren = false;

            if (BroadcastManager.Instance.IsEnabled(type) == true)
            {
                bool bResult = GetBroadcastMessage(log, type, out szBroadcastMsg, out nRepeat, out bUseSiren);

                if (bResult)
                {
                    SensorReactionLog smsLog = new SensorReactionLog();
                    smsLog.Message = "사내 방송 실시";
                    smsLog.Param1 = log.Param1;
                    smsLog.Param2 = log.Param2;
                    smsLog.Param3 = log.Param3;
                    smsLog.SensorHistoryID = log.SensorHistoryID;
                    smsLog.Type = SensorReactionLog.ReactionType.RUN_BROADCAST;
                    provider.AddReactionLog(smsLog);

                    BroadcastManager.Instance.AddSpeech(szBroadcastMsg, nRepeat, bUseSiren, type);
                }
            }
        }
        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.FIRE_DETECT_REPORT)
            {
                int nSOPGenUserID;

                // SensorZone이 아닌 개별 Sensor ID
                SensorReactionLog log = ReadFireReport(bytes, out nSOPGenUserID);

                // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
                NoControlSimulator(nSOPGenUserID);

                if (m_provider.CheckSituation(log.SensorHistoryID))
                {
                    m_provider.AddReactionLog(log);

                    // 사내방송 실시 - 화재보고에서는 방송내보내지 않는다.(아래로바뀜 20130-12-18)
                    // 사내방송 실시, 화재탐지의 방송을 중단하고  다시 화재신고시에 방송을보냄으로 변경'
                    // 삼천포 김명수대리요청
                    RunBroadcast(log, m_provider, BroadcastManager.SituationType.REPORT_FIRE);

                    // SMS전송
                    //m_provider.SendSMSToAllCompanyMember(log);
                    if (DataManager.Instance.UseReportFacilityManagers)
                        m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_FIRE);
                    else
                        m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_FIRE);

                    m_provider.MonitorNotifyFireProcess(log);
                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT);
                    m_provider.SendStatusSensorSignal(log);
                }
                else
                {
                    // 수동 신고의 처리
                    if (log.Param2 == "0")
                    {
                        ProcessManualFireDetect(log);
                    }

                }
            }
            else if (nHeader == TCP_ID.PSM_DETECT_REPORT)
            {
                int nSOPGenUserID;

                // SensorZone이 아닌 개별 Sensor ID
                SensorReactionLog log = ReadSpillReport(bytes, out nSOPGenUserID);

                // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
                NoControlSimulator(nSOPGenUserID);

                if (m_provider.CheckSituation(log.SensorHistoryID))
                {
                    m_provider.AddReactionLog(log);

                    RunBroadcast(log, m_provider, BroadcastManager.SituationType.REPORT_PSM);
                    // SMS전송
                    //m_provider.SendSMSToAllCompanyMember(log, true);
                    if (DataManager.Instance.UseReportFacilityManagers)
                        m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_PSM);
                    else
                        m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_PSM);

                    m_provider.MonitorNotifyFireProcess(log);
                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT);
                    m_provider.SendStatusSensorSignal(log);
                }
                else
                {
                    // 수동 신고의 처리
                    if (log.Param2 == "0")
                    {
                        ProcessManualFireDetect(log);
                    }

                }
            }
            else if (nHeader == TCP_ID.SECURITY_DETECT_REPORT)
            {
                int nSOPGenUserID;

                // SensorZone이 아닌 개별 Sensor ID
                SensorReactionLog log = ReadSecurityReport(bytes, out nSOPGenUserID);

                // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
                NoControlSimulator(nSOPGenUserID);

                if (m_provider.CheckSituation(log.SensorHistoryID))
                {
                    m_provider.AddReactionLog(log);

                    // 방범은 방송 없음
                    //RunBroadcast(log, m_provider, BroadcastManager.SituationType.R);

                    // SMS전송
                    //m_provider.SendSMS(log);

                    // 관련인원에게 문자 전송
                    if (DataManager.Instance.UseReportFacilityManagers)
                        m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_SECURITY);
                    else
                        m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_SECURITY);

                    m_provider.MonitorNotifySecurityProcess(log);
                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT);
                    m_provider.SendStatusSensorSignal(log);
                }                
            }
            else if (nHeader == TCP_ID.PSM_SENSOR_RESET)
            {
                ProcessPSMReset(bytes);
            }
            else if (nHeader == TCP_ID.PSM_BUZZER_STOP)
            {
                ProcessPSMBuzzer(bytes);
            }
            else if (nHeader == TCP_ID.MALFUNCTION_REPORT)
            {
                ProcessMalfunction(bytes, arrDatas);
            }

            else if (nHeader == TCP_ID.CHANGE_CONFIG)
            {
                ProcessChangedConfig(arrDatas, bytes);
            }
            else if (nHeader == TCP_ID.CLEAR_DETECT_REPORT)
            {
                ProcessDetectReportClear(bytes);
            }
            else if (nHeader == TCP_ID.REQUEST_RESTORE)
            {

                bool bExist = m_provider.ExistFireDetectSituation();
                if (bExist == true)
                {
                    byte[] sendbytes = new byte[6] { TCP_ID.REJECT_RESTORE, 0, 0, 0, 0, 0 };
                    try
                    {
                        m_provider.Send(sendbytes, 0, sendbytes.Length, state);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("RejectResotre", ex);
                    }

                }
                else
                {
                    byte[] sendbytes = new byte[6] { TCP_ID.ACCEPT_RESTORE, 0, 0, 0, 0, 0 };
                    try
                    {
                        m_provider.Send(sendbytes, 0, sendbytes.Length, state);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("AcceptRestore", ex);
                    }


                    m_provider.SendBeginRestore();
                }

            }
            else if (nHeader == TCP_ID.REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST)
            {
                SendSensorReactionLogList(state);
            }
            else if (nHeader == TCP_ID.EDIT_SENSOR_ZONE)
            {
                if (ProcessEditSensorZone(arrDatas))
                    SendEditSensorZone(bytes);
            }
            else if (nHeader == TCP_ID.SDMS_COMMAND)
            {
                ProcessSDMSCommand(arrDatas, bytes);
            }

            return true;
        }

        private void ProcessSDMSCommand(ArrayList arrDatas, byte[] bytes)
        {

            int nDataCount = arrDatas.Count;

            if (nDataCount == 0 || (arrDatas[0] is byte) == false)
                return;

            byte cmd = (byte)arrDatas[0];

            if (cmd == SDMSCommandType.CHANGE_PSM_SENSOR_STATUS)
            {
                if (ProcessChangePSMSensorStatus(arrDatas))
                    SendDataToSDMSClient(bytes);
            }
            else if (cmd == SDMSCommandType.PSM_SENSOR_ALARM_LEVEL)
            {
                if (SavePSMSensorAlarmLevel(arrDatas))
                    SendDataToSDMSClient(bytes);
            }
            else if (cmd == SDMSCommandType.CHANGE_TAG_ACTIVATION)
            {
                if(SaveChangeTagActivation(arrDatas))
                    SendDataToSDMSClient(bytes);
            }
            else
                SendDataToSDMSClient(bytes);
        }
        private bool SaveChangeTagActivation(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            if (nDataCount > 2 && (arrDatas[1] is int))     
            {   
                int endOfDatas = (int)arrDatas[1] + 2;
                for (int i = 2; i < endOfDatas; i++)
                {
                    int tagID = (int)arrDatas[i];
                    string deActivationCode = (string)arrDatas[++i];
                    SensorManager.Instance.updateSensorTagDeactivation(tagID, deActivationCode);
                }                
                return true;
            }
            return false;
            
        }
        private bool SavePSMSensorAlarmLevel(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount >= 5 && (arrDatas[1] is int) && (arrDatas[2] is float) && (arrDatas[3] is float) && (arrDatas[4] is float))
            {
                int nPSMSensorID = (int)arrDatas[1];
                float fLevel1 = (float)arrDatas[2];
                float fLevel2 = (float)arrDatas[3];
                float fLevel3 = (float)arrDatas[4];

                string strSQL = string.Format("Update PSMSensor set LimitLevel1 = {0}, LimitLevel2 = {1}, LimitLevel3 = {2} where ID = {3}",
                    fLevel1, fLevel2, fLevel3, nPSMSensorID);

                if (NetworkServer.Instance.DBManager.GetResultData(strSQL, 0) != null)
                {
                    PSMSensor sensor = PSMManager.Instance.GetSensor(nPSMSensorID);

                    if (sensor != null)
                    {
                        sensor.LimitLevel1 = fLevel1;
                        sensor.LimitLevel2 = fLevel2;
                        sensor.LimitLevel3 = fLevel3;
                    }

                    return true;
                }
            }

            return false;
        }

        private void SendDataToSDMSClient(byte[] bytes)
        {
            m_provider.SendClientData(bytes, TCP_CLIENT.SDMS_CLIENT, false);
        }

        private bool ProcessChangePSMSensorStatus(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount != 6)
                return false;

            if ((arrDatas[1] is int) && (arrDatas[2] is byte) && (arrDatas[3] is long) && (arrDatas[4] is long) && (arrDatas[5] is int))
            {
                int nSensorID = (int)arrDatas[1];
                byte status = (byte)arrDatas[2];
                long beginTime = (long)arrDatas[3];
                long endTime = (long)arrDatas[4];
                int nSOPGenUserID = (int)arrDatas[5];

                PSMSensor sensor = PSMManager.Instance.GetSensor(nSensorID);

                if (sensor != null)
                {
                    bool prevOff = sensor.IsOff();
                    PSMSensor.Status _status = PSMSensor.ToStatus((int)status);

                    bool spreadStatus = sensor.SensorStatus == PSMSensor.Status.Off4Work || _status == PSMSensor.Status.Off4Work;
                    sensor.SensorStatus = _status;

                    if (beginTime != 0)
                    {
                        DateTime dtBegin = DateTime.FromBinary(beginTime);
                        sensor.BeginWorkTime = new DBUtility.VariousData<DateTime>(dtBegin);
                    }
                    else
                        sensor.BeginWorkTime = null;

                    if (endTime != 0)
                    {
                        DateTime dtEnd = DateTime.FromBinary(endTime);
                        sensor.EndWorkTime = new DBUtility.VariousData<DateTime>(dtEnd);
                    }
                    else
                        sensor.EndWorkTime = null;

                    bool currentOff = sensor.IsOff();

                    return ClearPSMSensorAlarmNChangeStatusDB(sensor, nSOPGenUserID, spreadStatus, prevOff != currentOff);
                    // sensor와 관련된 알람을 해제한다.
                    /*ClearPSMSensorAlarm(sensor, nSOPGenUserID);

                    if (sensor.SensorStatus == PSMSensor.Status.On)
                    {
                        RequestPSMSensorAlarm(sensor);
                    }

                    if (spreadStatus)
                    {
                        List<PSMSensor> sensors = sensor.GetSameSensors();
                        sensors.Add(sensor);
                        return ChangePSMSensorStatusDBDatas(sensors);
                    }

                    return ChangePSMSensorStatusDBData(sensor);*/
                }
            }

            return false;
        }

        // spreadStatus : 같은 센서를 공유하는 다른 Tank들에도 상태정보가 변경된 것을 전파할 것인가?
        public static bool ClearPSMSensorAlarmNChangeStatusDB(PSMSensor sensor, int nSOPGenUserID, bool spreadStatus, bool onOffIsChanged)
        {
            if (onOffIsChanged)
            {
                // sensor와 관련된 알람을 해제한다.
                ClearPSMSensorAlarm(sensor, nSOPGenUserID);

                if (sensor.SensorStatus == PSMSensor.Status.On)
                {
                    RequestPSMSensorAlarm(sensor);
                }
            }

            if (spreadStatus)
            {
                List<PSMSensor> sensors = sensor.GetSameSensors();
                sensors.Add(sensor);
                return ChangePSMSensorStatusDBDatas(sensors);
            }

            return ChangePSMSensorStatusDBData(sensor);
        }

        // PSMSensorServer에게 sensor와 관련된 수신반에 복구 요청을 한다.
        private static void RequestPSMBuzzer(int nPSMSensosrZoneID, int nOnOff)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(ServerCommandType.REQUEST_PSM_BUZZER);
            arrDatas.Add(nPSMSensosrZoneID);
            arrDatas.Add(nOnOff);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SERVER_COMMAND, arrDatas);
            NetworkServer.Instance.ServiceProvider.SendData(bytes, false, TCP_CLIENT.PSM_SENSOR_SERVER);
        }


        internal static void RequestPSMSensorTestAlarmForEnergy(int nPSMSensorZoneID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(ServerCommandType.REQUEST_PSM_TEST_ALARM);
            arrDatas.Add(nPSMSensorZoneID);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SERVER_COMMAND, arrDatas);
            NetworkServer.Instance.ServiceProvider.SendData(bytes, false, TCP_CLIENT.PSM_SENSOR_SERVER);
        }

        // PSMSensorServer에게 sensor와 관련된 수신반에 복구 요청을 한다.
        private static void RequestPSMSensorReset(int nPSMSensosrZoneID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(ServerCommandType.REQUEST_PSM_SENSOR_RESET);
            arrDatas.Add(nPSMSensosrZoneID);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SERVER_COMMAND, arrDatas);
            NetworkServer.Instance.ServiceProvider.SendData(bytes, false, TCP_CLIENT.PSM_SENSOR_SERVER);
        }


        // PSMSensorServer에게 sensor와 관련된 알람이 존재하면 보내줄 것을 요청한다.
        private static void RequestPSMSensorAlarm(PSMSensor sensor)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(ServerCommandType.REQUEST_PSM_SENSOR_ALARM);
            arrDatas.Add(sensor.ID);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SERVER_COMMAND, arrDatas);
            NetworkServer.Instance.ServiceProvider.SendData(bytes, false, TCP_CLIENT.PSM_SENSOR_SERVER);
        }

        // sensor와 관련된 알람을 해제한다.
        public static void ClearPSMSensorAlarm(PSMSensor sensor, int nSOPGenUserID)
        {
            SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(sensor.ID, IFacility.FacilityType.PSM_SENSOR, sensor.EquipmentZone);

            if (sensorZone == null)
                return;

            int nSensorZoneHistoryID = SensorManager.Instance.GetSensorHistoryID(sensorZone.ID);

            if (nSensorZoneHistoryID < 0)
                return;

            SensorReactionLog log = new SensorReactionLog();

            log.SensorHistoryID = nSensorZoneHistoryID;
            log.Param2 = sensorZone.ID.ToString();
            log.Param3 = nSOPGenUserID.ToString();
            log.Message = "탐지된 누출신호가 무시됩니다.";
            log.Param1 = sensorZone.EquipZone == null ? "-1" : sensorZone.EquipZone.ID.ToString();
            log.Type = SensorReactionLog.ReactionType.MALFUNCTION;
            log.Status = SensorReactionLog.DetectionStatus.REAL;

            ProcessMalfunction(log);
        }

        private static bool ChangePSMSensorStatusDBDatas(List<PSMSensor> sensors)
        {
            foreach (PSMSensor sensor in sensors)
            {
                if (!ChangePSMSensorStatusDBData(sensor))
                    return false;
            }

            return true;
        }

        private static bool ChangePSMSensorStatusDBData(PSMSensor sensor)
        {
            string strSQL = "Select SensorID from PSMSensorSchedule where SensorID = " + sensor.ID.ToString();
            ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return InsertPSMSensorStatus(sensor);
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return InsertPSMSensorStatus(sensor);
            }

            return UpdatePSMSensorStatus(sensor);
        }

        private static bool InsertPSMSensorStatus(PSMSensor sensor)
        {
            string strBeginTime = GetTimeString(sensor.BeginWorkTime);
            string strEndTime = GetTimeString(sensor.EndWorkTime);

            string strSQL = string.Format("Insert into PSMSensorSchedule (SensorID, Status, BeginTime, EndTime, Description) values ({0}, {1}, {2}, {3}, NULL)",
                sensor.ID, (int)sensor.SensorStatus, strBeginTime, strEndTime);

            return NetworkServer.Instance.DBManager.GetResultData(strSQL, 0) != null;
        }

        private static bool UpdatePSMSensorStatus(PSMSensor sensor)
        {
            string strBeginTime = GetTimeString(sensor.BeginWorkTime);
            string strEndTime = GetTimeString(sensor.EndWorkTime);

            string strSQL = string.Format("Update PSMSensorSchedule set Status = {0}, BeginTime = {1}, EndTime = {2} where SensorID = {3}",
                (int)sensor.SensorStatus, strBeginTime, strEndTime, sensor.ID);

            return NetworkServer.Instance.DBManager.GetResultData(strSQL, 0) != null;
        }

        private static string GetTimeString(VariousData<DateTime> time)
        {
            if (time == null)
                return "NULL";

            string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'",
                time.Data.Year, time.Data.Month, time.Data.Day,
                time.Data.Hour, time.Data.Minute, time.Data.Second);

            return strTime;
        }

        private void SendEditSensorZone(byte[] bytes)
        {
            m_provider.SendDataToOther(bytes, this, false, TCP_CLIENT.SDMS_CLIENT);
            m_provider.SendClientData(bytes, TCP_CLIENT.SENSOR_MONITOR2, false);
            m_provider.SendClientData(bytes, TCP_CLIENT.PSM_SENSOR_SERVER, false);
        }

        private bool ProcessEditSensorZone(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount % 4 != 0)
                return false;

            for (int i = 0; i < nDataCount; i += 4)
            {
                int nSensorZoneID = (int)arrDatas[i];
                int nOriginEquipZoneID = (int)arrDatas[i + 1];
                int nChangedEquipZoneID = (int)arrDatas[i + 2];
                int nZoneID = (int)arrDatas[i + 3];

                SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    continue;

                EquipmentZone equipZoneOrigin = ZoneManager.Instance.GetEquipmentZone(nOriginEquipZoneID);
                EquipmentZone equipZoneChanged = ZoneManager.Instance.GetEquipmentZone(nChangedEquipZoneID);

                if (equipZoneOrigin != null)
                {
                    ArrayList arrSensorZones;

                    if (NetworkServer.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneOrigin, out arrSensorZones))
                    {
                        arrSensorZones.Remove(sensorZone);
                    }
                }

                if (equipZoneChanged != null)
                {
                    ArrayList arrSensorZones;

                    if (!NetworkServer.Instance.IOManager.D_EquipZoneSensor.TryGetValue(equipZoneChanged, out arrSensorZones))
                    {
                        arrSensorZones = new ArrayList();
                        NetworkServer.Instance.IOManager.D_EquipZoneSensor[equipZoneChanged] = arrSensorZones;
                    }

                    if (!arrSensorZones.Contains(sensorZone))
                        arrSensorZones.Add(sensorZone);
                }

                sensorZone.ZoneID = nZoneID;
                sensorZone.EquipZone = equipZoneChanged;

                if (!UpdateSensorZoneDB(sensorZone))
                    return false;
            }

            return true;
        }

        private bool UpdateSensorZoneDB(SensorZone sensorZone)
        {
            if (sensorZone == null)
                return true;

            string strSQL = string.Format("Update SensorZone set EquipZoneID = {0}, Zone = {1} where ID = {2}",
                sensorZone.EquipZone == null ? 0 : sensorZone.EquipZone.ID,
                sensorZone.ZoneID,
                sensorZone.ID);

            return NetworkServer.Instance.DBManager.GetResultData(strSQL, 0) != null;
        }

        private void ProcessChangedConfig(ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count < 3)
                return;

            try
            {
                byte byteClientType = (byte)arrDatas[0];
                string strPropertyName = (string)arrDatas[1];
                string strPropertyValue = (string)arrDatas[2];

                if (byteClientType != TCP_CLIENT.SDMS_CLIENT)
                    return;

                if (strPropertyName == SDMSConfig.PropertyName)
                {
                    int nConfigValue;

                    if (int.TryParse(strPropertyValue, out nConfigValue))
                    {
                        if (((nConfigValue & (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER))
                            ProcessChangeFacilityManager(bytes);
                    }
                }
                else if (strPropertyName == SDMSConfig.GetPropertyName(SDMSConfig.ConfigType.EQUIPZONE_CCTV))
                {
                    int nEquipZoneID;

                    if (int.TryParse(strPropertyValue, out nEquipZoneID))
                    {
                        ProcessChangeEquipZoneCCTV(bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("ProcessChangedConfig", ex);
            }
        }

        // nSOPGenUserID를 가진 SOP Simulator가 제어권을 가져갈 때까지 10초간 기다린다.
        // 그동안은 아무도 제어권을 가지지 않는 상태가 된다.
        private void NoControlSimulator(int nSOPGenUserID)
        {
            Thread t = new Thread(new ParameterizedThreadStart(NoControlThread));
            t.Start(nSOPGenUserID);
        }

        // 
        private void NoControlThread(object param)
        {
            int nSOPGenUserID = (int)param;

            ControlMonitoring.ControlManager.Instance.ControlClient = null;
            ControlMonitoring.ControlManager.Instance.ControlSOPGenUserID = nSOPGenUserID;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSOPGenUserID);
            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.GIVE_CONTROL_KEY, arrDatas);

            for (int i = 0; i < 15; i++)
            {
                m_provider.SendData(bytes, false, TCP_CLIENT.SOP_SIMULATOR);
                Thread.Sleep(1000);

                // 제어권 소유자가 생겼다.
                if (ControlMonitoring.ControlManager.Instance.ControlSOPGenUserID < 0)
                {
                    return;
                }
            }

            // 제어권 소유자가 아무도 없으므로 접속한 Client 가운데 첫번째 Client에게 제어권을 넘긴다.
            ControlMonitoring.ControlManager.Instance.ControlSOPGenUserID = -1;

            byte[] bytes2 = TcpHelper.MakeBytes(TCP_ID.GIVE_CONTROL, null);
            m_provider.SendData(bytes2, false, TCP_CLIENT.SOP_SIMULATOR, 1);
        }

        private void ProcessDetectReportClear(byte[] bytes)
        {
            int nPrevSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            int nGenUserID = BitConverter.ToInt32(bytes, 20);

            if (nPrevSensorHistoryID > 0)
            {
                TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

                if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
                {
                    byte nClientType = TCP_CLIENT.SDMS_CLIENT;

                    PingCount = 0;
                    // 화재 상황 종료


                    m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType);

                    int nZoneID = -1;
                    Zone zone = null;
                    
                    if (int.TryParse(history.LastReactionLog.Param1, out nZoneID))
                    {
                        ZoneManager.Instance.DicZones.TryGetValue(nZoneID, out zone);
                    }

                    m_provider.RemoveTimeHistory(history);
                    SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);

                    if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    {
                        SensorReactionLog log = new SensorReactionLog();
                        log.Param1 = nZoneID.ToString();
                        log.Param2 = "0";
                        log.Param3 = nGenUserID.ToString();
                        log.LogTime = DateTime.Now;
                        //log.Message = "화재 신호가 무시되었습니다.";

                        if (zone != null)
                        {
                            string szLocationName = zone.DisplayText;
                            log.Message = string.Format("[{0}]에서 수동신고된 화재신호가 복구되었습니다", szLocationName);
                        }
                        else
                            log.Message = "화재 신호가 무시되었습니다.";

                        log.SensorHistoryID = nPrevSensorHistoryID;
                        log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

                        m_provider.AddReactionLog(log);

                        // m_provider.SendSMSToAllCompanyMember(log);
                        if (DataManager.Instance.UseReportFacilityManagers)
                            m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_FIRE);
                        else
                            m_provider.SendSMSToAllCompanyMember(log, ServiceProvider.GetSMSMessageTypeFromLog(log));
                    }
                }
            }
        }

        private void ProcessChangeEquipZoneCCTV(byte[] bytes)
        {
            //int nEquipZoneID = ReadChangeEquipZoneCCTV(bytes);	


            m_provider.SendDataToOther(bytes, this, false, TCP_CLIENT.SDMS_CLIENT);
        }

        private void ProcessChangeFacilityManager(byte[] bytes)
        {
            m_provider.SendDataToOther(bytes, this, false, TCP_CLIENT.SDMS_CLIENT);

            DataManager.Instance.LoadFacilityManager();
        }

        private void ProcessPSMReset(byte[] bytes)
        {
            SensorReactionLog log = ReadUserReset(bytes);

            string szSensorZoneID = log.Param2;

            int nSensorZoneID = -1;
            if (int.TryParse(szSensorZoneID, out nSensorZoneID))
            {
                RequestPSMSensorReset(nSensorZoneID);

                // SensorZoneGroup 비우기
                SensorZoneGroup sensorZoneGroup = NetworkServer.Instance.IOManager.GetSensorZoneGroup(nSensorZoneID);

                if (sensorZoneGroup != null)
                    sensorZoneGroup.SensorDatas.Clear();
            }

            ProcessPSMUserReset(log);
        }

        private void ProcessPSMBuzzer(byte[] bytes)
        {
            // 로그를 사용하지 않으므로 UserReset을 사용한다. 별도로 필요한경우 Read함수를 새로 만든다.
            SensorReactionLog log = ReadUserReset(bytes);

            int nOnOff = log.SensorHistoryID;
            string szSensorZoneID = log.Param2;

            int nSensorZoneID = -1;
            if (int.TryParse(szSensorZoneID, out nSensorZoneID))
                RequestPSMBuzzer(nSensorZoneID, nOnOff);
        }


        private void ProcessMalfunction(byte[] bytes, ArrayList arArgs)
        {

            int nHistoryID = 0;
            TimeHistory history = m_provider.FindTimeHistory(nHistoryID);
            if( history != null)
            {
                
            }

            SensorReactionLog log = ReadMalfunctionReport(bytes);

            ProcessMalfunction(log);
        }

        private static void ProcessPSMUserReset(SensorReactionLog log)
        {
            ServiceProvider provider = NetworkServer.Instance.ServiceProvider;

            if (provider.CheckSituation(log.SensorHistoryID))
            {
                NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(log.SensorHistoryID);
                int nSensorID = NetworkServer.Instance.SensorManager.GetSensorID(log.SensorHistoryID);

                provider.AddReactionLog(log);
                provider.SendSMS(log, ServiceProvider.GetSMSMessageTypeFromLog(log));

                provider.RemoveSituation(log.SensorHistoryID);
                if (nSensorID > 0)
                {
                    SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);

                    if (sensor != null && sensor.SensorData == 1)
                    {
                        // 무시할 센서 리스트에 포함
                        provider.AddTempIgnoreSensor(sensor);

                        AbnormalSensorManager.Instance.Add(sensor.ID);
                    }
                }

                // Send Reaction Log
                provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT);
            }
        }

        private static void ProcessMalfunction(SensorReactionLog log)
        {
            ServiceProvider provider = NetworkServer.Instance.ServiceProvider;

            if (provider.CheckSituation(log.SensorHistoryID))
            {
                NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(log.SensorHistoryID);
                int nSensorID = NetworkServer.Instance.SensorManager.GetSensorID(log.SensorHistoryID);

                log.Status = SensorReactionLog.DetectionStatus.MALFUNCTION;

                provider.AddReactionLog(log);
                provider.SendSMS(log, ServiceProvider.GetSMSMessageTypeFromLog(log));

                provider.RemoveSituation(log.SensorHistoryID);
                if (nSensorID > 0)
                {
                    SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);

                    if (sensor != null && sensor.SensorData == 1)
                    {
                        // 무시할 센서 리스트에 포함
                        provider.AddTempIgnoreSensor(sensor);

                        AbnormalSensorManager.Instance.Add(sensor.ID);
                    }
                }

                // Send Reaction Log
                provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT);
            }
        }

        public int ReadChangeEquipZoneCCTV(byte[] bytes)
        {
            int chunkSize = BitConverter.ToInt32(bytes, 2);
            int nReadDataCount = 6;

            int nEquipZoneID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }
            return nEquipZoneID;
        }

        public SensorReactionLog ReadUserReset(byte[] bytes)
        {
            SensorReactionLog log = new SensorReactionLog();

            //int nReadDataCount = 1;
            //int chunkSize = (int)bytes[nReadDataCount++];
            int chunkSize = BitConverter.ToInt32(bytes, 2);
            int nReadDataCount = 6;

            int nSensorHistoryID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }
            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            int nSensorID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;
            //ResetSensorData(nSensorID);
            log.Param2 = nSensorID.ToString();

            int nSOPGenUser = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSOPGenUser = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;

            if (nSOPGenUser != -1)
                log.Param3 = nSOPGenUser.ToString();

            string strDescriptionText = "";
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.STRING)
                {
                    strDescriptionText = Encoding.UTF8.GetString(bytes, nReadDataCount + 5, nDataLength - 5);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;

            if (strDescriptionText.Length > 0)
                log.DescriptionText = strDescriptionText;

            int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = "탐지된 누출센서가 시스템 복구되었습니다.";
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("[{0}]에서 탐지된 누출신호가 시스템 복구되었습니다", szLocationName);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Type = SensorReactionLog.ReactionType.PSM_USER_RESET;
            log.SensorHistoryID = nSensorHistoryID;

            return log;
        }

        private string GetTypeString(IFacility.FacilityType eventType)
        {
            string resultMsg = "화재";
            switch (eventType)
            {
                case IFacility.FacilityType.Intrusion_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.Loiter_S1:
                    resultMsg = "배회";
                    break;
                case IFacility.FacilityType.Collapse_S1:
                    resultMsg = "넘어짐";
                    break;
                case IFacility.FacilityType.Theft_S1:
                    resultMsg = "도난";
                    break;
                case IFacility.FacilityType.Neglect_S1:
                    resultMsg = "방치";
                    break;
                case IFacility.FacilityType.VirtualFence_S1:
                    resultMsg = "(가상펜스)침입";
                    break;
                case IFacility.FacilityType.Fire_S1:
                    resultMsg = "화재";
                    break;
                case IFacility.FacilityType.EmergencyBell_S1:
                    resultMsg = "비상벨";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT1_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT2_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.InternalIntrusionT3_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.VaultIntrusionT4_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.FireF1_S1:
                    resultMsg = "화재";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC1_S1:
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC2_S1:
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.RescueQQ_S1:
                    resultMsg = "구급";
                    break;
                case IFacility.FacilityType.GasG1_S1:
                    resultMsg = "가스누출";
                    break;
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                    resultMsg = "정전";
                    break;
                case IFacility.FacilityType.LeakAbnormalityU4_S1:
                    resultMsg = "누수";
                    break;
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    resultMsg = "종합경보반 이상";
                    break;
                case IFacility.FacilityType.ExternalAlarmBell:
                    resultMsg = "비상벨 호출";
                    break;
                default:
                    break;
            }
            return resultMsg;
        }


        public SensorReactionLog ReadMalfunctionReport(byte[] bytes)
        {
            SensorReactionLog log = new SensorReactionLog();

            //int nReadDataCount = 1;
            //int chunkSize = (int)bytes[nReadDataCount++];
            int chunkSize = BitConverter.ToInt32(bytes, 2);
            int nReadDataCount = 6;

            int nSensorHistoryID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }
            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            int nSensorID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;
            //ResetSensorData(nSensorID);
            log.Param2 = nSensorID.ToString();

            IFacility.FacilityType sensorType = IFacility.FacilityType.FIRE_SENSOR;
            string szTypeMsg = "화재";
            try
            {
                SensorZone sz = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
                if (sz != null)
                {
                    sensorType = sz.Type;
                    szTypeMsg = GetTypeString(sensorType);
                }
            }
            catch(Exception)
            {
            }
            
            int nSOPGenUser = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSOPGenUser = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;

            if (nSOPGenUser != -1)
                log.Param3 = nSOPGenUser.ToString();

            string strDescriptionText = "";
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.STRING)
                {
                    strDescriptionText = Encoding.UTF8.GetString(bytes, nReadDataCount + 5, nDataLength - 5);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;

            if (strDescriptionText.Length > 0)
                log.DescriptionText = strDescriptionText;

            int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);


            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("탐지된 {0} 신호가 오작동으로 신고되었습니다", szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("[{0}]에서 탐지된 {1} 신호가 오작동으로 신고되었습니다", szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Type = SensorReactionLog.ReactionType.MALFUNCTION;
            log.SensorHistoryID = nSensorHistoryID;

            return log;
        }

        private void ProcessManualFireDetect(SensorReactionLog log)
        {
            int nZoneID = -1;
            int.TryParse(log.Param1, out nZoneID);
            int nPrevHistoryID = -1;
            if (SensorManager.Instance.GetSensorHistoryIDForManual(nZoneID, ref  nPrevHistoryID) != -1)
            {
                return;
            }

            string sqlID = "select max(id) as id from SensorZoneHistory";
            ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(sqlID, 0);
            int nResultCount = arrResult.Count;

            int nHistoryID = 0;
            for (int i = 0; i < nResultCount; i += 1)
            {
                //Data가 아예 안들어가 있을경우 0부터 시작
                int Find_Maxid = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                nHistoryID = Find_Maxid;
            }
            nHistoryID++;

            DateTime dtNow = DateTime.Now;
            string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            //History
            //string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time) Values('"
            //	+ nHistoryID + "','" + 0 + "','" + 1 + "','" + 1 + "','" + strDateTimeField + "')";

            string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time, param1, SiteID ) Values('"
                + nHistoryID + "','" + 0 + "','" + 1 + "','" + 1 + "','" + strDateTimeField + "','" + log.Param1 + "','" + m_nSiteID + "')";
            NetworkServer.Instance.DBManager.GetResultData(sqlInsert, 0);

            NetworkServer.Instance.SensorManager.DicSensorHistory[nHistoryID] = 0;

            log.SensorHistoryID = nHistoryID;
            log.Status = SensorReactionLog.DetectionStatus.REAL;
           
            TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now, SensorReactionLog.DetectionStatus.REAL);
            m_provider.AddTimeHistory(hs);

            m_provider.AddReactionLog(log);

            RunBroadcast(log, m_provider, BroadcastManager.SituationType.REPORT_FIRE);
            
            
            // SMS전송
            //m_provider.SendSMSToAllCompanyMember(log);
            if (DataManager.Instance.UseReportFacilityManagers)
                m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_FIRE);
            else
                m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_FIRE);
            

            m_provider.MonitorNotifyFireProcess(log);
            m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT);


            bool bReal = !DataManager.GetTranningMode();
            float x = 0.0f;
            float y = 0.0f;
            float z = 0.0f;

            ArrayList arDatas = new ArrayList();
            arDatas.Add(0);
            arDatas.Add(log.SensorHistoryID);
            arDatas.Add(nZoneID);
            arDatas.Add(log.LogTime.ToBinary());           
            arDatas.Add(x);
            arDatas.Add(y);
            arDatas.Add(z);
            arDatas.Add(bReal == true ? 0 : 1);


            byte[] bytes = TcpHelper.MakeBytes( TCP_ID.FIRE_SENSOR_SIGNAL, arDatas);


            //byte[] sensorIDBytes = ServiceProvider.MakeBytes(0);
            //byte[] sensorHistoryIDBytes = ServiceProvider.MakeBytes(log.SensorHistoryID);
            //byte[] zoneIDBytes = ServiceProvider.MakeBytes(nZoneID);
            //byte[] timeBytes = ServiceProvider.MakeBytes(log.LogTime.ToBinary());
            //byte[] xBytes = ServiceProvider.MakeBytes(x);
            //byte[] yBytes = ServiceProvider.MakeBytes(y);
            //byte[] zBytes = ServiceProvider.MakeBytes(z);            
            //byte[] realByte = ServiceProvider.MakeBytes(bReal == true ? 0 : 1);

            //int nBlockLength = sensorIDBytes.Length + sensorHistoryIDBytes.Length + zoneIDBytes.Length + timeBytes.Length + xBytes.Length + yBytes.Length + zBytes.Length + realByte.Length;
            //byte[] bytes = new byte[6 + nBlockLength];

            //bytes[0] = TCP_ID.FIRE_SENSOR_SIGNAL;
            //bytes[1] = 0;

            //int nChunkCount = 8;
            //byte[] chunkBytes = BitConverter.GetBytes(nChunkCount);
            //System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

            //int nIndex = 6;
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, sensorIDBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, sensorHistoryIDBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, zoneIDBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, timeBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, xBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, yBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, zBytes);
            //TcpLib2.TcpHelper.CopyBytes(bytes, ref nIndex, realByte);

            ServiceProvider.SendData(bytes, true, TCP_CLIENT.SOP_SIMULATOR);
        }


        private SensorReactionLog ReadSpillReport(byte[] bytes, out int nSOPGenUserID)
        {
            SensorReactionLog log = new SensorReactionLog();

            //int nReadDataCount = 1;
            //int chunkSize = (int)bytes[nReadDataCount++];
            int chunkSize = BitConverter.ToInt32(bytes, 2);
            int nReadDataCount = 6;

            int nSensorHistoryID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }

            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            int nEquipZoneID = -1;
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }
            chunkSize -= 1;

            log.Param1 = nEquipZoneID.ToString();

            int nSensorID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;
            log.Param2 = nSensorID.ToString();

            nSOPGenUserID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSOPGenUserID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }

            if (nSOPGenUserID != -1)
            {
                log.Param3 = nSOPGenUserID.ToString();
            }

            TimeHistory history = this.ServiceProvider.FindTimeHistory(log.SensorHistoryID);

            if (history != null)
            {
                log.Status = history.LastReactionLog.Status;
            }

            GetSpillReportString(nEquipZoneID, nSensorID, log);
            log.Type = SensorReactionLog.ReactionType.NOTIFY_PSM;
            return log;
        }

        private SensorReactionLog ReadSecurityReport(byte[] bytes, out int nSOPGenUserID)
        {
            SensorReactionLog log = new SensorReactionLog();
            log.Status = SensorReactionLog.DetectionStatus.REAL;

            //int nReadDataCount = 1;
            //int chunkSize = (int)bytes[nReadDataCount++];
            int chunkSize = BitConverter.ToInt32(bytes, 2);
            int nReadDataCount = 6;

            int nSensorHistoryID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }

            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            int nEquipZoneID = -1;
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }
            chunkSize -= 1;

            log.Param1 = nEquipZoneID.ToString();

            int nSensorID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;
            log.Param2 = nSensorID.ToString();

            nSOPGenUserID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSOPGenUserID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }

            if (nSOPGenUserID != -1)
            {
                log.Param3 = nSOPGenUserID.ToString();
            }

            TimeHistory history = this.ServiceProvider.FindTimeHistory(log.SensorHistoryID);
            int nEventType = -1;

            if (history != null)
            {
                log.Status = history.LastReactionLog.Status;
                nEventType = GetSecurityEventType(history);
            }

            GetSecurityReportString(nEquipZoneID, log, nEventType);
            log.Type = SensorReactionLog.ReactionType.NOTIFY_SECURITY;
            return log;
        }

        private int GetSecurityEventType(TimeHistory history)
        {
            if (history.LastReactionLog == null)
                return -1;

            if (history.LastReactionLog.Param3 == null)
                return -1;

            int nEventType;

            if (int.TryParse(history.LastReactionLog.Param3.Trim(), out nEventType) == false)
                return -1;

            return nEventType;
        }

        private SensorReactionLog ReadFireReport(byte[] bytes, out int nSOPGenUserID)
        {
            SensorReactionLog log = new SensorReactionLog();
            log.Status = SensorReactionLog.DetectionStatus.REAL;

            //int nReadDataCount = 1;
            //int chunkSize = (int)bytes[nReadDataCount++];
            int chunkSize = BitConverter.ToInt32(bytes, 2);
            int nReadDataCount = 6;

            int nSensorHistoryID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }

            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            int nEquipZoneID = -1;
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += nDataLength;
            }
            chunkSize -= 1;

            log.Param1 = nEquipZoneID.ToString();

            int nSensorID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }
            chunkSize -= 1;
            log.Param2 = nSensorID.ToString();

            nSOPGenUserID = -1;
            if (chunkSize > 0)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
                if (dataHeader == TCP_TYPE.INTEGER)
                {
                    nSOPGenUserID = BitConverter.ToInt32(bytes, nReadDataCount);
                    nReadDataCount += nDataLength;
                }
            }

            if (nSOPGenUserID != -1)
            {
                log.Param3 = nSOPGenUserID.ToString();
            }

            TimeHistory history = this.ServiceProvider.FindTimeHistory(log.SensorHistoryID);

            if (history != null)
            {
                log.Status = history.LastReactionLog.Status;
            }

            GetFireReportString(nEquipZoneID, log);
            log.Type = SensorReactionLog.ReactionType.NOTIFY_FIRE;
            return log;
        }

        public static string GetSpillReportString(int nEquipZoneID, int nSensorID, SensorReactionLog log)
        {
            string strMessage = "";

            if (nEquipZoneID == -1)
            {
                strMessage = "유해화학물질 누출이 신고되었습니다";
            }
            else
            {
                bool bMakeMessage = false;
                SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
                if( sensorZone != null)
                {
                    PSMSensor sensor = PSMManager.Instance.GetSensor(sensorZone.LinkedSensorID);
                    if (sensor != null)
                    {
                        PSMMaterial material = sensor.GetLinkedMaterial();
                        if (material != null)
                        {
                            string strMaterialName = material == null ? "유해화학물질" : material.Name;

                            // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                            // 수동신고
                            if (log != null && log.Param2 == "0")
                            {
                                Zone zone = ZoneManager.Instance.GetZone(nEquipZoneID);
                                if (zone != null)
                                {
                                    string szLocationName = zone.DisplayText;
                                    strMessage = string.Format("[{0}]에서 유해화학물질 누출이 신고되었습니다", szLocationName);
                                    bMakeMessage = true;
                                }
                            }
                            else
                            {
                                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                                if (equipZone != null)
                                {
                                    string szLocationName = equipZone.DisplayText;
                                    strMessage = string.Format("[{0}]에서 {1} 누출이 신고되었습니다", szLocationName, strMaterialName);
                                    bMakeMessage = true;
                                }
                            }
                        }
                    }
                }
                

                // Sensor나 Material이 없는 경우
                if (bMakeMessage == false)
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                    if (equipZone != null)
                    {
                        string szLocationName = equipZone.DisplayText;
                        strMessage = string.Format("[{0}]에서 유해화학물질 누출이 신고되었습니다", szLocationName);
                    }                    
                }               

                if (log != null)
                    log.Param1 = nEquipZoneID.ToString();
            }

            if (log != null)
            {
                if (log.Status == SensorReactionLog.DetectionStatus.TEST)
                    log.Message = "[테스트]" + strMessage;
                else
                    log.Message = strMessage;
            }

            return strMessage;
        }

        public static string GetSecurityReportString(int nEquipZoneID, SensorReactionLog log, int nEventType)
        {
            string strMessage = "";
            string strServerType = "";
            string strSituation = "방범";

            if (nEventType >= 0)
            {
                strServerType = ClientDataAsinFireMonitor.GetSecurityEventTypeName(nEventType);
                strSituation = SOPServer.EventTypeString.GetEventTypeDetectString(nEventType);
            }

            if (nEquipZoneID == -1)
            {
                strMessage = strSituation + " 상황이 신고되었습니다";
            }
            else
            {                
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    strMessage = string.Format("[{0}]에서 {1} 상황이 신고되었습니다", szLocationName, strSituation);
                }                

                if (log != null)
                    log.Param1 = nEquipZoneID.ToString();
            }

            if (log != null)
            {
                if (log.Status == SensorReactionLog.DetectionStatus.TEST)
                    log.Message = "[테스트]" + strServerType + strMessage;
                else
                    log.Message = strServerType + strMessage;
            }
            return strMessage;
        }

        public static string GetFireReportString(int nEquipZoneID, SensorReactionLog log)
        {
            string strMessage = "";

            if (nEquipZoneID == -1)
            {
                strMessage = "화재발생이 신고되었습니다";
            }
            else
            {
                // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                if (log != null && log.Param2 == "0")
                {
                    Zone zone = ZoneManager.Instance.GetZone(nEquipZoneID);
                    if (zone != null)
                    {
                        string szLocationName = zone.DisplayText;
                        strMessage = string.Format("[{0}]에서 화재발생이 신고되었습니다", szLocationName);
                    }
                }
                else
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                    if (equipZone != null)
                    {
                        string szLocationName = equipZone.DisplayText;
                        strMessage = string.Format("[{0}]에서 화재발생이 신고되었습니다", szLocationName);
                    }
                }

                if (log != null)
                    log.Param1 = nEquipZoneID.ToString();
            }

            if (log != null)
            {
                if (log.Status == SensorReactionLog.DetectionStatus.TEST)
                    log.Message = "[테스트]" + strMessage;
                else
                    log.Message = strMessage;
            }

            return strMessage;
        }
    }
}
