using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class PageBackStageMessage : Form, UnE.GUI.IRibbonButtonOwner
    {
        public enum DisasterType { FIRE = 0, POLLUTION, TERROR, HEAVY_SNOW, GENERAL_DISASTER, SUBMERGENCE, TYPHOON, EARTHQUAKE, TYPE_COUNT };

        private Dictionary<DisasterType, RibbonButton> m_dicDisasterButtons = new Dictionary<DisasterType, RibbonButton>();
        private Dictionary<DisasterType, ArrayList> m_dicDisasterMessageType = new Dictionary<DisasterType, ArrayList>();

        // DB에 저장되어 있는 원본 기록
        private Dictionary<DisasterType, ArrayList> m_dicDBDisasterMessageType = new Dictionary<DisasterType, ArrayList>();

        private RibbonButton m_btnSelected = null;
        private RibbonButton m_btnComboBoxSelected = null;
        private int m_nSelectedRowIndex = -1;
        private bool m_ignoreChanged = false;
        private bool m_editMode = false;
        private bool m_cellEditing = false;

        public bool EditMode
        {
            get { return m_editMode; }
            set
            {
                m_editMode = value;

                dataGridViewDisaster.AllowUserToAddRows = value;
                dataGridViewDisaster.AllowUserToDeleteRows = value;
                dataGridViewDisaster.ReadOnly = !value;
            }
        }

        public PageBackStageMessage()
        {
            InitializeComponent();
        }

        private void PageBackStageMessage_Load(object sender, EventArgs e)
        {
            LoadDisasterType();
            InitButtons();

            pictureBoxComboBody.Font = new Font("맑은고딕", 9, FontStyle.Regular);
            pictureBoxComboBody.TextColor = Color.Black;
        }

        private void LoadDisasterType()
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;
            string strSQL = "Select id, DisasterType, DisasterSubType from InternalTransmissionMessageType";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = (int)arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nDisasterType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strSubDisasterType = WebDBManager.GetStringField(arrResult[i + 2], "");

                if (nID < 0 || nDisasterType < 0 || nDisasterType >= (int)DisasterType.TYPE_COUNT)
                    continue;

                if (strSubDisasterType.Length == 0)
                    continue;

                DisasterType type = (DisasterType)nDisasterType;

                ArrayList arrSubDisasterTypes = null;
                ArrayList arrDBSubDisasterTypes = null;

                if (m_dicDisasterMessageType.ContainsKey(type))
                {
                    arrSubDisasterTypes = m_dicDisasterMessageType[type];
                    arrDBSubDisasterTypes = m_dicDBDisasterMessageType[type];
                }
                else
                {
                    arrSubDisasterTypes = new ArrayList();
                    arrDBSubDisasterTypes = new ArrayList();
                    m_dicDisasterMessageType[type] = arrSubDisasterTypes;
                    m_dicDBDisasterMessageType[type] = arrDBSubDisasterTypes;
                }

                arrSubDisasterTypes.Add(strSubDisasterType);
                arrDBSubDisasterTypes.Add(strSubDisasterType);
            }
        }

        private void InitButton(RibbonButton btn, DisasterType type)
        {
            btn.Owner = this;
            btn.Tag = type;
            m_dicDisasterButtons[type] = btn;

            if (btn.IsChecked)
                m_btnSelected = btn;
        }

        private void InitButtons()
        {
            InitButton(btnFire, DisasterType.FIRE);
            InitButton(btnPollution, DisasterType.POLLUTION);
            InitButton(btnTerror, DisasterType.TERROR);
            InitButton(btnHeavySnow, DisasterType.HEAVY_SNOW);
            InitButton(btnGeneralDisaster, DisasterType.GENERAL_DISASTER);
            InitButton(btnSubmergence, DisasterType.SUBMERGENCE);
            InitButton(btnTyphoon, DisasterType.TYPHOON);
            InitButton(btnEarthquake, DisasterType.EARTHQUAKE);

            SelectButton(m_btnSelected);
        }

        private void SelectButton(RibbonButton btn)
        {
            foreach (KeyValuePair<DisasterType, RibbonButton> pair in m_dicDisasterButtons)
            {
                pair.Value.IsChecked = false;
            }

            m_btnSelected = btn;

            if (btn != null)
            {
                btn.IsChecked = true;

                UpdateDisasterType((DisasterType)btn.Tag);
            }

            this.Refresh();
        }

        private void ShowGrid(bool show)
        {
            if (EditMode && !show)
            {
                dataGridViewDisaster.CommitEdit(DataGridViewDataErrorContexts.Commit);
                EditMode = false;
                CheckChangedData();
            }

            dataGridViewDisaster.Visible = show;
            btnEditDisaster.Enabled = !show;

            if (show)
                dataGridViewDisaster.Focus();
        }

        private bool CheckChangedData()
        {
            if (m_btnSelected == null)
                return true;

            DisasterType type = (DisasterType)m_btnSelected.Tag;

            ArrayList arrSubDisasters = null;

            if (m_dicDBDisasterMessageType.ContainsKey(type))
                arrSubDisasters = m_dicDBDisasterMessageType[type];
            else
                arrSubDisasters = new ArrayList();

            ArrayList arrGridText = new ArrayList();
            if (!DuplicationCheck(arrGridText))
                return false;

            int nCount1 = arrSubDisasters.Count;
            int nCount2 = arrGridText.Count;

            if (nCount1 != nCount2)
            {
                EnableSaveButton(true);

                if (nCount2 == 0)
                    m_dicDisasterMessageType.Remove(type);
                else
                    m_dicDisasterMessageType[type] = arrGridText;
            }
            else
            {
                for (int i = 0; i < nCount1; i++)
                {
                    string strText1 = (string)arrSubDisasters[i];
                    string strText2 = (string)arrGridText[i];

                    if (strText1 != strText2)
                    {
                        EnableSaveButton(true);
                        m_dicDisasterMessageType[type] = arrGridText;
                        break;
                    }
                }
            }

            return true;
        }

        private bool DuplicationCheck(ArrayList arrGridText)
        {
            int nRowCount = dataGridViewDisaster.Rows.Count;

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = dataGridViewDisaster.Rows[i];

                if (row.Cells[0].Value == null)
                    continue;

                string strSubDisaster = (string)row.Cells[0].Value;

                if (arrGridText.Contains(strSubDisaster))
                {
                    MessageBox.Show("중복된 데이터가 있습니다.");
                    dataGridViewDisaster.ClearSelection();
                    row.Cells[0].Selected = true;
                    return false;
                }

                if (strSubDisaster != null && strSubDisaster.Length > 0)
                    arrGridText.Add(strSubDisaster);
            }

            return true;
        }

        private void SetComboText(string strText, int nSelectedRowIndex)
        {
            pictureBoxComboBody.Text = strText;
            m_nSelectedRowIndex = nSelectedRowIndex;
            pictureBoxComboBody.Refresh();
        }

        private void UpdateDisasterType(DisasterType type)
        {
            ShowGrid(false);
            dataGridViewDisaster.Rows.Clear();
            m_nSelectedRowIndex = -1;

            if (!m_dicDisasterMessageType.ContainsKey(type))
            {
                SetComboText("", -1);
                return;
            }

            ArrayList arrSubDisasters = m_dicDisasterMessageType[type];

            if (arrSubDisasters.Count == 0)
            {
                SetComboText("", -1);
                return;
            }

            SetComboText((string)arrSubDisasters[0], 0);

            foreach (string strSubDisaster in arrSubDisasters)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                cell.Value = strSubDisaster;
                row.Cells.Add(cell);
                dataGridViewDisaster.Rows.Add(row);
            }
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (sender == m_btnSelected)
                return;

            SelectButton((RibbonButton)sender);
        }

        private void pictureBoxComboButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewDisaster.Visible)
                ShowGrid(false);
            else
            {
                int nHeight = 0;

                foreach (DataGridViewRow row in dataGridViewDisaster.Rows)
                {
                    nHeight += row.Height;
                }

                if (nHeight > 0)
                {
                    int nSelectedRowIndex = 0;

                    if (m_btnSelected == m_btnComboBoxSelected && m_btnComboBoxSelected != null && m_nSelectedRowIndex >= 0)
                        nSelectedRowIndex = m_nSelectedRowIndex;
                    else
                        m_nSelectedRowIndex = -1;

                    dataGridViewDisaster.Location = new Point(panelMiddle.Location.X + pictureBoxComboHeader.Location.X, panelMiddle.Location.Y + pictureBoxComboHeader.Location.Y + pictureBoxComboHeader.Size.Height);
                    dataGridViewDisaster.Size = new Size(pictureBoxComboHeader.Size.Width + pictureBoxComboBody.Size.Width, nHeight + 3);
                    ShowGrid(true);

                    dataGridViewDisaster.Rows[nSelectedRowIndex].Cells[0].Selected = true;
                }
            }
        }

        private void dataGridViewDisaster_Leave(object sender, EventArgs e)
        {
            ShowGrid(false);
        }

        private void labelDisaster_MouseDown(object sender, MouseEventArgs e)
        {
            ShowGrid(false);
        }

        private void panelMiddle_MouseDown(object sender, MouseEventArgs e)
        {
            ShowGrid(false);
        }

        private void PageBackStageMessage_MouseDown(object sender, MouseEventArgs e)
        {
            ShowGrid(false);
        }

        private void panelLeft_MouseDown(object sender, MouseEventArgs e)
        {
            ShowGrid(false);
        }

        private void dataGridViewDisaster_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (EditMode)
                return;

            if (dataGridViewDisaster.SelectedCells.Count == 0)
                return;

            m_btnComboBoxSelected = m_btnSelected;

            if (!m_ignoreChanged)
            {
                SetComboText((string)dataGridViewDisaster.SelectedCells[0].Value, dataGridViewDisaster.SelectedCells[0].RowIndex);
                pictureBoxComboBody.Refresh();
            }
            else
                m_ignoreChanged = false;
        }

        private void dataGridViewDisaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (EditMode)
                return;

            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                m_nSelectedRowIndex = dataGridViewDisaster.SelectedCells[0].RowIndex;
                m_ignoreChanged = true;
                //ShowGrid(false);
            }
        }

        private void dataGridViewDisaster_KeyUp(object sender, KeyEventArgs e)
        {
            if (EditMode)
            {
                if (e.KeyData == Keys.Delete && !m_cellEditing)
                {
                    if (dataGridViewDisaster.SelectedCells.Count > 0)
                    {
                        int nRowIndex = dataGridViewDisaster.SelectedCells[0].RowIndex;

                        if (nRowIndex < dataGridViewDisaster.Rows.Count - 1)
                        {
                            dataGridViewDisaster.Rows.RemoveAt(nRowIndex);

                            if (dataGridViewDisaster.SelectedCells.Count > 0 && dataGridViewDisaster.SelectedCells[0].Value != null)
                                SetComboText(dataGridViewDisaster.SelectedCells[0].Value.ToString(), dataGridViewDisaster.SelectedCells[0].RowIndex);
                            else
                                SetComboText("", -1);

                            ResizeGrid();
                        }
                    }
                }

                return;
            }

            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                if (m_nSelectedRowIndex >= 0)
                    SetComboText((string)dataGridViewDisaster.Rows[m_nSelectedRowIndex].Cells[0].Value, m_nSelectedRowIndex);

                ShowGrid(false);
                pictureBoxComboBody.Refresh();
            }
        }

        private void dataGridViewDisaster_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (EditMode)
                return;

            m_nSelectedRowIndex = dataGridViewDisaster.SelectedCells[0].RowIndex;
            ShowGrid(false);
            pictureBoxComboBody.Refresh();
        }

        private void PageBackStageMessage_Resize(object sender, EventArgs e)
        {
            panelMiddle.Size = new Size(this.Size.Width - panelMiddle.Location.X, panelMiddle.Size.Height);
        }

        private void btnEditDisaster_Click(object sender, EventArgs e)
        {
            EditMode = true;

            ResizeGrid();          
            ShowGrid(true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("변경된 데이터를 시스템에 적용하시겠습니까?", "주의", MessageBoxButtons.YesNo)
                == System.Windows.Forms.DialogResult.No)
            {
                if (MessageBox.Show("변경된 데이터를 이전 상태로 모두 초기화하시겠습니까?", "주의", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    ReturnToOrigin();
                    EnableSaveButton(false);
                }

                return;
            }

            EnableSaveButton(false);

            for (int i = 0; i < (int)DisasterType.TYPE_COUNT; i++)
            {
                DisasterType type = (DisasterType)i;
                ArrayList arrNewSubDisasters = CheckDifference(type);

                if (arrNewSubDisasters != null)
                    UpdateDB(type, arrNewSubDisasters);
            }
        }

        private void ReturnToOrigin()
        {
            for (int i = 0; i < (int)DisasterType.TYPE_COUNT; i++)
            {
                DisasterType type = (DisasterType)i;

                if (m_dicDBDisasterMessageType.ContainsKey(type))
                {
                    ArrayList arrDBText = m_dicDBDisasterMessageType[type];
                    ArrayList arrText = new ArrayList();

                    foreach (string strText in arrDBText)
                    {
                        arrText.Add(strText);
                    }

                    m_dicDisasterMessageType[type] = arrText;
                }
                else
                    m_dicDisasterMessageType.Remove(type);
            }

            if (m_btnSelected != null)
            {
                UpdateDisasterType((DisasterType)m_btnSelected.Tag);
            }
        }

        private bool PrintIDs(WebDBManager dbMgr)
        {
            string strSQL = "Select id from InternalTransmissionMessageType order by id";
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("BatchRollback");
                dbMgr.BatchRollback();
                return false;
            }

            string strIDs = "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (strIDs.Length == 0)
                    strIDs = nID.ToString();
                else
                    strIDs += ", " + nID.ToString();
            }

            System.Diagnostics.Trace.WriteLine("삭제전 ID들 : " + strIDs);

            return true;
        }

        private void UpdateDB(DisasterType type, ArrayList arrNewSubDisasters)
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;
            dbMgr.BeginBatch();

            if (!PrintIDs(dbMgr))
                return;

            // type에 해당하는 데이터를 DB에서 삭제한다.
            string strSQL = "Delete from InternalTransmissionMessageType where DisasterType = " + ((int)type).ToString();
            System.Diagnostics.Trace.WriteLine(strSQL);
            if (dbMgr.GetBatchData(strSQL) == null)
            {
                System.Diagnostics.Trace.WriteLine("BatchRollback");
                dbMgr.BatchRollback();
                return;
            }

            // 삭제된 Data로 인하여 비어있는 id가 있을 수 있다.
            // 비어있는 id 값을 얻어온다.
            strSQL = "Select id from InternalTransmissionMessageType order by id";
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            System.Diagnostics.Trace.WriteLine(strSQL);
            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("BatchRollback");
                dbMgr.BatchRollback();
                return;
            }

            int nCount = arrResult.Count;
            ArrayList arrIDs = new ArrayList();
            int nPrevIndex = 0, nMaxID = 0;
            string strIDs = "";

            for (int i = 0; i < nCount; i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (nID != nPrevIndex + 1)
                {
                    // 비어있는 id를 arrIDs에 담는다.
                    for (int j = nPrevIndex + 1; j < nID; j++)
                    {
                        arrIDs.Add(j);
                    }
                }

                nPrevIndex = nID;
                nMaxID = nID;

                if (strIDs.Length == 0)
                    strIDs = nID.ToString();
                else
                    strIDs += ", " + nID.ToString();
            }

            System.Diagnostics.Trace.WriteLine("남아있는 ID : " + strIDs);

            int nIDCount = arrIDs.Count;
            int nDataCount = arrNewSubDisasters.Count;

            ArrayList arrSubDisasters = new ArrayList();

            strSQL = "";

            string strFormat = "Insert into InternalTransmissionMessageType (id, DisasterType, DisasterSubType, Description)";
            strFormat += " values ({0}, {1}, '{2}', NULL);";

            // arrIDs에 있는 id값부터 하나씩 사용한다.
            for (int i = 0; i < nDataCount; i++)
            {
                int nID = i < nIDCount ? (int)arrIDs[i] : ++nMaxID;
                strSQL += string.Format(strFormat, nID, (int)type, (string)arrNewSubDisasters[i]);

                arrSubDisasters.Add(arrNewSubDisasters[i]);
            }

            if (nDataCount > 0)
            {
                System.Diagnostics.Trace.WriteLine(strSQL);

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    System.Diagnostics.Trace.WriteLine("BatchRollback");
                    dbMgr.BatchRollback();
                    return;
                }
            }

            System.Diagnostics.Trace.WriteLine("BatchCommit");
            dbMgr.BatchCommit();

            // m_dicDBDisasterMessageType의 값을 m_dicDisasterMessageType과 같도록 한다.
            if (nDataCount == 0)
                m_dicDBDisasterMessageType.Remove(type);
            else
                m_dicDBDisasterMessageType[type] = arrSubDisasters;
        }
        /*private void UpdateDB(DisasterType type, ArrayList arrNewSubDisasters)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            
            // type에 해당하는 데이터를 DB에서 삭제한다.
            string strSQL = "Delete from InternalTransmissionMessageType where DisasterType = " + ((int)type).ToString();
            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                return;
            }

            // 삭제된 Data로 인하여 비어있는 id가 있을 수 있다.
            // 비어있는 id 값을 얻어온다.
            strSQL = "Select id from InternalTransmissionMessageType order by id";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                return;
            }

            int nCount = arrResult.Count;
            ArrayList arrIDs = new ArrayList();
            int nPrevIndex = 0, nMaxID = 0;

            for (int i = 0; i < nCount; i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (nID != nPrevIndex + 1)
                {
                    // 비어있는 id를 arrIDs에 담는다.
                    for (int j = nPrevIndex + 1; j < nID; j++)
                    {
                        arrIDs.Add(j);
                    }
                }

                nPrevIndex = nID;
                nMaxID = nID;
            }

            int nIDCount = arrIDs.Count;
            int nDataCount = arrNewSubDisasters.Count;

            ArrayList arrSubDisasters = new ArrayList();

            strSQL = "";

            string strFormat = "Insert into InternalTransmissionMessageType (id, DisasterType, DisasterSubType, Description)";
            strFormat += " values ({0}, {1}, '{2}', NULL);";

            // arrIDs에 있는 id값부터 하나씩 사용한다.
            for (int i = 0; i < nDataCount; i++)
            {
                int nID = i < nIDCount ? (int)arrIDs[i] : ++nMaxID;
                strSQL += string.Format(strFormat, nID, (int)type, (string)arrNewSubDisasters[i]);

                arrSubDisasters.Add(arrNewSubDisasters[i]);
            }

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {
                return;
            }

            // m_dicDBDisasterMessageType의 값을 m_dicDisasterMessageType과 같도록 한다.
            if (nDataCount == 0)
                m_dicDBDisasterMessageType.Remove(type);
            else
                m_dicDBDisasterMessageType[type] = arrSubDisasters;
        }*/

        private ArrayList CheckDifference(DisasterType type)
        {
            ArrayList arrTextDB = null;
            ArrayList arrText = null;

            if (m_dicDBDisasterMessageType.ContainsKey(type))
                arrTextDB = m_dicDBDisasterMessageType[type];
            else
                arrTextDB = new ArrayList();

            if (m_dicDisasterMessageType.ContainsKey(type))
                arrText = m_dicDisasterMessageType[type];
            else
                arrText = new ArrayList();

            int nDBCount = arrTextDB.Count;
            int nCount = arrText.Count;

            if (nDBCount != nCount)
                return arrText;

            for (int i = 0; i < nCount; i++)
            {
                string strDBText = (string)arrTextDB[i];
                string strText = (string)arrText[i];

                if (strDBText != strText)
                    return arrText;
            }

            return null;
        }
        
        private void ResizeGrid()
        {
            int nHeight = 0;

            foreach (DataGridViewRow row in dataGridViewDisaster.Rows)
            {
                nHeight += row.Height;
            }

            if (nHeight > 0)
            {
                dataGridViewDisaster.Location = new Point(panelMiddle.Location.X + pictureBoxComboHeader.Location.X, panelMiddle.Location.Y + pictureBoxComboHeader.Location.Y + pictureBoxComboHeader.Size.Height);
                dataGridViewDisaster.Size = new Size(pictureBoxComboHeader.Size.Width + pictureBoxComboBody.Size.Width, nHeight + 3);
            }
        }

        private void dataGridViewDisaster_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            ResizeGrid();
        }

        private void dataGridViewDisaster_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ResizeGrid();
        }

        private void dataGridViewDisaster_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            m_cellEditing = true;
        }

        private void dataGridViewDisaster_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            m_cellEditing = false;
        }

        private void EnableSaveButton(bool enabled)
        {
            btnSave.Enabled = enabled;

            if (enabled)
                btnSave.BackColor = Color.SkyBlue;
            else
                btnSave.BackColor = Color.FromArgb(232, 230, 230);
        }
    }
}
