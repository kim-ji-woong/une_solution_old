using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ModelManager : MonoBehaviour
{    

    private static ModelManager m_Instance = null;
    public static ModelManager Instance
    {
        get
        {
            return m_Instance;             
        }
    }

    public Material m_HighlightMaterial;
    public Material HighlightMaterial
    {
        get { return m_HighlightMaterial; }
        set { m_HighlightMaterial = value; }
    }


    private float m_fTextLODDistance = 10.0f;
    public float TextLODDistance
    {
        get { return m_fTextLODDistance; }
        set { m_fTextLODDistance = value; }
    }
    
    private bool m_bFixedTextRatio = true;
    public bool FixTextRatio
    {
        get { return m_bFixedTextRatio; }
        set { m_bFixedTextRatio = value; }
    }

    private float m_DistanceRatioText = 110.0f;
    public float DistanceRatioText
    {
        get { return m_DistanceRatioText; }
        set { m_DistanceRatioText = value; }
    }

    public Font m_TextFont;
    public Font TextFont
    {
        get { return m_TextFont; }
        set { m_TextFont = value; }
    }

    public Material m_SpriteDefault;
    public Material SpriteDefault
    {
        get { return m_SpriteDefault; }
        set { m_SpriteDefault = value; }
    }

    public Material m_TextMaterial;
    public Material TextMaterial
    {
        get { return m_TextMaterial; }
        set { m_TextMaterial = value; }
    }

    public Color m_BDNameColor = Color.blue;
    public Color BuildingNameColor
    {
        get { return m_BDNameColor; }
        set { m_BDNameColor = value; }
    }

    public Color m_GrpNameColor = Color.yellow;
    public Color GroupNameColor
    {
        get { return m_GrpNameColor; }
        set { m_GrpNameColor = value; }
    }

    public Color m_TextColor = Color.green;
    public Color TextColor
    {
        get { return m_TextColor; }
        set { m_TextColor = value; }
    }
    
    private SortedList m_MeshAlias = new SortedList();

    private MainModel m_Model = null;
    public MainModel Model
    {
        get { return m_Model; }
        set { m_Model = value; }
    }

    private bool m_bFixedIconRatio = true;
    public bool FixIconRatio
    {
        get { return m_bFixedIconRatio; }
        set { m_bFixedIconRatio = value; }
    }

    // Inside
    private float m_DistanceRatioIcon = 80.0f;
    // outside
    //private float m_DistanceRatioText = 375.0f;
    public float DistanceRatioIcon
    {
        get { return m_DistanceRatioIcon; }
        set { m_DistanceRatioIcon = value; }
    }

    private int m_nCookie = 0;
    public int GetNextCookie()
    {
        m_nCookie++;
        return m_nCookie;
    }

    private int m_nIconCookie = 0;
    public int GetNextIconCookie()
    {
        m_nIconCookie++;
        return m_nIconCookie;
    }

    public void SetCookie(int nCookie)
    {
        m_nIconCookie = nCookie;
    }

    public void ReadIconCookie()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.position;

            string szMsg = string.Format("SendMessage('GetLastIconID({0})')", m_nIconCookie);
            Debug.unityLogger.Log(szMsg);
            proxy.RunPythonScript(szMsg);
        }
    }

    public void ReadCookie()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.position;

            string szMsg = string.Format("SendMessage('GetLastID({0})')", m_nCookie);
            Debug.unityLogger.Log(szMsg);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && proxy.UserObject != null)
        {
            
            proxy.UserObject.SetVariable("GetLastID", new Action(ReadCookie));
            proxy.UserObject.SetVariable("GetLastIconID", new Action(ReadIconCookie));
            proxy.UserObject.SetVariable("AddAliasName", new Action<string, string>(AddAliasName));

            proxy.UserObject.SetVariable("SetTextColor", new Action<int>(SetTextColor));
            proxy.UserObject.SetVariable("SetAliasTextColor", new Action<int>(SetAliasTextColor));

            proxy.UserObject.SetVariable("SetTextDistanceRatio", new Action<float>(SetTextDistanceRatio));
            proxy.UserObject.SetVariable("SetIconDistanceRatio", new Action<float>(SetIconDistanceRatio));
        }
    }

    private void SetTextDistanceRatio(float fDist)
    {
        m_DistanceRatioText = fDist;
    }

    private void SetIconDistanceRatio(float fDist)
    {
        m_DistanceRatioIcon = fDist;
    }
    
    public Color32 ToColor(int HexVal)
    {
        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, 255);
    }

    public void SetTextColor(int nColor)
    {
        m_TextColor = ToColor(nColor);
    }

    public void SetAliasTextColor(int nColor)
    {
        m_BDNameColor = ToColor(nColor);

        Model.UpdateAliasColor();
    }

    private DBUtility2.WebDBManager mWebDBManager = null;
    public DBUtility2.WebDBManager WebDB
    {
        get { return mWebDBManager; }
    }

    private int m_nSiteID = 200;
    public int SiteID
    {
        get { return m_nSiteID; }
    }

    private void CreateWebDB()
    {
        DBUtility2.Utility util = new DBUtility2.Utility();

        string szSiteID = "";
        if( Application.isEditor == false)
        {
            string szPath = Application.dataPath + "\\..\\config.ini";
            Debug.unityLogger.Log("Read INI : " + szPath);
            szSiteID = util.getinivalue("Server Connection Info", "siteid", szPath);
            Debug.unityLogger.Log("Read SiteID : " + szSiteID);
        }
        else
        {
            // 개발용 . 유니티 에디터에서 실행하는경우 UNE_HOME에 위치한 Config파일을 읽는다.
            string szPath = Environment.GetEnvironmentVariable("UNE_HOME") + "\\bin\\common12\\config.ini";
            szSiteID = util.getinivalue("Server Connection Info", "siteid", szPath);
            Debug.unityLogger.Log("Read SiteID : " + szSiteID);
        }
      
        int nSiteId = 200;
        if (int.TryParse(szSiteID, out nSiteId))
        {
            m_nSiteID = nSiteId;
        }

        if (m_nSiteID == 0)
            m_nSiteID = 201;
        Debug.unityLogger.Log("Set SiteID : " + m_nSiteID);

        mWebDBManager = new DBUtility2.WebDBManager(m_nSiteID);
        
    }

    public void LoadData()
    {
        SDMS.ZoneManager.Instance.LoadBuildingData();
        SDMS.ZoneManager.Instance.LoadZones();
        SDMS.ZoneManager.Instance.LoadEquipmentZone();
        SDMS.ZoneManager.Instance.Load3DText();

        
    }

    void Awake()
    {
        MainModel.WriteLog("ModelManager.Awake");
        m_Instance = this;

        CreateWebDB();

        LoadData();

        OnReady();

        AddPythonFunction();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void AddAliasName(string szMeshName, string szAliasName)
    {
        if (!m_MeshAlias.Contains(szMeshName))
            m_MeshAlias.Add(szMeshName, szAliasName);
        else
        {
            m_MeshAlias.Remove(szMeshName);
            m_MeshAlias.Add(szMeshName, szAliasName);
        }
    }

    public string GetAliasName(string szMeshName)
    {
        if (!m_MeshAlias.Contains(szMeshName))
            return "";
        string szName = (string)m_MeshAlias[szMeshName];

        Debug.unityLogger.Log("Find : " + szMeshName + "," + szName);
        return szName;
    }
    
    public void OnReady()
    {
        List<SDMS.Building> arBuildings = new List<SDMS.Building>(SDMS.ZoneManager.Instance.DicBuildings.Values);
        foreach(SDMS.Building building in arBuildings)
        {
            string szMeshName = building.BuildingID;

            string szAliasName = building.DisplayText;
            AddAliasName(szMeshName, szAliasName);
        }
    }    

    public void ClearSelect()
    {

    }
}
