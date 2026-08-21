using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Assets;
using System.Collections.Concurrent;

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
        "VisibleViewButton"
    };


    public bool m_bOrbitMode = false;
    public bool m_bTranslateMode = false;
    public bool m_bPickMode = false;

    public Camera m_mainCamera = null;
    public Color color = Color.green;
    //public Collider coll;

    public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, ZOOM, ADD_ICON, MOVE_ICON, DELETE_ICON };
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
    private UnityScene m_currentScene = null;
    
    private GameObject m_bottomObject = null;
    private bool m_rotatePOI = false;
    
    private MouseWorkMode m_nMode = MouseWorkMode.NONE;
    private CameraData m_initData = null;

    #region Blink
    private ConcurrentDictionary<GameObject, GameObject> m_dicActiveAlarmZones = new ConcurrentDictionary<GameObject, GameObject>();

    private bool m_hideAlarm = true;
    private float m_fAlarmZoneTime = 0.0f;
    private int m_nAlarmZoneData = 1;
    #endregion

    #region Orthographic
    private int m_nInitOrthoSize = 25;
    private int m_nOrthoMin = 5;
    private int m_nOrthoMax = 60;
    #endregion

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
        set { m_bEditMode = value; }
    }

    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            proxy.UserObject.SetVariable("SetEditMode", new Action<bool>(SetEditMode));
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

    public void SetEditMode(bool bEditMode)
    {
        if (m_bEditMode == bEditMode)
            return;

        m_bEditMode = bEditMode;

        if (m_bEditMode)
        {
            m_mainCamera.orthographic = true;
            m_mainCamera.orthographicSize = m_nInitOrthoSize;
            m_editModeType = EditModeType.None;
            m_strTargetEditIconType = "";
            m_nNewIconID = -1;

            m_rotatePOI = true;
        }
        else
        {
            m_mainCamera.orthographic = false;
            m_rotatePOI = false;
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
        m_bPickMode = bFalse;
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
        v.y = m_mainCamera.transform.localPosition.y - 39.6f;
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
                m_bTranslateMode = false;
                m_bOrbitMode = false;
                m_bPickMode = false;
                break;
            case MouseWorkMode.PANNING: // Pan Mode
                m_bTranslateMode = true;
                m_bOrbitMode = false;
                m_bPickMode = false;
                break;
            case MouseWorkMode.ORBIT: // Orbit Mode
                m_bTranslateMode = false;
                m_bOrbitMode = true;
                m_bPickMode = false;
                break;
            case MouseWorkMode.PICK: // Pick Mode
                m_bPickMode = true;
                m_bTranslateMode = false;
                m_bOrbitMode = false;
                break;
            default:
                m_bTranslateMode = false;
                m_bOrbitMode = false;
                m_bPickMode = false;
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

            scene.OrthoCameraData = orthoCameraData;
            orthoCamera.SetActive(isMainCamera);
            Debug.Log("OrthoCamera.Visible : " + strOrthoCameraName + ", " + isMainCamera);
        }

        scene.CameraData = cameraData;
        camera.SetActive(isMainCamera);
        Debug.Log("Camera.Visible : " + strCameraName + ", " + isMainCamera);

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

    private UnityScene SetIndoorScene(Dictionary<string, UnityScene> dicScenes, Dictionary<string, GameObject> dicCameraDatas, string strCameraName, string strOrthoCameraName, string strModelName, string strAlarmModelName, Vector3 vOrbitCenter, bool isMainCamera, bool isActiveModel, float fPanningScale, float fBottomElevation, float detectBottom, UnityScene otherScene = null, UnityScene.SceneOption option = UnityScene.SceneOption.None)
    {
        UnityScene scene = SetOutdoorScene(dicScenes, dicCameraDatas, strCameraName, strOrthoCameraName, strModelName, vOrbitCenter, isMainCamera, isActiveModel, fPanningScale, fBottomElevation, option);

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
        }

        return scene;
    }

    private void AddSceneChild(UnityScene scene, GameObject obj)
    {
        int nChildCount = obj.transform.childCount;

        if (nChildCount == 0)
        {
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
    }

    public void ShowAlarmZone(string strZoneName, bool hideAllOthers)
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
        
        if (scene != null)
        {
            if (m_bEditMode && isOutdoor == false)
            {
                m_mainCamera.transform.localPosition = scene.OrthoCameraData.LocalPosition;
                m_mainCamera.transform.localEulerAngles = scene.OrthoCameraData.LocalEulerAngle;
                m_mainCamera.transform.localScale = scene.OrthoCameraData.LocalScale;
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

        m_vPanMovedFromFirstPosition = Vector3.zero;
        //m_rotatePOI = true;

        if (m_bEditMode == false)
        {
            // 카메라 초기화를 위하여 살짝 회전시킨다.
            SetInitData();
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
        MainModel.WriteLog("MainModel.Awake");
        MakeCommandMap();

        if (m_mainCamera == null)
            m_mainCamera = Camera.main;

        Dictionary<string, GameObject> dicCameraDatas = new Dictionary<string, GameObject>();

        m_bottomObject = GameObject.Find("Plane");
        
        UnityScene outdoor = SetOutdoorScene(m_dicSceneOutdoors, dicCameraDatas, "Outdoor_Camera", "", "Outdoor_Model", new Vector3(-172.1f, 30.9f, 623.9f), false, false, 0.5f, -1.1f);
        UnityScene indoor1 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_1F", "Camera_Hotel_1F_Ortho", "h01f", "h01f_zone", new Vector3(0.0f, 2.07f, 0.0f), false, true, 0.5f, -8.5f, -0.3f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_2F", "Camera_Hotel_2F_Ortho", "h02f", "h02f_zone", new Vector3(0.0f, 3.68f, 0.0f), false, true, 0.5f, 2.5f, 6.1f, indoor1);
        UnityScene indoor3 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_3F", "Camera_Hotel_3F_Ortho", "h03f", "h03f_zone", new Vector3(0.0f, 9.13f, 0.0f), false, true, 0.5f, 5.5f, 6.1f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_4F", "Camera_Hotel_4F_Ortho", "h04f", "h04f_zone", new Vector3(0.0f, 10.08f, 0.0f), false, true, 0.5f, 9, 12.5f, indoor3);
        UnityScene indoor5 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_5F", "Camera_Hotel_5F_Ortho", "h05f", "h05f_zone", new Vector3(0.0f, 15.18f, 0.0f), false, true, 0.5f, 12, 12.5f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_6F", "Camera_Hotel_6F_Ortho", "h06f", "h06f_zone", new Vector3(0.0f, 16.33f, 0.0f), false, true, 0.5f, 15, 18.9f, indoor5);
        UnityScene indoor7 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_7F", "Camera_Hotel_7F_Ortho", "h07f", "h07f_zone", new Vector3(0.0f, 21.93f, 0.0f), false, true, 0.5f, 18, 18.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_8F", "Camera_Hotel_8F_Ortho", "h08f", "h08f_zone", new Vector3(0.0f, 22.73f, 0.0f), false, true, 0.5f, 22, 25.6f, indoor7);
        UnityScene indoor9 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_9F", "Camera_Hotel_9F_Ortho", "h09f", "h09f_zone", new Vector3(0.0f, 27.7f, 0.0f), false, true, 0.5f, 24, 25.6f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_10F", "Camera_Hotel_10F_Ortho", "h10f", "h10f_zone", new Vector3(0.0f, 29.13f, 0.0f), false, true, 0.5f, 25.5f, 28.8f, indoor9);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_11F", "Camera_Hotel_11F_Ortho", "h11f", "h11f_zone", new Vector3(0.0f, 33.82f, 0.0f), false, true, 0.5f, 32, 31.668f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_12F", "Camera_Hotel_12F_Ortho", "h12f", "h12f_zone", new Vector3(0.0f, 37.02f, 0.0f), false, true, 0.5f, 35, 34.868f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_13F", "Camera_Hotel_13F_Ortho", "h13f", "h13f_zone", new Vector3(0.0f, 40.24f, 0.0f), false, true, 0.5f, 38, 38.085f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_14F", "Camera_Hotel_14F_Ortho", "h14f", "h14f_zone", new Vector3(0.0f, 43.44f, 0.0f), false, true, 0.5f, 41, 41.284f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_15F", "Camera_Hotel_15F_Ortho", "h15f", "h15f_zone", new Vector3(0.0f, 46.63f, 0.0f), false, true, 0.5f, 44, 44.472f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_16F", "Camera_Hotel_16F_Ortho", "h16f", "h16f_zone", new Vector3(0.0f, 49.84f, 0.0f), false, true, 0.5f, 47, 47.683f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_17F", "Camera_Hotel_17F_Ortho", "h17f", "h17f_zone", new Vector3(0.0f, 53.06f, 0.0f), false, true, 0.5f, 50, 50.9f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_18F", "Camera_Hotel_18F_Ortho", "h18f", "h18f_zone", new Vector3(0.0f, 56.25f, 0.0f), false, true, 0.5f, 53, 54.097f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_19F", "Camera_Hotel_19F_Ortho", "h19f", "h19f_zone", new Vector3(0.0f, 59.45f, 0.0f), false, true, 0.5f, 57, 57.294f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_20F", "Camera_Hotel_20F_Ortho", "h20f", "h20f_zone", new Vector3(0.0f, 62.65f, 0.0f), false, true, 0.5f, 60, 60.492f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_21F", "Camera_Hotel_21F_Ortho", "h21f", "h21f_zone", new Vector3(0.0f, 65.8f, 0.0f), false, true, 0.5f, 64, 63.646f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_22F", "Camera_Hotel_22F_Ortho", "h22f", "h22f_zone", new Vector3(0.0f, 68.99f, 0.0f), false, true, 0.5f, 67, 66.832f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_23F", "Camera_Hotel_23F_Ortho", "h23f", "h23f_zone", new Vector3(0.0f, 72.19f, 0.0f), false, true, 0.5f, 70, 70.032f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_24F", "Camera_Hotel_24F_Ortho", "h24f", "h24f_zone", new Vector3(0.0f, 75.39f, 0.0f), false, true, 0.5f, 73, 73.23f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_25F", "Camera_Hotel_25F_Ortho", "h25f", "h25f_zone", new Vector3(0.0f, 78.58f, 0.0f), false, true, 0.5f, 76, 76.428f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_26F", "Camera_Hotel_26F_Ortho", "h26f", "h26f_zone", new Vector3(0.0f, 81.78f, 0.0f), false, true, 0.5f, 80, 79.626f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_27F", "Camera_Hotel_27F_Ortho", "h27f", "h27f_zone", new Vector3(0.0f, 84.48f, 0.0f), false, true, 0.5f, 83, 82.785f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_28F", "Camera_Hotel_28F_Ortho", "h28f", "h28f_zone", new Vector3(0.0f, 88.18f, 0.0f), false, true, 0.5f, 86, 86.027f);
        UnityScene indoor29 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_29F", "Camera_Hotel_29F_Ortho", "h29f", "h29f_zone", new Vector3(0.0f, 91.63f, 0.0f), false, true, 0.5f, 89, 88.927f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_29.5F", "Camera_Hotel_29.5F_Ortho", "h29_02f", "h29-2f_zone", new Vector3(0.0f, 94.51f, 0.0f), false, true, 0.5f, 92.5f, 93.627f, indoor29);
        UnityScene indoor30 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_30F", "Camera_Hotel_30F_Ortho", "h30f", "h30f_zone", new Vector3(0.0f, 96.83f, 0.0f), false, true, 0.5f, 96, 93.627f);
        SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_31F", "Camera_Hotel_31F_Ortho", "h31f", "h31f_zone", new Vector3(0.0f, 101.31f, 0.0f), false, true, 0.5f, 98, 99.252f, indoor30);
        UnityScene indoor32 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_32F", "Camera_Hotel_32F_Ortho", "h32f", "h32f_zone", new Vector3(0.0f, 105.14f, 0.0f), false, true, 0.5f, 103, 102.525f);
        UnityScene indoor =SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_33F", "Camera_Hotel_33F_Ortho", "h33f", "h33f_zone", new Vector3(0.0f, 108.66f, 0.0f), false, true, 0.5f, 105.5f, 104.214f, indoor32);

        ChangeScene(indoor1, false);
        //ChangeScene(outdoor, true);
        
        ModelManager.Instance.Model = this;

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

        if (m_hasFocus == false)
            return;

        CheckInitData();

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
                m_initData = null;
            }
        }
    }

    private void BlinkAlarmZone()
    {
        ICollection<GameObject> activeAlarmZones = m_dicActiveAlarmZones.Values;

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

        m_mainCamera.transform.position = vPos;

        if (m_vOrbitCenter != Vector3.zero)
        {
            m_mainCamera.transform.LookAt(m_vOrbitCenter);
        }
        else
        {
            m_mainCamera.transform.LookAt(rCenter);
            cCalc = rCenter;
        }
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

    private void Zoom(float zDelta = 0.0f)
    {
        if (m_bEditMode)
        {
            if (zDelta == 0.0f)
                zDelta = Input.GetAxis("Mouse ScrollWheel");

            if (zDelta < 0)
            {
                float fSize = m_mainCamera.orthographicSize;
                fSize += 1;

                if (fSize > m_nOrthoMax)
                    fSize = m_nOrthoMax;

                m_mainCamera.orthographicSize = fSize;
            }
            else if (zDelta > 0)
            {
                float fSize = m_mainCamera.orthographicSize;
                fSize -= 1;

                if (fSize < m_nOrthoMin)
                    fSize = m_nOrthoMin;

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

            float fZoomDistance = 14.0f;

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

            if (m_bPickMode)
                return PickMode();
            else if (m_bOrbitMode && m_bEditMode == false)
                return OrbitMode(MouseState.LButtonDown);
            else if (m_bTranslateMode)
                return PanningMode(MouseState.LButtonDown);
        }
        else if (m_bEditMode == false && Input.GetMouseButtonDown(MOUSE_RIGHT))
        {
            return OrbitMode(MouseState.RButtonDown);
            //SelectScene("b05f");
            //WriteCameraState();
            //SelectScene("01f");
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

        IconPOI poi = POIManager.Instance.AddIcon(m_strTargetEditIconType, m_nNewIconID, v);

        if (poi != null)
        {
            m_nNewIconID++;
            m_editModeType = EditModeType.MoveIcon;

            Vector3 vPos = poi.Position;
            string szMsg = string.Format("OnAddPOI('{0}_{1}',{2},{3},{4})", poi.IconType, poi.ID, vPos.x, vPos.y, vPos.z);
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
    void OnApplicationFocus(bool hasFocus)
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

                Debug.Log("m_fPanningScale: " + m_fPanningScale);
                Debug.Log("SetOrigin, m_vHorzUnit : " + m_vHorzUnit.x + ", " + m_vHorzUnit.y + ", " + m_vHorzUnit.z);

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