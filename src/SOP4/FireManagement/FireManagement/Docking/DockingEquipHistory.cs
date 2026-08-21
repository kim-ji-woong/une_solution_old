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


namespace FireManagement
{
    public partial class DockingEquipHistory : Form
    {
        private FireEquipment m_selectedEquipment = null;
        private Dictionary<object, DataGridViewRow> m_dicFEShape = new Dictionary<object, DataGridViewRow>();
        private Dictionary<object, DataGridViewRow> m_dicHDShape = new Dictionary<object, DataGridViewRow>();
        private Dictionary<object, DataGridViewRow> m_dicFAShape = new Dictionary<object, DataGridViewRow>();
        private Dictionary<object, DataGridViewRow> m_dicFRShape = new Dictionary<object, DataGridViewRow>();

        private FireEquipment.EquipmentType[] m_arrEquipmentType = new FireEquipment.EquipmentType[4] { FireEquipment.EquipmentType.FE, FireEquipment.EquipmentType.HD, FireEquipment.EquipmentType.FA, FireEquipment.EquipmentType.FR };
        private bool[] m_arrShowGrid = new bool[4] { true, true, true, true };

        private static char[] EMPTY_CHARS = new char[] { ' ', '\t', '\r', '\n' };

        // FormMain이 TagInputMode일때 편집한 설비번호에 해당하는 설비들을 저장하기 위한 Dictionary 객체
        // Key : FireEquipment ID
        private Dictionary<int, FireEquipment> m_dicTagInputModeEquipment = new Dictionary<int, FireEquipment>();

        //private FormEquipHistory m_equipHistory = null;

        public DockingEquipHistory()
        {
            InitializeComponent();
            InitControls();
            dataGridFA.Visible = false;
            dataGridHD.Visible = false;

            SetGridViewSize();
            //m_equipHistory = FormMain2.Instance.EquipmentHistoryViewer;
        }
        private void InitControls()
        {
            InitColumnHeader(dataGridFE, colStatus);
            InitColumnHeader(dataGridHD, colHDStatus);
            InitColumnHeader(dataGridFA, colFAStatus);
            InitColumnHeader(dataGridFR, colFRStatus);
        }

        private void InitColumnHeader(DataGridView grid, DataGridViewColumn column2)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void SetEquipments(ArrayList arrEquipments)
        {
            dataGridFE.Rows.Clear();
            dataGridHD.Rows.Clear();
            dataGridFA.Rows.Clear();
            dataGridFR.Rows.Clear();

            m_dicFEShape.Clear();
            m_dicHDShape.Clear();
            m_dicFAShape.Clear();
            m_dicFRShape.Clear();

            Dictionary<FireEquipment, FireEquipmentHistory> dicEquipmentHistory = FormMain2.Instance.DXFManager.EquipmentHistory;
            DateTime dtNow = DateTime.Now;

            foreach (FireEquipment equip in arrEquipments)
            {
                ArrayList arrHistory = FormMain2.Instance.IOManager.FindEquipmentHistoryList(equip.ID);

                if (arrHistory != null)
                {
                    foreach (FireEquipmentHistory history in arrHistory)
                    {
                        FireEquipmentHistory tmphistory = null;

                        if (dicEquipmentHistory.ContainsKey(equip))
                        {
                            tmphistory = dicEquipmentHistory[equip];

                            if (tmphistory.Time < history.Time)
                                dicEquipmentHistory[equip] = history;
                        }
                        else
                        {
                            tmphistory = history;
                            dicEquipmentHistory[equip] = tmphistory;
                        }

                        TimeSpan span = dtNow - history.Time;

                        // 30일 이내의 점검 기록만 표시
                        if (span.TotalDays < 30.0)
                        {
                            if (equip.Type == FireEquipment.EquipmentType.FE)
                                AddEquipment(equip, history, dataGridFE, m_dicFEShape);
                            else if (equip.Type == FireEquipment.EquipmentType.HD)
                                AddEquipment(equip, history, dataGridHD, m_dicHDShape);
                            else if (equip.Type == FireEquipment.EquipmentType.FA)
                                AddEquipment(equip, history, dataGridFA, m_dicFAShape);
                            else if (equip.Type == FireEquipment.EquipmentType.FR)
                                AddEquipment(equip, history, dataGridFR, m_dicFRShape);
                        }
                    }
                }
            }
    

            this.Refresh();
        }

        public void AddEquipmentHistory(Dictionary<FireEquipment, FireEquipmentHistory> dicEquipmentHistory)
        {
            dataGridFE.Rows.Clear();
            dataGridHD.Rows.Clear();
            dataGridFA.Rows.Clear();
            dataGridFR.Rows.Clear();

            m_dicFEShape.Clear();
            m_dicHDShape.Clear();
            m_dicFAShape.Clear();
            m_dicFRShape.Clear();

            foreach (KeyValuePair<FireEquipment, FireEquipmentHistory> pair in dicEquipmentHistory)
            {
                FireEquipment equip = pair.Key;
                FireEquipmentHistory history = pair.Value;

                if (equip.Type == FireEquipment.EquipmentType.FE)
                    AddEquipment(equip, history, dataGridFE, m_dicFEShape);
                else if (equip.Type == FireEquipment.EquipmentType.HD)
                    AddEquipment(equip , history, dataGridHD, m_dicHDShape);
                else if (equip.Type == FireEquipment.EquipmentType.FA)
                    AddEquipment(equip, history, dataGridFA, m_dicFAShape);
                else if (equip.Type == FireEquipment.EquipmentType.FR)
                    AddEquipment(equip, history, dataGridFR, m_dicFRShape);

                FormMain2.Instance.IOManager.AddEquipmentHistory(history);
            }

            this.Refresh();
        }

        private void AddEquipment(FireEquipment equip, FireEquipmentHistory history, DataGridView grid, Dictionary<object, DataGridViewRow> dicShape)
        {
            DataGridViewRow row = new DataGridViewRow();

            row.Height = 45;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = " " + equip.EquipID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            
            //DateTime dt = DateTime.ParseExact(history.Time.ToString(), "yyyy-MM-dd tt hh:mm:ss", null);
            DateTime dt = history.Time;
            //cell.Value = " " + dt;
            cell.Value = " " + history.Time;
            row.Cells.Add(cell);


            cell = new DataGridViewTextBoxCell();
            cell.Value = " " + FireEquipmentHistory.GetStatusText(history.Status);
            row.Cells.Add(cell);


            row.Tag = equip;
            grid.Rows.Add(row);

            if (equip.LinkedShape == null)
                FormMain2.Instance.DXFManager.AddEquipmentObjectToDXF(equip);
            else
            {
                ControlLayer layer = FormMain2.Instance.GetEquipmentLayer(equip.Type);

                if (layer == null)
                {
                    layer = FormMain2.Instance.DXFManager.MakeEquipmentLayer(equip.Type);
                }

                if (layer != null && !layer.Shapes.Contains(equip.LinkedShape))
                    layer.Add(equip.LinkedShape);
            }

            if (equip.LinkedShape != null)
                dicShape[equip.LinkedShape] = row;
        }

        public FireEquipment FindEquipment(object shape)
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
            else if (m_dicFRShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFRShape[shape];
                return (FireEquipment)row.Tag;
            }

            return null;
        }

        public bool SelectShape(object shape)
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
            }
            else if (m_dicHDShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicHDShape[shape];
                dataGridHD.CurrentCell = row.Cells[0];
                m_dicHDShape[shape].Selected = true;

                equip = (FireEquipment)row.Tag;
            }
            else if (m_dicFAShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFAShape[shape];
                dataGridFA.CurrentCell = row.Cells[0];
                m_dicFAShape[shape].Selected = true;

                equip = (FireEquipment)row.Tag;
            }
            else if (m_dicFRShape.ContainsKey(shape))
            {
                DataGridViewRow row = m_dicFRShape[shape];
                dataGridFR.CurrentCell = row.Cells[0];
                m_dicFRShape[shape].Selected = true;

                equip = (FireEquipment)row.Tag;
            }

            if (m_selectedEquipment == equip)
                return equip != null;
            else
            {
                if (m_selectedEquipment != null && m_selectedEquipment.LinkedShape != null)
                    FormMain2.Instance.DrawingControl.SelectShape(m_selectedEquipment.LinkedShape, false);

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

            FireEquipmentHistory history = new FireEquipmentHistory();

            FireEquipment equip = new FireEquipment();

            

            equip.DXFObjID = strDxfObjID;
            equip.EquipID = strEquipID;
            equip.Position = new PointF(x, y);
            equip.RFIDTag = strRFID;
            equip.RFIDTagID = strRFIDTagID;
            equip.Type = type;
            equip.Zone = zone;

            if (equip.Type == FireEquipment.EquipmentType.FE)
                AddEquipment(equip, history, dataGridFE, m_dicFEShape);
            else if (equip.Type == FireEquipment.EquipmentType.HD)
                AddEquipment(equip, history, dataGridHD, m_dicHDShape);
            else if (equip.Type == FireEquipment.EquipmentType.FA)
                AddEquipment(equip, history, dataGridFA, m_dicFAShape);
            else if (equip.Type == FireEquipment.EquipmentType.FR)
                AddEquipment(equip, history, dataGridFR, m_dicFRShape);
            else
                return null;

            //if (type == FireEquipment.EquipmentType.FE)
            //    AddEquipment(history, dataGridFE, m_dicFEShape);
            //else if (type == FireEquipment.EquipmentType.HD)
            //    AddEquipment(history, dataGridHD, m_dicHDShape);
            //else if (type == FireEquipment.EquipmentType.FA)
            //    AddEquipment(history, dataGridFA, m_dicFAShape);
            //else
            //    return null;

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
            else if (m_dicFRShape.ContainsKey(equip.LinkedShape))
            {
                DataGridViewRow row = m_dicFRShape[equip.LinkedShape];
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
            dataGridFR.ClearSelection();

            if (m_selectedEquipment != null)
            {
                if (m_selectedEquipment.LinkedShape != null)
                    FormMain2.Instance.DrawingControl.SelectShape(m_selectedEquipment.LinkedShape, false);
            }

            m_selectedEquipment = null;

            if (refresh)
                FormMain2.Instance.Refresh();
        }

        public void ApplyGridData()
        {
            ApplyGridData(dataGridFE);
            ApplyGridData(dataGridHD);
            ApplyGridData(dataGridFA);
            ApplyGridData(dataGridFR);
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
            int nTypeCount = m_arrEquipmentType.Count();

            for (int i = 0; i < nTypeCount; i++)
            {
                if (m_arrEquipmentType[i] == type)
                {
                    m_arrShowGrid[i] = show;
                    break;
                }
            }
        }

        public void Rearrange(FireEquipment.EquipmentType firstType, bool showFE, bool showHD, bool showFA, bool showFR)
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
            else if (firstType == m_arrEquipmentType[3])
            {
                m_arrEquipmentType[3] = m_arrEquipmentType[2];
                m_arrEquipmentType[2] = m_arrEquipmentType[1];
                m_arrEquipmentType[1] = m_arrEquipmentType[0];
                m_arrEquipmentType[0] = firstType;
            }

            ShowControl(FireEquipment.EquipmentType.FE, showFE);
            ShowControl(FireEquipment.EquipmentType.HD, showHD);
            ShowControl(FireEquipment.EquipmentType.FA, showFA);
            ShowControl(FireEquipment.EquipmentType.FR, showFR);

            //ResizeControl();
        }

        public void Rearrange(bool showFE, bool showHD, bool showFA, bool showFR)
        {
            ShowControl(FireEquipment.EquipmentType.FE, showFE);
            ShowControl(FireEquipment.EquipmentType.HD, showHD);
            ShowControl(FireEquipment.EquipmentType.FA, showFA);
            ShowControl(FireEquipment.EquipmentType.FR, showFR);

            //ResizeControl();
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
            else if (type == FireEquipment.EquipmentType.FR)
                grid = dataGridFR;
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
            Dictionary<FireEquipment, FireEquipmentHistory> dicEquipmentHistory = FormMain2.Instance.DXFManager.EquipmentHistory;
   
            if (dicEquipmentHistory.ContainsKey(equip) != false)
            {

                FireEquipment.EquipmentType type = equip.Type;

                object shape = equip.LinkedShape;
                Dictionary<object, DataGridViewRow> dicShape = null;
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
                else if (type == FireEquipment.EquipmentType.FR)
                {
                    dicShape = m_dicFRShape;
                    grid = dataGridFR;
                }
                else
                    return;

                ControlLayer layer = layer = FormMain2.Instance.GetEquipmentLayer(type);

                if (layer == null)
                    return;

                if (!dicShape.ContainsKey(shape))
                    return;

                DataGridViewRow row = dicShape[shape];
                grid.Rows.Remove(row);

                this.Refresh();
            }

            
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
                if (dataGridFR.Rows.Count > 0)
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

        public Dictionary<object, DataGridViewRow> FEShapes
        {
            get { return m_dicFEShape; }
        }

        public Dictionary<object, DataGridViewRow> HDShapes
        {
            get { return m_dicHDShape; }
        }

        public Dictionary<object, DataGridViewRow> FAShapes
        {
            get { return m_dicFAShape; }
        }

        public Dictionary<object, DataGridViewRow> FRShapes
        {
            get { return m_dicFRShape; }
        }

        //button 이벤트
        private void btn_Click(object sender, EventArgs e)
        {
            DataGridVisible((RibbonButton)sender);
        }

        private void DataGridVisible(RibbonButton btn)
        {
            if (btn == btnFireExtingusher)
            {
                dataGridFE.Visible = true;
                dataGridHD.Visible = false;
                dataGridFA.Visible = false;
                dataGridFR.Visible = false;

                if (btnFireExtingusher.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = true;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = false;
                    btnFireReceiver.IsChecked = false;
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
                dataGridFR.Visible = false;

                if (btnFirePlug.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = true;
                    btnFireAlarm.IsChecked = false;
                    btnFireReceiver.IsChecked = false;
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
                dataGridFR.Visible = false;

                if (btnFireAlarm.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = true;
                    btnFireReceiver.IsChecked = false;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            }
            else if (btn == btnFireReceiver)
            {
                dataGridFE.Visible = false;
                dataGridHD.Visible = false;
                dataGridFA.Visible = false;
                dataGridFR.Visible = true;

                if (btnFireReceiver.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = false;
                    btnFireReceiver.IsChecked = true;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            }

            btnFireExtingusher.Refresh();
            btnFirePlug.Refresh();
            btnFireAlarm.Refresh();
            btnFireReceiver.Refresh();
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

        private void dataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridView grid = (DataGridView)sender;
            FireEquipment equip = (FireEquipment)grid.Rows[e.RowIndex].Tag;

            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;

            if (FormMain2.Instance.EquipmentHistoryViewer == null || FormMain2.Instance.EquipmentHistoryViewer.IsDisposed)
            {
                FormMain2.Instance.EquipmentHistoryViewer = new FormEquipHistory();

                FormMain2.Instance.EquipmentHistoryViewer.StartPosition = FormStartPosition.Manual;
                Point pt = FormMain2.Instance.ViewControl.PointToScreen(new Point(FormMain2.Instance.ViewControl.PanelRightBar.Location.X -5
                    , FormMain2.Instance.ViewControl.PanelRightBar.Location.Y));

                FormMain2.Instance.EquipmentHistoryViewer.Location = new Point(pt.X - FormMain2.Instance.EquipmentHistoryViewer.Size.Width , pt.Y);
            }
            FormMain2.Instance.EquipmentHistoryViewer.Show(equip);
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
                FormMain2.Instance.DrawingControl.SelectShape(equip.LinkedShape, true);

            if (m_selectedEquipment == equip)
                return;
            else
            {
                if (m_selectedEquipment != null && m_selectedEquipment.LinkedShape != null)
                    FormMain2.Instance.DrawingControl.SelectShape(m_selectedEquipment.LinkedShape, false);

                m_selectedEquipment = equip;
                EventManager.Instance.ProcessEvent(Event.EQUIP_SELECTED, m_selectedEquipment);
                FormMain2.Instance.DrawingControl.Refresh();
            }
        }

        private void SetGridViewSize()
        {
            SetGridViewSize(dataGridFE);
            SetGridViewSize(dataGridHD);
            SetGridViewSize(dataGridFA);
            SetGridViewSize(dataGridFR);
        }

        private void SetGridViewSize(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 30;

            grid.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            grid.Font = new Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        public void ReSizeControl()
        {
            pictureBoxCircle01.Location = new Point(pictureBoxCircle01.Location.X, this.Height - FormMain2.Instance.PanelTop.Height);
            pictureBoxCircle02.Location = new Point(pictureBoxCircle02.Location.X, this.Height - FormMain2.Instance.PanelTop.Height);
            pictureBoxCircle03.Location = new Point(pictureBoxCircle03.Location.X, this.Height - FormMain2.Instance.PanelTop.Height);

            dataGridFE.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100;
            dataGridHD.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100;
            dataGridFA.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100;
            dataGridFR.Height = pictureBoxCircle01.Location.Y - FormMain2.Instance.PanelTop.Height - 100;
        }
    }
}
