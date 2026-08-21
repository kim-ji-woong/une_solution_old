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
    public class ClientDataSOPMonitor : ClientData
    {

        private ClientDataPSMSensor mPsmClient = null;
        public ClientDataSOPMonitor(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.SENSOR_MONITOR2;

            ReciverManager.Instance.GetReciverList();
			
            mPsmClient = new ClientDataPSMSensor(provider);
        }


        private object m_LockObj = new object();
        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
			PingCount = 0;
			if (nHeader == TCP_ID.SENSOR_DATA)
			{
				// 센서 데이터에 대해 반드시 한번에 한개만 전송해야 하므로 lock 이 필요함
				// skkim 2014-01-09
                DdMonitor.Enter(m_LockObj, true);
				{
					int nSensorID, data, nPrevSensorHistoryID = -1;
					bool connected = false;
                    IFacility.FacilityType sensorType;
                    int nHistoryID = NetworkServer.Instance.SensorManager.ProcessSensorData(arrDatas, out nSensorID, out data, out connected, ref nPrevSensorHistoryID, out sensorType);

                    if (sensorType == IFacility.FacilityType.PSM_SENSOR)
                    {
                        int nOriginSensorZoneID = BitConverter.ToInt32(bytes, 20);
                        SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
                        mPsmClient.PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, sensorZone, nOriginSensorZoneID, new DBUtility.VariousData<int>(data));
                    }
                    else
                        PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, nSensorID, data, connected);
				}
                DdMonitor.Exit(m_LockObj, true);
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

            if (nHeader == TCP_ID.PSM_SENSOR_DATA)
            {
                // 센서 데이터에 대해 반드시 한번에 한개만 전송해야 하므로 lock 이 필요함
                // skkim 2014-01-09
                DdMonitor.Enter(m_LockObj, true);
                {
                    mPsmClient.ProcessPSMSensorData(state, bytes, nHeader, arrDatas);
                }
                DdMonitor.Exit(m_LockObj, true);
            }

			return true;
		}

        private int GetReciverID(byte[] bytes, int nIdx = 0)
        {
			int nData = BitConverter.ToInt32(bytes, nIdx);
			return nData;
        }

        private bool IsReciveredPoll(byte[] bytes, int nIdx = 0)
        {
            int nData = BitConverter.ToInt32(bytes, nIdx);
            if (nData > 10)
                return true;

            return false;
        }

		private bool IsReciverConnected(byte[] bytes, int nIdx = 0)
		{
			int nData = BitConverter.ToInt32(bytes, nIdx);
			if (nData > 0)
				return true;
			
			return false;
		}


        private bool m_bFirst = true;
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
                bool bRecivePoll = IsReciveredPoll(bytes, nReadData);
                if (bConnected == false)
                    bRecivePoll = false;
				nReadData += 9;

				if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
				{
					Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
                    if (m_bFirst == true)
                    {
                        m_bFirst = false;
                        ReciverManager.Instance.UpdateState(reciver.ID, bConnected, bRecivePoll);
                    }
                    else
                    {
                        if (reciver.State == 0)
                        {
                            if (bConnected == true || bRecivePoll == true)
                            {
                                ReciverManager.Instance.UpdateState(reciver.ID, bConnected, bRecivePoll);
                            }


                        } else if (reciver.State == 1)
                        {
                            if (bConnected == false || bRecivePoll == true)
                            {
                                ReciverManager.Instance.UpdateState(reciver.ID, bConnected, bRecivePoll);
                            }
                        } else if (reciver.State == 11)
                        {
                            if (bConnected == false || bRecivePoll == false)
                            {
                                ReciverManager.Instance.UpdateState(reciver.ID, bConnected, bRecivePoll);
                            }
                        }
                    }
				}
			}

            byte nClientType = TCP_CLIENT.SDMS_CLIENT_SECOND;
            
            m_provider.SendClientData(bytes, nClientType, true);          
        }


        private void SendReciverState(int nReciverID , bool bConnected)
		{
            Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
            if (reciver == null)
                return;
            if (reciver.State == 1 && bConnected == true)
                return;

            if (reciver.State == 0 && bConnected == false)
                return;

            
            bool bRecivedPoll = reciver.RecivedPoll == 1 ? true : false;

            if (bConnected == false)
                bRecivedPoll = false;

            ReciverManager.Instance.UpdateState(nReciverID, bConnected, bRecivedPoll);

			//TCP_CLIENT nClientType = (bClient == true ? TCP_CLIENT.SDMS_CLIENT : TCP_CLIENT.SDMS_CLIENT_SECOND);
            //bClient = !bClient;

            //m_provider.SendReciverState(nReciverID, bConnected, nClientType);
		}

        private void ProcessReciverDisconnect(byte[] bytes)
        {
			this.PingCount = 0;
			int nReciverID = GetReciverID(bytes, 11);

			SendReciverState(nReciverID, false);
        }

        private void ProcessReciverConnect(byte[] bytes)
        {
			this.PingCount = 0;
			int nReciverID = GetReciverID(bytes, 11);

			SendReciverState(nReciverID, true);
        }
		static bool bClient = true;
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
				m_provider.SendSensorZoneData(nData, nSensorID, TCP_CLIENT.SDMS_CLIENT);
			}

			if (nData == 1 && nHistoryID != -1)
			{
				if (!m_provider.CheckSituation(nHistoryID))
				{
                    TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now, SensorReactionLog.DetectionStatus.REAL);
					m_provider.AddTimeHistory(hs);
					PingCount = 0;
					m_provider.SendSensorZoneData(nData, nSensorID, TCP_CLIENT.SDMS_CLIENT);
					PingCount = 0;

					SensorReactionLog log = CreateFireDetect(nHistoryID, nSensorID);

					m_provider.AddReactionLog(log);

					// 사내방송 실시 - 신호에 대해 현장확인 후 방송 보내도록 함(삼천포:김명수대리요청)
                    // 2013-12-18
					ClientDataSDMS.RunBroadcast(log, m_provider, BroadcastManager.SituationType.DETECT_FIRE);
					m_provider.SendSMS(log, SMSManager.SMSMessageType.DETECT_FIRE);

					// Send Reaction Log
					m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT_SECOND);

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
						byte nClientType = (bClient == true ? TCP_CLIENT.SDMS_CLIENT : TCP_CLIENT.SDMS_CLIENT_SECOND);
						bClient = !bClient;
						Thread.Sleep(5);
						PingCount = 0;
						
						m_provider.SendSensorZoneData(nData, nSensorID, nClientType, true);
						
						PingCount = 0;
						// 화재 상황 종료
						Thread.Sleep(5);
						nClientType = (bClient == true ? TCP_CLIENT.SDMS_CLIENT : TCP_CLIENT.SDMS_CLIENT_SECOND);
						bClient = !bClient;
						
						m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType,true, true);
                        m_provider.SendClearDetectReport(nPrevSensorHistoryID, TCP_CLIENT.SOP_SIMULATOR, false, true);
						

						Thread.Sleep(5);
						m_provider.RemoveTimeHistory(history);
						m_provider.RemoveSituation(nHistoryID, false);

						SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
						SensorManager.Instance.RemoveSensorHistory(nHistoryID);

						if (history.LastReactionLog.Type == libSensorProcess.ReactionType.BEGIN_STATUS ||
                            history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_FIRE)
						{
                            bool notifyFire = history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_FIRE;
                            SensorReactionLog log = CreateIgnoreFireDetect(nPrevSensorHistoryID, nSensorID);
							
                            //SensorReactionLog log = new SensorReactionLog();
							//log.LogTime = DateTime.Now;
							//log.Message = "화재 신호가 자동복구 되었습니다..";
							//log.SensorHistoryID = nPrevSensorHistoryID;
							//log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

							m_provider.AddReactionLog(log);

                            // 자동 복구시 문자 전송
                            if (notifyFire)
                            {
                                if (DataManager.Instance.UseReportFacilityManagers)
                                    m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_FIRE);
                                else
                                    m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_FIRE);
                            }
                            else
                                m_provider.SendSMS(log, ServiceProvider.GetSMSMessageTypeFromLog(log));
						}
					}
				}
			}
		}

        private SensorReactionLog CreateIgnoreFireDetect(int nHistoryID, int nSensorID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.IGNORE_FIRE;
            log.Status = SensorReactionLog.DetectionStatus.REAL;
            int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = "탐지된 화재신호가 현장 복구되었습니다";
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("[{0}]에서 탐지된 화재신호가 현장 복구되었습니다", szLocationName);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();

            return log;
        }	

		private SensorReactionLog CreateFireDetect(int nHistoryID, int nSensorID)
		{
			SensorReactionLog log = new SensorReactionLog();

			log.LogTime = DateTime.Now;
			log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.BEGIN_STATUS;
            log.Status = SensorReactionLog.DetectionStatus.REAL;
			int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
			if (nEquipZoneID == -1)
			{
				log.Message = "화재가 탐지되었습니다";
			}
			else
			{
				EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
				if (equipZone != null)
				{
                    // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                    string szLocationName = equipZone.DisplayText;
					log.Message = string.Format("[{0}]에서 화재가 탐지되었습니다", szLocationName);
				}
				log.Param1 = nEquipZoneID.ToString();
			}

			log.Param2 = nSensorID.ToString();

			return log;
		}	
	}
}

