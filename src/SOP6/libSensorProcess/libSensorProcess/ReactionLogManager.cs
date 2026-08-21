using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace libSensorProcess
{
    public class ReactionLogManager : IDisposable
    {
        private static ReactionLogManager m_Instance = null;

        public static ReactionLogManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ReactionLogManager();
                return m_Instance;
            }
        }

        public Int32 Count
        {
            get
            {
                mMutex.WaitOne();
                Int32 result = m_arLogHistory.Count;
                mMutex.ReleaseMutex();
                return result;
            }
        }



        private ArrayList m_arLogHistory = new ArrayList();

        public System.Collections.ArrayList LogHistory
        {
            get { return (ArrayList)m_arLogHistory.Clone(); }
        }

        private ArrayList m_arLogList = new ArrayList();
        private Mutex mMutex = new Mutex(false);

        protected ReactionLogManager()
        {
        }

        public void Dispose()
        {
            if (mMutex != null)
            {
                mMutex.ReleaseMutex();
                mMutex.Dispose();
            }
        }

        public void ClearLog()
        {
            try
            {
                mMutex.WaitOne();
                m_arLogHistory.Clear();
            }
            finally
            {
                mMutex.ReleaseMutex();
            }
        }

        public void ProcessLog(ReactionLog log, bool bNewLog = false)
        {
            if (log == null || log.SensorHistoryID == -1)
                return;

            //if (log.ReactionType == (int)(ReactionType.MALFUNCTION) ||
            //    (log.ReactionType == (int)(ReactionType.FINISH_SOP)) ||
            //    (log.ReactionType == (int)(ReactionType.IGNORE_SOP)))
            //{
            //    if (bNewLog == true)
            //    {
            //        // 화재 상황 종료
            //        ProcessManager.Instance.ProcessOwner.EndNotifyProcessInvoke(log);
            //        /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //        {
            //            FormMain.Instance.EndNotifyProcess(log);
            //        });*/
            //    }
            //}

            ReactionTypeInfo info = log.GetReactionTypeInfo();

            if (info == ReactionTypeInfo.BEGIN_STATUS || info == ReactionTypeInfo.CHANGE_ALARM_LEVEL)
            {
                ProcessIF process = (ProcessIF)ProcessManager.Instance.FindProcess(log.SensorHistoryID);

                if (process != null)
                {
                    process.SetDetectMode(log, ProcessManager.Instance.ProcessOwner);
                }
            }
            //if (log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS || log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
            //{
            //    ProcessManager.Instance.ProcessOwner.SetPSMDetectModeInvoke(log);
            //    /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //    {
            //        // 누출 탐지 모드
            //        FormMain.Instance.SetPSMDetectMode(log);
            //    });*/
            //}
            //else if (log.ReactionType == (int)ReactionType.END_PSM_STATUS)
            //{
            //    ProcessManager.Instance.ProcessOwner.SetNormalModeInvoke(log);
            //    /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //    {
            //        FormMain.Instance.SetNormalMode(log);
            //    });*/
            //}
            //else if (log.ReactionType == (int)(ReactionType.BEGIN_STATUS))
            //{
            //    ProcessManager.Instance.ProcessOwner.SetFireDetectModeInvoke(log);
            //    /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //    {
            //        // 화재 탐지 모드
            //        FormMain.Instance.SetFireDetectMode(log);
            //    });*/
            //}
            //else if (log.ReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS ||
            //        log.ReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS)
            //{
            //    ProcessManager.Instance.ProcessOwner.SetSecurityDetectModeInvoke(log);
            //    /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //    {
            //        // 방범신호 탐지 모드
            //        FormMain.Instance.SetSecurityDetectMode(log);
            //    });*/
            //}
            else if (info == ReactionTypeInfo.NOTIFY)
            /*else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE ||
                     log.ReactionType == (int)ReactionType.NOTIFY_PSM ||
                     log.ReactionType == (int)ReactionType.NOTIFY_SECURITY
                )*/
            {
                if (bNewLog == true)
                {
                    ProcessManager.Instance.ProcessOwner.NotifyProcessInvoke(log);
                    /*FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                        if (nSensorID != -1)
                        {
                            // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                            ProcessIF process = (ProcessIF)ProcessManager.Instance.GetProcess(nSensorID);
                            FormMain.Instance.BeginNotifyProcess(log);

                            if (process != null)
                                process.Select();

                            FormMain.Instance.SendDetectMessageToSOPSimulator();
                        }
                    });*/
                }
            }
            //else if (log.ReactionType == (int)(ReactionType.TRAINNING_FIRE))
            //{
            //    if (bNewLog == true)
            //    {
            //        ProcessManager.Instance.ProcessOwner.BeginNotifyProcessInvoke(log);
            //        /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //        {
            //            FormMain.Instance.BeginNotifyProcess(log);
            //        });*/
            //    }
            //}
            else if (log.ReactionType == (int)(ReactionType.RUN_SOP))
            {
                ProcessManager.Instance.ProcessOwner.RunSOPInvoke(log);
                /*FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SetRunSOPMode(log);
                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
                });*/
            }
            else if (log.ReactionType == (int)(ReactionType.RUN_N_CANCEL_SOP))
            {
                ProcessManager.Instance.ProcessOwner.RunNCancelSOPInvoke(log);
                /*FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SetRunNCancelSOPMode(log);
                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
                });*/
            }
            else if (log.ReactionType == (int)(ReactionType.FINISH_SOP))
            {
                ProcessManager.Instance.ProcessOwner.FinishSOPInvoke(log);
                /*FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SetFinishSOPMode(log);

                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
                });*/
            }
            else if (log.ReactionType == (int)(ReactionType.IGNORE_SOP))
            {
                ProcessManager.Instance.ProcessOwner.IgnoreSOPInvoke(log);
                /*FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SetIgnoreSOPMode(log);
                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
                });*/
            }
            //else if (log.ReactionType != (int)(ReactionType.END_STATUS) && log.ReactionType != (int)(ReactionType.PSM_USER_RESET))
            //{
            //    ProcessManager.Instance.ProcessOwner.SetNormalModeInvoke(log);
            //    /*FormMain.Instance.Invoke((MethodInvoker)delegate
            //    {
            //        FormMain.Instance.SetNormalMode(log);
            //    });*/
            //}

            if (info != ReactionTypeInfo.END_STATUS && info != ReactionTypeInfo.USER_RESET)
            /*if (log.ReactionType != (int)(ReactionType.END_STATUS) && log.ReactionType != (int)(ReactionType.END_PSM_STATUS)
                && log.ReactionType != (int)(ReactionType.PSM_USER_RESET))*/
            {
                ProcessManager.Instance.ProcessOwner.AddLogMessageInvoke(log);
                /*FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.AddLogMessage(log);
                });*/
            }
        }

        public void AddLog(ReactionLog log)
        {
            try
            {
                mMutex.WaitOne();
                if (!m_arLogHistory.Contains(log))
                {
                    m_arLogHistory.Add(log);
                }
            }
            finally
            {
                mMutex.ReleaseMutex();
            }

            ProcessLog(log, true);

            ProcessIF process = (ProcessIF)ProcessManager.Instance.FindProcess(log.SensorHistoryID);
            if (process != null)
            {
                process.LastLog = log;
            }
        }
    }

    public enum ReactionType
    {
        BEGIN_STATUS = 0,              // 상황 시작
        RUN_BROADCAST = 10,            // 사내 방송 실시         
        SEND_SMS = 11,                 // 문자메시지 발송
        MALFUNCTION = 21,              // 오작동 처리
        NOTIFY_SIGNAL = 22,            // 재난 신고
        IGNORE_SIGNAL = 23,            // 재난 탐지신호 무시
        //TRAINNING_FIRE = 24,         // 
        RUN_SOP = 30,                  // SOP 발동 
        RUN_N_CANCEL_SOP = 31,         // SOP 실행중 취소
        FINISH_SOP = 32,               // SOP 종료
        IGNORE_SOP = 33,               // SOP 실행 안함
        END_STATUS = 50,               // 상황 종료
        //BEGIN_PSM_STATUS = 60,
        //IGNORE_PSM_DETECT = 61,
        CHANGE_ALARM_DEPTH = 62,
        //NOTIFY_PSM = 63,
        USER_RESET = 64,
        //END_PSM_STATUS = 70,
        ETC = 100,                     // 기타
        RUN_DETECT_BROADCAST = 101,
        RUN_REPORT_BROADCAST = 102,
        SEND_DETECT_SMS = 111,
        SEND_REPORT_SMS = 112,
        SEND_MALFUNCTION_SMS = 113,
        SEND_REPAIR_SMS = 114,

        /*NOTIFY_EARTHQUAKE = 200,

        NOTIFY_SECURITY = 898,
        BEGIN_S1SVMS_STATUS = 899,
        IGNORE_S1SVMS_STATUS = 919,
        END_S1SVMS_STATUS = 920,


        BEGIN_S1ACCESS_STATUS = 921,
        IGNORE_S1ACCESS_STATUS = 939,
        END_S1ACCESS_STATUS = 940,


        BEGIN_SECOM_STATUS = 961,
        IGNORE_SECOM_STATUS = 969,
        END_SECOM_STATUS = 970,*/

        TIME_OUT = 1000
    }

    public enum ReactionTypeInfo
    {
        ETC = -1,
        BEGIN_STATUS = 0,   // 신호 탐지
        END_STATUS,         // 신호 종료
        IGNORE,             // 탐지신호 무시
        USER_RESET,         // 오동작 처리 혹은 신호복구
        NOTIFY,             // 재난 신고
        CHANGE_ALARM_LEVEL  // 알람단계 변경
    }

    public class ReactionLog
    {
        private int m_nID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nSensorHistoryID = -1;

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        private int m_nReactionType = 0;

        public int ReactionType
        {
            get { return m_nReactionType; }
            set { m_nReactionType = value; }
        }

        private DateTime m_LogTime;

        public System.DateTime LogTime
        {
            get { return m_LogTime; }
            set { m_LogTime = value; }
        }

        private string m_szMessage = "";

        public string Message
        {
            get { return m_szMessage; }
            set { m_szMessage = value; }
        }

        private string m_szParameter1 = "";

        public string Parameter1
        {
            get { return m_szParameter1; }
            set { m_szParameter1 = value; }
        }

        private string m_nParam2 = "";

        public string Parameter2
        {
            get { return m_nParam2; }
            set { m_nParam2 = value; }
        }

        private string m_szParameter3 = "";

        public string Parameter3
        {
            get { return m_szParameter3; }
            set { m_szParameter3 = value; }
        }

        private string m_nParam4 = "";

        public string Parameter4
        {
            get { return m_nParam4; }
            set { m_nParam4 = value; }
        }

        private string m_nParam5 = "";

        public string Parameter5
        {
            get { return m_nParam5; }
            set { m_nParam5 = value; }
        }

        private static Dictionary<int, libSensorProcess.ReactionType> m_dicReactionType = null;

        public override string ToString()
        {
            return String.Format("{0} {1}", m_LogTime.ToLongTimeString(), m_szMessage);
        }

        public ReactionTypeInfo GetReactionTypeInfo()
        {
            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, libSensorProcess.ReactionType>();

                foreach (libSensorProcess.ReactionType type in Enum.GetValues(typeof(libSensorProcess.ReactionType)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            libSensorProcess.ReactionType rType;

            if (m_dicReactionType.TryGetValue(m_nReactionType, out rType))
                return ToReactionTypeInfo(rType);

            return ReactionTypeInfo.ETC;
        }

        public static ReactionTypeInfo ToReactionTypeInfo(ReactionType type)
        {
            if (type == libSensorProcess.ReactionType.BEGIN_STATUS/* ||
                type == libSensorProcess.ReactionType.BEGIN_PSM_STATUS ||
                type == libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS ||
                type == libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS ||
                type == libSensorProcess.ReactionType.BEGIN_SECOM_STATUS*/)
                return ReactionTypeInfo.BEGIN_STATUS;
            else if (type == libSensorProcess.ReactionType.END_STATUS/* ||
                type == libSensorProcess.ReactionType.END_PSM_STATUS ||
                type == libSensorProcess.ReactionType.END_S1SVMS_STATUS ||
                type == libSensorProcess.ReactionType.END_S1ACCESS_STATUS ||
                type == libSensorProcess.ReactionType.END_SECOM_STATUS*/)
                return ReactionTypeInfo.END_STATUS;
            else if (type == libSensorProcess.ReactionType.IGNORE_SIGNAL/* ||
                type == libSensorProcess.ReactionType.IGNORE_PSM_DETECT ||
                type == libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS ||
                type == libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS ||
                type == libSensorProcess.ReactionType.IGNORE_SECOM_STATUS*/)
                return ReactionTypeInfo.IGNORE;
            else if (type == libSensorProcess.ReactionType.MALFUNCTION ||
                type == libSensorProcess.ReactionType.USER_RESET)
                return ReactionTypeInfo.USER_RESET;
            else if (type == libSensorProcess.ReactionType.CHANGE_ALARM_DEPTH)
                return ReactionTypeInfo.CHANGE_ALARM_LEVEL;
            else if (type == libSensorProcess.ReactionType.NOTIFY_SIGNAL/* ||
                type == libSensorProcess.ReactionType.NOTIFY_PSM ||
                type == libSensorProcess.ReactionType.NOTIFY_SECURITY*/)
                return ReactionTypeInfo.NOTIFY;

            return ReactionTypeInfo.ETC;
        }
    }
}