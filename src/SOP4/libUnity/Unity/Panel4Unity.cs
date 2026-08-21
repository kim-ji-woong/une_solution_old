using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Security.AccessControl;
using UnE.Spatial;
using UnE.Sensor;
using UnE.View.Content;
using UnE.Win32;

namespace UnE.Util.Unity
{
    public partial class Panel4Unity : ToolStripContainer, ISensorTooltipOwner, IBaseView
    {

        private Dictionary<string, int> mSharedCmdMap = new Dictionary<string, int>();
        private string[] szKeyName = 
        {
	        "SetMainModel", 
	        "CameraTranslate",
	        "CameraPosition",
	        "CameraAngles",
	        "CameraDirection",
	        "SetCameraPosition",
	        "SetCameraAngles",
	        "SetCameraDirection",
	        "SetZoomPosition",
	        "SetZoomObject",
	        "SetZoomObjectDistance",
	        "SetZoomObjectAngle",
	        "CameraView",
	        "SelectObject",
	        "SetEditMode",
	        "SetMode",
	        "Get3DPosition",
	        "UpdateAliasNames",
	        "ClearAllSelect",
	        "Get2DPosition",
	        "ModelZoom",
	        "SaveScreenShot",
	        "GetLastID",
	        "GetLastIconID",
	        "AddAliasName",
	        "SetTextColor",
	        "SetAliasTextColor",
	        "SetTextDistanceRatio",
	        "SetIconDistanceRatio",
	        "AddTextPOI",
	        "AddReverseLODTextPOI",
	        "AddIconPOI",
	        "ShowTextPOI",
	        "ShowIconPOI",
	        "ShowIconLayer",
	        "SelectIconPOI",
	        "RemoveIconPOI",
	        "ShowOutZoneVolume",
	        "HideOutZonevolume",
	        "HideAllOutZoneVolume",
	        "ShowEquipZoneVolume",
	        "HideEquipZonevolume",
	        "HideAllEquipZoneVolume",
            "ShowEvacCircle", 
            "SetInitEvacDistance", 
            "SetSecondEvacDistance",
            "SetEvacCircleCenter",
            "ClearSelectIconPOI",
            "HideEmpoll",
            "ShowEmpoll",
            "HideAllEmpoll",
            "SetEarthquake",
            "ShowPollution",
            "HidePollution"   
        };

        private Pipelib.PassivePipeServer m_PipeServer;
        private IntPtr m_hWndUnity = IntPtr.Zero;
        private Process m_ProcessUnity = null;


        private ILayerManager mLayerManager = null;
        public ILayerManager LayerManager
        {
            get { return mLayerManager; }
            set { mLayerManager = value; }
        }

        public void ShowNames(int nID, bool bVisible)
        {
        }

        public void SetTextPOILOD(int id, bool toggle, float fDist)
        {
        }

        public void ShowIconPOI(int nID, bool bVisible)
        {
        }
        
        

        private System.Windows.Forms.ContextMenuStrip m_PopupMenu = null;
        public System.Windows.Forms.ContextMenuStrip PopupMenu
        {
            get { return m_PopupMenu; }
            set { m_PopupMenu = value; }
        }
                
        private string m_szPipeName = "TestPipe";
        public string NamedPipeName
        {
            get { return m_szPipeName; }
            set { m_szPipeName = value; }
        }


        private string m_szUnityFileName = "EnergyOutside";
        private string m_szUnityExePath = @"C:\UNE\bin\common12\EnergyOutside.exe";
        public string UnityExePath
        {
            get { return m_szUnityExePath; }
            set
            {
                if (value == null || value == "")
                    return;
                string szFileName = Path.GetFileName(value);
                string ext = Path.GetExtension(value);
                m_szUnityFileName = szFileName.Replace(ext, "").Replace(".", "");
                m_szUnityExePath = value;
            }
        }


        private string szUnityName = "EnergyOutside";
        public string UnityWndName
        {
            get { return szUnityName; }
            set { szUnityName = value; }
        }
        
        private Action<int, float, float, float> m_IconPOIAddCallback = null;
        private Action<int, float, float, float> m_TextPOIAddCallback = null;


        private float m_fLastPickX = 0.0f;
        private float m_fLastPickY = 0.0f;
        private float m_fLastPickZ = 0.0f;

        private bool m_bAddTextMode = false;
        private bool m_bAddIconMode = false;

        private string m_szOverObjName = "";
        public string MouseOverObject
        {
            get { return m_szOverObjName; }
            set { m_szOverObjName = value; }
        }

        private string m_szPopupObjName = "";

        public string PopupObjName
        {
            get { return m_szPopupObjName; }
            set { m_szPopupObjName = value; }
        }


        private string m_szIconName = "";
        public string IconName
        {
            get { return m_szIconName; }
            set { m_szIconName = value; }
        }

        private string m_szPoiText = "";
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton3;
        private ToolStripButton toolStripButton4;
        private ToolStripButton toolStripButton5;
        private ToolStripButton toolStripButton6;
        private ToolStripButton toolStripButton7;
        private ToolStripButton toolStripButton8;
        private ToolStripButton toolStripButton9;
        private ToolStripButton toolStripButton10;
        private ToolStripButton toolStripButton11;
        private ToolStripButton toolStripButton12;
        private ToolStripButton toolStripButton13;
    
        public string PoiText
        {
            get { return m_szPoiText; }
            set { m_szPoiText = value; }
        }

        internal IContainer components;  /* Don't do owner Z ordering */
        internal int MakeLParam(int LoWord, int HiWord)
        {
            int i = (HiWord << 16) | (LoWord & 0xffff);
            return i;
        }

        private System.Windows.Forms.Timer m_TimerSize;

        private IBaseViewOwner m_Owner = null;

        // POI Tooltip
        private int m_nShowTooltipX = 0;
        private int m_nShowTooltipY = 0;
        private Form m_formTooltip = null;
        private System.Windows.Forms.Timer m_TooltipTimer = new System.Windows.Forms.Timer();

        // Mouse Over POI ID
        private int m_nEnterPOI = -1;

        // Unity Loading Complete Callback
        private Action m_CallbackReady = null;

        private object m_syncCmdLock = new object();


        public void SetEvacCircleCenter(float x, float y, float z)
        {
            INFO_POI info = new INFO_POI();
            info.x = x;
            info.y = y;
            info.z = z;
            info.bSet = 0;

            string szCMD = string.Format("CMD:SetEvacCircleCenter({0},{1},{2})", x, y, z);
            RunSyncUnityCmd("SetEvacCircleCenter", info, szCMD, 1000);
        }

        public void SetEvacClircleDistance(int distance1, int distance2)
        {
            INFO_POI info = new INFO_POI();
            info.nx = distance1;
            info.ny = distance2;
            info.bSet = 0;

            string szCMD = string.Format("CMD:SetInitEvacDistance({0})", distance1);
            RunSyncUnityCmd("SetInitEvacDistance", info, szCMD, 1000);

            szCMD = string.Format("CMD:SetSecondEvacDistance({0})", distance2);
            RunSyncUnityCmd("SetSecondEvacDistance", info, szCMD, 1000);
        }

        public void ShowEvacCircle(int nLevel)
        {
            INFO_POI info = new INFO_POI();
            info.bSelect = nLevel;
            info.bSet = 0;

            string szCMD = string.Format("CMD:ShowEvacCircle({0})", nLevel);
            RunSyncUnityCmd("ShowEvacCircle", info, szCMD, 1000);            
        }


        public void HideAllEmPoll()
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 0;

            string szCMD = "CMD:HideAllEmPoll()";
            RunSyncUnityCmd("HideAllEmPoll", info, szCMD, 1000);
        }

        public void HideEmPoll(int nPollID)
        {
            INFO_POI info = new INFO_POI();
            info.bSelect = nPollID;
            info.bSet = 0;

            string szCMD = string.Format("CMD:HideEmpoll({0})", nPollID);
            RunSyncUnityCmd("HideEmpoll", info, szCMD, 1000);        
        }
        public void ShowEmPoll(int nPollID)
        {
            INFO_POI info = new INFO_POI();
            info.bSelect = nPollID;
            info.bSet = 0;

            string szCMD = string.Format("CMD:ShowEmpoll({0})", nPollID);
            RunSyncUnityCmd("ShowEmpoll", info, szCMD, 1000);        
        }

        /*
         * 2018.4.2 Earthquake 추가. 이후 추가 변경시 unity에서 처리.
         * amount   : 1,2,3 지진 강도 (점점 세지는)
         * seconds : 초
         */
        public int ShowEarthquake(int amount, int seconds)
        {
            INFO_POI info = new INFO_POI();            
            info.bSet = 0;

            string szCMD = string.Format("CMD:SetEarthquake({0},{1})", amount, seconds);
            INFO_POI ret = RunSyncUnityCmd("SetEarthquake", info, szCMD, seconds * 1000);           
            return 1; 
        }

        /*
         * 2018.4.2 광교 오염물질 대기질 모델링 결과 보여주는 부분 추가. 
         * 주빅스 센서 데이터 읽은 후 기상 데이터에 따라 미리 시뮬레이션 된 결과를 보여주려 함.
         */

        public void ShowPollution(int direction, int windStrength)
        {
            /*
            direction 0 : N, 1 : NE, E : 2, 3 : SE, 4 : S, 5 : SW, 6 : W, 7 : NW
            windStrength 0 : 약함(M:바람강함),  2: 강함(X:바람약함)        
            */

            // string timestr = time.ToString("D4");
            
            INFO_POI info = new INFO_POI();
            info.bSet = 0;

            string szCMD = string.Format("CMD:ShowPollution({0},{1})", direction, windStrength);
            /**** 광교 오염물질 시뮬레이션을 하기 위해서는 아래의 코드를 실행시켜야 한다. 
             * 평상시 시뮬레이션이 필요하지 않으므로 동작을 막음. */
            // RunSyncUnityCmd("ShowPollution", info, szCMD, 3000);    
            
        }

        public void HidePollution()
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 0;

            string szCMD = "CMD:HidePollution()";
            RunSyncUnityCmd("HidePollution", info, szCMD, 1000);
        }

        public void MakeCommandMap()
        {
            for (int i = 0; i < szKeyName.Length; i++)
            {
                string szKey = szKeyName[i];
                mSharedCmdMap.Add(szKey, i + 10);
            }
        }

        private int GetMemIdx(string szCmd)
        {
            if (mSharedCmdMap.ContainsKey(szCmd))
            {
                return mSharedCmdMap[szCmd];
            }
            return -1;
        }
        
        private INFO_POI RunSyncUnityCmd(string szKey, INFO_POI info, string szCmd, int nTimeOut = 2000, int nSet = 1)
        {

            info.bSet = 0;
            info.nID = -1;
            INFO_POI result = new INFO_POI();

            int nIdx = GetMemIdx(szKey);
            if (nIdx == -1)
                return result; 
            
            lock(m_syncCmdLock)
            {                
                if (mSharedBuffer != null)
                {
                    mSharedBuffer.Write<INFO_POI>(ref info, nIdx);

                    m_PipeServer.Send(szCmd);

                    if (m_isIndoor == true)
                    {
                        Update3D();
                    }
                    int nCount = 0;
                    while (nCount < nTimeOut)
                    {
                        mSharedBuffer.Read<INFO_POI>(out result, nIdx);
                        if (result.bSet == nSet)
                            break;
                        Thread.Sleep(1);
                        nCount++;
                    }
                }
              
                return result;
            }            
        }

        internal void RemoveIconPOI(int nID, string szType)
        {
            try
            {
                if (m_PipeServer != null)
                {
                    string szCmd = string.Format("CMD:RemoveIconPOI({0}, '{1}')", nID, szType);
                    m_PipeServer.Send(szCmd);
                }  
            }
            catch(Exception)
            {
            }           
        }
        
        public Panel4Unity(IBaseViewOwner iOwner, int nSiteID)
        {
            m_Owner = iOwner;

            InitializeComponent();

            m_szHomeKey += "\\" + nSiteID.ToString();
            this.SizeChanged += Panel4Unity_SizeChanged;

            m_TimerSize = new System.Windows.Forms.Timer();
            m_TimerSize.Interval = 100;
            m_TimerSize.Tick += OnTimerSizeChanged;


            this.VisibleChanged += new System.EventHandler(this.Panel4Unity_VisibleChanged);
            //mLayerManager = new LayerManager(this);

            MakeCommandMap();
            
            this.GotFocus += Panel4Unity_GotFocus;

            this.ContentPanel.BackColor = Color.FromArgb(227, 226, 226);
            TopToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            LeftToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            RightToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            BottomToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
        }

        private string m_szPosSubKeyName = "MainToolStripPos";
        private string m_szToolStripName = "ToolboxStrip";
        public void AddMainToolStrip(ToolStrip strip)
        {
            if (strip == null)
                strip = toolStrip1;
            // read toolstrip position
            int nPos = ReadToolStripConfig();

            // Set StripName for using Key
            strip.Name = m_szToolStripName;

            // Add StripMenu
            SetToolStripMenu(strip, nPos);
        }

        public void RemoveMainToolStrip(ToolStrip strip)
        {
            this.RightToolStripPanel.Controls.Remove(strip);
            this.LeftToolStripPanel.Controls.Remove(strip);
            this.BottomToolStripPanel.Controls.Remove(strip);
            this.TopToolStripPanel.Controls.Remove(strip);
        }

        private void SetToolStripMenu(ToolStrip strip, int nPos)
        {            
            if (nPos == 1)
                this.RightToolStripPanel.Controls.Add(strip);
            else if (nPos == 2)
                this.LeftToolStripPanel.Controls.Add(strip);
            else if (nPos == 3)
                this.BottomToolStripPanel.Controls.Add(strip);
            else
                this.TopToolStripPanel.Controls.Add(strip);
        }

        private int ReadToolStripConfig()
        {
            int nResult = 0;
            try
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(m_szToolKey);
                if (rkey == null)
                {
                    return 0;
                }
                else
                {
                    nResult = (int)rkey.GetValue(m_szPosSubKeyName, 0);                     
                }
                if (rkey != null)
                    rkey.Close();
            }
            catch (System.Exception)
            {
            }
            return nResult;
        }

        private int GetToolStringPos()
        {
            if(TopToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 0;
            }
            else if (RightToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 1;
            }
            else if (LeftToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 2;
            }
            else if (BottomToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 3;
            }
            return 0;
        }

        private void WriteToolStripConfig(int nPos)
        {
            try
            {
                string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Deny));

                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(m_szToolKey, true);
                if (rkey == null)
                {
                    try
                    {
                        rkey = Registry.CurrentUser.CreateSubKey(m_szToolKey, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (rkey != null)
                {
                    rkey.SetValue(m_szPosSubKeyName, nPos);
                    rkey.Close();
                }
            }
            catch (System.Exception)
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            // write toolstrip position
            int nPos = GetToolStringPos();
            WriteToolStripConfig(nPos);

            if (m_PipeServer != null)
            {
                StopUnity();
                m_PipeServer.Dispose();
                m_PipeServer = null;
            }

            if(mSharedBuffer != null)
            {
                mSharedBuffer.Dispose();
            }
            
            m_vDragOrigin.Dispose();

            base.Dispose(disposing);
        }

        // 외부에서 Tooltip용 Control을 Add할때 사용 (ISensorTooltipOwner)
        public void AddToolTipControl(System.Windows.Forms.Control c)
        {
            if(this.Parent.Parent != null)
            {
                Parent.Parent.Controls.Add(c);
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Handle != null)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    base.OnSizeChanged(e);
                });
            }
        }

        public void SaveScreen(string szPath)
        {
          
            INFO_POI poi = new INFO_POI();
            string szUnixPath = szPath.Replace("\\", "/");
            string szCMD = string.Format("CMD:SaveScreenShot('{0}')", szUnixPath);

            if( m_isIndoor == true)
            {
                INFO_POI ret = RunSyncUnityCmd("SaveScreenShot", poi, szCMD, 5000, 10);
            }
            else
            {
                INFO_POI ret = RunSyncUnityCmd("SaveScreenShot", poi, szCMD, 4000);
            }
      
        }

        private System.Windows.Forms.Timer mUpdateTimer = null;
        public void Update3D()
        {
            try
            {
                NativeMethods.ShowWindow(m_hWndUnity, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(m_hWndUnity);
                if (m_isIndoor == true)
                {
                    if (mUpdateTimer == null)
                    {
                        mUpdateTimer = new System.Windows.Forms.Timer();
                        mUpdateTimer.Tick += UpdateTimer_Tick;
                    }

                    if (mUpdateTimer.Enabled == true)
                    {
                        mUpdateTimer.Stop();
                    }
                    mUpdateTimer.Interval = 4000;
                    mUpdateTimer.Enabled = true;
                    mUpdateTimer.Start();
                }  
            }
            catch(Exception)
            {
            }
        }

        void UpdateTimer_Tick(object sender, EventArgs e)
        {
            mUpdateTimer.Stop();
            mUpdateTimer.Enabled = false;

            NativeMethods.ShowWindow(m_hWndUnity, NativeMethods.SW_RESTORE);
            NativeMethods.SetForegroundWindow(m_hWndUnity);
        }

        public void SetMouseMode(int nMode)
        {
            if(m_PipeServer != null)
            {
                string szCmd = string.Format("CMD:SetMode({0}, {1})",nMode, bool.TrueString);
                m_PipeServer.Send(szCmd);
            }            
        }       


        // Indoor Only
        private void OpenModel(string szName)
        {
            if(m_PipeServer != null)
            {
                string szCmd = string.Format("CMD:OpenModel('{0}')", szName);
                m_PipeServer.Send(szCmd);
            }
        }

        private string m_szReservePath = "";
        private Zone m_zoneReserve = null;
        private bool m_bReserveLoad = false;

        public void OpenIndoor(string strPath, Zone zone)
        {
            // Unity에서 
            if (m_bFirstReady == true)
            {
                m_bReserveLoad = true;
                m_zoneReserve = zone;
                m_szReservePath = strPath;
                return;
            }

            if (!m_isIndoor)
                return;

            HideAllEquipmentZone();

            ArrayList arrPrevPOIs = null;
            ArrayList arrNextPOIs = null;

            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
                    arrPrevPOIs = arrPOIs;

                    foreach (POI poi in arrPOIs)
                    {
                        // 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
                        if (poi.Facility != null)
                        {
                            int nLayerID = poi.Facility.GetLayerID();
                            mLayerManager.GetLayer(nLayerID).Remove(poi.ID);
                        }


                        RemoveIconPOI(poi.ID, poi.Facility.IconPath);

                        if (poi.Popup != null)
                        {
                            poi.Popup.Close();
                            poi.Popup = null;
                        }
                    }
                }
            }

            ClearFireEquipments();

           // OpenModel(strPath);
            m_meshOpened = true;

            m_currentIndoorZone = zone;

            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
                    arrNextPOIs = arrPOIs;

                    foreach (POI poi in arrPOIs)
                    {
                        if (poi.Facility == null)
                            continue;

                        string strIconPath = poi.Facility.IconPath;

                        if (poi.Facility.Connected == false)
                        {
                            strIconPath = poi.Facility.DisconnectIconPath;
                        }
                        
                        int nID = AddIconPOI(poi, strIconPath, poi.X, poi.Y, poi.Z);

                        poi.ID = nID;

                        string szKey = string.Format("{0}_{1}", strIconPath, poi.Facility.ID);
                        m_dicPOIs[szKey] = poi;

                        if (poi.Popup == null && poi.Facility != null)
                        {
                            if (m_Factory == null)
                            {
                                m_Factory = PopupFactoryHelper.GetFactory();
                            }
                            if (poi.Popup == null)
                                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);
                        }

                        int nLayerID = poi.Facility.GetLayerID();
                        mLayerManager.GetLayer(nLayerID).Add(poi.ID);
                    }
                }

                //LoadFireEquipments();

                if (m_Owner != null)
                    m_Owner.OnChangeIndoorZone(m_currentIndoorZone);
            }

            ProcessCCTVLOD();
        }
       
        private void Panel4Unity_SizeChanged(object sender, EventArgs e)
        {
            m_TimerSize.Stop();
            m_TimerSize.Interval = 100;
            m_TimerSize.Start();
        }

        void OnTimerSizeChanged(object sender, EventArgs e)
        {
            Point pt = Location;
            if( this.Parent != null && this.Parent.IsHandleCreated == true && this.Parent.Visible == true)
            {
                NativeMethods.ShowWindow(m_hWndUnity, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(m_hWndUnity);

                NativeMethods.MoveWindow(m_hWndUnity, -11, -31, this.Parent.Width + 22, this.Height + 40, false);                
            }
            
            m_TimerSize.Stop();
        }
     
        internal void OnDataCmd(string cmd)
        {
            this.BeginInvoke(new Action(() =>
            {
                System.Diagnostics.Trace.WriteLine(cmd);
                UnE.Util.Unity.CommandProcessor cm = UnE.Util.Unity.CommandProcessor.Instance;
                cm.ProcessCommand(cmd, this);
            }
            ));  
        }
        /*
         * 지진 종료 콜백 
         */
        public void EarthquakeFinished()
        {
            if (m_Owner != null)
                m_Owner.OnFinishEarthquake();
        }

        /*
         * 지진 종료 후 건물 붕괴 콜백
         */
        public void CollapseBuilding(string buildingID)
        {
            if (m_Owner != null)
            {                
                m_Owner.OnCollapseBuilding(buildingID);
                //SelectObject(buildingID);
            }               
        }

        public void EarthquakeBeepFinish()
        {
            if (m_Owner != null)
            {
                m_Owner.OnBeepFinish();
                //SelectObject(buildingID);
            }              
        }
        private bool m_bUseIndoor = false;

        public bool UseIndoor
        {
            get { return m_bUseIndoor; }
            set { m_bUseIndoor = value; }
        }
        public bool BeginUnity(Action callbackReady)
        {
            KillProcess(m_szUnityFileName);

            string szFileName = "UnitySamOutsidePoiInfo";
            if (this.m_isIndoor == true)
            {
                if (m_bUseIndoor == false)
                    return true;
                szFileName = "UnitySamInsidePoiInfo";
            }

            try
            {                
                mSharedBuffer = new SharedMemory.BufferReadWrite(name: szFileName, bufferSize: 1024);

            }
            catch (Exception)
            {
                MessageBox.Show("Shared Memroy Init Fail");
                Application.Exit();
            }

            BeginServer();
            SetUnity(m_szUnityFileName);

            m_CallbackReady = callbackReady;
            return true;
        }

        private void BeginServer()
        {           

            m_PipeServer = new Pipelib.PassivePipeServer(true, m_szPipeName);
            m_PipeServer.OnReciveMessage += OnDataCmd;
            m_PipeServer.BeginPipe();
        }

        private void SetUnity(string szName)
        {
            m_szToolKey = @"SDMS\Unity\" + szName + @"\Toolstrip";

            m_hWndUnity = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, szUnityName);
            if (m_hWndUnity == IntPtr.Zero)
            {
                m_ProcessUnity = StartUnityPocess(m_szUnityExePath, m_szUnityExePath, "");
            }

            while (m_hWndUnity == IntPtr.Zero)
            {
                m_hWndUnity = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, szUnityName);
            }

            NativeMethods.SetParent(m_hWndUnity, this.ContentPanel.Handle);
            NativeMethods.ShowWindow(m_hWndUnity, NativeMethods.SW_SHOW);

            int style = NativeMethods.GetWindowLong(m_hWndUnity, NativeMethods.GWL_STYLE);
            int exStyle = NativeMethods.GetWindowLong(m_hWndUnity, NativeMethods.GWL_EXSTYLE);
            style &= ~(NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
            exStyle &= ~NativeMethods.WS_EX_CLIENTEDGE;
            exStyle |= (NativeMethods.WS_EX_MDICHILD | NativeMethods.WS_CHILD);
            NativeMethods.SetWindowLong(m_hWndUnity, NativeMethods.GWL_STYLE, (int)style);
            NativeMethods.SetWindowLong(m_hWndUnity, NativeMethods.GWL_EXSTYLE, (int)exStyle);

            NativeMethods.SetWindowPos(m_hWndUnity, IntPtr.Zero, 0, 0, 0, 0,
                NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOOWNERZORDER);
            Point pt = Location;

            // 화면보다 작아지지 않도록 여분의 크기와 위치를 준다.
            NativeMethods.MoveWindow(m_hWndUnity, -11, -31, this.Width + 22, this.Height + 40, false);
        }

        public void StopUnity()
        {
            if (m_PipeServer != null)
                m_PipeServer.StopPipe();
           
            KillProcess(m_szUnityFileName);
        }


        private bool m_bFirstReady = true;
        private SharedMemory.BufferReadWrite mSharedBuffer = null; 
        internal void OnReadyToSend()
        {           
            if(m_bFirstReady == true)
            {
                m_bFirstReady = false;

                

                if (m_CallbackReady != null)
                    m_CallbackReady.Invoke();

                m_TooltipTimer.Tick += OnShowCCTVTooltip;

                SetMouseMode((int)m_currentMode);
                              

                // Indoor의 경우 실내 모델 추가
                //if( m_bReserveLoad == true)
                //{
                //    OpenIndoor(m_szReservePath, m_zoneReserve);
               // }

                Update3D();
                
            }           
        } 
        
        //private void DeletePOI(int nID, int x, int y)
        //{
        //    if (nID > 0)
        //        DeletePOI(nID);
        //}

        public void SelectPOI(int nPOIID, string szPOIType)
        {
            ClearPOISelection();

            string szKey = string.Format("{0}_{1}", szPOIType, nPOIID);

            m_arSelectedPoi.Add(szKey);           

            if (m_dicPOIs.ContainsKey(szKey))
            {
                POI poi = m_dicPOIs[szKey];
                m_Owner.SelectedPOI = poi;


            }

            try
            {
                INFO_POI info = new INFO_POI();
                info.bSelect = 0;
                info.bSet = 0;

                string szCMD = string.Format("CMD:SelectIconPOI({0},'{1}', True, True)", nPOIID, szPOIType);
                RunSyncUnityCmd("SelectIconPOI", info, szCMD, 0);
            }
            catch (Exception)
            {
            }
        }

        public void OnSelectPOI(string szPOIID, float x, float y, bool bSelect)
        {
            ClearPOISelection();

            m_arSelectedPoi.Add(szPOIID);

            if (m_dicPOIs.ContainsKey(szPOIID))
            {
                POI poi = m_dicPOIs[szPOIID];

                m_Owner.SelectedPOI = poi;
            }
        }

        private char[] mIconPoiSpliter = { '_' };
        public void OnPickPOI(string szPOIID, float x, float y)
        {
            if( m_currentMode == MouseWorkMode.DEL_FACILITY)
            {
                DeletePOI(szPOIID);
                return;
            }


            if( m_currentMode != MouseWorkMode.PICK)
            {
                return;
            }
            
            bool bSelected = false;
            POI selectedPOI = null;
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (bSelected == false)
                {
                    m_arSelectedPoi.Add(szPOIID);
                }
                else
                {
                    m_arSelectedPoi.Remove(szPOIID);
                }
            }
            else
            {
                // Control키가 눌러지지 않는 경우 모두 클리어
                ClearPOISelection();
                bSelected = false;
            }

            // 현재 뷰에 포함된 POI인지 확인- add by skkim 2014-03-03
            if (szPOIID != null && m_dicPOIs.ContainsKey(szPOIID))
            {
                POI poi = m_dicPOIs[szPOIID];
                if (!bSelected)
                {

                    string[] szValues = szPOIID.Split(mIconPoiSpliter);

                    selectedPOI = poi;
                    m_Owner.SelectedPOI = poi;
                    bSelected = true;

                    try
                    {
                        INFO_POI info = new INFO_POI();
                        info.bSelect = 0;
                        info.bSet = 0;

                        string szCMD = string.Format("CMD:SelectIconPOI({0},'{1}', True, True)", szValues[1], szValues[0]);
                        RunSyncUnityCmd("SelectIconPOI", info, szCMD, 0);
                    }
                    catch (Exception)
                    {
                    }

                    if (poi.Popup != null)
                    {
                        Point pt = new Point((int)x, (this.Height - (int)y));
                        poi.Popup.Show(pt.X, pt.Y);
                    }
                }
            }

            if (!bSelected)
            {
                m_Owner.SelectedPOI = null;
                selectedPOI = null;
            }
            OnPostPick(selectedPOI);
        }

        private int m_nOnPoiX = 0;
        private int m_nOnPoiY = 0;
        private string m_szEnterType = "";
       
        public void OnOverPOI(int nID, string szType, float x, float y)
        {
            m_nOnPoiX = (int)x;
            m_nOnPoiY = (int)y;

            m_nEnterPOI = nID;
            m_szEnterType = szType;

            m_TooltipTimer.Interval = 500;
            m_TooltipTimer.Enabled = true;

        }

        public void OnLeavePOI(int nID, string szType)
        {
            if (m_nEnterPOI == -1 || nID != m_nEnterPOI)
                return;

            m_nEnterPOI = -1;
            m_szEnterType = "";

            m_TooltipTimer.Stop();
            m_TooltipTimer.Enabled = false;

            if (m_formTooltip != null)
                m_formTooltip.Visible = false;

            m_formTooltip = null;
        }
         
        private void OnMouseLeave(object sender, EventArgs e)
        {
          
        }

        private void ShowTooltip(MouseEventArgs e)
        {
            if (m_nShowTooltipX != e.X || m_nShowTooltipY != e.Y)
            {
               // m_TooltipTimer.Stop();
                m_TooltipTimer.Enabled = false;

                //m_bShowTooltip = false;
                if (m_formTooltip != null)
                {
                    m_formTooltip.Visible = false;
                    m_formTooltip = null;
                }
            }

            if (m_formTooltip == null)
            {
                m_nShowTooltipX = e.X;
                m_nShowTooltipY = e.Y;
                m_TooltipTimer.Enabled = true;
                m_TooltipTimer.Interval = 800;
                m_TooltipTimer.Start();
                //Debug.WriteLine("X={0}, Y={1}", m_nShowTooltipX, m_nShowTooltipY);
                //Debug.WriteLine(e.ToString());
            }
        }

        private string GetKey(int nID, string szType)
        {
            return string.Format("{0}_{1}", szType, nID);
        }

        private void OnShowCCTVTooltip(object sender, EventArgs e)
        {
            //m_bShowTooltip = false;

            m_TooltipTimer.Stop();
            m_TooltipTimer.Enabled = false;

            Point ptsc = System.Windows.Forms.Cursor.Position;
            Point ptIn = PointToClient(ptsc);

            Rectangle rect = this.ClientRectangle;
            if (!rect.Contains(ptIn))
                return;

            if (m_nEnterPOI != -1)
            {
                POI poi = null;

                string szKey = GetKey(m_nEnterPOI, m_szEnterType);

                if (m_dicPOIs.TryGetValue(szKey, out poi))
                {
                    if (poi.Zone == null)
                        return;
                    if (poi == null || poi.Facility == null)
                        return;

                    if (poi.Facility.Type != IFacility.FacilityType.CCTV)
                        return;

                    if (m_isIndoor != poi.IsIndoor)
                        return;

                    //Point pt = new Point(m_nOnPoiX + m_dX , (this.Height -m_nOnPoiY)+ m_dY);
                    Point pt = new Point(m_nOnPoiX , (this.Height - m_nOnPoiY));

                    CCTV cctv = (CCTV)poi.Facility;

                    if (m_formTooltip != null && m_formTooltip.IsDisposed == false && m_formTooltip.Visible == true)
                    {
                        m_formTooltip.Visible = false;
                        //this.Controls.Remove(m_formTooltip);
                    }

                    
                    m_formTooltip = new Form();

                    string szName = "CCTV : " + cctv.AccessKey;
                    string szZone = "위치 : " + poi.Zone.DisplayText;
                    Label lb = new Label();
                    lb.AutoSize = true;
                    lb.Text = szName;
                    lb.Location = new Point(10, 10);

                    int width1 = TextRenderer.MeasureText(lb.Text, new Font(lb.Font.FontFamily, lb.Font.Size, lb.Font.Style)).Width;

                    m_formTooltip.Controls.Add(lb);

                    Label lb2 = new Label();
                    lb2.AutoSize = true;
                    lb2.Text = szZone;
                    lb2.Location = new Point(10, 28);

                    int width2 = TextRenderer.MeasureText(lb2.Text, new Font(lb2.Font.FontFamily, lb2.Font.Size, lb2.Font.Style)).Width;

                    int maxWidth = width1 > width2 ? width1 : width2;
                    if (maxWidth < 130)
                    {
                        maxWidth = 130 + 20;
                    }
                    else
                    {
                        maxWidth = maxWidth + 20;
                    }
                    m_formTooltip.Controls.Add(lb2);

                    int nTooltipHeight = 50;
                    m_formTooltip.ShowInTaskbar = false;
                    m_formTooltip.Size = new Size(maxWidth, nTooltipHeight);
                    m_formTooltip.FormBorderStyle = FormBorderStyle.None;
                    m_formTooltip.StartPosition = FormStartPosition.Manual;
                    m_formTooltip.Opacity = 0.8f;
                    //Point pt = new Point(m_nOnPoiX , (this.Height - m_nOnPoiY));
                    m_formTooltip.Location = PointToScreen(new Point(pt.X - (maxWidth / 2), pt.Y - nTooltipHeight - 20 ));
                    //m_formTooltip.Location = (new Point(pt.X - (maxWidth / 2), pt.Y - nTooltipHeight - 20));

                    try
                    {
                        Form form = (Form)this.Parent; 
                    }
                    catch(Exception)
                    { }
                                             
                    m_formTooltip.BringToFront();
                    m_formTooltip.Show();
                   
                    return;
                }
            }
        }

        private void OnUnityProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine(e.Data);
        }

        private void OnUnityProcess_Exited(object sender, EventArgs e)
        {
            //int nExit = m_ProcessUnity.ExitCode;
        }

        private Process StartUnityPocess(string szFileName, string szWorkDir, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = szFileName;
            startInfo.WorkingDirectory = szWorkDir;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = args;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);

                process.Exited += OnUnityProcess_Exited;
                process.ErrorDataReceived += OnUnityProcess_ErrorDataReceived;
                process.OutputDataReceived += OnUnityProcess_ErrorDataReceived;
                return process;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return null;
        }     

        internal void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    try
                    {
                        process.CloseMainWindow();
                    }
                    catch (Exception ex)
                    {
                    }

                    try
                    {
                        process.Close();
                    }
                    catch (Exception ex)
                    {
                    }

                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private POI m_nLastPOI = null;
        private int m_nLastID =1;
        internal void OnReciveLastID(int nID)
        {
            if (this.Parent != null)
                this.Parent.Focus();
            
            m_nLastID = nID;

            if (m_bAddIconMode == true)
            {
                this.BeginInvoke(new Action(() =>
                {                    
                    if (m_IconPOIAddCallback != null)
                    {
                        m_IconPOIAddCallback.Invoke(m_nLastID, m_fLastPickX, m_fLastPickY, m_fLastPickZ);
                    }
                }));
            }
            if (m_bAddTextMode == true)
            {                
                this.BeginInvoke(new Action(() =>
                {
                        
                    if (m_TextPOIAddCallback != null)
                    {
                        m_TextPOIAddCallback.Invoke(m_nLastID, m_fLastPickX, m_fLastPickY, m_fLastPickZ);
                    }
                }));                
            }

            if( m_nLastPOI != null)
            {
                m_nLastPOI = null;
            }            
        }

        internal void OnPoistion2D(int nTag, float x, float y)
        {
            int i = 0;
            i++;
        }


        internal Vector3 GetPosition3D(int x, int y)
        {
            if (mSharedBuffer != null)
            {
                INFO_POI poiInfo = new INFO_POI();
                poiInfo.nID = 0;
                poiInfo.nx = (int)x;
                poiInfo.ny = (int)(this.Height - y);

                string szCmd = string.Format("CMD:Get3DPosition({0},{1})", x, y);
                INFO_POI ret = RunSyncUnityCmd("Get3DPosition", poiInfo, szCmd);

                float dx = ret.x;
                float dy = ret.y;
                float dz = ret.z;
                return new Vector3(dx, dy, dz);
            }
            return new Vector3(0,0,0);
        }

        public Point GetPosition2D(int id, float x, float y, float z)
        {
            if( mSharedBuffer != null)
            {
                INFO_POI poiInfo = new INFO_POI();
                poiInfo.nID = id;
                poiInfo.x = x;
                poiInfo.y = y;
                poiInfo.z = z;

                string szCmd = string.Format("CMD:Get2DPosition({0},{1},{2},{3})", id, x, y, z);
                INFO_POI ret = RunSyncUnityCmd("Get2DPosition", poiInfo, szCmd);

                int dx = ret.nx;
                int dy = this.Height - ret.ny;
                return new Point(dx, dy);
            }
            return new Point();
        }


        internal void OnPoistionPick(float x, float y, float z)
        {
            if (this.Parent != null)
                this.Parent.Focus();

            m_fLastPickX = x;
            m_fLastPickY = y;
            m_fLastPickZ = z;

            if(m_bAddIconMode == true)
            {
                string szCmd = string.Format("CMD:AddIconPOI('{0}',{1},{2},{3})", m_szIconName + "_11" , x, y, z);
                m_PipeServer.Send(szCmd);

                string szCmd2 = "CMD:GetLastID()";
                this.BeginInvoke(new Action(() =>
                {
                    m_PipeServer.Send(szCmd2);                    
                })); 
            }
            if(m_bAddTextMode == true)
            {
                string szText = this.m_szPoiText;
                if (szText != null && szText != "")
                {
                    string szCmd = string.Format("CMD:AddTextPOI('{0}',{1},{2},{3})", szText, x, y, z);
                    m_PipeServer.Send(szCmd);

                    string szCmd2 = "CMD:GetLastID()";
                    
                    this.BeginInvoke(new Action(() =>
                    {
                        m_PipeServer.Send(szCmd2);                        
                    })); 
                }                
            }
        }


        public int AddIconPOI(POI poi, string szIconName , float x, float y, float z)
        {
            int nLayerID = poi.Facility.GetLayerID();

            int nID = poi.Facility.ID;
            if( nID <= 0)
            {
                return AddIconPOI(szIconName + "_" + nLayerID + "_" + poi.Facility.ID, x, y, z);
            }
            else
            {
                AddIconPOI(szIconName + "_" + nLayerID + "_" + poi.Facility.ID, x, y, z);
            }
            return poi.Facility.ID;
        }


        private int m_nPoiID = 1;
        private int m_nTextID = 1;
        private int AddIconPOI(string szIconName , float x, float y, float z)
        {
            //if (y < 2.0f)
             //   y = 2.0f;

            if( m_PipeServer != null)
            {
                INFO_POI poiInfo = new INFO_POI();
                poiInfo.nID = -1;
                poiInfo.x = x;
                poiInfo.y = y;

                

                poiInfo.z = z;
                string szCmd = string.Format("CMD:AddIconPOI('{0}',{1},{2},{3})", szIconName, x, y, z);
                System.Diagnostics.Trace.WriteLine(szCmd);
                INFO_POI ret = RunSyncUnityCmd("AddIconPOI", poiInfo, szCmd, 1000, 22);
                m_nPoiID = ret.nID;

                System.Diagnostics.Trace.WriteLine("POI ID : " + m_nPoiID);
                m_nLastID = m_nPoiID;
                return m_nLastID;
            }
            return -1;
        }

        public int AddIconPOI(string szIconName, int x, int y)
        {
            if (m_PipeServer != null)
            {       
                INFO_POI poiInfo = new INFO_POI();
                string szCmd = string.Format("CMD:AddIconPOI2D('{0}',{1},{2})", m_szIconName, x - m_dX, y - m_dY);
                INFO_POI ret = RunSyncUnityCmd("AddIconPOI", poiInfo, szCmd, 2000, 22);
                m_nPoiID = ret.nID;
                m_nLastID = m_nPoiID;
                return m_nLastID;                
            }
            return -1;
        }


        private int m_nLODTextDistance = 3000;
        public int AddGroupName(string szText, float x, float y, float z)
        {
            if (szText != null && szText != "")
            {                
                INFO_POI poiInfo = new INFO_POI();
                string szCmd = string.Format("CMD:AddReverseLODTextPOI('{0}',{1},{2},{3})", szText, x, y, z);
                INFO_POI ret = RunSyncUnityCmd("AddReverseLODTextPOI", poiInfo, szCmd, m_nLODTextDistance, 24);
                m_nPoiID = ret.nID;
                m_nLastID = m_nPoiID;
                return m_nLastID;    
            }
            return -1;
        }

        public int AddTextPOI(string szText, float x, float y, float z)
        {
            if (szText != null && szText != "")
            {
                INFO_POI poiInfo = new INFO_POI();
                string szCmd = string.Format("CMD:AddTextPOI('{0}',{1},{2},{3})", szText, x, y, z);
                INFO_POI ret = RunSyncUnityCmd("AddTextPOI", poiInfo, szCmd, 2000, 23);
                m_nPoiID = ret.nID;
                m_nLastID = m_nPoiID;
                return m_nLastID;    
            }
            return -1;
        }

        public int AddTextPOI(string szText, int x, int y)
        {
            if (szText != null && szText != "")
            {
                INFO_POI poiInfo = new INFO_POI();
                string szCmd = string.Format("CMD:AddTextPOI2D('{0}',{1},{2})", szText, x - m_dX, y - m_dY);
                INFO_POI ret = RunSyncUnityCmd("AddTextPOI", poiInfo, szCmd, 2000, 23);
                m_nPoiID = ret.nID;
                m_nLastID = m_nPoiID;
                return m_nLastID;    
            }
            return -1;
        }
            

        internal void ClosePopup()
        {
            if(m_PopupMenu != null)
            {
                if(m_PopupMenu.Visible == true)
                {
                    m_PopupMenu.Close();
                }
            }
        }


        private int m_dX = -8;
        private int m_dY = -29;

        internal void ShowPopup(int x, int y)
        {
            if (m_PopupMenu != null)
            {
                m_PopupMenu.AutoClose = true;
                m_PopupMenu.Tag = new Point(x + m_dX, y + m_dY);
                m_PopupMenu.Show(this, x + m_dX, y + m_dY);
                if (m_PopupMenu.Visible == false)
                    m_PopupMenu.Show(this, x + m_dX, y + m_dY);
                else
                    m_PopupMenu.BringToFront();
            }           
        }

        internal void OnPostRightMouseUp(int x, int y)
        {
            ClearPOISelectionCmd();

            MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 1, x + m_dX, y , 0);
            OnMouseUp(this, args);


            //if (this.Parent != null)
            //    this.Parent.Focus();

            //ShowPopup(x, y);
        }

        internal void OnPostRightMouseDown(int x, int y)
        {
            MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 1, x + m_dX, y , 0);
            OnMouseDown(this, args);

            this.InvokeOnClick(this, args);

            if (this.Parent != null)
                this.Parent.Focus();

            //ClosePopup();           
        }

        internal void OnPostLeftMouseDown(int x, int y)
        {
            MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, x + m_dX, y , 0);
            OnMouseDown(this, args);
            
            this.InvokeOnClick(this, args);
            
            if (this.Parent != null)
                this.Parent.Focus();

            ClosePopup();
        }

        internal void OnPostLeftMouseUp(int x, int y)
        {
           

            MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, x + m_dX,  y , 0);
            OnMouseUp(this, args);
            
            if (this.Parent != null)
                this.Parent.Focus();

            ClosePopup();



        }

        internal void OnPostMiddleMouseDown(int x, int y)
        {
            MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Middle, 1, x + m_dX,  y , 0);
            OnMouseDown(this, args);

            this.InvokeOnClick(this, args);

            if (this.Parent != null)
                this.Parent.Focus();

            ClosePopup();
        }

        internal void OnPoseMiddleMouseUp(int x, int y)
        {
            MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Middle, 1, x + m_dX,  y , 0);
            OnMouseUp(this, args);
            if (this.Parent != null)
                this.Parent.Focus();

        }
        
        public void SendCommand(string szCmd)
        {
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }     
  
        internal void SetEnterObject(string szName)
        {
            m_szOverObjName = szName;
        }

        internal void SetLeaveObject(string szName)
        {
            if( m_szOverObjName == szName)
            {
                m_szOverObjName = "";
            }
        }

        public void SetIconPickAdd(bool bValue, Action<int, float, float, float> callback)
        {
            if (bValue == true)
            {
                m_IconPOIAddCallback = callback;
                m_bAddTextMode = false;
                m_PipeServer.Send("CMD:SetMode(2, True)");
                m_bAddIconMode = true;
            }
            else
            {
                m_IconPOIAddCallback = null;
                m_bAddIconMode = false;
                m_PipeServer.Send("CMD:SetMode(2, False)");
            }
        }

        public void SetTextPickAdd(bool bValue, Action<int, float, float, float> callback)
        {
            if (bValue == true)
            {
                m_TextPOIAddCallback = callback;
                m_bAddIconMode = false;
                m_PipeServer.Send("CMD:SetMode(2, True)");
                m_bAddTextMode = true;
            }
            else
            {
                m_TextPOIAddCallback = null;
                m_bAddTextMode = false;
                m_PipeServer.Send("CMD:SetMode(2, False)");
            }
        }

        public void SetTextColor(Color color)
        {
            int nColor = color.ToArgb();
            string szCmd = string.Format("CMD:SetTextColor({0})", nColor);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetAliasTextColor(Color color)
        {
            int nColor = color.ToArgb();
            string szCmd = string.Format("CMD:SetAliasTextColor({0})", nColor);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void AddAliasName(string szMeshName, string szAliasName)
        {
            string szCmd = string.Format("CMD:AddAliasName('{0}','{1}')", szMeshName, szAliasName);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void UpdateAliasNames()
        {
            if (m_PipeServer != null)
                m_PipeServer.Send("CMD:UpdateAliasNames()");
        }

        public void HideAllEquipmentZone()
        {
            string szCmd = "CMD:HideAllEquipZoneVolume()";
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }


        public void ShowEquipmentZone(EquipmentZone zone, bool bShow)
        {
            if( zone == null)
                return;
           
            if (bShow == true)
            {
                string szZoneName = zone.ZoneName + "_equipZone";
                string szCmd = string.Format("CMD:ShowEquipZoneVolume('{0}')", szZoneName);
                if (m_PipeServer != null)
                    m_PipeServer.Send(szCmd);
            }
            else
            {
                string szZoneName = zone.ZoneName + "_equipZone";
                string szCmd = string.Format("CMD:HideEquipZonevolume('{0}')", szZoneName);
                if (m_PipeServer != null)
                    m_PipeServer.Send(szCmd);
            }
        }


        public void SetZoomObjectDistance(float fDistance)
        {
            string szCmd = string.Format("CMD:SetZoomObjectDistance({0})", fDistance);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void ZoomTarget(float x, float y, float z)
        {
            string szCmd = string.Format("CMD:SetZoomPosition({0},{1},{2})", x, y, z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetZoomObject(string szMeshName)
        {
            string szCmd = string.Format("CMD:SetZoomObject('{0}')", szMeshName);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SelectObject(string szMeshName)
        {
            string szCmd = string.Format("CMD:SelectObject('{0}')", szMeshName);
            if (m_PipeServer != null)
            m_PipeServer.Send(szCmd);
        }

        public void ClearAllSelect()
        {
            string szCmd = "CMD:ClearAllSelect()";
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }



        public void SetHomeView()
        {
            //string szCmd = string.Format("CMD:CameraView('{0}')", "fit");
            //if (m_PipeServer != null)
            //    m_PipeServer.Send(szCmd);
        }

        public void SetFrontView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "fit");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

         public void SetTopView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "top");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetLeftView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "left");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetRightView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "right");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetRearView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "rear");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

      
 
        private bool m_bSaveHomeView = false;
        private string m_szToolKey = @"SDMS\Unity\Toolstrip";
        private string m_szHomeKey = @"SDMS\Unity\Homview";
        private Vector3 m_CamPos = null;
        private Vector3 m_Quater = null;
        private Vector3 m_CamDir = null;


        internal void OnReciveCameraPosition(float x, float y, float z)
        {            
        }

        internal void OnReciveCameraOrientaion(float x, float y, float z)
        {            
        }

        internal void OnReciveCameraDirection(float x, float y, float z)
        {           
        }

        private void GetCameraPosition()
        {
            string szCmd = "CMD:CameraPosition()";
            INFO_POI poi = new INFO_POI();
            INFO_POI ret = RunSyncUnityCmd("CameraPosition", poi, szCmd, 1000);
            m_CamPos = new Vector3(ret.x, ret.y, ret.z);
        }

        private void GetCameraOrientaion()
        {
            string szCmd = "CMD:CameraAngles()";
            INFO_POI poi = new INFO_POI();
            INFO_POI ret = RunSyncUnityCmd("CameraAngles", poi, szCmd, 1000);
            m_Quater = new Vector3(ret.x, ret.y, ret.z);
        }

        private void GetCameraDirection()
        {
            string szCmd = "CMD:CameraDirection()";
            INFO_POI poi = new INFO_POI();
            INFO_POI ret = RunSyncUnityCmd("CameraDirection", poi, szCmd, 1000);
            m_CamDir = new Vector3(ret.x, ret.y, ret.z);

            WriteHomeView(m_szCurrentHomeViewName);
            m_bSaveHomeView = true;
        }


        private void SetCameraPosition()
        {
            string szCmd = string.Format("CMD:SetCameraPosition({0},{1},{2})", m_CamPos.X, m_CamPos.Y, m_CamPos.Z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private void SetCameraOrientaion()
        {
            string szCmd = string.Format("CMD:SetCameraAngles({0},{1},{2})", m_Quater.X, m_Quater.Y, m_Quater.Z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private void SetCameraDirection()
        {
            string szCmd = string.Format("CMD:SetCameraDirection({0},{1},{2})", m_CamDir.X, m_CamDir.Y, m_CamDir.Z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private string m_szCurrentHomeViewName = "";
        public void SaveHomeView(string szName)
        {
            m_szCurrentHomeViewName = szName;
            GetCameraPosition();
            GetCameraOrientaion();
            GetCameraDirection();
        }

        public void LoadHomeView(string szName)
        {
            ReadHomeView(szName);   
            
            if(m_bSaveHomeView == true)
            {
                SetCameraPosition();
                SetCameraOrientaion();
                SetCameraDirection();
            }
        }

        private void ReadHomeView(string szName)
        {
            string szKeyName = m_szHomeKey + "\\" + szName;

            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName);
            if (rkey == null)
            {
                m_bSaveHomeView = false;
            }
            else
            {
                float x, y, z;
                string pX = (string)rkey.GetValue("POSITIONX");
                string pY = (string)rkey.GetValue("POSITIONY");
                string pZ = (string)rkey.GetValue("POSITIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;
                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_CamPos = new Vector3(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                pX = (string)rkey.GetValue("QUATERNIONX");
                pY = (string)rkey.GetValue("QUATERNIONY");
                pZ = (string)rkey.GetValue("QUATERNIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;

                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_Quater = new Vector3(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                pX = (string)rkey.GetValue("DIRECTIONX");
                pY = (string)rkey.GetValue("DIRECTIONY");
                pZ = (string)rkey.GetValue("DIRECTIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;

                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_CamDir = new Vector3(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                m_bSaveHomeView = true;
            }

            if (rkey != null)
                rkey.Close();
        }

        private void WriteHomeView(string szName)
        {
            string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

            RegistrySecurity rs = new RegistrySecurity();

            rs.AddAccessRule(new RegistryAccessRule(szUserName,
                RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Allow));

            rs.AddAccessRule(new RegistryAccessRule(szUserName,
                RegistryRights.ChangePermissions,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Deny));
            
            string szKeyName = m_szHomeKey + "\\" + szName;
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName, true);
            if (rkey == null)
            {
                try
                {
                    rkey = Registry.CurrentUser.CreateSubKey(szKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                }
                catch (Exception)
                {
                }
            }

            if (rkey != null)
            {
                if (m_CamPos != null)
                {
                    rkey.SetValue("POSITIONX", m_CamPos.X);
                    rkey.SetValue("POSITIONY", m_CamPos.Y);
                    rkey.SetValue("POSITIONZ", m_CamPos.Z);
                }
                if (m_Quater!= null)
                {
                    rkey.SetValue("QUATERNIONX", m_Quater.X);
                    rkey.SetValue("QUATERNIONY", m_Quater.Y);
                    rkey.SetValue("QUATERNIONZ", m_Quater.Z);
                }
                if (m_CamDir != null)
                {
                    rkey.SetValue("DIRECTIONX", m_CamDir.X);
                    rkey.SetValue("DIRECTIONY", m_CamDir.Y);
                    rkey.SetValue("DIRECTIONZ", m_CamDir.Z);
                }                
                rkey.Close();
            }
        }

        //private MouseWorkMode m_currentMode = MouseWorkMode.NONE;
		private MouseWorkMode m_currentMode = MouseWorkMode.PICK;

		public MouseWorkMode CurrentMouseWorkMode
		{
			get { return m_currentMode; }
			set 
            {
                m_currentMode = value;
                SetMouseMode((int)m_currentMode);
            }
		}

        
		private Zone m_currentIndoorZone = null;

		// key : POI id
		// value : POI 객체
		private Dictionary<string, POI> m_dicPOIs = new Dictionary<string, POI>();

		// Zone별 POI 리스트
		// Indoor View에서만 사용됨
		private Dictionary<Zone, ArrayList> m_dicZonePOIs = new Dictionary<Zone, ArrayList>();

        private bool m_isIndoor = false;
        public bool Indoor
        {
            get { return m_isIndoor; }
            set { m_isIndoor = value; }
        }

		// Panning 또는 Orbit, Zoom In/Out 등의 동작을 위하여 임시로 숨겨놓은 POI Popup 창 리스트
		private ArrayList m_arrTemporaryHiddenPOIs = new ArrayList();

		// POI Move등 수정이 가능한 모드
		private bool m_bEditMode = false;

		public bool EditMode
		{
			get { return m_bEditMode; }
			set
			{
				m_bEditMode = value;

                SetEditMode();

			}
		}

		private ArrayList m_arSelectedPoi = new ArrayList();

		public ArrayList SelectedPOIList
		{
			get { return m_arSelectedPoi; }
		}

		// POI Drag 모드
		private bool m_bDragPoi = false;

		// POI Drag 시작점
		private Point m_ptPrev = new Point();

		private UnE.Geometry.Vertex3F m_vDragOrigin = new UnE.Geometry.Vertex3F();

		// POI Drag Target
		private POI m_DragCurrent = null;

		// 실내뷰에서만 사용
		private bool m_meshOpened = false;

		public bool MeshOpened
		{
			get { return m_meshOpened; }
		}

		private ArrayList m_arrLODShowingPOIs = new ArrayList();
		private System.Windows.Forms.Timer timer1;

		// Important 등급의 CCTV가 화면에 보여지게될 카메라와의 최소 거리
		private static float m_fImportanceDistance = 350.0f;

        public void ShowTextPOI(int nID, bool bVisible)
        {

        }
        
        public void SetEditMode()
        {
            string szCmd = string.Format("CMD:SetEditMode({0})", m_bEditMode.ToString());
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void ShowIconPOI(int nID, string szType, bool bVisible)
        {
            string szCmd = string.Format("CMD:ShowIconPOI({0},'{1}',{2})", nID, szType, bVisible.ToString());
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }


        internal void ShowLayer(int nLayer, int nType, bool bShow)
        {
            if (nLayer == SDMS.ID.ID_LAYER_CCTV || nLayer == SDMS.ID.ID_LAYER_CCTVLOW)
            {

                string szCmd = string.Format("CMD:ShowIconLayer('{0}_{1}',{2})", CCTV.IconName(), SDMS.ID.ID_LAYER_CCTV, bShow.ToString());
                if (m_PipeServer != null)                    
                    m_PipeServer.Send(szCmd);

                szCmd = string.Format("CMD:ShowIconLayer('{0}_{1}',{2})", CCTV.IconName(), SDMS.ID.ID_LAYER_CCTVLOW, bShow.ToString());
                if (m_PipeServer != null)
                    m_PipeServer.Send(szCmd);
            }
            else if(nLayer == SDMS.ID.ID_LAYER_CCTV_DISCONNECTED)
            {
                string szCmd = string.Format("CMD:ShowIconLayer('{0}_{1}',{2})", CCTV.IconName(), nLayer, bShow.ToString());
                if (m_PipeServer != null)
                    m_PipeServer.Send(szCmd);
            }
            else if(nLayer == SDMS.ID.ID_LAYER_BUILDING_TEXT)
            {
                string szCmd = string.Format("CMD:ShowBuildingText({0})", bShow.ToString());
                if (m_PipeServer != null)
                    m_PipeServer.Send(szCmd);

            }
        }

        public void  EnablePOI(int nID, string szType, bool bEnable)
        {

        }

		public void OnMouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DoMouseWork(sender, e, MouseEvent.MOUSE_DOWN);

            if (m_Owner != null)
                m_Owner.OnPostPanelMouseDown();
		}

		public void OnMouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DoMouseWork(sender, e, MouseEvent.MOUSE_UP);

			if (e.Button == MouseButtons.Left)
			{
				if (m_currentMode == MouseWorkMode.PICK)
				{
					// IF NOT POI MOVE MODE
					//if (m_bDragPoi == false)
					//PickPOI(e.X, e.Y);
					//e//lse
					//{
					//	if (m_DragCurrent != null)
					//		OnPostMovePOI(m_DragCurrent, e);

					//	TurnOnTemporaryList();
					//}
					m_DragCurrent = null;
					m_bDragPoi = false;
				}
				else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
				{
					CreateFireSensor(e, null);
				}
				else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
				{
					CreateSpringCooler(e, null);
				}
				else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
				{
					CreatePumpPressure(e, null);
				}
				else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
				{
					//DeletePOI(e.X, e.Y);
				}
				else if (m_currentMode == MouseWorkMode.NEW_CCTV)
				{
					CreateCCTVPOI(e, null);
				}
			}
			//Invalidate(true);
		}

		public void OnMouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DoMouseWork(sender, e, MouseEvent.MOUSE_MOVE);

#if DEBUG
			//Position3D pos = GetCameraPosition();
			//Quaternion3D ori = GetCameraOrientaion();
			//Position3D dir = GetCameraDirection();

			//if (pos != null)
			//{
			//    Debug.WriteLine("POSITION : " + pos.X + "," + pos.Y + "," + pos.Z);
			//    Debug.WriteLine("DIRECTION : " + dir.X + "," + dir.Y + "," + dir.Z);
			//    Debug.WriteLine("ORIENTATION : " + ori.X + "," + ori.Y + "," + ori.Z + ","+ ori.W );
			//}
#endif
		}


        public void SaveViewState(string szName)
        {
            GetCameraPosition();
            GetCameraOrientaion();
            GetCameraDirection();

            WriteHomeView(szName);
        }

        public void LoadViewState(string szName)
        {
            LoadHomeView(szName);
            
            SetFrontView();

			SetCameraPosition();
			SetCameraOrientaion();
			SetCameraDirection();
			
            Update3D();
        }
         
        private void DoMouseWork(Object sender, MouseEventArgs e, MouseEvent mouseEvent)
        {
            if (m_Owner != null)
                m_Owner.HideAllPopup();

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            { 
                if (m_currentMode == MouseWorkMode.PICK)
                {
                    if (this.Focused == false)
                        Focus();

                    if (mouseEvent == MouseEvent.MOUSE_DOWN)
                    {
                        if (m_bEditMode == true)
                        {
                            //// SET POI MOVE MODE
                            //m_ptPrev.X = e.X;
                            //m_ptPrev.Y = e.Y;
                            //int nPOIID = base.OnSelectPOI(e.X, e.Y);

                            //Position3D pos3d = base.Get3DPoint(e.Location);
                            //System.Diagnostics.Trace.WriteLine("3D: " + pos3d.X + "," + pos3d.Z);
                            //if (nPOIID != -1)
                            //{
                            //    if (m_dicPOIs.ContainsKey(nPOIID))
                            //    {
                            //        m_DragCurrent = m_dicPOIs[nPOIID];
                            //        if (m_DragCurrent != null)
                            //        {
                            //            m_vDragOrigin.SetVertex(m_DragCurrent.X, m_DragCurrent.Y, m_DragCurrent.Z);
                            //            OnPostPick(null, m_arrTemporaryHiddenPOIs, false);
                            //        }
                            //    }
                            //}
                        }
                    }
                    else if (mouseEvent == MouseEvent.MOUSE_MOVE)
                    {
                        if (m_bEditMode == true)
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                //int dx = e.X - m_ptPrev.X;
                                //int dy = e.Y - m_ptPrev.Y;
                                //// POI MOVE
                                //if (m_DragCurrent != null && dx != 0 && dy != 0)
                                //{
                                //    POI poi = m_DragCurrent;
                                //    Point pt = (Point)this.Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
                                //    pt.X = pt.X + dx;
                                //    pt.Y = pt.Y + dy;
                                //    Position3D pos = Get3DPoint(pt);
                                //    m_ptPrev.X = e.X;
                                //    m_ptPrev.Y = e.Y;

                                //    if (base.MovePOI(poi.ID, pos.X, pos.Y, pos.Z))
                                //    {
                                //        poi.X = pos.X;
                                //        poi.Y = pos.Y;
                                //        poi.Z = pos.Z;                                      

                                //        m_bDragPoi = true;
                                //    }
                                //}
                            }
                            else
                            {
                                m_bDragPoi = false;
                                m_DragCurrent = null;
                            }
                        }
                    }
                }
                else if (m_currentMode == MouseWorkMode.PANNING)
                {
                    OnPrevPanning(mouseEvent);

                   // MouseEventArgs arg = new MouseEventArgs(MouseButtons.Middle, e.Clicks, e.X, e.Y, e.Delta);
                   // baseHandler(sender, arg);

                    OnPostPanning(mouseEvent);
                }
                else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_CCTV)
                { }
                else
                {
                    OnPrevOrbit(mouseEvent);
                    //baseHandler(sender, e);
                    OnPostOrbit(mouseEvent);
                }
            }
            else
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    //Point pt = PointToScreen(new Point(e.X, e.Y));

                    if (mouseEvent == MouseEvent.MOUSE_UP)
                    {
                        Point pt = PointToScreen(new Point(e.X, e.Y - 26));
                        if (this.PopupMenu != null && this.PopupMenu.Enabled == true)
                        {
                            this.PopupMenu.Show(pt.X, pt.Y);
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine("PopupDisabled");
                        }

                    }
                    if (mouseEvent == MouseEvent.MOUSE_DOWN)
                    {
                        string szBuildingName = m_szOverObjName;
                        m_szPopupObjName = m_szOverObjName;
                        Vector3 pos = GetPosition3D(e.X, e.Y);

                        this.PopupMenu.Tag = pos;


                        m_Owner.MenuIndoor.Enabled = false;

                        ToolStripItemCollection c = m_Owner.MenuIndoor.DropDownItems;
                        c.Clear();

                        ToolStripItemCollection r = m_Owner.MenuManualReport.DropDownItems;
                        r.Clear();

                        ToolStripItemCollection v = m_Owner.MenuManualCCTV.DropDownItems;
                        v.Clear();

                        Building building = null;
                        bool useIndoor3D = false;
                        if (m_isIndoor)
                        {
                            if (m_currentIndoorZone != null)
                            {
                                //EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(m_currentIndoorZone, pos.X, pos.Z);
                                //if (equipZone == null)
                                {
                                    building = m_currentIndoorZone.Building;
                                    m_Owner.MenuIndoor.Enabled = false;
                                    m_Owner.MenuManualReport.Tag = m_currentIndoorZone;
                                    m_Owner.MenuManualCCTV.Tag = m_currentIndoorZone;
                                }
                            }
                        }
                        else
                        {                          

                            building = m_Owner.GetBuilding(szBuildingName);
                           
                            if (building != null)
                            {
                                
                                if (building.BuildingID == "05" || building.BuildingID == "09" || building.BuildingID.StartsWith("500-"))
                                    useIndoor3D = true;
                                // 영흥
                                //if (szBuildingName == "yhz1" || szBuildingName == "yhz2")
                                //{
                                //    int nResult = CheckScenePosition(szBuildingName, 0, posCurrent.X);
                                //    if (nResult > 0)
                                //        building = ZoneManager.Instance.GetBuilding(szBuildingName + "_1");
                                //}
                                //else if (szBuildingName == "yhz3")
                                //{
                                //    int nResult = CheckScenePosition(szBuildingName, 0, posCurrent.X);
                                //    if (nResult < 0)
                                //        building = ZoneManager.Instance.GetBuilding(szBuildingName + "_1");
                                //}


                                foreach (Zone zone in building.FloorList)
                                {
                                    ToolStripMenuItem item = new ToolStripMenuItem();
                                    item.Tag = zone;
                                    item.Click += m_Owner.MenuIndoorClicked;
                                    item.Text = zone.DisplayText;
                                    c.Add(item);

                                    ToolStripMenuItem item2 = new ToolStripMenuItem();
                                    item2.Tag = zone;
                                    item2.Click += m_Owner.MenualReportClicked;
                                    item2.Text = zone.DisplayText;
                                    r.Add(item2);

                                    ToolStripMenuItem item3 = new ToolStripMenuItem();
                                    item3.Tag = zone;
                                    item3.Click += m_Owner.ManualCCTVClicked;
                                    item3.Text = zone.DisplayText;
                                    v.Add(item3);
                                }

                                m_Owner.MenuManualReport.Tag = building;
                                m_Owner.MenuManualCCTV.Tag = building;
                            }
                            if (c.Count == 0)
                                m_Owner.MenuIndoor.Enabled = false;

                            if (building == null)
                            {
                                Zone zone = m_Owner.GetOutsideZone(pos.X, pos.Z);
                                if (zone != null)
                                {
                                    m_Owner.MenuManualReport.Tag = null;
                                    m_Owner.MenuManualCCTV.Tag = null;

                                    ToolStripMenuItem item2 = new ToolStripMenuItem();
                                    item2.Tag = zone;
                                    item2.Click += m_Owner.MenualReportClicked;
                                    item2.Text = zone.DisplayText;
                                    r.Add(item2);

                                    ToolStripMenuItem item3 = new ToolStripMenuItem();
                                    item3.Tag = zone;
                                    item3.Click += m_Owner.ManualCCTVClicked;
                                    item3.Text = zone.DisplayText;
                                    v.Add(item3);
                                }
                            }
                        }

                        if (this.m_isIndoor == false)
                        {
                            //if (c.Count == 0)

                                m_Owner.MenuIndoor.Enabled = useIndoor3D;
                            //else
                            //    m_Owner.MenuIndoor.Enabled = true;
                            if (v.Count == 0)
                                m_Owner.MenuManualCCTV.Enabled = false;
                            else
                                m_Owner.MenuManualCCTV.Enabled = true;
                            if (r.Count == 0)
                                m_Owner.MenuManualReport.Enabled = false;
                            else
                                m_Owner.MenuManualReport.Enabled = true;

                            if (c.Count == 0 && v.Count == 0 && r.Count == 0)
                            {
                                ToolStrip ts = m_Owner.MenuManualReport.Owner;
                                ts.Enabled = false;
                            }
                            else
                            {
                                ToolStrip ts = m_Owner.MenuManualReport.Owner;
                                ts.Enabled = true;
                            }
                        }
                    }

                    return;
                }

                //if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                //    OnPrevPanning(mouseEvent);

                //baseHandler(sender, e);

                //if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                //    OnPostPanning(mouseEvent);
            }

            if (e.Button == MouseButtons.None && mouseEvent == MouseEvent.MOUSE_MOVE)
            {
                ShowTooltip(e);
            }
            else
            {
                OnMouseLeave(this, new EventArgs());
            }
        }

        private void OnPrevPanning(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPrevOrbit(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPostOrbit(MouseEvent e)
        {
            if (e == MouseEvent.MOUSE_UP)
                TurnOnTemporaryList();
            else
                OnScreenMove();
        }

        private void OnPostPanning(MouseEvent e)
        {
            if (e == MouseEvent.MOUSE_UP)
            {
                TurnOnTemporaryList();

                // LOD에 따라 CCTV POI들을 가시화한다.
                ProcessCCTVLOD();
            }
            else
                OnScreenMove();
        }
        
        private bool IsInCamera(Point pt)
        {
            if (pt.X < 0 || pt.Y < 0)
                return false;
            if (pt.X > this.Width)
                return false;
            if (pt.Y > this.Height)
                return false;
            return true;
        }

		public void ProcessCCTVLOD()
		{
            Type type = typeof(CCTV);
            m_arrLODShowingPOIs.Clear();

            foreach (KeyValuePair<string, POI> pair in m_dicPOIs)
            {
                POI poi = pair.Value;

                if (poi.Popup == null || poi.Facility == null || poi.Facility.GetType() != type)
                    continue;

                CCTV cctv = (CCTV)poi.Facility;

                if (cctv.LODType == CCTV.LOD.VERY_IMPORTANT)
                {
                    Point pt = GetPosition2D(poi.ID, poi.X, poi.Y, poi.Z);

                    if (IsInCamera(pt))
                    {
                        if (!poi.Popup.IsVisible())
                        {                           
                            poi.Popup.Show(pt.X, pt.Y);
                        }

                        m_arrLODShowingPOIs.Add(poi);
                    }
                    else
                    {
                        IPOIPopup ctrl = poi.Popup;
                        if (ctrl != null)
                            ctrl.Hide(false);
                    }
                }
                else if (cctv.LODType == CCTV.LOD.IMPORTANT)
                {
                    Point pt = GetPosition2D(poi.ID, poi.X, poi.Y, poi.Z);
                    if (IsInCamera(pt))// && GetPOIDistance(poi.ID) <= m_fImportanceDistance)
                    {
                        if (!poi.Popup.IsVisible())
                        {                          
                            poi.Popup.Show(pt.X, pt.Y);
                        }

                        m_arrLODShowingPOIs.Add(poi);
                    }
                    else
                    {
                        IPOIPopup ctrl = poi.Popup;
                        if (ctrl != null)
                            ctrl.Hide(false);
                    }
                }
            }
		}


        private void TurnOnWheelTemporaryList()
        {
            foreach (POI poi in m_arrTemporaryHiddenPOIsForWheel)
            {
                Point pt = GetPosition2D(poi.ID, poi.X, poi.Y, poi.Z);
                poi.Popup.Show(pt.X, pt.Y);

                //Control c = (Control)poi.Popup;
                //if( c!= null)
                //{
                //    c.BringToFront();
                //}

                //ShowWindow(poi.Popup.Handle, SW_RESTORE);
                //SetForegroundWindow(poi.Popup.Handle);
            }
            m_arrTemporaryHiddenPOIsForWheel.Clear();

            Update3D();
        }

		private void TurnOnTemporaryList()
		{
            foreach (POI poi in m_arrTemporaryHiddenPOIs)
            {
                Point pt = GetPosition2D(poi.ID, poi.X, poi.Y, poi.Z);
                poi.Popup.Show(pt.X, pt.Y);

                //ShowWindow(m_hWndUnity, SW_RESTORE);
                //SetForegroundWindow(poi.Popup.Handle);
            }
            m_arrTemporaryHiddenPOIs.Clear();


            Update3D();

		}



        public void OnDragPOI(string szID, float x, float y, float z)
        {
            if (m_dicPOIs.ContainsKey(szID))
            {
                POI poi = m_dicPOIs[szID];
                if (poi != null)
                {
                    OnPostMovePOI(poi, x, y, z);
                    //TurnOnTemporaryList();
                }
            }            
        }

        /// <summary>
        /// 드래그되는 POI의 데이터 변화를 저장하는 함수
        /// </summary>
        /// <param name="poi">선택된 POI</param>
        /// <param name="x">변경된 x</param>
        /// <param name="y">변경된 y</param>
        /// <param name="z">변경된 z</param>
		private void OnPostMovePOI(POI poi, float x, float y, float z)
		{
            m_vDragOrigin = new UnE.Geometry.Vertex3F(poi.X, poi.Y, poi.Z);
            float fDistance = m_vDragOrigin.GetDistance(new UnE.Geometry.Vertex3F(x, y, z));
            if (fDistance <= UnE.Geometry.Math.HALF_TOLERANCE())
                return;

            poi.X = x;
            poi.Y = y;
            poi.Z = z;

            if (poi.Type == IFacility.FacilityType.CCTV)
            {
                AddCCTVEditData(poi);
            }
            else if (poi.Type == IFacility.FacilityType.PRESSURE_SENSOR)
            {
                AddPressureSensorEditData(poi);
            }

		}

		private void AddCCTVEditData(POI poi)
		{
            if( m_Owner != null)
            {
                Zone zone = GetPOIZone(poi.X, poi.Y, poi.Z);
                m_Owner.AddCCTVEditData(poi, zone);
            }           
		}

        private void AddPressureSensorEditData(POI poi)
        {
            if (m_Owner != null)
            {
                Zone zone = GetPOIZone(poi.X, poi.Y, poi.Z);
                m_Owner.AddPressureSensorEditData(poi, zone);
            }
        }



		public void HideAllPOIPopup()
		{
			OnPostPick(null, null, true);
		}

        private void OnPostPick(POI poi, ArrayList arrHidden = null, bool absolutely = false)
        {
            bool refresh = false;

            if (arrHidden != null)
                arrHidden.Clear();

            if (m_isIndoor)
            {
                if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    foreach (POI _poi in arrPOIs)
                    {
                        if (_poi == poi || _poi.Popup == null || !_poi.Popup.IsVisible())
                            continue;

                        // 임시로 감췄다가 다시 나타나도록 하는 기능 사용안함
                        // [2017-04-05] 김지웅
                        //if (arrHidden != null)// && IsLODShowingPOI(_poi))
                        //    arrHidden.Add(_poi);

                        //_poi.Popup.Hide(absolutely);
                        //refresh = true;

                        if (absolutely == false)
                        {
                            _poi.Popup.Hide(absolutely);
                            refresh = true;
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, POI> pair in m_dicPOIs)
                {
                    if (pair.Value == poi || pair.Value.Popup == null || !pair.Value.Popup.IsVisible())
                        continue;

                    // 임시로 감췄다가 다시 나타나도록 하는 기능 사용안함
                    // [2017-04-05] 김지웅
                    //if (arrHidden != null)// && IsLODShowingPOI(pair.Value))
                    //    arrHidden.Add(pair.Value);

                    //if (pair.Value.Popup != null)
                    //    pair.Value.Popup.Hide(absolutely);

                    //refresh = true;

                    if (absolutely == false)
                    {
                        if (pair.Value.Popup != null)
                            pair.Value.Popup.Hide(absolutely);

                        refresh = true;
                    }
                }
            }

            //if (poi != null)
            //    FormMain.Instance.PageHome.OnPostPickPOI(poi);

            if (refresh)
            {
                Update();
            }
        }

        private bool IsLODShowingPOI(POI poi)
        {
            return m_arrLODShowingPOIs.Contains(poi);
        }

        private void OnScreenMove()
        {
            bool refresh = false;

            if (m_isIndoor)
            {
                if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    foreach (POI poi in arrPOIs)
                    {
                        if (OnMovePOI(poi))
                            refresh = true;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, POI> pair in m_dicPOIs)
                {
                    if (OnMovePOI(pair.Value))
                        refresh = true;
                }
            }

            if (refresh)
            {
                Update();
            }
        }

        private bool OnMovePOI(POI poi)
        {
            IPOIPopup popup = poi.Popup;

            if (popup != null && popup.IsVisible())
            {
                // 시스템 안정성의 이유로 3D View 이동에 따른 Popup창 위치변경을 하지 않는다.
                // [2017-04-05] 김지웅
                //Point pt = GetPosition2D(poi.ID, poi.X, poi.Y, poi.Z);
                //popup.Show(pt.X, pt.Y);
                return true;
            }

            return false;
        }

        public POI CreateFireSensor(MouseEventArgs e, Zone zone)
        {
            //Position3D pos = Get3DPoint(new Point(e.X, e.Y));
            if (m_Owner == null)
                return null;

            Vector3 pos = GetPosition3D(e.X, e.Y);

            FireSensor sensor = new FireSensor();
            POI poi = new POI();

            poi.X = pos.X;
            poi.Y = pos.Y;
            poi.Z = pos.Z;
            poi.Facility = sensor;
            //poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
            poi.IsIndoor = m_isIndoor;


            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (m_isIndoor)
            {
                poi.Zone = zone == null ? m_currentIndoorZone : zone;
            }
            else
            {
                poi.Zone = zone == null ? GetPOIZone(pos.X, pos.Y, pos.Z) : zone;
            }
            
            EquipmentZone equipZone = m_Owner.CheckEquipmentZone(poi.Zone, pos.X, pos.Z);
            if (equipZone == null)
                return null;

            sensor.EquipZoneID = equipZone.ID;

            string strPath = sensor.IconPath;
            int nID = AddIconPOI(poi, strPath, pos.X, pos.Y, pos.Z);
            poi.ID = nID;
            // set pick size;
            //base.SetPickSize(nID, 55, 55);

            string szKey  = GetKey(nID, strPath);
            m_dicPOIs[szKey] = poi;

            m_Owner.SelectedPOI = poi;

            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_DETECTOR).Add(nID);

            if (m_isIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            m_Owner.EditFireSensor(sensor);
            return poi;
        }

        public POI CreateSpringCooler(MouseEventArgs e, Zone zone)
        {
            if (m_Owner == null)
                return null;

            Vector3 pos = GetPosition3D(e.X, e.Y);

            SpringCooler sensor = new SpringCooler();
            POI poi = new POI();
            poi.X = pos.X;
            poi.Y = pos.Y;
            poi.Z = pos.Z;
            poi.Facility = sensor;
            poi.IsIndoor = m_isIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (m_isIndoor)
            {
                poi.Zone = zone == null ? m_currentIndoorZone : zone;
            }
            else
            {
                poi.Zone = zone == null ? GetPOIZone(pos.X, pos.Y, pos.Z) : zone;
            }
            EquipmentZone equipZone = m_Owner.CheckEquipmentZone(poi.Zone, pos.X, pos.Z);
            if (equipZone == null)
                return null;

            sensor.EquipZoneID = equipZone.ID;
            string strPath = sensor.IconPath;
            int nID = AddIconPOI(poi, strPath, pos.X, pos.Y, pos.Z);
            poi.ID = nID;
            
            // set pick size;
            //.SetPickSize(nID, 55, 55);
            string szKey = GetKey(nID, strPath);
            m_dicPOIs[szKey] = poi;

            m_Owner.SelectedPOI = poi;
            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_COOLER).Add(nID);

            if (m_isIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            m_Owner.EditSpringCooler(sensor);
            return poi;
        }

        public POI CreatePumpPressure(MouseEventArgs e, Zone zone)
        {
            if (m_Owner == null)
                return null;

            Vector3 pos = GetPosition3D(e.X, e.Y + 50); ;
            PumpPressureSensor sensor = new PumpPressureSensor();

            POI poi = new POI();
            poi.X = pos.X;
            poi.Y = pos.Y;
            poi.Z = pos.Z;
            poi.Facility = sensor;
            //poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
            poi.IsIndoor = m_isIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (m_isIndoor)
            {
                poi.Zone = zone == null ? m_currentIndoorZone : zone;
            }
            else
            {
                poi.Zone = zone == null ? GetPOIZone(pos.X, pos.Y, pos.Z) : zone;
            }

            EquipmentZone equipZone = m_Owner.CheckEquipmentZone(poi.Zone, pos.X, pos.Z);
            if (equipZone == null)
            {
                sensor.EquipZoneID = 0;
            }
            else
                sensor.EquipZoneID = equipZone.ID;
            string strPath = sensor.IconPath;
            int nID = AddIconPOI(poi, strPath, pos.X, pos.Y, pos.Z);
            poi.ID = nID;
            
            // set pick size;
            //base.SetPickSize(nID, 55, 55);
            string szKey = GetKey(nID, strPath);
            m_dicPOIs[szKey] = poi;
            m_Owner.SelectedPOI = poi;
            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_PERSURE).Add(nID);

            if (m_isIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            m_Owner.EditPumpPressureSensor(sensor);
            return poi;
        }

        public POI CreateCCTVPOI(MouseEventArgs e, Zone zone)
        {
            //return null;

            if (m_Owner == null)
                return null;

            Vector3 pos = GetPosition3D(e.X, e.Y);

            CCTV cctv = new CCTV();
            POI poi = new POI();           
            poi.X = pos.X;
            poi.Y = pos.Y;
            poi.Z = pos.Z;
            poi.Facility = cctv;

            string strPath = cctv.IconPath;            
            int nID = AddIconPOI(cctv.POI, strPath, pos.X, pos.Y, pos.Z);
            poi.ID = nID;

            poi.IsIndoor = m_isIndoor;


            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (m_isIndoor)
            {
                poi.Zone = zone == null ? m_currentIndoorZone : zone;
            }
            else
            {
                poi.Zone = zone == null ? GetPOIZone(pos.X, pos.Y, pos.Z) : zone;
            }
            string szKey = GetKey(nID, strPath);
            m_dicPOIs[szKey] = poi;
            m_Owner.SelectedPOI = poi;
            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_CCTV).Add(nID);

            if (m_isIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);
            }

            string szName = m_szOverObjName;
            Building building = m_Owner.GetBuilding(szName);
            if (building == null)
                m_Owner.EditCCTV(cctv);
            else
                m_Owner.EditCCTV(cctv, building.DisplayText);
            return poi;
        }

        private ArrayList m_arFireEquips = new ArrayList();

        public POI CreateFireEquipmentPOI(float x, float y, float z, FireEquipment equip, Zone zone)
        {
            if (m_Owner == null)
                return null;

            string strPath = equip.IconPath;

            POI poi = new POI();            
            poi.X = x;
            poi.Y = y;
            poi.Z = z;
            poi.Facility = equip;
            poi.IsIndoor = m_isIndoor;

            int nID = AddIconPOI(poi, strPath, x, y, z);
            poi.ID = nID;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (zone == null)
            {
                if (m_isIndoor)
                    poi.Zone = m_currentIndoorZone;
                else
                {
                    Point pt = GetPosition2D(nID, x, y, z);
                    MouseEventArgs e = new MouseEventArgs(MouseButtons.Right, 0, pt.X, pt.Y, 0);
                    poi.Zone = GetPOIZone(x, y, z);
                }
            }
            else
                poi.Zone = zone;
            string szKey = GetKey(nID, strPath);
            m_dicPOIs[szKey] = poi;

            m_Owner.SelectedPOI = poi;
            mLayerManager.GetLayer(equip.GetLayerID()).Add(nID);

            if (m_isIndoor && poi.Zone != null)
            {
                ArrayList arrPOIs = null;

                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    arrPOIs = m_dicZonePOIs[poi.Zone];
                else
                {
                    arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                }

                if (!arrPOIs.Contains(poi))
                    arrPOIs.Add(poi);

                m_arFireEquips.Add(poi);
            }
            return poi;
        }

        private Zone GetPOIZone(float x, float y, float z)
        {
            if (m_isIndoor)
            {
                return m_currentIndoorZone;

                //float nCurrentFloorIndex = -1.0f;
                //Building building = m_frmParent.GetCurrentBuilding(ref nCurrentFloorIndex);

                //if (building == null)
                //    return null;

                //return ZoneManager.Instance.GetZone(building.BuildingID, nCurrentFloorIndex);
            }
            
  
            //MouseEventArgs arg = new MouseEventArgs(MouseButtons.Right, e.Clicks, e.X, e.Y, e.Delta);
            ////base.OnMouseUp(this, arg);
            //base.OnSavePt(arg);


            string strBuildingID = m_szOverObjName;
            if (strBuildingID == "")
            {
                //ClearSelect();
                return m_Owner.GetOutsideZone(x, z);
            }
            else
            {
                //ClearSelect();
                Building building = m_Owner.GetBuilding(strBuildingID);
                if (building != null)
                {
                    Zone zone = m_Owner.GetZone(strBuildingID, building.MaxFloorIndex - 1);
                    if (zone == null)
                    {
                        return m_Owner.GetOutsideZone(x, z);
                    }
                    return zone;
                }
            }
            return m_Owner.GetOutsideZone(x, z);
        }

		public POI FindPOI(int nID, string szType)
		{
            string szKey = string.Format("{0}_{1}", szType, nID);
            if (m_dicPOIs.ContainsKey(szKey))
                return m_dicPOIs[szKey];

			return null;
		}

        public POI FindPOI(string szKey)
        {
            if (m_dicPOIs.ContainsKey(szKey))
                return m_dicPOIs[szKey];

            return null;
        }

		public bool DeletePOI(string szID)
		{
            if (!m_dicPOIs.ContainsKey(szID))
                return false;

            POI poi = m_dicPOIs[szID];

            if (m_dicPOIs.Remove(szID))
            {
                POI poiSelected = m_Owner.SelectedPOI;
                if (poiSelected != null && poiSelected.Facility != null)
                {
                  
                    m_Owner.SelectedPOI = null;
                }

                if (poi.Facility != null)
                {
                    m_Owner.RemoveCCTVPOI(poi.ID);

                    RemoveIconPOI(poi.ID, poi.Facility.IconPath);
                }

                

                if (poi.Zone != null)
                {
                    if (m_dicZonePOIs.ContainsKey(poi.Zone))
                    {
                        ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
                        arrPOIs.Remove(poi);
                    }
                }

                OnPostDeletePOI(poi);
                return true;
            }

			return false;
		}

        private void OnPostDeletePOI(POI poi)
        {
            if( m_Owner != null)
            {
                m_Owner.OnDeletePOI(poi);
            }
            
        }

		public void DeleteAllPOIs()
		{
            foreach (KeyValuePair<string, POI> pair in m_dicPOIs)
            {

                int nID = pair.Value.ID;
                string type = pair.Value.Facility.IconPath;
                m_Owner.RemoveCCTVPOI(nID);
                

                if (pair.Value.Popup != null)
                    pair.Value.Popup.Close();
                RemoveIconPOI(nID, type);
            }

            if (m_isIndoor && m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
                    arrPOIs.Clear();
                }
            }

            m_dicPOIs.Clear();
            if (m_Owner != null)
                m_Owner.SelectedPOI = null;
		}

        private IPopupFactory m_Factory = null;
		public void AddPOI(POI poi)
		{

            //return;

            poi.ParentView = this;

            if (poi.Facility == null)
                return;

            if(m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
            
            if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

            if (!m_isIndoor || (m_isIndoor && poi.Zone == m_currentIndoorZone))
            {
                string strIconPath = poi.Facility.IconPath;
                int nID = AddIconPOI(poi, strIconPath, poi.X, poi.Y, poi.Z);
                poi.ID = nID;
                string szKey = string.Format("{0}_{1}", strIconPath, nID);
               
                m_dicPOIs[szKey] = poi;
            }
            else if (poi.ID > 0)
            {
                string strIconPath = poi.Facility.IconPath;
                string szKey = string.Format("{0}_{1}", strIconPath, poi.ID);
                m_dicPOIs[szKey] = poi;
            }

            if (m_isIndoor && poi.Zone != null)
            {
                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
                    arrPOIs.Add(poi);
                }
                else
                {
                    ArrayList arrPOIs = new ArrayList();
                    m_dicZonePOIs[poi.Zone] = arrPOIs;
                    arrPOIs.Add(poi);
                }
            }
		}
		
		private System.Windows.Forms.Timer m_WheelTimer = null;
		private static bool m_bWheelProcess = false;
		private ArrayList m_arrTemporaryHiddenPOIsForWheel = new ArrayList();

        public void OnPostMiddleMouseUp(int x, int y)
        {

          
            m_bWheelProcess = true;

            if (m_WheelTimer == null || m_WheelTimer.Enabled == false)
            {
                if (m_WheelTimer == null)
                {
                    m_WheelTimer = new System.Windows.Forms.Timer();
                    m_WheelTimer.Interval = 600;
                    m_WheelTimer.Tick += new System.EventHandler(OnWheelTimerTick);
                }

                m_WheelTimer.Enabled = true;
                m_WheelTimer.Start();

                OnPostPick(null, m_arrTemporaryHiddenPOIsForWheel, true);
            }


            OnScreenMove();

            m_bWheelProcess = false;
        }
        public void OnMouseWheel(int x, int y, int delta)
        {
            m_bWheelProcess = true;

            if (m_PipeServer != null)
            {
                if( delta > 0)
                {
                    string szCmd = string.Format("CMD:ModelZoom({0})", (float)1);
                    m_PipeServer.Send(szCmd);
                }
                else if(delta < 0)
                {
                    string szCmd = string.Format("CMD:ModelZoom({0})", (float)-1);
                    m_PipeServer.Send(szCmd);
                }                
            }           

            if (m_WheelTimer == null || m_WheelTimer.Enabled == false)
            {
                if (m_WheelTimer == null)
                {
                    m_WheelTimer = new System.Windows.Forms.Timer();
                    m_WheelTimer.Interval = 600;
                    m_WheelTimer.Tick += new System.EventHandler(OnWheelTimerTick);
                }

                m_WheelTimer.Enabled = true;
                m_WheelTimer.Start();

                OnPostPick(null, m_arrTemporaryHiddenPOIsForWheel, true);
            }
            OnScreenMove();
            m_bWheelProcess = false;
        }

		private void OnWheelTimerTick(object sender, EventArgs e)
		{
			if (m_bWheelProcess == false)
			{
				TurnOnWheelTemporaryList();
				ProcessCCTVLOD();
			}
			m_WheelTimer.Enabled = false;
			m_WheelTimer.Stop();
		}

		public void ShowLayer(int nLayer, bool bShow)
		{
			foreach (KeyValuePair<string, POI> kv in m_dicPOIs)
			{
				POI poi = kv.Value;
				if (poi.Popup != null)
				{
					if (poi.Facility.GetLayerID() == nLayer)
					{
						poi.Popup.LayerVisible = bShow;
					}
				}
			}
		}


		public void UpdatePOI()
		{
			OnScreenMove();
		}

		public bool IsTemporaryHiddenPOI(POI poi)
		{
			bool bResult = m_arrTemporaryHiddenPOIs.Contains(poi);
			bool bResult2 = m_arrTemporaryHiddenPOIsForWheel.Contains(poi);
			return (bResult || bResult2);
		}	

		private void LoadFireEquipments()
		{
            if (m_isIndoor)
            {
                ArrayList arrEquipments = m_Owner.GetFireEquipments(this.m_currentIndoorZone);

                if (arrEquipments != null)
                {
                    foreach (FireEquipment equip in arrEquipments)
                    {
                        if (equip.GroupID != -1)
                            CreateFireEquipmentPOI(equip.X, equip.Y, equip.Z, equip, equip.Zone);
                    }
                }
            }
		}

		private void ClearFireEquipments()
		{
            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_FIREEXT).Objects.Clear();
            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_FIREHYD).Objects.Clear();
            mLayerManager.GetLayer(SDMS.ID.ID_LAYER_ALARMSTA).Objects.Clear();

            foreach (POI poi in m_arFireEquips)
            {
                ArrayList arrPOIs = null;
                if (m_dicZonePOIs.ContainsKey(poi.Zone))
                {
                    arrPOIs = m_dicZonePOIs[poi.Zone];

                    if (arrPOIs.Contains(poi))
                        arrPOIs.Remove(poi);
                }
            }
            m_arFireEquips.Clear();
		}

		public void ClearAllData()
        {
            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    foreach (POI poi in arrPOIs)
                    {
                        // 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
                        RemoveIconPOI(poi.ID, poi.Facility.IconPath);

                        if (poi.Popup != null)
                        {
                            poi.Popup.Close();
                            poi.Popup = null;
                        }
                    }
                }
            }
		}

        private void ClearPOISelectionCmd()
        {           
            // Clear All Select Unity            
            try
            {
                INFO_POI info = new INFO_POI();
                info.bSelect = 0;
                info.bSet = 0;

                string szCMD = "CMD:ClearSelectIconPOI(1, 'CCTV')";
                RunSyncUnityCmd("ClearSelectIconPOI", info, szCMD, 1000);
            }
            catch (Exception)
            {
            }            
        }

		public void ClearPOISelection()
		{
            if (m_arSelectedPoi.Count > 0)
            {
                m_arSelectedPoi.Clear();                
            }
            
		}

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Panel4Unity));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton10 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton11 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton12 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton13 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripButton7,
            this.toolStripButton8,
            this.toolStripButton9,
            this.toolStripButton10,
            this.toolStripButton11,
            this.toolStripButton12,
            this.toolStripButton13});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(100, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton6.Text = "toolStripButton6";
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton7.Text = "toolStripButton7";
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton8.Text = "toolStripButton8";
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton9.Image")));
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton9.Text = "toolStripButton9";
            // 
            // toolStripButton10
            // 
            this.toolStripButton10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton10.Image")));
            this.toolStripButton10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton10.Name = "toolStripButton10";
            this.toolStripButton10.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton10.Text = "toolStripButton10";
            // 
            // toolStripButton11
            // 
            this.toolStripButton11.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton11.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton11.Image")));
            this.toolStripButton11.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton11.Name = "toolStripButton11";
            this.toolStripButton11.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton11.Text = "toolStripButton11";
            // 
            // toolStripButton12
            // 
            this.toolStripButton12.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton12.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton12.Image")));
            this.toolStripButton12.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton12.Name = "toolStripButton12";
            this.toolStripButton12.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton12.Text = "toolStripButton12";
            // 
            // toolStripButton13
            // 
            this.toolStripButton13.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton13.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton13.Image")));
            this.toolStripButton13.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton13.Name = "toolStripButton13";
            this.toolStripButton13.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton13.Text = "toolStripButton13";
            // 
            // Panel4Unity
            // 
            // 
            // 
            // 
            this.BottomToolStripPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 175);
            this.BottomToolStripPanel.Name = "";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(150, 0);
            // 
            // 
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 175);
            // 
            // 
            // 
            this.LeftToolStripPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 175);
            // 
            // 
            // 
            this.RightToolStripPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightToolStripPanel.Location = new System.Drawing.Point(150, 0);
            this.RightToolStripPanel.Name = "";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 175);
            // 
            // 
            // 
            this.TopToolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(150, 0);
            this.VisibleChanged += new System.EventHandler(this.Panel4Unity_VisibleChanged);
            this.Enter += new System.EventHandler(this.Panel4Unity_Enter);
            this.Leave += new System.EventHandler(this.Panel4Unity_Leave);
            this.MouseEnter += new System.EventHandler(this.Panel4Unity_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.Panel4Unity_MouseLeave);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Panel4Unity_VisibleChanged(object sender, EventArgs e)
        {
            if( this.Visible == false)
            {
                ClearPOISelection();
                HideAllPOIPopup();
            }
            
        }

        private void Panel4Unity_MouseEnter(object sender, EventArgs e)
        {
            if (this.CanFocus)
            {
                this.Focus();
            }
        }

        private void Panel4Unity_MouseLeave(object sender, EventArgs e)
        {
           
        }

        private void Panel4Unity_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Unity Lost Focus");
        }

        private void Panel4Unity_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Unity Get Focus");
        }


        void Panel4Unity_GotFocus(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Unity Get Focus");
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        
        internal void RequestOutdoor()
        {
            m_Owner.RequestOutdoor();
        }

        public void Open3dModel(string szName)
        {
            string szCmd = string.Format("CMD:OpenModel('{0}')", szName);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void UpdateWindow()
        {}

        public void RemovePOI(float ox, float p, float oz)
        {}

        public void RemovePOI(int nID)
        {}

        public int AddPOI(string szIconPath, float p1, float p2, float p3)
        { return -1; }

        public void SetCheckPoistion(bool mCheckPosition)
        {}

        public int AddPOI(string szIconPath)
        { return -1; }

    }

   

    [StructLayout(LayoutKind.Sequential)]
    internal struct INFO_POI
    {
        internal int bSet;
        internal int nID;
        internal float x;
        internal float y;
        internal float z;
        internal int nx;
        internal int ny;
        internal int bSelect;
    }

    public class Vector3
    {
        public Vector3(float fx, float fy, float fz)
        {
            x = fx;
            y = fy;
            z = fz;
        }

        private float x;
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        private float y;
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        private float z;
        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }

}
