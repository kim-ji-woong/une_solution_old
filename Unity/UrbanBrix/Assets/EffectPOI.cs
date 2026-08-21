using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPOI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(m_vPos.x, m_fDetectBottom - 0.3f, m_vPos.z);        
        this.transform.localPosition = pos; // new Vector3(m_vPos.x, m_vPos.y, m_vPos.z);
    }

    private static GameObject m_fireEffect = null;

    private int m_nID = -1;
    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    private bool m_bVisible = true;
    public bool bVisible
    {
        get { return m_bVisible; }
        set { m_bVisible = value; }
    }

    private Vector3 m_vPos = new Vector3();
    public Vector3 Position
    {
        get { return m_vPos; }
        set { m_vPos = value; }
    }

    private float m_fDetectBottom = 0.0f;
    public float DetectBottom
    {
        get { return m_fDetectBottom; }
        set { m_fDetectBottom = value; }
    }

    private string m_strPOIType = "";
    public string POIType
    {
        get { return m_strPOIType; }
        set { m_strPOIType = value; }
    }



    public static void InitInstance()
    {
        if (m_fireEffect == null)
        {
            m_fireEffect = GameObject.Find("fire-flame-zone");
            m_fireEffect.SetActive(false);
        }
    }

    public static EffectPOI MakeInstance(Vector3 vPos, float fDetectBottom, string strOriginalType, string strType, int nID)
    {
        GameObject obj = null;
        EffectPOI effect = null;

        obj = Instantiate(m_fireEffect);
        obj.transform.SetParent(m_fireEffect.transform.parent);

        effect = obj.GetComponent<EffectPOI>();
        effect.Position = new Vector3(vPos.x, vPos.y, vPos.z);
        effect.DetectBottom = fDetectBottom;
        effect.ID = nID + POIManager.Instance.nFireEffectID;
        effect.m_strPOIType = strOriginalType + "_Effect";

        obj.name = strOriginalType + "_" + effect.ID;
        obj.SetActive(false);

        return effect;
    }
}
