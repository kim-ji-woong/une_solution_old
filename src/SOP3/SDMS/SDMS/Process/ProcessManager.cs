using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace SDMS
{
    public enum ProcessType
    {
        FireAlarm = 1,
        DisconnectSensor,
		ConnectSensor,

    }

    public interface ProcessIF
    {
        int DetectSensorID
        {
            get;
            set;
        }
        SDMS.SensorZone TargetSensor
        {
            get;
            set;
        }
        SDMS.EquipmentZone TargetZone
        {
            get;
            set;
        }
        int SensorHistoryID
        {
            get;
            set;
        }
        void BeginProcess();
        void AbortProcess();
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
			if (m_dicCurrentDetectProcess.ContainsKey(process.DetectSensorID))
			{
				m_dicCurrentDetectProcess.Remove(process.DetectSensorID);
				System.Diagnostics.Trace.WriteLine("Detect Dictionary Remove");
			}		
		}

		public void RemoveProcess(int nKey)
		{
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
       
        public void BeginProcess(SensorZone sensor, int nSensorHistoryID, ProcessType type, bool bRunProcess = true)
        {
            if (sensor == null)
                return;

			if (type == ProcessType.DisconnectSensor || type == ProcessType.ConnectSensor)
			{
				ProcessIF process = CreateProcess(type);
				if (process == null)
					return;				
				process.DetectSensorID = sensor.ID;
				process.TargetSensor = (SensorZone)sensor;
				process.TargetZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);
				process.SensorHistoryID = nSensorHistoryID;
				if (bRunProcess == true)
					process.BeginProcess();
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
                    process.TargetSensor = (SensorZone)sensor;
                    process.TargetZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);
                    process.SensorHistoryID = nSensorHistoryID;
                    if (bRunProcess == true)
                        process.BeginProcess();
                    
				}
			}           
        }

		public void BeginProcess(SensorZone sensor, Zone zone, int nSensorHistoryID, ProcessType type, bool bRunProcess = true)
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
					process.TargetSensor = (SensorZone)sensor;
					EquipmentZone eqzone = new EquipmentZone();
					eqzone.LinkedZone = zone;
					eqzone.Polygon = zone.Polygon;
					eqzone.ZoneName = zone.BroadcastName;
					process.TargetZone = eqzone;
					process.SensorHistoryID = nSensorHistoryID;

					if (bRunProcess == true)
						process.BeginProcess();
				}
			}
		}



		public void BeginProcess(SensorZone sensor, EquipmentZone zone, int nSensorHistoryID, ProcessType type, bool bRunProcess = true)
		{
			if (sensor == null)
				return;

			if (type == ProcessType.DisconnectSensor || type == ProcessType.ConnectSensor)
			{
				ProcessIF process = CreateProcess(type);
				if (process == null)
					return;
				process.DetectSensorID = sensor.ID;
				process.TargetSensor = (SensorZone)sensor;
				process.TargetZone = zone;
				process.SensorHistoryID = nSensorHistoryID;
				if (bRunProcess == true)
					process.BeginProcess();
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
					process.TargetSensor = (SensorZone)sensor;
					process.TargetZone = zone;
					process.SensorHistoryID = nSensorHistoryID;

					if (bRunProcess == true)
						process.BeginProcess();
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
    }
}
