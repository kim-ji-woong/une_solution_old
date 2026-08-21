using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
//using UnityEditorInternal;
using DBUtility2;

public class POIManager : MonoBehaviour
{
    private const string CCTV_TYPE = "CCTV";
    private const string CCTV1_TYPE = "CCTV_1";
    private const string CCTV2_TYPE = "CCTV_2";
    private const string CCTV3_TYPE = "CCTV_3";
    private const string CCTV4_TYPE = "CCTV_4";
    private const string DOOR_TYPE = "Door";
    private const string DOOR_ALARM_ON_TYPE = "DoorAlarmOn";
    private const string FIRE_TYPE = "Fire";
    private const string FIRE_ALARM_ON_TYPE = "FireAlarmOn";
    private const string FIREWALL_TYPE = "FireWall";
    private const string FIREWALL_ALARM_ON_TYPE = "FireWallAlarmOn";
    private const string GAS_TYPE = "Gas";
    private const string GAS_ALARM_ON_TYPE = "GasAlarmOn";

    // Key : Index
    private Dictionary<int, GameObject> m_dicCCTVGroup = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> m_dicDoorGroup = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> m_dicFireGroup = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> m_dicFireWallGroup = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> m_dicGasGroup = new Dictionary<int, GameObject>();

    // Key : ID
    // Value : Index
    private Dictionary<int, int> m_dicCCTVIndex = new Dictionary<int, int>();
    private Dictionary<int, int> m_dicDoorIndex = new Dictionary<int, int>();
    private Dictionary<int, int> m_dicFireIndex = new Dictionary<int, int>();
    private Dictionary<int, int> m_dicFireWallIndex = new Dictionary<int, int>();
    private Dictionary<int, int> m_dicGasIndex = new Dictionary<int, int>();

    private List<int> m_cctvIndexes = new List<int>();
    private List<int> m_doorIndexes = new List<int>();
    private List<int> m_fireIndexes = new List<int>();
    private List<int> m_fireWallIndexes = new List<int>();
    private List<int> m_gasIndexes = new List<int>();
    private int m_nCCTVArrayCount = 0;
    private int m_nDoorArrayCount = 0;
    private int m_nFireArrayCount = 0;
    private int m_nFireWallArrayCount = 0;
    private int m_nGasArrayCount = 0;
    
    private GameObject m_originalCCTVPOI = null;
    private GameObject m_originalCCTV1POI = null;
    private GameObject m_originalCCTV2POI = null;
    private GameObject m_originalCCTV3POI = null;
    private GameObject m_originalCCTV4POI = null;
    private GameObject m_originalDoorPOI = null;
    private GameObject m_originalDoorAlarmOnPOI = null;
    private GameObject m_originalFirePOI = null;
    private GameObject m_originalFireAlarmOnPOI = null;
    private GameObject m_originalFireWallPOI = null;
    private GameObject m_originalFireWallAlarmOnPOI = null;
    private GameObject m_originalGasPOI = null;
    private GameObject m_originalGasAlarmOnPOI = null;

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
        Debug.unityLogger.Log("Clear Select All POI ");

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
                    Debug.unityLogger.Log("Select POI Icon : " + nID + " , " + bSelect);
                    poi.SelectPOI(bSelect);                   
                }
                else
                {
                    Debug.unityLogger.Log("Select POI Icon : " + nID + " , " + false);
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
                    Debug.unityLogger.Log("Icon : " + nID + " , " + bSelect);
                    poi.SelectPOI(bSelect);
                    break;
                }
            }
        }
       
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

    private void ShowIconLayer(string iconTypeName, bool bShow)
    {
        Dictionary<int, int> dicIndex;
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
        }
    }

    private void ShowIconLayers(string iconTypeNames, bool hideOthers)
    {
        Dictionary<string, string> dicIconNames = ParseIconNames(iconTypeNames);

        Dictionary<int, int> dicIndex;
        List<int> indexes;
        GameObject obj;

        List<string> allPOITypes = GetAllTypes();

        foreach (KeyValuePair<string, string> pair in dicIconNames)
        {
            Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(pair.Value, out dicIndex, out indexes);

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
            }
        }

        if (hideOthers)
        {
            foreach (string strPOIType in allPOITypes)
            {
                Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strPOIType, out dicIndex, out indexes);

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
                }
            }
        }
    }

    private void HideIconLayers(string iconTypeNames, bool showOthers)
    {
        Dictionary<string, string> dicIconNames = ParseIconNames(iconTypeNames);

        Dictionary<int, int> dicIndex;
        List<int> indexes;
        GameObject obj;

        List<string> allPOITypes = GetAllTypes();

        foreach (KeyValuePair<string, string> pair in dicIconNames)
        {
            Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(pair.Value, out dicIndex, out indexes);

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
            }
        }

        if (showOthers)
        {
            foreach (string strPOIType in allPOITypes)
            {
                Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strPOIType, out dicIndex, out indexes);
                
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
                }
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

    private bool IsInclude(IconPOI poi, Dictionary<string, string> dicIconNames)
    {
        foreach (KeyValuePair<string, string> pair in dicIconNames)
        {
            if (poi.IconName.StartsWith(pair.Value))
                return true;
        }

        return false;
    }

    public void ShowIconPOI(int nID, string type, bool bShow)
    {
        foreach (IconPOI poi in m_arIconArray)
        {
            if (poi.ID == nID || poi.IconType == type)
            {
                Debug.unityLogger.Log("Icon : " + nID + " , " + bShow);
                poi.SetVisible(bShow);
                break;
            }
        }     
    }

    private void InitIndexList(List<int> indexes, int nArrayCount)
    {
        indexes.Clear();

        for (int i = 1; i <= nArrayCount; i++)
        {
            indexes.Add(i);
        }
    }

    private void HideAllCCTV()
    {
        foreach (KeyValuePair<int, GameObject> pair in m_dicCCTVGroup)
        {
            pair.Value.SetActive(false);
        }

        m_dicCCTVIndex.Clear();
        InitIndexList(m_cctvIndexes, m_nCCTVArrayCount);
    }

    private void HideAllDoor()
    {
        foreach (KeyValuePair<int, GameObject> pair in m_dicDoorGroup)
        {
            pair.Value.SetActive(false);
        }

        m_dicDoorIndex.Clear();
        InitIndexList(m_doorIndexes, m_nDoorArrayCount);
    }

    private void HideAllFire()
    {
        foreach (KeyValuePair<int, GameObject> pair in m_dicFireGroup)
        {
            pair.Value.SetActive(false);
        }

        m_dicFireIndex.Clear();
        InitIndexList(m_fireIndexes, m_nFireArrayCount);
    }

    private void HideAllFireWall()
    {
        foreach (KeyValuePair<int, GameObject> pair in m_dicFireWallGroup)
        {
            pair.Value.SetActive(false);
        }

        m_dicFireWallIndex.Clear();
        InitIndexList(m_fireWallIndexes, m_nFireWallArrayCount);
    }

    private void HideAllGas()
    {
        foreach (KeyValuePair<int, GameObject> pair in m_dicGasGroup)
        {
            pair.Value.SetActive(false);
        }

        m_dicGasIndex.Clear();
        InitIndexList(m_gasIndexes, m_nGasArrayCount);
    }

    public void ShowAllCCTV()
    {
        foreach (KeyValuePair<int, GameObject> pair in m_dicCCTVGroup)
        {
            pair.Value.SetActive(true);
        }
    }

    public void ShowCCTV(int nID)
    {
        GameObject cctv;

        if (m_dicCCTVGroup.TryGetValue(nID, out cctv))
        {
            IconPOI poi = cctv.GetComponent<IconPOI>();

            if (poi != null)
            {
                poi.Position = new Vector3(-41, 10, -26);
                //cctv.transform.localPosition = new Vector3(-41, 10, -26);
                //cctv.SetActive(true);
            }

            cctv.SetActive(true);
        }
    }

    public void ShowIconPOIFile(string strFilePath)
    {
        MainModel.WriteLog("ShowIconPOIFile");
        StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

        string strAlarmOn = "AlarmOn";

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

                bool isAlarmPOI = strType.Contains(strAlarmOn);

                if (isAlarmPOI)
                    strType = strType.Replace(strAlarmOn, "");

                int nID;
                bool visible;

                if (int.TryParse(strID, out nID) && StringToBoolean(strVisible, out visible))
                {
                    Dictionary<int, int> dicPOIIndex = null;
                    List<int> indexes;
                    Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strType, out dicPOIIndex, out indexes);

                    if (dicPOIGroup != null)
                    {
                        GameObject poi;
                        int nPOIIndex;

                        if (dicPOIIndex.TryGetValue(nID, out nPOIIndex))
                        {
                            if (dicPOIGroup.TryGetValue(nPOIIndex, out poi))
                            {
                                if (IsVisible(strType))
                                    poi.SetActive(visible);
                                else
                                    poi.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        reader.Close();
        File.Delete(strFilePath);
    }

    private bool IsVisible(string strLayerName)
    {
        bool visible = false;

        if (m_dicLayers.TryGetValue(strLayerName, out visible))
            return visible;

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
        MainModel.WriteLog("POIManager.Awake");
        Debug.Log("POIManager.Awake");
        m_Instance = this;
        AddPythonFunction();

        AddCCTVGroup(50);
        AddDoorGroup(30);
        AddFireGroup(30);
        AddFireWallGroup(30);
        AddGasGroup(30);

        SetOriginalPOI(ref m_originalCCTVPOI, CCTV_TYPE);
        SetOriginalPOI(ref m_originalCCTV1POI, CCTV1_TYPE);
        SetOriginalPOI(ref m_originalCCTV2POI, CCTV2_TYPE);
        SetOriginalPOI(ref m_originalCCTV3POI, CCTV3_TYPE);
        SetOriginalPOI(ref m_originalCCTV4POI, CCTV4_TYPE);
        SetOriginalPOI(ref m_originalDoorPOI, DOOR_TYPE);
        SetOriginalPOI(ref m_originalDoorAlarmOnPOI, DOOR_ALARM_ON_TYPE);
        SetOriginalPOI(ref m_originalFirePOI, FIRE_TYPE);
        SetOriginalPOI(ref m_originalFireAlarmOnPOI, FIRE_ALARM_ON_TYPE);
        SetOriginalPOI(ref m_originalFireWallPOI, FIREWALL_TYPE);
        SetOriginalPOI(ref m_originalFireWallAlarmOnPOI, FIREWALL_ALARM_ON_TYPE);
        SetOriginalPOI(ref m_originalGasPOI, GAS_TYPE);
        SetOriginalPOI(ref m_originalGasAlarmOnPOI, GAS_ALARM_ON_TYPE);
    }

    private void SetOriginalPOI(ref GameObject poi, string strPOIType)
    {
        string strName = strPOIType + "Origin";
        poi = GameObject.Find(strName);

        if (poi != null)
            poi.SetActive(false);
    }

    private void AddCCTVGroup(int nCCTVCount)
    {
        m_dicLayers[CCTV_TYPE] = true;

        for (int i=1;i<= nCCTVCount; i++)
        {
            AddCCTV(i);
        }

        InitIndexList(m_cctvIndexes, nCCTVCount);
        m_nCCTVArrayCount = nCCTVCount;
    }

    private void AddDoorGroup(int nDoorCount)
    {
        m_dicLayers[DOOR_TYPE] = true;

        for (int i = 1; i <= nDoorCount; i++)
        {
            AddDoor(i);
        }

        InitIndexList(m_doorIndexes, nDoorCount);
        m_nDoorArrayCount = nDoorCount;
    }

    private void AddFireGroup(int nFireCount)
    {
        m_dicLayers[FIRE_TYPE] = true;

        for (int i = 1; i <= nFireCount; i++)
        {
            AddFire(i);
        }

        InitIndexList(m_fireIndexes, nFireCount);
        m_nFireArrayCount = nFireCount;
    }

    private void AddFireWallGroup(int nFireWallCount)
    {
        m_dicLayers[FIREWALL_TYPE] = true;

        for (int i = 1; i <= nFireWallCount; i++)
        {
            AddFireWall(i);
        }

        InitIndexList(m_fireWallIndexes, nFireWallCount);
        m_nFireWallArrayCount = nFireWallCount;
    }

    private void AddGasGroup(int nGasCount)
    {
        m_dicLayers[GAS_TYPE] = true;

        for (int i = 1; i <= nGasCount; i++)
        {
            AddGas(i);
        }

        InitIndexList(m_gasIndexes, nGasCount);
        m_nGasArrayCount = nGasCount;
    }

    private void AddCCTV(int nIndex)
    {
        string strName = string.Format("CCTV_{0:00}", nIndex);
        GameObject cctv = GameObject.Find(strName);

        if (cctv != null)
        {
            m_dicCCTVGroup[nIndex] = cctv;
            cctv.SetActive(false);
        }
    }

    private void AddDoor(int nIndex)
    {
        string strName = string.Format("Door_{0:00}", nIndex);
        GameObject door = GameObject.Find(strName);

        if (door != null)
        {
            m_dicDoorGroup[nIndex] = door;
            door.SetActive(false);
        }
    }

    private void AddFire(int nIndex)
    {
        string strName = string.Format("Fire_{0:00}", nIndex);
        GameObject fire = GameObject.Find(strName);

        if (fire != null)
        {
            m_dicFireGroup[nIndex] = fire;
            fire.SetActive(false);
        }
    }

    private void AddFireWall(int nIndex)
    {
        string strName = string.Format("FireWall_{0:00}", nIndex);
        GameObject fireWall = GameObject.Find(strName);

        if (fireWall != null)
        {
            m_dicFireWallGroup[nIndex] = fireWall;
            fireWall.SetActive(false);
        }
    }

    private void AddGas(int nIndex)
    {
        string strName = string.Format("Gas_{0:00}", nIndex);
        GameObject gas = GameObject.Find(strName);

        if (gas != null)
        {
            m_dicGasGroup[nIndex] = gas;
            gas.SetActive(false);
        }
    }

    void Start ()
    {
        AddTextPOI("가나다", 0, 0, 0);
        AddTextPOI("가나다", 0, 10, 0);
        AddTextPOI("가나다234234", 0, 20, 0);
        AddTextPOI("가asdfs", 10, 0, 0);

        ShowTextPOI(0, true);
        ShowTextPOI(1, true);
        ShowTextPOI(2, true);
        ShowTextPOI(3, true);
    }  
	
	void Update () 
    {
    }

    public IconPOI AddIcon(string strIconType, int nID, Vector3 vPos)
    {
        if (nID < 0)
            return null;

        Dictionary<int, int> dicPOIIndex = null;
        List<int> indexes;
        Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strIconType, out dicPOIIndex, out indexes);

        if (dicPOIGroup == null)
            return null;

        int nIndex = strIconType.IndexOf('_');

        if (nIndex > 0)
            strIconType = strIconType.Substring(0, nIndex);

        GameObject poi;
        int nPOIIndex = PopIndex(indexes);

        if (nPOIIndex < 0)
            return null;

        if (dicPOIGroup.TryGetValue(nPOIIndex, out poi))
        {
            IconPOI icon = poi.GetComponent<IconPOI>();

            if (icon != null)
            {
                icon.Position = vPos;
                icon.IconType = strIconType;
                icon.ID = nID;

                dicPOIIndex[nID] = nPOIIndex;

                icon.SetVisible(true);
                poi.SetActive(true);
                return icon;
            }
            else
                PushIndex(indexes, nPOIIndex);
        }

        return null;
    }

    private int PopIndex(List<int> indexes)
    {
        if (indexes.Count == 0)
            return -1;

        int nIndex = indexes[0];
        indexes.RemoveAt(0);
        return nIndex;
    }

    private void PushIndex(List<int> indexes, int nIndex)
    {
        indexes.Insert(0, nIndex);
    }

    public bool RemoveIcon(IconPOI poi)
    {
        Dictionary<int, int> dicPOIIndex = null;
        List<int> indexes;
        Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(poi.IconType, out dicPOIIndex, out indexes);

        if (dicPOIGroup == null)
            return false;

        int nIndex;
        GameObject obj;

        if (dicPOIIndex.TryGetValue(poi.ID, out nIndex) == false)
            return false;

        if (dicPOIIndex.Remove(poi.ID) == false)
            return false;

        if (dicPOIGroup.TryGetValue(nIndex, out obj) == false)
            return false;

        PushIndex(indexes, nIndex);

        poi.SetVisible(false);
        obj.SetActive(false);

        return true;
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
        AddIconPOI(szPath, x, y, z, true);
    }

    private int AddIconPOI(string szPath, float x, float y, float z, bool useSharedFile)
    {
        int nCountID = ModelManager.Instance.GetNextIconCookie();
        string[] paths = szPath.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (paths == null || paths.Length != 3)
        {
            if (useSharedFile)
                ModelManager.Instance.Model.SaveSharedFile("AddIconPOI", 22, -1, true);
            return -1;
        }

        int nID = -1;
        if (int.TryParse(paths[2], out nID))
        {
            if (nID < 0)
            {
                nID = nCountID;
            }

            if (nID > nCountID)
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
            Debug.unityLogger.Log(szMsg);

            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<UnityEngine.Sprite>(paths[0]);

            if (spriteRenderer.sprite == null)
                Debug.Log("Sprite is null : " + paths[0]);

            spriteRenderer.material = ModelManager.Instance.SpriteDefault;
            spriteRenderer.color = Color.red;

            //int[] nLayers = GetSortingLayerUniqueIDs();
            // if (nLayers != null && nLayers.Length > 1)
            spriteRenderer.sortingLayerName = "POIIcon";
            spriteRenderer.sortingOrder = 0;
            //poi.m_nSoringLayerID = nLayers[1];
            Vector3 vec = spriteRenderer.sprite.bounds.extents;

            obj.transform.position = new Vector3(x, y + m_fExtraHeight, z);

            m_arIconArray.Add(poi);

        }

        if (useSharedFile)
            ModelManager.Instance.Model.SaveSharedFile("AddIconPOI", 22, nID, true);

        return nID;
    }

    private int AddIconPOI2(string szPath, float x, float y, float z)
    {
        MainModel.WriteLog("AddIconPOI2 : " + szPath);
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
        string strAlarmOn = "AlarmOn";
        bool isAlarmPOI = strPOIType.Contains(strAlarmOn);

        if (isAlarmPOI)
            strPOIType = strPOIType.Replace(strAlarmOn, "");

        Dictionary<int, int> dicPOIIndex = null;
        List<int> indexes;
        Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strPOIType, out dicPOIIndex, out indexes);

        if (dicPOIGroup == null)
            return -1;

        GameObject poi;
        int nPOIIndex = PopIndex(indexes);

        if (dicPOIGroup.TryGetValue(nPOIIndex, out poi))
        {
            IconPOI icon = poi.GetComponent<IconPOI>();

            if (icon != null)
            {
                icon.Position = new Vector3(x, y, z);
                icon.IconType = strPOIType;
                icon.ID = nID;

                dicPOIIndex[nID] = nPOIIndex;

                if (isAlarmPOI)
                    ChangePOIIcon(poi, strPOIType + strAlarmOn);
            }
            else
                PushIndex(indexes, nPOIIndex);
        }

        return nID;
    }

    private Dictionary<int, GameObject> GetPOIGroup(string strPOIType, out Dictionary<int, int> dicIndex, out List<int> indexes)
    {
        dicIndex = null;
        indexes = null;

        if (strPOIType == CCTV_TYPE)
        {
            dicIndex = m_dicCCTVIndex;
            indexes = m_cctvIndexes;
            return m_dicCCTVGroup;
        }
        else if (strPOIType == DOOR_TYPE)
        {
            dicIndex = m_dicDoorIndex;
            indexes = m_doorIndexes;
            return m_dicDoorGroup;
        }
        else if (strPOIType == FIRE_TYPE)
        {
            dicIndex = m_dicFireIndex;
            indexes = m_fireIndexes;
            return m_dicFireGroup;
        }
        else if (strPOIType == FIREWALL_TYPE)
        {
            dicIndex = m_dicFireWallIndex;
            indexes = m_fireWallIndexes;
            return m_dicFireWallGroup;
        }
        else if (strPOIType == GAS_TYPE)
        {
            dicIndex = m_dicGasIndex;
            indexes = m_gasIndexes;
            return m_dicGasGroup;
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

                MainModel.WriteLog("ReadLine : " + strLine);

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
                        //int nID = AddIconPOI(strIconName, x, y, z, false);
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

    public void ClearIconPOI(string strPOIType)
    {
        MainModel.WriteLog("ClearIconPOI : " + strPOIType);

        if (strPOIType == CCTV_TYPE)
            HideAllCCTV();
        else if (strPOIType == DOOR_TYPE)
            HideAllDoor();
        else if (strPOIType == FIRE_TYPE)
            HideAllFire();
        else if (strPOIType == FIREWALL_TYPE)
            HideAllFireWall();
        else if (strPOIType == GAS_TYPE)
            HideAllGas();
        else if (strPOIType.Length == 0)
        {
            HideAllCCTV();
            HideAllDoor();
            HideAllFire();
            HideAllFireWall();
            HideAllGas();
        }
    }

    private GameObject GetOriginalPOI(string strPOIType)
    {
        if (strPOIType == CCTV_TYPE)
            return m_originalCCTVPOI;
        else if (strPOIType == DOOR_TYPE)
            return m_originalDoorPOI;
        else if (strPOIType == DOOR_ALARM_ON_TYPE)
            return m_originalDoorAlarmOnPOI;
        else if (strPOIType == FIRE_TYPE)
            return m_originalFirePOI;
        else if (strPOIType == FIRE_ALARM_ON_TYPE)
            return m_originalFireAlarmOnPOI;
        else if (strPOIType == FIREWALL_TYPE)
            return m_originalFireWallPOI;
        else if (strPOIType == FIREWALL_ALARM_ON_TYPE)
            return m_originalFireWallAlarmOnPOI;
        else if (strPOIType == GAS_TYPE)
            return m_originalGasPOI;
        else if (strPOIType == GAS_ALARM_ON_TYPE)
            return m_originalGasAlarmOnPOI;
        else if (strPOIType == CCTV1_TYPE)
            return m_originalCCTV1POI;
        else if (strPOIType == CCTV2_TYPE)
            return m_originalCCTV2POI;
        else if (strPOIType == CCTV3_TYPE)
            return m_originalCCTV3POI;
        else if (strPOIType == CCTV4_TYPE)
            return m_originalCCTV4POI;

        return null;
    }

    public void ChangePOIIcon(string strPOIInfo, string strPOIType)
    {
        MainModel.WriteLog("ChangePOIIcon : " + strPOIInfo + ", " + strPOIType);
        string[] tokens = strPOIInfo.Split('_');

        if (tokens.Length != 2)
            return;

        string strOldPOIType = tokens[0];
        int nID;

        if (int.TryParse(tokens[1], out nID) == false)
            return;

        Dictionary<int, int> dicIndex;
        List<int> indexes;
        Dictionary<int, GameObject> dicPOIGroup = GetPOIGroup(strOldPOIType, out dicIndex, out indexes);

        if (dicPOIGroup == null)
            return;

        int nIndex;

        if (dicIndex.TryGetValue(nID, out nIndex) == false)
            return;

        GameObject poi;

        if (dicPOIGroup.TryGetValue(nIndex, out poi) == false)
            return;

        ChangePOIIcon(poi, strPOIType);
    }

    private void ChangePOIIcon(GameObject poi, string strPOIType)
    {
        GameObject originalPOI = GetOriginalPOI(strPOIType);

        if (originalPOI == null)
            return;

        SpriteRenderer spriteRendererSource = poi.GetComponent<SpriteRenderer>();
        SpriteRenderer spriteRendererTarget = originalPOI.GetComponent<SpriteRenderer>();

        if (spriteRendererSource == null)
            MainModel.WriteLog("ChangePOIIcon : spriteRendererSource is null");
        if (spriteRendererTarget == null)
            MainModel.WriteLog("ChangePOIIcon : spriteRendererTarget is null");

        if (spriteRendererSource != null && spriteRendererTarget != null)
        {
            spriteRendererSource.sprite = spriteRendererTarget.sprite;
            MainModel.WriteLog("ChangePOIIcon : Success");
        }
    }

    public void ChangePOIIconFile(string strFilePath)
    {
        MainModel.WriteLog("Change : " + strFilePath);
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
        MainModel.WriteLog("Rollback : " + strPOIType);
        if (strPOIType.Length == 0)
        {
            RollBackPOIIcon(m_dicCCTVGroup, m_originalCCTVPOI);
            RollBackPOIIcon(m_dicDoorGroup, m_originalDoorPOI);
            RollBackPOIIcon(m_dicFireGroup, m_originalFirePOI);
            RollBackPOIIcon(m_dicFireWallGroup, m_originalFireWallPOI);
        }
        else if (strPOIType == CCTV_TYPE)
            RollBackPOIIcon(m_dicCCTVGroup, m_originalCCTVPOI);
        else if (strPOIType == DOOR_TYPE)
            RollBackPOIIcon(m_dicDoorGroup, m_originalDoorPOI);
        else if (strPOIType == FIRE_TYPE)
            RollBackPOIIcon(m_dicFireGroup, m_originalFirePOI);
        else if (strPOIType == FIREWALL_TYPE)
            RollBackPOIIcon(m_dicFireWallGroup, m_originalFireWallPOI);
    }

    private void RollBackPOIIcon(Dictionary<int, GameObject> dicPOIGroup, GameObject targetPOI)
    {
        foreach (KeyValuePair<int, GameObject> pair in dicPOIGroup)
        {
            SpriteRenderer spriteRendererSource = pair.Value.GetComponent<SpriteRenderer>();
            SpriteRenderer spriteRendererTarget = targetPOI.GetComponent<SpriteRenderer>();

            if (spriteRendererSource != null && spriteRendererTarget != null)
            {
                spriteRendererSource.sprite = spriteRendererTarget.sprite;
            }
        }
    }

    private void OnAddPOIFile(string strPOIType, string strIDFilePath)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            //string szMsg = string.Format("SendMessage('AddIconPOIFileFinish(123)')");
            string szMsg = string.Format("SendMessage('AddIconPOIFileFinish({0}, {1})')", strPOIType, strIDFilePath);
            proxy.RunPythonScript(szMsg);
        }
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
