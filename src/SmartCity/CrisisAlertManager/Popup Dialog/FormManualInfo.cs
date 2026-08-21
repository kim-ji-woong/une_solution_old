using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisAlertManager.Popup_Dialog
{
    public partial class FormManualInfo : Form
    {
        public string ManualMember { get; set; }

        private FacilityType m_facilityType = FacilityType.NONE;
        private string m_strRiskLevel = CommonString.RiskLevel_Normal;
        private FacilityManual m_manual = null;

        public FormManualInfo(FacilityType facilityType, string strRiskLevel)
        {   // 추가 창 띄우기
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 15, 15));

            m_facilityType = facilityType;
            m_strRiskLevel = strRiskLevel;

            // UI 위치 및 사용유무 설정
            InitAddManual();
        }

        public FormManualInfo(FacilityManual manual)
        {   // 수정 창 띄우기
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 15, 15));

            m_facilityType = manual.FacilityType;
            m_strRiskLevel = manual.RiskLevel;
            m_manual = manual;

            // UI 위치 및 사용유무 설정
            InitReadManual();

            ShowManualData(manual);
        }

        private void ShowManualData(FacilityManual manual)
        {
            txtTitle.Text = manual.Title;
            txtManager.Tag = manual.Members;
            ShowManualMemberName(manual.Members);
            txtNumber.Text = manual.Number.ToString();
            txtMannual.Text = manual.Manual;

            // 단계 설정
            cmbLevel.SelectedIndex = CommonString.GetRiskLevelIndex(manual.RiskLevel) - 1;
        }

        

        private void InitAddManual()
        {
            btnOK.Visible = true;
            btnClose.Visible = true;
            btnSpread.Visible = false;
            btnModifity.Visible = false;
            btnSave.Visible = false;
            btnModifityCancle.Visible = false;

            pbManualInfo.Visible = false;
            pbManagerAdd.Visible = true;

            cmbLevel.Enabled = true;
            cmbLevel.SelectedIndex = CommonString.GetRiskLevelIndex(m_strRiskLevel) - 1;
        }

        private void InitReadManual()
        {
            btnModifity.Location = new Point(btnOK.Location.X, btnOK.Location.Y);
            btnSpread.Location = new Point(btnOK.Location.X - btnSpread.Size.Width - 25, btnOK.Location.Y);

            btnOK.Visible = false;
            btnClose.Visible = true;
            btnSpread.Visible = true;
            btnModifity.Visible = true;
            btnSave.Visible = false;
            btnModifityCancle.Visible = false;

            pbManualInfo.Visible = true;
            pbManagerAdd.Visible = false;

            txtTitle.Enabled = false;
            txtNumber.Enabled = false;
            txtMannual.Enabled = false;
            btnAddMember.Enabled = false;

            cmbLevel.Enabled = false;
        }

        private void InitModifityManual()
        {
            btnModifityCancle.Location = new Point(btnOK.Location.X, btnOK.Location.Y);
            btnSave.Location = new Point(btnModifityCancle.Location.X - btnSave.Size.Width - 25, btnOK.Location.Y);
            btnSpread.Location = new Point(btnSave.Location.X - btnSpread.Size.Width - 25, btnOK.Location.Y);

            btnOK.Visible = false;
            btnClose.Visible = true;
            btnSpread.Visible = true;
            btnModifity.Visible = false;
            btnSave.Visible = true;
            btnModifityCancle.Visible = true;

            txtTitle.Enabled = true;
            txtNumber.Enabled = true;
            txtMannual.Enabled = true;
            btnAddMember.Enabled = true;

            // 알람 단계
            cmbLevel.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnAddManual_Click(object sender, EventArgs e)
        {
            string strTitle = "";
            string strNumber = "";
            string strManager = "";
            string strManual = "";
            string strRiskLevel = "";

            int nNumber = -1;
            bool bChk = false;

            FormMessageBox msg;

            if (txtTitle.Text == null || txtTitle.Text == "")
            {
                msg = new FormMessageBox("확인", "제목이 입력되지 않았습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                txtTitle.Focus();
                return;
            }
            else if (txtNumber.Text == null || txtNumber.Text == "")
            {
                msg = new FormMessageBox("확인", "순번이 입력되지 않았습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                txtNumber.Focus();
                return;
            }

            strTitle = txtTitle.Text;
            strManager = (string)txtManager.Tag;
            strNumber = txtNumber.Text;
            strManual = txtMannual.Text;
            strRiskLevel = CommonString.GetRiskLevelString(cmbLevel.SelectedIndex + 1);

            nNumber = Int32.Parse(strNumber);

            bChk = FormMain.Instance.DataManager.InsertFacilityManual(m_facilityType, strRiskLevel, strTitle, strManager, nNumber, strManual);

            if (bChk == true)
            {
                msg = new FormMessageBox("성공", "메뉴얼 추가 되었습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                // 해당 순서 메뉴얼을 찾아 있다면 +1을 한다.
                CheckNumberManual(nNumber, strRiskLevel);

                this.DialogResult = DialogResult.Yes;
            }
            else
            {
                msg = new FormMessageBox("실패", "DB 추가가 실패하였습니다. \n관리자에게 문의 해주세요.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
            }
        }

        private void CheckNumberManual(int nNum, string strRiskLevel, int nID = -1)
        {   
            // 해당 순서 메뉴얼을 찾아 있다면 +1을 한다.
            FacilityManual manual = null;

            if (nID != -1)
                manual = FormMain.Instance.DataManager.CheckNumberManuals(m_facilityType, strRiskLevel, nNum, nID);
            else
                manual = FormMain.Instance.DataManager.CheckNumberManuals(m_facilityType, strRiskLevel, nNum);

            if (manual == null)
                return;

            if (nID != -1)
                CheckNumberManual(nNum + 1, strRiskLevel, nID);
            else
                CheckNumberManual(nNum + 1, strRiskLevel);

            manual.Number = manual.Number + 1;
            FormMain.Instance.DataManager.UpdateFacilityManual(manual.ID, manual.Title, manual.Members, manual.Number, manual.Manual, manual.RiskLevel);
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            string strManualMember = "";

            if (txtManager.Tag != null)
                strManualMember = (string)txtManager.Tag;

            FormManualMember manualMember = new FormManualMember(strManualMember, this);
            manualMember.StartPosition = FormStartPosition.CenterParent;

            if (manualMember.ShowDialog() == DialogResult.Yes)
            {
                strManualMember = "";
                txtManager.Tag = ManualMember;

                ShowManualMemberName(ManualMember);
            }
        }

        private void ShowManualMemberName(string strMember)
        {
            txtManager.Text = "";

            if (strMember == "" || strMember == null)
                return;

            Dictionary<int, DataCompanyMember> dicCompanyMembers = FormMain.Instance.DataManager.CompanyMembers;

            string strManualMember = "";
            string[] arrManualMember = strMember.Split(',');
            int nCount = arrManualMember.Length;

            for (int i = 0; i < nCount; i++)
            {
                string strMemberID = arrManualMember[i];
                strMemberID = strMemberID.Trim();

                int nMemberID = Int32.Parse(strMemberID);

                if (dicCompanyMembers.ContainsKey(nMemberID))
                {
                    if (strManualMember == "")
                        strManualMember += dicCompanyMembers[nMemberID].MemberName;
                    else
                        strManualMember += ", " + dicCompanyMembers[nMemberID].MemberName;
                }
            }

            txtManager.Text = strManualMember;
        }

        private void txtOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnModifity_Click(object sender, EventArgs e)
        {
            InitModifityManual();
        }

        private void btnModifityCancle_Click(object sender, EventArgs e)
        {
            // 경고문
            FormMessageBox msg = new FormMessageBox("행동요령 세부내용 수정 취소", "행동요령 세부내용 수정을 취소하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                // 데이터 초기화
                ShowManualData(m_manual);

                InitReadManual();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMessageBox msg;

            if (txtTitle.Text == null || txtTitle.Text == "")
            {
                msg = new FormMessageBox("확인", "제목이 입력되지 않았습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                txtTitle.Focus();
                return;
            }
            else if (txtNumber.Text == null)
            {
                msg = new FormMessageBox("확인", "순번이 입력되지 않았습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                txtNumber.Focus();
                return;
            }

            // 경고문
            msg = new FormMessageBox("행동요령 세부내용 수정 완료", "행동요령 세부내용 수정을 완료하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;

            if (msg.ShowDialog() == DialogResult.Yes)
            {
                string strTitle = "";
                string strNumber = "";
                string strManager = "";
                string strManual = "";
                string strRiskLevel = "";

                int nID = -1;
                int nNumber = -1;
                bool bChk = false;

                nID = m_manual.ID;
                strTitle = txtTitle.Text;
                strManager = (string)txtManager.Tag;
                strNumber = txtNumber.Text;
                strManual = txtMannual.Text;
                strRiskLevel = CommonString.GetRiskLevelString(cmbLevel.SelectedIndex + 1);

                nNumber = Int32.Parse(strNumber);

                bChk = FormMain.Instance.DataManager.UpdateFacilityManual(nID, strTitle, strManager, nNumber, strManual, strRiskLevel);

                if (bChk == true)
                {
                    // 해당 순서 메뉴얼을 찾아 있다면 +1을 한다.
                    CheckNumberManual(nNumber, strRiskLevel, nID);

                    this.DialogResult = DialogResult.Yes;
                }
                else
                {
                    msg = new FormMessageBox("실패", "DB 업데이트가 실패하였습니다. \n관리자에게 문의 해주세요.", MessageBoxButtons.OK);
                    msg.StartPosition = FormStartPosition.CenterParent;
                    msg.ShowDialog();
                }

            }
        }

        private void btnSpread_Click(object sender, EventArgs e)
        {
            // 담당자 번호 리스트 만들기
            if (txtManager.Tag == null || (string)txtManager.Tag == "")
            {
                FormMessageBox msg = new FormMessageBox("메시지 전송", "수신자가 없습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
                return;
            }

            string strMember = (string)txtManager.Tag;

            Dictionary<int, DataCompanyMember> dicCompanyMembers = FormMain.Instance.DataManager.CompanyMembers;

            List<string> listNumber = new List<string>();
            List<string> listName = new List<string>();
            string[] arrManualMember = strMember.Split(',');
            int nCount = arrManualMember.Length;

            for (int i = 0; i < nCount; i++)
            {
                string strMemberID = arrManualMember[i];
                strMemberID = strMemberID.Trim();

                int nMemberID = Int32.Parse(strMemberID);

                if (dicCompanyMembers.ContainsKey(nMemberID))
                {
                    if (dicCompanyMembers[nMemberID].PhoneNumber != "" && !listNumber.Contains(dicCompanyMembers[nMemberID].PhoneNumber))
                    {
                        listNumber.Add(dicCompanyMembers[nMemberID].PhoneNumber);

                        string strName = "";

                        if (dicCompanyMembers[nMemberID].Level != null)
                            strName = dicCompanyMembers[nMemberID].Level.LevelName + " " + dicCompanyMembers[nMemberID].MemberName + "(" + dicCompanyMembers[nMemberID].PhoneNumber + ")";
                        else
                            strName = dicCompanyMembers[nMemberID].MemberName + "(" + dicCompanyMembers[nMemberID].PhoneNumber + ")";

                        listName.Add(strName);
                    }
                        
                }
            }

            if (listNumber.Count == 0)
            {
                FormMessageBox msg = new FormMessageBox("메시지 전송", "수신자 번호가 없습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            string strMessage = txtMannual.Text;

            if (strMessage == null && strMessage == "")
            {
                FormMessageBox msg = new FormMessageBox("메시지 전송", "입력된 데이터가 없습니다.\n다시 한번 확인해주세요.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            // 메시지와 번호 리스트 디비에 저장
            if (FormMain.Instance.DataManager.InsertSMSSendMessage(listNumber, strMessage, m_facilityType))
            {
                string strNameList = "";

                // 기록
                foreach (string strName in listName)
                {
                    if (strNameList == "")
                        strNameList = strName.Trim();
                    else
                        strNameList += ", " + strName.Trim();
                }
                
                // 메시지 전송이력
                FormMain.Instance.DataManager.InsertSMSRecord(strNameList, strMessage, m_facilityType);

                FormMessageBox msg = new FormMessageBox("메시지 전송", "메시지 전송이 완료되었습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
