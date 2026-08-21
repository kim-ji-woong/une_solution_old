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
    public class ClientDataAsinFireMonitor : ClientData
    {
        private object m_LockObj = new object();
        private ClientDataCommon mCommonProcessor = null;

        public static string ServerTypeName
        {
            get { return "[화재센서]"; }
        }

        public ClientDataAsinFireMonitor(ServiceProvider provider)
        {
            m_szServerType = ServerTypeName;
            m_provider = provider;
            ClientType = TCP_CLIENT.ASIN_EVENT_RECIVER;
            mCommonProcessor = new ClientDataCommon(provider, this, ClientType);
            mCommonProcessor.CreateBeginReactionLog = this.CreateAccessEventLog;
            mCommonProcessor.CreateIgnoreReactionLog = this.CreateIgnoreAccessEvent;
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            ReciverManager.Instance.UpdateState(ReciverType.ASIN_RECEIVER, true);
            return base.ProcessFirstConnection(state);
        }

        public override void CloseClient()
        {
            ReciverManager.Instance.UpdateState(ReciverType.ASIN_RECEIVER, false);
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
            /*string resultMsg = "화재";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                    resultMsg = "화재";
                    break;            
                default:
                    break;
            }
            return resultMsg;           */
        }

        private String GetEventTypeIgnoreString(int nEventType)
        {
            return SOPServer.EventTypeString.GetEventTypeIgnoreString(nEventType);
            /*string resultMsg = "화재";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                    resultMsg = "화재";
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
            

            // Access에서 화재 Facility의 신호인경우 화재로 처리한다.
            // skkim 2017-03-17
            //if (nEventType == IFacility.ToIntType(IFacility.FacilityType.FireF1_S1))
            {
                log.Type = GetTestServerAccessEventLogType(nEventType);
                //log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;
            }
            //else
            //    log.Type = SensorReactionLog.ReactionType.BEGIN_S1ACCESS_STATUS;

            string szTestMsg = "";
            string strServerType = m_szServerType;

            if (mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA)
            {
                log.Status = SensorReactionLog.DetectionStatus.REAL;
            }
            else
            {
                log.Status = SensorReactionLog.DetectionStatus.TEST;
                szTestMsg = "[테스트]";
                strServerType = GetSecurityEventTypeName(nEventType);
            }

            string szTypeMsg = GetEventTypeDetectString(nEventType);
            int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("{0}{1}{2} 신호가 탐지되었습니다", szTestMsg, strServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 {3} 신호가 탐지되었습니다", szTestMsg,strServerType, szLocationName, szTypeMsg);
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
            log.Type = GetTestServerIgnoreAccessLogType(nEventType);
            //log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;
                       
            string szTestMsg = "";
            string strServerType = m_szServerType;

            if (mCommonProcessor.CurrentHeader == TCP_ID.SENSOR_DATA)
            {
                log.Status = SensorReactionLog.DetectionStatus.REAL;
            }
            else
            {
                log.Status = SensorReactionLog.DetectionStatus.TEST;
                szTestMsg = "[테스트]";
                strServerType = GetSecurityEventTypeName(nEventType);
            }
            string szTypeMsg = GetEventTypeIgnoreString(nEventType);

            int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("{0}{1}{2} 신호가 현장 복구되었습니다", szTestMsg, strServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 탐지된 {3} 신호가 현장 복구되었습니다", szTestMsg, strServerType, szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }
        
        // Access, SVMS, EMPoll, Asin 4가지 Test Sensor 신호가 모두 ClientDataAsinFireMonitor로 오기 때문에
        // 테스트 신호를 구분할 방법이 필요하다.
        public static string GetSecurityEventTypeName(int nEventType)
        {
            string resultMsg = "";

            UnE.Sensor.IFacility.FacilityType eventType = UnE.Sensor.IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
                case UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_TypeA:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_TypeB:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_GasEmission:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_ManualControl:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_LightType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_SiemensType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_Monitoring:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_SensingLine:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_MonitoringType:
                    resultMsg = ClientDataAsinFireMonitor.ServerTypeName;
                    break;
                case UnE.Sensor.IFacility.FacilityType.Intrusion_S1:
                case UnE.Sensor.IFacility.FacilityType.Loiter_S1:
                case UnE.Sensor.IFacility.FacilityType.Collapse_S1:
                case UnE.Sensor.IFacility.FacilityType.Theft_S1:
                case UnE.Sensor.IFacility.FacilityType.Neglect_S1:
                case UnE.Sensor.IFacility.FacilityType.VirtualFence_S1:
                case UnE.Sensor.IFacility.FacilityType.Fire_S1:
                case UnE.Sensor.IFacility.FacilityType.EmergencyBell_S1:
                    resultMsg = ClientDataSVMSEventReciver.ServerTypeName;
                    break;
                case UnE.Sensor.IFacility.FacilityType.GeneralIntrusionT1_S1:
                case UnE.Sensor.IFacility.FacilityType.GeneralIntrusionT2_S1:
                case UnE.Sensor.IFacility.FacilityType.InternalIntrusionT3_S1:
                case UnE.Sensor.IFacility.FacilityType.VaultIntrusionT4_S1:
                case UnE.Sensor.IFacility.FacilityType.FireF1_S1:
                case UnE.Sensor.IFacility.FacilityType.CustomerEmergencyC1_S1:
                case UnE.Sensor.IFacility.FacilityType.CustomerEmergencyC2_S1:
                case UnE.Sensor.IFacility.FacilityType.RescueQQ_S1:
                case UnE.Sensor.IFacility.FacilityType.GasG1_S1:
                case UnE.Sensor.IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                case UnE.Sensor.IFacility.FacilityType.LeakAbnormalityU4_S1:
                case UnE.Sensor.IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    resultMsg = ClientDataS1AccessEventReciver.ServerTypeName;
                    break;
                case UnE.Sensor.IFacility.FacilityType.ExternalAlarmBell:
                    resultMsg = ClientDataEMPollEventReciver.ServerTypeName;
                    break;
                case UnE.Sensor.IFacility.FacilityType.SecomFire:
                case UnE.Sensor.IFacility.FacilityType.SecomExternalAlarmBell:
                case UnE.Sensor.IFacility.FacilityType.SecomWomenAlarmBell:
                    resultMsg = ClientDataS1SecomServer.ServerTypeName;
                    break;
                default:
                    break;
            }
            return resultMsg;
        }

        private libSensorProcess.ReactionType GetTestServerIgnoreAccessLogType(int nEventType)
        {
            UnE.Sensor.IFacility.FacilityType eventType = UnE.Sensor.IFacility.ToFacilityType(nEventType);

            switch (eventType)
            {
                case UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_TypeA:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_TypeB:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_GasEmission:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_ManualControl:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_LightType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_SiemensType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_Monitoring:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_SensingLine:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_MonitoringType:
                    return libSensorProcess.ReactionType.IGNORE_FIRE;

                case UnE.Sensor.IFacility.FacilityType.Fire_S1:
                    return libSensorProcess.ReactionType.IGNORE_FIRE;
                case UnE.Sensor.IFacility.FacilityType.Intrusion_S1:
                case UnE.Sensor.IFacility.FacilityType.Loiter_S1:
                case UnE.Sensor.IFacility.FacilityType.Collapse_S1:
                case UnE.Sensor.IFacility.FacilityType.Theft_S1:
                case UnE.Sensor.IFacility.FacilityType.Neglect_S1:
                case UnE.Sensor.IFacility.FacilityType.VirtualFence_S1:
                case UnE.Sensor.IFacility.FacilityType.EmergencyBell_S1:
                    return libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS;

                case UnE.Sensor.IFacility.FacilityType.FireF1_S1:
                    return libSensorProcess.ReactionType.IGNORE_FIRE;
                case UnE.Sensor.IFacility.FacilityType.GeneralIntrusionT1_S1:
                case UnE.Sensor.IFacility.FacilityType.GeneralIntrusionT2_S1:
                case UnE.Sensor.IFacility.FacilityType.InternalIntrusionT3_S1:
                case UnE.Sensor.IFacility.FacilityType.VaultIntrusionT4_S1:
                case UnE.Sensor.IFacility.FacilityType.CustomerEmergencyC1_S1:
                case UnE.Sensor.IFacility.FacilityType.CustomerEmergencyC2_S1:
                case UnE.Sensor.IFacility.FacilityType.RescueQQ_S1:
                case UnE.Sensor.IFacility.FacilityType.GasG1_S1:
                case UnE.Sensor.IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                case UnE.Sensor.IFacility.FacilityType.LeakAbnormalityU4_S1:
                case UnE.Sensor.IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    return libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS;

                case UnE.Sensor.IFacility.FacilityType.ExternalAlarmBell:
                    // 비상벨은 Access신호와 통합해서 보낸다.
                    return libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS;

                case UnE.Sensor.IFacility.FacilityType.SecomFire:
                    return libSensorProcess.ReactionType.IGNORE_FIRE;
                case UnE.Sensor.IFacility.FacilityType.SecomExternalAlarmBell:
                case UnE.Sensor.IFacility.FacilityType.SecomWomenAlarmBell:
                    return libSensorProcess.ReactionType.IGNORE_SECOM_STATUS;
            }

            return libSensorProcess.ReactionType.IGNORE_FIRE;
        }

        private libSensorProcess.ReactionType GetTestServerAccessEventLogType(int nEventType)
        {
            UnE.Sensor.IFacility.FacilityType eventType = UnE.Sensor.IFacility.ToFacilityType(nEventType);

            switch (eventType)
            {
                case UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_TypeA:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_TypeB:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_GasEmission:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_ManualControl:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_LightType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_SiemensType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_Monitoring:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_SensingLine:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case UnE.Sensor.IFacility.FacilityType.FireSensor_MonitoringType:
                    return libSensorProcess.ReactionType.BEGIN_STATUS;

                case UnE.Sensor.IFacility.FacilityType.Fire_S1:
                    return libSensorProcess.ReactionType.BEGIN_STATUS;
                case UnE.Sensor.IFacility.FacilityType.Intrusion_S1:
                case UnE.Sensor.IFacility.FacilityType.Loiter_S1:
                case UnE.Sensor.IFacility.FacilityType.Collapse_S1:
                case UnE.Sensor.IFacility.FacilityType.Theft_S1:
                case UnE.Sensor.IFacility.FacilityType.Neglect_S1:
                case UnE.Sensor.IFacility.FacilityType.VirtualFence_S1:
                case UnE.Sensor.IFacility.FacilityType.EmergencyBell_S1:
                    return libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS;

                case UnE.Sensor.IFacility.FacilityType.FireF1_S1:
                    return libSensorProcess.ReactionType.BEGIN_STATUS;
                case UnE.Sensor.IFacility.FacilityType.GeneralIntrusionT1_S1:
                case UnE.Sensor.IFacility.FacilityType.GeneralIntrusionT2_S1:
                case UnE.Sensor.IFacility.FacilityType.InternalIntrusionT3_S1:
                case UnE.Sensor.IFacility.FacilityType.VaultIntrusionT4_S1:
                case UnE.Sensor.IFacility.FacilityType.CustomerEmergencyC1_S1:
                case UnE.Sensor.IFacility.FacilityType.CustomerEmergencyC2_S1:
                case UnE.Sensor.IFacility.FacilityType.RescueQQ_S1:
                case UnE.Sensor.IFacility.FacilityType.GasG1_S1:
                case UnE.Sensor.IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                case UnE.Sensor.IFacility.FacilityType.LeakAbnormalityU4_S1:
                case UnE.Sensor.IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    return libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS;

                case UnE.Sensor.IFacility.FacilityType.ExternalAlarmBell:
                    // 비상벨은 Access신호와 통합해서 보낸다.
                    return libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS;

                case UnE.Sensor.IFacility.FacilityType.SecomFire:
                    return libSensorProcess.ReactionType.BEGIN_STATUS;
                case UnE.Sensor.IFacility.FacilityType.SecomExternalAlarmBell:
                case UnE.Sensor.IFacility.FacilityType.SecomWomenAlarmBell:
                    return libSensorProcess.ReactionType.BEGIN_SECOM_STATUS;
            }

            return libSensorProcess.ReactionType.BEGIN_STATUS;
        }
    }
}

