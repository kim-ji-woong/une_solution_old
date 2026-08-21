using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using DBUtility;

namespace KpxUserAcceptance
{
    public class UserManager : TabControlManager
    {
        private string m_strRemoveUserIDs = "";
        private Dictionary<DataGridViewRow, UserGroup> m_dicUserRowGroup = new Dictionary<DataGridViewRow, UserGroup>();
        private ContextMenuStrip m_contextMenu = null;

        public ContextMenuStrip ContextMenu
        {
            set
            {
                m_contextMenu = value;
                InitMenuHandler();
            }
        }

        private void InitMenuHandler()
        {
            m_contextMenu.Items[0].Click += tsMenuSelectedUsers_Click;
        }

        public void InitGrid(Color colHeader)
        {
            //회원 편집
            SettingGridView(m_grid, "Id", "ID", colHeader);
            SettingGridView(m_grid, "UserName", "사용자명", colHeader);
            SettingGridView(m_grid, "PhoneNumber", "핸드폰 번호", colHeader);
            SettingGridView(m_grid, "SubPhoneNumber", "기존 핸드폰 번호", colHeader);

            DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "AlarmAuth";
            checkCol.HeaderText = "알람해제 가능 여부";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["AlarmAuth"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            checkCol.Width = 150;

            checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "SubAlarmAuth";
            checkCol.HeaderText = "기존 알람해제 가능 여부";
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            m_grid.Columns.Add(checkCol);

            checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "IsSms";
            checkCol.HeaderText = "문자 수신 여부";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["IsSms"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "SubIsSms";
            checkCol.HeaderText = "기존 문자 수신 여부";
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            m_grid.Columns.Add(checkCol);

            /*checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "Delete";
            checkCol.HeaderText = "삭제";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["Delete"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_grid.Columns["Delete"].Width = 50;*/

            DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
            comboCol.Name = "colUserGroup";
            comboCol.HeaderText = "사용자 그룹";
            comboCol.Sorted = false;
            comboCol.ReadOnly = false;
            comboCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            comboCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            UserGroup nullUserGroup = new UserGroup();
            nullUserGroup.ID = -1;
            nullUserGroup.GroupName = "없음";
            comboCol.Items.Add(nullUserGroup);

            DataGridViewCellStyle comboDefCellStyle = new DataGridViewCellStyle();
            comboDefCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            comboCol.DefaultCellStyle = comboDefCellStyle;

            m_grid.CellEndEdit += gridCellEndEdit;

            m_grid.Columns.Add(comboCol);
            comboCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            comboCol.Width = 60;

            m_grid.Columns["Id"].Visible = false;
            m_grid.Columns["SubPhoneNumber"].Visible = false;
            m_grid.Columns["SubAlarmAuth"].Visible = false;
            m_grid.Columns["SubIsSms"].Visible = false;
            m_grid.CellContentClick += gridCellContentClick;

            m_grid.MultiSelect = true;
            m_grid.AllowUserToDeleteRows = false;
            m_grid.KeyDown += gridKeyDown;
            m_grid.MouseClick += gridMouseClick;
        }

        private void DeleteUsers()
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

                    if (m_strRemoveUserIDs.Length == 0)
                        m_strRemoveUserIDs = nUserGroupID.ToString();
                    else
                        m_strRemoveUserIDs += ", " + nUserGroupID.ToString();
                }
            }

            rowIndices.Sort();

            if (rowIndices.Count == 0)
                return;

            if (MessageBox.Show("선택된 사용자를 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo)
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

        private void gridKeyDown(object sender, KeyEventArgs e)
        {
            if (m_grid.ReadOnly)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteUsers();
            }
        }

        private void gridMouseClick(object sender, MouseEventArgs e)
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

        void gridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_grid.Columns[e.ColumnIndex].Name == "AlarmAuth")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = false;
                else
                    chk.Value = true;
            }
            if (m_grid.Columns[e.ColumnIndex].Name == "IsSms")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = true;
                else
                    chk.Value = false;
            }
            if (m_grid.Columns[e.ColumnIndex].Name == "Delete")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = false;
                else
                    chk.Value = true;
            }
        }

        public void Refresh()
        {
            try
            {
                m_strRemoveUserIDs = "";
                m_dicUserRowGroup.Clear();
                m_grid.Rows.Clear();
                string strQuery = "SELECT ID, UserName, PhoneNumber, MobileUserLevel, IsSms, UserGroup FROM User WHERE Mobile=1";

                ArrayList arrResult = m_dbMgr.GetResultData(strQuery, 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count - 5; i += 6)
                {
                    int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strUserName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                    string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                    int nMobileUserLevel = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nIsSms = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nUserGroupID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                    string strDECPhoneNumber = string.Empty;
                    if (strPhoneNumber.Length > 0)
                        strDECPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                    UserGroup group = GetUserGroup(nUserGroupID);
                    int nRowIndex = m_grid.Rows.Add(nID, strUserName, strDECPhoneNumber, strDECPhoneNumber, (nMobileUserLevel == 1) ? false : true, (nMobileUserLevel == 1) ? false : true, (nIsSms == 1) ? true : false, (nIsSms == 1) ? true : false, group);

                    if (nRowIndex >= 0)
                    {
                        DataGridViewRow row = m_grid.Rows[nRowIndex];
                        row.Tag = nID;
                    }

                    m_dicUserRowGroup[m_grid.Rows[nRowIndex]] = group;
                }
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void ReadUserGroups()
        {
            m_grid.Rows.Clear();

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)m_grid.Columns[m_grid.Columns.Count - 1];
            int nItemCount = col.Items.Count;

            // 첫번째 아이템은 null data
            for (int i = nItemCount - 1; i >= 1; i--)
            {
                col.Items.RemoveAt(i);
            }

            string strSQL = "Select ID, GroupName from UserGroup";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strGroupName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strGroupName == null)
                    continue;

                UserGroup group = new UserGroup();
                group.ID = id.Data;
                group.GroupName = strGroupName;

                col.Items.Add(group);
            }
        }

        public void Save()
        {
            try
            {
                int chgCnt = 0;
                List<string> querys = new List<string>();

                foreach (DataGridViewRow row in m_grid.Rows)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    string phoneNumber = row.Cells["PhoneNumber"].Value.ToString();
                    string subPhoneNumber = row.Cells["SubPhoneNumber"].Value.ToString();
                    DataGridViewCheckBoxCell alarmAuthRow = row.Cells["AlarmAuth"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell subAlarmAuthRow = row.Cells["SubAlarmAuth"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell IsSmsRow = row.Cells["IsSms"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell subIsSmsRow = row.Cells["SubIsSms"] as DataGridViewCheckBoxCell;
                    //DataGridViewCheckBoxCell DeleteRow = row.Cells["Delete"] as DataGridViewCheckBoxCell;
                    DataGridViewComboBoxCell userGroup = row.Cells["colUserGroup"] as DataGridViewComboBoxCell;

                    /*if (Convert.ToBoolean(DeleteRow.Value))
                    {
                        querys.Add("DELETE FROM User WHERE id = " + id);
                        chgCnt++;
                    }
                    else*/
                    {
                        UserGroup _group = null;
                        bool changedUserGroup = m_dicUserRowGroup.TryGetValue(row, out _group);

                        string strUserGroup = "NULL";

                        if (changedUserGroup)
                        {
                            changedUserGroup = false;

                            if (userGroup.Value is UserGroup)
                            {
                                UserGroup group = (UserGroup)userGroup.Value;

                                if (group.ID > 0)
                                {
                                    strUserGroup = group.ID.ToString();
                                }

                                if (group != _group)
                                    changedUserGroup = true;
                            }
                        }

                        if (subPhoneNumber == phoneNumber && Convert.ToBoolean(subAlarmAuthRow.Value) == Convert.ToBoolean(alarmAuthRow.Value)
                                                          && Convert.ToBoolean(subIsSmsRow.Value) == Convert.ToBoolean(IsSmsRow.Value)
                                                          && changedUserGroup == false) continue;

                        if (phoneNumber.Length == 0) throw new ApplicationException("핸드폰 번호를 입력하세요.");

                        querys.Add(string.Format("UPDATE User SET PhoneNumber='{0}', MobileUserLevel={1}, IsSms={2}, UserGroup = {3} WHERE ID =" + id
                            , DBUtility.AES256Cipher.AES_encrypt(phoneNumber, key), (Convert.ToBoolean(alarmAuthRow.Value)) ? 0 : 1, (Convert.ToBoolean(IsSmsRow.Value)) ? 1 : 0, strUserGroup));

                        chgCnt++;
                    }
                }

                if (m_strRemoveUserIDs.Length > 0)
                {
                    string[] tokens = m_strRemoveUserIDs.Split(',');

                    if (tokens.Count() > 0)
                    {
                        querys.Add("Delete from User where ID in (" + m_strRemoveUserIDs + ")");
                        chgCnt += tokens.Count();
                    }
                }

                foreach (string item in querys)
                {
                    m_dbMgr.GetResultData(item, 0);
                }

                if (chgCnt > 0)
                {
                    MessageBox.Show(chgCnt + "건이 수정되었습니다.");
                }

                else
                    MessageBox.Show("변경할 내용이 없습니다.");
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void tsMenuSelectedUsers_Click(object sender, EventArgs e)
        {
            DeleteUsers();
        }
    }
}
