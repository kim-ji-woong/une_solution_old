using UnityEngine;
using System.Collections;

public class IconPOI : MonoBehaviour
{

    public bool m_bVisible = true;
    private int m_nID = -1;

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    private Vector3 mPosition;
    public Vector3 Position
    {
        get { return mPosition; }
        set { mPosition = value; }
    }

    private string m_IconName = "";
    public string IconName
    {
        get { return m_IconName; }
        set { m_IconName = value; }
    }


    public Collider2D mColldier;

    void Start()
    {
        //mColldier = gameObject.AddComponent<Collider2D>();
        //SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (m_bVisible == true)
        {
            SpriteRenderer mr2 = gameObject.GetComponent<SpriteRenderer>();
            mr2.enabled = true;

            Vector3 dir = Camera.main.transform.position - gameObject.transform.position;

            if (ModelManager.Instance.FixIconRatio == true)
            {
                float distance = dir.magnitude;
                float ratio = distance / ModelManager.Instance.DistanceRatioIcon;
                gameObject.transform.localScale = new Vector3(ratio, ratio, ratio);
            }

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            Vector3 vec = spriteRenderer.bounds.extents;
            gameObject.transform.position = new Vector3(mPosition.x, mPosition.y + (vec.y), mPosition.z);

            Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            Vector3 heading = Camera.main.transform.position + (ray1.direction * 1000000000.0f);
            gameObject.transform.LookAt(heading);

            Bounds bound = new Bounds(spriteRenderer.bounds.center, spriteRenderer.bounds.extents);
            mColldier.bounds.Encapsulate(bound);
            mColldier.transform.localScale = gameObject.transform.localScale;
            mColldier.transform.LookAt(heading);
        }
    }

    private void LeaveObject()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {

            string szMsg = string.Format("LeavePOI({0})", m_nID);
            //Debug.logger.Log(szMsg + " : Shared Mesh :" + mf.mesh.name);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);

        }
    }


    private bool m_bSelect = false;
    public bool Select
    {
        get { return m_bSelect; }
        set { m_bSelect = value; }
    }

    public void Pick()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            Vector3 vPos = Input.mousePosition;
            string szMsg = string.Format("PickPOI({0},{1},{2})", m_nID, vPos.x, vPos.y);
            //Debug.logger.Log(szMsg + " : Shared Mesh :" + mf.mesh.name);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);

        }
    }

    public void OnSelectPOI(bool bSelect)
    {
        m_bSelect = bSelect;
    }

    public void SelectPOI(bool bSelect)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            m_bSelect = bSelect;
            if (bSelect == true)
            {
                Vector3 vPos = Input.mousePosition;
                string szMsg = string.Format("SelectPOI({0},{1},{2}, {3})", m_nID, vPos.x, vPos.y, bSelect);
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg);
            }
        }
    }

    private void EnterObject()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            Vector3 vPos = Input.mousePosition;
            string szMsg = string.Format("EnterPOI({0},{1},{2})", m_nID, vPos.x, vPos.y);
            //Debug.logger.Log(szMsg + " : Shared Mesh :" + mf.mesh.name);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
        }
    }


    void OnMouseEnter()
    {
        float x = Input.mousePosition.x;
        float y = (Screen.height - Input.mousePosition.y);
        if ((x > 0 && y > 0) && (x < Screen.width && y < Screen.height))
        {
            EnterObject();
        }
    }

    void OnMouseExit()
    {
        float x = Input.mousePosition.x;
        float y = (Screen.height - Input.mousePosition.y);
        if ((x > 0 && y > 0) && (x < Screen.width && y < Screen.height))
        {
            LeaveObject();
        }
    }

    void OnMouseUp()
    {
        if (m_bDragPOI == false)
        {
            if (m_bVisible == true)
            {
                Pick();
            }
        }
        else
        {
            // Send Drag POI
            OnMovePOI();
        }
        m_bDragPOI = false;
    }

    void OnMovePOI()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            Vector3 vPos = mPosition;
            string szMsg = string.Format("OnDragPOI({0},{1},{2},{3})", m_nID, vPos.x, vPos.y, vPos.z);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
        }
    }


    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        m_bDragPOI = false;
        screenPoint = Input.mousePosition;
        offset = mPosition - ModelManager.Instance.Model.ScreenToGlobal((int)Input.mousePosition.x, (int)Input.mousePosition.y); ;
    }

    private bool m_bDragPOI = false;
    void OnMouseDrag()
    {
        if (m_bVisible == true)
        {
            if (ModelManager.Instance.Model.EditMode == true)
            {
                if (ModelManager.Instance.Model.Mode == MainModel.MouseWorkMode.PICK)
                {
                    if (screenPoint != Input.mousePosition)
                    {
                        Vector3 curScreenPoint = Input.mousePosition;
                        Vector3 curPosition = ModelManager.Instance.Model.ScreenToGlobal((int)curScreenPoint.x, (int)curScreenPoint.y);

                        mPosition = curPosition + offset;
                        m_bDragPOI = true;
                    }

                }
            }
        }
    }

    public void SetVisible(bool bVisible)
    {
        m_bVisible = bVisible;
        SpriteRenderer mr2 = gameObject.GetComponent<SpriteRenderer>();
        mr2.enabled = false;
    }
}
