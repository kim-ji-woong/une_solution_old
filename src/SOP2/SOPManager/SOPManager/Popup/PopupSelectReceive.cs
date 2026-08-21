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
    public partial class PopupSelectReceive : Form
    {
        ArrayList m_arrExternalTeam = new ArrayList();
        ArrayList m_arrRcvPhone = new ArrayList();
        ArrayList m_arrRcvFax = new ArrayList();
        ArrayList m_arrOriginTeam = new ArrayList();

        // 새로 추가되거나 변경된 것을 포함한 Data_ExternalTeam List
        // Grid Row Index, 행별 Data_ExternalTeam
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeamList = new Dictionary<int, Data_ExternalTeam>();
        // 삭제될 Data_ExternalTeam List
        private ArrayList m_arrRemoveExternalTeamList = new ArrayList();

        private int m_nItemID;

        private IPropertyExternal m_propertyExternal = null;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupSelectReceive(IPropertyExternal propertyExternal)
        {
            InitializeComponent();

            FormMain.Instance.ReadExternalTeam();
            m_arrExternalTeam = FormMain.Instance.ExternalTeam;

            m_propertyExternal = propertyExternal;
        }

        public void InitGrid(int nID, ArrayList arr)
        {
            m_nItemID = nID;
            
            AddSelectedTeam(arr);

            foreach (Data_ExternalTeam data in m_arrExternalTeam)
            {
                if (arr.Count == 0)
                {
                    AllExternalTeam(data);
                }
                else
                {
                    bool isCheck = false;
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if ((int)row.Tag == data.ID)
                        {
                            isCheck = true;
                            break;
                        }
                    }

                    if(isCheck)
                    {
                        continue;
                    }
                    else
                    {
                        AllExternalTeam(data);
                    }
                }
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

            if (dataGridViewReceive.AllowUserToAddRows)
                m_dicExternalTeamList[dataGridViewReceive.Rows.Count - 1] = team;
            else
                m_dicExternalTeamList[dataGridViewReceive.Rows.Count] = team;

            dataGridViewReceive.Rows.Add(gridRow);
        }

        private void AddSelectedTeam(ArrayList arr)
        {
            Image img = new Bitmap(global::SOPManager.Properties.Resources.call_18);

            foreach (Sections.ExternalTeamData exData in arr)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewImageCell ImageCell = new DataGridViewImageCell();
                ImageCell.Value = img;
                gridRow.Cells.Add(ImageCell);

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = exData.TeamName;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = exData.PhoneNumber;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = exData.FaxNumber;
                gridRow.Cells.Add(cell);

                gridRow.Tag = exData.TeamID;

                dataGridView.Rows.Add(gridRow);

                m_arrOriginTeam.Add(gridRow);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // DB 저장
            SaveReceiveList();

            //FormMain.Instance.GetPageLevel().GetPropertiesExternal().SelectedList = GetSelectedReceive(true);
            m_propertyExternal.SelectedList = GetSelectedReceive(true);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //FormMain.Instance.GetPageLevel().GetPropertiesExternal().SelectedList = GetSelectedReceive(false);
            m_propertyExternal.SelectedList = GetSelectedReceive(false);
            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Image img = new Bitmap(global::SOPManager.Properties.Resources.call_18);

            foreach (DataGridViewRow row in dataGridViewReceive.SelectedRows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                    continue;

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewImageCell ImageCell = new DataGridViewImageCell();
                ImageCell.Value = img;
                gridRow.Cells.Add(ImageCell);

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[0].Value.ToString();
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[1].Value.ToString();
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString();
                gridRow.Cells.Add(cell);

                // 새로 만든 데이터일 경우(row.Tag가 null일 경우) 새로 만들어진 행의 Index를 음수값으로 하여 tag로 삼는다.
                gridRow.Tag = row.Tag == null ? -row.Index : row.Tag;

                dataGridView.Rows.Add(gridRow);
            }

            foreach (DataGridViewRow row in dataGridViewReceive.SelectedRows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                    continue;
                if (row.Cells[0].Selected && row.Index < dataGridViewReceive.RowCount-1)
                {
                    dataGridViewReceive.Rows.Remove(row);
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[1].Value.ToString();
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[2].Value.ToString();
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = row.Cells[3].Value.ToString();
                gridRow.Cells.Add(cell);

                if (row.Tag != null && (int)row.Tag > 0)
                    gridRow.Tag = row.Tag;

                dataGridViewReceive.Rows.Add(gridRow);

                Sections.ExternalTeamData data = new Sections.ExternalTeamData();

                data.TeamID = row.Tag == null ? -1 : (int)row.Tag;
                data.TeamName = row.Cells[1].Value.ToString();
                data.PhoneNumber = row.Cells[2].Value.ToString();
                data.FaxNumber = row.Cells[3].Value.ToString();

                //FormMain.Instance.GetPageLevel().GetPropertiesExternal().SetRemoveReceive(m_nItemID, data);
                m_propertyExternal.SetRemoveReceive(m_nItemID, data);
            }

            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (row.Cells[0].Selected)
                {
                    dataGridView.Rows.Remove(row);
                }
            }
        } 

        private string GetSelectedReceive(bool isOK)
        {
            int nRow = 0;
            string strValue = "";

            if (dataGridView.RowCount != 0)
            {
                if (isOK)
                {
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        strValue += row.Cells[1].Value.ToString();
                        if (nRow != dataGridView.RowCount - 1)
                        {
                            strValue += ", ";
                            nRow++;
                        }

                        Sections.ExternalTeamData data = new Sections.ExternalTeamData();

                        data.TeamID = row.Tag == null ? -1 : (int)row.Tag;
                        data.TeamName = row.Cells[1].Value.ToString();
                        data.PhoneNumber = row.Cells[2].Value.ToString();
                        data.FaxNumber = row.Cells[3].Value.ToString();

                        //FormMain.Instance.GetPageLevel().GetPropertiesExternal().SetAddReceive(m_nItemID, data);
                        m_propertyExternal.SetAddReceive(m_nItemID, data);
                    }
                }
                else
                {
                     foreach (DataGridViewRow row in m_arrOriginTeam)
                     {
                         strValue += row.Cells[1].Value.ToString();
                         if (nRow != m_arrOriginTeam.Count - 1)
                         {
                             strValue += ", ";
                             nRow++;
                         }

                         Sections.ExternalTeamData data = new Sections.ExternalTeamData();

                         data.TeamID = row.Tag == null ? -1 : (int)row.Tag;
                         data.TeamName = row.Cells[1].Value.ToString();
                         data.PhoneNumber = row.Cells[2].Value.ToString();
                         data.FaxNumber = row.Cells[3].Value.ToString();

                         //FormMain.Instance.GetPageLevel().GetPropertiesExternal().SetAddReceive(m_nItemID, data);
                         m_propertyExternal.SetAddReceive(m_nItemID, data);
                     }
                }
            }

            return strValue;
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

        private void SaveReceiveList()
        {
            // Row Index, ExternalTeam
            Dictionary<int, Data_ExternalTeam> dicNewTeam = new Dictionary<int, Data_ExternalTeam>();
            //ArrayList arrNewTeam = new ArrayList();
            ArrayList arrUpdateTeam = new ArrayList();

            foreach (KeyValuePair<int, Data_ExternalTeam> pair in m_dicExternalTeamList)
            {
                int nResult = CheckExternalTeam(pair.Value);

                if (nResult == 1)
                    arrUpdateTeam.Add(pair.Value);
                else if (nResult == -1)
                {
                    dicNewTeam[pair.Key] = pair.Value;
                    //arrNewTeam.Add(pair.Value);
                }
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

            //foreach (Data_ExternalTeam team in arrNewTeam)
            foreach (KeyValuePair<int, Data_ExternalTeam> pair in dicNewTeam)
            {
                int nRowIndex = pair.Key;
                Data_ExternalTeam team = pair.Value;

                string strFaxNumber = team.FaxNumber.Length > 0 ? "'" + team.FaxNumber + "'" : "NULL";

                strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber) values ({0}, '{1}', '{2}', {3})",
                    ++nTeamID, team.TeamName, team.PhoneNumber, strFaxNumber);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                SetNewExternalTeamID(nRowIndex, nTeamID);
            }

            foreach (Data_ExternalTeam team in arrUpdateTeam)
            {
                string strFaxNumber = team.FaxNumber.Length > 0 ? "'" + team.FaxNumber + "'" : "NULL";

                strSQL = string.Format("Update ExternalTeam set PhoneNumber = '{0}', FaxNumber = {1} where id = {2}", 
                    team.PhoneNumber, strFaxNumber, team.ID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }

            /*ArrayList arrList = new ArrayList();

            if (dataGridViewReceive.RowCount != 0)
            {
                foreach (DataGridViewRow row in dataGridViewReceive.Rows)
                {
                    if(row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        Data_ExternalTeam data = new Data_ExternalTeam();
                        data.TeamName = row.Cells[0].Value.ToString();
                        data.PhoneNumber = row.Cells[1].Value.ToString();
                        data.FaxNumber = row.Cells[2].Value.ToString();
                        arrList.Add(data);
                    }
                }
            }
            if (dataGridView.RowCount != 0)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    Data_ExternalTeam data = new Data_ExternalTeam();
                    data.TeamName = row.Cells[1].Value.ToString();
                    data.PhoneNumber = row.Cells[2].Value.ToString();
                    data.FaxNumber = row.Cells[3].Value.ToString();
                    arrList.Add(data);
                }
            }

            if (arrList.Count == m_arrExternalTeam.Count) return;

            bool isCheck = false;
            foreach (Data_ExternalTeam newData in arrList)
            {
                foreach (Data_ExternalTeam data in m_arrExternalTeam)
                {
                    if (data.PhoneNumber == newData.PhoneNumber)
                    {
                        isCheck = true;
                    }
                }
                if (!isCheck)
                {
                    FormMain.Instance.SaveExternalTeam(newData.TeamName);
                }
                isCheck = false;
            }*/
        }

        private void SetNewExternalTeamID(int nRowIndex, int nNewID)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if ((int)row.Tag == -nRowIndex)
                {
                    row.Tag = nNewID;
                    return;
                }
            }
        }

        private void dataGridViewReceive_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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

        private void dataGridViewReceive_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewReceive)
                    return;

                if (dataGridViewReceive.SelectedRows == null || dataGridViewReceive.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewReceive.Rows.Count;
                if (dataGridViewReceive.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewReceive.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                if (!m_dicExternalTeamList.ContainsKey(nRowIndex))
                {
                    if (dataGridViewReceive.SelectedRows[0].Tag != null)
                    {
                        int nExternalTeamID = (int)dataGridViewReceive.SelectedRows[0].Tag;

                        if (nExternalTeamID > 0)
                        {
                            Data_ExternalTeam team = new Data_ExternalTeam();
                            team.ID = nExternalTeamID;
                            m_arrRemoveExternalTeamList.Add(team);
                        }
                    }

                    dataGridViewReceive.Rows.RemoveAt(nRowIndex);
                    return;
                }

                dataGridViewReceive.Rows.RemoveAt(nRowIndex);

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

        private void PopupSelectReceive_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectReceive_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupSelectReceive_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
