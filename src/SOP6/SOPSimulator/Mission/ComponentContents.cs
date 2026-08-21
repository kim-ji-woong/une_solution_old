using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Windows.Forms.VisualStyles;
using Sections;
using SDMS;
using DBUtility2;

using UnE.SOP;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;
using UnE.SOP.Log;
using SOPManager.Popup.SpecialMessagePanels;
using System.IO;

namespace SOPMonitoringSystem
{
	public partial class ComponentContents : Form
	{
        public enum RunMode { Ready = 0, Run, Complete, None };

		private bool m_isFlag = false;
		private int m_nComponentHistoryID = -1;
		private int m_nComponentID = -1;
		private State m_state = State.NORMAL;
		private DataLogGridViewRow m_logGridRow = null;
        private UnE.SOP.Process.ProcessSectionIF m_process = null;

        private Sections.SectionCommander m_commander = null;
        private string m_strCommanderName = "", m_strCommanderName2 = "";
        private string m_strCommanderPhoneNumber = "";

        private ArrayList m_receiverPhoneNumbers = null;
        private string m_strReceiverName = "";

        private int m_nAddMargin = -1;

        private DecisionProcessButton m_prevDecisionProcessButton = null;
        private bool m_systemCall = false;

        //private const int MISSION_TITLE_INDEX = 0;
        //private const int TRANS_TYPE_INDEX = 3;
        //private const int BROADCAST_INDEX = 5;
        //private const int DO_IT_INDEX = 5;
        private const int MISSION_PERFORMER_INDEX = 0;
        private const int MISSION_TEXT_INDEX = 1;
        private const int MISSION_TARGET_INDEX = 2;
        private const int SMS_INDEX = 3;
        private const int CONFIRM_COMPLETE_INDEX = 4;
        private const int TIME_INDEX = 5;
        private const int MISSION_ACTOR_INDEX = 6;
        

        private bool m_visibleExternalPanel = false;
        private int m_nGridInitPos = 0;
        private RunMode m_runMode = RunMode.None;

        private string m_strLocation = null;
        private VariousData<DateTime> m_dtDetect = null;
        private string m_strPSMMaterialName = null;
        private VariousData<int> m_psmDistance = null;
        private string m_strAmountSnowfall = null;
        private string m_strAlarmMessage = "";

        private bool m_disabled = false;
        // Key : ComponentHistoryID
        // Value : Accessed SOPGenUser ID
        private Dictionary<int, int> m_dicComponentAccessedUserID = new Dictionary<int, int>();

        private bool m_checkEndState = false;

        // 데이터그리드 이외에 다른 것을 보여주려고 할때 사용한다.
        public bool VisibleExternalPanel
        {
            get { return m_visibleExternalPanel; }
            set
            {
                m_visibleExternalPanel = value;

                if (m_visibleExternalPanel)
                {
                    panelExternal.Location = new Point(panelExternal.Location.X, m_nGridInitPos);
                    dataGridView.Location = new Point(dataGridView.Location.X, -1000);
                }
                else
                {
                    panelExternal.Location = new Point(panelExternal.Location.X, -1000);
                    dataGridView.Location = new Point(dataGridView.Location.X, m_nGridInitPos);
                }
                //panelExternal.Visible = value;
                //dataGridView.Visible = !value;
            }
        }

        public Panel ExternalPanel
        {
            get { return panelExternal; }
        }

        public UnE.SOP.Process.ProcessSectionIF Process
        {
            get { return m_process; }
            set { m_process = value; }
        }

        public string Title
        {
            get { return labelTitle.Text; }
        }

        public DataLogGridViewRow LogGridRow
        {
            get { return m_logGridRow; }
            set { m_logGridRow = value; }
        }

        public bool UseSMS
        {
            get { return btnSMS.Visible; }
        }

        public bool UseBroadcast
        {
            get { return btnBroadcast.Visible; }
        }

        private Sections.Section.ComponentType m_contentsSectionType = Section.ComponentType.NONE;

        public Sections.Section.ComponentType ContentsType
        {
            get
            {
                // changed by mwkim 2015-11-25 Section 객체는 그리드뷰객체가 가지고 있음.

                //if (m_logGridRow == null)
                //    return Sections.Section.ComponentType.NONE;

                //Sections.Section section = m_logGridRow.Section;
                //if (section == null)
                //    return Sections.Section.ComponentType.NONE;

                if (Section == null)
                    return Sections.Section.ComponentType.NONE;
                else
                    return Section.GetComponentType();
            }
        }

        public int ItemCount
        {
            get { return dataGridView.Rows.Count; }
        }

        public Sections.Section Section
        {
            get { return (Sections.Section)dataGridView.Tag; }
            set { dataGridView.Tag = value; }
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        public State State
        {
            get { return m_state; }
            set
            {
                if (m_state != value)
                {
                    m_state = value;

                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        SetStateColor();

                        if (m_state == UnE.SOP.Workstate.State.DONE)
                        {
                            //HideGrid();
                            // 이미 실행이 완료된 ComponentConents 이므로 다시 펼치기 전까진 비활성화 상태로 둔다.
                            EnableNextButton(false);
                            PostDone();
                        }
                        else
                        {
                            if (m_state == UnE.SOP.Workstate.State.RUN)
                            {
                                ShowGrid();
                                // 자동실행 옵션이 있으면 실행한다.
                                AutoRun();
                            }

                            if (dataGridView.SelectedCells.Count == 0 && dataGridView.Rows.Count > 0)
                            {
                                //dataGridView.Rows[0].Selected = true;
                            }
                        }

                        FormSOP.Instance.GetPageHome().RefreshComponentContents(this);
                        //this.Refresh();
                    });
                }
            }
        }

        public bool Disabled
        {
            get { return m_disabled; }
            set
            {
                m_disabled = value;

                this.dataGridView.Enabled = !m_disabled;
                //cboDecisions.Enabled = !m_disabled;
                cboDecisions.Disabled = m_disabled;
                Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();

                if (frm != null)
                    frm.Disabled = m_disabled;
            }
        }

        public bool Checked
        {
            set
            {
                int nRowCount = dataGridView.Rows.Count;

                for (int i=0;i<nRowCount;i++)
                {
                    DataGridViewRow row = dataGridView.Rows[i];
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)row.Cells[CONFIRM_COMPLETE_INDEX];
                    ChangeCompleteCheckBox(checkCell, dataGridView, i, value);
                }

                /*foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)row.Cells[CONFIRM_COMPLETE_INDEX];
                    checkCell.Value = value;
                }*/
            }
        }

        public ComponentContents()
        {
            InitializeComponent();


            FormSOP.SetDoubleBuffer(dataGridView, true);

            //AddGridData();
            pictureBox1.Image = GetImage(true);
            //pictureBox1.Image = GetImage(m_isFlag);
            InitGridHeaders();

            m_nGridInitPos = dataGridView.Location.Y;
            cboDecisions.CanVisible = false;

            labelSender.Text = string.Empty;

            // 실행자(또는 발신자)를 보여줄 것인가?
            ChangeVisiblityToPerformer(FormSOP.Instance.VisiblityToPerformer);
        }

        private void PostDone()
        {
            if (Section == null)
                return;

            Sections.Section.ComponentType type = Section.GetComponentType();

            if (type == Sections.Section.ComponentType.DECISION)
            {
                ProcessButton btn = null;
                int nLimit = 20;

                for (int i = 0; i < nLimit; i++)
                {
                    ISectionPainter painter = Section.GetSectionPainter(i);

                    if (painter == null)
                        break;

                    if (painter is ProcessButtonManager)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)painter;
                        List<DecisionProcessButton> buttons = new List<DecisionProcessButton>();

                        ProcessButton btnLeft = mgr.FindButton(Arrow.ArrowPosition.LEFT);

                        if (btnLeft != null && btnLeft.Status == ProcessButton.ButtonStatus.DONE)
                        {
                            btn = btnLeft;
                            break;
                        }

                        ProcessButton btnRight = mgr.FindButton(Arrow.ArrowPosition.RIGHT);

                        if (btnRight != null && btnRight.Status == ProcessButton.ButtonStatus.DONE)
                        {
                            btn = btnRight;
                            break;
                        }

                        ProcessButton btnBottom = mgr.FindButton(Arrow.ArrowPosition.BOTTOM);

                        if (btnBottom != null && btnBottom.Status == ProcessButton.ButtonStatus.DONE)
                        {
                            btn = btnBottom;
                            break;
                        }

                        ProcessButton btnTop = mgr.FindButton(Arrow.ArrowPosition.TOP);

                        if (btnTop != null && btnTop.Status == ProcessButton.ButtonStatus.DONE)
                        {
                            btn = btnTop;
                            break;
                        }

                        break;
                    }
                }

                if (btn != null)
                    PostRunDecision(btn);
            }
        }

        private void InitGridHeaders()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.FromArgb(200, 255, 255);
            }
        }

        /// <summary>
        /// Grid의 내용 초기화 (시나리오 시작시 이전에 쌓인 로그기록이 남아있으므로 이를 초기화 해준다.)
        /// </summary>
        public void InitGrid()
        {
            switch (m_contentsSectionType)
            {
                    // Process
                case Sections.Section.ComponentType.PROCESS:
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        (row.Cells[3] as DataGridViewDisableButtonCell).Enabled = true;
                        (row.Cells[4] as DataGridViewCheckBoxCell).Value = false;
                        (row.Cells[5] as DataGridViewTextBoxCell).Value = string.Empty;

                        row.Cells[3].Tag = null;
                        row.Cells[4].Tag = null;
                        row.Cells[5].Tag = null;
                    }

                    FormSOP.Instance.GetPageHome().RefreshComponentContents(this);
                    //dataGridView.Refresh();

                    break;
                    // 내부 전파
                case Sections.Section.ComponentType.INTERNAL:

                    foreach (Control ctl in panelExternal.Controls)
                    {
                        if (ctl is Popup.MissionMessage.FormMissionMessage)
                        {
                            (ctl as Popup.MissionMessage.FormMissionMessage).ResetForComponentContent();
                        }
                    }

                    break;
                    // 나머지는 Grid를 사용 안함. 초기화할 내용이 없음.
                default :
                    break;
            }
        }

        public void ShowGrid()
        {
            if (this.Section == null)
                return;

            Sections.Section.ComponentType type = this.Section.GetComponentType();

            if (type == Sections.Section.ComponentType.DECISION ||
                type == Sections.Section.ComponentType.ENDPOINT)
                return;

            m_isFlag = false;
            pictureBox1.Image = GetImage(m_isFlag);
            pictureBox1.Tag = m_isFlag;

            ShowContainer();
            ReSizeForm(m_isFlag);

            if (m_nAddMargin > 0)
                AddMargin(m_nAddMargin);
        }

        private void EnableNextButton(bool enabled)
        {
            btnNext.Enabled = enabled;
        }

        private void ShowContainer()
        {
            if (this.Section == null)
                return;

            Sections.Section.ComponentType type = this.Section.GetComponentType();

            if (type == Sections.Section.ComponentType.DECISION)
                return;

            if (VisibleExternalPanel)
                panelExternal.Show();
            else
                dataGridView.Show();

            // SOP가 실행중이면 ComponentContents를 펼쳤을때 btnNext를 누를수 있도록 변경한다.
            if (FormSOP.Instance.GetPageHome().IsWorkingMode(this.Section))
                EnableNextButton(true);
        }

        private void HideContainer()
        {
            if (VisibleExternalPanel)
                panelExternal.Hide();
            else
                dataGridView.Hide();
        }

		private void pictureBox1_Click(object sender, EventArgs e)
		{
            if (Section == null)
                return;

            Sections.Section.ComponentType type = Section.GetComponentType();

            if (type == Sections.Section.ComponentType.ENDPOINT ||
                type == Sections.Section.ComponentType.DECISION)
                return;
			/*if (label.Text.Contains("시작")) return;
			//if (label.Text.Contains("-")) return;
			if ((label.Text.Length - 1) == label.Text.LastIndexOf('-')) return;*/

			m_isFlag = !m_isFlag;
			pictureBox1.Image = GetImage(m_isFlag);
			pictureBox1.Tag = m_isFlag;

			if (m_isFlag)
				HideContainer();
			else
				ShowContainer();

			ReSizeForm(m_isFlag);
		}

        public void SendLogState(Section section, SectionState state = null, WorkFlow workFlow = null)
        {
            SectionTabPage page = null;

            if (state == null)
            {
                page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;

                if (page == null)
                {
                    PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                    page = (SectionTabPage)panel.Parent;
                    FormSOP.Instance.GetPageHome().TabControls.SelectedTab = page;
                }

                state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);
            }
            
            if (state != null)
            {
                if (workFlow == null)
                {
                    if (page == null)
                    {
                        page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;

                        if (page == null)
                        {
                            PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                            page = (SectionTabPage)panel.Parent;
                            FormSOP.Instance.GetPageHome().TabControls.SelectedTab = page;
                        }
                    }

                    workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);
                }

                if (workFlow != null)
                {
                    //state.DetailDatas.Clear();
                    //int nDefaultID = GetDefaultComponentHistoryID(state.DetailDatas);
                    SetDetailDatas(state.DetailDatas, -1, false);
                    workFlow.LogState(section, state, 0, 0);
                }
            }


        }


        /// <summary>
        /// 사용안함
        /// </summary>
        /// <param name="detailDatas"></param>
        /// <returns></returns>
        private int GetDefaultComponentHistoryID( Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> detailDatas)
        {
            if(detailDatas == null || detailDatas.Count == 0)
                return -1;
        
            int nIdx = detailDatas.Keys.Min();
            if( nIdx >=0 )
                return -1;

            return (nIdx - 1);

        }

        private void EnableButtonCell(DataGridView grid, bool enabled, int nRowIndex, int nColumnIndex)
        {
            DataGridViewDisableButtonCell cell = (DataGridViewDisableButtonCell)grid.Rows[nRowIndex].Cells[nColumnIndex];

            cell.Enabled = enabled;
            cell.ReadOnly = !enabled;
        }

        private MissionItem GetMissionItem(int nRowIndex)
        {
            if (this.Section is SectionProcess)
            {
                SectionDataProcess data = (SectionDataProcess)this.Section.Data;

                if (data.MissionItems.Count > nRowIndex)
                    return (MissionItem)data.MissionItems[nRowIndex];
            }

            return null;
        }

        private void RunTransfer(DataGridView grid, int nRowIndex, int nColumnIndex)
        {
            if (grid.Tag != null && grid.Tag is Section)
            {
                DataGridViewDisableButtonCell cell = (DataGridViewDisableButtonCell)grid.Rows[nRowIndex].Cells[nColumnIndex];

                if (!cell.Enabled)
                    return;

                if (nColumnIndex == SMS_INDEX)
                {
                    MissionItem item = GetMissionItem(nRowIndex);

                    if (item != null && item is MissionItemExternal)
                    {
                        RunExecute((MissionItemExternal)item);
                    }
                    else
                    {
                        if (UnE.SOP.ProxySOP.Instance.ConfirmSendSMS == true)
                        {
                            if (UnE.SOP.ProxySOP.Instance.ConfirmSMSAll == false)
                            {
                                MessageBoxEx msgBox = new MessageBoxEx();
                                msgBox.Text = "문자발송";
                                msgBox.ShowDialog();
                                if (msgBox.DialogResult == System.Windows.Forms.DialogResult.No)
                                    return;

                                if (msgBox.DialogResult == System.Windows.Forms.DialogResult.Ignore)
                                {
                                    UnE.SOP.ProxySOP.Instance.ConfirmSMSAll = true;
                                }
                            }
                        }
                    }

                    // 문자메시지 전송 시간을 Tag에 기록해둔다.
                    // TIME_INDEX에는 완료 시간을 기록한다.
                    cell.Tag = DateTime.Now;
                }
                /*else if (nColumnIndex == BROADCAST_INDEX)
                {
                    if (MessageBox.Show("방송을 실행하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                }*/

                //string strValue = grid.Rows[nRowIndex].Cells[TRANS_TYPE_INDEX].Value.ToString();

                /*if (grid.Rows[nRowIndex].Cells.Count == 5)
                    strValue = grid.Rows[nRowIndex].Cells[MISSION_TEXT_INDEX].Value.ToString();*/
                
                Sections.Section section = (Sections.Section)dataGridView.Tag;
                SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
                if (page == null)
                {
                    PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                    page = (SectionTabPage)panel.Parent;
                    FormSOP.Instance.GetPageHome().TabControls.SelectedTab = page;
                }
                SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);

                if (state == null)
                    return;

                if (state.GetType() == typeof(PSectionState))
                {
                    if (cell.Enabled)
                    {
                        WorkFlow workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

                        if (workFlow != null)
                        {
                            if (state != null)
                            {
                                //int flag = 1 << nRowIndex;
                                //int nCheckedRun = state.CheckedRun | flag;

                                //if (nColumnIndex == SMS_INDEX)
                                //    state.CheckNotify1 = state.CheckNotify1 | flag;
                                ///*else if (nColumnIndex == BROADCAST_INDEX)
                                //    state.CheckNotify2 = state.CheckNotify2 | flag;*/

                                //workFlow.LogState(section, state, nCheckedRun, state.CheckedComplete);

                                if (nColumnIndex == SMS_INDEX)
                                    SendCommanderSMS(grid.Rows[nRowIndex].Cells[MISSION_TEXT_INDEX].Value.ToString());
                                /*else if (nColumnIndex == BROADCAST_INDEX)
                                    RunBroadcast(grid.Rows[nRowIndex].Cells[MISSION_TEXT_INDEX].Value.ToString());*/

                                cell.Enabled = false;

                                SendLogState(section, state, workFlow);
                            }
                        }
                    }
                }

                if (LogGridRow != null)
                {
                    LogGridRow.Cells[5].Tag = state.CheckNotify1;
                    LogGridRow.Cells[6].Tag = state.CheckNotify2;
                }
            }
        }

        public void RunBroadcast(string strMsg, int nPlayCount, bool useSiren)
        {
            if (strMsg.Length == 0)
                return;

            UnE.SOP.TTS.TTSManager.Instance.AddSpeech(strMsg, nPlayCount, useSiren);
            SendLogState(this.Section);
        }

        private void SendCommanderSMS(string strMsg)
        { 
            Section section = (Section)dataGridView.Tag;
            if(section != null)
            {
                ArrayList arrTeamList;
                bool onlyTeamLeader;
                m_receiverPhoneNumbers = GetReceiverInfo(section, out m_strReceiverName, out arrTeamList, out onlyTeamLeader);
         
            }

            string strSenderPhoneNumber = m_strCommanderPhoneNumber;

            if (m_strCommanderPhoneNumber.Length == 0)
            {
                // 발신자 전화번호를 알수없을 경우 Default 전화번호를 사용한다.
                strSenderPhoneNumber = Popup.MissionMessage.FormMissionMessage.GetDefaultSMSCaller();
                //strSenderPhoneNumber = WebDBManager.SMSCaller;
            }

            if (strSenderPhoneNumber.Length == 0 || strMsg.Length == 0 || m_receiverPhoneNumbers == null || m_receiverPhoneNumbers.Count == 0)
                return;

            UnE.SOP.SMS.SMSManager.Instance.SendSMS(m_receiverPhoneNumbers, strSenderPhoneNumber, strMsg);
        }

        /*private bool DoItRow(DataGridView grid, int nRowIndex)
        {
            if (grid.Tag != null && grid.Tag is Section)
            {
                Section section = (Section)grid.Tag;
                DataGridViewDisableButtonCell cell = (DataGridViewDisableButtonCell)grid.Rows[nRowIndex].Cells[DO_IT_INDEX];

                if (cell.Enabled)
                {
                    PanelSection panel = section.GetParent();
                    SectionTabPage page = (SectionTabPage)panel.Parent;

                    WorkFlow workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

                    if (workFlow != null)
                    {
                        SectionState state = workFlow.FindState(section);

                        if (state != null)
                        {
                            int flag = 1 << nRowIndex;
                            int nCheckedRun = state.CheckedRun | flag;
                            workFlow.LogState(section, state, nCheckedRun, state.CheckedComplete);

                            cell.Enabled = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }*/

        bool m_bSelectedGridCellisSMSorComplete = false;
        bool m_bSelectedGridRow = false;
        ArrayList m_arrBeforeSelectedRows = new ArrayList();
        private void dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_bSelectedGridRow = false;
            m_bSelectedGridCellisSMSorComplete = false;

            m_arrBeforeSelectedRows.Clear();

            foreach (DataGridViewCell selectedCell in dataGridView.SelectedCells)
            {
                if (m_arrBeforeSelectedRows.Contains(selectedCell.RowIndex))
                    continue;

                m_arrBeforeSelectedRows.Add(selectedCell.RowIndex);
            }

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;


            DataGridView grid = (DataGridView)sender;
            DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
            if (cell != null)
            {
                DataGridViewRow row = cell.OwningRow;

                if (row.Selected == true)
                    m_bSelectedGridRow = true;

                if (e.ColumnIndex == SMS_INDEX || e.ColumnIndex == CONFIRM_COMPLETE_INDEX)
                {
                    m_bSelectedGridCellisSMSorComplete = true;

                    dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                    dataGridView.MultiSelect = false;
                }

            }

        }

		private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
            if (e.RowIndex < 0)
                return;

            if (this.Disabled)
                return;

            DataGridView grid = (DataGridView)sender;

            // 중복처리로 comment
            if (e.ColumnIndex == CONFIRM_COMPLETE_INDEX)
            {
                if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (checkCell != null)
                    {
                        ChangeCompleteCheckBox(checkCell, grid, e.RowIndex, !Convert.ToBoolean(checkCell.Value));
                    }
                }
            }

            //if (e.RowIndex < 0 || e.ColumnIndex < 0)
            //    return;

            //DataGridView grid = (DataGridView)sender;
            //if (grid.SelectedRows.Count > 0)
            //{
            //    DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
            //    if (cell != null)
            //    {
            //        if (this.Disabled == false)
            //        {
            //            if (e.ColumnIndex == CONFIRM_COMPLETE_INDEX)
            //            {
            //                if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            //                {
            //                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //                    if (checkCell != null)
            //                    {
            //                        ChangeCompleteCheckBox(checkCell, grid, e.RowIndex, !Convert.ToBoolean(checkCell.Value));
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

		}

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (this.Disabled)
                return;

            DataGridView grid = (DataGridView)sender;

            if (e.ColumnIndex == SMS_INDEX/* || e.ColumnIndex == BROADCAST_INDEX*/)
            {
                RunTransfer(grid, e.RowIndex, e.ColumnIndex);
            }
            else if (e.ColumnIndex == CONFIRM_COMPLETE_INDEX)
            {
                if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (checkCell == null)
                        return;

                    //m_bChangeCellClicked = true;
                   // ChangeCompleteCheckBox(checkCell, grid, e.RowIndex, (bool)checkCell.EditedFormattedValue);
                    //m_bChangeCellClicked = false;
                }
            }

        }

        private void dataGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView_MouseCaptureChanged(sender, e);
        }

        private void dataGridView_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (m_bSelectedGridCellisSMSorComplete == true)
            {
                dataGridView.ClearSelection();
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.MultiSelect = true;

                int nIndexCount = 0;

                foreach (int nRowIndex in m_arrBeforeSelectedRows.Cast<int>().ToArray())
                {
                    nIndexCount++;

                    if (dataGridView.Rows.Count > nRowIndex)
                    {
                        if (m_arrBeforeSelectedRows.Count == nIndexCount)
                            m_bSelectedGridCellisSMSorComplete = false;

                        dataGridView.Rows[nRowIndex].Selected = true;
                    }
                }
            }

            FormSOP.Instance.GetPageHome().ClearSelectComponentContentsExclude(this);
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            Point pt = dataGridView.PointToClient(Cursor.Position);
            DataGridView.HitTestInfo hitInfo = dataGridView.HitTest(pt.X, pt.Y);

            ArrayList arrSelectedRows = new ArrayList();

            foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            {
                if (m_bSelectedGridRow == false)
                {
                    if (cell.RowIndex == hitInfo.RowIndex)
                    {
                        if (hitInfo.ColumnIndex == SMS_INDEX || hitInfo.ColumnIndex == CONFIRM_COMPLETE_INDEX)
                        {
                            continue;
                        }
                    }
                }

                if (arrSelectedRows.Contains(cell.RowIndex))
                    continue;

                arrSelectedRows.Add(cell.RowIndex);
            }

            if (FormSOP.Instance.HasControl == false || m_bSelectedGridCellisSMSorComplete == false)
            {
                FormSOP.Instance.FrmMain3.SelectRows(arrSelectedRows, this);
            }

            int nCount = arrSelectedRows.Count;
            if (nCount > 0)
            {
                DataGridView grid = (DataGridView)sender;

                int nRowIdx = (int)arrSelectedRows[nCount - 1];
                //if (grid.Rows[nRowIdx].Cells.Count == 5)
                {
                    DataGridViewRow row = grid.Rows[nRowIdx];

                    DataGridViewCell cell1 = row.Cells[MISSION_TEXT_INDEX];
                    string szMissionText = (string)cell1.Value;

                    DataGridViewCell cell2 = row.Cells[MISSION_TARGET_INDEX];
                    string szToTarget = (string)cell2.Value;

                    DataGridViewCell cell3 = row.Cells[MISSION_ACTOR_INDEX];
                    string szSender = (string)cell3.Value;

                    DataGridViewCell cell4 = row.Cells[MISSION_PERFORMER_INDEX];
                    string szTextPerformer = (string)cell4.Value;

                    //string szValueMedium = (string)row.Cells[TRANS_TYPE_INDEX].Value;

                    if (szMissionText == null)
                        szMissionText = "";
                    if (szToTarget == null)
                        szToTarget = "";

                    // 임무 상세창 감춤
                    // changed by mwkim 2015-10-06 임무 상세창 다시 팝업되도록 주석 해제
                    if (m_runMode == RunMode.Run)
                    {
                        if (FormSOP.Instance.HasControl == true)
                        {
                            bool bNoPop = false;

                            // 문자메시지 발송 버튼 / 완료 체크박스를 담은 Cell을 클릭하였을 경우, 팝업되어있는 임무상세창이 없으면 액션 미실행.
                            //if (dataGridView.CurrentCell.ColumnIndex == SMS_INDEX
                            //    || dataGridView.CurrentCell.ColumnIndex == CONFIRM_COMPLETE_INDEX)
                            //{
                            //    bNoPop = true;
                            //}

                            PopupMissionText form = PopupMissionText.Instance;

                            if (bNoPop == false || form.Visible == true)
                            {
                                form.SetText(szMissionText, szToTarget, ""/*szValueMedium*/, szSender, szTextPerformer, this.Section);
                            }
                        }
                    }

                    if (FormSOP.Instance.HasControl == true && m_bSelectedGridCellisSMSorComplete == false)
                    {
                        if (this.ContentsType == Sections.Section.ComponentType.PROCESS)
                        {
                            // changed by mwkim 2015-11-25 다중행을 선택할 수 있도록 ','로 구분하여 문자열로 처리
                            //int nRow = nRowIdx;

                            string strRowIndex = String.Join(",", arrSelectedRows.Cast<int>().ToArray());
                            //int nComponentHistory = this.m_nComponentHistoryID;
                            int nComponentID = this.ComponentID;

                            if (Section != null && Section.GetParent() != null)
                            {
                                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)this.Section.GetParent();
                                SectionTabPage tabPage = (SectionTabPage)panel.Parent;
                                if (tabPage != null)
                                {
                                    int nRealMode = ((!tabPage.VirtualMode) == true) ? 1 : 0;
                                    int nActionStepID = tabPage.ActionStepID;

                                    if (WorkFlowManager.Instance.Get(nActionStepID, (nRealMode == 1 ? true : false)) != null)
                                    {
                                        WorkFlow work = WorkFlowManager.Instance.Get(nActionStepID, (nRealMode == 1 ? true : false));
                                        if (work.State == WorkFlowState.RUN)
                                        {
                                            if (nActionStepID > 0 && this.Visible == true)
                                            {
                                                FormSOP.Instance.NetworkManager.SendSelectMission(nActionStepID, nRealMode, nComponentID, strRowIndex);
                                                FormSOP.Instance.GetPageHome().OnCurrentSelectedMission(nActionStepID, nRealMode, nComponentID, strRowIndex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        private void ChangeCompleteCheckBox(DataGridViewCheckBoxCell checkCell, DataGridView grid, int iRowIndex, bool isChecked)
        {
            checkCell.Value = isChecked;

            //System.Diagnostics.Trace.WriteLine(DateTime.Now);

            grid.EndEdit();

            if (isChecked)
            {
                DateTime time = DateTime.Now;
                grid.Rows[iRowIndex].Cells[TIME_INDEX].Value = GetTimeString(time);
                grid.Rows[iRowIndex].Cells[TIME_INDEX].Tag = time;

                // 완료버튼이 눌려졌다.
                checkCell.Tag = null;
            }
            else
            {
                // TIME_INDEX Cell에는 완료된 시점의 시간을 남겨두고
                // 완료해제된 시간은 row.Tag에 넣어둔다.
                grid.Rows[iRowIndex].Tag = DateTime.Now;

                // 완료버튼이 해제되었다.
                checkCell.Tag = false;
            }

            EnableButtonCell(grid, !isChecked, iRowIndex, SMS_INDEX);
            SendLogState(this.Section);

            PostCompleteChecked();

            FormSOP.Instance.GetPageHome().RefreshComponentContents(this);
            //dataGridView.Refresh();

            //System.Diagnostics.Trace.WriteLine(DateTime.Now);
        }

        // 모든 완료 버튼이 Checked 상태일 경우 [다음] 버튼을 누른것과 같은 효과를 내도록 한다.
        private void PostCompleteChecked()
        {
            // 제어권이 없을 경우에는 신경쓰지 않는다.
            if (!FormSOP.Instance.HasControl)
                return;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[CONFIRM_COMPLETE_INDEX].Value == null)
                    return;

                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)row.Cells[CONFIRM_COMPLETE_INDEX];

                if ((bool)checkCell.EditedFormattedValue == false)
                    return;
            }

            ClickNextButtonIfItisEnabled();
        }

        public void ClickNextButtonIfItisEnabled()
        {
            // [다음] 버튼이 Enabled가 아니면 누를수 없다.
            if (!btnNext.Enabled)
                return;

            btnNext_Click(null, null);
        }

        public void SelectRow(string strRowIndex)
        {
            string[] arrRowindex = strRowIndex.Split(',');

            if (arrRowindex.Length < 1)
                return;

            gridView.ClearSelection();
            m_bSelectedGridRow = false;

            int nRowIndex = -1;

            foreach (string strindex in arrRowindex)
            {
                if (int.TryParse(strindex, out nRowIndex))
                {
                    if (gridView.RowCount <= nRowIndex || nRowIndex < 0)
                        continue;
                    else
                        gridView.Rows[nRowIndex].Selected = true;
                }
            }
        }


        private DateTime m_ExecTime;
        public DateTime ExecTime
        {
            get { return m_ExecTime; }
            set { m_ExecTime = value; }
        }

		public void SetTitle(string strTitle, DateTime time, string strStatus)
		{
            m_ExecTime = time;
            //label.Text = String.Format("{0:MM'/'dd tt h':'mm':'ss} / {1} / {2}", time, strTitle, strStatus);

            if (m_strLocation != null && m_dtDetect != null)
                labelTitle.Text = Parse(strTitle, m_dtDetect, m_strLocation, m_strPSMMaterialName, m_psmDistance, m_strAmountSnowfall, m_strAlarmMessage);
            else
                labelTitle.Text = strTitle;
		}

		public string GetTitle()
		{
			return labelTitle.Text;
		}

		public void ChangeTitle(string strTitle)
		{
            if (m_strLocation != null && m_dtDetect != null)
                labelTitle.Text = Parse(strTitle, m_dtDetect, m_strLocation, m_strPSMMaterialName, m_psmDistance, m_strAmountSnowfall, m_strAlarmMessage);
            else
                labelTitle.Text = strTitle;
		}

        public void ChangeTitle()
        {
            if (Section == null)
                return;

            Section.ComponentType type = Section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL)
            {
                SectionDataInternal data = (SectionDataInternal)Section.Data;

                string[] arrHeadTitle = { "(문자)", "(문자전파)", "(방송)", "(방송전파)" };
                string strSMS = "(문자)", strBroadcast = "(방송)";
                string strNumber, strTitle;

                GetLabelText(labelTitle, out strNumber, out strTitle);

                foreach (string strHeadTitle in arrHeadTitle)
                {
                    if (strTitle.StartsWith(strHeadTitle))
                    {
                        strTitle = strTitle.Replace(strHeadTitle, "").Trim();
                        break;
                    }
                }
                
                if (data.UseBroadcast)
                    labelTitle.Text = strNumber + strBroadcast + strTitle;
                else if (data.UseMobileApp)
                    labelTitle.Text = strNumber + strSMS + strTitle;
            }
        }

        private void GetLabelText(Label label, out string strNumber, out string strTitle)
        {
            strNumber = "";
            strTitle = label.Text;

            int nDotIndex = label.Text.IndexOf('.');

            if (nDotIndex <= 0)
                return;

            string str = label.Text.Substring(0, nDotIndex);

            int num;

            if (!int.TryParse(str, out num))
                return;

            int len = label.Text.Length;

            for (int i=nDotIndex+1;i<len;i++)
            {
                char ch = label.Text.ElementAt(i);

                if (i > nDotIndex + 1 || (ch != ' ' && ch != '\t'))
                {
                    strNumber = label.Text.Substring(0, i);
                    strTitle = label.Text.Substring(i);
                    break;
                }
            }
        }

		public void HideGrid()
		{
            HideContainer();
			m_isFlag = true;
			ReSizeForm(m_isFlag);
		}

        // SOP 실행전에 모든 ComponentContents를 접고, 실행버튼들은 Disable 시킨다.
        public void Ready(bool hideGrid = true)
        {
            ChangeTitle();

            if (hideGrid)
                HideGrid();

            btnExecute.Enabled = false;

            if (btnNext.Text != "시작")
                btnNext.Enabled = false;
            else
                btnNext.Enabled = true;

            dataGridView.Enabled = false;

            Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();

            if (frm != null)
                frm.Init();

            m_runMode = RunMode.Ready;
        }

        private Popup.MissionMessage.FormMissionMessage GetFormMissionMessage()
        {
            foreach (Control ctrl in panelExternal.Controls)
            {
                if (ctrl is Popup.MissionMessage.FormMissionMessage)
                {
                    Popup.MissionMessage.FormMissionMessage frm = (Popup.MissionMessage.FormMissionMessage)ctrl;
                    return frm;
                }
            }

            return null;
        }

        // SOP가 종료 또는 취소되었다.
        public void Complete()
        {
            Ready(false);
            m_runMode = RunMode.Complete;
        }

        // SOP 실행상태로 만든다.
        // dtDetect : 재난발생시각
        // strLocation : 재난발생위치
        public void Start(WorkflowOption option, bool isRealMode)
        //public void Start(VariousData<DateTime> dtDetect, string strLocation, string strBroadcastLocationName, string strPSMMaterialName, VariousData<int> psmDistance, string strAmountSnowfall)
        {
            if (this.Section == null)
                return;

            PreStart(option, isRealMode);

            Section.ComponentType type = this.Section.GetComponentType();

            btnExecute.Enabled = true;

            if (btnNext.Text != "시작" && btnNext.Text != "종료")
            {
                if (type == Sections.Section.ComponentType.DECISION)
                    HideGrid();
                else
                    ShowGrid();

                btnNext.Enabled = true;
            }
            else
            {
                HideGrid();

                if (btnNext.Text == "시작")
                    btnNext.Enabled = false;
                else
                    btnNext.Enabled = true;
            }

            dataGridView.Enabled = true;
            dataGridView.ClearSelection();
            m_runMode = RunMode.Run;

            m_checkEndState = false;
            m_state = UnE.SOP.Workstate.State.NORMAL;
            SetStateColor();

            ResetUserDefinedTeamNames();
        }

        public void ResetUserDefinedTeamNames(SectionTabPage page = null)
        {
            if (page == null)
            {
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)this.Section.GetParent();

                if (panel == null)
                    return;

                page = (SectionTabPage)panel.Parent;
            }

            List<Data_UserDefinedTeam> userDefinedTeams = page.GetUsingUserDefineTeams();

            if (userDefinedTeams.Count == 0)
                return;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                DataGridViewCell cell = row.Cells[MISSION_TARGET_INDEX];

                if (cell.Tag == null)
                    continue;

                ResetUserDefinedTeamNames(cell, userDefinedTeams);
            }
        }

        private void ResetUserDefinedTeamNames(DataGridViewCell cell, List<Data_UserDefinedTeam> userDefinedTeams)
        {
            string strValue = cell.Tag.ToString();

            if (strValue.Length == 0)
                return;

            cell.Value = cell.Tag;

            foreach (Data_UserDefinedTeam team in userDefinedTeams)
            {
                if (team.Tag != null && team.Tag is DataRoleMember)
                {
                    DataRoleMember roleMember = (DataRoleMember)team.Tag;
                    //string memberName = (string)team.Tag;

                    if (roleMember.MemberName.Length == 0)
                        continue;

                    strValue = strValue.Replace(team.TeamName, team.TeamName + "(" + roleMember.MemberName + ")");
                    cell.Value = strValue;
                    cell.ToolTipText = strValue;
                }
            }
        }

        // dtDetect : 재난발생시각
        // strLocation : 재난발생위치
        private void PreStart(WorkflowOption option, bool isRealMode)
        //private void PreStart(DBUtility.VariousData<DateTime> dtDetect, string strLocation, string strBroadcastLocationName, string strPSMMaterialName, VariousData<int> psmDistance, string strAmountSnowfall)
        {
            if (Section == null)
                return;

            VariousData<DateTime> dtDetect = null;
            string strLocation = "", strBroadcastLocationName = "", strPSMMaterialName = "", strAmountSnowfall = "";
            VariousData<int> psmDistance = null;
            string strAlarmMessage = "";

            if (option != null)
            {
                dtDetect = option.DetectTime;
                strLocation = option.PositionName;
                strBroadcastLocationName = option.BroadcastPositionName;
                strAlarmMessage = option.AlarmMessage;

                if (option is WorkflowOptionPSM)
                {
                    WorkflowOptionPSM optionPSM = (WorkflowOptionPSM)option;

                    if (optionPSM.PSMMaterial != null)
                        strPSMMaterialName = optionPSM.PSMMaterial.MaterialName;

                    psmDistance = new VariousData<int>(optionPSM.PSMDistance);
                }
                else if (option is WorkflowOptionSnowFall)
                {
                    WorkflowOptionSnowFall optionSnow = (WorkflowOptionSnowFall)option;

                    if (optionSnow.UseAmountSnowFall && optionSnow.AmountSnowFall > 0.0)
                        strAmountSnowfall = optionSnow.AmountSnowFall.ToString();
                }
            }

            if (this.Section != null)
            {
                if (this.Section is SectionDecision)
                    ChangeDecisionExpression(option, (SectionDecision)this.Section);
            }

            ChangeTitle();
            labelTitle.Text = Parse(labelTitle.Text, dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall, strAlarmMessage);

            ChangeCommanderName();
            ChangeGrid(option, isRealMode);
            //ChangeGrid(dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall);

            foreach (Control ctrl in panelExternal.Controls)
            {
                if (ctrl is Popup.MissionMessage.FormMissionMessage)
                {
                    Popup.MissionMessage.FormMissionMessage frm = (Popup.MissionMessage.FormMissionMessage)ctrl;
                    frm.Option = option;
                    frm.SetStartTime(dtDetect);

                    if (strLocation != null)
                        frm.SetLocation(strLocation, strBroadcastLocationName);

                    if (strPSMMaterialName != null)
                        frm.PSMMaterial = strPSMMaterialName;

                    if (psmDistance != null)
                        frm.PSMDistance = psmDistance.Data;

                    if (strAmountSnowfall != null)
                        frm.AmountSnowfall = strAmountSnowfall;

                    if (strAlarmMessage != null)
                        frm.AlarmMessage = strAlarmMessage;

                    frm.RunMode = true;
                }
            }
        }

        private void ChangeDecisionExpression(WorkflowOption option, SectionDecision section)
        {
            if (option == null)
                return;

            SectionDataDecision data = (SectionDataDecision)section.Data;

            if (data.ExpressionOrigin == null || data.ExpressionOrigin.Length == 0)
                return;

            if (option is WorkflowOptionEarthquake)
            {
                data.Expression = ChangeEarthquakeString(data.ExpressionOrigin, (WorkflowOptionEarthquake)option);
            }
            else if (option is WorkflowOptionPSM)
            {
                data.Expression = ChangePSMString(data.ExpressionOrigin, (WorkflowOptionPSM)option);
            }
            else if (option is WorkflowOptionSnowFall)
            {
                data.Expression = ChangeClimateString(data.ExpressionOrigin, (WorkflowOptionSnowFall)option);
            }
            else if (option != null)
            {
                data.Expression = ChangeCommonString(data.ExpressionOrigin, option);
            }

            /*if (data.Expression == null || data.Expression.Length == 0)
                return;

            if (option is WorkflowOptionEarthquake)
            {
                data.Expression = ChangeEarthquakeString(data.Expression, (WorkflowOptionEarthquake)option);
            }*/
        }

        public static string ChangeCommonString(string str, WorkflowOption option)
        {
            if (option.DetectTime != null)
            {
                string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", option.DetectTime.Data.Year, option.DetectTime.Data.Month, option.DetectTime.Data.Day, option.DetectTime.Data.Hour, option.DetectTime.Data.Minute, option.DetectTime.Data.Second);
                str = str.Replace(FormSpecialMessageHelpTime.GetVariableString(FormSpecialMessageHelpTime.VariableType.Time), strTime);
            }

            if (option.HasPosition)
            {
                str = str.Replace(FormSpecialMessageHelpLocation.GetVariableString(FormSpecialMessageHelpLocation.VariableType.Location), "'" + option.PositionName + "'");
            }

            if (option.WorkFlow != null)
            {
                string strSOPMode = "", strSOPFullMode = "";
                int nRealMode = option.WorkFlow.RunMode == WorkFlowMode.REAL ? 1 : 0;

                if (UnE.SOP.Utility.SOPSimulatorScript.GetSOPModeString(ref strSOPMode, nRealMode))
                {
                    str = str.Replace(FormSpecialMessageHelpSOPMode.GetVariableString(FormSpecialMessageHelpSOPMode.VariableType.SOPMode), "'" + strSOPMode + "'");
                }

                if (UnE.SOP.Utility.SOPSimulatorScript.GetSOPFullModeString(ref strSOPFullMode, nRealMode))
                {
                    str = str.Replace(FormSpecialMessageHelpSOPMode.GetVariableString(FormSpecialMessageHelpSOPMode.VariableType.SOPFullMode), "'" + strSOPFullMode + "'");
                }
            }

            ChangeUserDefinedString(ref str, option);
            return str;
        }

        private static void ChangeUserDefinedString(ref string str, WorkflowOption option)
        {
            if (option.UserDefinedParameters.Count > 0)
            {
                int nBeginIndex = 0;
                int nIndex = str.IndexOf('{', nBeginIndex);

                while (nIndex >= 0)
                {
                    int nIndex2 = str.IndexOf('}', nIndex + 1);

                    if (nIndex2 < 0)
                        break;

                    string strVariable = str.Substring(nIndex + 1, nIndex2 - nIndex - 1);
                    string strVariable2 = strVariable.Trim();

                    bool find = false;

                    foreach (KeyValuePair<SOPParameter, string> pair in option.UserDefinedParameters)
                    {
                        if (string.Compare(strVariable2, pair.Key.VariableName, true) == 0)
                        {
                            if (pair.Key.Type == SectionDataDecision.VariableType.STRING)
                                str = str.Replace("{" + strVariable + "}", "'" + pair.Value + "'");
                            else
                                str = str.Replace("{" + strVariable + "}", pair.Value);

                            nBeginIndex = nIndex + pair.Value.Length;
                            find = true;
                            break;
                        }
                    }

                    if (find == false)
                        nBeginIndex = nIndex2 + 1;

                    nIndex = str.IndexOf('{', nBeginIndex);
                }
            }

            ChangeBooleanType(ref str);
            /*string[] tokens = str.Split(new char[] { ' ', '\t' });
            str = "";

            foreach (string strToken in tokens)
            {
                if (str.Length > 0)
                    str += " ";

                if (strToken == "참" || string.Compare(strToken, "true", true) == 0)
                    str += "1";
                else if (strToken == "거짓" || string.Compare(strToken, "false", true) == 0)
                    str += "0";
                else
                    str += strToken;
            }*/
        }

        private static void ChangeBooleanType(ref string str)
        {
            string[] keys = new string[] { "참", "true", "거짓", "false" };
            string[] values = new string[] { "1", "1", "0", "0" };
            string strLower = str.ToLower();

            for (int i=0;i<keys.Count();i++)
            {
                int nBeginIndex = 0;

                while (nBeginIndex < str.Length)
                {
                    int nIndex = FindExpressionWordIndex(strLower, keys[i], nBeginIndex);

                    if (nIndex >= 0)
                    {
                        str = str.Substring(0, nIndex) + values[i] + str.Substring(nIndex + keys[i].Length);
                        strLower = str.ToLower();
                        nBeginIndex = nIndex + keys[i].Length;
                    }
                    else
                        break;
                }
            }
        }

        private static int FindExpressionWordIndex(string str, string strWord, int nBeginIndex)
        {
            int nIndex = str.IndexOf(strWord, nBeginIndex);

            if (nIndex < 0)
                return -1;

            char chBegin = (char)0;
            char chEnd = (char)0;
            int nWordLen = strWord.Length;

            if (nIndex > 0)
                chBegin = str.ElementAt(nIndex - 1);

            if (nIndex + nWordLen < str.Length)
                chEnd = str.ElementAt(nIndex + nWordLen);

            if (!CheckExpressionCharacter(chBegin) || !CheckExpressionCharacter(chEnd))
                return -1;

            return nIndex;
        }

        private static bool CheckExpressionCharacter(char ch)
        {
            if (ch == (char)0 || ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n' || ch == '<' ||
                ch == '>' || ch == '=' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '(' || ch == ')')
                return true;

            return false;
        }

        public static string ChangeEarthquakeString(string str, WorkflowOptionEarthquake option)
        {
            // 지진규모를 입력한다.
            str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Magnitude), option.Magnitude.ToString());
            // 지진진도를 입력한다.
            str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Intensity), option.Intensity.ToString());
            // 규모
            /*if (option.Mode == WorkflowOptionEarthquake.PowerMode.Magnitude)
            {
                // 지진규모를 입력한다.
                str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Magnitude), option.Magnitude.ToString());
            }
            // 진도
            else if (option.Mode == WorkflowOptionEarthquake.PowerMode.Intensity)
            {
                // 지진진도를 입력한다.
                str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Intensity), option.Intensity.ToString());
            }*/

            str = ChangeCommonString(str, option);
            return str;
        }

        public static string ChangePSMString(string str, WorkflowOptionPSM option)
        {
            if (option.PSMMaterial != null)
            {
                str = str.Replace(FormSpecialMessageHelpPSM.GetVariableString(FormSpecialMessageHelpPSM.VariableType.PSMMaterial), option.PSMMaterial.MaterialName);
            }

            // option.PSMDistance는 미터
            str = str.Replace(FormSpecialMessageHelpPSM.GetVariableString(FormSpecialMessageHelpPSM.VariableType.PSMDistanceM), option.PSMDistance.ToString());
            str = str.Replace(FormSpecialMessageHelpPSM.GetVariableString(FormSpecialMessageHelpPSM.VariableType.PSMDistanceKM), string.Format("{0:F1}", option.PSMDistance / 1000.0));

            str = ChangeCommonString(str, option);
            return str;
        }

        public static string ChangeClimateString(string str, WorkflowOptionSnowFall option)
        {
            if (option.UseAmountSnowFall)
            {
                str = str.Replace(FormSpecialMessageHelpClimate.GetVariableString(FormSpecialMessageHelpClimate.VariableType.SNOW_DEPTH), string.Format("{0:F0}", option.AmountSnowFall));
            }

            str = ChangeCommonString(str, option);
            return str;
        }

        private string ChangeProcessMissionText(WorkflowOption option, string strMissionText, bool isRealMode)
        {
            if (option == null)
                return strMissionText;

            SectionDataProcess data = (SectionDataProcess)this.Section.Data;

            if (option is WorkflowOptionEarthquake)
            {
                strMissionText = ChangeEarthquakeString(strMissionText, (WorkflowOptionEarthquake)option);
            }
            else if (option is WorkflowOptionPSM)
            {
                strMissionText = ChangePSMString(strMissionText, (WorkflowOptionPSM)option);
            }
            else if (option is WorkflowOptionSnowFall)
            {
                strMissionText = ChangeClimateString(strMissionText, (WorkflowOptionSnowFall)option);
            }
            else if (option != null)
            {
                strMissionText = ChangeCommonString(strMissionText, option);
            }

            foreach (MissionItem item in data.MissionItems)
            {
                if (item is MissionItemExternal)
                {
                    MissionItemExternal _item = (MissionItemExternal)item;
                    int nArgumentCount = _item.Arguments.Count;

                    if (strMissionText == _item.Mission)
                    {
                        // 주석이 있으면 주석을 표시한다.
                        if (_item.Description.Length > 0)
                            strMissionText = _item.Description;
                    }

                    for (int i = 0; i < nArgumentCount; i++)
                    {
                        if (option is WorkflowOptionEarthquake)
                        {
                            _item.Arguments[i] = ChangeEarthquakeString(_item.OriginalArguments[i], (WorkflowOptionEarthquake)option);
                        }
                        else if (option is WorkflowOptionPSM)
                        {
                            _item.Arguments[i] = ChangePSMString(_item.OriginalArguments[i], (WorkflowOptionPSM)option);
                        }
                        else if (option is WorkflowOptionSnowFall)
                        {
                            _item.Arguments[i] = ChangeClimateString(_item.OriginalArguments[i], (WorkflowOptionSnowFall)option);
                        }
                        else if (option != null)
                        {
                            _item.Arguments[i] = ChangeCommonString(_item.OriginalArguments[i], option);
                        }
                    }
                }
            }

            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMissionText, option.DetectTime == null ? DateTime.Now : option.DetectTime.Data, option.PositionName, option.AlarmMessage);
            param.RealMode = isRealMode ? 1 : 0;
            strMissionText = UnE.SOP.Utility.SOPSimulatorScript.Parse(param);

            return strMissionText;
        }

        private void ChangeGrid(WorkflowOption option, bool isRealMode)
        //private void ChangeGrid(VariousData<DateTime> dtDetect, string strLocation, string strPSMMaterialName, VariousData<int> psmDistance, string strAmountSnowfall)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[MISSION_TEXT_INDEX].Value == null)
                    continue;

                string strMissionText = "";

                if (row.Cells[MISSION_TEXT_INDEX].Tag != null)
                    strMissionText = row.Cells[MISSION_TEXT_INDEX].Tag.ToString();
                else if (row.Cells[MISSION_TEXT_INDEX].Value != null)
                    strMissionText = row.Cells[MISSION_TEXT_INDEX].Value.ToString();

                row.Cells[MISSION_TEXT_INDEX].Value = ChangeProcessMissionText(option, strMissionText, isRealMode);
                //row.Cells[MISSION_TEXT_INDEX].Value = ChangeProcessMissionText(option, row.Cells[MISSION_TEXT_INDEX].Value.ToString(), isRealMode);
                //row.Cells[MISSION_TEXT_INDEX].Value = Parse(row.Cells[MISSION_TEXT_INDEX].Value.ToString(), dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall);
            }
        }

        private string Parse(string strMsg, VariousData<DateTime> dtDetect, string strLocation, string strPSMMaterialName, VariousData<int> psmDistance, string strAmountSnowfall, string strAlarmMessage)
        {
            m_strLocation = strLocation;
            m_dtDetect = dtDetect;
            m_strPSMMaterialName = strPSMMaterialName;
            m_psmDistance = psmDistance;
            m_strAmountSnowfall = strAmountSnowfall;
            m_strAlarmMessage = strAlarmMessage;

            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMsg, dtDetect == null ? DateTime.Now : dtDetect.Data, strLocation, strAlarmMessage);

            if (strPSMMaterialName != null && strPSMMaterialName.Length > 0)
            {
                param.PSMMaterialType = strPSMMaterialName;
                param.PSMDistance = psmDistance.Data;
            }

            if (strAmountSnowfall != null && strAmountSnowfall.Length > 0)
                param.AmountSnowfall = strAmountSnowfall;

            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        public SectionState GetSectionState()
        {
            if (this.Section == null)
                return null;

            SectionTabPage page = (SectionTabPage)this.Section.GetParent().Parent;
            WorkFlow work = (WorkFlow)WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

            if (work == null)
                return null;

            return work.FindState(this.Section);
        }

        public void ChangeVisiblityToPerformer(bool isVisible)
        {
            if (dataGridView == null)
                return;

            if (dataGridView.Columns.Count <= MISSION_PERFORMER_INDEX + 1)
                return;

            dataGridView.Columns[MISSION_PERFORMER_INDEX].Visible = isVisible;

            Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();

            if (frm != null)
            {
                frm.ChangeVisiblityToPerformer(isVisible);
            }

            //int nPerpormerColumnWidth = dataGridView.Columns[MISSION_PERFORMER_INDEX].Width;

            //dataGridView.Columns[MISSION_PERFORMER_INDEX].Visible = isVisible;

            //if (dataGridView.Columns.Count <= MISSION_TEXT_INDEX + 1)
            //    return;

            //if (isVisible)
            //{
            //    dataGridView.Columns[MISSION_TEXT_INDEX].Width -= nPerpormerColumnWidth;
            //}
            //else
            //{
            //    dataGridView.Columns[MISSION_TEXT_INDEX].Width += nPerpormerColumnWidth;
            //}
        }

        public void ChangeCommanderName()
        {
            string strCommanderName = string.Empty;
            SectionState state = GetSectionState();

            bool bChangeActor = false;
            bool bChangePerformer = false;

            if (state == null)
                return;

            if (Section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
            {
                if (m_commander != null)
                {
                    labelSender.Text = String.Format("( 발신자 : {0} )", m_commander.DisplayText);

                    // m_commander.Team이 null이 아닐경우 CommanderName이 바뀌는 일은 없다.
                    if (m_commander.Team == null)
                    {
                        bChangeActor = true;
                    }
                }

                LoadComponentAccessedUsers(state);

                int nRowCount = dataGridView.Rows.Count;
                ArrayList arrMissionItem = ((dataGridView.Tag as SectionProcess).Data as Sections.SectionDataProcess).MissionItems;

                for (int i = 0; i < nRowCount; i++)
                {
                    bChangePerformer = false;

                    string strUserName = null;
                    DataGridViewRow row = dataGridView.Rows[i];
                    MissionItem missionItem = arrMissionItem[i] as MissionItem;

                    if (missionItem.Commander != null)
                    {
                        // m_commander.Team이 null이 아닐경우 CommanderName이 바뀌는 일은 없다.
                        if (missionItem.Commander.Team == null)
                        {
                            bChangePerformer = true;
                        }
                    }

                    if (bChangeActor == false && bChangePerformer == false)
                        continue;

                    int nComponentHistoryID;
                    UnE.SOP.History.HistorySectionData.DetailData detail = GetLastDetailData(i, state.DetailDatas, out nComponentHistoryID);

                    if (detail != null)
                    //if (/*state.Time != null || */state.DetailDatas.ContainsKey(i))
                    {
                        strCommanderName = GetAccessedUserName(state, nComponentHistoryID, detail.Time);

                        if (strCommanderName != null)
                            strUserName = strCommanderName;

                        /*if (GetAccessedUserName(out strCommanderName, state))
                            strUserName = strCommanderName;*/
                    }

                    if (strUserName == null)
                    {
                        strUserName = GetCurrentAccessedUserName(state.Time == null ? DateTime.Now : state.Time.Data);

                        if (strUserName == null)
                            strUserName = m_strCommanderName2;
                    }

                    if (bChangeActor)
                    {
                        row.Cells[MISSION_ACTOR_INDEX].Value = strUserName;
                        row.Cells[MISSION_ACTOR_INDEX].ToolTipText = strUserName;
                    }

                    if (bChangePerformer)
                    {
                        row.Cells[MISSION_PERFORMER_INDEX].Value = strUserName;
                        row.Cells[MISSION_PERFORMER_INDEX].ToolTipText = strUserName;
                    }

                    strCommanderName = strUserName;
                }
            }
            else
            {
                Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();

                if (frm != null)
                    strCommanderName = frm.ChangeCommanderName(state);
            }

            if (bChangeActor == true)
            {
                labelSender.Text = (String.IsNullOrWhiteSpace(strCommanderName) ? string.Empty : String.Format("( 발신자 : {0} )", strCommanderName));
            }

            /*foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (GetAccessedUserName(out strCommanderName, state))
                    row.Cells[MISSION_ACTOR_INDEX].Value = strCommanderName;
                else
                {
                    string strName = GetCurrentAccessedUserName(state.Time == null ? DateTime.Now : state.Time.Data);

                    if (strName == null)
                        strName = m_strCommanderName2;

                    row.Cells[MISSION_ACTOR_INDEX].Value = strName;
                }
            }*/
        }

        private string GetAccessedUserName(SectionState state, int nComponentHistoryID, VariousData<DateTime> time)
        {
            if (time == null)
                return null;

            int nAccessedUserID;
            bool find = m_dicComponentAccessedUserID.TryGetValue(nComponentHistoryID, out nAccessedUserID);

            if (!find)
            {
                LoadComponentAccessedUsers(state);
                find = m_dicComponentAccessedUserID.TryGetValue(nComponentHistoryID, out nAccessedUserID);
            }

            if (find)
            {
                Data_SOPGenUser user = FormSOP.Instance.SOPManager.GetSOPGenUser(nAccessedUserID);

                if (user == null)
                {
                    user = FormSOP.Instance.SOPManager.LoadSOPGenUser(nAccessedUserID);
                }

                if (user == null)
                    return null;

                bool dayLight = Popup.SOPLoader.IsNormal(time.Data);

                if (dayLight)
                {
                    if (user.DayLightCommander != null)
                        return user.DayLightCommander.DisplayText;
                    else if (user.NightCommander != null)
                        return user.NightCommander.DisplayText;
                }
                else
                {
                    if (user.NightCommander != null)
                        return user.NightCommander.DisplayText;
                    else if (user.DayLightCommander != null)
                        return user.DayLightCommander.DisplayText;
                }
            }

            return null;
        }

        // 특정 ComponentHistory에 대한 Accessed SOPGenUser ID를 얻어온다.
        private void LoadComponentAccessedUsers(SectionState state)
        {
            if (state == null)
                return;

            string strComponentHistoryIDs = "";

            foreach (KeyValuePair<int, List<UnE.SOP.History.HistorySectionData.DetailData>> pair in state.DetailDatas)
            {
                if (!m_dicComponentAccessedUserID.ContainsKey(pair.Key))
                {
                    if (strComponentHistoryIDs.Length == 0)
                        strComponentHistoryIDs = pair.Key.ToString();
                    else
                        strComponentHistoryIDs += ", " + pair.Key.ToString();
                }
            }

            if (strComponentHistoryIDs.Length > 0)
            {
                string strSQL = "Select ID, AccessedUserID from ComponentHistory where ID in (" + strComponentHistoryIDs + ")";
                ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i=0;i<nResultCount-1;i+=2)
                {
                    int nComponentHistoryID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nSOPGenUserID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                    if (nComponentHistoryID < 0 || nSOPGenUserID < 0)
                        continue;

                    m_dicComponentAccessedUserID[nComponentHistoryID] = nSOPGenUserID;
                }
            }
        }

        // nDataIndex에 해당하는 가장 마지막 Data를 얻어온다.
        private UnE.SOP.History.HistorySectionData.DetailData GetLastDetailData(int nDataIndex, Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> dicDetailDatas, out int nComponentHistoryID)
        {
            nComponentHistoryID = -1;
            UnE.SOP.History.HistorySectionData.DetailData lastData = null;

            foreach (KeyValuePair<int, List<UnE.SOP.History.HistorySectionData.DetailData>> pair in dicDetailDatas)
            {
                foreach (UnE.SOP.History.HistorySectionData.DetailData detail in pair.Value)
                {
                    if (detail.DataIndex != null && detail.DataIndex.Data == nDataIndex)
                    {
                        if (lastData == null)
                        {
                            lastData = detail;
                            nComponentHistoryID = pair.Key;
                        }
                        else
                        {
                            if (detail.Time == null && lastData.Time == null)
                            {
                                lastData = detail;
                                nComponentHistoryID = pair.Key;
                            }
                            else if (lastData.Time == null)
                            {
                                lastData = detail;
                                nComponentHistoryID = pair.Key;
                            }
                            else if (detail.Time != null && lastData.Time != null)
                            {
                                if (detail.Time.Data > lastData.Time.Data)
                                {
                                    lastData = detail;
                                    nComponentHistoryID = pair.Key;
                                }
                            }
                        }
                    }
                }
            }

            return lastData;
        }

        public bool GetAccessedUserName(out string strUserName, SectionState state = null)
        {
            strUserName = "";

            if (state == null)
            {
                state = GetSectionState();
            }

            if (state == null)
                return false;

            if (state != null && state.AccessedUserID > 0)
            {
                strUserName = GetControlUserName(state.AccessedUserID, state.Time == null ? DateTime.Now : state.Time.Data);

                if (strUserName == null)
                    strUserName = "";
                else
                    return true;
            }

            return false;
        }

        private string GetControlUserName(int nID, DateTime time)
        {
            Data_SOPGenUser user = FormSOP.Instance.SOPManager.GetSOPGenUser(nID);

            if (user == null)
            {
                user = FormSOP.Instance.SOPManager.LoadSOPGenUser(nID);
            }

            if (user != null)
            {
                bool dayLight = Popup.SOPLoader.IsNormal(time);

                if (dayLight)
                {
                    if (user.DayLightCommander != null)
                        return user.DayLightCommander.DisplayText;
                    else if (user.NightCommander != null)
                        return user.NightCommander.DisplayText;
                }
                else
                {
                    if (user.NightCommander != null)
                        return user.NightCommander.DisplayText;
                    else if (user.DayLightCommander != null)
                        return user.DayLightCommander.DisplayText;
                }
            }

            return null;
        }

        public string GetCurrentAccessedUserName(DateTime time)
        {
            int nControlUserID = FormSOP.Instance.ControlUserID;

            if (nControlUserID <= 0)
                return null;

            return GetControlUserName(nControlUserID, time);
        }

        public void ClearSelection()
        {
            dataGridView.ClearSelection();
        }

		public void AddGridData(Sections.Section section, string strStatus, int nCheckNotify1, int nCheckNotify2)
		{
            //교대 근무자 재조회
            FormSOP.Instance.SOPManager.LoadControlRoomMembers();

            m_contentsSectionType = section.GetComponentType();

            if (m_contentsSectionType == Sections.Section.ComponentType.PROCESS/* && strStatus != "실행 완료"*/)
			{
				Sections.SectionDataProcess dataSection = (Sections.SectionDataProcess)section.Data;
				SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
				if (page == null)
				{
					PanelSectionEx panel = (PanelSectionEx)section.GetParent();
					page = (SectionTabPage)panel.Parent;
					FormSOP.Instance.GetPageHome().TabControls.SelectedTab = page;
				}

				SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);

				int nCount = dataSection.MissionItems.Count;
				string[] strMission = null;
				string[] strTransmissionType = null;
				string[] strToNotify = null;
				bool[] useSMS = null;
				bool[] useBroadcast = null;
                SectionCommander[] arrCommander = null;

				if (nCount > 0)
				{
					strMission = new string[nCount];
					strTransmissionType = new string[nCount];
					strToNotify = new string[nCount];
					useSMS = new bool[nCount];
					useBroadcast = new bool[nCount];
                    arrCommander = new SectionCommander[nCount];

					int i = 0;
					int nBit = 0;
					foreach (Sections.MissionItem data in dataSection.MissionItems)
					{
						string szType = "무전기";
						switch (data.TransmissionType)
						{
							case 0:
								szType = "구두";
								break;
							case 1:
								szType = "전화";
								break;
							case 2:
								szType = "무전기";
								break;
							case 3:
								szType = "기타";
								break;

						}
                        
						strToNotify[i] = data.Target;
						//strTransmissionType[i] = "[" + szType + "]";
						strTransmissionType[i] = szType;
						strMission[i] = data.Mission;

						nBit = 1 << i;
						useSMS[i] = (nCheckNotify1 & nBit) == nBit;
						useBroadcast[i] = (nCheckNotify2 & nBit) == nBit;

						MissionItemInfo info = new MissionItemInfo();
						info.UseBroadcast = useBroadcast[i];
						info.UseSMS = useSMS[i];

						if (!info.UseBroadcast && !info.UseSMS)
							data.CheckItem = false;
						else
							data.CheckItem = true;

                        arrCommander[i] = data.Commander;

						FormSOP.Instance.SetMissionInfo(data, info);

						i++;
						if (i == 16)
							break;
					}
				}
				//AddGridData  string[] strTransType,  string[] strTarget, string[] strMission, 
                AddGridData(nCount, useSMS, useBroadcast, strTransmissionType, strToNotify, strMission, arrCommander, section, false, nCheckNotify1, nCheckNotify2);
			}
            else if (m_contentsSectionType == Sections.Section.ComponentType.TRANSMISSION/* && strStatus != "실행 완료"*/)
			{
				Sections.SectionDataTransmission dataSection = (Sections.SectionDataTransmission)section.Data;

				bool isPopupMessage = dataSection.DataInternal.UsePopupMessage;
				bool isMobileApp = dataSection.DataInternal.UseMobileApp;
				bool isBrodcast = dataSection.DataInternal.UseBroadcast;
				bool isSMS = dataSection.DataExternal.UseSMS;
				bool isFax = dataSection.DataExternal.UseFax;
				
				int nCnt = dataSection.DataExternal.SMSReceivers.Count + dataSection.DataExternal.FaxReceivers.Count + 3;

				string[] str = new string[nCnt];
				bool[] bUse = new bool[nCnt];

				int i = 0;

                str[i] = "(내부상황전파) 팝업메시지 사용";
                int nBit = 1 << i;
                bUse[i] = (nCheckNotify1 & nBit) == nBit;
                i++;

				str[i] = "(내부상황전파) 모바일메시지 사용";
				nBit = 1 << i;
				bUse[i] = (nCheckNotify1 & nBit) == nBit;
				i++;
				
				str[i] = "(내부상황전파) 사내방송 사용";
				nBit = 1 << i;
				bUse[i] = (nCheckNotify1 & nBit) == nBit;

				// nIdx = 3
				i = 2;

				if (dataSection.DataExternal.UseSMS)
				{
					foreach (Sections.ExternalTeamData data in dataSection.DataExternal.SMSReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
						nBit = 1 << i;
						bUse[i++] = (nCheckNotify1 & nBit) == nBit;
						if (i == 16)
							break;
					}
				}
				else
				{
					foreach (Sections.ExternalTeamData data in dataSection.DataExternal.SMSReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
						nBit = 1 << i;
						bUse[i++] = (nCheckNotify1 & nBit) == nBit;
						if (i == 16)
							break;
					}
				}
				// nIdx = 0
				int j = 0;
				if (dataSection.DataExternal.UseFax)
				{

					foreach (Sections.ExternalTeamData data in dataSection.DataExternal.FaxReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
						nBit = 1 << j;
						bUse[i++] = (nCheckNotify2 & nBit) == nBit;
						j++;
						if (j == 16)
							break;
					}
				}
				else
				{
					foreach (Sections.ExternalTeamData data in dataSection.DataExternal.FaxReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
						nBit = 1 << j;
						bUse[i++] = (nCheckNotify2 & nBit) == nBit;
						j++;
						if (j == 16)
							break;
					}
				}
				AddGridData(nCnt, bUse, str, section, nCheckNotify1, nCheckNotify2);
			}
            else if (m_contentsSectionType == Sections.Section.ComponentType.INTERNAL/* && strStatus != "실행 완료"*/)
			{
                AddInternalControls(section);
				/*Sections.SectionDataInternal dataSection = (Sections.SectionDataInternal)section.Data;

				bool[] bUse = new bool[3];
				string[] str = new string[3];
				int i = 0;

                str[i] = "(내부상황전파) 팝업메시지";
                int nBit = 1 << i;
                bUse[i++] = (nCheckNotify1 & nBit) == nBit;

				str[i] = "(내부상황전파) 모바일메시지";
				nBit = 1 << i;
				bUse[i++] = (nCheckNotify1 & nBit) == nBit;
								
				str[i] = "(내부상황전파) 사내방송";
				nBit = 1 << i;
				bUse[i++] = (nCheckNotify1 & nBit) == nBit;

				AddGridData(i, bUse, str, section, nCheckNotify1, nCheckNotify2);*/
			}
            else if (m_contentsSectionType == Sections.Section.ComponentType.EXTERNAL/* && strStatus != "실행 완료"*/)
			{
				Sections.SectionDataExternal dataSection = (Sections.SectionDataExternal)section.Data;
				SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;                
				if (page == null)
				{
					PanelSectionEx panel = (PanelSectionEx)section.GetParent();
					page = (SectionTabPage)panel.Parent;
					FormSOP.Instance.GetPageHome().TabControls.SelectedTab = page;
				}

				SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);

				if (page == null || state == null)
					return;

				bool isSMS = dataSection.UseSMS;
				bool isFax = dataSection.UseFax;

				int nCnt = dataSection.SMSReceivers.Count + dataSection.FaxReceivers.Count;
				string[] str = new string[nCnt];
				bool[] bUse = new bool[nCnt];
				int i = 0;
				int nBit = 0;
				if (dataSection.UseSMS)
				{
					foreach (Sections.ExternalTeamData data in dataSection.SMSReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
						nBit = 1 << i;
						bUse[i++] = (nCheckNotify1 & nBit) == nBit;
						if( i == 16 )
							break;
					}
				}
				else
				{
					foreach (Sections.ExternalTeamData data in dataSection.SMSReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
						nBit = 1 << i;
						bUse[i++] = (nCheckNotify1 & nBit) == nBit;
						if (i == 16)
							break;
					}
				}
				// nIdx = 0
				int j = 0;
				if (dataSection.UseFax)
				{
					
					foreach (Sections.ExternalTeamData data in dataSection.FaxReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
						nBit = 1 << j;
						bUse[i++] = (nCheckNotify2 & nBit) == nBit;
						j++;
						if (j == 16)
							break;
					}
				}
				else
				{
					foreach (Sections.ExternalTeamData data in dataSection.FaxReceivers)
					{
						str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
						nBit = 1 << j;
						bUse[i++] = (nCheckNotify2 & nBit) == nBit;
						j++;
						if (j == 16)
							break;
					}
				}
				AddGridData(nCnt, bUse, str, section, nCheckNotify1, nCheckNotify2);
			}
            else if (m_contentsSectionType == Sections.Section.ComponentType.DECISION)
            {
                cboDecisions.CanVisible = true;
                cboDecisions.Disabled = !this.Disabled;
                cboDecisions.Disabled = this.Disabled;

                int nLimit = 20;

                for (int i = 0; i < nLimit; i++)
                {
                    ISectionPainter painter = section.GetSectionPainter(i);

                    if (painter == null)
                        break;

                    if (painter is ProcessButtonManager)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)painter;
                        List<DecisionProcessButton> buttons = new List<DecisionProcessButton>();

                        AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.LEFT);
                        AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.RIGHT);
                        AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.BOTTOM);
                        AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.TOP);

                        buttons.Sort();

                        btnNext.Text = "다음";
                        btnExecute.Visible = false;
                        //cboDecisions.Visible = true;
                        cboDecisions.ShowControl();
                        cboDecisions.Location = new Point(btnExecute.Location.X + btnExecute.Size.Width - cboDecisions.Size.Width, cboDecisions.Location.Y);

                        foreach (DecisionProcessButton btn in buttons)
                        {
                            cboDecisions.Items.Add(btn);
                        }

                        if (cboDecisions.Items.Count > 0)
                            cboDecisions.SelectedIndex = 0;

                        break;
                    }
                }

                dataGridView.Tag = section;
                HideGrid();
            }
			else
			{
				dataGridView.Tag = section;
				HideGrid();
			}

            if (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)section.Data;

                if (data.IsBegin)
                    btnNext.Text = "시작";
                else
                    btnNext.Text = "종료";

                btnExecute.Visible = false;
                labelTitle.Size = new Size(btnExecute.Location.X + btnExecute.Size.Width - labelTitle.Location.X, labelTitle.Size.Height);
            }
		}

        private void AddDecisionProcessButton(List<DecisionProcessButton> buttons, ProcessButtonManager mgr, Arrow.ArrowPosition pos)
        {
            ProcessButton btn = mgr.FindButton(pos);

            if (btn != null && btn.Data != null)
            {
                foreach (Arrow arrow in btn.Data.Arrows)
                {
                    if (arrow.EndLink.GetComponentType() == Sections.Section.ComponentType.ANNOTATION)
                        continue;

                    string strArrowText = "";

                    if (arrow.Text.Length > 0)
                    {
                        if (arrow.EndLink.Data.SectionNumber > 0)
                            strArrowText = string.Format("{0}({1})", arrow.EndLink.Data.SectionNumber, arrow.Text);
                        else
                            strArrowText = string.Format("{0}({1})", arrow.EndLink.Title, arrow.Text);
                    }
                    else
                    {
                        if (arrow.EndLink.Data.SectionNumber > 0)
                        {
                            if (arrow.EndLink.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
                                strArrowText = string.Format("{0}({1})", arrow.EndLink.Data.SectionNumber, arrow.EndLink.Title);
                            else
                                strArrowText = string.Format("{0}", arrow.EndLink.Data.SectionNumber);
                        }
                        else
                            strArrowText = string.Format("{0}", arrow.EndLink.Title);
                    }

                    /*string strArrowText = arrow.Text;

                    if (strArrowText.Length == 0)
                    {
                        if (arrow.EndLink.Data.SectionNumber > 0)
                            strArrowText = arrow.EndLink.Data.SectionNumber.ToString();
                        else
                            strArrowText = arrow.EndLink.Title;
                    }*/

                    DecisionProcessButton button = new DecisionProcessButton(strArrowText, btn);
                    buttons.Add(button);
                }
            }
        }

        private void AddInternalControls(Section section)
        {
            /*IAnnounceMessage message = SOPMonitoringSystem.Process.InternalNotifyProcess.GetAnnounceMessage(section);

            if (message == null)
                return;

            Form frm = (Form)message;
            frm.TopMost = false;
            frm.TopLevel = false;
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;*/
            Popup.MissionMessage.FormMissionMessage frm = new Popup.MissionMessage.FormMissionMessage(section);

            //panelExternal.Size = frm.Size;
            frm.Dock = DockStyle.Fill;
            panelExternal.Controls.Clear();
            panelExternal.Controls.Add(frm);
            panelExternal.Show();

            frm.Dock = DockStyle.Fill;
            frm.Location = new Point(0, 0);
            frm.Show();

            VisibleExternalPanel = true;
            ShowGrid();
        }

		private void AddGridData(int nRowCount, 
			bool[] useSMS, 
			bool[] useBroadcast, 
			string[] strTransType, 
			string[] strTarget, 
			string[] strMission,
            SectionCommander[] arrCommander,
			Sections.Section section, 
			bool checkBoxReadOnly, 
			int nCheckNotify1,
			int nCheckNotify2)
		{
			ShowContainer();
			m_isFlag = false;

			int nRowHeight = 0;
			for (int i = 0; i < nRowCount; i++)
			{
				DataGridViewRow gridRow = new DataGridViewRow();
                gridRow.Height = 30;
                gridRow.DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 12.25F);

                /*DataGridViewCell cellTitle = new DataGridViewTextBoxCell();
                cellTitle.Value = section.Title;
                gridRow.Cells.Add(cellTitle);*/

                DataGridViewCell cellPerformer = new DataGridViewTextBoxCell();
                cellPerformer.Value = arrCommander[i] == null ? "" : arrCommander[i].DisplayText;
                cellPerformer.ToolTipText = arrCommander[i] == null ? "" : arrCommander[i].DisplayText;
                gridRow.Cells.Add(cellPerformer);

                DataGridViewCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strMission[i]; // i.ToString() + ". 상황전파";
                // 원본 데이터를 기억시킨다.
                cell3.Tag = strMission[i];
                gridRow.Cells.Add(cell3);

                DataGridViewCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = GetReceiverName(section);//strTarget[i]; // i.ToString() + ". 상황전파";
                if (cell2.Value != null)
                    cell2.Tag = cell2.Value.ToString();
                cell2.ToolTipText = cell2.Value.ToString();//strTarget[i];
                gridRow.Cells.Add(cell2);

                /*DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = strTransType[i]; // i.ToString() + ". 상황전파";
                gridRow.Cells.Add(cell1);*/

                DataGridViewDisableButtonCell btnCell = new DataGridViewDisableButtonCell();

                if (IsExternalRunMission(section.Data, i))
                    btnCell.Value = "System";
                else
                    btnCell.Value = "문자";

                //btnCell.Value = "실행";
                gridRow.Cells.Add(btnCell);

                /*btnCell = new DataGridViewDisableButtonCell();
                btnCell.Value = "실행";
                gridRow.Cells.Add(btnCell);*/
				/*DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
				checkCell.Value = useSMS[i];
				checkCell.ReadOnly = checkBoxReadOnly;
				checkCell.Tag = nCheckNotify1;
				gridRow.Cells.Add(checkCell);

				checkCell = new DataGridViewCheckBoxCell();
				checkCell.Value = useBroadcast[i];
				checkCell.ReadOnly = checkBoxReadOnly;
				checkCell.Tag = nCheckNotify2;
				gridRow.Cells.Add(checkCell);*/

                /*DataGridViewButtonCell cellButton = new DataGridViewDisableButtonCell();
                cellButton.Value = "실행";
                gridRow.Cells.Add(cellButton);*/

                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = false;
                gridRow.Cells.Add(checkCell);

                DataGridViewCell cellComplete = new DataGridViewTextBoxCell();
                //cellComplete.Value = GetTimeString(DateTime.Now);
                //cellComplete.ToolTipText = cell4.Value.ToString();
                gridRow.Cells.Add(cellComplete);

                DataGridViewCell cellActor = new DataGridViewTextBoxCell();
                cellActor.Value = GetActorName(section);
                cellActor.ToolTipText = cellActor.Value.ToString();
                gridRow.Cells.Add(cellActor);

				nRowHeight = gridRow.Height;

				dataGridView.Rows.Add(gridRow);
			}

			//dataGridView.Columns[TRANS_TYPE_INDEX].Width = 70;
			//dataGridView.Columns[MISSION_TARGET_INDEX].Width = 130;
            dataGridView.Tag = section;
            dataGridView.Size = new Size(dataGridView.Width, dataGridView.ColumnHeadersHeight + (nRowHeight * nRowCount) + 3);
			//this.columnBroadcast.Visible = true;

			ReSizeForm(m_isFlag);
		}

        private bool IsExternalRunMission(SectionData data, int nIndex)
        {
            if (data is SectionDataProcess)
            {
                SectionDataProcess dataProcess = (SectionDataProcess)data;

                if (dataProcess.MissionItems.Count > nIndex)
                {
                    if (dataProcess.MissionItems[nIndex] is MissionItemExternal)
                        return true;
                }
            }

            return false;
        }

        private string GetReceiverName(Sections.Section section)
        {
            if (section == null)
                return "";

            if (m_receiverPhoneNumbers != null)
                return m_strReceiverName;

            ArrayList arrTeamList;
            bool onlyTeamLeader;
            m_receiverPhoneNumbers = GetReceiverInfo(section, out m_strReceiverName, out arrTeamList, out onlyTeamLeader);
            /*if (section is Sections.SectionProcess)
            {
                Sections.SectionProcess process = (Sections.SectionProcess)section;

                string strReceiverName = process.TextDown;

                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
                m_receiverPhoneNumbers = GetReceiverPhoneNumbers(data.TeamList, data.TransferTeamLeaderOnly);

                if (m_receiverPhoneNumbers != null)
                    m_strReceiverName = strReceiverName;
            }*/

            return m_strReceiverName;
        }

        public static ArrayList GetReceiverInfo(Sections.Section section, out string strReceiverName, out ArrayList arrTeamList, out bool onlyTeamLeader)
        {
            arrTeamList = null;
            strReceiverName = "";
            onlyTeamLeader = true;
            ArrayList arrReceiverPhoneNumbers = null;
            //bool includeChildTeams = false;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionProcess process = (Sections.SectionProcess)section;

                strReceiverName = process.TextDown;

                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

                onlyTeamLeader = /*true;*/data.TransferTeamLeaderOnly;
                arrTeamList = data.TeamList;

                /*string strNames;
                m_receiverPhoneNumbers = GetReceiverPhoneNumbers(data.TeamList, data.TransferTeamLeaderOnly, out strNames);

                if (m_receiverPhoneNumbers != null)
                    m_strReceiverName = strReceiverName;*/
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

                // 내부상황전파는 무조건 전체 팀원에게 보낸다.
                onlyTeamLeader = false;// data.TransferTeamLeaderOnly;
                arrTeamList = data.TeamList;
                //includeChildTeams = true;
            }

            if (arrTeamList != null)
            {
                strReceiverName = "";

                foreach (SOPTeam team in arrTeamList)
                {
                    string strTeamName = team.IncludeChildTeams ? team.TeamName + "(+)" : team.TeamName;

                    if (strReceiverName.Length == 0)
                        strReceiverName = strTeamName;
                    else
                        strReceiverName += ", " + strTeamName;
                }

                string strNames;
                arrReceiverPhoneNumbers = GetReceiverPhoneNumbers(arrTeamList, onlyTeamLeader, /*includeChildTeams, */out strNames);

                if (strReceiverName == "")
                    strReceiverName = strNames;
            }

            return arrReceiverPhoneNumbers;
        }

        private static void AddChildTeams(ArrayList arrTeams)
        {
            List<SOPTeam> addTeams = new List<SOPTeam>();

            foreach (SOPTeam teamData in arrTeams)
            {
                if (teamData.TeamType == SOPTeam.SOPTeamType.External)    // 협력 회사 혹은 외부 기관
                {
                    Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetExternalTeam(teamData.TeamID);

                    if (team != null)
                        AddChildTeams(addTeams, team);
                }
                else if (teamData.TeamType == SOPTeam.SOPTeamType.Regular)    // 정규 조직
                {
                    Data_RegularTeam team = FormSOP.Instance.SOPManager.GetRegularTeam(teamData.TeamID);

                    if (team != null)
                        AddChildTeams(addTeams, team);
                }
            }

            foreach (SOPTeam teamData in addTeams)
            {
                if (!arrTeams.Contains(teamData))
                    arrTeams.Add(teamData);
            }
            //arrTeams.AddRange(addTeams);
        }

        private static void AddChildTeams(List<SOPTeam> addTeams, Data_RegularTeam team)
        {
            foreach (Data_RegularTeam childTeam in team.ChildTeams)
            {
                SOPTeam sopTeam = new SOPTeam();

                sopTeam.TeamID = childTeam.ID;
                sopTeam.TeamName = childTeam.TeamName;
                sopTeam.TeamType = SOPTeam.SOPTeamType.Regular;

                addTeams.Add(sopTeam);
                AddChildTeams(addTeams, childTeam);
            }
        }

        private static void AddChildTeams(List<SOPTeam> addTeams, Data_ExternalTeam team)
        {
            foreach (Data_ExternalTeam childTeam in team.ChildTeams)
            {
                SOPTeam sopTeam = new SOPTeam();

                sopTeam.TeamID = childTeam.ID;
                sopTeam.TeamName = childTeam.TeamName;
                sopTeam.TeamType = SOPTeam.SOPTeamType.External;

                addTeams.Add(sopTeam);
                AddChildTeams(addTeams, childTeam);
            }
        }

        private static void AddTemporaryMemberPhoneNumbers(Dictionary<string, string> dicPhoneNumbers, List<TemporaryMember> members)
        {
            Dictionary<int, string> dicCompanyMemberPhoneNumbers = new Dictionary<int, string>();

            foreach (TemporaryMember member in members)
            {
                if (member._MemberType == TemporaryMember.MemberType.CompanyMember)
                {
                    DataCompanyMember companyMember = DataManager.Instance.GetCompanyMember(member.MemberID);

                    if (companyMember == null)
                        continue;

                    dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                }
                else if (member._MemberType == TemporaryMember.MemberType.RegularTeam)
                {
                    if (member.TeamLeader == 1)
                    {
                        Data_RegularTeam team = FormSOP.Instance.SOPManager.GetRegularTeam(member.MemberID);

                        if (team == null)
                            continue;

                        int nLeaderID = GetRegularTeamLeaderID(team);

                        if (nLeaderID < 0)
                            continue;

                        DataCompanyMember companyMember = DataManager.Instance.GetCompanyMember(nLeaderID);

                        if (companyMember == null)
                            continue;

                        dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                    }
                    else
                    {
                        List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(member.MemberID, member.IncludeChildTeams);

                        if (companyMembers == null)
                            continue;

                        foreach (DataCompanyMember companyMember in companyMembers)
                        {
                            dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                        }
                    }
                }
                else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyMember)
                {
                    DataExternalMember externalMember = DataManager.Instance.GetExternalMember(member.MemberID);

                    if (externalMember != null)
                        dicPhoneNumbers[externalMember.PhoneNumber] = externalMember.PhoneNumber;
                }
                else if (member._MemberType == TemporaryMember.MemberType.ExternalTeam || member._MemberType == TemporaryMember.MemberType.ExternalCompanyTeam)
                {
                    AddExternalMemberPhoneNumbers(member.MemberID, member.IncludeChildTeams, dicPhoneNumbers);
                }
                else if (member._MemberType == TemporaryMember.MemberType.UserDefinedTeam)
                {
                    string strPhoneNumber, strTeamName;

                    if (DataManager.Instance.GetUserDefinedTeamInfo(member.MemberID, out strPhoneNumber, out strTeamName))
                    {
                        dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
                    }
                }
                else if (member._MemberType == TemporaryMember.MemberType.JobLevel)
                {
                    List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(member.MemberID);

                    if (companyMembers == null)
                        continue;

                    foreach (DataCompanyMember companyMember in companyMembers)
                    {
                        dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                    }
                }
            }
        }

        private static void AddExternalMemberPhoneNumbers(int nTeamID, bool includeChildTeams, Dictionary<string, string> dicPhoneNumbers)
        {
            List<DataExternalMember> externalMembers = DataManager.Instance.GetExternalMembers(nTeamID, includeChildTeams);

            if (externalMembers == null)
                return;

            foreach (DataExternalMember externalMember in externalMembers)
            {
                dicPhoneNumbers[externalMember.PhoneNumber] = externalMember.PhoneNumber;
            }
        }

        /*private static List<int> GetRegularMemberList(int nTeamID, bool includeChildTeams, bool parentTeamID = false, List<int> ids = null, Dictionary<int, int> dicCompanyMemberIDs = null)
        {
            string strSQL = "Select rml.CompanyMemberID, rml.RegularTeamID from RegularMemberList as rml, RegularTeam as team ";
            strSQL += "where team.ID = rml.RegularTeamID and ";

            if (parentTeamID)
                strSQL += "team.ParentTeamID = " + nTeamID.ToString();
            else
                strSQL += "team.ID = " + nTeamID.ToString();

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (ids == null)
                ids = new List<int>();

            if (dicCompanyMemberIDs == null)
                dicCompanyMemberIDs = new Dictionary<int, int>();

            Dictionary<int, int> dicTeamIDs = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> _teamID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || _teamID == null)
                    continue;

                if (dicCompanyMemberIDs.ContainsKey(id.Data) == false)
                {
                    dicCompanyMemberIDs[id.Data] = id.Data;
                    ids.Add(id.Data);
                }

                dicTeamIDs[_teamID.Data] = _teamID.Data;
            }

            if (includeChildTeams)
            {
                foreach (KeyValuePair<int, int> pair in dicTeamIDs)
                {
                    GetRegularMemberList(pair.Key, includeChildTeams, true, ids, dicCompanyMemberIDs);
                }
            }

            return ids;
        }*/

        private static ArrayList GetReceiverPhoneNumbers(ArrayList arrTeams, bool onlyTeamLeader, /*bool includeChildTeams, */out string strReceiverNames)
        {
            strReceiverNames = "";

            int nOriginalTeamCount = arrTeams.Count;

            /*if (includeChildTeams)
                AddChildTeams(arrTeams);*/

            //ControlTeamEditor.VaildMemberPhoneNumber.LoadDB();

            SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;

            int nTeamCount = arrTeams.Count;

            // 중복을 막기 위하여 Dictionary 사용
            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();

            for (int i = 0; i < nTeamCount;i++ )
            //foreach (SOPTeam teamData in arrTeams)
            {
                SOPTeam teamData = (SOPTeam)arrTeams[i];

                // nOriginalTeamCount보다 같거나 큰 것들은 자식 팀들이다.
                if (i < nOriginalTeamCount)
                {
                    if (strReceiverNames.Length == 0)
                        strReceiverNames = teamData.TeamName;
                    else
                        strReceiverNames += ", " + teamData.TeamName;
                }

                if (teamData.TeamType == SOPTeam.SOPTeamType.Normal || teamData.TeamType == SOPTeam.SOPTeamType.Holiday)   // 평일 조직 또는 야간 조직
                {
                    List<TemporaryMember> members = new List<TemporaryMember>();

                    if (IOManager.ReadTemporaryTeamMemberList((WebDBManager)UnE.SOP.ProxySOP.Instance.DBManager, teamData.TeamType == SOPTeam.SOPTeamType.Normal, teamData.IncludeChildTeams, teamData.TeamID, members))
                    {
                        AddTemporaryMemberPhoneNumbers(dicPhoneNumbers, members);
                    }
                    /*if (page != null)
                    {
                        List<DataRoleMember> roleMembers = null;

                        if (teamData.TeamType == SOPTeam.SOPTeamType.Normal)
                        {
                            Data_NormalTeam team = page.GetTemporaryNormalTeamMember(teamData.TeamID);

                            if (team != null && team.Tag != null)
                                roleMembers = (List<DataRoleMember>)team.Tag;
                        }
                        else
                        {
                            Data_EmergencyTeam team = page.GetTemporaryEmergencyTeamMember(teamData.TeamID);

                            if (team != null && team.Tag != null)
                                roleMembers = (List<DataRoleMember>)team.Tag;
                        }

                        if (roleMembers != null)
                        {
                            foreach (DataRoleMember roleMember in roleMembers)
                            {
                                AddRoleMemberPhoneNumber(roleMember, dicPhoneNumbers);
                            }
                        }
                    }*/
                    /*bool isDayLight = teamData.TeamType == SOPTeam.SOPTeamType.Normal;
                    bool includeMain = true;            // 정
                    bool includeSub = true;             // 부
                    bool includeTeamLeader = false;    // 반원
                    bool includeOthers = !onlyTeamLeader;

                    List<TemporaryMember> members = GetTemporaryMembers(teamData.TeamID, isDayLight, includeMain, includeSub, includeTeamLeader, includeOthers);

                    string strDisplayName = "", strPhoneNumber = "";

                    foreach (TemporaryMember member in members)
                    {
                        strPhoneNumber = "";

                        if (GetTemporaryMemberInfo(member, ref strDisplayName, ref strPhoneNumber))
                        {
                            if (strPhoneNumber.Length > 0 && !dicPhoneNumbers.ContainsKey(strPhoneNumber))
                            {
                                dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
                            }
                        }
                    }*/
                }
                else if (teamData.TeamType == SOPTeam.SOPTeamType.External)    // 협력 회사 혹은 외부 기관
                {
                    AddExternalMemberPhoneNumbers(teamData.TeamID, teamData.IncludeChildTeams, dicPhoneNumbers);
                    /*if (page != null)
                    {
                        Data_ExternalTeam team = page.GetExternalTeamMember(teamData.TeamID);

                        if (team != null && team.Tag != null)
                        {
                            AddRoleMemberPhoneNumber((DataRoleMember)team.Tag, dicPhoneNumbers);
                        }
                    }*/
                    /*List<ExternalCompanyTeam> teams = null;
                    Data_ExternalTeam company = FormSOP.Instance.SOPManager.GetExternalTeam(teamData.TeamID);

                    if (company != null)
                    {
                        teams = FormSOP.Instance.SOPManager.GetExternalCompanyTeams(company.ID);
                    }
                    else
                    {
                        ExternalCompanyTeam team = FormSOP.Instance.SOPManager.FindExternalCompanyTeam(teamData.TeamID);

                        if (team != null)
                        {
                            teams = new List<ExternalCompanyTeam>();
                            teams.Add(team);
                        }
                    }

                    if (teams == null || teams.Count == 0)
                        continue;

                    foreach (ExternalCompanyTeam team in teams)
                    {
                        if (onlyTeamLeader && team.Members.Count > 0)
                        {
                            // 팀장만 선택하는 옵션이면 협력업체 직원중 첫번째 직원의 전화번호만 검색한다.
                            // Changed by mwkim 2015-10-16 순차적으로 직원을 검색하여 현재 근무조에 해당되는 직원의 전화번호를 가져온다.
                            for (int nIndexExternalMember = 0; nIndexExternalMember < team.Members.Count; nIndexExternalMember++)
                            {
                                ExternalCompanyMember member = team.Members[nIndexExternalMember];

                                if(String.IsNullOrWhiteSpace(member.PhoneNumber) == false)
                                {
                                    if (ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(member.PhoneNumber) == true)
                                    {
                                        if (!dicPhoneNumbers.ContainsKey(member.PhoneNumber))
                                        {
                                            dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Changed by mwkim 2015-10-16 현재 근무조에 해당 되는 조직원에 대해서만 문자 발송(근무조 편성과 관계없는 직원은 무조건 발송)
                            ArrayList arrAllMemberPhoneNumbers = new ArrayList();

                            foreach (ExternalCompanyMember member in team.Members)
                            {
                                if (String.IsNullOrWhiteSpace(member.PhoneNumber) == true)
                                    arrAllMemberPhoneNumbers.Add(member.PhoneNumber);

                            }

                            // 현재 근무조에 해당되는 직원의 핸드폰 번호
                            foreach (string strValidPhoneNumber in ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrAllMemberPhoneNumbers))
                            {
                                dicPhoneNumbers[strValidPhoneNumber] = strValidPhoneNumber;
                            }
                            /////

                        }
                    }*/
                }
                else if (teamData.TeamType == SOPTeam.SOPTeamType.UserDefined)    // 사용자 정의 조직
                {
                    Data_UserDefinedTeam userDefinedTeam = DataManager.Instance.LoadUserDefinedTeam(teamData.TeamID);

                    if (userDefinedTeam != null)
                    {
                        dicPhoneNumbers[userDefinedTeam.PhoneNumber] = userDefinedTeam.PhoneNumber;
                    }
                    //Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeamMember(teamData.TeamID);

                    // Edit by skkim 2015-08-31
                    // action step에서 사용중인 UserDefine팀은 TabPage에 저장된다.
                    // 각 ActionStep마다 다른 UserDefine팀을 갖도록 수정함   
                    /*if (page != null)
                    {
                        Data_UserDefinedTeam team = page.GetUserDefinedTeamMember(teamData.TeamID);

                        if (team != null && team.Tag != null)
                        {
                            AddRoleMemberPhoneNumber((DataRoleMember)team.Tag, dicPhoneNumbers);
                        }
                    }*/
                }
                else if (teamData.TeamType == SOPTeam.SOPTeamType.Regular)    // 정규 조직
                {
                    List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(teamData.TeamID, teamData.IncludeChildTeams);

                    if (companyMembers == null)
                        continue;

                    foreach (DataCompanyMember companyMember in companyMembers)
                    {
                        dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                    }
                    /*ArrayList members = new ArrayList();

                    if (FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(teamData.TeamID, ref members))
                    {
                        foreach (Data_CompanyMember member in members)
                        {
                            if (member.PhoneNumber.Length > 0)
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                        }
                    }*/
                    /*if (page != null)
                    {
                        Data_RegularTeam team = page.GetRegularTeamMember(teamData.TeamID);

                        if (team != null && team.Tag != null)
                        {
                            List<DataRoleMember> roleMembers = (List<DataRoleMember>)team.Tag;

                            if (roleMembers != null)
                            {
                                foreach (DataRoleMember roleMember in roleMembers)
                                {
                                    if (roleMember.AllMembers)
                                        AddAllCompanyMemberPhoneNumber(dicPhoneNumbers);
                                    else
                                        AddRoleMemberPhoneNumber(roleMember, dicPhoneNumbers);
                                }
                            }
                            //AddRoleMemberPhoneNumber((DataRoleMember)team.Tag, dicPhoneNumbers);
                        }
                    }*/
                    /*Data_RegularTeam team = FormSOP.Instance.SOPManager.GetRegularTeam(teamData.TeamID);

                    if (team == null)
                        continue;

                    ArrayList arrMembers = new ArrayList();
                    FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(teamData.TeamID, ref arrMembers);

                    bool find = false;
                    //Data_CompanyMember teamLeader = null;

                    foreach (Data_CompanyMember member in arrMembers)
                    {
                        if (onlyTeamLeader)
                        {
                            int nPosition;

                            if (member.TeamPositions.TryGetValue(team, out nPosition))
                            {
                                if (nPosition == 2)
                                {
                                    if (member.PhoneNumber.Length > 0 && !dicPhoneNumbers.ContainsKey(member.PhoneNumber))
                                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;

                                    find = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Changed by mwkim 2015-10-16 현재 근무조에 해당 되는 조직원에 대해서만 문자 발송(근무조 편성과 관계없는 직원은 무조건 발송)
                            if (String.IsNullOrWhiteSpace(member.PhoneNumber) == false && !dicPhoneNumbers.ContainsKey(member.PhoneNumber))
                            {
                                if (ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(member.PhoneNumber) == true)
                                {
                                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                                }
                            }

                            find = true;
                        }
                    }

                    // Added by mwkim 2015-10-16 문자수신할 팀원 또는 팀장을 못찾은 경우 현재 근무중인 팀원에게 문자를 발송함(만약 근무중인 팀원이 없으면 팀원 모두에게 발송)
                    if (find == false)
                    {
                        ArrayList arrAllMemberPhoneNumbers = new ArrayList();

                        foreach (Data_CompanyMember member in arrMembers)
                        {
                            if(String.IsNullOrWhiteSpace(member.PhoneNumber) == false)
                            {
                                arrAllMemberPhoneNumbers.Add(member.PhoneNumber);
                            }
                        }

                        // 현재 근무조에 해당되는 직원의 핸드폰 번호
                        foreach (string strValidPhoneNumber in ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrAllMemberPhoneNumbers))
                        {
                            dicPhoneNumbers[strValidPhoneNumber] = strValidPhoneNumber;

                            find = true;
                        }

                        // 팀에 현재 근무조가 없으면 팀원 모두를 SMS 발송
                        if (find == false)
                        {
                            foreach (Data_CompanyMember member in arrMembers)
                            {
                                if (String.IsNullOrWhiteSpace(member.PhoneNumber) == false)
                                {
                                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                                    
                                }

                                find = true;
                            }
                        }
                    }*/

                }
                else if (teamData.TeamType == SOPTeam.SOPTeamType.ControlRoom)    // 교대근무자
                {
                    List<Data_ControlRoomMember> controlRoomMembers = FormSOP.Instance.SOPManager.GetControlRoomMembers(teamData.TeamID);

                    if (controlRoomMembers == null)
                        continue;

                    foreach (Data_ControlRoomMember member in controlRoomMembers)
                    {
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                    }
                    /*if (page != null)
                    {                        
                        if (!FormSOP.Instance.SOPManager.ControlRoomMembers.ContainsKey(teamData.TeamID))
                            continue;

                        Data_ControlRoomMember member = FormSOP.Instance.SOPManager.ControlRoomMembers[teamData.TeamID];

                        if (!dicPhoneNumbers.ContainsKey(member.PhoneNumber))
                            dicPhoneNumbers.Add(member.PhoneNumber, member.PhoneNumber);
                        else
                            dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                    }*/
                }
            }

            //ControlTeamEditor.VaildMemberPhoneNumber.ReleaseDB();

            ArrayList phoneNumbers = new ArrayList();

            foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
            {
                phoneNumbers.Add(pair.Value);
            }

            // 산출된 전화번호에서 근무표의 조원과 대조하여 유효한 전화번호만 색출
            phoneNumbers = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(phoneNumbers, ProxySOP.Instance.DBManager);

            return phoneNumbers;
        }

        private static void AddAllCompanyMemberPhoneNumber(Dictionary<string, string> dicPhoneNumbers)
        {
            List<Data_CompanyMember> allMembers = FormSOP.Instance.SOPManager.GetAllRegularCompanyMemberList();

            foreach (Data_CompanyMember member in allMembers)
            {
                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
            }
        }

        private static void AddRoleMemberPhoneNumber(DataRoleMember roleMember, Dictionary<string, string> dicPhoneNumbers)
        {
            if (roleMember.PhoneNumber != null && roleMember.PhoneNumber.Length > 0)
            {
                bool isValid;
                string strPhoneNumber = WebDBManager.ValidPhoneNumber(roleMember.PhoneNumber, out isValid);

                if (isValid && !dicPhoneNumbers.ContainsKey(roleMember.PhoneNumber))
                {
                    dicPhoneNumbers[roleMember.PhoneNumber] = roleMember.PhoneNumber;
                }
            }
        }

        private string GetActorName(Sections.Section section)
        {
            if (section == null)
                return "";

            if (m_commander != null)
                return m_strCommanderName;

            m_commander = GetCommanderInfo(section, out m_strCommanderName, out m_strCommanderName2, out m_strCommanderPhoneNumber);
            return m_strCommanderName;
        }

        public static SectionCommander GetCommanderInfo(Sections.Section section, out string strCommanderName, out string strCommanderName2, out string strCommanderPhoneNumber)
        {
            strCommanderName = strCommanderName2 = strCommanderPhoneNumber = "";
            SectionCommander commander = null;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

                if (data.Commander != null)
                    commander = data.Commander.Clone();
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

                if (data.Commander != null)
                    commander = data.Commander.Clone();
            }

            if (commander != null)
            {
                strCommanderName = GetSectionCommanderName(commander, out strCommanderName2, out strCommanderPhoneNumber);
            }

            return commander;
        }

        private static string GetSectionCommanderName(Sections.SectionCommander commander, out string strCommanderName2, out string strPhoneNumber)
        {
            string strDisplayText = null;
            strPhoneNumber = strCommanderName2 = "";

            if (commander == null)
                return "";

            if (commander.DisplayText != null && commander.DisplayText.Length > 0)
                strDisplayText = commander.DisplayText;

            if (commander.Team == null)
            {
                bool isDayLight = Popup.SOPLoader.IsNormal(DateTime.Now);

                if (isDayLight)
                {
                    if (FormSOP.Instance.SOPGenUserCommanderDayLight != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderDayLight;
                    else if (FormSOP.Instance.SOPGenUserCommanderNightHoliday != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderNightHoliday;
                    else
                        return "";
                }
                else
                {
                    if (FormSOP.Instance.SOPGenUserCommanderNightHoliday != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderNightHoliday;
                    else if (FormSOP.Instance.SOPGenUserCommanderDayLight != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderDayLight;
                    else
                        return "";
                }
            }

            strCommanderName2 = GetCommanderMemberName(commander, ref strPhoneNumber);

            if (strDisplayText == null)
                strDisplayText = strCommanderName2;

            if (commander.CallerPhoneNumber != null && commander.CallerPhoneNumber.Length > 0)
                strPhoneNumber = commander.CallerPhoneNumber;

            return strDisplayText;
        }

        private static string GetCommanderMemberName(Sections.SectionCommander commander, ref string strPhoneNumber)
        {
            string strDisplayText = null;

            if (commander.Team == null)
                return "";

            if (commander.DisplayText != null && commander.DisplayText.Length > 0)
                strDisplayText = commander.DisplayText;

            if (commander.IsTeamMember)
            {
                ArrayList arrMembers = new ArrayList();

                if (commander.Team.TeamType == SOPTeam.SOPTeamType.Regular)
                {
                    if (!FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(commander.Team.TeamID, ref arrMembers))
                        return "";
                    
                    foreach (Data_CompanyMember member in arrMembers)
                    {
                        if (member.ID == commander.TeamMemberID)
                        {
                            if (strDisplayText == null)
                                strDisplayText = member.MemberName;
                            strPhoneNumber = member.PhoneNumber;
                            break;
                        }
                    }
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.External)
                {
                    foreach (ExternalCompanyMember member in FormSOP.Instance.SOPManager.ExternalCompanyMembers)
                    {
                        if (member.ID == commander.TeamMemberID)
                        {
                            if (strDisplayText == null)
                                strDisplayText = member.MemberName;
                            strPhoneNumber = member.PhoneNumber;
                            break;
                        }
                    }
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Normal)
                {
                    if (!GetTemporaryMemberInfo(commander.TeamMemberID, true, ref strDisplayText, ref strPhoneNumber))
                        return "";
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Holiday)
                {
                    if (!GetTemporaryMemberInfo(commander.TeamMemberID, false, ref strDisplayText, ref strPhoneNumber))
                        return "";
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.ControlRoom)
                {
                    if (!FormSOP.Instance.SOPManager.ControlRoomMembers.ContainsKey(commander.Team.TeamID))
                        return "";
                    else
                    {
                        strPhoneNumber = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].PhoneNumber;
                        if (strDisplayText == null)
                            strDisplayText = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].MemberName;
                    } 
                }
            }
            else
            {
                if (commander.Team.TeamType == SOPTeam.SOPTeamType.Regular)
                {
                    Data_CompanyMember companyMember;
                    Data_RegularTeam team = GetRegularTeamLeaderInfo(commander.Team.TeamID, out companyMember);

                    if (team == null)
                        return "";

                    strPhoneNumber = companyMember.PhoneNumber;

                    if (strDisplayText != null)
                        strDisplayText = team.TeamName + "장";
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.External)
                {
                    ExternalCompanyTeam team = FormSOP.Instance.SOPManager.FindExternalCompanyTeam(commander.Team.TeamID);

                    if (team == null)
                        return "";

                    if (team.Members == null || team.Members.Count == 0)
                        return "";

                    ExternalCompanyMember member = team.Members[0];

                    strPhoneNumber = member.PhoneNumber;

                    if (strDisplayText != null)
                        strDisplayText = team.TeamName;
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.UserDefined)
                {

                    // Edit by skkim 2015-08-31
                    // action step에서 사용중인 UserDefine팀은 TabPage에 저장된다.
                    // 각 ActionStep마다 다른 UserDefine팀을 갖도록 수정함
                    //Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeam(commander.Team.TeamID);
                    SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
                    if (page != null)
                    {
                        Data_UserDefinedTeam team = page.GetUserDefinedTeamMember(commander.Team.TeamID);
                        if (team == null)
                            return "";

                        if (team.Tag != null)
                        {
                            DataRoleMember roleMember = (DataRoleMember)team.Tag;
                            strPhoneNumber = roleMember.PhoneNumber == null ? "" : roleMember.PhoneNumber;
                        }

                        if (strDisplayText != null)
                            strDisplayText = team.TeamName;
                    }
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Normal)
                {
                    TemporaryMember member = GetTemporaryMainMember(commander.Team.TeamID, true);

                    string strDisplayText2 = "";

                    if (!GetTemporaryMemberInfo(member, ref strDisplayText2, ref strPhoneNumber))
                        return "";

                    if (strDisplayText == null)
                        strDisplayText = strDisplayText2;
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Holiday)
                {
                    TemporaryMember member = GetTemporaryMainMember(commander.Team.TeamID, false);

                    string strDisplayText2 = "";

                    if (!GetTemporaryMemberInfo(member, ref strDisplayText2, ref strPhoneNumber))
                        return "";

                    if (strDisplayText == null)
                        strDisplayText = strDisplayText2;
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.ControlRoom)
                { 
                    if (!FormSOP.Instance.SOPManager.ControlRoomMembers.ContainsKey(commander.Team.TeamID))
                        return "";
                    else
                    {
                        strPhoneNumber = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].PhoneNumber;
                        if (strDisplayText == null)
                            strDisplayText = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].MemberName;
                    } 
                }
            }

            if (strDisplayText == null)
                return "";

            return strDisplayText;
        }

        public static List<TemporaryMember> GetTemporaryMembers(int nTeamID, bool isDayLight, bool includeMain, bool includeSub, bool includeTeamLeader, bool includeOthers)
        {
            List<TemporaryMember> members2 = new List<TemporaryMember>();
            List<TemporaryMember> members = FormSOP.Instance.SOPManager.GetTemporaryMembers(nTeamID, isDayLight);

            if (members == null || members.Count == 0)
                return members2;

            foreach (TemporaryMember member in members)
            {
                if (member._RoleType == TemporaryMember.RoleType.Main && includeMain)
                    members2.Add(member);
                else if (member._RoleType == TemporaryMember.RoleType.Sub && includeSub)
                    members2.Add(member);
                else if (member._RoleType == TemporaryMember.RoleType.TeamLeader && includeTeamLeader)
                    members2.Add(member);
                else if (includeOthers)
                    members2.Add(member);
            }

            return members2;
        }

        // 1. 비상조직의 [정] 관리자를 찾는다.
        // 2. [정]이 없으면 [부] 관리자를 찾는다.
        // 3. 그도 없으면 해당 조직의 아무나 리턴한다.
        private static TemporaryMember GetTemporaryMainMember(int nTeamID, bool isDayLight)
        {
            List<TemporaryMember> members = FormSOP.Instance.SOPManager.GetTemporaryMembers(nTeamID, isDayLight);

            if (members == null || members.Count == 0)
                return null;

            TemporaryMember subMain = null, teamLeader = null;

            foreach (TemporaryMember member in members)
            {
                if (member._RoleType == TemporaryMember.RoleType.Main)
                    return member;
                else if (member._RoleType == TemporaryMember.RoleType.Sub && subMain == null)
                    subMain = member;
                else if (member._RoleType == TemporaryMember.RoleType.TeamLeader && teamLeader == null)
                    teamLeader = member;
            }

            if (subMain != null)
                return subMain;

            if (teamLeader != null)
                return teamLeader;

            return members[0];
        }

        private static bool GetTemporaryMemberInfo(int nMemberID, bool isDayLight, ref string strDisplayText, ref string strPhoneNumber)
        {
            TemporaryMember member = FormSOP.Instance.SOPManager.GetTemporaryMember(nMemberID);

            if (member == null)
                return false;

            string strDisplayText2 = "";

            if (!GetTemporaryMemberInfo(member, ref strDisplayText2, ref strPhoneNumber))
                return false;

            if (strDisplayText == null)
                strDisplayText = strDisplayText2;

            return true;
        }

        public static bool GetTemporaryMemberInfo(TemporaryMember member, ref string strDisplayName, ref string strPhoneNumber, ref string strMemberName)
        {
            if (member == null)
                return false;

            strDisplayName = member.MemberName;

            if (member._MemberType == TemporaryMember.MemberType.RegularTeam)
            {
                Data_CompanyMember companyMember;
                Data_RegularTeam team = GetRegularTeamLeaderInfo(member.MemberID, out companyMember);

                if (team == null)
                    return false;

                strMemberName = companyMember.MemberName;
                strPhoneNumber = companyMember.PhoneNumber;
                return true;
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyTeam)
            {
                ExternalCompanyTeam team = FormSOP.Instance.SOPManager.FindExternalCompanyTeam(member.MemberID);

                if (team == null)
                    return false;

                return GetExternalFirstMemberPhoneNumber(team, ref strPhoneNumber, ref strMemberName);
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalTeam)
            {
                List<ExternalCompanyTeam> teams = FormSOP.Instance.SOPManager.GetExternalCompanyTeams(member.MemberID);

                if (teams == null || teams.Count == 0)
                    return false;

                ExternalCompanyTeam team = teams[0];
                return GetExternalFirstMemberPhoneNumber(team, ref strPhoneNumber, ref strMemberName);
            }
            else if (member._MemberType == TemporaryMember.MemberType.CompanyMember)
            {
                Data_CompanyMember companyMember = FormSOP.Instance.SOPManager.GetRegularCompanyMember(member.MemberID);

                if (companyMember == null)
                    return false;

                strPhoneNumber = companyMember.PhoneNumber;
                strMemberName = companyMember.MemberName;
                return true;
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyMember)
            {
                foreach (ExternalCompanyMember externalMember in FormSOP.Instance.SOPManager.ExternalCompanyMembers)
                {
                    if (externalMember.ID == member.MemberID)
                    {
                        strPhoneNumber = externalMember.PhoneNumber;
                        strMemberName = externalMember.MemberName;
                        return true;
                    }
                }

                return false;
            }
            else if (member._MemberType == TemporaryMember.MemberType.UserDefinedTeam)
            {
                Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeam(member.MemberID);

                if (team == null)
                    return false;

                strPhoneNumber = team.PhoneNumber;
                return true;
            }

            return false;
        }

        private static bool GetTemporaryMemberInfo(TemporaryMember member, ref string strDisplayName, ref string strPhoneNumber)
        {
            string strMemberName = "";
            return GetTemporaryMemberInfo(member, ref strDisplayName, ref strPhoneNumber, ref strMemberName);
        }

        private static Data_RegularTeam GetRegularTeamLeaderInfo(int nTeamID, out Data_CompanyMember member)
        {
            member = null;
            Data_RegularTeam team = LoadRegularTeam(nTeamID);

            if (team == null)
                return null;

            int nCompanyMemberID = GetRegularTeamLeaderID(team.ID);

            if (nCompanyMemberID < 0)
            {
                // 명시적으로 팀장이 선언되어 있지 않으면 팀장과 가장 가까운 직책을 선택한다.
                // 그마저도 없으면 가장 먼저 등록된 팀원을 리턴한다.
                nCompanyMemberID = GetRegularTeamLeaderID(team);

                if (nCompanyMemberID < 0)
                    return null;
            }

            member = FormSOP.Instance.SOPManager.GetRegularCompanyMember(nCompanyMemberID);

            if (member == null)
                return null;

            return team;
        }

        private static bool GetExternalFirstMemberPhoneNumber(ExternalCompanyTeam team, ref string strPhoneNumber, ref string strMemberName)
        {
            if (team.Members == null || team.Members.Count == 0)
                return false;

            ExternalCompanyMember externalMember = team.Members[0];
            strPhoneNumber = externalMember.PhoneNumber;
            strMemberName = externalMember.MemberName;
            return true;
        }

        // 명시적으로 팀장이 선언되어 있지 않으면 팀장과 가장 가까운 직책을 선택한다.
        // 그마저도 없으면 가장 먼저 등록된 팀원을 리턴한다.
        private static int GetRegularTeamLeaderID(Data_RegularTeam team)
        {
            ArrayList arrMembers = new ArrayList();
            if (FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(team.ID, ref arrMembers) == false)
                return -1;

            Data_CompanyMember teamLeader = null;

            foreach (Data_CompanyMember member in arrMembers)
            {
                if (teamLeader == null)
                    teamLeader = member;
                else
                {
                    int compare = teamLeader.CompareTo(member);

                    if (compare < 0)
                        teamLeader = member;
                    else if (compare == 0)
                    {
                        if (teamLeader.ID > member.ID)
                            teamLeader = member;
                    }
                }
            }

            /*string strSQL = "Select CompanyMemberID, PositionID from RegularMemberList where RegularTeamID = " + team.ID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            Data_CompanyMember teamLeader = null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount;i+=2 )
            {
                VariousData<int> companyMemberID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> positionID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (companyMemberID == null || positionID == null)
                    continue;

                Data_CompanyMember member = new Data_CompanyMember();
                member.ID = companyMemberID.Data;
                member.TeamPositions[team] = positionID.Data;

                if (teamLeader == null)
                    teamLeader = member;
                else
                {
                    int compare = teamLeader.CompareTo(member);

                    if (compare < 0)
                        teamLeader = member;
                    else if (compare == 0)
                    {
                        if (teamLeader.ID > member.ID)
                            teamLeader = member;
                    }
                }
            }

            if (teamLeader == null)
                return -1;*/

            if (teamLeader == null)
                return -1;

            return teamLeader.ID;
        }

        private static int GetRegularTeamLeaderID(int nTeamID)
        {
            string strSQL = "Select CompanyMemberID from RegularMemberList where RegularTeamID = " + nTeamID.ToString() + " and PositionID = 2";
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return WebDBManager.GetIntField(arrResult[0].ToString(), -1);
        }

        private static Data_RegularTeam LoadRegularTeam(int nTeamID)
        {
            string strSQL = "Select TeamName, ParentTeamID from RegularTeam where ID = " + nTeamID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            string strTeamName = WebDBManager.GetStringField(arrResult[0], "");
            int nParentTeamID = WebDBManager.GetIntField(arrResult[1].ToString(), -1);

            if (strTeamName == "null")
                return null;

            Data_RegularTeam team = new Data_RegularTeam();
            team.ID = nTeamID;
            team.ParentTeamID = nParentTeamID;
            team.TeamName = strTeamName;

            return team;
        }

		private void AddGridData(int nCount, bool[] bChecked, string[] szItem, Sections.Section secTarget, int nCheckNotify1, int nCheckNotify2)
		{
			ShowContainer();
			m_isFlag = false;
			int nRowHeight = 0;


            int count = 0;
			for (int i = 0; i < nCount; i++)
			{
                DataGridViewRow gridRow = new DataGridViewRow();
                gridRow.Height = 30;
                gridRow.DefaultCellStyle.Font = new System.Drawing.Font("맑은 고딕", 12.25F);

                /*DataGridViewCell cellTitle = new DataGridViewTextBoxCell();
                cellTitle.Value = secTarget.Title;
                gridRow.Cells.Add(cellTitle);*/

                DataGridViewCell cellActor = new DataGridViewTextBoxCell();
                cellActor.Value = GetActorName(secTarget);
                gridRow.Cells.Add(cellActor);

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = szItem[i];
                gridRow.Cells.Add(cell);

                DataGridViewCell cellNull2 = new DataGridViewTextBoxCell();
                cellNull2.Value = GetReceiverName(secTarget);
                gridRow.Cells.Add(cellNull2);

                /*DataGridViewCell cellNull1 = new DataGridViewTextBoxCell();
                cellNull1.Value = "";
                gridRow.Cells.Add(cellNull1);*/

                DataGridViewDisableButtonCell btnCell = new DataGridViewDisableButtonCell();
                btnCell.Value = "실행";
                gridRow.Cells.Add(btnCell);

                /*btnCell = new DataGridViewDisableButtonCell();
                btnCell.Value = "실행";
                gridRow.Cells.Add(btnCell);*/
				/*DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
				
				checkCell.Value = bChecked[i];
				checkCell.Tag = nCheckNotify1;
				gridRow.Cells.Add(checkCell);

				checkCell = new DataGridViewCheckBoxCell();
				gridRow.Cells.Add(checkCell);
				checkCell.Tag = nCheckNotify2;*/

                /*DataGridViewButtonCell cellButton = new DataGridViewDisableButtonCell();
                cellButton.Value = "실행";
                gridRow.Cells.Add(cellButton);*/

                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = false;
                gridRow.Cells.Add(checkCell);

                DataGridViewCell cell4 = new DataGridViewTextBoxCell();
                //cell4.Value = GetTimeString(DateTime.Now);
                gridRow.Cells.Add(cell4);

				nRowHeight = gridRow.Height;
                
                if (szItem[i] != null)
                {
                    if (szItem[i].IndexOf("팝업메시지", 0) > 0)
                        continue;
                }
                

				dataGridView.Rows.Add(gridRow);

                count++;
			}
			dataGridView.Tag = secTarget;
            dataGridView.Size = new Size(dataGridView.Width, dataGridView.ColumnHeadersHeight + (nRowHeight * count) + 3);

			//this.columnBroadcast.Visible = false;
			//this.Column2.Visible = false;
			this.Column3.Visible = false;
			labelTitle.Location = new Point(btnSMS.Location.X, labelTitle.Location.Y);
			btnSMS.Visible = false;
			btnBroadcast.Visible = false;

			ReSizeForm(m_isFlag);
		}

        private string GetTimeString(DateTime time)
        {
            // changed by mwkim 2015-11-23 무조건 {00:00} 형식의 시간으로 만 표기하도록 아래 IF문 주석처리
            //if (FormSOP.Instance.GetPageHome().CurrentOneTopPlayer == PageBackstageSOP.Player.ComponentContents)
            //{
            //    return string.Format("{0}월 {1}일 {2}시 {3}분 {4}초", time.Month, time.Day, time.Hour, time.Minute, time.Second);
            //}

            return string.Format("{0:00}:{1:00}", time.Hour, time.Minute);
        }

		public void UpdateContents(int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete)
		{
            int nRowCount = dataGridView.Rows.Count;

            if (this.Section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
			//if (btnSMS.Visible)  // Process
			{
				for (int i=0;i<nRowCount;i++)
				{
					DataGridViewRow row = dataGridView.Rows[i];
					int nBitFlag = 1 << i;

					//row.Cells[SMS_INDEX].Value = (nCheckedNotify1 & nBitFlag) == nBitFlag;
					//row.Cells[BROADCAST_INDEX].Value = (nCheckedNotify2 & nBitFlag) == nBitFlag;
				}
			}
			else
			{
				for (int i = 0; i < nRowCount; i++)
				{
					DataGridViewRow row = dataGridView.Rows[i];
					int nBitFlag = 1 << i;

					//row.Cells[SMS_INDEX].Value = (nCheckedNotify1 & nBitFlag) == nBitFlag;
				}
			}

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = dataGridView.Rows[i];
                int nBitFlag = 1 << i;
                bool enabled = true;

                if ((nCheckedRun & nBitFlag) == nBitFlag)
                    enabled = false;

                if ((nCheckedComplete & nBitFlag) == nBitFlag)
                {
                    enabled = false;
                    row.Cells[CONFIRM_COMPLETE_INDEX].Value = true;
                }
                else
                    row.Cells[CONFIRM_COMPLETE_INDEX].Value = false;

                row.Cells[CONFIRM_COMPLETE_INDEX].Tag = null;

                DataGridViewDisableButtonCell cell1 = (DataGridViewDisableButtonCell)row.Cells[SMS_INDEX];
                //DataGridViewDisableButtonCell cell2 = (DataGridViewDisableButtonCell)row.Cells[BROADCAST_INDEX];
                cell1.Enabled = /*cell2.Enabled = */enabled;
                //DataGridViewDisableButtonCell cell = (DataGridViewDisableButtonCell)row.Cells[DO_IT_INDEX];
                //cell.Enabled = enabled;
            }
		}

		public Image GetImage(bool isFlag)
		{
			Bitmap bmp = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.btn_arrow2);

			ImageList imgList = new ImageList();
			imgList.ImageSize = new Size(32, 32);
			imgList.Images.AddStrip(bmp);

			int nFlag = 0;
			if (!isFlag) nFlag = 1;

			Image img = imgList.Images[nFlag];

			return img;
		}

		public void ReSizeForm(bool isFlag)
		{
			Size szPanel = panel.Size;
			Size szGrid = VisibleExternalPanel ? panelExternal.Size : dataGridView.Size;

			if (!isFlag)
				this.Size = new Size(this.Width, szPanel.Height + szGrid.Height);
			else
				this.Size = new Size(this.Width, szPanel.Height);

			//FormSOP.Instance.GetPageHome().ReLocation();
		}

        public void AddMargin(int nMargin)
        {
            Size szPanel = panel.Size;
			Size szGrid = VisibleExternalPanel ? panelExternal.Size : dataGridView.Size;

            if (!m_isFlag)
                this.Size = new Size(this.Width, szPanel.Height + szGrid.Height + nMargin);
            else
                this.Size = new Size(this.Width, szPanel.Height);

            m_nAddMargin = nMargin;
        }

        public void RemoveMargin()
        {
            Size szPanel = panel.Size;
            Size szGrid = VisibleExternalPanel ? panelExternal.Size : dataGridView.Size;

            if (!m_isFlag)
                this.Size = new Size(this.Width, szPanel.Height + szGrid.Height);
            else
                this.Size = new Size(this.Width, szPanel.Height);

            m_nAddMargin = -1;
        }

		public Panel GetPanel()
		{
			return panel;
		}

		public DataGridView gridView
		{
			get { return dataGridView; }
		}

		private void pictureBoxBroadcast_MouseClick(object sender, MouseEventArgs e)
		{
			//pictureBoxClicked(BROADCAST_INDEX);
		}

		private void pictureBoxSMS_MouseClick(object sender, MouseEventArgs e)
		{
			pictureBoxClicked(SMS_INDEX);
		}

		private void pictureBoxClicked(int nColumnIndex)
		{
			int nRowCount = dataGridView.Rows.Count;

			if (nRowCount == 0)
				return;

			DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView.Rows[0].Cells[nColumnIndex];
			bool isChecked = !(bool)checkCell.EditedFormattedValue;

			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				row.Cells[nColumnIndex].Value = isChecked;
			}

			Sections.Section section = (Sections.Section)dataGridView.Tag;
			SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
			if (page == null)
			{
				PanelSectionEx panel = (PanelSectionEx)section.GetParent();
				page = (SectionTabPage)panel.Parent;
				FormSOP.Instance.GetPageHome().TabControls.SelectedTab = page;
			}
			SectionState state = WorkFlowManager.Instance.Find(section, !page.VirtualMode);

			if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS/* ||
				section.GetComponentType() == Sections.Section.ComponentType.INTERNAL*/)
			{
				int nCheckedNotify = 0;

				if (isChecked)
				{
					for (int i = 0; i < nRowCount; i++)
					{
						nCheckedNotify |= (1 << i);
					}
				}

				Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

				for (int i = 0; i < data.MissionItems.Count; i++)
				{
					Sections.MissionItem item = (Sections.MissionItem)data.MissionItems[i];

					MissionItemInfo info = FormSOP.Instance.GetMissionInfo(item);

					if (info == null)
					{
						info = new MissionItemInfo();

						if (nColumnIndex == SMS_INDEX)
						{
							info.UseSMS = isChecked;
							//info.UseBroadcast = (bool)dataGridView.Rows[i].Cells[BROADCAST_INDEX].EditedFormattedValue;
						}
						else
						{
							info.UseBroadcast = isChecked;
							info.UseSMS = (bool)dataGridView.Rows[i].Cells[SMS_INDEX].EditedFormattedValue;
						}

						FormSOP.Instance.SetMissionInfo(item, info);
					}
					else
					{
						if (nColumnIndex == SMS_INDEX)
							info.UseSMS = isChecked;
						else
							info.UseBroadcast = isChecked;
					}

					if (!info.UseSMS && !info.UseBroadcast)
						item.CheckItem = false;
					else
						item.CheckItem = true;

					int nBitFlag = 1 << i;
					state.CheckNotify1 = state.CheckNotify1 & (~nBitFlag);
					state.CheckNotify2 = state.CheckNotify2 & (~nBitFlag);

					if (info.UseSMS)
						state.CheckNotify1 = state.CheckNotify1 | nBitFlag;

					if (info.UseBroadcast)
						state.CheckNotify2 = state.CheckNotify2 | nBitFlag;

					if (i == 15)
						break;
				}

				if (nColumnIndex == SMS_INDEX)
					state.CheckNotify1 = nCheckedNotify;
				else
					state.CheckNotify2 = nCheckedNotify;

				if (LogGridRow != null)
				{
					LogGridRow.Cells[5].Tag = state.CheckNotify1;
					LogGridRow.Cells[6].Tag = state.CheckNotify2;
				}
			}
		}

        public bool GetItem(int nIndex, out bool isSendSMS, out bool isComplete, out string strSender, out string strItem, out string strTeamName, out string strPerformer)
        {
            //MISSION_ACTOR_INDEX 발신자
            //MISSION_TEXT_INDEX = 내용
            //MISSION_TARGET_INDEX = 수신자
            //SMS_INDEX = 문자버튼
            //CONFIRM_COMPLETE_INDEX = 완료체크
            //TIME_INDEX = 완료시간
            //MISSION_PERFORMER_INDEX 실행자

            isSendSMS = false;
            isComplete = false;
            strSender = "";
            strItem = "";
            strTeamName = "";
            strPerformer = "";

            if (nIndex < 0 || nIndex >= ItemCount)
                return false;

            DataGridViewDisableButtonCell cellBtn = (DataGridViewDisableButtonCell)dataGridView.Rows[nIndex].Cells[SMS_INDEX];

            if (cellBtn.Tag != null)
            {
                isSendSMS = true;
            }

            strSender = (string)dataGridView.Rows[nIndex].Cells[MISSION_ACTOR_INDEX].Value;
            strItem = (string)dataGridView.Rows[nIndex].Cells[MISSION_TEXT_INDEX].Value;
            strTeamName = (string)dataGridView.Rows[nIndex].Cells[MISSION_TARGET_INDEX].Value;
            isComplete = (bool)dataGridView.Rows[nIndex].Cells[CONFIRM_COMPLETE_INDEX].Value;
            strPerformer = (string)dataGridView.Rows[nIndex].Cells[MISSION_PERFORMER_INDEX].Value;

            return true;
        }

        public bool GetItem(out bool isBroadcast, out bool isExcute, out bool isComplete)
        {
            bool bReturn = false;

            isBroadcast = false;
            isExcute = false;
            isComplete = false;

            Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();
            if (frm != null)
            {
                bReturn = true;

                isBroadcast = frm.UseBroadcast;
                isExcute = (frm.ExecuteTime != null);
                isComplete = frm.IsComplete;
            }

            return bReturn;
        }

		public void EnableGrid(bool enabled)
		{
            this.Disabled = !enabled;
            //this.Enabled = enabled;
            //dataGridView.Enabled = panelExternal.Enabled = enabled;
			/*int nColumnCount = gridView.Columns.Count;
			if (nColumnCount == 0)
				return;

			Color disabledGridColor = Color.LightGray;

			foreach (DataGridViewRow row in gridView.Rows)
			{
				for (int i = 0; i < nColumnCount - 1; i++)
				{
					row.Cells[i].ReadOnly = !enabled;
				}

				if (!enabled)
				{
					for (int i=0;i<nColumnCount;i++)
					{
						row.Cells[i].Style.BackColor = disabledGridColor;
					}
				}
			}

			btnSMS.Enabled = enabled;
			btnBroadcast.Enabled = enabled;*/
		}

        private void OnDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormSOP.Instance.GetPageHome().OneTop(PageBackstageSOP.Player.ComponentContents);
        }

        private bool IsEndSection()
        {
            if (this.Section is SectionEndPoint)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)this.Section.Data;
                return !data.IsBegin;
            }

            return false;
        }

        public void SetStateColor()
        {
            if (m_state == UnE.SOP.Workstate.State.DONE)
            {
                if (IsEndSection())
                {
                    if (!m_checkEndState)
                    {
                        m_checkEndState = true;
                        FormSOP.Instance.GetPageHome().SelectComponentContents(this);
                    }
                }
                else
                    this.panelTitle.BackColor = this.panel.BackColor = WorkFlowManager.Instance.CompleteColor;
            }
            else if (m_state == UnE.SOP.Workstate.State.INPUT)
            {
                this.panelTitle.BackColor = this.panel.BackColor = WorkFlowManager.Instance.InputWaitColor;
            }
            else if (m_state == UnE.SOP.Workstate.State.NORMAL)
            {
                this.panelTitle.BackColor = this.panel.BackColor = WorkFlowManager.Instance.NoramlColor;
            }
            else if (m_state == UnE.SOP.Workstate.State.RUN)
            {
                this.panelTitle.BackColor = this.panel.BackColor = WorkFlowManager.Instance.InProgressColor;
            }
            else if (m_state == UnE.SOP.Workstate.State.SKIP)
            {
                this.panelTitle.BackColor = this.panel.BackColor = WorkFlowManager.Instance.SkipColor;
            }
        }

        public void SetTitleColor(Color color)
        {
            //if (m_state == UnE.SOP.Workstate.State.DONE)
            {
                this.panelTitle.BackColor = this.panel.BackColor = color;
            }
        }

        public Color GetTitleColor()
        {
            return this.panelTitle.BackColor;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.Section == null)
                return;

            if (this.Disabled)
                return;

            int nLimit = 20;
            Sections.Section.ComponentType type = Section.GetComponentType();

            if (type == Sections.Section.ComponentType.DECISION)
            {
                if (cboDecisions.SelectedIndex < 0)
                    return;

                m_prevDecisionProcessButton = (DecisionProcessButton)cboDecisions.Items[cboDecisions.SelectedIndex];
                m_prevDecisionProcessButton.ProcessButton.OnClick();
            }
            else if (btnNext.Text == "시작")
            {
                FormSOP.Instance.RunWorkflowWithEvent();
            }
            else if (btnNext.Text == "종료")
            {
                for (int i=0;i<nLimit;i++)
                {
                    ISectionPainter painter = this.Section.GetSectionPainter(i);

                    if (painter == null)
                        break;

                    if (painter is ProcessRectButtonManager)
                    {
                        ProcessRectButtonManager mgr = (ProcessRectButtonManager)painter;
                        ProcessButton btn = mgr.FindButton();

                        if (btn != null)
                        {
                            btn.OnClick();
                        }

                        break;
                    }
                }
            }
            else if (btnNext.Text == "다음")
            {
                for (int i = 0; i < nLimit; i++)
                {
                    ISectionPainter painter = this.Section.GetSectionPainter(i);

                    if (painter == null)
                        break;

                    if (painter is ProcessRectButtonManager)
                    {
                        ProcessRectButtonManager mgr = (ProcessRectButtonManager)painter;
                        ProcessButton btn = mgr.FindButton();

                        if (btn != null)
                        {
                            // 실행완료된 상태이더라도 다시 실행할 수 있도록 한다.
                            if (btn.Status == ProcessButton.ButtonStatus.DONE)
                            {
                                btn.Status = ProcessButton.ButtonStatus.WAIT;

                                SectionTabPage page = (SectionTabPage)this.Section.GetParent().Parent;
                                SectionState state = WorkFlowManager.Instance.Find(this.Section, !page.VirtualMode);

                                if (state == null)
                                    return;

                                if (state.State == UnE.SOP.Workstate.State.DONE)
                                    state.State = UnE.SOP.Workstate.State.NORMAL;
                            }

                            btn.OnClick();
                        }

                        break;
                    }
                }
            }
        }

        private void cboDecisions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemCall)
                return;

            if (cboDecisions.SelectedIndex < 0)
                return;

            if (m_runMode != RunMode.Run)
                return;

            DecisionProcessButton btn = (DecisionProcessButton)cboDecisions.Items[cboDecisions.SelectedIndex];

            if (m_prevDecisionProcessButton == btn)
                return;

            // 이미 한번 실행한 상태일 경우 다시 재실행 할수 있도록 만든다.
            if (Section != null && btnNext.Enabled == false)
            {
                Sections.Section.ComponentType type = Section.GetComponentType();

                if (type == Sections.Section.ComponentType.DECISION)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)this.Section.GetParent();
                    SectionTabPage tabPage = (SectionTabPage)panel.Parent;
                    SectionState state = WorkFlowManager.Instance.Find(Section, !tabPage.VirtualMode);

                    if (state != null)
                    {
                        state.InitState();
                        btnNext.Enabled = true;
                    }
                }
            }
        }

        public void RunDecision(ProcessButton btn, bool initState)
        {
            if (Section == null || Section.GetComponentType() != Sections.Section.ComponentType.DECISION)
                return;

            if (PostRunDecision(btn))
            {
                if (initState)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)this.Section.GetParent();
                    SectionTabPage tabPage = (SectionTabPage)panel.Parent;
                    SectionState state = WorkFlowManager.Instance.Find(Section, !tabPage.VirtualMode);

                    if (state != null)
                    {
                        // 재실행하는 것이므로 일단 초기화시킨다.
                        state.InitState();
                    }
                }

                btn.OnClick();
            }
            /*int nItemCount = cboDecisions.Items.Count;

            for (int i=0;i<nItemCount;i++)
            {
                DecisionProcessButton button = (DecisionProcessButton)cboDecisions.Items[i];

                if (button.ProcessButton == btn)
                {
                    cboDecisions.SelectedIndex = i;

                    if (initState)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)this.Section.GetParent();
                        SectionTabPage tabPage = (SectionTabPage)panel.Parent;
                        SectionState state = WorkFlowManager.Instance.Find(Section, !tabPage.VirtualMode);

                        if (state != null)
                        {
                            // 재실행하는 것이므로 일단 초기화시킨다.
                            state.InitState();
                        }
                    }

                    btn.OnClick();
                    btnExecute.Enabled = false;
                    m_prevDecisionProcessButton = button;
                    break;
                }
            }*/
        }

        private bool PostRunDecision(ProcessButton btn)
        {
            int nItemCount = cboDecisions.Items.Count;

            for (int i = 0; i < nItemCount; i++)
            {
                DecisionProcessButton button = (DecisionProcessButton)cboDecisions.Items[i];

                if (button.ProcessButton == btn)
                {
                    m_systemCall = true;
                    cboDecisions.SelectedIndex = i;
                    m_systemCall = false;

                    btnExecute.Enabled = false;
                    m_prevDecisionProcessButton = button;
                    return true;
                }
            }

            return false;
        }

        // nComponentHistoryID : 이 값이 0보다 작으면 ComponentHistoryID가 생성된 이후에 데이터가 옮겨진다.
        // onlyLastData : 이 값이 true이면 detailDatas에 여러 데이터가 저장될 경우 가장 마지막에 저장된 데이터만 남긴다.
        //                DB에 중복 로그를 저장하지 않도록 하기 위함이다.
        public void SetDetailDatas(Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> detailDatas, int nComponentHistoryID = -1, bool onlyLastData = true)
        {
            if (this.Section == null)
                return;

            Section.ComponentType type = this.Section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)row.Cells[CONFIRM_COMPLETE_INDEX];
                    DataGridViewDisableButtonCell btnCell = (DataGridViewDisableButtonCell)row.Cells[SMS_INDEX];
                    DataGridViewCell timeCell = row.Cells[TIME_INDEX];

                    if (checkCell.Value != null/* && (bool)checkCell.Value == true*/)
                    {
                        if ((bool)checkCell.Value == true)
                        {
                            // 완료 버튼이 눌려졌다.
                            if (timeCell.Tag == null || (timeCell.Tag is DateTime) == false)
                                continue;

                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_CHECKED, (DateTime)timeCell.Tag);
                            AddDetailData(row.Index, detailDatas, detail, nComponentHistoryID);
                        }
                        else if (checkCell.Tag != null)
                        {
                            // 완료 버튼이 해제됐다.
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_UNCHECKED, row.Tag == null ? DateTime.Now : (DateTime)row.Tag);
                            AddDetailData(row.Index, detailDatas, detail, nComponentHistoryID);
                        }
                    }

                    if (!btnCell.Enabled && btnCell.Tag != null && btnCell.Tag is DateTime)
                    {
                        UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.SEND_SMS, (DateTime)btnCell.Tag);
                        AddDetailData(row.Index, detailDatas, detail, nComponentHistoryID);
                    }
                }
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();

                if (frm != null)
                {
                    VariousData<DateTime> dtExecute = frm.ExecuteTime;
                    VariousData<DateTime> dtComplete = frm.CompleteTime;
                    VariousData<DateTime> dtUncomplete = frm.UncompleteTime;

                    if (frm.UseBroadcast)
                    {
                        int nBroadcastCount;
                        bool useSiren, runExecute, checkedComplete;
                        string strMsg;

                        frm.GetBroadcastOptions(out nBroadcastCount, out useSiren, out runExecute, out checkedComplete, out strMsg);

                        if (runExecute && dtExecute != null)
                        {
                            string strLog = string.Format("{0}, {1}, {2}", nBroadcastCount, useSiren ? 1 : 0, strMsg);
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(strLog, dtExecute.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.RUN_BROADCAST_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }

                        if (checkedComplete && dtComplete != null)
                        {
                            // 완료버튼을 누른 경우
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(1, dtComplete.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_BROADCAST_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }
                        else if (frm.UncheckedComplete)
                        {
                            // 완료버튼을 해제한 경우
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(0, dtUncomplete.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_BROADCAST_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }
                    }
                    else
                    {
                        string strCommanderText, strReceiverText, strMsg;
                        bool runExecute, checkedComplete;

                        if (frm.GetSMSOptions(out strCommanderText, out strReceiverText, out runExecute, out checkedComplete, out strMsg))
                        {
                            if (runExecute && dtExecute != null)
                            {
                                string strLog = string.Format("[{0}], [{1}], {2}", strCommanderText, strReceiverText, strMsg);
                                UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(strLog, dtExecute.Data);
                                AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.RUN_SMS_INTERNAL, detailDatas, detail, nComponentHistoryID);
                            }

                            if (checkedComplete && dtComplete != null)
                            {
                                // 완료버튼을 누른 경우
                                UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(1, dtComplete.Data);
                                AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_SMS_INTERNAL, detailDatas, detail, nComponentHistoryID);
                            }
                            else if (frm.UncheckedComplete)
                            {
                                // 완료버튼을 해제한 경우
                                UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(0, dtUncomplete.Data);
                                AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_SMS_INTERNAL, detailDatas, detail, nComponentHistoryID);
                            }
                        }
                    }
                }
            }

            if (onlyLastData)
            {
                List<UnE.SOP.History.HistorySectionData.DetailData> datas = null;
                UnE.SOP.History.HistorySectionData.DetailData lastData = null;
                int nKey = -1;
                bool isFirst = true;

                foreach (KeyValuePair<int, List<UnE.SOP.History.HistorySectionData.DetailData>> pair in detailDatas)
                {
                    foreach (UnE.SOP.History.HistorySectionData.DetailData data in pair.Value)
                    {
                        if (isFirst)
                        {
                            lastData = data;
                            datas = pair.Value;
                            nKey = pair.Key;
                            isFirst = false;
                        }
                        else
                        {
                            if ((lastData.Time == null && data.Time != null) ||
                                (lastData.Time != null && data.Time != null && lastData.Time.Data < data.Time.Data))
                            {
                                lastData = data;
                                datas = pair.Value;
                                nKey = pair.Key;
                            }
                        }
                    }
                }

                if (lastData != null)
                {
                    detailDatas.Clear();
                    datas.Clear();
                    datas.Add(lastData);
                    detailDatas[nKey] = datas;
                }
            }
        }

        private void AddDetailData(int nRowIndex, Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> detailDatas, UnE.SOP.History.HistorySectionData.DetailData detail, int nComponentHistoryID)
        {
            detail.DataIndex = new VariousData<int>(nRowIndex);

            foreach (KeyValuePair<int, List<UnE.SOP.History.HistorySectionData.DetailData>> pair in detailDatas)
            {
                foreach (UnE.SOP.History.HistorySectionData.DetailData data in pair.Value)
                {
                    // 이미 같은 데이터가 존재하면 다시 저장하지 않는다.
                    if (data.Equals(detail))
                        return;
                }
            }

            List<UnE.SOP.History.HistorySectionData.DetailData> details = null;

            if (!detailDatas.TryGetValue(nComponentHistoryID, out details))
            {
                details = new List<UnE.SOP.History.HistorySectionData.DetailData>();
                detailDatas[nComponentHistoryID] = details;
            }

            /*if (!detailDatas.TryGetValue(nRowIndex, out details))
            {
                details = new List<UnE.SOP.History.HistorySectionData.DetailData>();
                detailDatas[nRowIndex] = details;
            }*/

            if (!details.Contains(detail))
                details.Add(detail);
        }

        private bool CheckDetailCount(int nComponentHistoryID, List<UnE.SOP.History.HistorySectionData.DetailData> detailDatas)
        {
            if (detailDatas.Count == 0)
                return false;

            //UnE.SOP.History.HistorySectionData.DetailData data = detailDatas[0];
            //string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", data.Time.Data.Year, data.Time.Data.Month, data.Time.Data.Day, data.Time.Data.Hour, data.Time.Data.Minute, data.Time.Data.Second);

            //string strSQL2 = "Select max(ComponentHistoryID) from ComponentHistoryDetail where Time = '" + strTime + "'";
            //ArrayList arrResult2 = FormSOP.Instance.DBManager.GetResultData(strSQL2);

            //if (arrResult2 == null || arrResult2.Count == 0)
            //    return false;

            //DBUtility.VariousData<int> nComponentHistoryID = WebDBManager.GetIntField(arrResult2[0].ToString());

            //if (nComponentHistoryID == null)
            //    return false;

            string strSQL = string.Format("Select count(ComponentHistoryID) from ComponentHistoryDetail where ComponentHistoryID = {0}", nComponentHistoryID);
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> nCount = WebDBManager.GetIntField(arrResult[0].ToString());

            if (nCount == null)
                return false;

            if (nCount.Data == detailDatas.Count)
            {
                //System.Diagnostics.Trace.WriteLine("First DetailData ComponentHistoryID : " + nComponentHistoryID.ToString() + ", Count : " + nCount.Data.ToString());
                return true;
            }

            FormSOP.Instance.DBManager.GetResultData(strSQL);
            return false;
        }

        // detailDatas 정보를 ComponentContents 객체에 적용한다.
        public void ApplyDetailDatas(int nComponentHistoryID, List<UnE.SOP.History.HistorySectionData.DetailData> detailDatas)
        {
            if (!CheckDetailCount(nComponentHistoryID, detailDatas))
            {
                //System.Diagnostics.Trace.WriteLine("Count Error");
            }

            Popup.MissionMessage.FormMissionMessage frm = null;
            //System.Diagnostics.Trace.WriteLine("ApplyDetailDatas");

            foreach (UnE.SOP.History.HistorySectionData.DetailData detail in detailDatas)
            {
                if (detail.DataIndex == null)
                {
                    //System.Diagnostics.Trace.WriteLine("DataIndex is null");
                    continue;
                }
                /*else
                    System.Diagnostics.Trace.WriteLine("DataIndex is " + detail.DataIndex.Data.ToString());*/

                if (detail.DataIndex.Data >= 0)
                    ApplyDetailData(detail.DataIndex.Data, detail.Datai, detail.Time, false);
                else
                {
                    if (frm == null)
                        frm = GetFormMissionMessage();

                    ApplyFormMissionMessageDetailData(frm, detail);
                }
            }

            FormSOP.Instance.GetPageHome().RefreshComponentContents(this);
            //dataGridView.Refresh();
        }

        private void ApplyFormMissionMessageDetailData(Popup.MissionMessage.FormMissionMessage frm, UnE.SOP.History.HistorySectionData.DetailData detail)
        {
            if (frm == null)
                return;

            if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.RUN_SMS_INTERNAL)
            {
                if (frm.UseBroadcast || detail.Datas == null)
                    return;

                string strCommander, strCommanderDisplayText, strReceivers, strMsg;
                bool onlyTeamLeader;

                if (ParseRunSMSInternal(detail.Datas, out strCommander, out strCommanderDisplayText, out strReceivers, out onlyTeamLeader, out strMsg))
                {
                    Sections.SectionCommander commander = LoadCommander(strCommander);
                    List<SOPTeam> receivers = LoadReceivers(strReceivers);

                    frm.SetSMSOptions(commander, strCommanderDisplayText, receivers, new VariousData<bool>(onlyTeamLeader), strMsg, new VariousData<bool>(true), detail.Time, null, null);
                }
            }
            else if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.RUN_BROADCAST_INTERNAL)
            {
                if (!frm.UseBroadcast || detail.Datas == null)
                    return;

                int nBroadcastCount;
                bool useSiren;
                string strMsg;

                if (ParseRunBroadcastInternal(detail.Datas, out nBroadcastCount, out useSiren, out strMsg))
                {
                    frm.SetBroadcastOptions(new VariousData<int>(nBroadcastCount), new VariousData<bool>(useSiren), new VariousData<bool>(true), detail.Time, null, null, strMsg);
                }
            }
            else if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_SMS_INTERNAL)
            {
                if (frm.UseBroadcast || detail.Datai == null)
                    return;

                VariousData<bool> completed = null;

                if (detail.Datai.Data == 1)
                    completed = new VariousData<bool>(true);
                else if (detail.Datai.Data == 0)
                    completed = new VariousData<bool>(false);
                else
                    return;

                frm.SetSMSOptions(null, null, null, null, null, null, null, completed, detail.Time);
            }
            else if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_BROADCAST_INTERNAL)
            {
                if (!frm.UseBroadcast || detail.Datai == null)
                    return;

                VariousData<bool> completed = null;

                if (detail.Datai.Data == 1)
                    completed = new VariousData<bool>(true);
                else if (detail.Datai.Data == 0)
                    completed = new VariousData<bool>(false);
                else
                    return;

                frm.SetBroadcastOptions(null, null, null, null, completed, detail.Time, null);
            }
        }

        private bool ParseRunBroadcastInternal(string strOrigin, out int nBroadcastCount, out bool useSiren, out string strMsg)
        {
            nBroadcastCount = 1;
            useSiren = true;
            strMsg = "";

            int nIndex1 = strOrigin.IndexOf(',');

            if (nIndex1 < 0)
                return false;

            int nIndex2 = strOrigin.IndexOf(',', nIndex1 + 1);

            if (nIndex2 < 0)
                return false;

            string strBroadcastCount = "", strSiren = "";

            if (nIndex1 > 0)
                strBroadcastCount = strOrigin.Substring(0, nIndex1).Trim();

            if (nIndex2 - nIndex1 > 1)
                strSiren = strOrigin.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

            int.TryParse(strBroadcastCount, out nBroadcastCount);

            int nSiren;
            
            if (int.TryParse(strSiren, out nSiren))
            {
                if (nSiren == 0)
                    useSiren = false;
            }

            if (nIndex2 < strOrigin.Length - 1)
                strMsg = strOrigin.Substring(nIndex2 + 1).Trim();

            return true;
        }

        private List<SOPTeam> LoadReceivers(string strReceivers)
        {
            return IOManager.LoadTeamList(FormSOP.Instance.DBManager, strReceivers);
        }

        private Sections.SectionCommander LoadCommander(string strCommander)
        {
            int nCommanderMemberType = -2, nCommanderMemberID = -2;

            strCommander = strCommander.Trim();

            if (strCommander.Length > 0)
            {
                int nIndex1 = strCommander.IndexOf('(');
                int nIndex2 = strCommander.IndexOf(')');

                string strID = "", strType = "";

                if (nIndex1 > 0 && nIndex2 > nIndex1)
                {
                    strID = strCommander.Substring(0, nIndex1);

                    if (nIndex2 - nIndex1 > 1)
                        strType = strCommander.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                }

                if (strID.Length > 0)
                    int.TryParse(strID, out nCommanderMemberID);

                if (strType.Length > 0)
                    int.TryParse(strType, out nCommanderMemberType);

                return IOManager.LoadCommanderTeamMember(FormSOP.Instance.DBManager, nCommanderMemberType, nCommanderMemberID, "");
            }

            return null;
        }

        private bool ParseRunSMSInternal(string strOrigin, out string strCommander, out string strCommanderDisplayText, out string strReceivers, out bool onlyTeamLeader, out string strMsg)
        {
            strCommanderDisplayText = "";
            strCommander = strReceivers = strMsg = "";
            onlyTeamLeader = true;

            int nIndex1 = strOrigin.IndexOf('[');
            int nIndex2 = strOrigin.IndexOf(']');
            int nIndex3 = strOrigin.IndexOf(',');

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                return false;

            if (nIndex3 > nIndex1 && nIndex3 < nIndex2)
            {
                if (nIndex2 - nIndex3 > 1)
                    strCommanderDisplayText = strOrigin.Substring(nIndex3 + 1, nIndex2 - nIndex3 - 1).Trim();

                if (nIndex3 - nIndex1 > 1)
                    strCommander = strOrigin.Substring(nIndex1 + 1, nIndex3 - nIndex1 - 1).Trim();
            }
            else
            {
                if (nIndex2 - nIndex1 > 1)
                    strCommander = strOrigin.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            }

            nIndex1 = strOrigin.IndexOf(',', nIndex2 + 1);
            nIndex2 = strOrigin.IndexOf('[', nIndex2 + 1);

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                return false;

            nIndex3 = strOrigin.IndexOf(']', nIndex2 + 1);

            if (nIndex3 < 0 || nIndex3 <= nIndex2)
                return false;

            int nIndex4 = strOrigin.LastIndexOf(',', nIndex3);
            int nIndex5 = strOrigin.LastIndexOf(')', nIndex3);

            if (nIndex4 > nIndex5 && nIndex4 > nIndex2)
            {
                int nOnlyTeamLeader;
                string strOnlyTeamLeader = "";

                if (nIndex3 - nIndex4 > 1)
                    strOnlyTeamLeader = strOrigin.Substring(nIndex4 + 1, nIndex3 - nIndex4 - 1).Trim();

                if (int.TryParse(strOnlyTeamLeader, out nOnlyTeamLeader) && nOnlyTeamLeader == 0)
                    onlyTeamLeader = false;
            }

            if (nIndex4 - nIndex2 > 1)
                strReceivers = strOrigin.Substring(nIndex2 + 1, nIndex4 - nIndex2 - 1);

            nIndex1 = strOrigin.IndexOf(',', nIndex3 + 1);

            if (nIndex1 < 0)
                return false;

            strMsg = strOrigin.Substring(nIndex1 + 1).Trim();
            return true;
        }

        private void ApplyDetailData(int nRowIndex, VariousData<int> data, VariousData<DateTime> time, bool bRefresh = true)
        {
            if (data == null)
                return;

            if (dataGridView.Rows.Count <= nRowIndex)
                return;

            DataGridViewRow row = dataGridView.Rows[nRowIndex];

            if (row.IsNewRow)
                return;

            if (data.Data == (int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_CHECKED)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[CONFIRM_COMPLETE_INDEX];
                cell.Value = true;

                ((DataGridViewDisableButtonCell)row.Cells[SMS_INDEX]).Enabled = false;

                if (time != null)
                {
                    row.Cells[TIME_INDEX].Value = GetTimeString(time.Data);
                    row.Cells[TIME_INDEX].Tag = time.Data;
                }
                else
                {
                    row.Cells[TIME_INDEX].Value = null;
                    row.Cells[TIME_INDEX].Tag = null;
                }
            }
            else if (data.Data == (int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_UNCHECKED)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[CONFIRM_COMPLETE_INDEX];
                cell.Value = false;

                ((DataGridViewDisableButtonCell)row.Cells[SMS_INDEX]).Enabled = true;

                if (time != null)
                    row.Tag = time.Data;
                else
                    row.Tag = null;
            }
            else if (data.Data == (int)UnE.SOP.History.HistorySectionData.DetailData.DataType.SEND_SMS)
            {
                DataGridViewDisableButtonCell cell = (DataGridViewDisableButtonCell)row.Cells[SMS_INDEX];
                cell.Enabled = false;

                if (time != null)
                    cell.Tag = time.Data;
            }

            if (bRefresh == true)
            {
                FormSOP.Instance.GetPageHome().RefreshComponentContents(this);
                //dataGridView.Refresh();
            }
        }

        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (this.Disabled)
                e.Cancel = true;
        }

        public void ResizeGrid()
        {
            //const int LAST_COLUMN_DEFAULT_WIDTH = 55;

            //dataGridView.Columns[TIME_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dataGridView.Columns[MISSION_TEXT_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dataGridView.Columns[TIME_INDEX].Width = LAST_COLUMN_DEFAULT_WIDTH;

            //dataGridView.Columns[MISSION_TEXT_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dataGridView.Columns[TIME_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void labelSender_TextChanged(object sender, EventArgs e)
        {
            labelSender.Location = new Point(labelTitle.Location.X + labelTitle.Size.Width + 15, labelSender.Location.Y);
        }

        private void AutoRun()
        {
            Section section = this.Section;

            if (section is SectionDecision)
            {
                SectionDataDecision data = (SectionDataDecision)section.Data;

                if (data.Expression.Length > 0)
                {
                    string strError;

                    bool result = LogicalScriptParser.Execute(data.Expression, out strError);

                    if (strError.Length == 0)
                        AutoRunDecision(result);
                }
            }
            else if (section is SectionInternal)
            {
                SectionDataInternal data = (SectionDataInternal)section.Data;

                if (data.AutoRun)
                    AutoRunInternal();
            }
            else if (section is SectionProcess)
            {
                SectionDataProcess data = (SectionDataProcess)section.Data;

                if (data.AutoRun)
                    AutoRunProcess(data);
            }
        }

        private void AutoRunProcess(SectionDataProcess data)
        {
            int nMissionCount = data.MissionItems.Count;

            for (int i = 0; i < nMissionCount;i++ )
            {
                /*MissionItem item = (MissionItem)data.MissionItems[i];

                if (item is MissionItemExternal)
                {
                    RunExecute((MissionItemExternal)item);
                }*/

                if (i >= dataGridView.Rows.Count)
                    continue;

                try
                {
                    DataGridViewRow row = dataGridView.Rows[i];
                    row.Cells[SMS_INDEX].AccessibilityObject.DoDefaultAction();
                    row.Cells[CONFIRM_COMPLETE_INDEX].Value = true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }

            btnNext_Click(null, null);
        }

        private void RunExecute(MissionItemExternal item)
        {
            string strWorkingDirectory = ".\\", strExe = item.ExternalExeFilePath;

            int nIndex = item.ExternalExeFilePath.LastIndexOf('\\');

            if (nIndex >= 0)
            {
                strWorkingDirectory = strExe.Substring(0, nIndex + 1);
                strExe = strExe.Substring(nIndex + 1);
            }

            string strArguments = "";

            foreach (string strArgument in item.Arguments)
            {
                if (strArguments.Length > 0)
                    strArguments += " ";

                strArguments += "\"" + strArgument.Replace("\"", "\\\"") + "\"";

                /*if (strArgument.StartsWith("\"") && strArgument.EndsWith("\""))
                    strArguments += strArgument;
                else
                    strArguments += "\"" + strArgument + "\"";*/
            }

            PanelSectionEx panel = (PanelSectionEx)this.Section.GetParent();
            SectionTabPage page = (SectionTabPage)panel.Parent;
            int nActionStepHistoryID = page.ActionStepHistoryID;

            string strProcessName = GetExeName(strExe);

            if (System.IO.File.Exists(strWorkingDirectory + strExe))
            {
                // 외부연계 프로그램에서 호출한 SOP에 대한 정보를 알수 있도록 하기 위하여 ActionStepHistoryID를 별도의 파일에 기록해둔다.
                WriteActionStepHistoryID(nActionStepHistoryID, strProcessName, strWorkingDirectory);

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strExe;
                startInfo.WorkingDirectory = strWorkingDirectory;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = strArguments;

                System.Diagnostics.Process process;

                try
                {
                    process = System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    //System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // 외부연계 프로그램에서 호출한 SOP에 대한 정보를 알수 있도록 하기 위하여 ActionStepHistoryID를 별도의 파일에 기록해둔다.
                WriteActionStepHistoryID(nActionStepHistoryID, strProcessName);

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strExe;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = strArguments.Substring(1, strArguments.Length - 2);

                System.Diagnostics.Process process;

                try
                {
                    process = System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    //System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }

        private void WriteActionStepHistoryID(int nActionStepHistoryID, string strProcessName, string strWorkingDirectory = "")
        {
            string strPath = strProcessName + ".aid";

            if (strWorkingDirectory.Length > 0)
            {
                if (strWorkingDirectory.EndsWith("\\"))
                    strPath = strWorkingDirectory + strPath;
                else
                    strPath = strWorkingDirectory + "\\" + strPath;
            }

            if (File.Exists(strPath))
            {
                int nReadID = ReadAID(strPath);

                // 이미 nActionStepHistoryID가 쓰여져 있으면 다시 파일을 생성하지 않는다.
                if (nReadID == nActionStepHistoryID)
                    return;
                else
                    WriteAID(strPath, nActionStepHistoryID);
            }
            else
                WriteAID(strPath, nActionStepHistoryID);
        }

        private void WriteAID(string strPath, int nActionStepHistoryID)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(nActionStepHistoryID.ToString());

            using (var fs = File.Open(strPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        private int ReadAID(string strPath)
        {
            byte[] bytes;
            int nActionStepHistoryID;

            using (var fs = File.Open(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);

                string str = Encoding.UTF8.GetString(bytes);

                if (int.TryParse(str.Trim(), out nActionStepHistoryID))
                    return nActionStepHistoryID;
            }

            return -1;
        }

        private string GetExeName(string strExe)
        {
            int nSlashIndex1 = strExe.LastIndexOf('/');
            int nSlashIndex2 = strExe.LastIndexOf('\\');
            int nSlashIndex = -1;

            if (nSlashIndex1 >= 0 && nSlashIndex2 >= 0)
            {
                if (nSlashIndex1 > nSlashIndex2)
                    nSlashIndex = nSlashIndex1;
                else
                    nSlashIndex = nSlashIndex2;
            }
            else if (nSlashIndex1 >= 0)
            {
                nSlashIndex = nSlashIndex1;
            }
            else if (nSlashIndex2 >= 0)
            {
                nSlashIndex = nSlashIndex2;
            }

            if (nSlashIndex >= 0)
                strExe = strExe.Substring(nSlashIndex + 1);

            int nDotIndex = strExe.LastIndexOf('.');

            if (nDotIndex >= 0)
            {
                strExe = strExe.Substring(0, nDotIndex);
            }

            return strExe;
        }

        private void AutoRunInternal()
        {
            foreach (Control ctl in panelExternal.Controls)
            {
                if (ctl is Popup.MissionMessage.FormMissionMessage)
                {
                    bool noSMS = false;

                    if (Section != null && Section.Data != null && Section.Data is SectionDataInternal)
                    {
                        SectionDataInternal data = (SectionDataInternal)Section.Data;

                        // UsePopupMessage는 PC용 팝업 기능인데 이 기능은 현재 사용하지 않는다.
                        // SOP Loading 속도가 느릴 경우 SOP내에 포함된 문자전송 기능을 사용하려면 시간이 많이 걸리게 될 경우
                        // 해당 문자는 다른곳에서 먼저 보내도록 하고, SOP 자동실행시에는 이 문자를 보내지 않도록 한다.
                        // UsePopupMessage가 true일 경우 SOP 자동실행시에 문자를 보내지 않는다.
                        // [2017/10/13] 김지웅
                        if (data.UsePopupMessage == true)
                            noSMS = true;
                    }

                    (ctl as Popup.MissionMessage.FormMissionMessage).Run(noSMS);
                    break;
                }
            }
        }

        private void AutoRunDecision(bool yesno)
        {
            if (cboDecisions.Disabled || cboDecisions.Visible == false)
                return;

            int nItemCount = cboDecisions.Items.Count;

            for (int i=0;i<nItemCount;i++)
            {
                object item = cboDecisions.Items[i];

                if (item is DecisionProcessButton)
                {
                    DecisionProcessButton btn = (DecisionProcessButton)item;
                    DecisionProcessButton.YesNo decision = btn.Decision;
                    
                    if ((yesno == true && decision == DecisionProcessButton.YesNo.Yes) || (yesno == false && decision == DecisionProcessButton.YesNo.No))
                    {
                        cboDecisions.SelectedIndex = i;
                        btnNext_Click(null, null);
                        break;
                    }
                }
            }
        }
	}

    public class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
    {
        public DataGridViewDisableButtonColumn()
        {
            this.CellTemplate = new DataGridViewDisableButtonCell();
        }
    }

    public class DataGridViewDisableButtonCell : DataGridViewButtonCell
    {
        private bool enabledValue;
        public bool Enabled
        {
            get
            {
                return enabledValue;
            }
            set
            {
                enabledValue = value;
            }
        }

        // Override the Clone method so that the Enabled property is copied. 
        public override object Clone()
        {
            DataGridViewDisableButtonCell cell =
                (DataGridViewDisableButtonCell)base.Clone();
            cell.Enabled = this.Enabled;
            return cell;
        }

        // By default, enable the button cell. 
        public DataGridViewDisableButtonCell()
        {
            this.enabledValue = true;
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // The button cell is disabled, so paint the border,   
            // background, and disabled button for the cell. 
            if (!this.enabledValue)
            {
                // Draw the cell background, if specified. 
                if ((paintParts & DataGridViewPaintParts.Background) ==
                    DataGridViewPaintParts.Background)
                {
                    SolidBrush cellBackground =
                        new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // Draw the cell borders, if specified. 
                if ((paintParts & DataGridViewPaintParts.Border) ==
                    DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
                        advancedBorderStyle);
                }

                // Calculate the area in which to draw the button.
                Rectangle buttonArea = cellBounds;
                Rectangle buttonAdjustment =
                    this.BorderWidths(advancedBorderStyle);
                buttonArea.X += buttonAdjustment.X;
                buttonArea.Y += buttonAdjustment.Y;
                buttonArea.Height -= buttonAdjustment.Height;
                buttonArea.Width -= buttonAdjustment.Width;

                // Draw the disabled button.                
                ButtonRenderer.DrawButton(graphics, buttonArea,
                    PushButtonState.Disabled);

                // Draw the disabled button text.  
                if (this.FormattedValue is String)
                {
                    TextRenderer.DrawText(graphics,
                        (string)this.FormattedValue,
                        this.DataGridView.Font,
                        buttonArea, SystemColors.GrayText);
                }
            }
            else
            {
                // The button cell is enabled, so let the base class  
                // handle the painting. 
                base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                    elementState, value, formattedValue, errorText,
                    cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }

    class DecisionProcessButton : IComparable
    {
        public enum YesNo { No = 0, Yes, Unknown };

        private string m_strText = "";
        private ProcessButton m_btn = null;

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public ProcessButton ProcessButton
        {
            get { return m_btn; }
            set { m_btn = value; }
        }

        public YesNo Decision
        {
            get
            {
                string strText = this.Text.ToLower();

                if (strText.Contains("(예)") || strText.Contains("(네)") || strText.Contains("(yes)"))
                    return YesNo.Yes;
                else if (strText.Contains("(아니오)") || strText.Contains("(no)"))
                    return YesNo.No;

                return YesNo.Unknown;
            }
        }

        public DecisionProcessButton()
        {
        }

        public DecisionProcessButton(string strText, ProcessButton btn)
        {
            m_strText = strText;
            m_btn = btn;
        }

        public int CompareTo(object obj)
        {
            DecisionProcessButton btn = (DecisionProcessButton)obj;
            string thisText = this.Text.ToLower();
            string btnText = btn.Text.ToLower();

            if (this.Text.Length == 0)
            {
                if (btn.Text.Length == 0)
                    return 0;
                else
                    return 1;
            }
            else if (this.Text.Contains("(예)") || this.Text.Contains("(네)") || thisText.Contains("(yes)"))
            {
                if (btn.Text.Contains("(예)") || btn.Text.Contains("(네)") || btnText.Contains("(yes)"))
                    return 0;
                else
                    return -1;
            }
            else if (this.Text.Contains("(아니오)") || thisText.Contains("(no)"))
            {
                if (btn.Text.Contains("(아니오)") || btnText.Contains("(no)"))
                    return 0;
                else if (btn.Text.Length == 0)
                    return -1;
                else
                    return 1;
            }

            int thisNumber, btnNumber;

            // 둘다 숫자일 경우 숫자로 비교한다.
            if (GetNumber(this.Text, out thisNumber) && GetNumber(btn.Text, out btnNumber))
            {
                return thisNumber.CompareTo(btnNumber);
            }

            return this.Text.CompareTo(btn.Text);
        }

        private bool GetNumber(string strText, out int num)
        {
            int len = strText.Length;
            num = -1;

            for (int i=0;i<len;i++)
            {
                char ch = strText.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    if (num < 0)
                        num = ch - '0';
                    else
                        num = num * 10 + ch - '0';
                }
                else
                    break;
            }

            return num >= 0;
        }

        public override string ToString()
        {
            return string.Format("[{0}]로 분기", this.Text);
        }
    }

    public class DisabledComboBox : ComboBox
    {
        private bool m_disabled = false;
        // Disabled 상태일때 대신 나타나게 할 Button
        private Button m_btn = null;
        private bool m_canVisible = true;

        public bool CanVisible
        {
            get { return m_canVisible; }
            set
            {
                m_canVisible = value;

                if (!m_canVisible)
                    HideControl();
            }
        }

        public bool Disabled
        {
            get { return m_disabled; }
            set
            {
                if (m_disabled != value)
                {
                    m_disabled = value;

                    if (m_btn == null)
                    {
                        m_btn = new Button();

                        if (this.Parent != null)
                        {
                            this.Parent.Controls.Add(m_btn);
                            this.Parent.Controls.SetChildIndex(m_btn, 0);

                            if (!m_canVisible)
                                m_btn.Visible = false;
                        }

                        m_btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        m_btn.Anchor = this.Anchor;
                        m_btn.Font = this.Font;
                    }

                    if (m_canVisible)
                    {
                        if (m_disabled)
                        {
                            m_btn.Location = this.Location;
                            m_btn.Size = this.Size;
                            m_btn.Text = this.Text;
                        }

                        _ShowControl();
                    }
                    else
                        this.Hide();
                }
            }
        }

        public new Point Location
        {
            get { return base.Location; }
            set
            {
                base.Location = value;

                if (m_btn != null)
                    m_btn.Location = this.Location;
            }
        }

        public new Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;

                if (m_btn != null)
                    m_btn.Size = this.Size;
            }
        }

        public void ShowControl()
        {
            if (m_canVisible)
            {
                _ShowControl();
            }
            else
                this.Hide();
        }

        public void HideControl()
        {
            if (m_btn != null)
                m_btn.Hide();

            this.Hide();
        }

        private void _ShowControl()
        {
            if (!this.CanVisible)
                return;

            if (m_disabled)
            {
                if (this.Enabled)
                {
                    this.Hide();

                    if (m_btn != null)
                        m_btn.Show();
                }
                else
                {
                    if (m_btn != null)
                        m_btn.Hide();

                    this.Show();
                }
            }
            else
            {
                if (m_btn != null)
                    m_btn.Hide();

                this.Show();
            }
        }

        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                if (base.Enabled != value)
                {
                    base.Enabled = value;
                    _ShowControl();
                }
            }
        }
    }

    public class LogicalScriptParser
    {
        // == => =
        // || => or
        // && => and
        // != => <>
        // ! => not
        public static bool Execute(string strStatement, out string strError)
        {
            strError = "";

            strStatement = strStatement.Replace("&&", "and");
            strStatement = strStatement.Replace("||", "or");
            strStatement = strStatement.Replace("!=", "<>");
            strStatement = strStatement.Replace("==", "=");
            strStatement = strStatement.Replace("!", "not ");
            // 원래 '<'나 '>'은 '='보다 왼쪽에 위치해야 하지만
            // 개발자가 아닌 일반인들의 사용을 고려할때 엄격한 규칙을 요구하긴 힘들다.
            strStatement = strStatement.Replace("=<", "<= ");
            strStatement = strStatement.Replace("=>", ">= ");

            // 포함구문에 대한 처리
            ConditionalScriptParser.ContainsToLike(ref strStatement);

            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                object result = dt.Compute(strStatement, "");

                if (result != null && result is bool)
                    return (bool)result;
            }
            catch (Exception e)
            {
                strError = e.Message;
            }

            return false;
        }
    }
}
