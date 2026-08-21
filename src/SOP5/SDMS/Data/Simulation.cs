using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using libSensorProcess;

namespace SDMS
{
    public class Simulation
    {
        private class TimerEvent
        {
            public enum EventType { WAIT_FINISH_BROADCAST = 0, CHECK_SYNC, NONE };

            private EventType m_type = EventType.NONE;
            private int m_nData = 0;

            public EventType Type
            {
                get { return m_type; }
                set { m_type = value; }
            }

            public int Data
            {
                get { return m_nData; }
                set { m_nData = value; }
            }

            public TimerEvent()
            {
            }

            public TimerEvent(EventType type, int nData)
            {
                m_type = type;
                m_nData = 0;
            }
        }


        private static Simulation m_instance = null;

        public static Simulation Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Simulation();
                return m_instance; 
            }
        }

        private Timer timerSimulation = new Timer();
        private string m_strSimulationBroadcastResultFilePath = "";

        // 연습모드용 방송이 끝나기를 기다리는 Timer의 최대 대기시간(10분)
        private int m_nSimulationBroadcastTimerWaitTime = 600;


        private Simulation()
        {
            timerSimulation.Interval = 1000;
            timerSimulation.Tag = "";
            timerSimulation.Tick += new System.EventHandler(this.timerSimulation_Tick);
            m_strSimulationBroadcastResultFilePath = Application.StartupPath + "\\FinishSimulationBroadcast.txt";
        }

        public void FireSensorReportClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            object obj = item.Tag;
            if (obj == null)
                return;

            string strFireZoneName = "";

            if (obj.GetType() == typeof(Building))
            {
                Building building = (Building)obj;
                strFireZoneName = building.BroadcastName;
            }
            else
            {
                Zone outerZone = (Zone)obj;

                List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(outerZone);

                if (arrEquipZones != null)
                {
                    foreach (EquipmentZone equipZone in arrEquipZones)
                    {
                        if (SensorManager.Instance.DicSensorZone.ContainsKey(equipZone.ID))
                        {
                            EquipmentZoneObjectList list = SensorManager.Instance.DicSensorZone[equipZone.ID];

                            if (list.SensorList != null && list.SensorList.Count > 0)
                            {
                                if (GetSMSReportConfig())
                                //if (GetSMSConfig())
                                {
                                    // SOPServer를 이용하여 화재탐지 상황을 지정된 담당자들에게 문자로 보낸다.
                                    SendSimulationSMS(FormBroadcastConfig.SituationType.DETECT_FIRE, equipZone.ID);
                                }

                                ReactionLog log = new ReactionLog();
                                log.ID = log.SensorHistoryID = log.GetHashCode();

                                ISensor sensor = (ISensor)list.SensorList[0];
                                ProcessIF process = ProcessManager.Instance.BeginProcess(sensor, log, ProcessType.FireAlarm);
                                ProcessSimulationLog(process, sensor, log);

                                if (process.TargetZone != null)
                                    strFireZoneName = process.TargetZone.DisplayText;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool GetSMSReportConfig()
        {
            return FormSMSConfig.UseSMSOnReportFire;
        }

        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    process.Kill();
                    //break;
                }
            }
        }

        public void RunSimulationTimer(string strSiren, string strServerName, string strPort, string strMessage)
        {
            KillProcess("TTSSimulator");

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.Arguments = strSiren + " 1 " + strServerName + " " + strPort + " " + strMessage + " \"" + m_strSimulationBroadcastResultFilePath + "\"";
            info.CreateNoWindow = true;
            info.FileName = Application.StartupPath + "\\TTSSimulator.exe";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();

            // 연습모드용 방송이 끝나기를 기다리는 Timer 동작
            if (timerSimulation.Tag != null)
            {
                if (timerSimulation.Tag is string)
                    timerSimulation.Tag = null;
                else
                    return;
            }

            if (System.IO.File.Exists(m_strSimulationBroadcastResultFilePath))
                System.IO.File.Delete(m_strSimulationBroadcastResultFilePath);

            timerSimulation.Tag = new TimerEvent(TimerEvent.EventType.WAIT_FINISH_BROADCAST, 0);
            timerSimulation.Start();
        }

        private void timerSimulation_Tick(object sender, EventArgs e)
        {
            if (timerSimulation.Tag == null)
            {
                timerSimulation.Stop();
            }
            else
            {
                if ((timerSimulation.Tag is TimerEvent) == false)
                {
                    timerSimulation.Stop();
                    timerSimulation.Tag = null;
                    return;
                }

                TimerEvent tEvent = (TimerEvent)timerSimulation.Tag;

                if (tEvent.Type == TimerEvent.EventType.WAIT_FINISH_BROADCAST)
                {
                    if (System.IO.File.Exists(m_strSimulationBroadcastResultFilePath))
                    {
                        timerSimulation.Stop();
                        timerSimulation.Tag = null;
                        System.IO.File.Delete(m_strSimulationBroadcastResultFilePath);
                        ResetSimulationAlarmSound();
                    }
                    else
                    {
                        tEvent.Data += 1;

                        // 제한시간이 경과하면 강제로 타이머를 종료시킨다.
                        if (tEvent.Data >= m_nSimulationBroadcastTimerWaitTime)
                        {
                            timerSimulation.Stop();
                            timerSimulation.Tag = null;
                            ResetSimulationAlarmSound();
                        }
                    }
                }
                else if (tEvent.Type == TimerEvent.EventType.CHECK_SYNC)
                {
                    // 동기화 문제로 인하여 이미 종료된 Alarm 신호를 유지하고 있는지 확인한다.
                    ProcessIF process = ProcessManager.Instance.GetProcess(tEvent.Data);

                    if (process == null)
                        FireDetectProcess.SoundPlayer.Stop();

                    timerSimulation.Stop();
                    timerSimulation.Tag = null;
                }
            }
        }

        private void ResetSimulationAlarmSound()
        {
            SeletCaseData data = DlgSelectCase.Instance.CurrentData;

            if (data == null || data.Sensor == null)
                return;

            ProcessIF process = ProcessManager.Instance.GetProcess(data.Sensor.ID);

            if (process != null && data.Sensor.SoundOn)
            {
                FireDetectProcess.PlaySound();

                timerSimulation.Tag = new TimerEvent(TimerEvent.EventType.CHECK_SYNC, data.Sensor.ID);
                timerSimulation.Start();
            }
        }

        // SOPServer를 이용하여 화재탐지 상황을 지정된 담당자들에게 문자로 보낸다.
        public void SendSimulationSMS(FormBroadcastConfig.SituationType type, int nEquipZoneID)
        {
            if (FormManager_Simulation.ManagerPhoneNumbers.Count == 0)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(TrainingSimulatorCommandType.SEND_SDMS_SMS);
            arrDatas.Add((int)type);
            arrDatas.Add(nEquipZoneID);
            arrDatas.Add(FormManager_Simulation.ManagerPhoneNumbers.Count);
            arrDatas.Add("[연습모드]");

            foreach (KeyValuePair<string, string> pair in FormManager_Simulation.ManagerPhoneNumbers)
            {
                arrDatas.Add(pair.Value);
            }

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.TRAINING_SIMULATOR_COMMAND, arrDatas);
            SDMS.NetworkManager.Instance.Send(bytes);
        }

        // strBeginTag와 strEndTag로 둘러쌓인 부분을 제거한 문자열을 리턴한다.
        // strFullMessage : strBeginTag와 strEndTag를 포함한 문자열
        public static string GetMessage(string strOriginMessage, string strBeginTag, string strEndTag, out string strFullMessage)
        {
            int nLen = strOriginMessage.Length;
            int nIndex = 0;

            string strMessage = "";
            strFullMessage = "";
            int nBeginTagLength = strBeginTag.Length;
            int nEndTagLength = strEndTag.Length;

            while (nIndex < nLen)
            {
                int nIndex1 = strOriginMessage.IndexOf(strBeginTag, nIndex);

                if (nIndex1 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex);
                    break;
                }

                int len = nIndex1 - nIndex;

                if (len > 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex, len);
                    strMessage += strOriginMessage.Substring(nIndex, len);
                }

                int nIndex2 = strOriginMessage.IndexOf(strEndTag, nIndex1 + nBeginTagLength);

                if (nIndex2 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex1);
                    break;
                }

                len = nIndex2 - (nIndex1 + nBeginTagLength);

                if (len > 0)
                    strFullMessage += strOriginMessage.Substring(nIndex1 + nBeginTagLength, len);

                nIndex = nIndex2 + nEndTagLength;
            }

            return strMessage;
        }

        public static string GetBroadcastMessage(string strOriginMessage, string strFireZoneName, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            szBroadcastMessage = szBroadcastMessage.Replace("●", strFireZoneName);
            return szBroadcastMessage;
        }

        private void ProcessSimulationLog(ProcessIF process, ISensor sensor, ReactionLog log)
        {
            if (process == null)
                return;

            if (process.GetType() != typeof(FireDetectProcess))
                return;

            log.LogTime = DateTime.Now;
            log.Parameter1 = sensor.EquipZoneID.ToString();
            log.Parameter2 = sensor.ID.ToString();
            log.ReactionType = (int)ReactionType.BEGIN_STATUS;
            log.SensorHistoryID = 0;

            EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);

            if (equipZone != null)
            {
                log.Message = "[" + equipZone.DisplayText + "]에서 화재가 탐지 되었습니다.";
            }

            FireDetectProcess fProcess = (FireDetectProcess)process;
            fProcess.LastLog = log;

            ReactionLogManager.Instance.ProcessLog(log, true);
        }
    }
}
