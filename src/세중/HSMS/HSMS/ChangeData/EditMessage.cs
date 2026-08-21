using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace HSMS
{
    class EditMessage : ChangedData
    {
        public EditMessage()
        {            
        }

        private bool m_bChecked = false;
        public bool Checked
        {
            get { return m_bChecked; }
            set { m_bChecked = value; }
        }

        public override bool Update(DBConn conn)
        {
            int nSiteID = FormMain.Instance.SiteID;
            
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)ChangeDataType.SMSCONFIG);
            arrDatas.Add(ChangedData.UPDATE);
            arrDatas.Add(m_bChecked);
            arrDatas.Add(nSiteID);            
            bool bResult = false;
            try
            {
                NetworkManager netMgr = FormMain.Instance.NetMgr;
                byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                netMgr.Send(sendBytes, netMgr.ClientProvider);
                bResult = true;
            }
            catch(Exception)
            {
            }
            return bResult;
        }
        public override void AddToManager(IChangedDataManager mgr)
        {
            throw new NotImplementedException();
        }
    }
}
