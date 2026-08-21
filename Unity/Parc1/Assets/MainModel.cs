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
        "GetWalls",
        "LoadWalls",
        "ChangeWall",
        "SetWallEditMode",
        "SetUseSnap",
        "GetWallInfo",
        "AddSpaceText",
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
    private int m_nOrthoMax = 60;
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
            proxy.UserObject.SetVariable("GetWalls", new Action<string>(GetWalls));
            proxy.UserObject.SetVariable("LoadWalls", new Action<string, string>(LoadWalls));
            proxy.UserObject.SetVariable("SetUseSnap", new Action<bool>(SetUseSnap));
            proxy.UserObject.SetVariable("GetWallInfo", new Action<float, float, float, float>(GetWallInfo));

            proxy.UserObject.SetVariable("AddSpaceText", new Action<string>(AddSpaceText));
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
            m_mainCamera.orthographic = false;
            m_rotatePOI = false;
            m_bWallEditMode = false;
            
            CustomizingController.Instance.ClearSelectedWall(); // 편집 모드가 종료될 때 선택되있는 가벽이 있으면 해제하기
            CustomizingController.Instance.SetWallColor(); // 가벽 편집 모드일때 가벽 색상 바꾸기

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
            if (btnName != "btnPanning" && btnName != "btnPoiVisible" && btnName != "btnText" && btnName != "btnOrbit" && btnName != "btnSlideLeft" && btnName != "btnHome" && btnName != "btnManualReport"
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
            Debug.Log(strModelName + " is NULL");
            return null;
        }

        if (redModel == null)
        {
            Debug.Log(strRedModelName + " is NULL");
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

        if (scene != null)
        {
            if (m_bEditMode && isOutdoor == false)
            {
                m_mainCamera.transform.localPosition = scene.OrthoCameraData.LocalPosition;
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

        UnityScene outdoor = SetOutdoorScene(m_dicSceneOutdoors, dicCameraDatas, "Outdoor_Camera", "", "Outdoor_Model", new Vector3(-172.1f, 30.9f, 623.9f), false, false, 0.5f, -1.1f);

        SetBuildingScene(m_dicSceneBuildings, outdoor, "ht", "ht_red");
        SetBuildingScene(m_dicSceneBuildings, outdoor, "sm", "sm_red");
        SetBuildingScene(m_dicSceneBuildings, outdoor, "t01", "t01_red");
        SetBuildingScene(m_dicSceneBuildings, outdoor, "t02", "t02_red");

        #region Hotel
        UnityScene indoor1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_1F", "Camera_Hotel_1F_Ortho", "h01f", "h01f_zone", new Vector3(0.0f, 2.07f, 0.0f), false, true, 0.5f, -8.5f, 1f, 4.5f);
        UnityScene indoor2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_2F", "Camera_Hotel_2F_Ortho", "h02f", "h02f_zone", new Vector3(0.0f, 12.5f, 0.0f), false, true, 0.5f, 2.5f, 4.2f, 7.5f, indoor1);
        UnityScene indoor3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_3F", "Camera_Hotel_3F_Ortho", "h03f", "h03f_zone", new Vector3(0.0f, 15.5f, 0.0f), false, true, 0.5f, 5.5f, 7.4f, 10.5f);
        UnityScene indoor4 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_4F", "Camera_Hotel_4F_Ortho", "h04f", "h04f_zone", new Vector3(0.0f, 19.0f, 0.0f), false, true, 0.5f, 9, 10.6f, 13.5f, indoor3);
        UnityScene indoor5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_5F", "Camera_Hotel_5F_Ortho", "h05f", "h05f_zone", new Vector3(0.0f, 22, 0.0f), false, true, 0.5f, 12, 13.8f, 16.5f);
        UnityScene indoor6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_6F", "Camera_Hotel_6F_Ortho", "h06f", "h06f_zone", new Vector3(0.0f, 25, 0.0f), false, true, 0.5f, 15, 17f, 20f, indoor5);
        UnityScene indoor7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_7F", "Camera_Hotel_7F_Ortho", "h07f", "h07f_zone", new Vector3(0.0f, 28, 0.0f), false, true, 0.5f, 18, 20.2f, 23.5f);
        UnityScene indoor8 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_8F", "Camera_Hotel_8F_Ortho", "h08f", "h08f_zone", new Vector3(0.0f, 32, 0.0f), false, true, 0.5f, 22, 23.4f, 27.0f, indoor7);
        UnityScene indoor9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_9F", "Camera_Hotel_9F_Ortho", "h09f", "h09f_zone", new Vector3(0.0f, 34, 0.0f), false, true, 0.5f, 24, 26.6f, 30f);
        UnityScene indoor10 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_10F", "Camera_Hotel_10F_Ortho", "h10f", "h10f_zone", new Vector3(0.0f, 38.8f, 0.0f), false, true, 0.5f, 25.5f, 29.8f, 33.2f, indoor9);
        UnityScene indoor11 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_11F", "Camera_Hotel_11F_Ortho", "h11f", "h11f_zone", new Vector3(0.0f, 42, 0.0f), false, true, 0.5f, 32, 32.8f, 36.4f);
        UnityScene indoor12 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_12F", "Camera_Hotel_12F_Ortho", "h12f", "h12f_zone", new Vector3(0.0f, 45, 0.0f), false, true, 0.5f, 35, 36f, 39.6f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_13F", "Camera_Hotel_13F_Ortho", "h13f", "h13f_zone", new Vector3(0.0f, 48, 0.0f), false, true, 0.5f, 38, 39.2f, 42.8f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_14F", "Camera_Hotel_14F_Ortho", "h14f", "h14f_zone", new Vector3(0.0f, 51, 0.0f), false, true, 0.5f, 41, 42.4f, 46f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_15F", "Camera_Hotel_15F_Ortho", "h15f", "h15f_zone", new Vector3(0.0f, 54, 0.0f), false, true, 0.5f, 44, 45.6f, 49.2f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_16F", "Camera_Hotel_16F_Ortho", "h16f", "h16f_zone", new Vector3(0.0f, 57, 0.0f), false, true, 0.5f, 47, 48.8f, 52.4f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_17F", "Camera_Hotel_17F_Ortho", "h17f", "h17f_zone", new Vector3(0.0f, 60, 0.0f), false, true, 0.5f, 50, 52.2f, 55.6f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_18F", "Camera_Hotel_18F_Ortho", "h18f", "h18f_zone", new Vector3(0.0f, 63, 0.0f), false, true, 0.5f, 53, 55.4f, 58.8f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_19F", "Camera_Hotel_19F_Ortho", "h19f", "h19f_zone", new Vector3(0.0f, 67, 0.0f), false, true, 0.5f, 57, 58.6f, 62f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_20F", "Camera_Hotel_20F_Ortho", "h20f", "h20f_zone", new Vector3(0.0f, 70.0f, 0.0f), false, true, 0.5f, 60, 61.8f, 65.2f);
        UnityScene indoor21 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_21F", "Camera_Hotel_21F_Ortho", "h21f", "h21f_zone", new Vector3(0.0f, 74, 0.0f), false, true, 0.5f, 64, 64.8f, 68.4f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_22F", "Camera_Hotel_22F_Ortho", "h22f", "h22f_zone", new Vector3(0.0f, 77, 0.0f), false, true, 0.5f, 67, 68.2f, 71.6f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_23F", "Camera_Hotel_23F_Ortho", "h23f", "h23f_zone", new Vector3(0.0f, 80, 0.0f), false, true, 0.5f, 70, 71.2f, 74.8f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_24F", "Camera_Hotel_24F_Ortho", "h24f", "h24f_zone", new Vector3(0.0f, 83, 0.0f), false, true, 0.5f, 73, 74.4f, 78f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_25F", "Camera_Hotel_25F_Ortho", "h25f", "h25f_zone", new Vector3(0.0f, 86, 0.0f), false, true, 0.5f, 76, 77.5f, 81.2f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_26F", "Camera_Hotel_26F_Ortho", "h26f", "h26f_zone", new Vector3(0.0f, 90, 0.0f), false, true, 0.5f, 80, 80.9f, 84.4f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_27F", "Camera_Hotel_27F_Ortho", "h27f", "h27f_zone", new Vector3(0.0f, 93, 0.0f), false, true, 0.5f, 83, 84f, 87.6f);
        UnityScene indoor28 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_28F", "Camera_Hotel_28F_Ortho", "h28f", "h28f_zone", new Vector3(0.0f, 96, 0.0f), false, true, 0.5f, 86, 87f, 90.8f);
        UnityScene indoor29 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_29F", "Camera_Hotel_29F_Ortho", "h29f", "h29f_zone", new Vector3(0.0f, 99, 0.0f), false, true, 0.5f, 89, 90.1f, 94f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_29.5F", "Camera_Hotel_29.5F_Ortho", "h29_02f", "h29-2f_zone", new Vector3(0.0f, 102.5f, 0.0f), false, true, 0.5f, 92.5f, 93.3f, 97.2f, indoor29);
        UnityScene indoor30 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_30F", "Camera_Hotel_30F_Ortho", "h30f", "h30f_zone", new Vector3(0.0f, 106, 0.0f), false, true, 0.5f, 96, 96.2f, 100.4f);
        UnityScene indoor31 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_31F", "Camera_Hotel_31F_Ortho", "h31f", "h31f_zone", new Vector3(0.0f, 108, 0.0f), false, true, 0.5f, 98, 100.5f, 103.6f, indoor30);
        UnityScene indoor32 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_32F", "Camera_Hotel_32F_Ortho", "h32f", "h32f_zone", new Vector3(0.0f, 113, 0.0f), false, true, 0.5f, 103, 102.4f, 106.8f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_33F", "Camera_Hotel_33F_Ortho", "h33f", "h33f_zone", new Vector3(0.0f, 115.5f, 0.0f), false, true, 0.5f, 105.5f, 105.6f, 110f, indoor32);
        #endregion

        #region OfficeA
        UnityScene officeA1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_1F", "Camera_OfficeA_1F_Ortho", "t01_01f", "t01_01f_zone", new Vector3(75.0f, 2.07f, 0.0f), false, true, 0.5f, -10.0f, -2.0f, 4.5f);
        UnityScene officeA2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_2F", "Camera_OfficeA_2F_Ortho", "t01_02f", "t01_02f_zone", new Vector3(75.0f, 9.66f, 0.0f), false, true, 0.5f, 7f, 7.65184f, 15f);
        UnityScene officeA3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_3F", "Camera_OfficeA_3F_Ortho", "t01_03f", "t01_03f_zone", new Vector3(75.0f, 15.16f, 0.0f), false, true, 0.5f, 12.5f, 12.80184f, 20.5f);
        UnityScene officeA4 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_4F", "Camera_OfficeA_4F_Ortho", "t01_04f", "t01_04f_zone", new Vector3(75.0f, 20.16f, 0.0f), false, true, 0.5f, 17.5f, 17.95184f, 25.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_5F", "Camera_OfficeA_5F_Ortho", "t01_05f", "t01_05f_zone", new Vector3(75.0f, 25.16f, 0.0f), false, true, 0.5f, 22.5f, 23.10184f, 30.5f);
        UnityScene officeA6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_6F", "Camera_OfficeA_6F_Ortho", "t01_06f", "t01_06f_zone", new Vector3(75.0f, 30.16f, 0.0f), false, true, 0.5f, 27.5f, 28.25184f, 35.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_7F", "Camera_OfficeA_7F_Ortho", "t01_07f", "t01_07f_zone", new Vector3(75.0f, 35.16f, 0.0f), false, true, 0.5f, 32.5f, 33.40185f, 40.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_8F", "Camera_OfficeA_8F_Ortho", "t01_08f", "t01_08f_zone", new Vector3(75.0f, 40.16f, 0.0f), false, true, 0.5f, 37.5f, 38.55185f, 45.9f);
        UnityScene officeA9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_9F", "Camera_OfficeA_9F_Ortho", "t01_09f", "t01_09f_zone", new Vector3(75.0f, 45.66f, 0.0f), false, true, 0.5f, 43f, 43.70186f, 51.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_10F", "Camera_OfficeA_10F_Ortho", "t01_10f", "t01_10f_zone", new Vector3(75.0f, 50.66f, 0.0f), false, true, 0.5f, 48.5f, 48.85185f, 56.3f);
        UnityScene officeA11 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_11F", "Camera_OfficeA_11F_Ortho", "t01_11f", "t01_11f_zone", new Vector3(75.0f, 55.66f, 0.0f), false, true, 0.5f, 53.5f, 54.00185f, 61.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_12F", "Camera_OfficeA_12F_Ortho", "t01_12f", "t01_12f_zone", new Vector3(75.0f, 60.66f, 0.0f), false, true, 0.5f, 58.5f, 59.15187f, 66.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_13F", "Camera_OfficeA_13F_Ortho", "t01_13f", "t01_13f_zone", new Vector3(75.0f, 65.66f, 0.0f), false, true, 0.5f, 63.5f, 64.30188f, 71.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_14F", "Camera_OfficeA_14F_Ortho", "t01_14f", "t01_14f_zone", new Vector3(75.0f, 71.66f, 0.0f), false, true, 0.5f, 69f, 69.45189f, 77.1f);
        UnityScene officeA15 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_15F", "Camera_OfficeA_15F_Ortho", "t01_15f", "t01_15f_zone", new Vector3(75.0f, 76.66f, 0.0f), false, true, 0.5f, 74f, 74.60189f, 82.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_16F", "Camera_OfficeA_16F_Ortho", "t01_16f", "t01_16f_zone", new Vector3(75.0f, 81.66f, 0.0f), false, true, 0.5f, 79f, 79.75188f, 87.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_17F", "Camera_OfficeA_17F_Ortho", "t01_17f", "t01_17f_zone", new Vector3(75.0f, 86.66f, 0.0f), false, true, 0.5f, 84.5f, 84.90187f, 92.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_18F", "Camera_OfficeA_18F_Ortho", "t01_18f", "t01_18f_zone", new Vector3(75.0f, 91.66f, 0.0f), false, true, 0.5f, 89.5f, 90.05188f, 97.9f);
        UnityScene officeA19 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_19F", "Camera_OfficeA_19F_Ortho", "t01_19f", "t01_19f_zone", new Vector3(75.0f, 96.66f, 0.0f), false, true, 0.5f, 94.5f, 95.20188f, 103.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_20F", "Camera_OfficeA_20F_Ortho", "t01_20f", "t01_20f_zone", new Vector3(75.0f, 101.66f, 0.0f), false, true, 0.5f, 99.5f, 100.35185f, 108.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_21F", "Camera_OfficeA_21F_Ortho", "t01_21f", "t01_21f_zone", new Vector3(75.0f, 107.66f, 0.0f), false, true, 0.5f, 105f, 105.50185f, 113.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_22F", "Camera_OfficeA_22F_Ortho", "t01_22f", "t01_22f_zone", new Vector3(75.0f, 112.66f, 0.0f), false, true, 0.5f, 110f, 110.65185f, 118.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_23F", "Camera_OfficeA_23F_Ortho", "t01_23f", "t01_23f_zone", new Vector3(75.0f, 117.66f, 0.0f), false, true, 0.5f, 115.5f, 115.80185f, 123.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_24F", "Camera_OfficeA_24F_Ortho", "t01_24f", "t01_24f_zone", new Vector3(75.0f, 122.66f, 0.0f), false, true, 0.5f, 120f, 120.95675f, 129.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_25F", "Camera_OfficeA_25F_Ortho", "t01_25f", "t01_25f_zone", new Vector3(75.0f, 127.66f, 0.0f), false, true, 0.5f, 125f, 126.10185f, 134.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_26F", "Camera_OfficeA_26F_Ortho", "t01_26f", "t01_26f_zone", new Vector3(75.0f, 132.66f, 0.0f), false, true, 0.5f, 130f, 131.25185f, 139.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_27F", "Camera_OfficeA_27F_Ortho", "t01_27f", "t01_27f_zone", new Vector3(75.0f, 137.66f, 0.0f), false, true, 0.5f, 135f, 136.40195f, 144.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_28F", "Camera_OfficeA_28F_Ortho", "t01_28f", "t01_28f_zone", new Vector3(75.0f, 143.66f, 0.0f), false, true, 0.5f, 141f, 141.55185f, 149.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_29F", "Camera_OfficeA_29F_Ortho", "t01_29f", "t01_29f_zone", new Vector3(75.0f, 148.66f, 0.0f), false, true, 0.5f, 146f, 146.70185f, 155.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_30F", "Camera_OfficeA_30F_Ortho", "t01_30f", "t01_30f_zone", new Vector3(75.0f, 153.66f, 0.0f), false, true, 0.5f, 151f, 151.85185f, 160.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_31F", "Camera_OfficeA_31F_Ortho", "t01_31f", "t01_31f_zone", new Vector3(75.0f, 158.66f, 0.0f), false, true, 0.5f, 156f, 157.00185f, 165.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_32F", "Camera_OfficeA_32F_Ortho", "t01_32f", "t01_32f_zone", new Vector3(75.0f, 163.66f, 0.0f), false, true, 0.5f, 161f, 162.15185f, 170.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_33F", "Camera_OfficeA_33F_Ortho", "t01_33f", "t01_33f_zone", new Vector3(75.0f, 168.66f, 0.0f), false, true, 0.5f, 166f, 167.30185f, 175.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_34F", "Camera_OfficeA_34F_Ortho", "t01_34f", "t01_34f_zone", new Vector3(75.0f, 174.66f, 0.0f), false, true, 0.5f, 172f, 172.45185f, 181.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_35F", "Camera_OfficeA_35F_Ortho", "t01_35f", "t01_35f_zone", new Vector3(75.0f, 179.66f, 0.0f), false, true, 0.5f, 177f, 177.60185f, 186.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_36F", "Camera_OfficeA_36F_Ortho", "t01_36f", "t01_36f_zone", new Vector3(75.0f, 184.66f, 0.0f), false, true, 0.5f, 182f, 182.75195f, 191.5f);
        UnityScene officeA37 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_37F", "Camera_OfficeA_37F_Ortho", "t01_37f", "t01_37f_zone", new Vector3(75.0f, 189.66f, 0.0f), false, true, 0.5f, 187.5f, 187.90185f, 196.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_38F", "Camera_OfficeA_38F_Ortho", "t01_38f", "t01_38f_zone", new Vector3(75.0f, 194.66f, 0.0f), false, true, 0.5f, 192.5f, 193.05185f, 201.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_39F", "Camera_OfficeA_39F_Ortho", "t01_39f", "t01_39f_zone", new Vector3(75.0f, 199.66f, 0.0f), false, true, 0.5f, 198f, 198.20185f, 207.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_40F", "Camera_OfficeA_40F_Ortho", "t01_40f", "t01_40f_zone", new Vector3(75.0f, 204.66f, 0.0f), false, true, 0.5f, 203f, 203.35185f, 212.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_41F", "Camera_OfficeA_41F_Ortho", "t01_41f", "t01_41f_zone", new Vector3(75.0f, 210.66f, 0.0f), false, true, 0.5f, 208f, 208.50185f, 217.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_42F", "Camera_OfficeA_42F_Ortho", "t01_42f", "t01_42f_zone", new Vector3(75.0f, 215.66f, 0.0f), false, true, 0.5f, 213f, 213.65195f, 222.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_44F", "Camera_OfficeA_44F_Ortho", "t01_44f", "t01_43f_zone", new Vector3(75.0f, 225.66f, 0.0f), false, true, 0.5f, 223.5f, 223.95195f, 227.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_43F", "Camera_OfficeA_43F_Ortho", "t01_43f", "t01_44f_zone", new Vector3(75.0f, 220.66f, 0.0f), false, true, 0.5f, 218.5f, 218.80195f, 233.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_45F", "Camera_OfficeA_45F_Ortho", "t01_45f", "t01_45f_zone", new Vector3(75.0f, 230.66f, 0.0f), false, true, 0.5f, 228.5f, 229.10195f, 238.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_46F", "Camera_OfficeA_46F_Ortho", "t01_46f", "t01_46f_zone", new Vector3(75.0f, 235.66f, 0.0f), false, true, 0.5f, 234f, 233.7445f, 243.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_47F", "Camera_OfficeA_47F_Ortho", "t01_47f", "t01_47f_zone", new Vector3(75.0f, 241.66f, 0.0f), false, true, 0.5f, 239f, 239.40185f, 248.7f);

        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_48F", "Camera_OfficeA_48F_Ortho", "t01_48f", "t01_48f_zone", new Vector3(75.0f, 246.66f, 0.0f), false, true, 0.5f, 244f, 244.55185f, 253.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_49F", "Camera_OfficeA_49F_Ortho", "t01_49f", "t01_49f_zone", new Vector3(75.0f, 251.66f, 0.0f), false, true, 0.5f, 249f, 249.70185f, 259.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_50F", "Camera_OfficeA_50F_Ortho", "t01_50f", "t01_50f_zone", new Vector3(75.0f, 256.66f, 0.0f), false, true, 0.5f, 254.5f, 254.85185f, 264.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_51F", "Camera_OfficeA_51F_Ortho", "t01_51f", "t01_51f_zone", new Vector3(75.0f, 261.66f, 0.0f), false, true, 0.5f, 259.5f, 260.00185f, 269.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_52F", "Camera_OfficeA_52F_Ortho", "t01_52f", "t01_52f_zone", new Vector3(75.0f, 266.66f, 0.0f), false, true, 0.5f, 264.5f, 265.15185f, 274.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_53F", "Camera_OfficeA_53F_Ortho", "t01_53f", "t01_53f_zone", new Vector3(75.0f, 271.66f, 0.0f), false, true, 0.5f, 270f, 270.30195f, 279.9f);
        UnityScene officeA54 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_54F", "Camera_OfficeA_54F_Ortho", "t01_54f", "t01_54f_zone", new Vector3(75.0f, 277.66f, 0.0f), false, true, 0.5f, 275f, 275.45185f, 285.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_55F", "Camera_OfficeA_55F_Ortho", "t01_55f", "t01_55f_zone", new Vector3(75.0f, 282.66f, 0.0f), false, true, 0.5f, 280f, 280.60185f, 290.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_56F", "Camera_OfficeA_56F_Ortho", "t01_56f", "t01_56f_zone", new Vector3(75.0f, 287.66f, 0.0f), false, true, 0.5f, 285f, 285.75185f, 295.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_57F", "Camera_OfficeA_57F_Ortho", "t01_57f", "t01_57f_zone", new Vector3(75.0f, 292.66f, 0.0f), false, true, 0.5f, 290.5f, 290.90175f, 300.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_58F", "Camera_OfficeA_58F_Ortho", "t01_58f", "t01_58f_zone", new Vector3(75.0f, 297.66f, 0.0f), false, true, 0.5f, 295.5f, 296.05175f, 305.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_59F", "Camera_OfficeA_59F_Ortho", "t01_59f", "t01_59f_zone", new Vector3(75.0f, 302.66f, 0.0f), false, true, 0.5f, 301f, 301.20185f, 311.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_60F", "Camera_OfficeA_60F_Ortho", "t01_60f", "t01_60f_zone", new Vector3(75.0f, 307.66f, 0.0f), false, true, 0.5f, 306f, 306.35175f, 316.3f);

        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_61F", "Camera_OfficeA_61F_Ortho", "t01_61f", "t01_61f_zone", new Vector3(75.0f, 313.66f, 0.0f), false, true, 0.5f, 311f, 311.50175f, 321.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_62F", "Camera_OfficeA_62F_Ortho", "t01_62f", "t01_62f_zone", new Vector3(75.0f, 318.66f, 0.0f), false, true, 0.5f, 316f, 316.65175f, 326.7f);
        UnityScene officeA63 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_63F", "Camera_OfficeA_63F_Ortho", "t01_63f", "t01_63f_zone", new Vector3(75.0f, 323.66f, 0.0f), false, true, 0.5f, 321f, 321.80185f, 331.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_64F", "Camera_OfficeA_64F_Ortho", "t01_64f", "t01_64f_zone", new Vector3(75.0f, 328.66f, 0.0f), false, true, 0.5f, 326.5f, 326.95185f, 337.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_65F", "Camera_OfficeA_65F_Ortho", "t01_65f", "t01_65f_zone", new Vector3(75.0f, 333.66f, 0.0f), false, true, 0.5f, 331.5f, 332.10175f, 342.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_66F", "Camera_OfficeA_66F_Ortho", "t01_66f", "t01_66f_zone", new Vector3(75.0f, 338.66f, 0.0f), false, true, 0.5f, 336.5f, 337.25175f, 347.5f);

        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_67F", "Camera_OfficeA_67F_Ortho", "t01_67f", "t01_67f_zone", new Vector3(75.0f, 344.66f, 0.0f), false, true, 0.5f, 342f, 342.16175f, 352.7f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_68F", "Camera_OfficeA_68F_Ortho", "t01_68f", "t01_68f_zone", new Vector3(75.0f, 349.66f, 0.0f), false, true, 0.5f, 347f, 347.77675f, 357.9f);

        UnityScene officeA69 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeA_69F", "Camera_OfficeA_69F_Ortho", "t01_69f", "t01_69f_zone", new Vector3(75.0f, 351.66f, 0.0f), false, true, 0.5f, 348.5f, 353.50615f, 360f);
        #endregion

        #region OfficeB
        UnityScene officeB1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_1F", "Camera_OfficeB_1F_Ortho", "t02_01f", "t02_01f_zone", new Vector3(78.3f, 2.07f, -68.3f), false, true, 0.5f, -10.0f, -2.0f, 4.5f);
        UnityScene officeB2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_2F", "Camera_OfficeB_2F_Ortho", "t02_02f", "t02_02f_zone", new Vector3(78.3f, 9.66f, -68.3f), false, true, 0.5f, -10.0f, 7.5f, 15f);
        UnityScene officeB3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_3F", "Camera_OfficeB_3F_Ortho", "t02_03f", "t02_03f_zone", new Vector3(78.3f, 15.16f, -68.3f), false, true, 0.5f, -10.0f, 12.22f, 20f);
        UnityScene officeB4 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_4F", "Camera_OfficeB_4F_Ortho", "t02_04f", "t02_04f_zone", new Vector3(78.3f, 20.16f, -68.3f), false, true, 0.5f, -10.0f, 16.75f, 24.5f);
        UnityScene officeB5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_5F", "Camera_OfficeB_5F_Ortho", "t02_05f", "t02_05f_zone", new Vector3(78.3f, 25.16f, -68.3f), false, true, 0.5f, -10.0f, 21.28f, 29f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_6F", "Camera_OfficeB_6F_Ortho", "t02_06f", "t02_06f_zone", new Vector3(78.3f, 30.16f, -68.3f), false, true, 0.5f, -10.0f, 25.81f, 33.5f);
        UnityScene officeB7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_7F", "Camera_OfficeB_7F_Ortho", "t02_07f", "t02_07f_zone", new Vector3(78.3f, 35.16f, -68.3f), false, true, 0.5f, -10.0f, 30.34f, 38f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_8F", "Camera_OfficeB_8F_Ortho", "t02_08f", "t02_08f_zone", new Vector3(78.3f, 40.16f, -68.3f), false, true, 0.5f, -10.0f, 34.87f, 42.5f);
        UnityScene officeB9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_9F", "Camera_OfficeB_9F_Ortho", "t02_09f", "t02_09f_zone", new Vector3(78.3f, 45.66f, -68.3f), false, true, 0.5f, -10.0f, 39.4f, 47f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_10F", "Camera_OfficeB_10F_Ortho", "t02_10f", "t02_10f_zone", new Vector3(78.3f, 50.66f, -68.3f), false, true, 0.5f, -10.0f, 43.93f, 51.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_11F", "Camera_OfficeB_11F_Ortho", "t02_11f", "t02_11f_zone", new Vector3(78.3f, 55.66f, -68.3f), false, true, 0.5f, -10.0f, 48.46f, 55f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_12F", "Camera_OfficeB_12F_Ortho", "t02_12f", "t02_12f_zone", new Vector3(78.3f, 60.66f, -68.3f), false, true, 0.5f, -10.0f, 52.99f, 59.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_13F", "Camera_OfficeB_13F_Ortho", "t02_13f", "t02_13f_zone", new Vector3(78.3f, 65.66f, -68.3f), false, true, 0.5f, -10.0f, 57.32f, 64f);
        UnityScene officeB14 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_14F", "Camera_OfficeB_14F_Ortho", "t02_14f", "t02_14f_zone", new Vector3(78.3f, 71.66f, -68.3f), false, true, 0.5f, -10.0f, 61.9f, 68.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_15F", "Camera_OfficeB_15F_Ortho", "t02_15f", "t02_15f_zone", new Vector3(78.3f, 76.66f, -68.3f), false, true, 0.5f, -10.0f, 66.31f, 73f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_16F", "Camera_OfficeB_16F_Ortho", "t02_16f", "t02_16f_zone", new Vector3(78.3f, 81.66f, -68.3f), false, true, 0.5f, -10.0f, 70.82f, 77.5f);
        UnityScene officeB17 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_17F", "Camera_OfficeB_17F_Ortho", "t02_17f", "t02_17f_zone", new Vector3(78.3f, 86.66f, -68.3f), false, true, 0.5f, -10.0f, 75.3f, 82f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_18F", "Camera_OfficeB_18F_Ortho", "t02_18f", "t02_18f_zone", new Vector3(78.3f, 91.66f, -68.3f), false, true, 0.5f, -10.0f, 79.8f, 86.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_19F", "Camera_OfficeB_19F_Ortho", "t02_19f", "t02_19f_zone", new Vector3(78.3f, 96.66f, -68.3f), false, true, 0.5f, -10.0f, 84.3f, 91f);
        UnityScene officeB20 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_20F", "Camera_OfficeB_20F_Ortho", "t02_20f", "t02_20f_zone", new Vector3(78.3f, 101.66f, -68.3f), false, true, 0.5f, -10.0f, 88.8f, 95.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_21F", "Camera_OfficeB_21F_Ortho", "t02_21f", "t02_21f_zone", new Vector3(78.3f, 107.66f, -68.3f), false, true, 0.5f, -10.0f, 93.3f, 100f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_22F", "Camera_OfficeB_22F_Ortho", "t02_22f", "t02_22f_zone", new Vector3(78.3f, 112.66f, -68.3f), false, true, 0.5f, -10.0f, 97.8f, 104.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_23F", "Camera_OfficeB_23F_Ortho", "t02_23f", "t02_23f_zone", new Vector3(78.3f, 117.66f, -68.3f), false, true, 0.5f, -10.0f, 102.1f, 109f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_24F", "Camera_OfficeB_24F_Ortho", "t02_24f", "t02_24f_zone", new Vector3(78.3f, 122.66f, -68.3f), false, true, 0.5f, -10.0f, 106.6f, 113.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_25F", "Camera_OfficeB_25F_Ortho", "t02_25f", "t02_25f_zone", new Vector3(78.3f, 127.66f, -68.3f), false, true, 0.5f, -10.0f, 111.1f, 118f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_26F", "Camera_OfficeB_26F_Ortho", "t02_26f", "t02_26f_zone", new Vector3(78.3f, 132.66f, -68.3f), false, true, 0.5f, -10.0f, 115.6f, 122.5f);

        UnityScene officeB27 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_27F", "Camera_OfficeB_27F_Ortho", "t02_27f", "t02_27f_zone", new Vector3(78.3f, 137.66f, -68.3f), false, true, 0.5f, -10.0f, 120.1f, 127f);
        UnityScene officeB28 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_28F", "Camera_OfficeB_28F_Ortho", "t02_28f", "t02_28f_zone", new Vector3(78.3f, 143.66f, -68.3f), false, true, 0.5f, -10.0f, 124.6f, 131.5f);
        UnityScene officeB29 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_29F", "Camera_OfficeB_29F_Ortho", "t02_29f", "t02_29f_zone", new Vector3(78.3f, 148.66f, -68.3f), false, true, 0.5f, -10.0f, 129.1f, 136f);
        UnityScene officeB30 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_30F", "Camera_OfficeB_30F_Ortho", "t02_30f", "t02_30f_zone", new Vector3(78.3f, 153.66f, -68.3f), false, true, 0.5f, -10.0f, 133.6f, 140.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_31F", "Camera_OfficeB_31F_Ortho", "t02_31f", "t02_31f_zone", new Vector3(78.3f, 158.66f, -68.3f), false, true, 0.5f, -10.0f, 138.1f, 145f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_32F", "Camera_OfficeB_32F_Ortho", "t02_32f", "t02_32f_zone", new Vector3(78.3f, 163.66f, -68.3f), false, true, 0.5f, -10.0f, 142.6f, 149.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_33F", "Camera_OfficeB_33F_Ortho", "t02_33f", "t02_33f_zone", new Vector3(78.3f, 168.66f, -68.3f), false, true, 0.5f, -10.0f, 147.1f, 154f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_34F", "Camera_OfficeB_34F_Ortho", "t02_34f", "t02_34f_zone", new Vector3(78.3f, 174.66f, -68.3f), false, true, 0.5f, -10.0f, 151.6f, 158.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_35F", "Camera_OfficeB_35F_Ortho", "t02_35f", "t02_35f_zone", new Vector3(78.3f, 179.66f, -68.3f), false, true, 0.5f, -10.0f, 156.1f, 163f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_36F", "Camera_OfficeB_36F_Ortho", "t02_36f", "t02_36f_zone", new Vector3(78.3f, 184.66f, -68.3f), false, true, 0.5f, -10.0f, 160.6f, 167.5f);
        UnityScene officeB37 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_37F", "Camera_OfficeB_37F_Ortho", "t02_37f", "t02_37f_zone", new Vector3(78.3f, 189.66f, -68.3f), false, true, 0.5f, -10.0f, 165.7f, 172f);
        UnityScene officeB38 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_38F", "Camera_OfficeB_38F_Ortho", "t02_38f", "t02_38f_zone", new Vector3(78.3f, 194.66f, -68.3f), false, true, 0.5f, -10.0f, 170.2f, 176.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_39F", "Camera_OfficeB_39F_Ortho", "t02_39f", "t02_39f_zone", new Vector3(78.3f, 199.66f, -68.3f), false, true, 0.5f, -10.0f, 174.8f, 181f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_40F", "Camera_OfficeB_40F_Ortho", "t02_40f", "t02_40f_zone", new Vector3(78.3f, 204.66f, -68.3f), false, true, 0.5f, -10.0f, 179.4f, 185.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_41F", "Camera_OfficeB_41F_Ortho", "t02_41f", "t02_41f_zone", new Vector3(78.3f, 210.66f, -68.3f), false, true, 0.5f, -10.0f, 183.9f, 190f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_42F", "Camera_OfficeB_42F_Ortho", "t02_42f", "t02_42f_zone", new Vector3(78.3f, 215.66f, -68.3f), false, true, 0.5f, -10.0f, 188.4f, 194.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_43F", "Camera_OfficeB_43F_Ortho", "t02_43f", "t02_43f_zone", new Vector3(78.3f, 220.66f, -68.3f), false, true, 0.5f, -10.0f, 192.9f, 199f);
        UnityScene officeB44 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_44F", "Camera_OfficeB_44F_Ortho", "t02_44f", "t02_44f_zone", new Vector3(78.3f, 225.66f, -68.3f), false, true, 0.5f, -10.0f, 197.4f, 203.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_45F", "Camera_OfficeB_45F_Ortho", "t02_45f", "t02_45f_zone", new Vector3(78.3f, 230.66f, -68.3f), false, true, 0.5f, -10.0f, 201.9f, 208f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_46F", "Camera_OfficeB_46F_Ortho", "t02_46f", "t02_46f_zone", new Vector3(78.3f, 235.66f, -68.3f), false, true, 0.5f, -10.0f, 206.4f, 212.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_47F", "Camera_OfficeB_47F_Ortho", "t02_47f", "t02_47f_zone", new Vector3(78.3f, 241.66f, -68.3f), false, true, 0.5f, -10.0f, 210.9f, 217f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_48F", "Camera_OfficeB_48F_Ortho", "t02_48f", "t02_48f_zone", new Vector3(78.3f, 246.66f, -68.3f), false, true, 0.5f, -10.0f, 215.4f, 221.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_49F", "Camera_OfficeB_49F_Ortho", "t02_49f", "t02_49f_zone", new Vector3(78.3f, 251.66f, -68.3f), false, true, 0.5f, -10.0f, 219.9f, 226f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_50F", "Camera_OfficeB_50F_Ortho", "t02_50f", "t02_50f_zone", new Vector3(78.3f, 256.66f, -68.3f), false, true, 0.5f, -10.0f, 224.3f, 230.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_51F", "Camera_OfficeB_51F_Ortho", "t02_51f", "t02_51f_zone", new Vector3(78.3f, 261.66f, -68.3f), false, true, 0.5f, -10.0f, 228.8f, 235f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_52F", "Camera_OfficeB_52F_Ortho", "t02_52f", "t02_52f_zone", new Vector3(78.3f, 266.66f, -68.3f), false, true, 0.5f, -10.0f, 233.1f, 239.5f);
        UnityScene officeB53 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_OfficeB_53F", "Camera_OfficeB_53F_Ortho", "t02_53f", "t02_53f_zone", new Vector3(78.3f, 271.66f, -68.3f), false, true, 0.5f, -10.0f, 237.5f, 244f);


        #endregion

        #region Retail
        UnityScene r1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_1F", "Camera_Retail_1F_Ortho", "retail_01f", "retail_01f_zone", new Vector3(35.7f, 2.07f, -15.0f), false, true, 0.5f, -10.0f, 23.4f, 32f);
        UnityScene r2 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_2F", "Camera_Retail_2F_Ortho", "retail_02f", "retail_02f_zone", new Vector3(35.7f, 9.66f, -15.0f), false, true, 0.5f, -10.0f, 29.4f, 38f);
        UnityScene r3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_3F", "Camera_Retail_3F_Ortho", "retail_03f", "retail_03f_zone", new Vector3(35.7f, 15.16f, -15.0f), false, true, 0.5f, -10.0f, 34.9f, 44f);
        UnityScene r4 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_4F", "Camera_Retail_4F_Ortho", "retail_04f", "retail_04f_zone", new Vector3(35.7f, 20.16f, -15.0f), false, true, 0.5f, -10.0f, 26f, 32f);
        UnityScene r5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_5F", "Camera_Retail_5F_Ortho", "retail_05f", "retail_05f_zone", new Vector3(35.7f, 25.16f, -15.0f), false, true, 0.5f, -10.0f, 34.5f, 38f);
        UnityScene r6 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_6F", "Camera_Retail_6F_Ortho", "retail_06f", "retail_06f_zone", new Vector3(35.7f, 30.16f, -15.0f), false, true, 0.5f, -10.0f, 52.9f, 62f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_7F", "Camera_Retail_7F_Ortho", "retail_07f", "retail_07f_zone", new Vector3(35.7f, 35.16f, -15.0f), false, true, 0.5f, -10.0f, 60.4f, 68f);
        UnityScene r8 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Retail_8F", "Camera_Retail_8F_Ortho", "retail_08f", "retail_08f_zone", new Vector3(35.7f, 40.16f, -15.0f), false, true, 0.5f, -10.0f, 64.6f, 74f);
        #endregion

        #region 지하
        List<CameraData> undergroundCameras = MakeUndergroundCameraDatas();
        List<CameraData> undergroundOrthoCameras = MakeUndergroundOrthoCameraDatas();

        UnityScene b1f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[0], undergroundOrthoCameras[0], "b01f", "b01f_zone", new Vector3(-288, 91, -651), false, true, 0.5f, -10.0f, 14.4f, 20f);
        UnityScene b2f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[1], undergroundOrthoCameras[1], "b02f", "b02f_zone", new Vector3(-288, 77, -651), false, true, 0.5f, -10.0f, 0.07f, 7f);
        UnityScene b3f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[2], undergroundOrthoCameras[2], "b03f", "b03f_zone", new Vector3(-288, 67, -651), false, true, 0.5f, -10.0f, -6.9f, -2f);
        UnityScene b4f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[3], undergroundOrthoCameras[3], "b04f", "b04f_zone", new Vector3(-288, 57, -651), false, true, 0.5f, -10.0f, -0.5f, 5f);
        UnityScene b5f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[4], undergroundOrthoCameras[4], "b05f", "b05f_zone", new Vector3(-288, 50, -651), false, true, 0.5f, -10.0f, 2.7f, 8f);
        UnityScene b6f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[5], undergroundOrthoCameras[5], "b06f", "b06f_zone", new Vector3(-288, 41, -651), false, true, 0.5f, -10.0f, -0.2f, 4f);
        UnityScene b7f = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[6], undergroundOrthoCameras[6], "b07f", "b07f_zone", new Vector3(-288, 21, -651), false, true, 0.5f, -10.0f, -2.8f, 5f);
        UnityScene b7upf = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[6], undergroundOrthoCameras[6], "b07f_up", "b07f_up_zone", new Vector3(-288, 21, -651), false, true, 0.5f, -10.0f, -1.5f, 6.5f);
        UnityScene bf = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, undergroundCameras[7], undergroundOrthoCameras[7], "path_of_subway", "b02_sub_zone", new Vector3(-318, 77, -331), false, true, 0.5f, -10.0f, 71.2f, 84f);
        #endregion

        ModelManager.Instance.Model = this;

        //SetEditMode(true);
        //m_bEditMode = true;
        //m_bWallEditMode = true;
        //ChangeScene(officeB7, false);
        //ChangeScene(indoorUnderground, false);
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

    private void AddIcon()
    {        
        int x = (int)Input.mousePosition.x;
        int y = (int)Input.mousePosition.y;
        
        Vector3 v = ScreenToWorldForPOI(x, y);

        //m_strTargetEditIconType = "Fire";
        //m_nNewIconID = 1;

        EffectPOI fireEffct = null;
        CanvasPOI poi = CanvasPOI.MakeInstance(v, m_strTargetEditIconType, m_strTargetEditIconType, m_nNewIconID, ref fireEffct);

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
    
    public void AddWall()
    {
        if (m_bEditMode)
            CustomizingController.Instance.BT_SpawnWall();
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

    public void ChangeSpaceText()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = "SendMessage('ChangeSpaceText()')";
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

    public void GetSpaceTexts(string path)
    {
        CustomSpaceText.Instance.GetSpaceText(path);
    }

    public void LoadSpaceTexts(string path, string sceneName)
    {
        CustomSpaceText.Instance.LoadSpaceText(path, sceneName);
    }

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