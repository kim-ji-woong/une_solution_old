using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public class SectionTimeText : Section
    {
        protected SectionTimeTextBox m_textBoxBeginTime = null;
        protected SectionTimeTextBox m_textBoxProcessTime = null;
        protected int m_nBeginTimeHour = 0, m_nBeginTimeMinute = 0;
        protected int m_nProcessTimeHour = 0, m_nProcessTimeMinute = 0;
        protected string m_strPrev = "";
        // 만일 중복된 이름의 멤버가 존재할 경우 Member ID로 구분
        //protected int m_nMemberID = -1;
        protected TeamData m_data = null;

        // 조직 그룹에 연결된 임무 정보
        private MemberofSection m_missionData = null;
        // 프로세스에 연결된 상황전파
        private ProcessSection m_processData = null;

        static protected int SPACE_TIME_N_RECT = 5;

        public SectionTimeText(Form frmParent)
            : base(frmParent)
        {
            InitControl();
        }

        public SectionTimeText(Form frmParent, int x, int y)
            : base(frmParent, x, y)
        {
            InitControl();
        }

        public SectionTimeText(Form frmParent, int x, int y, int width, int height)
            : base(frmParent, x, y, width, height)
        {
            InitControl();
        }

        public override void SetText(string text)
        {
            m_textBox.Text = text;
            m_strPrev = text;
        }

        public void SetData(TeamData data)
        {
            if (data != null)
                SetText(data.Name);

            m_data = data;
        }

        public string GetPrevText()
        {
            return m_strPrev;
        }

        // m_textBox의 text가 strText로 변경되었음
        public override void OnTextChanged(string strText)
        {
            // Process일 경우
            if (GetParentSection() == null)
                return;

            if (strText.Length > 0)
            {
                ArrayList arrTeamData = new ArrayList();
                int nDataCount = FormTeam.Instance(true).FindItem(strText, ref arrTeamData);

                if (nDataCount == 0)
                {
                    MessageBox.Show(string.Format("{0}라는 팀 또는 팀원이 존재하지 않습니다.", strText));
                    m_textBox.Text = m_strPrev;
                    return;
                }
                else if (nDataCount > 1)
                {
                    FormTeamList frm = new FormTeamList(arrTeamData);

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        int nIndex = frm.GetSelectedIndex();

                        if (nIndex < 0)
                        {
                            m_textBox.Text = m_strPrev;
                            return;
                        }
                        else
                        {
                            TeamData data = (TeamData)arrTeamData[nIndex];
                            //m_nMemberID = data.ID;
                            m_data = data;
                        }
                    }
                    else
                    {
                        m_textBox.Text = m_strPrev;
                        return;
                    }
                }
                else// if (nDataCount == 1)
                {
                    TeamData data = (TeamData)arrTeamData[0];
                    //m_nMemberID = data.ID;
                    m_data = data;
                }
            }
            else
            {
                //m_nMemberID = -1;
                m_data = null;
            }

            m_strPrev = strText;
        }

        protected void InitControl()
        {
            m_textBoxBeginTime = new SectionTimeTextBox(this);
            m_textBoxProcessTime = new SectionTimeTextBox(this);

            m_textBoxBeginTime.Size = new Size(60, m_nHeight / 2);
            m_textBoxBeginTime.Left = x - SPACE_TIME_N_RECT - m_textBoxBeginTime.Size.Width;
            m_textBoxBeginTime.Top = y + 2;

            m_textBoxBeginTime.Parent = m_frmParent;
            m_textBoxBeginTime.BorderStyle = BorderStyle.None;
            m_textBoxBeginTime.TextAlign = HorizontalAlignment.Right;
            //m_textBoxBeginTime.SelectionAlignment = HorizontalAlignment.Right;
            //m_textBoxBeginTime.Enabled = false;
            //m_textBoxBeginTime.ReadOnly = true;
            m_textBoxBeginTime.ForeColor = Color.Red;
            
            
            m_textBoxProcessTime.Size = new Size(60, m_nHeight / 2);
            m_textBoxProcessTime.Left = x - SPACE_TIME_N_RECT - m_textBoxProcessTime.Size.Width;
            m_textBoxProcessTime.Top = y + m_nHeight / 2 + 2;

            m_textBoxProcessTime.Parent = m_frmParent;
            m_textBoxProcessTime.BorderStyle = BorderStyle.None;
            m_textBoxProcessTime.TextAlign = HorizontalAlignment.Right;
            //m_textBoxProcessTime.SelectionAlignment = HorizontalAlignment.Right;
            //m_textBoxProcessTime.Enabled = false;
            //m_textBoxProcessTime.ReadOnly = true;
            m_textBoxProcessTime.ForeColor = Color.Blue;

            SetTime(true, 0, 0);
            SetTime(false, 0, 0);
        }

        protected void _SetTime(int nHour, int nMinute, string strTag, ref int rTimeHour, ref int rTimeMinute, ref SectionTimeTextBox rTextBox)
        {
            rTimeHour = nHour;
            rTimeMinute = nMinute;

            rTextBox.SetTag(strTag);
            rTextBox.Text = String.Format("{0} {1:00}h:{2:00}m", strTag, rTimeHour, rTimeMinute);
        }

        public void SetTime(bool isBegin, int nHour, int nMinute)
        {
            if (isBegin)
                _SetTime(nHour, nMinute, ">", ref m_nBeginTimeHour, ref m_nBeginTimeMinute, ref m_textBoxBeginTime);
            else
                _SetTime(nHour, nMinute, "+", ref m_nProcessTimeHour, ref m_nProcessTimeMinute, ref m_textBoxProcessTime);
        }

        public void SetTime(SectionTimeTextBox textBox, int nHour, int nMinute)
        {
            if (textBox == m_textBoxBeginTime)
            {
                m_nBeginTimeHour = nHour;
                m_nBeginTimeMinute = nMinute;
            }
            else if (textBox == m_textBoxProcessTime)
            {
                m_nProcessTimeHour = nHour;
                m_nProcessTimeMinute = nMinute;
            }
        }

        public void GetTime(bool isBegin, out int rHour, out int rMinute)
        {
            if (isBegin)
            {
                rHour = m_nBeginTimeHour;
                rMinute = m_nBeginTimeMinute;
            }
            else
            {
                rHour = m_nProcessTimeHour;
                rMinute = m_nProcessTimeMinute;
            }
        }

        public void GetTime(SectionTimeTextBox textBox, out int rHour, out int rMinute)
        {
            rHour = rMinute = 0;

            if (textBox == m_textBoxBeginTime)
            {
                rHour = m_nBeginTimeHour;
                rMinute = m_nBeginTimeMinute;
            }
            else if (textBox == m_textBoxProcessTime)
            {
                rHour = m_nProcessTimeHour;
                rMinute = m_nProcessTimeMinute;
            }
        }

        public string GetTimeString(SectionTimeTextBox textBox, bool noTag)
        {
            if (textBox == m_textBoxBeginTime)
            {
                return GetTimeString(true, noTag);
            }
            else if (textBox == m_textBoxProcessTime)
            {
                return GetTimeString(false, noTag);
            }

            return "";
        }

        public string GetTimeString(bool isBeginTime, bool noTag)
        {
            if (isBeginTime)
            {
                if (noTag)
                    return String.Format("{0:00}:{1:00}", m_nBeginTimeHour, m_nBeginTimeMinute);
                else
                    return String.Format("> {0:00}h:{1:00}m", m_nBeginTimeHour, m_nBeginTimeMinute);
            }
            else
            {
                if (noTag)
                    return String.Format("{0:00}:{1:00}", m_nProcessTimeHour, m_nProcessTimeMinute);
                else
                    return String.Format("+ {0:00}h:{1:00}m", m_nProcessTimeHour, m_nProcessTimeMinute);
            }

            //return "";
        }

        public bool CheckDuplicateTeamSchedule()
        {
            int nHour, nMinute;
            GetTime(true, out nHour, out nMinute);

            if (!CheckDuplicateTeamSchedule(ref nHour, ref nMinute))
                return false;

            SetTime(true, nHour, nMinute);
            return true;
        }

        // 같은 조직, 같은 시간의 데이터가 이미 존재하는지 확인
        // m_textBoxBeginTime에만 해당함
        protected bool CheckDuplicateTeamSchedule(ref int nHour, ref int nMinute)
        {
            // Process는 중복체크 하지 않음
            if (m_sectionParent == null)
                return true;
            if (Data == null)
                return true;

            FormTeamSchedule frm = null;
            int nIndex = -1;

            ArrayList arrChilds = m_sectionParent.GetChildSections();

            foreach (SectionTimeText section in arrChilds)
            {
                if (section == this) continue;
                if (section.Data == null) continue;

                if (section.Data.Type == this.Data.Type &&
                    section.Data.ID == this.Data.ID)
                {
                    if (frm == null)
                        frm = new FormTeamSchedule(Data.Type == TeamData.DataType.TeamData);

                    int nHour2, nMin2;
                    section.GetTime(true, out nHour2, out nMin2);

                    frm.AddBeginTime(string.Format("{0:00}:{1:00}", nHour2, nMin2));

                    if (nHour == nHour2 && nMinute == nMin2)
                    {
                        nIndex = frm.GetDataCount() - 1;
                        frm.SetData(Data);
                        frm.SetDescription("이미 같은 시간의 데이터가 존재합니다.\r\n기존에 존재하는 것과 다른 시작 시간을 지정하세요.h");
                        frm.SelectCell(nIndex, 0);
                    }
                }
            }

            if (frm != null && nIndex >= 0)
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    frm.GetBeginTime(out nHour, out nMinute);
                    return true;
                }
                else
                    return false;
            }

            return true;
        }

        public bool ChangeTime(SectionTimeTextBox textBox, ref int nHour, ref int nMinute)
        {
            //bool isBeginTime = true;
            int nAddHour, nAddMinute;
            FormProcess frm = (FormProcess)m_frmParent;

            if (textBox == m_textBoxBeginTime)
            {
                //isBeginTime = true;
                if (!CheckDuplicateTeamSchedule(ref nHour, ref nMinute))
                    return false;

                nAddHour = nHour - m_nBeginTimeHour;
                nAddMinute = nMinute - m_nBeginTimeMinute;
                AddTime(nAddHour, nAddMinute);
            }
            else if (textBox == m_textBoxProcessTime)
            {
                //isBeginTime = false;

                nAddHour = nHour - m_nProcessTimeHour;
                nAddMinute = nMinute - m_nProcessTimeMinute;
            }
            else
                return false;

            frm.OnAddTime(this, nAddHour, nAddMinute);
            return true;
        }

        public void AddTime(int nHour, int nMinute)
        {
            m_nBeginTimeHour += nHour;
            m_nBeginTimeMinute += nMinute;

            if (m_nBeginTimeMinute >= 60)
            {
                m_nBeginTimeHour += (m_nBeginTimeMinute / 60);
                m_nBeginTimeMinute = m_nBeginTimeMinute % 60;
            }
            else if (m_nBeginTimeMinute < 0)
            {
                int nAddHour = (m_nBeginTimeMinute / 60) - 1;
                m_nBeginTimeHour += nAddHour;
                m_nBeginTimeMinute = m_nBeginTimeMinute - nAddHour * 60;
            }

            foreach (SectionTimeText child in m_arrChildSection)
            {
                child.AddTime(nHour, nMinute);
            }

            SetTime(true, m_nBeginTimeHour, m_nBeginTimeMinute);
        }

        public override void Show()
        {
            base.Show();
            m_textBoxBeginTime.Show();
            m_textBoxProcessTime.Show();

            m_textBox.Visible = true;
            m_textBoxBeginTime.Visible = true;
            m_textBoxProcessTime.Visible = true;

            foreach (SectionTimeText child in m_arrChildSection)
            {
                child.Show();
            }
        }

        public override void Hide()
        {
            base.Hide();
            m_textBoxBeginTime.Hide();
            m_textBoxProcessTime.Hide();

            if (m_textBox.Visible == true)
            {
                m_textBox.Visible = false;
                m_textBoxBeginTime.Visible = false;
                m_textBoxProcessTime.Visible = false;
            }

            foreach (SectionTimeText child in m_arrChildSection)
            {
                child.Hide();
            }
        }

        public override Point Position
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                if (x != value.X || y != value.Y)
                {
                    base.Position = value;

                    m_textBoxBeginTime.Left = x - SPACE_TIME_N_RECT - m_textBoxBeginTime.Size.Width;
                    m_textBoxBeginTime.Top = y + 2;

                    m_textBoxProcessTime.Left = x - SPACE_TIME_N_RECT - m_textBoxProcessTime.Size.Width;
                    m_textBoxProcessTime.Top = y + m_nHeight / 2 + 2;
                }
            }
        }

        public TeamData Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public MemberofSection MissionData
        {
            get { return m_missionData; }
            set
            {
                m_missionData = value;
                if (m_missionData != null)
                {
                    if (m_missionData.LinkedSection != this)
                        m_missionData.LinkedSection = this;
                }
            }
        }

        public ProcessSection ProcessData
        {
            get { return m_processData; }
            set
            {
                m_processData = value;
                if (m_processData != null)
                {
                    if (m_processData.LinkedSection != this)
                        m_processData.LinkedSection = this;
                }
            }
        }
    }
}
