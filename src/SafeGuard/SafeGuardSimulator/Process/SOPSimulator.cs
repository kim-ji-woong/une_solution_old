using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;
using UnE.SOP.Workstate;
using UnE.SOP.Process;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace SOPManager
{
    public class SOPSimulator : IWorkflowContainer, UnE.SOP.ISOPContainer
    {
        public enum SimulatorMessage { ENABLE_START = 0, DISABLE_START };

        /*protected static Color NoramlColor = Color.FromArgb(210, 210, 210);
        protected static Color WaitColor = Color.FromArgb(210, 210, 210);

        protected static Color CompleteColor = Color.FromArgb(252, 213, 181);
        protected static Color InProgressColor = Color.FromArgb(142, 180, 227);
        protected static Color InputWaitColor = Color.FromArgb(255, 174, 201);
        protected static Color SkipColor = Color.FromArgb(255, 233, 127);
        protected static Color TeamCompleteColor = Color.FromArgb(128, 128, 128);*/

        private PanelSectionEx m_panelCurrent = null;
        private bool m_runningSOP = false;
        //private Dictionary<Section, State> m_dicSectionState = new Dictionary<Section, State>();
        private WorkFlow m_currentWork = null;

        private static SOPSimulator m_instance = null;

        public static SOPSimulator Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new SOPSimulator();

                return m_instance;
            }
        }

        public bool RunningSOP
        {
            get { return m_runningSOP; }
        }

        private SOPSimulator()
        {
            PathNotifier.PathColor = Color.Purple;

            ProcessSectionManager pProcessManager = ProcessSectionManager.Instance;
            pProcessManager.Factory = SOPManager.Process.ProcessSectionFactory.Instance;

            UnE.SOP.ProxySOP.Instance.SOPContainer = this;
        }

        public void OnOpenSOP(PanelSectionEx panel)
        {
            m_panelCurrent = panel;
            m_runningSOP = false;

            //InitSectionStates();
            
            FormMain.Instance.SendMessageToRibbon(SimulatorMessage.ENABLE_START);
        }

        public void Start()
        {
            if (m_panelCurrent == null)
                return;

            /*Section sectionBegin = GetBeginSection();

            if (sectionBegin == null)
                return;

            SetSectionState(sectionBegin, State.INPUT);*/
            WorkFlow work = RunWorkflow();
            m_panelCurrent.HideBeginSectionButton();

            FormMain.Instance.SendMessageToRibbon(SimulatorMessage.DISABLE_START);
            m_runningSOP = true;
        }

        public void Finish()
        {
            if (m_panelCurrent == null)
                return;

            if (m_currentWork != null)
                m_currentWork.Done(DateTime.Now, true);

            FormMain.Instance.SendMessageToRibbon(SimulatorMessage.ENABLE_START);
            m_runningSOP = false;
        }

        private WorkFlow RunWorkflow()
        {
            ActionStepTabPage tabPage = (ActionStepTabPage)m_panelCurrent.Parent;

            if (!WorkFlowManager.Instance.Exist(tabPage.ActionStepID, true))
            {
                AddWorkflow(tabPage);
            }

            WorkFlow work = (WorkFlow)WorkFlowManager.Instance.Get(tabPage.ActionStepID, true);

            if (work != null)
            {
                work.SOPName = SopDocManager.Instance.DisasterName;

                if (work.Start())
                {
                    m_panelCurrent.HideBeginSectionButton();
                }
            }

            SetCurrentWorkflow(work);
            return work;
        }

        private void AddWorkflow(TabPage page)
        {
            int tabId = -1;
            ArrayList arSections = new ArrayList();
            foreach (Control control in page.Controls)
            {
                if (control.GetType() == typeof(Sections.PanelSectionEx))
                {
                    Sections.PanelSectionEx pane = (Sections.PanelSectionEx)control;
                    tabId = pane.ActionStepID;
                    arSections.AddRange(pane.Sections);
                }
            }
            WorkFlowManager manager = WorkFlowManager.Instance;

            ActionStepTabPage tabPage = (ActionStepTabPage)page;
            WorkFlow work = manager.Add(tabPage.ActionStepID, arSections, true);
            work.WorkFlowEvent += this.OnWorkflowChanged;
        }

        /*private void SetSectionState(Section section, State state)
        {
            m_dicSectionState[section] = state;
            section.Shape.SetNotify(false);

            if (state == State.DONE)
                SetSectionColor(section, CompleteColor, Shape.ShapeStatus.PROCESSED);
            else if (state == State.INPUT)
            {
                SetSectionColor(section, WaitColor, Shape.ShapeStatus.WAITING);
                section.Shape.SetNotify(true);
            }
            else if (state == State.NORMAL)
                SetSectionColor(section, NoramlColor, Shape.ShapeStatus.NORMAL);
            else if (state == State.RUN)
                SetSectionColor(section, InProgressColor, Shape.ShapeStatus.PROCESSING);
            else if (state == State.SKIP)
                SetSectionColor(section, SkipColor, Shape.ShapeStatus.SKIPPED);
        }

        private void SetSectionColor(Section section, Color color, Shape.ShapeStatus status)
        {
            Section.ComponentType type = section.GetComponentType();

            if (type == Section.ComponentType.PROCESS)
            {
                SectionProcess psection = (SectionProcess)section;
                psection.SetFillColor(color, true);
                section.SetColor(Section.ColorTarget.FILL, color);
            }
            else
            {
                section.SetColor(Section.ColorTarget.FILL, color);
            }

            section.Shape.Status = status;
        }

        private void InitSectionStates()
        {
            if (m_panelCurrent == null)
                return;

            m_dicSectionState.Clear();

            foreach (Section section in m_panelCurrent.Sections)
            {
                m_dicSectionState[section] = State.NORMAL;
            }
        }

        private Section GetBeginSection()
        {
            foreach (Section section in m_panelCurrent.Sections)
            {
                if (section is SectionEndPoint)
                {
                    SectionDataEndPoint data = (SectionDataEndPoint)section.Data;

                    if (data.IsBegin)
                        return section;
                }
            }

            return null;
        }*/





        #region IWorkflowContainer
        public void ClearProcess()
        {

        }

        public bool CompleteSection(Section section, PanelSection panel)
        {
            panel.CompleteSection(section);
            return true;
        }

        public bool FocusSection(Section section)
        {
            if (section == null)
            {
                if (RunningSOP)
                    return false;
            }

            FormPageSOP page = FormMain.Instance.GetPageLevel();
            page.GetBarComponentDetail().Section = section;

            if (section != null && section is SectionEndPoint)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)section.Data;

                if (data.IsBegin == false)
                    Finish();
            }

            return true;
        }

        public ArrayList GetAllSenario()
        {
            return null;
        }

        public SOPScenario GetCurrentSOPScenario()
        {
            return null;
        }

        public bool IsWorkingMode(Section section)
        {
            return true;
        }

        public bool IsWorkingMode(int nActionStepID, bool bReal)
        {
            return true;
        }

        public void OnLoadScenario(SOPScenario sc)
        {

        }

        protected ArrayList GetNextSections(Section section)
        {
            ArrayList arList = new ArrayList();
            foreach (Arrow arrow in section.Arrows)
            {
                if (arrow.BeginLink == section && arrow.EndLink != null)
                {
                    if (!arList.Contains(arrow.EndLink))
                        arList.Add(arrow.EndLink);
                }
            }
            return arList;
        }

        public void OnWorkflowChanged(object sender, WorkFlowEventArgs args)
        {
            if (args.State == WorkFlowState.RUN)
            {
                WorkFlow work = (WorkFlow)sender;

                if (work.StartState != null)
                {
                    ArrayList sections = GetNextSections(work.StartState.Section);

                    if (sections.Count > 0)
                        FocusSection((Section)sections[0]);
                }
            }
            else if (args.State == WorkFlowState.STOP)
            {
                System.Diagnostics.Trace.WriteLine("Workflow Stop");
            }
            else if (args.State == WorkFlowState.DONE)
            {
                System.Diagnostics.Trace.WriteLine("Workflow Done");
            }
            /*if (sender == null || args == null)
                return;

            // Workflow종료 Event의 HistoryEvent를 기록
            if (args.State == WorkFlowState.STOP)
            {
                UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                bool bSendSMS = workflow.BeginEndEventSendSMS;
                m_pageHome.OnCloseWorkFlow(args.ActionStepID, args.RealMode, args.State);
            }
            if (args.State == WorkFlowState.DONE)
            {
                UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                bool bSendSMS = workflow.BeginEndEventSendSMS;
                TabPageManager.Instance.SetUsePage(args.ActionStepID, false, args.RealMode);
                FormSOP.Instance.DoneWorkflow();
                m_pageHome.OnCloseWorkFlow(args.ActionStepID, args.RealMode, args.State);
            }
            if (args.State == WorkFlowState.RUN)
            {
                ComponentContents contents = m_pageHome.GetCurrentSelectedComponentContents();

                UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                bool bSendSMS = workflow.BeginEndEventSendSMS;
                HistoryManager.Instance.AddActionStepHistory(args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, contents == null ? null : contents.Section, bSendSMS);
            }*/
        }

        public void PostChangeSectionState(Section section, State state)
        {

        }

        public int ReadCurrentActionStep(ref bool bReal)
        {
            return 0;
        }

        public void RunWorkflowWithEvent()
        {

        }

        public void SelectedScenario(int nActionStepID, bool isRealMode)
        {

        }

        public void SetCurrentActionStep(int nActionStepID, bool bReal)
        {

        }

        public void SetCurrentWorkflow(WorkFlow work)
        {
            m_currentWork = work;
        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite = false)
        {

        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode)
        {

        }
        #endregion

        #region ISOPContainer
        public void BeginHistory()
        {
        }

        public void CreateSOPContainer(int nID, string szName, bool isSimulationMode, bool onlySDMS, int nTargetMonitor)
        {
        }

        public void EndHistory()
        {
        }

        public void LinkDisasterSystem(UnE.SOP.IDisasterContainer form)
        {
        }

        public SectionCommander LoadSectionCommander(int nTeamType, int nMemberID, string strDisplayText)
        {
            return null;
        }

        public UnE.SOP.DisasterInfo ReloadDisaster(int nActionStepID)
        {
            return null;
        }

        public void SelectComponent(int nActionStepID, bool isRealMode, Section section)
        {
        }

        #endregion
    }
}
