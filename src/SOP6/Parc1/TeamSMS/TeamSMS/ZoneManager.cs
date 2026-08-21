using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace TeamSMS
{
    public class ZoneManager
    {
        public static Building GetBuilding(string strLocation)
        {
            string strBuildingName = GetBuildingName(strLocation);

            if (strBuildingName == null)
                return null;

            WebDBManager dbMgr = new WebDBManager(201);

            string strSQL = "Select ID, MaxFloor, MinFloor from Building where BuildingName = '" + strBuildingName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
