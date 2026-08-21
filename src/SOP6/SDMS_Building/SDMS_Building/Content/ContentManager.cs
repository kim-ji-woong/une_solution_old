using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.View.Content;
using DBUtility2;
using System.Collections;
using System.IO;
using SDMS;
using UnE.Sensor;
using UnE.Spatial;
using UnE.PSM;
using libSensorProcess;
using SDMS_Building.Data;

namespace SDMS_Building.Content
{
    public class ContentManager : IFormContentOwner, IBaseViewOwner, IChangedDataManager
    {
        public enum ViewMode { Ogre3D = 0, Unity3D, View2D };

        private Panel m_panelBody = null;
        private IFormContent m_ContentForm = null;
        private WebDBManager m_dbMgr = null;
        private FormMain m_frmMain = null;
        private ContentOwnerTab m_prevTab = ContentOwnerTab.M3D_TAB;
        private ContentOwnerTab m_currentTab = ContentOwnerTab.M3D_TAB;
        private System.Timers.Timer m_timer = new System.Timers.Timer();

        public ContentManager(Panel panelBody, WebDBManager dbMgr, FormMain frmMain)
        {
            m_panelBody = panelBody;
            m_dbMgr = dbMgr;
            m_frmMain = frmMain;

            SDMSPopupFactory.Instance.Init();
            CreateContentForm();
            Init3DView();
        }

        private void CreateContentForm()
        {
            try
            {
                m_ContentForm = CreateContentFormBySiteID(m_dbMgr.SiteID);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("3D 환경을 초기화 하지 못하였습니다.\n모니터링을 종료합니다.");
                System.Diagnostics.Trace.WriteLine(ex.Message);

                Application.Exit();
            }
        }

        private IFormContent CreateContentFormBySiteID(int nSiteID)
        {
            ViewUtils.RegisterContentViewOwner(this);

            ViewMode mainViewMode;
            string strUnityExe, strUnityWindowName;

            if (ReadMainViewMode(nSiteID, out mainViewMode, out strUnityExe, out strUnityWindowName) == false)
                return null;

            if (mainViewMode == ViewMode.Unity3D)
            {
                KillUnityProcess(strUnityExe);
                UnE.View.Content.FormContentUnity form = new UnE.View.Content.FormContentUnity(this, strUnityExe, strUnityWindowName);

                m_ContentForm = form;
                form.TopLevel = false;
                form.Parent = m_panelBody;
                form.Dock = DockStyle.Fill;
                m_panelBody.Controls.Add(form);
                return form;
            }
            else if (mainViewMode == ViewMode.View2D)
            {
                FormContent2DOnly form = new FormContent2DOnly(this);

                m_ContentForm = form;
                form.TopLevel = false;
                form.Parent = m_panelBody;
                form.Dock = DockStyle.Fill;
                m_panelBody.Controls.Add(form);
                return form;
            }
            else if (mainViewMode == ViewMode.Ogre3D)
            {
                FormContent2D form = new FormContent2D(this);

                m_ContentForm = form;
                form.TopLevel = false;
                form.Parent = m_panelBody;
                form.Dock = DockStyle.Fill;
                m_panelBody.Controls.Add(form);
                return form;
            }

            return null;
        }

        private bool ReadMainViewMode(int nSiteID, out ViewMode mainViewMode, out string strUnityExe, out string strUnityWindowName)
        {
            mainViewMode = ViewMode.Unity3D;
            strUnityExe = strUnityWindowName = "";

            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'MainViewMode' and SiteID = " + nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            int nMode;

            if (int.TryParse(strValue.Trim(), out nMode) == false)
                return false;

            if (nMode < (int)ViewMode.Ogre3D || nMode > (int)ViewMode.View2D)
                return false;

            mainViewMode = (ViewMode)nMode;

            if (mainViewMode == ViewMode.Unity3D)
            {
                string strExe = "UnityExePath", strWindow = "UnityWindowName";
                strSQL = string.Format("Select PropertyName, PropertyValue from OptionSDMS where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}", strExe, strWindow, nSiteID);
                arrResult = m_dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    string strName = WebDBManager.GetStringField(arrResult[i]);
                    strValue = WebDBManager.GetStringField(arrResult[i + 1]);

                    if (strName == null)
                        continue;

                    strName = strName.Trim();

                    if (string.Compare(strName, strExe, true) == 0)
                        strUnityExe = strValue.Trim();
                    else if (string.Compare(strName, strWindow, true) == 0)
                        strUnityWindowName = strValue.Trim();
                }

                return strUnityExe.Length > 0 && strUnityWindowName.Length > 0;
            }

            return true;
        }

        // 기존에 실행되고 있던 프로세스가 있으면 종료시킨다..
        private void KillUnityProcess(string strUnityExe)
        {
            int nIndex1 = strUnityExe.LastIndexOf('\\');
            int nIndex2 = strUnityExe.LastIndexOf('/');

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                if (nIndex1 > nIndex2)
                    strUnityExe = strUnityExe.Substring(nIndex1 + 1);
                else
                    strUnityExe = strUnityExe.Substring(nIndex2 + 1);
            }
            else if (nIndex1 >= 0)
                strUnityExe = strUnityExe.Substring(nIndex1 + 1);
            else if (nIndex2 >= 0)
                strUnityExe = strUnityExe.Substring(nIndex2 + 1);

            int nIndex = strUnityExe.LastIndexOf('.');

            if (nIndex > 0)
                strUnityExe = strUnityExe.Substring(0, nIndex);

            UnE.View.Content.FormContentUnity.KillProcess(strUnityExe);
        }

        private bool m_bInit3DView = false;
        public void Init3DView()
        {
            if (m_bInit3DView == true)
                return;

            m_bInit3DView = true;

            string strSkinFolder = StylesPath();
            m_ContentForm.Show();

            m_ContentForm.Init3DView();
            ShowLayer(ID.ID_LAYER_ALARMSTA, false);
            ShowLayer(ID.ID_LAYER_RECIVER, false);
            System.Diagnostics.Trace.WriteLine("Page Load: " + DateTime.Now);

            m_ContentForm.CurrentMouseWorkMode = MouseWorkMode.PICK;

            m_timer.Interval = 5000;
            m_timer.Elapsed += OnTimer;
            m_timer.Start();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ProcessManager.Instance.CurrentDetectProcess.Count == 0/* && m_noProcessDisaster.CheckTimeout()*/)
            {
                if (m_ContentForm != null && !m_ContentForm.IsDisposed)
                {
                    //m_ContentForm.HideZoneVolume();
                    //m_ContentForm.RedrawWindow();
                }
            }
        }

        public bool ShowLayer(int id, bool bShow)
        {
            return m_ContentForm.ShowLayer(id, bShow);
        }

        private string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");
            return strExePath + "\\Styles\\";
        }

        public static string EnginePath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            return szMainPath;
        }

        public void AddContentForm()
        {
            Form form = (Form)m_ContentForm;
            form.Parent = m_panelBody;
            form.Dock = DockStyle.Fill;
            m_panelBody.Controls.Add(form);
        }

        public Form RemoveContentForm()
        {
            Form form = (Form)m_ContentForm;
            m_panelBody.Controls.Remove(form);
            return form;
        }

        #region IFormContentOwner 인터페이스
        public ContentOwnerTab PreviousTab
        {
            get { return m_prevTab; }
        }

        public ContentOwnerTab CurrentTab
        {
            get { return m_currentTab; }
            set
            {
                m_prevTab = m_currentTab;
                m_currentTab = value;
            }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public Form InvokeForm
        {
            get { return m_frmMain; }
        }

        // 현재 사용하지 않음
        public bool ExtractInside
        {
            get { return true; }
            set { }
        }

        public string ResourcePath
        {
            get { return EnginePath(); }
        }

        // 현재 사용하지 않음
        public bool ExtractOutside
        {
            get { return true; }
            set { }
        }

        private bool m_isChangedEquipZoneCCTV = false;
        public bool IsChangedEquipZoneCCTV
        {
            get { return m_isChangedEquipZoneCCTV; }
            set { m_isChangedEquipZoneCCTV = value; }
        }

        public IFormContent ContentForm
        {
            get { return m_ContentForm; }
        }

        public IChangedDataManager IChangedDataManager
        {
            get { return this; }
        }

        private POI m_poiSelected = null;

        public POI SelectedPOI
        {
            get { return m_poiSelected; }
            set { m_poiSelected = value; }
        }

        public int ChangeTab(ContentOwnerTab tab)
        {
            return m_frmMain.ChangeTab(tab);
        }

        public void ChangeZoneComboBox(Zone zone)
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

        public ArrayList GetFireEquipments(Zone zone)
        {
            return FormMain.Instance.DataManager.GetFireEquipments(zone);
        }

        public PSMMaterial GetPSMMaterial(int nMaterialType)
        {
            return Data.PSMManager.Instance.GetMaterial(nMaterialType);
        }

        public PSMSensor GetPSMSensor(int nID)
        {
            return Data.PSMManager.Instance.GetSensor(nID);
        }
        
        public void LoadPOI(ISensorTooltipOwner view, bool bIndoor)
        {
            FormMain.Instance.DataManager.LoadPOI(m_dbMgr, view, bIndoor);
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

        public void OnPostPickPOI(POI poi)
        {
        }

        public void OnReadyDataLoad()
        {
            System.Diagnostics.Trace.WriteLine("OnReadyDataLoad : 3D Loading 완료");
            m_frmMain.OnReadyDataLoad();
        }

        public void SelectIndoorZone(Zone zone)
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
            System.Diagnostics.Trace.WriteLine("SHowEquipZoneCCTVs : EquipZoneID(" + nEquipZoneID + ")");
        }
        #endregion

        #region IBaseViewOwner
        public ToolStripMenuItem MenuManualCCTV
        {
            get { return m_ContentForm.GetMenu("ManualCCTV"); }
        }

        public ToolStripMenuItem MenuIndoor
        {
            get { return m_ContentForm.GetMenu("Indoor"); }
        }

        public ToolStripMenuItem MenuManualReport
        {
            get { return m_ContentForm.GetMenu("ManualReport"); }
        }

        public void AddCCTVEditData(POI poi, Zone zone)
        {
            CCTV cctv = (CCTV)poi.Facility;
            if (cctv == null)
                return;

            EditCCTV editCCTV = new EditCCTV(cctv);
            editCCTV.Position = new UnE.Geometry.Vertex3F(poi.X, poi.Y, poi.Z);
            editCCTV.Zone = zone;
            editCCTV.AddToManager(this);

            poi.Zone = editCCTV.Zone;
        }

        public void AddPressureSensorEditData(POI poi, Zone zone)
        {
        }

        public EquipmentZone CheckEquipmentZone(Zone zone, float x, float y)
        {
            return ZoneManager.Instance.CheckEquipmentZone(zone, x, y);
        }

        public void EditCCTV(CCTV cctv, string szDescription)
        {
            EditCCTV editCCTV = new EditCCTV(cctv);
            if (szDescription != null && szDescription != "")
            {
                editCCTV.Description = szDescription;
            }
            editCCTV.AddToManager(this);
        }

        public void EditCCTV(CCTV cctv)
        {
            EditCCTV editCCTV = new EditCCTV(cctv);
            editCCTV.AddToManager(this);
        }

        public void EditFireSensor(FireSensor sensor)
        {

        }

        public void EditPumpPressureSensor(PumpPressureSensor sensor)
        {

        }

        public void EditSpringCooler(SpringCooler sensor)
        {

        }

        public Building GetBuilding(string szBuildingName)
        {
            string szTemp = szBuildingName;

            if (UnE.SOP.ProxySOP.Instance.SiteID == 100)
            {
                // 서울대 버전은 z제거
                if (szBuildingName.StartsWith("z"))
                {
                    szTemp = szBuildingName.Remove(0, 1);
                }
            }

            if (UnE.SOP.ProxySOP.Instance.SiteID == 3)
            {
                // 에너지 광교 버전은 _MeshPart변환
                if (szBuildingName.Contains("_MeshPart"))
                {
                    string[] sp = szBuildingName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sp != null)
                    {
                        szTemp = sp[0];
                    }
                }
            }
            if (UnE.SOP.ProxySOP.Instance.SiteID == 101)
            {
                // 예:607 공동연구소동 처리 (부산대는 _01,_02 등으로 처리할 예정)
                if (szBuildingName.Contains("_0"))
                {
                    string[] sp = szBuildingName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sp != null)
                    {
                        szTemp = sp[0];
                    }
                }
            }
            if (UnE.SOP.ProxySOP.Instance.SiteID == 999 || UnE.SOP.ProxySOP.Instance.SiteID == 102)
            {
                if (szBuildingName.Contains("동"))
                {
                    szTemp = "APT" + szBuildingName.Replace("동", "");
                }
            }

            return ZoneManager.Instance.GetBuilding(szTemp);
        }

        public Zone GetOutsideZone(float x, float y)
        {
            return ZoneManager.Instance.GetOutsideZone(x, y);
        }

        public Zone GetZone(string szBuildingID, int nFloor)
        {
            return ZoneManager.Instance.GetZone(szBuildingID, nFloor);
        }

        public void HideAllPopup()
        {
        }

        public void ManualCCTVClicked(object sender, EventArgs e)
        {
            m_ContentForm.ManualCCTVClick(sender, e);
        }

        public void MenualReportClicked(object sender, EventArgs e)
        {
            m_ContentForm.ManualReportClick(sender, e);
        }

        public void MenuIndoorClicked(object sender, EventArgs e)
        {
            m_ContentForm.IndoorMenuClick(sender, e);
        }

        public void OnBeepFinish()
        {
            System.Diagnostics.Trace.WriteLine("OnBeepFinish");
        }

        public void OnChangeIndoorZone(Zone currentZone)
        {
        }

        public void OnCollapseBuilding(string buildingID, bool isReal = false)
        {
        }

        public void OnDeletePOI(POI poi)
        {
            FormMain.Instance.OnDeletePOI(poi);
            /*switch (poi.Type)
            {
                case IFacility.FacilityType.CCTV:
                    EditCCTV cctv = new EditCCTV((CCTV)poi.Facility);
                    cctv.IsDeleting = true;
                    cctv.AddToManager(this);
                    break;

                case IFacility.FacilityType.FIRE_SENSOR:
                    EditFireSensor fireSensor = new EditFireSensor((FireSensor)poi.Facility);
                    fireSensor.IsDeleting = true;
                    fireSensor.AddToManager(this);
                    break;

                case IFacility.FacilityType.COOLER_SENSOR:
                    EditSpringCooler coolingSensor = new EditSpringCooler((SpringCooler)poi.Facility);
                    coolingSensor.IsDeleting = true;
                    coolingSensor.AddToManager(this);
                    break;

                case IFacility.FacilityType.PRESSURE_SENSOR:
                    EditPumpPressuerSensor pressureSensor = new EditPumpPressuerSensor((PumpPressureSensor)poi.Facility);
                    pressureSensor.IsDeleting = true;
                    pressureSensor.AddToManager(this);
                    break;
            }*/
        }

        public void OnMovePOI(POI poi)
        {
            FormMain.Instance.OnMovePOI(poi);
        }

        public void OnAddPOI(POI poi)
        {
            if (poi.Facility != null)
            {
                if (poi.Facility.Type == IFacility.FacilityType.CCTV)
                {
                    CCTV cctv = (CCTV)poi.Facility;
                    cctv.AccessKey = "새 CCTV " + poi.ID.ToString();
                }
                else if (poi.Facility.Type == IFacility.FacilityType.FIRE_SENSOR)
                {
                    FireSensor sensor = (FireSensor)poi.Facility;
                    sensor.SensorName = "새 화재 Sensor " + poi.ID.ToString();
                }
            }

            FormMain.Instance.OnAddPOI(poi);
        }

        public void OnFinishEarthquake()
        {
            System.Diagnostics.Trace.WriteLine("OnFinishEarthquake");
        }

        public void OnPostPanelMouseDown()
        {
        }

        public void RemoveCCTVPOI(int nLayerID, int nID)
        {
            m_ContentForm.Layers.GetLayer(nLayerID).Remove(nID);
        }

        public void RemoveCCTVPOI(int nID)
        {
            m_ContentForm.Layers.GetLayer(ID.ID_LAYER_CCTV).Remove(nID);
            m_ContentForm.Layers.GetLayer(ID.ID_LAYER_CCTVLOW).Remove(nID);
            m_ContentForm.Layers.GetLayer(ID.ID_LAYER_CCTV_DISCONNECTED).Remove(nID);
        }

        public void RequestOutdoor()
        {
            if (m_ContentForm != null)
                m_ContentForm.LayoutOutside();
        }

        public void OnMessage(string strMessageType, string strMessage)
        {
            switch (strMessageType)
            {
                case "OnClick":
                    ViewOnClick(strMessage);
                    break;
            }
        }

        private void ViewOnClick(string strBtnImageName)
        {
            if (strBtnImageName.Contains("btnPoiVisible_"))
                m_frmMain.ShowPOIVisibleForm();
            else if (strBtnImageName.Contains("btnSlideLeft_"))
            {
                m_frmMain.SetLeftPanelSlide();
            }
            else if (strBtnImageName.Contains("btnSlideRight_"))
            {
                m_frmMain.SetRightPanelSlide();
            }
            else if (strBtnImageName.Contains("btnManualReport_"))
            {
                m_frmMain.ShowManualReport();
            }
        }
        #endregion

        #region IChangedDataManager
        private ArrayList m_arrChangedData = new ArrayList();

        public ArrayList GetDataList()
        {
            return m_arrChangedData;
        }

        public void RemoveData(ChangedData data)
        {
            m_arrChangedData.Remove(data);

            /*ImageButton btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
            btn.Enabled = m_arrChangedData.Count > 0;
            FormMain.Instance.CheckButton(btn, btn.Enabled);*/
        }

        public void SomethingChanged(ChangedData data)
        {
            if (data != null)
                m_arrChangedData.Add(data);

            /*ImageButton btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
            btn.Enabled = m_arrChangedData.Count > 0;
            FormMain.Instance.CheckButton(btn, btn.Enabled);*/
        }

        public void ChangeWall()
        {
            FormMain.Instance.ChangeWall();
        }

        public void ChangeSpaceText()
        {
            FormMain.Instance.ChangeSpaceText();
        }

        public void GetWallInfo(float x, float y, float scale, float rotate)
        {
            FormMain.Instance.GetWallInfo(x, y, scale, rotate);
        }
        #endregion
    }
}
