using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using SDMS;

namespace UnE.View.Content
{

    public class ZonePolygonManager
    {

        // 2D용 외곽존 Polygon 
        private SortedList<string, ZonePolygon> mOutterPolyList = new SortedList<string, ZonePolygon>();

        // 2D용 EquipmentZone Polygon 
        private SortedList<string, EquipmentZonePolygon> mEquipPolyList = new SortedList<string, EquipmentZonePolygon>();

        // 2D용 건물 Polygon
        private SortedList<string, ZonePolygon> mBuildingPolyList = new SortedList<string, ZonePolygon>();

        private IZoneManager mZoneManager = null;

        public ZonePolygonManager(IZoneManager zMgr)
        {
            mZoneManager = zMgr;

            CreateEquipmentZonePolygon();
            CreateOutterZonePolygon();
            CreateBuildingZonePolygon();
        }

        private void CreateBuildingZonePolygon()
        {

        }
        
        private void CreateEquipmentZonePolygon()
        {
            //List<EquipmentZone> arOutterZone = new List<EquipmentZone>(mZoneManager.DicEquipZones.Values);

            //foreach (EquipmentZone zone in arOutterZone)
            //{
            //    Triangulator tr = zone.Polygon;

            //    int[] indices = tr.Triangulate();
            //    Vector3[] vertices = new Vector3[tr.VertextLength()];
            //    for (int i = 0; i < vertices.Length; i++)
            //    {
            //        vertices[i] = new Vector3(tr.Points[i].x, 0.5f, tr.Points[i].y);
            //    }

            //    Mesh msh = new Mesh();
            //    msh.vertices = vertices;
            //    msh.triangles = indices;
            //    msh.RecalculateNormals();
            //    msh.RecalculateBounds();

            //    msh.name = zone.ZoneName + "_equipzoneMesh";

            //    GameObject newGameObj = new GameObject(zone.ZoneName + "_equipZone");
            //    newGameObj.transform.parent = gameObject.transform;

            //    EquipmentZonePolygon poly = newGameObj.AddComponent<EquipmentZonePolygon>();
            //    poly.Name = zone.ZoneName;
            //    MeshRenderer rend = newGameObj.AddComponent<MeshRenderer>();

            //    MeshFilter filter = newGameObj.AddComponent<MeshFilter>();
            //    filter.name = zone.ZoneName + "_equipZone";
            //    filter.mesh = msh;

            //    rend.material = ModelManager.Instance.HighlightMaterial;
            //    rend.enabled = false;

            //    if (!mEquipPolyList.ContainsKey(zone.ZoneName))
            //    {
            //        mEquipPolyList.Add(zone.ZoneName, poly);
            //    }
            //    else
            //    {
            //        Debug.logger.Log(zone.ZoneName);
            //    }
            //}

        }


        private void CreateOutterZonePolygon()
        {
            //List<Zone> arOutterZone = new List<Zone>(mZoneManager.DicOutdoorZones.Values);

            //foreach (Zone zone in arOutterZone)
            {
                // Use the triangulator to get indices for creating triangles
                //Triangulator tr = zone.Polygon;

                //int[] indices = tr.Triangulate();
                //Vector3[] vertices = new Vector3[tr.VertextLength()];
                //for (int i = 0; i < vertices.Length; i++)
                //{
                //    vertices[i] = new Vector3(tr.Points[i].x, 0.5f, tr.Points[i].y);
                //}

                //Mesh msh = new Mesh();
                //msh.vertices = vertices;
                //msh.triangles = indices;
                //msh.RecalculateNormals();
                //msh.RecalculateBounds();

                //msh.name = zone.ZoneName + "_outter_zoneMesh";

                //GameObject newGameObj = new GameObject(zone.ZoneName + "_outter_zone");
                //newGameObj.transform.parent = gameObject.transform;

                //ZonePolygon poly = newGameObj.AddComponent<ZonePolygon>();
                //poly.Name = zone.ZoneName;
                //MeshRenderer rend = newGameObj.AddComponent<MeshRenderer>();

                //MeshFilter filter = newGameObj.AddComponent<MeshFilter>();
                //filter.name = zone.ZoneName + "_outterZone";
                //filter.mesh = msh;

                //rend.material = ModelManager.Instance.HighlightMaterial;
                //rend.enabled = false;
                //mOutterPolyList.Add(zone.ZoneName, poly);
            }

        }

        void ShowEquipZoneVolume(string szName)
        {
            if (mOutterPolyList.ContainsKey(szName))
            {
                mEquipPolyList[szName].SetVisible(true);
            }
        }

        void HideEquipZonevolume(string szName)
        {
            if (mEquipPolyList.ContainsKey(szName))
            {
                mEquipPolyList[szName].SetVisible(false);
            }
        }

        void HideAllEquipZoneVolume()
        {
            foreach (KeyValuePair<string, EquipmentZonePolygon> pair in mEquipPolyList)
            {
                pair.Value.SetVisible(false);
            }
        }

        void ShowZoneVolume(string szName)
        {
            if (mOutterPolyList.ContainsKey(szName))
            {
                mOutterPolyList[szName].SetVisible(true);
            }
        }

        void HideZonevolume(string szName)
        {
            if (mOutterPolyList.ContainsKey(szName))
            {
                mOutterPolyList[szName].SetVisible(false);
            }
        }

        void HideAllZoneVolume()
        {
            foreach (KeyValuePair<string, ZonePolygon> pair in mOutterPolyList)
            {
                pair.Value.SetVisible(false);
            }
        }
    }
   
   
    public class ZonePolygon
    {

        internal void SetVisible(bool p)
        {
            //throw new NotImplementedException();
        }
    }

    public class EquipmentZonePolygon
    {

        internal void SetVisible(bool p)
        {
            //throw new NotImplementedException();
        }
    }
}
