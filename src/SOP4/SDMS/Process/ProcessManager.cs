using System;
using System.Collections.Generic;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;

namespace SDMS
{
	public enum ProcessType
	{
		FireAlarm = 1,
		DisconnectSensor,
		ConnectSensor,
        PSMAlarm,
        SecurityAlarm,
	}

	public interface ProcessIF
	{
        DateTime DetectTime
        {
            get;
            set;
        }

		int DetectSensorID
		{
			get;
			set;
		}

		ISensor TargetSensor
		{
			get;
			set;
		}

		EquipmentZone TargetZone
		{
			get;
			set;
		}

		int SensorHistoryID
		{
			get;
			set;
		}

        bool ShowOpenSOP
        {
            get;
            set;
        }
        // 프로세스의 시작 동작
		void BeginProcess();

        // 프로세스의 시작하지 않는경우 필요한 준비 동작
        void ReadyProcess();

        // 프로세스의 중지 동작
		void AbortProcess();

        ReactionLog LastLog
        {
            get;
            set;
        }

        bool Select();

        void HideCCTV();

        ProcessType ProcessType
        {
            get;
        }

        int AlarmLevel
        {
            get;
            set;
        }
	}

	public class ProcessManager : IDisposable
	{
		private static ProcessManager m_instance = null;

		public static ProcessManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ProcessManager();
				return m_instance;
			}
		}

		//private Mutex m_ProcessMutex = new Mutex(false);
		// Key : Sensor ID(not SensorZone)
		private Dictionary<int, ProcessIF> m_dicCurrentDetectProcess = new Dictionary<int, ProcessIF>();

		public Dictionary<int, ProcessIF> CurrentDetectProcess
		{
			get { return m_dicCurrentDetectProcess; }
		}

		private ProcessManager()
		{
		}

		public void Dispose()
		{
			foreach (KeyValuePair<int, ProcessIF> pair in m_dicCurrentDetectProcess)
			{
				ProcessIF process = pair.Value;
				process.AbortProcess();
			}
			m_dicCurrentDetectProcess.Clear();
		}

		public void EndProcess(ProcessIF process)
		{
			try
			{
				if (FireDetectProcess.SoundPlayer.SoundLocation != null)
					FireDetectProcess.SoundPlayer.Stop();
			}
			catch (System.Exception)
			{
			}

			try
			{
				process.AbortProcess();
			}
			catch (System.Exception)
			{
			}
		}

		public void RemoveProcess(ProcessIF process)
		{
            try
            {
                if (FireDetectProcess.SoundPlayer.SoundLocation != null)
                    FireDetectProcess.SoundPlayer.Stop();
            }
            catch (System.Exception)
            {
            }

			if (m_dicCurrentDetectProcess.ContainsKey(process.DetectSensorID))
			{
				m_dicCurrentDetectProcess.Remove(process.DetectSensorID);
				//System.Diagnostics.Trace.WriteLine("Detect Dictionary Remove");
			}
		}

		public void RemoveProcess(int nKey)
		{
            if (FireDetectProcess.SoundPlayer.SoundLocation != null)
                FireDetectProcess.SoundPlayer.Stop();

			if (m_dicCurrentDetectProcess.ContainsKey(nKey))
			{
				m_dicCurrentDetectProcess.Remove(nKey);
			}
		}

		public void EndProcess(int nKey)
		{
			if (FireDetectProcess.SoundPlayer.SoundLocation != null)
				FireDetectProcess.SoundPlayer.Stop();
			ProcessIF process = GetProcess(nKey);
			if (process != null)
			{
				process.AbortProcess();
			}
		}

		public ProcessIF BeginProcess(ISensor sensor, ReactionLog log, ProcessType type, bool bRunProcess = true)
		{ 
			if (sensor == null)
				return null;

			ProcessIF process = null;

			if (type == ProcessType.DisconnectSensor || type == ProcessType.ConnectSensor)
			{
				process = CreateProcess(type);
				if (process == null)
					return null;
				process.DetectSensorID = sensor.ID;
				process.TargetSensor = (ISensor)sensor;
				process.TargetZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);
                process.LastLog = log;
				process.SensorHistoryID = log == null ? -1 : log.SensorHistoryID;
                
                if (bRunProcess == true)
                {
                    process.BeginProcess();
                    // SOP Loading
                    if (!FormMain.Instance.UsePopupSensorOn)
                    {

                        UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                        if (container != null)
                        {
                            DateTime sopTime = log == null ? DateTime.Now : log.LogTime;

                            // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                            // skkim 2017-04-11
                            // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                            if( UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                                FormMain.Instance.OpenSOP(process.TargetZone, sopTime, process);
                            }
                            else
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                            }
                        }                        
                    }
                }
                else
                {
                    if (!FormMain.Instance.UsePopupSensorOn)
                    {
                        UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                        if (container != null)
                        {
                            DateTime sopTime = log == null ? DateTime.Now : log.LogTime;
                            // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                            // skkim 2017-04-11
                            // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                                FormMain.Instance.OpenSOP(process.TargetZone, sopTime, process);
                            }
                            else
                            {
                                process.DetectTime = sopTime;
                                process.ShowOpenSOP = true;
                            }
                        }
                    }
                    process.ReadyProcess();
                }
			}
			else
			{
				if (!m_dicCurrentDetectProcess.ContainsKey(sensor.ID))
				{
					process = CreateProcess(type);
					if (process == null)
						return null;

					m_dicCurrentDetectProcess.Add(sensor.ID, process);
                    process.LastLog = log;
					process.DetectSensorID = sensor.ID;
					process.TargetSensor = (ISensor)sensor;
					process.TargetZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);
					process.SensorHistoryID = log == null ? -1 : log.SensorHistoryID;

                    // PSM의 경우 초기 Alarm값을 설정하여 준다. 대피반경에서 사용.
                    // added by skkim 2016-04-07
                    if(type == ProcessType.PSMAlarm)
                    {
                        try
                        {
                            int nLevel = Convert.ToInt32(log.Parameter5);
                            process.AlarmLevel = nLevel;
                        }
                        catch(Exception)
                        { } 
                    }
                    
                    if (bRunProcess == true)
                    {
                        process.BeginProcess();
                        // SOP Loading
                        if (!FormMain.Instance.UsePopupSensorOn)
                        {
                           UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                           if (container != null)
                           {
                               DateTime sopTime = log == null ? DateTime.Now : log.LogTime;
                               // 
                               // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                               // skkim 2017-04-11
                               // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                               if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                               {
                                   process.ShowOpenSOP = true;
                                   process.DetectTime = sopTime;
                                   FormMain.Instance.OpenSOP(process.TargetZone, sopTime, process);
                               }
                               else
                               {
                                   process.DetectTime = sopTime;
                                   process.ShowOpenSOP = true;
                               }
                           }
                        }
                    }
                    else
                    {
                        if (!FormMain.Instance.UsePopupSensorOn)
                        {
                            UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                            if (container != null)
                            {
                                DateTime sopTime = log == null ? DateTime.Now : log.LogTime;
                                // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                                // skkim 2017-04-11
                                // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                {
                                    process.ShowOpenSOP = true;
                                    process.DetectTime = sopTime;
                                    FormMain.Instance.OpenSOP(process.TargetZone, sopTime, process);
                                }
                                else
                                {
                                    process.DetectTime = sopTime;
                                    process.ShowOpenSOP = true;
                                }
                            }
                        }

                        process.ReadyProcess();
                    }
				}
			}

			return process;
		}

        /// <summary>
        /// 수동신고의 경우 ReactionLog  가 없이 가상 센서로 생성한다.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="zone"></param>
        /// <param name="nSensorHistoryID"></param>
        /// <param name="type"></param>
        /// <param name="bRunProcess"></param>
		public void BeginProcess(ISensor sensor, Zone zone, int nSensorHistoryID, ProcessType type, bool bRunProcess = true)
		{
			if (type == ProcessType.DisconnectSensor || type == ProcessType.ConnectSensor)
			{
				ProcessIF process = CreateProcess(type);
				if (process == null)
					return;
                
				process.DetectSensorID = sensor.ID;
				process.TargetSensor = sensor;
				process.TargetZone = null;
				process.SensorHistoryID = nSensorHistoryID;
                if (bRunProcess == true)
                    process.BeginProcess();
                else
                    process.ReadyProcess();
            }
			else
			{
				if (!m_dicCurrentDetectProcess.ContainsKey(sensor.ID))
				{
					ProcessIF process = CreateProcess(type);
					if (process == null)
						return;

					m_dicCurrentDetectProcess.Add(sensor.ID, process);

					process.DetectSensorID = sensor.ID;
					process.TargetSensor = (ISensor)sensor;
					EquipmentZone eqzone = new EquipmentZone();
					eqzone.LinkedZone = zone;
					eqzone.Polygon = zone.Polygon;
                    eqzone.ZoneName = zone.DisplayText;
					process.TargetZone = eqzone;
					process.SensorHistoryID = nSensorHistoryID;                   

                    if (bRunProcess == true)
                    {
                        process.BeginProcess();

                        // SOP Loading
                        if (!FormMain.Instance.UsePopupSensorOn)
                        {
                            DateTime sopTime = DateTime.Now;

                            UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                            if (container != null)
                            {
                                // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                                // skkim 2017-04-11
                                // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                {
                                    process.ShowOpenSOP = true;
                                    process.DetectTime = sopTime;
                                    FormMain.Instance.OpenSOP(process.TargetZone, sopTime, process);
                                }
                                else
                                {
                                    process.DetectTime = sopTime;
                                    process.ShowOpenSOP = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!FormMain.Instance.UsePopupSensorOn)
                        {
                            DateTime sopTime = DateTime.Now;

                            UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                            if (container != null)
                            {
                                // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                                // skkim 2017-04-11
                                // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                {
                                    process.ShowOpenSOP = true;
                                    process.DetectTime = sopTime;
                                    FormMain.Instance.OpenSOP(process.TargetZone, sopTime, process);
                                }
                                else
                                {
                                    process.DetectTime = sopTime;
                                    process.ShowOpenSOP = true;
                                }
                            }
                        }
                        process.ReadyProcess();
                    }

                   
				}
			}
		}

		private ProcessIF CreateProcess(ProcessType type)
		{
			if (type == ProcessType.FireAlarm)
			{
				FireDetectProcess p = new FireDetectProcess();
				return p;
			}
			if (type == ProcessType.DisconnectSensor)
			{
				DisconnectNotifyProcess p = new DisconnectNotifyProcess();
				return p;
			}
			if (type == ProcessType.ConnectSensor)
			{
				ConnectNotifyProcess p = new ConnectNotifyProcess();
				return p;
			}
            if (type == ProcessType.PSMAlarm)
            {
                GasDetectProcess p = new GasDetectProcess();
                return p;
            }
            if( type == ProcessType.SecurityAlarm)
            {
                SecurityAlarmProcess p = new SecurityAlarmProcess();
                return p;
            }

			return null;
		}

		public ProcessIF GetProcess(int nSensorID)
		{
			if (m_dicCurrentDetectProcess.ContainsKey(nSensorID))
				return m_dicCurrentDetectProcess[nSensorID];

			return null;
		}

		public ProcessIF FindProcess(int nSensorHistoryID)
		{
			foreach (KeyValuePair<int, ProcessIF> pair in m_dicCurrentDetectProcess)
			{
				if (pair.Value.SensorHistoryID == nSensorHistoryID)
					return pair.Value;
			}

			return null;
		}


        public List<ProcessIF> GetAllPSMSignalProcess()
        {
            List<ProcessIF> arResult = new List<ProcessIF>();
            foreach (KeyValuePair<int, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                //if (pair.Value.TargetSensor != null && pair.Value.TargetSensor.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    if(pair.Value.ProcessType == ProcessType.PSMAlarm)
                        arResult.Add(pair.Value);
                }
            }
            return arResult;
        }

        public List<ProcessIF> GetAllFireSignalProcess()
        {
            List<ProcessIF> arResult = new List<ProcessIF>();
            foreach (KeyValuePair<int, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                //if (pair.Value.TargetSensor != null && pair.Value.TargetSensor.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    if (pair.Value.ProcessType == ProcessType.FireAlarm)
                        arResult.Add(pair.Value);
                }
            }
            return arResult;
        }

        public List<ProcessIF> GetAllSecurityAlarmProcess()
        {
            List<ProcessIF> arResult = new List<ProcessIF>();
            foreach (KeyValuePair<int, ProcessIF> pair in m_dicCurrentDetectProcess)
            {               
                if (pair.Value.ProcessType == ProcessType.SecurityAlarm)
                    arResult.Add(pair.Value);
            }
            return arResult;
        }
	}
}