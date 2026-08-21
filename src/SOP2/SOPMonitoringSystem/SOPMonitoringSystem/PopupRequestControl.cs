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
    public partial class PopupRequestControl : Form
    {
        private int m_nStrongUserID = -1;

        public PopupRequestControl(int nSOPGenUserID, int nSOPGeunUserLevel)
        {
            InitializeComponent();
            ControlChecked(nSOPGenUserID, nSOPGeunUserLevel);
        }

        public bool HasData()
        {
            return dataGridView.Rows.Count > 0;
        }

        private void ControlChecked(int nSOPGenUserID, int nSOPGeunUserLevel)
        {
            dataGridView.Rows.Clear();
            ArrayList arrRequests = FormMain.Instance.GetRequestControl();

            int nMaxLevelUserID = -1;
            int nMaxLevel = -1;

            foreach (ControlCheck data in arrRequests)
            {
                if (data.UserID == nSOPGenUserID)
                    continue;

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = data.MemberID;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.MemberName;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.Time;
                gridRow.Cells.Add(cell);

                gridRow.Tag = data.UserID;

                dataGridView.Rows.Add(gridRow);

                if (data.UserID != nSOPGenUserID && data.UserLevel > nSOPGeunUserLevel)
                {
                    if (nMaxLevel < data.UserLevel)
                    {
                        nMaxLevelUserID = data.UserID;
                        nMaxLevel = data.UserLevel;
                    }
                }
            }

            if (nMaxLevelUserID > 0)
                m_nStrongUserID = nMaxLevelUserID;
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                FormMain.Instance.ChangeUserID = (int)row.Tag;
                break;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // 제어권 요청을 한 User 가운데 현재 제어권 가진 User보다 높은 레벨의 User
        public int StrongUserID
        {
            get { return m_nStrongUserID; }
        }

        private int m_nNotiyCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            m_nNotiyCount++;
            if (m_nNotiyCount == 1 || m_nNotiyCount == 4 || m_nNotiyCount == 7)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.Activate();
                this.WindowState = FormWindowState.Normal;
            }
            if (m_nNotiyCount == 8)
            {
                timer1.Stop();
                m_nNotiyCount = 0;
            }          
        }

        private void PopupRequestControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void PopupRequestControl_Shown(object sender, EventArgs e)
        {
            timer1.Interval = 500;
            timer1.Enabled = true;
            m_nNotiyCount = 0;
        }

        public void CancelForm()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
