using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;

namespace PSensorServer
{
    public class DBUtil
    {
        public static int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select MAX(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return 0;
            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        public static string GetKeyTimeString(DateTime dt)
        {
            string szResult = string.Format("{0}{1:D2}{2:D2}{3:D2}", dt.Day, dt.Hour, dt.Minute, dt.Second);
            return szResult;
        }

        public static int[] ToIntArray(long a, int n)
        {
            if (n == 3)
            {
                long temp = (long)(uint.MaxValue >> 12);
                int a1 = (int)(a & temp);
                int a2 = (int)((a >> 21) & temp);
                int a3 = (int)((a >> 42) & temp);
                return new int[] { a1, a2, a3 };
            }

            if (n == 2)
            {
                int a1 = (int)(a & (long)uint.MaxValue);
                int a2 = (int)(a >> 32);
                return new int[] { a1, a2 };
            }
            return null;
        }

        public static long ToLong(int a1, int a2)
        {
            long b = a2;
            b = b << 32;
            b = b | (uint)a1;
            return b;
        }

        // 상중하 1,20,1,20,1,20로 shift
        // 한개가 가지는 최대 크기는 +-1048575
        // 최상위 1bit는 남겨둠
        public static long ToLong(int a1, int a2, int a3)
        {
            long b = ((uint)a3 & (uint.MaxValue >> 12));
            b = b << 42;
            long aa = (long)((uint)a2 & (uint.MaxValue >> 12)) << 21;
            b = b | aa;
            b = b | ((uint)a1 & (uint.MaxValue >> 12));
            return b;
        }
    }
}
