using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;
using Sections;
using SectionContents.Utility;
using DBUtility2;
using System.Collections;
using SectionContents.PopupDialog;

namespace SectionContents.Fancy
{
    public partial class ComponentContents : UserControl, ISectionContents
    {
        private static Color NormalStateColor = Color.FromArgb(197, 197, 197);
        private static Color ProcessedStateColor = Color.FromArgb(108, 119, 141);
        private static Color ProcessingStateColor = Color.FromArgb(79, 104, 174);
        private static Color CurrentStateLeftColor = Color.FromArgb(219, 40, 68);
        private static Color CurrentStateRightColor = Color.FromArgb(188, 23, 49);

        private static Image NormalTLImage = null;
        private static Image NormalLeftImage = null;
        private static Image NormalTRImage = null;
        private static Image NormalRightImage = null;
        private static Image ProcessedTLImage = null;
        private static Image ProcessedLeftImage = null;
        private static Image ProcessedTRImage = null;
        private static Image ProcessedRightImage = null;
        private static Image ProcessingTLImage = null;
        private static Image ProcessingLeftImage = null;
        private static Image ProcessingTRImage = null;
        private static Image ProcessingRightImage = null;
        private static Image CurrentTLImage = null;
        private static Image CurrentLeftImage = null;
        private static Image CurrentTRImage = null;
        private static Image CurrentRightImage = null;

        private const int TeamNameWidth = 272;
        private const int TopBarHeight = 60;
        private const int TextBeginPos = 20;
        private const int MaxRowCount = 5;

        private static FormSpecialMessageBox m_frmSpecialMessage = null;

        private Section m_section = null;
        private bool m_isSelected = false;
        private bool m_isCollapsed = true;
        private State m_state = State.NORMAL;

        private Image m_currentLeftImage = null;
        private Image m_currentRightImage = null;

        private Color m_titleBackColor;
        private Color m_teamNameBackColor;
        private Color m_titleColor = Color.White;
        private Color m_teamNameColor = Color.White;

        private static Font m_titleFont = new Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private static Font m_teamNameFont = new Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private static StringFormat m_textFormat = GetStringFormat();

        private Brush m_titleBrush = null;
        private Brush m_teamNameBrush = null;

        private string m_strTitle = "";
        private string m_strTeamName = "";
        private Rectangle m_rectTitle, m_rectTeamName;

        private int m_nMissionCount = 0;
        // Key : MissionIndex
        private Dictionary<int, PanelMission> m_dicMissionIndex = new Dictionary<int, PanelMission>();
        private PanelInternal m_internalPanel = null;
        private int m_nExtendHeight = 60;

        private int m_nComponentHistoryID = -1;
        private ComponentContents m_nextContents = null;
        private bool m_isEnabled = false;

        private ISectionContentsOwner m_owner = null;
        private SectionCommander m_commander = null;

        private PanelMission m_selectedMission = null;
        private bool m_systemCall = false;

        private bool m_receiverInfoTime = false;
        private string m_strReceiverInfo = "";

        // m_owner가 null이라서 진행되지 못한 Item들
        private ArrayList m_arrWaitingContentsOwnerItems = new ArrayList();

        public Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public State State
        {
            get { return m_state; }
            set
            {
                if (m_state != value)
                {
                    m_state = value;

                    if (m_isSelected)
                        OnSelect(true);
                    else
                        SetStateColor();

                    if (m_state == UnE.SOP.Workstate.State.DONE)
                    {
                        //HideGrid();
                        // 이미 실행이 완료된 ComponentConents 이므로 다시 펼치기 전까진 비활성화 상태로 둔다.
                        EnableNextButton(false);
                        //PostDone();
                    }
                    else
                    {
                        if (m_state == UnE.SOP.Workstate.State.RUN)
                        {
                            // 자동실행 옵션이 있으면 실행한다.
                            AutoRun();
                        }
                    }
                }
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected != value)
                {
                    OnSelect(value, value);
                }
            }
        }

        public bool Collapsed
        {
            get { return m_isCollapsed; }
            set
            {
                if (m_isCollapsed != value)
                {
                    m_isCollapsed = value;
                    OnSelect(m_isSelected);
                    ResizeControl();

                    rbtnCollapse.IsChecked = m_isCollapsed;
                    rbtnCollapse.Refresh();
                }
            }
        }

        // 번호를 붙인 제목
        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        // 번호가 없는 제목
        public string OriginalTitle
        {
            get
            {
                if (m_section == null)
                    return "";

                return m_section.Title;
            }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public ISectionContents NextContents
        {
            get { return m_nextContents; }
            set { m_nextContents = (ComponentContents)value; }
        }

        public bool EnableControl
        {
            get { return m_isEnabled; }
            set
            {
                if (m_isEnabled != value)
                {
                    m_isEnabled = value;
                    SetEnable();
                }
            }
        }

        public ISectionContentsOwner ContentsOwner
        {
            get { return m_owner; }
            set
            {
                ISectionContentsOwner oldOwner = m_owner;
                m_owner = value;

                if (oldOwner == null && m_owner != null)
                {
                    PostSetContentsOwner();
                }
            }
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public SectionCommander Commander
        {
            get { return m_commander; }
            set { m_commander = value; }
        }

        private UEWpfControl.WpfComboBox m_cbDecisions = null;

        public ComponentContents(Section section, ISectionContentsOwner owner = null)
        {
            InitializeComponent();

            m_owner = owner;
            m_cbDecisions = new UEWpfControl.WpfComboBox();
            eleDecisions.Child = m_cbDecisions;
            m_cbDecisions.SetSize(eleDecisions.Width, eleDecisions.Height);
            
            SetResource();

            m_section = section;
            InitSectionData();
        }

        private void ComponentContentsProcess_Paint(object sender, PaintEventArgs e)
        {
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            if (m_titleBackColor == m_teamNameBackColor)
            {
                using (Brush brush = new SolidBrush(m_titleBackColor))
                {
                    // 모서리 부분은 둥글게 투명처리가 되기 때문에 10만큼 빼고 그린다.
                    e.Graphics.FillRectangle(brush, 10, 0, this.Size.Width - 20, TopBarHeight);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(m_titleBackColor))
                {
                    // 모서리 부분은 둥글게 투명처리가 되기 때문에 10만큼 빼고 그린다.
                    e.Graphics.FillRectangle(brush, 10, 0, this.Size.Width - 10 - TeamNameWidth, TopBarHeight);
                }

                using (Brush brush = new SolidBrush(m_teamNameBackColor))
                {
                    // 모서리 부분은 둥글게 투명처리가 되기 때문에 10만큼 빼고 그린다.
                    e.Graphics.FillRectangle(brush, this.Size.Width - TeamNameWidth, 0, TeamNameWidth - 10, TopBarHeight);
                }
            }

            e.Graphics.DrawImage(m_currentLeftImage, 0, 0);
            e.Graphics.DrawImage(m_currentRightImage, this.Size.Width - m_currentRightImage.Width, 0);

            if (m_strTitle.Length > 0)
                e.Graphics.DrawString(m_strTitle, m_titleFont, m_titleBrush, m_rectTitle, m_textFormat);

            if (m_receiverInfoTime)
            {
                e.Graphics.DrawString(m_strReceiverInfo, m_teamNameFont, m_teamNameBrush, m_rectTeamName, m_textFormat);
            }
            else
            {
                if (m_strTeamName.Length > 0)
                    e.Graphics.DrawString(m_strTeamName, m_teamNameFont, m_teamNameBrush, m_rectTeamName, m_textFormat);
            }

            /*if (m_strTeamName.Length > 0)
                e.Graphics.DrawString(m_strTeamName, m_teamNameFont, m_teamNameBrush, m_rectTeamName, m_textFormat);*/
        }

        private void SetResource()
        {
            if (NormalTLImage == null)
            {
                NormalTLImage = global::SectionContents.Properties.Resources.NormalMission_TL_Round;
                NormalLeftImage = global::SectionContents.Properties.Resources.NormalMission_Left_Round;
                NormalTRImage = global::SectionContents.Properties.Resources.NormalMission_TR_Round;
                NormalRightImage = global::SectionContents.Properties.Resources.NormalMission_Right_Round;

                ProcessedTLImage = global::SectionContents.Properties.Resources.CompleteMission_TL_Round;
                ProcessedLeftImage = global::SectionContents.Properties.Resources.CompleteMission_Left_Round;
                ProcessedTRImage = global::SectionContents.Properties.Resources.CompleteMission_TR_Round;
                ProcessedRightImage = global::SectionContents.Properties.Resources.CompleteMission_Right_Round;

                ProcessingTLImage = global::SectionContents.Properties.Resources.ProcessingMission_TL_Round;
                ProcessingLeftImage = global::SectionContents.Properties.Resources.ProcessingMission_Left_Round;
                ProcessingTRImage = global::SectionContents.Properties.Resources.ProcessingMission_TR_Round;
                ProcessingRightImage = global::SectionContents.Properties.Resources.ProcessingMission_Right_Round;

                CurrentTLImage = global::SectionContents.Properties.Resources.CurrentMission_TL_Round;
                CurrentLeftImage = global::SectionContents.Properties.Resources.CurrentMission_Left_Round;
                CurrentTRImage = global::SectionContents.Properties.Resources.CurrentMission_TR_Round;
                CurrentRightImage = global::SectionContents.Properties.Resources.CurrentMission_Right_Round;
            }

            m_currentLeftImage = NormalLeftImage;
            m_currentRightImage = NormalRightImage;
            m_teamNameBackColor = this.BackColor;

            m_titleBackColor = NormalStateColor;
            m_teamNameBackColor = NormalStateColor;

            m_titleBrush = new SolidBrush(m_titleColor);
            m_teamNameBrush = new SolidBrush(m_teamNameColor);

            SetTextRectangle();
        }

        private void SetTextRectangle()
        {
            m_rectTitle = new Rectangle(TextBeginPos, 0, this.Size.Width - TeamNameWidth - TextBeginPos, TopBarHeight);

            int x = this.Size.Width - TeamNameWidth + TextBeginPos;
            m_rectTeamName = new Rectangle(x, 0, rbtnNext.Location.X - 10 - x, TopBarHeight);
        }

        private void SetStateColor()
        {
            if (m_state == State.NORMAL || m_state == State.INPUT || m_state == State.SKIP)
            {
                if (m_isCollapsed)
                    SetColor(NormalStateColor, NormalStateColor, NormalLeftImage, NormalRightImage);
                else
                    SetColor(NormalStateColor, NormalStateColor, NormalTLImage, NormalTRImage);
            }
            else if (m_state == State.DONE)
            {
                if (m_isCollapsed)
                    SetColor(ProcessedStateColor, ProcessedStateColor, ProcessedLeftImage, ProcessedRightImage);
                else
                    SetColor(ProcessedStateColor, ProcessedStateColor, ProcessedTLImage, ProcessedTRImage);
            }
            else if (m_state == State.RUN)
            {
                if (m_isCollapsed)
                    SetColor(ProcessingStateColor, ProcessingStateColor, ProcessingLeftImage, ProcessingRightImage);
                else
                    SetColor(ProcessingStateColor, ProcessingStateColor, ProcessingTLImage, ProcessingTRImage);
            }
            else
                return;

            if (AllowRefresh())
                Refresh();
            else if (m_owner != null)
                m_owner.NeedRefreshContents(this);
        }

        private bool AllowRefresh()
        {
            if (m_owner == null)
                return true;

            return m_owner.AllowSectionRefresh;
        }

        private void OnSelect(bool isSelected, bool hasFocus = false)
        {
            m_isSelected = isSelected;

            if (m_isSelected)
            {
                if (m_isCollapsed)
                    SetColor(CurrentStateLeftColor, CurrentStateRightColor, CurrentLeftImage, CurrentRightImage);
                else
                    SetColor(CurrentStateLeftColor, CurrentStateRightColor, CurrentTLImage, CurrentTRImage);

                if (m_section != null)
                {
                    m_section.SetColor(Section.ColorTarget.FILL, WorkFlowManager.Instance.CurrentColor);

                    if (m_section.Shape.ShapeStyler != null)
                        m_section.Shape.ShapeStyler.SetCurrent(true);
                }

                if (hasFocus)
                    FocusContents();

                if (AllowRefresh())
                    Refresh();
                else if (m_owner != null)
                    m_owner.NeedRefreshContents(this);
            }
            else
            {
                SetStateColor();

                if (m_section != null)
                {
                    if (m_section.Shape.ShapeStyler != null)
                        m_section.Shape.ShapeStyler.SetCurrent(false);
                }
            }
        }

        public void FocusContents()
        {
            if (this.Parent != null && this.Parent is ScrollableControl)
            {
                ScrollableControl panel = (ScrollableControl)this.Parent;

                if (this.Parent != null)
                {
                    int nPos = 0;

                    foreach (Control ctrl in this.Parent.Controls)
                    {
                        if (ctrl == this)
                            break;

                        nPos += ctrl.Size.Height;
                    }

                    if (nPos >= panel.VerticalScroll.Minimum && nPos <= panel.VerticalScroll.Maximum)
                        panel.VerticalScroll.Value = nPos;
                }

                //this.Select();

                /*int nPanelSize = panel.Size.Height;
                int nContentsSize = this.Size.Height;
                int nContentsPosition = this.Location.Y;

                // 바닥으로부터의 최소 유격거리
                int nSpace = 200;
                int nDiff = nPanelSize - (nContentsPosition + nContentsSize);

                if (nDiff < nSpace)
                {
                    using (Control c = new Control() { Parent = panel, Height = 1, Top = nPanelSize + (nSpace - nDiff) })
                    {
                        panel.ScrollControlIntoView(c);
                    }
                }*/

                if (AllowRefresh())
                    panel.Refresh();
                else if (m_owner != null)
                    m_owner.NeedRefreshContents(this);
            }
        }

        private void SetColor(Color colorTitle, Color colorTeamName, Image imgLeft, Image imgRight)
        {
            m_titleBackColor = colorTitle;
            m_teamNameBackColor = colorTeamName;
            m_currentLeftImage = imgLeft;
            m_currentRightImage = imgRight;
        }

        private void ComponentContentsProcess_Resize(object sender, EventArgs e)
        {
            SetTextRectangle();

            if (this.Size.Height > TopBarHeight)
            {
                panelBody.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - TopBarHeight);
                panelBody.Show();
            }
            else
            {
                if (panelBody.Size.Width != this.ClientSize.Width)
                    panelBody.Size = new Size(this.ClientSize.Width, panelBody.Size.Height);

                panelBody.Hide();
            }
        }

        public static StringFormat GetStringFormat()
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Near;
            return format;
        }

        private void InitSectionData()
        {
            m_dicMissionIndex.Clear();

            if (m_section == null)
            {
                rbtnNext.Visible = false;
                rbtnCollapse.Visible = false;
            }
            else if (m_section is SectionEndPoint)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)m_section.Data;
                SetTitle(m_section.Title, data);

                if (data.IsBegin)
                {
                    rbtnNext.NormalImage = global::SectionContents.Properties.Resources.Begin_Normal;
                    rbtnNext.MouseOverImage = global::SectionContents.Properties.Resources.Begin_MouseOver;
                    rbtnNext.ClickedImage = global::SectionContents.Properties.Resources.Begin_Clicked;
                    rbtnNext.DisabledImage = global::SectionContents.Properties.Resources.Begin_Disabled;

                    EnableNextButton(true);
                    EnableControl = true;
                }
                else
                {
                    rbtnNext.NormalImage = global::SectionContents.Properties.Resources.End_Normal;
                    rbtnNext.MouseOverImage = global::SectionContents.Properties.Resources.End_MouseOver;
                    rbtnNext.ClickedImage = global::SectionContents.Properties.Resources.End_Clicked;
                    rbtnNext.DisabledImage = global::SectionContents.Properties.Resources.End_Disabled;
                }

                rbtnCollapse.Visible = false;
            }
            else if (m_section is SectionDecision)
            {
                rbtnCollapse.Visible = false;
                SetTitle(m_section.Title, m_section.Data);

                int nLimit = 20;

                for (int i = 0; i < nLimit; i++)
                {
                    ISectionPainter painter = m_section.GetSectionPainter(i);

                    if (painter == null)
                        break;

                    if (painter is ProcessButtonManager)
                    {
                        ProcessButtonManager mgr = (ProcessButtonManager)painter;
                        List<DecisionProcessButton> buttons = new List<DecisionProcessButton>();

                        SectionContentsHelper.AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.LEFT);
                        SectionContentsHelper.AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.RIGHT);
                        SectionContentsHelper.AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.BOTTOM);
                        SectionContentsHelper.AddDecisionProcessButton(buttons, mgr, Arrow.ArrowPosition.TOP);

                        buttons.Sort();
                        
                        eleDecisions.Visible = true;

                        m_cbDecisions.customComboBox.DisplayMemberPath = "TextToString";

                        foreach (DecisionProcessButton btn in buttons)
                        {
                            m_cbDecisions.customComboBox.Items.Add(btn);
                        }

                        if (m_cbDecisions.customComboBox.Items.Count > 0)
                            m_cbDecisions.customComboBox.SelectedIndex = 0;

                        break;
                    }
                }
            }
            else if (m_section is SectionProcess)
            {
                SectionDataProcess data = (SectionDataProcess)m_section.Data;
                SectionProcess section = (SectionProcess)m_section;
                m_strTeamName = section.TextDown;
                SetTitle(section.TextUP, data);

                m_nMissionCount = data.MissionItems.Count;
                int nPanelHeight = 0, nRowHeight = 0;
                List<PanelMission> panels = new List<PanelMission>();

                for (int i = m_nMissionCount - 1; i >= 0; i--)
                {
                    PanelMission panel;
                    MissionItem mission = (MissionItem)data.MissionItems[i];
                    nRowHeight = AddMissionData(i, mission, mission.Mission, i < m_nMissionCount - 1, out panel);
                    nPanelHeight += nRowHeight;

                    panels.Add(panel);
                }

                if (m_owner != null)
                    SetReceivers(panels);
                else
                    m_arrWaitingContentsOwnerItems.Add(panels);

                if (nRowHeight > 0)
                {
                    if (m_nMissionCount <= MaxRowCount)
                        m_nExtendHeight = TopBarHeight + nPanelHeight;
                    else
                        m_nExtendHeight = MaxRowCount * nRowHeight;
                }
            }
            else if (m_section is SectionInternal)
            {
                SectionDataInternal data = (SectionDataInternal)m_section.Data;
                SectionInternal section = (SectionInternal)m_section;

                PanelInternal panel = AddInternalPanel(data);
                SetTitle(section.Title, data);
                m_strTeamName = GetReceiverName(data.TeamList, panel);

                if (m_owner != null)
                    panel.SetReceivers();
                else
                    m_arrWaitingContentsOwnerItems.Add(panel);

                m_internalPanel = panel;
            }

            this.Size = new Size(this.Size.Width, TopBarHeight);
        }

        private void PostSetContentsOwner()
        {
            foreach (object item in m_arrWaitingContentsOwnerItems)
            {
                if (item is PanelInternal)
                {
                    PanelInternal panel = (PanelInternal)item;
                    panel.SetReceivers();
                }
                else if (item is List<PanelMission>)
                {
                    List<PanelMission> panels = (List<PanelMission>)item;
                    SetReceivers(panels);
                }
            }

            m_arrWaitingContentsOwnerItems.Clear();
        }

        private void SetReceivers(List<PanelMission> panels)
        {
            if (panels.Count == 0)
                return;

            string strSender;
            Dictionary<string, string> dicPhoneNumbers;
            ArrayList arrPhoneNumbers = SectionContentsHelper.GetSMSInfo(this, out strSender, out dicPhoneNumbers);

            foreach (PanelMission panel in panels)
            {
                panel.SetReceivers(arrPhoneNumbers, strSender, dicPhoneNumbers);
            }
        }

        private void SetTitle(string strTitle, SectionData data)
        {
            if (data.SectionNumber > 0)
                m_strTitle = string.Format("{0}. {1}", data.SectionNumber, strTitle);
            else
                m_strTitle = strTitle;
        }

        private PanelInternal AddInternalPanel(SectionDataInternal data)
        {
            PanelInternal panel = new PanelInternal(this, !data.UseBroadcast, data.UseSiren);
            m_nExtendHeight = TopBarHeight + panel.Size.Height;
            panel.Location = new Point(0, 0);
            panel.Dock = DockStyle.Fill;
            panel.Message = data.BroadcastMessage;
            panelBody.Controls.Add(panel);

            panelBody.Size = new Size(panelBody.Size.Width, m_nExtendHeight - TopBarHeight);
            return panel;
        }

        private string GetReceiverName(System.Collections.ArrayList arrTeams, PanelInternal panel)
        {
            string strReceiverName = "";

            foreach (SOPTeam team in arrTeams)
            {
                string strTeamName = team.IncludeChildTeams ? team.TeamName + "(+)" : team.TeamName;
                panel.AddTeamName(strTeamName);

                if (strReceiverName.Length == 0)
                    strReceiverName = strTeamName;
                else
                    strReceiverName += ", " + strTeamName;
            }

            return strReceiverName;
        }

        private int AddMissionData(int nRowIndex, MissionItem mission, string strMission, bool lineVisible, out PanelMission panel)
        {
            panel = new PanelMission(this, mission);
            panel.Location = new Point(0, 0);
            panel.MissionText = strMission;
            panel.Dock = DockStyle.Top;
            panelBody.Controls.Add(panel);

            panel.MissionIndex = nRowIndex;
            m_dicMissionIndex[nRowIndex] = panel;

            int nRowHeight = panel.Size.Height;
            panel.LineVisible = lineVisible;

            return nRowHeight;
        }

        private void rbtnCollapse_Click(object sender, EventArgs e)
        {
            m_isCollapsed = !m_isCollapsed;
            rbtnCollapse.IsChecked = !m_isCollapsed;
            ResizeControl();
        }

        private void ResizeControl()
        {
            int nPrevHeight = this.Size.Height;

            if (m_isCollapsed)
                this.Size = new Size(this.Size.Width, TopBarHeight);
            else
                this.Size = new Size(this.Size.Width, m_nExtendHeight);

            if (m_nextContents != null)
                m_nextContents.MoveVertical(this.Size.Height - nPrevHeight);
        }

        public void OnCheckedComplete(int nMissionIndex, bool isChecked)
        {
            SectionState state = null;

            if (nMissionIndex < 0)
            {
                PanelSection panel = this.Section.GetParent();
                SectionTabPage page = (SectionTabPage)panel.Parent;
                state = WorkFlowManager.Instance.Find(this.Section, !page.VirtualMode);

                state.CheckedComplete = isChecked ? 1 : 0;
            }

            SectionContentsHelper.SendLogState(this, state);

            if (isChecked)
                PostCompleteChecked();
        }

        public void RunMissionExternal(MissionItemExternal mission)
        {
            if (m_owner != null)
            {
                PanelSection panel = this.Section.GetParent();
                SectionTabPage page = (SectionTabPage)panel.Parent;
                mission.ActionStepHistoryID = page.ActionStepHistoryID;
                mission.TabPage = page;
                m_owner.OnSectionContentsEvent(SectionContentsEvent.RunMissionExternal, mission);
            }
        }

        // 모든 완료 버튼이 Checked 상태일 경우 [다음] 버튼을 누른것과 같은 효과를 내도록 한다.
        private void PostCompleteChecked()
        {
            foreach (KeyValuePair<int, PanelMission> pair in m_dicMissionIndex)
            {
                if (pair.Value.IsComplete == false)
                    return;
            }

            if (rbtnNext.Enabled)
                rbtnNext_Click(null, null);
        }

        public void MoveVertical(int move)
        {
            this.Location = new Point(this.Location.X, this.Location.Y + move);

            if (m_nextContents != null)
                m_nextContents.MoveVertical(move);
        }

        private void rbtnNext_Click(object sender, EventArgs e)
        {
            if (m_section == null)
                return;

            EnableNextButton(false);
            m_cbDecisions.customComboBox.IsEnabled = false;

            int nLimit = 20;

            if (m_section is SectionEndPoint)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)m_section.Data;

                if (data.IsBegin)
                {
                    if (m_owner != null)
                        m_owner.OnSectionContentsEvent(SectionContentsEvent.RunSOP, null);
                }
                else
                {
                    for (int i = 0; i < nLimit; i++)
                    {
                        ISectionPainter painter = m_section.GetSectionPainter(i);

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
            }
            else if (m_section is SectionDecision)
            {
                if (m_cbDecisions.customComboBox.SelectedIndex < 0)
                    return;

                DecisionProcessButton btn = (DecisionProcessButton)m_cbDecisions.customComboBox.Items[m_cbDecisions.customComboBox.SelectedIndex];
                btn.ProcessButton.OnClick();
            }
            else if (m_section is SectionProcess || m_section is SectionInternal)
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

        private void SetEnable()
        {
            EnableNextButton(m_isEnabled);

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is PanelMission)
                {
                    PanelMission panel = (PanelMission)ctrl;
                    panel.EnableControl = m_isEnabled;
                }
                else if (ctrl is PanelInternal)
                {
                    PanelInternal panel = (PanelInternal)ctrl;
                    panel.EnableControl = m_isEnabled;
                }
            }
        }

        private void EnableNextButton(bool enabled)
        {
            rbtnNext.Enabled = enabled;
            m_cbDecisions.customComboBox.IsEnabled = enabled;
        }

        // 내부상황전파
        public bool GetItem(out bool isBroadcast, out bool isExecute, out bool isComplete, out int nBroadcastCount, out bool useSiren, out VariousData<DateTime> executeTime, out VariousData<DateTime> completeTime, out VariousData<DateTime> unCompleteTime, out string strMessage)
        {
            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is PanelInternal)
                {
                    PanelInternal panel = (PanelInternal)ctrl;
                    panel.GetItem(out isBroadcast, out isExecute, out isComplete, out nBroadcastCount, out useSiren, out executeTime, out completeTime, out unCompleteTime);
                    strMessage = panel.ExecuteMessage;
                    return true;
                }
            }

            executeTime = unCompleteTime = completeTime = null;
            isBroadcast = isExecute = isComplete = false;
            nBroadcastCount = 0;
            useSiren = false;
            strMessage = "";
            return false;
        }

        // 프로세스
        public bool GetItem(int nRowIndex, out bool isSendSMS, out bool isComplete, out string strSender, out string strItem, out string strTeamName, out string strPerformer, out VariousData<DateTime> executeTime, out VariousData<DateTime> completeTime, out VariousData<DateTime> unCompleteTime)
        {
            PanelMission panel = null;

            if (m_dicMissionIndex.TryGetValue(nRowIndex, out panel) == false)
            {
                isSendSMS = isComplete = false;
                strSender = strItem = strTeamName = strPerformer = "";
                completeTime = executeTime = unCompleteTime = null;
                return false;
            }

            isSendSMS = false;
            strSender = "";
            strTeamName = m_strTeamName;
            strPerformer = "";

            panel.GetItem(out isComplete, out strItem, out executeTime, out completeTime, out unCompleteTime);
            return true;
        }

        public bool SelectRow(List<int> rowIndexList)
        {
            foreach (int nRowIndex in rowIndexList)
            {
                if (nRowIndex < 0 || nRowIndex >= m_nMissionCount)
                    return false;
            }

            foreach (KeyValuePair<int, PanelMission> pair in m_dicMissionIndex)
            {
                if (rowIndexList.Contains(pair.Key))
                    pair.Value.IsSelected = true;
                else
                    pair.Value.IsSelected = false;
            }

            return true;
        }

        public void SetDetailData(int nRowIndex, int nData, DBUtility2.VariousData<DateTime> time)
        {
            if (nRowIndex < 0 || m_nMissionCount <= nRowIndex)
                return;

            PanelMission panel = null;

            if (m_dicMissionIndex.TryGetValue(nRowIndex, out panel))
            {
                if (nData == (int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_CHECKED)
                {
                    panel.SetCompleteCheck(true);
                    
                    if (time != null)
                    {
                        panel.TimeString = GetTimeString(time.Data);
                        panel.CompleteTime = time;
                    }
                    else
                    {
                        panel.TimeString = "";
                        panel.CompleteTime = null;
                    }
                }
                else if (nData == (int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_UNCHECKED)
                {
                    panel.SetCompleteCheck(false);

                    if (time != null)
                        panel.CompleteTime = time;
                    else
                        panel.CompleteTime = null;
                }
                else if (nData == (int)UnE.SOP.History.HistorySectionData.DetailData.DataType.SEND_SMS)
                {
                    if (time != null)
                        panel.CompleteTime = time;
                }
            }
        }

        public void SetDetailDatas(int nComponentHistoryID, List<UnE.SOP.History.HistorySectionData.DetailData> detailDatas)
        {
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
                {
                    if (detail.Datai == null)
                        continue;

                    SetDetailData(detail.DataIndex.Data, detail.Datai.Data, detail.Time);
                }
                else
                {
                    if (m_internalPanel != null)
                        m_internalPanel.SetDetailData(detail);
                }
            }
        }

        public static string GetTimeString(DateTime time)
        {
            return string.Format("{0:00}:{1:00}", time.Hour, time.Minute);
        }

        // SOP 실행상태로 만든다.
        public void Start(UnE.SOP.Workstate.WorkflowOption option, bool isRealMode)
        {
            if (this.Section == null)
                return;

            PreStart(option, isRealMode);
        }

        // SOP 실행이 종료되었다.
        public void Finish()
        {
            if (m_section == null)
            {
                rbtnNext.Visible = false;
                rbtnCollapse.Visible = false;
            }
            else if (m_section is SectionEndPoint)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)m_section.Data;
                
                if (data.IsBegin)
                {
                    rbtnNext.NormalImage = global::SectionContents.Properties.Resources.Begin_Normal;
                    rbtnNext.MouseOverImage = global::SectionContents.Properties.Resources.Begin_MouseOver;
                    rbtnNext.ClickedImage = global::SectionContents.Properties.Resources.Begin_Clicked;
                    rbtnNext.DisabledImage = global::SectionContents.Properties.Resources.Begin_Disabled;

                    EnableNextButton(true);
                    EnableControl = true;
                    return;
                }
            }
            
            EnableControl = false;
        }

        private void PreStart(WorkflowOption option, bool isRealMode)
        {
            if (this.Section == null)
                return;

            DBUtility2.VariousData<DateTime> dtDetect = null;
            string strLocation = "", strBroadcastLocationName = "", strPSMMaterialName = "", strAmountSnowfall = "";
            DBUtility2.VariousData<int> psmDistance = null;
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

                    psmDistance = new DBUtility2.VariousData<int>(optionPSM.PSMDistance);
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

            SectionContentsHelper.ChangeTitle(this);
            m_strTitle = SectionContentsHelper.Parse(m_strTitle, dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall, strAlarmMessage);

            SectionContentsHelper.ChangeCommanderName(this);
            ChangeMIssions(option, isRealMode);

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is PanelInternal)
                {
                    PanelInternal panel = (PanelInternal)ctrl;

                    string strMessage = SectionContentsHelper.Parse(panel.Message, dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall, strAlarmMessage);
                    panel.Message = SectionContentsHelper.ChangeText(strMessage, option, panel.IsSMS);

                    if (option != null)
                        panel.Message = ParseUserDefinedParameters(option, panel.Message);

                    panel.ClearState();

                    /*panel.Option = option;
                    panel.SetStartTime(dtDetect);

                    if (strLocation != null)
                        panel.SetLocation(strLocation, strBroadcastLocationName);

                    if (strPSMMaterialName != null)
                        panel.PSMMaterial = strPSMMaterialName;

                    if (psmDistance != null)
                        panel.PSMDistance = psmDistance.Data;

                    if (strAmountSnowfall != null)
                        panel.AmountSnowfall = strAmountSnowfall;

                    panel.RunMode = true;*/
                }
            }
        }

        private void ChangeMIssions(WorkflowOption option, bool isRealMode)
        {
            foreach (KeyValuePair<int, PanelMission> pair in m_dicMissionIndex)
            {
                pair.Value.ChangeMission(option, isRealMode);
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
                data.Expression = SectionContentsHelper.ChangeEarthquakeString(data.ExpressionOrigin, (WorkflowOptionEarthquake)option);
            }
            else if (option is WorkflowOptionPSM)
            {
                data.Expression = SectionContentsHelper.ChangePSMString(data.ExpressionOrigin, (WorkflowOptionPSM)option);
            }
            else if (option is WorkflowOptionSnowFall)
            {
                data.Expression = SectionContentsHelper.ChangeClimateString(data.ExpressionOrigin, (WorkflowOptionSnowFall)option);
            }
            else if (option is WorkflowOptionWind)
            {
                data.Expression = SectionContentsHelper.ChangeClimateString(data.ExpressionOrigin, (WorkflowOptionWind)option);
            }
            else if (option != null)
            {
                data.Expression = SectionContentsHelper.ChangeCommonString(data.ExpressionOrigin, option);
            }

            /*if (data.Expression == null || data.Expression.Length == 0)
                return;

            if (option is WorkflowOptionEarthquake)
            {
                data.Expression = ChangeEarthquakeString(data.Expression, (WorkflowOptionEarthquake)option);
            }*/

            data.Expression = ParseUserDefinedParameters(option, data.Expression);
        }

        public void UpdateContents(int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete)
        {
            if (m_section == null)
                return;

            PanelMission panel = null;

            for (int i = 0; i < m_nMissionCount; i++)
            {
                if (m_dicMissionIndex.TryGetValue(i, out panel) == false)
                    continue;

                int nBitFlag = 1 << i;
                //bool smsEnabled = true;

                //if ((nCheckedRun & nBitFlag) == nBitFlag)
                //    smsEnabled = false;

                if ((nCheckedComplete & nBitFlag) == nBitFlag)
                {
                    //smsEnabled = false;
                    panel.IsComplete = true;
                }
                else
                    panel.IsComplete = false;
            }
        }

        public void OnSelectMission(PanelMission panel, bool isSelected)
        {
            if (m_systemCall)
                return;

            if (isSelected && m_selectedMission == panel)
                return;

            m_systemCall = true;

            if (isSelected)
            {
                if (m_selectedMission != null)
                    m_selectedMission.IsSelected = false;

                m_selectedMission = panel;
            }
            else
            {
                if (m_selectedMission == panel)
                    m_selectedMission = null;
                else
                {
                    m_selectedMission.IsSelected = false;
                    m_selectedMission = null;
                }
            }

            m_systemCall = false;
        }

        public static void ShowSpecialMessageHelp()
        {
            if (m_frmSpecialMessage == null || m_frmSpecialMessage.IsDisposed)
            {
                m_frmSpecialMessage = new FormSpecialMessageBox();
                m_frmSpecialMessage.StartPosition = FormStartPosition.CenterParent;
            }

            if (m_frmSpecialMessage.Visible)
            {
                m_frmSpecialMessage.Focus();
                return;
            }

            m_frmSpecialMessage.Show();
            
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
            PanelMission mission;

            for (int i = 0; i < m_nMissionCount; i++)
            {
                if (m_dicMissionIndex.TryGetValue(i, out mission))
                {
                    mission.AutoRun();
                    mission.SetCompleteCheck(true);
                }
            }

            rbtnNext_Click(null, null);
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

            if (System.IO.File.Exists(strWorkingDirectory + strExe))
            {
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

        private void AutoRunInternal()
        {
            bool isComplete = m_internalPanel.IsComplete;
            m_internalPanel.Execute();
        }

        private void ComponentContents_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Process에 대해서만 적용한다.
                if (m_nMissionCount > 0)
                {
                    PanelMission panel;

                    if (m_dicMissionIndex.TryGetValue(0, out panel))
                    {
                        if (m_rectTeamName.Contains(e.X, e.Y))
                        {
                            m_strReceiverInfo = panel.ReceiverText;
                            m_receiverInfoTime = true;
                            Refresh();
                        }
                    }
                }
            }
        }

        private void ComponentContents_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                m_receiverInfoTime = false;
                Refresh();
            }
        }

        private void AutoRunDecision(bool yesno)
        {
            if (eleDecisions.Enabled == false || eleDecisions.Visible == false)
                return;

            int nItemCount = m_cbDecisions.customComboBox.Items.Count;

            for (int i = 0; i < nItemCount; i++)
            {
                object item = m_cbDecisions.customComboBox.Items[i];

                if (item is DecisionProcessButton)
                {
                    DecisionProcessButton btn = (DecisionProcessButton)item;
                    DecisionProcessButton.YesNo decision = btn.Decision;

                    if ((yesno == true && decision == DecisionProcessButton.YesNo.Yes) || (yesno == false && decision == DecisionProcessButton.YesNo.No))
                    {
                        m_cbDecisions.customComboBox.SelectedIndex = i;
                        rbtnNext_Click(null, null);
                        break;
                    }
                }
            }
        }

        public static string ParseUserDefinedParameters(WorkflowOption option, string str)
        {
            string strLower = str.ToLower();

            foreach (KeyValuePair<UnE.SOP.SOPParameter, string> pair in option.UserDefinedParameters)
            {
                string strParamName = "{" + pair.Key.VariableName.ToLower() + "}";

                int nIndex = strLower.IndexOf(strParamName);

                while (nIndex >= 0)
                {
                    string strSrc1 = strLower.Substring(nIndex, strParamName.Length);
                    string strSrc2 = str.Substring(nIndex, strParamName.Length);

                    strLower = strLower.Replace(strSrc1, pair.Value);
                    str = str.Replace(strSrc2, pair.Value);
                    nIndex = strLower.IndexOf(strParamName);
                }
            }

            return str;
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

            strStatement = strStatement.Replace("\r", "");
            strStatement = strStatement.Replace("\n", "");

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
            SOPMonitoringSystem.ConditionalScriptParser.ContainsToLike(ref strStatement);

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
