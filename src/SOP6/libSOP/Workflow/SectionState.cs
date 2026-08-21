using DBUtility2;
using Sections;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnE.SOP.Process;

namespace UnE
{
    namespace SOP
    {
        using History;

        namespace Workstate
        {
            public enum State
            {
                NORMAL = 1,
                RUN = 2,
                DONE = 3,
                INPUT = 4,
                SKIP = 5
            }

            public enum ProcessDirectionHistory
            {
                NONE = 0,
                TOP = 1,
                RIGHT = 2,
                BOTTOM = 4,
                LEFT = 8
            }            

            public class SectionState
            {
				protected Color NoramlColor = Color.FromArgb(210, 210, 210);
				protected Color WaitColor = Color.FromArgb(210, 210, 210);

                protected Color CompleteColor = Color.FromArgb(252, 213, 181);
                protected Color InProgressColor = Color.FromArgb(142, 180, 227);
                protected Color InputWaitColor = Color.FromArgb(255, 174, 201);
                protected Color SkipColor = Color.FromArgb(255, 233, 127);
                protected Color TeamCompleteColor = Color.FromArgb(128, 128, 128);
                protected Color TeamNormalColor = Color.Aqua;
                protected Color Current;

                protected DateTime? m_beginTime = null;

                protected static IHistoryManager m_historyManager = null;

                public static IHistoryManager HistoryManager
                {
                    get { return m_historyManager; }
                    set { m_historyManager = value; }
                }

                // 동일 Object인지 검사용
                public int HashCode
                {
                    get { return this.GetHashCode(); }
                }

                protected int m_nCheckedNotify1 = 0;
                public int CheckNotify1
                {
                    get { return m_nCheckedNotify1; }
                    set { m_nCheckedNotify1 = value; }
                }

                protected int m_nCheckedNotify2 = 0;
                public int CheckNotify2
                {
                    get { return m_nCheckedNotify2; }
                    set { m_nCheckedNotify2 = value; }
                }

                protected int m_nCheckedRun = 0;
                public int CheckedRun
                {
                    get { return m_nCheckedRun; }
                    set { m_nCheckedRun = value; }
                }

                protected int m_nCheckedComplete = 0;
                public int CheckedComplete
                {
                    get { return m_nCheckedComplete; }
                    set { m_nCheckedComplete = value; }
                }

                protected int m_nCompleteCount = 0;
                public int CompleteCount
                {
                    get { return m_nCompleteCount; }
                    set { m_nCompleteCount = value; }
                }

                protected bool mbEnd = false;
                public bool EndState
                {
                    get { return mbEnd; }
                    set { mbEnd = value; }
                }

                protected bool mbBegin = false;
                public bool BeginState
                {
                    get { return mbBegin; }
                    set { mbBegin = value; }
                }

                // SectionState를 변경한 SOPGenUser의 ID
                protected int m_nAccessedUserID = -1;
                public int AccessedUserID
                {
                    get { return m_nAccessedUserID; }
                    set { m_nAccessedUserID = value; }
                }

                public DateTime? BeginTime
                {
                    get { return m_beginTime; }
                    set { m_beginTime = value; }
                }

                // Key : ComponentHistoryID
                protected Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> m_dicDetailDatas = new Dictionary<int, List<History.HistorySectionData.DetailData>>();
                public Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> DetailDatas
                {
                    get { return m_dicDetailDatas; }
                }

                protected WorkFlow mParent = null;
                public event ChangeStateEvent OnChange;
                public event PostChangeEvent OnPostChange;
                public WorkFlow Parent
                {
                    get { return mParent; }
                }

                protected Section mSection = null;
                public Section Section
                {
                    get { return mSection; }
                }
                protected State mState = State.NORMAL;
                public State State
                {
                    get { return mState; }
                    set
                    {
                        mState = value;

                        if (m_contents != null)
                            m_contents.State = mState;

                        if (m_beginTime == null)
                            m_beginTime = DateTime.Now;
                    }
                }

                private UnE.SOP.Sections.ISectionContents m_contents = null;
                public UnE.SOP.Sections.ISectionContents SectionContents
                {
                    get { return m_contents; }
                    set { m_contents = value; }
                }

                private bool m_isSelected = false;
                public bool IsSelected
                {
                    get { return m_isSelected; }
                    set
                    {
                        m_isSelected = value;

                        if (m_contents != null)
                        {
                            m_contents.IsSelected = m_isSelected;
                        }

                        if (mSection != null && m_isSelected)
                        {
                            SetColor(mSection, WorkFlowManager.Instance.CurrentColor, GetShapeStatus(mState));
                        }
                    }
                }

                // ProcessDirection에 대한 비트 Flag 조합
                protected int mProcessDirections = (int)ProcessDirectionHistory.NONE;
                public int ProcessDirections
                {
                    get { return mProcessDirections; }
                    set { mProcessDirections = value; }
                }

                // SectionState가 변경된 시간
                protected VariousData<DateTime> m_time = null;
                public VariousData<DateTime> Time
                {
                    get { return m_time; }
                    set { m_time = value; }
                }

                public void InitState()
                {
                    mState = State.NORMAL;
                    NoramlColor = WorkFlowManager.Instance.NoramlColor;
                    InputWaitColor = WorkFlowManager.Instance.WaitColor;
                    CompleteColor = WorkFlowManager.Instance.CompleteColor;
                    InProgressColor = WorkFlowManager.Instance.InProgressColor;
                    InputWaitColor = WorkFlowManager.Instance.InputWaitColor;
                    SkipColor = WorkFlowManager.Instance.SkipColor;
                    TeamCompleteColor = WorkFlowManager.Instance.TeamCompleteColor;
                    TeamNormalColor = WorkFlowManager.Instance.TeamNormalColor;
                    ClearNotify();
                    //SetColor(SelectSection, NoramlColor);
                    mSection.Data.AggSection = null;
                    m_nCompleteCount = 0;

                    SetInitStateColor();

                    // ProcessButton 초기화
                    if (mSection != null && mSection.GetSectionPainter(0) != null)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)mSection.GetSectionPainter(0);
                        mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT);
                    }

                    if (mSection != null && mSection.GetSectionPainter(1) != null)
                    {
                        ConfirmButtonManager mgr = (ConfirmButtonManager)mSection.GetSectionPainter(1);
                        mgr.SetAllButtonsStatus(ConfirmButton.ButtonStatus.WAIT);
                    }
                }

                public virtual void SetInitStateColor()
                {
                    if (mSection != null)
                    {
                        SetColor(mSection, WorkFlowManager.Instance.NoramlColor, Shape.ShapeStatus.NORMAL);
                    }
                }

                protected bool mbNotify = false;
                public bool MbNotify
                {
                    get { return mbNotify; }
                    set { mbNotify = value; }
                }

                Shape.ShapeStatus GetShapeStatus(State state)
                {
                    if (state == State.NORMAL)
                        return Shape.ShapeStatus.NORMAL;
                    else if (state == State.INPUT)
                        return Shape.ShapeStatus.WAITING;
                    else if (state == State.DONE)
                        return Shape.ShapeStatus.PROCESSED;
                    else if (state == State.RUN)
                        return Shape.ShapeStatus.PROCESSING;
                    else if (state == State.SKIP)
                        return Shape.ShapeStatus.SKIPPED;

                    return Shape.ShapeStatus.NORMAL;
                }

                public void CopyState(SectionState state)
                {
                    mState = state.mState;
                    mbNotify = state.mbNotify;
                    Current = state.Current;
                    SetColor(mSection, Current, GetShapeStatus(mState));
                    if (mState == State.DONE)
                    {
                        SetProcessUnderColor(mSection, TeamCompleteColor);
                    }
                    else
                    {
                        SetProcessUnderColor(mSection, TeamNormalColor);
                    }

                    mSection.Notify(mbNotify);
                }

                public void CopyState(State state, Color colorCurrent)
                {
                    mState = state;
                    mbNotify = state == State.INPUT;
                    Current = colorCurrent;
                    SetColor(mSection, Current, GetShapeStatus(mState));
                    if (mState == State.DONE)
                    {
                        SetProcessUnderColor(mSection, TeamCompleteColor);
                    }
                    else
                    {
                        SetProcessUnderColor(mSection, TeamNormalColor);
                    }
                    mSection.Notify(mbNotify);
                }

                public SectionState(WorkFlow parent, Section section)
                {
                    mParent = parent;
                    mSection = section;
                    mState = State.NORMAL;

                    NoramlColor = WorkFlowManager.Instance.NoramlColor;
                    InputWaitColor = WorkFlowManager.Instance.WaitColor;
                    CompleteColor = WorkFlowManager.Instance.CompleteColor;
                    InProgressColor = WorkFlowManager.Instance.InProgressColor;
                    InputWaitColor = WorkFlowManager.Instance.InputWaitColor;
                    SkipColor = WorkFlowManager.Instance.SkipColor;
                    TeamCompleteColor = WorkFlowManager.Instance.TeamCompleteColor;
                    TeamNormalColor = WorkFlowManager.Instance.TeamNormalColor;

                }

                public void ClearNotify()
                {
                    mbNotify = false;
                    mSection.Notify(mbNotify);
                }

                public void SetProcessUnderColor(Section section, Color color)
                {
                    Section.ComponentType type = mSection.GetComponentType();
                    if (type == Section.ComponentType.PROCESS)
                    {
                        SectionProcess psection = (SectionProcess)section;
                        psection.SetFillColor(color, false);
                        Current = color;
						//section.SetColor(Section.ColorTarget.FILL, color);
					}
                }

                public void SetColor(Section section, Color color, Shape.ShapeStatus status)
                {
                    Section.ComponentType type = section.GetComponentType();
                    //Section.ComponentType type = mSection.GetComponentType();
                    if (type == Section.ComponentType.PROCESS)
                    {
                        SectionProcess psection = (SectionProcess)section;
                        psection.SetFillColor(color, true);
						section.SetColor(Section.ColorTarget.FILL, color);
                        Current = color;
                    }
                    else
                    {
                        Current = color;
                        section.SetColor(Section.ColorTarget.FILL, color);
                    }

                    section.Shape.Status = status;
                }


                public virtual void Complete(int nProcessDirections = (int)ProcessDirectionHistory.NONE)
                {
                    //if (mState != State.DONE)
                    {
                        this.ProcessDirections |= nProcessDirections;

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.DONE);
                        if (OnChange(this, args))
                        {
                            m_nCompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
                            //m_nCompleteCount++;
                            SetColor(Section, CompleteColor, Shape.ShapeStatus.PROCESSED);
                            SetProcessUnderColor(Section, TeamCompleteColor);
                            mState = State.DONE;
                            mProcessDirections = nProcessDirections;
                            mbNotify = false;
                            mSection.Notify(mbNotify);

                            // 종료 Section은 이후에 SOP 종료 이벤트가 발생하므로 OnPostChange(...)를 호출하지 않아도 된다.
                            //if (!IsEndSection())
                                OnPostChange(this, args);
                        }
                    }
                    /*else
                    {
                        this.ProcessDirections |= nProcessDirections;

                        // SelectSection은 이미 완료되었으므로 다음 Section에 대한 상태만 변화시킨다.
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.DONE);
                        OnChange(this, args);
                    }*/
                }

                private bool IsEndSection()
                {
                    if (this.Section == null)
                        return false;

                    if (this.Section.GetComponentType() == global::Sections.Section.ComponentType.ENDPOINT)
                    {
                        global::Sections.SectionEndPoint section = (global::Sections.SectionEndPoint)this.Section;
                        global::Sections.SectionDataEndPoint data = (global::Sections.SectionDataEndPoint)section.Data;

                        return !data.IsBegin;
                    }

                    return false;
                }

                public virtual void Wait()
                {
                    if (mState != State.NORMAL)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.NORMAL);
                        //if (OnChange(this, args))
                        //{
                        SetColor(Section, NoramlColor, Shape.ShapeStatus.NORMAL);
                        SetProcessUnderColor(Section, TeamNormalColor);
                        mState = State.NORMAL;
                        mbNotify = false;
                        mSection.Notify(mbNotify);
                        OnPostChange(this, args);
                        //}
                    }

                }

                public virtual void InProgress()
                {
                    if (BeginTime == null)
                    {
                        BeginTime = DateTime.Now;
                        m_historyManager.AddSectionHistory(this.Section, this, mState, -1, false, 0, 0, 0, 0, new Dictionary<int, List<History.HistorySectionData.DetailData>>());
                    }

                    if (mState != State.RUN)
                    {
                        State s = State.RUN;
                        Color color = InProgressColor;
                        Shape.ShapeStatus status = Shape.ShapeStatus.PROCESSING;

                        if (BeginState == true)
                        {
                            s = State.DONE;
                            color = CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                        }

                        ProcessSectionFactory factory = ProcessSectionManager.Instance.Factory;
                        ProcessSectionManager.Instance.Add(factory.CreateProcess(this));

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, s);
                        if (BeginState == true || mParent.WaitComplete == false)
                        {
                            if (OnChange(this, args))
                            {
                                SetColor(Section, color, status);
                                mState = s;

                            }
                        }
                        else
                        {
                            SetColor(Section, color, status);
                            mState = s;

                        }

                        SetProcessUnderColor(Section, TeamNormalColor);
                        mbNotify = false;
                        mSection.Notify(mbNotify);
                        OnPostChange(this, args);


                        //if( mSection != null && IsEndSection())
                        //{
                        //    Complete();
                        //}
                    }

                }

                public void AsyncSkip()
                {

                }

                public void AsyncInProgress()
                {
                    if (mState != State.RUN)
                    {
                        State s = State.RUN;
                        Color color = InProgressColor;
                        Shape.ShapeStatus status = Shape.ShapeStatus.PROCESSING;

                        if (BeginState == true)
                        {
                            s = State.DONE;
                            color = CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                        }

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, s);
                        if (BeginState == true || mParent.WaitComplete == false)
                        {
                            if (OnChange(this, args))
                            {
                                SetColor(Section, color, status);
                                mState = s;

                            }
                        }
                        else
                        {
                            SetColor(Section, color, status);
                            mState = s;

                        }
                        SetProcessUnderColor(Section, TeamNormalColor);
                        mbNotify = false;
                        mSection.Notify(mbNotify);
                        OnPostChange(this, args);

                        if (mSection != null && IsEndSection())
                        {
                            Complete();
                        }
                    }
                }

                public virtual void InputWait()
                {
                    // Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
                    if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
                        mSection.GetComponentType() == Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
                    {

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.INPUT);

                        SetProcessUnderColor(Section, TeamNormalColor);
                        SetColor(Section, InputWaitColor, Shape.ShapeStatus.WAITING);
                        mState = State.INPUT;
                        mbNotify = true;
                        mSection.Notify(mbNotify);

                        if (mSection.GetSectionPainter(0) != null)
                        {
                            ProcessButtonManager mgr = (ProcessButtonManager)mSection.GetSectionPainter(0);
                            mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, this);
                        }
                        if (mSection != null && mSection.GetSectionPainter(1) != null)
                        {
                            ConfirmButtonManager mgr = (ConfirmButtonManager)mSection.GetSectionPainter(1);
                            mgr.SetAllButtonsStatus(ConfirmButton.ButtonStatus.WAIT, null, this);
                        }

                        IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                        if (mainForm != null)
                            mainForm.PostChangeSectionState(mSection, mState);

                        OnPostChange(this, args);
                    }
                }

                public virtual void Skip()
                {
                    if (mState != State.SKIP && mState != State.DONE && mState != State.RUN)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.SKIP);
                        if (OnChange(this, args))
                        {
                            SetColor(Section, SkipColor, Shape.ShapeStatus.SKIPPED);
                            SetProcessUnderColor(Section, TeamNormalColor);
                            mState = State.SKIP;
                            mbNotify = false;
                            mSection.Notify(mbNotify);

                            OnPostChange(this, args);
                        }
                    }
                }

                public virtual void Cancel()
                {
                    if (mState == State.RUN)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.NORMAL);
                        if (mParent.WaitComplete == true)
                        {
                            OnChange(this, args);
                        }
                        Wait();

                        OnPostChange(this, args);
                    }
                }


                public void WaitChanged(object sender, WaitOptionChangeEventArgs e)
                {

                }


                public void OnChangeColor(object sender, ColorChangeEventArgs e)
                {
                    NoramlColor = WorkFlowManager.Instance.NoramlColor;
                    WaitColor = WorkFlowManager.Instance.WaitColor;
                    CompleteColor = WorkFlowManager.Instance.CompleteColor;
                    InProgressColor = WorkFlowManager.Instance.InProgressColor;
                    InputWaitColor = WorkFlowManager.Instance.InputWaitColor;
                    SkipColor = WorkFlowManager.Instance.SkipColor;
                    TeamCompleteColor = WorkFlowManager.Instance.TeamCompleteColor;
                    TeamNormalColor = WorkFlowManager.Instance.TeamNormalColor;
                    Color color = NoramlColor;
                    Shape.ShapeStatus status = Shape.ShapeStatus.NORMAL;

                    switch (mState)
                    {
                        case State.NORMAL:
                            break;
                        case State.RUN:
                            color = InProgressColor;
                            status = Shape.ShapeStatus.PROCESSING;
                            break;
                        case State.DONE:
                            color = CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                            break;
                        case State.INPUT:
                            color = InputWaitColor;
                            status = Shape.ShapeStatus.WAITING;
                            break;
                        case State.SKIP:
                            color = SkipColor;
                            status = Shape.ShapeStatus.SKIPPED;
                            break;
                    }
                    SetColor(Section, color, status);
                }
            }

            public class TSectionState : SectionState
            {
                public event ChangeStateEvent OnChangeT;
                public event PostChangeEvent OnPostChangeT;

                ProcessSectionIF process = null;
                public ProcessSectionIF Process
                {
                    get { return process; }
                    set { process = value; }
                }

                public TSectionState(WorkFlow parent, Section section)
                    : base(parent, section)
                {
                    WorkFlow.GetTransmissionCheckedNotify((SectionTransmission)section, out m_nCheckedNotify1, out m_nCheckedNotify2);
                }

                public override void InputWait()
                {
                    // Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
                    if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
                        mSection.GetComponentType() == Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
                    {

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.INPUT);

                        SetProcessUnderColor(Section, TeamNormalColor);
                        SetColor(Section, InputWaitColor, Shape.ShapeStatus.WAITING);
                        mState = State.INPUT;
                        mbNotify = true;
                        mSection.Notify(mbNotify);

                        if (mSection.GetSectionPainter(0) != null)
                        {
                            ProcessButtonManager mgr = (ProcessButtonManager)mSection.GetSectionPainter(0);
                            mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, this);
                        }

                        if (mSection != null && mSection.GetSectionPainter(1) != null)
                        {
                            ConfirmButtonManager mgr = (ConfirmButtonManager)mSection.GetSectionPainter(1);
                            mgr.SetAllButtonsStatus(ConfirmButton.ButtonStatus.WAIT, null, this);
                        }

                        IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                        if (mainForm != null)
                            mainForm.PostChangeSectionState(mSection, mState);

                        OnPostChangeT(this, args);
                    }
                }

                public override void InProgress()
                {
                    if (BeginTime == null)
                    {
                        BeginTime = DateTime.Now;
                        m_historyManager.AddSectionHistory(this.Section, this, mState, -1, false, 0, 0, 0, 0, new Dictionary<int, List<History.HistorySectionData.DetailData>>());
                    }

                    process = null;
                    if (State != State.RUN)
                    {
                        State s = State.RUN;
                        Color color = WorkFlowManager.Instance.InProgressColor;
                        Shape.ShapeStatus status = Shape.ShapeStatus.PROCESSING;

                        if (BeginState == true)
                        {
                            s = State.DONE;
                            color = WorkFlowManager.Instance.CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                        }

                        ProcessSectionFactory factory = ProcessSectionManager.Instance.Factory;
                        process = factory.CreateProcess(this);
                        ProcessSectionManager.Instance.Add(process);

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, s);
                        if (BeginState == true || Parent.WaitComplete == false)
                        {
                            if (OnChangeT(this, args))
                            {
                                SetColor(Section, color, status);
                                State = s;
                            }
                        }
                        else
                        {
                            SetColor(Section, color, status);
                            State = s;
                        }

                        SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamNormalColor);
                        MbNotify = false;
                        Section.Notify(MbNotify);

                        OnPostChangeT(this, args);
                    }
                }

                public override void Complete(int nProcessDirections = (int)ProcessDirectionHistory.NONE)
                {
                    if (State != State.DONE)
                    {
                        //ProcessManager.Instance.Add(process);
                        //process = ProcessFactory.CreateProcess(this);

                        ProcessSectionIF tProcess = process;
                        if (process == null)
                            return;
                        tProcess.StartBrodcast();

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, State.DONE);
                        if (OnChangeT(this, args))
                        {
                            CompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
                            //m_nCompleteCount++;
                            SetColor(Section, WorkFlowManager.Instance.CompleteColor, Shape.ShapeStatus.PROCESSED);
                            SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamCompleteColor);
                            State = State.DONE;
                            MbNotify = false;
                            Section.Notify(MbNotify);

                            mProcessDirections = nProcessDirections;

                            OnPostChangeT(this, args);
                        }
                    }
                }
            }

            public class ESectionState : SectionState
            {
                public event ChangeStateEvent OnChangeE;
                public event PostChangeEvent OnPostChangeE;

                ProcessSectionIF process = null;
                public ProcessSectionIF Process
                {
                    get { return process; }
                    set { process = value; }
                }

                public ESectionState(WorkFlow parent, Section section)
                    : base(parent, section)
                {
                    WorkFlow.GetExternalCheckedNotify((SectionExternal)section, out m_nCheckedNotify1, out m_nCheckedNotify2);
                }

                public override void InputWait()
                {
                    // Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
                    if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
                        mSection.GetComponentType() == Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
                    {

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.INPUT);

                        SetProcessUnderColor(Section, TeamNormalColor);
                        SetColor(Section, InputWaitColor, Shape.ShapeStatus.WAITING);
                        mState = State.INPUT;
                        mbNotify = true;
                        mSection.Notify(mbNotify);

                        if (mSection.GetSectionPainter(0) != null)
                        {
                            ProcessButtonManager mgr = (ProcessButtonManager)mSection.GetSectionPainter(0);
                            mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, this);
                        }
                        if (mSection != null && mSection.GetSectionPainter(1) != null)
                        {
                            ConfirmButtonManager mgr = (ConfirmButtonManager)mSection.GetSectionPainter(1);
                            mgr.SetAllButtonsStatus(ConfirmButton.ButtonStatus.WAIT, null, this);
                        }

                        IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                        if (mainForm != null)
                            mainForm.PostChangeSectionState(mSection, mState);

                        OnPostChangeE(this, args);
                    }
                }

                public override void InProgress()
                {
                    if (BeginTime == null)
                    {
                        BeginTime = DateTime.Now;
                        m_historyManager.AddSectionHistory(this.Section, this, mState, -1, false, 0, 0, 0, 0, new Dictionary<int, List<History.HistorySectionData.DetailData>>());
                    }

                    process = null;
                    if (State != State.RUN)
                    {
                        State s = State.RUN;
                        Color color = WorkFlowManager.Instance.InProgressColor;
                        Shape.ShapeStatus status = Shape.ShapeStatus.PROCESSING;

                        if (BeginState == true)
                        {
                            s = State.DONE;
                            color = WorkFlowManager.Instance.CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                        }

                        ProcessSectionFactory factory = ProcessSectionManager.Instance.Factory;
                        process = factory.CreateProcess(this);
                        ProcessSectionManager.Instance.Add(process);

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, s);
                        if (BeginState == true || Parent.WaitComplete == false)
                        {
                            if (OnChangeE(this, args))
                            {
                                SetColor(Section, color, status);
                                State = s;
                            }
                        }
                        else
                        {
                            SetColor(Section, color, status);
                            State = s;
                        }

                        SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamNormalColor);
                        MbNotify = false;
                        Section.Notify(MbNotify);

                        OnPostChangeE(this, args);
                    }
                }

                public override void Complete(int nProcessDirections = (int)ProcessDirectionHistory.NONE)
                {
                    if (State != State.DONE)
                    {
                        //ProcessManager.Instance.Add(process);
                        //process = ProcessFactory.CreateProcess(this);
                        if (process == null)
                            return;
                        process.SendSMSMessage();

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, State.DONE);
                        if (OnChangeE(this, args))
                        {
                            CompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
                            //m_nCompleteCount++;
                            SetColor(Section, WorkFlowManager.Instance.CompleteColor, Shape.ShapeStatus.PROCESSED);
                            SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamCompleteColor);
                            State = State.DONE;
                            MbNotify = false;
                            Section.Notify(MbNotify);

                            mProcessDirections = nProcessDirections;

                            OnPostChangeE(this, args);
                        }
                    }
                }

                public override void Cancel()
                {
                    if (mState == State.RUN)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.NORMAL);
                        if (mParent.WaitComplete == true)
                        {
                            OnChangeE(this, args);
                        }
                        Wait();

                        OnPostChangeE(this, args);
                    }
                }

                public override void Wait()
                {
                    if (mState != State.NORMAL)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.NORMAL);
                        //if (OnChange(this, args))
                        //{
                        SetColor(Section, NoramlColor, Shape.ShapeStatus.NORMAL);
                        SetProcessUnderColor(Section, TeamNormalColor);
                        mState = State.NORMAL;
                        mbNotify = false;
                        mSection.Notify(mbNotify);

                        OnPostChangeE(this, args);
                    }
                }
            }

            public class ISectionState : SectionState
            {
                public event ChangeStateEvent OnChangeI;
                public event PostChangeEvent OnPostChangeI;

                ProcessSectionIF process = null;
                public ProcessSectionIF Process
                {
                    get { return process; }
                    set { process = value; }
                }

                public ISectionState(WorkFlow parent, Section section)
                    : base(parent, section)
                {
                    WorkFlow.GetInternalCheckedNotify((SectionInternal)section, out m_nCheckedNotify1);
                }

                public override void InputWait()
                {
                    // Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
                    if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
                        mSection.GetComponentType() == Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
                    {

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.INPUT);

                        SetProcessUnderColor(Section, TeamNormalColor);
                        SetColor(Section, InputWaitColor, Shape.ShapeStatus.WAITING);
                        mState = State.INPUT;
                        mbNotify = true;
                        mSection.Notify(mbNotify);

                        if (mSection.GetSectionPainter(0) != null)
                        {
                            ProcessButtonManager mgr = (ProcessButtonManager)mSection.GetSectionPainter(0);
                            mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, this);
                        }
                        if (mSection != null && mSection.GetSectionPainter(1) != null)
                        {
                            ConfirmButtonManager mgr = (ConfirmButtonManager)mSection.GetSectionPainter(1);
                            mgr.SetAllButtonsStatus(ConfirmButton.ButtonStatus.WAIT, null, this);
                        }

                        IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                        if (mainForm != null)
                            mainForm.PostChangeSectionState(mSection, mState);

                        OnPostChangeI(this, args);
                    }
                }

                public override void InProgress()
                {
                    if (BeginTime == null)
                    {
                        BeginTime = DateTime.Now;
                        m_historyManager.AddSectionHistory(this.Section, this, mState, -1, false, 0, 0, 0, 0, new Dictionary<int, List<History.HistorySectionData.DetailData>>());
                    }

                    process = null;
                    if (State != State.RUN)
                    {
                        State s = State.RUN;
                        Color color = WorkFlowManager.Instance.InProgressColor;
                        Shape.ShapeStatus status = Shape.ShapeStatus.PROCESSING;

                        if (BeginState == true)
                        {
                            s = State.DONE;
                            color = WorkFlowManager.Instance.CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                        }

                        ProcessSectionFactory factory = ProcessSectionManager.Instance.Factory;
                        process = factory.CreateProcess(this);
                        ProcessSectionManager.Instance.Add(process);

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, s);
                        if (BeginState == true || Parent.WaitComplete == false)
                        {
                            if (OnChangeI(this, args))
                            {
                                SetColor(Section, color, status);
                                State = s;
                            }
                        }
                        else
                        {
                            SetColor(Section, color, status);
                            State = s;
                        }

                        SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamNormalColor);
                        MbNotify = false;
                        Section.Notify(MbNotify);

                        OnPostChangeI(this, args);
                    }
                }

                public override void Complete(int nProcessDirections = (int)ProcessDirectionHistory.NONE)
                {
                    if (State != State.DONE)
                    {
                        ProcessSectionIF iProcess = process;
                        if (process == null)
                        {
                            ProcessSectionFactory factory = ProcessSectionManager.Instance.Factory;
                            process = factory.CreateProcess(this);
                            iProcess = process;
                        }

                        // [다음] 버튼을 눌렀을때 Section이 [완료] 상태가 되어 방송 또는 문자가 실행되는 오류 제거
                        //iProcess.StartBrodcast();

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, State.DONE);
                        if (OnChangeI(this, args))
                        {
                            CompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
                            //m_nCompleteCount++;
                            SetColor(Section, WorkFlowManager.Instance.CompleteColor, Shape.ShapeStatus.PROCESSED);
                            SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamCompleteColor);
                            State = State.DONE;
                            MbNotify = false;
                            Section.Notify(MbNotify);

                            mProcessDirections = nProcessDirections;

                            OnPostChangeI(this, args);
                        }
                    }
                }

                public override void Cancel()
                {
                    if (mState == State.RUN)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.NORMAL);
                        if (mParent.WaitComplete == true)
                        {
                            OnChangeI(this, args);
                        }
                        Wait();

                        OnPostChangeI(this, args);
                    }
                }

                public override void Wait()
                {
                    if (mState != State.NORMAL)
                    {
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.NORMAL);
                        //if (OnChange(this, args))
                        //{
                        SetColor(Section, NoramlColor, Shape.ShapeStatus.NORMAL);
                        SetProcessUnderColor(Section, TeamNormalColor);
                        mState = State.NORMAL;
                        mbNotify = false;
                        mSection.Notify(mbNotify);

                        OnPostChangeI(this, args);
                    }
                }
            }

            // Process Section State
            public class PSectionState : SectionState
            {
                public event ChangeStateEvent OnChangeP;
                public event PostChangeEvent OnPostChangeP;

                protected ProcessSectionIF process = null;
                public ProcessSectionIF Process
                {
                    get { return process; }
                    set { process = value; }
                }

                public PSectionState(WorkFlow parent, Section section)
                    : base(parent, section)
                {
                    WorkFlow.GetProcessCheckedNotify((SectionProcess)section, out m_nCheckedNotify1, out m_nCheckedNotify2);
                }

                public override void SetInitStateColor()
                {
                    base.SetInitStateColor();
                    SetProcessUnderColor(Section, TeamNormalColor);
                }

                public override void InputWait()
                {
                    // Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
                    if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
                        mSection.GetComponentType() == Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
                    {

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.INPUT);

                        SetProcessUnderColor(Section, TeamNormalColor);
                        SetColor(Section, InputWaitColor, Shape.ShapeStatus.WAITING);
                        mState = State.INPUT;
                        mbNotify = true;
                        mSection.Notify(mbNotify);

                        if (mSection.GetSectionPainter(0) != null)
                        {
                            ProcessButtonManager mgr = (ProcessButtonManager)mSection.GetSectionPainter(0);
                            mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, this);
                        }

                        if (mSection != null && mSection.GetSectionPainter(1) != null)
                        {
                            ConfirmButtonManager mgr = (ConfirmButtonManager)mSection.GetSectionPainter(1);
                            mgr.SetAllButtonsStatus(ConfirmButton.ButtonStatus.WAIT, null, this);
                        }

                        IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                        if (mainForm != null)
                            mainForm.PostChangeSectionState(mSection, mState);

                        OnPostChangeP(this, args);
                    }
                }

                public override void InProgress()
                {
                    if (BeginTime == null)
                    {
                        BeginTime = DateTime.Now;
                        m_historyManager.AddSectionHistory(this.Section, this, mState, -1, false, 0, 0, 0, 0, new Dictionary<int, List<History.HistorySectionData.DetailData>>());
                    }

                    process = null;
                    //if (State != State.RUN)
                    {
                        State s = State.RUN;
                        Color color = WorkFlowManager.Instance.InProgressColor;
                        Shape.ShapeStatus status = Shape.ShapeStatus.PROCESSING;
                        if (BeginState == true)
                        {
                            s = State.DONE;
                            color = WorkFlowManager.Instance.CompleteColor;
                            status = Shape.ShapeStatus.PROCESSED;
                        }

                        //ProcessManager.Instance.Add(ProcessFactory.CreateProcess(this));

                        StateChangeEventArgs args = new StateChangeEventArgs(Parent, Section, this, State, s);
                        if (BeginState == true || Parent.WaitComplete == false)
                        {
                            if (OnChangeP(this, args))
                            {
                                SetColor(Section, color, status);
                                State = s;

                            }
                        }
                        else
                        {
                            SetColor(Section, color, status);
                            State = s;

                        }

                        SetProcessUnderColor(Section, WorkFlowManager.Instance.TeamNormalColor);
                        MbNotify = false;
                        Section.Notify(MbNotify);

                        OnPostChangeP(this, args);
                    }
                }

                public override void Complete(int nProcessDirections = (int)ProcessDirectionHistory.NONE)
                {
                    if (mState != State.DONE)
                    {
                        this.ProcessDirections |= nProcessDirections;

                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.DONE);
                        if (OnChangeP(this, args))
                        {
                            m_nCompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
                            //m_nCompleteCount++;
                            SetColor(Section, CompleteColor, Shape.ShapeStatus.PROCESSED);
                            SetProcessUnderColor(Section, TeamCompleteColor);
                            mState = State.DONE;
                            mProcessDirections = nProcessDirections;
                            mbNotify = false;
                            mSection.Notify(mbNotify);

                            ProcessSectionFactory factory = ProcessSectionManager.Instance.Factory;
                            ProcessSectionIF process = factory.CreateProcess(this);
                            ProcessSectionManager.Instance.Add(process);

                            OnPostChangeP(this, args);
                        }
                    }
                    else
                    {
                        this.ProcessDirections |= nProcessDirections;

                        // SelectSection은 이미 완료되었으므로 다음 Section에 대한 상태만 변화시킨다.
                        StateChangeEventArgs args = new StateChangeEventArgs(mParent, Section, this, mState, State.DONE);
                        OnChangeP(this, args);
                    }
                }
            }
            
        }
    }
}
