using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
//using UnityEditorInternal;
using DBUtility2;
using Assets;

public class POIManager : MonoBehaviour
{
    public const string CCTV_TYPE = "CCTV";
    private const string CCTV1_TYPE = "CCTV_1";
    private const string CCTV2_TYPE = "CCTV_2";
    private const string CCTV3_TYPE = "CCTV_3";
    private const string CCTV4_TYPE = "CCTV_4";
    public const string DOOR_TYPE = "Door";
    private const string DOOR_ALARM_ON_TYPE = "DoorAlarmOn";
    public const string FIRE_TYPE = "Fire";
    private const string FIRE_ALARM_ON_TYPE = "FireAlarmOn";
    public const string FIRE_ALARM_ON_TYPE_EFFECT = "Fire_Effect";
    public const string FIREWALL_TYPE = "FireWall";
    private const string FIREWALL_ALARM_ON_TYPE = "FireWallAlarmOn";
    public const string GAS_TYPE = "Gas";
    private const string GAS_ALARM_ON_TYPE = "GasAlarmOn";

    public int nFireEffectID = 1000000;

    // First Key : POIType
    // Second Key : POI ID
    private Dictionary<string, Dictionary<int, CanvasPOI>> m_dicPOIs = new Dictionary<string, Dictionary<int, CanvasPOI>>();
    private Dictionary<string, Dictionary<int, EffectPOI>> m_dicEffectPOIs = new Dictionary<string, Dictionary<int, EffectPOI>>();

    // Key : Layer Type 이름
    // Value : Visible
    private Dictionary<string, bool> m_dicLayers = new Dictionary<string, bool>();

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
        if (proxy != null && proxy.UserObject != null)
        {
            proxy.UserObject.SetVariable("AddTextPOI", new Action<string, float, float, float>(AddTextPOI));
            proxy.UserObject.SetVariable("AddReverseLODTextPOI", new Action<string, float, float, float>(AddReverseLODTextPOI));
            proxy.UserObject.SetVariable("AddReverseLODTextPOIFile", new Action<string>(AddReverseLODTextPOIFile));
            proxy.UserObject.SetVariable("AddIconPOI", new Action<string, float, float, float>(AddIconPOI));
            proxy.UserObject.SetVariable("AddIconPOIFile", new Action<string, string>(AddIconPOIFile));
            proxy.UserObject.SetVariable("ClearIconPOI", new Action<string>(ClearIconPOI));

            proxy.UserObject.SetVariable("AddTextPOI2D", new Action<string, int, int>(AddTextPOI2D));
            proxy.UserObject.SetVariable("AddIconPOI2D", new Action<string, int, int>(AddIconPOI2D));

            proxy.UserObject.SetVariable("ShowTextPOI", new Action<int, bool>(ShowTextPOI));
            proxy.UserObject.SetVariable("ShowBuildingText", new Action<bool>(ShowBuildingText));
            proxy.UserObject.SetVariable("ShowIconPOI", new Action<int, string, bool>(ShowIconPOI));
            proxy.UserObject.SetVariable("ShowIconPOIFile", new Action<string>(ShowIconPOIFile));

            proxy.UserObject.SetVariable("ChangePOIIcon", new Action<string, string>(ChangePOIIcon));
            proxy.UserObject.SetVariable("ChangePOIIconFile", new Action<string>(ChangePOIIconFile));
            proxy.UserObject.SetVariable("RollBackPOIIcon", new Action<string>(RollBackPOIIcon));

            proxy.UserObject.SetVariable("ShowIconLayer", new Action<string, bool>(ShowIconLayer));
            proxy.UserObject.SetVariable("ShowIconLayers", new Action<string, bool>(ShowIconLayers));
            proxy.UserObject.SetVariable("HideIconLayers", new Action<string, bool>(HideIconLayers));
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

    private void RemoveEffectPOI(EffectPOI poi, Dictionary<int, EffectPOI> dicEffectPOIs)
    {
        poi.gameObject.SetActive(false);
        dicEffectPOIs.Remove(poi.ID);

        Destroy(poi.gameObject);
        Destroy(poi);
    }

    private void SelectIconPOI(int nID, string type, bool bSelect, bool bOtherClear)
    {
    }

    public void ShowTextPOI(int nID, bool bShow)
    {       

        foreach (TextPOI poi in m_arTextArray)
        {
            if (poi.ID == nID)
            {
                Debug.Log("ShowTextPOI : " + poi.ID);
                poi.SetVisible(bShow);
                break;
            }
        }

        Debug.Log("ShowTextPOI Fail");
    }

    private List<string> GetAllTypes()
    {
        List<string> types = new List<string>();

        types.Add(CCTV_TYPE);
        types.Add(DOOR_TYPE);
        types.Add(FIRE_TYPE);
        types.Add(FIREWALL_TYPE);
        types.Add(GAS_TYPE);

        return types;
    }

    public void ShowIconLayer(string iconTypeName, bool bShow)
    {
        Dictionary<int, CanvasPOI> dicPOIs = null;
        m_dicLayers[iconTypeName] = bShow;

        // 편집모드에서는 CCTV만 보이도록 한다.
        if (bShow && ModelManager.Instance.Model.EditMode && iconTypeName != CCTV_TYPE)
            return;
        
        if (m_dicPOIs.TryGetValue(iconTypeName, out dicPOIs))
        {
            foreach (KeyValuePair<int, CanvasPOI> pair in dicPOIs)
            {
                pair.Value.bVisible = bShow;
                pair.Value.gameObject.SetActive(bShow);
                //MainModel.WriteLog("SH fire visible : " + pair.Value.ID + " / " + bShow);
            }

            if (iconTypeName == FIRE_TYPE/*FIRE_ALARM_ON_TYPE*/)
            {
                Dictionary<int, EffectPOI> dicEffectPOIs = null;
                if (m_dicEffectPOIs.TryGetValue(FIRE_ALARM_ON_TYPE_EFFECT, out dicEffectPOIs))
                {
                    foreach (KeyValuePair<int, EffectPOI> pair in dicEffectPOIs)
                    {
                        //MainModel.WriteLog("SH fireEffect visible : " + pair.Value.ID + " / " + bShow);
                        pair.Value.bVisible = bShow;
                        pair.Value.gameObject.SetActive(bShow);
                    }
                }
            }
        }
        /*Dictionary<int, int> dicIndex;
        List<int> indexes;
        GameObject obj;

        Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(iconTypeName, out dicIndex, out indexes);

        if (dicPOIGroup != null)
        {
            m_dicLayers[iconTypeName] = bShow;

            foreach (KeyValuePair<int, int> pair in dicIndex)
            {
                if (dicPOIGroup.TryGetValue(pair.Value, out obj))
                {
                    obj.SetActive(bShow);

                    if (bShow)
                        Debug.Log("ShowIconLayer visible");
                }
            }
        }*/
    }

    public void ShowIconLayers(string iconTypeNames, bool hideOthers)
    {
        Dictionary<string, string> dicIconNames = ParseIconNames(iconTypeNames);
        List<string> allPOITypes = GetAllTypes();

        foreach (KeyValuePair<string, string> pair in dicIconNames)
        {
            ShowIconLayer(pair.Value, true);
            allPOITypes.Remove(pair.Value);

            /*Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(pair.Value, out dicIndex, out indexes);

            if (dicPOIGroup != null)
            {
                m_dicLayers[pair.Value] = true;

                foreach (KeyValuePair<int, int> pair2 in dicIndex)
                {
                    if (dicPOIGroup.TryGetValue(pair2.Value, out obj))
                    {
                        obj.SetActive(true);
                        Debug.Log("ShowIconLayers :" + iconTypeNames);
                    }
                }
                
                allPOITypes.Remove(pair.Value);
            }*/
        }

        if (hideOthers)
        {
            foreach (string strPOIType in allPOITypes)
            {
                ShowIconLayer(strPOIType, false);
                /*Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strPOIType, out dicIndex, out indexes);

                if (dicPOIGroup != null)
                {
                    m_dicLayers[strPOIType] = false;

                    foreach (KeyValuePair<int, int> pair in dicIndex)
                    {
                        if (dicPOIGroup.TryGetValue(pair.Value, out obj))
                        {
                            obj.SetActive(false);
                        }
                    }
                }*/
            }
        }
    }

    private void HideIconLayers(string iconTypeNames, bool showOthers)
    {
        Dictionary<string, string> dicIconNames = ParseIconNames(iconTypeNames);

        List<string> allPOITypes = GetAllTypes();

        foreach (KeyValuePair<string, string> pair in dicIconNames)
        {
            ShowIconLayer(pair.Value, false);
            allPOITypes.Remove(pair.Value);
            /*Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(pair.Value, out dicIndex, out indexes);

            if (dicPOIGroup != null)
            {
                m_dicLayers[pair.Value] = false;

                foreach (KeyValuePair<int, int> pair2 in dicIndex)
                {
                    if (dicPOIGroup.TryGetValue(pair2.Value, out obj))
                    {
                        obj.SetActive(false);
                    }
                }
                
                allPOITypes.Remove(pair.Value);
            }*/
        }

        if (showOthers)
        {
            foreach (string strPOIType in allPOITypes)
            {
                ShowIconLayer(strPOIType, true);
                /*Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strPOIType, out dicIndex, out indexes);
                
                if (dicPOIGroup != null)
                {
                    m_dicLayers[strPOIType] = true;

                    foreach (KeyValuePair<int, int> pair in dicIndex)
                    {
                        if (dicPOIGroup.TryGetValue(pair.Value, out obj))
                        {
                            obj.SetActive(true);
                        }
                    }
                }*/
            }
        }
    }

    private Dictionary<string, string> ParseIconNames(string iconNames)
    {
        string[] tokens = iconNames.Split('_');
        Dictionary<string, string> dicIconNames = new Dictionary<string, string>();

        foreach (string strToken in tokens)
        {
            string strIconName = strToken.Trim();

            if (strIconName.Length == 0)
                continue;

            dicIconNames[strIconName] = strIconName;
        }

        return dicIconNames;
    }

    public void ShowIconPOI(int nID, string type, bool bShow)
    {
        CanvasPOI poi = GetPOI(type, nID);

        if (poi != null)
            poi.gameObject.SetActive(bShow);
        
        ModelManager.Instance.Model.InitPOILod(type);
    }

    public void ShowIconPOIFile(string strFilePath)
    {
        StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

        string strAlarmOn = "AlarmOn";
        string strPrevPOIType = "";
        bool visibleLayer = false;

        Dictionary<string, string> dicPOITypes = new Dictionary<string, string>();

        while (reader.EndOfStream == false)
        {
            string strLine = reader.ReadLine().Trim();

            if (strLine.Length == 0)
                continue;
            
            int nIndex1 = strLine.IndexOf(',');
            int nIndex2 = strLine.IndexOf(',', nIndex1 + 1);

            if (nIndex1 > 0 && nIndex2 > nIndex1)
            {
                string strID = strLine.Substring(0, nIndex1).Trim();
                string strVisible = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                string strType = strLine.Substring(nIndex2 + 1).Trim();
                string strOriginalType = strType;

                dicPOITypes[strOriginalType] = strOriginalType;

                bool isAlarmPOI = strType.Contains(strAlarmOn);

                if (isAlarmPOI)
                    strOriginalType = strType.Replace(strAlarmOn, "");

                int nID;
                bool visible;

                if (strPrevPOIType != strOriginalType)
                {
                    visibleLayer = IsVisible(strOriginalType);
                    strPrevPOIType = strOriginalType;
                }

                if (int.TryParse(strID, out nID) && StringToBoolean(strVisible, out visible))
                {
                    CanvasPOI poi = GetPOI(strOriginalType, nID);

                    if (poi != null)
                    {
                        poi.bVisible = visible;

                        poi.gameObject.SetActive(visible && visibleLayer);

                        if (strOriginalType == FIRE_TYPE && isAlarmPOI)
                        {
                            EffectPOI effectPOI = GetEffectPOI(FIRE_ALARM_ON_TYPE_EFFECT, nID + nFireEffectID);
                            if (effectPOI != null)
                            {
                                effectPOI.bVisible = visible;
                                effectPOI.gameObject.SetActive(visible && visibleLayer);
                            }
                        }
                    }
                }
            }
        }

        reader.Close();
        File.Delete(strFilePath);

        foreach (KeyValuePair<string, string> pair in dicPOITypes)
        {
            ModelManager.Instance.Model.InitPOILod(pair.Key);
        }

        ModelManager.Instance.Model.OnApplicationFocus(true);
    }

    private bool IsVisible(string strLayerName)
    {
        bool visible = false;

        if (m_dicLayers.TryGetValue(strLayerName, out visible))
        {
            if (visible == false)
            {
                return visible;
            }
            else
            {
                // 편집모드에서는 CCTV Icon만 보이도록 한다.
                if (ModelManager.Instance.Model.EditMode)
                {
                    if (strLayerName == CCTV_TYPE)
                        return true;
                    else
                        return false;
                }
                else
                    return visible;
            }
        }

        return false;
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

    void Awake()
    {
        //MainModel.WriteLog("POIManager.Awake");
        Debug.Log("POIManager.Awake");
        m_Instance = this;
        AddPythonFunction();

        CanvasPOI.InitInstance();
        EffectPOI.InitInstance();
        //CanvasPOI poi = CanvasPOI.MakeInstance(new Vector3(0.0f, 5.5f, 0.0f), "CCTV", "CCTV", 10);
        //poi.gameObject.SetActive(true);
    }

    void Start ()
    {
    }  
	
	void Update () 
    {
    }

    public bool RemoveIcon(CanvasPOI poi)
    {
        Dictionary<int, CanvasPOI> dicPOIs;
        
        if (m_dicPOIs.TryGetValue(poi.OriginalPOIType, out dicPOIs))
        {
            RemovePOI(poi, dicPOIs);
        }
    
        return true;
    }

    public void AddIconPOI2D(string szPath, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddIconPOI(szPath, hit.x, hit.y, hit.z);
    }

    public void AddIconPOI(string szPath, float x, float y, float z)
    {
        AddIconPOI2(szPath, x, y, z);
    }

    private int AddIconPOI2(string szPath, float x, float y, float z)
    {
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

        string strPOIType = paths[0];
        string strOriginalPOIType = strPOIType;
        string strAlarmOn = "AlarmOn";
        bool isAlarmPOI = strPOIType.Contains(strAlarmOn);

        if (isAlarmPOI)
            strOriginalPOIType = strPOIType.Replace(strAlarmOn, "");

        EffectPOI effect = null;
        CanvasPOI poi = CanvasPOI.MakeInstance(new Vector3(x, y, z), ModelManager.Instance.Model.CurrentScene.DBottomHeight, strOriginalPOIType, strPOIType, nID, ref effect);
        
        if (poi == null)
            return -1;
        
        AddPOI(poi, strOriginalPOIType);

        if (effect != null && isAlarmPOI)
        {
            AddEffectPOI(effect, FIRE_ALARM_ON_TYPE_EFFECT);
        }

        return nID;
    }

    public void AddPOI(CanvasPOI poi, string strPOIType)
    {
        Dictionary<int, CanvasPOI> pois;

        if (m_dicPOIs.TryGetValue(strPOIType, out pois) == false)
        {
            pois = new Dictionary<int, CanvasPOI>();
            m_dicPOIs[strPOIType] = pois;
        }

        pois[poi.ID] = poi;
    }

    public void AddEffectPOI(EffectPOI effect, string strPOIType)
    {
        Dictionary<int, EffectPOI> effects;

        if (m_dicEffectPOIs.TryGetValue(strPOIType, out effects) == false)
        {
            effects = new Dictionary<int, EffectPOI>();
            m_dicEffectPOIs[strPOIType] = effects;
        }

        effects[effect.ID] = effect;
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

    private EffectPOI GetEffectPOI(string strPOIType, int nID)
    {
        Dictionary<int, EffectPOI> pois;
        return GetEffectPOI2(strPOIType, nID, out pois);
    }

    private EffectPOI GetEffectPOI2(string strPOIType, int nID, out Dictionary<int, EffectPOI> pois)
    {
        EffectPOI poi = null;
        
        if (m_dicEffectPOIs.TryGetValue(strPOIType, out pois))
        {
            pois.TryGetValue(nID, out poi);
        }

        return poi;
    }

    public List<CanvasPOI> GetPOIList(string strPOIType)
    {
        Dictionary<int, CanvasPOI> pois;

        if (m_dicPOIs.TryGetValue(strPOIType, out pois))
        {
            List<CanvasPOI> poiList = new List<CanvasPOI>();
            poiList.AddRange(pois.Values);
            return poiList;
        }

        return null;
    }

    public void AddIconPOIFile(string strPOIType, string strFilePath)
    {
        MainModel.WriteLog("AddIconPOIFile : " + strFilePath);

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
            MainModel.WriteLog("[Error] AddIconPOIFile : " + e.Message);
        }
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

                if (poi.OriginalPOIType == FIRE_TYPE)
                {
                    EffectPOI effect = GetEffectPOI(FIRE_ALARM_ON_TYPE_EFFECT, poi.ID + nFireEffectID);
                    if (effect != null)
                    {
                        Dictionary<int, EffectPOI> dicEffectPOI;
                        if (m_dicEffectPOIs.TryGetValue(FIRE_ALARM_ON_TYPE_EFFECT, out dicEffectPOI))
                            RemoveEffectPOI(effect, dicEffectPOI);
                    }
                }
                
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
            try
            {
                foreach (KeyValuePair<string, Dictionary<int, EffectPOI>> pair in m_dicEffectPOIs)
                {
                    foreach (KeyValuePair<int, EffectPOI> item in pair.Value)
                    {
                        EffectPOI effectPOI = item.Value;
                        if (effectPOI != null)
                        {
                            effectPOI.bVisible = false;
                            effectPOI.gameObject.SetActive(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainModel.WriteLog("[Error] RollBackPOIIcon : " + ex.Message);
            }
        }
        else
        {
            Dictionary<int, CanvasPOI> dicPOIs;

            if (m_dicPOIs.TryGetValue(strPOIType, out dicPOIs))
                RollBackPOIIcon(dicPOIs);
        }
    }

    private void RollBackPOIIcon(Dictionary<int, CanvasPOI> dicPOIs)
    {
        foreach (KeyValuePair<int, CanvasPOI> pair in dicPOIs)
        {
            pair.Value.ChangeImage(pair.Value.OriginalPOIType);
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

    public void AddTextPOI2D(string szText, int x, int y)
    {
        y = Screen.height - y;
        Vector3 hit = ModelManager.Instance.Model.ScreenToGlobal(x, y);
        AddTextPOI(szText, hit.x, hit.y, hit.z);
    }

    public GameObject POIPref;
    public void AddTextPOI(string szText, float x, float y,float z)
    {
        GameObject obj = Instantiate(POIPref);
        obj.layer = 10; //

        Transform chile = obj.transform.GetChild(0);
        chile.localScale = new Vector3(szText.Length * 10, chile.localScale.y, chile.localScale.z);

        TextPOI poi = obj.GetComponent<TextPOI>();
        poi.ID = ModelManager.Instance.GetNextCookie();
        poi.Color = ModelManager.Instance.TextColor;

        TextMesh mNameText = obj.GetComponent<TextMesh>();
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
                mNameText.color = ModelManager.Instance.TextColor;
                mNameText.text = szText;
                mNameText.transform.position = new Vector3(x, y, z);
                Vector3 dir = Camera.main.transform.position - mNameText.transform.position;
                mNameText.transform.LookAt(mNameText.transform.position - dir);
                render.enabled = true;

                m_arTextArray.Add(poi);
                Debug.Log("AddTextPOI : " + poi.ID);

                ModelManager.Instance.Model.SaveSharedFile("AddTextPOI", 23, poi.ID, true);
            }                    
        }
    }

    public int _AddTextPOI(string szText, float x, float y, float z)
    {
        GameObject obj = Instantiate(POIPref);
        obj.layer = 10; //

        Transform chile = obj.transform.GetChild(0);
        chile.localScale = new Vector3(szText.Length*10, chile.localScale.y, chile.localScale.z);

        TextPOI poi = obj.GetComponent<TextPOI>();
        poi.ID = ModelManager.Instance.GetNextCookie();
        poi.Color = ModelManager.Instance.TextColor;

        TextMesh mNameText = obj.GetComponent<TextMesh>();
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
                mNameText.color = ModelManager.Instance.TextColor;
                mNameText.text = szText;
                mNameText.transform.position = new Vector3(x, y, z);
                Vector3 dir = Camera.main.transform.position - mNameText.transform.position;
                mNameText.transform.LookAt(mNameText.transform.position - dir);
                render.enabled = true;

                m_arTextArray.Add(poi);
                Debug.Log("AddTextPOI : " + poi.ID);

                ModelManager.Instance.Model.SaveSharedFile("AddTextPOI", 23, poi.ID, true);
            }
        }

        return poi.ID;
    }

    public void AddReverseLODTextPOIFile(string strFilePath)
    {
        StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

        while (reader.EndOfStream == false)
        {
            string strLine = reader.ReadLine().Trim();

            if (strLine.Length == 0)
                continue;

            int nIndex3 = strLine.LastIndexOf(',');
            int nIndex2 = strLine.LastIndexOf(',', nIndex3 - 1);
            int nIndex1 = strLine.LastIndexOf(',', nIndex2 - 1);

            string strName = strLine.Substring(0, nIndex1).Trim();
            string strX = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
            string strY = strLine.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1).Trim();
            string strZ = strLine.Substring(nIndex3 + 1).Trim();

            float x, y, z;

            if (float.TryParse(strX, out x) && float.TryParse(strY, out y) && float.TryParse(strZ, out z))
            {
                AddReverseLODTextPOI(strName, x, y, z);
            }
        }

        reader.Close();
        File.Delete(strFilePath);
    }

    public void AddReverseLODTextPOI(string szText, float x, float y, float z)
    {
        string strSceneName = "";
        int nIndex = szText.IndexOf('_');

        if (nIndex > 0)
        {
            strSceneName = szText.Substring(0, nIndex);
            szText = szText.Substring(nIndex + 1);
        }

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
                //render.material = mat;
                //render.materials[0] = mat;

                mNameText.font = ModelManager.Instance.m_TextFont;
                mNameText.anchor = TextAnchor.LowerCenter;
                mNameText.characterSize = 2f;
                mNameText.fontStyle = FontStyle.Normal;
                mNameText.color = ModelManager.Instance.TextColor;
                mNameText.text = szText;
                mNameText.transform.position = new Vector3(x, y, z);
                Vector3 dir = Camera.main.transform.position - mNameText.transform.position;
                mNameText.transform.LookAt(mNameText.transform.position - dir);
                render.enabled = true;

                m_arTextArray.Add(poi);
                poi.SceneName = strSceneName;

                //ModelManager.Instance.Model.SaveSharedFile("AddReverseLODTextPOI", 24, poi.ID, true);
            }
        }
    }

    public void ShowBuildingText(bool bShow)
    {
        foreach (TextPOI poi in m_arTextArray)
        {
            poi.SetVisible(bShow);
        }
    }

    private void ShowGroupName(bool bVisible)
    {

    }

    ArrayList m_arGrpNames = new ArrayList();
    public void AddGroupName(string szText, float x, float y, float z)
    {
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
