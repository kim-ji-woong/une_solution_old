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
using DBUtility2;
using SectionContents.Utility;
using System.Collections;

namespace SectionContents.Fancy
{
    public partial class PanelInternal : UserControl
    {
        private static Pen m_pen = new Pen(Color.FromArgb(224, 224, 224), 1.0f);
        private static StringFormat m_textFormat = ComponentContents.GetStringFormat();
        private static Font m_titleFont = new Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

        private const int TextBeginPos = 25;
        private const int TopAreaHeight = 60;

        private ComponentContents m_owner = null;
        private SolidBrush m_titleBrush = new SolidBrush(Color.Black);
        private Rectangle m_rectTitle;

        private int m_nGridLocationFromEnd = -1;
        private int m_nDistanceToGrid = -1;
        private bool m_isEnabled = false;
        private bool m_isSMS = true;
        private bool m_useSiren = false;
        private string m_strExecuteMessage = "";
        private bool m_isComplete = false;
        private VariousData<DateTime> m_completeTime = null;
        private VariousData<DateTime> m_unCompleteTime = null;
        private VariousData<DateTime> m_executeTime = null;

        private Dictionary<string, string> m_dicPhoneNumbers = null;
        private ArrayList m_arrPhoneNumbers = null;
        private string m_strSender = "";
        private string m_strSMSSendResult = "";
        private Color m_clrSMSResultFail = Color.Red;
        private Color m_clrSMSResultSuccess = Color.Green;
        private Color m_clrSMSResult = Color.Black;

        private DataGridView m_gridSMSMembers = null;

        private bool m_systemInput = false;

        public bool EnableControl
        {
            get { return m_isEnabled; }
            set { SetEnable(value); }
        }

        public bool IsSMS
        {
            get { return m_isSMS; }
            set
            {
                if (m_isSMS != value)
                {
                    m_isSMS = value;
                    ChangeType();
                }
            }
        }

        public bool UseSiren
        {
            get { return m_useSiren; }
            set { SetSiren(value); }
        }

        public string Message
        {
            get { return textBoxMessage.Text; }
            set { textBoxMessage.Text = value; }
        }

        // 실제로 내보낸 메시지
        public string ExecuteMessage
        {
            get { return m_strExecuteMessage; }
        }

        public bool IsComplete
        {
            get { return m_isComplete; }
            //get { return rbtnComplete.IsChecked; }
        }

        public Color TitleColor
        {
            get { return m_titleBrush.Color; }
            set { m_titleBrush.Color = value; }
        }

        public PanelInternal(ComponentContents owner, bool isSMS, bool useSiren)
        {
            InitializeComponent();
            m_owner = owner;
            m_isComplete = rbtnComplete.IsChecked;
            IsSMS = isSMS;
            UseSiren = useSiren;

            MakeSMSMemberGrid();

            m_nGridLocationFromEnd = this.Size.Width - gridReceivers.Location.X;
            m_nDistanceToGrid = gridReceivers.Location.X - (textBoxMessage.Location.X + textBoxMessage.Size.Width);
            PanelInternal_Resize(null, null);
        }

        private void MakeSMSMemberGrid()
        {
            DataGridViewTextBoxColumn colTeamName = new DataGridViewTextBoxColumn();

            colTeamName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            colTeamName.DefaultCellStyle = colReceiver.DefaultCellStyle;
            colTeamName.HeaderText = "팀이름";
            colTeamName.Name = "colTeamName";
            colTeamName.ReadOnly = true;
            colTeamName.Width = 80;
            colTeamName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;

            DataGridViewTextBoxColumn colMemberName = new DataGridViewTextBoxColumn();

            colMemberName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            colMemberName.DefaultCellStyle = colReceiver.DefaultCellStyle;
            colMemberName.HeaderText = "수신자";
            colMemberName.Name = "colMemberName";
            colMemberName.ReadOnly = true;
            colMemberName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;

            m_gridSMSMembers = new DataGridView();

            m_gridSMSMembers.AllowUserToAddRows = false;
            m_gridSMSMembers.AllowUserToDeleteRows = false;
            m_gridSMSMembers.BackgroundColor = System.Drawing.Color.White;
            m_gridSMSMembers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            m_gridSMSMembers.ColumnHeadersDefaultCellStyle = gridReceivers.ColumnHeadersDefaultCellStyle;
            m_gridSMSMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            m_gridSMSMembers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {colTeamName, colMemberName});
            m_gridSMSMembers.Location = gridReceivers.Location;
            m_gridSMSMembers.Name = "gridSMSMembers";
            m_gridSMSMembers.ReadOnly = true;
            m_gridSMSMembers.RowHeadersVisible = false;
            m_gridSMSMembers.RowTemplate.Height = 23;
            m_gridSMSMembers.Size = gridReceivers.Size;
            m_gridSMSMembers.Visible = false;

            m_gridSMSMembers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);

            this.Controls.Add(m_gridSMSMembers);
        }

        private void PanelInternal_Resize(object sender, EventArgs e)
        {
            if (m_isSMS)
            {
                gridReceivers.Location = new Point(this.Size.Width - m_nGridLocationFromEnd, gridReceivers.Location.Y);

                if (m_gridSMSMembers != null)
                    m_gridSMSMembers.Location = gridReceivers.Location;

                textBoxMessage.Size = new Size(gridReceivers.Location.X - m_nDistanceToGrid - textBoxMessage.Location.X, textBoxMessage.Size.Height);
            }
            else
            {
                textBoxMessage.Size = new Size(this.Size.Width - m_nGridLocationFromEnd + gridReceivers.Size.Width - textBoxMessage.Location.X, textBoxMessage.Size.Height);
            }

            m_rectTitle = new Rectangle(TextBeginPos, 0, rbtnSpecial.Location.X - 10 - TextBeginPos, 60);
        }

        private void PanelInternal_Paint(object sender, PaintEventArgs e)
        {
            if (m_strSMSSendResult.Length == 0)
            {
                e.Graphics.DrawString("발송정보" + m_strSMSSendResult, m_titleFont, m_titleBrush, m_rectTitle, m_textFormat);
            }
            else
            {
                string strHeader = "발송정보";
                SizeF size = e.Graphics.MeasureString(strHeader, m_titleFont);
                e.Graphics.DrawString(strHeader, m_titleFont, m_titleBrush, m_rectTitle, m_textFormat);

                RectangleF rect = new RectangleF(m_rectTitle.X + size.Width, m_rectTitle.Y, m_rectTitle.Width - size.Width, m_rectTitle.Height);

                using (Font font = new Font(m_titleFont, FontStyle.Bold))
                {
                    using (Brush brush = new SolidBrush(m_clrSMSResult))
                    {
                        e.Graphics.DrawString(m_strSMSSendResult, font, brush, rect, m_textFormat);
                    }
                }
            }

            e.Graphics.DrawLine(m_pen, 0, TopAreaHeight - 2, this.Size.Width - 10, TopAreaHeight - 2);
        }

        public void AddTeamName(string strTeamName)
        {
            int nRowIndex = gridReceivers.Rows.Add();

            if (nRowIndex >= 0)
            {
                DataGridViewRow row = gridReceivers.Rows[nRowIndex];
                row.Cells[0].Value = strTeamName;
            }
        }

        private void SetEnable(bool enabled)
        {
            m_isEnabled = enabled;
            textBoxMessage.Enabled = rbtnSiren.Enabled = m_isEnabled;
            rbtnSMS.Enabled = rbtnComplete.Enabled = m_isEnabled;

            SetCompleteImage();
            this.Refresh();
        }

        private void SetSiren(bool use)
        {
            m_useSiren = use;
            rbtnSiren.IsChecked = use;

            if (rbtnSiren.IsChecked)
            {
                rbtnSiren.DisabledImage = global::SectionContents.Properties.Resources.SirenUse_Disabled;
            }
            else
            {
                rbtnSiren.DisabledImage = global::SectionContents.Properties.Resources.SirenNoUse_Disabled;
            }
        }

        private void ChangeType()
        {
            if (m_isSMS)
            {
                rbtnSMS.NormalImage = global::SectionContents.Properties.Resources.SMS_Selected;
                rbtnSMS.DisabledImage = global::SectionContents.Properties.Resources.SMS_Disabled;
                rbtnSMS.MouseOverImage = global::SectionContents.Properties.Resources.SMS_Selected_MouseOver;
                rbtnSiren.Visible = false;
                gridReceivers.Visible = true;
            }
            else
            {
                rbtnSMS.NormalImage = global::SectionContents.Properties.Resources.Broadcast_Selected;
                rbtnSMS.DisabledImage = global::SectionContents.Properties.Resources.Broadcast_Disabled;
                rbtnSMS.MouseOverImage = global::SectionContents.Properties.Resources.Broadcast_Selected_MouseOver;
                rbtnSiren.Visible = true;
                gridReceivers.Visible = false;

                textBoxMessage.Size = new Size(this.Size.Width - m_nGridLocationFromEnd + gridReceivers.Location.X + gridReceivers.Size.Width - textBoxMessage.Location.X, textBoxMessage.Size.Height);
            }
        }

        private void rbtnSiren_Click(object sender, EventArgs e)
        {
            rbtnSiren.IsChecked = !rbtnSiren.IsChecked;
            SetSiren(rbtnSiren.IsChecked);
        }

        private void rbtnComplete_Click(object sender, EventArgs e)
        {
            SetComplete(!IsComplete);
            //rbtnComplete.IsChecked = !rbtnComplete.IsChecked;
            //m_completeTime = new VariousData<DateTime>(DateTime.Now);
            rbtnSiren.Enabled = rbtnSMS.Enabled = !IsComplete;
            //rbtnSiren.Enabled = rbtnSMS.Enabled = !rbtnComplete.IsChecked;

            if (IsComplete)
            {
                m_completeTime = new VariousData<DateTime>(DateTime.Now);
                m_unCompleteTime = null;
            }
            else
            {
                m_unCompleteTime = new VariousData<DateTime>(DateTime.Now);
                m_completeTime = null;
            }

            if (IsComplete)
            //if (rbtnComplete.IsChecked)
                rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Checked_Disabled;
            else
                rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_Disabled;

            rbtnComplete.Refresh();

            if (m_owner != null)
                m_owner.OnCheckedComplete(-1, IsComplete);
        }

        public void GetItem(out bool isBroadcast, out bool isExecute, out bool isComplete, out int nBroadcastCount, out bool useSiren, out VariousData<DateTime> executeTime, out VariousData<DateTime> completeTime, out VariousData<DateTime> unCompleteTime)
        {
            isBroadcast = !m_isSMS;
            isExecute = m_executeTime != null;
            isComplete = IsComplete;
            //isComplete = m_completeTime != null;
            completeTime = m_completeTime;
            unCompleteTime = m_unCompleteTime;
            executeTime = m_executeTime;

            if (isExecute)
            {
                nBroadcastCount = 1;
                useSiren = m_useSiren;
            }
            else
            {
                nBroadcastCount = 0;
                useSiren = false;
            }
        }

        private void rbtnSMS_Click(object sender, EventArgs e)
        {
            if (m_owner.ContentsOwner == null)
                return;

            if (m_isSMS)
            {
                // 수신자 정보가 초기화된 이후에 DB가 바뀌었을수 있으니 새로 읽어온다.
                SetReceivers();
                /*string strSender;
                ArrayList arrPhoneNumbers = SectionContentsHelper.GetSMSInfo(m_owner, out strSender);

                if (arrPhoneNumbers == null)
                    return;*/

                if (m_arrPhoneNumbers != null && m_strSender.Length > 0)
                {
                    string strErrorMessage = "";

                    if (m_owner.ContentsOwner.OnSendSMSClick(m_arrPhoneNumbers, m_strSender, textBoxMessage.Text, !m_systemInput, out strErrorMessage))
                    {
                        m_clrSMSResult = m_clrSMSResultSuccess;
                        AfterRunExecute();
                        SectionContentsHelper.SendLogState(m_owner);
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
            else
            {
                if (m_owner.ContentsOwner.OnRunBroadcastClick(textBoxMessage.Text, 1, rbtnSiren.IsChecked, !m_systemInput) == false)
                    return;

                //if (m_owner.ContentsOwner.OnRunBroadcastClick(textBoxMessage.Text, 1, rbtnSiren.IsChecked))
                {
                    AfterRunExecute();
                    SectionContentsHelper.SendLogState(m_owner);
                }
            }
        }

        private void AfterRunExecute()
        {
            rbtnSMS.Enabled = false;
            rbtnSiren.Enabled = false;
            m_executeTime = new VariousData<DateTime>(DateTime.Now);
            m_strExecuteMessage = textBoxMessage.Text;

            if (m_isSMS)
            {
                m_strSMSSendResult = string.Format(" - 전송완료({0:00}:{1:00}:{2:00})", m_executeTime.Data.Hour, m_executeTime.Data.Minute, m_executeTime.Data.Second);
            }
        }

        public void SetDetailData(UnE.SOP.History.HistorySectionData.DetailData detail)
        {
            if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.RUN_SMS_INTERNAL)
            {
                if (m_isSMS == false || detail.Datas == null)
                    return;

                string strCommander, strCommanderDisplayText, strReceivers, strMsg;
                bool onlyTeamLeader;

                if (ParseRunSMSInternal(detail.Datas, out strCommander, out strCommanderDisplayText, out strReceivers, out onlyTeamLeader, out strMsg))
                {
                    textBoxMessage.Text = strMsg;
                    rbtnSMS.Enabled = false;

                    //Sections.SectionCommander commander = LoadCommander(strCommander);
                    //List<SOPTeam> receivers = LoadReceivers(strReceivers);

                    //frm.SetSMSOptions(commander, strCommanderDisplayText, receivers, new VariousData<bool>(onlyTeamLeader), strMsg, new VariousData<bool>(true), detail.Time, null, null);
                }
            }
            else if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.RUN_BROADCAST_INTERNAL)
            {
                if (m_isSMS || detail.Datas == null)
                    return;

                int nBroadcastCount;
                bool useSiren;
                string strMsg;

                if (ParseRunBroadcastInternal(detail.Datas, out nBroadcastCount, out useSiren, out strMsg))
                {
                    textBoxMessage.Text = strMsg;
                    rbtnSMS.Enabled = false;
                    //frm.SetBroadcastOptions(new VariousData<int>(nBroadcastCount), new VariousData<bool>(useSiren), new VariousData<bool>(true), detail.Time, null, null, strMsg);
                }
            }
            else if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_SMS_INTERNAL)
            {
                if (m_isSMS == false || detail.Datai == null)
                    return;

                VariousData<bool> completed = null;

                if (detail.Datai.Data == 1)
                    completed = new VariousData<bool>(true);
                else if (detail.Datai.Data == 0)
                    completed = new VariousData<bool>(false);
                else
                    return;

                SetCompleteCheck(completed.Data);
                //frm.SetSMSOptions(null, null, null, null, null, null, null, completed, detail.Time);
            }
            else if (detail.DataIndex.Data == UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_BROADCAST_INTERNAL)
            {
                if (m_isSMS || detail.Datai == null)
                    return;

                VariousData<bool> completed = null;

                if (detail.Datai.Data == 1)
                    completed = new VariousData<bool>(true);
                else if (detail.Datai.Data == 0)
                    completed = new VariousData<bool>(false);
                else
                    return;

                SetCompleteCheck(completed.Data);
                //frm.SetBroadcastOptions(null, null, null, null, completed, detail.Time, null);
            }
        }

        private void SetCompleteCheck(bool isChecked)
        {
            if (IsComplete != isChecked)
            {
                SetComplete(isChecked);
                //rbtnComplete.IsChecked = isChecked;
                SetCompleteImage();
                rbtnComplete.Refresh();
            }
        }

        private void SetCompleteImage()
        {
            if (rbtnComplete.Enabled == false)
            {
                if (IsComplete)
                    rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Checked_Disabled;
                else
                    rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_Disabled;
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

        private void rbtnSpecial_Click(object sender, EventArgs e)
        {
            ComponentContents.ShowSpecialMessageHelp();
        }

        public void Execute()
        {
            m_systemInput = true;

            if (rbtnSMS.Enabled)
                rbtnSMS_Click(null, null);

            if (IsComplete == false)
                rbtnComplete_Click(null, null);

            m_systemInput = false;
        }

        private void SetComplete(bool isComplete)
        {
            rbtnComplete.IsChecked = isComplete;
            m_isComplete = isComplete;
        }

        public bool SetReceivers()
        {
            if (m_isSMS)
            {
                string strSender;
                m_arrPhoneNumbers = SectionContentsHelper.GetSMSInfo(m_owner, out strSender, out m_dicPhoneNumbers);

                if (m_arrPhoneNumbers == null)
                    return false;

                m_strSender = strSender;
                SetSMSMembers();

                string strReceiver = string.Format("수신자(총 {0}명)", m_arrPhoneNumbers.Count);
                gridReceivers.Columns[0].HeaderText = strReceiver;
                return true;
            }

            return false;
        }

        private void SetSMSMembers()
        {
            m_gridSMSMembers.Rows.Clear();

            if (m_dicPhoneNumbers == null)
                return;

            foreach (KeyValuePair<string, string> pair in m_dicPhoneNumbers)
            {
                if (pair.Key.Length == 0)
                    continue;

                string[] tokens = pair.Value.Split(';');

                if (tokens.Count() != 2)
                    continue;

                int nRowIndex = m_gridSMSMembers.Rows.Add();

                if (nRowIndex < 0)
                    continue;

                DataGridViewRow row = m_gridSMSMembers.Rows[nRowIndex];

                row.Cells[0].Value = tokens[0].Trim();
                row.Cells[1].Value = tokens[1].Trim();
            }
        }

        private void tsMenuReceiverMembers_Click(object sender, EventArgs e)
        {
            if (sender == tsMenuShowReceiverMembers)
            {
                m_gridSMSMembers.Show();
                gridReceivers.Hide();
            }
            else if (sender == tsMenuHideReceiverMembers)
            {
                gridReceivers.Show();
                m_gridSMSMembers.Hide();
            }
        }

        private void grid_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender == gridReceivers)
            {
                tsMenuShowReceiverMembers.Enabled = true;
                tsMenuHideReceiverMembers.Enabled = false;
            }
            else if (sender == m_gridSMSMembers)
            {
                tsMenuShowReceiverMembers.Enabled = false;
                tsMenuHideReceiverMembers.Enabled = true;
            }
            else
                return;

            contextMenuStrip1.Show((Control)sender, e.X, e.Y);
        }

        public void ClearState()
        {
            if (m_isSMS)
            {
                gridReceivers.Show();
                m_gridSMSMembers.Hide();
            }

            m_strSMSSendResult = "";
        }
    }
}
