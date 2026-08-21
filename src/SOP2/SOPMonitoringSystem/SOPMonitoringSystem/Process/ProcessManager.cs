using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Sections;

namespace SOPMonitoringSystem
{
    public interface AnnounceMessage
    {
        string Message
        {
            get;
            set;
        }
        int Count
        {
            get;
            set;
        }
        bool UseSenarioMessage
        {
            get;
            set;
        }

        bool UseSystemMessage
        {
            get;
            set;
        }
        string SystemMessage
        {
            get;
            set;
        }
        string SenarioMessage
        {
            get;
            set;
        }
        bool UseSiren
        {
            get;
            set;
        }
    }

    namespace Process
    {
        public enum ProcessState
        {
            STANDBY = 1,
            RUNNING = 2,
            ABORT = 3,
            END = 4,
            ERROR = 5
        }

        public class ProcessEventArgs
        {
        }

        public delegate void PreProcessEvent(object sender, ProcessEventArgs e);
        public delegate void PostProcessEvent(object sender, ProcessEventArgs e);

        public abstract class ProcessIF
        {
            public virtual event PreProcessEvent OnPreProcess;
            public virtual event PostProcessEvent OnPostProcess;
            protected ProcessState nState = ProcessState.STANDBY;
            public ProcessState State
            {
                get { return nState; }
                set { nState = value; }
            }
            protected Thread mThread = null;
            public System.Threading.Thread Thread
            {
                get { return mThread; }
                set { mThread = value; }
            }
            public virtual void Progress() { }

            public virtual void Dispose() {}

            protected void SendMessage(ArrayList callList, string szCaller, string szMessage)
            {
                if (FormMain.Instance.SMSOn == false)
                    return;

                if (callList == null || callList.Count == 0)
                    return;
                string message = szMessage;

                if (FormMain.Instance.UseEzSMS == false)
                {
                    //FormMain.Instance.DBManager.SendSMS(callList, szCaller, szMessage);
                    SMSManager.Instance.SendSMS(callList, szCaller, szMessage);
                }
                else
                {
                    try
                    {
                        ArrayList list = new ArrayList();
                        foreach (string callNumber in callList)
                        {
                            if (callNumber == null)
                                continue;

                            if (callNumber == "" || callNumber == "0" || callNumber == "0000000")
                                continue;

                            list.Add(callNumber);
                        }
                        //list.Clear();
                       // list.Add("01043632290");

						//if (list.Count > 0)
						//{
						//    ezSMSComponent.ISMS m_sms = new ezSMSComponent.SMS();
						//    m_sms.ServiceCode = "020026C9FCC7C39E41A88C2CF52D00D7BAA6";
						//    ezSMSComponent.LoginInfo login = m_sms.Login("121.254.175.25", 4545, "unes", "unes0101");
						//    ezSMSComponent.Receivers receiver = m_sms.CreateReceivers();

						//    foreach (string callNumber in list)
						//    {
						//        receiver.AddDirect(callNumber, message, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
						//    }

						//    ezSMSComponent.SendResults results = m_sms.SendSMS(szCaller, receiver);
						//    foreach (ezSMSComponent.SendResult result in results)
						//    {
						//        if (result.Result != ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
						//        {
						//        }
						//    }
						//}
                    }
                    catch (Exception)
                    {
                    }
                }       
            }

            // strMsg에 {time}, {location}과 같은 특수 문자열이 존재하면
            // 해당 내용을 실제 시간과 장소로 바꾸어준다.
            public string ParseSpecialMessage(string strMsg, DateTime dtTime, string strPlace, bool isRealMode, bool isNormalMode)
            {
                return UnE.Utility.SOPSimulatorScript.Parse(strMsg, dtTime, strPlace, isRealMode ? 1 : 0, isNormalMode ? 1 : 0);
            }
        }

        public class ProcessFactory
        {
            public static ProcessIF CreateProcess(Sections.SectionState state)
            {
                if (state == null)
                    return null;
                
                if (state.Section.GetComponentType() == Section.ComponentType.INTERNAL)
                {
                    return new InternalNotifyProcess(state);
                }
                else if (state.Section.GetComponentType() == Section.ComponentType.EXTERNAL)
                {
                    return new ExternalNotifyProcess(state);
                }
                else if (state.Section.GetComponentType() == Section.ComponentType.TRANSMISSION)
                {
                    return new TransmissionNotifyProcess(state); 
                }
                else if (state.Section.GetComponentType() == Section.ComponentType.PROCESS)
                {
                    return new TaskNotifyProcess(state);
                }

                return null;
            }
        }
        
        public class ProcessManager : IDisposable
        {
            protected static ProcessManager instance = null;
            public static ProcessManager Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new ProcessManager();
                        instance.Start();
                    }
                    return instance;
                }
            }
            

            private ArrayList mWaitQueue = new ArrayList();
            private ArrayList mRunQueue = new ArrayList();
            private Thread mProcessThread = null;
            private int mSleepTime = 300;
            public int SleepTime
            {
                get { return mSleepTime; }
                set { mSleepTime = value; }
            }
            private bool mbProcess = true;         

           
            private ProcessManager() { }
            public void Dispose()
            {
                Stop();
                AbortProcessAll();
            }

            public bool Progress
            {
                get { return mbProcess; }
                set
                {
                    mbProcess = value;
                    if (mbProcess == false)
                    {
                        mSleepTime = -1;
                    }
                    else
                    {
                        mSleepTime = 300;
                    }
                }
            }           

            private void Start()
            {
                mProcessThread = new Thread(ProcessQueue);
                mProcessThread.Start();
            }

            private void Stop()
            {
                mbProcess = false;
                mProcessThread.Join();
            }

            private void ProcessQueue()
            {
                while (mbProcess == true)
                {
                    // 대기 큐를 확인
                    if (mWaitQueue.Count > 0)
                    {
                        // 실행큐를 검사
                        ArrayList arDelete = null;
                        foreach (ProcessIF p in mRunQueue)
                        {
                            if (p.State == ProcessState.END)
                            {
                                if (arDelete == null)
                                    arDelete = new ArrayList();
                                arDelete.Add(p);
                            }
                        }
                        if (arDelete != null)
                        {
                            foreach (ProcessIF p in arDelete)
                            {
                                mRunQueue.Remove(p);
                            }
                        }
                        
                        // 대기 Queue에서 Process를 한개 꺼낸다.
                        ProcessIF process = null;
                        lock (mWaitQueue)
                        {
                            process = (ProcessIF)mWaitQueue[0];
                            mWaitQueue.RemoveAt(0);
                        }
                        
                        // Process의 상태 정보를 등록
                        process.State = ProcessState.RUNNING;

                        // Process를 비동기로 런칭
                        process.Thread = new Thread(process.Progress);
                        process.Thread.Start();
                        // 실행 큐에 추가
                        mRunQueue.Add(process);
                    }                    
                    // 선점 방지 재우기
                    Thread.Sleep(mSleepTime);
                }
            }

            public void AddFirst(ProcessIF process)
            {
                if (process == null)
                    return;
                lock (mWaitQueue)
                {
                    mWaitQueue.Insert(0, process);
                } 
            }

            public void Add(ProcessIF process)
            {
                if (process == null)
                    return;
                lock (mWaitQueue)
                {
                    mWaitQueue.Add(process);
                }                
            }

            public void AbortProcessAll()
            {
                foreach (ProcessIF process in mRunQueue)
                {
                    if (process.State == ProcessState.RUNNING)
                    {
                        try
                        {                            
                            process.Thread.Interrupt();
                            process.Thread.Abort();
                            process.State = ProcessState.ABORT;
                        }
                        catch (Exception)
                        {

                        }
                        
                    }
                }
            }
        }
    }    
}
