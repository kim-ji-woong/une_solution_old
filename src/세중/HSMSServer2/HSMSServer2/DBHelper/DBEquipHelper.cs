using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using HSMS;

namespace HSMSServer2
{
    public class DBEquipHelper
    {
        public static bool DeleteEquip(DBConn conn, DataEquip equip)
        {
            if (equip == null)
                return false;
            
            int nSiteID = NetworkServer.Instance.SiteID;

            bool bResult = false;
            try
            {
                string strDeleteSQL = string.Format("Delete from Equipment where ID = {0} and SiteID = {1}", equip.ID, nSiteID);
                bResult = DBHelper.ExecuteSQL(conn, strDeleteSQL);
                if (bResult == true)
                {
                    equip.ID = -1;
                    equip.Boundary = null;
                    equip.SensorPosition = null;
                    equip.SensorFinishPosition = null;
                    equip.SensorDirVector = null;
                    equip.OriginPosition = null;
                    equip.SensorDetect = true;
                }
            }
            catch (System.Exception)
            {
            }            
            return bResult;
        }

        public static bool AddEquip(DBConn conn, DataEquip equip)
        {
            if (equip == null)
                return false;
            
            bool bResult = false;
            try
            {  
                string strBoundary = "";
                string strTextCenter = "";

                string strSensorPos = "";
                string strSensorFinishPos = "";
                string strSensorDirVector = "";

                DataManager dataMgr = NetworkServer.Instance.DataManager;
                Dictionary<string, EquipmentRawData> dicEquipRawData = dataMgr.DicEquipRawDatas;
                if (dicEquipRawData.ContainsKey(equip.Name))
                {
                    int nSiteID = NetworkServer.Instance.SiteID;
                    int nMaxID = -1;                

                    EquipmentRawData equipRawData = dicEquipRawData[equip.Name];
                    strBoundary = equipRawData.Boundary;
                    strTextCenter = equipRawData.TextCenter;
                    strSensorPos = equipRawData.SensorPos;
                    strSensorFinishPos = equipRawData.SensorFinishPos;
                    strSensorDirVector = equipRawData.SensorDirVector;

                    UnE.Geometry.Polygon polygon = dataMgr.GetPolygon(strBoundary);
                    if (polygon != null)
                    { 
                        string strSQL = "insert into Equipment (ID, EquipCode, MeshName, EquipGroupName, Boundary, SensorPos, SensorFinishPos, SensorDirVector, SiteID, TextCenter, SensorDetect) Values(" + DBHelper.MaxID + ",'"
                            + equip.Code + "', '" + equip.Name + "', '" + equip.EquipmentGroup.GroupName + "', '" + strBoundary + "', '" + strSensorPos + "', '" + strSensorFinishPos + "', '" + strSensorDirVector + "', " + nSiteID + ", '" + strTextCenter + "', 1)";

                        bResult = DBHelper.ExecuteSQL(conn, strSQL, "Equipment", ref nMaxID );
                        if (bResult == true)
                        {
                            UnE.Geometry.Vertex2D vEquipOrigin = dataMgr.ResetPolygonCoords(polygon);
                            UnE.Geometry.Vertex2D vSensorPos = dataMgr.GetVertex(strSensorPos);
                            UnE.Geometry.Vertex2D vSensorFinishPos = dataMgr.GetVertex(strSensorFinishPos);
                            UnE.Geometry.Vertex2D vSensorDirVector = dataMgr.GetVertex(strSensorDirVector);

                            equip.Boundary = polygon;

                            if (vSensorPos != null)
                                equip.SensorPosition = vSensorPos;

                            if (vSensorFinishPos != null)
                                equip.SensorFinishPosition = vSensorFinishPos;

                            if (vSensorDirVector != null)
                                equip.SensorDirVector = vSensorDirVector;
                            
                            equip.ID = nMaxID;
                            equip.SiteID = nSiteID;
                            equip.Boundary = polygon;
                            equip.OriginPosition = vEquipOrigin;
                            equip.SensorDetect = true;
                        }
                    }
                }                
            }
            catch (System.Exception)
            {
            }            
            return bResult;
        }
    }
}
