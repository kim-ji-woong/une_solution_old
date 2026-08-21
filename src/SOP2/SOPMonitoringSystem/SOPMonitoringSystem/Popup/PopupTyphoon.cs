using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SOPMonitoringSystem.Process;
using Sections;
using System.Diagnostics;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class PopupTyphoon : Form, AnnounceMessage
    {
        private Sections.Section mSection = null;
        private string m_strMode = "훈련 비상상황";
        private string szMessage = "";
        string AnnounceMessage.Message
        {
            get { return szMessage; }
            set { szMessage = value; }
        }
        private int nAnnoCount = 1;
        int AnnounceMessage.Count
        {
            get { return nAnnoCount; }
            set { nAnnoCount = value; }
        }
        private bool bUseSystemMsg = false;
        bool AnnounceMessage.UseSystemMessage
        {
            get { return bUseSystemMsg; }
            set { bUseSystemMsg = value; }
        }
        private string szSystemMsg = "";
        string AnnounceMessage.SystemMessage
        {
            get { return szSystemMsg; }
            set { szSystemMsg = value; }
        }
        private bool m_useSiren = true;
        bool AnnounceMessage.UseSiren
        {
            get { return m_useSiren; }
            set { m_useSiren = value; }
        }
        private bool m_useSenarioMessage = false;
        bool AnnounceMessage.UseSenarioMessage
        {
            get { return m_useSenarioMessage; }
            set { m_useSenarioMessage = value; }
        }
        private string m_szSenarioMessage = "";
        string AnnounceMessage.SenarioMessage
        {
            get { return m_szSenarioMessage; }
            set
            {
                m_szSenarioMessage = value;
                if (m_szSenarioMessage != null && !m_szSenarioMessage.Equals(""))
                {
                    cboBoxDisaster.SelectedIndex = 5;
                }
            }
        }

        private string szTime = "";

        string szMainMsg0 = "본부 재난안전대책본부에서 $1 을 알려드립니다.\n\n" +
            "현재  (==1==) 주의보가 발효 되어  (==2==) 으로 인한 피해가 예상 되오니, " +
            "전 직원은 다음 사항을 확인하여 피해가 없도록 철저히 대비하여 주시기 바랍니다.\n" +
            "1. 소화기 위치를 잘 숙지하시기 바랍니다.\n" +
            "2. 출입문 한 곳을 제외한 모든 출입문과 창문을 완전히 닫아 주시기 바랍니다.\n" +
            "3. 해안, 위험구현 가까이 주차한 차량은 피하가 없도록 안전한 장소로 이동해 주시기 바랍니다.\n" +
            "4. 중점관리 시설물에 대한 점검을 철저히 시행해 주시기 바랍니다.\n" +
            "5. 각 부서에서 관리하는 비상전원 설비, 회사장, 방파제, 부두 시설물, 석탄 취급 설비, 취수구, 파워블록 외벽, 오폐수처리설비, 배수펌프 작동 상태를 다시 한번 점검하시고, \n" +
            "6. 야적 자재의 유실 방지를 위한 보강조치를 확인하시기 바랍니다.";

        string szMainMsg1 = "본부 재난안전대책본부에서 $1 을 알려드립니다.\n\n" +
            "금일 현재시각  (==1==) 으로 인한 기상경보 발효로  (==3==) 비상을 발령합니다.\n" +
            "전 직원은 부서별 비상대응 책무를 수행하시기 바랍니다.";


        string szMainMsg2 = "본부 재난안전대책본부에서 $1 을 알려드립니다.\n" +
            "금일 현재시각  (==1==) 경보가 발효 되었습니다. \n" +
            "전 직원은 건물 내에서 대기하며 비상상황에 대응해 주시기 바라며, " +
            "해안설비, 저탄장, 부두시설, 취수로 등의 강풍, 해일 위험 지역 접근을 금지합니다.\n" +
            "사무실에 근무하고 계신 분은 지금 즉시 현장에서 신속히 대피할 수 있도록 긴급 연락해 주시기 바랍니다.";

        public PopupTyphoon(bool bVirtual)
            : base()
        {
            InitializeComponent();
            AdjustLocation(FormMain.Instance);
            RealMode(!bVirtual);
            

            szTime = GetTime();
            szMainMsg0 = szMainMsg0.Replace("$1", m_strMode);
            szMainMsg1 = szMainMsg1.Replace("$1", m_strMode);
            szMainMsg2 = szMainMsg2.Replace("$1", m_strMode);

            textMessage.Text = szMainMsg1;

            //comboType1.Location = new Point(110, 42);
            //comboType3.Location = new Point(300, 42);
            //comboType2.Visible = false;

            cboBoxDisaster.SelectedIndex = 0;
            cboBoxNumber.SelectedIndex = 0;

            comboType1.SelectedIndex = 0;
            comboType2.SelectedIndex = 0;
            comboType3.SelectedIndex = 0;
            groupBox2.Hide();
        }

        private string GetTime()
        {
            DateTime dtNow = DateTime.Now;   // 현재 날짜, 시간 얻기
            string szTime = dtNow.Year.ToString() + ("년") +
                dtNow.Month.ToString() + ("월") +
                dtNow.Day.ToString() + ("일") +
                dtNow.Hour.ToString() + ("시") +
                dtNow.Minute.ToString() + ("분 ");

            return szTime;
        }

        private void AdjustLocation(Form parent)
        {
            Size size = parent.Size;
            Point p = parent.Location;
            int x = p.X + (size.Width / 2) - (this.Size.Width / 2);
            int y = p.Y + (size.Height / 2) - (this.Size.Height / 2);
            this.Location = new Point(x, y);
        }

        public void RealMode(bool isReal)
        {
            if (isReal)
                m_strMode = "실제 비상상황";
        }

        private void cboBoxNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            string szText = combo.SelectedItem.ToString();
            if (szText == null || szText == "")
                return;

            if (szText == "상황 종료시까지 무한 반복")
            {
                nAnnoCount = -1;
            }
            else
            {
                nAnnoCount = int.Parse(szText);
            }
        }

        private void cboBoxDisaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            Debug.WriteLine(textMessage.Size.ToString());
            

            switch(cbo.SelectedIndex)
            {
                case 0:     
                    comboType1.Visible = true;
                    comboType1.Location = new Point(44, 42);
                    comboType2.Visible = true;
                    comboType2.Location = new Point(246, 42);
                    comboType3.Visible = false;
                    textMessage.Enabled = true;
                    textMessage.Text = szMainMsg0;
                    textMessage.ReadOnly = true;
                    textMessage.BackColor = Control.DefaultBackColor;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 1:
                    comboType1.Visible = true;
                    comboType1.Location = new Point(110, 42);
                    comboType2.Visible = false;
                    comboType3.Visible = true;
                    comboType3.Location = new Point(371, 42);
                    textMessage.Text = szMainMsg1;
                    textMessage.Enabled = true;
                    textMessage.ReadOnly = true;
                    textMessage.BackColor = Control.DefaultBackColor;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 2:
                    comboType1.Visible = true;
                    comboType1.Location = new Point(114, 24);
                    comboType2.Visible = false;
                    comboType3.Visible = false;
                    textMessage.Text = szMainMsg2;
                    textMessage.Enabled = true;
                    textMessage.ReadOnly = true;
                    textMessage.BackColor = Control.DefaultBackColor;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 3: // 시스템 제공
                    comboType1.Visible = false;
                    comboType2.Visible = false;
                    comboType3.Visible = false;
                    textMessage.ReadOnly = true;
                    textMessage.Enabled = false;
                    textMessage.Text = szSystemMsg;
                    textMessage.BackColor = Control.DefaultBackColor;
                    bUseSystemMsg = true;
                    m_useSenarioMessage = false;
                    break;
                case 4: // 사용자 정의
                    comboType1.Visible = false;
                    comboType2.Visible = false;
                    comboType3.Visible = false;
                    textMessage.Enabled = true;
                    textMessage.Text = "";
                    textMessage.ReadOnly = false;
                    textMessage.BackColor = Color.White;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 5: // 시나리오
                    comboType1.Visible = false;
                    comboType2.Visible = false;
                    comboType3.Visible = false;
                    textMessage.Enabled = true;                    
                    textMessage.Text = m_szSenarioMessage;
                    textMessage.ReadOnly = false;
                    textMessage.BackColor = Color.White;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = true;
                    break;
            }
        }       

        private void btnOK_Click(object sender, EventArgs e)
        {
            if( SetBrodcast())
				this.DialogResult = DialogResult.OK;
           
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {       
     
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (btnNext.Text == "마침")
            {
                groupBox1.Show();
                groupBox2.Hide();

                btnNext.Text = "다음>";
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (btnNext.Text == "다음>")
            {
                groupBox1.Hide();
                groupBox2.Show();

                btnNext.Text = "마침";
            }
            else if (btnNext.Text == "마침")
            {
                if( SetBrodcast())
					DialogResult = DialogResult.OK;
            }
        }

        private bool SetBrodcast()
        {
            m_useSiren = checkBoxSiren.Checked;

            if (cboBoxNumber.SelectedIndex == -1)
                return false;
             
            if (bUseSystemMsg == true)
            {
                szMessage = szSystemMsg;
            }
            else
            {
                if (cboBoxDisaster.SelectedIndex == -1)
                    return false;
                if (comboType1.SelectedIndex == -1 || comboType2.SelectedIndex == -1 || comboType3.SelectedIndex == -1)
                    return false;

                string szText1 = comboType1.SelectedItem.ToString();
                string szText2 = comboType2.SelectedItem.ToString();
                string szText3 = comboType3.SelectedItem.ToString();

                int nSelect = cboBoxDisaster.SelectedIndex;
                string szText = textMessage.Text;
                if (nSelect != 4 || nSelect != 5)
                {
                    szMessage = szText.Replace("$1", m_strMode).Replace("(==1==)", szText1).Replace("(==2==)", szText2).Replace("(==3==)", szText3);
                }
                else
                {
                    szMessage = szText;
                }
            }
			return true;
        }
        public string GetMessage()
        {
            return textBox1.Text;
        }
        public void SetData(Sections.SectionState sectionState)
        {
            mSection = sectionState.Section;
            //this.Text = "외부상황전파 - " + mSection.Data.Title;
            btnBack.Visible = btnNext.Visible = btnCancel2.Visible = false;
            if (mSection.GetComponentType() == Sections.Section.ComponentType.TRANSMISSION)
            {
                btnBack.Visible = btnNext.Visible = btnCancel2.Visible = true;
                Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)mSection.Data;

                if (sectionData.DataExternal.UseSMS)
                {
                    ArrayList list = sectionData.DataExternal.SMSReceivers;
                    foreach (Sections.ExternalTeamData data in list)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        cell1.Value = data.TeamName;
                        row.Cells.Add(cell1);
                        DataGridViewCell cell2 = new DataGridViewTextBoxCell();
                        cell2.Value = data.PhoneNumber;
                        row.Cells.Add(cell2);
                        dataGridViewSMS.Rows.Add(row);
                    }
                    textBox1.Text = sectionData.DataExternal.SMSMessage;
                    checkUseSMS.Checked = true;
                }

                if (sectionData.DataExternal.UseFax)
                {
                    ArrayList list = sectionData.DataExternal.FaxReceivers;
                    foreach (Sections.ExternalTeamData data in list)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                        cell1.Value = data.TeamName;
                        row.Cells.Add(cell1);
                        DataGridViewCell cell2 = new DataGridViewTextBoxCell();
                        cell2.Value = data.PhoneNumber;
                        row.Cells.Add(cell2);
                        dataGridViewFax.Rows.Add(row);
                    }
                    textBox1.Text = sectionData.DataExternal.SMSMessage;
                }
            }
        }
    }
}
