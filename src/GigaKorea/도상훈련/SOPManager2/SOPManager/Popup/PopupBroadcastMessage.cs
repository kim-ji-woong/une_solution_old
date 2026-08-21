using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpeechLib;

namespace SOPManager
{
    public partial class PopupBroadcastMessage : Form
    {
        private string m_szText = "";
        private Sections.SectionCommander m_commander = null;

        private PropertiesInternal m_propertiesInternal = null;
        public PropertiesInternal PropertiesInternal
        {
            get { return m_propertiesInternal; }
            set
            {
                m_propertiesInternal = value;
                if (m_propertiesInternal != null)
                    InitText();
            }
        }

        string m_szTitleText = "";
        public string TitleText
        {
            get { return m_szTitleText; }
            set { m_szTitleText = value; }
        }

        private Sections.SectionInternal m_section = null;

        private ArrayList m_TeamList = new ArrayList();
        public ArrayList TeamList
        {
            get { return m_TeamList; }
        }

        public PopupBroadcastMessage()
        {
            InitializeComponent();
        }

        private void InitText()
        {
            m_section = (Sections.SectionInternal)m_propertiesInternal.GetSection();

            m_szTitleText = m_section.Title;
            textBox.Text = m_szTitleText;

            m_TeamList = (ArrayList)m_propertiesInternal.SelectedTeamList.Clone() ;
            InitSelectTeam();
            
            Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)m_section.Data;
            m_commander = new Sections.SectionCommander();
            m_commander.DisplayText = sectionData.Commander.DisplayText;
            m_commander.IsTeamMember = sectionData.Commander.IsTeamMember;
            m_commander.Team = sectionData.Commander.Team;
            m_commander.TeamMemberID = sectionData.Commander.TeamMemberID;

            if (m_commander != null && m_commander.DisplayText != null)
                textBoxCommander.Text = m_commander.DisplayText;           

            if(sectionData.UseMobileApp == true)
            {
                rbBtnMobile.Checked = true;
            }
            else
            {
                rbBtnBroadcast.Checked = true;
            }

            InitText(sectionData.BroadcastMessage);

            checkBoxAutoRun.Checked = sectionData.AutoRun;

            //btnPreview.Font = new System.Drawing.Font(Program.prgFont, 12f, FontStyle.Bold);
            UpdateMobileMessage();
            UpdateBroadCast();
            UpdateAutoRun();
            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            foreach (Control ctl in this.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        private void UpdateMobileMessage()
        {
            if (rbBtnMobile.Checked == true)
            {
                picTextSend.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picTextSend.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }
        }

        private void UpdateBroadCast()
        {
            if (rbBtnBroadcast.Checked == true)
            {
                picBroadcast.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picBroadcast.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }
        }

        private void UpdateAutoRun()
        {
            if (checkBoxAutoRun.Checked == true)
            {
                picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;
            }
            else
            {
                picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_szText = textMessage.Text;

            SaveData();

            this.DialogResult = DialogResult.OK;
			this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
			this.Close();
        }

        public void InitText(string szText)
        {
			m_szText = szText;
            textMessage.Text = szText;
        }

		
        public string GetMessage()
        {
			return m_szText;
        }

        private void PopupBroadcastMessage_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void PopupBroadcastMessage_MouseMove(object sender, MouseEventArgs e)
        {
        }
        
        private void btnSpecialMessage_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ShowSpecialMessage();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);

            PopupPreviewMessage preview = new PopupPreviewMessage(textMessage.Text, dtTime, "[재난발생위치]");
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(preview);
            frame.TopMost = true;
			frame.ShowDialog();
        }

        private void btnTTS_Click(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);

            PopupPreListenMessage preview = new PopupPreListenMessage(textMessage.Text, dtTime, "[재난발생위치]");
            preview.ChangeMsg += (msg) =>
                {
                    textMessage.Text = msg;
                };

            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(preview);
            frame.TopMost = true;
            frame.ShowDialog();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSelectCommander_Click(object sender, EventArgs e)
        {
            // 171207 KYJ
            //Popup.PopupSelectCommander frm = new Popup.PopupSelectCommander(m_commander);
            PopupSelectTeam frm = new PopupSelectTeam(m_commander);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(frm);
            frame.Text = "발신자 선택";
            frame.Sizable = false;
            frame.StartPosition = FormStartPosition.CenterScreen;
            if (frame.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxCommander.Text = frm.DisplayText;

                if (m_commander == null)
                    m_commander = new Sections.SectionCommander();

                m_commander.Team = (Sections.SOPTeam)frm.SelectedTeam;
                m_commander.IsTeamMember = false;
                m_commander.DisplayText = frm.DisplayText;
            }
        }

        private void btnSelectTeam_Click(object sender, EventArgs e)
        {
            PopupSelectTeam();
        }

        private void rbBtnMobile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBtnMobile.Checked == true)
            {
                textBoxCommander.Enabled = true;
                btnSelectCommander.Enabled = true;
                txtSelectTeam.Enabled = true;
                btnSelectTeam.Enabled = true;
            }
        }

        private void rbBtnBroadcast_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBtnBroadcast.Checked == true)
            {
                textBoxCommander.Enabled = false;
                btnSelectCommander.Enabled = false;
                txtSelectTeam.Enabled = false;
                btnSelectTeam.Enabled = false;               
            }
        }

        private void PopupSelectTeam()
        {
            using (PopupSelectTeam form = new PopupSelectTeam(false))
            {
                UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
                frame.Sizable = false;
                form.TeamList = this.m_TeamList;
                form.PropertiesInternal = m_propertiesInternal;
                frame.StartPosition = FormStartPosition.CenterScreen;

                if (frame.ShowDialog(this) == DialogResult.OK)
                {
                    m_TeamList = form.TeamList;
                    InitSelectTeam();
                }
            }
        }

        private void InitSelectTeam()
        {
            string strTeamText = "";

            foreach (Sections.SOPTeam item in m_TeamList)
            {
                string strTeamName = item.IncludeChildTeams ? item.TeamName + SOPManager.PopupSelectTeam.INCLUDE_TAG : item.TeamName;

                if (String.IsNullOrWhiteSpace(strTeamText))
                    strTeamText = strTeamName;
                else
                    strTeamText += String.Format(", {0}", strTeamName);
            }

            txtSelectTeam.Text = strTeamText;
        }

        public void SaveData()
        {
            bool bChangedData = false;

            m_propertiesInternal.EnabledSnapshot = false;
            Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)m_section.Data;

            if (m_szTitleText != textBox.Text)
            {
                SaveSnapShot("내부상황전파 변경");
                m_szTitleText = textBox.Text;
                m_propertiesInternal.Text = TitleText;
                bChangedData = true;
            }           
            
            if(CompareCommander(sectionData.Commander,m_commander))
            {
                SaveSnapShot("내부상황전파 변경");
                m_propertiesInternal.SectionCommander = m_commander;
                bChangedData = true;
            }

            if (rbBtnBroadcast.Checked == true && sectionData.UseBroadcast == false)
            {
                SaveSnapShot("내부상황전파 변경");
                m_propertiesInternal.BroadcastConfig = "사용";
                bChangedData = true;
            }

            if (rbBtnMobile.Checked == true && sectionData.UseMobileApp == false)
            {
                SaveSnapShot("내부상황전파 변경");
                m_propertiesInternal.MobileAppConfig = "사용";
                bChangedData = true;
            }

            if (sectionData.BroadcastMessage != textMessage.Text)
            {
                SaveSnapShot("내부상황전파 변경");
                m_propertiesInternal.BroadcastContent = textMessage.Text;
                bChangedData = true;
            }

            if (CompareTeamList(sectionData.TeamList, m_TeamList))
            {
                SaveSnapShot("내부상황전파 변경");
                m_propertiesInternal.SelectedTeamList = m_TeamList;
                bChangedData = true;
            }

            if (sectionData.AutoRun != checkBoxAutoRun.Checked)
            {
                SaveSnapShot("내부상황전파 변경");
                m_propertiesInternal.AutoRun = checkBoxAutoRun.Checked ? UsingType.TypeList[1] : UsingType.TypeList[0];
                bChangedData = true;
            }
            
            m_propertiesInternal.EnabledSnapshot = true;

            if (bChangedData == true)
            {
                if(m_section.GetParent() != null)
                {
                    m_section.GetParent().Refresh();
                }
            }
            m_propertiesInternal.EnabledSnapshot = true;
        }

        private bool bSaveSnapShot = false;
        private void SaveSnapShot(string szName)
        {
            if (bSaveSnapShot == false)
            {
                UndoRedoManager.Instance.SaveSnapshot(szName);
                bSaveSnapShot = true;
            }
        }

        private bool CompareCommander(Sections.SectionCommander commander1, Sections.SectionCommander commander2)
        {
            if (commander1 == null)
                return false;

            if (commander2 == null)
                return true;

            if (commander1.Team != null && commander2.Team == null)
            {
                return true;
            }
            if (commander2.Team != null && commander1.Team == null)
            {
                return true;
            }
            if ((commander2.Team == null && commander1.Team == null) && (commander2.DisplayText == commander1.DisplayText))
            {
                return false;
            }

            if (commander1.DisplayText != commander2.DisplayText)
                return true;
            if (commander1.TeamMemberID != commander2.TeamMemberID)
                return true;
            if (commander1.Team.TeamType != commander2.Team.TeamType)
            {
                return true;
            }
            if (commander1.Team.TeamID != commander2.Team.TeamID)
            {
                return true;
            }

            return false;
        }
        
        private bool CompareTeamList(ArrayList arMission, ArrayList arOrgMission)
        {
            if (arMission == null)
                return false;

            if (arOrgMission == null)
                return true;

            if (arMission.Count != arOrgMission.Count)
                return true;

            for (int i = 0; i < arMission.Count; i++)
            {
                Sections.SOPTeam item = (Sections.SOPTeam)arMission[i];
                Sections.SOPTeam item2 = (Sections.SOPTeam)arOrgMission[i];

                if (item.TeamID != item2.TeamID)
                    return true;
                if (item.TeamName != item2.TeamName)
                    return true;
                if (item.TeamType != item2.TeamType)
                    return true;
                if (item.IncludeChildTeams != item2.IncludeChildTeams)
                    return true;
            }
            return false;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void RibbonBtn_MouseDown(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton btn = sender as UnE.GUI.RibbonButton;
            if (btn == null) return;

            btn.ForeColor = Color.Black;
        }

        private void RibbonBtn_MouseUp(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton btn = sender as UnE.GUI.RibbonButton;
            if (btn == null) return;

            btn.ForeColor = Color.White;
        }

        private void MobileMessage_Click(object sender, EventArgs e)
        {
            rbBtnMobile.Checked = true;
            rbBtnBroadcast.Checked = false;

            UpdateMobileMessage();
            UpdateBroadCast();
        }

        private void BroadcastMessage_Click(object sender, EventArgs e)
        {
            rbBtnMobile.Checked = false;
            rbBtnBroadcast.Checked = true;

            UpdateMobileMessage();
            UpdateBroadCast();
        }

        private void AutoRun_Click(object sender, EventArgs e)
        {
            checkBoxAutoRun.Checked = !checkBoxAutoRun.Checked;
            UpdateAutoRun();
        }
    }
}
