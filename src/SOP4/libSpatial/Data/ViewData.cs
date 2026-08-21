using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;


namespace UnE.View.Content
{    

	public enum ContentOwnerTab { FILE_TAB = 0, M3D_TAB, ADMIN_TAB, REPORT_TAB, M2D_TAB, CCTV_TAB, BOTH};
    public enum ContentOwnerTabRightDockingMode { NONE = 0, SHOW_PSM, SHOW_LOCATION, SHOW_CCTV, SHOW_DISASTER };

    public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };

    public enum MouseEvent { MOUSE_DOWN = 0, MOUSE_UP, MOUSE_MOVE };

    public enum ViewType { OUTSIDE, INSIDE, BOTHSIDE };

    public interface IFormContent
    {
        void Init3DView();

        IBaseViewOwner BaseViewOwner
        {
            get;
        }

        Zone ManualClickZone
        {
            get;
            set;
        }

        bool IsDisposed
        {
            get;
        }

        bool Visible
        {
            get;
            set;
        }

        void Show();
        void RedrawWindow();


        bool EditMode
        {
            get;
            set;
        }

        int NumLayout
        {
            get;
            set;
        }

        void SaveCurrentTabLayout();
        void LoadTabLayout(int tabNumber);

        void LayoutBothside();
        void LayoutOutside();
        void LayoutInside();

        void AddMainToolStrip(System.Windows.Forms.ToolStrip strip, ViewType vtype);


        MouseWorkMode CurrentMouseWorkMode
        {
            get;
            set;
        }

        ILayerManager Layers
        {
            get;
        }

        bool ShowLayer(int nLayerID, bool bShow);


        UnE.Sensor.ISensorTooltipOwner OutdoorView
        {
            get;
        }
        UnE.Sensor.ISensorTooltipOwner IndoorView
        {
            get;
        }

        void PushViewState(bool bSavedCurrentTab = false);
        void ClearTabState();
        void ClearViewState();
        void RestoreViewState();

        void ClearPOISelection();

        System.Windows.Forms.ToolStripMenuItem GetMenu(string szName);
        void SetMenu(string szName, System.Windows.Forms.ToolStripMenuItem menu);

        void IndoorMenuClick(object sender, EventArgs e);
        void ManualReportClick(object sender, EventArgs e);
        void ManualCCTVClick(object sender, EventArgs e);

        void HomeView(string szName);

        void SelectPOILoadZone(UnE.Sensor.POI poi, bool isIndoor);
        void HideAllPOIPopup();
        void ShowZoneVolume(int zoneID, int nEquipZoneID, bool bOutDoorWnd, bool bShow);
        void ShowZoneVolume(int zoneID, bool bOutDoorWnd, bool bShow);

        void HidePoll(int nPollID);
        void ShowEmPoll(int nPollID);

        void HideZoneVolume();
        void ZoomTarget(float x, float y, float z, bool isIndoor);
        void ZoomBuilding(string szBuidingID);

        void ZoomOut();
        void ZoomIn();

        void TopView();

        void ShowEvacCircle(int nLevel);
        void SetEvacDistance(int nSensorID);
        void HideEvacCircle();
        void SetEvacCenter(EquipmentZone zone);


        void LoadPOIs();

        void SetFilePath(string strCMOFolderPath, string strOutsideFilePath, string strInsideFilePath, Dictionary<string, string> dicInsideCMO);

        void SetCurrentBuilding(Building building, Zone showFloor);


        void Invalidate3DView(bool bEraseBk);

        string SaveToTempImage();
        void SaveToImage();

        Building GetCurrentBuilding(ref float nFloorIdx);

        void View1Click(object sender, EventArgs e);

        void View2Click(object sender, EventArgs e);

        void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode);
        bool EarthquakeEventIsFinished();

        void SelectBuilding(string strBuildingID);

        void ShowBuildingCollapse(string szBuildingID, string szDisplayName);
        void CloseBuilingCollapse(string szBuildingID);
        void ShowPollutionView(int windDirection, int windSpeed);
        void HidePollutioinView();
    }

    public interface IFormContentOwner
    {
        DBUtility.WebDBManager DBManager
        {
            get;
        }

        System.Windows.Forms.Form InvokeForm
        {
            get;
        }

        string ResourcePath
        {
            get;
        }

        SDMS.IChangedDataManager IChangedDataManager
        {
            get;
        }
        UnE.Sensor.POI SelectedPOI
        {
            get;
            set;
        }

        IFormContent ContentForm
        {
            get;
        }

        void LoadPOI(UnE.Sensor.ISensorTooltipOwner view, bool bIndoor);

        void EnableFireReportBtn(bool bEnable);
        void EnableFireReportBtn(bool bEnable, int nType);
       
        void OnPostPickPOI(UnE.Sensor.POI poi);
        void ChangeZoneComboBox(Zone zone);

        ArrayList GetFireEquipments(Zone zone);


        bool IsChangedEquipZoneCCTV
		{
			get;
			set;
		}


        void OnReadyDataLoad();
        
        bool ExtractOutside
        {
            get;
            set;
        }

        bool ExtractInside
        {
            get;
            set;
        }
        
        void SetBuilingCollapseDetect( string strPosition, bool isRealMode);
        void SetEarthquakeDetect(int nIntensity, float fMagnitude, string strPosition, bool isRealMode);


        int ChangeTab(ContentOwnerTab tab);
        ContentOwnerTab CurrentTab
        {
            get;
        }

        ContentOwnerTab PreviousTab
        {
            get;
        }


        void SelectIndoorZone(Zone zone);


        void ShowEquipZoneCCTVs(int nEquipZoneID);        
        void ShowCCTVForm(bool bShow);

        void Check3DViewMode(int nID);

        void OnClick3D();
        void OnClick2D();
        void OnClickBothView(bool isChecked);

        UnE.PSM.PSMSensor GetPSMSensor(int nID);
        UnE.PSM.PSMMaterial GetPSMMaterial(int nMaterialType);

    }

    public interface IBaseViewOwner
    {
        UnE.Sensor.POI SelectedPOI
        {
            get;
            set;
        }

        void OnDeletePOI(UnE.Sensor.POI poi);

        void AddCCTVEditData(UnE.Sensor.POI poi, Zone zone);
        void AddPressureSensorEditData(UnE.Sensor.POI poi, Zone zone);

        void OnPostPanelMouseDown();

        void HideAllPopup();
         
        System.Windows.Forms.ToolStripMenuItem MenuIndoor
        {
            get;
        }

        System.Windows.Forms.ToolStripMenuItem MenuManualReport
        {
            get;
        }

        System.Windows.Forms.ToolStripMenuItem MenuManualCCTV
        {
            get;
        }

        void MenuIndoorClicked(object sender, EventArgs e);
        void MenualReportClicked(object sender, EventArgs e);
        void ManualCCTVClicked(object sender, EventArgs e);

        void RemoveCCTVPOI(int nID);
        void RemoveCCTVPOI(int nLayerID, int nID);

        Building GetBuilding(string szBuildingName);
        Zone GetOutsideZone(float x, float y);

        Zone GetZone(string szBuildingID, int nFloor);
        EquipmentZone CheckEquipmentZone(Zone zone, float x, float y);

        void EditFireSensor(UnE.Sensor.FireSensor sensor);
        void EditSpringCooler(UnE.Sensor.SpringCooler sensor);
        void EditPumpPressureSensor(UnE.Sensor.PumpPressureSensor sensor);

        void EditCCTV(UnE.Sensor.CCTV cctv);
        void EditCCTV(UnE.Sensor.CCTV cctv, string szDescription);

        ArrayList GetFireEquipments(Zone currentIndoorZone);

        void OnChangeIndoorZone(Zone currentZone);

        void RequestOutdoor();

        void OnFinishEarthquake();
        void OnCollapseBuilding(string buildingID, bool isReal = false);
        void OnBeepFinish();     
    }

    public interface IBaseView
    {
        void Refresh();

        void SaveHomeView(string szName);
        void LoadHomeView(string szName);

        System.Drawing.Point GetPosition2D(int nPOIID, float x, float y, float z);

        void AddMainToolStrip(System.Windows.Forms.ToolStrip strip);

        ILayerManager LayerManager
        {
            get;
            set;
        }


        void ShowNames(int nID, bool bShow);
        void SetTextPOILOD(int nID, bool bToogle, float dist);
        void ShowIconPOI(int nID, bool bShow);

        void UpdateWindow();

        void RemovePOI(float ox, float p, float oz);

        void RemovePOI(int nID);

        int AddPOI(string szIconPath, float p1, float p2, float p3);

        void SetCheckPoistion(bool mCheckPosition);

        int AddPOI(string szIconPath);
    }


    #region LayerManager
    public interface ILayerManager
    {
        void HideLayer(int nLayerID);
        void ShowLayer(int nLayerID);
        ILayer GetLayer(int nLayerID);

        void RemoveLayerChild(int nObjID);
    }


    public interface ILayer
    {
        ArrayList Objects
        {
            get;
        }
        void Add(int nID);
        void Remove(int nID);
        void SetVisible(bool bVisible);
    }
    #endregion

}

