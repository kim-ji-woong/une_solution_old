using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP.Workstate;
using Sections;
using SectionContents.Utility;
using DBUtility2;
using System.Collections;

namespace SectionContents.Fancy
{
    public partial class PanelMission : UserControl
    {
        private static Pen m_pen = new Pen(Color.FromArgb(224, 224, 224), 1.0f);
        private static StringFormat m_textFormat = ComponentContents.GetStringFormat();

        private const int TextBeginPos = 25;

        private bool m_lineVisible = true;
        private ComponentContents m_owner = null;
        private int m_nMissionIndex = -1;
        private string m_strMission = "", m_strTime = "";
        private VariousData<DateTime> m_completeTime = null;
        private VariousData<DateTime> m_unCompleteTime = null;
        private VariousData<DateTime> m_executeTime = null;
        private MissionItem m_mission = null;

        private static Color NotSelectedColor = Color.White;
        //private static Color SelectedColor = Color.FromArgb(61, 138, 247);
        private static Color SelectedColor = Color.FromArgb(138, 192, 250);

        private static Font m_missionFont = new Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private static Font m_timeFont = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private static SolidBrush m_enabledTimeBrush = new SolidBrush(Color.FromArgb(0, 140, 255));
        private static SolidBrush m_disabledTimeBrush = new SolidBrush(Color.FromArgb(196, 196, 196));

        private SolidBrush m_missionBrush = new SolidBrush(Color.Black);
        private SolidBrush m_timeBrush = m_enabledTimeBrush;
        private Rectangle m_rectMission;
        private Rectangle m_rectTime;

        private bool m_isEnabled = false;
        private bool m_isSelected = false;

        private Dictionary<string, string> m_dicPhoneNumbers = null;
        private ArrayList m_arrPhoneNumbers = null;
        private string m_strSender = "";
        private string m_strSMSSendResult = "";
        private Color m_clrSMSResultFail = Color.Red;
        private Color m_clrSMSResultSuccess = Color.Green;
        private Color m_clrSMSResult = Color.Black;

        public bool LineVisible
        {
            get { return m_lineVisible; }
            set { m_lineVisible = value; }
        }

        public string MissionText
        {
            get { return m_strMission; }
            set { m_strMission = value; }
        }

        public bool IsComplete
        {
            get { return rbtnComplete.IsChecked; }
            set
            {
                if (rbtnComplete.IsChecked != value)
                {
                    rbtnComplete.IsChecked = value;
                    rbtnComplete.Refresh();
                }
            }
        }

        public string TimeString
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }

        public VariousData<DateTime> CompleteTime
        {
            get { return m_completeTime; }
            set { m_completeTime = value; }
        }

        public VariousData<DateTime> ExecuteTime
        {
            get { return m_executeTime; }
            set { m_executeTime = value; }
        }

        public int MissionIndex
        {
            get { return m_nMissionIndex; }
            set { m_nMissionIndex = value; }
        }

        public Color MissionColor
        {
            get { return m_missionBrush.Color; }
            set { m_missionBrush.Color = value; }
        }

        public Color TimeColor
        {
            get { return m_timeBrush.Color; }
            set { m_timeBrush.Color = value; }
        }

        public bool EnableControl
        {
            get { return m_isEnabled; }
            set { SetEnable(value); }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set { SelectControl(value); }
        }

        public string ReceiverText
        {
            get
            {
                if (m_arrPhoneNumbers == null)
                    return "수신자(총 0명)";

                return string.Format("수신자(총 {0}명)", m_arrPhoneNumbers.Count);
            }
        }

        public PanelMission(ComponentContents owner, MissionItem mission)
        {
            InitializeComponent();
            m_owner = owner;
            m_mission = mission;

            PanelMission_SizeChanged(null, null);
        }

        private void rbtnComplete_Click(object sender, EventArgs e)
        {
            SetCompleteCheck(!rbtnComplete.IsChecked);
            //rbtnComplete.IsChecked = !rbtnComplete.IsChecked;

            if (rbtnComplete.IsChecked)
            {
                m_completeTime = new VariousData<DateTime>(DateTime.Now);
                m_strTime = ComponentContents.GetTimeString(m_completeTime.Data);
            }
            else
            {
                m_strTime = "";
                m_unCompleteTime = new VariousData<DateTime>(DateTime.Now);
            }

            rbtnComplete.Refresh();

            if (m_owner != null)
                m_owner.OnCheckedComplete(m_nMissionIndex, rbtnComplete.IsChecked);
        }

        private void PanelMission_Paint(object sender, PaintEventArgs e)
        {
            if (m_strMission.Length > 0)
                e.Graphics.DrawString(m_strMission, m_missionFont, m_missionBrush, m_rectMission, m_textFormat);

            if (m_strTime.Length > 0)
                e.Graphics.DrawString(m_strTime, m_timeFont, m_timeBrush, m_rectTime, m_textFormat);

            if (m_lineVisible)
            {
                e.Graphics.DrawLine(m_pen, 0, this.Size.Height - 2, this.Size.Width - 10, this.Size.Height - 2);
            }
        }

        private void PanelMission_SizeChanged(object sender, EventArgs e)
        {
            int empty = 10;

            int x = this.Size.Width - rbtnComplete.Width;

            //int x = rbtnComplete.Location.X + rbtnComplete.Size.Width + 4;
            //m_rectTime = new Rectangle(x, 0, this.Size.Width - x, this.Height);

            int rectTimeWidth = 60;
            m_rectTime = new Rectangle(x - rectTimeWidth - empty, 0, rectTimeWidth, this.Height);

            m_rectMission = new Rectangle(TextBeginPos, 0, m_rectTime.X - empty - TextBeginPos - (int)(rbtnRun.Size.Width * 1.5), this.Height);
        }

        public void SetCompleteCheck(bool isChecked)
        {
            if (rbtnComplete.IsChecked != isChecked)
            {
                rbtnComplete.IsChecked = isChecked;

                if (rbtnComplete.IsChecked)
                    rbtnRun.Enabled = false;
                else
                    rbtnRun.Enabled = true;

                SetCompleteImage();
                rbtnComplete.Refresh();
            }
        }

        private void SetEnable(bool enabled)
        {
            m_isEnabled = enabled;
            rbtnComplete.Enabled = m_isEnabled;
            rbtnRun.Enabled = m_isEnabled;

            SetCompleteImage();

            if (m_isEnabled)
                m_timeBrush = m_enabledTimeBrush;
            else
                m_timeBrush = m_disabledTimeBrush;

            this.Refresh();
        }

        private void SetCompleteImage()
        {
            if (rbtnComplete.Enabled == false)
            {
                if (rbtnComplete.IsChecked)
                    rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Checked_Disabled;
                else
                    rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_Disabled;
            }
        }

        public void GetItem(out bool isComplete, out string strItem, out VariousData<DateTime> executeTime, out VariousData<DateTime> completeTime, out VariousData<DateTime> unCompleteTime)
        {
            executeTime = m_executeTime;
            isComplete = rbtnComplete.IsChecked;
            //isComplete = m_completeTime != null;
            strItem = m_strMission;

            if (isComplete)
            {
                completeTime = m_completeTime;
                unCompleteTime = null;
            }
            else
            {
                completeTime = null;
                unCompleteTime = m_unCompleteTime;
            }

            //completeTime = m_completeTime;
            //unCompleteTime = m_unCompleteTime;
        }

        private void PanelMission_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_isEnabled == false)
                return;

            if (e.X >= rbtnComplete.Location.X && e.X <= rbtnComplete.Location.X + rbtnComplete.Size.Width &&
                e.Y >= rbtnComplete.Location.Y && e.Y <= rbtnComplete.Location.Y + rbtnComplete.Size.Height)
                return;

            IsSelected = !IsSelected;
        }

        public void SelectControl(bool isSelected)
        {
            if (m_isSelected != isSelected)
            {
                m_isSelected = isSelected;

                if (m_isSelected)
                    this.BackColor = SelectedColor;
                else
                    this.BackColor = NotSelectedColor;

                if (m_owner != null)
                    m_owner.OnSelectMission(this, m_isSelected);

                if (m_owner == null || m_owner.ContentsOwner == null || m_owner.ContentsOwner.AllowSectionRefresh)
                    Refresh();
            }
        }

        private void rbtnRun_Click(object sender, EventArgs e)
        {
            if (m_mission != null)
            {
                if (m_mission is MissionItemExternal)
                {
                    m_owner.RunMissionExternal((MissionItemExternal)m_mission);
                }
                else
                {
                    string strMission = m_strMission.Trim();

                    if (strMission.Length > 0)
                    {
                        // 수신자 정보가 초기화된 이후에 DB가 바뀌었을수 있으니 새로 읽어온다.
                        SetReceivers();

                        if (m_arrPhoneNumbers != null && m_strSender.Length > 0)
                        {
                            string strErrorMessage = "";
                            strMission = "[" + m_owner.OriginalTitle + "] " + strMission;

                            if (m_owner.ContentsOwner.OnSendSMSClick(m_arrPhoneNumbers, m_strSender, strMission, true, out strErrorMessage))
                            {
                                m_executeTime = new VariousData<DateTime>(DateTime.Now);
                                m_clrSMSResult = m_clrSMSResultSuccess;
                                //AfterRunExecute();
                                SectionContentsHelper.SendLogState(m_owner, null, null, new MissionData(MissionData.ProcessType.SendSMS, m_executeTime.Data, m_nMissionIndex));
                            }
                            else
                            {
                                m_clrSMSResult = m_clrSMSResultFail;

                                if (strErrorMessage.Length > 0)
                                    m_strSMSSendResult = " - " + strErrorMessage;
                            }

                            Refresh();
                        }
                    }
                }

                rbtnRun.Enabled = false;
            }
        }

        public void AutoRun()
        {
            if (rbtnRun.Visible == false || rbtnRun.Enabled == false)
                return;

            rbtnRun_Click(null, null);
        }

        public bool SetReceivers()
        {
            string strSender;
            m_arrPhoneNumbers = SectionContentsHelper.GetSMSInfo(m_owner, out strSender, out m_dicPhoneNumbers);

            if (m_arrPhoneNumbers == null)
                return false;

            RemoveNullPhoneNumber();

            m_strSender = strSender;
            /*SetSMSMembers();

            string strReceiver = string.Format("수신자(총 {0}명)", m_arrPhoneNumbers.Count);
            gridReceivers.Columns[0].HeaderText = strReceiver;*/
            return m_arrPhoneNumbers.Count > 0;
        }

        public void SetReceivers(ArrayList arrPhoneNumbers, string strSender, Dictionary<string, string> dicPhoneNumbers)
        {
            m_arrPhoneNumbers = arrPhoneNumbers;
            m_strSender = strSender;

            if (dicPhoneNumbers == null)
                m_dicPhoneNumbers = null;
            else
            {
                if (m_dicPhoneNumbers == null)
                    m_dicPhoneNumbers = new Dictionary<string, string>();
                else
                    m_dicPhoneNumbers.Clear();

                foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
                {
                    m_dicPhoneNumbers[pair.Key] = pair.Value;
                }
            }

            RemoveNullPhoneNumber();
        }

        private void RemoveNullPhoneNumber()
        {
            if (m_arrPhoneNumbers != null)
                m_arrPhoneNumbers.Remove("");
        }

        public string ChangeMission(WorkflowOption option, bool isRealMode)
        {
            if (option == null)
                return m_strMission;

            if (option is WorkflowOptionEarthquake)
            {
                m_strMission = SectionContentsHelper.ChangeEarthquakeString(m_strMission, (WorkflowOptionEarthquake)option);
            }
            else if (option is WorkflowOptionPSM)
            {
                m_strMission = SectionContentsHelper.ChangePSMString(m_strMission, (WorkflowOptionPSM)option);
            }
            else if (option is WorkflowOptionSnowFall)
            {
                m_strMission = SectionContentsHelper.ChangeClimateString(m_strMission, (WorkflowOptionSnowFall)option);
            }
            else if (option is WorkflowOptionWind)
            {
                m_strMission = SectionContentsHelper.ChangeClimateString(m_strMission, (WorkflowOptionWind)option);
            }
            else if (option != null)
            {
                m_strMission = SectionContentsHelper.ChangeCommonString(m_strMission, option);
            }

            if (m_mission != null)
            {
                if (m_mission is MissionItemExternal)
                {
                    MissionItemExternal item = (MissionItemExternal)m_mission;
                    int nArgumentCount = item.Arguments.Count;

                    if (m_strMission.ToLower().StartsWith("#exec"))
                    //if (m_strMission == item.Mission)
                    {
                        // 주석이 있으면 주석을 표시한다.
                        if (item.Description.Length > 0)
                            m_strMission = item.Description;
                    }

                    for (int i = 0; i < nArgumentCount; i++)
                    {
                        if (option is WorkflowOptionEarthquake)
                        {
                            item.Arguments[i] = SectionContentsHelper.ChangeEarthquakeString(item.OriginalArguments[i], (WorkflowOptionEarthquake)option);
                        }
                        else if (option is WorkflowOptionPSM)
                        {
                            item.Arguments[i] = SectionContentsHelper.ChangePSMString(item.OriginalArguments[i], (WorkflowOptionPSM)option);
                        }
                        else if (option is WorkflowOptionSnowFall)
                        {
                            item.Arguments[i] = SectionContentsHelper.ChangeClimateString(item.OriginalArguments[i], (WorkflowOptionSnowFall)option);
                        }
                        else if (option is WorkflowOptionWind)
                        {
                            item.Arguments[i] = SectionContentsHelper.ChangeClimateString(item.OriginalArguments[i], (WorkflowOptionWind)option);
                        }
                        else if (option != null)
                        {
                            item.Arguments[i] = SectionContentsHelper.ChangeCommonString(item.OriginalArguments[i], option);
                        }
                    }

                    SetButtonMode(false);
                    //rbtnRun.Text = "실행";
                }
                else
                {
                    SetButtonMode(true);
                    //rbtnRun.Text = "문자";
                }
            }

            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(m_strMission, option.DetectTime == null ? DateTime.Now : option.DetectTime.Data, option.PositionName, option.AlarmMessage);
            param.RealMode = isRealMode ? 1 : 0;
            m_strMission = UnE.SOP.Utility.SOPSimulatorScript.Parse(param);

            m_strMission = ComponentContents.ParseUserDefinedParameters(option, m_strMission);

            return m_strMission;
        }

        private void SetButtonMode(bool isSMS)
        {
            if (isSMS)
            {
                if (UnE.SOP.ProxySOP.Instance.SiteID == 205)
                {
                    rbtnRun.Visible = false;
                    return;
                }
                rbtnRun.CheckedBkgndImage = null;
                rbtnRun.CheckedImage = null;
                rbtnRun.CheckedMouseOver = null;
                rbtnRun.ClickedBackgroundImage = null;
                rbtnRun.ClickedImage = null;
                rbtnRun.DisabledBkgndImage = null;
                rbtnRun.DisabledImage = global::SectionContents.Properties.Resources.SMS_Disabled;
                rbtnRun.MouseOverBkgndImage = null;
                rbtnRun.MouseOverImage = global::SectionContents.Properties.Resources.SMS_Selected_MouseOver;
                rbtnRun.NormalImage = global::SectionContents.Properties.Resources.SMS_Selected;                
            }
            else
            {
                rbtnRun.CheckedBkgndImage = null;
                rbtnRun.CheckedImage = null;
                rbtnRun.CheckedMouseOver = null;
                rbtnRun.ClickedBackgroundImage = null;
                rbtnRun.ClickedImage = global::SectionContents.Properties.Resources.RunButton_MouseOver;
                rbtnRun.DisabledBkgndImage = null;
                rbtnRun.DisabledImage = global::SectionContents.Properties.Resources.RunButton_Disabled;
                rbtnRun.MouseOverBkgndImage = null;
                rbtnRun.MouseOverImage = global::SectionContents.Properties.Resources.RunButton_MouseOver;
                rbtnRun.NormalImage = global::SectionContents.Properties.Resources.RunButton_Normal;
            }

            rbtnRun.Refresh();
        }
    }
}
