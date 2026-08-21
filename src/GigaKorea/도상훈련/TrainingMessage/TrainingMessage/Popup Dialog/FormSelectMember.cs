using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainingMessage.Data;

namespace TrainingMessage.Popup_Dialog
{
    public partial class FormSelectMember : Form
    {
        List<string> m_listReceiver = new List<string>();

        public FormSelectMember(string strReceiver)
        {
            InitializeComponent();

            LoadMember(strReceiver);
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadReceiver(string strReceiver)
        {
            string[] arrMember = strReceiver.Split(',');
            int nCount = arrMember.Length;

            for (int i = 0; i < nCount; i++)
            {
                string strNickName = arrMember[i].Trim();
                m_listReceiver.Add(strNickName);
            }
        }

        private void LoadMember(string strReceiver)
        {
            Dictionary<int, MemberData> dicMembers;
            dicMembers = FormMain.Instance.DataManager.Members;

            string[] arrMember = strReceiver.Split(',');
            int nCount = arrMember.Length;

            foreach (KeyValuePair<int, MemberData> pair in dicMembers)
            {
                MemberData member = pair.Value;

                int nRowIndex = gridMember.Rows.Add();
                gridMember.Rows[nRowIndex].Cells[colName.Index].Value = member.NickName;


                for (int i = 0; i < nCount; i++)
                {
                    string strNickName = arrMember[i].Trim();

                    if (member.NickName == strNickName)
                    {
                        gridMember.Rows[nRowIndex].Cells[colCheck.Index].Value = true;
                        //m_listReceiver.Add(strNickName);
                    }

                    
                }
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string strResult = "";

            foreach (string strReceiver in m_listReceiver)
            {
                if (strResult == "")
                    strResult = strReceiver;
                else
                    strResult += ", " + strReceiver;
            }

            FormMain.Instance.Receiver = strResult;
            this.DialogResult = DialogResult.Yes;
        }

        private void gridMember_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러

            // 체크 유무를 확인하여 
            if (gridMember.Columns[e.ColumnIndex].Name == "colCheck")
            {
                DataGridViewRow row = gridMember.Rows[e.RowIndex];
                string strCheck = row.Cells[colCheck.Index].Value.ToString();
                string strNickName = row.Cells[colName.Index].Value.ToString();

                if (strCheck == "True")
                {
                    m_listReceiver.Add(strNickName);
                }
                else if (strCheck == "False")
                {
                    m_listReceiver.Remove(strNickName);
                }
            }
        }

        private void gridMember_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridMember.IsCurrentCellDirty)
            {
                gridMember.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

        }
    }
}
