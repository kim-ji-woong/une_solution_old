using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using DBUtility2;
using System.Collections;

namespace BuildingSMS
{
    public class ZoneManager
    {
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private static WebDBManager m_dbMgr = null;

        public ZoneManager()
        {
            SetBuilding();
            /*MakeHotel();
            MakeRetail();
            MakeOfficeA();
            MakeOfficeB();*/
        }

        public static WebDBManager GetDBManager()
        {
            if (m_dbMgr == null)
            {
                int nSiteID, nDBType;

                if (ReadConfig("siteid", out nSiteID) == false)
                    nSiteID = 205;

                if (ReadConfig("dbType", out nDBType) == false)
                    nDBType = 0;

                string strDBName = ConfigurationManager.AppSettings["dbName"].ToString().Trim();
                string strWebServerURL = ConfigurationManager.AppSettings["webServerURL"].ToString().Trim();

                m_dbMgr = new WebDBManager(strDBName, nSiteID);
                m_dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;
                m_dbMgr.WebServerURL = strWebServerURL;
            }

            return m_dbMgr;
        }

        public int GetBuildingCount()
        {
            return m_dicBuildings.Count;
        }

        public Building GetBuilding(int nBuildingIndex)
        {
            KeyValuePair<int, Building> pair = m_dicBuildings.ElementAt(nBuildingIndex);
            return pair.Value;
        }

        public void SetBuilding()
        {
            WebDBManager dbMgr = GetDBManager();

            string strCondition = "'" + Building.HotelCode + "', '" + Building.RetailCode + "', '" + Building.Tower1Code + "', '" + Building.Tower2Code + "'";
            string strSQL = "Select ID, BuildingCode, BuildingName, MaxFloor, MinFloor from Building where BuildingCode in (" + strCondition + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCode = WebDBManager.GetStringField(arrResult[i + 1]);
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> maxFloor = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> minFloor = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (id == null || strCode == null || strBuildingName == null || maxFloor == null || minFloor == null)
                    continue;

                Building building = new Building();

                building.ID = id.Data;
                building.BuildingCode = strCode;
                building.BuildingName = strBuildingName;
                building.MaxFloor = maxFloor.Data;
                building.MinFloor = minFloor.Data;

                int _maxFloor, _minFloor;
                GetBuildingFloors(strCode, out _maxFloor, out _minFloor);

                building.MaxFloor = _maxFloor;
                building.MinFloor = _minFloor;

                m_dicBuildings[building.ID] = building;
            }
        }

        private static void GetBuildingFloors(string strBuildingCode, out int maxFloor, out int minFloor)
        {
            if (strBuildingCode == Building.HotelCode)
            {
                maxFloor = 26;
                minFloor = -8;
            }
            else if (strBuildingCode == Building.Tower1Code)
            {
                maxFloor = 21;
                minFloor = -8;
            }
            else if (strBuildingCode == Building.Tower2Code)
            {
                maxFloor = 25;
                minFloor = -8;
            }
            else
            {
                maxFloor = 5;
                minFloor = 0;
            }
        }

        /*private void MakeHotel()
        {
            Building building = new Building();
            building.BuildingName = "Hotel";
            building.MinFloor = -6;
            building.MaxFloor = 32;
            building.ID = 1;

            m_dicBuildings[building.ID] = building;
        }

        private void MakeRetail()
        {
            Building building = new Building();
            building.BuildingName = "Retail";
            building.MinFloor = -7;
            building.MaxFloor = 7;
            building.ID = 2;

            m_dicBuildings[building.ID] = building;
        }

        private void MakeOfficeA()
        {
            Building building = new Building();
            building.BuildingName = "OfficeA";
            building.MinFloor = -7;
            building.MaxFloor = 68;
            building.ID = 3;

            m_dicBuildings[building.ID] = building;
        }
        private void MakeOfficeB()
        {
            Building building = new Building();
            building.BuildingName = "OfficeB";
            building.MinFloor = -7;
            building.MaxFloor = 52;
            building.ID = 4;

            m_dicBuildings[building.ID] = building;
        }*/

        public static Building GetBuilding(string strLocation)
        {
            string strBuildingCode = GetBuildingCode(strLocation);

            if (strBuildingCode == null)
                return null;

            GetDBManager();

            string strSQL = "Select ID, BuildingName, MaxFloor, MinFloor from Building where BuildingCode = '" + strBuildingCode + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 4)
                return null;
            
            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            string strBuildingName = WebDBManager.GetStringField(arrResult[1].ToString());
            VariousData<int> maxFloor = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<int> minFloor = WebDBManager.GetIntField(arrResult[3].ToString());

            if (id == null || maxFloor == null || minFloor == null || strBuildingName == null)
                return null;

            Building building = new Building();

            building.ID = id.Data;
            building.BuildingName = strBuildingName;
            building.MaxFloor = maxFloor.Data;
            building.MinFloor = minFloor.Data;
            building.BuildingCode = strBuildingCode;

            int _maxFloor, _minFloor;
            GetBuildingFloors(strBuildingCode, out _maxFloor, out _minFloor);

            building.MaxFloor = _maxFloor;
            building.MinFloor = _minFloor;

            return building;
        }

        public static bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private static string GetBuildingCode(string strLocation)
        {
            strLocation = strLocation.ToLower();

            if (strLocation.Contains("호텔") || strLocation.Contains("hotel"))
                return Building.HotelCode;
            else if (strLocation.Contains("백화점") || strLocation.Contains("리테일") || strLocation.Contains("retail"))
                return Building.RetailCode;
            else if (strLocation.Contains("오피스") || strLocation.Contains("타워") || strLocation.Contains("office") || strLocation.Contains("tower"))
            {
                if (strLocation.StartsWith("u"))
                    return Building.Tower1Code;
                else if (strLocation.StartsWith("t"))
                    return Building.Tower2Code;
            }

            return null;
        }
    }
}
