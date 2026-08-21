using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using UnE.SOP.Workstate;
using Sections;

namespace UnE
{
    namespace SOP
    {        
        namespace Process
        { 
            public class ProcessSectionManager : IDisposable
            {
                protected static ProcessSectionManager instance = null;
                public static ProcessSectionManager Instance
                {
                    get
                    {
                        if (instance == null)
                        {
                            instance = new ProcessSectionManager();
                            instance.Start();
                        }
                        return instance;
                    }
                }

                private ProcessSectionFactory m_factory = null;
                public ProcessSectionFactory Factory
                {
                    get { return m_factory; }
                    set { m_factory = value; }
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


                private ProcessSectionManager() { }
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
                    mProcessThread.Name = "ProcessQueue";
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
                            foreach (ProcessSectionIF p in mRunQueue)
                            {
                                if (p.State == ProcessSectionState.END)
                                {
                                    if (arDelete == null)
                                        arDelete = new ArrayList();
                                    arDelete.Add(p);
                                }
                            }
                            if (arDelete != null)
                            {
                                foreach (ProcessSectionIF p in arDelete)
                                {
                                    mRunQueue.Remove(p);
                                }
                            }

                            // 대기 Queue에서 Process를 한개 꺼낸다.
                            ProcessSectionIF process = null;
                            lock (mWaitQueue)
                            {
                                process = (ProcessSectionIF)mWaitQueue[0];
                                mWaitQueue.RemoveAt(0);
                            }

                            // Process의 상태 정보를 등록
                            process.State = ProcessSectionState.RUNNING;

                            // Process를 비동기로 런칭
                            process.Thread = new Thread(process.Progress);
                            process.Thread.Name = "Process_" + process.GetType();
                            process.Thread.Start();

                            System.Diagnostics.Trace.WriteLine("Process : " + process.Thread.Name);
                            // 실행 큐에 추가
                            mRunQueue.Add(process);
                        }
                        // 선점 방지 재우기
                        Thread.Sleep(mSleepTime);
                    }
                }

                public void AddFirst(ProcessSectionIF process)
                {
                    if (process == null)
                        return;
                    lock (mWaitQueue)
                    {
                        mWaitQueue.Insert(0, process);
                    }
                }

                public void Add(ProcessSectionIF process)
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
                    foreach (ProcessSectionIF process in mRunQueue)
                    {
                        if (process.State == ProcessSectionState.RUNNING)
                        {
                            try
                            {
                                process.Thread.Interrupt();
                                process.Thread.Abort();
                                process.State = ProcessSectionState.ABORT;
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
}