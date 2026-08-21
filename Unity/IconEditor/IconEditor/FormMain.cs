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
using UnE.View.Content;
using DBUtility2;
using SDMS;
using UnE.Util.Unity;
using System.IO;
using System.Threading;

namespace IconEditor
{
    public partial class FormMain : Form, IFormContentOwner, IChangedDataManager, IBaseViewOwner
    {
        private string PREV_DATA_FILE = "remember.dat";

        private WebDBManager m_dbMgr = null;
        private FormContentUnity m_frmContent = null;
        private ZoneManager m_zoneMgr = new ZoneManager();
        private int m_nCCTVID = 1;

        private UnE.Sensor.POI m_poi1 = null, m_poi2 = null;
        private POIManager m_poiManager = new POIManager();

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            labelFilePath.Text = "";
            if (m_zoneMgr.ReadZones())
            {
                InitData();
                RunUnity();
            }
            else
                MessageBox.Show("Zone 정보를 읽어올 수 없습니다.");

            m_zoneMgr.ReadElevation();
            m_poiManager.RightSide = checkBoxRightSide.Checked;
        }

        private void FormMain_ResizeBegin(object sender, EventArgs e)
        {
            FixSplitDistance();
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            UnFixSplitDistance();
        }

        // FormMain의 크기가 변경될때 Split Distance가 바뀌지 않도록 한다.
        private void FixSplitDistance()
        {
            splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        }

        private void UnFixSplitDistance()
        {
            splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.None;
        }

        private void RunUnity()
        {
            string strExe = System.Configuration.ConfigurationManager.AppSettings.Get("exe");
            string strWindowName = System.Configuration.ConfigurationManager.AppSettings.Get("windowName");
            string strProcessName = GetProcessName(strExe);

            // 기존에 실행되고 있던 모듈이 있으면 중지시킨다.
            UnE.Util.UtilMethods.KillProcess(strProcessName);
            UnE.View.Content.ViewUtils.RegisterContentViewOwner(this);

            UnE.View.Content.FormContentUnity form = new UnE.View.Content.FormContentUnity(this, strExe, strWindowName);

            m_frmContent = form;
            form.TopLevel = false;
            form.Parent = panel3D;
            form.Dock = DockStyle.Fill;
            panel3D.Controls.Add(form);

            m_frmContent.Init3DView();
            
            // Ortho 모드로 바꾼다음 3D를 보이게 한다.
            m_frmContent.Hide();
        }

        private string GetProcessName(string strExe)
        {
            int nIndex1 = strExe.LastIndexOf('\\');
            int nIndex2 = strExe.LastIndexOf('/');

            string strProcessName = strExe;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                if (nIndex1 > nIndex2)
                    strProcessName = strExe.Substring(nIndex1 + 1);
                else
                    strProcessName = strExe.Substring(nIndex2 + 1);
            }
            else if (nIndex1 >= 0)
                strProcessName = strExe.Substring(nIndex1 + 1);
            else if (nIndex2 >= 0)
                strProcessName = strExe.Substring(nIndex2 + 1);

            int nDotIndex = strProcessName.LastIndexOf('.');

            if (nDotIndex > 0)
                strProcessName = strProcessName.Substring(0, nDotIndex);

            return strProcessName;
        }

        private const int SC_RESTORE = 0xF120;
        private const int SC_RESTORE2 = 0xF122;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_MAXIMIZE2 = 0xF032;
        private const int SC_MINIMIZE = 0xF020;

        protected override void WndProc(ref Message m)
        {
            // WM_SYSCOMMAND
            if (m.Msg == 0x0112)
            {
                int wParam = (int)m.WParam;

                if (wParam == SC_RESTORE || wParam == SC_RESTORE2 ||
                    wParam == SC_MAXIMIZE || wParam == SC_MINIMIZE ||
                    wParam == SC_MAXIMIZE2)
                {
                    FixSplitDistance();
                }
            }
            
            base.WndProc(ref m);
        }

        #region IFormContentOwner
        public ContentOwnerTab PreviousTab
        {
            get { return ContentOwnerTab.M3D_TAB; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public Form InvokeForm
        {
            get { return this; }
        }

        public bool ExtractInside
        {
            get { return false; }
            set { }
        }

        public ContentOwnerTab CurrentTab
        {
            get { return ContentOwnerTab.M3D_TAB; }
        }

        public string ResourcePath
        {
            get { return ""; }
        }

        public bool ExtractOutside
        {
            get { return false; }
            set { }
        }

        public bool IsChangedEquipZoneCCTV
        {
            get { return false; }
            set { }
        }

        public IFormContent ContentForm
        {
            get { return m_frmContent; }
        }

        public IChangedDataManager IChangedDataManager
        {
            get { return this; }
        }

        public UnE.Sensor.POI SelectedPOI
        {
            get { return null; }
            set { }
        }

        public int ChangeTab(ContentOwnerTab tab)
        {
            return 0;
        }

        public void ChangeZoneComboBox(UnE.Spatial.Zone zone)
        {
        }

        public void Check3DViewMode(int nID)
        {
        }

        public void EnableFireReportBtn(bool bEnable, int nType)
        {
        }

        public void EnableFireReportBtn(bool bEnable)
        {
        }

        public ArrayList GetFireEquipments(UnE.Spatial.Zone zone)
        {
            return null;
        }

        public UnE.PSM.PSMMaterial GetPSMMaterial(int nMaterialType)
        {
            return null;
        }

        public UnE.PSM.PSMSensor GetPSMSensor(int nID)
        {
            return null;
        }

        public void LoadPOI(UnE.Sensor.ISensorTooltipOwner view, bool bIndoor)
        {
        }

        public void OnClick2D()
        {
        }

        public void OnClick3D()
        {
        }

        public void OnClickBothView(bool isChecked)
        {
        }

        public void OnPostPickPOI(UnE.Sensor.POI poi)
        {
        }

        public void OnReadyDataLoad()
        {
            m_frmContent.EditMode = true;
            m_frmContent.BlinkMode = false;

            if (cboBuildings.Items.Count > 0)
                cboBuildings.SelectedIndex = 0;
        }

        public void SelectIndoorZone(UnE.Spatial.Zone zone)
        {
        }

        public void SetBuilingCollapseDetect(string strPosition, bool isRealMode)
        {
        }

        public void SetEarthquakeDetect(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {
        }

        public void ShowCCTVForm(bool bShow)
        {
        }

        public void ShowEquipZoneCCTVs(int nEquipZoneID)
        {
        }
        #endregion

        #region IChangedDataManager
        public ArrayList GetDataList()
        {
            return null;
        }

        public void RemoveData(ChangedData data)
        {
        }

        public void SomethingChanged(ChangedData data)
        {
        }
        #endregion

        #region IBaseViewOwner
        public ToolStripMenuItem MenuManualCCTV
        {
            get { return null; }
        }

        public ToolStripMenuItem MenuIndoor
        {
            get { return null; }
        }

        public ToolStripMenuItem MenuManualReport
        {
            get { return null; }
        }

        public void AddCCTVEditData(UnE.Sensor.POI poi, UnE.Spatial.Zone zone)
        {
        }

        public void AddPressureSensorEditData(UnE.Sensor.POI poi, UnE.Spatial.Zone zone)
        {
        }

        public UnE.Spatial.EquipmentZone CheckEquipmentZone(UnE.Spatial.Zone zone, float x, float y)
        {
            return null;
        }

        public void EditCCTV(UnE.Sensor.CCTV cctv, string szDescription)
        {
        }

        public void EditCCTV(UnE.Sensor.CCTV cctv)
        {
        }

        public void EditFireSensor(UnE.Sensor.FireSensor sensor)
        {
        }

        public void EditPumpPressureSensor(UnE.Sensor.PumpPressureSensor sensor)
        {
        }

        public void EditSpringCooler(UnE.Sensor.SpringCooler sensor)
        {
        }

        public UnE.Spatial.Building GetBuilding(string szBuildingName)
        {
            return null;
        }

        public UnE.Spatial.Zone GetOutsideZone(float x, float y)
        {
            return null;
        }

        public UnE.Spatial.Zone GetZone(string szBuildingID, int nFloor)
        {
            return null;
        }

        public void HideAllPopup()
        {
        }

        public void ManualCCTVClicked(object sender, EventArgs e)
        {
        }

        public void MenualReportClicked(object sender, EventArgs e)
        {
        }

        public void MenuIndoorClicked(object sender, EventArgs e)
        {
        }

        public void OnBeepFinish()
        {
        }

        public void OnChangeIndoorZone(UnE.Spatial.Zone currentZone)
        {
        }

        public void OnCollapseBuilding(string buildingID, bool isReal = false)
        {
        }

        public void OnDeletePOI(UnE.Sensor.POI poi)
        {
        }

        public void OnFinishEarthquake()
        {
        }

        public void OnPostPanelMouseDown()
        {
        }

        public void RemoveCCTVPOI(int nLayerID, int nID)
        {
        }

        public void RemoveCCTVPOI(int nID)
        {
        }

        public void RequestOutdoor()
        {
        }

        public void OnMessage(string strMessageType, string strMessage)
        {
        }

        public void OnAddPOI(UnE.Sensor.POI poi)
        {
            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            panel.ChangePOIIcon(poi, "CCTV_" + m_nCCTVID.ToString());

            if (m_nCCTVID == 1)
                m_poi1 = poi;
            else if (m_nCCTVID == 2)
                m_poi2 = poi;

            radioMovePOI.Checked = true;
            m_nCCTVID++;
        }

        public void OnMovePOI(UnE.Sensor.POI poi)
        {
        }

        public void ChangeWall()
        {
        }

        public void GetWallInfo(float x, float y, float scale, float rotate)
        {

        }

        public void ChangeSpaceText()
        {

        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            m_frmContent.EditMode = true;
        }

        private void InitData()
        {
            List<UnE.Spatial.Building> buildings = m_zoneMgr.GetBuildings();

            foreach (UnE.Spatial.Building building in buildings)
            {
                cboBuildings.Items.Add(building);
            }
        }

        private void cboBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboFloors.Items.Clear();

            if (cboBuildings.SelectedIndex < 0)
                return;

            UnE.Spatial.Building building = (UnE.Spatial.Building)cboBuildings.Items[cboBuildings.SelectedIndex];

            foreach (UnE.Spatial.Zone zone in building.FloorList)
            {
                cboFloors.Items.Add(zone);
            }

            if (cboFloors.Items.Count > 0)
                cboFloors.SelectedIndex = 0;

            if (m_frmContent.Visible == false)
                m_frmContent.Show();
        }

        private void cboFloors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFloors.SelectedIndex < 0)
                return;

            UnE.Spatial.Zone zone = (UnE.Spatial.Zone)cboFloors.Items[cboFloors.SelectedIndex];
            m_frmContent.HideAllAlarmZones();
            m_frmContent.SelectScene(zone.DisplayText);

            m_frmContent.EditMode = true;
            //m_frmContent.CurrentMouseWorkMode = MouseWorkMode.SELECT_ZONE;

            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            panel.ClearPOI("CCTV");

            m_poi1 = m_poi2 = null;
            m_nCCTVID = 1;
            radioAddPOI.Checked = true;
            radioPOI_CheckedChanged(null, null);

            //ReadDataFile();
        }

        private void checkBoxEditPOI_CheckedChanged(object sender, EventArgs e)
        {
            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;

            if (checkBoxEditPOI.Checked)
                radioPOI_CheckedChanged(null, null);
            else
                panel.SelectEditMode(Panel4Unity.EditModeType.None);
        }

        private void radioPOI_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEditPOI.Checked == false)
                return;

            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;

            if (radioAddPOI.Checked)
            {
                if (m_nCCTVID <= 2)
                {
                    panel.SelectEditMode(Panel4Unity.EditModeType.AddIcon, "CCTV_" + m_nCCTVID.ToString());
                }
            }
            else
            {
                panel.SelectEditMode(Panel4Unity.EditModeType.MoveIcon, "CCTV_" + m_nCCTVID.ToString());
            }

            panel.Refresh();
        }

        private void splitContainerMain_Panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                string strFileName = files[0].ToLower();

                if (strFileName.EndsWith("csv"))
                {
                    this.Cursor = Cursors.WaitCursor;

                    ReadCSV(strFileName);

                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void splitContainerMain_Panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("csv"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void ReadCSV(string strFileName)
        {
            if (cboFloors.SelectedIndex < 0)
            {
                MessageBox.Show("선택된 건물과 층이 존재하지 않습니다.");
                return;
            }

            UnE.Spatial.Zone zone = (UnE.Spatial.Zone)cboFloors.Items[cboFloors.SelectedIndex];

            float fElevation;
            if (m_zoneMgr.GetElevation(zone, out fElevation) == false)
                return;

            if (m_nCCTVID < 3)
            {
                MessageBox.Show("POI Icon 2개가 추가되어야만 합니다.");
                return;
            }

            float poi1X, poi1Y, poi2X, poi2Y;

            if (GetCADXY(textBoxPOI1X, "POI1.X", out poi1X) == false)
                return;
            if (GetCADXY(textBoxPOI1Y, "POI1.Y", out poi1Y) == false)
                return;
            if (GetCADXY(textBoxPOI2X, "POI2.X", out poi2X) == false)
                return;
            if (GetCADXY(textBoxPOI2Y, "POI2.Y", out poi2Y) == false)
                return;

            //WriteDataFile();

            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            panel.ClearPOI("CCTV");
            panel.ShowIconLayer("CCTV");

            labelStatus.Text = "";
            labelFilePath.Text = "";

            List<int> poiIDs;
            List<string> poiTypes;
            List<bool> poiVisibles;
            List<UnE.Sensor.POI> pois = m_poiManager.LoadPOI(m_poi1, m_poi2, poi1X, poi1Y, poi2X, poi2Y, strFileName, fElevation, out poiIDs, out poiTypes, out poiVisibles);

            string strTag = string.Format("1-{0}-1-1", zone.ID);
            string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AddPOI" + strTag + ".txt";
            panel.AddPOIFile("CCTV", strFilePath, pois);

            strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ShowPOI" + strTag + ".txt";
            panel.ShowIconPOIFile("CCTV", strFilePath, poiIDs, poiTypes, poiVisibles);
            panel.Refresh();

            WritePOIInfo(pois, zone, m_poiManager.FileName);
        }

        private void WritePOIInfo(List<UnE.Sensor.POI> pois, UnE.Spatial.Zone zone, string strFileName)
        {
            int nDotIndex = strFileName.LastIndexOf('.');

            if (nDotIndex < 0)
                return;

            strFileName = strFileName.Substring(0, nDotIndex);

            int nIndex1 = strFileName.LastIndexOf('/');
            int nIndex2 = strFileName.LastIndexOf('\\');

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                if (nIndex1 > nIndex2)
                    strFileName = strFileName.Substring(nIndex1 + 1);
                else
                    strFileName = strFileName.Substring(nIndex2 + 1);
            }
            else if (nIndex1 >= 0)
                strFileName = strFileName.Substring(nIndex1 + 1);
            else if (nIndex2 >= 0)
                strFileName = strFileName.Substring(nIndex2 + 1);
            else
                return;

            string strFolderName = "POIResult";

            if (Directory.Exists(strFolderName) == false)
                Directory.CreateDirectory(strFolderName);

            string strFilePath = string.Format("{0}\\{1:0000}({2}).txt", strFolderName, zone.ID, strFileName);
            string strFilePath2 = string.Format("{0}\\all.txt", strFolderName);
            StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.UTF8);
            StreamWriter writer2 = new StreamWriter(strFilePath2, true, Encoding.UTF8);

            foreach (POIData poi in pois)
            {
                //string strLine = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", poi.SensorType, poi.SensorName, zone.ID, poi.X, poi.Y, poi.Z);
                string strLine = string.Format("Update FireSensor Set X={0}, Y={1}, Z={2} Where Description='{3}' And ZoneID={4};", poi.X, poi.Y, poi.Z, poi.SensorName, zone.ID);
                writer.WriteLine(strLine);
                writer2.WriteLine(strLine);
            }

            writer.Close();
            writer2.Close();

            labelStatus.Text = string.Format("POI {0}개 생성", pois.Count);
            labelFilePath.Text = string.Format("POI File : " + strFilePath);
            splitContainerMain.Panel1.Refresh();
        }

        private bool GetCADXY(TextBox textBox, string strTag, out float coord, bool messageBox = true)
        {
            coord = 0.0f;
            string str = textBox.Text.Trim();

            if (str.Length == 0)
            {
                if (messageBox)
                {
                    textBox.Focus();
                    MessageBox.Show("CAD좌표(" + strTag + ")가 입력되지 않았습니다.");
                }

                return false;
            }

            if (float.TryParse(str, out coord) == false)
            {
                if (messageBox)
                {
                    textBox.Focus();
                    MessageBox.Show("CAD좌표(" + strTag + ")는 숫자 형태의 값만 입력 가능합니다.");
                }

                return false;
            }

            return true;
        }

        //private void WriteDataFile()
        //{
        //    float poi1X, poi1Y, poi2X, poi2Y;

        //    if (GetCADXY(textBoxPOI1X, "POI1.X", out poi1X, false) == false)
        //        return;
        //    if (GetCADXY(textBoxPOI1Y, "POI1.Y", out poi1Y, false) == false)
        //        return;
        //    if (GetCADXY(textBoxPOI2X, "POI2.X", out poi2X, false) == false)
        //        return;
        //    if (GetCADXY(textBoxPOI2Y, "POI2.Y", out poi2Y, false) == false)
        //        return;

        //    if (m_poi1 != null && m_poi2 != null)
        //    {
        //        StreamWriter writer = new StreamWriter(PREV_DATA_FILE, false, Encoding.UTF8);

        //        string strLog = string.Format("{0:F},{1:F},{2:F},{3:F}", poi1X, poi1Y, poi2X, poi2Y);
        //        writer.WriteLine(strLog);
        //        writer.WriteLine(string.Format("{0:F},{1:F},{2:F},{3:F}", m_poi1.X, m_poi1.Z, m_poi2.X, m_poi2.Z));

        //        writer.Close();
        //    }
        //}

        //private void ReadDataFile()
        //{
        //    if (File.Exists(PREV_DATA_FILE) == false)
        //        return;

        //    StreamReader reader = new StreamReader(PREV_DATA_FILE, Encoding.UTF8);

        //    string strLine = reader.ReadLine().Trim();
        //    string[] tokens = strLine.Split(',');

        //    if (tokens.Count() == 4)
        //    {
        //        textBoxPOI1X.Text = tokens[0].Trim();
        //        textBoxPOI1Y.Text = tokens[1].Trim();
        //        textBoxPOI2X.Text = tokens[2].Trim();
        //        textBoxPOI2Y.Text = tokens[3].Trim();
        //    }

        //    strLine = reader.ReadLine().Trim();
        //    reader.Close();

        //    /*tokens = strLine.Split(',');

        //    if (tokens.Count() == 4)
        //    {
        //        float x1, y1, x2, y2;

        //        if (float.TryParse(tokens[0].Trim(), out x1) &&
        //            float.TryParse(tokens[1].Trim(), out y1) &&
        //            float.TryParse(tokens[2].Trim(), out x2) &&
        //            float.TryParse(tokens[3].Trim(), out y2))
        //        {
        //            UnE.Spatial.Zone zone = (UnE.Spatial.Zone)cboFloors.Items[cboFloors.SelectedIndex];

        //            float fElevation;
        //            if (m_zoneMgr.GetElevation(zone, out fElevation) == false)
        //                return;

        //            POIManager mgr = new POIManager();

        //            List<int> poiIDs;
        //            List<string> poiTypes;
        //            List<bool> poiVisibles;
        //            List<UnE.Sensor.POI> pois = mgr.Make2POIs(x1, y1, x2, y2, fElevation, out poiIDs, out poiTypes, out poiVisibles);

        //            if (pois.Count < 2)
        //                return;

        //            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;

        //            string strTag = string.Format("1-{0}-1-1", zone.ID);
        //            string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AddPOI" + strTag + ".txt";
        //            panel.AddPOIFile("CCTV", strFilePath, pois);

        //            strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ShowPOI" + strTag + ".txt";
        //            panel.ShowIconPOIFile("CCTV", strFilePath, poiIDs, poiTypes, poiVisibles);
        //            panel.Refresh();

        //            ArrayList arr = new ArrayList();
        //            arr.Add(pois[0]);
        //            arr.Add(pois[1]);
        //            arr.Add(strFilePath);

        //            Thread t = new Thread(new ParameterizedThreadStart(ChangePOIThread));
        //            t.Start(arr);
        //        }
        //    }*/
        //}

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //WriteDataFile();
        }

        private void btnShow2POIs_Click(object sender, EventArgs e)
        {
            Show2POI();
        }

        private void checkBoxRightSide_CheckedChanged(object sender, EventArgs e)
        {
            m_poiManager.RightSide = checkBoxRightSide.Checked;

            if (m_poiManager.HasData && m_poi1 != null && m_poi2 != null)
            {
                float poi1X, poi1Y, poi2X, poi2Y;

                if (GetCADXY(textBoxPOI1X, "POI1.X", out poi1X) == false)
                    return;
                if (GetCADXY(textBoxPOI1Y, "POI1.Y", out poi1Y) == false)
                    return;
                if (GetCADXY(textBoxPOI2X, "POI2.X", out poi2X) == false)
                    return;
                if (GetCADXY(textBoxPOI2Y, "POI2.Y", out poi2Y) == false)
                    return;

                Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
                panel.ClearPOI("CCTV");
                panel.ShowIconLayer("CCTV");

                UnE.Spatial.Zone zone = (UnE.Spatial.Zone)cboFloors.Items[cboFloors.SelectedIndex];

                List<int> poiIDs;
                List<string> poiTypes;
                List<bool> poiVisibles;
                List<UnE.Sensor.POI> pois = m_poiManager.ReloadPOI(m_poi1, m_poi2, poi1X, poi1Y, poi2X, poi2Y, out poiIDs, out poiTypes, out poiVisibles);

                string strTag = string.Format("1-{0}-1-1", zone.ID);
                string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AddPOI" + strTag + ".txt";
                panel.AddPOIFile("CCTV", strFilePath, pois);

                strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ShowPOI" + strTag + ".txt";
                panel.ShowIconPOIFile("CCTV", strFilePath, poiIDs, poiTypes, poiVisibles);
                panel.Refresh();

                WritePOIInfo(pois, zone, m_poiManager.FileName);
            }
        }

        /*private void ChangePOIThread(object arg)
        {
            ArrayList arr = (ArrayList)arg;

            UnE.Sensor.POI poi1 = (UnE.Sensor.POI)arr[0];
            UnE.Sensor.POI poi2 = (UnE.Sensor.POI)arr[1];
            string strFilePath = (string)arr[2];

            int nSleep = 200;
            int nLimit = 150;

            for (int i=0;i<nLimit && File.Exists(strFilePath);i++)
            {
                Thread.Sleep(nSleep);
            }

            if (File.Exists(strFilePath) == false)
            {
                m_nCCTVID = 3;

                this.Invoke((MethodInvoker)delegate
                {
                    Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
                    panel.ChangePOIIcon(poi1, "CCTV_1");
                    panel.ChangePOIIcon(poi2, "CCTV_1");
                    panel.Refresh();
                });
            }
        }*/

        private void Show2POI()
        {
            if (m_poi1 == null || m_poi2 == null)
                return;

            double x1 = m_poi1.X;
            double y1 = m_poi1.Z;
            double x2 = m_poi2.X;
            double y2 = m_poi2.Z;

            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            UnE.Spatial.Zone zone = (UnE.Spatial.Zone)cboFloors.Items[cboFloors.SelectedIndex];

            float fElevation;

            if (m_zoneMgr.GetElevation(zone, out fElevation) == false)
                return;

            panel.ClearPOI("CCTV");
            panel.ShowIconLayer("CCTV");

            POIData poiData = new POIData();
            poiData.X = (float)x1;
            poiData.Y = fElevation;
            poiData.Z = (float)y1;
            poiData.SensorName = "P1";
            poiData.SensorType = "열감지기";
            poiData.ID = poiData.Facility.ID = 1;
            m_poi1 = poiData;

            panel.AddIconPOI(poiData, "CCTV", (float)x1, 20, (float)y1);
            panel.ShowIconPOI(1, "CCTV", true);
            panel.ChangePOIIcon(poiData, "CCTV_1");

            poiData = new POIData();
            poiData.X = (float)x2;
            poiData.Y = fElevation;
            poiData.Z = (float)y2;
            poiData.SensorName = "P2";
            poiData.SensorType = "열감지기";
            poiData.ID = poiData.Facility.ID = 2;
            m_poi2 = poiData;

            panel.AddIconPOI(poiData, "CCTV", (float)x2, 20, (float)y2);
            panel.ShowIconPOI(2, "CCTV", true);
            panel.ChangePOIIcon(poiData, "CCTV_2");
            m_nCCTVID = 3;

            panel.Refresh();
        }
    }
}
