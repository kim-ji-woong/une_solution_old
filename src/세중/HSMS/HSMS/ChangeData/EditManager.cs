using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace HSMS
{
    public class EditManager : ChangedData
    {
        private int m_nSqlType = 0;
        public new int SQLType
        {
            get { return m_nSqlType; }
            set { m_nSqlType = value; }
        }

        private DataWorker m_Manager = null;
        public DataWorker Manager
        {
            get { return m_Manager; }
            set { m_Manager = value; }
        }

        public override bool Update(DBConn conn)
        {
            if (m_Manager == null)
                return false;

            int nSiteID = FormMain.Instance.SiteID;
            bool bResult = false;
            try
            {
                NetworkManager netMgr = FormMain.Instance.NetMgr;
                if (m_nSqlType == ChangedData.DELETE)
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.MANAGER);
                    arrDatas.Add(m_nSqlType);
                    arrDatas.Add(m_Manager.MemberID);
                    arrDatas.Add(nSiteID);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                    netMgr.Send(sendBytes, netMgr.ClientProvider);
                    bResult = true;
                }
                else if (m_nSqlType == ChangedData.INSERT)
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.MANAGER);
                    arrDatas.Add(m_nSqlType);
                    arrDatas.Add(m_Manager.MemberID);
                    arrDatas.Add(nSiteID);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                    netMgr.Send(sendBytes, netMgr.ClientProvider);
                    bResult = true;
                }                
            }
            catch (System.Exception)
            {
                
            }            
            return bResult;
        }
        
        public override void AddToManager(IChangedDataManager mgr)
        {

        } 

    }
}
