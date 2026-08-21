using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;
using DBUtility2;
using System.IO;
//using UnityEditorInternal;

public class POIManager : MonoBehaviour
{
    private const string CCTV_TYPE = "CCTV";
    private const string CCTV_SELECTED_TYPE = "CCTVSelected";
    private const string SELECTED_TYPE = "Selected";

    // First Key : POIType
    // Second Key : POI ID
    private Dictionary<string, Dictionary<int, CanvasPOI>> m_dicPOIs = new Dictionary<string, Dictionary<int, CanvasPOI>>();
    
    private static POIManager m_Instance = null;
    public static POIManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    //private ArrayList m_arIconArray = new ArrayList();
    private ArrayList m_arTextArray = new ArrayList();


    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && proxy.UserObject != null)
        {
            proxy.UserObject.SetVariable("AddTextPOI", new Action<string, float, float, float>(AddTextPOI));
            proxy.UserObject.SetVariable("AddReverseLODTextPOI", new Action<string, float, float, float>(AddReverseLODTextPOI));
            proxy.UserObject.SetVariable("AddIconPOI", new Action<string, float, float, float>(AddIconPOI));
            proxy.UserObject.SetVariable("AddIconPOIFile", new Action<string, string>(AddIconPOIFile));
            proxy.UserObject.SetVariable("ClearIconPOI", new Action<string>(ClearIconPOI));

            proxy.UserObject.SetVariable("AddTextPOI2D", new Action<string, int, int>(AddTextPOI2D));
            proxy.UserObject.SetVariable("AddIconPOI2D", new Action<string, int, int>(AddIconPOI2D));

            proxy.UserObject.SetVariable("ShowTextPOI", new Action<int, bool>(ShowTextPOI));
            proxy.UserObject.SetVariable("ShowIconPOI", new Action<int, string, bool>(ShowIconPOI));
            proxy.UserObject.SetVariable("ShowIconPOIFile", new Action<string>(ShowIconPOIFile));

            proxy.UserObject.SetVariable("ChangePOIIcon", new Action<string, string>(ChangePOIIcon));
            proxy.UserObject.SetVariable("ChangePOIIconFile", new Action<string>(ChangePOIIconFile));
            proxy.UserObject.SetVariable("RollBackPOIIcon", new Action<string>(RollBackPOIIcon));
            proxy.UserObject.SetVariable("SelectPOI", new Action<string, int, bool>(SelectPOI));

            proxy.UserObject.SetVariable("ShowIconLayer", new Action<string, bool>(ShowIconLayer));
            proxy.UserObject.SetVariable("SelectIconPOI", new Action<int, string, bool, bool>(SelectIconPOI));
            proxy.UserObject.SetVariable("RemoveIconPOI", new Action<int, string>(RemoveIconPOI));

            proxy.UserObject.SetVariable("ClearSelectIconPOI", new Action<int, string>(ClearSelectIconPOI));
        }
    }

    private void ClearSelectIconPOI(int nID, string szType)
    {
    }

    private void RemoveIconPOI(int nID, string type)
    {
        Dictionary<int, CanvasPOI> dicPOIs;
        CanvasPOI poi = GetPOI2(type, nID, out dicPOIs);

        if (poi != null)
        {
            RemovePOI(poi, dicPOIs);
        }
    }

    private void RemovePOI(CanvasPOI poi, Dictionary<int, CanvasPOI> dicPOIs)
    {
        poi.gameObject.SetActive(false);
        dicPOIs.Remove(poi.ID);

        Destroy(poi.gameObject);
        Destroy(poi);
    }

    private void SelectIconPOI(int nID, string type, bool bSelect, bool bOtherClear)
    {
        if (bOtherClear)
            RollBackPOIIcon(type);

        SelectPOI(type, nID, bSelect);
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
        MainModel.WriteLog("ShowIconLayer : " + iconName + ", " + bShow);
        string strLayerType = GetLayerType(iconName);

        Dictionary<int, CanvasPOI> dicPOIs;

        if (m_dicPOIs.TryGetValue(strLayerType, out dicPOIs))
        {
            foreach (KeyValuePair<int, CanvasPOI> pair in dicPOIs)
            {
                pair.Value.gameObject.SetActive(bShow);
            }
        }
    }

    private string GetLayerType(string iconName)
    {
        string[] tokens = iconName.Split('_');
        return tokens[0].Trim();
    }

    private void ShowIconPOI(int nID, string type, bool bShow)
    {
        CanvasPOI poi = GetPOI(type, nID);

        if (poi != null)
            poi.gameObject.SetActive(bShow);
    }

    void Awake()
    {
        m_Instance = this;
        AddPythonFunction();

        CanvasPOI.InitInstance();
        //m_canvasPOI = CanvasPOI.MakeInstance(new Vector3(-830, -20, -1108), "CCTV");
    }

	void Start ()
    {
    }  
	
	void Update () 
    {  
	}

    public void AddIconPOI2D(string szPath, int x, int y)
    {
    }

    public void AddIconPOI(string szPath, float x, float y, float z)
    {
    }

    public void AddTextPOI2D(string szText, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddTextPOI(szText, hit.x, hit.y, hit.z);
    }

    public void AddTextPOI(string szText, float x, float y,float z)
    {       
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

    public void AddIconPOIFile(string strPOIType, string strFilePath)
    {
        try
        {
            StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

            string strIDFilePath = strFilePath + ".id";
            StreamWriter writer = new StreamWriter(strIDFilePath, false, System.Text.Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex1 = strLine.IndexOf(',');
                int nIndex2 = strLine.IndexOf(',', nIndex1 + 1);
                int nIndex3 = strLine.IndexOf(',', nIndex2 + 1);

                if (nIndex1 > 0 && nIndex2 > nIndex1 && nIndex3 > nIndex2)
                {
                    string strX = strLine.Substring(0, nIndex1).Trim();
                    string strY = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                    string strZ = strLine.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1).Trim();
                    string strIconName = strLine.Substring(nIndex3 + 1).Trim();

                    float x, y, z;

                    if (float.TryParse(strX, out x) && float.TryParse(strY, out y) && float.TryParse(strZ, out z))
                    {
                        int nID = AddIconPOI2(strIconName, x, y, z);
                        writer.WriteLine(strIconName + "," + nID.ToString());
                    }
                }
            }

            writer.Close();
            reader.Close();

            File.Delete(strFilePath);
            OnAddPOIFile(strPOIType, strIDFilePath);
        }
        catch (Exception e)
        {
            MainModel.WriteLog(e.Message);
        }
    }

    private void OnAddPOIFile(string strPOIType, string strIDFilePath)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = string.Format("SendMessage('AddIconPOIFileFinish({0}, {1})')", strPOIType, strIDFilePath);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void AddPOI(CanvasPOI poi, string strPOIType)
    {
        Dictionary<int, CanvasPOI> pois;

        if (m_dicPOIs.TryGetValue(strPOIType, out pois) == false)
        {
            pois = new Dictionary<int, CanvasPOI>();
            m_dicPOIs[strPOIType] = pois;
        }

        pois[poi.ID] = poi;
    }

    private CanvasPOI GetPOI(string strPOIType, int nID)
    {
        Dictionary<int, CanvasPOI> pois;
        return GetPOI2(strPOIType, nID, out pois);
    }

    private CanvasPOI GetPOI2(string strPOIType, int nID, out Dictionary<int, CanvasPOI> pois)
    {
        CanvasPOI poi = null;

        if (m_dicPOIs.TryGetValue(strPOIType, out pois))
        {
            pois.TryGetValue(nID, out poi);
        }

        return poi;
    }

    private int AddIconPOI2(string szPath, float x, float y, float z)
    {
        MainModel.WriteLog("AddIconPOI2 : " + szPath + ", " + x + ", " + y + ", " + z);
        int nCountID = ModelManager.Instance.GetNextIconCookie();
        string[] paths = szPath.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (paths == null || paths.Length != 3)
        {
            return -1;
        }

        int nID = -1;
        if (int.TryParse(paths[2], out nID))
        {
            if (nID < 0)
            {
                nID = nCountID;
            }
        }

        string strPOIType = paths[0].Trim();
        string strOriginalPOIType = strPOIType;
        string strSelected = "Selected";
        bool isSelected = strPOIType.Contains(strSelected);

        if (isSelected)
            strPOIType = strPOIType.Replace(strSelected, "");

        CanvasPOI poi = CanvasPOI.MakeInstance(new Vector3(x, y, z), strOriginalPOIType, strPOIType, nID);

        if (poi == null)
            return -1;

        AddPOI(poi, strOriginalPOIType);

        return nID;
    }

    public void ClearIconPOI(string strPOIType)
    {
        MainModel.WriteLog("ClearIconPOI : " + strPOIType);

        if (strPOIType.Length == 0)
        {
            foreach (KeyValuePair<string, Dictionary<int, CanvasPOI>> pair in m_dicPOIs)
            {
                ClearIconPOI(pair.Value);
            }
        }
        else
        {
            Dictionary<int, CanvasPOI> dicPOIs;

            if (m_dicPOIs.TryGetValue(strPOIType, out dicPOIs))
                ClearIconPOI(dicPOIs);
        }
    }

    private void ClearIconPOI(Dictionary<int, CanvasPOI> dicPOIs)
    {
        while (dicPOIs.Count > 0)
        {
            CanvasPOI poi = null;

            foreach (KeyValuePair<int, CanvasPOI> _pair in dicPOIs)
            {
                poi = _pair.Value;
                break;
            }

            if (poi != null)
            {
                int nID = poi.ID;
                RemovePOI(poi, dicPOIs);

                if (dicPOIs.ContainsKey(nID))
                    break;
            }
            else
                break;
        }
    }

    public void ChangePOIIcon(string strPOIInfo, string strPOIType)
    {
        string[] tokens = strPOIInfo.Split('_');

        if (tokens.Length != 2)
            return;

        string strOriginalPOIType = tokens[0];
        int nID;

        if (int.TryParse(tokens[1], out nID) == false)
            return;

        CanvasPOI poi = GetPOI(strOriginalPOIType, nID);

        if (poi != null)
        {
            poi.ChangeImage(strPOIType);
        }
    }

    public void ChangePOIIconFile(string strFilePath)
    {
        StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

        while (reader.EndOfStream == false)
        {
            string strLine = reader.ReadLine().Trim();

            if (strLine.Length == 0)
                continue;

            int nIndex = strLine.LastIndexOf(',');

            if (nIndex < 0)
                continue;

            string strPOIInfo = strLine.Substring(0, nIndex).Trim();
            string strPOIType = strLine.Substring(nIndex + 1).Trim();
            ChangePOIIcon(strPOIInfo, strPOIType);
        }

        reader.Close();
        File.Delete(strFilePath);
    }

    public void RollBackPOIIcon(string strPOIType)
    {
        if (strPOIType.Length == 0)
        {
            foreach (KeyValuePair<string, Dictionary<int, CanvasPOI>> pair in m_dicPOIs)
            {
                RollBackPOIIcon(pair.Value);
            }
        }
        else
        {
            Dictionary<int, CanvasPOI> dicPOIs;

            if (m_dicPOIs.TryGetValue(strPOIType, out dicPOIs))
                RollBackPOIIcon(dicPOIs);
        }
    }

    public void SelectPOI(string strPOIType, int nID, bool selected)
    {
        CanvasPOI poi = GetPOI(strPOIType, nID);

        if (poi != null)
        {
            if (selected)
                strPOIType += SELECTED_TYPE;

            poi.ChangeImage(strPOIType);

            if (selected)
            {
                CanvasPOI.SelectedPOI = poi;
                MainModel.SetDragMode(true);
            }
            else
            {
                MainModel.OnPOIMoved(poi, true);
            }
        }
    }

    private void RollBackPOIIcon(Dictionary<int, CanvasPOI> dicPOIs/*Dictionary<int, GameObject> dicPOIGroup, GameObject targetPOI*/)
    {
        foreach (KeyValuePair<int, CanvasPOI> pair in dicPOIs)
        {
            pair.Value.ChangeImage(pair.Value.OriginalPOIType);
        }
    }

    public void ShowIconPOIFile(string strFilePath)
    {
        MainModel.WriteLog("ShowIconPOIFile : " + strFilePath);
        StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

        string strSelected = "Selected";

        while (reader.EndOfStream == false)
        {
            string strLine = reader.ReadLine().Trim();

            if (strLine.Length == 0)
                continue;

            MainModel.WriteLog("ReadLine : " + strLine);
            int nIndex1 = strLine.IndexOf(',');
            int nIndex2 = strLine.IndexOf(',', nIndex1 + 1);

            if (nIndex1 > 0 && nIndex2 > nIndex1)
            {
                string strID = strLine.Substring(0, nIndex1).Trim();
                string strVisible = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                string strType = strLine.Substring(nIndex2 + 1).Trim();
                string strOriginalType = strType;

                bool isSelected = strType.Contains(strSelected);

                if (isSelected)
                    strType = strType.Replace(strSelected, "");

                int nID;
                bool visible;

                if (int.TryParse(strID, out nID) && StringToBoolean(strVisible, out visible))
                {
                    CanvasPOI poi = GetPOI(strOriginalType, nID);

                    if (poi != null)
                    {
                        poi.gameObject.SetActive(visible);
                    }
                }
            }
        }

        reader.Close();
        File.Delete(strFilePath);
    }

    private bool StringToBoolean(string str, out bool value)
    {
        value = false;

        string strLower = str.ToLower();

        if (strLower == "true" || strLower == "1")
        {
            value = true;
            return true;
        }
        else if (strLower == "false" || strLower == "0")
        {
            value = false;
            return true;
        }

        return false;
    }
}
