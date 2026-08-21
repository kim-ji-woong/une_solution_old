using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadData();

            CalcFireSensorPosition();

            UpdateSensorPosition();
        }

        public static void LoadData()
        {
            ZoneManager.Instance.DBManager.WebServerURL = "http://192.168.0.195:8080/SOP";
            ZoneManager.Instance.DBManager.DatabaseName = "SOP3";
            ZoneManager.Instance.LoadBuildingData();
            ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZone();

            SensorManager.Instance.ReadAllSensorData();
        }

        public static void CalcFireSensorPosition()
        {
            Hashtable table = SensorManager.Instance.DicFireSensor;
            
            foreach (FireSensor fireSensor in table.Values)
            {
                //FireSensor fireSensor = pair.Value;

                EquipmentZone equipZone = fireSensor.EquipZone;
                if (equipZone == null)
                    return;
                UnE.Geometry.Vertex2D vert = equipZone.Polygon.CalcWeightCenter();
                Zone zone = equipZone.LinkedZone;
                UnE.Geometry.Vertex2D vert2 = zone.Polygon.CalcWeightCenter();
                float x = -(float)(vert2.x - vert.x);
                float z = -(float)(vert.y - vert2.y);
               
                fireSensor.X = x;
                fireSensor.Y = 0.5f;
                fireSensor.Z = z;
            } 
        }

        public static void UpdateSensorPosition()
        {
            WebDBManager dbMgr = ZoneManager.Instance.DBManager;
            Hashtable table = SensorManager.Instance.DicFireSensor;
            foreach (FireSensor fireSensor in table.Values)
            {

                string strValue = string.Format("X = {0}, Y = {1}, Z = {2}", fireSensor.X, fireSensor.Y, fireSensor.Z);
                string strSQL = string.Format("Update FireSensor set {0} where id = {1}", strValue, fireSensor.ID);
                if (dbMgr.GetResultData(strSQL, 0) != null)
                {

                }
            }
        }
    }

}
