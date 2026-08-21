using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            MakeHotel();
            MakeRetail();
            MakeOfficeA();
            MakeOfficeB();
        }

        public static WebDBManager GetDBManager()
        {
            if (m_dbMgr == null)
            {
                int nSiteID;

                if (ReadConfig("siteid", out nSiteID) == false)
                    nSiteID = 201;

                m_dbMgr = new WebDBManager(nSiteID);
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

        private void MakeHotel()
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
        }

        public static Building GetBuilding(string strLocation)
        {
            string strBuildingName = GetBuildingName(strLocation);

            if (strBuildingName == null)
                return null;

            GetDBManager();

            string strSQL = "Select ID, MaxFloor, MinFloor from Building where BuildingName = '" + strBuildingName + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 3)
                return null;
            
            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> maxFloor = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> minFloor = WebDBManager.GetIntField(arrResult[2].ToString());

            if (id == null || maxFloor == null || minFloor == null)
                return null;

            Building building = new Building();

            building.ID = id.Data;
            building.BuildingName = strBuildingName;
            building.MaxFloor = maxFloor.Data;
            building.MinFloor = minFloor.Data;

            return building;
        }

        public static bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private static string GetBuildingName(string strLocation)
        {
            strLocation = strLocation.ToLower();

            if (strLocation.Contains("호텔") || strLocation.Contains("hotel"))
                return "호텔";
            else if (strLocation.Contains("백화점") || strLocation.Contains("리테일") || strLocation.Contains("retail"))
                return "리테일";
            else if (strLocation.Contains("오피스") || strLocation.Contains("타워") || strLocation.Contains("office") || strLocation.Contains("tower"))
            {
                if (strLocation.Contains("a"))
                    return "타워1";
                else if (strLocation.Contains("b"))
                    return "타워2";

                int nLen = strLocation.Length;

                for (int i = 0; i < nLen; i++)
                {
                    char ch = strLocation.ElementAt(i);

                    if (ch == '0')
                        continue;
                    else if (ch == '1')
                        return "타워1";
                    else if (ch == '2')
                        return "타워2";
                }
            }

            return null;
        }
    }
}
