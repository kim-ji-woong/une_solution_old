using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;

namespace HSMS
{
    class EditPermitLevel : ChangedData
    {
        private int m_nSQLType = 0;
        private DataZone m_zone = null;
        private VariousData<string> m_PermitLevel = null;

        public int ID
        {
            get { return m_zone == null ? -1 : m_zone.ID; }
        }
        public string PermitLevel
        {
            set { m_PermitLevel = new VariousData<string>(value); }
        }

        public new int SQLType
        {
            get { return m_nSQLType; }
            set { m_nSQLType = value; }
        }

        public DataZone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public EditPermitLevel()
        {            
        }

        public override bool Update(DBConn conn)
        {
            if (m_zone == null)
                return false; 
            try
            {
                NetworkManager netMgr = FormMain.Instance.NetMgr;
                //수정
                if (m_nSQLType == ChangedData.UPDATE)
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.ZONELEVEL);
                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(Zone.ID);
                    arrDatas.Add((string)m_PermitLevel.Data);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                    netMgr.Send(sendBytes, netMgr.ClientProvider); 


                }                
                return true;
            }
            catch (System.Exception)
            {                
            }            
            return false;
        }
        public override void AddToManager(IChangedDataManager mgr)
        {
            throw new NotImplementedException();
        }
    }
}
