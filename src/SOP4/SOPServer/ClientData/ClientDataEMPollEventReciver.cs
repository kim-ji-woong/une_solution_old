﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;
using UnE.Spatial;

namespace SDMSServer
{
    public class ClientDataEMPollEventReciver : ClientData
    {

        private object m_LockObj = new object();
        private ClientDataCommon mCommonProcessor = null;

        public static string ServerTypeName
        {
            get { return "[EMPOLL]"; }
        }

        public ClientDataEMPollEventReciver(ServiceProvider provider)
        {
            m_szServerType = ServerTypeName;
            m_provider = provider;
            ClientType = TCP_CLIENT.SAINTOP_EVENT_RECIVER;
            mCommonProcessor = new ClientDataCommon(provider, this, ClientType);
            mCommonProcessor.CreateBeginReactionLog = this.CreateAccessEventLog;
            mCommonProcessor.CreateIgnoreReactionLog = this.CreateIgnoreAccessEvent;

            // Update Reciver State
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            ReciverManager.Instance.UpdateState(ReciverType.EMPOLL_RECEIVER, true);
            return base.ProcessFirstConnection(state);
        }

        public override void CloseClient()
        {
            ReciverManager.Instance.UpdateState(ReciverType.EMPOLL_RECEIVER, false);
            base.CloseClient();
        }

        // 다른 ClientData에서 수신된 Data를 이용하여 SVMS이벤트를 처리하는 경우에 사용함
        // 주의점 : 내부에서 Lock을 사용하므로 동일 Lock루틴중에 사용하면 Deadlock이 발생함
        public bool ProcessSensorData(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            return OnReceive(state, bytes, nHeader, arrDatas);
        }

        // bytes는 length byte가 제거되었음0
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;
            if(nHeader == TCP_ID.SERVER_COMMAND)
            {
                //ProcessServerCommand(bytes, arrDatas);
                //return true;
            }
            return mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
        }
             
        private String GetEventTypeDetectString(int nEventType)
        {
            return SOPServer.EventTypeString.GetEventTypeDetectString(nEventType);
            /*string resultMsg = "";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.ExternalAlarmBell:
                    resultMsg = "비상벨 호출";
                    break;            
                default:
                    break;
            }
            return resultMsg;     */      
        }

        private String GetEventTypeIgnoreString(int nEventType)
        {
            return SOPServer.EventTypeString.GetEventTypeIgnoreString(nEventType);
            /*string resultMsg = "";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.ExternalAlarmBell:
                    resultMsg = "비상벨 호출";
                    break;    
                default:
                    break;
            }
            return resultMsg;  */
        }

        private SensorReactionLog CreateAccessEventLog(int nHistoryID, int nSensorID, int nEventType)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = SensorReactionLog.ReactionType.BEGIN_S1ACCESS_STATUS;

            string szTestMsg = "";
            if (mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA)
            {
                log.Status = SensorReactionLog.DetectionStatus.REAL;
            }
            else
            {
                log.Status = SensorReactionLog.DetectionStatus.TEST;
                szTestMsg = "[테스트]";               
            }

            string szTypeMsg = GetEventTypeDetectString(nEventType);
            int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("{0}{1}{2} 탐지되었습니다", szTestMsg,m_szServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 {3} 탐지되었습니다", szTestMsg, m_szServerType,szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }
            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }

        private SensorReactionLog CreateIgnoreAccessEvent(int nHistoryID, int nSensorID, int nEventType)
        {
            SensorReactionLog log = new SensorReactionLog();
            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            // 비상벨은 Access신호와 통합해서 보낸다.
            log.Type = SensorReactionLog.ReactionType.IGNORE_S1ACCESS_STATUS;
           
            string szTestMsg = "";
            if (mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA)
            {
                log.Status = SensorReactionLog.DetectionStatus.REAL;
            }
            else
            {
                log.Status = SensorReactionLog.DetectionStatus.TEST;
                szTestMsg = "[테스트]";
            }
            string szTypeMsg = GetEventTypeIgnoreString(nEventType);

            int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("{0}{1}{2} 신호가 현장 복구되었습니다", szTestMsg,m_szServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 탐지된 {3} 신호가 현장 복구되었습니다", szTestMsg, m_szServerType, szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }        
    }
}

