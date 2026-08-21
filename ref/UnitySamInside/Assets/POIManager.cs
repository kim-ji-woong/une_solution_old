using UnityEngine;
using System;
using System.Collections;

public class POIManager : MonoBehaviour
{


    private static POIManager m_Instance = null;
    public static POIManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private ArrayList m_arIconArray = new ArrayList();
    private ArrayList m_arTextArray = new ArrayList();


    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            proxy.UserObject.SetVariable("AddTextPOI", new Action<string, float, float, float>(AddTextPOI));
            proxy.UserObject.SetVariable("AddIconPOI", new Action<string, float, float, float>(AddIconPOI));

            proxy.UserObject.SetVariable("AddTextPOI2D", new Action<string, int, int>(AddTextPOI2D));
            proxy.UserObject.SetVariable("AddIconPOI2D", new Action<string, int, int>(AddIconPOI2D));

            proxy.UserObject.SetVariable("ShowTextPOI", new Action<int, bool>(ShowTextPOI));
            proxy.UserObject.SetVariable("ShowIconPOI", new Action<int, bool>(ShowIconPOI));


            proxy.UserObject.SetVariable("ShowIconLayer", new Action<string, bool>(ShowIconLayer));

            proxy.UserObject.SetVariable("SelectIconPOI", new Action<int, bool>(SelectIconPOI));

            proxy.UserObject.SetVariable("RemoveIconPOI", new Action<int>(RemoveIconPOI));



        }
    }

    private void RemoveIconPOI(int nID)
    {
        IconPOI removePOI = null;
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.ID == nID)
            {
                removePOI = poi;
                break;
            }
        }
        if (removePOI != null)
        {
            removePOI.gameObject.SetActive(false);
            removePOI.SetVisible(false);
            m_arIconArray.Remove(removePOI);

            Destroy(removePOI.gameObject);
            Destroy(removePOI);
        }
    }

    private void SelectIconPOI(int nID, bool bSelect)
    {
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.ID == nID)
            {
                Debug.logger.Log("Icon : " + nID + " , " + bSelect);
                poi.SelectPOI(bSelect);
                break;
            }
        }
    }

    private void ShowTextPOI(int nID, bool bShow)
    {

        foreach (TextPOI poi in m_arTextArray)
        {
            if (poi.ID == nID)
            {

                poi.SetVisible(bShow);
                break;
            }
        }
    }

    private void ShowIconLayer(string iconName, bool bShow)
    {
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.IconName == iconName)
            {
                Debug.logger.Log("Icon : " + poi.ID + " , " + bShow);
                poi.SetVisible(bShow);
            }
        }
    }

    private void ShowIconPOI(int nID, bool bShow)
    {
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.ID == nID)
            {
                Debug.logger.Log("Icon : " + nID + " , " + bShow);
                poi.SetVisible(bShow);
                break;
            }
        }

    }

    void Awake()
    {
        m_Instance = this;
        AddPythonFunction();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void AddIconPOI2D(string szPath, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddIconPOI(szPath, hit.x, hit.y, hit.z);
    }

    private float m_fExtraHeight = 7.0f;

    public void AddIconPOI(string szPath, float x, float y, float z)
    {

        Transform trans = ModelManager.Instance.CurrentTransform;
        if (trans == null)
            return;

        int nID = ModelManager.Instance.GetNextIconCookie();
        GameObject obj = new GameObject("Icon_POI_" + nID);
        obj.transform.parent = trans;        

        string[] paths = szPath.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);


        obj.layer = 11;

        IconPOI poi = obj.AddComponent<IconPOI>();
        poi.IconName = szPath;
        poi.ID = nID;
        poi.Position = new Vector3(x, y, z);
        poi.mColldier = obj.AddComponent<BoxCollider2D>();

        string szMsg = string.Format("AddIconPOI ID = {0}", nID);
        Debug.logger.Log(szMsg);

        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<UnityEngine.Sprite>(paths[0]);
        Vector3 vec = spriteRenderer.sprite.bounds.extents;

        obj.transform.position = new Vector3(x, y + m_fExtraHeight, z);

        m_arIconArray.Add(poi);

        ModelManager.Instance.Model.SaveSharedFile(22, poi.ID, true);
    }

    public void AddTextPOI2D(string szText, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddTextPOI(szText, hit.x, hit.y, hit.z);
    }

    public void AddTextPOI(string szText, float x, float y, float z)
    {
        GameObject obj = new GameObject(szText);
        obj.layer = 10; //

        TextPOI poi = obj.AddComponent<TextPOI>();
        poi.ID = ModelManager.Instance.GetNextCookie();
        poi.Color = ModelManager.Instance.TextColor;

        TextMesh mNameText = obj.AddComponent<TextMesh>();
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render != null)
        {
            Material mat = ModelManager.Instance.TextMaterial;
            if (mat != null)
            {
                mat.color = ModelManager.Instance.TextColor;
                render.material = mat;
                render.materials[0] = mat;

                mNameText.font = ModelManager.Instance.m_TextFont;
                mNameText.anchor = TextAnchor.LowerCenter;
                mNameText.characterSize = 1f;
                mNameText.fontStyle = FontStyle.Normal;
                mNameText.color = ModelManager.Instance.TextColor;
                mNameText.text = szText;
                mNameText.transform.position = new Vector3(x, y, z);
                Vector3 dir = Camera.main.transform.position - mNameText.transform.position;
                mNameText.transform.LookAt(mNameText.transform.position - dir);
                render.enabled = true;

                m_arTextArray.Add(poi);

                ModelManager.Instance.Model.SaveSharedFile(23, poi.ID, true);
            }
        }
    }


    private void ShowGroupName(bool bVisible)
    {

    }

    ArrayList m_arGrpNames = new ArrayList();
    public void AddGroupName(string szText, float x, float y, float z)
    {

        Transform trans = ModelManager.Instance.CurrentTransform;
        if (trans == null)
            return;

        int nID = ModelManager.Instance.GetNextCookie();
        GameObject obj = new GameObject("TextPOI_" + nID);
        obj.transform.parent = trans;
        obj.layer = 9; //

        TextPOI poi = obj.AddComponent<TextPOI>();
        poi.ID = nID;

        TextMesh mNameText = obj.AddComponent<TextMesh>();
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render != null)
        {
            Material mat = ModelManager.Instance.TextMaterial;
            if (mat != null)
            {
                mat.color = ModelManager.Instance.GroupNameColor;
                render.material = mat;
                render.materials[0] = mat;

                mNameText.font = ModelManager.Instance.m_TextFont;
                mNameText.anchor = TextAnchor.LowerCenter;
                mNameText.characterSize = 1f;
                mNameText.fontStyle = FontStyle.Normal;
                mNameText.color = ModelManager.Instance.GroupNameColor;
                mNameText.text = szText;
                mNameText.transform.position = new Vector3(x, y, z);
                Vector3 dir = Camera.main.transform.position - mNameText.transform.position;
                mNameText.transform.LookAt(mNameText.transform.position - dir);
                render.enabled = true;

                m_arGrpNames.Add(poi);
            }
        }
    }
}
