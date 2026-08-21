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
    public partial class PopupGeneral : Form, AnnounceMessage
    {
        private Sections.Section mSection = null;
        //private string m_strMode = "훈련 비상상황";
        private string szMessage = "";
        string AnnounceMessage.Message
        {
            get { return szMessage; }
            set { szMessage = value; }
        }
        int nAnnoCount = 1;
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
                    m_comboDisaster.SelectedIndex = 1;
                }
            }
        }
        
      
        public PopupGeneral(bool bVirtual)
            : base()
        {
            InitializeComponent();
            RealMode(!bVirtual);
            AdjustLocation(FormMain.Instance);

            textMessage.Text = "";
            textMessage.Enabled = true;
            textMessage.ReadOnly = true;
            cmbAnnoCount.SelectedIndex = 0;
            m_comboDisaster.SelectedIndex = 0;

            textMessage.Enabled = true;
            textMessage.Text = "";
            textMessage.ReadOnly = false;
            textMessage.BackColor = Color.White;
            bUseSystemMsg = false;

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
            //if (isReal)
             //   m_strMode = "실제 비상상황";

        }

        private void cmbAnnoCount_SelectedIndexChanged(object sender, EventArgs e)
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
                try
                {
                    nAnnoCount = int.Parse(szText);
                }
                catch (System.FormatException)
                {
                    nAnnoCount = -1;
                }
            }
        }

        private void m_comboDisaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            switch (cbo.SelectedIndex)
            {
                case 0:
                    textMessage.Enabled = true;
                    textMessage.Text = "";
                    textMessage.ReadOnly = false;
                    textMessage.BackColor = Color.White;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = false;
                    break;
                case 1:
                    textMessage.Enabled = true;
                    textMessage.Text = m_szSenarioMessage;
                    textMessage.ReadOnly = false;
                    bUseSystemMsg = false;
                    m_useSenarioMessage = true;
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if( SetBrodcast())
				DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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

        private bool SetBrodcast()
        {
            m_useSiren = checkBoxSiren.Checked;

            if (cmbAnnoCount.SelectedIndex == -1)
                return false;

            if (m_comboDisaster.SelectedIndex == -1)
                return false;  
              
            szMessage = textMessage.Text;
			return false;
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
