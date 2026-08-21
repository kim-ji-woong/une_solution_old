using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class MainModel : MonoBehaviour, IInputClickHandler, IInputHandler, INavigationHandler
{
    private Stopwatch m_sw = new Stopwatch();   // Touch Time
    private Stopwatch m_swNavi = new Stopwatch(); // Navigation Time

    private ModelManager m_ModelManager = null;

    private bool m_bDragged = false;
    private Vector3 m_MousePosCur;
    private Vector3 m_MousePosPrev;
    private Vector3 mOrbitCenter;
    private Vector3 mTranslateStartPt;

    private float yAngle = 0.0f;
    private float xAngle = 0.0f;
    private float m_fZoomDistance = 10.0f;

    private Collider m_coll;

    private Bounds m_Bounds = new Bounds();     // gameObject의 Bouunds

    private bool m_bActiveUnder = true;

    void Awake()
    {
        AwakeChild(transform);
    }

    // Use this for initialization
    void Start()
    {
        BoxCollider col = gameObject.AddComponent<BoxCollider>();

        bool bFirst = true;
        CalcBounds(transform, ref bFirst, ref m_Bounds);

        m_ModelManager = FindModelManager(transform);

        col.center = new Vector3(m_Bounds.center.x, m_Bounds.min.y, m_Bounds.center.z);
        //col.size = new Vector3(m_Bounds.max.x-m_Bounds.min.x, m_Bounds.max.y-m_Bounds.min.y, m_Bounds.max.z-m_Bounds.min.z);
        col.size = new Vector3((m_Bounds.max.x - m_Bounds.min.x) * 2, 0.1f, (m_Bounds.max.z - m_Bounds.min.z) * 2);

        m_coll = (Collider)col;
        m_coll.isTrigger = true;

        // 카메라 위치와 방향 설정
        //InitCameraPos();

        xAngle = Camera.main.transform.eulerAngles.y;
        yAngle = Camera.main.transform.eulerAngles.x;

        if (gameObject.name == "Under")
            m_ModelManager.ShowModel(ModelManager.ModelIndex.model_under, false);
    }

    // Update is called once per frame
    void Update()
    {
        MainModel model = GetComponent<MainModel>();
        if (model == null)
            return;

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        // 마우스가 화면 밖으로 이동시 마우스 이벤트 중지
        if (mouseX <= 0 || mouseX >= Screen.width || mouseY <= 0 || mouseY >= Screen.height)
            return;

        // Left mouse down
        if (Input.GetMouseButtonDown(0))
        {
            if (!PIck(ref mOrbitCenter))
            {
                mOrbitCenter = m_Bounds.center;
            }

            m_MousePosPrev = Input.mousePosition;
            m_bDragged = true;
        }
        // Wheel mouse down
        else if (Input.GetMouseButtonDown(2))
        {
            RaycastHit hit1;
            Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (m_coll.Raycast(ray1, out hit1, Mathf.Infinity))
            {
                //m_ScreenCenter = hit1.point;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (m_coll.Raycast(ray, out hit, Mathf.Infinity))
            {
                m_bDragged = true;

                mTranslateStartPt = hit.point;
                //mZoomCenter = hit.point;
                mOrbitCenter = hit.point;

                //m_MousePosStart = Input.mousePosition;
                m_MousePosPrev = Input.mousePosition;
            }
            else
            {
                m_bDragged = false;
            }
        }
        if (m_bDragged == true)
        {
            m_MousePosCur = Input.mousePosition;
            if (Input.GetMouseButton(0))
            {
                UpdateRotation();
            }
            else if (Input.GetMouseButton(2))
            {
                UpdateTranslate();
            }
            m_MousePosPrev = m_MousePosCur;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float zDelta = Input.GetAxis("Mouse ScrollWheel");
            UpdateZoom(zDelta);

            m_MousePosPrev = m_MousePosCur;
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
        {
            m_bDragged = false;
        }
    }

    private void AwakeChild(Transform node)
    {
        MeshFilter mf = node.GetComponent<MeshFilter>();
        if (mf != null)
        {
            MeshCollider collider = node.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mf.sharedMesh;
            collider.convex = false;
        }
        else
        {
            SkinnedMeshRenderer mr = node.GetComponent<SkinnedMeshRenderer>();
            if (mr != null)
            {
                Mesh colliderMesh = new Mesh();
                MeshCollider collider = node.gameObject.AddComponent<MeshCollider>();
                mr.BakeMesh(colliderMesh);
                collider.sharedMesh = colliderMesh;
                collider.convex = false;
            }
        }

        foreach (Transform child in node)
        {
            AwakeChild(child);
        }
    }

    private ModelManager FindModelManager(Transform t)
    {
        if (t == null)
            return null;

        ModelManager manager = t.parent.GetComponent<ModelManager>();
        if (manager != null)
            return manager;

        return FindModelManager(t.parent);
    }

    // 각 모델에 따른 카메라 위치 및 방향 설정
    private void InitCameraPos()
    {
        Vector3 dir = new Vector3(0f, -0.6f, 0.8f);
        Ray ray = new Ray(m_Bounds.center, -dir);
        Vector3 pos = ray.GetPoint(m_Bounds.extents.magnitude * 1.2f);
        Camera.main.transform.position = pos;
        Camera.main.transform.forward = dir;
    }

    public Transform PIck(ref Vector3 rPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        Transform rTransform = null;
        float dist = 100000000f;

        if (Hittest(ray, hit, transform, ref dist, ref rTransform, ref rPos))
            return rTransform;

        return null;
    }

    private bool Hittest(Ray ray, RaycastHit hit, Transform t, ref float rDist, ref Transform rTransform, ref Vector3 rPos)
    {
        bool bRes = false;
        foreach (Transform child in t)
        {
            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();

                if (collider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.distance < rDist)
                    {
                        rDist = hit.distance;
                        rTransform = child;
                        rPos = hit.point;
                        bRes = true;
                    }
                }
            }

            if (Hittest(ray, hit, child, ref rDist, ref rTransform, ref rPos))
            {
                bRes = true;
            }
        }
        return bRes;
    }

    // 전체 바운딩 박스 계산
    private void CalcBounds(Transform t, ref bool bFirst, ref Bounds bounds)
    {
        // 모든 gameObject의 MeshFilter를 검사
        MeshRenderer mr = t.gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            if (bFirst)
            {
                bounds.SetMinMax(mr.bounds.min, mr.bounds.max);
                bFirst = false;
            }
            else
                bounds.Encapsulate(mr.bounds);

            int order = mr.sortingOrder;
        }
        else
        {
            SkinnedMeshRenderer smr = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                if (bFirst)
                {
                    bounds.SetMinMax(smr.bounds.min, smr.bounds.max);
                    bFirst = false;
                }
                else
                    bounds.Encapsulate(smr.bounds);
            }
        }

        foreach (Transform child in t)
        {
            CalcBounds(child, ref bFirst, ref bounds);
        }
    }

    private void UpdateRotation()
    {
        Vector3 PtDiff = m_MousePosCur - m_MousePosPrev;
        if (PtDiff.x == 0 && PtDiff.y == 0)
            return;

        float pitch = (-0.5f * PtDiff.x);
        float yaw = (-0.5f * PtDiff.y);

        if (yAngle + yaw < 5)
            yaw = 5 - yAngle;
        else if (yAngle + yaw > 85)
            yaw = 85 - yAngle;

        xAngle += pitch;
        yAngle += yaw;

        Camera.main.transform.RotateAround(mOrbitCenter, Camera.main.transform.right, yaw);
        Camera.main.transform.RotateAround(mOrbitCenter, -Vector3.up, pitch);
    }

    private void UpdateTranslate()
    {
        if (m_MousePosCur == m_MousePosPrev)
            return;

        RaycastHit hit1;
        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (m_coll.Raycast(ray1, out hit1, Mathf.Infinity))
        {
            if (mTranslateStartPt != Vector3.zero)
            {
                Vector3 move = mTranslateStartPt - hit1.point;

                if (move.magnitude > 50.0f)
                {
                    move = move.normalized * 50.0f;
                }

                if (!MeshHit(Camera.main.transform.position, Camera.main.transform.position + move))
                {
                    Vector3 camPos = Camera.main.transform.position + move;
                    if (camPos.y < 1.0f)
                        camPos.y = 1.0f;

                    Camera.main.transform.position = camPos;
                }
            }
        }
    }

    private void UpdateZoom(float zDelta)
    {
        if (zDelta != 0)
        {
            Vector3 vTarget = Vector3.zero;
            Vector3 vDir = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                vTarget = hit.point;
                vDir = (hit.point - Camera.main.transform.position).normalized;
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
                if (Physics.Raycast(ray, out hit))
                {
                    vTarget = hit.point;
                }
                else
                    vTarget = new Vector3(Camera.main.transform.position.x, m_Bounds.center.y, Camera.main.transform.position.z);
                vDir = Camera.main.transform.forward;
            }

            Vector3 move = vDir * (zDelta * m_fZoomDistance);
            if ((Camera.main.transform.position - vTarget).magnitude < move.magnitude && zDelta > 0)
            {
                return;
            }

            Camera.main.transform.Translate(move, Space.World);
        }
    }

    private bool MeshHit(Vector3 v1, Vector3 v2)
    {
        bool bHit = false;
        SortedList arHitPt = new SortedList();
        Vector3 dir = v2 - v1;
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


                Ray ray1 = new Ray(v1, dir);
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    if (hit1.distance < dir.magnitude)
                    {
                        // Hit된경우 거리값을 키로 저장한다.
                        arHitPt.Add(hit1.distance, hit1);

                        bHit = true;
                    }

                }
            }

            if (MeshHitChild(child, v1, dir))
            {
                bHit = true;
            }
        }
        return bHit;
    }

    private bool MeshHitChild(Transform node, Vector3 org, Vector3 dir)
    {
        foreach (Transform child in node)
        {
            if (MeshHitChild(child, org, dir))
            {
                return true;
            }

            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
                Ray ray1 = new Ray(org, dir);
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    // Hit된경우 거리값을 키로 저장한다.
                    if (hit1.distance < dir.magnitude)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        UnityEngine.Debug.Log("OnInputClicked");
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (m_sw.ElapsedMilliseconds < 1000)
        {
            m_ModelManager.ShowMenu();
        }
        else if (m_ModelManager.IsActive(ModelManager.ModelIndex.model_under))
        {
            if (m_sw.ElapsedMilliseconds < 3000)
            {
                m_ModelManager.ShowMenu();
            }
        }
        m_sw.Stop();

        //if (m_bActiveUnder)
        //{
        //    if (m_bActiveUnder &&
        //        (m_ModelManager.mouseMode == ModelManager.MouseMode.mode_orbit ||
        //        m_ModelManager.mouseMode == ModelManager.MouseMode.mode_pan))
        //    {
        //        m_ModelManager.ShowModel(ModelManager.ModelIndex.model_under, true);
        //    }
        //}
    }

    public void OnInputDown(InputEventData eventData)
    {
        m_sw.Reset();
        m_sw.Start();
        UnityEngine.Debug.Log("OnInputDown");
    }

    public void OnNavigationStarted(NavigationEventData eventData)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit = new RaycastHit();
        Transform rTransform = null;
        float dist = 100000000f;

        Vector3 rPos = Vector3.zero;
        if (!Hittest(ray, hit, transform, ref dist, ref rTransform, ref rPos))
            return;

        // 속도가 느려 모델 Activate 설정
        if (m_ModelManager.mouseMode == ModelManager.MouseMode.mode_orbit ||
            m_ModelManager.mouseMode == ModelManager.MouseMode.mode_pan)
        {
            m_bActiveUnder = m_ModelManager.IsActive(ModelManager.ModelIndex.model_under);
            if (transform.name == "Over")
            {
                if (m_bActiveUnder)
                {
                    m_ModelManager.ShowModel(ModelManager.ModelIndex.model_under, false);
                }
            }
            else if (transform.name == "Under")
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    Transform child = transform.GetChild(i);
                    if(child != rTransform.parent)
                        child.gameObject.SetActive(false);
                }
            }
        }

        m_ModelManager.pivot = rPos;
    }

    public void OnNavigationUpdated(NavigationEventData eventData)
    {
        if (m_ModelManager.mouseMode == ModelManager.MouseMode.mode_orbit)
        {
            m_ModelManager.UpdateRotation(null, eventData.CumulativeDelta);
        }
        else if (m_ModelManager.mouseMode == ModelManager.MouseMode.mode_pan)
        {
            m_ModelManager.UpdateTranslate(null, eventData.CumulativeDelta.normalized);
            //UpdateTranslate2(eventData.CumulativeDelta.normalized);
        }
    }

    public void OnNavigationCompleted(NavigationEventData eventData)
    {
        // 속도가 느려 모델 Activate 설정
        if (m_ModelManager.mouseMode == ModelManager.MouseMode.mode_orbit ||
            m_ModelManager.mouseMode == ModelManager.MouseMode.mode_pan)
        {
            if (transform.name == "Over")
            {
                if (m_bActiveUnder)
                {
                    m_ModelManager.ShowModel(ModelManager.ModelIndex.model_under, true);
                }
            }
            else if (transform.name == "Under")
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnNavigationCanceled(NavigationEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void UpdateRotation2(Vector3 vPivot, Vector3 dir)
    {
        //transform.RotateAround(rPos, -Vector3.up, dir.x);
        transform.RotateAround(vPivot, -Vector3.up * 2, dir.x);
    }

    public void UpdateTranslate2(Vector3 move)
    {
        if (Camera.main.transform.forward.z < 0)
        {
            move.x *= -1;
            move.z *= -1;
        }
        transform.Translate(move / 200f);
    }
}
