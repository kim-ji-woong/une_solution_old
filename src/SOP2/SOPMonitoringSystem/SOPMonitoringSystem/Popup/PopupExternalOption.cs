using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sections;
using System.Diagnostics;

namespace SOPMonitoringSystem
{
    public partial class PopupExternalOption : Form
    {
        private Section mSection = null;
        // 외부 팀 이름, 전화번호
        private Dictionary<string, string> m_dicExternalTeamPhoneNumber = new Dictionary<string, string>();
        // 외부 팀 이름, Fax 번호
        private Dictionary<string, string> m_dicExternalTeamFaxNumber = new Dictionary<string, string>();

        public PopupExternalOption()
        {            
            InitializeComponent();
            AdjustLocation(FormMain.Instance);
            
        }
        private void AdjustLocation(Form parent)
        {
            Size size = parent.Size;
            Point p = parent.Location;
            int x = p.X + (size.Width / 2) - (this.Size.Width / 2);
            int y = p.Y + (size.Height / 2) - (this.Size.Height / 2);
            this.Location = new Point(x, y);
        }

        public void SetData(SectionState  sectionState)
        {
            mSection = sectionState.Section;
            this.Text = "외부상황전파 - " + mSection.Data.Title;

            if (mSection.GetComponentType() == Sections.Section.ComponentType.TRANSMISSION)
            {
                SectionDataTransmission sectionData = (SectionDataTransmission)mSection.Data;

                if (sectionData.DataExternal.UseSMS)
                {
                    ArrayList list = sectionData.DataExternal.SMSReceivers;
                    foreach (ExternalTeamData data in list)
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
                    foreach (ExternalTeamData data in list)
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
            else
            {
                SectionDataExternal external = (SectionDataExternal)mSection.Data;
                if (external.UseSMS)
                {
                    ArrayList list = external.SMSReceivers;
                    foreach (ExternalTeamData data in list)
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
                    textBox1.Text = external.SMSMessage;
                    checkUseSMS.Checked = true;
                }

                if (external.UseFax)
                {
                    ArrayList list = external.FaxReceivers;
                    foreach (ExternalTeamData data in list)
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
                    textBox1.Text = external.SMSMessage;
                }
            }
        }

        public string GetMessage()
        {
            return textBox1.Text;
        }

        private void runBtnClick(object sender, EventArgs e)
        {
            //if (textBox1.Text == "")
            //    return;
            m_dicExternalTeamFaxNumber.Clear();
            m_dicExternalTeamPhoneNumber.Clear();

            GetPhoneNumber(dataGridViewSMS, m_dicExternalTeamPhoneNumber);
            GetPhoneNumber(dataGridViewFax, m_dicExternalTeamFaxNumber);

            this.DialogResult = DialogResult.OK;
        }

        private void GetPhoneNumber(DataGridView grid, Dictionary<string, string> dicPhoneNumber)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                string strTeamName = (string)row.Cells[0].Value;
                string strPhoneNumber = (string)row.Cells[1].Value;

                if (strTeamName == null || strPhoneNumber == null)
                    continue;

                if (strTeamName.Length == 0 || strPhoneNumber.Length == 0)
                    continue;

                bool isValid;
                strPhoneNumber = WebDBManager.ValidPhoneNumber(strPhoneNumber, out isValid);
                if (!isValid)
                    continue;

                dicPhoneNumber[strTeamName] = strPhoneNumber;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public Dictionary<string, string> ExternalTeamPhoneNumbers
        {
            get { return m_dicExternalTeamPhoneNumber; }
        }

        public Dictionary<string, string> ExternalTeamFaxNumbers
        {
            get { return m_dicExternalTeamFaxNumber; }
        }
    }
}
