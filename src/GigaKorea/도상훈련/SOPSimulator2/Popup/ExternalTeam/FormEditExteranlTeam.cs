using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP;

namespace SOPMonitoringSystem
{
    public partial class FormEditExteranlTeam : Form
    {
        private const int TEAM_NAME_INDEX = 0;
        private const int PHONE_NUMBER_INDEX = 1;
        private const int FAX_NUMBER_INDEX = 2;
        private const int ROLE_INDEX = 3;
        private const int MEMBER_INDEX = 4;
        private const int BUTTON_INDEX = 5;

        // 새로 추가되거나 변경된 것을 포함한 Data_UserDefinedTeam List
        // Grid Row Index, 행별 Data_UserDefinedTeam
        private Dictionary<int, Data_UserDefinedTeam> m_dicUserDefinedTeamList = new Dictionary<int, Data_UserDefinedTeam>();
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeamList = new Dictionary<int, Data_ExternalTeam>();
        private Dictionary<int, Data_NormalTeam> m_dicNormalTeamList = new Dictionary<int, Data_NormalTeam>();
        private Dictionary<int, Data_EmergencyTeam> m_dicEmergencyTeamList = new Dictionary<int, Data_EmergencyTeam>();
        private Dictionary<int, Data_RegularTeam> m_dicRegularTeamList = new Dictionary<int, Data_RegularTeam>();
        // 삭제될 Data_UserDefinedTeam List
        private ArrayList m_arrRemoveUserDefinedTeamList = new ArrayList();

        private ArrayList m_UsingTeams = new ArrayList();
        public ArrayList UsingTeams
        {
            get { return m_UsingTeams; }
            //set { m_UsingTeams = value; }
        }

        public FormEditExteranlTeam()
        {
            InitializeComponent();
        }

        private void FormEditExteranlTeam_Load(object sender, EventArgs e)
        {
            InitGrid();
            //InitUserDefinedGrid();
        }

        private void InitGrid()
        {
            m_dicUserDefinedTeamList.Clear();
            m_dicExternalTeamList.Clear();
            m_dicNormalTeamList.Clear();
            m_dicEmergencyTeamList.Clear();
            m_dicRegularTeamList.Clear();

            dataGridViewUsingTeams.ClearSelection();
            dataGridViewUsingTeams.Rows.Clear();

            foreach (object team in UsingTeams)
            {
                if (team is Data_UserDefinedTeam)
                    AddTeam((Data_UserDefinedTeam)team);
                else if (team is Data_ExternalTeam)
                    AddTeam((Data_ExternalTeam)team);
                else if (team is Data_NormalTeam)
                    AddTeam((Data_NormalTeam)team);
                else if (team is Data_EmergencyTeam)
                    AddTeam((Data_EmergencyTeam)team);
                else if (team is Data_RegularTeam)
                    AddTeam((Data_RegularTeam)team);
            }
        }

        /*private void InitUserDefinedGrid()
        {
            m_dicUserDefinedTeamList.Clear();
            dataGridViewUsingTeams.ClearSelection();
            dataGridViewUsingTeams.Rows.Clear();

            //List<Data_ExternalTeam> arrUserDefinedTeam = FormSOP.Instance.SOPManager.UserDefineTeams;
            foreach (Data_UserDefinedTeam data in UsingTeams)
            {
                
                AllUserDefinedTeam(data);
            }
        }

        private void AllUserDefinedTeam(Data_UserDefinedTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_UserDefinedTeam team = new Data_UserDefinedTeam();

            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;

            cell = new DataGridViewTextBoxCell();
            cell.Value = (data.PhoneNumber == null || data.PhoneNumber == "null") ? "" : data.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = (data.FaxNumber == null || data.FaxNumber == "null") ? "" : data.FaxNumber;
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            cell = new DataGridViewTextBoxCell();
            gridRow.Cells.Add(cell);

            gridRow.Tag = team;
            team.ID = data.ID;
                        
            cell = new DataGridViewTextBoxCell();
            cell.Value = (data.Tag == null || data.Tag.ToString() == "null") ? "" : data.Tag.ToString();
            gridRow.Cells.Add(cell);
            team.Tag = data.Tag;            

            DataGridViewButtonCell btn = new DataGridViewButtonCell();
            btn.Value = "지정하기";
            gridRow.Cells.Add(btn);

            gridRow.Height = gridRow.Height + 3;

            if (dataGridViewUsingTeams.AllowUserToAddRows)
                m_dicUserDefinedTeamList[dataGridViewUsingTeams.Rows.Count - 1] = team;
            else
                m_dicUserDefinedTeamList[dataGridViewUsingTeams.Rows.Count] = team;

            dataGridViewUsingTeams.Rows.Add(gridRow);
        }*/

        public static DataRoleMember MakeRoleMember(Data_UserDefinedTeam data)
        {
            DataRoleMember roleMember = new DataRoleMember();
            data.Tag = roleMember;
            return roleMember;
        }

        public static DataRoleMember MakeRoleMember(Data_ExternalTeam data)
        {
            DataRoleMember roleMember;
            ExternalCompanyMember teamLeader = GetExternalTeamLeader(data.ID);

            if (teamLeader == null)
                roleMember = new DataRoleMember();
            else
                roleMember = new DataRoleMember(teamLeader.MemberName, teamLeader.PhoneNumber, "", "");

            data.Tag = roleMember;
            return roleMember;
        }

        public static List<DataRoleMember> MakeRoleMember(Data_RegularTeam data)
        {
            List<DataRoleMember> roleMembers = new List<DataRoleMember>();

            if (data.ParentTeamID < 0)
            {
                DataRoleMember roleMember = new DataRoleMember("전직원", "", "", "");
                roleMember.AllMembers = true;
                roleMembers.Add(roleMember);
            }
            else
            {
                List<Data_CompanyMember> teamMembers = GetRegularMembers(data.ID);

                if (teamMembers != null)
                {
                    foreach (Data_CompanyMember member in teamMembers)
                    {
                        DataRoleMember roleMember = new DataRoleMember(member.MemberName, member.PhoneNumber, "", "");
                        roleMembers.Add(roleMember);
                        break;
                    }
                }
            }

            if (roleMembers.Count == 0)
            {
                roleMembers.Add(new DataRoleMember());
            }

            data.Tag = roleMembers;
            return roleMembers;
        }
        /*public static DataRoleMember MakeRoleMember(Data_RegularTeam data)
        {
            DataRoleMember roleMember;
            Data_CompanyMember teamLeader = GetRegularTeamLeader(data.ID);

            if (teamLeader == null)
                roleMember = new DataRoleMember();
            else
                roleMember = new DataRoleMember(teamLeader.MemberName, teamLeader.PhoneNumber, "", "");

            data.Tag = roleMember;
            return roleMember;
        }*/

        public static List<DataRoleMember> MakeRoleMember(Data_NormalTeam data)
        {
            List<DataRoleMember> roleMembers = GetTemporaryRoleMembers(null, data.ID, true);
            data.Tag = roleMembers;
            return roleMembers;
        }

        public static List<DataRoleMember> MakeRoleMember(Data_EmergencyTeam data)
        {
            List<DataRoleMember> roleMembers = GetTemporaryRoleMembers(null, data.ID, false);
            data.Tag = roleMembers;
            return roleMembers;
        }

        private void AddTeam(Data_UserDefinedTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_UserDefinedTeam team = new Data_UserDefinedTeam();

            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;

            DataRoleMember roleMember = null;

            if (data.Tag == null)
            {
                roleMember = MakeRoleMember(data);
            }
            else
            {
                roleMember = (DataRoleMember)data.Tag;
            }

            cell.Tag = roleMember;

            cell = new DataGridViewTextBoxCell();
            cell.Value = roleMember.PhoneNumber;
            gridRow.Cells.Add(cell);

            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            cell = new DataGridViewTextBoxCell();
            gridRow.Cells.Add(cell);

            gridRow.Tag = team;
            team.ID = data.ID;

            cell = new DataGridViewTextBoxCell();
            cell.Value = roleMember.MemberName;
            gridRow.Cells.Add(cell);
            team.Tag = data.Tag;

            DataGridViewButtonCell btn = new DataGridViewButtonCell();
            btn.Value = "지정하기";
            gridRow.Cells.Add(btn);

            gridRow.Height = gridRow.Height + 3;

            if (dataGridViewUsingTeams.AllowUserToAddRows)
                m_dicUserDefinedTeamList[dataGridViewUsingTeams.Rows.Count - 1] = team;
            else
                m_dicUserDefinedTeamList[dataGridViewUsingTeams.Rows.Count] = team;

            dataGridViewUsingTeams.Rows.Add(gridRow);
        }

        private void AddTeam(Data_ExternalTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_ExternalTeam team = new Data_ExternalTeam();

            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;

            DataRoleMember roleMember = null;

            if (data.Tag == null)
            {
                roleMember = MakeRoleMember(data);
            }
            else
            {
                roleMember = (DataRoleMember)data.Tag;
            }

            cell.Tag = roleMember;

            cell = new DataGridViewTextBoxCell();
            cell.Value = roleMember.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            cell = new DataGridViewTextBoxCell();
            gridRow.Cells.Add(cell);

            gridRow.Tag = team;
            team.ID = data.ID;

            cell = new DataGridViewTextBoxCell();
            cell.Value = roleMember.MemberName;
            gridRow.Cells.Add(cell);
            team.Tag = data.Tag;

            DataGridViewButtonCell btn = new DataGridViewButtonCell();
            btn.Value = "지정하기";
            gridRow.Cells.Add(btn);

            gridRow.Height = gridRow.Height + 3;

            if (dataGridViewUsingTeams.AllowUserToAddRows)
                m_dicExternalTeamList[dataGridViewUsingTeams.Rows.Count - 1] = team;
            else
                m_dicExternalTeamList[dataGridViewUsingTeams.Rows.Count] = team;

            dataGridViewUsingTeams.Rows.Add(gridRow);
        }

        private void AddTeam(Data_RegularTeam data)
        {
            List<DataRoleMember> roleMembers = null;

            if (data.Tag == null)
            {
                roleMembers = MakeRoleMember(data);
            }
            else
            {
                roleMembers = (List<DataRoleMember>)data.Tag;
            }

            Data_RegularTeam team = new Data_RegularTeam();
            team.TeamName = data.TeamName;
            team.ID = data.ID;
            team.Tag = data.Tag;

            foreach (DataRoleMember roleMember in roleMembers)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                if (data.TeamName.EndsWith("장") == false && data.TeamName.EndsWith("본부") == false)
                    cell.Value = data.TeamName + "장";
                else
                    cell.Value = data.TeamName;

                gridRow.Cells.Add(cell);
                cell.Tag = roleMember;

                cell = new DataGridViewTextBoxCell();
                cell.Value = roleMember.PhoneNumber;
                gridRow.Cells.Add(cell);

                // 전직원일 경우 편집할 수 없도록 한다.
                if (roleMember.AllMembers)
                    cell.ReadOnly = true;

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                gridRow.Cells.Add(cell);

                gridRow.Tag = team;

                cell = new DataGridViewTextBoxCell();
                cell.Value = roleMember.MemberName;
                gridRow.Cells.Add(cell);

                // 전직원일 경우 편집할 수 없도록 한다.
                if (roleMember.AllMembers)
                    cell.ReadOnly = true;

                DataGridViewButtonCell btn = new DataGridViewButtonCell();
                btn.Value = "지정하기";
                gridRow.Cells.Add(btn);

                gridRow.Height = gridRow.Height + 3;

                if (dataGridViewUsingTeams.AllowUserToAddRows)
                    m_dicRegularTeamList[dataGridViewUsingTeams.Rows.Count - 1] = team;
                else
                    m_dicRegularTeamList[dataGridViewUsingTeams.Rows.Count] = team;

                dataGridViewUsingTeams.Rows.Add(gridRow);
            }
        }

        private void AddTeam(Data_NormalTeam data)
        {
            List<DataRoleMember> roleMembers = GetTemporaryRoleMembers((List<DataRoleMember>)data.Tag, data.ID, true);
            data.Tag = roleMembers;

            Data_NormalTeam team = new Data_NormalTeam();
            team.ID = data.ID;
            team.TeamName = data.TeamName;
            team.Tag = data.Tag;

            foreach (DataRoleMember roleMember in roleMembers)
            {
                AddTeam(roleMember, team.TeamName, team, true);
            }
        }

        private void AddTeam(Data_EmergencyTeam data)
        {
            List<DataRoleMember> roleMembers = GetTemporaryRoleMembers((List<DataRoleMember>)data.Tag, data.ID, false);
            data.Tag = roleMembers;

            Data_EmergencyTeam team = new Data_EmergencyTeam();
            team.ID = data.ID;
            team.TeamName = data.TeamName;
            team.Tag = data.Tag;

            foreach (DataRoleMember roleMember in roleMembers)
            {
                AddTeam(roleMember, team.TeamName, team, false);
            }
        }

        private static List<DataRoleMember> GetTemporaryRoleMembers(List<DataRoleMember> roleMembers, int nTemporaryTeamID, bool isNormal)
        {
            if (roleMembers == null)
            {
                roleMembers = new List<DataRoleMember>();
                List<TemporaryMember> members = GetTemporaryMembers(nTemporaryTeamID, isNormal);

                if (members != null)
                {
                    string strDisplayName = "", strPhoneNumber = "", strMemberName = "";

                    foreach (TemporaryMember member in members)
                    {
                        if (ComponentContents.GetTemporaryMemberInfo(member, ref strDisplayName, ref strPhoneNumber, ref strMemberName))
                        {
                            DataRoleMember roleMember = new DataRoleMember(strMemberName, strPhoneNumber, TemporaryMember.GetRoleTypeString(member._RoleType), strDisplayName);
                            roleMembers.Add(roleMember);
                        }
                    }
                }

                if (roleMembers.Count == 0)
                {
                    // DataRoleMember가 하나도 없을 경우 빈 데이터를 하나 넣는다.
                    roleMembers.Add(new DataRoleMember());
                }
            }

            return roleMembers;
        }

        private void AddTeam(DataRoleMember roleMember, string strTeamName, object data, bool isNormal)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();

            cell.Value = strTeamName;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = roleMember.PhoneNumber;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            gridRow.Cells.Add(cell);

            if (roleMember.Role.Length > 0 && roleMember.JobName.Length > 0)
                cell.Value = roleMember.Role + "(" + roleMember.JobName + ")";
            else if (roleMember.Role.Length > 0)
                cell.Value = roleMember.Role;
            else if (roleMember.JobName.Length > 0)
                cell.Value = roleMember.JobName;

            gridRow.Tag = data;
            gridRow.Cells[0].Tag = roleMember;

            cell = new DataGridViewTextBoxCell();
            cell.Value = roleMember.MemberName;
            gridRow.Cells.Add(cell);

            DataGridViewButtonCell btn = new DataGridViewButtonCell();
            btn.Value = "지정하기";
            gridRow.Cells.Add(btn);

            gridRow.Height = gridRow.Height + 3;

            if (isNormal)
            {
                if (dataGridViewUsingTeams.AllowUserToAddRows)
                    m_dicNormalTeamList[dataGridViewUsingTeams.Rows.Count - 1] = (Data_NormalTeam)data;
                else
                    m_dicNormalTeamList[dataGridViewUsingTeams.Rows.Count] = (Data_NormalTeam)data;
            }
            else
            {
                if (dataGridViewUsingTeams.AllowUserToAddRows)
                    m_dicEmergencyTeamList[dataGridViewUsingTeams.Rows.Count - 1] = (Data_EmergencyTeam)data;
                else
                    m_dicEmergencyTeamList[dataGridViewUsingTeams.Rows.Count] = (Data_EmergencyTeam)data;
            }

            dataGridViewUsingTeams.Rows.Add(gridRow);
        }

        private static List<TemporaryMember> GetTemporaryMembers(int nTeamID, bool isNormal)
        {
            bool includeMain = true;            // 정
            bool includeSub = true;             // 부
            bool includeTeamLeader = true;     // 팀장
            bool includeOthers = true;         // 반원

            List<TemporaryMember> members = ComponentContents.GetTemporaryMembers(nTeamID, isNormal, includeMain, includeSub, includeTeamLeader, includeOthers);
            return members;
        }

        private static ExternalCompanyMember GetExternalTeamLeader(int nTeamID)
        {
            ExternalCompanyTeam team = FormSOP.Instance.SOPManager.FindExternalCompanyTeam(nTeamID);

            if (team == null)
                return null;

            ExternalCompanyMember member = new ExternalCompanyMember();

            // 회사일 경우
            if (team.ParentTeam == null)
            {
                Data_ExternalTeam company = FormSOP.Instance.SOPManager.GetExternalTeam(nTeamID);

                if (company != null)
                    member.PhoneNumber = company.PhoneNumber;
            }
            else
            {
                if (team.Members.Count > 0)
                {
                    member.MemberName = team.Members[0].MemberName;
                    member.PhoneNumber = team.Members[0].PhoneNumber;
                }
            }

            return member;
        }

        // 1. 팀장이 존재하면 팀장을 리턴한다.
        // 2. 팀장이 존재하지 않고 파트장이 존재할 경우 파트장을 리턴한다.
        // 3. 그렇지 않을 경우 모든 팀원을 리턴한다.
        private static List<Data_CompanyMember> GetRegularMembers(int nTeamID)
        {
            ArrayList members = new ArrayList();

            if (!FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(nTeamID, ref members))
                return null;

            List<Data_CompanyMember> teamLeaders = new List<Data_CompanyMember>();
            List<Data_CompanyMember> partLeaders = new List<Data_CompanyMember>();
            List<Data_CompanyMember> allMembers = new List<Data_CompanyMember>();

            foreach (Data_CompanyMember member in members)
            {
                foreach (KeyValuePair<Data_RegularTeam, int> pair in member.TeamPositions)
                {
                    if (pair.Key.ID == nTeamID)
                    {
                        if (pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.TEAM_LEADER ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.실장 ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.처장 ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.본부장)
                            teamLeaders.Add(member);
                        else if (pair.Value == 3)
                            partLeaders.Add(member);

                        if (pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.TEAM_MEMBER ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.TEAM_LEADER ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.실장 ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.본부장 ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.처장 ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.PART_LEADER ||
                            pair.Value == (int)ControlTeamEditor.JobPosition.PositionType.CENTER_LEADER)
                            allMembers.Add(member);
                    }
                }
            }

            if (teamLeaders.Count > 0)
            {
                teamLeaders.Sort();
                return teamLeaders;
            }
            else if (partLeaders.Count > 0)
            {
                partLeaders.Sort();
                return partLeaders;
            }

            return allMembers;
        }

        /*private static Data_CompanyMember GetRegularTeamLeader(int nTeamID)
        {
            ArrayList members = new ArrayList();

            if (!FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(nTeamID, ref members))
                return null;

            // 팀장이 없을 경우 그 대리인을 찾아서 리턴한다.
            Data_CompanyMember viceLeader = null;
            int nViceJobPosition = -1;

            foreach (Data_CompanyMember member in members)
            {
                foreach (KeyValuePair<Data_RegularTeam, int> pair in member.TeamPositions)
                {
                    if (pair.Key.ID == nTeamID)
                    {
                        if (pair.Value == 2)
                            return member;
                        else
                        {
                            if (viceLeader == null || ControlTeamEditor.JobPosition.CompareJobPosition(nViceJobPosition, pair.Value) == 1)
                            {
                                viceLeader = member;
                                nViceJobPosition = pair.Value;
                            }
                        }
                    }
                }
            }

            if (viceLeader != null)
                return viceLeader;

            return null;
        }*/

        /*public void FindGridRowUserDefined(int TeamID)
        {
            dataGridViewUsingTeams.ClearSelection();
            foreach (DataGridViewRow row in dataGridViewUsingTeams.Rows)
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;
                if (team.ID == TeamID)
                {
                    row.Selected = true;
                    break;
                }
            }
        }*/

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            else if(keyData == Keys.Delete)
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SaveUsingTeams()
        {
            foreach (DataGridViewRow row in dataGridViewUsingTeams.Rows)
            {
                if (row.IsNewRow || row.Cells[0].Tag == null)
                    continue;

                DataRoleMember roleMember = (DataRoleMember)row.Cells[0].Tag;
                roleMember.PhoneNumber = row.Cells[PHONE_NUMBER_INDEX].Value == null ? "" : row.Cells[PHONE_NUMBER_INDEX].Value.ToString();
                roleMember.MemberName = row.Cells[MEMBER_INDEX].Value == null ? "" : row.Cells[MEMBER_INDEX].Value.ToString();
            }
        }

        /*private Data_UserDefinedTeam FindUserDefinedTeam(int nTeamID, List<Data_UserDefinedTeam> arrTeamList)
        {
            int nTeamCount = arrTeamList.Count;

            for (int i = 0; i < nTeamCount; i++)
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)arrTeamList[i];
                if (team.ID == nTeamID)
                    return team;
            }

            return null;
        }

        // 기존에 존재하던 사용자 정의 조직 데이터인가 여부.
        // 만일 기존에 존재하던 팀이라면 데이터가 바뀌었는지 여부
        // Return 값 : 0(기존에 존재하던 팀이며 아무것도 바뀌지 않음)
        //             1(기존에 존재하던 팀이며, 데이터가 바뀌었음)
        //            -1(새로운 팀)
        //            -1(잘못된 데이터)
        private int CheckUserDefinedTeam(Data_UserDefinedTeam team)
        {
            if (team.TeamName.Length == 0)
                return -2;


            List<Data_UserDefinedTeam> arrUserDefinedTeam = UsingTeams;
            foreach (Data_UserDefinedTeam data in arrUserDefinedTeam)
            {
                if (data.TeamName == team.TeamName)
                {
                    team.ID = data.ID;

                    if (team.PhoneNumber.Length == 0)
                        return -2;

                    if (team.PhoneNumber == data.PhoneNumber &&
                        team.FaxNumber == data.FaxNumber)
                        return 0;
                    else
                        return 1;
                }
            }

            team.ID = -1;
            return -1;
        }

        private void SaveUserDefinedList()
        {
            ArrayList arrNewTeam = new ArrayList();
            ArrayList arrUpdateTeam = new ArrayList();

            foreach (KeyValuePair<int, Data_UserDefinedTeam> pair in this.m_dicUserDefinedTeamList)
            {
                int nResult = CheckUserDefinedTeam(pair.Value);

                if (nResult == 1)
                    arrUpdateTeam.Add(pair.Value);
            }

            foreach (Data_UserDefinedTeam team in arrUpdateTeam)
            {                
                // Update [ActionStepUsingUserDefinedTeam]               

                Data_UserDefinedTeam _team = FindUserDefinedTeam(team.ID, UsingTeams);
                if (_team != null)
                {
                    _team.FaxNumber = team.FaxNumber;
                    _team.PhoneNumber = team.PhoneNumber;
                    _team.TeamName = team.TeamName;
                    _team.Tag = team.Tag;
                }
            }


        }*/

        public void OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                if (dataGridViewUsingTeams.SelectedRows == null || dataGridViewUsingTeams.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewUsingTeams.Rows.Count;
                if (dataGridViewUsingTeams.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewUsingTeams.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                DataGridViewRow row = dataGridViewUsingTeams.SelectedRows[0];

                row.Cells[PHONE_NUMBER_INDEX].Value = "";
                row.Cells[MEMBER_INDEX].Value = "";
            }
        }

        /*private void dataGridViewUserDefined_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewUsingTeams)
                    return;

                if (dataGridViewUsingTeams.SelectedRows == null || dataGridViewUsingTeams.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewUsingTeams.Rows.Count;
                if (dataGridViewUsingTeams.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewUsingTeams.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                dataGridViewUsingTeams.Rows.RemoveAt(nRowIndex);

                if (!m_dicUserDefinedTeamList.ContainsKey(nRowIndex))
                    return;

                Data_UserDefinedTeam selectedTeam = m_dicUserDefinedTeamList[nRowIndex];
                if (selectedTeam.ID > 0)
                    m_arrRemoveUserDefinedTeamList.Add(selectedTeam);

                /////////////////////////////////////////////////////////////////
                // dictionary의 데이터를 삭제된 행을 기준으로 하나씩 아래로 내린다.
                for (int i = nRowIndex + 1; i < nRowCount; i++)
                {
                    m_dicUserDefinedTeamList[i - 1] = m_dicUserDefinedTeamList[i];
                }

                m_dicUserDefinedTeamList.Remove(nRowCount - 1);
                /////////////////////////////////////////////////////////////////
            }
        }*/

        // nRowIndex의 첫번째 Cell의 텍스트가 다른 행에 이미 존재하는지 여부를 확인한다.
        // 이미 존재하면 false, 존재하지 않으면 true를 리턴한다.
        private bool CheckDuplicate(DataGridView grid, int nRowIndex, string strValue)
        {
            int nRowCount = grid.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                if (i == nRowIndex)
                    continue;

                if (grid.Rows[i].Cells[0].Value != null)
                {
                    if (grid.Rows[i].Cells[0].Value.ToString() == strValue)
                        return false;
                }

            }

            return true;
        }

        private void dataGridViewUsingTeams_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];
            if (row == null)
                return;

            object value = row.Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            DataRoleMember roleMember = (DataRoleMember)row.Cells[0].Tag;

            if (e.ColumnIndex == PHONE_NUMBER_INDEX || e.ColumnIndex == FAX_NUMBER_INDEX)
            {
                string szPhoneNumber = "";
                bool isCheck = ValidPhoneNumber(strValue, out szPhoneNumber);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");

                    if (roleMember == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == PHONE_NUMBER_INDEX ? roleMember.PhoneNumber : "";
                }
            }
            else if (e.ColumnIndex == MEMBER_INDEX)
            {
                grid.Rows[e.RowIndex].Cells[MEMBER_INDEX].Tag = strValue;
            }
        }

        /*private void dataGridViewUserDefined_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];
            if (row == null)
                return;

            object value = row.Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;

            if (e.ColumnIndex == TEAM_NAME_INDEX)
            {
                if (team != null && !CheckDuplicate(grid, e.RowIndex, strValue))
                {
                    value = team.TeamName;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_UserDefinedTeam();
                        m_dicUserDefinedTeamList[e.RowIndex] = team;
                        row.Tag = team;

                        team.PhoneNumber = "";
                        team.FaxNumber = "";

                    }

                    // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                    team.TeamName = strValue;
                    team.ID = -1;
                }
            }
            else if (e.ColumnIndex == PHONE_NUMBER_INDEX || e.ColumnIndex == FAX_NUMBER_INDEX)
            {
                string szPhoneNumber = "";
                bool isCheck = ValidPhoneNumber(strValue, out szPhoneNumber);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");

                    if (team == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == PHONE_NUMBER_INDEX ? team.PhoneNumber : team.FaxNumber;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_UserDefinedTeam();
                        m_dicUserDefinedTeamList[e.RowIndex] = team;
                        row.Tag = team;

                        team.PhoneNumber = "";
                        team.FaxNumber = "";
                    }

                    if (e.ColumnIndex == PHONE_NUMBER_INDEX)
                        team.PhoneNumber = szPhoneNumber;
                    else
                        team.FaxNumber = szPhoneNumber;

                    grid.Rows[e.RowIndex].Cells[MEMBER_INDEX].Value = "";
                    grid.Rows[e.RowIndex].Cells[MEMBER_INDEX].Tag = null;
                    team.Tag = null;
                }
            }
            else if (e.ColumnIndex == MEMBER_INDEX)
            {
                grid.Rows[e.RowIndex].Cells[MEMBER_INDEX].Value = strValue;
                grid.Rows[e.RowIndex].Cells[MEMBER_INDEX].Tag = strValue;
            }
        }*/
        private bool ValidPhoneNumber(string strPhoneNumber, out string strValid)
        {
            //isValid = true;

            strValid = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '-')
                {
                    if (ch >= '0' && ch <= '9')
                        strValid += ch;
                    else
                    {
                        //isValid = false;
                        return false;
                    }
                }
            }

            return true;
        }
  
        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveUsingTeams();
            //SaveUserDefinedList();
                       
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void dataGridViewUsingTeams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == BUTTON_INDEX)
            {
                DataRoleMember roleMember = (DataRoleMember)dataGridViewUsingTeams.Rows[e.RowIndex].Cells[0].Tag;

                // 전직원에 해당할 경우 편집할 수 없도록 한다.
                if (roleMember == null || roleMember.AllMembers)
                    return;

                FormTeamTree treeForm = new FormTeamTree();
                if (treeForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string szName = treeForm.SelectedName;
                    string szPhone = treeForm.SelectedPhone;
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dataGridViewUsingTeams.Rows[e.RowIndex].Cells[MEMBER_INDEX];
                    if (cell != null)
                    {
                        cell.Value = szName;
                        cell.Tag = szName;
                    }

                    DataGridViewTextBoxCell cell2 = (DataGridViewTextBoxCell)dataGridViewUsingTeams.Rows[e.RowIndex].Cells[PHONE_NUMBER_INDEX];
                    if (cell2 != null)
                    {
                        cell2.Value = szPhone;
                    }
                }
            }
        }

        /*private void dataGridViewUserDefined_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == BUTTON_INDEX)
            {
                FormTeamTree treeForm = new FormTeamTree();
                if (treeForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string szName = treeForm.SelectedName;
                    string szPhone = treeForm.SelectedPhone;
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dataGridViewUsingTeams.Rows[e.RowIndex].Cells[MEMBER_INDEX];
                    if (cell != null)
                    {
                        cell.Value = szName;
                        cell.Tag = szName;
                    }

                    DataGridViewTextBoxCell cell2 = (DataGridViewTextBoxCell)dataGridViewUsingTeams.Rows[e.RowIndex].Cells[PHONE_NUMBER_INDEX];
                    if (cell2 != null)
                    {
                        cell2.Value = szPhone;
                    }

                    (dataGridViewUsingTeams.Rows[e.RowIndex].Tag as Data_UserDefinedTeam).PhoneNumber = szPhone;
                    (dataGridViewUsingTeams.Rows[e.RowIndex].Tag as Data_UserDefinedTeam).Tag = szName;
                }
            }
        }*/

        public void SetUsingTeam(UnE.SOP.Sections.SectionTabPage page)
        {
            m_UsingTeams.Clear();

            // 사용자정의 조직만 사용한다.
            List<Data_UserDefinedTeam> userDefinedTeams = page.GetUsingUserDefineTeams();
            /*List<Data_ExternalTeam> externalTeams = page.GetUsingExternalTeams();
            List<Data_NormalTeam> normalTeams = page.GetUsingTemporaryNormalTeams();
            List<Data_EmergencyTeam> emergencyTeams = page.GetUsingTemporaryEmergencyTeams();
            List<Data_RegularTeam> regularTeams = page.GetUsingRegularTeams();*/

            if (userDefinedTeams != null)
                m_UsingTeams.AddRange(userDefinedTeams);

            /*if (externalTeams != null)
                m_UsingTeams.AddRange(externalTeams);

            if (normalTeams != null)
                m_UsingTeams.AddRange(normalTeams);

            if (emergencyTeams != null)
                m_UsingTeams.AddRange(emergencyTeams);

            if (regularTeams != null)
                m_UsingTeams.AddRange(regularTeams);*/
        }
    }

    /*public class DataGridViewEx : DataGridView
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.Delete)
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }*/
}
