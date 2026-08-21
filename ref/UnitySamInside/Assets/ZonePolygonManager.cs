using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SDMS;

public class ZonePolygonManager : MonoBehaviour 
{
    private Dictionary<string, ZonePolygon> mOutterPolyList = new Dictionary<string, ZonePolygon>();

    private Dictionary<string, EquipmentZonePolygon> mEquipPolyList = new Dictionary<string, EquipmentZonePolygon>();

    private static ZonePolygonManager m_Instance = null;
    public static ZonePolygonManager Instance
    {
        get { return m_Instance; }   
    }



    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            proxy.UserObject.SetVariable("ShowOutZoneVolume", new Action<string>(ShowZoneVolume));
            proxy.UserObject.SetVariable("HideOutZonevolume", new Action<string>(HideZonevolume));
            proxy.UserObject.SetVariable("HideAllOutZoneVolume", new Action(HideAllZoneVolume));

            proxy.UserObject.SetVariable("ShowEquipZoneVolume", new Action<string>(ShowEquipZoneVolume));
            proxy.UserObject.SetVariable("HideEquipZonevolume", new Action<string>(HideEquipZonevolume));
            proxy.UserObject.SetVariable("HideAllEquipZoneVolume", new Action(HideAllEquipZoneVolume));
        }
    }

    void Awake()
    {
        m_Instance = this;

        AddPythonFunction();

        //CreateOutterZoneMesh();

        CreateEquipmentZoneMesh();
    }

	void Start () 
    {
        	
	}


    private void CreateEquipmentZoneMesh()
    {
        List<EquipmentZone> arOutterZone = new List<EquipmentZone>(ZoneManager.Instance.DicEquipZones.Values);

        foreach (EquipmentZone zone in arOutterZone)
        {
            Triangulator tr = zone.Polygon;

            Zone orgZone = zone.LinkedZone;
            if (orgZone == null)
                continue;

            //if (!zone.ZoneName.StartsWith("경영지원처"))
            //    continue;

            // Get Zone Bound
            // Get Bound Center


            Vector2 vCenter = orgZone.Polygon.CalcWeightCenter();
            //Vector2 vCenter = tr.CalcWeightCenter();

            int[] indices = tr.Triangulate();
            Vector3[] vertices = new Vector3[tr.VertextLength()];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(tr.Points[i].x - vCenter.x, 0.5f, tr.Points[i].y - vCenter.y);
                //vertices[i] = new Vector3(tr.Points[i].x , 0.5f, tr.Points[i].y);
            }

            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            msh.name = zone.ZoneName + "_equipzoneMesh";


            string szObjName = zone.ZoneName + "_equipZone";
            GameObject newGameObj = new GameObject(szObjName);
            //newGameObj.transform.parent = gameObject.transform;

            EquipmentZonePolygon poly = newGameObj.AddComponent<EquipmentZonePolygon>();
            poly.m_bVisible = false;
            poly.Name = zone.ZoneName;
            MeshRenderer rend = newGameObj.AddComponent<MeshRenderer>();
            MeshFilter filter = newGameObj.AddComponent<MeshFilter>();
            filter.name = szObjName;
            filter.mesh = msh;

            rend.material = ModelManager.Instance.HighlightMaterial;
            rend.enabled = false;
            if (!mEquipPolyList.ContainsKey(szObjName))
            {
                mEquipPolyList.Add(szObjName, poly);
            }
            else
            {
                Debug.logger.Log(zone.ZoneName);
            }
        }

    }

   
    private void CreateOutterZoneMesh()
    {
        List<Zone> arOutterZone = new List<Zone>(ZoneManager.Instance.DicOutdoorZones.Values);

        foreach(Zone zone in arOutterZone)
        {
            // Use the triangulator to get indices for creating triangles
            Triangulator tr = zone.Polygon;
            
            int[] indices = tr.Triangulate();
            Vector3[] vertices = new Vector3[tr.VertextLength()];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(tr.Points[i].x, 0.5f, tr.Points[i].y);
            }

            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();
            
            msh.name = zone.ZoneName + "_outter_zoneMesh";

            GameObject newGameObj = new GameObject(zone.ZoneName + "_outter_zone");
            newGameObj.transform.parent = gameObject.transform;

            ZonePolygon poly = newGameObj.AddComponent<ZonePolygon>();
            poly.Name = zone.ZoneName;
            MeshRenderer rend  = newGameObj.AddComponent<MeshRenderer>();
            
            MeshFilter filter = newGameObj.AddComponent<MeshFilter>();
            filter.name = zone.ZoneName + "_outterZone";
            filter.mesh = msh;

            rend.material = ModelManager.Instance.HighlightMaterial;
            rend.enabled = false;

            if (!mOutterPolyList.ContainsKey(zone.ZoneName))
            {
                mOutterPolyList.Add(zone.ZoneName, poly);
            }
            else
            {
                Debug.logger.Log(zone.ZoneName);
            }
            

        }
        
    }

    void ShowEquipZoneVolume(string szKey)
    {
        //string szKey = szName + "_equipZone";
        if (mEquipPolyList.ContainsKey(szKey))
        {
            mEquipPolyList[szKey].SetVisible(true);

            Camera.main.Render();
        }
        else
        {
            Debug.logger.Log("ShowFail : " + szKey);
        }
    }

    void HideEquipZonevolume(string szKey)
    {
        //string szKey = szName + "_equipZone";
        if (mEquipPolyList.ContainsKey(szKey))
        {
            mEquipPolyList[szKey].SetVisible(false);

            Camera.main.Render();
        }
        else
        {
            Debug.logger.Log("HideFail : " + szKey);
        }
    }

    void HideAllEquipZoneVolume()
    {
        foreach (KeyValuePair<string, EquipmentZonePolygon> pair in mEquipPolyList)
        {
            pair.Value.SetVisible(false);
        }
        Camera.main.Render();
    }

    void ShowZoneVolume(string szName)
    {
        if(mOutterPolyList.ContainsKey(szName))
        {
            mOutterPolyList[szName].SetVisible(true);
        }

        Camera.main.Render();
    }

    void HideZonevolume(string szName)
    {
        if (mOutterPolyList.ContainsKey(szName))
        {
            mOutterPolyList[szName].SetVisible(false);
        }

        Camera.main.Render();
    }

    void HideAllZoneVolume()
    {
        foreach(KeyValuePair<string,ZonePolygon> pair in mOutterPolyList)
        {
            pair.Value.SetVisible(false);
        }

        Camera.main.Render();
    }
	
	void Update ()
    {
	
	}
}
