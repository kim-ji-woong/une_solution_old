using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;
using System.Drawing;

namespace KpxUserAcceptance
{
    public class UserGroupManager : TabControlManager
    {
        // 유종, 재고, 레벨, 레벨한계, 비중, 유량, 온도, 온도 범위, 배관알람, 탱크알람, 공지사항 여부
        private enum TankItem { LIQUID_TYPE = 1, MASS, LEVEL, LEVEL_RANGE, GRAVITY, FLOW, TEMP, TEMP_RANGE, TANK_ALARM, NOTICE, LEAK };
        // 배관 알람
        private enum PipeItem { PIPE_ALARM = 1};

        // Key : Tank ID
        // Value : Tank Column
        private Dictionary<int, DataGridViewColumn> m_dicTankIDColumn = new Dictionary<int, DataGridViewColumn>();
        private string m_strRemoveUserGroupIDs = "";
        private ContextMenuStrip m_contextMenu = null;

        public ContextMenuStrip ContextMenu
        {
            set
            {
                m_contextMenu = value;
                InitMenuHandler();
            }
        }

        protected override void InitGridHandler()
        {
            m_grid.RowsAdded += gridRowsAdded;
            m_grid.KeyDown += gridKeyDown;
            m_grid.MouseClick += gridMouseClick;
        }

        private void InitMenuHandler()
        {
            m_contextMenu.Items[0].Click += tsMenuSelectAllTankItems_Click;
            m_contextMenu.Items[1].Click += tsMenuUnselectAllTankItems_Click;
            m_contextMenu.Items[2].Click += tsMenuSelectAllTanks_Click;
            m_contextMenu.Items[3].Click += tsMenuUnselectAllTanks_Click;
            m_contextMenu.Items[4].Click += tsMenuSelectedUserGroups_Click;
        }

        public void gridRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            int nRowCount = grid.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = grid.Rows[i];
                row.Cells[0].ReadOnly = true;

                if (row.IsNewRow)
                    continue;

                row.Cells[0].Value = i + 1;
            }
        }

        public void gridKeyDown(object sender, KeyEventArgs e)
        {
            if (m_grid.ReadOnly)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteUserGroups();
            }
        }

        private void DeleteUserGroups()
        {
            List<int> rowIndices = new List<int>();

            foreach (DataGridViewCell cell in m_grid.SelectedCells)
            {
                if (rowIndices.Contains(cell.RowIndex))
                    continue;

                if (cell.OwningRow == null)
                    continue;

                rowIndices.Add(cell.RowIndex);

                if (cell.OwningRow.Tag != null)
                {
                    int nUserGroupID = (int)cell.OwningRow.Tag;

                    if (m_strRemoveUserGroupIDs.Length == 0)
                        m_strRemoveUserGroupIDs = nUserGroupID.ToString();
                    else
                        m_strRemoveUserGroupIDs += ", " + nUserGroupID.ToString();
                }
            }

            rowIndices.Sort();

            if (rowIndices.Count == 0)
                return;

            if (MessageBox.Show("선택된 사용자 그룹을 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo)
                == System.Windows.Forms.DialogResult.No)
                return;

            int nRowCount = rowIndices.Count;

            for (int i = nRowCount - 1; i >= 0; i--)
            {
                m_grid.Rows.RemoveAt(rowIndices[i]);
            }

            nRowCount = m_grid.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = m_grid.Rows[i];

                if (row.IsNewRow)
                    continue;

                row.Cells[0].Value = i + 1;
            }
        }

        public void gridMouseClick(object sender, MouseEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (e.Button == System.Windows.Forms.MouseButtons.Right && grid.ReadOnly == false)
            {
                if (grid.SelectedCells.Count > 0)
                {
                    List<int> selectedRowIndices = new List<int>();

                    foreach (DataGridViewCell cell in grid.SelectedCells)
                    {
                        if (selectedRowIndices.Contains(cell.RowIndex) == false)
                            selectedRowIndices.Add(cell.RowIndex);
                    }

                    m_contextMenu.Tag = selectedRowIndices;
                    m_contextMenu.Show(grid, e.Location);
                }
            }
        }

        public void Save()
        {
            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[1].Value != null)
                {
                    string strValue = row.Cells[1].Value.ToString().Trim();

                    if (strValue.Length > 0)
                        continue;
                }

                row.Cells[1].Selected = true;
                MessageBox.Show("사용자 그룹명은 빈 칸으로 둘 수 없습니다.");
                return;
            }

            m_grid.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (m_strRemoveUserGroupIDs.Length > 0)
            {
                m_dbMgr.BeginBatch();

                if (ClearUserGroup(m_strRemoveUserGroupIDs) == false)
                {
                    m_dbMgr.BatchRollback();
                    goto RETURN_FAIL;
                }

                if (RemoveUserGroup(m_strRemoveUserGroupIDs) == false)
                {
                    m_dbMgr.BatchRollback();
                    goto RETURN_FAIL;
                }

                m_dbMgr.BatchCommit();
            }

            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Tag == null)
                    InsertUserGroup(row);
                else
                    UpdateUserGroup(row, (int)row.Tag);
            }

            m_grid.Cursor = System.Windows.Forms.Cursors.Arrow;
            MessageBox.Show("변경사항이 적용되었습니다.");
            return;

        RETURN_FAIL:
            m_grid.Cursor = System.Windows.Forms.Cursors.Arrow;
            MessageBox.Show("삭제된 사용자 그룹을 시스템에 적용하는 도중 오류가 발생되어 삭제가 정상적으로 진행되지 못하였습니다.");
        }

        private bool ClearUserGroup(string strUserGroupIDs)
        {
            string strSQL = "Update User set UserGroup = NULL where UserGroup in (" + strUserGroupIDs + ")";
            return m_dbMgr.GetBatchData(strSQL) != null;
        }

        private bool RemoveUserGroup(string strUserGroupIDs)
        {
            string strSQL = "Delete from UserGroup where ID in (" + strUserGroupIDs + ")";
            return m_dbMgr.GetBatchData(strSQL) != null;
        }

        private void UpdateUserGroup(DataGridViewRow row, int nUserGroupID)
        {
            string strTankAccess = GetTankAccess(row);
            string strPipeAccess = row.Cells[2].Value == null || (bool)row.Cells[2].Value == false ? "0" : "1";
            string strTankItems = GetTankItems(row);
            string strPipeItems = GetPipeItems(row);

            string strFormat = "Update UserGroup set GroupName = '{0}', PipeAccess = {1}, PipeItems = '{5}', TankAccess = '{2}', TankItems = '{3}' where ID = {4}";
            string strSQL = string.Format(strFormat, row.Cells[1].Value.ToString(), strPipeAccess, strTankAccess, strTankItems, nUserGroupID, strPipeItems);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private void InsertUserGroup(DataGridViewRow row)
        {
            int nID = GetMaxID("UserGroup") + 1;
            string strTankAccess = GetTankAccess(row);
            string strPipeAccess = row.Cells[2].Value == null || (bool)row.Cells[2].Value == false ? "0" : "1";
            string strTankItems = GetTankItems(row);
            string strPipeItems = GetPipeItems(row);

            string strSQL = "Insert into UserGroup (ID, GroupName, PipeAccess, PipeItems, TankAccess, TankItems) values (";
            strSQL += string.Format("{0}, '{1}', {2}, '{3}', '{4}', '{5}')",
                nID, row.Cells[1].Value.ToString(), strPipeAccess, strPipeItems, strTankAccess, strTankItems);

            m_dbMgr.GetResultData(strSQL, 0);
        }

        private string GetTankItems(DataGridViewRow row)
        {
            int nPipeIndex = 3;
            int nTankItemCount = Enum.GetNames(typeof(TankItem)).Length;

            string strTankItems = "";

            for (int i = 1; i <= nTankItemCount; i++)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[nPipeIndex + i];

                if (cell.Value != null)
                {
                    bool isChecked = (bool)cell.Value;

                    if (isChecked)
                    {
                        if (strTankItems.Length == 0)
                            strTankItems = i.ToString();
                        else
                            strTankItems += ", " + i.ToString();
                    }
                }
            }

            return strTankItems;
        }

        private string GetPipeItems(DataGridViewRow row)
        {
            int nPipeIndex = 2;
            int nPipeItemCount = Enum.GetNames(typeof(PipeItem)).Length;

            string strTankItems = "";

            for (int i = 1; i <= nPipeItemCount; i++)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[nPipeIndex + i];

                if (cell.Value != null)
                {
                    bool isChecked = (bool)cell.Value;

                    if (isChecked)
                    {
                        if (strTankItems.Length == 0)
                            strTankItems = i.ToString();
                        else
                            strTankItems += ", " + i.ToString();
                    }
                }
            }

            return strTankItems;
        }

        private string GetTankAccess(DataGridViewRow row)
        {
            string strTankAccess = "";

            foreach (KeyValuePair<int, DataGridViewColumn> pair in m_dicTankIDColumn)
            {
                if (row.Cells[pair.Value.Index].Value != null)
                {
                    bool isChecked = (bool)row.Cells[pair.Value.Index].Value;

                    if (isChecked)
                    {
                        if (strTankAccess.Length == 0)
                            strTankAccess = pair.Key.ToString();
                        else
                            strTankAccess += ", " + pair.Key.ToString();
                    }
                }
            }

            return strTankAccess;
        }

        private int GetMaxID(string strTableName)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        public void Refresh()
        {
            m_strRemoveUserGroupIDs = "";
            m_grid.Rows.Clear();

            string strSQL = "Select ID, GroupName, PipeAccess, PipeItems, TankAccess, TankItems from UserGroup";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strGroupName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                DBUtility.VariousData<int> pipeAccess = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strPipeItems = DBUtility.WebDBManager.GetStringField(arrResult[i + 3]);
                string strTankAccess = DBUtility.WebDBManager.GetStringField(arrResult[i + 4]);
                string strTankItems = DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);

                if (id == null || strGroupName == null || pipeAccess == null || strTankAccess == null || strTankItems == null)
                    continue;

                int nRowIndex = m_grid.Rows.Add();

                if (nRowIndex < 0)
                    continue;

                DataGridViewRow row = m_grid.Rows[nRowIndex];
                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[1].Value = strGroupName;
                row.Cells[2].Value = pipeAccess.Data == 1 ? true : false;

                SetTankCells(row, strTankAccess);
                SetTankItems(row, strTankItems, 4);
                SetPipeItems(row, strPipeItems, 3);
                row.Tag = id.Data;
            }
        }

        private void SetTankItems(DataGridViewRow row, string strTankItems, int nTankItemBeginColumnIndex)
        {
            int nTankItemCount = Enum.GetNames(typeof(TankItem)).Length;

            for (int i = 0; i < nTankItemCount; i++)
            {
                row.Cells[i + nTankItemBeginColumnIndex].Value = false;
            }

            string[] tokens = strTankItems.Split(',');

            foreach (string strToken in tokens)
            {
                int nTankItem;

                if (int.TryParse(strToken.Trim(), out nTankItem))
                {
                    row.Cells[nTankItemBeginColumnIndex + nTankItem - 1].Value = true;
                }
            }
        }

        private void SetPipeItems(DataGridViewRow row, string strItems, int nTankItemBeginColumnIndex)
        {
            int nItemCount = Enum.GetNames(typeof(PipeItem)).Length;

            for (int i = 0; i < nItemCount; i++)
            {
                row.Cells[i + nTankItemBeginColumnIndex].Value = false;
            }

            if (strItems == null)
                return;

            string[] tokens = strItems.Split(',');

            foreach (string strToken in tokens)
            {
                int nTankItem;

                if (int.TryParse(strToken.Trim(), out nTankItem))
                {
                    row.Cells[nTankItemBeginColumnIndex + nTankItem - 1].Value = true;
                }
            }
        }

        private void SetTankCells(DataGridViewRow row, string strTankAccess)
        {
            foreach (KeyValuePair<int, DataGridViewColumn> pair in m_dicTankIDColumn)
            {
                row.Cells[pair.Value.Index].Value = false;
            }

            string[] tokens = strTankAccess.Split(',');

            foreach (string strToken in tokens)
            {
                int nTankID;

                if (int.TryParse(strToken.Trim(), out nTankID))
                {
                    DataGridViewColumn column = null;

                    if (m_dicTankIDColumn.TryGetValue(nTankID, out column))
                    {
                        row.Cells[column.Index].Value = true;
                    }
                }
            }
        }

        public void InitGrid(Color colHeader)
        {
            m_grid.ColumnHeadersDefaultCellStyle.BackColor = colHeader;

            string strSQL = "Select ID, Name from Tank order by Name";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strName == null || id == null)
                    continue;

                DataGridViewColumn column = new DataGridViewCheckBoxColumn();
                column.HeaderText = strName;
                column.Width = 60;

                m_dicTankIDColumn[id.Data] = column;
                m_grid.Columns.Add(column);
            }
        }

        public void tsMenuSelectAllTankItems_Click(object sender, EventArgs e)
        {
            List<int> selectedRowIndices = (List<int>)m_contextMenu.Tag;

            int nPipeIndex = 3;
            int nTankItemCount = Enum.GetNames(typeof(TankItem)).Length;

            foreach (int nRowIndex in selectedRowIndices)
            {
                DataGridViewRow row = m_grid.Rows[nRowIndex];

                for (int i = 1; i <= nTankItemCount; i++)
                {
                    row.Cells[nPipeIndex + i].Value = true;
                }
            }
        }

        public void tsMenuUnselectAllTankItems_Click(object sender, EventArgs e)
        {
            List<int> selectedRowIndices = (List<int>)m_contextMenu.Tag;

            int nPipeIndex = 3;
            int nTankItemCount = Enum.GetNames(typeof(TankItem)).Length;

            foreach (int nRowIndex in selectedRowIndices)
            {
                DataGridViewRow row = m_grid.Rows[nRowIndex];

                for (int i = 1; i <= nTankItemCount; i++)
                {
                    row.Cells[nPipeIndex + i].Value = false;
                }
            }
        }

        public void tsMenuSelectAllTanks_Click(object sender, EventArgs e)
        {
            List<int> selectedRowIndices = (List<int>)m_contextMenu.Tag;

            int nPipeIndex = 3;
            int nTankItemCount = Enum.GetNames(typeof(TankItem)).Length;
            int nColumnCount = m_grid.Columns.Count;

            foreach (int nRowIndex in selectedRowIndices)
            {
                DataGridViewRow row = m_grid.Rows[nRowIndex];

                for (int i = nPipeIndex + nTankItemCount + 1; i < nColumnCount; i++)
                {
                    row.Cells[i].Value = true;
                }
            }
        }

        public void tsMenuUnselectAllTanks_Click(object sender, EventArgs e)
        {
            List<int> selectedRowIndices = (List<int>)m_contextMenu.Tag;

            int nPipeIndex = 3;
            int nTankItemCount = Enum.GetNames(typeof(TankItem)).Length;
            int nColumnCount = m_grid.Columns.Count;

            foreach (int nRowIndex in selectedRowIndices)
            {
                DataGridViewRow row = m_grid.Rows[nRowIndex];

                for (int i = nPipeIndex + nTankItemCount + 1; i < nColumnCount; i++)
                {
                    row.Cells[i].Value = false;
                }
            }
        }

        public void tsMenuSelectedUserGroups_Click(object sender, EventArgs e)
        {
            DeleteUserGroups();
        }
    }
}
