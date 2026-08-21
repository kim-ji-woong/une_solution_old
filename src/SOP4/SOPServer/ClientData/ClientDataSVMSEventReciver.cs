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
    public class ClientDataSVMSEventReciver : ClientData
    {

        private object m_LockObj = new object();
        private ClientDataCommon mCommonProcessor = null;

        public static string ServerTypeName
        {
            get { return "[SVMS]"; }
        }

        public ClientDataSVMSEventReciver(ServiceProvider provider)
        {
            m_szServerType = ServerTypeName;

            m_provider = provider;
            ClientType = TCP_CLIENT.SVMS_EVENT_RECIVER;
            mCommonProcessor = new ClientDataCommon(provider, this, ClientType);
            mCommonProcessor.CreateBeginReactionLog = this.CreateSVMSEventLog;
            mCommonProcessor.CreateIgnoreReactionLog = this.CreateIgnoreFireDetect;
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            ReciverManager.Instance.UpdateState(ReciverType.SVMS_RECEIVER, true);
            return base.ProcessFirstConnection(state);
        }

        public override void CloseClient()
        {
            ReciverManager.Instance.UpdateState(ReciverType.SVMS_RECEIVER, false);
            base.CloseClient();
        }

        // 다른 ClientData에서 수신된 Data를 이용하여 SVMS이벤트를 처리하는 경우에 사용함
        // 주의점 : 내부에서 Lock을 사용하므로 동일 Lock루틴중에 사용하면 Deadlock이 발생함
        public void ProcessSensorData(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            OnReceive(state, bytes, nHeader, arrDatas);
        }

        // bytes는 length byte가 제거되었음0
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;
            return mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
        }

        private String GetEventTypeDetectString(int nEventType)
        {
            return SOPServer.EventTypeString.GetEventTypeDetectString(nEventType);
            /*string resultMsg = "";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.Intrusion_S1:
                    resultMsg = "침입이";
                    break;
                case IFacility.FacilityType.Loiter_S1:
                    resultMsg = "배회가";
                    break;
                case IFacility.FacilityType.Slip_S1:
                    resultMsg = "넘어짐이";
                    break;
                case IFacility.FacilityType.Steal_S1:
                    resultMsg = "도난이";
                    break;
                case IFacility.FacilityType.Abandoned_S1:
                    resultMsg = "방치가";
                    break;
                case IFacility.FacilityType.VirtualFence_S1:
                    resultMsg = "(가상펜스)침입이";
                    break;
                case IFacility.FacilityType.Fire_S1:
                    resultMsg = "화재가";
                    break;
                case IFacility.FacilityType.EmergencyBell_S1:
                    resultMsg = "비상벨 신호가";
                    break;
                default:
                    break;    
            }
            return resultMsg;           */
        }

        private String GetEventTypeIgnoreString(int nEventType)
        {
            return SOPServer.EventTypeString.GetEventTypeIgnoreString(nEventType);
            /*string resultMsg = "";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.Intrusion_S1:
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.Loiter_S1:
                    resultMsg = "배회";
                    break;
                case IFacility.FacilityType.Slip_S1:
                    resultMsg = "넘어짐";
                    break;
                case IFacility.FacilityType.Steal_S1:
                    resultMsg = "도난";
                    break;
                case IFacility.FacilityType.Abandoned_S1:
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
                default:
                    break;
            }
            return resultMsg;*/
        }

        private SensorReactionLog CreateSVMSEventLog(int nHistoryID, int nSensorID, int nEventType)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;

            // SVMS 지능형 화재의 경우 일반화재로 처리한다.
            // skkim 2017-03-27
            if (nEventType == FacilityEx.ToIntType(IFacility.FacilityType.Fire_S1))
            {
                log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;
            }
            else
            {
                log.Type = SensorReactionLog.ReactionType.BEGIN_S1SVMS_STATUS;
            }

            string szTestMsg = "";
            if (mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA || mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA_WITH_TAG)
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
                    log.Message = string.Format("{0}{1}[{2}]에서 {3} 탐지되었습니다", szTestMsg, m_szServerType, szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }
            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }

        private SensorReactionLog CreateIgnoreFireDetect(int nHistoryID, int nSensorID, int nEventType)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;

            // SVMS 지능형 화재의 경우 일반화재로 처리한다.
            // skkim 2017-03-27
            if (nEventType == FacilityEx.ToIntType(IFacility.FacilityType.Fire_S1))
            {
                log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;
            }
            else
            {
                log.Type = SensorReactionLog.ReactionType.IGNORE_S1SVMS_STATUS;
            }
           
            string szTestMsg = "";
            if (mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA || mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA_WITH_TAG)
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
                log.Message = string.Format("{0}{1}{2}신호가 현장 복구되었습니다", szTestMsg,m_szServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 탐지된{3}신호가 현장 복구되었습니다", szTestMsg,m_szServerType, szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }        
    }
}

