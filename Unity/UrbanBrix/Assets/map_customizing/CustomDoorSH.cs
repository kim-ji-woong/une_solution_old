using System.Collections;
using System.Collections.Generic;
using UnE.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomDoorSH : MonoBehaviour
{
    public GameObject DoorPref;
    private static CustomDoorSH m_instance = null;
    public static CustomDoorSH Instance { get { return m_instance; } }

    private MainModel modelController;

    private CustomizingStage m_curStage = CustomizingStage.None;

    private Vector3 firstFHit;
    private Vector3 firstPos;
    private Vector3 floorHit;

    private bool isHitFloor = false;

    private DoorSH m_curSpawnDoor = null;
    private DoorSH m_curDetectDoor = null;
    private DoorSH m_curSelectDoor = null;

    private Mark m_curDetectMark = null;
    private Mark m_curSelectMark = null;

    private Dictionary<string, List<GameObject>> m_dicDoors = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> DicDoors
    {
        get { return m_dicDoors; }
        set { m_dicDoors = value; }
    }
    private static Dictionary<string, bool> m_dicChgDoors = new Dictionary<string, bool>(); // scene별 수정유무

    private void Awake()
    {
        m_instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        modelController = FindObjectOfType<MainModel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (ModelManager.Instance == null)
            return;

        if (!ModelManager.Instance.Model.EditMode)
            return;

        if (!ModelManager.Instance.Model.WallEditMode)
            return;

        Detect();

        switch (m_curStage)
        {
            case CustomizingStage.None:
                if (Input.GetMouseButtonDown(0))
                {
                    if (m_curDetectDoor)
                    {
                        m_curSelectDoor = m_curDetectDoor;
                        m_curStage = CustomizingStage.Mode_Move;
                        m_curSelectDoor.Select(CustomizingStage.Mode_Move);

                        CustomizingController.Instance.ClearSelectedWall();
                    }
                }
                break;
            case CustomizingStage.Spawn:
                m_curSpawnDoor.transform.position = floorHit;

                Vector3 vFrom = m_curSpawnDoor.transform.position;
                Vector3 vTo = new Vector3();
                Wall wall = FindNearWall(vFrom, ref vTo);
                if (wall != null)
                {
                    m_curSpawnDoor.transform.position = new Vector3(vTo.x, vTo.y, vTo.z);
                    m_curSpawnDoor.transform.rotation = wall.transform.rotation;
                    m_curSpawnDoor.SetWall(wall);
                    SetChange();

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (isHitFloor)
                        {
                            if (!m_dicDoors.ContainsKey(modelController.CurrentScene.SceneName))
                                m_dicDoors.Add(modelController.CurrentScene.SceneName, new List<GameObject>());
                            m_dicDoors[modelController.CurrentScene.SceneName].Add(m_curSpawnDoor.gameObject);

                            m_curSpawnDoor = null;
                            m_curStage = CustomizingStage.None;
                        }
                    }
                }
                break;
            case CustomizingStage.Mode_Move:
                if (Input.GetMouseButtonDown(0))
                {
                    if (!m_curDetectDoor)
                    {
                        m_curStage = CustomizingStage.None;
                        m_curSelectDoor.Select(CustomizingStage.None);
                        m_curSelectDoor = null;
                        break;
                    }
                    else if (m_curDetectDoor != m_curSelectDoor)
                    {
                        m_curSelectDoor.Select(CustomizingStage.None);
                        m_curSelectDoor = m_curDetectDoor;
                        m_curSelectDoor.Select(m_curStage);
                        
                        break;
                    }

                    if (m_curDetectMark)
                    {
                        firstFHit = floorHit;
                        firstPos = m_curSelectDoor.transform.position;
                        m_curSelectMark = m_curDetectMark;
                        m_curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && m_curSelectMark)
                {
                    switch (m_curSelectMark.kind)
                    {
                        case TransformKind.Move_X:
                            {
                                Vector3 newPos = firstPos + m_curSelectDoor.transform.right * Vector3.Dot(m_curSelectDoor.transform.right, floorHit - firstFHit);
                                newPos.x = SnapController.XPos(newPos, m_curSelectDoor.GetComponent<SnapObj>());

                                m_curSelectDoor.transform.position = newPos;
                            }
                            break;
                        case TransformKind.Move_Z:
                            m_curSelectDoor.transform.position = firstPos + m_curSelectDoor.transform.forward * Vector3.Dot(m_curSelectDoor.transform.forward, floorHit - firstFHit);
                            break;
                        case TransformKind.Move_Both:
                            m_curSelectDoor.transform.position = firstPos + floorHit - firstFHit;
                            break;
                    }

                    vFrom = m_curSelectDoor.transform.position;
                    vTo = new Vector3();
                    wall = FindNearWall(vFrom, ref vTo);
                    if (wall != null)
                    {
                        // 다른 가벽으로 변경했을 때 해당 문의 기존 ParentWall을 삭제한다
                        m_curSelectDoor.SetWall(wall);

                        m_curSelectDoor.transform.position = vTo;
                        m_curSelectDoor.transform.rotation = wall.transform.rotation;

                        SetChange();
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (m_curSelectMark)
                    {
                        // 인접한 가벽이 없으면 원래 있던 가벽 좌표에 돌려놓는다
                        vFrom = m_curSelectDoor.transform.position;
                        vTo = new Vector3();
                        wall = FindNearWall(vFrom, ref vTo);
                        if (wall == null)
                        {
                            m_curSelectDoor.transform.position = m_curSelectDoor.ParentWall.transform.position;
                        }

                        m_curSelectMark.Select(false);
                        m_curSelectMark = null;

                        SetChange();
                    }
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (m_curSelectDoor != null)
            {
                m_curSelectDoor.SetWall(null);

                Destroy(m_curSelectDoor.gameObject);
                m_dicDoors[modelController.CurrentScene.SceneName].Remove(m_curSelectDoor.gameObject);
                m_curSelectDoor = null;

                m_curStage = CustomizingStage.None;
                SetChange();
            }
        }
    }

    private void Detect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        isHitFloor = modelController.CurrentScene.DetectFloor(ray.origin, ray.direction, out floorHit);

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(camRay, float.MaxValue);

        float closestDist = float.MaxValue;
        m_curDetectMark = null;
        foreach (var item in hits)
        {
            Mark curMark = item.collider.GetComponent<Mark>();
            float curDist = (item.point - Camera.main.transform.position).sqrMagnitude;
            if (curMark)
            {
                if (curDist < closestDist)
                {
                    m_curDetectMark = curMark;
                    closestDist = curDist;
                }
            }
        }

        m_curDetectDoor = null;
        foreach (var item in hits)
        {
            m_curDetectDoor = item.collider.GetComponentInParent<DoorSH>();
            if (m_curDetectDoor)
            {
                break;
            }
        }
    }

    public void BT_SpawnDoor()
    {
        try
        {
            if (!CustomizingController.Instance.DicWalls.ContainsKey(modelController.CurrentScene.SceneName))
                return;

            if (CustomizingController.Instance.DicWalls[modelController.CurrentScene.SceneName].Count == 0)
                return;

            if (!m_curSpawnDoor)
            {
                m_curSpawnDoor = Instantiate(DoorPref).GetComponent<DoorSH>();
            }

            m_curStage = CustomizingStage.Spawn;
        }
        catch (System.Exception ex)
        {
            MainModel.WriteLog("[ERROR] BT_SpawnDoor " + ex.Message);
            if (m_curSpawnDoor == null)
                MainModel.WriteLog("[ERROR] BT_SpawnDoor m_curSpawnDoor is null");
            if (DoorPref == null)
                MainModel.WriteLog("[ERROR] BT_SpawnDoor DoorPref is null");
        }
    }

    public DoorSH InstantiateDoor()
    {
        DoorSH door = Instantiate(DoorPref).GetComponent<DoorSH>();
        return door;
    }

    public void ClearSelectedDoor()
    {
        try
        {
            m_curStage = CustomizingStage.None;
            if (m_curSelectDoor != null)
            {
                m_curSelectDoor.Select(CustomizingStage.None);
                m_curSelectDoor = null;
                m_curSelectMark = null;
            }
        }
        catch (System.Exception ex)
        {
            MainModel.WriteLog("[ERROR] ClearSelectedDoor " + ex.Message);
        }
    }

    public void ResetChagne()
    {
        m_dicChgDoors[modelController.CurrentScene.SceneName] = false;
    }

    // 가벽이 수정되었음을 SDMS에 알려준다
    // false 에서 true가 될때 최초 한번만 알려줌 (저장 버튼 활성화를 위해서)
    private void SetChange()
    {
        if (!m_dicChgDoors.ContainsKey(modelController.CurrentScene.SceneName) || m_dicChgDoors[modelController.CurrentScene.SceneName] == false)
        {
            CustomizingController.DicChgWalls[modelController.CurrentScene.SceneName] = true;
            m_dicChgDoors[modelController.CurrentScene.SceneName] = true;

            modelController.ChangeDoor();
        }
    }

    public void RemoveDoor(Wall wall)
    {
        if (!m_dicDoors.ContainsKey(modelController.CurrentScene.SceneName))
            return;

        List<GameObject> deleteDoors = new List<GameObject>();

        List<GameObject> doors = m_dicDoors[modelController.CurrentScene.SceneName];
        foreach (GameObject item in doors)
        {
            DoorSH door = item.GetComponent<DoorSH>();
            if (wall.Doors.Contains(door))
            {
                Destroy(item);
                deleteDoors.Add(item);
            }
        }

        foreach (GameObject door in deleteDoors)
        {
            m_dicDoors[modelController.CurrentScene.SceneName].Remove(door);
        }
    }

    private Wall FindNearWall(Vector3 fromV, ref Vector3 vec)
    {
        try
        {
            if (CustomizingController.Instance.DicWalls != null)
            {
                float fZoom = modelController.GetZoomSize() * 0.03f;
                List<GameObject> walls = CustomizingController.Instance.DicWalls[modelController.CurrentScene.SceneName];

                foreach (GameObject obj in walls)
                {
                    Wall wall = obj.GetComponent<Wall>();
                    if (wall == null)
                        continue;

                    // 1. GetDistance
                    // 2. GetNearestVertex
                    // 3. IsInclude
                    Vector3 vBeginTemp = wall.GetWallPoint(true);
                    Vector3 vEndTemp = wall.GetWallPoint(false);

                    Vertex2D vBegin2D = new Vertex2D(vBeginTemp.x, vBeginTemp.z); // 가벽 시작점
                    Vertex2D vEnd2D = new Vertex2D(vEndTemp.x, vEndTemp.z);         // 가벽 끝점
                    Vertex2D vPoint2D = new Vertex2D(fromV.x, fromV.z);

                    Line2D line = new Line2D(vBegin2D, vEnd2D);
                    double distance = line.GetDistance(vPoint2D, false);

                    if (distance <= fZoom)
                    {
                        Vertex3D vBegin3D = new Vertex3D(vBeginTemp.x, vBeginTemp.y, vBeginTemp.z); // 가벽 시작점
                        Vertex3D vEnd3D = new Vertex3D(vEndTemp.x, vEndTemp.y, vEndTemp.z);         // 가벽 끝점
                        Vertex3D vPoint3D = new Vertex3D(fromV.x, fromV.y, fromV.z);

                        Vertex3D pos = Math.GetNearestVertex(vPoint3D, vBegin3D, vEnd3D, true);
                        vec = new Vector3((float)pos.x, (float)pos.y, (float)pos.z);

                        return wall;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            MainModel.WriteLog("[ERROR] FindNearWall " + ex.Message);
        }

        return null;
    }

    private void MovingDoor(Wall wall, DoorSH door)
    {
        //Vector3 vBeginTemp = wall.GetWallPoint(true);
        //Vector3 vEndTemp = wall.GetWallPoint(false);

        //Vertex2D vBegin2D = new Vertex2D(vBeginTemp.x, vBeginTemp.z); // 가벽 시작점
        //Vertex2D vEnd2D = new Vertex2D(vEndTemp.x, vEndTemp.z);         // 가벽 끝점

        //Line2D line = new Line2D(vBegin2D, vEnd2D);

        //Vertex2D ss = (vBegin2D + vEnd2D) / 2;

        //Vertex2D ss2 = new Vertex2D(door.transform.position.x, door.transform.position.z);

        ////Vertex2D ss3 = ss2 / ss * 100;
    }
}
