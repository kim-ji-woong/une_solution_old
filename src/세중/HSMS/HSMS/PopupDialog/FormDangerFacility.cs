using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace HSMS
{
    public partial class FormDangerFacility : Form
    {
        private Dictionary<string, DataEquip> m_dicGridData = new Dictionary<string, DataEquip>();
        private DBConn m_ConnectionHSMS = null;
        private ArrayList m_arrTempDataGrid = null;
        private ArrayList m_arrDBData = null;
        private DataManager m_DataMgr = null;
        
        public FormDangerFacility()
        {
            InitializeComponent();
            m_ConnectionHSMS = new DBConn("HSMS");
            m_DataMgr = FormMain.Instance.DataMgr;

            m_arrDBData = FormMain.Instance.DataMgr.GetEquips();
        }

        private void FormDangerFacility_Load(object sender, EventArgs e)
        {
            SetGridView();
            SetEquipTreeNode();
            LoadDataGridView();
            LoadEquipmentGroup();
        }

        private void LoadEquipmentGroup()
        {
            cmbEquipGroup.Items.Clear();

            DataManager dataMgr = FormMain.Instance.DataMgr;
            int nGroupCount = dataMgr.GetEquipmentGroupCount();

            for (int i=0;i<nGroupCount;i++)
            {
                EquipmentGroup group = dataMgr.GetEquipmentGroup(i);
                cmbEquipGroup.Items.Add(group);
            }

            if (nGroupCount > 0)
                cmbEquipGroup.SelectedIndex = 0;
        }

        private void SetGridView()
        {
            for (int i = 0; i < gridMember.Columns.Count; i++)
            {
                gridMember.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < gridManager.Columns.Count; i++)
            {
                gridManager.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            gridMember.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridMember.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        
        private void LoadDataGridView()
        {
            int nCount = 0;
            
            gridManager.Rows.Clear();

            m_arrTempDataGrid = FormMain.Instance.DataMgr.GetEquips();            
            foreach(DataEquip equip in m_arrTempDataGrid)
            {
                nCount++;
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell0 = new DataGridViewTextBoxCell();
                cell0.Value = nCount;
                row.Cells.Add(cell0);

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = equip.Name;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cellGroup = new DataGridViewTextBoxCell();
                cellGroup.Value = equip.EquipmentGroup == null ? EquipmentGroup.DefaultEquipmentGroup : equip.EquipmentGroup;
                row.Cells.Add(cellGroup);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = equip.Standard;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = equip.Maker;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = equip.Number;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = equip.TypeName;
                row.Cells.Add(cell5);

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = equip.Sensor;
                row.Cells.Add(cell6);

                row.Tag = equip;

                gridManager.Rows.Add(row);

                m_dicGridData[equip.Code] = equip;
            }
        }

        private void SetEquipTreeNode()
        {
                treeViewTeam.Nodes.Clear();
                TreeNode EquipTypeNode = null;
                Dictionary<string, DataEquip> EquipNames = ERPManager.Instance.DicEquips;

                foreach (KeyValuePair<string, DataEquip> pair in EquipNames)
                {
                    DataEquip equip = pair.Value;
                    EquipTypeNode = new TreeNode(equip.Name);
                    EquipTypeNode.Tag = equip;
                    treeViewTeam.Nodes.Add(EquipTypeNode);
    

                    //Dictionary<string, ArrayList> dicStandard = new Dictionary<string, ArrayList>();
                    //ArrayList arEquips = null;
                    //foreach (DataEquipStandard Equipstandard in equipName.EquipStandards)
                    //{
                    //    if (Equipstandard.Equips.Count == 0)
                    //        continue;
                        
                    //    if (!dicStandard.ContainsKey(Equipstandard.ID))
                    //    {
                    //        arEquips = new ArrayList();
                    //        dicStandard[Equipstandard.ID] = arEquips;
                    //    }
                    //    else
                    //    {
                    //        arEquips = (ArrayList)dicStandard[Equipstandard.ID];

                    //    }
                    //    arEquips.AddRange(Equipstandard.Equips);
                    //}
               
                    //if(dicStandard.ContainsKey(equipName.ID))
                    //{
                    //    EquipTypeNode.Tag = arEquips;
                    //}
           
                    
                    //foreach (KeyValuePair<string, ArrayList> p in dicStandard)
                    //{
                    //    string szStandard = p.Key;
                    //    ArrayList arEquips = p.Value;



                    //    TreeNode standardNode = new TreeNode(szStandard);
                    //    standardNode.Tag = arEquips;
                    //    EquipTypeNode.Nodes.Add(standardNode);
                    //}
                }

                Dictionary<string, DataEquip> equips = ERPManager.Instance.DicEquips;
                bool m_bFirst = true;
                TreeNode UnteamNode = null;
                foreach (KeyValuePair<string, DataEquip> pair in equips)
                {
                    DataEquip equip = pair.Value;
                    if (equip.EquipStandard == null)
                    {
                        if (m_bFirst == true)
                        {
                            m_bFirst = false;
                            UnteamNode = new TreeNode("Unknown Type");
                            EquipTypeNode.Nodes.Add(UnteamNode);
                            break;
                        }
                    }
                }
                treeViewTeam.ExpandAll();
            }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;

            TreeNode node = e.Node;
            //ArrayList arEquip = null;

            DataEquip equip = null;

            if (node.Text == "Unknown Team")
            {
                Dictionary<string, DataEquip> equips = ERPManager.Instance.DicEquips;
                foreach (KeyValuePair<string, DataEquip> pair in equips)
                {
                    DataEquip equip2 = pair.Value;
                    if (equip2.EquipName == null)
                    {
                        equip = equip2;
                    }
                }
            }
            else
            {
                object obj = node.Tag;
                //if (obj.GetType() == typeof(DataEquipName))
                //    return;

                equip = (DataEquip)obj;
            }

            Dictionary<string, DataEquip> dicDataCar = ERPManager.Instance.DicEquips;

            gridMember.Rows.Clear();

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = equip.Name;
            row.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = equip.Standard;
            row.Cells.Add(cell2);

            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell3.Value = equip.Maker;
            row.Cells.Add(cell3);

            DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
            cell4.Value = equip.TypeName;
            row.Cells.Add(cell4);

            DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
            cell5.Value = equip.Sensor;
            row.Cells.Add(cell5);

            row.Tag = equip;

            gridMember.Rows.Add(row);
        }

        private void treeViewTeam_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbEquipGroup.SelectedItem == null)
                return;

            EquipmentGroup group = (EquipmentGroup)cmbEquipGroup.SelectedItem;

            int nRowCount = gridManager.Rows.Count;
            int nCount = 0;
            if (gridManager.Rows.Count == 0)
                nCount = 0;
            else
                nCount = (int)gridManager.Rows[nRowCount - 1].Cells[0].Value;

            nCount++;

            DataGridViewSelectedRowCollection arRows = gridMember.SelectedRows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    bool isChecked = true;
                    DataGridViewRow row = arRows[i];
                    DataEquip equip = (DataEquip)row.Tag;
                    DataGridViewRow row2 = new DataGridViewRow();
                    row2.Tag = equip;

                    //중복검사
                    if (m_dicGridData.ContainsKey(equip.Code))
                    {
                        DataEquip data = m_dicGridData[equip.Code];

                        if (data == equip)
                            isChecked = false;
                    }

                    //센서아이디가 없는 데이터는 추가X
                    if (equip.Sensor.Trim() == "")
                    {
                        MessageBox.Show("센서아이디가 없는 데이터는 추가할 수 없습니다.");
                        isChecked = false;
                        continue;
                    }

                    //중복되면 추가안함
                    if (isChecked == false)
                    {
                        MessageBox.Show("중복된 데이터입니다..");
                        continue;
                    }
                    m_dicGridData[equip.Code] = equip;

                    DataGridViewTextBoxCell cell0 = new DataGridViewTextBoxCell();
                    cell0.Value = nCount;
                    row2.Cells.Add(cell0);

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = equip.Name;
                    row2.Cells.Add(cell1);

                    DataGridViewTextBoxCell cellGroup = new DataGridViewTextBoxCell();
                    cellGroup.Value = group;
                    row2.Cells.Add(cellGroup);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = equip.Standard;
                    row2.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = equip.Maker;
                    row2.Cells.Add(cell3);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = equip.Number;
                    row2.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                    cell5.Value = equip.TypeName;
                    row2.Cells.Add(cell5);

                    DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                    cell6.Value = equip.Sensor;
                    row2.Cells.Add(cell6);

                    gridManager.Rows.Add(row2);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection arRows = gridManager.SelectedRows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    
                    DataGridViewRow row = arRows[i];
                    gridManager.Rows.Remove(row);

                    DataEquip equip = (DataEquip)row.Tag;
                    m_dicGridData.Remove(equip.Code);

                   
                }
            }

            int nCount = 0;
            for (int i = 0; i < gridManager.Rows.Count; i++)
            {
                nCount++;
                gridManager.Rows[i].Cells[0].Value = nCount;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            /*m_arrTempDataGrid.Clear();

            DataGridViewRowCollection arRows = gridManager.Rows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];

                    DataEquip equip = (DataEquip)row.Tag;
                    
                    m_arrTempDataGrid.Add(equip);
                }
            }

            UpdateChangeData(m_arrDBData, m_arrTempDataGrid);

            m_arrTempDataGrid.Clear();*/
            UpdateChangeData(m_arrDBData, gridManager.Rows);

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private DataEquip FindDataEquip(int nID, DataGridViewRowCollection rows, out EquipmentGroup group)
        {
            group = null;

            foreach (DataGridViewRow row in rows)
            {
                DataEquip equip = (DataEquip)row.Tag;

                if (equip == null)
                    continue;

                group = row.Cells[2].Value == null ? EquipmentGroup.DefaultEquipmentGroup : (EquipmentGroup)row.Cells[2].Value;

                if (equip.ID == nID)
                    return equip;
            }

            return null;
        }

        private ArrayList m_arrEditEquips = new ArrayList();

        private void UpdateChangeData(ArrayList arrOrigin, DataGridViewRowCollection rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                DataEquip equip = (DataEquip)row.Tag;

                if (equip == null)
                    continue;

                EquipmentGroup group = row.Cells[2].Value == null ? EquipmentGroup.DefaultEquipmentGroup : (EquipmentGroup)row.Cells[2].Value;

                if (equip.ID < 0)
                {
                    EditEquipment editEquipment = new EditEquipment();
                    editEquipment.Equip = equip;
                    editEquipment.Code = equip.Code;
                    editEquipment.EquipmentGroup = group;
                    editEquipment.SQLType = ChangedData.INSERT;

                    m_arrEditEquips.Add(editEquipment);
                }
            }

            foreach (DataEquip equip in arrOrigin)
            {
                EquipmentGroup group;
                DataEquip _equip = FindDataEquip(equip.ID, rows, out group);

                if (_equip == null)
                {
                    EditEquipment editEquipment = new EditEquipment();
                    editEquipment.Equip = equip;
                    editEquipment.SQLType = ChangedData.DELETE;

                    m_arrEditEquips.Add(editEquipment);
                }
                else 
                {
                    if (equip.EquipmentGroup != group)
                    {
                        EditEquipment editEquipment = new EditEquipment();
                        editEquipment.Equip = equip;
                        editEquipment.EquipmentGroup = group;
                        editEquipment.SQLType = ChangedData.UPDATE;

                        m_arrEditEquips.Add(editEquipment);
                    }
                }
            }

            UpdateDB(m_arrEditEquips);
        }

        /*private void UpdateChangeData(ArrayList arrOrigin, ArrayList arrCurrent)
        {
            foreach (DataEquip equip in arrCurrent)
            {
                if (equip.ID < 0)
                {

                    EditEquipment editEquipment = new EditEquipment();
                    editEquipment.Equip = equip;
                    editEquipment.Code = equip.Code;
                    editEquipment.SQLType = ChangedData.INSERT;

                    m_arrEditEquips.Add(editEquipment);

                }
            }

            foreach (DataEquip equip in arrOrigin)
            {
                if (FindDataEquip(equip.ID, arrCurrent) == null)
                {
                    EditEquipment editEquipment = new EditEquipment();
                    editEquipment.Equip = equip;
                    editEquipment.SQLType = ChangedData.DELETE;

                    m_arrEditEquips.Add(editEquipment);
                }
            }
            UpdateDB(m_arrEditEquips);
        }*/

        private void UpdateDB(ArrayList arr)
        {
            ArrayList arrDeletes = new ArrayList();

            foreach (EditEquipment editEquipment in arr)
            {
                //데이터 넣었다가 뺀거면 값을 바꿀 필요가 없음
                if (editEquipment.ID < 0)
                {
                    if (editEquipment.SQLType == ChangedData.DELETE)
                    {
                        arrDeletes.Add(editEquipment);
                    }
                }
            }

            foreach(EditEquipment editequip in arrDeletes)
            {
                arr.Remove(editequip);
            }

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)ChangeDataType.EDIT_EQUIPMENT);

            foreach (EditEquipment editEquipment in arr)
            {
                editEquipment.Datas = arrDatas;

                if (!editEquipment.Update(m_ConnectionHSMS))
                    return;
            }

            if (arrDatas.Count > 1)
            {
                byte[] bytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA_LIST, arrDatas);
                FormMain.Instance.NetMgr.Send(bytes, FormMain.Instance.NetMgr.ClientProvider);
            }
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            FormAddGroup frm = new FormAddGroup();

            frm.SetGridHeader("설비그룹 이름");
            frm.SetTitle("설비그룹 추가");
            frm.DefGroupName = EquipmentGroup.DefaultEquipmentGroup.GroupName;
            frm.DefGroupNickName = EquipmentGroup.DefaultEquipmentGroup.ToString();

            foreach (EquipmentGroup group in cmbEquipGroup.Items)
            {
                frm.AddGroupName(group.ToString());
            }

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                EquipmentGroup group = new EquipmentGroup(frm.NewGroupName);
                cmbEquipGroup.Items.Add(group);
                cmbEquipGroup.SelectedItem = group;

                FormMain.Instance.DataMgr.AddEquipmentGroup(group);
            }
        }

        private void gridManager_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Point pt = gridManager.PointToClient(Cursor.Position);
                contextMenuStrip1.Show(gridManager, pt);
            }
        }

        private void editEquipGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridManager.SelectedCells.Count == 0)
                return;

            int nRowIndex = gridManager.SelectedCells[0].RowIndex;
            DataGridViewRow row = gridManager.Rows[nRowIndex];

            if (row != null && row.Tag != null)
            {
                DataEquip equip = (DataEquip)row.Tag;
                FormEditEquipGroup frm = new FormEditEquipGroup();

                frm.EquipName = equip.Name;

                if (equip.EquipmentGroup != null)
                    frm.GroupName = equip.EquipmentGroup.ToString();

                foreach (EquipmentGroup group in cmbEquipGroup.Items)
                {
                    frm.AddGroupName(group.ToString());
                }

                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    EquipmentGroup group = FormMain.Instance.DataMgr.FindEquipmentGroup(frm.GroupName);
                    RefreshRow(row, group);
                }
            }
        }

        private void RefreshRow(DataGridViewRow row, EquipmentGroup group)
        {
            DataEquip equip = (DataEquip)row.Tag;

            if (equip == null)
                return;

            row.Cells[2].Value = group == null ? EquipmentGroup.DefaultEquipmentGroup : group;
        }
    }
}
