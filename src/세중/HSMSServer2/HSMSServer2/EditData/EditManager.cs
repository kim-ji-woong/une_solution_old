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
    public class EditManager
    {
        public static byte[] ProcessChangeManager(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;

            if (nChangeType == EditData.UPDATE)
            {
            }

            else if (nChangeType == EditData.DELETE)
            {
                string szMemberID = (string)arrDatas[2];
                int nSiteID = (int)arrDatas[3];

                Manager mgr = dataMgr.GetManager(szMemberID);
                if (mgr != null)
                {
                    if (DBManagerHelper.DeleteManager(dbMgr, mgr))
                    {                        
                        dataMgr.RemoveManager(mgr);
                        return bytes;
                    }
                }               
            }

            else if (nChangeType == EditData.INSERT)
            {
                string szMemberID = (string)arrDatas[2];
                int nSiteID = (int)arrDatas[3];

                Manager mgr = dataMgr.GetManager(szMemberID);
                if (mgr == null)
                {
                    Manager newMgr = new Manager();
                    newMgr.MemberID = szMemberID;
                    newMgr.SiteID = nSiteID;

                    if (DBManagerHelper.AddManager(dbMgr, newMgr))
                    {
                        dataMgr.AddManager(newMgr);
                        ArrayList arData = new ArrayList();
                        arData.Add((int)ChangeDataType.MANAGER);
                        arData.Add(nChangeType);
                        arData.Add(newMgr.ID);
                        arData.Add(newMgr.MemberID);
                        arData.Add(newMgr.SiteID);
                        return ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arData);
                    }
                }
            }
            return null;
        }
    }
}
