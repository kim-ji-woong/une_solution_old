using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;

namespace CrisisAlertManager.CrisisAlert
{
    public partial class uFormSpread : UserControl
    {
        private FacilityType m_facilityType = FacilityType.NONE;
        private int m_nSensorID;

        Dictionary<int, DataTeam> m_dicRegularTeams = null;                                             // 소속 리스트
        Dictionary<int, DataTeam> m_dicSubTeams = null;                                                 // 부서 리스트
        Dictionary<int, DataCompanyMember> m_dicCompanyMembers = null;                                  // 팀원
        Dictionary<int, FacilityManager> m_dicCheckCompanyMembers = null;                               // 체크 팀원 리스트
        
        List<FacilityManager> m_listFacilityManagers = null;                                            // 담당자 리스트
        List<FacilityManager> m_listCheckFacilityManagers = new List<FacilityManager>();                // 체크 담당자 리스트

        List<FacilityManager> m_listAddManagers = new List<FacilityManager>();                          // 추가된 담당자 리스트
        List<FacilityManager> m_listRemoveManagers = new List<FacilityManager>();                       // 제거된 담당자 리스트

        FacilityMessage m_facilityMessage = new FacilityMessage();


        //public uFormSpread(FacilityType facility, int nID)
        public uFormSpread(FacilityType facility)
        {
            InitializeComponent();

            m_facilityType = facility;
            //m_nSensorID = nID;

            // 그리드 셀 줄바꿈 설정
            gridManager.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // 전파 대상자 불러오기
            LoadFacilityManagers();

            // 대상자 추가 그리드 표시
            LoadGridMainTeam();

            // 전파 메시지 불러오기
            LoadFacilityMessage();
        }

        private void LoadFacilityMessage()
        {
            //m_facilityMessage = FormMain.Instance.DataManager.GetFacilityMessage(m_facilityType, m_nSensorID);
            m_facilityMessage = FormMain.Instance.DataManager.GetFacilityMessage(m_facilityType);

            txtMessage.Text = m_facilityMessage.Message;
        }

        private void LoadFacilityManagers()
        {
            m_listFacilityManagers = new List<FacilityManager>();
            //m_listFacilityManagers = FormMain.Instance.DataManager.GetFacilityManager(m_facilityType, m_nSensorID);
            m_listFacilityManagers = FormMain.Instance.DataManager.GetFacilityManager(m_facilityType);

            gridManager.Rows.Clear();

            foreach (FacilityManager manager in m_listFacilityManagers)
            {
                int nRowIndex = gridManager.Rows.Add();
                string strColInfo = "";
                string strPhoneNumber = "";

                if (manager.CompanyMember != null && manager.CompanyMember.Level != null)
                {
                    strColInfo = manager.CompanyMember.Level.LevelName + " " + manager.CompanyMember.MemberName + "\n" + manager.CompanyMember.PhoneNumber;
                    strPhoneNumber = manager.CompanyMember.PhoneNumber;
                }
                else if (manager.CompanyMember != null)
                {
                    strColInfo = manager.CompanyMember.MemberName + "\n" + manager.CompanyMember.PhoneNumber;
                    strPhoneNumber = manager.CompanyMember.PhoneNumber;
                }
                else
                {
                    strColInfo = manager.Department + " " + manager.Name + "\n" + manager.PhoneNumber;
                    strPhoneNumber = manager.PhoneNumber;
                }

                gridManager.Rows[nRowIndex].Cells[colInfo.Index].Value = strColInfo;
                //gridManager.Rows[nRowIndex].Cells[colPhoneNum.Index].Value = strPhoneNumber;
                gridManager.Rows[nRowIndex].Tag = manager;
            }
        }

        private void LoadGridMainTeam()
        {
            gridMainTeam.Rows.Clear();

            m_dicRegularTeams = new Dictionary<int, DataTeam>();
            m_dicRegularTeams = FormMain.Instance.DataManager.GetMainTeams();

            foreach (KeyValuePair<int, DataTeam> item in m_dicRegularTeams)
            {
                DataTeam data = item.Value;

                int nRowIndex = gridMainTeam.Rows.Add();
                gridMainTeam.Rows[nRowIndex].Cells[colTeamName.Index].Value = data.TeamName;
                gridMainTeam.Rows[nRowIndex].Tag = data;
            }

            if (m_dicRegularTeams.Count > 0)
            {
                DataTeam data = (DataTeam)gridMainTeam.Rows[0].Tag;

                if (data.ID == -1)
                    return;

                LoadGridSubTeam(data.ID);
            }
        }

        private void LoadGridSubTeam(int nParentID)
        {
            gridSubTeam.Rows.Clear();
            lbSubTeam.Text = "";

            m_dicSubTeams = new Dictionary<int, DataTeam>();
            m_dicSubTeams = FormMain.Instance.DataManager.GetSubTeams(nParentID);

            if (m_dicSubTeams == null)
                return;

            foreach (KeyValuePair<int, DataTeam> item in m_dicSubTeams)
            {
                DataTeam data = item.Value;

                int nRowIndex = gridSubTeam.Rows.Add();
                gridSubTeam.Rows[nRowIndex].Cells[colSubTeamName.Index].Value = data.TeamName;
                gridSubTeam.Rows[nRowIndex].Tag = data;
            }

            if (m_dicSubTeams.Count > 0)
            {
                DataTeam data = (DataTeam)gridSubTeam.Rows[0].Tag;

                if (data.ID == -1)
                    return;

                lbSubTeam.Text = data.TeamName;

                LoadGridMember(data.ID);
            }
        }

        private void LoadGridMember(int nTeamID)
        {
            gridMember.Rows.Clear();
            m_dicCheckCompanyMembers = new Dictionary<int, FacilityManager>();

            m_dicCompanyMembers = new Dictionary<int, DataCompanyMember>();
            m_dicCompanyMembers = FormMain.Instance.DataManager.GetCompanyMembers(nTeamID);

            if (m_dicCompanyMembers == null)
                return;


            foreach (KeyValuePair<int, DataCompanyMember> item in m_dicCompanyMembers)
            {
                DataCompanyMember data = item.Value;

                int nRowIndex = gridMember.Rows.Add();
                
                string strName = "";
                if (data.Level == null)
                    strName = data.MemberName;
                else
                    strName = data.Level.LevelName + " " + data.MemberName;

                gridMember.Rows[nRowIndex].Cells[colName.Index].Value = strName;

                gridMember.Rows[nRowIndex].Tag = data;


                // 담당자 리스트에 포함된다면 체크, 체크된 리스트에 추가
                foreach(FacilityManager manager in m_listFacilityManagers)
                {
                    if (manager.CompanyMember != null)
                    {
                        if (data.ID == manager.CompanyMember.ID)
                        {
                            gridMember.Rows[nRowIndex].Cells[colCheck.Index].Value = true;

                            FacilityManager facilityManager = new FacilityManager();
                            facilityManager.CompanyMember = data;
                            facilityManager.FacilityType = m_facilityType;
                            //facilityManager.SensorID = m_nSensorID;

                            m_dicCheckCompanyMembers[manager.CompanyMember.ID] = manager;
                        }
                    }
                }
            }
        }


        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void gridMainTeam_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            gridSubTeam.Rows.Clear();
            gridMember.Rows.Clear();
            
            DataTeam data = new DataTeam();

            foreach (DataGridViewCell cell in gridMainTeam.SelectedCells)
            {
                if (cell.RowIndex < 0)
                    continue;

                DataGridViewRow row = gridMainTeam.Rows[cell.RowIndex];
                if (row.Tag == null)
                    continue;
                else
                {
                    data = (DataTeam)row.Tag;
                    break;
                }
            }

            if (data.ID == -1)
                return;

            LoadGridSubTeam(data.ID);
        }

        private void gridSubTeam_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTeam data = new DataTeam();
            lbSubTeam.Text = "";

            foreach (DataGridViewCell cell in gridSubTeam.SelectedCells)
            {
                if (cell.RowIndex < 0)
                    continue;

                DataGridViewRow row = gridSubTeam.Rows[cell.RowIndex];
                if (row.Tag == null)
                    continue;
                else
                {
                    data = (DataTeam)row.Tag;
                    break;
                }
            }

            if (data.ID == -1)
                return;

            lbSubTeam.Text = data.TeamName;

            LoadGridMember(data.ID);
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {   // 체크 팀원 리스트를 추가하는 방식
            if (m_dicCheckCompanyMembers.Count == 0)
                return;
            
            foreach (KeyValuePair<int, FacilityManager> pair in m_dicCheckCompanyMembers)
            {
                FacilityManager data = pair.Value;

                if (FindGridManager(data))
                    continue;

                // 그리드 표시
                int nRowIndex = gridManager.Rows.Add();
                
                string strInfo = "";
                if (data.CompanyMember.Level == null)
                    strInfo = data.CompanyMember.MemberName + "\n" + data.CompanyMember.PhoneNumber;
                else
                    strInfo = data.CompanyMember.Level.LevelName + " " + data.CompanyMember.MemberName + "\n" + data.CompanyMember.PhoneNumber;

                gridManager.Rows[nRowIndex].Cells[colInfo.Index].Value = strInfo;
                gridManager.Rows[nRowIndex].Tag = data;

                m_listFacilityManagers.Add(data);

                // 추가 또는 제거 담당자 리스트 관리
                if (m_listRemoveManagers.Contains(data))
                    m_listRemoveManagers.Remove(data);
                else
                    m_listAddManagers.Add(data);
            }
        }

        private bool FindGridManager(FacilityManager manager)
        {
            bool bRet = false;

            for (int i = 0; i < gridManager.Rows.Count; i++)
            {
                DataGridViewRow row = gridManager.Rows[i];

                if (row.Tag == null)
                    continue;

                FacilityManager data = (FacilityManager)row.Tag;

                if (data.CompanyMember != null && manager.CompanyMember != null)
                {
                    if (data.CompanyMember.ID == manager.CompanyMember.ID)
                        bRet = true;
                }
                else if (data.Name != "" && manager.Name != "")
                {
                    if (data.Name == manager.Name && data.Department == manager.Department)
                        bRet = true;
                }
            }

            return bRet;
        }

        private void gridMember_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridMember.IsCurrentCellDirty)
            {
                gridMember.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridMember_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러

            // 체크 유무를 확인하여 
            if (gridMember.Columns[e.ColumnIndex].Name == "colCheck")
            {
                DataGridViewRow row = gridMember.Rows[e.RowIndex];
                string strCheck = row.Cells[colCheck.Index].Value.ToString();

                if (strCheck == "True")
                {   // 체크시 체크 팀원 리스트에 저장
                    if (row.Tag == null)
                        return;

                    DataCompanyMember companyMember = (DataCompanyMember)row.Tag;
                    FacilityManager manager = new FacilityManager();
                    manager.CompanyMember = companyMember;
                    manager.FacilityType = m_facilityType;
                    //manager.SensorID = m_nSensorID;

                    m_dicCheckCompanyMembers[manager.CompanyMember.ID] = manager;
                }
                else if (strCheck == "False")
                {   // 체크 해제시 체크 팀원 리스트에서 제거
                    if (row.Tag == null)
                        return;

                    DataCompanyMember companyMember = (DataCompanyMember)row.Tag;
                    m_dicCheckCompanyMembers.Remove(companyMember.ID);
                }
            }
        }

        private void gridManager_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridManager.IsCurrentCellDirty)
            {
                gridManager.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridManager_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러

            // 체크 유무를 확인하여 
            if (gridManager.Columns[e.ColumnIndex].Name == "colManagerCheck")
            {
                DataGridViewRow row = gridManager.Rows[e.RowIndex];
                string strCheck = row.Cells[colManagerCheck.Index].Value.ToString();

                if (strCheck == "True")
                {   // 체크시 체크 담당자 리스트에 저장
                    if (row.Tag == null)
                        return;

                    FacilityManager manager = (FacilityManager)row.Tag;
                    m_listCheckFacilityManagers.Add(manager);
                }
                else if (strCheck == "False")
                {   // 체크 해제시 체크 담당자 리스트에서 제거
                    if (row.Tag == null)
                        return;

                    FacilityManager manager = (FacilityManager)row.Tag;
                    m_listCheckFacilityManagers.Remove(manager);
                }
                
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (m_listCheckFacilityManagers.Count == 0)
                return;

            List<FacilityManager> listCheckFacilityManagers = new List<FacilityManager>();

            foreach (FacilityManager item in m_listCheckFacilityManagers)
            {
                listCheckFacilityManagers.Add(item);
            }

            // 체크 담당자 리스트를 제거하는 방식
            foreach (FacilityManager manager in listCheckFacilityManagers)
            {
                for (int nRowIndex = 0; nRowIndex < gridManager.Rows.Count; nRowIndex++)
                {
                    if (gridManager.Rows[nRowIndex].Tag == manager)
                    {
                        // 추가 또는 제거 담당자 리스트 관리
                        if (m_listAddManagers.Contains(manager))
                            m_listAddManagers.Remove(manager);
                        else
                            m_listRemoveManagers.Add(manager);

                        // 그리드 표시
                        gridManager.Rows.RemoveAt(nRowIndex);
                        m_listFacilityManagers.Remove(manager);
                        m_listCheckFacilityManagers.Remove(manager);
                        break;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMessageBox msg;

            if (m_listAddManagers.Count == 0 && m_listRemoveManagers.Count == 0 && txtMessage.Text == m_facilityMessage.Message)
            {
                msg = new FormMessageBox("임시저장", "변경된 데이터가 없습니다.\n다시 한번 확인해주세요.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }
            
            if (SaveFacilityManager())
            {
                msg = new FormMessageBox("임시저장", "저장이 완료되었습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();

                FormMain.Instance.DataManager.ReloadFacilityManager();
                this.Visible = false;
                return;
            }
            else
            {
                msg = new FormMessageBox("임시저장", "저장이 실패하였습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }
        }

        private bool SaveFacilityManager()
        {
            foreach (FacilityManager addManager in m_listAddManagers)
            {
                if (!FormMain.Instance.DataManager.InsertFacilityManager(addManager))
                    return false;
            }

            foreach (FacilityManager removeManager in m_listRemoveManagers)
            {
                if (!FormMain.Instance.DataManager.DeleteFacilityManager(removeManager))
                    return false;
            }

            if (m_facilityMessage.Message != txtMessage.Text)
            {
                m_facilityMessage.Message = txtMessage.Text;

                if (m_facilityMessage.ID == -1)
                {
                    // 센서 메시지 추가
                    m_facilityMessage.FacilityType = m_facilityType;
                    //m_facilityMessage.SensorID = m_nSensorID;
                    FormMain.Instance.DataManager.InsertFacilityMessage(m_facilityMessage);
                }
                else
                {
                    // 센서 메시지 수정
                    FormMain.Instance.DataManager.UpdateFacilityMessage(m_facilityMessage);
                }
            }

            return true;
        }

        private void btnAddManager_Click(object sender, EventArgs e)
        {
            FormMessageBox msg;

            if (textBoxName.Text.Length == 0)
            {
                msg = new FormMessageBox("대상자 직접 입력", "이름이 입력되지 않았습니다.\n다시 한번 확인해주세요.\n[예시 : 홍길동]", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }
            else if (textBoxPhoneNumber.Text.Length == 0)
            {
                msg = new FormMessageBox("대상자 직접 입력", "연락처가 입력되지 않았습니다.\n(문자 제외 숫자만 입력)\n다시 한번 확인해주세요.\n[예시 : 01012345678]", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }



            FacilityManager manager = new FacilityManager();
            manager.FacilityType = m_facilityType;
            //manager.SensorID = m_nSensorID;
            manager.Name = textBoxName.Text;
            manager.PhoneNumber = textBoxPhoneNumber.Text;

            if (textBoxDepartment.Text.Length != 0)
            {
                manager.Department = textBoxDepartment.Text;
            }

            if (FindGridManager(manager))
                return;

            // 추가 또는 제거 담당자 리스트 관리
            if (m_listRemoveManagers.Contains(manager))
                m_listRemoveManagers.Remove(manager);
            else
                m_listAddManagers.Add(manager);

            // 그리드 표시
            int nRowIndex = gridManager.Rows.Add();
            gridManager.Rows[nRowIndex].Cells[colInfo.Index].Value = manager.Department + " " + manager.Name + "\n" + manager.PhoneNumber;
            //gridManager.Rows[nRowIndex].Cells[colPhoneNum.Index].Value = manager.PhoneNumber;
            gridManager.Rows[nRowIndex].Tag = manager;

            m_listFacilityManagers.Add(manager);
        }

        private void btnRemoveManager_Click(object sender, EventArgs e)
        {
            textBoxDepartment.Text = "";
            textBoxName.Text = "";
            textBoxPhoneNumber.Text = "";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtMessage.Text = m_facilityMessage.Message;
        }

        private void btnSMSSend_Click(object sender, EventArgs e)
        {
            List<string> listNumber = new List<string>();
            List<string> listName = new List<string>();

            // 담당자 번호 리스트 만들기
            foreach (FacilityManager manager in m_listFacilityManagers)
            {
                if (manager.CompanyMember != null && manager.CompanyMember.PhoneNumber != null && manager.CompanyMember.PhoneNumber != "")
                {
                    string strName = "";
                    string strNumber = manager.CompanyMember.PhoneNumber;

                    if (manager.CompanyMember.Level != null)
                        strName = manager.CompanyMember.Level.LevelName + " " + manager.CompanyMember.MemberName + "(" + manager.CompanyMember.PhoneNumber + ")";
                    else
                        strName = manager.CompanyMember.MemberName + "(" + manager.CompanyMember.PhoneNumber + ")";

                    if (!listNumber.Contains(strNumber))
                    {
                        listNumber.Add(strNumber);
                        listName.Add(strName);
                    }
                        
                }
                else if (manager.PhoneNumber != null && manager.PhoneNumber != "")
                {
                    string strNumber = manager.PhoneNumber;
                    string strName = manager.Department + " " + manager.Name + "(" + manager.PhoneNumber + ")";

                    if (!listNumber.Contains(strNumber))
                    {
                        listNumber.Add(strNumber);
                        listName.Add(strName);
                    }
                        
                }
            }

            if (listNumber.Count() == 0)
            {
                FormMessageBox msg = new FormMessageBox("메시지 전송", "수신자 번호가 없습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            string strMessage = txtMessage.Text;

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

                // 메시지 전송 이력
                FormMain.Instance.DataManager.InsertSMSRecord(strNameList, strMessage, m_facilityType);

                FormMessageBox msg = new FormMessageBox("메시지 전송", "메시지 전송이 완료되었습니다.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
            }
            else
            {
                FormMessageBox msg = new FormMessageBox("메시지 전송", "메시지 전송이 실패하였습니다.\n다시 시도해주세요.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
            }
        }
    }
}
