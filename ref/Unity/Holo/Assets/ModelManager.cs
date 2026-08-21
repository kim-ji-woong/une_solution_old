using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ModelManager : MonoBehaviour {

    public enum MouseMode
    {
        mode_None = 0,
        mode_orbit,
        mode_pan
    }

    public enum ModelIndex
    {
        model_over = 0,
        model_under,
        model_base,
        model_new
    }

    private MenuManager m_MenuManager = null;

    private MouseMode m_mouseMode = MouseMode.mode_None;
    private Bounds m_Bounds = new Bounds(); // 전체 모델의 바운드
    private Vector3 m_vPivot = Vector3.zero;
    private bool[] m_arrActive = new bool[4] { true, true, true, false };

    private bool m_bMoving = false;
    private int m_nModelType = -1;
    private int m_nCommand = 0;

    private float m_fAngle = 0;
    private float m_fMove = 0;
    private float m_fScale = 0;
    private int m_nRotateSpeed = 5;
    private int m_nMoveSpeed = 5;
    private float m_fScaleSpeed = 0.01f;

    private Vector3 m_vOrigin = Vector3.zero;
    private Vector3 m_rOrigin = Vector3.zero;
    private Vector3 m_sOrigin = Vector3.zero;

    private Vector3[] m_vDesPos = new Vector3[5];
    private Vector3[] m_vDesRot = new Vector3[5];
    private Vector3[] m_vDesScale = new Vector3[5];

    public MouseMode mouseMode
    {
        get { return m_mouseMode; }
        set { m_mouseMode = value; }
    }

    public Bounds bounds { get { return m_Bounds; } }
    public Vector3 pivot { set { m_vPivot = value; } }
    public bool IsMoving { get { return m_bMoving; } }
    public int modelType { set { m_nModelType = value; } }

    // Use this for initialization
    void Start () {
        //m_MenuManager = transform.parent.GetComponent<MenuManager>();
        m_vDesPos[0] = new Vector3(0, -0.8f, 1.5f);
        m_vDesPos[1] = new Vector3(-0.5f, -1.1f, 1.8f);
        m_vDesPos[2] = new Vector3(-96.8f, -12f, -30.7f);
        m_vDesPos[3] = new Vector3(-109.0f, -4.9f, -155.5f);
        m_vDesPos[4] = new Vector3(-141.0f, -21f, -200f);

        m_vDesRot[0] = new Vector3(0, 125f, 0);
        m_vDesRot[1] = new Vector3(0, 125f, 0);
        m_vDesRot[2] = new Vector3(0, 125f, 0);
        m_vDesRot[3] = new Vector3(0, 125f, 0);
        m_vDesRot[4] = new Vector3(0, 125f, 0);

        m_vDesScale[0] = new Vector3(0.0004f, 0.0004f, 0.0004f);
        m_vDesScale[1] = new Vector3(0.01f, 0.01f, 0.01f);
        m_vDesScale[2] = new Vector3(1f, 1f, 1f);
        m_vDesScale[3] = new Vector3(1f, 1f, 1f);
        m_vDesScale[4] = new Vector3(1f, 1f, 1f);
    }
	
	// Update is called once per frame
	void Update () {
        if (transform == null || transform.childCount == 0)
            return;

        Transform child = transform.GetChild(0);

        bool bKeyDown = false;

        if (!m_bMoving)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_nModelType = 0;
                bKeyDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_nModelType = 1;
                bKeyDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_nModelType = 2;
                bKeyDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                m_nModelType = 3;
                bKeyDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                m_nModelType = 4;
                bKeyDown = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
            ShowHide(0);
        else if (Input.GetKeyDown(KeyCode.W))
            ShowHide(1);
        else if (Input.GetKeyDown(KeyCode.E))
            ShowHide(2);
        else if (Input.GetKeyDown(KeyCode.R))
            ShowHide(3);
        else if (Input.GetKeyDown(KeyCode.T))
            ShowHide(4);
        else if (Input.GetKeyDown(KeyCode.Y))
            ShowHide(5);
        else if (Input.GetKeyDown(KeyCode.M))
            ShowMenu();

        if (Input.GetKey(KeyCode.UpArrow))
            ModelTranslate(0);
        else if (Input.GetKey(KeyCode.DownArrow))
            ModelTranslate(1);
        else if (Input.GetKey(KeyCode.LeftArrow))
            ModelTranslate(2);
        else if (Input.GetKey(KeyCode.RightArrow))
            ModelTranslate(3);
        else if(Input.GetKey(KeyCode.A))
        {
            m_vPivot = Camera.main.transform.position;
            UpdateRotation(transform, Vector3.right);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_vPivot = Camera.main.transform.position;
            UpdateRotation(transform, -Vector3.right);
        }

        if (bKeyDown)
        {
            KeyDown();
        }

        Move();

        if (m_nCommand == 0)
        {
            m_nModelType = -1;
            m_bMoving = false;
        }
    }

    public void KeyDown()
    {
        m_bMoving = true;
        m_nCommand = 1;

        Transform child = transform.GetChild(0);

        m_vOrigin = child.transform.position;
        m_rOrigin = child.transform.eulerAngles;
        m_sOrigin = child.transform.localScale;

        m_fAngle = m_rOrigin.y - m_vDesRot[m_nModelType].y;
        m_fMove = 0;
        m_fScale = m_sOrigin.x;

        if (m_nModelType <= 1)
        {
            child.GetChild(0).gameObject.SetActive(true);
            m_arrActive[0] = true;
        }
        else
        {
            child.GetChild(0).gameObject.SetActive(false);
            m_arrActive[1] = false;
        }
    }

    public void AddBounds(Bounds bounds)
    {
        if (m_Bounds.size == Vector3.zero)
        {
            m_Bounds.SetMinMax(bounds.min, bounds.max);
        }
        else
            m_Bounds.Encapsulate(bounds);
    }

    public void SetMouseMode(int nMode)
    {
        m_mouseMode = (MouseMode)nMode;
    }

    public void ShowModel(ModelIndex model, bool bShow)
    {
        int idx = (int)model;
        Transform child = transform.GetChild(idx);
        child.gameObject.SetActive(bShow);
        m_arrActive[idx] = bShow;

        // 지상과 지하 다 숨으면 안됨.
        // 다른 하나 보여주기
        int idx2 = (idx + 1) % 2;
        if (!bShow)
        {
            Transform t = transform.GetChild(idx2);
            t.gameObject.SetActive(true);
            m_arrActive[idx2] = true;
        }
    }

    public void UpdateRotation(Transform parent, Vector3 dir)
    {
        if (parent == null)
            parent = transform;

        foreach (Transform child in parent)
        {
            MainModel model = child.GetComponent<MainModel>();
            if (model != null)
            {
                model.UpdateRotation2(m_vPivot, dir);
                continue;
            }
            
            UpdateRotation(child, dir);
        }
    }

    public void UpdateTranslate(Transform parent, Vector3 move)
    {
        if (parent == null)
            parent = transform;

        foreach (Transform child in parent)
        {
            MainModel model = child.GetComponent<MainModel>();
            if (model != null)
            {
                model.UpdateTranslate2(move);
                continue;
            }
            
            UpdateTranslate(child, move);
        }
    }

    public void Move()
    {
        Transform child = transform.GetChild(0);

        if (m_nCommand == 1)
        {
            if (EqualVector(m_rOrigin, m_vDesRot[m_nModelType]))
            {
                m_nCommand = 2;
                return;
            }
            UpdateRotate();
        }
        else if (m_nCommand == 2)
        {
            if (EqualVector(m_sOrigin, m_vDesScale[m_nModelType]))
            {
                m_nCommand = 3;
                return;
            }

            if (m_sOrigin.x < m_vDesScale[m_nModelType].x)
            {
                m_fScaleSpeed = 0.0005f;
                if (child.transform.localScale.x >= 0.1f)
                    m_fScaleSpeed = 0.1f;
                else if (child.transform.localScale.x >= 0.01f)
                    m_fScaleSpeed = 0.01f;
                else if (child.transform.localScale.x >= 0.001f)
                    m_fScaleSpeed = 0.001f;
            }
            else
            {
                m_fScaleSpeed = 0.1f;
                if (child.transform.localScale.x <= 0.001f)
                    m_fScaleSpeed = 0.0005f;
                else if (child.transform.localScale.x <= 0.01f)
                    m_fScaleSpeed = 0.001f;
                else if (child.transform.localScale.x <= 0.1f)
                    m_fScaleSpeed = 0.01f;
            }

            UpdateScale(m_vDesScale[m_nModelType].x);
        }
        else if (m_nCommand == 3)
        {
            if (EqualVector(m_vOrigin, m_vDesPos[m_nModelType]))
            {
                m_nCommand = 0;
                return;
            }
            UpdateMove(m_vDesPos[m_nModelType]);
        }
    }

    private void UpdateRotate()
    {
        Transform child = transform.GetChild(0);

        int nSign = 1;
        if (m_fAngle < 0)
            nSign = -1;

        float angle = m_nRotateSpeed;

        if (m_fAngle * nSign < m_nRotateSpeed)
        {
            angle = m_fAngle;
            m_fAngle = 0;
        }
        else
            m_fAngle -= m_nRotateSpeed * nSign;

        child.RotateAround(m_vOrigin, Vector3.up, angle * -nSign);

        if (m_fAngle == 0)
            m_nCommand = 2;
    }

    public void UpdateMove(Vector3 vDest)
    {
        Transform child = transform.GetChild(0);

        float len = Vector3.Distance(m_vOrigin, vDest);

        m_fMove += m_nMoveSpeed;
        if (m_fMove >= len)
        {
            m_fMove = len;
            m_nCommand = 0;
        }

        Vector3 dir = (vDest - m_vOrigin).normalized;
        Ray ray = new Ray(m_vOrigin, dir);
        Vector3 movePos = ray.GetPoint(m_fMove);

        child.position = movePos;
    }

    public void UpdateScale(float fScale)
    {
        Transform child = transform.GetChild(0);

        int nSign = 1;
        if (fScale < m_sOrigin.x)
            nSign = -1;

        m_fScale += m_fScaleSpeed * nSign;
        if(nSign == 1 && m_fScale > fScale)
        {
            m_fScale = fScale;
            m_nCommand = 3;
        }
        else if(nSign == -1 && m_fScale < fScale)
        {
            m_fScale = fScale;
            m_nCommand = 3;
        }

        child.localScale = new Vector3(m_fScale, m_fScale, m_fScale);
    }

    public void ShowMenu()
    {
        if (m_MenuManager == null)
            m_MenuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

        if (!m_MenuManager.visible)
            mouseMode = MouseMode.mode_None;

        m_MenuManager.Visible(!m_MenuManager.visible);
    }

    public bool IsActive(ModelIndex model)
    {
        return m_arrActive[(int)model];
    }

    public void ScaleChange()
    {
        if (transform.localScale.x < 0.1)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    public void ModelTranslate(int nDir)
    {
        Vector3 vDir = Vector3.zero;

        if (nDir == 0) // forward
        {
            vDir = -Camera.main.transform.forward;
        }
        else if (nDir == 1) // back
        {
            vDir = Camera.main.transform.forward;
        }
        else if (nDir == 2) // left
            vDir = Camera.main.transform.right;
        else // right
            vDir = -Camera.main.transform.right;

        Transform child = transform.GetChild(0);
        child.transform.Translate(vDir * 4 * Time.deltaTime, Space.World);
    }

    public bool ShowHide(int nModel)
    {
        Transform t = transform.GetChild(0);
        Transform bim = t.GetChild(1);
        if (nModel == 0)  // 지형
        {
            m_arrActive[0] = !m_arrActive[0];
            t.GetChild(0).gameObject.SetActive(m_arrActive[0]);
            return m_arrActive[0];
        }
        else if(nModel == 1) // 지하차도
        {
            m_arrActive[1] = !m_arrActive[1];
            bim.GetChild(0).gameObject.SetActive(m_arrActive[1]);
            return m_arrActive[1];
        }
        else if (nModel == 2) // 기존
        {
            m_arrActive[2] = !m_arrActive[2];
            bim.GetChild(1).gameObject.SetActive(m_arrActive[2]);
            bim.GetChild(2).gameObject.SetActive(m_arrActive[2]);
            t.GetChild(2).gameObject.SetActive(m_arrActive[2]); // cube(충돌지점)
            return m_arrActive[2];
        }
        else if (nModel == 3) // 대안
        {
            m_arrActive[3] = !m_arrActive[3];
            bim.GetChild(3).gameObject.SetActive(m_arrActive[3]);
            bim.GetChild(4).gameObject.SetActive(m_arrActive[3]);
            return m_arrActive[3];
        }
        else if (nModel == 4) // 기존 Show
        {
            bim.GetChild(1).gameObject.SetActive(true);
            bim.GetChild(2).gameObject.SetActive(true);
            m_arrActive[2] = true;
            bim.GetChild(3).gameObject.SetActive(false);
            bim.GetChild(4).gameObject.SetActive(false);
            m_arrActive[3] = false;
            t.GetChild(2).gameObject.SetActive(true); // cube(충돌지점)
        }
        else if (nModel == 5) // 대안 Show
        {
            bim.GetChild(1).gameObject.SetActive(false);
            bim.GetChild(2).gameObject.SetActive(false);
            m_arrActive[2] = false;
            bim.GetChild(3).gameObject.SetActive(true);
            bim.GetChild(4).gameObject.SetActive(true);
            m_arrActive[3] = true;
            t.GetChild(2).gameObject.SetActive(false); // cube(충돌지점)
        }
        return true;
    }

    float EPSILON = 0.00001f;
    private bool EqualVector(Vector3 v1, Vector3 v2)
    {
        if ((v1.x < v2.x + EPSILON && v1.x > v2.x - EPSILON)
            && (v1.y < v2.y + EPSILON && v1.y > v2.y - EPSILON)
            && (v1.z < v2.z + EPSILON && v1.z > v2.z - EPSILON))
            return true;

        return false;
    }
}
