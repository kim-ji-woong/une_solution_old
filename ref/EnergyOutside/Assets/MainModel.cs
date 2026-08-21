using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct INFO_POI
{
    public int bSet;
    public int nID;
    public float x;
    public float y;
    public float z;
    public int nx;
    public int ny;
    public int bSelect;   
}

public class MainModel : MonoBehaviour
{

    private Dictionary<string, int> mSharedCmdMap = new Dictionary<string, int>();
    private string[] szKeyName = 
    {
	    "SetMainModel", 
	    "CameraTranslate",
	    "CameraPosition",
	    "CameraAngles",
	    "CameraDirection",
	    "SetCameraPosition",
	    "SetCameraAngles",
	    "SetCameraDirection",
	    "SetZoomPosition",
	    "SetZoomObject",
	    "SetZoomObjectDistance",
	    "SetZoomObjectAngle",
	    "CameraView",
	    "SelectObject",
	    "SetEditMode",
	    "SetMode",
	    "Get3DPosition",
	    "UpdateAliasNames",
	    "ClearAllSelect",
	    "Get2DPosition",
	    "ModelZoom",
	    "SaveScreenShot",
	    "GetLastID",
	    "GetLastIconID",
	    "AddAliasName",
	    "SetTextColor",
	    "SetAliasTextColor",
	    "SetTextDistanceRatio",
	    "SetIconDistanceRatio",
	    "AddTextPOI",
	    "AddReverseLODTextPOI",
	    "AddIconPOI",
	    "ShowTextPOI",
	    "ShowIconPOI",
	    "ShowIconLayer",
	    "SelectIconPOI",
	    "RemoveIconPOI",
	    "ShowOutZoneVolume",
	    "HideOutZonevolume",
	    "HideAllOutZoneVolume",
	    "ShowEquipZoneVolume",
	    "HideEquipZonevolume",
	    "HideAllEquipZoneVolume",         
        "ShowEvacCircle", 
        "SetInitEvacDistance", 
        "SetSecondEvacDistance",
        "SetEvacCircleCenter",
        "ClearSelectIconPOI",
        "HideEmpoll",
        "ShowEmpoll",
        "HideAllEmpoll",
        "SetEarthquake",
        "ShowPolㅣution",
        "HidePolㅣution"              
    };


    public bool m_bOrbitMode = false;
    private bool m_bDragged = false;
    public bool m_bTranslateMode = false;
    public bool m_bPickMode = false;

    private bool m_bZoominng = false;
    private bool m_bPanning = false;
    
    private Vector3 m_MousePosCur;
    private Vector3 m_MousePosStart;
    private Vector3 m_MousePosPrev;

    private Vector3 mTranslatePt;
    private Vector3 mTranslateStartPt;
    private Vector3 m_ScreenCenter;

    private Vector3 mOrbitCenter;
    private float m_RotRatioX = 0.0125f;
    private float m_RotRatioY = 0.0125f;

    private Vector3 mZoomCenter;
    private float m_fZoomDistance = 10.0f;

    public Camera m_MainCamera;
    public Color color = Color.green;
    public Collider coll;
    GameObject[] apartments = null;

    public Material grayMaterial;

    public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };
    //10분단위
    public Dictionary<int, string> pollutionDic = new Dictionary<int, string>();
   
    private MouseWorkMode m_nMode = MouseWorkMode.NONE;
    public MouseWorkMode Mode
    {
        get { return m_nMode; }
        set { m_nMode = value; }
    }

    private SharedMemory.BufferReadWrite file = null;

    private bool m_bEditMode = false;
    public bool EditMode
    {
        get { return m_bEditMode; }
        set { m_bEditMode = value; }
    }

    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            proxy.UserObject.SetVariable("SetEditMode", new Action<bool>(SetEditMode));
            proxy.UserObject.SetVariable("SetMode", new Action<int, bool>(SetMode));
            proxy.UserObject.SetVariable("Get3DPosition", new Action<int, int>(Get3DPosition));
            proxy.UserObject.SetVariable("UpdateAliasNames", new Action(UpdateAliasName));
            proxy.UserObject.SetVariable("ClearAllSelect", new Action(ClearSelect));
            proxy.UserObject.SetVariable("Get2DPosition", new Action<int, float, float, float>(Get2DPosition));
            proxy.UserObject.SetVariable("ModelZoom", new Action<float>(ManualZoom));
            proxy.UserObject.SetVariable("SaveScreenShot", new Action<string>(SaveScreenShot));

            proxy.UserObject.SetVariable("ShowBuildingText", new Action<bool>(ShowBuildingText));

            proxy.UserObject.SetVariable("ShowEvacCircle", new Action<int>(ShowEvacCircle));
            proxy.UserObject.SetVariable("SetEvacCircleCenter", new Action<float, float, float>(SetEvacCircleCenter));
            proxy.UserObject.SetVariable("SetInitEvacDistance", new Action<int>(SetInitEvacDistance));
            proxy.UserObject.SetVariable("SetSecondEvacDistance", new Action<int>(SetSecondEvacDistance));

            proxy.UserObject.SetVariable("HideAllEmpoll", new Action(HideAllEmpoll));
            proxy.UserObject.SetVariable("HideEmpoll", new Action<int>(HideEmpoll));
            proxy.UserObject.SetVariable("ShowEmpoll", new Action<int>(ShowEmpoll));
            proxy.UserObject.SetVariable("ShowPollution", new Action<int, int>(ShowPollution));
            proxy.UserObject.SetVariable("HidePollution", new Action(HidePollution));
        }
    }

    /*
     * SelectPolutionImage(int offsetImage)
     * Selection Projection Polution Image : by hypark.
     * 
     */

    private bool isPollutionProcessView = false;
    private int pollutionDirection = 0;
    private string windStrengthStr = "X";
    private List<GameObject> exChildObjects = new List<GameObject>();
    private Vector3 backgroundObjectPos = new Vector3(0,0,0);

    public void ShowPollution(int direction, int windStrength)
    {

        /*
        direction 0 : N, 1 : NE, E : 2, 3 : SE, 4 : S, 5 : SW, 6 : W, 7 : NW
        windStrength 2 : 오염도 약함(M:바람강함),  0: 오염도 강함(X:바람약함)    
        */

        // string timestr = time.ToString("D4");
        GameObject exObject = transform.Find("EX").gameObject;
        
        Transform[] exChildTransfroms = exObject.GetComponentsInChildren<Transform>();  //모든 child를 다 포함한다(최하단까지).
        
        foreach (Transform child in exChildTransfroms)
        {
            
            if (!child.gameObject.tag.Equals("apart") && child.gameObject.name.StartsWith("V2X"))   //포함할 child 필터링
            {
                exChildObjects.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
                
        }
        GameObject backObject = transform.Find("background").gameObject;
        backgroundObjectPos = backObject.transform.position;        
        backObject.SetActive(true);
        backObject.transform.position = exObject.transform.position;
        currentViewOffset = 0;
        pollutionDirection = direction;
        switch (windStrength)
        {
            case 0 :                    
                windStrengthStr = "X";
                break;
            case 1 :
                windStrengthStr = "A";  //사용 안함.
                break;
            case 2 :
                windStrengthStr = "M";
                break;
        }
        isPollutionProcessView = true;
        SaveSharedFile("ShowPolution");

    }

    public void HidePollution()
    {
        if (exChildObjects.Count > 0 && isPollutionProcessView)       //EX의 child list가 이미 clear된 경우엔 무시 
        {
            isPollutionProcessView = false;
            currentViewOffset = 0;
            nextTime = 0.0f;
            GameObject exObject = transform.Find("EX").gameObject;

            foreach (GameObject child in exChildObjects)
            {
                child.SetActive(true);
            }
            GameObject backObject = transform.Find("background").gameObject;
            backObject.transform.position = backgroundObjectPos;
            backObject.SetActive(false);
            exChildObjects.Clear();
            SaveSharedFile("HidePollution");
            
        }
        
       

    }
    public void HideAllEmpollPolygon()
    {
        for(int i = 0 ; i < 8 ; i++)
        {
            Transform em1 = transform.Find("EMPOLL_400" + i);
            if (em1 != null)
            {
                MeshRenderer mr2 = em1.gameObject.GetComponent<MeshRenderer>();
                if (mr2 != null)
                    mr2.enabled = false;
            }
        }        
    }

    public void HideAllEmpoll()
    {
        HideAllEmpollPolygon();
        SaveSharedFile("HideAllEmpoll");
    }

    public void HideEmpoll(int nPollID)
    {
        //Transform em = transform.Find("EMPOLL");
        //if (em != null)
        {
            Transform t = transform.Find("EMPOLL_" + nPollID);
            if (t != null)
            {
                MeshRenderer mr2 = t.gameObject.GetComponent<MeshRenderer>();
                if (mr2 != null)
                    mr2.enabled = true;

                Debug.logger.Log("HideEmpoll " + nPollID);
            }
        }
        SaveSharedFile("HideEmpoll");
    }

    public void ShowEmpoll(int nPollID)
    {
        //Transform em = transform.Find("EMPOLL");
        //if( em  != null)
        {
            Transform t = transform.Find("EMPOLL_" + nPollID);
            if( t != null)
            {   
                MeshRenderer mr2 = t.gameObject.GetComponent<MeshRenderer>();
                if(mr2 != null)
                    mr2.enabled = true;

                 Debug.logger.Log("ShowEmpoll " + nPollID);
            }         
        }  
        SaveSharedFile("ShowEmpoll");
    }
    
    public void SetEvacCircleCenter(float x, float y, float z)
    {
        foreach (Transform child in transform)
        {
           // MeshFilter mf = child.GetComponent<MeshFilter>();
          //  if (mf != null)
          //  {
                EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
                if (sm != null)
                {
                    Debug.logger.Log("SetEvacCircleCenter");

                    float dy = child.position.y;
                    child.position = new Vector3(x, dy, z);
                }
          //  }
        }

        SaveSharedFile("SetEvacCircleCenter");
    }

    public void ShowEvacCircle(int nLevel)
    {
        foreach (Transform child in transform)
        {
            //MeshFilter mf = child.GetComponent<MeshFilter>();
            //if (mf != null)
            //{
                EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
                if (sm != null)
                {
                    if(sm.m_nID <= nLevel)
                    {
                        sm.m_Visible = true;
                    }
                    else
                    {
                        sm.m_Visible = false;
                    }
                    Debug.logger.Log("ShowEvacCircle");
                    
                }
           // }
        }

        SaveSharedFile("ShowEvacCircle");
    }

    public void SetInitEvacDistance(int nDistance)
    {       
        foreach (Transform child in transform)
        {
            //MeshFilter mf = child.GetComponent<MeshFilter>();
            //if (mf != null)
            {
                EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
                if (sm != null)
                {
                    if( sm.m_nID == 1)
                    {
                        sm.SetRaious(nDistance);
                        SaveSharedFile("SetInitEvacDistance");
                        Debug.logger.Log("SetInitEvacDistance");
                        break;  
                    }
                }
            }
        }       
    }

    public void SetSecondEvacDistance(int nDistance)
    {
        foreach (Transform child in transform)
        {
           // MeshFilter mf = child.GetComponent<MeshFilter>();
           // if (mf != null)
            {
                EvacSphere sm = child.gameObject.GetComponent<EvacSphere>();
                if (sm != null)
                {
                    if (sm.m_nID == 2)
                    {
                        sm.SetRaious(nDistance);
                        SaveSharedFile("SetSecondEvacDistance");
                        Debug.logger.Log("SetSecondEvacDistance");
                        break;
                    }
                }
            }
        }  
    }

    public void SetEditMode(bool bEditMode)
    {
        m_bEditMode = bEditMode;
    }


    public void MakeCommandMap()
    {
        for(int i = 0 ; i < szKeyName.Length ; i++)
        {
            string szKey = szKeyName[i];
            mSharedCmdMap.Add(szKey, i + 10);
        }
    }

    private int GetMemIdx(string szCmd)
    {
        if(mSharedCmdMap.ContainsKey(szCmd))
        {
            return mSharedCmdMap[szCmd];
        }
        return -1;
    }

    public void SaveSharedFile(string szKey, int bSet, int nID, bool bbb)
    {   
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = bSet;
            info.nID = nID;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }

    public void SaveSharedFile(string szKey)
    {
        int nIdx = GetMemIdx(szKey);
        if( nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }        
    }
    public void SaveSharedFile(string szKey, Vector3 vec)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.x = vec.x;
            info.y = vec.y;
            info.z = vec.z;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }
    
    public void SaveSharedFile(string szKey, float x, float y, float z)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {
            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.x = x;
            info.y = y;
            info.z = z;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }
    public void SaveSharedFile(string szKey, int x, int y)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {

            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.nx = x;
            info.ny = y;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }
    public void SaveSharedFile(string szKey, int bSelect)
    {
        int nIdx = GetMemIdx(szKey);
        if (nIdx > -1)
        {

            INFO_POI info = new INFO_POI();
            info.bSet = 1;
            info.bSelect = bSelect;
            if (file != null)
            {
                file.Write<INFO_POI>(ref info, nIdx);
            }
        }
    }


    void SaveScreenShot(string szPath)
    {
        Application.CaptureScreenshot(szPath);
        SaveSharedFile("SaveScreenShot");     
    }

    void Get2DPosition( int nTag, float x, float y, float z)
    {
		Vector3 m3DPosition = new Vector3(x, y, z);
        Vector3 m2DPosition = Camera.main.WorldToScreenPoint(m3DPosition);

        SaveSharedFile("Get2DPosition", (int)m2DPosition.x, (int)m2DPosition.y);

        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = string.Format("SendMessage('2DPosition({0}, {1}, {2})')", nTag, (int)m2DPosition.x, (int)m2DPosition.y);
            proxy.RunPythonScript(szMsg);
        }
    }
		
    private void SetPickMode(bool bFalse)
    {
        m_bPickMode = bFalse;
    }

    private bool HitTestChild(Transform node, SortedList arHitPt)
    {
        foreach (Transform child in node.transform)
        {
            if(HitTestChild(child, arHitPt))
            {
                return true;
            }

            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    // Hit된경우 거리값을 키로 저장한다.
                    arHitPt.Add(hit1.distance, hit1);
                    return true;
                }
            }
        }
        return false;             
    }

    public void Get3DPosition(int x, int y)
    {
         PythonProxy proxy = PythonProxy.Instance;
         if (proxy != null)
         {            
             bool bHit = false;
             SortedList arHitPt = new SortedList();

             // Hit가되는 Mesh를 모두 찾는다.
             foreach (Transform child in transform)
             {
                 // 모든 gameObject의 MeshFilter를 검사
                 MeshFilter mf = child.GetComponent<MeshFilter>();
                 if (mf != null)
                 {
                     // MeshCollider를 이용하여 HitTest를 수행
                     MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                     RaycastHit hit1;
                     Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                     if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                     {
                         // Hit된경우 거리값을 키로 저장한다.
                         arHitPt.Add(hit1.distance, hit1);                        
                         bHit = true;
                     }
                 }

                 if(HitTestChild(child, arHitPt))
                 {
                     bHit = true;
                 }
             }
             
             // Hit된 Mesh가 있다면
             if( bHit == true)             
             {
                 // 가장 가까운 것을 선택한다.
                 RaycastHit vec = (RaycastHit)(arHitPt.GetValueList()[0]);
                 m_PickPos = vec.point;
                 
                 SaveSharedFile("Get3DPosition",m_PickPos);
                 
                 string szMsg = string.Format("SendMessage('3DPosition({0}, {1}, {2})')", m_PickPos.x, m_PickPos.y, m_PickPos.z);
                 proxy.RunPythonScript(szMsg);
                 return;
             }

             // Mesh Hit가 되지 않았다면 전체 바운드로 사용하는 BoxCollider로 HitTest를 수행한다.
             RaycastHit hit2;
             Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
             if (coll.Raycast(ray2, out hit2, Mathf.Infinity))
             {
                 m_PickPos = hit2.point;
                 SaveSharedFile("Get3DPosition", m_PickPos);
                 string szMsg = string.Format("SendMessage('3DPosition({0}, {1}, {2})')", m_PickPos.x, m_PickPos.y, m_PickPos.z); 
                 proxy.RunPythonScript(szMsg);
             }
         }
    }


    private bool HitTestChild2D(Transform node, SortedList arHitPt)
    {
        foreach (Transform child in node.transform)
        {
            if (HitTestChild(child, arHitPt))
            {
                return true;
            }

            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    // Hit된경우 거리값을 키로 저장한다.
                    arHitPt.Add(hit1.distance, hit1);
                    return true;
                }
            }
        }
        return false;
    }

    private bool MeshHitChild(Transform node, Vector3 org, Vector3 dir)
    {
        foreach (Transform child in node)
        {
            if (MeshHitChild(child, org, dir))
            {
                return true;
            }

            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
                Ray ray1 = new Ray(org, dir);
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    // Hit된경우 거리값을 키로 저장한다.
                    if (hit1.distance < dir.magnitude)
                    {
                        return true;
                    }                   
                }
            }
        }
        return false;
    }

    public bool MeshHit(Vector3 v1, Vector3 v2)
    {
        bool bHit = false;
        SortedList arHitPt = new SortedList();
        Vector3 dir = v2 - v1;
        // Hit가되는 Mesh를 모두 찾는다.
        foreach (Transform child in transform)
        {
            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
              

                Ray ray1 = new Ray(v1, dir);
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    if( hit1.distance < dir.magnitude)
                    {
                        // Hit된경우 거리값을 키로 저장한다.
                        arHitPt.Add(hit1.distance, hit1);

                        bHit = true;
                    }
                  
                }
            }

            if (MeshHitChild(child, v1, dir))
            {
                bHit = true;
            }
        }
        return bHit;      
    }

    public Vector3 ScreenToGlobal(int x, int y)
    {
        bool bHit = false;
        SortedList arHitPt = new SortedList();

        // Hit가되는 Mesh를 모두 찾는다.
        foreach (Transform child in transform)
        {
            // 모든 gameObject의 MeshFilter를 검사
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                // MeshCollider를 이용하여 HitTest를 수행
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(x, y));
                if (collider.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    // Hit된경우 거리값을 키로 저장한다.
                    arHitPt.Add(hit1.distance, hit1);
                    bHit = true;
                }
            }

            if(HitTestChild2D(child, arHitPt))
            {
                bHit = true;
            }
        }

        // Hit된 Mesh가 있다면
        if (bHit == true)
        {
            // 가장 가까운 것을 선택한다.
            RaycastHit vec = (RaycastHit)(arHitPt.GetValueList()[0]);
            return vec.point;
        }

        // Mesh Hit가 되지 않았다면 전체 바운드로 사용하는 BoxCollider로 HitTest를 수행한다.
        RaycastHit hit2;
        Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(x, y));
        if (coll.Raycast(ray2, out hit2, Mathf.Infinity))
        {
            return hit2.point;
        }

        return new Vector3(float.MinValue, float.MinValue, float.MinValue);
    }

    private void MouseUpPosition(int x, int y, int nMouse)
    {       
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szButton = "Left";   
            if( nMouse == 1)
            {
                szButton = "Right";
            }
            else if(nMouse == 2)
            {
                szButton = "Middle";
            }
            string szMsg = string.Format("SendMessage('OnMouse{0}Up({1}, {2})')", szButton, x, y);

            //Debug.logger.Log(szMsg);
            proxy.RunPythonScript(szMsg); 
        }
    }

    private void MouseDownPosition(int x, int y, int nMouse)
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szButton = "Left";
            if (nMouse == 1)
            {
                szButton = "Right";
            }
            else if (nMouse == 2)
            {
                szButton = "Middle";
            }
            string szMsg = string.Format("SendMessage('OnMouse{0}Down({1}, {2})')", szButton, x, y);

            //Debug.logger.Log(szMsg);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void ReadyToRead()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            string szMsg = "SendMessage('ReadyToRead()')";
            proxy.RunPythonScript(szMsg);
        }
    }

    // public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };
    private void SetMode(int nMode, bool bTrue)
    {
        m_nMode = (MouseWorkMode)nMode;

        switch (m_nMode)
        {
            case MouseWorkMode.NONE: // NONE
                m_bTranslateMode = false;
                m_bOrbitMode = false;
                m_bPickMode = false;
                break;
            case MouseWorkMode.PANNING: // Pan Mode
                m_bTranslateMode = true;
                m_bOrbitMode = false;
                m_bPickMode = false;
                break;
            case MouseWorkMode.ORBIT: // Orbit Mode
                m_bTranslateMode = false;
                m_bOrbitMode = true;
                m_bPickMode = false;
                break;
            case MouseWorkMode.PICK: // Pick Mode
                m_bPickMode = true;
                m_bTranslateMode = false;
                m_bOrbitMode = false;
                break;
            default:
                m_bTranslateMode = false;
                m_bOrbitMode = false;
                m_bPickMode = false;
                break;
        }        
    }

    public void AllClearBuildings()
    {
        ClearSelectChild(transform);
    }

    private void ClearSelectChild(Transform node)
    {
        foreach (Transform child in node)
        {
            ClearSelectChild(child);
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    sm.ClearSelect();
                }
            }
        }
    }
    private void ClearSelect()
    {
        ClearSelectChild(transform);

        HideAllEmpollPolygon();
    }

    public void ShowBuildingText(bool bShow)
    {
        ShowBuildingTextChild(transform, bShow);
    }

    public void ShowBuildingTextChild(Transform node, bool bShow)
    {
        foreach (Transform child in node)
        {
            ShowBuildingTextChild(child, bShow);

            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    sm.ShowText = bShow;
                }
            }
        }
    }


    public void UpdateAliasNameChild(Transform node)
    {
        foreach (Transform child in node)
        {
            UpdateAliasNameChild(child);            
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    Debug.logger.Log("UpdateAliasName : " + mf.name);
                    sm.UpdateAliasName();
                }
                else
                {
                    Debug.logger.Log("UpdateAliasName fail : " + mf.name);
                }
            }
            
        }
    }


    private void UpdateAliasName()
    {
        Debug.logger.Log("Begin UpdateAliasName");

        UpdateAliasNameChild(transform);       

        Debug.logger.Log("End UpdateAliasName");
    }

    public void UpdateAliasColorChild(Transform node)
    {
        foreach (Transform child in node)
        {
            UpdateAliasColorChild(child);

            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                if (child.gameObject.name.StartsWith("Evac"))
                    continue;

                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();
                if (sm != null)
                {
                    sm.SetTextColor(ModelManager.Instance.BuildingNameColor);
                }
            }
        }  
    }

    public void UpdateAliasColor()
    {
        UpdateAliasColorChild(transform);
    }

    public void AwakeChild(Transform node)
    {
        foreach (Transform child in node)
        {
            AwakeChild(child);

            child.gameObject.AddComponent<SelectionModel>();
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {

                Mesh mesh = mf.mesh;
                if( !mesh.name.StartsWith("z"))
                {
                    
                   
                }
                
                child.gameObject.AddComponent<MeshCollider>();
                MeshCollider collider = child.gameObject.GetComponent<MeshCollider>();
                collider.sharedMesh = mf.sharedMesh;
                collider.convex = false;
                SelectionModel sm = child.gameObject.GetComponent<SelectionModel>();

                sm.meshCollider = collider;
              
            }
        }
    }

    private void Awake()
    {

        MakeCommandMap();

        
        ModelManager.Instance.Model = this;
        Light[] lights = Light.GetLights(LightType.Directional, 0);
        Light light = lights[0];

        Vector3 dir = gameObject.transform.position - light.transform.position;
        
        
        //exObject.transform.Translate(new Vector3(0.0f, 0.3f, 0.0f));
        
        //MeshFilter meshCombine = testobject.AddComponent<MeshFilter>();


        //MeshFilter[] meshFilters = testobject.GetComponentsInChildren<MeshFilter>(true);
        //Debug.logger.Log("개수 : " + meshFilters.Length);

        //CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        //int i = 0;
        //while (i < meshFilters.Length)
        //{
        //    Debug.logger.Log(meshFilters[i].name);
        //    combine[i].mesh = meshFilters[i].mesh;
        //    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //    meshFilters[i].gameObject.SetActive(false);
        //    i++;
        //}
        //testobject.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //testobject.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        

        //MeshRenderer renderer = new MeshRenderer();
        //testobject.AddComponent<MeshRenderer>();
        //testobject.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("201605010130.png") as Texture;
        //testobject.SetActive(true);
        //testobject.transform.Translate(0.0f, 0.3f, 0.0f);

        

        //meshCombine.tramsform.Translate(0, 2, 0);


        AwakeChild(transform);

        AddPythonFunction();


        try
        {
            file = new SharedMemory.BufferReadWrite(name: "UnitySamOutsidePoiInfo");
        }
        catch(Exception)
        { }

       

        HideAllEmpollPolygon();

        ReadyToRead();

    }
   
    void Start()
    {
        m_MainCamera = GetComponent<Camera>();
        coll = GetComponent<Collider>();
        coll.isTrigger = true;
        m_ScreenCenter = coll.bounds.center;

        //polutionDic
        string fmt = "0000";

        for (int i = 0; i < 60; i++)
        {
            pollutionDic.Add(i, i.ToString(fmt));
        }

        //ShowPollution(6, 2);
    }


    private void polutionView(int viewOffset)
    {
        string textureName = windStrengthStr + pollutionDirection + "/" + windStrengthStr + pollutionDirection + "_" + pollutionDic[viewOffset];   //X0_0000

        //Texture texture = Resources.Load("201605010240") as Texture;
        Texture texture = Resources.Load(textureName) as Texture;

        GameObject backObject = transform.Find("background").gameObject;
       
        backObject.GetComponent<Projector>().material.SetTexture("_ShadowTex", texture);
        string szMsg = string.Format("PolutionProcessTime(\"{0}\")", viewOffset);
        //Debug.logger.Log(szMsg);
        if (PassivePipeProxy.Instance != null)
            PassivePipeProxy.Instance.SendServer(szMsg);
    }

    private float TimeLeft = 1.0f;
    private float nextTime = 0.0f;
    private int currentViewOffset = 0;
    
    void Update()
    {       
        if (isPollutionProcessView)
        {
            if (Time.time > nextTime)
            {
                nextTime = Time.time + TimeLeft;

                if (currentViewOffset >= pollutionDic.Count - 1)
                {
                    HidePollution(); 
                    return;
                }
                polutionView(currentViewOffset);
                currentViewOffset++;
            }
        }
        
        // Wheel button click
        if (Input.GetMouseButtonDown(2) && !(Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y == Screen.height - 1))
        {
            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);
            MouseDownPosition((int)x, (int)y, 2);

            RaycastHit hit1;
            Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
            {
                m_ScreenCenter = hit1.point;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (coll.Raycast(ray, out hit, Mathf.Infinity))
            {
                m_bPanning = true;
                m_bZoominng = true;
                m_bDragged = true;

                mTranslateStartPt = hit.point;
                mZoomCenter = hit.point;
                mOrbitCenter = hit.point;

                m_MousePosStart = Input.mousePosition;
                m_MousePosPrev = Input.mousePosition;
            }
            else
            {
                m_bPanning = false;
                m_bZoominng = false;
                m_bDragged = false;
            }
        }

        if( m_bPickMode == false)
        { 
            // Mouse Work Mode
            if (Input.GetMouseButtonDown(0))
            {
                float x = Input.mousePosition.x;
                float y = (Screen.height - Input.mousePosition.y);
                MouseDownPosition((int)x, (int)y, 0);
                
                RaycastHit hit;
                Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
                if (coll.Raycast(ray2, out hit, Mathf.Infinity))
                {
                    m_ScreenCenter = hit.point;
                }

                RaycastHit hit1;
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
                {
                    mTranslateStartPt = hit1.point;
                    mZoomCenter = hit1.point;
                    mOrbitCenter = hit1.point;

                    m_MousePosStart = Input.mousePosition;
                    m_MousePosPrev = Input.mousePosition;

                    m_bPanning = false;
                    m_bZoominng = false;
                    m_bDragged = true;
                }
                else
                {
                    m_bPanning = false;
                    m_bZoominng = false;
                    m_bDragged = false;
                }                
            }            
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_bDragged = false;
                Get3DPosition((int)Input.mousePosition.x, (int)Input.mousePosition.y);
            }
        }

        // DragMode
        if (m_bDragged == true)
        {
            m_MousePosCur = Input.mousePosition;
            if (Input.GetMouseButton(0))
            {                
                if (m_bOrbitMode == true)
                {
                    UpdateRotation();
                }
                else if (m_bTranslateMode == true)
                {
                    UpdateTranslate();
                }
            }
            else if (Input.GetMouseButton(2))
            {
                if (m_bPanning == true)
                {
                    UpdateTranslate();
                }
                //if (m_bZoominng == true)
                //{
                //    float zDelta = Input.GetAxis("Mouse ScrollWheel");
                //    UpdateZoom(zDelta);
                //}
            }
            m_MousePosPrev = m_MousePosCur;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1)
            {

            }
            else
            {
                float zDelta = Input.GetAxis("Mouse ScrollWheel");
                UpdateZoom(zDelta);
            }
            m_MousePosPrev = m_MousePosCur;
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
        {
            m_bDragged = false;

            m_bPanning = false;
            m_bZoominng = false;

            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);

            if (Input.GetMouseButtonUp(2))
                MouseUpPosition((int)x, (int)y, 2);
            else
                MouseUpPosition((int)x, (int)y, 0);
        }


        if (Input.GetMouseButtonUp(1))
        {
            m_bDragged = false;
            m_bPanning = false;
            m_bZoominng = false;

            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);
            MouseUpPosition((int)x, (int)y, 1);
        }
        if (Input.GetMouseButtonDown(1))
        {
            m_bDragged = false;
            m_bPanning = false;
            m_bZoominng = false;

            float x = Input.mousePosition.x;
            float y = (Screen.height - Input.mousePosition.y);
            MouseDownPosition((int)x, (int)y, 1);
        }
    }

    private Vector3 m_PickPos = new Vector3();

    private void UpdateTranslate()
    {
        if (m_MousePosCur == m_MousePosPrev)
            return;

        RaycastHit hit1;
        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
        {
            mTranslatePt = hit1.point;
            if(mTranslateStartPt != Vector3.zero)
            {
                Vector3 move = mTranslateStartPt - mTranslatePt;

                if( move.magnitude > 50.0f)
                {
                    move = move.normalized * 50.0f;
                }

                if(!MeshHit(Camera.main.transform.position, Camera.main.transform.position + move))
                {
                    Vector3 camPos = Camera.main.transform.position + move;
                    if (camPos.y < 1.0f)
                        camPos.y = 1.0f;

                    Camera.main.transform.position = camPos;
                }                
            }
        }
    }

    private void ManualZoom(float zDelta)
    {
        if (zDelta > 0.0f)
            zDelta = 0.1f;
        else if (zDelta < 0.0f)
            zDelta = -0.1f;

        RaycastHit hit1;
        Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
        if (coll.Raycast(ray1, out hit1, Mathf.Infinity))
        {
            Vector3 ptScreenCenter = hit1.point;
            Debug.logger.Log("1Zoom Deleta :" + zDelta);
            if (zDelta != 0)
            {

                Debug.logger.Log("2Zoom Deleta :" + zDelta);

                float distance = -(float)((zDelta) * m_fZoomDistance);
                Vector3 vOrgPos = Camera.main.transform.position;
                Vector3 zCenter = new Vector3(ptScreenCenter.x, ptScreenCenter.y, ptScreenCenter.z);

                Vector3 dirCam = (zCenter - vOrgPos).normalized;
                Vector3 dir = dirCam * distance;
                Vector3 vPos = vOrgPos - dir;
                if (vPos.y < 1.0f)
                    vPos.y = 1.0f;
                if (!MeshHit(Camera.main.transform.position, vPos))
                {
                    Camera.main.transform.position = vPos;
                }
            }
        }
    }
  
    private void UpdateZoom(float zDelta)
    {
        if (zDelta != 0)
        {
            float distance = -(float)((zDelta) * m_fZoomDistance);
            Vector3 vOrgPos = Camera.main.transform.position;
            Vector3 zCenter = new Vector3(m_ScreenCenter.x, m_ScreenCenter.y, m_ScreenCenter.z);

            Vector3 dirCam = (zCenter - vOrgPos).normalized;
            Vector3 dir = dirCam * distance;
            Vector3 vPos = vOrgPos - dir;
            if (vPos.y < 1.0f)
                vPos.y = 1.0f;
            if (!MeshHit(Camera.main.transform.position, vPos))
            {
                Camera.main.transform.position = vPos;
            }
        }
    }


    private float yAngle = 0.0f;
    private float xAngle = 0.0f;
    private bool bRotateFirst = true;

    private void UpdateRotation()
    {
        Vector3 yUnit = new Vector3(0f, -1f, 0f);
        if(bRotateFirst == true)
        {
            bRotateFirst = false;
            Vector3 vX = new Vector3(1.0f, 0.0f, 0.0f);
            Camera.main.transform.localRotation.ToAngleAxis(out xAngle, out yUnit);
            Camera.main.transform.localRotation.ToAngleAxis(out yAngle, out vX);

        }

        Vector3 PtDiff = m_MousePosCur - m_MousePosPrev;       
        Vector3 rCenter = new Vector3(m_ScreenCenter.x, m_ScreenCenter.y, m_ScreenCenter.z);
        if (m_MousePosCur == m_MousePosPrev)
            return;

        float pitch = (-0.5f * PtDiff.y);
        float yaw = (-0.5f * PtDiff.x);

        xAngle += yaw;
        yAngle += pitch;

        if( yAngle >= 80.0f)
        {
            if( pitch > 0)
                pitch = 0f;
        }

        if (yAngle <= 5.0f)
        {
            if( pitch < 0)
             pitch = 0f;
        }

        yAngle = Mathf.Clamp(yAngle, 0, 80f);
        xAngle = Mathf.Clamp(xAngle, 0, 360f);

        Quaternion rot1 = Quaternion.AngleAxis(yaw, yUnit);
        Quaternion rot2 = Quaternion.AngleAxis(pitch, Camera.main.transform.right);
        Quaternion q1 = (rot1 * rot2);

        Vector3 MposToCam = q1 * (Camera.main.transform.position - rCenter);
        //Camera.main.transform.position = (MposToCam + rCenter);
        Vector3 vPos = (MposToCam + rCenter);
        if (vPos.y < 1.0f)
            vPos.y = 1.0f;


        Vector3 cCalc = m_ScreenCenter;
        if (!MeshHit(Camera.main.transform.position, vPos))
        {

            Camera.main.transform.position = vPos;
            if (m_ScreenCenter != Vector3.zero)
            {
                Camera.main.transform.LookAt(m_ScreenCenter);
            }
            else
            {
                Camera.main.transform.LookAt(rCenter);
                cCalc = rCenter;
            }
            xAngle += yaw;
            yAngle += pitch;

            Vector3 pos1 = Camera.main.transform.position;
            Vector3 pos2 = Camera.main.transform.position;
            pos1.y = 0;

            Vector3 pos3 = cCalc;
            pos3.y = 0;
            Vector3 vLen = pos1 - pos3;
            float fLength1 = vLen.magnitude;
            float fLength2 = (pos2 - pos3).magnitude;

            double dValue = Math.Acos(fLength1 / fLength2);
            yAngle = (float)(dValue * 180 / Math.PI);
        }


    }

    
    //private void OnMouseOver()
    //{
        
        
    //}

    //private void OnMouseUp()
    //{ 
    //}

    //void OnMouseDown()
    //{
    //    int i = 0;
    //    i++;
    //}

    //private void OnMouseClick()
    //{
    //}


}
