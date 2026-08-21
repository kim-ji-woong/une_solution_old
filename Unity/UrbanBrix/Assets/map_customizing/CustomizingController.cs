using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CustomizingStage
{
    None,
    Spawn,
    Mode_Move,
    Mode_Scale,
    Mode_Rotate
}
public enum TransformKind
{
    Move_X,
    Move_Z,
    Move_Both,
    Scale,
    Rotate,
    MoveText_X,
    MoveText_Z,
    MoveText_Both,
}

public class CustomizingController : MonoBehaviour
{
    public GameObject wallPref;
    public InputField scaleIF;
    public InputField rotateIF;    

    private CustomizingStage m_curStage = CustomizingStage.None;
    public CustomizingStage CurStage
    {
        get { return m_curStage; }
        set { m_curStage = value; }
    }
    private Vector3 floorHit;
    public Vector3 FloorHit
    {
        get { return floorHit; }
    }
    private bool isHitFloor = false;
    public bool IsHitFloor
    {
        get { return IsHitFloor; }
    }
    private Vector3 firstFHit;
    private Vector3 firstPos;
    private Vector3 rotCenterPos;
    private float firstRot;
    private Wall curSpawnWall = null;
    private Wall curDetectWall = null;
    public Wall CurDetectWall
    {
        get { return curDetectWall; }
    }
    private Wall m_curSelectWall = null;
    public Wall CurSelectWall
    {
        get { return m_curSelectWall; }
    }
    private Mark curDetectMark = null;
    private Mark curSelectMark = null;
    private Dictionary<string, List<GameObject>> m_dicWalls = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> DicWalls
    {
        get { return m_dicWalls; }
        set { m_dicWalls = value; }
    }

    private static Dictionary<string, bool> m_dicChgWalls = new Dictionary<string, bool>(); // scene별 수정유무
    public static Dictionary<string, bool> DicChgWalls
    {
        get { return m_dicChgWalls; }
        set { m_dicChgWalls = value; }
    }

    private MainModel modelController;

    private static CustomizingController m_instance = null;
    public static CustomizingController Instance
    {
        get { return m_instance; }
    }
    
    private void Awake()
    {
        m_instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        modelController = FindObjectOfType<MainModel>();
    }
    
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
                    if (curDetectWall)
                    {
                        m_curSelectWall = curDetectWall;

                        m_curSelectWall.Select(CustomizingStage.Mode_Move);

                        m_curStage = CustomizingStage.Mode_Move;

                        SnapController.Init(m_dicWalls[modelController.CurrentScene.SceneName], 1.0f);
                        //modelController.GetWallInfo(m_curSelectWall.transform.position.x, m_curSelectWall.transform.position.z, m_curSelectWall.Scale, m_curSelectWall.Rotate);

                        CustomDoorSH.Instance.ClearSelectedDoor();
                    }
                }

                break;
            case CustomizingStage.Spawn:

                curSpawnWall.transform.position = floorHit; //new Vector3(floorHit.x, 0.0f, floorHit.z);

                if (Input.GetMouseButtonDown(0))
                {
                    if (isHitFloor)
                    {
                        if (!m_dicWalls.ContainsKey(modelController.CurrentScene.SceneName))
                            m_dicWalls.Add(modelController.CurrentScene.SceneName, new List<GameObject>());
                        m_dicWalls[modelController.CurrentScene.SceneName].Add(curSpawnWall.gameObject);

                        curSpawnWall = null;
                        m_curStage = CustomizingStage.None;

                        SetChange();
                    }
                }

                break;
            case CustomizingStage.Mode_Move:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!curDetectWall)
                    {
                        m_curStage = CustomizingStage.None;
                        m_curSelectWall.Select(CustomizingStage.None);
                        m_curSelectWall = null;
                        break;
                    }
                    else if (curDetectWall != m_curSelectWall)
                    {
                        m_curSelectWall.Select(CustomizingStage.None);
                        m_curSelectWall = curDetectWall;
                        m_curSelectWall.Select(m_curStage);
                        modelController.GetWallInfo(m_curSelectWall.transform.position.x, m_curSelectWall.transform.position.z, m_curSelectWall.Scale, m_curSelectWall.Rotate);

                        break;
                    }

                    if (curDetectMark)
                    {

                        firstFHit = floorHit;
                        firstPos = m_curSelectWall.transform.position;
                        curSelectMark = curDetectMark;
                        curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && curSelectMark)
                {
                    
                    switch (curSelectMark.kind)
                    {
                        case TransformKind.Move_X:
                            {
                                Vector3 newPos = firstPos + m_curSelectWall.transform.right * Vector3.Dot(m_curSelectWall.transform.right, floorHit - firstFHit);
                                newPos.x = SnapController.XPos(newPos, m_curSelectWall.GetComponent<SnapObj>());

                                m_curSelectWall.transform.position = newPos;

                                // 출입문도 같이 이동
                                //if (m_curSelectWall.Doors != null && m_curSelectWall.Doors.Count > 0)
                                //{
                                //    Vector3 doorPos = m_curSelectWall.Doors[0].transform.localPosition;
                                //    m_curSelectWall.Doors[0].transform.localPosition = new Vector3(newPos.x, doorPos.y, doorPos.z);
                                //}
                            }
                            break;
                        case TransformKind.Move_Z:
                            {
                                m_curSelectWall.transform.position = firstPos + m_curSelectWall.transform.forward * Vector3.Dot(m_curSelectWall.transform.forward, floorHit - firstFHit);
                                // 출입문도 같이 이동
                                //if (m_curSelectWall.Doors != null && m_curSelectWall.Doors.Count > 0)
                                //{
                                //    Vector3 doorPos = m_curSelectWall.Doors[0].transform.localPosition;
                                //    m_curSelectWall.Doors[0].transform.localPosition = new Vector3(doorPos.x, doorPos.y, m_curSelectWall.transform.position.z);
                                //}
                            }
                            break;
                        case TransformKind.Move_Both:
                            {
                                m_curSelectWall.transform.position = firstPos + floorHit - firstFHit;
                            }
                            break;
                    }

                    if (m_bUseSnap)
                    {
                        // 인접한 가벽이 있는지 체크 (Snap)
                        Vector3 vAxis = m_curSelectWall.GetWallPoint(false); // 움직이는 축
                        Vector3 vTo = new Vector3();
                        Wall wall = FindNearWall(vAxis, ref vTo);
                        if (wall != null)
                        {

                            m_curSelectWall.transform.position = m_curSelectWall.GetWallPoint(true, vTo);
                        }
                        else
                        {
                            vAxis = m_curSelectWall.GetWallPoint(true); // 움직이는 축
                            vTo = new Vector3();
                            wall = FindNearWall(vAxis, ref vTo);
                            if (wall != null)
                                m_curSelectWall.transform.position = m_curSelectWall.GetWallPoint(false, vTo);
                        } 
                    }

                    SetChange();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (curSelectMark)
                    {
                        curSelectMark.Select(false);
                        curSelectMark = null;
                    }
                }

                break;
            case CustomizingStage.Mode_Scale:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!curDetectWall)
                    {
                        m_curStage = CustomizingStage.None;
                        m_curSelectWall.Select(CustomizingStage.None);
                        m_curSelectWall = null;
                        break;
                    }
                    else if (curDetectWall != m_curSelectWall)
                    {
                        m_curSelectWall.Select(CustomizingStage.None);
                        m_curSelectWall = curDetectWall;
                        m_curSelectWall.Select(m_curStage);
                        modelController.GetWallInfo(m_curSelectWall.transform.position.x, m_curSelectWall.transform.position.z, m_curSelectWall.Scale, m_curSelectWall.Rotate);

                        break;
                    }

                    if (curDetectMark)
                    {
                        curSelectMark = curDetectMark;
                        curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && curSelectMark)
                {
                    bool referenceAxis = true; // 기준축
                    if (curSelectMark.ToString() == "1 (Mark)") // 선택한 Mark가 1번이라면 2번 Mark가 기준축이 됨
                        referenceAxis = false;

                    SetWallPosition(referenceAxis, floorHit, true);

                    SetChange();

                    if (m_bUseSnap)
                    {
                        // 인접한 가벽이 있는지 체크 (Snap)
                        Vector3 vMovingAxis = m_curSelectWall.GetWallPoint(!referenceAxis); // 움직이는 축
                        Vector3 vTo = new Vector3();
                        Wall wall = FindNearWall(vMovingAxis, ref vTo);
                        if (wall != null)
                            SetWallPosition(referenceAxis, vTo, true); 
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (curSelectMark)
                    {
                        curSelectMark.Select(false);
                        curSelectMark = null;
                    }
                }

                break;
            case CustomizingStage.Mode_Rotate:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!curDetectWall)
                    {
                        m_curStage = CustomizingStage.None;
                        m_curSelectWall.Select(CustomizingStage.None);
                        m_curSelectWall = null;

                        //scaleIF.text = "";
                        //rotateIF.text = "";
                        break;
                    }
                    else if (curDetectWall != m_curSelectWall)
                    {
                        m_curSelectWall.Select(CustomizingStage.None);
                        m_curSelectWall = curDetectWall;
                        m_curSelectWall.Select(m_curStage);
                        modelController.GetWallInfo(m_curSelectWall.transform.position.x, m_curSelectWall.transform.position.z, m_curSelectWall.Scale, m_curSelectWall.Rotate);

                        break;
                    }

                    if (curDetectMark)
                    {
                        curSelectMark = curDetectMark;
                        curSelectMark.Select(true);                        
                    }
                }
                else if (Input.GetMouseButton(0) && curSelectMark)
                {
                    bool referenceAxis = true;
                    if (curSelectMark.ToString() == "Rotate1 (Mark)")
                        referenceAxis = false;

                    SetWallPosition(referenceAxis, floorHit, false);
                    SetChange();                    
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (curSelectMark)
                    {
                        curSelectMark.Select(false);
                        curSelectMark = null;
                    }
                }
                
                break;
        }
        
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (m_curSelectWall != null)
            {
                //m_curSelectWall.
                Destroy(m_curSelectWall.gameObject);
                CustomDoorSH.Instance.RemoveDoor(m_curSelectWall);
                m_dicWalls[modelController.CurrentScene.SceneName].Remove(m_curSelectWall.gameObject);
                m_curSelectWall = null;

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
        curDetectMark = null;
        foreach (var item in hits)
        {
            Mark curMark = item.collider.GetComponent<Mark>();
            float curDist = (item.point - Camera.main.transform.position).sqrMagnitude;
            if (curMark)
            {
                if (curDist < closestDist)
                {
                    curDetectMark = curMark;
                    closestDist = curDist;
                }
            }
        }

        curDetectWall = null;
        foreach (var item in hits)
        {
            curDetectWall = item.collider.GetComponentInParent<Wall>();
            if (curDetectWall)
            {
                break;
            }
        }
    }

    private void SetWallPosition(bool referenceAxis, Vector3 vTo, bool chgScale)
    {
        // 0. 기존 좌표
        Vector3 vFrom = m_curSelectWall.GetWallPoint(referenceAxis);

        // 1. 변경된 크기 적용 (기존 좌표에서 마우스 좌표까지)
        if (chgScale)
            m_curSelectWall.Scale = Vector3.Distance(vFrom, vTo);

        // 2. 회전하기
        float deltaRot = Vector3.SignedAngle(Vector3.right, vTo - vFrom, Vector3.up);
        if (referenceAxis)
            deltaRot -= 90;
        else
            deltaRot += 90;
        Vector3 currentAngle = m_curSelectWall.transform.localEulerAngles;
        m_curSelectWall.transform.localEulerAngles = new Vector3(currentAngle.x, deltaRot, currentAngle.z); 

        // 3. 회전한 후 좌표
        Vector3 p11 = m_curSelectWall.GetWallPoint(referenceAxis);
        Vector3 vNewCenter = m_curSelectWall.transform.localPosition + vFrom - p11;

        m_curSelectWall.transform.localPosition = vNewCenter;
        // 출입문도 같이 이동
        if (m_curSelectWall.Doors != null && m_curSelectWall.Doors.Count > 0)
        {
            for (int i = 0; i < m_curSelectWall.Doors.Count; i++)
            {
                m_curSelectWall.Doors[i].transform.localPosition = vNewCenter;
                m_curSelectWall.Doors[i].transform.localEulerAngles = new Vector3(currentAngle.x, deltaRot, currentAngle.z); 
            }
        }

        m_curSelectWall.UpdateScale(); // Mark 좌표 업데이트
    }
    
    private Wall FindNearWall(Vector3 fromV, ref Vector3 vec)
    {
        if (m_dicWalls != null)
        {
            float fZoom = modelController.GetZoomSize() * 0.03f;
            List<GameObject> walls = m_dicWalls[modelController.CurrentScene.SceneName];
            //foreach (KeyValuePair<string, List<GameObject>> item in m_dicWalls)
            //{
                foreach (GameObject obj in walls)
                {
                    Wall wall = obj.GetComponent<Wall>();

                    if (m_curSelectWall == wall)
                        continue;

                    Vector3 toV1 = wall.GetWallPoint(true);

                    float distance = Vector3.Distance(fromV, toV1);

                    if (distance <= fZoom)
                    {
                        vec = toV1;
                        return wall;
                    }
                    else
                    {
                        Vector3 toV2 = wall.GetWallPoint(false);
                        float distance2 = Vector3.Distance(fromV, toV2);
                        if (distance2 <= fZoom)
                        {
                            vec = toV2;
                            return wall;
                        }
                    }
                }
            //} 
        }

        return null;
    }

    private float GetScale(Vector3 vFrom, Vector3 vTo)
    {
        if (m_curSelectWall == null)
            return 0.0f;

        Vector3 p1 = vFrom; // 기준점        
        Vector3 mousePT = vTo; 
        
        float scale = Vector3.Distance(p1, mousePT);
        
        return scale;
    }

    private float GetAngle(bool left)
    {
        if (m_curSelectWall == null)
            return 0.0f;

        Vector3 center = m_curSelectWall.transform.localPosition;
        Vector3 p1 = m_curSelectWall.GetWallPoint(left); // 기준점        
        Vector3 mousePT = floorHit; // 마우스 좌표

        float halfScale = m_curSelectWall.Scale / 2;
        float aa = mousePT.x - center.x + halfScale;
        float angle = Mathf.Sin(-aa) * Mathf.Rad2Deg;
        Debug.Log("Angle : ");
        
        return 0.0f;
    }

    public void BT_SpawnWall()
    {
        try
        {
            ClearSelectedWall();

            if (!curSpawnWall)
            {
                curSpawnWall = Instantiate(wallPref).GetComponent<Wall>();

                if (curSpawnWall.mat == null)
                {
                    curSpawnWall.mat = curSpawnWall.mesh.GetComponent<MeshRenderer>().material;
                    curSpawnWall.mat.SetColor("_TopColor", m_editModeColor);
                }
            }

            if (modelController.CurrentScene.SceneName.Substring(0, 1) != "h") // 호텔만 0도로 시작함
            {
                curSpawnWall.transform.rotation = Quaternion.Euler(0, -90f, 0);
            }



            m_curStage = CustomizingStage.Spawn;
        }
        catch (Exception ex)
        {
            MainModel.WriteLog("[ERROR] " + ex.Message);
        }
    }
    public void BT_MoveMode()
    {
        if (m_curSelectWall)
        {
            m_curSelectWall.Select(CustomizingStage.Mode_Move);
            m_curStage = CustomizingStage.Mode_Move;

            SnapController.Init(m_dicWalls[modelController.CurrentScene.SceneName], 1.0f);
        }
    }
    public void BT_RotateMode()
    {
        if (m_curSelectWall)
        {
            m_curSelectWall.Select(CustomizingStage.Mode_Rotate);
            m_curStage = CustomizingStage.Mode_Rotate;
        }
    }
    public void BT_ScaleMode()
    {
        if (m_curSelectWall)
        {
            m_curSelectWall.Select(CustomizingStage.Mode_Scale);
            m_curStage = CustomizingStage.Mode_Scale;
        }
    }
    public void IF_ScaleEnd()
    {
        if (CustomFunction.IsNumber(scaleIF.text) && m_curSelectWall)
        {
            float scaleValue = float.Parse(scaleIF.text);

            if (scaleValue > 0.0f)
            {
                m_curSelectWall.ScaleFromCenter(scaleValue);
            }
        }
    }
    public void IF_RotateEnd()
    {
        if (CustomFunction.IsNumber(rotateIF.text) && m_curSelectWall)
        {
            float rotValue = float.Parse(rotateIF.text);

            m_curSelectWall.transform.rotation = Quaternion.Euler(0, rotValue, 0);
        }
    }

    /// <summary>
    /// 해당 Scene에 있는 가벽들의 정보를 파일에 쓴다
    /// </summary>
    /// <param name="path">Directory</param>
    public void GetWalls(string path)
    {
        foreach (KeyValuePair<string, bool> item in m_dicChgWalls)
        {
            // 수정된 가벽이 있는 scene만 파일로 쓴다
            if (!item.Value)
                continue;

            string strSceneName = item.Key;

            if (!m_dicWalls.ContainsKey(strSceneName))
                continue;

            string fullPath = path + strSceneName + ".txt";

            List<GameObject> objs = m_dicWalls[strSceneName];

            using (StreamWriter sw = new StreamWriter(fullPath, false, System.Text.Encoding.UTF8))
            {
                if (objs == null || objs.Count == 0)
                {
                    sw.WriteLine("");
                    continue;
                }

                foreach (GameObject obj in objs)
                {
                    Wall wall = obj.GetComponent<Wall>();
                    string line = string.Format("{0},{1},{2},{3},{4}", wall.transform.position.x, wall.transform.position.y, wall.transform.position.z, wall.Rotate, wall.Scale);

                    if (wall.Doors != null && wall.Doors.Count > 0)
                    {
                        line += "," + wall.Doors.Count;
                        foreach (DoorSH door in wall.Doors)
                        {
                            line += string.Format(",{0},{1},{2}", door.transform.position.x, door.transform.position.y, door.transform.position.z);
                        }
                    }
                    else
                    {
                        line += ",0";
                    }
                    sw.WriteLine(line);
                }
            }
        }

        m_dicChgWalls.Clear();
    }

    /// <summary>
    /// 파일에 있는 가벽정보를 읽어서 해당 Scene에 추가한다
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sceneName"></param>
    public void LoadWalls(string path, string sceneName)
    {
        // 다른 층 가벽 숨기기
        foreach (KeyValuePair<string, List<GameObject>> item in m_dicWalls)
        {
            foreach (GameObject item2 in item.Value)
            {
                Destroy(item2);
            }
        }

        if (!m_dicWalls.ContainsKey(sceneName))
            m_dicWalls.Add(sceneName, new List<GameObject>());
        else
        {
            foreach (KeyValuePair<string, List<GameObject>> item in m_dicWalls)
            {
                if (item.Key == sceneName)
                {
                    foreach (GameObject item2 in item.Value)
                    {
                        Destroy(item2);
                    } 
                }
            }
                        
            m_dicWalls[sceneName].Clear();
            m_dicChgWalls[modelController.CurrentScene.SceneName] = false;
        }
        
        if (!File.Exists(path))
            return;
        
        using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
        {
            while (sr.EndOfStream == false)
            {
                string strLine = sr.ReadLine().Trim();

                if (strLine.Length == 0)
                    return;

                string[] args = strLine.Split(',');
                if (args.Length < 6)
                    continue;
                
                float x;
                float y;
                float z;
                float rotate;
                float scale;
                int doorCount;

                if (!float.TryParse(args[0], out x) || !float.TryParse(args[1], out y) || !float.TryParse(args[2], out z) || !float.TryParse(args[3], out rotate) || !float.TryParse(args[4], out scale) || !int.TryParse(args[5], out doorCount))
                    continue;
                
                Wall wall = Instantiate(wallPref).GetComponent<Wall>();
                wall.mat = wall.mesh.GetComponent<MeshRenderer>().material;
                wall.transform.position = new Vector3(x, y, z);
                wall.transform.rotation = Quaternion.Euler(0, rotate, 0);
                wall.Scale = scale;
                wall.UpdateScale();

                if (doorCount > 0)
                {
                    for (int i = 0; i < doorCount; i++)
                    {
                        int index = i * 3;

                        float doorX;
                        float doorY;
                        float doorZ;
                        if (!float.TryParse(args[6 + index], out doorX) || !float.TryParse(args[7 + index], out doorY) || !float.TryParse(args[8 + index], out doorZ))
                            continue;

                        DoorSH door = CustomDoorSH.Instance.InstantiateDoor();
                        if (door == null)
                            continue;

                        door.transform.position = new Vector3(doorX, doorY, doorZ);
                        door.transform.rotation = Quaternion.Euler(0, rotate, 0);
                        door.SetWall(wall);

                        if (!CustomDoorSH.Instance.DicDoors.ContainsKey(modelController.CurrentScene.SceneName))
                            CustomDoorSH.Instance.DicDoors.Add(modelController.CurrentScene.SceneName, new List<GameObject>());
                        CustomDoorSH.Instance.DicDoors[modelController.CurrentScene.SceneName].Add(door.gameObject);
                    }
                }

                m_dicWalls[sceneName].Add(wall.gameObject);                
            }
        }

        SetWallColor();
        //MainModel.WriteLog("Wall Count " + sceneName + " : "+ m_dicWalls[sceneName].Count);
    }
    
    // 가벽이 수정되었음을 SDMS에 알려준다
    // false 에서 true가 될때 최초 한번만 알려줌 (저장 버튼 활성화를 위해서)
    private void SetChange()
    {
        if (!m_dicChgWalls.ContainsKey(modelController.CurrentScene.SceneName) || m_dicChgWalls[modelController.CurrentScene.SceneName] == false)
        {
            m_dicChgWalls[modelController.CurrentScene.SceneName] = true;
            modelController.ChangeWall();
        }
    }

    public void ClearSelectedWall()
    {
        m_curStage = CustomizingStage.None;
        if (m_curSelectWall != null)
        {
            m_curSelectWall.Select(CustomizingStage.None);
            m_curSelectWall = null;
            curSelectMark = null;
        }
    }

    public void ResetChagne()
    {
        m_dicChgWalls[modelController.CurrentScene.SceneName] = false;
    }

    private Color m_editModeColor = Color.red;
    private Color m_noneModeColor = new Color((float)0.18, (float)0.18, (float)0.18);
    public void SetWallColor()
    {
        try
        {
            if (m_dicWalls == null)
                return;

            Color color = m_noneModeColor;
            if (modelController.EditMode && modelController.WallEditMode)
                color = m_editModeColor;

            foreach (KeyValuePair<string, List<GameObject>> item in m_dicWalls)
            {
                if (item.Value == null || item.Value.Count == 0)
                    continue;

                if (item.Key != modelController.CurrentScene.SceneName)
                    continue;
                
                foreach (GameObject obj in item.Value)
                {
                    Wall wall = obj.GetComponent<Wall>();
                    if (wall == null || wall.mat == null)
                        continue;
                    
                    wall.mat.SetColor("_TopColor", color);
                    
                }
            }
        }
        catch (Exception ex)
        {
            MainModel.WriteLog("[ERROR] CustomizingController.SetWallColor() : " + ex.Message);
        }
    }

    private bool m_bUseSnap = true;
    public void SetUseSnap(bool use)
    {
        m_bUseSnap = use;
    }
}
