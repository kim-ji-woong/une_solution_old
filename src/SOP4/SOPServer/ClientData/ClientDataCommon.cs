
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMSServer
{
    public class ClientDataCommon
    {
        public delegate SensorReactionLog CreateBeginEventReactionLog(int nHistoryID, int nSensorID, int nEventType);
        public delegate SensorReactionLog CreateIgnoreEventReactionLog(int nHistoryID, int nSensorID, int nEventType);

        private CreateBeginEventReactionLog getBeginReactionLog = null;
        public CreateBeginEventReactionLog CreateBeginReactionLog
        {
            set { getBeginReactionLog = value; }
        }

        private CreateIgnoreEventReactionLog getIgnoreReactionLog = null;
        public CreateIgnoreEventReactionLog CreateIgnoreReactionLog
        {
            set { getIgnoreReactionLog = value; }
        }

        private object m_LockObj = new object();

        private ServiceProvider m_provider;
        // SDMS.TCP_CLIENT
        private byte m_clientType;
        private ClientData m_Owner = null;
        private bool m_bUseBroadcast = false;
        public bool OnBroadcast
        {
            get { return m_bUseBroadcast; }
            set { m_bUseBroadcast = value; }
        }

        // clientType : SDMS.TCP_CLIENT
        public ClientDataCommon(ServiceProvider provider, ClientData owner, byte clientType)
        {
            m_provider = provider;
            m_clientType = clientType;
            m_Owner = owner;
        }

        // 다른 ClientData에서 수신된 Data를 이용하여 SVMS이벤트를 처리하는 경우에 사용함
        // 주의점 : 내부에서 Lock을 사용하므로 동일 Lock루틴중에 사용하면 Deadlock이 발생함
        public bool ProcessSensorData(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (getBeginReactionLog == null || getIgnoreReactionLog == null)
                throw new NotImplementedException("콜백함수를 먼저 등록하세요");

            return OnReceive(state, bytes, nHeader, arrDatas);
        }

        private int m_nCurrentHeader = -1;

        public int CurrentHeader
        {
            get { return m_nCurrentHeader; }
            set { m_nCurrentHeader = value; }
        }

        private int nSensorTagID = -1;
        // bytes는 length byte가 제거되었음0
        protected bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            m_Owner.PingCount = 0;
            if (nHeader == TCP_ID.SENSOR_DATA || nHeader == TCP_ID.TEST_SENSOR_DATA || nHeader == TCP_ID.SENSOR_DATA_WITH_TAG)
            { 
                m_nCurrentHeader = nHeader;              

                // 센서 데이터에 대해 반드시 한번에 한개만 전송해야 하므로 lock 이 필요함
                // skkim 2014-01-09
                DdMonitor.Enter(m_LockObj, true);
                {
                    int nSensorID, data, nPrevSensorHistoryID = -1;
                    bool connected = false;
                    IFacility.FacilityType sensorType;

                    if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
                    {
                        int nSensorType = (int)arrDatas[0];
                        int nSensorTagID = (int)arrDatas[1];
                        int nSensorZoneID = (int)arrDatas[2];
                        int nSensorData = (int)arrDatas[3];
                        /*int nSensorType = BitConverter.ToInt32(bytes, 11);
                        int nSensorZoneID = BitConverter.ToInt32(bytes, 20);
                        int nSensorData = BitConverter.ToInt32(bytes, 29);
                        int nSensorTagID = -1;*/
                        if (nHeader == TCP_ID.SENSOR_DATA_WITH_TAG)
                        {
                            if (arrDatas.Count >= 5 && arrDatas[4] is int)
                            {
                                int nSensorTagHistoryID = (int)arrDatas[4];
                            }
                            //nSensorTagID = BitConverter.ToInt32(bytes, 38);
                        }

                        sensorType = IFacility.ToFacilityType(nSensorType);

                        // DB에 SensorZoneHistory를 등록하고 ID를 돌려준다.
                        int nHistoryID = NetworkServer.Instance.SensorManager.ProcessSensorData(arrDatas, out nSensorID, out data, out connected, ref nPrevSensorHistoryID, out sensorType);

                        // 등록된 SensorZoneHistoryID를 이용하여 ReactionLog를 생성하여 DB에 저장 후 클라어인트에 전송한다.
                        PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, nSensorID, data, (int)sensorType, connected);
                    }
                }
                DdMonitor.Exit(m_LockObj, true);

                m_nCurrentHeader = -1;
            }
            else if (nHeader == TCP_ID.RECIVER_CONNECT)
            {
                DdMonitor.Enter(m_LockObj, true);
                ProcessReciverConnect(bytes);
                DdMonitor.Exit(m_LockObj, true);
            }
            else if (nHeader == TCP_ID.RECIVER_DISCONNECT)
            {
                DdMonitor.Enter(m_LockObj, true);
                ProcessReciverDisconnect(bytes);
                DdMonitor.Exit(m_LockObj, true);
            }
            else if (nHeader == TCP_ID.ALL_RECIVER_STATE)
            {
                DdMonitor.Enter(m_LockObj, true);
                ProcessAllReciverState(bytes);
                DdMonitor.Exit(m_LockObj, true);
            }           
            return true;
        }


        private void ProcessChangeEquipmentZone(ArrayList arDatas)
        {
            ArrayList arData =  arDatas.GetRange(1, arDatas.Count - 1);           
            for( int  i = 0 ; i < arData.Count; i += 2 )
            {
                int nEquipZoneID = (int)arData[i];
                string szChangeName = (string)arData[i+1];

                // Change EquipmentZone Name

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if( equipZone != null)
                {
                    equipZone.DisplayText = szChangeName;
                }                
            }
        }
        
        private int GetReciverID(byte[] bytes, int nIdx = 0)
        {
            int nData = BitConverter.ToInt32(bytes, nIdx);
            return nData;
        }

        private bool IsReciverConnected(byte[] bytes, int nIdx = 0)
        {
            int nData = BitConverter.ToInt32(bytes, nIdx);
            if (nData == 1)
                return true;

            return false;
        }

        private void ProcessAllReciverState(byte[] bytes)
        {
            int nReciverID = -1;
            bool bConnected = false;

            int nDataCount = BitConverter.ToInt32(bytes, 2);

            int nReadData = 11;
            for (int i = 0; i < nDataCount; i += 2)
            {
                nReciverID = GetReciverID(bytes, nReadData);
                nReadData += 9;

                bConnected = IsReciverConnected(bytes, nReadData);
                nReadData += 9;

                if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
                {
                    Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
                    if (reciver.State != (bConnected == true ? 1 : 0))
                    {
                        ReciverManager.Instance.UpdateState(reciver.ID, bConnected, false);
                    }
                }
            }
            
            byte nClientType = TCP_CLIENT.SDMS_CLIENT_SECOND;
            m_provider.SendClientData(bytes, nClientType, true);
        }

        private void SendReciverState(int nReciverID, bool bConnected)
        {
            Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
            if (reciver == null)
                return;
            if (reciver.State == 1 && bConnected == true)
                return;
            if (reciver.State == 0 && bConnected == false)
                return;
            ReciverManager.Instance.UpdateState(nReciverID, bConnected, false);
        }

        private void ProcessReciverDisconnect(byte[] bytes)
        {
            m_Owner.PingCount = 0;
            int nReciverID = GetReciverID(bytes, 11);
            SendReciverState(nReciverID, false);
        }

        private void ProcessReciverConnect(byte[] bytes)
        {
            m_Owner.PingCount = 0;
            int nReciverID = GetReciverID(bytes, 11);

            SendReciverState(nReciverID, true);
        }

        static bool bClient = true;

        /// <summary>
        /// 등록된 SensorZoneHistoryID를 사용해 ReactionLog생성 및 사용자 전송, 문자 및 방송을 수행하는 함수
        /// </summary>
        /// <param name="nHistoryID">센서동작의 경우 PreocessSensorData에서 추가된HistoryID , 센서 클리어의 경우 -1</param>
        /// <param name="nPrevSensorHistoryID">센서 클리어의 경우 같은 센서의 이전 History ( 동작시 HistoryID )</param>
        /// <param name="nSensorID">해당 센서 ID</param>
        /// <param name="nData">동작의 경우 1, 클리어의 경우 0</param>
        /// <param name="bConnected">접속정보</param>
        private void PostProcessSensorData(int nHistoryID, int nPrevSensorHistoryID, int nSensorID, int nData, int nEventType, bool bConnected)
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
                m_provider.SendSensorZoneData(nData, nSensorID, TCP_CLIENT.SDMS_CLIENT);
            }

            // 데이터와 HistoryID가 있는경우만 처리
            if (nData == 1 && nHistoryID != -1)
            {
                if (!m_provider.CheckSituation(nHistoryID))
                {
                    SensorReactionLog.DetectionStatus type = SensorReactionLog.DetectionStatus.REAL;
                    if (m_nCurrentHeader == TCP_ID.SENSOR_DATA || m_nCurrentHeader == TCP_ID.SENSOR_DATA_WITH_TAG)
                    {
                        type = SensorReactionLog.DetectionStatus.REAL;
                    }
                    else
                    {
                        type  = SensorReactionLog.DetectionStatus.TEST;
                    }

                    TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now, type);
                    m_provider.AddTimeHistory(hs);
                    m_Owner.PingCount = 0;
                    m_provider.SendSensorZoneData(nData, nSensorID, TCP_CLIENT.SDMS_CLIENT);
                    m_Owner.PingCount = 0;

                    SensorReactionLog log = getBeginReactionLog(nHistoryID, nSensorID, nEventType);
                    log.Param4 = nSensorTagID.ToString();

                    m_provider.AddReactionLog(log);

                    // 사내방송 실시 - 신호에 대해 현장확인 후 방송 보내도록 함(삼천포:김명수대리요청)
                    // 2013-12-18
                    if (m_bUseBroadcast == true || log.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
                        ClientDataSDMS.RunBroadcast(log, m_provider, BroadcastManager.SituationType.DETECT_FIRE);

                    m_provider.SendSMS(log, ServiceProvider.GetSMSMessageTypeFromLog(log));

                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT_SECOND);

                    hs.LastReactionLog = log;
                    // Send History ID
                    //SendSensorHistoryID(nHistoryID);

                    m_provider.MonitorDetectFireProcess(log);
                }
            }
            // 데이터가 0이고 HistoryID가 -1인경우는 센서 클리어임
            else if (nData == 0 && nHistoryID != -1)
            {
                // 이전 신호가 있는경우만 처리함
                if (nPrevSensorHistoryID > 0)
                {
                    TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

                    // 이전 신호 History가 있는경우만 처리
                    if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
                    {
                        byte nClientType = (bClient == true ? TCP_CLIENT.SDMS_CLIENT : TCP_CLIENT.SDMS_CLIENT_SECOND);
                        bClient = !bClient;
                        Thread.Sleep(5);
                        m_Owner.PingCount = 0;

                        m_provider.SendSensorZoneData(nData, nSensorID, nClientType, true);

                        m_Owner.PingCount = 0;
                        // 화재 상황 종료
                        Thread.Sleep(5);
                        nClientType = (bClient == true ? TCP_CLIENT.SDMS_CLIENT : TCP_CLIENT.SDMS_CLIENT_SECOND);
                        bClient = !bClient;

                        m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType, true, true);
                        m_provider.SendClearDetectReport(nPrevSensorHistoryID, TCP_CLIENT.SOP_SIMULATOR, false, true);


                        Thread.Sleep(5);

                        // 알람 종료 처리 - history 삭제
                        m_provider.RemoveTimeHistory(history);
                        // 상황 종료 처리, HistoryID에 대한 처리 종료
                        m_provider.RemoveSituation(nHistoryID, false);

                        // 이전 HistoryID와 현재 ID에 대해 종료 처리
                        SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
                        SensorManager.Instance.RemoveSensorHistory(nHistoryID);

                        if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS ||
                            history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_S1ACCESS_STATUS ||
                            history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_S1SVMS_STATUS ||
                            history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE ||
                            history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_SECURITY
                            )
                        {
                            bool notifyFire = history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE;
                            bool notifySecurity = history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_SECURITY;

                            SensorReactionLog log = getIgnoreReactionLog(nPrevSensorHistoryID, nSensorID, nEventType);
                            m_provider.AddReactionLog(log);

                            // 자동 복구시 문자 전송
                            //if (notifyFire)
                            //    m_provider.SendSMSToAllCompanyMember(log);
                            //else
                            // 자동 복구시 문자 전송
                            if (notifyFire)
                            {
                                if (DataManager.Instance.UseReportFacilityManagers)
                                    m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_FIRE);
                                else
                                    m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_FIRE);
                            }
                            else if (notifySecurity)
                            {
                                if (DataManager.Instance.UseReportFacilityManagers)
                                    m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_SECURITY);
                                else
                                    m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_SECURITY);
                            }
                            else
                                m_provider.SendSMS(log, ServiceProvider.GetSMSMessageTypeFromLog(log));
                        }
                    }
                }
            }
        }

    }
}

