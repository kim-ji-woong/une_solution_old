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
    public class ClientDataS1AccessEventReciver : ClientData
    {

        private object m_LockObj = new object();
        private ClientDataCommon mCommonProcessor = null;

        public static string ServerTypeName
        {
            get { return "[Access]"; }
        }

        public ClientDataS1AccessEventReciver(ServiceProvider provider)
        {
            m_szServerType = ServerTypeName;
            m_provider = provider;
            ClientType = TCP_CLIENT.ACCESS_EVENT_RECIVER;
            mCommonProcessor = new ClientDataCommon(provider, this, ClientType);
            mCommonProcessor.CreateBeginReactionLog = this.CreateAccessEventLog;
            mCommonProcessor.CreateIgnoreReactionLog = this.CreateIgnoreAccessEvent;
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            ReciverManager.Instance.UpdateState(ReciverType.ACCESS_RECEIVER, true);
            return base.ProcessFirstConnection(state);
        }

        public override void CloseClient()
        {
            ReciverManager.Instance.UpdateState(ReciverType.ACCESS_RECEIVER, false);
            base.CloseClient();
        }

        // 다른 ClientData에서 수신된 Data를 이용하여 Access 이벤트를 처리하는 경우에 사용함
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
                ProcessServerCommand(bytes, arrDatas);
                return true;
            }
            return mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
        }

        private void ProcessServerCommand(byte[] bytes, ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount <= 0)
                return;

            int nCommand = (int)(byte)arrDatas[0];

            // int nCount , int id, string text,
            if (nCommand == (int)ServerCommandType.EQUIPMENTZONE_CHANGE_NAME)
            {
                try
                {
                    ProcessChangeEquipmentZone(arrDatas);

                    ServiceProvider.Instance.SendClientData(bytes, TCP_CLIENT.SDMS_CLIENT, true);
                }
                catch(Exception)
                {
                }          
            }
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

        private String GetEventTypeDetectString(int nEventType)
        {
            return SOPServer.EventTypeString.GetEventTypeDetectString(nEventType);
            /*string resultMsg = "";

            IFacility.FacilityType eventType = IFacility.ToFacilityType(nEventType);
            switch (eventType)
            {
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
            if (nEventType == FacilityEx.ToIntType(IFacility.FacilityType.FireF1_S1))
            {
                log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;
            }
            else
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
                log.Message = string.Format("{0}{1}{2} 탐지되었습니다", szTestMsg,ServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 {3} 탐지되었습니다", szTestMsg, ServerType, szLocationName, szTypeMsg);
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

            // Access에서 화재 Facility의 신호인경우 화재로 처리한다.
            // skkim 2017-03-17
            if (nEventType == FacilityEx.ToIntType(IFacility.FacilityType.FireF1_S1))
            {
                log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;
            }
            else
            {
                log.Type = SensorReactionLog.ReactionType.IGNORE_S1ACCESS_STATUS;
            }
           
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
                log.Message = string.Format("{0} {1}{2} 신호가 현장 복구되었습니다", szTestMsg, ServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 탐지된 {3} 신호가 현장 복구되었습니다", szTestMsg,ServerType, szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }        
    }
}

