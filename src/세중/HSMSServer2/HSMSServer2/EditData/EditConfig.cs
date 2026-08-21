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
    public class EditSMSConfig :  EditData
    {
        public static byte[] ProcessChangeConfige(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];            
            bool bValue = (bool)arrDatas[2];

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;

            if( DBMessageHelper.UpdateSMSConfig(dbMgr, bValue))
            {  
                if (bValue == true)
                {
                    dataMgr.MessageChecked = true;
                }
                else
                {
                    dataMgr.MessageChecked = false;
                }
            }

            return bytes;
        }
    }
}
