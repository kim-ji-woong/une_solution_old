using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomSpaceText : MonoBehaviour
{
    public GameObject SpaceTextPref;
    
    private static CustomSpaceText m_instance = null;
    public static CustomSpaceText Instance
    {
        get { return m_instance; }
    }

    private MainModel modelController;

    private CustomizingStage m_curStage = CustomizingStage.None;
    private SpaceText m_curSpawnText = null;
    private SpaceText m_curDetectText = null;
    private SpaceText m_curSelectText = null;
    private Mark m_curDetectMark = null;
    private Mark m_curSelectMark = null;
    private Vector3 m_firstFHit;
    private Vector3 m_firstPos;
    private bool m_isHitFloor = false;
    private Vector3 m_floorHit;

    private List<SpaceText> m_spaceTexts = new List<SpaceText>();
    private bool m_bIsChg = false;

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        modelController = FindObjectOfType<MainModel>();
    }
    
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (ModelManager.Instance.Model.RotatePOI)
        {
            foreach (SpaceText item in m_spaceTexts)
            {
                item.transform.rotation = Quaternion.Euler(90, 90, 0);
            }
            //gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y + 90, gameObject.transform.eulerAngles.z);
        }
        else
        {
            foreach (SpaceText item in m_spaceTexts)
            {
                Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
                Vector3 heading = Camera.main.transform.position + (ray1.direction * 1000000000.0f);
                item.transform.LookAt(heading); 
            }
        }

        if (!ModelManager.Instance.Model.EditMode) // 편집모드
            return;

        if (!ModelManager.Instance.Model.WallEditMode) // 가벽 편집모드
            return;
        
        Detect();

        switch (m_curStage)
        {
            case CustomizingStage.None:
                if (Input.GetMouseButtonDown(0))
                {
                    if (m_curDetectText != null)
                    {
                        m_curSelectText = m_curDetectText;
                        m_curSelectText.Select(CustomizingStage.Mode_Move);
                        
                        m_curStage = CustomizingStage.Mode_Move;
                    }
                }
                break;
            case CustomizingStage.Spawn:
                m_curSpawnText.transform.position = m_floorHit;
                m_curSpawnText.transform.rotation = Quaternion.Euler(90, 90, 0);
                if (Input.GetMouseButtonDown(0))
                {
                    if (m_isHitFloor)
                    {
                        if (!m_spaceTexts.Contains(m_curSpawnText))
                            m_spaceTexts.Add(m_curSpawnText);

                        SetSize(m_curSpawnText);

                        m_curSpawnText = null;

                        m_curStage = CustomizingStage.None;

                        
                        SetChange();
                    }
                }
                break;
            case CustomizingStage.Mode_Move:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!m_curDetectText)
                    {
                        ClearSelectedText();
                        break;
                    }
                    else if (m_curDetectText != m_curSelectText)
                    {
                        m_curSelectText.Select(CustomizingStage.None);
                        m_curSelectText = m_curDetectText;
                        m_curSelectText.Select(m_curStage);
                        break;
                    }

                    if (m_curDetectMark)
                    {
                        m_firstFHit = m_floorHit;
                        m_firstPos = m_curSelectText.transform.position;
                        m_curSelectMark = m_curDetectMark;
                        m_curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && m_curSelectMark)
                {
                    switch (m_curSelectMark.kind)
                    {
                        case TransformKind.MoveText_X:
                            {
                                Vector3 newPos = m_firstPos + m_curSelectText.transform.right * Vector3.Dot(m_curSelectText.transform.right, CustomizingController.Instance.FloorHit - m_firstFHit);
                                newPos.x = SnapController.XPos(newPos, m_curSelectText.GetComponent<SnapObj>());

                                m_curSelectText.transform.position = newPos;
                            }
                            break;
                        case TransformKind.MoveText_Z:
                            m_curSelectText.transform.position = m_firstPos + m_curSelectText.transform.forward * Vector3.Dot(m_curSelectText.transform.forward, CustomizingController.Instance.FloorHit - m_firstFHit);
                            break;
                        case TransformKind.MoveText_Both:
                            m_curSelectText.transform.position = m_firstPos + m_floorHit - m_firstFHit;
                            break;
                    }

                    SetChange();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (m_curSelectMark)
                    {
                        m_curSelectMark.Select(false);
                        m_curSelectMark = null;
                    }
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (m_curSelectText!= null)
            {
                Destroy(m_curSelectText.gameObject);
                m_spaceTexts.Remove(m_curSelectText);
                m_curSelectText = null;

                m_curStage = CustomizingStage.None;
                SetChange();
            }
        }
    }
    
    private void Detect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        m_isHitFloor = modelController.CurrentScene.DetectFloor(ray.origin, ray.direction, out m_floorHit);

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

        m_curDetectText = null;
        foreach (var item in hits)
        {
            m_curDetectText = item.collider.GetComponentInParent<SpaceText>();
            if (m_curDetectText != null)
                break;
        }
    }

    public void AddSpaceText(string strTxt)
    {
        ClearSelectedText();

        m_curSpawnText = Instantiate(SpaceTextPref).GetComponent<SpaceText>();
        TextMesh mesh = m_curSpawnText.GetComponent<TextMesh>();
        mesh.text = strTxt;
        mesh.color = m_editModeColor;
        m_curStage = CustomizingStage.Spawn;
    }

    private float GetWidth(TextMesh mesh)
    {
        float width = 0;
        foreach (char symbol in mesh.text)
        {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
            {
                width += info.advance;
            }
        }

        return width;
    }

    // 수정되었음을 SDMS에 알려준다
    // false 에서 true가 될때 최초 한번만 알려줌 (저장 버튼 활성화를 위해서)
    private void SetChange()
    {
        if (!m_bIsChg)
        {
            m_bIsChg = true;
            modelController.ChangeSpaceText();
        }
    }

    public void ClearSelectedText()
    {
        m_curStage = CustomizingStage.None;
        if (m_curSelectText != null)
        {
            m_curSelectText.Select(CustomizingStage.None);
            m_curSelectText = null;
            m_curSelectMark = null;
        }
    }

    private Color m_editModeColor = Color.red;
    private Color m_noneModeColor = Color.black;
    public void SetSpaceTextColor()
    {
        Color color = m_noneModeColor;

        if (modelController.EditMode && modelController.WallEditMode)
            color = m_editModeColor;
                
        foreach (SpaceText obj in m_spaceTexts)
        {
            TextMesh mesh = obj.GetComponent<TextMesh>();
            mesh.color = color;
        }
    }

    private void SetSize(SpaceText text)
    {
        TextMesh mesh = text.GetComponent<TextMesh>();
        float width = GetWidth(mesh);
        float x = width / 10;

        BoxCollider box = text.GetComponent<BoxCollider>();
        box.size = new Vector3(x, box.size.y, box.size.z);

        BoxCollider box2 = text.MoveBothMark.GetComponent<BoxCollider>();
        box2.size = new Vector3(x, box2.size.y, box2.size.z);

        Transform ts = text.MoveBothMark.gameObject.transform.GetChild(0);
        ts.localScale = new Vector3(x, ts.localScale.y, ts.localScale.z);
    }

    /// <summary>
    /// 파일에 있는 정보를 읽어서 해당 Scene에 추가한다
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sceneName"></param>
    public void LoadSpaceText(string path, string sceneName)
    {
        foreach (SpaceText item in m_spaceTexts)            
            Destroy(item.gameObject);

        m_spaceTexts.Clear();
        m_bIsChg = false;
        
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
                if (args.Length != 4)
                    continue;

                string strText;
                float x;
                float y;
                float z;

                if (!float.TryParse(args[1], out x) || !float.TryParse(args[2], out y) || !float.TryParse(args[3], out z))
                    continue;

                strText = args[0];

                SpaceText text = Instantiate(SpaceTextPref).GetComponent<SpaceText>();                
                text.transform.position = new Vector3(x, y, z);
                text.transform.rotation = Quaternion.Euler(90, 90, 0);

                TextMesh mesh = text.GetComponent<TextMesh>();
                mesh.text = strText;

                SetSize(text);
                
                m_spaceTexts.Add(text);
            }
        }

        SetSpaceTextColor();
        //MainModel.WriteLog("text Count " + sceneName + " : " + m_spaceTexts.Count);
    }

    /// <summary>
    /// 해당 Scene에 있는 가벽들의 정보를 파일에 쓴다
    /// </summary>
    /// <param name="path">Directory</param>
    public void GetSpaceText(string path)
    {
        // 수정된 scene만 파일로 쓴다
        if (!m_bIsChg)
            return;

        string strSceneName = modelController.CurrentScene.SceneName;
        
        string fullPath = path + strSceneName + ".txt";
        
        using (StreamWriter sw = new StreamWriter(fullPath, false, System.Text.Encoding.UTF8))
        {
            if (m_spaceTexts == null || m_spaceTexts.Count == 0)
            {
                sw.WriteLine("");
            }
            else
            {
                foreach (SpaceText obj in m_spaceTexts)
                {
                    TextMesh mesh = obj.GetComponent<TextMesh>();
                     
                    string line = string.Format("{0},{1},{2},{3}", mesh.text, obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
                    sw.WriteLine(line);
                }
            }
        }

        m_bIsChg = false;
    }

    public static bool bShow = true;
    public void VisibleSpaceText(bool bShow)
    {
        if (m_spaceTexts == null || m_spaceTexts.Count == 0)
            return;

        foreach (SpaceText item in m_spaceTexts)
        {
            item.gameObject.SetActive(bShow);
        }
    }
}
