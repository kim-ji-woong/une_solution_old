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
    public class EditIgnoreDetect : EditData
    {
        public static byte[] ProcessChangeIgnoreDetect(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            int nChangeType = (int)arrDatas[1];
            return null;
        }
    }
}
