using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Data.SqlClient;
using HSMS;


namespace HSMSServer2
{
    public class EditEquip : EditData
    {
        /*public static byte[] ProcessChangeEquip(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            // Update
            if (nChangeType == EditData.UPDATE)
            {
            }
            // Delete Equip
            else if (nChangeType == EditData.DELETE)
            {
                int nTargetEquip = (int)arrDatas[2];
                DataEquip equip = dataMgr.GetEquipFromID(nTargetEquip);
                if (equip != null)
                {
                    if (DBEquipHelper.DeleteEquip(dbMgr, equip))
                    {
                        dataMgr.RemoveEquip(equip);

                        equip.SensorDetect = true;
                        equip.ID = -1;
                        equip.Boundary = null;
                        equip.SensorPosition = null;
                        equip.SensorDirVector = null;
                        equip.OriginPosition = null;

                        return bytes;
                    }
                }                
            }

            else if (nChangeType == EditData.INSERT)
            {
                string szEquipName = (string)arrDatas[2];
                string strEquipGroupName = (string)arrDatas[3];
                string szBoundary = (string)arrDatas[4];
                string szSensorPos = (string)arrDatas[5];
                string szSensorFinishPos = (string)arrDatas[6];
                string szSensorDirVector = (string)arrDatas[7];
                string szTextCenter = (string)arrDatas[8];
                int nSiteID = (int)arrDatas[9];

                Dictionary<string, DataEquip> dicEquip = ERPManager.Instance.DicEquips;
                if (dicEquip.ContainsKey(szEquipName))
                {
                    DataEquip equip = dicEquip[szEquipName];
                    if (equip != null)
                    {
                        equip.SiteID = nSiteID;
                        UnE.Geometry.Polygon polygon = dataMgr.GetPolygon(szBoundary);
                        if (polygon != null)
                        {
                            UnE.Geometry.Vertex2D vEquipOrigin = dataMgr.ResetPolygonCoords(polygon);
                            UnE.Geometry.Vertex2D vSensorPos = dataMgr.GetVertex(szSensorPos);
                            UnE.Geometry.Vertex2D vSensorFinishPos = dataMgr.GetVertex(szSensorFinishPos);
                            UnE.Geometry.Vertex2D vSensorDirVector = dataMgr.GetVertex(szSensorDirVector);
                            equip.Boundary = polygon;

                            if (vSensorPos != null)
                                equip.SensorPosition = vSensorPos;

                            if (vSensorFinishPos != null)
                                equip.SensorFinishPosition = vSensorFinishPos;

                            if (vSensorDirVector != null)
                                equip.SensorDirVector = vSensorDirVector;

                            equip.Boundary = polygon;
                            equip.OriginPosition = vEquipOrigin;

                            EquipmentGroup group = dataMgr.FindEquipmentGroup(strEquipGroupName);

                            if (group == null)
                            {
                                if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.GroupName ||
                                    strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                                    group = EquipmentGroup.DefaultEquipmentGroup;
                                else
                                    group = new EquipmentGroup(strEquipGroupName);

                                dataMgr.AddEquipmentGroup(group);
                            }

                            equip.EquipmentGroup = group;
                        }

                        if (DBEquipHelper.AddEquip(dbMgr, equip))
                        {
                            dataMgr.AddEquip(equip);

                            ArrayList arData = new ArrayList();
                            arData.Add((int)ChangeDataType.EQUIP);
                            arData.Add(nChangeType);
                            arData.Add(equip.ID);
                            arData.Add(szEquipName);
                            arData.Add(szBoundary);
                            arData.Add(szSensorPos);
                            arData.Add(szSensorFinishPos);
                            arData.Add(szSensorDirVector);
                            arData.Add(szTextCenter);
                            arData.Add(nSiteID);

                            return ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arData);
                        }
                    }
                }
            }
            return null;
        }*/

        private static int ProcessInsert(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr)
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;

            int nIDIndex = nIndex;

            string szEquipName = (string)arrDatas[nIndex++];
            string strEquipGroupName = (string)arrDatas[nIndex++];
            string szBoundary = (string)arrDatas[nIndex++];
            string szSensorPos = (string)arrDatas[nIndex++];
            string szSensorFinishPos = (string)arrDatas[nIndex++];
            string szSensorDirVector = (string)arrDatas[nIndex++];
            string szTextCenter = (string)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];

            Dictionary<string, DataEquip> dicEquip = ERPManager.Instance.DicEquips;
            if (dicEquip.ContainsKey(szEquipName))
            {
                DataEquip equip = dicEquip[szEquipName];

                if (equip != null)
                {
                    equip.SiteID = nSiteID;
                    UnE.Geometry.Polygon polygon = dataMgr.GetPolygon(szBoundary);
                    if (polygon != null)
                    {
                        UnE.Geometry.Vertex2D vEquipOrigin = dataMgr.ResetPolygonCoords(polygon);
                        UnE.Geometry.Vertex2D vSensorPos = dataMgr.GetVertex(szSensorPos);
                        UnE.Geometry.Vertex2D vSensorFinishPos = dataMgr.GetVertex(szSensorFinishPos);
                        UnE.Geometry.Vertex2D vSensorDirVector = dataMgr.GetVertex(szSensorDirVector);
                        equip.Boundary = polygon;

                        if (vSensorPos != null)
                            equip.SensorPosition = vSensorPos;

                        if (vSensorFinishPos != null)
                            equip.SensorFinishPosition = vSensorFinishPos;

                        if (vSensorDirVector != null)
                            equip.SensorDirVector = vSensorDirVector;

                        equip.Boundary = polygon;
                        equip.OriginPosition = vEquipOrigin;

                        EquipmentGroup group = dataMgr.FindEquipmentGroup(strEquipGroupName);

                        if (group == null)
                        {
                            if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.GroupName ||
                                strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                                group = EquipmentGroup.DefaultEquipmentGroup;
                            else
                                group = new EquipmentGroup(strEquipGroupName);

                            dataMgr.AddEquipmentGroup(group);
                        }

                        equip.EquipmentGroup = group;
                    }

                    if (DBEquipHelper.AddEquip(dbMgr, equip))
                    {
                        arrDatas.Insert(nIDIndex, equip.ID);
                        dataMgr.AddEquip(equip);
                        return nIndex;
                    }
                }
            }

            return -1;
        }

        // Return 값 : arrDatas가 변경되었는가 여부
        public static bool ProcessChangeDataList2(ArrayList arrDatas)
        {
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            bool isChanged = false;
            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i++)
            {
                try
                {
                    int nSqlType = (int)arrDatas[i];

                    if (nSqlType == (int)EditData.UPDATE)
                        i = ProcessUpdate2(arrDatas, i + 1, dbMgr, connection);
                    else if (nSqlType == (int)EditData.DELETE)
                        i = ProcessDelete2(arrDatas, i + 1, dbMgr, connection);
                    else if (nSqlType == (int)EditData.INSERT)
                    {
                        connection.Close();

                        i = ProcessInsert(arrDatas, i + 1, dbMgr);

                        if (i < 0)
                            return false;
                        else
                        {
                            isChanged = true;
                            nDataCount++;
                        }

                        connection = dbMgr.Connect();
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }
            }

            connection.Close();
            return isChanged;
        }

        // Return 값 : Last Index
        private static int ProcessDelete2(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr, SqlConnection connection)
        {
            int nEquipID = (int)arrDatas[nIndex++];
            
            string strSQL = string.Format("Delete from Equipment where ID = {0}", nEquipID);

            dbMgr.ExecuteSQL(strSQL, connection);

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DataEquip equip = dataMgr.GetEquipFromID(nEquipID);

            if (equip != null)
            {
                equip.ID = -1;
                dataMgr.RemoveEquip(equip);
            }

            return nIndex - 1;
        }

        // Return 값 : Last Index
        private static int ProcessUpdate2(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr, SqlConnection connection)
        {
            int nEquipID = (int)arrDatas[nIndex++];
            string strEquipGroupName = (string)arrDatas[nIndex++];

            string strSQL = string.Format("Update Equipment set EquipGroupName = '{0}' where ID = {1}",
                strEquipGroupName, nEquipID);

            dbMgr.ExecuteSQL(strSQL, connection);

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DataEquip equip = dataMgr.GetEquipFromID(nEquipID);

            if (equip != null)
            {
                EquipmentGroup group = dataMgr.FindEquipmentGroup(strEquipGroupName);

                if (group == null)
                {
                    if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.GroupName ||
                        strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                        group = EquipmentGroup.DefaultEquipmentGroup;
                    else
                        group = new EquipmentGroup(strEquipGroupName);

                    dataMgr.AddEquipmentGroup(group);
                }

                equip.EquipmentGroup = group;
            }

            return nIndex - 1;
        }

        // Return 값 : arrDatas가 변경되었는가 여부
        public static bool ProcessChangeDataList(ArrayList arrDatas)
        {
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i++)
            {
                try
                {
                    int nSqlType = (int)arrDatas[i];

                    if (nSqlType == (int)EditData.UPDATE)
                        i = ProcessUpdate(arrDatas, i + 1, dbMgr, connection);
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }
            }

            connection.Close();
            return false;
        }

        private static Dictionary<string, int> GetWorkerToDistanceOptionIDs(DBConn conn, SqlConnection connection, string strItemName)
        {
            string strSQL = "Select id, ItemValue from Options where ItemName = '" + strItemName + "' and SiteID = " + NetworkServer.Instance.SiteID.ToString();
            SqlDataReader reader = conn.ExecuteReader(strSQL, connection);

            Dictionary<string, int> dicOptionID = new Dictionary<string, int>();

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strValue = (string)reader[1];

                int nIndex = strValue.IndexOf('_');

                if (nIndex >= 0)
                {
                    string strZoneGroupName = strValue.Substring(0, nIndex);
                    dicOptionID[strZoneGroupName] = nID;
                }
            }

            reader.Close();

            return dicOptionID;
        }

        // Return 값 : arrDatas가 변경되었는가 여부
        public static bool ProcessChangeAlarmDistance(ArrayList arrDatas)
        {
            float fWorkerToCarDistanceBoth, fWorkerToCarDistanceOneSide/*, fWorkerToZoneDistance, fWorkerToEquipDistance*/;
            float fCoGasTolerance, fMethaneTolerance;
            int nIndex = 1;

            Dictionary<string, float> dicWorkerToZoneDistance = new Dictionary<string, float>();
            Dictionary<string, float> dicWorkerToEquipDistance = new Dictionary<string, float>();

            DBConn conn = NetworkServer.Instance.DBManager;
            SqlConnection connection = conn.Connect();
            
            if (GetAlarmDistance(arrDatas, ref nIndex, out fWorkerToCarDistanceBoth))
            {
                float itemValue = fWorkerToCarDistanceBoth * 1000;
                
                string strSQL = "Update Options Set ItemValue = '" + itemValue.ToString() + "' where ItemName = 'WorkerToCarDistanceBoth' And SiteID = " + NetworkServer.Instance.SiteID.ToString();
                conn.ExecuteSQL(strSQL, connection);

                NetworkServer.Instance.DataManager.WorkerToCarDistanceBoth = fWorkerToCarDistanceBoth;
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, out fWorkerToCarDistanceOneSide))
            {
                float itemValue = fWorkerToCarDistanceOneSide * 1000;

                string strSQL = "Update Options Set ItemValue = '" + itemValue.ToString() + "' where ItemName = 'WorkerToCarDistanceOneSide' And SiteID = " + NetworkServer.Instance.SiteID.ToString();
                conn.ExecuteSQL(strSQL, connection);

                NetworkServer.Instance.DataManager.WorkerToCarDistanceOneSide = fWorkerToCarDistanceOneSide;
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, dicWorkerToZoneDistance))
            {
                Dictionary<string, int> dicOptionID = GetWorkerToDistanceOptionIDs(conn, connection, "WorkerToZoneDistance");
                int nMaxID = -1;

                foreach (KeyValuePair<string, float> pair in dicWorkerToZoneDistance)
                {
                    string itemValue = pair.Key + "_" + (pair.Value * 1000).ToString();

                    if (dicOptionID.ContainsKey(pair.Key))
                    {
                        int nOptionID = dicOptionID[pair.Key];

                        string strSQL = "Update Options Set ItemValue = '" + itemValue + "' where ID = " + nOptionID.ToString();
                        conn.ExecuteSQL(strSQL, connection);
                    }
                    else
                    {
                        if (nMaxID < 0)
                            nMaxID = AlarmManager.GetMaxID("Options", connection) + 1;

                        string strSQL = "Insert into Options (ID, ItemName, ItemValue, SiteID, Description) values ";
                        strSQL += string.Format("({0}, 'WorkerToZoneDistance', '{1}', {2}, NULL)", nMaxID++, itemValue, NetworkServer.Instance.SiteID);
                        conn.ExecuteSQL(strSQL, connection);
                    }

                    NetworkServer.Instance.DataManager.SetWorkerToZoneDistance(pair.Key, pair.Value);
                }
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, dicWorkerToEquipDistance))
            {
                Dictionary<string, int> dicOptionID = GetWorkerToDistanceOptionIDs(conn, connection, "WorkerToEquipDistance");
                int nMaxID = -1;

                foreach (KeyValuePair<string, float> pair in dicWorkerToEquipDistance)
                {
                    string itemValue = pair.Key + "_" + (pair.Value * 1000).ToString();

                    if (dicOptionID.ContainsKey(pair.Key))
                    {
                        int nOptionID = dicOptionID[pair.Key];

                        string strSQL = "Update Options Set ItemValue = '" + itemValue + "' where ID = " + nOptionID.ToString();
                        conn.ExecuteSQL(strSQL, connection);
                    }
                    else
                    {
                        if (nMaxID < 0)
                            nMaxID = AlarmManager.GetMaxID("Options", connection) + 1;

                        string strSQL = "Insert into Options (ID, ItemName, ItemValue, SiteID, Description) values ";
                        strSQL += string.Format("({0}, 'WorkerToEquipDistance', '{1}', {2}, NULL)", nMaxID++, itemValue, NetworkServer.Instance.SiteID);
                        conn.ExecuteSQL(strSQL, connection);
                    }

                    NetworkServer.Instance.DataManager.SetWorkerToEquipDistance(pair.Key, pair.Value);
                }
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, out fCoGasTolerance))
            {
                float itemValue = fCoGasTolerance;

                string strSQL = "Update Options Set ItemValue = '" + itemValue.ToString() + "' where ItemName = 'COGasTolerance' And SiteID = " + NetworkServer.Instance.SiteID.ToString();
                conn.ExecuteSQL(strSQL, connection);

                NetworkServer.Instance.DataManager.COGasTolerance = fCoGasTolerance;
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, out fMethaneTolerance))
            {
                float itemValue = fMethaneTolerance;

                string strSQL = "Update Options Set ItemValue = '" + itemValue.ToString() + "' where ItemName = 'MethaneTolerance' And SiteID = " + NetworkServer.Instance.SiteID.ToString();
                conn.ExecuteSQL(strSQL, connection);

                NetworkServer.Instance.DataManager.MethaneTolerance = fMethaneTolerance;
            }
            /*if (GetAlarmDistance(arrDatas, ref nIndex, out fWorkerToZoneDistance))
            {
                float itemValue = fWorkerToZoneDistance * 1000;

                string strSQL = "Update Options Set ItemValue = " + itemValue.ToString() + " where ItemName = 'WorkerToZoneDistance' And SiteID = " + NetworkServer.Instance.SiteID.ToString();
                conn.ExecuteSQL(strSQL, connection);

                NetworkServer.Instance.DataManager.WorkerToZoneDistance = fWorkerToZoneDistance;
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, out fWorkerToEquipDistance))
            {
                float itemValue = fWorkerToEquipDistance * 1000;

                string strSQL = "Update Options Set ItemValue = '" + itemValue.ToString() + "' where ItemName = 'WorkerToEquipDistance' And SiteID = " + NetworkServer.Instance.SiteID.ToString();
                conn.ExecuteSQL(strSQL, connection);

                NetworkServer.Instance.DataManager.WorkerToEquipDistance = fWorkerToEquipDistance;
            }*/

            connection.Close();
            return false;
        }

        private static bool GetAlarmDistance(ArrayList arrDatas, ref int nIndex, Dictionary<string, float> dicAlarmDistance)
        {
            int nDataCount = arrDatas.Count;

            if (nIndex >= nDataCount)
                return false;

            try
            {
                bool isChanged = (bool)arrDatas[nIndex++];

                if (isChanged)
                {
                    while (nIndex < nDataCount)
                    {
                        if (arrDatas[nIndex].GetType() == typeof(bool))
                            break;

                        string strItemName = (string)arrDatas[nIndex++];
                        float fDistance = (float)arrDatas[nIndex++];
                        dicAlarmDistance[strItemName] = fDistance;
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private static bool GetAlarmDistance(ArrayList arrDatas, ref int nIndex, out float fDistance)
        {
            fDistance = 0.0f;
            int nDataCount = arrDatas.Count;

            if (nIndex >= nDataCount)
                return false;

            try
            {
                bool isChanged = (bool)arrDatas[nIndex++];

                if (isChanged)
                {
                    fDistance = (float)arrDatas[nIndex++];
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        // Return 값 : Last Index
        private static int ProcessUpdate(ArrayList arrDatas, int nIndex, HSMS.DBConn dbMgr, SqlConnection connection)
        {
            int nEquipID = (int)arrDatas[nIndex++];
            string strEquipCode = (string)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];
            bool isDetect = (bool)arrDatas[nIndex];

            string strSQL = string.Format("Update Equipment set EquipCode = '{0}', SiteID = {1}, SensorDetect = {2} where ID = {3}",
                strEquipCode, nSiteID, isDetect ? 1 : 0, nEquipID);

            dbMgr.ExecuteSQL(strSQL, connection);

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DataEquip equip = dataMgr.GetEquipFromID(nEquipID);

            if (equip != null)
            {
                equip.Code = strEquipCode;
                equip.SiteID = nSiteID;
                equip.SensorDetect = isDetect;
            }

            return nIndex;
        }
    }
}
