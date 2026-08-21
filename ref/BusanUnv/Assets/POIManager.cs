using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
//using UnityEditorInternal;

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
            proxy.UserObject.SetVariable("AddReverseLODTextPOI", new Action<string, float, float, float>(AddReverseLODTextPOI));
            proxy.UserObject.SetVariable("AddIconPOI", new Action<string, float, float, float>(AddIconPOI));
            
            proxy.UserObject.SetVariable("AddTextPOI2D", new Action<string, int, int>(AddTextPOI2D));
            proxy.UserObject.SetVariable("AddIconPOI2D", new Action<string, int, int>(AddIconPOI2D));

            proxy.UserObject.SetVariable("ShowTextPOI", new Action<int, bool>(ShowTextPOI));
            proxy.UserObject.SetVariable("ShowIconPOI", new Action<int, string, bool>(ShowIconPOI));

            proxy.UserObject.SetVariable("ShowIconLayer", new Action<string, bool>(ShowIconLayer));
            proxy.UserObject.SetVariable("SelectIconPOI", new Action<int, string, bool, bool>(SelectIconPOI));
            proxy.UserObject.SetVariable("RemoveIconPOI", new Action<int, string>(RemoveIconPOI));

            proxy.UserObject.SetVariable("ClearSelectIconPOI", new Action<int, string>(ClearSelectIconPOI));
        }
    }

    private void ClearSelectIconPOI(int nID, string szType)
    {
        Debug.logger.Log("Clear Select All POI ");

        foreach (IconPOI poi in m_arIconArray)
        {
            poi.SelectPOI(false);
        }

        ModelManager.Instance.Model.SaveSharedFile("ClearSelectIconPOI"); 
    }

    private void RemoveIconPOI(int nID, string type)
    {
        IconPOI removePOI = null;
        ArrayList ar = new ArrayList();
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.ID == nID && poi.IconType == type)
            {
                removePOI = poi;
                ar.Add(poi);
            }
        }
        foreach(IconPOI poi in ar)
        {
            poi.gameObject.SetActive(false);
            poi.SetVisible(false);
            m_arIconArray.Remove(poi);

            Destroy(poi.gameObject);
            Destroy(poi);
        }
    }

    private void SelectIconPOI(int nID, string type, bool bSelect, bool bOtherClear)
    {
        if (bOtherClear == true)
        {
            foreach (IconPOI poi in m_arIconArray)
            {
                if (poi.ID == nID && poi.IconType == type)
                {
                    Debug.logger.Log("Select POI Icon : " + nID + " , " + bSelect);
                    poi.SelectPOI(bSelect);                   
                }
                else
                {
                    Debug.logger.Log("Select POI Icon : " + nID + " , " + false);
                    poi.SelectPOI(false);  
                }
            }
        }
        else
        {
            foreach (IconPOI poi in m_arIconArray)
            {
                if (poi.ID == nID && poi.IconType == type)
                {
                    Debug.logger.Log("Icon : " + nID + " , " + bSelect);
                    poi.SelectPOI(bSelect);
                    break;
                }
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
            if (poi.IconName.StartsWith(iconName))
            {
                Debug.logger.Log("Icon : " + poi.ID + " , " + bShow);
                poi.SetVisible(bShow); 
            }
        }
    }

    private void ShowIconPOI(int nID, string type, bool bShow)
    {
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.ID == nID || poi.IconType == type)
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

	void Start ()
    { 	    
	}  
	
	void Update () 
    {  
	}

    public void AddIconPOI2D(string szPath, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddIconPOI(szPath, hit.x, hit.y, hit.z);
    }

    private float m_fExtraHeight = 0.5f;

    public void AddIconPOI(string szPath, float x, float y, float z)
    {
        int nCountID = ModelManager.Instance.GetNextIconCookie();
        string[] paths = szPath.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (paths == null || paths.Length != 3)
        {
            ModelManager.Instance.Model.SaveSharedFile("AddIconPOI", 22, -1, true);
            return;        
        }

        int nID = -1;
        if(int.TryParse(paths[2], out nID))
        {
            if( nID < 0)
            {
                nID = nCountID;
            }

            if( nID > nCountID )
            {
                ModelManager.Instance.SetCookie(nID);
            }

            GameObject obj = new GameObject("ICON_" + nID.ToString());
            obj.layer = 11;

            IconPOI poi = obj.AddComponent<IconPOI>();
            poi.IconName = szPath;
            poi.ID = nID;
            poi.IconType = paths[0];
            poi.Position = new Vector3(x, y, z);
            //poi.Select = true;
            //poi.mColldier = obj.AddComponent<SphereCollider>();

            poi.mColldier = obj.AddComponent<BoxCollider>();

            string szMsg = string.Format("AddIconPOI ID = {0}", nID);
            Debug.logger.Log(szMsg);

            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<UnityEngine.Sprite>(paths[0]);

            spriteRenderer.material = ModelManager.Instance.SpriteDefault;
            spriteRenderer.color = Color.white;

            //int[] nLayers = GetSortingLayerUniqueIDs();
            // if (nLayers != null && nLayers.Length > 1)
            //spriteRenderer.sortingLayerID = 743565889;
            spriteRenderer.sortingLayerName = "POIIcon";
            spriteRenderer.sortingOrder = 0;
            //poi.m_nSoringLayerID = nLayers[1];
            Vector3 vec = spriteRenderer.sprite.bounds.extents;

            obj.transform.position = new Vector3(x, y + m_fExtraHeight, z);

            m_arIconArray.Add(poi);
           
        }
        ModelManager.Instance.Model.SaveSharedFile("AddIconPOI", 22, nID, true);
    }

    //public int[] GetSortingLayerUniqueIDs()
    //{
    //    Type internalEditorUtilityType = typeof(InternalEditorUtility);
    //    PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
    //    return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    //}

    //public string[] GetSortingLayerNames()
    //{
    //    Type internalEditorUtilityType = typeof(InternalEditorUtility);
    //    PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
    //    return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    //}


    public void AddTextPOI2D(string szText, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddTextPOI(szText, hit.x, hit.y, hit.z);
    }

    public void AddTextPOI(string szText, float x, float y,float z)
    {

        Debug.logger.Log("AddTextPOI szText : " + szText + " x : " + x + " y : " + y + " z : " + z);
        GameObject obj = new GameObject(szText);
        obj.layer = 10; //

        TextPOI poi = obj.AddComponent<TextPOI>();
        poi.ID = ModelManager.Instance.GetNextCookie();
        poi.Color = ModelManager.Instance.TextColor;

        TextMesh mNameText = obj.AddComponent<TextMesh>();
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if(render!= null)
        {
            Material mat = ModelManager.Instance.TextMaterial;
            if(mat != null)
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

                ModelManager.Instance.Model.SaveSharedFile("AddTextPOI", 23, poi.ID, true);
            }                    
        }
    }
    public void AddReverseLODTextPOI(string szText, float x, float y, float z)
    {
        GameObject obj = new GameObject(szText);
        obj.layer = 10; //

        TextPOI poi = obj.AddComponent<TextPOI>();
        poi.ReversLOD = true;
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

                ModelManager.Instance.Model.SaveSharedFile("AddReverseLODTextPOI", 24, poi.ID, true);
            }
        }
    }

    private void ShowGroupName(bool bVisible)
    {

    }

    ArrayList m_arGrpNames = new ArrayList();
    public void AddGroupName(string szText, float x, float y, float z)
    {

        Debug.logger.Log("ADDGroupName");

        GameObject obj = new GameObject(szText);
        obj.layer = 9; //

        TextPOI poi = obj.AddComponent<TextPOI>();
        poi.ID = ModelManager.Instance.GetNextCookie();

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
