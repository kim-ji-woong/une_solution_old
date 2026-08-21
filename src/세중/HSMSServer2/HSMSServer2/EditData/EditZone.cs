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
    public class EditZone : EditData
    {
        public static byte[] ProcessChangeZone(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            // EnterLevel Update
            if (nChangeType == EditData.UPDATE)
            {
                int nZoneID = (int)arrDatas[2];
                string szPermitLevel = (string)arrDatas[3];

                int nZoneGroupCount = dataMgr.GetZoneGroupCount();

                //ArrayList arZone = dataMgr.DataZones;
                //foreach(DataZone zone in arZone)
                for (int i = 0; i < nZoneGroupCount;i++ )
                {
                    ZoneGroup group = dataMgr.GetZoneGroup(i);

                    int nZoneCount = group.GetZoneCount();

                    for (int j = 0; j < nZoneCount; j++)
                    {
                        DataZone zone = group.GetZone(j);

                        if (zone.ID == nZoneID)
                        {
                            // 복사본을 만들어 레벨값을 세팅
                            DataZone tempZone = (DataZone)zone.Clone();
                            tempZone.RemoveAllPermitLevels();
                            if (szPermitLevel != null && szPermitLevel != "")
                            {
                                string[] permits = szPermitLevel.Split(',');
                                for (int k = 0; k < permits.Length; k++)
                                {
                                    int nLevel = 0;
                                    if (int.TryParse(permits[k], out nLevel))
                                    {
                                        tempZone.AddPermitLevel(nLevel);
                                    }
                                }
                            }

                            // 저장에 성공하는 경우 Zone에 저장한다.
                            if (DBZoneHelper.UpdateZoneLevel(dbMgr, tempZone))
                            {
                                zone.RemoveAllPermitLevels();
                                if (szPermitLevel != null && szPermitLevel != "")
                                {
                                    string[] permits = szPermitLevel.Split(',');
                                    for (int k = 0; k < permits.Length; k++)
                                    {
                                        int nLevel = 0;
                                        if (int.TryParse(permits[k], out nLevel))
                                        {
                                            zone.AddPermitLevel(nLevel);
                                        }
                                    }
                                }
                                return bytes;
                            }
                            break;
                        }
                    }
                }
            }
            return null;
        }

        public static byte[] ProcessChangeZoneGroup(ArrayList arrDatas, byte[] bytes)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 4)
                return null;

            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;

            if (nChangeType == EditData.UPDATE)
            {
                SqlConnection connection = null;

                try
                {
                    connection = dbMgr.Connect();

                    for (int i = 2; i < nDataCount - 1; i += 2)
                    {
                        int nZoneID = (int)arrDatas[i];
                        string strGroupName = (string)arrDatas[i + 1];

                        string strSQL = string.Format("Update Zone set ZoneGroupName = '{0}' where ID = {1}", strGroupName, nZoneID);
                        dbMgr.ExecuteSQL(strSQL, connection);

                        ZoneGroup group = dataMgr.FindZoneGroup(strGroupName);

                        if (group == null)
                        {
                            if (strGroupName == ZoneGroup.DefaultZoneGroup.GroupName ||
                                strGroupName == ZoneGroup.DefaultZoneGroup.ToString())
                            {
                                group = ZoneGroup.DefaultZoneGroup;

                                if (dataMgr.FindZoneGroup(group.GroupName) == null)
                                    dataMgr.AddZoneGroup(group);
                            }
                            else
                            {
                                group = new ZoneGroup(strGroupName);
                                dataMgr.AddZoneGroup(group);
                            }
                        }

                        DataZone zone = dataMgr.FindZone(nZoneID);

                        if (zone != null)
                            zone.ZoneGroup = group;
                    }

                    connection.Close();
                }
                catch (Exception)
                {
                    if (connection != null)
                        connection.Close();

                    return null;
                }
            }

            return bytes;
        }
    }

}
