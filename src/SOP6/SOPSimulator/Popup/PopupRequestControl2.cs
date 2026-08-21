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
    public partial class PopupRequestControl : Form, UnE.GUI.IImageButtonOwner
    {
        //private int m_nStrongUserID = -1;

        public PopupRequestControl()//int nSOPGenUserID, int nSOPGeunUserLevel)
        {
            InitializeComponent();

            this.AllowTransparency = true;
            this.Opacity = 1.0;
            this.TransparencyKey = this.BackColor;

            InitButtons();

            //ControlChecked(nSOPGenUserID, nSOPGeunUserLevel);
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove;

        private void PopupRequestControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupRequestControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void PopupRequestControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }
        #endregion

        private void InitButtons()
        {
            btnOK.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.PopupRequestControl_button;
            btnOK.ImageClicked = global::SOPMonitoringSystem.Properties.Resources.PopupRequestControl_button_clicked;

            btnCancel.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.PopupRequestControl_button;
            btnCancel.ImageClicked = global::SOPMonitoringSystem.Properties.Resources.PopupRequestControl_button_clicked;

            btnOK.Owner = this;
            btnCancel.Owner = this;
        }

        public void OnImageButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnImageButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (sender == btnOK)
            {
                DataGridViewRow trgRow = null;

                foreach (DataGridViewCell cell in gridRequest.SelectedCells)
                {
                    trgRow = gridRequest.Rows[cell.RowIndex];
                    //FormMain.Instance.ChangeUserID = (int)row.Tag;
                    break;
                }

                if (trgRow == null)
                {
                    MessageBox.Show("제어권을 인계할 대상을 지정해 주세요");
                }
                else
                {
                    FormSOP.Instance.NetworkManager.SendControl(GetUserIDs((string)trgRow.Tag), trgRow.Cells[0].Tag.ToString());
                    FormSOP.Instance.ClearRequestControl();
                    FormSOP.Instance.SetControl(false);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else if (sender == btnCancel)
            {
                foreach (DataGridViewRow row in gridRequest.Rows)
                {
                    FormSOP.Instance.NetworkManager.SendRejectRequestControl((string)row.Tag, row.Cells[0].Tag.ToString());
                }

                FormSOP.Instance.ClearRequestControl();

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        // strUserID : 제어권을 가지게될 계정 ID
        // 리턴값 : strUserID를 List의 제일 처음에 넣고 나머지 계정의 ID들을 그 뒤에 넣는다.
        private List<string> GetUserIDs(string strUserID)
        {
            List<string> userIDs = new List<string>();

            userIDs.Add(strUserID);

            foreach (DataGridViewRow row in gridRequest.Rows)
            {
                if (row.Tag != null && row.Tag is string)
                {
                    if ((string)row.Tag == strUserID)
                        continue;
                    else
                        userIDs.Add((string)row.Tag);
                }
            }

            return userIDs;
        }

        public void AddUser(string strUserID, string strUserNickName, string strIP)
        {
            lock (this)
            {
                foreach (DataGridViewRow row in gridRequest.Rows)
                {
                    string strID = (string)row.Tag;

                    if (strID == strUserID)
                        return;
                }

                DataGridViewRow gridRow = new DataGridViewRow();
                gridRow.Tag = strUserID;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strUserID;
                gridRow.Cells.Add(cell);
                cell.Tag = strIP;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strUserNickName;
                gridRow.Cells.Add(cell);
                
                cell = new DataGridViewTextBoxCell();
                cell.Value = DateTime.Now;
                gridRow.Cells.Add(cell);

                gridRequest.Rows.Add(gridRow);
            }
        }

        public int RemoveUser(string strUserID)
        {
            lock (this)
            {
                foreach (DataGridViewRow row in gridRequest.Rows)
                {
                    string strID = (string)row.Tag;

                    if (strID == strUserID)
                    {
                        gridRequest.Rows.Remove(row);
                        break;
                    }
                }

                return gridRequest.Rows.Count;
            }
        }

        /*public bool HasData()
        {
            return gridRequest.Rows.Count > 0;
        }

        private void ControlChecked(int nSOPGenUserID, int nSOPGeunUserLevel)
        {
            gridRequest.Rows.Clear();
            ArrayList arrRequests = FormMain.Instance.GetRequestControl();

            int nMaxLevelUserID = -1;
            int nMaxLevel = -1;

            foreach (ControlCheck data in arrRequests)
            {
                if (data.UserID == nSOPGenUserID)
                    continue;

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cell.Value = data.MemberID;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cell.Value = data.MemberName;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cell.Value = data.Time;
                gridRow.Cells.Add(cell);

                gridRow.Tag = data.UserID;

                gridRequest.Rows.Add(gridRow);

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

        // 제어권 요청을 한 User 가운데 현재 제어권 가진 User보다 높은 레벨의 User
        public int StrongUserID
        {
            get { return m_nStrongUserID; }
        }*/

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

        private void PopupRequestControl_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(FormFrame.Instance.Location.X + this.Location.X, FormFrame.Instance.Location.Y + this.Location.Y);

            timer1.Interval = 500;
            timer1.Enabled = true;
            m_nNotiyCount = 0;
        }

        private void PopupRequestControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        public void CancelForm()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
