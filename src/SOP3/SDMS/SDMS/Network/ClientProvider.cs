using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using SOP;

namespace SDMS
{
    public class ClientProvider : ClientServiceProvider
    {
		private int m_nProviderNum = 0;
        public int ProviderNum
        {
            get { return m_nProviderNum; }
            set { m_nProviderNum = value; }
        }
 
		
        private int m_nProviderType = 1;
        public int ProviderType
        {
            get { return m_nProviderType; }
            set { m_nProviderType = value; }
        }

        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        //private byte[] m_arrTemp = null;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProvider(NetworkManager mgr, int nID)
        {
			m_nProviderNum = nID;
			if (nID == 2)
				m_nProviderType = (int)TCP_CLIENT.SDMS_CLIENT_SECOND;
			else
				m_nProviderType = (int)TCP_CLIENT.SDMS_CLIENT;
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
			this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

		private void ProcessReactionHistoryLogList(byte[] bytes)
        {
            ArrayList arrReactionLog = new ArrayList();

			int nChunkCount = (int)BitConverter.ToInt32(bytes, 2);

            int nLogChunkSize = 10;
            int nLogCount = (nChunkCount) / nLogChunkSize;

            if (nLogCount > 0)
            {
				int nOffset = 6;

                for (int i = 0; i < nLogCount; i++)
                {
                    ReactionLog log = ReadReactionHistoryLog(bytes, ref nOffset, nLogChunkSize);
					if( log != null)
						arrReactionLog.Add(log);
                }
            }

            ArrayList arrRemoveProcess = new ArrayList();
            ArrayList arrCurrentLog = new ArrayList();

            foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
            {
                ReactionLog log = FindLog(arrReactionLog, pair.Value.SensorHistoryID);

                // 현재 진행중인 화재 가운데 arrReactionLog에 포함되지 않은것은 이미 종료된 화재이다.
                if (log == null)
                {
                    EndProcess(pair.Value);
                    //RemoveProcess(pair.Value);
                    arrRemoveProcess.Add(pair.Value);
                }
                else
                {
                    arrCurrentLog.Add(log);
                    arrReactionLog.Remove(log);
                }
            }

            foreach (ProcessIF process in arrRemoveProcess)
            {
                RemoveProcess(process);
            }

			bool bRunSimulator = false;

            // 새로운 화재 상황에 대한 Log
            foreach (ReactionLog log in arrReactionLog)
            {
				if (log.ReactionType == (int)ReactionType.BEGIN_STATUS)
				{
					BeginProcess(log);
					ProcessIF process = ProcessManager.Instance.GetProcess(log.SensorHistoryID);
					if (process != null)
					{
						FormMain.Instance.Invoke((MethodInvoker)delegate
						{
							FormMain.Instance.AddFireDectect((FireDetectProcess)process, false);
						});
					}
					Thread.Sleep(300);
				}
                else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE || log.ReactionType == (int)ReactionType.TRAINNING_FIRE)
                {
                    // 화재 신고 또는 탐지 부터 받을시  해당 프로세스가 없는 경우 추가 해준다.
                    ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                    if (process == null)
                    {
                        BeginProcess(log, true);
                        process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                        if (process != null)
                        {
                            FormMain.Instance.Invoke((MethodInvoker)delegate
                            {
                                FormMain.Instance.AddFireDectect((FireDetectProcess)process, false);
								bRunSimulator = true;
                            });
                        }
                    }
                }
				else if (log.ReactionType != (int)ReactionType.END_STATUS)
				{
					AddProcess(log, false);
				}

				if (log.ReactionType == (int)ReactionType.IGNORE_SOP 
					|| log.ReactionType == (int)ReactionType.RUN_N_CANCEL_SOP 
					|| log.ReactionType == (int)ReactionType.FINISH_SOP)
				{
					// 종료되지 않은 수동신고의 경우 종료 할 수 있도록 프로세스를 추가해준다.
					int nHistoryID = log.SensorHistoryID;
					int nZoneID = SensorHistoryManager.Instance.GetManualFireReportZone(nHistoryID);
					if (nZoneID != -1)
					{
						log.Parameter1 = nZoneID.ToString();
						log.Parameter2 = "0";
						BeginProcess(log, true);						
					}				
				}
				
				// 화재신고가 포함되어 있는경우 SOP시뮬레이터 기동
				if(bRunSimulator == true)
					FormMain.Instance.SendFireDetectMessageToSOPSimulator();

                ReactionLogManager.Instance.AddLog(log);
            }


			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
				FormMain.Instance.SelectLastFireDectectProcess();
			});

			FireDetectProcess processLast = FormMain.Instance.LastFireDetectProcess;
			if (processLast != null)
			{
				bool bSelected = processLast.Select();
				if (bSelected)
				{
					ReactionLogManager.Instance.ProcessLog(processLast.LastLog, true);
				}
			}

            // 기존에 진행되고 있던 화재 상황에 대한 Log
            foreach (ReactionLog log in arrCurrentLog)
            {
                ReactionLogManager.Instance.AddLog(log);
            }
        }

        private void AddProcess(ReactionLog log, bool bAddSelected = true)
        {
            string strSQL = string.Format("select szh.SensorID, sz.Type, sz.OrgSensorID, fs.X, fs.Y, fs.Z, sz.EquipZoneID from SensorZoneHistory as szh, SensorZone as sz, FireSensor as fs where szh.ID = {0} and szh.SensorID = sz.ID and sz.OrgSensorID = fs.ID",
                log.SensorHistoryID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 7)
                return;

            int nSensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            int nSensorType = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            int nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[2].ToString(), -1);
            float x = DBUtility.WebDBManager.GetFloatField(arrResult[3].ToString(), 0.0f);
            float y = DBUtility.WebDBManager.GetFloatField(arrResult[4].ToString(), 0.0f);
            float z = DBUtility.WebDBManager.GetFloatField(arrResult[5].ToString(), 0.0f);
            int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[6].ToString(), -1);

            if (!ProcessManager.Instance.CurrentDetectProcess.ContainsKey(nSensorID))
            {
                FireDetectProcess process = null;

                if (nSensorType == 1)
                    process = new FireDetectProcess();
                else
                    return;

                if (!SensorManager.Instance.DicAllSenor.ContainsKey(nSensorID))
                    return;

                EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                if (zone == null)
                    return;

                SensorZone sensor = SensorManager.Instance.DicAllSenor[nSensorID];

                process.DetectSensorID = nSensorID;
                process.SensorHistoryID = log.SensorHistoryID;
                process.TargetSensor = sensor;
                process.TargetZone = zone;

                ProcessManager.Instance.CurrentDetectProcess[nSensorID] = process;

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
					FormMain.Instance.AddFireDectect(process, bAddSelected);
                });
            }
        }

        private ReactionLog FindLog(ArrayList arrReactionLog, int nSensorHistoryID)
        {
            foreach (ReactionLog log in arrReactionLog)
            {
                if (log.SensorHistoryID == nSensorHistoryID)
                    return log;
            }

            return null;
        }

        private ReactionLog ReadReactionHistoryLog(byte[] bytes, ref int nOffset, int chunkSize)
        {
            ReactionLog log = new ReactionLog();

            int nLogID = -1;
            int nSensorHistoryID = -1;
            int nReadDataCount = nOffset;          

            // Reaction History ID
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nLogID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
            }
            chunkSize -= 1;
            log.ID = nLogID;

            // Sensor History ID
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
            }
            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            // Reaction Type
            int nReactionType = -1;
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nReactionType = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
            }
            chunkSize -= 1;
            log.ReactionType = nReactionType;

            for (int i = 0; i < chunkSize; i++)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;

                // Message , Parameter1
                if (dataHeader == TCP_TYPE.STRING)
                {
                    byte[] bytesBlock = new byte[nDataLength];
                    System.Buffer.BlockCopy(bytes, nReadDataCount, bytesBlock, 0, nDataLength);
                    string szValue = Encoding.UTF8.GetString(bytesBlock, 0, nDataLength);
                    nReadDataCount += nDataLength;

                    switch (i)
                    {
                        case 1: //  Message 
                            log.Message = szValue;
                            break;
                        case 2: // Param 1
                            log.Parameter1 = szValue;
                            break;
                        case 3: // Param 2
                            log.Parameter2 = szValue;
                            break;
                        case 4: // Param 3
                            log.Parameter3 = szValue;
                            break;
                        case 5: // Param 4
                            log.Parameter4 = szValue;
                            break;
                        case 6: // Param 5
                            log.Parameter5 = szValue;
                            break;
                    }
                }
                // LogTime
                if (dataHeader == TCP_TYPE.LONG)
                {
                    long value = BitConverter.ToInt64(bytes, nReadDataCount);

                    nReadDataCount += nDataLength;
                    if (value == 0)
                        log.LogTime = DateTime.Now;
                    else
                        log.LogTime = DateTime.FromBinary(value);
                }
            }

            nOffset = nReadDataCount;
            return log;
        }

		private ReactionLog ReadReactionHistoryLog(byte[] bytes)
		{
            int nOffset = 6;
            int chunkSize = (int)BitConverter.ToInt16(bytes, 2);
            return ReadReactionHistoryLog(bytes, ref nOffset, chunkSize);
		}

        public override void OnReceiveData()
        {
            OnReceive(ReceivedData);
        }

       
        private bool OnReceive(byte[] bytes)
        {
            try
            {
                if (bytes != null)
                {
                    m_isReadingProcess = true;

                    m_arrReceived = bytes;

                    int nBytesCount = m_arrReceived.Count();

                    if (nBytesCount > 0)
                    {
                        m_nPingCount = 0;
                        SendData(TCP_ID.I_AM_HERE);

						m_mgr.RecvLog(m_arrReceived, m_nProviderNum);

                        short nHeader;
                        ArrayList arrDatas = ReadBytes(m_arrReceived, out nHeader);

                        if (nHeader == TCP_ID.ARE_YOU_THERE)
                        {
                            //SendData(TCP_ID.I_AM_HERE);
                        }
                        else if (nHeader == TCP_ID.WHO_ARE_YOU)
                        {
                            SendData(TCP_ID.WHO_I_AM, TCP_TYPE.INTEGER, BitConverter.GetBytes(m_nProviderType));
                        }
                        else if (nHeader == TCP_ID.SENSOR_REACTION_HISTORY_DATA)
                        {
                            lock (m_mgr.Lock)
                            {
                                ReactionLog log = ReadReactionHistoryLog(m_arrReceived);

                                if (log.ReactionType == (int)ReactionType.BEGIN_STATUS)
                                {
                                    BeginProcess(log);
                                }
                                else if (log.ReactionType == (int)ReactionType.MALFUNCTION)
                                {
                                    EndProcess(log.SensorHistoryID);
                                    m_isReadingProcess = false;
                                    return true;
                                }
                                else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE)
                                {
                                    if (FireDetectProcess.SoundPlayer.IsLoadCompleted == true)
                                    {
                                        FireDetectProcess.SoundPlayer.Stop();
                                    }
                                    // 수동 신고
                                    if (log.Parameter2 == "0")
                                    {
                                        Debug.WriteLine("Recive Manual Report Log");
                                        BeginProcess(log);
                                        //FormMain.Instance.SendFireDetectMessageToSOPSimulator();
                                    }
                                }

                                if (log.ReactionType != (int)ReactionType.BEGIN_STATUS)
                                {
                                    DlgSelectCase.ProcessingSensorHistoryID = log.SensorHistoryID;
                                }
                                ReactionLogManager.Instance.AddLog(log);
                            }
                        }
                        else if (nHeader == TCP_ID.CLEAR_DETECT_REPORT)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessClearProcess(m_arrReceived);
                            }                            
                        }
                        else if (nHeader == TCP_ID.SENSOR_REACTION_HISTORY_DATA_LIST)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessReactionHistoryLogList(m_arrReceived);
                            }
                        }
                        else if (nHeader == TCP_ID.SENSOR_ZONE_DATA)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessSensorData(m_arrReceived);
                            }
                        }
                        else if (nHeader == TCP_ID.IGNORE_DETECT_REPORT)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessIgnoreDetect(m_arrReceived);
                            }
                        }                        
						else if (nHeader == TCP_ID.ALL_RECIVER_STATE)
						{
                            ProcessAllReciverState(arrDatas);
						}
						else if (nHeader == TCP_ID.RECIVER_CONNECT || nHeader == TCP_ID.RECIVER_DISCONNECT)
						{
							int nReciverID = BitConverter.ToInt32(m_arrReceived, 11);
							int nConnected = BitConverter.ToInt32(m_arrReceived, 20);
							ProcessReciverState(nReciverID, nConnected);
						}
						else if (nHeader == TCP_ID.CHANGE_CONFIG)
						{
                            ProcessChangeConfig(arrDatas);
						}
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            m_isReadingProcess = false;
            return true;
        }

        private void ProcessChangeConfig(ArrayList arrDatas)
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
                        if (((nConfigValue & (int)SDMSConfig.ConfigType.COMPANY_MEMBER) == (int)SDMSConfig.ConfigType.COMPANY_MEMBER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.REGULAR_TEAM) == (int)SDMSConfig.ConfigType.REGULAR_TEAM) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) == (int)SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) == (int)SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM))
                            ProcessChangeCompanyMember();

                        if (((nConfigValue & (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER))
                            ProcessChangeFacilityManager();
                    }
                }
                else if (strPropertyName == SDMSConfig.GetPropertyName(SDMSConfig.ConfigType.EQUIPZONE_CCTV))
                {
                    int nEquipZoneID;

                    if (int.TryParse(strPropertyValue, out nEquipZoneID))
                    {
                        ProcessChangeEquipZoneCCTV(nEquipZoneID);
                    }
                }
            }
            catch (Exception e)
            {
				ConnectionLogEx.Instance.WriteLine(e.StackTrace);
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

		private void ProcessAllReciverState(ArrayList arRecivers)
		{
			int nReciverID = -1;
			bool bConnected = false;

            int nDataCount = arRecivers.Count;

			for (int i = 0; i < nDataCount; i += 2)
			{
                nReciverID = (int)arRecivers[i];
                bConnected = (int)arRecivers[i + 1] == 1 ? true : false;

				if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
				{
					Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
                  	ReciverManager.Instance.UpdateState(nReciverID, bConnected);					
				}
				this.PingCount = 0;
			} 
		}

		private void ProcessReciverState(int nReciverID, int nConnect)
		{
			ReciverManager.Instance.UpdateState(nReciverID, (nConnect == 1 ? true : false));
		}

		private void ProcessChangeEquipZoneCCTV(int nEquipZoneID)
		{
			FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
			{
				// CCTV 뷰 닫기
				if (FormMain.Instance.ShowEquipZoneCCTV)
				{
					if (FormMain.Instance.CurrentEquipZone.ID == nEquipZoneID)
					{
						// EditEquipZoneCCTV의 데이터 삭제
						PageBackstageHome.Instance.RemoveEquipZoneCCTVData();

                        if (PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(Form4CCTV))
                        {
                            // cCTV뷰 닫기
                            FormMain.Instance.ShowEquipZoneCCTV = false;
                            PageBackstageHome.TranslucentForm.InnerForm.Close();
                        }
					}
				}
				CCTVManager.Instance.LoadEquipZoneCCTV(nEquipZoneID);
			});
		}

		private void ProcessChangeCompanyMember()
		{
			FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
			{
				FormMain.Instance.DataManager.ReloadCompanyMember();
			});
		}

        private void ProcessChangeFacilityManager()
        {
			FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
			{
				// EditFacilityManager 의 데이터 삭제
				PageBackstageHome.Instance.RemoveEditManagerData();

				// FormEditManager 닫기
				if (PageBackstageHome.TranslucentForm.InnerSubForm != null &&
					PageBackstageHome.TranslucentForm.InnerSubForm.GetType() == typeof(FormEditManager))
				{
					PageBackstageHome.TranslucentForm.InnerSubForm.Close();
				}
				// FormManager 닫기


				if (PageBackstageHome.TranslucentForm.InnerForm != null &&
					PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(FormManager))
				{
					PageBackstageHome.TranslucentForm.InnerForm.Close();
				}
				FormMain.Instance.DataManager.LoadFacilityManager();
			});
        }

        private void ProcessIgnoreDetect(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            EndProcess(nSensorHistoryID);
        }


		/// <summary>
		/// 센서 접속정보 변경에 따른 이벤트 생성 함수
		/// </summary>
		/// <param name="bytes"></param>
        private void ProcessSensorData(byte[] bytes)
        {
            int nSensorZoneID = BitConverter.ToInt32(bytes, 11); // 11
            int nSensorType = BitConverter.ToInt32(bytes, 20); // 20
            int nConnected = BitConverter.ToInt32(bytes, 29); // 
            int nZoneID = BitConverter.ToInt32(bytes, 38); // 
            int nSensorData = BitConverter.ToInt32(bytes, 47);
            int nSensorID = BitConverter.ToInt32(bytes, 56);

			if (SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
            {
				SensorZone sensor = SensorManager.Instance.DicAllSenor[nSensorZoneID];

				bool bPrevState = sensor.Connected;
                sensor.Connected = nConnected == 1;
                sensor.SensorData = nSensorData;
                sensor.SensorData = nSensorData;
                if (sensor.POI != null)
                {
                    sensor.POI.Facility.Connected = (nConnected == 1);
                    int nHistoryID = SensorHistoryManager.Instance.HistoryID;
                    if (bPrevState != sensor.Connected && sensor.Connected == false)
                    {
                        ProcessManager.Instance.BeginProcess(sensor, nHistoryID, ProcessType.DisconnectSensor);
                    }
                    else if (bPrevState != sensor.Connected && sensor.Connected == true)
                    {
                        ProcessManager.Instance.BeginProcess(sensor, nHistoryID, ProcessType.ConnectSensor);
                    }				
                }			
				
            }
        }

		/// <summary>
		/// 해당 History id에 대한 프로세스 종료
		/// </summary>
		/// <param name="nSensorHistoryID"></param>
        private void EndProcess(int nSensorHistoryID)
        {
            ProcessIF process = ProcessManager.Instance.FindProcess(nSensorHistoryID);
            EndProcess(process);
        }

		/// <summary>
		/// Clear Sensor Data 에서 사용되는 프로세스 종료및 제거 - 상황종료
		/// </summary>
		/// <param name="bytes"></param>
        private void EndProcess(byte[] bytes)
        {

            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            ProcessIF process = ProcessManager.Instance.FindProcess(nSensorHistoryID);
			if (process != null && process.ToString() != "")
			{
                Debug.WriteLine("EndProcess :" + nSensorHistoryID.ToString());
                Debug.WriteLine("EndProcess :" + process);
                EndProcess(process);
                RemoveProcess(process);
			}
        }

        private void ProcessClearProcess(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            ProcessIF process = ProcessManager.Instance.FindProcess(nSensorHistoryID);
            if (process != null && process.ToString() != "")
            {
                Debug.WriteLine("EndProcess :" + nSensorHistoryID.ToString());
                Debug.WriteLine("EndProcess :" + process);
                EndProcess(process);
                RemoveProcess(process);

                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.SetNormalMode(0);
                        PageBackstageHome.Instance.ContentForm.HideZoneVolume();
                        ConnectionLogEx.Instance.WriteLine("Hide All Zone Volume");
                    });
                } 
            }
            else
            {
                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.SetNormalMode(0);
                        PageBackstageHome.Instance.ContentForm.HideZoneVolume();
                        ConnectionLogEx.Instance.WriteLine("Hide All Zone Volume");
                    });
                }                
            }           
        }        

		/// <summary>
		/// 해당 프로세스를 종료 시키고 프로세스의 HistoryID에대해 정상 모드로 변경 
		/// </summary>
		/// <param name="process"></param>
        private void EndProcess(ProcessIF process)
        {

            if (process != null)
            {
                int nHistoryID = process.SensorHistoryID;
                int nSensorID = process.TargetSensor.ID;
                ProcessManager.Instance.EndProcess(process);
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    ConfirmDialogManager.Instance.RemoveDialog(nHistoryID, nSensorID);
                });
                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        ConfirmDialogManager.Instance.CloseAllDialog();
                        FormMain.Instance.SetNormalMode(process.SensorHistoryID);
                    });
                }
            }
            else
            {
                

                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        ConfirmDialogManager.Instance.CloseAllDialog();
                        FormMain.Instance.SetNormalMode(0);
                    });
                }

            }
        }

		private void RemoveProcess(ProcessIF process)
		{
			if (process != null)
			{
                if (process.TargetSensor != null)
                    process.TargetSensor.SoundOn = true;

				// 참조하지 않도록 프로제스 목록에서 제거
				ProcessManager.Instance.RemoveProcess(process);
				// 해당 History ID 제거
				SensorHistoryManager.Instance.RemoveSensorHistory(process.SensorHistoryID);
				// Combo박스에서 제거
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.RemoveFireDetect((FireDetectProcess)process);

                    if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                    {
                        FormMain.Instance.SetNormalMode(0);
                    }
				});

                
			}
		}

        private void BeginProcess(ReactionLog log, bool bAddOnly = false)
        {
            int nZoneID = -1, nSensorZoneID = -1;

            int.TryParse(log.Parameter1, out nZoneID);
            int.TryParse(log.Parameter2, out nSensorZoneID);

            EquipmentZone zone = null;
            SensorZone sensor = null;

            if (nZoneID > 0)
                zone = ZoneManager.Instance.GetEquipZone(nZoneID);

			if (nSensorZoneID > 0)
			{
				if (SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
				{
					sensor = SensorManager.Instance.DicAllSenor[nSensorZoneID];
				}
			}
			else // 수동 신고
			{
                Debug.WriteLine("Recive Manual Report Log");
				SensorHistoryManager.Instance.AddSensorHistoryID(log.SensorHistoryID);
				System.Diagnostics.Trace.WriteLine(string.Format("BeginProcess Maual"));
				FireSensor ss = new FireSensor();
				ss.ID = nZoneID * 1000;
				Zone realzone = ZoneManager.Instance.GetZone(nZoneID);
				ProcessManager.Instance.BeginProcess(ss, realzone, log.SensorHistoryID, ProcessType.FireAlarm, false);
				FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.FindProcess(log.SensorHistoryID);
				if (process != null)
				{
					process.LastLog = log;
					FormMain.Instance.Invoke((MethodInvoker)delegate
					{
						FormMain.Instance.AddFireDectect(process, !bAddOnly);
					});
				}				
				return;
			}

            System.Diagnostics.Trace.WriteLine(string.Format("SensorZoneID : {0}", nSensorZoneID));

            if (sensor != null)
            {
				SensorHistoryManager.Instance.AddSensorHistoryID(log.SensorHistoryID);
                System.Diagnostics.Trace.WriteLine(string.Format("BeginProcess"));
				ProcessManager.Instance.BeginProcess(sensor, log.SensorHistoryID, ProcessType.FireAlarm, !bAddOnly);
            }
        }

		public void SendData(short header, List<KeyValuePair<byte, byte[]>> arList)
		{
			if (header < 0)
				return;

			if (arList == null || arList.Count >= 10000)
				return;
			
			int dataLength = 0;

			foreach ( KeyValuePair<byte, byte[]> pair in arList)
			{
				dataLength += pair.Value.Length;
				dataLength += 5;
			}

			byte[] sndData = new byte[dataLength + 6];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(arList.Count);

			sndData[0] = nHader[0];
			sndData[1] = nHader[1];

			sndData[2] = nCount[0];
			sndData[3] = nCount[1];
			sndData[4] = nCount[2];
			sndData[5] = nCount[3];

			int nDataCount = 6;
			foreach (KeyValuePair<byte, byte[]> pair in arList)
			{
				byte[] datas = pair.Value;

				sndData[nDataCount++] = pair.Key;
				byte[] lengthData = BitConverter.GetBytes(datas.Length);
				for (int i = 0; i < 4; i++)
				{
					sndData[nDataCount++] = lengthData[i];	
				}
				for (int i = 0; i < datas.Length; i++)
				{
					sndData[nDataCount++] = datas[i];
				}
			}
            if (this.IsClientDisposed == false)
                m_mgr.Send(sndData, this);
		}

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
			byte[] bytes = new byte[6];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(0);

			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			if (this.Client.Client != null)
			{
				if (this.Client.Client.Connected == true)
					m_mgr.Send(bytes, this);
			}           
        }

		public void SendData(short header, byte dataHeader, byte[] datas)
		{
			if (header < 0)
				return;

			if (datas.Length >= 10000)
				return;
			if (datas == null || datas.Length == 0)
				return;

			byte[] sndData = new byte[datas.Length + 11];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			sndData[0] = nHader[0];
			sndData[1] = nHader[1];

			// SET DATA COUNT
			sndData[2] = nCount[0];
			sndData[3] = nCount[1];
			sndData[4] = nCount[2];
			sndData[5] = nCount[3];  

			// SET DATA TYPE
			sndData[6] = dataHeader;
			
			// SET DATA LENGTH
			byte[] lengthData = BitConverter.GetBytes(datas.Length);
			for (int i = 0; i < 4; i++)
			{
				if (lengthData.Length > i)
				{
					sndData[7 + i] = lengthData[i];
				}
			}

			// SET DATA
			for (int i = 0; i < datas.Length; i++)
			{
				sndData[i + 11] = datas[i];
			}

			if (this.IsClientDisposed == false)
				m_mgr.Send(sndData, this);
		}

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection(m_nProviderNum);
            //m_arrTemp = null;

            if (m_nProviderNum == 2)
            {
                try
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        foreach (Reciver reciver in ReciverManager.Instance.DicReciverList.Values)
                        {
                            reciver.State = 0;
                            int nReciverID = reciver.ID;
                            bool bConnected = false;
                            ReciverManager.Instance.UpdateState(nReciverID, bConnected);
                        }
                    });
                }
                catch (System.Exception)
                {                	
                }
                
            }
			
        }

        public new void Close()
        {
            base.Close();
            //m_arrTemp = null;
        }

        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] datas = enc.GetBytes(data);

            int nDataLength = datas.Length;

            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(bool data)
        {
            int nDataLength = sizeof(bool);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BOOLEAN;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short data)
        {
            int nDataLength = sizeof(short);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.SHORT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(byte data)
        {
            int nDataLength = sizeof(byte);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BYTE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short nHeader, ArrayList arrDatas)
        {
            int nChunkCount = arrDatas == null ? 0 : arrDatas.Count;

            ArrayList arrBytes = new ArrayList();
            int nBytesCount = 0;

            for (int i = 0; i < nChunkCount; i++)
            {
                object data = arrDatas[i];
                Type type = data.GetType();
                byte[] bytes = null;

                if (type == typeof(int))
                    bytes = MakeBytes((int)data);
                else if (type == typeof(long))
                    bytes = MakeBytes((long)data);
                else if (type == typeof(float))
                    bytes = MakeBytes((float)data);
                else if (type == typeof(bool))
                    bytes = MakeBytes((bool)data);
                else if (type == typeof(double))
                    bytes = MakeBytes((double)data);
                else if (type == typeof(short))
                    bytes = MakeBytes((short)data);
                else if (type == typeof(byte))
                    bytes = MakeBytes((byte)data);
                else if (type == typeof(string))
                    bytes = MakeBytes((string)data);
                else
                    return null;

                nBytesCount += bytes.Length;
                arrBytes.Add(bytes);
            }

            byte[] _bytes = new byte[6 + nBytesCount];
            byte[] headerBytes = BitConverter.GetBytes(nHeader);
            byte[] lengthBytes = BitConverter.GetBytes(nChunkCount);

            _bytes[0] = headerBytes[0];
            _bytes[1] = headerBytes[1];
            _bytes[2] = lengthBytes[0];
            _bytes[3] = lengthBytes[1];
            _bytes[4] = lengthBytes[2];
            _bytes[5] = lengthBytes[3];

            int nIndex = 6;

            foreach (byte[] bytes in arrBytes)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _bytes[nIndex + i] = bytes[i];
                }

                nIndex += bytes.Length;
            }

            return _bytes;
        }

        private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nTotalLength, out bool isNullData)
        {
            isNullData = false;

            if (nBytesLength < nIndex + 5)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;
            else if (nDataLength > 0)
            {
                if (nBytesLength < nIndex + nTotalLength)
                    return false;

                nIndex += nTotalLength;
            }
            else
            {
                isNullData = true;
                nIndex += 5;
            }

            return true;
        }

        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            int nLength = bytes.Length;

            if (nLength < 6)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 6;
            bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                byte type = bytes[nIndex];

                if (type == TCP_TYPE.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                        arrResult.Add(nData);
                    }
                }
                else if (type == TCP_TYPE.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                        arrResult.Add(fData);
                    }
                }
                else if (type == TCP_TYPE.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                        arrResult.Add(dData);
                    }
                }
                else if (type == TCP_TYPE.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                        arrResult.Add(lData);
                    }
                }
                else if (type == TCP_TYPE.BOOLEAN)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                        arrResult.Add(bData);
                    }
                }
                else if (type == TCP_TYPE.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 7, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                        arrResult.Add(sData);
                    }
                }
                else if (type == TCP_TYPE.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        byte data = bytes[nIndex - 1];
                        arrResult.Add(data);
                    }
                }
                else if (type == TCP_TYPE.STRING)
                {
                    if (nLength < nIndex + 5)
                        return null;

                    int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                    if (nDataLength < 0)
                        return null;
                    else if (nDataLength > 0)
                    {
                        if (nLength < nIndex + 5 + nDataLength)
                            return null;

                        string strData = Encoding.UTF8.GetString(bytes, nIndex + 5, nDataLength);
                        arrResult.Add(strData);

                        nIndex += 5 + nDataLength;
                    }
                    else
                    {
                        arrResult.Add("");
                        nIndex += 5;
                    }
                }
                else
                    return null;
            }

            return arrResult;
        }

        public void SendRequestDataList()
        {
            SendData(TCP_ID.REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST);
        }

        public void SendChangedConfig(byte byteClientType, string strPropertyName, string strPropertyValue)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(byteClientType);
            arrDatas.Add(strPropertyName);
            arrDatas.Add(strPropertyValue);

            byte[] bytes = MakeBytes(TCP_ID.CHANGE_CONFIG, arrDatas);

            m_mgr.Send(bytes, this);
        }

        // 현재 진행중인 화재신호가 유효한 값인지 DB에서 검사한다.
        public void CheckValidProcess()
        {
            if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
            {
                if (FormMain.Instance != null && FormMain.Instance.IsHandleCreated)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {                        
                        FormMain.Instance.ClearAllFireDetect();                        
                    });
                }
                return;
            }

            try
            {
                string strSubSQL = "";
                Dictionary<int, ProcessIF> dicSensorZoneProcess = new Dictionary<int,ProcessIF>();

                foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
                {
                    dicSensorZoneProcess[pair.Value.TargetSensor.ID] = pair.Value;

                    if (strSubSQL.Length == 0)
                        strSubSQL = pair.Value.TargetSensor.ID.ToString();
                    else
                        strSubSQL += ", " + pair.Value.TargetSensor.ID.ToString();
                    
                    //string str = string.Format("(select max(id) from SensorZoneHistory where SensorID = {0})", pair.Value.TargetSensor.ID);

                    //if (strSubSQL.Length == 0)
                    //    strSubSQL = str;
                    //else
                    //    strSubSQL += ", " + str;
                }

                int nCount = ProcessManager.Instance.CurrentDetectProcess.Count;

                string strSQL = string.Format("select ID, Data from SensorZone where id in ({0})", strSubSQL);

                int nDetectCount = 0;
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
                if (arrResult != null)
                {
                    int nResultCount = arrResult.Count;

                    for (int i = 0; i < nResultCount - 1; i += 2)
                    {
                        int nSensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                        int nData = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                        nDetectCount++;
                        if (nData == 1)
                            dicSensorZoneProcess.Remove(nSensorZoneID);
                    }

                    foreach (KeyValuePair<int, ProcessIF> pair in dicSensorZoneProcess)
                    {
                        EndProcess(pair.Value);
                        RemoveProcess(pair.Value);
                    }

                    if (nDetectCount != nCount)
                    {
                        SendRequestDataList();
                    }
                }                

                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    if (FormMain.Instance != null && FormMain.Instance.IsHandleCreated)
                    {
                        FormMain.Instance.Invoke((MethodInvoker)delegate
                        {                           
                            FormMain.Instance.ClearAllFireDetect();                            
                        });
                    }
                    return;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
