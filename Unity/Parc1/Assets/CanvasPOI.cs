using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasPOI : MonoBehaviour
{
    private static GameObject m_cctvRef = null;
    private static GameObject m_doorRef = null;
    private static GameObject m_fireRef = null;    
    private static GameObject m_fireWallRef = null;
    private static GameObject m_gasRef = null;

    private static CanvasPOI m_selectedPOI = null;

    private Vector3 m_vPos = new Vector3();
    private int m_nID = -1;
    private string m_strPOIType = "";
    private string m_strOriginalPOIType = "";

    private bool m_bDragPOI = false;
    private static CanvasPOI m_clickedPOI = null;
    
    public Vector3 Position
    {
        get { return m_vPos; }
        set { m_vPos = value; }
    }

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string POIType
    {
        get { return m_strPOIType; }
        set { m_strPOIType = value; }
    }

    public string OriginalPOIType
    {
        get { return m_strOriginalPOIType; }
        set { m_strOriginalPOIType = value; }
    }

    public static CanvasPOI SelectedPOI
    {
        get { return m_selectedPOI; }
        set { m_selectedPOI = value; }
    }

    private bool m_bVisible = true;
    public bool bVisible
    {
        get { return m_bVisible; }
        set { m_bVisible = value; }
    }

    public bool IsAlarmPOI
    {
        get { return m_strPOIType.Contains("AlarmOn"); }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(m_vPos);

        if (ViewportPosition.z >= 0.0f)
        {
            RectTransform canvasTransform = GetComponentInParent<RectTransform>();

            if (canvasTransform != null)
            {
                canvasTransform.anchorMin = ViewportPosition;
                canvasTransform.anchorMax = ViewportPosition;

                Vector2 WorldObject_ScreenPosition = new Vector2(
                                                         ((ViewportPosition.x * canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.5f)),
                                                         ((ViewportPosition.y * canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f)));


                RectTransform transform = GetComponent<RectTransform>();
                if (transform != null)
                    transform.anchoredPosition = WorldObject_ScreenPosition; 
            }
        }
    }

    public static void InitInstance()
    {
        if (m_cctvRef == null)
        {
            m_cctvRef = GameObject.Find("CCTV_Ref");
            m_cctvRef.SetActive(false);
        }

        if (m_doorRef == null)
        {
            m_doorRef = GameObject.Find("Door_Ref");
            m_doorRef.SetActive(false);
        }

        if (m_fireRef == null)
        {
            m_fireRef = GameObject.Find("Fire_Ref");
            m_fireRef.SetActive(false);
        }
        
        if (m_fireWallRef == null)
        {
            m_fireWallRef = GameObject.Find("FireWall_Ref");
            m_fireWallRef.SetActive(false);
        }

        if (m_gasRef == null)
        {
            m_gasRef = GameObject.Find("Gas_Ref");
            m_gasRef.SetActive(false);
        }
    }

    public static CanvasPOI MakeInstance(Vector3 vPos, string strOriginalType, string strType, int nID, ref EffectPOI effectPOI)
    {
        GameObject obj = null;
        CanvasPOI poi = null;

        if (strOriginalType == POIManager.CCTV_TYPE)
        {
            obj = Instantiate(m_cctvRef);
            obj.transform.SetParent(m_cctvRef.transform.parent);
        }
        else if (strOriginalType == POIManager.DOOR_TYPE)
        {
            obj = Instantiate(m_doorRef);
            obj.transform.SetParent(m_doorRef.transform.parent);
        }
        else if (strOriginalType == POIManager.FIRE_TYPE)
        {
            obj = Instantiate(m_fireRef);
            obj.transform.SetParent(m_fireRef.transform.parent);
        }
        else if (strOriginalType == POIManager.FIREWALL_TYPE)
        {
            obj = Instantiate(m_fireWallRef);
            obj.transform.SetParent(m_fireWallRef.transform.parent);
        }
        else if (strOriginalType == POIManager.GAS_TYPE)
        {
            obj = Instantiate(m_gasRef);
            obj.transform.SetParent(m_gasRef.transform.parent);
        }

        if (obj != null)
        {
            poi = obj.GetComponent<CanvasPOI>();
            poi.Position = vPos;
            poi.ID = nID;
            poi.m_strPOIType = strOriginalType;
            poi.m_strOriginalPOIType = strOriginalType;

            if (strOriginalType != strType)
                poi.ChangeImage(strType);

            obj.name = strOriginalType + "_" + nID;
            obj.SetActive(false);

            if (strOriginalType == POIManager.FIRE_TYPE)
            {
                effectPOI = EffectPOI.MakeInstance(vPos, strOriginalType, strOriginalType, nID);
            }
        }

        return poi;
    }

    public void ChangeImage(string strPOIType)
    {
        if (m_strPOIType == strPOIType)
            return;

        if (strPOIType.Length > 0)
        {
            UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();

            if (img != null)
            {
                Sprite sprite = Resources.Load(strPOIType, typeof(Sprite)) as Sprite;

                if (sprite != null)
                {
                    img.sprite = sprite;
                    m_strPOIType = strPOIType;
                }
            }
        }
    }

    public void Pick()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vPos = Input.mousePosition;
            string szMsg = string.Format("PickPOI('{0}_{1}',{2},{3})", m_strOriginalPOIType, m_nID, vPos.x, vPos.y);
            Debug.Log(szMsg + " : " + Position.x + ", " + Position.y + ", " + Position.z);

            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
        }
    }

    public void OnMouseDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            m_bDragPOI = false;
            m_clickedPOI = this;
        }
    }

    public void OnMouseUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_clickedPOI == this && ModelManager.Instance.Model.EditMode && ModelManager.Instance.Model.EditType == MainModel.EditModeType.DeleteIcon)
            {
                if (POIManager.Instance.RemoveIcon(this))
                {
                    Vector3 vPos = m_vPos;
                    string szMsg = string.Format("OnRemovePOI({0}_{1})", m_strOriginalPOIType, m_nID);
                    if (PassivePipeProxy.Instance != null)
                        PassivePipeProxy.Instance.SendServer(szMsg);
                }

                return;
            }

            if (m_bDragPOI == false)
            {
                if ((ModelManager.Instance.Model.EditMode && ModelManager.Instance.Model.EditType == MainModel.EditModeType.PickIcon) || ModelManager.Instance.Model.EditMode == false)
                    Pick();
            }
            else
            {
                if (ModelManager.Instance.Model.EditMode && ModelManager.Instance.Model.EditType == MainModel.EditModeType.MoveIcon)
                {
                    // Finish Drag
                    OnFinishDrag();
                }
            }

            m_bDragPOI = false;
        }
    }

    public void OnMouseDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_clickedPOI == this && ModelManager.Instance.Model.EditMode && ModelManager.Instance.Model.EditType == MainModel.EditModeType.MoveIcon)
            {
                m_bDragPOI = true;
                m_vPos = ModelManager.Instance.Model.ScreenToWorldForPOI((int)eventData.position.x, (int)eventData.position.y);
            }
        }
    }

    private void OnFinishDrag()
    {
        PythonProxy proxy = PythonProxy.Instance;

        if (proxy != null)
        {
            Vector3 vPos = m_vPos;
            string szMsg = string.Format("OnDragPOI('{0}_{1}',{2},{3},{4})", m_strOriginalPOIType, m_nID, vPos.x, vPos.y, vPos.z);
            if (PassivePipeProxy.Instance != null)
                PassivePipeProxy.Instance.SendServer(szMsg);
        }
    }

    public void OnMouseEnter(PointerEventData eventData)
    {
        if (m_strOriginalPOIType != POIManager.CCTV_TYPE)
            return;

        Vector3 vPos = Input.mousePosition;
        string szMsg = string.Format("EnterPOI({0},'{1}',{2}, {3})", m_nID, m_strOriginalPOIType, vPos.x, vPos.y);
        if (PassivePipeProxy.Instance != null)
            PassivePipeProxy.Instance.SendServer(szMsg);
    }

    public void OnMouseExit(PointerEventData eventData)
    {
        if (m_strOriginalPOIType != POIManager.CCTV_TYPE)
            return;

        Vector3 vPos = Input.mousePosition;
        string szMsg = string.Format("LeavePOI({0},'{1}')", m_nID, m_strOriginalPOIType);
        if (PassivePipeProxy.Instance != null)
            PassivePipeProxy.Instance.SendServer(szMsg);
    }
}
