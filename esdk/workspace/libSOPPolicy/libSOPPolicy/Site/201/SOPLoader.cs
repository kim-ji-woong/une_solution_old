using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace libSOPPolicy
{
    using Common;

    class SOPLoader
    {
        public bool GetLinkedSOP(string strSOPFullPath, object dbMgr, out int nDisasterID)
        {
            nDisasterID = -1;
            string[] tokens = null;

            if (strSOPFullPath.Contains('/'))
                tokens = strSOPFullPath.Split('/');
            else if (strSOPFullPath.Contains('\\'))
                tokens = strSOPFullPath.Split('\\');
            else
                return false;

            if (tokens == null || tokens.Count() < 3)
                return false;

            string strDisasterCategoryName = tokens[0].Trim();
            string strSubDisasterCategoryName = tokens[1].Trim();
            string strDisasterName = tokens[2].Trim();

            string strSQL = "Select d.ID, v.ID from DisasterCategory as dc, SubDisasterCategory as sdc, Disaster as d, Version as v ";
            strSQL += string.Format("where dc.ID = sdc.DisasterID and sdc.ID = d.SubDisasterID and d.VersionID = v.ID and dc.CategoryName = '{0}' and sdc.SubCategoryName = '{1}' and d.DisasterName = '{2}'", strDisasterCategoryName, strSubDisasterCategoryName, strDisasterName);
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount < 2)
                return false;

            VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[nResultCount - 2].ToString());
            VariousData<int> versionID = WebDBManager.GetIntField(arrResult[nResultCount - 1].ToString());

            if (disasterID == null || versionID == null)
                return false;

            nDisasterID = disasterID.Data;

            return true;
        }

        public int GetDisasterBuilding(int nDisasterID, object dbMgr)
        {
            string strSQL = "Select DisasterID, BuildingID from DisasterOwner where DisasterID = " + nDisasterID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count < 2)
                return -1;

            VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[1].ToString());

            if (buildingID == null)
                return -1;

            return buildingID.Data;
        }

        /*private bool GetSensorZoneInfo(int nSensorZoneID, WebDBManager dbMgr, out int nZoneID, out int nEquipZoneID)
        {
            nZoneID = nEquipZoneID = -1;

            string strSQL = "Select sz.EquipZoneID, ez.LinkedZoneIDList from SensorZone as sz, EquipmentZone as ez where sz.EquipZoneID = ez.ID and sz.ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return false;

            VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[0].ToString());
            string zoneIDs = WebDBManager.GetStringField(arrResult[1]);

            if (equipZoneID == null)
                return false;

            nEquipZoneID = equipZoneID.Data;

            if (zoneIDs != null && zoneIDs.Length > 0)
            {
                string[] ids = zoneIDs.Trim().Split(',');
                int nID;

                foreach (string strID in ids)
                {
                    if (int.TryParse(strID, out nID))
                    {
                        nZoneID = nID;
                        break;
                    }
                }
            }

            return true;
        }

        private string GetLinkedSOPName(WebDBManager dbMgr, int nZoneID)
        {
            string strAllSOP = null;

            if (zone == null)
            {
                return strAllSOP;
            }

            string strSQL = "Select SOPName, LinkedBuildingID, LinkedZoneID from FireSensorSOPLink where SiteID = " + ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i], "");
                string strLinkedBuildingID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strLinkedZoneID = WebDBManager.GetStringField(arrResult[i + 2], "");

                if (strLinkedBuildingID == "null" && strLinkedZoneID == "null")
                    strAllSOP = strSOPName;

                if (strLinkedBuildingID != "null" && zone.Building != null)
                {
                    List<Building> buildings = GetBuildings(strLinkedBuildingID);

                    if (buildings != null)
                    {
                        if (buildings.Contains(zone.Building))
                            return strSOPName;
                    }
                }

                if (strLinkedZoneID != "null")
                {
                    List<Zone> zones = GetZones(strLinkedZoneID);

                    if (zones != null)
                    {
                        if (zones.Contains(zone))
                            return strSOPName;
                    }
                }
            }

            return strAllSOP;
        }*/
    }
}
