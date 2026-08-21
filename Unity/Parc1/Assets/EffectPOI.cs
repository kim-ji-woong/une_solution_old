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
        string check = "";
        Vector3 pos = m_vPos;
        if (ModelManager.Instance.Model.CurrentScene.SceneName.Substring(0, 1) == "h")
            pos.y = pos.y - 5.5f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName.Substring(0, 3) == "t01")
            pos.y = pos.y - 8.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName.Substring(0, 3) == "t02")
            pos.y = pos.y - 7.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName.Substring(0, 1) == "r")
            pos.y = pos.y - 6.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName == "b03f")
            pos.y = pos.y - 7.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName == "b04f")
            pos.y = pos.y - 7.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName == "b05f")
            pos.y = pos.y - 8.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName == "b06f")
            pos.y = pos.y - 6.5f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName == "b07f")
            pos.y = pos.y - 7.0f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName == "b07f_up")
            pos.y = pos.y - 8.5f;
        else if (ModelManager.Instance.Model.CurrentScene.SceneName.Substring(0, 1) == "b")
        {
            pos.y = pos.y - 9.5f;
            check = "b";
        }
        

        MainModel.WriteLog("SH : " + check + ", " + ModelManager.Instance.Model.CurrentScene.SceneName);

        this.transform.localPosition = pos; // new Vector3(m_vPos.x, m_vPos.y, m_vPos.z);

        MainModel.WriteLog(ModelManager.Instance.Model.CurrentScene.SceneName + " : " + pos.y);
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

    public static EffectPOI MakeInstance(Vector3 vPos, string strOriginalType, string strType, int nID)
    {
        GameObject obj = null;
        EffectPOI effect = null;

        obj = Instantiate(m_fireEffect);
        obj.transform.SetParent(m_fireEffect.transform.parent);

        effect = obj.GetComponent<EffectPOI>();
        effect.Position = new Vector3(vPos.x, vPos.y, vPos.z);
        effect.ID = nID + POIManager.Instance.nFireEffectID;
        effect.m_strPOIType = strOriginalType + "_Effect";

        obj.name = strOriginalType + "_" + effect.ID;
        obj.SetActive(false);

        return effect;
    }
}
