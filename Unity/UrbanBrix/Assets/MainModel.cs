using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Assets;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct INFO_POI
{
    public int bSet;
    public int nID;
    public float x;
    public float y;
    public float z;
    public int nx;
    public int ny;
    public int bSelect;
}

public class MainModel : MonoBehaviour
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
        "SelectEditMode",
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
        "AddReverseLODTextPOIFile",
        "AddIconPOI",
        "AddIconPOIFile",
        "ClearIconPOI",
        "ChangePOIIcon",
        "ChangePOIIconFile",
        "RollBackPOIIcon",
        "ShowTextPOI",
        "ShowIconPOI",
        "ShowIconPOIFile",
        "ShowIconLayer",
        "ShowIconLayers",
        "HideIconLayers",
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
        "SetEarthquake",
        "SelectScene",
        "ShowAlarmZone",
        "HideAlarmZone",
        "HideAllAlarmZones",
        "VisibleViewButton",
        "AddWall",
        "AddDoor",
        "GetWalls",
        "LoadWalls",
        "ChangeWall",
        "SetWallEditMode",
        "SetUseSnap",
        "GetWallInfo",
        "AddSpaceText",
        "ChangeColorSpaceText",
        "ChangeFontSpaceText",
        "GetSpaceTexts",
        "LoadSpaceTexts",
        "SetBlinkMode",
        "SetPoiLod",
        "AddPoiLodValue",
        "ClearPoiLodValue"
    };
    
    public bool m_orbitMode = false;
    public bool m_translateMode = false;
    public bool m_pickMode = false;
    public bool m_selectAlarmZoneMode = false;

    public Camera m_mainCamera = null;
    public Color color = Color.green;
    //public Collider coll;

    public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, ZOOM, ADD_ICON, MOVE_ICON, DELETE_ICON, NOT_USE_MODE, SELECT_ZONE };
    public enum MouseState { None, LButtonDown, LButtonUp, RButtonDown, RButtonUp, MButtonDown, MButtonUp };
    public enum EditModeType { None = 0, AddIcon, MoveIcon, DeleteIcon, PickIcon };

    public const int MOUSE_LEFT = 0;
    public const int MOUSE_RIGHT = 1;
    public const int MOUSE_WHEEL = 2;

    private MouseState m_prevState = MouseState.None;
    private MouseWorkMode m_prevMovingMode = MouseWorkMode.NONE;

    private Vector3 m_vMouseOrigin = new Vector3();
    private Vector3 m_vCameraOrigin = new Vector3();
    private Plane m_plane = null;

    private Vector3 m_vHorzUnit = new Vector3();
    private Vector3 m_vVertUnit = new Vector3();
    private Vector3 m_vOrbitCenter = new Vector3();
    private bool m_isFirstOrbit = true;
    private float m_xAngle = 0.0f;
    private float m_yAngle = 0.0f;

    // Camera가 바라보는 방향으로 타겟까지의 거리
    private float m_fTargetLength;
    public Vector3 m_target;

    // 메인 카메라의 Y좌표가 이 값 아래로 내려가선 안된다.
    public float CameraBottomLimit = -100.0f;

    public bool useExternalOrbitCenter = true;
    public Vector3 ExternalOrbitCenter = new Vector3();
    private Vector3 m_vCurrentPanMoved = new Vector3();
    private Vector3 m_vPanMovedFromFirstPosition = new Vector3();

    public float m_fPanningScale = 1.0f;

    // Key : SceneName
    private static Dictionary<string, UnityScene> m_dicSceneIndoors = new Dictionary<string, UnityScene>();
    // Key : SceneName
    private static Dictionary<string, UnityScene> m_dicSceneOutdoors = new Dictionary<string, UnityScene>();
    // Key : SceneName
    private static Dictionary<string, UnityScene> m_dicSceneBuildings = new Dictionary<string, UnityScene>();
    private UnityScene m_currentScene = null;
    
    private GameObject m_bottomObject = null;
    private bool m_rotatePOI = false;
    
    private MouseWorkMode m_nMode = MouseWorkMode.NONE;
    private CameraData m_initData = null;
    private bool m_isOutdoorView = true;
    public bool IsOutdoorView
    {
        get { return m_isOutdoorView; }
    }

    #region Blink
    private ConcurrentDictionary<GameObject, GameObject> m_dicActiveAlarmZones = new ConcurrentDictionary<GameObject, GameObject>();

    private bool m_hideAlarm = true;
    private float m_fAlarmZoneTime = 0.0f;
    private int m_nAlarmZoneData = 1;

    private bool m_blinkMode = true;
    #endregion

    #region LOD
    private List<SDMS.PoiLod> m_lods = new List<SDMS.PoiLod>();
    private SDMS.PoiLod m_currentPoiLod = null;
    // Key : POI Type
    // Value : LOD 사용여부
    private Dictionary<string, bool> m_dicUsePoiLOD = new Dictionary<string, bool>();
    #endregion

    #region Orthographic
    private int m_nInitOrthoSize = 25;
    private int m_nOrthoMin = 5;
    private int m_nOrthoMax = 150;
    #endregion

    private GameObject m_objPoiHiddenCount = null;

    public UnityScene CurrentScene
    {
        get { return m_currentScene; }
    }

    public MouseWorkMode Mode
    {
        get { return m_nMode; }
        set { m_nMode = value; }
    }

    public bool RotatePOI
    {
        get { return m_rotatePOI; }
    }

    public EditModeType EditType
    {
        get { return m_editModeType; }
    }

    private SharedMemory.BufferReadWrite file = null;
    //private static StreamWriter m_writer = new StreamWriter("c:/temp/MainModel.txt", false, System.Text.Encoding.UTF8);

    public static void WriteLog(string strLog)
    {
        //DateTime dtNow = DateTime.Now;
        //string strTime = string.Format("[{0:00}:{1:00}:{2:00}] : ", dtNow.Hour, dtNow.Minute, dtNow.Second);
        //m_writer.WriteLine(strTime + strLog);
        //m_writer.Flush();
    }

    private EditModeType m_editModeType = EditModeType.None;
    private string m_strTargetEditIconType = "";
    private int m_nNewIconID = -1;

    private bool m_bEditMode = false;
    public bool EditMode
    {
        get { return m_bEditMode; }
        set { SetEditMode(value); }
    }

    // 가벽 편집모드인가?
    private bool m_bWallEditMode = false;
    public bool WallEditMode
    {
        get { return m_bWallEditMode; }
        set { m_bWallEditMode = value; }
    }

    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            proxy.UserObject.SetVariable("SetEditMode", new Action<bool>(SetEditMode));
            proxy.UserObject.SetVariable("SetWallEditMode", new Action<bool>(SetWallEditMode));
            proxy.UserObject.SetVariable("SelectEditMode", new Action<string>(SelectEditMode));
            proxy.UserObject.SetVariable("SetMode", new Action<int, bool>(SetMode));
            proxy.UserObject.SetVariable("Get3DPosition", new Action<int, int>(Get3DPosition));
            proxy.UserObject.SetVariable("UpdateAliasNames", new Action(UpdateAliasName));
            proxy.UserObject.SetVariable("ClearAllSelect", new Action(ClearSelect));
            proxy.UserObject.SetVariable("Get2DPosition", new Action<int, float, float, float>(Get2DPosition));

            proxy.UserObject.SetVariable("ModelZoom", new Action<float>(ManualZoom));
            proxy.UserObject.SetVariable("SetZoomObject", new Action<string>(SetZoomObject));

            proxy.UserObject.SetVariable("SaveScreenShot", new Action<string>(SaveScreenShot));

            proxy.UserObject.SetVariable("ShowEvacCircle", new Action<int>(ShowEvacCircle));
            proxy.UserObject.SetVariable("SetEvacCircleCenter", new Action<float, float, float>(SetEvacCircleCenter));
            proxy.UserObject.SetVariable("SetInitEvacDistance", new Action<int>(SetInitEvacDistance));
            proxy.UserObject.SetVariable("SetSecondEvacDistance", new Action<int>(SetSecondEvacDistance));
            proxy.UserObject.SetVariable("SendMessage", new Action<string>(SendServer));

            proxy.UserObject.SetVariable("SelectScene", new Action<string>(SelectScene));
            proxy.UserObject.SetVariable("ShowAlarmZone", new Action<string, bool>(ShowAlarmZone));
            proxy.UserObject.SetVariable("HideAlarmZone", new Action<string>(HideAlarmZone));
            proxy.UserObject.SetVariable("HideAllAlarmZones", new Action(HideAllAlarmZones));
            proxy.UserObject.SetVariable("VisibleViewButton", new Action<string, bool>(VisibleViewButton));

            proxy.UserObject.SetVariable("AddWall", new Action(AddWall));
            proxy.UserObject.SetVariable("AddDoor", new Action(AddDoor));
            proxy.UserObject.SetVariable("GetWalls", new Action<string>(GetWalls));
            proxy.UserObject.SetVariable("LoadWalls", new Action<string, string>(LoadWalls));
            proxy.UserObject.SetVariable("SetUseSnap", new Action<bool>(SetUseSnap));
            proxy.UserObject.SetVariable("GetWallInfo", new Action<float, float, float, float>(GetWallInfo));

            proxy.UserObject.SetVariable("AddSpaceText", new Action<string>(AddSpaceText));
            proxy.UserObject.SetVariable("ChangeColorSpaceText", new Action<string>(ChangeColorSpaceText));
            proxy.UserObject.SetVariable("ChangeFontSpaceText", new Action<string, float, int>(ChangeFontSpaceText));
            proxy.UserObject.SetVariable("GetSpaceTexts", new Action<string>(GetSpaceTexts));
            proxy.UserObject.SetVariable("LoadSpaceTexts", new Action<string, string>(LoadSpaceTexts));

            proxy.UserObject.SetVariable("SetBlinkMode", new Action<bool>(SetBlinkMode));
            proxy.UserObject.SetVariable("SetPoiLod", new Action<string, bool>(SetPoiLod));
            proxy.UserObject.SetVariable("AddPoiLodValue", new Action<float, float, float>(AddPoiLodValue));
            proxy.UserObject.SetVariable("ClearPoiLodValue", new Action(ClearPoiLodValue));
        }
    }

    public void SendServer(string szMsg)
    {
        PassivePipeProxy.Instance.SendServer(szMsg);
    }

    public void SetEvacCircleCenter(float x, float y, float z)
    {
        foreach (Transform child in transform)
        {
            // MeshFilter mf = child.GetComponent<MeshFilter>();
            //  if (mf != null)
            //  {
            EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
            if (sm != null)
            {
                Debug.unityLogger.Log("SetEvacCircleCenter");

                float dy = child.position.y;
                child.position = new Vector3(x, dy, z);
            }
            //  }
        }

        SaveSharedFile("SetEvacCircleCenter");
    }

    public void ShowEvacCircle(int nLevel)
    {
        foreach (Transform child in transform)
        {
            //MeshFilter mf = child.GetComponent<MeshFilter>();
            //if (mf != null)
            //{
            EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
            if (sm != null)
            {
                if (sm.m_nID <= nLevel)
                {
                    sm.m_Visible = true;
                }
                else
                {
                    sm.m_Visible = false;
                }
                Debug.unityLogger.Log("ShowEvacCircle");

            }
            // }
        }

        SaveSharedFile("ShowEvacCircle");
    }

    public void SetInitEvacDistance(int nDistance)
    {
        foreach (Transform child in transform)
        {
            //MeshFilter mf = child.GetComponent<MeshFilter>();
            //if (mf != null)
            {
                EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
                if (sm != null)
                {
                    if (sm.m_nID == 1)
                    {
                        sm.SetRaious(nDistance);
                        SaveSharedFile("SetInitEvacDistance");
                        Debug.unityLogger.Log("SetInitEvacDistance");
                        break;
                    }
                }
            }
        }
    }

    public void SetSecondEvacDistance(int nDistance)
    {
        foreach (Transform child in transform)
        {
            // MeshFilter mf = child.GetComponent<MeshFilter>();
            // if (mf != null)
            {
                EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
                if (sm != null)
                {
                    if (sm.m_nID == 2)
                    {
                        sm.SetRaious(nDistance);
                        SaveSharedFile("SetSecondEvacDistance");
                        Debug.unityLogger.Log("SetSecondEvacDistance");
                        break;
                    }
                }
            }
        }
    }

    public void SetWallEditMode(bool bEdidtMode)
    {
        if (!m_bEditMode) // 편집 모드일때만 가벽 편집이 가능함
            return;
        
        if (m_bWallEditMode == bEdidtMode)
            return;
        
        m_bWallEditMode = bEdidtMode;
        CustomizingController.Instance.SetWallColor(); // 가벽 편집 모드일때 가벽 색상 바꾸기
        CustomSpaceText.Instance.SetSpaceTextColor();
    }

    public void SetBlinkMode(bool blinkMode)
    {
        m_blinkMode = blinkMode;
    }

    public void SetEditMode(bool bEditMode)
    {
        if (m_bEditMode == bEditMode)
            return;
        
        m_bEditMode = bEditMode;
        
        foreach (SDMS.PoiLod lod in m_lods)
        {
            lod.InitializeAll();
        }

        if (m_bEditMode)
        {
            EdgeDetect edgeDetect = m_mainCamera.GetComponent<EdgeDetect>();
            if (edgeDetect != null)
            {
                edgeDetect.enabled = false;
                //edgeDetect.gameObject.SetActive(false); 
            }

            m_mainCamera.orthographic = true;
            m_mainCamera.orthographicSize = m_nInitOrthoSize;
            m_editModeType = EditModeType.None;
            m_strTargetEditIconType = "";
            m_nNewIconID = -1;

            m_rotatePOI = true;

            // 텍스트모드를 off해도 편집모드에서는 보여야함
            if (!CustomSpaceText.bShow)
                CustomSpaceText.Instance.VisibleSpaceText(true);
        }
        else
        {
            EdgeDetect edgeDetect = m_mainCamera.GetComponent<EdgeDetect>();
            if (edgeDetect != null)
            {
                edgeDetect.enabled = true;
                //edgeDetect.gameObject.SetActive(true);
            }

            m_mainCamera.orthographic = false;
            m_rotatePOI = false;
            m_bWallEditMode = false;
            
            CustomizingController.Instance.ClearSelectedWall(); // 편집 모드가 종료될 때 선택되있는 가벽이 있으면 해제하기
            CustomizingController.Instance.SetWallColor(); // 가벽 편집 모드일때 가벽 색상 바꾸기
            CustomizingController.Instance.ResetChagne();

            CustomDoorSH.Instance.ClearSelectedDoor(); // 편집 모드가 종료될 때 선택되있는 출입문이 있으면 해제하기
            CustomDoorSH.Instance.ResetChagne();

            // 텍스트모드가 off였으면 다시 off해놓는다
            if (!CustomSpaceText.bShow)
                CustomSpaceText.Instance.VisibleSpaceText(false);

            CustomSpaceText.Instance.ClearSelectedText();
            CustomSpaceText.Instance.SetSpaceTextColor();
            SelectScene(this.m_currentScene.SceneName);
        }
        
        foreach (KeyValuePair<UnityEngine.UI.Image, bool> item in UIEventSystem.DicViewButtons) // 편집 모드일때 (확대, 축소..등) unity control key 없애기
        {
            string btnName = item.Key.name;
            if (btnName != "btnPanning" && btnName != "btnPoiVisible" && btnName != "btnBroadcast" && btnName != "btnText" && btnName != "btnOrbit" && btnName != "btnSlideLeft" && btnName != "btnHome" && btnName != "btnManualReport"
                && btnName != "btnZoomIn" && btnName != "btnZoomOut")
                continue;
            
            item.Key.enabled = !m_bEditMode;
        }
        
        if (m_currentScene != null)
        {
            if (m_bEditMode && m_currentScene.OrthoCameraData != null)
            {
                SetInitData(m_currentScene.OrthoCameraData);
                /*m_mainCamera.transform.localPosition = m_currentScene.OrthoCameraData.LocalPosition;
                m_mainCamera.transform.localEulerAngles = m_currentScene.OrthoCameraData.LocalEulerAngle;
                m_mainCamera.transform.localScale = m_currentScene.OrthoCameraData.LocalScale;*/
            }
            else
            {
                SetInitData(m_currentScene.CameraData);
                /*m_mainCamera.transform.localPosition = m_currentScene.CameraData.LocalPosition;
                m_mainCamera.transform.localEulerAngles = m_currentScene.CameraData.LocalEulerAngle;
                m_mainCamera.transform.localScale = m_currentScene.CameraData.LocalScale;*/
            }
        }
    }

    private void SelectEditMode(string strEditMode)
    {
        string[] tokens = strEditMode.Split('_');
        string strMode = tokens[0].Trim();

        if (string.Compare(strMode, "AddIcon", true) == 0)
        {
            m_editModeType = EditModeType.AddIcon;

            if (tokens.Length >= 2)
                m_strTargetEditIconType = tokens[1].Trim();
            else
                m_strTargetEditIconType = "";

            if (tokens.Length >= 3)
            {
                if (int.TryParse(tokens[2].Trim(), out m_nNewIconID) == false)
                    m_nNewIconID = -1;
            }
        }
        else if (string.Compare(strMode, "MoveIcon", true) == 0)
        {
            m_editModeType = EditModeType.MoveIcon;
        }
        else if (string.Compare(strMode, "DeleteIcon", true) == 0)
        {
            m_editModeType = EditModeType.DeleteIcon;
        }
        else if (string.Compare(strMode, "PickIcon", true) == 0)
        {
            m_editModeType = EditModeType.PickIcon;
        }
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

    public void SaveSharedFile(string szKey, int bSet, int nID, bool bbb)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = bSet;
            info.nID = nID;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }

    public void SaveSharedFile(string szKey)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }
    public void SaveSharedFile(string szKey, Vector3 vec)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.x = vec.x;
            info.y = vec.y;
            info.z = vec.z;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }

    public void SaveSharedFile(string szKey, float x, float y, float z)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.x = x;
            info.y = y;
            info.z = z;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }
    public void SaveSharedFile(string szKey, int x, int y)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {

            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.nx = x;
            info.ny = y;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }
    public void SaveSharedFile(string szKey, int bSelect)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {

            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.bSelect = bSelect;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }


    void SaveScreenShot(string szPath)
    {
        ScreenCapture.CaptureScreenshot(szPath);
        SaveSharedFile("SaveScreenShot");
    }

    void Get2DPosition(int nTag, float x, float y, float z)
    {
        Vector3 m3DPosition = new Vector3(x, y, z);
        Vector3 m2DPosition = Camera.main.WorldToScreenPoint(m3DPosition);

        SaveSharedFile("Get2DPosition", (int)m2DPosition.x, (int)m2DPosition.y);

        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = string.Format("SendMessage('2DPosition({0}, {1}, {2})')", nTag, (int)m2DPosition.x, (int)m2DPosition.y);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void SetPickMode(bool bFalse)
    {
        m_pickMode = bFalse;
    }
    
    public void Get3DPosition(int x, int y)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            bool bHit = false;
            SortedList arHitPt = new SortedList();

            // Hit가되는 Mesh를 모두 찾는다.
            foreach (Transform child in transform)
            {
                // 모든 gameObject의 MeshFilter를 검사
                MeshFilter mf = child.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    // MeshCollider를 이용하여 HitTest를 수행
                    MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                    RaycastHit hit1;
                    Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                    {
                        // Hit된경우 거리값을 키로 저장한다.
                        arHitPt.Add(hit1.distance, hit1);
                        bHit = true;
                    }
                }
            }

            // Hit된 Mesh가 있다면
            if (bHit == true)
            {
                // 가장 가까운 것을 선택한다.
                RaycastHit vec = (RaycastHit)(arHitPt.GetValueList()[0]);
                m_PickPos = vec.point;

                SaveSharedFile("Get3DPosition", m_PickPos);

                string szMsg = string.Format("SendMessage('3DPosition({0}, {1}, {2})')", m_PickPos.x, m_PickPos.y, m_PickPos.z);
                proxy.RunPythonScript(szMsg);
                return;
            }

            // Mesh Hit가 되지 않았다면 전체 바운드로 사용하는 BoxCollider로 HitTest를 수행한다.
            /*RaycastHit hit2;
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (coll.Raycast(ray2, out hit2, Mathf.Infinity))
            {
                m_PickPos = hit2.point;
                SaveSharedFile("Get3DPosition", m_PickPos);
                string szMsg = string.Format("SendMessage('3DPosition({0}, {1}, {2})')", m_PickPos.x, m_PickPos.y, m_PickPos.z); 
                proxy.RunPythonScript(szMsg);
            }*/
        }
    }

    public Vector3 ScreenToGlobal(int x, int y)
    {
        bool bHit = false;
        SortedList arHitPt = new SortedList();

        // Hit가되는 Mesh를 모두 찾는다.
        foreach (Transform child in transform)
        {
            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(x, y));
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    // Hit된경우 거리값을 키로 저장한다.
                    arHitPt.Add(hit1.distance, hit1);
                    bHit = true;
                }
            }
        }

        // Hit된 Mesh가 있다면
        if (bHit == true)
        {
            // 가장 가까운 것을 선택한다.
            RaycastHit vec = (RaycastHit)(arHitPt.GetValueList()[0]);
            return vec.point;
        }

        // Mesh Hit가 되지 않았다면 전체 바운드로 사용하는 BoxCollider로 HitTest를 수행한다.
        /*RaycastHit hit2;
        Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(x, y));
        if (coll.Raycast(ray2, out hit2, Mathf.Infinity))
        {
            return hit2.point;
        }*/

        return new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    public Vector3 ScreenToWorldForPOI(int x, int y)
    {
        Vector3 v = m_mainCamera.ScreenToWorldPoint(new Vector3(x, y, m_mainCamera.nearClipPlane));
        v.y = CurrentScene.fPoiBottom; // m_mainCamera.transform.localPosition.y - 39.6f;
        return v;
    }

    private void ReadyToRead()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = "SendMessage('ReadyToRead()')";
            proxy.RunPythonScript(szMsg);
        }
    }

    public void SetMode(int nMode, bool bTrue)
    {
        m_nMode = (MouseWorkMode)nMode;

        switch (m_nMode)
        {
            case MouseWorkMode.NONE: // NONE
                m_translateMode = false;
                m_orbitMode = false;
                m_pickMode = false;
                m_selectAlarmZoneMode = false;
                break;
            case MouseWorkMode.PANNING: // Pan Mode
                m_translateMode = true;
                m_orbitMode = false;
                m_pickMode = false;
                m_selectAlarmZoneMode = false;
                break;
            case MouseWorkMode.ORBIT: // Orbit Mode
                m_translateMode = false;
                m_orbitMode = true;
                m_pickMode = false;
                m_selectAlarmZoneMode = false;
                break;
            case MouseWorkMode.PICK: // Pick Mode
                m_pickMode = true;
                m_translateMode = false;
                m_orbitMode = false;
                m_selectAlarmZoneMode = false;
                break;
            case MouseWorkMode.SELECT_ZONE:
                m_selectAlarmZoneMode = true;
                m_pickMode = false;
                m_translateMode = false;
                m_orbitMode = false;
                break;
            default:
                m_translateMode = false;
                m_orbitMode = false;
                m_pickMode = false;
                m_selectAlarmZoneMode = false;
                break;
        }
    }

    private void ClearSelect()
    {
        foreach (Transform child in transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    sm.ClearSelect();
                }
            }
        }

        Debug.unityLogger.Log("End UpdateAliasName");
    }

    private void UpdateAliasName()
    {
        Debug.unityLogger.Log("Begin UpdateAliasName");
        foreach (Transform child in transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    Debug.unityLogger.Log("UpdateAliasName : " + mf.name);
                    sm.UpdateAliasName();
                }
                else
                {
                    Debug.unityLogger.Log("UpdateAliasName fail : " + mf.name);
                }

            }
        }

        Debug.unityLogger.Log("End UpdateAliasName");
    }

    public void UpdateAliasColor()
    {
        foreach (Transform child in transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                if (child.gameObject.name.StartsWith("Evac"))
                    continue;

                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    sm.SetTextColor(ModelManager.Instance.BuildingNameColor);
                }
            }
        }
    }

    private UnityScene SetOutdoorScene(Dictionary<string, UnityScene> dicScenes, Dictionary<string, GameObject> dicCameraDatas, CameraData normalCamera, CameraData orthoCamera, string strModelName, Vector3 vOrbitCenter, bool isMainCamera, bool isActiveModel, float fPanningScale, float fBottomElevation, UnityScene.SceneOption option = UnityScene.SceneOption.None)
    {
        GameObject model = GameObject.Find(strModelName);

        if (model == null)
        {
            Debug.Log(strModelName + " is NULL");
            return null;
        }

        UnityScene scene = new UnityScene();

        if (orthoCamera != null)
        {
            scene.OrthoCameraData = orthoCamera;
        }

        scene.CameraData = normalCamera;

        scene.Model = model;
        model.SetActive(isActiveModel);

        scene.OrbitCenter = vOrbitCenter;
        scene.PanningScale = fPanningScale;
        scene.SceneName = strModelName;
        scene.Option = option;
        scene.BottomElevation = fBottomElevation;

        dicScenes[scene.SceneName] = scene;
        scene.Model.SetActive(false);
        return scene;
    }

    private UnityScene SetOutdoorScene(Dictionary<string, UnityScene> dicScenes, Dictionary<string, GameObject> dicCameraDatas, string strCameraName, string strOrthoCameraName, string strModelName, Vector3 vOrbitCenter, bool isMainCamera, bool isActiveModel, float fPanningScale, float fBottomElevation, UnityScene.SceneOption option = UnityScene.SceneOption.None)
    {
        GameObject camera = GameObject.Find(strCameraName);
        GameObject orthoCamera = GameObject.Find(strOrthoCameraName);
        GameObject model = GameObject.Find(strModelName);        

        if (camera == null)
        {
            if (dicCameraDatas.TryGetValue(strCameraName, out camera) == false)
            {
                Debug.Log(strCameraName + " is null");
                return null;
            }
        }

        if (orthoCamera == null)
        {
            /*if (dicCameraDatas.TryGetValue(strOrthoCameraName, out orthoCamera) == false)
            {
                Debug.Log(strOrthoCameraName + " is null");
                return null;
            }*/
        }

        if (model == null)
        {
            Debug.Log(strModelName + " is NULL");
            return null;
        }

        dicCameraDatas[strCameraName] = camera;

        UnityScene scene = new UnityScene();

        CameraData cameraData = new CameraData();
        cameraData.LocalPosition = camera.transform.localPosition;
        cameraData.LocalEulerAngle = camera.transform.localEulerAngles;
        cameraData.LocalScale = camera.transform.localScale;
        cameraData.CameraName = strCameraName;

        if (orthoCamera != null)
        {
            CameraData orthoCameraData = new CameraData();
            orthoCameraData.LocalPosition = orthoCamera.transform.localPosition;
            orthoCameraData.LocalEulerAngle = orthoCamera.transform.localEulerAngles;
            orthoCameraData.LocalScale = orthoCamera.transform.localScale;
            orthoCameraData.CameraName = strOrthoCameraName;

            Camera _orthoCamera = orthoCamera.GetComponent<Camera>();
            orthoCameraData.OrthoSize = _orthoCamera.orthographicSize;

            scene.OrthoCameraData = orthoCameraData;
            orthoCamera.SetActive(isMainCamera);
        }

        scene.CameraData = cameraData;
        camera.SetActive(isMainCamera);

        scene.Model = model;
        model.SetActive(isActiveModel);

        scene.OrbitCenter = vOrbitCenter;
        scene.PanningScale = fPanningScale;
        scene.SceneName = strModelName;
        scene.Option = option;
        scene.BottomElevation = fBottomElevation;

        dicScenes[scene.SceneName] = scene;
        scene.Model.SetActive(false);
        return scene;
    }

    private UnityScene SetBuildingScene(Dictionary<string, UnityScene> dicScenes, UnityScene outdoorScene, string strModelName, string strRedModelName)
    {
        outdoorScene.Model.SetActive(true);

        GameObject model = GameObject.Find(strModelName);
        GameObject redModel = GameObject.Find(strRedModelName);

        if (model == null)
        {
            Debug.Log(strModelName + " is NULL2");
            return null;
        }

        if (redModel == null)
        {
            Debug.Log(strRedModelName + " is NULL3");
            return null;
        }

        UnityScene scene = new UnityScene();

        CameraData cameraData = new CameraData();
        cameraData.LocalPosition = outdoorScene.CameraData.LocalPosition;
        cameraData.LocalEulerAngle = outdoorScene.CameraData.LocalEulerAngle;
        cameraData.LocalScale = outdoorScene.CameraData.LocalScale;
        cameraData.CameraName = outdoorScene.CameraData.CameraName;

        scene.CameraData = cameraData;

        scene.Model = model;
        scene.RedModel = redModel;
        redModel.SetActive(false);

        scene.OrbitCenter = outdoorScene.OrbitCenter;
        scene.PanningScale = outdoorScene.PanningScale;
        scene.SceneName = strModelName;
        scene.Option = outdoorScene.Option;
        scene.BottomElevation = outdoorScene.BottomElevation;

        dicScenes[scene.SceneName] = scene;

        outdoorScene.Model.SetActive(false);
        return scene;
    }

    private UnityScene SetIndoorScene(Dictionary<string, UnityScene> dicScenes, Dictionary<string, GameObject> dicCameraDatas, object cameraInfo, object orthoCameraInfo, string strModelName, string strAlarmModelName, Vector3 vOrbitCenter, bool isMainCamera, bool isActiveModel, float fPanningScale, float fBottomElevation, float detectBottom, float fPoiBottom, UnityScene otherScene = null, UnityScene.SceneOption option = UnityScene.SceneOption.None)
    {
        UnityScene scene = null;

        if (cameraInfo is string)
            scene = SetOutdoorScene(dicScenes, dicCameraDatas, (string)cameraInfo, (string)orthoCameraInfo, strModelName, vOrbitCenter, isMainCamera, isActiveModel, fPanningScale, fBottomElevation, option);
        else if (cameraInfo is CameraData)
            scene = SetOutdoorScene(dicScenes, dicCameraDatas, (CameraData)cameraInfo, (CameraData)orthoCameraInfo, strModelName, vOrbitCenter, isMainCamera, isActiveModel, fPanningScale, fBottomElevation, option);

        if (scene != null && strAlarmModelName != null && strAlarmModelName.Length > 0)
        {
            scene.ActiveAlarmZones = m_dicActiveAlarmZones;
            GameObject alarmModel = GameObject.Find(strAlarmModelName);

            if (alarmModel != null)
            {
                for (int i = 0; i < alarmModel.transform.childCount; i++)
                {
                    GameObject child = alarmModel.transform.GetChild(i).gameObject;

                    if (child == null)
                        continue;

                    AddSceneChild(scene, child);
                    /*child.SetActive(false);
                    scene.AddAlarmZone(child, child.name);*/
                }
            }

            if (otherScene != null)
                scene.OtherScenes.Add(otherScene);
        }

        if(scene!=null)
        {
            scene.DBottomHeight = detectBottom;
            scene.fPoiBottom = fPoiBottom;
        }

        return scene;
    }

    private void AddSceneChild(UnityScene scene, GameObject obj)
    {
        int nChildCount = obj.transform.childCount;

        if (nChildCount == 0)
        {
            MeshCollider collider = obj.AddComponent<MeshCollider>();

            obj.SetActive(false);
            scene.AddAlarmZone(obj, obj.name);
        }
        else
        {
            for (int i = 0; i < nChildCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;

                if (child == null)
                    continue;

                AddSceneChild(scene, child);
            }
        }
    }

    public void SelectScene(string strSceneName)
    {
        WriteLog("SelectScene : " + strSceneName);
        UnityScene scene;

        if (m_dicSceneOutdoors.TryGetValue(strSceneName, out scene))
            ChangeScene(scene, true);
        else if (m_dicSceneIndoors.TryGetValue(strSceneName, out scene))
            ChangeScene(scene, false);
        else if (m_dicSceneBuildings.TryGetValue(strSceneName, out scene))
            ChangeBuildingScene(scene);
    }

    public void ShowAlarmZone(string strZoneName, bool hideAllOthers)
    {
        if (m_dicSceneBuildings.ContainsKey(strZoneName))
        {
            SelectScene(strZoneName);
        }
        else
        {
            if (m_currentScene != null)
            {
                if (strZoneName.Contains("\t"))
                {
                    List<string> zoneNames = GetNameList(strZoneName);
                    int nZoneCount = zoneNames.Count;

                    if (nZoneCount == 0)
                        return;

                    m_currentScene.ShowAlarmZone(zoneNames[0], hideAllOthers);

                    for (int i = 1; i < nZoneCount; i++)
                    {
                        m_currentScene.ShowAlarmZone(zoneNames[i], false);
                    }
                }
                else
                    m_currentScene.ShowAlarmZone(strZoneName, hideAllOthers);
            }
        }
    }

    private List<string> GetNameList(string strZoneName)
    {
        List<string> names = new List<string>();
        string[] tokens = strZoneName.Split('\t');

        foreach (string strToken in tokens)
        {
            names.Add(strToken.Trim());
        }

        return names;
    }

    public void HideAlarmZone(string strZoneName)
    {
        if (m_currentScene != null)
            m_currentScene.HideAlarmZone(strZoneName);
    }

    public void HideAllAlarmZones()
    {
        if (m_currentScene != null)
            m_currentScene.HideAllAlarmZones();
    }

    public void VisibleViewButton(string btnName, bool visible)
    {
        foreach (KeyValuePair<UnityEngine.UI.Image, bool> item in UIEventSystem.DicViewButtons)
        {
            if (item.Key.name == btnName)
            {
                item.Key.enabled = visible;
                break;
            }
        }
    }

    private void ChangeScene(UnityScene scene, bool isOutdoor)
    {
        //if (m_currentScene == scene)
        //    return;
        HideBuildingRed();

        //if (isOutdoor)
        //{
        //    GameObject trees = GameObject.Find("Trees");
        //    //GameObject trees = GameObject.FindGameObjectWithTag("outdoorTrees");

        //    if (trees != null)
        //    {
        //        trees.SetActive(true);

        //        //UnityEngine.Cubemap cubemap = trees.GetComponent<UnityEngine.Cubemap>();
        //    }

        //    GameObject trees2 = GameObject.Find("Maple 2");
        //    if (trees2 != null)
        //    {
        //        trees2.SetActive(true);
        //    }
        //}

        if (scene != null)
        {
            if (m_bEditMode && isOutdoor == false)
            {
                // 200f : 모델 하나 안나와서 젤 높은값으로 줌
                //m_mainCamera.transform.localPosition = new Vector3(scene.OrthoCameraData.LocalPosition.x, 200f, scene.OrthoCameraData.LocalPosition.z);
                m_mainCamera.transform.localPosition = new Vector3(scene.OrthoCameraData.LocalPosition.x, scene.OrthoCameraData.LocalPosition.y, scene.OrthoCameraData.LocalPosition.z);
                m_mainCamera.transform.localEulerAngles = scene.OrthoCameraData.LocalEulerAngle;
                m_mainCamera.transform.localScale = scene.OrthoCameraData.LocalScale;
                m_mainCamera.orthographicSize = scene.OrthoCameraData.OrthoSize;
            }
            else
            {
                m_mainCamera.transform.localPosition = scene.CameraData.LocalPosition;
                m_mainCamera.transform.localEulerAngles = scene.CameraData.LocalEulerAngle;
                m_mainCamera.transform.localScale = scene.CameraData.LocalScale;
            }
        }

        if (m_currentScene != null)
        {
            m_currentScene.Model.SetActive(false);

            foreach (UnityScene otherScene in m_currentScene.OtherScenes)
            {
                otherScene.Model.SetActive(false);
            }
        }

        EdgeDetect edgeDetect = m_mainCamera.GetComponent<EdgeDetect>();
        if (edgeDetect != null)
        {
            if (isOutdoor)
            {
                edgeDetect.enabled = true; 
            }
            else
            {
                edgeDetect.enabled = false;
            }
        } 

        m_currentScene = scene;

        if (m_currentScene != null)
        {
            m_currentScene.Model.SetActive(true);
            ExternalOrbitCenter = m_currentScene.OrbitCenter;
            m_fPanningScale = m_currentScene.PanningScale;

            SetBottomElevation();

            foreach (UnityScene otherScene in m_currentScene.OtherScenes)
            {
                otherScene.Model.SetActive(true);
            }
        }

        if (isOutdoor)
            m_mainCamera.clearFlags = CameraClearFlags.Skybox;
        else
            m_mainCamera.clearFlags = CameraClearFlags.SolidColor;

        m_isOutdoorView = isOutdoor;

        m_vPanMovedFromFirstPosition = Vector3.zero;
        //m_rotatePOI = true;

        if (m_bEditMode == false)
        {
            // 카메라 초기화를 위하여 살짝 회전시킨다.
            SetInitData();
        }

        m_fZoomLevel = 0.0f;

        InitAllPOILod();

        if (m_isOutdoorView)
            SetHiddenPOICount();
    }

    private void ChangeBuildingScene(UnityScene scene)
    {
        if (m_currentScene != null)
        {
            m_currentScene.Model.SetActive(false);

            foreach (UnityScene otherScene in m_currentScene.OtherScenes)
            {
                otherScene.Model.SetActive(false);
            }
        }

        foreach (KeyValuePair<string, UnityScene> pair in m_dicSceneOutdoors)
        {
            pair.Value.Model.SetActive(true);
            m_currentScene = pair.Value;
        }

        foreach (KeyValuePair<string, UnityScene> pair in m_dicSceneBuildings)
        {
            if (pair.Value == scene)
            {
                if (pair.Value.RedModel != null)
                    pair.Value.RedModel.SetActive(true);

                if (pair.Value.Model != null)
                    pair.Value.Model.SetActive(false);
            }
            else
            {
                if (pair.Value.RedModel != null)
                    pair.Value.RedModel.SetActive(false);

                if (pair.Value.Model != null)
                    pair.Value.Model.SetActive(true);
            }
        }

        if (m_currentScene != null)
        {
            ExternalOrbitCenter = m_currentScene.OrbitCenter;
            m_fPanningScale = m_currentScene.PanningScale;

            SetBottomElevation();

            foreach (UnityScene otherScene in m_currentScene.OtherScenes)
            {
                otherScene.Model.SetActive(true);
            }
        }

        m_mainCamera.transform.localPosition = m_currentScene.CameraData.LocalPosition;
        m_mainCamera.transform.localEulerAngles = m_currentScene.CameraData.LocalEulerAngle;
        m_mainCamera.transform.localScale = m_currentScene.CameraData.LocalScale;
        m_mainCamera.clearFlags = CameraClearFlags.Skybox;

        m_isOutdoorView = true;

        m_vPanMovedFromFirstPosition = Vector3.zero;
        
        // 카메라 초기화를 위하여 살짝 회전시킨다.
        SetInitData();

        m_fZoomLevel = 0.0f;

        InitAllPOILod();

        if (m_isOutdoorView)
            SetHiddenPOICount();
    }

    private void HideBuildingRed()
    {
        foreach (KeyValuePair<string, UnityScene> pair in m_dicSceneOutdoors)
        {
            pair.Value.Model.SetActive(false);
        }

        foreach (KeyValuePair<string, UnityScene> pair in m_dicSceneBuildings)
        {
            if (pair.Value.RedModel != null)
                pair.Value.RedModel.SetActive(false);

            if (pair.Value.Model != null)
                pair.Value.Model.SetActive(true);
        }
    }

    private void SetInitData()
    {
        CameraData initData = new CameraData();
        initData.CameraName = "0";
        initData.LocalPosition = m_mainCamera.transform.localPosition;
        initData.LocalEulerAngle = m_mainCamera.transform.localEulerAngles;
        initData.LocalScale = m_mainCamera.transform.localScale;
        m_initData = initData;

        Vector3 vOrigin = new Vector3(100, 100);

        OrbitMode(MouseState.RButtonDown);
        m_vMouseOrigin = vOrigin;
        _Orbit(new Vector3(vOrigin.x + 0.1f, vOrigin.y));

        NoneMode();
    }

    private void SetInitData(CameraData data)
    {
        CameraData initData = new CameraData();
        initData.CameraName = "0";
        initData.LocalPosition = data.LocalPosition;
        initData.LocalEulerAngle = data.LocalEulerAngle;
        initData.LocalScale = data.LocalScale;
        initData.OrthoSize = data.OrthoSize;

        m_initData = initData;
    }

    private void SetBottomElevation()
    {
        if (m_currentScene == null || m_bottomObject == null)
            return;

        float fElevation = m_currentScene.BottomElevation;

        foreach (UnityScene scene in m_currentScene.OtherScenes)
        {
            if (scene.BottomElevation < fElevation)
                fElevation = scene.BottomElevation;
        }

        m_bottomObject.transform.localPosition = new Vector3(m_bottomObject.transform.localPosition.x, fElevation, m_bottomObject.transform.localPosition.z);
    }

    private void Awake()
    {
        MakeCommandMap();

        if (m_mainCamera == null)
            m_mainCamera = Camera.main;

        InitText();

        Dictionary<string, GameObject> dicCameraDatas = new Dictionary<string, GameObject>();

        m_bottomObject = GameObject.Find("Plane");

        /*m_lods.Add(new SDMS.PoiLod(this, -1000000, -5, 140));
        m_lods.Add(new SDMS.PoiLod(this, -5, -2, 110));
        m_lods.Add(new SDMS.PoiLod(this, -2, 0, 80));
        m_lods.Add(new SDMS.PoiLod(this, 0, 2, 50));
        m_lods.Add(new SDMS.PoiLod(this, 2, 1000000, 0));

        m_dicUsePoiLOD["CCTV"] = true;*/

        UnityScene outdoor = SetOutdoorScene(m_dicSceneOutdoors, dicCameraDatas, "Outdoor_Camera", "", "Outdoor", new Vector3(0, 0, 0), false, false, 0.5f, 0.0f);
        /*UnityScene outdoor = SetOutdoorScene(m_dicSceneOutdoors, dicCameraDatas, "Outdoor_Camera", "", "out-ab-building", new Vector3(0, 0, 0), false, false, 0.5f, 0.0f);
        UnityScene outdoorGround = SetOutdoorScene(m_dicSceneOutdoors, dicCameraDatas, "Outdoor_Camera", "", "out-ab-map", new Vector3(0, 0, 0), false, false, 0.5f, 0.0f);*/

        SetBuildingScene(m_dicSceneBuildings, outdoor, "common-ab", "common-ab-0");
        SetBuildingScene(m_dicSceneBuildings, outdoor, "Hotel", "Hotel-0");
        SetBuildingScene(m_dicSceneBuildings, outdoor, "Office-a", "Office-a-0");
        SetBuildingScene(m_dicSceneBuildings, outdoor, "Office-b", "Office-b-0");

        #region Hotel
        UnityScene hotel6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_6F", "Camera_Hotel_6F_Ortho", "Hotel-6f", "", new Vector3(-713, -0.6f, 34), false, true, 0.5f, -5f, -2.1f, 5f);
        UnityScene hotel7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_7F", "Camera_Hotel_7F_Ortho", "Hotel-7f", "", new Vector3(-713, 3.05f, 34), false, true, 0.5f, 3.05f, 4.6f, 9.3f);
        UnityScene hotelPIT = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_PIT", "Camera_Hotel_PIT_Ortho", "Hotel-PIT", "", new Vector3(-713, 3.05f, 34), false, true, 0.5f, 5.0f, 9.4f, 13.3f);
        UnityScene hotel8 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_8F", "Camera_Hotel_8F_Ortho", "Hotel-8f", "", new Vector3(-713, 6.7f, 34), false, true, 0.5f, 6.7f, 9.4f, 13.3f);
        UnityScene hotel9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_9F", "Camera_Hotel_9F_Ortho", "Hotel-9f", "", new Vector3(-713, 10.35f, 34), false, true, 0.5f, 10.35f, 13.8f, 17f);
        UnityScene hotel10 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_10F", "Camera_Hotel_10F_Ortho", "Hotel-10f", "", new Vector3(-713, 14f, 34), false, true, 0.5f, 14f, 17.2f, 20.3f);
        UnityScene hotel11 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_11F", "Camera_Hotel_11F_Ortho", "Hotel-11f", "", new Vector3(-713, 17.65f, 34), false, true, 0.5f, 17.65f, 20.5f, 23.6f);
        UnityScene hotel12 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_12F", "Camera_Hotel_12F_Ortho", "Hotel-12f", "", new Vector3(-713, 21.3f, 34), false, true, 0.5f, 21.3f, 23.9f, 26.9f);
        UnityScene hotel13 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_13F", "Camera_Hotel_13F_Ortho", "Hotel-13f", "", new Vector3(-713, 24.95f, 34), false, true, 0.5f, 24.95f, 27.2f, 30.3f);
        UnityScene hotel14 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_14F", "Camera_Hotel_14F_Ortho", "Hotel-14f", "", new Vector3(-713, 28.6f, 34), false, true, 0.5f, 28.6f, 30.6f, 33.7f);
        UnityScene hotel15 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_15F", "Camera_Hotel_15F_Ortho", "Hotel-15f", "", new Vector3(-713, 32.25f, 34), false, true, 0.5f, 32.25f, 33.9f, 37.1f);
        UnityScene hotel16 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_16F", "Camera_Hotel_16F_Ortho", "Hotel-16f", "", new Vector3(-713, 35.9f, 34), false, true, 0.5f, 35.9f, 37.2f, 40.5f);
        UnityScene hotel17 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_17F", "Camera_Hotel_17F_Ortho", "Hotel-17f", "", new Vector3(-713, 39.55f, 34), false, true, 0.5f, 39.55f, 40.6f, 43.6f);
        UnityScene hotel18 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_18F", "Camera_Hotel_18F_Ortho", "Hotel-18f", "", new Vector3(-713, 43.2f, 34), false, true, 0.5f, 43.2f, 44f, 47f);
        UnityScene hotel19 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_19F", "Camera_Hotel_19F_Ortho", "Hotel-19f", "", new Vector3(-713, 46.85f, 34), false, true, 0.5f, 46.5f, 47.3f, 50.4f);
        UnityScene hotel20 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_20F", "Camera_Hotel_20F_Ortho", "Hotel-20f", "", new Vector3(-713, 50.5f, 34), false, true, 0.5f, 50.15f, 50.6f, 53.8f);
        UnityScene hotel21 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_21F", "Camera_Hotel_21F_Ortho", "Hotel-21f", "", new Vector3(-713, 54.15f, 34), false, true, 0.5f, 53.15f, 54f, 57.2f);
        UnityScene hotel22 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_22F", "Camera_Hotel_22F_Ortho", "Hotel-22f", "", new Vector3(-713, 57.8f, 34), false, true, 0.5f, 56.15f, 57.3f, 60.6f);
        UnityScene hotel23 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_23F", "Camera_Hotel_23F_Ortho", "Hotel-23f", "", new Vector3(-713, 61.45f, 34), false, true, 0.5f, 59.15f, 60.7f, 64f);
        UnityScene hotel24 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_24F", "Camera_Hotel_24F_Ortho", "Hotel-24f", "", new Vector3(-713, 65.1f, 34), false, true, 0.5f, 62.15f, 64f, 67.4f);
        UnityScene hotel25 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_25F", "Camera_Hotel_25F_Ortho", "Hotel-25f", "", new Vector3(-713, 68.75f, 34), false, true, 0.5f, 65.15f, 67.3f, 70.7f);
        UnityScene hotel26 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_26F", "Camera_Hotel_26F_Ortho", "Hotel-26f", "", new Vector3(-713, 72.4f, 34), false, true, 0.5f, 68.15f, 70.8f, 74f);
        UnityScene hotel27 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_27F", "Camera_Hotel_27F_Ortho", "Hotel-27f", "", new Vector3(-713, 76.05f, 34), false, true, 0.5f, 71.15f, 74.1f, 77f);
        UnityScene hotelPH1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_PH1", "Camera_Hotel_PH1_Ortho", "Hotel-PH-1", "", new Vector3(-713, 76.05f, 34), false, true, 0.5f, 74.15f, 77.2f, 80.5f);
        UnityScene hotelPH2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_PH2", "Camera_Hotel_PH2_Ortho", "Hotel-PH-2", "", new Vector3(-713, 76.05f, 34), false, true, 0.5f, 77.15f, 80.9f, 83.9f);
        UnityScene hotelPH3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_PH3", "Camera_Hotel_PH3_Ortho", "Hotel-PH-3", "", new Vector3(-713, 76.05f, 34), false, true, 0.5f, 80.15f, 84f, 87f);

        #endregion

        #region OfficeA
        UnityScene officeA5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_5F", "Camera_OfficeA_5F_Ortho", "Office-a-5f", "", new Vector3(-771, -10f, 37), false, true, 0.5f, -10f, -6.2f, -1.7f);
        UnityScene officeA6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_6F", "Camera_OfficeA_6F_Ortho", "Office-a-6f", "", new Vector3(-771, -4.41f, 37), false, true, 0.5f, -4.41f, -1.4f, 3.2f);
        UnityScene officeA7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_7F", "Camera_OfficeA_7F_Ortho", "Office-a-7f", "", new Vector3(-771, 0.38f, 37), false, true, 0.5f, 0.38f, 3.0f, 7.6f);
        UnityScene officeA8 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_8F", "Camera_OfficeA_8F_Ortho", "Office-a-8f", "", new Vector3(-771, 5.17f, 37), false, true, 0.5f, 5.17f, 7.6f, 12f);
        UnityScene officeA9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_9F", "Camera_OfficeA_9F_Ortho", "Office-a-9f", "", new Vector3(-771, 9.96f, 37), false, true, 0.5f, 9.96f, 12f, 16.4f);
        UnityScene officeA10 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_10F", "Camera_OfficeA_10F_Ortho", "Office-a-10f", "", new Vector3(-771, 14.75f, 37), false, true, 0.5f, 14.75f, 16.4f, 20.8f);
        UnityScene officeA11 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_11F", "Camera_OfficeA_11F_Ortho", "Office-a-11f", "", new Vector3(-771, 19.54f, 37), false, true, 0.5f, 19.54f, 20.8f, 25.2f);
        UnityScene officeA12 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_12F", "Camera_OfficeA_12F_Ortho", "Office-a-12f", "", new Vector3(-771, 24.33f, 37), false, true, 0.5f, 24.33f, 25.2f, 29.6f);
        UnityScene officeA13 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_13F", "Camera_OfficeA_13F_Ortho", "Office-a-13f", "", new Vector3(-771, 29.12f, 37), false, true, 0.5f, 29.12f, 29.6f, 34f);
        UnityScene officeA14 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_14F", "Camera_OfficeA_14F_Ortho", "Office-a-14f", "", new Vector3(-771, 33.91f, 37), false, true, 0.5f, 33f, 34f, 38.4f);
        UnityScene officeA15 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_15F", "Camera_OfficeA_15F_Ortho", "Office-a-15f", "", new Vector3(-771, 38.7f, 37), false, true, 0.5f, 37.79f, 38.4f, 42.8f);
        UnityScene officeA16 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_16F", "Camera_OfficeA_16F_Ortho", "Office-a-16f", "", new Vector3(-771, 43.49f, 37), false, true, 0.5f, 41f, 42.8f, 47.2f);
        UnityScene officeA17 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_17F", "Camera_OfficeA_17F_Ortho", "Office-a-17f", "", new Vector3(-771, 48.28f, 37), false, true, 0.5f, 46f, 47.2f, 51.6f);
        UnityScene officeA18 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_18F", "Camera_OfficeA_18F_Ortho", "Office-a-18f", "", new Vector3(-771, 53.07f, 37), false, true, 0.5f, 51.42f, 51.6f, 56f);
        UnityScene officeA19 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_19F", "Camera_OfficeA_19F_Ortho", "Office-a-19f", "", new Vector3(-771, 57.86f, 37), false, true, 0.5f, 55.84f, 56f, 60.4f);
        UnityScene officeA20 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_20F", "Camera_OfficeA_20F_Ortho", "Office-a-20f", "", new Vector3(-771, 62.65f, 37), false, true, 0.5f, 60.26f, 60.4f, 64.8f);
        UnityScene officeA21 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_21F", "Camera_OfficeA_21F_Ortho", "Office-a-21f", "", new Vector3(-771, 67.44f, 37), false, true, 0.5f, 64.68f, 64.8f, 69.2f);
        UnityScene officeA22 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_22F", "Camera_OfficeA_22F_Ortho", "Office-a-22f", "", new Vector3(-771, 72.23f, 37), false, true, 0.5f, 69.1f, 69.2f, 73.6f);
        UnityScene officeAPH1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_PH1", "Camera_OfficeA_PH1_Ortho", "Office-a-PH-1", "", new Vector3(-771, 72.23f, 37), false, true, 0.5f, 73.52f, 73.6f, 78f);
        UnityScene officeAPH2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_PH2", "Camera_OfficeA_PH2_Ortho", "Office-a-PH-2", "", new Vector3(-771, 72.23f, 37), false, true, 0.5f, 73.52f, 77.3f, 80f);
        UnityScene officeAPH3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_PH3", "Camera_OfficeA_PH3_Ortho", "Office-a-PH-3", "", new Vector3(-771, 72.23f, 37), false, true, 0.5f, 73.52f, 80.3f, 84f);
        #endregion

        #region OfficeB
        UnityScene officeBPIT = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_PIT", "Camera_OfficeB_PIT_Ortho", "Office-b-PIT", "", new Vector3(-532, 40.00f, 22), false, true, 0.5f, 18.00f, 27.30f, 30.35f);
        UnityScene officeB5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_5F", "Camera_OfficeB_5F_Ortho", "Office-b-5f", "", new Vector3(-532, 44.00f, 22), false, true, 0.5f, 22.00f, 33.40f, 37.98f);
        UnityScene officeB6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_6F", "Camera_OfficeB_6F_Ortho", "Office-b-6f", "", new Vector3(-532, 48.00f, 22), false, true, 0.5f, 26.00f, 37.75f, 42.28f);
        UnityScene officeB7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_7F", "Camera_OfficeB_7F_Ortho", "Office-b-7f", "", new Vector3(-532, 52.00f, 22), false, true, 0.5f, 30.00f, 41.98f, 46.38f);
        UnityScene officeB8 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_8F", "Camera_OfficeB_8F_Ortho", "Office-b-8f", "", new Vector3(-532, 56.00f, 22), false, true, 0.5f, 34.00f, 46.28f, 50.88f);
        UnityScene officeB9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_9F", "Camera_OfficeB_9F_Ortho", "Office-b-9f", "", new Vector3(-532, 60.00f, 22), false, true, 0.5f, 10.18f, 50.58f, 55.18f);
        UnityScene officeB10 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_10F", "Camera_OfficeB_10F_Ortho", "Office-b-10f", "", new Vector3(-532, 64.00f, 22), false, true, 0.5f, 14.94f, 54.60f, 59.20f);
        UnityScene officeB11 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_11F", "Camera_OfficeB_11F_Ortho", "Office-b-11f", "", new Vector3(-532, 68.00f, 22), false, true, 0.5f, 19.70f, 58.90f, 63.68f);
        UnityScene officeB12 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_12F", "Camera_OfficeB_12F_Ortho", "Office-b-12f", "", new Vector3(-532, 72.00f, 22), false, true, 0.5f, 24.46f, 62.98f, 67.50f);
        UnityScene officeB13 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_13F", "Camera_OfficeB_13F_Ortho", "Office-b-13f", "", new Vector3(-532, 76.00f, 22), false, true, 0.5f, 29.22f, 67.40f, 72.38f);
        UnityScene officeB14 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_14F", "Camera_OfficeB_14F_Ortho", "Office-b-14f", "", new Vector3(-532, 80.00f, 22), false, true, 0.5f, 33.98f, 71.88f, 76.68f);
        UnityScene officeB15 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_15F", "Camera_OfficeB_15F_Ortho", "Office-b-15f", "", new Vector3(-532, 84.00f, 22), false, true, 0.5f, 38.74f, 76.18f, 80.98f);
        UnityScene officeB16 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_16F", "Camera_OfficeB_16F_Ortho", "Office-b-16f", "", new Vector3(-532, 88.00f, 22), false, true, 0.5f, 43.50f, 80.58f, 85.28f);
        UnityScene officeB17 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_17F", "Camera_OfficeB_17F_Ortho", "Office-b-17f", "", new Vector3(-532, 92.00f, 22), false, true, 0.5f, 48.26f, 84.98f, 89.58f);
        UnityScene officeB18 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_18F", "Camera_OfficeB_18F_Ortho", "Office-b-18f", "", new Vector3(-532, 96.00f, 22), false, true, 0.5f, 53.02f, 89.38f, 93.88f);
        UnityScene officeB19 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_19F", "Camera_OfficeB_19F_Ortho", "Office-b-19f", "", new Vector3(-532, 100.0f, 22), false, true, 0.5f, 57.78f, 93.78f, 98.38f);
        UnityScene officeB20 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_20F", "Camera_OfficeB_20F_Ortho", "Office-b-20f", "", new Vector3(-532, 104.0f, 22), false, true, 0.5f, 62.54f, 98.20f, 102.50f);
        UnityScene officeB21 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_21F", "Camera_OfficeB_21F_Ortho", "Office-b-21f", "", new Vector3(-532, 108.0f, 22), false, true, 0.5f, 67.30f, 102.60f, 107.00f);
        UnityScene officeB22 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_22F", "Camera_OfficeB_22F_Ortho", "Office-b-22f", "", new Vector3(-532, 112.0f, 22), false, true, 0.5f, 72.06f, 107.00f, 111.50f);
        UnityScene officeB23 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_23F", "Camera_OfficeB_23F_Ortho", "Office-b-23f", "", new Vector3(-532, 116.0f, 22), false, true, 0.5f, 76.82f, 111.40f, 116.00f);
        UnityScene officeB24 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_24F", "Camera_OfficeB_24F_Ortho", "Office-b-24f", "", new Vector3(-532, 120.0f, 22), false, true, 0.5f, 81.58f, 115.80f, 120.50f);
        UnityScene officeB25 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_25F", "Camera_OfficeB_25F_Ortho", "Office-b-25f", "", new Vector3(-532, 124.0f, 22), false, true, 0.5f, 86.34f, 120.20f, 125.00f);
        UnityScene officeB26 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_26F", "Camera_OfficeB_26F_Ortho", "Office-b-26f", "", new Vector3(-532, 128.0f, 22), false, true, 0.5f, 91.10f, 124.60f, 129.50f);
        UnityScene officeBPH1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_PH1", "Camera_OfficeB_PH1_Ortho", "Office-b-PH-1", "", new Vector3(-509, 132.0f, 22), false, true, 0.5f, 95.86f, 129.20f, 133.80f);
        UnityScene officeBPH2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_PH2", "Camera_OfficeB_PH2_Ortho", "Office-b-PH-2", "", new Vector3(-509, 136.0f, 22), false, true, 0.5f, 95.86f, 132.30f, 136f);
        UnityScene officeBPH3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_PH3", "Camera_OfficeB_PH3_Ortho", "Office-b-PH-3", "", new Vector3(-509, 140.0f, 22), false, true, 0.5f, 95.86f, 135.40f, 140f);

        #endregion

        // 공용공간
        #region Common
        UnityScene common1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_1F", "Camera_Common_1F_Ortho", "common-ab-1f", "", new Vector3(-649, 4.33f, 3), false, true, 0.5f, 4.33f, 9f, 16f);
        UnityScene common2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_2F", "Camera_Common_2F_Ortho", "common-ab-2f", "", new Vector3(-649, 11.58f, 3), false, true, 0.5f, 11.58f, 15.7f, 22.2f);
        UnityScene common3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_3F", "Camera_Common_3F_Ortho", "common-ab-3f", "", new Vector3(-649, 18.25f, 3), false, true, 0.5f, 18.25f, 21.7f, 28.4f);
        UnityScene common4 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_4F", "Camera_Common_4F_Ortho", "common-ab-4f", "", new Vector3(-649, 24.57f, 3), false, true, 0.5f, 24.57f, 27.7f, 34.6f);
        UnityScene common5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_5F", "Camera_Common_5F_Ortho", "common-ab-5f", "", new Vector3(-649, 31.38f, 3), false, true, 0.5f, 31.38f, 33.7f, 40.8f);
        UnityScene common6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_6F", "Camera_Common_6F_Ortho", "common-ab-6f", "", new Vector3(-649, 38.01f, 3), false, true, 0.5f, 38.01f, 39.7f, 46.8f);
        UnityScene common7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_7F", "Camera_Common_7F_Ortho", "common-ab-7f", "", new Vector3(-649, 44.9f, 3), false, true, 0.5f, 44.9f, 45.5f, 51.3f);

        UnityScene commonB1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B1F", "Camera_Common_B1F_Ortho", "basement-ab-1b", "", new Vector3(-878.1f, 37.7f, -198f), false, true, 0.5f, 30f, 34.8f, 43.5f);
        UnityScene commonB2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B2F", "Camera_Common_B2F_Ortho", "basement-ab-2b", "", new Vector3(-878.1f, 27.4f, -198f), false, true, 0.5f, 20f, 25.7f, 35.2f);
        UnityScene commonB3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B3F", "Camera_Common_B3F_Ortho", "basement-ab-3b", "", new Vector3(-878.1f, 21.1f, -198f), false, true, 0.5f, 10f, 20.6f, 25.9f);
        UnityScene commonB4 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B4F", "Camera_Common_B4F_Ortho", "basement-ab-4b", "", new Vector3(-878.1f, 14.8f, -198f), false, true, 0.5f, 0f, 16.5f, 20.9f);
        UnityScene commonB5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B5F", "Camera_Common_B5F_Ortho", "basement-ab-5b", "", new Vector3(-878.1f, 10.4f, -198f), false, true, 0.5f, -10f, 12.7f, 16f);
        UnityScene commonB6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B6F", "Camera_Common_B6F_Ortho", "basement-ab-6b", "", new Vector3(-878.1f, 5.7f, -198f), false, true, 0.5f, -20f, 9f, 13f);
        UnityScene commonB7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B7F", "Camera_Common_B7F_Ortho", "basement-ab-7b", "", new Vector3(-878.1f, 1f, -198f), false, true, 0.5f, -30f, 5.3f, 10f);
        UnityScene commonB8 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Common_B8F", "Camera_Common_B8F_Ortho", "basement-ab-8b", "", new Vector3(-878.1f, -3.3f, -198f), false, true, 0.5f, -40f, 1.6f, 6.2f);
        #endregion

        ModelManager.Instance.Model = this;

        //SetEditMode(true);
        //m_bWallEditMode = true;
        //ChangeScene(common1, false);
        ChangeScene(outdoor, true);

        Light[] lights = Light.GetLights(LightType.Directional, 0);
        Light light = lights[0];

        Vector3 dir = gameObject.transform.position - light.transform.position;

        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<SelectionModel>();
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {

                Mesh mesh = mf.mesh;
                child.gameObject.AddComponent<MeshCollider>();
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                collider.sharedMesh = mf.sharedMesh;
                collider.convex = false;
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();

                sm.meshCollider = collider;
            }
        }

        AddPythonFunction();


        //ShowEvacCircle(true);

        try
        {
            file = new SharedMemory.BufferReadWrite(name: "UnitySamOutsidePoiInfo");
        }
        catch (Exception)
        { }

        // 시간차이로 3D 화면이 나타나지 않는 문제가 있어 2초간 지연시킨다.
        DelayReadyThread(2000);
        //WriteLog("ReadyToRead");
        //ReadyToRead();
        //WriteLog("Finish");

        //POIManager.Instance.AddReverseLODTextPOI("Test Message", -54, 3, 23);
        //POIManager.Instance.ShowBuildingText(false);

        // Unity Editor에서 Mouse Click해서 AlarmZone 확인하기
        /*SetEditMode(true);
        SetBlinkMode(false);
        SetMode(9, true);*/
    }

    private void InitText()
    {
        m_objPoiHiddenCount = GameObject.Find("PoiHiddenCount");
        
        if (m_objPoiHiddenCount != null)
        {
            UnityEngine.UI.Text text = m_objPoiHiddenCount.GetComponent<UnityEngine.UI.Text>();

            if (text != null)
            {
                text.text = "";
            }
        }
    }

    private void DelayReadyThread(int nDelayTime)
    {
        System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DelayReady));
        t.Start(nDelayTime);
    }

    private void DelayReady(object param)
    {
        int nDelayTime = (int)param;
        System.Threading.Thread.Sleep(nDelayTime);
        ReadyToRead();
    }
    
    void Start()
    {
        if (m_mainCamera == null)
            m_mainCamera = Camera.main;

        Vector3 forward = m_mainCamera.transform.forward;
        Vector3 pos = m_mainCamera.transform.position;

        Vector3 v = GetNearestVertex(m_target, forward, pos + forward * 100);
        m_fTargetLength = Plane.GetDistance(pos, v);
        /*m_MainCamera = GetComponent<Camera>();
        coll = GetComponent<Collider>();
        coll.isTrigger = true;
        m_ScreenCenter = coll.bounds.center;*/
    }

    void Update()
    {
        BlinkAlarmZone();
        CheckInitData();

        if (m_hasFocus == false)
            return;

        /*if (Input.GetMouseButtonDown(MOUSE_RIGHT))
        {
            POIManager.Instance.ShowIconPOI(1, "CCTV", true);
        }*/
        
        MouseWorkMode mode = GetMode();
        
        if (mode == MouseWorkMode.PANNING)
            Pan();
        else if (mode == MouseWorkMode.ORBIT && m_bEditMode == false)
            Orbit();
        else if (mode == MouseWorkMode.ZOOM)
            Zoom();
        else if (mode == MouseWorkMode.PICK)
            Pick();
        else if (mode == MouseWorkMode.ADD_ICON)
            AddIcon();
        else if (mode == MouseWorkMode.SELECT_ZONE)
            SelectAlarmZone();

        bool isChanged = SetCurrentLOD();
        SetPoiLodVisible(isChanged);
    }

    private void SetPoiLodVisible(bool isChanged)
    {
        if (m_currentPoiLod == null)
        {
            if (isChanged)
            {
                foreach (KeyValuePair<string, bool> pair in m_dicUsePoiLOD)
                {
                    List<CanvasPOI> pois = POIManager.Instance.GetPOIList(pair.Key);

                    if (pois != null)
                    {
                        foreach (CanvasPOI poi in pois)
                        {
                            if (!poi.bVisible)
                                continue;

                            poi.gameObject.SetActive(true);
                        }
                    }
                }
            }

            SetHiddenPOICount();
            return;
        }

        foreach (KeyValuePair<string, bool> pair in m_dicUsePoiLOD)
        {
            if (pair.Value)
            {
                if (isChanged || m_currentPoiLod.NeedUpdate(pair.Key))
                {
                    m_currentPoiLod.SetPOIVisible(pair.Key, POIManager.Instance.GetPOIList(pair.Key));
                    SetHiddenPOICount();
                }
            }
        }
    }

    public float ZoomLevel
    {
        get { return m_fZoomLevel; }
    }

    private void SetHiddenPOICount()
    {
        if (m_objPoiHiddenCount != null)
        {
            UnityEngine.UI.Text text = m_objPoiHiddenCount.GetComponent<UnityEngine.UI.Text>();

            if (m_isOutdoorView)
            {
                if (text != null)
                {
                    text.text = "";
                }
            }
            else if (m_currentPoiLod != null)
            {
                int nHiddenCount = m_currentPoiLod.GetCurrentHiddenCount();

                if (nHiddenCount == 0)
                {
                    if (text != null)
                    {
                        text.text = "";
                    }
                }
                else
                {
                    if (text != null)
                    {
                        text.text = "숨겨진 POI 개수 : " + nHiddenCount.ToString();
                    }
                }
            }
            else
            {
                if (text != null)
                {
                    text.text = "";
                }
            }
        }
    }

    // Return 값 : CurrentLOD가 변경되었는가?
    private bool SetCurrentLOD()
    {
        if (m_currentPoiLod != null)
        {
            if (m_fZoomLevel >= m_currentPoiLod.MinZoomLevel && m_fZoomLevel < m_currentPoiLod.MaxZoomLevel)
                return false;   
        }
        
        SDMS.PoiLod lod = null;

        foreach (SDMS.PoiLod poiLOD in m_lods)
        {
            if (m_fZoomLevel >= poiLOD.MinZoomLevel && m_fZoomLevel < poiLOD.MaxZoomLevel)
            {
                lod = poiLOD;
                break;
            }
        }

        if (lod != m_currentPoiLod)
        {
            m_currentPoiLod = lod;

            //if (m_currentPoiLod != null)
            //    m_currentPoiLod.SetPOIVisible();
            return true;
        }

        return false;
    }

    private void CheckInitData()
    {
        CameraData initData = m_initData;

        if (initData != null)
        {
            if (initData.CameraName == "0")
                initData.CameraName = "1";
            else
            {
                m_mainCamera.transform.localPosition = initData.LocalPosition;
                m_mainCamera.transform.localEulerAngles = initData.LocalEulerAngle;
                m_mainCamera.transform.localScale = initData.LocalScale;
                m_mainCamera.orthographicSize = initData.OrthoSize;
                m_initData = null;
            }
        }
    }

    private void BlinkAlarmZone()
    {
        ICollection<GameObject> activeAlarmZones = m_dicActiveAlarmZones.Values;

        if (m_blinkMode)
        {
            if (activeAlarmZones.Count > 0)
            {
                m_fAlarmZoneTime += Time.deltaTime;
                int nAlarmZoneTime = (int)m_fAlarmZoneTime;

                if (nAlarmZoneTime >= m_nAlarmZoneData)
                {
                    m_fAlarmZoneTime -= m_nAlarmZoneData;
                    m_hideAlarm = !m_hideAlarm;

                    if (m_hideAlarm)
                    {
                        foreach (GameObject alarmZone in activeAlarmZones)
                        {
                            alarmZone.SetActive(false);
                        }
                    }
                    else
                    {
                        //Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);

                        foreach (GameObject alarmZone in activeAlarmZones)
                        {
                            //alarmZone.transform.localScale = scale;
                            alarmZone.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                m_fAlarmZoneTime = 0.0f;
                m_hideAlarm = true;
            }
        }
        else
        {
            foreach (GameObject alarmZone in activeAlarmZones)
            {
                alarmZone.SetActive(true);
            }
        }
    }

    private void Pan()
    {
        if (m_plane != null)
        {
            Vector3 vMove = m_vMouseOrigin - Input.mousePosition;
            vMove = m_vHorzUnit * vMove.x + m_vVertUnit * vMove.y;
            //m_mainCamera.transform.position = m_vCameraOrigin + vMove;

            Vector3 vPos = m_vCameraOrigin + vMove;

            if (m_currentScene == null)
            {
                if (vPos.y < CameraBottomLimit)
                    vPos.y = CameraBottomLimit;
            }
            else
            {
                if (vPos.y < m_currentScene.BottomElevation)
                    vPos.y = m_currentScene.BottomElevation;
            }

            m_mainCamera.transform.position = vPos;
            m_vCurrentPanMoved = vMove;
        }
    }

    private void Orbit()
    {
        _Orbit(Input.mousePosition);
    }

    private void _Orbit(Vector3 vMouseInput)
    {
        if (m_plane == null)
            return;

        if (useExternalOrbitCenter)
        {
            //Vector3 vOrbitCenter = ExternalOrbitCenter;
            m_vOrbitCenter = ExternalOrbitCenter + m_vPanMovedFromFirstPosition;
        }
        else
        {
            Vector3 vOrbitCenter;
            Ray ray = m_mainCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));

            if (m_plane.GetVertex(ray, out vOrbitCenter))
                m_vOrbitCenter = vOrbitCenter;
        }

        Vector3 yUnit = new Vector3(0f, -1f, 0f);

        if (m_isFirstOrbit == true)
        {
            m_isFirstOrbit = false;
            Vector3 vX = new Vector3(1.0f, 0.0f, 0.0f);
            m_mainCamera.transform.localRotation.ToAngleAxis(out m_xAngle, out yUnit);
            m_mainCamera.transform.localRotation.ToAngleAxis(out m_yAngle, out vX);
        }

        Vector3 vCurrent = vMouseInput;

        Vector3 vDiff = vCurrent - m_vMouseOrigin;
        Vector3 rCenter = new Vector3(m_vOrbitCenter.x, m_vOrbitCenter.y, m_vOrbitCenter.z);
        if (vCurrent == m_vMouseOrigin)
            return;

        float pitch = (-0.5f * vDiff.y);
        float yaw = (-0.5f * vDiff.x);

        m_xAngle += yaw;
        m_yAngle += pitch;

        if (m_yAngle >= 80.0f)
        {
            if (pitch > 0)
                pitch = 0f;
        }

        if (m_yAngle <= 5.0f)
        {
            if (pitch < 0)
                pitch = 0f;
        }

        m_yAngle = Mathf.Clamp(m_yAngle, 0, 80f);
        m_xAngle = Mathf.Clamp(m_xAngle, 0, 360f);

        Quaternion rot1 = Quaternion.AngleAxis(yaw, yUnit);
        Quaternion rot2 = Quaternion.AngleAxis(pitch, m_mainCamera.transform.right);
        Quaternion q1 = (rot1 * rot2);

        Vector3 MposToCam = q1 * (m_mainCamera.transform.position - rCenter);
        Vector3 vPos = (MposToCam + rCenter);
        if (vPos.y < 2.0f)
            vPos.y = 2.0f;

        Vector3 cCalc = m_vOrbitCenter;

        if (m_currentScene == null)
        {
            if (vPos.y < CameraBottomLimit)
                vPos.y = CameraBottomLimit;
        }
        else
        {
            if (vPos.y < m_currentScene.BottomElevation)
                vPos.y = m_currentScene.BottomElevation;
        }

        //m_mainCamera.transform.position = vPos;
        Vector3 vTarget;

        if (m_vOrbitCenter != Vector3.zero)
        {
            //m_mainCamera.transform.LookAt(m_vOrbitCenter);
            vTarget = m_vOrbitCenter;
        }
        else
        {
            //m_mainCamera.transform.LookAt(rCenter);
            cCalc = rCenter;
            vTarget = rCenter;
        }

        if (vPos.y < vTarget.y)
            vPos.y = vTarget.y;

        m_mainCamera.transform.position = vPos;
        m_mainCamera.transform.LookAt(vTarget);

        m_xAngle += yaw;
        m_yAngle += pitch;

        Vector3 pos1 = Camera.main.transform.position;
        Vector3 pos2 = Camera.main.transform.position;
        pos1.y = 0;

        Vector3 pos3 = cCalc;
        pos3.y = 0;
        Vector3 vLen = pos1 - pos3;
        float fLength1 = vLen.magnitude;
        float fLength2 = (pos2 - pos3).magnitude;

        double dValue = System.Math.Acos(fLength1 / fLength2);
        m_yAngle = (float)(dValue * 180 / System.Math.PI);

        m_vMouseOrigin = vCurrent;
        m_rotatePOI = false;
    }
    
    public float GetZoomSize()
    {
        return m_mainCamera.orthographicSize;
    }

    private float m_fZoomLevel = 0.0f;

    private void Zoom(float zDelta = 0.0f)
    {
        if (m_bEditMode)
        {
            CameraData cameraData = m_currentScene == null ? null : m_currentScene.OrthoCameraData;
            int nOrthoMax = m_nOrthoMax;
            int nOrthoMin = m_nOrthoMin;

            if (cameraData != null && cameraData.OrthoMinSize != null)
                nOrthoMin = cameraData.OrthoMinSize.Data;

            if (cameraData != null && cameraData.OrthoMaxSize != null)
                nOrthoMax = cameraData.OrthoMaxSize.Data;

            if (zDelta == 0.0f)
                zDelta = Input.GetAxis("Mouse ScrollWheel");

            if (zDelta < 0)
            {
                float fSize = m_mainCamera.orthographicSize;
                fSize += 1;

                if (fSize > nOrthoMax)
                    fSize = nOrthoMax;

                m_mainCamera.orthographicSize = fSize;
            }
            else if (zDelta > 0)
            {
                float fSize = m_mainCamera.orthographicSize;
                fSize -= 1;

                if (fSize < nOrthoMin)
                    fSize = nOrthoMin;

                m_mainCamera.orthographicSize = fSize;
            }
        }
        else
        {
            if (m_plane == null)
            {
                SetOrigin();

                if (m_plane == null)
                    return;
            }

            if (zDelta == 0.0f)
                zDelta = Input.GetAxis("Mouse ScrollWheel");

            Vector3 vOrbitCenter;
            Ray ray = m_mainCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));

            if (m_plane.GetVertex(ray, out vOrbitCenter))
                m_vOrbitCenter = vOrbitCenter;

            int nFlag = m_isOutdoorView ? 4 : 1;
            float fZoomDistance = 14.0f * nFlag;

            if (zDelta != 0)
            {
                float distance = -(float)((zDelta) * fZoomDistance);
                Vector3 vOrgPos = m_mainCamera.transform.position;
                Vector3 zCenter = new Vector3(m_vOrbitCenter.x, m_vOrbitCenter.y, m_vOrbitCenter.z);

                Vector3 dirCam = (zCenter - vOrgPos).normalized;
                Vector3 dir = dirCam * distance;
                Vector3 vPos = vOrgPos - dir;

                if (vPos.y < 3.0f)
                    vPos.y = 3.0f;
                else
                    m_fZoomLevel += zDelta;

                m_mainCamera.transform.position = vPos;
            }
        }
    }

    /*private void WriteCameraState()
    {
        string strLog = string.Format("Camera Position : {0}, {1}, {2}", m_mainCamera.transform.localPosition.x, m_mainCamera.transform.localPosition.y, m_mainCamera.transform.localPosition.z);
        WriteLog(strLog);

        strLog = string.Format("Camera Rotation : {0}, {1}, {2}", m_mainCamera.transform.localEulerAngles.x, m_mainCamera.transform.localEulerAngles.y, m_mainCamera.transform.localEulerAngles.z);
        WriteLog(strLog);
    }*/

    private void Pick()
    {
        Get3DPosition((int)Input.mousePosition.x, (int)Input.mousePosition.y);
    }

    private MouseWorkMode GetMode()
    {
        if (Input.GetMouseButtonDown(MOUSE_WHEEL))
        {
            return PanningMode(MouseState.MButtonDown);
        }
        else if (Input.GetMouseButtonDown(MOUSE_LEFT))
        {
            if (m_bEditMode)
            {
                if (m_editModeType == EditModeType.AddIcon)
                    return AddIconMode();
                else if (m_editModeType == EditModeType.MoveIcon)
                    return MoveIconMode();
                else if (m_editModeType == EditModeType.DeleteIcon)
                    return DeleteIconMode();
                else if (m_editModeType == EditModeType.PickIcon)
                    return PickMode();
            }

            if (m_pickMode)
                return PickMode();
            else if (m_orbitMode && m_bEditMode == false)
                return OrbitMode(MouseState.LButtonDown);
            else if (m_translateMode)
                return PanningMode(MouseState.LButtonDown);
            else if (m_selectAlarmZoneMode)
                return SelectAlarmZoneMode();
        }
        else if (m_bEditMode == false && Input.GetMouseButtonDown(MOUSE_RIGHT))
        {
            return OrbitMode(MouseState.RButtonDown);
            //SelectScene("b05f");
            //WriteCameraState();
            //SelectScene("01f");
        }
        else if (m_bEditMode == true && Input.GetMouseButtonDown(MOUSE_RIGHT))
        {
            CustomizingController custom = CustomizingController.Instance;
            if (custom.CurSelectWall != null)
            {
                if (custom.CurStage == CustomizingStage.Mode_Move)
                {
                    custom.CurStage = CustomizingStage.Mode_Scale;
                    custom.BT_ScaleMode();
                }
                else if (custom.CurStage == CustomizingStage.Mode_Scale)
                {
                    custom.CurStage = CustomizingStage.Mode_Rotate;
                    custom.BT_RotateMode();
                }
                else if (custom.CurStage == CustomizingStage.Mode_Rotate)
                {
                    custom.CurStage = CustomizingStage.Mode_Move;
                    custom.BT_MoveMode();
                }
            }
        }

        if (m_prevMovingMode == MouseWorkMode.PANNING)
        {
            if (m_prevState == MouseState.LButtonDown && Input.GetMouseButtonUp(MOUSE_LEFT))
            {
                InitPanMoving();
                return NoneMode();
            }
            else if (m_prevState == MouseState.MButtonDown && Input.GetMouseButtonUp(MOUSE_WHEEL))
            {
                InitPanMoving();
                return NoneMode();
            }

            return MouseWorkMode.PANNING;
        }
        else if (m_prevMovingMode == MouseWorkMode.ORBIT)
        {
            if (Input.GetMouseButtonUp(MOUSE_LEFT) || Input.GetMouseButtonUp(MOUSE_RIGHT))
                return NoneMode();

            return MouseWorkMode.ORBIT;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (m_hasFocus == false)
                return NoneMode();

            if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1)
            {
            }
            else
            {
                SetOrigin();
                return MouseWorkMode.ZOOM;
            }
        }

        return NoneMode();
    }
 
    private void InitPanMoving()
    {
        m_vPanMovedFromFirstPosition += m_vCurrentPanMoved;
    }

    public void AddIcon()
    {        
        int x = (int)Input.mousePosition.x;
        int y = (int)Input.mousePosition.y;
        
        Vector3 v = ScreenToWorldForPOI(x, y);

        //m_strTargetEditIconType = "Fire";
        //m_nNewIconID = 1;

        // TEST
        if (m_strTargetEditIconType.Length == 0)
            m_strTargetEditIconType = POIManager.FIRE_TYPE;

        EffectPOI fireEffct = null;
        CanvasPOI poi = CanvasPOI.MakeInstance(v, CurrentScene.DBottomHeight, m_strTargetEditIconType, m_strTargetEditIconType, m_nNewIconID, ref fireEffct);

        if (poi != null)
        {
            POIManager.Instance.AddPOI(poi, poi.OriginalPOIType);
            poi.gameObject.SetActive(true);
            if (fireEffct != null)
            {
                POIManager.Instance.AddEffectPOI(fireEffct, POIManager.FIRE_ALARM_ON_TYPE_EFFECT);
                fireEffct.gameObject.SetActive(true);
            }

            m_nNewIconID++;
            m_editModeType = EditModeType.MoveIcon;

            Vector3 vPos = poi.Position;
            string szMsg = string.Format("OnAddPOI('{0}_{1}',{2},{3},{4})", poi.OriginalPOIType, poi.ID, vPos.x, vPos.y, vPos.z);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
        }
    }

    private MouseWorkMode AddIconMode()
    {
        m_prevState = MouseState.None;
        m_prevMovingMode = MouseWorkMode.NONE;
        return MouseWorkMode.ADD_ICON;
    }

    private MouseWorkMode MoveIconMode()
    {
        m_prevState = MouseState.None;
        m_prevMovingMode = MouseWorkMode.NONE;
        return MouseWorkMode.MOVE_ICON;
    }

    private MouseWorkMode DeleteIconMode()
    {
        m_prevState = MouseState.None;
        m_prevMovingMode = MouseWorkMode.NONE;
        return MouseWorkMode.DELETE_ICON;
    }

    private MouseWorkMode NoneMode()
    {
        m_prevState = MouseState.None;
        m_prevMovingMode = MouseWorkMode.NONE;
        return MouseWorkMode.NONE;
    }

    private MouseWorkMode PickMode()
    {
        m_prevMovingMode = MouseWorkMode.NONE;
        m_prevState = MouseState.None;
        return MouseWorkMode.PICK;
    }

    private MouseWorkMode OrbitMode(MouseState mouseState)
    {
        SetOrigin();

        m_prevState = mouseState;
        m_prevMovingMode = MouseWorkMode.ORBIT;
        return MouseWorkMode.ORBIT;
    }

    private MouseWorkMode PanningMode(MouseState mouseState)
    {
        SetOrigin();

        m_prevState = mouseState;
        m_prevMovingMode = MouseWorkMode.PANNING;
        return MouseWorkMode.PANNING;
    }

    private MouseWorkMode SelectAlarmZoneMode()
    {
        m_prevMovingMode = MouseWorkMode.NONE;
        m_prevState = MouseState.None;
        return MouseWorkMode.SELECT_ZONE;
    }

    private Vector3 m_PickPos = new Vector3();

    public void ManualZoom(float zDelta)
    {
        if (zDelta > 0.0f)
            zDelta = 0.1f;
        else if (zDelta < 0.0f)
            zDelta = -0.1f;

        Zoom(zDelta);
    }

    private bool m_hasFocus = false;
    public void OnApplicationFocus(bool hasFocus)
    {
        m_hasFocus = hasFocus;
    }

    private void SetOrigin()
    {
        Vector3 forward = m_mainCamera.transform.forward;
        Vector3 up = m_mainCamera.transform.up;
        Vector3 right = Vector3.Cross(up, forward);
        Vector3 vTarget = m_mainCamera.transform.position + forward * m_fTargetLength;

        Vector3 vR = vTarget + right * 100;
        Vector3 vUp = vTarget + up * 100;

        m_plane = Plane.MakePlane(vTarget, vR, vUp);

        if (m_plane != null)
        {
            Ray ray1 = m_mainCamera.ScreenPointToRay(Vector3.zero);
            Ray ray2 = m_mainCamera.ScreenPointToRay(new Vector3(Screen.width, 0.0f, 0.0f));
            Ray ray3 = m_mainCamera.ScreenPointToRay(new Vector3(0.0f, Screen.height, 0.0f));

            Vector3 vTL, vBL, vBR;

            if (m_plane.GetVertex(ray1, out vBL) && m_plane.GetVertex(ray2, out vBR) && m_plane.GetVertex(ray3, out vTL))
            {
                m_vHorzUnit = ((vBR - vBL) / Screen.width) * m_fPanningScale;
                m_vVertUnit = ((vTL - vBL) / Screen.height) * m_fPanningScale;

                m_vCameraOrigin = m_mainCamera.transform.position;
                m_vMouseOrigin = Input.mousePosition;
            }
            else
                m_plane = null;
        }
    }

    private Vector3 GetNearestVertex(Vector3 rVertex, Vector3 vLineBegin, Vector3 vLineEnd)
    {
        float dLen = Plane.GetDistance(rVertex, vLineBegin);
        float dLen2 = Plane.GetDistance(rVertex, vLineEnd);

        if (dLen <= Plane.TOLERANCE || dLen2 <= Plane.TOLERANCE)
            return rVertex;

        double dAngle = Plane.GetAngle(rVertex, vLineBegin, vLineEnd);
        float dH = (float)(dLen * System.Math.Cos(dAngle));

        Vector3 vertex = Plane.GetLinearVertex(vLineBegin, vLineEnd, dH);
        return vertex;
    }

    public GameObject GetAlarmZoneObject(string strMeshName)
    {
        if (m_currentScene != null)
        {
            return m_currentScene.GetAlarmZone(strMeshName);
        }

        return null;
    }

    public void SetZoomObject(string szObjectName)
    {
        SetOrigin();
        GameObject obj = GetAlarmZoneObject(szObjectName);

        if (obj == null)
            return;

        Transform movObj = obj.transform;

        if (movObj != null)
        {
            MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Bounds objBound = mr.bounds;

                Vector3 vCamera = m_plane.GetNearestVertex(m_mainCamera.transform.position);
                Vector3 vObj = m_plane.GetNearestVertex(objBound.center);
                Vector3 vCameraPos = vObj + m_mainCamera.transform.position - vCamera;
                m_mainCamera.transform.position = vCameraPos;
            }
        }
    }

    #region map customizing
    public void AddWall()
    {
        if (m_bEditMode)
        {
            CustomDoorSH.Instance.ClearSelectedDoor();
            CustomizingController.Instance.BT_SpawnWall();
        }
    }

    public void GetWalls(string path)
    {
        CustomizingController.Instance.GetWalls(path);
    }

    public void LoadWalls(string path, string sceneName)
    {
        CustomizingController.Instance.LoadWalls(path, sceneName);
    }

    public void ChangeWall()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = "SendMessage('ChangeWall()')";
            proxy.RunPythonScript(szMsg);
        }
    }

    public void ChangeDoor()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = "SendMessage('ChangeWall()')";
            proxy.RunPythonScript(szMsg);
        }
    }

    public void SetUseSnap(bool bUse)
    {
        CustomizingController.Instance.SetUseSnap(bUse);
    }

    public void GetWallInfo(float x, float y, float scale, float rotate)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = string.Format("SendMessage('GetWallInfo({0}, {1}, {2}, {3})')", x, y, scale, rotate);
            proxy.RunPythonScript(szMsg);
        }
    }

    public void AddSpaceText(string strTxt)
    {
        CustomSpaceText.Instance.AddSpaceText(strTxt);
    }

    public void ChangeFontSpaceText(string strFontName, float nSize, int nStyle)
    {
        CustomSpaceText.Instance.ChangeFontSpaceText(nSize, strFontName, nStyle);
    }

    public void ChangeColorSpaceText(string hexColor)
    {
        CustomSpaceText.Instance.ChangeColorSpaceText(hexColor);
    }

    public void GetSpaceTexts(string path)
    {
        CustomSpaceText.Instance.GetSpaceText(path);
    }

    public void LoadSpaceTexts(string path, string sceneName)
    {
        CustomSpaceText.Instance.LoadSpaceText(path, sceneName);
    }

    public void ChangeSpaceText()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = "SendMessage('ChangeSpaceText()')";
            proxy.RunPythonScript(szMsg);
        }
    } 

    public void AddDoor()
    {
        if (m_bEditMode)
        {
            CustomizingController.Instance.ClearSelectedWall();
            CustomDoorSH.Instance.ClearSelectedDoor();
            CustomDoorSH.Instance.BT_SpawnDoor();
        }
    }
    #endregion

    private List<CameraData> MakeUndergroundCameraDatas()
    {
        List<CameraData> cameras = new List<CameraData>();

        AddCameraData(cameras, new Vector3(-273.6014f, 259.7175f, -582.4695f), new Vector3(70, 190, 0), "Camera_Underground_B1F");
        AddCameraData(cameras, new Vector3(-274.2664f, 249.1929f, -586.242f), new Vector3(70, 190, 0), "Camera_Underground_B2F");
        AddCameraData(cameras, new Vector3(-276.6622f, 235.8334f, -581.8f), new Vector3(70, 190, 0), "Camera_Underground_B3F");
        AddCameraData(cameras, new Vector3(-276.995f, 230.5711f, -583.6862f), new Vector3(70, 190, 0), "Camera_Underground_B4F");
        AddCameraData(cameras, new Vector3(-274.7945f, 217.8418f, -581.8129f), new Vector3(70, 190, 0), "Camera_Underground_B5F");
        AddCameraData(cameras, new Vector3(-274.7684f, 211.0676f, -579.5443f), new Vector3(70, 190, 0), "Camera_Underground_B6F");
        AddCameraData(cameras, new Vector3(-242.5889f, 174.013f, -581.5966f), new Vector3(70, 190, 0), "Camera_Underground_B7F");
        AddCameraData(cameras, new Vector3(-418.9181f, 365.5539f, -330.0398f), new Vector3(70, 90, 0), "Camera_SubwayPath_B02F");

        return cameras;
    }

    private List<CameraData> MakeUndergroundOrthoCameraDatas()
    {
        List<CameraData> cameras = new List<CameraData>();

        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B1F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B2F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B3F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B4F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B5F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B6F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-288.5688f, 626.5f, -651.4611f), new Vector3(90, 180, 0), "Camera_Underground_B7F_Ortho", 100, 120);
        AddOrthoCameraData(cameras, new Vector3(-294.5279f, 643.3f, -331.2228f), new Vector3(90, 90, 0), "Camera_SubwayPath_B02F_Ortho", 180, 220);

        return cameras;
    }

    private void AddCameraData(List<CameraData> cameras, Vector3 vPosition, Vector3 vEulerAngle, string strCameraName)
    {
        CameraData cameraData = new CameraData();
        cameraData.LocalPosition = vPosition;
        cameraData.LocalEulerAngle = vEulerAngle;
        cameraData.LocalScale = new Vector3(1, 1, 1);
        cameraData.CameraName = strCameraName;

        cameras.Add(cameraData);
    }

    private void AddOrthoCameraData(List<CameraData> cameras, Vector3 vPosition, Vector3 vEulerAngle, string strCameraName, float fOrthoSize, int nOrthoMax = -1, int nOrthoMin = -1)
    {
        CameraData cameraData = new CameraData();
        cameraData.LocalPosition = vPosition;
        cameraData.LocalEulerAngle = vEulerAngle;
        cameraData.LocalScale = new Vector3(1, 1, 1);
        cameraData.CameraName = strCameraName;
        cameraData.OrthoSize = fOrthoSize;

        if (nOrthoMax > 0)
            cameraData.OrthoMaxSize = new DBUtility2.VariousData<int>(nOrthoMax);

        if (nOrthoMin > 0)
            cameraData.OrthoMinSize = new DBUtility2.VariousData<int>(nOrthoMin);

        cameras.Add(cameraData);
    }

    public string SelectAlarmZone()
    {
        if (m_currentScene != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return m_currentScene.SelectAlarmZone(ray);
        }

        return null;
    }

    // Zone별 POI들을 읽어서 POI 좌표에 따라 EquipZone을 찾아준다.
    public void CheckPOIZones()
    {
        SetEditMode(true);

        Dictionary<int, string> dicPOIZones = ReadPOIZoneList();

        foreach (KeyValuePair<int, string> pair in dicPOIZones)
        {
            int nIndex = pair.Value.LastIndexOf(';');

            if (nIndex < 0)
                continue;

            string strSceneName = pair.Value.Substring(0, nIndex);
            string strFileName = pair.Value.Substring(nIndex + 1);

            SelectScene(strSceneName);

            if (m_currentScene == null)
                return;

            int nDotIndex = strFileName.LastIndexOf('.');
            string strResultFileName = "";

            if (nDotIndex < 0)
            {
                strResultFileName = strFileName + "_result";
            }
            else
            {
                strResultFileName = strFileName.Substring(0, nDotIndex) + "_result" + strFileName.Substring(nDotIndex);
            }

            if (File.Exists(strFileName) == false)
                continue;

            StreamWriter writer = new StreamWriter(strResultFileName, false, System.Text.Encoding.UTF8);
            StreamReader reader = new StreamReader(strFileName, System.Text.Encoding.UTF8);

            Vector3 vDir = new Vector3(0.0f, -1.0f, 0.0f);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                if (tokens.Length < 5)
                    continue;

                string strID = tokens[0].Trim();
                string strPOIName = tokens[1].Trim();
                string strX = tokens[2].Trim();
                string strY = tokens[3].Trim();
                string strZ = tokens[4].Trim();

                int nID;
                float x, y, z;

                if (int.TryParse(strID, out nID) == false)
                    continue;

                if (float.TryParse(strX, out x) == false)
                    continue;
                if (float.TryParse(strY, out y) == false)
                    continue;
                if (float.TryParse(strZ, out z) == false)
                    continue;

                Vector3 vOrigin = new Vector3(x, y, z);
                Ray ray = new Ray(vOrigin, vDir);
                string strZoneName = m_currentScene.SelectAlarmZone(ray);

                if (strZoneName == null)
                    strZoneName = "null";

                string strLog = string.Format("{0}\t{1}\t{2}", nID, strPOIName, strZoneName);
                writer.WriteLine(strLog);
            }

            reader.Close();
            writer.Close();
        }
    }

    private Dictionary<int, string> ReadPOIZoneList()
    {
        Dictionary<int, string> dicPOIZones = new Dictionary<int, string>();
        StreamReader reader = new StreamReader("C:/temp/poiZoneList.txt", System.Text.Encoding.UTF8);

        while (reader.EndOfStream == false)
        {
            string strLine = reader.ReadLine().Trim();

            if (strLine.Length == 0)
                continue;

            string[] tokens = strLine.Split('\t');

            if (tokens.Length < 3)
                continue;

            string strID = tokens[0].Trim();
            string strSceneName = tokens[1].Trim();
            string strFileName = tokens[2].Trim();

            int nID;

            if (int.TryParse(strID, out nID) == false)
                continue;

            if (strFileName.IndexOf('/') < 0 && strFileName.IndexOf('\\') < 0)
            {
                strFileName = "C:\\Temp\\POI\\" + strFileName;
            }

            dicPOIZones[nID] = strSceneName + ";" + strFileName;
        }

        reader.Close();
        return dicPOIZones;
    }

    public void InitPOILod(string strPOIType)
    {
        foreach (SDMS.PoiLod lod in m_lods)
        {
            lod.Initialize(strPOIType);
        }
    }

    private void InitAllPOILod()
    {
        foreach (SDMS.PoiLod lod in m_lods)
        {
            lod.InitializeAll();
        }

        m_currentPoiLod = null;
    }

    public void SetPoiLod(string strPOIType, bool useLOD)
    {
        m_dicUsePoiLOD[strPOIType] = useLOD;

        if (useLOD == false)
        {
            List<CanvasPOI> pois = POIManager.Instance.GetPOIList(strPOIType);

            foreach (CanvasPOI poi in pois)
            {
                poi.gameObject.SetActive(true);
            }
        }
        else
        {
            InitAllPOILod();
        }
    }

    public void AddPoiLodValue(float fMinZoomValue, float fMaxZoomValue, float fDistance)
    {
        m_lods.Add(new SDMS.PoiLod(this, fMinZoomValue, fMaxZoomValue, fDistance));

        InitAllPOILod();
    }

    public void ClearPoiLodValue()
    {
        m_lods.Clear();
        m_currentPoiLod = null;

        foreach (KeyValuePair<string, bool> pair in m_dicUsePoiLOD)
        {
            List<CanvasPOI> pois = POIManager.Instance.GetPOIList(pair.Key);

            foreach (CanvasPOI poi in pois)
            {
                poi.gameObject.SetActive(true);
            }
        }
    }
}

public class Plane
{
    public const float TOLERANCE = 0.001f;

    private double a, b, c, d;

    public Plane(double a, double b, double c, double d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }

    // v1, v2, v3를 지나는 평면의 방정식을 구한다.(ax + by + cz + d = 0)
    // v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
    public static Plane MakePlane(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        if (GetDistance(v1, v2) <= TOLERANCE ||
            GetDistance(v2, v3) <= TOLERANCE ||
            GetDistance(v3, v1) <= TOLERANCE)
            return null;

        double a = v1.y * (v2.z - v3.z) + v2.y * (v3.z - v1.z) + v3.y * (v1.z - v2.z);
        double b = v1.z * (v2.x - v3.x) + v2.z * (v3.x - v1.x) + v3.z * (v1.x - v2.x);
        double c = v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y);
        double d = -(v1.x * (v2.y * v3.z - v3.y * v2.z) + v2.x * (v3.y * v1.z - v1.y * v3.z) + v3.x * (v1.y * v2.z - v2.y * v1.z));
        return new Plane(a, b, c, d);
    }

    // 평면(ax + by + cz + d = 0) 위에서 rVertex와 가장 가까운 점을 알려준다.
    public Vector3 GetNearestVertex(Vector3 vertex)
    {
        double k = -(a * vertex.x + b * vertex.y + c * vertex.z + d) / (a * a + b * b + c * c);
        return new Vector3((float)(a * k + vertex.x), (float)(b * k + vertex.y), (float)(c * k + vertex.z));
    }

    public static float GetDistance(Vector3 v1, Vector3 v2)
    {
        return (float)System.Math.Sqrt((v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y) + (v2.z - v1.z) * (v2.z - v1.z));
    }

    // v1과 vCenter가 이루는 직선과 vCenter와 v2가 이루는
    // 직선이 서로 만나 이루는 각을 리턴한다.
    // Return 값 : Radian
    public static double GetAngle(Vector3 v1, Vector3 vCenter, Vector3 v2)
    {
        // 코사인 제2법칙
        // C²= A²+ B²- 2ABcosΘ
        double a = GetDistance(v1, vCenter);
        double b = GetDistance(v2, vCenter);
        double c = GetDistance(v1, v2);

        double cosData = (a * a + b * b - c * c) / 2 / a / b;
        if (cosData < -1.0) cosData = -1.0;
        else if (cosData > 1.0) cosData = 1.0;

        return System.Math.Acos(cosData);
    }

    // v1과 v2를 잇는 직선상에서 v1으로부터 v2 방향으로 dLength 만큼
    // 떨어진 거리의 점을 구한다.
    public static Vector3 GetLinearVertex(Vector3 v1, Vector3 v2, float fLength)
    {
        // v1과 v2 사이의 거리
        float dL = GetDistance(v1, v2);

        if (dL <= TOLERANCE)
            return new Vector3(v1.x, v1.y, v1.z);

        Vector3 v3 = v1 + (v2 - v1) * fLength / dL;
        return v3;
    }

    // 평면과 직선이 만나는 점을 구한다.
    public bool GetVertex(Ray ray, out Vector3 vResult)
    {
        vResult = Vector3.zero;
        double data1 = a * ray.direction.x + b * ray.direction.y + c * ray.direction.z;

        if (data1 <= TOLERANCE)
            return false;

        double data2 = a * ray.origin.x + b * ray.origin.y + c * ray.origin.z + d;
        float t = (float)(-(data2 / data1));

        vResult.x = ray.origin.x + ray.direction.x * t;
        vResult.y = ray.origin.y + ray.direction.y * t;
        vResult.z = ray.origin.z + ray.direction.z * t;

        return true;
    }
}