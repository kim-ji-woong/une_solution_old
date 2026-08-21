using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDMS;
using System.Collections;
using TcpLib2;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMSServer
{
    public class ClientDataS1SecomServer : ClientData
    {
        private ClientDataCommon mCommonProcessor = null;

        public static string ServerTypeName
        {
            get { return "[Secom]"; }
        }

        public ClientDataS1SecomServer(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.S1_SECOM_EVENT_RECEIVER;
            m_szServerType = ServerTypeName;

            mCommonProcessor = new ClientDataCommon(provider, this, ClientType);
            mCommonProcessor.CreateBeginReactionLog = this.CreateSecomEventLog;
            mCommonProcessor.CreateIgnoreReactionLog = this.CreateIgnoreSecomEvent;
        }

        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            ReciverManager.Instance.UpdateState(ReciverType.SECOM_RECEIVER, true);
            return base.ProcessFirstConnection(state);
        }

        public override void CloseClient()
        {
            ReciverManager.Instance.UpdateState(ReciverType.SECOM_RECEIVER, false);
            base.CloseClient();
        }


        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;
            bool bResult = true;
            
            if (nHeader == TCP_ID.TEST_SENSOR_DATA || nHeader == TCP_ID.SENSOR_DATA)
            {
                int nSensorType = -1;

                if (arrDatas.Count > 0 && arrDatas[0] is int)
                    nSensorType = (int)arrDatas[0];

                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

                if (sensorType == IFacility.FacilityType.FIRE_SENSOR || sensorType == IFacility.FacilityType.SecomFire)
                {
                    bResult = mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
                else if (sensorType == IFacility.FacilityType.SecomExternalAlarmBell)
                {
                    bResult = mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
                else if (sensorType == IFacility.FacilityType.SecomWomenAlarmBell)
                {
                    bResult = mCommonProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
            }

            return bResult;
        }

        private SensorReactionLog CreateSecomEventLog(int nHistoryID, int nSensorID, int nEventType)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;

            if (nEventType == (int)IFacility.FacilityType.SecomFire || nEventType == (int)IFacility.FacilityType.FIRE_SENSOR)
            {
                log.Type = libSensorProcess.ReactionType.BEGIN_STATUS;
            }
            else
            {
                log.Type = libSensorProcess.ReactionType.BEGIN_SECOM_STATUS;
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

            string szTypeMsg = SOPServer.EventTypeString.GetEventTypeDetectString(nEventType);
            int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("{0}{1}{2} 탐지되었습니다", szTestMsg, m_szServerType, szTypeMsg);
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

        private SensorReactionLog CreateIgnoreSecomEvent(int nHistoryID, int nSensorID, int nEventType)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;

            if (nEventType == (int)IFacility.FacilityType.SecomFire || nEventType == (int)IFacility.FacilityType.FIRE_SENSOR)
            {
                log.Type = libSensorProcess.ReactionType.IGNORE_FIRE;
            }
            else
            {
                log.Type = libSensorProcess.ReactionType.IGNORE_SECOM_STATUS;
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
            string szTypeMsg = SOPServer.EventTypeString.GetEventTypeIgnoreString(nEventType);

            int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = string.Format("{0}{1}{2}신호가 현장 복구되었습니다", szTestMsg, m_szServerType, szTypeMsg);
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("{0}{1}[{2}]에서 탐지된{3}신호가 현장 복구되었습니다", szTestMsg, m_szServerType, szLocationName, szTypeMsg);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();
            log.Param3 = nEventType.ToString();

            return log;
        }
    }
}
