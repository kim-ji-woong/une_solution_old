using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Sections;
using UnE.SOP.Process;
using UnE.SOP.Sections;
using DBUtility;

namespace UnE
{
	namespace SOP
	{  
		namespace Workstate
		{
			public delegate bool ChangeStateEvent(object sender, StateChangeEventArgs e);
			public delegate bool PostChangeEvent(object sender, StateChangeEventArgs e);
			public delegate void ChangeColor(object sender, ColorChangeEventArgs e);           
			public delegate bool ChangeSopEvent(object sender, SopChangeEventArgs e);
			
			public delegate void WorkFlowEvent(object sender, WorkFlowEventArgs e);          
			

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

			public class DeleteOptionChangeEventArgs
			{
				public DeleteOptionChangeEventArgs()
				{
				}
			}

			public class WaitOptionChangeEventArgs
			{
				public WaitOptionChangeEventArgs()
				{
				}
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

				private WorkFlowState m_nState = WorkFlowState.DISABLE;
				public UnE.SOP.Workstate.WorkFlowState State
				{
					get { return m_nState; }
					set { m_nState = value; }
				}

				private int m_nActionStepID = -1;
				public int ActionStepID
				{
					get { return m_nActionStepID; }
					set { m_nActionStepID = value; }
				}
					  
				private bool m_bRealMode = false;
				public bool RealMode
				{
					get { return m_bRealMode; }
					set { m_bRealMode = value; }
				}
				
				private DateTime m_Time;
				public System.DateTime Time
				{
					get { return m_Time; }
					set { m_Time = value; }
				}

				private bool m_bNoDBWrite = false;
				public bool NoDBWrite
				{
					get { return m_bNoDBWrite; }
					set { m_bNoDBWrite = value; }
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

			public class WorkFlow
			{
                // 동일 Object인지 검사용
                public int HashCode
                {
                    get { return this.GetHashCode(); }
                }

				public event WorkFlowEvent WorkFlowEvent;

                private WorkflowOption m_option = null;
                public WorkflowOption Option
                {
                    get { return m_option; }
                    set
                    {
                        m_option = value;

                        if (m_option != null)
                            m_option.WorkFlow = this;
                    }
                }
                /*private List<Shelter> m_shelters = null;
                public List<Shelter> Shelters
                {
                    get { return m_shelters; }
                    set { m_shelters = value; }
                }*/
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
                public SectionState StartState
                {
                    get { return mStartState; }
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
				/*public string Position
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
				}*/

				public string SOPName
				{
					get { return szSOPName; }
					set { szSOPName = value; }
				}
				/*HistoryDisasterPosition mLastPos = null;
				public HistoryDisasterPosition LastPosition
				{
					get { return mLastPos; }
					set { mLastPos = value; }
				}*/

                /*private string m_strAmountSnowfall = "";
                public string AmountSnowfall
                {
                    get { return m_strAmountSnowfall; }
                    set { m_strAmountSnowfall = value; }
                }

                public bool UseAmountSnowfall
                {
                    get { return m_strAmountSnowfall == null || m_strAmountSnowfall.Length == 0 ? false : true; }
                }*/

				//////////////////////////////////////////////////////////////////////////
				// VARIABLE
				public string szSOPName = "";
				//private bool bHasPosition = false;
				//private string szPosition = "";

                //private string m_strPSMMaterialName = "";
                // 방호대피거리(미터)
                //private int m_nPSMDistance = 0;

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


				private bool m_BeginEndEventSendSMS = false;
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
					//dtDetectTime = DateTime.Now;
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

						foreach (Section section in arSections)
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

							if (section.GetComponentType() == Section.ComponentType.ENDPOINT)
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

					//밖으로
					//FormMain.Instance.GetPageOption().WaitOptionChange += WaitOptionChanged;
				}

				private void WaitOptionChanged(object sender, WaitOptionChangeEventArgs e)
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
							if (state != null)
                                //state.InputWait();
                                state.InProgress();
						}

						if (e.CurState == Workstate.State.NORMAL && e.PrevState == Workstate.State.RUN)
						{
							if (nextStateList.Count > 0)
							{
								SectionState state = (SectionState)nextStateList[0];
								if (state != null)
								{
									Form form = WorkFlowManager.Instance.MainForm;
									form.Invoke((MethodInvoker)delegate
									{
                                        IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                                        if (mainForm != null)
                                            mainForm.FocusSection(state.Section);
									});
								}
							}
						}
					}
					return true;
				}

				private bool PostChangeState(object sender, StateChangeEventArgs e)
				{
					if (nState == WorkFlowState.STOP || nState == WorkFlowState.PAUSE)
						return false;

					SectionState curState = (SectionState)sender;
					Section section = curState.Section;

					LogState(section, curState, e);


                    IWorkflowContainer mainForm = ProxySOP.Instance.WorkflowContainer;
                    if (mainForm != null)
                        mainForm.TouchSection(section);
					//if (section.Data.AggSection != null)
					//{
					//	SectionState linkState = FindState(curState.Section.Data.AggSection, true);
					//	linkState.CopyState(curState);
					//}

					if (curState.EndState == true && e.CurState == Workstate.State.DONE)
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

					for (int i = 0; i < nMissionCount; i++)
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

					/*int nIdx = 0;
					int nBit = 0;
					if (data.UseSMS)
					{
						foreach (ExternalTeamData exTeam in data.SMSReceivers)
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
						foreach (ExternalTeamData exTeam in data.FaxReceivers)
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
						foreach (ExternalTeamData exTeam in data.DataExternal.SMSReceivers)
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
						foreach (ExternalTeamData exTeam in data.DataExternal.FaxReceivers)
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
                        nCheckedNotify2 |= 16;
				}

                public void LogState(Section section, SectionState state, int nCheckedRun, int nCheckedComplete)
                {
                    state.AccessedUserID = ProxySOP.Instance.SOPGenUserID;
                    state.CheckedRun = nCheckedRun;
                    state.CheckedComplete = nCheckedComplete;

                    History.HistoryManager history = History.HistoryManager.Instance;
                    History.HistorySectionData data = history.AddSectionHistory(section, state.State, state.ProcessDirections, true, state.CheckNotify1, state.CheckNotify2, state.CheckedRun, state.CheckedComplete, state.DetailDatas);
                   
                    if (data != null)
                        state.Time = new VariousData<DateTime>(data.Time);

                    //state.DetailDatas.Clear();
                }

                private void LogState(Section section, SectionState state, StateChangeEventArgs e)
				{
                    state.AccessedUserID = ProxySOP.Instance.SOPGenUserID;

					History.HistoryManager history = History.HistoryManager.Instance;
					Section.ComponentType type = section.GetComponentType();
                    History.HistorySectionData historyData = null;

					if (type == Section.ComponentType.ENDPOINT)
					{
						SectionDataEndPoint sectionData = (SectionDataEndPoint)section.Data;
                        if (state.State == Workstate.State.DONE)
                        {
                            PanelSection panel = (PanelSection)section.GetParent();
                            SectionTabPage tabPage = (SectionTabPage)panel.Parent;

                            // 종료이벤트의 로그는 외부에서 종료 이벤트 수신시 처리하도록 변경
                            // 2014-03-18 skkim
                            //history.AddActionStepHistory(panel.ActionStepID, !tabPage.VirtualMode, sectionData.IsBegin ? WorkFlowState.RUN : WorkFlowState.DONE);
                            
                            if (WorkFlowEvent != null)
                            {
                                // 시작 이벤트만 보낸다.
                                if (sectionData.IsBegin == true)
                                {
                                    WorkFlowEventArgs arg = new WorkFlowEventArgs();
                                    arg.State = WorkFlowState.RUN;                                     
                                    arg.ActionStepID = nActionStepID;
                                    arg.RealMode = !tabPage.VirtualMode;
                                    //arg.Time = this.DetectTime;//DateTime.Now;
                                    arg.NoDBWrite = false;

                                    if (m_option != null && m_option.DetectTime != null)
                                        arg.Time = m_option.DetectTime.Data;
                                    else
                                        arg.Time = DateTime.Now;

                                    WorkFlowEvent(this, arg);
                                }                                
                            }                           
                        }
                        historyData = history.AddSectionHistory(section, state.State, state.ProcessDirections, true, 0, 0, 0, 0, state.DetailDatas);
					}
					else if (type == Section.ComponentType.PROCESS)
					{
						int nCheckedNotify1 = state.CheckNotify1;
						int nCheckedNotify2 = state.CheckNotify2;
                        historyData = history.AddSectionHistory(section, state.State, state.ProcessDirections, true, nCheckedNotify1, nCheckedNotify2, state.CheckedRun, state.CheckedComplete, state.DetailDatas);
					}
					else if (type == Section.ComponentType.TRANSSOP || type == Section.ComponentType.LINK)
					{
                        historyData = history.AddSectionHistory(section, state.State, state.ProcessDirections, true, 0, 0, 0, 0, state.DetailDatas);
					}
					else if (type == Section.ComponentType.DECISION)
					{
                        historyData = history.AddDecisionHistory((SectionDecision)section, state.State, state.ProcessDirections, DecisionNextSection, true);
					}
					else if (type == Section.ComponentType.INTERNAL)
					{
						SectionDataInternal data = (SectionDataInternal)section.Data;
						if (data != null)
						{
							int nCheckedNotify1 = state.CheckNotify1;
                            historyData = history.AddInternalHistory((SectionInternal)section, state.State, state.ProcessDirections, nCheckedNotify1, state.CheckedRun, state.CheckedComplete, data.UsePopupMessage, data.UseMobileApp, data.UseBroadcast, true);
						}
					}
					else if (type == Section.ComponentType.EXTERNAL)
					{
						int nCheckedNotify1 = state.CheckNotify1;
						int nCheckedNotify2 = state.CheckNotify2;
                        historyData = history.AddExternalHistory((SectionExternal)section, state.State, state.ProcessDirections, nCheckedNotify1, nCheckedNotify2, state.CheckedRun, state.CheckedComplete, false, false, true);
					}
					else if (type == Section.ComponentType.TRANSMISSION)
					{
						int nCheckedNotify1 = state.CheckNotify1;
						int nCheckedNotify2 = state.CheckNotify2;
                        historyData = history.AddTransmissionHistory((SectionTransmission)section, state.State, state.ProcessDirections, nCheckedNotify1, nCheckedNotify2, state.CheckedRun, state.CheckedComplete, false, false, false, false, false, true);
					}

                    if (historyData != null)
                        state.Time = new VariousData<DateTime>(historyData.Time);
				}

				private Section mDecision = null;
				public Section Decision
				{
					get { return mDecision; }
					set { mDecision = value; }
				}

				private Section mNextSection = null;
				public Section DecisionNextSection
				{
					get { return mNextSection; }
					set { mNextSection = value; }
				}

				public void SetDecisionSection(Section nextSection)
				{
					mNextSection = nextSection;
				}

				// Process 진행 방향이 맞는지 확인
				private bool CheckProcessDirection(SectionState state, Arrow.ArrowPosition pos)
				{
					// 진행 방향이 설정되어 있지 않으면 무조건 return true
					if (state.ProcessDirections == 0)
						return true;

					ProcessDirectionHistory direction = ProcessButton.ToProcessDirection(pos);

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
						if (arrow.BeginLink == section && CheckProcessDirection(stateCurrent, arrow.BeginPosition))
						{
							Section end = arrow.EndLink;
							if (end.GetComponentType() == Section.ComponentType.LINK)
							{
								SectionLink link = (SectionLink)end;
								SectionDataLink data = (SectionDataLink)link.Data;
								if (data.LinkedSection != null)
									findList.Add(FindState(data.LinkedSection, true));
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
					if (section == null)
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
							SectionDataLink data = (SectionDataLink)link.Data;
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
					foreach (SectionState state in mSectionList.Values)
					{
						state.InitState();
					}
				}
				public bool Start()
				{
                    if (mStartState == null)
                        return false;

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

					if (sectionBegin.GetSectionPainter(0) == null)
						return;

					ProcessButtonManager mgr = (ProcessButtonManager)sectionBegin.GetSectionPainter(0);
					mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.DONE, null, state);
				}

				public bool Done(DateTime time, bool noDBWrite = false)
				{
					if (CheckDisable())
						return false;

					if (nState == WorkFlowState.DONE)
						return true;

					nState = WorkFlowState.DONE;
					IList list = mSectionList.GetValueList();
					foreach (SectionState state in list)
					{
						state.ClearNotify();
					}

					bool bReal = (RunMode == WorkFlowMode.REAL ? true : false);
					if (WorkFlowEvent != null)
					{
						WorkFlowEventArgs arg = new WorkFlowEventArgs();
						arg.State = WorkFlowState.DONE;
						arg.ActionStepID = nActionStepID;
						arg.RealMode = bReal;
						arg.Time = time;
						arg.NoDBWrite = noDBWrite;

						WorkFlowEvent(this, arg);
					}
					return true;
				}

				public bool Stop(DateTime time, bool noDBWrite = false)
				{
					if (CheckDisable())
						return false;

					if (nState == WorkFlowState.STOP)
						return false;

					nState = WorkFlowState.STOP;
					IList list = mSectionList.GetValueList();
					foreach (SectionState state in list)
					{
						state.ClearNotify();
					}
                    
					bool bReal = this.RunMode == WorkFlowMode.REAL;
					if (WorkFlowEvent != null)
					{
						WorkFlowEventArgs arg = new WorkFlowEventArgs();
						arg.State = State;
						arg.ActionStepID = nActionStepID;
						arg.RealMode = bReal;
						arg.Time = time;
						arg.NoDBWrite = noDBWrite;
                        WorkFlowEvent(this, arg);
                    }
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

					if (rhs.mSectionList.Count == mSectionList.Count)
					{
						foreach (SectionState state in mSectionList.Values)
						{
						}
					}
				}
			}

			public class WorkFlowManager
			{
				private Color noramlColor = Color.FromArgb(210, 210, 210);
				public System.Drawing.Color NoramlColor
				{
					get { return noramlColor; }
					set { noramlColor = value; }
				}
				private Color waitColor = Color.FromArgb(210,210,210);
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

                private Color currentColor = Color.FromArgb(255, 0, 0);
                public Color CurrentColor
                {
                    get { return currentColor; }
                    set { currentColor = value; }
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
                            instance = new WorkFlowManager();

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

				public System.Windows.Forms.Form MainForm
				{
					get { return ProxySOP.Instance.InvokeForm; }
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

                    if (ProxySOP.Instance.WorkflowContainer != null)
                    {
                        workflow.WorkFlowEvent -= ProxySOP.Instance.WorkflowContainer.OnWorkflowChanged;
                        workflow.WorkFlowEvent += ProxySOP.Instance.WorkflowContainer.OnWorkflowChanged;
                    } 
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
						//if (work.State == WorkFlowState.STANDBY || work.State == WorkFlowState.RUN || work.State == WorkFlowState.PAUSE)
						{
							SectionState state = work.FindState(section);
							if (state != null)
								return state;
						}
					}
					return null;
				}

                public static Color GetStateColor(State state)
                {
                    switch (state)
                    {
                        case State.DONE:
                            return WorkFlowManager.Instance.CompleteColor;

                        case State.INPUT:
                            return WorkFlowManager.Instance.InputWaitColor;

                        case State.NORMAL:
                            return WorkFlowManager.Instance.NoramlColor;

                        case State.RUN:
                            return WorkFlowManager.Instance.InProgressColor;

                        case State.SKIP:
                            return WorkFlowManager.instance.SkipColor;
                    }

                    return Control.DefaultBackColor;
                }
			}
		}
	}
}
