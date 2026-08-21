using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DXFUtility
{
    public class BuildingZoneMaker
    {
        private WebDBManager m_dbMgr = new WebDBManager();
        private string m_strFolderPath = "";

        public BuildingZoneMaker(string strFolderPath, WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            m_strFolderPath = strFolderPath;
        }

        private string FloorString(float fFloorIndex)
        {
            string strResult = "";

            if (fFloorIndex < 0)
                strResult = string.Format(" 지하 {0:f1}층", -fFloorIndex);
            else
                strResult = string.Format(" {0:f1}층", fFloorIndex + 1);

            if (strResult.EndsWith(".0층"))
                return strResult.Substring(0, strResult.Length - 3) + "층";

            return strResult;
        }

        private string AddFloorString(float fFloorIndex)
        {
            int nFloorindex = (int)fFloorIndex;
            float fData = fFloorIndex - nFloorindex;

            if (fData == 0.0f)
                return "NULL";

            if (fData < 0.05f)
                return "NULL";

            return string.Format("'{0:f1}'", fData);
        }

        public bool Run()
        {
            string strSQL = "select max(id) from Zone";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nZoneID = arrResult.Count == 0 ? 0 : m_dbMgr.GetIntField(arrResult[0].ToString(), 0);

            string strFormat = "Insert into Zone (ID, ZoneName, SiteID, BuildingID, FloorIndex, AddFloor, ";
            strFormat += "Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, ";
            strFormat += "TextCenter, BroadcastName) values ({0}, '{1}', 1, {2}, {3}, {4}, NULL, NULL, NULL, NULL, NULL, NULL, '{5}')";

            int nLen = m_strFolderPath.Length;
            string[] arrFolders = System.IO.Directory.GetDirectories(m_strFolderPath);

            foreach (string strFolderPath in arrFolders)
            {
                string strFolderName = strFolderPath.Substring(nLen + 1);

                int nIndex = strFolderPath.IndexOf('_', nLen + 1);
                string strBuildingID = strFolderPath.Substring(nLen + 1, nIndex - (nLen + 1));

                strSQL = string.Format("select id, BuildingName, BroadCastingText from Building where BuildingID = '{0}'", strBuildingID);
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                if (arrResult.Count < 3)
                    continue;

                int nBuildingID = m_dbMgr.GetIntField(arrResult[0].ToString(), -1);
                string strBuildingName = m_dbMgr.GetStringField(arrResult[1], "");
                string strBroadcast = m_dbMgr.GetStringField(arrResult[2], strBuildingName);

                if (string.Compare(strBroadcast, "null") == 0)
                    strBroadcast = strBuildingName;

                if (strBroadcast.EndsWith("*층"))
                    strBroadcast = strBroadcast.Substring(0, strBroadcast.Length - 3);

                string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);
                ArrayList arrFilePath = new ArrayList();

                foreach (string strFilePath in arrFiles)
                {
                    int nDotIndex = strFilePath.LastIndexOf('.');
                    string strExt = strFilePath.Substring(nDotIndex + 1);

                    if (string.Compare(strExt, "dxf", true) != 0)
                        continue;

                    arrFilePath.Add(new FileName(strFilePath));
                }

                arrFilePath.Sort();
                string strQuery = "";

                foreach (FileName file in arrFilePath)
                {
                    // 이미 DB에 존재하는 Zone인지 검사한다.
                    if (CheckZoneDuplicate(nBuildingID, (int)file.FloorIndex, AddFloorString(file.FloorIndex)))
                    {
                        strSQL = string.Format(strFormat, ++nZoneID, strBuildingName + FloorString(file.FloorIndex),
                            nBuildingID, (int)file.FloorIndex, AddFloorString(file.FloorIndex), strBroadcast + FloorString(file.FloorIndex));

                        if (strQuery.Length == 0)
                            strQuery = strSQL;
                        else
                            strQuery += ";" + strSQL;
                    }
                }

                if (strQuery.Length > 0)
                {
                    if (m_dbMgr.GetResultData(strQuery, 0) == null)
                        return false;
                }
            }

            return true;
        }

        // 이미 DB에 존재하는 Zone인지 검사한다.
        private bool CheckZoneDuplicate(int nBuildingID, int nFloorIndex, string strAddFloor)
        {
            string strSQL = string.Format("Select id from Zone where BuildingID = {0} and FloorIndex = {1} and AddFloor {2}",
                nBuildingID, nFloorIndex, strAddFloor == "NULL" ? "is NULL" : "= " + strAddFloor);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            return arrResult.Count == 0;
        }

        /*public bool Run()
        {
            string strSQL = "select max(id) from Zone";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nZoneID = arrResult.Count == 0 ? 0 : m_dbMgr.GetIntField(arrResult[0].ToString(), 0);

            strSQL = "select id, MaxFloor, MinFloor, BuildingName, BroadCastingText from Building";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strFormat = "Insert into Zone (ID, ZoneName, SiteID, BuildingID, FloorIndex, ";
            strFormat += "Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, ";
            strFormat += "TextCenter, BroadcastName) values ({0}, '{1}', 1, {2}, {3}, NULL, NULL, NULL, NULL, NULL, NULL, '{4}')";

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = m_dbMgr.GetIntField(arrResult[i].ToString(), -1);
                int nMaxFloor = m_dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                int nMinFloor = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                string strBuildingName = m_dbMgr.GetStringField(arrResult[i + 3], "");
                string strBroadcast = m_dbMgr.GetStringField(arrResult[i + 4], strBuildingName);

                if (string.Compare(strBroadcast, "null") == 0)
                    strBroadcast = strBuildingName;

                if (strBroadcast.EndsWith("*층"))
                    strBroadcast = strBroadcast.Substring(0, strBroadcast.Length - 3);

                if (nID < 0)
                    continue;

                string strQuery = "";

                for (int j = nMinFloor; j <= nMaxFloor; j++)
                {
                    strSQL = string.Format(strFormat, ++nZoneID, strBuildingName + FloorString(j),
                        nID, j, strBroadcast + FloorString(j));

                    if (strQuery.Length == 0)
                        strQuery = strSQL;
                    else
                        strQuery += ";" + strSQL;
                }

                if (strQuery.Length > 0)
                {
                    if (m_dbMgr.GetResultData(strQuery, 0) == null)
                        return false;
                }
            }

            return true;
        }*/
    }
}
