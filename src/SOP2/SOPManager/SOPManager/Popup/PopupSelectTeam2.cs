using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace SOPManager
{
    public partial class PopupSelectTeam2 : Form
    {
        // 0(평일), 1(휴일), 2(외부 기관), 3(사용자 정의 조직), 4(정규 조직)
        private int m_nCurrentTeamType = -1;
        private int m_nSelectedTeamType = -1;

        private ArrayList m_arrExternalTeam = new ArrayList();
        private ArrayList m_arrUserDefinedTeam = new ArrayList();

        // 새로 추가되거나 변경된 것을 포함한 Data_ExternalTeam List
        // Grid Row Index, 행별 Data_ExternalTeam
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeamList = new Dictionary<int, Data_ExternalTeam>();
        // 삭제될 Data_ExternalTeam List
        private ArrayList m_arrRemoveExternalTeamList = new ArrayList();

        // 새로 추가되거나 변경된 것을 포함한 Data_UserDefinedTeam List
        // Grid Row Index, 행별 Data_UserDefinedTeam
        private Dictionary<int, Data_UserDefinedTeam> m_dicUserDefinedTeamList = new Dictionary<int, Data_UserDefinedTeam>();
        // 삭제될 Data_UserDefinedTeam List
        private ArrayList m_arrRemoveUserDefinedTeamList = new ArrayList();

        // 정규 조직의 표시 여부
        private bool m_noRegularTeam = false;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupSelectTeam2(int nCurrentTeamType, bool noRegularTeam = false)
        {
            InitializeComponent();

            m_nCurrentTeamType = nCurrentTeamType;
            m_arrExternalTeam = FormMain.Instance.ExternalTeam;
            m_arrUserDefinedTeam = FormMain.Instance.UserDefinedTeam;

            panel1.Visible = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_nSelectedTeamType == 2)
            {
                // DB 저장
                SaveExternalList();
            }
            else if (m_nSelectedTeamType == 3)
            {
                // DB 저장
                SaveUserDefinedList();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 기존에 존재하던 외부팀 데이터인가 여부.
        // 만일 기존에 존재하던 팀이라면 데이터가 바뀌었는지 여부
        // Return 값 : 0(기존에 존재하던 팀이며 아무것도 바뀌지 않음)
        //             1(기존에 존재하던 팀이며, 데이터가 바뀌었음)
        //            -1(새로운 팀)
        //            -1(잘못된 데이터)
        private int CheckExternalTeam(Data_ExternalTeam team)
        {
            if (team.TeamName.Length == 0)
                return -2;

            foreach (Data_ExternalTeam data in m_arrExternalTeam)
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

        private int FindExternalTeam(int nTeamID, ArrayList arrTeamList)
        {
            int nTeamCount = arrTeamList.Count;
            
            for (int i = 0; i < nTeamCount; i++)
            {
                Data_ExternalTeam team = (Data_ExternalTeam)arrTeamList[i];
                if (team.ID == nTeamID)
                    return i;
            }

            return -1;
        }

        private int FindUserDefinedTeam(int nTeamID, ArrayList arrTeamList)
        {
            int nTeamCount = arrTeamList.Count;

            for (int i = 0; i < nTeamCount; i++)
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)arrTeamList[i];
                if (team.ID == nTeamID)
                    return i;
            }

            return -1;
        }

        private void SaveExternalList()
        {
            ArrayList arrNewTeam = new ArrayList();
            ArrayList arrUpdateTeam = new ArrayList();

            foreach (KeyValuePair<int, Data_ExternalTeam> pair in m_dicExternalTeamList)
            {
                int nResult = CheckExternalTeam(pair.Value);

                if (nResult == 1)
                    arrUpdateTeam.Add(pair.Value);
                else if (nResult == -1)
                    arrNewTeam.Add(pair.Value);
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strRemoveIDs = "", strSQL;

            ////////////////////////////////////////////////////////////////////
            // 데이터 삭제
            foreach (Data_ExternalTeam team in m_arrRemoveExternalTeamList)
            {
                if (strRemoveIDs.Length == 0)
                    strRemoveIDs = team.ID.ToString();
                else
                    strRemoveIDs += ", " + team.ID.ToString();

                int nIndex = FindExternalTeam(team.ID, FormMain.Instance.ExternalTeam);
                if (nIndex >= 0)
                    FormMain.Instance.ExternalTeam.RemoveAt(nIndex);
            }

            if (strRemoveIDs.Length > 0)
            {
                strSQL = string.Format("Delete from ExternalTeam where id in ({0})", strRemoveIDs);
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }

            m_arrRemoveExternalTeamList.Clear();
            ////////////////////////////////////////////////////////////////////

            strSQL = "select max(id) from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
				nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            foreach (Data_ExternalTeam team in arrNewTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber) values ({0}, '{1}', '{2}', {3})",
                    ++nTeamID, team.TeamName, strPhoneNumber, strFaxNumber);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                team.ID = nTeamID;
                FormMain.Instance.ExternalTeam.Add(team);
            }

            foreach (Data_ExternalTeam team in arrUpdateTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Update ExternalTeam set PhoneNumber = '{0}', FaxNumber = {1} where id = {2}",
                    strPhoneNumber, strFaxNumber, team.ID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                int nIndex = FindExternalTeam(team.ID, FormMain.Instance.ExternalTeam);
                if (nIndex >= 0)
                {
                    Data_ExternalTeam _team = (Data_ExternalTeam)FormMain.Instance.ExternalTeam[nIndex];
                    _team.FaxNumber = team.FaxNumber;
                    _team.PhoneNumber = team.PhoneNumber;
                    _team.TeamName = team.TeamName;
                }
            }
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

            foreach (Data_UserDefinedTeam data in m_arrUserDefinedTeam)
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
                else if (nResult == -1)
                    arrNewTeam.Add(pair.Value);
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strRemoveIDs = "", strSQL;

            ////////////////////////////////////////////////////////////////////
            // 데이터 삭제
            foreach (Data_UserDefinedTeam team in m_arrRemoveUserDefinedTeamList)
            {
                if (strRemoveIDs.Length == 0)
                    strRemoveIDs = team.ID.ToString();
                else
                    strRemoveIDs += ", " + team.ID.ToString();

                int nIndex = FindUserDefinedTeam(team.ID, FormMain.Instance.UserDefinedTeam);
                if (nIndex >= 0)
                    FormMain.Instance.UserDefinedTeam.RemoveAt(nIndex);
            }

            if (strRemoveIDs.Length > 0)
            {
                strSQL = string.Format("Delete from UserDefinedTeam where id in ({0})", strRemoveIDs);
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }

            m_arrRemoveUserDefinedTeamList.Clear();
            ////////////////////////////////////////////////////////////////////

            strSQL = "select max(id) from UserDefinedTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
				nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            foreach (Data_UserDefinedTeam team in arrNewTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Insert into UserDefinedTeam (ID, TeamName, PhoneNumber, FaxNumber) values ({0}, '{1}', '{2}', {3})",
                    ++nTeamID, team.TeamName, strPhoneNumber, strFaxNumber);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                team.ID = nTeamID;
                FormMain.Instance.UserDefinedTeam.Add(team);
            }

            foreach (Data_UserDefinedTeam team in arrUpdateTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Update UserDefinedTeam set PhoneNumber = '{0}', FaxNumber = {1} where id = {2}",
                    strPhoneNumber, strFaxNumber, team.ID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                int nIndex = FindUserDefinedTeam(team.ID, FormMain.Instance.UserDefinedTeam);
                if (nIndex >= 0)
                {
                    Data_UserDefinedTeam _team = (Data_UserDefinedTeam)FormMain.Instance.UserDefinedTeam[nIndex];
                    _team.FaxNumber = team.FaxNumber;
                    _team.PhoneNumber = team.PhoneNumber;
                    _team.TeamName = team.TeamName;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopupSelectTeam2_Load(object sender, EventArgs e)
        {
            // 평일과 야간은 동시에 나타날 수 없어야 하므로, 4번째 Radio Button은 불필요하다.
            radioTeam4.Visible = false;
            rdPictureBox4.Visible = false;
            rdLabel4.Visible = false;

			rdPictureBox3.Visible = false;
			rdLabel3.Visible = false;

            if (m_nCurrentTeamType == 0)
            {
                radioTeam1.Text = "외부 기관";
                radioTeam1.Tag = 2;
                rdLabel1.Text = radioTeam1.Text;

                radioTeam2.Text = "사용자 정의 조직";
                radioTeam2.Tag = 3;
                rdLabel2.Text = radioTeam2.Text;

                //if (m_noRegularTeam)
                    radioTeam3.Visible = false;
				//else
				//{
				//    radioTeam3.Text = "정규 조직";
				//    radioTeam3.Tag = 4;
				//    rdLabel3.Text = radioTeam3.Text;
				//}
            }
            else if (m_nCurrentTeamType == 1)
            {
                radioTeam1.Text = "외부 기관";
                radioTeam1.Tag = 2;
                rdLabel1.Text = radioTeam1.Text;

                radioTeam2.Text = "사용자 정의 조직";
                radioTeam2.Tag = 3;
                rdLabel2.Text = radioTeam2.Text;

                //if (m_noRegularTeam)
                    radioTeam3.Visible = false;
				//else
				//{
				//    radioTeam3.Text = "정규 조직";
				//    radioTeam3.Tag = 4;
				//    rdLabel3.Text = radioTeam3.Text;
				//}
            }
            else if (m_nCurrentTeamType == 2)
            {
                if (FormMain.Instance.GetPageDisaster().IsWeekMode())
                {
                    radioTeam1.Text = "평일 비상 조직";
                    radioTeam1.Tag = 0;
                    rdLabel1.Text = radioTeam1.Text;
                }
                else
                {
                    radioTeam1.Text = "야간 및 휴일 비상 조직";
                    radioTeam1.Tag = 1;
                    rdLabel1.Text = radioTeam1.Text;
                }

                radioTeam2.Text = "사용자 정의 조직";
                radioTeam2.Tag = 3;
                rdLabel2.Text = radioTeam2.Text;

                //if (m_noRegularTeam)
                    radioTeam3.Visible = false;
				//else
				//{
				//    radioTeam3.Text = "정규 조직";
				//    radioTeam3.Tag = 4;
				//    rdLabel3.Text = radioTeam3.Text;
				//}
            }
            else if (m_nCurrentTeamType == 3)
            {
                if (FormMain.Instance.GetPageDisaster().IsWeekMode())
                {
                    radioTeam1.Text = "평일 비상 조직";
                    radioTeam1.Tag = 0;
                    rdLabel1.Text = radioTeam1.Text;
                }
                else
                {
                    radioTeam1.Text = "야간 및 휴일 비상 조직";
                    radioTeam1.Tag = 1;
                    rdLabel1.Text = radioTeam1.Text;
                }

                radioTeam2.Text = "외부 기관";
                radioTeam2.Tag = 2;
                rdLabel2.Text = radioTeam2.Text;

                if (m_noRegularTeam)
                    radioTeam3.Visible = false;
                else
                {
                    radioTeam3.Text = "정규 조직";
                    radioTeam3.Tag = 4;
                    rdLabel3.Text = radioTeam3.Text;
                }
            }
            else if (m_nCurrentTeamType == 4)
            {
                if (FormMain.Instance.GetPageDisaster().IsWeekMode())
                {
                    radioTeam1.Text = "평일 비상 조직";
                    radioTeam1.Tag = 0;
                    rdLabel1.Text = radioTeam1.Text;
                }
                else
                {
                    radioTeam1.Text = "야간 및 휴일 비상 조직";
                    radioTeam1.Tag = 1;
                    rdLabel1.Text = radioTeam1.Text;
                }

                radioTeam2.Text = "외부 기관";
                radioTeam2.Tag = 2;
                rdLabel2.Text = radioTeam2.Text;

                if (m_noRegularTeam)
                    radioTeam3.Visible = false;
                else
                {
                    radioTeam3.Text = "사용자 정의 조직";
                    radioTeam3.Tag = 3;
                    rdLabel3.Text = radioTeam3.Text;
                }
            }
            else
                return;
            /*if (m_nCurrentTeamType == 0)
            {
                radioTeam1.Text = "야간 및 휴일 비상 조직";
                radioTeam1.Tag = 1;
                radioTeam2.Text = "외부 기관";
                radioTeam2.Tag = 2;
                radioTeam3.Text = "사용자 정의 조직";
                radioTeam3.Tag = 3;
                radioTeam4.Text = "정규 조직";
                radioTeam4.Tag = 4;
            }
            else if (m_nCurrentTeamType == 1)
            {
                radioTeam1.Text = "평일 비상 조직";
                radioTeam1.Tag = 0;
                radioTeam2.Text = "외부 기관";
                radioTeam2.Tag = 2;
                radioTeam3.Text = "사용자 정의 조직";
                radioTeam3.Tag = 3;
                radioTeam4.Text = "정규 조직";
                radioTeam4.Tag = 4;
            }
            else if (m_nCurrentTeamType == 2)
            {
                radioTeam1.Text = "평일 비상 조직";
                radioTeam1.Tag = 0;
                radioTeam2.Text = "야간 및 휴일 비상 조직";
                radioTeam2.Tag = 1;
                radioTeam3.Text = "사용자 정의 조직";
                radioTeam3.Tag = 3;
                radioTeam4.Text = "정규 조직";
                radioTeam4.Tag = 4;
            }
            else if (m_nCurrentTeamType == 3)
            {
                radioTeam1.Text = "평일 비상 조직";
                radioTeam1.Tag = 0;
                radioTeam2.Text = "야간 및 휴일 비상 조직";
                radioTeam2.Tag = 1;
                radioTeam3.Text = "외부 기관";
                radioTeam3.Tag = 2;
                radioTeam4.Text = "정규 조직";
                radioTeam4.Tag = 4;
            }
            else if (m_nCurrentTeamType == 4)
            {
                radioTeam1.Text = "평일 비상 조직";
                radioTeam1.Tag = 0;
                radioTeam2.Text = "야간 및 휴일 비상 조직";
                radioTeam2.Tag = 1;
                radioTeam3.Text = "외부 기관";
                radioTeam3.Tag = 2;
                radioTeam4.Text = "사용자 정의 조직";
                radioTeam4.Tag = 3;
            }
            else
                return;*/

            radioTeam1.Checked = true;

            SetRadioImage();

            InitUserDefinedGrid();
            InitExternalGrid();
        }

        private void InitUserDefinedGrid()
        {
            foreach (Data_UserDefinedTeam data in m_arrUserDefinedTeam)
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
            cell.Value = data.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.FaxNumber;
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            gridRow.Tag = data.ID;
            team.ID = data.ID;

            if (dataGridViewUserDefined.AllowUserToAddRows)
                m_dicUserDefinedTeamList[dataGridViewUserDefined.Rows.Count - 1] = team;
            else
                m_dicUserDefinedTeamList[dataGridViewUserDefined.Rows.Count] = team;

            dataGridViewUserDefined.Rows.Add(gridRow);
        }

        private void InitExternalGrid()
        {
            foreach (Data_ExternalTeam data in m_arrExternalTeam)
            {
                AllExternalTeam(data);
            }
        }

        private void AllExternalTeam(Data_ExternalTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_ExternalTeam team = new Data_ExternalTeam();

            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.FaxNumber;
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            gridRow.Tag = data.ID;
            team.ID = data.ID;

            if (dataGridViewExternal.AllowUserToAddRows)
                m_dicExternalTeamList[dataGridViewExternal.Rows.Count - 1] = team;
            else
                m_dicExternalTeamList[dataGridViewExternal.Rows.Count] = team;

            dataGridViewExternal.Rows.Add(gridRow);
        }

        private void radioTeam_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn == null)
                return;

            int nTeamType = (int)btn.Tag;
            m_nSelectedTeamType = nTeamType;

            if (nTeamType == 0 || nTeamType == 1)
            {
				label1.Visible = false;
				label2.Visible = false;
				label4.Visible = false;

                labelTeamName.Visible = false;
                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = false;
				
                m_nSelectedTeamType = nTeamType;
            }
            else if (nTeamType == 2)
            {
				label1.Visible = true;
				label2.Visible = true;
				label4.Visible = true;

                labelTeamName.Text = "외부 기관";
                labelTeamName.Visible = true;
                dataGridViewExternal.Visible = true;
                dataGridViewUserDefined.Visible = false;
            }
            else if (nTeamType == 3)
            {
				label1.Visible = true;
				label2.Visible = true;
				label4.Visible = true;

                labelTeamName.Text = "사용자 정의 조직";
                labelTeamName.Visible = true;
                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = true;
            }
            else// if (nTeamType == 4)
            {
                labelTeamName.Visible = false;
                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = false;
            }
        }

        private void dataGridViewUserDefined_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewUserDefined)
                    return;

                if (dataGridViewUserDefined.SelectedRows == null || dataGridViewUserDefined.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewUserDefined.Rows.Count;
                if (dataGridViewUserDefined.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewUserDefined.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                dataGridViewUserDefined.Rows.RemoveAt(nRowIndex);

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
        }

        private void dataGridViewExternal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewExternal)
                    return;

                if (dataGridViewExternal.SelectedRows == null || dataGridViewExternal.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewExternal.Rows.Count;
                if (dataGridViewExternal.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewExternal.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                dataGridViewExternal.Rows.RemoveAt(nRowIndex);

                if (!m_dicExternalTeamList.ContainsKey(nRowIndex))
                    return;

                Data_ExternalTeam selectedTeam = m_dicExternalTeamList[nRowIndex];
                if (selectedTeam.ID > 0)
                    m_arrRemoveExternalTeamList.Add(selectedTeam);

                /////////////////////////////////////////////////////////////////
                // dictionary의 데이터를 삭제된 행을 기준으로 하나씩 아래로 내린다.
                for (int i = nRowIndex + 1; i < nRowCount; i++)
                {
                    m_dicExternalTeamList[i - 1] = m_dicExternalTeamList[i];
                }

                m_dicExternalTeamList.Remove(nRowCount - 1);
                /////////////////////////////////////////////////////////////////
            }
        }

        // nRowIndex의 첫번째 Cell의 텍스트가 다른 행에 이미 존재하는지 여부를 확인한다.
        // 이미 존재하면 false, 존재하지 않으면 true를 리턴한다.
        private bool CheckDuplicate(DataGridView grid, int nRowIndex, string strValue)
        {
            int nRowCount = grid.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                if (i == nRowIndex)
                    continue;

                if (grid.Rows[i].Cells[0].Value.ToString() == strValue)
                    return false;
            }

            return true;
        }

        private void dataGridViewExternal_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            object value = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            Data_ExternalTeam team = m_dicExternalTeamList.ContainsKey(e.RowIndex) ? m_dicExternalTeamList[e.RowIndex] : null;

            if (e.ColumnIndex == 0)
            {
                if (team != null && !CheckDuplicate(grid, e.RowIndex, strValue))
                {
                    value = team.TeamName;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_ExternalTeam();
                        m_dicExternalTeamList[e.RowIndex] = team;
                    }

                    // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                    team.TeamName = strValue;
                    team.ID = -1;
                }
            }
            else if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                bool isCheck = FormMain.Instance.GetPageLevel().numericCheck(strValue);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");

                    if (team == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == 1 ? team.PhoneNumber : team.FaxNumber;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_ExternalTeam();
                        m_dicExternalTeamList[e.RowIndex] = team;
                    }

                    if (e.ColumnIndex == 1)
                        team.PhoneNumber = strValue;
                    else
                        team.FaxNumber = strValue;
                }
            }
        }

        private void dataGridViewUserDefined_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            object value = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            Data_UserDefinedTeam team = m_dicUserDefinedTeamList.ContainsKey(e.RowIndex) ? m_dicUserDefinedTeamList[e.RowIndex] : null;

            if (e.ColumnIndex == 0)
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
                    }

                    // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                    team.TeamName = strValue;
                    team.ID = -1;
                }
            }
            else if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                bool isCheck = FormMain.Instance.GetPageLevel().numericCheck(strValue);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");

                    if (team == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == 1 ? team.PhoneNumber : team.FaxNumber;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_UserDefinedTeam();
                        m_dicUserDefinedTeamList[e.RowIndex] = team;
                    }

                    if (e.ColumnIndex == 1)
                        team.PhoneNumber = strValue;
                    else
                        team.FaxNumber = strValue;
                }
            }
        }

        public int SelectedTeamType
        {
            get { return m_nSelectedTeamType; }
        }


        private void SetRadioImage()
        {
            if (radioTeam1.Checked == true)
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (radioTeam2.Checked == true)
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (radioTeam3.Checked == true)
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (radioTeam4.Checked == true)
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }


        }

        private void rdPictureBox1_Click(object sender, EventArgs e)
        {
            rdLabel1_Click(sender, e);
        }

        private void rdPictureBox2_Click(object sender, EventArgs e)
        {
            rdLabel2_Click(sender, e);
        }

        private void rdPictureBox3_Click(object sender, EventArgs e)
        {
            rdLabel3_Click(sender, e);
        }

        private void rdPictureBox4_Click(object sender, EventArgs e)
        {
            rdLabel4_Click(sender, e);
        }

        private void rdLabel1_Click(object sender, EventArgs e)
        {
            if (radioTeam1.Checked == false)
            {
                radioTeam1.Checked = !radioTeam1.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel2_Click(object sender, EventArgs e)
        {
            if (radioTeam2.Checked == false)
            {
                radioTeam2.Checked = !radioTeam2.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel3_Click(object sender, EventArgs e)
        {
            if (radioTeam3.Checked == false)
            {
                radioTeam3.Checked = !radioTeam3.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel4_Click(object sender, EventArgs e)
        {
            if (radioTeam4.Checked == false)
            {
                radioTeam4.Checked = !radioTeam4.Checked;
                SetRadioImage();
            }
        }

        private void PopupSelectTeam2_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectTeam2_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupSelectTeam2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }
}
