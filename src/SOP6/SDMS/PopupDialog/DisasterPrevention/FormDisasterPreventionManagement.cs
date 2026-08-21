using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using UnE.Spatial;

namespace SDMS.PopupDialog.DisasterPrevention
{
    public partial class FormDisasterPreventionManagement : Form, IChildControl
    {
        private enum Mode { SEARCH, EDIT }
        private Mode m_CurMode = Mode.SEARCH;

        private static FormDisasterPreventionManagement m_Instance = null;
        public static FormDisasterPreventionManagement Instance
        {
            get { return m_Instance; }
        }
        private WebDBManager m_dbMgr = null; 
        public static int DockingWidth { get { return 685; } }

        private List<DisasterPrevention> m_DPList = new List<DisasterPrevention>();
        private List<DisasterType> m_TypeList = new List<DisasterType>();
        
        private DateTimePicker m_CellDateTimePicker = null;
        private FormLocationCfg m_FormLocationCfg = null;
        
        private List<DisasterPrevention> m_DeleteDP = new List<DisasterPrevention>();        
        private bool m_bIsEdit = false; // 수정된 항목이 있는지

        private ArrayList SaveArr = new ArrayList();
        private string m_strLogoFileName = string.Empty;

        #region IChildControl 멤버

        public void OnAdded(Control parent)
        {
             
        }

        public void OnRemoved(Control parent)
        {
            
        }

        #endregion
         
        #region 초기화
        public FormDisasterPreventionManagement()
        {
            m_Instance = this;
            this.DoubleBuffered = true;

            InitializeComponent(); 
             
            m_dbMgr = FormMain.Instance.DBManager;

            m_strLogoFileName = GetReportLogoFileName();
            DisplayType();
            InitGridView();
            DisplayDB();
            DisplayGridView();
        } 

        private void InitGridView()
        { 
            ColDevName.DisplayMember = "Name";

            foreach (DisasterType item in m_TypeList)
            {
                ColDevName.Items.Add(item);
            } 

            ColCheckCycle.Items.Add("수시");
            ColCheckCycle.Items.Add("매일");
            ColCheckCycle.Items.Add("매주");
            ColCheckCycle.Items.Add("매달");
            ColCheckCycle.Items.Add("매년"); 

            ColCheckWay.Items.Add("육안검사"); 

            m_CellDateTimePicker = new DateTimePicker();
            m_CellDateTimePicker.ValueChanged += new EventHandler(cellDateTimePickerValueChanged);
            m_CellDateTimePicker.Visible = false;
            m_CellDateTimePicker.CustomFormat = "yyyy-MM-dd";
            m_CellDateTimePicker.Format = DateTimePickerFormat.Custom;
            dataGridView1.Controls.Add(m_CellDateTimePicker);
        }
        void cellDateTimePickerValueChanged(object sender, EventArgs e)
        {  
            m_CellDateTimePicker.Visible = false;
                        
            if (dataGridView1.CurrentCell.ColumnIndex != ColvalidityDate.Index)
                return;
             
            object cellValue = dataGridView1.CurrentCell.Value;
            object cellOrgTag = dataGridView1.CurrentCell.Tag;
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
             
            DisasterPrevention dp = dataGridView1.Rows[rowIndex].Tag as DisasterPrevention;
            if (dp == null)
            {
                dp = new DisasterPrevention();
                dp.RowIdx = rowIndex;
                m_DPList.Add(dp);
                dataGridView1.Rows[rowIndex].Tag = dp;
            }
            dp.RowIdx = rowIndex;

            string strCellOrgTag = "";
            if (cellOrgTag == null)
                strCellOrgTag = "";
            else
                strCellOrgTag = cellOrgTag.ToString();
            if (cellValue != null)
            {
                string chgCellValue = m_CellDateTimePicker.Value.ToString("yyyy-MM-dd");
                if (cellValue.ToString() != chgCellValue)
                {
                    dp.IsChg = true;
                    dp.Date = chgCellValue;
                    dp.DateOld = strCellOrgTag;

                    dataGridView1.CurrentCell.Value = chgCellValue; 
                } 
            } 
        } 
        #endregion

        #region GridView 이벤트
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; 

            if (row.Cells[ColNum.Index].Value == null || row.Cells[ColNum.Index].Value.ToString() == "")
            {
                row.Cells[ColNum.Index].Value = e.RowIndex + 1;
            }
        } 

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            object cellOrgTag = cell.Tag;
            object cellValue = cell.Value;
            DisasterPrevention dp = dataGridView1.Rows[e.RowIndex].Tag as DisasterPrevention;
            if (dp == null)
            {
                dp = new DisasterPrevention();
                dp.RowIdx = e.RowIndex;
                m_DPList.Add(dp);
                dataGridView1.Rows[e.RowIndex].Tag = dp;
            }
            dp.RowIdx = e.RowIndex;

            if (e.ColumnIndex == ColDevName.Index)
            {
                if (cellOrgTag != cellValue)
                { 
                    foreach (DisasterType item in m_TypeList)
                    {
                        if (item == cellValue as DisasterType || item.Name == cellValue.ToString())
                        {
                            dp.IsChg = true;
                            dp.Type = item;
                            dp.TypeOld = cell.Tag as DisasterType;
                            cell.Tag = item;
                            break;
                        }
                    }                    
                }
            }
            else if (e.ColumnIndex == ColLocation.Index)
            {
                if (cellValue == null)
                    return;

                EquipmentZone ezone = cellOrgTag as EquipmentZone;
                if (ezone == null || ezone.DisplayText != cellValue.ToString())
                {
                    dp.IsChg = true;
                    dp.EquipmentZone = ezone;
                    dp.EquipmentZoneOld = cell.Tag as EquipmentZone;
                    cell.Tag = ezone;
                } 
            }
            else if (e.ColumnIndex == ColStandardQuantity.Index || e.ColumnIndex == ColStatusQuantity.Index)
            {
                if (cellValue == null)
                    return;

                int quantity = -1;
                if (!int.TryParse(cellValue.ToString(), out quantity) || Convert.ToInt32(cellValue) < 0)
                {
                    cell.Value = 0;
                    FormDPMsgBox frm = new FormDPMsgBox("", "0이상의 숫자로 입력하세요", MessageBoxButtons.OK);
                    Point parentPt = this.Parent.Parent.PointToScreen(this.Parent.Parent.Location);
                    int x = (this.Size.Width / 2) - (frm.Size.Width / 2);
                    int y = (this.Size.Height / 2) - (frm.Size.Height / 2);
                    frm.Location = new Point(parentPt.X + x, parentPt.Y + y);
                    frm.StartPosition = FormStartPosition.Manual;
                    frm.ShowDialog(); 
                    return;
                }

                if (Convert.ToInt32(cellValue) != Convert.ToInt32(cellOrgTag))
                {
                    dp.IsChg = true;
                    if (e.ColumnIndex == ColStandardQuantity.Index)
                    {
                        dp.StandardQuantity = Convert.ToInt32(cellValue);
                        dp.StandardQuantityOld = Convert.ToInt32(cell.Tag);
                    }
                    else if (e.ColumnIndex == ColStatusQuantity.Index)
                    {
                        dp.StatusQuantity = Convert.ToInt32(cellValue);
                        dp.StatusQuantityOld = Convert.ToInt32(cell.Tag);
                    }
                    cell.Tag = Convert.ToInt32(cellValue);
                }
            }
            else if (e.ColumnIndex == ColCheckCycle.Index || e.ColumnIndex == ColCheckWay.Index)
            {
                if (cellOrgTag == null || cellValue.ToString() != cellOrgTag.ToString())
                { 
                    dp.IsChg = true;
                    if (e.ColumnIndex == ColCheckCycle.Index)
                    {
                        dp.CheckCycle = cellValue.ToString();
                        dp.CheckCycleOld = (cell.Tag == null) ? "" : cell.Tag.ToString();
                    }
                    else if (e.ColumnIndex == ColCheckWay.Index)
                    {
                        dp.CheckWay = cellValue.ToString();
                        dp.CheckWayOld = (cell.Tag == null) ? "" : cell.Tag.ToString();
                    }
                    cell.Tag = cellValue.ToString();
                }
            } 
            else if (e.ColumnIndex == ColvalidityDate.Index)
            {
                //string strCellOrgTag = "";
                //if (cellOrgTag == null)
                //    strCellOrgTag = "";
                //else
                //    strCellOrgTag = cellOrgTag.ToString();
                //if (cellValue != null || cellValue.ToString() != strCellOrgTag)
                //{
                //    dp.IsChg = true;
                //    dp.Date = cellValue.ToString();
                //    dp.DateOld = strCellOrgTag;
                //    cell.Tag = cellValue.ToString();
                //}
            }

            if (!m_bIsEdit)
                m_bIsEdit = true;
        } 

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // No
            //dataGridView1.Rows[e.RowIndex].Cells[ColNum.Index].Value = dataGridView1.Rows.Count;
            // 장비명
            //if (m_TypeList != null && m_TypeList.Count > 0)
            //    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[ColDevName.Index].Value = m_TypeList[0] as DisasterType;
            // 기준 수량
            dataGridView1.Rows[e.RowIndex].Cells[ColStandardQuantity.Index].Value = 0;
            // 현황 수량
            dataGridView1.Rows[e.RowIndex].Cells[ColStatusQuantity.Index].Value = 0;
            // 점검주기
            dataGridView1.Rows[e.RowIndex].Cells[ColCheckCycle.Index].Value =
                (dataGridView1.Rows[e.RowIndex].Cells[ColCheckCycle.Index] as DataGridViewComboBoxCell).Items[0];
            // 점검방법
            dataGridView1.Rows[e.RowIndex].Cells[ColCheckWay.Index].Value =
                (dataGridView1.Rows[e.RowIndex].Cells[ColCheckWay.Index] as DataGridViewComboBoxCell).Items[0];
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.FormattedValue.ToString().Length > 0)
            {
                if (e.ColumnIndex == ColDevName.Index)
                {
                    bool isFind = FindType(e.FormattedValue.ToString());
                    if (!isFind)
                    {
                        DisasterType type = new DisasterType();
                        type.Name = e.FormattedValue.ToString();
                        m_TypeList.Add(type);
                        ColDevName.Items.Add(type);

                        DataGridViewCell cell = dataGridView1.SelectedCells[0];
                        cell.Value = type;
                    }
                }
                else if (e.ColumnIndex == ColvalidityDate.Index)
                {
                    //string orgTag = (dataGridView1.CurrentCell.Tag == null) ? "" : dataGridView1.CurrentCell.Tag.ToString();
                    //dataGridView1.CurrentCell.Value = e.FormattedValue.ToString(); // m_CellDateTimePicker.Value.ToString("yyyy-MM-dd");
                    //dataGridView1.CurrentCell.Tag = dataGridView1.CurrentCell.Value.ToString();
                    //DisasterPrevention dp = dataGridView1.Rows[e.RowIndex].Tag as DisasterPrevention;
                    //if (dp == null)
                    //{
                    //    dp = new DisasterPrevention();
                    //    dp.RowIdx = e.RowIndex;
                    //    m_DPList.Add(dp);
                    //    dataGridView1.Rows[e.RowIndex].Tag = dp;
                    //}

                    //dp.Date = dataGridView1.CurrentCell.Value.ToString();
                    //dp.DateOld = orgTag;
                    //if (dp.Date != dp.DateOld)
                    //    dp.IsChg = true; 
                }
            }
        }

        //DataGridViewCell lastCell = null;
        //bool lastCellSelected = false;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_CurMode == Mode.SEARCH)
                return;

            if (e.RowIndex < 0)
                return;

            //if (m_CurMode == Mode.SEARCH)
            //{
            //    DataGridViewCell curCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //    if (lastCell == null)
            //        lastCell = curCell;
            //    if (lastCell == curCell)
            //    {
            //        if (lastCellSelected)
            //            curCell.Selected = false;  
            //        else
            //            curCell.Selected = true;
            //        lastCellSelected = curCell.Selected;
            //    }
            //    lastCell = curCell;                                 
            //    return;
            //}

            if (e.ColumnIndex == ColvalidityDate.Index)
            {
                Rectangle tempRect = this.dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                m_CellDateTimePicker.Location = tempRect.Location;
                m_CellDateTimePicker.Width = tempRect.Width;
                try
                {
                    m_CellDateTimePicker.Value = DateTime.Parse(dataGridView1.CurrentCell.Value.ToString());
                }
                catch
                {
                    m_CellDateTimePicker.Value = DateTime.Now;
                }

                string orgTag = (dataGridView1.CurrentCell.Tag == null) ? "" : dataGridView1.CurrentCell.Tag.ToString();
                dataGridView1.CurrentCell.Value = m_CellDateTimePicker.Value.ToString("yyyy-MM-dd");
                dataGridView1.CurrentCell.Tag = dataGridView1.CurrentCell.Value.ToString();
                DisasterPrevention dp = dataGridView1.Rows[e.RowIndex].Tag as DisasterPrevention;
                if (dp == null)
                {
                    dp = new DisasterPrevention();
                    dp.RowIdx = e.RowIndex;
                    m_DPList.Add(dp);
                    dataGridView1.Rows[e.RowIndex].Tag = dp;
                }
                
                dp.Date = dataGridView1.CurrentCell.Value.ToString();
                dp.DateOld = orgTag;
                if (dp.Date != dp.DateOld)
                    dp.IsChg = true;

                m_CellDateTimePicker.Visible = true;

                // 번호입력을 위해서
                dataGridView1_CellBeginEdit(dataGridView1, new DataGridViewCellCancelEventArgs(e.ColumnIndex, e.RowIndex));
            }

            //if (!dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                if (e.ColumnIndex == ColDevName.Index || e.ColumnIndex == ColCheckCycle.Index || e.ColumnIndex == ColCheckWay.Index)
                {
                    //string editedValue = dataGridView1.CurrentCell.EditedFormattedValue.ToString();
                    //if (e.ColumnIndex == ColCheckCycle.Index)
                    //{
                    //    if (editedValue == "수시" || editedValue == "매일" || editedValue == "매주" || editedValue == "매달" || editedValue == "매년")
                    //        return;
                    //}
                    //if (e.ColumnIndex == ColCheckWay.Index)
                    //{
                    //    if (editedValue == "육안검사")
                    //        return;
                    //}

                    dataGridView1.BeginEdit(true);

                    ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                    if (comboBox == null)
                        return;

                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;

                    if (comboBox.Tag == null)
                        comboBox.Leave += new EventHandler(comboBox_Leave);

                    comboBox.Tag = true;
                } 
            }
        }

        void comboBox_Leave(object sender, EventArgs e)
        { 
            ComboBox cbo = (ComboBox)sender;
            if (cbo.Tag == null)
                return;

            if (dataGridView1.SelectedCells.Count != 1)
                return;

            if (cbo.Text.Length > 0)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == ColDevName.Index)
                {
                    bool isFind = FindType(cbo.Text);
                    if (!isFind)
                    {
                        DisasterType type = new DisasterType();
                        type.Name = cbo.Text;
                        m_TypeList.Add(type);
                        ColDevName.Items.Add(type);

                        DataGridViewCell cell = dataGridView1.CurrentCell;
                        cell.Value = type; 
                    } 
                    else
                    {
                        foreach (DisasterType type in m_TypeList)
                        {
                            if (type.Name == cbo.Text)
                            {
                                DataGridViewCell cell = dataGridView1.CurrentCell;
                                cell.Value = type;
                                break;
                            }
                        }
                    }
                }
                else if (dataGridView1.CurrentCell.ColumnIndex == ColCheckCycle.Index)
                {
                    if (!ColCheckCycle.Items.Contains(cbo.Text.Trim())) 
                        ColCheckCycle.Items.Add(cbo.Text.Trim()); 

                    DataGridViewCell cell = dataGridView1.CurrentCell;
                    cell.Value = cbo.Text; 
                }
                else if (dataGridView1.CurrentCell.ColumnIndex == ColCheckWay.Index)
                {
                    if (!ColCheckWay.Items.Contains(cbo.Text.Trim()))
                        ColCheckWay.Items.Add(cbo.Text.Trim());

                    DataGridViewCell cell = dataGridView1.CurrentCell;
                    cell.Value = cbo.Text; 
                }

                dataGridView1.NotifyCurrentCellDirty(true);
                dataGridView1.NotifyCurrentCellDirty(false);
            }
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != ColvalidityDate.Index)
                return;

            if (m_CellDateTimePicker.Visible)
                m_CellDateTimePicker.Visible = false;
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (m_CurMode != Mode.EDIT)
                return;

            if (e.KeyCode == Keys.Escape)
            {
                if (!m_bIsEdit)
                {
                    SetMode(false);
                    return;
                } 

                if (RollBack())
                {
                    DisplayGridView();
                    SetMode(false);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                List<int> rowIndexs = new List<int>();
                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    if (!rowIndexs.Contains(cell.RowIndex))
                        rowIndexs.Add(cell.RowIndex);
                }

                int cnt = rowIndexs.Count;
                if (cnt <= 0)
                    return;

                FormDPMsgBox frm = new FormDPMsgBox("", "선택한 " + cnt +"개 항목을 삭제하시겠습니까?", MessageBoxButtons.YesNo);
                Point parentPt = this.Parent.Parent.PointToScreen(this.Parent.Parent.Location);
                int x = (this.Size.Width / 2) - (frm.Size.Width / 2);
                int y = (this.Size.Height / 2) - (frm.Size.Height / 2);
                frm.Location = new Point(parentPt.X + x, parentPt.Y + y);
                frm.StartPosition = FormStartPosition.Manual;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.No)
                    return; 

                DeleteRow();
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == ColvalidityDate.Index)
                {
                    DisasterPrevention dp = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Tag as DisasterPrevention;
                    if (dp == null)
                        return;

                    if (dp.Date == "")
                        return;

                    dp.Date = "";
                    dp.DateOld = dataGridView1.CurrentCell.Tag.ToString();
                    dp.IsChg = true;
                    dataGridView1.CurrentCell.Value = "";
                    dataGridView1.CurrentCell.Tag = "";

                    m_CellDateTimePicker.Visible = false;

                    m_bIsEdit = true;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ColLocation.Index)
            {
                if (m_CurMode == Mode.EDIT && m_FormLocationCfg == null)
                    ShowLocationCfg();
            }
        }
        #endregion
         
        #region 버튼이벤트
        private void button_edit_Click(object sender, EventArgs e)
        {
            SetMode();
        } 

        private void button_locationCfg_Click(object sender, EventArgs e)
        {
            ShowLocationCfg();
        }

        void m_FormLocationCfg_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_FormLocationCfg.Dispose();
            m_FormLocationCfg = null;

            if (m_CurMode == Mode.SEARCH)
                button_locationCfg.Enabled = false;
            else if (m_CurMode == Mode.EDIT)
                button_locationCfg.Enabled = true;
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            button_export.Enabled = false;
            SaveHWPForDPMgr();
            button_export.Enabled = true;
        } 
        #endregion

        #region 조회 함수
        private void DisplayType()
        {
            ArrayList arrResult = m_dbMgr.GetResultData("SELECT ID, Name FROM DisasterPreventionEquipmentType");
            if (arrResult == null)
                return;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);

                DisasterType type = new DisasterType();
                type.ID = nID;
                type.Name = strName;

                m_TypeList.Add(type);
            }
        }

        private bool FindType(string strName)
        {
            foreach (DisasterType item in m_TypeList)
            {
                if (item.Name == strName)
                    return true;
            }

            return false;
        } 

        private void DisplayDB()
        { 
            m_DPList.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT dpe.ID, dpe.TypeID, ez.ID, dpe.Name, StandardQuantity, Quantity, CheckCycle, CheckWay, ValidityDate, dpel.ID ");
            sb.Append("  FROM DisasterPreventionEquipment as dpe  ");
            //sb.Append(" INNER JOIN DisasterPreventionEquipmentType as dpet ON dpe.TypeID=dpet.ID ");
            sb.Append(" INNER JOIN DisasterPreventionEquipmentLocation as dpel ON dpe.locationID=dpel.ID ");
            sb.Append(" INNER JOIN EquipmentZone as ez ON ez.ID=dpel.LocationName ");

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int rowIndex = 0;
            for (int i = 0; i < arrResult.Count; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nEquipmentZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strTypeName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nStandardQuantity = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nStatusQuantity = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strCheckCycle = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strCheckWay = WebDBManager.GetStringField(arrResult[i + 7], "");
                string strValidityDate = WebDBManager.GetStringField(arrResult[i + 8], "");
                int nLocationID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                 
                DisasterPrevention dp = new DisasterPrevention();
                dp.ID = nID;
                dp.RowIdx = rowIndex;
                dp.LocationID = nLocationID;
                 
                foreach (DisasterType item in m_TypeList)
                {
                    if (item.ID == nTypeID)
                    { 
                        dp.Type = item;
                        dp.TypeOld = item;
                        break;
                    }
                }

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipmentZoneID);
                if (equipZone != null)
                {  
                    dp.EquipmentZone = equipZone;
                    dp.EquipmentZoneOld = equipZone;
                }
                  
                dp.StandardQuantity = nStandardQuantity;
                dp.StandardQuantityOld = nStandardQuantity;
                  
                dp.StatusQuantity = nStatusQuantity;
                dp.StatusQuantityOld = nStatusQuantity;
                  
                dp.CheckCycle = strCheckCycle;
                dp.CheckCycleOld = strCheckCycle;
                  
                dp.CheckWay = strCheckWay;
                dp.CheckWayOld = strCheckWay;
                  
                dp.Date = strValidityDate;
                dp.DateOld = strValidityDate;
                 
                m_DPList.Add(dp);
                rowIndex++;
            }
        }

        private void DisplayGridView()
        {
            dataGridView1.Rows.Clear();
            int rowNum = 1;
             
            m_DPList.Sort(delegate(DisasterPrevention a, DisasterPrevention b)
            {
                int xdiff = a.RowIdx.CompareTo(b.RowIdx);
                if (xdiff != 0) return xdiff;
                else return a.RowIdx.CompareTo(b.RowIdx);
            });

            foreach (DisasterPrevention dp in m_DPList)
            {
                DataGridViewRow row = MakeNewRow();
                 
                CellDataUpdate(row.Cells[ColDevName.Index], dp.Type, dp.Type);
                CellDataUpdate(row.Cells[ColLocation.Index], (dp.EquipmentZone == null) ? null : dp.EquipmentZone.DisplayText, dp.EquipmentZone);
                CellDataUpdate(row.Cells[ColStandardQuantity.Index], dp.StandardQuantity, dp.StandardQuantity);
                CellDataUpdate(row.Cells[ColStatusQuantity.Index], dp.StatusQuantity, dp.StatusQuantity);

                if (!ColCheckCycle.Items.Contains(dp.CheckCycle))
                    ColCheckCycle.Items.Add(dp.CheckCycle);
                CellDataUpdate(row.Cells[ColCheckCycle.Index], dp.CheckCycle, dp.CheckCycle);

                if (!ColCheckWay.Items.Contains(dp.CheckWay))
                    ColCheckWay.Items.Add(dp.CheckWay);
                CellDataUpdate(row.Cells[ColCheckWay.Index], dp.CheckWay, dp.CheckWay);
                CellDataUpdate(row.Cells[ColvalidityDate.Index], dp.Date, dp.Date);
                row.Cells[ColNum.Index].Value = rowNum;
                row.Tag = dp;
                rowNum++;
            }
        }

        public DataGridViewRow MakeNewRow()
        {
            if (dataGridView1.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                return dataGridView1.Rows[dataGridView1.Rows.Count - 2];
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[dataGridView1.Rows.Count - 1];
        }
        #endregion 
         
        #region 위치 설정 
        public void SetLocation(EquipmentZone zone)
        {
            if (m_CurMode != Mode.EDIT)
                return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell == null)
                    return;

                if (cell.ColumnIndex != ColLocation.Index)
                    return;

                DisasterPrevention dp = dataGridView1.Rows[cell.RowIndex].Tag as DisasterPrevention;
                if (dp == null)
                {
                    dp = new DisasterPrevention();
                    m_DPList.Add(dp);
                    dataGridView1.Rows[cell.RowIndex].Tag = dp;
                }

                dp.EquipmentZone = zone;
                dp.EquipmentZoneOld = cell.Tag as EquipmentZone;
                dp.IsChg = true;

                cell.Value = zone.DisplayText;
                cell.Tag = zone;

                // Edit Mode 호출. 없으면 자동으로 Row가 생기지 않음
                dataGridView1.NotifyCurrentCellDirty(true);
                dataGridView1.NotifyCurrentCellDirty(false);

                // 번호입력을 위해서
                dataGridView1_CellBeginEdit(dataGridView1, new DataGridViewCellCancelEventArgs(cell.ColumnIndex, cell.RowIndex));

                if (!m_bIsEdit)
                    m_bIsEdit = true;
            } 
        }  
        #endregion
         
        private void SetMode(bool isSave = true)
        {
            if (m_CurMode == Mode.SEARCH)
            {
                // 조회 모드 -> 편집 모드
                dataGridView1.ReadOnly = false;
                dataGridView1.AllowUserToAddRows = true;
                ColNum.ReadOnly = true;
                ColLocation.ReadOnly = true;
                button_export.Enabled = false;
                button_edit.Text = "저장";
                button_locationCfg.Enabled = true;

                m_CurMode = Mode.EDIT;
                m_bIsEdit = false;
            }
            else if (m_CurMode == Mode.EDIT)
            {
                if (isSave)
                {
                    if (!Save())
                        return;
                }

                if (m_FormLocationCfg != null && m_FormLocationCfg.Visible)
                    m_FormLocationCfg.Close();

                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                button_export.Enabled = true;
                button_edit.Text = "편집";
                button_locationCfg.Enabled = false;

                m_CurMode = Mode.SEARCH;
                m_bIsEdit = false;
            }            
        }

        private void ShowLocationCfg()
        {
            m_FormLocationCfg = new FormLocationCfg();
            m_FormLocationCfg.FormClosed += m_FormLocationCfg_FormClosed;
            Point parentPt = this.Parent.Parent.PointToScreen(this.Parent.Parent.Location);
            m_FormLocationCfg.Location = new Point(parentPt.X - m_FormLocationCfg.Size.Width, parentPt.Y);
            m_FormLocationCfg.StartPosition = FormStartPosition.Manual;
            m_FormLocationCfg.Show();

            button_locationCfg.Enabled = false;
        }

        private void DeleteRow()
        {
            List<int> rowIndex = new List<int>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                DisasterPrevention dp = dataGridView1.Rows[cell.RowIndex].Tag as DisasterPrevention;
                if (dp == null)
                    continue;

                dp.RowIdx = cell.RowIndex;
                if (dp == null)
                    continue;

                rowIndex.Add(cell.RowIndex);

                if (dp.ID > 0) 
                    m_DeleteDP.Add(dp);
                m_DPList.Remove(dp);
            } 

            if (m_DeleteDP.Count > 0)
                m_bIsEdit = true;

            DisplayGridView();
        }

        private bool RollBack()
        {
            FormDPMsgBox frm = new FormDPMsgBox("경고", "수정한 내용을 복구하시겠습니까?", MessageBoxButtons.YesNo);
            Point parentPt = this.Parent.Parent.PointToScreen(this.Parent.Parent.Location);
            int x = (this.Size.Width / 2) - (frm.Size.Width / 2);
            int y = (this.Size.Height / 2) - (frm.Size.Height / 2);
            frm.Location = new Point(parentPt.X + x, parentPt.Y + y);
            frm.StartPosition = FormStartPosition.Manual;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.No)
                return false; 

            List<DisasterPrevention> tempDps = new List<DisasterPrevention>();

            foreach (DisasterPrevention dp in m_DPList)
            { 
                if (dp == null)
                    continue;

                if (!dp.IsChg)
                    continue;

                if (dp.ID < 0)
                {
                    tempDps.Add(dp);
                    continue;
                }

                if (dp.Type != dp.TypeOld)
                {
                    foreach (DisasterType type in m_TypeList)
                    {
                        if (type == dp.TypeOld)
                        { 
                            dp.Type = dp.TypeOld;
                            break;
                        }
                    }
                }
                if (dp.EquipmentZone != dp.EquipmentZoneOld) 
                    dp.EquipmentZone = dp.EquipmentZoneOld; 
                if (dp.StandardQuantity != dp.StandardQuantityOld) 
                    dp.StandardQuantity = dp.StandardQuantityOld; 
                if (dp.StatusQuantity != dp.StatusQuantityOld) 
                    dp.StatusQuantity = dp.StatusQuantityOld; 
                if (dp.CheckCycle != dp.CheckCycleOld) 
                    dp.CheckCycle = dp.CheckCycleOld; 
                if (dp.CheckWay != dp.CheckWayOld) 
                    dp.CheckWay = dp.CheckWayOld; 
                if (dp.Date != dp.DateOld) 
                    dp.Date = dp.DateOld; 

                dp.IsChg = false;
            }

            foreach (DisasterPrevention item in tempDps)
            {
                m_DPList.Remove(item);
            }

            foreach (DisasterPrevention item in m_DeleteDP)
            {
                m_DPList.Add(item);
            } 

            m_DeleteDP.Clear();

            return true;
        }

        private bool Save()
        { 
            List<DisasterPrevention> InsertDP = new List<DisasterPrevention>();
            List<DisasterPrevention> UpdateDP = new List<DisasterPrevention>();
            List<DisasterPrevention> DeleteDP = new List<DisasterPrevention>();
            bool isQ = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DisasterPrevention dp = row.Tag as DisasterPrevention;
                if (dp == null)
                    continue;

                if (!dp.IsChg)
                    continue;

                if (dp.Type == null || dp.Type.Name.Length == 0 || dp.EquipmentZone == null)
                {
                    //m_DPList.Remove(dp);
                    DeleteDP.Add(dp);
                    isQ = true;
                    continue;
                } 

                if (dp.ID < 0)
                    InsertDP.Add(dp); 
                else
                    UpdateDP.Add(dp); 
            }

            if (isQ)
            {
                FormDPMsgBox frm = new FormDPMsgBox("", "장비명과 위치가 입력되지 않은 항목은\r삭제됩니다. 계속하시겠습니까?", MessageBoxButtons.YesNo);
                Point parentPt = this.Parent.Parent.PointToScreen(this.Parent.Parent.Location);
                int x = (this.Size.Width / 2) - (frm.Size.Width / 2);
                int y = (this.Size.Height / 2) - (frm.Size.Height / 2);
                frm.Location = new Point(parentPt.X + x, parentPt.Y + y);
                frm.StartPosition = FormStartPosition.Manual;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.No)
                    return false;                 
            }

            foreach (var item in m_DeleteDP)
            {
                if (item.ID < 0)
                    continue;

                DeleteDB(item.ID);
            }
            m_DeleteDP.Clear();

            foreach (DisasterPrevention item in InsertDP)
            {
                if (InsertDB(item))
                    item.IsChg = false;
            }
            foreach (DisasterPrevention item in UpdateDP)
            {
                if (UpdateDB(item))
                    item.IsChg = false;
            }
            foreach (DisasterPrevention item in DeleteDP)
            {
                m_DPList.Remove(item);
            }

            DisplayGridView();

            return true;
        }

        private void CellDataUpdate(DataGridViewCell cell, object value, object tag)
        {
            cell.Value = value;
            cell.Tag = tag;
        }

        #region HWP
        private string m_strHWPPath = null;
        private bool SaveHWPForDPMgr()
        {
            bool isHwpSetup = false;
             
            SDMS.Report.HwpCtrlData data = new SDMS.Report.HwpCtrlData();
            isHwpSetup = data.GetRegistry(ref m_strHWPPath);

            FormDPMsgBox frm = null;
            Point parentPt = this.Parent.Parent.PointToScreen(this.Parent.Parent.Location);
            int x = 0;
            int y = 0;

            //한글 설치여부
            if (isHwpSetup == false)
            {
                frm = new FormDPMsgBox("", "아래한글이 설치되지 않았습니다.", MessageBoxButtons.OK);
                x = (this.Size.Width / 2) - (frm.Size.Width / 2);
                y = (this.Size.Height / 2) - (frm.Size.Height / 2);
                frm.Location = new Point(parentPt.X + x, parentPt.Y + y);
                frm.StartPosition = FormStartPosition.Manual;
                frm.ShowDialog();                    

                //DialogResult result = MessageBox.Show("아래한글이 설치되지 않았습니다.");
                
                return false;
            }

            string SavePath = GetHWPFilePath("방재장비");
            if (SavePath == null)
                return false;
                        
            List<int> rowIndexs = new List<int>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (!rowIndexs.Contains(cell.RowIndex))
                    rowIndexs.Add(cell.RowIndex); 
            }
            rowIndexs.Sort();

            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            string strMsg = "";
            int cnt = rowIndexs.Count;
            if (cnt == 0)
            {
                strMsg = "방재장비 전체 목록에 대하여\r한글문서로 저장하시겠습니까?";
                 
                // 모든 데이터 넣어줌
                for (int index = 0; index < dataGridView1.Rows.Count; index++)
                {
                    DataGridViewRow row = dataGridView1.Rows[index];
                    rows.Add(row);
                }
            }
            else
            {
                strMsg = "선택한 " + cnt + "개 항목에 대하여\r한글문서로 저장하시겠습니까?";
                for (int i = 0; i < rowIndexs.Count; i++)
                {                    
                    rows.Add(dataGridView1.Rows[i]);                    
                } 
            }

            frm = new FormDPMsgBox("", strMsg, MessageBoxButtons.YesNo);
            x = (this.Size.Width / 2) - (frm.Size.Width / 2);
            y = (this.Size.Height / 2) - (frm.Size.Height / 2);
            frm.Location = new Point(parentPt.X + x, parentPt.Y + y);
            frm.StartPosition = FormStartPosition.Manual;
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.Yes)
                return false;

            SaveHwpCrtl(rows);
            FileWriter();
            SetHwpData();

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.Arguments = ((int)SDMS.Data.ReportMode.DisasterPrevention).ToString() + " " + SavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            info.CreateNoWindow = true;
            info.FileName = Application.StartupPath + "\\HwpReport.exe";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
            this.Cursor = Cursors.WaitCursor;

            int nCount = 0;
            bool bSuccess = true;
            while (process.HasExited == false)
            {
                process.WaitForExit(500);

                if (30 == nCount)
                {
                    process.Kill();
                    MessageBox.Show("오류 발생");
                    bSuccess = false;
                    break;
                }
            }

            if (bSuccess == true)
            {
                RunHWP(SavePath);
                //MessageBox.Show("저장되었습니다.");
            }

            this.Cursor = Cursors.Default;

            return true;
        }

        // 저장할 한글 파일의 경로
        private string GetHWPFilePath(string strDocType)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("__{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            try
            {
                string strFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (strFolderPath != null && strFolderPath.Length > 0)
                {
                    if (!System.IO.Directory.Exists(strFolderPath + "\\리포트"))
                        System.IO.Directory.CreateDirectory(strFolderPath + "\\리포트");

                    return strFolderPath + "\\리포트\\" + strDocType + strTime + ".hwp";
                }
            }
            catch (Exception)
            {
            }

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "한글 문서 (*.hwp)|*.hwp";

            dlg.FileName = strDocType + "_" + strTime;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string strSavePath = dlg.FileName;
                //strSavePath = subGap(strSavePath);
                return strSavePath;
            }

            return null;
        }

        public void FileWriter()
        { 
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveData.txt"))
            {
                foreach (string line in SaveArr)
                {
                    {
                        file.WriteLine(line);
                    }
                }
                file.Close();
            }
        }

        private void SaveHwpCrtl(List<DataGridViewRow> rows)
        {
            // 한글파일 출력전에 데이터를 저장하도록 함. 
             
            SaveArr.Clear();

            //int rowCnt = dataGridView1.RowCount;
            //if (m_CurMode == Mode.EDIT)
            //    rowCnt--; // 마지막Row 제외

            for (int index = 0; index < rows.Count; index++)
            {
                DataGridViewRow row = rows[index];

                SaveArr.Add(row.Cells[ColNum.Index].Value.ToString());
                DisasterType type = row.Cells[ColDevName.Index].Value as DisasterType;
                if (type == null)
                    SaveArr.Add("");
                else
                    SaveArr.Add(type.Name);
                SaveArr.Add(row.Cells[ColLocation.Index].Value.ToString());
                SaveArr.Add(row.Cells[ColStandardQuantity.Index].Value.ToString());
                SaveArr.Add(row.Cells[ColStatusQuantity.Index].Value.ToString());
                SaveArr.Add(""); // 지정 용도
                SaveArr.Add(row.Cells[ColCheckCycle.Index].Value.ToString());
                SaveArr.Add(row.Cells[ColCheckWay.Index].Value.ToString());
                SaveArr.Add((row.Cells[ColvalidityDate.Index].Value == null) ? " " : row.Cells[ColvalidityDate.Index].Value.ToString());
                SaveArr.Add(""); // 비고
            }

            //for (int index = 0; index < rowCnt; index++)
            //{
            //    DataGridViewRow row = dataGridView1.Rows[index];

            //    SaveArr.Add(row.Cells[ColNum.Index].Value.ToString());
            //    DisasterType type = row.Cells[ColDevName.Index].Value as DisasterType;
            //    if (type == null)
            //        SaveArr.Add("");
            //    else
            //        SaveArr.Add(type.Name); 
            //    SaveArr.Add(row.Cells[ColLocation.Index].Value.ToString());
            //    SaveArr.Add(row.Cells[ColStandardQuantity.Index].Value.ToString());
            //    SaveArr.Add(row.Cells[ColStatusQuantity.Index].Value.ToString());
            //    SaveArr.Add(""); // 지정 용도
            //    SaveArr.Add(row.Cells[ColCheckCycle.Index].Value.ToString());
            //    SaveArr.Add(row.Cells[ColCheckWay.Index].Value.ToString());
            //    SaveArr.Add((row.Cells[ColvalidityDate.Index].Value == null) ? " " : row.Cells[ColvalidityDate.Index].Value.ToString());
            //    SaveArr.Add(""); // 비고
            //}
        }

        public void SetHwpData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt"))
            {
                file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); 
                file.Close();
            }

            try
            {
                System.IO.StreamWriter stream = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveMemo.txt");
                stream.Close();
            }
            catch (Exception)
            {
            }
        }

        private void RunHWP(string strFilePath)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.Arguments = strFilePath;
            info.FileName = m_strHWPPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
        }

        private string GetReportLogoFileName()
        {
            string strSQL = "Select PropertyValue from OptionSdms where PropertyName='LogoFileName' and SiteID=" + UnE.SOP.ProxySOP.Instance.SiteID;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0) return string.Empty;

            string logoName = WebDBManager.GetStringField(arrResult[0].ToString(), string.Empty);

            return logoName;
        }
        #endregion

        #region DB
        private bool InsertDB(DisasterPrevention dp)
        {
            if (dp.EquipmentZone == null)
                return false;

            m_dbMgr.BeginBatch();

            ArrayList arrResult = m_dbMgr.GetBatchData("SELECT IFNULL(MAX(ID),0) FROM DisasterPreventionEquipmentLocation");
            if (arrResult == null || arrResult.Count == 0)
            {
                m_dbMgr.BatchRollback();
                return false;
            }
            int nLocationID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;
            dp.LocationID = nLocationID;
            StringBuilder sb2 = new StringBuilder();
            sb2.Append("INSERT INTO DisasterPreventionEquipmentLocation (ID, LocationName, X, Y)");
            sb2.AppendFormat("VALUES ({0}, '{1}', {2}, {3})", nLocationID, dp.EquipmentZone.ID, 0, 0);

            if (m_dbMgr.GetBatchData(sb2.ToString()) == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            int nTypeID = -1;
            string strTypeName = dp.Type.Name;

            if (dp.Type.ID < 0)
            {
                nTypeID = InsertTypeDB(dp.Type);
                dp.Type.ID = nTypeID;
            }
            else
                nTypeID = dp.Type.ID;

            if (nTypeID < 0)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            arrResult = m_dbMgr.GetBatchData("SELECT IFNULL(MAX(ID),0) FROM DisasterPreventionEquipment");
            if (arrResult == null || arrResult.Count == 0)
            {
                m_dbMgr.BatchRollback();
                return false;
            }
            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;
            dp.ID = nID;

            StringBuilder sb3 = new StringBuilder();
            sb3.Append("INSERT INTO DisasterPreventionEquipment (ID, TypeID, LocationID, Quantity, Name, CheckCycle, CheckWay, StandardQuantity, ValidityDate)");
            sb3.AppendFormat("VALUES ({0},{1},{2},{3},'{4}','{5}','{6}',{7},'{8}') ", nID, nTypeID, nLocationID, dp.StatusQuantity, strTypeName, dp.CheckCycle, dp.CheckWay, dp.StandardQuantity, dp.Date);

            if (m_dbMgr.GetBatchData(sb3.ToString()) == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            m_dbMgr.BatchCommit();

            return true;
        }

        private int InsertTypeDB(DisasterType type)
        {
            ArrayList arrResult = m_dbMgr.GetBatchData("SELECT IFNULL(MAX(ID),0) FROM DisasterPreventionEquipmentType");
            if (arrResult == null || arrResult.Count == 0)
            {
                m_dbMgr.BatchRollback();
                return -1;
            }
            int nTypeID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO DisasterPreventionEquipmentType (ID, Name)");
            sb.AppendFormat("VALUES ({0}, '{1}')", nTypeID, type.Name);

            if (m_dbMgr.GetBatchData(sb.ToString()) == null)
            {
                m_dbMgr.BatchRollback();
                return -1;
            }

            foreach (DisasterType item in m_TypeList)
            {
                if (item.Name == type.Name)
                {
                    item.ID = nTypeID;
                    return nTypeID;
                }
            }

            return -1;
        }

        private bool UpdateDB(DisasterPrevention dp)
        {
            string strQueryHead = "UPDATE DisasterPreventionEquipment SET ";
            string strQueryBody = "";

            m_dbMgr.BeginBatch();

            if (dp.Type != dp.TypeOld)
            {
                foreach (DisasterType item in m_TypeList)
                {
                    if (item == dp.Type)
                    {
                        int nTypeID = item.ID;
                        if (nTypeID < 0)
                        {
                            nTypeID = InsertTypeDB(item);
                            item.ID = nTypeID;
                        }

                        //CellDataUpdate(row.Cells[ColDevName.Index], item, item);
                        dp.TypeOld = dp.Type;

                        strQueryBody = "TypeID = " + nTypeID + ", Name = '" + item.Name + "'";
                        break;
                    }
                }
            }
            if (dp.EquipmentZone != dp.EquipmentZoneOld)
            {
                //CellDataUpdate(row.Cells[ColLocation.Index], dp.EquipmentZone.DisplayText, dp.EquipmentZone);
                dp.EquipmentZoneOld = dp.EquipmentZone;

                if (m_dbMgr.GetBatchData(string.Format("UPDATE DisasterPreventionEquipmentLocation SET LocationName='{0}' WHERE ID={1}", dp.EquipmentZone.ID, dp.LocationID)) == null)
                {
                    m_dbMgr.BatchRollback();
                    return false;
                } 
            }
            if (dp.StandardQuantity != dp.StandardQuantityOld)
            {
                //CellDataUpdate(row.Cells[ColStandardQuantity.Index], dp.StandardQuantity, dp.StandardQuantity);
                dp.StandardQuantityOld = dp.StandardQuantity;

                if (strQueryBody.Length != 0)
                    strQueryBody += ", ";
                strQueryBody += "StandardQuantity=" + dp.StandardQuantity;
            }
            if (dp.StatusQuantity != dp.StatusQuantityOld)
            {
                //CellDataUpdate(row.Cells[ColStatusQuantity.Index], dp.StatusQuantity, dp.StatusQuantity);
                dp.StatusQuantityOld = dp.StatusQuantity;

                if (strQueryBody.Length != 0)
                    strQueryBody += ", ";
                strQueryBody += "Quantity=" + dp.StatusQuantity;
            }
            if (dp.CheckCycle != dp.CheckCycleOld)
            {
                //CellDataUpdate(row.Cells[ColCheckCycle.Index], dp.CheckCycle, dp.CheckCycle);
                dp.CheckCycleOld = dp.CheckCycle;

                if (strQueryBody.Length != 0)
                    strQueryBody += ", ";
                strQueryBody += "CheckCycle='" + dp.CheckCycle + "'";
            }
            if (dp.CheckWay != dp.CheckWayOld)
            {
                //CellDataUpdate(row.Cells[ColCheckWay.Index], dp.CheckWay, dp.CheckWay);
                dp.CheckWayOld = dp.CheckWay;

                if (strQueryBody.Length != 0)
                    strQueryBody += ", ";
                strQueryBody += "CheckWay='" + dp.CheckWay + "'";
            }
            if (dp.Date != dp.DateOld)
            {
                //CellDataUpdate(row.Cells[ColvalidityDate.Index], dp.Date, dp.Date);
                dp.DateOld = dp.Date;

                if (strQueryBody.Length != 0)
                    strQueryBody += ", ";
                strQueryBody += "ValidityDate='" + dp.Date + "'";
            }

            if (strQueryBody.Length > 0)
            {
                strQueryBody += " WHERE ID = " + dp.ID;

                if (m_dbMgr.GetBatchData(strQueryHead + strQueryBody) == null)
                {
                    m_dbMgr.BatchRollback();
                    return false;
                } 
            }

            m_dbMgr.BatchCommit();
            return true;
        }

        private bool DeleteDB(int nID)
        {
            m_dbMgr.BeginBatch();

            int nLocationID = -1;
            ArrayList arrResult = m_dbMgr.GetBatchData("SELECT LocationID FROM DisasterPreventionEquipment WHERE ID=" + nID);
            if (arrResult == null || arrResult.Count == 0)
            {
                m_dbMgr.BatchRollback();
                return false;
            }
            nLocationID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            string strDelQuery = "DELETE FROM DisasterPreventionEquipment WHERE ID=" + nID;
            if (m_dbMgr.GetBatchData(strDelQuery) == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            string strDelQuery2 = "DELETE FROM DisasterPreventionEquipmentLocation WHERE ID=" + nLocationID;
            if (m_dbMgr.GetBatchData(strDelQuery2) == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            m_dbMgr.BatchCommit();

            return true;
        }  
        #endregion   
    }

    public class DisasterType
    {
        private int m_nID = -1;
        private string m_strName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
    }

    public class DisasterPrevention
    {
        private bool m_bIsChg = false;
        private int m_nRowIdx = -1;
         
        private int m_nID = -1;
        private int m_nLocationID = -1;

        private DisasterType m_Type = new DisasterType();
        private EquipmentZone m_EquipmentZone = null;
        private int m_nStandardQuantity = 0;
        private int m_nStatusQuantity = 0;
        private string m_strCheckCycle = "";
        private string m_strCheckWay = "";
        private string m_strDate = "";

        private DisasterType m_TypeOld = new DisasterType();
        private EquipmentZone m_EquipmentZoneOld = null;
        private int m_nStandardQuantityOld = 0;
        private int m_nStatusQuantityOld = 0;
        private string m_strCheckCycleOld = "";
        private string m_strCheckWayOld = "";
        private string m_strDateOld = "";

        public bool IsChg
        {
            get { return m_bIsChg; }
            set { m_bIsChg = value; }
        }
        public int RowIdx
        {
            get { return m_nRowIdx; }
            set { m_nRowIdx = value; }
        } 

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public DisasterType Type
        {
            get { return m_Type; }
            set { m_Type= value; }
        }
        public int LocationID
        {
            get { return m_nLocationID; }
            set { m_nLocationID = value; }
        }
        public EquipmentZone EquipmentZone
        {
            get { return m_EquipmentZone; }
            set { m_EquipmentZone = value; }
        }
        public int StandardQuantity
        {
            get { return m_nStandardQuantity; }
            set { m_nStandardQuantity = value; }
        }
        public int StatusQuantity
        {
            get { return m_nStatusQuantity; }
            set { m_nStatusQuantity = value; }
        }
        public string CheckCycle
        {
            get { return (m_strCheckCycle.Length == 0) ? "수시" : m_strCheckCycle; }
            set { m_strCheckCycle = value; }
        }
        public string CheckWay
        {
            get { return (m_strCheckWay.Length == 0) ? "육안검사" : m_strCheckWay; }
            set { m_strCheckWay = value; }
        }
        public string Date
        {
            get { return m_strDate; }
            set { m_strDate = value; }
        }

        public DisasterType TypeOld
        {
            get { return m_TypeOld; }
            set { m_TypeOld = value; }
        }
        public EquipmentZone EquipmentZoneOld
        {
            get { return m_EquipmentZoneOld; }
            set { m_EquipmentZoneOld = value; }
        }
        public int StandardQuantityOld
        {
            get { return m_nStandardQuantityOld; }
            set { m_nStandardQuantityOld = value; }
        }
        public int StatusQuantityOld
        {
            get { return m_nStatusQuantityOld; }
            set { m_nStatusQuantityOld = value; }
        }
        public string CheckCycleOld
        {
            get { return m_strCheckCycleOld; }
            set { m_strCheckCycleOld = value; }
        }
        public string CheckWayOld
        {
            get { return m_strCheckWayOld; }
            set { m_strCheckWayOld = value; }
        }
        public string DateOld
        {
            get { return m_strDateOld; }
            set { m_strDateOld = value; }
        }
    }
}
