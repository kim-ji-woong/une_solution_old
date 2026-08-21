using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrisisAlertManager.Popup_Dialog.Message;
using CrisisAlertManager.Popup_Dialog;
using CrisisAlertManager.Data;

namespace CrisisAlertManager.Group
{
    public partial class uFormGroup : UserControl
    {
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<int, DataTeam> m_dicAddCityHalls = new Dictionary<int, DataTeam>();
        private Dictionary<int, DataTeam> m_dicRemoveCityHalls = new Dictionary<int, DataTeam>();
        private Dictionary<int, DataTeam> m_dicUpdateCityHalls = new Dictionary<int, DataTeam>();
        private Dictionary<int, DataTeam> m_dicCheckCityHalls = new Dictionary<int, DataTeam>();

        private Dictionary<int, DataCompanyMember> m_dicCompanyMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataCompanyMember> m_dicAddMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataCompanyMember> m_dicRemoveMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataCompanyMember> m_dicUpdateMembers = new Dictionary<int, DataCompanyMember>();

        Dictionary<int, JobLevel> m_dicJobLevels = null;

        private UEWpfControl.WpfComboBox m_cbDepartment = null;
        private UEWpfControl.WpfComboBox m_cbFacilityType = null;

        DataTeam m_selectTeam = null;               // 현재 선택된 시구청
        DataTeam m_selectDepartment = null;         // 현재 선택된 부서

        string m_selectFacilityType = null;         // 현재 선택된 타입

        // 전파대상자 선택 팝업창에서 선택값 불러오기
        public string FacilityType { get; set; }

        public bool IsSave
        {
            get { return btnSave.Enabled; }
        }

        public uFormGroup()
        {
            InitializeComponent();

            InitComboBox();
            InitRegularTeam();

            LoadGridCityHall();

            
        }

        private void InitRegularTeam()
        {
            m_dicRegularTeams = new Dictionary<int, DataTeam>();
            m_dicCompanyMembers = new Dictionary<int, DataCompanyMember>();

            Dictionary<int, DataTeam> dicRegularTeams = FormMain.Instance.DataManager.RegularTeams;
            Dictionary<int, DataCompanyMember> dicCompanyMembers = FormMain.Instance.DataManager.CompanyMembers;

            foreach (KeyValuePair<int, DataTeam> pair in dicRegularTeams)
            {
                DataTeam data = pair.Value;

                DataTeam team = new DataTeam();
                team.ID = data.ID;
                team.TeamName = data.TeamName;

                //team.ParentTeam = data.ParentTeam;
                if (data.ParentTeam != null && m_dicRegularTeams.ContainsKey(data.ParentTeam.ID))
                    team.ParentTeam = m_dicRegularTeams[data.ParentTeam.ID];

                m_dicRegularTeams[pair.Key] = team;
            }

            foreach (KeyValuePair<int, DataCompanyMember> pair in dicCompanyMembers)
            {
                DataCompanyMember data = pair.Value;

                DataCompanyMember member = new DataCompanyMember();
                member.ID = data.ID;
                member.MemberName = data.MemberName;

                //member.Team = data.Team;
                if (data.Team != null && m_dicRegularTeams.ContainsKey(data.Team.ID))
                    member.Team = m_dicRegularTeams[data.Team.ID];
                
                member.Level = data.Level;
                member.PhoneNumber = data.PhoneNumber;

                // TODO: 해당 인원에 대한 상황전파 
                string strTypes = "";
                //member.FacilityTypes = data.FacilityTypes;
                List<FacilityManager> listFacilities = FormMain.Instance.DataManager.GetFacilityManagerID(member.ID);
                foreach(FacilityManager facility in listFacilities)
                {
                    string type = "";

                    if (facility.FacilityType == Data.FacilityType.FIRE_SENSOR)
                        type = CommonString.FacilityType_Fire_Kor;
                    else if (facility.FacilityType == Data.FacilityType.FLOOD_SENSOR)
                        type = CommonString.FacilityType_Flood_Kor;
                    else if (facility.FacilityType == Data.FacilityType.HEAT_SENSOR)
                        type = CommonString.FacilityType_Heat_Kor;
                    else if (facility.FacilityType == Data.FacilityType.COLLAPSE_SENSOR)
                        type = CommonString.FacilityType_Collapse_Kor;

                    if (strTypes == "")
                        strTypes = type;
                    else
                        strTypes += ", " + type;
                }

                member.FacilityTypes = strTypes;

                m_dicCompanyMembers[pair.Key] = member;
            }
        }

        private void InitJobLevelComboBox()
        {
            m_dicJobLevels = FormMain.Instance.DataManager.JobLevels;

            foreach (KeyValuePair<int, JobLevel> pair in m_dicJobLevels)
            {
                JobLevel job = pair.Value;

                if (job.ID == -1)
                    continue;

                colJobLevel.Items.Add(job.LevelName);
            }
        }

        private void InitFacilityTypeComboBox()
        {
            m_cbFacilityType.customComboBox.Items.Add(CommonString.Group_Select);
            m_cbFacilityType.customComboBox.Items.Add(CommonString.FacilityType_Fire_Kor);
            m_cbFacilityType.customComboBox.Items.Add(CommonString.FacilityType_Flood_Kor);
            m_cbFacilityType.customComboBox.Items.Add(CommonString.FacilityType_Heat_Kor);
            m_cbFacilityType.customComboBox.Items.Add(CommonString.FacilityType_Collapse_Kor);

            m_cbFacilityType.customComboBox.SelectedIndex = 0;
        }

        private void InitComboBox()
        {
            m_cbDepartment = new UEWpfControl.WpfComboBox();
            eleDepartment.Child = m_cbDepartment;
            m_cbDepartment.customComboBox.SelectionChanged += EleDepartment_SelectionChanged;
            m_cbDepartment.SetSize(eleDepartment.Width, eleDepartment.Height);
            m_cbDepartment.customComboBox.DisplayMemberPath = "TeamName";

            m_cbFacilityType = new UEWpfControl.WpfComboBox();
            eleFacilityType.Child = m_cbFacilityType;
            //m_cbFacilityType.customComboBox.DropDownOpened += EleFireSensorComboBox_DropDownOpened;
            m_cbFacilityType.customComboBox.SelectionChanged += EleFacilityType_SelectionChanged;
            m_cbFacilityType.SetSize(eleFacilityType.Width, eleFacilityType.Height);
            //m_cbFacilityType.customComboBox.DisplayMemberPath = "Addr";

            // 그리드 콤보박스 위치 설정
            gridMember.Columns[colJobLevel.Index].CellTemplate.Style.Padding = new Padding(0, 10, 0, 10);

            InitJobLevelComboBox();
            InitFacilityTypeComboBox();

        }

        private void EleDepartment_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (m_cbDepartment.customComboBox.SelectedItem == null)
                return;

            if (gridMember.Rows.Count > 0)
                gridMember.Rows.Clear();
            
            DataTeam department = (DataTeam)m_cbDepartment.customComboBox.SelectedItem;

            LoadGridMember(department);
        }

        private void EleFacilityType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (m_cbFacilityType.customComboBox.SelectedIndex == 0)
                m_selectFacilityType = null;
            else
                m_selectFacilityType = m_cbFacilityType.customComboBox.SelectedItem.ToString();

            EleDepartment_SelectionChanged(null, null);
        }

        private void LoadGridCityHall()
        {
            gridCityHall.Rows.Clear();

            Dictionary<int, DataTeam> dicCityHalls = LoadCityHalls();

            foreach (KeyValuePair<int, DataTeam> item in dicCityHalls)
            {
                DataTeam data = item.Value;

                int nRowIndex = gridCityHall.Rows.Add();
                gridCityHall.Rows[nRowIndex].Cells[colCityHallName.Index].Value = data.TeamName;
                gridCityHall.Rows[nRowIndex].Tag = data;
            }

            // 첫번째 시구청 부서 표시하기
            if (gridCityHall.Rows.Count > 0)
            {
                DataTeam team = (DataTeam)gridCityHall.Rows[0].Tag;
                m_selectTeam = team;

                Dictionary<int, DataTeam> dicDepartments = LoadDepartments(team);

                LoadComboDepartment(dicDepartments);
                //LoadGridMember();
            }
        }

        private Dictionary<int, DataTeam> LoadCityHalls()
        {
            Dictionary<int, DataTeam> dicCityHalls = new Dictionary<int, DataTeam>();

            foreach (KeyValuePair<int, DataTeam> item in m_dicRegularTeams)
            {
                DataTeam data = item.Value;

                if (data.ParentTeam == null)
                {
                    dicCityHalls[data.ID] = data;
                }
            }

            return dicCityHalls;
        }

        private Dictionary<int, DataTeam> LoadDepartments(DataTeam team)
        {
            Dictionary<int, DataTeam> dicDepartments = new Dictionary<int, DataTeam>();

            foreach (KeyValuePair<int, DataTeam> item in m_dicRegularTeams)
            {
                DataTeam data = item.Value;

                if (data.ParentTeam == team)
                {
                    dicDepartments[data.ID] = data;
                }
            }

            return dicDepartments;
        }

        private void btnCityHallAdd_Click(object sender, EventArgs e)
        {
            // 중복검사
            string strTeamName = CheckNewTeamName();
            int nID = CheckNewTeamID();

            // 새조직 추가하기
            DataTeam team = new DataTeam();
            team.ID = nID;
            team.TeamName = strTeamName;

            m_dicRegularTeams[nID] = team;
            m_dicAddCityHalls[nID] = team;

            // 그리드 표시
            int nRowIndex = gridCityHall.Rows.Add();
            gridCityHall.Rows[nRowIndex].Cells[colCityHallName.Index].Value = team.TeamName;
            gridCityHall.Rows[nRowIndex].Tag = team;

            // 추가된 셀 선택
            gridCityHall.CurrentCell = gridCityHall.Rows[nRowIndex].Cells[colCityHallName.Index];
            gridCityHall.BeginEdit(true);

            CheckSaveState();
        }

        private string CheckNewTeamName()
        {
            int i = 1;
            bool bChk = true;
            string strTeamName = "";

            while (bChk)
            {
                strTeamName = "새조직 " + i.ToString();
                bChk = false;

                //bChk = FormMain.Instance.DataManager.CheckTempName(strTeamName);
                foreach (KeyValuePair<int, DataTeam> pair in m_dicRegularTeams)
                {
                    string strName = pair.Value.TeamName;
                    DataTeam parent = pair.Value.ParentTeam;

                    if (strName == strTeamName && parent == null)
                    {
                        bChk = true;
                        break;
                    }
                }

                i++;
            }

            return strTeamName;
        }

        private int CheckNewTeamID()
        {
            int nRet = 0;
            bool bChk = true;

            while (bChk)
            {
                nRet++;
                bChk = m_dicRegularTeams.ContainsKey(nRet);
            }

            return nRet;
        }

        private void gridCityHall_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridCityHall.IsCurrentCellDirty)
            {
                gridCityHall.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridCityHall_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridCityHall.Columns[e.ColumnIndex].Name == "colCheck")
            {
                DataGridViewRow row = gridCityHall.Rows[e.RowIndex];
                string strCheck = row.Cells[colCheck.Index].Value.ToString();

                if (strCheck == "True")
                {   // 체크시 체크 리스트에 저장
                    if (row.Tag == null)
                        return;

                    DataTeam team = (DataTeam)row.Tag;
                    m_dicCheckCityHalls[team.ID] = team;
                }
                else if (strCheck == "False")
                {   // 체크 해제시 체크 리스트에서 제거
                    if (row.Tag == null)
                        return;

                    DataTeam team = (DataTeam)row.Tag;
                    m_dicCheckCityHalls.Remove(team.ID);
                }
            }
        }

        private void btnCityHallRemove_Click(object sender, EventArgs e)
        {
            if (m_dicCheckCityHalls.Count == 0)
                return;

            Dictionary<int, DataTeam> dicCheckCityHalls = new Dictionary<int, DataTeam>();

            foreach (KeyValuePair<int, DataTeam> pair in m_dicCheckCityHalls)
            {
                int nID = pair.Key;
                DataTeam team = pair.Value;

                dicCheckCityHalls[nID] = team;
            }

            foreach (KeyValuePair<int, DataTeam> pair in dicCheckCityHalls)
            {
                int nID = pair.Key;
                DataTeam team = pair.Value;

                for (int nRowIndex = 0; nRowIndex < gridCityHall.Rows.Count; nRowIndex++)
                {
                    if (gridCityHall.Rows[nRowIndex].Tag == team)
                    {
                        // 해당 부서 및 인원 삭제
                        RemoveCityHall(team);

                        // 추가 또는 제거 리스트 관리
                        if (m_dicAddCityHalls.ContainsValue(team))
                            m_dicAddCityHalls.Remove(nID);
                        else
                            m_dicRemoveCityHalls[nID] = team;

                        // 그리드 표시
                        gridCityHall.Rows.RemoveAt(nRowIndex);
                        m_dicRegularTeams.Remove(nID);
                        m_dicCheckCityHalls.Remove(nID);
                        break;
                    }
                }
            }

            CheckSaveState();
        }

        private void RemoveCityHall(DataTeam cityHall)
        {
            Dictionary<int, DataTeam> dicRegularTeams = new Dictionary<int, DataTeam>();
            Dictionary<int, DataCompanyMember> dicCompanyMembers = new Dictionary<int, DataCompanyMember>();

            foreach (KeyValuePair<int, DataTeam> pair in m_dicRegularTeams)
            {
                int nID = pair.Key;
                DataTeam team = pair.Value;

                dicRegularTeams[nID] = team;
            }

            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicCompanyMembers)
            {
                int nID = pair.Key;
                DataCompanyMember Member = pair.Value;

                dicCompanyMembers[nID] = Member;
            }

            // 시구청 해당하는 부서 체크
            foreach (KeyValuePair<int, DataTeam> pair in dicRegularTeams)
            {
                DataTeam data = pair.Value;

                if (data.ParentTeam != null && data.ParentTeam.ID == cityHall.ID)
                {
                    // 부서에 해당하는 인원 체크
                    foreach (KeyValuePair<int, DataCompanyMember> pair2 in dicCompanyMembers)
                    {
                        DataCompanyMember dataMember = pair2.Value;

                        // 해당 인원 삭제
                        if (dataMember.Team.ID == data.ID)
                        {
                            if (m_dicAddMembers.ContainsKey(dataMember.ID))
                                m_dicAddMembers.Remove(dataMember.ID);
                            else
                            {
                                m_dicRemoveMembers[dataMember.ID] = dataMember;

                                if (m_dicUpdateMembers.ContainsKey(dataMember.ID))
                                    m_dicUpdateMembers.Remove(dataMember.ID);
                            }

                            m_dicCompanyMembers.Remove(dataMember.ID);
                        }
                    }

                    // 해당 부서 삭제
                    if (m_dicAddCityHalls.ContainsKey(data.ID))
                        m_dicAddCityHalls.Remove(data.ID);
                    else
                    {
                        m_dicRemoveCityHalls[data.ID] = data;

                        if (m_dicUpdateCityHalls.ContainsKey(data.ID))
                            m_dicUpdateCityHalls.Remove(data.ID);
                    }

                    m_dicRegularTeams.Remove(data.ID);
                }
            }
        }

        private void gridCityHall_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = gridCityHall.Rows[e.RowIndex];
            if (row.Tag == null)
                return;

            DataTeam team = (DataTeam)row.Tag;
            m_selectTeam = team;

            Dictionary<int, DataTeam> dicDepartments = new Dictionary<int, DataTeam>();
            dicDepartments = LoadDepartments(team);

            // 해당 부서 드롭박스 표시
            LoadComboDepartment(dicDepartments);

        }

        private void LoadComboDepartment(Dictionary<int, DataTeam> dicDepartments)
        {
            m_cbDepartment.customComboBox.Items.Clear();
            //colDepartment.Items.Clear();

            DataTeam temp = new DataTeam();
            temp.TeamName = CommonString.Group_Select;

            m_cbDepartment.customComboBox.Items.Add(temp);

            foreach (KeyValuePair<int, DataTeam> pair in dicDepartments)
            {
                DataTeam team = pair.Value;
                m_cbDepartment.customComboBox.Items.Add(team);
                //colDepartment.Items.Add(team.TeamName);
            }

            m_cbDepartment.customComboBox.SelectedIndex = 0;
        }

        private void LoadGridMember(DataTeam department)
        {
            if (department.TeamName == CommonString.Group_Select)
            {
                Dictionary<int, DataTeam> dicDepartments = new Dictionary<int, DataTeam>();
                dicDepartments = LoadDepartments(m_selectTeam);

                foreach (KeyValuePair<int, DataTeam> pair in dicDepartments)
                {
                    DataTeam team = pair.Value;
                    Dictionary<int, DataCompanyMember> dicMember = LoadMember(team);

                    ShowGridMember(dicMember);
                }

                gridMember.Columns[colDepartment.Index].ReadOnly = false;
            }
            else
            {
                Dictionary<int, DataCompanyMember> dicMember = LoadMember(department);

                ShowGridMember(dicMember);

                gridMember.Columns[colDepartment.Index].ReadOnly = true;
            }

            if (gridMember.Rows.Count > 0)
            {
                string strDepartment = (string)gridMember.Rows[0].Cells[colDepartment.Index].Value;
                m_selectDepartment = CheckDepartmentName(strDepartment);
            }
        }

        private Dictionary<int, DataCompanyMember> LoadMember(DataTeam team)
        {
            Dictionary<int, DataCompanyMember> dicMember = new Dictionary<int, DataCompanyMember>();

            foreach (KeyValuePair<int, DataCompanyMember> item in m_dicCompanyMembers)
            {
                DataCompanyMember data = item.Value;

                if (data.Team == team)
                {
                    dicMember[data.ID] = data;
                }
            }

            return dicMember;
        }

        private void ShowGridMember(Dictionary<int, DataCompanyMember> dicMember)
        {
            foreach (KeyValuePair<int, DataCompanyMember> pair in dicMember)
            {
                DataCompanyMember member = pair.Value;

                // 상황전파 대상자 선택 체크
                if (m_selectFacilityType == null || member.FacilityTypes.Contains(m_selectFacilityType))
                {
                    // 그리드 표시
                    AddGridMember(member);
                }
                
            }
        }

        private void gridMember_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 셀 클릭시 선택된 부서 체크
            m_selectDepartment = null;

            string strDepartment = (string)gridMember.Rows[e.RowIndex].Cells[colDepartment.Index].Value;
            string strFacilityType = (string)gridMember.Rows[e.RowIndex].Cells[colFacilityType.Index].Value;
            m_selectDepartment = CheckDepartmentName(strDepartment);
           
            // 기본 상황전파 대상자 선택
            if (e.ColumnIndex == colSelectFacilityType.Index)
            {
                FormSelectFacilityType msg = new FormSelectFacilityType(strFacilityType, this);
                msg.StartPosition = FormStartPosition.CenterParent;
                
                if (msg.ShowDialog() == DialogResult.Yes)
                {
                    gridMember.Rows[e.RowIndex].Cells[colFacilityType.Index].Value = FacilityType;

                    if (gridMember.Rows[e.RowIndex].Tag != null)
                    {
                        DataCompanyMember data = (DataCompanyMember)gridMember.Rows[e.RowIndex].Tag;
                        data.FacilityTypes = FacilityType;

                        // 수정 시 기존 멤버이라면 업데이트 된 멤버로 추가
                        if (!m_dicAddMembers.ContainsKey(data.ID) && m_dicCompanyMembers.ContainsKey(data.ID))
                        {
                            if (!m_dicUpdateMembers.ContainsKey(data.ID))
                                m_dicUpdateMembers[data.ID] = data;
                        }

                        CheckSaveState();
                    }
                        
                }
            }

            // 부서 입력을 콤보 박스로 사용할 경우 <미완성>
            // 드랍박스 입력 방식으로 사용하기 위한 작업
            //if (e.ColumnIndex == colDepartment.Index)
            //{
            //    gridMember.CurrentCell = gridMember.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //    gridMember.BeginEdit(true);

            //    ComboBox comboBox = (ComboBox)gridMember.EditingControl;
            //    comboBox.DropDownStyle = ComboBoxStyle.DropDown;

            //    comboBox.Leave += new EventHandler(cellComboBox_Leave);
            //    comboBox.Tag = true;
            //}
        }

        // 부서 입력을 콤보 박스로 사용할 경우 <미완성>
        //private void cellComboBox_Leave(object sender, EventArgs e)
        //{
        //   ComboBox cbo = (ComboBox)sender;

        //    DataTeam department = null;

        //    if (cbo.Tag == null)
        //        return;

        //    DataGridViewCell cell = gridMember.SelectedCells[0];

        //    string strDepartment = cbo.Text;
        //    department = CheckDepartmentName(strDepartment);

        //    if (department == null)
        //    {
        //        // 새로운 부서일 경우 부서 생성
        //        department = new DataTeam();
        //        int nID = CheckNewTeamID();

        //        department.ID = nID;
        //        department.ParentTeam = m_selectTeam;
        //        department.TeamName = strDepartment;

        //        m_dicRegularTeams[nID] = department;
        //        m_dicAddCityHalls[nID] = department;

        //        cbo.Items.Add(strDepartment);
        //        colDepartment.Items.Add(strDepartment);
        //        m_cbDepartment.customComboBox.Items.Add(department);
        //    }

        //    // 변경한 값으로 내용 변경
        //    gridMember.Rows[cell.RowIndex].Cells[colDepartment.Index].Value = department.TeamName;
        //}

        private void btnMemberAdd_Click(object sender, EventArgs e)
        {
            // 현재 선택된 부서 체크
            DataTeam department = (DataTeam)m_cbDepartment.customComboBox.SelectedItem;

            // 중복검사
            string strMemberName = CheckNewMemberName(department);
            DataTeam team = CheckDepartment(department);
            
            int nID = MaxMemberID();
            nID++;

            // 새 사용자 추가
            DataCompanyMember member = new DataCompanyMember();
            member.ID = nID;
            member.MemberName = strMemberName;
            member.Team = team;

            m_dicCompanyMembers[nID] = member;
            m_dicAddMembers[nID] = member;

            // 그리드 표시
            AddGridMember(member);

            // 추가된 셀 포커스 이동(이름)
            int nRowIndex = gridMember.Rows.Count;
            gridMember.CurrentCell = gridMember.Rows[nRowIndex - 1].Cells[colName.Index];
            gridMember.BeginEdit(true);

            CheckSaveState();

        }

        private void AddGridMember(DataCompanyMember member)
        {
            int nRowIndex = gridMember.Rows.Add();
            gridMember.Rows[nRowIndex].Cells[colNo.Index].Value = nRowIndex + 1;
            gridMember.Rows[nRowIndex].Cells[colCityHall.Index].Value = m_selectTeam.TeamName;
            gridMember.Rows[nRowIndex].Cells[colDepartment.Index].Value = member.Team.TeamName;

            if (member.Level != null)
                gridMember.Rows[nRowIndex].Cells[colJobLevel.Index].Value = member.Level.LevelName;

            gridMember.Rows[nRowIndex].Cells[colName.Index].Value = member.MemberName;
            gridMember.Rows[nRowIndex].Cells[colPhoneNum.Index].Value = member.PhoneNumber;
            gridMember.Rows[nRowIndex].Cells[colFacilityType.Index].Value = member.FacilityTypes;
            gridMember.Rows[nRowIndex].Tag = member;

            if (nRowIndex == 0)
                m_selectDepartment = member.Team;
        }

        private string CheckNewMemberName(DataTeam department)
        {
            int i = 1;
            bool bChk = true;
            string strMemberName = "";

            while (bChk)
            {
                strMemberName = "새인원 " + i.ToString();
                bChk = false;

                if (department.TeamName == CommonString.Group_Select)
                {
                    Dictionary<int, DataTeam> dicDepartments = new Dictionary<int, DataTeam>();
                    dicDepartments = LoadDepartments(m_selectTeam);

                    foreach (KeyValuePair<int, DataTeam> pair in dicDepartments)
                    {
                        DataTeam team = pair.Value;

                        foreach (KeyValuePair<int, DataCompanyMember> pair2 in m_dicCompanyMembers)
                        {
                            string strName = pair2.Value.MemberName;

                            if (strName == strMemberName && pair2.Value.Team == team)
                            {
                                bChk = true;
                                break;
                            }
                        }

                    }
                }
                else
                {
                    foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicCompanyMembers)
                    {
                        string strName = pair.Value.MemberName;

                        if (strName == strMemberName && department == pair.Value.Team)
                        {
                            bChk = true;
                            break;
                        }
                    }
                }
               
                i++;
            }

            return strMemberName;
        }

        private int MaxMemberID()
        {
            int nMax = 0;
            bool bChk = true;

            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicCompanyMembers)
            {
                //nRet++;
                //bChk = m_dicCompanyMembers.ContainsKey(nRet);

                int nID = pair.Key;

                if (nMax < nID)
                    nMax = nID;
            }

            return nMax;
        }

        private DataTeam CheckDepartment(DataTeam department)
        {
            if (department.TeamName == CommonString.Group_Select)
            {
                // 선택된 시구청에 부서 확인
                Dictionary<int, DataTeam> dicDepartments = new Dictionary<int, DataTeam>();
                dicDepartments = LoadDepartments(m_selectTeam);

                // 있으면 첫번째 부서 반환
                foreach (KeyValuePair<int, DataTeam> pair in dicDepartments)
                {
                    DataTeam team = pair.Value;
                    return team;
                }

                // 없으면 새로 생성한 뒤에 반환
                int nID = CheckNewTeamID();
                string strDepartmentName = CheckNewDepartmentName(m_selectTeam);

                DataTeam newTeam = new DataTeam();
                newTeam.ID = nID;
                newTeam.TeamName = strDepartmentName;
                newTeam.ParentTeam = m_selectTeam;

                m_dicRegularTeams[nID] = newTeam;
                m_dicAddCityHalls[nID] = newTeam;

                m_cbDepartment.customComboBox.Items.Add(newTeam);

                return newTeam;
            }

            return department;
        }

        private string CheckNewDepartmentName(DataTeam department)
        {
            int i = 1;
            bool bChk = true;
            string strDepartmentName = "";

            while (bChk)
            {
                strDepartmentName = "새부서 " + i.ToString();
                bChk = false;

                foreach (KeyValuePair<int, DataTeam> pair in m_dicRegularTeams)
                {
                    string strName = pair.Value.TeamName;
                    DataTeam parent = pair.Value.ParentTeam;

                    if (strName == strDepartmentName && parent == department)
                    {
                        bChk = true;
                        break;
                    }
                }

                i++;
            }

            return strDepartmentName;
        }

        private void btnMemberRemove_Click(object sender, EventArgs e)
        {
            gridMember.BeginEdit(false);

            // 선택된 행 찾기
            DataGridViewRow row = null;
            int nRowIdx = 0;

            nRowIdx = SelectedGridMemberRow(out row);

            // 선택된 행 데이터 삭제 및 행 삭제
            if (row != null && row.Tag != null)
            {
                // 선택된 행 데이터 삭제
                DataCompanyMember member = (DataCompanyMember)row.Tag;

                m_dicCompanyMembers.Remove(member.ID);

                if (m_dicAddMembers.ContainsKey(member.ID))
                    m_dicAddMembers.Remove(member.ID);
                else
                {
                    m_dicRemoveMembers[member.ID] = member;

                    if (m_dicUpdateMembers.ContainsKey(member.ID))
                        m_dicUpdateMembers.Remove(member.ID);
                }
                    

                // 선택된 행 삭제
                gridMember.Rows.RemoveAt(nRowIdx);
            }

            // 행 삭제 후 부서 인원 확인 후 부서 삭제
            CheckRefreshDepartment();

            // 삭제 후 선택된 행의 부서 재 선택
            if (gridMember.Rows.Count > 0)
            {
                nRowIdx = SelectedGridMemberRow(out row);

                string strDepartment = (string)gridMember.Rows[nRowIdx].Cells[colDepartment.Index].Value;
                m_selectDepartment = CheckDepartmentName(strDepartment);
            }

            CheckSaveState();
        }

        private int SelectedGridMemberRow(out DataGridViewRow row)
        {
            int nRowIdx = 0;
            int nCount = gridMember.Rows.Count;

            row = null;

            for (int i = 0; i < nCount; i++)
            {
                if (gridMember.Rows[i].Selected)
                {
                    row = gridMember.Rows[i];
                    nRowIdx = i;
                    break;
                }
            }

            return nRowIdx;
        }

        private void gridMember_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (gridMember.Rows[e.RowIndex].Tag == null)
                return;

            string strLevelName = "";
            string strDepartment = "";

            DataCompanyMember member = (DataCompanyMember)gridMember.Rows[e.RowIndex].Tag;

            if (e.ColumnIndex == colName.Index)
                member.MemberName = (string)gridMember.Rows[e.RowIndex].Cells[colName.Index].Value;
            else if (e.ColumnIndex == colPhoneNum.Index)
                member.PhoneNumber = (string)gridMember.Rows[e.RowIndex].Cells[colPhoneNum.Index].Value;
            else if (e.ColumnIndex == colDepartment.Index)
            {
                DataTeam department = null;

                strDepartment = (string)gridMember.Rows[e.RowIndex].Cells[colDepartment.Index].Value;

                department = CheckDepartmentName(strDepartment);

                if (department == null)
                {
                    // 새로운 부서일 경우 부서 생성
                    department = new DataTeam();
                    int nID = CheckNewTeamID();

                    department.ID = nID;
                    department.ParentTeam = m_selectTeam;
                    department.TeamName = strDepartment;

                    m_dicRegularTeams[nID] = department;
                    m_dicAddCityHalls[nID] = department;

                    m_cbDepartment.customComboBox.Items.Add(department);
                }

                member.Team = department;

                // 변경되기 전의 부서 인원을 확인 후 0명이면 삭제
                CheckRefreshDepartment();

                // 변경된 부서로 재 선택
                m_selectDepartment = department;
            }
            else if (e.ColumnIndex == colJobLevel.Index)
            {
                JobLevel level = null;

                strLevelName = (string)gridMember.Rows[e.RowIndex].Cells[colJobLevel.Index].Value;

                level = CheckLevelName(strLevelName);

                if (level != null)
                    member.Level = level;
            }

            // 수정 시 기존 멤버이라면 업데이트 된 멤버로 추가
            if (!m_dicAddMembers.ContainsKey(member.ID) && m_dicCompanyMembers.ContainsKey(member.ID))
            {
                if (!m_dicUpdateMembers.ContainsKey(member.ID))
                    m_dicUpdateMembers[member.ID] = member;
            }

            CheckSaveState();

        }

        private void CheckRefreshDepartment()
        {   // 변경되기 전의 부서 인원을 확인 후 0명이면 삭제
            if (m_selectDepartment == null)
                return;

            int nCount = 0;

            // 변경되기 전의 부서 인원을 확인
            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicCompanyMembers)
            {
                DataCompanyMember data = pair.Value;

                if (data.Team.ID == m_selectDepartment.ID)
                    nCount++;
            }

            // 부서 인원이 0이라면 제거
            if (nCount == 0)
            {
                if (m_dicRegularTeams.ContainsKey(m_selectDepartment.ID))
                    m_dicRegularTeams.Remove(m_selectDepartment.ID);

                if (m_dicAddCityHalls.ContainsKey(m_selectDepartment.ID))
                    m_dicAddCityHalls.Remove(m_selectDepartment.ID);
                else
                    m_dicRemoveCityHalls[m_selectDepartment.ID] = m_selectDepartment;

                // 부서 선택 콤보박스 초기화
                if (m_cbDepartment.customComboBox.SelectedItem == m_selectDepartment)
                {
                    m_cbDepartment.customComboBox.Items.Remove(m_selectDepartment);
                    m_cbDepartment.customComboBox.SelectedIndex = 0;
                }
                else
                    m_cbDepartment.customComboBox.Items.Remove(m_selectDepartment);
            }
        }

        private JobLevel CheckLevelName(string strJob)
        {
            JobLevel retJob = null;

            foreach (KeyValuePair<int, JobLevel> pair in m_dicJobLevels)
            {
                JobLevel job = pair.Value;

                if (job.LevelName == strJob)
                    retJob = job;
            }

            return retJob;
        }

        private DataTeam CheckDepartmentName(string strDepartment)
        {
            DataTeam retTeam = null;

            foreach (object obj in m_cbDepartment.customComboBox.Items)
            {
                DataTeam team = (DataTeam)obj;

                //if (team.TeamName == strDepartment)
                if (strDepartment.Equals(team.TeamName))
                    retTeam = team;
            }

            //if (retTeam == null)
            //{
            //    // 새로운 부서일 경우 부서 생성
            //    DataTeam department = new DataTeam();
            //    int nID = CheckNewTeamID();

            //    department.ID = nID;
            //    department.ParentTeam = m_selectTeam;
            //    department.TeamName = strDepartment;

            //    m_dicRegularTeams[nID] = department;
            //    m_dicAddCityHalls[nID] = department;

            //    m_cbDepartment.customComboBox.Items.Add(department);
            //}

            return retTeam;
        }

        //private void ReloadDepartment()
        //{
        //    foreach (KeyValuePair<int, DataTeam> pair in m_dicRegularTeams)
        //    {
        //        DataTeam team = pair.Value;
        //        int nCount = 0;

        //        if (team.ParentTeam != null)
        //        {
        //            foreach (KeyValuePair<int, DataCompanyMember> pair2 in m_dicCompanyMembers)
        //            {
        //                DataCompanyMember member = pair2.Value;

        //                if (member.Team.ID == team.ID)
        //                {
        //                    nCount++;
        //                }
        //            }

        //            if (nCount == 0)
        //            {

        //            }
        //        }
        //    }
        //}

        private void btnRedo_Click(object sender, EventArgs e)
        {
            
        }

        private void gridCityHall_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (gridCityHall.Rows[e.RowIndex].Tag == null)
                return;

            DataTeam team = (DataTeam)gridCityHall.Rows[e.RowIndex].Tag;

            if (e.ColumnIndex == colCityHallName.Index)
            {
                string strCityHallName = (string)gridCityHall.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                team.TeamName = strCityHallName;

                if (!m_dicAddCityHalls.ContainsKey(team.ID))
                {
                    if (!m_dicUpdateCityHalls.ContainsKey(team.ID))
                        m_dicUpdateCityHalls[team.ID] = team;
                }

                CheckSaveState();
            }
        }

        private void CheckSaveState()
        {
            if (m_dicAddCityHalls.Count != 0 || m_dicRemoveCityHalls.Count != 0 || m_dicUpdateCityHalls.Count != 0 || 
                m_dicAddMembers.Count != 0 || m_dicRemoveMembers.Count != 0 || m_dicUpdateMembers.Count != 0)
            {
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 멤버 삭제
            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicRemoveMembers)
            {
                DataCompanyMember member = pair.Value;

                FormMain.Instance.DataManager.DeleteCompanyMember(member);

                // 상황전파 대상자 제거
                RemoveFacilityManager(member);
            }
            // 시구청 삭제
            foreach (KeyValuePair<int, DataTeam> pair in m_dicRemoveCityHalls)
            {
                DataTeam data = pair.Value;

                FormMain.Instance.DataManager.DeleteRegularteam(data);
            }
            // 시구청 추가
            foreach (KeyValuePair<int, DataTeam> pair in m_dicAddCityHalls)
            {
                DataTeam data = pair.Value;

                FormMain.Instance.DataManager.InsertRegularteam(data);
            }
            // 시구청 업데이트
            foreach (KeyValuePair<int, DataTeam> pair in m_dicUpdateCityHalls)
            {
                DataTeam data = pair.Value;

                FormMain.Instance.DataManager.UpdateRegularteam(data);
            }

            // 멤버 추가
            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicAddMembers)
            {
                DataCompanyMember member = pair.Value;

                FormMain.Instance.DataManager.InsertCompanyMember(member);

                // 상황전파 대상자 업데이트
                UpdateFacilityManager(member);
            }
            // 멤버 업데이트
            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicUpdateMembers)
            {
                DataCompanyMember member = pair.Value;

                FormMain.Instance.DataManager.UpdateCompanyMember(member);

                // 상황전파 대상자 업데이트
                UpdateFacilityManager(member);
            }

            // 데이터 초기화
            m_dicRemoveCityHalls = new Dictionary<int, DataTeam>();
            m_dicAddCityHalls = new Dictionary<int, DataTeam>();
            m_dicUpdateCityHalls = new Dictionary<int, DataTeam>();

            m_dicRemoveMembers = new Dictionary<int, DataCompanyMember>();
            m_dicAddMembers = new Dictionary<int, DataCompanyMember>();
            m_dicUpdateMembers = new Dictionary<int, DataCompanyMember>();

            FormMain.Instance.DataManager.LoadTeam();

            // 버튼 초기화(앞으로, 뒤로, 세이브 버튼 상태)
            CheckSaveState();
        }

        private void UpdateFacilityManager(DataCompanyMember member)
        {
            string strFacility = member.FacilityTypes;

            if (strFacility.Contains(CommonString.FacilityType_Fire_Kor))
            {
                // 등록이 되지 않았다면 등록
                if (null == FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.FIRE_SENSOR, member.ID))
                {
                    FacilityManager addManager = new FacilityManager();
                    addManager.FacilityType = Data.FacilityType.FIRE_SENSOR;
                    addManager.CompanyMember = member;

                    FormMain.Instance.DataManager.InsertFacilityManager(addManager);
                }
            }
            else
            {
                FacilityManager manager = null;
                manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.FIRE_SENSOR, member.ID);

                // 등록이 되었다면 삭제
                if (null != manager)
                    FormMain.Instance.DataManager.DeleteFacilityManager(manager);
            }


            if (strFacility.Contains(CommonString.FacilityType_Flood_Kor))
            {
                // 등록이 되지 않았다면 등록
                if (null == FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.FLOOD_SENSOR, member.ID))
                {
                    FacilityManager addManager = new FacilityManager();
                    addManager.FacilityType = Data.FacilityType.FLOOD_SENSOR;
                    addManager.CompanyMember = member;

                    FormMain.Instance.DataManager.InsertFacilityManager(addManager);
                }
            }
            else
            {
                FacilityManager manager = null;
                manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.FLOOD_SENSOR, member.ID);

                // 등록이 되었다면 삭제
                if (null != manager)
                    FormMain.Instance.DataManager.DeleteFacilityManager(manager);
            }


            if (strFacility.Contains(CommonString.FacilityType_Heat_Kor))
            {
                // 등록이 되지 않았다면 등록
                if (null == FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.HEAT_SENSOR, member.ID))
                {
                    FacilityManager addManager = new FacilityManager();
                    addManager.FacilityType = Data.FacilityType.HEAT_SENSOR;
                    addManager.CompanyMember = member;

                    FormMain.Instance.DataManager.InsertFacilityManager(addManager);
                }
            }
            else
            {
                FacilityManager manager = null;
                manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.HEAT_SENSOR, member.ID);

                // 등록이 되었다면 삭제
                if (null != manager)
                    FormMain.Instance.DataManager.DeleteFacilityManager(manager);
            }


            if (strFacility.Contains(CommonString.FacilityType_Collapse_Kor))
            {
                // 등록이 되지 않았다면 등록
                if (null == FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.COLLAPSE_SENSOR, member.ID))
                {
                    FacilityManager addManager = new FacilityManager();
                    addManager.FacilityType = Data.FacilityType.COLLAPSE_SENSOR;
                    addManager.CompanyMember = member;

                    FormMain.Instance.DataManager.InsertFacilityManager(addManager);
                }
            }
            else
            {
                FacilityManager manager = null;
                manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.COLLAPSE_SENSOR, member.ID);

                // 등록이 되었다면 삭제
                if (null != manager)
                    FormMain.Instance.DataManager.DeleteFacilityManager(manager);
            }
        }

        private void RemoveFacilityManager(DataCompanyMember member)
        {
            FacilityManager manager = null;
            manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.FIRE_SENSOR, member.ID);
            // 등록이 되었다면 삭제
            if (null != manager)
                FormMain.Instance.DataManager.DeleteFacilityManager(manager);

            manager = null;
            manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.FLOOD_SENSOR, member.ID);
            // 등록이 되었다면 삭제
            if (null != manager)
                FormMain.Instance.DataManager.DeleteFacilityManager(manager);

            manager = null;
            manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.HEAT_SENSOR, member.ID);
            // 등록이 되었다면 삭제
            if (null != manager)
                FormMain.Instance.DataManager.DeleteFacilityManager(manager);

            manager = null;
            manager = FormMain.Instance.DataManager.SearchFacilityManager(Data.FacilityType.COLLAPSE_SENSOR, member.ID);
            // 등록이 되었다면 삭제
            if (null != manager)
                FormMain.Instance.DataManager.DeleteFacilityManager(manager);
        }

        private void gridMember_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("CellValueChanged");
        }

        // 핸드폰 번호 숫자 필터링
        private void gridMember_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int nIndex = gridMember.CurrentCell.ColumnIndex;

            if (nIndex == colPhoneNum.Index)
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += colPhoneNum_KeyPress;
            }
        }

        private void colPhoneNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
    }
}
