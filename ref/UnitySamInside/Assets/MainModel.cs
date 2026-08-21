using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

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
    public bool m_bOrbitMode = false;
    private bool m_bDragged = false;
    public bool m_bTranslateMode = false;
    public bool m_bPickMode = false;

    private bool m_bZoominng = false;
    private bool m_bPanning = false;

    private Vector3 m_MousePosCur;
    private Vector3 m_MousePosStart;
    private Vector3 m_MousePosPrev;

    private Vector3 mTranslatePt;
    private Vector3 mTranslateStartPt;
    private Vector3 m_ScreenCenter;

    private Vector3 mOrbitCenter;
    private float m_RotRatioX = 0.0125f;
    private float m_RotRatioY = 0.0125f;

    private Vector3 mZoomCenter;
    private float m_fZoomDistance = 140.0f;

    public Camera m_MainCamera;
    public Color color = Color.green;
    public Collider coll;

    public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };

    private MouseWorkMode m_nMode = MouseWorkMode.NONE;

    public MouseWorkMode Mode
    {
        get { return m_nMode; }
    }

    private SharedMemory.BufferReadWrite file = null;
   
    public Bounds mainBound;
    public Bounds modelBound;

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
            proxy.UserObject.SetVariable("SetMode", new Action<int, bool>(SetMode));
            proxy.UserObject.SetVariable("Get3DPosition", new Action<int, int>(Get3DPosition));
            proxy.UserObject.SetVariable("UpdateAliasNames", new Action(UpdateAliasName));
            proxy.UserObject.SetVariable("ClearAllSelect", new Action(ClearSelect));
            proxy.UserObject.SetVariable("Get2DPosition", new Action<int, float, float, float>(Get2DPosition));

            proxy.UserObject.SetVariable("ModelZoom", new Action<float>(ManualZoom));


            proxy.UserObject.SetVariable("SaveScreenShot", new Action<string>(SaveScreenShot));
        }
    }
    public void SetEditMode(bool bEditMode)
    {
        m_bEditMode = bEditMode;
    }
    public void SaveSharedFile(int bSet, int nID, bool bbb)
    {
        INFO_POI info = new INFO_POI();
        info.bSet = bSet;
        info.nID = nID;
        if (file != null)
        {
            file.Write<INFO_POI>(ref info);
        }
    }
    public void SaveSharedFile()
    {
        INFO_POI info = new INFO_POI();
        info.bSet = 1;
        if (file != null)
        {
            file.Write<INFO_POI>(ref info);
        }
    }
    public void SaveSharedFile(Vector3 vec)
    {
        INFO_POI info = new INFO_POI();
        info.bSet = 1;
        info.x = vec.x;
        info.y = vec.y;
        info.z = vec.z;
        if (file != null)
        {
            file.Write<INFO_POI>(ref info);
        }
    }

    public void SaveSharedFile(float x, float y, float z)
    {
        INFO_POI info = new INFO_POI();
        info.bSet = 10;
        info.x = x;
        info.y = y;
        info.z = z;
        if (file != null)
        {
            file.Write<INFO_POI>(ref info);
        }
    }
    public void SaveSharedFile(int x, int y)
    {
        INFO_POI info = new INFO_POI();
        info.bSet = 1;
        info.nx = x;
        info.ny = y;
        if (file != null)
        {
            file.Write<INFO_POI>(ref info);
        }
    }
    public void SaveSharedFile(int bSelect)
    {
        INFO_POI info = new INFO_POI();
        info.bSet = 1;
        info.bSelect = bSelect;
        if (file != null)
        {
            file.Write<INFO_POI>(ref info);
        }
    }


    void SaveScreenShot(string szPath)
    {
        Debug.logger.Log(szPath);
        Application.CaptureScreenshot(szPath);
        SaveSharedFile();

        Debug.logger.Log("Saved Screen shot");
    }

    void Get2DPosition(int nTag, float x, float y, float z)
    {
        Vector3 m3DPosition = new Vector3(x, y, z);
        Vector3 m2DPosition = Camera.main.WorldToScreenPoint(m3DPosition);

        SaveSharedFile((int)m2DPosition.x, (int)m2DPosition.y);

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

                SaveSharedFile(m_PickPos);

                string szMsg = string.Format("SendMessage('3DPosition({0}, {1}, {2})')", m_PickPos.x, m_PickPos.y, m_PickPos.z);
                proxy.RunPythonScript(szMsg);
                return;
            }

            // Mesh Hit가 되지 않았다면 전체 바운드로 사용하는 BoxCollider로 HitTest를 수행한다.
            RaycastHit hit2;
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (coll.Raycast(ray2, out hit2, Mathf.Infinity))
            {
                m_PickPos = hit2.point;
                SaveSharedFile(m_PickPos);
                string szMsg = string.Format("SendMessage('3DPosition({0}, {1}, {2})')", m_PickPos.x, m_PickPos.y, m_PickPos.z);
                proxy.RunPythonScript(szMsg);
            }
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
        RaycastHit hit2;
        Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(x, y));
        if (coll.Raycast(ray2, out hit2, Mathf.Infinity))
        {
            return hit2.point;
        }

        return new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    private void MouseUpPosition(int x, int y, int nMouse)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szButton = "Left";
            if (nMouse == 1)
            {
                szButton = "Right";
            }
            else if (nMouse == 2)
            {
                szButton = "Middle";
            }
            string szMsg = string.Format("SendMessage('OnMouse{0}Up({1}, {2})')", szButton, x, y);

            //Debug.logger.Log(szMsg);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void MouseDownPosition(int x, int y, int nMouse)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szButton = "Left";
            if (nMouse == 1)
            {
                szButton = "Right";
            }
            else if (nMouse == 2)
            {
                szButton = "Middle";
            }
            string szMsg = string.Format("SendMessage('OnMouse{0}Down({1}, {2})')", szButton, x, y);

            //Debug.logger.Log(szMsg);
            proxy.RunPythonScript(szMsg);
        }
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

    // public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };
    private void SetMode(int nMode, bool bTrue)
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

        Debug.logger.Log("End UpdateAliasName");
    }

    private void UpdateAliasName()
    {
        Debug.logger.Log("Begin UpdateAliasName");
        foreach (Transform child in transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    Debug.logger.Log("UpdateAliasName : " + mf.name);
                    sm.UpdateAliasName();
                }
                else
                {
                    Debug.logger.Log("UpdateAliasName fail : " + mf.name);
                }

            }
        }

        Debug.logger.Log("End UpdateAliasName");
    }

    public void UpdateAliasColor()
    {
        foreach (Transform child in transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    sm.SetTextColor(ModelManager.Instance.BuildingNameColor);
                }
            }
        }
    }


    private void CalcBounds(Transform main, SelectionModel sm)
    {
        foreach (Transform child in main)
        {
            if (child.childCount > 0)
            {
                CalcBounds(child, sm);              
            }

            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                sm.MeshBounds.Encapsulate(mr.bounds);
                if (child.gameObject.activeInHierarchy == true)
                {
                    modelBound = mr.bounds;
                }
            }
            
        }
    }

    private void CalcBounds(Transform main)
    {
        SelectionModel sm = main.gameObject.GetComponent<SelectionModel>();
        foreach (Transform child in main)
        {
            if (child.childCount > 0)
            {
                CalcBounds(child, sm);
              
                
            }

            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                sm.MeshBounds.Encapsulate(mr.bounds);
                if (child.gameObject.activeInHierarchy == true)
                {
                    modelBound = mr.bounds;
                }
            }
            
        }
    }

    private void Awake()
    {
        ModelManager.Instance.Model = this;
        Light[] lights = Light.GetLights(LightType.Directional, 0);
        Light light = lights[0];

        Vector3 dir = gameObject.transform.position - light.transform.position;

        foreach (Transform child in transform)
        {
            SelectionModel model = child.gameObject.AddComponent<SelectionModel>();
            
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
                Debug.logger.Log("Mesh Filter : " + child.name);
            }
            

            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Debug.logger.Log("Mesh Render : " + child.name);
                mainBound.Encapsulate(mr.bounds);
                if (child.gameObject.activeInHierarchy == true)
                {
                    modelBound = mr.bounds;
                    model.MeshBounds = mr.bounds;
                }
            }
            else
            {
                if (child.childCount > 0)
                {
                    CalcBounds(child);
                    mainBound.Encapsulate(model.MeshBounds);
                }
            }
        }

        AddPythonFunction();



        try
        {
            file = new SharedMemory.BufferReadWrite(name: "UnitySamInsidePoiInfo");
        }catch(Exception)
        { }

     
        try
        {
            ReadyToRead();
        }
        catch (Exception)
        {

        }
        
    }

    void Start()
    {
        m_MainCamera = GetComponent<Camera>();
        coll = GetComponent<Collider>();
        coll.isTrigger = true;
        m_ScreenCenter = coll.bounds.center;

        ModelManager.Instance.OpenModel("z1_1");
    }

    void Update()
    {

        // Wheel button click
        if (Input.GetMouseButtonDown(2))
        {
            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);
            MouseDownPosition((int)x, (int)y, 2);

            RaycastHit hit1;
            Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
            {
                m_ScreenCenter = hit1.point;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (coll.Raycast(ray, out hit, Mathf.Infinity))
            {
                m_bPanning = true;
                m_bZoominng = true;
                m_bDragged = true;

                mTranslateStartPt = hit.point;
                mZoomCenter = hit.point;
                mOrbitCenter = hit.point;

                m_MousePosStart = Input.mousePosition;
                m_MousePosPrev = Input.mousePosition;
            }
            else
            {
                m_bPanning = false;
                m_bZoominng = false;
                m_bDragged = false;
            }
        }

        if (m_bPickMode == false)
        {            // Mouse Work Mode
            if (Input.GetMouseButtonDown(0))
            {
                float x = Input.mousePosition.x;
                float y = (Screen.height - Input.mousePosition.y);
                MouseDownPosition((int)x, (int)y, 0);


                RaycastHit hit;
                Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
                if (coll.Raycast(ray2, out hit, Mathf.Infinity))
                {
                    m_ScreenCenter = hit.point;
                }

                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    mTranslateStartPt = hit1.point;
                    mZoomCenter = hit1.point;
                    mOrbitCenter = hit1.point;

                    m_MousePosStart = Input.mousePosition;
                    m_MousePosPrev = Input.mousePosition;

                    m_bPanning = false;
                    m_bZoominng = false;
                    m_bDragged = true;
                }
                else
                {
                    m_bPanning = false;
                    m_bZoominng = false;
                    m_bDragged = false;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_bDragged = false;
                Get3DPosition((int)Input.mousePosition.x, (int)Input.mousePosition.y);
            }
        }

        // DragMode
        if (m_bDragged == true)
        {
            m_MousePosCur = Input.mousePosition;
            if (Input.GetMouseButton(0))
            {
                if (m_bOrbitMode == true)
                {
                    UpdateRotation();
                }
                else if (m_bTranslateMode == true)
                {
                    UpdateTranslate();
                }
            }
            if (Input.GetMouseButton(2))
            {
                if (m_bPanning == true)
                {
                    UpdateTranslate();
                }
            }
            m_MousePosPrev = m_MousePosCur;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //if (m_bZoominng == true)
            {
                float zDelta = Input.GetAxis("Mouse ScrollWheel");
                UpdateZoom(zDelta);
            }
            m_MousePosPrev = m_MousePosCur;
        }
       

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
        {
            m_bDragged = false;

            m_bPanning = false;
            m_bZoominng = false;

            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);

            if (Input.GetMouseButtonUp(2))
                MouseUpPosition((int)x, (int)y, 2);
            else
                MouseUpPosition((int)x, (int)y, 0);
        }


        if (Input.GetMouseButtonUp(1))
        {
            m_bDragged = false;
            m_bPanning = false;
            m_bZoominng = false;

            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);
            MouseUpPosition((int)x, (int)y, 1);
        }
        if (Input.GetMouseButtonDown(1))
        {
            m_bDragged = false;
            m_bPanning = false;
            m_bZoominng = false;

            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);
            MouseDownPosition((int)x, (int)y, 1);
        }
    }

    private Vector3 m_PickPos = new Vector3();

    private void UpdateTranslate()
    {
        if (m_MousePosCur == m_MousePosPrev)
            return;

        RaycastHit hit1;
        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
        {
            mTranslatePt = hit1.point;
            if (mTranslateStartPt != Vector3.zero)
            {
                Vector3 move = mTranslateStartPt - mTranslatePt;

                if (move.magnitude > 50.0f)
                {
                    move = move.normalized * 50.0f;
                }
                Vector3 camPos = Camera.main.transform.position + move;
                if (camPos.y < 1.0f)
                    camPos.y = 1.0f;

                Camera.main.transform.position = camPos;
            }
        }
    }

    private void ManualZoom(float zDelta)
    {
        Debug.logger.Log("0Zoom Deleta :" + zDelta);
        RaycastHit hit1;
        Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
        if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
        {
            Vector3 ptScreenCenter = hit1.point;
            Debug.logger.Log("1Zoom Deleta :" + zDelta);
            if (zDelta != 0)
            {

                Debug.logger.Log("2Zoom Deleta :" + zDelta);

                float distance = -(float)((zDelta) * m_fZoomDistance);
                Vector3 vOrgPos = Camera.main.transform.position;
                Vector3 zCenter = new Vector3(ptScreenCenter.x, ptScreenCenter.y, ptScreenCenter.z);

                Vector3 dirCam = (zCenter - vOrgPos).normalized;
                Vector3 dir = dirCam * distance;
                Vector3 vPos = vOrgPos - dir;
                if (vPos.y < 1.0f)
                    vPos.y = 1.0f;

                Camera.main.transform.position = vPos;
            }
        }
    }

    private void UpdateZoom(float zDelta)
    {
        if (zDelta != 0)
        {
            float distance = -(float)((zDelta) * m_fZoomDistance);
            Vector3 vOrgPos = Camera.main.transform.position;
            Vector3 zCenter = new Vector3(m_ScreenCenter.x, m_ScreenCenter.y, m_ScreenCenter.z);

            Vector3 dirCam = (zCenter - vOrgPos).normalized;
            Vector3 dir = dirCam * distance;
            Vector3 vPos = vOrgPos - dir;
            if (vPos.y < 1.0f)
                vPos.y = 1.0f;

            Camera.main.transform.position = vPos;
        }
    }

    private void UpdateRotation()
    {
        Vector3 PtDiff = m_MousePosCur - m_MousePosPrev;
        Vector3 rCenter = new Vector3(m_ScreenCenter.x, m_ScreenCenter.y, m_ScreenCenter.z);
        if (m_MousePosCur == m_MousePosPrev)
            return;

        float pitch = (-0.5f * PtDiff.y) * m_RotRatioY;
        float yaw = (-0.5f * PtDiff.x) * m_RotRatioX;
        if (pitch > 70f)
        {
            pitch = 70f;
        }

        Vector3 yUnit = new Vector3(0f, 1f, 0f);
        Quaternion rot1 = new Quaternion();
        rot1.SetAxisAngle(-yUnit, yaw);

        Quaternion rot2 = new Quaternion();
        rot2.SetAxisAngle(Camera.main.transform.right, pitch);

        Quaternion q1 = (rot1 * rot2);
        Vector3 MposToCam = q1 * (Camera.main.transform.position - rCenter);

        Camera.main.transform.position = (MposToCam + rCenter);
        Vector3 vPos = Camera.main.transform.position;
        if (vPos.y < 1.0f)
            vPos.y = 1.0f;

        Camera.main.transform.position = vPos;
        if (m_ScreenCenter != Vector3.zero)
        {
            Camera.main.transform.LookAt(m_ScreenCenter);
        }
        else
        {
            Camera.main.transform.LookAt(rCenter);
        }
    }


    private void OnMouseOver()
    {


    }

    private void OnMouseUp()
    {
    }

    void OnMouseDown()
    {
        int i = 0;
        i++;
    }

    private void OnMouseClick()
    {
    }


}
