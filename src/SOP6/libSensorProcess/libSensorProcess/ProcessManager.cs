using System;
using System.Collections.Generic;
using UnE.Spatial;
using UnE.Sensor;

namespace libSensorProcess
{
    public enum ProcessType
    {
        FireAlarm = 1,
        DisconnectSensor,
        ConnectSensor,
        PSMAlarm,
        SecurityAlarm,
        EarthquakeAlarm,
        TemperatureHumidityAlarm,
        PowerOffAlarm,
        DoorAlarm,
        BlackoutAlarm,
        StrongWindAlarm,
        FirewallAlarm,
        TerrorAlarm, // 테러알람
        SubmergencyAlarm, // 침수알람
        CoronaAlarm // 코로나
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

        // 외부 센서신호를 통하여 생성된 Process일 경우 ProcessIF 객체 생성 이후에 ReactionLog 객체를 이용하여 Process 초기화를 한다.
        void InitFromSensor(ReactionLog log);

        // 새로운 신호가 탐지되었음을 ProcessOwner에게 알린다.
        void SetDetectMode(ReactionLog log, IProcessOwner owner);
        void SetAlarmLevel(ReactionLog log);
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

        // Key : Sensor ID(not SensorZone)
        /// <summary>
        /// key
        /// 수동 신고 x : SensorID(not SensorZone)
        /// 수동 신고 o : Zone을 선택한 수동신고 -> Z_ZoneID_FacilityType_SensorID
        ///            : Building을 선택한 수동신고 -> B_BuildingID_FacilityType_SensorID
        ///            : 위치를 선택하지 않은 수동신고 (예:강풍) -> SensorID
        /// </summary>
        private Dictionary<string, ProcessIF> m_dicCurrentDetectProcess = new Dictionary<string, ProcessIF>();
        public Dictionary<string, ProcessIF> CurrentDetectProcess
        {
            get { return m_dicCurrentDetectProcess; }
        }

        private int historyIDToSOPClosing = -1;
        public int HistoryIDToSOPClosing
        {
            set { historyIDToSOPClosing = value; }
            get { return historyIDToSOPClosing; }
        }

        /*** 같은 EquipmentZone의 센서가 이미 Process에 있을때 EquipmentZone이 같은 센서를 대기 시키고 
         * 센서 복구 신호가 같은 Equipzone에 들어오면 Process로 진행하도록 하고.
         * 해당 센서 복구 신호가 올 경우 Remove.
         * 이 딕셔너리는 오직 프로세스가 이미 있을때 신호 복구 및 프로세스 진행을 위함. ****/
        private Dictionary<int, ProcessIF> sleepDetectProcess = new Dictionary<int, ProcessIF>();
        public Dictionary<int, ProcessIF> SleepDetectProcess
        {
            get { return sleepDetectProcess; }
        }


        private IZoneManager m_zoneManager = null;

        public IZoneManager ZoneManager
        {
            get { return m_zoneManager; }
            set { m_zoneManager = value; }
        }

        private IProcessOwner m_processManager = null;

        public IProcessOwner ProcessOwner
        {
            get { return m_processManager; }
            set { m_processManager = value; }
        }

        private ProcessManager()
        {
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
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

        public void RemoveSleepProcess(ProcessIF process)
        {
            if (sleepDetectProcess.ContainsKey(process.DetectSensorID))
            {
                sleepDetectProcess.Remove(process.DetectSensorID);                
            }
        }

        public void RemoveSleepProcess(int nKey)
        {
            if (sleepDetectProcess.ContainsKey(nKey))
            {
                sleepDetectProcess.Remove(nKey);
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

            Zone zone = null;
            if (process.TargetZone != null)
                zone = process.TargetZone.LinkedZone;

            string key = GetDetectKey(process.DetectSensorID, zone);
            if (m_dicCurrentDetectProcess.ContainsKey(key))
            {
                ProcessIF beRemovalProcess = m_dicCurrentDetectProcess[key];
                
                ProcessIF sleepProcess = FindSleepProcessByEquipmentID(process.TargetSensor.EquipZoneID);
                if (sleepProcess != null && sleepProcess.DetectSensorID != process.DetectSensorID)
                {                    
                    sleepProcess.ReadyProcess();
                    sleepProcess.BeginProcess();
                    HistoryIDToSOPClosing = sleepProcess.SensorHistoryID;
                    m_dicCurrentDetectProcess.Add(key, sleepProcess);
                    sleepDetectProcess.Remove(sleepProcess.DetectSensorID);
                }
                else
                {
                    HistoryIDToSOPClosing = -1;
                }
                m_dicCurrentDetectProcess.Remove(key);                
            }
        }

        //public void RemoveProcess(int nKey)
        //{
        //    if (FireDetectProcess.SoundPlayer.SoundLocation != null)
        //        FireDetectProcess.SoundPlayer.Stop();

        //    if (m_dicCurrentDetectProcess.ContainsKey(nKey))
        //    {
        //        ProcessIF process = m_dicCurrentDetectProcess[nKey];                                

        //        ProcessIF sleepProcess = FindSleepProcessByEquipmentID(process.TargetSensor.EquipZoneID);
        //        if (sleepProcess != null)
        //        {                   
                   
        //            sleepProcess.ReadyProcess();
        //            sleepProcess.BeginProcess();
        //            HistoryIDToSOPClosing = sleepProcess.SensorHistoryID;

        //            string key = GetDetectKey(sleepProcess.DetectSensorID, sleepProcess.TargetZone.LinkedZone);

        //            m_dicCurrentDetectProcess.Add(key, sleepProcess);
        //            sleepDetectProcess.Remove(sleepProcess.DetectSensorID);
        //        }
        //        else
        //        {
        //            HistoryIDToSOPClosing = -1;
        //        }
        //        m_dicCurrentDetectProcess.Remove(nKey);        
        //    }                       
        //}

        public ProcessIF FindSleepProcessByEquipmentID(int equipzoneID)
        {
            foreach (KeyValuePair<int, ProcessIF> pair in SleepDetectProcess)
            {
                if (pair.Value.TargetSensor.EquipZoneID == equipzoneID)
                    return pair.Value;
            }
            return null;
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
        /***  같은 EquipmentZone 센서 처리 : 대기 프로세스를 만들기 위한 메서드 ***/
        public ProcessIF MakeSleepProcess(ISensor sensor, ReactionLog log, ProcessType type, bool bRunProcess = true)
        {
            if (sensor == null)
                return null;

            ProcessIF process = null;
            if (type == ProcessType.DisconnectSensor || type == ProcessType.ConnectSensor)
            {
                process = CreateProcess(type);
                if (process == null)
                    return null;

                process.DetectTime = log.LogTime;
                process.DetectSensorID = sensor.ID;
                process.TargetSensor = (ISensor)sensor;
                process.TargetZone = ZoneManager.GetEquipZone(sensor.EquipZoneID);
                process.LastLog = log;
                process.SensorHistoryID = log == null ? -1 : log.SensorHistoryID;

                process.InitFromSensor(log);
            }
            else
            {
                if (!sleepDetectProcess.ContainsKey(sensor.ID))
                {
                    process = CreateProcess(type);
                    if (process == null)
                        return null;

                    sleepDetectProcess.Add(sensor.ID, process);
                    process.LastLog = log;
                    process.DetectTime = log.LogTime;
                    process.DetectSensorID = sensor.ID;
                    process.TargetSensor = (ISensor)sensor;
                    process.TargetZone = ZoneManager.GetEquipZone(sensor.EquipZoneID);
                    process.SensorHistoryID = log == null ? -1 : log.SensorHistoryID;

                    process.InitFromSensor(log);
                }
            }

            return process;
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

                process.DetectTime = log.LogTime;
                process.DetectSensorID = sensor.ID;
                process.TargetSensor = (ISensor)sensor;
                process.TargetZone = ZoneManager.GetEquipZone(sensor.EquipZoneID);
                process.LastLog = log;
                process.SensorHistoryID = log == null ? -1 : log.SensorHistoryID;

                process.InitFromSensor(log);

                if (bRunProcess == true)
                {
                    process.BeginProcess();
                    // SOP Loading
                    if (!ProcessOwner.UsePopupSensorOn)
                    {
                        // SOP Simulator가 분리되어 있으므로 굳이 null 검사를 할 필요가 없다.
                        // [2018/01/04] 김지웅
                        //UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                        //if (container != null)
                        {
                            DateTime sopTime = log == null ? DateTime.Now : log.LogTime;

                            // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                            // skkim 2017-04-11
                            // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                            if (ProcessOwner.OpenSOPOnDetectSensor == true)
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                                ProcessOwner.OpenSOP(process.TargetZone, sopTime, process);
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
                    if (!ProcessOwner.UsePopupSensorOn)
                    {
                        // SOP Simulator가 분리되어 있으므로 굳이 null 검사를 할 필요가 없다.
                        // [2018/01/04] 김지웅
                        //UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                        //if (container != null)
                        {
                            DateTime sopTime = log == null ? DateTime.Now : log.LogTime;
                            // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                            // skkim 2017-04-11
                            // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                            if (ProcessOwner.OpenSOPOnDetectSensor == true)
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                                ProcessOwner.OpenSOP(process.TargetZone, sopTime, process);
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
                Zone zone = null;
                EquipmentZone eqZone = ZoneManager.GetEquipZone(sensor.EquipZoneID);
                if (eqZone != null && eqZone.LinkedZone != null)
                {
                    zone = ZoneManager.GetZone(eqZone.LinkedZone.ID);
                }
                
                string key = GetDetectKey(sensor.ID, zone);

                if (!m_dicCurrentDetectProcess.ContainsKey(key))
                {
                    process = CreateProcess(type);
                    if (process == null)
                        return null;

                    m_dicCurrentDetectProcess.Add(key, process);
                    process.LastLog = log;
                    process.DetectTime = log.LogTime;
                    process.DetectSensorID = sensor.ID;
                    process.TargetSensor = (ISensor)sensor;
                    process.TargetZone = ZoneManager.GetEquipZone(sensor.EquipZoneID);
                    process.SensorHistoryID = log == null ? -1 : log.SensorHistoryID;

                    process.InitFromSensor(log);
                    
                    if (bRunProcess == true)
                    {
                        process.BeginProcess();
                        // SOP Loading
                        if (!ProcessOwner.UsePopupSensorOn)
                        {
                            DateTime sopTime = log == null ? DateTime.Now : log.LogTime;
                            if (ProcessOwner.OpenSOPOnDetectSensor == true)
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                                ProcessOwner.OpenSOP(process.TargetZone, sopTime, process);
                            }
                            else
                            {
                                process.DetectTime = sopTime;
                                process.ShowOpenSOP = true;
                            }
                        }
                    }
                    else
                    {
                        if (!ProcessOwner.UsePopupSensorOn)
                        {
                            DateTime sopTime = log == null ? DateTime.Now : log.LogTime;
                            if (ProcessOwner.OpenSOPOnDetectSensor == true)
                            {
                                process.ShowOpenSOP = true;
                                process.DetectTime = sopTime;
                                ProcessOwner.OpenSOP(process.TargetZone, sopTime, process);
                            }
                            else
                            {
                                process.DetectTime = sopTime;
                                process.ShowOpenSOP = true;
                            }
                        }

                        process.ReadyProcess();
                    }
                }
            }

            return process;
        }

        /// <summary>
        /// 수동신고의 경우 ReactionLog가 없이 가상 센서로 생성한다.
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
                string key = GetDetectKey(sensor.ID, zone);
                if (!m_dicCurrentDetectProcess.ContainsKey(key))
                {
                    ProcessIF process = CreateProcess(type);
                    if (process == null)
                        return;

                    m_dicCurrentDetectProcess.Add(key, process);

                    process.DetectSensorID = sensor.ID;
                    process.TargetSensor = (ISensor)sensor;
                    if (zone != null)
                    {
                        EquipmentZone eqzone = new EquipmentZone();
                    
                        eqzone.LinkedZone = zone;
                        eqzone.Polygon = zone.Polygon;
                        eqzone.ZoneName = zone.DisplayText;
                        process.TargetZone = eqzone;
                    }
                    
                    process.SensorHistoryID = nSensorHistoryID;

                    if (bRunProcess == true)
                    {
                        process.BeginProcess();

                        // SOP Loading
                        if (!ProcessOwner.UsePopupSensorOn)
                        {
                            DateTime sopTime = DateTime.Now;

                            // SOP Simulator가 분리되어 있으므로 굳이 null 검사를 할 필요가 없다.
                            // [2018/01/04] 김지웅
                            //UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                            //if (container != null)
                            {
                                // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                                // skkim 2017-04-11
                                // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                if (ProcessOwner.OpenSOPOnDetectSensor == true)
                                {
                                    process.ShowOpenSOP = true;
                                    process.DetectTime = sopTime;
                                    ProcessOwner.OpenSOP(process.TargetZone, sopTime, process);
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
                        if (!ProcessOwner.UsePopupSensorOn)
                        {
                            DateTime sopTime = DateTime.Now;

                            // SOP Simulator가 분리되어 있으므로 굳이 null 검사를 할 필요가 없다.
                            // [2018/01/04] 김지웅
                            //UnE.SOP.Workstate.IWorkflowContainer container = UnE.SOP.ProxySOP.Instance.WorkflowContainer;
                            //if (container != null)
                            {
                                // 서울대 관련 작업중 SOP가 실행되고 있어도 새로운 SOP가 열리도록 수정함(kjw요청)
                                // skkim 2017-04-11
                                // if( container.GetCurrentSOPScenario() == null && UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == true)
                                if (ProcessOwner.OpenSOPOnDetectSensor == true)
                                {
                                    process.ShowOpenSOP = true;
                                    process.DetectTime = sopTime;
                                    ProcessOwner.OpenSOP(process.TargetZone, sopTime, process);
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
        public ProcessIF CreateSleepProcess(ProcessType type)
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
            if (type == ProcessType.SecurityAlarm)
            {
                SecurityAlarmProcess p = new SecurityAlarmProcess();
                return p;
            }

            return null;
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
            if (type == ProcessType.SecurityAlarm)
            {
                SecurityAlarmProcess p = new SecurityAlarmProcess();
                return p;
            }
            if (type == ProcessType.EarthquakeAlarm)
            {
                EarthquakeAlarmProcess p = new EarthquakeAlarmProcess();
                return p;
            }
            if (type == ProcessType.TemperatureHumidityAlarm)
            {
                TemperatureHumidityAlarmProcess p = new TemperatureHumidityAlarmProcess();
                return p;
            }
            if (type == ProcessType.TerrorAlarm || type == ProcessType.StrongWindAlarm || type == ProcessType.SubmergencyAlarm ||
                type == ProcessType.BlackoutAlarm || type == ProcessType.DoorAlarm || type == ProcessType.FirewallAlarm || type == ProcessType.CoronaAlarm)
            {
                EtcProcess p = new EtcProcess(type);
                return p;
            }

            return null;
        }

        public ProcessIF GetProcess(int nSensorID)
        {
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                string key = "";

                int result = -1;
                if (int.TryParse(pair.Key, out result))
                {
                    if (result != nSensorID)
                        continue;
                }
                else
                {
                    string[] resultArr = pair.Key.Split('_');
                    if (resultArr.Length < 4)
                        continue;

                    if (Convert.ToInt32(resultArr[3]) != nSensorID)
                        continue;
                }

                return pair.Value;
            }
            
            return null;
        }
        public ProcessIF GetSleepProcess(int nSensorZoneID)
        {
            if (sleepDetectProcess.ContainsKey(nSensorZoneID))
                return sleepDetectProcess[nSensorZoneID];

            return null;
        }

        public ProcessIF FindSleepProcess(int nSensorHistoryID)
        {
            foreach (KeyValuePair<int, ProcessIF> pair in sleepDetectProcess)
            {
                if (pair.Value.SensorHistoryID == nSensorHistoryID)
                    return pair.Value;
            }

            return null;
        }
        public ProcessIF FindProcess(int nSensorHistoryID)
        {
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                if (pair.Value.SensorHistoryID == nSensorHistoryID)
                    return pair.Value;
            }

            return null;
        }
        /**
         * 같은 EquipmentZone 인 프로세스가 있을 때 리턴해주기 위해 작성. 
         * 2018.7.16 by hypark
         */
        public ProcessIF FindSameEquipmentZoneProcess(int equipmentZoneID)
        {
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                if (pair.Value.TargetSensor.EquipZoneID == equipmentZoneID)
                    return pair.Value;
            }

            return null;
        }
        

        public List<ProcessIF> GetAllPSMSignalProcess()
        {
            List<ProcessIF> arResult = new List<ProcessIF>();
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                //if (pair.Value.TargetSensor != null && pair.Value.TargetSensor.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    if (pair.Value.ProcessType == ProcessType.PSMAlarm)
                        arResult.Add(pair.Value);
                }
            }
            return arResult;
        }

        public List<ProcessIF> GetAllFireSignalProcess()
        {
            List<ProcessIF> arResult = new List<ProcessIF>();
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
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
            foreach (KeyValuePair<string, ProcessIF> pair in m_dicCurrentDetectProcess)
            {
                if (pair.Value.ProcessType == ProcessType.SecurityAlarm)
                    arResult.Add(pair.Value);
            }
            return arResult;
        }

        public static string EnginPath()
        {
            string szMainPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (System.IO.File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (System.IO.File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (System.IO.File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            return szMainPath;
        }

        static public void PlaySound()
        {
            FireDetectProcess.PlaySound();
        }

        public string GetDetectKey(int sensorID, Zone zone)
        {
            string key = "";
            // 수동 신고
            if (sensorID >= SOPWebServer.Header.ManualReportDefaultID)
            {
                // 건물 선택이 필요없는 재난 ex)강풍
                if (zone == null)
                {
                    key = sensorID.ToString();
                }
                else
                {
                    int nfacilityType = sensorID - SOPWebServer.Header.ManualReportDefaultID;
                    key = "Z_" + zone.ID + nfacilityType + "_" +"_" + sensorID;
                }
            }
            else
                key = sensorID.ToString();

            return key;
        }
    }
}