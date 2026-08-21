using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasPOI : MonoBehaviour
{
    private static GameObject m_cctvRef = null;

    private static CanvasPOI m_selectedPOI = null;

    private Vector3 m_vPos = new Vector3();
    private int m_nID = -1;
    private string m_strPOIType = "";
    private string m_strOriginalPOIType = "";

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

            canvasTransform.anchorMin = ViewportPosition;
            canvasTransform.anchorMax = ViewportPosition;

            Vector2 WorldObject_ScreenPosition = new Vector2(
                                                     ((ViewportPosition.x * canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.5f)),
                                                     ((ViewportPosition.y * canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f)));


            RectTransform transform = GetComponent<RectTransform>();
            transform.anchoredPosition = WorldObject_ScreenPosition;
        }
    }

    public static void InitInstance()
    {
        if (m_cctvRef == null)
        {
            m_cctvRef = GameObject.Find("CCTV_Ref");
            m_cctvRef.SetActive(false);
        }
    }

    public static CanvasPOI MakeInstance(Vector3 vPos, string strOriginalType, string strType, int nID)
    {
        if (strOriginalType == "CCTV")
        {
            GameObject cctv = Instantiate(m_cctvRef);

            CanvasPOI poi = cctv.GetComponent<CanvasPOI>();
            poi.Position = vPos;
            poi.ID = nID;
            poi.m_strPOIType = strType;
            poi.m_strOriginalPOIType = strOriginalType;
            poi.transform.localScale = new Vector3(0.667f, 0.667f, 1.0f);

            if (strOriginalType != strType)
                poi.ChangeImage(strType);

            cctv.transform.SetParent(m_cctvRef.transform.parent);

            cctv.name = strOriginalType + "_" + nID;
            cctv.SetActive(false);
            return poi;
        }

        return null;
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
        PassivePipeProxy proxy = PassivePipeProxy.Instance;
        if (proxy != null)
        {
            Vector3 vPos = Input.mousePosition;
            string szMsg = string.Format("PickPOI('{0}_{1}',{2},{3})", m_strOriginalPOIType, m_nID, vPos.x, vPos.y);
            Debug.Log(szMsg + " : " + Position.x + ", " + Position.y + ", " + Position.z);

            proxy.SendServer(szMsg);
        }
    }

    public void OnMouseEnter(PointerEventData eventData)
    {
        Vector3 vPos = Input.mousePosition;
        string szMsg = string.Format("EnterPOI({0},'{1}',{2}, {3})", m_nID, m_strOriginalPOIType, vPos.x, vPos.y);
        if (PassivePipeProxy.Instance != null)
            PassivePipeProxy.Instance.SendServer(szMsg);
    }

    public void OnMouseExit(PointerEventData eventData)
    {
        Vector3 vPos = Input.mousePosition;
        string szMsg = string.Format("LeavePOI({0},'{1}')", m_nID, m_strOriginalPOIType);
        if (PassivePipeProxy.Instance != null)
            PassivePipeProxy.Instance.SendServer(szMsg);
    }
}
