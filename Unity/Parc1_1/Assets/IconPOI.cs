using UnityEngine;
using System.Collections;

public class IconPOI : MonoBehaviour {

    public bool m_bVisible = true;
    private int m_nID = -1;

    private static IconPOI m_clickedPOI = null;

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public Vector3 mPosition;
    public Vector3 Position
    {
        get { return mPosition; }
        set { mPosition = value; }
    }

    private string m_szType = "";
    public string IconType
    {
        get { return m_szType; }
        set { m_szType = value; }
    }

    public int m_nSoringLayerID = 0;

    private string m_IconName = "";
    public string IconName
    {
        get { return m_IconName; }
        set { m_IconName = value; }
    }

    public BoxCollider mColldier;
	void Start ()
    {
        //mColldier = gameObject.AddComponent<Collider2D>();
        //SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();        
    }
	
	void Update ()
    {
        if (m_bVisible == true)
        {
            SpriteRenderer mr2 = gameObject.GetComponent<SpriteRenderer>();
            mr2.enabled = true;

            float ratio = 4.9f;

            if (ModelManager.Instance.Model.EditMode == false)
            {
                Vector3 dir = Camera.main.transform.position - gameObject.transform.position;

                float distance = dir.magnitude;
                ratio = distance / ModelManager.Instance.DistanceRatioIcon;
                ratio *= 7.5f;
            }

            if (ModelManager.Instance.FixIconRatio == true)
            {
                gameObject.transform.localScale = new Vector3(ratio, ratio, ratio);
            }

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            Vector3 vec = spriteRenderer.bounds.extents;
            gameObject.transform.position = new Vector3(mPosition.x, mPosition.y + (vec.y), mPosition.z);

            Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            Vector3 heading = Camera.main.transform.position + (ray1.direction * 1000000000.0f);
            gameObject.transform.LookAt(heading);


            if (m_bSelect == true)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }


            Bounds bound = new Bounds(spriteRenderer.bounds.center, spriteRenderer.bounds.extents);
            mColldier.transform.position = new Vector3(mPosition.x, mPosition.y + (vec.y), mPosition.z);
            mColldier.bounds.Encapsulate(bound);


            // mColldier.transform.localScale = gameObject.transform.localScale;

            mColldier.transform.LookAt(heading);


            Vector3 size = spriteRenderer.bounds.size;
            size.x /= gameObject.transform.localScale.x;
            size.y /= gameObject.transform.localScale.y;
            size.z = 2.0f;
            GetComponent<BoxCollider>().size = size;

            CalcPositons();
            DrawBox();

            //Vector3 start = mColldier.bounds.min;
            //Vector3 end = mColldier.bounds.max;
            //Debug.DrawLine(start, end,Color.green);

            if (ModelManager.Instance != null && ModelManager.Instance.Model != null)
            {
                if (ModelManager.Instance.Model.RotatePOI)
                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y + 90, gameObject.transform.eulerAngles.z);
            }
        }
	}

    //void Update()
    //{
       
    //}

    public Color color = Color.green;

    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;    
     
    void CalcPositons()
    {
        Bounds bounds = GetComponent<BoxCollider>().bounds;


        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner

        v3FrontTopLeft = transform.TransformPoint(v3FrontTopLeft);
        v3FrontTopRight = transform.TransformPoint(v3FrontTopRight);
        v3FrontBottomLeft = transform.TransformPoint(v3FrontBottomLeft);
        v3FrontBottomRight = transform.TransformPoint(v3FrontBottomRight);
        v3BackTopLeft = transform.TransformPoint(v3BackTopLeft);
        v3BackTopRight = transform.TransformPoint(v3BackTopRight);
        v3BackBottomLeft = transform.TransformPoint(v3BackBottomLeft);
        v3BackBottomRight = transform.TransformPoint(v3BackBottomRight);
    }

    void DrawBox()
    {
        //if (Input.GetKey (KeyCode.S)) {
        Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        Vector3 start = mColldier.bounds.min;
        Vector3 end = mColldier.bounds.max;
        Gizmos.DrawLine(start, end);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

            Vector3 start = mColldier.bounds.min;
            Vector3 end = mColldier.bounds.max;
            Gizmos.DrawLine(start, end);

    }

    private void LeaveObject()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            
            string szMsg = string.Format("LeavePOI({0}, '{1}')", m_nID, m_szType);
            //Debug.unityLogger.Log(szMsg);
            //System.Diagnostics.Trace.WriteLine(szMsg);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
            
        }

        m_clickedPOI = null;
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
            string szMsg = string.Format("PickPOI('{0}_{1}',{2},{3})", m_szType, m_nID, vPos.x, vPos.y);
            //Debug.unityLogger.Log(szMsg);
            //System.Diagnostics.Trace.WriteLine(szMsg);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);

            //m_bSelect = !m_bSelect;
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
            if( bSelect == true)
            {
                //Vector3 vPos = Input.mousePosition;
                //string szMsg = string.Format("SelectPOI('{0}_{1}',{2}, {3}, {4})",m_szType, m_nID, vPos.x, vPos.y, bSelect);
                //Debug.logger.Log(szMsg);
                ////System.Diagnostics.Trace.WriteLine(szMsg);
                //if (PassivePipeProxy.Instance != null)
                //    PassivePipeProxy.Instance.SendServer(szMsg);
            }
        }
    }


    private bool m_bEnter = false;
    private void EnterObject()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            Vector3 vPos = Input.mousePosition;
            string szMsg = string.Format("EnterPOI({0},'{1}',{2}, {3})",  m_nID,m_szType, vPos.x, vPos.y);
            //Debug.unityLogger.Log(szMsg);
            //System.Diagnostics.Trace.WriteLine(szMsg);
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
            m_bEnter = true;
            EnterObject();
        }
    }

    void OnMouseExit()
    {
        float x = Input.mousePosition.x;
        float y = (Screen.height - Input.mousePosition.y);
        if ((x > 0 && y > 0) && (x < Screen.width && y < Screen.height))
        {
            m_bEnter = false;
            LeaveObject();
        }
    }

    void OnMouseUp()
    {
        if (m_clickedPOI == this && ModelManager.Instance.Model.EditMode && ModelManager.Instance.Model.EditType == MainModel.EditModeType.DeleteIcon)
        {
            if (POIManager.Instance.RemoveIcon(this))
            {
                Vector3 vPos = mPosition;
                string szMsg = string.Format("OnRemovePOI({0}_{1})", this.m_szType, m_nID);
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg);
            }

            return;
        }

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
            // Finish Drag
            OnFinishDrag();
        }
        m_bDragPOI = false;
    }

    void OnMovePOI()
    {
        /*PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && m_bVisible == true)
        {
            Vector3 vPos = mPosition;
            string szMsg = string.Format("OnDragPOI('{0}_{1}',{2},{3},{4})", this.m_szType, m_nID, vPos.x, vPos.y, vPos.z);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);            
        }*/
    }

    private void OnFinishDrag()
    {
        PythonProxy proxy = PythonProxy.Instance;

        if (proxy != null && m_bVisible == true)
        {
            Vector3 vPos = mPosition;
            string szMsg = string.Format("OnDragPOI('{0}_{1}',{2},{3},{4})", this.m_szType, m_nID, vPos.x, vPos.y, vPos.z);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
        }
    }

    void OnMouseDown()
    {
        m_bDragPOI = false;
        m_clickedPOI = this;
    }

    private bool m_bDragPOI = false;
    void OnMouseDrag()
    {
        if (m_bVisible == true)
        {
            if (ModelManager.Instance.Model.EditMode == true)
            {
                if (ModelManager.Instance.Model.EditType == MainModel.EditModeType.MoveIcon)
                {
                    mPosition = ModelManager.Instance.Model.ScreenToWorldForPOI((int)Input.mousePosition.x, (int)Input.mousePosition.y);
                    m_bDragPOI = true;
                }
            }
        }
    }

    void OnMouseOver()
    {
        if (m_bEnter == true)
            return;

        float x = Input.mousePosition.x;
        float y = (Screen.height - Input.mousePosition.y);
        if ((x > 0 && y > 0) && (x < Screen.width && y < Screen.height))
        {
            m_bEnter = true;
            EnterObject();
        }

    }

    public void SetVisible(bool bVisible)
    {
        m_bVisible = bVisible;
        SpriteRenderer mr2 = gameObject.GetComponent<SpriteRenderer>();
        mr2.enabled = false;
    }	
}
