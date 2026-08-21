using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using SOPMonitoringSystem.Process;
using SOPMonitoringSystem;
using System.Diagnostics;
using System.Windows.Forms;

namespace Sections
{
	public delegate bool ChangeStateEvent(object sender, StateChangeEventArgs e);
	public delegate bool PostChangeEvent(object sender, StateChangeEventArgs e);
	public delegate void WorkFlowEvent(object sender, WorkFlowEventArgs e);
	public delegate void ChangeColor(object sender, ColorChangeEventArgs e);

	public delegate bool ChangeSopEvent(object sender, SopChangeEventArgs e);
		

	public enum State
	{
		NORMAL = 1,
		RUN = 2,
		DONE = 3,
		INPUT = 4,
		SKIP = 5
	}

	public enum ProcessDirection
	{
		NONE = 0,
		TOP = 1,
		RIGHT = 2,
		BOTTOM = 4,
		LEFT = 8
	}

	public enum WorkFlowState
	{
		STANDBY = 1,
		RUN = 2,
		PAUSE = 3,
		WAIT = 4,
		STOP = 5,   // 실행 취소
		DISABLE = 6,
		DONE = 7    // 정상 완료
	}

	public enum WorkFlowMode
	{
		REAL = 1,
		VIRTUAL = 2
	}

	public class SopChangeEventArgs
	{
		public SopChangeEventArgs()
		{
		
		}

	}

	public class ColorChangeEventArgs
	{
		public ColorChangeEventArgs()
		{
			
		}
	}
	public class WorkFlowEventArgs
	{
		public WorkFlowEventArgs()
		{

		}
	}
	public class StateChangeEventArgs
	{
		private WorkFlow mParent = null;
		public WorkFlow Parent
		{
			get { return mParent; }
		}
		private Section mSection = null;
		public Section Section
		{
			get { return mSection; }
		}
		private Section mNextSection = null;
		public Section NextSection
		{
			get { return mNextSection; }
			set { mNextSection = value; }
		}
			
		private SectionState mSectionState = null;
		public SectionState SectionState
		{
			get { return mSectionState; }
		}
		private State mCurState = State.NORMAL;
		public State CurState
		{
			get { return mCurState; }
		}
		private State mPrevState = State.NORMAL;
		public State PrevState
		{
			get { return mPrevState; }
		}
		public StateChangeEventArgs(WorkFlow parent, Section section, SectionState sectionState, State prevState, State curState)
		{
			mParent = parent;
			mSection = section;
			mSectionState = sectionState;
			mCurState = curState;
			mPrevState = prevState;
		}
	}

	public class SectionState
	{
		protected Color NoramlColor = Color.White;
		protected Color WaitColor = Color.White;
		protected Color CompleteColor = Color.FromArgb(252, 213, 181);
		protected Color InProgressColor = Color.FromArgb(142, 180, 227);
		protected Color InputWaitColor = Color.FromArgb(255, 174, 201);
		protected Color SkipColor = Color.FromArgb(255, 233, 127);
		protected Color TeamCompleteColor = Color.FromArgb(128, 128, 128);
		protected Color TeamNormalColor = Color.Aqua;
		protected Color Current;

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
			set { mState = value; }
		}

		// ProcessDirection에 대한 비트 Flag 조합
		protected int mProcessDirections = (int)Sections.ProcessDirection.NONE;
		public int ProcessDirections
		{
			get { return mProcessDirections; }
			set { mProcessDirections = value; }
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
			SelectSection.Data.AggSection = null;
			m_nCompleteCount = 0;

            SetInitStateColor();

			// ProcessButton 초기화
			if (mSection != null && mSection.AdditionalPainter != null)
			{
				SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)mSection.AdditionalPainter;
				mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.WAIT);
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
            if (state == Sections.State.NORMAL)
                return Shape.ShapeStatus.NORMAL;
            else if (state == Sections.State.INPUT)
                return Shape.ShapeStatus.WAITING;
            else if (state == Sections.State.DONE)
                return Shape.ShapeStatus.PROCESSED;
            else if (state == Sections.State.RUN)
                return Shape.ShapeStatus.PROCESSING;
            else if (state == Sections.State.SKIP)
                return Shape.ShapeStatus.SKIPPED;

            return Shape.ShapeStatus.NORMAL;
        }

		public void CopyState(SectionState state)
		{
			mState = state.mState;
			mbNotify = state.mbNotify;
			Current = state.Current;
			SetColor(mSection, Current, GetShapeStatus(mState));
			if (mState == Sections.State.DONE)
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
			mbNotify = state == Sections.State.INPUT;
			Current = colorCurrent;
			SetColor(mSection, Current, GetShapeStatus(mState));
			if (mState == Sections.State.DONE)
			{
				SetProcessUnderColor(mSection, TeamCompleteColor);
			}
			else
			{
				SetProcessUnderColor(mSection, TeamNormalColor);
			}                
			mSection.Notify(mbNotify);
		}

		public SectionState(WorkFlow parent , Section section)
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

		public Section SelectSection
		{
			get
			{
				return mSection;
			}
		}

		public void ClearNotify()
		{
			mbNotify = false;
			mSection.Notify(mbNotify);
		}

		public void SetProcessUnderColor(Section section, Color color)
		{
			Sections.Section.ComponentType type = mSection.GetComponentType();
			if (type == Sections.Section.ComponentType.PROCESS)
			{
				SectionProcess psection = (SectionProcess)section;
				//psection.SetFillColor(color, false);
				Current = color;
			}
		}

		public void SetColor(Section section, Color color, Sections.Shape.ShapeStatus status)
		{
			Sections.Section.ComponentType type = mSection.GetComponentType();
			if (type == Sections.Section.ComponentType.PROCESS)
			{
				SectionProcess psection = (SectionProcess)section;
				//psection.SetFillColor(color, true);
				Current = color;
			}
			else
			{
				Current = color;
				section.SetColor(Section.ColorTarget.FILL, color);
			}

            section.Shape.Status = status;
		}


		public virtual void Complete(int nProcessDirections = (int)ProcessDirection.NONE)
		{
			if (mState != State.DONE)
			{
				this.ProcessDirections |= nProcessDirections;

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.DONE);
				if (OnChange(this, args))
				{
					m_nCompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
					//m_nCompleteCount++;
					SetColor(SelectSection, CompleteColor, Shape.ShapeStatus.PROCESSED);
					SetProcessUnderColor(SelectSection, TeamCompleteColor);
					mState = State.DONE;
					mProcessDirections = nProcessDirections;
					mbNotify = false;
					mSection.Notify(mbNotify);
					OnPostChange(this, args);
				}
			}
			else
			{
				this.ProcessDirections |= nProcessDirections;

				// SelectSection은 이미 완료되었으므로 다음 Section에 대한 상태만 변화시킨다.
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.DONE);
				OnChange(this, args);
			}
		}

		public virtual void Wait()
		{
			if (mState != State.NORMAL)
			{
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.NORMAL);
				//if (OnChange(this, args))
				//{
					SetColor(SelectSection, NoramlColor, Shape.ShapeStatus.NORMAL);
					SetProcessUnderColor(SelectSection, TeamNormalColor);
					mState = State.NORMAL;
					mbNotify = false;
					mSection.Notify(mbNotify);
					OnPostChange(this, args);
				//}
			}
			   
		}

		public virtual void InProgress()
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

				ProcessManager.Instance.Add(ProcessFactory.CreateProcess(this));
				   
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, s);
				if (BeginState == true || mParent.WaitComplete == false)
				{
					if (OnChange(this, args))
					{
						SetColor(SelectSection, color, status);
						mState = s;

					}
				}
				else
				{
					SetColor(SelectSection, color, status);
					mState = s;

				}

				SetProcessUnderColor(SelectSection, TeamNormalColor);
				mbNotify = false;
				mSection.Notify(mbNotify);
				OnPostChange(this, args);
						
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

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, s);
				if (BeginState == true || mParent.WaitComplete == false)
				{
					if (OnChange(this, args))
					{
						SetColor(SelectSection, color, status);
						mState = s;

					}
				}
				else
				{
					SetColor(SelectSection, color, status);
					mState = s;

				}
				SetProcessUnderColor(SelectSection, TeamNormalColor);
				mbNotify = false;
				mSection.Notify(mbNotify);
				OnPostChange(this, args);                    					
			}
		}

		public virtual void InputWait()
		{
			// Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
			if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
				SelectSection.GetComponentType() == Sections.Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
			{

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.INPUT);

				SetProcessUnderColor(SelectSection, TeamNormalColor);
				SetColor(SelectSection, InputWaitColor, Shape.ShapeStatus.WAITING);
				mState = State.INPUT;
				mbNotify = true;
				mSection.Notify(mbNotify);

				if (mSection.AdditionalPainter != null)
				{
					SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)mSection.AdditionalPainter;
					mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.WAIT, null, this);
				}

				OnPostChange(this, args);                    
			}
		}

		public virtual void Skip()
		{                
			if (mState != State.SKIP && mState != State.DONE && mState != State.RUN)
			{
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.SKIP);
				if (OnChange(this, args))
				{
					SetColor(SelectSection, SkipColor, Shape.ShapeStatus.SKIPPED);
					SetProcessUnderColor(SelectSection, TeamNormalColor);
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
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.NORMAL);
				if (mParent.WaitComplete == true)
				{
					OnChange(this, args);
				}
				Wait();

				OnPostChange(this, args);
			}
		}


		public void WaitChanged(object sender, SOPMonitoringSystem.WaitOptionChangeEventArgs e)
		{
				
		}


		public void OnChangeColor(object sender , ColorChangeEventArgs e )
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

			switch(mState)
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
			SetColor(SelectSection, color, status);
		}
	}
	
	public class TSectionState : SectionState
	{
		public event ChangeStateEvent OnChangeT;
		public event PostChangeEvent OnPostChangeT;

		ProcessIF process = null;
		public SOPMonitoringSystem.Process.ProcessIF Process
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
				SelectSection.GetComponentType() == Sections.Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
			{

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.INPUT);

				SetProcessUnderColor(SelectSection, TeamNormalColor);
				SetColor(SelectSection, InputWaitColor, Shape.ShapeStatus.WAITING);
				mState = State.INPUT;
				mbNotify = true;
				mSection.Notify(mbNotify);

				if (mSection.AdditionalPainter != null)
				{
					SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)mSection.AdditionalPainter;
					mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.WAIT, null, this);
				}

				OnPostChangeT(this, args);
			}
		}

		public override void InProgress()
		{
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
				process =ProcessFactory.CreateProcess(this);
				ProcessManager.Instance.Add(process);

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, s);
				if (BeginState == true || Parent.WaitComplete == false)
				{
					if (OnChangeT(this, args))
					{
						SetColor(SelectSection, color, status);
						State = s;
					}
				}
				else
				{
					SetColor(SelectSection, color, status);
					State = s;
				}

				SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamNormalColor);
				MbNotify = false;
				Section.Notify(MbNotify);

				OnPostChangeT(this, args);
			}
		}

		public override void Complete(int nProcessDirections = (int)ProcessDirection.NONE)
		{
			if (State != State.DONE)
			{
				//ProcessManager.Instance.Add(process);
				//process = ProcessFactory.CreateProcess(this);

				TransmissionNotifyProcess tProcess = (TransmissionNotifyProcess)process;
				if (process == null) return;
				tProcess.StartBrodcast();

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, State.DONE);
				if (OnChangeT(this, args))
				{
					CompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
					//m_nCompleteCount++;
					SetColor(SelectSection, WorkFlowManager.Instance.CompleteColor, Shape.ShapeStatus.PROCESSED);
					SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamCompleteColor);
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

		ProcessIF process = null;
		public SOPMonitoringSystem.Process.ProcessIF Process
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
				SelectSection.GetComponentType() == Sections.Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
			{

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.INPUT);

				SetProcessUnderColor(SelectSection, TeamNormalColor);
				SetColor(SelectSection, InputWaitColor, Shape.ShapeStatus.WAITING);
				mState = State.INPUT;
				mbNotify = true;
				mSection.Notify(mbNotify);

				if (mSection.AdditionalPainter != null)
				{
					SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)mSection.AdditionalPainter;
					mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.WAIT, null, this);
				}

				OnPostChangeE(this, args);
			}
		}

		public override void InProgress()
		{
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
				process = ProcessFactory.CreateProcess(this);
				ProcessManager.Instance.Add(process);

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, s);
				if (BeginState == true || Parent.WaitComplete == false)
				{
					if (OnChangeE(this, args))
					{
						SetColor(SelectSection, color, status);
						State = s;
					}
				}
				else
				{
					SetColor(SelectSection, color, status);
					State = s;
				}

				SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamNormalColor);
				MbNotify = false;
				Section.Notify(MbNotify);

				OnPostChangeE(this, args);
			}
		}

		public override void Complete(int nProcessDirections = (int)ProcessDirection.NONE)
		{
			if (State != State.DONE)
			{
				//ProcessManager.Instance.Add(process);
				//process = ProcessFactory.CreateProcess(this);

				ExternalNotifyProcess eProcess = (ExternalNotifyProcess)process;
				if (process == null) return;
				eProcess.SendSMSMessage();

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, State.DONE);
				if (OnChangeE(this, args))
				{
					CompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
					//m_nCompleteCount++;
					SetColor(SelectSection, WorkFlowManager.Instance.CompleteColor, Shape.ShapeStatus.PROCESSED);
					SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamCompleteColor);
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
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.NORMAL);
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
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.NORMAL);
				//if (OnChange(this, args))
				//{
				SetColor(SelectSection, NoramlColor, Shape.ShapeStatus.NORMAL);
				SetProcessUnderColor(SelectSection, TeamNormalColor);
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

		ProcessIF process = null;
		public SOPMonitoringSystem.Process.ProcessIF Process
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
				SelectSection.GetComponentType() == Sections.Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
			{

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.INPUT);

				SetProcessUnderColor(SelectSection, TeamNormalColor);
				SetColor(SelectSection, InputWaitColor, Shape.ShapeStatus.WAITING);
				mState = State.INPUT;
				mbNotify = true;
				mSection.Notify(mbNotify);

				if (mSection.AdditionalPainter != null)
				{
					SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)mSection.AdditionalPainter;
					mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.WAIT, null, this);
				}

				OnPostChangeI(this, args);
			}
		}

		public override void InProgress()
		{
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
				process = ProcessFactory.CreateProcess(this);
				ProcessManager.Instance.Add(process);

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, s);
				if (BeginState == true || Parent.WaitComplete == false)
				{
					if (OnChangeI(this, args))
					{
						SetColor(SelectSection, color, status);
						State = s;
					}
				}
				else
				{
					SetColor(SelectSection, color, status);
					State = s;
				}

				SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamNormalColor);
				MbNotify = false;
				Section.Notify(MbNotify);

				OnPostChangeI(this, args);
			}
		}

		public override void Complete(int nProcessDirections = (int)ProcessDirection.NONE)
		{
			if (State != State.DONE)
			{
				//ProcessManager.Instance.Add(process);
				//process = ProcessFactory.CreateProcess(this);

				InternalNotifyProcess iProcess = (InternalNotifyProcess)process;
				if (process == null) return;
				iProcess.StartBrodcast();

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, State.DONE);
				if (OnChangeI(this, args))
				{
					CompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
					//m_nCompleteCount++;
					SetColor(SelectSection, WorkFlowManager.Instance.CompleteColor, Shape.ShapeStatus.PROCESSED);
					SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamCompleteColor);
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
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.NORMAL);
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
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.NORMAL);
				//if (OnChange(this, args))
				//{
				SetColor(SelectSection, NoramlColor, Shape.ShapeStatus.NORMAL);
				SetProcessUnderColor(SelectSection, TeamNormalColor);
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

		ProcessIF process = null;
		public SOPMonitoringSystem.Process.ProcessIF Process
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
            SetProcessUnderColor(SelectSection, TeamNormalColor);
        }

		public override void InputWait()
		{
			// Decision의 경우 완료된 상태에서도 다시 입력대기를 받는다.
			if ((mState != State.INPUT && mState != State.DONE && mState != State.RUN) ||
				SelectSection.GetComponentType() == Sections.Section.ComponentType.DECISION && (mState == State.NORMAL || mState == State.DONE))
			{

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.INPUT);

				SetProcessUnderColor(SelectSection, TeamNormalColor);
				SetColor(SelectSection, InputWaitColor, Shape.ShapeStatus.WAITING);
				mState = State.INPUT;
				mbNotify = true;
				mSection.Notify(mbNotify);

				if (mSection.AdditionalPainter != null)
				{
					SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)mSection.AdditionalPainter;
					mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.WAIT, null, this);
				}

				OnPostChangeP(this, args);
			}
		}

		public override void InProgress()
		{
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

				//ProcessManager.Instance.Add(ProcessFactory.CreateProcess(this));

				StateChangeEventArgs args = new StateChangeEventArgs(Parent, SelectSection, this, State, s);
				if (BeginState == true || Parent.WaitComplete == false)
				{
					if (OnChangeP(this, args))
					{
						SetColor(SelectSection, color, status);
						State = s;

					}
				}
				else
				{
					SetColor(SelectSection, color, status);
					State = s;

				}

				SetProcessUnderColor(SelectSection, WorkFlowManager.Instance.TeamNormalColor);
				MbNotify = false;
				Section.Notify(MbNotify);

				OnPostChangeP(this, args);
			}
		}

		public override void Complete(int nProcessDirections = (int)ProcessDirection.NONE)
		{
			if (mState != State.DONE)
			{
				this.ProcessDirections |= nProcessDirections;

				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.DONE);
				if (OnChangeP(this, args))
				{
					m_nCompleteCount = Section.CompleteCount = Section.CompleteCount + 1;
					//m_nCompleteCount++;
					SetColor(SelectSection, CompleteColor, Shape.ShapeStatus.PROCESSED);
					SetProcessUnderColor(SelectSection, TeamCompleteColor);
					mState = State.DONE;
					mProcessDirections = nProcessDirections;
					mbNotify = false;
					mSection.Notify(mbNotify);

					ProcessManager.Instance.Add(ProcessFactory.CreateProcess(this));

					OnPostChangeP(this, args);
				}
			}
			else
			{
				this.ProcessDirections |= nProcessDirections;

				// SelectSection은 이미 완료되었으므로 다음 Section에 대한 상태만 변화시킨다.
				StateChangeEventArgs args = new StateChangeEventArgs(mParent, SelectSection, this, mState, State.DONE);
				OnChangeP(this, args);
			}
		}
	}

	public class WorkFlow
	{
		//////////////////////////////////////////////////////////////////////////
		// PROPERTIES
		public WorkFlowMode RunMode
		{
			get { return nRunMode; }
			set { nRunMode = value; }
		}
		public WorkFlowState State
		{
			get { return nState; }
			set { nState = value; }
		}
		public SectionState Current
		{
			get { return mSectionCur; }
		}
		public SectionState Next
		{
			get { return mSectionPrev; }
		}
		public SectionState Prev
		{
			get { return mSectionNext; }
		}
		public int RunCount
		{
			get { return nRunCount; }
			set { nRunCount = value; }
		}
		public int StopCount
		{
			get { return nStopCount; }
			set { nStopCount = value; }
		}
		public int SkipCount
		{
			get { return nSkipCount; }
			set { nSkipCount = value; }
		}
		public bool WaitComplete
		{
			get { return bWaitComplete; }
			set { bWaitComplete = value; }
		}            
		public string Position
		{
			get { return szPosition; }
			set { szPosition = value; }
		}            
		public bool HasPosition
		{
			get { return bHasPosition; }
			set { bHasPosition = value; }
		}

		private DateTime dtDetectTime;
		public DateTime DetectTime
		{
			get { return dtDetectTime; }
			set { dtDetectTime = value; }
		}
			
		public string SOPName
		{
			get { return szSOPName; }
			set { szSOPName = value; }
		}
		SOPMonitoringSystem.HistoryDiasterPosition mLastPos = null;
		public SOPMonitoringSystem.HistoryDiasterPosition LastPosition
		{
			get { return mLastPos; }
			set { mLastPos = value; }
		}
		//////////////////////////////////////////////////////////////////////////
		// VARIABLE
		public string szSOPName = "";
		private bool bHasPosition = false;
		private string szPosition = "";

		private SortedList mSectionList = null;
		private SortedList mHashList = null;
			
		private int nRunCount = 0;            
		private int nStopCount = 0;            
		private int nSkipCount = 0;            
		private int nRestartCount = 0;

		private WorkFlowMode nRunMode = WorkFlowMode.REAL;            
		private WorkFlowState nState = WorkFlowState.STANDBY;           

		private SectionState mStartState = null;
		private SectionState mSectionCur = null;
		private SectionState mSectionPrev = null;
		private SectionState mSectionNext = null;

		private bool bWaitComplete = true;

		private int nActionStepID = -1;
		public int ActionStepID
		{
			get { return nActionStepID; }
			set { nActionStepID = value; }
		}


		private bool m_BeginEndEventSendSMS = true;
		public bool BeginEndEventSendSMS
		{
			get { return m_BeginEndEventSendSMS; }
			set { m_BeginEndEventSendSMS = value; }
		}

		public event ChangeColor OnChangeColor;
			
		//////////////////////////////////////////////////////////////////////////
		// METHOD          
		public WorkFlow(int actionStepID, ArrayList arSections, WorkFlowMode mode)
		{
			dtDetectTime = DateTime.Now;
			nRunMode = mode;     
			if (arSections == null)
			{
				nState = WorkFlowState.DISABLE;
			}
			else
			{
				nActionStepID = actionStepID;
				mHashList = new SortedList(arSections.Count);
				mSectionList = new SortedList(arSections.Count);
					
				foreach (Section section in arSections )
				{
					if (section.GetComponentType() == Section.ComponentType.ANNOTATION)
					{
						continue;
					}

					int nHash = section.GetHashCode();
					//mHashList.Add(section.Data.ID);

					SectionState state = null;
					if (section.GetComponentType() == Section.ComponentType.TRANSMISSION)
						state = new TSectionState(this, section);
					else if (section.GetComponentType() == Section.ComponentType.INTERNAL)
						state = new ISectionState(this, section);
					else if (section.GetComponentType() == Section.ComponentType.EXTERNAL)
						state = new ESectionState(this, section);
					else if (section.GetComponentType() == Section.ComponentType.PROCESS)
						state = new PSectionState(this, section);
					else
						state = new SectionState(this, section);

					if( section.GetComponentType() == Section.ComponentType.ENDPOINT)
					{
						SectionDataEndPoint data = (SectionDataEndPoint)section.Data;
						if (data.IsBegin == true)
						{
							mStartState = state;
							mStartState.BeginState = true;
						}
						else
						{
							state.EndState = true;
						}
					}
					if (section.GetComponentType() == Section.ComponentType.TRANSSOP)
					{
						state.EndState = true;                           
					}

					Section.ComponentType componentType = section.GetComponentType();

					if (componentType == Section.ComponentType.TRANSMISSION)
					{
						TSectionState tState = (TSectionState)state;
						tState.OnChangeT += new ChangeStateEvent(ChangeSectionState);
						tState.OnPostChangeT += new PostChangeEvent(PostChangeState);
					}
					else if (componentType == Section.ComponentType.INTERNAL)
					{
						ISectionState iState = (ISectionState)state;
						iState.OnChangeI += new ChangeStateEvent(ChangeSectionState);
						iState.OnPostChangeI += new PostChangeEvent(PostChangeState);
					}
					else if (componentType == Section.ComponentType.EXTERNAL)
					{
						ESectionState eState = (ESectionState)state;
						eState.OnChangeE += new ChangeStateEvent(ChangeSectionState);
						eState.OnPostChangeE += new PostChangeEvent(PostChangeState);
					}
					else if (componentType == Section.ComponentType.PROCESS)
					{
						PSectionState eState = (PSectionState)state;
						eState.OnChangeP += new ChangeStateEvent(ChangeSectionState);
						eState.OnPostChangeP += new PostChangeEvent(PostChangeState);
					}
					//else
					//{

					state.OnChange += new ChangeStateEvent(ChangeSectionState);
					state.OnPostChange += new PostChangeEvent(PostChangeState);
						
					//}
					this.OnChangeColor += state.OnChangeColor;
					mSectionList.Add(nHash, state);
				}                    
			}

			SOPMonitoringSystem.FormMain.Instance.GetPageOption().WaitOptionChange += WaitOptionChanged;
		}

		private void WaitOptionChanged(object sender, SOPMonitoringSystem.WaitOptionChangeEventArgs e)
		{
			WaitComplete = !WaitComplete;
			foreach (SectionState state in mSectionList.Values)
			{
				state.WaitChanged(sender, e);
			}
		}

		public void ChangeColorEvent()
		{
			OnChangeColor(this, new ColorChangeEventArgs());
		}

		private bool ChangeSectionState(object sender, StateChangeEventArgs e)
		{
			if (nState == WorkFlowState.STOP || nState == WorkFlowState.PAUSE)
				return false;

			SectionState curState = (SectionState)sender;

			if (mNextSection == null)
			{

				ArrayList nextStateList = FindNext(curState);
				foreach (SectionState state in nextStateList)
				{
					if( state != null)
						state.InputWait();
				}

				if (e.CurState == Sections.State.NORMAL && e.PrevState == Sections.State.RUN)
				{
                    if (nextStateList.Count > 0)
                    {
                        SectionState state = (SectionState)nextStateList[0];
                        if (state != null)
                        {
                            SOPMonitoringSystem.FormMain.Instance.Invoke((MethodInvoker)delegate
                            {
                                SOPMonitoringSystem.FormMain.Instance.FocusSection(state.Section);
                            });
                        }                       
                    }					
				}
			}			
			return true;
		}

		private int nCountd = 0;
		private bool PostChangeState(object sender, StateChangeEventArgs e)
		{
			Debug.WriteLine(nCountd++);
			
			if (nState == WorkFlowState.STOP || nState == WorkFlowState.PAUSE)
				return false;

			SectionState curState = (SectionState)sender;
			Section section = curState.Section;

			LogState(section, curState, e);

			//if (section.Data.AggSection != null)
			//{
			//	SectionState linkState = FindState(curState.Section.Data.AggSection, true);
			//	linkState.CopyState(curState);
			//}

			if (curState.EndState == true && e.CurState == Sections.State.DONE)
			{
				Done(DateTime.Now);
			}			   
			return true;
		}

		public static void GetProcessCheckedNotify(SectionProcess section, out int nCheckedNotify1, out int nCheckedNotify2)
		{
			nCheckedNotify1 = 0;
			nCheckedNotify2 = 0;

			SectionDataProcess data = (SectionDataProcess)section.Data;
			if (data == null)
				return;

			int nMissionCount = (int)data.MissionItems.Count;

			for (int i=0;i<nMissionCount;i++)
			{
				int nSMSFlag = 0;
                int nBroadcastFlag = 0;// 1 << i;

				nCheckedNotify1 |= nSMSFlag;
				nCheckedNotify2 |= nBroadcastFlag;
			}
		}

		public static void GetInternalCheckedNotify(SectionInternal section, out int nCheckedNotify1)
		{
			nCheckedNotify1 = 0;

			SectionDataInternal data = (SectionDataInternal)section.Data;
			if (data == null)
				return;

			if (data.UsePopupMessage)
				nCheckedNotify1 |= 1;

			if (data.UseMobileApp)
				nCheckedNotify1 |= 2;

			if (data.UseBroadcast)
				nCheckedNotify1 |= 4;
		}

		public static void GetExternalCheckedNotify(SectionExternal section, out int nCheckedNotify1, out int nCheckedNotify2)
		{
			nCheckedNotify1 = 0;
			nCheckedNotify2 = 0;
			SectionDataExternal data = (SectionDataExternal)section.Data;
			if (data == null)
				return;
			  
			/*int nIdx = 3;
			int nBit = 0;
			if (data.UseSMS)
			{
				foreach (Sections.ExternalTeamData exTeam in data.SMSReceivers)
				{
					nBit = 1 << nIdx;
					nCheckedNotify1 |= nBit;
					nIdx++;
					if (nIdx == 16)
						break;
				}
			}
			else
			{
			   nCheckedNotify1 = 0;
			}

			nIdx = 0;
			if (data.UseFax)
			{
				foreach (Sections.ExternalTeamData exTeam in data.FaxReceivers)
				{
					 nBit = 1 << nIdx;
					 nCheckedNotify2 |= nBit;
					 nIdx++;
					 if (nIdx == 16)
						 break;
				}
			}
			else
			{
				nCheckedNotify2 = 0;
			}

			if (data.UseSMS)
				nCheckedNotify1 |= (1 << 31);

			if (data.UseFax)
				nCheckedNotify2 |= (1 << 31);*/
            if (data.UseSMS)
                nCheckedNotify1 |= 8;

            if (data.UseFax)
                nCheckedNotify1 |= 16;
		}

		public static void GetTransmissionCheckedNotify(SectionTransmission section, out int nCheckedNotify1, out int nCheckedNotify2)
		{
			nCheckedNotify1 = 0;
			nCheckedNotify2 = 0;
			SectionDataTransmission data = (SectionDataTransmission)section.Data;
			if (data == null)
				return;

			if (data.DataInternal.UsePopupMessage)
				nCheckedNotify1 |= 1;

			if (data.DataInternal.UseMobileApp)
				nCheckedNotify1 |= 2;

			if (data.DataInternal.UseBroadcast)
				nCheckedNotify1 |= 4;
			   
			/*int nIdx = 3;
			int nBit = 0;
			if (data.DataExternal.UseSMS)
			{
				foreach (Sections.ExternalTeamData exTeam in data.DataExternal.SMSReceivers)
				{
					nBit = 1 << nIdx;
					nCheckedNotify1 |= nBit;
					nIdx++;
					if (nIdx == 16)
						break;
				}
			}
			else
			{
				nCheckedNotify1 = 0;
			}

			nIdx = 0;
			if (data.DataExternal.UseFax)
			{
				foreach (Sections.ExternalTeamData exTeam in data.DataExternal.FaxReceivers)
				{
					nBit = 1 << nIdx;
					nCheckedNotify2 |= nBit;
					nIdx++;
					if (nIdx == 16)
						break;
				}
			}
			else
			{
				nCheckedNotify2 = 0;
			}

			if (data.DataExternal.UseSMS)
				nCheckedNotify1 |= (1 << 31);

			if (data.DataExternal.UseFax)
				nCheckedNotify2 |= (1 << 31);*/
            if (data.DataExternal.UseSMS)
                nCheckedNotify1 |= 8;

            if (data.DataExternal.UseFax)
                nCheckedNotify1 |= 16;
		}

		private void LogState(Section section, SectionState state, StateChangeEventArgs e)
		{
			SOPMonitoringSystem.History.HistoryManager history = SOPMonitoringSystem.History.HistoryManager.Instance;
			Sections.Section.ComponentType type = section.GetComponentType();

			if (type == Section.ComponentType.ENDPOINT)
			{
				Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)section.Data;

				if (state.State == Sections.State.DONE)
				{
					Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
					Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;
					history.AddActionStepHistory(panel.ActionStepID, !tabPage.VirtualMode, sectionData.IsBegin ? WorkFlowState.RUN : WorkFlowState.DONE);
				}

				history.AddSectionHistory(section, state.State, state.ProcessDirections, true, 0, 0);
			}
			else if (type == Section.ComponentType.PROCESS)
			{
				//int nCheckedNotify1, nCheckedNotify2;
				int nCheckedNotify1 = state.CheckNotify1;
				int nCheckedNotify2 = state.CheckNotify2;
				//GetProcessCheckedNotify((SectionProcess)section, out nCheckedNotify1, out nCheckedNotify2);
				history.AddSectionHistory(section, state.State, state.ProcessDirections, true, nCheckedNotify1, nCheckedNotify2);
			}
			else if (type == Section.ComponentType.TRANSSOP || type == Section.ComponentType.LINK)
			{
				history.AddSectionHistory(section, state.State, state.ProcessDirections, true, 0, 0);
			}
			else if (type == Section.ComponentType.DECISION)
			{
				history.AddDecisionHistory((Sections.SectionDecision)section, state.State, state.ProcessDirections, DecisionNextSection, true);
			}
			else if (type == Section.ComponentType.INTERNAL)
			{
				SectionDataInternal data = (SectionDataInternal)section.Data;
				if (data != null)
				{
					int nCheckedNotify1 = state.CheckNotify1;
					//GetInternalCheckedNotify((SectionInternal)section, out nCheckedNotify1);
					history.AddInternalHistory((Sections.SectionInternal)section, state.State, state.ProcessDirections, nCheckedNotify1, data.UsePopupMessage, data.UseMobileApp, data.UseBroadcast, true);
				}

			}
			else if (type == Section.ComponentType.EXTERNAL)
			{
				int nCheckedNotify1 = state.CheckNotify1;
				int nCheckedNotify2 = state.CheckNotify2;
				//GetExternalCheckedNotify((SectionExternal)section, out nCheckedNotify1, out nCheckedNotify2);
				history.AddExternalHistory((Sections.SectionExternal)section, state.State, state.ProcessDirections, nCheckedNotify1,nCheckedNotify2, false, false, true);
			}
			else if (type == Section.ComponentType.TRANSMISSION)
			{
				int nCheckedNotify1 = state.CheckNotify1;
				int nCheckedNotify2 = state.CheckNotify2;
				//GetTransmissionCheckedNotify((SectionTransmission)section, out nCheckedNotify1, out nCheckedNotify2);
				history.AddTransmissionHistory((Sections.SectionTransmission)section, state.State, state.ProcessDirections, nCheckedNotify1, nCheckedNotify2, false, false, false, false, false, true);
			}
		}

		private Sections.Section mDecision = null;
		public Sections.Section Decision
		{
			get { return mDecision; }
			set { mDecision = value; }
		}

		private Sections.Section mNextSection = null;
		public Sections.Section DecisionNextSection
		{
			get { return mNextSection; }
			set { mNextSection = value; }
		}

		public void SetDecisionSection( Section nextSection)
		{              
			mNextSection = nextSection;
		}

		// Process 진행 방향이 맞는지 확인
		private bool CheckProcessDirection(SectionState state, Arrow.ArrowPosition pos)
		{
			// 진행 방향이 설정되어 있지 않으면 무조건 return true
			if (state.ProcessDirections == 0)
				return true;

			ProcessDirection direction = SOPMonitoringSystem.ProcessButton.ToProcessDirection(pos);

			if ((state.ProcessDirections & (int)direction) == (int)direction)
				return true;

			return false;
		}

		public ArrayList FindNext(SectionState stateCurrent)
		{
			ArrayList findList = new ArrayList();
			Section section = stateCurrent.SelectSection;
			foreach (Arrow arrow in section.Arrows)
			{
				if( arrow.BeginLink == section && CheckProcessDirection(stateCurrent, arrow.BeginPosition))
				{
					Section end = arrow.EndLink;
					if (end.GetComponentType() == Section.ComponentType.LINK)
					{
						SectionLink link = (SectionLink)end;
						Sections.SectionDataLink data = (Sections.SectionDataLink)link.Data;
						if (data.LinkedSection != null)
							findList.Add(FindState(data.LinkedSection, true));
						//findList.Add(FindState(end, true)); 
					}
					else
						findList.Add(FindState(end, true));
				}                   
			}
			return findList;
		}
			
		private bool CheckDisable()
		{
			if (nState == WorkFlowState.DISABLE)
			{
				return true;
			}
			return false;
		}

		public SectionState FindState(Section section)
		{
			return FindState(section, false);
		}

		public SectionState FindState(Section section, bool bIncludeLink)
		{
			if( section == null)
				return null;

			int nHashCode = section.GetHashCode();
			int nIdx = mSectionList.IndexOfKey(nHashCode);
			if (nIdx < 0)
				return null;
			SectionState state = (SectionState)mSectionList.GetByIndex(nIdx);				
			if (bIncludeLink == false)
			{
				if (section.GetComponentType() == Section.ComponentType.LINK)
				{
					SectionLink link = (SectionLink)section;
					Sections.SectionDataLink data = (Sections.SectionDataLink)link.Data;
					if (data.LinkedSection != null)
					{
						state = FindState(data.LinkedSection, bIncludeLink);
					}
				}
			}                
			return (state);
		}
		public void InitState()
		{
			foreach( SectionState state in mSectionList.Values)
			{
				state.InitState();
			}
		}
		public bool Start()
		{
			if (CheckDisable())
				return false;
			InitState();
			// 이미 시작
			if (nState == WorkFlowState.RUN)
				return false;
			nState = WorkFlowState.RUN;

			// 시작 Section의 Process 버튼들을 모두 완료 상태로 변화시킨다.
			SetStartProcessButtons(mStartState);

			mStartState.InProgress();
				
			return true;
		}

		// 시작 Section의 Process 버튼들을 모두 완료 상태로 변화시킨다.
		protected void SetStartProcessButtons(SectionState state)
		{
            if (state == null)
                return;

			SectionEndPoint sectionBegin = (SectionEndPoint)state.Section;

			if (sectionBegin.AdditionalPainter == null)
				return;

			SOPMonitoringSystem.ProcessButtonManager mgr = (SOPMonitoringSystem.ProcessButtonManager)sectionBegin.AdditionalPainter;
			mgr.SetAllButtonsStatus(SOPMonitoringSystem.ProcessButton.ButtonStatus.DONE, null, state);
		}

		public bool Done(DateTime time, bool noDBWrite = false)
		{
			if (CheckDisable())
				return false;
				
			if(nState == WorkFlowState.DONE)
				return true;
				
			nState = WorkFlowState.DONE;
			IList list = mSectionList.GetValueList();
			foreach ( SectionState state in list)
			{
				state.ClearNotify();
			}

			bool bReal = (RunMode == WorkFlowMode.REAL ? true : false);
			TabPageManager.Instance.SetUsePage(nActionStepID, false, bReal);

			SOPMonitoringSystem.History.HistoryManager.Instance.AddActionStepHistory(this.nActionStepID, this.RunMode == WorkFlowMode.REAL, this.State, time, noDBWrite);

			SOPMonitoringSystem.FormMain.Instance.DoneWorkflow();

			return true;
		}
		public bool Stop(DateTime time, bool noDBWrite = false)
		{
			if (CheckDisable())
				return false;
				
			if(nState == WorkFlowState.STOP)
				return false;
				
			nState = WorkFlowState.STOP;
			IList list = mSectionList.GetValueList();
			foreach ( SectionState state in list)
			{
				state.ClearNotify();
			}

			//SOPMonitoringSystem.History.HistoryManager.Instance.AddActionStepHistory(this.nActionStepID, this.RunMode == WorkFlowMode.REAL, this.State);
			SOPMonitoringSystem.History.HistoryManager.Instance.AddActionStepHistory(this.nActionStepID, this.RunMode == WorkFlowMode.REAL, this.State, time, noDBWrite);

			return true;
		}

		public bool Pause()
		{
			if (CheckDisable())
				return false;
			nState = WorkFlowState.PAUSE;

			return true;
		}

		public void PrevStep()
		{
			if (CheckDisable())
				return;
			nState = WorkFlowState.RUN;
		}

		public void NextStep()
		{
			if (CheckDisable())
				return;
			nState = WorkFlowState.RUN;
		}

		public void Skip()
		{
			if (CheckDisable())
				return;
			nState = WorkFlowState.RUN;
		}

		public void CopyState(WorkFlow rhs)
		{
			nRunMode = rhs.nRunMode; 
			nState = rhs.nState;
			mSectionCur = rhs.mSectionCur;
			mSectionPrev = rhs.mSectionPrev;
			mSectionNext = rhs.mSectionNext;
			nRunCount = rhs.nRunCount; 
			nStopCount = rhs.nStopCount;
			nSkipCount = rhs.nSkipCount;
			bWaitComplete = rhs.bWaitComplete;
			nRestartCount = rhs.nRestartCount;
			mStartState = rhs.mStartState;
			mSectionCur = rhs.mSectionCur;
			mSectionPrev = rhs.mSectionPrev;
			mSectionNext = rhs.mSectionNext;
			nActionStepID = rhs.nActionStepID;

			if(rhs.mSectionList.Count == mSectionList.Count)
			{                    
				foreach (SectionState state in mSectionList.Values)
				{
				}
			}
		}

	}

	public class WorkFlowManager
	{
		private Color noramlColor = Color.White;
		public System.Drawing.Color NoramlColor 
		{
			get { return noramlColor; }
			set { noramlColor = value; }
		}
		private Color waitColor = Color.White;
		public System.Drawing.Color WaitColor
		{
			get { return waitColor; }
			set { waitColor = value; }
		}

		private Color teamNormalColor = Color.Aqua;
		public System.Drawing.Color TeamNormalColor
		{
			get { return teamNormalColor; }
			set { teamNormalColor = value; }
		}
		  
		private Color teamCompleteColor = Color.FromArgb(128, 128, 128);
		public System.Drawing.Color TeamCompleteColor
		{
			get { return teamCompleteColor; }
			set { teamCompleteColor = value; }
		}

		private Color completeColor = Color.FromArgb(252, 213, 181);
		public System.Drawing.Color CompleteColor
		{
			get { return completeColor; }
			set { completeColor = value; }
		}
		private Color inProgressColor = Color.FromArgb(142, 180, 227);
		public System.Drawing.Color InProgressColor
		{
			get { return inProgressColor; }
			set { inProgressColor = value; }
		}
		private Color inputWaitColor = Color.FromArgb(255, 174, 201);
		public System.Drawing.Color InputWaitColor
		{
			get { return inputWaitColor; }
			set { inputWaitColor = value; }
		}
		private Color skipColor = Color.FromArgb(255, 233, 127);
		public System.Drawing.Color SkipColor
		{
			get { return skipColor; }
			set { skipColor = value; }
		}


		private bool bDeleteComplete = false;
		public bool DeleteComplete
		{
			get { return bDeleteComplete; }
			set { bDeleteComplete = value; }
		}

		private bool bWaitComplete = true;
		public bool WaitComplete
		{
			get { return bWaitComplete; }
			set { bWaitComplete = value; }
		}

		protected static WorkFlowManager instance = null;
		public static WorkFlowManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new WorkFlowManager();						
				}
				return instance;
			}
		}

		protected SortedList mWorks = null;
		public System.Collections.SortedList RealWorkFlowList
		{
			get { return mWorks; }                
		}
		protected SortedList mWorksVirtual = null;
		public System.Collections.SortedList VirtualWorkFlowList
		{
			get { return mWorksVirtual; }
		}
			
		protected bool mbInit = false;
		public bool Init
		{
			get { return mbInit; }               
		}

		private WorkFlowManager()
		{
			mWorks = new SortedList();
			mWorksVirtual = new SortedList();
		}

		public void RemoveAll()
		{
			mWorks.Clear();
			mbInit = false;
		}

		public bool Remove(int nKey, bool bReal)
		{
			if (Exist(nKey, bReal) == false)
				return false;

			if (bReal == true)
			{                    
				int nIdx = mWorks.IndexOfKey(nKey);
				if (nIdx < 0)
					return false;
				mWorks.RemoveAt(nIdx);				
				if (mWorks.Count == 0)
					mbInit = false;
				return true;
			}
			else
			{
				int nIdx = mWorksVirtual.IndexOfKey(nKey);
				if (nIdx < 0)
					return false;
				mWorksVirtual.RemoveAt(nIdx);

				if (mWorksVirtual.Count == 0)
					mbInit = false;
				return true;                    
			}				
		}

		public bool Exist(int nActionStepID, bool bReal)
		{
			if (bReal == true)
			{
				if (mWorks.ContainsKey(nActionStepID))
					return true;                    
			}
			else
			{
				if (mWorksVirtual.ContainsKey(nActionStepID))
					return true;
			}
			return false;
		}

		public WorkFlow Get(int nActionID, bool bReal)
		{
			if (Exist(nActionID, bReal) == false)
				return null;
			if (bReal == true)
			{
				int nIdx = mWorks.IndexOfKey(nActionID);
				WorkFlow workflow = (WorkFlow)mWorks.GetByIndex(nIdx);
				return workflow;
			}
			else
			{
				int nIdx = mWorksVirtual.IndexOfKey(nActionID);
				WorkFlow workflow = (WorkFlow)mWorksVirtual.GetByIndex(nIdx);
				return workflow;
			}
				
		}
			
		public WorkFlow Add(int nActionStepID, ArrayList arSections, bool bReal)
		{
			if (arSections == null)
			{
				return null;
			}
			WorkFlowMode mode = (bReal == true) ? WorkFlowMode.REAL : WorkFlowMode.VIRTUAL;
			WorkFlow workflow = new WorkFlow(nActionStepID, arSections, mode);
			workflow.WaitComplete = bWaitComplete;

			if (bReal == true)
				mWorks.Add(nActionStepID, workflow);
			else
				mWorksVirtual.Add(nActionStepID, workflow);
			mbInit = true;

			return workflow;
		}

		public bool Run(int nActionStepID, bool bReal)
		{
			SortedList target = null;
			if (bReal == true)             
				target = mWorks;               
			else
				target = mWorksVirtual;
			if (target.ContainsKey(nActionStepID) == false)
				return false;
			int nIdx = target.IndexOfKey(nActionStepID);

			WorkFlow workflow = (WorkFlow)target.GetByIndex(nIdx);
			bool bResult = workflow.Start();
			return bResult;
		}

		public void ChangeColor()
		{
			foreach (WorkFlow work in mWorks.Values)
			{
				work.ChangeColorEvent();
			}
			foreach (WorkFlow work in mWorksVirtual.Values)
			{
				work.ChangeColorEvent();
			}
		}

		public SectionState Find(Section section, bool bReal)
		{

			if (section == null)
				return null;

			SortedList target = null;
			if (bReal == true)
				target = mWorks;
			else
				target = mWorksVirtual;

			foreach (WorkFlow work in target.Values)
			{
				if (work.State == WorkFlowState.STANDBY || work.State == WorkFlowState.RUN || work.State == WorkFlowState.PAUSE)
				{
					SectionState state = work.FindState(section);
					if (state != null)
						return state;
				}					
			}
			return null;
		}

	}
}
