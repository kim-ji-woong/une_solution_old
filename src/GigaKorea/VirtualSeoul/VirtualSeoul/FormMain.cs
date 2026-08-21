using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DXFViewer;
using UnE.Geometry;
using System.IO;

namespace VirtualSeoul
{
    public partial class FormMain : Form
    {
        private Layer m_poiLayer = null;
        private Color POILayerColor = Color.Red;
        private string POIFolder = "POI";

        private string m_strBuildingID = "";
        private Dictionary<string, POIType> m_dicPOITypes = new Dictionary<string, POIType>();
        private string m_strServerURL = "";

        private POI m_selectedPOI = null;
        private bool m_lButtonClicked = false;
        private Vertex2D m_vPOIOrigin = null;
        private Vertex2D m_vLClick = null;

        private bool POIEditMode
        {
            get { return dxfControl.IsOpened && checkBoxEditPOI.Checked; }
        }

        public FormMain()
        {
            InitializeComponent();

            toolStripStatusLabel1.Text = "";
            labelPOICode.Text = "";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitLevels();
            InitPOIs();
        }

        private void InitPOIs()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();

            m_strServerURL = util.getinivalue("Server", "url");
            m_strBuildingID = util.getinivalue("Building", "ID");

            string strFolderPath = ".\\" + POIFolder;

            if (Directory.Exists(strFolderPath) == false)
                return;

            Graphics g = CreateGraphics();
            string[] files = Directory.GetFiles(strFolderPath, "*.poi");

            foreach (string strFile in files)
            {
                int nIndex1 = strFile.LastIndexOf('\\');
                int nIndex2 = strFile.LastIndexOf('.');
                string strCode = "";

                if (nIndex1 < 0)
                    strCode = strFile.Substring(0, nIndex2);
                else
                    strCode = strFile.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                string strPOIName = util.getinivalue("POI", strCode);

                if (strPOIName.Length == 0)
                    continue;

                POIType poiType = new POIType(strPOIName);

                if (poiType.LoadPOI(strFile, strCode, g))
                {
                    cboPOIs.Items.Add(poiType);
                    m_dicPOITypes[strCode] = poiType;
                }
            }
            /*string strFolderPath = ".\\" + POIFolder;

            if (Directory.Exists(strFolderPath) == false)
                return;

            Graphics g = CreateGraphics();
            string[] files = Directory.GetFiles(strFolderPath, "*.poi");

            foreach (string strFile in files)
            {
                POIType poiType = new POIType();

                if (poiType.LoadPOI(strFile, g))
                    cboPOIs.Items.Add(poiType);
            }*/
        }

        private void InitLevels()
        {
            int nHeight = 0;
            string str = textBoxLevelElevation.Text.Trim();

            if (int.TryParse(str, out nHeight) == false || nHeight <= 0)
                return;

            for (int i = 0; i < 1; i++)
            {
                Level level = new Level();
                level.FloorIndex = i;
                level.Height = nHeight;
                cboLevels.Items.Add(level);
            }
        }

        private void tsMenuOpenDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                ClearLayer();

                bool isSuccess = dxfControl.OpenDXF(dlg.FileName);
                toolStripStatusLabel1.Text = "";

                if (!isSuccess)
                {
                    string strError = "DXF 불러오기가 실패하였습니다.";
                    MessageBox.Show(strError);
                }
                else
                {
                    this.Text = dlg.FileName;
                    SetLayer(dxfControl.Layers);
                    EnableControl();
                }

                // POI 정보를 불러온다.
                LoadPOIs();

                dxfControl._Refresh();
                this.Cursor = Cursors.Arrow;
            }
        }

        private void EnableControl()
        {
            cboLevels.Enabled = textBoxLevelElevation.Enabled = cboPOIs.Enabled = true;
            radioAddPOI.Enabled = radioDeletePOI.Enabled = true;
            btnAddDownLevel.Enabled = btnAddUpLevel.Enabled = true;
            btnDeleteLevel.Enabled = btnSortLevel.Enabled = true;
            tsMenuLoadFromServer.Enabled = tsMenuSaveToServer.Enabled = true;
            radioMovePOI.Enabled = true;
            tsMenuExportPML.Enabled = tsMenuImportPML.Enabled = true;
        }

        private void SetLayer(ArrayList arrLayers)
        {
            if (arrLayers.Count == 0)
                return;

            Layer firstLayer = (Layer)arrLayers[0];

            Layer poiLayer = new Layer(firstLayer.Owner, firstLayer.GetLineType());
            poiLayer.LayerName = "POI";
            poiLayer.LineColor = POILayerColor;
            dxfControl.Layers.Add(poiLayer);
            arrLayers.Insert(0, poiLayer);

            m_poiLayer = poiLayer;
            poiLayer.Hidden = false;

            foreach (Layer layer in arrLayers)
            {
                int nRowIndex = gridLayer.Rows.Add();
                DataGridViewRow row = gridLayer.Rows[nRowIndex];

                row.Cells[0].Value = !layer.Hidden;
                row.Cells[1].Value = layer.LayerName;

                DataGridViewButtonCell cell = (DataGridViewButtonCell)row.Cells[2];
                cell.FlatStyle = FlatStyle.Flat;
                cell.Style.BackColor = layer.LineColor;
                cell.Style.SelectionBackColor = layer.LineColor;
                row.Tag = layer;
            }

            gridLayer.Rows[0].Cells[1].Selected = true;
        }

        private void ClearLayer()
        {
            gridLayer.Rows.Clear();
            m_poiLayer = null;
        }

        private void gridLayer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
                gridLayer.CommitEdit(DataGridViewDataErrorContexts.Commit);
            else if (e.ColumnIndex == 2)
            {
                DataGridViewRow row = gridLayer.Rows[e.RowIndex];
                DataGridViewButtonCell cell = (DataGridViewButtonCell)row.Cells[e.ColumnIndex];

                ColorDialog colorDialog = new ColorDialog();
                colorDialog.Color = cell.Style.BackColor;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Layer layer = (Layer)row.Tag;
                    layer.LineColor = colorDialog.Color;

                    cell.Style.BackColor = colorDialog.Color;
                    cell.Style.SelectionBackColor = colorDialog.Color;

                    dxfControl._Refresh();
                }
            }
        }

        private void gridLayer_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= gridLayer.RowCount)
                return;

            DataGridViewRow row = gridLayer.Rows[e.RowIndex];

            if (row.Tag != null && row.Tag is DXFViewer.Layer)
            {
                DXFViewer.Layer layer = (DXFViewer.Layer)row.Tag;

                if (e.ColumnIndex == 0)
                {
                    if (row.Cells[0].Value == null)
                        return;

                    layer.Hidden = !(bool)row.Cells[0].Value;
                    dxfControl._Refresh();
                }
                else if (e.ColumnIndex == 1)
                {
                    if (row.Cells[1].Value == null)
                        layer.LayerName = "";
                    else
                        layer.LayerName = row.Cells[1].Value.ToString();
                }
            }
        }

        private void dxfControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dxfControl.IsOpened == false)
                return;

            UnE.Geometry.Vertex2D vertex = dxfControl.ScreenToGlobal(e.X, e.Y);

            if (vertex != null)
                toolStripStatusLabel1.Text = string.Format("({0}, {1})", vertex.x, vertex.y);

            if (m_selectedPOI != null && m_lButtonClicked)
            {
                Vertex2D vMove = vertex - m_vLClick;
                m_selectedPOI.Move(m_vPOIOrigin + vMove);
                dxfControl._Refresh();
            }
        }

        private void dxfControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_lButtonClicked = false;

                if (checkBoxEditPOI.Checked)
                {
                    if (radioAddPOI.Checked)
                        AddPOI(e.X, e.Y);
                    else if (radioDeletePOI.Checked)
                        DeletePOI(e.X, e.Y);
                    else if (radioMovePOI.Checked)
                        PickPOI(e.X, e.Y);
                }
            }
        }

        private void PickPOI(int x, int y)
        {
            if (cboLevels.SelectedIndex < 0)
            {
                cboLevels.Focus();
                MessageBox.Show("층을 먼저 선택하세요.");
                return;
            }

            Level level = (Level)cboLevels.Items[cboLevels.SelectedIndex];
            Vertex2D vPos = dxfControl.ScreenToGlobal(x, y);

            m_selectedPOI = SelectPOI(vPos, level);

            if (m_selectedPOI != null)
            {
                m_lButtonClicked = true;
                m_vPOIOrigin = m_selectedPOI.Position;
                m_vLClick = vPos;
            }
        }

        private POI SelectPOI(Vertex2D vPos, Level level)
        {
            if (m_poiLayer == null)
                return null;

            foreach (POI poi in level.POIs)
            {
                if (poi.HitTest(vPos))
                {
                    return poi;
                }
            }

            return null;
        }

        private void DeletePOI(int x, int y)
        {
            if (cboLevels.SelectedIndex < 0)
            {
                cboLevels.Focus();
                MessageBox.Show("POI를 삭제할 층을 선택하세요.");
                return;
            }

            Level level = (Level)cboLevels.Items[cboLevels.SelectedIndex];
            Vertex2D vPos = dxfControl.ScreenToGlobal(x, y);

            if (DeletePOI(vPos, level) != null)
                dxfControl._Refresh();
        }

        private POI DeletePOI(Vertex2D vPos, Level level)
        {
            if (m_poiLayer == null)
                return null;

            foreach (POI poi in level.POIs)
            {
                if (poi.HitTest(vPos))
                {
                    foreach (Shape shape in poi.Shapes)
                    {
                        m_poiLayer.Remove(shape);
                    }

                    level.POIs.Remove(poi);
                    return poi;
                }
            }

            return null;
        }

        private void AddPOI(int x, int y)
        {
            if (cboLevels.SelectedIndex < 0)
            {
                cboLevels.Focus();
                MessageBox.Show("POI를 추가할 층을 선택하세요.");
                return;
            }

            Level level = (Level)cboLevels.Items[cboLevels.SelectedIndex];

            int nElevation;

            if (GetFloorHeight(out nElevation) == false)
                return;

            if (cboPOIs.SelectedIndex < 0)
            {
                cboPOIs.Focus();
                MessageBox.Show("추가할 POI Type을 선택하세요.");
                return;
            }

            POIType poiType = (POIType)cboPOIs.Items[cboPOIs.SelectedIndex];
            Vertex2D vPos = dxfControl.ScreenToGlobal(x, y);

            AddPOI(vPos, poiType, level);

            dxfControl._Refresh();
        }

        private void AddPOI(Vertex2D vPos, POIType poiType, Level level)
        {
            if (m_poiLayer == null)
                return;

            POI poi = poiType.MakePOI(vPos);

            if (poi != null)
            {
                foreach (Shape shape in poi.Shapes)
                {
                    m_poiLayer.Add(shape);
                }

                poi.TL = poiType.TL + vPos;
                poi.BL = poiType.BL + vPos;
                poi.BR = poiType.BR + vPos;
                level.POIs.Add(poi);

                poi.Name = string.Format("{0}_{1}_{2}", poi.POIType.Name, level.Name, level.POIs.Count);
                poi.Position = vPos;
                poi.SetShapePosition();
            }
        }

        private bool GetFloorHeight(out int nHeight)
        {
            nHeight = 0;
            string strElevation = textBoxLevelElevation.Text.Trim();

            if (strElevation.Length == 0)
            {
                textBoxLevelElevation.Focus();
                MessageBox.Show("층높이를 입력하세요.");
                return false;
            }

            if (int.TryParse(strElevation, out nHeight) == false || nHeight <= 0)
            {
                textBoxLevelElevation.Focus();
                MessageBox.Show("층높이는 0보다 큰 정수로 입력되어야만 합니다.");
                return false;
            }

            return true;
        }

        private void btnAddDownLevel_Click(object sender, EventArgs e)
        {
            Level level = (Level)cboLevels.Items[0];

            int nHeight;

            if (GetFloorHeight(out nHeight) == false)
                return;

            Level newLevel = new Level();
            newLevel.FloorIndex = level.FloorIndex - 1;
            newLevel.Height = nHeight;

            cboLevels.Items.Insert(0, newLevel);
        }

        private void btnAddUpLevel_Click(object sender, EventArgs e)
        {
            Level level = (Level)cboLevels.Items[cboLevels.Items.Count - 1];

            int nHeight;

            if (GetFloorHeight(out nHeight) == false)
                return;

            Level newLevel = new Level();
            newLevel.FloorIndex = level.FloorIndex + 1;
            newLevel.Height = nHeight;

            cboLevels.Items.Add(newLevel);
        }

        private void btnDeleteLevel_Click(object sender, EventArgs e)
        {
            if (cboLevels.SelectedIndex < 0)
            {
                cboLevels.Focus();
                MessageBox.Show("삭제할 층을 먼저 선택하세요.");
                return;
            }

            Level level = (Level)cboLevels.Items[cboLevels.SelectedIndex];

            if (level.FloorIndex == 0)
            {
                cboLevels.Focus();
                MessageBox.Show("1층은 삭제할 수 없습니다.");
                return;
            }

            cboLevels.Items.RemoveAt(cboLevels.SelectedIndex);
        }

        private void btnSortLevel_Click(object sender, EventArgs e)
        {
            int nFirstIndex = -1;
            int nLevelCount = cboLevels.Items.Count;

            for (int i=0;i<nLevelCount;i++)
            {
                Level level = (Level)cboLevels.Items[i];

                if (level.FloorIndex == 0)
                {
                    nFirstIndex = i;
                    break;
                }
            }

            if (nFirstIndex < 0)
                return;

            for (int i=0;i < nLevelCount; i++)
            {
                if (i == nFirstIndex)
                    continue;

                Level level = (Level)cboLevels.Items[i];
                level.FloorIndex = i - nFirstIndex;
            }

            int nSelectedIndex = cboLevels.SelectedIndex;
            List<Level> levels = new List<Level>();
            
            foreach (Level level in cboLevels.Items)
            {
                levels.Add(level);
            }

            cboLevels.Items.Clear();

            foreach (Level level in levels)
            {
                cboLevels.Items.Add(level);
            }

            cboLevels.SelectedIndex = nSelectedIndex;
        }

        private void cboLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPOIs();
            dxfControl._Refresh();
        }

        private void LoadPOIs()
        {
            if (m_poiLayer != null)
            {
                m_poiLayer.RemoveAll();

                if (cboLevels.SelectedIndex >= 0)
                {
                    Level level = (Level)cboLevels.Items[cboLevels.SelectedIndex];

                    foreach (POI poi in level.POIs)
                    {
                        foreach (Shape shape in poi.Shapes)
                        {
                            m_poiLayer.Add(shape);
                        }
                    }
                }
            }
        }

        private void tsMenuLoadFromServer_Click(object sender, EventArgs e)
        {
            CalcElevations();

            WebServiceManager mgr = new WebServiceManager(m_strServerURL);
            List<Level> levels = mgr.SearchLevels(m_strBuildingID, m_dicPOITypes);

            if (levels == null)
                return;

            cboLevels.Items.Clear();

            if (m_poiLayer != null)
                m_poiLayer.RemoveAll();

            foreach (Level level in levels)
            {
                cboLevels.Items.Add(level);
            }

            if (cboLevels.Items.Count > 0)
                cboLevels.SelectedIndex = 0;
        }

        private void tsMenuSaveToServer_Click(object sender, EventArgs e)
        {
            CalcElevations();

            WebServiceManager mgr = new WebServiceManager(m_strServerURL);

            if (mgr.RemoveLevels(m_strBuildingID, m_dicPOITypes))
            {
                foreach (Level level in cboLevels.Items)
                {
                    if (mgr.AddLevel(m_strBuildingID, level) == false)
                        return;
                }
            }
        }

        private void CalcElevations()
        {
            Level levelPrev = null;
            int nFirstLevelIndex = -1;

            for (int i = 0; i < cboLevels.Items.Count; i++)
            {
                Level level = (Level)cboLevels.Items[i];

                if (nFirstLevelIndex < 0)
                {
                    if (level.FloorIndex == 0)
                    {
                        nFirstLevelIndex = i;
                        levelPrev = level;
                    }
                }
                else
                {
                    level.Elevation = levelPrev.Elevation + levelPrev.Height;
                    levelPrev = level;
                }
            }

            if (nFirstLevelIndex < 0)
                return;

            levelPrev = (Level)cboLevels.Items[nFirstLevelIndex];

            for (int i = nFirstLevelIndex - 1; i >= 0; i--)
            {
                Level level = (Level)cboLevels.Items[i];
                level.Elevation = levelPrev.Elevation - level.Height;
                levelPrev = level;
            }
        }

        private void cboPOIs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPOIs.SelectedIndex < 0)
                labelPOICode.Text = "";
            else
            {
                POIType poiType = (POIType)cboPOIs.Items[cboPOIs.SelectedIndex];
                labelPOICode.Text = poiType.Code;
            }
        }

        private void dxfControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_lButtonClicked = false;
        }

        private void tsMenuExportPML_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PML Files|*.pml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "PML 저장";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                List<Level> levels = new List<Level>();

                foreach (Level level in cboLevels.Items)
                {
                    levels.Add(level);
                }

                XMLManager mgr = new XMLManager();

                if (mgr.Export(dlg.FileName, levels))
                    MessageBox.Show("파일이 생성되었습니다.");
                else
                    MessageBox.Show("파일 생성이 실패하였습니다.");
            }
        }

        private void tsMenuImportPML_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "PML Files|*.pml";
            dlg.FilterIndex = 0;
            dlg.Title = "PML 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                XMLManager mgr = new XMLManager();

                List<Level> levels = mgr.Import(dlg.FileName, m_dicPOITypes);

                if (levels == null)
                {
                    if (mgr.ErrorMessage.Length > 0)
                        MessageBox.Show("파일을 불러오는데 실패하였습니다.\r\n" + mgr.ErrorMessage);
                    else
                        MessageBox.Show("파일을 불러오는데 실패하였습니다.");
                }
                else
                {
                    Level currentLevel = (Level)cboLevels.SelectedItem;
                    cboLevels.Items.Clear();

                    Level selectedLevel = null;

                    foreach (Level level in levels)
                    {
                        cboLevels.Items.Add(level);

                        if (currentLevel != null && currentLevel.FloorIndex == level.FloorIndex)
                            selectedLevel = level;
                    }

                    if (selectedLevel != null)
                        cboLevels.SelectedItem = selectedLevel;
                }
            }
        }
    }
}
