using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class PopupSnowfall : Form, AnnounceMessage
    {
        private Sections.Section mSection = null;
        private string m_strMode = "훈련 상황";

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
                    m_comboDisaster.SelectedIndex = 3;
                }
            }
        }

        private string szTime = "";
        private string szWarn = "주의보";
        private string szAmount = "5";
       
        private string szMainText =
            "본부 재난안전대책본부에서 $1을 알려드립니다.\n\n" +
            "$2 현재 우리본부 인근지역에 적설량      $3  cm\n" +
            "예상되는 대설    $4      가 발효되었습니다.\n\n" +
            "전 직원께서는 폭설예보에 대비한 조치사항을 숙지하시어 분담업무를 수행하시기 바라며, " +
            "출퇴근길에는 가능한 회사에서 제공한 대형버스를 탑승해 주시고, " +
            "불가피하게 자가 승용차를 운행할 경우에는 반드시 월동 장비를 채비하시기 바라며, " +
            "결빙이 우려되는 진분계 고갯길과 남부식품 경사 길을 피하여 항공대 앞 도로로 우회하여 주시기 바랍니다.";

        public PopupSnowfall(bool bVirtual)
            : base()
        {
            InitializeComponent();
            RealMode(!bVirtual);
            AdjustLocation(FormMain.Instance);
            m_comboDisaster.SelectedIndex = 0;
            cmbRepeatCount.SelectedIndex = 0;
            cmbWarn.SelectedIndex = 0;

            szTime = GetTime();
            szMainText = szMainText.Replace("$2", szTime);
            szMainText = szMainText.Replace("$1", m_strMode);
            textMessage.Text = szMainText;
            textAmount.Text = szAmount;
            groupBox2.Hide();
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

        private void cmbWarn_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            if (combo.SelectedIndex== 0)
            {
                szWarn = "주의보";
            }
            else
            {
                szWarn = " 경보";
            }            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //if (cmbRepeatCount.SelectedIndex == -1)
            //    return;

            //if (bUseSystemMsg == true)
            //{
            //    szMessage = szSystemMsg;
            //}
            //else
            //{
            //    if (textAmount.Text == "")
            //    {
            //        return;
            //    }
            //    szAmount = textAmount.Text;                
            //    if (m_comboDisaster.SelectedIndex == -1)
            //        return;
            //    int nSelect = m_comboDisaster.SelectedIndex;

            //    if (nSelect == 1 || nSelect == 2)
            //    {
            //        szMessage = textMessage.Text;
            //    }
            //    else
            //    {
            //        szMessage = szMainText.Replace("$1", m_strMode).Replace("$3", szAmount).Replace("$4", szWarn);
            //    }
            //}
            SetBrodcast();
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
                SetBrodcast();

                DialogResult = DialogResult.OK;
            }
        }

        private void textAmount_TextChanged(object sender, EventArgs e)
        {
            szAmount = textAmount.Text;
        }

        private void m_comboDisaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            int nSelected = combo.SelectedIndex;
            switch(nSelected)
            {
                case 0:
                    cmbWarn.Visible = true;
                    textAmount.Visible = true;
                    textMessage.Enabled = true;
                    textMessage.ReadOnly = true;
                    szMessage = szMainText.Replace("$1", m_strMode).Replace("$3",  szAmount ).Replace("$4", szWarn );
                    textMessage.Text = szMainText;
                    textMessage.BackColor = Control.DefaultBackColor;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 1:
                    cmbWarn.Visible = false;
                    textAmount.Visible = false;
                    textMessage.ReadOnly = true;
                    textMessage.Enabled = false;
                    textMessage.Text = "";
                    textMessage.BackColor = Control.DefaultBackColor;
                    bUseSystemMsg = true;
                    m_useSenarioMessage = false;
                    break;
                case 2:
                    cmbWarn.Visible = false;
                    textAmount.Visible = false;
                    textMessage.Enabled = true;
                    textMessage.Text = "";
                    textMessage.ReadOnly = false;
                    textMessage.BackColor = Color.White;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 3:
                    textMessage.Enabled = true;
                    textAmount.Visible = false;
                    textMessage.Text = m_szSenarioMessage;
                    textMessage.ReadOnly = false;
                    textMessage.BackColor = Color.White;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = true;
                    break;
                default:
                    break;
            }           
        }

        private void cmbRepeatCount_SelectedIndexChanged(object sender, EventArgs e)
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
        
        private void SetBrodcast()
        {
            m_useSiren = checkBoxSiren.Checked;

            if (cmbRepeatCount.SelectedIndex == -1)
                return;

            if (bUseSystemMsg == true)
            {
                szMessage = szSystemMsg;
            }
            else
            {
                if (textAmount.Text == "")
                {
                    return;
                }
                szAmount = textAmount.Text;
                if (m_comboDisaster.SelectedIndex == -1)
                    return;
                int nSelect = m_comboDisaster.SelectedIndex;

                if (nSelect == 1 || nSelect == 2 || nSelect == 3)
                {
                    szMessage = textMessage.Text;
                }
                else
                {
                    szMessage = szMainText.Replace("$1", m_strMode).Replace("$3", szAmount).Replace("$4", szWarn);
                }
            }
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
