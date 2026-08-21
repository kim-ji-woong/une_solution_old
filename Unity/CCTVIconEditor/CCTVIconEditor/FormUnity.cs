using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnE.View.Content;
using DBUtility2;
using SDMS;
using System.Collections;
using UnE.Util.Unity;
using UnE.Sensor;

namespace CCTVIconEditor
{
    public partial class FormUnity : Form, IFormContentOwner, IChangedDataManager, IBaseViewOwner
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private string m_strExe = "", m_strWindowName = "";
        private FormContentUnity m_frmContent = null;
        private WebDBManager m_dbMgr = new WebDBManager(1);

        public FormUnity()
        {
            InitializeComponent();
            m_strExe = System.Configuration.ConfigurationManager.AppSettings.Get("exe");
            m_strWindowName = System.Configuration.ConfigurationManager.AppSettings.Get("windowName");
        }

        private void FormUnity_Load(object sender, EventArgs e)
        {
            UnE.View.Content.ViewUtils.RegisterContentViewOwner(this);

            UnE.View.Content.FormContentUnity form = new UnE.View.Content.FormContentUnity(this, m_strExe, m_strWindowName);

            m_frmContent = form;
            form.TopLevel = false;
            form.Parent = panelBody;
            form.Dock = DockStyle.Fill;
            panelBody.Controls.Add(form);

            m_frmContent.Init3DView();
            m_frmContent.Show();

            /*ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = m_strExe;
            var process = Process.Start(info);

            // Milli Second
            int nLimit = 5000;
            int nSleep = 100;

            for (int i=0;i<nLimit && process.MainWindowHandle == IntPtr.Zero; i+=nSleep)
            {
                System.Threading.Thread.Sleep(nSleep);
            }

            SetParent(process.MainWindowHandle, panelBody.Handle);*/
        }

        public void SelectScene(string strSceneName)
        {
            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            panel.HideAllAlarmZones();
            panel.SelectScene(strSceneName);
        }

        public void ShowAlarmZone(string strZoneName)
        {
            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            panel.ShowAlarmZone(strZoneName, true);
        }

        public void SaveImage(string strImagePath)
        {
            string strPath = m_frmContent.SaveToTempImage();

            for (int i = 0; i < 5; i++)
            {
                if (System.IO.File.Exists(strPath))
                {
                    try
                    {
                        System.IO.File.Copy(strPath, strImagePath);
                        System.IO.File.Delete(strPath);
                        break;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        public void AddPOIFile(List<CCTV> cctvs)
        {
            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            panel.ClearPOI("");

            List<POI> pois = new List<POI>();
            List<int> poiIDs = new List<int>();
            List<string> poiTypes = new List<string>();
            List<bool> poiVisibles = new List<bool>();

            foreach (CCTV cctv in cctvs)
            {
                UnE.Sensor.CCTV _cctv = new UnE.Sensor.CCTV();
                _cctv.ID = cctv.ID;

                POI poi = new POI();
                poi.ID = _cctv.ID;
                poi.Facility = _cctv;
                poi.X = cctv.X;
                poi.Y = cctv.Y;
                poi.Z = cctv.Z;
                _cctv.POI = poi;

                pois.Add(poi);
                poiIDs.Add(_cctv.ID);
                poiTypes.Add("CCTV");
                poiVisibles.Add(true);
            }

            string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AddPOI.txt";
            panel.AddPOIFile("CCTV", strFilePath, pois);

            strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ShowPOI.txt";
            panel.ShowIconPOIFile("CCTV", strFilePath, poiIDs, poiTypes, poiVisibles);
        }

        private const string CCTV_TYPE = "CCTV";
        private const string CCTV_SELECTED_TYPE = "CCTVSelected";

        public void SelectCCTV(CCTV cctv)
        {
            UnE.Sensor.POI poi = new UnE.Sensor.POI();
            poi.ID = cctv.ID;
            poi.Facility = new UnE.Sensor.CCTV();

            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            //panel.SelectPOI(cctv.ID, "CCTV");
            panel.RollBackPOIIcon(CCTV_TYPE);
            panel.ChangePOIIcon(poi, CCTV_SELECTED_TYPE);
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
            System.Diagnostics.Trace.WriteLine("OnMessage : " + strMessageType + ", " + strMessage);

            if (strMessageType == "POIMoved")
            {
                OnPOIMoved(strMessage);
            }
        }

        private void OnPOIMoved(string strMessage)
        {
            string[] tokens = strMessage.Split(',');

            if (tokens.Count() == 4)
            {
                int nID;
                float x, y, z;

                if (int.TryParse(tokens[0].Trim(), out nID) == false)
                    return;

                if (float.TryParse(tokens[1].Trim(), out x) == false ||
                    float.TryParse(tokens[2].Trim(), out y) == false ||
                    float.TryParse(tokens[3].Trim(), out z) == false)
                    return;

                FormMain.Instance.OnPOIMoved(nID, x, y, z);
            }
        }

        public void OnAddPOI(POI poi)
        {
        }
        
        public void OnMovePOI(POI poi)
        {
        }

        public void ChangeWall()
        {
        }

        public void ChangeSpaceText()
        {
        }

        public void GetWallInfo(float x, float y, float scale, float rotate)
        {
        }

        #endregion
    }
}
