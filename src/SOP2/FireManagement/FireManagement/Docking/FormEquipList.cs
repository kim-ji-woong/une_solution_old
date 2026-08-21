using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using UnE.GUI;

namespace FireManagement
{
    public partial class FormEquipList : Form
    {
        private FireEquipment m_selectedEquipment = null;
        private DXFViewer.Shape m_selectedShape = null;
        private Dictionary<DXFViewer.Shape, DataGridViewRow> m_dicFEShape = new Dictionary<DXFViewer.Shape, DataGridViewRow>();
        private Dictionary<DXFViewer.Shape, DataGridViewRow> m_dicHDShape = new Dictionary<DXFViewer.Shape, DataGridViewRow>();
        private Dictionary<DXFViewer.Shape, DataGridViewRow> m_dicFAShape = new Dictionary<DXFViewer.Shape, DataGridViewRow>();

        private FireEquipment.EquipmentType[] m_arrEquipmentType = new FireEquipment.EquipmentType[3] { FireEquipment.EquipmentType.FE, FireEquipment.EquipmentType.HD, FireEquipment.EquipmentType.FA };
        private bool[] m_arrShowGrid = new bool[3] { true, true, true };

        private static char[] EMPTY_CHARS = new char[] { ' ', '\t', '\r', '\n' };

        // FormMain이 TagInputMode일때 편집한 설비번호에 해당하는 설비들을 저장하기 위한 Dictionary 객체
        // Key : FireEquipment ID
        private Dictionary<int, FireEquipment> m_dicTagInputModeEquipment = new Dictionary<int, FireEquipment>();

        public DXFViewer.Shape SelectedShape
        {
            get { return m_selectedShape; }
        }

        public FormEquipList()
        {
            InitializeComponent();

            InitControls();
            dataGridFA.Visible = false;
            dataGridHD.Visible = false;
            SetGridViewSize();
        }

        private void InitControls()
        {
            colEquipID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colRFIDTag.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colHDEquipID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colHDRFID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colHDStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colFAEquipID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colFARFID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colFAStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void FormEquipList_Resize(object sender, EventArgs e)
        {
            ResizeControl();
            /*int nGridSize = 240;

            // 소화기
            labelFE.Location = new Point(0, 0);
            labelFE.Size = new Size(this.Size.Width, labelFE.Height);

            dataGridFE.Location = new Point(0, labelFE.Size.Height);
            dataGridFE.Size = new Size(this.Size.Width, nGridSize);

            // 소화전
            labelHD.Location = new Point(0, dataGridFE.Location.Y + dataGridFE.Size.Height);
            labelHD.Size = new Size(this.Size.Width, labelFE.Height);

            dataGridHD.Location = new Point(0, labelHD.Location.Y + labelHD.Size.Height);
            dataGridHD.Size = new Size(this.Size.Width, nGridSize);

            // 발신기
            labelFA.Location = new Point(0, dataGridHD.Location.Y + dataGridHD.Size.Height);
            labelFA.Size = new Size(this.Size.Width, labelFE.Height);

            dataGridFA.Location = new Point(0, labelFA.Location.Y + labelFA.Size.Height);

            if (this.Size.Height > dataGridFA.Location.Y + nGridSize)
                dataGridFA.Size = new Size(this.Size.Width, this.Size.Height - labelFA.Location.Y);
            else
                dataGridFA.Size = new Size(this.Size.Width, nGridSize);*/
        }

        private void ResizeControl()
        {
            //int nGridSize = 240;
            //int y = 0;

            //Label lastLabel = null;
            //DataGridView lastGrid = null;

            //for (int i = 0; i < 3; i++)
            //{
            //    Label label = null;
            //    DataGridView grid = null;
            //    GetControl(m_arrEquipmentType[i], ref label, ref grid);

            //    if (m_arrShowGrid[i])
            //    {
            //        lastLabel = label;
            //        lastGrid = grid;
            //        label.Visible = true;
            //        grid.Visible = true;

            //        label.Location = new Point(0, y);
            //        label.Size = new Size(this.Size.Width, label.Height);

            //        grid.Location = new Point(0, label.Size.Height + y);
            //        grid.Size = new Size(this.Size.Width, nGridSize);

            //        y = grid.Location.Y + grid.Size.Height;
            //    }
            //    else
            //    {
            //        label.Visible = false;
            //        grid.Visible = false;
            //    }
            //}

            //// 화면 크기에 비하여 마지막 Grid의 크기가 작으면 Grid를 화면 크기만큼 키워준다.
            //if (lastGrid != null)
            //{
            //    if (this.Size.Height > lastGrid.Location.Y + nGridSize)
            //        lastGrid.Size = new Size(this.Size.Width, this.Size.Height - lastGrid.Location.Y);
            //}
        }

        private void GetControl(FireEquipment.EquipmentType type, ref Label label, ref DataGridView grid)
        {
            //if (type == FireEquipment.EquipmentType.FE)
            //{
            //    //label = labelFE;
            //    grid = dataGridFE;
            //}
            //else if (type == FireEquipment.EquipmentType.HD)
            //{
            //   // label = labelHD;
            //    grid = dataGridHD;
            //}
            //else// if (type == FireEquipment.EquipmentType.FA)
            //{
            //   // label = labelFA;
            //    grid = dataGridFA;
            //}
        }

        public void SetEquipments(ArrayList arrEquipments)
        {
            dataGridFE.Rows.Clear();
            dataGridHD.Rows.Clear();
            dataGridFA.Rows.Clear();

            m_dicFEShape.Clear();
            m_dicHDShape.Clear();
            m_dicFAShape.Clear();

            try
            {
                foreach (FireEquipment equip in arrEquipments)
                {
                    if (equip.Type == FireEquipment.EquipmentType.FE)
                        AddEquipment(equip, dataGridFE, m_dicFEShape);
                    else if (equip.Type == FireEquipment.EquipmentType.HD)
                        AddEquipment(equip, dataGridHD, m_dicHDShape);
                    else if (equip.Type == FireEquipment.EquipmentType.FA)
                        AddEquipment(equip, dataGridFA, m_dicFAShape);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            this.Refresh();
        }

        public void AddEquipmentHistory(Dictionary<FireEquipment, FireEquipmentHistory> m_dicEquipmentHistory)
        {
            dataGridFE.Rows.Clear();
            dataGridHD.Rows.Clear();
            dataGridFA.Rows.Clear();

            m_dicFEShape.Clear();
            m_dicHDShape.Clear();
            m_dicFAShape.Clear();

            foreach (FireEquipment equip in FormMain2.Instance.CurrentEquipments)
            {
                foreach (KeyValuePair<FireEquipment, FireEquipmentHistory> pair in m_dicEquipmentHistory)
                {
                    FireEquipmentHistory history = pair.Value;


                    if (equip.Type == FireEquipment.EquipmentType.FE)
                        AddEquipment(equip, dataGridFE, m_dicFEShape);
                    else if (equip.Type == FireEquipment.EquipmentType.HD)
                        AddEquipment(equip, dataGridHD, m_dicHDShape);
                    else if (equip.Type == FireEquipment.EquipmentType.FA)
                        AddEquipment(equip, dataGridFA, m_dicFAShape);
                }
            }

            this.Refresh();
        }

        private void AddEquipment(FireEquipment equip,  DataGridView grid, Dictionary<DXFViewer.Shape, DataGridViewRow> dicShape)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Height = 45;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = " " + equip.RFIDTag;
            row.Cells.Add(cell);


            cell = new DataGridViewTextBoxCell();
            cell.Value = " " + equip.EquipID;
            row.Cells.Add(cell);

            ArrayList arrHistory = FormMain2.Instance.IOManager.FindEquipmentHistoryList(equip.ID);
            if (arrHistory != null)
            {
                //가장 최근의 상태
                FireEquipmentHistory history2 = (FireEquipmentHistory)arrHistory[arrHistory.Count - 1];

                cell = new DataGridViewTextBoxCell();
                cell.Value = " " + FireEquipmentHistory.GetStatusText(history2.Status);
                row.Cells.Add(cell);
            }

            row.Tag = equip;
            grid.Rows.Add(row);

            if (equip.LinkedShape == null)
                FormMain2.Instance.DXFManager.AddEquipmentObjectToDXF(equip);
            else
            {
                DXFViewer.Layer layer = FormMain2.Instance.GetEquipmentLayer(equip.Type);

                if (layer != null && !layer.Shapes.Contains(equip.LinkedShape))
                    layer.Add(equip.LinkedShape);
            }

            if (equip.LinkedShape != null)
                dicShape[equip.LinkedShape] = row;
        }

        

        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            ClearSelection();

            DataGridView grid = (DataGridView)sender;
            FireEquipment equip = (FireEquipment)grid.Rows[e.RowIndex].Tag;

            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;

            if (equip.LinkedShape != null)
                equip.LinkedShape.Selected = true;

            if (m_selectedEquipment == equip)
                return;
            else
            {
                if (m_selectedEquipment != null && m_selectedEquipment.LinkedShape != null)
                    m_selectedEquipment.LinkedShape.Selected = false;

                m_selectedEquipment = equip;
                EventManager.Instance.ProcessEvent(Event.EQUIP_SELECTED, m_selectedEquipment);
                FormMain2.Instance.DXFControl.Refresh();
            }
        }

        public FireEquipment FindEquipment(DXFViewer.Shape shape)
        {
            if (m_dicFEShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFEShape[shape];
                return (FireEquipment)row.Tag;
            }
            else if (m_dicHDShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicHDShape[shape];
                return (FireEquipment)row.Tag;
            }
            else if (m_dicFAShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFAShape[shape];
                return (FireEquipment)row.Tag;
            }

            return null;
        }

        public bool SelectShape(DXFViewer.Shape shape)
        {
            if (m_selectedEquipment != null && m_selectedEquipment.LinkedShape == shape)
                return true;

            ClearSelection();

            FireEquipment equip = null;

            if (m_dicFEShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFEShape[shape];
                dataGridFE.CurrentCell = row.Cells[0];
                m_dicFEShape[shape].Selected = true;

                equip = (FireEquipment)row.Tag;

                if (!dataGridFE.Visible)
                    btnFireExtingusher_Click(null, null);
            }
            else if (m_dicHDShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicHDShape[shape];
                dataGridHD.CurrentCell = row.Cells[0];
                m_dicHDShape[shape].Selected = true;

                equip = (FireEquipment)row.Tag;

                if (!dataGridHD.Visible)
                    btnFirePlug_Click(null, null);
            }
            else if (m_dicFAShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFAShape[shape];
                dataGridFA.CurrentCell = row.Cells[0];
                m_dicFAShape[shape].Selected = true;

                equip = (FireEquipment)row.Tag;

                if (!dataGridFA.Visible)
                    btnFireAlarm_Click(null, null);
            }

            m_selectedShape = shape;

            if (shape != null)
                m_selectedShape.Selected = true;

            if (m_selectedEquipment == equip)
                return equip != null;
            else
            {
                if (m_selectedEquipment != null && m_selectedEquipment.LinkedShape != null)
                    m_selectedEquipment.LinkedShape.Selected = false;

                m_selectedEquipment = equip;
                EventManager.Instance.ProcessEvent(Event.EQUIP_SELECTED, m_selectedEquipment);
            }

            return equip != null;
        }

        private bool GetDxfObjID(Zone zone, FireEquipment.EquipmentType type, out string strDxfObjID)
        {
            strDxfObjID = "";

            int nIDCount = 0;
            ArrayList arrEquipments = FormMain2.Instance.IOManager.GetEquipments(zone);

            foreach (FireEquipment equip in arrEquipments)
            {
                if (equip.Type == type)
                    nIDCount++;
            }

            if (zone.Building == null)
                strDxfObjID = string.Format("-{0}-{1}", FireEquipment.GetTypeID(type), nIDCount + 1);
            else
                strDxfObjID = string.Format("{0}-{1}-{2}", zone.Building.BuildingID, FireEquipment.GetTypeID(type), nIDCount + 1);

            return true;
        }

        // DB로부터 직접 읽음
        /*private bool GetDxfObjID(Zone zone, FireEquipment.EquipmentType type, out string strDxfObjID)
        {
            strDxfObjID = "";

            string strZoneCondition;

            if (zone.Building == null)
                strZoneCondition = " = " + zone.ID.ToString();
            else
                strZoneCondition = string.Format(" in (select id from Zone where BuildingID = {0})", zone.Building.ID);

            string strSQL = string.Format("select count(id) from FireEquipment where EquipType = {0} and zoneID {1}",
                (int)type, strZoneCondition);

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nIDCount = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            
            if (zone.Building == null)
                strDxfObjID = string.Format("-{0}-{1}", FireEquipment.GetTypeID(type), nIDCount + 1);
            else
                strDxfObjID = string.Format("{0}-{1}-{2}", zone.Building.BuildingID, FireEquipment.GetTypeID(type), nIDCount + 1);

            return true;
        }*/

        public FireEquipment AddNewEquipment(string strRFID, string strRFIDTagID, string strEquipID, FireEquipment.EquipmentType type, float x, float y, string strDescription)
        {
            Zone zone = FormMain2.Instance.CurrentZone;
            string strDxfObjID;

            if (!GetDxfObjID(zone, type, out strDxfObjID))
                return null;

            FireEquipment equip = new FireEquipment();

            equip.DXFObjID = strDxfObjID;
            equip.EquipID = strEquipID;
            equip.Position = new PointF(x, y);
            equip.RFIDTag = strRFID;
            equip.RFIDTagID = strRFIDTagID;
            equip.Type = type;
            equip.Zone = zone;

            if (type == FireEquipment.EquipmentType.FE)
                AddEquipment(equip, dataGridFE, m_dicFEShape);
            else if (type == FireEquipment.EquipmentType.HD)
                AddEquipment(equip, dataGridHD, m_dicHDShape);
            else if (type == FireEquipment.EquipmentType.FA)
                AddEquipment(equip, dataGridFA, m_dicFAShape);
            else
                return null;

            ArrayList arrEquipments = FormMain2.Instance.IOManager.GetEquipments(zone);
            if (arrEquipments != null)
                arrEquipments.Add(equip);

            FormMain2.Instance.CurrentEquipments.Add(equip);
            return equip;
        }

        public void SetRFID(FireEquipment equip, string strRFID)
        {
            if (equip.LinkedShape == null)
                return;

            FireEquipment otherEquip = FormMain2.Instance.DXFManager.FindEquipment(strRFID, equip);

            if (otherEquip != null)
            {
                string strMsg = string.Format("같은 RFID를 가진 설비 [{0}]가 이미 존재합니다.\r\n기존 설비의 RFID 값을 확인해 주십시오.", otherEquip.EquipID);
                MessageBox.Show(strMsg);
                return;

                /*string strMsg = string.Format("같은 RFID를 가진 설비 [{0}]가 이미 존재합니다.\r\n기존 설비의 RFID 값을 지우고 현재 선택된 설비[{1}]로 RFID를 지정하시겠습니까?",
                    otherEquip.EquipID, equip.EquipID);

                if (MessageBox.Show(strMsg, "RFID 중복", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    ChangeRFID(otherEquip, "");
                else
                    return;*/
            }

            ChangeRFID(equip, strRFID);
        }

        private void ChangeRFID(FireEquipment equip, string strRFID)
        {
            if (m_dicFEShape.ContainsKey(equip.LinkedShape))
            {
                DataGridViewRow row = m_dicFEShape[equip.LinkedShape];
                row.Cells[0].Value = strRFID;
            }
            else if (m_dicHDShape.ContainsKey(equip.LinkedShape))
            {
                DataGridViewRow row = m_dicHDShape[equip.LinkedShape];
                row.Cells[0].Value = strRFID;
            }
            else if (m_dicFAShape.ContainsKey(equip.LinkedShape))
            {
                DataGridViewRow row = m_dicFAShape[equip.LinkedShape];
                row.Cells[0].Value = strRFID;
            }
            else
                return;

            equip.RFIDTag = strRFID;
        }

        public void ClearSelection(bool refresh = false)
        {
            dataGridFE.ClearSelection();
            dataGridHD.ClearSelection();
            dataGridFA.ClearSelection();

            if (m_selectedEquipment != null)
            {
                if (m_selectedEquipment.LinkedShape != null)
                    m_selectedEquipment.LinkedShape.Selected = false;
            }

            if (m_selectedShape != null)
                m_selectedShape.Selected = false;

            m_selectedEquipment = null;
            m_selectedShape = null;

            if (refresh)
                FormMain2.Instance.Refresh();
        }

        public void ApplyGridData()
        {
            ApplyGridData(dataGridFE);
            ApplyGridData(dataGridHD);
            ApplyGridData(dataGridFA);
        }

        private void ApplyGridData(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                FireEquipment equip = (FireEquipment)row.Tag;
                if (equip == null)
                    continue;

                if (row.Cells[0].Value == null)
                    equip.RFIDTag = "";
                else
                    equip.RFIDTag = RemoveSticky(row.Cells[0].Value.ToString());

                if (row.Cells[1].Value == null)
                    equip.EquipID = "";
                else
                    equip.EquipID = RemoveSticky(row.Cells[1].Value.ToString());
            }
        }

        // 문자열 앞뒤의 빈 문자들을 제거한다.
        public static string RemoveSticky(string strText)
        {
            string strResult = strText.TrimStart(EMPTY_CHARS);
            return strResult.TrimEnd(EMPTY_CHARS);
        }

        private void ShowControl(FireEquipment.EquipmentType type, bool show)
        {
            for (int i = 0; i < 3; i++)
            {
                if (m_arrEquipmentType[i] == type)
                {
                    m_arrShowGrid[i] = show;
                    break;
                }
            }
        }

        public void Rearrange(FireEquipment.EquipmentType firstType, bool showFE, bool showHD, bool showFA)
        {
            if (firstType == m_arrEquipmentType[1])
            {
                m_arrEquipmentType[1] = m_arrEquipmentType[0];
                m_arrEquipmentType[0] = firstType;
            }
            else if (firstType == m_arrEquipmentType[2])
            {
                m_arrEquipmentType[2] = m_arrEquipmentType[1];
                m_arrEquipmentType[1] = m_arrEquipmentType[0];
                m_arrEquipmentType[0] = firstType;
            }

            ShowControl(FireEquipment.EquipmentType.FE, showFE);
            ShowControl(FireEquipment.EquipmentType.HD, showHD);
            ShowControl(FireEquipment.EquipmentType.FA, showFA);

            ResizeControl();
        }

        public void Rearrange(bool showFE, bool showHD, bool showFA)
        {
            ShowControl(FireEquipment.EquipmentType.FE, showFE);
            ShowControl(FireEquipment.EquipmentType.HD, showHD);
            ShowControl(FireEquipment.EquipmentType.FA, showFA);

            ResizeControl();
        }

        private DataGridViewCell GetSelectedEquipIDCell(DataGridView grid)
        {
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                if (cell.ColumnIndex == 1)
                    return cell;
            }

            return null;
        }

        // TagInputMode에서만 사용됨
        public void InputEquipID(FireEquipment equip, string strInit)
        {
            FireEquipment.EquipmentType type = equip.Type;
            DataGridView grid = null;

            if (type == FireEquipment.EquipmentType.FE)
                grid = dataGridFE;
            else if (type == FireEquipment.EquipmentType.FA)
                grid = dataGridFA;
            else if (type == FireEquipment.EquipmentType.HD)
                grid = dataGridHD;
            else
                return;

            DataGridViewCell cellEquipID = GetSelectedEquipIDCell(grid);
            if (cellEquipID == null)
                return;

            grid.CurrentCell = cellEquipID;
            cellEquipID.Value = strInit;
            grid.BeginEdit(false);
        }

        public void DeleteEquipment(FireEquipment equip)
        {
            FireEquipment.EquipmentType type = equip.Type;

            DXFViewer.Shape shape = equip.LinkedShape;
            Dictionary<DXFViewer.Shape, DataGridViewRow> dicShape = null;
            DataGridView grid = null;

            if (type == FireEquipment.EquipmentType.FE)
            {
                dicShape = m_dicFEShape;
                grid = dataGridFE;
            }
            else if (type == FireEquipment.EquipmentType.HD)
            {
                dicShape = m_dicHDShape;
                grid = dataGridHD;
            }
            else if (type == FireEquipment.EquipmentType.FA)
            {
                dicShape = m_dicFAShape;
                grid = dataGridFA;
            }
            else
                return;

            DXFViewer.Layer layer = layer = FormMain2.Instance.GetEquipmentLayer(type);

            if (layer == null)
                return;

            if (!dicShape.ContainsKey(shape))
                return;

            DataGridViewRow row = dicShape[shape];
            grid.Rows.Remove(row);

            layer.Remove(shape);
        }

        public int GridWidth
        {
            get { return dataGridFE.Size.Width; }
        }

        public bool IsOpened
        {
            get
            {
                if (dataGridFE.Rows.Count > 0)
                    return true;
                if (dataGridHD.Rows.Count > 0)
                    return true;
                if (dataGridFA.Rows.Count > 0)
                    return true;

                return false;
            }
        }

        public FireEquipment SelectedEquipment
        {
            get { return m_selectedEquipment; }
        }

        // TagInputMode에서만 사용됨
        public Dictionary<int, FireEquipment> EditedFireEquipmentsInTagInputMode
        {
            get { return m_dicTagInputModeEquipment; }
        }

        public Dictionary<DXFViewer.Shape, DataGridViewRow> FEShapes
        {
            get { return m_dicFEShape; }
        }

        public Dictionary<DXFViewer.Shape, DataGridViewRow> HDShapes
        {
            get { return m_dicHDShape; }
        }

        public Dictionary<DXFViewer.Shape, DataGridViewRow> FAShapes
        {
            get { return m_dicFAShape; }
        }


        //button 이벤트
        private void btnFireExtingusher_Click(object sender, EventArgs e)
        {
            DataGridVisible(btnFireExtingusher);
        }

        private void btnFirePlug_Click(object sender, EventArgs e)
        {
            DataGridVisible(btnFirePlug);
        }

        private void btnFireAlarm_Click(object sender, EventArgs e)
        {
            DataGridVisible(btnFireAlarm);
        }

        private void DataGridVisible(RibbonButton btn)
        {
            if (btn == btnFireExtingusher)
            {
                dataGridFE.Visible = true;
                dataGridHD.Visible = false;
                dataGridFA.Visible = false;

                if (btnFireExtingusher.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = true;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = false;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            }
            else if (btn == btnFirePlug)
            {
                dataGridFE.Visible = false;
                dataGridHD.Visible = true;
                dataGridFA.Visible = false;

                if (btnFirePlug.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = true;
                    btnFireAlarm.IsChecked = false;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            }
            else if (btn == btnFireAlarm)
            {
                dataGridFE.Visible = false;
                dataGridHD.Visible = false;
                dataGridFA.Visible = true;

                if (btnFireAlarm.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = true;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            }

            btnFireExtingusher.Refresh();
            btnFirePlug.Refresh();
            btnFireAlarm.Refresh();
        }

        private void btnRefresh(RibbonButton btn1, RibbonButton btn2)
        {
            if (btn1.IsChecked == true)
            {
                btn1.IsChecked = false;
                btn1.Refresh();
            }
            else if (btn2.IsChecked == true)
            {
                btn2.IsChecked = false;
                btn2.Refresh();
            }
        }

        private void dataGridFA_MouseMove(object sender, MouseEventArgs e)
        {
            FormEquipList_MouseMove(sender, e);
        }

        private void dataGridFA_MouseDown(object sender, MouseEventArgs e)
        {
            FormEquipList_MouseDown(sender, e);
            
        }

        private void dataGridFA_MouseUp(object sender, MouseEventArgs e)
        {
            FormEquipList_MouseUp(sender, e);
        }


        private void ChangeTab(int nTab)
        {
            switch (nTab)
            {
                case 0:
                    DataGridVisible(btnFireExtingusher);
                    break;
                case 1:
                    DataGridVisible(btnFirePlug);
                    break;
                case 2:
                    DataGridVisible(btnFireAlarm);
                    break;
                default:
                    DataGridVisible(btnFireExtingusher);
                    break;
            };
        }

        private int m_nCurrentTab = 0;
        private bool m_bDragProcess = false;
        private void CalcChangeTab(Point ptDown, Point ptUp)
        {
            int dx = ptDown.X - ptUp.X;

            if (Math.Abs(dx) < 15)
            {
                return;
            }

            if (dx < 0)
            {
                m_nCurrentTab -= 1;
                if (m_nCurrentTab < 0)
                    m_nCurrentTab = 2;


            }
            else
            {
                m_nCurrentTab += 1;
                if (m_nCurrentTab > 2)
                    m_nCurrentTab = 0;
            }

            ChangeTab(m_nCurrentTab);

        }
        private void FormEquipList_MouseMove(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Left)
            {
                m_bDragProcess = true;
            }
            else
            {
                m_bDragProcess = false;
            }
        }

        private Point m_ptFirst = new Point();
        private void FormEquipList_MouseDown(object sender, MouseEventArgs e)
        {
            m_ptFirst = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
        }

        private void FormEquipList_MouseUp(object sender, MouseEventArgs e)
        {
            if( m_bDragProcess == true)
            {
                Point pt = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
                CalcChangeTab( m_ptFirst, pt);
            }
            m_bDragProcess = false;
        }

        private void SetGridViewSize()
        {
            dataGridFE.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridFE.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridFE.ColumnHeadersHeight = 30;
           
            dataGridFE.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridFE.Font = new Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            dataGridHD.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridHD.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridHD.ColumnHeadersHeight = 30;

            dataGridHD.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridHD.Font = new Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            dataGridFA.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridFA.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridFA.ColumnHeadersHeight = 30;

            dataGridFA.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridFA.Font = new Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        public void ReSizeControl()
        {
            pictureBoxCircle01.Location = new Point(pictureBoxCircle01.Location.X, this.Height - FormMain2.Instance.PanelTop.Height);
            pictureBoxCircle02.Location = new Point(pictureBoxCircle02.Location.X, this.Height - FormMain2.Instance.PanelTop.Height);
            pictureBoxCircle03.Location = new Point(pictureBoxCircle03.Location.X, this.Height - FormMain2.Instance.PanelTop.Height);

            dataGridFE.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100 ;
            dataGridHD.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100 ;
            dataGridFA.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100 ;
        }

        public void ChangeEditMode(bool isEdit = true)
        {
            //설비추가모드일땐 수정이 가능하다
            if (isEdit == true)
            {
                lblEquipList.Text = "설비리스트 편집"; 

                dataGridFE.ReadOnly = false;
                dataGridHD.ReadOnly = false;
                dataGridFA.ReadOnly = false;
                
                colStatus.ReadOnly = true;
                colHDStatus.ReadOnly = true;
                colFAStatus.ReadOnly = true;
            }
            else if(isEdit == false)
            {
                lblEquipList.Text = "설비리스트";

                dataGridFE.ReadOnly = true;
                dataGridHD.ReadOnly = true;
                dataGridFA.ReadOnly = true;
            }
        }
    }
}
