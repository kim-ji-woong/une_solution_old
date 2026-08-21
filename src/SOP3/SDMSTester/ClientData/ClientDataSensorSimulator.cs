using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDMS;
using System.Collections;
using System.Threading;

namespace SDMSServer
{
    public class ClientDataSensorSimulator : ClientData
    {
        public ClientDataSensorSimulator(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.SENSOR_SIMULATOR;
        }

        protected override bool OnReceive(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.SENSOR_DATA)
            {
                int nSensorID, data, nPrevSensorHistoryID = -1;
                bool connected = false;
                int nHistoryID = NetworkServer.Instance.SensorManager.ProcessSensorData(bytes, out nSensorID, out data, out connected, ref nPrevSensorHistoryID);
                PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, nSensorID, data, connected);
            }

            return true;
        }

        private void PostProcessSensorData(int nHistoryID, int nPrevSensorHistoryID, int nSensorID, int nData, bool bConnected)
        {
            // comment by skkim : AbnormalSensorManager에서 대행
            // 임시로 무시된 Sensor List에서 해제할 것이 있는지 검사
            if (nSensorID > 0 && nData == 0)
            {
                m_provider.RemoveTempIgnoreSensor(nSensorID);
            }

            // Connection만 변경되는 경우 리턴값이 -2임
            if (nHistoryID == -2)
            {
                m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
            }

            if (nData == 1 && nHistoryID != -1)
            {
                if (!m_provider.CheckSituation(nHistoryID))
                {
                    TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now);
                    m_provider.AddTimeHistory(hs);

                    m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);

                    SensorReactionLog log = CreateFireDetect(nHistoryID, nSensorID);
                    m_provider.AddReactionLog(log);

                    // 사내방송 실시
                    RunBroadcast(log);
                    m_provider.SendSMS(log);

                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);

                    hs.LastReactionLog = log;
                    // Send History ID
                    //SendSensorHistoryID(nHistoryID);

                    m_provider.MonitorDetectFireProcess(log);
                }
            }
            else if (nData == 0 && nHistoryID != -1)
            {
                //int nPrevSensorHistoryID = SensorManager.Instance.GetSensorHistoryID(nSensorID, true, 1);

                if (nPrevSensorHistoryID > 0)
                {
                    TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

                    if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
                    {
                        m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
						Thread.Sleep(10);
                        // 화재 상황 종료
                        m_provider.SendClearDetectReport(nPrevSensorHistoryID, ClientData.ClientType.SDMS_CLIENT);
						Thread.Sleep(10);
                        m_provider.RemoveTimeHistory(history);
                        m_provider.RemoveSituation(nHistoryID, false);

                        SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
                        SensorManager.Instance.RemoveSensorHistory(nHistoryID);

                        if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
                        {
                            SensorReactionLog log = new SensorReactionLog();

                            log.LogTime = DateTime.Now;
                            log.Message = "화재 신호가 무시되었습니다.";
                            log.SensorHistoryID = nPrevSensorHistoryID;
                            log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

                            m_provider.AddReactionLog(log);
                        }
                    }
                }
            }
        }

        private SensorReactionLog CreateFireDetect(int nHistoryID, int nSensorID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;
            int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = "화재가 탐지 되었습니다";
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szZoneName = equipZone.BroadcastName;
                    log.Message = string.Format("[{0}]에서 화재가 탐지 되었습니다", szZoneName);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();

            return log;
        }

        private void RunBroadcast(SensorReactionLog log)
        {
            // 화재 발생 방송
            string szBroadcastMsg = "";
            int nRepeat = 1;
            bool bUseSiren = false;
            bool bResult = GetBroadcastMessage(log, out szBroadcastMsg, out nRepeat, out bUseSiren);
            if (bResult)
            {
                if (BroadcastManager.Instance.IsEnabled(BroadcastManager.SituationType.DETECT_FIRE) == true)
                {
                    SensorReactionLog smsLog = new SensorReactionLog();
                    smsLog.Message = "사내 방송 실시";
                    smsLog.Param1 = log.Param1;
                    smsLog.Param2 = log.Param2;
                    smsLog.SensorHistoryID = log.SensorHistoryID;
                    smsLog.Type = SensorReactionLog.ReactionType.RUN_BROADCAST;
                    m_provider.AddReactionLog(smsLog);

                    BroadcastManager.Instance.AddSpeech(szBroadcastMsg, nRepeat, bUseSiren, BroadcastManager.SituationType.DETECT_FIRE);
                }
            }
        }

        private bool GetBroadcastMessage(SensorReactionLog log, out string szBroadcastMessage, out int nRepeat, out bool bSiren)
        {
            szBroadcastMessage = "";
            bSiren = false;
            nRepeat = 1;
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // 화재 탐지시 방송
            string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = 0";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                bool useBroadcast = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                bool useSiren = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nRepeatCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (useBroadcast == false)
                {
                    return false;
                }

                bSiren = useSiren;
                nRepeat = 1;

                int nEquipZoneID = -1;
                int.TryParse(log.Param1, out nEquipZoneID);

                string szOnce = "";
                int nIdx = strMessage.IndexOf("<<");
                int nIdx2 = strMessage.IndexOf(">>");
                if (nIdx != -1 && nIdx2 != -1)
                    szOnce = strMessage.Substring(nIdx + 2, (nIdx2 - nIdx) - 2);

                string szMsg = strMessage.Substring(nIdx2 + 2);
                string szMsg1 = "";
                if (nEquipZoneID != -1)
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

                    if (equipZone != null)
                        szMsg1 = szMsg.Replace("●", equipZone.BroadcastName);
                }
                szBroadcastMessage = szOnce + szMsg1;
                for (int j = 0; j < nRepeatCount; j++)
                {
                    szBroadcastMessage += "...";
                    szBroadcastMessage += szMsg1;
                }
            }
            return true;
        }
    }
}
