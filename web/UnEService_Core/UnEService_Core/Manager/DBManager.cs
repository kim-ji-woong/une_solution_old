using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core.Manager
{
    public abstract class DBManager
    {
        public static string Host { get; set; }

        public static string ID { get; set; }

        public static string PW { get; set; }

        public static string CharSet { get; set; }

        public DateTime CreateTime { get; set; }

        public long TransactionKey { get; set; }

        public bool IsRemoved { get; set; }

        public abstract string BatchCommit();
        public abstract string BatchRollback();

        protected static bool IsSelectQuery(string strSQL)
        {
            strSQL = strSQL.Trim().ToLower();
            return strSQL.StartsWith("select") || strSQL.StartsWith("with");
        }

        protected static void AddNullData(List<string> datas)
        {
            datas.Add("~");
        }

        protected static void AddData(List<string> datas, object data)
        {
            // 데이터 로드 시 배열일 경우 처리
            // 바로 ToString을 하여 데이터 추가 시 예를 들어 System.Byte[] 식으로 추가가 되기 때문에 따로 처리
            if (data.GetType().IsArray)
            {
                Type dataType = data.GetType();
                data = Convert.ChangeType(data, dataType);
                int length = ((Array)data).Length;
                string tempStr = "";

                if (length > 0)
                {
                    object[] dataTemp = new object[length];
                    Array.Copy((Array)data, dataTemp, length);
                    tempStr = string.Join(",", dataTemp);
                }

                datas.Add("!" + tempStr);
            }
            else
            {
                datas.Add("!" + data.ToString());
            }
        }

        protected static string[] MakeSuccess(List<string> datas)
        {
            string[] results = null;

            if (datas == null)
            {
                results = new string[2];

                results[0] = "1";
                results[1] = "0";
            }
            else
            {
                int nDataCount = datas.Count + 2;
                results = new string[nDataCount];

                results[0] = "1";
                results[1] = datas.Count.ToString();

                for (int i = 2; i < nDataCount; i++)
                {
                    results[i] = datas[i - 2];
                }
            }

            return results;
        }
    }
}
